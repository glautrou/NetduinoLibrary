using System;
using Microsoft.SPOT;
using System.Threading;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using System.Runtime.CompilerServices;

// Thank you to Chris Walker

namespace Netduino.Components.Emitter.Sound
{
    /// <summary>
    /// Piezzo buzzer
    /// </summary>
    public class PiezoBuzzer : IBuzzer
    {
        private static readonly int BeatsPerMinute = 90;
        private static readonly int BeatTimeInMilliseconds = 60000 / BeatsPerMinute; // 60,000 milliseconds per minute
        private static readonly int PauseTimeInMilliseconds = (int)(BeatTimeInMilliseconds * 0.1);

        /// <summary>
        /// Song to play
        /// </summary>
        private Song Song { get; set; }

        /// <summary>
        /// Speaker used to play the song
        /// </summary>
        private PWM speaker;

        /// <summary>
        /// Instantiate the piezzo buzzer
        /// </summary>
        /// <param name="pin"></param>
        public PiezoBuzzer(Microsoft.SPOT.Hardware.Cpu.Pin pin)
        {
            speaker = new PWM(pin);
        }

        /// <summary>
        /// Play the song with this buzzer
        /// </summary>
        /// <param name="melody">Melody to be played</param>
        public void PlaySong(string melody)
        {
            if (disposed)
            {
                throw new ObjectDisposedException("Piezzo buzzer");
            }

            Song = new Song(melody);
            if (OnSongStart != null) OnSongStart();
            Song.Play(this);
            if (OnSongFinished != null) OnSongFinished();
        }

        /// <summary>
        /// Play a note
        /// </summary>
        public void PlayNote(uint period, uint duration, int beatCount)
        {
            if (disposed)
            {
                throw new ObjectDisposedException("Piezzo buzzer");
            }

            // Play the note for the desired number of beats
            speaker.SetPulse(period, duration);
            Thread.Sleep(BeatTimeInMilliseconds * beatCount - PauseTimeInMilliseconds);

            // Pause for 1/10th of a beat in between every note.
            speaker.SetDutyCycle(0);
            Thread.Sleep(PauseTimeInMilliseconds);
        }


        #region Dispose
        private bool disposed;
        /// <summary>
        /// Deletes an instance of the <see cref="PiezzoBuzzer"/> class.
        /// </summary>
        ~PiezoBuzzer()
        {
            Dispose(false);
        }

        /// <summary>
        /// Releases resources used by this <see cref="PiezzoBuzzer"/> object.
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
                    speaker.Dispose();
                }
                finally
                {
                    disposed = true;
                }
            }
        }
        #endregion


        #region Events
        public delegate void SongStartDelegate();
        /// <summary>
        /// Raised when the song starts
        /// </summary>
        public event SongStartDelegate OnSongStart;

        public delegate void SongFinishedDelegate();
        /// <summary>
        /// Raised when the song finished
        /// </summary>
        public event SongFinishedDelegate OnSongFinished;
        #endregion
    }
}
