using System;
using System.Collections.Generic;
using System.Text;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.Workplace.Gui;

namespace Krista.FM.Client.Workplace.Commands
{
	public class ShowNavigationCommand : AbstractCommand
	{
		public ShowNavigationCommand()
		{
			key = "ShowNavigationCommand";
			caption = "Область навигации";
			shortcut = System.Windows.Forms.Shortcut.CtrlN;
		}

		public override void Run()
		{
			WorkplaceSingleton.Workplace.WorkplaceLayout.ShowPad(WorkplaceSingleton.Workplace.ExplorerPane);
		}
	}
}
