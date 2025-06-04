namespace Jennifer.Account.E2ETests;

public static class HttpClientFactory
{
    public static HttpClient Create()
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        return new HttpClient(handler)
        {
            BaseAddress = new Uri("https://localhost:7288")
        };
    }
}