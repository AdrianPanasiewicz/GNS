using System;

namespace GroundControlSystem.Communication
{
    /// <summary>
    /// Interfejs definiujący podstawowe operacje dla odbiornika USB.
    /// Zawiera metody inicjalizacji, odbierania danych i zamykania połączenia.
    /// </summary>
    public interface IUSBReceiver
    {
        void InitializeConnection();
        byte[] ReceiveData();
        void CloseConnection();
    }
}