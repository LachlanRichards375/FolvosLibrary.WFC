using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FolvosLibrary.WFC
{
	public interface IWFCManager
	{

		public void SetImporter(IWFCImporter importer);
		public void SetExporter(IWFCExporter exporter);
		public WFCError? Collapse();

		public bool HasInitialized();

		public void Initialize();
		public void Generate();
		public void GenerateStep(int step = 1);
		public void Cleanup();

		public WFCTile[] GetDomain();
		public void ClearQueue();


		//Lifecycle
		public event Action OnInitialize;
		public event Action OnResult;
		public event Action<WFCError> OnError;
		public event Action OnCleanup;

		//EditorWindow
		public void DrawSize();

		//Logging
		public void PrintCells();
	}
}