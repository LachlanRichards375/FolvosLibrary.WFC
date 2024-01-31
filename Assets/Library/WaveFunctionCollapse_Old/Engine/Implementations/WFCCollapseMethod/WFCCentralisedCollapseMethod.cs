using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Profiling;
using UnityEditor;
using UnityEngine;

namespace FolvosLibrary.WFC
{
	[CreateAssetMenu(menuName = "Folvos/WFC/CollapseMethods/Centralized Collapse Method"), System.Serializable]
	public class WFCCentralisedCollapseMethod : IWFCCollapseMethod
	{
		ConcurrentDictionary<WFCPosition, List<WFCCell>> toAlert = new ConcurrentDictionary<WFCPosition, List<WFCCell>>();
		ConcurrentQueue<WFCCellUpdate> updateQueue = new ConcurrentQueue<WFCCellUpdate>();
		int maximumThreadCount = 1;
		Task[] threadList = new Task[0];
		int countInQueue = 0;

		bool continueWorkFlag = true;
		Exception workerException = null;
		static ProfilerMarker profilerMarker = new ProfilerMarker("CentralisedCollapse.Collapse");

		public override void Initalize(IWFCManager manager)
		{
			manager.OnResult += () => continueWorkFlag = false;
			maximumThreadCount = manager.MaxThreadCount;
		}

		public override void Collapse(WFCPosition position)
		{
			profilerMarker.Begin();
			Enqueue(position);

			while (countInQueue > 0)
			{
				/*Loop until threads are finished or exception thrown*/
				if (workerException != null)
				{
					throw workerException;
				}
			}
			profilerMarker.End();
		}

		void NumberedThreadLoop()
		{
			while (continueWorkFlag)
			{
				while (countInQueue > 0 && continueWorkFlag)
				{
					try
					{

						if (updateQueue.TryDequeue(out WFCCellUpdate updateBeingProcessed))
						{
							countInQueue--;
							WFCPosition cellUpdatePos = updateBeingProcessed.UpdatedCell.GetPosition();
							//if no one cares about this cell ignore it.
							if (!toAlert.ContainsKey(cellUpdatePos)) { continue; }
							List<WFCCell> listOfAlertees = toAlert[cellUpdatePos];
							for (int i = 0; i < listOfAlertees.Count; i++)
							{
								WFCCellUpdate? update = listOfAlertees[i].DomainCheck(updateBeingProcessed);
								if (update.HasValue)
								{
									updateQueue.Enqueue(update.Value);
									countInQueue++;
								}
							}
						}
					}
					catch (Exception e)
					{
						workerException = e;
					}
				}
			}
		}

		public override void CollapseSpecificCell(WFCPosition position, WFCTile toCollapseTo)
		{
			Enqueue(position, toCollapseTo);
		}

		void Enqueue(WFCPosition position, WFCTile toCollapseTo = null)
		{
			if (toCollapseTo != null)
			{
				updateQueue.Enqueue(manager.GetCell(position).Collapse(toCollapseTo));
			}
			else
			{
				updateQueue.Enqueue(manager.GetCell(position).Collapse());
			}

			countInQueue++;

			if (threadList.Length == 0)
			{
				threadList = new Task[maximumThreadCount];
				for (int i = 0; i < maximumThreadCount; i++)
				{
					threadList[i] = Task.Run(NumberedThreadLoop);
				}
			}
		}

		public override void RegisterForCellUpdates(WFCPosition positionOfInterest, WFCCell toRegister)
		{
			if (!toAlert.ContainsKey(positionOfInterest))
			{
				toAlert.TryAdd(positionOfInterest, new List<WFCCell>());
			}

			List<WFCCell> addTo = toAlert[positionOfInterest];

			if (addTo.Contains(toRegister))
			{
				return;
			}

			addTo.Add(toRegister);
		}

		public override void DeRegisterForCellUpdates(WFCPosition positionOfInterest, WFCCell toDeregister)
		{
			if (!toAlert.ContainsKey(positionOfInterest))
			{
				return;
			}

			toAlert[positionOfInterest].Remove(toDeregister);
		}

		public override void Reset()
		{
			threadList = new Task[0];
			workerException = null;

			toAlert = new ConcurrentDictionary<WFCPosition, List<WFCCell>>();
			updateQueue = new ConcurrentQueue<WFCCellUpdate>();
			continueWorkFlag = true;
		}
	}
}