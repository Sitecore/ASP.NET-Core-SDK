using System.Text;
using AutoFixture;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.AutoFixture.Extensions;
using Sitecore.AspNetCore.SDK.ExperienceEditor.Configuration;
using Sitecore.AspNetCore.SDK.ExperienceEditor.Middleware;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Serialization;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Xunit;

namespace Sitecore.AspNetCore.SDK.ExperienceEditor.Tests.Middleware;

public class ExperienceEditorMiddlewareFixture
{
    private const string EeSampleRequest =
        """{"id":"jssdevex","args":["/?sc_httprenderengineurl=https%3a%2f%2f8eeabadd.ngrok.io","{\"sitecore\":{\"context\":{\"pageEditing\":false,\"site\":{\"name\":\"jssdevex\"},\"pageState\":\"normal\",\"language\":\"en\"},\"route\":{\"name\":\"home\",\"displayName\":\"home\",\"fields\":{\"pageTitle\":{\"value\":\"Welcome to Sitecore JSS\"}},\"databaseName\":\"master\",\"deviceId\":\"fe5d7fdf-89c0-4d99-9aa3-b5fbd009c9f3\",\"itemId\":\"4e8410b0-28c5-52c5-8439-12a1ab247560\",\"itemLanguage\":\"en\",\"itemVersion\":1,\"layoutId\":\"80848506-1859-5f78-8fc6-f692c0c49795\",\"templateId\":\"6c0659f1-c66d-5877-a83b-510b6e0c64a2\",\"templateName\":\"App Route\",\"placeholders\":{\"jss-main\":[{\"uid\":\"2c4a53cc-9da8-5f51-9d79-6ee2fc671b2d\",\"componentName\":\"ContentBlock\",\"dataSource\":\"{695CF95F-3E00-5B9F-A090-EB9C6D666DB5}\",\"params\":{},\"fields\":{\"heading\":{\"value\":\"Welcome to Sitecore JSS\"},\"content\":{\"value\":\"<p>Thanks for using JSS!! Here are some resources to get you started:</p>\\n\\n<h3><a href=\\\"https://jss.sitecore.net\\\" rel=\\\"noopener noreferrer\\\">Documentation</a></h3>\\n<p>The official JSS documentation can help you with any JSS task from getting started to advanced techniques.</p>\\n\\n<h3><a href=\\\"/styleguide\\\">Styleguide</a></h3>\\n<p>The JSS styleguide is a living example of how to use JSS, hosted right in this app.\\nIt demonstrates most of the common patterns that JSS implementations may need to use,\\nas well as useful architectural patterns.</p>\\n\\n<h3><a href=\\\"/graphql\\\">GraphQL</a></h3>\\n<p>JSS features integration with the Sitecore GraphQL API to enable fetching non-route data from Sitecore - or from other internal backends as an API aggregator or proxy.\\nThis route is a living example of how to use an integrate with GraphQL data in a JSS app.</p>\\n\\n<div class=\\\"alert alert-dark\\\">\\n  <h4>This app is a boilerplate</h4>\\n  <p>The JSS samples are a boilerplate, not a library. That means that any code in this app is meant for you to own and customize to your own requirements.</p>\\n  <p>Want to change the lint settings? Do it. Want to read manifest data from a MongoDB database? Go for it. This app is yours.</p>\\n</div>\\n\\n<div class=\\\"alert alert-dark\\\">\\n  <h4>How to start with an empty app</h4>\\n  <p>To remove all of the default sample content (the Styleguide and GraphQL routes) and start out with an empty JSS app:</p>\\n  <ol>\\n    <li>Delete <code>/src/components/Styleguide*</code> and <code>/src/components/GraphQL*</code></li>\\n    <li>Delete <code>/sitecore/definitions/components/Styleguide*</code>, <code>/sitecore/definitions/templates/Styleguide*</code>, and <code>/sitecore/definitions/components/GraphQL*</code></li>\\n    <li>Delete <code>/data/component-content/Styleguide</code></li>\\n    <li>Delete <code>/data/content/Styleguide</code></li>\\n    <li>Delete <code>/data/routes/styleguide</code> and <code>/data/routes/graphql</code></li>\\n    <li>Delete <code>/data/dictionary/*.yml</code></li>\\n  </ol>\\n</div>\\n\"}}}]}}}}","{\"language\":\"en\",\"dictionary\":{\"Documentation\":\"Documentation\",\"GraphQL\":\"GraphQL\",\"Styleguide\":\"Styleguide\",\"styleguide-sample\":\"This is a dictionary entry in English as a demonstration\"},\"httpContext\":{\"request\":{\"url\":\"https://jssdevex.dev.local:443/?sc_httprenderengineurl=https://8eeabadd.ngrok.io\",\"path\":\"/Home/Sample\",\"querystring\":{\"sc_httprenderengineurl\":\"https://8eeabadd.ngrok.io\"},\"userAgent\":\"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.100 Safari/537.36 Edg/80.0.361.53\"}}}"],"functionName":"renderView","moduleName":"server.bundle","jssEditingSecret":"mysecret"}""";

    private const string EditRequest =
        """{"id":"jss-sample-app","args":["/?sc_itemid=%7bcfdd7ba2-e646-5294-87fc-6fad34451a97%7d&sc_ee_fb=false&sc_lang=en","{\"sitecore\":{\"context\":{\"pageEditing\":true,\"user\":{\"domain\":\"sitecore\",\"name\":\"Admin\"},\"site\":{\"name\":\"jss-sample-app\"},\"pageState\":\"edit\",\"language\":\"en\",\"itemPath\":\"/graphql/sample-1\"},\"route\":{\"name\":\"sample-1\",\"displayName\":\"sample-1\",\"fields\":{\"pageTitle\":{\"value\":\"Sample 1 Page Title\",\"editable\":\"<input id='fld_CFDD7BA2E646529487FC6FAD34451A97_EB294DC3969B59CDA1932B5F18DB8776_en_1_722e36f6799b4e229ee00a77a5c65332_326' class='scFieldValue' name='fld_CFDD7BA2E646529487FC6FAD34451A97_EB294DC3969B59CDA1932B5F18DB8776_en_1_722e36f6799b4e229ee00a77a5c65332_326' type='hidden' value=\\\"Sample 1 Page Title\\\" /><span class=\\\"scChromeData\\\">{\\\"contextItem\\\":{\\\"id\\\":\\\"cfdd7ba2-e646-5294-87fc-6fad34451a97\\\",\\\"version\\\":1,\\\"language\\\":\\\"en\\\",\\\"revision\\\":\\\"722e36f6799b4e229ee00a77a5c65332\\\"},\\\"fieldId\\\":\\\"eb294dc3-969b-59cd-a193-2b5f18db8776\\\",\\\"fieldType\\\":\\\"Single-Line Text\\\",\\\"fieldWebEditParameters\\\":{\\\"prevent-line-break\\\":\\\"true\\\"},\\\"commands\\\":[{\\\"click\\\":\\\"chrome:common:edititem({command:\\\\\\\"webedit:open\\\\\\\"})\\\",\\\"header\\\":\\\"Edit the related item\\\",\\\"icon\\\":\\\"/temp/iconcache/office/16x16/cubes.png\\\",\\\"disabledIcon\\\":\\\"/temp/cubes_disabled16x16.png\\\",\\\"isDivider\\\":false,\\\"tooltip\\\":\\\"Edit the related item in the Content Editor.\\\",\\\"type\\\":\\\"common\\\"},{\\\"click\\\":\\\"chrome:rendering:personalize({command:\\\\\\\"webedit:personalize\\\\\\\"})\\\",\\\"header\\\":\\\"Personalize\\\",\\\"icon\\\":\\\"/temp/iconcache/office/16x16/users_family.png\\\",\\\"disabledIcon\\\":\\\"/temp/users_family_disabled16x16.png\\\",\\\"isDivider\\\":false,\\\"tooltip\\\":\\\"Create or edit personalization for this component.\\\",\\\"type\\\":\\\"sticky\\\"}],\\\"contextItemUri\\\":\\\"sitecore://master/{CFDD7BA2-E646-5294-87FC-6FAD34451A97}?lang=en&ver=1\\\",\\\"custom\\\":{},\\\"displayName\\\":\\\"Page Title\\\",\\\"expandedDisplayName\\\":null}</span><span id=\\\"fld_CFDD7BA2E646529487FC6FAD34451A97_EB294DC3969B59CDA1932B5F18DB8776_en_1_722e36f6799b4e229ee00a77a5c65332_326_edit\\\" sc_parameters=\\\"prevent-line-break=true\\\" contenteditable=\\\"true\\\" class=\\\"scWebEditInput\\\" scFieldType=\\\"single-line text\\\" scDefaultText=\\\"[No text in field]\\\">Sample 1 Page Title</span>\"}},\"databaseName\":\"master\",\"deviceId\":\"fe5d7fdf-89c0-4d99-9aa3-b5fbd009c9f3\",\"itemId\":\"cfdd7ba2-e646-5294-87fc-6fad34451a97\",\"itemLanguage\":\"en\",\"itemVersion\":1,\"layoutId\":\"5179e218-3df6-5af7-8147-d2d4c05da992\",\"templateId\":\"dfe73d70-9835-584e-b0f5-28c58ab064d7\",\"templateName\":\"App Route\",\"placeholders\":{\"jss-main\":[{\"name\":\"code\",\"type\":\"text/sitecore\",\"contents\":\"{\\\"contextItem\\\":{\\\"id\\\":\\\"cfdd7ba2-e646-5294-87fc-6fad34451a97\\\",\\\"version\\\":1,\\\"language\\\":\\\"en\\\",\\\"revision\\\":\\\"722e36f6799b4e229ee00a77a5c65332\\\"},\\\"placeholderKey\\\":\\\"jss-main\\\",\\\"placeholderMetadataKeys\\\":[\\\"jss-main\\\"],\\\"editable\\\":true,\\\"commands\\\":[{\\\"click\\\":\\\"chrome:placeholder:addControl\\\",\\\"header\\\":\\\"Add to here\\\",\\\"icon\\\":\\\"/temp/iconcache/office/16x16/add.png\\\",\\\"disabledIcon\\\":\\\"/temp/add_disabled16x16.png\\\",\\\"isDivider\\\":false,\\\"tooltip\\\":\\\"Add a new rendering to the '{0}' placeholder.\\\",\\\"type\\\":\\\"\\\"},{\\\"click\\\":\\\"chrome:placeholder:editSettings\\\",\\\"header\\\":\\\"\\\",\\\"icon\\\":\\\"/temp/iconcache/office/16x16/window_gear.png\\\",\\\"disabledIcon\\\":\\\"/temp/window_gear_disabled16x16.png\\\",\\\"isDivider\\\":false,\\\"tooltip\\\":\\\"Edit the placeholder settings.\\\",\\\"type\\\":\\\"\\\"}],\\\"contextItemUri\\\":\\\"sitecore://master/{CFDD7BA2-E646-5294-87FC-6FAD34451A97}?lang=en&ver=1\\\",\\\"custom\\\":{\\\"allowedRenderings\\\":[\\\"EF310C848E2655329A11DB68DCF37BEF\\\",\\\"E2FA6BCC80A05FC4B71BC05B597A4060\\\",\\\"F0ED8D3C014657EB83EFF8AA2A351BA6\\\",\\\"603EAA1A61D05B7185CA346D9B55FECC\\\"],\\\"editable\\\":\\\"true\\\"},\\\"displayName\\\":\\\"Main\\\",\\\"expandedDisplayName\\\":null}\",\"attributes\":{\"type\":\"text/sitecore\",\"chrometype\":\"placeholder\",\"kind\":\"open\",\"id\":\"jss_main\",\"key\":\"jss-main\",\"class\":\"scpm\",\"data-selectable\":\"true\"}},{\"name\":\"code\",\"type\":\"text/sitecore\",\"contents\":\"{\\\"contextItem\\\":{\\\"id\\\":\\\"27f322af-3d30-5f2f-ada2-dab630d35ec6\\\",\\\"version\\\":1,\\\"language\\\":\\\"en\\\",\\\"revision\\\":\\\"ad0b2a91d2c5484facf59feb6b49dff1\\\"},\\\"renderingId\\\":\\\"ef310c84-8e26-5532-9a11-db68dcf37bef\\\",\\\"renderingInstanceId\\\":\\\"{9157CC8C-5760-5114-9F2D-93CBE39B30DC}\\\",\\\"editable\\\":true,\\\"commands\\\":[{\\\"click\\\":\\\"javascript:Sitecore.PageModes.PageEditor.postRequest('webedit:fieldeditor(command={2F7D34E2-EB93-47ED-8177-B1F429A9FCD7},fields=heading|content,id={27F322AF-3D30-5F2F-ADA2-DAB630D35EC6})',null,false)\\\",\\\"header\\\":\\\"Edit Fields\\\",\\\"icon\\\":\\\"/temp/iconcache/office/16x16/pencil.png\\\",\\\"disabledIcon\\\":\\\"/temp/pencil_disabled16x16.png\\\",\\\"isDivider\\\":false,\\\"tooltip\\\":\\\"\\\",\\\"type\\\":null},{\\\"click\\\":\\\"chrome:dummy\\\",\\\"header\\\":\\\"Separator\\\",\\\"icon\\\":\\\"\\\",\\\"disabledIcon\\\":\\\"\\\",\\\"isDivider\\\":false,\\\"tooltip\\\":null,\\\"type\\\":\\\"separator\\\"},{\\\"click\\\":\\\"chrome:rendering:sort\\\",\\\"header\\\":\\\"Change position\\\",\\\"icon\\\":\\\"/temp/iconcache/office/16x16/document_size.png\\\",\\\"disabledIcon\\\":\\\"/temp/document_size_disabled16x16.png\\\",\\\"isDivider\\\":false,\\\"tooltip\\\":\\\"Move component.\\\",\\\"type\\\":\\\"\\\"},{\\\"click\\\":\\\"javascript:Sitecore.PageModes.PageEditor.postRequest('webedit:componentoptions(referenceId={9157CC8C-5760-5114-9F2D-93CBE39B30DC},renderingId={EF310C84-8E26-5532-9A11-DB68DCF37BEF},id={27F322AF-3D30-5F2F-ADA2-DAB630D35EC6})',null,false)\\\",\\\"header\\\":\\\"Edit Experience Editor Options\\\",\\\"icon\\\":\\\"/temp/iconcache/office/16x16/clipboard_check_edit.png\\\",\\\"disabledIcon\\\":\\\"/temp/clipboard_check_edit_disabled16x16.png\\\",\\\"isDivider\\\":false,\\\"tooltip\\\":\\\"Edit the Experience Editor options for the component.\\\",\\\"type\\\":\\\"common\\\"},{\\\"click\\\":\\\"chrome:rendering:properties\\\",\\\"header\\\":\\\"Edit component properties\\\",\\\"icon\\\":\\\"/temp/iconcache/office/16x16/elements_branch.png\\\",\\\"disabledIcon\\\":\\\"/temp/elements_branch_disabled16x16.png\\\",\\\"isDivider\\\":false,\\\"tooltip\\\":\\\"Edit the properties for the component.\\\",\\\"type\\\":\\\"common\\\"},{\\\"click\\\":\\\"javascript:Sitecore.PageModes.PageEditor.postRequest('webedit:setdatasource(referenceId={9157CC8C-5760-5114-9F2D-93CBE39B30DC},renderingId={EF310C84-8E26-5532-9A11-DB68DCF37BEF},id={27F322AF-3D30-5F2F-ADA2-DAB630D35EC6})',null,false)\\\",\\\"header\\\":\\\"dsHeaderParameter\\\",\\\"icon\\\":\\\"/temp/iconcache/office/16x16/data.png\\\",\\\"disabledIcon\\\":\\\"/temp/data_disabled16x16.png\\\",\\\"isDivider\\\":false,\\\"tooltip\\\":\\\"dsTooltipParameter\\\",\\\"type\\\":\\\"datasourcesmenu\\\"},{\\\"click\\\":\\\"chrome:rendering:personalize({command:\\\\\\\"webedit:personalize\\\\\\\"})\\\",\\\"header\\\":\\\"Personalize\\\",\\\"icon\\\":\\\"/temp/iconcache/office/16x16/users_family.png\\\",\\\"disabledIcon\\\":\\\"/temp/users_family_disabled16x16.png\\\",\\\"isDivider\\\":false,\\\"tooltip\\\":\\\"Create or edit personalization for this component.\\\",\\\"type\\\":\\\"sticky\\\"},{\\\"click\\\":\\\"chrome:common:edititem({command:\\\\\\\"webedit:open\\\\\\\"})\\\",\\\"header\\\":\\\"Edit the related item\\\",\\\"icon\\\":\\\"/temp/iconcache/office/16x16/cubes.png\\\",\\\"disabledIcon\\\":\\\"/temp/cubes_disabled16x16.png\\\",\\\"isDivider\\\":false,\\\"tooltip\\\":\\\"Edit the related item in the Content Editor.\\\",\\\"type\\\":\\\"datasourcesmenu\\\"},{\\\"click\\\":\\\"chrome:rendering:delete\\\",\\\"header\\\":\\\"Delete\\\",\\\"icon\\\":\\\"/temp/iconcache/office/16x16/delete.png\\\",\\\"disabledIcon\\\":\\\"/temp/delete_disabled16x16.png\\\",\\\"isDivider\\\":false,\\\"tooltip\\\":\\\"Remove component.\\\",\\\"type\\\":\\\"sticky\\\"}],\\\"contextItemUri\\\":\\\"sitecore://master/{27F322AF-3D30-5F2F-ADA2-DAB630D35EC6}?lang=en&ver=1\\\",\\\"custom\\\":{\\\"renderingID\\\":\\\"EF310C848E2655329A11DB68DCF37BEF\\\",\\\"editable\\\":\\\"true\\\"},\\\"displayName\\\":\\\"Content Block\\\",\\\"expandedDisplayName\\\":null}\",\"attributes\":{\"type\":\"text/sitecore\",\"chrometype\":\"rendering\",\"kind\":\"open\",\"hintname\":\"Content Block\",\"id\":\"r_9157CC8C576051149F2D93CBE39B30DC\",\"class\":\"scpm\",\"data-selectable\":\"true\"}},{\"uid\":\"9157cc8c-5760-5114-9f2d-93cbe39b30dc\",\"componentName\":\"ContentBlock\",\"dataSource\":\"{27F322AF-3D30-5F2F-ADA2-DAB630D35EC6}\",\"params\":{},\"fields\":{\"heading\":{\"value\":\"GraphQL Sample 1\",\"editable\":\"<input id='fld_27F322AF3D305F2FADA2DAB630D35EC6_FDA8C2D475365B22A75E2A8ED09C6302_en_1_ad0b2a91d2c5484facf59feb6b49dff1_327' class='scFieldValue' name='fld_27F322AF3D305F2FADA2DAB630D35EC6_FDA8C2D475365B22A75E2A8ED09C6302_en_1_ad0b2a91d2c5484facf59feb6b49dff1_327' type='hidden' value=\\\"GraphQL Sample 1\\\" /><span class=\\\"scChromeData\\\">{\\\"contextItem\\\":{\\\"id\\\":\\\"27f322af-3d30-5f2f-ada2-dab630d35ec6\\\",\\\"version\\\":1,\\\"language\\\":\\\"en\\\",\\\"revision\\\":\\\"ad0b2a91d2c5484facf59feb6b49dff1\\\"},\\\"fieldId\\\":\\\"fda8c2d4-7536-5b22-a75e-2a8ed09c6302\\\",\\\"fieldType\\\":\\\"Single-Line Text\\\",\\\"fieldWebEditParameters\\\":{\\\"prevent-line-break\\\":\\\"true\\\"},\\\"commands\\\":[{\\\"click\\\":\\\"chrome:common:edititem({command:\\\\\\\"webedit:open\\\\\\\"})\\\",\\\"header\\\":\\\"Edit the related item\\\",\\\"icon\\\":\\\"/temp/iconcache/office/16x16/cubes.png\\\",\\\"disabledIcon\\\":\\\"/temp/cubes_disabled16x16.png\\\",\\\"isDivider\\\":false,\\\"tooltip\\\":\\\"Edit the related item in the Content Editor.\\\",\\\"type\\\":\\\"common\\\"},{\\\"click\\\":\\\"chrome:rendering:personalize({command:\\\\\\\"webedit:personalize\\\\\\\"})\\\",\\\"header\\\":\\\"Personalize\\\",\\\"icon\\\":\\\"/temp/iconcache/office/16x16/users_family.png\\\",\\\"disabledIcon\\\":\\\"/temp/users_family_disabled16x16.png\\\",\\\"isDivider\\\":false,\\\"tooltip\\\":\\\"Create or edit personalization for this component.\\\",\\\"type\\\":\\\"sticky\\\"}],\\\"contextItemUri\\\":\\\"sitecore://master/{27F322AF-3D30-5F2F-ADA2-DAB630D35EC6}?lang=en&ver=1\\\",\\\"custom\\\":{},\\\"displayName\\\":\\\"heading\\\",\\\"expandedDisplayName\\\":null}</span><span id=\\\"fld_27F322AF3D305F2FADA2DAB630D35EC6_FDA8C2D475365B22A75E2A8ED09C6302_en_1_ad0b2a91d2c5484facf59feb6b49dff1_327_edit\\\" sc_parameters=\\\"prevent-line-break=true\\\" contenteditable=\\\"true\\\" class=\\\"scWebEditInput\\\" scFieldType=\\\"single-line text\\\" scDefaultText=\\\"[No text in field]\\\">GraphQL Sample 1</span>\"},\"content\":{\"value\":\"<p>A child route here to illustrate the power of GraphQL queries. <a href=\\\"/graphql\\\">Back to GraphQL route</a></p>\\n\",\"editable\":\"<input id='fld_27F322AF3D305F2FADA2DAB630D35EC6_A40366CE32D25868A373A20EC9C51573_en_1_ad0b2a91d2c5484facf59feb6b49dff1_328' class='scFieldValue' name='fld_27F322AF3D305F2FADA2DAB630D35EC6_A40366CE32D25868A373A20EC9C51573_en_1_ad0b2a91d2c5484facf59feb6b49dff1_328' type='hidden' value=\\\"&lt;p&gt;A child route here to illustrate the power of GraphQL queries. &lt;a href=&quot;/graphql&quot;&gt;Back to GraphQL route&lt;/a&gt;&lt;/p&gt;\\n\\\" /><span class=\\\"scChromeData\\\">{\\\"contextItem\\\":{\\\"id\\\":\\\"27f322af-3d30-5f2f-ada2-dab630d35ec6\\\",\\\"version\\\":1,\\\"language\\\":\\\"en\\\",\\\"revision\\\":\\\"ad0b2a91d2c5484facf59feb6b49dff1\\\"},\\\"fieldId\\\":\\\"a40366ce-32d2-5868-a373-a20ec9c51573\\\",\\\"fieldType\\\":\\\"Rich Text\\\",\\\"fieldWebEditParameters\\\":{},\\\"commands\\\":[{\\\"click\\\":\\\"chrome:field:editcontrol({command:\\\\\\\"webedit:edithtml\\\\\\\"})\\\",\\\"header\\\":\\\"Edit Text\\\",\\\"icon\\\":\\\"/temp/iconcache/office/16x16/pencil.png\\\",\\\"disabledIcon\\\":\\\"/temp/pencil_disabled16x16.png\\\",\\\"isDivider\\\":false,\\\"tooltip\\\":\\\"Edit the text\\\",\\\"type\\\":null},{\\\"click\\\":\\\"chrome:field:execute({command:\\\\\\\"bold\\\\\\\", userInterface:true, value:true})\\\",\\\"header\\\":\\\"\\\",\\\"icon\\\":\\\"/temp/iconcache/office/16x16/font_style_bold.png\\\",\\\"disabledIcon\\\":\\\"/temp/font_style_bold_disabled16x16.png\\\",\\\"isDivider\\\":false,\\\"tooltip\\\":\\\"Bold\\\",\\\"type\\\":null},{\\\"click\\\":\\\"chrome:field:execute({command:\\\\\\\"Italic\\\\\\\", userInterface:true, value:true})\\\",\\\"header\\\":\\\"\\\",\\\"icon\\\":\\\"/temp/iconcache/office/16x16/font_style_italics.png\\\",\\\"disabledIcon\\\":\\\"/temp/font_style_italics_disabled16x16.png\\\",\\\"isDivider\\\":false,\\\"tooltip\\\":\\\"Italic\\\",\\\"type\\\":null},{\\\"click\\\":\\\"chrome:field:execute({command:\\\\\\\"Underline\\\\\\\", userInterface:true, value:true})\\\",\\\"header\\\":\\\"\\\",\\\"icon\\\":\\\"/temp/iconcache/office/16x16/font_style_underline.png\\\",\\\"disabledIcon\\\":\\\"/temp/font_style_underline_disabled16x16.png\\\",\\\"isDivider\\\":false,\\\"tooltip\\\":\\\"Underline\\\",\\\"type\\\":null},{\\\"click\\\":\\\"chrome:field:insertexternallink\\\",\\\"header\\\":\\\"\\\",\\\"icon\\\":\\\"/temp/iconcache/office/16x16/earth_link.png\\\",\\\"disabledIcon\\\":\\\"/temp/earth_link_disabled16x16.png\\\",\\\"isDivider\\\":false,\\\"tooltip\\\":\\\"Insert an external link into the text field.\\\",\\\"type\\\":null},{\\\"click\\\":\\\"chrome:field:insertlink\\\",\\\"header\\\":\\\"\\\",\\\"icon\\\":\\\"/temp/iconcache/office/16x16/link.png\\\",\\\"disabledIcon\\\":\\\"/temp/link_disabled16x16.png\\\",\\\"isDivider\\\":false,\\\"tooltip\\\":\\\"Insert a link into the text field.\\\",\\\"type\\\":null},{\\\"click\\\":\\\"chrome:field:removelink\\\",\\\"header\\\":\\\"\\\",\\\"icon\\\":\\\"/temp/iconcache/office/16x16/link_broken.png\\\",\\\"disabledIcon\\\":\\\"/temp/link_broken_disabled16x16.png\\\",\\\"isDivider\\\":false,\\\"tooltip\\\":\\\"Remove link.\\\",\\\"type\\\":null},{\\\"click\\\":\\\"chrome:field:insertimage\\\",\\\"header\\\":\\\"Insert image\\\",\\\"icon\\\":\\\"/temp/iconcache/office/16x16/photo_landscape.png\\\",\\\"disabledIcon\\\":\\\"/temp/photo_landscape_disabled16x16.png\\\",\\\"isDivider\\\":false,\\\"tooltip\\\":\\\"Insert an image into the text field.\\\",\\\"type\\\":null},{\\\"click\\\":\\\"chrome:common:edititem({command:\\\\\\\"webedit:open\\\\\\\"})\\\",\\\"header\\\":\\\"Edit the related item\\\",\\\"icon\\\":\\\"/temp/iconcache/office/16x16/cubes.png\\\",\\\"disabledIcon\\\":\\\"/temp/cubes_disabled16x16.png\\\",\\\"isDivider\\\":false,\\\"tooltip\\\":\\\"Edit the related item in the Content Editor.\\\",\\\"type\\\":\\\"common\\\"},{\\\"click\\\":\\\"chrome:rendering:personalize({command:\\\\\\\"webedit:personalize\\\\\\\"})\\\",\\\"header\\\":\\\"Personalize\\\",\\\"icon\\\":\\\"/temp/iconcache/office/16x16/users_family.png\\\",\\\"disabledIcon\\\":\\\"/temp/users_family_disabled16x16.png\\\",\\\"isDivider\\\":false,\\\"tooltip\\\":\\\"Create or edit personalization for this component.\\\",\\\"type\\\":\\\"sticky\\\"}],\\\"contextItemUri\\\":\\\"sitecore://master/{27F322AF-3D30-5F2F-ADA2-DAB630D35EC6}?lang=en&ver=1\\\",\\\"custom\\\":{},\\\"displayName\\\":\\\"content\\\",\\\"expandedDisplayName\\\":null}</span><span scFieldType=\\\"rich text\\\" scDefaultText=\\\"[No text in field]\\\" contenteditable=\\\"true\\\" class=\\\"scWebEditInput\\\" id=\\\"fld_27F322AF3D305F2FADA2DAB630D35EC6_A40366CE32D25868A373A20EC9C51573_en_1_ad0b2a91d2c5484facf59feb6b49dff1_328_edit\\\"><p>A child route here to illustrate the power of GraphQL queries. <a href=\\\"/graphql\\\">Back to GraphQL route</a></p>\\n</span>\"}}},{\"name\":\"code\",\"type\":\"text/sitecore\",\"contents\":\"\",\"attributes\":{\"type\":\"text/sitecore\",\"id\":\"scEnclosingTag_r_\",\"chrometype\":\"rendering\",\"kind\":\"close\",\"hintkey\":\"Content Block\",\"class\":\"scpm\"}},{\"name\":\"code\",\"type\":\"text/sitecore\",\"contents\":\"\",\"attributes\":{\"type\":\"text/sitecore\",\"id\":\"scEnclosingTag_\",\"chrometype\":\"placeholder\",\"kind\":\"close\",\"hintname\":\"Main\",\"class\":\"scpm\"}}]}}}}","{\"language\":\"en\",\"dictionary\":{\"Documentation\":\"Documentation\",\"GraphQL\":\"GraphQL\",\"Styleguide\":\"Styleguide\",\"styleguide-sample\":\"This is a dictionary entry in English as a demonstration\"},\"httpContext\":{\"request\":{\"url\":\"https://sc100xm1cm:443/?sc_itemid={cfdd7ba2-e646-5294-87fc-6fad34451a97}&sc_ee_fb=false&sc_lang=en\",\"path\":\"/\",\"querystring\":{\"sc_itemid\":\"{cfdd7ba2-e646-5294-87fc-6fad34451a97}\",\"sc_ee_fb\":\"false\",\"sc_lang\":\"en\"},\"userAgent\":\"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.45 Safari/537.36 Edg/84.0.522.20\"}}}"],"functionName":"renderView","moduleName":"server.bundle","jssEditingSecret":"mysecret"}""";

    private const string PreviewRequest =
        """{"id":"jss-sample-app","args":["/?sc_itemid=%7bcfdd7ba2-e646-5294-87fc-6fad34451a97%7d&sc_ee_fb=false&sc_lang=en&sc_mode=preview&sc_debug=0&sc_trace=0&sc_prof=0&sc_ri=0&sc_rb=0","{\"sitecore\":{\"context\":{\"pageEditing\":false,\"site\":{\"name\":\"jss-sample-app\"},\"pageState\":\"preview\",\"language\":\"en\",\"itemPath\":\"/graphql/sample-1\"},\"route\":{\"name\":\"sample-1\",\"displayName\":\"sample-1\",\"fields\":{\"pageTitle\":{\"value\":\"Sample 1 Page Title\"}},\"databaseName\":\"master\",\"deviceId\":\"fe5d7fdf-89c0-4d99-9aa3-b5fbd009c9f3\",\"itemId\":\"cfdd7ba2-e646-5294-87fc-6fad34451a97\",\"itemLanguage\":\"en\",\"itemVersion\":1,\"layoutId\":\"5179e218-3df6-5af7-8147-d2d4c05da992\",\"templateId\":\"dfe73d70-9835-584e-b0f5-28c58ab064d7\",\"templateName\":\"App Route\",\"placeholders\":{\"jss-main\":[{\"uid\":\"9157cc8c-5760-5114-9f2d-93cbe39b30dc\",\"componentName\":\"ContentBlock\",\"dataSource\":\"{27F322AF-3D30-5F2F-ADA2-DAB630D35EC6}\",\"params\":{},\"fields\":{\"heading\":{\"value\":\"GraphQL Sample 1\"},\"content\":{\"value\":\"<p>A child route here to illustrate the power of GraphQL queries. <a href=\\\"/graphql\\\">Back to GraphQL route</a></p>\\n\"}}}]}}}}","{\"language\":\"en\",\"dictionary\":{\"Documentation\":\"Documentation\",\"GraphQL\":\"GraphQL\",\"Styleguide\":\"Styleguide\",\"styleguide-sample\":\"This is a dictionary entry in English as a demonstration\"},\"httpContext\":{\"request\":{\"url\":\"https://sc100xm1cm:443/?sc_itemid={cfdd7ba2-e646-5294-87fc-6fad34451a97}&sc_ee_fb=false&sc_lang=en&sc_mode=preview&sc_debug=0&sc_trace=0&sc_prof=0&sc_ri=0&sc_rb=0\",\"path\":\"/\",\"querystring\":{\"sc_itemid\":\"{cfdd7ba2-e646-5294-87fc-6fad34451a97}\",\"sc_ee_fb\":\"false\",\"sc_lang\":\"en\",\"sc_mode\":\"preview\",\"sc_debug\":\"0\",\"sc_trace\":\"0\",\"sc_prof\":\"0\",\"sc_ri\":\"0\",\"sc_rb\":\"0\"},\"userAgent\":\"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.45 Safari/537.36 Edg/84.0.522.20\"}}}"],"functionName":"renderView","moduleName":"server.bundle","jssEditingSecret":"mysecret"}""";

    private const string JssEditingSecret = "mysecret";

    // ReSharper disable once UnusedMember.Global - Used by testing framework
    public static Action<IFixture> AutoSetup => f =>
    {
        f.Behaviors.Add(new OmitOnRecursionBehavior());
    };

    [Theory]
    [AutoNSubstituteData]
    public void Ctor_Guarded(GuardClauseAssertion guard)
    {
        guard.VerifyConstructors<ExperienceEditorMiddleware>();
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task Invoke_Guarded(ExperienceEditorMiddleware sut)
    {
        Func<Task> act =
            () => sut.Invoke(null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task Invoke_NextMiddlewareCalled([Frozen] RequestDelegate next, HttpContext httpContext, ExperienceEditorMiddleware sut)
    {
        await sut.Invoke(httpContext);

        Received.InOrder(() => next.Invoke(httpContext));
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task Invoke_WhenRequestFromIncorrectEndpoint_DoesNotProcess(IOptions<ExperienceEditorOptions> options, HttpContext httpContext, ExperienceEditorMiddleware sut)
    {
        httpContext.Request.Method = HttpMethods.Post;
        httpContext.Request.Path = new PathString("/jss-render");
        httpContext.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(EeSampleRequest));
        FeatureCollection fc = new();
        httpContext.Features.Returns(fc);

        options.Value.Endpoint = "/incorrect-endpoint";

        await sut.Invoke(httpContext);

        httpContext.GetSitecoreRenderingContext().Should().BeNull("EE Middleware not run because request has incorrect endpoint");
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task Invoke_WhenGetRequest_DoesNotProcess(HttpContext httpContext, ExperienceEditorMiddleware sut)
    {
        FeatureCollection fc = new();
        httpContext.Features.Returns(fc);

        await sut.Invoke(httpContext);

        httpContext.GetSitecoreRenderingContext().Should().BeNull("EE Middleware not run because request is a GET");
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task Invoke_WhenCorrectRequest_Processes([Frozen] IOptions<ExperienceEditorOptions> options, HttpContext httpContext, ExperienceEditorMiddleware sut)
    {
        options.Value.Endpoint = "/jss-render";
        options.Value.JssEditingSecret = JssEditingSecret;
        FeatureCollection fc = new();

        httpContext.Request.Method = HttpMethods.Post;
        httpContext.Request.Path = new PathString("/jss-render");
        httpContext.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(EeSampleRequest));
        httpContext.Response.Body = new MemoryStream();
        httpContext.Features.Returns(fc);

        await sut.Invoke(httpContext);

        httpContext.GetSitecoreRenderingContext().Should().NotBeNull();
    }

    [Theory]
    [InlineAutoNSubstituteData(EditRequest)]
    [InlineAutoNSubstituteData(PreviewRequest)]
    public async Task Invoke_SetsCorrectRequestPath(string request, ILogger<ExperienceEditorMiddleware> logger, HttpContext httpContext)
    {
        string path = null!;

        // ReSharper disable once StringLiteralTypo
        IOptions<ExperienceEditorOptions> options = new OptionsWrapper<ExperienceEditorOptions>(new ExperienceEditorOptions { JssEditingSecret = "mysecret" });
        ExperienceEditorMiddleware sut = new(
            context =>
            {
                path = context.Request.Path;
                return Task.CompletedTask;
            },
            options,
            new JsonLayoutServiceSerializer(),
            logger);

        options.Value.Endpoint = "/jss-render";
        httpContext.Request.Method = HttpMethods.Post;
        httpContext.Request.Path = new PathString("/jss-render");
        httpContext.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(request));
        httpContext.Response.Body = new MemoryStream();

        await sut.Invoke(httpContext);

        path.Should().Be("/graphql/sample-1");
    }
}