using System.Collections;
using System.Collections.Generic;
using FolvosLibrary.WFC;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class IWFCManagerTests : WFCTests
{
	WFCManager_2D manager;
	BeachImporter importer;
	BeachWFCExporter exporter;
	IWFCPosition size = new IWFCPosition(3, 3);
	IWFCPosition SmallerLengthSize = new IWFCPosition(1, 3);

	[OneTimeSetUp]
	public void TestSetup()
	{
		manager = (WFCManager_2D)ScriptableObject.CreateInstance(typeof(WFCManager_2D));
		importer = (BeachImporter)ScriptableObject.CreateInstance(typeof(BeachImporter));
		exporter = (BeachWFCExporter)ScriptableObject.CreateInstance(typeof(BeachWFCExporter));

		importer.returner = GetDomain();
	}

	[Test]
	public void TestAssignImporterToManager()
	{
		manager.SetImporter(importer);
		Assert.That(manager.GetImporter() == importer, "Assigned importer is not the one provided");
	}

	[Test]
	public void TestAssignExporterToManager()
	{
		manager.SetExporter(exporter);
		Assert.That(manager.GetExporter() == exporter, "Assigned exporter is not the one provided");
	}

	[Test]
	public void TestSetManagerSize()
	{
		manager.SetSize(size);
		Assert.That(manager.GetSize() == size, "Manager did not set size correctly");
	
		manager.SetSize(SmallerLengthSize);
		Assert.That(manager.GetSize() == SmallerLengthSize, "Manager did not set size with different lengths correctly");
	}

	[Test]
	public void TestInitializeCells()
	{
		manager.SetImporter(importer);
		manager.SetExporter(exporter);
		manager.SetSize(size);
		manager.Initialize();
		Assert.That(manager.GetCell(new IWFCPosition(0, 0)) != null, "Manager did not initialize cells");
		Assert.That(manager.GetCell(new IWFCPosition(size.x - 1, size.y - 1)) != null, $"Manager did not initialize {size.x}*{size.y} cells");
	}
}
