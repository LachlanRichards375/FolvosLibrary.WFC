using System;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace FolvosLibrary.WFC
{
	[CreateAssetMenu(menuName = "Folvos/WFC/CollapseMethods/Centralized Collapse Method"), System.Serializable]
	public class WFCCentralisedCollapseMethod : IWFCCollapseMethod
	{
		Dictionary<IWFCPosition, List<IWFCCell>> toAlert = new Dictionary<IWFCPosition, List<IWFCCell>>();
		List<WFCCellUpdate> updateQueue = new List<WFCCellUpdate>();
		int maximumThreadCount = 1;

		public override void Collapse(IWFCPosition position)
		{
			updateQueue.Add(manager.GetCell(position).Collapse());

			while (updateQueue.Count > 0)
			{
				WFCCellUpdate updateBeingProcessed = updateQueue[0];
				updateQueue.RemoveAt(0);

				IWFCPosition cellUpdatePos = updateBeingProcessed.UpdatedCell.GetPosition();

				if (!toAlert.ContainsKey(cellUpdatePos))
				{
					continue;
				}

				List<IWFCCell> listOfAlertees = toAlert[cellUpdatePos];
				Thread[] threadList = new Thread[listOfAlertees.Count];
				Semaphore threadWrite = new Semaphore(maximumThreadCount, maximumThreadCount);
				for (int i = 0; i < listOfAlertees.Count; i++)
				{
					threadList[i] = new Thread(ThreadedLoop);
					threadList[i].Name = $"{listOfAlertees[i].GetPosition()}";
					threadList[i].Start((new ThreadData(listOfAlertees[i], threadWrite, updateBeingProcessed)));
				}

				foreach (Thread t in threadList)
				{
					t.Join();
				}
			}
		}

		class ThreadData
		{
			public ThreadData(IWFCCell toAlert, Semaphore writeAccess, WFCCellUpdate update)
			{
				this.toAlert = toAlert;
				this.threadWrite = writeAccess;
				this.update = update;
			}
			public IWFCCell toAlert;
			public Semaphore threadWrite;
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
			data.threadWrite.WaitOne();
			if (update.HasValue)
			{
				updateQueue.Add(update.Value);
			}
			data.threadWrite.Release();
		}

		public override void CollapseSpecificCell(IWFCPosition position, WFCTile toCollapseTo)
		{
			updateQueue.Add(manager.GetCell(position).Collapse(toCollapseTo));
		}

		public override void RegisterForCellUpdates(IWFCPosition positionOfInterest, IWFCCell toRegister)
		{
			if (!toAlert.ContainsKey(positionOfInterest))
			{
				toAlert.Add(positionOfInterest, new List<IWFCCell>());
			}

			List<IWFCCell> addTo = toAlert[positionOfInterest];

			if (addTo.Contains(toRegister))
			{
				return;
			}

			addTo.Add(toRegister);
		}

		public override void DeRegisterForCellUpdates(IWFCPosition positionOfInterest, IWFCCell toDeregister)
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