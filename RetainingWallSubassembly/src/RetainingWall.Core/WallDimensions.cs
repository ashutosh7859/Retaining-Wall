using System;

namespace RetainingWall.Core
{
    /// <summary>
    /// Represents the standard dimensions for a retaining wall case.
    /// All values are stored internally in metres.
    /// </summary>
    public class WallDimensions
    {
        /// <summary>Dimension A in metres.</summary>
        public double A { get; }
        /// <summary>Dimension B in metres.</summary>
        public double B { get; }
        /// <summary>Dimension C in metres.</summary>
        public double C { get; }
        /// <summary>Dimension D in metres.</summary>
        public double D { get; }
        /// <summary>Dimension E in metres.</summary>
        public double E { get; }
        /// <summary>Dimension F in metres.</summary>
        public double F { get; }
        /// <summary>Dimension G in metres.</summary>
        public double G { get; }
        /// <summary>Dimension H1 in metres.</summary>
        public double H1 { get; }
        /// <summary>Dimension H2 in metres.</summary>
        public double H2 { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WallDimensions"/> class with values in metres.
        /// </summary>
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
