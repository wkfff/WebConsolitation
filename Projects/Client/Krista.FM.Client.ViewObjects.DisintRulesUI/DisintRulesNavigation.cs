using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Krista.FM.Client.Common;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Client.ViewObjects.DisintRulesUI.Forms;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.ViewObjects.DisintRulesUI
{
	public partial class DisintRulesNavigation : BaseViewObject.BaseNavigationCtrl
	{
        private static DisintRulesNavigation instance;

        //protected bool 

        internal static DisintRulesNavigation Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DisintRulesNavigation();
                }
                return instance;
            }
        }

        private Dictionary<string, DisintRulesUI> openedViewObjects;
        /// <summary>
        /// показывает, нужно ли обновлять данные нормативов
        /// </summary>
	    internal List<string> refreshNormatives;

        internal Infragistics.Win.UltraWinExplorerBar.UltraExplorerBar ultraExplorerBar1;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsManager ToolBarManager;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseNavigationCtrl_Toolbars_Dock_Area_Left;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseNavigationCtrl_Toolbars_Dock_Area_Right;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseNavigationCtrl_Toolbars_Dock_Area_Top;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _BaseNavigationCtrl_Toolbars_Dock_Area_Bottom;
        private System.ComponentModel.IContainer components;

		public DisintRulesNavigation()
		{
		    instance = this;
            Caption = "Нормативы отчислений доходов";
		}

        public override System.Drawing.Image TypeImage16
        {
            get { return Properties.Resources.pump_Normatives_16; }
        }

        public override System.Drawing.Image TypeImage24
        {
            get { return Properties.Resources.pump_Normatives_24; }
        }

		/// <summary>
		/// Инициализация
		/// </summary>
        public override void Initialize()
		{
            InitializeComponent();

            openedViewObjects = new Dictionary<string, DisintRulesUI>();
            refreshNormatives = new List<string>();

            ultraExplorerBar1.ItemCheckStateChanged += new Infragistics.Win.UltraWinExplorerBar.ItemCheckStateChangedEventHandler(ultraExplorerBar1_ItemCheckStateChanged);

            Workplace.ViewClosed += new Krista.FM.Client.Common.Gui.ViewContentEventHandler(Workplace_ViewClosed);
            Workplace.ActiveWorkplaceWindowChanged += new System.EventHandler(Workplace_ActiveWorkplaceWindowChanged);
            base.Initialize();
		}

        void Workplace_ActiveWorkplaceWindowChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (Workplace.WorkplaceLayout.ActiveContent != null)
                {
                    string key = ((BaseViewObj) Workplace.WorkplaceLayout.ActiveContent).Key;
                    if (!openedViewObjects.ContainsKey(key))
                        return;
                    Workplace.SwitchTo("Нормативы отчислений доходов");
                    if (ultraExplorerBar1.CheckedItem == null)
                        return;
                    if (key != ultraExplorerBar1.CheckedItem.Key)
                    {
                        ultraExplorerBar1.Groups[0].Items[key].Checked = true;
                        ultraExplorerBar1.Groups[0].Items[key].Active = true;
                    }
                }
            }
            catch
            { }
        }

        private void ultraExplorerBar1_ItemCheckStateChanged(object sender, Infragistics.Win.UltraWinExplorerBar.ItemEventArgs e)
        {
            if (e.Item.Checked)
            {
                if (!openedViewObjects.ContainsKey(e.Item.Key))
                {
                    DisintRulesUI viewObject = new DisintRulesUI(e.Item.Key);
                    viewObject.Workplace = Workplace;
                    viewObject.Initialize();
                    //viewObject.LoadData();
                    viewObject.ViewCtrl.Text = e.Item.Text;
                    OnActiveItemChanged(this, viewObject);
                    openedViewObjects.Add(e.Item.Key, viewObject);
                }
                else
                {
                    OnActiveItemChanged(this, openedViewObjects[e.Item.Key]);
                    ((DisintRulesUI) openedViewObjects[e.Item.Key]).BurnGridRefreshData(refreshNormatives.Contains(e.Item.Key));
                }
            }
        }

        private void Workplace_ViewClosed(object sender, Krista.FM.Client.Common.Gui.ViewContentEventArgs e)
        {
            string forRemove = null;
            foreach (KeyValuePair<string, DisintRulesUI> item in openedViewObjects)
            {
                if (item.Value == e.Content)
                {
                    forRemove = item.Key;
                    break;
                }
            }

            if (forRemove != null)
                openedViewObjects.Remove(forRemove);

            if (ultraExplorerBar1.ActiveItem != null && ultraExplorerBar1.ActiveItem.Key == forRemove)
            {
                ultraExplorerBar1.ActiveItem.Checked = false;
                ultraExplorerBar1.ActiveItem.Active = false;
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
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup ultraExplorerBarGroup1 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem1 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DisintRulesNavigation));
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem2 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem3 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem4 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem5 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem6 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.UltraToolbar ultraToolbar1 = new Infragistics.Win.UltraWinToolbars.UltraToolbar("UltraToolbar1");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Infragistics.Win.UltraWinToolbars.ButtonTool("NormativesTransfert");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Infragistics.Win.UltraWinToolbars.ButtonTool("NormativesTransfert");
            Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
            this.ultraExplorerBar1 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBar();
            this.ToolBarManager = new Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(this.components);
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            ((System.ComponentModel.ISupportInitialize)(this.ultraExplorerBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ToolBarManager)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraExplorerBar1
            // 
            this.ultraExplorerBar1.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.ultraExplorerBar1.Dock = System.Windows.Forms.DockStyle.Fill;
            ultraExplorerBarItem1.Key = "1D4678A6-0167-4a01-A8F6-3E2F30233592";
            ultraExplorerBarItem1.Settings.AllowDragCopy = Infragistics.Win.UltraWinExplorerBar.ItemDragStyle.None;
            ultraExplorerBarItem1.Settings.AllowDragMove = Infragistics.Win.UltraWinExplorerBar.ItemDragStyle.None;
            ultraExplorerBarItem1.Settings.AllowEdit = Infragistics.Win.DefaultableBoolean.False;
            appearance1.Image = ((object)(resources.GetObject("appearance1.Image")));
            ultraExplorerBarItem1.Settings.AppearancesSmall.Appearance = appearance1;
            ultraExplorerBarItem1.Text = "Нормативы для закачки данных";
            ultraExplorerBarItem2.Key = "953D18C8-5C58-4797-B046-9964A1AC97D3";
            appearance2.Image = ((object)(resources.GetObject("appearance2.Image")));
            ultraExplorerBarItem2.Settings.AppearancesSmall.Appearance = appearance2;
            ultraExplorerBarItem2.Text = "По Бюджетному кодексу РФ";
            ultraExplorerBarItem3.Key = "D357705E-5594-4f2a-8E78-0872DDEC54E7";
            appearance3.Image = ((object)(resources.GetObject("appearance3.Image")));
            ultraExplorerBarItem3.Settings.AppearancesSmall.Appearance = appearance3;
            ultraExplorerBarItem3.Text = "Субъекта РФ";
            ultraExplorerBarItem4.Key = "97D945A2-75C7-4301-AFEF-20CE62F30F48";
            appearance4.Image = global::Krista.FM.Client.ViewObjects.DisintRulesUI.Properties.Resources.VSProject_genericfile;
            ultraExplorerBarItem4.Settings.AppearancesSmall.Appearance = appearance4;
            ultraExplorerBarItem4.Text = "Муниципального района";
            ultraExplorerBarItem5.Key = "61C6D67E-66E5-47c4-BB8E-3B5478975D96";
            appearance5.Image = ((object)(resources.GetObject("appearance5.Image")));
            ultraExplorerBarItem5.Settings.AppearancesSmall.Appearance = appearance5;
            ultraExplorerBarItem5.Text = "Дифференцированные субъекта РФ";
            ultraExplorerBarItem6.Key = "334845A1-FC38-43da-9AB8-AB94AA79FBFE";
            appearance6.Image = ((object)(resources.GetObject("appearance6.Image")));
            ultraExplorerBarItem6.Settings.AppearancesSmall.Appearance = appearance6;
            ultraExplorerBarItem6.Text = "Дифференцированные муниципального района";
            ultraExplorerBarGroup1.Items.AddRange(new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem[] {
            ultraExplorerBarItem1,
            ultraExplorerBarItem2,
            ultraExplorerBarItem3,
            ultraExplorerBarItem4,
            ultraExplorerBarItem5,
            ultraExplorerBarItem6});
            ultraExplorerBarGroup1.Settings.AllowDrag = Infragistics.Win.DefaultableBoolean.False;
            ultraExplorerBarGroup1.Settings.AllowEdit = Infragistics.Win.DefaultableBoolean.False;
            ultraExplorerBarGroup1.Settings.AllowItemDrop = Infragistics.Win.DefaultableBoolean.False;
            ultraExplorerBarGroup1.Settings.HeaderVisible = Infragistics.Win.DefaultableBoolean.False;
            ultraExplorerBarGroup1.Text = "New Group";
            this.ultraExplorerBar1.Groups.AddRange(new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup[] {
            ultraExplorerBarGroup1});
            this.ultraExplorerBar1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ultraExplorerBar1.Location = new System.Drawing.Point(0, 27);
            this.ultraExplorerBar1.Name = "ultraExplorerBar1";
            this.ultraExplorerBar1.ShowDefaultContextMenu = false;
            this.ultraExplorerBar1.Size = new System.Drawing.Size(278, 445);
            this.ultraExplorerBar1.Style = Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarStyle.VisualStudio2005Toolbox;
            this.ultraExplorerBar1.TabIndex = 0;
            this.ultraExplorerBar1.ViewStyle = Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarViewStyle.Office2000;
            // 
            // ToolBarManager
            // 
            this.ToolBarManager.DesignerFlags = 1;
            this.ToolBarManager.DockWithinContainer = this;
            this.ToolBarManager.ShowFullMenusDelay = 500;
            ultraToolbar1.DockedColumn = 0;
            ultraToolbar1.DockedRow = 0;
            ultraToolbar1.NonInheritedTools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool1});
            ultraToolbar1.Text = "UltraToolbar1";
            this.ToolBarManager.Toolbars.AddRange(new Infragistics.Win.UltraWinToolbars.UltraToolbar[] {
            ultraToolbar1});
            appearance17.Image = ((object)(resources.GetObject("appearance17.Image")));
            buttonTool2.SharedPropsInternal.AppearancesSmall.Appearance = appearance17;
            buttonTool2.SharedPropsInternal.Caption = "Перенос нормативов из расчeта МБТ";
            this.ToolBarManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool2});
            this.ToolBarManager.ToolClick += new Infragistics.Win.UltraWinToolbars.ToolClickEventHandler(this.ultraToolbarsManager1_ToolClick);
            // 
            // _BaseNavigationCtrl_Toolbars_Dock_Area_Left
            // 
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Left.BackColor = System.Drawing.SystemColors.Window;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Left.Location = new System.Drawing.Point(0, 27);
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Left.Name = "_BaseNavigationCtrl_Toolbars_Dock_Area_Left";
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(0, 445);
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Left.ToolbarsManager = this.ToolBarManager;
            // 
            // _BaseNavigationCtrl_Toolbars_Dock_Area_Right
            // 
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Right.BackColor = System.Drawing.SystemColors.Window;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(278, 27);
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Right.Name = "_BaseNavigationCtrl_Toolbars_Dock_Area_Right";
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(0, 445);
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Right.ToolbarsManager = this.ToolBarManager;
            // 
            // _BaseNavigationCtrl_Toolbars_Dock_Area_Top
            // 
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Top.BackColor = System.Drawing.SystemColors.Window;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Top.Name = "_BaseNavigationCtrl_Toolbars_Dock_Area_Top";
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(278, 27);
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Top.ToolbarsManager = this.ToolBarManager;
            // 
            // _BaseNavigationCtrl_Toolbars_Dock_Area_Bottom
            // 
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.SystemColors.Window;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 472);
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Bottom.Name = "_BaseNavigationCtrl_Toolbars_Dock_Area_Bottom";
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(278, 0);
            this._BaseNavigationCtrl_Toolbars_Dock_Area_Bottom.ToolbarsManager = this.ToolBarManager;
            // 
            // DisintRulesNavigation
            // 
            this.Controls.Add(this.ultraExplorerBar1);
            this.Controls.Add(this._BaseNavigationCtrl_Toolbars_Dock_Area_Left);
            this.Controls.Add(this._BaseNavigationCtrl_Toolbars_Dock_Area_Right);
            this.Controls.Add(this._BaseNavigationCtrl_Toolbars_Dock_Area_Top);
            this.Controls.Add(this._BaseNavigationCtrl_Toolbars_Dock_Area_Bottom);
            this.Name = "DisintRulesNavigation";
            this.Size = new System.Drawing.Size(278, 472);
            ((System.ComponentModel.ISupportInitialize)(this.ultraExplorerBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ToolBarManager)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		public override void Customize()
		{
			ComponentCustomizer.CustomizeInfragisticsComponents(components);
			base.Customize();
		}

        private void ultraToolbarsManager1_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {
            string toolKey = e.Tool.Key;
            switch (toolKey)
            {
                case "NormativesTransfert":
                    NormativesTransfert();
                    break;
            }
        }

        private void NormativesTransfert()
        {
            int variant = 0;
            int marks = 0;
            string code = string.Empty;
            string name = string.Empty;
            if (frmTransfertNormatives.ShowTransfertParams(ref variant, ref marks, ref code, ref name))
            {
                Workplace.OperationObj.Text = "Перенос данных из блока 'ФО_0009_ФОНДЫ'";
                Workplace.OperationObj.StartOperation();
                try
                {
                    string warningMessages = string.Empty;
                    Workplace.ActiveScheme.DisintRules.FundDataTransfert(variant, marks, code, name, ref warningMessages);
                    Workplace.OperationObj.StopOperation();
                    if (!string.IsNullOrEmpty(warningMessages))
                    {
                        MessageBox.Show(warningMessages, "Перенос данных", MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                    }
                    DisintRulesUI normatives = Workplace.ActiveContent as DisintRulesUI;
                    if (normatives != null && normatives.currentNormatives == NormativesKind.VarNormativesRegionRF)
                    {
                        normatives.RefreshData();
                    }
                }
                catch (Exception)
                {
                    Workplace.OperationObj.StopOperation();
                    throw;
                }
            }
        }
	}

    public static class NormativesKeys
    {
        public const string AllNormatives = "1D4678A6-0167-4a01-A8F6-3E2F30233592";
        public const string NormativesBK = "953D18C8-5C58-4797-B046-9964A1AC97D3";
        public const string NormativesRegionRF = "D357705E-5594-4f2a-8E78-0872DDEC54E7";
        public const string NormativesMR = "97D945A2-75C7-4301-AFEF-20CE62F30F48";
        public const string VarNormativesRegionRF = "61C6D67E-66E5-47c4-BB8E-3B5478975D96";
        public const string VarNormativesMR = "334845A1-FC38-43da-9AB8-AB94AA79FBFE";
    }
}