
using UnityEngine;
namespace FolvosLibrary.WFC
{
	public abstract partial class IWFCManager : ScriptableObject
	{
		public virtual void Generate()
		{
			try
			{
				while (grid.RemainingCellsToCollapse() > 0)
				{
					GenerateOnce();
				}
				InvokeOnResult();
			}
			catch (ImpossibleDomainException e)
			{
				InvokeOnError();
			}
		}
		public virtual void GenerateStep(int step = 1)
		{
			try
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
			catch (ImpossibleDomainException e)
			{
				InvokeOnError();
			}
		}
		public virtual async System.Threading.Tasks.Task GenerateTimeLapse(System.Threading.CancellationTokenSource cancellationToken, int millsBetweenStep)
		{
			try
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
			catch (ImpossibleDomainException e)
			{
				InvokeOnError();
			}
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