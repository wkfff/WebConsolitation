using System.Diagnostics;

namespace Krista.Diagnostics
{
    public class HostTypeFilter : TraceFilter
	{
		public override bool ShouldTrace(TraceEventCache cache, string source, TraceEventType eventType, int id, string formatOrMessage, object[] args, object data1, object[] data)
		{
			return
				(RunMode.RunModeType == RunModeType.Console) && consoleHost
				|| (RunMode.RunModeType == RunModeType.Service) && serviceHost
				|| (RunMode.RunModeType == RunModeType.WinForm) && winformHost;

		}

		private readonly bool consoleHost;
		private readonly bool serviceHost;
		private readonly bool winformHost;

		public HostTypeFilter(string ñonfig)
		{
			foreach(string host in ñonfig.Split(new char[]{','}))
			{
				if (host.Trim().ToLower() == "console")
				{
					consoleHost = true;
				} else if (host.Trim().ToLower() == "service")
				{
					serviceHost = true;
				}
				else if (host.Trim().ToLower() == "winform")
				{
					winformHost = true;
				}
			}
		}

		public HostTypeFilter()
		{
		}
	}
}
