using System;
using Microsoft.SPOT;
using System.Collections;
using System.Threading;

namespace Netduino.Components.Emitter.Sound
{
    /// <summary>
    /// The song that should be played by the buzzer
    /// </summary>
    public class Song
    {
        /// <summary>
        /// Store the notes on the music scale and their associated pulse lengths
        /// </summary>
        private Hashtable scale;

        /// <summary>
        /// Melody of the song (letter of note followed by length of note)
        /// </summary>
        public string Melody { get; private set; }

        /// <summary>
        /// Initialize the song
        /// </summary>
        /// <param name="melody">Melody of the song. <example>Example: C1C1C1g1a1a1g2E1E1D1D1C2</example> </param>
        public Song(string melody)
        {
            InitNotes();
            Melody = melody;
        }

        /// <summary>
        /// Initialize the notes and their associated pulse length
        /// </summary>
        private void InitNotes()
        {
            scale = new Hashtable();

            // low octave
            scale.Add("c", 1915u);
            scale.Add("d", 1700u);
            scale.Add("e", 1519u);
            scale.Add("f", 1432u);
            scale.Add("g", 1275u);
            scale.Add("a", 1136u);
            scale.Add("b", 1014u);
            // high octave
            scale.Add("C", 956u);
            scale.Add("D", 851u);
            scale.Add("E", 758u);
            // silence ("hold note")
            scale.Add("h", 0u);
        }

        /// <summary>
        /// Play the song
        /// </summary>
        /// <param name="speaker">Speaker to use</param>
        internal void Play(IBuzzer speaker)
        {
            // Interpret and play the song
            for (int i = 0; i < Melody.Length; i += 2)
            {
                // Extract each note and its length in beats
                var note = Melody.Substring(i, 1);
                var beatCount = int.Parse(Melody.Substring(i + 1, 1));

                // Look up the note duration
                var noteDuration = (uint)scale[note];   // in ms

                // Play the note
                speaker.PlayNote(noteDuration * 2, noteDuration, beatCount);
            }
        }
    }
}
