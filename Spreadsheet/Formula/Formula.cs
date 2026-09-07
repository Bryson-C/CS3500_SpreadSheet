// <summary>
//   <para>
//     This code is provided to start your assignment.  It was written
//     by Profs Joe, Danny, Jim, and Travis.  You should keep this attribution
//     at the top of your code where you have your header comment, along
//     with any other required information.
//   </para>
//   <para>
//     You should remove/add/adjust comments in your file as appropriate
//     to represent your work and any changes you make.
//   </para>
// </summary>

using System.Text;

namespace Formula;

using System.Text.RegularExpressions;

/// <summary>
///   <para>
///     This class represents formulas written in standard infix notation using standard precedence
///     rules.  The allowed symbols are non-negative numbers written using double-precision
///     floating-point syntax; variables that consist of one or more letters followed by
///     one or more numbers; parentheses; and the four operator symbols +, -, *, and /.
///   </para>
///   <para>
///     Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
///     a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable;
///     and "x 23" consists of a variable "x" and a number "23".  Otherwise, spaces are to be removed.
///   </para>
///   <para>
///     For Assignment Two, you are to implement the following functionality:
///   </para>
///   <list type="bullet">
///     <item>
///        Formula Constructor which checks the syntax of a formula.
///     </item>
///     <item>
///        Get Variables
///     </item>
///     <item>
///        ToString
///     </item>
///   </list>
/// </summary>
public class Formula
{
    /// <summary>
    ///     This Enum Is Used For Classifying The Last Token Read When Iterating Over A Token List.
    ///     The Values Themselves Are Not Important, __BUT__ Their Ranges Are!! The Enum Is Used To Avoid Magic Numbers Inside The Code Base, As Well As Check (In A Single Check If A TokenType Belongs To An Operator For Example) 
    ///     Additionally, It Makes Comparing Logic To Implementation Easier  
    /// </summary>
    private enum TokenType
    {
        // This Is The Starting Case
        Unset,
        // Parenthesis Group
        OpenParen,
        CloseParen,
        // Value Group
        Variable,
        Number,
        // Operator Group
        AddOp,
        SubOp,
        MulOp,
        DivOp,
    }

    /// <summary>
    ///     Checks Whether A Give Token Type Is An Operator Or Not.
    ///     Note: It Is Important That AddOp Comes First And DivOp Comes Last In The Enum __AND__ That All Operator Tokens Are Grouped Together 
    /// </summary>
    /// <param name="type">
    ///     The Token Needing To Be Checked If It Is A Variable Or Not
    /// </param>
    /// <returns>
    ///     Returns True If The Token Type Is Any Of: AddOp, SubOp, MulOp, DivOp
    ///     Otherwise False
    /// </returns>
    bool TokenTypeIsOperator(TokenType type)
    {
        return (type >= TokenType.AddOp && type <= TokenType.DivOp);
    }
    
    
    /// <summary>
    ///   All variables are letters followed by numbers.  This pattern
    ///   represents valid variable name strings.
    /// </summary>
    private const string VariableRegExPattern = @"[a-zA-Z]+\d+";

    /// <summary>
    ///     The formulaVariables List Will Store All Encountered Variables From A Given Formula.
    ///     The Format They Will Be Stored In Will Match What Is Expected In The Formula Canonical Form:
    ///     i.e. x7 = X7, abc50 = ABC50, A5 = A5
    /// </summary>
    private HashSet<string> _formulaVariables;

    /// <summary>
    ///     This String Will Be Built During The Formula Constructor (Because It Will Likely Not Be Changed)
    ///     Additionally, Its Not Worth Rebuilding The String Each Time The User Calls ToString()
    /// </summary>
    private String _canonicalString;
    
    /// <summary>
    ///   Initializes a new instance of the <see cref="Formula"/> class.
    ///   <para>
    ///     Creates a Formula from a string that consists of an infix expression written as
    ///     described in the class comment.  If the expression is syntactically incorrect,
    ///     throws a FormulaFormatException with an explanatory Message.  See the assignment
    ///     specifications for the syntax rules you are to implement.
    ///   </para>
    ///   <para>
    ///     Non-Exhaustive Example Errors:
    ///   </para>
    ///   <list type="bullet">
    ///     <item>
    ///        Invalid variable name, e.g., x, x1x  (Note: x1 is valid, but would be normalized to X1)
    ///     </item>
    ///     <item>
    ///        Empty formula, e.g., string.Empty
    ///     </item>
    ///     <item>
    ///        Mismatched Parentheses, e.g., "(("
    ///     </item>
    ///     <item>
    ///        Invalid Following Rule, e.g., "2x+5"
    ///     </item>
    ///   </list>
    /// </summary>
    /// <param name="formula"> The string representation of the formula to be created.</param>
    public Formula( string formula )
    {
        // Rule 1 Handler: There Must Be At Least 1 Token
        if (formula.Length <= 0)
        {
            throw new FormulaFormatException("Formula Must Not Be Empty");
        }
        
        List<String> tokens = GetTokens(formula);
        
        // Rule 5 Handler: The First Token Must Be A Number, Variable, Or Open Parenthesis
        // Note, At The Moment There Is No Need To Save The Double From "TryParse" From Either Of The Following Statements As The Value Won't Be Used
        if (!Double.TryParse(tokens.First(), out double _) && !IsVar(tokens.First()) && tokens.First() != "(")
        {
            throw new FormulaFormatException("Formula Must Start With A Number, A Variable, Or An Open Parenthesis");
        } 
        
        // Rule 6 Handler: The Last Token Must Be A Number, Variable, Or Closing Parenthesis
        if (!Double.TryParse(tokens.Last(), out double _) && !IsVar(tokens.Last()) && tokens.Last() != ")")
        {
            throw new FormulaFormatException("Formula Must End With A Number, A Variable, Or An Open Parenthesis");
        }

        // From Here On Out, The Rules Will Involve State
        
        // Here We Initialize The formulaVariables Variable So That We Are Not Trying To Access A Null Reference/Pointer
        _formulaVariables = new HashSet<string>();
        
        // Canonical String Builder, Once Done Iterating Over The Tokens, We Can Convert To A String And Assign _canonicalString To Its Value
        StringBuilder canStrBuilder = new StringBuilder();
        
        // paren = The Amount Of Total Parenthesis We Encounter, '(' = +1, ')' = -1
        //      If paren == 0, Then The Formula Is Balanced, If It Ever Reaches Negatives, Then We Know It Violates The Closing Parenthesis Rule (Rule 3)
        int paren = 0;
        
        // curTokenType = The Token That Is Currently Being Processed, The Purpose Of Its Existence Is To Ensure That Following Rules Can Be Checked Efficiently
        //      And lastTokenType Is Assigned To curTokenType At The End Of Each Loop
        TokenType curTokenType;
        
        // lastTokenType = The Last Token's Type That Was Encountered. There Is No Need To Handle Cases Where The Loop Encounters An Invalid Type As The Constructor Should Give An Error
        //      lastTokenType Will Be Set At The End Of Each Iteration Of The Tokens Loop To The Value Of curTokenType
        TokenType lastTokenType = TokenType.Unset;
        
        foreach (string token in tokens)
        {
            if (token == "(")
            {
                paren++;
                canStrBuilder.Append("(");
                curTokenType = TokenType.OpenParen;
            }
            else if (token == ")")
            {
                paren--;
                canStrBuilder.Append(")");
                curTokenType = TokenType.CloseParen;
                // Rule 3 Handler: Because Of The Nature Of Rule 3, At __Any__ Point If There Is More Closing Parenthesis, We Need To Give An Error
                if (paren < 0)
                {
                    throw new FormulaFormatException("Formula Must Never Have More Closing Parenthesis Than Opening Parenthesis");
                }
            }
            else if (token == "+") 
            {
                canStrBuilder.Append("+");
                curTokenType = TokenType.AddOp;
            }
            else if (token == "-") 
            {
                canStrBuilder.Append("-");
                curTokenType = TokenType.SubOp;
            }
            else if (token == "*") 
            {
                canStrBuilder.Append("*");   
                curTokenType = TokenType.MulOp;
            }
            else if (token == "/") 
            {
                canStrBuilder.Append("/");
                curTokenType = TokenType.DivOp;
            }
            else if (Double.TryParse(token, out double number))
            {
                // number Should Be In The Correct Form As We Expect "TryParse" To Have The Correct Implementation Of The Number's Canonical Form
                canStrBuilder.Append(number);
                curTokenType = TokenType.Number;
            }
            else if (IsVar(token))
            {
                // Since "IsVar(...)" Is Expected To Work, All We Need To Handle Here Is Turning The Variable Into Its Canonical Form (i.e. Uppercased Letters Followed By Numbers)
                _formulaVariables.Add(token.ToUpper());
                canStrBuilder.Append(token.ToUpper());
                curTokenType = TokenType.Variable;
            }
            // Rule 2 Handler: In This Case, We Cannot Figure Out What The Token Type Is, So We Must Throw An Error Stating There Is An Invalid Token
            else
            {
                throw new FormulaFormatException("Formula Must Only Contain Tokens: (, ), +, -, *, /, Variables, And Numbers"); 
            }

            // Check For Incompatibilities Of Following Tokens (i.e. curTokenType Following lastTokenType)
            
            // Rule 7 Handler: Token Types Following An Open Parenthesis Or Operator Must Be A Number, Variable, Or Open Parenthesis
            // Note: This Checks To See If The Condition Above Can Pass, And Inverts It So When Its False It Can Throw An Error
            if (
                (lastTokenType == TokenType.OpenParen || TokenTypeIsOperator(lastTokenType)) && 
                !(curTokenType == TokenType.OpenParen || curTokenType == TokenType.Number || curTokenType == TokenType.Variable))
            {
                throw new FormulaFormatException("A Token Following An Open Parenthesis Or Operator Must Be A Number, Variable, Or Opening Parenthesis");
            }
            // Rule 8 Handler: Tokens Following A Number, Variable, Or Closing Parenthesis Must Be An Operator Or Closing Parenthesis
            if (
                (lastTokenType == TokenType.Number || lastTokenType == TokenType.Variable || lastTokenType == TokenType.CloseParen) && 
                !(TokenTypeIsOperator(curTokenType) || curTokenType == TokenType.CloseParen))
            {
                throw new FormulaFormatException("A Token Following A Number, Variable, Or Closing Parenthesis Must be An Operator Or Closing Parenthesis");
            }
            
            lastTokenType = curTokenType;
        }

        // Rule 4 Handler: Make Sure The Parenthesis Are Balanced (Explained Above)
        if (paren != 0)
        {
            throw new FormulaFormatException("Opening And Closing Parenthesis Must Balanced");
        }

        _canonicalString = canStrBuilder.ToString();
    }

    /// <summary>
    ///   <para>
    ///     Returns a set of all the variables in the formula.
    ///   </para>
    ///   <remarks>
    ///     Important: no variable may appear more than once in the returned set, even
    ///     if it is used more than once in the Formula.
	///     Variables should be returned in canonical form, having all letters converted
	///     to uppercase.
    ///   </remarks>
    ///   <list type="bullet">
    ///     <item>new("x1+y1*z1").GetVariables() should return a set containing "X1", "Y1", and "Z1".</item>
    ///     <item>new("x1+X1"   ).GetVariables() should return a set containing "X1".</item>
    ///   </list>
    /// </summary>
    /// <returns> the set of variables (string names) representing the variables referenced by the formula. </returns>
    public ISet<string> GetVariables( )
    {
        // This Will Simply Return The Variables That Were Read Upon Creation Of The Formula Object Via The Constructor
        return _formulaVariables;
    }

    /// <summary>
    ///   <para>
    ///     Returns a string representation of a canonical form of the formula.
    ///   </para>
    ///   <para>
    ///     The string will contain no spaces.
    ///   </para>
    ///   <para>
    ///     If the string is passed to the Formula constructor, the new Formula f
    ///     will be such that this.ToString() == f.ToString().
    ///   </para>
    ///   <para>
    ///     All the variable and number tokens in the string will be normalized.
    ///     For numbers, this means that the original string token is converted to
    ///     a number using double.Parse or double.TryParse, then converted back to a
    ///     string using double.ToString.
    ///     For variables, this means all letters are uppercased.
    ///   </para>
    ///   <para>
    ///       For example:
    ///   </para>
    ///   <code>
    ///       new("x1 + Y1").ToString() should return "X1+Y1"
    ///       new("x1 + 5.0000").ToString() should return "X1+5".
    ///   </code>
    ///   <para>
    ///     This method should execute in O(1) time.
    ///   </para>
    /// </summary>
    /// <returns>
    ///   A canonical version (string) of the formula. All "equal" formulas
    ///   should have the same value here.
    /// </returns>
    public override string ToString( )
    {
        return _canonicalString;
    }

    /// <summary>
    ///   Reports whether "token" is a variable.  It must be one or more letters
    ///   followed by one or more numbers.
    /// </summary>
    /// <param name="token"> A token that may be a variable. </param>
    /// <returns> true if the string matches the requirements, e.g., A1 or a1. </returns>
    private static bool IsVar( string token )
    {
        // notice the use of ^ and $ to denote that the entire string being matched is just the variable
        string standaloneVarPattern = $"^{VariableRegExPattern}$";
        return Regex.IsMatch( token, standaloneVarPattern );
    }

    /// <summary>
    ///   <para>
    ///     Given an expression, enumerates the tokens that compose it.
    ///   </para>
    ///   <para>
    ///     Tokens returned are:
    ///   </para>
    ///   <list type="bullet">
    ///     <item>left paren</item>
    ///     <item>right paren</item>
    ///     <item>one of the four operator symbols</item>
    ///     <item>a string consisting of one or more letters followed by one or more numbers</item>
    ///     <item>a double literal</item>
    ///     <item>and anything that doesn't match one of the above patterns</item>
    ///   </list>
    ///   <para>
    ///     There are no empty tokens; white space is ignored (except to separate other tokens).
    ///   </para>
    /// </summary>
    /// <param name="formula"> A string representing an infix formula such as 1*B1/3.0. </param>
    /// <returns> The ordered list of tokens in the formula. </returns>
    private static List<string> GetTokens( string formula )
    {
        List<string> results = [];

        string lpPattern = @"\(";
        string rpPattern = @"\)";
        string opPattern = @"[\+\-*/]";
        string doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
        string spacePattern = @"\s+";

        // Overall pattern
        string pattern = string.Format(
                                        "({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                        lpPattern,
                                        rpPattern,
                                        opPattern,
                                        VariableRegExPattern,
                                        doublePattern,
                                        spacePattern);

        // Enumerate matching tokens that don't consist solely of white space.
        foreach ( string s in Regex.Split( formula, pattern, RegexOptions.IgnorePatternWhitespace ) )
        {
            if ( !Regex.IsMatch( s, @"^\s*$", RegexOptions.Singleline ) )
            {
                results.Add(s);
            }
        }

        return results;
    }
}


/// <summary>
///   Used to report syntax errors in the argument to the Formula constructor.
/// </summary>
public class FormulaFormatException : Exception
{
    /// <summary>
    ///   Initializes a new instance of the <see cref="FormulaFormatException"/> class.
    ///   <para>
    ///      Constructs a FormulaFormatException containing the explanatory message.
    ///   </para>
    /// </summary>
    /// <param name="message"> A developer defined message describing why the exception occured.</param>
    public FormulaFormatException( string message )
        : base( message )
    {
        // All this does is call the base constructor. No extra code needed.
    }
}
