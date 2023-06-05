using System.Collections;
using System.Collections.Generic;
using FolvosLibrary.WFC;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class IWFCManagerTests
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
	}
}
