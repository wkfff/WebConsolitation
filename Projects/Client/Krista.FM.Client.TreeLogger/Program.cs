using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;

namespace Krista.FM.Client.TreeLogger
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Log log = new Log("");
			log.ShowLog(null);
			string[] commandLineArgs = System.Environment.GetCommandLineArgs();
			string logFileName = string.Empty;
			for (int i = 0; i < commandLineArgs.Length; i++)
			{
				if (Path.GetExtension(commandLineArgs[i]).Equals(".xmllog",
					StringComparison.OrdinalIgnoreCase))
				{
					logFileName = commandLineArgs[i];
					break;
				}
			}
			if (!string.IsNullOrEmpty(logFileName)) { log.LoadFromXML(logFileName); }
			Application.Run();

			//Application.Run(new frmLog());
		}
	}
}