using UnityEngine;
namespace FolvosLibrary.WFC
{
	public abstract partial class IWFCManager : ScriptableObject
	{
		//Logging
		public virtual void PrintCells()
		{
			grid.PrintCells();
		}


		public void PrintGenerationDivider()
		{
			Logging.Logging.LogMessage message = new Logging.Logging.LogMessage();

			message.MessageFrom = Logging.Logging.ProjectGroups.WFCManager;
			message.Priority = Logging.Logging.Priority.Low;
			message.Message = "|*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*|";
			Logging.Logging.Message(message);
		}
	}
}