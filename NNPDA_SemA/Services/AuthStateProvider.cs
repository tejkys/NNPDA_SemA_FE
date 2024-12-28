using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.Authorization;
using NNPDA_SemA.Entities;
using System.Security.Claims;
using System.Text.Json;
using Blazored.LocalStorage;
using static System.Net.WebRequestMethods;

namespace NNPDA_SemA.Services
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
       private readonly ILocalStorageService _localStorageService;
        private readonly HttpClient _http;

        public AuthStateProvider(ILocalStorageService localStorageService, HttpClient http)
        {
            _localStorageService = localStorageService;
            _http = http;

        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var jwtToken = await _localStorageService.GetItemAsync<string>("jwt-access-token");

            if (string.IsNullOrEmpty(jwtToken))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(jwtToken), "jwtAuth")));
        }

        private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];

            var jsonBytes = ParseBase64WithoutPadding(payload);

            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));
            return claims;
        }
        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }

        public void NotifyAuthState()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public async Task Logout()
        {
            await _localStorageService.RemoveItemAsync("jwt-access-token");
            _http.DefaultRequestHeaders.Authorization = null;
            NotifyAuthState();
        }

        public async Task<User> GetUser()
        {
            var user = new User();
            var authenticationState = await GetAuthenticationStateAsync();

            var displayNameClaim = authenticationState.User.Claims.FirstOrDefault(c => c.Type == "name");
            user.Username = displayNameClaim?.Value ?? string.Empty;
            return user;
        }
    }
}

