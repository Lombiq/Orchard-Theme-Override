using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.ContentManagement;

namespace Piedone.ThemeOverride.Models
{
    public class ThemeOverrideSettingsPart : ContentPart<ThemeOverrideSettingsPartRecord>
    {
        public string Style
        {
            get { return Record.Style; }
            set { Record.Style = value; }
        }
    }
}
