namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;

/// <summary>
/// Exposes a Value property on an <see cref="IField"/>.
/// </summary>
/// <typeparam name="TValue">The value type.</typeparam>
public interface IValueField<TValue> : IField
{
    /// <summary>
    /// Gets or sets the value of the <see cref="IField"/>.
    /// </summary>
    TValue Value { get; set; }
}