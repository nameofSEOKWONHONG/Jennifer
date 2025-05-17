using Jennifer.External.OAuth.Abstracts;

namespace Jennifer.External.OAuth.Contracts;

public class ExternalOAuthResult : IExternalOAuthResult
{
    public bool IsSuccess { get; private set; }
    public string ExternalId { get; private set; }
    public string Email { get; private set; }
    public string Name { get; private set; }
    public string Reason { get; private set; }

    public static ExternalOAuthResult Success(string id, string email, string name)
        => new() { IsSuccess = true, ExternalId = id, Email = email, Name = name };

    public static ExternalOAuthResult Fail(string reason)
        => new() { IsSuccess = false, Reason = reason }; // 실패 이유를 Email에 임시 보관해도 되고 별도 필드로 확장 가능
}