using System;
using UnityEngine;
namespace FolvosLibrary.WFC
{
	public abstract partial class IWFCManager : ScriptableObject
	{
		//Lifecycle
		public event Action OnInitialize;
		public event Action OnResult;
		public event Action<WFCError> OnError;
		public event Action OnCleanup;

		protected void InvokeOnInitialize()
		{
			OnInitialize?.Invoke();
		}

		protected void InvokeOnResult()
		{
			OnResult?.Invoke();
		}

		protected void InvokeOnError(WFCError error)
		{
			OnError?.Invoke(error);
		}

		protected void InvokeOnCleanup()
		{
			OnCleanup?.Invoke();
		}
	}
}