using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

using ApiPagingDemo.Services;
using ApiPagingDemo.PagingHelpers;

namespace ApiPagingDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get([FromQuery] SearchParameters ownerParameters)
        {
            var service = new WeatherService().WeatherForecasts().AsQueryable();

            var result = PagedList<WeatherForecast>.ToPagedList(service.OrderBy(on => on.Id),
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