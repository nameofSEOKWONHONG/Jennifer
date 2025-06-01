using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using Jennifer.Account.Application.Auth.Contracts;
using Jennifer.SharedKernel;
using ModelContextProtocol.Server;

namespace Jennifer.Mcp;

[McpServerToolType]
public class TimeTool
{
    // 👇 Mark a method as an MCP tools
    [McpServerTool, Description("Get the current time for a city")]
    public static string GetCurrentTime(string city) => 
        $"It is {DateTime.Now.Hour:00}:{DateTime.Now.Minute:00} in {city}.";

    [McpServerTool, Description("User sign in")]
    public static async Task<Result<TokenResponse>> SignIn(string email, string password)
    {
        var client = new HttpClient();
        client.BaseAddress = new Uri("https://localhost:7288");
        var body = new { email = email, password = password };
        var res  = await client.PostAsync("/api/auth/signin", new StringContent(JsonSerializer.Serialize(body)));
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<Result<TokenResponse>>();
    }
}