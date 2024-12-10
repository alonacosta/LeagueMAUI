using LeagueMAUI.Models;
using LeagueMAUI.Services;
using LeagueMAUI.Validations;

namespace LeagueMAUI.Pages;

public partial class LoginPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private string _roleName;

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
            var role = await GetRoleAsync(EntEmail.Text);
            _roleName = role.RoleName;
            Application.Current!.MainPage = new AppShell(_apiService, _validator, _roleName);
        }
        else
        {
            await DisplayAlert("Error", "Something's gone wrong", "Cancel");
        }
    }

    public async Task<Role> GetRoleAsync(string username)
    {
        try
        {
            var (role, errorMessage) = await _apiService.GetUserRole(EntEmail.Text);

            //if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            //{
            //    await DisplayLoginPage();
            //    return null;  // Ou o valor padrão, conforme sua lógica
            //}
            //if (errorMessage == "NotFound")
            //{
            //    await DisplayAlert("Alert", "No clubs found.", "OK");
            //    return null;  // Ou outra lógica adequada
            //}
            //_roleName = role.RoleName;
            return role;  
        }
        catch (Exception)
        {
            await DisplayAlert("Error", "There was an error retrieving the orders. Please try again later.", "OK");
            return null;
        }
    }

    private async void TapRecoverPassword_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new RecoverPasswordPage(_apiService, _validator));
    }
}