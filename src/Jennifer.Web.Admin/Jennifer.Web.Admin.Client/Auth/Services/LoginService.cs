using Jennifer.SharedKernel;
using Jennifer.SharedKernel.Account.Auth;
using Jennifer.Web.Admin.Auth;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Jennifer.Web.Admin.Client.Auth.Services
{
    public class LoginService : ILoginService
    {
        private readonly HttpClient _http;
        private readonly JwtAuthenticationStateProvider _authProvider;

        public LoginService(HttpClient http, JwtAuthenticationStateProvider authProvider)
        {
            _http = http;
            _authProvider = authProvider;
        }

        public async Task<Result<TokenResponse>> LoginAsync(string email, string password)
        {
            var response = await _http.PostAsJsonAsync("/api/v1/auth/signin", new { email, password });
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<Result<TokenResponse>>();
                if (!result.IsSuccess)
                    return result;

                await _authProvider.MarkUserAsAuthenticated(result.Data.AccessToken, result.Data.RefreshToken);
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.Data.AccessToken);
                return result;
            }
            return await Result<TokenResponse>.FailureAsync("Login failed. Please check your credentials.");
        }
    }

}
