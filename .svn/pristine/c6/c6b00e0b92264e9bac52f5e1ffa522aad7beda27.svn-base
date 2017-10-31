using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Krista.FM.Client.Common;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Infragistics.Win.Misc;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.ViewObjects.ProtocolsUI
{
    public class ProtocolsNavigation : BaseNavigationCtrl, IProtocolNavigation
    {
        private static ProtocolsNavigation instance;

        internal static ProtocolsNavigation Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ProtocolsNavigation();
                }
                return instance;
            }
        }

        private Dictionary<string, ProtocolsViewObject> openedViewObjects;


        private System.ComponentModel.Container components = null;
        private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor udteBeginPeriodArch;
        private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor udteEndPeriodArch;
        private Panel pnlSaveToArch;
        private UltraLabel label2;
        private UltraLabel label1;
        private Infragistics.Win.UltraWinExplorerBar.UltraExplorerBar uebNavi;
        private Button btnSaveToArchive;

        
        public ProtocolsNavigation()
        {
            instance = this;
            Caption = "Протоколы";
        }

        public override System.Drawing.Image TypeImage16
        {
            get { return Properties.Resources.log_Main_16; }
        }

        public override System.Drawing.Image TypeImage24
        {
            get { return Properties.Resources.log_Main_24; }
        }

        /// <summary>
        /// Инициализация объкта навигации.
        /// </summary>
        public override void Initialize()
        {
            InitializeComponent();

            openedViewObjects = new Dictionary<string, ProtocolsViewObject>();

            uebNavi.ItemCheckStateChanged += new Infragistics.Win.UltraWinExplorerBar.ItemCheckStateChangedEventHandler(ProtocolsNavi_ItemCheckStateChanged);
            Workplace.ViewClosed += new Krista.FM.Client.Common.Gui.ViewContentEventHandler(Workplace_ViewClosed);
            Workplace.ActiveWorkplaceWindowChanged += new EventHandler(Workplace_ActiveWorkplaceWindowChanged);

            udteEndPeriodArch.Value = DateTime.Today;

            SetSelectedMinDate();

            base.Initialize();
        }

        // гвиды, на которые в будущем планируется перевести ключи для закладок протоколов
        // {AE403EC0-A351-483b-993B-547F661CC5C6}

        // {F2D807F1-23B6-4046-A63B-977F7B3AAEA5}

        // {79B926B3-8B3C-43c9-A60D-6C3709D3FC11}

        // {4B3E42F1-98E1-4bc0-899E-69F4B7BD9E15}

        // {3CCC266A-8104-4d46-9E83-5734B2D1BB7D}

        // {44E19333-6E1B-44c5-8B9C-E3DBF7F2018F}

        // {2DA4065E-B033-4ece-BBBA-0947E0026D2D}

        // {AE638547-63BB-4b39-A829-5E2E81FF68B3}

        // {BA04BECE-0448-43f6-BBE8-4D78016743E7}

        // {79BC4CE2-8791-48d9-A33D-DAFC3B77634C}

        void Workplace_ActiveWorkplaceWindowChanged(object sender, EventArgs e)
        {
            if (Workplace.WorkplaceLayout.ActiveContent != null)
            {
                string key = ((BaseViewObj)Workplace.WorkplaceLayout.ActiveContent).Key;
                if (openedViewObjects.ContainsKey(key))
                {
                    ModulesTypes modulesType = (ModulesTypes)Enum.Parse(typeof(ModulesTypes), key);
                    Workplace.SwitchTo("Протоколы");
                    ProtocolsViewObject viewObject = openedViewObjects[key];

                    if (uebNavi.CheckedItem == null)
                        return;
                    if (key != uebNavi.CheckedItem.Key && uebNavi.Groups[0].Items.Exists(key))
                    {
                        uebNavi.Groups[0].Items[key].Checked = true;
                        uebNavi.Groups[0].Items[key].Active = true;
                    }
                }
            }
        }

        void Workplace_ViewClosed(object sender, Krista.FM.Client.Common.Gui.ViewContentEventArgs e)
        {
            string forRemove = string.Empty;
            foreach (KeyValuePair<string, ProtocolsViewObject> item in openedViewObjects)
            {
                if (item.Value == e.Content)
                {
                    forRemove = item.Key;
                    break;
                }
            }

            if (!string.IsNullOrEmpty(forRemove))
            {
                try
                {
                    openedViewObjects[forRemove].Dispose();
                }
                finally
                {
                    openedViewObjects.Remove(forRemove);
                }
            }

            if (uebNavi.CheckedItem == null)
                return;
            if (forRemove == null)
                 return;

            //ModulesTypes modulesType = (ModulesTypes)Enum.Parse(typeof(ModulesTypes), uebNavi.CheckedItem.Key);
             if (forRemove == uebNavi.CheckedItem.Key)
            {
                uebNavi.CheckedItem.Active = false;
                uebNavi.CheckedItem.Checked = false;
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
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup ultraExplorerBarGroup1 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem1 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProtocolsNavigation));
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem2 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem3 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem4 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem5 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem6 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem7 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem8 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem9 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem10 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem11 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem12 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem13 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem14 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem15 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            this.pnlSaveToArch = new System.Windows.Forms.Panel();
            this.label2 = new Infragistics.Win.Misc.UltraLabel();
            this.label1 = new Infragistics.Win.Misc.UltraLabel();
            this.btnSaveToArchive = new System.Windows.Forms.Button();
            this.udteEndPeriodArch = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.udteBeginPeriodArch = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.uebNavi = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBar();
            this.pnlSaveToArch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udteEndPeriodArch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udteBeginPeriodArch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uebNavi)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlSaveToArch
            // 
            this.pnlSaveToArch.AutoSize = true;
            this.pnlSaveToArch.BackColor = System.Drawing.SystemColors.MenuBar;
            this.pnlSaveToArch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlSaveToArch.Controls.Add(this.label2);
            this.pnlSaveToArch.Controls.Add(this.label1);
            this.pnlSaveToArch.Controls.Add(this.btnSaveToArchive);
            this.pnlSaveToArch.Controls.Add(this.udteEndPeriodArch);
            this.pnlSaveToArch.Controls.Add(this.udteBeginPeriodArch);
            this.pnlSaveToArch.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlSaveToArch.Location = new System.Drawing.Point(0, 335);
            this.pnlSaveToArch.Name = "pnlSaveToArch";
            this.pnlSaveToArch.Size = new System.Drawing.Size(234, 65);
            this.pnlSaveToArch.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(112, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 14);
            this.label2.TabIndex = 19;
            this.label2.Text = "по";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(10, 14);
            this.label1.TabIndex = 18;
            this.label1.Text = "с";
            // 
            // btnSaveToArchive
            // 
            this.btnSaveToArchive.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnSaveToArchive.Location = new System.Drawing.Point(9, 36);
            this.btnSaveToArchive.Name = "btnSaveToArchive";
            this.btnSaveToArchive.Size = new System.Drawing.Size(212, 24);
            this.btnSaveToArchive.TabIndex = 17;
            this.btnSaveToArchive.Text = "Архивировать протоколы";
            this.btnSaveToArchive.UseVisualStyleBackColor = true;
            this.btnSaveToArchive.Click += new System.EventHandler(this.SaveToArchive_Click);
            // 
            // udteEndPeriodArch
            // 
            this.udteEndPeriodArch.DisplayStyle = Infragistics.Win.EmbeddableElementDisplayStyle.VisualStudio2005;
            this.udteEndPeriodArch.Location = new System.Drawing.Point(133, 11);
            this.udteEndPeriodArch.Name = "udteEndPeriodArch";
            this.udteEndPeriodArch.Size = new System.Drawing.Size(88, 21);
            this.udteEndPeriodArch.TabIndex = 16;
            // 
            // udteBeginPeriodArch
            // 
            this.udteBeginPeriodArch.ButtonStyle = Infragistics.Win.UIElementButtonStyle.ButtonSoft;
            this.udteBeginPeriodArch.DisplayStyle = Infragistics.Win.EmbeddableElementDisplayStyle.VisualStudio2005;
            this.udteBeginPeriodArch.Location = new System.Drawing.Point(23, 11);
            this.udteBeginPeriodArch.Name = "udteBeginPeriodArch";
            this.udteBeginPeriodArch.Size = new System.Drawing.Size(88, 21);
            this.udteBeginPeriodArch.TabIndex = 15;
            // 
            // uebNavi
            // 
            this.uebNavi.AcceptsFocus = Infragistics.Win.DefaultableBoolean.True;
            this.uebNavi.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uebNavi.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            ultraExplorerBarItem1.Key = "UsersOperationsModule";
            ultraExplorerBarItem1.Settings.AllowDragCopy = Infragistics.Win.UltraWinExplorerBar.ItemDragStyle.None;
            ultraExplorerBarItem1.Settings.AllowDragMove = Infragistics.Win.UltraWinExplorerBar.ItemDragStyle.None;
            appearance1.Image = ((object)(resources.GetObject("appearance1.Image")));
            ultraExplorerBarItem1.Settings.AppearancesSmall.Appearance = appearance1;
            ultraExplorerBarItem1.Text = "Действия пользователей";
            ultraExplorerBarItem2.Key = "ProcessDataModule";
            ultraExplorerBarItem2.Text = "Обработка данных";
            ultraExplorerBarItem3.Key = "DataPumpModule";
            ultraExplorerBarItem3.Settings.AllowDragCopy = Infragistics.Win.UltraWinExplorerBar.ItemDragStyle.None;
            ultraExplorerBarItem3.Settings.AllowDragMove = Infragistics.Win.UltraWinExplorerBar.ItemDragStyle.None;
            appearance2.Image = ((object)(resources.GetObject("appearance2.Image")));
            ultraExplorerBarItem3.Settings.AppearancesSmall.Appearance = appearance2;
            ultraExplorerBarItem3.Text = "Закачка данных";
            ultraExplorerBarItem4.Key = "BridgeOperationsModule";
            ultraExplorerBarItem4.Settings.AllowDragCopy = Infragistics.Win.UltraWinExplorerBar.ItemDragStyle.None;
            ultraExplorerBarItem4.Settings.AllowDragMove = Infragistics.Win.UltraWinExplorerBar.ItemDragStyle.None;
            appearance3.Image = ((object)(resources.GetObject("appearance3.Image")));
            ultraExplorerBarItem4.Settings.AppearancesSmall.Appearance = appearance3;
            ultraExplorerBarItem4.Text = "Сопоставление классификаторов";
            ultraExplorerBarItem5.Key = "ClassifiersModule";
            appearance4.Image = ((object)(resources.GetObject("appearance4.Image")));
            ultraExplorerBarItem5.Settings.AppearancesSmall.Appearance = appearance4;
            ultraExplorerBarItem5.Text = "Классификаторы и таблицы";
            ultraExplorerBarItem6.Key = "MDProcessingModule";
            ultraExplorerBarItem6.Settings.AllowDragCopy = Infragistics.Win.UltraWinExplorerBar.ItemDragStyle.None;
            ultraExplorerBarItem6.Settings.AllowDragMove = Infragistics.Win.UltraWinExplorerBar.ItemDragStyle.None;
            appearance5.Image = ((object)(resources.GetObject("appearance5.Image")));
            ultraExplorerBarItem6.Settings.AppearancesSmall.Appearance = appearance5;
            ultraExplorerBarItem6.Text = "Расчет многомерной базы";
            ultraExplorerBarItem7.Key = "ReviseDataModule";
            ultraExplorerBarItem7.Settings.AllowDragCopy = Infragistics.Win.UltraWinExplorerBar.ItemDragStyle.None;
            ultraExplorerBarItem7.Settings.AllowDragMove = Infragistics.Win.UltraWinExplorerBar.ItemDragStyle.None;
            appearance6.Image = ((object)(resources.GetObject("appearance6.Image")));
            ultraExplorerBarItem7.Settings.AppearancesSmall.Appearance = appearance6;
            ultraExplorerBarItem7.Text = "Сверка данных";
            ultraExplorerBarItem8.Key = "DeleteDataModule";
            ultraExplorerBarItem8.Text = "Удаление данных";
            ultraExplorerBarItem9.Key = "SystemEventsModule";
            ultraExplorerBarItem9.Settings.AllowDragCopy = Infragistics.Win.UltraWinExplorerBar.ItemDragStyle.None;
            ultraExplorerBarItem9.Settings.AllowDragMove = Infragistics.Win.UltraWinExplorerBar.ItemDragStyle.None;
            appearance7.Image = ((object)(resources.GetObject("appearance7.Image")));
            ultraExplorerBarItem9.Settings.AppearancesSmall.Appearance = appearance7;
            ultraExplorerBarItem9.Text = "Системные сообщения";
            ultraExplorerBarItem10.Key = "PreviewDataModule";
            ultraExplorerBarItem10.Text = "Предпросмотр данных";
            ultraExplorerBarItem11.Key = "AuditModule";
            appearance8.Image = ((object)(resources.GetObject("appearance8.Image")));
            ultraExplorerBarItem11.Settings.AppearancesSmall.Appearance = appearance8;
            ultraExplorerBarItem11.Text = "Аудит";
            ultraExplorerBarItem12.Checked = true;
            ultraExplorerBarItem12.Key = "UpdateModule";
            ultraExplorerBarItem12.Text = "Обновление схемы";
            ultraExplorerBarItem13.Key = "DataSourceModule";
            ultraExplorerBarItem13.Text = "Источники данных";
            ultraExplorerBarItem14.Key = "TransferDBToNewYearModule";
            ultraExplorerBarItem14.Text = "Перевод базы на новый год";
            ultraExplorerBarItem15.Key = "MessagesExchangeModule";
            ultraExplorerBarItem15.Text = "Обмен сообщениями";
            ultraExplorerBarGroup1.Items.AddRange(new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem[] {
            ultraExplorerBarItem1,
            ultraExplorerBarItem2,
            ultraExplorerBarItem3,
            ultraExplorerBarItem4,
            ultraExplorerBarItem5,
            ultraExplorerBarItem6,
            ultraExplorerBarItem7,
            ultraExplorerBarItem8,
            ultraExplorerBarItem9,
            ultraExplorerBarItem10,
            ultraExplorerBarItem11,
            ultraExplorerBarItem12,
            ultraExplorerBarItem13,
            ultraExplorerBarItem14,
            ultraExplorerBarItem15});
            ultraExplorerBarGroup1.Text = "New Group";
            this.uebNavi.Groups.AddRange(new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup[] {
            ultraExplorerBarGroup1});
            this.uebNavi.GroupSettings.AllowDrag = Infragistics.Win.DefaultableBoolean.False;
            this.uebNavi.GroupSettings.AllowItemDrop = Infragistics.Win.DefaultableBoolean.False;
            this.uebNavi.GroupSettings.BorderStyleItemArea = Infragistics.Win.UIElementBorderStyle.None;
            this.uebNavi.GroupSettings.HeaderVisible = Infragistics.Win.DefaultableBoolean.False;
            this.uebNavi.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.uebNavi.ItemSettings.AllowDragCopy = Infragistics.Win.UltraWinExplorerBar.ItemDragStyle.None;
            this.uebNavi.ItemSettings.AllowDragMove = Infragistics.Win.UltraWinExplorerBar.ItemDragStyle.None;
            this.uebNavi.Location = new System.Drawing.Point(0, 0);
            this.uebNavi.Name = "uebNavi";
            this.uebNavi.ShowDefaultContextMenu = false;
            this.uebNavi.Size = new System.Drawing.Size(228, 329);
            this.uebNavi.Style = Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarStyle.VisualStudio2005Toolbox;
            this.uebNavi.TabIndex = 0;
            this.uebNavi.ViewStyle = Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarViewStyle.Office2000;
            // 
            // ProtocolsNavigation
            // 
            this.Controls.Add(this.pnlSaveToArch);
            this.Controls.Add(this.uebNavi);
            this.Name = "ProtocolsNavigation";
            this.Size = new System.Drawing.Size(234, 400);
            this.pnlSaveToArch.ResumeLayout(false);
            this.pnlSaveToArch.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udteEndPeriodArch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udteBeginPeriodArch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uebNavi)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        /// <summary>
        /// Обработчик перемещения по модулям
        /// </summary>
        private void ProtocolsNavi_ItemCheckStateChanged(object sender, Infragistics.Win.UltraWinExplorerBar.ItemEventArgs e)
        {
            if (e.Item.Checked)
            {
                ModulesTypes modulesType = (ModulesTypes)Enum.Parse(typeof(ModulesTypes), e.Item.Key);

                ProtocolsViewObject viewObject = null;

                if (!openedViewObjects.ContainsKey(e.Item.Key))
                {
                    viewObject = new ProtocolsViewObject(modulesType);
                    viewObject.Workplace = Workplace;
                    viewObject.Initialize();
                    viewObject.LoadData();
                    OnActiveItemChanged(this, viewObject);
                    openedViewObjects.Add(e.Item.Key, viewObject);
                }
                else
                {
                    viewObject = openedViewObjects[e.Item.Key];
                    OnActiveItemChanged(this, viewObject);
                }

            }
        }

        /// <summary>
        /// получение граничных значений дат протоколов для архивирования
        /// </summary>
        private void SetSelectedMinDate()
        {
            IBaseProtocol ViewProtocol = Workplace.ActiveScheme.GetProtocol(Assembly.GetExecutingAssembly().ManifestModule.Name);
            try
            {
                Workplace.OperationObj.Text = "Запрос данных";
                Workplace.OperationObj.StartOperation(); 
                udteBeginPeriodArch.Value = ViewProtocol.MinProtocolsDate;
                Workplace.OperationObj.StopOperation();
            }
            finally
            {
                if (ViewProtocol != null)
                {
                    ViewProtocol.Dispose();
                }
            }
        }

        private IDbDataParameter[] DateTimeFilterParamsSave = null;
        private IDbDataParameter[] DateTimeFilterParamsDel = null;

        /// <summary>
        /// Удаление заархивированных протоколов
        /// </summary
        private bool RemoveProtocols(DateTime RemoveBeginDate, DateTime RemoveEndDate)
        {
            IBaseProtocol ViewProtocol = this.Workplace.ActiveScheme.GetProtocol(System.Reflection.Assembly.GetExecutingAssembly().ManifestModule.Name);
            IDatabase tmpdelDB = Workplace.ActiveScheme.SchemeDWH.DB;
            bool RemoveResult = true;
            bool DeleteFronDB = true;

            try
            {

                this.Workplace.OperationObj.Text = "Удаление данных";
                this.Workplace.OperationObj.StartOperation();

                /* 1 - Закачка данных  (ОК)
                   2 - Сопоставление классификаторов  (ОК)
                   3 - Расчет многомерной базы  (ОК)
                   4 - Действия пользователей
                   5 - Системные сообщения
                   6 - Сверка данных
                   7 - Обработка данных
                   8 - Удаление сообщения
                   9 - Предпросмотр данных
                   10 - Классификаторы и таблицы
                   11 - Аудит
                   12 - Обновление схемы
                   13 - Источники данных
                */

                for (int val = 1; val <= 13; val++)
                {

                    // удаляем все протоколы, кроме аудита
                    if (val != 11)
                    {
                        DateTimeFilterParamsDel = new IDbDataParameter[3];
                        DateTimeFilterParamsDel[0] = new System.Data.OleDb.OleDbParameter("p0", val);//tmpdelDB.CreateParameter("p0", val, DbType.Int32);
                        DateTimeFilterParamsDel[1] = new System.Data.OleDb.OleDbParameter("p1", RemoveBeginDate);// tmpdelDB.CreateParameter("p1", RemoveBeginDate, DbType.DateTime);
                        DateTimeFilterParamsDel[2] = new System.Data.OleDb.OleDbParameter("p2", RemoveEndDate.AddDays(1));//tmpdelDB.CreateParameter("p2", RemoveEndDate.AddDays(1), DbType.DateTime);

                        string queryStr = String.Format("DELETE FROM HUB_EventProtocol WHERE (ClassOfProtocol = ?) AND (EventDateTime BETWEEN ? AND ?)");
                        DeleteFronDB = ViewProtocol.DeleteProtocolArchive(queryStr, DateTimeFilterParamsDel);

                        if (!DeleteFronDB)
                        {
                            RemoveResult = false;
                        }
                    }

                }
                this.Workplace.OperationObj.StopOperation();
            }
            finally
            {
                tmpdelDB.Dispose();

                if (ViewProtocol != null)
                {
                    ViewProtocol.Dispose();
                    ViewProtocol = null;
                }
            }
            return (RemoveResult);
        }

        /// <summary>
        /// Сохранение данных в архив XML
        /// </summary>
        private void SaveToArchive_Click(object sender, EventArgs e)
        {
            string systemArchiveDirectory = Workplace.ActiveScheme.ArchiveDirectory;
            if (!Directory.Exists(systemArchiveDirectory))
            {
                MessageBox.Show(string.Format("Сетевой '{0}' путь не найден", systemArchiveDirectory),
                    "Архивирование протоколов", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            IBaseProtocol ViewProtocol = this.Workplace.ActiveScheme.GetProtocol(System.Reflection.Assembly.GetExecutingAssembly().ManifestModule.Name);
            IUsersOperationProtocol protocol = (IUsersOperationProtocol)this.Workplace.ActiveScheme.GetProtocol("Workplace.exe");
            IDatabase tmpfnDB = Workplace.ActiveScheme.SchemeDWH.DB;

            // если начальная дата больше конечной меняем их местами
            if (udteBeginPeriodArch.DateTime > udteEndPeriodArch.DateTime)
            {
                DateTime tmpDate = udteBeginPeriodArch.DateTime;
                udteBeginPeriodArch.Value = udteEndPeriodArch.Value;
                udteEndPeriodArch.Value = tmpDate;
            }

            DateTime fnBeginDate = udteBeginPeriodArch.DateTime;
            DateTime fnEndDate = udteEndPeriodArch.DateTime;

            string ArchiveCatalogName = String.Empty;
            string FullFileName = String.Empty;

            string msgText = String.Empty;
            string DateFilterQuery = "(dpp.EventDateTime between ? and ?)";

            DataTable LogForSave = new DataTable();
            ModulesTypes arcProtocol;

            string QuesStr = String.Format("Протоколы с {0} по {1} будут сохранены в архив и удалены из базы данных. Выполнить данную операцию ? ",
                fnBeginDate.ToShortDateString(), fnEndDate.ToShortDateString());

            if (MessageBox.Show(QuesStr, "Архивирование протоколов", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    this.Workplace.OperationObj.Text = "Сохранение данных";
                    this.Workplace.OperationObj.StartOperation();

                    ArchiveCatalogName = String.Format("{0}\\{1} Архив протоколов с {2} по {3} ", systemArchiveDirectory,
                        DateTime.Now.ToString("yyyyMMdd HH-mm-ss"), fnBeginDate.ToString("yyyyMMdd"), fnEndDate.ToString("yyyyMMdd"));

                    foreach (int value in Enum.GetValues(typeof(ModulesTypes)))
                    {
                        string name = Enum.GetName(typeof(ModulesTypes), value);
                        arcProtocol = (ModulesTypes)Enum.Parse(typeof(ModulesTypes), name);

                        if (arcProtocol != ModulesTypes.AuditModule)
                        {
                            DateTimeFilterParamsSave = new IDbDataParameter[2];
                            DateTimeFilterParamsSave[0] = tmpfnDB.CreateParameter("p0", fnBeginDate, DbType.DateTime);
                            DateTimeFilterParamsSave[1] = tmpfnDB.CreateParameter("p1", fnEndDate.AddDays(1), DbType.DateTime);
                            ViewProtocol.GetProtocolData(arcProtocol, ref LogForSave, DateFilterQuery, DateTimeFilterParamsSave);
                            // если выборка не пустая
                            if (LogForSave.Rows.Count != 0)
                            {
                                // получение описания типа протокола
                                FieldInfo fi = typeof(ModulesTypes).GetField(name);
                                DescriptionAttribute da = (DescriptionAttribute)Attribute.GetCustomAttribute(fi, typeof(DescriptionAttribute));

                                // копирование таблицы в DataSet
                                DataSet ds = new DataSet(arcProtocol.ToString());
                                DataTable dt = LogForSave.Copy();
                                ds.Tables.Add(dt);

                                // сохранение 
                                if (!Directory.Exists(ArchiveCatalogName))
                                    Directory.CreateDirectory(ArchiveCatalogName);
                                FullFileName = String.Format("{0}\\{1} {2} {3}.xml", ArchiveCatalogName,
                                    fnBeginDate.ToString("yyyyMMdd"),
                                    fnEndDate.ToString("yyyyMMdd"), da.Description);
                                XmlTextWriter writer = new XmlTextWriter(FullFileName, Encoding.GetEncoding(1251));
                                writer.Formatting = Formatting.Indented;
                                writer.IndentChar = ('\t');
                                writer.WriteStartDocument(true);
                                ds.WriteXml(writer, XmlWriteMode.IgnoreSchema);
                                writer.BaseStream.Dispose();
                                ds.Dispose();
                                LogForSave.Clear();
                            }
                        }
                    }

                    LogForSave.Dispose();


                    // Запись в протокол
                    if (RemoveProtocols(fnBeginDate, fnEndDate))
                        msgText = String.Format("Протоколы с {0} по {1} сохранены в каталог '{2}' и успешно удалены из базы.",
                            fnBeginDate.ToShortDateString(), fnEndDate.ToShortDateString(), ArchiveCatalogName);
                    else
                        msgText = String.Format("Протоколы с {0} по {1} сохранены в каталог '{2}'. Во время удаления из базы произошла ошибка.",
                            fnBeginDate.ToShortDateString(), fnEndDate.ToShortDateString(), ArchiveCatalogName);

                    UsersOperationEventKind kind = UsersOperationEventKind.uoeProtocolsToArchive;
                    protocol.WriteEventIntoUsersOperationProtocol(kind, msgText, SystemInformation.ComputerName);

                    ViewProtocol.GetProtocolsDate(ref fnBeginDate, ref fnEndDate);
                    udteBeginPeriodArch.Value = fnBeginDate;
                    udteEndPeriodArch.Value = fnEndDate;
                    this.Workplace.OperationObj.StopOperation();
                    MessageBox.Show(msgText, "Архивирование протоколов", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                finally
                {
                    
                    if (ViewProtocol != null)
                    {
                        ViewProtocol.Dispose();
                        ViewProtocol = null;
                    }
                    if (protocol != null)
                        protocol.Dispose();

                    tmpfnDB.Dispose();
                }
            }
        }
        
        public override void Customize()
		{
			ComponentCustomizer.CustomizeInfragisticsComponents(this.components);
			base.Customize();
		}

        public IInplaceProtocolView ProtocolsInplacer()
        {
            ProtocolsViewObject pvo = new ProtocolsViewObject(ModulesTypes.UsersOperationsModule);
            pvo.Workplace = this.Workplace;
            pvo.InInplaceMode = true;
            pvo.Initialize();
            return (IInplaceProtocolView)pvo;
        }
    }
}
