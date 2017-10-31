using System;
using System.IO;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.Common;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.ForecastUI.Gui.Form2P;
using Krista.FM.Client.Workplace.Gui;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.ForecastUI.Commands
{
	public class FillParamCommand : AbstractCommand
	{
		public FillParamCommand()
		{
			key = "btnFillParam";
			caption = "Заполнить параметры";
			//iconKey = "ButtonGreenAlt";
		}

		public override void Run()
		{
			Form2pUI content = (Form2pUI)WorkplaceSingleton.Workplace.ActiveContent;
			BaseClsView vo = (BaseClsView)content.ViewCtrl;
			UltraGridRow row = vo.ugeCls.ugData.ActiveRow;
			if (row != null)
			{
				if (content.GetActiveDetailGridEx().ugData.Rows.Count != 0)
				{
					MessageBox.Show("Заполнить параметры можно только для пустого варианта формы 2п!", "Ошибка заполнения параметров", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				else
				{
					Int32 rowID = Convert.ToInt32(row.Cells["ID"].Value);
					Int32 year = Convert.ToInt32(row.Cells["REFYEAR"].Value);
					content.Service.CreateNewForm2p(rowID, year);
					content.Refresh();
				}
			}
		}
	}

	class Form2pScenarioModal : ScenarioModal
	{
		private Int32 estYear;

		public int EstYear
		{
			set { estYear = value; }
		}

		public override void AdditionsFilters(BaseClsUI clsUI)
		{
			base.AdditionsFilters(clsUI);
			//Отфильтровываем сценарии в которых оценочный год равен estYear, а базовый соответственной estYear-1
			clsUI.UltraGridExComponent.ugData.DisplayLayout.Bands[0].ColumnFilters["REFYEAR"].FilterConditions.Add(FilterComparisionOperator.Equals, estYear-1);
		}
	}

	public class FillFromScenCommand : AbstractCommand
	{
		public FillFromScenCommand()
		{
			key = "btnFillFromScen";
			caption = "Заполнить на основе сценария";
			//iconKey = "ButtonGreenAlt";
		}

		public override void Run()
		{
			//IWorkplace workplace = WorkplaceSingleton.Workplace;
			Form2pUI content = (Form2pUI)WorkplaceSingleton.Workplace.ActiveContent;
			BaseClsView vo = (BaseClsView)content.ViewCtrl;
			UltraGridRow row = vo.ugeCls.ugData.ActiveRow;

			if (content.checkFillingStrictParam())
			{
				Int32 id = Convert.ToInt32(row.Cells["ID"].Value);

				Form2pScenarioModal sm = new Form2pScenarioModal();
				sm.EstYear = Convert.ToInt32(row.Cells["REFYEAR"].Value);

				Int32 scenID = sm.ShowScenarioModal();
				if (scenID != -1)
				{
					WorkplaceSingleton.Workplace.OperationObj.Text = "Формирование Формы-2П";
					WorkplaceSingleton.Workplace.OperationObj.StartOperation();
					content.Service.FillFromScen(scenID, id);
					WorkplaceSingleton.Workplace.OperationObj.StopOperation();
				}
			}
			else
				MessageBox.Show("Заполнены не все обязательные параметры за отчетные годы. Расчет невозможен!","Ошибка расчета",MessageBoxButtons.OK,MessageBoxIcon.Error);
		}
	}


	public class SaveToFormCommand : AbstractCommand
	{
		public SaveToFormCommand()
		{
			key = "btnSaveToForm";
			caption = "Сохранить Форму 2П в формате МЭР РФ";
			//iconKey = "ButtonGreenAlt";
		}

		public override void Run()
		{
			//IWorkplace workplace = WorkplaceSingleton.Workplace;
			Form2pUI content = (Form2pUI)WorkplaceSingleton.Workplace.ActiveContent;
			BaseClsView vo = (BaseClsView)content.ViewCtrl;
			UltraGridRow row = vo.ugeCls.ugData.ActiveRow;

			Int32 id = Convert.ToInt32(row.Cells["ID"].Value);

			frmSaveForm2p frm = new frmSaveForm2p();
			frm.Content = content;
			frm.SetVariant1(row.Cells["NAME"].Value.ToString(), id);

			if (frm.ShowDialog((Form)ForecastNavigation.Instance.Workplace) == DialogResult.OK)
			{
				Byte[] buff = null;
				SaveFileDialog dlg = new SaveFileDialog();
				dlg.Filter = "Excel документы *.xls|*.xls";
				if (dlg.ShowDialog((Form)ForecastNavigation.Instance.Workplace) == DialogResult.OK)
				{
					WorkplaceSingleton.Workplace.OperationObj.Text = "Сохранение Формы-2П в формате МЭР";
					WorkplaceSingleton.Workplace.OperationObj.StartOperation();
					buff = content.Service.SaveFormToExcel(frm.Id1, frm.Id2, frm.Year);
				}
				
				if ((buff != null) && (buff.Length >0))
				{
					FileStream fs = null;
					try
					{
						fs = new FileStream(dlg.FileName, FileMode.CreateNew, FileAccess.Write);
						fs.Write(buff, 0, buff.Length);
					}
					finally
					{
						if (fs != null)
						{
							fs.Flush();
							fs.Close();
						}
					}
				}
				dlg.Dispose();
				WorkplaceSingleton.Workplace.OperationObj.StopOperation();
			}
			//frm.Sh

			/*Form2pModal fm = new Form2pModal();
			//sm.EstYear = Convert.ToInt32(row.Cells["REFYEAR"].Value);

			Int32 scenID = fm.ShowForm2pModal();
			if (scenID != -1)
			{
				WorkplaceSingleton.Workplace.OperationObj.Text = "Формирование Формы-2П";
				WorkplaceSingleton.Workplace.OperationObj.StartOperation();
				content.Service.FillFromScen(scenID, id);
				WorkplaceSingleton.Workplace.OperationObj.StopOperation();
			}*/
		}
	}

	public class PumpDataCommand : AbstractCommand
	{
		public PumpDataCommand()
		{
			key = "btnPumpData";
			caption = "Заполнить Форму 2П на базаовые годы из классификаторов";
			//iconKey = "ButtonGreenAlt";
		}

		public override void Run()
		{
			//IWorkplace workplace = WorkplaceSingleton.Workplace;
			Form2pUI content = (Form2pUI)WorkplaceSingleton.Workplace.ActiveContent;
			BaseClsView vo = (BaseClsView)content.ViewCtrl;
			UltraGridRow row = vo.ugeCls.ugData.ActiveRow;

			Int32 id = Convert.ToInt32(row.Cells["ID"].Value);

			content.Service.PumpFromAnotherTable(id, true);
		}
	}

	public class AltForecastCommand : AbstractCommand
	{
		public AltForecastCommand()
		{
			key = "btnAltForecast";
			caption = "Прогноз Формы 2п математико-статистическим методом";
			//iconKey = "ButtonGreenAlt";
		}

		public override void Run()
		{
			//IWorkplace workplace = WorkplaceSingleton.Workplace;
			Form2pUI content = (Form2pUI)WorkplaceSingleton.Workplace.ActiveContent;
			BaseClsView vo = (BaseClsView)content.ViewCtrl;
			UltraGridRow row = vo.ugeCls.ugData.ActiveRow;

			Int32 id = Convert.ToInt32(row.Cells["ID"].Value);

			content.Service.AlternativeForecast(id);
		}
	}


}
