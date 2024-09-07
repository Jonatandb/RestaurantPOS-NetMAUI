using RestaurantPOS.Data;

namespace RestaurantPOS
{
    public partial class App : Application
    {

        public App(DatabaseService databaseService)
        {
            InitializeComponent();

            MainPage = new AppShell();

            Task.Run(async () => await databaseService.InitializeDatabase())
                .GetAwaiter()
                .GetResult();
        }

    }
}
