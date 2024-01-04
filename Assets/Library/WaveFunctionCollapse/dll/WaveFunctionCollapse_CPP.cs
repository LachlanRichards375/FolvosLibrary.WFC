
using System;
using System.Runtime.InteropServices;
using FolvosLibrary.WFC;

public class WaveFunctionCollapse_CPP
{
	#region C++ Imports
	[DllImport("WaveFunctionCollapse.dll")]
	static extern IntPtr Threaded2DCollapse_Create();

	[DllImport("WaveFunctionCollapse.dll")]
	static extern IntPtr Grid2D_Create(IntPtr position);

	[DllImport("WaveFunctionCollapse.dll")]
	static extern IntPtr WFCPosition_Create2D(uint x, uint y);
	[DllImport("WaveFunctionCollapse.dll")]
	static extern IntPtr WFCPosition_Create3D(uint x, uint y, uint z);
	[DllImport("WaveFunctionCollapse.dll")]
	static extern IntPtr WFCPosition_Create4D(uint x, uint y, uint z, uint w);

	[DllImport("WaveFunctionCollapse.dll")]
	static extern IntPtr IWFCManager_Create(IntPtr collapseMethod, IntPtr grid, short threadCount);

	[DllImport("WaveFunctionCollapse.dll")]
	static extern void AddTileToDomain(ulong tilesToAdd);

	[DllImport("WaveFunctionCollapse.dll")]
	static extern int IWFCManager_Collapse(IntPtr manager, ulong toCollapseTo, IntPtr position);
	[DllImport("WaveFunctionCollapse.dll")]
	static extern int IWFCManager_Run(IntPtr manager);
	[DllImport("WaveFunctionCollapse.dll")]
	static extern ulong[] IWFCManager_GetResult(IntPtr manager, IntPtr length);

	[DllImport("WaveFunctionCollapse.dll")]
	static extern void WFCRule_Add_CellIsNot(ulong tile, ulong goal, uint localTargetCount, WFCPosition[] localTargets);
	#endregion

	public struct CellIsNotRule
	{
		public ulong tile;
		public ulong goal;
		public WFCPosition[] localTargets;
	}

	IntPtr manager;

	public WaveFunctionCollapse_CPP Create2DWFC(WFCPosition size)
	{
		IntPtr collapse = Threaded2DCollapse_Create();
		IntPtr grid = Grid2D_Create(WFCPositionToIntPtr(size));
		manager = IWFCManager_Create(collapse, grid, 12);
		return this;
	}

	public WaveFunctionCollapse_CPP AddCellIsNotRule(CellIsNotRule ruleToAdd)
	{
		WFCRule_Add_CellIsNot(ruleToAdd.tile, ruleToAdd.goal, (uint)ruleToAdd.localTargets.Length, ruleToAdd.localTargets);
		return this;
	}

	public WaveFunctionCollapse_CPP AddTilesToDomain(ulong tilesToAdd)
	{
		AddTileToDomain(tilesToAdd);
		return this;
	}

	public WaveFunctionCollapse_CPP CollapseSpecificCell(ulong toCollapseTo, WFCPosition position)
	{
		if (manager == IntPtr.Zero)
		{
			//This should throw an error and make me realise something's wrong
			return null;
		}
		IWFCManager_Collapse(manager, toCollapseTo, WFCPositionToIntPtr(position));
		return this;
	}

	public WaveFunctionCollapse_CPP RunGenerator()
	{
		if (manager == IntPtr.Zero)
		{
			//This should throw an error and make me realise something's wrong
			return null;
		}
		try
		{
			IWFCManager_Run(manager);
		}
		catch (Exception e)
		{
		}
		return this;
	}

	// public ulong[] Results => IWFCManager_GetResult(manager, new IntPtr());

	public ulong[] GetResults()
	{
		int test = 0;
		return IWFCManager_GetResult(manager, new IntPtr(test));

	}

	IntPtr WFCPositionToIntPtr(WFCPosition toConvert)
	{
		if (toConvert.IsVector4())
		{
			return WFCPosition_Create4D((uint)toConvert.x, (uint)toConvert.y, (uint)toConvert.z, (uint)toConvert.w);
		}
		else if (toConvert.IsVector3())
		{
			return WFCPosition_Create3D((uint)toConvert.x, (uint)toConvert.y, (uint)toConvert.z);
		}
		else
		{
			return WFCPosition_Create2D((uint)toConvert.x, (uint)toConvert.y);
		}
	}
}
