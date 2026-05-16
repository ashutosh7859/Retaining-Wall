namespace RetainingWall.Civil3D;

/// <summary>
/// Phase 1 marker for the Civil 3D integration assembly.
/// </summary>
public static class Civil3DAssembly
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
