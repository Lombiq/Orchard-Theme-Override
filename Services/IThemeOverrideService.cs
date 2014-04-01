using System;
using System.Collections.Generic;
using Orchard;
using Orchard.UI.Resources;

namespace Piedone.ThemeOverride.Services
{
    public interface IOverrides
    {
        Uri FaviconUri { get; set; }
        IEnumerable<Uri> StylesheetUris { get; }
        ICustomResource CustomStyles { get; }
        IEnumerable<Uri> HeadScriptUris { get; }
        ICustomResource CustomHeadScript { get; }
        IEnumerable<Uri> FootScriptUris { get; }
        ICustomResource CustomFootScript { get; }
        string CustomPlacementContent { get; }
    }

    public interface ICustomResource
    {
        Uri Uri { get; }
        string Content { get; }
    }


    public interface IThemeOverrideService : IDependency
    {
        void SaveFaviconUri(Uri uri);
        void SaveStyles(IEnumerable<Uri> stylesheetUris, string customStyles);
        void SaveScripts(IEnumerable<Uri> scriptUris, string customScript, ResourceLocation location);
        void SavePlacement(string customPlacement);
        IOverrides GetOverrides();
    }
}
