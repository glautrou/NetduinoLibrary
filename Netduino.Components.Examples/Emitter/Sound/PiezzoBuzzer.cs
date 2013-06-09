using System;
using Microsoft.SPOT;
using System.Threading;
using SecretLabs.NETMF.Hardware.NetduinoPlus;

namespace Netduino.Components.Examples.Emitter.Sound
{
    /// <summary>
    /// Example of the piezzo buzzer
    /// Wiring:
    /// +: Digital I/O 5
    /// -: GND
    /// </summary>
    public class PiezzoBuzzer
    {
        private static readonly Microsoft.SPOT.Hardware.Cpu.Pin Pin = Pins.GPIO_PIN_D5;
        private static readonly string Melody = "C1C1C1g1a1a1g2E1E1D1D1C2";

        internal static void Run()
        {
            var speaker = new Netduino.Components.Emitter.Sound.PiezoBuzzer(Pin);

            speaker.OnSongStart += speaker_OnSongStart;
            speaker.OnSongFinished += speaker_OnSongFinished;

            speaker.PlaySong(Melody);
            Thread.Sleep(Timeout.Infinite);
        }

        static void speaker_OnSongStart()
        {
            Debug.Print("Song start...");
        }

        static void speaker_OnSongFinished()
        {
            Debug.Print("Song finished.");
        }
    }
}
