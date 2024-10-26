namespace GroundControlSystem.DataModels
{
    /// <summary>
    /// Model danych GPS.
    /// </summary>
    public class GPSData
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public int Seconds { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int SatellitesUsed { get; set; }
        public float AltitudeGPS { get; set; }
        public float SpeedOverGround { get; set; }
        public float CourseOverGround { get; set; }
        public string GNSSMode { get; set; }
    }
}
