namespace Jennifer.External.OAuth.Abstracts;

public interface IExternalOAuthResult
{
    bool IsSuccess { get; }
    string ExternalId { get; }
    string Email { get; }
    string Name { get; }
    string Reason { get; }
}