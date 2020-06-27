using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ArduinoSerial
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SerialPort _serial = new SerialPort();

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }

        public async Task<string> WaitSerialData()
        {
            byte[] buffer = new byte[1];
            string result = string.Empty;

            while (true)
            {
                await _serial.BaseStream.ReadAsync(buffer, 0, 1);
                result += _serial.Encoding.GetString(buffer);

                if (result.EndsWith(_serial.NewLine))
                {
                    result = result.Substring(0, result.Length - _serial.NewLine.Length);
                    result.TrimEnd('\r', '\n');
                    Debug.Write(string.Format("Data: {0}", result));
                    result += "\r\n";
                    return result;
                }
            }
        }

        public async Task<bool> SendSerialData(string data, bool ack)
        {
            var bytes = ASCIIEncoding.ASCII.GetBytes(data);
            await _serial.BaseStream.WriteAsync(bytes, 0, bytes.Length);

            if (!ack)
            {
                return true;
            }

            var result = await WaitSerialData();
            return result == "ACK";

        }

        private void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() => Connect());
        }
        private async Task Connect()
        {
            SetConnectStatus("........", Brushes.White);

            try
            {
                if (_serial.IsOpen)
                    _serial.Close();

                _serial.PortName = "COM6";
                _serial.BaudRate = 115200;
                _serial.Open();


                string readedString = await WaitSerialData();

                if (readedString.StartsWith("c:0"))
                {
                    SetConnectStatus("Found", Brushes.Green);
                    await SendSerialData(">", false);
                }
                else
                {
                    SetConnectStatus("NOT found", Brushes.Red);
                }
            }
            catch (Exception ex)
            {
                SetConnectStatus(ex.Message, Brushes.Red);
            }
        }

        private void BtnSendOn_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() => LedOn());
        }
        private async Task LedOn()
        {
            await SendSerialData("1", true);
        }


        private void BtnSendOff_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() => LedOff());
        }
        private async Task LedOff()
        {
            await SendSerialData("0", true);
        }



        private void SetConnectStatus(string status, Brush color)
        {
            this.Dispatcher.Invoke(() =>
            {
                TxtConnectResult.Text = status;
                TxtConnectResult.Background = color;
            });
        }




        private async void BtnReceive_Click(object sender, RoutedEventArgs e)
        {
            await SendSerialData("2", false);
            _serial.DataReceived += _serial_DataReceived;
        }
        private void _serial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string data = sp.ReadExisting();
            AddReceivedData(data);

        }
        private void AddReceivedData(string data)
        {
            this.Dispatcher.Invoke(() =>
            {
                TxtReceive.Text += Environment.NewLine + data;
            });
        }

    }
}
