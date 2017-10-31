using System;
using System.Collections.Generic;
using System.Text;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Wizards;
using Krista.FM.Client.Workplace.Gui;
using MSXML2;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Commands
{
	/// <summary>
	/// Запрос курс валют от ЦБР.
	/// </summary>
	public class RequestRateExchangeCBRCommand : AbstractCommand
	{
		public RequestRateExchangeCBRCommand()
		{
			key = "RequestRateExchangeCBR";
			caption = "Запросить курс валют от ЦБР (требуется подключение к Интернет)";
			iconKey = "ButtonBlue";
		}

		public override void Run()
		{
			LoadRateExchangeWizard wizard = new LoadRateExchangeWizard();
			wizard.ShowDialog();

			((BaseClsUI)WorkplaceSingleton.Workplace.ActiveContent).Refresh();
		}
	}
}
