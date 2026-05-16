using System.Collections.Generic;

namespace RetainingWall.Core
{
    /// <summary>
    /// Represents a generated link between two points for the retaining wall geometry.
    /// </summary>
    public class GeometryLink
    {
        public GeometryPoint StartPoint { get; }
        public GeometryPoint EndPoint { get; }
        public List<string> Codes { get; }

        public GeometryLink(GeometryPoint startPoint, GeometryPoint endPoint, params string[] codes)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
            Codes = new List<string>(codes);
        }
    }
}
