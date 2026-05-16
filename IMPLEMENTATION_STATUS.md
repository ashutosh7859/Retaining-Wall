# Implementation Status

Source of truth: `RetainingWallSubassembly_Plan.md`.

## Current State

- Status: Phase 1 scaffold complete as of 2026-05-16.
- Created `RetainingWallSubassembly/` with solution, source projects, basic runnable test project, deployment bundle folder, docs folder, and scripts folder.
- Target: Civil 3D 2026, x64, .NET 8 Windows for Autodesk-facing projects; `RetainingWall.Core` targets plain `net8.0`.
- SDK pinning: `global.json` requests .NET SDK 8.0.100 with `latestMajor` roll-forward so the installed .NET 10 SDK can build the net8.0 projects.
- Autodesk references: centralized in `Directory.Build.props` and `Directory.Build.targets`; included only for projects with `UsesAutodeskCivil3D=true` and only when all local DLL paths exist.
- Autodesk local paths found on this machine: `AeccDbMgd.dll` under `C:\Program Files\Autodesk\AutoCAD 2026\C3D`; AutoCAD managed DLLs under `C:\Program Files\Autodesk\AutoCAD 2026`.
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
- Core wall case table: not started.
- Override/default parameter model: not started.
- Geometry generation and mirroring: not started.
- Civil 3D SATemplate integration: not started.
- `RW_CREATE_SURFACES` command: not started.
- Bundle deployment: scaffold only; not validated in Civil 3D.
- Documentation: placeholder files created from the planned structure.
- Automated tests: basic scaffold test runner added and passing; planned domain tests not started.
- Civil 3D manual validation: not started.
