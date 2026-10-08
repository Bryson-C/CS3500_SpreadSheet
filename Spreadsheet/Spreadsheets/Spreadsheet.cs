// <copyright file="Spreadsheet.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>

// Written by Joe Zachary for CS 3500, September 2013
// Update by Profs Kopta, de St. Germain, Martin, Fall 2021, Fall 2024, Fall 2025
//     - Updated return types
//     - Updated documentation

using System.Text.RegularExpressions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Spreadsheets;

using Formula;
using DependencyGraph;

/// <summary>
///   <para>
///     Thrown to indicate that a change to a cell will cause a circular dependency.
///   </para>
/// </summary>
public class CircularException : Exception
{
}

/// <summary>
///   <para>
///     Thrown to indicate that a name parameter was invalid.
///   </para>
/// </summary>
public class InvalidNameException : Exception
{
}


/// <summary>
/// <para>
///   Thrown to indicate that a read or write attempt has failed with
///   an expected error message informing the user of what went wrong.
/// </para>
/// </summary>
public class SpreadsheetReadWriteException : Exception
{
    /// <summary>
    ///   <para>
    ///     Creates the exception with a message defining what went wrong.
    ///   </para>
    /// </summary>
    /// <param name="msg"> An informative message to the user. </param>
    public SpreadsheetReadWriteException( string msg )
        : base( msg )
    {
    }
}

/// <summary>
///     A class to store data of a cell from a spreadsheet.
///     Can be given a double, string, or formula.
///     Is not responsible for who it depends on, it will only store the data of a single cell
/// </summary>
public class Cell
{
    /// <summary>
    ///     This variable can only be a double, string, or Formula
    ///     The value must be valid
    ///     and therefore should also be able to be fearlessly converted to the
    ///     type stored in "StringForm"/given by the constructors (with the exception of removing the '=' in formula string forms)
    /// </summary>
    [JsonInclude]
    public string StringForm { get; private set; }

    /// <summary>
    ///     Sets the _cellData object to a double, and the ValueType to CellType.Double
    /// </summary>
    /// <param name="number"> the value desired for the cell to have stored </param>
    public Cell(double number)
    {
        StringForm = number.ToString();
    }

    /// <summary>
    /// Default Constructor To Allow For Serialization
    /// </summary>
    public Cell()
    {
        
    }

    /// <summary>
    ///     Sets the _cellData object to a string, and the ValueType to CellType.String
    /// </summary>
    /// <param name="str"> the string desired for the cell to have stored </param>
    public Cell(string str)
    {
        StringForm = str;
    }

    /// <summary>
    ///     Sets the _cellData object to a formula, and the ValueType to CellType.Formula
    ///     The formula must be a valid formula
    /// </summary>
    /// <param name="formula"> the formula desired for the cell to have stored </param>
    public Cell(Formula formula)
    {
        StringForm = "="+formula;
    }

    /// <summary>
    /// Will return true if the cell was initially set as a double type, otherwise false
    /// If true, then the value of the cell should be safe to convert to said type
    /// </summary>
    public bool IsDoubleType(out double result)
    {
        return Double.TryParse(StringForm, out result);
    }

    /// <summary>
    /// Will return true if the cell was initially set as a double type, otherwise false
    /// If true, then the value of the cell should be safe to convert to said type
    /// </summary>
    public bool IsStringType()
    {
        return !IsDoubleType(out _) && StringForm.First() != '=';
    }

    /// <summary>
    /// Will return true if the cell was initially set as a double type, otherwise false
    /// If true, then the value of the cell should be safe to convert to said type
    /// </summary>
    public bool IsFormulaType()
    {
        return StringForm.First() == '=';
    }

    /// <summary>
    ///     Returns the data stored in CellData, after checking the type
    ///     (<see cref="IsDoubleType"/>, <see cref="IsStringType"/>, <see cref="IsFormulaType"/>)
    ///     it should be safe to convert to said type
    ///
    ///     Since the constructors only allow the actual datatypes (not just their string representations)
    ///     this should mean that its safe to convert back to one of said datatypes
    /// </summary>
    /// <returns> Returns the data stored in CellData </returns>
    public object GetCellData()
    {
        if (IsDoubleType(out double result))
        {
            return result;
        } else if (IsStringType())
        {
            return StringForm;
        }
        // If it's not either of the above, then it has to be a formula
        return new Formula(StringForm.Substring(1, StringForm.Length-1));
    }
    
}


/// <summary>
///   <para>
///     A Spreadsheet object represents the state of a simple spreadsheet.  A
///     spreadsheet represents an infinite number of named cells.
///   </para>
/// <para>
///     Valid Cell Names: A string is a valid cell name if and only if it is one or
///     more letters followed by one or more numbers, e.g., A5, BC27.
/// </para>
/// <para>
///    Cell names are case-insensitive, so "x1" and "X1" are the same cell name.
///    Your code should normalize (uppercased) any stored name but accept either.
/// </para>
/// <para>
///     A spreadsheet represents a cell corresponding to every possible cell name.  (This
///     means that a spreadsheet contains an infinite number of cells.)  In addition to
///     a name, each cell has a contents and a value.  The distinction is important.
/// </para>
/// <para>
///     The <b>contents</b> of a cell can be (1) a string, (2) a double, or (3) a Formula.
///     If the contents of a cell is set to the empty string, the cell is considered empty.
/// </para>
/// <para>
///     By analogy, the contents of a cell in Excel is what is displayed on
///     the editing line when the cell is selected.
/// </para>
/// <para>
///     In a new spreadsheet, the contents of every cell is the empty string. Note:
///     this is by definition (it is IMPLIED, not stored).
/// </para>
/// <para>
///     The <b>value</b> of a cell can be (1) a string, (2) a double, or (3) a FormulaError.
///     (By analogy, the value of an Excel cell is what is displayed in that cell's position
///     in the grid.) We are not concerned with cell values yet, only with their contents,
///     but for context:
/// </para>
/// <list type="number">
///   <item>If a cell's contents is a string, its value is that string.</item>
///   <item>If a cell's contents is a double, its value is that double.</item>
///   <item>
///     <para>
///       If a cell's contents is a Formula, its value is either a double or a FormulaError,
///       as reported by the Evaluate method of the Formula class.  For this assignment,
///       you are not dealing with values yet.
///     </para>
///   </item>
/// </list>
/// <para>
///     Spreadsheets are never allowed to contain a combination of Formulas that establish
///     a circular dependency.  A circular dependency exists when a cell depends on itself,
///     either directly or indirectly.
///     For example, suppose that A1 contains B1*2, B1 contains C1*2, and C1 contains A1*2.
///     A1 depends on B1, which depends on C1, which depends on A1.  That's a circular
///     dependency.
/// </para>
/// </summary>
public class Spreadsheet
{

    /// <summary>
    ///     Default constructor allowing the spreadsheet  to be serialized
    /// </summary>
    public Spreadsheet()
    {
        
    }
    
    /// <summary>
    /// This Will Store All The Non-Empty Cells As Well Their Dependents And Dependees
    /// </summary>
    private DependencyGraph _dependencyGraph = new();

    /// <summary>
    ///     The cells which are stored along with their position (i.e. "A1")
    ///     Note: Not responsible for maintaining the dependency graph, only the contents at a cell's location
    ///
    ///     Note: Currently, as asked by PS5 " You should define an appropriate Cell class", and as such, for
    ///         future proofing, I will use "Cell" rather than "object" as the type
    /// </summary>
    [JsonPropertyName("Cells"), JsonInclude]
    private Dictionary<string, Cell> _contentCells = new();
    
    /// <summary>
    ///   Provides a copy of the normalized names of all the cells in the spreadsheet
    ///   that contain information (i.e., non-empty cells).
    /// </summary>
    /// <returns>
    ///   A set of the names of all the non-empty cells in the spreadsheet.
    /// </returns>
    public ISet<string> GetNamesOfAllNonemptyCells()
    {
        return new HashSet<string>(_contentCells.Keys);
    }

    /// <summary>
    ///   Returns the contents (as opposed to the value) of the named cell.
    /// </summary>
    ///
    /// <exception cref="InvalidNameException">
    ///   Thrown if the name is invalid.
    /// </exception>
    ///
    /// <param name="name">The name of the spreadsheet cell to query. </param>
    /// <returns>
    ///   The contents as either a string, a double, or a Formula.
    ///   See the class header summary.
    /// </returns>
    public object GetCellContents(string name)
    {
        // Here we dont need to check if the name is invalid, as there will never
        // be a named value pushed to the "_contentCells" map which is invalid (because its checked before
        // it is set in "SetCellContents")
        if (!_contentCells.ContainsKey(name))
        {
            throw new InvalidNameException();
        }
        return _contentCells[name].GetCellData();
    }

    /// <summary>
    ///     This matches the "IsVar" from "Formula.cs" which means anything that passes this
    ///     check should be a valid name i.e. 1 or more letters and 1 or more numbers:
    ///         A1, BC17, ...
    /// </summary>
    /// <param name="str">The string to check whether it's a valid name</param>
    /// <returns> Returns true if the passed in string matches the pattern of a valid name/variable </returns>
    private bool IsValidVarName(string str)
    {
        return Regex.IsMatch(str, $"^{@"[a-zA-Z]+\d+"}$" );    
    } 
    
    /// <summary>
    ///  Set the contents of the named cell to the given number.
    /// </summary>
    ///
    /// <exception cref="InvalidNameException">
    ///   If the name is invalid, throw an InvalidNameException.
    /// </exception>
    ///
    /// <param name="name"> The name of the cell. </param>
    /// <param name="number"> The new contents of the cell. </param>
    /// <returns>
    ///   <para>
    ///     This method returns an ordered list consisting of the passed in name
    ///     followed by the names of all other cells whose value depends, directly
    ///     or indirectly, on the named cell.
    ///   </para>
    ///   <para>
    ///     The order must correspond to a valid dependency ordering for recomputing
    ///     all the cells, i.e., if you re-evaluate each cell in the order of the list,
    ///     the overall spreadsheet will be correctly updated.
    ///   </para>
    ///   <para>
    ///     For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
    ///     list [A1, B1, C1] is returned, i.e., A1 was changed, so then A1 must be
    ///     evaluated, followed by B1, followed by C1.
    ///   </para>
    /// </returns>
    private IList<string> SetCellContents(string name, double number)
    {
        // since only numbers don't have dependencies, don't worry about the dependency graph
        if (_contentCells.ContainsKey(name))
        {
            _contentCells[name] = new Cell(number);
        } 
        else
        {
            _contentCells.Add(name, new Cell(number));
        }
        // by now we know that the cell must've changed
        Changed = true;
        return GetCellsToRecalculate(name).ToList();
    }

    /// <summary>
    ///   The contents of the named cell becomes the given text.
    /// </summary>
    ///
    /// <exception cref="InvalidNameException">
    ///   If the name is invalid, throw an InvalidNameException.
    /// </exception>
    /// <param name="name"> The name of the cell. </param>
    /// <param name="text"> The new contents of the cell. </param>
    /// <returns>
    ///   The same list as defined in <see cref="SetCellContents(string, double)"/>.
    /// </returns>
    private IList<string> SetCellContents(string name, string text)
    {
        // since only text (not to be confused with formulas textual representation)
        // don't have dependencies, don't worry about the dependency graph
        if (_contentCells.ContainsKey(name))
        {
            _contentCells[name] = new Cell(text);
        } 
        else
        {
            _contentCells.Add(name, new Cell(text));
        }  
        // by now we know that the cell must've changed
        Changed = true;
        return GetCellsToRecalculate(name).ToList();
    }

    /// <summary>
    ///   Set the contents of the named cell to the given formula.
    /// </summary>
    /// <exception cref="InvalidNameException">
    ///   If the name is invalid, throw an InvalidNameException.
    /// </exception>
    /// <exception cref="CircularException">
    ///   <para>
    ///     If changing the contents of the named cell to be the formula would
    ///     cause a circular dependency, throw a CircularException, and no
    ///     change is made to the spreadsheet.
    ///   </para>
    /// </exception>
    /// <param name="name"> The name of the cell. </param>
    /// <param name="formula"> The new contents of the cell. </param>
    /// <returns>
    ///   The same list as defined in <see cref="SetCellContents(string, double)"/>.
    /// </returns>
    private IList<string> SetCellContents(string name, Formula formula)
    {
        if (_contentCells.ContainsKey(name))
        {
            _contentCells[name] = new Cell(formula);
        }
        else
        {
            _contentCells.Add(name, new Cell(formula));
        }
        
        // add all the variables included in the formula as a dependency for the dependency graph
        foreach (var v in formula.GetVariables())
        {
            _dependencyGraph.AddDependency(v, name);
        }
        try
        {
            return GetCellsToRecalculate(name).ToList();
        }
        catch (CircularException e)
        {
            foreach (var v in formula.GetVariables())
            {
                _dependencyGraph.RemoveDependency(v,name);
            }
            _contentCells.Remove(name);

            throw;
        }
    }

    /// <summary>
    ///   Returns an enumeration, without duplicates, of the names of all cells whose
    ///   values depend directly on the value of the named cell.
    /// </summary>
    /// <param name="name"> This <b>MUST</b> be a valid name.  </param>
    /// <returns>
    ///   <para>
    ///     Returns an enumeration, without duplicates, of the names of all cells
    ///     that contain formulas containing name.
    ///   </para>
    ///   <para>For example, suppose that: </para>
    ///   <list type="bullet">
    ///      <item>A1 contains 3</item>
    ///      <item>B1 contains the formula A1 * A1</item>
    ///      <item>C1 contains the formula B1 + A1</item>
    ///      <item>D1 contains the formula B1 - C1</item>
    ///   </list>
    ///   <para> The direct dependents of A1 are B1 and C1. </para>
    /// </returns>
    private IEnumerable<string> GetDirectDependents(string name)
    {
        return _dependencyGraph.GetDependents(name);
    }

    /// <summary>
    ///   <para>
    ///     This method is implemented for you, but makes use of your GetDirectDependents.
    ///   </para>
    ///   <para>
    ///     Returns an enumeration of the names of all cells whose values must
    ///     be recalculated, assuming that the contents of the cell referred
    ///     to by name has changed.  The cell names are enumerated in an order
    ///     in which the calculations should be done.
    ///   </para>
    ///   <exception cref="CircularException">
    ///     If the cell referred to by name is involved in a circular dependency,
    ///     throws a CircularException.
    ///   </exception>
    ///   <para>
    ///     For example, suppose that:
    ///   </para>
    ///   <list type="number">
    ///     <item>
    ///       A1 contains 5
    ///     </item>
    ///     <item>
    ///       B1 contains the formula A1 + 2.
    ///     </item>
    ///     <item>
    ///       C1 contains the formula A1 + B1.
    ///     </item>
    ///     <item>
    ///       D1 contains the formula A1 * 7.
    ///     </item>
    ///     <item>
    ///       E1 contains 15
    ///     </item>
    ///   </list>
    ///   <para>
    ///     If A1 has changed, then A1, B1, C1, and D1 must be recalculated,
    ///     and they must be recalculated in an order which has A1 first, and B1 before C1
    ///     (there are multiple such valid orders).
    ///     The method will produce one of those enumerations.
    ///   </para>
    ///   <para>
    ///      PLEASE NOTE THAT THIS METHOD DEPENDS ON THE METHOD GetDirectDependents.
    ///      IT WON'T WORK UNTIL GetDirectDependents IS IMPLEMENTED CORRECTLY.
    ///   </para>
    /// </summary>
    /// <param name="name"> The name of the cell.  Requires that name be a valid cell name.</param>
    /// <returns>
    ///    Returns an enumeration of the names of all cells whose values must
    ///    be recalculated.
    /// </returns>
    private IEnumerable<string> GetCellsToRecalculate(string name)
    {
        LinkedList<string> changed = new();
        HashSet<string> visited = [];
        Visit(name, name, visited, changed);
        return changed;
    }

    /// <summary>
    ///   A helper for the GetCellsToRecalculate method.
    ///   From a starting cell name, recursively visit each of its neighbors and their neighbors.
    /// </summary>
    ///
    /// <param name="start">
    ///     The Node to start with
    ///     (typically will be the same as "name", when calling at the "top level")
    ///     If start is encountered as a neighbor of other nodes, then a CircularException will be thrown
    /// </param>
    /// <param name="name">
    ///     The node currently being visited
    ///     (typically will be the same as "start", when calling at the "top level")
    /// </param>
    /// <param name="visited">
    ///     A set which tells the program what nodes to not revisit
    /// </param>
    /// <param name="changed">
    ///     A list which will be modified to have a list dependencies from the "name" node
    ///     (typically will also start with "start").
    ///     Will return a list sorted as such: [ this, neighbor, neighbor's neighbor, ..., last dependency ] 
    /// </param>
    /// <exception cref="CircularException">
    ///   If the start node is reached as a neighbor (whether adjacent, or further) then throw a circular exception
    /// </exception>
    private void Visit(string start, string name, ISet<string> visited, LinkedList<string> changed)
    {
        // Mark the current cell being visited as "visited" so the program doesn't get stuck/revisit dependents
        visited.Add(name);
        // get all neighbors of the current cell (breadth first style if im not mistaken)
        foreach (string n in GetDirectDependents(name))
        {
            // if the current neighbor is the same as where we asked the "visit" to start, we've encountered
            // a circular dependency and should throw an error
            if (n.Equals(start))
            {
                throw new CircularException();
            }
            // if this neighbor has not been visited, then we recursively visit it, and repeat the process
            else if (!visited.Contains(n))
            {
                Visit(start, n, visited, changed);
            }
        }
        
        // Push the name of the node which is visited to the front of the list,
        // since every visited node will be the new first (after all its neighbors),
        // we are essentially making a list which is sorted:
        // [ this, neighbor, neighbor's neighbor, ..., last dependency ] 
        changed.AddFirst(name);
    }
    
    
    
    /// <summary>
    ///   <para>
    ///     Return the value of the named cell, as defined by
    ///     <see cref="GetCellValue(string)"/>.
    ///   </para>
    /// </summary>
    /// <param name="name"> The cell in question. </param>
    /// <returns>
    ///   <see cref="GetCellValue(string)"/>
    /// </returns>
    /// <exception cref="InvalidNameException">
    ///   If the provided name is invalid, throws an InvalidNameException.
    /// </exception>
    public object this[string name]
    {
        get { return GetCellValue(name); }
    }


	  /// <summary>
    /// True if this spreadsheet has been changed since it was
    /// created or saved (whichever happened most recently),
    /// False otherwise.
    /// </summary>
    [JsonIgnore]
    public bool Changed { get; private set; }


	  /// <summary>
    /// Constructs a spreadsheet using the saved data in the file referred to by
    /// the given filename.
    /// <see cref="Save(string)"/>
    /// </summary>
    /// <exception cref="SpreadsheetReadWriteException">
    ///   Thrown if the file can not be loaded into a spreadsheet for any reason
    /// </exception>
    /// <param name="filename">The path to the file containing the spreadsheet to load</param>
    public Spreadsheet(string filename)
    {
        try
        {
            string jsonData = File.ReadAllText(filename);
            Spreadsheet? ss = JsonSerializer.Deserialize<Spreadsheet>(jsonData);
            // Since this is in a try block, if any of the following fail, then it will also
            // get caught below, so it should be "safe" to simply use these
            foreach (KeyValuePair<string, Cell> cell in ss._contentCells)
            {
                SetContentsOfCell(cell.Key, cell.Value.StringForm);
            }
        }
        catch (Exception e)
        {
            // SpreadsheetReadWriteException is the catch all exception, catch and throw it as the new exception with
            // the old message still attached
            throw new SpreadsheetReadWriteException("Failed Reading Spreadsheet Saved File: " + e.Message);
        }
    }
      
    /// <summary>
    /// Saves this spreadsheet to a file
    /// </summary>
    /// <param name="filename"> The name (with path) of the file to save to.</param>
    /// <exception cref="SpreadsheetReadWriteException">
    ///   If there are any problems opening, writing, or closing the file,
    ///   the method should throw a SpreadsheetReadWriteException with an
    ///   explanatory message.
    /// </exception>
    public void Save( string filename )
    {
        try
        {
            
            string s = JsonSerializer.Serialize(this);
            File.WriteAllText(filename, s);
        }
        catch (Exception e)
        {
            // Catch all for any exceptions that may occur, throw SpreadsheetReadWriteException
            // with original exception attached to the message
            throw new SpreadsheetReadWriteException("Failed Writing Spread Sheet To File: " + e.Message);
        }
    }

    /// <summary>
    ///   <para>
    ///     Return the value of the named cell.
    ///   </para>
    /// </summary>
    /// <param name="name"> The cell in question. </param>
    /// <returns>
    ///   Returns the value (as opposed to the contents) of the named cell.  The return
    ///   value should be either a string, a double, or a CS3500.Formula.FormulaError.
    /// </returns>
    /// <exception cref="InvalidNameException">
    ///   If the provided name is invalid, throws an InvalidNameException.
    /// </exception>
    public object GetCellValue( string name )
    {
        if (!IsValidVarName(name))
        {
            throw new InvalidNameException();
        }

        if (_contentCells[name].IsDoubleType(out double result))
        {
            return result;
        }
        else if (_contentCells[name].IsStringType())
        {
            return _contentCells[name].StringForm;
        }
        // if not the other two, it must be a formula
        try
        {
            // because of the nature of the lookup, it will throw a "KeyNotFoundException"
            // when a value doesnt exist under "str", in this case, that means we can change it to a formula exception
            // and throw that with details of the "parent exception"
            return ((Formula)_contentCells[name].GetCellData()).Evaluate((str) => (double)GetCellValue(str));
        }
        catch (Exception _) {}
        // the only case where this would be called is when an exception is caught.
        // So the only valid value we could have is a formula error
        return new FormulaError("Failed Evaluating Formula '" + _contentCells[name] + "'");
    }

    /// <summary>
    ///   <para>
    ///     Set the contents of the named cell to be the provided string
    ///     which will either represent (1) a string, (2) a number, or
    ///     (3) a formula (based on the prepended '=' character).
    ///   </para>
    ///   <para>
    ///     Rules of parsing the input string:
    ///   </para>
    ///   <list type="bullet">
    ///     <item>
    ///       <para>
    ///         If 'content' parses as a double, the contents of the named
    ///         cell becomes that double.
    ///       </para>
    ///     </item>
    ///     <item>
    ///         If the string does not begin with an '=', the contents of the
    ///         named cell becomes 'content'.
    ///     </item>
    ///     <item>
    ///       <para>
    ///         If 'content' begins with the character '=', an attempt is made
    ///         to parse the remainder of content into a Formula f using the Formula
    ///         constructor.  There are then three possibilities:
    ///       </para>
    ///       <list type="number">
    ///         <item>
    ///           If the remainder of content cannot be parsed into a Formula, a
    ///           CS3500.Formula.FormulaFormatException is thrown.
    ///         </item>
    ///         <item>
    ///           Otherwise, if changing the contents of the named cell to be f
    ///           would cause a circular dependency, a CircularException is thrown,
    ///           and no change is made to the spreadsheet.
    ///         </item>
    ///         <item>
    ///           Otherwise, the contents of the named cell becomes f.
    ///         </item>
    ///       </list>
    ///     </item>
    ///   </list>
    /// </summary>
    /// <returns>
    ///   <para>
    ///     The method returns a list consisting of the name plus the names
    ///     of all other cells whose value depends, directly or indirectly,
    ///     on the named cell. The order of the list should be any order
    ///     such that if cells are re-evaluated in that order, their dependencies
    ///     are satisfied by the time they are evaluated.
    ///   </para>
    ///   <example>
    ///     For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
    ///     list {A1, B1, C1} is returned.
    ///   </example>
    /// </returns>
    /// <exception cref="InvalidNameException">
    ///     If name is invalid, throws an InvalidNameException.
    /// </exception>
    /// <exception cref="CircularException">
    ///     If a formula would result in a circular dependency, throws CircularException.
    /// </exception>
    public IList<string> SetContentsOfCell( string name, string content )
	  {
          if (!IsValidVarName(name))
          {
              throw new InvalidNameException();
          }
          
          // 1. if double
          if (Double.TryParse(content, out double result))
          {
              return SetCellContents(name, result);
          } 
          // 2. if string
          else if (content.Trim().First() != '=')
          {
              return SetCellContents(name, content);
          }
          // 3. starts with '=', try parsing as a formula
          else
          {
              // No need to manually check for throwing errors here,
              // it should be handled inside the formula constructor
              Formula f = new Formula(content.Substring(1, content.Length - 1));
              // inside we dont need to check for circular dependencies, it will be
              // thrown from the actual called function
              return SetCellContents(name, f);
          }
	  }
    
}