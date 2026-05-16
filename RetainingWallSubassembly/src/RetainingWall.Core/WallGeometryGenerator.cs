using System.Collections.Generic;
using System.Linq;

namespace RetainingWall.Core
{
    /// <summary>
    /// Generates retaining wall geometry based on parameters.
    /// </summary>
    public class WallGeometryGenerator
    {
        public double PccThickness { get; set; } = 0.150;
        public double PccProjection { get; set; } = 0.150;
        public double FilterThickness { get; set; } = 0.600;
        public double WallTopOffset { get; set; } = 0.450;

        public GeometryResult Generate(WallDimensions dims, Side side)
        {
            var result = new GeometryResult();
            int sign = side == Side.Left ? -1 : 1;

            // Define points mathematically assuming Left side (negative X).
            // We'll apply the sign multiplier at the end.

            // The exact mapping to typical retaining walls:
            // Top Width = A
            // Bottom Stem Width = B
            // Toe Width = C
            // Heel Width = D
            // Toe Thickness = E
            // Heel Thickness = G
            // Total Height = H1
            // (Assuming H2 is some other depth, we'll use it if needed, but H1, A, B, C, D, E, G are sufficient for standard shape).

            double xAttach = 0;
            double yAttach = 0;

            double xTopBack = WallTopOffset;
            double yTopBack = 0;

            double xTopFront = xTopBack + dims.A;
            double yTopFront = 0;

            double yStemBottomFront = -dims.H1 + dims.E;
            double xStemBottomFront = xTopBack + dims.B;

            double yStemBottomBack = -dims.H1 + dims.G;
            double xStemBottomBack = xTopBack;

            // Toe
            double xToeTop = xStemBottomFront + dims.C;
            double yToeTop = yStemBottomFront;
            double xToeBot = xToeTop;
            double yToeBot = -dims.H1;

            // Heel
            double xHeelTop = xStemBottomBack - dims.D;
            double yHeelTop = yStemBottomBack;
            double xHeelBot = xHeelTop;
            double yHeelBot = -dims.H1;

            double xStemBotFrontFooting = xStemBottomFront;
            double yStemBotFrontFooting = -dims.H1;

            double xStemBotBackFooting = xStemBottomBack;
            double yStemBotBackFooting = -dims.H1;

            // Create Points (apply sign)
            var pAttach = AddPoint(result, xAttach * sign, yAttach, "RW_Attach");
            var pTopBack = AddPoint(result, xTopBack * sign, yTopBack, "RW_TopBack");
            var pTopFront = AddPoint(result, xTopFront * sign, yTopFront, "RW_TopFront");
            
            var pStemBotFront = AddPoint(result, xStemBottomFront * sign, yStemBottomFront);
            var pToeTop = AddPoint(result, xToeTop * sign, yToeTop);
            var pToeBot = AddPoint(result, xToeBot * sign, yToeBot, "RW_FootingToe");
            var pHeelBot = AddPoint(result, xHeelBot * sign, yHeelBot, "RW_FootingHeel");
            var pHeelTop = AddPoint(result, xHeelTop * sign, yHeelTop);
            var pStemBotBack = AddPoint(result, xStemBottomBack * sign, yStemBottomBack);

            // Concrete Links
            var lTop = AddLink(result, pTopBack, pTopFront, "RW_Top", "RW_Concrete");
            var lFront = AddLink(result, pTopFront, pStemBotFront, "RW_Wall", "RW_Concrete");
            var lToeTop = AddLink(result, pStemBotFront, pToeTop, "RW_Footing", "RW_Concrete");
            var lToeFace = AddLink(result, pToeTop, pToeBot, "RW_Footing", "RW_Concrete");
            var lBot = AddLink(result, pToeBot, pHeelBot, "RW_Footing", "RW_Datum", "RW_Concrete");
            var lHeelFace = AddLink(result, pHeelBot, pHeelTop, "RW_Footing", "RW_Concrete");
            var lHeelTop = AddLink(result, pHeelTop, pStemBotBack, "RW_Footing", "RW_Concrete");
            var lBack = AddLink(result, pStemBotBack, pTopBack, "RW_Wall", "RW_Concrete");

            result.Shapes.Add(new GeometryShape(new List<GeometryLink> { lTop, lFront, lToeTop, lToeFace, lBot, lHeelFace, lHeelTop, lBack }, "RW_Concrete"));

            // PCC
            double xPccToe = xToeBot + PccProjection;
            double yPccBot = yToeBot - PccThickness;
            double xPccHeel = xHeelBot - PccProjection;
            
            var pPccToeTop = AddPoint(result, xToeBot * sign, yToeBot);
            var pPccHeelTop = AddPoint(result, xHeelBot * sign, yHeelBot);

            var pPccToeBot = AddPoint(result, xPccToe * sign, yPccBot, "RW_PccLeft"); // Assuming Left means outer
            var pPccHeelBot = AddPoint(result, xPccHeel * sign, yPccBot, "RW_PccRight"); // Assuming Right means inner

            var pPccToeProj = AddPoint(result, xPccToe * sign, yToeBot);
            var pPccHeelProj = AddPoint(result, xPccHeel * sign, yHeelBot);

            var lPccTop = AddLink(result, pPccHeelTop, pPccToeTop, "RW_PCC"); // Same as concrete bottom but reverse or shared
            var lPccToeFace = AddLink(result, pPccToeTop, pPccToeProj, "RW_PCC");
            var lPccToeSide = AddLink(result, pPccToeProj, pPccToeBot, "RW_PCC", "RW_Datum");
            var lPccBot = AddLink(result, pPccToeBot, pPccHeelBot, "RW_PCC", "RW_Datum");
            var lPccHeelSide = AddLink(result, pPccHeelBot, pPccHeelProj, "RW_PCC", "RW_Datum");
            var lPccHeelFace = AddLink(result, pPccHeelProj, pPccHeelTop, "RW_PCC");

            result.Shapes.Add(new GeometryShape(new List<GeometryLink> { lPccTop, lPccToeFace, lPccToeSide, lPccBot, lPccHeelSide, lPccHeelFace }, "RW_PCC"));

            // Filter (Behind Wall)
            double xFilterTop = xTopBack - FilterThickness;
            double xFilterHeel = xHeelTop - FilterThickness;
            
            var pFilterTopBack = AddPoint(result, xFilterTop * sign, yTopBack);
            var pFilterHeelBack = AddPoint(result, xFilterHeel * sign, yHeelTop);

            var lFilterTop = AddLink(result, pFilterTopBack, pTopBack, "RW_Filter");
            var lFilterBackFace = AddLink(result, pTopBack, pStemBotBack, "RW_Filter");
            var lFilterHeelTop = AddLink(result, pStemBotBack, pHeelTop, "RW_Filter");
            var lFilterBot = AddLink(result, pHeelTop, pFilterHeelBack, "RW_Filter");
            var lFilterOuter = AddLink(result, pFilterHeelBack, pFilterTopBack, "RW_Filter", "RW_Datum");

            result.Shapes.Add(new GeometryShape(new List<GeometryLink> { lFilterTop, lFilterBackFace, lFilterHeelTop, lFilterBot, lFilterOuter }, "RW_FilterMedia"));

            // Attach link
            AddLink(result, pAttach, pTopBack, "RW_Top");

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
