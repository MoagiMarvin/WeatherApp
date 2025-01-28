using WeatherApp.ViewModels;

namespace WeatherApp.Views;

public partial class WeatherPage : ContentPage
{
    private readonly Dictionary<string, (Color start, Color end)> _weatherGradients = new()
    {
        { "Clear", (Color.Parse("#87CEEB"), Color.Parse("#1E90FF")) },        // Sunny
        { "Clouds", (Color.Parse("#A9A9A9"), Color.Parse("#696969")) },       // Cloudy
        { "Rain", (Color.Parse("#4682B4"), Color.Parse("#000080")) },         // Rainy
        { "Snow", (Color.Parse("#E0FFFF"), Color.Parse("#B0E0E6")) },         // Snowy
        { "Thunderstorm", (Color.Parse("#4B0082"), Color.Parse("#191970")) }, // Stormy
        { "Mist", (Color.Parse("#B8B8B8"), Color.Parse("#A9A9A9")) },        // Misty
        { "Default", (Color.Parse("#87CEEB"), Color.Parse("#1E90FF")) }       // Default blue sky
    };

    public WeatherPage(WeatherViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;

        // Subscribe to property changes on the ViewModel
        viewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(WeatherViewModel.WeatherData) &&
            ((WeatherViewModel)BindingContext).WeatherData != null)
        {
            UpdateBackground();
        }
    }

    private void UpdateBackground()
    {
        var weatherData = ((WeatherViewModel)BindingContext).WeatherData;
        if (weatherData == null) return;

        var weatherMain = weatherData.Description?.Split(' ')[0] ?? "Default";
        var (startColor, endColor) = _weatherGradients.FirstOrDefault(
            x => weatherMain.Contains(x.Key, StringComparison.OrdinalIgnoreCase)
        ).Value;

        if (startColor == default && endColor == default)
        {
            (startColor, endColor) = _weatherGradients["Default"];
        }

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