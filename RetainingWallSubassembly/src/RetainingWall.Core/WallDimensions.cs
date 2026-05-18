using System;

namespace RetainingWall.Core
{
    /// <summary>
    /// Represents the dimensions of a retaining wall.
    /// All values are stored internally in metres.
    /// </summary>
    public class WallDimensions
    {
        /// <summary>Top width of stem (a) in metres.</summary>
        public double A { get; }
        /// <summary>Bottom stem width (b) in metres.</summary>
        public double B { get; }
        /// <summary>Toe width (c) in metres.</summary>
        public double C { get; }
        /// <summary>Heel width (d) in metres.</summary>
        public double D { get; }
        /// <summary>Toe thickness (e) in metres.</summary>
        public double E { get; }
        /// <summary>Footing thickness at stem junction (f) in metres.</summary>
        public double F { get; }
        /// <summary>Heel thickness (g) in metres.</summary>
        public double G { get; }
        /// <summary>Total height (H1) in metres.</summary>
        public double H1 { get; }
        /// <summary>Shear key depth / Footing depth (H2) in metres.</summary>
        public double H2 { get; }

        public WallDimensions(double a, double b, double c, double d, double e, double f, double g, double h1, double h2)
        {
            A = a;
            B = b;
            C = c;
            D = d;
            E = e;
            F = f;
            G = g;
            H1 = h1;
            H2 = h2;
        }
    }
}
