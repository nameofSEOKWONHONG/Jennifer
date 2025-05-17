using Jennifer.External.OAuth.Abstracts;

namespace Jennifer.External.OAuth;

public class ExternalOAuthHandlerFactory: IExternalOAuthHandlerFactory
{
    private readonly Dictionary<string, IExternalOAuthHandler> _handlers;

    public ExternalOAuthHandlerFactory(IEnumerable<IExternalOAuthHandler> handlers)
    {
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        _handlers = new Dictionary<string, IExternalOAuthHandler>(StringComparer.OrdinalIgnoreCase);

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

    public IExternalOAuthHandler Resolve(string providerName)
    {
        if (string.IsNullOrWhiteSpace(providerName)) return null;

        _handlers.TryGetValue(providerName.ToLower(), out var handler);
        return handler;
    }

}