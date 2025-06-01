using System.ComponentModel;
using System.Text.Json;
using Jennifer.Account.Application.Auth.Contracts;
using Jennifer.SharedKernel;
using ModelContextProtocol.Server;

namespace Jennifer.McpServer;

[McpServerToolType]
public sealed class EchoTool
{
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
        var client = new HttpClient();
        client.BaseAddress = new Uri("https://localhost:7288");
        client.DefaultRequestHeaders.Accept.Clear();
        var body = new
        {
            email = email,
            password = password
        };
        var res = await client.PostAsJsonAsync("/api/v1/auth/signin", body);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<Result<TokenResponse>>();
    }
}