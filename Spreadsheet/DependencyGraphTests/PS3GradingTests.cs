// <copyright file="GradingTests.cs" company="UofU-CS3500">
// Copyright (c) 2025 UofU-CS3500. All rights reserved.
// </copyright>

namespace PS2GradingTests;

using DependencyGraph;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Authors:   Joe Zachary
///            Daniel Kopta
///            Jim de St. Germain
///            Travis Martin
/// Date:      Updated Spring 2026
/// Course:    CS 3500, University of Utah, Kahlert School of Computing
/// Copyright: CS 3500 - This work may not be copied for use
///                      in Academic Coursework.  See below.
///
/// File Contents:
///
///   This file contains proprietary grading tests for CS 3500.  These tests cases
///   are for individual student use only and MAY NOT BE SHARED.  Do not back them up
///   nor place them in any online repository.  Improper use of these test cases
///   can result in removal from the course and an academic misconduct sanction.
///
///   These tests are for your private use only to improve the quality of the
///   rest of your assignments.
///   This is a test class for DependencyGraphTest and is intended
///   to contain all DependencyGraphTest Unit Tests.
/// </summary>
[TestClass]
public class DependencyGraphTest
{
    // ************************** TESTS ON EMPTY DGs ************************* //

    /// <summary>
    ///   Empty graph should contain nothing.
    /// </summary>
    [TestMethod]
    [Timeout( 2000, CooperativeCancellation = true)]
    [TestCategory( "1" )]
    public void Size_EmptyGraph_ReturnsZero( )
    {
        DependencyGraph dg = new();
        Assert.AreEqual( 0, dg.Size );
    }

    /// <summary>
    ///   Empty graph should not have a node with dependees.
    /// </summary>
    [TestMethod]
    [Timeout( 2000, CooperativeCancellation = true)]
    [TestCategory( "2" )]
    public void HasDependees_EmptyGraph_NoDependees( )
    {
        DependencyGraph dg = new();
        Assert.IsFalse( dg.HasDependees( "x" ) );
    }

    /// <summary>
    ///   Empty graph should not have a node with dependees.
    /// </summary>
    [TestMethod]
    [Timeout( 2000, CooperativeCancellation = true)]
    [TestCategory( "3" )]
    public void HasDependents_EmptyGraph_NoDependents( )
    {
        DependencyGraph dg = new();
        Assert.IsFalse( dg.HasDependents( "x" ) );
    }

    /// <summary>
    ///   Removing from an empty DG shouldn't fail.
    /// </summary>
    [TestMethod]
    [Timeout( 2000, CooperativeCancellation = true)]
    [TestCategory( "4" )]
    public void RemoveDependency_EmptyGraph_NoEffect( )
    {
        DependencyGraph dg = new();
        Assert.IsFalse( dg.HasDependents( "x" ) );
        dg.RemoveDependency( "x", "y" );
        Assert.IsFalse( dg.HasDependents( "x" ) );
    }

    /// <summary>
    ///   Replace on an empty DG shouldn't fail.
    /// </summary>
    [TestMethod]
    [Timeout( 2000, CooperativeCancellation = true)]
    [TestCategory( "5" )]
    public void ReplaceDependents_EmptyGraph_NoEffect( )
    {
        DependencyGraph dg = new();
        dg.ReplaceDependents( "x", [] );
    }

    /// <summary>
    ///   Replace on an empty DG shouldn't fail.
    /// </summary>
    [TestMethod]
    [Timeout( 2000, CooperativeCancellation = true)]
    [TestCategory( "6" )]
    public void ReplaceDependees_EmptyGraph_NoEffect( )
    {
        DependencyGraph dg = new();
        dg.ReplaceDependees( "y", [] );
    }

    // ************************ MORE TESTS ON EMPTY DGs *********************** //

    /// <summary>
    ///   Add one element, check the size, remove the element, check the size.
    /// </summary>
    [TestMethod]
    [Timeout( 2000, CooperativeCancellation = true)]
    [TestCategory( "7" )]
    public void AddRemove_FromEmpty_OneThenZero( )
    {
        DependencyGraph dg = new();
        dg.AddDependency( "x", "y" );
        Assert.AreEqual( 1, dg.Size );
        dg.RemoveDependency( "x", "y" );
        Assert.AreEqual( 0, dg.Size );
    }

    /// <summary>
    ///   Add multiple (two) elements, check the size, remove them, check the size.
    /// </summary>
    [TestMethod]
    [Timeout( 2000, CooperativeCancellation = true)]
    [TestCategory( "8" )]
    public void AddRemove_FromEmpty_TwoThenZero( )
    {
        DependencyGraph t = new();
        t.AddDependency( "x", "y" );
        Assert.IsTrue( t.HasDependees( "y" ) );
        Assert.IsTrue( t.HasDependents( "x" ) );
        t.RemoveDependency( "x", "y" );
        Assert.IsFalse( t.HasDependees( "y" ) );
        Assert.IsFalse( t.HasDependents( "x" ) );
    }

    /// <summary>
    ///    <para>
    ///      Check if after adding the x->y we have the correct dependees and
    ///      the correct dependents.
    ///    </para>
    ///    <para>
    ///      After removing, there should be no contents left.
    ///    </para>
    /// </summary>
    [TestMethod]
    [Timeout( 2000, CooperativeCancellation = true)]
    [TestCategory( "9" )]
    public void AddAndRemoveDependency_FromEmpty_Valid( )
    {
        DependencyGraph dg = new();
        dg.AddDependency( "x", "y" );

        Assert.IsTrue( dg.GetDependees( "y" ).Matches( ["x"] ) );
        Assert.IsTrue( dg.GetDependents( "x" ).Matches( ["y"] ) );

        dg.RemoveDependency( "x", "y" );

        Assert.IsTrue( dg.GetDependees( "y" ).Matches( [] ) );
        Assert.IsTrue( dg.GetDependents( "x" ).Matches( [] ) );
    }

    /// <summary>
    ///   Add a dependency and the number of dependees should be 1.
    ///   Remove it, and the number should be zero.
    /// </summary>
    [TestMethod]
    [Timeout( 2000, CooperativeCancellation = true)]
    [TestCategory( "10" )]
    public void GetDependees_AddThenRemove_SizeOneThenZero( )
    {
        DependencyGraph dg = new();
        dg.AddDependency( "x", "y" );
        Assert.AreEqual( 1, dg.GetDependees("y").Count() );
        dg.RemoveDependency( "x", "y" );
        Assert.AreEqual( 0, dg.GetDependees("x").Count() );
    }

    /// <summary>
    ///   Removing from an empty DG shouldn't fail.
    /// </summary>
    [TestMethod]
    [Timeout( 2000, CooperativeCancellation = true)]
    [TestCategory( "11" )]
    public void RemoveDependency_DoItTwice_ShouldNotError( )
    {
        DependencyGraph dg = new();
        dg.AddDependency( "x", "y" );
        Assert.AreEqual( 1, dg.Size );
        dg.RemoveDependency( "x", "y" );
        Assert.AreEqual( 0, dg.Size );
        dg.RemoveDependency( "x", "y" );
        Assert.AreEqual( 0, dg.Size );
    }

    /// <summary>
    ///   Replace dependencies with an empty list should not fail.
    /// </summary>
    [TestMethod]
    [Timeout( 2000, CooperativeCancellation = true)]
    [TestCategory( "12" )]
    public void RemoveAndReplace_CanReplaceWithEmpty( )
    {
        DependencyGraph dg = new();
        dg.AddDependency( "x", "y" );
        Assert.AreEqual( 1, dg.Size );

        dg.RemoveDependency( "x", "y" );
        dg.ReplaceDependents( "x", [] );
        dg.ReplaceDependees( "y", [] );
        Assert.AreEqual( 0, dg.Size );
    }

    // ********************** Making Sure that Static Variables Weren't Used ****************** //

    /// <summary>
    ///   <para>
    ///     It should be possible to have more than one DG at a time.
    ///   </para>
    ///   <remark>
    ///     This test is repeated to increase its weighting in the grading.
    ///   </remark>
    /// </summary>
    [TestMethod]
    [Timeout( 2000, CooperativeCancellation = true)]
    [TestCategory( "13" )]
    public void Constructor_CreateMultipleInstances_EachIsIndependentOfTheOther_1( )
    {
        DependencyGraph dg1 = new();
        DependencyGraph dg2 = new();
        dg1.AddDependency( "x", "y" );
        Assert.AreEqual( 1, dg1.Size );
        Assert.AreEqual( 0, dg2.Size );
    }

    /// <summary>
    ///   Increase weight of Previous Test. Not necessary in Non-Grading test suite.
    /// </summary>
    [TestMethod]
    [Timeout( 2000, CooperativeCancellation = true)]
    [TestCategory( "14" )]
    public void Constructor_CreateMultipleInstances_EachIsIndependentOfTheOther_2( )
    {
        Constructor_CreateMultipleInstances_EachIsIndependentOfTheOther_1();
    }

    /// <summary>
    ///   Increase weight of Previous Test. Not necessary in Non-Grading test suite.
    /// </summary>
    [TestMethod]
    [Timeout( 2000, CooperativeCancellation = true)]
    [TestCategory( "15" )]
    public void TestStatic1_3( )
    {
        Constructor_CreateMultipleInstances_EachIsIndependentOfTheOther_1();
    }

    /// <summary>
    ///   Increase weight of Previous Test. Not necessary in Non-Grading test suite.
    /// </summary>
    [TestMethod]
    [Timeout( 2000, CooperativeCancellation = true)]
    [TestCategory( "16" )]
    public void TestStatic1_4( )
    {
        Constructor_CreateMultipleInstances_EachIsIndependentOfTheOther_1();
    }

    /// <summary>
    ///   Increase weight of Previous Test. Not necessary in Non-Grading test suite.
    /// </summary>
    [TestMethod]
    [Timeout( 2000, CooperativeCancellation = true)]
    [TestCategory( "17" )]
    public void TestStatic1_5( )
    {
        Constructor_CreateMultipleInstances_EachIsIndependentOfTheOther_1();
    }

    /**************************** SIMPLE NON-EMPTY TESTS ****************************/

    /// <summary>
    ///   Add four dependencies and the size should be four.
    /// </summary>
    [TestMethod]
    [Timeout( 2000, CooperativeCancellation = true)]
    [TestCategory( "18" )]
    public void Size_AddFour_ResultIsFour( )
    {
        DependencyGraph dg = new();
        dg.AddDependency( "a", "b" );
        dg.AddDependency( "a", "c" );
        dg.AddDependency( "c", "b" );
        dg.AddDependency( "b", "d" );
        Assert.AreEqual( 4, dg.Size );
    }

    /// <summary>
    ///    After adding two items that depend on "b", check the dependees on the "b" node.
    ///    There should be two.
    ///  </summary>
    [TestMethod]
    [Timeout( 2000, CooperativeCancellation = true)]
    [TestCategory( "19" )]
    public void AddDependencyGetDependees_AddTwoDependenciesToSameNode_GetDependeesValueOfTwo( )
    {
        DependencyGraph dg = new();

        dg.AddDependency( "a", "b" );
        dg.AddDependency( "a", "c" );
        dg.AddDependency( "c", "b" );
        dg.AddDependency( "b", "d" );

        var dependees= dg.GetDependees("b").ToList();

        Assert.AreEqual( 2, dependees.Count() );
        Assert.IsTrue( dependees.Matches( ["a", "c"] ) );
    }

    /// <summary>
    ///   Given a -> b, a -> c, c -> b, and b -> d.
    ///   a has dependents. a does not have dependees.  b had both dependents and dependees.
    /// </summary>
    [TestMethod]
    [Timeout( 2000, CooperativeCancellation = true)]
    [TestCategory( "20" )]
    public void HasDependentsHasDependees_SmallGraph_ReturnsAppropriateTrueFalse( )
    {
        DependencyGraph dg = new();
        dg.AddDependency( "a", "b" );
        dg.AddDependency( "a", "c" );
        dg.AddDependency( "c", "b" );
        dg.AddDependency( "b", "d" );
        Assert.IsTrue( dg.HasDependents( "a" ) );
        Assert.IsFalse( dg.HasDependees( "a" ) );
        Assert.IsTrue( dg.HasDependents( "b" ) );
        Assert.IsTrue( dg.HasDependees( "b" ) );
    }

    /// <summary>
    ///    Check that the simple graph contains the right dependents and dependees.
    /// </summary>
    [TestMethod]
    [Timeout( 2000, CooperativeCancellation = true)]
    [TestCategory( "21" )]
    public void GetDependeesGetDependents_SimpleGraph_ContainsCorrectValues( )
    {
        DependencyGraph dg = new();
        dg.AddDependency( "a", "b" );
        dg.AddDependency( "a", "c" );
        dg.AddDependency( "c", "b" );
        dg.AddDependency( "b", "d" );

        IEnumerable<string> dependentsOfA = dg.GetDependents("a");
        IEnumerable<string> dependeesOfB = dg.GetDependees( "b" );

        Assert.IsTrue( dependentsOfA.Matches( ["b", "c"] ) );
        Assert.IsTrue( dependeesOfB.Matches( ["a", "c"] ) );
    }

    /// <summary>
    ///   Test that a simple graph does not have values for items
    ///   that were not added to the graph....
    /// </summary>
    [TestMethod]
    [Timeout( 2000, CooperativeCancellation = true)]
    [TestCategory( "22" )]
    public void GetDependentsGetDependees_GetValuesFromValidAndInvalidNodes_ReturnsZeroOrOne(  )
    {
        DependencyGraph dg = new();
        dg.AddDependency( "a", "b" );
        dg.AddDependency( "a", "c" );
        dg.AddDependency( "c", "b" );
        dg.AddDependency( "b", "d" );

        IEnumerable<string> dependeesOfE = dg.GetDependees("e");
        IEnumerable<string> dependentsOfE = dg.GetDependents( "e" );
        IEnumerable<string> dependeesOfF = dg.GetDependees("f");
        IEnumerable<string> dependentsOfF = dg.GetDependents( "f" );
        IEnumerable<string> dependeesOfD = dg.GetDependees("d");

        Assert.AreEqual( 0, dependeesOfE.Count() );
        Assert.AreEqual( 0, dependeesOfF.Count() );
        Assert.AreEqual( 1, dependeesOfD.Count() );
        Assert.AreEqual( 0, dependentsOfE.Count() );
        Assert.AreEqual( 0, dependentsOfF.Count() );
    }

    /// <summary>
    ///   FIXME.
    /// </summary>
    [TestMethod]
    [Timeout( 2000, CooperativeCancellation = true)]
    [TestCategory( "23" )]
    public void TestDuplicatesSize( )
    {
        DependencyGraph t = new();
        t.AddDependency( "a", "b" );
        t.AddDependency( "a", "c" );
        t.AddDependency( "a", "b" );
        t.AddDependency( "c", "b" );
        t.AddDependency( "b", "d" );
        t.AddDependency( "c", "b" );
        Assert.AreEqual( 4, t.Size );
    }

    /// <summary>
    ///   Test that "set" functionality is correct. Cannot add the same item multiple times.
    /// </summary>
    [TestMethod]
    [Timeout( 2000, CooperativeCancellation = true)]
    [TestCategory( "24" )]
    public void AddDependency_AddTheSameItemMultipleTimes_ShouldOnlyAddOnce( )
    {
        DependencyGraph t = new();
        t.AddDependency( "a", "b" );
        t.AddDependency( "a", "b" );
        t.AddDependency( "c", "b" );
        t.AddDependency( "c", "b" );
        Assert.AreEqual( 2, t.GetDependees( "b" ).Count() );
        Assert.AreEqual( 1, t.GetDependents( "a" ).Count() );
        Assert.AreEqual( 1, t.GetDependents( "c" ).Count() );
    }

    /// <summary>
    ///   Attempting to add the same dependency multiple time also
    ///   does not affect the HasDependents functionality.
    /// </summary>
    [TestMethod]
    [Timeout( 2000, CooperativeCancellation = true)]
    [TestCategory( "25" )]
    public void HasDependentsDependees_AddDuplicatesDependencies_DoesNotChangeGraph( )
    {
        DependencyGraph t = new();
        t.AddDependency( "a", "b" );
        t.AddDependency( "a", "c" );
        t.AddDependency( "a", "b" );
        t.AddDependency( "c", "b" );
        t.AddDependency( "b", "d" );
        t.AddDependency( "c", "b" );
        Assert.IsTrue( t.HasDependents( "a" ) );
        Assert.IsFalse( t.HasDependees( "a" ) );
        Assert.IsTrue( t.HasDependents( "b" ) );
        Assert.IsTrue( t.HasDependees( "b" ) );
    }

    /// <summary>
    ///   FIXME.
    /// </summary>
    [TestMethod]
    [Timeout( 2000, CooperativeCancellation = true)]
    [TestCategory( "26" )]
    public void TestDuplicatesEnumerator( )
    {
        DependencyGraph t = new();
        t.AddDependency( "a", "b" );
        t.AddDependency( "a", "c" );
        t.AddDependency( "a", "b" );
        t.AddDependency( "c", "b" );
        t.AddDependency( "b", "d" );
        t.AddDependency( "c", "b" );

        IEnumerator<string> e = t.GetDependees("a").GetEnumerator();
        Assert.IsFalse( e.MoveNext() );

        e = t.GetDependees( "b" ).GetEnumerator();
        Assert.IsTrue( e.MoveNext() );

        string s1 = e.Current;
        Assert.IsTrue( e.MoveNext() );
        string s2 = e.Current;
        Assert.IsFalse( e.MoveNext() );
        Assert.IsTrue( ( ( s1 == "a" ) && ( s2 == "c" ) ) || ( ( s1 == "c" ) && ( s2 == "a" ) ) );

        e = t.GetDependees( "c" ).GetEnumerator();
        Assert.IsTrue( e.MoveNext() );
        Assert.AreEqual( "a", e.Current );
        Assert.IsFalse( e.MoveNext() );

        e = t.GetDependees( "d" ).GetEnumerator();
        Assert.IsTrue( e.MoveNext() );
        Assert.AreEqual( "b", e.Current );
        Assert.IsFalse( e.MoveNext() );
        e.Dispose();
    }

    /// <summary>
    ///   FIXME.
    /// </summary>
    [TestMethod]
    [Timeout( 2000, CooperativeCancellation = true)]
    [TestCategory( "27" )]
    public void TestDuplicatesEnumerator2( )
    {
        DependencyGraph t = new();
        t.AddDependency( "a", "b" );
        t.AddDependency( "a", "c" );
        t.AddDependency( "a", "b" );
        t.AddDependency( "c", "b" );
        t.AddDependency( "b", "d" );
        t.AddDependency( "c", "b" );

        IEnumerator<string> e = t.GetDependents("a").GetEnumerator();
        Assert.IsTrue( e.MoveNext() );

        string s1 = e.Current;
        Assert.IsTrue( e.MoveNext() );
        string s2 = e.Current;
        Assert.IsFalse( e.MoveNext() );
        Assert.IsTrue( ( ( s1 == "b" ) && ( s2 == "c" ) ) || ( ( s1 == "c" ) && ( s2 == "b" ) ) );

        e = t.GetDependents( "b" ).GetEnumerator();
        Assert.IsTrue( e.MoveNext() );
        Assert.AreEqual( "d", e.Current );
        Assert.IsFalse( e.MoveNext() );

        e = t.GetDependents( "c" ).GetEnumerator();
        Assert.IsTrue( e.MoveNext() );
        Assert.AreEqual( "b", e.Current );
        Assert.IsFalse( e.MoveNext() );

        e = t.GetDependents( "d" ).GetEnumerator();
        Assert.IsFalse( e.MoveNext() );
        e.Dispose();
    }

    /// <summary>
    ///   FIXME.
    /// </summary>
    [TestMethod]
    [Timeout( 2000, CooperativeCancellation = true)]
    [TestCategory( "28" )]
    public void TestComplexAddRemove( )
    {
        DependencyGraph t = new();
        t.AddDependency( "x", "y" );
        t.AddDependency( "a", "b" );
        t.AddDependency( "a", "c" );
        t.AddDependency( "a", "d" );
        t.AddDependency( "c", "b" );
        t.RemoveDependency( "a", "d" );
        t.AddDependency( "e", "b" );
        t.AddDependency( "b", "d" );
        t.RemoveDependency( "e", "b" );
        t.RemoveDependency( "x", "y" );
        Assert.AreEqual( 4, t.Size );
    }

    /// <summary>
    ///    FIXME.
    /// </summary>
    [TestMethod]
    [Timeout( 2000, CooperativeCancellation = true)]
    [TestCategory( "29" )]
    public void TestAddRemoveIndexer( )
    {
        DependencyGraph t = new();
        t.AddDependency( "x", "y" );
        t.AddDependency( "a", "b" );
        t.AddDependency( "a", "c" );
        t.AddDependency( "a", "d" );
        t.AddDependency( "c", "b" );
        t.RemoveDependency( "a", "d" );
        t.AddDependency( "e", "b" );
        t.AddDependency( "b", "d" );
        t.RemoveDependency( "e", "b" );
        t.RemoveDependency( "x", "y" );
        Assert.AreEqual( 2, t.GetDependees("b").Count() );
    }

    /// <summary>
    ///    FIXME.
    /// </summary>
    [TestMethod]
    [Timeout( 2000, CooperativeCancellation = true)]
    [TestCategory( "30" )]
    public void TestAddRemoveDependencies( )
    {
        DependencyGraph t = new();
        t.AddDependency( "x", "y" );
        t.AddDependency( "a", "b" );
        t.AddDependency( "a", "c" );
        t.AddDependency( "a", "d" );
        t.AddDependency( "c", "b" );
        t.RemoveDependency( "a", "d" );
        t.AddDependency( "e", "b" );
        t.AddDependency( "b", "d" );
        t.RemoveDependency( "e", "b" );
        t.RemoveDependency( "x", "y" );
        Assert.IsTrue( t.HasDependents( "a" ) );
        Assert.IsFalse( t.HasDependees( "a" ) );
        Assert.IsTrue( t.HasDependents( "b" ) );
        Assert.IsTrue( t.HasDependees( "b" ) );
    }

    /// <summary>
    ///    FIXME.
    /// </summary>
    [TestMethod]
    [Timeout( 2000, CooperativeCancellation = true)]
    [TestCategory( "31" )]
    public void TestComplexEnumerator( )
    {
        DependencyGraph t = new();
        t.AddDependency( "x", "y" );
        t.AddDependency( "a", "b" );
        t.AddDependency( "a", "c" );
        t.AddDependency( "a", "d" );
        t.AddDependency( "c", "b" );
        t.RemoveDependency( "a", "d" );
        t.AddDependency( "e", "b" );
        t.AddDependency( "b", "d" );
        t.RemoveDependency( "e", "b" );
        t.RemoveDependency( "x", "y" );

        IEnumerator<string> e = t.GetDependees("a").GetEnumerator();
        Assert.IsFalse( e.MoveNext() );

        e = t.GetDependees( "b" ).GetEnumerator();
        Assert.IsTrue( e.MoveNext() );

        string s1 = e.Current;
        Assert.IsTrue( e.MoveNext() );
        string s2 = e.Current;
        Assert.IsFalse( e.MoveNext() );
        Assert.IsTrue( ( ( s1 == "a" ) && ( s2 == "c" ) ) || ( ( s1 == "c" ) && ( s2 == "a" ) ) );

        e = t.GetDependees( "c" ).GetEnumerator();
        Assert.IsTrue( e.MoveNext() );
        Assert.AreEqual( "a", e.Current );
        Assert.IsFalse( e.MoveNext() );

        e = t.GetDependees( "d" ).GetEnumerator();
        Assert.IsTrue( e.MoveNext() );
        Assert.AreEqual( "b", e.Current );
        Assert.IsFalse( e.MoveNext() );
        e.Dispose();
    }

    /// <summary>
    ///    FIXME.
    /// </summary>
    [TestMethod]
    [Timeout( 2000, CooperativeCancellation = true)]
    [TestCategory( "32" )]
    public void TestComplexEnumerator2( )
    {
        DependencyGraph t = new();
        t.AddDependency( "x", "y" );
        t.AddDependency( "a", "b" );
        t.AddDependency( "a", "c" );
        t.AddDependency( "a", "d" );
        t.AddDependency( "c", "b" );
        t.RemoveDependency( "a", "d" );
        t.AddDependency( "e", "b" );
        t.AddDependency( "b", "d" );
        t.RemoveDependency( "e", "b" );
        t.RemoveDependency( "x", "y" );

        IEnumerator<string> e = t.GetDependents("a").GetEnumerator();
        Assert.IsTrue( e.MoveNext() );

        string s1 = e.Current;
        Assert.IsTrue( e.MoveNext() );
        string s2 = e.Current;
        Assert.IsFalse( e.MoveNext() );
        Assert.IsTrue( ( ( s1 == "b" ) && ( s2 == "c" ) ) || ( ( s1 == "c" ) && ( s2 == "b" ) ) );

        e = t.GetDependents( "b" ).GetEnumerator();
        Assert.IsTrue( e.MoveNext() );
        Assert.AreEqual( "d", e.Current );
        Assert.IsFalse( e.MoveNext() );

        e = t.GetDependents( "c" ).GetEnumerator();
        Assert.IsTrue( e.MoveNext() );
        Assert.AreEqual( "b", e.Current );
        Assert.IsFalse( e.MoveNext() );

        e = t.GetDependents( "d" ).GetEnumerator();
        Assert.IsFalse( e.MoveNext() );
        e.Dispose();
    }

    /// <summary>
    ///   FIXME.
    /// </summary>
    [TestMethod]
    [Timeout( 2000, CooperativeCancellation = true)]
    [TestCategory( "33" )]
    public void TestEmptyReplaceDependees( )
    {
        DependencyGraph dg = new();

        dg.ReplaceDependees( "b", [ "a" ] );

        Assert.AreEqual( 1, dg.Size );
        Assert.IsTrue( new HashSet<string> { "b" }.SetEquals( dg.GetDependents( "a" ) ) );
    }

    /// <summary>
    ///    FIXME.
    /// </summary>
    [TestMethod]
    [Timeout( 2000, CooperativeCancellation = true)]
    [TestCategory( "34" )]
    public void TestComplexReplace( )
    {
        DependencyGraph t = new();
        t.AddDependency( "x", "b" );
        t.AddDependency( "a", "z" );
        t.ReplaceDependents( "b", [] );
        t.AddDependency( "y", "b" );
        t.ReplaceDependents( "a", ["c"] );
        t.AddDependency( "w", "d" );
        t.ReplaceDependees( "b", ["a", "c" ] );
        t.ReplaceDependees( "d", [ "b"] );
        Assert.AreEqual( 4, t.Size );
    }

    /// <summary>
    ///    FIXME.
    /// </summary>
    [TestMethod]
    [Timeout( 2000, CooperativeCancellation = true)]
    [TestCategory( "35" )]
    public void TestComplexReplaceIndexer( )
    {
        DependencyGraph t = new();
        t.AddDependency( "x", "b" );
        t.AddDependency( "a", "z" );
        t.ReplaceDependents( "b", [] );
        t.AddDependency( "y", "b" );
        t.ReplaceDependents( "a", [ "c" ]);
        t.AddDependency( "w", "d" );
        t.ReplaceDependees( "b", [ "a", "c" ] );
        t.ReplaceDependees( "d", [ "b" ]);
        Assert.AreEqual( 2, t.GetDependees("b").Count() );
    }

    /// <summary>
    ///    FIXME.
    /// </summary>
    [TestMethod]
    [Timeout( 2000, CooperativeCancellation = true)]
    [TestCategory( "36" )]
    public void TestComplexReplace2( )
    {
        DependencyGraph t = new();
        t.AddDependency( "x", "b" );
        t.AddDependency( "a", "z" );
        t.ReplaceDependents( "b", [] );
        t.AddDependency( "y", "b" );
        t.ReplaceDependents( "a", [ "c" ]);
        t.AddDependency( "w", "d" );
        t.ReplaceDependees( "b", [ "a", "c" ] );
        t.ReplaceDependees( "d", [ "b" ]);
        Assert.IsTrue( t.HasDependents( "a" ) );
        Assert.IsFalse( t.HasDependees( "a" ) );
        Assert.IsTrue( t.HasDependents( "b" ) );
        Assert.IsTrue( t.HasDependees( "b" ) );
    }

    /// <summary>
    ///    FIXME.
    /// </summary>
    [TestMethod]
    [Timeout( 2000, CooperativeCancellation = true)]
    [TestCategory( "37" )]
    public void TestComplexReplaceEnumerator1( )
    {
        DependencyGraph t = SetupComplexDependencies();

        IEnumerator<string> e = t.GetDependees("a").GetEnumerator();
        Assert.IsFalse( e.MoveNext() );

        e = t.GetDependees( "b" ).GetEnumerator();
        Assert.IsTrue( e.MoveNext() );

        string s1 = e.Current;
        Assert.IsTrue( e.MoveNext() );
        string s2 = e.Current;
        Assert.IsFalse( e.MoveNext() );
        Assert.IsTrue( ( ( s1 == "a" ) && ( s2 == "c" ) ) || ( ( s1 == "c" ) && ( s2 == "a" ) ) );

        e = t.GetDependees( "c" ).GetEnumerator();
        Assert.IsTrue( e.MoveNext() );
        Assert.AreEqual( "a", e.Current );
        Assert.IsFalse( e.MoveNext() );

        e = t.GetDependees( "d" ).GetEnumerator();
        Assert.IsTrue( e.MoveNext() );
        Assert.AreEqual( "b", e.Current );
        Assert.IsFalse( e.MoveNext() );
        e.Dispose();
    }

    /// <summary>
    ///    FIXME.
    /// </summary>
    [TestMethod]
    [Timeout( 2000, CooperativeCancellation = true)]
    [TestCategory( "38" )]
    public void TestComplexReplaceEnumerator2( )
    {
        DependencyGraph t = SetupComplexDependencies();

        IEnumerator<string> e = t.GetDependents("a").GetEnumerator();
        Assert.IsTrue( e.MoveNext() );

        string s1 = e.Current;
        Assert.IsTrue( e.MoveNext() );
        string s2 = e.Current;
        Assert.IsFalse( e.MoveNext() );
        Assert.IsTrue( ( ( s1 == "b" ) && ( s2 == "c" ) ) || ( ( s1 == "c" ) && ( s2 == "b" ) ) );

        e = t.GetDependents( "b" ).GetEnumerator();
        Assert.IsTrue( e.MoveNext() );
        Assert.AreEqual( "d", e.Current );
        Assert.IsFalse( e.MoveNext() );

        e = t.GetDependents( "c" ).GetEnumerator();
        Assert.IsTrue( e.MoveNext() );
        Assert.AreEqual( "b", e.Current );
        Assert.IsFalse( e.MoveNext() );

        e = t.GetDependents( "d" ).GetEnumerator();
        Assert.IsFalse( e.MoveNext() );
        e.Dispose();
    }

    // ************************** STRESS TESTS REPEATED MULTIPLE TIMES ******************************** //

    /// <summary>
    ///    Using lots of data.
    /// </summary>
    [TestMethod]
    [Timeout( 2000, CooperativeCancellation = true)]
    [TestCategory( "39" )]
    public void StressTest1_1( )
    {
        // Dependency graph
        DependencyGraph t = new();

        // A bunch of strings to use
        const int size = 200;
        string[] letters = new string[size];
        for ( int i = 0; i < size; i++ )
        {
            letters[i] =  "a" + i;
        }

        // The correct answers
        HashSet<string>[] dents = new HashSet<string>[size];
        HashSet<string>[] dees = new HashSet<string>[size];
        for ( int i = 0; i < size; i++ )
        {
            dents[i] = [];
            dees[i] = [];
        }

        // Add a bunch of dependencies
        for ( int i = 0; i < size; i++ )
        {
            for ( int j = i + 1; j < size; j++ )
            {
                t.AddDependency( letters[i], letters[j] );
                dents[i].Add( letters[j] );
                dees[j].Add( letters[i] );
            }
        }

        // Remove a bunch of dependencies
        for ( int i = 0; i < size; i++ )
        {
            for ( int j = i + 4; j < size; j += 4 )
            {
                t.RemoveDependency( letters[i], letters[j] );
                dents[i].Remove( letters[j] );
                dees[j].Remove( letters[i] );
            }
        }

        // Add some back
        for ( int i = 0; i < size; i++ )
        {
            for ( int j = i + 1; j < size; j += 2 )
            {
                t.AddDependency( letters[i], letters[j] );
                dents[i].Add( letters[j] );
                dees[j].Add( letters[i] );
            }
        }

        // Remove some more
        for ( int i = 0; i < size; i += 2 )
        {
            for ( int j = i + 3; j < size; j += 3 )
            {
                t.RemoveDependency( letters[i], letters[j] );
                dents[i].Remove( letters[j] );
                dees[j].Remove( letters[i] );
            }
        }

        // Make sure everything is right
        for ( int i = 0; i < size; i++ )
        {
            Assert.IsTrue( dents[i].SetEquals( new HashSet<string>( t.GetDependents( letters[i] ) ) ) );
            Assert.IsTrue( dees[i].SetEquals( new HashSet<string>( t.GetDependees( letters[i] ) ) ) );
        }
    }

    /// <summary>
    ///   Increase weight of Stress Test 1.
    /// </summary>
    [TestMethod]
    [Timeout( 2000, CooperativeCancellation = true)]
    [TestCategory( "40" )]
    public void StressTest1_2( )
    {
        StressTest1_1();
    }

    /// <summary>
    ///   Increase weight of Stress Test 1.
    /// </summary>
    [TestMethod]
    [Timeout( 2000, CooperativeCancellation = true)]
    [TestCategory( "41" )]
    public void StressTest1_3( )
    {
        StressTest1_1();
    }

    // ********************************** ANOTHER STRESS TEST, REPEATED ******************** //

    /// <summary>
    ///    Using lots of data with replacement.  FIXME: What does this really mean?.
    ///    FIXME: stress test 15 seems to be almost the same? Why almost? Can they be rectified?
    /// </summary>
    [TestMethod]
    [Timeout( 2000, CooperativeCancellation = true)]
    [TestCategory( "42" )]
    public void StressTest8_1( )
    {
        // Dependency graph
        DependencyGraph t = new();

        // A bunch of strings to use
        const int size = 400;
        string[] letters = new string[size];
        for ( int i = 0; i < size; i++ )
        {
            letters[i] = "a" + i;
        }

        // The correct answers
        HashSet<string>[] dents = new HashSet<string>[size];
        HashSet<string>[] dees = new HashSet<string>[size];
        for ( int i = 0; i < size; i++ )
        {
            dents[i] = [];
            dees[i] = [];
        }

        // Add a bunch of dependencies
        for ( int i = 0; i < size; i++ )
        {
            for ( int j = i + 1; j < size; j++ )
            {
                t.AddDependency( letters[i], letters[j] );
                dents[i].Add( letters[j] );
                dees[j].Add( letters[i] );
            }
        }

        // Remove a bunch of dependencies
        for ( int i = 0; i < size; i++ )
        {
            for ( int j = i + 2; j < size; j += 3 )
            {
                t.RemoveDependency( letters[i], letters[j] );
                dents[i].Remove( letters[j] );
                dees[j].Remove( letters[i] );
            }
        }

        // Replace a bunch of dependents
        for ( int i = 0; i < size; i += 2 )
        {
            HashSet<string> newDents = [];
            for ( int j = 0; j < size; j += 5 )
            {
                newDents.Add( letters[j] );
            }

            t.ReplaceDependents( letters[i], newDents );

            foreach ( string s in dents[i] )
            {
                dees[int.Parse( s[1..] )].Remove( letters[i] );
            }

            foreach ( string s in newDents )
            {
                dees[int.Parse( s[1..] )].Add( letters[i] );
            }

            dents[i] = newDents;
        }

        // Make sure everything is right
        for ( int i = 0; i < size; i++ )
        {
            Assert.IsTrue( dents[i].SetEquals( new HashSet<string>( t.GetDependents( letters[i] ) ) ) );
            Assert.IsTrue( dees[i].SetEquals( new HashSet<string>( t.GetDependees( letters[i] ) ) ) );
        }
    }

    /// <summary>
    ///   Increase weight of StressTest8.
    /// </summary>
    [TestMethod]
    [Timeout( 2000, CooperativeCancellation = true)]
    [TestCategory( "43" )]
    public void StressTest8_2( )
    {
        StressTest8_1();
    }

    /// <summary>
    ///   Increase weight of StressTest8.
    /// </summary>
    [TestMethod]
    [Timeout( 2000, CooperativeCancellation = true)]
    [TestCategory( "44" )]
    public void StressTest10( )
    {
        StressTest8_1();
    }

    // ********************************** A THIRD STRESS TEST, REPEATED ******************** //

    /// <summary>
    ///    Using lots of data with replacement. FIXME: ????.
    /// </summary>
    [TestMethod]
    [Timeout( 2000, CooperativeCancellation = true)]
    [TestCategory( "45" )]
    public void StressTest15_1( )
    {
        // Dependency graph
        DependencyGraph t = new();

        // A bunch of strings to use
        const int size = 1000;
        string[] letters = new string[size];
        for ( int i = 0; i < size; i++ )
        {
            letters[i] = "a" + i;
        }

        // The correct answers
        HashSet<string>[] dents = new HashSet<string>[size];
        HashSet<string>[] dees = new HashSet<string>[size];
        for ( int i = 0; i < size; i++ )
        {
            dents[i] = [];
            dees[i] = [];
        }

        // Add a bunch of dependencies
        for ( int i = 0; i < size; i++ )
        {
            for ( int j = i + 1; j < size; j++ )
            {
                t.AddDependency( letters[i], letters[j] );
                dents[i].Add( letters[j] );
                dees[j].Add( letters[i] );
            }
        }

        for ( int i = 0; i < size; i++ )
        {
            Assert.IsTrue( dents[i].SetEquals( new HashSet<string>( t.GetDependents( letters[i] ) ) ) );
            Assert.IsTrue( dees[i].SetEquals( new HashSet<string>( t.GetDependees( letters[i] ) ) ) );
        }

        // Remove a bunch of dependencies
        for ( int i = 0; i < size; i++ )
        {
            for ( int j = i + 2; j < size; j += 3 )
            {
                t.RemoveDependency( letters[i], letters[j] );
                dents[i].Remove( letters[j] );
                dees[j].Remove( letters[i] );
            }
        }

        for ( int i = 0; i < size; i++ )
        {
            Assert.IsTrue( dents[i].SetEquals( new HashSet<string>( t.GetDependents( letters[i] ) ) ) );
            Assert.IsTrue( dees[i].SetEquals( new HashSet<string>( t.GetDependees( letters[i] ) ) ) );
        }

        // Replace a bunch of dependees
        for ( int i = 0; i < size; i += 2 )
        {
            HashSet<string> newDees = [];
            for ( int j = 0; j < size; j += 9 )
            {
                newDees.Add( letters[j] );
            }

            t.ReplaceDependees( letters[i], newDees );

            foreach ( string s in dees[i] )
            {
                dents[int.Parse( s[1..] )].Remove( letters[i] );
            }

            foreach ( string s in newDees )
            {
                dents[int.Parse( s[1..] )].Add( letters[i] );
            }

            dees[i] = newDees;
        }

        // Make sure everything is right
        for ( int i = 0; i < size; i++ )
        {
            Assert.IsTrue( dents[i].SetEquals( new HashSet<string>( t.GetDependents( letters[i] ) ) ) );
            Assert.IsTrue( dees[i].SetEquals( new HashSet<string>( t.GetDependees( letters[i] ) ) ) );
        }
    }

    /// <summary>
    ///   Increase weight of StressTest15.
    /// </summary>
    [TestMethod]
    [Timeout( 2000, CooperativeCancellation = true)]
    [TestCategory( "46" )]
    public void StressTest15_2( )
    {
        StressTest15_1();
    }

    /// <summary>
    ///   Increase weight of StressTest15.
    /// </summary>
    [TestMethod]
    [Timeout( 2000, CooperativeCancellation = true)]
    [TestCategory( "47" )]
    public void StressTest17( )
    {
        StressTest15_1();
    }

    /// <summary>
    ///   Helper code to build a simple dependency graph.
    /// </summary>
    /// <returns> The new graph. </returns>
    private static DependencyGraph SetupComplexDependencies( )
    {
        DependencyGraph t = new();
        t.AddDependency( "x", "b" );
        t.AddDependency( "a", "z" );
        t.ReplaceDependents( "b", [] );
        t.AddDependency( "y", "b" );
        t.ReplaceDependents( "a", ["c"] );
        t.AddDependency( "w", "d" );
        t.ReplaceDependees( "b", ["a", "c"] );
        t.ReplaceDependees( "d", ["b"] );
        return t;
    }
}

/// <summary>
///   Helper methods for the tests above.
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    ///   Check to see if the two "sets" (source and items) match, i.e.,
    ///   contain exactly the same values.
    /// </summary>
    /// <param name="source"> original container.</param>
    /// <param name="items"> elements to match against.</param>
    /// <returns> true if every element in source is in items and vice versa. They are the "same set".</returns>
    public static bool Matches( this IEnumerable<string> source, params string[] items )
    {
        var enumerable = source.ToList();
        return (enumerable.Count() == items.Length) && items.All( item => enumerable.Contains( item ) );
    }
}
