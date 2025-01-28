using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WeatherApp.Models;
using WeatherApp.Services;

namespace WeatherApp.ViewModels
{
    public class WeatherViewModel : INotifyPropertyChanged
    {
        private readonly WeatherService _weatherService;
        private WeatherData _weatherData;
        private bool _isBusy;

        public WeatherData WeatherData
        {
            get => _weatherData;
            set
            {
                _weatherData = value;
                OnPropertyChanged();
            }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        public ICommand RefreshCommand { get; }

        public WeatherViewModel(WeatherService weatherService)
        {
            _weatherService = weatherService;
            RefreshCommand = new Command(async () => await LoadWeatherAsync());

            // Load weather data when ViewModel is created
            MainThread.BeginInvokeOnMainThread(async () => await LoadWeatherAsync());
        }

        public async Task LoadWeatherAsync()
        {
            if (IsBusy)
                return;

            try
            {
                Debug.WriteLine("=== Starting weather fetch ===");
                IsBusy = true;
                WeatherData = await _weatherService.GetWeatherDataAsync();
            }
            catch (PermissionException pEx)
            {
                Debug.WriteLine($"Permission error: {pEx.Message}");
                await Shell.Current.DisplayAlert("Permission Error", "Location permission is required.", "OK");
            }
            catch (HttpRequestException hEx)
            {
                Debug.WriteLine($"Network error: {hEx.Message}");
                await Shell.Current.DisplayAlert("Network Error", "Unable to connect to weather service.", "OK");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"General error: {ex.GetType().Name}");
                Debug.WriteLine($"Message: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                await Shell.Current.DisplayAlert("Error", "Unable to load weather data.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}