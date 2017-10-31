using System;
using System.Collections.Generic;
using System.Text;
using Krista.FM.Client.TreeLogger.Properties;

namespace Krista.FM.Client.TreeLogger
{
	internal static class LoggerUtils
	{
		internal static string OpStatusToString(OperationStatus status)
		{
			switch (status)
			{
				case OperationStatus.Waiting:
					return Resources.opStatusWaiting;
				case OperationStatus.Running:
					return Resources.opStatusRunning;
				case OperationStatus.FinishedOK:
					return Resources.opStatusFinishedOK;
				case OperationStatus.FinishedWithErrors:
					return Resources.opStatusFinishedWithErrors;
				case OperationStatus.FinishedWithWarnings:
					return Resources.opStatusFinishedWithWarnings;
				case OperationStatus.Info:
					return Resources.opStatusInfo;
				default:
					return Resources.opStatusInfo;
			}
		}

		internal static string OpStatusToImageKey(OperationStatus status)
		{
			switch (status)
			{
				case OperationStatus.Waiting:
					return "Undefined_16";
				case OperationStatus.Running:
					return "Running_16";
				case OperationStatus.FinishedOK:
					return "OK_16";
				case OperationStatus.FinishedWithErrors:
					return "Error_16";
				case OperationStatus.FinishedWithWarnings:
					return "Warning_16";
				case OperationStatus.Info:
					return "Info_16";
				default:
					return "Info_16";
			}
		}

		internal static OperationStatus StringToOpStatus(string status)
		{
			if (status.Equals(Resources.opStatusWaiting))
				return OperationStatus.Waiting;
			else
				if (status.Equals(Resources.opStatusFinishedOK))
					return OperationStatus.FinishedOK;
				else
					if (status.Equals(Resources.opStatusFinishedWithErrors))
						return OperationStatus.FinishedWithErrors;
					else
						if (status.Equals(Resources.opStatusFinishedWithWarnings))
							return OperationStatus.FinishedWithWarnings;
						else
							return OperationStatus.Running;
		}
	}
}