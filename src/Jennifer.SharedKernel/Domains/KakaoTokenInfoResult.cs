using System.Text.Json.Serialization;

namespace Jennifer.SharedKernel.Domains;

public class KakaoSignResult : IExternalSignResult
{
    public string ProviderId { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
}

public class KakaoTokenInfoResult
{
    [JsonPropertyName("id")]
    public long Id { get; set; }
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
    [JsonPropertyName("app_id")]
    public int AppId { get; set; }
}

public class KakaoUserResult
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("connected_at")]
    public DateTime ConnectedAt { get; set; }

    [JsonPropertyName("properties")]
    public KakaoProperties Properties { get; set; }

    [JsonPropertyName("kakao_account")]
    public KakaoAccount KakaoAccount { get; set; }
}

public class KakaoProperties
{
    [JsonPropertyName("nickname")]
    public string Nickname { get; set; }

    [JsonPropertyName("profile_image")]
    public string ProfileImage { get; set; }

    [JsonPropertyName("thumbnail_image")]
    public string ThumbnailImage { get; set; }
}

public class KakaoAccount
{
    [JsonPropertyName("profile_needs_agreement")]
    public bool ProfileNeedsAgreement { get; set; }

    [JsonPropertyName("profile")]
    public KakaoProfile Profile { get; set; }

    [JsonPropertyName("has_email")]
    public bool HasEmail { get; set; }

    [JsonPropertyName("email_needs_agreement")]
    public bool EmailNeedsAgreement { get; set; }

    [JsonPropertyName("is_email_valid")]
    public bool IsEmailValid { get; set; }

    [JsonPropertyName("is_email_verified")]
    public bool IsEmailVerified { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }
}

public class KakaoProfile
{
    [JsonPropertyName("nickname")]
    public string Nickname { get; set; }

    [JsonPropertyName("thumbnail_image_url")]
    public string ThumbnailImageUrl { get; set; }

    [JsonPropertyName("profile_image_url")]
    public string ProfileImageUrl { get; set; }
}