using LeagueMAUI.Models;
using LeagueMAUI.Services;
using LeagueMAUI.Validations;

namespace LeagueMAUI.Pages;

public partial class OfficePlayersPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private readonly Club _club;
    private readonly int _clubId;
    private bool _loginPageDisplayed = false;
    public OfficePlayersPage(ApiService apiService, IValidator validator, Club club)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
        _club = club;
        _clubId = club.Id;
    }

    protected override async void OnAppearing()
    {
        await GetGymsAsync();
    }

    private async Task<IEnumerable<Player>> GetGymsAsync()
    {
        try
        {
            var (players, errorMessage) = await _apiService.GetPlayers(_clubId);

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return Enumerable.Empty<Player>();
            }
            if (errorMessage == "NotFound")
            {
                await DisplayAlert("Alert", "No players found for this club.", "OK");
                return Enumerable.Empty<Player>();
            }
            if (players is null)
            {
                await DisplayAlert("Error", errorMessage ?? "No players found for this club.", "OK");
                return Enumerable.Empty<Player>();
            }

            CvPlayersList.ItemsSource = players;

            return players;
        }
        catch (Exception)
        {
            await DisplayAlert("Error", "There was an error retrieving the orders. Please try again later.", "OK");
            return Enumerable.Empty<Player>();
        }

    }
    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }

    private void CvPlayersList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var currentSelection = e.CurrentSelection.FirstOrDefault() as Player;

        if (currentSelection is null)
            return;

        Navigation.PushAsync(new PlayerDetailsPage(_apiService, _validator, currentSelection, _clubId));

        ((CollectionView)sender).SelectedItem = null;
    }

    private void BtnCreate_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new CreatePlayerPage(_apiService, _validator, _club, _clubId));
    }
}