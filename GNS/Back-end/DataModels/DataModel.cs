using System;

namespace SerialCom.DataModel
{
    public class TimeData
    {
         public DateTime TimeStamp { get; set; }
    }

    public class IMUData
    {
        public double AccX { get; set; }
        public double AccY { get; set; }
        public double AccZ { get; set; }
        public double GyroX { get; set; }
        public double GyroY { get; set; }
        public double GyroZ { get; set; }
        public double MagX { get; set; }
        public double MagY { get; set; }
        public double MagZ { get; set; }
        public double Heading {  get; set; }
        public double Pitch { get; set; }
        public double Roll { get; set; }
    }

    public class BaroData 
    {
        public double AccZInertial { get; set; }
        public double VerticalVelocity { get; set; }
        public double Pressure { get; set; }
        public double Altitude { get; set; }
    }

    public class GPSData 
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}