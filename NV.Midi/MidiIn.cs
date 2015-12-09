using System;
using System.Runtime.InteropServices;

namespace NV.Midi
{
    public class MidiIn
    {
        public delegate void OnMessageEventHandler(object sender, MidiInEventArgs args);
        public event OnMessageEventHandler Message;

        private readonly WinMm.WinMm.MidiDelegate _midiInProc;
        private int _handle;

        public MidiIn()
        {
            _midiInProc = MidiProc;
            _handle = 0;
        }

        public void Close()
        {
            var error = WinMm.WinMm.midiInClose(_handle);
            _handle = 0;

            switch (error)
            {
                case WinMm.WinMm.MMSYSERR_NOERROR:
                    return;
                case WinMm.WinMm.MMSYSERR_INVALHANDLE:
                    throw new Exception("The specified device handle is invalid.");
                case WinMm.WinMm.MMSYSERR_NOMEM:
                    throw new Exception("The system is unable to allocate or lock memory.");
                default:
                    throw new Exception("Unknown Exception was throwen");
            }
        }

        public void Open(int id)
        {
            var error = WinMm.WinMm.midiInOpen(ref _handle, id, _midiInProc, 0, WinMm.WinMm.CALLBACK_FUNCTION);

            switch (error)
            {
            case WinMm.WinMm.MMSYSERR_NOERROR:
                return;
            case WinMm.WinMm.MMSYSERR_ALLOCATED:
                throw new Exception("The specified resource is already allocated.");
            case WinMm.WinMm.MMSYSERR_BADDEVICEID:
                throw new Exception("The specified device identifier is out of range.");
            case WinMm.WinMm.MMSYSERR_INVALFLAG:
                throw new Exception("The flags specified by dwFlags are invalid.");
            case WinMm.WinMm.MMSYSERR_INVALPARAM:
                throw new Exception("The specified pointer or structure is invalid.");
            case WinMm.WinMm.MMSYSERR_NOMEM:
                throw new Exception("The system is unable to allocate or lock memory.");
            default:
                throw new Exception("Unknown Exception was throwen");
            }
        }

        public void Start()
        {
            var error = WinMm.WinMm.midiInStart(_handle);

            switch (error)
            {
                case WinMm.WinMm.MMSYSERR_NOERROR:
                    return;
                case WinMm.WinMm.MMSYSERR_INVALHANDLE:
                    throw new Exception("The specified device handle is invalid.");
                default:
                    throw new Exception("Unknown Exception was throwen");
            }
        }

        public void Stop()
        {
            var error = WinMm.WinMm.midiInStop(_handle);

            switch (error)
            {
                case WinMm.WinMm.MMSYSERR_NOERROR:
                    return;
                case WinMm.WinMm.MMSYSERR_INVALHANDLE:
                    throw new Exception("The specified device handle is invalid.");
                default:
                    throw new Exception("Unknown Exception was throwen");
            }
        }

        private void MidiProc(int hMidiIn,
            int wMsg,
            int dwInstance,
            int dwParam1,
            int dwParam2)
        {
            if (wMsg != WinMm.WinMm.MM_MIM_DATA)
                return;

            var param1 = (dwParam1 >> 8) & 0xFF;
            var param2 = (dwParam1 >> 16) & 0xFF;

            if(Message != null)
                Message(this, new MidiInEventArgs(param1, param2));
        }

        public int Count()
        {
            return WinMm.WinMm.midiInGetNumDevs();
        }

        public string GetDeviceName(int id)
        {
            var caps = new WinMm.WinMm.MIDIINCAPS();
            var error = WinMm.WinMm.midiInGetDevCaps(id, ref caps, Marshal.SizeOf(caps));

            switch (error)
            {
                case WinMm.WinMm.MMSYSERR_NOERROR:
                    return caps.szPname;
                case WinMm.WinMm.MMSYSERR_BADDEVICEID:
                    throw new Exception("The specified device identifier is out of range.");
                case WinMm.WinMm.MMSYSERR_INVALPARAM:
                    throw new Exception("The specified pointer or structure is invalid.");
                case WinMm.WinMm.MMSYSERR_NODRIVER:
                    throw new Exception("The driver is not installed.");
                case WinMm.WinMm.MMSYSERR_NOMEM:
                    throw new Exception("The system is unable to allocate or lock memory.");
                default:
                    throw new Exception("Unknown Exception was throwen");
            }
        }
    }
}
