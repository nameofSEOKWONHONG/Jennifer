using System.Text.Json.Serialization;

namespace Jennifer.External.OAuth.Contracts;

public sealed class LineUserResult
{
    [JsonPropertyName("userId")]
    public string UserId { get; set; }

    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; }

    [JsonPropertyName("pictureUrl")]
    public string PictureUrl { get; set; }
}

public sealed class LineIdTokenClaims
{
    [JsonPropertyName("sub")]
    public string UserId { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("picture")]
    public string PictureUrl { get; set; }
}