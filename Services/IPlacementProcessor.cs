using System;
using System.Collections.Generic;
using Orchard;
using Orchard.DisplayManagement.Descriptors;

namespace Piedone.ThemeOverride.Services
{
    public interface IPlacementDeclaration
    {
        PlacementInfo Placement { get; }
        Func<ShapePlacementContext, bool> Predicate { get; }
    }


    public interface IPlacementProcessor : IDependency
    {
        IDictionary<string, IEnumerable<IPlacementDeclaration>> Process(string placementDeclaration);
    }
}
