using System;

namespace RetainingWall.Core
{
    /// <summary>
    /// Represents the dimensions of a retaining wall.
    /// All values are stored internally in metres.
    /// </summary>
    public class WallDimensions
    {
        /// <summary>Recess thickness at P10 (a) in metres.</summary>
        public double A { get; }
        /// <summary>Stem thickness at footing plane (b) in metres.</summary>
        public double B { get; }
        /// <summary>Heel projection length (c) in metres.</summary>
        public double C { get; }
        /// <summary>Toe projection length (d) in metres.</summary>
        public double D { get; }
        /// <summary>Vertical height of outer heel face (e) in metres.</summary>
        public double E { get; }
        /// <summary>Maximum footing depth at stem junction (f) in metres.</summary>
        public double F { get; }
        /// <summary>Vertical height of outer toe face (g) in metres.</summary>
        public double G { get; }
        /// <summary>Total height (H1) in metres.</summary>
        public double H1 { get; }
        /// <summary>Foundation depth from GL to footing bottom (H2) in metres.</summary>
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
