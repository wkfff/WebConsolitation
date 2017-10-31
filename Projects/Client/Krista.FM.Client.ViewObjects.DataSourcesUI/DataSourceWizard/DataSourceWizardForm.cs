using System;
using System.Windows.Forms;

using Krista.FM.Common;
using Krista.FM.Common;
using Krista.FM.Client.Common.Wizards;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.ViewObjects.DataSourcesUI.DataSourceWizard
{
    public partial class DataSourceWizardForm : Form
    {
        public DataSourceWizardForm()
        {
            ShowInTaskbar = false;
            InitializeComponent();
        }

        private void DataSourceWizardForm_Load(object sender, System.EventArgs e)
        {
            Icon = Owner.Icon;
        }

        private void AddDataSource(IDataKind dataKind)
        {
            IDataSourceManager dsManager = DataSourcesNavigation.Instance.Workplace.ActiveScheme.DataSourceManager;

            IDataSource ds = dsManager.DataSources.CreateElement();
            // добавляем в источник параметры по умолчанию
            ds.BudgetName = string.Empty;
            ds.DataCode = dataKind.Code;
            ds.DataName = dataKind.Name;
            ds.ParametersType = dataKind.ParamKind;
            ds.SupplierCode = dataKind.Supplier.Name;
            ds.Territory = string.Empty;
            ds.Variant = string.Empty;

            // для каждого типа источника пытаемся получить параметры, введенные с формы
            if (ds.ParametersType != ParamKindTypes.WithoutParams)
            {
                string value = string.Empty;

                // год
                value = wizardDataSourcesParametersPage.GetSourceParamValue(DataSourcesParametersPage.SourceParams.Year);
                if (value != string.Empty)
                    ds.Year = Convert.ToInt32(value);
                
                // вариант
                ds.Variant = wizardDataSourcesParametersPage.GetSourceParamValue(DataSourcesParametersPage.SourceParams.Variant);
                
                // квартал
                value = wizardDataSourcesParametersPage.GetSourceParamValue(DataSourcesParametersPage.SourceParams.Quarter);
                if (value != string.Empty)
                    ds.Quarter = Convert.ToInt32(value);
                
                // месяц
                value = wizardDataSourcesParametersPage.GetSourceParamValue(DataSourcesParametersPage.SourceParams.Month);
                if (value != string.Empty)
                    ds.Month = Convert.ToInt32(value);
                
                // территория
                ds.Territory = wizardDataSourcesParametersPage.GetSourceParamValue(DataSourcesParametersPage.SourceParams.Territory);

                // финансовый орган
                ds.BudgetName = wizardDataSourcesParametersPage.GetSourceParamValue(DataSourcesParametersPage.SourceParams.BudgetName);
            }

            int? dataSourceID = ds.FindInDatabase();

            if (dataSourceID == null)
            {
                try
                {
                    string dataSourceName = dsManager.GetDataSourceName(ds.Save());
                    wizardFinalPage.Description2 = String.Format(
                        "Источник \"{0}\" успешно добавлен.",
                        dataSourceName);
                }
                catch (ServerException e)
                {
                    wizardFinalPage.Description2 = String.Format(
                        "При добавлении источника данных произошла ошибка: \"{0}\"",
                        Krista.Diagnostics.KristaDiagnostics.ExpandException(e));
                }
            }
            else
            {
                string dataSourceName = dsManager.GetDataSourceName((int)dataSourceID);
                wizardFinalPage.Description2 = String.Format(
                    "Добавляемый источник \"{0}\" уже существует. Задайте другие параметры.",
                    dataSourceName);
            }
        }

        private void wizardForm_Next(object sender, Krista.FM.Client.Common.Wizards.WizardForm.EventNextArgs e)
        {
            WizardPageBase page = (WizardPageBase)sender;
            if (page.Equals(wizardDataSourcesTreePage))
            {
                IDataKind dk = wizardDataSourcesTreePage.SelecterDataKind;
                if (dk.ParamKind == ParamKindTypes.WithoutParams)
                {
                    AddDataSource(dk);
                    e.Step = 2;
                }
                else
                {
                    wizardDataSourcesParametersPage.SelectedDataKind = dk;
                    wizardDataSourcesParametersPage.CreateControls();
                    e.Step = 1;
                }
            }

            if (page.Equals(wizardDataSourcesParametersPage))
            {
                AddDataSource(wizardDataSourcesTreePage.SelecterDataKind);
                e.Step = 1;
            }
        }

        private void wizardForm_Back(object sender, Krista.FM.Client.Common.Wizards.WizardForm.EventNextArgs e)
        {
            WizardPageBase page = (WizardPageBase)sender;
			if (page.Equals(wizardDataSourcesTreePage))
			{
				wizardForm.WizardButtons |= WizardForm.TWizardsButtons.Next;
			}
			else if (page.Equals(wizardFinalPage))
            {
				wizardForm.WizardButtons |= WizardForm.TWizardsButtons.Cancel;
				
				IDataKind dk = wizardDataSourcesTreePage.SelecterDataKind;
                if (dk.ParamKind == ParamKindTypes.WithoutParams)
                {
                    e.Step = 2;
                }
            }
        }

        private void wizardForm_Finish(object sender, Krista.FM.Client.Common.Wizards.WizardForm.EventNextArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void wizardForm_WizardClosed(object sender, EventArgs e)
        {
            Close();
        }

        private void wizardForm_Cancel(object sender, Krista.FM.Client.Common.Wizards.WizardForm.EventWizardCancelArgs e)
        {
            //DialogResult = DialogResult.Cancel;
        }

        private void wizardForm_PageShown(object sender, Krista.FM.Client.Common.Wizards.WizardForm.EventNextArgs e)
        {
			WizardPageBase page = (WizardPageBase)sender;
			if (page.Equals(wizardWelcomePage))
            {
                if (wizardWelcomePage.DontShow)
                {
                    wizardForm.MoveNextStep();
                }
            }
			if (page.Equals(wizardDataSourcesTreePage))
			{
				if (wizardDataSourcesTreePage.SelecterDataKind == null &&
					(wizardForm.WizardButtons & WizardForm.TWizardsButtons.Next) == WizardForm.TWizardsButtons.Next)
						wizardForm.WizardButtons -= WizardForm.TWizardsButtons.Next;
			}
			if (page.Equals(wizardFinalPage))
			{
				if ((wizardForm.WizardButtons & WizardForm.TWizardsButtons.Cancel) == WizardForm.TWizardsButtons.Cancel)
					wizardForm.WizardButtons -= WizardForm.TWizardsButtons.Cancel;
			}
		}
    }
}
