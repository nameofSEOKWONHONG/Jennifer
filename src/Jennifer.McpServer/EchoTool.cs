using System.ComponentModel;
using System.Net.Http.Headers;
using eXtensionSharp;
using Jennifer.Account.Application.Auth.Contracts;
using Jennifer.SharedKernel;
using ModelContextProtocol.Server;

namespace Jennifer.McpServer;

[McpServerToolType]
public sealed class EchoTool
{
    public static TokenResponse Token { get; set; }
    
    public EchoTool()
    {
    }
    
    [McpServerTool, Description("Says Hello to a user")]
    public static string Echo(string username)
    {
        return "Hello " + username;
    }
    
    [McpServerTool, Description("SignIn")]
    public static async Task<Result<TokenResponse>> SignIn(string email, string password)
    {
        using var client = new HttpClient();
        client.BaseAddress = new Uri("https://localhost:7288");
        client.DefaultRequestHeaders.Accept.Clear();
        var body = new
        {
            email = email,
            password = password
        };
        var res = await client.PostAsJsonAsync("/api/v1/auth/signin", body);
        res.EnsureSuccessStatusCode();
        var result = await res.Content.ReadFromJsonAsync<Result<TokenResponse>>();
        Token = result.Data;
        return result;
    }

    [McpServerTool, Description("Jennifer Users")]
    public static async Task<PaginatedResult<UserDto>> Users()
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        };

        using var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://localhost:7288")
        };
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token.AccessToken);

        var res = await client.GetAsync("/api/v1/user?PageNo=1&PageSize=10");
        res.EnsureSuccessStatusCode();
        
        var result = await res.Content.ReadAsStringAsync();
        var rtn = result.xDeserialize<PaginatedResult<UserDto>>();
        return rtn;
    }
}