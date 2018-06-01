using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Midiator.Gui
{
    public partial class MainWindow : Form
    {
        private IObservable<EventPattern<EventArgs>> _startStopButtonClick;
        private readonly IObservable<EventPattern<EventArgs>> _refreshButtonClick;
        private readonly IDisposable _refreshButtonCallback;
        private IDisposable _startStopButtonCallback;
        private bool _isPlaying = false;
        public MainWindow()
        {
            InitializeComponent();
            _startStopButtonClick = CreateButtonClickObservable(StartStopButton);
            _refreshButtonClick = CreateButtonClickObservable(RefreshPortsButton);
            _refreshButtonCallback = _refreshButtonClick.Subscribe(OnRefreshButtonClick);
            _startStopButtonCallback = _startStopButtonClick.Subscribe(OnStartStopButtonClick);
            RefreshPortsButton.PerformClick();
        }

        public void OnStartStopButtonClick(EventPattern<EventArgs> x)
        {
            var button = x.Sender as Button;
            if(button == null) throw new ArgumentException("Expected sender to be Button.", nameof(x));
            _isPlaying = !_isPlaying;
            button.Text = _isPlaying ? "Stop" : "Start";
        }

        private void OnRefreshButtonClick(EventPattern<EventArgs> x)
        {
            var currentPorts = GetPorts();
            AddMissing(SerialPortComboBox.Items, currentPorts);
            RemoveOld(SerialPortComboBox.Items, currentPorts);
        }

        private void RemoveOld(ComboBox.ObjectCollection comboboxPorts, string[] currentPorts)
        {
            var portsToRemove = comboboxPorts.Cast<string>().Where(x => !currentPorts.Contains(x));
            portsToRemove.ForEach(comboboxPorts.Remove);
        }

        private void AddMissing(ComboBox.ObjectCollection comboboxPorts, string[] currentPorts)
        {
            var portsToAdd = currentPorts.Where(port => !comboboxPorts.Contains(port)).ToArray();
            comboboxPorts.AddRange(portsToAdd);
        }

        private static IObservable<EventPattern<EventArgs>> CreateButtonClickObservable(Button button)
        {
            return Observable.FromEventPattern<EventHandler, EventArgs>(
                x => x.Invoke,
                x => button.Click += x,
                x => button.Click -= x);
        }

        private string[] GetPorts() => SerialPort.GetPortNames();
    }
}
