using GroundControlSystem.Communication;
using GroundControlSystem.DataModels;
using GroundControlSystem.TelemetryProcessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;

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

            // Stworzenie procesora do obslugi pobieranych danych z USB
            TelemetryProcessor processor = new TelemetryProcessor(saveFilePath);

            // Ustaw flagę true, aby użyć symulacji, false dla rzeczywistego USB
            bool useSimulation = true;

            USBManager usbManager = new USBManager(useSimulation);

            // 1. Inicjalizuj połączenie USB i odbierz dane
            usbManager.usbReceiver.InitializeConnection();
            Console.WriteLine("Rozpoczynanie odbioru danych z USB...");


            // Stworzenie watku do obslugi back-end
            Thread MainThread = new Thread(() => BackEnd(processor, usbManager));
            MainThread.Name = "Main thread";


            // Stworzenie watku do obslugi GUI
            Thread GUIThread = new Thread(GUI);
            GUIThread.SetApartmentState(ApartmentState.STA);
            GUIThread.Name = "GUI thread";

            // Uruchomienie obu watkow
            MainThread.Start();
            GUIThread.Start();

        }

        /// <summary>
        /// Funkcja do obslugi backend:
        /// 1. Pobieranie danych z USB.
        /// 2. Agregacja danych i ich przetwarzanie.
        /// 3. Zapisywanie do pliku CSV.
        /// 4. Przesylanie do Front- end.
        /// </summary>
        public static void BackEnd(TelemetryProcessor processor, USBManager usbManager)
        {
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
        }

        /// <summary>
        /// Funkcja do obslugi front-end:
        /// 1. Wyswietlanie danych w czasie rzeczywistym w okreslonym formacie.
        /// 2. Pozwalanie na komunikacje z uzytkownikiem.
        /// </summary>
        public static void GUI()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new GNS());
        }
    }

}
