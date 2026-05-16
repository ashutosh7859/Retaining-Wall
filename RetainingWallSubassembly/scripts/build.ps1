[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',

    [string]$Platform = 'x64'
)

$ErrorActionPreference = 'Stop'

$Root = Resolve-Path (Join-Path $PSScriptRoot '..')
$Solution = Join-Path $Root 'RetainingWallSubassembly.sln'
$CoreTests = Join-Path $Root 'tests\RetainingWall.Core.Tests\RetainingWall.Core.Tests.csproj'

dotnet build $Solution --configuration $Configuration -p:Platform=$Platform
dotnet run --project $CoreTests --configuration $Configuration --no-restore
