#!/usr/bin/env powershell
<#
.SYNOPSIS
    Validates the RoadEdgeRetainingWall Civil 3D bundle.
#>

[CmdletBinding()]
param(
    [string]$BundlePath = 'C:\ProgramData\Autodesk\ApplicationPlugins\RoadEdgeRetainingWall.bundle',
    [switch]$SkipRegistryCheck,
    [switch]$LaunchCivil3D
)

$ErrorActionPreference = 'Stop'

function Write-Pass {
    param([string]$Message)
    Write-Host "  [OK] $Message" -ForegroundColor Green
}

function Write-Info {
    param([string]$Message)
    Write-Host "  [INFO] $Message" -ForegroundColor Gray
}

function Assert-Condition {
    param(
        [bool]$Condition,
        [string]$Message
    )

    if (-not $Condition) {
        throw $Message
    }

    Write-Pass $Message
}

function Assert-RequiredFile {
    param(
        [string]$Root,
        [string]$RelativePath
    )

    $fullPath = Join-Path $Root $RelativePath
    if (-not (Test-Path -LiteralPath $fullPath)) {
        throw "Missing required file: $RelativePath"
    }

    Write-Pass "Found $RelativePath"
}

function Test-ZoneIdentifier {
    param(
        [string]$Path,
        [string]$DisplayName
    )

    try {
        $zoneStream = Get-Item -LiteralPath $Path -Stream Zone.Identifier -ErrorAction Stop
    } catch [System.Management.Automation.ItemNotFoundException] {
        Write-Pass "$DisplayName is not marked as downloaded from the internet."
        return
    } catch [System.IO.FileNotFoundException] {
        Write-Pass "$DisplayName is not marked as downloaded from the internet."
        return
    } catch {
        Write-Info "Zone.Identifier stream check is not available for $DisplayName on this file system."
        return
    }

    if ($null -ne $zoneStream) {
        throw "$DisplayName has a Zone.Identifier stream. Unblock it before loading in Civil 3D."
    }

    Write-Pass "$DisplayName is not marked as downloaded from the internet."
}

Write-Host ''
Write-Host '========== RoadEdgeRetainingWall Deployment Validation ==========' -ForegroundColor Cyan

if (-not (Test-Path -LiteralPath $BundlePath)) {
    throw "Bundle path does not exist: $BundlePath"
}

$resolvedBundlePath = (Resolve-Path -LiteralPath $BundlePath).Path
Write-Info "Bundle path: $resolvedBundlePath"

Write-Host ''
Write-Host '[1/5] Checking bundle structure...' -ForegroundColor Yellow

$requiredFiles = @(
    'PackageContents.xml',
    'RoadEdgeRetainingWall_Main.atc',
    'Contents/Win64/RetainingWall.Commands.dll',
    'Contents/Win64/RetainingWall.Civil3D.dll',
    'Contents/Win64/RetainingWall.Core.dll',
    'ToolPalette/RoadEdgeRetainingWall.xtp',
    'Images/RoadEdgeRetainingWall.png',
    'Images/Surface.png'
)

foreach ($file in $requiredFiles) {
    Assert-RequiredFile -Root $resolvedBundlePath -RelativePath $file
}

Write-Host ''
Write-Host '[2/5] Validating PackageContents.xml...' -ForegroundColor Yellow

$manifestPath = Join-Path $resolvedBundlePath 'PackageContents.xml'
[xml]$manifest = Get-Content -LiteralPath $manifestPath

$runtimeRequirements = $manifest.SelectSingleNode('//RuntimeRequirements')
Assert-Condition ($null -ne $runtimeRequirements) 'Manifest has RuntimeRequirements.'
Assert-Condition ($runtimeRequirements.SeriesMin -eq 'R25.1' -and $runtimeRequirements.SeriesMax -eq 'R25.1') 'Manifest targets AutoCAD/Civil 3D series R25.1.'

$commands = @($manifest.SelectNodes('//Command'))
$commandNames = @($commands | ForEach-Object { $_.GetAttribute('Global') })
Assert-Condition ($commandNames -contains 'RW_CREATE_SURFACES') 'Manifest registers RW_CREATE_SURFACES.'
Assert-Condition (-not ($commandNames -contains 'RW_ROAD_EDGE_RETAINING_WALL')) 'Manifest does not register fake RW_ROAD_EDGE_RETAINING_WALL command.'

$componentEntries = @($manifest.SelectNodes('//ComponentEntry'))
Assert-Condition ($componentEntries.Count -eq 2) 'Manifest has two component entries.'

Write-Host ''
Write-Host '[3/5] Validating ATC catalog...' -ForegroundColor Yellow

$catalogPath = Join-Path $resolvedBundlePath 'RoadEdgeRetainingWall_Main.atc'
[xml]$catalog = Get-Content -LiteralPath $catalogPath

$geometryMode = $catalog.SelectSingleNode('//GeometryGenerateMode')
Assert-Condition ($null -ne $geometryMode -and $geometryMode.InnerText -eq 'UseDotNet') 'ATC uses .NET geometry generation.'

$dotNetClass = $catalog.SelectSingleNode('//DotNetClass')
Assert-Condition ($null -ne $dotNetClass) 'ATC contains DotNetClass.'
Assert-Condition ($dotNetClass.InnerText -eq 'Subassembly.RoadEdgeRetainingWall') 'ATC points to Subassembly.RoadEdgeRetainingWall.'

$assemblyReference = $dotNetClass.GetAttribute('Assembly')
$expectedRelativeAssembly = '.\Contents\Win64\RetainingWall.Civil3D.dll'
$expectedAbsoluteAssembly = 'C:\ProgramData\Autodesk\ApplicationPlugins\RoadEdgeRetainingWall.bundle\Contents\Win64\RetainingWall.Civil3D.dll'
Assert-Condition (($assemblyReference -eq $expectedRelativeAssembly) -or ($assemblyReference -eq $expectedAbsoluteAssembly)) 'ATC uses an expected Civil3D assembly path.'

if ([System.IO.Path]::IsPathRooted($assemblyReference)) {
    $resolvedAssemblyPath = $assemblyReference
} else {
    $assemblyPath = ($assemblyReference -replace '^[.][\\/]', '') -replace '/', '\'
    $resolvedAssemblyPath = Join-Path $resolvedBundlePath $assemblyPath
}
Assert-Condition (Test-Path -LiteralPath $resolvedAssemblyPath) 'ATC DotNetClass assembly exists.'

$units = $catalog.SelectSingleNode('//Units')
Assert-Condition ($null -ne $units -and $units.InnerText -eq 'm') 'ATC units are metric meters.'

Write-Host ''
Write-Host '[4/5] Checking assembly file state...' -ForegroundColor Yellow

foreach ($dll in @(
    'Contents/Win64/RetainingWall.Commands.dll',
    'Contents/Win64/RetainingWall.Civil3D.dll',
    'Contents/Win64/RetainingWall.Core.dll'
)) {
    $dllPath = Join-Path $resolvedBundlePath $dll
    Test-ZoneIdentifier -Path $dllPath -DisplayName $dll
}

Write-Host ''
Write-Host '[5/5] Checking Content Browser catalog registry...' -ForegroundColor Yellow

if ($SkipRegistryCheck) {
    Write-Info 'Registry check skipped.'
} else {
    $registryPath = 'HKCU:\Software\Autodesk\Autodesk Content Browser\88\RegisteredCatalogs\RoadEdgeRetainingWall'
    if (-not (Test-Path $registryPath)) {
        throw "Catalog registry key is missing: $registryPath"
    }

    $registry = Get-ItemProperty -Path $registryPath
    $expectedCatalogPath = Join-Path $resolvedBundlePath 'RoadEdgeRetainingWall_Main.atc'
    Assert-Condition ($registry.Url -eq $expectedCatalogPath) 'Registry URL points to the installed ATC catalog.'
    Assert-Condition ($registry.DisplayName -eq 'Road Edge Retaining Wall') 'Registry display name is correct.'
    Assert-Condition ($registry.Type -eq 1) 'Registry catalog type is 1.'
}

Write-Host ''
Write-Host 'Validation complete. Bundle is ready for Civil 3D manual testing.' -ForegroundColor Green

if ($LaunchCivil3D) {
    $civil3dExe = 'C:\Program Files\Autodesk\AutoCAD 2026\acad.exe'
    if (-not (Test-Path -LiteralPath $civil3dExe)) {
        throw "Civil 3D executable not found: $civil3dExe"
    }

    Start-Process -FilePath $civil3dExe -WindowStyle Hidden
}
