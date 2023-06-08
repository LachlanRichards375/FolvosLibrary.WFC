using System.Collections;
using System.Collections.Generic;
using FolvosLibrary.WFC;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class IWFCCellTests : WFCTests
{
	[OneTimeSetUp]
	public void TestSetup()
	{
		manager = GetManager();
	}

	[Test]
	public void RegularConstructorWorks()
	{
		IWFCCell inital = new IWFCCell(manager, CellInitialPosition);
		Assert.That(inital.GetPosition().AsVector2Int() == CellInitialPosition.AsVector2Int(), "Cell initial position is not being set correctly");
	}

	[Test]
	public void Test_CellHasDomain(){	
        Assert.That(manager.GetCell(new IWFCPosition(0,0)).Domain.Count == 3, "Domain not initialized properly On Cells");
        Assert.That(manager.GetCell(new IWFCPosition(2,2)).Domain.Count == 3, "Domain not initialized properly On Cells");
   	}

	IWFCPosition CellInitialPosition = new IWFCPosition(1, 1);
	[Test]
	public void CopyConstructorWorks()
	{
		IWFCCell inital = new IWFCCell(manager, CellInitialPosition);
		Assert.That(inital.GetPosition().AsVector2Int() == CellInitialPosition.AsVector2Int(), "Cell initial position is not being set correctly. !!Check Regular Constructor!!");
		IWFCCell clone = new IWFCCell(inital);
		Assert.That(clone.GetPosition().AsVector2Int() == CellInitialPosition.AsVector2Int(), "Cloned Cell position is not being set correctly.");
		Assert.That(!ReferenceEquals(CellInitialPosition, clone.GetPosition()), "Cloned Cell position is a reference to the initial cell position");
		
		IWFCPosition clonePos = clone.GetPosition();

		clonePos.y = 2;

		Assert.That(clone.GetPosition() == CellInitialPosition, $"Should not be able to modify the cell position, result {clone.GetPosition().AsVector2Int()} != {CellInitialPosition}");
	}

	public void Test_InitializedTileRuleNotReferenceToOriginal(){
		IWFCCell initial = new IWFCCell(manager, CellInitialPosition);
		initial.Domain = new List<WFCTile>(GetDomain());
		initial.RuleSetup();

		Assert.That(!ReferenceEquals(initial.Domain[0], GetDomain()[0]), "Initialized Tile is a reference to original Domain");
		Assert.That(!ReferenceEquals(initial.Domain[0].Rules[0], GetDomain()[0].Rules[0]), "Initialized Tile Rule is a reference to original rules");
	}
}
