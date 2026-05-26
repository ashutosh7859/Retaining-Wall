using System;

namespace RetainingWall.Core
{
    /// <summary>
    /// Dictates how the retaining wall dimensions are determined.
    /// </summary>
    public enum OperatingMode
    {
        DynamicTable = 0,
        PresetDropdown = 1,
        FullCustom = 2
    }

    /// <summary>
    /// Represents standard wall structural case table presets (Type 2 to Type 12).
    /// </summary>
    public enum WallPreset
    {
        Type2 = 2,
        Type3 = 3,
        Type4 = 4,
        Type5 = 5,
        Type6 = 6,
        Type7 = 7,
        Type8 = 8,
        Type9 = 9,
        Type10 = 10,
        Type11 = 11,
        Type12 = 12
    }

    /// <summary>
    /// Centralized configuration, default settings, and case tables for the Retaining Wall subassembly.
    /// All physical dimensions are in metres.
    /// </summary>
    public static class WallConfig
    {
        // ==========================================
        // 1. Default Parameter Values
        // ==========================================
        
        // General Controls
        public const OperatingMode DefaultOperatingMode = OperatingMode.DynamicTable;
        public const WallPreset DefaultWallPreset = WallPreset.Type2;
        public const Side DefaultSide = Side.Left; // Default side

        // Dimensions - Stem Notch & Batter Defaults
        public const double DefaultW = 0.450;      // Stem Top Width (w)
        public const double DefaultX1 = 0.250;     // Notch Drop (x1)
        public const double DefaultX2 = 0.250;     // Notch Slope Drop (x2)
        public const double DefaultA = 0.450;      // Notch Recess (a)

        // Dimensions - Custom Footing & Base Defaults (Used only in Full Custom)
        public const double DefaultCustomH1 = 3.000; // Stem Height (H1)
        public const double DefaultCustomB = 0.450;  // Stem Base Width (b)
        public const double DefaultCustomC = 1.450;  // Heel Projection (c)
        public const double DefaultCustomD = 0.700;  // Toe Projection (d)
        public const double DefaultCustomE = 0.250;  // Heel Edge Thickness (e)
        public const double DefaultCustomF = 0.350;  // Max Footing Thickness (f)
        public const double DefaultCustomG = 0.250;  // Toe Edge Thickness (g)
        public const double DefaultCustomH2 = 0.850; // Base Depth (H2)

        // Foundations - PCC Mud Mat Defaults
        public const double DefaultPccThickness = 0.150; // Mud Mat Thickness (PCC)
        public const double DefaultPccOffset = 0.150;    // Mud Mat Offset (PCC)

        // Backfill - Filter Media Defaults
        public const double DefaultFmThickness = 0.600;  // Filter Media Thickness

        // ==========================================
        // 2. Wall Case Dimension Table
        // ==========================================
        private static readonly int[] Cases = { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

        /// <summary>
        /// Gets the standard dimensions for the specified wall case.
        /// Dimensions are converted from mm to metres internally.
        /// </summary>
        /// <param name="wallCase">The integer wall case (2 through 12).</param>
        /// <returns>A <see cref="WallDimensions"/> instance containing the dimensions in metres.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the wall case is not between 2 and 12.</exception>
        public static WallDimensions GetDimensions(int wallCase)
        {
            return wallCase switch
            {
                2  => new WallDimensions(0.450, 0.450, 1.450, 0.700, 0.250, 0.350, 0.250, 2.000, 0.850),
                3  => new WallDimensions(0.450, 0.450, 1.900, 1.000, 0.250, 0.450, 0.250, 3.000, 1.100),
                4  => new WallDimensions(0.450, 0.500, 2.400, 1.300, 0.250, 0.500, 0.250, 4.000, 1.350),
                5  => new WallDimensions(0.450, 0.700, 2.750, 1.500, 0.300, 0.650, 0.300, 5.000, 1.650),
                6  => new WallDimensions(0.450, 0.800, 3.200, 1.700, 0.450, 0.800, 0.450, 6.000, 1.850),
                7  => new WallDimensions(0.450, 0.900, 3.800, 1.900, 0.500, 1.000, 0.500, 7.000, 2.050),
                8  => new WallDimensions(0.450, 1.000, 3.950, 2.100, 0.650, 1.200, 0.650, 8.000, 2.150),
                9  => new WallDimensions(0.450, 1.100, 4.450, 2.150, 0.750, 1.300, 0.750, 9.000, 2.250),
                10 => new WallDimensions(0.450, 1.150, 4.600, 2.250, 0.750, 1.350, 0.750, 10.000, 2.300),
                11 => new WallDimensions(0.450, 1.300, 4.850, 2.350, 0.750, 1.400, 0.750, 11.000, 2.400),
                12 => new WallDimensions(0.450, 1.450, 5.250, 2.500, 0.800, 1.500, 0.800, 12.000, 2.500),
                _  => throw new ArgumentOutOfRangeException(nameof(wallCase), wallCase, "Wall case must be between 2 and 12.")
            };
        }

        /// <summary>
        /// Gets the standard dimensions for a case that best matches the specified total height H1.
        /// Finds the first case where the standard H1 is greater than or equal to the specified height.
        /// </summary>
        /// <param name="height">The requested wall height in metres.</param>
        /// <returns>A <see cref="WallDimensions"/> instance matching the appropriate wall case.</returns>
        public static WallDimensions GetDimensionsByHeight(double height)
        {
            if (height <= 2.0)
            {
                return GetDimensions(2);
            }

            foreach (var c in Cases)
            {
                var dims = GetDimensions(c);
                if (dims.H1 >= height)
                {
                    return dims;
                }
            }

            return GetDimensions(12); // Fallback to max case if height exceeds Case 12
        }
    }
}
