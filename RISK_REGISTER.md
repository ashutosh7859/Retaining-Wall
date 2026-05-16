# Risk Register

Source of truth: `RetainingWallSubassembly_Plan.md`.

| Risk | Status | Mitigation |
| --- | --- | --- |
| Civil 3D 2026 SATemplate registration details differ from available samples. | Open | Follow installed `C3DStockSubassemblies` pattern and validate in Civil 3D early. |
| Autodesk DLL references may vary by machine/install path. | Partially mitigated | Default to `C:\Program Files\Autodesk\AutoCAD 2026`; accept `AeccDbMgd.dll` under `C3D`; allow MSBuild property overrides; keep `<Private>false</Private>`. |
| Autodesk managed DLLs can emit MSB3277 assembly-version warnings under the installed .NET SDK. | Open | Build currently succeeds; revisit warning handling when real Civil 3D API code is introduced. |
| Corridor surface boundary API may be unreliable. | Open | Implement link-code surfaces first; apply corridor extents or feature-line-code boundary where reliable. |
| Vertical wall faces can create bad TIN triangulation for earthwork. | Open | Keep structural vertical/near-vertical links out of earthwork surfaces. |
| Manual station-region wall case workflow may be misused. | Open | Document that Type 2-12 is manually selected per corridor region/station range. |
| Network workspace may be inaccessible during implementation. | Mitigated for Phase 1 | Workspace required elevated access in the tool session; scaffold was created directly under the project share. |
