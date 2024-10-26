namespace GroundControlSystem.DataModels
{
    /// <summary>
    /// Główna klasa, która agreguje dane telemetryczne z różnych źródeł.
    /// </summary>
    public class TelemetryPacket
    {
        public IMUData IMU { get; set; }
        public BaroData Barometer { get; set; }
        public GPSData GPS { get; set; }
        public TSSData TSS { get; set; }
        public TVSData TVS { get; set; }

        public TelemetryPacket()
        {
            IMU = new IMUData();
            Barometer = new BaroData();
            GPS = new GPSData();
            TSS = new TSSData();
            TVS = new TVSData();
        }

        /// <summary>
        /// Serializuje dane telemetryczne do formatu CSV.
        /// </summary>
        public string ToCSV()
        {
            return $"{IMU.AccX};{IMU.AccY};{IMU.AccZ};{IMU.GyroX};{IMU.GyroY};{IMU.GyroZ};{IMU.MagX};{IMU.MagY};{IMU.MagZ};" +
                   $"{IMU.VerVel};{IMU.VelAcc};{IMU.Pitch};{IMU.Roll};{IMU.Heading};{Barometer.Pressure};{Barometer.Temperature};" +
                   $"{Barometer.Altitude};{GPS.Year};{GPS.Month};{GPS.Day};{GPS.Hours};{GPS.Minutes};{GPS.Seconds};" +
                   $"{GPS.Latitude};{GPS.Longitude};{GPS.SatellitesUsed};{GPS.AltitudeGPS};{GPS.SpeedOverGround};{GPS.CourseOverGround};" +
                   $"{GPS.GNSSMode};{TSS.PWMsignalServo1};{TSS.PWMsignalServo2};{TSS.PWMsignalServo3};{TSS.PWMsignalServo4};" +
                   $"{TVS.PWMsignalServo5};{TVS.PWMsignalServo6}";
        }
    }
}
