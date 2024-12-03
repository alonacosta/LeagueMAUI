using LeagueMAUI.Pages;
using LeagueMAUI.Services;
using LeagueMAUI.Validations;

namespace LeagueMAUI
{
    public partial class App : Application
    {
        private readonly ApiService _apiService;
        private readonly IValidator _validator;
        public App(ApiService apiService, IValidator validator)
        {
            InitializeComponent();
            _apiService = apiService;
            _validator = validator;
            MainPage = new NavigationPage(new LoginPage(_apiService, _validator));
           
        }
    }
}
