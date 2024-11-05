using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Documents;
using SerialCom;

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
        /// Zapisuje dane telemetryczne do pliku CSV. Zalecane jest przeniesc plik CSV z folder Data, aby nie mieszac danych.
        /// </summary>
        /// <param name="packet">Pakiet telemetryczny do zapisania.</param>
        public void SaveToCSV(TelemetryData packet)
        {
            var records = new List<TelemetryData>
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
