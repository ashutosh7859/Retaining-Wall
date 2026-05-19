# Retaining Wall Subassembly: Reference Manual

A custom .NET subassembly for Autodesk Civil 3D 2026 that handles dynamic, preset, and custom retaining walls, including PCC mud mat bedding and gravel backfill drainage media.

---

## 1. Project Architecture

The solution separates Autodesk-free core geometry from the Autodesk platform bindings:
*   **`RetainingWall.Core`**: Contains all geometry calculations, structural dimension tables, and notch batter algorithms. Free of any Autodesk DLL references, allowing fast unit testing.
*   **`RetainingWall.Civil3D`**: Integrates with the Civil 3D corridor lifecycle. Implements subassembly parameter binding, unboxing conversions, and automatic side mirroring.
*   **`RetainingWall.Commands`**: Registers standard custom commands like `RW_CREATE_SURFACES`.

---

## 2. Parameter Configurations & Modes

The retaining wall subassembly operates in three modes:
1.  **Dynamic Table (Standard)**: Automatically samples the target ground surface at $X = -w$ to measure the local stem height $H_1$, dynamically matching the structural dimensions from the standard Type 2 to Type 12 engineering design tiers.
2.  **Preset Dropdown**: Allows the engineer to manually select a fixed wall preset (Type 2 to Type 12) from a drop-down menu in the Civil 3D Properties Palette.
3.  **Full Custom**: Allows manual entry of all dimensions (footing width, toe/heel projections, footing depths) directly from the properties palette, overriding presets.

---

## 3. Deployment & Installation

To build the custom subassembly and install it on your local Civil 3D instance:

1.  **Close Civil 3D** (to prevent assembly write-lock errors).
2.  **Run the Developer Installation Script**:
    ```powershell
    powershell -ExecutionPolicy Bypass -File scripts\install-dev.ps1
    ```
3.  **Validate the Installation** (Optional):
    ```powershell
    powershell -ExecutionPolicy Bypass -File scripts\validate-deployment.ps1
    ```
4.  **Reopen Civil 3D**. The subassembly will autoload and be available on the custom Tool Palette!

---

## 4. Corridor Surfaces & Earthworks (`RW_CREATE_SURFACES`)

The custom command `RW_CREATE_SURFACES` automates the creation of corridor surfaces for material volume tracking:
1.  **`RW_Top_Surface`**: Bounded by finished wall top links (`RW_Top`). Used to model finished wall level.
2.  **`RW_Earthwork_Surface`**: Bounded by bottom datum links (`RW_Datum`, PCC bottom, backfill excavation). Used for cut/fill earthwork calculations.
3.  **`RW_Structure_Reference`**: Bounded by concrete, PCC, and filter edges. Used for visual modeling and structure references.

To run: Type `RW_CREATE_SURFACES` in the Civil 3D command line, select your retaining wall corridor, and the three surfaces will be generated or updated instantly.
