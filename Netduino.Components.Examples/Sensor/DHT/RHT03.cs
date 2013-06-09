using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using System.Threading;
using Netduino.Components.Sensor.DHT;

namespace Netduino.Components.Examples.Sensor.DHT
{
    public class RHT03
    {
        private static readonly Cpu.Pin PinOut = Pins.GPIO_PIN_D0;
        private static readonly Cpu.Pin PinIn = Pins.GPIO_PIN_D1;
        private static readonly PullUpResistor Resistor = PullUpResistor.Internal;
        private static readonly int SensorOutputRefreshDelay = 5000;    // in ms

        internal static void Run()
        {
            var sensor = new Netduino.Components.Sensor.DHT.RHT03.RHT03Sensor(PinOut, PinIn, Resistor);
            Thread.Sleep(2000);

            while (true)
            {
                if (sensor.Read())
                {
                    var output = (Netduino.Components.Sensor.DHT.Output)sensor.Output;
                    var temperature = output.TemperatureInCelcius.ToString("F1");
                    var humidity = output.Humidity.ToString("F1");
                    Debug.Print("Temperature: " + temperature + "°C, Humidity: " + humidity + "%");
                }

                Thread.Sleep(SensorOutputRefreshDelay);
            }
        }
    }
}
