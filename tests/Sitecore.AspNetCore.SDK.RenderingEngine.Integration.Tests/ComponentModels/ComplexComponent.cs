using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Fields;
using Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Attributes;
using Sitecore.AspNetCore.SDK.TestData.Models;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.ComponentModels;

public class ComplexComponent
{
    public TextField? Header { get; set; } // maps to a field for the component by default

    public RichTextField? Content { get; set; } // maps to a field for the component by default

    public CustomFieldType? CustomField { get; set; }

    [SitecoreComponentProperty(Name = "Name")]
    public string? ComponentName { get; set; }

    [SitecoreRouteProperty(Name = "Name")]
    public string? RouteName { get; set; }

    [SitecoreContextProperty]
    public bool IsEditing { get; set; }

    [SitecoreRouteField(Name = "pageTitle")]
    public TextField? PageTitle { get; set; }

    [SitecoreComponentFields]
    public CustomClass? NestedComponent { get; set; }

    [SitecoreRouteFields]
    public RouteFields? RouteFields { get; set; }

    [SitecoreComponentParameter]
    public string? ParamName { get; set; }
}