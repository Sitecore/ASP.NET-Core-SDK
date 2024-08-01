namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Logging;

public class IntegrationTestLoggerProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName) => new InMemoryLogger();

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}