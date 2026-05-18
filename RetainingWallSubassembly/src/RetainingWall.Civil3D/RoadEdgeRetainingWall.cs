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
            
            // Mode Selectors
            paramsBool.Add("UseTable", true);
            paramsBool.Add("SelectByHeight", false);
            
            // Table selection parameter
            paramsLong.Add("WallCase", 2);

            // Dimensions / Overrides
            // When UseTable = True, these act as overrides (if > 0).
            // When UseTable = False, these act as the direct manual dimensions.
            paramsDouble.Add("A", 0.0);
            paramsDouble.Add("B", 0.0);
            paramsDouble.Add("C", 0.0);
            paramsDouble.Add("D", 0.0);
            paramsDouble.Add("E", 0.0);
            paramsDouble.Add("F", 0.0);
            paramsDouble.Add("G", 0.0);
            paramsDouble.Add("H1", 0.0);
            paramsDouble.Add("H2", 0.0);

            // Construction parameters
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
            
            var useTable = GetBoolParam(corridorState, "UseTable", true);
            var selectByHeight = GetBoolParam(corridorState, "SelectByHeight", false);
            
            // Read direct dimension/override inputs
            double inputA = GetDoubleParam(corridorState, "A", 0.0);
            double inputB = GetDoubleParam(corridorState, "B", 0.0);
            double inputC = GetDoubleParam(corridorState, "C", 0.0);
            double inputD = GetDoubleParam(corridorState, "D", 0.0);
            double inputE = GetDoubleParam(corridorState, "E", 0.0);
            double inputF = GetDoubleParam(corridorState, "F", 0.0);
            double inputG = GetDoubleParam(corridorState, "G", 0.0);
            double inputH1 = GetDoubleParam(corridorState, "H1", 0.0);
            double inputH2 = GetDoubleParam(corridorState, "H2", 0.0);

            WallDimensions dims;

            if (useTable)
            {
                WallDimensions baseDims;
                if (selectByHeight)
                {
                    // If selecting by height, we use inputH1 if specified (>0), else default to Case 2 height
                    double lookupHeight = inputH1 > 0 ? inputH1 : 2.000;
                    baseDims = WallCaseTable.GetDimensionsByHeight(lookupHeight);
                }
                else
                {
                    var wallCase = GetIntParam(corridorState, "WallCase", 2);
                    baseDims = WallCaseTable.GetDimensions(wallCase);
                }

                // Apply overrides (any value > 0 overrides the table value)
                dims = new WallDimensions(
                    inputA > 0 ? inputA : baseDims.A,
                    inputB > 0 ? inputB : baseDims.B,
                    inputC > 0 ? inputC : baseDims.C,
                    inputD > 0 ? inputD : baseDims.D,
                    inputE > 0 ? inputE : baseDims.E,
                    inputF > 0 ? inputF : baseDims.F,
                    inputG > 0 ? inputG : baseDims.G,
                    inputH1 > 0 ? inputH1 : baseDims.H1,
                    inputH2 > 0 ? inputH2 : baseDims.H2
                );
            }
            else
            {
                // Manual mode: use inputs directly. Fallback to Case 2 defaults if any inputs are 0.
                var defaultCase2 = WallCaseTable.GetDimensions(2);
                dims = new WallDimensions(
                    inputA > 0 ? inputA : defaultCase2.A,
                    inputB > 0 ? inputB : defaultCase2.B,
                    inputC > 0 ? inputC : defaultCase2.C,
                    inputD > 0 ? inputD : defaultCase2.D,
                    inputE > 0 ? inputE : defaultCase2.E,
                    inputF > 0 ? inputF : defaultCase2.F,
                    inputG > 0 ? inputG : defaultCase2.G,
                    inputH1 > 0 ? inputH1 : defaultCase2.H1,
                    inputH2 >= 0 ? inputH2 : defaultCase2.H2 // Allow 0 for H2 (no shear key)
                );
            }

            var generator = new WallGeometryGenerator
            {
                PccThickness = GetDoubleParam(corridorState, "PccThickness", 0.150),
                PccProjection = GetDoubleParam(corridorState, "PccProjection", 0.150),
                FilterThickness = GetDoubleParam(corridorState, "FilterThickness", 0.600),
                WallTopOffset = GetDoubleParam(corridorState, "WallTopOffset", 0.450)
            };

            var result = generator.Generate(dims, side);

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
