using System;
using System.Collections.Generic;
using System.Text;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.Workplace.Gui;

namespace Krista.FM.Client.ViewObjects.ForecastUI.Commands
{
	public class FilterOutOfRange : AbstractCommand
	{
		public FilterOutOfRange()
		{
			key = "btnFilterOutOfRange";
			caption = "Отфильтровать вышедшие за границы параметры";
			//iconKey = "ButtonGreenAlt";
		}

		public override void Run()
		{
			ForecastUI content = (ForecastUI)WorkplaceSingleton.Workplace.ActiveContent;
			BaseClsView vo = (BaseClsView)content.ViewCtrl;
			content.SwitchFilter();
		}
	}

	public class GroupByGroup : AbstractCommand
	{
		public GroupByGroup()
		{
			key = "btnGroupByGroup";
			caption = "Группировать";
			//iconKey = "ButtonGreenAlt";
		}

		public override void Run()
		{
			BaseForecastUI content = (BaseForecastUI)WorkplaceSingleton.Workplace.ActiveContent;
			BaseClsView vo = (BaseClsView)content.ViewCtrl;
			content.SwitchGroup();
		}
	}
}
