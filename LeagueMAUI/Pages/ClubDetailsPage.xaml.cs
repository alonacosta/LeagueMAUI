using LeagueMAUI.Models;
using LeagueMAUI.Services;
using LeagueMAUI.Validations;

namespace LeagueMAUI.Pages;

public partial class ClubDetailsPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private bool _loginPageDisplayed = false;
    private readonly Club _club;
    private readonly int _clubId;

    public ClubDetailsPage(ApiService apiService, IValidator validator, Club club)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
        _club = club;
        _clubId = club.Id;

        ImageClub.Source = _club.ImageFullPath;
        LblNameClub.Text = _club.Name;
        LblStadium.Text = _club.Stadium;
        LblCapacity.Text = _club.Capacity.ToString();
        LblHeadCoach.Text = _club.HeadCoach;
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

            CvPlayers.ItemsSource = players;
            
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
}