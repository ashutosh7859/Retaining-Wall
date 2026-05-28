using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.Runtime;
using RetainingWall.Core;

namespace RetainingWall.Civil3D
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles")]
    public class RoadEdgeRetainingWall : SATemplate
    {
        protected override void GetLogicalNamesImplement(CorridorState corridorState)
        {
            if (corridorState == null) return;
            try
            {
                var param = corridorState.ParamsLong.Add("GroundSurface", (int)ParamLogicalNameType.Surface);
                param.DisplayName = "Ground Surface Target";
            }
            catch { }
        }

        protected override void GetInputParametersImplement(CorridorState corridorState)
        {
            var paramsLong = corridorState.ParamsLong;
            var paramsDouble = corridorState.ParamsDouble;

            var sideParam = paramsLong.Add("Side", (int)WallConfig.DefaultSide);
            if (sideParam != null) { sideParam.DisplayName = "Side"; sideParam.Description = "Wall side: Left or Right"; }

            var modeParam = paramsLong.Add("OperatingMode", (int)WallConfig.DefaultOperatingMode);
            if (modeParam != null) { modeParam.DisplayName = "Operating Mode"; modeParam.Description = "Operating configuration mode (Dynamic Table, Preset Dropdown, or Full Custom)"; }

            var presetParam = paramsLong.Add("WallPreset", (int)WallConfig.DefaultWallPreset);
            if (presetParam != null) { presetParam.DisplayName = "Wall Preset"; presetParam.Description = "Standard design case preset (Type 2 to 12)"; }

            // Dimensions - Stem Notch & Batter
            var wParam = paramsDouble.Add("w", WallConfig.DefaultW);
            if (wParam != null) { wParam.DisplayName = "Stem Top Width (w)"; wParam.Description = "Top stem width P1 to P2 (meters)"; }

            var x1Param = paramsDouble.Add("x1", WallConfig.DefaultX1);
            if (x1Param != null) { x1Param.DisplayName = "Notch Drop (x1)"; x1Param.Description = "Vertical drop from P1 to P11 (meters)"; }

            var x2Param = paramsDouble.Add("x2", WallConfig.DefaultX2);
            if (x2Param != null) { x2Param.DisplayName = "Notch Slope Drop (x2)"; x2Param.Description = "Vertical drop of notch slope P11 to P10 (meters)"; }

            var aParam = paramsDouble.Add("a", WallConfig.DefaultA);
            if (aParam != null) { aParam.DisplayName = "Notch Recess (a)"; aParam.Description = "Recess thickness at P10 (meters)"; }

            // Dimensions - Custom Footing & Base (Full Custom only)
            var h1Param = paramsDouble.Add("Custom_H1", WallConfig.DefaultCustomH1);
            if (h1Param != null) { h1Param.DisplayName = "Stem Height (H1)"; h1Param.Description = "Manual stem height H1 in Custom mode (meters)"; }

            var bParam = paramsDouble.Add("Custom_b", WallConfig.DefaultCustomB);
            if (bParam != null) { bParam.DisplayName = "Stem Base Width (b)"; bParam.Description = "Stem thickness at footing plane in Custom mode (meters)"; }

            var cParam = paramsDouble.Add("Custom_c", WallConfig.DefaultCustomC);
            if (cParam != null) { cParam.DisplayName = "Heel Projection (c)"; cParam.Description = "Heel projection length in Custom mode (meters)"; }

            var dParam = paramsDouble.Add("Custom_d", WallConfig.DefaultCustomD);
            if (dParam != null) { dParam.DisplayName = "Toe Projection (d)"; dParam.Description = "Toe projection length in Custom mode (meters)"; }

            var eParam = paramsDouble.Add("Custom_e", WallConfig.DefaultCustomE);
            if (eParam != null) { eParam.DisplayName = "Heel Edge Thickness (e)"; eParam.Description = "Vertical thickness of outer heel face in Custom mode (meters)"; }

            var fParam = paramsDouble.Add("Custom_f", WallConfig.DefaultCustomF);
            if (fParam != null) { fParam.DisplayName = "Max Footing Thickness (f)"; fParam.Description = "Maximum footing depth at stem junction in Custom mode (meters)"; }

            var gParam = paramsDouble.Add("Custom_g", WallConfig.DefaultCustomG);
            if (gParam != null) { gParam.DisplayName = "Toe Edge Thickness (g)"; gParam.Description = "Vertical thickness of outer toe face in Custom mode (meters)"; }

            var h2Param = paramsDouble.Add("Custom_H2", WallConfig.DefaultCustomH2);
            if (h2Param != null) { h2Param.DisplayName = "Base Depth (H2)"; h2Param.Description = "Foundation depth from GL to footing bottom in Custom mode (meters)"; }

            // Foundations - PCC Mud Mat
            var pccTParam = paramsDouble.Add("PccThickness", WallConfig.DefaultPccThickness);
            if (pccTParam != null) { pccTParam.DisplayName = "Mud Mat Thickness (PCC)"; pccTParam.Description = "Portland cement concrete thickness (meters)"; }

            var pccOParam = paramsDouble.Add("PccOffset", WallConfig.DefaultPccOffset);
            if (pccOParam != null) { pccOParam.DisplayName = "Mud Mat Offset (PCC)"; pccOParam.Description = "PCC offset distance (meters)"; }

            // Backfill - Filter Media
            var fmParam = paramsDouble.Add("FmThickness", WallConfig.DefaultFmThickness);
            if (fmParam != null) { fmParam.DisplayName = "Filter Media Thickness"; fmParam.Description = "Filter media thickness (meters)"; }
        }

        protected override void GetOutputParametersImplement(CorridorState corridorState)
        {
            if (corridorState == null) return;
            var paramsLong = corridorState.ParamsLong;
            var paramsDouble = corridorState.ParamsDouble;

            // Safe ParamAccess update to prevent resetting user-input parameters (e.g. Custom_H1, w, a)
            string[] doubleParams = { "w", "x1", "x2", "a", "Custom_H1", "Custom_b", "Custom_c", "Custom_d", "Custom_e", "Custom_f", "Custom_g", "Custom_H2", "PccThickness", "PccOffset", "FmThickness" };
            foreach (var name in doubleParams)
            {
                try
                {
                    var param = paramsDouble[name];
                    if (param != null) param.Access = ParamAccessType.InputAndOutput;
                }
                catch { }
            }

            string[] longParams = { "Side", "OperatingMode", "WallPreset" };
            foreach (var name in longParams)
            {
                try
                {
                    var param = paramsLong[name];
                    if (param != null) param.Access = ParamAccessType.InputAndOutput;
                }
                catch { }
            }
        }

        protected override void DrawImplement(CorridorState corridorState)
        {
            if (corridorState == null) return;

            var sideInt = GetIntParam(corridorState, "Side", 1);
            // Civil 3D maps Side parameter to integers based on the ATC enum.
            // Our ATC maps: Right = 0, Left = 1.
            var side = (sideInt == 1) ? RetainingWall.Core.Side.Left : RetainingWall.Core.Side.Right;
            double sign = side == RetainingWall.Core.Side.Left ? 1.0 : -1.0;

            // Fetch Core parameters
            var operatingMode = (OperatingMode)GetIntParam(corridorState, "OperatingMode", (int)WallConfig.DefaultOperatingMode);
            var wallPreset = (WallPreset)GetIntParam(corridorState, "WallPreset", (int)WallConfig.DefaultWallPreset);

            // Fetch Dimensions - Stem Notch & Batter
            double w = GetDoubleParam(corridorState, "w", WallConfig.DefaultW);
            double x1 = GetDoubleParam(corridorState, "x1", WallConfig.DefaultX1);
            double x2 = GetDoubleParam(corridorState, "x2", WallConfig.DefaultX2);
            double a = GetDoubleParam(corridorState, "a", WallConfig.DefaultA);

            // Fetch Custom parameters
            double custom_H1 = GetDoubleParam(corridorState, "Custom_H1", WallConfig.DefaultCustomH1);
            double custom_b = GetDoubleParam(corridorState, "Custom_b", WallConfig.DefaultCustomB);
            double custom_c = GetDoubleParam(corridorState, "Custom_c", WallConfig.DefaultCustomC);
            double custom_d = GetDoubleParam(corridorState, "Custom_d", WallConfig.DefaultCustomD);
            double custom_e = GetDoubleParam(corridorState, "Custom_e", WallConfig.DefaultCustomE);
            double custom_f = GetDoubleParam(corridorState, "Custom_f", WallConfig.DefaultCustomF);
            double custom_g = GetDoubleParam(corridorState, "Custom_g", WallConfig.DefaultCustomG);
            double custom_H2 = GetDoubleParam(corridorState, "Custom_H2", WallConfig.DefaultCustomH2);

            // Fetch Foundations & Backfill parameters
            double pccThickness = GetDoubleParam(corridorState, "PccThickness", WallConfig.DefaultPccThickness);
            double pccOffset = GetDoubleParam(corridorState, "PccOffset", WallConfig.DefaultPccOffset);
            double fmThickness = GetDoubleParam(corridorState, "FmThickness", WallConfig.DefaultFmThickness);

            // Parameter safety check and warning log
            if (operatingMode != OperatingMode.FullCustom)
            {
                if (Math.Abs(custom_H1 - WallConfig.DefaultCustomH1) > 0.001 ||
                    Math.Abs(custom_b - WallConfig.DefaultCustomB) > 0.001 ||
                    Math.Abs(custom_c - WallConfig.DefaultCustomC) > 0.001 ||
                    Math.Abs(custom_d - WallConfig.DefaultCustomD) > 0.001 ||
                    Math.Abs(custom_e - WallConfig.DefaultCustomE) > 0.001 ||
                    Math.Abs(custom_f - WallConfig.DefaultCustomF) > 0.001 ||
                    Math.Abs(custom_g - WallConfig.DefaultCustomG) > 0.001 ||
                    Math.Abs(custom_H2 - WallConfig.DefaultCustomH2) > 0.001)
                {
                    try
                    {
                        var doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
                        if (doc != null)
                        {
                            doc.Editor.WriteMessage("\n[Warning] Road Edge Retaining Wall: Custom height/footing/base parameters were modified, but Operating Mode is not set to Full Custom. These overrides are being ignored.\n");
                        }
                    }
                    catch { }

                    try
                    {
                        var eventLogType = Type.GetType("System.Diagnostics.EventLog, System.Diagnostics.EventLog, Version=8.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51")
                                           ?? Type.GetType("System.Diagnostics.EventLog, System.Diagnostics.EventLog");
                        if (eventLogType != null)
                        {
                            string sourceName = "Civil 3D Road Edge Retaining Wall Subassembly";
                            var sourceExistsMethod = eventLogType.GetMethod("SourceExists", new[] { typeof(string) });
                            if (sourceExistsMethod != null)
                            {
                                bool sourceExists = sourceExistsMethod.Invoke(null, new object[] { sourceName }) is bool val && val;
                                if (!sourceExists)
                                {
                                    var createSourceMethod = eventLogType.GetMethod("CreateEventSource", new[] { typeof(string), typeof(string) });
                                    if (createSourceMethod != null)
                                    {
                                        createSourceMethod.Invoke(null, new object[] { sourceName, "Application" });
                                    }
                                }
                            }
                            
                            var eventLogEntryType = Type.GetType("System.Diagnostics.EventLogEntryType, System.Diagnostics.EventLog, Version=8.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51")
                                                    ?? Type.GetType("System.Diagnostics.EventLogEntryType, System.Diagnostics.EventLog");
                            if (eventLogEntryType != null)
                            {
                                object warningEnum = Enum.Parse(eventLogEntryType, "Warning");
                                var writeEntryMethod = eventLogType.GetMethod("WriteEntry", new[] { typeof(string), typeof(string), eventLogEntryType });
                                if (writeEntryMethod != null)
                                {
                                    writeEntryMethod.Invoke(null, new object[] { 
                                        sourceName, 
                                        "Road Edge Retaining Wall: Custom height/footing/base parameters were modified, but Operating Mode is not set to Full Custom. These overrides are being ignored.", 
                                        warningEnum 
                                    });
                                }
                            }
                        }
                    }
                    catch { }
                }
            }

            // Read target surface elevation with absolute nearest boundary fallback
            double yGL = double.NaN;

            if (corridorState.Mode != CorridorMode.Layout)
            {
                try
                {
                    var db = HostApplicationServices.WorkingDatabase;
                    var tm = db.TransactionManager;

                    ObjectId alignmentId = ObjectId.Null;
                    double station = corridorState.CurrentStation;
                    double baselineOffset = corridorState.CurrentOffset;

                    bool isFixedAlignmentOffset = corridorState.CurrentAlignmentIsOffsetAlignment && corridorState.CurrentAssemblyOffsetIsFixed;
                    if (isFixedAlignmentOffset)
                    {
                        alignmentId = corridorState.CurrentBaselineId;
                        baselineOffset += corridorState.CurrentAssemblyFixedOffset;
                    }
                    else
                    {
                        alignmentId = corridorState.CurrentAlignmentId;
                    }

                    var paramsSurface = corridorState.ParamsSurface;
                    ObjectId surfaceId = paramsSurface.Value("GroundSurface");

                    if (surfaceId != ObjectId.Null && alignmentId != ObjectId.Null)
                    {
                        using (var trans = tm.StartTransaction())
                        {
                            var surface = (Autodesk.Civil.DatabaseServices.Surface)trans.GetObject(surfaceId, OpenMode.ForRead, false, false);
                            var alignment = (Alignment)trans.GetObject(alignmentId, OpenMode.ForRead, false, false);

                            // Target offset is -w * sign relative to baseline offset
                            double targetOffset = baselineOffset - (w * sign);

                            double east = 0.0, north = 0.0;
                            alignment.PointLocation(station, targetOffset, ref east, ref north);

                            try
                            {
                                yGL = surface.FindElevationAtXY(east, north);
                            }
                            catch { }

                            if (double.IsNaN(yGL))
                            {
                                // Out-of-Bounds Fallback Rule: Locate the absolute nearest boundary/perimeter point
                                // of the DTM surface along the current cross-section station perpendicular to alignment.
                                // Scan in 0.05m increments up to 100m in both directions from targetOffset.
                                double scanStep = 0.05;
                                double maxScanDistance = 100.0;
                                for (double dist = scanStep; dist <= maxScanDistance; dist += scanStep)
                                {
                                    // 1. Try going inwards (towards baseline)
                                    double offsetIn = targetOffset + (dist * sign);
                                    alignment.PointLocation(station, offsetIn, ref east, ref north);
                                    double elevIn = double.NaN;
                                    try { elevIn = surface.FindElevationAtXY(east, north); } catch { }

                                    // 2. Try going outwards (away from baseline)
                                    double offsetOut = targetOffset - (dist * sign);
                                    alignment.PointLocation(station, offsetOut, ref east, ref north);
                                    double elevOut = double.NaN;
                                    try { elevOut = surface.FindElevationAtXY(east, north); } catch { }

                                    if (!double.IsNaN(elevIn))
                                    {
                                        yGL = elevIn;
                                        break;
                                    }
                                    if (!double.IsNaN(elevOut))
                                    {
                                        yGL = elevOut;
                                        break;
                                    }
                                }

                                // Fallback to baselineOffset if still NaN after 100m scan
                                if (double.IsNaN(yGL))
                                {
                                    alignment.PointLocation(station, baselineOffset, ref east, ref north);
                                    try
                                    {
                                        yGL = surface.FindElevationAtXY(east, north);
                                    }
                                    catch { }
                                }
                            }

                            trans.Commit();
                        }
                    }
                }
                catch { }
            }

            // Convert to relative elevation (yGL relative to P1 baseline attachment point elevation)
            if (double.IsNaN(yGL))
            {
                yGL = 0.0;
            }
            else
            {
                yGL = yGL - corridorState.CurrentElevation;
            }

            double measuredH1;
            if (corridorState.Mode == CorridorMode.Layout)
            {
                measuredH1 = 3.0; // Use a sensible 3m default height in Assembly Layout view
            }
            else
            {
                measuredH1 = -yGL;
                if (measuredH1 < 0.1) measuredH1 = 0.1;
            }

            WallDimensions dims;
            if (operatingMode == OperatingMode.DynamicTable)
            {
                double lookupHeight = measuredH1;
                if (lookupHeight > 12.0)
                {
                    lookupHeight = 12.0;
                }
                var baseDims = WallConfig.GetDimensionsByHeight(lookupHeight);
                dims = new WallDimensions(
                    baseDims.A,
                    baseDims.B,
                    baseDims.C,
                    baseDims.D,
                    baseDims.E,
                    baseDims.F,
                    baseDims.G,
                    measuredH1,
                    baseDims.H2
                );
            }
            else if (operatingMode == OperatingMode.PresetDropdown)
            {
                int presetNum = (int)wallPreset;
                if (presetNum < 2) presetNum = 2;
                if (presetNum > 12) presetNum = 12;
                dims = WallConfig.GetDimensions(presetNum);
            }
            else // FullCustom
            {
                dims = new WallDimensions(a, custom_b, custom_c, custom_d, custom_e, custom_f, custom_g, custom_H1, custom_H2);
            }

            var generator = new WallGeometryGenerator
            {
                OperatingMode = operatingMode,
                WallPreset = wallPreset,
                
                w = w,
                x1 = x1,
                x2 = x2,
                a = a,
                
                Custom_H1 = custom_H1,
                Custom_b = custom_b,
                Custom_c = custom_c,
                Custom_d = custom_d,
                Custom_e = custom_e,
                Custom_f = custom_f,
                Custom_g = custom_g,
                Custom_H2 = custom_H2,
                
                PccThickness = pccThickness,
                PccOffset = pccOffset,
                FmThickness = fmThickness
            };

            // Generate geometry using the resolved side parameter.
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

            // Write back all calculated values of input/output parameters
            try
            {
                WriteIntParam(corridorState, "Side", sideInt);
                WriteIntParam(corridorState, "OperatingMode", (int)operatingMode);
                WriteIntParam(corridorState, "WallPreset", (int)wallPreset);

                WriteDoubleParam(corridorState, "w", w);
                WriteDoubleParam(corridorState, "x1", x1);
                WriteDoubleParam(corridorState, "x2", x2);
                WriteDoubleParam(corridorState, "a", a);

                // Write back the active/resolved dimensions to Custom_* parameters
                WriteDoubleParam(corridorState, "Custom_H1", dims.H1);
                WriteDoubleParam(corridorState, "Custom_b", dims.B);
                WriteDoubleParam(corridorState, "Custom_c", dims.C);
                WriteDoubleParam(corridorState, "Custom_d", dims.D);
                WriteDoubleParam(corridorState, "Custom_e", dims.E);
                WriteDoubleParam(corridorState, "Custom_f", dims.F);
                WriteDoubleParam(corridorState, "Custom_g", dims.G);
                WriteDoubleParam(corridorState, "Custom_H2", dims.H2);

                WriteDoubleParam(corridorState, "PccThickness", pccThickness);
                WriteDoubleParam(corridorState, "PccOffset", pccOffset);
                WriteDoubleParam(corridorState, "FmThickness", fmThickness);
            }
            catch { }
        }

        private void WriteDoubleParam(CorridorState corridorState, string name, double value)
        {
            try
            {
                var param = corridorState.ParamsDouble[name];
                if (param != null)
                {
                    param.Value = value;
                }
                else
                {
                    corridorState.ParamsDouble.Add(name, value);
                }
            }
            catch { }
        }

        private void WriteIntParam(CorridorState corridorState, string name, int value)
        {
            try
            {
                var param = corridorState.ParamsLong[name];
                if (param != null)
                {
                    param.Value = value;
                }
                else
                {
                    corridorState.ParamsLong.Add(name, value);
                }
            }
            catch { }
        }

        private string GetStringParam(CorridorState corridorState, string name, string defaultValue)
        {
            try
            {
                var param = corridorState.ParamsString[name];
                return param != null ? param.Value : defaultValue;
            }
            catch { return defaultValue; }
        }

        private int GetIntParam(CorridorState corridorState, string name, int defaultValue)
        {
            try
            {
                var param = corridorState.ParamsLong[name];
                return param != null ? Convert.ToInt32(param.Value) : defaultValue;
            }
            catch { return defaultValue; }
        }

        private double GetDoubleParam(CorridorState corridorState, string name, double defaultValue)
        {
            try
            {
                var param = corridorState.ParamsDouble[name];
                return param != null ? Convert.ToDouble(param.Value) : defaultValue;
            }
            catch { return defaultValue; }
        }

        private bool GetBoolParam(CorridorState corridorState, string name, bool defaultValue)
        {
            try
            {
                var param = corridorState.ParamsBool[name];
                return param != null ? Convert.ToBoolean(param.Value) : defaultValue;
            }
            catch { return defaultValue; }
        }
    }
}
