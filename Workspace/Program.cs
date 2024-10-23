using LikwidatorBackend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestingCharts
{
    internal static class Program
    {
        /// <summary>
        /// Główny punkt wejścia dla aplikacji.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Initialize backend classes
            USBConnection usbConnection = new USBConnection("COM3", 9600); // Example USB port and baud rate

            // Pass USB connection to Form1 for real-time data updates
            Form1 mainForm = new Form1(usbConnection);

            Application.Run(mainForm);
        }
    }
}
