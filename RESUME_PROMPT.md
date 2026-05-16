# Resume Prompt

Use this prompt to continue the project:

```text
Read RetainingWallSubassembly_Plan.md, IMPLEMENTATION_STATUS.md, NEXT_STEPS.md, DECISIONS.md, RISK_REGISTER.md, and VALIDATION_LOG.md.

Use RetainingWallSubassembly_Plan.md as the source of truth.
Continue from the first task in NEXT_STEPS.md only.
Do not redesign the architecture unless a blocker proves the current plan cannot work.
Do not introduce PKT/SAC dependency or EGL-driven wall case selection.
Keep changes scoped to the current packet.
Run the smallest useful verification after changes.
Before stopping, update IMPLEMENTATION_STATUS.md, NEXT_STEPS.md, DECISIONS.md if decisions changed, RISK_REGISTER.md if risks changed, and VALIDATION_LOG.md with any checks run.
```
