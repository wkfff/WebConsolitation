using System.Windows.Forms;

using Krista.FM.Client.Common.Gui;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Commands
{
	/// <summary>
	/// Расчет журнала процентов всех договоров по всем ставкам ЦБ.
	/// </summary>
	public class CalcPercentsForCreditIncomesCommand : AbstractCommand
	{
		public CalcPercentsForCreditIncomesCommand()
		{
			key = "CalcPercentsForCreditIncomesCommand";
			caption = "Расчет журнала процентов всех договоров по всем ставкам ЦБ";
			iconKey = "ButtonGreen";
		}

		public override void Run()
		{
			try
			{
				FinSourcePlanningNavigation.Instance.Workplace.OperationObj.StartOperation();
				FinSourcePlanningNavigation.Instance.Workplace.OperationObj.Text = "Выполнение операции...";

                Server.FinSourcePlanningServer creditServer = Server.FinSourcePlanningServer.GetPlaningIncomesServer();
                creditServer.CalcPercentsForCredits(FinSourcePlanningNavigation.Instance.CurrentVariantID, -1);
                creditServer = Server.FinSourcePlanningServer.GetCreditIssuedServer();
                creditServer.CalcPercentsForCredits(FinSourcePlanningNavigation.Instance.CurrentVariantID, -1);

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
