using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace Jennifer.Wasm.Admin.Auth
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

                // 인증된 사용자
                var user = new ClaimsPrincipal(identity);
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", storedToken);
                return new AuthenticationState(user);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("JavaScript interop"))
            {
                return GetAnonymous(); // prerender 등 JS 접근 불가 상황
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
            var user = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        public async Task MarkUserAsLoggedOut()
        {
            await _storage.RemoveItemAsync(TokenKey);
            await _storage.RemoveItemAsync(RefreshToken);
            NotifyAuthenticationStateChanged(Task.FromResult(GetAnonymous()));
        }

        private ClaimsIdentity ParseClaimsFromJwt(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            return new ClaimsIdentity(jwtToken.Claims, "auth");
        }
    }
}
