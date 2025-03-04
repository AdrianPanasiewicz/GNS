﻿using GroundControlSystem.Communication;
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
            // Stworz klase odpowiedzialna za czytanie danych z SerialPort i zainicjalizuj ja
            serialReader = new LoRaSerialReader();
            //serialReader.ShowPortSelection();
            //serialReader.Init();

            // Znajdz sciezke do przestrzeni roboczej
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
            string saveFilePath = projectDirectory + "\\GNS\\Data\\telemetry_data.csv";

            // Stworzenie procesora do obslugi pobieranych danych z USB
            processor = new TelemetryProcessor(saveFilePath);

            serialReader.DataReceived += Program.OnDataReceived;

            // Stworzenie watku do obslugi back-end
            Thread BackEndThread = new Thread(() => BackEnd(processor, serialReader));
            BackEndThread.IsBackground = true;
            BackEndThread.Name = "Main thread";


            // Stworzenie watku do obslugi GUI
            Thread GUIThread = new Thread(GUI);
            GUIThread.SetApartmentState(ApartmentState.STA);
            GUIThread.Name = "GUI thread";

            // Uruchomienie obu watkow
            GUIThread.Start();
            Thread.Sleep(7000);
            BackEndThread.Start();

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
            // Uruchom klase dp czytania danych z IUSB
            serialReader.Run();
            GNS formInstance = Application.OpenForms.OfType<GNS>().FirstOrDefault();

        }

        /// <summary>
        /// Metoda do obslugi front-end:
        /// 1. Wyswietlanie danych w czasie rzeczywistym w okreslonym formacie.
        /// 2. Pozwalanie na komunikacje z uzytkownikiem.
        /// </summary>
        public static void GUI()
        {
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            formInstance = new GNS();
            Application.Run(formInstance);
            Environment.Exit(0);
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
