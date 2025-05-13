using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;

namespace SerialCom
{
    public class LoRaSerialReader
    {
        private SerialPort _serialPort;
        private string _portName;
        private int _baudRate;
        private bool _continue;
        private List<string> _receivedTelemetry;
        public List<string> ReceivedTelemetry { get => _receivedTelemetry; }
        private Thread serialThread;
        private bool _portConnected = false;
        public bool IsPortConnected => _portConnected;

        public delegate void DataReceivedEventHandler(object source, EventArgs e);
        public event DataReceivedEventHandler DataReceived;

        public LoRaSerialReader()
        {
            serialThread = new Thread(ReadPort);
            _serialPort = new SerialPort();
            _receivedTelemetry = new List<string>();
            ShowPortSelection();
            Init();
        }

        public void Init()
        {
            if (!string.IsNullOrEmpty(_portName))
            {
                _serialPort.PortName = _portName;
            }

            // Set default baud rate if invalid
            _serialPort.BaudRate = _baudRate > 0 ? _baudRate : 9600;

            _serialPort.ReadTimeout = 7000;
            _serialPort.WriteTimeout = 7000;
        }

        public void Run()
        {
            AttemptPortAccess();

            while (_continue)
            {
                Thread.Sleep(100);
            }

            if (_serialPort.IsOpen)
            {
                serialThread.Join();
                _serialPort.Close();
            }
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
                        OnDataReceived();
                    }
                }
                catch (TimeoutException)
                {
                    if (_portConnected)
                    {
                        MessageBox.Show("No data received - check transmitter connection.\nContinuing in offline mode.",
                                      "SerialCom Warning",
                                      MessageBoxButtons.OK,
                                      MessageBoxIcon.Warning);
                        _portConnected = false;
                    }
                    Thread.Sleep(2000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Read error: {ex.Message}");
                    Thread.Sleep(1000);
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

            if (string.IsNullOrEmpty(_portName))
            {
                MessageBox.Show("Running in offline mode", "Info",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
                _portConnected = false;
                _continue = false;
                return;
            }

            for (int i = 0; i < attempts; i++)
            {
                try
                {
                    _serialPort.Open();
                    _continue = true;
                    _portConnected = true;
                    serialThread.Start();
                    portAvailable = true;
                    MessageBox.Show("SerialCom started successfully", "SerialCom", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Port access attempt {i + 1} failed: {ex.Message}");
                    Thread.Sleep(1000);
                }
            }

            if (!portAvailable)
            {
                MessageBox.Show("Port is unavailable or busy.\nYou can still use the application without live data.",
                              "SerialCom Warning",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Warning);
                _portConnected = false;
                _continue = false;
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
                return new TelemetryData
                {
                    IsValid = false,
                    Status = "No data available - check connection or transmitter"
                };
            }

            string lastMessage = _receivedTelemetry.Last();
            var telemetryData = new TelemetryData();

            var dataParts = lastMessage.Split(';');

            telemetryData.LoRa.MsgLength = dataParts.Length > 0 ? ParseDoubleSafe(dataParts[0]) : 0.0;
            telemetryData.LoRa.RSSI = dataParts.Length > 1 ? ParseDoubleSafe(dataParts[1]) : 0.0;
            telemetryData.LoRa.SNR = dataParts.Length > 2 ? ParseDoubleSafe(dataParts[2]) : 0.0;
            telemetryData.Time.TimeStamp = dataParts.Length > 3 && DateTime.TryParseExact(dataParts[3], "H:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var timestamp) ? timestamp : DateTime.MinValue;

            telemetryData.IMU.AccX = dataParts.Length > 4 ? ParseDoubleSafe(dataParts[4]) : 0.0;
            telemetryData.IMU.AccY = dataParts.Length > 5 ? ParseDoubleSafe(dataParts[5]) : 0.0;
            telemetryData.IMU.AccZ = dataParts.Length > 6 ? ParseDoubleSafe(dataParts[6]) : 0.0;
            telemetryData.IMU.GyroX = dataParts.Length > 7 ? ParseDoubleSafe(dataParts[7]) : 0.0;
            telemetryData.IMU.GyroY = dataParts.Length > 8 ? ParseDoubleSafe(dataParts[8]) : 0.0;
            telemetryData.IMU.GyroZ = dataParts.Length > 9 ? ParseDoubleSafe(dataParts[9]) : 0.0;
            telemetryData.IMU.MagX = dataParts.Length > 10 ? ParseDoubleSafe(dataParts[10]) : 0.0;
            telemetryData.IMU.MagY = dataParts.Length > 11 ? ParseDoubleSafe(dataParts[11]) : 0.0;
            telemetryData.IMU.MagZ = dataParts.Length > 12 ? ParseDoubleSafe(dataParts[12]) : 0.0;
            telemetryData.IMU.Heading = dataParts.Length > 13 ? ParseDoubleSafe(dataParts[13]) : 0.0;
            telemetryData.IMU.Pitch = dataParts.Length > 14 ? ParseDoubleSafe(dataParts[14]) : 0.0;
            telemetryData.IMU.Roll = dataParts.Length > 15 ? ParseDoubleSafe(dataParts[15]) : 0.0;

            telemetryData.Baro.AccZInertial = dataParts.Length > 16 ? ParseDoubleSafe(dataParts[16]) : 0.0;
            telemetryData.Baro.VerticalVelocity = dataParts.Length > 17 ? ParseDoubleSafe(dataParts[17]) : 0.0;
            telemetryData.Baro.Pressure = dataParts.Length > 18 ? ParseDoubleSafe(dataParts[18]) : 0.0;
            telemetryData.Baro.Altitude = dataParts.Length > 19 ? ParseDoubleSafe(dataParts[19]) : 0.0;

            telemetryData.GPS.Latitude = dataParts.Length > 20 ? ParseDoubleSafe(dataParts[20]) : 0.0;
            telemetryData.GPS.Longitude = dataParts.Length > 21 ? ParseDoubleSafe(dataParts[21]) : 0.0;

            telemetryData.IsValid = true;
            return telemetryData;
        }

        private double ParseDoubleSafe(string value)
        {
            return double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out double result) ? result : 0.0;
        }

        protected virtual void OnDataReceived()
        {
            DataReceived?.Invoke(this, EventArgs.Empty);
        }

        private void ShowPortSelection()
        {
            Form portForm = new Form
            {
                Text = "SerialCom Config",
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

            Button cancelButton = new Button();
            cancelButton.Text = "Continue Without Port";
            cancelButton.Location = new Point(200, 80);
            cancelButton.Click += (sender, e) =>
            {
                _portName = "";
                _baudRate = 0;
                portForm.Close();
            };

            portForm.Controls.Add(portLabel);
            portForm.Controls.Add(portComboBox);
            portForm.Controls.Add(baudLabel);
            portForm.Controls.Add(baudComboBox);
            portForm.Controls.Add(connectButton);
            portForm.Controls.Add(cancelButton);

            portForm.ShowDialog();
        }

        private void LoRaConfig()
        {
            Thread.Sleep(100);
            _serialPort.WriteLine("at");
            Thread.Sleep(100);
            _serialPort.WriteLine("at+mode=test");
            Thread.Sleep(100);
            _serialPort.WriteLine("at+test=rfcfg");
            Thread.Sleep(100);
            _serialPort.WriteLine("at+test=rxlrpkt");
            Thread.Sleep(100);
        }
    }
}