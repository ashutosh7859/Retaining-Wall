# ATC Files for Road Edge Retaining Wall Subassembly

## Overview
These ATC (Autodesk Tool Catalog) files define how the Road Edge Retaining Wall subassembly appears and behaves in Civil 3D's tool catalog and subassembly properties.

## Main Catalog

### RoadEdgeRetainingWall_Main.atc
The main catalog file:
- Defines the catalog with ID `{6C3F6A92-3F95-46D2-92DE-C13E8B6D6E31}`.
- Registers the `RoadEdgeRetainingWall` subassembly tool directly at the catalog root level.
- Points Civil 3D to the compatibility class `Subassembly.RoadEdgeRetainingWall`.
- References the installed ProgramData `RetainingWall.Civil3D.dll` explicitly so Civil 3D can resolve the class even if it imports or caches the ATC entry elsewhere.
- Lists all public subassembly parameters with defaults and descriptions.
- Declares metric units with `<Units>m</Units>`.

## Parameters Defined

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| Side | String | Left | Wall side: Left or Right |
| WallCase | Long | 2 | Wall case type (2-12) |
| UseDimensionOverrides | Bool | 0 | Enable dimension overrides |
| OverrideA-H2 | Double | 0.0 | Dimension override values in meters |
| PccThickness | Double | 0.150 | Portland cement concrete thickness |
| PccProjection | Double | 0.150 | PCC projection distance |
| FilterThickness | Double | 0.600 | Filter material thickness |
| WallTopOffset | Double | 0.450 | Offset at wall top |

## Bundle Layout

```text
RoadEdgeRetainingWall.bundle/
|-- PackageContents.xml
|-- RoadEdgeRetainingWall_Main.atc
|-- Contents/Win64/
|   |-- RetainingWall.Civil3D.dll
|   |-- RetainingWall.Commands.dll
|   |-- RetainingWall.Core.dll
|   `-- ...
|-- ToolPalette/
|   `-- RoadEdgeRetainingWall.xtp
`-- Images/
    |-- RoadEdgeRetainingWall.png
    `-- Surface.png
```

## Civil 3D Loading Notes
- `PackageContents.xml` autoloads the .NET assemblies and registers only the real helper command, `RW_CREATE_SURFACES`.
- `RoadEdgeRetainingWall_Main.atc` is the subassembly tool entry point.
- The HKCU Content Browser registry key should point to the installed ProgramData ATC file.
- Run `scripts\validate-deployment.ps1` after packaging or installation to catch path, registry, or command drift.
