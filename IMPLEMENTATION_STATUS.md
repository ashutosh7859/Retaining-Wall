# Implementation Status

Source of truth: `RetainingWallSubassembly_Plan.md`.

## Current State

- Status: Phase 7 (bundle finalization, deployment, and parametric refinements) fully complete as of 2026-05-18.
- Created `RetainingWallSubassembly/` with solution, source projects, basic runnable test project, deployment bundle folder, docs folder, and scripts folder.
- Target: Civil 3D 2026, x64, .NET 8 Windows for Autodesk-facing projects; `RetainingWall.Core` targets plain `net8.0`.
- SDK pinning: `global.json` requests .NET SDK 8.0.100 with `latestMajor` roll-forward so the installed .NET 10 SDK can build the net8.0 projects.
- Autodesk references: centralized in `Directory.Build.props` and `Directory.Build.targets`; included only for projects with `UsesAutodeskCivil3D=true` and only when all local DLL paths exist.
- Autodesk local paths found on this machine: `AeccDbMgd.dll` under `C:\Program Files\Autodesk\AutoCAD 2026\C3D`; `AecBaseMgd.dll` under `C:\Program Files\Autodesk\AutoCAD 2026\ACA`; AutoCAD managed DLLs under `C:\Program Files\Autodesk\AutoCAD 2026`.
- Autodesk references use `<Private>false</Private>` so they are not copied to project output.
- `RetainingWall.Core` has no Autodesk references and includes only a Phase 1 marker type with XML comments.
- `RetainingWall.Core.Tests` is a zero-package console test runner that can run without NuGet test packages and currently checks the Core marker plus the no-Autodesk-reference boundary.

## Created Structure

- `RetainingWallSubassembly/RetainingWallSubassembly.sln`
- `RetainingWallSubassembly/src/RetainingWall.Core`
- `RetainingWallSubassembly/src/RetainingWall.Civil3D`
- `RetainingWallSubassembly/src/RetainingWall.Commands`
- `RetainingWallSubassembly/tests/RetainingWall.Core.Tests`
- `RetainingWallSubassembly/deployment/RoadEdgeRetainingWall.bundle`
- `RetainingWallSubassembly/docs`
- `RetainingWallSubassembly/scripts`

## Completion Snapshot

- Phase 1 solution and folder scaffold: complete.
- Conditional Autodesk reference setup: complete.
- Core Autodesk independence guard: basic passing test in place.
- Core wall case table: complete. Models and tests added for mm-to-m conversion and invalid cases.
- Override/default parameter model: complete. `WallDimensionOverrides` created and tests added.
- Geometry generation and mirroring: complete. `WallGeometryGenerator` created with left/right support and verified shapes.
- Civil 3D SATemplate integration: complete (Phase 5). API uncertainties resolved via Autodesk sample inspection.
- `RW_CREATE_SURFACES` command: complete (Phase 6). Automated creation/update of Top, Earthwork, and Structure surfaces.
- **Bundle deployment & Parametric Refinements: complete (Phase 7).**
  - Release binaries built (no Autodesk DLLs copied).
  - DLLs copied to `Contents/Win64/`.
  - `PackageContents.xml` updated with correct paths, command entries, and Civil 3D 2026 runtime requirements (R25.1).
  - **Table/Manual Toggle**: Dynamic dropdown modes added (Use Table/Select Table by Height).
  - **Height lookup**: Automatic lookup matching standard cases from total height $H_1$.
  - **Footing Junction Correction**: Footing top now slopes correctly using junction thickness $F$ (previously ignored).
  - **Shear Key support**: Optional shear key $H_2$ modeled under the stem, with PCC wrapping it uniformly.
  - Bundle deployed to `C:\ProgramData\Autodesk\ApplicationPlugins\RoadEdgeRetainingWall.bundle/`.
  - Tool palette catalog registered in the registry.
  - Validation script created.
- Documentation: `docs/codes-and-surfaces.md` updated with Phase 6 details.
- Automated tests: core domain tests for table, overrides, and geometry are complete and passing; Civil 3D adapter verified via build.
- **Civil 3D manual validation: successfully completed.**
