using WeatherApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace WeatherApp.Views;

public partial class WeatherPage : ContentPage
{
    public WeatherPage(WeatherViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}