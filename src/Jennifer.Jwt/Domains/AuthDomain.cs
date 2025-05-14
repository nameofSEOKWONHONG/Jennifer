namespace Jennifer.Jwt.Domains;

public record TokenResponse(string AccessToken, string RefreshToken);

public record SignInRequest(string Email, string Password);