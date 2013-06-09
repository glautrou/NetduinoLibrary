using System;
using Microsoft.SPOT;

namespace Netduino.Components.Emitter.Sound
{
    public interface IBuzzer : IDisposable
    {
        void PlaySong(string melody);
        void PlayNote(uint period, uint duration, int beatCount);
    }
}
