using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.ContentManagement.Records;

namespace Piedone.ThemeOverride.Models
{
    public class ThemeOverrideSettingsPartRecord : ContentPartRecord
    {
        public virtual string Style { get; set; }
    }
}
