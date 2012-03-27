using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Piedone.ThemeOverride.Models;

namespace Piedone.ThemeOverride.Handlers
{
    public class ThemeOverrideSettingsPartHandler : ContentHandler
    {
        public ThemeOverrideSettingsPartHandler(IRepository<ThemeOverrideSettingsPartRecord> repository)
        {
            Filters.Add(new ActivatingFilter<ThemeOverrideSettingsPart>("Site"));

            Filters.Add(StorageFilter.For(repository));
        }
    }
}
