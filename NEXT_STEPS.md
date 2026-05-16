# Next Steps

Source of truth: `RetainingWallSubassembly_Plan.md`.

1. Implement `RetainingWall.Core` with the Type 2-12 wall case table, exact mm-to-m conversion, parameter defaults, override rules, and side mirroring.
2. Replace the scaffold test runner assertions with core domain checks for table conversion, override behavior, invalid wall cases, mirroring, and closed concrete/PCC/filter shapes.
3. Re-run `RetainingWallSubassembly/scripts/build.ps1` and resolve or document any Autodesk reference warnings that affect real Civil 3D API code.
4. Only after the core model and tests are in place, begin the `RetainingWall.Civil3D` SATemplate-style subassembly shell.
5. Keep `RW_CREATE_SURFACES` and detailed Civil 3D command behavior deferred until the Civil 3D integration phase.

Do not introduce PKT/SAC dependency or EGL-driven wall case selection.
