using GroundControlSystem.Communication;
using GroundControlSystem.DataModels;
using GroundControlSystem.TelemetryProcessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GNS
{
    internal static class Program
    {
        /// <summary>
        /// Główny punkt wejścia dla aplikacji.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Znajdz sciezke do przestrzeni roboczej
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;

            string saveFilePath = projectDirectory + "\\GNS\\Data\\telemetry_data.csv";


            TelemetryProcessor processor = new TelemetryProcessor(saveFilePath);

            // Ustaw flagę true, aby użyć symulacji, false dla rzeczywistego USB
            bool useSimulation = true;

            USBManager usbManager = new USBManager(useSimulation);

            // 1. Inicjalizuj połączenie USB i odbierz dane
            usbManager.usbReceiver.InitializeConnection();
            Console.WriteLine("Rozpoczynanie odbioru danych z USB...");

            while (true)
            {
                // 2. Przetwórz dane do obiektu telemetrycznego
                byte[] rawData = usbManager.usbReceiver.ReceiveData();
                TelemetryPacket telemetryPacket = processor.ProcessRawData(rawData);

                // 3. Zapisz dane do CSV
                processor.SaveToCSV(telemetryPacket);

                // 4. Wyświetl dane telemetryczne w konsoli
                Console.WriteLine("Dane telemetryczne:");
                Console.WriteLine(telemetryPacket.ToCSV());

                Thread.Sleep(500);
            }

            ////5. Zamknij polaczenie
            //usbManager.usbReceiver.CloseConnection();

            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new GNS());
        }
    }
}
