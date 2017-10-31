using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Commands;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server;
using Krista.FM.Client.Workplace.Services;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.FinSourcePlanning;
using Krista.FM.ServerLibrary.TemplatesService;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Credits
{
    public class BaseCreditUI : FinSourcePlanningUI
    {
        public BaseCreditUI(IFinSourceBaseService service, string key)
            : base(service, key)
        {
        }

        internal string ActiveDetailKey
        {
            get; set;
        }

        public override void Initialize()
        {
            base.Initialize();

            UltraToolbar toolbar = ((BaseClsView)ViewCtrl).ugeCls.utmMain.Toolbars["FinSourcePlanning"];

            AddReports(toolbar);

            DetailGridInitializeLayout += OnDetailGridInitializeLayout;
            ((BaseClsView)ViewCtrl).ugeCls.OnAfterRowInsert += ugeCls_OnAfterRowInsert;
        }

        protected virtual void ugeCls_OnAfterRowInsert(object sender, UltraGridRow row)
        {
            
        }

        protected virtual void OnDetailGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

        }

        private static DataTable GetTranshTable(UltraGridRow activeMasterRow)
        {
            DataTable dtTransh = new DataTable();
            dtTransh.Columns.Add("ID", typeof(Int32));
            DataColumn column = dtTransh.Columns.Add("Num");
            column.Caption = "Номер договора";
            column = dtTransh.Columns.Add("Sum");
            column.Caption = "Сумма в рублях";
            bool isCurrency = Convert.ToInt32(activeMasterRow.Cells["RefOKV"].Value) != -1;
            if (isCurrency)
            {
                column = dtTransh.Columns.Add("CurrencySum");
                column.Caption = "Сумма в валюте";
            }
            foreach (UltraGridRow row in activeMasterRow.ChildBands[0].Rows)
            {
                DataRow newRow = dtTransh.NewRow();
                newRow[0] = row.Cells["ID"].Value;
                newRow[1] = row.Cells["Num"].Value;
                newRow[2] = row.Cells["Sum"].Value;
                if (isCurrency)
                    newRow[3] = row.Cells["CurrencySum"].Value;
                dtTransh.Rows.Add(newRow);
            }
            return dtTransh;
        }

        private void AddReports(UltraToolbar toolbar)
        {
            DataTable dtTemplates = Workplace.ActiveScheme.TemplatesService.Repository.GetTemplatesInfo(TemplateTypes.System);
			foreach (DataRow row in dtTemplates.Select(String.Format("Code = '{0}'", "Credit")))
            {
                CreateReportCommand cmd = new CreateReportCommand(Convert.ToInt32(row["ID"]), row["Name"].ToString(), row["DocumentFileName"].ToString());
                ButtonTool btnCreateReport = CommandService.AttachToolbarTool(cmd, toolbar, "Templates");
                switch (Path.GetExtension(row["DocumentFileName"].ToString()).ToLower())
                {
                    case ".xls":
                        btnCreateReport.SharedProps.AppearancesSmall.Appearance.Image = Resources.ru.excelDocument;
                        break;
                    case ".xlt":
                        btnCreateReport.SharedProps.AppearancesSmall.Appearance.Image = Resources.ru.excelTemplate;
                        break;
                    case ".doc":
                        btnCreateReport.SharedProps.AppearancesSmall.Appearance.Image = Resources.ru.wordDocument;
                        break;
                    case ".dot":
                        btnCreateReport.SharedProps.AppearancesSmall.Appearance.Image = Resources.ru.wordlTemplate;
                        break;
                }
            }
            //CreditDiagramCommand diagramCommand = new CreditDiagramCommand();
            //CommandService.AttachToolbarTool(diagramCommand, toolbar, "Templates");
        }

        /// <summary>
        /// Показывает, можно ли для договора расчитать пени
        /// </summary>
        public bool CanCalculatePenalties
        {
            get
            {
                UltraGridRow row = UltraGridHelper.GetActiveRowCells(((BaseClsView)ViewCtrl).ugeCls.ugData);
                if (row == null)
                    return false;
                if (row.Cells["RefSStatusPlan"].Value is DBNull)
                    return false;
                if (Convert.ToInt32(row.Cells["RefSStatusPlan"].Value) == 0)
                    return true;
                return false;
            }
        }

        public virtual FinSourcePlanningServer GetFinSourcePlanningServer()
        {
            return null;
        }

        private DataTable dtJournalCB;

        internal Decimal GetCBPercent(DateTime startDate)
        {
            if (dtJournalCB == null)
                dtJournalCB = Utils.GetEntityTable(SchemeObjectsKeys.d_S_JournalCB_Key);
            if (dtJournalCB == null || dtJournalCB.Rows.Count == 0)
                return 0;
            DataRow[] rows = dtJournalCB.Select(string.Format("InputDate <= '{0}'", startDate), "InputDate ASC");
            return rows.Length > 0 ? Convert.ToDecimal(rows[rows.Length - 1]["PercentRate"]) : 0;
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
        /*
        /// <summary>
        /// получение параметров иерархии
        /// </summary>
        /// <param name="clsObject"></param>
        /// <param name="gridEx"></param>
        /// <returns></returns>
        public override HierarchyInfo GetHierarchyInfoFromClsObject(IEntity clsObject, UltraGridEx gridEx)
        {
            HierarchyInfo newHrInfo = new HierarchyInfo();

            if (clsObject.ClassType == ClassTypes.Table)
            {
                newHrInfo.LevelsNames = new string[] { clsObject.Caption };
                newHrInfo.LevelsCount = 1;
                newHrInfo.FlatLevelName = newHrInfo.LevelsNames[0];
                gridEx.SingleBandLevelName = newHrInfo.LevelsNames[0];
                newHrInfo.CurViewState = ViewState.Flat;
                return newHrInfo;
            }

            newHrInfo.LevelsNames = new string[] { "Генеральный договор", "Транш" };
            newHrInfo.LevelsCount = 2;
            newHrInfo.FlatLevelName = newHrInfo.LevelsNames[0];
            gridEx.SingleBandLevelName = newHrInfo.LevelsNames[0];
            newHrInfo.ParentClmnName = "ID";
            newHrInfo.ParentRefClmnName = "ParentID";
            newHrInfo.ObjectHierarchyType = Components.HierarchyType.ParentChild;
            newHrInfo.CurViewState = ViewState.Hierarchy;
            return newHrInfo;
        }*/

        protected DataTable dtChange;

        protected override void BeforeSaveData()
        {
            dtChange = dsObjData.Tables[0].Copy();
        }

        public override bool SaveData(object sender, EventArgs e)
        {
            if (base.SaveData(sender, e))
            {
                DataColumn parentIDColumn = dtChange.Columns["ParentID"];
                IEntityAssociation association = ActiveDataObj.Associated[a_S_FactAttract];
                foreach (DataRow row in dtChange.Rows)
                {
                    // вносим изменения только при удалении записей
                    if (row.RowState == DataRowState.Deleted)
                    {
                        object deletedID = row["ID", DataRowVersion.Original];
                        if (!row.IsNull(parentIDColumn, DataRowVersion.Original))
                        {
                            // удаляем факт предоставления у генерального договора, относящийся к удаляемому траншу
                            using (IDataUpdater du = association.RoleData.GetDataUpdater(
                                        string.Format("RefTransh = {0}", deletedID), null, null))
                            {
                                DataTable dtDeleted = new DataTable();
                                du.Fill(ref dtDeleted);
                                foreach (DataRow deletedRow in dtDeleted.Rows)
                                {
                                    deletedRow.Delete();
                                }
                                du.Update(ref dtDeleted);
                            }
                        }
                    }
                }
                return true;
            }
            return false;
        }

        protected DataTable dtDetailChange;

        protected override void BeforeDetailSaveData(ref DataSet detail)
        {
            dtDetailChange = dsDetail.Tables[0].Copy();
        }

        protected virtual string t_S_FactAttract
        {
            get { return string.Empty; }
        }

        protected virtual string t_S_FactDebt
        {
            get { return string.Empty; }
        }

        protected virtual string a_S_FactAttract
        {
            get { return string.Empty; }
        }

        protected virtual string a_S_FactDebt
        {
            get { return string.Empty; }
        }

        protected override void AfterDetailRowInsert(object sender, UltraGridRow row)
        {
            base.AfterDetailRowInsert(sender, row);

            if (row.Cells.Exists("RefOKV"))
            {
                UltraGridRow masterRow = UltraGridHelper.GetActiveRowCells(((BaseClsView)ViewCtrl).ugeCls.ugData);
                int refOKV = Convert.ToInt32(masterRow.Cells["RefOKV"].Value);
                row.Cells["RefOKV"].Value = refOKV;
            }

            Credit credit = new Credit(GetActiveDataRow());
            if (row.Band.Columns.Exists("RefKIF"))
                row.Cells["RefKIF"].Value = Workplace.ActiveScheme.FinSourcePlanningFace.GetCreditClassifierRef(activeDetailObject.ObjectKey,
                    SchemeObjectsKeys.d_KIF_Plan_Key, CurrentDataSourceID, credit.RefOKV, credit.CreditsType, credit.RefTerrType, credit.ProgramCode);
            if (row.Band.Columns.Exists("RefEKR"))
                row.Cells["RefEKR"].Value = Workplace.ActiveScheme.FinSourcePlanningFace.GetCreditClassifierRef(activeDetailObject.ObjectKey,
                    SchemeObjectsKeys.d_EKR_PlanOutcomes_Key, CurrentDataSourceID, credit.RefOKV, credit.CreditsType, credit.RefTerrType, credit.ProgramCode);
            if (row.Band.Columns.Exists("RefR"))
                row.Cells["RefR"].Value = Workplace.ActiveScheme.FinSourcePlanningFace.GetCreditClassifierRef(activeDetailObject.ObjectKey,
                    SchemeObjectsKeys.d_R_Plan_Key, CurrentDataSourceID, credit.RefOKV, credit.CreditsType, credit.RefTerrType, credit.ProgramCode);
            if (row.Band.Columns.Exists("RefKD"))
                row.Cells["RefKD"].Value = Workplace.ActiveScheme.FinSourcePlanningFace.GetCreditClassifierRef(activeDetailObject.ObjectKey,
                    SchemeObjectsKeys.d_KD_Plan_Key, CurrentDataSourceID, credit.RefOKV, credit.CreditsType, credit.RefTerrType, credit.ProgramCode);

            if (activeDetailObject.ObjectKey == SchemeObjectsKeys.t_S_CollateralCI_Key)
            {
                row.Cells["RefSCollateral"].Value = 0;
            }
        }

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

        #region Транши

        public override void SetTaskId(ref UltraGridRow row)
        {
            base.SetTaskId(ref row);
            // для траншей устанавливаем параметры записи-генельного договора
            if (row.ParentRow != null)
            {
                UltraGridRow parentRow = row.ParentRow;
                int transhesCount = parentRow.ChildBands[0].Rows.Count;
                row.Cells["Num"].Value = string.Format("{0}/{1}", parentRow.Cells["Num"].Value, transhesCount);
                row.Cells["Purpose"].Value = parentRow.Cells["Purpose"].Value;
                row.Cells["GenDocInfo"].Value = string.Format("{0}; {1}", parentRow.Cells["Num"].Value,
                    Convert.ToDateTime(parentRow.Cells["ContractDate"].Value).ToShortDateString());
                row.Cells["PercentRate"].Value = parentRow.Cells["PercentRate"].Value;
                row.Cells["PenaltyDebtRate"].Value = parentRow.Cells["PenaltyDebtRate"].Value;
                row.Cells["PenaltyPercentRate"].Value = parentRow.Cells["PenaltyPercentRate"].Value;
                row.Cells["CreditPercent"].Value = parentRow.Cells["CreditPercent"].Value;
                row.Cells["PretermDischarge"].Value = parentRow.Cells["PretermDischarge"].Value;
                row.Cells["ChargeFirstDay"].Value = parentRow.Cells["ChargeFirstDay"].Value;
                row.Cells["RefSExtension"].Value = 2;
                row.Cells["RefSRepayDebt"].Value = parentRow.Cells["RefSRepayDebt"].Value;
                row.Cells["RefSRepayPercent"].Value = parentRow.Cells["RefSRepayPercent"].Value;
                row.Cells["RefOrganizations"].Value = parentRow.Cells["RefOrganizations"].Value;
                row.Cells["RefOKV"].Value = parentRow.Cells["RefOKV"].Value;
                row.Cells["RefSStatusPlan"].Value = parentRow.Cells["RefSStatusPlan"].Value;
                row.Cells["RefTypeContract"].Value = parentRow.Cells["RefTypeContract"].Value;
                row.Cells["RefRegions"].Value = parentRow.Cells["RefRegions"].Value;
                row.Cells["RefPeriodDebt"].Value = parentRow.Cells["RefPeriodDebt"].Value;
                row.Cells["RefPeriodRate"].Value = parentRow.Cells["RefPeriodRate"].Value;
                row.Cells["RefKindBorrow"].Value = parentRow.Cells["RefKindBorrow"].Value;
            }
        }

        #endregion

        internal virtual bool HasFactAttractionData()
        {
            return true;
        }

        public DateTime FormDate
        {
            get; set;
        }

        public string FormComment
        {
            get; set;
        }

        internal void SetContractStatus(UltraGridRow contractRow)
        {
            if (contractRow.Cells["RefSStatusPlan"].Value == null ||
                contractRow.Cells["RefSStatusPlan"].Value == DBNull.Value)
                return;
            int contractStatus = Convert.ToInt32(contractRow.Cells["RefSStatusPlan"].Value);
            switch (contractStatus)
            {
                case -1:
                    contractRow.Cells["PlanStatus"].ButtonAppearance.Image = Resources.ru.UnknownContract;
                    contractRow.Cells["PlanStatus"].ToolTipText = "Неуказанный статус договора";
                    break;
                case 0:
                    contractRow.Cells["PlanStatus"].ButtonAppearance.Image = Resources.ru.ApplyContract;
                    contractRow.Cells["PlanStatus"].ToolTipText = "Принят";
                    break;
                case 1:
                    contractRow.Cells["PlanStatus"].ButtonAppearance.Image = Resources.ru.Not_ApplyContract;
                    contractRow.Cells["PlanStatus"].ToolTipText = "Не принят";
                    break;
                case 2:
                    contractRow.Cells["PlanStatus"].ButtonAppearance.Image = Resources.ru.lock_icon;
                    contractRow.Cells["PlanStatus"].ToolTipText = "Рефинансирован";
                    break;
                case 3:
                    contractRow.Cells["PlanStatus"].ButtonAppearance.Image = Resources.ru.lock_icon;
                    contractRow.Cells["PlanStatus"].ToolTipText = "Досрочно погашен";
                    break;
                case 4:
                    contractRow.Cells["PlanStatus"].ButtonAppearance.Image = Resources.ru.ClosedContract;
                    contractRow.Cells["PlanStatus"].ToolTipText = "Закрыт";
                    break;
                case 5:
                    contractRow.Cells["PlanStatus"].ButtonAppearance.Image = Resources.ru.coins_icon;
                    contractRow.Cells["PlanStatus"].ToolTipText = "На рефинансирование";
                    break;
            }
        }

        protected override void ugeCls_OnClickCellButton(object sender, CellEventArgs e)
        {
            base.ugeCls_OnClickCellButton(sender, e);

            if (string.Compare(e.Cell.Column.Key, "PlanStatus", true) == 0)
            {
                object[] planStatus = new object[1];
                if (GetStatusPlan(ref planStatus))
                {
                    e.Cell.Row.Cells["RefSStatusPlan"].Value = planStatus[0];
                    e.Cell.Row.Update();
                }
            }
        }

        internal bool DetailHasVersions(string detailKey)
        {
            switch (detailKey)
            {
                case SchemeObjectsKeys.t_S_PlanDebtCI_Key:
                case SchemeObjectsKeys.t_S_PlanServiceCI_Key:
                    return true;
                default:
                    return false;
            }
        }

        internal void SetLastCalculation()
        {
            if (!DetailHasVersions(ActiveDetailKey))
                return;
            using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
            {
                object currentId = UltraGridHelper.GetActiveID(vo.ugeCls.ugData);
                IEntity entity = Workplace.ActiveScheme.RootPackage.FindEntityByName(ActiveDetailKey);
                var dtCalculation = db.ExecQuery(
                    string.Format("select EstimtDate, CalcComment from {0} where id = (select max(id) from {0} where RefCreditInc = ?)", entity.FullDBName),
                    QueryResultTypes.DataTable,
                    new DbParameterDescriptor("p0", currentId)) as DataTable;
                if (dtCalculation != null && dtCalculation.Rows.Count > 0)
                {
                    if (dtCalculation.Rows[0].IsNull("EstimtDate") || dtCalculation.Rows[0].IsNull("CalcComment"))
                        return;
                    var calcVersion = new VersionParams(dtCalculation.Rows[0].Field<DateTime>("EstimtDate"),
                                                        dtCalculation.Rows[0].Field<string>("CalcComment"));

                    var cb = activeDetailGrid.utmMain.Tools["CalculationResults"] as ComboBoxTool;
                    if (cb != null)
                    {
                        cb.SelectedItem = cb.ValueList.FindByDataValue(calcVersion);
                    }
                }
            }
        }
    }
}
