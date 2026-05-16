using System.Collections.Generic;

namespace RetainingWall.Core
{
    /// <summary>
    /// Represents the complete generated geometry for the retaining wall.
    /// </summary>
    public class GeometryResult
    {
        public List<GeometryPoint> Points { get; } = new List<GeometryPoint>();
        public List<GeometryLink> Links { get; } = new List<GeometryLink>();
        public List<GeometryShape> Shapes { get; } = new List<GeometryShape>();
    }
}
