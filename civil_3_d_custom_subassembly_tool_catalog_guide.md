# Autodesk Civil 3D Custom Subassembly Tool Catalog Guide

## Overview

An Autodesk Tool Catalog is used to organize and distribute custom subassemblies inside Autodesk Civil 3D.

A complete custom subassembly deployment typically includes:

| Component | Purpose |
|---|---|
| `.dll` | Compiled .NET subassembly assembly |
| `.atc` | Tool catalog definition |
| `.reg` | Windows registry registration |
| Images | Icons shown in Content Browser |
| Help files | Optional documentation |
| HTML cover page | Catalog introduction page |
| `.pkt` | Packaged distributable bundle |

---

# Tool Catalog Architecture

```text
Content Browser
│
├── Registry (.reg)
│
└── Catalog (.atc)
    │
    ├── Categories
    │   └── Tools
    │       └── Subassemblies
    │
    ├── Images
    ├── Help Files
    └── .NET DLL References
```

---

# Creating a Tool Catalog

## Step 1 — Create the Main `.atc` Catalog File

Create a file:

```text
<Name>Tools Catalog.atc
```

Example:

```text
RetainingWallTools Catalog.atc
```

### Important Notes

- XML tags are case-sensitive
- File format is XML
- Save as plain ASCII text
- Usually stored in the Tool Catalogs directory

---

# Main Catalog Structure

The main catalog contains:

- Catalog metadata
- Categories
- References to subassembly groups

## Example Structure

```xml
<Catalog>
    <ItemID idValue="{GUID}"/>

    <Properties>
        <ItemName>My Catalog</ItemName>
        <Description>Custom retaining wall tools</Description>
    </Properties>

    <Categories>
        <Category>
            <ItemID idValue="{CATEGORY_GUID}"/>
            <Url href="RetainingWalls.atc"/>
        </Category>
    </Categories>
</Catalog>
```

---

# Important Main Catalog Tags

| Tag | Purpose |
|---|---|
| `<Catalog>` | Root node |
| `<ItemID>` | Unique GUID |
| `<Properties>` | Catalog metadata |
| `<ItemName>` | Display name |
| `<Description>` | Description |
| `<Images>` | Catalog icon |
| `<Categories>` | Group of tools |
| `<Url>` | Reference to category `.atc` |

---

# Creating Categories and Tools

A category `.atc` file contains actual tools/subassemblies.

## Tool Category Structure

```xml
<Category>

    <ItemID idValue="{CATEGORY_GUID}"/>

    <Tools>

        <Tool>

            <ItemID idValue="{TOOL_GUID}"/>

            <Properties>
                <ItemName>BasicWall</ItemName>
            </Properties>

            <Data>
                <AeccDbSubassembly>

                    <GeometryGenerateMode>
                        UseDotNet
                    </GeometryGenerateMode>

                    <DotNetClass Assembly="MySubassembly.dll">
                        MyNamespace.BasicWall
                    </DotNetClass>

                </AeccDbSubassembly>
            </Data>

        </Tool>

    </Tools>

</Category>
```

---

# Critical .NET Tags

## GeometryGenerateMode

Defines implementation type.

```xml
<GeometryGenerateMode>UseDotNet</GeometryGenerateMode>
```

Possible values:

| Value | Meaning |
|---|---|
| `UseDotNet` | C#/.NET subassembly |
| `UseVBA` | VBA macro-based |

---

## DotNetClass

References your assembly and class.

```xml
<DotNetClass Assembly="MySubassembly.dll">
    Namespace.ClassName
</DotNetClass>
```

### Notes

- Paths are relative to the `.atc`
- Class must inherit Civil 3D subassembly base classes
- DLL must be deployable

---

# Defining Parameters

Parameters appear in Civil 3D Properties UI.

## Example

```xml
<Params>

    <WallHeight
        DataType="Double"
        TypeInfo="16"
        DisplayName="Wall Height"
        Description="Height of retaining wall">
        5.0
    </WallHeight>

</Params>
```

---

# Parameter Fields

| Field | Meaning |
|---|---|
| Parameter Name | Internal identifier |
| `DataType` | Variable type |
| `TypeInfo` | Validation type |
| `DisplayName` | UI name |
| `Description` | Tooltip/help |
| Value | Default value |

---

# Common Data Types

## Boolean

| Type | Meaning |
|---|---|
| Bool | True/False |
| BoolNoYes | No/Yes |
| BoolOffOn | Off/On |

## Integer Types

| Type | Meaning |
|---|---|
| Long | Any integer |
| NonNegativeLong | Positive or zero |
| NonZeroLong | Non-zero integer |

## Double Types

| TypeInfo | Meaning |
|---|---|
| 0 | Any double |
| 8 | Grade |
| 14 | Angle |
| 16 | Distance |
| 17 | Dimension |
| 21 | Elevation |
| 25 | Percent |

---

# Units

```xml
<Units>m</Units>
```

Valid values:

| Value | Meaning |
|---|---|
| `m` | Metric |
| `foot` | Imperial |

---

# Images and Icons

Recommended icon size:

```text
64 × 64 px
```

Supported formats:

- `.png`
- `.jpg`
- `.gif`
- `.bmp`

Example:

```xml
<Image cx="64" cy="64" src=".\Images\Wall.png"/>
```

---

# Help Files

Optional help integration.

```xml
<Help>
    <HelpFile>.\Help\WallHelp.chm</HelpFile>
    <HelpCommand>HELP_HHWND_TOPIC</HelpCommand>
    <HelpData>Wall.htm</HelpData>
</Help>
```

---

# Creating a Tool Catalog Cover Page

You can create an HTML landing page for the catalog.

Convention:

```text
<Name> - ToolCatalogCoverPage.html
```

Usually stored beside `.atc` files.

Typical content:

- Catalog overview
- Tool descriptions
- Usage instructions
- Branding

---

# Creating the Registry File (`.reg`)

Civil 3D catalogs must be registered in Windows Registry.

## Registry Structure

```reg
REGEDIT4

[HKEY_CURRENT_USER\Software\Autodesk\Autodesk Content Browser\60\RegisteredCatalogs\MyCatalog]

"ItemID"="{GUID}"
"Url"="C:\\ToolCatalogs\\MyCatalog.atc"
"DisplayName"="My Tools"
"Description"="Custom retaining wall tools"
"Publisher"="My Company"
```

---

# Important Registry Fields

| Key | Purpose |
|---|---|
| `ItemID` | Must match catalog GUID |
| `Url` | Path to `.atc` |
| `DisplayName` | Browser name |
| `Description` | Tooltip description |
| `Publisher` | Creator |
| `GroupType` | Catalog group GUID |

---

# GUID Requirements

GUIDs are mandatory for:

- Catalogs
- Categories
- Tools
- Registry entries

Example:

```text
{F6F066F4-ABF2-4838-B007-17DFDDE2C869}
```

You can generate GUIDs using:

- Visual Studio
- guidgen.exe
- PowerShell
- Online generators

---

# Installing Custom Subassemblies

## Manual Installation Workflow

### 1. Copy DLL

```text
MySubassembly.dll
```

Usually into:

```text
Sample\Civil 3D API\C3DStockSubassemblies
```

### 2. Copy `.atc` Files

Into Tool Catalog directory.

### 3. Copy Optional Assets

- Images
- Help files
- HTML pages

### 4. Register Catalog

Double-click `.reg`

OR import manually.

---

# Packaging as `.pkt`

`.pkt` is simply a renamed ZIP archive.

## Package Creation Workflow

### Step 1

Create folder:

```text
RetainingWallPackage\
```

### Step 2

Place required files:

```text
.dll
.atc
.reg
Images\
Help\
html
```

### Step 3

Zip folder.

### Step 4

Rename:

```text
Package.zip
```

to:

```text
Package.pkt
```

---

# Recommended Real-World Folder Structure

```text
RetainingWallCatalog/
│
├── RetainingWallCatalog.atc
├── RetainingWalls.atc
├── RetainingWallCatalog.reg
├── RetainingWallCoverPage.html
│
├── Assemblies/
│   └── RetainingWall.dll
│
├── Images/
│   ├── Catalog.png
│   └── Wall.png
│
└── Help/
    └── WallHelp.chm
```

---

# Recommended Workflow for Custom Retaining Wall Subassemblies

| Component | Recommendation |
|---|---|
| Logic | C# .NET |
| IDE | VS Code or Visual Studio |
| Deployment | Custom `.atc` |
| Distribution | `.pkt` |
| UI | Parameter-driven |
| Icons/help | Optional initially |

---

# Final Recommendation

For advanced retaining wall systems and highly custom corridor workflows:

- Prefer `.NET` over SAC
- Use custom `.atc` packaging
- Build reusable parameter-driven systems
- Maintain deployable `.pkt` distributions

SAC is useful for:

- quick generic assemblies
- visual prototyping
- simple parametric sections

However, `.NET` is significantly better for:

- scalable engineering systems
- reusable workflows
- advanced geometry logic
- automation
- interoperability with Revit and Dynamo
- enterprise-level infrastructure tooling

---

# Source

Based on Autodesk Civil 3D documentation regarding:

- Tool Catalog ATC files
- Registry configuration
- Custom subassembly deployment
- PKT packaging workflows

