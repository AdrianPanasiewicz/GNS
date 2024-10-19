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
        /// Reads incoming data from the USB-B connection.
        /// </summary>
        /// <returns>A string containing the incoming data.</returns>
        public string ReadData()
        {
            try
            {
                if (_serialPort.IsOpen && _isRunning)
                {
                    string data = _serialPort.ReadLine();
                    Console.WriteLine($"Data received: {data}");
                    return data;
                }
                else
                {
                    throw new InvalidOperationException("USB-B connection is not open.");
                }
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

            // Continuously read data (could be handled by a separate thread or task)
            for (int i = 0; i < 5; i++)  // Example of reading 5 times
            {
                string data = usbConnection.ReadData();
                if (data != null)
                {
                    // Parse and handle data here
                }
                Thread.Sleep(1000); // Simulate a delay between reads
            }

            usbConnection.CloseConnection();
        }
    }
}
