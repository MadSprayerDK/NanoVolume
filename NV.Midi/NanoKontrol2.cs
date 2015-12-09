using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NV.Midi
{
    public class NanoKontrol2
    {
        public delegate void OnSliderMoveEventDelegate(object sender, MidiInEventArgs args);
        public event OnSliderMoveEventDelegate SliderMove;

        public delegate void OnKnobMoveEventDelegate(object sender, MidiInEventArgs args);
        public event OnKnobMoveEventDelegate KnobMove;

        public delegate void OnSoloPressEventDelegate(object sender, MidiInEventArgs args);
        public event OnSoloPressEventDelegate SoloPressed;

        public delegate void OnMutePressEventDelegate(object sender, MidiInEventArgs args);
        public event OnMutePressEventDelegate MutePressed;

        public delegate void OnNextPressedEventDelegate(object sender, MidiInEventArgs args);
        public event OnNextPressedEventDelegate NextPressed;

        public delegate void OnPrevPressedEventDelegate(object sender, MidiInEventArgs args);
        public event OnPrevPressedEventDelegate PrevPressed;

        public delegate void OnPausePressEventDelegate(object sender, MidiInEventArgs args);
        public event OnPausePressEventDelegate PausePressed;

        public delegate void OnPlayPressEventDelegate(object sender, MidiInEventArgs args);
        public event OnPlayPressEventDelegate PlayPressed;

        private readonly MidiIn _midi;
        private readonly int _midiId;

        public NanoKontrol2()
        {
            _midi = new MidiIn();

            _midi.Message += midi_Message;

            for (var i = 0; i < _midi.Count(); i++)
            {
                var name = _midi.GetDeviceName(i);

                if(name.Contains("nanoKONTROL2"))
                {
                    _midiId = i;
                    break;
                }
            }
        }

        private void midi_Message(object sender, MidiInEventArgs args)
        {
            var value = (args.Value*100)/127;
            if (args.Id >= 0 && args.Id <= 7)
                SliderMove?.Invoke(this, new MidiInEventArgs(args.Id, value));
            else if(args.Id >= 16 && args.Id <= 23)
                KnobMove?.Invoke(this, new MidiInEventArgs(args.Id - 16, value));
            else if(args.Id >= 32 && args.Id <= 39)
                SoloPressed?.Invoke(this, new MidiInEventArgs(args.Id - 32, value));
            else if (args.Id >= 48 && args.Id <= 55)
                MutePressed?.Invoke(this, new MidiInEventArgs(args.Id - 48, value));
            else if(args.Id == 43)
                PrevPressed?.Invoke(this, new MidiInEventArgs(args.Id, value));
            else if(args.Id == 44)
                NextPressed?.Invoke(this, new MidiInEventArgs(args.Id, value));
            else if(args.Id == 42)
                PausePressed?.Invoke(this, new MidiInEventArgs(args.Id, value));
            else if (args.Id == 41)
                PlayPressed?.Invoke(this, new MidiInEventArgs(args.Id, value));
        }

        public void Start()
        {
            _midi.Open(_midiId);
            _midi.Start();
        }

        public void Stop()
        {
            _midi.Stop();
            _midi.Close();
        }
    }
}
