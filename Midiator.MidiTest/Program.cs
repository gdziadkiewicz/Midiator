using System;
using System.IO.Ports;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using NAudio.Midi;

namespace Midiator.MidiTest
{
    public class Program
    {
        private static readonly int _minVal = -2500;
        private static readonly int MaxVal = 5000;
        private static MidiSender _midiSender;

        public static void Main(string[] args)
        {
            _midiSender = new MidiSender(1, MidiController.Modulation);
            using (var serialPort = new SerialPort("COM3", 38400, Parity.None, 8, StopBits.One))
            {
                serialPort.Open();
                var propChanged = FromEventPattern(serialPort);
                var subscribe = propChanged.Subscribe(OnNext);

                while (!Console.KeyAvailable)
                {
                }
                
                subscribe.Dispose();
                _midiSender.Dispose();
            }
        }

        private static void OnNext(EventPattern<SerialDataReceivedEventArgs> x)
        {
            var buffer = new byte[12];
            ((SerialPort) x.Sender).Read(buffer,0,12);
            int ax, ay, az, gx, gy, gz;
            ax = BitConverter.ToInt16(buffer, 0);
            ay = BitConverter.ToInt16(buffer, 2);
            az = BitConverter.ToInt16(buffer, 4);
            gx = BitConverter.ToInt16(buffer, 6);
            gy = BitConverter.ToInt16(buffer, 8);
            gz = BitConverter.ToInt16(buffer, 10);

            int limitedValue = ay <= _minVal? 0: ay>=MaxVal? MaxVal: ay-_minVal;
            double d = limitedValue / (double)MaxVal;
            var value = (int)(d * 127);
            var limitedOut = value < 0 ? 0 : value > 127 ? 127 : value;
            Console.WriteLine(limitedOut);
            _midiSender.SendSignal(limitedOut);
        }

        private static IObservable<EventPattern<SerialDataReceivedEventArgs>> FromEventPattern(SerialPort serialPort)
        {
            return Observable.FromEventPattern<SerialDataReceivedEventHandler, SerialDataReceivedEventArgs>
            (
                h => serialPort.DataReceived += h,
                h => serialPort.DataReceived -= h);
        }
    }
}
