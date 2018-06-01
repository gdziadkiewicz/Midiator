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
            var line = ((SerialPort) x.Sender).ReadLine();
            int ax, ay, az, gx, gy, gz;
            try
            {
                var matchCollection = line.Split('\t').Skip(1).ToList();
                ax = Int32.Parse(matchCollection[0]);
                ay = Int32.Parse(matchCollection[1]);
                az = Int32.Parse(matchCollection[2]);
                gx = Int32.Parse(matchCollection[3]);
                gy = Int32.Parse(matchCollection[4]);
                gz = Int32.Parse(matchCollection[5]);
            }
            catch
            {
                return;
            }
            finally
            {
                Console.WriteLine(line);
            }

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
