using System;
using System.IO.Ports;
using System.Threading;
using System.Diagnostics;

namespace LikwidatorBackend
{
    /// <summary>
    /// USBConnection is responsible for managing the USB-B communication between the Ground Control System
    /// and the rocket's telemetry data via LoRa.
    /// </summary>
    public class USBConnection : IDisposable
    {
        private SerialPort _serialPort;
        private bool _isRunning;

        // Event to notify listeners when new data is received and processed
        public event Action<TelemetryData> OnDataProcessed;

        /// <summary>
        /// Initializes a new instance of the USBConnection class.
        /// </summary>
        /// <param name="portName">The name of the serial port (e.g., COM3, COM4).</param>
        /// <param name="baudRate">Baud rate for the serial communication (e.g., 9600, 115200).</param>
        public USBConnection(string portName, int baudRate = 9600)
        {
            _serialPort = new SerialPort
            {
                PortName = portName,
                BaudRate = baudRate,
                DataBits = 8,
                Parity = Parity.None,
                StopBits = StopBits.One,
                Handshake = Handshake.None,
                ReadTimeout = 5000,
                WriteTimeout = 5000
            };

            _isRunning = false;
        }

        /// <summary>
        /// Opens the USB-B connection.
        /// </summary>
        public void OpenConnection()
        {
            try
            {
                _serialPort.Open();
                _isRunning = true;
                Console.WriteLine("USB-B connection opened successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to open USB-B connection: {ex.Message}");
            }
        }

        /// <summary>
        /// Continuously reads data and processes it.
        /// This method can be run on a separate thread to handle data in the background.
        /// </summary>
        public void StartReading()
        {
            if (!_serialPort.IsOpen)
            {
                throw new InvalidOperationException("USB-B connection is not open.");
            }

            _isRunning = true;

            while (_isRunning)
            {
                string data = ReadData();
                if (data != null)
                {
                    TelemetryData processedData = ProcessData(data);
                    OnDataProcessed?.Invoke(processedData);  // Notify listeners about the new data
                }
                Thread.Sleep(33);  // Adjust based on expected data frequency
            }
        }

        /// <summary>
        /// Reads incoming data from the USB-B connection.
        /// </summary>
        /// <returns>A string containing the incoming data.</returns>
        private string ReadData()
        {
            try
            {
                string data = _serialPort.ReadLine();
                Console.WriteLine($"Data received: {data}");
                return data;
            }
            catch (TimeoutException)
            {
                Console.WriteLine("Timeout occurred while reading data.");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading data from USB-B: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Processes the raw data received from the USB-B connection.
        /// </summary>
        /// <param name="rawData">The raw data as a string.</param>
        /// <returns>A TelemetryData object containing the parsed information.</returns>
        private TelemetryData ProcessData(string rawData)
        {
            // Example of parsing the simplified LORA format: g1;g2:g3;vv;va;p;r;h;A;la;lo;so;co
            string[] dataParts = rawData.Split(';');
            try
            {
                var telemetryData = new TelemetryData
                {
                    GyroX = float.Parse(dataParts[0]),
                    GyroY = float.Parse(dataParts[1]),
                    GyroZ = float.Parse(dataParts[2]),
                    VerVel = float.Parse(dataParts[3]),
                    VelAcc = float.Parse(dataParts[4]),
                    Pitch = float.Parse(dataParts[5]),
                    Roll = float.Parse(dataParts[6]),
                    Heading = float.Parse(dataParts[7]),
                    Altitude = float.Parse(dataParts[8]),
                    Latitude = dataParts[9],
                    Longitude = dataParts[10],
                    SpeedOverGround = float.Parse(dataParts[11]),
                    CourseOverGround = float.Parse(dataParts[12])
                };

                Console.WriteLine("Data processed successfully.");
                return telemetryData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing data: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Closes the USB-B connection.
        /// </summary>
        public void CloseConnection()
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.Close();
                _isRunning = false;
                Console.WriteLine("USB-B connection closed.");
            }
        }

        /// <summary>
        /// Disposes of the USBConnection object and closes the serial port.
        /// </summary>
        public void Dispose()
        {
            CloseConnection();
            _serialPort.Dispose();
        }

        /// <summary>
        /// Example method to demonstrate usage of the USBConnection class.
        /// </summary>
        public static void ExampleUsage()
        {
            USBConnection usbConnection = new USBConnection("COM3", 115200);

            usbConnection.OpenConnection();

            // Attach an event handler to process the telemetry data
            usbConnection.OnDataProcessed += (TelemetryData data) =>
            {
                if (data != null)
                {
                    Console.WriteLine($"Processed Data: GyroX={data.GyroX}, Altitude={data.Altitude}");
                    // Perform further actions with the processed data
                }
            };

            // Start reading data in a separate thread
            Thread readThread = new Thread(new ThreadStart(usbConnection.StartReading));
            readThread.Start();

            // Simulate the application running for a while
            Thread.Sleep(10000);

            usbConnection.CloseConnection();
        }
    }
}