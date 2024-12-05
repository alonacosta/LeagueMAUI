using LeagueMAUI.Models;
using LeagueMAUI.Services;
using LeagueMAUI.Validations;

namespace LeagueMAUI.Pages;

public partial class DashboardPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private bool _loginPageDisplayed = false;  
    public DashboardPage(ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService)); 
        _validator = validator;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetStatisticsAsync();
    }    

    private async Task<IEnumerable<Statistic>> GetStatisticsAsync()
    {
        try
        {  
            var (statistics, errorMessage) = await _apiService.GetStatistics();

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return Enumerable.Empty<Statistic>();
            }
            if (errorMessage == "NotFound")
            {
                await DisplayAlert("Alert", "No statistics found.", "OK");
                return Enumerable.Empty<Statistic>();
            }
            if (statistics is null)
            {
                await DisplayAlert("Erro", errorMessage ?? "No statistics found.", "OK");
                return Enumerable.Empty<Statistic>();
            }

            dataGridStatistics.ItemsSource = statistics;

            return statistics;
        }
        catch (Exception)
        {
            await DisplayAlert("Erro", "Ocorreu um erro ao obter os pedidos. Tente novamente mais tarde.", "OK");
            return Enumerable.Empty<Statistic>();
        }       
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }

}