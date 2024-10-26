using GroundControlSystem.Communication;
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
            // Ustaw na true, aby używać symulowanego połączenia, false dla rzeczywistego
            USBManager manager = new USBManager(useSimulation: true);

            manager.StartReceivingData();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new GNS());
        }
    }
}
