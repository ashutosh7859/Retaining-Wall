# Deployment Status vs. Tool Catalog Guide

This document compares our current `RoadEdgeRetainingWall.bundle` deployment folder against the recommendations in [civil_3_d_custom_subassembly_tool_catalog_guide.md](file:///w:/ASHU_tpi76_W/10_PROJECTS/Retaining%20Wall/civil_3_d_custom_subassembly_tool_catalog_guide.md).

## Comparison Summary

| Component | Guide Requirement | Current Status | Notes |
| :--- | :--- | :--- | :--- |
| **Catalog (`.atc`)** | Main ATC file | **✓ Done** | [Main ATC](file:///w:/ASHU_tpi76_W/10_PROJECTS/Retaining%20Wall/RetainingWallSubassembly/deployment/RoadEdgeRetainingWall.bundle/RoadEdgeRetainingWall_Main.atc) contains the direct tool definition, bypassing nested folders. |
| **Registry (`.reg`)** | Registration in Windows Registry | **⚠ Pending** | [Registry Template](file:///w:/ASHU_tpi76_W/10_PROJECTS/Retaining%20Wall/RetainingWallSubassembly/deployment/RoadEdgeRetainingWall.bundle/RoadEdgeRetainingWall_Catalog.reg) created. You mentioned you'll run this later. |
| **Images** | 64x64px Icons for Catalog/Tools | **✓ Done** | Generated [RoadEdgeRetainingWall.png](file:///w:/ASHU_tpi76_W/10_PROJECTS/Retaining%20Wall/RetainingWallSubassembly/deployment/RoadEdgeRetainingWall.bundle/Images/RoadEdgeRetainingWall.png) and placed in `Images/`. |
| **Help Files** | `.chm` or Documentation | **⚠ In Progress** | `Help/` folder created. `.chm` reference exists in ATC, but actual compiled help is pending. |
| **Cover Page** | HTML landing page for catalog | **✓ Done** | [RoadEdgeRetainingWall_CoverPage.html](file:///w:/ASHU_tpi76_W/10_PROJECTS/Retaining%20Wall/RetainingWallSubassembly/deployment/RoadEdgeRetainingWall.bundle/RoadEdgeRetainingWall_CoverPage.html) created. |
| **Binaries** | Compiled `.dll` in `Contents` | **✓ Done** | Located in `Contents/Win64/`. |

## Key Differences & Adjustments

1.  **Bundle vs. Catalog Folder**: We are using the **Autodesk Bundle (`.bundle`)** format which is superior for modern Civil 3D/AutoCAD deployment as it allows for automatic loading via `ProgramData\Autodesk\ApplicationPlugins`.
2.  **Asset Locations**: The guide recommends an `Assemblies/` folder. We are using the bundle standard `Contents/Win64/` which is already correctly referenced in our [Tools ATC](file:///w:/ASHU_tpi76_W/10_PROJECTS/Retaining%20Wall/RetainingWallSubassembly/deployment/RoadEdgeRetainingWall.bundle/RoadEdgeRetainingWall_Tools.atc).
3.  **Registry Path**: The registry template uses `Software\Autodesk\Autodesk Content Browser\60`. This may need verification depending on the specific Civil 3D version's Content Browser instance.

## Next Steps for Full Alignment

- [ ] **Verify Registry Path**: Confirm if `60` is the correct version for Civil 3D 2026's Content Browser.
- [ ] **Compile Help**: Convert markdown documentation to a format suitable for the `Help/` directory (or update ATC to point to HTML).
- [ ] **Test in Content Browser**: Import the catalog into the Civil 3D Content Browser and verify the cover page and tool icons appear correctly.
