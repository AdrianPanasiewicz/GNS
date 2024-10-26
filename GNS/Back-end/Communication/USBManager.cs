using System;

namespace GroundControlSystem.Communication
{
    /// <summary>
    /// Klasa zarządzająca, odpowiedzialna za wybór, czy używamy rzeczywistego USB, czy symulacji.
    /// </summary>
    public class USBManager
    {
        public IUSBReceiver usbReceiver;

        /// <summary>
        /// Tworzy instancję menedżera USB. 
        /// W zależności od parametru useSimulation, wybiera rzeczywiste połączenie lub symulowane.
        /// </summary>
        /// <param name="useSimulation">Ustaw true, jeśli chcesz używać symulacji, false dla rzeczywistego USB.</param>
        public USBManager(bool useSimulation)
        {
            if (useSimulation)
            {
                usbReceiver = new SimulatedUSBReceiver();
            }
            else
            {
                usbReceiver = new USBReceiver();
            }
        }

        /// <summary>
        /// Rozpoczyna proces odbierania danych z wybranego źródła (USB lub symulacja).
        /// </summary>
        public void StartReceivingData()
        {
            usbReceiver.InitializeConnection();
            var data = usbReceiver.ReceiveData();
            Console.WriteLine($"Odebrano dane: {BitConverter.ToString(data)}");
            usbReceiver.CloseConnection();
        }
    }
}
