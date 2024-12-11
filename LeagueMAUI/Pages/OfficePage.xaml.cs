using LeagueMAUI.Services;
using LeagueMAUI.Validations;

namespace LeagueMAUI.Pages;

public partial class OfficePage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    public OfficePage(ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
    }

    private void TapClubs_Tapped(object sender, TappedEventArgs e)
    {
        Navigation.PushAsync(new OfficeClubsPage(_apiService, _validator));
    }

    private void MyAccount_Tapped(object sender, TappedEventArgs e)
    {
        Navigation.PushAsync(new ProfilePage(_apiService, _validator));
    }

    private void Round_Tapped(object sender, TappedEventArgs e)
    {

    }
}