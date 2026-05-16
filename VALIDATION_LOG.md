# Validation Log

Source of truth: `RetainingWallSubassembly_Plan.md`.

## 2026-05-16

- Phase 1 scaffold created under `RetainingWallSubassembly/`.
- Ran `powershell -NoProfile -ExecutionPolicy Bypass -File RetainingWallSubassembly/scripts/build.ps1`.
- Build result: succeeded with 6 MSB3277 warnings from Autodesk managed DLL assembly-version conflicts and 0 errors.
- Basic test result: `RetainingWall.Core.Tests passed.`
- Verified `RetainingWall.Core.Tests` checks that `RetainingWall.Core` does not reference `AeccDbMgd`, `acmgd`, `acdbmgd`, or `accoremgd`.
- Checked Autodesk output copying with `Get-ChildItem` filters for `*mgd*.dll` in `RetainingWall.Civil3D` and `RetainingWall.Commands` Release output folders; no Autodesk DLLs were copied.
- Civil 3D manual validation was not run.

## Required Later Checks

- Core tests: table mm-to-m conversion, overrides, invalid cases, mirroring, closed shapes.
- Build: Release x64 compiles and Autodesk DLLs are not copied into output.
- Civil 3D: bundle autoloads and subassembly appears through the selected tool entry.
- Corridor QA: Type 2 and Type 12 sections match expectations.
- Region QA: adjacent corridor regions with different wall cases rebuild cleanly.
- Surface QA: `RW_CREATE_SURFACES` creates expected retaining wall surfaces.
- Volume QA: compare `RW_Earthwork_Surface` or pasted composite surface against EGL.
