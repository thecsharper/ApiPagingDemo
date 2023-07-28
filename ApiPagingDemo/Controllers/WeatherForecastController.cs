using System.Text.Json;

using Microsoft.AspNetCore.Mvc;

using ApiPagingDemo.PagingHelpers;
using ApiPagingDemo.Services;

namespace ApiPagingDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IWeatherService _forecasts;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IWeatherService forecasts)
        {
            _logger = logger;
            _forecasts = forecasts;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get([FromQuery] SearchParameters ownerParameters)
        {
            var forecasts = _forecasts.WeatherForecasts().AsQueryable().OrderBy(on => on.Id);

            var result = PagedList<WeatherForecast>.ToPagedList(forecasts,
                                                                ownerParameters.PageNumber,
                                                                ownerParameters.PageSize);

            var metadata = new
            {
                result.TotalCount,
                result.PageSize,
                result.CurrentPage,
                result.TotalPages,
                result.HasNext,
                result.HasPrevious
            };

            var metaOutput = JsonSerializer.Serialize(metadata);
            Response.Headers.Add("X-Pagination", metaOutput);

            _logger.LogInformation(metaOutput);

            return result;
        }
    }
}