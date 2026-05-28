# Codex Implementation Script

Project: Civil 3D 2026 Retaining Wall Subassembly

Use this file as the operating script for implementing `RetainingWallSubassembly_Plan.md` without wasting model quota. The same pattern can be copied into other projects by changing the phase list, acceptance checks, and resume prompt.

## Core Idea

The work should live in the repository, not in the chat.

Every implementation session must:

1. Read the current project files.
2. Pick one small packet of work.
3. Implement only that packet.
4. Run the smallest useful verification.
5. Update the status files before stopping.

This makes the project resumable by Codex, another model, or a human.

## Files To Create First

Create these files before coding begins:

```text
IMPLEMENTATION_STATUS.md
NEXT_STEPS.md
DECISIONS.md
RESUME_PROMPT.md
```

Optional but useful:

```text
RISK_REGISTER.md
VALIDATION_LOG.md
```

### What Each File Does

`IMPLEMENTATION_STATUS.md`

- Records what is complete, what is partial, and what is unstarted.
- Should be updated after every meaningful implementation session.

`NEXT_STEPS.md`

- Lists the next 1 to 5 tasks only.
- Keeps the next session focused.

`DECISIONS.md`

- Records architecture decisions and reasons.
- Prevents future sessions from redesigning already-settled choices.

`RESUME_PROMPT.md`

- Contains the exact prompt to give any model when resuming.
- This is the contingency plan if Codex quota runs out.

`RISK_REGISTER.md`

- Tracks uncertain Civil 3D API details, Autodesk loading issues, and manual validation risks.

`VALIDATION_LOG.md`

- Records test runs, build results, Civil 3D manual checks, and screenshots/section checks to capture later.

## Session Script

Use this script at the start of every Codex session:

```text
Read RetainingWallSubassembly_Plan.md, IMPLEMENTATION_STATUS.md, NEXT_STEPS.md, and DECISIONS.md.

Continue the project from the next unchecked task only.
Do not redesign the architecture unless a blocker proves the current plan cannot work.
Keep edits scoped to the current packet.
Run the smallest useful verification after changes.
Update IMPLEMENTATION_STATUS.md, NEXT_STEPS.md, and DECISIONS.md if any decision changed.
Before stopping, update RESUME_PROMPT.md if the resume instructions are now stale.
```

## Token Discipline Rules

Follow these rules to avoid spending quota inefficiently:

1. Do not ask for the whole project at once.
2. Work in packets that can finish in one session.
3. Ask Codex to read only relevant files.
4. Avoid pasting large files into chat if they already exist in the workspace.
5. Prefer tests and build output over long discussion.
6. Keep Civil 3D-specific work separate from pure C# logic.
7. Update status files at the end of each packet.
8. If stuck, write the blocker into `RISK_REGISTER.md` and continue with an independent packet.

## Agent And Session Rules

Use one main Codex session as the project owner.

Use extra agents or extra sessions only for isolated work:

- documentation drafting
- reviewing tests
- checking a specific API uncertainty
- summarizing a single file or folder

Do not let two sessions edit the same file group at the same time.

Good split:

```text
Main session: RetainingWall.Core implementation
Side session: Draft docs from current code
Side session: Review unit tests for missing cases
```

Bad split:

```text
Session A: edits geometry classes
Session B: also edits geometry classes
Session C: rewrites project structure
```

## Implementation Phases

### Phase 0: Project Memory Setup

Goal:

- Create durable project memory files.

Tasks:

- Create `IMPLEMENTATION_STATUS.md`.
- Create `NEXT_STEPS.md`.
- Create `DECISIONS.md`.
- Create `RESUME_PROMPT.md`.
- Optional: create `RISK_REGISTER.md` and `VALIDATION_LOG.md`.

Acceptance check:

- A new model can read those files and understand what to do next without reading the whole chat.

Suggested Codex prompt:

```text
Create the project memory files for this retaining wall subassembly implementation.
Use RetainingWallSubassembly_Plan.md as the source of truth.
Keep the files concise and practical.
Do not start coding yet.
```

### Phase 1: Solution Scaffold

Goal:

- Create the repository structure without solving all logic yet.

Tasks:

- Create `RetainingWallSubassembly/`.
- Create `src/RetainingWall.Core/`.
- Create `src/RetainingWall.Civil3D/`.
- Create `src/RetainingWall.Commands/`.
- Create `tests/RetainingWall.Core.Tests/`.
- Create `deployment/RoadEdgeRetainingWall.bundle/`.
- Create `docs/`.
- Create `scripts/`.
- Add solution and project files.

Acceptance check:

- The solution opens.
- `RetainingWall.Core.Tests` can run, even if tests are initially minimal.
- Autodesk DLLs are not required for the core test project.

Suggested Codex prompt:

```text
Implement Phase 1 only: scaffold the .NET solution and folders from RetainingWallSubassembly_Plan.md.
Keep Autodesk-dependent projects buildable only if the local references exist.
Make RetainingWall.Core independent from Autodesk assemblies.
Add a basic test project that can run.
Update the status files before stopping.
```

### Phase 2: Core Wall Case Table

Goal:

- Implement the Type 2 to Type 12 table in pure C#.

Tasks:

- Add wall case model.
- Store dimensions internally in metres.
- Convert drawing table values from mm to m.
- Reject invalid wall cases clearly.
- Add XML comments for public core types.

Acceptance check:

- Unit tests prove every table value converts correctly.
- Invalid cases fail with a clear exception or result.

Suggested Codex prompt:

```text
Implement Phase 2 only: the wall case table in RetainingWall.Core.
Use the exact Type 2-12 table from RetainingWallSubassembly_Plan.md and convert mm to m internally.
Add focused unit tests for all values and invalid wall cases.
Run tests and update the status files.
```

### Phase 3: Override Rules

Goal:

- Implement dimension overrides without touching Civil 3D yet.

Tasks:

- Add `UseDimensionOverrides`.
- Add override values for A, B, C, D, E, F, G, H1, H2.
- If overrides are enabled, any value greater than zero replaces the table value.
- Values less than or equal to zero keep the table value.

Acceptance check:

- Tests cover enabled overrides, disabled overrides, zero values, negative values, and partial overrides.

Suggested Codex prompt:

```text
Implement Phase 3 only: override rules in RetainingWall.Core.
Do not touch Civil 3D projects.
Add tests for enabled, disabled, zero, negative, and partial override cases.
Run tests and update the status files.
```

### Phase 4: Core Geometry

Goal:

- Generate wall-side geometry from pure inputs.

Tasks:

- Define point, link, and shape models in `RetainingWall.Core`.
- Attachment point is local `(0,0)`.
- Support `Left` and `Right`.
- Mirror offsets correctly.
- Generate concrete, PCC, filter, backfill, and datum-related geometry as far as the plan defines.
- Keep Civil 3D API writing out of this phase.

Acceptance check:

- Left/right tests produce equal and opposite offsets.
- Shape tests prove concrete, PCC, and filter shapes are closed.
- Geometry tests do not require Autodesk DLLs.

Suggested Codex prompt:

```text
Implement Phase 4 only: pure core geometry generation for the retaining wall.
Keep all Autodesk API calls out of RetainingWall.Core.
Add tests for left/right mirroring and closed shapes.
Run tests and update the status files.
```

### Phase 5: Civil 3D Subassembly Adapter

Goal:

- Convert core geometry into Civil 3D points, links, and shapes.

Tasks:

- Add `Subassembly.RoadEdgeRetainingWall : SATemplate`.
- Map Civil 3D input parameters into core inputs.
- Write points through `corridorState.Points`.
- Write links through `corridorState.Links`.
- Write shapes through `corridorState.Shapes`.
- Apply point, link, and shape codes from the plan.

Acceptance check:

- Project compiles on a machine with Civil 3D 2026 assemblies.
- Autodesk references use `CopyLocal=false`.
- No SAC runtime dependency is introduced unless documented as necessary.

Suggested Codex prompt:

```text
Implement Phase 5 only: the Civil 3D adapter skeleton and geometry writing layer.
RetainingWall.Core must remain Autodesk-free.
Reference Civil 3D 2026 assemblies with CopyLocal=false.
If exact SATemplate API details are uncertain, create a narrow adapter and record uncertainties in RISK_REGISTER.md.
Update the status files before stopping.
```

### Phase 6: Surface Helper Command

Goal:

- Add `RW_CREATE_SURFACES`.

Tasks:

- Add command project logic.
- Let user select a corridor.
- Create or update:
  - `RW_Top_Surface`
  - `RW_Earthwork_Surface`
  - `RW_Structure_Reference`
- Use planned link codes.
- Keep road surfaces outside this command.

Acceptance check:

- Command compiles.
- Manual Civil 3D validation steps are documented.
- Any uncertain API behavior is recorded in `RISK_REGISTER.md`.

Suggested Codex prompt:

```text
Implement Phase 6 only: RW_CREATE_SURFACES command.
Keep the command scoped to retaining-wall corridor surfaces only.
Record any uncertain Civil 3D API behavior in RISK_REGISTER.md.
Update docs and status files before stopping.
```

### Phase 7: Bundle And Scripts

Goal:

- Make the subassembly deployable.

Tasks:

- Add `PackageContents.xml`.
- Add `Contents/Win64/` layout.
- Add tool palette metadata or placeholder files if exact Civil 3D format must be verified.
- Add `scripts/build.ps1`.
- Add `scripts/pack-bundle.ps1`.
- Add `scripts/install-dev.ps1`.

Acceptance check:

- Release x64 build script exists.
- Bundle package script creates the expected folder structure.
- Install script copies to the expected Autodesk application plugins location or a documented dev location.

Suggested Codex prompt:

```text
Implement Phase 7 only: deployment bundle structure and scripts.
Do not redesign the code.
Make scripts conservative and document any path assumptions.
Run whatever script checks are safe locally and update the status files.
```

### Phase 8: Documentation

Goal:

- Create user and developer documentation.

Tasks:

- `docs/README.md`
- `docs/parameters.md`
- `docs/wall-cases.md`
- `docs/codes-and-surfaces.md`
- `docs/installation.md`
- `docs/validation-checklist.md`

Acceptance check:

- Docs match implemented behavior.
- Docs do not claim Civil 3D behavior that has not been validated.
- Validation checklist clearly separates automated tests from manual Civil 3D QA.

Suggested Codex prompt:

```text
Implement Phase 8 only: documentation.
Base the docs on the current code and RetainingWallSubassembly_Plan.md.
Do not invent validated Civil 3D behavior if it has not been tested.
Update the status files before stopping.
```

### Phase 9: Manual Civil 3D Validation

Goal:

- Prove the subassembly works inside Civil 3D.

Tasks:

- Install bundle.
- Confirm autoload registration.
- Confirm subassembly appears or loads through chosen tool entry.
- Build a corridor region with Type 2.
- Build a corridor region with Type 12.
- Test adjacent regions with different wall cases.
- Run `RW_CREATE_SURFACES`.
- Confirm retaining wall surfaces remain separate from road surfaces.
- Record screenshots and section checks.

Acceptance check:

- `VALIDATION_LOG.md` records exact Civil 3D version, date, drawing used, wall cases tested, results, and remaining issues.

Suggested Codex prompt:

```text
Help me perform Phase 9 manual Civil 3D validation.
Ask for observations and errors one step at a time.
Update VALIDATION_LOG.md and RISK_REGISTER.md based on the results.
Do not change code unless a specific validation failure requires it.
```

## Stop Rule

Before ending any implementation session, Codex must update:

```text
IMPLEMENTATION_STATUS.md
NEXT_STEPS.md
```

Update these when relevant:

```text
DECISIONS.md
RISK_REGISTER.md
VALIDATION_LOG.md
RESUME_PROMPT.md
```

The final response from Codex should say:

```text
Completed:
- ...

Verified:
- ...

Next:
- ...

Known risks:
- ...
```

## Resume Prompt Template

Put this in `RESUME_PROMPT.md` and keep it current:

```text
You are continuing implementation of the Civil 3D 2026 Retaining Wall Subassembly.

Read these files first:
- RetainingWallSubassembly_Plan.md
- Codex_Implementation_Script.md
- IMPLEMENTATION_STATUS.md
- NEXT_STEPS.md
- DECISIONS.md
- RISK_REGISTER.md if it exists
- VALIDATION_LOG.md if it exists

Source of truth:
- RetainingWallSubassembly_Plan.md defines the intended product.
- IMPLEMENTATION_STATUS.md defines what has already been done.
- NEXT_STEPS.md defines the next small task.
- DECISIONS.md defines choices that should not be casually reversed.

Rules:
- Work on only the next unchecked task unless I explicitly redirect you.
- Keep RetainingWall.Core free of Autodesk references.
- Prefer tests over long reasoning.
- Run the smallest useful verification after changes.
- Do not let Civil 3D API uncertainty block pure core work.
- Record blockers in RISK_REGISTER.md.
- Update IMPLEMENTATION_STATUS.md and NEXT_STEPS.md before stopping.
```

## How To Make Your Own Script For Future Projects

For any future project, copy this structure and replace the phase list.

Every good implementation script should answer:

1. What is the source of truth?
2. What files preserve memory between sessions?
3. What is the smallest first useful implementation?
4. Which parts can be tested without external software?
5. Which parts require manual validation?
6. What should the model read at the start of a session?
7. What must the model update before stopping?
8. What work can a side agent safely do?
9. What work must stay in the main session?
10. What does "done" mean for each phase?

Generic phase pattern:

```text
Phase N: Name

Goal:
- ...

Tasks:
- ...

Acceptance check:
- ...

Suggested prompt:
- ...
```

Generic session prompt:

```text
Read the plan, implementation script, status, next steps, and decisions.
Work on the next unchecked task only.
Keep edits scoped.
Run the smallest useful verification.
Update status and next steps before stopping.
```

That is the reusable method: small packets, durable memory, narrow verification, and a written handoff.
