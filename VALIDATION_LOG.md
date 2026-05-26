# Validation Log

Source of truth: `RetainingWallSubassembly_Plan.md`.

## 2026-05-16

- Phase 1 scaffold created under `RetainingWallSubassembly/`.
- Ran `powershell -NoProfile -ExecutionPolicy Bypass -File RetainingWallSubassembly/scripts/build.ps1`.
- Build result: succeeded with 6 MSB3277 warnings from Autodesk managed DLL assembly-version conflicts and 0 errors.
- Basic test result: `RetainingWall.Core.Tests passed.`
- Verified `RetainingWall.Core.Tests` checks that `RetainingWall.Core` does not reference `AeccDbMgd`, `acmgd`, `acdbmgd`, or `accoremgd`.
- Checked Autodesk output copying with `Get-ChildItem` filters for `*mgd*.dll` in `RetainingWall.Civil3D` and `RetainingWall.Commands` Release output folders; no Autodesk DLLs were copied.
- **Phase 5 Adapter build success**: `RetainingWall.Civil3D` compiles against real Civil 3D 2026 DLLs.
- Verified `SATemplate.cs` correctly handles `CorridorState` and geometry collections (`Points`, `Links`, `Shapes`).
- Verified `RoadEdgeRetainingWall.cs` correctly maps parameters and wires insertion calls for points, links, and shapes.
- **Phase 6 Command build success**: `RetainingWall.Commands` compiles with `RW_CREATE_SURFACES` logic.
- Resolved missing assembly `AecBaseMgd.dll` found in `ACA` subfolder.
- Verified `CorridorSurface` API usage for link code addition and boundary creation.
- Civil 3D manual validation was not run.

### Phase 7 Bundle Deployment (2026-05-16)

- **Build Release output verified**: All DLLs compiled successfully in Release configuration.
  - RetainingWall.Commands.dll (7.5 KB)
  - RetainingWall.Civil3D.dll (10 KB)
  - RetainingWall.Core.dll (13 KB)
- **DLLs copied to bundle**: All three DLLs + debug symbols copied to `C:\ProgramData\Autodesk\ApplicationPlugins\RoadEdgeRetainingWall.bundle\Contents\Win64/`.
- **PackageContents.xml finalized**:
  - Fixed ModuleName paths from source folders to `./Contents/Win64/` for deployment portability.
  - Added ComponentEntry for `RetainingWall.Civil3D` with command group `RW_SUBASSY` and command `RW_ROAD_EDGE_RETAINING_WALL`.
  - Added ComponentEntry for `RetainingWall.Commands` with command group `RW_COMMANDS` and command `RW_CREATE_SURFACES`.
  - Set RuntimeRequirements to `SeriesMin="R25.1" SeriesMax="R25.1"` for Civil 3D 2026.
  - Added SupportPath and ToolPalettePath declarations.
- **Tool palette created**: Basic palette structure with entries for subassembly and commands.
- **Bundle deployed**: Copied entire bundle to `C:\ProgramData\Autodesk\ApplicationPlugins\RoadEdgeRetainingWall.bundle/`.
- **Bundle structure verified**: All required files present and accessible.
- **Validation script created**: `scripts/validate-deployment.ps1` for deployment verification.

## Required Later Checks (Phase 7 Manual Validation)

- Civil 3D autoload: bundle loads at startup or on NETLOAD command.
- Subassembly registration: `RoadEdgeRetainingWall` appears in Subassembly Selector.
- Subassembly creation: corridor section created with correct parameters (Side, WallCase, overrides).
- Corridor geometry: points, links, shapes with correct codes present in section view.
- Mirroring: left/right geometry mirrored correctly.
- RW_CREATE_SURFACES command: creates three corridor surfaces with correct link codes.
- Parameter overrides: geometry updates correctly when overrides are applied.
- No loader errors: no missing dependencies or assembly resolution issues.

## 2026-05-26

- **Issue identified**: The tool catalog definition in `RoadEdgeRetainingWall_Main.atc` had reversed values for the `Side` parameter enum (where `Right` was set to `0` and `Left` was set to `1`). Since Civil 3D uses standard `Left = 0` and `Right = 1`, and the subassembly code maps `sideInt == 1` to `Side.Right`, this caused selecting "Right" to render as a left retaining wall and vice versa.
- **Rectification**: Swapped the enum values in `RoadEdgeRetainingWall_Main.atc` so that `Left` matches `0` and `Right` matches `1`. Changed default value to `0` so `Left` remains the default.
- **Nested Mirroring Issue & Resolution**:
  - *Symptom*: When attaching the subassembly to another subassembly on the Left side, it rendered as a Right-side retaining wall.
  - *Cause*: Civil 3D automatically applies a horizontal flip (`x = -x`) to the geometry of any subassembly placed on the Left side of the assembly baseline. Because the C# adapter `RoadEdgeRetainingWall.cs` was also manually mirroring the geometry (multiplying X by `-1`), the double-mirroring canceled out, resulting in a Right-side wall.
  - *Solution*: Modified [RoadEdgeRetainingWall.cs](file:///w:/ASHU_tpi76_W/10_PROJECTS/C%23/04_Retaining%20Wall/RetainingWallSubassembly/src/RetainingWall.Civil3D/RoadEdgeRetainingWall.cs#L376) to always generate the geometry in the local "Right" coordinate system (`RetainingWall.Core.Side.Right`), allowing the Civil 3D engine to automatically mirror it to the Left side correctly.
- **Build & Verification**:
  - Rebuilt the C# solution using `scripts/build.ps1` (DotNet build succeeded).
  - Re-packaged the bundle using `scripts/pack-bundle.ps1`.
  - Copied/deployed the corrected `RoadEdgeRetainingWall_Main.atc` catalog file directly to the ProgramData active plugins directory.
  - Overwrote the locked dll `RetainingWall.Civil3D.dll` in the active plugins folder by renaming the locked file to `.old` and copying the fresh DLL.
  - Ran `scripts/validate-deployment.ps1` successfully, passing all structure, manifest, ATC, assembly state, and registry validation checks.


