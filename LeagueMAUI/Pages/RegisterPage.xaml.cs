using LeagueMAUI.Services;
using LeagueMAUI.Validations;

namespace LeagueMAUI.Pages;

public partial class RegisterPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    public RegisterPage(ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
    }

    private async void BtnSignup_Clicked(object sender, EventArgs e)
    {
        if (await _validator.Validate(EntFirstName.Text, EntLastName.Text, EntEmail.Text, EntPhoneNumber.Text, EntPassword.Text, EntConfirm.Text))
        {

            var response = await _apiService.Register(EntFirstName.Text, EntLastName.Text, EntEmail.Text,
                                                          EntPhoneNumber.Text, EntPassword.Text, EntConfirm.Text);

            if (!response.HasError)
            {
                await DisplayAlert("Success", "Your account has been successfully created. Please confirm your email!!!", "OK");
                await Navigation.PushAsync(new LoginPage(_apiService, _validator));
            }
            else
            {
                await DisplayAlert("Error", "Something's gone wrong!!!", "Cancel");
            }
        }
        else
        {
            string messageError = "";
            messageError += _validator.FirstNameError != null ? $"\n- {_validator.FirstNameError}" : "";
            messageError += _validator.LastNameError != null ? $"\n- {_validator.LastNameError}" : "";
            messageError += _validator.EmailError != null ? $"\n- {_validator.EmailError}" : "";
            messageError += _validator.PhoneError != null ? $"\n- {_validator.PhoneError}" : "";
            messageError += _validator.PasswordError != null ? $"\n- {_validator.PasswordError}" : "";
            messageError += _validator.ConfirmError != null ? $"\n- {_validator.ConfirmError}" : "";

            await DisplayAlert("Error", messageError, "OK");
        }
    }

    private async void TapLogin_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }
}