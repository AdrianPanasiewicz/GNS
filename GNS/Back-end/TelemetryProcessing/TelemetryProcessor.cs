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
        private string _saveFilePath;
        private bool _NewLaunchFlag;

        /// <summary>
        /// Konstruktor inicjalizujący procesor telemetryczny.
        /// </summary>
        /// <param name="saveFilePath">Ścieżka do pliku, w którym zapisywane będą dane telemetryczne w formacie CSV.</param>
        public TelemetryProcessor(string saveFilePath)
        {
            _saveFilePath = saveFilePath;
            _NewLaunchFlag = true;
        }


        /// <summary>
        /// Zapisuje dane telemetryczne do pliku CSV. Jeśli plik o tej samej nazwie już istnieje,
        /// tworzy nowy plik o tej samej nazwie z dodatkiem (n), gdzie n to liczba plików o tej nazwie w folderze.
        /// </summary>
        /// <param name="packet">Pakiet telemetryczny do zapisania.</param>
        public void SaveToCSV(TelemetryData packet)
        {
            var records = new List<TelemetryData> { packet };

            // Sprawdzamy czy plik o podanej ścieżce już istnieje, jeśli tak, tworzymy unikalną nazwę
            string filePath = _saveFilePath;
            int fileCounter = 1;

            // Tylko jeśli _NewLaunchFlag jest ustawiony na true
            if (_NewLaunchFlag)
            {
                while (File.Exists(filePath))
                {
                    // Dodanie numeracji do nazwy pliku
                    string directory = Path.GetDirectoryName(_saveFilePath);
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(_saveFilePath);
                    string extension = Path.GetExtension(_saveFilePath);
                    filePath = Path.Combine(directory, $"{fileNameWithoutExtension}({fileCounter}){extension}");
                    fileCounter++;
                }

                // Ustaw flagę na false po utworzeniu pliku
                _NewLaunchFlag = false;
                _saveFilePath = filePath; // Zaktualizuj ścieżkę pliku

                try
                {
                    // Konfiguracja do zapisu nagłówków
                    var csvConfigWithHeader = new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        HasHeaderRecord = true
                    };

                    using (StreamWriter streamWriter = new StreamWriter(filePath))
                    using (CsvWriter csvWriter = new CsvWriter(streamWriter, csvConfigWithHeader))
                    {
                        // Zapisujemy nagłówki - dostosuj według swoich zmiennych
                        csvWriter.WriteField("MsgLength");
                        csvWriter.WriteField("RSSI");
                        csvWriter.WriteField("SNR");
                        csvWriter.WriteField("TimeStamp");
                        csvWriter.WriteField("AccX");
                        csvWriter.WriteField("AccY");
                        csvWriter.WriteField("AccZ");
                        csvWriter.WriteField("GyroX");
                        csvWriter.WriteField("GyroY");
                        csvWriter.WriteField("GyroZ");
                        csvWriter.WriteField("MagX");
                        csvWriter.WriteField("MagY");
                        csvWriter.WriteField("MagZ");
                        csvWriter.WriteField("Heading");
                        csvWriter.WriteField("Pitch");
                        csvWriter.WriteField("Roll");
                        csvWriter.WriteField("Baro_AccZInertial");
                        csvWriter.WriteField("VerticalVelocity");
                        csvWriter.WriteField("Pressure");
                        csvWriter.WriteField("Altitude");
                        csvWriter.WriteField("Latitude");
                        csvWriter.WriteField("Longitude");
                        csvWriter.NextRecord(); // Przechodzimy do następnego rekordu

                        // Zapisujemy rekordy z nagłówkami
                        csvWriter.WriteRecords(records);
                    }

                    Console.WriteLine($"Dane zapisane do nowego pliku CSV: {filePath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Błąd podczas zapisu do pliku CSV: {ex.Message}");
                }
            }
            else
            {
                // Jeśli plik nie został utworzony w nowym uruchomieniu, dodajemy do istniejącego pliku
                try
                {
                    // Sprawdzamy, czy plik istnieje przed dodaniem rekordów
                    var csvConfigWithoutHeader = new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        HasHeaderRecord = false
                    };

                    using (StreamWriter streamWriter = new StreamWriter(filePath, append: true))
                    using (CsvWriter csvWriter = new CsvWriter(streamWriter, csvConfigWithoutHeader))
                    {
                        // Zapisujemy rekordy bez nagłówków
                        csvWriter.WriteRecords(records);
                    }

                    Console.WriteLine($"Dane dopisane do istniejącego pliku CSV: {filePath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Błąd podczas zapisu do pliku CSV: {ex.Message}");
                }
            }
        }

    }
}
