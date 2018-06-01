using System;
using NAudio.Midi;

namespace Midiator.MidiTest
{
    public class MidiSender : IDisposable, IMidiSender
    {
        private int _absoluteTime = 0;
        private readonly int _channel = 1;
        private readonly MidiController _breathController;
        private readonly MidiOut _midiOut;

        public MidiSender(int deviceNumber, MidiController controller)
        {
            _breathController = controller;
            _midiOut = new MidiOut(deviceNumber);
        }

        public void SendSignal(int value)
        {
            var controlChangeEvent = new ControlChangeEvent(_absoluteTime, _channel, _breathController, value);
            _midiOut.Send(controlChangeEvent.GetAsShortMessage());
        }

        public void Dispose()
        {
            _midiOut?.Dispose();
        }
    }
}
