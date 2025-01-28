namespace WeatherApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(Views.WeatherPage), typeof(Views.WeatherPage));
        }
    }
}