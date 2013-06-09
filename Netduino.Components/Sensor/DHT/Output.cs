using System;
using Microsoft.SPOT;

namespace Netduino.Components.Sensor.DHT
{
    /// <summary>
    /// Output results from the DHT sensor
    /// </summary>
    public class Output : IOutput
    {
        /// <summary>Degrees in Celcius</summary>
        public float TemperatureInCelcius { get; private set; }

        /// <summary>Degrees in Fahrenheit</summary>
        public float TemperatureInFahrenheit { get { return TemperatureInCelcius * 1.8f + 32; } }

        /// <summary>Humidity</summary>
        public double Humidity { get; private set; }

        /// <summary>
        /// Set data received from the sensor
        /// </summary>
        public void SetData(byte[] data)
        {
            SetHumidity(data);
            SetTemperature(data);
        }

        /// <summary>
        /// Set the humidity received from sensor
        /// </summary>
        private void SetHumidity(byte[] data)
        {
            // The first byte is integral, the second decimal part
            Humidity = ((data[0] << 8) | data[1]) * 0.1F;
        }

        /// <summary>
        /// Set the temperature received from sensor
        /// </summary>
        private void SetTemperature(byte[] data)
        {
            var temp = (((data[2] & 0x7F) << 8) | data[3]) * 0.1F;
            TemperatureInCelcius = (data[2] & 0x80) == 0 ? temp : -temp; // MSB = 1 means negative
        }
    }
}
