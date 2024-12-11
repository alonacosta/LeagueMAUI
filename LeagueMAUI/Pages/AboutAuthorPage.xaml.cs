using LeagueMAUI.Services;
using LeagueMAUI.Validations;

namespace LeagueMAUI.Pages;

public partial class AboutAuthorPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    public AboutAuthorPage(ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
    }
}