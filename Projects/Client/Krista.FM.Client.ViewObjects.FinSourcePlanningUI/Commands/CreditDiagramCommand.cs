using System;
using System.Data;
using System.Windows.Forms;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Credits;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server;
using Krista.FM.Client.Workplace.Gui;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Commands
{
    public class CreditDiagramCommand : AbstractCommand
    {
        public CreditDiagramCommand()
        {
            caption = "Лимит кредитной линии";
            key = "btnCreditDiagramCommand";
        }

        public override void Run()
        {
            BaseCreditUI content = (BaseCreditUI)WorkplaceSingleton.Workplace.ActiveContent;
            DataRow activeRow = content.GetActiveDataRow();
            // если не выделено ни одной записи, выходим
            if (activeRow == null)
            {
                MessageBox.Show(WorkplaceSingleton.Workplace.WindowHandle, "Не выбрано ни одного договора",
                    "Источники финансирования", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
                
            if (!activeRow.IsNull("ParentID"))
                activeRow = activeRow.Table.Select(string.Format("ID = {0}", activeRow["ID"]))[0];
            FinSourcePlanningServer planningServer = content.GetFinSourcePlanningServer();
            Credit credit = planningServer.GetCredit(content.GetDataRow(Convert.ToInt32(activeRow["ID"])));
            CreditDiagramForm.ShowDiagram(planningServer.GetCreditLineInform(credit));
        }
    }
}
