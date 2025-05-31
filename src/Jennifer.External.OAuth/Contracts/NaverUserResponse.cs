using System.Text.Json.Serialization;

namespace Jennifer.External.OAuth.Contracts;

public sealed class NaverUserResponse
{
    [JsonPropertyName("resultcode")]
    public string ResultCode { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("response")]
    public NaverUserInfo Response { get; set; }
}

public sealed class NaverUserInfo
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("nickname")]
    public string Nickname { get; set; }
}