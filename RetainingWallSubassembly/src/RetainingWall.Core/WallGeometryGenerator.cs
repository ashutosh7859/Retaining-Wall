using System.Collections.Generic;

namespace RetainingWall.Core
{
    /// <summary>
    /// Generates retaining wall geometry based on customizable parameters.
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

            double xAttach = 0;
            double yAttach = 0;

            double xTopBack = WallTopOffset;
            double yTopBack = 0;

            double xTopFront = xTopBack + dims.A;
            double yTopFront = 0;

            double yStemBottom = -dims.H1 + dims.F;
            
            double xStemBottomBack = xTopBack;
            double xStemBottomFront = xTopBack + dims.B;

            double xToeTop = xStemBottomFront + dims.C;
            double yToeTop = -dims.H1 + dims.E;
            double xToeBot = xToeTop;
            double yToeBot = -dims.H1;

            double xHeelTop = xStemBottomBack - dims.D;
            double yHeelTop = -dims.H1 + dims.G;
            double xHeelBot = xHeelTop;
            double yHeelBot = -dims.H1;

            // Shear Key logic using H2
            bool hasKey = dims.H2 > 0;
            double yKeyBot = -dims.H1 - dims.H2;
            double xKeyBack = xStemBottomBack;
            double xKeyFront = xKeyBack + dims.A; // Key width matches stem top width A for proportional structural sizing
            
            if (xKeyFront > xStemBottomFront)
                xKeyFront = xStemBottomFront;

            // --- Points ---
            var pAttach = AddPoint(result, xAttach * sign, yAttach, "RW_Attach");
            var pTopBack = AddPoint(result, xTopBack * sign, yTopBack, "RW_TopBack");
            var pTopFront = AddPoint(result, xTopFront * sign, yTopFront, "RW_TopFront");
            
            var pStemBotFront = AddPoint(result, xStemBottomFront * sign, yStemBottom);
            var pToeTop = AddPoint(result, xToeTop * sign, yToeTop);
            var pToeBot = AddPoint(result, xToeBot * sign, yToeBot, "RW_FootingToe");
            
            GeometryPoint pKeyFrontTop = null, pKeyFrontBot = null, pKeyBackBot = null, pKeyBackTop = null;
            if (hasKey)
            {
                pKeyFrontTop = AddPoint(result, xKeyFront * sign, yToeBot);
                pKeyFrontBot = AddPoint(result, xKeyFront * sign, yKeyBot, "RW_Key");
                pKeyBackBot = AddPoint(result, xKeyBack * sign, yKeyBot, "RW_Key");
                pKeyBackTop = AddPoint(result, xKeyBack * sign, yHeelBot);
            }

            var pHeelBot = AddPoint(result, xHeelBot * sign, yHeelBot, "RW_FootingHeel");
            var pHeelTop = AddPoint(result, xHeelTop * sign, yHeelTop);
            var pStemBotBack = AddPoint(result, xStemBottomBack * sign, yStemBottom);

            // --- Concrete Links ---
            var linksConcrete = new List<GeometryLink>();
            linksConcrete.Add(AddLink(result, pTopBack, pTopFront, "RW_Top", "RW_Concrete"));
            linksConcrete.Add(AddLink(result, pTopFront, pStemBotFront, "RW_Wall", "RW_Concrete"));
            linksConcrete.Add(AddLink(result, pStemBotFront, pToeTop, "RW_Footing", "RW_Concrete"));
            linksConcrete.Add(AddLink(result, pToeTop, pToeBot, "RW_Footing", "RW_Concrete"));
            
            if (hasKey)
            {
                linksConcrete.Add(AddLink(result, pToeBot, pKeyFrontTop, "RW_Footing", "RW_Datum", "RW_Concrete"));
                linksConcrete.Add(AddLink(result, pKeyFrontTop, pKeyFrontBot, "RW_Footing", "RW_Datum", "RW_Concrete"));
                linksConcrete.Add(AddLink(result, pKeyFrontBot, pKeyBackBot, "RW_Footing", "RW_Datum", "RW_Concrete"));
                linksConcrete.Add(AddLink(result, pKeyBackBot, pKeyBackTop, "RW_Footing", "RW_Datum", "RW_Concrete"));
                linksConcrete.Add(AddLink(result, pKeyBackTop, pHeelBot, "RW_Footing", "RW_Datum", "RW_Concrete"));
            }
            else
            {
                linksConcrete.Add(AddLink(result, pToeBot, pHeelBot, "RW_Footing", "RW_Datum", "RW_Concrete"));
            }

            linksConcrete.Add(AddLink(result, pHeelBot, pHeelTop, "RW_Footing", "RW_Concrete"));
            linksConcrete.Add(AddLink(result, pHeelTop, pStemBotBack, "RW_Footing", "RW_Concrete"));
            linksConcrete.Add(AddLink(result, pStemBotBack, pTopBack, "RW_Wall", "RW_Concrete"));

            result.Shapes.Add(new GeometryShape(linksConcrete, "RW_Concrete"));

            // --- PCC Links ---
            var linksPcc = new List<GeometryLink>();
            double xPccToe = xToeBot + PccProjection;
            double xPccHeel = xHeelBot - PccProjection;
            
            var pPccToeTop = AddPoint(result, xToeBot * sign, yToeBot);
            var pPccHeelTop = AddPoint(result, xHeelBot * sign, yHeelBot);
            var pPccToeProj = AddPoint(result, xPccToe * sign, yToeBot);
            var pPccHeelProj = AddPoint(result, xPccHeel * sign, yHeelBot);
            
            var pPccToeBot = AddPoint(result, xPccToe * sign, yToeBot - PccThickness, "RW_PccLeft"); 
            var pPccHeelBot = AddPoint(result, xPccHeel * sign, yHeelBot - PccThickness, "RW_PccRight"); 

            // Left edge
            linksPcc.Add(AddLink(result, pPccToeTop, pPccToeProj, "RW_PCC"));
            linksPcc.Add(AddLink(result, pPccToeProj, pPccToeBot, "RW_PCC", "RW_Datum"));

            // Bottom edge (incorporating key if active)
            if (hasKey)
            {
                var pPccKeyFrontTop = AddPoint(result, xKeyFront * sign, yToeBot - PccThickness);
                var pPccKeyFrontBot = AddPoint(result, xKeyFront * sign, yKeyBot - PccThickness);
                var pPccKeyBackBot = AddPoint(result, xKeyBack * sign, yKeyBot - PccThickness);
                var pPccKeyBackTop = AddPoint(result, xKeyBack * sign, yHeelBot - PccThickness);

                linksPcc.Add(AddLink(result, pPccToeBot, pPccKeyFrontTop, "RW_PCC", "RW_Datum"));
                linksPcc.Add(AddLink(result, pPccKeyFrontTop, pPccKeyFrontBot, "RW_PCC", "RW_Datum"));
                linksPcc.Add(AddLink(result, pPccKeyFrontBot, pPccKeyBackBot, "RW_PCC", "RW_Datum"));
                linksPcc.Add(AddLink(result, pPccKeyBackBot, pPccKeyBackTop, "RW_PCC", "RW_Datum"));
                linksPcc.Add(AddLink(result, pPccKeyBackTop, pPccHeelBot, "RW_PCC", "RW_Datum"));
            }
            else
            {
                linksPcc.Add(AddLink(result, pPccToeBot, pPccHeelBot, "RW_PCC", "RW_Datum"));
            }

            // Right edge
            linksPcc.Add(AddLink(result, pPccHeelBot, pPccHeelProj, "RW_PCC", "RW_Datum"));
            linksPcc.Add(AddLink(result, pPccHeelProj, pPccHeelTop, "RW_PCC"));

            // Top boundary (sharing or mirroring footing bottom)
            if (hasKey)
            {
                linksPcc.Add(AddLink(result, pPccHeelTop, pKeyBackTop, "RW_PCC"));
                linksPcc.Add(AddLink(result, pKeyBackTop, pKeyBackBot, "RW_PCC"));
                linksPcc.Add(AddLink(result, pKeyBackBot, pKeyFrontBot, "RW_PCC"));
                linksPcc.Add(AddLink(result, pKeyFrontBot, pKeyFrontTop, "RW_PCC"));
                linksPcc.Add(AddLink(result, pKeyFrontTop, pPccToeTop, "RW_PCC"));
            }
            else
            {
                linksPcc.Add(AddLink(result, pPccHeelTop, pPccToeTop, "RW_PCC"));
            }

            result.Shapes.Add(new GeometryShape(linksPcc, "RW_PCC"));

            // --- Filter ---
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
