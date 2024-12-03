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
        }
    }
}
