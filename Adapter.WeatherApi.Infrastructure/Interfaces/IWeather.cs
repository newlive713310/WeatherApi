using Adapter.WeatherApi.Infrastructure.Models.Entities;

namespace Adapter.WeatherApi.Infrastructure.Interfaces
{
    public interface IWeather
    {
        Task<WeatherReply> GetWeatherByCityName(string city, string correlationId);
        Task<WeatherReply> GetWeatherByCoordinates(string lat, string lon, string correlationId);
    }
}
