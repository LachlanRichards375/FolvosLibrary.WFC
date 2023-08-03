
using UnityEngine;
namespace FolvosLibrary.WFC
{
	public abstract partial class IWFCManager : ScriptableObject
	{
		public virtual void Generate()
		{
			while (grid.RemainingCellsToCollapse() > 0)
			{
				GenerateOnce();
			}
			InvokeOnResult();
		}
		public virtual void GenerateStep(int step = 1)
		{
			for (int i = 0; i < step && grid.RemainingCellsToCollapse() > 0; i++)
			{
				GenerateOnce();
			}

			if (grid.RemainingCellsToCollapse() > 0)
			{
				UpdateOutput();
			}

			if (grid.RemainingCellsToCollapse() <= 0)
			{
				InvokeOnResult();
			}
		}
		public virtual async System.Threading.Tasks.Task GenerateTimeLapse(System.Threading.CancellationTokenSource cancellationToken, int millsBetweenStep)
		{
			while (grid.RemainingCellsToCollapse() > 0)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					break;
				}
				GenerateOnce();
				UpdateOutput();
				await System.Threading.Tasks.Task.Delay(millsBetweenStep);
			}
			if (grid.RemainingCellsToCollapse() > 0)
			{
				//Cancled early
			}
			InvokeOnResult();
		}

		public virtual bool DrawSize(bool ForceReset = false)
		{
			if (grid == null)
			{
				return false;
			}
			grid.DrawSize(ForceReset);
			return true;
		}

	}
}