using System;
using System.Data;
using System.Drawing;
using Infragistics.Win.UltraWinExplorerBar;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Balance;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.BorrowingVolume;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Calculations.DebtLimit;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Capital;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.CapitalOperations.LoanToCredit;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.CapitalOperations.Redemption;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.CapitalPlanningOperations;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Guarantee.GuaranteeIssued;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server;
using Krista.FM.Common.Services;
using Krista.FM.Client.Common;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Commands;
using Krista.FM.Client.Workplace.Gui;
using Krista.FM.Client.Workplace.Services;

using Krista.FM.Common;
using Krista.FM.ServerLibrary;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Calculations.Remains;
using Krista.FM.Client.Reports;


namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI
{
    public class FinSourcePlanningNavigation : BaseNavigationCtrl
    {
        private static FinSourcePlanningNavigation instance;

        internal static FinSourcePlanningNavigation Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new FinSourcePlanningNavigation();
                }
                return instance;
            }
        }

        public override Image TypeImage16
        {
            get { return Resources.ru.FinSources_16; }
        }

        public override Image TypeImage24
        {
            get { return Resources.ru.FinSources_24; }
        }

        public FinSourcePlanningNavigation()
		{
            ResourceService.RegisterStrings("Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Resources", typeof(FinSourcePlanningNavigation).Assembly);
            ResourceService.RegisterImages("Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Resources", typeof(FinSourcePlanningNavigation).Assembly);

            instance = this;
            Caption = "${res:FinSourcePlanningUICaption}";
		}

        private Infragistics.Win.UltraWinToolbars.UltraToolbarsManager ultraToolbarsManager;
        private System.Windows.Forms.Panel BaseNavigationCtrl_Fill_Panel;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseNavigationCtrl_Toolbars_Dock_Area_Left;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseNavigationCtrl_Toolbars_Dock_Area_Right;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseNavigationCtrl_Toolbars_Dock_Area_Top;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseNavigationCtrl_Toolbars_Dock_Area_Bottom;
        private System.Windows.Forms.ImageList il;

        public static int BaseYear = DateTime.Today.Year + 1;

        public static void SetCurrentYear(int year)
        {
            BaseYear = year;
        }

        private int currentVariantID = -1;
        private string currentVariantCaption = string.Empty;
        private int currentSourceID = -1;

        internal UltraExplorerBar ultraExplorerBar;
        private Infragistics.Win.Misc.UltraButton btnSelectVariant;
        private Infragistics.Win.Misc.UltraLabel lblVariantComment;
        private Infragistics.Win.Misc.UltraLabel lblVariantYears;
        private Infragistics.Win.Misc.UltraLabel lblVariantCodeName;
        private System.ComponentModel.IContainer components;

        /// <summary>
        /// Инициализация
        /// </summary>
        public override void Initialize()
        {
            InitializeComponent();

            ultraExplorerBar.ItemCheckStateChanged += ultraExplorerBar_ItemCheckStateChanged;
            ultraExplorerBar.ItemCheckStateChanging += ultraExplorerBar_ItemCheckStateChanging;

            Workplace.ViewClosed += Workplace_ViewClosed;
            Workplace.ActiveWorkplaceWindowChanged += Workplace_ActiveWorkplaceWindowChanged;

            base.Initialize();

            AddSchemeObject2ExplorerBarItem(SchemeObjectsKeys.d_Variant_Borrow_Key);
            AddSchemeObject2ExplorerBarItem(SchemeObjectsKeys.d_S_JournalCB_Key, "Журнал ставок ЦБ");
            AddSchemeObject2ExplorerBarItem(SchemeObjectsKeys.d_S_RateValue, "Курсы валют");
            AddSchemeObject2ExplorerBarItem(SchemeObjectsKeys.d_S_Constant_Key, "Константы для ИФ");
            AddSchemeObject2ExplorerBarItem(SchemeObjectsKeys.d_Marks_EstimatesData_Key);
            AddSchemeObject2ExplorerBarItem(SchemeObjectsKeys.d_Marks_Estimates_Key);
            AddSchemeObject2ExplorerBarItem(SchemeObjectsKeys.d_S_KindBorrow_Key, "Виды заимствований");
            AddSchemeObject2ExplorerBarItem(SchemeObjectsKeys.d_S_Extensions, "Вид ссуды");
            AddSchemeObject2ExplorerBarItem(SchemeObjectsKeys.d_S_CollateralObjects_Key, "Объекты недвижимого имущества");
            AddSchemeObject2ExplorerBarItem(SchemeObjectsKeys.d_Fact_Holidays, "Перечень праздников");
            AddSchemeObject2ExplorerBarItem(SchemeObjectsKeys.d_Readings_JobTitleSignature, "Должности");

            il.Images.Add(Resources.ru.excelDocument);
            il.Images.Add(Resources.ru.excelTemplate);
            il.Images.Add(Resources.ru.wordDocument);
            il.Images.Add(Resources.ru.wordlTemplate);
            il.Images.Add(Resources.ru.transfert);

            CommandService.AttachCommandToControl(new SelectVariantCommand(), btnSelectVariant);

            UltraToolbar toolBar = ultraToolbarsManager.Toolbars[0];

            ButtonTool btnCreateReport = CommandService.AttachToolbarTool(new BudgetTransfertCommand(), toolBar);
            btnCreateReport.SharedProps.AppearancesSmall.Appearance.Image = il.Images[5];

            ReportMenuParams reportMenuParams = new ReportMenuParams();
            reportMenuParams.il = il;
            reportMenuParams.tb = toolBar;
            reportMenuParams.tbManager = ultraToolbarsManager;
            
            IFReportMenu reportMenu = new IFReportMenu(reportMenuParams);
            reportMenu.scheme = Workplace.ActiveScheme;
            reportMenu.window = Workplace.WindowHandle;
            reportMenu.operationObj = Workplace.OperationObj;
            reportMenu.IFMainReportMenuList();

            SetDefaultVariant();

            SetFinSourcesPermissions();
        }

        private void SetDefaultVariant()
        {
            IEntity entity = Workplace.ActiveScheme.RootPackage.FindEntityByName(SchemeObjectsKeys.d_Variant_Borrow_Key);
            using (IDataUpdater du = entity.GetDataUpdater())
            {
                DataSet dsVariant = new DataSet();
                du.Fill(ref dsVariant);
                foreach (DataRow variantRow in dsVariant.Tables[0].Rows)
                {
                    if (Convert.ToBoolean(variantRow["CurrentVariant"]))
                    {
                        Instance.SetCurrentVariant(Convert.ToInt32(variantRow["ID"]));
                        break;
                    }
                }
                if (CurrentVariantID == -1)
                    Instance.SetCurrentVariant(0);
            }
        }

		/// <summary>
		/// Добавляет объект в навигационную область.
		/// </summary>
		private void AddSchemeObject2ExplorerBarItem(string key)
		{
			AddSchemeObject2ExplorerBarItem(key, String.Empty);
		}

		/// <summary>
		/// Добавляет объект в навигационную область.
		/// </summary>
		private void AddSchemeObject2ExplorerBarItem(string key, string caption)
		{
            if (!key.Contains("_if"))
                key = string.Concat(key, "_if");
			IEntity entity = Workplace.ActiveScheme.RootPackage.FindEntityByName(key.Split('_')[0]);
			if (entity != null)
			{
				UltraExplorerBarItem ultraExplorerBarItem =
					new UltraExplorerBarItem();
				ultraExplorerBarItem.Key = key;
				ultraExplorerBarItem.Settings.AppearancesSmall.Appearance = new Infragistics.Win.Appearance();
				ultraExplorerBarItem.Text = String.IsNullOrEmpty(caption) ? entity.FullCaption : caption;
				ultraExplorerBar.Groups[7].Items.Add(ultraExplorerBarItem);
			}
		}

        /// <summary>
        /// Текущий выбранный вариант.
        /// </summary>
        public int CurrentVariantID
        {
            get { return currentVariantID; }
        }

        public string CurrentVariantCaption
        {
            get { return currentVariantCaption; }
        }

        public int CurrentVariantYear
        {
            get { return BaseYear; }
        }

        /// <summary>
        /// Источник данных
        /// </summary>
        public int CurrentSourceID
        {
            get { return currentSourceID; }
            private set { currentSourceID = value; }
        }

        private void OnVariantChanged()
        {
            if (VariantChanged != null)
            {
                VariantChanged(this, EventArgs.Empty);
            }

            if (VariantChangedNew != null)
            {
                VariantChangeEventHandler e = new VariantChangeEventHandler(CurrentVariantID, CurrentSourceID, CurrentVariantCaption);
                VariantChangedNew(this, e);
            }
        }

        internal void SetCurrentVariant(int refVariant)
        {
            currentVariantID = refVariant;
            using (IDatabase db = Instance.Workplace.ActiveScheme.SchemeDWH.DB)
            {
                IEntity entity = Instance.Workplace.ActiveScheme.RootPackage.FindEntityByName(SchemeObjectsKeys.d_Variant_Borrow_Key);
                DataTable dt = (DataTable) db.ExecQuery(String.Format(
                    "select Code, Name, CurrentYear, PlanPeriod, VariantComment from {0} where ID = ?", entity.FullDBName),
                    QueryResultTypes.DataTable,
                    new System.Data.OleDb.OleDbParameter("ID", CurrentVariantID));

                DataRow row = dt.Rows[0];
                currentVariantCaption = String.Format("{0}", row[1]);
                lblVariantCodeName.Text = currentVariantCaption;
                lblVariantYears.Text = String.Format("{0} - {1}", row[2], row[3]);
                lblVariantComment.Text = String.Format("{0}", row[4]);
            }
            GetDataSourceIDByVariantID(currentVariantID);
            OnVariantChanged();
            ultraExplorerBar.Groups[0].Items[0].Visible = currentVariantID > -2;
            ultraToolbarsManager.Tools["CalculateBorrowingValume"].SharedProps.Enabled = (CurrentVariantID != -2);
        }

        private BaseViewObj FinSourcePlanningUIFactory(string objectKey)
        {
            string newKey = objectKey.Split('_')[0];
            BaseViewObj viewObj = null;
            switch (newKey)
            {
                case SchemeObjectsKeys.f_S_Guarantincome_Key:
                    viewObj = new FinSourcePlanningUI(Workplace.ActiveScheme.FinSourcePlanningFace.GuarantIncomeService);
                    break;
                case SchemeObjectsKeys.PlanningOperationsUI:
                    viewObj = new CapitalOperationsUI(newKey);
                    break;
                case SchemeObjectsKeys.LoanToCreditUI:
                    viewObj = new LoanToCreditUI(newKey);
                    break;
                case SchemeObjectsKeys.RedemptionUI:
                    viewObj = new RedemptionUI(newKey);
                    break;
                case SchemeObjectsKeys.f_S_Сreditincome_Key:
                    string creditType = objectKey.Split('_')[1];
                    switch (creditType)
                    {
                        case "Bud":
                            viewObj = new Gui.Credits.CreditIncomes.BudgetCredit(Workplace.ActiveScheme.FinSourcePlanningFace.СreditIncomeService, objectKey);
                            break;
                        case "Org":
                            viewObj = new Gui.Credits.CreditIncomes.OrganizationCredit(Workplace.ActiveScheme.FinSourcePlanningFace.СreditIncomeService, objectKey);
                            break;
                    }
                    break;
                case SchemeObjectsKeys.f_S_Creditissued_Key:
                    creditType = objectKey.Split('_')[1];
                    switch (creditType)
                    {
                        case "Bud":
                            viewObj = new Gui.Credits.CreditIssued.BudgetCredit(Workplace.ActiveScheme.FinSourcePlanningFace.СreditIssuedService, objectKey);
                            break;
                        case "Org":
                            viewObj = new Gui.Credits.CreditIssued.OrganizationCredit(Workplace.ActiveScheme.FinSourcePlanningFace.СreditIssuedService, objectKey);
                            break;
                    }
                    break;
                case SchemeObjectsKeys.f_S_Capital_Key:
                    viewObj = new CapitalUI(Workplace.ActiveScheme.FinSourcePlanningFace.CapitalService, objectKey);
                    break;
                case SchemeObjectsKeys.f_S_Guarantissued_Key:
                    viewObj = new GuaranteeIssuedUI(Workplace.ActiveScheme.FinSourcePlanningFace.GuarantIssuedService, objectKey);
                    break;
                case SchemeObjectsKeys.d_S_JournalCB_Key:
                    {
                        IEntity entity = Workplace.ActiveScheme.RootPackage.FindEntityByName(newKey);
                        viewObj = new JournalCBDataClsUI(entity);
                    }
                    break;
                case SchemeObjectsKeys.d_S_RateValue:
                    {
                        IEntity entity = Workplace.ActiveScheme.RootPackage.FindEntityByName(newKey);
                        viewObj = new RateValueDataClsUI(entity, -1);
                    }
                    break;
                case SchemeObjectsKeys.f_S_RemainsDesign:
                    {
                        IEntity entity = Workplace.ActiveScheme.RootPackage.FindEntityByName(newKey);
                        viewObj = new RemainsUI(entity);
                    }
                    break;
                case SchemeObjectsKeys.d_Variant_Borrow_Key:
                    {
                        IEntity entity = Workplace.ActiveScheme.RootPackage.FindEntityByName(newKey);
                        viewObj = new FinSourcePlaningVariantUI(entity);
                    }
                    break;
                case SchemeObjectsKeys.f_S_VolumeHoldings_Key:
                    {
                        IEntity entity = Workplace.ActiveScheme.RootPackage.FindEntityByName(newKey);
                        viewObj = new BorrowingVolumeUI(entity);
                    }
                    break;
                case SchemeObjectsKeys.d_S_Constant_Key:
                    {
                        IEntity entity = Workplace.ActiveScheme.RootPackage.FindEntityByName(newKey);
                        viewObj = new FinSourcePlaningConstsUI(entity);
                    }
                    break;
                case SchemeObjectsKeys.d_Marks_EstimatesData_Key:
                case SchemeObjectsKeys.d_Marks_Estimates_Key:
                case SchemeObjectsKeys.d_S_KindBorrow_Key:
                case SchemeObjectsKeys.d_S_CollateralObjects_Key:
                case SchemeObjectsKeys.d_S_Extensions:
                case SchemeObjectsKeys.d_Readings_JobTitleSignature:
                    {
                        IEntity entity = Workplace.ActiveScheme.RootPackage.FindEntityByName(newKey);
                        viewObj = new ReferenceUI(entity);
                    }
                    break;
                case SchemeObjectsKeys.f_S_Plan_Key:
                    {
                        IEntity entity = Workplace.ActiveScheme.RootPackage.FindEntityByName(newKey);
                        viewObj = new IFResultIU(entity);
                    }
                    break;
                case SchemeObjectsKeys.BKKUIndicatorsUI_Key:
                    viewObj = new BKKUIndicatorsUI();
                    break;
                case SchemeObjectsKeys.DDEIndicatorsUI_Key:
                    viewObj = new DDEIndicatorsUI(newKey);
                    break;
                case SchemeObjectsKeys.expenseValuation_Key:
                    viewObj = new ExpenseValuationUI();
                    break;
                case SchemeObjectsKeys.d_Fact_Holidays:
                    {
                        IEntity entity = Workplace.ActiveScheme.RootPackage.FindEntityByName(newKey);
                        viewObj = new HolydaysUI(entity);
                    }
                    break;
                case SchemeObjectsKeys.f_S_DebtLimit:
                    {
                        IEntity entity = Workplace.ActiveScheme.RootPackage.FindEntityByName(newKey);
                        viewObj = new DebtLimitUI(entity);
                    }
                    break;
                default:
                    throw new ArgumentException("objectKey", newKey);
            }

            OnVariantChanged();
            return viewObj;
        }

		/// <summary>
		/// Предварение выбора элемента в навигационной области.
		/// </summary>
		private void ultraExplorerBar_ItemCheckStateChanging(object sender, Infragistics.Win.UltraWinExplorerBar.CancelableItemEventArgs e)
		{
			// Если вариант не выбран, то принудительно заставляем пользователя выбрать вариант.
		    string key = e.Item.Key.Split('_')[0];
			if (currentVariantID == -1 && (
                key == SchemeObjectsKeys.f_S_Сreditincome_Key ||
                key == SchemeObjectsKeys.d_S_JournalCB_Key ||
                key == SchemeObjectsKeys.BKKUIndicatorsUI_Key ||
                key == SchemeObjectsKeys.DDEIndicatorsUI_Key ||
                key == SchemeObjectsKeys.f_S_Creditissued_Key ||
                key == SchemeObjectsKeys.f_S_Guarantissued_Key ||
                key == SchemeObjectsKeys.f_S_RemainsDesign ||
                key == SchemeObjectsKeys.f_S_Capital_Key))
			{
				new SelectVariantCommand().Run();
				if (currentVariantID == -1)
				{
					e.Cancel = true;
				}
			}
		}

		/// <summary>
		/// Выбор элемента в навигационной области.
		/// </summary>
        private void ultraExplorerBar_ItemCheckStateChanged(object sender, Infragistics.Win.UltraWinExplorerBar.ItemEventArgs e)
        {
            if (e.Item.Checked)
            {
                IViewContent vc = WorkplaceSingleton.Workplace.GetOpenedContent(e.Item.Key);
                if (vc != null)
                {
                    vc.WorkplaceWindow.SelectWindow();
                }
                else
                {
                    BaseViewObj viewObject = FinSourcePlanningUIFactory(e.Item.Key);
                    viewObject.Workplace = Workplace;
                    viewObject.Initialize();
                    viewObject.ViewCtrl.Text = string.Concat("ИФ_", e.Item.Text);
                    OnActiveItemChanged(this, viewObject);
                    viewObject.InitializeData();
                }
            }
        }

        private void Workplace_ActiveWorkplaceWindowChanged(object sender, EventArgs e)
        {
            if (Workplace.WorkplaceLayout.ActiveContent != null)
            {
                string key = ((BaseViewObj)Workplace.WorkplaceLayout.ActiveContent).Key;
                UltraExplorerBarGroup group = null;
                foreach (UltraExplorerBarGroup groupBar in ultraExplorerBar.Groups)
                {
                    if (groupBar.Items.Exists(key))
                    {
                        group = groupBar;
                        group.Expanded = true;
                        break;
                    }
                }

                if (group == null)
                    return;
                Workplace.SwitchTo("Источники финансирования");
                if (ultraExplorerBar.CheckedItem == null)
                    return;
                if (key != ultraExplorerBar.CheckedItem.Key && group.Items[key].Visible)
                {
                    group.Items[key].Checked = true;
                    group.Items[key].Active = true;
                }
            }
        }

        private void Workplace_ViewClosed(object sender, ViewContentEventArgs e)
        {
            if (ultraExplorerBar.CheckedItem == null)
                return;

            if (e.Content.Key == ultraExplorerBar.CheckedItem.Key)
            {
                ultraExplorerBar.CheckedItem.Active = false;
                ultraExplorerBar.CheckedItem.Checked = false;
            }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.Panel pnlVariant;
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup ultraExplorerBarGroup1 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem1 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup ultraExplorerBarGroup2 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem2 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup ultraExplorerBarGroup3 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem3 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem4 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup ultraExplorerBarGroup4 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem5 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem6 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup ultraExplorerBarGroup5 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem7 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem8 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup ultraExplorerBarGroup8 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem15 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem17 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem16 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup ultraExplorerBarGroup6 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem9 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem10 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem11 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem12 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem13 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem14 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup ultraExplorerBarGroup7 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup();
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.UltraToolbar ultraToolbar1 = new Infragistics.Win.UltraWinToolbars.UltraToolbar("utbReports");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool1 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("pmReports");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool2 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("pmReports");
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnFactDebts");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool4 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnExpireDebts");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FinSourcePlanningNavigation));
            this.lblVariantComment = new Infragistics.Win.Misc.UltraLabel();
            this.lblVariantYears = new Infragistics.Win.Misc.UltraLabel();
            this.lblVariantCodeName = new Infragistics.Win.Misc.UltraLabel();
            this.btnSelectVariant = new Infragistics.Win.Misc.UltraButton();
            this.ultraExplorerBar = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBar();
            this.BaseNavigationCtrl_Fill_Panel = new System.Windows.Forms.Panel();
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.ultraToolbarsManager = new Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(this.components);
            this.il = new System.Windows.Forms.ImageList(this.components);
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            pnlVariant = new System.Windows.Forms.Panel();
            pnlVariant.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraExplorerBar)).BeginInit();
            this.BaseNavigationCtrl_Fill_Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraToolbarsManager)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlVariant
            // 
            pnlVariant.BackColor = System.Drawing.SystemColors.Window;
            pnlVariant.Controls.Add(this.lblVariantComment);
            pnlVariant.Controls.Add(this.lblVariantYears);
            pnlVariant.Controls.Add(this.lblVariantCodeName);
            pnlVariant.Controls.Add(this.btnSelectVariant);
            pnlVariant.Dock = System.Windows.Forms.DockStyle.Bottom;
            pnlVariant.Location = new System.Drawing.Point(0, 338);
            pnlVariant.Name = "pnlVariant";
            pnlVariant.Size = new System.Drawing.Size(231, 111);
            pnlVariant.TabIndex = 1;
            // 
            // lblVariantComment
            // 
            this.lblVariantComment.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblVariantComment.Location = new System.Drawing.Point(3, 68);
            this.lblVariantComment.Name = "lblVariantComment";
            this.lblVariantComment.Size = new System.Drawing.Size(225, 40);
            this.lblVariantComment.TabIndex = 4;
            // 
            // lblVariantYears
            // 
            this.lblVariantYears.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblVariantYears.Location = new System.Drawing.Point(3, 50);
            this.lblVariantYears.Name = "lblVariantYears";
            this.lblVariantYears.Size = new System.Drawing.Size(225, 14);
            this.lblVariantYears.TabIndex = 2;
            // 
            // lblVariantCodeName
            // 
            this.lblVariantCodeName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblVariantCodeName.Location = new System.Drawing.Point(3, 32);
            this.lblVariantCodeName.Name = "lblVariantCodeName";
            this.lblVariantCodeName.Size = new System.Drawing.Size(225, 14);
            this.lblVariantCodeName.TabIndex = 1;
            // 
            // btnSelectVariant
            // 
            this.btnSelectVariant.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            appearance2.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnSelectVariant.Appearance = appearance2;
            this.btnSelectVariant.Location = new System.Drawing.Point(3, 3);
            this.btnSelectVariant.Name = "btnSelectVariant";
            this.btnSelectVariant.Size = new System.Drawing.Size(225, 23);
            this.btnSelectVariant.TabIndex = 0;
            this.btnSelectVariant.Text = "Выбрать вариант";
            // 
            // ultraExplorerBar
            // 
            this.ultraExplorerBar.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraExplorerBar.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            ultraExplorerBarItem1.Key = "27a49ecd-e9e7-49bc-bec7-c3fd83a10522_if";
            ultraExplorerBarItem1.Text = "Средства от продажи акций";
            ultraExplorerBarGroup1.Items.AddRange(new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem[] {
            ultraExplorerBarItem1});
            ultraExplorerBarGroup1.Settings.HeaderVisible = Infragistics.Win.DefaultableBoolean.False;
            ultraExplorerBarGroup1.Text = "Средства от продажи акций";
            ultraExplorerBarItem2.Key = "799c95c4-1816-45dc-8faf-1326767c0a98_if";
            ultraExplorerBarItem2.Text = "Ценные бумаги";
            ultraExplorerBarGroup2.Items.AddRange(new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem[] {
            ultraExplorerBarItem2});
            ultraExplorerBarGroup2.Settings.HeaderVisible = Infragistics.Win.DefaultableBoolean.False;
            ultraExplorerBarGroup2.Text = "Ценные бумаги";
            ultraExplorerBarItem3.Key = "042556fd-89a9-4b44-bc3e-2e645560a6bf_if";
            ultraExplorerBarItem3.Text = "Гарантии";
            ultraExplorerBarItem4.Key = "8085e515-d224-4725-ada9-855d9d83bb8c";
            ultraExplorerBarItem4.Text = "Гарантии полученные";
            ultraExplorerBarItem4.Visible = false;
            ultraExplorerBarGroup3.Items.AddRange(new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem[] {
            ultraExplorerBarItem3,
            ultraExplorerBarItem4});
            ultraExplorerBarGroup3.Settings.HeaderVisible = Infragistics.Win.DefaultableBoolean.False;
            ultraExplorerBarGroup3.Text = "Гарантии";
            ultraExplorerBarItem5.Key = "d3a9668b-0a65-4a6a-bca6-090768c822d0_Org_if";
            ultraExplorerBarItem5.Text = "Кредиты от кредитных организаций";
            ultraExplorerBarItem6.Key = "d3a9668b-0a65-4a6a-bca6-090768c822d0_Bud_if";
            ultraExplorerBarItem6.Text = "Бюджетные кредиты";
            ultraExplorerBarGroup4.Items.AddRange(new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem[] {
            ultraExplorerBarItem5,
            ultraExplorerBarItem6});
            ultraExplorerBarGroup4.Text = "Кредиты полученные";
            ultraExplorerBarItem7.Key = "fb029d1d-e648-46b4-8a1f-bff21ea0fbf5_Bud_if";
            ultraExplorerBarItem7.Text = "Бюджетные кредиты другим бюджетам";
            ultraExplorerBarItem8.Key = "fb029d1d-e648-46b4-8a1f-bff21ea0fbf5_Org_if";
            ultraExplorerBarItem8.Text = "Бюджетные кредиты юридическим лицам";
            ultraExplorerBarGroup5.Items.AddRange(new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem[] {
            ultraExplorerBarItem7,
            ultraExplorerBarItem8});
            ultraExplorerBarGroup5.Text = "Кредиты предоставленные";
            ultraExplorerBarItem15.Key = "Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.CapitalPlanningOperations.Pl" +
                "anningOperationsUI";
            ultraExplorerBarItem15.Text = "Размещение займа";
            ultraExplorerBarItem17.Key = "Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.CapitalOperations.Redemption" +
                ".RedemptionUI";
            ultraExplorerBarItem17.Text = "Выкуп облигаций";
            ultraExplorerBarItem16.Key = "Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.CapitalOperations.LoanToCred" +
                "it.LoanToCreditUI";
            ultraExplorerBarItem16.Text = "Замена займа на кредит";
            ultraExplorerBarGroup8.Items.AddRange(new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem[] {
            ultraExplorerBarItem15,
            ultraExplorerBarItem17,
            ultraExplorerBarItem16});
            ultraExplorerBarGroup8.Text = "Планирование операций с ЦБ";
            ultraExplorerBarItem9.Key = "Krista.FM.Client.ViewObjects.FinSourcePlanningUI.BKKUIndicatorsUI";
            ultraExplorerBarItem9.Text = "Оценка проекта бюджета";
            ultraExplorerBarItem10.Key = "Krista.FM.Client.ViewObjects.FinSourcePlanningUI.DDEIndicatorsUI";
            ultraExplorerBarItem10.Text = "ДДЕ";
            ultraExplorerBarItem11.Key = "1a8df258-af45-45fd-a3bb-9820d9401f59_if";
            ultraExplorerBarItem11.Text = "Расчет остатков средств бюджета";
            ultraExplorerBarItem12.Key = "47d09e3f-c5b1-4a73-80cc-3a222ab8fe30_if";
            ultraExplorerBarItem12.Text = "Определение объема заимствований";
            ultraExplorerBarItem13.Key = "4c605776-b478-4a18-bc9a-28c82ff34186_if";
            ultraExplorerBarItem13.Text = "Оценка расходов на обслуживание";
            ultraExplorerBarItem14.Key = "d108b2dc-4725-4d98-88f1-2c3db453c6ff_if";
            ultraExplorerBarItem14.Text = "Предельные значения долга";
            ultraExplorerBarGroup6.Items.AddRange(new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem[] {
            ultraExplorerBarItem9,
            ultraExplorerBarItem10,
            ultraExplorerBarItem11,
            ultraExplorerBarItem12,
            ultraExplorerBarItem13,
            ultraExplorerBarItem14});
            ultraExplorerBarGroup6.Key = "MainGroup";
            ultraExplorerBarGroup6.Settings.AllowDrag = Infragistics.Win.DefaultableBoolean.False;
            ultraExplorerBarGroup6.Settings.AllowEdit = Infragistics.Win.DefaultableBoolean.False;
            ultraExplorerBarGroup6.Settings.AllowItemDrop = Infragistics.Win.DefaultableBoolean.False;
            ultraExplorerBarGroup6.Settings.HeaderVisible = Infragistics.Win.DefaultableBoolean.True;
            ultraExplorerBarGroup6.Text = "Расчеты";
            ultraExplorerBarGroup7.Expanded = false;
            ultraExplorerBarGroup7.Text = "Справочники";
            this.ultraExplorerBar.Groups.AddRange(new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup[] {
            ultraExplorerBarGroup1,
            ultraExplorerBarGroup2,
            ultraExplorerBarGroup3,
            ultraExplorerBarGroup4,
            ultraExplorerBarGroup5,
            ultraExplorerBarGroup8,
            ultraExplorerBarGroup6,
            ultraExplorerBarGroup7});
            this.ultraExplorerBar.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ultraExplorerBar.Location = new System.Drawing.Point(0, 0);
            this.ultraExplorerBar.Name = "ultraExplorerBar";
            this.ultraExplorerBar.ShowDefaultContextMenu = false;
            this.ultraExplorerBar.Size = new System.Drawing.Size(231, 332);
            this.ultraExplorerBar.Style = Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarStyle.VisualStudio2005Toolbox;
            this.ultraExplorerBar.TabIndex = 0;
            this.ultraExplorerBar.ViewStyle = Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarViewStyle.Office2000;
            // 
            // BaseNavigationCtrl_Fill_Panel
            // 
            this.BaseNavigationCtrl_Fill_Panel.Controls.Add(pnlVariant);
            this.BaseNavigationCtrl_Fill_Panel.Controls.Add(this.ultraExplorerBar);
            this.BaseNavigationCtrl_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.BaseNavigationCtrl_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BaseNavigationCtrl_Fill_Panel.Location = new System.Drawing.Point(0, 23);
            this.BaseNavigationCtrl_Fill_Panel.Name = "BaseNavigationCtrl_Fill_Panel";
            this.BaseNavigationCtrl_Fill_Panel.Size = new System.Drawing.Size(231, 449);
            this.BaseNavigationCtrl_Fill_Panel.TabIndex = 0;
            // 
            // _BaseNavigationCtrl_Toolbars_Dock_Area_Left
            // 
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Left.BackColor = System.Drawing.SystemColors.Control;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Left.Location = new System.Drawing.Point(0, 23);
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Left.Name = "_BaseNavigationCtrl_Toolbars_Dock_Area_Left";
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(0, 449);
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Left.ToolbarsManager = this.ultraToolbarsManager;
            // 
            // ultraToolbarsManager
            // 
            appearance12.BackColor = System.Drawing.SystemColors.Control;
            this.ultraToolbarsManager.Appearance = appearance12;
            this.ultraToolbarsManager.DesignerFlags = 1;
            this.ultraToolbarsManager.DockWithinContainer = this;
            this.ultraToolbarsManager.ImageListSmall = this.il;
            this.ultraToolbarsManager.LockToolbars = true;
            this.ultraToolbarsManager.RightAlignedMenus = Infragistics.Win.DefaultableBoolean.False;
            this.ultraToolbarsManager.RuntimeCustomizationOptions = Infragistics.Win.UltraWinToolbars.RuntimeCustomizationOptions.None;
            this.ultraToolbarsManager.ShowFullMenusDelay = 500;
            this.ultraToolbarsManager.ShowQuickCustomizeButton = false;
            ultraToolbar1.DockedColumn = 0;
            ultraToolbar1.DockedRow = 0;
            ultraToolbar1.NonInheritedTools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            popupMenuTool1});
            ultraToolbar1.Text = "utbReports";
            this.ultraToolbarsManager.Toolbars.AddRange(new Infragistics.Win.UltraWinToolbars.UltraToolbar[] {
            ultraToolbar1});
            appearance13.Image = "reports.bmp";
            popupMenuTool2.SharedPropsInternal.AppearancesSmall.Appearance = appearance13;
            popupMenuTool2.SharedPropsInternal.Caption = "Отчеты по договорам";
            popupMenuTool2.SharedPropsInternal.DisplayStyle = Infragistics.Win.UltraWinToolbars.ToolDisplayStyle.ImageAndText;
            buttonTool2.SharedPropsInternal.Caption = "Фактическая задолженность по кредитам";
            buttonTool4.SharedPropsInternal.Caption = "Просроченная задолженность по кредитам";
            this.ultraToolbarsManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            popupMenuTool2,
            buttonTool2,
            buttonTool4});
            // 
            // il
            // 
            this.il.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("il.ImageStream")));
            this.il.TransparentColor = System.Drawing.Color.Magenta;
            this.il.Images.SetKeyName(0, "reports.bmp");
            // 
            // _BaseNavigationCtrl_Toolbars_Dock_Area_Right
            // 
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Right.BackColor = System.Drawing.SystemColors.Control;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(231, 23);
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Right.Name = "_BaseNavigationCtrl_Toolbars_Dock_Area_Right";
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(0, 449);
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Right.ToolbarsManager = this.ultraToolbarsManager;
            // 
            // _BaseNavigationCtrl_Toolbars_Dock_Area_Top
            // 
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Top.BackColor = System.Drawing.SystemColors.Control;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Top.Name = "_BaseNavigationCtrl_Toolbars_Dock_Area_Top";
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(231, 23);
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Top.ToolbarsManager = this.ultraToolbarsManager;
            // 
            // _BaseNavigationCtrl_Toolbars_Dock_Area_Bottom
            // 
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.SystemColors.Control;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 472);
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Bottom.Name = "_BaseNavigationCtrl_Toolbars_Dock_Area_Bottom";
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(231, 0);
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Bottom.ToolbarsManager = this.ultraToolbarsManager;
            // 
            // FinSourcePlanningNavigation
            // 
            this.Controls.Add(this.BaseNavigationCtrl_Fill_Panel);
            this.Controls.Add(this._BaseNavigationCtrl_Toolbars_Dock_Area_Left);
            this.Controls.Add(this._BaseNavigationCtrl_Toolbars_Dock_Area_Right);
            this.Controls.Add(this._BaseNavigationCtrl_Toolbars_Dock_Area_Top);
            this.Controls.Add(this._BaseNavigationCtrl_Toolbars_Dock_Area_Bottom);
            this.Name = "FinSourcePlanningNavigation";
            this.Size = new System.Drawing.Size(231, 472);
            pnlVariant.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ultraExplorerBar)).EndInit();
            this.BaseNavigationCtrl_Fill_Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ultraToolbarsManager)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        public override void Customize()
        {
            ComponentCustomizer.CustomizeInfragisticsComponents(components);
            base.Customize();
        }

        internal event EventHandler VariantChanged;

        internal event VariantEventHandler VariantChangedNew;

        #region настройка прав

        private void SetFinSourcesPermissions()
        {
            bool capitalsVisible = Workplace.ActiveScheme.FinSourcePlanningFace.CheckUIModuleVisible("Capitals");
            bool creditIncomesVisible = Workplace.ActiveScheme.FinSourcePlanningFace.CheckUIModuleVisible("CreditIncomes");
            bool creditIssuedVisible = Workplace.ActiveScheme.FinSourcePlanningFace.CheckUIModuleVisible("CreditIssued");
            bool guaranteeIssuedVisible = Workplace.ActiveScheme.FinSourcePlanningFace.CheckUIModuleVisible("GuaranteeIssued");
            bool referenceBooksVisible = Workplace.ActiveScheme.FinSourcePlanningFace.CheckUIModuleVisible("ReferenceBooks");
            bool ddeVisible = Workplace.ActiveScheme.FinSourcePlanningFace.CheckUIModuleVisible("DDE");
            bool bbkVisible = Workplace.ActiveScheme.FinSourcePlanningFace.CheckUIModuleVisible("BBK");
            bool volumeHoldings = Workplace.ActiveScheme.FinSourcePlanningFace.CheckUIModuleVisible("VolumeHoldings");
            bool remainsDesign = Workplace.ActiveScheme.FinSourcePlanningFace.CheckUIModuleVisible("RemainsDesign");
            bool сapitalOperations = Workplace.ActiveScheme.FinSourcePlanningFace.CheckUIModuleVisible("CapitalOperations");
            
            ultraExplorerBar.Groups[1].Visible = capitalsVisible;
            ultraExplorerBar.Groups[2].Visible =  guaranteeIssuedVisible;
            ultraExplorerBar.Groups[3].Visible = creditIncomesVisible;
            ultraExplorerBar.Groups[4].Visible = creditIssuedVisible;
            ultraExplorerBar.Groups[5].Visible = сapitalOperations;
            ultraExplorerBar.Groups[7].Visible = referenceBooksVisible;
            ultraExplorerBar.Groups[6].Items["Krista.FM.Client.ViewObjects.FinSourcePlanningUI.DDEIndicatorsUI"].Visible
                = ddeVisible;
            ultraExplorerBar.Groups[6].Items["Krista.FM.Client.ViewObjects.FinSourcePlanningUI.BKKUIndicatorsUI"].Visible
                = bbkVisible;
            ultraExplorerBar.Groups[6].Items[SchemeObjectsKeys.f_S_RemainsDesign + "_if"].Visible = remainsDesign;
            ultraExplorerBar.Groups[6].Items[SchemeObjectsKeys.f_S_VolumeHoldings_Key + "_if"].Visible = volumeHoldings;

            bool calculatesVisible = false;
            foreach (UltraExplorerBarItem item in ultraExplorerBar.Groups[4].Items)
                if (item.Visible)
                {
                    calculatesVisible = true;
                    break;
                }
            ultraExplorerBar.Groups[4].Visible = calculatesVisible;
        }

        #endregion

        #region получение источника данных в зависимости от выбранного варианта

        private void GetDataSourceIDByVariantID(int variantID)
        {
            if (variantID == -1)
                return;

            IEntity variantEntity = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme.RootPackage.FindEntityByName(
                SchemeObjectsKeys.d_Variant_Borrow_Key);

            using (IDatabase db = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme.SchemeDWH.DB)
            {
                int dataSourceID = Utils.GetDataSourceID(db, "ФО", 29, 29, BaseYear);
                DataTable dt = (DataTable)db.ExecQuery("select ID from DataSources where SupplierCode = 'ФО' and DataCode = 29 and Year = ? and deleted = 0",
                    QueryResultTypes.DataTable,
                    new DbParameterDescriptor("ID", BaseYear));
                if (dt.Rows.Count == 0)
                {
                    IDataSource ds = Workplace.ActiveScheme.DataSourceManager.DataSources.CreateElement();
                    ds.SupplierCode = "ФО";
                    ds.DataCode = "0029";
                    ds.DataName = "Проект бюджета";
                    ds.Year = BaseYear;
                    ds.ParametersType = ParamKindTypes.Year;
                    dataSourceID = ds.Save();
                }

                else if (dt.Rows.Count > 1)
                {
                    throw new FinSourcePlanningException("Обнаружено несколько источников данных за один год по источникам финансирования");
                }
                else
                {
                    dataSourceID = Convert.ToInt32(dt.Rows[0][0]);
                }

                CurrentSourceID = dataSourceID;
            }
        }


        #endregion
    }

    public delegate void VariantEventHandler(object sender, VariantChangeEventHandler e);

    public class VariantChangeEventHandler : EventArgs
    {
        public VariantChangeEventHandler(int variantId, int sourceId, string variantCaption)
        {
            this.variantId = variantId;
            this.variantCaption = variantCaption;
            this.sourceId = sourceId;
        }

        private int variantId;
        /// <summary>
        /// ID нового варианта
        /// </summary>
        public int VariantID
        {
            get { return variantId; }
        }

        private string variantCaption;
        /// <summary>
        /// наименование нового варианта
        /// </summary>
        public string VariantCaption
        {
            get { return variantCaption; }
        }

        private int sourceId;
        /// <summary>
        /// ID нового варианта
        /// </summary>
        public int SourceID
        {
            get { return sourceId; }
        }
    }
}
