using System.Diagnostics.CodeAnalysis;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;

namespace Sitecore.AspNetCore.SDK.TestData.Models;

public class CustomFieldType : Field<string>
{
    [SetsRequiredMembers]
    public CustomFieldType()
    {
        Value = string.Empty;
    }

    [SetsRequiredMembers]
    public CustomFieldType(string value1, string value2)
    {
        Value = value1 + value2;
    }
}