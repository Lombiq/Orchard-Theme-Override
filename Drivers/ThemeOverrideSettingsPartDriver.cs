using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Localization;
using Orchard.UI.Resources;
using Piedone.ThemeOverride.Models;
using Piedone.ThemeOverride.Services;

namespace Piedone.ThemeOverride.Drivers
{
    public class ThemeOverrideSettingsPartDriver : ContentPartDriver<ThemeOverrideSettingsPart>
    {
        private readonly IThemeOverrideService _themeOverrideService;

        public Localizer T { get; set; }

        public ThemeOverrideSettingsPartDriver(IThemeOverrideService themeOverrideService)
        {
            _themeOverrideService = themeOverrideService;

            T = NullLocalizer.Instance;
        }


        protected override void Importing(ThemeOverrideSettingsPart part, ImportContentContext context)
        {
            var partName = part.PartDefinition.Name;

            context.ImportAttribute(partName, "CustomStyles", value => part.CustomStyles = value);
            context.ImportAttribute(partName, "StylesheetUrisJson", value => part.StylesheetUrisJson = value);
            context.ImportAttribute(partName, "CustomHeadScript", value => part.CustomHeadScript = value);
            context.ImportAttribute(partName, "HeadScriptUrisJson", value => part.HeadScriptUrisJson = value);
            context.ImportAttribute(partName, "CustomFootScript", value => part.CustomFootScript = value);
            context.ImportAttribute(partName, "FootScriptUrisJson", value => part.FootScriptUrisJson = value);
            context.ImportAttribute(partName, "CustomPlacementContent", value => part.CustomPlacementContent = value);

            var stylesheetUris = new List<Uri>();
            if (!string.IsNullOrEmpty(part.StylesheetUrisJson) || part.StylesheetUrisJson.Equals("[]"))
            {
                foreach (var url in part.StylesheetUrisJson.Split(Environment.NewLine.ToArray(), StringSplitOptions.RemoveEmptyEntries))
                {
                    Uri stylesheetUri;
                    TryCreateUri(url, out stylesheetUri);
                    stylesheetUris.Add(stylesheetUri);
                }
            }

            var headScriptUris = new List<Uri>();
            if (!string.IsNullOrEmpty(part.HeadScriptUrisJson))
            {
                foreach (var url in part.HeadScriptUrisJson.Split(Environment.NewLine.ToArray(), StringSplitOptions.RemoveEmptyEntries))
                {
                    Uri headScriptUri;
                    TryCreateUri(url, out headScriptUri);
                    headScriptUris.Add(headScriptUri);
                }
            }

            var footScriptUris = new List<Uri>();
            if (!string.IsNullOrEmpty(part.FootScriptUrisJson))
            {
                foreach (var url in part.FootScriptUrisJson.Split(Environment.NewLine.ToArray(), StringSplitOptions.RemoveEmptyEntries))
                {
                    Uri footScriptUri;
                    TryCreateUri(url, out footScriptUri);
                    footScriptUris.Add(footScriptUri);
                }
            }

            _themeOverrideService.SaveStyles(stylesheetUris, part.CustomStyles);
            _themeOverrideService.SaveScripts(headScriptUris, part.CustomHeadScript, ResourceLocation.Head);
            _themeOverrideService.SaveScripts(footScriptUris, part.CustomFootScript, ResourceLocation.Foot);
            _themeOverrideService.SavePlacement(part.CustomPlacementContent);
        }

        private static void TryCreateUri(string url, out Uri uri)
        {
            uri = null;

            var stylesheetUriKind = Uri.IsWellFormedUriString(url, UriKind.Absolute) ? UriKind.Absolute : UriKind.Relative;
            uri = new Uri(url, stylesheetUriKind);
        }
    }
}
