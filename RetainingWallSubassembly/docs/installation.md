# Retaining Wall Subassembly: Installation & Build Guide

This subassembly is distributed as an AutoCAD Autoload Bundle. 

## Requirements
* Autodesk Civil 3D 2026 (x64)
* .NET 8 SDK (for building the project)

## Installation Workflow

To deploy the subassembly to your local machine for use:

1. **Close Civil 3D**: Ensure Civil 3D is fully closed. Otherwise, file locking will prevent the DLLs from being overwritten or updated.
2. **Run the Install Script**: Navigate to the `scripts` directory and execute the developer installation script. This script builds the solution and copies the `.bundle` folder into the AutoCAD ApplicationPlugins directory.
   ```powershell
   cd RetainingWallSubassembly
   powershell -ExecutionPolicy Bypass -File scripts\install-dev.ps1
   ```
3. **Open Civil 3D**: Launch Civil 3D. The autoload mechanism will automatically detect the `RoadEdgeRetainingWall.bundle` and load `RetainingWall.Civil3D.dll` into the application domain.
4. **Access the Tool**: Open the Civil 3D Tool Palettes (`Ctrl+3`). You will find the custom subassembly icon on the Tool Palette.

## Troubleshooting
* **Tool not appearing**: Run the `scripts\validate-deployment.ps1` script to verify that the bundle was correctly placed in `%APPDATA%\Autodesk\ApplicationPlugins` or `%PROGRAMDATA%\Autodesk\ApplicationPlugins`.
* **Subassembly not rendering in corridor**: Check the Civil 3D Event Viewer. The custom core logic logs errors (such as out-of-bounds wall heights) directly to the Event Viewer.
* **Assembly missing error during build**: Ensure Civil 3D 2026 is installed in the default location (`C:\Program Files\Autodesk\AutoCAD 2026`). If it's on another drive, you must update the path in `Directory.Build.targets`.
