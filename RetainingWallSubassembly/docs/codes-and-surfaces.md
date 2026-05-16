# Codes And Surfaces

This document describes the point, link, and shape codes used by the Retaining Wall subassembly and how they are used to generate corridor surfaces.

## Point Codes

| Code | Description |
| --- | --- |
| `RW_Attach` | Attachment point at road edge/top back of wall. |
| `RW_TopBack` | Top back corner of the wall. |
| `RW_TopFront` | Top front corner of the wall. |
| `RW_FootingToe` | Front bottom corner of the footing. |
| `RW_FootingHeel` | Back bottom corner of the footing. |
| `RW_PccLeft` | Left bottom corner of the PCC. |
| `RW_PccRight` | Right bottom corner of the PCC. |
| `RW_EarthworkLimit` | Lateral limit for backfill/excavation. |

## Link Codes

| Code | Surface Mapping | Description |
| --- | --- | --- |
| `RW_Top` | `RW_Top_Surface` | Top surface of the wall. |
| `RW_Wall` | `RW_Structure_Reference` | Vertical faces of the wall. |
| `RW_Footing` | `RW_Structure_Reference` | Footing links. |
| `RW_PCC` | `RW_Structure_Reference` | PCC links. |
| `RW_Filter` | `RW_Structure_Reference` | Filter media links. |
| `RW_Backfill` | `RW_Earthwork_Surface` | Backfill material links. |
| `RW_Excavation` | `RW_Earthwork_Surface` | Excavation/cut links. |
| `RW_Datum` | `RW_Earthwork_Surface` | Bottom-most links for volume calculation. |

## Shape Codes

| Code | Description |
| --- | --- |
| `RW_Concrete` | Concrete wall and footing area. |
| `RW_PCC` | PCC leveling layer area. |
| `RW_FilterMedia` | Back-of-wall drainage area. |
| `RW_Backfill` | Engineered backfill area. |

## Corridor Surfaces

The `RW_CREATE_SURFACES` command automates the creation of the following corridor surfaces:

1. **RW_Top_Surface**
   - Links: `RW_Top`
   - Overhang Correction: Top Links
   - Boundary: Corridor Extents
   - Purpose: Final finished level of the wall.

2. **RW_Earthwork_Surface**
   - Links: `RW_Datum`, `RW_Backfill`, `RW_Excavation`
   - Overhang Correction: Bottom Links
   - Boundary: Corridor Extents
   - Purpose: Volume comparison against EGL.

3. **RW_Structure_Reference**
   - Links: `RW_PCC`, `RW_Wall`, `RW_Footing`, `RW_Filter`
   - Overhang Correction: None
   - Boundary: Corridor Extents
   - Purpose: Visual reference or non-volume structural modeling.

## Workflow

1. Select the corridor containing Retaining Wall subassembly regions.
2. Run the command `RW_CREATE_SURFACES`.
3. The command will create or update the three surfaces listed above.
4. If the corridor layout changes, re-run the command to ensure all links are included.
