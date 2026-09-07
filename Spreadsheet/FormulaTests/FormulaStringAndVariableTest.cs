namespace FormulaTests;

using Formula;

[TestClass]
public class FormulaStringAndVariableTest
{
    [TestMethod]
    public void FormulaToString_OneNumber_Valid()
    {
        Formula f = new Formula("1.0");
        Assert.AreEqual("1", f.ToString()); 
    }

    [TestMethod]
    public void FormulaToString_CanonicalStringEqualsNonCanonicalString_Valid()
    {
        Formula f1 = new Formula("1.0");
        Formula f2 = new Formula("1");
        Assert.IsTrue(f1.ToString().Equals(f2.ToString()));
    }

    [TestMethod]
    public void FormulaToString_CanonicalStringEqualsNonCanonicalString_Invalid()
    {
        Formula f1 = new Formula("1.55");
        Formula f2 = new Formula("1");
        Assert.IsFalse(f1.ToString().Equals(f2.ToString()));
    }

    [TestMethod]
    public void FormulaToString_HasNoSpace_Valid()
    {
        Assert.Fail();
    }
}