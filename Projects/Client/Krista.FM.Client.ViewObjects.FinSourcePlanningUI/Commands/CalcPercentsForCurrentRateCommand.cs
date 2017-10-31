using System;
using System.Windows.Forms;

using Infragistics.Win.UltraWinGrid;

using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.Workplace.Gui;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Commands
{
	/// <summary>
	/// Расчет журнала процентов всех договоров по текущей ставке ЦБ.
	/// </summary>
	public class CalcPercentsForCurrentRateCommand : AbstractCommand
	{
		public CalcPercentsForCurrentRateCommand()
		{
			key = "CalcPercentsForCurrentRateCommand";
			caption = "Расчет журнала процентов всех договоров по текущей ставке ЦБ";
			iconKey = "ButtonGreenAlt";
		}

		public override void Run()
		{
			try
			{
				FinSourcePlanningNavigation.Instance.Workplace.OperationObj.StartOperation();
				FinSourcePlanningNavigation.Instance.Workplace.OperationObj.Text = "Выполнение операции...";

				JournalCBDataClsUI content = (JournalCBDataClsUI)WorkplaceSingleton.Workplace.ActiveContent;
				UltraGridRow row = ((BaseClsView)content.ViewCtrl).ugeCls.ugData.ActiveRow;
				if (row != null)
				{
                    Server.FinSourcePlanningServer creditServer = Server.FinSourcePlanningServer.GetPlaningIncomesServer();
                    creditServer.CalcPercentsForCredits(FinSourcePlanningNavigation.Instance.CurrentVariantID, Convert.ToInt32(row.Cells["ID"].Value));
				    creditServer = Server.FinSourcePlanningServer.GetCreditIssuedServer();
                    creditServer.CalcPercentsForCredits(FinSourcePlanningNavigation.Instance.CurrentVariantID, Convert.ToInt32(row.Cells["ID"].Value));
				}
				else
					throw new FinSourcePlanningException("Необходимо выделить запись.");

				FinSourcePlanningNavigation.Instance.Workplace.OperationObj.StopOperation();
				MessageBox.Show("Операция завершена успешно.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (FinSourcePlanningException ex)
			{
				FinSourcePlanningNavigation.Instance.Workplace.OperationObj.StopOperation();
				MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				FinSourcePlanningNavigation.Instance.Workplace.OperationObj.StopOperation();
			}
		}
	}
}
