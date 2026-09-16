namespace DependencyGraphTests;

using DependencyGraph;

/*
 * NOTE: DependencyGraph will be simplified to "DG" in code names
 */
[TestClass] 
public sealed class DependencyGraphTests
{
    // Given cells:
    // A1: "A2+A3", A2: "A3*A4", A3: "..." (no vars), A4: "..." (no vars)
    // the dependency graph looks as such:
    // cell | dependents | dependees
    // A1   |            | A2, A3
    // A2   | A1         | A3, A4
    // A3   | A1, A2     | 
    // A4   | A2         |
    // This is the default dependency graph used as an example from PS3
    private static DependencyGraph BuildExampleDGFromPS3()
    {
        // building dependencies for the formula cells above
        DependencyGraph dg = new DependencyGraph();
        // A1
        dg.AddDependency("A2", "A1");
        dg.AddDependency("A3", "A1");
        // A2
        dg.AddDependency("A3", "A2");
        dg.AddDependency("A4", "A2");
        // A3 & A4 don't have dependencies, but they will have dependents

        return dg;
    }
    
    [TestMethod]
    public void DG_AddDependencies_ValidCount()
    {
        DependencyGraph dg = new DependencyGraph();
        for (int i = 0; i < 1000; i++)
        {
            // for this simply add A_n as a dependency to A1 (i.e. A1 gains a receiver of A_n)
            dg.AddDependency(String.Format("A{0}",i+1), "A1");
        }
        Assert.AreEqual(1000, dg.Size);
    }
    
    [TestMethod]
    public void DG_AddDependenciesMultipleColumns_ValidCount()
    {
        DependencyGraph dg = new DependencyGraph();
        for (int i = 0; i < 1000; i++)
            dg.AddDependency(String.Format("B{0}",i+1), "A1");
        
        for (int i = 0; i < 1000; i++)
            dg.AddDependency(String.Format("C{0}",i+1), "B1");
        
        Assert.AreEqual(2000, dg.Size);
    }

    [TestMethod]
    public void DG_RemoveOnEmpty_DoNothingSizeEqualsZero()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.RemoveDependency("A2", "A1");
        Assert.AreEqual(0, dg.Size);
    }

    // This test is to check that in a formula in cells:
    // A1: "A2+A3", A2: "A3*A4", A3: "..." (no vars), A4: "..." (no vars)
    // the dependency graph looks as such:
    // cell | dependents | dependees
    // A1   |            | A2, A3
    // A2   | A1         | A3, A4
    // A3   | A1, A2     | 
    // A4   | A2         |
    // This matches the given example in PS3 and will be the standard test for making
    // sure that everything is correct
    [TestMethod]
    public void DG_SimpleDG_DependeesDependentsAreSavedInCorrectCollection()
    {
        // building dependencies for the formula cells above (which match the example from )
        DependencyGraph dg = BuildExampleDGFromPS3();
        
        // no dependents on A1
        Assert.IsEmpty(dg.GetDependents("A1"));
        // A2 and A3 as dependees on A1
        Assert.Contains("A2", dg.GetDependees("A1"));
        Assert.Contains("A3", dg.GetDependees("A1"));
        
        // A1 as a dependent of A2
        Assert.Contains("A1", dg.GetDependents("A2"));
        // A3 and A4 as dependees of A2
        Assert.Contains("A3", dg.GetDependees("A2"));
        Assert.Contains("A4", dg.GetDependees("A2"));
        
        // A1 and A2 as dependents of A3
        Assert.Contains("A1", dg.GetDependents("A3"));
        Assert.Contains("A2", dg.GetDependents("A3"));
        
        // A2 as dependents of A4
        Assert.Contains("A2", dg.GetDependents("A4"));
    }
    
    [TestMethod]
    public void DG_GetAllDependenciesAndDependentsOfColumn_Valid()
    {
        // for this test, simulate the formula in cell A1: "A2 + A3 + A4 + A5 + A6"
        List<string> dependeesList = ["A2", "A3", "A4", "A5", "A6"];
        
        DependencyGraph dg = new DependencyGraph();
        foreach (var dependee in dependeesList)
            dg.AddDependency(dependee, "A1");

        // I need to check both as they both maps internally are being modified when using "AddDependency"
        // I basically just need to check that adding dependees does not change the state of dependents
        foreach (var dependee in dependeesList)
            Assert.Contains(dependee, dg.GetDependees("A1"));
        
        // this should not have any dependents, so expect an empty list, not an exception
        var dependents = dg.GetDependents("A1");
        Assert.AreEqual(0, dependents.Count());
    }

    [TestMethod]
    public void DG_GetDependeesWhenEmpty_ReturnEmptyList()
    {
        DependencyGraph dg = new DependencyGraph();
        Assert.HasCount(0, dg.GetDependees("XYZ999"));
    }
    
    [TestMethod]
    public void DG_GetDependentsWhenEmpty_ReturnEmptyList()
    {
        DependencyGraph dg = new DependencyGraph();
        Assert.HasCount(0, dg.GetDependents("XYZ999"));
    }
    
    [TestMethod]
    public void DG_HasDependees_True()
    {
        DependencyGraph dg = new DependencyGraph();
        // cell A1: "A2+A3"
        dg.AddDependency("A2", "A1");
        dg.AddDependency("A3", "A1");

        Assert.IsTrue(dg.HasDependees("A1"));
    }

    [TestMethod]
    public void DG_HasDependeesWhenEmpty_False()
    {
        DependencyGraph dg = new DependencyGraph();
        Assert.IsFalse(dg.HasDependees("A1"));
    }

    [TestMethod]
    public void DG_HasDependents_True()
    {
        DependencyGraph dg = new DependencyGraph();
        // cell A1: "A2+A3"
        dg.AddDependency("A2", "A1");
        dg.AddDependency("A3", "A1");

        // both A2 and A3 have A1 as a dependent
        Assert.IsTrue(dg.HasDependents("A2"));
        Assert.IsTrue(dg.HasDependents("A3"));
    }

    [TestMethod]
    public void DG_HasDependentsWhenEmpty_False()
    {
        DependencyGraph dg = new DependencyGraph();
        Assert.IsFalse(dg.HasDependents("A1"));
    }
    
    [TestMethod]
    public void DG_NonEmpty_True()
    {
        DependencyGraph dg = new DependencyGraph();
        // cell A1: "A2"
        dg.AddDependency("A2", "A1");
        Assert.AreEqual(1, dg.Size);
    }
    
    [TestMethod]
    public void DG_RemoveOnlyDependency_SizeEqualsZero()
    {
        DependencyGraph dg = new DependencyGraph();
        // A1: "A2"
        dg.AddDependency("A2", "A1");
        // make sure the dependency exists so it can be removed
        Assert.AreEqual(1, dg.Size);
        
        dg.RemoveDependency("A2", "A1");
        Assert.AreEqual(0, dg.Size);
    }

    [TestMethod]
    public void DG_RemoveDependencyWhenEmpty_DoNothing()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.RemoveDependency("A2", "A1");
    }

    [TestMethod]
    public void DG_TestSimpleDependeeReplacement_ReplacedOneDependee()
    {
        DependencyGraph dg = new DependencyGraph();
        // cell A1: "A2"
        // - dependee: "A2"
        // - dependent: {}
        dg.AddDependency("A2", "A1");
        // make sure only 1 order pair exists: (A1, A2) (See above) and only 1 dependee exists
        Assert.HasCount(1, dg.GetDependees("A1"));
        Assert.Contains("A2", dg.GetDependees("A1"));
        Assert.AreEqual(1, dg.Size);
        
        // Replace the "A2" dependee with "A3"
        dg.ReplaceDependees("A1", ["A3"]);
        // then make sure only that dependee exists, and that the dependees still have a size of 1
        Assert.HasCount(1, dg.GetDependees("A1"));
        Assert.Contains("A3", dg.GetDependees("A1"));
        Assert.AreEqual(1, dg.Size);
    }

    [TestMethod]
    public void DG_TestSimpleDependentReplacement_ReplacedOneDependent()
    {
        DependencyGraph dg = new DependencyGraph();
        // cell A1: "A2"
        // - dependee: "A2"
        // - dependent: {}
        // A2:
        // - dependee: {}
        // - dependent: "A1"
        dg.AddDependency("A2", "A1");
        // make sure only 1 order pair exists: (A1, A2) (See above) and that A1 is truly a dependent of A2 (as expected) 
        Assert.Contains("A1", dg.GetDependents("A2"));
        Assert.HasCount(1, dg.GetDependents("A2"));
        Assert.AreEqual(1, dg.Size);
        
        // Replace the "A1" dependent of A2 with "A3" and ensure that, it is the only one replaced
        // (no duplicates and old dependent was replaced)
        dg.ReplaceDependents("A2", ["A3"]);
        Assert.Contains("A3", dg.GetDependents("A2"));
        Assert.HasCount(1, dg.GetDependents("A2"));
        Assert.AreEqual(1, dg.Size);
    }
    
    
    /*
    [TestMethod] public void DG_HasDependencies_True() { }
    [TestMethod] public void DG_HasDependencies_True() { }
    [TestMethod] public void DG_HasDependencies_True() { }
    [TestMethod] public void DG_HasDependencies_True() { }
    [TestMethod] public void DG_HasDependencies_True() { }
    [TestMethod] public void DG_HasDependencies_True() { }
    */
}