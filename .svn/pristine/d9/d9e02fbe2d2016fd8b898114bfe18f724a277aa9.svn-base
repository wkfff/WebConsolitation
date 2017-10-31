using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.Workplace.Gui;

namespace Krista.FM.Client.Workplace.Commands
{
	public class AboutCommand : AbstractCommand
	{
		public AboutCommand()
		{
			key = "AboutCommand";
			caption = "О программе";
		}

		public override void Run()
		{
			Common.Forms.About.ShowAbout(WorkplaceSingleton.Workplace.ActiveScheme, WorkplaceSingleton.Workplace);
		}
	}
}
