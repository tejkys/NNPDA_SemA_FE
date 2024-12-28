using Blazored.LocalStorage;
using System.Net.Http.Headers;


namespace NNPDA_SemA.Services;

public class AuthHttpClient : HttpClient
{
    public AuthHttpClient(ISyncLocalStorageService localStorageService, IConfiguration configuration)
    {
        BaseAddress = new Uri(configuration["BaseAddress"]);
        var jwtToken = localStorageService.GetItem<string>("jwt-access-token");
        if (!string.IsNullOrEmpty(jwtToken))
        {
            DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        }
    }

}