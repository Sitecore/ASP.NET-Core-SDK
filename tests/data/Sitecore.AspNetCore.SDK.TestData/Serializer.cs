using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Serialization;

namespace Sitecore.AspNetCore.SDK.TestData;

[ExcludeFromCodeCoverage]
public static class Serializer
{
    public static string Serialize(object obj)
    {
        return Encoding.UTF8.GetString(JsonSerializer.SerializeToUtf8Bytes(obj, GetOptions()));
    }

    public static T? Deserialize<T>(string data)
    {
        return JsonSerializer.Deserialize<T>(data, GetOptions());
    }

    public static JsonSerializerOptions GetOptions()
    {
        return JsonLayoutServiceSerializer.GetDefaultSerializerOptions();
    }
}