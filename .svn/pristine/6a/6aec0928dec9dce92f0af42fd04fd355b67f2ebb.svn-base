using System;
using System.Data;
using System.Windows.Forms;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.Validations.Visualizations;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.CreditIncomes;
using Krista.FM.Client.Workplace.Gui;
using System.Xml;
using Krista.FM.ServerLibrary.Validations;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Commands
{
	public class ValidateCommand : AbstractCommand
	{
	    private readonly XmlDocument configuration;

        public ValidateCommand(XmlDocument configuration)
		{
			key = "ValidateCommand";
			caption = "Контроль";
            this.configuration = configuration;
		}

		public override void Run()
		{
            FinSourcePlanningUI content = (FinSourcePlanningUI)WorkplaceSingleton.Workplace.ActiveContent;
			BaseClsView vo = (BaseClsView)content.ViewCtrl;
			int masterID = Convert.ToInt32(vo.ugeCls.ugData.ActiveRow.Cells["ID"].Value);
			DataRow[] rows = ((DataView)((BindingSource)vo.ugeCls.ugData.DataSource).DataSource).ToTable().Select(String.Format("ID = {0}", masterID));
			
			IValidatorMessageHolder vmh = CreditIncomeValidations.Validate(configuration, rows[0]);

            if (content.MessagesVisualizator != null)
		        content.MessagesVisualizator.Hide();
			content.MessagesVisualizator = new MessagesVisualizator(vmh);

			if (vmh.HasError)
            {
                vo.ugeCls.ugData.ActiveRow.Cells["clmnValidate"].Appearance.Image =
                    content.il.Images[3];
                vo.ugeCls.ugData.ActiveRow.Cells["clmnValidate"].ToolTipText
                    = "кредитный договор не прошел контроль";
                if (MessageBox.Show(content.Workplace.WindowHandle,
                    "Кредитный договор не прошел контроль. Сохранить отчет об ошибках?", "Контроль",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                { 
                    content.MessagesVisualizator.GetReport(content.Workplace.WindowHandle, "Протокол контроля договора");
                }
            }
            else
            {
                vo.ugeCls.ugData.ActiveRow.Cells["clmnValidate"].Appearance.Image =
                    content.il.Images[2];
                vo.ugeCls.ugData.ActiveRow.Cells["clmnValidate"].ToolTipText
                    = "кредитный договор контроль прошел";
            }
		}
	}
}
