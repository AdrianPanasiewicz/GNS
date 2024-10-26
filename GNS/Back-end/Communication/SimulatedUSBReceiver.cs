using System;

namespace GroundControlSystem.Communication
{
    /// <summary>
    /// Symulowany odbiornik USB. Generuje losowe dane dla celów testowych i debugowania.
    /// </summary>
    public class SimulatedUSBReceiver : IUSBReceiver
    {
        private bool _isConnected;

        /// <summary>
        /// Symuluje inicjalizację połączenia USB.
        /// </summary>
        public void InitializeConnection()
        {
            _isConnected = true;
            Console.WriteLine("Symulowane połączenie USB nawiązane.");
        }

        /// <summary>
        /// Generuje losowe dane telemetryczne, które symulują dane z urządzenia USB.
        /// </summary>
        public byte[] ReceiveData()
        {
            if (!_isConnected) throw new InvalidOperationException("Brak symulowanego połączenia USB.");

            Random rand = new Random();
            byte[] simulatedData = new byte[64];

            for (int i = 0; i < simulatedData.Length; i++)
            {
                simulatedData[i] = (byte)rand.Next(0, 256);
            }

            Console.WriteLine("Symulowane dane odebrane.");
            return simulatedData;
        }

        /// <summary>
        /// Symuluje zamknięcie połączenia USB.
        /// </summary>
        public void CloseConnection()
        {
            _isConnected = false;
            Console.WriteLine("Symulowane połączenie USB zamknięte.");
        }
    }
}
