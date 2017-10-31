using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using System.Data;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.CapitalPlanningOperations
{
    public class CapitalOperationsView : BaseView
    {
        #region визуальные компоненты

        private Infragistics.Win.UltraWinTabControl.UltraTabControl ultraTabControl1;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage1;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl1;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl2;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private Infragistics.Win.UltraWinTabControl.UltraTabControl ultraTabControl2;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage2;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl4;
        internal UltraGrid ug2;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl5;
        internal UltraGrid ug3;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private Infragistics.Win.Misc.UltraPanel ultraPanel4;
        internal UltraGrid ugNominal;
        private Infragistics.Win.Misc.UltraPanel ultraPanel1;
        private Infragistics.Win.Misc.UltraButton btnDeleteNominal;
        private Infragistics.Win.Misc.UltraButton btnAddNominal;
        private System.Windows.Forms.Label label7;
        private Infragistics.Win.Misc.UltraPanel ultraPanel3;
        internal UltraGrid ugCoupon;
        private Infragistics.Win.Misc.UltraPanel ultraPanel2;
        private System.Windows.Forms.Label label8;
        private Infragistics.Win.Misc.UltraPanel ultraPanel5;
        private System.Windows.Forms.GroupBox groupBox2;
        internal UltraNumericEditor ne6;
        internal UltraNumericEditor ne5;
        private System.Windows.Forms.RadioButton rb6;
        internal RadioButton rb5;
        internal UltraNumericEditor ne7;
        internal UltraNumericEditor ne8;
        private System.Windows.Forms.GroupBox groupBox1;
        internal UltraNumericEditor ne4;
        internal UltraNumericEditor ne3;
        internal UltraNumericEditor ne2;
        internal UltraNumericEditor ne1;
        internal RadioButton rb3;
        internal RadioButton rb2;
        internal RadioButton rb1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBox2;

        #endregion

        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl6;
        internal UltraGrid ug1;
        private Infragistics.Win.Misc.UltraPanel ultraPanel7;
        internal UltraGrid ugCapitalBond;
        internal CheckBox cbEndPeriodPayment;
        internal Infragistics.Win.UltraWinToolbars.UltraToolbarsManager tbManager;
        private System.ComponentModel.IContainer components;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseView_Toolbars_Dock_Area_Left;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseView_Toolbars_Dock_Area_Right;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseView_Toolbars_Dock_Area_Top;
        internal Infragistics.Win.Misc.UltraValidator ultraValidator;
        internal UltraDateTimeEditor Date;
        internal CheckBox cbIsConstRate;
        private Label label2;
        internal UltraNumericEditor CouponsCount;
        internal RadioButton rbRubPrice;
        internal Infragistics.Win.Misc.UltraButton btnClearNominalPays;
        internal Infragistics.Win.Misc.UltraButton CouponsClear;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseView_Toolbars_Dock_Area_Bottom;

        private DataTable NominalTable
        {
            get; set;
        }
        private DataTable CouponTable
        {
            get; set;
        }
        private DataTable CapitalBondTable
        {
            get; set;
        }
        private DataTable NominalBondTable
        {
            get; set;
        }
        private DataTable PaymentBondTable
        {
            get; set;
        }

        /// <summary>
        /// Показывает, были ли пересчитаны данные по параметрам
        /// </summary>
        internal bool IsCalculationChanged
        {
            get;
            set;
        }

        internal bool IsBondsChanged
        {
            get; set;
        }

        public CapitalOperationsView()
        {
            InitializeComponent();

            NominalTable = new DataTable();
            DataColumn column = NominalTable.Columns.Add("Num", typeof(int));
            column.Caption = "Номер";
            column = NominalTable.Columns.Add("DayCount", typeof(int));
            column.Caption = "Количество дней до выплаты номинальной стоимости";
            column = NominalTable.Columns.Add("Nominal", typeof(int));
            column.Caption = "Выплата номинальной стоимости, руб.";
            ugNominal.DataSource = NominalTable;

            CouponTable = new DataTable();
            column = CouponTable.Columns.Add("Num", typeof(int));
            column.Caption = "Номер";
            column = CouponTable.Columns.Add("DayCount", typeof(int));
            column.Caption = "Количество дней в купонном периоде";
            column = CouponTable.Columns.Add("CouponRate", typeof(decimal));
            column.Caption = "Ставка купонного дохода, % годовых";
            column = CouponTable.Columns.Add("Nominal", typeof(decimal));
            column.Caption = "Непогашенная часть номинальной стоимости, руб.";
            ugCoupon.DataSource = CouponTable;

            CapitalBondTable = new DataTable();
            NominalBondTable = new DataTable();
            PaymentBondTable = new DataTable();

            SetEmptyData();
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.Appearance appearance25 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance26 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance28 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance27 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance33 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance29 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance36 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance32 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance30 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance31 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance34 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance35 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance65 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance66 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance67 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance68 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance69 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance70 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance71 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance72 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance73 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance74 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance75 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance76 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance49 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance53 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance54 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance55 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance56 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance57 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance58 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance59 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance61 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance62 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance63 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance64 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance52 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CapitalOperationsView));
            Infragistics.Win.Appearance appearance88 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance51 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance21 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance24 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance20 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance18 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance19 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance22 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance23 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance50 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance37 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance38 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance39 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance40 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance41 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance42 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance43 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance44 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance45 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance46 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance47 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance48 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab6 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab4 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab5 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab1 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab2 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinToolbars.UltraToolbar ultraToolbar1 = new Infragistics.Win.UltraWinToolbars.UltraToolbar("DataOperations");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool8 = new Infragistics.Win.UltraWinToolbars.ButtonTool("NewCalculation");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Infragistics.Win.UltraWinToolbars.ButtonTool("SaveData");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool2 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("PopupMenuTool1");
            Infragistics.Win.UltraWinToolbars.ComboBoxTool comboBoxTool1 = new Infragistics.Win.UltraWinToolbars.ComboBoxTool("Calculations");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool3 = new Infragistics.Win.UltraWinToolbars.ButtonTool("DeleteData");
            Infragistics.Win.UltraWinToolbars.UltraToolbar ultraToolbar2 = new Infragistics.Win.UltraWinToolbars.UltraToolbar("ToolBarCalculate");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Calculate");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool4 = new Infragistics.Win.UltraWinToolbars.ButtonTool("RefreshData");
            Infragistics.Win.Appearance appearance89 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool5 = new Infragistics.Win.UltraWinToolbars.ButtonTool("SaveData");
            Infragistics.Win.Appearance appearance90 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool6 = new Infragistics.Win.UltraWinToolbars.ButtonTool("DeleteData");
            Infragistics.Win.Appearance appearance91 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ComboBoxTool comboBoxTool2 = new Infragistics.Win.UltraWinToolbars.ComboBoxTool("Calculations");
            Infragistics.Win.ValueList valueList1 = new Infragistics.Win.ValueList(0);
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool7 = new Infragistics.Win.UltraWinToolbars.ButtonTool("NewCalculation");
            Infragistics.Win.Appearance appearance93 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool9 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Calculate");
            Infragistics.Win.Appearance appearance92 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool10 = new Infragistics.Win.UltraWinToolbars.ButtonTool("CreateReport");
            Infragistics.Win.Appearance appearance94 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance95 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool1 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("PopupMenuTool1");
            Infragistics.Win.Appearance appearance99 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool14 = new Infragistics.Win.UltraWinToolbars.ButtonTool("LoanLayout");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool15 = new Infragistics.Win.UltraWinToolbars.ButtonTool("LoanLayoutFull");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool16 = new Infragistics.Win.UltraWinToolbars.ButtonTool("LoanCompare");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool12 = new Infragistics.Win.UltraWinToolbars.ButtonTool("LoanLayout");
            Infragistics.Win.Appearance appearance97 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool13 = new Infragistics.Win.UltraWinToolbars.ButtonTool("LoanLayoutFull");
            Infragistics.Win.Appearance appearance98 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool11 = new Infragistics.Win.UltraWinToolbars.ButtonTool("LoanCompare");
            Infragistics.Win.Appearance appearance86 = new Infragistics.Win.Appearance();
            this.ultraTabPageControl6 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.ug1 = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.ultraTabPageControl4 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.ug2 = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.ultraTabPageControl5 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.ug3 = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.ultraTabPageControl1 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.ultraPanel4 = new Infragistics.Win.Misc.UltraPanel();
            this.ugNominal = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.ultraPanel1 = new Infragistics.Win.Misc.UltraPanel();
            this.btnClearNominalPays = new Infragistics.Win.Misc.UltraButton();
            this.btnDeleteNominal = new Infragistics.Win.Misc.UltraButton();
            this.btnAddNominal = new Infragistics.Win.Misc.UltraButton();
            this.label7 = new System.Windows.Forms.Label();
            this.ultraPanel3 = new Infragistics.Win.Misc.UltraPanel();
            this.ugCoupon = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.ultraPanel2 = new Infragistics.Win.Misc.UltraPanel();
            this.CouponsClear = new Infragistics.Win.Misc.UltraButton();
            this.label8 = new System.Windows.Forms.Label();
            this.ultraPanel5 = new Infragistics.Win.Misc.UltraPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.cbIsConstRate = new System.Windows.Forms.CheckBox();
            this.cbEndPeriodPayment = new System.Windows.Forms.CheckBox();
            this.Date = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.ne6 = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.ne5 = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.rb6 = new System.Windows.Forms.RadioButton();
            this.rb5 = new System.Windows.Forms.RadioButton();
            this.ne7 = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.CouponsCount = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.ne8 = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ne4 = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.ne3 = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.ne2 = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.ne1 = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.rbRubPrice = new System.Windows.Forms.RadioButton();
            this.rb3 = new System.Windows.Forms.RadioButton();
            this.rb2 = new System.Windows.Forms.RadioButton();
            this.rb1 = new System.Windows.Forms.RadioButton();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.ultraTabPageControl2 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.ultraPanel7 = new Infragistics.Win.Misc.UltraPanel();
            this.ugCapitalBond = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.ultraTabControl2 = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this.ultraTabSharedControlsPage2 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.ultraValidator = new Infragistics.Win.Misc.UltraValidator(this.components);
            this.ultraTabControl1 = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this.ultraTabSharedControlsPage1 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this._BaseView_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.tbManager = new Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(this.components);
            this._BaseView_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._BaseView_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._BaseView_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.ultraTabPageControl6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ug1)).BeginInit();
            this.ultraTabPageControl4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ug2)).BeginInit();
            this.ultraTabPageControl5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ug3)).BeginInit();
            this.ultraTabPageControl1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.ultraPanel4.ClientArea.SuspendLayout();
            this.ultraPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ugNominal)).BeginInit();
            this.ultraPanel1.ClientArea.SuspendLayout();
            this.ultraPanel1.SuspendLayout();
            this.ultraPanel3.ClientArea.SuspendLayout();
            this.ultraPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ugCoupon)).BeginInit();
            this.ultraPanel2.ClientArea.SuspendLayout();
            this.ultraPanel2.SuspendLayout();
            this.ultraPanel5.ClientArea.SuspendLayout();
            this.ultraPanel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Date)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ne6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ne5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ne7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CouponsCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ne8)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ne4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ne3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ne2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ne1)).BeginInit();
            this.ultraTabPageControl2.SuspendLayout();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.ultraPanel7.ClientArea.SuspendLayout();
            this.ultraPanel7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ugCapitalBond)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraTabControl2)).BeginInit();
            this.ultraTabControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraValidator)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraTabControl1)).BeginInit();
            this.ultraTabControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbManager)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraTabPageControl6
            // 
            this.ultraTabPageControl6.Controls.Add(this.ug1);
            this.ultraTabPageControl6.Location = new System.Drawing.Point(1, 20);
            this.ultraTabPageControl6.Name = "ultraTabPageControl6";
            this.ultraTabPageControl6.Size = new System.Drawing.Size(895, 286);
            // 
            // ug1
            // 
            appearance25.BackColor = System.Drawing.SystemColors.Window;
            appearance25.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.ug1.DisplayLayout.Appearance = appearance25;
            this.ug1.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.ug1.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance26.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance26.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance26.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance26.BorderColor = System.Drawing.SystemColors.Window;
            this.ug1.DisplayLayout.GroupByBox.Appearance = appearance26;
            appearance28.ForeColor = System.Drawing.SystemColors.GrayText;
            this.ug1.DisplayLayout.GroupByBox.BandLabelAppearance = appearance28;
            this.ug1.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.ug1.DisplayLayout.GroupByBox.Hidden = true;
            appearance27.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance27.BackColor2 = System.Drawing.SystemColors.Control;
            appearance27.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance27.ForeColor = System.Drawing.SystemColors.GrayText;
            this.ug1.DisplayLayout.GroupByBox.PromptAppearance = appearance27;
            this.ug1.DisplayLayout.MaxColScrollRegions = 1;
            this.ug1.DisplayLayout.MaxRowScrollRegions = 1;
            appearance33.BackColor = System.Drawing.SystemColors.Window;
            appearance33.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ug1.DisplayLayout.Override.ActiveCellAppearance = appearance33;
            appearance29.BackColor = System.Drawing.SystemColors.Highlight;
            appearance29.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.ug1.DisplayLayout.Override.ActiveRowAppearance = appearance29;
            this.ug1.DisplayLayout.Override.AllowColMoving = Infragistics.Win.UltraWinGrid.AllowColMoving.NotAllowed;
            this.ug1.DisplayLayout.Override.AllowColSwapping = Infragistics.Win.UltraWinGrid.AllowColSwapping.NotAllowed;
            this.ug1.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.ug1.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.ug1.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance36.BackColor = System.Drawing.SystemColors.Window;
            this.ug1.DisplayLayout.Override.CardAreaAppearance = appearance36;
            appearance32.BorderColor = System.Drawing.Color.Silver;
            appearance32.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.ug1.DisplayLayout.Override.CellAppearance = appearance32;
            this.ug1.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.CellSelect;
            this.ug1.DisplayLayout.Override.CellPadding = 0;
            appearance30.BackColor = System.Drawing.SystemColors.Control;
            appearance30.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance30.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance30.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance30.BorderColor = System.Drawing.SystemColors.Window;
            this.ug1.DisplayLayout.Override.GroupByRowAppearance = appearance30;
            appearance31.TextHAlignAsString = "Left";
            this.ug1.DisplayLayout.Override.HeaderAppearance = appearance31;
            this.ug1.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.ug1.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance34.BackColor = System.Drawing.SystemColors.Window;
            appearance34.BorderColor = System.Drawing.Color.Silver;
            this.ug1.DisplayLayout.Override.RowAppearance = appearance34;
            this.ug1.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            appearance35.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ug1.DisplayLayout.Override.TemplateAddRowAppearance = appearance35;
            this.ug1.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.ug1.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.ug1.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.ug1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ug1.Location = new System.Drawing.Point(0, 0);
            this.ug1.Name = "ug1";
            this.ug1.Size = new System.Drawing.Size(895, 286);
            this.ug1.TabIndex = 3;
            this.ug1.Text = "ultraGrid2";
            // 
            // ultraTabPageControl4
            // 
            this.ultraTabPageControl4.Controls.Add(this.ug2);
            this.ultraTabPageControl4.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl4.Name = "ultraTabPageControl4";
            this.ultraTabPageControl4.Size = new System.Drawing.Size(895, 286);
            // 
            // ug2
            // 
            appearance65.BackColor = System.Drawing.SystemColors.Window;
            appearance65.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.ug2.DisplayLayout.Appearance = appearance65;
            this.ug2.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.ug2.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance66.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance66.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance66.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance66.BorderColor = System.Drawing.SystemColors.Window;
            this.ug2.DisplayLayout.GroupByBox.Appearance = appearance66;
            appearance67.ForeColor = System.Drawing.SystemColors.GrayText;
            this.ug2.DisplayLayout.GroupByBox.BandLabelAppearance = appearance67;
            this.ug2.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.ug2.DisplayLayout.GroupByBox.Hidden = true;
            appearance68.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance68.BackColor2 = System.Drawing.SystemColors.Control;
            appearance68.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance68.ForeColor = System.Drawing.SystemColors.GrayText;
            this.ug2.DisplayLayout.GroupByBox.PromptAppearance = appearance68;
            this.ug2.DisplayLayout.MaxColScrollRegions = 1;
            this.ug2.DisplayLayout.MaxRowScrollRegions = 1;
            appearance69.BackColor = System.Drawing.SystemColors.Window;
            appearance69.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ug2.DisplayLayout.Override.ActiveCellAppearance = appearance69;
            appearance70.BackColor = System.Drawing.SystemColors.Highlight;
            appearance70.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.ug2.DisplayLayout.Override.ActiveRowAppearance = appearance70;
            this.ug2.DisplayLayout.Override.AllowColMoving = Infragistics.Win.UltraWinGrid.AllowColMoving.NotAllowed;
            this.ug2.DisplayLayout.Override.AllowColSwapping = Infragistics.Win.UltraWinGrid.AllowColSwapping.NotAllowed;
            this.ug2.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.ug2.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.ug2.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance71.BackColor = System.Drawing.SystemColors.Window;
            this.ug2.DisplayLayout.Override.CardAreaAppearance = appearance71;
            appearance72.BorderColor = System.Drawing.Color.Silver;
            appearance72.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.ug2.DisplayLayout.Override.CellAppearance = appearance72;
            this.ug2.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.CellSelect;
            this.ug2.DisplayLayout.Override.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
            this.ug2.DisplayLayout.Override.CellPadding = 0;
            appearance73.BackColor = System.Drawing.SystemColors.Control;
            appearance73.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance73.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance73.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance73.BorderColor = System.Drawing.SystemColors.Window;
            this.ug2.DisplayLayout.Override.GroupByRowAppearance = appearance73;
            appearance74.TextHAlignAsString = "Left";
            this.ug2.DisplayLayout.Override.HeaderAppearance = appearance74;
            this.ug2.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.ug2.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance75.BackColor = System.Drawing.SystemColors.Window;
            appearance75.BorderColor = System.Drawing.Color.Silver;
            this.ug2.DisplayLayout.Override.RowAppearance = appearance75;
            this.ug2.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            appearance76.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ug2.DisplayLayout.Override.TemplateAddRowAppearance = appearance76;
            this.ug2.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.ug2.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.ug2.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.ug2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ug2.Location = new System.Drawing.Point(0, 0);
            this.ug2.Name = "ug2";
            this.ug2.Size = new System.Drawing.Size(895, 286);
            this.ug2.TabIndex = 2;
            this.ug2.Text = "ultraGrid1";
            // 
            // ultraTabPageControl5
            // 
            this.ultraTabPageControl5.Controls.Add(this.ug3);
            this.ultraTabPageControl5.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl5.Name = "ultraTabPageControl5";
            this.ultraTabPageControl5.Size = new System.Drawing.Size(895, 286);
            // 
            // ug3
            // 
            appearance49.BackColor = System.Drawing.SystemColors.Window;
            appearance49.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.ug3.DisplayLayout.Appearance = appearance49;
            this.ug3.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.ug3.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance53.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance53.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance53.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance53.BorderColor = System.Drawing.SystemColors.Window;
            this.ug3.DisplayLayout.GroupByBox.Appearance = appearance53;
            appearance54.ForeColor = System.Drawing.SystemColors.GrayText;
            this.ug3.DisplayLayout.GroupByBox.BandLabelAppearance = appearance54;
            this.ug3.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.ug3.DisplayLayout.GroupByBox.Hidden = true;
            appearance55.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance55.BackColor2 = System.Drawing.SystemColors.Control;
            appearance55.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance55.ForeColor = System.Drawing.SystemColors.GrayText;
            this.ug3.DisplayLayout.GroupByBox.PromptAppearance = appearance55;
            this.ug3.DisplayLayout.MaxColScrollRegions = 1;
            this.ug3.DisplayLayout.MaxRowScrollRegions = 1;
            appearance56.BackColor = System.Drawing.SystemColors.Window;
            appearance56.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ug3.DisplayLayout.Override.ActiveCellAppearance = appearance56;
            appearance57.BackColor = System.Drawing.SystemColors.Highlight;
            appearance57.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.ug3.DisplayLayout.Override.ActiveRowAppearance = appearance57;
            this.ug3.DisplayLayout.Override.AllowColMoving = Infragistics.Win.UltraWinGrid.AllowColMoving.NotAllowed;
            this.ug3.DisplayLayout.Override.AllowColSwapping = Infragistics.Win.UltraWinGrid.AllowColSwapping.NotAllowed;
            this.ug3.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.ug3.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.ug3.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance58.BackColor = System.Drawing.SystemColors.Window;
            this.ug3.DisplayLayout.Override.CardAreaAppearance = appearance58;
            appearance59.BorderColor = System.Drawing.Color.Silver;
            appearance59.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.ug3.DisplayLayout.Override.CellAppearance = appearance59;
            this.ug3.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.CellSelect;
            this.ug3.DisplayLayout.Override.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
            this.ug3.DisplayLayout.Override.CellPadding = 0;
            appearance61.BackColor = System.Drawing.SystemColors.Control;
            appearance61.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance61.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance61.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance61.BorderColor = System.Drawing.SystemColors.Window;
            this.ug3.DisplayLayout.Override.GroupByRowAppearance = appearance61;
            appearance62.TextHAlignAsString = "Left";
            this.ug3.DisplayLayout.Override.HeaderAppearance = appearance62;
            this.ug3.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.ug3.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance63.BackColor = System.Drawing.SystemColors.Window;
            appearance63.BorderColor = System.Drawing.Color.Silver;
            this.ug3.DisplayLayout.Override.RowAppearance = appearance63;
            this.ug3.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            appearance64.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ug3.DisplayLayout.Override.TemplateAddRowAppearance = appearance64;
            this.ug3.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.ug3.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.ug3.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.ug3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ug3.Location = new System.Drawing.Point(0, 0);
            this.ug3.Name = "ug3";
            this.ug3.Size = new System.Drawing.Size(895, 286);
            this.ug3.TabIndex = 2;
            this.ug3.Text = "ultraGrid4";
            // 
            // ultraTabPageControl1
            // 
            this.ultraTabPageControl1.Controls.Add(this.splitContainer2);
            this.ultraTabPageControl1.Controls.Add(this.ultraPanel5);
            this.ultraTabPageControl1.Location = new System.Drawing.Point(1, 20);
            this.ultraTabPageControl1.Name = "ultraTabPageControl1";
            this.ultraTabPageControl1.Size = new System.Drawing.Size(897, 461);
            // 
            // splitContainer2
            // 
            this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 222);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.ultraPanel4);
            this.splitContainer2.Panel1.Controls.Add(this.ultraPanel1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.ultraPanel3);
            this.splitContainer2.Panel2.Controls.Add(this.ultraPanel2);
            this.splitContainer2.Size = new System.Drawing.Size(897, 239);
            this.splitContainer2.SplitterDistance = 425;
            this.splitContainer2.TabIndex = 2;
            // 
            // ultraPanel4
            // 
            this.ultraPanel4.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            // 
            // ultraPanel4.ClientArea
            // 
            this.ultraPanel4.ClientArea.Controls.Add(this.ugNominal);
            this.ultraPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraPanel4.Location = new System.Drawing.Point(0, 41);
            this.ultraPanel4.Name = "ultraPanel4";
            this.ultraPanel4.Size = new System.Drawing.Size(423, 196);
            this.ultraPanel4.TabIndex = 3;
            // 
            // ugNominal
            // 
            appearance1.BackColor = System.Drawing.SystemColors.Window;
            appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.ugNominal.DisplayLayout.Appearance = appearance1;
            this.ugNominal.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.ugNominal.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance2.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance2.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance2.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance2.BorderColor = System.Drawing.SystemColors.Window;
            this.ugNominal.DisplayLayout.GroupByBox.Appearance = appearance2;
            appearance4.ForeColor = System.Drawing.SystemColors.GrayText;
            this.ugNominal.DisplayLayout.GroupByBox.BandLabelAppearance = appearance4;
            this.ugNominal.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.ugNominal.DisplayLayout.GroupByBox.Hidden = true;
            appearance3.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance3.BackColor2 = System.Drawing.SystemColors.Control;
            appearance3.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance3.ForeColor = System.Drawing.SystemColors.GrayText;
            this.ugNominal.DisplayLayout.GroupByBox.PromptAppearance = appearance3;
            this.ugNominal.DisplayLayout.MaxColScrollRegions = 1;
            this.ugNominal.DisplayLayout.MaxRowScrollRegions = 1;
            appearance9.BackColor = System.Drawing.SystemColors.Window;
            appearance9.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ugNominal.DisplayLayout.Override.ActiveCellAppearance = appearance9;
            appearance5.BackColor = System.Drawing.SystemColors.Highlight;
            appearance5.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.ugNominal.DisplayLayout.Override.ActiveRowAppearance = appearance5;
            this.ugNominal.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.ugNominal.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.ugNominal.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance12.BackColor = System.Drawing.SystemColors.Window;
            this.ugNominal.DisplayLayout.Override.CardAreaAppearance = appearance12;
            appearance8.BorderColor = System.Drawing.Color.Silver;
            appearance8.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.ugNominal.DisplayLayout.Override.CellAppearance = appearance8;
            this.ugNominal.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.ugNominal.DisplayLayout.Override.CellPadding = 0;
            appearance6.BackColor = System.Drawing.SystemColors.Control;
            appearance6.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance6.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance6.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance6.BorderColor = System.Drawing.SystemColors.Window;
            this.ugNominal.DisplayLayout.Override.GroupByRowAppearance = appearance6;
            appearance7.TextHAlignAsString = "Left";
            this.ugNominal.DisplayLayout.Override.HeaderAppearance = appearance7;
            this.ugNominal.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.ugNominal.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance10.BackColor = System.Drawing.SystemColors.Window;
            appearance10.BorderColor = System.Drawing.Color.Silver;
            this.ugNominal.DisplayLayout.Override.RowAppearance = appearance10;
            this.ugNominal.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            appearance11.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ugNominal.DisplayLayout.Override.TemplateAddRowAppearance = appearance11;
            this.ugNominal.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.ugNominal.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.ugNominal.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.ugNominal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ugNominal.Location = new System.Drawing.Point(0, 0);
            this.ugNominal.Name = "ugNominal";
            this.ugNominal.Size = new System.Drawing.Size(423, 196);
            this.ugNominal.TabIndex = 2;
            this.ugNominal.Text = "ultraGrid1";
            this.ugNominal.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ugNominal_InitializeLayout);
            // 
            // ultraPanel1
            // 
            this.ultraPanel1.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            // 
            // ultraPanel1.ClientArea
            // 
            this.ultraPanel1.ClientArea.Controls.Add(this.btnClearNominalPays);
            this.ultraPanel1.ClientArea.Controls.Add(this.btnDeleteNominal);
            this.ultraPanel1.ClientArea.Controls.Add(this.btnAddNominal);
            this.ultraPanel1.ClientArea.Controls.Add(this.label7);
            this.ultraPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.ultraPanel1.Location = new System.Drawing.Point(0, 0);
            this.ultraPanel1.Name = "ultraPanel1";
            this.ultraPanel1.Size = new System.Drawing.Size(423, 41);
            this.ultraPanel1.TabIndex = 2;
            // 
            // btnClearNominalPays
            // 
            appearance52.Image = ((object)(resources.GetObject("appearance52.Image")));
            appearance52.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance52.ImageVAlign = Infragistics.Win.VAlign.Middle;
            this.btnClearNominalPays.Appearance = appearance52;
            this.btnClearNominalPays.ButtonStyle = Infragistics.Win.UIElementButtonStyle.ButtonSoft;
            this.btnClearNominalPays.Location = new System.Drawing.Point(72, 7);
            this.btnClearNominalPays.Name = "btnClearNominalPays";
            this.btnClearNominalPays.Size = new System.Drawing.Size(24, 24);
            this.btnClearNominalPays.TabIndex = 1;
            this.btnClearNominalPays.Click += new System.EventHandler(this.btnDeleteNominal_Click);
            // 
            // btnDeleteNominal
            // 
            appearance88.Image = ((object)(resources.GetObject("appearance88.Image")));
            appearance88.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance88.ImageVAlign = Infragistics.Win.VAlign.Middle;
            this.btnDeleteNominal.Appearance = appearance88;
            this.btnDeleteNominal.ButtonStyle = Infragistics.Win.UIElementButtonStyle.ButtonSoft;
            this.btnDeleteNominal.Location = new System.Drawing.Point(42, 7);
            this.btnDeleteNominal.Name = "btnDeleteNominal";
            this.btnDeleteNominal.Size = new System.Drawing.Size(24, 24);
            this.btnDeleteNominal.TabIndex = 1;
            this.btnDeleteNominal.Click += new System.EventHandler(this.btnDeleteNominal_Click);
            // 
            // btnAddNominal
            // 
            appearance51.Image = ((object)(resources.GetObject("appearance51.Image")));
            appearance51.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance51.ImageVAlign = Infragistics.Win.VAlign.Middle;
            this.btnAddNominal.Appearance = appearance51;
            this.btnAddNominal.ButtonStyle = Infragistics.Win.UIElementButtonStyle.ButtonSoft;
            this.btnAddNominal.Location = new System.Drawing.Point(12, 7);
            this.btnAddNominal.Name = "btnAddNominal";
            this.btnAddNominal.Size = new System.Drawing.Size(24, 24);
            this.btnAddNominal.TabIndex = 1;
            this.btnAddNominal.Click += new System.EventHandler(this.btnAddNominal_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(104, 13);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(192, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Погашение номинальной стоимости";
            // 
            // ultraPanel3
            // 
            this.ultraPanel3.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            // 
            // ultraPanel3.ClientArea
            // 
            this.ultraPanel3.ClientArea.Controls.Add(this.ugCoupon);
            this.ultraPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraPanel3.Location = new System.Drawing.Point(0, 41);
            this.ultraPanel3.Name = "ultraPanel3";
            this.ultraPanel3.Size = new System.Drawing.Size(466, 196);
            this.ultraPanel3.TabIndex = 4;
            // 
            // ugCoupon
            // 
            appearance13.BackColor = System.Drawing.SystemColors.Window;
            appearance13.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.ugCoupon.DisplayLayout.Appearance = appearance13;
            this.ugCoupon.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.ugCoupon.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance14.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance14.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance14.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance14.BorderColor = System.Drawing.SystemColors.Window;
            this.ugCoupon.DisplayLayout.GroupByBox.Appearance = appearance14;
            appearance16.ForeColor = System.Drawing.SystemColors.GrayText;
            this.ugCoupon.DisplayLayout.GroupByBox.BandLabelAppearance = appearance16;
            this.ugCoupon.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.ugCoupon.DisplayLayout.GroupByBox.Hidden = true;
            appearance15.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance15.BackColor2 = System.Drawing.SystemColors.Control;
            appearance15.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance15.ForeColor = System.Drawing.SystemColors.GrayText;
            this.ugCoupon.DisplayLayout.GroupByBox.PromptAppearance = appearance15;
            this.ugCoupon.DisplayLayout.MaxColScrollRegions = 1;
            this.ugCoupon.DisplayLayout.MaxRowScrollRegions = 1;
            appearance21.BackColor = System.Drawing.SystemColors.Window;
            appearance21.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ugCoupon.DisplayLayout.Override.ActiveCellAppearance = appearance21;
            appearance17.BackColor = System.Drawing.SystemColors.Highlight;
            appearance17.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.ugCoupon.DisplayLayout.Override.ActiveRowAppearance = appearance17;
            this.ugCoupon.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.ugCoupon.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.ugCoupon.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance24.BackColor = System.Drawing.SystemColors.Window;
            this.ugCoupon.DisplayLayout.Override.CardAreaAppearance = appearance24;
            appearance20.BorderColor = System.Drawing.Color.Silver;
            appearance20.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.ugCoupon.DisplayLayout.Override.CellAppearance = appearance20;
            this.ugCoupon.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.ugCoupon.DisplayLayout.Override.CellPadding = 0;
            appearance18.BackColor = System.Drawing.SystemColors.Control;
            appearance18.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance18.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance18.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance18.BorderColor = System.Drawing.SystemColors.Window;
            this.ugCoupon.DisplayLayout.Override.GroupByRowAppearance = appearance18;
            appearance19.TextHAlignAsString = "Left";
            this.ugCoupon.DisplayLayout.Override.HeaderAppearance = appearance19;
            this.ugCoupon.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.ugCoupon.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance22.BackColor = System.Drawing.SystemColors.Window;
            appearance22.BorderColor = System.Drawing.Color.Silver;
            this.ugCoupon.DisplayLayout.Override.RowAppearance = appearance22;
            this.ugCoupon.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            appearance23.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ugCoupon.DisplayLayout.Override.TemplateAddRowAppearance = appearance23;
            this.ugCoupon.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.ugCoupon.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.ugCoupon.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.ugCoupon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ugCoupon.Location = new System.Drawing.Point(0, 0);
            this.ugCoupon.Name = "ugCoupon";
            this.ugCoupon.Size = new System.Drawing.Size(466, 196);
            this.ugCoupon.TabIndex = 1;
            this.ugCoupon.Text = "ultraGrid2";
            this.ugCoupon.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ugNominal_InitializeLayout);
            // 
            // ultraPanel2
            // 
            this.ultraPanel2.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            // 
            // ultraPanel2.ClientArea
            // 
            this.ultraPanel2.ClientArea.Controls.Add(this.CouponsClear);
            this.ultraPanel2.ClientArea.Controls.Add(this.label8);
            this.ultraPanel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.ultraPanel2.Location = new System.Drawing.Point(0, 0);
            this.ultraPanel2.Name = "ultraPanel2";
            this.ultraPanel2.Size = new System.Drawing.Size(466, 41);
            this.ultraPanel2.TabIndex = 3;
            // 
            // CouponsClear
            // 
            appearance50.Image = ((object)(resources.GetObject("appearance50.Image")));
            appearance50.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance50.ImageVAlign = Infragistics.Win.VAlign.Middle;
            this.CouponsClear.Appearance = appearance50;
            this.CouponsClear.ButtonStyle = Infragistics.Win.UIElementButtonStyle.ButtonSoft;
            this.CouponsClear.Location = new System.Drawing.Point(12, 7);
            this.CouponsClear.Name = "CouponsClear";
            this.CouponsClear.Size = new System.Drawing.Size(24, 24);
            this.CouponsClear.TabIndex = 1;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(44, 13);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(45, 13);
            this.label8.TabIndex = 0;
            this.label8.Text = "Купоны";
            // 
            // ultraPanel5
            // 
            this.ultraPanel5.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            // 
            // ultraPanel5.ClientArea
            // 
            this.ultraPanel5.ClientArea.Controls.Add(this.label2);
            this.ultraPanel5.ClientArea.Controls.Add(this.cbIsConstRate);
            this.ultraPanel5.ClientArea.Controls.Add(this.cbEndPeriodPayment);
            this.ultraPanel5.ClientArea.Controls.Add(this.Date);
            this.ultraPanel5.ClientArea.Controls.Add(this.groupBox2);
            this.ultraPanel5.ClientArea.Controls.Add(this.ne7);
            this.ultraPanel5.ClientArea.Controls.Add(this.CouponsCount);
            this.ultraPanel5.ClientArea.Controls.Add(this.ne8);
            this.ultraPanel5.ClientArea.Controls.Add(this.groupBox1);
            this.ultraPanel5.ClientArea.Controls.Add(this.label6);
            this.ultraPanel5.ClientArea.Controls.Add(this.label5);
            this.ultraPanel5.ClientArea.Controls.Add(this.label4);
            this.ultraPanel5.ClientArea.Controls.Add(this.label3);
            this.ultraPanel5.ClientArea.Controls.Add(this.comboBox2);
            this.ultraPanel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.ultraPanel5.Location = new System.Drawing.Point(0, 0);
            this.ultraPanel5.Name = "ultraPanel5";
            this.ultraPanel5.Size = new System.Drawing.Size(897, 222);
            this.ultraPanel5.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 149);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(168, 13);
            this.label2.TabIndex = 28;
            this.label2.Text = "Количество купонных периодов";
            // 
            // cbIsConstRate
            // 
            this.cbIsConstRate.AutoSize = true;
            this.cbIsConstRate.Checked = true;
            this.cbIsConstRate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbIsConstRate.Location = new System.Drawing.Point(20, 175);
            this.cbIsConstRate.Name = "cbIsConstRate";
            this.cbIsConstRate.Size = new System.Drawing.Size(218, 17);
            this.cbIsConstRate.TabIndex = 27;
            this.cbIsConstRate.Text = "Постоянная ставка купонного дохода";
            this.cbIsConstRate.UseVisualStyleBackColor = true;
            // 
            // cbEndPeriodPayment
            // 
            this.cbEndPeriodPayment.AutoSize = true;
            this.cbEndPeriodPayment.Location = new System.Drawing.Point(20, 119);
            this.cbEndPeriodPayment.Name = "cbEndPeriodPayment";
            this.cbEndPeriodPayment.Size = new System.Drawing.Size(286, 17);
            this.cbEndPeriodPayment.TabIndex = 2;
            this.cbEndPeriodPayment.Text = "Погашение номинальной стоимости в конце срока";
            this.cbEndPeriodPayment.UseVisualStyleBackColor = true;
            this.cbEndPeriodPayment.CheckedChanged += new System.EventHandler(this.cbEndPeriodPayment_CheckedChanged);
            // 
            // Date
            // 
            this.Date.Location = new System.Drawing.Point(281, 91);
            this.Date.Name = "Date";
            this.Date.Size = new System.Drawing.Size(93, 21);
            this.Date.TabIndex = 26;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.ne6);
            this.groupBox2.Controls.Add(this.ne5);
            this.groupBox2.Controls.Add(this.rb6);
            this.groupBox2.Controls.Add(this.rb5);
            this.groupBox2.Location = new System.Drawing.Point(391, 123);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(451, 65);
            this.groupBox2.TabIndex = 25;
            this.groupBox2.TabStop = false;
            // 
            // ne6
            // 
            this.ne6.Location = new System.Drawing.Point(318, 11);
            this.ne6.MaskInput = "nnn,nnn,nnn,nnn,nnn";
            this.ne6.MaxValue = new decimal(new int[] {
            -1530494977,
            232830,
            0,
            0});
            this.ne6.MinValue = new decimal(new int[] {
            -1530494977,
            232830,
            0,
            -2147483648});
            this.ne6.Name = "ne6";
            this.ne6.Nullable = true;
            this.ne6.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Decimal;
            this.ne6.PromptChar = ' ';
            this.ne6.Size = new System.Drawing.Size(125, 21);
            this.ne6.TabIndex = 5;
            this.ne6.Tag = "99999999";
            this.ne6.ValueChanged += new System.EventHandler(this.ne7_ValueChanged);
            // 
            // ne5
            // 
            this.ne5.Location = new System.Drawing.Point(318, 38);
            this.ne5.MaskInput = "-nnn,nnn,nnn,nnn,nnn";
            this.ne5.MaxValue = new decimal(new int[] {
            -1530494977,
            232830,
            0,
            0});
            this.ne5.MinValue = new decimal(new int[] {
            -1530494977,
            232830,
            0,
            -2147483648});
            this.ne5.Name = "ne5";
            this.ne5.Nullable = true;
            this.ne5.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Decimal;
            this.ne5.PromptChar = ' ';
            this.ne5.Size = new System.Drawing.Size(125, 21);
            this.ne5.TabIndex = 5;
            this.ne5.Tag = "999999999999";
            // 
            // rb6
            // 
            this.rb6.AutoSize = true;
            this.rb6.Location = new System.Drawing.Point(6, 38);
            this.rb6.Name = "rb6";
            this.rb6.Size = new System.Drawing.Size(260, 17);
            this.rb6.TabIndex = 1;
            this.rb6.TabStop = true;
            this.rb6.Text = "Объем средств от размещаемого займа, руб.";
            this.rb6.UseVisualStyleBackColor = true;
            this.rb6.CheckedChanged += new System.EventHandler(this.rb5_CheckedChanged);
            // 
            // rb5
            // 
            this.rb5.AutoSize = true;
            this.rb5.Location = new System.Drawing.Point(6, 10);
            this.rb5.Name = "rb5";
            this.rb5.Size = new System.Drawing.Size(278, 17);
            this.rb5.TabIndex = 1;
            this.rb5.TabStop = true;
            this.rb5.Text = "Количество облигаций размещаемого займа, шт.";
            this.rb5.UseVisualStyleBackColor = true;
            this.rb5.CheckedChanged += new System.EventHandler(this.rb5_CheckedChanged);
            // 
            // ne7
            // 
            this.ne7.Location = new System.Drawing.Point(318, 37);
            this.ne7.MaskInput = "n,nnn";
            this.ne7.MaxValue = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.ne7.MinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.ne7.Name = "ne7";
            this.ne7.Nullable = true;
            this.ne7.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Decimal;
            this.ne7.Size = new System.Drawing.Size(56, 21);
            this.ne7.TabIndex = 20;
            this.ne7.ValueChanged += new System.EventHandler(this.ne7_ValueChanged);
            // 
            // CouponsCount
            // 
            this.CouponsCount.Location = new System.Drawing.Point(318, 147);
            this.CouponsCount.MaskInput = "nn";
            this.CouponsCount.Name = "CouponsCount";
            this.CouponsCount.Nullable = true;
            this.CouponsCount.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Decimal;
            this.CouponsCount.Size = new System.Drawing.Size(56, 21);
            this.CouponsCount.TabIndex = 19;
            this.CouponsCount.ValueChanged += new System.EventHandler(this.ne7_ValueChanged);
            // 
            // ne8
            // 
            this.ne8.Location = new System.Drawing.Point(318, 64);
            this.ne8.MaskInput = "n,nnn";
            this.ne8.Name = "ne8";
            this.ne8.Nullable = true;
            this.ne8.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Decimal;
            this.ne8.Size = new System.Drawing.Size(56, 21);
            this.ne8.TabIndex = 19;
            this.ne8.ValueChanged += new System.EventHandler(this.ne7_ValueChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ne4);
            this.groupBox1.Controls.Add(this.ne3);
            this.groupBox1.Controls.Add(this.ne2);
            this.groupBox1.Controls.Add(this.ne1);
            this.groupBox1.Controls.Add(this.rbRubPrice);
            this.groupBox1.Controls.Add(this.rb3);
            this.groupBox1.Controls.Add(this.rb2);
            this.groupBox1.Controls.Add(this.rb1);
            this.groupBox1.Location = new System.Drawing.Point(391, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(451, 115);
            this.groupBox1.TabIndex = 24;
            this.groupBox1.TabStop = false;
            // 
            // ne4
            // 
            this.ne4.Location = new System.Drawing.Point(373, 88);
            this.ne4.MaskInput = "-nnnn.nn";
            this.ne4.MaxValue = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.ne4.MinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.ne4.Name = "ne4";
            this.ne4.Nullable = true;
            this.ne4.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Decimal;
            this.ne4.Size = new System.Drawing.Size(70, 21);
            this.ne4.TabIndex = 4;
            this.ne4.ValueChanged += new System.EventHandler(this.ne4_ValueChanged);
            // 
            // ne3
            // 
            this.ne3.Location = new System.Drawing.Point(373, 62);
            this.ne3.MaskInput = "-nnn.nnn";
            this.ne3.MaxValue = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.ne3.MinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.ne3.Name = "ne3";
            this.ne3.Nullable = true;
            this.ne3.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Decimal;
            this.ne3.Size = new System.Drawing.Size(70, 21);
            this.ne3.TabIndex = 4;
            this.ne3.ValueChanged += new System.EventHandler(this.ne3_ValueChanged);
            // 
            // ne2
            // 
            this.ne2.Location = new System.Drawing.Point(373, 36);
            this.ne2.MaskInput = "-nnn.nnnn";
            this.ne2.Name = "ne2";
            this.ne2.Nullable = true;
            this.ne2.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Decimal;
            this.ne2.Size = new System.Drawing.Size(70, 21);
            this.ne2.TabIndex = 4;
            this.ne2.Tag = "99,9999";
            this.ne2.ValueChanged += new System.EventHandler(this.ne3_ValueChanged);
            // 
            // ne1
            // 
            this.ne1.Location = new System.Drawing.Point(373, 10);
            this.ne1.MaskInput = "-nn.nn";
            this.ne1.Name = "ne1";
            this.ne1.Nullable = true;
            this.ne1.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Decimal;
            this.ne1.Size = new System.Drawing.Size(70, 21);
            this.ne1.TabIndex = 4;
            this.ne1.ValueChanged += new System.EventHandler(this.ne3_ValueChanged);
            // 
            // rbRubPrice
            // 
            this.rbRubPrice.AutoSize = true;
            this.rbRubPrice.Location = new System.Drawing.Point(6, 90);
            this.rbRubPrice.Name = "rbRubPrice";
            this.rbRubPrice.Size = new System.Drawing.Size(175, 17);
            this.rbRubPrice.TabIndex = 2;
            this.rbRubPrice.TabStop = true;
            this.rbRubPrice.Text = "Текущая рыночная цена, руб.";
            this.rbRubPrice.UseVisualStyleBackColor = true;
            this.rbRubPrice.CheckedChanged += new System.EventHandler(this.rb1_CheckedChanged);
            // 
            // rb3
            // 
            this.rb3.AutoSize = true;
            this.rb3.Location = new System.Drawing.Point(6, 64);
            this.rb3.Name = "rb3";
            this.rb3.Size = new System.Drawing.Size(163, 17);
            this.rb3.TabIndex = 2;
            this.rb3.TabStop = true;
            this.rb3.Text = "Текущая рыночная цена, %";
            this.rb3.UseVisualStyleBackColor = true;
            this.rb3.CheckedChanged += new System.EventHandler(this.rb1_CheckedChanged);
            // 
            // rb2
            // 
            this.rb2.AutoSize = true;
            this.rb2.Location = new System.Drawing.Point(6, 36);
            this.rb2.Name = "rb2";
            this.rb2.Size = new System.Drawing.Size(284, 17);
            this.rb2.TabIndex = 1;
            this.rb2.TabStop = true;
            this.rb2.Text = "Эффективная доходность к погашению, % годовых";
            this.rb2.UseVisualStyleBackColor = true;
            this.rb2.CheckedChanged += new System.EventHandler(this.rb1_CheckedChanged);
            // 
            // rb1
            // 
            this.rb1.AutoSize = true;
            this.rb1.Location = new System.Drawing.Point(6, 12);
            this.rb1.Name = "rb1";
            this.rb1.Size = new System.Drawing.Size(213, 17);
            this.rb1.TabIndex = 0;
            this.rb1.TabStop = true;
            this.rb1.Text = "Ставка купонного дохода, % годовых";
            this.rb1.UseVisualStyleBackColor = true;
            this.rb1.CheckedChanged += new System.EventHandler(this.rb1_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(16, 95);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(215, 13);
            this.label6.TabIndex = 23;
            this.label6.Text = "Дата начала первого купонного периода";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(17, 69);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(122, 13);
            this.label5.TabIndex = 21;
            this.label5.Text = "Срок обращения, дней";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 43);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(160, 13);
            this.label4.TabIndex = 18;
            this.label4.Text = "Номинальная стоимость, руб.";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(17, 14);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(279, 21);
            this.label3.TabIndex = 17;
            this.label3.Text = "Принимаемое для расчета количество дней в году";
            // 
            // comboBox2
            // 
            this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Items.AddRange(new object[] {
            "360",
            "365",
            "366"});
            this.comboBox2.Location = new System.Drawing.Point(318, 10);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(56, 21);
            this.comboBox2.TabIndex = 15;
            // 
            // ultraTabPageControl2
            // 
            this.ultraTabPageControl2.Controls.Add(this.splitContainer3);
            this.ultraTabPageControl2.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl2.Name = "ultraTabPageControl2";
            this.ultraTabPageControl2.Size = new System.Drawing.Size(897, 461);
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.ultraPanel7);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.ultraTabControl2);
            this.splitContainer3.Size = new System.Drawing.Size(897, 461);
            this.splitContainer3.SplitterDistance = 150;
            this.splitContainer3.TabIndex = 0;
            // 
            // ultraPanel7
            // 
            // 
            // ultraPanel7.ClientArea
            // 
            this.ultraPanel7.ClientArea.Controls.Add(this.ugCapitalBond);
            this.ultraPanel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraPanel7.Location = new System.Drawing.Point(0, 0);
            this.ultraPanel7.Name = "ultraPanel7";
            this.ultraPanel7.Size = new System.Drawing.Size(897, 150);
            this.ultraPanel7.TabIndex = 2;
            // 
            // ugCapitalBond
            // 
            appearance37.BackColor = System.Drawing.SystemColors.Window;
            appearance37.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.ugCapitalBond.DisplayLayout.Appearance = appearance37;
            this.ugCapitalBond.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.ugCapitalBond.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance38.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance38.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance38.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance38.BorderColor = System.Drawing.SystemColors.Window;
            this.ugCapitalBond.DisplayLayout.GroupByBox.Appearance = appearance38;
            appearance39.ForeColor = System.Drawing.SystemColors.GrayText;
            this.ugCapitalBond.DisplayLayout.GroupByBox.BandLabelAppearance = appearance39;
            this.ugCapitalBond.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.ugCapitalBond.DisplayLayout.GroupByBox.Hidden = true;
            appearance40.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance40.BackColor2 = System.Drawing.SystemColors.Control;
            appearance40.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance40.ForeColor = System.Drawing.SystemColors.GrayText;
            this.ugCapitalBond.DisplayLayout.GroupByBox.PromptAppearance = appearance40;
            this.ugCapitalBond.DisplayLayout.MaxColScrollRegions = 1;
            this.ugCapitalBond.DisplayLayout.MaxRowScrollRegions = 1;
            appearance41.BackColor = System.Drawing.SystemColors.Window;
            appearance41.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ugCapitalBond.DisplayLayout.Override.ActiveCellAppearance = appearance41;
            appearance42.BackColor = System.Drawing.SystemColors.Highlight;
            appearance42.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.ugCapitalBond.DisplayLayout.Override.ActiveRowAppearance = appearance42;
            this.ugCapitalBond.DisplayLayout.Override.AllowColMoving = Infragistics.Win.UltraWinGrid.AllowColMoving.NotAllowed;
            this.ugCapitalBond.DisplayLayout.Override.AllowColSwapping = Infragistics.Win.UltraWinGrid.AllowColSwapping.NotAllowed;
            this.ugCapitalBond.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.ugCapitalBond.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.ugCapitalBond.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance43.BackColor = System.Drawing.SystemColors.Window;
            this.ugCapitalBond.DisplayLayout.Override.CardAreaAppearance = appearance43;
            appearance44.BorderColor = System.Drawing.Color.Silver;
            appearance44.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.ugCapitalBond.DisplayLayout.Override.CellAppearance = appearance44;
            this.ugCapitalBond.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.CellSelect;
            this.ugCapitalBond.DisplayLayout.Override.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
            this.ugCapitalBond.DisplayLayout.Override.CellPadding = 0;
            appearance45.BackColor = System.Drawing.SystemColors.Control;
            appearance45.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance45.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance45.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance45.BorderColor = System.Drawing.SystemColors.Window;
            this.ugCapitalBond.DisplayLayout.Override.GroupByRowAppearance = appearance45;
            appearance46.TextHAlignAsString = "Left";
            this.ugCapitalBond.DisplayLayout.Override.HeaderAppearance = appearance46;
            this.ugCapitalBond.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.ugCapitalBond.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance47.BackColor = System.Drawing.SystemColors.Window;
            appearance47.BorderColor = System.Drawing.Color.Silver;
            this.ugCapitalBond.DisplayLayout.Override.RowAppearance = appearance47;
            this.ugCapitalBond.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            appearance48.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ugCapitalBond.DisplayLayout.Override.TemplateAddRowAppearance = appearance48;
            this.ugCapitalBond.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.ugCapitalBond.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.ugCapitalBond.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.ugCapitalBond.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ugCapitalBond.Location = new System.Drawing.Point(0, 0);
            this.ugCapitalBond.Name = "ugCapitalBond";
            this.ugCapitalBond.Size = new System.Drawing.Size(897, 150);
            this.ugCapitalBond.TabIndex = 1;
            this.ugCapitalBond.Text = "ultraGrid3";
            // 
            // ultraTabControl2
            // 
            this.ultraTabControl2.Controls.Add(this.ultraTabSharedControlsPage2);
            this.ultraTabControl2.Controls.Add(this.ultraTabPageControl4);
            this.ultraTabControl2.Controls.Add(this.ultraTabPageControl5);
            this.ultraTabControl2.Controls.Add(this.ultraTabPageControl6);
            this.ultraTabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraTabControl2.Location = new System.Drawing.Point(0, 0);
            this.ultraTabControl2.Name = "ultraTabControl2";
            this.ultraTabControl2.SharedControlsPage = this.ultraTabSharedControlsPage2;
            this.ultraTabControl2.Size = new System.Drawing.Size(897, 307);
            this.ultraTabControl2.Style = Infragistics.Win.UltraWinTabControl.UltraTabControlStyle.Excel;
            this.ultraTabControl2.TabIndex = 0;
            ultraTab6.TabPage = this.ultraTabPageControl6;
            ultraTab6.Text = "Погашение номинальной стоимости";
            ultraTab4.TabPage = this.ultraTabPageControl4;
            ultraTab4.Text = "Расходы по выплате купонного дохода";
            ultraTab5.TabPage = this.ultraTabPageControl5;
            ultraTab5.Text = "Расходы по выплате с учетом фактических дат";
            this.ultraTabControl2.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab6,
            ultraTab4,
            ultraTab5});
            // 
            // ultraTabSharedControlsPage2
            // 
            this.ultraTabSharedControlsPage2.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage2.Name = "ultraTabSharedControlsPage2";
            this.ultraTabSharedControlsPage2.Size = new System.Drawing.Size(895, 286);
            // 
            // ultraValidator
            // 
            this.ultraValidator.ErrorImageAlignment = System.Windows.Forms.ErrorIconAlignment.MiddleLeft;
            this.ultraValidator.MessageBoxIcon = System.Windows.Forms.MessageBoxIcon.Warning;
            this.ultraValidator.NotificationSettings.Action = Infragistics.Win.Misc.NotificationAction.Image;
            // 
            // ultraTabControl1
            // 
            this.ultraTabControl1.Controls.Add(this.ultraTabSharedControlsPage1);
            this.ultraTabControl1.Controls.Add(this.ultraTabPageControl1);
            this.ultraTabControl1.Controls.Add(this.ultraTabPageControl2);
            this.ultraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraTabControl1.Location = new System.Drawing.Point(0, 54);
            this.ultraTabControl1.Name = "ultraTabControl1";
            this.ultraTabControl1.SharedControlsPage = this.ultraTabSharedControlsPage1;
            this.ultraTabControl1.Size = new System.Drawing.Size(899, 482);
            this.ultraTabControl1.Style = Infragistics.Win.UltraWinTabControl.UltraTabControlStyle.Excel;
            this.ultraTabControl1.TabIndex = 0;
            ultraTab1.TabPage = this.ultraTabPageControl1;
            ultraTab1.Text = "Параметры расчета";
            ultraTab2.TabPage = this.ultraTabPageControl2;
            ultraTab2.Text = "Размещение облигационного займа";
            this.ultraTabControl1.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab1,
            ultraTab2});
            // 
            // ultraTabSharedControlsPage1
            // 
            this.ultraTabSharedControlsPage1.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage1.Name = "ultraTabSharedControlsPage1";
            this.ultraTabSharedControlsPage1.Size = new System.Drawing.Size(897, 461);
            // 
            // _BaseView_Toolbars_Dock_Area_Left
            // 
            this._BaseView_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._BaseView_Toolbars_Dock_Area_Left.BackColor = System.Drawing.SystemColors.Control;
            this._BaseView_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            this._BaseView_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._BaseView_Toolbars_Dock_Area_Left.Location = new System.Drawing.Point(0, 54);
            this._BaseView_Toolbars_Dock_Area_Left.Name = "_BaseView_Toolbars_Dock_Area_Left";
            this._BaseView_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(0, 482);
            this._BaseView_Toolbars_Dock_Area_Left.ToolbarsManager = this.tbManager;
            // 
            // tbManager
            // 
            this.tbManager.DesignerFlags = 1;
            this.tbManager.DockWithinContainer = this;
            this.tbManager.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbManager.LockToolbars = true;
            this.tbManager.ShowFullMenusDelay = 500;
            ultraToolbar1.DockedColumn = 0;
            ultraToolbar1.DockedRow = 0;
            comboBoxTool1.InstanceProps.Width = 307;
            ultraToolbar1.NonInheritedTools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool8,
            buttonTool2,
            popupMenuTool2,
            comboBoxTool1,
            buttonTool3});
            ultraToolbar1.Text = "DataOperations";
            ultraToolbar2.DockedColumn = 0;
            ultraToolbar2.DockedRow = 1;
            ultraToolbar2.NonInheritedTools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool1});
            ultraToolbar2.Text = "ToolBarCalculate";
            this.tbManager.Toolbars.AddRange(new Infragistics.Win.UltraWinToolbars.UltraToolbar[] {
            ultraToolbar1,
            ultraToolbar2});
            appearance89.Image = ((object)(resources.GetObject("appearance89.Image")));
            buttonTool4.SharedPropsInternal.AppearancesSmall.Appearance = appearance89;
            buttonTool4.SharedPropsInternal.Caption = "Обновить данные";
            appearance90.Image = ((object)(resources.GetObject("appearance90.Image")));
            buttonTool5.SharedPropsInternal.AppearancesSmall.Appearance = appearance90;
            buttonTool5.SharedPropsInternal.Caption = "Сохранить данные";
            appearance91.Image = ((object)(resources.GetObject("appearance91.Image")));
            buttonTool6.SharedPropsInternal.AppearancesSmall.Appearance = appearance91;
            buttonTool6.SharedPropsInternal.Caption = "Удалить данные";
            comboBoxTool2.SharedPropsInternal.Caption = "Сохраненные расчеты";
            comboBoxTool2.SharedPropsInternal.DisplayStyle = Infragistics.Win.UltraWinToolbars.ToolDisplayStyle.ImageAndText;
            valueList1.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            comboBoxTool2.ValueList = valueList1;
            appearance93.Image = ((object)(resources.GetObject("appearance93.Image")));
            buttonTool7.SharedPropsInternal.AppearancesSmall.Appearance = appearance93;
            buttonTool7.SharedPropsInternal.Caption = "Новый расчет";
            appearance92.Image = ((object)(resources.GetObject("appearance92.Image")));
            buttonTool9.SharedPropsInternal.AppearancesSmall.Appearance = appearance92;
            buttonTool9.SharedPropsInternal.Caption = "Рассчитать";
            buttonTool9.SharedPropsInternal.DisplayStyle = Infragistics.Win.UltraWinToolbars.ToolDisplayStyle.ImageAndText;
            appearance94.Image = ((object)(resources.GetObject("appearance94.Image")));
            buttonTool10.SharedPropsInternal.AppearancesLarge.Appearance = appearance94;
            appearance95.Image = ((object)(resources.GetObject("appearance95.Image")));
            buttonTool10.SharedPropsInternal.AppearancesSmall.Appearance = appearance95;
            buttonTool10.SharedPropsInternal.Caption = "Создать отчет";
            popupMenuTool1.InstanceProps.DisplayStyle = Infragistics.Win.UltraWinToolbars.ToolDisplayStyle.ImageAndText;
            appearance99.Image = ((object)(resources.GetObject("appearance99.Image")));
            popupMenuTool1.SharedPropsInternal.AppearancesSmall.Appearance = appearance99;
            popupMenuTool1.SharedPropsInternal.Caption = "Отчеты";
            popupMenuTool1.SharedPropsInternal.DisplayStyle = Infragistics.Win.UltraWinToolbars.ToolDisplayStyle.ImageAndText;
            popupMenuTool1.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool14,
            buttonTool15,
            buttonTool16});
            appearance97.Image = ((object)(resources.GetObject("appearance97.Image")));
            buttonTool12.SharedPropsInternal.AppearancesSmall.Appearance = appearance97;
            buttonTool12.SharedPropsInternal.Caption = "Размещений займа";
            appearance98.Image = ((object)(resources.GetObject("appearance98.Image")));
            buttonTool13.SharedPropsInternal.AppearancesSmall.Appearance = appearance98;
            buttonTool13.SharedPropsInternal.Caption = "Размещений займа (подробно)";
            appearance86.Image = ((object)(resources.GetObject("appearance86.Image")));
            buttonTool11.SharedPropsInternal.AppearancesSmall.Appearance = appearance86;
            buttonTool11.SharedPropsInternal.Caption = "Сравнение займов";
            buttonTool11.SharedPropsInternal.DisplayStyle = Infragistics.Win.UltraWinToolbars.ToolDisplayStyle.ImageAndText;
            this.tbManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool4,
            buttonTool5,
            buttonTool6,
            comboBoxTool2,
            buttonTool7,
            buttonTool9,
            buttonTool10,
            popupMenuTool1,
            buttonTool12,
            buttonTool13,
            buttonTool11});
            // 
            // _BaseView_Toolbars_Dock_Area_Right
            // 
            this._BaseView_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._BaseView_Toolbars_Dock_Area_Right.BackColor = System.Drawing.SystemColors.Control;
            this._BaseView_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._BaseView_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._BaseView_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(899, 54);
            this._BaseView_Toolbars_Dock_Area_Right.Name = "_BaseView_Toolbars_Dock_Area_Right";
            this._BaseView_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(0, 482);
            this._BaseView_Toolbars_Dock_Area_Right.ToolbarsManager = this.tbManager;
            // 
            // _BaseView_Toolbars_Dock_Area_Top
            // 
            this._BaseView_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._BaseView_Toolbars_Dock_Area_Top.BackColor = System.Drawing.SystemColors.Control;
            this._BaseView_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._BaseView_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._BaseView_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._BaseView_Toolbars_Dock_Area_Top.Name = "_BaseView_Toolbars_Dock_Area_Top";
            this._BaseView_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(899, 54);
            this._BaseView_Toolbars_Dock_Area_Top.ToolbarsManager = this.tbManager;
            // 
            // _BaseView_Toolbars_Dock_Area_Bottom
            // 
            this._BaseView_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._BaseView_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.SystemColors.Control;
            this._BaseView_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._BaseView_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._BaseView_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 536);
            this._BaseView_Toolbars_Dock_Area_Bottom.Name = "_BaseView_Toolbars_Dock_Area_Bottom";
            this._BaseView_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(899, 0);
            this._BaseView_Toolbars_Dock_Area_Bottom.ToolbarsManager = this.tbManager;
            // 
            // CapitalOperationsView
            // 
            this.Controls.Add(this.ultraTabControl1);
            this.Controls.Add(this._BaseView_Toolbars_Dock_Area_Left);
            this.Controls.Add(this._BaseView_Toolbars_Dock_Area_Right);
            this.Controls.Add(this._BaseView_Toolbars_Dock_Area_Top);
            this.Controls.Add(this._BaseView_Toolbars_Dock_Area_Bottom);
            this.Name = "CapitalOperationsView";
            this.Size = new System.Drawing.Size(899, 536);
            this.ultraTabPageControl6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ug1)).EndInit();
            this.ultraTabPageControl4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ug2)).EndInit();
            this.ultraTabPageControl5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ug3)).EndInit();
            this.ultraTabPageControl1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.ultraPanel4.ClientArea.ResumeLayout(false);
            this.ultraPanel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ugNominal)).EndInit();
            this.ultraPanel1.ClientArea.ResumeLayout(false);
            this.ultraPanel1.ClientArea.PerformLayout();
            this.ultraPanel1.ResumeLayout(false);
            this.ultraPanel3.ClientArea.ResumeLayout(false);
            this.ultraPanel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ugCoupon)).EndInit();
            this.ultraPanel2.ClientArea.ResumeLayout(false);
            this.ultraPanel2.ClientArea.PerformLayout();
            this.ultraPanel2.ResumeLayout(false);
            this.ultraPanel5.ClientArea.ResumeLayout(false);
            this.ultraPanel5.ClientArea.PerformLayout();
            this.ultraPanel5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Date)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ne6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ne5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ne7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CouponsCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ne8)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ne4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ne3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ne2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ne1)).EndInit();
            this.ultraTabPageControl2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.ResumeLayout(false);
            this.ultraPanel7.ClientArea.ResumeLayout(false);
            this.ultraPanel7.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ugCapitalBond)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraTabControl2)).EndInit();
            this.ultraTabControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ultraValidator)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraTabControl1)).EndInit();
            this.ultraTabControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tbManager)).EndInit();
            this.ResumeLayout(false);

        }

        public Infragistics.Win.UltraWinToolbars.UltraToolbarsManager ToolbarsManager
        {
            get { return tbManager; }
        }

        public FinancialCalculationParams GetCalculationParams()
        {
            var calcParams = new FinancialCalculationParams();
            calcParams.Basis = Convert.ToInt32(comboBox2.SelectedItem);
            if (Date.Value != null && Date.Value != DBNull.Value)
                calcParams.CouponStartDate = Convert.ToDateTime(Date.Value);
            calcParams.CalculationDate = DateTime.Today;
            //calcParams.Name = tbName.Text;
            calcParams.Nominal = Convert.ToDecimal(ne7.Value);
            calcParams.CurrencyBorrow = Convert.ToInt32(ne8.Value);
            calcParams.YTM = Convert.ToDecimal(ne2.Value);
            calcParams.CalculatedValue = GetCalculatedValue();
            calcParams.CurrentPriceRur = Convert.ToDecimal(ne4.Value);
            calcParams.CurrentPricePercent = Convert.ToDecimal(ne3.Value);
            calcParams.TotalCount = Convert.ToInt64(ne6.Value);
            calcParams.TotalSum = Convert.ToDecimal(ne5.Value);
            calcParams.CouponR = Convert.ToDecimal(ne1.Value);
            calcParams.IsEndDateRepay = cbEndPeriodPayment.Checked;
            calcParams.IsConstRate = cbIsConstRate.Checked;
            calcParams.CouponsCount = Convert.ToInt32(CouponsCount.Value);
            List<NominalCost> nominalList = new List<NominalCost>();
            foreach (DataRow row in NominalTable.Rows)
            {
                NominalCost nominal = new NominalCost();
                nominal.Num = row.Field<int>("Num");
                nominal.NominalSum = row.Field<int>("Nominal");
                nominal.DayCount = row.Field<int>("DayCount");
                nominalList.Add(nominal);
            }
            calcParams.PaymentsNominalCost = nominalList.ToArray();
            List<Coupon> couponList = new List<Coupon>();
            foreach (DataRow row in CouponTable.Rows)
            {
                Coupon coupon = new Coupon();
                coupon.Num = row.Field<int>("Num");
                coupon.CouponRate = row.IsNull("CouponRate") ? 0 : row.Field<decimal>("CouponRate");
                coupon.Nominal = row.IsNull("Nominal") ? 0 : row.Field<decimal>("Nominal");
                coupon.DayCount = row.IsNull("DayCount") ? 0 : row.Field<int>("DayCount");
                couponList.Add(coupon);
            }
            calcParams.Coupons = couponList.ToArray();
            return calcParams;
        }

        private CalculatedValue GetCalculatedValue()
        {
            if (rb1.Checked)
                return CalculatedValue.CouponR;
            if (rb2.Checked)
                return CalculatedValue.YTM;
            if (rb3.Checked)
                return CalculatedValue.CurrPriceRur;
            return CalculatedValue.CurrPricePercent;
        }

        private void btnAddNominal_Click(object sender, EventArgs e)
        {
            DataRow newRow = NominalTable.NewRow();
            newRow["Num"] = NominalTable.Rows.Count + 1;
            newRow["Nominal"] = 0;
            newRow["DayCount"] = 0;
            NominalTable.Rows.Add(newRow);
        }

        private void btnDeleteNominal_Click(object sender, EventArgs e)
        {
            if (ugNominal.ActiveRow == null)
                return;
            int num = Convert.ToInt32(ugNominal.ActiveRow.Cells["Num"].Value);
            NominalTable.Select("Num = " + num)[0].Delete();
            NominalTable.AcceptChanges();
            for (int i = 0; i <= NominalTable.Rows.Count - 1; i++)
            {
                NominalTable.Rows[i]["Num"] = i + 1;
            }
        }

        private void btnAddCoupon_Click(object sender, EventArgs e)
        {
            DataRow newRow = CouponTable.NewRow();
            newRow["Num"] = CouponTable.Rows.Count + 1;
            newRow["CouponRate"] = 0;
            newRow["Nominal"] = 0;
            newRow["DayCount"] = 0;
            CouponTable.Rows.Add(newRow);
        }

        private void btnDeleteCoupon_Click(object sender, EventArgs e)
        {
            if (ugCoupon.ActiveRow == null)
                return;
            int num = Convert.ToInt32(ugCoupon.ActiveRow.Cells["Num"].Value);
            CouponTable.Select("Num = " + num)[0].Delete();
            CouponTable.AcceptChanges();
            for (int i = 0; i <= CouponTable.Rows.Count - 1; i++)
            {
                CouponTable.Rows[i]["Num"] = i + 1;
            }
        }

        private void rb1_CheckedChanged(object sender, EventArgs e)
        {
            ne1.ReadOnly = false;
            ne1.Appearance.ResetBackColor();
            ne2.ReadOnly = false;
            ne2.Appearance.ResetBackColor();
            ne3.ReadOnly = false;
            ne3.Appearance.ResetBackColor();
            ne4.ReadOnly = false;
            ne4.Appearance.ResetBackColor();
            RadioButton rb = sender as RadioButton;
            switch (rb.Name)
            {
                case "rb1":
                    ne1.Enabled = true;
                    ne1.ReadOnly = true;
                    //ne1.Appearance.BackColor = Color.Khaki;
                    break;
                case "rb2":
                    ne2.ReadOnly = true;
                    //ne2.Appearance.BackColor = Color.Khaki;
                    break;
                case "rb3":
                case "rbRubPrice":
                    ne3.ReadOnly = true;
                    //ne3.Appearance.BackColor = Color.Khaki;
                    ne4.ReadOnly = true;
                    //ne4.Appearance.BackColor = Color.Khaki;
                    break;
            }
        }

        private void rb5_CheckedChanged(object sender, EventArgs e)
        {
            ne5.ReadOnly = false;
            ne5.Appearance.ResetBackColor();
            ne6.ReadOnly = false;
            ne6.Appearance.ResetBackColor();
            RadioButton rb = sender as RadioButton;
            switch (rb.Name)
            {
                case "rb5":
                    ne6.ReadOnly = true;
                    //ne6.Appearance.BackColor = Color.Khaki;
                    break;
                case "rb6":
                    ne5.ReadOnly = true;
                    //ne5.Appearance.BackColor = Color.Khaki;
                    break;
            }
        }

        private void ugNominal_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            var grid = sender as UltraGrid;
            if (grid == null)
                return;

            foreach (UltraGridColumn column in e.Layout.Bands[0].Columns)
            {
                if (column.Key == "Num")
                {
                    column.CellActivation = Activation.NoEdit;
                }
                else
                    if (grid.Name == "ugCoupon" && (column.Key == "Nominal" || column.Key == "DayCount"))
                    {
                        column.CellActivation = Activation.NoEdit;
                    }
                else
                {
                    if (column.DataType == typeof(decimal))
                    {
                        column.CellMultiLine = DefaultableBoolean.False;
                        column.MaskDataMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.Raw;
                        column.MaskClipMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.Raw;
                        column.MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiterals;
                        column.CellAppearance.TextHAlign = HAlign.Right;
                        column.PadChar = '_';
                        if (column.Key == "CouponRate")
                            column.MaskInput = "nnn.nn";
                        else
                            column.MaskInput = "nnnn.nn";
                    }
                    if (column.DataType == typeof(int))
                    {
                        column.CellMultiLine = DefaultableBoolean.False;
                        column.MaskDataMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.Raw;
                        column.MaskClipMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.Raw;
                        column.MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiterals;
                        column.CellAppearance.TextHAlign = HAlign.Right;
                        column.PadChar = '_';
                        column.MaskInput = "nnnn";
                    }
                }
            }

            e.Layout.Bands[0].Columns[0].Width = 40;
            e.Layout.Bands[0].Columns[1].Width = 90;
            e.Layout.Bands[0].Columns[2].Width = 130;
            if (e.Layout.Bands[0].Columns.Count == 4)
                e.Layout.Bands[0].Columns[3].Width = 160;
        }

        internal void CalculateUnpayValue(FinancialCalculationParams calculationParams, int periodsCount, decimal couponRate)
        {
            int period = Convert.ToInt32(ne8.Value);
            int daysCount = 0;

            if (calculationParams.IsConstRate || calculationParams.Coupons.Length == 0)
            {
                CouponTable.Clear();
                for (int i = 0; i <= periodsCount - 1; i++)
                {
                    DataRow newCoupon = CouponTable.NewRow();
                    newCoupon["Num"] = CouponTable.Rows.Count + 1;
                    newCoupon["DayCount"] = i == periodsCount - 1
                                                ? period - daysCount
                                                : period/periodsCount;
                    daysCount += period/periodsCount;
                    newCoupon["CouponRate"] = calculationParams.CalculatedValue == CalculatedValue.CouponR ? 0 : couponRate;
                    newCoupon["Nominal"] = Convert.ToDecimal(ne7.Value);
                    CouponTable.Rows.Add(newCoupon);
                }
            }
            else
            {
                for (int i = 0; i <= periodsCount - 1; i++)
                {
                    DataRow newCoupon = CouponTable.Rows[i];
                    newCoupon["DayCount"] = i == periodsCount - 1
                                                ? period - daysCount
                                                : period / periodsCount;
                    daysCount += period / periodsCount;
                    newCoupon["Nominal"] = Convert.ToDecimal(ne7.Value);
                }
            }

            decimal nominal = Convert.ToDecimal(Convert.ToDecimal(ne7.Value));
            int t1 = 0;
            int nominalIndex = 1;
            foreach (DataRow couponRow in CouponTable.Rows)
            {
                t1 += Convert.ToInt32(couponRow["DayCount"]);
                foreach (DataRow nominalRow in NominalTable.Select(string.Format("Num = {0}", nominalIndex)))
                {
                    int t2 = Convert.ToInt32(nominalRow["DayCount"]);
                    if (t1 > t2)
                    {
                        nominal -= Convert.ToDecimal(nominalRow["Nominal"]);
                        nominalIndex++;
                    }
                }
                couponRow["Nominal"] = nominal;
            }
        }

        internal void CalculateCouponsNominal()
        {
            decimal nominal = Convert.ToDecimal(Convert.ToDecimal(ne7.Value));
            int t1 = 0;
            int nominalIndex = 1;
            foreach (DataRow couponRow in CouponTable.Rows)
            {
                t1 += Convert.ToInt32(couponRow["DayCount"]);
                foreach (DataRow nominalRow in NominalTable.Select(string.Format("Num = {0}", nominalIndex)))
                {
                    int t2 = Convert.ToInt32(nominalRow["DayCount"]);
                    if (t1 <= t2)
                        couponRow["Nominal"] = nominal;
                    if (t1 > t2)
                    {
                        nominal -= Convert.ToDecimal(nominalRow["Nominal"]);
                        couponRow["Nominal"] = nominal;
                        nominalIndex++;
                    }
                }
            }
        }

        internal void ClearNominalPays()
        {
            NominalTable.Clear();
        }

        internal void ClearCoupons()
        {
            CouponTable.Clear();
        }

        internal void SetCalculatedValue(decimal calcValue, FinancialCalculationParams calculationParam)
        {
            if (NominalTable.Rows.Count == 0)
            {
                cbEndPeriodPayment.Checked = false;
                cbEndPeriodPayment.Checked = true;
            }

            switch (calculationParam.CalculatedValue)
            {
                case CalculatedValue.YTM:
                    ne2.Value = calcValue;
                    break;
                case CalculatedValue.CurrPriceRur:
                    ne3.Value = Math.Round(calcValue * 100 / calculationParam.Nominal, 3,
                                   MidpointRounding.AwayFromZero);
                    calculationParam = GetCalculationParams();
                    ne4.Value = Math.Round(calculationParam.Nominal * calculationParam.CurrentPricePercent / 100, 2,
                                   MidpointRounding.AwayFromZero);
                    break;
                case CalculatedValue.CouponR:
                    ne1.Value = calcValue;
                    break;
            }

            calculationParam = GetCalculationParams();
            if (calculationParam.CalculatedValue == CalculatedValue.CouponR)
            {
                foreach (DataRow row in CouponTable.Rows)
                {
                    row["CouponRate"] = calculationParam.CouponR;
                }
            }
            else if (!calculationParam.IsConstRate)
            {
                ne1.Value = 0;
            }
            
            // Количество облигаций размещаемого займа
            if (rb5.Checked)
            {
                decimal value = Math.Truncate(calculationParam.TotalSum / (calculationParam.CurrentPriceRur));
                ne6.Value = value;
            }
            // Объем средств от размещения займа
            if (rb6.Checked)
            {
                decimal value = Math.Round(calculationParam.TotalCount * calculationParam.CurrentPriceRur, 2, MidpointRounding.AwayFromZero);
                ne5.Value = value;
            }
            IsCalculationChanged = true;
        }

        private void cbEndPeriodPayment_CheckedChanged(object sender, EventArgs e)
        {
            btnAddNominal.Enabled = !cbEndPeriodPayment.Checked;
            btnDeleteNominal.Enabled = !cbEndPeriodPayment.Checked;
            btnClearNominalPays.Enabled = !cbEndPeriodPayment.Checked;
            if (cbEndPeriodPayment.Checked)
            {
                if (ne8.Value == null || Convert.ToInt32(ne8.Value) == 0)
                {
                    MessageBox.Show(
                        "Не заполнен параметр 'Срок обращения'. Пожалуйста заполните этот параметр расчета",
                        "Параметры расчета", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                NominalTable.Clear();
                DataRow row = NominalTable.NewRow();
                row["Num"] = 1;
                row["DayCount"] = Convert.ToInt32(ne8.Value);
                row["Nominal"] = Convert.ToInt32(ne7.Value);
                NominalTable.Rows.Add(row);
            }
            else
            {
                NominalTable.Clear();
            }
        }

        internal void SetData(DataTable dtParams, DataTable dtCoupons, DataTable dtNominal)
        {
            CouponTable.Clear();
            NominalTable.Clear();

            ne7.Value = dtParams.Rows[0]["Nominal"];
            ne8.Value = dtParams.Rows[0]["CurrencyBorrow"];
            ne1.Value = dtParams.Rows[0]["CouponR"];
            ne2.Value = dtParams.Rows[0]["YTM"];
            ne4.Value = dtParams.Rows[0]["CurrPriceRub"];
            ne3.Value = dtParams.Rows[0]["CurrPrice"];
            ne5.Value = dtParams.Rows[0]["TotalSum"];
            ne6.Value = dtParams.Rows[0]["TotalCount"];
            Date.Value = dtParams.Rows[0]["StartCpnDate"];
            cbEndPeriodPayment.Checked = Convert.ToBoolean(dtParams.Rows[0]["IsEndDateRepay"]);
            cbIsConstRate.Checked = Convert.ToBoolean(dtParams.Rows[0]["IsConstRate"]);
            CouponsCount.Value = dtParams.Rows[0].IsNull("CouponsCount")
                               ? dtCoupons.Rows.Count
                               : Convert.ToInt32(dtParams.Rows[0]["CouponsCount"]);
            int basis = Convert.ToInt32(dtParams.Rows[0]["Basis"]);
            switch (basis)
            {
                case 365:
                    comboBox2.SelectedIndex = 1;
                    break;
                case 366:
                    comboBox2.SelectedIndex = 2;
                    break;
                case 360:
                    comboBox2.SelectedIndex = 0;
                    break;
            }

            foreach (DataRow row in dtCoupons.Rows)
            {
                DataRow coupon = CouponTable.NewRow();
                coupon["Num"] = row["Num"];
                coupon["DayCount"] = row["DayCpnCount"];
                coupon["CouponRate"] = row["CouponRate"];
                coupon["Nominal"] = row["Nomi"];
                CouponTable.Rows.Add(coupon);
            }
            CouponTable.AcceptChanges();
            ugCoupon.DataSource = null;
            ugCoupon.DataSource = CouponTable;
            NominalTable.Clear();
            foreach (DataRow row in dtNominal.Rows)
            {
                DataRow nominal = NominalTable.NewRow();
                nominal["Num"] = row["Num"];
                nominal["DayCount"] = row["DayCount"];
                nominal["Nominal"] = row["NomSum"];
                NominalTable.Rows.Add(nominal);
            }
            NominalTable.AcceptChanges();
        }

        internal void SetBondData(DataTable dtCapitalBond, DataTable dtNominalBond, DataTable dtPaymentBond)
        {
            CapitalBondTable = dtCapitalBond.Copy();
            NominalBondTable = dtNominalBond.Copy();
            PaymentBondTable = dtPaymentBond.Copy();
            ugCapitalBond.DataSource = CapitalBondTable;
            ug1.DataSource = NominalBondTable;
            //if (PaymentBondTable.Select("StartDate is null").Length == 0)
            {
                ug2.DataSource = null;
                ug2.DataSource = PaymentBondTable;
            }
            ug3.DataSource = null;
            if (Date.Value != null && Date.Value != DBNull.Value)
                ug3.DataSource = PaymentBondTable;
        }

        internal void SetEmptyData()
        {
            Date.Value = DateTime.Today;
            rb5.Checked = true;
            comboBox2.SelectedIndex = 1;
            ne1.Value = 0;
            ne2.Value = 0;
            ne3.Value = 0;
            ne4.Value = 0;
            ne5.Value = 0;
            ne6.Value = 0;
            ne7.Value = 1000;
            ne8.Value = 0;
            CouponsCount.Value = 0;
            cbIsConstRate.Checked = true;
            cbEndPeriodPayment.Checked = false; 
            rb1.Checked = true;

            CouponTable.Clear();
            NominalTable.Clear();
            CapitalBondTable.Clear();
            NominalBondTable.Clear();
            PaymentBondTable.Clear();
            PaymentBondTable.Clear();
        }

        internal void GetData(FinancialCalculationParams calcParams, ref DataTable dtParams, ref DataTable dtCoupons, ref DataTable dtNominals,
            ref DataTable dtCapitalBond, ref DataTable dtNominalBond, ref DataTable dtPaymentBond)
        {
            dtParams.Clear();
            dtCoupons.Clear();
            dtNominals.Clear();
            dtPaymentBond.Clear();
            dtNominalBond.Clear();
            dtCapitalBond.Clear();
            DataRow newRow = dtParams.NewRow();
            newRow["SourceID"] = FinSourcePlanningNavigation.Instance.CurrentSourceID;
            newRow["TaskID"] = -1;
            newRow["CalcDate"] = calcParams.CalculationDate;
            newRow["Name"] = calcParams.Name;
            newRow["Basis"] = calcParams.Basis;
            newRow["StartCpnDate"] = calcParams.CouponStartDate.Year < 1980 ? DBNull.Value : (object)calcParams.CouponStartDate;
            newRow["CouponR"] = calcParams.CouponR;
            newRow["YTM"] = calcParams.YTM;
            newRow["CurrPriceRub"] = calcParams.CurrentPriceRur;
            newRow["CurrPrice"] = calcParams.CurrentPricePercent;
            newRow["TotalCount"] = calcParams.TotalCount;
            newRow["TotalSum"] = calcParams.TotalSum;
            newRow["Nominal"] = calcParams.Nominal;
            newRow["CurrencyBorrow"] = calcParams.CurrencyBorrow;
            newRow["IsEndDateRepay"] = calcParams.IsEndDateRepay;
            newRow["IsConstRate"] = calcParams.IsConstRate;
            newRow["CouponsCount"] = calcParams.CouponsCount;
            dtParams.Rows.Add(newRow);
            foreach (Coupon coupon in calcParams.Coupons)
            {
                newRow = dtCoupons.NewRow();
                newRow["Num"] = coupon.Num;
                newRow["DayCpnCount"] = coupon.DayCount;
                newRow["CouponRate"] = coupon.CouponRate;
                newRow["Nomi"] = coupon.Nominal;
                dtCoupons.Rows.Add(newRow);
            }
            foreach (NominalCost nominal in calcParams.PaymentsNominalCost)
            {
                newRow = dtNominals.NewRow();
                newRow["Num"] = nominal.Num;
                newRow["DayCount"] = nominal.DayCount;
                newRow["NomSum"] = nominal.NominalSum;
                dtNominals.Rows.Add(newRow);
            }

            foreach (DataRow row in CapitalBondTable.Rows)
            {
                dtCapitalBond.Rows.Add(row.ItemArray);
            }
            foreach (DataRow row in PaymentBondTable.Rows)
            {
                dtPaymentBond.Rows.Add(row.ItemArray);
            }
            foreach (DataRow row in NominalBondTable.Rows)
            {
                dtNominalBond.Rows.Add(row.ItemArray);
            }
        }

        internal void GetBondData(ref DataTable dtCapitalBond, ref DataTable dtNominalBond, ref DataTable dtPaymentBond)
        {
            dtPaymentBond.Clear();
            dtNominalBond.Clear();
            dtCapitalBond.Clear();
            foreach (DataRow row in CapitalBondTable.Rows)
            {
                dtCapitalBond.Rows.Add(row.ItemArray);
            }
            foreach (DataRow row in PaymentBondTable.Rows)
            {
                dtPaymentBond.Rows.Add(row.ItemArray);
            }
            foreach (DataRow row in NominalBondTable.Rows)
            {
                dtNominalBond.Rows.Add(row.ItemArray);
            }
        }

        private void ne7_ValueChanged(object sender, EventArgs e)
        {
            UltraNumericEditor editor = (UltraNumericEditor)sender;
            if (editor.Value == null || editor.Value == DBNull.Value)
                editor.Value = 0;
        }

        private void ne3_ValueChanged(object sender, EventArgs e)
        {
            UltraNumericEditor editor = (UltraNumericEditor) sender;
            if (editor.ReadOnly)
                return;
            if (editor.Value == null || editor.Value == DBNull.Value)
                editor.Value = 0;

            FinancialCalculationParams calculationParam = GetCalculationParams();
            ne4.ReadOnly = true;
            ne4.Value = Math.Round(calculationParam.Nominal*calculationParam.CurrentPricePercent / 100, 2,
                                   MidpointRounding.AwayFromZero);
            ne4.ReadOnly = false;
        }

        private void ne4_ValueChanged(object sender, EventArgs e)
        {
            UltraNumericEditor editor = (UltraNumericEditor)sender;
            if (editor.ReadOnly)
                return;
            if (editor.Value == null || editor.Value == DBNull.Value)
                editor.Value = 0;

            FinancialCalculationParams calculationParam = GetCalculationParams();
            if (calculationParam.Nominal == 0)
                return;
            ne3.ReadOnly = true;
            ne3.Value = Math.Round((calculationParam.CurrentPriceRur * 100) / calculationParam.Nominal , 4,
                                   MidpointRounding.AwayFromZero);
            ne3.ReadOnly = false;
        }
    }
}
