using Microsoft.Extensions.Logging;
using WeatherApp.Services;
using WeatherApp.ViewModels;
using WeatherApp.Views;

namespace WeatherApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            builder.Services.AddSingleton<WeatherService>();

            // Register pages and view models
            builder.Services.AddSingleton<WeatherService>();
            builder.Services.AddSingleton<WeatherViewModel>();
            builder.Services.AddSingleton<WeatherPage>();
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
