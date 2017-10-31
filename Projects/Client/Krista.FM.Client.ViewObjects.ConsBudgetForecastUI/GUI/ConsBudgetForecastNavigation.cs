using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infragistics.Win.UltraWinExplorerBar;
using Krista.FM.Client.Common;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.AssociatedCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.DataCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.FactTables;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.GUI.Balance;
using Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.GUI.Handbooks;
using Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.GUI.IncomesEvalPlan;
using Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.GUI.IncomesYearPlan;
using Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.GUI.PriorForecast;
using Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.GUI.TaxpayersSum;
using Krista.FM.Client.Workplace.Gui;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.ConsBudgetForecastUI
{
    public delegate void NextFinancialYearChange(object sender, int nextFinancialYear);

    public class ConsBudgetForecastNavigation : BaseNavigationCtrl
    {
        private System.Windows.Forms.Panel panel2;
        internal UltraExplorerBar uebNavigation;
        private static ConsBudgetForecastNavigation instance;

        internal static ConsBudgetForecastNavigation Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ConsBudgetForecastNavigation();
                }
                return instance;
            }
        }

        public  ConsBudgetForecastNavigation()
        {
            Caption = "Планирование доходов";
            instance = this;
        }

        public override System.Drawing.Image TypeImage16
        {
            get { return Properties.Resources.Income_icon_16; }
        }

        public override System.Drawing.Image TypeImage24
        {
            get { return Properties.Resources.Income_icon_32; }
        }

        /// <summary>
        /// Инициализация интерфейса
        /// </summary>
        public override void Initialize()
        {
            InitializeComponent();

            uebNavigation.ItemCheckStateChanged += new Infragistics.Win.UltraWinExplorerBar.ItemCheckStateChangedEventHandler(uebNavigation_ItemCheckStateChanged);
            Workplace.ViewClosed += new ViewContentEventHandler(Workplace_ViewClosed);
            Workplace.ActiveWorkplaceWindowChanged += Workplace_ActiveWorkplaceWindowChanged;

            base.Initialize();

            InfragisticsRusification.LocalizeAll();

            SetPermissions();
        }

        void uebNavigation_ItemCheckStateChanged(object sender, Infragistics.Win.UltraWinExplorerBar.ItemEventArgs e)
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
                    BaseViewObj viewObject = UIFactory(e.Item.Key);
                    viewObject.Workplace = Workplace;
                    viewObject.Initialize();
                    viewObject.ViewCtrl.Text = string.Concat(e.Item.Text);
                    OnActiveItemChanged(this, viewObject);
                    viewObject.InitializeData();
                }
            }
        }

        private BaseViewObj UIFactory(string objectKey)
        {
            string key = objectKey.Split('_')[0];
            BaseViewObj baseViewObj = null;
            switch (key)
            {
                case ObjectKeys.consBudgetForecastUI:
                    baseViewObj = new ConsBudgetForecastUI(objectKey);
                    break;
                case ObjectKeys.yearIncomingsPlan:
                    baseViewObj = new IncomesYearPlanUI(objectKey);
                    break;
                case ObjectKeys.priorForecastUI:
                    baseViewObj = new PriorForecastUI(objectKey);
                    break;
                case ObjectKeys.b_Regions_BridgePlan:
                    IEntity entity = Workplace.ActiveScheme.RootPackage.FindEntityByName(key);
                    baseViewObj = new AssociationClassifierUI(entity, objectKey);
                    break;
                case ObjectKeys.d_KD_PlanIncomes:
                    entity = Workplace.ActiveScheme.RootPackage.FindEntityByName(key);
                    baseViewObj = new KdPlanIncomesCls(entity, objectKey);
                    break;
                case ObjectKeys.d_Regions_Plan:
                case ObjectKeys.d_Org_TaxBenPay:
                case ObjectKeys.d_KVSR_Plan:
                    entity = Workplace.ActiveScheme.RootPackage.FindEntityByName(key);
                    baseViewObj = new DataClassifierUI(entity, objectKey);
                    break;
                case ObjectKeys.d_Variant_PlanIncomes:
                    entity = Workplace.ActiveScheme.RootPackage.FindEntityByName(key);
                    baseViewObj = new VariantClassifierUI(entity, objectKey);
                    break;
                case ObjectKeys.f_D_FOPlanInc:
                case ObjectKeys.f_D_FOPlanIncDivide:
                    entity = Workplace.ActiveScheme.RootPackage.FindEntityByName(key);
                    baseViewObj = new FactTableUI(entity, objectKey);
                    break;
                case ObjectKeys.balance:
                    baseViewObj = new BalanceUI(key);
                    break;
                case ObjectKeys.IncomesEvalPlan:
                    baseViewObj = new IncomesEvalPlanUI(key);
                    break;
                case ObjectKeys.TaxpayerSum:
                    baseViewObj = new TaxpayersSumUI(key);
                    break;
            }
            return baseViewObj;
        }

        #region глобальные события воркплейса

        void Workplace_ActiveWorkplaceWindowChanged(object sender, EventArgs e)
        {
            if (Workplace.WorkplaceLayout.ActiveContent != null)
            {
                string key = ((BaseViewObj)Workplace.WorkplaceLayout.ActiveContent).Key;
                UltraExplorerBarGroup group = null;
                foreach (UltraExplorerBarGroup groupBar in uebNavigation.Groups)
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
                Workplace.SwitchTo("Планирование доходов");
                if (uebNavigation.CheckedItem == null)
                    return;
                if (!uebNavigation.Visible)
                    return;
                if (key != uebNavigation.CheckedItem.Key && group.Items[key].Visible)
                {
                    group.Items[key].Checked = true;
                    group.Items[key].Active = true;
                }
            }
        }

        void Workplace_ViewClosed(object sender, ViewContentEventArgs e)
        {
            if (uebNavigation.CheckedItem == null)
                return;

            if (e.Content.Key == uebNavigation.CheckedItem.Key)
            {
                uebNavigation.CheckedItem.Active = false;
                uebNavigation.CheckedItem.Checked = false;
            }
        }

        #endregion

        private void InitializeComponent()
        {
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup ultraExplorerBarGroup1 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem1 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConsBudgetForecastNavigation));
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem8 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem9 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem10 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem11 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem14 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup ultraExplorerBarGroup2 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem2 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem3 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem4 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem5 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem6 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem7 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem12 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem13 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            this.panel2 = new System.Windows.Forms.Panel();
            this.uebNavigation = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBar();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.uebNavigation)).BeginInit();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.uebNavigation);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(216, 472);
            this.panel2.TabIndex = 3;
            // 
            // uebNavigation
            // 
            this.uebNavigation.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.uebNavigation.Dock = System.Windows.Forms.DockStyle.Fill;
            ultraExplorerBarItem1.Key = "Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.GUI.ConsBudgetForecastUI";
            ultraExplorerBarItem1.Settings.AllowDragCopy = Infragistics.Win.UltraWinExplorerBar.ItemDragStyle.None;
            ultraExplorerBarItem1.Settings.AllowDragMove = Infragistics.Win.UltraWinExplorerBar.ItemDragStyle.None;
            ultraExplorerBarItem1.Settings.AllowEdit = Infragistics.Win.DefaultableBoolean.False;
            appearance1.Image = ((object)(resources.GetObject("appearance1.Image")));
            ultraExplorerBarItem1.Settings.AppearancesSmall.Appearance = appearance1;
            ultraExplorerBarItem1.Text = "Расщепление доходов";
            ultraExplorerBarItem8.Key = "Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.GUI.IncomesYearPlan.IncomesYear" +
                "PlanUI";
            appearance2.Image = ((object)(resources.GetObject("appearance2.Image")));
            ultraExplorerBarItem8.Settings.AppearancesSmall.Appearance = appearance2;
            ultraExplorerBarItem8.Text = "Годовой план по доходам";
            ultraExplorerBarItem9.Key = "Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.GUI.Balance.BalanceUI";
            appearance3.Image = ((object)(resources.GetObject("appearance3.Image")));
            ultraExplorerBarItem9.Settings.AppearancesSmall.Appearance = appearance3;
            ultraExplorerBarItem9.Text = "Балансировка";
            ultraExplorerBarItem10.Key = "Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.GUI.IncomesEvalPlan.IncomesEval" +
                "PlanUI";
            appearance4.Image = ((object)(resources.GetObject("appearance4.Image")));
            ultraExplorerBarItem10.Settings.AppearancesSmall.Appearance = appearance4;
            ultraExplorerBarItem10.Text = "Оценка и прогноз по доходам";
            ultraExplorerBarItem11.Key = "Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.GUI.TaxpayersSum.TaxpayersSumUI" +
                "";
            appearance5.Image = ((object)(resources.GetObject("appearance5.Image")));
            ultraExplorerBarItem11.Settings.AppearancesSmall.Appearance = appearance5;
            ultraExplorerBarItem11.Text = "Суммы НП к доплате (уменьшению)";
            ultraExplorerBarItem14.Key = "Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.GUI.PriorForecast.PriorForecast" +
                "UI";
            appearance6.Image = ((object)(resources.GetObject("appearance6.Image")));
            ultraExplorerBarItem14.Settings.AppearancesSmall.Appearance = appearance6;
            ultraExplorerBarItem14.Text = "Предварительный прогноз по доходам";
            ultraExplorerBarGroup1.Items.AddRange(new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem[] {
            ultraExplorerBarItem1,
            ultraExplorerBarItem8,
            ultraExplorerBarItem9,
            ultraExplorerBarItem10,
            ultraExplorerBarItem11,
            ultraExplorerBarItem14});
            ultraExplorerBarGroup1.Settings.AllowDrag = Infragistics.Win.DefaultableBoolean.False;
            ultraExplorerBarGroup1.Settings.AllowEdit = Infragistics.Win.DefaultableBoolean.False;
            ultraExplorerBarGroup1.Settings.AllowItemDrop = Infragistics.Win.DefaultableBoolean.False;
            ultraExplorerBarGroup1.Settings.HeaderVisible = Infragistics.Win.DefaultableBoolean.False;
            ultraExplorerBarGroup1.Text = "New Group";
            ultraExplorerBarItem2.Key = "1525f07f-8a60-47af-9b80-7200e74956bc_cbf";
            ultraExplorerBarItem2.Text = "Вариант.Проект доходов";
            ultraExplorerBarItem3.Key = "1f34cc90-16fd-4fcf-b994-0c8a680d7e23_cbf";
            ultraExplorerBarItem3.Text = "Районы.Планирование";
            ultraExplorerBarItem4.Key = "a6e33772-325a-4932-a0aa-7ce82f0b3921_cbf";
            ultraExplorerBarItem4.Text = "КД.Планирование";
            ultraExplorerBarItem5.Key = "24962405-0ac5-48ed-83f6-127134116703_cbf";
            ultraExplorerBarItem5.Text = "Районы.Сопоставимый планирование";
            ultraExplorerBarItem6.Key = "80319561-787b-4791-a85d-5a26b7a1c19f_cbf";
            ultraExplorerBarItem6.Text = "Доходы.ФО_Результат доходов без расщепления";
            ultraExplorerBarItem7.Key = "3f71b13b-3e87-45ad-8f72-1d023da07d10_cbf";
            ultraExplorerBarItem7.Text = "Доходы.ФО_Результат доходов с расщеплением";
            ultraExplorerBarItem12.Key = "3cb29958-a461-4c4b-b8dd-3f6ff0f67982_cbf";
            ultraExplorerBarItem12.Text = "Организации.МОФО_НП к доплате_уменьшению";
            ultraExplorerBarItem13.Key = "dd69b4e1-f257-49ce-b553-442d094ae39a_cbf";
            ultraExplorerBarItem13.Text = "Администратор.Планирование";
            ultraExplorerBarGroup2.Items.AddRange(new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem[] {
            ultraExplorerBarItem2,
            ultraExplorerBarItem3,
            ultraExplorerBarItem4,
            ultraExplorerBarItem5,
            ultraExplorerBarItem6,
            ultraExplorerBarItem7,
            ultraExplorerBarItem12,
            ultraExplorerBarItem13});
            ultraExplorerBarGroup2.Text = "Справочники";
            this.uebNavigation.Groups.AddRange(new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup[] {
            ultraExplorerBarGroup1,
            ultraExplorerBarGroup2});
            this.uebNavigation.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.uebNavigation.Location = new System.Drawing.Point(0, 0);
            this.uebNavigation.Name = "uebNavigation";
            this.uebNavigation.ShowDefaultContextMenu = false;
            this.uebNavigation.Size = new System.Drawing.Size(216, 472);
            this.uebNavigation.Style = Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarStyle.VisualStudio2005Toolbox;
            this.uebNavigation.TabIndex = 2;
            this.uebNavigation.ViewStyle = Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarViewStyle.Office2000;
            // 
            // ConsBudgetForecastNavigation
            // 
            this.Controls.Add(this.panel2);
            this.Name = "ConsBudgetForecastNavigation";
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.uebNavigation)).EndInit();
            this.ResumeLayout(false);

        }

        private int nextFinancialYear;

        public int NextFinancialYear
        {
            get { return nextFinancialYear; }

            private set { nextFinancialYear = value; }
        }


        internal event NextFinancialYearChange nextFinancialYearChange;

        /// <summary>
        /// устанавливаем права
        /// </summary>
        private void SetPermissions()
        {
            bool visibleAll = Workplace.ActiveScheme.UsersManager.CheckPermissionForSystemObject("IncomesPlaning",
                (int)IncomesPlaningOperations.ViewPlaningOperations, false);

            bool incomesSplitVisible = Workplace.ActiveScheme.UsersManager.CheckPermissionForSystemObject("IncomesSplit",
                (int)IncomesPlaningModuleOperations.ViewPlaningOperationsModule, false);
            bool incomesYearPlanVisible = Workplace.ActiveScheme.UsersManager.CheckPermissionForSystemObject("IncomesYearPlan",
                (int)IncomesPlaningModuleOperations.ViewPlaningOperationsModule, false);
            bool balanceVisible = Workplace.ActiveScheme.UsersManager.CheckPermissionForSystemObject("Balance",
                (int)IncomesPlaningModuleOperations.ViewPlaningOperationsModule, false);
            bool incomesEvalPlanVisible = Workplace.ActiveScheme.UsersManager.CheckPermissionForSystemObject("IncomesEvalPlan",
                (int)IncomesPlaningModuleOperations.ViewPlaningOperationsModule, false);
            bool taxpayersSumVisible = Workplace.ActiveScheme.UsersManager.CheckPermissionForSystemObject("TaxpayersSum",
                (int)IncomesPlaningModuleOperations.ViewPlaningOperationsModule, false);
            bool priorForecastVisible = Workplace.ActiveScheme.UsersManager.CheckPermissionForSystemObject("PriorForecast",
                (int)IncomesPlaningModuleOperations.ViewPlaningOperationsModule, false);

            uebNavigation.Groups[0].Items[0].Visible = visibleAll || incomesSplitVisible;
            uebNavigation.Groups[0].Items[1].Visible = visibleAll || incomesYearPlanVisible;
            uebNavigation.Groups[0].Items[2].Visible = visibleAll || balanceVisible;
            uebNavigation.Groups[0].Items[3].Visible = visibleAll || incomesEvalPlanVisible;
            uebNavigation.Groups[0].Items[4].Visible = visibleAll || taxpayersSumVisible;
            uebNavigation.Groups[0].Items[5].Visible = visibleAll || priorForecastVisible;
        }
    }
}
