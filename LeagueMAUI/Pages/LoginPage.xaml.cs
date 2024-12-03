using LeagueMAUI.Services;
using LeagueMAUI.Validations;

namespace LeagueMAUI.Pages;

public partial class LoginPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    public LoginPage(ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
    }

    private async void BtnSignIn_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(EntEmail.Text))
        {
            await DisplayAlert("Error", "Enter your email address", "Cancel");
            return;
        }

        if (string.IsNullOrEmpty(EntPassword.Text))
        {
            await DisplayAlert("Error", "Enter your password", "Cancel");
            return;
        }

        var response = await _apiService.Login(EntEmail.Text, EntPassword.Text);

        if (!response.HasError)
        {
            Application.Current!.MainPage = new AppShell(_apiService, _validator);
        }
        else
        {
            await DisplayAlert("Error", "Something's gone wrong", "Cancel");
        }
    }

    private void TapRecoverPassword_Tapped(object sender, TappedEventArgs e)
    {

    }
}