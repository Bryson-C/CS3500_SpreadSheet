// <copyright file="Spreadsheet.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>

// Written by Joe Zachary for CS 3500, September 2013
// Update by Profs Kopta, de St. Germain, Martin, Fall 2021, Fall 2024, Fall 2025
//     - Updated return types
//     - Updated documentation

using System.Text.RegularExpressions;

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
///     A class to store data of a cell from a spreadsheet.
///     Can be given a double, string, or formula.
///     Is not responsible for who it depends on, it will only store the data of a single cell
/// </summary>
public class Cell
{
    /// <summary>
    ///     This will be used to check what type the cell is holding,
    ///     it should be assumed that if a cell has one of these types,
    ///     then parsing the "_cellData" value as that type should not throw
    ///     an error
    /// </summary>
    private enum CellType
    {
        Double, String, Formula
    }
    
    private CellType ValueType { get; }
    
    /// <summary>
    ///     This variable can only be a double, string, or Formula
    ///     As specified above <see cref="CellType"/>, the value must be valid
    ///     and therefore should also be able to be fearlessly converted to the
    ///     type stored in "_cellType"/given by the constructors
    ///
    ///     "CellData" is not to be confused with the value of the cell, it represents the cell's
    ///     contents, i.e. a formula's string not its evaluated value
    ///
    ///     The value of CellData should not be set after the constructor is called
    ///     (readonly/immutable)
    /// </summary>
    private object CellData { get; }

    /// <summary>
    ///     Sets the _cellData object to a double, and the ValueType to CellType.Double
    /// </summary>
    /// <param name="number"> the value desired for the cell to have stored </param>
    public Cell(double number)
    {
        CellData = number;
        ValueType = CellType.Double;
    }

    /// <summary>
    ///     Sets the _cellData object to a string, and the ValueType to CellType.String
    /// </summary>
    /// <param name="str"> the string desired for the cell to have stored </param>
    public Cell(string str)
    {
        CellData = str;
        ValueType = CellType.String;
    }

    /// <summary>
    ///     Sets the _cellData object to a formula, and the ValueType to CellType.Formula
    ///     The formula must be a valid formula
    /// </summary>
    /// <param name="formula"> the formula desired for the cell to have stored </param>
    public Cell(Formula formula)
    {
        CellData = formula;
        ValueType = CellType.Formula;
    }

    /// <summary>
    /// Will return true if the cell was initially set as a double type, otherwise false
    /// If true, then the value of the cell should be safe to convert to said type
    /// </summary>
    public bool IsDoubleType()
    {
        return ValueType == CellType.Double;
    }

    /// <summary>
    /// Will return true if the cell was initially set as a double type, otherwise false
    /// If true, then the value of the cell should be safe to convert to said type
    /// </summary>
    public bool IsStringType()
    {
        return ValueType == CellType.String;
    }

    /// <summary>
    /// Will return true if the cell was initially set as a double type, otherwise false
    /// If true, then the value of the cell should be safe to convert to said type
    /// </summary>
    public bool IsFormulaType()
    {
        return ValueType == CellType.Formula;
    }

    /// <summary>
    ///     Returns the data stored in CellData, after checking the type
    ///     (<see cref="IsDoubleType"/>, <see cref="IsStringType"/>, <see cref="IsFormulaType"/>)
    ///     it should be safe to convert to said type
    /// </summary>
    /// <returns> Returns the data stored in CellData </returns>
    public object GetCellData()
    {
        return CellData;
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
    public IList<string> SetCellContents(string name, double number)
    {
        if (!IsValidVarName(name))
        {
            throw new InvalidNameException();
        }
        // since only numbers don't have dependencies, don't worry about the dependency graph
        if (_contentCells.ContainsKey(name))
        {
            _contentCells[name] = new Cell(number);
        } 
        else
        {
            _contentCells.Add(name, new Cell(number));
        }    
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
    public IList<string> SetCellContents(string name, string text)
    {
        if (!IsValidVarName(name))
        {
            throw new InvalidNameException();
        }
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
    public IList<string> SetCellContents(string name, Formula formula)
    {
        if (!IsValidVarName(name))
        {
            throw new InvalidNameException();
        }
        
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
        return GetCellsToRecalculate(name).ToList();
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
}