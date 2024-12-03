using LeagueMAUI.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LeagueMAUI.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiService> _logger;
        JsonSerializerOptions _serializerOptions;

        public ApiService(HttpClient httpClient, ILogger<ApiService> logger)
        {
            _logger = logger;
            _httpClient = httpClient;
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
        }

        public async Task<ApiResponse<bool>> Register(string firstName, string lastName, string email,
                                                      string phoneNumber, string password, string confirm)
        {
            try
            {
                var register = new Register()
                {
                    FirstName = firstName,
                    LastName = lastName,    
                    Email = email,
                    PhoneNumber = phoneNumber,
                    Password = password,
                    Confirm = confirm
                };

                var json = JsonSerializer.Serialize(register, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await PostRequest("api/Account/Register", content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Error sending HTTP request: {response.StatusCode}");
                    return new ApiResponse<bool>
                    {
                        ErrorMessage = $"Error sending HTTP request: {response.StatusCode}"
                    };
                }

                //var successMessage = $"User registered successfully, confirm e-mail, the instruction has been sent to: {email}";
                //_logger.LogInformation(successMessage);

                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"User registration error: {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> Login(string email, string password)
        {
            try
            {
                var login = new Login()
                {
                    Email = email,
                    Password = password
                };

                var json = JsonSerializer.Serialize(login, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");



                var response = await PostRequest("api/Account/Login", content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Error sending HTTP request : {response.StatusCode}");
                    return new ApiResponse<bool>
                    {
                        ErrorMessage = $"Error sending HTTP request : {response.StatusCode}"
                    };
                }

                var jsonResult = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Token>(jsonResult, _serializerOptions);

                Preferences.Set("accesstoken", result!.AccessToken);              
                Preferences.Set("userid", result.UserId!);
                Preferences.Set("username", result.UserName);
                Preferences.Set("name", result.Name);

                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Login error : {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }
        }

        private async Task<HttpResponseMessage> PostRequest(string uri, HttpContent content)
        {
            var enderecoUrl = AppConfig.BaseUrl + uri;
            try
            {
                var result = await _httpClient.PostAsync(enderecoUrl, content);
                return result;
            }
            catch (Exception ex)
            {
                // Log o erro ou trate conforme necessário
                _logger.LogError($"Erro ao enviar requisição POST para {uri}: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }
    }
}
