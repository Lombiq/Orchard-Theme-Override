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
        public string StylesheetUrisJson
        {
            get { return this.Retrieve(x => x.StylesheetUrisJson); }
            set { this.Store(x => x.StylesheetUrisJson, value); }
        }

        public bool CustomStylesIsSaved
        {
            get { return this.Retrieve(x => x.CustomStylesIsSaved); }
            set { this.Store(x => x.CustomStylesIsSaved, value); }
        }

        public string HeadScriptUrisJson
        {
            get { return this.Retrieve(x => x.HeadScriptUrisJson); }
            set { this.Store(x => x.HeadScriptUrisJson, value); }
        }

        public bool CustomHeadScriptIsSaved
        {
            get { return this.Retrieve(x => x.CustomHeadScriptIsSaved); }
            set { this.Store(x => x.CustomHeadScriptIsSaved, value); }
        }

        public string FootScriptUrisJson
        {
            get { return this.Retrieve(x => x.FootScriptUrisJson); }
            set { this.Store(x => x.FootScriptUrisJson, value); }
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
