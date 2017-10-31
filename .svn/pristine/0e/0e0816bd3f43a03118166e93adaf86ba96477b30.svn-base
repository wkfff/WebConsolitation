using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Krista.FM.Client.Common;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.ServerLibrary;

using Infragistics.Win;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.Misc;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.AssociatedCls
{
	public partial class AssociateMaster : Form
	{
		private IBridgeAssociation masterAssociation;

		// по умалчанию будем сопоставл€ть по всему классификатору
		private int masterDataClsSourceID = -1;
	    private int masterBridgeClsSourceID = -1;

		private int NextPageIndex = 0;

		IWorkplace masterWorkplace = null;

        public AssociateMaster(IBridgeAssociation CurentAssociation, int dataClsCurrentDataClsSourceId, int bridgeClsCurrentDataClsSourceId, IWorkplace currentWorkplace)
		{
			InitializeComponent();
			masterAssociation = CurentAssociation;
			masterDataClsSourceID = dataClsCurrentDataClsSourceId;
            masterBridgeClsSourceID = bridgeClsCurrentDataClsSourceId;
			masterWorkplace = currentWorkplace;
            rbAllClassifier.Enabled = CurentAssociation.AssociationClassType != AssociationClassTypes.BridgeBridge;

			// «аполн€ем список правил сопоставлени€
			int indexDefaultRule = 0;
			string defaultRule = masterAssociation.GetDefaultAssociateRule();
			foreach (IAssociateRule rule in masterAssociation.AssociateRules.Values)
			{
				lbAssociationRules.Items.Add(new AssociationRuleItem(rule.ObjectKey, rule.Name));
				if (!String.IsNullOrEmpty(defaultRule))
                {
					if (rule.Name == defaultRule)
                    {
                        indexDefaultRule = lbAssociationRules.Items.Count - 1;
                    }
                }
			}
			if (lbAssociationRules.Items.Count > 0)
                lbAssociationRules.SetSelected(indexDefaultRule, true);
		}

        /// <summary>
        /// ¬ызывает мастер сопоставлени€
        /// </summary>
        /// <param name="CurentAssociation"></param>
        /// <param name="dataClsCurrentSourceID"></param>
        /// <param name="currentWorkplace"></param>
        /// <returns></returns>
        public static bool Associate(IBridgeAssociation CurentAssociation, int dataClsCurrentSourceID, int bridgeClsCurrentDataClsSourceId, IWorkplace currentWorkplace)
        {
            AssociateMaster master = new AssociateMaster(CurentAssociation, dataClsCurrentSourceID, bridgeClsCurrentDataClsSourceId, currentWorkplace);
            return master.ShowDialog() == DialogResult.OK;
        }

		/// <summary>
		///  ќбработчик нажати€ на сопоставление
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnAssociate_Click(object sender, EventArgs e)
		{
            if (rbAllClassifier.Checked)
                label1.Text = string.Format("¬сего записей: {0}", masterAssociation.GetAllRecordsCount());
            else
                label1.Text = string.Format("¬сего записей: {0}", masterAssociation.GetRecordsCountByCurrentDataSource(masterDataClsSourceID));
			// не будем даже выводить список правил сопоставлени€, лучше и проще наверное создать одно и по нему сопоставл€ть
			IAssociateRule masterRule = null;
		    string ruleKey = string.Empty;
            if (lbAssociationRules.Items.Count > 0)
            {
                masterRule = masterAssociation.AssociateRules[((AssociationRuleItem)lbAssociationRules.SelectedItem).Key].CloneRule();
                ruleKey = ((AssociationRuleItem) lbAssociationRules.SelectedItem).Key;
            }

			int associateResult = 0;
			try
			{
				// сопоставл€ть по всему классификатору или по текущему источнику
				if (rbAllClassifier.Checked || masterDataClsSourceID < 0)
					masterDataClsSourceID = -1;
				// если получили не нулевое правило, то ставим дл€ нее параметры
			    AssociationRuleParams ruleParams = new AssociationRuleParams();
                StringElephanterSettings ruleSetting = new StringElephanterSettings();
                if (!string.IsNullOrEmpty(ruleKey))
                {
                    //AssociationRuleParams ruleparams = new AssociationRuleParams();
                    ruleParams.AddNewRecords = cbAddToBridge.Checked;
                    ruleParams.ReAssiciate = rbClear_Associate.Checked;
                    ruleParams.UseConversionTable = rbConversionTable.Checked || rbUseAll.Checked;
                    ruleParams.UseFieldCoincidence = rbFieldCoincidence.Checked || rbUseAll.Checked;

                    //StringElephanterSettings ruleSetting = new StringElephanterSettings();
                    // учитваем цифры в наименовании
                    ruleSetting.AllowDigits = !cbCheckDigitsInName.Checked;
                    // учитываем одиночные буквы
                    ruleSetting.AllowSingleChars = cbCheckSingleLiters.Checked;

                }
				// сопоставл€ем классификатор
				SetAssociateCount(l1, l2);
				masterWorkplace.OperationObj.Text = "ќбработка данных";
				masterWorkplace.OperationObj.StartOperation();
				if (masterRule == null)
					associateResult = masterAssociation.Associate(masterDataClsSourceID, masterBridgeClsSourceID);
				else
                    associateResult = masterAssociation.Associate(masterDataClsSourceID, masterBridgeClsSourceID, ruleKey, ruleSetting, ruleParams);
					//associateResult = masterAssociation.Associate(masterSourceID, masterRule);
				masterWorkplace.OperationObj.StopOperation();
				SetAssociateCount(l3, l4);
			    btnApply.Text = "OK";
                btnApply.Click -= new EventHandler(btnAssociate_Click);
                btnApply.Click += new EventHandler(btnApply_Click);
			    btnApply.DialogResult = DialogResult.OK;
			}
			finally
			{
				NextPageIndex++;
				lCaption.Text = "–езультаты сопоставлени€";
				lAssociateRes.Text = string.Format("ќбработанно {0} записей", associateResult);
				utcWizardMain.SelectedTab = utcWizardMain.Tabs[NextPageIndex];
				btnCancel.Enabled = false;
			}
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			// закрываем мастер
			this.Close();
			masterWorkplace = null;
		}

		private void btnApply_Click(object sender, EventArgs e)
		{
			// закрываем мастер
			this.Close();
			masterWorkplace = null;
		}
		
		/// <summary>
		///  ѕолучение данных о количестве всех записей и сопоставленных записей
		/// </summary>
        private void SetAssociateCount(UltraLabel ll1, UltraLabel ll2)
		{
            int allRecordsCount = masterAssociation.GetRecordsCountByCurrentDataSource(masterDataClsSourceID);// CommonMethods.GetDataCLSRecordCount(masterSourceID, true, masterAssociation, masterWorkplace);
            int unAssociateRecordsCount = masterAssociation.GetUnassociateRecordsCountByCurrentDataSource(masterDataClsSourceID);//CommonMethods.GetDataCLSRecordCount(masterSourceID, false, masterAssociation, masterWorkplace);
            ll1.Text = string.Format("—опоставлено: {0}", allRecordsCount - unAssociateRecordsCount);
            ll2.Text = string.Format("Ќе сопоставлено: {0}", unAssociateRecordsCount);
		}

		private void lbAssociationRules_SelectedIndexChanged(object sender, EventArgs e)
		{
			defaultRuleCheckEditor.CheckedChanged -= defaultRuleCheckEditor_CheckedChanged;
			bool isDefault = masterAssociation.GetDefaultAssociateRule() == lbAssociationRules.SelectedItem.ToString();
			defaultRuleCheckEditor.Checked = isDefault;
			defaultRuleCheckEditor.Enabled = !isDefault;
			defaultRuleCheckEditor.CheckedChanged += defaultRuleCheckEditor_CheckedChanged;
		}

		private void defaultRuleCheckEditor_CheckedChanged(object sender, EventArgs e)
		{
			masterAssociation.SetDefaultAssociateRule(lbAssociationRules.SelectedItem.ToString());
		}
	}

    /// <summary>
    ///  ласс дл€ представлени€ правила сопоставлени€ в визуальном списке.
    /// </summary>
    internal class AssociationRuleItem
    {
        private readonly string key;
        private readonly string name;

        public AssociationRuleItem(string key, string name)
        {
            this.key = key;
            this.name = name;
        }

        public string Key
        {
            get { return key; }
        }

        public override string ToString()
        {
            return name;
        }
    }
}