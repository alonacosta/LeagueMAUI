using LeagueMAUI.Services;
using LeagueMAUI.Validations;

namespace LeagueMAUI.Pages;

public partial class ProfilePage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private bool _loginPageDisplayed = false;
    private bool _isDataLoaded = false;
    public ProfilePage(ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
        _validator = validator;
        LblUserName.Text = Preferences.Get("name", string.Empty);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (!_isDataLoaded)
        {
            ImgBtnProfile.Source = await GetImageProfile();
            _isDataLoaded = false;
        }

    }

    private async Task<string?> GetImageProfile()
    {
        string imageDefault = AppConfig.ProfileDefaultImage;

        var (response, errorMessage) = await _apiService.GetImageUserProfile(Preferences.Get("username", ""));

        if (errorMessage is not null)
        {
            switch (errorMessage)
            {
                case "Unauthorized":
                    if (!_loginPageDisplayed)
                    {
                        await DisplayLoginPage();
                        return null;
                    }
                    break;
                default:
                    await DisplayAlert("Erro", errorMessage ?? "It was not possible to obtain the image.", "OK");
                    return imageDefault;
            }
        }
        if (response?.ImageUrl is not null)
        {
            return response.ImageUrl; 
        }

        return imageDefault;
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

    private async void ImgBtnProfile_Clicked(object sender, EventArgs e)
    {
        try
        {
            var imageArray = await SelectImageAsync();
            if (imageArray is null)
            {
                await DisplayAlert("Error", "Image could not be uploaded", "Ok");
                return;
            }
            ImgBtnProfile.Source = ImageSource.FromStream(() => new MemoryStream(imageArray));

            var response = await _apiService.UploadImageUser(imageArray);
            if (response.Data)
            {
                await DisplayAlert("", "Image uploaded successfully", "Ok");
                _isDataLoaded = false;
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

    private void TapMatches_Tapped(object sender, TappedEventArgs e)
    {
        Navigation.PushAsync(new MatchesPage(_apiService, _validator));
    }

    private void MyAccount_Tapped(object sender, TappedEventArgs e)
    {
        Navigation.PushAsync(new MyAccountPage(_apiService, _validator));
    }

    private void Club_Tapped(object sender, TappedEventArgs e)
    {
        Navigation.PushAsync(new ClubsPage(_apiService, _validator));
    }

    private void BtnLogout_Clicked(object sender, EventArgs e)
    {
        Preferences.Set("accesstoken", string.Empty);
        Application.Current!.MainPage = new NavigationPage(new LoginPage(_apiService, _validator));
    }

    private void TabAboutAuthor_Tapped(object sender, TappedEventArgs e)
    {
        Navigation.PushAsync(new AboutAuthorPage(_apiService, _validator));
    }
}