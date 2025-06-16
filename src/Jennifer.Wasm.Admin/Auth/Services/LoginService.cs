using Jennifer.SharedKernel;
using Jennifer.SharedKernel.Account.Auth;
using Jennifer.Wasm.Admin.Auth;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Jennifer.Wasm.Admin.Auth.Services
{
    public class LoginService : ILoginService
    {
        private readonly HttpClient _http;        

        public LoginService(HttpClient http)
        {
            _http = http;
        }

        public async Task<Result<TokenResponse>> LoginAsync(string email, string password)
        {
            var response = await _http.PostAsJsonAsync("/api/v1/auth/signin", new { email = email, password = password });
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<Result<TokenResponse>>();
                if (!result.IsSuccess)
                    return result;
                
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.Data.AccessToken);
                return result;
            }
            return await Result<TokenResponse>.FailureAsync("Login failed. Please check your credentials.");
        }
    }

}
