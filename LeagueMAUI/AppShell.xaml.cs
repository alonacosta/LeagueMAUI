using LeagueMAUI.Models;
using LeagueMAUI.Pages;
using LeagueMAUI.Services;
using LeagueMAUI.Validations;

namespace LeagueMAUI
{
    public partial class AppShell : Shell
    {
        private readonly ApiService _apiService;
        private readonly IValidator _validator;
        private readonly string _roleName;

        public AppShell(ApiService apiService, IValidator validator, string roleName)
        {
            InitializeComponent();
            _apiService = apiService;
            _validator = validator;
            _roleName = roleName;
            ConfigureShell();
        }

        private async void ConfigureShell()
        {
            var dashboardPage = new DashboardPage(_apiService, _validator);
            var clubsPage = new ClubsPage(_apiService, _validator);
           
            var matchesPage = new MatchesPage(_apiService, _validator);
            var profilePage = new ProfilePage(_apiService, _validator);

            //Items.Add(new TabBar
            //{
            //    Items =
            //{
            //    new ShellContent { Title = "Dashboard",Icon = "dashboard",Content = dashboardPage  },
            //    new ShellContent { Title = "Clubs", Icon = "club",Content = clubsPage },               
            //    new ShellContent { Title = "Matches",Icon = "ball",Content = matchesPage },
            //    new ShellContent { Title = "Profile",Icon = "user_profile",Content = profilePage }
            //}
            //});
            var tabBar = new TabBar
            {
                Items =
                {
                    new ShellContent { Title = "Dashboard", Icon = "dashboard", Content = dashboardPage },
                    new ShellContent { Title = "Clubs", Icon = "club", Content = clubsPage },
                    new ShellContent { Title = "Matches", Icon = "ball", Content = matchesPage },
                    new ShellContent { Title = "Profile", Icon = "user_profile", Content = profilePage }
                }
            };

            if(_roleName == "Representative")
            {
                var officePage = new OfficePage(_apiService, _validator);  // Página para representative
                tabBar.Items.Add(new ShellContent
                {
                    Title = "Office",
                    Icon = "office_icon",  
                    Content = officePage
                });
            }

            Items.Add(tabBar);
        }
    }
}
