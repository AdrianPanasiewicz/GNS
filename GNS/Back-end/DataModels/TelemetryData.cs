using SerialCom.DataModel;
using System;
using System.Globalization;

namespace SerialCom
{
    public class TelemetryData
    {
        public LoRaData LoRa { get; set; }
        public TimeData Time { get; set; }
        public IMUData IMU { get; set; }
        public BaroData Baro { get; set; }
        public GPSData GPS { get; set; }

        public TelemetryData()
        {
            LoRa = new LoRaData();
            Time = new TimeData();
            IMU = new IMUData();
            Baro = new BaroData();
            GPS = new GPSData();
        }

        public override string ToString()
        {
            return
                "\nLoRa:\n" + $"Message length: {LoRa.MsgLength}\n" + $"RSSI: {LoRa.RSSI}\n" + $"SNR: {LoRa.SNR}\n" +
                $"\nTime: {Time.TimeStamp}\n\n" +
                "IMU:\n" + $"AccXYZ: \t{IMU.AccX}\t {IMU.AccY}\t {IMU.AccZ}\n" +
                $"GyroXYZ: \t{IMU.GyroX}\t {IMU.GyroY}\t {IMU.GyroZ}\n" +
                $"MagXYZ: \t{IMU.MagX}\t {IMU.MagY}\t {IMU.MagZ}\n" +
                $"\nHeading: \t{IMU.Heading}\t Pitch: {IMU.Pitch}\t Roll: {IMU.Roll}\n\n" +
                "Baro:\n" + $"AccZInertial: {Baro.AccZInertial}\t VerticalVelocity: {Baro.VerticalVelocity}\n" +
                $"Pressure: {Baro.Pressure}\t Altitude: {Baro.Altitude}\n\n" +
                "GPS:\n" + $"Latitude: {GPS.Latitude}\t Longitude: {GPS.Longitude}";
        }

        public string ToCSV()
        {
            return string.Join(",",
                LoRa.MsgLength, LoRa.RSSI, LoRa.SNR,
                Time.TimeStamp,
                IMU.AccX, IMU.AccY, IMU.AccZ,
                IMU.GyroX, IMU.GyroY, IMU.GyroZ,
                IMU.MagX, IMU.MagY, IMU.MagZ,
                IMU.Heading, IMU.Pitch, IMU.Roll,
                Baro.AccZInertial, Baro.VerticalVelocity,
                Baro.Pressure, Baro.Altitude,
                GPS.Latitude, GPS.Longitude,
                LoRa.RSSI, LoRa.SNR
            );
        }
    }
}
