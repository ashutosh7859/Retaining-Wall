try {
    $env:PATH += ";C:\Program Files\Autodesk\AutoCAD 2026;C:\Program Files\Autodesk\AutoCAD 2026\C3D"
    Set-Location "C:\Program Files\Autodesk\AutoCAD 2026\C3D"
    
    # Load managed dependencies first
    [System.Reflection.Assembly]::LoadFrom("C:\Program Files\Autodesk\AutoCAD 2026\acdbmgd.dll") | Out-Null
    [System.Reflection.Assembly]::LoadFrom("C:\Program Files\Autodesk\AutoCAD 2026\acmgd.dll") | Out-Null
    
    # Load Civil 3D Subassembly runtime DLL
    $assembly = [System.Reflection.Assembly]::LoadFrom("C:\Program Files\Autodesk\AutoCAD 2026\C3D\AeccDbMgd.dll")
    
    Write-Host "=== CorridorState Members ==="
    $stateType = $assembly.GetType("Autodesk.Civil.Runtime.CorridorState")
    if ($stateType) {
        $stateType.GetMembers() | Where-Object { $_.MemberType -in "Property", "Method" } | Select-Object Name, MemberType | Format-Table -AutoSize
    } else {
        Write-Host "CorridorState type not found!"
    }
} catch {
    Write-Error $_
}
