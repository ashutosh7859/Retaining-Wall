# Retaining Wall Subassembly

Phase 1 scaffold for a pure .NET Civil 3D 2026 retaining wall subassembly.

The solution separates Autodesk-free core logic from Civil 3D integration. `RetainingWall.Core` must stay free of Autodesk assembly references. Autodesk-dependent projects use local Civil 3D 2026 references only when the expected DLLs exist under `C:\Program Files\Autodesk\AutoCAD 2026`.

No PKT, SAC runtime, or EGL-driven wall case selection is introduced in this phase.
