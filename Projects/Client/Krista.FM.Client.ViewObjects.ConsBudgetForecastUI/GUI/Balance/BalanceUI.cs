using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Excel;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinGrid.ExcelExport;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.Common;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.DataCls;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.Common;
using Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.GUI.Handbooks;
using Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.Server;
using Krista.FM.ServerLibrary;
using ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle;
using ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle;
using GetBoolDelegate = Krista.FM.Client.Common.GetBoolDelegate;

namespace Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.GUI.Balance
{
    public class BalanceUI : BaseViewObj
    {
        public BalanceUI(string key)
            : base (key)
        {
            Caption = "Балансировка";
        }

        #region свойства

        internal BalanceView ViewObject
        {
            get; set;
        }

        internal object IncomesVariant
        {
            get; set;
        }

        internal int Year
        {
            get; set;
        }

        internal int VariantYear
        {
            get; set;
        }

        internal object ChargeVariant
        {
            get; set;
        }

        internal object IFVariant
        {
            get; set;
        }

        internal BalanceService BalanceService
        {
            get; set;
        }

        internal List<string> TaxNoTaxCodes
        {
            get; set;
        }

        internal Dictionary<string, string> TaxNoTaxNames
        {
            get; set;
        }

        internal List<string> FreeSupplyCodes
        {
            get; set;
        }

        internal Dictionary<string, string> FreeSupplyNames
        {
            get; set;
        }

        internal List<string> IncomeWorkCodes
        {
            get; set;
        }

        internal Dictionary<string, string> IncomeWorkNames
        {
            get; set;
        }

        internal Dictionary<FixedRowIndex, frmModalTemplate> SelectCodesForms
        {
            get; set;
        }

        

        #endregion

        protected override void SetViewCtrl()
        {
            fViewCtrl = new BalanceView();
            fViewCtrl.ViewContent = this;
        }

        public override void Initialize()
        {
            base.Initialize();

            BalanceService = new BalanceService(Workplace.ActiveScheme);

            ViewObject = (BalanceView) fViewCtrl;
            ViewObject.tbManager.ToolClick += tbManager_ToolClick;

            ViewObject.ugData.InitializeLayout += ugData_InitializeLayout;
            ViewObject.ugData.InitializeRow += ugData_InitializeRow;
            ViewObject.ugData.ClickCellButton += ugData_ClickCellButton;
            ViewObject.ugData.MouseEnterElement += ugData_MouseEnterElement;
            ViewObject.ugData.MouseLeaveElement += ugData_MouseLeaveElement;
            Year = -1;
            IncomesVariant = null;
            ChargeVariant = null;
            IFVariant = null;

            ViewObject.tbManager.AfterToolCloseup += tbManager_AfterToolCloseup;
            ViewObject.tbManager.ToolValueChanged += tbManager_ToolValueChanged;

            TaxNoTaxCodes = new List<string>();
            FreeSupplyCodes = new List<string>();
            IncomeWorkCodes = new List<string>();
            TaxNoTaxNames = new Dictionary<string, string>();
            FreeSupplyNames = new Dictionary<string, string>();
            IncomeWorkNames = new Dictionary<string, string>();
            SelectCodesForms = new Dictionary<FixedRowIndex, frmModalTemplate>();
        }

        void tbManager_ToolValueChanged(object sender, ToolEventArgs e)
        {
            switch(e.Tool.Key)
            {
                case "cbYear":
                    Year = Convert.ToInt32(((ComboBoxTool)e.Tool).Value); 
                    break;
            }
        }

        #region настройка грида

        void ugData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            Dictionary<string, string> regions = BalanceService.Regions;
            e.Layout.UseFixedHeaders = true;
            // настраиваем объединение записей по коду и наименованию КД
            e.Layout.Bands[0].Columns["Code"].MergedCellStyle = MergedCellStyle.Always;
            e.Layout.Bands[0].Columns["Code"].MergedCellContentArea = MergedCellContentArea.VirtualRect;
            e.Layout.Bands[0].Columns["Code"].Width = 110;
            e.Layout.Bands[0].Columns["Code"].CellAppearance.BackColor = Color.LightGoldenrodYellow;
            e.Layout.Bands[0].Columns["Name"].MergedCellStyle = MergedCellStyle.Always;
            e.Layout.Bands[0].Columns["Name"].MergedCellContentArea = MergedCellContentArea.VirtualRect;
            e.Layout.Bands[0].Columns["Name"].CellAppearance.BackColor = Color.LightGoldenrodYellow;
            e.Layout.Bands[0].Columns["Name"].MergedCellAppearance.TextVAlign = VAlign.Top;
            e.Layout.Bands[0].Columns["Name"].Width = 350;
            e.Layout.Bands[0].Columns["Name"].CellMultiLine = DefaultableBoolean.False;
            
            e.Layout.Bands[0].Groups.Add("Code", "Классификация");
            e.Layout.Bands[0].Groups.Add("Name", "Наименование");
            e.Layout.Bands[0].Columns["Index"].Hidden = true;

            // называем по русски все поля. Плюс добавляем поля, которые будут объединять остальные по признаку района
            foreach (KeyValuePair<string , string> kvp in regions)
            {
                e.Layout.Bands[0].Groups.Add(kvp.Key, kvp.Value);
            }

            foreach (UltraGridColumn column in e.Layout.Bands[0].Columns)
            {
                column.AllowRowFiltering = DefaultableBoolean.False;
                column.SortIndicator = SortIndicator.Disabled;
                if (column.Key.Contains("_"))
                {
                    column.CellMultiLine = DefaultableBoolean.False;
                    string refRegion = column.Key.Split('_')[0];
                    column.Group = e.Layout.Bands[0].Groups[refRegion];
                    column.MaskDataMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.Raw;
                    column.MaskClipMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.Raw;
                    column.MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiterals;
                    column.CellAppearance.TextHAlign = HAlign.Right;
                    column.PadChar = '_';
                    column.MaskInput = "-nnn,nnn,nnn,nnn.nn";
                    column.Width = 110;
                }
                else
                {
                    if (!column.Hidden)
                    {
                        if (e.Layout.Bands[0].Groups.Exists(column.Key))
                            column.Group = e.Layout.Bands[0].Groups[column.Key];
                        else
                            column.Hidden = true;
                        column.CellActivation = Activation.NoEdit;
                    }
                }
                if (column.Key.Contains("Result"))
                {
                    column.CellAppearance.BackColor = Color.LightGoldenrodYellow;
                }
                column.CellActivation = Activation.ActivateOnly;
            }

            e.Layout.Bands[0].GroupHeaderLines = 3;
            e.Layout.Bands[0].ColHeaderLines = 3;

            e.Layout.Override.FixedHeaderIndicator = FixedHeaderIndicator.None;
            e.Layout.Override.AllowGroupMoving = AllowGroupMoving.NotAllowed;
            e.Layout.Override.AllowColMoving = AllowColMoving.NotAllowed;
            e.Layout.GroupByBox.Hidden = true;

            e.Layout.Bands[0].Groups["Code"].Header.Fixed = true;
            e.Layout.Bands[0].Groups["Name"].Header.Fixed = true;
        }

        void ugData_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            UltraGridRow row = e.Row;
            if (row.Cells["Index"].Value != null && row.Cells["Index"].Value != DBNull.Value)
            {
                var rowIndex = (FixedRowIndex)Convert.ToInt32(row.Cells["Index"].Value);
                if (rowIndex != FixedRowIndex.IncomeCode)
                    row.CellAppearance.FontData.Bold = DefaultableBoolean.True;
                
                if (rowIndex == FixedRowIndex.IncomeWork || rowIndex == FixedRowIndex.FreeSupply || rowIndex == FixedRowIndex.TaxNoTax)
                {
                    row.Cells["Name"].Column.ButtonDisplayStyle = ButtonDisplayStyle.Always; 
                    row.Cells["Name"].Style = ColumnStyle.EditButton;
                }
            }
        }

        #endregion

        void tbManager_ToolClick(object sender, ToolClickEventArgs e)
        {
            string toolKey = e.Tool.Key;
            switch (toolKey)
            {
                case "btnIncomesVariant":
                    string variantCaption = string.Empty;
                    int variantYear = 0;
                    int variantId = 0;
                    if (ChooseVariant(ObjectKeys.d_Variant_PlanIncomes, ref variantCaption, ref variantYear, ref variantId))
                    {
                        IncomesVariant = variantId;
                        e.Tool.SharedProps.Caption = variantCaption;
                        FillYears(variantYear);
                        VariantYear = variantYear;
                        BurnRefresh(true);
                    }
                    break;
                case "btnChargeVariant":
                    variantCaption = string.Empty;
                    variantYear = 0;
                    variantId = 0;
                    if (ChooseVariant(ObjectKeys.d_Variant_PlanOutcomes, ref variantCaption, ref variantYear, ref variantId))
                    {
                        ChargeVariant = variantId;
                        e.Tool.SharedProps.Caption = variantCaption;
                        BurnRefresh(true);
                    }
                    break;
                case "btnIFVariant":
                    variantCaption = string.Empty;
                    variantYear = 0;
                    variantId = 0;
                    if (ChooseVariant(ObjectKeys.d_Variant_Borrow, ref variantCaption, ref variantYear, ref variantId))
                    {
                        IFVariant = variantId;
                        e.Tool.SharedProps.Caption = variantCaption;
                        BurnRefresh(true);
                    }
                    break;
                case "btnGetBalance":
                    if(IncomesVariant == null || ChargeVariant == null || IFVariant == null || Year == -1)
                    {
                        MessageBox.Show("Необходимо выбрать все параметры вывода данных", "Балансировка", MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                        return;
                    }

                    BalanceParams balanceParams = new BalanceParams();
                    balanceParams.ChargeVariant = ChargeVariant;
                    balanceParams.IncomesVariant = IncomesVariant;
                    balanceParams.IFVariant = IFVariant;
                    balanceParams.Year = Year;
                    balanceParams.VariantYear = VariantYear;
                    var balanceIncomeParams = new BalanceIncomeParams(TaxNoTaxCodes, FreeSupplyCodes, IncomeWorkCodes,
                        TaxNoTaxNames, FreeSupplyNames, IncomeWorkNames);
                    DataTable dtBalance = BalanceService.GetData(balanceParams, balanceIncomeParams);
                    ViewObject.ugData.DataSource = null;
                    ViewObject.ugData.DataSource = dtBalance;
                    BurnRefresh(false);
                    break;
                case "btnExport":
                    CreateReport();
                    break;
            }
        }

        private void CreateReport()
        {
            var fileName = "Балансировка";

            if (ExportImportHelper.GetFileName(fileName, ExportImportHelper.fileExtensions.xls, true, ref fileName))
            {
                var exporter = new UltraGridExcelExporter();
                exporter.BeginExport += exporter_BeginExport;
                exporter.EndExport += exporter_EndExport;
                exporter.Export(ViewObject.ugData, fileName);
                Process.Start(fileName);
            }
        }

        private void exporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.Layout.Appearance.FontData.Name = "Arial";
            e.Layout.BorderStyle = UIElementBorderStyle.Solid;
        }

        private void exporter_EndExport(object sender, EndExportEventArgs e)
        {
            var sheet = e.CurrentWorksheet;
            
            sheet.MergedCellsRegions.Add(0, 0, 1, 0);
            sheet.MergedCellsRegions.Add(0, 1, 1, 1);

            for (var i = 0; i < e.CurrentRowIndex; i++)
            {
                var row = sheet.Rows[i];

                for (var j = 0; j < sheet.Columns.Count(); j++)
                {
                    var cellValue = row.Cells[j];

                    if (j == 1)
                    {
                        cellValue.CellFormat.WrapText = ExcelDefaultableBoolean.True;
                    }

                    cellValue.CellFormat.FillPatternForegroundColor = Color.White;

                    if (i > 1)
                    {
                        cellValue.CellFormat.FormatString = "###,###,##0.0";
                        cellValue.CellFormat.BottomBorderColor = Color.Black;
                        cellValue.CellFormat.LeftBorderColor = Color.Black;
                        cellValue.CellFormat.RightBorderColor = Color.Black;
                        cellValue.CellFormat.TopBorderColor = Color.Black;

                        cellValue.CellFormat.BottomBorderStyle = CellBorderLineStyle.Thin;
                        cellValue.CellFormat.LeftBorderStyle = CellBorderLineStyle.Thin;
                        cellValue.CellFormat.RightBorderStyle = CellBorderLineStyle.Thin;
                        cellValue.CellFormat.TopBorderStyle = CellBorderLineStyle.Thin;
                    }
                    else
                    {
                        cellValue.CellFormat.Font.Height = 160;
                        cellValue.CellFormat.Alignment = HorizontalCellAlignment.Center;
                        cellValue.CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                    }
                }
            }
        }

        void tbManager_AfterToolCloseup(object sender, ToolDropdownEventArgs e)
        {
            if (e.Tool.Key == "cbYear")
            {
                Year = Convert.ToInt32(((ComboBoxTool)e.Tool).Value); 
            }
        }

        private void FillYears(int year)
        {
            ComboBoxTool tool = ViewObject.tbManager.Tools["cbYear"] as ComboBoxTool;
            tool.ValueList.ValueListItems.Clear();
            tool.ValueList.ValueListItems.Add(year, year.ToString());
            tool.ValueList.ValueListItems.Add(year + 1, (year + 1).ToString());
            tool.ValueList.ValueListItems.Add(year + 2, (year + 2).ToString());
            tool.SelectedIndex = 0;
        }

        #region выбор варианта

        private bool ChooseVariant(string clsKey, ref string variantCaption, ref int variantYear, ref int variantId)
        {
            frmModalTemplate tmpClsForm = new frmModalTemplate();
            IClassifier cls = ConsBudgetForecastNavigation.Instance.Workplace.ActiveScheme.Classifiers[clsKey];
            BaseClsUI clsUI = new DataClsUI(cls);
            clsUI.Workplace = ConsBudgetForecastNavigation.Instance.Workplace;
            clsUI.Initialize();
            clsUI.InitModalCls(-1);
            clsUI.RefreshAttachedData();
            tmpClsForm.SuspendLayout();
            try
            {
                tmpClsForm.AttachCls(clsUI);
                ComponentCustomizer.CustomizeInfragisticsControls(tmpClsForm);
            }
            finally
            {
                tmpClsForm.ResumeLayout();
            }
            clsUI.UltraGridExComponent.ugData.DisplayLayout.Bands[0].Columns["Code"].SortIndicator =
                SortIndicator.Ascending;
            if (tmpClsForm.ShowDialog() == DialogResult.OK)
            {
                variantCaption = clsUI.UltraGridExComponent.ugData.ActiveRow.Cells["NAME"].Value.ToString();
                if (clsUI.UltraGridExComponent.ugData.DisplayLayout.Bands[0].Columns.Exists("RefYear"))
                    variantYear = Convert.ToInt32(clsUI.UltraGridExComponent.ugData.ActiveRow.Cells["RefYear"].Value);
                variantId = Convert.ToInt32(clsUI.UltraGridExComponent.ugData.ActiveRow.Cells["ID"].Value);
                return true;
            }
            return false;
        }

        #endregion

        #region добавление кодов 

        void ugData_ClickCellButton(object sender, CellEventArgs e)
        {
            var dt = new DataTable();
            var rowIndex = (FixedRowIndex)Convert.ToInt32(e.Cell.Row.Cells["Index"].Value);
            frmModalTemplate form = null;
            if (SelectCodesForms.ContainsKey(rowIndex))
                form = SelectCodesForms[rowIndex];
            else
                SelectCodesForms.Add(rowIndex, form);
            string filter = string.Empty;
            switch (rowIndex)
            {
                case FixedRowIndex.TaxNoTax:
                    filter = "and CodeStr like '___1%'";
                    break;
                case FixedRowIndex.FreeSupply:
                    filter = "and CodeStr like '___2%'";
                    break;
                case FixedRowIndex.IncomeWork:
                    filter = "and CodeStr like '___3%'";
                    break;
            }

            if (GetPlaningKDCodes(filter, ref dt, ref form))
            {
                List<string> codes = new List<string>();
                Dictionary<string, string> names = new Dictionary<string, string>();
                GetCodes(dt, ref codes, ref names);
                switch (rowIndex)
                {
                    case  FixedRowIndex.TaxNoTax:
                        TaxNoTaxCodes = codes;
                        TaxNoTaxNames = names;
                        break;
                    case FixedRowIndex.FreeSupply:
                        FreeSupplyCodes = codes;
                        FreeSupplyNames = names;
                        break;
                    case FixedRowIndex.IncomeWork:
                        IncomeWorkCodes = codes;
                        IncomeWorkNames = names;
                        break;
                }
                BurnRefresh(true);
            }
            SelectCodesForms[rowIndex] = form;
        }

        private bool GetPlaningKDCodes(string filter, ref DataTable dtPlaningKD, ref frmModalTemplate form)
        {
            UltraGrid ultraGrid = null;
            if (form == null)
            {
                IEntity entity = Workplace.ActiveScheme.RootPackage.FindEntityByName(ObjectKeys.d_KD_PlanIncomes);
                DataClsUI planingKdUI = new SelectRowsClsUI(entity, VariantYear);
                // получаем нужный классификатор
                // создаем объект просмотра классификаторов нужного типа
                planingKdUI.AdditionalFilter = filter;
                planingKdUI.Workplace = Workplace;
                planingKdUI.RestoreDataSet = false;
                planingKdUI.Initialize();
                planingKdUI.InitModalCls(-1);
                // создаем форму
                form = new frmModalTemplate();
                form.AttachCls(planingKdUI);
                planingKdUI.RefreshAttachedData();
                ComponentCustomizer.CustomizeInfragisticsControls(form);
                // ...загружаем данные
                foreach (UltraGridBand band in planingKdUI.UltraGridExComponent.ugData.DisplayLayout.Bands)
                {
                    band.Columns["CodeStr_Remasked"].SortIndicator = SortIndicator.Ascending;
                }
                ultraGrid = planingKdUI.UltraGridExComponent.ugData;
            }
            else
                ultraGrid = form.AttachedCls.UltraGridExComponent.ugData;

            if (form.ShowDialog((Form)Workplace) == DialogResult.OK)
            {
                if (ultraGrid.ActiveRow != null)
                    ultraGrid.ActiveRow.Update();
                dtPlaningKD = form.AttachedCls.GetClsDataSet().Tables[0];
                return true;
            }
            return false;
        }

        #region добавление новых строк в грид

        private void GetCodes(DataTable dtCodes, ref List<string> codes, ref Dictionary<string , string> names)
        {
            ViewObject.ugData.BeginUpdate();
            try
            {
                // смотрим те коды, которые у нас были выбраны
                codes = new List<string>();
                names = new Dictionary<string, string>();
                foreach (DataRow selectRow in dtCodes.Select("SelectedRow = true"))
                {
                    codes.Add(selectRow["CodeStr"].ToString());
                    names.Add(selectRow["CodeStr"].ToString(), selectRow["Name"].ToString());
                }
            }
            finally
            {
                ViewObject.ugData.EndUpdate();
            }
        }

        #endregion

        #endregion

        private void BurnRefresh(bool isBurn)
        {
            if (isBurn && IncomesVariant != null)
            {
                InfragisticsHelper.BurnTool(ViewObject.tbManager.Tools["btnGetBalance"], true);
            }
            else
            {
                InfragisticsHelper.BurnTool(ViewObject.tbManager.Tools["btnGetBalance"], false);
            }
        }

        private Infragistics.Win.ToolTip toolTipValue = null;

        public Infragistics.Win.ToolTip ToolTip
        {
            get
            {
                if (null == this.toolTipValue)
                {
                    this.toolTipValue = new Infragistics.Win.ToolTip(fViewCtrl);
                    this.toolTipValue.DisplayShadow = true;
                    this.toolTipValue.AutoPopDelay = 0;
                    this.toolTipValue.InitialDelay = 0;
                }
                return this.toolTipValue;
            }
        }

        private void ShowToolTip(Control parentControl, UIElement uiElement, string text)
        {
            ToolTip.ToolTipText = text;
            var tooltipPos = new Point(uiElement.Rect.Left, uiElement.Rect.Bottom);
            ToolTip.Show(parentControl.PointToScreen(tooltipPos));
        }

        void ugData_MouseLeaveElement(object sender, UIElementEventArgs e)
        {
            var element = e.Element as EditButtonUIElement;
            if (element != null)
            {
                ToolTip.Hide();
            }
        }

        void ugData_MouseEnterElement(object sender, UIElementEventArgs e)
        {
            /*
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"D:\Elements.txt", true))
            {
                file.WriteLine(e.Element.ToString());
            }  
            */
            var element = e.Element as EditButtonUIElement;

            if (element != null)
            {
                var rowIndex = (FixedRowIndex)Convert.ToInt32(element.Row.Cells["Index"].Value);
                switch (rowIndex)
                {
                    case FixedRowIndex.TaxNoTax:
                        ShowToolTip(ViewObject.ugData, element, "Вывести отдельные коды по налоговым и неналоговым доходам");
                        break;
                    case FixedRowIndex.IncomeWork:
                        ShowToolTip(ViewObject.ugData, element, "Вывести отдельные коды по безвозмездным поступлениям");
                        break;
                    case FixedRowIndex.FreeSupply:
                        ShowToolTip(ViewObject.ugData, element, "Вывести отдельные коды по доходам от приносящей доход деятельности");
                        break;
                }
            }
        }
    }
}
