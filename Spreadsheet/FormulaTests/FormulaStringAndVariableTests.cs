namespace FormulaTests;

using Formula;

[TestClass]
public class FormulaStringAndVariableTests
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
    public void FormulaToString_IntegerStringEqualsDoubleString_DoesNotTruncateDecimals()
    {
        Formula f1 = new Formula("1.55");
        Formula f2 = new Formula("1");
        // By "Does Not Truncate Decimals" I Mean To Say That "1.55" Does Not Simply Ignore The Decimal
        // Points When Converting To A String, This Also Makes Sure That Both Integers And Doubles Are Handled
        // Correctly When Converting To Strings
        Assert.IsFalse(f1.ToString().Equals(f2.ToString()));
    }

    [TestMethod]
    public void FormulaToString_SpacesAndAllCapsEqualsAllLowerAndNoSpaces_Valid()
    {
        Formula f1 = new Formula("1 + A1 + B2 * C3");
        Formula f2 = new Formula("1+a1+b2*c3");
        Assert.AreEqual(f1.ToString(), f2.ToString());
    }

    [TestMethod]
    public void FormulaGetVariables_ContainsAllGivenVariables_Valid()
    {
        Formula f1 = new Formula("a1 + a2 + a3");
        Assert.HasCount(3, f1.GetVariables());
    }

    [TestMethod]
    public void FormulaGetVariables_ContainsNoVariablesInScientificNotationOnlyString_Valid()
    {
        Formula f1 = new Formula("1E1+2e2+3e3*4e4");
        Assert.HasCount(0, f1.GetVariables());
    }

    [TestMethod]
    public void FormulaConstructor_CanRecreateFormulaFromCanonicalString_Valid()
    {
        Formula f1 = new Formula("(100) * 1e2 + (40 + A2 + A3) / B2/b3");
        Formula f2 = new Formula(f1.ToString());
        Assert.AreEqual(f1.ToString(), f2.ToString());
    }

}