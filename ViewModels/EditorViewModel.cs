using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Piedone.ThemeOverride.ViewModels
{
    public class EditorViewModel
    {
        public string StylesheetUrls { get; set; }
        public string CustomStylesContent { get; set; }
        public string HeadScriptUrls { get; set; }
        public string CustomHeadScriptContent { get; set; }
        public string FootScriptUrls { get; set; }
        public string CustomFootScriptContent { get; set; }
        public string CustomPlacementContent { get; set; }
    }
}
