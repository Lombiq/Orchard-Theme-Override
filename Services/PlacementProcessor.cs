using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using Orchard.DisplayManagement.Descriptors;
using Orchard.DisplayManagement.Descriptors.ShapePlacementStrategy;

namespace Piedone.ThemeOverride.Services
{
    // Mainly copy of ShapePlacementParsingStrategy, see: https://github.com/OrchardCMS/Orchard/issues/4138
    public class PlacementProcessor : IPlacementProcessor
    {
        public IDictionary<string, IEnumerable<IPlacementDeclaration>> Process(string placementDeclaration)
        {
            var placements = new Dictionary<string, IEnumerable<IPlacementDeclaration>>();

            if (string.IsNullOrEmpty(placementDeclaration)) return placements;

            placementDeclaration = placementDeclaration.Trim();

            if (!placementDeclaration.StartsWith("<Placement>"))
            {
                placementDeclaration = "<Placement>" + placementDeclaration;
            }
            if (!placementDeclaration.EndsWith("</Placement>"))
            {
                placementDeclaration += "</Placement>";
            }

            var placementFile = new PlacementParser().Parse(placementDeclaration);
            if (placementFile != null)
            {
                // Invert the tree into a list of leaves and the stack
                var entries = DrillDownShapeLocations(placementFile.Nodes, Enumerable.Empty<PlacementMatch>());
                foreach (var entry in entries)
                {
                    var shapeLocation = entry.Item1;
                    var matches = entry.Item2;

                    string shapeType;
                    string differentiator;
                    GetShapeType(shapeLocation, out shapeType, out differentiator);

                    Func<ShapePlacementContext, bool> predicate = ctx => true;
                    if (differentiator != "")
                    {
                        predicate = ctx => (ctx.Differentiator ?? "") == differentiator;
                    }

                    if (matches.Any())
                    {
                        predicate = matches.SelectMany(match => match.Terms).Aggregate(predicate, BuildPredicate);
                    }

                    var placement = new PlacementInfo();

                    var segments = shapeLocation.Location.Split(';').Select(s => s.Trim());
                    foreach (var segment in segments)
                    {
                        if (!segment.Contains('='))
                        {
                            placement.Location = segment;
                        }
                        else
                        {
                            var index = segment.IndexOf('=');
                            var property = segment.Substring(0, index).ToLower();
                            var value = segment.Substring(index + 1);
                            switch (property)
                            {
                                case "shape":
                                    placement.ShapeType = value;
                                    break;
                                case "alternate":
                                    placement.Alternates = new[] { value };
                                    break;
                                case "wrapper":
                                    placement.Wrappers = new[] { value };
                                    break;
                            }
                        }
                    }

                    if (!placements.ContainsKey(shapeType))
                    {
                        placements[shapeType] = Enumerable.Empty<IPlacementDeclaration>();
                    }

                    placements[shapeType] = placements[shapeType].Concat(new[]
                    {
                        new PlacementDeclaration
                        {
                            Predicate = predicate,
                            Placement = placement
                        }
                    });
                }
            }

            return placements;
        }


        private void GetShapeType(PlacementShapeLocation shapeLocation, out string shapeType, out string differentiator)
        {
            differentiator = "";
            shapeType = shapeLocation.ShapeType;
            var dashIndex = shapeType.LastIndexOf('-');
            if (dashIndex > 0 && dashIndex < shapeType.Length - 1)
            {
                differentiator = shapeType.Substring(dashIndex + 1);
                shapeType = shapeType.Substring(0, dashIndex);
            }
        }

        private static Func<ShapePlacementContext, bool> BuildPredicate(Func<ShapePlacementContext, bool> predicate, KeyValuePair<string, string> term)
        {
            var expression = term.Value;
            switch (term.Key)
            {
                case "ContentType":
                    if (expression.EndsWith("*"))
                    {
                        var prefix = expression.Substring(0, expression.Length - 1);
                        return ctx => ((ctx.ContentType ?? "").StartsWith(prefix) || (ctx.Stereotype ?? "").StartsWith(prefix)) && predicate(ctx);
                    }
                    return ctx => ((ctx.ContentType == expression) || (ctx.Stereotype == expression)) && predicate(ctx);
                case "DisplayType":
                    if (expression.EndsWith("*"))
                    {
                        var prefix = expression.Substring(0, expression.Length - 1);
                        return ctx => (ctx.DisplayType ?? "").StartsWith(prefix) && predicate(ctx);
                    }
                    return ctx => (ctx.DisplayType == expression) && predicate(ctx);
                case "Path":
                    var normalizedPath = VirtualPathUtility.IsAbsolute(expression)
                                             ? VirtualPathUtility.ToAppRelative(expression)
                                             : VirtualPathUtility.Combine("~/", expression);

                    if (normalizedPath.EndsWith("*"))
                    {
                        var prefix = normalizedPath.Substring(0, normalizedPath.Length - 1);
                        return ctx => VirtualPathUtility.ToAppRelative(String.IsNullOrEmpty(ctx.Path) ? "/" : ctx.Path).StartsWith(prefix, StringComparison.OrdinalIgnoreCase) && predicate(ctx);
                    }

                    normalizedPath = VirtualPathUtility.AppendTrailingSlash(normalizedPath);
                    return ctx => (ctx.Path.Equals(normalizedPath, StringComparison.OrdinalIgnoreCase)) && predicate(ctx);
            }
            return predicate;
        }

        private static IEnumerable<Tuple<PlacementShapeLocation, IEnumerable<PlacementMatch>>> DrillDownShapeLocations(
            IEnumerable<PlacementNode> nodes,
            IEnumerable<PlacementMatch> path)
        {

            // return shape locations nodes in this place
            foreach (var placementShapeLocation in nodes.OfType<PlacementShapeLocation>())
            {
                yield return new Tuple<PlacementShapeLocation, IEnumerable<PlacementMatch>>(placementShapeLocation, path);
            }
            // recurse down into match nodes
            foreach (var placementMatch in nodes.OfType<PlacementMatch>())
            {
                foreach (var findShapeLocation in DrillDownShapeLocations(placementMatch.Nodes, path.Concat(new[] { placementMatch })))
                {
                    yield return findShapeLocation;
                }
            }
        }


        public class PlacementDeclaration : Piedone.ThemeOverride.Services.IPlacementDeclaration
        {
            public Func<ShapePlacementContext, bool> Predicate { get; set; }
            public PlacementInfo Placement { get; set; }
        }
    }


    // Copied from PlacementFileParser, see: https://github.com/OrchardCMS/Orchard/issues/4138
    public class PlacementParser
    {
        public PlacementFile Parse(string placementText)
        {
            if (placementText == null)
                return null;


            var element = XElement.Parse(placementText);
            return new PlacementFile
            {
                Nodes = Accept(element).ToList()
            };
        }


        private IEnumerable<PlacementNode> Accept(XElement element)
        {
            switch (element.Name.LocalName)
            {
                case "Placement":
                    return AcceptMatch(element);
                case "Match":
                    return AcceptMatch(element);
                case "Place":
                    return AcceptPlace(element);
            }
            return Enumerable.Empty<PlacementNode>();
        }

        private IEnumerable<PlacementNode> AcceptMatch(XElement element)
        {
            if (element.HasAttributes == false)
            {
                // Match with no attributes will collapse child results upward
                // rather than return an unconditional node
                return element.Elements().SelectMany(Accept);
            }

            // return match node that carries back key/value dictionary of condition,
            // and has child rules nested as Nodes
            return new[]{new PlacementMatch{
                Terms = element.Attributes().ToDictionary(attr=>attr.Name.LocalName, attr=>attr.Value),
                Nodes=element.Elements().SelectMany(Accept).ToArray(),
            }};
        }

        private IEnumerable<PlacementShapeLocation> AcceptPlace(XElement element)
        {
            // return attributes as part locations
            return element.Attributes().Select(attr => new PlacementShapeLocation
            {
                ShapeType = attr.Name.LocalName,
                Location = attr.Value
            });
        }
    }
}
