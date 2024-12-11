using LeagueMAUI.Models;
using LeagueMAUI.Services;
using LeagueMAUI.Validations;

namespace LeagueMAUI.Pages;

public partial class CreatePlayerPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private readonly Club _club;
    private readonly int _clubId;
    private bool _loginPageDisplayed = false;
    private IEnumerable<Position> _positions;
    public CreatePlayerPage(ApiService apiService, IValidator validator, Club club, int clubId)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
        _club = club;
        _clubId = clubId;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadPositionsAsync();
    }

    private async Task LoadPositionsAsync()
    {
        var listPositions = await GetPositionsAsync();
        if (listPositions.Any())
        {
            _positions = listPositions;
            PickerPosition.ItemsSource = _positions.ToList(); 
        }
    }

    private async Task<IEnumerable<Position>> GetPositionsAsync()
    {
        try
        {
            var (positions, errorMessage) = await _apiService.GetPositions();

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return Enumerable.Empty<Position>();
            }
            if (errorMessage == "NotFound")
            {
                await DisplayAlert("Alert", "No matches found for this round.", "OK");
                return Enumerable.Empty<Position>();
            }
            if (positions is null)
            {
                await DisplayAlert("Error", errorMessage ?? "No positions found.", "OK");
                return Enumerable.Empty<Position>();
            }
            return positions;
        }
        catch (Exception)
        {
            await DisplayAlert("Error", "There was an error retrieving the orders. Please try again later.", "OK");
            return Enumerable.Empty<Position>();
        }

    }
    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }
    

    private async void CreatePlayer_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(EntPlayerName.Text))
        {
            await DisplayAlert("Error", "Please enter the player's name.", "OK");
            return;
        }

        if (PickerPosition.SelectedItem is not Position selectedPosition)
        {
            await DisplayAlert("Error", "Please select a position.", "OK");
            return;
        }

        var player = new PlayerIn
        {
            Name = EntPlayerName.Text,
            PositionId = selectedPosition.Id,
            ClubId = _clubId
        };

        try
        {
            var result = await _apiService.CreatePlayerAsync(player);
            if (!result.HasError)
            {
                await DisplayAlert("Success", "Player successfully created!", "OK");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Erro", "Erro ao salvar o jogador.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error saving the player: {ex.Message}", "OK");
        }
    }
}