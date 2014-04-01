using Orchard.ContentManagement.Handlers;
using Piedone.ThemeOverride.Models;

namespace Piedone.ThemeOverride.Handlers
{
    public class ThemeOverrideSettingsPartHandler : ContentHandler
    {
        public ThemeOverrideSettingsPartHandler()
        {
            Filters.Add(new ActivatingFilter<ThemeOverrideSettingsPart>("Site"));

            OnInitializing<ThemeOverrideSettingsPart>((ctx, part) => part.CustomPlacementContent = "<Placement></Placement>");
        }
    }
}
