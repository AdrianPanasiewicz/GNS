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

        private Thread serialThread;
        
        public delegate void DataReceivedEventHandler(object source, EventArgs e);

        public event DataReceivedEventHandler DataReceived;

        public LoRaSerialReader() 
        {
            serialThread = new Thread(ReadPort);
            _serialPort = new SerialPort();
            _receivedTelemetry = new List<string>();
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

            while(_continue)
            {
                try
                {
                    string message = _serialPort.ReadLine();

                    if (skipFirstRead)
                    {
                        skipFirstRead = false;
                        continue;
                    }

                    _receivedTelemetry.Add(message);
                    OnDataReceived();
                }
                catch (TimeoutException) 
                {
                    MessageBox.Show("No data received within the timeout period.", "Error: Delay detected!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    _continue = false;
                }
            }
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
            var dataParts = _receivedTelemetry.Last().Split(';');

            if (dataParts.Length == 19)
            {
                return new TelemetryData
                {
                    Time = new TimeData
                    {
                        TimeStamp = DateTime.Parse(dataParts[0])
                    },
                    IMU = new IMUData
                    {
                        AccX = double.Parse(dataParts[1], CultureInfo.InvariantCulture),
                        AccY = double.Parse(dataParts[2], CultureInfo.InvariantCulture),
                        AccZ = double.Parse(dataParts[3], CultureInfo.InvariantCulture),
                        GyroX = double.Parse(dataParts[4], CultureInfo.InvariantCulture),
                        GyroY = double.Parse(dataParts[5], CultureInfo.InvariantCulture),
                        GyroZ = double.Parse(dataParts[6], CultureInfo.InvariantCulture),
                        MagX = double.Parse(dataParts[7], CultureInfo.InvariantCulture),
                        MagY = double.Parse(dataParts[8], CultureInfo.InvariantCulture),
                        MagZ = double.Parse(dataParts[9], CultureInfo.InvariantCulture),
                        Heading = double.Parse(dataParts[10], CultureInfo.InvariantCulture),
                        Pitch = double.Parse(dataParts[11], CultureInfo.InvariantCulture),
                        Roll = double.Parse(dataParts[12], CultureInfo.InvariantCulture)
                    },
                    Baro = new BaroData
                    {
                        AccZInertial = double.Parse(dataParts[13], CultureInfo.InvariantCulture),
                        VerticalVelocity = double.Parse(dataParts[14], CultureInfo.InvariantCulture),
                        Pressure = double.Parse(dataParts[15], CultureInfo.InvariantCulture),
                        Altitude = double.Parse(dataParts[16], CultureInfo.InvariantCulture)
                    },
                    GPS = new GPSData
                    {
                        Latitude = double.Parse(dataParts[17], CultureInfo.InvariantCulture),
                        Longitude = double.Parse(dataParts[18], CultureInfo.InvariantCulture)
                    }
                };
            }
            else
            {
                throw new ArgumentException($"Zbyt mało danych.");
            }
        }

        public List<TelemetryData> ToTelemetryDataList()
        {
            var telemetryDataList = new List<TelemetryData>();

            foreach (var data in _receivedTelemetry)
            {
                var dataParts = data.Split(';');

                
                if (dataParts.Length == 19) 
                {
                    var telemetryData = new TelemetryData
                    {
                        Time = new TimeData
                        {
                            TimeStamp = DateTime.Parse(dataParts[0])
                        },
                        IMU = new IMUData
                        {
                            AccX = double.Parse(dataParts[1].Replace(',', '.'), CultureInfo.InvariantCulture),
                            AccY = double.Parse(dataParts[2].Replace(',', '.'), CultureInfo.InvariantCulture),
                            AccZ = double.Parse(dataParts[3].Replace(',', '.'), CultureInfo.InvariantCulture),
                            GyroX = double.Parse(dataParts[4].Replace(',', '.'), CultureInfo.InvariantCulture),
                            GyroY = double.Parse(dataParts[5].Replace(',', '.'), CultureInfo.InvariantCulture),
                            GyroZ = double.Parse(dataParts[6].Replace(',', '.'), CultureInfo.InvariantCulture),
                            MagX = double.Parse(dataParts[7].Replace(',', '.'), CultureInfo.InvariantCulture),
                            MagY = double.Parse(dataParts[8].Replace(',', '.'), CultureInfo.InvariantCulture),
                            MagZ = double.Parse(dataParts[9].Replace(',', '.'), CultureInfo.InvariantCulture),
                            Heading = double.Parse(dataParts[10].Replace(',', '.'), CultureInfo.InvariantCulture),
                            Pitch = double.Parse(dataParts[11].Replace(',', '.'), CultureInfo.InvariantCulture),
                            Roll = double.Parse(dataParts[12].Replace(',', '.'), CultureInfo.InvariantCulture)
                        },
                        Baro = new BaroData
                        {
                            AccZInertial = double.Parse(dataParts[13].Replace(',', '.'), CultureInfo.InvariantCulture),
                            VerticalVelocity = double.Parse(dataParts[14].Replace(',', '.'), CultureInfo.InvariantCulture),
                            Pressure = double.Parse(dataParts[15].Replace(',', '.'), CultureInfo.InvariantCulture),
                            Altitude = double.Parse(dataParts[16].Replace(',', '.'), CultureInfo.InvariantCulture)
                        },
                        GPS = new GPSData
                        {
                            Latitude = double.Parse(dataParts[17].Replace(',', '.'), CultureInfo.InvariantCulture),
                            Longitude = double.Parse(dataParts[18].Replace(',', '.'), CultureInfo.InvariantCulture)
                        }
                    };

                    telemetryDataList.Add(telemetryData);
                }
                else
                {
                    
                    throw new ArgumentException($"Zbyt mało danych w: {data}");
                }
            }

            return telemetryDataList;
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
