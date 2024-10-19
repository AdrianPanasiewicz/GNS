using System;
using System.Collections.Generic;
using System.IO;

namespace LikwidatorBackend
{
    /// <summary>
    /// A class for handling data storage and retrieval in CSV format.
    /// </summary>
    public class DataHandler
    {
        private string _filePath;

        public DataHandler(string filePath)
        {
            _filePath = filePath;

            // Create file if it does not exist
            if (!File.Exists(_filePath))
            {
                using (var writer = new StreamWriter(_filePath, true))
                {
                    // Write header
                    writer.WriteLine("GyroX,GyroY,GyroZ,VerVel,VelAcc,Pitch,Roll,Heading,Altitude,Latitude,Longitude,SpeedOverGround,CourseOverGround");
                }
            }
        }

        /// <summary>
        /// Saves the telemetry data to a CSV file.
        /// </summary>
        /// <param name="data">The telemetry data to save.</param>
        public void SaveTelemetryData(TelemetryData data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            using (var writer = new StreamWriter(_filePath, true))
            {
                writer.WriteLine($"{data.GyroX},{data.GyroY},{data.GyroZ},{data.VerVel},{data.VelAcc}," +
                                 $"{data.Pitch},{data.Roll},{data.Heading},{data.Altitude}," +
                                 $"{data.Latitude},{data.Longitude},{data.SpeedOverGround},{data.CourseOverGround}");
            }
        }

        /// <summary>
        /// Retrieves all telemetry data from the CSV file.
        /// </summary>
        /// <returns>A list of telemetry data.</returns>
        public List<TelemetryData> RetrieveTelemetryData()
        {
            var telemetryDataList = new List<TelemetryData>();

            using (var reader = new StreamReader(_filePath))
            {
                // Skip the header line
                reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    var telemetryData = new TelemetryData
                    {
                        GyroX = float.Parse(values[0]),
                        GyroY = float.Parse(values[1]),
                        GyroZ = float.Parse(values[2]),
                        VerVel = float.Parse(values[3]),
                        VelAcc = float.Parse(values[4]),
                        Pitch = float.Parse(values[5]),
                        Roll = float.Parse(values[6]),
                        Heading = float.Parse(values[7]),
                        Altitude = float.Parse(values[8]),
                        Latitude = values[9],
                        Longitude = values[10],
                        SpeedOverGround = float.Parse(values[11]),
                    };
                        CourseOverGround = float.Parse(values[12])

                    telemetryDataList.Add(telemetryData);
                }
            }

            return telemetryDataList;
        }
    }
}
