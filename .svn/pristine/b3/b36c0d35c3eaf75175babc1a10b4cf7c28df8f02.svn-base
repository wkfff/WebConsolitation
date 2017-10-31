using System;
using System.Data;
using System.Drawing;
using Infragistics.Win.UltraWinExplorerBar;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.ViewObjects.FinSourceDebtorBookUI.Commands;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Client.ViewObjects.FinSourceDebtorBookUI.Commands.Reports;
using Krista.FM.Client.ViewObjects.FinSourceDebtorBookUI.Gui;
using Krista.FM.Client.ViewObjects.FinSourceDebtorBookUI.Services;
using Krista.FM.Client.Workplace.Gui;
using Krista.FM.Client.Workplace.Services;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;
using Krista.FM.Domain;
using Krista.FM.Domain.Services.FinSourceDebtorBook;
using Krista.FM.Client.Reports;

namespace Krista.FM.Client.ViewObjects.FinSourceDebtorBookUI
{
    public enum UserRegionType
    {
        Subject,
        Town,
        Region,
        Settlement,
        Unknown
    }

    public class DebtBookNavigation : BaseNavigationCtrl
    {
        #region Визуальные компоненты

        internal Infragistics.Win.UltraWinExplorerBar.UltraExplorerBar ultraExplorerBar;
        private System.Windows.Forms.Panel panelVariant;
        private Infragistics.Win.Misc.UltraButton btnSelectVariant;
        private System.Windows.Forms.TextBox tbVariantCaption;

        #endregion
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsManager ultraToolbarsManager1;
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.Panel BaseNavigationCtrl_Fill_Panel;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseNavigationCtrl_Toolbars_Dock_Area_Left;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseNavigationCtrl_Toolbars_Dock_Area_Right;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseNavigationCtrl_Toolbars_Dock_Area_Top;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseNavigationCtrl_Toolbars_Dock_Area_Bottom;
        private System.Windows.Forms.ImageList il;
        private System.Windows.Forms.ComboBox cbDataState;
        private System.Windows.Forms.Panel panel1;

        private static DebtBookNavigation instance;

        public static DebtBookNavigation Instance
        {
            get
            {
                if (instance == null)
                    instance = new DebtBookNavigation();
                return instance;
            }
        }

        public override Image TypeImage16
        {
            get { return Properties.Resources.books_016; }
        }

        public override Image TypeImage24
        {
            get { return Properties.Resources.books_024; }
        }

        public DebtBookNavigation()
        {
            instance = this;
            currentVariantID = -1;
            Caption = "Долговая книга";
        }

        private int currentVariantID;
        /// <summary>
        /// текущий вариант
        /// </summary>
        public int CurrentVariantID
        {
            get{ return currentVariantID; }
            private set { currentVariantID = value; }
        }

        private int currentSourceID;
        /// <summary>
        /// текущий источник данных
        /// </summary>
        public int CurrentSourceID
        {
            get { return currentSourceID; }
            private set { currentSourceID = value; }
        }

        private int currentRegion = -1;
        /// <summary>
        /// регион текущего пользователя
        /// </summary>
        public int CurrentRegion
        {
            get
            {
                return currentRegion;
            }
            private set 
            {
                currentRegion = value; 
            }
        }

        private int currentStatusSchb;
        /// <summary>
        /// Текущий статус.
        /// </summary>
        public int CurrentStatusSchb
        {
            get { return currentStatusSchb; }
            private set { currentStatusSchb = value; }
        }

        private int userYear = -1;
        /// <summary>
        /// год источника, по которому записан текущий пользователь
        /// </summary>
        public int UserYear
        {
            get { return userYear; }
            private set { userYear = value; }
        }

        private int currentAnalizSourceID = -1;
        /// <summary>
        /// текущий источник для районы.анализ
        /// </summary>
        public int CurrentAnalizSourceID
        {
            get
            {
                return currentAnalizSourceID;
            }
            private set { currentAnalizSourceID = value; }
        }

        private DateTime calculateDate;
        /// <summary>
        /// дата, на которую считаем долговую книгу
        /// </summary>
        public DateTime CalculateDate
        {
            get { return calculateDate; }
            private set { calculateDate = value; }
        }

        private bool isVariantComplete;

        public bool IsVariantComplete
        {
            get { return isVariantComplete; }
            private set { isVariantComplete = value; }
        }

        private UserRegionType userRegionType;
        /// <summary>
        /// тип принадлежности пользователя к региону
        /// </summary>
        public UserRegionType UserRegionType
        {
            get { return userRegionType; }
            set { userRegionType = value; }
        }

        private int subjectRegionID = -1;

        public int SubjectRegionID
        {
            get
            {
                return subjectRegionID;
            }
            private set { subjectRegionID = value; }

        }

        private int variantYear = -1;
        /// <summary>
        /// Год текущего варианта
        /// </summary>
        public int VariantYear
        {
            get { return variantYear; }
            private set { variantYear = value; }
        }

        private bool allowEditData;
        public bool AllowEditData
        {
            get { return allowEditData; }
        }

        private IFinSourceDebtorBookFacade services;
        public IFinSourceDebtorBookFacade Services
        {
            get { return services; }
        }

        private RegionsAccordanceService regionsAccordanceService;

        public override void Initialize()
        {
            InitializeComponent();

            base.Initialize();

            // TODO: Либо создавать экземпляры на клиенте (для разработки) либо получать их от вервера (реальная эксплуатация).
            services = new ClientServicesFacade(Workplace.ActiveScheme);
            regionsAccordanceService = services.RegionsAccordanceService;

            Workplace.ViewClosed += Workplace_ViewClosed;
            Workplace.ActiveWorkplaceWindowChanged += Workplace_ActiveWorkplaceWindowChanged;

            ultraExplorerBar.ItemCheckStateChanged += ultraExplorerBar_ItemCheckStateChanged;
            ultraExplorerBar.ItemCheckStateChanging += ultraExplorerBar_ItemCheckStateChanging;

            cbDataState.Items.Add("На редактировании");
            cbDataState.Items.Add("Ввод данных завершен");
            cbDataState.Items.Add("Утверждено");

            SetDefaultVariant();
            SubjectRegionID = GetSubjectRegion();
            SetUserRegion();
            UserRegionType = GetUserRegionType(CurrentRegion);
            FillProtocolData();
            CheckCompliteData();

            UltraToolbar toolBar = ultraToolbarsManager1.Toolbars[0];

            CommandService.AttachCommandToControl(new SelectVariantCommand(), btnSelectVariant);

            ButtonTool button = CommandService.AttachToolbarTool(new TransfertDataCommand(), toolBar);
            button.SharedProps.AppearancesSmall.Appearance.Image = il.Images[1];

            // выгрузка в минфин
            ButtonTool btnCreateReport = CommandService.AttachToolbarTool(new MinfinReportCommand(), toolBar);
            btnCreateReport.SharedProps.Visible = UserRegionType == UserRegionType.Subject;
            btnCreateReport.SharedProps.AppearancesSmall.Appearance.Image = Properties.Resources.CashRegister16;
            ButtonTool btnCopyRegionData = CommandService.AttachToolbarTool(new CopyRegionDataToVariantCommand(), toolBar);
            btnCopyRegionData.SharedProps.AppearancesSmall.Appearance.Image = il.Images[3];

            ultraExplorerBar.Groups[1].Visible = !(UserRegionType == UserRegionType.Settlement || UserRegionType == UserRegionType.Unknown);
            ultraExplorerBar.Groups[1].Items[0].Visible = UserRegionType == UserRegionType.Subject;
            ultraExplorerBar.Groups[1].Items[1].Visible = UserRegionType == UserRegionType.Subject;
            ultraExplorerBar.Groups[1].Items[2].Visible = UserRegionType == UserRegionType.Subject;

            cbDataState.Visible = UserRegionType == UserRegionType.Region ||
                UserRegionType == UserRegionType.Settlement || UserRegionType == UserRegionType.Town;
            
            btnCopyRegionData.SharedProps.Visible =
                (UserRegionType == UserRegionType.Region || UserRegionType == UserRegionType.Town) && AllowEditData;
            button.SharedProps.Visible = UserRegionType == UserRegionType.Subject;

            cbDataState.Enabled = CurrentVariantID >= 0;
        }

        /// <summary>
        /// устанавливаем текущий вариант по умолчанию
        /// </summary>
        private void SetDefaultVariant()
        {
            IEntity entity = Workplace.ActiveScheme.RootPackage.FindEntityByName(DomainObjectsKeys.d_Variant_Schuldbuch);
            using (IDataUpdater du = entity.GetDataUpdater())
            {
                DataSet dsVariant = new DataSet();
                du.Fill(ref dsVariant);
                foreach (DataRow variantRow in dsVariant.Tables[0].Rows)
                {
                    if (Convert.ToBoolean(variantRow["CurrentVariant"]))
                    {
                        Instance.SetCurrentVariant(Convert.ToInt32(variantRow["ID"]), variantRow, false);
                    }
                }
            }
        }

        /// <summary>
        /// установка региона текущего пользователя
        /// </summary>
        internal void SetUserRegion()
        {
            using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
            {
                string userRegionQuery = string.Format("select refregion from users where id = {0}", Workplace.ActiveScheme.UsersManager.GetCurrentUserID());
                object queryResult = db.ExecQuery(userRegionQuery, QueryResultTypes.Scalar);
                if (queryResult is DBNull)
                    CurrentRegion = -1;
                else
                {
                    int userRegion = Convert.ToInt32(queryResult);
                    UserYear = regionsAccordanceService.GetRegionYear(userRegion, db);
                    regionsAccordanceService.FillData(UserYear);
                    if (VariantYear != UserYear && CurrentVariantID != -1)
                    {
                        CurrentRegion = regionsAccordanceService.GetRegionsByYear(VariantYear, userRegion, userYear)[0];
                        regionsAccordanceService.SetRegionTitles(userRegion, UserYear, VariantYear);
                    }
                    else
                    {
                        CurrentRegion = userRegion;
                        regionsAccordanceService.SetRegionTitles(userRegion, UserYear, UserYear - 1);
                    }
                }
            }
        }

        internal int GetActualRegion(int region, IDatabase db)
        {
            UserYear = regionsAccordanceService.GetRegionYear(region, db);
            if (VariantYear != UserYear && CurrentVariantID != -1)
                return regionsAccordanceService.GetRegionsByYear(VariantYear, region, userYear)[0];
            return -1;
        }

        private int GetSubjectRegion()
        {
            currentAnalizSourceID = services.DataSourceService.GetDataSource(VariantYear, "ФО", 6, "Анализ данных", true);
            IEntity entity = Workplace.ActiveScheme.RootPackage.FindEntityByName(DomainObjectsKeys.d_Regions_Analysis);
            using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
            {
                object queryResult = db.ExecQuery(string.Format("select ID from {0} where RefTerr = 3 and SourceID = {1}",
                    entity.FullDBName, CurrentAnalizSourceID), QueryResultTypes.Scalar);
                if (queryResult is DBNull)
                    return -1;
                return Convert.ToInt32(queryResult);
            }
        }

        /// <summary>
        /// получаем тип региона, к которому привязан пользователь
        /// </summary>
        public UserRegionType GetUserRegionType(int region)
        {
            using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
            {
                string regionQuery = string.Format("select RefTerr from d_Regions_Analysis where id = {0}",
                    region);
                DataTable dtRegion = (DataTable)db.ExecQuery(regionQuery, QueryResultTypes.DataTable);
                if (dtRegion.Rows.Count != 0)
                {
                    int regionType = Convert.ToInt32(dtRegion.Rows[0][0]);
                    switch (regionType)
                    {
                        case 3:
                            return UserRegionType.Subject;
                        case 4:
                            return UserRegionType.Region;
                        case 7:
                            return UserRegionType.Town;
                        case 5:
                        case 6:
                        case 11:
                            return UserRegionType.Settlement;
                    }
                }
                return UserRegionType.Unknown;
            }
        }

        #region События визуальных компонентов

        void ultraExplorerBar_ItemCheckStateChanging(object sender, Infragistics.Win.UltraWinExplorerBar.CancelableItemEventArgs e)
        {
            // Если вариант не выбран, то принудительно заставляем пользователя выбрать вариант.
            string key = e.Item.Key.Split('_')[0];
            if (currentVariantID == -1 && (
                key == DomainObjectsKeys.f_S_SchBCapital ||
                key == DomainObjectsKeys.f_S_SchBCreditincome ||
                key == DomainObjectsKeys.f_S_SchBGuarantissued))
            {
                new SelectVariantCommand().Run();
                if (currentVariantID == -1)
                {
                    e.Cancel = true;
                }
            }
        }

        void ultraExplorerBar_ItemCheckStateChanged(object sender, Infragistics.Win.UltraWinExplorerBar.ItemEventArgs e)
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
                    BaseViewObj viewObject = CreateClsObject(e.Item.Key);
                    viewObject.Workplace = Workplace;
                    viewObject.Initialize();
                    viewObject.ViewCtrl.Text = string.Concat("ДК_", e.Item.Text);

                    OnActiveItemChanged(this, viewObject);

                    viewObject.InitializeData();
                }
            }
        }

        #endregion

        #region создание, переход между объектами просмотра

        private BaseViewObj CreateClsObject(string objectKey)
        {
            string newKey = objectKey.Split('_')[0];

            switch (newKey)
            {
                case DomainObjectsKeys.d_Variant_Schuldbuch:
                    IEntity entity = Workplace.ActiveScheme.RootPackage.FindEntityByName(objectKey);
                    return new DebtorBookVariantUI(entity);
                case DomainObjectsKeys.f_S_SchBCreditincome:
                    string creditType = objectKey.Split('_')[1];
                    BaseViewObj clsUI = null;
                    switch (creditType)
                    {
                        case "Bud":
                            clsUI = new BudgetCreditUI(objectKey);
                            break;
                        case "Org":
                            clsUI = new OrganizationCreditUI(objectKey);
                            break;
                    }
                    return clsUI;
                case DomainObjectsKeys.f_S_SchBGuarantissued:
                    return new GuaranteeUI(newKey);
                case DomainObjectsKeys.f_S_SchBCapital:
                    return new CapitalUI(newKey);
                case DomainObjectsKeys.t_S_ProtocolTransfer:
                    entity = Workplace.ActiveScheme.RootPackage.FindEntityByName(objectKey);
                    return new ProtocolUI(entity);
                case DomainObjectsKeys.d_S_RegionMatchOperation:
                    entity = Workplace.ActiveScheme.RootPackage.FindEntityByName(objectKey);
                    return new RegionsAccordanceUI(entity);
                case DomainObjectsKeys.d_S_TitleReport:
                    entity = Workplace.ActiveScheme.RootPackage.FindEntityByName(objectKey);
                    return new PostsHandBookUI(entity); 
            }
            return null;
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
                Workplace.SwitchTo("Долговая книга");
                if (ultraExplorerBar.CheckedItem == null)
                    return;
                if (key != ultraExplorerBar.CheckedItem.Key)
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

        #endregion

        #region инициаизация визуальных компонентов

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup ultraExplorerBarGroup1 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem1 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem2 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem3 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem4 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup ultraExplorerBarGroup2 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem5 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem6 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem7 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem8 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DebtBookNavigation));
            Infragistics.Win.Appearance appearance24 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.UltraToolbar ultraToolbar1 = new Infragistics.Win.UltraWinToolbars.UltraToolbar("UltraToolbar");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool1 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("pmReports");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool3 = new Infragistics.Win.UltraWinToolbars.ButtonTool("btnDataTransfert");
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool2 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("pmReports");
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            this.ultraExplorerBar = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBar();
            this.panelVariant = new System.Windows.Forms.Panel();
            this.tbVariantCaption = new System.Windows.Forms.TextBox();
            this.btnSelectVariant = new Infragistics.Win.Misc.UltraButton();
            this.il = new System.Windows.Forms.ImageList(this.components);
            this.BaseNavigationCtrl_Fill_Panel = new System.Windows.Forms.Panel();
            this.cbDataState = new System.Windows.Forms.ComboBox();
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ultraToolbarsManager1 = new Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.ultraExplorerBar)).BeginInit();
            this.panelVariant.SuspendLayout();
            this.BaseNavigationCtrl_Fill_Panel.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraToolbarsManager1)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraExplorerBar
            // 
            this.ultraExplorerBar.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.ultraExplorerBar.Dock = System.Windows.Forms.DockStyle.Fill;
            ultraExplorerBarItem1.Key = "328a93cf-9769-4980-97e3-32570636b125";
            ultraExplorerBarItem1.Text = "Ценные бумаги";
            ultraExplorerBarItem2.Key = "43c55c92-c819-4e0b-95a1-3b941bc2789f_Org";
            ultraExplorerBarItem2.Text = "Кредиты от кредитных организаций";
            ultraExplorerBarItem3.Key = "43c55c92-c819-4e0b-95a1-3b941bc2789f_Bud";
            ultraExplorerBarItem3.Text = "Кредиты от других бюджетов";
            ultraExplorerBarItem4.Key = "6930d45e-89a3-4f28-b1c4-b28502593750";
            ultraExplorerBarItem4.Text = "Гарантии";
            ultraExplorerBarGroup1.Items.AddRange(new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem[] {
            ultraExplorerBarItem1,
            ultraExplorerBarItem2,
            ultraExplorerBarItem3,
            ultraExplorerBarItem4});
            ultraExplorerBarGroup1.Key = "MainGroup";
            ultraExplorerBarGroup1.Settings.AllowDrag = Infragistics.Win.DefaultableBoolean.False;
            ultraExplorerBarGroup1.Settings.AllowEdit = Infragistics.Win.DefaultableBoolean.False;
            ultraExplorerBarGroup1.Settings.AllowItemDrop = Infragistics.Win.DefaultableBoolean.False;
            ultraExplorerBarGroup1.Settings.HeaderVisible = Infragistics.Win.DefaultableBoolean.False;
            ultraExplorerBarGroup1.Text = "New Group";
            ultraExplorerBarGroup2.Expanded = false;
            ultraExplorerBarItem5.Key = "f37827df-c22a-4569-9512-c0c48791d46c";
            ultraExplorerBarItem5.Text = "Вариант.Долговая книга";
            ultraExplorerBarItem6.Key = "f47fee26-165f-42cb-9d3c-4e8d58d58f97";
            ultraExplorerBarItem6.Text = "Протокол передачи данных";
            ultraExplorerBarItem7.Key = "65e95012-39d2-4d96-90ed-ed91775205d4";
            ultraExplorerBarItem7.Text = "Соответсвие районов";
            ultraExplorerBarItem8.Key = "4d192956-aced-4718-a87c-b2e5519c022a";
            ultraExplorerBarItem8.Text = "Должности для отчетов";
            ultraExplorerBarGroup2.Items.AddRange(new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem[] {
            ultraExplorerBarItem5,
            ultraExplorerBarItem6,
            ultraExplorerBarItem7,
            ultraExplorerBarItem8});
            ultraExplorerBarGroup2.Text = "Справочники";
            this.ultraExplorerBar.Groups.AddRange(new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup[] {
            ultraExplorerBarGroup1,
            ultraExplorerBarGroup2});
            this.ultraExplorerBar.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ultraExplorerBar.Location = new System.Drawing.Point(0, 0);
            this.ultraExplorerBar.Name = "ultraExplorerBar";
            this.ultraExplorerBar.ShowDefaultContextMenu = false;
            this.ultraExplorerBar.Size = new System.Drawing.Size(216, 328);
            this.ultraExplorerBar.Style = Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarStyle.VisualStudio2005Toolbox;
            this.ultraExplorerBar.TabIndex = 1;
            this.ultraExplorerBar.ViewStyle = Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarViewStyle.Office2000;
            // 
            // panelVariant
            // 
            this.panelVariant.Controls.Add(this.tbVariantCaption);
            this.panelVariant.Controls.Add(this.btnSelectVariant);
            this.panelVariant.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelVariant.Location = new System.Drawing.Point(0, 349);
            this.panelVariant.Name = "panelVariant";
            this.panelVariant.Size = new System.Drawing.Size(216, 100);
            this.panelVariant.TabIndex = 2;
            // 
            // tbVariantCaption
            // 
            this.tbVariantCaption.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbVariantCaption.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbVariantCaption.Location = new System.Drawing.Point(0, 23);
            this.tbVariantCaption.Multiline = true;
            this.tbVariantCaption.Name = "tbVariantCaption";
            this.tbVariantCaption.ReadOnly = true;
            this.tbVariantCaption.Size = new System.Drawing.Size(216, 77);
            this.tbVariantCaption.TabIndex = 2;
            // 
            // btnSelectVariant
            // 
            appearance2.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnSelectVariant.Appearance = appearance2;
            this.btnSelectVariant.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnSelectVariant.Location = new System.Drawing.Point(0, 0);
            this.btnSelectVariant.Name = "btnSelectVariant";
            this.btnSelectVariant.Size = new System.Drawing.Size(216, 23);
            this.btnSelectVariant.TabIndex = 1;
            this.btnSelectVariant.Text = "Выбрать вариант";
            // 
            // il
            // 
            this.il.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("il.ImageStream")));
            this.il.TransparentColor = System.Drawing.Color.Magenta;
            this.il.Images.SetKeyName(0, "");
            this.il.Images.SetKeyName(1, "");
            this.il.Images.SetKeyName(2, "ExportExcel1.bmp");
            this.il.Images.SetKeyName(3, "Webcontrol_Detailsview.bmp");
            // 
            // BaseNavigationCtrl_Fill_Panel
            // 
            this.BaseNavigationCtrl_Fill_Panel.Controls.Add(this.panel1);
            this.BaseNavigationCtrl_Fill_Panel.Controls.Add(this.panelVariant);
            this.BaseNavigationCtrl_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.BaseNavigationCtrl_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BaseNavigationCtrl_Fill_Panel.Location = new System.Drawing.Point(0, 23);
            this.BaseNavigationCtrl_Fill_Panel.Name = "BaseNavigationCtrl_Fill_Panel";
            this.BaseNavigationCtrl_Fill_Panel.Size = new System.Drawing.Size(216, 449);
            this.BaseNavigationCtrl_Fill_Panel.TabIndex = 0;
            // 
            // cbDataState
            // 
            this.cbDataState.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.cbDataState.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDataState.FormattingEnabled = true;
            this.cbDataState.Location = new System.Drawing.Point(0, 328);
            this.cbDataState.Name = "cbDataState";
            this.cbDataState.Size = new System.Drawing.Size(216, 21);
            this.cbDataState.TabIndex = 3;
            this.cbDataState.SelectedIndexChanged += new System.EventHandler(this.cbDataState_SelectedIndexChanged);
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
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Left.ToolbarsManager = this.ultraToolbarsManager1;
            // 
            // _BaseNavigationCtrl_Toolbars_Dock_Area_Right
            // 
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Right.BackColor = System.Drawing.SystemColors.Control;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(216, 23);
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Right.Name = "_BaseNavigationCtrl_Toolbars_Dock_Area_Right";
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(0, 449);
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Right.ToolbarsManager = this.ultraToolbarsManager1;
            // 
            // _BaseNavigationCtrl_Toolbars_Dock_Area_Top
            // 
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Top.BackColor = System.Drawing.SystemColors.Control;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Top.Name = "_BaseNavigationCtrl_Toolbars_Dock_Area_Top";
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(216, 23);
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Top.ToolbarsManager = this.ultraToolbarsManager1;
            // 
            // _BaseNavigationCtrl_Toolbars_Dock_Area_Bottom
            // 
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.SystemColors.Control;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 472);
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Bottom.Name = "_BaseNavigationCtrl_Toolbars_Dock_Area_Bottom";
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(216, 0);
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Bottom.ToolbarsManager = this.ultraToolbarsManager1;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.ultraExplorerBar);
            this.panel1.Controls.Add(this.cbDataState);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(216, 349);
            this.panel1.TabIndex = 4;
            // 
            // ultraToolbarsManager1
            // 
            appearance24.BackColor = System.Drawing.SystemColors.Control;
            this.ultraToolbarsManager1.Appearance = appearance24;
            this.ultraToolbarsManager1.DesignerFlags = 1;
            this.ultraToolbarsManager1.DockWithinContainer = this;
            this.ultraToolbarsManager1.ImageListSmall = this.il;
            this.ultraToolbarsManager1.LockToolbars = true;
            this.ultraToolbarsManager1.RightAlignedMenus = Infragistics.Win.DefaultableBoolean.False;
            this.ultraToolbarsManager1.RuntimeCustomizationOptions = Infragistics.Win.UltraWinToolbars.RuntimeCustomizationOptions.None;
            this.ultraToolbarsManager1.ShowFullMenusDelay = 500;
            this.ultraToolbarsManager1.ShowQuickCustomizeButton = false;
            ultraToolbar1.DockedColumn = 0;
            ultraToolbar1.DockedRow = 0;
            ultraToolbar1.NonInheritedTools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            popupMenuTool1});
            ultraToolbar1.Text = "UltraToolbar";
            this.ultraToolbarsManager1.Toolbars.AddRange(new Infragistics.Win.UltraWinToolbars.UltraToolbar[] {
            ultraToolbar1});
            appearance13.Image = "transfert.bmp";
            buttonTool3.SharedPropsInternal.AppearancesSmall.Appearance = appearance13;
            buttonTool3.SharedPropsInternal.Caption = "Перенос данных из источников финансирования";
            appearance12.Image = 0;
            popupMenuTool2.SharedPropsInternal.AppearancesSmall.Appearance = appearance12;
            popupMenuTool2.SharedPropsInternal.Caption = "Отчеты долговой книги";
            popupMenuTool2.SharedPropsInternal.DisplayStyle = Infragistics.Win.UltraWinToolbars.ToolDisplayStyle.ImageAndText;
            this.ultraToolbarsManager1.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool3,
            popupMenuTool2});
            // 
            // DebtBookNavigation
            // 
            this.Controls.Add(this.BaseNavigationCtrl_Fill_Panel);
            this.Controls.Add(this._BaseNavigationCtrl_Toolbars_Dock_Area_Left);
            this.Controls.Add(this._BaseNavigationCtrl_Toolbars_Dock_Area_Right);
            this.Controls.Add(this._BaseNavigationCtrl_Toolbars_Dock_Area_Top);
            this.Controls.Add(this._BaseNavigationCtrl_Toolbars_Dock_Area_Bottom);
            this.Name = "DebtBookNavigation";
            ((System.ComponentModel.ISupportInitialize)(this.ultraExplorerBar)).EndInit();
            this.panelVariant.ResumeLayout(false);
            this.panelVariant.PerformLayout();
            this.BaseNavigationCtrl_Fill_Panel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ultraToolbarsManager1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        #region работа с вариантом

        public event EventHandler VariantChanged;

        internal void SetCurrentVariant(int variantID, DataRow variantRow, bool fireVariantChangedEvent)
        {
            //Code, Name, CurrentYear, PlanPeriod, VariantComment
            string[] variantLines = new string[3];
            variantLines[0] = String.Format("({0}) {1}", variantRow["Code"], variantRow["Name"]);
            variantLines[1] = String.Format("Отчетная дата {0}", Convert.ToDateTime(variantRow["ReportDate"]).ToShortDateString());
            variantLines[2] = String.Format("{0}", variantRow["VariantComment"]);

            VariantYear = Convert.ToInt32(variantRow["ActualYear"]);
            IsVariantComplete = Convert.ToBoolean(variantRow["VariantCompleted"]);
            CalculateDate = Convert.ToDateTime(variantRow["ReportDate"]);

            tbVariantCaption.Lines = variantLines;

            CurrentVariantID = variantID;
            CurrentSourceID = services.DataSourceService.GetDataSource(Convert.ToInt32(variantRow["ActualYear"]));

            if (fireVariantChangedEvent)
            {
                OnVariantChanged();
            }
        }

        private void OnVariantChanged()
        {
            SubjectRegionID = GetSubjectRegion();
            SetUserRegion();
            CheckCompliteData();
            FillProtocolData();
            cbDataState.Enabled = CurrentVariantID >= 0;
            if (VariantChanged != null)
            {
                VariantChanged(this, new EventArgs());
            }
        }

        #endregion

        private void cbDataState_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!cbDataState.Enabled)
                return;

            // Если статус не поменялся, то ничего не делаем
            if (currentStatusSchb == cbDataState.SelectedIndex + 1)
                return;

            using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
            {
                db.ExecQuery(
                    "insert into t_S_ProtocolTransfer (DataTransfer, RefStatusSchb, RefRegion, RefVariant) values (?, ?, ?, ?)",
                    QueryResultTypes.NonQuery,
                    new DbParameterDescriptor("DataTransfer", DateTime.Now, DbType.DateTime),
                    new DbParameterDescriptor("RefStatusSchb", cbDataState.SelectedIndex + 1),
                    new DbParameterDescriptor("RefRegion", CurrentRegion),
                    new DbParameterDescriptor("RefVariant", CurrentVariantID));
                
                currentStatusSchb = cbDataState.SelectedIndex + 1;
            }
        }

        private void CheckCompliteData()
        {
            using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
            {
                DataTable queryResult = (DataTable)db.ExecQuery(
                    "select RefStatusSchb from t_S_ProtocolTransfer where RefRegion = ? and RefVariant = ? order by ID desc", 
                    QueryResultTypes.DataTable,
                    new DbParameterDescriptor("RefRegion", CurrentRegion),
                    new DbParameterDescriptor("RefVariant", CurrentVariantID));
                if (queryResult.Rows.Count > 0)
                {
                    currentStatusSchb = Convert.ToInt32(Convert.ToInt32(queryResult.Rows[0]["RefStatusSchb"]));
                    StatusSchbRefresh(currentStatusSchb);
                }
                else
                {
                    currentStatusSchb = 1;
                    StatusSchbRefresh(currentStatusSchb);
                }
                allowEditData = currentStatusSchb == 1;
                
                if (UserRegionType == UserRegionType.Region || UserRegionType == UserRegionType.Town)
                {
                    if (ultraToolbarsManager1.Tools.Exists("CopyRegionDataToVariantCommand"))
                    {
                        ButtonTool button = (ButtonTool) ultraToolbarsManager1.Tools["CopyRegionDataToVariantCommand"];
                        button.SharedProps.Visible = cbDataState.Enabled;
                    }
                }
            }
        }

        private void StatusSchbRefresh(int statusSchb)
        {
            cbDataState.Items.Clear();
            switch (statusSchb)
            {
                case 1:
                    cbDataState.Items.Add("На редактировании");
                    cbDataState.Items.Add("Ввод данных завершен");
                    cbDataState.Enabled = true;
                    break;
                case 2:
                    cbDataState.Items.Add("Ввод данных завершен");
                    cbDataState.Enabled = false;
                    break;
                case 3: 
                    cbDataState.Items.Add("Утверждено");
                    cbDataState.Enabled = false;
                    break;
            }
            cbDataState.SelectedIndexChanged -= cbDataState_SelectedIndexChanged;
            cbDataState.SelectedIndex = 0;
            cbDataState.SelectedIndexChanged += cbDataState_SelectedIndexChanged;
        }

        private void FillProtocolData()
        {
            if (CurrentVariantID == -1)
                return;
            using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
            {
                DataTable dtRegions = (DataTable)db.ExecQuery(
                    "select ID, Name from d_Regions_Analysis where SourceID = ? and (RefTerr = 4 or RefTerr = 7)",
                    QueryResultTypes.DataTable,
                    new DbParameterDescriptor("SourceID", CurrentAnalizSourceID));

                IEntity entity = Workplace.ActiveScheme.RootPackage.FindEntityByName(DomainObjectsKeys.t_S_ProtocolTransfer);

                using (IDataUpdater upd = entity.GetDataUpdater(
                    "RefVariant = ?", null, new DbParameterDescriptor("RefVariant", CurrentVariantID)))
                {
                    DataTable dtProtocol = new DataTable();
                    upd.Fill(ref dtProtocol);
                    if (dtProtocol.Rows.Count != 0)
                        return;
                    foreach (DataRow regionRow in dtRegions.Rows)
                    {
                        DataRow newRow = dtProtocol.NewRow();
                        newRow["ID"] = entity.GetGeneratorNextValue;
                        newRow["RefStatusSchb"] = 1;
                        newRow["RefRegion"] = regionRow["ID"];
                        newRow["RefVariant"] = CurrentVariantID;
                        dtProtocol.Rows.Add(newRow);
                    }
                    upd.Update(ref dtProtocol);
                }
            }
        }
    }
}