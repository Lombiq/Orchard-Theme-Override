using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.UI.Resources;
using Piedone.ThemeOverride.Models;
using Piedone.ThemeOverride.Services;

namespace Piedone.ThemeOverride.Drivers
{
    public class ThemeOverrideSettingsPartDriver : ContentPartDriver<ThemeOverrideSettingsPart>
    {
        private readonly IThemeOverrideService _themeOverrideService;


        public ThemeOverrideSettingsPartDriver(IThemeOverrideService themeOverrideService)
        {
            _themeOverrideService = themeOverrideService;
        }


        protected override void Importing(ThemeOverrideSettingsPart part, ImportContentContext context)
        {
            var partName = part.PartDefinition.Name;

            context.ImportAttribute(partName, "CustomStyles", value => part.CustomStyles = value);
            context.ImportAttribute(partName, "HeadScriptUrisJson", value => part.HeadScriptUrisJson = value);
            context.ImportAttribute(partName, "FootScriptUrisJson", value => part.FootScriptUrisJson = value);
            context.ImportAttribute(partName, "CustomPlacementContent", value => part.CustomPlacementContent = value);

            _themeOverrideService.SaveStyles(null, part.CustomStyles);
            _themeOverrideService.SaveScripts(null, part.HeadScriptUrisJson, ResourceLocation.Head);
            _themeOverrideService.SaveScripts(null, part.FootScriptUrisJson, ResourceLocation.Foot);
            _themeOverrideService.SavePlacement(part.CustomPlacementContent);
        }
    }
}
