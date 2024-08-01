using Sitecore.AspNetCore.SDK.LayoutService.Client.Request;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Interfaces;

/// <summary>
/// Contract for mapping <see cref="SitecoreLayoutRequest"/> entries to an object of the given type.
/// </summary>
/// <typeparam name="T">The type the request is mapped to.</typeparam>
public interface IMapRequest<T>
    where T : class
{
    /// <summary>
    /// Gets the list of mappings from a <see cref="SitecoreLayoutRequest"/> to T.
    /// </summary>
    List<Action<SitecoreLayoutRequest, T>> RequestMap { get; init; }
}