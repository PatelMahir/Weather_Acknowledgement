namespace Weather_Acknowledgement.Models
{
    public class WeatherReading
    {
        public int Id { get; set; }
        public int WeatherStationId { get; set; }
        public WeatherStation WeatherStation { get; set; }
        public DateTime Timestamp { get; set; }
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public double WindSpeed { get; set; }
        public string WindDirection { get; set; }
        public double Precipitation { get; set; }
    }
}