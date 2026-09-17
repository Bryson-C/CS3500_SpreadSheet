// Skeleton implementation written by Joe Zachary for CS 3500, September 2013
// Version 1.1 - Joe Zachary
//   (Fixed error in comment for RemoveDependency)
// Version 1.2 - Daniel Kopta Fall 2018
//   (Clarified meaning of dependent and dependee)
//   (Clarified names in solution/project structure)
// Version 1.3 - H. James de St. Germain Fall 2024

namespace DependencyGraph;

/// <summary>
///   <para>
///     (s1,t1) is an ordered pair of strings, meaning t1 depends on s1.
///     (in other words: s1 must be evaluated before t1.)
///   </para>
///   <para>
///     A DependencyGraph can be modeled as a set of ordered pairs of strings.
///     Two ordered pairs (s1,t1) and (s2,t2) are considered equal if and only
///     if s1 equals s2 and t1 equals t2.
///   </para>
///   <remarks>
///     Recall that sets never contain duplicates.
///     If an attempt is made to add an element to a set, and the element is already
///     in the set, the set remains unchanged.
///   </remarks>
///   <para>
///     Given a DependencyGraph DG:
///   </para>
///   <list type="number">
///     <item>
///       If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
///       (The set of things that depend on s.)
///     </item>
///     <item>
///       If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
///       (The set of things that s depends on.)
///     </item>
///   </list>
///   <para>
///      For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}.
///   </para>
///   <code>
///     dependents("a") = {"b", "c"}
///     dependents("b") = {"d"}
///     dependents("c") = {}
///     dependents("d") = {"d"}
///     dependees("a")  = {}
///     dependees("b")  = {"a"}
///     dependees("c")  = {"a"}
///     dependees("d")  = {"b", "d"}
///   </code>
/// </summary>
public class DependencyGraph
{
    
    /// <summary>
    ///     this depends on another "node". It will contain all the variables that depend on another variable (the key).
    ///     For example: A3 is a variable, it needs to have A1 and A2 to be evaluated, A1 and A2 are the dependents of A3;
    ///     This means we set A3 as the Key, and add A1 and A2 to the set of unique (which is why a hashset is being used) values.
    ///     To get the ordered pair of strings, we can return (key, value[0]), (key, value[1]), ..., (key, value[n])
    ///
    ///     for a better summary of how these are modified when given formulas, see internals of <see cref="AddDependency"/>
    /// </summary>
    private Dictionary<string, HashSet<string>> _dependents = new();
    
    /// <summary>
    ///     another "node" depends on this. It will contain the set of variables that need the key to be evaluated first
    ///     For example: A3 is a variable, A1 and A2 need A3 to be evaluated first, so we would have (A3, A1), (A3, A2) as ordered pairs
    ///     the actual structure of the data would look similar to: [A3] = {A1, A2}
    ///
    ///     for a better summary of how these are modified when given formulas, see internals of <see cref="AddDependency"/>
    /// </summary>
    private Dictionary<string, HashSet<string>> _dependees = new();
    
    
    /// <summary>
    ///   Initializes a new instance of the <see cref="DependencyGraph"/> class.
    ///   The initial DependencyGraph is empty.
    /// </summary>
    public DependencyGraph()
    {
        // in here not much needs to be done initially as the dictionary should be initialized "on creation" of the object.
        // most of the functionality will happen when dependents/dependees are added
    }

    /// <summary>
    ///     This will track how many total dependencies were added to the graph.
    ///     it will only increase when calling "AddDependency" or decrease when calling "RemoveDependency"     
    /// </summary>
    private int _orderedPairCount = 0;
    
    /// <summary>
    /// The number of ordered pairs in the DependencyGraph.
    /// </summary>
    public int Size
    {
        // given a formula in cell A1: "A2+A3+A4", B1: "10", and C1:"B1"
        // we have ordered pairs: (A1, A2), (A1, A3), (A1, A4), (C1, B1)
        // since we can only modify dependencies through "AddDependency", and we need to follow the sequence:
        // Add A2 as a dependency to A1, Add A3 as a dependency to A1, ...
        // then we know each call adds one to the size, each remove subtracts one from the size
        get => _orderedPairCount; 
        
        // this will only be called within the class, see "_orderedPairCount" for explantation
        private set => _orderedPairCount = value;
        
    }

    /// <summary>
    ///   Reports whether the given node has dependents (i.e., other nodes depend on it).
    /// </summary>
    /// <param name="nodeName"> The name of the node.</param>
    /// <returns> true if the node has dependents. </returns>
    public bool HasDependents(string nodeName)
    {
        return _dependents.ContainsKey(nodeName) && _dependents[nodeName].Count > 0;
    }

    /// <summary>
    ///   Reports whether the given node has dependees (i.e., depends on one or more other nodes).
    /// </summary>
    /// <returns> true if the node has dependees.</returns>
    /// <param name="nodeName">The name of the node.</param>
    public bool HasDependees(string nodeName)
    {
        
        return _dependees.ContainsKey(nodeName) && _dependees[nodeName].Count > 0;
    }

    /// <summary>
    ///   <para>
    ///     Returns the dependents of the node with the given name.
    ///   </para>
    /// </summary>
    /// <param name="nodeName"> The node we are looking at.</param>
    /// <returns> The dependents of nodeName. </returns>
    public IEnumerable<string> GetDependents(string nodeName)
    {
        if (_dependents.ContainsKey(nodeName))
            return _dependents[nodeName];
        return new List<string>();
    }

    /// <summary>
    ///   <para>
    ///     Returns the dependees of the node with the given name.
    ///   </para>
    /// </summary>
    /// <param name="nodeName"> The node we are looking at.</param>
    /// <returns> The dependees of nodeName. </returns>
    public IEnumerable<string> GetDependees(string nodeName)
    {
        if (_dependees.ContainsKey(nodeName))
            return _dependees[nodeName];
        return new List<string>();
    }

    /// <summary>
    /// <para>Adds the ordered pair (dependee, dependent), if it doesn't exist.</para>
    ///
    /// <para>
    ///   This can be thought of as: dependee must be evaluated before dependent
    /// </para>
    /// </summary>
    /// <param name="dependee"> the name of the node that must be evaluated first</param>
    /// <param name="dependent"> the name of the node that cannot be evaluated until after dependee</param>
    public void AddDependency(string dependee, string dependent)
    {
        // As I understand it, given a formula in cell A1: "A2+A3"
        // A1 gets the dependees: A2 and A3
        // A2 gets the dependent A1
        // A3 gets the dependent A1
        // A2 and A3 must be evaluated first so, for the parameters this would look like:
        //      AddDependency(A2, A1);
        //      AddDependency(A3, A1);
        
        // this can also be interpreted as A1 supplies values to no one, and receives values from A2 and A3 (dependees) 
        // under this notation, a variables receives values from its dependees, and supplies values to its dependents
        // I personally like this notation better because it's different by more than 2 letters, but regardless, the train goes on
        // and abstraction should simplify the linguistic side of the problem 
        
        
        // make sure the hashset exists, to do this make sure the key is in the map.
        // if not, create the key, and create a new empty hashset
        
        // using a new simple example: cell A1: "A2"
        // dependees of [A1 (dependent)] = {A2 (dependee)}
        if (!_dependees.ContainsKey(dependent))
        {
            _dependees[dependent] = new HashSet<string>();
        }
        
        // likewise, make sure the dependents list can be pushed to
        
        // dependent of [A2(dependee)] = {A1 (dependent)}
        if (!_dependents.ContainsKey(dependee))
        {
            _dependents[dependee] = new HashSet<string>();
        }
        
        // here we have to check that the changes actually occur, otherwise the _orderedPairCount variable will become desynchronized
        if (!_dependees[dependent].Contains(dependee) && !_dependents[dependee].Contains(dependent))
        {
            _dependees[dependent].Add(dependee);
            _dependents[dependee].Add(dependent);
            Size++;
        }
    }

    /// <summary>
    ///   <para>
    ///     Removes the ordered pair (dependee, dependent), if it exists.
    ///   </para>
    /// </summary>
    /// <param name="dependee"> The name of the node that must be evaluated first</param>
    /// <param name="dependent"> The name of the node that cannot be evaluated until after dependee</param>
    public void RemoveDependency(string dependee, string dependent)
    {
        // if the dependee exists and has a dependant, remove it.
        // We have to do this for the dependents in the same if because only if both go off should the ordered pair count decrease
        // this ensures that the count will always be correct
        if (_dependees.ContainsKey(dependent) && _dependees[dependent].Contains(dependee)
            &&
            _dependents.ContainsKey(dependee) && _dependents[dependee].Contains(dependent))
        {
            _dependees[dependent].Remove(dependee);
            _dependents[dependee].Remove(dependent);
            Size--;
        }
    }

    /// <summary>
    ///   Removes all existing ordered pairs of the form (nodeName, *).  Then, for each
    ///   t in newDependents, adds the ordered pair (nodeName, t).
    /// </summary>
    /// <param name="nodeName"> The name of the node whose dependents are being replaced </param>
    /// <param name="newDependents"> The new dependents for nodeName</param>
    public void ReplaceDependents(string nodeName, IEnumerable<string> newDependents)
    {
        foreach (var dependent in GetDependents(nodeName))
        {
            RemoveDependency(nodeName, dependent);
        }
        
        foreach (var dependent in newDependents)
        {
            AddDependency(nodeName, dependent);
        }
    }

    /// <summary>
    ///   <para>
    ///     Removes all existing ordered pairs of the form (*, nodeName).  Then, for each
    ///     t in newDependees, adds the ordered pair (t, nodeName).
    ///   </para>
    /// </summary>
    /// <param name="nodeName"> The name of the node who's dependees are being replaced</param>
    /// <param name="newDependees"> The new dependees for nodeName</param>
    public void ReplaceDependees(string nodeName, IEnumerable<string> newDependees)
    {
        foreach (var dependee in GetDependees(nodeName))
        {
            RemoveDependency(dependee, nodeName);
        }
        
        foreach (var dependee in newDependees)
        {
            AddDependency(dependee, nodeName);
        }
    }
}