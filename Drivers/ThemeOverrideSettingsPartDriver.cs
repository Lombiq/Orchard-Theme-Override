using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Localization;
using Orchard.UI.Resources;
using Piedone.ThemeOverride.Models;
using Piedone.ThemeOverride.Services;
using System;
using System.Collections.Generic;
using System.Linq;

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

        protected override void Exporting(ThemeOverrideSettingsPart part, ExportContentContext context)
        {
            var element = context.Element(part.PartDefinition.Name);

            element.SetAttributeValue("CustomStyles", _themeOverrideService.GetOverrides().CustomStyles.Content);
            element.SetAttributeValue("CustomHeadScript", _themeOverrideService.GetOverrides().CustomHeadScript.Content);
            element.SetAttributeValue("CustomFootScript", _themeOverrideService.GetOverrides().CustomFootScript.Content);
        }


        protected override void Importing(ThemeOverrideSettingsPart part, ImportContentContext context)
        {
            if (!string.IsNullOrEmpty(part.FaviconUrl))
            {
                if (TryCreateUri(part.FaviconUrl, out Uri faviconUri))
                {
                    _themeOverrideService.SaveFaviconUri(faviconUri);
                }
            }

            var stylesheetUris = new List<Uri>();
            if (!string.IsNullOrEmpty(part.StylesheetUrisJson))
            {
                foreach (var url in part.StylesheetUrisJson.Split(Environment.NewLine.ToArray(), StringSplitOptions.RemoveEmptyEntries))
                {
                    if (TryCreateUri(url, out Uri stylesheetUri))
                    {
                        stylesheetUris.Add(stylesheetUri);
                    }
                }
            }

            var headScriptUris = new List<Uri>();
            if (!string.IsNullOrEmpty(part.HeadScriptUrisJson))
            {
                foreach (var url in part.HeadScriptUrisJson.Split(Environment.NewLine.ToArray(), StringSplitOptions.RemoveEmptyEntries))
                {
                    if (TryCreateUri(url, out Uri headScriptUri))
                    {
                        headScriptUris.Add(headScriptUri);
                    }
                }
            }

            var footScriptUris = new List<Uri>();
            if (!string.IsNullOrEmpty(part.FootScriptUrisJson))
            {
                foreach (var url in part.FootScriptUrisJson.Split(Environment.NewLine.ToArray(), StringSplitOptions.RemoveEmptyEntries))
                {
                    if (TryCreateUri(url, out Uri footScriptUri))
                    {
                        footScriptUris.Add(footScriptUri);
                    }
                }
            }

            var partName = part.PartDefinition.Name;
            string customStyles = "";
            string customHeadScript = "";
            string customFootScript = "";

            context.ImportAttribute(partName, "CustomStyles", value => customStyles = value);
            context.ImportAttribute(partName, "CustomHeadScript", value => customHeadScript = value);
            context.ImportAttribute(partName, "CustomFootScript", value => customFootScript = value);

            _themeOverrideService.SaveStyles(stylesheetUris, customStyles);
            _themeOverrideService.SaveScripts(headScriptUris, customHeadScript, ResourceLocation.Head);
            _themeOverrideService.SaveScripts(footScriptUris, customFootScript, ResourceLocation.Foot);
            _themeOverrideService.SavePlacement(part.CustomPlacementContent);
        }


        private bool TryCreateUri(string url, out Uri uri)
        {
            uri = null;

            if (!string.IsNullOrEmpty(url))
            {
                if (!Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute)) return false;

                var stylesheetUriKind = Uri.IsWellFormedUriString(url, UriKind.Absolute) ? UriKind.Absolute : UriKind.Relative;
                uri = new Uri(url, stylesheetUriKind);

                return true;
            }

            return false;
        }
    }
}
