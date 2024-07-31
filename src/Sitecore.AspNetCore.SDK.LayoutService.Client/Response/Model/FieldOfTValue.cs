namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;

/// <summary>
/// Represents an arbitrary field in a Sitecore layout service response that contains a value.
/// </summary>
/// <typeparam name="TValue">The value type.</typeparam>
[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Generic Type.")]
public class Field<TValue>
    : Field, IValueField<TValue>
{
    /// <inheritdoc />
    public required TValue Value { get; set; }
}