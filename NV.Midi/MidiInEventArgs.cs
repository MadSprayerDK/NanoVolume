using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NV.Midi
{
    public class MidiInEventArgs : EventArgs
    {
        public MidiInEventArgs(int id, int value)
        {
            Id = id;
            Value = value;
        }

        public int Id { private set; get; }
        public int Value { private set; get; }
    }
}
