# Retaining Wall Subassembly: Validation Checklist

When deploying updates or validating a new installation, use the following manual QA steps in Civil 3D.

## 1. Bundle Registration
- [ ] Ensure Civil 3D is closed.
- [ ] Run `scripts/install-dev.ps1` and verify it succeeds.
- [ ] Run `scripts/validate-deployment.ps1` and verify the `.bundle` exists in the expected ApplicationPlugins folder.
- [ ] Open Civil 3D. Verify no error dialogs appear regarding assembly loading.

## 2. Tool Palette
- [ ] Open the Tool Palette (`Ctrl+3`).
- [ ] Verify the custom Retaining Wall subassembly icon is present.
- [ ] Click the tool and place it in a new Assembly.

## 3. Properties & Parameters
- [ ] Select the placed subassembly.
- [ ] Open the Properties Palette.
- [ ] Verify that `OperatingMode` defaults to `DynamicTable`.
- [ ] Change `OperatingMode` to `FullCustom` and verify that the custom dimensions take effect on the geometry.
- [ ] Change `OperatingMode` to `PresetDropdown` and verify that changing `WallPreset` from Type 2 to Type 12 correctly resizes the wall geometry.
- [ ] Change `Side` to `Right` and verify the geometry correctly mirrors.

## 4. Corridor Targeting
- [ ] Build a simple corridor using the assembly.
- [ ] Set `OperatingMode` to `DynamicTable`.
- [ ] Create a target EG surface that varies in elevation across the station range.
- [ ] Rebuild the corridor.
- [ ] Review cross-sections. Verify that the wall height and corresponding structural dimensions change dynamically based on the EG surface depth at each station.

## 5. Surfaces & Volumes
- [ ] Run the custom command `RW_CREATE_SURFACES`.
- [ ] Verify that `RW_Top_Surface` and `RW_Earthwork_Surface` are created in the Toolspace.
- [ ] Check the surface boundaries and ensure they match the top/datum link codes exactly.
- [ ] Run a Volume Dashboard report comparing `RW_Earthwork_Surface` to the EG surface. Verify a volume is calculated successfully.
