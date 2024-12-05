using LeagueMAUI.Services;
using LeagueMAUI.Validations;

namespace LeagueMAUI.Pages;

public partial class MatchesPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    public MatchesPage(ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
    }
}