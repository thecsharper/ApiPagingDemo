namespace ApiPagingDemo.Services
{
    public interface IWeatherService
    {
        IEnumerable<WeatherForecast> WeatherForecasts();
    }
}