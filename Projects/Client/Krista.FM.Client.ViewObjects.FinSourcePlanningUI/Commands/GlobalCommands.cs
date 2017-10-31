using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server;
using Krista.FM.Client.Workplace.Gui;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Forms;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Commands
{
    public class BudgetTransfertCommand : AbstractCommand
    {
        public BudgetTransfertCommand()
        {
            key = "CalculateBorrowingValume";
            caption = "Перенос данных в проект бюджета";
        }

        public override void Run()
        {
            int incomeVariant = -1;
            int outcomeVariant = -1;
            int sourceID = -1;
            if (BudgetTransfertParamsForm.ShowBudgetTransfertParams(WorkplaceSingleton.Workplace.WindowHandle, ref incomeVariant, ref outcomeVariant, ref sourceID))
            {
                WorkplaceSingleton.Workplace.OperationObj.Text = "Обработка данных";
                WorkplaceSingleton.Workplace.OperationObj.StartOperation();
                try
                {
                    int budgetLevel = Utils.GetBudgetLevel(Convert.ToInt32(
                            WorkplaceSingleton.Workplace.ActiveScheme.GlobalConstsManager.Consts["TerrPartType"].Value));
                    BudgetTransfert budgetTransfert = new BudgetTransfert(WorkplaceSingleton.Workplace.ActiveScheme);
                    budgetTransfert.TransfertData(incomeVariant, outcomeVariant, FinSourcePlanningNavigation.Instance.CurrentVariantID,
                        FinSourcePlanningNavigation.Instance.CurrentSourceID, budgetLevel, (IClassifiersProtocol)WorkplaceSingleton.Workplace.ActiveScheme.GetProtocol("Workplace.exe"));

                    WorkplaceSingleton.Workplace.OperationObj.StopOperation();
                    MessageBox.Show(WorkplaceSingleton.Workplace.WindowHandle, "Перенос данных завершился успешно", "Перенос данных в проект бюджета",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                finally
                {
                    WorkplaceSingleton.Workplace.OperationObj.StopOperation();
                }
            }
        }
    }

    public class SetAllReferencesCommand : AbstractCommand
    {
        public SetAllReferencesCommand()
        {
            key = "CalculateBorrowingValume";
            caption = "Установка ссылок на классификаторы";
        }

        public override void Run()
        {
            WorkplaceSingleton.Workplace.OperationObj.Text = "Обработка данных";
            WorkplaceSingleton.Workplace.OperationObj.StartOperation();
            try
            {
                WorkplaceSingleton.Workplace.ActiveScheme.FinSourcePlanningFace.SetAllReferences(FinSourcePlanningNavigation.Instance.CurrentSourceID);
                WorkplaceSingleton.Workplace.OperationObj.StopOperation();
                MessageBox.Show(WorkplaceSingleton.Workplace.WindowHandle, "Установка ссылок завершилась успешно", "Установка ссылок",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception e)
            {
                WorkplaceSingleton.Workplace.OperationObj.StopOperation();
                throw;// new Exception(e.Message, e.InnerException);
            }
        }
    }
}
