using System.Net.Http.Json;
using static Jennifer.Wasm.Admin.Pages.Weather;

namespace Jennifer.Wasm.Admin.Services.Weathers;

public interface IWeatherService
{
    Task<IQueryable<WeatherForecast>> GetWeatherForecastAsync();
}

public class WeatherService : IWeatherService
{
    private readonly HttpClient http;

    public WeatherService(HttpClient http)
    {
        this.http = http;
    }
    public async Task<IQueryable<WeatherForecast>> GetWeatherForecastAsync()
    {
        return (await this.http.GetFromJsonAsync<List<WeatherForecast>>("sample-data/weather.json")).AsQueryable();
    }
}
