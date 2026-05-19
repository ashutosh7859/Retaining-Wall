using System;
using System.IO;
using System.Reflection;

try
{
    var autocadDir = @"C:\Program Files\Autodesk\AutoCAD 2026";
    var c3dDir = @"C:\Program Files\Autodesk\AutoCAD 2026\C3D";
    
    // Set assembly resolve event to help loader find dependencies
    AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
    {
        var name = new AssemblyName(args.Name).Name;
        var path1 = Path.Combine(autocadDir, name + ".dll");
        if (File.Exists(path1)) return Assembly.LoadFrom(path1);
        var path2 = Path.Combine(c3dDir, name + ".dll");
        if (File.Exists(path2)) return Assembly.LoadFrom(path2);
        return null;
    };
    
    // Load acdbmgd first
    Assembly.LoadFrom(Path.Combine(autocadDir, "acdbmgd.dll"));
    Assembly.LoadFrom(Path.Combine(autocadDir, "acmgd.dll"));
    
    var assembly = Assembly.LoadFrom(Path.Combine(c3dDir, "AeccDbMgd.dll"));
    
    var stateType = assembly.GetType("Autodesk.Civil.Runtime.CorridorState");
    if (stateType == null)
    {
        Console.WriteLine("Could not find Autodesk.Civil.Runtime.CorridorState type.");
        return;
    }
    
    Console.WriteLine("=== CorridorState Properties ===");
    foreach (var prop in stateType.GetProperties())
    {
        Console.WriteLine($"- {prop.Name} (Type: {prop.PropertyType.FullName})");
    }
    
    Console.WriteLine("\n=== CorridorState Methods ===");
    foreach (var method in stateType.GetMethods())
    {
        if (method.DeclaringType == stateType)
        {
            Console.WriteLine($"- {method.ReturnType.Name} {method.Name}({string.Join(", ", System.Linq.Enumerable.Select(method.GetParameters(), p => p.ParameterType.Name + " " + p.Name))})");
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error during reflection: {ex}");
}

