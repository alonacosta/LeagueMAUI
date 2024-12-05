using LeagueMAUI.Models;
using LeagueMAUI.Services;
using LeagueMAUI.Validations;

namespace LeagueMAUI.Pages;

public partial class ClubsPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private bool _isDataLoaded = false;
    private bool _loginPageDisplayed = false;
    public ClubsPage(ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
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
        await GetClubsAsync();
    }

    private async Task<IEnumerable<Club>> GetClubsAsync()
    {
        try
        {
            loadIndicator.IsRunning = true;
            loadIndicator.IsVisible = true;

            var (clubs, errorMessage) = await _apiService.GetClubs();

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return Enumerable.Empty<Club>();
            }
            if (errorMessage == "NotFound")
            {
                await DisplayAlert("Alert", "No clubs found.", "OK");
                return Enumerable.Empty<Club>();
            }
            if (clubs is null)
            {
                await DisplayAlert("Erro", errorMessage ?? "No clubs found.", "OK");
                return Enumerable.Empty<Club>();
            }

            CvClubs.ItemsSource = clubs;

            return clubs;
        }
        catch (Exception)
        {
            await DisplayAlert("Erro", "Ocorreu um erro ao obter os pedidos. Tente novamente mais tarde.", "OK");
            return Enumerable.Empty<Club>();
        }
        finally
        {
            loadIndicator.IsRunning = false;
            loadIndicator.IsVisible = false;
        }
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }

    private void CvClubs_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var currentSelection = e.CurrentSelection.FirstOrDefault() as Club;

        if (currentSelection is null)
            return;

        Navigation.PushAsync(new ClubDetailsPage(_apiService, _validator, currentSelection));

        ((CollectionView)sender).SelectedItem = null;
    }

    //private void BtnTeam_Clicked(object sender, EventArgs e)
    //{

    //}
}