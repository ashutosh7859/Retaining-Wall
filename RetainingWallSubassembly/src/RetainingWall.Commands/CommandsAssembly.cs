using Autodesk.AutoCAD.Runtime;

[assembly: CommandClass(typeof(RetainingWall.Commands.CreateSurfacesCommand))]

namespace RetainingWall.Commands;

/// <summary>
/// Entry point and registration for Retaining Wall commands.
/// </summary>
public static class CommandsAssembly
{
    /// <summary>
    /// Gets whether the expected local Autodesk assemblies were available at build time.
    /// </summary>
    public static bool AutodeskReferencesAvailable =>
#if AUTODESK_REFERENCES_AVAILABLE
        true;
#else
        false;
#endif
}
