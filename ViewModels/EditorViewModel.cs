using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Piedone.ThemeOverride.ViewModels
{
    public class EditorViewModel
    {
        public string StylesheetUrl { get; set; }
        public string CustomStylesContent { get; set; }
        public string HeadScriptUrl { get; set; }
        public string CustomHeadScriptContent { get; set; }
        public string FootScriptUrl { get; set; }
        public string CustomFootScriptContent { get; set; }
        public string CustomPlacementContent { get; set; }
    }
}
