using System;
using System.Collections.Generic;
using System.Text;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.Workplace.Gui;

namespace Krista.FM.Client.Workplace.Commands
{
	public class ChangePasswordCommand : AbstractCommand
	{
		public ChangePasswordCommand()
		{
			key = "ChangePasswordCommand";
			caption = "Изменить пароль";
		}

		public override void Run()
		{
			FormChangePassword.ChangePassword(WorkplaceSingleton.Workplace.ActiveScheme, false, null);
		}
	}
}
