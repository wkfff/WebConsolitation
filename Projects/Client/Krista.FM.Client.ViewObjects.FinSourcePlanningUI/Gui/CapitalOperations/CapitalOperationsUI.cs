using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.Components;
using Krista.FM.Client.Reports;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.CapitalOperations;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.CapitalOperations.DataWarning;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Forms;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.CapitalPlanningOperations
{
    public class CapitalOperationsUI : BaseCapitalOperationsUI
    {
        internal CapitalOperationsView ViewObject
        {
            get { return (CapitalOperationsView)fViewCtrl; }
        }

        public CapitalOperationsUI(string key)
            : base(key)
        {
            Caption = "Планирование операций с ценными бумагами";
        }

        private DataTable dtParams;
        private DataTable dtNominal;
        private DataTable dtCoupons;
        private DataTable dtCapitalBond;
        private DataTable dtNominalBond;
        private DataTable dtPaymentBond;

        private IEntity ParamsEntity
        { 
            get; set;
        }
        private IEntity NominalEntity
        {
            get; set;
        }
        private IEntity CouponEntity
        {
            get; set;
        }
        private IEntity CapitalBondEntity
        {
            get; set;
        }
        private IEntity NominalBondEntity
        {
            get; set;
        }
        private IEntity PaymentBondEntity
        {
            get; set;
        }

        private Dictionary<string, CalculationUniqueParams> calculations;

        protected override void SetViewCtrl()
        {
            fViewCtrl = new CapitalOperationsView();
            fViewCtrl.ViewContent = this;
        }

        public override void Initialize()
        {
            base.Initialize();

            WarningList = new Dictionary<string, DataWarningNotifier>();

            ParamsEntity =
                Workplace.ActiveScheme.RootPackage.FindEntityByName(CapitalOperationsKeys.f_S_ICalcParam);
            NominalEntity =
                Workplace.ActiveScheme.RootPackage.FindEntityByName(CapitalOperationsKeys.t_S_IRepaymentNom);
            CouponEntity =
                Workplace.ActiveScheme.RootPackage.FindEntityByName(CapitalOperationsKeys.t_S_ICoupons);
            CapitalBondEntity =
                Workplace.ActiveScheme.RootPackage.FindEntityByName(CapitalOperationsKeys.f_S_IssueBond);
            NominalBondEntity =
                    Workplace.ActiveScheme.RootPackage.FindEntityByName(CapitalOperationsKeys.t_S_INom);
            PaymentBondEntity =
                    Workplace.ActiveScheme.RootPackage.FindEntityByName(CapitalOperationsKeys.t_S_IPaymnts);

            ViewObject.ToolbarsManager.ToolClick += ToolbarsManager_ToolClick;
            ViewObject.ToolbarsManager.ToolValueChanged += ToolbarsManager_ToolValueChanged;
            ViewObject.ugCapitalBond.InitializeLayout += ugCapitalBond_InitializeLayout;
            ViewObject.ugCapitalBond.BeforeColumnChooserDisplayed += BeforeColumnChooserDisplayed;
            ViewObject.ug1.InitializeLayout += ug1_InitializeLayout;
            ViewObject.ug1.BeforeColumnChooserDisplayed += BeforeColumnChooserDisplayed;
            ViewObject.ug2.InitializeLayout += ug2_InitializeLayout;
            ViewObject.ug2.BeforeColumnChooserDisplayed += BeforeColumnChooserDisplayed;
            ViewObject.ug3.InitializeLayout += ug3_InitializeLayout;
            ViewObject.ug3.BeforeColumnChooserDisplayed += BeforeColumnChooserDisplayed;
            ViewObject.ugCoupon.InitializeLayout += ugCoupon_InitializeLayout;
            ViewObject.ugNominal.InitializeLayout += ugNominal_InitializeLayout;
            ViewObject.ugCoupon.InitializeRow += ugCoupon_InitializeRow;
            ViewObject.ugCoupon.AfterCellUpdate += new CellEventHandler(ugCoupon_AfterCellUpdate);

            SetEditorCheck(ViewObject.ne2);
            SetEditorCheck(ViewObject.ne5);
            SetEditorCheck(ViewObject.ne6); 

            ViewObject.ugCapitalBond.MouseClick += Grid_MouseDown;
            ViewObject.ug1.MouseClick += Grid_MouseDown;
            ViewObject.ug2.MouseClick += Grid_MouseDown;
            ViewObject.ug3.MouseClick += Grid_MouseDown;

            ViewObject.CouponsClear.Click += CouponsClear_Click;

            ViewObject.cbIsConstRate.CheckedChanged += new EventHandler(cbIsConstRate_CheckedChanged);

            ViewObject.btnClearNominalPays.Click += btnClearNominalPays_Click;

            calculations = new Dictionary<string, CalculationUniqueParams>();
            LoadEmptyData();
            LoadCalculations(true);
            bool enableCalculation = CheckCalculatePermission();
            BurnCalculationButton(enableCalculation);
        }

        void CouponsClear_Click(object sender, EventArgs e)
        {
            ViewObject.ClearCoupons();
        }

        void ugCoupon_AfterCellUpdate(object sender, CellEventArgs e)
        {
            if (string.Compare(e.Cell.Column.Key, "CouponRate", true) == 0)
            {
                if (e.Cell.Value != null && e.Cell.Value != DBNull.Value)
                {
                    decimal rowValue = Convert.ToDecimal(e.Cell.Value);
                    string rowIndex = Convert.ToInt32(e.Cell.Row.Cells["Num"].Value).ToString();
                    if (rowValue > Convert.ToDecimal(99.99))
                    {
                        if (!InvalidDataEditors.Contains(rowIndex))
                        {
                            InvalidDataEditors.Add(rowIndex);
                            BurnGridCellWarning(true, e.Cell);
                        }
                    }
                    else
                    {
                        if (InvalidDataEditors.Contains(rowIndex))
                        {
                            InvalidDataEditors.Remove(rowIndex);
                            BurnGridCellWarning(false, e.Cell);
                        }
                    }
                }
            }
        }

        void ugCoupon_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            if (e.Row.Cells["CouponRate"].Value != null && e.Row.Cells["CouponRate"].Value != DBNull.Value)
            {
                decimal rowValue = Convert.ToDecimal(e.Row.Cells["CouponRate"].Value);
                string rowIndex = Convert.ToInt32(e.Row.Cells["Num"].Value).ToString();
                if (rowValue > Convert.ToDecimal(99.99))
                {
                    if (!InvalidDataEditors.Contains(rowIndex))
                    {
                        InvalidDataEditors.Add(rowIndex);
                        BurnGridCellWarning(true, e.Row.Cells["CouponRate"]);
                    }
                }
                else
                {
                    if (InvalidDataEditors.Contains(rowIndex))
                    {
                        InvalidDataEditors.Remove(rowIndex);
                        BurnGridCellWarning(false, e.Row.Cells["CouponRate"]);
                    }
                }
            }
        }

        void btnClearCoupons_Click(object sender, EventArgs e)
        {
            ViewObject.ClearCoupons();
        }

        void btnClearNominalPays_Click(object sender, EventArgs e)
        {
            ViewObject.ClearNominalPays();
        }

        void cbIsConstRate_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb == null)
                return;
            if (!cb.Checked)
            {
                ViewObject.rb1.Checked = false;
                ViewObject.rb2.Checked = true;
                ViewObject.ne1.Value = 0;
            }
            else
            {
                ViewObject.ne1.ReadOnly = ViewObject.rb1.Checked;
                ViewObject.ne1.Appearance.ResetBackColor();
            }
            ViewObject.rb1.Enabled = cb.Checked;
            ViewObject.ne1.Enabled = cb.Checked;
        }

        private Point ColumnChooserPoint
        {
            get; set;
        }

        void Grid_MouseDown(object sender, MouseEventArgs e)
        {
            UltraGrid grid = (UltraGrid)sender;

            ColumnChooserButtonUIElement columnChooserButtonUIElement = grid.DisplayLayout.UIElement.LastElementEntered.Parent as ColumnChooserButtonUIElement;
            UIElement uiElement = grid.DisplayLayout.UIElement.LastElementEntered;

            while (columnChooserButtonUIElement == null && uiElement != null)
            {
                columnChooserButtonUIElement = uiElement.Parent as ColumnChooserButtonUIElement;
                uiElement = uiElement.Parent;
            }
            
            if (columnChooserButtonUIElement != null)
            {
                ColumnChooserPoint =
                    grid.PointToScreen(new Point(columnChooserButtonUIElement.Rect.Left,
                                                 columnChooserButtonUIElement.Rect.Bottom));
            }
        }

        #region Инициализация гридов 

        void BeforeColumnChooserDisplayed(object sender, BeforeColumnChooserDisplayedEventArgs e)
        {
            //e.Dialog.Size = new Size(400, 100);

            // By default UltraGrid retains the column chooser dialog instance. You can 
            // set the DisposeOnClose to True to cause the UltraGrid to dispose the dialog 
            // when it's closed by the user.
            ColumnChooserDialog ccDialog = e.Dialog;
            ccDialog.SetDesktopLocation(ColumnChooserPoint.X, ColumnChooserPoint.Y);

            ccDialog.ShowInTaskbar = false;
            ccDialog.ShowIcon = false;
            ccDialog.FormBorderStyle = FormBorderStyle.FixedDialog;
            ccDialog.Text = "Выбор колонок";
            ccDialog.MaximizeBox = false;
            ccDialog.MinimizeBox = false;
            ccDialog.DialogResult = DialogResult.OK;
            ccDialog.StartPosition = FormStartPosition.Manual;

            //ccDialog.AutoSize = true;
            ccDialog.ColumnChooserControl.AutoSize = false;
            ccDialog.ColumnChooserControl.ColumnDisplayOrder = ColumnDisplayOrder.SameAsGrid;
            e.Dialog.Size = ccDialog.ColumnChooserControl.GetIdealSize();
            // You can use the ColumnChooserControl property of the dialog to access the
            // column chooser control that actually displays the list of the columns.
            e.Dialog.ColumnChooserControl.MultipleBandSupport = MultipleBandSupport.SingleBandOnly;
            e.Dialog.ColumnChooserControl.Style = ColumnChooserStyle.AllColumnsWithCheckBoxes;

            // By default column chooser attempts to look similar to the source grid whose
            // columns are being displayed in the column chooser. You can set the
            // SyncLookWithSourceGrid to false to prevent column chooser from doing this
            // This will also ensure that the column chooser won't override your appearance
            e.Dialog.ColumnChooserControl.SyncLookWithSourceGrid = true;
            e.Dialog.ColumnChooserControl.DisplayLayout.Appearance.BackColor = SystemColors.Window;
            ccDialog.ShowDialog();
            e.Cancel = true;
        }

        void ugCapitalBond_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            UltraGrid grid = (UltraGrid) sender;
            InitializeGrid(CapitalBondEntity, grid);
            e.Layout.Bands[0].Columns["ID"].Hidden = true;
            e.Layout.Bands[0].Columns["SourceID"].Hidden = true;
            e.Layout.Bands[0].Columns["TaskID"].Hidden = true;
            e.Layout.Bands[0].Columns["IDCalcParam"].Hidden = true;

            e.Layout.Override.RowSelectors = DefaultableBoolean.True;
            e.Layout.Override.RowSelectorHeaderStyle = RowSelectorHeaderStyle.ColumnChooserButton;
            e.Layout.Bands[0].Columns["ID"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
            e.Layout.Bands[0].Columns["SourceID"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
            e.Layout.Bands[0].Columns["TaskID"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
            e.Layout.Bands[0].Columns["IDCalcParam"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
        }

        void ug1_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            UltraGrid grid = (UltraGrid)sender;
            InitializeGrid(NominalBondEntity, grid);
            e.Layout.Bands[0].Columns["ID"].Hidden = true;
            e.Layout.Bands[0].Columns["RefIssBonds"].Hidden = true;

            e.Layout.Override.RowSelectors = DefaultableBoolean.True;
            e.Layout.Override.RowSelectorHeaderStyle = RowSelectorHeaderStyle.ColumnChooserButton;
            e.Layout.Bands[0].Columns[0].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
            e.Layout.Bands[0].Columns["ID"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
            e.Layout.Bands[0].Columns["RefIssBonds"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
        }

        void ug2_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            UltraGrid grid = (UltraGrid)sender;
            InitializeGrid(PaymentBondEntity, grid);
            e.Layout.Bands[0].Columns["ID"].Hidden = true;
            e.Layout.Bands[0].Columns["StartDate"].Hidden = true;
            e.Layout.Bands[0].Columns["EndDate"].Hidden = true;
            e.Layout.Bands[0].Columns["PayDate"].Hidden = true;
            e.Layout.Bands[0].Columns["RefIssBonds"].Hidden = true;

            grid.DisplayLayout.Override.SummaryDisplayArea = SummaryDisplayAreas.TopFixed;
            SummarySettings s = grid.DisplayLayout.Bands[0].Summaries.Add(
                    SummaryType.Sum, grid.DisplayLayout.Bands[0].Columns["DayCpnCount"]);
            s.DisplayFormat = "{0:##,##0}";
            s.Appearance.TextHAlign = HAlign.Right;

            s = grid.DisplayLayout.Bands[0].Summaries.Add(
                    SummaryType.Sum, grid.DisplayLayout.Bands[0].Columns["TotalCoupon"]);
            s.DisplayFormat = "{0:##,##0.00#}";
            s.Appearance.TextHAlign = HAlign.Right;

            e.Layout.Override.RowSelectors = DefaultableBoolean.True;
            e.Layout.Override.RowSelectorHeaderStyle = RowSelectorHeaderStyle.ColumnChooserButton;
            e.Layout.Bands[0].Columns["ID"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
            e.Layout.Bands[0].Columns["RefIssBonds"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
            e.Layout.Bands[0].Columns["StartDate"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
            e.Layout.Bands[0].Columns["PayDate"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
            e.Layout.Bands[0].Columns["EndDate"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
        }

        void ug3_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            UltraGrid grid = (UltraGrid)sender;
            InitializeGrid(PaymentBondEntity, grid);
            e.Layout.Bands[0].Columns["ID"].Hidden = true;
            e.Layout.Bands[0].Columns["DayCount"].Hidden = true;
            e.Layout.Bands[0].Columns["RefIssBonds"].Hidden = true;

            grid.DisplayLayout.Override.SummaryDisplayArea = SummaryDisplayAreas.TopFixed;

            SummarySettings s = grid.DisplayLayout.Bands[0].Summaries.Add(
                    SummaryType.Sum, grid.DisplayLayout.Bands[0].Columns["DayCpnCount"]);
            s.DisplayFormat = "{0:##,##0}";
            s.Appearance.TextHAlign = HAlign.Right;

            s = grid.DisplayLayout.Bands[0].Summaries.Add(
                    SummaryType.Sum, grid.DisplayLayout.Bands[0].Columns["TotalCoupon"]);
            s.DisplayFormat = "{0:##,##0.00#}";
            s.Appearance.TextHAlign = HAlign.Right;

            e.Layout.Override.RowSelectors = DefaultableBoolean.True;
            e.Layout.Override.RowSelectorHeaderStyle = RowSelectorHeaderStyle.ColumnChooserButton;
            e.Layout.Bands[0].Columns["ID"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
            e.Layout.Bands[0].Columns["DayCount"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
            e.Layout.Bands[0].Columns["RefIssBonds"].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
        }

        void ugCoupon_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            UltraGrid grid = (UltraGrid)sender;
            grid.DisplayLayout.Override.SummaryDisplayArea = SummaryDisplayAreas.TopFixed;
            SummarySettings s = grid.DisplayLayout.Bands[0].Summaries.Add(
                    SummaryType.Sum, grid.DisplayLayout.Bands[0].Columns["DayCount"]);
            s.DisplayFormat = "{0:##,##0}";
            s.Appearance.TextHAlign = HAlign.Right;
        }

        void ugNominal_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            UltraGrid grid = (UltraGrid)sender;
            grid.DisplayLayout.Override.SummaryDisplayArea = SummaryDisplayAreas.TopFixed;
            SummarySettings s = grid.DisplayLayout.Bands[0].Summaries.Add(
                    SummaryType.Sum, grid.DisplayLayout.Bands[0].Columns["NomSum"]);
            s.DisplayFormat = "{0:##,##0.00#}";
            s.Appearance.TextHAlign = HAlign.Right;
        }

        private void InitializeGrid(IEntity entity, UltraGrid grid)
        {
            foreach (IDataAttribute item in entity.Attributes.Values)
            {
                // получаем прокси атрибута
                IDataAttribute attr = item;
                // **** Запоминаем необходимые параметры чтобы лишний раз не дергать прокси в цикле ****
                string attrName = attr.Name;
                string attrCaption = attr.Description;
                int attrSize = attr.Size;
                int attrMantissaSize = attr.Scale;
                DataAttributeTypes attrType = attr.Type;
                string attrMask = attr.Mask;
                bool nullableColumn = attr.IsNullable;
                // свойства для группировки колонок на системные и т.п.
                GridColumnState state = new GridColumnState();
                if (nullableColumn)
                    state.IsNullable = true;

                if (attrSize > 20 && attrSize < 80)
                    state.ColumnWidth = 20;
                else
                    state.ColumnWidth = attrSize;
                state.IsHiden = !attr.Visible;
                state.IsReadOnly = attr.IsReadOnly;
                state.ColumnName = attrName;
                state.DefaultValue = attr.DefaultValue;
                state.isTextColumn = attr.StringIdentifier;
                state.IsBLOB = attr.Type == DataAttributeTypes.dtBLOB;
                state.ColumnPosition = attr.Position;

                switch (attrType)
                {
                    case DataAttributeTypes.dtBoolean:
                        state.ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
                        break;
                    case DataAttributeTypes.dtChar:
                        break;
                    case DataAttributeTypes.dtDate:
                        state.ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle.Date;
                        break;
                    case DataAttributeTypes.dtDateTime:
                        state.ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle.DateTime;
                        break;
                    case DataAttributeTypes.dtDouble:
                        if (attrMantissaSize >= 20)
                            attrMantissaSize = 19;
                        string newMask = GetMask(attrSize - attrMantissaSize);
                        state.Mask = String.Concat('-', newMask, '.', String.Empty.PadRight(attrMantissaSize, 'n'));
                        break;
                    // для целочисленных типов - устанавливаем маску по умолчанию
                    // равной размеру атрибута. Может перекрываться маской
                    // определенной в XML-е схемы
                    case DataAttributeTypes.dtInteger:
                        if (state.IsReference)
                        {
                            string tmpName = attr.LookupObjectName;
                            // для лукапа типа календарь нужно будет создавать дополнительное поле типа string
                            if (tmpName.Contains("fx.Date.YearDay"))
                            {
                                state.Mask = "nnnn.nn.nn";
                                state.CalendarColumn = true;
                                state.IsSystem = false;
                                state.ColumnType = UltraGridEx.ColumnType.Standart;
                            }
                            else
                            {
                                state.Mask = string.Concat("-", string.Empty.PadLeft(attrSize, 'n'));
                            }
                        }
                        else
                        {
                            state.Mask = string.Compare("ID", attrName, true) == 0 ?
                                string.Concat("-", string.Empty.PadLeft(attrSize, 'n')) :
                                String.Concat("-", GetMask(attrSize));
                        }
                        break;
                    case DataAttributeTypes.dtString:
                        state.Mask = String.Empty.PadRight(attrSize, 'a');
                        break;
                }

                if ((attrMask != null) && (attrMask != String.Empty))
                    state.Mask = attrMask;
                state.ColumnCaption = attrCaption;
                UltraGridColumn column = grid.DisplayLayout.Bands[0].Columns[attrName];
                column.Header.Caption = attrCaption;
                column.Header.VisiblePosition = state.ColumnPosition;
                SetColumnWdth(column, state);
                if (!string.IsNullOrEmpty(state.Mask))
                {
                    column.CellMultiLine = DefaultableBoolean.False;
                    column.MaskDataMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.Raw;
                    column.MaskClipMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.Raw;
                    column.MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiterals;
                    column.CellAppearance.TextHAlign = HAlign.Right;
                    column.PadChar = '_';
                    //string newMask = GetMask(attrSize - attrMantissaSize);
                    column.MaskInput = state.Mask;//String.Concat('-', newMask, '.', String.Empty.PadRight(attrMantissaSize, 'n'));
                }
            }
        }

        private static string GetMask(int masklength)
        {
            int charges = masklength / 3;
            int remainder = masklength % 3;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i <= charges - 1; i++)
            {
                sb.Append(",nnn");
            }
            if (remainder == 0)
                sb.Remove(0, 1);
            return string.Concat(String.Empty.PadRight(remainder, 'n'), sb.ToString());
        }

        void SetColumnWdth(UltraGridColumn column, GridColumnState curState)
        {
            switch (column.Header.Caption)
            {
                case "ID":
                    column.Width = 40;
                    break;
                case "":
                    column.Width = 20;
                    break;
                default:
                    if (curState.ColumnWidth > 260)
                        column.Width = 260;
                    else
                        if (curState.ColumnWidth >= 190)
                            column.Width = 190;
                        else
                            if (curState.ColumnWidth <= 20)
                                column.Width = 100;
                            else
                                column.Width = curState.ColumnWidth;
                    break;
            }
        }

        #endregion


        void ToolbarsManager_ToolValueChanged(object sender, ToolEventArgs e)
        {
            if (e.Tool.Key == "Calculations")
            {
                ComboBoxTool comboBoxTool = e.Tool as ComboBoxTool;
                string value = comboBoxTool.Value == null ? string.Empty : comboBoxTool.Value.ToString();
                CalculationUniqueParams calculationUniqueParams = string.IsNullOrEmpty(value) ? null : new CalculationUniqueParams(value.ToString());
                CleanWarnings();
                if (calculationUniqueParams != null)
                {
                    LoadData(calculationUniqueParams);
                    CurrentCalculationCaption = string.Format("{0} ({1})", calculationUniqueParams.Name,
                                                              calculationUniqueParams.CalculationDate.ToShortDateString());
                }
                else
                {
                    LoadEmptyData();
                    CurrentCalculationCaption = string.Empty;
                }
            }
        }

        void ToolbarsManager_ToolClick(object sender, ToolClickEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "SaveData":
                    if (ViewObject.IsCalculationChanged || ViewObject.IsBondsChanged)
                    {
                        if (SaveData())
                            BurnSaveData(false);
                    }
                    break;
                case "RefeshData":
                    LoadCalculations(true);
                    break;
                case "DeleteData":
                    var cb = (ComboBoxTool)e.Tool.ToolbarsManager.Tools["Calculations"];
                    if (cb.SelectedIndex == -1)
                        return;
                    if (MessageBox.Show(string.Format("Удалить расчет '{0}'?", CurrentCalculationCaption),
                        "Удаление расчета",MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        DeleteData();
                    }
                    break;
                case "NewCalculation":
                    LoadEmptyData();
                    break;
                case "LoanLayout":
                    if (dtParams.Rows.Count > 0)
                    {
                        var reportCommand = new ReportMOCapitalBasementDetailCommand
                        {
                            window = Workplace.WindowHandle,
                            operationObj = Workplace.OperationObj,
                            scheme = Workplace.ActiveScheme
                        };

                        var calcId = Convert.ToString(dtParams.Rows[0]["ID"]);
                        reportCommand.SetReportParamValue(ReportConsts.ParamMasterFilter, calcId);
                        reportCommand.Run();
                    }
                    break;
                case "LoanLayoutFull":
                    if (dtParams.Rows.Count > 0)
                    {
                        var reportCommand = new ReportMOCapitalBasementCommand
                        {
                            window = Workplace.WindowHandle,
                            operationObj = Workplace.OperationObj,
                            scheme = Workplace.ActiveScheme
                        };

                        var calcId = Convert.ToString(dtParams.Rows[0]["ID"]);
                        reportCommand.SetReportParamValue(ReportConsts.ParamMasterFilter, calcId);
                        reportCommand.Run();
                    }
                    break;
                case "LoanCompare":
                        var compareCommand = new ReportMOCompareCapitalBasementCommand()
                        {
                            window = Workplace.WindowHandle,
                            operationObj = Workplace.OperationObj,
                            scheme = Workplace.ActiveScheme
                        };

                        compareCommand.Run();
                    break;
                case "Calculate":
                    Calculate();
                    break;
                case "CreateReport":
                    break;
            }
        }

        private void Calculate()
        {
            ViewObject.ugCoupon.PerformAction(UltraGridAction.ExitEditMode);
            ViewObject.ugNominal.PerformAction(UltraGridAction.ExitEditMode);

            FinancialCalculationParams calculationParams = ViewObject.GetCalculationParams();

            if (!CalculationParamsValidation())
            {
                MessageBox.Show("Не все параметры заполнены для расчета или заполнены не корректно", "Расчеты", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (calculationParams.IsEndDateRepay || calculationParams.PaymentsNominalCost.Length == 0)
            {
                FillNominalPayment();
            }

            if (!calculationParams.IsConstRate && calculationParams.Coupons.Length != 0
                && calculationParams.Coupons.Length != calculationParams.CouponsCount)
            {
                MessageBox.Show("Количество купонных периодов не равно количеству записей в детали 'Купоны'. Проверьте правильность заполнения", "Расчеты", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            // автоматическое заполнение деталей погашение номинальной стоимости и купонов
            ViewObject.CalculateUnpayValue(calculationParams, calculationParams.CouponsCount, calculationParams.CouponR);
            calculationParams = ViewObject.GetCalculationParams();

            int daysCount = 0;

            if (calculationParams.PaymentsNominalCost.Length == 0)
            {
                MessageBox.Show("Не заполнены данные по погашению номинальной стоимости", "Расчеты", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            decimal dayCountInCouponPeriod = Math.Round(Convert.ToDecimal(calculationParams.CurrencyBorrow / calculationParams.CouponsCount), 0,
                                                    MidpointRounding.AwayFromZero);
            decimal nominalSum = 0;
            foreach (var nomimalCost in calculationParams.PaymentsNominalCost)
            {
                if (nomimalCost.DayCount > calculationParams.CurrencyBorrow)
                {
                    string message =
                        string.Format("Количество дней до выплаты номинальной стоимости ({0}) в детали «Погашение номинальной стоимости» не должно быть больше срока обращения ({1})",
                        nomimalCost.DayCount, calculationParams.CurrencyBorrow);
                    MessageBox.Show(message, "Расчеты",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                if (nomimalCost.DayCount % dayCountInCouponPeriod != 0)
                {
                    string message =
                        string.Format(
                            "Количество дней до погашения номинальной стоимости ({0}) должно быть кратно количеству дней в купонном периоде ({1}). Проверьте правильность заполнения детали 'Погашение номинальной стоимости' и параметра 'Срок обращения'",
                            nomimalCost.DayCount, dayCountInCouponPeriod);
                    MessageBox.Show(message, "Расчеты",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                nominalSum += nomimalCost.NominalSum;
            }
            if (nominalSum != calculationParams.Nominal)
            {
                MessageBox.Show(string.Format("Сумма выплат в детали 'Погашение номинальной стоимости' ({0}) должна быть равна номинальной стоимости ({1})",
                    nominalSum, calculationParams.Nominal),
                    "Расчеты", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            daysCount = 0;
            foreach (Coupon coupon in calculationParams.Coupons)
            {
                if (coupon.DayCount == 0)
                {
                    MessageBox.Show("Не заполнено количество дней в детали 'Купоны'", "Расчеты", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (calculationParams.CalculatedValue != CalculatedValue.CouponR && coupon.CouponRate == 0)
                {
                    MessageBox.Show("Необходимо заполнить ставки купонного дохода для каждого купонного периода в детализации 'Купоны'", "Расчеты", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (coupon.DayCount > 366)
                {
                    MessageBox.Show(string.Format("Количество дней в купонном периоде ({0}) не должно быть больше 366 дней. Проверьте правильность заполнения Срока обращения или количества купонных периодов", coupon.DayCount), "Расчеты", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                daysCount += coupon.DayCount;
            }

            if (daysCount != calculationParams.CurrencyBorrow)
            {
                MessageBox.Show("Общее количество дней в детали 'Купоны' должно быть равно сроку обращения", "Расчеты", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ViewObject.CalculateCouponsNominal();

            FinancialCalculation calculation = new FinancialCalculation();
            decimal value = calculation.Calculate(calculationParams);
            ViewObject.SetCalculatedValue(value, calculationParams);
            FillBonds();
            BurnSaveData(true);
        }

        private void FillNominalPayment()
        {
            ViewObject.cbEndPeriodPayment.Checked = false;
            ViewObject.cbEndPeriodPayment.Checked = true;
        }

        private void LoadData(CalculationUniqueParams uniqueParams)
        {
            using (IDataUpdater du = ParamsEntity.GetDataUpdater("Name = ? and CalcDate = ?", null,
                new DbParameterDescriptor("p0", uniqueParams.Name), new DbParameterDescriptor("p1", uniqueParams.CalculationDate)))
            {
                du.Fill(ref dtParams);
            }
            if (dtParams.Rows.Count == 0)
                return;
            int refParams = Convert.ToInt32(dtParams.Rows[0]["ID"]);
            using (IDataUpdater du = NominalEntity.GetDataUpdater("RefICalcParam = ?", null,
                new DbParameterDescriptor("p0", refParams)))
            {
                du.Fill(ref dtNominal);
            }
            using (IDataUpdater du = CouponEntity.GetDataUpdater("RefICalcParam = ?", null,
                new DbParameterDescriptor("p0", refParams)))
            {
                du.Fill(ref dtCoupons);
            }
            using (IDataUpdater du = CapitalBondEntity.GetDataUpdater("IDCalcParam = ?", null,
                new DbParameterDescriptor("p0", refParams)))
            {
                du.Fill(ref dtCapitalBond);
            }
            if (dtCapitalBond.Rows.Count > 0)
            {
                object bondId = dtCapitalBond.Rows[0]["ID"];
                using (IDataUpdater du = NominalBondEntity.GetDataUpdater("RefIssBonds = ?", null,
                    new DbParameterDescriptor("p0", bondId)))
                {
                    du.Fill(ref dtNominalBond);
                }
                using (IDataUpdater du = PaymentBondEntity.GetDataUpdater("RefIssBonds = ?", null,
                    new DbParameterDescriptor("p0", bondId)))
                {
                    du.Fill(ref dtPaymentBond);
                }
            }

            ViewObject.SetData(dtParams, dtCoupons, dtNominal);
            ViewObject.SetBondData(dtCapitalBond, dtNominalBond, dtPaymentBond);
        }

        private void LoadEmptyData()
        {
            CleanWarnings();
            dtParams = new DataTable();
            dtNominal = new DataTable();
            dtCoupons = new DataTable();
            dtCapitalBond = new DataTable();
            dtPaymentBond = new DataTable();
            dtNominalBond = new DataTable();
            IEntity paramsEntity =
                Workplace.ActiveScheme.RootPackage.FindEntityByName(CapitalOperationsKeys.f_S_ICalcParam);
            using (IDataUpdater du = paramsEntity.GetDataUpdater("1 = 2", null))
            {
                du.Fill(ref dtParams);
            }

            IEntity nominalEntity =
                Workplace.ActiveScheme.RootPackage.FindEntityByName(CapitalOperationsKeys.t_S_IRepaymentNom);
            using (IDataUpdater du = nominalEntity.GetDataUpdater("1 = 2", null))
            {
                du.Fill(ref dtNominal);
            }
            IEntity couponEntity =
                Workplace.ActiveScheme.RootPackage.FindEntityByName(CapitalOperationsKeys.t_S_ICoupons);
            using (IDataUpdater du = couponEntity.GetDataUpdater("1 = 2", null))
            {
                du.Fill(ref dtCoupons);
            }

            IEntity capitalBondEntity =
                Workplace.ActiveScheme.RootPackage.FindEntityByName(CapitalOperationsKeys.f_S_IssueBond);
            using (IDataUpdater du = capitalBondEntity.GetDataUpdater("1 = 2", null))
            {
                du.Fill(ref dtCapitalBond);
            }
            IEntity nominalBondEntity =
                Workplace.ActiveScheme.RootPackage.FindEntityByName(CapitalOperationsKeys.t_S_INom);
            using (IDataUpdater du = nominalBondEntity.GetDataUpdater("1 = 2", null))
            {
                du.Fill(ref dtNominalBond);
            }
            IEntity paymentBondEntity =
                Workplace.ActiveScheme.RootPackage.FindEntityByName(CapitalOperationsKeys.t_S_IPaymnts);
            using (IDataUpdater du = paymentBondEntity.GetDataUpdater("1 = 2", null))
            {
                du.Fill(ref dtPaymentBond);
            }
            ViewObject.SetEmptyData();
            ComboBoxTool comboBoxTool = ViewObject.ToolbarsManager.Tools["Calculations"] as ComboBoxTool;
            comboBoxTool.SelectedIndex = -1;
        }

        private bool SaveData()
        {
            if (InvalidDataEditors.Count > 0)
            {
                MessageBox.Show("Сохранение расчета невозможно. Параметры расчета введены неверно", "Сохранение данных",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            string calculationName = string.Empty;
            if (!GetCalculationSaveName(ref calculationName))
                return false;
            FinancialCalculationParams calculationParams = ViewObject.GetCalculationParams();
            calculationParams.Name = calculationName;
            ViewObject.ugCoupon.PerformAction(UltraGridAction.ExitEditMode);
            ViewObject.ugNominal.PerformAction(UltraGridAction.ExitEditMode);
            if (ViewObject.IsCalculationChanged)
                ViewObject.GetData(calculationParams, ref dtParams, ref dtCoupons, ref dtNominal, ref dtCapitalBond, ref dtNominalBond, ref dtPaymentBond);

            DeleteSavedData(calculationName, calculationParams.CalculationDate);

            using (IDataUpdater du = ParamsEntity.GetDataUpdater())
            {
                du.Update(ref dtParams);
            }
            int id = -1;
            using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
            {
                id = Convert.ToInt32(db.ExecQuery("select id from f_S_ICalcParam where Name = ? and CalcDate = ?", QueryResultTypes.Scalar,
                             new DbParameterDescriptor("p0", dtParams.Rows[0]["Name"]),
                             new DbParameterDescriptor("p1", dtParams.Rows[0]["CalcDate"])));
            }
            dtParams.Rows[0]["ID"] = id;

            foreach (DataRow row in dtCoupons.Rows)
            {
                row["RefICalcParam"] = id;
                row["ID"] = CouponEntity.GetGeneratorNextValue;
            }
            using (IDataUpdater du = CouponEntity.GetDataUpdater())
            {
                du.Update(ref dtCoupons);
            }

            foreach (DataRow row in dtNominal.Rows)
            {
                row["RefICalcParam"] = id;
                row["ID"] = NominalEntity.GetGeneratorNextValue;
            }
            using (IDataUpdater du = NominalEntity.GetDataUpdater())
            {
                du.Update(ref dtNominal);
            }

            if (dtCapitalBond.Rows.Count > 0)
            {
                dtCapitalBond.Rows[0]["IDCalcParam"] = id;
                using (IDataUpdater du = CapitalBondEntity.GetDataUpdater())
                {
                    du.Update(ref dtCapitalBond);
                }
                using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
                {
                    id = Convert.ToInt32(db.ExecQuery("select id from f_S_IssueBond where IDCalcParam = ?", QueryResultTypes.Scalar,
                        new DbParameterDescriptor("p0", id)));
                }
                foreach (DataRow row in dtNominalBond.Rows)
                {
                    row["RefIssBonds"] = id;
                    row["ID"] = NominalBondEntity.GetGeneratorNextValue;
                }
                using (IDataUpdater du = NominalBondEntity.GetDataUpdater())
                {
                    du.Update(ref dtNominalBond);
                }

                foreach (DataRow row in dtPaymentBond.Rows)
                {
                    row["RefIssBonds"] = id;
                    row["ID"] = PaymentBondEntity.GetGeneratorNextValue;
                }
                using (IDataUpdater du = PaymentBondEntity.GetDataUpdater())
                {
                    du.Update(ref dtPaymentBond);
                }
            }
            ViewObject.IsCalculationChanged = false;
            ViewObject.IsBondsChanged = false;
            LoadCalculations(false);
            FindLastSavedCalculation(calculationParams);
            return true;
        }

        private bool GetCalculationSaveName(ref string calculationName)
        {
            List<string> comments = new List<string>();
            ComboBoxTool cb = (ComboBoxTool)ViewObject.ToolbarsManager.Tools["Calculations"];
            foreach (var obj in cb.ValueList.ValueListItems)
            {
                string calcName = string.Empty;
                DateTime calcDate = DateTime.MinValue;
                GetCalculationUniqueParams(obj.DataValue.ToString(), ref calcDate, ref calcName);
                comments.Add(calcName);
            }
            return SelectCommentForm.ShowSaveCalcResultsForm(comments, ref calculationName);
        }

        private void FindLastSavedCalculation(FinancialCalculationParams calculationParams)
        {
            ComboBoxTool comboBoxTool = ViewObject.ToolbarsManager.Tools["Calculations"] as ComboBoxTool;
            CalculationUniqueParams uniqueParams = new CalculationUniqueParams(calculationParams.Name, calculationParams.CalculationDate);
            comboBoxTool.SelectedItem = comboBoxTool.ValueList.FindByDataValue(uniqueParams.ToString());
        }

        private void DeleteSavedData(string calculationName, DateTime calculationDate)
        {
            using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
            {
                object masterId = db.ExecQuery("select id from f_S_ICalcParam where Name = ? and CalcDate = ?", QueryResultTypes.Scalar,
                            new DbParameterDescriptor("p0", calculationName),
                            new DbParameterDescriptor("p1", calculationDate));
                if (masterId != null && masterId != DBNull.Value)
                {

                    // при изменении параметров расчета, удаляем старые из базы
                    if (ViewObject.IsCalculationChanged)
                    {
                        db.ExecQuery("Delete from t_S_IRepaymentNom where RefICalcParam = ?",
                                        QueryResultTypes.NonQuery,
                                        new DbParameterDescriptor("p0", masterId));
                        db.ExecQuery("Delete from t_S_ICoupons where RefICalcParam = ?", QueryResultTypes.NonQuery,
                                        new DbParameterDescriptor("p0", masterId));
                        db.ExecQuery("delete from f_S_ICalcParam where id = ?", QueryResultTypes.NonQuery,
                                        new DbParameterDescriptor("p0", masterId));
                    }

                    // при изменении размещения удаляем старое размещение
                    object bonndsId = db.ExecQuery("select id from f_S_IssueBond where IDCalcParam = ?",
                                                    QueryResultTypes.Scalar,
                                                    new DbParameterDescriptor("p0", masterId));
                    if (bonndsId != DBNull.Value && bonndsId != null)
                    {
                        db.ExecQuery("Delete from t_S_INom where RefIssBonds = ?", QueryResultTypes.NonQuery,
                                        new DbParameterDescriptor("p0", bonndsId));
                        db.ExecQuery("Delete from t_S_IPaymnts where RefIssBonds = ?", QueryResultTypes.NonQuery,
                                        new DbParameterDescriptor("p0", bonndsId));
                        db.ExecQuery("Delete from f_S_IssueBond where IDCalcParam = ?",
                                        QueryResultTypes.NonQuery,
                                        new DbParameterDescriptor("p0", masterId));
                    }
                }
            }
        }

        private void DeleteData()
        {
            // удаляем данные из всех возможных таблиц
            if (dtParams.Rows.Count == 0)
                return;
            object masterId = dtParams.Rows[0]["ID"];
            if (masterId != DBNull.Value && masterId != null)
            {
                using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
                {
                    db.ExecQuery("Delete from t_S_IRepaymentNom where RefICalcParam = ?", QueryResultTypes.NonQuery,
                                 new DbParameterDescriptor("p0", masterId));
                    db.ExecQuery("Delete from t_S_ICoupons where RefICalcParam = ?", QueryResultTypes.NonQuery,
                                 new DbParameterDescriptor("p0", masterId));
                    object bonndsId = db.ExecQuery("select id from f_S_IssueBond where IDCalcParam = ?",
                        QueryResultTypes.Scalar, new DbParameterDescriptor("p0", masterId));
                    if (bonndsId != DBNull.Value && bonndsId != null)
                    {
                        db.ExecQuery("Delete from t_S_INom where RefIssBonds = ?", QueryResultTypes.NonQuery,
                                     new DbParameterDescriptor("p0", bonndsId));
                        db.ExecQuery("Delete from t_S_IPaymnts where RefIssBonds = ?", QueryResultTypes.NonQuery,
                                     new DbParameterDescriptor("p0", bonndsId));
                        db.ExecQuery("Delete from f_S_IssueBond where IDCalcParam = ?", QueryResultTypes.NonQuery,
                                     new DbParameterDescriptor("p0", masterId));
                    }
                    db.ExecQuery("Delete from f_S_ICalcParam where ID = ?", QueryResultTypes.NonQuery,
                                     new DbParameterDescriptor("p0", masterId));
                }
                LoadCalculations(true);
            }
            else
            {
                dtParams.Clear();
                dtNominal.Clear();
                dtCoupons.Clear();
                LoadCalculations(true);
            }
        }

        private void FillBonds()
        {
            dtPaymentBond.Clear();
            dtNominalBond.Clear();
            dtCapitalBond.Clear();

            FinancialCalculationParams calculationParams = ViewObject.GetCalculationParams();
            DataRow row = dtCapitalBond.NewRow();
            row["SourceId"] = FinSourcePlanningNavigation.Instance.CurrentSourceID;
            row["TaskID"] = -1;
            row["Basis"] = calculationParams.Basis;
            row["Nominal"] = calculationParams.Nominal;
            row["YTM"] = calculationParams.YTM;
            row["CurrPriceRub"] = calculationParams.CurrentPriceRur;
            row["CurrPrice"] = calculationParams.CurrentPricePercent;
            decimal diff = calculationParams.Nominal - calculationParams.CurrentPriceRur;
            row["DiffBtwNP"] = diff;
            row["TotalCount"] = calculationParams.TotalCount;
            row["TotalSum"] = calculationParams.TotalSum;
            row["TotalDiffBtwNP"] = diff * calculationParams.TotalCount;
            dtCapitalBond.Rows.Add(row);

            foreach (NominalCost nominal in calculationParams.PaymentsNominalCost)
            {
                row = dtNominalBond.NewRow();
                row["Num"] = nominal.Num;
                row["DayCount"] = nominal.DayCount;
                row["NomSum"] = nominal.NominalSum;
                dtNominalBond.Rows.Add(row);
            }
            DateTime startDate = calculationParams.CouponStartDate;
            int t = 0;
            foreach (Coupon coupon in calculationParams.Coupons)
            {
                row = dtPaymentBond.NewRow();
                row["Basis"] = calculationParams.Basis;
                row["Num"] = coupon.Num;
                if (startDate.Year > 1980)
                {
                    row["StartDate"] = startDate;
                    DateTime endDate = startDate.AddDays(coupon.DayCount);
                    row["EndDate"] = endDate;
                    startDate = endDate;
                }
                row["DayCpnCount"] = coupon.DayCount;
                row["Nomi"] = coupon.Nominal;
                row["Rate"] = coupon.CouponRate;
                row["TotalCount"] = calculationParams.TotalCount;
                decimal couponSum =
                    Math.Round(coupon.CouponRate*coupon.DayCount*coupon.Nominal/(calculationParams.Basis*100), 2,
                                MidpointRounding.AwayFromZero);
                row["Coupon"] = couponSum;
                row["TotalCoupon"] = couponSum*calculationParams.TotalCount;
                t += coupon.DayCount;
                foreach (NominalCost nominal in calculationParams.PaymentsNominalCost)
                {
                    if (nominal.DayCount == t)
                    {
                        row["DayCount"] = nominal.DayCount;
                        row["NomSum"] = nominal.NominalSum;
                        if (startDate.Year > 1980)
                        {
                            row["PayDate"] = calculationParams.CouponStartDate.AddDays(nominal.DayCount);
                        }
                        break;
                    }
                }
                dtPaymentBond.Rows.Add(row);
            }
            ViewObject.SetBondData(dtCapitalBond, dtNominalBond, dtPaymentBond);
            BurnSaveData(true);
        }

        private void LoadCalculations(bool selectFirstItem)
        {
            BurnSaveData(false);
            calculations.Clear();
            ComboBoxTool comboBoxTool = ViewObject.ToolbarsManager.Tools["Calculations"] as ComboBoxTool;
            comboBoxTool.ValueList.ValueListItems.Clear();
            using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
            {
                DataTable dt = (DataTable)db.ExecQuery("select distinct CalcDate, Name from f_S_ICalcParam order by CalcDate desc, Name", QueryResultTypes.DataTable);
                foreach (DataRow row in dt.Rows)
                {
                    string name = row[1].ToString();
                    string date = Convert.ToDateTime(row[0]).ToShortDateString();
                    string key = name + " (" + date + ")";
                    CalculationUniqueParams uniqueParams = new CalculationUniqueParams(name, Convert.ToDateTime(row[0]));
                    comboBoxTool.ValueList.ValueListItems.Add(uniqueParams.ToString(), key);
                }
            }
            if (selectFirstItem)
            {
                if (comboBoxTool.ValueList.ValueListItems.Count > 0)
                    comboBoxTool.SelectedIndex = 0;
                else
                {
                    LoadEmptyData();
                }
            }

            ViewObject.ugNominal.DisplayLayout.Bands[0].Columns["Num"].SortIndicator = SortIndicator.Ascending;
            ViewObject.ugCoupon.DisplayLayout.Bands[0].Columns["Num"].SortIndicator = SortIndicator.Ascending;
        }

        private void BurnSaveData(bool isBurn)
        {
            InfragisticsHelper.BurnTool(ViewObject.ToolbarsManager.Tools["SaveData"], isBurn);
        }

        private void BurnCalculationButton(bool burn)
        {
            InfragisticsHelper.BurnTool(ViewObject.ToolbarsManager.Tools["Calculate"], burn);
            ViewObject.ToolbarsManager.Tools["Calculate"].SharedProps.Enabled = burn;
        }

        private bool CalculationParamsValidation()
        {
            CleanWarnings();
            FinancialCalculationParams calculationParams = ViewObject.GetCalculationParams();

            if (calculationParams.Nominal == 0)
            {
                WarningList.Add(ViewObject.ne7.Name, new DataWarningNotifier(ViewObject.ne7, "Не заполнена номинальная стоимость"));
            }

            if (calculationParams.CurrencyBorrow == 0)
            {
                WarningList.Add(ViewObject.ne8.Name, new DataWarningNotifier(ViewObject.ne8, "Не заполнен срок обращения"));
            }

            if (calculationParams.CouponsCount == 0)
            {
                WarningList.Add(ViewObject.CouponsCount.Name, new DataWarningNotifier(ViewObject.CouponsCount, "Не заполнено количество купонных периодов"));
            }

            if (calculationParams.CalculatedValue != CalculatedValue.CouponR &&
                calculationParams.IsConstRate &&
                calculationParams.CouponR == 0)
            {
                WarningList.Add(ViewObject.ne1.Name, new DataWarningNotifier(ViewObject.ne1, "Не заполнена ставка % годовых"));
            }

            if ((calculationParams.CalculatedValue == CalculatedValue.CouponR ||
                calculationParams.CalculatedValue == CalculatedValue.YTM))
            {
                if (calculationParams.CurrentPricePercent == 0)
                    WarningList.Add(ViewObject.ne3.Name, new DataWarningNotifier(ViewObject.ne3, "Не заполнена текущая рыночная цена, %"));
                if (calculationParams.CurrentPriceRur == 0)
                    WarningList.Add(ViewObject.ne4.Name, new DataWarningNotifier(ViewObject.ne4, "Не заполнена текущая рыночная цена в рублях"));
            }

            if ((calculationParams.CalculatedValue == CalculatedValue.CouponR ||
                calculationParams.CalculatedValue == CalculatedValue.CurrPriceRur) &&
                calculationParams.YTM == 0)
            {
                WarningList.Add(ViewObject.ne2.Name, new DataWarningNotifier(ViewObject.ne2, "Не заполнена эффективная доходность к погашению"));
            }

            if (ViewObject.rb5.Checked)
            {
                if (calculationParams.TotalSum == 0)
                    WarningList.Add(ViewObject.ne5.Name, new DataWarningNotifier(ViewObject.ne5, "Не заполнен объем средств от размещаемого займа"));
            }
            else 
            {
                if (calculationParams.TotalCount == 0)
                    WarningList.Add(ViewObject.ne6.Name, new DataWarningNotifier(ViewObject.ne6, "Не заполнен количество облигаций размещаемого займа"));
            }

            if (calculationParams.CurrencyBorrow > 0 && calculationParams.CouponsCount > 0)
            {
                if (calculationParams.CurrencyBorrow % calculationParams.CouponsCount != 0)
                {
                    WarningList.Add(ViewObject.ne8.Name, new DataWarningNotifier(ViewObject.ne8, "Срок обращения на количество купонов должен делиться без остатка"));
                    WarningList.Add(ViewObject.CouponsCount.Name, new DataWarningNotifier(ViewObject.CouponsCount, "Срок обращения на количество купонов должен делиться без остатка"));
                }
            }

            CheckMaxEditorValue(ViewObject.ne5);
            CheckMaxEditorValue(ViewObject.ne6, 99999999);
            CheckMaxEditorValue(ViewObject.ne2, Convert.ToDecimal(99.9999));

            return WarningList.Count == 0;
        }
    }
}
