namespace Jennifer.External.OAuth.Contracts;

internal class ExternalOAuthOption
{
    private static Lazy<ExternalOAuthOption> lazy = new(() => new ExternalOAuthOption());
    public static ExternalOAuthOption Instance => lazy.Value;
    private ExternalOAuthOption() { }
    
    public Dictionary<string, string> Options { get; set; } = new();
}