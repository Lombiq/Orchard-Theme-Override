using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard;

namespace Piedone.ThemeOverride.Services
{
    public interface IThemeOverrideService : IDependency
    {
        void SaveStyle(string css);
        string GetStyle();
        bool TryGetStylePublicUrl(out string publicUrl);
    }
}
