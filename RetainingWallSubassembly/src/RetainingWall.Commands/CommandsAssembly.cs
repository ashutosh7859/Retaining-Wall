namespace RetainingWall.Commands;

/// <summary>
/// Phase 1 marker for future retaining wall commands.
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
