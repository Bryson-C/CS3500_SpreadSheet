namespace FormulaTests;

using Formula;

[TestClass]
public class FormulaEvaluationTests
{
    // EVERYTHING ABOVE HERE WILL TEST EQUALITY/HASHING

    [TestMethod]
    public void FormulaGetHashCode_TwoDifferentFormulas_UniqueHashes()
    {
        Assert.AreNotEqual(new Formula("A1 + A2").GetHashCode(), new Formula("A2 + A2").GetHashCode());
    }
    
    [TestMethod]
    public void FormulaGetHashCode_TwoSameFormulas_SameHashes()
    {
        Assert.AreEqual(new Formula("A1 + A2").GetHashCode(), new Formula("A1 + A2").GetHashCode());
    }
    
    // If f1.Equals(f2), then it must be the case that f1.GetHashCode() == f2.GetHashCode()
    // But By Definition Of .Equals(...), The Formula Is What Is Being Compared Not The Value,
    // So If 2 Formulas Get The Same Result, But With Different Formulas, They Must Be Different
    [TestMethod]
    public void FormulaGetHashCode_TwoDifferentEqualFormulas_DifferentHashes()
    {
        Formula f1 = new Formula("200 - 100");
        Formula f2 = new Formula("200 / 2");
        // First Assert They Are Equal
        Assert.AreNotEqual(f1, f2);
        Assert.AreNotEqual(f1.GetHashCode(), f2.GetHashCode());
    }
    
    [TestMethod]
    public void FormulaEquals_TwoSameFormulas_AreEqual()
    {
        Assert.AreEqual(new Formula("A1 + A2"), new Formula("A1 + A2"));
    }
    
    [TestMethod]
    public void FormulaEquals_TwoDifferentFormulas_NotEqual()
    {
        Assert.AreNotEqual(new Formula("A1 + A2"), new Formula("A1 + A3"));
    }
    
    [TestMethod]
    public void FormulaEquals_TwoDifferentObjectTypes_NotEqual()
    {
        // Compare A Formula With A String Of The Same Value
        Assert.IsFalse(new Formula("A1 + A2").Equals("A1 + A2"));
    }
    
    [TestMethod]
    public void FormulaNotEqualsOperator_CompareAgainstDifferentFormula_NotEqual()
    {
        // Im Using "IsTrue" Because I Want To Make Sure That It __IS TRUE__ That A1 + A2 __DOES NOT EQUAL__ another formula object
        Assert.IsTrue(new Formula("A1 + A2") != new Formula("A1 + A3"));
    }
    
    [TestMethod]
    public void FormulaNotEquals_CompareAgainstNull_NotEqual()
    {
        // Im Using "IsTrue" Because I Want To Make Sure That It __IS TRUE__ That A1 + A2 __DOES NOT EQUAL__ another formula object
        Assert.IsFalse(new Formula("A1 + A2").Equals(null));
    }
    
    [TestMethod]
    public void FormulaEqualsOperator_CompareAgainstSelf_Equal()
    {
        Formula f1 = new Formula("A1 + A2");
        Formula f2 = new Formula("A1 + A2");
        Assert.IsTrue(f1 == f2);
    }
    
    // EVERYTHING BELOW HERE WILL TEST EVALUATION
    
    [TestMethod]
    public void FormulaEvaluate_SingleValueFormula_Valid()
    {
        Assert.AreEqual(1.0, (double)new Formula("1.0").Evaluate(_ => 0), 1e-9);
    }
    
    [TestMethod]
    public void FormulaEvaluate_SingleVariableFormula_Valid()
    {
        Assert.AreEqual(1.0, (double)new Formula("A1").Evaluate(x =>
        {
            if (x == "A1") return 1.0;
            return 0.0;
        }), 1e-9);
    }
    
    [TestMethod]
    public void FormulaEvaluate_TwoTermFormula_Valid()
    {
        Assert.AreEqual(2.0, (double)new Formula("1.0 + 1.0").Evaluate(_ => 0), 1e-9);
    }
    
    [TestMethod]
    public void FormulaEvaluate_OneVarOneNumberFormula_Valid()
    {
        Assert.AreEqual(2.0, (double)new Formula("A1 + 1.0").Evaluate(x =>
        {
            if (x == "A1") return 1.0;
            return 0.0;
        }), 1e-9);
    }
    
    [TestMethod]
    public void FormulaEvaluate_OnlyNumbersTestAllOperators_Valid()
    {
        Assert.AreEqual(2.0, (double)new Formula("((1*0.25)+(0.75-0.5))/0.25").Evaluate(_ => 0), 1e-9);
    }

    [TestMethod]
    public void FormulaEvaluate_OnlyVariablesTestAllOperators_Valid()
    {
        Lookup l = x =>
        {
            switch (x)
            {
                case "A1": return 1;
                case "A2": return 0.25;
                case "A3": return 0.75;
                case "A4": return 0.5;
                case "A5": return 0.25;
            }
            return 0;
        };
        
        Assert.AreEqual(2.0, (double)new Formula("((A1*A2)+(A3-A4))/A5").Evaluate(l), 1e-9);
    }
    
    [TestMethod]
    public void FormulaEvaluate_TwoDifferentFormulasSameResultDifferentHash_Valid()
    {
        Assert.AreEqual(
            (double)new Formula("20 + 1.0").Evaluate(x =>  0), 
            (double)new Formula("20 + 1.0").Evaluate(x =>  0), 
            1e-9);
    }
    
    [TestMethod]
    public void FormulaEvaluate_DivisionByZero_ShouldReturnError()
    {
        Assert.IsInstanceOfType<FormulaError>(new Formula("100/0").Evaluate(_ => 0));
    }
    
    [TestMethod]
    public void FormulaEvaluate_TryGettingUndefinedVariable_ShouldReturnError()
    {
        Lookup l = (x) =>
        {
            throw new ArgumentException("No Variables Exist");
        };
        Assert.IsInstanceOfType<FormulaError>(new Formula("A1").Evaluate(l));
    }
    
    // These Test Don't Have Much "Usage" Other Than Ensuring More Code Coverage That Wasn't Covered From Other Tests
    // Or In Short, Other Tests Covered The Same Method, But These Are To Double-Check That The Formulas Are Correct In
    // All Scenarios
    
    [TestMethod]
    public void FormulaEvaluate_MultiplyAfterParenthesis_Valid()
    {
        Assert.AreEqual(600_000, (double)new Formula("(100 * 100) * 60 * (100 / 100)").Evaluate(_ => 0), 1e-9);
    }
    
    [TestMethod]
    public void FormulaEvaluate_DivideAfterParenthesis_Valid()
    {
        Assert.AreEqual(0.0166666666667, (double)new Formula("(100 / 100) / 60 / (100 / 100)").Evaluate(_ => 0), 1e-9);
    }
    
    [TestMethod]
    public void FormulaEvaluate_PlusChain_Valid()
    {
        Assert.AreEqual(120, (double)new Formula("20 + 20 + 20 + 20 + A2 + 20").Evaluate(x => 20), 1e-9);
    }
    
    [TestMethod]
    public void FormulaEvaluate_MinusChain_Valid()
    {
        Assert.AreEqual(-80, (double)new Formula("20 - 20 - 20 - 20 - A2 - 20").Evaluate(x => 20), 1e-9);
    }
    
    [TestMethod]
    public void FormulaEvaluate_DivideVarAfterNumber_Valid()
    {
        Assert.AreEqual(1, (double)new Formula("20 / A9").Evaluate(x => 20), 1e-9);
    }
    
    [TestMethod]
    public void FormulaEvaluate_DivideByVarEqualingZero_ReturnsError()
    {
        Assert.IsInstanceOfType<FormulaError>(new Formula("20 / A9").Evaluate(x => 0));
    }
}