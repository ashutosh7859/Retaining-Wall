# ATC Files for Road Edge Retaining Wall Subassembly

## Overview
These ATC (Autodesk Tool Catalog) files define how the Road Edge Retaining Wall subassembly appears and behaves in Civil 3D's tool catalog and subassembly properties.

## Files Created

### 1. **RoadEdgeRetainingWall_Main.atc**
The main catalog file that:
- Defines the overall catalog with ID: `{6C3F6A92-3F95-46D2-92DE-C13E8B6D6E31}` (matches ProductCode in PackageContents.xml)
- References a single category of subassembly tools via the `<Categories>` element
- Points to `RoadEdgeRetainingWall_Tools.atc` for the actual tool definitions

**Key elements:**
- `<ItemID>`: Unique GUID for the catalog
- `<Category>`: References the tools file (relative path: `.\RoadEdgeRetainingWall_Tools.atc`)
- `<Images>`: Icon for the catalog display (64x64 pixels recommended)
- `<Description>`: Text shown in Civil 3D's catalog browser

### 2. **RoadEdgeRetainingWall_Tools.atc**
The category file that:
- Defines the `RoadEdgeRetainingWall` subassembly tool
- Specifies the .NET class: `RetainingWall.Civil3D.Subassembly.RoadEdgeRetainingWall`
- Lists all public parameters with their default values and descriptions
- References the `RetainingWall.Civil3D.dll` assembly

**Key elements:**
- `<Tool>`: Defines a single subassembly tool
- `<DotNetClass>`: Path to the C# class implementing the subassembly
- `<Params>`: All parameters visible in the subassembly Properties dialog

## Parameters Defined

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| Side | String | Left | Wall side: Left or Right |
| WallCase | Long | 2 | Wall case type (2-12) |
| UseDimensionOverrides | String | No | Enable dimension overrides |
| OverrideA-H2 | Double | 0.0 | Dimension override values (meters) |
| PccThickness | Double | 0.150 | Portland cement concrete thickness |
| PccProjection | Double | 0.150 | PCC projection distance |
| FilterThickness | Double | 0.600 | Filter material thickness |
| WallTopOffset | Double | 0.450 | Offset at wall top |

## Integration with Bundle

The ATC files are deployed as part of the `RoadEdgeRetainingWall.bundle` package:

```
RoadEdgeRetainingWall.bundle/
├── PackageContents.xml          (Application plugin registration)
├── RoadEdgeRetainingWall_Main.atc
├── RoadEdgeRetainingWall_Tools.atc
├── Contents/Win64/
│   ├── RetainingWall.Civil3D.dll
│   ├── RetainingWall.Commands.dll
│   └── ...
├── ToolPalette/
│   └── RoadEdgeRetainingWall.xtp
└── Images/
    └── RoadEdgeRetainingWall.png (recommended: 64x64)
```

## How Civil 3D Uses These Files

1. When Civil 3D loads the plugin, it scans the bundle for ATC files
2. The ATC files tell Civil 3D about available subassemblies
3. The `RoadEdgeRetainingWall` subassembly appears in the Subassembly Composer catalog
4. When a user creates the subassembly in a corridor, Civil 3D displays the parameters defined in the ATC file
5. The `DotNetClass` reference tells Civil 3D which C# class to instantiate when the subassembly is used

## XML Validation Notes

- **Case-sensitive**: All XML tags must match exactly (e.g., `<ItemID>` not `<itemid>`)
- **GUIDs**: The catalog GUID must match the ProductCode in `PackageContents.xml`
- **Relative paths**: All paths are relative to the ATC file location (e.g., `.\Contents\Win64\...`)
- **Assembly path**: Points to the compiled DLL in the bundle's Contents folder

## Next Steps

1. Ensure the icon file (`Images/RoadEdgeRetainingWall.png`) is created and placed in the bundle
2. Verify the DLL path in `<DotNetClass>` matches the actual deployment location
3. Test the bundle deployment by:
   - Restarting Civil 3D
   - Opening the Subassembly Composer
   - Confirming `RoadEdgeRetainingWall` appears in the catalog
   - Creating a test subassembly and verifying all parameters display correctly
