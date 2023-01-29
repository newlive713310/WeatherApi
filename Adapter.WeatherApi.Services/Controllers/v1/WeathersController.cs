using Adapter.WeatherApi.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Adapter.WeatherApi.Services.Controllers.v1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [EnableCors()]
    public class WeathersController : ControllerBase
    {
        private readonly IWeather _repo;
        public WeathersController(
            IWeather repo
            )
        {
            _repo = repo;
        }
        /// <summary>
        /// Get weather data by city name
        /// </summary>
        /// <param name="city">city name</param>
        /// <param name="correlationId">correlationId for checking in logs</param>
        /// <returns></returns>
        [HttpGet("[action]")]
        public async Task<IActionResult> GetWeatherByCityName(string city, [FromHeader] string correlationId)
        {
            try
            {
                var response = await _repo.GetWeatherByCityName(city, correlationId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex.Message);
                throw;
            }
            finally { }
        }
        /// <summary>
        /// Get weather data by coordinates
        /// </summary>
        /// <param name="lat">latitude</param>
        /// <param name="lon">longitude</param>
        /// <param name="correlationId">correlationId for checking in logs</param>
        /// <returns></returns>
        [HttpGet("[action]")]
        public async Task<IActionResult> GetWeatherByCoordinates(string lat, string lon, [FromHeader] string correlationId)
        {
            try
            {
                var response = await _repo.GetWeatherByCoordinates(lat, lon, correlationId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex.Message);
                throw;
            }
            finally { }
        }
    }
}
