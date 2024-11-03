using GroundControlSystem.Communication;
using GroundControlSystem.TelemetryProcessing;
using SerialCom;
using System;
using System.IO;
using System.Linq;
using System.Threading;
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
            LoRaSerialReader serialReader = new LoRaSerialReader();
            serialReader.Init();

            // Znajdz sciezke do przestrzeni roboczej
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
            string saveFilePath = projectDirectory + "\\GNS\\Data\\telemetry_data.csv";

            // Stworzenie procesora do obslugi pobieranych danych z USB
            TelemetryProcessor processor = new TelemetryProcessor(saveFilePath);

            //// Ustaw flagę true, aby użyć symulacji, false dla rzeczywistego USB
            //bool useSimulation = true;

            //USBManager usbManager = new USBManager(useSimulation);

            //// Inicjalizuj połączenie USB i odbierz dane
            //usbManager.usbReceiver.InitializeConnection();
            //Console.WriteLine("Rozpoczynanie odbioru danych z USB...");


            // Stworzenie watku do obslugi back-end
            Thread BackEndThread = new Thread(() => BackEnd(processor, serialReader));
            BackEndThread.Name = "Main thread";


            // Stworzenie watku do obslugi GUI
            Thread GUIThread = new Thread(GUI);
            GUIThread.SetApartmentState(ApartmentState.STA);
            GUIThread.Name = "GUI thread";

            // Uruchomienie obu watkow
            GUIThread.Start();
            Thread.Sleep(5000);
            BackEndThread.Start();

        }


        /// <summary>
        /// Funkcja do obslugi backend:
        /// 1. Pobieranie danych z USB.
        /// 2. Agregacja danych i ich przetwarzanie.
        /// 3. Zapisywanie do pliku CSV.
        /// 4. Przesylanie do Front- end.
        /// </summary>
        /// <param name="processor"></param>
        /// <param name="usbManager"></param>
        public static void BackEnd(TelemetryProcessor processor, LoRaSerialReader serialReader)
        {
            serialReader.Run();

            while (true)
            {
                GNS formInstance = Application.OpenForms.OfType<GNS>().FirstOrDefault();

                // 2. Przetwórz dane do obiektu telemetrycznego
                TelemetryData telemetryPacket = serialReader.ToTelemetryData();

                // 3. Zapisz dane do CSV
                processor.SaveToCSV(telemetryPacket);

                // 4. Wyświetl dane telemetryczne w konsoli
                Console.WriteLine("Dane telemetryczne:");
                Console.WriteLine(telemetryPacket.ToString());


                // 5. Przeslij dane telemetryczne do GUI
                if (formInstance != null)
                {
                    formInstance.Invoke(new Action(() =>
                    {
                        formInstance.AddTelemetryDataPoint(telemetryPacket);
                    }));
                }

                // 6. Czekaj okreslona chwile
                Thread.Sleep(100);
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
            Environment.Exit(0);
        }

    }

}
