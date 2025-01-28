using WeatherApp.ViewModels;

namespace WeatherApp.Views;

public partial class WeatherPage : ContentPage
{
    private readonly Dictionary<string, (Color start, Color end)> _weatherGradients = new()
    {
        { "clear", (Color.Parse("#87CEEB"), Color.Parse("#1E90FF")) },     // Clear sky
        { "clouds", (Color.Parse("#A9A9A9"), Color.Parse("#696969")) },    // Cloudy
        { "rain", (Color.Parse("#4682B4"), Color.Parse("#000080")) },      // Rain
        { "drizzle", (Color.Parse("#4682B4"), Color.Parse("#000080")) },   // Drizzle
        { "thunderstorm", (Color.Parse("#4B0082"), Color.Parse("#191970"))}, // Thunderstorm
        { "snow", (Color.Parse("#E0FFFF"), Color.Parse("#B0E0E6")) },      // Snow
        { "mist", (Color.Parse("#B8B8B8"), Color.Parse("#A9A9A9")) }       // Mist/Fog
    };

    private readonly WeatherViewModel _viewModel;

    public WeatherPage(WeatherViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;

        // Subscribe to property changes
        _viewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(WeatherViewModel.WeatherData) && _viewModel.WeatherData != null)
        {
            UpdateBackgroundBasedOnWeather();
        }
    }

    private void UpdateBackgroundBasedOnWeather()
    {
        if (_viewModel.WeatherData?.Description == null) return;

        string weatherCondition = _viewModel.WeatherData.Description.ToLower();

        // Find matching weather condition
        var (startColor, endColor) = _weatherGradients.FirstOrDefault(x =>
            weatherCondition.Contains(x.Key)).Value;

        // If no match found, use default clear sky colors
        if (startColor == default)
        {
            (startColor, endColor) = _weatherGradients["clear"];
        }

        // Update the background
        BackgroundGrid.Background = new LinearGradientBrush
        {
            EndPoint = new Point(0, 1),
            GradientStops = new GradientStopCollection
            {
                new GradientStop { Color = startColor, Offset = 0.0f },
                new GradientStop { Color = endColor, Offset = 1.0f }
            }
        };
    }
}