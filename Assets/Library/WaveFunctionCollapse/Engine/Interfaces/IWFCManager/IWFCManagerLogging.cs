using UnityEngine;
namespace FolvosLibrary.WFC
{
	public abstract partial class IWFCManager : ScriptableObject
	{
		//Logging
		public abstract void PrintCells();


		public void PrintGenerationDivider()
		{
			Logging.Logging.LogMessage message = new Logging.Logging.LogMessage();

			message.MessageFrom = Logging.Logging.ProjectGroups.WFCManager;
			message.Priority = Logging.Logging.Priority.Low;
			message.Message = "|*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*|";
			Logging.Logging.Message(message);
		}

		//For some reason it needs Package.Class.Static method()
		public void PrintEntropyQueue()
		{
			Logging.Logging.LogMessage message = new Logging.Logging.LogMessage();

			message.MessageFrom = Logging.Logging.ProjectGroups.WFCManager;
			message.Priority = Logging.Logging.Priority.Low;

			string s = $"> Entropy Queue: \n";

			int i = 0;
			foreach (IWFCCell cell in EntropyQueue)
			{
				s += i + ">\t" + cell.GetPosition() + " " + cell.ToString() + "\n";
				i++;
			}

			message.Message = s;
			Logging.Logging.Message(message);
		}
	}
}