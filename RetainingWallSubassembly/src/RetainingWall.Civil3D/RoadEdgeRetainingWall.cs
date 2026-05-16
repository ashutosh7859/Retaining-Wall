using System;
using System.Collections.Generic;
using Autodesk.Civil.Runtime;
using RetainingWall.Core;

namespace RetainingWall.Civil3D
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles")]
    public class RoadEdgeRetainingWall : SATemplate
    {
        protected override void GetInputParametersImplement(CorridorState corridorState)
        {
            var paramsLong = corridorState.ParamsLong;
            var paramsDouble = corridorState.ParamsDouble;
            var paramsBool = corridorState.ParamsBool;
            var paramsString = corridorState.ParamsString;

            paramsString.Add("Side", "Left");
            paramsLong.Add("WallCase", 2);
            paramsBool.Add("UseDimensionOverrides", false);
            paramsDouble.Add("OverrideA", 0.0);
            paramsDouble.Add("OverrideB", 0.0);
            paramsDouble.Add("OverrideC", 0.0);
            paramsDouble.Add("OverrideD", 0.0);
            paramsDouble.Add("OverrideE", 0.0);
            paramsDouble.Add("OverrideF", 0.0);
            paramsDouble.Add("OverrideG", 0.0);
            paramsDouble.Add("OverrideH1", 0.0);
            paramsDouble.Add("OverrideH2", 0.0);
            paramsDouble.Add("PccThickness", 0.150);
            paramsDouble.Add("PccProjection", 0.150);
            paramsDouble.Add("FilterThickness", 0.600);
            paramsDouble.Add("WallTopOffset", 0.450);
        }

        protected override void DrawImplement(CorridorState corridorState)
        {
            if (corridorState == null) return;

            var sideStr = GetStringParam(corridorState, "Side", "Left");
            var side = string.Equals(sideStr, "Right", StringComparison.OrdinalIgnoreCase) ? RetainingWall.Core.Side.Right : RetainingWall.Core.Side.Left;
            
            var wallCase = GetIntParam(corridorState, "WallCase", 2);
            var dims = WallCaseTable.GetDimensions(wallCase);
            
            var overrides = new WallDimensionOverrides
            {
                UseDimensionOverrides = GetBoolParam(corridorState, "UseDimensionOverrides", false),
                OverrideA = GetDoubleParam(corridorState, "OverrideA", 0.0),
                OverrideB = GetDoubleParam(corridorState, "OverrideB", 0.0),
                OverrideC = GetDoubleParam(corridorState, "OverrideC", 0.0),
                OverrideD = GetDoubleParam(corridorState, "OverrideD", 0.0),
                OverrideE = GetDoubleParam(corridorState, "OverrideE", 0.0),
                OverrideF = GetDoubleParam(corridorState, "OverrideF", 0.0),
                OverrideG = GetDoubleParam(corridorState, "OverrideG", 0.0),
                OverrideH1 = GetDoubleParam(corridorState, "OverrideH1", 0.0),
                OverrideH2 = GetDoubleParam(corridorState, "OverrideH2", 0.0)
            };
            var finalDims = overrides.Apply(dims);

            var generator = new WallGeometryGenerator
            {
                PccThickness = GetDoubleParam(corridorState, "PccThickness", 0.150),
                PccProjection = GetDoubleParam(corridorState, "PccProjection", 0.150),
                FilterThickness = GetDoubleParam(corridorState, "FilterThickness", 0.600),
                WallTopOffset = GetDoubleParam(corridorState, "WallTopOffset", 0.450)
            };

            var result = generator.Generate(finalDims, side);

            var corePointsToC3DPoints = new Dictionary<GeometryPoint, Autodesk.Civil.DatabaseServices.IPoint>();
            foreach (var cp in result.Points)
            {
                var pt = corridorState.Points.Add(cp.X, cp.Y, "");
                foreach (var code in cp.Codes)
                {
                    try { pt.Codes.Add(code); } catch { }
                }
                corePointsToC3DPoints[cp] = pt;
            }

            var coreLinksToC3DLinks = new Dictionary<GeometryLink, Autodesk.Civil.DatabaseServices.Link>();
            foreach (var cl in result.Links)
            {
                var pts = new Autodesk.Civil.DatabaseServices.IPoint[2];
                pts[0] = corePointsToC3DPoints[cl.StartPoint];
                pts[1] = corePointsToC3DPoints[cl.EndPoint];
                var l = corridorState.Links.Add(pts, new string[0]);
                foreach (var code in cl.Codes)
                {
                    try { l.Codes.Add(code); } catch { }
                }
                coreLinksToC3DLinks[cl] = l;
            }

            foreach (var cs in result.Shapes)
            {
                var links = new Autodesk.Civil.DatabaseServices.Link[cs.Links.Count];
                for (int i = 0; i < cs.Links.Count; i++)
                {
                    links[i] = coreLinksToC3DLinks[cs.Links[i]];
                }
                var s = corridorState.Shapes.Add(links, new string[0]);
                foreach (var code in cs.Codes)
                {
                    try { s.Codes.Add(code); } catch { }
                }
            }
        }

        private string GetStringParam(CorridorState corridorState, string name, string defaultValue)
        {
            try { return corridorState.ParamsString[name].Value; }
            catch { return defaultValue; }
        }

        private int GetIntParam(CorridorState corridorState, string name, int defaultValue)
        {
            try { return (int)corridorState.ParamsLong[name].Value; }
            catch { return defaultValue; }
        }

        private double GetDoubleParam(CorridorState corridorState, string name, double defaultValue)
        {
            try { return corridorState.ParamsDouble[name].Value; }
            catch { return defaultValue; }
        }

        private bool GetBoolParam(CorridorState corridorState, string name, bool defaultValue)
        {
            try { return corridorState.ParamsBool[name].Value; }
            catch { return defaultValue; }
        }
    }
}
