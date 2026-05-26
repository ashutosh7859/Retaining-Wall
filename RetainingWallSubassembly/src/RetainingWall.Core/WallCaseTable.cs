using System;

namespace RetainingWall.Core
{
    /// <summary>
    /// Legacy redirect class for WallCaseTable, now centralized in WallConfig.
    /// </summary>
    [Obsolete("Use WallConfig instead.")]
    public static class WallCaseTable
    {
        /// <summary>
        /// Gets standard dimensions. Redirects to WallConfig.GetDimensions.
        /// </summary>
        public static WallDimensions GetDimensions(int wallCase) => WallConfig.GetDimensions(wallCase);

        /// <summary>
        /// Gets standard dimensions by height. Redirects to WallConfig.GetDimensionsByHeight.
        /// </summary>
        public static WallDimensions GetDimensionsByHeight(double height) => WallConfig.GetDimensionsByHeight(height);
    }
}
