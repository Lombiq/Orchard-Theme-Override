using System;
using Orchard.ContentManagement;

namespace Piedone.ThemeOverride.Models
{
    public class ThemeOverrideSettingsPart : ContentPart
    {
        public string FaviconUrl
        {
            get { return this.Retrieve(x => x.FaviconUrl); }
            set { this.Store(x => x.FaviconUrl, value); }
        }

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

        public DateTime CustomStylesModifieddUtc
        {
            get { return this.Retrieve(x => x.CustomStylesModifieddUtc); }
            set { this.Store(x => x.CustomStylesModifieddUtc, value); }
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

        public DateTime CustomHeadScriptModifiedUtc
        {
            get { return this.Retrieve(x => x.CustomHeadScriptModifiedUtc); }
            set { this.Store(x => x.CustomHeadScriptModifiedUtc, value); }
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

        public DateTime CustomFootScriptModifiedUtc
        {
            get { return this.Retrieve(x => x.CustomFootScriptModifiedUtc); }
            set { this.Store(x => x.CustomFootScriptModifiedUtc, value); }
        }

        public string CustomPlacementContent
        {
            get { return this.Retrieve(x => x.CustomPlacementContent); }
            set { this.Store(x => x.CustomPlacementContent, value); }
        }
    }
}
