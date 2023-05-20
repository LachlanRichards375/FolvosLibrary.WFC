using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FolvosLibrary.Logging
{
	public static partial class Logging
	{
		[System.Flags]
		public enum ProjectGroups
		{
			Debug = 1,
			Test = 2,
		}
	}
}