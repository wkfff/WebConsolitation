using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.Common;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Capital;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server;
using Krista.FM.Client.Workplace.Gui;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI
{
    public partial class CapitalTranshCreateWizard : Form
    {
        private DataRow masterRow;

        private CapitalServer capitalServer;

        public CapitalTranshCreateWizard(CapitalServer capitalServer, DataRow masterRow)
        {
            // устанавливаем основные обработчики мастера
            // настраиваем мастер для работы
            InitializeComponent();

            this.masterRow = masterRow;
            this.capitalServer = capitalServer;

            wizard.Cancel += wizard_Cancel;
            wizard.Finish += wizard_Finish;
            wizard.Next += wizard_Next;
            wizard.Back += wizard_Back;
            wizard.WizardClosed += wizard_WizardClosed;

            uceCurrencyRate.BeforeDropDown += uceCurrencyRate_BeforeDropDown;

            SetData();
        }

        private void wizard_Next(object sender, Common.Wizards.WizardForm.EventNextArgs e)
        {
            if (e.CurrentPage == wizardParametersPage)
            {
                wizardFinalPage.Description2 = FillTranshes();
            }
        }

        private void wizard_Back(object sender, Common.Wizards.WizardForm.EventNextArgs e)
        {
            
        }

        private void wizard_Cancel(object sender, Common.Wizards.WizardForm.EventWizardCancelArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void wizard_Finish(object sender, Common.Wizards.WizardForm.EventNextArgs e)
        {
            Close();
        }

        private void wizard_WizardClosed(object sender, EventArgs e)
        {
            Close();
        }

        // выпадающий список с курсами валют
        void uceCurrencyRate_BeforeDropDown(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            // показываем справочник с курсом текущей валюты
            int refOkv = Convert.ToInt32(masterRow["RefOKV"]);
            object[] exchRate = new object[1];
            FinSourcePlanningUI content = (FinSourcePlanningUI)WorkplaceSingleton.Workplace.ActiveContent;
            if (content.GetExchangeRate(refOkv, new string[1] { "EXCHANGERATE" }, ref exchRate))
            {
                uceCurrencyRate.Items.Clear();
                uceCurrencyRate.Items.Add(exchRate[0]);
                uceCurrencyRate.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Установка первоначальных данных для заполнения детали
        /// </summary>
        private void SetData()
        {
            int refOkv = Convert.ToInt32(masterRow["RefOKV"]);

            uceCurrencyRate.Visible = refOkv != -1;
            uteCurrency.Visible = refOkv != -1;
            ulExchangeRate.Visible = refOkv != -1;
            ulCurrency.Visible = refOkv != -1;
            uteCurrency.Text = masterRow["REFOKV"].ToString();
            uceCurrencyRate.Value = masterRow["ExchangeRate"];
            uceCurrencyRate.SelectedIndex = 0;
        }

        /// <summary>
        /// заполнение детали
        /// </summary>
        /// <returns></returns>
        private string FillTranshes()
        {
            // заполняем деталь "Транши"
            capitalServer.FillTransh(Convert.ToInt32(uneTranshCount.Value), ucbEqualCount.Checked, masterRow);
            wizardFinalPage.Description2 = "Расчет траншей успешно завершен";
            wizard.WizardButtons = Common.Wizards.WizardForm.TWizardsButtons.Next;

            return string.Empty;
        }
    }
}