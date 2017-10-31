using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Forms;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server.Guarantees;
using Krista.FM.Client.Workplace.Gui;
using Krista.FM.Common.ReportHelpers;
using Krista.FM.Common.Templates;
using Krista.FM.ServerLibrary.TemplatesService;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Commands
{
    public class CalculateDebtCommand : AbstractCommand
    {
        protected string detailKey;

        public CalculateDebtCommand()
        {
            key = "CalculateDebtCommand";
            caption = "Расчет фактической задолженности";
            detailKey = SchemeObjectsKeys.t_S_DebtCI_Key;
        }

        public override void Run()
        {
            DateTime calculateDate = new DateTime();
            if (DateSelectForm.ShowDateForm(WorkplaceSingleton.Workplace.WindowHandle, ref calculateDate))
            {
                WorkplaceSingleton.Workplace.OperationObj.Text = "Расчет фактической задолженности";
                WorkplaceSingleton.Workplace.OperationObj.StartOperation();
                try
                {
                    FinSourcePlanningUI content = (FinSourcePlanningUI) WorkplaceSingleton.Workplace.ActiveContent;
                    Credit credit = new Credit(content.GetActiveDataRow());
                    DebtServer debtServer = new DebtServer();
                    debtServer.CalculateDebt(calculateDate, credit);
                    content.RefreshDetail(detailKey);
                    WorkplaceSingleton.Workplace.OperationObj.StopOperation();
                    MessageBox.Show(WorkplaceSingleton.Workplace.WindowHandle,
                        "Расчет фактической задолженности завершился успешно",
                        "Источники финансирования", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch
                { 
                    WorkplaceSingleton.Workplace.OperationObj.StopOperation(); 

                }
            }
        }
    }
}
