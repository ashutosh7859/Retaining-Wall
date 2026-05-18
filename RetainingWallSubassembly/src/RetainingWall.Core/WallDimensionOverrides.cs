using System;

namespace RetainingWall.Core
{
    /// <summary>
    /// Manages the application of manual dimension overrides over base case dimensions.
    /// </summary>
    public class WallDimensionOverrides
    {
        public bool UseDimensionOverrides { get; set; }
        public double OverrideA { get; set; }
        public double OverrideB { get; set; }
        public double OverrideC { get; set; }
        public double OverrideD { get; set; }
        public double OverrideE { get; set; }
        public double OverrideF { get; set; }
        public double OverrideG { get; set; }
        public double OverrideH1 { get; set; }
        public double OverrideH2 { get; set; }

        /// <summary>
        /// Applies enabled positive overrides over the specified base dimensions.
        /// </summary>
        public WallDimensions Apply(WallDimensions baseDims)
        {
            if (!UseDimensionOverrides) return baseDims;

            return new WallDimensions(
                OverrideA > 0 ? OverrideA : baseDims.A,
                OverrideB > 0 ? OverrideB : baseDims.B,
                OverrideC > 0 ? OverrideC : baseDims.C,
                OverrideD > 0 ? OverrideD : baseDims.D,
                OverrideE > 0 ? OverrideE : baseDims.E,
                OverrideF > 0 ? OverrideF : baseDims.F,
                OverrideG > 0 ? OverrideG : baseDims.G,
                OverrideH1 > 0 ? OverrideH1 : baseDims.H1,
                OverrideH2 > 0 ? OverrideH2 : baseDims.H2
            );
        }
    }
}
