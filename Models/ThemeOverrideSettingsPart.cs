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

        public bool CustomStylesIsSaved
        {
            get { return this.Retrieve(x => x.CustomStylesIsSaved); }
            set { this.Store(x => x.CustomStylesIsSaved, value); }
        }

        public string HeadScriptUrl
        {
            get { return this.Retrieve(x => x.HeadScriptUrl); }
            set { this.Store(x => x.HeadScriptUrl, value); }
        }

        public bool CustomHeadScriptIsSaved
        {
            get { return this.Retrieve(x => x.CustomHeadScriptIsSaved); }
            set { this.Store(x => x.CustomHeadScriptIsSaved, value); }
        }

        public string FootScriptUrl
        {
            get { return this.Retrieve(x => x.FootScriptUrl); }
            set { this.Store(x => x.FootScriptUrl, value); }
        }

        public bool CustomFootScriptIsSaved
        {
            get { return this.Retrieve(x => x.CustomFootScriptIsSaved); }
            set { this.Store(x => x.CustomFootScriptIsSaved, value); }
        }

        public string CustomPlacementContent
        {
            get { return this.Retrieve(x => x.CustomPlacementContent); }
            set { this.Store(x => x.CustomPlacementContent, value); }
        }
    }
}
