# Civil 3D 2026 .NET Retaining Wall Subassembly Plan

## Summary

- Build a pure .NET Civil 3D subassembly, with no PKT/SAC dependency.
- Use C#, deployed through an AutoCAD/Civil 3D autoload bundle.
- Subassembly starts at the road edge/base point and builds the wall-side geometry only.
- Wall "Type" means the drawing table case, Type 2-12, not wall family.
- Wall case is selected manually by corridor region/station range; EGL must not control wall height/case selection.
- Road surface and retaining wall surface remain separate; earthwork comparison can later use pasted/composite surfaces.

## Project Structure

Create a clean solution like:

```text
RetainingWallSubassembly/
  src/
    RetainingWall.Core/
    RetainingWall.Civil3D/
    RetainingWall.Commands/
  tests/
    RetainingWall.Core.Tests/
  deployment/
    RoadEdgeRetainingWall.bundle/
      PackageContents.xml
      Contents/Win64/
      ToolPalette/
  docs/
    README.md
    parameters.md
    wall-cases.md
    codes-and-surfaces.md
    installation.md
    validation-checklist.md
  scripts/
    build.ps1
    pack-bundle.ps1
    install-dev.ps1
```

- `RetainingWall.Core`: pure geometry/table logic, no Autodesk references.
- `RetainingWall.Civil3D`: SATemplate-style subassembly class and Civil 3D API writing.
- `RetainingWall.Commands`: helper commands for loading, diagnostics, and corridor surface setup.
- `deployment`: autoload bundle, palette metadata, icons, and packaged DLLs.
- `docs`: user-facing and developer-facing documentation.

## Key Implementation Changes

- Add class `Subassembly.RoadEdgeRetainingWall : SATemplate`.
- Follow Autodesk's installed `C3DStockSubassemblies` pattern, but implement in C#.
- Target Civil 3D 2026 / x64 / .NET 8 Windows.
- Reference Civil 3D assemblies with `CopyLocal=false`:
  - `AeccDbMgd.dll`
  - `acmgd.dll`
  - `acdbmgd.dll`
  - `accoremgd.dll`
- Use the default install root: `C:\Program Files\Autodesk\AutoCAD 2026`.
- Do not reference SAC runtime unless a later implementation proves it is required.

## Public Inputs

- `Side`: default `Left`; supports `Left` and `Right`.
- `WallCase`: integer enum `2` to `12`.
- `UseDimensionOverrides`: Yes/No, default No.
- Override parameters:
  - `OverrideA`
  - `OverrideB`
  - `OverrideC`
  - `OverrideD`
  - `OverrideE`
  - `OverrideF`
  - `OverrideG`
  - `OverrideH1`
  - `OverrideH2`
- Override rule: if `UseDimensionOverrides=Yes`, any override value `> 0` replaces the table value; values `<= 0` keep the table value.
- Construction parameters:
  - `PccThickness=0.150`
  - `PccProjection=0.150`
  - `FilterThickness=0.600`
  - `WallTopOffset=0.450`
- All dimensions stored internally in metres.

## Wall Case Table

Implement the drawing table exactly, converting mm to m:

```csv
Type,a,b,c,d,e,f,g,H1,H2
2,450,450,1450,700,250,350,250,2000,850
3,450,450,1900,1000,250,450,250,3000,1100
4,450,500,2400,1300,250,500,250,4000,1350
5,450,700,2750,1500,300,650,300,5000,1650
6,450,800,3200,1700,450,800,450,6000,1850
7,450,900,3800,1900,500,1000,500,7000,2050
8,450,1000,3950,2100,650,1200,650,8000,2150
9,450,1100,4450,2150,750,1300,750,9000,2250
10,450,1150,4600,2250,750,1350,750,10000,2300
11,450,1300,4850,2350,750,1400,750,11000,2400
12,450,1450,5250,2500,800,1500,800,12000,2500
```

## Geometry And Codes

- Attachment point is local `(0,0)` at road edge/top back of wall.
- Generate points, links, and shapes via `corridorState.Points`, `corridorState.Links`, and `corridorState.Shapes`.
- Do not create wall case from EGL. EGL is only for later volume comparison.
- Use separate codes for surfaces and quantities.

### Point Codes

- `RW_Attach`
- `RW_TopBack`
- `RW_TopFront`
- `RW_FootingToe`
- `RW_FootingHeel`
- `RW_PccLeft`
- `RW_PccRight`
- `RW_EarthworkLimit`

### Link Codes

- `RW_Top`
- `RW_Wall`
- `RW_Footing`
- `RW_PCC`
- `RW_Filter`
- `RW_Backfill`
- `RW_Excavation`
- `RW_Datum`

### Shape Codes

- `RW_Concrete`
- `RW_PCC`
- `RW_FilterMedia`
- `RW_Backfill`

Keep vertical or near-vertical wall faces out of earthwork TIN surfaces where they would create bad triangulation.

## Surface Helper Command

Add command `RW_CREATE_SURFACES`:

- User selects corridor.
- Command creates or updates retaining-wall corridor surfaces:
  - `RW_Top_Surface`: from `RW_Top`
  - `RW_Earthwork_Surface`: from `RW_Datum`, `RW_Backfill`, `RW_Excavation`
  - `RW_Structure_Reference`: optional non-volume surface from structural links
- Apply corridor extents boundary or feature-line-code boundary where API support is reliable.
- Road surfaces remain outside this command.

## Documentation Requirements

- `README.md`: purpose, version support, no PKT/SAC note, high-level workflow.
- `parameters.md`: all Civil 3D parameters, defaults, units, override behavior.
- `wall-cases.md`: Type 2-12 table and station-region workflow.
- `codes-and-surfaces.md`: point/link/shape codes, recommended corridor surfaces, volume workflow.
- `installation.md`: build, bundle install path, Civil 3D loading, troubleshooting.
- `validation-checklist.md`: manual Civil 3D QA steps and acceptance screenshots/sections to capture.
- Add XML comments for public C# types in `RetainingWall.Core`.

## Test Plan

- Unit tests in `RetainingWall.Core.Tests`:
  - all Type 2-12 table values convert mm to m correctly.
  - override rules work per dimension.
  - invalid `WallCase` fails clearly.
  - left/right mirroring produces equal and opposite offsets.
  - generated geometry has closed concrete/PCC/filter shapes.
- Build checks:
  - `scripts/build.ps1` compiles Release x64.
  - Autodesk DLLs are referenced but not copied into output.
- Civil 3D manual validation:
  - autoload bundle registers successfully.
  - subassembly appears/loads through the chosen tool entry.
  - corridor region Type 2 and Type 12 produce expected sections.
  - two adjacent corridor regions with different wall cases rebuild cleanly.
  - `RW_CREATE_SURFACES` creates the expected retaining wall surfaces.
  - volume workflow can compare `RW_Earthwork_Surface` or pasted composite surface against EGL.

## Assumptions And Defaults

- Use C# and autoload bundle deployment.
- Use case table plus optional overrides.
- Default wall side is `Left`; user can switch to `Right`.
- No EGL target in v1 for wall height/case selection.
- Retaining wall surface is separate from road surface.
- If the project workspace path is inaccessible during implementation, scaffold first under `C:\tmp\RetainingWallSubassembly` and move to the project share after access is resolved.
