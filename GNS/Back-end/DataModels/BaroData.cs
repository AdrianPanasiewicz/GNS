namespace GroundControlSystem.DataModels
{
    /// <summary>
    /// Model danych z czujnika barometrycznego.
    /// </summary>
    public class BaroData
    {
        public float Pressure { get; set; }
        public float Temperature { get; set; }
        public float Altitude { get; set; }
    }
}
