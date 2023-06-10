using FolvosLibrary.WFC;
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class CellIsNotRule : WFCTests
{
    [OneTimeSetUp]
    public void TestSetup()
    {
        manager = GetManager();
    }

    [Test]
    public void Test_CreateEmptyRule_MultiCellIsNot()
    {
        MultiCellIsNotTarget2D rule = new MultiCellIsNotTarget2D();
        Assert.That(!(rule is null), "Rule was not created successfully");
    }

    [Test]
    public void Test_CreateRuleCopy_MultiCellIsNot()
    {
        MultiCellIsNotTarget2D ruleToCopy = new MultiCellIsNotTarget2D();
        SetFlag(ref ruleToCopy.direction, CellDirection.Direction.West);
        ruleToCopy.goal = GetGrass(); //Grass

        MultiCellIsNotTarget2D copy = new MultiCellIsNotTarget2D(ruleToCopy);

        Assert.That(!ReferenceEquals(ruleToCopy, copy), "Copied object is a reference to the initial object");

        Assert.That(copy.goal == ruleToCopy.goal, "Copied rule does not have the same goal as the original");
        Assert.That(copy.direction == ruleToCopy.direction, "Copied rule does not have the same direction goal as the original");
    }

    IWFCCell FirstCell;
    IWFCCell SecondCell;
    [SetUp]
    public void Setup()
    {
        FirstCell = manager.GetCell(new IWFCPosition(0, 0));
        SecondCell = manager.GetCell(new IWFCPosition(0, 1));
        FirstCell.RuleSetup();
        SecondCell.RuleSetup();
    }

    [Test]
    public void Test_RulePassesCorrectly()
    {
        Assert.That(manager.CollapseSpecificCell(new IWFCPosition(0, 0), GetGrass()) == null, "Unable to collapse First cell");

        //Collapse cell to grass in position 0,0
        //Water Rule in position 0,1 should fail
    }

    [Test]
    public void Test_TargetCellsArray()
    {
        MultiCellIsNotTarget2D FirstRule = (FirstCell.Domain[0].Rules[0] as MultiCellIsNotTarget2D);
        Vector2Int[] FirstTargetCellsArray = FirstRule.targetCells;
        MultiCellIsNotTarget2D SecondRule = (SecondCell.Domain[SecondCell.Domain.Count - 1].Rules[0] as MultiCellIsNotTarget2D);
        Vector2Int[] SecondTargetCellsArray = SecondRule.targetCells;

        Assert.That(!ReferenceEquals(FirstCell, SecondCell), "Cells are the same object at position (0,0) and (1,1)");
        Assert.That(!ReferenceEquals(FirstCell.Domain[0], SecondCell.Domain[0]), "Tiles in the cells are the same objects");

        //Test first cell targeting correct
        Assert.That(FirstTargetCellsArray.Length == 3, $"Cell {FirstCell.GetPositionString()} is targeting ({FirstRule.GetTargetCells()}), not ((0,1), (1,1), (1,0))");

        //Test second cell targeting correct
        Assert.That(SecondTargetCellsArray.Length == 5, $"Cell {SecondCell.GetPositionString()} is targeting ({SecondRule.GetTargetCells()}), not ((0,2), (1,2), (1,1), (1,0), (0,0))");

    }
}
