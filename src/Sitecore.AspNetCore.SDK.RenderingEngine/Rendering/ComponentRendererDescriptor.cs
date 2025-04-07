namespace Sitecore.AspNetCore.SDK.RenderingEngine.Rendering;

/// <summary>
/// Service descriptor for a <see cref="IComponentRenderer"/>.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ComponentRendererDescriptor"/> class.
/// </remarks>
/// <param name="match">The predicate to use when retrieving a <see cref="IComponentRenderer"/>.</param>
/// <param name="factory">The factory method to create a new instance of the <see cref="IComponentRenderer"/>.</param>
/// <param name="componentName">The name of the component being added.</param>
public class ComponentRendererDescriptor(
    Predicate<string> match,
    Func<IServiceProvider, IComponentRenderer> factory,
    string componentName = "")
{
    private readonly Func<IServiceProvider, IComponentRenderer> _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    private readonly object _lock = new();

    private IComponentRenderer? _instance;

    /// <summary>
    /// Gets a predicate used for matching Sitecore layout components.
    /// </summary>
    public Predicate<string> Match { get; } = match ?? throw new ArgumentNullException(nameof(match));

    /// <summary>
    /// Gets the name of the component.
    /// </summary>
    public string ComponentName { get; } = componentName;

    /// <summary>
    /// Gets an instance of an <see cref="IComponentRenderer"/>, creating one if it has not yet been instantiated.
    /// </summary>
    /// <param name="services">The <see cref="IServiceProvider"/>.</param>
    /// <returns>An instance of an <see cref="IComponentRenderer"/>.</returns>
    public IComponentRenderer GetOrCreate(IServiceProvider services)
    {
        ArgumentNullException.ThrowIfNull(services);

        if (_instance == null)
        {
            lock (_lock)
            {
                _instance = _factory(services);
            }
        }

        return _instance;
    }
}