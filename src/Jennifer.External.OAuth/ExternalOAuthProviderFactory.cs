using Jennifer.External.OAuth.Abstracts;

namespace Jennifer.External.OAuth;

public class ExternalOAuthProviderFactory: IExternalOAuthProviderFactory
{
    private readonly Dictionary<string, IExternalOAuthProvider> _handlers;

    public ExternalOAuthProviderFactory(IEnumerable<IExternalOAuthProvider> handlers)
    {
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        _handlers = new Dictionary<string, IExternalOAuthProvider>(StringComparer.OrdinalIgnoreCase);

        foreach (var handler in handlers)
        {
            if (!seen.Add(handler.Provider))
            {
                throw new InvalidOperationException(
                    $"Duplicate Provider registration detected: '{handler.Provider}'");
            }

            _handlers[handler.Provider] = handler;
        }
    }

    public IExternalOAuthProvider Resolve(string providerName)
    {
        ArgumentNullException.ThrowIfNull(providerName);

        _handlers.TryGetValue(providerName.ToLower(), out var handler);
        return handler;
    }

}