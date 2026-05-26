using System;
using System.Collections.Generic;

namespace RetainingWall.Core
{
    /// <summary>
    /// Generates retaining wall geometry based on customizable parameters.
    /// </summary>
    public class WallGeometryGenerator
    {
        // Core Configuration Control
        public OperatingMode OperatingMode { get; set; } = WallConfig.DefaultOperatingMode;
        public WallPreset WallPreset { get; set; } = WallConfig.DefaultWallPreset;

        // Dimensions - Stem Notch & Batter
        public double w { get; set; } = WallConfig.DefaultW;
        public double x1 { get; set; } = WallConfig.DefaultX1;
        public double x2 { get; set; } = WallConfig.DefaultX2;
        public double a { get; set; } = WallConfig.DefaultA;

        // Dimensions - Custom Footing & Base (Used only in Full Custom)
        public double Custom_H1 { get; set; } = WallConfig.DefaultCustomH1;
        public double Custom_b { get; set; } = WallConfig.DefaultCustomB;
        public double Custom_c { get; set; } = WallConfig.DefaultCustomC;
        public double Custom_d { get; set; } = WallConfig.DefaultCustomD;
        public double Custom_e { get; set; } = WallConfig.DefaultCustomE;
        public double Custom_f { get; set; } = WallConfig.DefaultCustomF;
        public double Custom_g { get; set; } = WallConfig.DefaultCustomG;
        public double Custom_H2 { get; set; } = WallConfig.DefaultCustomH2;

        // Foundations - PCC Mud Mat
        public double PccThickness { get; set; } = WallConfig.DefaultPccThickness;
        public double PccOffset { get; set; } = WallConfig.DefaultPccOffset;

        // Backfill - Filter Media
        public double FmThickness { get; set; } = WallConfig.DefaultFmThickness;

        public GeometryResult Generate(WallDimensions dims, Side side)
        {
            var result = new GeometryResult();
            double sign = side == Side.Left ? 1.0 : -1.0;

            // Step 1: Resolve dimensions based on operating mode
            double H1, H2, b, c, d, e, f, g;

            if (OperatingMode == OperatingMode.FullCustom)
            {
                // In Full Custom mode, use Custom_* dimensions directly.
                H1 = Custom_H1;
                H2 = Custom_H2;
                b = Custom_b;
                c = Custom_c;
                d = Custom_d;
                e = Custom_e;
                f = Custom_f;
                g = Custom_g;
            }
            else
            {
                // For DynamicTable and PresetDropdown modes, dimensions are loaded from WallCaseTable (passed in dims)
                H1 = dims.H1;
                H2 = dims.H2;
                b = dims.B;
                c = dims.C;
                d = dims.D;
                e = dims.E;
                f = dims.F;
                g = dims.G;
            }

            // Step 2: Compute sequential raw coordinates relative to P1(0,0) as if on Side.Right (sign = 1.0)
            double p1_x = 0.0;
            double p1_y = 0.0;
            
            double p2_x = p1_x - w;
            double p2_y = p1_y;
            
            double p11_x = p1_x;
            double p11_y = p1_y - x1;
            
            double p10_x = p2_x + a;
            double p10_y = p11_y - x2;
            
            double p3_x = p2_x;
            double p3_y = p2_y - H1;
            
            double p4_x = p2_x;
            double p4_y = p3_y - H2 + f;
            
            double p5_x = p4_x - d;
            double p5_y = p3_y - H2 + g;
            
            double p6_x = p5_x;
            double p6_y = p3_y - H2;
            
            double p7_x = p4_x + b + c;
            double p7_y = p3_y - H2;
            
            double p8_x = p7_x;
            double p8_y = p7_y + e;
            
            double p9_x = p4_x + b;
            double p9_y = p7_y + f;
            
            double p12_x = p6_x - PccOffset;
            double p12_y = p6_y;
            
            double p13_x = p12_x;
            double p13_y = p12_y - PccThickness;
            
            double p14_x = p7_x + PccOffset;
            double p14_y = p13_y;
            
            double p15_x = p14_x;
            double p15_y = p7_y;
            
            double p16_x = p10_x + FmThickness;
            double p16_y = p10_y;
            
            // P18 is back-face vector intersection with the ground line plane at Y = -H1.
            // Vector is between P10 and P9.
            // Equation of line: X = P10.X + (Y - P10.Y) * ((P9.X - P10.X) / (P9.Y - P10.Y))
            // We evaluate at Y = -H1.
            double p18_y = -H1;
            double p18_x = p10_x;
            double divisor = p9_y - p10_y;
            if (Math.Abs(divisor) > 1e-6)
            {
                p18_x = p10_x + ((-H1) - p10_y) * ((p9_x - p10_x) / divisor);
            }
            
            double p17_x = p18_x + FmThickness;
            double p17_y = -H1;

            // Step 3: Add Points to GeometryResult (apply sign to X coordinates)
            var P1  = AddPoint(result, p1_x * sign, p1_y, "RW_Attach", "RW_P1", "Hinge", "Top");
            var P2  = AddPoint(result, p2_x * sign, p2_y, "RW_TopStemFront", "RW_P2", "Top");
            var P3  = AddPoint(result, p3_x * sign, p3_y, "RW_BaseStemFront", "RW_P3", "RW_H1");
            var P4  = AddPoint(result, p4_x * sign, p4_y, "RW_TopToe", "RW_P4", "Top");
            var P5  = AddPoint(result, p5_x * sign, p5_y, "RW_OuterToe", "RW_P5");
            var P6  = AddPoint(result, p6_x * sign, p6_y, "RW_FootingToe", "RW_P6", "Datum");
            var P7  = AddPoint(result, p7_x * sign, p7_y, "RW_FootingHeel", "RW_P7", "Datum");
            var P8  = AddPoint(result, p8_x * sign, p8_y, "RW_OuterHeel", "RW_P8");
            var P9  = AddPoint(result, p9_x * sign, p9_y, "RW_TopHeel", "RW_P9");
            var P10 = AddPoint(result, p10_x * sign, p10_y, "RW_NotchBottom", "RW_P10");
            var P11 = AddPoint(result, p11_x * sign, p11_y, "RW_NotchTop", "RW_P11", "Top");
            
            var P12 = AddPoint(result, p12_x * sign, p12_y, "RW_PccLeft", "RW_P12");
            var P13 = AddPoint(result, p13_x * sign, p13_y, "RW_PccBottomLeft", "RW_P13", "Datum");
            var P14 = AddPoint(result, p14_x * sign, p14_y, "RW_PccBottomRight", "RW_P14", "Datum");
            var P15 = AddPoint(result, p15_x * sign, p15_y, "RW_PccRight", "RW_P15");
            
            var P16 = AddPoint(result, p16_x * sign, p16_y, "RW_FilterTopRight", "RW_P16", "Top");
            var P18 = AddPoint(result, p18_x * sign, p18_y, "RW_FilterBottomLeft", "RW_P18", "RW_H1", "Datum");
            var P17 = AddPoint(result, p17_x * sign, p17_y, "RW_FilterBottomRight", "RW_P17", "Datum");

            // Step 4: Add Links to GeometryResult
            // Shape 1 (Main Structure) Links: P1 -> P2 -> P3 -> P4 -> P5 -> P6 -> P7 -> P8 -> P9 -> P10 -> P11 -> P1
            var l1  = AddLink(result, P1, P2, "RW_Link_TopStem", "RW_Top", "Top", "RW_Concrete");
            var l2  = AddLink(result, P2, P3, "RW_Link_StemFront", "RW_Wall", "RW_Concrete");
            var l3  = AddLink(result, P3, P4, "RW_Link_StemBaseFront", "RW_Wall", "RW_Concrete");
            var l4  = AddLink(result, P4, P5, "RW_Link_ToeTop", "RW_Footing", "Top", "RW_Concrete");
            var l5  = AddLink(result, P5, P6, "RW_Link_ToeOuter", "RW_Footing", "RW_Concrete");
            var l6  = AddLink(result, P6, P7, "RW_Link_FootingBottom", "RW_Footing", "RW_Datum", "Datum", "RW_Concrete");
            var l7  = AddLink(result, P7, P8, "RW_Link_HeelOuter", "RW_Footing", "RW_Concrete");
            var l8  = AddLink(result, P8, P9, "RW_Link_HeelTop", "RW_Footing", "RW_Concrete");
            var l9  = AddLink(result, P9, P10, "RW_Link_StemBack", "RW_Wall", "RW_Concrete");
            var l10 = AddLink(result, P10, P11, "RW_Link_NotchSlope", "RW_Wall", "RW_Concrete");
            var l11 = AddLink(result, P11, P1, "RW_Link_NotchDrop", "RW_Wall", "RW_Concrete");

            // Shape 2 (PCC Bed) Links: P6 -> P12 -> P13 -> P14 -> P15 -> P7 -> P6
            var l12 = AddLink(result, P6, P12, "RW_Link_PccOffsetLeft", "RW_PCC");
            var l13 = AddLink(result, P12, P13, "RW_Link_PccLeftFace", "RW_PCC", "RW_Datum", "Datum");
            var l14 = AddLink(result, P13, P14, "RW_Link_PccBottom", "RW_PCC", "RW_Datum", "Datum");
            var l15 = AddLink(result, P14, P15, "RW_Link_PccRightFace", "RW_PCC", "RW_Datum", "Datum");
            var l16 = AddLink(result, P15, P7, "RW_Link_PccOffsetRight", "RW_PCC");

            // Shape 3 (Filter Media) Links: P10 -> P16 -> P17 -> P18 -> P10
            var l17 = AddLink(result, P10, P16, "RW_Link_FilterTop", "RW_Filter", "Top");
            var l18 = AddLink(result, P16, P17, "RW_Link_FilterOuter", "RW_Filter", "RW_Datum", "Datum");
            var l19 = AddLink(result, P17, P18, "RW_Link_FilterBottom", "RW_Filter");
            var l20 = AddLink(result, P18, P10, "RW_Link_FilterInner", "RW_Filter");

            // Step 5: Add Shapes to GeometryResult
            result.Shapes.Add(new GeometryShape(new List<GeometryLink> { l1, l2, l3, l4, l5, l6, l7, l8, l9, l10, l11 }, "Structure_Concrete", "Concrete"));
            result.Shapes.Add(new GeometryShape(new List<GeometryLink> { l12, l13, l14, l15, l16, l6 }, "PCC_Lean_Concrete", "Subbase"));
            result.Shapes.Add(new GeometryShape(new List<GeometryLink> { l17, l18, l19, l20 }, "Filter_Drainage_Media", "Gravel"));

            return result;
        }

        private GeometryPoint AddPoint(GeometryResult result, double x, double y, params string[] codes)
        {
            var p = new GeometryPoint(x, y, codes);
            result.Points.Add(p);
            return p;
        }

        private GeometryLink AddLink(GeometryResult result, GeometryPoint p1, GeometryPoint p2, params string[] codes)
        {
            var l = new GeometryLink(p1, p2, codes);
            result.Links.Add(l);
            return l;
        }
    }
}
