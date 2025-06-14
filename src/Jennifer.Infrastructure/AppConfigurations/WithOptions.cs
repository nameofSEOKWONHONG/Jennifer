namespace Jennifer.Infrastructure.AppConfigurations;

public class WithOptions
{
    private static Lazy<WithOptions> _instance = new Lazy<WithOptions>(() => new WithOptions());
    public static WithOptions Instance => _instance.Value;

    /// <summary>
    /// Specifies whether the application should enable distributed caching functionality
    /// using Jennifer's integration with caching services such as Redis.
    /// </summary>
    /// <remarks>
    /// This property determines if distributed caching is configured and used within
    /// the application. When set to true, the application will leverage distributed caching
    /// mechanisms as part of Jennifer's infrastructure.
    /// </remarks>
    public bool WithJenniferDistributeCache { get; set; }

    /// <summary>
    /// Specifies whether the application should enable IP rate limiting functionality
    /// using Jennifer's integration with distributed rate limiting strategies.
    /// </summary>
    /// <remarks>
    /// This property determines if IP rate limiting is configured and applied within
    /// the application. When set to true, the application will enforce request rate limits
    /// for client IPs as part of Jennifer's infrastructure.
    /// </remarks>
    public bool WithJenniferIpRateLimit { get; set; }

    /// <summary>
    /// Indicates whether the application should enable SignalR functionality
    /// as part of Jennifer's infrastructure for real-time communication.
    /// </summary>
    /// <remarks>
    /// When set to true, SignalR services are configured and optionally integrate
    /// with a Redis backplane for distributed real-time messaging capabilities.
    /// This property reflects whether SignalR is enabled within the application.
    /// </remarks>
    public bool WithJenniferSignalr { get; set; }

    /// <summary>
    /// Indicates whether the Jennifer mail service functionality is enabled.
    /// </summary>
    /// <remarks>
    /// This property determines if the Jennifer mail service, which integrates with Kafka for email processing,
    /// is activated within the application. When set to true, the application will configure and utilize mail service
    /// components for handling email operations through Kafka messaging infrastructure.
    /// </remarks>
    public bool WithJenniferMailService { get; set; }

    /// <summary>
    /// Indicates whether the application should enable MongoDB functionality
    /// through Jennifer's integration with MongoDB services.
    /// </summary>
    /// <remarks>
    /// This property determines if MongoDB support is configured and utilized within
    /// the application. When set to true, Jennifer's infrastructure will leverage
    /// MongoDB for database operations and associated functionalities.
    /// </remarks>
    public bool WithJenniferMongodb { get; set; }

    /// <summary>
    /// Determines whether the application should enable the IP blocking functionality
    /// for restricting access based on specified conditions.
    /// </summary>
    /// <remarks>
    /// This property controls whether the IP block mechanism should be active in the application.
    /// If enabled, the application will use services such as caching and Redis to manage
    /// blocked IPs and enforce access restrictions.
    /// </remarks>
    public bool WorkIpBlock { get; set; } = true;
}