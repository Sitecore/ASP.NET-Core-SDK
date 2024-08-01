namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Logging;

/// <summary>
/// An in memory logger for use with Integration Tests.
/// </summary>
public class InMemoryLogger : ILogger, IDisposable
{
    private readonly Disposable _disposable = new();

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        InMemoryLog.Log.Add(formatter(state, exception));
    }

    public bool IsEnabled(LogLevel logLevel) => true;

    public IDisposable BeginScope<TState>(TState state)
        where TState : notnull => _disposable;

    public void Dispose()
    {
        _disposable.Dispose();
        GC.SuppressFinalize(this);
    }

    private class Disposable : IDisposable
    {
        public void Dispose()
        {
        }
    }
}