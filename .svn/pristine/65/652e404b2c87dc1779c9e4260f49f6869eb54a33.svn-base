using System;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.ForecastUI.Validations;
using Krista.FM.Client.Workplace.Gui;
using Krista.FM.Client.Workplace.Services;
using Krista.FM.Common;
using Krista.FM.ServerLibrary.Forecast;
using Krista.FM.ServerLibrary.Validations;

namespace Krista.FM.Client.ViewObjects.ForecastUI.Commands
{
	public class CalcScenarioCommand : AbstractCommand
	{
		public CalcScenarioCommand()
		{
			key = "btnCalcScenario";
			caption = "Расчет сценария";
			//iconKey = "ButtonGreenAlt";
		}

		public override void Run()
		{
			ScenarioUI content = (ScenarioUI)WorkplaceSingleton.Workplace.ActiveContent;
			BaseClsView vo = (BaseClsView)content.ViewCtrl;
			UltraGridRow row = vo.ugeCls.ugData.ActiveRow;
			
			if (content.MessagesVisualizator != null)
				content.MessagesVisualizator.Hide();
			
			if (row != null)
			{
				if ((row.Cells["UserID"].Value ==  DBNull.Value) || (Convert.ToInt32(row.Cells["UserID"].Value) == 0))
				{
					return;
				}
				if ((ScenarioStatus)(Convert.ToInt32(row.Cells["ReadyToCalc"].Value)) !=  ScenarioStatus.ReadyToCalc)
				{
					return;
				}
				Int32 rowID = Convert.ToInt32(row.Cells["ID"].Value);

				WorkplaceSingleton.Workplace.OperationObj.Text = "Расчет модели";
				WorkplaceSingleton.Workplace.OperationObj.StartOperation();
				content.Service.CalcModel(rowID);
				WorkplaceSingleton.Workplace.OperationObj.StopOperation();
				content.Refresh();
			}
	
		}
	}

	public class CalcScenarioWithValidCommand : AbstractCommand
	{
		public CalcScenarioWithValidCommand()
		{
			key = "btnCalcScenarioVal";
			caption = "Расчет сценария c выделением изменившихся индикаторов";
			//iconKey = "ButtonGreenAlt";
		}

		public override void Run()
		{
			ScenarioUI content = (ScenarioUI)WorkplaceSingleton.Workplace.ActiveContent;
			BaseClsView vo = (BaseClsView)content.ViewCtrl;
			UltraGridRow row = vo.ugeCls.ugData.ActiveRow;

			if (content.MessagesVisualizator != null)
				content.MessagesVisualizator.Hide();

			if (row != null)
			{
				if ((row.Cells["UserID"].Value == DBNull.Value) || (Convert.ToInt32(row.Cells["UserID"].Value) == 0))
				{
					return;
				}
				if ((ScenarioStatus)(Convert.ToInt32(row.Cells["ReadyToCalc"].Value)) != ScenarioStatus.ReadyToCalc)
				{
					return;
				}
				Int32 rowID = Convert.ToInt32(row.Cells["ID"].Value);

                content.CalcedValidation = null;

				WorkplaceSingleton.Workplace.OperationObj.Text = "Расчет модели";
				WorkplaceSingleton.Workplace.OperationObj.StartOperation();
				IValidatorMessageHolder vmh = content.Service.CalcModel(rowID);
				WorkplaceSingleton.Workplace.OperationObj.StopOperation();

                content.CalcedValidation = vmh;

				/*IValidatorMessageHolder vmh2 = ScenarioValidation.Validate(vmh);
				if (content.MessagesVisualizator != null)
					content.MessagesVisualizator.Hide();*/

				content.Refresh();

				////content.MessagesVisualizator = new ForecastMessagesVisualizator(vmh2);
			}

		}
	}

	public class CopyBaseScenarioParam : AbstractCommand
	{
		public CopyBaseScenarioParam()
		{
			key = "btnCopyParam";
			caption = "Заполнить параметры из базового сценария";
			//iconKey = "ButtonGreenAlt";
		}

		public override void Run()
		{
			ScenarioUI content = (ScenarioUI)WorkplaceSingleton.Workplace.ActiveContent;
			BaseClsView vo = (BaseClsView)content.ViewCtrl;
			UltraGridRow row = vo.ugeCls.ugData.ActiveRow;

			if (content.MessagesVisualizator != null)
				content.MessagesVisualizator.Hide();

			if (row != null)
			{
				Int32 rowID = Convert.ToInt32(row.Cells["ID"].Value);
				content.Service.CopyScenarioDetails(rowID, null);
			}
		}
	}

	public class SetReadyToCalc : AbstractCommand
	{
		public SetReadyToCalc()
		{
			key = "btnSetReadyToCalc";
			caption = "Пометить готовым к расчету";
			//iconKey = "ButtonGreenAlt";
		}

		public override void Run()
		{
			ScenarioUI content = (ScenarioUI)WorkplaceSingleton.Workplace.ActiveContent;
			BaseClsView vo = (BaseClsView)content.ViewCtrl;
			UltraGridRow row = vo.ugeCls.ugData.ActiveRow;

			if (content.MessagesVisualizator != null)
				content.MessagesVisualizator.Hide();

			if (row != null)
			{
				content.Service.SetScenarioStatus(Convert.ToInt32(row.Cells["ID"].Value), ScenarioStatus.ReadyToCalc);
				content.Refresh();
			}
		}
	}

	public class FillAdj : AbstractCommand
	{
		public FillAdj()
		{
			key = "btnFillAdj";
			caption = "Заполнить регуляторы исходя из значения на оценочный год и индекса роста";
			//iconKey = "ButtonGreenAlt";
		}

		public override void Run()
		{
			ScenarioUI content = (ScenarioUI)WorkplaceSingleton.Workplace.ActiveContent;
			ComboButtonTool cbt = CommandService.GetComboButtonByKey(content.GetActiveDetailGridEx().utmMain, key);
			UltraGridRow row = content.GetActiveDetailGridEx().ugData.ActiveRow;

			if ((row != null) && (cbt.Comboboxtool.Value != null))
			{
				String s = cbt.Comboboxtool.Value.ToString().TrimEnd('%');
				Double d;
				try
				{
					d = Convert.ToDouble(s) / 100;
				}
				catch (Exception e)
				{
					throw new ForecastException(e.Message, e);
				}
								
				Double estval = Convert.ToDouble(row.Cells["VALUEESTIMATE"].Value);
				row.Cells["VALUEY1"].Value = estval * (1 + d);
				row.Cells["VALUEY2"].Value = estval * (1 + d) * (1 + d);
				row.Cells["VALUEY3"].Value = estval * (1 + d) * (1 + d) * (1 + d);
				row.Cells["VALUEY4"].Value = estval * (1 + d) * (1 + d) * (1 + d) * (1 + d);
				row.Cells["VALUEY5"].Value = estval * (1 + d) * (1 + d) * (1 + d) * (1 + d) * (1 + d);
				row.Cells["IndexDef"].Value = d;
			}
		}
	}
}
