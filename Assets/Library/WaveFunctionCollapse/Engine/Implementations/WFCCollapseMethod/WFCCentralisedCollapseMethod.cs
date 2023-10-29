using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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

		public override void Collapse(WFCPosition position)
		{
			updateQueue.Enqueue(manager.GetCell(position).Collapse());
			countInQueue++;

			if (threadList.Length == 0)
			{
				threadList = new Task[maximumThreadCount];
				for (int i = 0; i < maximumThreadCount; i++)
				{
					threadList[i] = Task.Run(numberedThreadLoop);
				}
			}

			while (countInQueue > 0) {/*Loop until threads are finished*/}
		}
		Action numberedThreadLoop()
		{
			while (true)
			{
				while (countInQueue > 0)
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
					else
					{
						Thread.Sleep(100);
					}
				}
				Thread.Sleep(100);
			}
		}

		public override void CollapseSpecificCell(WFCPosition position, WFCTile toCollapseTo)
		{
			updateQueue.Enqueue(manager.GetCell(position).Collapse(toCollapseTo));
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

		public override void DrawOptions()
		{
			maximumThreadCount = EditorGUILayout.IntField("Simultaneous Thread Count", maximumThreadCount);
			if (maximumThreadCount <= 0)
			{
				maximumThreadCount = 1;
			}
		}
	}
}