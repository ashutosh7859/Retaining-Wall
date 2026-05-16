[CmdletBinding()]
param(
    [string]$DestinationRoot = (Join-Path $env:APPDATA 'Autodesk\ApplicationPlugins')
)

$ErrorActionPreference = 'Stop'

$Root = Resolve-Path (Join-Path $PSScriptRoot '..')
$BundleRoot = Join-Path $Root 'deployment\RoadEdgeRetainingWall.bundle'
$Destination = Join-Path $DestinationRoot 'RoadEdgeRetainingWall.bundle'

New-Item -ItemType Directory -Force -Path $DestinationRoot | Out-Null
Copy-Item -LiteralPath $BundleRoot -Destination $Destination -Recurse -Force

Write-Host "Installed development bundle to $Destination"
