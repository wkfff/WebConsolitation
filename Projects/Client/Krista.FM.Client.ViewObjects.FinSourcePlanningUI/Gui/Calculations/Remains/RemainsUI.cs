using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Commands;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server;
using Krista.FM.Client.Workplace.Services;
using Krista.FM.ServerLibrary;
using Krista.FM.Common;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Calculations.Remains
{
    public class RemainsUI : CalculationBaseUI
    {
        public RemainsUI(IEntity entity)
            : base(entity)
        {
            Caption = "Расчет остатков средств бюджета";
        }

        private string oktmo = string.Empty;

        public override void Initialize()
        {
            base.Initialize();

            oktmo = Workplace.ActiveScheme.GlobalConstsManager.Consts["OKTMO"].Value.ToString();
            // добавляем различные командные кнопки на тулбар
            UltraToolbar tb = vo.ugeCls.utmMain.Toolbars[0];
            ButtonTool tool = CommandService.AttachToolbarTool(new CalculateRemainsCommand(), tb);
            tool.SharedProps.AppearancesSmall.Appearance.Image = il.Images[0];
            InfragisticsHelper.BurnTool(tool, true);

            bool calcVisible = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme.UsersManager.CheckPermissionForSystemObject("FinSourcePlaning",
                (int)FinSourcePlaningOperations.Calculate, false);
            if (!calcVisible)
                calcVisible = Workplace.ActiveScheme.UsersManager.CheckPermissionForSystemObject("VolumeHoldings",
                (int)FinSourcePlaningCalculateUIModuleOperations.Calculate, false);
            tool.SharedProps.Visible = calcVisible;

            tool = CommandService.AttachToolbarTool(new RemainsReportCommand(), tb);
            tool.SharedProps.AppearancesSmall.Appearance.Image = il.Images[1];

            ComboBoxTool cb = new ComboBoxTool("CalculationResults");
            cb.DropDownStyle = DropDownStyle.DropDownList;
            cb.SharedProps.Width = 200;
            ((BaseClsView)ViewCtrl).ugeCls.utmMain.Tools.AddRange(new ToolBase[] { cb });
            tb.Tools.AddTool("CalculationResults");

            vo.ugeCls.utmMain.ToolValueChanged += utmActions_ToolValueChanged;
            vo.ugeCls.ugData.AfterCellUpdate += ugData_AfterCellUpdate;

            // получаем список параметров с которыми можно будет загрузить данные
            ReloadCalculationList();
        }

        private void ReloadCalculationList()
        {
            ((ComboBoxTool)vo.ugeCls.utmMain.Tools["CalculationResults"]).ValueList.ValueListItems.Clear();
            // получаем список параметров с которыми можно будет загрузить данные
            using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
            {
                DataTable dtParams =
                    (DataTable)db.ExecQuery(string.Format("select Distinct CalcComment from {0} where RefBrwVariant = {1}",
                    ActiveDataObj.FullDBName, FinSourcePlanningNavigation.Instance.CurrentVariantID), QueryResultTypes.DataTable);
                // добавляем списки ранее сохраненных данных
                ((ComboBoxTool)vo.ugeCls.utmMain.Tools["CalculationResults"]).ValueList.ValueListItems.Add("Новые данные для расчета", "Новые данные для расчета");
                foreach (DataRow row in dtParams.Rows)
                {
                    ((ComboBoxTool)vo.ugeCls.utmMain.Tools["CalculationResults"]).ValueList.ValueListItems.Add(
                        string.Format("{0}", row[0]));
                }
                ((ComboBoxTool)vo.ugeCls.utmMain.Tools["CalculationResults"]).SelectedIndex = 0;
            }
        }

        void utmActions_ToolValueChanged(object sender, ToolEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "CalculationResults":
                    if (dsObjData.Tables.Count == 0)
                        break;
                    // получим параметры расчетов, которые проводились ранее для их загрузки
                    if (((ComboBoxTool)e.Tool).SelectedIndex == 0)
                    {
                        dsObjData.Tables[0].Clear();
                        dsObjData.Tables[0].AcceptChanges();
                        break;
                    }
                    currentComment = ((ComboBoxTool)e.Tool).Value.ToString();
                    LoadData(vo.ugeCls, new EventArgs());
                    break;
            }
        }

        public override void ugeCls_OnGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            base.ugeCls_OnGridInitializeLayout(sender, e);
            #region выделение некоторых колонок жирным шрифтом
            e.Layout.Bands[0].Columns["RemainsAccretion"].Header.Appearance.FontData.Bold = DefaultableBoolean.True;
            e.Layout.Bands[0].Columns["RemainsRecession"].Header.Appearance.FontData.Bold = DefaultableBoolean.True;
            e.Layout.Bands[0].Columns["RemainsChange"].Header.Appearance.FontData.Bold = DefaultableBoolean.True;
            #endregion
        }

        protected override GridColumnsStates ugeCls_OnGetGridColumnsState(object sender)
        {
            GridColumnsStates states = base.ugeCls_OnGetGridColumnsState(sender);
            states["RefIncVariant"].IsReadOnly = true;
            states["RefRVariant"].IsReadOnly = true;
            states["RefBrwVariant"].IsReadOnly = true;
            foreach (var state in states)
            {
                if (!state.Value.IsSystem && state.Value.ColumnType == UltraGridEx.ColumnType.Standart)
                {
                    state.Value.IsReadOnly = false;
                }
            }

            return states;
        }

        public override bool Refresh()
        {
            bool value = base.Refresh();
            ReloadCalculationList();
            return value;
        }

        void ugData_AfterCellUpdate(object sender, CellEventArgs e)
        {
            if (e.Cell.Column.DataType != typeof (Decimal))
                return;
            if (e.Cell.Value is DBNull)
                e.Cell.Value = 0;

            UltraGridRow gridRow = UltraGridHelper.GetRowCells(e.Cell.Row);
            string columnName = e.Cell.Column.Key.ToUpper();
            switch (columnName)
            {
                case "INCOME":
                case "CHARGE":
                    decimal income = Convert.ToDecimal(gridRow.Cells["Income"].Value);
                    decimal charge = Convert.ToDecimal(gridRow.Cells["Charge"].Value);
                    decimal creditReturnPerson = Convert.ToDecimal(gridRow.Cells["CreditReturnPerson"].Value);
                    decimal safetyStock = Convert.ToDecimal(gridRow.Cells["SafetyStock"].Value);
                    decimal creditExtensionPerson = Convert.ToDecimal(gridRow.Cells["CreditExtensionPerson"].Value);
                    decimal nameGuarantee = Convert.ToDecimal(gridRow.Cells["NameGuarantee"].Value);
                    decimal receiptCredit = Convert.ToDecimal(gridRow.Cells["ReceiptCredit"].Value);
                    decimal receiptBudgCredit = Convert.ToDecimal(gridRow.Cells["ReceiptBudgCredit"].Value);
                    decimal issueCapital = Convert.ToDecimal(gridRow.Cells["IssueCapital"].Value);
                    decimal creditReturnBudget = Convert.ToDecimal(gridRow.Cells["CreditReturnBudget"].Value);
                    decimal repayCredit = Convert.ToDecimal(gridRow.Cells["repayCredit"].Value);
                    decimal repayBudgCredit = Convert.ToDecimal(gridRow.Cells["RepayBudgCredit"].Value);
                    decimal creditExtensionBudget = Convert.ToDecimal(gridRow.Cells["CreditExtensionBudget"].Value);
                    decimal dischargeCapital = Convert.ToDecimal(gridRow.Cells["DischargeCapital"].Value);
                    decimal remainsAccretion = income + receiptCredit + receiptBudgCredit + issueCapital + creditReturnBudget +
                                       creditReturnPerson + safetyStock;
                    decimal remainsRecession = charge + repayCredit + repayBudgCredit + dischargeCapital + creditExtensionBudget +
                                       creditExtensionPerson + nameGuarantee;
                    decimal remainsChange = remainsAccretion - remainsRecession;

                    gridRow.Cells["RemainsAccretion"].Value = remainsAccretion;
                    gridRow.Cells["RemainsRecession"].Value = remainsRecession;
                    gridRow.Cells["RemainsChange"].Value = remainsChange;
                    break;
            }
        }

        private void AddNewIfRow(decimal value, int refVariant, int period, string constName, IDatabase db)
        {
            object[] contsData = finSourcesRererencesUtils.GetConstDataByName(constName);
            int refKif = -1;
            if (contsData != null && contsData.Length > 0)
            {
                refKif = Utils.GetClassifierRowID(db, SchemeObjectsKeys.d_KIF_Plan_Key,
                                                        FinSourcePlanningNavigation.Instance.CurrentSourceID,
                                                        "CodeStr",
                                                        contsData[0].ToString(), contsData[1].ToString(), true);
                switch (constName)
                {
                    case "KIFRemainsAccretion":
                        SetCourse(refKif, 1, db);
                        break;
                    case "KIFRemainsRecession":
                        SetCourse(refKif, -1, db);
                        break;
                }
                
            }
            int budLevel = Utils.GetBudgetLevel(Convert.ToInt32(Workplace.ActiveScheme.GlobalConstsManager.Consts["TerrPartType"].Value));

            RemainsServer.AddNewIfRow(Workplace.ActiveScheme, refKif, value, refVariant, period, budLevel, FinSourcePlanningNavigation.Instance.CurrentSourceID);
        }

        private void SetCourse(int id, int cource, IDatabase db)
        {
            IEntity entity = Workplace.ActiveScheme.RootPackage.FindEntityByName(SchemeObjectsKeys.d_KIF_Plan_Key);
            db.ExecQuery(string.Format("update {0} set RefKIF = {1} where ID = {2}", entity.FullDBName, cource, id),
                QueryResultTypes.NonQuery);
        }

        public override bool SaveData(object sender, EventArgs e)
        {
            if (base.SaveData(sender, e))
            {
                using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
                {
                    int refVariant = Convert.ToInt32(dsObjData.Tables[0].Rows[0]["RefBrwVariant"]);
                    // перед добавлением новых записей удалим старые
                    db.ExecQuery("delete from f_S_Plan where SourceID = ? and RefSVariant = ? and InterfaceSign = 3",
                                 QueryResultTypes.NonQuery,
                                 new DbParameterDescriptor("SourceID", FinSourcePlanningNavigation.Instance.CurrentSourceID),
                                 new DbParameterDescriptor("RefSVariant", refVariant));
                    foreach (DataRow row in dsObjData.Tables[0].Rows)
                    {
                        AddNewIfRow(Convert.ToDecimal(row["RemainsAccretion"]), refVariant,
                                    Convert.ToInt32(row["RefYearDayUNV"]), "KIFRemainsAccretion", db);
                        AddNewIfRow(Convert.ToDecimal(row["RemainsRecession"]), refVariant,
                                    Convert.ToInt32(row["RefYearDayUNV"]), "KIFRemainsRecession", db);
                    }
                }
                return true;
            }
            return false;
        }
    }
}
