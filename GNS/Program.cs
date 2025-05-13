using GroundControlSystem.Communication;
using GroundControlSystem.TelemetryProcessing;
using SerialCom;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace GNS
{
    internal static class Program
    {
        private static LoRaSerialReader serialReader;
        private static TelemetryProcessor processor;
        private static GNS formInstance;


    [STAThread]
    static void Main()
        {
            Logger.Log("========== Application Starting ==========");

            try
            {
                Logger.Log("Initializing serialReader...");
                serialReader = new LoRaSerialReader();
                Logger.Log("Serial reader initialized.");

                // Log working directory
                Logger.Log($"Current directory: {Environment.CurrentDirectory}");

                // Log CSV file path
                string workingDirectory = Environment.CurrentDirectory;
                string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
                string saveFilePath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Data",
                    "telemetry_data.csv");
        
                Logger.Log($"CSV save path: {saveFilePath}");

                // Initialize processor
                Logger.Log("Creating TelemetryProcessor...");
                processor = new TelemetryProcessor(saveFilePath);
                Logger.Log("Processor created.");

                // Thread initialization
                Logger.Log("Starting GUI thread...");
                Thread GUIThread = new Thread(GUI);
                GUIThread.SetApartmentState(ApartmentState.STA);
                GUIThread.Name = "GUI thread";
                GUIThread.Start();
                Logger.Log("GUI thread started.");

                Thread.Sleep(7000); // Consider logging why this delay exists
                Logger.Log("Starting BackEnd thread...");
                Thread BackEndThread = new Thread(() => BackEnd(processor, serialReader));
                BackEndThread.IsBackground = true;
                BackEndThread.Name = "Main thread";
                BackEndThread.Start();
                Logger.Log("BackEnd thread started.");
            }
            catch (Exception ex)
            {
                Logger.LogException("Main()", ex);
                MessageBox.Show($"Critical startup error: {ex.Message}");
                Environment.Exit(1);
            }
        }


        /// <summary>
        /// Metoda do obslugi back-end:
        /// 1. Pobieranie danych z USB.
        /// 2. Agregacja danych i ich przetwarzanie.
        /// 3. Zapisywanie do pliku CSV.
        /// 4. Przesylanie do Front- end.
        /// </summary>
        /// <param name="processor"></param>
        /// <param name="serialReader"></param>
        public static void BackEnd(TelemetryProcessor processor, LoRaSerialReader serialReader)
        {
            Logger.Log("BackEnd thread started.");
            try
            {
                if (serialReader.IsPortConnected)
                {
                    Logger.Log("Starting serialReader.Run()...");
                    serialReader.Run();
                }
                else
                {
                    Logger.Log("Serial port not connected. BackEnd thread exiting.");
                }
            }
            catch (Exception ex)
            {
                Logger.LogException("BackEnd", ex);
            }
        }

        /// <summary>
        /// Metoda do obslugi front-end:
        /// 1. Wyswietlanie danych w czasie rzeczywistym w okreslonym formacie.
        /// 2. Pozwalanie na komunikacje z uzytkownikiem.
        /// </summary>
        public static void GUI()
        {
            Logger.Log("GUI thread started.");
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Logger.Log("Creating GNS form...");
                formInstance = new GNS();
                Logger.Log("Starting application loop...");
                Application.Run(formInstance);
                Logger.Log("Application loop exited.");
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Logger.LogException("GUI", ex);
                MessageBox.Show($"GUI crash: {ex.Message}");
            }
        }
        /// <summary>
        /// Wzywana metoda w przypadku pojawienia się wydarzenia DataReceived
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public static void OnDataReceived(object source, EventArgs e)
        {
            // 1. Przetwórz dane do obiektu telemetrycznego
            TelemetryData telemetryPacket = serialReader.ToTelemetryData();

            // 2. Zapisz dane do CSV
            processor.SaveToCSV(telemetryPacket);

            // 3. Wyświetl dane telemetryczne w konsoli
            Console.WriteLine("Dane telemetryczne:");
            Console.WriteLine(telemetryPacket.ToString());


            // 4. Przeslij dane telemetryczne do GUI
            if (formInstance != null)
            {
                formInstance.Invoke(new Action(() =>
                {
                    formInstance.AddTelemetryDataPoint(telemetryPacket);
                }));
            }
        }

    }

}
