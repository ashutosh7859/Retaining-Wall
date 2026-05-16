[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release'
)

$ErrorActionPreference = 'Stop'

$Root = "$PSScriptRoot\.."
$BundleRoot = "$Root\deployment\RoadEdgeRetainingWall.bundle"
$Win64 = "$BundleRoot\Contents\Win64"

New-Item -ItemType Directory -Force -Path $Win64 | Out-Null

$ProjectNames = @('RetainingWall.Civil3D', 'RetainingWall.Commands')

foreach ($ProjectName in $ProjectNames) {
    $Candidates = @(
        "$Root\src\$ProjectName\bin\x64\$Configuration\net8.0-windows\$ProjectName.dll",
        "$Root\src\$ProjectName\bin\$Configuration\net8.0-windows\$ProjectName.dll"
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
