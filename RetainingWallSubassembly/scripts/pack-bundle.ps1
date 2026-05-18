[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',

    [string]$Platform = 'x64'
)

$ErrorActionPreference = 'Stop'

$Root = Resolve-Path (Join-Path $PSScriptRoot '..')
$BundleRoot = Join-Path $Root 'deployment\RoadEdgeRetainingWall.bundle'
$Win64 = Join-Path $BundleRoot 'Contents\Win64'

New-Item -ItemType Directory -Force -Path $Win64 | Out-Null

Get-ChildItem -LiteralPath $Win64 -File | Remove-Item -Force

$projects = @(
    @{ Name = 'RetainingWall.Core'; Framework = 'net8.0' },
    @{ Name = 'RetainingWall.Civil3D'; Framework = 'net8.0-windows' },
    @{ Name = 'RetainingWall.Commands'; Framework = 'net8.0-windows' }
)

foreach ($project in $projects) {
    $projectName = $project.Name
    $framework = $project.Framework
    $candidates = @(
        (Join-Path $Root "src\$projectName\bin\$Platform\$Configuration\$framework"),
        (Join-Path $Root "src\$projectName\bin\$Configuration\$framework")
    )

    $sourceDir = $candidates | Where-Object { Test-Path $_ } | Select-Object -First 1
    if (-not $sourceDir) {
        throw "$projectName output was not found. Run scripts\build.ps1 first."
    }

    foreach ($extension in @('dll', 'pdb', 'xml')) {
        Get-ChildItem -LiteralPath $sourceDir -Filter "$projectName.$extension" -File -ErrorAction SilentlyContinue |
            Copy-Item -Destination $Win64 -Force
    }

    $dllPath = Join-Path $Win64 "$projectName.dll"
    if (-not (Test-Path $dllPath)) {
        throw "$projectName.dll was not copied into $Win64."
    }
}

Write-Host "Bundle prepared at $BundleRoot"
