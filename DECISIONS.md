# Decisions

Source of truth: `RetainingWallSubassembly_Plan.md`.

## Settled

- Build a pure .NET Civil 3D subassembly; do not depend on PKT, SAC, or EGL-driven case selection in v1.
- Use C# with an AutoCAD/Civil 3D autoload bundle.
- Use `RetainingWall.Core` for Autodesk-free geometry and table logic.
- Use `RetainingWall.Civil3D` only for SATemplate-style Civil 3D API integration.
- Use `RetainingWall.Commands` for helper commands, including `RW_CREATE_SURFACES`.
- Wall `Type` means drawing table case `2` through `12`, selected manually by corridor region/station range.
- Store all dimensions internally in metres; convert the provided mm table exactly.
- Default side is `Left`; `Right` mirrors offsets.
- Retaining wall and road surfaces remain separate.
- Keep vertical or near-vertical wall faces out of earthwork TIN surfaces where they would triangulate badly.
- Phase 1 uses a zero-package console test project for `RetainingWall.Core.Tests` so the basic test can run without NuGet test framework dependencies.

## Reference Paths And Assemblies

- Civil 3D install root: `C:\Program Files\Autodesk\AutoCAD 2026`.
- Autodesk references: `AeccDbMgd.dll`, `acmgd.dll`, `acdbmgd.dll`, `accoremgd.dll`, `AecBaseMgd.dll`, and optionally `AeccCommonMgd.dll`.
- `AeccDbMgd.dll` and `AeccCommonMgd.dll` are found under the local `C3D` subfolder.
- `AecBaseMgd.dll` is found under the local `ACA` subfolder; it is required for `Corridor` and `Entity` base type resolution.
- `CorridorSurface.LinkCodes()` is a method in Civil 3D 2026 .NET API, returning a `string[]`.
- `CorridorSurface.AddLinkCode(string code, bool addAsBreakLine)` is used to add link codes to corridor surfaces.
- Autodesk references must use `CopyLocal=false` / `<Private>false</Private>`.
- Autodesk references are centralized in `Directory.Build.targets` and activated by `UsesAutodeskCivil3D=true` when all local DLL paths exist.

## Open Until Proven

- Exact SATemplate registration mechanics for Civil 3D 2026.
- Reliable API method for corridor surface boundary creation (automated via `AddCorridorExtentsBoundary` for now).
- Whether any SAC runtime reference is unexpectedly required; current decision is no.
