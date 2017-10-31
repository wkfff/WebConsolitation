using System;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.ForecastUI.Validations;
using Krista.FM.Client.Workplace.Gui;
using Krista.FM.ServerLibrary.Validations;
using System.Collections.Generic;


namespace Krista.FM.Client.ViewObjects.ForecastUI.Commands
{
	public class CalcValuationCommand : AbstractCommand
	{
		public CalcValuationCommand()
		{
			key = "btnCalcValuation";
			caption = "Прямой расчет варианта расчета";
			//iconKey = "ButtonGreenAlt";
		}

		public override void Run()
		{
			ValuationUI content = (ValuationUI)WorkplaceSingleton.Workplace.ActiveContent;
			BaseClsView vo = (BaseClsView)content.ViewCtrl;
			UltraGridRow row = vo.ugeCls.ugData.ActiveRow;
			if (row != null)
			{
				Int32 rowID = Convert.ToInt32(row.Cells["ID"].Value);
				
				WorkplaceSingleton.Workplace.OperationObj.Text = "Расчет модели";
				WorkplaceSingleton.Workplace.OperationObj.StartOperation();
				content.Service.CalcModel(rowID);
				WorkplaceSingleton.Workplace.OperationObj.StopOperation(); 

				content.Refresh();
			}
		}
	}

	public class CalcValuationWithValidCommand : AbstractCommand
	{
		public CalcValuationWithValidCommand()
		{
			key = "btnCalcValuationVal";
			caption = "Прямой расчет варианта расчета c выделением изменившихся индикаторов";
			//iconKey = "ButtonGreenAlt";
		}

		public override void Run()
		{
			ValuationUI content = (ValuationUI)WorkplaceSingleton.Workplace.ActiveContent;
			BaseClsView vo = (BaseClsView)content.ViewCtrl;
			UltraGridRow row = vo.ugeCls.ugData.ActiveRow;
			if (row != null)
			{
				Int32 rowID = Convert.ToInt32(row.Cells["ID"].Value);
				
				WorkplaceSingleton.Workplace.OperationObj.Text = "Расчет модели";
				WorkplaceSingleton.Workplace.OperationObj.StartOperation();
				IValidatorMessageHolder vmh = content.Service.CalcModel(rowID);
				WorkplaceSingleton.Workplace.OperationObj.StopOperation(); 

				IValidatorMessageHolder vmh2 = ValuationValidation.Validate(vmh);
				if (content.MessagesVisualizator != null)
					content.MessagesVisualizator.Hide();

				content.Refresh();

				content.MessagesVisualizator = new ForecastMessagesVisualizator(vmh2);
			}

		}
	}

	public class CreateValuationCommand : AbstractCommand
	{
		public CreateValuationCommand()
		{
			key = "btnCreateValuation";
			caption = "Создать варианта расчета";
			//iconKey = "ButtonGreenAlt";
		}

		public override void Run()
		{

		}
	}

	public class IdicPlanCommand : AbstractCommand
	{
		public IdicPlanCommand()
		{
			key = "btnIdicPlan";
			caption = "Индикативное планирование";
			//iconKey = "ButtonGreenAlt";
		}

		public override void Run()
		{
			ValuationUI content = (ValuationUI)WorkplaceSingleton.Workplace.ActiveContent;
			BaseClsView vo = (BaseClsView)content.ViewCtrl;
			UltraGridRow row = vo.ugeCls.ugData.ActiveRow;
			
			if (row != null)
			{
				Int32 rowID = Convert.ToInt32(row.Cells["ID"].Value);
				Dictionary<Int32, String> tmpAdj = new Dictionary<Int32, String>();
				Dictionary<Int32, String> tmpInd = new Dictionary<Int32, String>();
				
				MasterRow mr = content.mRows[rowID];
				DetailCells dcInd = mr[SchemeObjectsKeys.t_S_Indicators_Key];
				DetailCells dcAdj = mr[SchemeObjectsKeys.t_S_Adjusters_Key];

				if ((dcAdj.Count == 0) || (dcInd.Count == 0))
					return;

				foreach (KeyValuePair<Int32, String> pair in dcAdj)
				{
					Int32 id = Convert.ToInt32(content.GetDetailGridEx(SchemeObjectsKeys.t_S_Adjusters_Key).ugData.Rows[pair.Key].Cells["ID"].Value);
					tmpAdj.Add(id, pair.Value);
				}

				foreach (KeyValuePair<Int32, String> pair in dcInd)
				{
					Int32 id = Convert.ToInt32(content.GetDetailGridEx(SchemeObjectsKeys.t_S_Indicators_Key).ugData.Rows[pair.Key].Cells["ID"].Value);
					tmpInd.Add(id, pair.Value);
				}

				WorkplaceSingleton.Workplace.OperationObj.Text = "Выполняется индикативное планирование";
				WorkplaceSingleton.Workplace.OperationObj.StartOperation();
				content.Service.IdicPlanning(rowID, tmpInd, tmpAdj);
				WorkplaceSingleton.Workplace.OperationObj.StopOperation(); 
			}
		}
	}

}
