using System.Collections;
using System.Collections.Generic;
using FolvosLibrary.WFC;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class IWFCManagerTests : WFCTests
{

	[OneTimeSetUp]
	public void TestSetup()
	{
		manager = GetManager();
	}

	[Test]
	public void TestAssignImporterToManager()
	{
		IWFCImporter importer = (BeachImporter)ScriptableObject.CreateInstance(typeof(BeachImporter));
		manager.SetImporter(importer);
		Assert.That(manager.GetImporter() == importer, "Assigned importer is not the one provided");
	}

	[Test]
	public void TestAssignExporterToManager()
	{
		IWFCExporter exporter = (BeachWFCExporter)ScriptableObject.CreateInstance(typeof(BeachWFCExporter));
		Debug.Log($"manager is null? {manager is null} exporter is null? {exporter is null}");
		manager.SetExporter(exporter);
		Assert.That(manager.GetExporter() == exporter, "Assigned exporter is not the one provided");
	}

	IWFCPosition SmallerLengthSize = new IWFCPosition(1, 3);
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
