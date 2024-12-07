using LeagueMAUI.Models;
using LeagueMAUI.Services;
using LeagueMAUI.Validations;

namespace LeagueMAUI.Pages;

public partial class MatchesPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private bool _isDataLoaded = false;
    private bool _loginPageDisplayed = false;
    public MatchesPage(ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
    }

    protected override async void OnAppearing()
    {
        await GetRoundsAsync();
    }

    private async Task<IEnumerable<Round>> GetRoundsAsync()
    {
        try
        {
            loadRoundIndicator.IsRunning = true;
            loadRoundIndicator.IsVisible = true;

            var (rounds, errorMessage) = await _apiService.GetRounds();

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return Enumerable.Empty<Round>();
            }
            if (errorMessage == "NotFound")
            {
                await DisplayAlert("Alert", "No rounds found.", "OK");
                return Enumerable.Empty<Round>();
            }
            if (rounds is null)
            {
                await DisplayAlert("Erro", errorMessage ?? "No rounds found.", "OK");
                return Enumerable.Empty<Round>();
            }

            CvRounds.ItemsSource = rounds;

            return rounds;
        }
        catch (Exception)
        {
            await DisplayAlert("Erro", "Ocorreu um erro ao obter os pedidos. Tente novamente mais tarde.", "OK");
            return Enumerable.Empty<Round>();
        }
        finally
        {
            loadRoundIndicator.IsRunning = false;
            loadRoundIndicator.IsVisible = false;
        }
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }

    private void CvRounds_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var currentSelection = e.CurrentSelection.FirstOrDefault() as Round;

        if (currentSelection is null)
            return;

        Navigation.PushAsync(new AllMatchesPage(_apiService, _validator, currentSelection.Id));

        ((CollectionView)sender).SelectedItem = null;
    }
}