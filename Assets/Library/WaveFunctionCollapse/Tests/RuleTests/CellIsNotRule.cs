using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using FolvosLibrary.WFC;

public class CellIsNotRule : WFCTests
{
    [OneTimeSetUp]
	public void TestSetup()
	{
		manager = GetManager();
	}

    [Test]
    public void Test_CreateEmptyRule_MultiCellIsNot(){
        MultiCellIsNotTarget2D rule = new MultiCellIsNotTarget2D();
        Assert.That(!(rule is null), "Rule was not created successfully");
    }

    [Test]
    public void Test_CreateRuleCopy_MultiCellIsNot(){
        MultiCellIsNotTarget2D ruleToCopy = new MultiCellIsNotTarget2D();
        SetFlag(ref ruleToCopy.direction, CellDirection.Direction.West);
        ruleToCopy.goal = GetGrass(); //Grass

        MultiCellIsNotTarget2D copy = new MultiCellIsNotTarget2D(ruleToCopy);

        Assert.That(!ReferenceEquals(ruleToCopy, copy), "Copied object is a reference to the initial object");

        Assert.That(copy.goal == ruleToCopy.goal, "Copied rule does not have the same goal as the original");
        Assert.That(copy.direction == ruleToCopy.direction, "Copied rule does not have the same direction goal as the original");
    }
}
