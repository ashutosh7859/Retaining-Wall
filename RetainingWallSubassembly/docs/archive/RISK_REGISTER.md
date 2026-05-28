# Risk Register

Source of truth: `RetainingWallSubassembly_Plan.md`.

| Risk | Status | Mitigation |
| --- | --- | --- |
| Civil 3D 2026 SATemplate registration details differ from available samples. | Partially mitigated | Followed installed `C3DStockSubassemblies` pattern (VB samples); C# implementation build-verified. |
| Autodesk DLL references may vary by machine/install path. | Mitigated | Found `AeccDbMgd.dll` in `C3D` and `AecBaseMgd.dll` in `ACA`; updated `Directory.Build.props` to include these paths. |
| Autodesk managed DLLs can emit MSB3277 assembly-version warnings under the installed .NET SDK. | Mitigated | Warnings exist but do not block build or execution; suppression added where possible. |
| Corridor surface boundary API may be unreliable. | Mitigated | Automated via `AddCorridorExtentsBoundary` for now; verified build against correct `AddLinkCode` signature. |
| Vertical wall faces can create bad TIN triangulation for earthwork. | Open | Keep structural vertical/near-vertical links out of earthwork surfaces. |
| Manual station-region wall case workflow may be misused. | Open | Document that Type 2-12 is manually selected per corridor region/station range. |
| Network workspace may be inaccessible during implementation. | Mitigated for Phase 1 | Workspace required elevated access in the tool session; scaffold was created directly under the project share. |
| Missing Civil 3D subassembly API references (e.g. `ParametricInstancedComponent`, `CorridorPoint`, `CorridorLink`, `CorridorShape`). | Closed | Verified correct types via Autodesk sample code inspection; implementation uses `DatabaseServices.IPoint`, `Link`, etc. |
| Missing `ParametricAttribute` for Civil 3D properties. | Open | Adapter reads directly from `corridorState.ParamsDouble[...]` as fallback. Validation needed in Civil 3D Properties Palette. |
