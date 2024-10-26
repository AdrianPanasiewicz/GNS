using System;

namespace GroundControlSystem.Communication
{
    /// <summary>
    /// Klasa zarządzająca, odpowiedzialna za wybór, czy używamy rzeczywistego USB, czy symulacji.
    /// </summary>
    public class USBManager
    {
        private IUSBReceiver _usbReceiver;

        /// <summary>
        /// Tworzy instancję menedżera USB. 
        /// W zależności od parametru useSimulation, wybiera rzeczywiste połączenie lub symulowane.
        /// </summary>
        /// <param name="useSimulation">Ustaw true, jeśli chcesz używać symulacji, false dla rzeczywistego USB.</param>
        public USBManager(bool useSimulation)
        {
            if (useSimulation)
            {
                _usbReceiver = new SimulatedUSBReceiver();
            }
            else
            {
                _usbReceiver = new USBReceiver();
            }
        }

        /// <summary>
        /// Rozpoczyna proces odbierania danych z wybranego źródła (USB lub symulacja).
        /// </summary>
        public void StartReceivingData()
        {
            _usbReceiver.InitializeConnection();
            var data = _usbReceiver.ReceiveData();
            Console.WriteLine($"Odebrano dane: {BitConverter.ToString(data)}");
            _usbReceiver.CloseConnection();
        }
    }
}
