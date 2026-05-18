[CmdletBinding()]
param(
    [string]$DestinationRoot = 'C:\ProgramData\Autodesk\ApplicationPlugins',
    [switch]$SkipCatalogRegistration
)

$ErrorActionPreference = 'Stop'

$Root = Resolve-Path (Join-Path $PSScriptRoot '..')
$BundleRoot = Resolve-Path (Join-Path $Root 'deployment\RoadEdgeRetainingWall.bundle')
$DestinationRootPath = if (Test-Path $DestinationRoot) {
    (Resolve-Path -LiteralPath $DestinationRoot).Path
} else {
    (New-Item -ItemType Directory -Force -Path $DestinationRoot).FullName
}
$Destination = Join-Path $DestinationRootPath 'RoadEdgeRetainingWall.bundle'

if (Test-Path $Destination) {
    $resolvedDestination = (Resolve-Path -LiteralPath $Destination).Path
    $expectedLeaf = Split-Path -Leaf $resolvedDestination
    $isUnderDestinationRoot = $resolvedDestination.StartsWith($DestinationRootPath, [System.StringComparison]::OrdinalIgnoreCase)

    if (-not $isUnderDestinationRoot -or $expectedLeaf -ne 'RoadEdgeRetainingWall.bundle') {
        throw "Refusing to remove unexpected destination path: $resolvedDestination"
    }

    Remove-Item -LiteralPath $resolvedDestination -Recurse -Force
}

New-Item -ItemType Directory -Force -Path $Destination | Out-Null
Copy-Item -Path (Join-Path $BundleRoot '*') -Destination $Destination -Recurse -Force

if (-not $SkipCatalogRegistration) {
    $catalogPath = Join-Path $Destination 'RoadEdgeRetainingWall_Main.atc'
    $registryPath = 'HKCU:\Software\Autodesk\Autodesk Content Browser\88\RegisteredCatalogs\RoadEdgeRetainingWall'

    if (-not (Test-Path $catalogPath)) {
        throw "Catalog file was not installed: $catalogPath"
    }

    New-Item -Path $registryPath -Force | Out-Null
    Set-ItemProperty -Path $registryPath -Name 'ItemID' -Value '{6C3F6A92-3F95-46D2-92DE-C13E8B6D6E31}'
    Set-ItemProperty -Path $registryPath -Name 'Url' -Value $catalogPath
    Set-ItemProperty -Path $registryPath -Name 'DisplayName' -Value 'Road Edge Retaining Wall'
    Set-ItemProperty -Path $registryPath -Name 'Description' -Value 'Civil 3D retaining wall subassembly with corridor surfaces helper.'
    Set-ItemProperty -Path $registryPath -Name 'Publisher' -Value 'RetainingWallSubassembly'
    New-ItemProperty -Path $registryPath -Name 'Type' -PropertyType DWord -Value 1 -Force | Out-Null
}

Write-Host "Installed bundle to $Destination"
