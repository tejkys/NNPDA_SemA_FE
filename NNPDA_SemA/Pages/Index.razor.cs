using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Text.RegularExpressions;
using NNPDA_SemA.Entities;
using Blazored.LocalStorage;
using NNPDA_SemA.Services;
using System.Net.Http;
using System.Net.Http.Json;
using System;

namespace NNPDA_SemA.Pages
{
    public partial class Index : ComponentBase
    {
        [Inject] public HttpClient _httpClient { get; set; } 
        [Inject] private ILocalStorageService _localStorageService { get; set; }
        [Inject] private AuthStateProvider _authStateProvider { get; set; }


        private string _errorMessage = string.Empty;
        private string _successMessage = string.Empty;
        private LoginRequest _loginRequest = new();
        private RegisterRequest _registerRequest = new();
        private RecoverPasswordRequest _recoverPasswordRequest = new();
        private PasswordRecoveryRequest _passwordRecoveryRequest = new();
        private bool _processing = false;
        private MudForm _form;
        private IEnumerable<string> PasswordStrength(string pw)
        {
            if (string.IsNullOrWhiteSpace(pw))
            {
                yield return "Heslo je požadováno!";
                yield break;
            }
            if (pw.Length < 8)
                yield return "Heslo musí být alespoň 8 znaků dlouhé";
            if (!Regex.IsMatch(pw, @"[a-z]"))
                yield return "Heslo musí obsahovat alespoň jeden malý znak";
            if (!Regex.IsMatch(pw, @"[0-9]"))
                yield return "Heslo musí obsahovat alespoň jedno číslo";
        }

        private string PasswordMatch(string arg)
        {
            if (_registerRequest.Password != arg)
                return "Hesla se neshodují";
            return null;
        }
        private async Task LogIn()
        {
            _processing = true;

            var response = await _httpClient.PostAsJsonAsync("/api/v1/auth/signin", _loginRequest);
            if (!response.IsSuccessStatusCode)
            {
                _errorMessage = "Nepodařilo se přihlásit";
                _processing = false;
                return;
            }
            var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
            await _localStorageService.SetItemAsync<string>("jwt-access-token", tokenResponse.Token);
            _authStateProvider.NotifyAuthState();
            _processing = false;
        }
        private async Task Register()
        {
            _processing = true;
            var response = await _httpClient.PostAsJsonAsync("/api/v1/auth/signup", _registerRequest);
            if (!response.IsSuccessStatusCode)
            {
                _errorMessage = "Nepodařilo se přihlásit";
                _processing = false;
                return;
            }
            _successMessage = "Účet vytvořen";

            var responseContent = await response.Content.ReadFromJsonAsync<TokenResponse>();
            await _localStorageService.SetItemAsync<string>("jwt-access-token", responseContent.Token);
            _authStateProvider.NotifyAuthState();

            _processing = false;
        }
        private async Task RequestToken()
        {
            _processing = true;
            var response = await _httpClient.PostAsJsonAsync("/api/v1/auth/getRecoveryPasswordToken", _recoverPasswordRequest);
            if (!response.IsSuccessStatusCode)
            {
                _errorMessage = "Nepodařilo se";
                _processing = false;
                return;
            }
            _successMessage = await response.Content.ReadAsStringAsync();
            _processing = false;
        }
        private async Task ChangePassword()
        {
            _processing = true;
            var response = await _httpClient.PostAsJsonAsync("/api/v1/auth/recoverPasswordByToken", _passwordRecoveryRequest);
            if (!response.IsSuccessStatusCode)
            {
                _errorMessage = "Nepodařilo se";
                _processing = false;
                return;
            }
            _successMessage = await response.Content.ReadAsStringAsync();
            _processing = false;
        }
    }
}
