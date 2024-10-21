using System;
using System.Collections.Generic;
using System.Threading;

namespace LikwidatorBackend
{
    class Program
    {
        static void Main(string[] args)
        {
            string usbPortName = "COM3"; // Adjust the port name as needed
            int baudRate = 115200; // Adjust the baud rate if necessary
            string csvFilePath = "telemetryData.csv"; // Specify the CSV file path

            // Initialize DataHandler for saving telemetry data
            var dataHandler = new DataHandler(csvFilePath);

            // Create USBConnection instance
            using (var usbConnection = new USBConnection(usbPortName, baudRate))
            {
                usbConnection.OpenConnection();

                // Attach an event handler to process the telemetry data
                usbConnection.OnDataProcessed += (TelemetryData data) =>
                {
                    if (data != null)
                    {
                        // Save the telemetry data to CSV
                        dataHandler.SaveTelemetryData(data);

                        // Output the processed data for the GUI
                        Console.WriteLine($"Processed Data: GyroX={data.GyroX}, Altitude={data.Altitude}");
                    }
                };

                // Start reading data in a separate thread
                Thread readThread = new Thread(new ThreadStart(usbConnection.StartReading));
                readThread.Start();

                // Simulate the application running (you can replace this with a proper application loop)
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();

                // Close the connection
                usbConnection.CloseConnection();
                readThread.Join(); // Wait for the read thread to finish
            }

            // Retrieve data for GUI (if needed)
            List<TelemetryData> allTelemetryData = dataHandler.RetrieveTelemetryData();
            // You can process or display the retrieved data as needed
            foreach (var data in allTelemetryData)
            {
                Console.WriteLine($"Retrieved Data: GyroX={data.GyroX}, Altitude={data.Altitude}");
            }
        }
    }
}