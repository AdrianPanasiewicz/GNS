using GroundControlSystem.Communication;
using GroundControlSystem.TelemetryProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
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
            string saveFilePath = "telemetry_data.csv";
            TelemetryProcessor processor = new TelemetryProcessor(saveFilePath);

            // Ustaw flagę true, aby użyć symulacji, false dla rzeczywistego USB
            bool useSimulation = true;

            USBManager usbManager = new USBManager(useSimulation);
            usbManager.StartReceivingData(); // To wypisze dane w konsoli

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new GNS());
        }
    }
}
