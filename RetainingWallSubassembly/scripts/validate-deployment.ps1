#!/usr/bin/env powershell
<# 
.SYNOPSIS
    Verify RoadEdgeRetainingWall bundle deployment and test in Civil 3D
.DESCRIPTION
    This script:
    1. Verifies bundle structure and file integrity
    2. Checks for missing dependencies
    3. Provides guidance for manual testing in Civil 3D
#>

param(
    [switch]$LaunchCivil3D = $false,
    [switch]$SkipValidation = $false
)

$ErrorActionPreference = "Stop"
$bundlePath = "C:\ProgramData\Autodesk\ApplicationPlugins\RoadEdgeRetainingWall.bundle"

Write-Host "`n========== RoadEdgeRetainingWall Deployment Validation ==========" -ForegroundColor Cyan

# 1. Verify bundle structure
Write-Host "`n[1/4] Checking bundle structure..." -ForegroundColor Yellow

$requiredFiles = @(
    "PackageContents.xml",
    "Contents/Win64/RetainingWall.Commands.dll",
    "Contents/Win64/RetainingWall.Civil3D.dll",
    "Contents/Win64/RetainingWall.Core.dll",
    "ToolPalette/RoadEdgeRetainingWall.xtp"
)

$allFilesPresent = $true
foreach ($file in $requiredFiles) {
    $fullPath = Join-Path $bundlePath $file
    if (Test-Path $fullPath) {
        Write-Host "  ✓ $file" -ForegroundColor Green
    } else {
        Write-Host "  ✗ $file (MISSING)" -ForegroundColor Red
        $allFilesPresent = $false
    }
}

if (-not $allFilesPresent) {
    Write-Host "`n⚠ Some files are missing. Deployment is incomplete." -ForegroundColor Red
    exit 1
}

# 2. Validate PackageContents.xml
Write-Host "`n[2/4] Validating PackageContents.xml..." -ForegroundColor Yellow
$xmlPath = Join-Path $bundlePath "PackageContents.xml"
try {
    [xml]$xml = Get-Content $xmlPath
    Write-Host "  ✓ XML is well-formed" -ForegroundColor Green
    
    # Check for required elements
    $commands = $xml.SelectNodes("//Command")
    Write-Host "  ✓ Found $($commands.Count) command(s) registered" -ForegroundColor Green
    
    $components = $xml.SelectNodes("//ComponentEntry")
    Write-Host "  ✓ Found $($components.Count) component(s)" -ForegroundColor Green
} catch {
    Write-Host "  ✗ XML validation failed: $_" -ForegroundColor Red
    exit 1
}

# 3. Check assembly signatures and blocking
Write-Host "`n[3/4] Checking assembly files..." -ForegroundColor Yellow
$dllPath = Join-Path $bundlePath "Contents/Win64/RetainingWall.Commands.dll"
$item = Get-Item $dllPath

$zone = $item.Zone
if ($zone -eq $null) {
    Write-Host "  ✓ $($item.Name) is not marked as blocked" -ForegroundColor Green
} else {
    Write-Host "  ⚠ $($item.Name) may be blocked (Zone: $zone)" -ForegroundColor Yellow
    Write-Host "    To unblock: Right-click file Properties Unblock OK" -ForegroundColor Gray
}

# 4. Summary
Write-Host "`n[4/4] Deployment Status" -ForegroundColor Yellow
Write-Host "  ✓ Bundle deployed to: $bundlePath" -ForegroundColor Green
Write-Host "  ✓ All required files present" -ForegroundColor Green
Write-Host "  ✓ Configuration valid" -ForegroundColor Green

Write-Host "`n========== Next Steps: Manual Testing in Civil 3D ==========" -ForegroundColor Cyan

$testInstructions = @"
Follow these steps to test the subassembly in Civil 3D 2026:

1. CLOSE Civil 3D completely if running

2. LAUNCH Civil 3D 2026 (fresh instance)

3. VERIFY BUNDLE LOAD:
   - Open command line (press C or Ctrl+9)
   - Type: NETLOAD
   - Select: $bundlePath\Contents\Win64\RetainingWall.Commands.dll

4. TEST SUBASSEMBLY:
   - Home > Create Subassembly
   - Look for RoadEdgeRetainingWall in the selector
   - Set WallCase=2, Side=Left
   - Create a corridor section and verify geometry

5. TEST RW_CREATE_SURFACES COMMAND:
   - Type: RW_CREATE_SURFACES
   - Select the corridor
   - Verify three surfaces are created

6. RECORD RESULTS in IMPLEMENTATION_STATUS.md
"@

Write-Host $testInstructions -ForegroundColor White

if ($LaunchCivil3D) {
    Write-Host "`n[!] Starting Civil 3D - manually run tests per instructions above" -ForegroundColor Yellow
    & "C:\Program Files\Autodesk\AutoCAD 2026\acad.exe"
}

Write-Host "`nValidation complete. Bundle is ready for testing." -ForegroundColor Green
