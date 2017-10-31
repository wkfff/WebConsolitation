using System;
using System.Data;
using System.Windows.Forms;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Capital;
using Krista.FM.Client.Workplace.Gui;
using Krista.FM.ServerLibrary.FinSourcePlanning;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Commands.CapitalCommands
{
    public class CalculateCapitalBalanceCommand : AbstractCommand
    {
        private CapitalServer Server
        {
            get; set;
        }

        public CalculateCapitalBalanceCommand(CapitalServer server)
        {
            Server = server;
            key = "CalculateCapitalBalanceCommand";
            caption = "Расчет текущего остатка по основному долгу";
        }

        public override void Run()
        {
            try
            {
                WorkplaceSingleton.Workplace.OperationObj.Text = "Обработка данных";
                WorkplaceSingleton.Workplace.OperationObj.StartOperation();
                var content = (FinSourcePlanningUI)WorkplaceSingleton.Workplace.ActiveContent;
                ((ICapitalService)content.Service).FillDebtRemainder(FinSourcePlanningNavigation.Instance.CurrentVariantID);
                WorkplaceSingleton.Workplace.OperationObj.StopOperation();
                MessageBox.Show(WorkplaceSingleton.Workplace.WindowHandle,
                   string.Format("Расчет текущего остатка завершен успешно. Для отображения результатов необходимо обновить данные"),
                   "Расчет текущего остатка", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception)
            {
                WorkplaceSingleton.Workplace.OperationObj.StopOperation();
                throw;
            }
        }
    }
}
