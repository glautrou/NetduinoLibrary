// Thank you to CW2 Stanislav "CW" Simicek

namespace Netduino.Components.Sensor.DHT
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using Microsoft.SPOT;
    using Microsoft.SPOT.Hardware;

    /// <summary>
    /// Encapsulates the common functionality of DHT sensors.
    /// </summary>
    public abstract class DhtSensor : ISensor, IDisposable
    {
        private bool disposed;

        private InterruptPort portIn;
        private TristatePort portOut;

        /// <summary>
        /// Humidity + Temperature from sensor
        /// </summary>
        public IOutput Output { get; set; }

        private long data;
        private long bitMask;
        private long lastTicks;
        private byte[] bytes = new byte[4];

        private AutoResetEvent dataReceived = new AutoResetEvent(false);

        // Instantiated via derived class
        protected DhtSensor(Cpu.Pin pin1, Cpu.Pin pin2, PullUpResistor pullUp)
        {
            Output = new Output();

            var resistorMode = (Port.ResistorMode)pullUp;

            portIn = new InterruptPort(pin2, false, resistorMode, Port.InterruptMode.InterruptEdgeLow);
            portIn.OnInterrupt += new NativeEventHandler(portIn_OnInterrupt);
            portIn.DisableInterrupt();  // Enabled automatically in the previous call

            portOut = new TristatePort(pin1, true, false, resistorMode);

            if (!CheckPins())
            {
                throw new InvalidOperationException("DHT sensor pins are not connected together.");
            }
        }

        /// <summary>
        /// Deletes an instance of the <see cref="DhtSensor"/> class.
        /// </summary>
        ~DhtSensor()
        {
            Dispose(false);
        }

        /// <summary>
        /// Releases resources used by this <see cref="DhtSensor"/> object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the resources associated with the <see cref="DhtSensor"/> object.
        /// </summary>
        /// <param name="disposing">
        /// <b>true</b> to release both managed and unmanaged resources;
        /// <b>false</b> to release only unmanaged resources.
        /// </param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected void Dispose(bool disposing)
        {
            if (!disposed)
            {
                try
                {
                    portIn.Dispose();
                    portOut.Dispose();
                }
                finally
                {
                    disposed = true;
                }
            }
        }

        /// <summary>
        /// Gets the start delay, in milliseconds.
        /// </summary>
        protected abstract int StartDelay { get; }

        /// <summary>
        /// Converts raw sensor data.
        /// </summary>
        /// <param name="data">The sensor raw data, excluding the checksum.</param>
        /// <remarks>
        /// If the checksum verification fails, this method is not called.
        /// </remarks>
        protected abstract void Convert(byte[] data);

        /// <summary>
        /// Retrieves measured data from the sensor.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the operation succeeds and the data is valid, otherwise <c>false</c>.
        /// </returns>
        public bool Read()
        {
            if (disposed)
            {
                throw new ObjectDisposedException("DHT Sensor");
            }

            // The 'bitMask' also serves as edge counter: data bit edges plus
            // extra ones at the beginning of the communication (presence pulse).
            bitMask = 1L << 41;

            data = 0;
            // lastTicks = 0; // This is not really needed, we measure duration
            // between edges and the first three values are ignored anyway.

            // Initiate communication
            portOut.Active = true;
            portOut.Write(false);       // Pull bus low
            Thread.Sleep(StartDelay);
            portIn.EnableInterrupt();   // Turn on the receiver
            portOut.Active = false;     // Release bus

            bool dataValid = false;

            // Now the interrupt handler is getting called on each falling edge.
            // The communication takes up to 5 ms, but the interrupt handler managed
            // code takes longer to execute than is the duration of sensor pulse
            // (interrupts are queued), so we must wait for the last one to finish
            // and signal completion. 20 ms should be enough, 50 ms is safe.
            if (dataReceived.WaitOne(50, false))
            {
                if (ValidateChecksum())
                {
                    dataValid = true;
                    Convert(bytes);
                }
                else
                    Debug.Print("DHT sensor data has invalid checksum.");
            }
            else
            {
                portIn.DisableInterrupt();  // Stop receiver
                Debug.Print("DHT sensor data timeout.");  // TODO: TimeoutException?
            }
            return dataValid;
        }

        /// <summary>
        /// Check that the checksum is valid
        /// </summary>
        private bool ValidateChecksum()
        {
            // TODO: Use two short-s ?
            bytes[0] = (byte)((data >> 32) & 0xFF);
            bytes[1] = (byte)((data >> 24) & 0xFF);
            bytes[2] = (byte)((data >> 16) & 0xFF);
            bytes[3] = (byte)((data >> 8) & 0xFF);

            byte checksum = (byte)(bytes[0] + bytes[1] + bytes[2] + bytes[3]);
            return checksum == (byte)(data & 0xFF);
        }

        // If the received data has invalid checksum too often, adjust this value
        // based on the actual sensor pulse durations. It may be a little bit
        // tricky, because the resolution of system clock is only 21.33 µs.
        private const long BitThreshold = 1050;

        private void portIn_OnInterrupt(uint pin, uint state, DateTime time)
        {
            var ticks = time.Ticks;
            if ((ticks - lastTicks) > BitThreshold)
            {
                // If the time between edges exceeds threshold, it is bit '1'
                data |= bitMask;
            }
            if ((bitMask >>= 1) == 0)
            {
                // Received the last edge, stop and signal completion
                portIn.DisableInterrupt();
                dataReceived.Set();
            }
            lastTicks = ticks;
        }

        // Returns true if the ports are wired together, otherwise false.
        private bool CheckPins()
        {
            Debug.Assert(portIn != null, "Input port should not be null.");
            Debug.Assert(portOut != null, "Output port should not be null.");
            Debug.Assert(!portOut.Active, "Output port should not be active.");

            portOut.Active = true;  // Switch to output
            portOut.Write(false);
            var expectedFalse = portIn.Read();
            portOut.Active = false; // Switch to input
            var expectedTrue = portIn.Read();
            return (expectedTrue && !expectedFalse);
        }
    }
}