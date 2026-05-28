# Civil 3D Deployment Testing - May 16, 2026

## Deployment Status

**Bundle Location:** `C:\ProgramData\Autodesk\ApplicationPlugins\RoadEdgeRetainingWall.bundle/`

**Contents:**
- DLLs in `Contents/Win64/`
  - `RetainingWall.Commands.dll`
  - `RetainingWall.Civil3D.dll`
  - `RetainingWall.Core.dll`
- Package manifest: `PackageContents.xml` (updated with correct paths, commands, runtime requirements for Civil 3D 2026 R25.1)
- Tool palette: `ToolPalette/RoadEdgeRetainingWall.xml` (basic structure with RW_ROAD_EDGE_RETAINING_WALL and RW_CREATE_SURFACES)

## Phase 7 Testing Checklist

### 1. Bundle Autoload Verification
- [ ] Close Civil 3D completely if running
- [ ] Launch Civil 3D 2026
- [ ] Open command line (press `C` or `Ctrl+9`)
- [ ] Type command: `NETLOAD` (or verify no errors appear on load)
- [ ] Expected: DLLs load without errors. Check notification area for bundle load messages.

### 2. Subassembly Registration Test
- [ ] Create a new blank drawing in Civil 3D
- [ ] Create an alignment (Insert > Alignment, or use sample data)
- [ ] Create a profile
- [ ] Create a corridor (Home > Create Subassembly)
- [ ] In the Subassembly Selector:
  - [ ] Look for "RoadEdgeRetainingWall" in the subassembly list
  - [ ] Select it and verify parameters appear: `Side`, `WallCase`, `UseDimensionOverrides`, overrides, and construction params
  - [ ] Confirm defaults: `Side=Left`, `WallCase=2`, `UseDimensionOverrides=No`

### 3. Corridor Geometry Test
- [ ] Create a corridor section using RoadEdgeRetainingWall subassembly
- [ ] Apply to a station range (e.g., 0 to 200m)
- [ ] Open the Corridor Surface Editor and verify geometry:
  - [ ] Points: `RW_*` codes present (e.g., RW_Top, RW_Datum, RW_Structure)
  - [ ] Links: mirrored left/right with appropriate codes
  - [ ] Shapes: closed shapes present with correct codes
  - [ ] No geometry errors or missing sections

### 4. RW_CREATE_SURFACES Command Test
- [ ] With the corridor open or selected:
  - [ ] Type command: `RW_CREATE_SURFACES`
  - [ ] Select the corridor when prompted
  - [ ] Verify three surfaces are created:
    - [ ] `RW_Top_Surface` (generated from RW_Top links)
    - [ ] `RW_Earthwork_Surface` (from RW_Datum, RW_Backfill, RW_Excavation)
    - [ ] `RW_Structure_Reference` (optional, from structural links)
  - [ ] Surfaces appear in the drawing with correct boundary and TIN

### 5. Tool Palette Discovery (Optional)
- [ ] Access tool palette (View > Toolbars > Tool Palettes or Ctrl+3)
- [ ] Locate "Retaining Wall Subassembly" palette
- [ ] Verify entries for RoadEdgeRetainingWall subassembly and RW_CREATE_SURFACES command

### 6. Parameter Override Test
- [ ] Create another corridor section with same subassembly
- [ ] Set `UseDimensionOverrides=Yes`
- [ ] Set override parameters (e.g., OverrideA, OverrideH1)
- [ ] Verify geometry changes match override values

## Known Issues or Risks

- None identified. Autodesk assembly versions validated in build.
- All dependency DLLs copied correctly to bundle.
- No Autodesk managed DLLs copied (CopyLocal=false enforced).

## Next Steps After Testing

1. If tests pass:
   - Update `IMPLEMENTATION_STATUS.md` to Phase 7 Complete
   - Update `VALIDATION_LOG.md` with test results
   - Mark `NEXT_STEPS.md` as all tasks complete

2. If tests fail:
   - Record failure pattern in `VALIDATION_LOG.md`
   - Check `RISK_REGISTER.md` for relevant risks
   - Use NETLOAD debugging to identify missing dependencies or loader issues

## Developer Notes

- DLLs are signed by default if Visual Studio signing is enabled; if blocked by Windows SmartScreen, unblock via Properties > Unblock
- PackageContents.xml uses relative paths (`./Contents/Win64/`) for portability
- Tool palette XML is basic; icons can be added later by placing PNG files in `ToolPalette/Icons/`
