using Formula;
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
        ss.SetContentsOfCell("A1", "1.0");
        ss.SetContentsOfCell("A2", "2.0");
        ss.SetContentsOfCell("A3", "3.0");
        ss.SetContentsOfCell("A4", "4.0");
        
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
        Assert.Throws<InvalidNameException>(() => ss.SetContentsOfCell("SomeName", "100") );
        Assert.Throws<InvalidNameException>(() => ss.SetContentsOfCell("AnotherName", "Hello World") );
        Assert.Throws<InvalidNameException>(() => ss.SetContentsOfCell("YetAnotherName", "=A1") );
    }

    [TestMethod]
    public void SetCellContents_OverwriteContentsWithDifferentTypes_Valid()
    {
        Spreadsheet ss = new();
        ss.SetContentsOfCell("a1", "100");
        ss.SetContentsOfCell("a1", "one hundred");
        ss.SetContentsOfCell("a1", "=50 * 2");
    }

    [TestMethod]
    public void SetCellContents_SetUniqueCellsOfDifferentTypes_Valid()
    {
        Spreadsheet ss = new();
        ss.SetContentsOfCell("A1", "100");
        ss.SetContentsOfCell("B1", "one hundred");
        ss.SetContentsOfCell("C1", "=200 - 100");
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
        ss.SetContentsOfCell("A1", "100");
        ss.SetContentsOfCell("B1", "=A1*2");
        ss.SetContentsOfCell("C1", "=B1+A1");


        // Then change A1, expect [A1, B1, C1]
        var change = ss.SetContentsOfCell("A1", "10");
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
        ss.SetContentsOfCell("A1", "100");
        ss.SetContentsOfCell("B1", "=A1 + 100");
        ss.SetContentsOfCell("C1", "=A1 + 200");
        ss.SetContentsOfCell("D1", "=C1 + B1");
        ss.SetContentsOfCell("E1", "=D1");
        
        // by changing A1, its expected that the cells should change in this order:
        // [A1, C1, B1, D1, E1]
        // The reason C1 comes before B1 should be because the function "GetCellsToRecalculate"
        // calls ".AddFirst(...)", meaning append to front, and since C1 will be "visited" later
        // that means that C1 will be "added first" after B1
        var change = ss.SetContentsOfCell("A1", "10");
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
        ss.SetContentsOfCell("A1", "=B1");
        Assert.Throws<CircularException>(() => ss.SetContentsOfCell("B1", "=A1"));
    }

    [TestMethod]
    public void SetCellContents_CircularDependencyNotAddedToSS()
    {
        Spreadsheet ss = new();
        ss.SetContentsOfCell("A1", "=B1");
        Assert.Throws<CircularException>(() => ss.SetContentsOfCell("B1", "=A1"));
        foreach (var i in ss.GetNamesOfAllNonemptyCells())
        {
            Console.WriteLine(i);
        }
        Assert.DoesNotContain("B1", ss.GetNamesOfAllNonemptyCells());
    }
    
    // It's not mentioned explicitly whether to allow overwriting previous cells,
    // but given the name "setCellContents", I would expect that to change the cell regardless.
    // That being said, calling ".Add(key, val)" on a dictionary with the key already added throws an exception
    // so basically just make sure nothing throws
    [TestMethod]
    public void SetCellContents_OverwritePreviousValue_Valid()
    {
        Spreadsheet ss = new();
        ss.SetContentsOfCell("A1", "100");
        ss.SetContentsOfCell("A1", "20");
        ss.SetContentsOfCell("B1", "100");
        ss.SetContentsOfCell("B1", "20");
        
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
        Assert.IsTrue(c.IsDoubleType(out _));
        Assert.AreEqual(182.88, (double)c.GetCellData(), 1e-9);
    }
    
    
    // Past This Point Is PS6 Tests

    [TestMethod]
    public void SetContentsOfCell_DoubleFromString_Valid()
    {
        Spreadsheet ss = new();
        ss.SetContentsOfCell("A1", "10.5");
        // We can actually unsafely cast the value to a double, if it throws an
        // exception, then that is the expectation of a cast that should pass
        Assert.AreEqual(10.5, (double)ss.GetCellContents("A1"), 1e-9);
    }
    
    [TestMethod]
    public void SetContentsOfCell_StringFromString_Valid()
    {
        Spreadsheet ss = new();
        ss.SetContentsOfCell("A1", "Hello World");
        // We can actually unsafely cast the value to a double, if it throws an
        // exception, then that is the expectation of a cast that should pass
        Assert.AreEqual("Hello World", (string)ss.GetCellContents("A1"));
    }
    
    [TestMethod]
    public void SetContentsOfCell_FormulaFromString_Valid()
    {
        Spreadsheet ss = new();
        ss.SetContentsOfCell("A1", "= 5.25 + 5.25");
        // We can actually unsafely cast the value to a double, if it throws an
        // exception, then that is the expectation of a cast that should pass
        Assert.AreEqual(10.5, (double)((Formula.Formula)ss.GetCellContents("A1")).Evaluate(x=>0), 1e-9);
    }

    [TestMethod]
    public void SetContentsOfCell_InvalidFormula_FormulaExceptionError()
    {
        Spreadsheet ss = new();
        Assert.Throws<FormulaFormatException>(()=>ss.SetContentsOfCell("A1", "=$$$$"));
    }

    [TestMethod]
    public void SpreadSheetConstructor_FailFileRead_NewEmptySpreadSheet()
    {
        Assert.Throws<SpreadsheetReadWriteException>(() => new Spreadsheet("/File/That/Does/Not/Exist.json"));
    }
    
    [TestMethod]
    public void SpreadsheetConstructor_CreateSSThenReloadSameSS_NoExceptions()
    {
        string saveName = "CreateSSThenReloadSameSS.json";
        // split saving and loading into 2 parts (to resuse var name and explicitly show there is 2 operations
        // in this test)
        {
            Spreadsheet ss = new();
            // try 1 of each type the spreadsheet can store
            ss.SetContentsOfCell("A1", "10.5");
            ss.SetContentsOfCell("B1", "Hello World");
            ss.SetContentsOfCell("C1", "=20*100");
            // save the file under the test that will be used
            ss.Save(saveName);
        }

        {
            Spreadsheet ss = new(saveName);
            Assert.AreEqual(10.5, (double)ss.GetCellContents("A1"), 1e-9);
            Assert.AreEqual("Hello World", (String)ss.GetCellContents("B1"));
            Assert.AreEqual(2_000.0, (double)((Formula.Formula)ss.GetCellContents("C1")).Evaluate(x=>0), 1e-9);
        }
    }

    [TestMethod]
    public void SpreadSheetConstructor_TryLoadInvalidPath_FileException()
    {
        Spreadsheet ss = new();
        Assert.Throws<SpreadsheetReadWriteException>(() => ss.Save("/some/nonsense/path.txt"));
    }

    [TestMethod]
    public void SpreadSheetConstructor_DependencyGraphMatchesOriginalAfterLoad_True()
    {
        // code reused from an earlier test
        Spreadsheet ss = new Spreadsheet();
        ss.SetContentsOfCell("A1", "100");
        ss.SetContentsOfCell("B1", "=A1 + 100");
        ss.SetContentsOfCell("C1", "=A1 + 200");
        ss.SetContentsOfCell("D1", "=C1 + B1");
        ss.SetContentsOfCell("E1", "=D1");
        
        // by changing A1, its expected that the cells should change in this order:
        // [A1, C1, B1, D1, E1]
        // The reason C1 comes before B1 should be because the function "GetCellsToRecalculate"
        // calls ".AddFirst(...)", meaning append to front, and since C1 will be "visited" later
        // that means that C1 will be "added first" after B1
        var change = ss.SetContentsOfCell("A1", "10");
        Assert.AreEqual("A1", change[0]); // self
        Assert.AreEqual("C1", change[1]);
        Assert.AreEqual("B1", change[2]);
        Assert.AreEqual("D1", change[3]);
        Assert.AreEqual("E1", change[4]);

        const string filename = "DependencyGraphSameAfterLoad.json";
        
        ss.Save(filename);

        Spreadsheet ss2 = new(filename);
        var change2 = ss2.SetContentsOfCell("A1", "10");
        Assert.AreEqual("A1", change2[0]); // self
        Assert.AreEqual("C1", change2[1]);
        Assert.AreEqual("B1", change2[2]);
        Assert.AreEqual("D1", change2[3]);
        Assert.AreEqual("E1", change2[4]);
    }

    [TestMethod]
    public void GetCellValues_ValidInputTypes_Valid()
    {
        Spreadsheet ss = new();
        // get all 3 types of values
        ss.SetContentsOfCell("A1", "100");
        ss.SetContentsOfCell("A2", "one hundred");
        ss.SetContentsOfCell("A3", "=50+50");

        Assert.AreEqual(100, (double)ss.GetCellValue("A1"), 1e-9);
        Assert.AreEqual("one hundred", (string)ss.GetCellValue("A2"));
        Assert.AreEqual(100, (double)ss.GetCellValue("A3"), 1e-9);
    }

    [TestMethod]
    public void GetCellValues_ValidInputDependsOnOtherVars_Valid()
    {
        Spreadsheet ss = new();
        ss.SetContentsOfCell("A1", "100");
        ss.SetContentsOfCell("A2", "100");
        ss.SetContentsOfCell("A3", "=A1+A2");
        
        // Index the spreadsheet by using the brackets operator
        Assert.AreEqual(200, (double)ss["A3"], 1e-9);
    }

    [TestMethod]
    public void GetCellValues_DependsOnUndefinedVar_ExceptionThrown()
    {
        Spreadsheet ss = new();
        ss.SetContentsOfCell("A1", "=A2");

        Assert.IsInstanceOfType<FormulaError>(ss["A1"]);
    }

    [TestMethod]
    public void GetCellValues_InvalidName_ExceptionThrow()
    {
        Spreadsheet ss = new();
        Assert.Throws<InvalidNameException>(() => ss["randomcell"]);
    }
    
    [TestMethod]
    public void GetCellValues_SaveInvalidCell_CircularExceptionThrow()
    {
        Spreadsheet ss = new();
        ss.SetContentsOfCell("A1", "=A2");
        ss.Save("TestSaveInvalidCell.json");
    }
    
    
    
}
