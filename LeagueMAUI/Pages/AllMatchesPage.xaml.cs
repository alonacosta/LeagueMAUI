using LeagueMAUI.Models;
using LeagueMAUI.Services;
using LeagueMAUI.Validations;

namespace LeagueMAUI.Pages;

public partial class AllMatchesPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private bool _loginPageDisplayed = false;
    private readonly int _roundId;
    public AllMatchesPage(ApiService apiService, IValidator validator, int roundId)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
        _roundId = roundId;
    }

    protected override async void OnAppearing()
    {
        await GetMatchesAsync();
    }

    private async Task<IEnumerable<Match>> GetMatchesAsync()
    {
        try
        {
            var (matches, errorMessage) = await _apiService.GetMatches(_roundId);

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return Enumerable.Empty<Match>();
            }
            if (errorMessage == "NotFound")
            {
                await DisplayAlert("Alert", "No matches found for this round.", "OK");
                return Enumerable.Empty<Match>();
            }
            if (matches is null)
            {
                await DisplayAlert("Error", errorMessage ?? "No matches found for this round.", "OK");
                return Enumerable.Empty<Match>();
            }

            CvMatches.ItemsSource = matches;

            return matches;
        }
        catch (Exception)
        {
            await DisplayAlert("Error", "There was an error retrieving the orders. Please try again later.", "OK");
            return Enumerable.Empty<Match>();
        }

    }
    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }

    private void CvMatches_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

    }
}