﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.ContentManagement.Handlers;
using Piedone.ThemeOverride.Models;

namespace Piedone.ThemeOverride.Handlers
{
    public class ThemeOverrideSettingsPartHandler : ContentHandler
    {
        public ThemeOverrideSettingsPartHandler()
        {
            Filters.Add(new ActivatingFilter<ThemeOverrideSettingsPart>("Site"));
        }
    }
}
