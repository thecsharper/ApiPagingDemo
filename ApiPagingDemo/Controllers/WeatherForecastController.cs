using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

using ApiPagingDemo.PagingHelpers;

namespace ApiPagingDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IQueryable<WeatherForecast> _forecasts;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IQueryable<WeatherForecast> forecasts)
        {
            _logger = logger;
            _forecasts = forecasts;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get([FromQuery] SearchParameters ownerParameters)
        {
            var result = PagedList<WeatherForecast>.ToPagedList(_forecasts.OrderBy(on => on.Id),
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