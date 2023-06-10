using UnityEngine;

namespace FolvosLibrary.Logging
{
	public static partial class Logging
	{
		public static Priority LoggingLevel = Priority.Mid;

		public enum Priority
		{
			Low = 0,
			Mid = 1,
			High = 2,
			Critical = 3,
		}

		public static ProjectGroups LoggingGroups;

		public struct LogMessage
		{
			public string Message;
			public Priority Priority;
			public ProjectGroups MessageFrom;

			public LogMessage(string message, Priority priority = Priority.Low, ProjectGroups[] messageFrom = null)
			{
				Message = message;
				Priority = priority;
				MessageFrom = new ProjectGroups();
				if (messageFrom != null)
				{
					for (int i = 0; i < messageFrom.Length; i++)
					{
						FlagsHelper.Set(ref MessageFrom, messageFrom[i]);
					}
				}
			}

			public override string ToString()
			{
				return Message;
			}
		}

		static bool canLog(LogMessage message)
		{

			if (message.Priority < LoggingLevel)
			{
				return false;
			}

			if (!FlagsHelper.IsSet(message.MessageFrom, LoggingGroups))
			{
				return false;
			}

			return true;
		}

		public static void Message(LogMessage message)
		{
			if (canLog(message))
				Debug.Log(message);
		}

		public static void Warning(LogMessage message)
		{
			if (canLog(message))
				Debug.LogWarning(message);
		}

		public static void Error(LogMessage message)
		{
			if (canLog(message))
				Debug.LogError(message);
		}
	}

	public static class FlagsHelper
	{
		public static bool IsSet<T>(T flags, T flag) where T : struct
		{
			int flagsValue = (int)(object)flags;
			int flagValue = (int)(object)flag;

			return (flagsValue & flagValue) != 0;
		}

		public static void Set<T>(ref T flags, T flag) where T : struct
		{
			int flagsValue = (int)(object)flags;
			int flagValue = (int)(object)flag;

			flags = (T)(object)(flagsValue | flagValue);
		}

		public static void Unset<T>(ref T flags, T flag) where T : struct
		{
			int flagsValue = (int)(object)flags;
			int flagValue = (int)(object)flag;

			flags = (T)(object)(flagsValue & (~flagValue));
		}
	}
}