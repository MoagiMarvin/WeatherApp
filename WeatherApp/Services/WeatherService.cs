using System.Diagnostics;
using System.Text.Json;
using WeatherApp.Models;

namespace WeatherApp.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;
        private const string ApiKey = "a6382cd479083c72f6f57ab2de9935e2";
        private const string BaseUrl = "https://api.openweathermap.org/data/2.5";

        public WeatherService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<WeatherData> GetWeatherDataAsync()
        {
            try
            {
                Debug.WriteLine("Starting to get weather data...");

                var location = await GetCurrentLocationAsync();
                Debug.WriteLine($"Location obtained - Lat: {location.Latitude}, Lon: {location.Longitude}");

                var url = $"{BaseUrl}/weather?lat={location.Latitude}&lon={location.Longitude}&appid={ApiKey}&units=metric";
                Debug.WriteLine($"API URL: {url}");

                var response = await _httpClient.GetStringAsync(url);
                Debug.WriteLine($"API Response received: {response}");

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var weatherResponse = JsonSerializer.Deserialize<WeatherResponse>(response, options);

                if (weatherResponse == null)
                    throw new Exception("Failed to deserialize weather response");

                Debug.WriteLine("Successfully deserialized response");

                return new WeatherData
                {
                    Temperature = weatherResponse.Main?.Temp ?? 0,
                    FeelsLike = weatherResponse.Main?.Feels_like ?? 0,
                    Humidity = weatherResponse.Main?.Humidity ?? 0,
                    Description = weatherResponse.Weather?.FirstOrDefault()?.Description ?? "No description available",
                    Icon = weatherResponse.Weather?.FirstOrDefault()?.Icon ?? "",
                    CityName = weatherResponse.Name ?? "Unknown Location",
                    WindSpeed = weatherResponse.Wind?.Speed ?? 0,
                    Latitude = location.Latitude,
                    Longitude = location.Longitude
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in GetWeatherDataAsync: {ex.GetType().Name}");
                Debug.WriteLine($"Error Message: {ex.Message}");
                Debug.WriteLine($"Stack Trace: {ex.StackTrace}");

                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }

                throw;
            }
        }

        private async Task<(double Latitude, double Longitude)> GetCurrentLocationAsync()
        {
            try
            {
                Debug.WriteLine("Checking location permissions...");
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                Debug.WriteLine($"Location permission status: {status}");

                if (status != PermissionStatus.Granted)
                {
                    Debug.WriteLine("Requesting location permission...");
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                    Debug.WriteLine($"Permission request result: {status}");

                    if (status != PermissionStatus.Granted)
                        throw new Exception("Location permission not granted");
                }

                var location = await Geolocation.GetLastKnownLocationAsync();

                if (location == null)
                {
                    Debug.WriteLine("No last known location, requesting current location...");
                    location = await Geolocation.GetLocationAsync(new GeolocationRequest
                    {
                        DesiredAccuracy = GeolocationAccuracy.Medium,
                        Timeout = TimeSpan.FromSeconds(30)
                    });
                }

                if (location == null)
                    throw new Exception("Could not get location");

                return (location.Latitude, location.Longitude);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in GetCurrentLocationAsync: {ex.GetType().Name}");
                Debug.WriteLine($"Error Message: {ex.Message}");
                Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw;
            }
        }
    }
}