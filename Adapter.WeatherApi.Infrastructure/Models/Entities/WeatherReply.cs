namespace Adapter.WeatherApi.Infrastructure.Models.Entities
{
    public class WeatherReply
    {
        public string Temp { get; set; }
        public string Summary { get; set; }
        public string City { get; set; }
        public static WeatherReply GetEmptyModel()
        {
            return new WeatherReply();
        }
    }
}
