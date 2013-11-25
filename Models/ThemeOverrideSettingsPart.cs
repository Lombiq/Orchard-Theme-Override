using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.ContentManagement;

namespace Piedone.ThemeOverride.Models
{
    public class ThemeOverrideSettingsPart : ContentPart
    {
        public string StylesheetUrl
        {
            get { return this.Retrieve(x => x.StylesheetUrl); }
            set { this.Store(x => x.StylesheetUrl, value); }
        }

        public bool CustomStylesSaved
        {
            get { return this.Retrieve(x => x.CustomStylesSaved); }
            set { this.Store(x => x.CustomStylesSaved, value); }
        }
    }
}
