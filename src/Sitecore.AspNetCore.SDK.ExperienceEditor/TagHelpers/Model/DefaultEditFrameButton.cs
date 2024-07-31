namespace Sitecore.AspNetCore.SDK.ExperienceEditor.TagHelpers.Model;

/// <summary>
/// This class contains a set of default edit frame buttons.
/// </summary>
public static class DefaultEditFrameButton
{
    /// <summary>
    /// Gets edit layout button.
    /// </summary>
    public static WebEditButton EditLayout => new()
    {
        Header = "Edit Layout",
        Icon = "/~/icon/Office/16x16/document_selection.png",
        Click = "webedit:openexperienceeditor",
        Tooltip = "Open the item for editing",
    };

    /// <summary>
    /// Gets delete button.
    /// </summary>
    public static WebEditButton Delete => new()
    {
        Header = "Delete Link",
        Icon = "/~/icon/Office/16x16/delete.png",
        Click = "webedit:delete",
        Tooltip = "Delete the item",
    };

    /// <summary>
    /// Gets move up button.
    /// </summary>
    public static WebEditButton MoveUp => new()
    {
        Header = "Move Up",
        Icon = "/~/icon/Office/16x16/navigate_up.png",
        Click = "item:moveup",
        Tooltip = "Move the item up",
    };

    /// <summary>
    /// Gets move down button.
    /// </summary>
    public static WebEditButton MoveDown => new()
    {
        Header = "Move Down",
        Icon = "/~/icon/Office/16x16/navigate_down.png",
        Click = "item:movedown",
        Tooltip = "Move the item down",
    };

    /// <summary>
    /// Gets move first button.
    /// </summary>
    public static WebEditButton MoveFirst => new()
    {
        Header = "Move First",
        Icon = "/~/icon/Office/16x16/navigate_up2.png",
        Click = "item:movefirst",
        Tooltip = "Move the item first",
    };

    /// <summary>
    /// Gets move last button.
    /// </summary>
    public static WebEditButton MoveLast => new()
    {
        Header = "Move Last",
        Icon = "/~/icon/Office/16x16/navigate_down2.png",
        Click = "item:movelast",
        Tooltip = "Move the item last",
    };

    /// <summary>
    /// Gets insert button.
    /// </summary>
    public static WebEditButton Insert => new()
    {
        Header = "Insert New",
        Icon = "/~/icon/Office/16x16/insert_from_template.png",
        Click = "webedit:new",
        Tooltip = "Insert a new item",
    };
}