using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NewsWebsite.Components.Weather
{
    public class WeatherViewComponent : ViewComponent
    {
        private readonly HttpClient _httpClient;

        public WeatherViewComponent(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var apiKey = "47bb5292a5e048759d6140931242708"; // Thay YOUR_API_KEY bằng API key từ WeatherAPI
            var city = "Ho Chi Minh";
            var url = $"http://api.weatherapi.com/v1/current.json?key={apiKey}&q={city}&aqi=no";

            var response = await _httpClient.GetStringAsync(url);
            var weatherData = JsonConvert.DeserializeObject<dynamic>(response);

            return View(weatherData);
        }
    }
}
