using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FolvosLibrary.WFC
{
	public interface IWFCCell
	{
		public void Collapse();
		public bool HasCollapsed();
		public float CalculateEntropy();
		public event Action<WFCCellUpdate> OnCellUpdate;
	}
}