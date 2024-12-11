using LeagueMAUI.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
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

        public async Task<ApiResponse<bool>> RecoverPassword(string email)
        {
            try
            {
                var recover = new RecoverPassword()
                {
                    Email = email                    
                };

                var json = JsonSerializer.Serialize(recover, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await PostRequest("api/Account/RecoverPassword", content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Error sending HTTP request : {response.StatusCode}");
                    return new ApiResponse<bool>
                    {
                        ErrorMessage = $"Error sending HTTP request : {response.StatusCode}"
                    };
                }

                //var jsonResult = await response.Content.ReadAsStringAsync();
                //var result = JsonSerializer.Deserialize<Token>(jsonResult, _serializerOptions);

                //Preferences.Set("accesstoken", result!.AccessToken);
                //Preferences.Set("userid", result.UserId!);
                //Preferences.Set("username", result.UserName);
                //Preferences.Set("name", result.Name);

                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Recover Password error : {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> UploadImageUser(byte[] imageArray)
        {
            try
            {
                var content = new MultipartFormDataContent();
                content.Add(new ByteArrayContent(imageArray), "file", "image.jpg");
                var token = Preferences.Get("accesstoken", string.Empty);
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await PostRequest("api/Account/UploadPhoto", content);

                if (!response.IsSuccessStatusCode)
                {
                    string errorMessage = response.StatusCode == HttpStatusCode.Unauthorized
                      ? "Unauthorized"
                      : $"Erro ao enviar requisição HTTP: {response.StatusCode}";

                    _logger.LogError($"Error sending HTTP request: {response.StatusCode}");
                    return new ApiResponse<bool> { ErrorMessage = errorMessage };
                }
                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error uploading user image: {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> UploadImagePlayer(byte[] imageArray, int playerId)
        {
            try
            {
                var content = new MultipartFormDataContent();
                content.Add(new ByteArrayContent(imageArray), "file", "image.jpg");
                content.Add(new StringContent(playerId.ToString()), "playerId");
                //var content = new MultipartFormDataContent
                //{
                //    { new ByteArrayContent(imageArray), "file", "image.jpg" },
                //    { new StringContent(playerId.ToString()), "playerId" }
                //};
                //var content = new MultipartFormDataContent();
                //content.Add(new ByteArrayContent(imageArray), "file", "image.jpg");
                var token = Preferences.Get("accesstoken", string.Empty);
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await PostRequest("api/Clubs/UploadPhotoPlayer", content);

                if (!response.IsSuccessStatusCode)
                {
                    string errorMessage = response.StatusCode == HttpStatusCode.Unauthorized
                      ? "Unauthorized"
                      : $"Erro ao enviar requisição HTTP: {response.StatusCode}";

                    _logger.LogError($"Error sending HTTP request: {response.StatusCode}");
                    return new ApiResponse<bool> { ErrorMessage = errorMessage };
                }
                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error uploading player image: {ex.Message}");
                return new ApiResponse<bool> { ErrorMessage = ex.Message };
            }
        }

        public async Task<ApiResponse<bool>> CreatePlayerAsync(PlayerIn player)
        {
            try
            {
                var json = JsonSerializer.Serialize(player, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await PostRequest("api/Clubs/CreatePlayer", content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Error sending HTTP request: {response.StatusCode}");
                    return new ApiResponse<bool>
                    {
                        ErrorMessage = $"Error sending HTTP request: {response.StatusCode}"
                    };
                }

                return new ApiResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Player registration error: {ex.Message}");
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
                // Log the error or treat as necessary
                _logger.LogError($"Error sending POST request to {uri}: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }

        public async Task<(List<Statistic>? Statistics, string? ErrorMessage)> GetStatistics()
        {
            string endpoint = $"api/Dashboard/GetStatistics";

            return await GetAsync<List<Statistic>>(endpoint);
        }

        public async Task<(List<Club>? Clubs, string? ErrorMessage)> GetClubs()
        {
            string endpoint = $"api/Clubs/GetClubs";

            return await GetAsync<List<Club>>(endpoint);
        }

        public async Task<(List<Player>? Players, string? ErrorMessage)> GetPlayers(int clubId)
        {
            string endpoint = $"api/Clubs/GetPlayersByClub/{clubId}";

            return await GetAsync<List<Player>>(endpoint);
        }

        public async Task<(List<Round>? Rounds, string? ErrorMessage)> GetRounds()
        {
            string endpoint = $"api/Matches/GetRounds";

            return await GetAsync<List<Round>>(endpoint);
        }

        public async Task<(List<Match>? Matches, string? ErrorMessage)> GetMatches(int roundId)
        {
            string endpoint = $"api/Matches/GetMatchesByRound/{roundId}";

            return await GetAsync<List<Match>>(endpoint);
        }

        public async Task<(ImageProfile? ImageProfile, string? ErrorMessage)> GetImageUserProfile(string email)
        {
            string endpoint = $"api/Account/GetUserImage/{email}";
            return await GetAsync<ImageProfile>(endpoint);
        }

        public async Task<(UserInfo? UserInfo, string? ErrorMessage)> GetUserInfo(string userId)
        {
            return await GetAsync<UserInfo>($"api/Account/GetUserInfo/{userId}");
        }

        public async Task<(Role? Role, string? ErrorMessage)> GetUserRole(string email)
        {            
            return await GetAsync<Role>($"api/Account/GetUserRole/{email}");
        }

        public async Task<(List<Position>? Positions, string? ErrorMessage)> GetPositions()
        {
            string endpoint = $"api/Clubs/GetPlayersPositions";

            return await GetAsync<List<Position>>(endpoint);
        }

        private async Task<(T? Data, string? ErrorMessage)> GetAsync<T>(string endpoint)
        {
            try
            {
                AddAuthorizationHeader();

                var response = await _httpClient.GetAsync(AppConfig.BaseUrl + endpoint);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<T>(responseString, _serializerOptions);
                    return (data ?? Activator.CreateInstance<T>(), null);
                }
                else
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        string errorMessage = "Unauthorized";
                        _logger.LogWarning(errorMessage);
                        return (default, errorMessage);
                    }

                    string generalErrorMessage = $"Request error: {response.ReasonPhrase}";
                    _logger.LogError(generalErrorMessage);

                    return (default, generalErrorMessage);
                }
            }
            catch (HttpRequestException ex)
            {
                string errorMessage = $"HTTP request error: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return (default, errorMessage);
            }
            catch (JsonException ex)
            {
                string errorMessage = $"JSON deserialisation error: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return (default, errorMessage);
            }
            catch (Exception ex)
            {
                string errorMessage = $"Unexpected error: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return (default, errorMessage);
            }
        }

        public async Task<(bool Data, string? ErrorMessage)> UpdatePlayerAsync(int playerId, PlayerIn player)
        {
            try
            {
                var json = JsonSerializer.Serialize(player, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await PutRequest($"api/Clubs/UpdatePlayer?playerId={playerId}", content);

                //var content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
                //var response = await PutRequest($"api/Clubs/UpdatePlayer?playerId={playerId}", content);
                if (response.IsSuccessStatusCode)
                {
                    return (true, null);
                }
                else
                {
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        string errorMessage = "Unauthorized";
                        _logger.LogWarning(errorMessage);
                        return (false, errorMessage);
                    }
                    string generalErrorMessage = $"Erro na requisição: {response.ReasonPhrase}";
                    _logger.LogError(generalErrorMessage);
                    return (false, generalErrorMessage);
                }
            }
            catch (HttpRequestException ex)
            {
                string errorMessage = $"Erro de requisição HTTP: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return (false, errorMessage);
            }
            catch (Exception ex)
            {
                string errorMessage = $"Erro inesperado: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return (false, errorMessage);
            }
        }

        private async Task<HttpResponseMessage> PutRequest(string uri, HttpContent content)
        {
            var enderecoUrl = AppConfig.BaseUrl + uri;
            try
            {
                AddAuthorizationHeader();
                var result = await _httpClient.PutAsync(enderecoUrl, content);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending PUT request to {uri}: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }

        public async Task<(bool Data, string? ErrorMessage)> DeletePlayerAsync(int playerId)
        {
            try
            {
                var response = await DeleteRequest($"api/Clubs/DeletePlayer?playerId={playerId}");
                if (response.IsSuccessStatusCode)
                {
                    return (true, null);
                }
                else
                {
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        string errorMessage = "Unauthorized";
                        _logger.LogWarning(errorMessage);
                        return (false, errorMessage);
                    }
                    string generalErrorMessage = $"Erro na requisição: {response.ReasonPhrase}";
                    _logger.LogError(generalErrorMessage);
                    return (false, generalErrorMessage);
                }
            }
            catch (HttpRequestException ex)
            {
                string errorMessage = $"Erro de requisição HTTP: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return (false, errorMessage);
            }
            catch (Exception ex)
            {
                string errorMessage = $"Erro inesperado: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return (false, errorMessage);
            }
        }

        private async Task<HttpResponseMessage> DeleteRequest(string uri)
        {
            var enderecoUrl = AppConfig.BaseUrl + uri;
            try
            {
                var result = await _httpClient.DeleteAsync(enderecoUrl);
                return result;
            }
            catch (Exception ex)
            {
                // Log the error or handle as needed
                _logger.LogError($"Error sending DELETE request to {uri}: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }

        private void AddAuthorizationHeader()
        {
            var token = Preferences.Get("accesstoken", string.Empty);
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
    }
}
