// Thank you to Giuliano for this file

using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace Netduino.Components.Sensor.DHT.RHT03
{
    /// <summary>
    /// Represents an instance of the RHT03 sensor.
    /// </summary>
    /// <remarks>
    /// Humidity measurement range 0 - 100%, accuracy ±2-5%.
    /// Temperature measurement range -40 - 80°C, accuracy ±0.5°C.
    /// </remarks>
    public class RHT03Sensor : DhtSensor
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="RHT03Sensor"/> class.
        /// </summary>
        /// <param name="pin1">The identifier for the sensor's data bus port.</param>
        /// <param name="pin2">The identifier for the sensor's data bus port.</param>
        /// <param name="pullUp">The pull-up resistor type.</param>
        /// <remarks>
        /// The ports identified by <paramref name="pin1"/> and <paramref name="pin2"/>
        /// must be wired together.
        /// </remarks>
        public RHT03Sensor(Cpu.Pin pin1, Cpu.Pin pin2, PullUpResistor pullUp)
            : base(pin1, pin2, pullUp)
        {}

        protected override int StartDelay { get { return 2; }}   // At least 1 ms

        protected override void Convert(byte[] data)
        {
            Debug.Assert(data != null);
            Debug.Assert(data.Length == 4);

            Output.SetData(data);
        }
    }
}