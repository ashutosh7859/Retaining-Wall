using System.Collections.Generic;

namespace RetainingWall.Core
{
    /// <summary>
    /// Represents a generated closed shape for the retaining wall geometry.
    /// </summary>
    public class GeometryShape
    {
        public List<GeometryLink> Links { get; }
        public List<string> Codes { get; }

        public GeometryShape(List<GeometryLink> links, params string[] codes)
        {
            Links = new List<GeometryLink>(links);
            Codes = new List<string>(codes);
        }
    }
}
