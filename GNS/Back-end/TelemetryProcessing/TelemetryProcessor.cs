using CsvHelper;
using CsvHelper.Configuration;
using GroundControlSystem.DataModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Documents;

namespace GroundControlSystem.TelemetryProcessing
{
    /// <summary>
    /// Klasa odpowiedzialna za przetwarzanie danych telemetrycznych.
    /// </summary>
    public class TelemetryProcessor
    {
        private readonly string _saveFilePath;

        /// <summary>
        /// Konstruktor inicjalizujący procesor telemetryczny.
        /// </summary>
        /// <param name="saveFilePath">Ścieżka do pliku, w którym zapisywane będą dane telemetryczne w formacie CSV.</param>
        public TelemetryProcessor(string saveFilePath)
        {
            _saveFilePath = saveFilePath;
        }

        /// <summary>
        /// Przetwarza dane z surowego bufora (np. odebranego z USB) i mapuje je na obiekt TelemetryPacket.
        /// </summary>
        /// <param name="rawData">Surowe dane z urządzenia.</param>
        public TelemetryPacket ProcessRawData(byte[] rawData)
        {
            // Przykładowa logika parsowania danych, zakładając, że dane są w odpowiedniej kolejności
            // To będzie zależne od formatu, w jakim dane są przesyłane przez USB.
            var telemetryPacket = new TelemetryPacket();

            // IMU
            telemetryPacket.IMU.AccX = BitConverter.ToSingle(rawData, 0);
            telemetryPacket.IMU.AccY = BitConverter.ToSingle(rawData, 4);
            telemetryPacket.IMU.AccZ = BitConverter.ToSingle(rawData, 8);
            telemetryPacket.IMU.GyroX = BitConverter.ToSingle(rawData, 12);
            telemetryPacket.IMU.GyroY = BitConverter.ToSingle(rawData, 16);
            telemetryPacket.IMU.GyroZ = BitConverter.ToSingle(rawData, 20);
            telemetryPacket.IMU.MagX = BitConverter.ToSingle(rawData, 24);
            telemetryPacket.IMU.MagY = BitConverter.ToSingle(rawData, 28);
            telemetryPacket.IMU.MagZ = BitConverter.ToSingle(rawData, 32);
            telemetryPacket.IMU.VerVel = BitConverter.ToSingle(rawData, 36);
            telemetryPacket.IMU.VelAcc = BitConverter.ToSingle(rawData, 40);
            telemetryPacket.IMU.Pitch = BitConverter.ToSingle(rawData, 44);
            telemetryPacket.IMU.Roll = BitConverter.ToSingle(rawData, 48);
            telemetryPacket.IMU.Heading = BitConverter.ToSingle(rawData, 52);

            // BARO
            telemetryPacket.Barometer.Pressure = BitConverter.ToSingle(rawData, 56);
            telemetryPacket.Barometer.Temperature = BitConverter.ToSingle(rawData, 60);
            telemetryPacket.Barometer.Altitude = BitConverter.ToSingle(rawData, 64);

            // GPS
            telemetryPacket.GPS.Year = BitConverter.ToInt32(rawData, 68);
            telemetryPacket.GPS.Month = BitConverter.ToInt32(rawData, 72);
            telemetryPacket.GPS.Day = BitConverter.ToInt32(rawData, 76);
            telemetryPacket.GPS.Hours = BitConverter.ToInt32(rawData, 80);
            telemetryPacket.GPS.Minutes = BitConverter.ToInt32(rawData, 84);
            telemetryPacket.GPS.Seconds = BitConverter.ToInt32(rawData, 88);
            telemetryPacket.GPS.Latitude = BitConverter.ToDouble(rawData, 92);
            telemetryPacket.GPS.Longitude = BitConverter.ToDouble(rawData, 100);
            telemetryPacket.GPS.SatellitesUsed = BitConverter.ToInt32(rawData, 108);
            telemetryPacket.GPS.AltitudeGPS = BitConverter.ToSingle(rawData, 112);
            telemetryPacket.GPS.SpeedOverGround = BitConverter.ToSingle(rawData, 116);
            telemetryPacket.GPS.CourseOverGround = BitConverter.ToSingle(rawData, 120);
            telemetryPacket.GPS.GNSSMode = System.Text.Encoding.ASCII.GetString(rawData, 124, 2);

            // TSS
            telemetryPacket.TSS.PWMsignalServo1 = BitConverter.ToInt32(rawData, 128);
            telemetryPacket.TSS.PWMsignalServo2 = BitConverter.ToInt32(rawData, 132);
            telemetryPacket.TSS.PWMsignalServo3 = BitConverter.ToInt32(rawData, 136);
            telemetryPacket.TSS.PWMsignalServo4 = BitConverter.ToInt32(rawData, 140);

            // TVS
            telemetryPacket.TVS.PWMsignalServo5 = BitConverter.ToInt32(rawData, 144);
            telemetryPacket.TVS.PWMsignalServo6 = BitConverter.ToInt32(rawData, 148);

            return telemetryPacket;
        }

        /// <summary>
        /// Zapisuje dane telemetryczne do pliku CSV. Zalecane jest przeniesc plik CSV z folder Data, aby nie mieszac danych.
        /// </summary>
        /// <param name="packet">Pakiet telemetryczny do zapisania.</param>
        public void SaveToCSV(TelemetryPacket packet)
        {
            var records = new List<TelemetryPacket>
            {
                packet,
            };

            string csvLine = packet.ToCSV();

            try
            {
                if (!File.Exists(_saveFilePath))
                {
                    using (File.Create(_saveFilePath)) { }

                    var WhileCreatingConfigPersons = new CsvConfiguration(CultureInfo.InvariantCulture)
                    { HasHeaderRecord = true };

                    using (StreamWriter streamWriter = new StreamWriter(_saveFilePath))
                    using (CsvWriter csvWriter = new CsvWriter(streamWriter, WhileCreatingConfigPersons))
                    {
                        csvWriter.WriteRecords(records);
                    }
                }

                var configPersons = new CsvConfiguration(CultureInfo.InvariantCulture)
                {  HasHeaderRecord = false };

                using (StreamWriter streamWriter = new StreamWriter(_saveFilePath, true))
                using (CsvWriter csvWriter = new CsvWriter(streamWriter, configPersons))
                {
                    csvWriter.WriteRecords(records);
                }

                Console.WriteLine("Data written to CSV successfully.");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd podczas zapisu do pliku CSV: {ex.Message}");
            }
        }

    }
}
