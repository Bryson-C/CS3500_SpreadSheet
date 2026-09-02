// <copyright file="FormulaSyntaxTests.cs" company="UofU-CS3500">
//   Copyright 2024 UofU-CS3500. All rights reserved.
// </copyright>
// <authors> [Insert Your Name] </authors>
// <date> [Insert the Date] </date>

namespace FormulaTests;

using CS3500.Formula3; // Change this using statement to use different formula implementations.

/// <summary>
///   <para>
///     The following class shows the basics of how to use the MSTest framework,
///     including:
///   </para>
///   <list type="number">
///     <item> How to catch exceptions. </item>
///     <item> How a test of valid code should look. </item>
///   </list>
/// </summary>
[TestClass]
public class FormulaSyntaxTests
{
    /// <summary>
    ///   <para>
    ///     Make sure constructor accepts a single valid token type
    ///   </para>
    ///   <remarks>
    ///     This test example should not throw an error when given a single valid token.
    ///
    ///     No need to test against this "No Tokens Rule" should cover this case 
    ///   </remarks>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_OneToken_Valid( )
    {
        _ = new Formula( "1" );
    }
    
    /// <summary>
    ///   <para>
    ///     This test makes sure the right kind of exception is thrown
    ///     when trying to create a formula with no tokens.
    ///   </para>
    ///   <remarks>
    ///     <list type="bullet">
    ///       <item>
    ///         We use the _ (discard) notation because the formula object
    ///         is not used after that point in the method.  Note: you can also
    ///         use _ when a method must match an interface but does not use
    ///         some of the required arguments to that method.
    ///       </item>
    ///       <item>
    ///         string.Empty is often considered best practice (rather than using "") because it
    ///         is explicit in intent (e.g., perhaps the coder forgot to but something in "").
    ///       </item>
    ///       <item>
    ///         The name of a test method should follow the MS standard:
    ///         https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices
    ///       </item>
    ///       <item>
    ///         All methods should be documented, but perhaps not to the same extent
    ///         as this one.  The remarks here are for your educational
    ///         purposes (i.e., a developer would assume another developer would know these
    ///         items) and would be superfluous in your code.
    ///       </item>
    ///       <item>
    ///         Notice the use of the attribute tag [ExpectedException] which tells the test
    ///         that the code should throw an exception, and if it doesn't an error has occurred;
    ///         i.e., the correct implementation of the constructor should result
    ///         in this exception being thrown based on the given poorly formed formula.
    ///       </item>
    ///     </list>
    ///   </remarks>
    ///   <example>
    ///     <code>
    ///        // here is how we call the formula constructor with a string representing the formula
    ///        _ = new Formula( "5+5" );
    ///     </code>
    ///   </example>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestNoTokens_Invalid( )
    {
        Assert.Throws<FormulaFormatException>( () => _ = new Formula( "" ) );
        // note: it is arguable that you should replace "" with string.Empty for readability and clarity of intent (e.g., not a cut-and-paste error or a "I forgot to put something there" error).
    }

    /// <summary>
    ///   <para>
    ///     Make sure constructor accepts all valid tokens in a valid configuration
    ///   </para>
    ///   <remarks>
    ///     Given every valid token (in valid configurations) the function should not throw an error
    ///   </remarks>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_ValidTokenRule_Valid( )
    {
        // (, ), +, -, *, /, variables, and numbers
        _ = new Formula( "(1+1) - 8 * A3 / 4.5 + b7" );
    }
    
    /// <summary>
    ///   <para>
    ///     Make sure the valid token rule is tested against invalid tokens such as # or $
    ///   </para>
    ///   <remarks>
    ///     Given a few invalid tokens should throw an error
    ///   </remarks>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_ValidTokenRule_Invalid( )
    {
        Assert.Throws<FormulaFormatException>( () => _ = new Formula( "# $ %" ) );
    }
    
    /// <summary>
    ///   <para>
    ///     Make sure constructor accepts all valid tokens in a valid configuration
    ///   </para>
    ///   <remarks>
    ///     Given every valid token (in valid configurations) the function should not throw an error
    ///   </remarks>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_ClosingParenthesisRule_Valid( )
    {
        // (, ), +, -, *, /, variables, and numbers
        _ = new Formula( "(1+1) - 8 * A3 / 4.5 + b7" );
    }
    
    /// <summary>
    ///   <para>
    ///     Make sure constructor is invalid when the closing parenthesis rule is broken
    ///   </para>
    ///   <remarks>
    ///     Given a simple erroneous test for the closing parenthesis the function should throw an error
    ///   </remarks>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_ClosingParenthesisRule_Invalid( )
    {
        Assert.Throws<FormulaFormatException>( () => _ = new Formula( ")(1)" ) );
    }

    /// <summary>
    ///   <para>
    ///     Make sure the constructor parses a balanced number of parenthesis
    ///   </para>
    ///   <remarks>
    ///     Given a valid set of parenthesis, no error should be thrown
    ///   </remarks>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_BalancedParenthesisRule_Valid( )
    {
        _ = new Formula( "(1 + 1) / (2 + 2)" );
    }
    
    /// <summary>
    ///   <para>
    ///     Make sure the constructor parses a balanced number of parenthesis, an error is expected
    ///   </para>
    ///   <remarks>
    ///     Given an invalid set of parenthesis, an exception should be thrown
    ///   </remarks>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_BalancedParenthesisRule_Invalid( )
    {
        // this case should result in a format exception but does not, leaving here as an example
        // if it were to end in another symbol other than a number it would work as expected.
        // I suspect this is something to do with parsing (or attempting to parse) numbers with decimals
        // (but that's just my theory)
        //Assert.Throws<FormulaFormatException>( () => _ = new Formula( "(1 * 2) * (((1 + 2" ) );
        
        Assert.Throws<FormulaFormatException>( () => _ = new Formula( "(1 * 2) * (((1 + 2 + " ) );

    }

    /// <summary>
    ///   <para>
    ///     The first token of an expression must be a number, a variable, or an opening parenthesis.
    ///   </para>
    ///   <remarks>
    ///     Given a multiple valid formats for the first token rule, the function should not throw an error
    ///     on any of the formulas
    ///   </remarks>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_FirstTokenRule_Valid( )
    { 
        // test multiple cases since there can only be 1 first token and each test should result in the same
        // validity
        _ = new Formula( "10 + 20" );
        _ = new Formula( "A3" );
        _ = new Formula( "4.0" );
        _ = new Formula( "(A3 + A2)" );
    }
    
    /// <summary>
    ///   <para>
    ///     The first token of an expression must be a number, a variable, or an opening parenthesis.
    ///     Test that this fails on each invalid case
    ///   </para>
    ///   <remarks>
    ///     Given a multiple invalid formats for the first token rule, the function should throw an error
    ///     on all of the formulas
    ///   </remarks>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_FirstTokenRule_Invalid( )
    { 
        // test multiple cases since there can only be 1 first token. Each of the cases are invalid
        // additionally make sure each formula has its own assert statement to make testing easier
        Assert.Throws<FormulaFormatException>( () => _ = new Formula( "+ A3" ));
        Assert.Throws<FormulaFormatException>( () => _ = new Formula( "^" ));
        Assert.Throws<FormulaFormatException>( () => _ = new Formula( "token" ));
    }

    /// <summary>
    ///   <para>
    ///     The last token of an expression must be a number, a variable, or a closing parenthesis.
    ///   </para>
    ///   <remarks>
    ///     Given a valid last token formula, no errors should be thrown. Multiple test cases exist in
    ///     this test as they test functionality and only 1 last token can exist per formula.
    ///   </remarks>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_LastTokenRule_Valid( )
    {
        _ = new Formula( "100.01" );
        _ = new Formula( "(A10)" );
        _ = new Formula( "(1000 + 0.09)" );
    }
    
    /// <summary>
    ///   <para>
    ///     The last token of an expression must be a number, a variable, or a closing parenthesis.
    ///   </para>
    ///   <remarks>
    ///     Given an invalid last token formula, an error should be thrown. In this case multiple conditions
    ///     will be given in the same test as they should all throw and only 1 last token can exist
    ///   </remarks>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_LastTokenRule_Invalid( )
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula( "& &" ));
        Assert.Throws<FormulaFormatException>(() => _ = new Formula( "/" ));
        Assert.Throws<FormulaFormatException>(() => _ = new Formula( "100 + (" ));
    }
    
    /// <summary>
    ///   <para>
    ///     Make sure a simple well-formed formula is accepted by the constructor (the constructor
    ///     should not throw an exception).
    ///   </para>
    ///   <remarks>
    ///     This is an example of a test that is not expected to throw an exception, i.e., it succeeds.
    ///     In other words, the formula "1+1" is a valid formula which should not cause any errors.
    ///   </remarks>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestFirstTokenNumber_Valid( )
    {
        _ = new Formula( "1+1" );
    }

    /// <summary>
    ///   <para>
    ///     Any token that immediately follows an opening parenthesis or an operator must be either a number, a variable, or an opening parenthesis.
    ///   </para>
    ///   <remarks>
    ///     This is an example of a test that is not expected to throw an exception
    ///   </remarks>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_ParenthesisOperatorFollowingRule_Valid( )
    {
        _ = new Formula( "( 1 / 8 ) + ( A3 ) + ( ( 8 ) / ( 4 ) )" );
    }

    /// <summary>
    ///   <para>
    ///     Any token that immediately follows an opening parenthesis or an operator must be either a number, a variable, or an opening parenthesis.
    ///   </para>
    ///   <remarks>
    ///     This is an example of a test that is expected to throw an exception
    ///   </remarks>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_ParenthesisOperatorFollowingRule_Invalid( )
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula("( / )"));
    }
    
    /// <summary>
    ///   <para>
    ///     Any token that immediately follows a number, a variable, or a closing parenthesis must be either an operator or a closing parenthesis.
    ///   </para>
    ///   <remarks>
    ///     This is an example of a test that is expected to not throw an exception
    ///   </remarks>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_ExtraFollowingRule_Valid( )
    {
        _ = new Formula("10 / A3 + ( 10 )");
    }
    
    /// <summary>
    ///   <para>
    ///     Any token that immediately follows a number, a variable, or a closing parenthesis must be either an operator or a closing parenthesis.
    ///   </para>
    ///   <remarks>
    ///     This is an example of a test that is expected to throw an exception
    ///   </remarks>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_ExtraFollowingRule_Invalid( )
    {
        Assert.Throws<FormulaFormatException>(() => _ = new Formula("10 10 +"));
    }
    
    
    /// <summary>
    ///   <para>
    ///     Make sure a simple well-formed formula with surrounding valid parenthesis is accepted
    ///     by the constructor (the constructor should not throw an exception).
    ///   </para>
    ///   <remarks>
    ///     This is an example of a test that is not expected to throw an exception, i.e., it succeeds.
    ///     In other words, the formula "(1+1)" is a valid formula which should not cause any errors.
    ///   </remarks>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestParenthesis_Valid( )
    {
        _ = new Formula( "(1+1)" );
    }
}