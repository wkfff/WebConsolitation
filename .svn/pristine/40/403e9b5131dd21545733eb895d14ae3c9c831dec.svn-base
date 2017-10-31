 using System;
using System.Collections.Generic;
 using System.Data;
 using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
 using Infragistics.Win.UltraWinGrid;
 using Infragistics.Win.UltraWinToolbars;
 using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
 using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server;
 using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server.Guarantees;
 using Krista.FM.ServerLibrary;
 using Krista.FM.ServerLibrary.FinSourcePlanning;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Guarantee
{
    public class GuaranteeUI : FinSourcePlanningUI
    {
        internal GuaranteeUI(IFinSourceBaseService service, string key)
			: base(service, key)
		{
		}

        public override void Initialize()
        {
            base.Initialize();

            vo.ugeCls.ugData.AfterCellUpdate += ugData_AfterCellUpdate;
        }

        void ugData_AfterCellUpdate(object sender, CellEventArgs e)
        {
        }

        internal XmlDocument GetConfiguration(string configurationResource)
        {
            Stream stream = GetType().Module.Assembly.GetManifestResourceStream(configurationResource);
            if (stream == null)
                throw new Exception(String.Format("Ресурс \"{0}\" не найден.", configurationResource));

            XmlDocument doc = new XmlDocument();
            doc.Load(stream);
            return doc;
        }

        public override void CheckClassifierPermissions()
        {
            base.CheckClassifierPermissions();

            allowAddRecord = um.CheckPermissionForSystemObject("FinSourcePlaning", (int)FinSourcePlaningOperations.AddRecord, false);
            allowClearClassifier = um.CheckPermissionForSystemObject("FinSourcePlaning", (int)FinSourcePlaningOperations.ClearData, false);
            allowDelRecords = um.CheckPermissionForSystemObject("FinSourcePlaning", (int)FinSourcePlaningOperations.DelRecord, false);
            allowEditRecords = um.CheckPermissionForSystemObject("FinSourcePlaning", (int)FinSourcePlaningOperations.EditRecord, false);
            allowImportClassifier = um.CheckPermissionForSystemObject("FinSourcePlaning", (int)FinSourcePlaningOperations.ImportData, false);

            if (!allowAddRecord)
                allowAddRecord = um.CheckPermissionForSystemObject("GuaranteeIssued", (int)FinSourcePlaningUIModuleOperations.AddRecord, false);
            if (!allowClearClassifier)
                allowClearClassifier = um.CheckPermissionForSystemObject("GuaranteeIssued", (int)FinSourcePlaningUIModuleOperations.ClearData, false);
            if (!allowDelRecords)
                allowDelRecords = um.CheckPermissionForSystemObject("GuaranteeIssued", (int)FinSourcePlaningUIModuleOperations.DelRecord, false);
            if (!allowEditRecords)
                allowEditRecords = um.CheckPermissionForSystemObject("GuaranteeIssued", (int)FinSourcePlaningUIModuleOperations.EditRecord, false);
            if (!allowImportClassifier)
                allowImportClassifier = um.CheckPermissionForSystemObject("GuaranteeIssued", (int)FinSourcePlaningUIModuleOperations.ImportData, false);

            if (!allowAddRecord && !allowDelRecords && !allowEditRecords && !allowImportClassifier)
                viewOnly = true;
            else
                viewOnly = false;
        }

        #region

        protected override void ugeCls_OnAfterRowActivate(object sender, EventArgs e)
        {
            base.ugeCls_OnAfterRowActivate(sender, e);
            SetTransfertContractButton();
        }

        public override void UpdateToolbar()
        {
            base.UpdateToolbar();

            ToolBase tool = ((BaseClsView)ViewCtrl).ugeCls.utmMain.Tools["VariantTransfer"];
            switch (FinSourcePlanningNavigation.Instance.CurrentVariantID)
            {
                case 0:
                    tool.SharedProps.AppearancesSmall.Appearance.Image = il.Images[9];
                    tool.SharedProps.ToolTipText = "Перенос договора в архив";
                    break;
                default:
                    tool.SharedProps.AppearancesSmall.Appearance.Image = il.Images[8];
                    tool.SharedProps.ToolTipText = "Перенос договора в действующие договора";
                    break;
            }
        }

        protected override void ugeCls_OnAfterRowsDelete(object sender, BeforeRowsDeletedEventArgs e)
        {
            base.ugeCls_OnAfterRowsDelete(sender, e);
            ToolBase tool = ((BaseClsView)ViewCtrl).ugeCls.utmMain.Tools["VariantTransfer"];
            // скрываем кнопку для записей, которые как либо были изменены.
            tool.SharedProps.Enabled = false;
            tool.SharedProps.ToolTipText = "Перенос удаленных договоров запрещен";
        }

        public override void CancelChanges()
        {
            SetTransfertContractButton();
        }

        public override bool  SaveData(object sender, EventArgs e)
        {
            DataTable dtChanges = dsObjData.Tables[0].Clone();
            DataTable dt = dsObjData.Tables[0].GetChanges(DataRowState.Modified);
            if (dt != null)
                dtChanges.Merge(dt);
            dt = dsObjData.Tables[0].GetChanges(DataRowState.Unchanged);
            if (dt != null)
                dtChanges.Merge(dt);
            bool value = base.SaveData(sender, e);
            if (!value)
                return false;
            // заполнение детали План исполнения по гарантии
            // для ярославля ничго не делаем
            if (string.Compare(
                Workplace.ActiveScheme.GlobalConstsManager.Consts["OKTMO"].Value.ToString(), "78 000 000") == 0)
                return true;
            foreach (DataRow row in dsObjData.Tables[0].Rows)
            {
                if (dtChanges.Select(string.Format("ID = {0}", row["ID"])).Length == 0)
                {
                    GuaranteeServer guaranteeServer = GuaranteeServer.GetGuaranteeServer(
                        Convert.ToInt32(row["RefOKV"]), Workplace.ActiveScheme);
                    guaranteeServer.CalcObligationExecutionPlan(new Server.Guarantees.Guarantee(row),
                                                                FinSourcePlanningNavigation.Instance.CurrentVariantYear);
                }
            }
            return true;
        }

        #endregion
    }
}
