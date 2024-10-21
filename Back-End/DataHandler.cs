using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace LikwidatorBackend
{
    public class DataHandler
    {
        private string _filePath;

        public DataHandler(string filePath)
        {
            _filePath = filePath;

            // If the file doesn't exist, create it with the headers
            if (!File.Exists(_filePath))
            {
                using (var writer = new StreamWriter(_filePath, false))
                {
                    writer.WriteLine("GyroX,GyroY,GyroZ,VerVel,VelAcc,Pitch,Roll,Heading,Altitude,Latitude,Longitude,SpeedOverGround,CourseOverGround");
                }
            }
        }

        /// <summary>
        /// Saves the telemetry data to a CSV file.
        /// </summary>
        /// <param name="data">The telemetry data to save.</param>
        public void SaveData(TelemetryData data)
        {
            using (var writer = new StreamWriter(_filePath, true))
            {
                writer.WriteLine($"{data.GyroX},{data.GyroY},{data.GyroZ},{data.VerVel},{data.VelAcc},{data.Pitch},{data.Roll},{data.Heading},{data.Altitude},{data.Latitude},{data.Longitude},{data.SpeedOverGround},{data.CourseOverGround}");
            }
        }

        /// <summary>
        /// Loads all telemetry data from the CSV file.
        /// </summary>
        /// <returns>A list of TelemetryData objects.</returns>
        public List<TelemetryData> LoadData()
        {
            var telemetryDataList = new List<TelemetryData>();

            using (var reader = new StreamReader(_filePath))
            {
                string headerLine = reader.ReadLine(); // Skip header line

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    var data = new TelemetryData
                    {
                        GyroX = float.Parse(values[0], CultureInfo.InvariantCulture),
                        GyroY = float.Parse(values[1], CultureInfo.InvariantCulture),
                        GyroZ = float.Parse(values[2], CultureInfo.InvariantCulture),
                        VerVel = float.Parse(values[3], CultureInfo.InvariantCulture),
                        VelAcc = float.Parse(values[4], CultureInfo.InvariantCulture),
                        Pitch = float.Parse(values[5], CultureInfo.InvariantCulture),
                        Roll = float.Parse(values[6], CultureInfo.InvariantCulture),
                        Heading = float.Parse(values[7], CultureInfo.InvariantCulture),
                        Altitude = float.Parse(values[8], CultureInfo.InvariantCulture),
                        Latitude = values[9],
                        Longitude = values[10],
                        SpeedOverGround = float.Parse(values[11], CultureInfo.InvariantCulture),
                        CourseOverGround = float.Parse(values[12], CultureInfo.InvariantCulture)
                    };

                    telemetryDataList.Add(data);
                }
            }

            return telemetryDataList;
        }
    }
}
