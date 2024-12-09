using LeagueMAUI.Services;
using LeagueMAUI.Validations;

namespace LeagueMAUI.Pages;

public partial class RecoverPasswordPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    public RecoverPasswordPage(ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
    }

    private async void RecoverPassword_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(EntEmail.Text))
        {
            await DisplayAlert("Error", "Enter your email address", "Cancel");
            return;
        }

        var response = await _apiService.RecoverPassword(EntEmail.Text);

        if (!response.HasError)
        {
            await DisplayAlert("Success", "A reset link has been sent to your email.", "Ok");
            await Navigation.PushAsync(new LoginPage(_apiService, _validator));
        }
        else
        {
            await DisplayAlert("Error", "Something went wrong. Please try again later.", "Cancel");
        }
        
    }
}