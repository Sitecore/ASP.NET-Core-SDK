using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;

namespace Sitecore.AspNetCore.SDK.Pages.Request.Handlers.GraphQL;

public partial class GraphQLEditingServiceHandler
{
    // Define the work item class outside of the method to avoid nested class issues
    private class PlaceholderWorkItem
    {
        public string Name { get; }
        public string Id { get; }
        public Placeholder Features { get; }
        public Placeholder Output { get; }
        public Component ParentComponent { get; }
        public string PlaceholderKey { get; }

        public PlaceholderWorkItem(
            string name,
            string id,
            Placeholder features,
            Placeholder output,
            Component parentComponent = null,
            string placeholderKey = null)
        {
            Name = name;
            Id = id;
            Features = features;
            Output = output;
            ParentComponent = parentComponent;
            PlaceholderKey = placeholderKey;
        }
    }
}