[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release'
)

$ErrorActionPreference = 'Stop'

$Root = Resolve-Path (Join-Path $PSScriptRoot '..')
$BundleRoot = Join-Path $Root 'deployment\RoadEdgeRetainingWall.bundle'
$Win64 = Join-Path $BundleRoot 'Contents\Win64'

New-Item -ItemType Directory -Force -Path $Win64 | Out-Null

$ProjectNames = @('RetainingWall.Civil3D', 'RetainingWall.Commands')

foreach ($ProjectName in $ProjectNames) {
    $Candidates = @(
        Join-Path $Root "src\$ProjectName\bin\x64\$Configuration\net8.0-windows\$ProjectName.dll",
        Join-Path $Root "src\$ProjectName\bin\$Configuration\net8.0-windows\$ProjectName.dll"
    )

    $Source = $Candidates | Where-Object { Test-Path $_ } | Select-Object -First 1

    if ($Source) {
        Copy-Item -LiteralPath $Source -Destination $Win64 -Force
    }
    else {
        Write-Warning "$ProjectName.dll was not found. Run scripts\build.ps1 first."
    }
}

Write-Host "Bundle scaffold prepared at $BundleRoot"
