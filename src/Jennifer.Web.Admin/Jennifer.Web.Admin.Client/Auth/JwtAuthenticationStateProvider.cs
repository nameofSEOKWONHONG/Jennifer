using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace Jennifer.Web.Admin.Auth
{
    public class JwtAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ISessionStorageService _storage;
        private readonly HttpClient _http;
        private const string TokenKey = "authToken";
        private const string RefreshToken = "refreshToken";

        public JwtAuthenticationStateProvider(ISessionStorageService storage, HttpClient http)
        {
            _storage = storage;
            _http = http;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var storedToken = await _storage.GetItemAsStringAsync(TokenKey);

                if (string.IsNullOrWhiteSpace(storedToken))
                    return GetAnonymous();

                var identity = ParseClaimsFromJwt(storedToken);
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", storedToken);
                return new AuthenticationState(new ClaimsPrincipal(identity));
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("JavaScript interop calls cannot be issued"))
            {
                // Prerendering 중: 익명 사용자로 처리
                return GetAnonymous();
            }
        }

        private AuthenticationState GetAnonymous()
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        public async Task MarkUserAsAuthenticated(string token, string refreshToken)
        {
            await _storage.SetItemAsStringAsync(TokenKey, token);
            await _storage.SetItemAsStringAsync(RefreshToken, refreshToken);
            
            var identity = ParseClaimsFromJwt(token);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity))));
        }

        public async Task MarkUserAsLoggedOut()
        {
            await _storage.RemoveItemAsync(TokenKey);
            await _storage.RemoveItemAsync(RefreshToken);
            
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()))));
        }

        private ClaimsIdentity ParseClaimsFromJwt(string jwt)
        {
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);
            var claims = token.Claims;
            return new ClaimsIdentity(claims, "jwt");
        }
    }
}
