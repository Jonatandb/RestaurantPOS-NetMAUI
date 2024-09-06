using RestaurantPOS.Data;

namespace RestaurantPOS
{
    public partial class App : Application
    {
        private readonly DatabaseService _databaseService;

        public App(DatabaseService databaseService)
        {
            InitializeComponent();

            MainPage = new AppShell();

            _databaseService = databaseService;

        }

        protected override async void OnStart()
        {
            base.OnStart();

            // Initialize and seed the database
            await _databaseService.InitializeDatabase();

        }
    }
}
