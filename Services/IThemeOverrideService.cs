using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard;

namespace Piedone.ThemeOverride.Services
{
    public interface IOverrides // Get it, iStyle, hehe...
    {
        Uri StylesheetUri { get; }
        ICustomStyles CustomStyles { get; }
    }

    public interface ICustomStyles
    {
        Uri Uri { get; }
        string Content { get; }
    }


    public interface IThemeOverrideService : IDependency
    {
        void SaveStyles(Uri stylesheetUri, string customStyles);
        IOverrides GetOverrides();
    }
}
