using LeagueMAUI.Models;
using LeagueMAUI.Services;
using LeagueMAUI.Validations;

namespace LeagueMAUI.Pages;

public partial class MyAccountPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private bool _loginPageDisplayed = false;
    private bool _isDataLoaded = false;
    public MyAccountPage(ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
        _validator = validator;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (!_isDataLoaded)
        {
            await LoadDataAsync();
            _isDataLoaded = true;
        }
    }
    private async Task LoadDataAsync()
    {
        await GetUserInfoAsync();
    }
    private async Task<UserInfo?> GetUserInfoAsync()
    {
        try
        {
            var (userInfo, errorMessage) = await _apiService.GetUserInfo(Preferences.Get("userid", ""));

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return null;
            }

            if (userInfo == null)
            {
                await DisplayAlert("Error", errorMessage ?? "User details not available.", "OK");
                return null;
            }

            ImgBtnProfile.Source = userInfo.ImageUrl;
            LblUserName.Text = userInfo.Name;
            LblEmail.Text = userInfo.Email;
            LblPhone.Text = userInfo.Phone;

            return userInfo;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An unexpected error has occurred: {ex.Message}", "OK");
            return null;
        }
    }
    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }
}