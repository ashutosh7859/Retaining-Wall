using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RetainingWall.Core;

var failures = new List<string>();

// Existing basic tests
Expect(CoreAssembly.AssemblyName == "RetainingWall.Core", "Core assembly marker returns the expected name.");
ExpectNoAutodeskReferences(typeof(CoreAssembly).Assembly);

// Phase 2 tests
TestTableValues();
TestInvalidCases();

// Phase 3 tests
TestOverrides();

// Phase 4 tests
TestGeometryMirroring();
TestGeometryShapes();

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

void ExpectDimensions(int wallCase, double a, double b, double c, double d, double e, double f, double g, double h1, double h2)
{
    var dims = WallCaseTable.GetDimensions(wallCase);
    Expect(Math.Abs(dims.A - a) < 0.001, $"Wall case {wallCase} A should be {a}, but was {dims.A}");
    Expect(Math.Abs(dims.B - b) < 0.001, $"Wall case {wallCase} B should be {b}, but was {dims.B}");
    Expect(Math.Abs(dims.C - c) < 0.001, $"Wall case {wallCase} C should be {c}, but was {dims.C}");
    Expect(Math.Abs(dims.D - d) < 0.001, $"Wall case {wallCase} D should be {d}, but was {dims.D}");
    Expect(Math.Abs(dims.E - e) < 0.001, $"Wall case {wallCase} E should be {e}, but was {dims.E}");
    Expect(Math.Abs(dims.F - f) < 0.001, $"Wall case {wallCase} F should be {f}, but was {dims.F}");
    Expect(Math.Abs(dims.G - g) < 0.001, $"Wall case {wallCase} G should be {g}, but was {dims.G}");
    Expect(Math.Abs(dims.H1 - h1) < 0.001, $"Wall case {wallCase} H1 should be {h1}, but was {dims.H1}");
    Expect(Math.Abs(dims.H2 - h2) < 0.001, $"Wall case {wallCase} H2 should be {h2}, but was {dims.H2}");
}

void TestTableValues()
{
    ExpectDimensions(2, 0.450, 0.450, 1.450, 0.700, 0.250, 0.350, 0.250, 2.000, 0.850);
    ExpectDimensions(3, 0.450, 0.450, 1.900, 1.000, 0.250, 0.450, 0.250, 3.000, 1.100);
    ExpectDimensions(4, 0.450, 0.500, 2.400, 1.300, 0.250, 0.500, 0.250, 4.000, 1.350);
    ExpectDimensions(5, 0.450, 0.700, 2.750, 1.500, 0.300, 0.650, 0.300, 5.000, 1.650);
    ExpectDimensions(6, 0.450, 0.800, 3.200, 1.700, 0.450, 0.800, 0.450, 6.000, 1.850);
    ExpectDimensions(7, 0.450, 0.900, 3.800, 1.900, 0.500, 1.000, 0.500, 7.000, 2.050);
    ExpectDimensions(8, 0.450, 1.000, 3.950, 2.100, 0.650, 1.200, 0.650, 8.000, 2.150);
    ExpectDimensions(9, 0.450, 1.100, 4.450, 2.150, 0.750, 1.300, 0.750, 9.000, 2.250);
    ExpectDimensions(10, 0.450, 1.150, 4.600, 2.250, 0.750, 1.350, 0.750, 10.000, 2.300);
    ExpectDimensions(11, 0.450, 1.300, 4.850, 2.350, 0.750, 1.400, 0.750, 11.000, 2.400);
    ExpectDimensions(12, 0.450, 1.450, 5.250, 2.500, 0.800, 1.500, 0.800, 12.000, 2.500);
}

void TestInvalidCases()
{
    bool threwLower = false;
    try
    {
        WallCaseTable.GetDimensions(1);
    }
    catch (ArgumentOutOfRangeException)
    {
        threwLower = true;
    }
    Expect(threwLower, "Wall case 1 should throw ArgumentOutOfRangeException.");

    bool threwUpper = false;
    try
    {
        WallCaseTable.GetDimensions(13);
    }
    catch (ArgumentOutOfRangeException)
    {
        threwUpper = true;
    }
    Expect(threwUpper, "Wall case 13 should throw ArgumentOutOfRangeException.");
}

void TestOverrides()
{
    var baseDims = WallCaseTable.GetDimensions(2); // A:0.45, B:0.45, C:1.45, D:0.7, E:0.25, F:0.35, G:0.25, H1:2.0, H2:0.85

    // 1. Disabled overrides
    var overrides = new WallDimensionOverrides { UseDimensionOverrides = false, OverrideA = 1.0 };
    var result = overrides.Apply(baseDims);
    Expect(Math.Abs(result.A - 0.45) < 0.001, "A should not be overridden when UseDimensionOverrides is false.");

    // 2. Enabled overrides (all positive)
    overrides = new WallDimensionOverrides
    {
        UseDimensionOverrides = true,
        OverrideA = 1.1, OverrideB = 1.2, OverrideC = 1.3, OverrideD = 1.4,
        OverrideE = 1.5, OverrideF = 1.6, OverrideG = 1.7, OverrideH1 = 1.8, OverrideH2 = 1.9
    };
    result = overrides.Apply(baseDims);
    Expect(Math.Abs(result.A - 1.1) < 0.001, "A should be overridden to 1.1");
    Expect(Math.Abs(result.B - 1.2) < 0.001, "B should be overridden to 1.2");
    Expect(Math.Abs(result.C - 1.3) < 0.001, "C should be overridden to 1.3");
    Expect(Math.Abs(result.D - 1.4) < 0.001, "D should be overridden to 1.4");
    Expect(Math.Abs(result.E - 1.5) < 0.001, "E should be overridden to 1.5");
    Expect(Math.Abs(result.F - 1.6) < 0.001, "F should be overridden to 1.6");
    Expect(Math.Abs(result.G - 1.7) < 0.001, "G should be overridden to 1.7");
    Expect(Math.Abs(result.H1 - 1.8) < 0.001, "H1 should be overridden to 1.8");
    Expect(Math.Abs(result.H2 - 1.9) < 0.001, "H2 should be overridden to 1.9");

    // 3. Zero values
    overrides = new WallDimensionOverrides { UseDimensionOverrides = true, OverrideA = 0.0 };
    result = overrides.Apply(baseDims);
    Expect(Math.Abs(result.A - 0.45) < 0.001, "A should remain 0.45 when override is 0.0");

    // 4. Negative values
    overrides = new WallDimensionOverrides { UseDimensionOverrides = true, OverrideB = -1.5 };
    result = overrides.Apply(baseDims);
    Expect(Math.Abs(result.B - 0.45) < 0.001, "B should remain 0.45 when override is negative");

    // 5. Partial overrides
    overrides = new WallDimensionOverrides { UseDimensionOverrides = true, OverrideC = 2.5, OverrideD = 0, OverrideE = -0.5, OverrideF = 1.0 };
    result = overrides.Apply(baseDims);
    Expect(Math.Abs(result.A - 0.45) < 0.001, "A should remain 0.45 (no override provided)");
    Expect(Math.Abs(result.C - 2.5) < 0.001, "C should be overridden to 2.5");
    Expect(Math.Abs(result.D - 0.70) < 0.001, "D should remain 0.70 (override is 0)");
    Expect(Math.Abs(result.E - 0.25) < 0.001, "E should remain 0.25 (override is negative)");
    Expect(Math.Abs(result.F - 1.0) < 0.001, "F should be overridden to 1.0");
}

void TestGeometryMirroring()
{
    var dims = WallCaseTable.GetDimensions(2);
    var generator = new WallGeometryGenerator();

    var leftResult = generator.Generate(dims, Side.Left);
    var rightResult = generator.Generate(dims, Side.Right);

    Expect(leftResult.Points.Count == rightResult.Points.Count, "Left and right should have same number of points.");

    for (int i = 0; i < leftResult.Points.Count; i++)
    {
        var pLeft = leftResult.Points[i];
        var pRight = rightResult.Points[i];

        Expect(Math.Abs(pLeft.X - (-pRight.X)) < 0.001, $"Point {i} X should be mirrored. Left: {pLeft.X}, Right: {pRight.X}");
        Expect(Math.Abs(pLeft.Y - pRight.Y) < 0.001, $"Point {i} Y should be identical. Left: {pLeft.Y}, Right: {pRight.Y}");
    }
}

void TestGeometryShapes()
{
    var dims = WallCaseTable.GetDimensions(2);
    var generator = new WallGeometryGenerator();
    var result = generator.Generate(dims, Side.Left);

    Expect(result.Shapes.Count == 3, "Should generate 3 shapes: Concrete, PCC, Filter.");

    var concreteShape = result.Shapes.FirstOrDefault(s => s.Codes.Contains("RW_Concrete"));
    Expect(concreteShape != null, "Concrete shape should exist.");
    Expect(IsClosedShape(concreteShape.Links), "Concrete shape should be closed.");

    var pccShape = result.Shapes.FirstOrDefault(s => s.Codes.Contains("RW_PCC"));
    Expect(pccShape != null, "PCC shape should exist.");
    Expect(IsClosedShape(pccShape.Links), "PCC shape should be closed.");

    var filterShape = result.Shapes.FirstOrDefault(s => s.Codes.Contains("RW_FilterMedia"));
    Expect(filterShape != null, "Filter shape should exist.");
    Expect(IsClosedShape(filterShape.Links), "Filter shape should be closed.");
}

bool IsClosedShape(List<GeometryLink> links)
{
    if (links.Count < 3) return false;

    // Check if the end of one link connects to the start of the next,
    // and the last connects to the first.
    for (int i = 0; i < links.Count; i++)
    {
        var current = links[i];
        var next = links[(i + 1) % links.Count];

        if (Math.Abs(current.EndPoint.X - next.StartPoint.X) > 0.001 ||
            Math.Abs(current.EndPoint.Y - next.StartPoint.Y) > 0.001)
        {
            return false;
        }
    }
    return true;
}
