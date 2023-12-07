namespace SpaceWeatherAPI;

public class WeatherData
{
    public int Id { get; set; }
    public int SpaceObjectId { get; set; }
    public string TemperatureC { get; set; }
    public string Summary { get; set; }
    public DateTime Date { get; set; }
}