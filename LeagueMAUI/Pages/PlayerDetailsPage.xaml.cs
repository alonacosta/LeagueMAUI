using LeagueMAUI.Models;
using LeagueMAUI.Services;
using LeagueMAUI.Validations;

namespace LeagueMAUI.Pages;

public partial class PlayerDetailsPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private bool _loginPageDisplayed = false;
    private readonly Player _player;
    private readonly int _clubId;   
    private IEnumerable<Position> _positions;
    public PlayerDetailsPage(ApiService apiService, IValidator validator, Player player, int clubId)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
        _player = player;
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
            EntPlayerName.Text = _player.Name;
            ImgBtnPlayer.Source = _player.ImageUrl;
            _positions = listPositions;
            PickerPosition.ItemsSource = _positions.ToList();

            PickerPosition.SelectedItem = _positions.FirstOrDefault(pos => pos.Name == _player.PositionName);
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

            //CvMatches.ItemsSource = matches;

            return positions;
        }
        catch (Exception)
        {
            await DisplayAlert("Error", "There was an error retrieving the orders. Please try again later.", "OK");
            return Enumerable.Empty<Position>();
        }

    }

    private async Task<byte[]?> SelectImageAsync()
    {
        try
        {
            var file = await MediaPicker.PickPhotoAsync();

            if (file is null) return null;

            using (var stream = await file.OpenReadAsync())
            using (var memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }
        catch (FeatureNotSupportedException)
        {
            await DisplayAlert("Error", "The functionality is not supported on the device", "Ok");
        }
        catch (PermissionException)
        {
            await DisplayAlert("Error", "Permissions not granted to access the camera or gallery", "Ok");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Image selection error: {ex.Message}", "Ok");
        }
        return null;
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }

    private async void DeletePlayer_Clicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Confirm", "Are you sure you want to delete this player?", "Yes", "No");
        if (!confirm) return;

        try
        {
            var (isSuccess, message) = await _apiService.DeletePlayerAsync(_player.Id);
            if (isSuccess)
            {
                await DisplayAlert("Success", "Player deleted successfully!", "OK");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Error", message ?? "Error deleting the player.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error deleting the player: {ex.Message}", "OK");
        }
    }

    private async void EditPlayer_Clicked(object sender, EventArgs e)
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
            var (isSuccess, message) = await _apiService.UpdatePlayerAsync(_player.Id, player);
            if (isSuccess)
            {
                await DisplayAlert("Success", "Player successfully updated!", "OK");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Error", "Error saving the player.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error saving the player: {ex.Message}", "OK");
        }
    }

    private async void ImgBtnPlayer_Clicked(object sender, EventArgs e)
    {
        try
        {
            var imageArray = await SelectImageAsync();
            if (imageArray is null)
            {
                await DisplayAlert("Error", "Image could not be uploaded", "Ok");
                return;
            }
            ImgBtnPlayer.Source = ImageSource.FromStream(() => new MemoryStream(imageArray));

            var response = await _apiService.UploadImagePlayer(imageArray, _player.Id);
            if (response.Data)
            {
                await DisplayAlert("", "Image uploaded successfully", "Ok");                
            }
            else
            {
                await DisplayAlert("Error", response.ErrorMessage ?? "An unknown error has occurred", "Cancel");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An unexpected error has occurred: {ex.Message}", "Ok");
        }
    }
}