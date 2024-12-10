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
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NDaF5cWWtCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdnWH9cc3RXR2BeUUJxXkc=");
            InitializeComponent();
            _apiService = apiService;
            _validator = validator;
           
            SetMainPage();

        }

        private void SetMainPage()
        {
            var accessToken = Preferences.Get("accesstoken", string.Empty);

            if (string.IsNullOrEmpty(accessToken))
            {
                MainPage = new NavigationPage(new LoginPage(_apiService, _validator));
                return;
            }

            MainPage = new AppShell(_apiService, _validator, "");
        }
    }
}
