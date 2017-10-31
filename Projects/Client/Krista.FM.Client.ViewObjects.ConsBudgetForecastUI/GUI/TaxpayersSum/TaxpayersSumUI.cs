using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.Common;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.DataCls;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.Server;
using Krista.FM.ServerLibrary;
using ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle;
using ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle;

namespace Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.GUI.TaxpayersSum
{
    public class TaxpayersSumUI : BaseViewObj
    {
        public TaxpayersSumUI(string key)
            : base(key)
        {
            Caption = "Годовой план по доходам";
        }

        public TaxpayersSumView ViewObject
        {
            get; set;
        }

        private long MunicipalId
        {
            get; set;
        }

        private TaxpayersSumService TaxpayersService
        {
            get; set;
        }

        private DataTable TaxpayersData
        {
            get; set;
        }

        private DataTable ResultData
        {
            get; set;
        }

        private int CurrentYear
        {
            get; set;
        }

        public int SourceId
        {
            get; set;
        }

        private List<long> Taxpayers
        {
            get; set;
        }

        protected override void SetViewCtrl()
        {
            fViewCtrl = new TaxpayersSumView();
            fViewCtrl.ViewContent = this;
        }

        public override void Initialize()
        {
            base.Initialize();
            ViewObject = fViewCtrl as TaxpayersSumView;
            TaxpayersService = new TaxpayersSumService(Workplace.ActiveScheme);
            ViewObject.Municipals.EditorButtonClick += Municipals_EditorButtonClick;

            ViewObject.ResultsGrid.InitializeLayout += ResultsGrid_InitializeLayout;
            ViewObject.ResultsGrid.AfterColPosChanged += TaxpayerDataGrid_AfterColPosChanged;

            ViewObject.TaxpayerDataGrid.AfterColPosChanged += TaxpayerDataGrid_AfterColPosChanged;
            ViewObject.TaxpayerDataGrid.AfterGroupPosChanged += TaxpayerDataGrid_AfterGroupPosChanged;
            ViewObject.TaxpayerDataGrid.InitializeLayout += TaxpayerDataGrid_InitializeLayout;
            ViewObject.TaxpayerDataGrid.InitializeRow += TaxpayerDataGrid_InitializeRow;
            ViewObject.TaxpayerDataGrid.AfterCellUpdate += TaxpayerDataGrid_AfterCellUpdate;
            ViewObject.TaxpayerDataGrid.BeforeCellUpdate += TaxpayerDataGrid_BeforeCellUpdate;
            ViewObject.TaxpayerDataGrid.ClickCellButton += TaxpayerDataGrid_ClickCellButton;

            ViewObject.tbManager.ToolClick += tbManager_ToolClick;
            ViewObject.cbYears.SelectedIndexChanged += cbYears_SelectedIndexChanged;
            ViewObject.cbYears.SelectedIndex = ViewObject.cbYears.FindString(DateTime.Today.Year.ToString());
            ViewObject.Quarter.SelectedIndex = 0;

            Taxpayers = new List<long>();

            LoadEmptyData();
        }

        void TaxpayerDataGrid_ClickCellButton(object sender, CellEventArgs e)
        {
            var oldRefOrg = e.Cell.Row.Cells["RefOrg"].Value;
            var columns = new List<string>(new string[] { "ID", "INN", "Name" });
            var values = new List<object>();
            if (ChooseRef(ObjectKeys.d_Org_TaxBenPay, string.Empty, "INN", columns, ref values))
            {
                var newRefOrg = values[0];
                ChangeTaxpayer(oldRefOrg, newRefOrg, values[2], values[1]);
            }
        }

        void TaxpayerDataGrid_AfterGroupPosChanged(object sender, AfterGroupPosChangedEventArgs e)
        {
            if (e.PosChanged == PosChanged.Sized)
            {
                int width = ViewObject.TaxpayerDataGrid.DisplayLayout.Bands[0].Groups["Taxpayer"].Width
                    + ViewObject.TaxpayerDataGrid.DisplayLayout.Bands[0].Groups["IndicatorName"].Width;
                ViewObject.ResultsGrid.DisplayLayout.Bands[0].Columns["IndicatorName"].Width = width;
                ViewObject.ResultsGrid.DisplayLayout.Bands[0].Columns["SumPayment"].Width =
                    ViewObject.TaxpayerDataGrid.DisplayLayout.Bands[0].Columns["SumPayment"].Width;
                ViewObject.ResultsGrid.DisplayLayout.Bands[0].Columns["SumReduction"].Width =
                    ViewObject.TaxpayerDataGrid.DisplayLayout.Bands[0].Columns["SumReduction"].Width;
            }
        }

        void cbYears_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentYear = Convert.ToInt32(ViewObject.cbYears.SelectedItem);
            SourceId = TaxpayersService.GetSourceId(CurrentYear);
            ViewObject.Municipals.Tag = null;
            ViewObject.Municipals.Text = string.Empty;
            LoadEmptyData();
        }

        void tbManager_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {
            string toolKey = e.Tool.Key;
            switch (toolKey)
            {
                case "RefreshData":
                    LoadData();
                    break;
                case "CancelChanges":
                    CancelChanges();
                    break;
                case "SaveChanges":
                    SaveData();
                    break;
                case "AddNew":
                    AddNewTaxpayer();
                    break;
                case "Delete":
                    DeleteTaxpayer();
                    break;
            }
        }

        #region настройка гридов

        private decimal OldValue
        {
            get; set;
        }

        void TaxpayerDataGrid_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
        {
            OldValue = e.Cell.Value == null || e.Cell.Value == DBNull.Value ? 0 : Convert.ToDecimal(e.Cell.Value);
        }

        void TaxpayerDataGrid_AfterCellUpdate(object sender, CellEventArgs e)
        {
            bool isResult = Convert.ToDecimal(e.Cell.Row.Cells["IndicatorCode"].Value) == 0;
            if (isResult)
            {
                return;
            }
            // меняем результирующую строку
            var columnKey = e.Cell.Column.Key;
            var dataRow = TaxpayersData.Select(string.Format("ID = {0}", e.Cell.Row.Cells["ID"].Value))[0];
            if (string.Compare(columnKey, "SumPayment", true) == 0 || string.Compare(columnKey, "SumReduction", true) == 0)
            {
                int rowIndicatorCode = Convert.ToInt32(e.Cell.Row.Cells["IndicatorCode"].Value);
                var resultRow = TaxpayersData.Select(string.Format("RefOrg = {0} and IndicatorCode = 0", e.Cell.Row.Cells["RefOrg"].Value))[0];
                var currentValue = dataRow[columnKey];
                currentValue = currentValue == null || currentValue == DBNull.Value ? 0 : Convert.ToDecimal(currentValue);
                resultRow[columnKey] = resultRow.IsNull(columnKey)
                    ? currentValue
                    : Convert.ToDecimal(resultRow[columnKey]) - OldValue + Convert.ToDecimal(currentValue);
                switch (rowIndicatorCode)
                {
                    case 1:
                        ResultData.Rows[1][columnKey] = ResultData.Rows[1].IsNull(columnKey) 
                            ? currentValue
                            : Convert.ToDecimal(ResultData.Rows[1][columnKey]) - OldValue + Convert.ToDecimal(currentValue);
                        break;
                    case 2:
                        ResultData.Rows[2][columnKey] = ResultData.Rows[2].IsNull(columnKey)
                            ? currentValue
                            : Convert.ToDecimal(ResultData.Rows[2][columnKey]) - OldValue + Convert.ToDecimal(currentValue);
                        break;
                }
                ResultData.Rows[0][columnKey] = ResultData.Rows[0].IsNull(columnKey)
                    ? currentValue
                    : Convert.ToDecimal(ResultData.Rows[0][columnKey]) - OldValue + Convert.ToDecimal(currentValue);

            }
            switch (columnKey)
            {
                case "SumPayment":
                    
                    if (e.Cell.Value != null && e.Cell.Value != DBNull.Value)
                    {
                        var value = Convert.ToDecimal(e.Cell.Value);
                        e.Cell.Row.Cells["SumReduction"].Activation = value != 0 ? Activation.ActivateOnly : Activation.AllowEdit;
                    }
                    else
                    {
                        e.Cell.Row.Cells["SumReduction"].Activation = Activation.AllowEdit;
                    }
                    break;
                case "SumReduction":
                    if (e.Cell.Value != null && e.Cell.Value != DBNull.Value)
                    {
                        var value = Convert.ToDecimal(e.Cell.Value);
                        e.Cell.Row.Cells["SumPayment"].Activation = value != 0 ? Activation.ActivateOnly : Activation.AllowEdit;
                    }
                    else
                    {
                        e.Cell.Row.Cells["SumPayment"].Activation = Activation.AllowEdit;
                    }
                    break;
            }

            BurnChangeData(true);
        }

        void TaxpayerDataGrid_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            var row = e.Row;
            if (Convert.ToInt32(row.Cells["IndicatorCode"].Value) == 0)
                row.Activation = Activation.ActivateOnly;
            if (row.Cells["SumPayment"].Value != null && row.Cells["SumPayment"].Value != DBNull.Value)
            {
                var value = Convert.ToDecimal(row.Cells["SumPayment"].Value);
                if (value != 0)
                    row.Cells["SumReduction"].Activation = Activation.ActivateOnly;
            }
            else if (row.Cells["SumReduction"].Value != null && row.Cells["SumReduction"].Value != DBNull.Value)
            {
                var value = Convert.ToDecimal(row.Cells["SumReduction"].Value);
                if (value != 0)
                    row.Cells["SumPayment"].Activation = Activation.ActivateOnly;
            }
        }

        void TaxpayerDataGrid_AfterColPosChanged(object sender, AfterColPosChangedEventArgs e)
        {
            if (ViewObject.TaxpayerDataGrid.DataSource == null)
                return;

            if (e.PosChanged == PosChanged.Sized)
            {
                int width = ViewObject.TaxpayerDataGrid.DisplayLayout.Bands[0].Groups["Taxpayer"].Width
                    + ViewObject.TaxpayerDataGrid.DisplayLayout.Bands[0].Groups["IndicatorName"].Width;
                ViewObject.ResultsGrid.DisplayLayout.Bands[0].Columns["IndicatorName"].Width = width;
                ViewObject.ResultsGrid.DisplayLayout.Bands[0].Columns["SumPayment"].Width =
                    ViewObject.TaxpayerDataGrid.DisplayLayout.Bands[0].Columns["SumPayment"].Width;
                ViewObject.ResultsGrid.DisplayLayout.Bands[0].Columns["SumReduction"].Width =
                    ViewObject.TaxpayerDataGrid.DisplayLayout.Bands[0].Columns["SumReduction"].Width;
            }
        }

        void TaxpayerDataGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Override.AllowColMoving = AllowColMoving.NotAllowed;
            e.Layout.Override.AllowGroupMoving = AllowGroupMoving.NotAllowed;
            e.Layout.Override.AllowRowFiltering = DefaultableBoolean.False;
            var taxpayerGroup = e.Layout.Bands[0].Groups.Add("Taxpayer", "Налогоплательщик");
            taxpayerGroup.Header.VisiblePosition = 0;
            taxpayerGroup.Header.Appearance.TextHAlign = HAlign.Center;
            var indicatorGroup = e.Layout.Bands[0].Groups.Add("IndicatorName", string.Empty);
            var sumPaymentGroup = e.Layout.Bands[0].Groups.Add("SumPayment", string.Empty);
            var sumReductionGroup = e.Layout.Bands[0].Groups.Add("SumReduction", string.Empty);
            e.Layout.Bands[0].Columns["ID"].Hidden = true;

            var orgChangeColumn = e.Layout.Bands[0].Columns.Add("OrgChange", string.Empty);
            orgChangeColumn.Style = ColumnStyle.Button;
            orgChangeColumn.ButtonDisplayStyle = ButtonDisplayStyle.Always;
            orgChangeColumn.Header.VisiblePosition = 0;
            orgChangeColumn.Width = 20;
            orgChangeColumn.Group = taxpayerGroup;

            e.Layout.Bands[0].Columns["INN"].MergedCellStyle = MergedCellStyle.Always;
            e.Layout.Bands[0].Columns["INN"].Header.Caption = "ИНН";
            e.Layout.Bands[0].Columns["INN"].Header.VisiblePosition = 1;
            e.Layout.Bands[0].Columns["INN"].SortIndicator = SortIndicator.Disabled;
            e.Layout.Bands[0].Columns["INN"].Group = taxpayerGroup;
            e.Layout.Bands[0].Columns["INN"].CellActivation = Activation.NoEdit;

            e.Layout.Bands[0].Columns["Name"].MergedCellStyle = MergedCellStyle.Always;
            e.Layout.Bands[0].Columns["Name"].Header.Caption = "Наименование";
            e.Layout.Bands[0].Columns["Name"].Width = 200;
            e.Layout.Bands[0].Columns["Name"].Header.VisiblePosition = 2;
            e.Layout.Bands[0].Columns["Name"].SortIndicator = SortIndicator.Disabled;
            e.Layout.Bands[0].Columns["Name"].CellActivation = Activation.NoEdit;
            e.Layout.Bands[0].Columns["Name"].Group = taxpayerGroup;

            e.Layout.Bands[0].Columns["IndicatorName"].Header.Caption = "Наименование показателей";
            e.Layout.Bands[0].Columns["IndicatorName"].Width = 150;
            e.Layout.Bands[0].Columns["IndicatorName"].Header.VisiblePosition = 2;
            e.Layout.Bands[0].Columns["IndicatorName"].SortIndicator = SortIndicator.Disabled;
            e.Layout.Bands[0].Columns["IndicatorName"].CellActivation = Activation.NoEdit;
            e.Layout.Bands[0].Columns["IndicatorName"].Group = indicatorGroup;
            e.Layout.Bands[0].Columns["IndicatorCode"].Hidden = true;

            e.Layout.Bands[0].Columns["SumPayment"].CellMultiLine = DefaultableBoolean.False;
            e.Layout.Bands[0].Columns["SumPayment"].Header.Caption = "Сумма НП к доплате (+)";
            e.Layout.Bands[0].Columns["SumPayment"].Width = 150;
            e.Layout.Bands[0].Columns["SumPayment"].SortIndicator = SortIndicator.Disabled;
            e.Layout.Bands[0].Columns["SumPayment"].MaskDataMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.Raw;
            e.Layout.Bands[0].Columns["SumPayment"].MaskClipMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.Raw;
            e.Layout.Bands[0].Columns["SumPayment"].MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiterals;
            e.Layout.Bands[0].Columns["SumPayment"].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns["SumPayment"].PadChar = '_';
            e.Layout.Bands[0].Columns["SumPayment"].MaskInput = "-nnn,nnn,nnn,nnn,nnn.nn";
            e.Layout.Bands[0].Columns["SumPayment"].Group = sumPaymentGroup;

            e.Layout.Bands[0].Columns["SumReduction"].CellMultiLine = DefaultableBoolean.False;
            e.Layout.Bands[0].Columns["SumReduction"].Header.Caption = "Сумма НП к уменьшению (-)";
            e.Layout.Bands[0].Columns["SumReduction"].Width = 150;
            e.Layout.Bands[0].Columns["SumReduction"].SortIndicator = SortIndicator.Disabled;
            e.Layout.Bands[0].Columns["SumReduction"].MaskDataMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.Raw;
            e.Layout.Bands[0].Columns["SumReduction"].MaskClipMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.Raw;
            e.Layout.Bands[0].Columns["SumReduction"].MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiterals;
            e.Layout.Bands[0].Columns["SumReduction"].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns["SumReduction"].PadChar = '_';
            e.Layout.Bands[0].Columns["SumReduction"].MaskInput = "-nnn,nnn,nnn,nnn,nnn.nn";
            e.Layout.Bands[0].Columns["SumReduction"].Group = sumReductionGroup;

            e.Layout.Bands[0].Columns["RefOrg"].Hidden = true;
        }

        void ResultsGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Override.AllowColMoving = AllowColMoving.NotAllowed;
            e.Layout.Override.AllowGroupMoving = AllowGroupMoving.NotAllowed;

            e.Layout.Bands[0].Columns["IndicatorName"].Header.Caption = "Наименование показателей";
            e.Layout.Bands[0].Columns["IndicatorName"].Width = 200;
            e.Layout.Bands[0].Columns["IndicatorName"].SortIndicator = SortIndicator.Disabled;
            e.Layout.Bands[0].Columns["IndicatorCode"].Hidden = true;

            e.Layout.Bands[0].Columns["SumPayment"].CellActivation = Activation.ActivateOnly;
            e.Layout.Bands[0].Columns["SumPayment"].CellMultiLine = DefaultableBoolean.False;
            e.Layout.Bands[0].Columns["SumPayment"].Header.Caption = "Сумма НП к доплате (+)";
            e.Layout.Bands[0].Columns["SumPayment"].Width = 150;
            e.Layout.Bands[0].Columns["SumPayment"].SortIndicator = SortIndicator.Disabled;
            e.Layout.Bands[0].Columns["SumPayment"].MaskDataMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.Raw;
            e.Layout.Bands[0].Columns["SumPayment"].MaskClipMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.Raw;
            e.Layout.Bands[0].Columns["SumPayment"].MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiterals;
            e.Layout.Bands[0].Columns["SumPayment"].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns["SumPayment"].PadChar = '_';
            e.Layout.Bands[0].Columns["SumPayment"].MaskInput = "-nnn,nnn,nnn,nnn,nnn.nn";

            e.Layout.Bands[0].Columns["SumReduction"].CellActivation = Activation.ActivateOnly;
            e.Layout.Bands[0].Columns["SumReduction"].CellMultiLine = DefaultableBoolean.False;
            e.Layout.Bands[0].Columns["SumReduction"].Header.Caption = "Сумма НП к уменьшению (-)";
            e.Layout.Bands[0].Columns["SumReduction"].Width = 150;
            e.Layout.Bands[0].Columns["SumReduction"].SortIndicator = SortIndicator.Disabled;
            e.Layout.Bands[0].Columns["SumReduction"].MaskDataMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.Raw;
            e.Layout.Bands[0].Columns["SumReduction"].MaskClipMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.Raw;
            e.Layout.Bands[0].Columns["SumReduction"].MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiterals;
            e.Layout.Bands[0].Columns["SumReduction"].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns["SumReduction"].PadChar = '_';
            e.Layout.Bands[0].Columns["SumReduction"].MaskInput = "-nnn,nnn,nnn,nnn,nnn.nn";
        }

        #endregion

        void Municipals_EditorButtonClick(object sender, EditorButtonEventArgs e)
        {
            List<string> columns = new List<string>(new string[] { "ID", "Code", "Name" });
            List<object> values = new List<object>();
            if (ChooseRef(ObjectKeys.d_Regions_Plan, string.Empty,
                "Code", columns, ref values))
            {
                ((UltraTextEditor)sender).Text = values[2].ToString();
                ((UltraTextEditor)sender).Tag = values[2];
                MunicipalId = Convert.ToInt64(values[0]);
                BurnRefresh(true);
            }
        }

        private bool ChooseRef(string clsKey, string dataFilter, string codeName, List<string> columns, ref List<object> values)
        {
            frmModalTemplate tmpClsForm = new frmModalTemplate();
            IEntity cls = ConsBudgetForecastNavigation.Instance.Workplace.ActiveScheme.RootPackage.FindEntityByName(clsKey);
            BaseClsUI clsUi = clsKey == ObjectKeys.d_Regions_Plan ? new DataClsUI(cls) : (BaseClsUI)new DataClsUI(cls);
            clsUi.AdditionalFilter = dataFilter;
            clsUi.Workplace = ConsBudgetForecastNavigation.Instance.Workplace;
            clsUi.Initialize();
            clsUi.InitModalCls(-1);
            clsUi.CurrentDataSourceYear = CurrentYear;
            if (clsKey == ObjectKeys.d_Org_TaxBenPay)
            {
                clsUi.MaxFactTableRecordCount = 1000;
            }
            clsUi.RefreshAttachedData();
            tmpClsForm.SuspendLayout();
            try
            {
                tmpClsForm.AttachCls(clsUi);
                ComponentCustomizer.CustomizeInfragisticsControls(tmpClsForm);
            }
            finally
            {
                tmpClsForm.ResumeLayout();
            }
            if (clsKey == ObjectKeys.d_Org_TaxBenPay)
            {
                clsUi.UltraGridExComponent.ugData.DisplayLayout.GroupByBox.Hidden = true;
                clsUi.UltraGridExComponent.ugFilter.DisplayLayout.GroupByBox.Hidden = true;
                clsUi.UltraGridExComponent.ugFilter.DisplayLayout.Bands[0].Columns[UltraGridEx.StateColumnName].Hidden = true;
            }
            clsUi.UltraGridExComponent.ugData.DisplayLayout.Bands[0].Columns[codeName].SortIndicator =
                SortIndicator.Ascending;
            if (tmpClsForm.ShowDialog() == DialogResult.OK)
            {
                UltraGridRow activeRow = UltraGridHelper.GetActiveRowCells(clsUi.UltraGridExComponent.ugData);
                foreach (string columnName in columns)
                {
                    values.Add(activeRow.Cells[columnName].Value);
                }
                return true;
            }
            return false;
        }

        #region работа с данными

        private void CancelChanges()
        {
            TaxpayersData.RejectChanges();
            ResultData.RejectChanges();
            BurnChangeData(false);
        }

        private void GetResult()
        {
            int yearDayUNV = GetYearDayUNV();
            ResultData = GetResultTable();
            var dtResultsData = TaxpayersService.GetResultsSum(MunicipalId, yearDayUNV, SourceId);
            var newRow = ResultData.NewRow();
            newRow["IndicatorName"] = "Всего";
            newRow["IndicatorCode"] = 0;
            newRow["SumPayment"] = dtResultsData.Rows[0].IsNull("SumPayment") ? 0 : Decimal.Divide(Convert.ToDecimal(dtResultsData.Rows[0]["SumPayment"]), 1000);
            newRow["SumReduction"] = dtResultsData.Rows[0].IsNull("SumReduction") ? 0 : Decimal.Divide(Convert.ToDecimal(dtResultsData.Rows[0]["SumReduction"]), 1000);
            ResultData.Rows.Add(newRow);
            newRow = ResultData.NewRow();
            newRow["IndicatorName"] = "Федеральный бюджет";
            newRow["IndicatorCode"] = 1;
            newRow["SumPayment"] = dtResultsData.Rows[1].IsNull("SumPayment") ? 0 : Decimal.Divide(Convert.ToDecimal(dtResultsData.Rows[1]["SumPayment"]), 1000);
            newRow["SumReduction"] = dtResultsData.Rows[1].IsNull("SumReduction") ? 0 : Decimal.Divide(Convert.ToDecimal(dtResultsData.Rows[1]["SumReduction"]), 1000);
            ResultData.Rows.Add(newRow);
            newRow = ResultData.NewRow();
            newRow["IndicatorName"] = "Областной бюджет";
            newRow["IndicatorCode"] = 2;
            newRow["SumPayment"] = dtResultsData.Rows[2].IsNull("SumPayment") ? 0 : Decimal.Divide(Convert.ToDecimal(dtResultsData.Rows[2]["SumPayment"]), 1000);
            newRow["SumReduction"] = dtResultsData.Rows[2].IsNull("SumReduction") ? 0 : Decimal.Divide(Convert.ToDecimal(dtResultsData.Rows[2]["SumReduction"]), 1000);
            ResultData.Rows.Add(newRow);
            ResultData.AcceptChanges();
        }

        private DataTable GetResultTable()
        {
            var dtResults = new DataTable();
            dtResults.Columns.Add("IndicatorName", typeof(string));
            dtResults.Columns.Add("IndicatorCode", typeof(string));
            dtResults.Columns.Add("SumPayment", typeof(decimal));
            dtResults.Columns.Add("SumReduction", typeof(decimal));
            return dtResults;
        }

        private DataTable GetTaxpayersTable()
        {
            var dtTaxpayersSum = new DataTable();
            dtTaxpayersSum.Columns.Add("ID", typeof(int));
            dtTaxpayersSum.Columns.Add("IndicatorName", typeof(string));
            dtTaxpayersSum.Columns.Add("IndicatorCode", typeof(int));
            dtTaxpayersSum.Columns.Add("INN", typeof(string));
            dtTaxpayersSum.Columns.Add("Name", typeof(string));
            dtTaxpayersSum.Columns.Add("SumPayment", typeof(decimal));
            dtTaxpayersSum.Columns.Add("SumReduction", typeof(decimal));
            dtTaxpayersSum.Columns.Add("RefOrg", typeof(decimal));
            return dtTaxpayersSum;
        }

        private void LoadEmptyData()
        {
            TaxpayersData = GetTaxpayersTable();
            ViewObject.ResultsGrid.DataSource = GetResultTable();
            ViewObject.TaxpayerDataGrid.DataSource = null;
            ViewObject.TaxpayerDataGrid.DataSource = TaxpayersData;
        }

        private void LoadData()
        {
            GetResult();
            ViewObject.ResultsGrid.DataSource = ResultData;
            TaxpayersData = GetTaxpayersTable();
            ViewObject.TaxpayerDataGrid.DataSource = null;
            ViewObject.TaxpayerDataGrid.DataSource = TaxpayersData;
            BurnRefresh(false);
            BurnChangeData(false);
            Taxpayers.Clear();
        }

        private void SaveData()
        {
            ViewObject.TaxpayerDataGrid.UpdateData();
            TaxpayersService.SaveData(ResultData, TaxpayersData, SourceId, MunicipalId, GetYearDayUNV());
            TaxpayersData.AcceptChanges();
            BurnRefresh(false);
            BurnChangeData(false);
            ResultData.AcceptChanges();
            TaxpayersData.AcceptChanges();
        }

        #endregion

        private void BurnRefresh(bool isBurn)
        {
            if (isBurn)
            {
                InfragisticsHelper.BurnTool(ViewObject.tbManager.Tools["RefreshData"], true);
            }
            else
            {
                InfragisticsHelper.BurnTool(ViewObject.tbManager.Tools["RefreshData"], false);
            }
        }

        private void BurnChangeData(bool isBurn)
        {
            if (isBurn)
            {
                InfragisticsHelper.BurnTool(ViewObject.tbManager.Tools["CancelChanges"], true);
                InfragisticsHelper.BurnTool(ViewObject.tbManager.Tools["SaveChanges"], true);
            }
            else
            {
                InfragisticsHelper.BurnTool(ViewObject.tbManager.Tools["CancelChanges"], false);
                InfragisticsHelper.BurnTool(ViewObject.tbManager.Tools["SaveChanges"], false);
            }
        }

        private int GetYearDayUNV()
        {
            return Convert.ToInt32(ViewObject.cbYears.SelectedItem)*10000 + 9990 +
                Convert.ToInt32(ViewObject.Quarter.SelectedItem);
        }

        #region

        private void DeleteTaxpayer()
        {
            var activeGridRow = ViewObject.TaxpayerDataGrid.ActiveRow;
            if (activeGridRow == null)
                return;
            var refOrg = activeGridRow.Cells["RefOrg"].Value;

            if (MessageBox.Show("Удалить данные выбранного налогоплательщика?", "Удаление данных",MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.Yes)
            {
                TaxpayersService.DeleteOrgData(refOrg, MunicipalId, SourceId, GetYearDayUNV());
                var deletedRows = TaxpayersData.Select(string.Format("RefOrg = {0}", refOrg));
                for (int i = 0; i < deletedRows.Length; i++)
                {
                    var resultRow = ResultData.Select(string.Format("IndicatorCode = {0}", deletedRows[i]["IndicatorCode"]))[0];
                    if (!deletedRows[i].IsNull("SumPayment"))
                    {
                        decimal value = Convert.ToDecimal(deletedRows[i]["SumPayment"]);
                        resultRow["SumPayment"] = Convert.ToDecimal(resultRow["SumPayment"]) - value;
                    }
                    if (!deletedRows[i].IsNull("SumReduction"))
                    {
                        decimal value = Convert.ToDecimal(deletedRows[i]["SumReduction"]);
                        resultRow["SumReduction"] = Convert.ToDecimal(resultRow["SumReduction"]) - value;
                    }

                    deletedRows[i].Delete();
                    deletedRows[i].AcceptChanges();
                }
                TaxpayersService.SaveResultData(ResultData, SourceId, MunicipalId, GetYearDayUNV());
                ResultData.AcceptChanges();
                Taxpayers.Remove(Convert.ToInt64(refOrg));

            }
        }

        private void ChangeTaxpayer(object oldRefOrg, object newRefOrg, object newOrgName, object newOgrInn)
        {
            var newDataRows = TaxpayersService.GetTaxpayerSum(MunicipalId, Convert.ToInt64(newRefOrg), GetYearDayUNV(), SourceId);
            var rows = TaxpayersData.Select(string.Format("RefOrg = {0}", oldRefOrg));
            for (int i = 0; i <= rows.Length - 1; i++)
            {
                rows[i]["RefOrg"] = newRefOrg;
                rows[i]["INN"] = newOgrInn;
                rows[i]["Name"] = newOrgName;
                rows[i]["SumPayment"] = newDataRows.Rows[0].IsNull("SumPayment") 
                    ? newDataRows.Rows[0]["SumPayment"] 
                    : Decimal.Divide(Convert.ToDecimal(newDataRows.Rows[0]["SumPayment"]), 1000);
                rows[i]["SumReduction"] = newDataRows.Rows[0].IsNull("SumReduction")
                    ? newDataRows.Rows[0]["SumReduction"]
                    : Decimal.Divide(Convert.ToDecimal(newDataRows.Rows[0]["SumReduction"]), 1000);
            }
        }

        private void AddNewTaxpayer()
        {
            var columns = new List<string>(new string[]{"ID", "INN", "Name"});
            var values = new List<object>();
            if (ChooseRef(ObjectKeys.d_Org_TaxBenPay, string.Empty, "INN", columns, ref values))
            {
                long refOrg = Convert.ToInt64(values[0]);
                if (Taxpayers.Contains(refOrg))
                {
                    MessageBox.Show(
                        "Данные по выбранному налогоплательщику уже присутствуют в интерфейсе. Пожалуйста, выберите другого налогоплательщика",
                        "Выбор налогоплательщика", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                Taxpayers.Add(refOrg);
                DataTable dtRegionData = TaxpayersService.GetTaxpayerSum(MunicipalId, Convert.ToInt64(values[0]), GetYearDayUNV(), SourceId);
                var resultRow = TaxpayersData.NewRow();
                resultRow["ID"] = TaxpayersData.Rows.Count + 1;
                resultRow["IndicatorName"] = "Всего";
                resultRow["IndicatorCode"] = 0;
                resultRow["INN"] = values[1];
                resultRow["Name"] = values[2];
                resultRow["SumPayment"] = dtRegionData.Rows[0].IsNull("SumPayment") ? 0 : 
                    Decimal.Divide(Convert.ToDecimal(dtRegionData.Rows[0]["SumPayment"]), 1000);
                resultRow["SumReduction"] = dtRegionData.Rows[0].IsNull("SumReduction") ? 0 : 
                    Decimal.Divide(Convert.ToDecimal(dtRegionData.Rows[0]["SumReduction"]), 1000);
                resultRow["RefOrg"] = values[0];
                TaxpayersData.Rows.Add(resultRow);
                var fedBudRow = TaxpayersData.NewRow();
                fedBudRow["ID"] = TaxpayersData.Rows.Count + 1;
                fedBudRow["IndicatorName"] = "Федеральный бюджет";
                fedBudRow["IndicatorCode"] = 1;
                fedBudRow["INN"] = values[1];
                fedBudRow["Name"] = values[2];
                fedBudRow["SumPayment"] = dtRegionData.Rows[1].IsNull("SumPayment") ? 0 : 
                    Decimal.Divide(Convert.ToDecimal(dtRegionData.Rows[1]["SumPayment"]), 1000);
                fedBudRow["SumReduction"] = dtRegionData.Rows[1].IsNull("SumReduction") ? 0 : 
                    Decimal.Divide(Convert.ToDecimal(dtRegionData.Rows[1]["SumReduction"]), 1000);
                fedBudRow["RefOrg"] = values[0];
                TaxpayersData.Rows.Add(fedBudRow);
                var oblBudRow = TaxpayersData.NewRow();
                oblBudRow["ID"] = TaxpayersData.Rows.Count + 1;
                oblBudRow["IndicatorName"] = "Областной бюджет";
                oblBudRow["IndicatorCode"] = 2;
                oblBudRow["INN"] = values[1];
                oblBudRow["Name"] = values[2];
                oblBudRow["SumPayment"] = dtRegionData.Rows[2].IsNull("SumPayment") ? 0 : 
                    Decimal.Divide(Convert.ToDecimal(dtRegionData.Rows[2]["SumPayment"]), 1000);
                oblBudRow["SumReduction"] = dtRegionData.Rows[2].IsNull("SumReduction") ? 0 : 
                    Decimal.Divide(Convert.ToDecimal(dtRegionData.Rows[2]["SumReduction"]), 1000);
                oblBudRow["RefOrg"] = values[0];
                TaxpayersData.Rows.Add(oblBudRow);
                TaxpayersData.AcceptChanges();
            }
        }

        #endregion
    }
}
