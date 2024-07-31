using System.Globalization;
using System.Text.Encodings.Web;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Fields;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Presentation;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Properties;
using File = Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Properties.File;

// ReSharper disable StringLiteralTypo
namespace Sitecore.AspNetCore.SDK.TestData;

public static class CannedResponses
{
    public static SitecoreLayoutResponseContent Simple => new()
    {
        Sitecore = new SitecoreData
        {
            Context = new Context
            {
                IsEditing = true,
                Language = "en",
                PageState = PageState.Normal,
                Site = new Site
                {
                    Name = "Test site"
                }
            },
            Route = new Route
            {
                DatabaseName = "db name",
                DeviceId = "device id",
                ItemId = "some id",
                ItemLanguage = "some language",
                ItemVersion = 50,
                Name = "some name",
                DisplayName = "display name",
                LayoutId = "some layout id",
                TemplateId = "some template id",
                TemplateName = "some template name",
                Fields =
                {
                    ["checkbox"] = new CheckboxField(true),
                    ["date"] = new DateField(DateTime.Now),
                    ["number"] = new NumberField(25),
                    ["rich"] = new RichTextField("this is <b>rich</b>", false),
                    ["rich-encoded"] = new RichTextField("this is &lt;b&gt;rich&lt;/b&gt;"),
                    ["text"] = new TextField("a value"),
                    ["hyperlink"] = new HyperLinkField(new HyperLink { Target = "/" }),
                    ["image"] = new ImageField(new Image { Src = "/images/thing.png" }),
                    ["item"] = new ItemLinkField { Id = Guid.NewGuid() }
                },
                Placeholders =
                {
                    ["p1"] =
                    [
                        new Component
                        {
                            DataSource = "source",
                            Id = "an id",
                            Name = "a name",
                            Parameters =
                            {
                                ["alpha"] = "one"
                            },
                            Fields =
                            {
                                ["checkbox"] = new CheckboxField(true),
                                ["date"] = new DateField(DateTime.Now),
                                ["number"] = new NumberField(25),
                                ["rich"] = new RichTextField("this is <b>rich</b>", false),
                                ["rich-encoded"] = new RichTextField("this is &lt;b&gt;rich&lt;/b&gt;"),
                                ["text"] = new TextField("a value"),
                                ["hyperlink"] = new HyperLinkField(new HyperLink { Target = "/" }),
                                ["image"] = new ImageField(new Image { Src = "/images/thing.png" }),
                                ["item"] = new ItemLinkField { Id = Guid.NewGuid() }
                            }
                        }
                    ]
                }
            }
        }
    };

    public static SitecoreLayoutResponseContent SimpleWithContent => new()
    {
        Sitecore = new SitecoreData
        {
            Context = new Context
            {
                IsEditing = true,
                Language = "en",
                PageState = PageState.Normal,
                Site = new Site
                {
                    Name = "Test site"
                }
            },
            Route = new Route
            {
                DatabaseName = "db name",
                DeviceId = "device id",
                ItemId = "some id",
                ItemLanguage = "some language",
                ItemVersion = 50,
                Name = "some name",
                LayoutId = "some layout id",
                TemplateId = "some template id",
                TemplateName = "some template name",
                Fields =
                {
                    ["content"] = new ContentListField { new() { Id = Guid.NewGuid() } },
                    ["checkbox"] = new CheckboxField(true),
                    ["date"] = new DateField(DateTime.Now),
                    ["number"] = new NumberField(25),
                    ["rich"] = new RichTextField("this is <b>rich</b>", false),
                    ["rich-encoded"] = new RichTextField("this is &lt;b&gt;rich&lt;/b&gt;"),
                    ["text"] = new TextField("a value"),
                    ["hyperlink"] = new HyperLinkField(new HyperLink { Target = "/" }),
                    ["image"] = new ImageField(new Image { Src = "/images/thing.png" }),
                    ["item"] = new ItemLinkField { Id = Guid.NewGuid() }
                },
                Placeholders =
                {
                    ["p1"] =
                    [
                        new Component
                        {
                            DataSource = "source",
                            Id = "an id",
                            Name = "a name",
                            Parameters =
                            {
                                ["alpha"] = "one"
                            },
                            Fields =
                            {
                                ["text1"] = new TextField("a value"),
                                ["hyperlink1"] = new HyperLinkField(new HyperLink { Target = "/" }),
                                ["image1"] = new ImageField(new Image { Src = "/images/thing.png" }),
                                ["item1"] = new ItemLinkField { Id = Guid.NewGuid() }
                            }
                        }
                    ]
                }
            }
        }
    };

    public static SitecoreLayoutResponseContent StyleGuide => new()
    {
        Sitecore = new SitecoreData
        {
            Context = new Context
            {
                Language = "en",
                IsEditing = false,
                PageState = PageState.Normal,
                Site = new Site
                {
                    Name = "JssDisconnectedLayoutService"
                }
            },
            Route = new Route
            {
                DatabaseName = "available-in-connected-mode",
                DeviceId = "available-in-connected-mode",
                ItemId = "available-in-connected-mode",
                ItemLanguage = "en",
                ItemVersion = 1,
                LayoutId = "available-in-connected-mode",
                TemplateId = "available-in-connected-mode",
                TemplateName = "available-in-connected-mode",
                Name = "styleguide",
                Fields =
                {
                    ["pageTitle"] = new TextField("Styleguide | Sitecore JSS")
                },
                Placeholders =
                {
                    ["p1"] =
                    [
                        new Component
                        {
                            DataSource = "source",
                            Id = "an id",
                            Name = "a name",
                            Parameters =
                            {
                                ["alpha"] = "one"
                            },
                            Fields =
                            {
                                ["checkbox"] = new CheckboxField(true),
                                ["date"] = new DateField(DateTime.Now),
                                ["number"] = new NumberField(25),
                                ["rich"] = new RichTextField("this is <b>rich</b>", false),
                                ["rich-encoded"] = new RichTextField("this is &lt;b&gt;rich&lt;/b&gt;"),
                                ["text"] = new TextField("a value"),
                                ["hyperlink"] = new HyperLinkField(new HyperLink { Target = "/" }),
                                ["image"] = new ImageField(new Image { Src = "/images/thing.png" }),
                                ["item"] = new ItemLinkField { Id = Guid.NewGuid() }
                            }
                        }
                    ],
                    ["jss-main"] =
                    [
                        new Component
                        {
                            Id = "{E02DDB9B-A062-5E50-924A-1940D7E053CE}",
                            Name = "ContentBlock",
                            Fields =
                            {
                                ["heading"] = new TextField("JSS Styleguide"),
                                ["content"] = new RichTextField(
                                    "<p>This is a live set of examples of how to use JSS. For more information on using JSS, please see <a href=\"https://jss.sitecore.net\" target=\"_blank\" rel=\"noopener noreferrer\">the documentation</a>.</p>\n<p>The content and layout of this page is defined in <code>/data/routes/styleguide/en.yml</code></p>\n")
                            }
                        },

                        new Component
                        {
                            Id = "{34A6553C-81DE-5CD3-989E-853F6CB6DF8C}",
                            Name = "Styleguide-Layout",
                            Placeholders =
                            {
                                ["jss-styleguide-layout"] =
                                [
                                    new Component
                                    {
                                        Id = "{B7C779DA-2B75-586C-B40D-081FCB864256}",
                                        Name = "Styleguide-Section",
                                        Fields =
                                        {
                                            ["heading"] = new TextField("Content Data")
                                        },
                                        Placeholders =
                                        {
                                            ["jss-styleguide-section"] =
                                            [
                                                new Component
                                                {
                                                    Id = "{63B0C99E-DAC7-5670-9D66-C26A78000EAE}",
                                                    Name = "Styleguide-FieldUsage-Text",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Single-Line Text"),
                                                        ["sample"] =
                                                            new RichTextField(
                                                                "This is a sample text field. <mark>HTML is encoded.</mark> In Sitecore, editors will see a <input type=\"text\">."),
                                                        ["sample2"] =
                                                            new RichTextField(
                                                                "This is another sample text field using rendering options. <mark>HTML supported with encode=false.</mark> Cannot edit because editable=false.")
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{F1EA3BB5-1175-5055-AB11-9C48BF69427A}",
                                                    Name = "Styleguide-FieldUsage-Text",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Multi-Line Text"),
                                                        ["description"] =
                                                            new RichTextField(
                                                                "<small>Multi-line text tells Sitecore to use a <code>textarea</code> for editing; consumption in JSS is the same as single-line text.</small>"),
                                                        ["sample"] =
                                                            new RichTextField(
                                                                "This is a sample multi-line text field. <mark>HTML is encoded.</mark> In Sitecore, editors will see a textarea."),
                                                        ["sample2"] =
                                                            new RichTextField(
                                                                "This is another sample multi-line text field using rendering options. <mark>HTML supported with encode=false.</mark>")
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{69CEBC00-446B-5141-AD1E-450B8D6EE0AD}",
                                                    Name = "Styleguide-FieldUsage-RichText",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Rich Text"),
                                                        ["sample"] =
                                                            new RichTextField(
                                                                "<p>This is a sample rich text field. <mark>HTML is always supported.</mark> In Sitecore, editors will see a WYSIWYG editor for these fields.</p>"),
                                                        ["sample2"] = new RichTextField(
                                                            "<p>Another sample rich text field, using options. Keep markup entered in rich text fields as simple as possible - ideally bare tags only (no classes). Adding a wrapping class can help with styling within rich text blocks.</p>\n<marquee>But you can use any valid HTML in a rich text field!</marquee>\n")
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{5630C0E6-0430-5F6A-AF9E-2D09D600A386}",
                                                    Name = "Styleguide-FieldUsage-Image",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Image"),
                                                        ["sample1"] = new ImageField(new Image
                                                        {
                                                            Src = "/data/media/img/sc_logo.png",
                                                            Alt = "Sitecore Logo"
                                                        }),
                                                        ["sample2"] = new ImageField(new Image
                                                        {
                                                            Src = "/data/media/img/jss_logo.png",
                                                            Alt = "Sitecore JSS Logo"
                                                        })
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{BAD43EF7-8940-504D-A09B-976C17A9A30C}",
                                                    Name = "Styleguide-FieldUsage-File",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("File"),
                                                        ["description"] = new RichTextField(
                                                            "<small>Note: Sitecore does not support inline editing of File fields. The value must be edited in Experience Editor by using the edit rendering fields button (looks like a pencil) with the whole component selected.</small>\n"),
                                                        ["file"] = new FileField(new File
                                                        {
                                                            Src = "/data/media/files/jss.pdf",
                                                            Title = "Example File",
                                                            Description =
                                                                "This data will be added to the Sitecore Media Library on import"
                                                        })
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{FF90D4BD-E50D-5BBF-9213-D25968C9AE75}",
                                                    Name = "Styleguide-FieldUsage-Number",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Number"),
                                                        ["description"] =
                                                            new RichTextField(
                                                                "<small>Number tells Sitecore to use a number entry for editing.</small>"),
                                                        ["sample"] = new NumberField(1.21),
                                                        ["sample2"] = new NumberField(71),
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{B5C1C74A-A81D-59B2-85D8-09BC109B1F70}",
                                                    Name = "Styleguide-FieldUsage-Checkbox",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Checkbox"),
                                                        ["description"] = new RichTextField(
                                                            "<small>Note: Sitecore does not support inline editing of Checkbox fields. The value must be edited in Experience Editor by using the edit rendering fields button (looks like a pencil) with the whole component selected.</small>\n"),
                                                        ["checkbox"] = new CheckboxField(true),
                                                        ["checkbox2"] = new CheckboxField(false),
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{F166A7D6-9EC8-5C53-B825-33405DB7F575}",
                                                    Name = "Styleguide-FieldUsage-Date",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Date"),
                                                        ["description"] = new RichTextField(
                                                            "<p><small>Both <code>Date</code> and <code>DateTime</code> field types are available. Choosing <code>DateTime</code> will make Sitecore show editing UI for time; both types store complete date and time values internally. Date values in JSS are formatted using <a href=\"https://en.wikipedia.org/wiki/ISO_8601#Combined_date_and_time_representations\" target=\"_blank\">ISO 8601 formatted strings</a>, for example <code>2012-04-23T18:25:43.511Z</code>.</small></p>\n<div class=\"alert alert-warning\"><small>Note: this is a JavaScript date format (e.g. <code>new Date().toISOString()</code>), and is different from how Sitecore stores date field values internally. Sitecore-formatted dates will not work.</small></div>\n"),
                                                        ["date"] = new DateField(
                                                            DateTime.Parse(
                                                                "2012-05-04T00:00:00Z",
                                                                CultureInfo.InvariantCulture)),
                                                        ["dateTime"] =
                                                            new DateField(DateTime.Parse(
                                                                "2018-03-14T15:00:00Z",
                                                                CultureInfo.InvariantCulture)),
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{56A9562A-6813-579B-8ED2-FDDAB1BFD3D2}",
                                                    Name = "Styleguide-FieldUsage-Link",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("General Link"),
                                                        ["description"] =
                                                            new RichTextField(
                                                                "<p>A <em>General Link</em> is a field that represents an <code>&lt;a&gt;</code> tag.</p>"),
                                                        ["externalLink"] = new HyperLinkField(new HyperLink
                                                        {
                                                            Href = "https://www.sitecore.com",
                                                            Text = "Link to Sitecore"
                                                        }),
                                                        ["internalLink"] = new HyperLinkField(new HyperLink
                                                            { Href = "/" }),
                                                        ["mediaLink"] = new HyperLinkField(new HyperLink
                                                        {
                                                            Href = "/data/media/files/jss.pdf", Text = "Link to PDF"
                                                        }),
                                                        ["emailLink"] = new HyperLinkField(new HyperLink
                                                        {
                                                            Href = "mailto:foo@bar.com", Text = "Send an Email"
                                                        }),
                                                        ["paramsLink"] = new HyperLinkField(new HyperLink
                                                        {
                                                            Href = "https://dev.sitecore.net",
                                                            Text = "Sitecore Dev Site",
                                                            Target = "_blank",
                                                            Class = "font-weight-bold",
                                                            Title = "<a> title attribute"
                                                        })
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{A44AD1F8-0582-5248-9DF9-52429193A68B}",
                                                    Name = "Styleguide-FieldUsage-ItemLink",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Item Link"),
                                                        ["description"] = new RichTextField(
                                                            "<p>\n  <small>\n    Item Links are a way to reference another content item to use data from it.\n    Referenced items may be shared.\n    To reference multiple content items, use a <em>Content List</em> field.<br />\n    <strong>Note:</strong> Sitecore does not support inline editing of Item Link fields. The value must be edited in Experience Editor by using the edit rendering fields button (looks like a pencil) with the whole component selected.\n  </small>\n</p>\n"),
                                                        ["sharedItemLink"] = new ItemLinkField
                                                        {
                                                            Fields =
                                                            {
                                                                ["textField"] =
                                                                    new TextField(
                                                                        "ItemLink Demo (Shared) Item 1 Text Field")
                                                            }
                                                        },
                                                        ["localItemLink"] = new ItemLinkField
                                                        {
                                                            Fields =
                                                            {
                                                                ["textField"] =
                                                                    new TextField("Referenced item textField")
                                                            }
                                                        }
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{2F609D40-8AD9-540E-901E-23AA2600F3EB}",
                                                    Name = "Styleguide-FieldUsage-ContentList",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Content List"),
                                                        ["description"] = new RichTextField(
                                                            "<p>\n  <small>\n    Content Lists are a way to reference zero or more other content items.\n    Referenced items may be shared.\n    To reference a single content item, use an <em>Item Link</em> field.<br />\n    <strong>Note:</strong> Sitecore does not support inline editing of Content List fields. The value must be edited in Experience Editor by using the edit rendering fields button (looks like a pencil) with the whole component selected.\n  </small>\n</p>\n"),
                                                        ["sharedContentList"] = new ContentListField
                                                        {
                                                            new()
                                                            {
                                                                Fields =
                                                                {
                                                                    ["textField"] =
                                                                        new TextField(
                                                                            "ContentList Demo (Shared) Item 1 Text Field")
                                                                }
                                                            },
                                                            new()
                                                            {
                                                                Fields =
                                                                {
                                                                    ["textField"] =
                                                                        new TextField(
                                                                            "ContentList Demo (Shared) Item 2 Text Field")
                                                                }
                                                            }
                                                        },
                                                        ["localContentList"] = new ContentListField
                                                        {
                                                            new()
                                                            {
                                                                Fields =
                                                                {
                                                                    ["textField"] =
                                                                        new TextField("Hello World Item 1")
                                                                }
                                                            },
                                                            new()
                                                            {
                                                                Fields =
                                                                {
                                                                    ["textField"] =
                                                                        new TextField("Hello World Item 2")
                                                                }
                                                            }
                                                        },
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{352ED63D-796A-5523-89F5-9A991DDA4A8F}",
                                                    Name = "Styleguide-FieldUsage-Custom",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Custom Fields"),
                                                        ["description"] = new RichTextField(
                                                            "<p>\n  <small>\n    Any Sitecore field type can be consumed by JSS.\n    In this sample we consume the <em>Integer</em> field type.<br />\n    <strong>Note:</strong> For field types with complex data, custom <code>FieldSerializer</code>s may need to be implemented on the Sitecore side.\n  </small>\n</p>\n"),
                                                        ["customIntField"] = new Field<int> { Value = 31337 }
                                                    }
                                                }
                                            ]
                                        }
                                    },

                                    new Component
                                    {
                                        Id = "{7DE41A1A-24E4-5963-8206-3BB0B7D9DD69}",
                                        Name = "Styleguide-Section",
                                        Fields =
                                        {
                                            ["heading"] = new TextField("Layout Patterns")
                                        },
                                        Placeholders =
                                        {
                                            ["jss-header"] =
                                            [
                                                new Component
                                                {
                                                    Id = "{1A6FCB1C-E97B-5325-8E4E-6E13997A4A1A}",
                                                    Name = "Styleguide-Layout-Reuse"
                                                }
                                            ],
                                            ["jss-styleguide-section"] =
                                            [
                                                new Component
                                                {
                                                    Id = "{3A5D9C50-D8C1-5A12-8DA8-5D56C2A5A69A}",
                                                    Name = "Styleguide-Layout-Reuse",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Reusing Content"),
                                                        ["description"] = new RichTextField(
                                                            "<p>JSS provides powerful options to reuse content, whether it's sharing a common piece of text across pages or sketching out a site with repeating <em>lorem ipsum</em> content.</p>")
                                                    },
                                                    Placeholders =
                                                    {
                                                        ["jss-reuse-example"] =
                                                        [
                                                            new Component
                                                            {
                                                                Id = "{AA328B8A-D6E1-5B37-8143-250D2E93D6B8}",
                                                                Name = "ContentBlock",
                                                                Fields =
                                                                {
                                                                    ["content"] = new RichTextField(
                                                                        "<p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Quisque felis mauris, pretium id neque vitae, vulputate pellentesque tortor. Mauris hendrerit dolor et ipsum lobortis bibendum non finibus neque. Morbi volutpat aliquam magna id posuere. Duis commodo cursus dui, nec interdum velit congue nec. Aliquam erat volutpat. Aliquam facilisis, sapien quis fringilla tincidunt, magna nulla feugiat neque, a consectetur arcu orci eu augue.</p>")
                                                                }
                                                            },

                                                            new Component
                                                            {
                                                                Id = "{C4330D34-623C-556C-BF4C-97C93D40FB1E}",
                                                                Name = "ContentBlock",
                                                                Fields =
                                                                {
                                                                    ["content"] = new RichTextField(
                                                                        "<p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Quisque felis mauris, pretium id neque vitae, vulputate pellentesque tortor. Mauris hendrerit dolor et ipsum lobortis bibendum non finibus neque. Morbi volutpat aliquam magna id posuere. Duis commodo cursus dui, nec interdum velit congue nec. Aliquam erat volutpat. Aliquam facilisis, sapien quis fringilla tincidunt, magna nulla feugiat neque, a consectetur arcu orci eu augue.</p>")
                                                                }
                                                            },

                                                            new Component
                                                            {
                                                                Id = "{A42D8B1C-193D-5627-9130-F7F7F87617F1}",
                                                                Name = "ContentBlock",
                                                                Fields =
                                                                {
                                                                    ["content"] = new RichTextField(
                                                                        "<p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Quisque felis mauris, pretium id neque vitae, vulputate pellentesque tortor. Mauris hendrerit dolor et ipsum lobortis bibendum non finibus neque. Morbi volutpat aliquam magna id posuere. Duis commodo cursus dui, nec interdum velit congue nec. Aliquam erat volutpat. Aliquam facilisis, sapien quis fringilla tincidunt, magna nulla feugiat neque, a consectetur arcu orci eu augue.</p>")
                                                                }
                                                            },

                                                            new Component
                                                            {
                                                                Id = "{0F4CB47A-979E-5139-B50B-A8E40C73C236}",
                                                                Name = "ContentBlock",
                                                                Fields =
                                                                {
                                                                    ["content"] =
                                                                        new RichTextField(
                                                                            "<p>Mix and match reused and local content. Check out <code>/data/routes/styleguide/en.yml</code> to see how.</p>")
                                                                }
                                                            }
                                                        ]
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{538E4831-F157-50BB-AC74-277FCAC9FDDB}",
                                                    Name = "Styleguide-Layout-Tabs",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Tabs"),
                                                        ["description"] =
                                                            new RichTextField(
                                                                "<p>Creating hierarchical components like tabs is made simpler in JSS because it's easy to introspect the layout structure.</p>")
                                                    },
                                                    Placeholders =
                                                    {
                                                        ["jss-tabs"] =
                                                        [
                                                            new Component
                                                            {
                                                                Id = "{7ECB2ED2-AC9B-58D1-8365-10CA74824AF7}",
                                                                Name = "Styleguide-Layout-Tabs-Tab",
                                                                Fields =
                                                                {
                                                                    ["title"] = new TextField("Tab 1"),
                                                                    ["content"] =
                                                                        new RichTextField("<p>Tab 1 contents!</p>")
                                                                }
                                                            },

                                                            new Component
                                                            {
                                                                Id = "{AFD64900-0A61-50EB-A674-A7A884E0D496}",
                                                                Name = "Styleguide-Layout-Tabs-Tab",
                                                                Fields =
                                                                {
                                                                    ["title"] = new TextField("Tab 2"),
                                                                    ["content"] =
                                                                        new RichTextField("<p>Tab 2 contents!</p>")
                                                                }
                                                            },

                                                            new Component
                                                            {
                                                                Id = "{44C12983-3A84-5462-84C0-6CA1430050C8}",
                                                                Name = "Styleguide-Layout-Tabs-Tab",
                                                                Fields =
                                                                {
                                                                    ["title"] = new TextField("Tab 3"),
                                                                    ["content"] =
                                                                        new RichTextField("<p>Tab 3 contents!</p>")
                                                                }
                                                            }
                                                        ]
                                                    }
                                                }
                                            ]
                                        }
                                    },

                                    new Component
                                    {
                                        Id = "{2D806C25-DD46-51E3-93DE-63CF9035122C}",
                                        Name = "Styleguide-Section",
                                        Fields =
                                        {
                                            ["heading"] = new TextField("Sitecore Patterns")
                                        },
                                        Placeholders =
                                        {
                                            ["jss-styleguide-section"] =
                                            [
                                                new Component
                                                {
                                                    Id = "{471FA16A-BB82-5C42-9C95-E7EAB1E3BD30}",
                                                    Name = "Styleguide-SitecoreContext",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Sitecore Context"),
                                                        ["description"] = new RichTextField(
                                                            "<p><small>The Sitecore Context contains route-level data about the current context - for example, <code>pageState</code> enables conditionally executing code based on whether Sitecore is in Experience Editor or not.</small></p>")
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{21F21053-8F8A-5436-BC79-E674E246A2FC}",
                                                    Name = "Styleguide-RouteFields",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Route-level Fields"),
                                                        ["description"] = new RichTextField(
                                                            "<p><small>Route-level content fields are defined on the <em>route</em> instead of on a <em>component</em>. This allows multiple components to share the field data on the same route - and querying is much easier on route level fields, making <em>custom route types</em> ideal for filterable/queryable data such as articles.</small></p>")
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{A0A66136-C21F-52E8-A2EA-F04DCFA6A027}",
                                                    Name = "Styleguide-ComponentParams",
                                                    Parameters =
                                                    {
                                                        ["cssClass"] = "alert alert-success",
                                                        ["columns"] = "5",
                                                        ["useCallToAction"] = "true"
                                                    },
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Component Params"),
                                                        ["description"] = new RichTextField(
                                                            "<p><small>Component params (also called Rendering Parameters) allow storing non-content parameters for a component. These params should be used for more technical options such as CSS class names or structural settings.</small></p>")
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{7F765FCB-3B10-58FD-8AA7-B346EF38C9BB}",
                                                    Name = "Styleguide-Tracking",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Tracking"),
                                                        ["description"] =
                                                            new RichTextField(
                                                                "<p><small>JSS supports tracking Sitecore analytics events from within apps. Give it a try with this handy interactive demo.</small></p>")
                                                    }
                                                }
                                            ]
                                        }
                                    },

                                    new Component
                                    {
                                        Id = "{66AF8F03-0B52-5425-A6AF-6FB54F2D64D9}",
                                        Name = "Styleguide-Section",
                                        Fields =
                                        {
                                            ["heading"] = new TextField("Multilingual Patterns")
                                        },
                                        Placeholders =
                                        {
                                            ["jss-styleguide-section"] =
                                            [
                                                new Component
                                                {
                                                    Id = "{CF1B5D2B-C949-56E7-9594-66AFACEACA9D}",
                                                    Name = "Styleguide-Multilingual",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Translation Patterns"),
                                                        ["sample"] =
                                                            new TextField("This text can be translated in en.yml")
                                                    }
                                                }
                                            ]
                                        }
                                    }
                                ]
                            }
                        }

                    ],
                    ["jss-styleguide-layout"] = [],
                    ["jss-styleguide-section"] = [],
                    ["jss-header"] = [],
                    ["jss-reuse-example"] = [],
                    ["jss-tabs"] = []
                }
            }
        }
    };

    public static SitecoreLayoutResponseContent StyleGuide1 => new()
    {
        Sitecore = new SitecoreData
        {
            Context = new Context
            {
                Language = "en",
                IsEditing = false,
                PageState = PageState.Normal,
                Site = new Site
                {
                    Name = "JssDisconnectedLayoutService"
                }
            },
            Route = new Route
            {
                DatabaseName = "available-in-connected-mode",
                DeviceId = "available-in-connected-mode",
                ItemId = "available-in-connected-mode",
                ItemLanguage = "en",
                ItemVersion = 1,
                LayoutId = SitecoreLayoutIds.Styleguide1LayoutId,
                TemplateId = "available-in-connected-mode",
                TemplateName = "available-in-connected-mode",
                Name = "styleguide",
                Fields =
                {
                    ["pageTitle"] = new TextField("Styleguide | Sitecore JSS")
                },
                Placeholders =
                {
                    ["jss-main"] =
                    [
                        new Component
                        {
                            Id = "{E02DDB9B-A062-5E50-924A-1940D7E053CE}",
                            Name = "ContentBlock",
                            Fields =
                            {
                                ["heading"] = new TextField("ContentBlock 1 - JSS Styleguide"),
                                ["content"] = new RichTextField(
                                    "<p>This is a live set of examples of how to use JSS. For more information on using JSS, please see <a href=\"https://jss.sitecore.net\" target=\"_blank\" rel=\"noopener noreferrer\">the documentation</a>.</p>\n<p>The content and layout of this page is defined in <code>/data/routes/styleguide/en.yml</code></p>\n")
                            }
                        },

                        new Component
                        {
                            Id = "{E02DDB9B-A062-5E50-924A-1940D7E053CF}",
                            Name = "ContentBlock",
                            Fields =
                            {
                                ["heading"] = new TextField("ContentBlock 2 - JSS Styleguide"),
                                ["content"] = new RichTextField(
                                    "<p>This is a live set of examples of how to use JSS. For more information on using JSS, please see <a href=\"https://jss.sitecore.net\" target=\"_blank\" rel=\"noopener noreferrer\">the documentation</a>.</p>\n<p>The content and layout of this page is defined in <code>/data/routes/styleguide/en.yml</code></p>\n")
                            }
                        },

                        new Component
                        {
                            Id = "{34A6553C-81DE-5CD3-989E-853F6CB6DF8C}",
                            Name = "Styleguide-Layout",
                            Placeholders =
                            {
                                ["jss-styleguide-layout"] =
                                [
                                    new Component
                                    {
                                        Id = "{B7C779DA-2B75-586C-B40D-081FCB864256}",
                                        Name = "Styleguide-Section",
                                        Fields =
                                        {
                                            ["heading"] = new TextField("Content Data")
                                        },
                                        Placeholders =
                                        {
                                            ["jss-styleguide-section"] =
                                            [
                                                new Component
                                                {
                                                    Id = "{63B0C99E-DAC7-5670-9D66-C26A78000EAE}",
                                                    Name = "Styleguide-FieldUsage-Text",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Single-Line Text"),
                                                        ["sample"] =
                                                            new RichTextField(
                                                                "This is a sample text field. <mark>HTML is encoded.</mark> In Sitecore, editors will see a <input type=\"text\">."),
                                                        ["sample2"] =
                                                            new RichTextField(
                                                                "This is another sample text field using rendering options. <mark>HTML supported with encode=false.</mark> Cannot edit because editable=false.")
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{F1EA3BB5-1175-5055-AB11-9C48BF69427A}",
                                                    Name = "Styleguide-FieldUsage-Text",
                                                    Fields =
                                                    {
                                                        ["heading"] =
                                                            new TextField(
                                                                $"This is {Environment.NewLine} Multi-Line Text"),
                                                        ["description"] =
                                                            new RichTextField(
                                                                "<small>Multi-line text tells Sitecore to use a <code>textarea</code> for editing; consumption in JSS is the same as single-line text.</small>"),
                                                        ["sample"] =
                                                            new RichTextField(
                                                                "This is a sample multi-line text field. <mark>HTML is encoded.</mark> In Sitecore, editors will see a textarea."),
                                                        ["sample2"] =
                                                            new RichTextField(
                                                                "This is another sample multi-line text field using rendering options. <mark>HTML supported with encode=false.</mark>")
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{69CEBC00-446B-5141-AD1E-450B8D6EE0AD}",
                                                    Name = "Styleguide-FieldUsage-RichText",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Rich Text"),
                                                        ["sample"] =
                                                            new RichTextField(
                                                                "<p>This is a sample rich text field. <mark>HTML is always supported.</mark> In Sitecore, editors will see a WYSIWYG editor for these fields.</p>"),
                                                        ["sample2"] = new RichTextField(
                                                            "<p>Another sample rich text field, using options. Keep markup entered in rich text fields as simple as possible - ideally bare tags only (no classes). Adding a wrapping class can help with styling within rich text blocks.</p>\n<marquee>But you can use any valid HTML in a rich text field!</marquee>\n")
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{5630C0E6-0430-5F6A-AF9E-2D09D600A386}",
                                                    Name = "Styleguide-FieldUsage-Image",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Image"),
                                                        ["sample1"] = new ImageField(new Image
                                                        {
                                                            Src = "/data/media/img/sc_logo.png",
                                                            Alt = "Sitecore Logo"
                                                        }),
                                                        ["sample2"] = new ImageField(new Image
                                                        {
                                                            Src = "/data/media/img/jss_logo.png",
                                                            Alt = "Sitecore JSS Logo"
                                                        })
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{BAD43EF7-8940-504D-A09B-976C17A9A30C}",
                                                    Name = "Styleguide-FieldUsage-File",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("File"),
                                                        ["description"] = new RichTextField(
                                                            "<small>Note: Sitecore does not support inline editing of File fields. The value must be edited in Experience Editor by using the edit rendering fields button (looks like a pencil) with the whole component selected.</small>\n"),
                                                        ["file"] = new FileField(new File
                                                        {
                                                            Src = "/data/media/files/jss.pdf",
                                                            Title = "Example File",
                                                            Description =
                                                                "This data will be added to the Sitecore Media Library on import"
                                                        })
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{FF90D4BD-E50D-5BBF-9213-D25968C9AE75}",
                                                    Name = "Styleguide-FieldUsage-Number",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Number"),
                                                        ["description"] =
                                                            new RichTextField(
                                                                "<small>Number tells Sitecore to use a number entry for editing.</small>"),
                                                        ["sample"] = new NumberField(1.21),
                                                        ["sample2"] = new NumberField(71),
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{B5C1C74A-A81D-59B2-85D8-09BC109B1F70}",
                                                    Name = "Styleguide-FieldUsage-Checkbox",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Checkbox"),
                                                        ["description"] = new RichTextField(
                                                            "<small>Note: Sitecore does not support inline editing of Checkbox fields. The value must be edited in Experience Editor by using the edit rendering fields button (looks like a pencil) with the whole component selected.</small>\n"),
                                                        ["checkbox"] = new CheckboxField(true),
                                                        ["checkbox2"] = new CheckboxField(false),
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{F166A7D6-9EC8-5C53-B825-33405DB7F575}",
                                                    Name = "Styleguide-FieldUsage-Date",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Date"),
                                                        ["description"] = new RichTextField(
                                                            "<p><small>Both <code>Date</code> and <code>DateTime</code> field types are available. Choosing <code>DateTime</code> will make Sitecore show editing UI for time; both types store complete date and time values internally. Date values in JSS are formatted using <a href=\"https://en.wikipedia.org/wiki/ISO_8601#Combined_date_and_time_representations\" target=\"_blank\">ISO 8601 formatted strings</a>, for example <code>2012-04-23T18:25:43.511Z</code>.</small></p>\n<div class=\"alert alert-warning\"><small>Note: this is a JavaScript date format (e.g. <code>new Date().toISOString()</code>), and is different from how Sitecore stores date field values internally. Sitecore-formatted dates will not work.</small></div>\n"),
                                                        ["date"] = new DateField(
                                                            DateTime.Parse(
                                                                "2012-05-04T00:00:00Z",
                                                                CultureInfo.InvariantCulture)),
                                                        ["dateTime"] =
                                                            new DateField(DateTime.Parse(
                                                                "2018-03-14T15:00:00Z",
                                                                CultureInfo.InvariantCulture)),
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{56A9562A-6813-579B-8ED2-FDDAB1BFD3D2}",
                                                    Name = "Styleguide-FieldUsage-Link",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("General Link"),
                                                        ["description"] =
                                                            new RichTextField(
                                                                "<p>A <em>General Link</em> is a field that represents an <code>&lt;a&gt;</code> tag.</p>"),
                                                        ["externalLink"] = new HyperLinkField(new HyperLink
                                                        {
                                                            Href = "https://www.sitecore.com",
                                                            Text = "Link to Sitecore", Class = "link-class"
                                                        }),
                                                        ["internalLink"] = new HyperLinkField(new HyperLink
                                                            { Href = "/" }),
                                                        ["mediaLink"] = new HyperLinkField(new HyperLink
                                                        {
                                                            Href = "/data/media/files/jss.pdf", Text = "Link to PDF"
                                                        }),
                                                        ["emailLink"] = new HyperLinkField(new HyperLink
                                                        {
                                                            Href = "mailto:foo@bar.com", Text = "Send an Email"
                                                        }),
                                                        ["paramsLink"] = new HyperLinkField(new HyperLink
                                                        {
                                                            Href = "https://dev.sitecore.net",
                                                            Text = "Sitecore Dev Site",
                                                            Target = "_blank",
                                                            Class = "font-weight-bold",
                                                            Title = "<a> title attribute"
                                                        })
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{A44AD1F8-0582-5248-9DF9-52429193A68B}",
                                                    Name = "Styleguide-FieldUsage-ItemLink",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Item Link"),
                                                        ["description"] = new RichTextField(
                                                            "<p>\n  <small>\n    Item Links are a way to reference another content item to use data from it.\n    Referenced items may be shared.\n    To reference multiple content items, use a <em>Content List</em> field.<br />\n    <strong>Note:</strong> Sitecore does not support inline editing of Item Link fields. The value must be edited in Experience Editor by using the edit rendering fields button (looks like a pencil) with the whole component selected.\n  </small>\n</p>\n"),
                                                        ["sharedItemLink"] = new ItemLinkField
                                                        {
                                                            Id = Guid.Parse("325c730f-91e6-53f8-adb4-9b35451a9e9e"),
                                                            Url = "/Content/Styleguide/ItemLinkField/Item1",
                                                            Fields =
                                                            {
                                                                ["textField"] =
                                                                    new TextField(
                                                                        "ItemLink Demo (Shared) Item 1 Text Field")
                                                            }
                                                        },
                                                        ["localItemLink"] = new ItemLinkField
                                                        {
                                                            Id = Guid.Parse("f49a72d9-15e0-53d2-83f4-6b55ea815f62"),
                                                            Url =
                                                                "/styleguide/Page-Components/styleguide-jss-styleguide-section-B73482E131E5A083D77A50554BC74A4758E29636DF6824F6E2F272EE778C28A095/styleguide-jss-styleguide-section-B75151F05CFDC4CAFFE44E5BAED9D59BEA82565EC11CE75B7DEF3634495EC1DAB7",
                                                            Fields =
                                                            {
                                                                ["textField"] =
                                                                    new TextField("Referenced item textField")
                                                            }
                                                        },
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{2F609D40-8AD9-540E-901E-23AA2600F3EB}",
                                                    Name = "Styleguide-FieldUsage-ContentList",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Content List"),
                                                        ["description"] = new RichTextField(
                                                            "<p>\n  <small>\n    Content Lists are a way to reference zero or more other content items.\n    Referenced items may be shared.\n    To reference a single content item, use an <em>Item Link</em> field.<br />\n    <strong>Note:</strong> Sitecore does not support inline editing of Content List fields. The value must be edited in Experience Editor by using the edit rendering fields button (looks like a pencil) with the whole component selected.\n  </small>\n</p>\n"),
                                                        ["sharedContentList"] = new ContentListField
                                                        {
                                                            new()
                                                            {
                                                                Id = Guid.Parse(
                                                                    "5fbb46d4-5090-5c92-967a-89dcee9bb94f"),
                                                                Fields =
                                                                {
                                                                    ["textField"] =
                                                                        new TextField(
                                                                            "ContentList Demo (Shared) Item 1 Text Field")
                                                                }
                                                            },
                                                            new()
                                                            {
                                                                Id = Guid.Parse(
                                                                    "b5eea8da-9b23-51d4-b84a-e0d28af14f7c"),
                                                                Fields =
                                                                {
                                                                    ["textField"] =
                                                                        new TextField(
                                                                            "ContentList Demo (Shared) Item 2 Text Field")
                                                                }
                                                            }
                                                        },
                                                        ["localContentList"] = new ContentListField
                                                        {
                                                            new()
                                                            {
                                                                Id = Guid.Parse(
                                                                    "ed7fd1c3-35df-5d3d-8f9b-b75c97b1abe4"),
                                                                Fields =
                                                                {
                                                                    ["textField"] =
                                                                        new TextField("Hello World Item 1")
                                                                }
                                                            },
                                                            new()
                                                            {
                                                                Id = Guid.Parse(
                                                                    "94cd33e0-08da-525a-a73f-3adb5876424f"),
                                                                Fields =
                                                                {
                                                                    ["textField"] =
                                                                        new TextField("Hello World Item 2")
                                                                }
                                                            }
                                                        },
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{352ED63D-796A-5523-89F5-9A991DDA4A8F}",
                                                    Name = "Styleguide-FieldUsage-Custom",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Custom Fields"),
                                                        ["description"] = new RichTextField(
                                                            "<p>\n  <small>\n    Any Sitecore field type can be consumed by JSS.\n    In this sample we consume the <em>Integer</em> field type.<br />\n    <strong>Note:</strong> For field types with complex data, custom <code>FieldSerializer</code>s may need to be implemented on the Sitecore side.\n  </small>\n</p>\n"),
                                                        ["customIntField"] = new Field<int> { Value = 31337 }
                                                    }
                                                }
                                            ]
                                        }
                                    },

                                    new Component
                                    {
                                        Id = "{7DE41A1A-24E4-5963-8206-3BB0B7D9DD69}",
                                        Name = "Styleguide-Section",
                                        Fields =
                                        {
                                            ["heading"] = new TextField("Layout Patterns")
                                        },
                                        Placeholders =
                                        {
                                            ["jss-header"] =
                                            [
                                                new Component
                                                {
                                                    Id = "{1A6FCB1C-E97B-5325-8E4E-6E13997A4A1A}",
                                                    Name = "Styleguide-Layout-Reuse"
                                                }
                                            ],
                                            ["jss-styleguide-section"] =
                                            [
                                                new Component
                                                {
                                                    Id = "{3A5D9C50-D8C1-5A12-8DA8-5D56C2A5A69A}",
                                                    Name = "Styleguide-Layout-Reuse",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Reusing Content"),
                                                        ["description"] = new RichTextField(
                                                            "<p>JSS provides powerful options to reuse content, whether it's sharing a common piece of text across pages or sketching out a site with repeating <em>lorem ipsum</em> content.</p>")
                                                    },
                                                    Placeholders =
                                                    {
                                                        ["jss-reuse-example"] =
                                                        [
                                                            new Component
                                                            {
                                                                Id = "{AA328B8A-D6E1-5B37-8143-250D2E93D6B8}",
                                                                Name = "ContentBlock",
                                                                Fields =
                                                                {
                                                                    ["content"] = new RichTextField(
                                                                        "<p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Quisque felis mauris, pretium id neque vitae, vulputate pellentesque tortor. Mauris hendrerit dolor et ipsum lobortis bibendum non finibus neque. Morbi volutpat aliquam magna id posuere. Duis commodo cursus dui, nec interdum velit congue nec. Aliquam erat volutpat. Aliquam facilisis, sapien quis fringilla tincidunt, magna nulla feugiat neque, a consectetur arcu orci eu augue.</p>")
                                                                }
                                                            },

                                                            new Component
                                                            {
                                                                Id = "{C4330D34-623C-556C-BF4C-97C93D40FB1E}",
                                                                Name = "ContentBlock",
                                                                Fields =
                                                                {
                                                                    ["content"] = new RichTextField(
                                                                        "<p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Quisque felis mauris, pretium id neque vitae, vulputate pellentesque tortor. Mauris hendrerit dolor et ipsum lobortis bibendum non finibus neque. Morbi volutpat aliquam magna id posuere. Duis commodo cursus dui, nec interdum velit congue nec. Aliquam erat volutpat. Aliquam facilisis, sapien quis fringilla tincidunt, magna nulla feugiat neque, a consectetur arcu orci eu augue.</p>")
                                                                }
                                                            },

                                                            new Component
                                                            {
                                                                Id = "{A42D8B1C-193D-5627-9130-F7F7F87617F1}",
                                                                Name = "ContentBlock",
                                                                Fields =
                                                                {
                                                                    ["content"] = new RichTextField(
                                                                        "<p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Quisque felis mauris, pretium id neque vitae, vulputate pellentesque tortor. Mauris hendrerit dolor et ipsum lobortis bibendum non finibus neque. Morbi volutpat aliquam magna id posuere. Duis commodo cursus dui, nec interdum velit congue nec. Aliquam erat volutpat. Aliquam facilisis, sapien quis fringilla tincidunt, magna nulla feugiat neque, a consectetur arcu orci eu augue.</p>")
                                                                }
                                                            },

                                                            new Component
                                                            {
                                                                Id = "{0F4CB47A-979E-5139-B50B-A8E40C73C236}",
                                                                Name = "ContentBlock",
                                                                Fields =
                                                                {
                                                                    ["content"] =
                                                                        new RichTextField(
                                                                            "<p>Mix and match reused and local content. Check out <code>/data/routes/styleguide/en.yml</code> to see how.</p>")
                                                                }
                                                            }
                                                        ]
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{538E4831-F157-50BB-AC74-277FCAC9FDDB}",
                                                    Name = "Styleguide-Layout-Tabs",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Tabs"),
                                                        ["description"] =
                                                            new RichTextField(
                                                                "<p>Creating hierarchical components like tabs is made simpler in JSS because it's easy to introspect the layout structure.</p>")
                                                    },
                                                    Placeholders =
                                                    {
                                                        ["jss-tabs"] =
                                                        [
                                                            new Component
                                                            {
                                                                Id = "{7ECB2ED2-AC9B-58D1-8365-10CA74824AF7}",
                                                                Name = "Styleguide-Layout-Tabs-Tab",
                                                                Fields =
                                                                {
                                                                    ["title"] = new TextField("Tab 1"),
                                                                    ["content"] =
                                                                        new RichTextField("<p>Tab 1 contents!</p>")
                                                                }
                                                            },

                                                            new Component
                                                            {
                                                                Id = "{AFD64900-0A61-50EB-A674-A7A884E0D496}",
                                                                Name = "Styleguide-Layout-Tabs-Tab",
                                                                Fields =
                                                                {
                                                                    ["title"] = new TextField("Tab 2"),
                                                                    ["content"] =
                                                                        new RichTextField("<p>Tab 2 contents!</p>")
                                                                }
                                                            },

                                                            new Component
                                                            {
                                                                Id = "{44C12983-3A84-5462-84C0-6CA1430050C8}",
                                                                Name = "Styleguide-Layout-Tabs-Tab",
                                                                Fields =
                                                                {
                                                                    ["title"] = new TextField("Tab 3"),
                                                                    ["content"] =
                                                                        new RichTextField("<p>Tab 3 contents!</p>")
                                                                }
                                                            }
                                                        ]
                                                    }
                                                }
                                            ]
                                        }
                                    },

                                    new Component
                                    {
                                        Id = "{2D806C25-DD46-51E3-93DE-63CF9035122C}",
                                        Name = "Styleguide-Section",
                                        Fields =
                                        {
                                            ["heading"] = new TextField("Sitecore Patterns")
                                        },
                                        Placeholders =
                                        {
                                            ["jss-styleguide-section"] =
                                            [
                                                new Component
                                                {
                                                    Id = "{471FA16A-BB82-5C42-9C95-E7EAB1E3BD30}",
                                                    Name = "Styleguide-SitecoreContext",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Sitecore Context"),
                                                        ["description"] = new RichTextField(
                                                            "<p><small>The Sitecore Context contains route-level data about the current context - for example, <code>pageState</code> enables conditionally executing code based on whether Sitecore is in Experience Editor or not.</small></p>")
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{21F21053-8F8A-5436-BC79-E674E246A2FC}",
                                                    Name = "Styleguide-RouteFields",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Route-level Fields"),
                                                        ["description"] = new RichTextField(
                                                            "<p><small>Route-level content fields are defined on the <em>route</em> instead of on a <em>component</em>. This allows multiple components to share the field data on the same route - and querying is much easier on route level fields, making <em>custom route types</em> ideal for filterable/queryable data such as articles.</small></p>")
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{A0A66136-C21F-52E8-A2EA-F04DCFA6A027}",
                                                    Name = "Styleguide-ComponentParams",
                                                    Parameters =
                                                    {
                                                        ["cssClass"] = "alert alert-success",
                                                        ["columns"] = "5",
                                                        ["useCallToAction"] = "true"
                                                    },
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Component Params"),
                                                        ["description"] = new RichTextField(
                                                            "<p><small>Component params (also called Rendering Parameters) allow storing non-content parameters for a component. These params should be used for more technical options such as CSS class names or structural settings.</small></p>")
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{7F765FCB-3B10-58FD-8AA7-B346EF38C9BB}",
                                                    Name = "Styleguide-Tracking",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Tracking"),
                                                        ["description"] =
                                                            new RichTextField(
                                                                "<p><small>JSS supports tracking Sitecore analytics events from within apps. Give it a try with this handy interactive demo.</small></p>")
                                                    }
                                                }
                                            ]
                                        }
                                    },

                                    new Component
                                    {
                                        Id = "{66AF8F03-0B52-5425-A6AF-6FB54F2D64D9}",
                                        Name = "Styleguide-Section",
                                        Fields =
                                        {
                                            ["heading"] = new TextField("Multilingual Patterns")
                                        },
                                        Placeholders =
                                        {
                                            ["jss-styleguide-section"] =
                                            [
                                                new Component
                                                {
                                                    Id = "{CF1B5D2B-C949-56E7-9594-66AFACEACA9D}",
                                                    Name = "Styleguide-Multilingual",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Translation Patterns"),
                                                        ["sample"] =
                                                            new TextField("This text can be translated in en.yml")
                                                    }
                                                }
                                            ]
                                        }
                                    }
                                ]
                            }
                        }
                    ],
                    ["jss-styleguide-layout"] = [],
                    ["jss-styleguide-section"] = [],
                    ["jss-header"] = [],
                    ["jss-reuse-example"] = [],
                    ["jss-tabs"] = []
                }
            },
            Devices =
            [
                new Device
                {
                    Id = "fe5d7fdf-89c0-4d99-9aa3-b5fbd009c9f3",
                    LayoutId = "14030e9f-ce92-49c6-ad87-7d49b50e42ea",
                    Renderings =
                    [
                        new Rendering
                        {
                            Id = "885b8314-7d8c-4cbb-8000-01421ea8f406",
                            InstanceId = "43222d12-08c9-453b-ae96-d406ebb95126",
                            PlaceholderKey = "main"
                        },

                        new Rendering
                        {
                            Id = "ce4adcfb-7990-4980-83fb-a00c1e3673db",
                            InstanceId = "cf044ad9-0332-407a-abde-587214a2c808",
                            PlaceholderKey = "/main/centercolumn"
                        },

                        new Rendering
                        {
                            Id = "493b3a83-0fa7-4484-8fc9-4680991cf743",
                            InstanceId = "b343725a-3a93-446e-a9c8-3a2cbd3db489",
                            PlaceholderKey = "/main/centercolumn/content"
                        }
                    ]
                },

                new Device
                {
                    Id = "46d2f427-4ce5-4e1f-ba10-ef3636f43534",
                    LayoutId = "14030e9f-ce92-49c6-ad87-7d49b50e42ea",
                    Renderings =
                    [
                        new Rendering
                        {
                            Id = "493b3a83-0fa7-4484-8fc9-4680991cf743",
                            InstanceId = "a08c9132-dbd1-474f-a2ca-6ca26a4aa650",
                            PlaceholderKey = "content"
                        }
                    ]
                }
            ]
        }
    };

    public static SitecoreLayoutResponseContent StyleGuide2 => new()
    {
        Sitecore = new SitecoreData
        {
            Context = new Context
            {
                Language = "en",
                IsEditing = false,
                PageState = PageState.Normal,
                Site = new Site
                {
                    Name = "JssDisconnectedLayoutService"
                }
            },
            Route = new Route
            {
                DatabaseName = "available-in-connected-mode",
                DeviceId = "available-in-connected-mode",
                ItemId = "available-in-connected-mode",
                ItemLanguage = "en",
                ItemVersion = 1,
                LayoutId = SitecoreLayoutIds.Styleguide2LayoutId,
                TemplateId = "available-in-connected-mode",
                TemplateName = "available-in-connected-mode",
                Name = "styleguide",
                Fields =
                {
                    ["pageTitle"] = new TextField("Styleguide | Sitecore JSS")
                },
                Placeholders =
                {
                    ["jss-main"] =
                    [
                        new Component
                        {
                            Id = "{E02DDB9B-A062-5E50-924A-1940D7E053CE}",
                            Name = "ContentBlock",
                            Fields =
                            {
                                ["heading"] = new TextField("JSS Styleguide"),
                                ["content"] = new RichTextField(
                                    "<p>This is a live set of examples of how to use JSS. For more information on using JSS, please see <a href=\"https://jss.sitecore.net\" target=\"_blank\" rel=\"noopener noreferrer\">the documentation</a>.</p>\n<p>The content and layout of this page is defined in <code>/data/routes/styleguide/en.yml</code></p>\n")
                            }
                        },

                        new Component
                        {
                            Id = "{34A6553C-81DE-5CD3-989E-853F6CB6DF8C}",
                            Name = "Styleguide-Layout",
                            Placeholders =
                            {
                                ["jss-styleguide-layout"] =
                                [
                                    new Component
                                    {
                                        Id = "{B7C779DA-2B75-586C-B40D-081FCB864256}",
                                        Name = "Styleguide-Section",
                                        Fields =
                                        {
                                            ["heading"] = new TextField("Content Data")
                                        },
                                        Placeholders =
                                        {
                                            ["jss-styleguide-section"] =
                                            [
                                                new Component
                                                {
                                                    Id = "{63B0C99E-DAC7-5670-9D66-C26A78000EAE}",
                                                    Name = "Styleguide-FieldUsage-Text",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Single-Line Text"),
                                                        ["sample"] =
                                                            new RichTextField(
                                                                "This is a sample text field. <mark>HTML is encoded.</mark> In Sitecore, editors will see a <input type=\"text\">."),
                                                        ["sample2"] =
                                                            new RichTextField(
                                                                "This is another sample text field using rendering options. <mark>HTML supported with encode=false.</mark> Cannot edit because editable=false.")
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{F1EA3BB5-1175-5055-AB11-9C48BF69427A}",
                                                    Name = "Styleguide-FieldUsage-Text",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Multi-Line Text"),
                                                        ["description"] =
                                                            new RichTextField(
                                                                "<small>Multi-line text tells Sitecore to use a <code>textarea</code> for editing; consumption in JSS is the same as single-line text.</small>"),
                                                        ["sample"] =
                                                            new RichTextField(
                                                                "This is a sample multi-line text field. <mark>HTML is encoded.</mark> In Sitecore, editors will see a textarea."),
                                                        ["sample2"] =
                                                            new RichTextField(
                                                                "This is another sample multi-line text field using rendering options. <mark>HTML supported with encode=false.</mark>")
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{69CEBC00-446B-5141-AD1E-450B8D6EE0AD}",
                                                    Name = "Styleguide-FieldUsage-RichText",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Rich Text"),
                                                        ["sample"] =
                                                            new RichTextField(
                                                                "<p>This is a sample rich text field. <mark>HTML is always supported.</mark> In Sitecore, editors will see a WYSIWYG editor for these fields.</p>"),
                                                        ["sample2"] = new RichTextField(
                                                            "<p>Another sample rich text field, using options. Keep markup entered in rich text fields as simple as possible - ideally bare tags only (no classes). Adding a wrapping class can help with styling within rich text blocks.</p>\n<marquee>But you can use any valid HTML in a rich text field!</marquee>\n")
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{5630C0E6-0430-5F6A-AF9E-2D09D600A386}",
                                                    Name = "Styleguide-FieldUsage-Image",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Image"),
                                                        ["sample1"] = new ImageField(new Image
                                                        {
                                                            Src = "/data/media/img/sc_logo.png",
                                                            Alt = "Sitecore Logo"
                                                        }),
                                                        ["sample2"] = new ImageField(new Image
                                                        {
                                                            Src = "/data/media/img/jss_logo.png",
                                                            Alt = "Sitecore JSS Logo"
                                                        })
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{BAD43EF7-8940-504D-A09B-976C17A9A30C}",
                                                    Name = "Styleguide-FieldUsage-File",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("File"),
                                                        ["description"] = new RichTextField(
                                                            "<small>Note: Sitecore does not support inline editing of File fields. The value must be edited in Experience Editor by using the edit rendering fields button (looks like a pencil) with the whole component selected.</small>\n"),
                                                        ["file"] = new FileField(new File
                                                        {
                                                            Src = "/data/media/files/jss.pdf",
                                                            Title = "Example File",
                                                            Description =
                                                                "This data will be added to the Sitecore Media Library on import"
                                                        })
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{FF90D4BD-E50D-5BBF-9213-D25968C9AE75}",
                                                    Name = "Styleguide-FieldUsage-Number",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Number"),
                                                        ["description"] =
                                                            new RichTextField(
                                                                "<small>Number tells Sitecore to use a number entry for editing.</small>"),
                                                        ["sample"] = new NumberField(1.21),
                                                        ["sample2"] = new NumberField(71),
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{B5C1C74A-A81D-59B2-85D8-09BC109B1F70}",
                                                    Name = "Styleguide-FieldUsage-Checkbox",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Checkbox"),
                                                        ["description"] = new RichTextField(
                                                            "<small>Note: Sitecore does not support inline editing of Checkbox fields. The value must be edited in Experience Editor by using the edit rendering fields button (looks like a pencil) with the whole component selected.</small>\n"),
                                                        ["checkbox"] = new CheckboxField(true),
                                                        ["checkbox2"] = new CheckboxField(false),
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{F166A7D6-9EC8-5C53-B825-33405DB7F575}",
                                                    Name = "Styleguide-FieldUsage-Date",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Date"),
                                                        ["description"] = new RichTextField(
                                                            "<p><small>Both <code>Date</code> and <code>DateTime</code> field types are available. Choosing <code>DateTime</code> will make Sitecore show editing UI for time; both types store complete date and time values internally. Date values in JSS are formatted using <a href=\"https://en.wikipedia.org/wiki/ISO_8601#Combined_date_and_time_representations\" target=\"_blank\">ISO 8601 formatted strings</a>, for example <code>2012-04-23T18:25:43.511Z</code>.</small></p>\n<div class=\"alert alert-warning\"><small>Note: this is a JavaScript date format (e.g. <code>new Date().toISOString()</code>), and is different from how Sitecore stores date field values internally. Sitecore-formatted dates will not work.</small></div>\n"),
                                                        ["date"] = new DateField(
                                                            DateTime.Parse(
                                                                "2012-05-04T00:00:00Z",
                                                                CultureInfo.InvariantCulture)),
                                                        ["dateTime"] =
                                                            new DateField(DateTime.Parse(
                                                                "2018-03-14T15:00:00Z",
                                                                CultureInfo.InvariantCulture)),
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{56A9562A-6813-579B-8ED2-FDDAB1BFD3D2}",
                                                    Name = "Styleguide-FieldUsage-Link",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("General Link"),
                                                        ["description"] =
                                                            new RichTextField(
                                                                "<p>A <em>General Link</em> is a field that represents an <code>&lt;a&gt;</code> tag.</p>"),
                                                        ["externalLink"] = new HyperLinkField(new HyperLink
                                                        {
                                                            Href = "https://www.sitecore.com",
                                                            Text = "Link to Sitecore"
                                                        }),
                                                        ["internalLink"] = new HyperLinkField(new HyperLink
                                                            { Href = "/" }),
                                                        ["mediaLink"] = new HyperLinkField(new HyperLink
                                                        {
                                                            Href = "/data/media/files/jss.pdf", Text = "Link to PDF"
                                                        }),
                                                        ["emailLink"] = new HyperLinkField(new HyperLink
                                                        {
                                                            Href = "mailto:foo@bar.com", Text = "Send an Email"
                                                        }),
                                                        ["paramsLink"] = new HyperLinkField(new HyperLink
                                                        {
                                                            Href = "https://dev.sitecore.net",
                                                            Text = "Sitecore Dev Site",
                                                            Target = "_blank",
                                                            Class = "font-weight-bold",
                                                            Title = "<a> title attribute"
                                                        })
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{A44AD1F8-0582-5248-9DF9-52429193A68B}",
                                                    Name = "Styleguide-FieldUsage-ItemLink",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Item Link"),
                                                        ["description"] = new RichTextField(
                                                            "<p>\n  <small>\n    Item Links are a way to reference another content item to use data from it.\n    Referenced items may be shared.\n    To reference multiple content items, use a <em>Content List</em> field.<br />\n    <strong>Note:</strong> Sitecore does not support inline editing of Item Link fields. The value must be edited in Experience Editor by using the edit rendering fields button (looks like a pencil) with the whole component selected.\n  </small>\n</p>\n"),
                                                        ["sharedItemLink"] = new ItemLinkField
                                                        {
                                                            Fields =
                                                            {
                                                                ["textField"] =
                                                                    new TextField(
                                                                        "ItemLink Demo (Shared) Item 1 Text Field")
                                                            }
                                                        },
                                                        ["localItemLink"] = new ItemLinkField
                                                        {
                                                            Fields =
                                                            {
                                                                ["textField"] =
                                                                    new TextField("Referenced item textField")
                                                            }
                                                        },
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{2F609D40-8AD9-540E-901E-23AA2600F3EB}",
                                                    Name = "Styleguide-FieldUsage-ContentList",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Content List"),
                                                        ["description"] = new RichTextField(
                                                            "<p>\n  <small>\n    Content Lists are a way to reference zero or more other content items.\n    Referenced items may be shared.\n    To reference a single content item, use an <em>Item Link</em> field.<br />\n    <strong>Note:</strong> Sitecore does not support inline editing of Content List fields. The value must be edited in Experience Editor by using the edit rendering fields button (looks like a pencil) with the whole component selected.\n  </small>\n</p>\n"),
                                                        ["sharedContentList"] = new ContentListField
                                                        {
                                                            new()
                                                            {
                                                                Fields =
                                                                {
                                                                    ["textField"] =
                                                                        new TextField(
                                                                            "ContentList Demo (Shared) Item 1 Text Field")
                                                                }
                                                            },
                                                            new()
                                                            {
                                                                Fields =
                                                                {
                                                                    ["textField"] =
                                                                        new TextField(
                                                                            "ContentList Demo (Shared) Item 2 Text Field")
                                                                }
                                                            }
                                                        },
                                                        ["localContentList"] = new ContentListField
                                                        {
                                                            new()
                                                            {
                                                                Fields =
                                                                {
                                                                    ["textField"] =
                                                                        new TextField("Hello World Item 1")
                                                                }
                                                            },
                                                            new()
                                                            {
                                                                Fields =
                                                                {
                                                                    ["textField"] =
                                                                        new TextField("Hello World Item 2")
                                                                }
                                                            }
                                                        },
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{352ED63D-796A-5523-89F5-9A991DDA4A8F}",
                                                    Name = "Styleguide-FieldUsage-Custom",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Custom Fields"),
                                                        ["description"] = new RichTextField(
                                                            "<p>\n  <small>\n    Any Sitecore field type can be consumed by JSS.\n    In this sample we consume the <em>Integer</em> field type.<br />\n    <strong>Note:</strong> For field types with complex data, custom <code>FieldSerializer</code>s may need to be implemented on the Sitecore side.\n  </small>\n</p>\n"),
                                                        ["customIntField"] = new Field<int> { Value = 31337 }
                                                    }
                                                }
                                            ]
                                        }
                                    },

                                    new Component
                                    {
                                        Id = "{7DE41A1A-24E4-5963-8206-3BB0B7D9DD69}",
                                        Name = "Styleguide-Section",
                                        Fields =
                                        {
                                            ["heading"] = new TextField("Layout Patterns")
                                        },
                                        Placeholders =
                                        {
                                            ["jss-header"] =
                                            [
                                                new Component
                                                {
                                                    Id = "{1A6FCB1C-E97B-5325-8E4E-6E13997A4A1A}",
                                                    Name = "Styleguide-Layout-Reuse"
                                                }
                                            ],
                                            ["jss-styleguide-section"] =
                                            [
                                                new Component
                                                {
                                                    Id = "{3A5D9C50-D8C1-5A12-8DA8-5D56C2A5A69A}",
                                                    Name = "Styleguide-Layout-Reuse",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Reusing Content"),
                                                        ["description"] = new RichTextField(
                                                            "<p>JSS provides powerful options to reuse content, whether it's sharing a common piece of text across pages or sketching out a site with repeating <em>lorem ipsum</em> content.</p>")
                                                    },
                                                    Placeholders =
                                                    {
                                                        ["jss-reuse-example"] =
                                                        [
                                                            new Component
                                                            {
                                                                Id = "{AA328B8A-D6E1-5B37-8143-250D2E93D6B8}",
                                                                Name = "ContentBlock",
                                                                Fields =
                                                                {
                                                                    ["content"] = new RichTextField(
                                                                        "<p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Quisque felis mauris, pretium id neque vitae, vulputate pellentesque tortor. Mauris hendrerit dolor et ipsum lobortis bibendum non finibus neque. Morbi volutpat aliquam magna id posuere. Duis commodo cursus dui, nec interdum velit congue nec. Aliquam erat volutpat. Aliquam facilisis, sapien quis fringilla tincidunt, magna nulla feugiat neque, a consectetur arcu orci eu augue.</p>")
                                                                }
                                                            },

                                                            new Component
                                                            {
                                                                Id = "{C4330D34-623C-556C-BF4C-97C93D40FB1E}",
                                                                Name = "ContentBlock",
                                                                Fields =
                                                                {
                                                                    ["content"] = new RichTextField(
                                                                        "<p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Quisque felis mauris, pretium id neque vitae, vulputate pellentesque tortor. Mauris hendrerit dolor et ipsum lobortis bibendum non finibus neque. Morbi volutpat aliquam magna id posuere. Duis commodo cursus dui, nec interdum velit congue nec. Aliquam erat volutpat. Aliquam facilisis, sapien quis fringilla tincidunt, magna nulla feugiat neque, a consectetur arcu orci eu augue.</p>")
                                                                }
                                                            },

                                                            new Component
                                                            {
                                                                Id = "{A42D8B1C-193D-5627-9130-F7F7F87617F1}",
                                                                Name = "ContentBlock",
                                                                Fields =
                                                                {
                                                                    ["content"] = new RichTextField(
                                                                        "<p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Quisque felis mauris, pretium id neque vitae, vulputate pellentesque tortor. Mauris hendrerit dolor et ipsum lobortis bibendum non finibus neque. Morbi volutpat aliquam magna id posuere. Duis commodo cursus dui, nec interdum velit congue nec. Aliquam erat volutpat. Aliquam facilisis, sapien quis fringilla tincidunt, magna nulla feugiat neque, a consectetur arcu orci eu augue.</p>")
                                                                }
                                                            },

                                                            new Component
                                                            {
                                                                Id = "{0F4CB47A-979E-5139-B50B-A8E40C73C236}",
                                                                Name = "ContentBlock",
                                                                Fields =
                                                                {
                                                                    ["content"] =
                                                                        new RichTextField(
                                                                            "<p>Mix and match reused and local content. Check out <code>/data/routes/styleguide/en.yml</code> to see how.</p>")
                                                                }
                                                            }
                                                        ]
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{538E4831-F157-50BB-AC74-277FCAC9FDDB}",
                                                    Name = "Styleguide-Layout-Tabs",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Tabs"),
                                                        ["description"] =
                                                            new RichTextField(
                                                                "<p>Creating hierarchical components like tabs is made simpler in JSS because it's easy to introspect the layout structure.</p>")
                                                    },
                                                    Placeholders =
                                                    {
                                                        ["jss-tabs"] =
                                                        [
                                                            new Component
                                                            {
                                                                Id = "{7ECB2ED2-AC9B-58D1-8365-10CA74824AF7}",
                                                                Name = "Styleguide-Layout-Tabs-Tab",
                                                                Fields =
                                                                {
                                                                    ["title"] = new TextField("Tab 1"),
                                                                    ["content"] =
                                                                        new RichTextField("<p>Tab 1 contents!</p>")
                                                                }
                                                            },

                                                            new Component
                                                            {
                                                                Id = "{AFD64900-0A61-50EB-A674-A7A884E0D496}",
                                                                Name = "Styleguide-Layout-Tabs-Tab",
                                                                Fields =
                                                                {
                                                                    ["title"] = new TextField("Tab 2"),
                                                                    ["content"] =
                                                                        new RichTextField("<p>Tab 2 contents!</p>")
                                                                }
                                                            },

                                                            new Component
                                                            {
                                                                Id = "{44C12983-3A84-5462-84C0-6CA1430050C8}",
                                                                Name = "Styleguide-Layout-Tabs-Tab",
                                                                Fields =
                                                                {
                                                                    ["title"] = new TextField("Tab 3"),
                                                                    ["content"] =
                                                                        new RichTextField("<p>Tab 3 contents!</p>")
                                                                }
                                                            }
                                                        ]
                                                    }
                                                }
                                            ]
                                        }
                                    },

                                    new Component
                                    {
                                        Id = "{2D806C25-DD46-51E3-93DE-63CF9035122C}",
                                        Name = "Styleguide-Section",
                                        Fields =
                                        {
                                            ["heading"] = new TextField("Sitecore Patterns")
                                        },
                                        Placeholders =
                                        {
                                            ["jss-styleguide-section"] =
                                            [
                                                new Component
                                                {
                                                    Id = "{471FA16A-BB82-5C42-9C95-E7EAB1E3BD30}",
                                                    Name = "Styleguide-SitecoreContext",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Sitecore Context"),
                                                        ["description"] = new RichTextField(
                                                            "<p><small>The Sitecore Context contains route-level data about the current context - for example, <code>pageState</code> enables conditionally executing code based on whether Sitecore is in Experience Editor or not.</small></p>")
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{21F21053-8F8A-5436-BC79-E674E246A2FC}",
                                                    Name = "Styleguide-RouteFields",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Route-level Fields"),
                                                        ["description"] = new RichTextField(
                                                            "<p><small>Route-level content fields are defined on the <em>route</em> instead of on a <em>component</em>. This allows multiple components to share the field data on the same route - and querying is much easier on route level fields, making <em>custom route types</em> ideal for filterable/queryable data such as articles.</small></p>")
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{A0A66136-C21F-52E8-A2EA-F04DCFA6A027}",
                                                    Name = "Styleguide-ComponentParams",
                                                    Parameters =
                                                    {
                                                        ["cssClass"] = "alert alert-success",
                                                        ["columns"] = "5",
                                                        ["useCallToAction"] = "true"
                                                    },
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Component Params"),
                                                        ["description"] = new RichTextField(
                                                            "<p><small>Component params (also called Rendering Parameters) allow storing non-content parameters for a component. These params should be used for more technical options such as CSS class names or structural settings.</small></p>")
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{7F765FCB-3B10-58FD-8AA7-B346EF38C9BB}",
                                                    Name = "Styleguide-Tracking",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Tracking"),
                                                        ["description"] =
                                                            new RichTextField(
                                                                "<p><small>JSS supports tracking Sitecore analytics events from within apps. Give it a try with this handy interactive demo.</small></p>")
                                                    }
                                                }
                                            ]
                                        }
                                    },

                                    new Component
                                    {
                                        Id = "{66AF8F03-0B52-5425-A6AF-6FB54F2D64D9}",
                                        Name = "Styleguide-Section",
                                        Fields =
                                        {
                                            ["heading"] = new TextField("Multilingual Patterns")
                                        },
                                        Placeholders =
                                        {
                                            ["jss-styleguide-section"] =
                                            [
                                                new Component
                                                {
                                                    Id = "{CF1B5D2B-C949-56E7-9594-66AFACEACA9D}",
                                                    Name = "Styleguide-Multilingual",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Translation Patterns"),
                                                        ["sample"] =
                                                            new TextField("This text can be translated in en.yml")
                                                    }
                                                }
                                            ]
                                        }
                                    }
                                ]
                            }
                        }
                    ],
                    ["jss-styleguide-layout"] = [],
                    ["jss-styleguide-section"] = [],
                    ["jss-header"] = [],
                    ["jss-reuse-example"] = [],
                    ["jss-tabs"] = []
                }
            },
            Devices =
            [
                new Device
                {
                    Id = "fe5d7fdf-89c0-4d99-9aa3-b5fbd009c9f3",
                    LayoutId = "14030e9f-ce92-49c6-ad87-7d49b50e42ea",
                    Renderings =
                    [
                        new Rendering
                        {
                            Id = "885b8314-7d8c-4cbb-8000-01421ea8f406",
                            InstanceId = "43222d12-08c9-453b-ae96-d406ebb95126",
                            PlaceholderKey = "main"
                        },

                        new Rendering
                        {
                            Id = "ce4adcfb-7990-4980-83fb-a00c1e3673db",
                            InstanceId = "cf044ad9-0332-407a-abde-587214a2c808",
                            PlaceholderKey = "/main/centercolumn"
                        },

                        new Rendering
                        {
                            Id = "493b3a83-0fa7-4484-8fc9-4680991cf743",
                            InstanceId = "b343725a-3a93-446e-a9c8-3a2cbd3db489",
                            PlaceholderKey = "/main/centercolumn/content"
                        }
                    ]
                },

                new Device
                {
                    Id = "46d2f427-4ce5-4e1f-ba10-ef3636f43534",
                    LayoutId = "14030e9f-ce92-49c6-ad87-7d49b50e42ea",
                    Renderings =
                    [
                        new Rendering
                        {
                            Id = "493b3a83-0fa7-4484-8fc9-4680991cf743",
                            InstanceId = "a08c9132-dbd1-474f-a2ca-6ca26a4aa650",
                            PlaceholderKey = "content"
                        }
                    ]
                }
            ]
        }
    };

    public static SitecoreLayoutResponseContent WithVisitorIdentificationLayoutPlaceholder => new()
    {
        Sitecore = new SitecoreData
        {
            Context =
                new Context
                {
                    Language = TestConstants.Language,
                    IsEditing = false,
                    PageState = PageState.Normal,
                    Site = new Site { Name = "TestSiteName" }
                },
            Route = new Route
            {
                LayoutId = TestConstants.VisitorIdentificationPageLayoutId,
                DatabaseName = TestConstants.DatabaseName,
                DeviceId = "test-device-id",
                ItemId = TestConstants.TestItemId,
                ItemLanguage = "en",
                ItemVersion = 1,
                TemplateId = "test-template-id",
                TemplateName = "test-template-name",
                Name = "styleguide",
            }
        }
    };

    public static SitecoreLayoutResponseContent StyleGuideWithComponentParameters => new()
    {
        Sitecore = new SitecoreData
        {
            Context = new Context
            {
                Language = "en",
                IsEditing = false,
                PageState = PageState.Normal,
                Site = new Site
                {
                    Name = "JssDisconnectedLayoutService"
                }
            },
            Route = new Route
            {
                DatabaseName = "available-in-connected-mode",
                DeviceId = "available-in-connected-mode",
                ItemId = "available-in-connected-mode",
                ItemLanguage = "en",
                ItemVersion = 1,
                LayoutId = "{E02DDB9B-A062-5E50-924A-1940D7E053C2}",
                TemplateId = "available-in-connected-mode",
                TemplateName = "available-in-connected-mode",
                Name = "styleguide",
                Fields =
                {
                    ["pageTitle"] = new TextField("Styleguide | Sitecore JSS")
                },
                Placeholders =
                {
                    ["jss-main"] =
                    [
                        new Component
                        {
                            Id = "{E02DDB9B-A062-5E50-924A-1940D7E053CE}",
                            Name = "ContentBlock",
                            Fields =
                            {
                                ["heading"] = new TextField("JSS Styleguide"),
                                ["content"] = new RichTextField(
                                    "<p>This is a live set of examples of how to use JSS. For more information on using JSS, please see <a href=\"https://jss.sitecore.net\" target=\"_blank\" rel=\"noopener noreferrer\">the documentation</a>.</p>\n<p>The content and layout of this page is defined in <code>/data/routes/styleguide/en.yml</code></p>\n")
                            },
                            Parameters = new Dictionary<string, string>
                            {
                                { "CmpParam1", "Value1" },
                                { "CmpParam2", "Value2" },
                                { "CmpParam3", "Value3" }
                            }
                        }
                    ]
                }
            }
        }
    };

    public static SitecoreLayoutResponseContent StyleGuide1WithoutRoute => new()
    {
        Sitecore = new SitecoreData
        {
            Context = new Context
            {
                Language = "en",
                IsEditing = false,
                PageState = PageState.Normal,
                Site = new Site
                {
                    Name = "JssDisconnectedLayoutService"
                }
            },
            Route = null
        }
    };

    public static SitecoreLayoutResponseContent StyleGuide1WithContext => new()
    {
        ContextRawData = "{" +
                         "\"testClass1\": {" +
                         " \"testString\": \"stringExample\",    " +
                         "\"testInt\": 123,   " +
                         "\"testtime\": \"2020-12-08T13:09:44.1255842+02:00\" " +
                         " }," +
                         "\"testClass2\": {" +
                         "\"testString\": \"stringExample2\"," +
                         "\"testInt\": 1234 " +
                         "}, " +
                         "\"singleProperty\":\"SinglePropertyData\", " +
                         "\"pageEditing\": false," +
                         "\"site\": {" +
                         "\"name\": " +
                         "\"jss-react-sample\"" +
                         "}," +
                         "\"pageState\": \"normal\"," +
                         "\"language\": \"en\"}",
        Sitecore = new SitecoreData
        {
            Context = new Context
            {
                Language = "en",
                IsEditing = false,
                PageState = PageState.Normal,
                Site = new Site
                {
                    Name = "JssDisconnectedLayoutService"
                }
            },
            Route = new Route
            {
                DatabaseName = "test-database-name",
                DeviceId = "available-in-connected-mode",
                ItemId = "available-in-connected-mode",
                ItemLanguage = "en",
                ItemVersion = 1,
                LayoutId = "{E02DDB9B-A062-5E50-924A-1940D7E053C1}",
                TemplateId = "available-in-connected-mode",
                TemplateName = "available-in-connected-mode",
                Name = "styleguide",
                Fields =
                {
                    ["pageTitle"] = new TextField("Styleguide | Sitecore JSS")
                },
                Placeholders =
                {
                    ["jss-main"] =
                    [
                        new Component
                        {
                            Id = "{E02DDB9B-A062-5E50-924A-1940D7E053CD}",
                            Name = "HeaderBlock",
                            Fields =
                            {
                                ["heading1"] = new TextField("HeaderBlock - This is heading1"),
                                ["heading2"] = new TextField("HeaderBlock - This is heading2"),
                            }
                        },

                        new Component
                        {
                            Id = "{E02DDB9B-A062-5E50-924A-1940D7E053CE}",
                            Name = "ContentBlock",
                            Fields =
                            {
                                ["heading"] = new TextField("ContentBlock 1 - JSS Styleguide"),
                                ["content"] = new RichTextField(
                                    "<p>This is a live set of examples of how to use JSS. For more information on using JSS, please see <a href=\"https://jss.sitecore.net\" target=\"_blank\" rel=\"noopener noreferrer\">the documentation</a>.</p>\n<p>The content and layout of this page is defined in <code>/data/routes/styleguide/en.yml</code></p>\n")
                            }
                        },

                        new Component
                        {
                            Id = "{E02DDB9B-A062-5E50-924A-1940D7E053CF}",
                            Name = "ContentBlock",
                            Fields =
                            {
                                ["heading"] = new TextField("ContentBlock 2 - JSS Styleguide"),
                                ["content"] = new RichTextField(
                                    "<p>This is a live set of examples of how to use JSS. For more information on using JSS, please see <a href=\"https://jss.sitecore.net\" target=\"_blank\" rel=\"noopener noreferrer\">the documentation</a>.</p>\n<p>The content and layout of this page is defined in <code>/data/routes/styleguide/en.yml</code></p>\n")
                            }
                        },

                        new Component
                        {
                            Id = "{34A6553C-81DE-5CD3-989E-853F6CB6DF8C}",
                            Name = "Styleguide-Layout",
                            Placeholders =
                            {
                                ["jss-styleguide-layout"] =
                                [
                                    new Component
                                    {
                                        Id = "{B7C779DA-2B75-586C-B40D-081FCB864256}",
                                        Name = "Styleguide-Section",
                                        Fields =
                                        {
                                            ["heading"] = new TextField("Content Data")
                                        },
                                        Placeholders =
                                        {
                                            ["jss-styleguide-section"] =
                                            [
                                                new Component
                                                {
                                                    Id = "{63B0C99E-DAC7-5670-9D66-C26A78000EAE}",
                                                    Name = "Styleguide-FieldUsage-Text",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Single-Line Text"),
                                                        ["sample"] =
                                                            new RichTextField(
                                                                "This is a sample text field. <mark>HTML is encoded.</mark> In Sitecore, editors will see a <input type=\"text\">."),
                                                        ["sample2"] =
                                                            new RichTextField(
                                                                "This is another sample text field using rendering options. <mark>HTML supported with encode=false.</mark> Cannot edit because editable=false.")
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{F1EA3BB5-1175-5055-AB11-9C48BF69427A}",
                                                    Name = "Styleguide-FieldUsage-Text",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Multi-Line Text"),
                                                        ["description"] =
                                                            new RichTextField(
                                                                "<small>Multi-line text tells Sitecore to use a <code>textarea</code> for editing; consumption in JSS is the same as single-line text.</small>"),
                                                        ["sample"] =
                                                            new RichTextField(
                                                                "This is a sample multi-line text field. <mark>HTML is encoded.</mark> In Sitecore, editors will see a textarea."),
                                                        ["sample2"] =
                                                            new RichTextField(
                                                                "This is another sample multi-line text field using rendering options. <mark>HTML supported with encode=false.</mark>")
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{69CEBC00-446B-5141-AD1E-450B8D6EE0AD}",
                                                    Name = "Styleguide-FieldUsage-RichText",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Rich Text"),
                                                        ["sample"] =
                                                            new RichTextField(
                                                                "<p>This is a sample rich text field. <mark>HTML is always supported.</mark> In Sitecore, editors will see a WYSIWYG editor for these fields.</p>"),
                                                        ["sample2"] = new RichTextField(
                                                            "<p>Another sample rich text field, using options. Keep markup entered in rich text fields as simple as possible - ideally bare tags only (no classes). Adding a wrapping class can help with styling within rich text blocks.</p>\n<marquee>But you can use any valid HTML in a rich text field!</marquee>\n")
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{5630C0E6-0430-5F6A-AF9E-2D09D600A386}",
                                                    Name = "Styleguide-FieldUsage-Image",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Image"),
                                                        ["sample1"] = new ImageField(new Image
                                                        {
                                                            Src = "/data/media/img/sc_logo.png",
                                                            Alt = "Sitecore Logo"
                                                        }),
                                                        ["sample2"] = new ImageField(new Image
                                                        {
                                                            Src = "/data/media/img/jss_logo.png",
                                                            Alt = "Sitecore JSS Logo"
                                                        })
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{BAD43EF7-8940-504D-A09B-976C17A9A30C}",
                                                    Name = "Styleguide-FieldUsage-File",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("File"),
                                                        ["description"] = new RichTextField(
                                                            "<small>Note: Sitecore does not support inline editing of File fields. The value must be edited in Experience Editor by using the edit rendering fields button (looks like a pencil) with the whole component selected.</small>\n"),
                                                        ["file"] = new FileField(new File
                                                        {
                                                            Src = "/data/media/files/jss.pdf",
                                                            Title = "Example File",
                                                            Description =
                                                                "This data will be added to the Sitecore Media Library on import"
                                                        })
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{FF90D4BD-E50D-5BBF-9213-D25968C9AE75}",
                                                    Name = "Styleguide-FieldUsage-Number",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Number"),
                                                        ["description"] =
                                                            new RichTextField(
                                                                "<small>Number tells Sitecore to use a number entry for editing.</small>"),
                                                        ["sample"] = new NumberField(1.21),
                                                        ["sample2"] = new NumberField(71),
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{B5C1C74A-A81D-59B2-85D8-09BC109B1F70}",
                                                    Name = "Styleguide-FieldUsage-Checkbox",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Checkbox"),
                                                        ["description"] = new RichTextField(
                                                            "<small>Note: Sitecore does not support inline editing of Checkbox fields. The value must be edited in Experience Editor by using the edit rendering fields button (looks like a pencil) with the whole component selected.</small>\n"),
                                                        ["checkbox"] = new CheckboxField(true),
                                                        ["checkbox2"] = new CheckboxField(false),
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{F166A7D6-9EC8-5C53-B825-33405DB7F575}",
                                                    Name = "Styleguide-FieldUsage-Date",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Date"),
                                                        ["description"] = new RichTextField(
                                                            "<p><small>Both <code>Date</code> and <code>DateTime</code> field types are available. Choosing <code>DateTime</code> will make Sitecore show editing UI for time; both types store complete date and time values internally. Date values in JSS are formatted using <a href=\"https://en.wikipedia.org/wiki/ISO_8601#Combined_date_and_time_representations\" target=\"_blank\">ISO 8601 formatted strings</a>, for example <code>2012-04-23T18:25:43.511Z</code>.</small></p>\n<div class=\"alert alert-warning\"><small>Note: this is a JavaScript date format (e.g. <code>new Date().toISOString()</code>), and is different from how Sitecore stores date field values internally. Sitecore-formatted dates will not work.</small></div>\n"),
                                                        ["date"] = new DateField(
                                                            DateTime.Parse(
                                                                "2012-05-04T00:00:00Z",
                                                                CultureInfo.InvariantCulture)),
                                                        ["dateTime"] =
                                                            new DateField(DateTime.Parse(
                                                                "2018-03-14T15:00:00Z",
                                                                CultureInfo.InvariantCulture)),
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{56A9562A-6813-579B-8ED2-FDDAB1BFD3D2}",
                                                    Name = "Styleguide-FieldUsage-Link",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("General Link"),
                                                        ["description"] =
                                                            new RichTextField(
                                                                "<p>A <em>General Link</em> is a field that represents an <code>&lt;a&gt;</code> tag.</p>"),
                                                        ["externalLink"] = new HyperLinkField(new HyperLink
                                                        {
                                                            Href = "https://www.sitecore.com",
                                                            Text = "Link to Sitecore"
                                                        }),
                                                        ["internalLink"] = new HyperLinkField(new HyperLink
                                                            { Href = "/" }),
                                                        ["mediaLink"] = new HyperLinkField(new HyperLink
                                                        {
                                                            Href = "/data/media/files/jss.pdf",
                                                            Text = "Link to PDF"
                                                        }),
                                                        ["emailLink"] = new HyperLinkField(new HyperLink
                                                        {
                                                            Href = "mailto:foo@bar.com", Text = "Send an Email"
                                                        }),
                                                        ["paramsLink"] = new HyperLinkField(new HyperLink
                                                        {
                                                            Href = "https://dev.sitecore.net",
                                                            Text = "Sitecore Dev Site",
                                                            Target = "_blank",
                                                            Class = "font-weight-bold",
                                                            Title = "<a> title attribute"
                                                        })
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{A44AD1F8-0582-5248-9DF9-52429193A68B}",
                                                    Name = "Styleguide-FieldUsage-ItemLink",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Item Link"),
                                                        ["description"] = new RichTextField(
                                                            "<p>\n  <small>\n    Item Links are a way to reference another content item to use data from it.\n    Referenced items may be shared.\n    To reference multiple content items, use a <em>Content List</em> field.<br />\n    <strong>Note:</strong> Sitecore does not support inline editing of Item Link fields. The value must be edited in Experience Editor by using the edit rendering fields button (looks like a pencil) with the whole component selected.\n  </small>\n</p>\n"),
                                                        ["sharedItemLink"] = new ItemLinkField
                                                        {
                                                            Fields =
                                                            {
                                                                ["textField"] =
                                                                    new TextField(
                                                                        "ItemLink Demo (Shared) Item 1 Text Field")
                                                            }
                                                        },
                                                        ["localItemLink"] = new ItemLinkField
                                                        {
                                                            Fields =
                                                            {
                                                                ["textField"] =
                                                                    new TextField("Referenced item textField")
                                                            }
                                                        },
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{2F609D40-8AD9-540E-901E-23AA2600F3EB}",
                                                    Name = "Styleguide-FieldUsage-ContentList",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Content List"),
                                                        ["description"] = new RichTextField(
                                                            "<p>\n  <small>\n    Content Lists are a way to reference zero or more other content items.\n    Referenced items may be shared.\n    To reference a single content item, use an <em>Item Link</em> field.<br />\n    <strong>Note:</strong> Sitecore does not support inline editing of Content List fields. The value must be edited in Experience Editor by using the edit rendering fields button (looks like a pencil) with the whole component selected.\n  </small>\n</p>\n"),
                                                        ["sharedContentList"] = new ContentListField
                                                        {
                                                            new()
                                                            {
                                                                Fields =
                                                                {
                                                                    ["textField"] =
                                                                        new TextField(
                                                                            "ContentList Demo (Shared) Item 1 Text Field")
                                                                }
                                                            },
                                                            new()
                                                            {
                                                                Fields =
                                                                {
                                                                    ["textField"] =
                                                                        new TextField(
                                                                            "ContentList Demo (Shared) Item 2 Text Field")
                                                                }
                                                            }
                                                        },
                                                        ["localContentList"] = new ContentListField
                                                        {
                                                            new()
                                                            {
                                                                Fields =
                                                                {
                                                                    ["textField"] =
                                                                        new TextField("Hello World Item 1")
                                                                }
                                                            },
                                                            new()
                                                            {
                                                                Fields =
                                                                {
                                                                    ["textField"] =
                                                                        new TextField("Hello World Item 2")
                                                                }
                                                            }
                                                        },
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{352ED63D-796A-5523-89F5-9A991DDA4A8F}",
                                                    Name = "Styleguide-FieldUsage-Custom",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Custom Fields"),
                                                        ["description"] = new RichTextField(
                                                            "<p>\n  <small>\n    Any Sitecore field type can be consumed by JSS.\n    In this sample we consume the <em>Integer</em> field type.<br />\n    <strong>Note:</strong> For field types with complex data, custom <code>FieldSerializer</code>s may need to be implemented on the Sitecore side.\n  </small>\n</p>\n"),
                                                        ["customIntField"] = new Field<int> { Value = 31337 }
                                                    }
                                                }
                                            ]
                                        }
                                    },

                                    new Component
                                    {
                                        Id = "{7DE41A1A-24E4-5963-8206-3BB0B7D9DD69}",
                                        Name = "Styleguide-Section",
                                        Fields =
                                        {
                                            ["heading"] = new TextField("Layout Patterns")
                                        },
                                        Placeholders =
                                        {
                                            ["jss-header"] =
                                            [
                                                new Component
                                                {
                                                    Id = "{1A6FCB1C-E97B-5325-8E4E-6E13997A4A1A}",
                                                    Name = "Styleguide-Layout-Reuse"
                                                }
                                            ],
                                            ["jss-styleguide-section"] =
                                            [
                                                new Component
                                                {
                                                    Id = "{3A5D9C50-D8C1-5A12-8DA8-5D56C2A5A69A}",
                                                    Name = "Styleguide-Layout-Reuse",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Reusing Content"),
                                                        ["description"] = new RichTextField(
                                                            "<p>JSS provides powerful options to reuse content, whether it's sharing a common piece of text across pages or sketching out a site with repeating <em>lorem ipsum</em> content.</p>")
                                                    },
                                                    Placeholders =
                                                    {
                                                        ["jss-reuse-example"] =
                                                        [
                                                            new Component
                                                            {
                                                                Id = "{AA328B8A-D6E1-5B37-8143-250D2E93D6B8}",
                                                                Name = "ContentBlock",
                                                                Fields =
                                                                {
                                                                    ["content"] = new RichTextField(
                                                                        "<p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Quisque felis mauris, pretium id neque vitae, vulputate pellentesque tortor. Mauris hendrerit dolor et ipsum lobortis bibendum non finibus neque. Morbi volutpat aliquam magna id posuere. Duis commodo cursus dui, nec interdum velit congue nec. Aliquam erat volutpat. Aliquam facilisis, sapien quis fringilla tincidunt, magna nulla feugiat neque, a consectetur arcu orci eu augue.</p>")
                                                                }
                                                            },

                                                            new Component
                                                            {
                                                                Id = "{C4330D34-623C-556C-BF4C-97C93D40FB1E}",
                                                                Name = "ContentBlock",
                                                                Fields =
                                                                {
                                                                    ["content"] = new RichTextField(
                                                                        "<p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Quisque felis mauris, pretium id neque vitae, vulputate pellentesque tortor. Mauris hendrerit dolor et ipsum lobortis bibendum non finibus neque. Morbi volutpat aliquam magna id posuere. Duis commodo cursus dui, nec interdum velit congue nec. Aliquam erat volutpat. Aliquam facilisis, sapien quis fringilla tincidunt, magna nulla feugiat neque, a consectetur arcu orci eu augue.</p>")
                                                                }
                                                            },

                                                            new Component
                                                            {
                                                                Id = "{A42D8B1C-193D-5627-9130-F7F7F87617F1}",
                                                                Name = "ContentBlock",
                                                                Fields =
                                                                {
                                                                    ["content"] = new RichTextField(
                                                                        "<p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Quisque felis mauris, pretium id neque vitae, vulputate pellentesque tortor. Mauris hendrerit dolor et ipsum lobortis bibendum non finibus neque. Morbi volutpat aliquam magna id posuere. Duis commodo cursus dui, nec interdum velit congue nec. Aliquam erat volutpat. Aliquam facilisis, sapien quis fringilla tincidunt, magna nulla feugiat neque, a consectetur arcu orci eu augue.</p>")
                                                                }
                                                            },

                                                            new Component
                                                            {
                                                                Id = "{0F4CB47A-979E-5139-B50B-A8E40C73C236}",
                                                                Name = "ContentBlock",
                                                                Fields =
                                                                {
                                                                    ["content"] =
                                                                        new RichTextField(
                                                                            "<p>Mix and match reused and local content. Check out <code>/data/routes/styleguide/en.yml</code> to see how.</p>")
                                                                }
                                                            }
                                                        ]
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{538E4831-F157-50BB-AC74-277FCAC9FDDB}",
                                                    Name = "Styleguide-Layout-Tabs",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Tabs"),
                                                        ["description"] =
                                                            new RichTextField(
                                                                "<p>Creating hierarchical components like tabs is made simpler in JSS because it's easy to introspect the layout structure.</p>")
                                                    },
                                                    Placeholders =
                                                    {
                                                        ["jss-tabs"] =
                                                        [
                                                            new Component
                                                            {
                                                                Id = "{7ECB2ED2-AC9B-58D1-8365-10CA74824AF7}",
                                                                Name = "Styleguide-Layout-Tabs-Tab",
                                                                Fields =
                                                                {
                                                                    ["title"] = new TextField("Tab 1"),
                                                                    ["content"] =
                                                                        new RichTextField(
                                                                            "<p>Tab 1 contents!</p>")
                                                                }
                                                            },

                                                            new Component
                                                            {
                                                                Id = "{AFD64900-0A61-50EB-A674-A7A884E0D496}",
                                                                Name = "Styleguide-Layout-Tabs-Tab",
                                                                Fields =
                                                                {
                                                                    ["title"] = new TextField("Tab 2"),
                                                                    ["content"] =
                                                                        new RichTextField(
                                                                            "<p>Tab 2 contents!</p>")
                                                                }
                                                            },

                                                            new Component
                                                            {
                                                                Id = "{44C12983-3A84-5462-84C0-6CA1430050C8}",
                                                                Name = "Styleguide-Layout-Tabs-Tab",
                                                                Fields =
                                                                {
                                                                    ["title"] = new TextField("Tab 3"),
                                                                    ["content"] =
                                                                        new RichTextField(
                                                                            "<p>Tab 3 contents!</p>")
                                                                }
                                                            }
                                                        ]
                                                    }
                                                }
                                            ]
                                        }
                                    },

                                    new Component
                                    {
                                        Id = "{2D806C25-DD46-51E3-93DE-63CF9035122C}",
                                        Name = "Styleguide-Section",
                                        Fields =
                                        {
                                            ["heading"] = new TextField("Sitecore Patterns")
                                        },
                                        Placeholders =
                                        {
                                            ["jss-styleguide-section"] =
                                            [
                                                new Component
                                                {
                                                    Id = "{471FA16A-BB82-5C42-9C95-E7EAB1E3BD30}",
                                                    Name = "Styleguide-SitecoreContext",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Sitecore Context"),
                                                        ["description"] = new RichTextField(
                                                            "<p><small>The Sitecore Context contains route-level data about the current context - for example, <code>pageState</code> enables conditionally executing code based on whether Sitecore is in Experience Editor or not.</small></p>")
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{21F21053-8F8A-5436-BC79-E674E246A2FC}",
                                                    Name = "Styleguide-RouteFields",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Route-level Fields"),
                                                        ["description"] = new RichTextField(
                                                            "<p><small>Route-level content fields are defined on the <em>route</em> instead of on a <em>component</em>. This allows multiple components to share the field data on the same route - and querying is much easier on route level fields, making <em>custom route types</em> ideal for filterable/queryable data such as articles.</small></p>")
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{A0A66136-C21F-52E8-A2EA-F04DCFA6A027}",
                                                    Name = "Styleguide-ComponentParams",
                                                    Parameters =
                                                    {
                                                        ["cssClass"] = "alert alert-success",
                                                        ["columns"] = "5",
                                                        ["useCallToAction"] = "true"
                                                    },
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Component Params"),
                                                        ["description"] = new RichTextField(
                                                            "<p><small>Component params (also called Rendering Parameters) allow storing non-content parameters for a component. These params should be used for more technical options such as CSS class names or structural settings.</small></p>")
                                                    }
                                                },

                                                new Component
                                                {
                                                    Id = "{7F765FCB-3B10-58FD-8AA7-B346EF38C9BB}",
                                                    Name = "Styleguide-Tracking",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Tracking"),
                                                        ["description"] =
                                                            new RichTextField(
                                                                "<p><small>JSS supports tracking Sitecore analytics events from within apps. Give it a try with this handy interactive demo.</small></p>")
                                                    }
                                                }
                                            ]
                                        }
                                    },

                                    new Component
                                    {
                                        Id = "{66AF8F03-0B52-5425-A6AF-6FB54F2D64D9}",
                                        Name = "Styleguide-Section",
                                        Fields =
                                        {
                                            ["heading"] = new TextField("Multilingual Patterns")
                                        },
                                        Placeholders =
                                        {
                                            ["jss-styleguide-section"] =
                                            [
                                                new Component
                                                {
                                                    Id = "{CF1B5D2B-C949-56E7-9594-66AFACEACA9D}",
                                                    Name = "Styleguide-Multilingual",
                                                    Fields =
                                                    {
                                                        ["heading"] = new TextField("Translation Patterns"),
                                                        ["sample"] =
                                                            new TextField(
                                                                "This text can be translated in en.yml")
                                                    }
                                                }
                                            ]
                                        }
                                    }
                                ]
                            }
                        }

                    ],
                    ["jss-styleguide-layout"] = [],
                    ["jss-styleguide-section"] = [],
                    ["jss-header"] = [],
                    ["jss-reuse-example"] = [],
                    ["jss-tabs"] = []
                }
            }
        }
    };

    public static SitecoreLayoutResponseContent WithNestedPlaceholder => new()
    {
        Sitecore = new SitecoreData
        {
            Context = new Context
            {
                Language = TestConstants.Language,
                IsEditing = false,
                PageState = PageState.Normal,
                Site = new Site
                {
                    Name = "JssDisconnectedLayoutService"
                }
            },
            Route = new Route
            {
                LayoutId = TestConstants.NestedPlaceholderPageLayoutId,

                DatabaseName = TestConstants.DatabaseName,
                DeviceId = "test-device-id",
                ItemId = TestConstants.TestItemId,
                ItemLanguage = "en",
                ItemVersion = 1,
                TemplateId = "test-template-id",
                TemplateName = "test-template-name",
                Name = "styleguide",
                Fields =
                {
                    ["pageTitle"] = new TextField(TestConstants.PageTitle),
                    ["searchKeywords"] = new TextField(TestConstants.SearchKeywords)
                },
                Placeholders =
                {
                    ["placeholder-1"] =
                    [
                        new Component
                        {
                            Id = "{34A6553C-81DE-5CD3-989E-853F6CB6DF8C}",
                            Name = "Component-1",
                            Placeholders =
                            {
                                ["placeholder-2"] =
                                [
                                    new Component
                                    {
                                        Id = "{B7C779DA-2B75-586C-B40D-081FCB864256}",
                                        Name = "Component-2",
                                        Fields =
                                        {
                                            ["TestField"] = new RichTextField(TestConstants.RichTextFieldValue1)
                                        }
                                    }
                                ]
                            }
                        },

                        new Component
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = "Component-3",
                            Fields =
                            {
                                ["TestField"] = new TextField(TestConstants.TestFieldValue),
                                ["EmptyField"] = new TextField(string.Empty),
                                ["NullValueField"] = new TextField(null!),
                                ["MultiLineField"] = new TextField(TestConstants.TestMultilineFieldValue),
                                ["EncodedField"] = new TextField(TestConstants.RichTextFieldValue1)
                            }
                        },

                        new Component
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = "Component-4",
                            Fields =
                            {
                                ["Date"] = new DateField(DateTime.Parse("12.12.19", CultureInfo.InvariantCulture)),
                                ["RichTextField1"] = new RichTextField(TestConstants.RichTextFieldValue1),
                                ["RichTextField2"] = new RichTextField(TestConstants.RichTextFieldValue2),
                                ["EmptyField"] = new RichTextField(string.Empty),
                                ["NullValueField"] = new RichTextField(null!),
                                ["TestField"] = new TextField(TestConstants.TestFieldValue)
                            }
                        },

                        new Component
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = "Component-5",
                            Fields =
                            {
                                ["TestField"] = new TextField(TestConstants.TestFieldValue),
                                ["EmptyField"] = new TextField(string.Empty),
                                ["NullValueField"] = new TextField(null!),
                                ["MultiLineField"] = new TextField(TestConstants.TestMultilineFieldValue),
                                ["Date"] = new DateField(DateTime.Parse("12.12.19", CultureInfo.InvariantCulture))
                            }
                        },

                        new Component
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = "Component-6",
                            Fields =
                            {
                                ["TestField"] = new TextField(TestConstants.TestFieldValue + " from Component-6")
                            }
                        },

                        new Component
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = "Complex-Component",
                            Fields =
                            {
                                ["Header"] = new TextField(TestConstants.PageTitle),
                                ["Content"] = new RichTextField(TestConstants.RichTextFieldValue1),
                                ["Header2"] = new TextField(TestConstants.TestFieldValue),
                                ["CustomField"] = new Models.CustomFieldType(TestConstants.TestFieldValue, "custom")
                            },
                            Parameters = new Dictionary<string, string>
                            {
                                { "ParamName", "ParamName-Value" }
                            }
                        },

                        new Component
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = "Custom-Model-Context-Component",
                        },

                        new Component
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = "Component-With-All-Field-Types",
                            Fields =
                            {
                                ["TextField"] = new TextField(TestConstants.TestFieldValue),
                                ["MultiLineField"] = new TextField(TestConstants.TestMultilineFieldValue),
                                ["RichTextField1"] = new RichTextField(TestConstants.RichTextFieldValue1),
                                ["RichTextField2"] = new RichTextField(TestConstants.RichTextFieldValue2),
                                ["LinkField"] = new HyperLinkField(new HyperLink
                                {
                                    Href = "/", Text = "Sample Link", Class = "sample", Target = "_blank",
                                    Title = "title"
                                }),
                                ["ImageField"] = new ImageField(new Image { Alt = "sample", Src = "sample.png" }),
                                ["MediaLibraryItemImageField"] = new ImageField(new Image
                                {
                                    Alt = "sample",
                                    Src =
                                        "https://cdinstance/en/-/media/094AED0302E7486880CB19926661FB77.ashx?h=51&w=204"
                                }),
                                ["DateField"] = new DateField(DateTime.Parse(
                                    "2012-05-04T00:00:00Z",
                                    CultureInfo.InvariantCulture)),
                                ["NumberField"] = new NumberField(9.99)
                            }
                        },

                        new Component
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = "Component-With-Links",
                            Fields =
                            {
                                ["internalLink"] = new HyperLinkField(new HyperLink
                                    { Href = "/", Text = "This is field text" }),
                                ["paramsLink"] = new HyperLinkField(new HyperLink
                                {
                                    Href = "https://dev.sitecore.net",
                                    Text = "Sitecore Dev Site",
                                    Target = "_blank",
                                    Class = "font-weight-bold",
                                    Title = "title attribute"
                                }),
                                ["text"] = new TextField(TestConstants.TestFieldValue),
                            }
                        },

                        new Component
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = "Component-With-Files",
                            Fields =
                            {
                                ["fileLink"] = new FileField(new File
                                {
                                    MimeType = "application/pdf",
                                    Description = "Download link description",
                                    Src = "/doc.pdf",
                                    Title = "Download link text"
                                })
                            }
                        },

                        new Component
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = "Component-With-Images",
                            Fields =
                            {
                                ["FirstImage"] = new ImageField(new Image { Alt = "sample", Src = "sample.png" }),
                                ["SecondImage"] = new ImageField(new Image
                                    { Alt = "second", Src = "site/second.png" }),
                                ["Heading"] = new TextField(TestConstants.TestFieldValue),
                            }
                        },

                        new Component
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = "Component-With-Dates",
                            Fields =
                            {
                                ["date"] = new DateField(TestConstants.DateTimeValue),
                                ["text"] = new TextField(TestConstants.TestFieldValue)
                            }
                        },

                        new Component
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = "Component-With-Number",
                            Fields =
                            {
                                ["number"] = new NumberField(1.21),
                                ["text"] = new TextField(TestConstants.TestFieldValue)
                            }
                        }
                    ]
                }
            }
        }
    };

    public static SitecoreLayoutResponseContent WithMissingData => new()
    {
        Sitecore = new SitecoreData
        {
            Context = new Context
            {
                // missing context properties
            },
            Route = new Route
            {
                LayoutId = TestConstants.NestedPlaceholderPageLayoutId,

                // missing route properties and fields
                Placeholders =
                {
                    ["placeholder-1"] =
                    [
                        new Component
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = "Component-With-Missing-Data",

                            // missing component fields
                            Placeholders =
                            {
                                ["placeholder-2"] =
                                [
                                    new Component
                                    {
                                        // Id = "{B7C779DA-2B75-586C-B40D-081FCB864256}",
                                        Name = "Component-Without-Id",
                                        Fields =
                                        {
                                            ["TestField"] = new RichTextField(TestConstants.RichTextFieldValue1)
                                        }
                                    }
                                ]
                            }
                        }
                    ]
                }
            }
        }
    };

    public static SitecoreLayoutResponseContent WithMissingComponent => new()
    {
        Sitecore = new SitecoreData
        {
            Context = new Context(),
            Route = new Route
            {
                LayoutId = TestConstants.MissingComponentPageLayoutId,
                Placeholders =
                {
                    ["placeholder-with-missing-component"] =
                    [
                        new Component
                        {
                            Id = "{3C80CB13-D1DA-4946-8BC2-72ABF94D15E5}",
                            Name = "Component-3",
                        }
                    ]
                }
            }
        }
    };

    public static SitecoreLayoutResponseContent EditablePage => new()
    {
        Sitecore = new SitecoreData
        {
            Context = new Context
            {
                Language = TestConstants.Language,
                IsEditing = true,
                PageState = PageState.Edit,
                Site = new Site
                {
                    Name = "sample"
                }
            },
            Route = new Route
            {
                LayoutId = TestConstants.NestedPlaceholderPageLayoutId,

                DatabaseName = TestConstants.DatabaseName,
                DeviceId = "test-device-id",
                ItemId = TestConstants.TestItemId,
                ItemLanguage = "en",
                ItemVersion = 1,
                TemplateId = "test-template-id",
                TemplateName = "test-template-name",
                Name = "styleguide",
                Fields =
                {
                    ["pageTitle"] = new TextField
                    {
                        Value = TestConstants.PageTitle,
                        EditableMarkup = "<input id='fld_616E2DAABB71511782B1B360EF600213_23D2153F15A75FA08560F4A2192F0B03_en_1_76f157a7b0ed4bdf8dbfbccde805c355_282' class='scFieldValue' name='fld_616E2DAABB71511782B1B360EF600213_23D2153F15A75FA08560F4A2192F0B03_en_1_76f157a7b0ed4bdf8dbfbccde805c355_282' type='hidden' value=\"Styleguide | Sitecore JSS\" /><span class=\"scChromeData\">{\"commands\":[{\"click\":\"chrome:common:edititem({command:\\\"webedit:open\\\"})\",\"header\":\"Edit the related item\",\"icon\":\"/temp/iconcache/office/16x16/cubes.png\",\"disabledIcon\":\"/temp/cubes_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the related item in the Content Editor.\",\"type\":\"common\"},{\"click\":\"chrome:rendering:personalize({command:\\\"webedit:personalize\\\"})\",\"header\":\"Personalize\",\"icon\":\"/temp/iconcache/office/16x16/users_family.png\",\"disabledIcon\":\"/temp/users_family_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Create or edit personalization for this component.\",\"type\":\"sticky\"}],\"contextItemUri\":\"sitecore://master/{616E2DAA-BB71-5117-82B1-B360EF600213}?lang=en&ver=1\",\"custom\":{},\"displayName\":\"Page Title\",\"expandedDisplayName\":null}</span><span id=\"fld_616E2DAABB71511782B1B360EF600213_23D2153F15A75FA08560F4A2192F0B03_en_1_76f157a7b0ed4bdf8dbfbccde805c355_282_edit\" sc_parameters=\"prevent-line-break=true\" contenteditable=\"true\" class=\"scWebEditInput\" scFieldType=\"single-line text\" scDefaultText=\"[No text in field]\">Styleguide | Sitecore JSS</span>"
                    }
                },
                Placeholders =
                {
                    ["placeholder-1"] =
                    [
                        new EditableChrome
                        {
                            Name = "code",
                            Type = "text/sitecore",
                            Content =
                                "{\"commands\":[{\"click\":\"chrome:placeholder:addControl\",\"header\":\"Add to here\",\"icon\":\"/temp/iconcache/office/16x16/add.png\",\"disabledIcon\":\"/temp/add_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Add a new rendering to the '{0}' placeholder.\",\"type\":\"\"},{\"click\":\"chrome:placeholder:editSettings\",\"header\":\"\",\"icon\":\"/temp/iconcache/office/16x16/window_gear.png\",\"disabledIcon\":\"/temp/window_gear_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the placeholder settings.\",\"type\":\"\"}],\"contextItemUri\":\"sitecore://master/{616E2DAA-BB71-5117-82B1-B360EF600213}?lang=en&ver=1\",\"custom\":{\"allowedRenderings\":[\"1DE91AADC1465D8983FA31A8FD63EBB3\",\"4E3C94B3A9D25478B7548D87283D8AA6\",\"26D9B310A5365D6B975442DB6BE1D381\",\"27EA18D87B6456108919947077956819\"],\"editable\":\"true\"},\"displayName\":\"Main\",\"expandedDisplayName\":null}",
                            Attributes = new Dictionary<string, string>
                            {
                                { "type", "text/sitecore" },
                                { "chrometype", "placeholder" },
                                { "kind", "open" },
                                { "id", "placeholder_1" },
                                { "key", "placeholder-1" },
                                { "class", "scpm" },
                                { "data-selectable", "true" }
                            }
                        },

                        new EditableChrome
                        {
                            Name = "code",
                            Type = "text/sitecore",
                            Content =
                                "{\"commands\":[{\"click\":\"javascript:Sitecore.PageModes.PageEditor.postRequest('webedit:fieldeditor(command={2F7D34E2-EB93-47ED-8177-B1F429A9FCD7},fields=heading|content,id={585596CA-7903-500B-8DF2-0357DD6E3FAC})',null,false)\",\"header\":\"Edit Fields\",\"icon\":\"/temp/iconcache/office/16x16/pencil.png\",\"disabledIcon\":\"/temp/pencil_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"\",\"type\":null},{\"click\":\"chrome:dummy\",\"header\":\"Separator\",\"icon\":\"\",\"disabledIcon\":\"\",\"isDivider\":false,\"tooltip\":null,\"type\":\"separator\"},{\"click\":\"chrome:rendering:sort\",\"header\":\"Change position\",\"icon\":\"/temp/iconcache/office/16x16/document_size.png\",\"disabledIcon\":\"/temp/document_size_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Move component.\",\"type\":\"\"},{\"click\":\"javascript:Sitecore.PageModes.PageEditor.postRequest('webedit:componentoptions(referenceId={E02DDB9B-A062-5E50-924A-1940D7E053CE},renderingId={1DE91AAD-C146-5D89-83FA-31A8FD63EBB3},id={585596CA-7903-500B-8DF2-0357DD6E3FAC})',null,false)\",\"header\":\"Edit Experience Editor Options\",\"icon\":\"/temp/iconcache/office/16x16/clipboard_check_edit.png\",\"disabledIcon\":\"/temp/clipboard_check_edit_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the Experience Editor options for the component.\",\"type\":\"common\"},{\"click\":\"chrome:rendering:properties\",\"header\":\"Edit component properties\",\"icon\":\"/temp/iconcache/office/16x16/elements_branch.png\",\"disabledIcon\":\"/temp/elements_branch_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the properties for the component.\",\"type\":\"common\"},{\"click\":\"javascript:Sitecore.PageModes.PageEditor.postRequest('webedit:setdatasource(referenceId={E02DDB9B-A062-5E50-924A-1940D7E053CE},renderingId={1DE91AAD-C146-5D89-83FA-31A8FD63EBB3},id={585596CA-7903-500B-8DF2-0357DD6E3FAC})',null,false)\",\"header\":\"dsHeaderParameter\",\"icon\":\"/temp/iconcache/office/16x16/data.png\",\"disabledIcon\":\"/temp/data_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"dsTooltipParameter\",\"type\":\"datasourcesmenu\"},{\"click\":\"chrome:rendering:personalize({command:\\\"webedit:personalize\\\"})\",\"header\":\"Personalize\",\"icon\":\"/temp/iconcache/office/16x16/users_family.png\",\"disabledIcon\":\"/temp/users_family_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Create or edit personalization for this component.\",\"type\":\"sticky\"},{\"click\":\"chrome:common:edititem({command:\\\"webedit:open\\\"})\",\"header\":\"Edit the related item\",\"icon\":\"/temp/iconcache/office/16x16/cubes.png\",\"disabledIcon\":\"/temp/cubes_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the related item in the Content Editor.\",\"type\":\"datasourcesmenu\"},{\"click\":\"chrome:rendering:delete\",\"header\":\"Delete\",\"icon\":\"/temp/iconcache/office/16x16/delete.png\",\"disabledIcon\":\"/temp/delete_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Remove component.\",\"type\":\"sticky\"}],\"contextItemUri\":\"sitecore://master/{585596CA-7903-500B-8DF2-0357DD6E3FAC}?lang=en&ver=1\",\"custom\":{\"renderingID\":\"1DE91AADC1465D8983FA31A8FD63EBB3\",\"editable\":\"true\"},\"displayName\":\"Content Block\",\"expandedDisplayName\":null}",
                            Attributes = new Dictionary<string, string>
                            {
                                { "type", "text/sitecore" },
                                { "chrometype", "rendering" },
                                { "kind", "open" },
                                { "id", "r_E02DDB9BA0625E50924A1940D7E053CE" },
                                { "hintname", "Component 1" },
                                { "class", "scpm" },
                                { "data-selectable", "true" }
                            }
                        },

                        new Component
                        {
                            Id = "{34A6553C-81DE-5CD3-989E-853F6CB6DF8C}",
                            Name = "Component-1",
                            Placeholders =
                            {
                                ["placeholder-2"] =
                                [
                                    new EditableChrome
                                    {
                                        Name = "code",
                                        Type = "text/sitecore",
                                        Content =
                                            "{\"commands\":[{\"click\":\"chrome:placeholder:addControl\",\"header\":\"Add to here\",\"icon\":\"/temp/iconcache/office/16x16/add.png\",\"disabledIcon\":\"/temp/add_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Add a new rendering to the '{0}' placeholder.\",\"type\":\"\"},{\"click\":\"chrome:placeholder:editSettings\",\"header\":\"\",\"icon\":\"/temp/iconcache/office/16x16/window_gear.png\",\"disabledIcon\":\"/temp/window_gear_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the placeholder settings.\",\"type\":\"\"}],\"contextItemUri\":\"sitecore://master/{616E2DAA-BB71-5117-82B1-B360EF600213}?lang=en&ver=1\",\"custom\":{\"allowedRenderings\":[\"1DE91AADC1465D8983FA31A8FD63EBB3\",\"4E3C94B3A9D25478B7548D87283D8AA6\",\"26D9B310A5365D6B975442DB6BE1D381\",\"27EA18D87B6456108919947077956819\"],\"editable\":\"true\"},\"displayName\":\"Main\",\"expandedDisplayName\":null}",
                                        Attributes = new Dictionary<string, string>
                                        {
                                            { "type", "text/sitecore" },
                                            { "chrometype", "placeholder" },
                                            { "kind", "open" },
                                            {
                                                "id",
                                                "_placeholder_1_placeholder_2__34A6553C_81DE_5CD3_989E_853F6CB6DF8C__0"
                                            },
                                            {
                                                "key",
                                                "/placeholder-1/placeholder-2-{34A6553C-81DE-5CD3-989E-853F6CB6DF8C}-0"
                                            },
                                            { "class", "scpm" },
                                            { "data-selectable", "true" }
                                        }
                                    },

                                    new EditableChrome
                                    {
                                        Name = "code",
                                        Type = "text/sitecore",
                                        Content =
                                            "{\"commands\":[{\"click\":\"javascript:Sitecore.PageModes.PageEditor.postRequest('webedit:fieldeditor(command={2F7D34E2-EB93-47ED-8177-B1F429A9FCD7},fields=heading|content,id={585596CA-7903-500B-8DF2-0357DD6E3FAC})',null,false)\",\"header\":\"Edit Fields\",\"icon\":\"/temp/iconcache/office/16x16/pencil.png\",\"disabledIcon\":\"/temp/pencil_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"\",\"type\":null},{\"click\":\"chrome:dummy\",\"header\":\"Separator\",\"icon\":\"\",\"disabledIcon\":\"\",\"isDivider\":false,\"tooltip\":null,\"type\":\"separator\"},{\"click\":\"chrome:rendering:sort\",\"header\":\"Change position\",\"icon\":\"/temp/iconcache/office/16x16/document_size.png\",\"disabledIcon\":\"/temp/document_size_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Move component.\",\"type\":\"\"},{\"click\":\"javascript:Sitecore.PageModes.PageEditor.postRequest('webedit:componentoptions(referenceId={E02DDB9B-A062-5E50-924A-1940D7E053CE},renderingId={1DE91AAD-C146-5D89-83FA-31A8FD63EBB3},id={585596CA-7903-500B-8DF2-0357DD6E3FAC})',null,false)\",\"header\":\"Edit Experience Editor Options\",\"icon\":\"/temp/iconcache/office/16x16/clipboard_check_edit.png\",\"disabledIcon\":\"/temp/clipboard_check_edit_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the Experience Editor options for the component.\",\"type\":\"common\"},{\"click\":\"chrome:rendering:properties\",\"header\":\"Edit component properties\",\"icon\":\"/temp/iconcache/office/16x16/elements_branch.png\",\"disabledIcon\":\"/temp/elements_branch_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the properties for the component.\",\"type\":\"common\"},{\"click\":\"javascript:Sitecore.PageModes.PageEditor.postRequest('webedit:setdatasource(referenceId={E02DDB9B-A062-5E50-924A-1940D7E053CE},renderingId={1DE91AAD-C146-5D89-83FA-31A8FD63EBB3},id={585596CA-7903-500B-8DF2-0357DD6E3FAC})',null,false)\",\"header\":\"dsHeaderParameter\",\"icon\":\"/temp/iconcache/office/16x16/data.png\",\"disabledIcon\":\"/temp/data_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"dsTooltipParameter\",\"type\":\"datasourcesmenu\"},{\"click\":\"chrome:rendering:personalize({command:\\\"webedit:personalize\\\"})\",\"header\":\"Personalize\",\"icon\":\"/temp/iconcache/office/16x16/users_family.png\",\"disabledIcon\":\"/temp/users_family_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Create or edit personalization for this component.\",\"type\":\"sticky\"},{\"click\":\"chrome:common:edititem({command:\\\"webedit:open\\\"})\",\"header\":\"Edit the related item\",\"icon\":\"/temp/iconcache/office/16x16/cubes.png\",\"disabledIcon\":\"/temp/cubes_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the related item in the Content Editor.\",\"type\":\"datasourcesmenu\"},{\"click\":\"chrome:rendering:delete\",\"header\":\"Delete\",\"icon\":\"/temp/iconcache/office/16x16/delete.png\",\"disabledIcon\":\"/temp/delete_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Remove component.\",\"type\":\"sticky\"}],\"contextItemUri\":\"sitecore://master/{585596CA-7903-500B-8DF2-0357DD6E3FAC}?lang=en&ver=1\",\"custom\":{\"renderingID\":\"1DE91AADC1465D8983FA31A8FD63EBB3\",\"editable\":\"true\"},\"displayName\":\"Content Block\",\"expandedDisplayName\":null}",
                                        Attributes = new Dictionary<string, string>
                                        {
                                            { "type", "text/sitecore" },
                                            { "chrometype", "rendering" },
                                            { "kind", "open" },
                                            { "id", "r_B7C779DA2B75586CB40D081FCB864256" },
                                            { "hintname", "Component 2" },
                                            { "class", "scpm" },
                                            { "data-selectable", "true" }
                                        }
                                    },

                                    new Component
                                    {
                                        Id = "{B7C779DA-2B75-586C-B40D-081FCB864256}",
                                        Name = "Component-2",
                                        Fields =
                                        {
                                            ["TestField"] = new RichTextField(TestConstants.RichTextFieldValue1),
                                            ["ImageField"] = new ImageField(new Image
                                            {
                                                Alt = "Sitecore Logo",
                                                Src =
                                                    "/sitecore/shell/-/jssmedia/styleguide/data/media/img/sc_logo.png?iar=0"
                                            })
                                            {
                                                EditableMarkup =
                                                    "<input id='fld_2121847759D950C4B7F4FBCD69760250_2AE326C130E557708C99E099EE76D4D3_en_1_aeede4c4d2924ff0bfafee1419803eb4_42' class='scFieldValue' name='fld_2121847759D950C4B7F4FBCD69760250_2AE326C130E557708C99E099EE76D4D3_en_1_aeede4c4d2924ff0bfafee1419803eb4_42' type='hidden' value=\"&lt;image alt=&quot;Sitecore Logo&quot; mediaid=&quot;{47408259-ECC4-553D-9585-73E504EEEDCE}&quot; /&gt;\" /><code id=\"fld_2121847759D950C4B7F4FBCD69760250_2AE326C130E557708C99E099EE76D4D3_en_1_aeede4c4d2924ff0bfafee1419803eb4_42_edit\" type=\"text/sitecore\" chromeType=\"field\" scFieldType=\"image\" class=\"scpm\" kind=\"open\">{\"commands\":[{\"click\":\"chrome:field:editcontrol({command:\\\"webedit:chooseimage\\\"})\",\"header\":\"Choose Image\",\"icon\":\"/sitecore/shell/themes/standard/custom/16x16/photo_landscape2.png\",\"disabledIcon\":\"/temp/photo_landscape2_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Choose an image.\",\"type\":\"\"},{\"click\":\"chrome:field:editcontrol({command:\\\"webedit:editimage\\\"})\",\"header\":\"Properties\",\"icon\":\"/sitecore/shell/themes/standard/custom/16x16/photo_landscape2_edit.png\",\"disabledIcon\":\"/temp/photo_landscape2_edit_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Modify image appearance.\",\"type\":\"\"},{\"click\":\"chrome:field:editcontrol({command:\\\"webedit:clearimage\\\"})\",\"header\":\"Clear\",\"icon\":\"/sitecore/shell/themes/standard/custom/16x16/photo_landscape2_delete.png\",\"disabledIcon\":\"/temp/photo_landscape2_delete_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Remove the image.\",\"type\":\"\"},{\"click\":\"chrome:common:edititem({command:\\\"webedit:open\\\"})\",\"header\":\"Edit the related item\",\"icon\":\"/temp/iconcache/office/16x16/cubes.png\",\"disabledIcon\":\"/temp/cubes_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the related item in the Content Editor.\",\"type\":\"common\"},{\"click\":\"chrome:rendering:personalize({command:\\\"webedit:personalize\\\"})\",\"header\":\"Personalize\",\"icon\":\"/temp/iconcache/office/16x16/users_family.png\",\"disabledIcon\":\"/temp/users_family_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Create or edit personalization for this component.\",\"type\":\"sticky\"},{\"click\":\"chrome:rendering:editvariations({command:\\\"webedit:editvariations\\\"})\",\"header\":\"Edit variations\",\"icon\":\"/temp/iconcache/office/16x16/windows.png\",\"disabledIcon\":\"/temp/windows_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the variations.\",\"type\":\"sticky\"}],\"contextItemUri\":\"sitecore://master/{21218477-59D9-50C4-B7F4-FBCD69760250}?lang=en&ver=1\",\"custom\":{},\"displayName\":\"sample1\",\"expandedDisplayName\":null}</code><img src=\"/sitecore/shell/-/jssmedia/styleguide/data/media/img/sc_logo.png?iar=0\" alt=\"Sitecore Logo\" /><code class=\"scpm\" type=\"text/sitecore\" chromeType=\"field\" kind=\"close\"></code>"
                                            }
                                        }
                                    },

                                    new EditableChrome
                                    {
                                        Name = "code",
                                        Type = "text/sitecore",
                                        Content = string.Empty,
                                        Attributes = new Dictionary<string, string>
                                        {
                                            { "type", "text/sitecore" },
                                            { "chrometype", "rendering" },
                                            { "kind", "close" },
                                            { "id", "scEnclosingTag_r_" },
                                            { "hintkey", "Component 2" },
                                            { "class", "scpm" }
                                        }
                                    },

                                    new EditableChrome
                                    {
                                        Name = "code",
                                        Type = "text/sitecore",
                                        Content = string.Empty,
                                        Attributes = new Dictionary<string, string>
                                        {
                                            { "type", "text/sitecore" },
                                            { "chrometype", "placeholder" },
                                            { "kind", "close" },
                                            { "id", "scEnclosingTag_" },
                                            { "hintname", "Placeholder-2" },
                                            { "class", "scpm" }
                                        }
                                    }
                                ]
                            }
                        },

                        new EditableChrome
                        {
                            Name = "code",
                            Type = "text/sitecore",
                            Content = string.Empty,
                            Attributes = new Dictionary<string, string>
                            {
                                { "type", "text/sitecore" },
                                { "chrometype", "rendering" },
                                { "kind", "close" },
                                { "id", "scEnclosingTag_r_" },
                                { "hintkey", "Component 1" },
                                { "class", "scpm" }
                            }
                        },

                        new EditableChrome
                        {
                            Name = "code",
                            Type = "text/sitecore",
                            Content = string.Empty,
                            Attributes = new Dictionary<string, string>
                            {
                                { "type", "text/sitecore" },
                                { "chrometype", "placeholder" },
                                { "kind", "close" },
                                { "id", "scEnclosingTag_" },
                                { "hintname", "Placeholder-1" },
                                { "class", "scpm" }
                            }
                        }
                    ]
                }
            },
            Devices =
            [
                new Device
                {
                    Id = "fe5d7fdf-89c0-4d99-9aa3-b5fbd009c9f3",
                    LayoutId = "14030e9f-ce92-49c6-ad87-7d49b50e42ea",
                    Renderings =
                    [
                        new Rendering
                        {
                            Id = "885b8314-7d8c-4cbb-8000-01421ea8f406",
                            InstanceId = "43222d12-08c9-453b-ae96-d406ebb95126",
                            PlaceholderKey = "main"
                        },

                        new Rendering
                        {
                            Id = "ce4adcfb-7990-4980-83fb-a00c1e3673db",
                            InstanceId = "cf044ad9-0332-407a-abde-587214a2c808",
                            PlaceholderKey = "/main/centercolumn"
                        },

                        new Rendering
                        {
                            Id = "493b3a83-0fa7-4484-8fc9-4680991cf743",
                            InstanceId = "b343725a-3a93-446e-a9c8-3a2cbd3db489",
                            PlaceholderKey = "/main/centercolumn/content"
                        }
                    ]
                },

                new Device
                {
                    Id = "46d2f427-4ce5-4e1f-ba10-ef3636f43534",
                    LayoutId = "14030e9f-ce92-49c6-ad87-7d49b50e42ea",
                    Renderings =
                    [
                        new Rendering
                        {
                            Id = "493b3a83-0fa7-4484-8fc9-4680991cf743",
                            InstanceId = "a08c9132-dbd1-474f-a2ca-6ca26a4aa650",
                            PlaceholderKey = "content"
                        }
                    ]
                }
            ]
        }
    };

    public static SitecoreLayoutResponseContent HorizonEditablePage => new()
    {
        Sitecore = new SitecoreData
        {
            Context = new Context
            {
                Language = TestConstants.Language,
                IsEditing = true,
                PageState = PageState.Edit,
                Site = new Site
                {
                    Name = "sample"
                }
            },
            Route = new Route
            {
                LayoutId = TestConstants.NestedPlaceholderPageLayoutId,

                DatabaseName = TestConstants.DatabaseName,
                DeviceId = "test-device-id",
                ItemId = TestConstants.TestItemId,
                ItemLanguage = "en",
                ItemVersion = 1,
                TemplateId = "test-template-id",
                TemplateName = "test-template-name",
                Name = "styleguide",
                Fields =
                {
                    ["pageTitle"] = new TextField
                    {
                        Value = TestConstants.PageTitle,
                        EditableMarkup = "<input id='fld_8F7BEF7528A554F0B7C4998B51B67C75_152F40EDFE765861B425522375549742_en_1_60748843912c4eb5a66c94e9e275e52b_391' class='scFieldValue' name='fld_8F7BEF7528A554F0B7C4998B51B67C75_152F40EDFE765861B425522375549742_en_1_60748843912c4eb5a66c94e9e275e52b_391' type='hidden' value=\"Styleguide | Sitecore JSS\" /><span class=\"scChromeData\">{\"contextItem\":{\"id\":\"8f7bef75-28a5-54f0-b7c4-998b51b67c75\",\"version\":1,\"language\":\"en\",\"revision\":\"60748843912c4eb5a66c94e9e275e52b\"},\"fieldId\":\"152f40ed-fe76-5861-b425-522375549742\",\"fieldType\":\"Single-Line Text\",\"fieldWebEditParameters\":{\"prevent-line-break\":\"true\"},\"commands\":[{\"click\":\"chrome:common:edititem({command:\\\"webedit:open\\\"})\",\"header\":\"Edit the related item\",\"icon\":\"/temp/iconcache/office/16x16/cubes.png\",\"disabledIcon\":\"/temp/cubes_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the related item in the Content Editor.\",\"type\":\"common\"},{\"click\":\"chrome:rendering:personalize({command:\\\"webedit:personalize\\\"})\",\"header\":\"Personalize\",\"icon\":\"/temp/iconcache/office/16x16/users_family.png\",\"disabledIcon\":\"/temp/users_family_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Create or edit personalization for this component.\",\"type\":\"sticky\"}],\"contextItemUri\":\"sitecore://master/{8F7BEF75-28A5-54F0-B7C4-998B51B67C75}?lang=en&ver=1\",\"custom\":{},\"displayName\":\"Page Title\",\"expandedDisplayName\":null}</span><span id=\"fld_8F7BEF7528A554F0B7C4998B51B67C75_152F40EDFE765861B425522375549742_en_1_60748843912c4eb5a66c94e9e275e52b_391_edit\" sc_parameters=\"prevent-line-break=true\" contenteditable=\"true\" class=\"scWebEditInput\" scFieldType=\"single-line text\" scDefaultText=\"[No text in field]\">Styleguide | Sitecore JSS</span>"
                    }
                },
                Placeholders =
                {
                    ["placeholder-1"] =
                    [
                        new EditableChrome
                        {
                            Name = "code",
                            Type = "text/sitecore",
                            Content =
                                "{\"contextItem\":{\"id\":\"8f7bef75-28a5-54f0-b7c4-998b51b67c75\",\"version\":1,\"language\":\"en\",\"revision\":\"60748843912c4eb5a66c94e9e275e52b\"},\"placeholderKey\":\"jss-main\",\"placeholderMetadataKeys\":[\"jss-main\"],\"editable\":true,\"commands\":[{\"click\":\"chrome:placeholder:addControl\",\"header\":\"Add to here\",\"icon\":\"/temp/iconcache/office/16x16/add.png\",\"disabledIcon\":\"/temp/add_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Add a new rendering to the '{0}' placeholder.\",\"type\":\"\"},{\"click\":\"chrome:placeholder:editSettings\",\"header\":\"\",\"icon\":\"/temp/iconcache/office/16x16/window_gear.png\",\"disabledIcon\":\"/temp/window_gear_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the placeholder settings.\",\"type\":\"\"}],\"contextItemUri\":\"sitecore://master/{8F7BEF75-28A5-54F0-B7C4-998B51B67C75}?lang=en&ver=1\",\"custom\":{\"allowedRenderings\":[\"D8120D14950758B8BEF2E7A5158BD50F\",\"71AAF5C592425806BE7CECDF9154840B\",\"F351174D07C0547FBBDAEE51349C7DF5\",\"9B43C994B9835EC1A578401EA9478115\",\"FB33EEE82D6F5DD49CC71A5CBEBA728F\"],\"editable\":\"true\"},\"displayName\":\"Main\",\"expandedDisplayName\":null}",
                            Attributes = new Dictionary<string, string>
                            {
                                { "type", "text/sitecore" },
                                { "chrometype", "placeholder" },
                                { "kind", "open" },
                                { "id", "placeholder_1" },
                                { "key", "placeholder-1" },
                                { "class", "scpm" },
                                { "data-selectable", "true" }
                            }
                        },

                        new EditableChrome
                        {
                            Name = "code",
                            Type = "text/sitecore",
                            Content =
                                "{\"contextItem\":{\"id\":\"a2484483-af6f-5723-a29f-785e12ced97b\",\"version\":1,\"language\":\"en\",\"revision\":\"c950fc1bd5484df88dc99bce389d51a0\"},\"renderingId\":\"71aaf5c5-9242-5806-be7c-ecdf9154840b\",\"renderingInstanceId\":\"{E02DDB9B-A062-5E50-924A-1940D7E053CE}\",\"editable\":true,\"commands\":[{\"click\":\"javascript:Sitecore.PageModes.PageEditor.postRequest('webedit:fieldeditor(command={2F7D34E2-EB93-47ED-8177-B1F429A9FCD7},fields=heading|content,id={A2484483-AF6F-5723-A29F-785E12CED97B})',null,false)\",\"header\":\"Edit Fields\",\"icon\":\"/temp/iconcache/office/16x16/pencil.png\",\"disabledIcon\":\"/temp/pencil_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"\",\"type\":null},{\"click\":\"chrome:dummy\",\"header\":\"Separator\",\"icon\":\"\",\"disabledIcon\":\"\",\"isDivider\":false,\"tooltip\":null,\"type\":\"separator\"},{\"click\":\"chrome:rendering:sort\",\"header\":\"Change position\",\"icon\":\"/temp/iconcache/office/16x16/document_size.png\",\"disabledIcon\":\"/temp/document_size_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Move component.\",\"type\":\"\"},{\"click\":\"javascript:Sitecore.PageModes.PageEditor.postRequest('webedit:componentoptions(referenceId={E02DDB9B-A062-5E50-924A-1940D7E053CE},renderingId={71AAF5C5-9242-5806-BE7C-ECDF9154840B},id={A2484483-AF6F-5723-A29F-785E12CED97B})',null,false)\",\"header\":\"Edit Experience Editor Options\",\"icon\":\"/temp/iconcache/office/16x16/clipboard_check_edit.png\",\"disabledIcon\":\"/temp/clipboard_check_edit_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the Experience Editor options for the component.\",\"type\":\"common\"},{\"click\":\"chrome:rendering:properties\",\"header\":\"Edit component properties\",\"icon\":\"/temp/iconcache/office/16x16/elements_branch.png\",\"disabledIcon\":\"/temp/elements_branch_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the properties for the component.\",\"type\":\"common\"},{\"click\":\"javascript:Sitecore.PageModes.PageEditor.postRequest('webedit:setdatasource(referenceId={E02DDB9B-A062-5E50-924A-1940D7E053CE},renderingId={71AAF5C5-9242-5806-BE7C-ECDF9154840B},id={A2484483-AF6F-5723-A29F-785E12CED97B})',null,false)\",\"header\":\"dsHeaderParameter\",\"icon\":\"/temp/iconcache/office/16x16/data.png\",\"disabledIcon\":\"/temp/data_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"dsTooltipParameter\",\"type\":\"datasourcesmenu\"},{\"click\":\"chrome:rendering:personalize({command:\\\"webedit:personalize\\\"})\",\"header\":\"Personalize\",\"icon\":\"/temp/iconcache/office/16x16/users_family.png\",\"disabledIcon\":\"/temp/users_family_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Create or edit personalization for this component.\",\"type\":\"sticky\"},{\"click\":\"chrome:common:edititem({command:\\\"webedit:open\\\"})\",\"header\":\"Edit the related item\",\"icon\":\"/temp/iconcache/office/16x16/cubes.png\",\"disabledIcon\":\"/temp/cubes_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the related item in the Content Editor.\",\"type\":\"datasourcesmenu\"},{\"click\":\"chrome:rendering:delete\",\"header\":\"Delete\",\"icon\":\"/temp/iconcache/office/16x16/delete.png\",\"disabledIcon\":\"/temp/delete_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Remove component.\",\"type\":\"sticky\"}],\"contextItemUri\":\"sitecore://master/{A2484483-AF6F-5723-A29F-785E12CED97B}?lang=en&ver=1\",\"custom\":{\"renderingID\":\"71AAF5C592425806BE7CECDF9154840B\",\"editable\":\"true\"},\"displayName\":\"Content Block\",\"expandedDisplayName\":null}",
                            Attributes = new Dictionary<string, string>
                            {
                                { "type", "text/sitecore" },
                                { "chrometype", "rendering" },
                                { "kind", "open" },
                                { "id", "r_E02DDB9BA0625E50924A1940D7E053CE" },
                                { "hintname", "Component 1" },
                                { "class", "scpm" },
                                { "data-selectable", "true" }
                            }
                        },

                        new Component
                        {
                            Id = "{34A6553C-81DE-5CD3-989E-853F6CB6DF8C}",
                            Name = "Component-1",
                            Placeholders =
                            {
                                ["placeholder-2"] =
                                [
                                    new EditableChrome
                                    {
                                        Name = "code",
                                        Type = "text/sitecore",
                                        Content =
                                            "{\"contextItem\":{\"id\":\"8f7bef75-28a5-54f0-b7c4-998b51b67c75\",\"version\":1,\"language\":\"en\",\"revision\":\"60748843912c4eb5a66c94e9e275e52b\"},\"placeholderKey\":\"/jss-main/jss-styleguide-layout-{34A6553C-81DE-5CD3-989E-853F6CB6DF8C}-0\",\"placeholderMetadataKeys\":[\"/jss-main/jss-styleguide-layout\",\"jss-styleguide-layout\"],\"editable\":true,\"commands\":[{\"click\":\"chrome:placeholder:addControl\",\"header\":\"Add to here\",\"icon\":\"/temp/iconcache/office/16x16/add.png\",\"disabledIcon\":\"/temp/add_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Add a new rendering to the '{0}' placeholder.\",\"type\":\"\"},{\"click\":\"chrome:placeholder:editSettings\",\"header\":\"\",\"icon\":\"/temp/iconcache/office/16x16/window_gear.png\",\"disabledIcon\":\"/temp/window_gear_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the placeholder settings.\",\"type\":\"\"}],\"contextItemUri\":\"sitecore://master/{8F7BEF75-28A5-54F0-B7C4-998B51B67C75}?lang=en&ver=1\",\"custom\":{\"allowedRenderings\":[\"A7BFC343F487521593488FBB0D209FA6\"],\"editable\":\"true\"},\"displayName\":\"jss-styleguide-layout\",\"expandedDisplayName\":null}",
                                        Attributes = new Dictionary<string, string>
                                        {
                                            { "type", "text/sitecore" },
                                            { "chrometype", "placeholder" },
                                            { "kind", "open" },
                                            {
                                                "id",
                                                "_placeholder_1_placeholder_2__34A6553C_81DE_5CD3_989E_853F6CB6DF8C__0"
                                            },
                                            {
                                                "key",
                                                "/placeholder-1/placeholder-2-{34A6553C-81DE-5CD3-989E-853F6CB6DF8C}-0"
                                            },
                                            { "class", "scpm" },
                                            { "data-selectable", "true" }
                                        }
                                    },

                                    new EditableChrome
                                    {
                                        Name = "code",
                                        Type = "text/sitecore",
                                        Content =
                                            "{\"contextItem\":{\"id\":\"9f76f747-bc96-572a-bbcb-9d7655f98ac2\",\"version\":1,\"language\":\"en\",\"revision\":\"39157d6881cc4ef5aba1dae028fd4fb9\"},\"renderingId\":\"a7bfc343-f487-5215-9348-8fbb0d209fa6\",\"renderingInstanceId\":\"{B7C779DA-2B75-586C-B40D-081FCB864256}\",\"editable\":true,\"commands\":[{\"click\":\"javascript:Sitecore.PageModes.PageEditor.postRequest('webedit:fieldeditor(command={2F7D34E2-EB93-47ED-8177-B1F429A9FCD7},fields=heading,id={9F76F747-BC96-572A-BBCB-9D7655F98AC2})',null,false)\",\"header\":\"Edit Fields\",\"icon\":\"/temp/iconcache/office/16x16/pencil.png\",\"disabledIcon\":\"/temp/pencil_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"\",\"type\":null},{\"click\":\"chrome:dummy\",\"header\":\"Separator\",\"icon\":\"\",\"disabledIcon\":\"\",\"isDivider\":false,\"tooltip\":null,\"type\":\"separator\"},{\"click\":\"chrome:rendering:sort\",\"header\":\"Change position\",\"icon\":\"/temp/iconcache/office/16x16/document_size.png\",\"disabledIcon\":\"/temp/document_size_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Move component.\",\"type\":\"\"},{\"click\":\"javascript:Sitecore.PageModes.PageEditor.postRequest('webedit:componentoptions(referenceId={B7C779DA-2B75-586C-B40D-081FCB864256},renderingId={A7BFC343-F487-5215-9348-8FBB0D209FA6},id={9F76F747-BC96-572A-BBCB-9D7655F98AC2})',null,false)\",\"header\":\"Edit Experience Editor Options\",\"icon\":\"/temp/iconcache/office/16x16/clipboard_check_edit.png\",\"disabledIcon\":\"/temp/clipboard_check_edit_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the Experience Editor options for the component.\",\"type\":\"common\"},{\"click\":\"chrome:rendering:properties\",\"header\":\"Edit component properties\",\"icon\":\"/temp/iconcache/office/16x16/elements_branch.png\",\"disabledIcon\":\"/temp/elements_branch_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the properties for the component.\",\"type\":\"common\"},{\"click\":\"javascript:Sitecore.PageModes.PageEditor.postRequest('webedit:setdatasource(referenceId={B7C779DA-2B75-586C-B40D-081FCB864256},renderingId={A7BFC343-F487-5215-9348-8FBB0D209FA6},id={9F76F747-BC96-572A-BBCB-9D7655F98AC2})',null,false)\",\"header\":\"dsHeaderParameter\",\"icon\":\"/temp/iconcache/office/16x16/data.png\",\"disabledIcon\":\"/temp/data_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"dsTooltipParameter\",\"type\":\"datasourcesmenu\"},{\"click\":\"chrome:rendering:personalize({command:\\\"webedit:personalize\\\"})\",\"header\":\"Personalize\",\"icon\":\"/temp/iconcache/office/16x16/users_family.png\",\"disabledIcon\":\"/temp/users_family_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Create or edit personalization for this component.\",\"type\":\"sticky\"},{\"click\":\"chrome:common:edititem({command:\\\"webedit:open\\\"})\",\"header\":\"Edit the related item\",\"icon\":\"/temp/iconcache/office/16x16/cubes.png\",\"disabledIcon\":\"/temp/cubes_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the related item in the Content Editor.\",\"type\":\"datasourcesmenu\"},{\"click\":\"chrome:rendering:delete\",\"header\":\"Delete\",\"icon\":\"/temp/iconcache/office/16x16/delete.png\",\"disabledIcon\":\"/temp/delete_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Remove component.\",\"type\":\"sticky\"}],\"contextItemUri\":\"sitecore://master/{9F76F747-BC96-572A-BBCB-9D7655F98AC2}?lang=en&ver=1\",\"custom\":{\"renderingID\":\"A7BFC343F487521593488FBB0D209FA6\",\"editable\":\"true\"},\"displayName\":\"Styleguide-Section\",\"expandedDisplayName\":null}",
                                        Attributes = new Dictionary<string, string>
                                        {
                                            { "type", "text/sitecore" },
                                            { "chrometype", "rendering" },
                                            { "kind", "open" },
                                            { "id", "r_B7C779DA2B75586CB40D081FCB864256" },
                                            { "hintname", "Component 2" },
                                            { "class", "scpm" },
                                            { "data-selectable", "true" }
                                        }
                                    },

                                    new Component
                                    {
                                        Id = "{B7C779DA-2B75-586C-B40D-081FCB864256}",
                                        Name = "Component-2",
                                        Fields =
                                        {
                                            ["TestField"] = new RichTextField(TestConstants.RichTextFieldValue1)
                                            {
                                                EditableMarkup =
                                                    "<input id='fld_A2484483AF6F5723A29F785E12CED97B_6856AF27B4135FCEB3FDC560612F1199_en_1_c950fc1bd5484df88dc99bce389d51a0_393' class='scFieldValue' name='fld_A2484483AF6F5723A29F785E12CED97B_6856AF27B4135FCEB3FDC560612F1199_en_1_c950fc1bd5484df88dc99bce389d51a0_393' type='hidden' value=\"" +
                                                    HtmlEncoder.Default.Encode(TestConstants.RichTextFieldValue1) +
                                                    "\" /><span class=\"scChromeData\">{\"contextItem\":{\"id\":\"a2484483-af6f-5723-a29f-785e12ced97b\",\"version\":1,\"language\":\"en\",\"revision\":\"c950fc1bd5484df88dc99bce389d51a0\"},\"fieldId\":\"6856af27-b413-5fce-b3fd-c560612f1199\",\"fieldType\":\"Rich Text\",\"fieldWebEditParameters\":{},\"commands\":[{\"click\":\"chrome:field:editcontrol({command:\\\"webedit:edithtml\\\"})\",\"header\":\"Edit Text\",\"icon\":\"/temp/iconcache/office/16x16/pencil.png\",\"disabledIcon\":\"/temp/pencil_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the text\",\"type\":null},{\"click\":\"chrome:field:execute({command:\\\"bold\\\", userInterface:true, value:true})\",\"header\":\"\",\"icon\":\"/temp/iconcache/office/16x16/font_style_bold.png\",\"disabledIcon\":\"/temp/font_style_bold_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Bold\",\"type\":null},{\"click\":\"chrome:field:execute({command:\\\"Italic\\\", userInterface:true, value:true})\",\"header\":\"\",\"icon\":\"/temp/iconcache/office/16x16/font_style_italics.png\",\"disabledIcon\":\"/temp/font_style_italics_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Italic\",\"type\":null},{\"click\":\"chrome:field:execute({command:\\\"Underline\\\", userInterface:true, value:true})\",\"header\":\"\",\"icon\":\"/temp/iconcache/office/16x16/font_style_underline.png\",\"disabledIcon\":\"/temp/font_style_underline_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Underline\",\"type\":null},{\"click\":\"chrome:field:insertexternallink\",\"header\":\"\",\"icon\":\"/temp/iconcache/office/16x16/earth_link.png\",\"disabledIcon\":\"/temp/earth_link_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Insert an external link into the text field.\",\"type\":null},{\"click\":\"chrome:field:insertlink\",\"header\":\"\",\"icon\":\"/temp/iconcache/office/16x16/link.png\",\"disabledIcon\":\"/temp/link_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Insert a link into the text field.\",\"type\":null},{\"click\":\"chrome:field:removelink\",\"header\":\"\",\"icon\":\"/temp/iconcache/office/16x16/link_broken.png\",\"disabledIcon\":\"/temp/link_broken_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Remove link.\",\"type\":null},{\"click\":\"chrome:field:insertimage\",\"header\":\"Insert image\",\"icon\":\"/temp/iconcache/office/16x16/photo_landscape.png\",\"disabledIcon\":\"/temp/photo_landscape_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Insert an image into the text field.\",\"type\":null},{\"click\":\"chrome:common:edititem({command:\\\"webedit:open\\\"})\",\"header\":\"Edit the related item\",\"icon\":\"/temp/iconcache/office/16x16/cubes.png\",\"disabledIcon\":\"/temp/cubes_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the related item in the Content Editor.\",\"type\":\"common\"},{\"click\":\"chrome:rendering:personalize({command:\\\"webedit:personalize\\\"})\",\"header\":\"Personalize\",\"icon\":\"/temp/iconcache/office/16x16/users_family.png\",\"disabledIcon\":\"/temp/users_family_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Create or edit personalization for this component.\",\"type\":\"sticky\"}],\"contextItemUri\":\"sitecore://master/{A2484483-AF6F-5723-A29F-785E12CED97B}?lang=en&ver=1\",\"custom\":{},\"displayName\":\"content\",\"expandedDisplayName\":null}</span><span scFieldType=\"rich text\" scDefaultText=\"[No text in field]\" contenteditable=\"true\" class=\"scWebEditInput\" id=\"fld_A2484483AF6F5723A29F785E12CED97B_6856AF27B4135FCEB3FDC560612F1199_en_1_c950fc1bd5484df88dc99bce389d51a0_393_edit\">" +
                                                    TestConstants.RichTextFieldValue1 + "</span>"
                                            },
                                            ["EmptyField"] = new TextField(string.Empty)
                                            {
                                                EditableMarkup =
                                                    "<input id='fld_A2484483AF6F5723A29F785E12CED97B_6856AF27B4135FCEB3FDC560612F1199_en_1_c950fc1bd5484df88dc99bce389d51a0_393' class='scFieldValue' name='fld_A2484483AF6F5723A29F785E12CED97B_6856AF27B4135FCEB3FDC560612F1199_en_1_c950fc1bd5484df88dc99bce389d51a0_393' type='hidden' value=\"\" /><span class=\"scChromeData\">{\"contextItem\":{\"id\":\"a2484483-af6f-5723-a29f-785e12ced97b\",\"version\":1,\"language\":\"en\",\"revision\":\"c950fc1bd5484df88dc99bce389d51a0\"},\"fieldId\":\"6856af27-b413-5fce-b3fd-c560612f1199\",\"fieldType\":\"Rich Text\",\"fieldWebEditParameters\":{},\"commands\":[{\"click\":\"chrome:field:editcontrol({command:\\\"webedit:edithtml\\\"})\",\"header\":\"Edit Text\",\"icon\":\"/temp/iconcache/office/16x16/pencil.png\",\"disabledIcon\":\"/temp/pencil_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the text\",\"type\":null},{\"click\":\"chrome:field:execute({command:\\\"bold\\\", userInterface:true, value:true})\",\"header\":\"\",\"icon\":\"/temp/iconcache/office/16x16/font_style_bold.png\",\"disabledIcon\":\"/temp/font_style_bold_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Bold\",\"type\":null},{\"click\":\"chrome:field:execute({command:\\\"Italic\\\", userInterface:true, value:true})\",\"header\":\"\",\"icon\":\"/temp/iconcache/office/16x16/font_style_italics.png\",\"disabledIcon\":\"/temp/font_style_italics_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Italic\",\"type\":null},{\"click\":\"chrome:field:execute({command:\\\"Underline\\\", userInterface:true, value:true})\",\"header\":\"\",\"icon\":\"/temp/iconcache/office/16x16/font_style_underline.png\",\"disabledIcon\":\"/temp/font_style_underline_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Underline\",\"type\":null},{\"click\":\"chrome:field:insertexternallink\",\"header\":\"\",\"icon\":\"/temp/iconcache/office/16x16/earth_link.png\",\"disabledIcon\":\"/temp/earth_link_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Insert an external link into the text field.\",\"type\":null},{\"click\":\"chrome:field:insertlink\",\"header\":\"\",\"icon\":\"/temp/iconcache/office/16x16/link.png\",\"disabledIcon\":\"/temp/link_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Insert a link into the text field.\",\"type\":null},{\"click\":\"chrome:field:removelink\",\"header\":\"\",\"icon\":\"/temp/iconcache/office/16x16/link_broken.png\",\"disabledIcon\":\"/temp/link_broken_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Remove link.\",\"type\":null},{\"click\":\"chrome:field:insertimage\",\"header\":\"Insert image\",\"icon\":\"/temp/iconcache/office/16x16/photo_landscape.png\",\"disabledIcon\":\"/temp/photo_landscape_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Insert an image into the text field.\",\"type\":null},{\"click\":\"chrome:common:edititem({command:\\\"webedit:open\\\"})\",\"header\":\"Edit the related item\",\"icon\":\"/temp/iconcache/office/16x16/cubes.png\",\"disabledIcon\":\"/temp/cubes_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the related item in the Content Editor.\",\"type\":\"common\"},{\"click\":\"chrome:rendering:personalize({command:\\\"webedit:personalize\\\"})\",\"header\":\"Personalize\",\"icon\":\"/temp/iconcache/office/16x16/users_family.png\",\"disabledIcon\":\"/temp/users_family_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Create or edit personalization for this component.\",\"type\":\"sticky\"}],\"contextItemUri\":\"sitecore://master/{A2484483-AF6F-5723-A29F-785E12CED97B}?lang=en&ver=1\",\"custom\":{},\"displayName\":\"content\",\"expandedDisplayName\":null}</span><span scFieldType=\"rich text\" scDefaultText=\"[No text in field]\" contenteditable=\"true\" class=\"scWebEditInput\" id=\"fld_A2484483AF6F5723A29F785E12CED97B_6856AF27B4135FCEB3FDC560612F1199_en_1_c950fc1bd5484df88dc99bce389d51a0_393_edit\"></span>"
                                            }
                                        }
                                    },

                                    new EditableChrome
                                    {
                                        Name = "code",
                                        Type = "text/sitecore",
                                        Content = string.Empty,
                                        Attributes = new Dictionary<string, string>
                                        {
                                            { "type", "text/sitecore" },
                                            { "chrometype", "rendering" },
                                            { "kind", "close" },
                                            { "id", "scEnclosingTag_r_" },
                                            { "hintkey", "Component 2" },
                                            { "class", "scpm" }
                                        }
                                    },

                                    new EditableChrome
                                    {
                                        Name = "code",
                                        Type = "text/sitecore",
                                        Content = string.Empty,
                                        Attributes = new Dictionary<string, string>
                                        {
                                            { "type", "text/sitecore" },
                                            { "chrometype", "placeholder" },
                                            { "kind", "close" },
                                            { "id", "scEnclosingTag_" },
                                            { "hintname", "Placeholder-2" },
                                            { "class", "scpm" }
                                        }
                                    }
                                ]
                            }
                        },

                        new EditableChrome
                        {
                            Name = "code",
                            Type = "text/sitecore",
                            Content = string.Empty,
                            Attributes = new Dictionary<string, string>
                            {
                                { "type", "text/sitecore" },
                                { "chrometype", "rendering" },
                                { "kind", "close" },
                                { "id", "scEnclosingTag_r_" },
                                { "hintkey", "Component 1" },
                                { "class", "scpm" }
                            }
                        },

                        new EditableChrome
                        {
                            Name = "code",
                            Type = "text/sitecore",
                            Content =
                                "{\"contextItem\":{\"id\":\"9f76f747-bc96-572a-bbcb-9d7655f98ac2\",\"version\":1,\"language\":\"en\",\"revision\":\"39157d6881cc4ef5aba1dae028fd4fb9\"},\"renderingId\":\"a7bfc343-f487-5215-9348-8fbb0d209fa6\",\"renderingInstanceId\":\"{B7C779DA-2B75-586C-B40D-081FCB864256}\",\"editable\":true,\"commands\":[{\"click\":\"javascript:Sitecore.PageModes.PageEditor.postRequest('webedit:fieldeditor(command={2F7D34E2-EB93-47ED-8177-B1F429A9FCD7},fields=heading,id={9F76F747-BC96-572A-BBCB-9D7655F98AC2})',null,false)\",\"header\":\"Edit Fields\",\"icon\":\"/temp/iconcache/office/16x16/pencil.png\",\"disabledIcon\":\"/temp/pencil_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"\",\"type\":null},{\"click\":\"chrome:dummy\",\"header\":\"Separator\",\"icon\":\"\",\"disabledIcon\":\"\",\"isDivider\":false,\"tooltip\":null,\"type\":\"separator\"},{\"click\":\"chrome:rendering:sort\",\"header\":\"Change position\",\"icon\":\"/temp/iconcache/office/16x16/document_size.png\",\"disabledIcon\":\"/temp/document_size_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Move component.\",\"type\":\"\"},{\"click\":\"javascript:Sitecore.PageModes.PageEditor.postRequest('webedit:componentoptions(referenceId={B7C779DA-2B75-586C-B40D-081FCB864256},renderingId={A7BFC343-F487-5215-9348-8FBB0D209FA6},id={9F76F747-BC96-572A-BBCB-9D7655F98AC2})',null,false)\",\"header\":\"Edit Experience Editor Options\",\"icon\":\"/temp/iconcache/office/16x16/clipboard_check_edit.png\",\"disabledIcon\":\"/temp/clipboard_check_edit_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the Experience Editor options for the component.\",\"type\":\"common\"},{\"click\":\"chrome:rendering:properties\",\"header\":\"Edit component properties\",\"icon\":\"/temp/iconcache/office/16x16/elements_branch.png\",\"disabledIcon\":\"/temp/elements_branch_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the properties for the component.\",\"type\":\"common\"},{\"click\":\"javascript:Sitecore.PageModes.PageEditor.postRequest('webedit:setdatasource(referenceId={B7C779DA-2B75-586C-B40D-081FCB864256},renderingId={A7BFC343-F487-5215-9348-8FBB0D209FA6},id={9F76F747-BC96-572A-BBCB-9D7655F98AC2})',null,false)\",\"header\":\"dsHeaderParameter\",\"icon\":\"/temp/iconcache/office/16x16/data.png\",\"disabledIcon\":\"/temp/data_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"dsTooltipParameter\",\"type\":\"datasourcesmenu\"},{\"click\":\"chrome:rendering:personalize({command:\\\"webedit:personalize\\\"})\",\"header\":\"Personalize\",\"icon\":\"/temp/iconcache/office/16x16/users_family.png\",\"disabledIcon\":\"/temp/users_family_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Create or edit personalization for this component.\",\"type\":\"sticky\"},{\"click\":\"chrome:common:edititem({command:\\\"webedit:open\\\"})\",\"header\":\"Edit the related item\",\"icon\":\"/temp/iconcache/office/16x16/cubes.png\",\"disabledIcon\":\"/temp/cubes_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the related item in the Content Editor.\",\"type\":\"datasourcesmenu\"},{\"click\":\"chrome:rendering:delete\",\"header\":\"Delete\",\"icon\":\"/temp/iconcache/office/16x16/delete.png\",\"disabledIcon\":\"/temp/delete_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Remove component.\",\"type\":\"sticky\"}],\"contextItemUri\":\"sitecore://master/{9F76F747-BC96-572A-BBCB-9D7655F98AC2}?lang=en&ver=1\",\"custom\":{\"renderingID\":\"A7BFC343F487521593488FBB0D209FA6\",\"editable\":\"true\"},\"displayName\":\"Styleguide-Section\",\"expandedDisplayName\":null}",
                            Attributes = new Dictionary<string, string>
                            {
                                { "type", "text/sitecore" },
                                { "chrometype", "rendering" },
                                { "kind", "open" },
                                { "id", "r_B7C779DA2B75586CB40D081FCB864256" },
                                { "hintname", "Component 3" },
                                { "class", "scpm" },
                                { "data-selectable", "true" }
                            }
                        },

                        new Component
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = "Component-3",
                            Fields =
                            {
                                ["TestField"] = new TextField(TestConstants.TestFieldValue)
                                {
                                    EditableMarkup =
                                        "<input id='fld_8F7BEF7528A554F0B7C4998B51B67C75_152F40EDFE765861B425522375549742_en_1_60748843912c4eb5a66c94e9e275e52b_391' class='scFieldValue' name='fld_8F7BEF7528A554F0B7C4998B51B67C75_152F40EDFE765861B425522375549742_en_1_60748843912c4eb5a66c94e9e275e52b_391' type='hidden' value=\"" +
                                        HtmlEncoder.Default.Encode(TestConstants.TestFieldValue) +
                                        "\" /><span class=\"scChromeData\">{\"contextItem\":{\"id\":\"8f7bef75-28a5-54f0-b7c4-998b51b67c75\",\"version\":1,\"language\":\"en\",\"revision\":\"60748843912c4eb5a66c94e9e275e52b\"},\"fieldId\":\"152f40ed-fe76-5861-b425-522375549742\",\"fieldType\":\"Single-Line Text\",\"fieldWebEditParameters\":{\"prevent-line-break\":\"true\"},\"commands\":[{\"click\":\"chrome:common:edititem({command:\\\"webedit:open\\\"})\",\"header\":\"Edit the related item\",\"icon\":\"/temp/iconcache/office/16x16/cubes.png\",\"disabledIcon\":\"/temp/cubes_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the related item in the Content Editor.\",\"type\":\"common\"},{\"click\":\"chrome:rendering:personalize({command:\\\"webedit:personalize\\\"})\",\"header\":\"Personalize\",\"icon\":\"/temp/iconcache/office/16x16/users_family.png\",\"disabledIcon\":\"/temp/users_family_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Create or edit personalization for this component.\",\"type\":\"sticky\"}],\"contextItemUri\":\"sitecore://master/{8F7BEF75-28A5-54F0-B7C4-998B51B67C75}?lang=en&ver=1\",\"custom\":{},\"displayName\":\"Page Title\",\"expandedDisplayName\":null}</span><span id=\"fld_8F7BEF7528A554F0B7C4998B51B67C75_152F40EDFE765861B425522375549742_en_1_60748843912c4eb5a66c94e9e275e52b_391_edit\" sc_parameters=\"prevent-line-break=true\" contenteditable=\"true\" class=\"scWebEditInput\" scFieldType=\"single-line text\" scDefaultText=\"[No text in field]\">" +
                                        TestConstants.TestFieldValue + "</span>"
                                }
                            }
                        },

                        new EditableChrome
                        {
                            Name = "code",
                            Type = "text/sitecore",
                            Content = string.Empty,
                            Attributes = new Dictionary<string, string>
                            {
                                { "type", "text/sitecore" },
                                { "chrometype", "rendering" },
                                { "kind", "close" },
                                { "id", "scEnclosingTag_r_" },
                                { "hintkey", "Component 3" },
                                { "class", "scpm" }
                            }
                        },

                        new EditableChrome
                        {
                            Name = "code",
                            Type = "text/sitecore",
                            Content =
                                "{\"contextItem\":{\"id\":\"9f76f747-bc96-572a-bbcb-9d7655f98ac2\",\"version\":1,\"language\":\"en\",\"revision\":\"39157d6881cc4ef5aba1dae028fd4fb9\"},\"renderingId\":\"a7bfc343-f487-5215-9348-8fbb0d209fa6\",\"renderingInstanceId\":\"{B7C779DA-2B75-586C-B40D-081FCB864256}\",\"editable\":true,\"commands\":[{\"click\":\"javascript:Sitecore.PageModes.PageEditor.postRequest('webedit:fieldeditor(command={2F7D34E2-EB93-47ED-8177-B1F429A9FCD7},fields=heading,id={9F76F747-BC96-572A-BBCB-9D7655F98AC2})',null,false)\",\"header\":\"Edit Fields\",\"icon\":\"/temp/iconcache/office/16x16/pencil.png\",\"disabledIcon\":\"/temp/pencil_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"\",\"type\":null},{\"click\":\"chrome:dummy\",\"header\":\"Separator\",\"icon\":\"\",\"disabledIcon\":\"\",\"isDivider\":false,\"tooltip\":null,\"type\":\"separator\"},{\"click\":\"chrome:rendering:sort\",\"header\":\"Change position\",\"icon\":\"/temp/iconcache/office/16x16/document_size.png\",\"disabledIcon\":\"/temp/document_size_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Move component.\",\"type\":\"\"},{\"click\":\"javascript:Sitecore.PageModes.PageEditor.postRequest('webedit:componentoptions(referenceId={B7C779DA-2B75-586C-B40D-081FCB864256},renderingId={A7BFC343-F487-5215-9348-8FBB0D209FA6},id={9F76F747-BC96-572A-BBCB-9D7655F98AC2})',null,false)\",\"header\":\"Edit Experience Editor Options\",\"icon\":\"/temp/iconcache/office/16x16/clipboard_check_edit.png\",\"disabledIcon\":\"/temp/clipboard_check_edit_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the Experience Editor options for the component.\",\"type\":\"common\"},{\"click\":\"chrome:rendering:properties\",\"header\":\"Edit component properties\",\"icon\":\"/temp/iconcache/office/16x16/elements_branch.png\",\"disabledIcon\":\"/temp/elements_branch_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the properties for the component.\",\"type\":\"common\"},{\"click\":\"javascript:Sitecore.PageModes.PageEditor.postRequest('webedit:setdatasource(referenceId={B7C779DA-2B75-586C-B40D-081FCB864256},renderingId={A7BFC343-F487-5215-9348-8FBB0D209FA6},id={9F76F747-BC96-572A-BBCB-9D7655F98AC2})',null,false)\",\"header\":\"dsHeaderParameter\",\"icon\":\"/temp/iconcache/office/16x16/data.png\",\"disabledIcon\":\"/temp/data_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"dsTooltipParameter\",\"type\":\"datasourcesmenu\"},{\"click\":\"chrome:rendering:personalize({command:\\\"webedit:personalize\\\"})\",\"header\":\"Personalize\",\"icon\":\"/temp/iconcache/office/16x16/users_family.png\",\"disabledIcon\":\"/temp/users_family_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Create or edit personalization for this component.\",\"type\":\"sticky\"},{\"click\":\"chrome:common:edititem({command:\\\"webedit:open\\\"})\",\"header\":\"Edit the related item\",\"icon\":\"/temp/iconcache/office/16x16/cubes.png\",\"disabledIcon\":\"/temp/cubes_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the related item in the Content Editor.\",\"type\":\"datasourcesmenu\"},{\"click\":\"chrome:rendering:delete\",\"header\":\"Delete\",\"icon\":\"/temp/iconcache/office/16x16/delete.png\",\"disabledIcon\":\"/temp/delete_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Remove component.\",\"type\":\"sticky\"}],\"contextItemUri\":\"sitecore://master/{9F76F747-BC96-572A-BBCB-9D7655F98AC2}?lang=en&ver=1\",\"custom\":{\"renderingID\":\"A7BFC343F487521593488FBB0D209FA6\",\"editable\":\"true\"},\"displayName\":\"Styleguide-Section\",\"expandedDisplayName\":null}",
                            Attributes = new Dictionary<string, string>
                            {
                                { "type", "text/sitecore" },
                                { "chrometype", "rendering" },
                                { "kind", "open" },
                                { "id", "r_B7C779DA2B75586CB40D081FCB864256" },
                                { "hintname", "Component 4" },
                                { "class", "scpm" },
                                { "data-selectable", "true" }
                            }
                        },

                        new Component
                        {
                            Id = "{B7C779DA-2B75-586C-B40D-081FCB864256}",
                            Name = "Component-4",
                            Fields =
                            {
                                ["RichTextField1"] = new RichTextField(TestConstants.RichTextFieldValue1)
                                {
                                    EditableMarkup =
                                        "<input id='fld_A2484483AF6F5723A29F785E12CED97B_6856AF27B4135FCEB3FDC560612F1199_en_1_c950fc1bd5484df88dc99bce389d51a0_393' class='scFieldValue' name='fld_A2484483AF6F5723A29F785E12CED97B_6856AF27B4135FCEB3FDC560612F1199_en_1_c950fc1bd5484df88dc99bce389d51a0_393' type='hidden' value=\"" +
                                        HtmlEncoder.Default.Encode(TestConstants.RichTextFieldValue1) +
                                        "\" /><span class=\"scChromeData\">{\"contextItem\":{\"id\":\"a2484483-af6f-5723-a29f-785e12ced97b\",\"version\":1,\"language\":\"en\",\"revision\":\"c950fc1bd5484df88dc99bce389d51a0\"},\"fieldId\":\"6856af27-b413-5fce-b3fd-c560612f1199\",\"fieldType\":\"Rich Text\",\"fieldWebEditParameters\":{},\"commands\":[{\"click\":\"chrome:field:editcontrol({command:\\\"webedit:edithtml\\\"})\",\"header\":\"Edit Text\",\"icon\":\"/temp/iconcache/office/16x16/pencil.png\",\"disabledIcon\":\"/temp/pencil_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the text\",\"type\":null},{\"click\":\"chrome:field:execute({command:\\\"bold\\\", userInterface:true, value:true})\",\"header\":\"\",\"icon\":\"/temp/iconcache/office/16x16/font_style_bold.png\",\"disabledIcon\":\"/temp/font_style_bold_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Bold\",\"type\":null},{\"click\":\"chrome:field:execute({command:\\\"Italic\\\", userInterface:true, value:true})\",\"header\":\"\",\"icon\":\"/temp/iconcache/office/16x16/font_style_italics.png\",\"disabledIcon\":\"/temp/font_style_italics_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Italic\",\"type\":null},{\"click\":\"chrome:field:execute({command:\\\"Underline\\\", userInterface:true, value:true})\",\"header\":\"\",\"icon\":\"/temp/iconcache/office/16x16/font_style_underline.png\",\"disabledIcon\":\"/temp/font_style_underline_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Underline\",\"type\":null},{\"click\":\"chrome:field:insertexternallink\",\"header\":\"\",\"icon\":\"/temp/iconcache/office/16x16/earth_link.png\",\"disabledIcon\":\"/temp/earth_link_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Insert an external link into the text field.\",\"type\":null},{\"click\":\"chrome:field:insertlink\",\"header\":\"\",\"icon\":\"/temp/iconcache/office/16x16/link.png\",\"disabledIcon\":\"/temp/link_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Insert a link into the text field.\",\"type\":null},{\"click\":\"chrome:field:removelink\",\"header\":\"\",\"icon\":\"/temp/iconcache/office/16x16/link_broken.png\",\"disabledIcon\":\"/temp/link_broken_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Remove link.\",\"type\":null},{\"click\":\"chrome:field:insertimage\",\"header\":\"Insert image\",\"icon\":\"/temp/iconcache/office/16x16/photo_landscape.png\",\"disabledIcon\":\"/temp/photo_landscape_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Insert an image into the text field.\",\"type\":null},{\"click\":\"chrome:common:edititem({command:\\\"webedit:open\\\"})\",\"header\":\"Edit the related item\",\"icon\":\"/temp/iconcache/office/16x16/cubes.png\",\"disabledIcon\":\"/temp/cubes_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the related item in the Content Editor.\",\"type\":\"common\"},{\"click\":\"chrome:rendering:personalize({command:\\\"webedit:personalize\\\"})\",\"header\":\"Personalize\",\"icon\":\"/temp/iconcache/office/16x16/users_family.png\",\"disabledIcon\":\"/temp/users_family_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Create or edit personalization for this component.\",\"type\":\"sticky\"}],\"contextItemUri\":\"sitecore://master/{A2484483-AF6F-5723-A29F-785E12CED97B}?lang=en&ver=1\",\"custom\":{},\"displayName\":\"content\",\"expandedDisplayName\":null}</span><span scFieldType=\"rich text\" scDefaultText=\"[No text in field]\" contenteditable=\"true\" class=\"scWebEditInput\" id=\"fld_A2484483AF6F5723A29F785E12CED97B_6856AF27B4135FCEB3FDC560612F1199_en_1_c950fc1bd5484df88dc99bce389d51a0_393_edit\">" +
                                        TestConstants.RichTextFieldValue1 + "</span>"
                                },
                                ["RichTextField2"] = new RichTextField(TestConstants.RichTextFieldValue2)
                                {
                                    EditableMarkup =
                                        "<input id='fld_A2484483AF6F5723A29F785E12CED97B_6856AF27B4135FCEB3FDC560612F1199_en_1_c950fc1bd5484df88dc99bce389d51a0_393' class='scFieldValue' name='fld_A2484483AF6F5723A29F785E12CED97B_6856AF27B4135FCEB3FDC560612F1199_en_1_c950fc1bd5484df88dc99bce389d51a0_393' type='hidden' value=\"" +
                                        HtmlEncoder.Default.Encode(TestConstants.RichTextFieldValue2) +
                                        "\" /><span class=\"scChromeData\">{\"contextItem\":{\"id\":\"a2484483-af6f-5723-a29f-785e12ced97b\",\"version\":1,\"language\":\"en\",\"revision\":\"c950fc1bd5484df88dc99bce389d51a0\"},\"fieldId\":\"6856af27-b413-5fce-b3fd-c560612f1199\",\"fieldType\":\"Rich Text\",\"fieldWebEditParameters\":{},\"commands\":[{\"click\":\"chrome:field:editcontrol({command:\\\"webedit:edithtml\\\"})\",\"header\":\"Edit Text\",\"icon\":\"/temp/iconcache/office/16x16/pencil.png\",\"disabledIcon\":\"/temp/pencil_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the text\",\"type\":null},{\"click\":\"chrome:field:execute({command:\\\"bold\\\", userInterface:true, value:true})\",\"header\":\"\",\"icon\":\"/temp/iconcache/office/16x16/font_style_bold.png\",\"disabledIcon\":\"/temp/font_style_bold_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Bold\",\"type\":null},{\"click\":\"chrome:field:execute({command:\\\"Italic\\\", userInterface:true, value:true})\",\"header\":\"\",\"icon\":\"/temp/iconcache/office/16x16/font_style_italics.png\",\"disabledIcon\":\"/temp/font_style_italics_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Italic\",\"type\":null},{\"click\":\"chrome:field:execute({command:\\\"Underline\\\", userInterface:true, value:true})\",\"header\":\"\",\"icon\":\"/temp/iconcache/office/16x16/font_style_underline.png\",\"disabledIcon\":\"/temp/font_style_underline_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Underline\",\"type\":null},{\"click\":\"chrome:field:insertexternallink\",\"header\":\"\",\"icon\":\"/temp/iconcache/office/16x16/earth_link.png\",\"disabledIcon\":\"/temp/earth_link_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Insert an external link into the text field.\",\"type\":null},{\"click\":\"chrome:field:insertlink\",\"header\":\"\",\"icon\":\"/temp/iconcache/office/16x16/link.png\",\"disabledIcon\":\"/temp/link_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Insert a link into the text field.\",\"type\":null},{\"click\":\"chrome:field:removelink\",\"header\":\"\",\"icon\":\"/temp/iconcache/office/16x16/link_broken.png\",\"disabledIcon\":\"/temp/link_broken_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Remove link.\",\"type\":null},{\"click\":\"chrome:field:insertimage\",\"header\":\"Insert image\",\"icon\":\"/temp/iconcache/office/16x16/photo_landscape.png\",\"disabledIcon\":\"/temp/photo_landscape_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Insert an image into the text field.\",\"type\":null},{\"click\":\"chrome:common:edititem({command:\\\"webedit:open\\\"})\",\"header\":\"Edit the related item\",\"icon\":\"/temp/iconcache/office/16x16/cubes.png\",\"disabledIcon\":\"/temp/cubes_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the related item in the Content Editor.\",\"type\":\"common\"},{\"click\":\"chrome:rendering:personalize({command:\\\"webedit:personalize\\\"})\",\"header\":\"Personalize\",\"icon\":\"/temp/iconcache/office/16x16/users_family.png\",\"disabledIcon\":\"/temp/users_family_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Create or edit personalization for this component.\",\"type\":\"sticky\"}],\"contextItemUri\":\"sitecore://master/{A2484483-AF6F-5723-A29F-785E12CED97B}?lang=en&ver=1\",\"custom\":{},\"displayName\":\"content\",\"expandedDisplayName\":null}</span><span scFieldType=\"rich text\" scDefaultText=\"[No text in field]\" contenteditable=\"true\" class=\"scWebEditInput\" id=\"fld_A2484483AF6F5723A29F785E12CED97B_6856AF27B4135FCEB3FDC560612F1199_en_1_c950fc1bd5484df88dc99bce389d51a0_393_edit\">" +
                                        TestConstants.RichTextFieldValue2 + "</span>"
                                }
                            }
                        },

                        new EditableChrome
                        {
                            Name = "code",
                            Type = "text/sitecore",
                            Content = string.Empty,
                            Attributes = new Dictionary<string, string>
                            {
                                { "type", "text/sitecore" },
                                { "chrometype", "rendering" },
                                { "kind", "close" },
                                { "id", "scEnclosingTag_r_" },
                                { "hintkey", "Component 4" },
                                { "class", "scpm" }
                            }
                        },

                        new EditableChrome
                        {
                            Name = "code",
                            Type = "text/sitecore",
                            Content = string.Empty,
                            Attributes = new Dictionary<string, string>
                            {
                                { "type", "text/sitecore" },
                                { "chrometype", "placeholder" },
                                { "kind", "close" },
                                { "id", "scEnclosingTag_" },
                                { "hintname", "Placeholder-1" },
                                { "class", "scpm" }
                            }
                        }
                    ]
                }
            },
            Devices =
            [
                new Device
                {
                    Id = "fe5d7fdf-89c0-4d99-9aa3-b5fbd009c9f3",
                    LayoutId = "14030e9f-ce92-49c6-ad87-7d49b50e42ea",
                    Renderings =
                    [
                        new Rendering
                        {
                            Id = "885b8314-7d8c-4cbb-8000-01421ea8f406",
                            InstanceId = "43222d12-08c9-453b-ae96-d406ebb95126",
                            PlaceholderKey = "main"
                        },

                        new Rendering
                        {
                            Id = "ce4adcfb-7990-4980-83fb-a00c1e3673db",
                            InstanceId = "cf044ad9-0332-407a-abde-587214a2c808",
                            PlaceholderKey = "/main/centercolumn"
                        },

                        new Rendering
                        {
                            Id = "493b3a83-0fa7-4484-8fc9-4680991cf743",
                            InstanceId = "b343725a-3a93-446e-a9c8-3a2cbd3db489",
                            PlaceholderKey = "/main/centercolumn/content"
                        }
                    ]
                },

                new Device
                {
                    Id = "46d2f427-4ce5-4e1f-ba10-ef3636f43534",
                    LayoutId = "14030e9f-ce92-49c6-ad87-7d49b50e42ea",
                    Renderings =
                    [
                        new Rendering
                        {
                            Id = "493b3a83-0fa7-4484-8fc9-4680991cf743",
                            InstanceId = "a08c9132-dbd1-474f-a2ca-6ca26a4aa650",
                            PlaceholderKey = "content"
                        }
                    ]
                }
            ]
        }
    };

    public static SitecoreLayoutResponseContent PageWithPreview => new()
    {
        Sitecore = new SitecoreData
        {
            Context = new Context
            {
                Language = TestConstants.Language,
                IsEditing = false,
                PageState = PageState.Preview,
                Site = new Site
                {
                    Name = "JssDisconnectedLayoutService"
                }
            },
            Route = new Route
            {
                LayoutId = TestConstants.NestedPlaceholderPageLayoutId,

                DatabaseName = TestConstants.DatabaseName,
                DeviceId = "test-device-id",
                ItemId = TestConstants.TestItemId,
                ItemLanguage = "en",
                ItemVersion = 1,
                TemplateId = "test-template-id",
                TemplateName = "test-template-name",
                Name = "styleguide",
                Fields =
                {
                    ["pageTitle"] = new TextField(TestConstants.PageTitle),
                    ["searchKeywords"] = new TextField(TestConstants.SearchKeywords)
                },
                Placeholders =
                {
                    ["placeholder-1"] =
                    [
                        new Component
                        {
                            Id = "{34A6553C-81DE-5CD3-989E-853F6CB6DF8C}",
                            Name = "Component-1",
                            Placeholders =
                            {
                                ["placeholder-2"] =
                                [
                                    new Component
                                    {
                                        Id = "{B7C779DA-2B75-586C-B40D-081FCB864256}",
                                        Name = "Component-2",
                                        Fields =
                                        {
                                            ["TestField"] = new RichTextField(TestConstants.RichTextFieldValue1)
                                        }
                                    }
                                ]
                            }
                        },

                        new Component
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = "Component-3",
                            Fields =
                            {
                                ["TestField"] = new TextField(TestConstants.TestFieldValue),
                                ["EmptyField"] = new TextField(string.Empty),
                                ["NullValueField"] = new TextField(null!),
                                ["MultiLineField"] = new TextField(TestConstants.TestMultilineFieldValue),
                                ["EncodedField"] = new TextField(TestConstants.RichTextFieldValue1)
                            }
                        },

                        new Component
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = "Component-4",
                            Fields =
                            {
                                ["Date"] = new DateField(DateTime.Parse("12.12.19", CultureInfo.InvariantCulture)),
                                ["RichTextField1"] = new RichTextField(TestConstants.RichTextFieldValue1),
                                ["RichTextField2"] = new RichTextField(TestConstants.RichTextFieldValue2),
                                ["EmptyField"] = new RichTextField(string.Empty),
                                ["NullValueField"] = new RichTextField(null!),
                                ["TestField"] = new TextField(TestConstants.TestFieldValue)
                            }
                        },

                        new Component
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = "Component-5",
                            Fields =
                            {
                                ["TestField"] = new TextField(TestConstants.TestFieldValue),
                                ["EmptyField"] = new TextField(string.Empty),
                                ["NullValueField"] = new TextField(null!),
                                ["MultiLineField"] = new TextField(TestConstants.TestMultilineFieldValue),
                                ["Date"] = new DateField(DateTime.Parse("12.12.19", CultureInfo.InvariantCulture))
                            }
                        },

                        new Component
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = "Component-6",
                            Fields =
                            {
                                ["TestField"] = new TextField(TestConstants.TestFieldValue + " from Component-6")
                            }
                        },

                        new Component
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = "Complex-Component",
                            Fields =
                            {
                                ["Header"] = new TextField(TestConstants.PageTitle),
                                ["Content"] = new RichTextField(TestConstants.RichTextFieldValue1),
                                ["Header2"] = new TextField(TestConstants.TestFieldValue),
                                ["CustomField"] = new Models.CustomFieldType(TestConstants.TestFieldValue, "custom")
                            }
                        },

                        new Component
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = "Component-With-All-Field-Types",
                            Fields =
                            {
                                ["TextField"] = new TextField(TestConstants.TestFieldValue),
                                ["MultiLineField"] = new TextField(TestConstants.TestMultilineFieldValue),
                                ["RichTextField1"] = new RichTextField(TestConstants.RichTextFieldValue1),
                                ["RichTextField2"] = new RichTextField(TestConstants.RichTextFieldValue2),
                                ["LinkField"] = new HyperLinkField(new HyperLink
                                {
                                    Href = "/", Text = "Sample Link", Class = "sample", Target = "_blank",
                                    Title = "title"
                                }),
                                ["ImageField"] = new ImageField(new Image { Alt = "sample", Src = "sample.png" }),
                                ["NumberField"] = new NumberField(9.99)
                            }
                        },

                        new Component
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = "Component-With-Links",
                            Fields =
                            {
                                ["internalLink"] = new HyperLinkField(new HyperLink
                                    { Href = "/", Text = "This text should be ignored" }),
                                ["paramsLink"] = new HyperLinkField(new HyperLink
                                {
                                    Href = "https://dev.sitecore.net",
                                    Text = "Sitecore Dev Site",
                                    Target = "_blank",
                                    Class = "font-weight-bold",
                                    Title = "title attribute"
                                }),
                                ["text"] = new TextField(TestConstants.TestFieldValue),
                            }
                        },

                        new Component
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = "Component-With-Images",
                            Fields =
                            {
                                ["FirstImage"] = new ImageField(new Image { Alt = "sample", Src = "sample.png" }),
                                ["SecondImage"] = new ImageField(new Image
                                    { Alt = "second", Src = "site/second.png" }),
                                ["Heading"] = new TextField(TestConstants.TestFieldValue),
                            }
                        },

                        new Component
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = "Component-With-Number",
                            Fields =
                            {
                                ["number"] = new NumberField(1.21),
                                ["text"] = new TextField(TestConstants.TestFieldValue)
                            }
                        }
                    ]
                }
            }
        }
    };

    // ReSharper disable once MemberCanBePrivate.Global - Must be available
    public static class SitecoreLayoutIds
    {
        public const string Styleguide1LayoutId = "{E02DDB9B-A062-5E50-924A-1940D7E053C1}";

        public const string Styleguide2LayoutId = "{E02DDB9B-A062-5E50-924A-1940D7E053C2}";
    }
}