namespace GroundControlSystem.DataModels
{
    /// <summary>
    /// Model danych dla czujników IMU.
    /// </summary>
    public class IMUData
    {
        public float AccX { get; set; }
        public float AccY { get; set; }
        public float AccZ { get; set; }
        public float GyroX { get; set; }
        public float GyroY { get; set; }
        public float GyroZ { get; set; }
        public float MagX { get; set; }
        public float MagY { get; set; }
        public float MagZ { get; set; }
        public float VerVel { get; set; }
        public float VelAcc { get; set; }
        public float Pitch { get; set; }
        public float Roll { get; set; }
        public float Heading { get; set; }
    }
}
