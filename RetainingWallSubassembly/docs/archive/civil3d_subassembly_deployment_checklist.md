# Civil 3D .NET Subassembly Deployment Checklist

This guide provides a repeatable office-wide checklist for deploying custom .NET subassemblies in Autodesk Civil 3D by using manual loading for validation, application bundles for managed deployment, custom tool palettes for user access, and package registration through `PackageContents.xml`.[web:7][web:9] It is intended for developers and CAD/BIM administrators who need a searchable internal PDF-style reference for consistent rollouts across multiple workstations.[web:7][web:10]

## Scope

Use this checklist when the deliverable includes a compiled .NET DLL, supporting assemblies, palette content, icons, and an Autodesk application bundle that Civil 3D can discover at startup or on command invocation.[web:7][web:10] Autodesk documents that `PackageContents.xml` defines the application package, the products and releases it supports, and how the plug-in is loaded, including startup or command-triggered loading.[web:7]

## 1. Prepare the build

- [ ] Target the correct AutoCAD/Civil 3D runtime for the release being deployed, because Autodesk notes that the autoloader behavior changed with AutoCAD 2025 and `RuntimeRequirements` must be specified for .NET applications.[web:10]
- [ ] Build the primary subassembly DLL and confirm that every referenced dependency required at runtime is copied to the deployment output so the loader can resolve those files from the bundle location.[web:7][web:10]
- [ ] Keep all managed dependencies, resource files, icons, palette files, and support content inside the application bundle so paths remain portable and office deployment is predictable.[web:7]
- [ ] Verify that the assembly is not blocked by Windows after download or copy operations, because blocked files or untrusted locations can prevent loading in AutoCAD-based products.[web:15]

### Recommended folder pattern

Use an Autodesk application bundle under the `ApplicationPlugins` folder so Civil 3D can discover the package through the standard autoloader structure.[web:7] A common layout is shown below.

```text
YourCompany.Subassembly.bundle/
├─ PackageContents.xml
├─ Contents/
│  ├─ Win64/
│  │  ├─ YourCompany.Subassembly.dll
│  │  ├─ DependencyA.dll
│  │  ├─ DependencyB.dll
│  │  ├─ Resources\...
│  │  └─ Icons\...
│  └─ ToolPalettes/
│     └─ CustomPaletteContent\...
```

## 2. Validate with NETLOAD

- [ ] Start Civil 3D and run `NETLOAD` to load the compiled DLL manually during development or troubleshooting.[web:9]
- [ ] If `FILEDIA` is set to 0, provide the assembly file path at the command prompt, because Autodesk states `NETLOAD` will prompt for the assembly file name in that mode.[web:9]
- [ ] After loading, run one of the registered commands to confirm the assembly initializes correctly before packaging it for wider deployment.[web:7][web:9]
- [ ] Treat successful `NETLOAD` testing as a pre-deployment gate; if the DLL does not load manually, correct dependency, trust, or version issues before bundle rollout.[web:9][web:15]

### NETLOAD troubleshooting checks

- [ ] Confirm the DLL and all dependent DLLs are present in accessible locations.[web:15]
- [ ] Confirm the folder is trusted in the AutoCAD/Civil 3D file security settings when enterprise policy requires trusted paths for executable content.[web:15]
- [ ] Confirm downloaded files are unblocked in Windows file properties when copied from email, Teams, or the internet.[web:15]
- [ ] Confirm the build targets the correct framework/runtime expected by the Civil 3D release in use.[web:10]

## 3. Register the bundle

Place the bundle in a location monitored by Autodesk's application autoloader, typically under an `ApplicationPlugins` folder, and define the package with `PackageContents.xml`.[web:7] Autodesk states that `PackageContents.xml` can specify supported products, supported releases, operating system, support paths, tool palette paths, commands, and whether the app loads on startup.[web:7]

### Minimum registration tasks

- [ ] Create a `PackageContents.xml` file at the root of the `.bundle` folder.[web:7]
- [ ] Add an `ApplicationPackage` element with required metadata such as `SchemaVersion`, `AppVersion`, and `ProductCode`.[web:7]
- [ ] Add `Components` and at least one `ComponentEntry` with the DLL path in `ModuleName`.[web:7]
- [ ] Set `AppType` to `.Net` for the managed assembly entry.[web:10]
- [ ] Add `RuntimeRequirements` with `OS="Win64"` and explicit `SeriesMin` and `SeriesMax` values appropriate for the Civil 3D/AutoCAD platform version being supported.[web:10]
- [ ] Define `Commands` entries so Civil 3D can invoke the assembly through a known command group and command names.[web:7]
- [ ] Add `SupportPath` and `ToolPalettePath` when the package includes support files and tool palette content stored inside the bundle.[web:7]

### Example `PackageContents.xml`

```xml
<?xml version="1.0" encoding="utf-8"?>
<ApplicationPackage
  SchemaVersion="1.0"
  Name="YourCompany Subassembly Tools"
  AppVersion="1.0.0"
  Description="Custom Civil 3D subassembly deployment"
  ProductCode="PUT-GUID-HERE">

  <CompanyDetails Name="YourCompany" />

  <Components>
    <RuntimeRequirements OS="Win64" />

    <ComponentEntry
      AppName="YourCompany Subassembly Tools"
      ModuleName="./Contents/Win64/YourCompany.Subassembly.dll"
      AppDescription="Custom Civil 3D subassembly loader"
      AppType=".Net"
      LoadOnAutoCADStartup="True">

      <RuntimeRequirements OS="Win64" SeriesMin="R25.0" SeriesMax="R25.0" />

      <Commands GroupName="YC_SUBASSY">
        <Command Global="YCLOADSUB" Local="YCLOADSUB" />
      </Commands>
    </ComponentEntry>
  </Components>
</ApplicationPackage>
```

This example follows Autodesk's documented package structure and the 2025 guidance to declare explicit runtime bounds for .NET applications.[web:7][web:10]

## 4. Reference dependencies correctly

The most important deployment rule is that every assembly dependency the main DLL needs at runtime must be physically present where the assembly loader can resolve it from the bundle, or exposed through paths declared in the package.[web:7] Autodesk documents `SupportPath` as a package option, which can help direct AutoCAD-based products to support content inside the bundle.[web:7]

### Dependency checklist

- [ ] Set project references so dependent DLLs copy to the output folder during build.[web:12]
- [ ] Avoid mixing debug-only local paths, developer machine absolute paths, or user-profile-specific paths in production deployment.[web:7]
- [ ] Keep version-matched dependencies together in the bundle's `Contents/Win64` folder unless a different structured support path is intentionally declared.[web:7]
- [ ] Test the bundle on a clean workstation that does not have developer tools installed, because hidden machine-level references can mask missing deployment files.[web:7]
- [ ] If the subassembly relies on additional resources such as JSON, XML, images, CUIX, ATC, or palette assets, place those files in the bundle and reference them by relative path where possible.[web:7]

## 5. Create custom tool palettes

Autodesk's Civil 3D content packs can include tool palettes for subassemblies, which confirms tool palette content is a supported deployment asset in Civil 3D environments.[web:4] Autodesk also documents `ToolPalettePath` in `PackageContents.xml`, allowing palette content stored in the package to be referenced as part of deployment.[web:7]

### Palette implementation steps

- [ ] Build the custom tool palette content and store it in a dedicated folder inside the bundle, such as `Contents/ToolPalettes`.[web:7]
- [ ] Add or update `ToolPalettePath` in the package so Civil 3D can locate the palette resources supplied with the bundle.[web:7]
- [ ] Standardize palette names, icons, and category structure across the office so users can find subassemblies consistently.[web:4]
- [ ] Test the palette on a non-developer workstation and confirm that dragged or invoked tools still resolve the underlying subassembly and support files.[web:4][web:7]

## 6. Office rollout procedure

- [ ] Freeze a release folder containing the final `.bundle` package and record the supported Civil 3D versions.[web:7][web:10]
- [ ] Deploy the bundle to the agreed workstation or network `ApplicationPlugins` location used by the office CAD standard.[web:7]
- [ ] Open Civil 3D and confirm autoload behavior at startup or confirm command-triggered loading as configured in `PackageContents.xml`.[web:7]
- [ ] Verify the custom commands are available and that the palette content appears correctly.[web:4][web:7]
- [ ] Run a corridor test drawing using the deployed subassembly to confirm functional behavior, not just successful loading.[web:9]
- [ ] Document the package version, deployment date, rollback location, and owning developer/admin for support continuity.[web:7]

## 7. Failure patterns

| Symptom | Likely cause | Corrective action |
|---|---|---|
| DLL loads with `NETLOAD` on one machine only | Missing dependency or hidden local reference | Copy all required dependent assemblies into the bundle and retest on a clean machine.[web:7][web:15] |
| Bundle is ignored at startup | Incorrect or incomplete `PackageContents.xml` | Validate `ApplicationPackage`, `ComponentEntry`, `ModuleName`, and `RuntimeRequirements` entries.[web:7][web:10] |
| Works before 2025, fails in newer releases | Runtime bounds not aligned with current AutoCAD platform | Add explicit `SeriesMin` and `SeriesMax` entries per Autodesk 2025 guidance.[web:10] |
| Commands do not appear after load | Commands not declared correctly or assembly did not initialize fully | Recheck command registration and test by manual `NETLOAD` first.[web:7][web:9] |
| NETLOAD reports load failure | File blocked, untrusted location, or missing dependency | Unblock the file, verify trusted path policy, and confirm dependent DLL availability.[web:15] |

## 8. Standard office checklist

### Developer handoff

- [ ] Source project builds without warnings or missing references.
- [ ] Release output includes the main DLL and every dependent DLL required at runtime.[web:7]
- [ ] Bundle contains `PackageContents.xml`, icons, palette files, and support content.[web:7]
- [ ] `NETLOAD` validation completed in Civil 3D before handoff.[web:9]
- [ ] Supported Civil 3D releases documented using platform series mapping.[web:10]

### CAD admin acceptance

- [ ] Bundle copied to approved `ApplicationPlugins` deployment location.[web:7]
- [ ] Tool palette path resolves correctly on a standard user workstation.[web:7]
- [ ] Commands load and execute under a non-admin user profile.[web:7][web:15]
- [ ] Sample drawing test completed successfully.[web:9]
- [ ] Rollback package archived.

## 9. Implementation note

For office-wide implementation, use manual `NETLOAD` only as a validation step and rely on a properly structured `.bundle` package with an accurate `PackageContents.xml` file for repeatable deployment.[web:7][web:9] The package should keep the primary DLL, all dependent assemblies, and tool palette/support assets together so the Civil 3D assembly loader can resolve them consistently on every workstation.[web:7][web:10]
