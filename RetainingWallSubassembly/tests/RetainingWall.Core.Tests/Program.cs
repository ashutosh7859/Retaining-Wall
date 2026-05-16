using System.Reflection;
using RetainingWall.Core;

var failures = new List<string>();

Expect(CoreAssembly.AssemblyName == "RetainingWall.Core", "Core assembly marker returns the expected name.");
ExpectNoAutodeskReferences(typeof(CoreAssembly).Assembly);

if (failures.Count > 0)
{
    Console.Error.WriteLine("RetainingWall.Core.Tests failed:");
    foreach (var failure in failures)
    {
        Console.Error.WriteLine($"- {failure}");
    }

    return 1;
}

Console.WriteLine("RetainingWall.Core.Tests passed.");
return 0;

void Expect(bool condition, string description)
{
    if (!condition)
    {
        failures.Add(description);
    }
}

void ExpectNoAutodeskReferences(Assembly assembly)
{
    var forbidden = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "AeccDbMgd",
        "acmgd",
        "acdbmgd",
        "accoremgd"
    };

    var unexpected = assembly.GetReferencedAssemblies()
        .Select(reference => reference.Name)
        .Where(name => name is not null && forbidden.Contains(name))
        .ToArray();

    Expect(unexpected.Length == 0, $"Core assembly should not reference Autodesk assemblies. Found: {string.Join(", ", unexpected)}");
}
