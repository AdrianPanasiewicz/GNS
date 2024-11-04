using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using SerialCom.DataModel;
using System.Collections.Concurrent;
using GroundControlSystem.TelemetryProcessing;
using System.Windows.Forms;
using System.Drawing;
using GNS;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace SerialCom
{
    public class LoRaSerialReader
    {
        
        private SerialPort _serialPort;
        private string _portName;
        private int _baudRate;
        private bool _continue;
        
        // ReadMe
        // 
        // Tutaj znajduje się zmienna przechowująca otrzymane dane lotu.
        // Na dole pliku znajdują się dwie funkcje pozwalające przetłumaczyć wiersz z tej listy na obiekt TelemetryData,
        // który powinien być dla Was użyteczny.

        // Żeby skorzystać z LoRaSerialReader po utworzeniu obiektu należy najpierw wywołać funkcję Init(), a następnie Run()
        // (na potrzeby testów ustawione jest, że czyta ona 50 razy dane z portu).
        //
        // W konsoli powinny pokazać się zapytania o konfigurację - możecie je zostawić lub spróbować przenieść do okienka dialogowego dla użytkownika.
        //
        // Wątek który czyta dane z portu cały czas dodaje otrzymaną wiadomość na ostatnie miejsce na liście.
        //
        // Żeby zatrzymać działanie LoRaSerialReader należy wywołać funkcję Stop() a po niej Dispose() (oczywiście najpierw trzeba usunąć warunek if z funkcji Run()
        // (while myślę, że może zostać puste i prawdziwe zgodnie z warunkiem który tam jest, zapobiega to zakończeniu wątku).
        //

        private List<string> _receivedTelemetry;
        public List<string> ReceivedTelemetry { get => _receivedTelemetry; }
        private string _partialMessage;

        private Thread serialThread;
        
        public delegate void DataReceivedEventHandler(object source, EventArgs e);

        public event DataReceivedEventHandler DataReceived;

        public LoRaSerialReader() 
        {
            serialThread = new Thread(ReadPort);
            _serialPort = new SerialPort();
            _receivedTelemetry = new List<string>();
            _partialMessage = string.Empty;
        }

        public void Init() 
        {       
            _serialPort.PortName = _portName;
            _serialPort.BaudRate = _baudRate;
            _serialPort.ReadTimeout = 2000;
            _serialPort.WriteTimeout = 2000;
        }

        public void Run() 
        {
            AttemptPortAccess();

            while (_continue)
            {
                
            }
            serialThread.Join();
            _serialPort.Close();
            Dispose();
        }

        public void ReadPort()
        {
            Thread.Sleep(100);

            bool skipFirstRead = true;
            string currentData = "";

            while (_continue)
            {
                try
                {
                    string message = _serialPort.ReadLine().Trim();

                    if (skipFirstRead)
                    {
                        skipFirstRead = false;
                        continue;
                    }

                    if (message.StartsWith("+TEST: LEN:"))
                    {
                        var len = message.Split(',')[0].Replace("+TEST: LEN:", "").Trim();
                        var rssi = message.Split(',')[1].Replace("RSSI:", "").Trim();
                        var snr = message.Split(',')[2].Replace("SNR:", "").Trim();

                        currentData = $"{len};{rssi};{snr};";
                    }
                    else if (message.StartsWith("+TEST: RX"))
                    {
                        string rxData = message.Replace("+TEST: RX \"", "").TrimEnd('"');
                        string decodedData = HexToString(rxData);

                        currentData += decodedData;
                        _receivedTelemetry.Add(currentData);

                        currentData = "";
                    }
                }
                catch (TimeoutException)
                {
                    MessageBox.Show("No data received within the timeout period.","Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _continue = false;
                }
            }
        }

        private string HexToString(string hex)
        {
            hex = hex.Replace(" ", "");
            var bytes = new byte[hex.Length / 2];

            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }

            return System.Text.Encoding.UTF8.GetString(bytes);
        }

        private void AttemptPortAccess()
        {
            bool portAvailable = false;
            int attempts = 5;

            for (int i = 0; i < attempts; i++)
            {
                try
                {
                    _serialPort.Open();
                    _continue = true;
                    serialThread.Start();

                    portAvailable = true;

                    MessageBox.Show("SerialCom started successfully","LoRa Config", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                }
                catch (UnauthorizedAccessException)
                {
                    Thread.Sleep(1000);
                }
            }

            if (!portAvailable)
            {
                MessageBox.Show("Port is busy...\nUnable to connect after 5 attempts", "LoRa Config", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void Stop() 
        {
            _continue = false;
        }

        private void Dispose()
        {            
            if (_serialPort != null) { _serialPort.Dispose(); }
        }

        public TelemetryData ToTelemetryData()
        {
            if (!_receivedTelemetry.Any())
            {
                throw new InvalidOperationException("No telemetry data available.");
            }

            string lastMessage = _receivedTelemetry.Last();
            var telemetryData = new TelemetryData();

            var dataParts = lastMessage.Split(';');

            telemetryData.LoRa.MsgLength = double.Parse(dataParts[0], CultureInfo.InvariantCulture);
            telemetryData.LoRa.RSSI = double.Parse(dataParts[1], CultureInfo.InvariantCulture);
            telemetryData.LoRa.SNR = double.Parse(dataParts[2], CultureInfo.InvariantCulture);

            telemetryData.Time.TimeStamp = DateTime.ParseExact(dataParts[3], "H:mm:ss", CultureInfo.InvariantCulture);

            telemetryData.IMU.AccX = double.Parse(dataParts[4], CultureInfo.InvariantCulture);
            telemetryData.IMU.AccY = double.Parse(dataParts[5], CultureInfo.InvariantCulture);
            telemetryData.IMU.AccZ = double.Parse(dataParts[6], CultureInfo.InvariantCulture);
            telemetryData.IMU.GyroX = double.Parse(dataParts[7], CultureInfo.InvariantCulture);
            telemetryData.IMU.GyroY = double.Parse(dataParts[8], CultureInfo.InvariantCulture);
            telemetryData.IMU.GyroZ = double.Parse(dataParts[9], CultureInfo.InvariantCulture);
            telemetryData.IMU.MagX = double.Parse(dataParts[10], CultureInfo.InvariantCulture);
            telemetryData.IMU.MagY = double.Parse(dataParts[11], CultureInfo.InvariantCulture);
            telemetryData.IMU.MagZ = double.Parse(dataParts[12], CultureInfo.InvariantCulture);
            telemetryData.IMU.Heading = double.Parse(dataParts[13], CultureInfo.InvariantCulture);
            telemetryData.IMU.Pitch = double.Parse(dataParts[14], CultureInfo.InvariantCulture);
            telemetryData.IMU.Roll = double.Parse(dataParts[15], CultureInfo.InvariantCulture);

            telemetryData.Baro.AccZInertial = double.Parse(dataParts[16], CultureInfo.InvariantCulture);
            telemetryData.Baro.VerticalVelocity = double.Parse(dataParts[17], CultureInfo.InvariantCulture);
            telemetryData.Baro.Pressure = double.Parse(dataParts[18], CultureInfo.InvariantCulture);
            telemetryData.Baro.Altitude = double.Parse(dataParts[19], CultureInfo.InvariantCulture);

            telemetryData.GPS.Latitude = double.Parse(dataParts[20], CultureInfo.InvariantCulture);
            telemetryData.GPS.Longitude = double.Parse(dataParts[21], CultureInfo.InvariantCulture);

            return telemetryData;
        }

        protected virtual void OnDataReceived()
        {
            if (DataReceived != null)
            {
                DataReceived(this, EventArgs.Empty);
            }
        }

        public void ShowPortSelection()
        {
            Form portForm = new Form
            {
                Text = "LoRa Config",
                MaximizeBox = false,
                MinimizeBox = false,
                ClientSize = new Size(380, 150)
            };

            Label portLabel = new Label
            {
                Text = "Port:",
                Location = new Point(10, 10),
                BackColor = Color.Transparent
            };

            ComboBox portComboBox = new ComboBox();
            string[] availablePorts = SerialPort.GetPortNames();
            if (availablePorts.Length == 0)
            {
                portComboBox.Items.Add("No serial ports found");
                portComboBox.SelectedIndex = 0;
                portComboBox.Enabled = false;
            }
            else
            {               
                portComboBox.Items.AddRange(availablePorts);
                portComboBox.Enabled = true;
            }
            portComboBox.Location = new Point(10, 40);
            portComboBox.Width = 150;

            Label baudLabel = new Label();
            baudLabel.Text = "Baud Rate:";
            baudLabel.Location = new Point(200, 10);

            ComboBox baudComboBox = new ComboBox();
            baudComboBox.Items.AddRange(new object[] { 9600 });
            baudComboBox.Location = new Point(200, 40);
            baudComboBox.Width = 150;

            Button connectButton = new Button();
            connectButton.Text = "Connect";
            connectButton.Location = new Point(10, 80);
            connectButton.Click += (sender, e) =>
            {
                _portName = portComboBox.SelectedItem?.ToString();
                _baudRate = int.TryParse(baudComboBox.SelectedItem?.ToString(), out int baud) ? baud : 0;

                if (string.IsNullOrEmpty(_portName) || _portName == "No serial ports found")
                {
                    MessageBox.Show("Please select a valid serial port.", "Invalid Port", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (_baudRate <= 0)
                {
                    MessageBox.Show("Please select a valid baud rate.", "Invalid Baud Rate", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                portForm.Close();
            };

            portForm.Controls.Add(portLabel);
            portForm.Controls.Add(portComboBox);
            portForm.Controls.Add(baudLabel);
            portForm.Controls.Add(baudComboBox);
            portForm.Controls.Add(connectButton);

            portForm.ShowDialog();
        }
    }
}
