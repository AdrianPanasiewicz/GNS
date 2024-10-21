namespace LikwidatorBackend
{
    /// <summary>
    /// A class representing the telemetry data received from the rocket.
    /// </summary>
    public class TelemetryData
    {
        public float GyroX { get; set; }
        public float GyroY { get; set; }
        public float GyroZ { get; set; }
        public float VerVel { get; set; }
        public float VelAcc { get; set; }
        public float Pitch { get; set; }
        public float Roll { get; set; }
        public float Heading { get; set; }
        public float Altitude { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public float SpeedOverGround { get; set; }
        public float CourseOverGround { get; set; }
    }
}
