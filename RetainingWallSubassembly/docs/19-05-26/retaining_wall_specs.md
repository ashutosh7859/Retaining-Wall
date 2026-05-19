# Retaining Wall Subassembly: Production Specifications

## 1. Input Parameters & UI Headings
Expose these exact parameters in the C# class. Group them strictly under the specified categories to prevent UI bloat in the Civil 3D Properties Palette.

| Category Heading | Parameter Name | Type | Behavior / Default Value |
| :--- | :--- | :--- | :--- |
| **Core Configuration Control** | `OperatingMode` | Enum | Dropdown: Dynamic Table, Preset Dropdown, Full Custom. |
| | `WallPreset` | Enum | Dropdown: Type 2 through Type 12. *(Ignored if not in Preset Mode)*. |
| **Dimensions - Stem Notch & Batter** | `w` | Double | 0.450 m. Top stem width ($P_1$ to $P_2$). |
| | `x1` | Double | 0.200 m. Vertical drop from $P_1$ to $P_{11}$. |
| | `x2` | Double | 0.150 m. Vertical drop of notch slope ($P_{11}$ to $P_{10}$). |
| | `a` | Double | 0.450 m. Recess thickness at $P_{10}$. |
| **Dimensions - Custom Footing & Base**<br><br>*(Used only when OperatingMode = Full Custom. Send warning to Event Viewer if edited in other modes)* | `Custom_b` | Double | 0.450 m. Stem thickness at footing plane ($P_4$ to $P_9$). |
| | `Custom_c` | Double | 1.450 m. Heel projection length. |
| | `Custom_d` | Double | 0.700 m. Toe projection length. |
| | `Custom_e` | Double | 0.250 m. Vertical height of outer heel face. |
| | `Custom_f` | Double | 0.350 m. Maximum footing depth at stem junction. |
| | `Custom_g` | Double | 0.250 m. Vertical height of outer toe face. |
| | `Custom_H2` | Double | 0.850 m. Foundation depth from GL to footing bottom. |
| **Foundations - PCC Mud Mat** | `PccThickness` | Double | 0.150 m. Vertical thickness of lean concrete base. |
| | `PccOffset` | Double | 0.150 m. Horizontal offset extension past $P_6$ and $P_7$. |
| **Backfill - Filter Media** | `FmThickness` | Double | 0.600 m. Constant horizontal thickness of drainage backfill zone. |

---

## 2. Fallback EGL & Target Surface Calculations
* **Primary Step:** Sample target surface elevation at $X = -w$. Distance from $P_1$ to this elevation defines $measuredH_1$.
* **Out-of-Bounds Fallback Rule:** If the surface target returns null at $X = -w$, locate the absolute nearest boundary/perimeter point of the DTM surface along the current cross-section station. Extract its Y coordinate. Calculate $measuredH_1$ as the vertical gap from $P_1$ to that edge elevation.
* **Table Tier Selection (Dynamic Mode):** Use the determined $measuredH_1$ to scan the sorted structural table rows. Select the first row where $measuredH_1 \le H1\_Max$. If it exceeds 12.0 m, fail safely by locking parameters to Type 12. Any intermediate height must round up to the higher tier variables to preserve structural safety.

---

## 3. Sequential Geometry Layout (SAC Method)
Establish all coordinates relative to preceding points. Origin $P_1$ is located at local `(0,0)`. All database values must be converted from mm to meters.

| Node | X-Coordinate Formula | Y-Coordinate Formula | Notes |
| :--- | :--- | :--- | :--- |
| **P₁** | `0.0` | `0.0` | Attachment / Baseline Reference |
| **P₂** | `P1.X - w` | `P1.Y` | |
| **P₃** | `P2.X` | `P2.Y - H1` | Ground Level Intercept Line |
| **P₄** | `P2.X` | `P3.Y - H2 + f` | |
| **P₅** | `P4.X - d` | `P3.Y - H2 + g` | |
| **P₆** | `P5.X` | `P3.Y - H2` | Footing Bottom Left corner |
| **P₇** | `P2.X + b + c` | `P3.Y - H2` | Footing Bottom Right corner |
| **P₈** | `P7.X` | `P7.Y + e` | |
| **P₉** | `P2.X + b` | `P7.Y + f` | |
| **P₁₀** | `P2.X + a` | `P1.Y - x1 - x2` | |
| **P₁₁** | `P1.X` | `P1.Y - x1` | |
| **P₁₂** | `P6.X - PccOffset` | `P6.Y` | |
| **P₁₃** | `P12.X` | `P12.Y - PccThickness` | |
| **P₁₄** | `P7.X + PccOffset` | `P13.Y` | |
| **P₁₅** | `P14.X` | `P7.Y` | |
| **P₁₆** | `P10.X + FmThickness` | `P10.Y` | |
| **P₁₈** | `P10.X + ((-H1) - P10.Y) * ((P9.X - P10.X) / (P9.Y - P10.Y))` | `-H1` | Back-face vector intersection with GL plane |
| **P₁₇** | `P18.X + FmThickness` | `-H1` | |

---

## 4. Shape & Material Coding
Generate closed shape loops using these exact string tags to allow seamless corridor volume lookups:
* **Shape 1 (Main Structure):** Bounded sequentially by loops `P₁ → P₂ → P₃ → P₄ → P₅ → P₆ → P₇ → P₈ → P₉ → P₁₀ → P₁₁ → P₁`. Code as: `"Structure_Concrete"`.
* **Shape 2 (PCC Bed):** Bounded sequentially by loops `P₆ → P₁₂ → P₁₃ → P₁₄ → P₁₅ → P₇ → P₆`. Code as: `"PCC_Lean_Concrete"`.
* **Shape 3 (Filter Media):** Bounded sequentially by loops `P₁₀ → P₁₆ → P₁₇ → P₁₈ → P₁₀`. Code as: `"Filter_Drainage_Media"`.
