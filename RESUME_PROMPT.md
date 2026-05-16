# Resume Prompt – Phase 6

**Last updated:** 2026-05-16 (Phase 5 complete)

## Situation

The Civil 3D adapter is fully wired to the real Autodesk API and compiles successfully. The subassembly can now generate corridor geometry (points, links, shapes) with correct codes and mirroring. Phase 5 verified that `SATemplate` inheritance works, `CorridorState` is accessible, and parameters are registered.

**Next objective:** Implement the `RW_CREATE_SURFACES` helper command (Phase 6).

## Context Refresh

Read these first (in order):
1. `RetainingWallSubassembly_Plan.md` — architecture, wall case table, codes, surfaces strategy
2. `IMPLEMENTATION_STATUS.md` — completion snapshot
3. `NEXT_STEPS.md` — prioritized work queue
4. `DECISIONS.md` — past trade-offs
5. `RISK_REGISTER.md` — closed risks and any new ones
6. `VALIDATION_LOG.md` — what was verified so far

## Phase 6 Scope: RW_CREATE_SURFACES Command

### Objectives
1. Create a new command class `RW_CREATE_SURFACES` in `RetainingWall.Commands`.
2. Command flow:
   - Prompt user to select a corridor from the current document.
   - Create or update three retaining-wall corridor surfaces:
     - `RW_Top_Surface`: generated from `RW_Top` links across all regions/stations
     - `RW_Earthwork_Surface`: generated from `RW_Datum`, `RW_Backfill`, and `RW_Excavation` links
     - `RW_Structure_Reference`: optional non-volume surface from structural links (PCC, Wall, Footing, Filter)
3. Apply corridor extents boundary or feature-line-code boundary where API support is reliable.
4. Do not create or modify road surfaces—keep them separate.

### Technical Guidance
- **Civil 3D API**: Use `Corridor` and `Baseline` APIs to query regions and link codes. Surfaces are created via `CivilDocument.CorridorSurfaces.Add()`.
- **Link code filtering**: Iterate corridor regions and extract links by code name (e.g., `link.CodeName == "RW_Top"`).
- **Boundary handling**: Experiment with `CivilDocument.FeatureLines` or `Corridor.Surfaces[].BoundaryOptions` to bind surfaces to corridor extents.
- **Error handling**: Gracefully handle missing corridors, missing link codes, or invalid region definitions.

### Constraints
- Keep road surfaces outside this command.
- Do not introduce PKT/SAC dependency or EGL-driven wall case selection.
- Do not redesign the architecture unless a blocker proves the current plan cannot work.
- Keep changes scoped to the current packet.

### Deliverables
- **Code**: `RetainingWall.Commands/` updated with command class, command registration, and integration with `CommandsAssembly.cs`.
- **Verification**: Build successfully; command loads in Civil 3D test.
- **Documentation**: Update `docs/codes-and-surfaces.md` with surface creation workflow and link-code-to-surface mapping.

## How to Proceed

1. **Read and understand Phase 6 in the plan** (`RetainingWallSubassembly_Plan.md`, "Surface Helper Command" section).
2. **Design the command flow**:
   - What input does the user provide? (corridor selection)
   - What outputs are created? (three corridor surfaces)
   - What intermediate steps? (link filtering, TIN generation, boundary binding)
3. **Stub the command class** in `RetainingWall.Commands/` with method signatures.
4. **Implement link-code filtering** first (lowest risk).
5. **Implement corridor surface creation** using real Civil 3D surface API calls.
6. **Test in Civil 3D** by selecting a corridor with existing retaining wall subassembly regions.
7. **Update documentation** with the surface creation workflow.

## Rules

- Use `RetainingWallSubassembly_Plan.md` as the source of truth.
- Continue from the first task in `NEXT_STEPS.md` only.
- Run the smallest useful verification after each meaningful change.
- Before stopping, update:
  - `IMPLEMENTATION_STATUS.md` — current progress
  - `NEXT_STEPS.md` — next prioritized tasks
  - `DECISIONS.md` — if new decisions changed
  - `RISK_REGISTER.md` — if risks changed
  - `VALIDATION_LOG.md` — checks run and results

---

**Good luck! Questions? Re-read the plan. Still stuck? Check DECISIONS.md and RISK_REGISTER.md for context.**
