using Adapter.WeatherApi.Infrastructure.Interfaces;
using Adapter.WeatherApi.Infrastructure.Models.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace Adapter.WeatherApi.Infrastructure.Repos
{
    public class WeatherRepo : IWeather
    {
        private readonly ILogger<WeatherRepo> _logger;
        private readonly string _apiKey = String.Empty;
        private readonly string _weatherApi = String.Empty;
        private readonly string _mongoDb = String.Empty;

        public WeatherRepo(
            ILogger<WeatherRepo> logger,
            IConfiguration configuration
            )
        {
            _logger = logger;
            _apiKey = configuration.GetSection("Api-Key").Value;
            _weatherApi = configuration.GetSection("WeatherApi").Value;
            _mongoDb = configuration.GetSection("MongoDb").Value;
        }
        public async Task<WeatherReply> GetWeatherByCityName(string city, string correlationId)
        {
            // Creating empty returned model
            var reply = WeatherReply.GetEmptyModel();

            _logger.LogDebug($"Debug Info | CorrelationId {correlationId}: Querying weather data to external service with parameter {city}");

            using (var clientHttp = new HttpClient())
            {
                try
                {
                    clientHttp.BaseAddress = new Uri(_weatherApi);
                    var response = await clientHttp.GetAsync($"/data/2.5/weather?q={city}&appid={_apiKey}&units=metric");
                    response.EnsureSuccessStatusCode();

                    // Getting data from weather external service
                    var stringResult = await response.Content.ReadAsStringAsync();

                    _logger.LogTrace($"Trace Info | CorrelationId {correlationId}: Returned weather data from {nameof(GetWeatherByCityName)} method {JsonConvert.SerializeObject(stringResult)}");

                    var rawWeather = JsonConvert.DeserializeObject<WeatherResponse>(stringResult);
                    reply = new WeatherReply()
                    {
                        Temp = rawWeather.Main.Temp,
                        Summary = string.Join(",", rawWeather.Weather.Select(x => x.Main)),
                        City = rawWeather.Name
                    };

                    // Creating Mongo client
                    var clientMongo = new MongoClient(_mongoDb);

                    using (var cursor = await clientMongo.ListDatabasesAsync())
                    {
                        _logger.LogDebug($"Debug Info | CorrelationId {correlationId}: Getting db test in MongoDb");
                        var db = clientMongo.GetDatabase("test");

                        // Checking collection WeatherResponses exists
                        var collectionExists = db.ListCollectionNames().ToList().Contains("WeatherResponses");

                        if (!collectionExists)
                        {
                            _logger.LogDebug($"Debug Info | CorrelationId {correlationId}: Creating collection WeatherResponses in MongoDb");
                            await db.CreateCollectionAsync("WeatherResponses");
                        }

                        var weatherResponses = db.GetCollection<BsonDocument>("WeatherResponses");

                        var doc = new BsonDocument
                            {
                                { "Datetime", DateTime.UtcNow},
                                { "CityName", city},
                                { "CorrelationId", correlationId},
                                { "Temp", reply.Temp },
                                { "Summary", reply.Summary },
                                { "City", reply.City }
                            };

                        await weatherResponses.InsertOneAsync(doc);
                    }
                }
                catch (HttpRequestException httpRequestException) { throw; }
                finally { }

                return reply;
            }
        }

        public async Task<WeatherReply> GetWeatherByCoordinates(string lat, string lon, string correlationId)
        {
            // Creating empty returned model
            var reply = WeatherReply.GetEmptyModel();

            _logger.LogDebug($"Debug Info | CorrelationId {correlationId}: Querying weather data to external service with parameter {lat}, {lon}");

            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(_weatherApi);
                    var response = await client.GetAsync($"/data/2.5/weather?lat={lat}&lon={lon}&appid={_apiKey}");
                    response.EnsureSuccessStatusCode();

                    // Getting data from weather external service
                    var stringResult = await response.Content.ReadAsStringAsync();

                    _logger.LogTrace($"Trace Info | CorrelationId {correlationId}: Returned weather data from {nameof(GetWeatherByCityName)} method {JsonConvert.SerializeObject(stringResult)}");

                    var rawWeather = JsonConvert.DeserializeObject<WeatherResponse>(stringResult);
                    reply = new WeatherReply()
                    {
                        Temp = rawWeather.Main.Temp,
                        Summary = string.Join(",", rawWeather.Weather.Select(x => x.Main)),
                        City = rawWeather.Name
                    };

                    // Creating Mongo client
                    var clientMongo = new MongoClient(_mongoDb);

                    using (var cursor = await clientMongo.ListDatabasesAsync())
                    {
                        _logger.LogDebug($"Debug Info | CorrelationId {correlationId}: Getting db test in MongoDb");
                        var db = clientMongo.GetDatabase("test");

                        // Checking collection WeatherResponses exists
                        var collectionExists = db.ListCollectionNames().ToList().Contains("WeatherResponses");

                        if (!collectionExists)
                        {
                            _logger.LogDebug($"Debug Info | CorrelationId {correlationId}: Creating collection WeatherResponses in MongoDb");
                            await db.CreateCollectionAsync("WeatherResponses");
                        }

                        var weatherResponses = db.GetCollection<BsonDocument>("WeatherResponses");

                        var doc = new BsonDocument
                            {
                                { "Datetime", DateTime.UtcNow},
                                { "Latitude", lat},
                                { "Longitude", lon},
                                { "CorrelationId", correlationId},
                                { "Temp", reply.Temp },
                                { "Summary", reply.Summary },
                                { "City", reply.City }
                            };

                        await weatherResponses.InsertOneAsync(doc);
                    }
                }
                catch (HttpRequestException httpRequestException) { throw; }
                finally { }

                return reply;
            }
        }
    }
}
