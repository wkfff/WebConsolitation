using System;
using System.Collections.Generic;
using System.Text;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Commands
{
	/// <summary>
	/// Выбор варианта.
	/// </summary>
	public class SelectVariantCommand : AbstractCommand
	{
		public SelectVariantCommand()
		{
			key = "SelectVariantCommand";
			caption = "Выбрать вариант";
		}

		public override void Run()
		{
			object refVariant = null;

			if (FinSourcePlanningNavigation.Instance.Workplace.ClsManager.ShowClsModal(
				SchemeObjectsKeys.d_Variant_Borrow_Key, FinSourcePlanningNavigation.Instance.CurrentVariantID, -1, -1, ref refVariant))
			{
				if (refVariant != null && FinSourcePlanningNavigation.Instance.CurrentVariantID != Convert.ToInt32(refVariant))
				{
					FinSourcePlanningNavigation.Instance.SetCurrentVariant(Convert.ToInt32(refVariant));
				}
			}
		}
	}
}
