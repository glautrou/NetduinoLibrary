using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using System.Threading;

namespace Netduino.Components.Examples.Playground
{
    public class BlinkIntegratedLED
    {
        public static void Run()
        {
            var led = new OutputPort(Pins.ONBOARD_LED, false);
            var button = new InputPort(Pins.ONBOARD_SW1, false, Port.ResistorMode.Disabled);
            var buttonState = false;

            while (true)
            {
                buttonState = button.Read();
                //led.Write(!buttonState);
                if (buttonState)
                {
                    Debug.Print("Pressed");
                    led.Write(true);
                    Thread.Sleep(500);
                    led.Write(false);
                    Thread.Sleep(250);
                }
            }
        }
    }
}
