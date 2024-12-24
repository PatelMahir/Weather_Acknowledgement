namespace Weather_Acknowledgement.Models
{public class WeatherStation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime InstallationDate { get; set; }
        public List<WeatherReading> Readings { get; set; }
    }
}
