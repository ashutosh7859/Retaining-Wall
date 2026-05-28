# Next Steps

Source of truth: `RetainingWallSubassembly_Plan.md`.

## Phase 7 – Manual Civil 3D Validation

1. Close Civil 3D completely if running.
2. Launch Civil 3D 2026 (autoloader should discover the bundle).
3. Test NETLOAD:
   - Command line: `NETLOAD`
   - Path: `C:\ProgramData\Autodesk\ApplicationPlugins\RoadEdgeRetainingWall.bundle\Contents\Win64\RetainingWall.Commands.dll`
   - Expected: No errors; command loads successfully.

4. Test subassembly creation:
   - Create a corridor using `RoadEdgeRetainingWall` subassembly.
   - Verify parameters appear: `Side`, `WallCase`, `UseDimensionOverrides`, overrides, construction params.
   - Create a section and verify geometry (points, links, shapes with correct codes).

5. Test `RW_CREATE_SURFACES` command:
   - Type command: `RW_CREATE_SURFACES`
   - Select the corridor.
   - Verify three surfaces created: `RW_Top_Surface`, `RW_Earthwork_Surface`, `RW_Structure_Reference`.

6. Record results and close out:
   - If all tests pass: Update `IMPLEMENTATION_STATUS.md` to Phase 7 Complete and mark as ready for production rollout.
   - If tests fail: Refer to `PHASE_7_TESTING.md` troubleshooting guide; check `VALIDATION_LOG.md` and `RISK_REGISTER.md` for context.

Do not introduce PKT/SAC dependency or EGL-driven wall case selection.
Refer to `PHASE_7_TESTING.md` for detailed test checklist and troubleshooting.
