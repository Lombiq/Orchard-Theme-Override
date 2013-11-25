using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard;
using Orchard.UI.Resources;

namespace Piedone.ThemeOverride.Services
{
    public interface IOverrides // Get it, iStyle, hehe...
    {
        Uri StylesheetUri { get; }
        ICustomResource CustomStyles { get; }
        Uri HeadScriptUri { get; }
        ICustomResource CustomHeadScript { get; }
        Uri FootScriptUri { get; }
        ICustomResource CustomFootScript { get; }
    }

    public interface ICustomResource
    {
        Uri Uri { get; }
        string Content { get; }
    }


    public interface IThemeOverrideService : IDependency
    {
        void SaveStyles(Uri stylesheetUri, string customStyles);
        void SaveScripts(Uri scriptUri, string customScript, ResourceLocation location);
        IOverrides GetOverrides();
    }
}
