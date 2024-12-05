using LeagueMAUI.Pages;
using LeagueMAUI.Services;
using LeagueMAUI.Validations;

namespace LeagueMAUI
{
    public partial class AppShell : Shell
    {
        private readonly ApiService _apiService;
        private readonly IValidator _validator;
        public AppShell(ApiService apiService, IValidator validator)
        {
            InitializeComponent();
            _apiService = apiService;
            _validator = validator;
            ConfigureShell();
        }

        private void ConfigureShell()
        {
            var dashboardPage = new DashboardPage(_apiService, _validator);
            var clubsPage = new ClubsPage(_apiService, _validator);
           
            var matchesPage = new MatchesPage(_apiService, _validator);
            var profilePage = new ProfilePage(_apiService, _validator);

            Items.Add(new TabBar
            {
                Items =
            {
                new ShellContent { Title = "Dashboard",Icon = "dashboard",Content = dashboardPage  },
                new ShellContent { Title = "Clubs", Icon = "club",Content = clubsPage },               
                new ShellContent { Title = "Matches",Icon = "ball",Content = matchesPage },
                new ShellContent { Title = "Profile",Icon = "user_profile",Content = profilePage }
            }
            });
        }
    }
}
