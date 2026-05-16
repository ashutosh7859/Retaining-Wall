using System.Collections.Generic;

namespace RetainingWall.Core
{
    /// <summary>
    /// Represents a generated 2D point for the retaining wall geometry.
    /// </summary>
    public class GeometryPoint
    {
        public double X { get; }
        public double Y { get; }
        public List<string> Codes { get; }

        public GeometryPoint(double x, double y, params string[] codes)
        {
            X = x;
            Y = y;
            Codes = new List<string>(codes);
        }
    }
}
