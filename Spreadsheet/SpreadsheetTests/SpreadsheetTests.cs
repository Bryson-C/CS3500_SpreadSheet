using Spreadsheets;

namespace SpreadsheetTests;

[TestClass]
public sealed class SpreadsheetTests
{
    [TestMethod]
    public void GetNamesOfNonEmptyCells_Empty_ReturnEmptyList()
    {
        Spreadsheet ss = new();
        Assert.IsEmpty(ss.GetNamesOfAllNonemptyCells());
    }

    [TestMethod]
    public void GetNamesOfNonEmptyCells_NonEmpty_ReturnNonEmptyList()
    {
        Spreadsheet ss = new();
        ss.SetCellContents("A1", 1.0);
        ss.SetCellContents("A2", 2.0);
        ss.SetCellContents("A3", 3.0);
        ss.SetCellContents("A4", 4.0);
        
        Assert.Contains("A1", ss.GetNamesOfAllNonemptyCells());
        Assert.Contains("A2", ss.GetNamesOfAllNonemptyCells());
        Assert.Contains("A3", ss.GetNamesOfAllNonemptyCells());
        Assert.Contains("A4", ss.GetNamesOfAllNonemptyCells());
    }

    [TestMethod]
    public void GetCellContents_InvalidName_ThrowsError()
    {
        Spreadsheet ss = new();
        Assert.Throws<InvalidNameException>(() => ss.GetCellContents("SomeCell"));
    }
    
    // this will test throwing errors for strings, doubles, and formulas as the actual value doesnt matter,
    // just that the error occurs in all circumstances
    [TestMethod]
    public void SetCellContents_InvalidName_ThrowError()
    {
        Spreadsheet ss = new();
        Assert.Throws<InvalidNameException>(() => ss.SetCellContents("SomeName", 100) );
        Assert.Throws<InvalidNameException>(() => ss.SetCellContents("AnotherName", "Hello World") );
        Assert.Throws<InvalidNameException>(() => ss.SetCellContents("YetAnotherName", new Formula.Formula("A1")) );
    }

    [TestMethod]
    public void SetCellContents_OverwriteContentsWithDifferentTypes_Valid()
    {
        Spreadsheet ss = new();
        ss.SetCellContents("a1", 100);
        ss.SetCellContents("a1", "one hundred");
        ss.SetCellContents("a1", new Formula.Formula("50 * 2"));
    }

    [TestMethod]
    public void SetCellContents_SetUniqueCellsOfDifferentTypes_Valid()
    {
        Spreadsheet ss = new();
        ss.SetCellContents("A1", -100);
        ss.SetCellContents("B1", "negative one hundred");
        ss.SetCellContents("C1", new Formula.Formula("100 - 200"));
    }
    
    [TestMethod]
    public void SetCellContents_ValidCell_HasDependencies()
    {
        // Test based off the given example:
        // For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1,
        // the list [A1, B1, C1] is returned, i.e., A1 was changed, so then A1
        // must be evaluated, followed by B1, followed by C1
        
        // Set up spreadsheet
        Spreadsheet ss = new();
        ss.SetCellContents("A1", 100);
        ss.SetCellContents("B1", new Formula.Formula("A1*2"));
        ss.SetCellContents("C1", new Formula.Formula("B1+A1"));


        // Then change A1, expect [A1, B1, C1]
        var change = ss.SetCellContents("A1", 10);
        Assert.AreEqual("A1", change[0]);
        Assert.AreEqual("B1", change[1]);
        Assert.AreEqual("C1", change[2]);

    }

    // Basically checking that it still works if each dependency is not just one after the other
    // in a straight path
    [TestMethod]
    public void SetCellContents_BranchingDependencies_HasMultipleDependencyPaths()
    {
        Spreadsheet ss = new Spreadsheet();
        ss.SetCellContents("A1", 100);
        ss.SetCellContents("B1", new Formula.Formula("A1 + 100"));
        ss.SetCellContents("C1", new Formula.Formula("A1 + 200"));
        ss.SetCellContents("D1", new Formula.Formula("C1 + B1"));
        ss.SetCellContents("E1", new Formula.Formula("D1"));
        
        // by changing A1, its expected that the cells should change in this order:
        // [A1, C1, B1, D1, E1]
        // The reason C1 comes before B1 should be because the function "GetCellsToRecalculate"
        // calls ".AddFirst(...)", meaning append to front, and since C1 will be "visited" later
        // that means that C1 will be "added first" after B1
        var change = ss.SetCellContents("A1", 10);
        Assert.AreEqual("A1", change[0]); // self
        Assert.AreEqual("C1", change[1]);
        Assert.AreEqual("B1", change[2]);
        Assert.AreEqual("D1", change[3]);
        Assert.AreEqual("E1", change[4]);
    }
    
    [TestMethod]
    public void SetCellContents_CircularDependency_ThrowCircularError()
    {
        Spreadsheet ss = new();
        ss.SetCellContents("A1", new Formula.Formula("B1"));
        Assert.Throws<CircularException>(() => ss.SetCellContents("B1", new Formula.Formula("A1")) );
    }
    
    // It's not mentioned explicitly whether to allow overwriting previous cells,
    // but given the name "setCellContents", I would expect that to change the cell regardless.
    // That being said, calling ".Add(key, val)" on a dictionary with the key already added throws an exception
    // so basically just make sure nothing throws
    [TestMethod]
    public void SetCellContents_OverwritePreviousValue_Valid()
    {
        Spreadsheet ss = new();
        ss.SetCellContents("A1", 100);
        ss.SetCellContents("A1", 20);
        ss.SetCellContents("B1", 100);
        ss.SetCellContents("B1", 20);
        
        Assert.AreEqual(20.0, (double)ss.GetCellContents("A1"), 1e-9);
        Assert.AreEqual(20.0, (double)ss.GetCellContents("B1"), 1e-9);
    }
    
    // These Tests Are Meant To Pair The Type Check With The Conversion As They Should Almost Always Be Paired
    // Together Anyways
    [TestMethod]
    public void Cell_FormulaConversions_IsFormulaAndValidConversion()
    {
        Cell c = new Cell(new Formula.Formula("A1 + B2"));
        Assert.IsTrue(c.IsFormulaType());
        // Do conversion, shouldn't throw an error if above is true
        var f = ((Formula.Formula)c.GetCellData()).GetVariables();
        Assert.Contains("A1", f);
        Assert.Contains("B2", f);
    }

    [TestMethod]
    public void Cell_StringConversion_IsStringAndValidConversion()
    {
        Cell c = new Cell("Hello World");
        Assert.IsTrue(c.IsStringType());
        Assert.AreEqual("Hello World", (string)c.GetCellData());
    }
    
    [TestMethod]
    public void Cell_DoubleConversion_IsDoubleAndValidConversion()
    {
        Cell c = new Cell(182.88);
        Assert.IsTrue(c.IsDoubleType());
        Assert.AreEqual(182.88, (double)c.GetCellData(), 1e-9);
    }
    
}
