using System.Collections;
using System.Collections.Generic;
using FolvosLibrary.Logging;
using UnityEngine;

public class LoggingTest : MonoBehaviour
{
	// Start is called before the first frame update
	void Start()
	{
		FlagsHelper.Set(ref Logging.LoggingGroups, Logging.ProjectGroups.Test);
		Logging.Message(new Logging.LogMessage(("Debug"), Logging.Priority.Critical, new Logging.ProjectGroups[] { Logging.ProjectGroups.Debug }));
		Logging.Message(new Logging.LogMessage(("Test"), Logging.Priority.Critical, new Logging.ProjectGroups[] { Logging.ProjectGroups.Test }));
		Logging.Message(new Logging.LogMessage(("Test Debug"), Logging.Priority.Critical, new Logging.ProjectGroups[] { Logging.ProjectGroups.Test, Logging.ProjectGroups.Debug }));
	}
}
