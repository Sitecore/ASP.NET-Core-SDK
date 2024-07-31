using Sitecore.AspNetCore.SDK.ExperienceEditor.TagHelpers.Model;

// ReSharper disable StringLiteralTypo
namespace Sitecore.AspNetCore.SDK.ExperienceEditor.TagHelpers;

/// <inheritdoc />
internal class ChromeDataBuilder : IChromeDataBuilder
{
    /// <inheritdoc />
    public ChromeCommand MapButtonToCommand(EditButtonBase button, string? itemId, IDictionary<string, object?>? parameters)
    {
        if (button is DividerEditButton dividerEditButton)
        {
            return new ChromeCommand
            {
                Click = "chrome:dummy",
                Header = dividerEditButton.Header,
                Icon = dividerEditButton.Icon,
                IsDivider = true,
                Tooltip = null,
                Type = "separator"
            };
        }

        if (button is WebEditButton webEditButton && !string.IsNullOrWhiteSpace(webEditButton.Click))
        {
            return CommandBuilder(webEditButton, itemId, parameters);
        }

        FieldEditButton? fieldEditButton = button as FieldEditButton;
        string fieldsString = string.Join('|', fieldEditButton?.Fields ?? []);
        webEditButton = new WebEditButton
        {
            Click = $"webedit:fieldeditor(command={DefaultEditFrameButtonIds.Edit},fields={fieldsString})",
            Tooltip = button.Tooltip,
            Header = button.Header,
            Icon = button.Icon,
        };

        return CommandBuilder(webEditButton, itemId, parameters);
    }

    private static ChromeCommand CommandBuilder(WebEditButton button, string? itemId, IDictionary<string, object?>? frameParameters)
    {
        if (string.IsNullOrWhiteSpace(button.Click) ||
            button.Click.StartsWith("javascript:", StringComparison.OrdinalIgnoreCase) ||
            button.Click.StartsWith("chrome:", StringComparison.OrdinalIgnoreCase) ||
            string.IsNullOrWhiteSpace(itemId))
        {
            return new ChromeCommand
            {
                IsDivider = false,
                Type = button.Type,
                Header = button.Header ?? string.Empty,
                Icon = button.Icon ?? string.Empty,
                Tooltip = button.Tooltip ?? string.Empty,
                Click = button.Click ?? string.Empty,
            };
        }

        string? message = button.Click;
        Dictionary<string, string> parameters = [];

        // Extract any parameters already in the command
        int length = button.Click.IndexOf('(', StringComparison.OrdinalIgnoreCase);
        if (length >= 0)
        {
            int end = button.Click.IndexOf(')', StringComparison.OrdinalIgnoreCase);
            if (end < 0)
            {
                throw new ArgumentException("Message with arguments must end with ).");
            }

            parameters = button.Click[(length + 1)..end]
                .Split(',')
                .Select(x => x.Trim())
                .Aggregate(new Dictionary<string, string>(), (previous, current) =>
                {
                    string[] parts = current.Split('=');
                    previous[parts[0]] = parts.Length < 2
                        ? string.Empty
                        : parts[1];
                    return previous;
                });

            message = button.Click[..length];
        }

        parameters["id"] = itemId;

        if (button.Parameters != null && button.Parameters.Any())
        {
            foreach ((string key, object? value) in button.Parameters)
            {
                parameters[key] = value?.ToString() ?? string.Empty;
            }
        }

        if (frameParameters != null && frameParameters.Any())
        {
            foreach ((string key, object? value) in frameParameters)
            {
                parameters[key] = value?.ToString() ?? string.Empty;
            }
        }

        string parameterString = string.Join(", ", parameters.Select(x => $"{x.Key}={x.Value}"));
        string click = $"{message}({parameterString})";

        return new ChromeCommand
        {
            IsDivider = false,
            Type = button.Type,
            Header = button.Header ?? string.Empty,
            Icon = button.Icon ?? string.Empty,
            Tooltip = button.Tooltip ?? string.Empty,
            Click = $"javascript:Sitecore.PageModes.PageEditor.postRequest('{click}',null,false)"
        };
    }
}