using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Calculations.DDE;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.DDE;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Forms;
using Krista.FM.Client.Workplace.Gui;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Commands
{
    public class DDECalculationCommand : AbstractCommand
    {
        public DDECalculationCommand()
        {
            key = "DDECalculation";
            caption = "Расчет доступной долговой емкости";
        }

        public override void Run()
        {
            DDEIndicatorsUI content = (DDEIndicatorsUI)WorkplaceSingleton.Workplace.ActiveContent;
            DdeCalculationParams calculationParams = content.ViewObject.GetCalculationParams();
            if (calculationParams.PlaningVariant == -1)
            {
                calculationParams.PlaningVariant = 0;
                //MessageBox.Show("Не указан плановый вариант ИФ", "Расчет ДДЕ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //return;
            }

            content.Workplace.OperationObj.Text = "Расчет ДДЕ";
            content.Workplace.OperationObj.StartOperation();
            try
            {
                DDEService ddeService = new DDEService(content.Workplace.ActiveScheme);
                DataTable dataTable = ddeService.CalculateDDE(calculationParams, FinSourcePlanningNavigation.Instance.CurrentSourceID);
                content.SetCalculationData(dataTable);
                content.ViewObject.TabControl.Tabs[1].Active = true;
                //content.ViewObject.Comment = string.Empty;
                content.ViewObject.TabControl.SelectedTab = content.ViewObject.TabControl.Tabs[1];
            }
            finally
            {
                content.Workplace.OperationObj.StopOperation();
            }
        }
    }
}
