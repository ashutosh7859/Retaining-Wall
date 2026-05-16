using System;

namespace RetainingWall.Core
{
    /// <summary>
    /// Represents overrides for standard retaining wall dimensions.
    /// </summary>
    public class WallDimensionOverrides
    {
        /// <summary>Gets or sets a value indicating whether dimension overrides should be used.</summary>
        public bool UseDimensionOverrides { get; set; }
        
        /// <summary>Override for Dimension A in metres.</summary>
        public double OverrideA { get; set; }
        /// <summary>Override for Dimension B in metres.</summary>
        public double OverrideB { get; set; }
        /// <summary>Override for Dimension C in metres.</summary>
        public double OverrideC { get; set; }
        /// <summary>Override for Dimension D in metres.</summary>
        public double OverrideD { get; set; }
        /// <summary>Override for Dimension E in metres.</summary>
        public double OverrideE { get; set; }
        /// <summary>Override for Dimension F in metres.</summary>
        public double OverrideF { get; set; }
        /// <summary>Override for Dimension G in metres.</summary>
        public double OverrideG { get; set; }
        /// <summary>Override for Dimension H1 in metres.</summary>
        public double OverrideH1 { get; set; }
        /// <summary>Override for Dimension H2 in metres.</summary>
        public double OverrideH2 { get; set; }

        /// <summary>
        /// Applies the overrides to the specified base dimensions.
        /// If <see cref="UseDimensionOverrides"/> is false, the base dimensions are returned.
        /// If an override value is greater than zero, it replaces the base dimension.
        /// </summary>
        /// <param name="baseDimensions">The standard dimensions from the wall case table.</param>
        /// <returns>A new <see cref="WallDimensions"/> instance with overrides applied.</returns>
        public WallDimensions Apply(WallDimensions baseDimensions)
        {
            if (!UseDimensionOverrides)
            {
                return baseDimensions;
            }

            return new WallDimensions(
                OverrideA > 0 ? OverrideA : baseDimensions.A,
                OverrideB > 0 ? OverrideB : baseDimensions.B,
                OverrideC > 0 ? OverrideC : baseDimensions.C,
                OverrideD > 0 ? OverrideD : baseDimensions.D,
                OverrideE > 0 ? OverrideE : baseDimensions.E,
                OverrideF > 0 ? OverrideF : baseDimensions.F,
                OverrideG > 0 ? OverrideG : baseDimensions.G,
                OverrideH1 > 0 ? OverrideH1 : baseDimensions.H1,
                OverrideH2 > 0 ? OverrideH2 : baseDimensions.H2
            );
        }
    }
}
