using System.Collections;
using System.Collections.Generic;
using FolvosLibrary.WFC;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class IWFCCellTests
{
	WFCManager_2D manager;
	BeachImporter importer;
	BeachWFCExporter exporter;
	IWFCPosition size = new IWFCPosition(3, 3);

	[OneTimeSetUp]
	public void TestSetup()
	{
		manager = (WFCManager_2D)ScriptableObject.CreateInstance(typeof(WFCManager_2D));
		importer = (BeachImporter)ScriptableObject.CreateInstance(typeof(BeachImporter));
		exporter = (BeachWFCExporter)ScriptableObject.CreateInstance(typeof(BeachWFCExporter));

		manager.SetImporter(importer);
		manager.SetExporter(exporter);
		manager.SetSize(size);
		// manager.Initialize();
		//Needed?
	}

	[Test]
	public void RegularConstructorWorks()
	{
		IWFCCell inital = new IWFCCell(manager, CellInitialPosition);
		Assert.That(inital.GetPosition().AsVector2Int() == CellInitialPosition.AsVector2Int(), "Cell initial position is not being set correctly");
	}

	IWFCPosition CellInitialPosition = new IWFCPosition(1, 1);
	[Test]
	public void CopyConstructorWorks()
	{
		IWFCCell inital = new IWFCCell(manager, CellInitialPosition);
		Assert.That(inital.GetPosition().AsVector2Int() == CellInitialPosition.AsVector2Int(), "Cell initial position is not being set correctly. !!Check Regular Constructor!!");
		IWFCCell clone = new IWFCCell(inital);
		Assert.That(clone.GetPosition().AsVector2Int() == CellInitialPosition.AsVector2Int(), "Cloned Cell position is not being set correctly.");
		// Assert.That(clone.GetPosition().GetHashCode() != CellInitialPosition.GetHashCode(), "Cloned Cell position is a reference to the initial cell position");
		// IWFCPosition clonePos = clone.GetPosition();

		// clonePos.y = 2;

		// Assert.That(clone.GetPosition().AsVector2Int() == new Vector2Int(1, 2), $"Modifying clone position unsuccessful, result {clone.GetPosition().AsVector2Int()} != (1,2)");
	}
}
