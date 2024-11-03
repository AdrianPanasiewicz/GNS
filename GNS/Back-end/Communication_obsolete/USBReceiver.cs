using System;
using LibUsbDotNet;
using LibUsbDotNet.Main;

namespace GroundControlSystem.Communication
{
    /// <summary>
    /// Implementacja rzeczywistego odbiornika USB.
    /// Używa LibUsbDotNet do komunikacji z urządzeniem USB.
    /// </summary>
    public class USBReceiver : IUSBReceiver
    {
        private UsbDevice _device;

        /// <summary>
        /// Inicjalizuje połączenie z urządzeniem USB na podstawie VendorID i ProductID.
        /// </summary>
        public void InitializeConnection()
        {
            UsbDeviceFinder myUsbFinder = new UsbDeviceFinder(0x1234, 0x5678); // Wstaw odpowiednie ID urządzenia
            _device = UsbDevice.OpenUsbDevice(myUsbFinder);

            if (_device == null)
            {
                throw new Exception("Nie znaleziono urządzenia USB.");
            }
        }

        /// <summary>
        /// Odbiera dane z połączenia USB i zwraca je jako tablicę bajtów.
        /// </summary>
        public byte[] ReceiveData()
        {
            if (_device == null) throw new InvalidOperationException("Brak połączenia USB.");

            byte[] buffer = new byte[64]; // Wielkość zależy od danych
            int bytesRead;

            UsbEndpointReader reader = _device.OpenEndpointReader(ReadEndpointID.Ep01);
            ErrorCode ec = reader.Read(buffer, 5000, out bytesRead);

            if (ec != ErrorCode.None)
            {
                throw new Exception("Błąd podczas odczytu danych z USB: " + ec.ToString());
            }

            return buffer;
        }

        /// <summary>
        /// Zamyka połączenie USB.
        /// </summary>
        public void CloseConnection()
        {
            if (_device != null)
            {
                _device.Close();
                _device = null;
            }
        }
    }
}
