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
		ConcurrentDictionary<WFCPosition, List<IWFCCell>> toAlert = new ConcurrentDictionary<WFCPosition, List<IWFCCell>>();
		ConcurrentQueue<WFCCellUpdate> updateQueue = new ConcurrentQueue<WFCCellUpdate>();
		int maximumThreadCount = 1;
		Thread[] threadList = new Thread[0];

		public override void Collapse(WFCPosition position)
		{
			updateQueue.Enqueue(manager.GetCell(position).Collapse());

			while (updateQueue.Count > 0)
			{
				updateQueue.TryDequeue(out WFCCellUpdate updateBeingProcessed);

				WFCPosition cellUpdatePos = updateBeingProcessed.UpdatedCell.GetPosition();

				//No one cares about this cell
				if (!toAlert.ContainsKey(cellUpdatePos))
				{
					continue;
				}

				List<IWFCCell> listOfAlertees = toAlert[cellUpdatePos];

				Task[] tasks = new Task[listOfAlertees.Count];
				for (int i = 0; i < listOfAlertees.Count; i++)
				{
					int localInt = i;
					tasks[i] = Task.Run(() => ThreadedLoop(new ThreadData(listOfAlertees[localInt], updateBeingProcessed)));
				}
				Task.WaitAll(tasks);
			}
		}
		class ThreadData
		{
			public ThreadData(IWFCCell toAlert, WFCCellUpdate update)
			{
				this.toAlert = toAlert;
				this.update = update;
			}
			public IWFCCell toAlert;
			public WFCCellUpdate update;
		}

		// public void ThreadedLoop(ThreadData data)
		void ThreadedLoop(object obj)
		{
			ThreadData data = obj as ThreadData;
			if (data == null)
			{
				Debug.LogError("ThreadedLoop was passed a null object");
				return;
			}

			WFCCellUpdate? update = data.toAlert.DomainCheck(data.update);
			if (update.HasValue)
			{
				updateQueue.Enqueue(update.Value);
			}
		}

		public override void CollapseSpecificCell(WFCPosition position, WFCTile toCollapseTo)
		{
			updateQueue.Enqueue(manager.GetCell(position).Collapse(toCollapseTo));
		}

		public override void RegisterForCellUpdates(WFCPosition positionOfInterest, IWFCCell toRegister)
		{
			if (!toAlert.ContainsKey(positionOfInterest))
			{
				toAlert.TryAdd(positionOfInterest, new List<IWFCCell>());
			}

			List<IWFCCell> addTo = toAlert[positionOfInterest];

			if (addTo.Contains(toRegister))
			{
				return;
			}

			addTo.Add(toRegister);
		}

		public override void DeRegisterForCellUpdates(WFCPosition positionOfInterest, IWFCCell toDeregister)
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