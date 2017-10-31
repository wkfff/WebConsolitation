using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using Infragistics.Win.UltraWinExplorerBar;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.AssociatedCls;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Client.Workplace.Gui;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.ReportsUI.Gui
{
    public class ReportsNavigation : BaseNavigationCtrl
    {
        public ReportsNavigation()
        {
            Caption = "Отчеты системы";
            InitializeComponent();
            instance = this;
        }

        public override Image TypeImage16
        {
            get { return Properties.Resources.reports_icon_16; }
        }

        public override Image TypeImage24
        {
            get { return Properties.Resources.reports_icon_24; }
        }

        private ReportsNavigation instance;
        public ReportsNavigation Instance
        {
            get
            {
                if (instance == null)
                    instance = new ReportsNavigation();
                return instance;
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            Workplace.ViewClosed += Workplace_ViewClosed;
            Workplace.ActiveWorkplaceWindowChanged += Workplace_ActiveWorkplaceWindowChanged;
            ebNavigation.ItemCheckStateChanged += ebNavigation_ItemCheckStateChanged;
            DataTable dtGroups = Workplace.ActiveScheme.UsersManager.GetGroupsForUser(Workplace.ActiveScheme.UsersManager.GetCurrentUserID());
            DataRow[] groups = dtGroups.Select("ID = 1");
            ebNavigation.Groups[0].Items["ReportsConstructor"].Visible = Convert.ToBoolean(groups[0]["IsMember"]);
        }

        #region события визуальных компонентов области навигации

        void ebNavigation_ItemCheckStateChanged(object sender, ItemEventArgs e)
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
                    BaseViewObj viewObject = GetViewObject(e.Item.Key);
                    viewObject.Workplace = Workplace;
                    viewObject.Initialize();
                    viewObject.ViewCtrl.Text = e.Item.Text;
                    OnActiveItemChanged(this, viewObject);
                    viewObject.InitializeData();
                }
            }
        }

        #endregion

        private BaseViewObj GetViewObject(string key)
        {
            BaseViewObj viewObj = null;

            switch (key)
            {
                case "ReportsUI":
                    viewObj = new ReportsUI(key);
                    break;
                case "b_Org_EGRUL":
                    IEntity cls =
                        Workplace.ActiveScheme.RootPackage.FindEntityByName("7473679b-3ebb-43ca-999d-7f8fdd3efb34");
                    if (cls != null)
                        viewObj = new EgrulClsUI(cls, key);
                    break;
                case "b_IP_EGRIP":
                    cls =
                        Workplace.ActiveScheme.RootPackage.FindEntityByName("e7ac5579-1974-4a9f-8d83-80a8beace782");
                    if (cls != null)
                        viewObj = new EgripClsUI(cls, key);
                    break;
                case "ReportsConstructor":
                    viewObj = new ReportsConstructorUI(key);
                    break;
            }
            return viewObj;
        }

        private void Workplace_ActiveWorkplaceWindowChanged(object sender, EventArgs e)
        {
            if (Workplace.WorkplaceLayout.ActiveContent != null)
            {
                string key = ((BaseViewObj)Workplace.WorkplaceLayout.ActiveContent).Key;
                UltraExplorerBarGroup group = null;
                foreach (UltraExplorerBarGroup groupBar in ebNavigation.Groups)
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
                Workplace.SwitchTo("Отчеты системы");
                if (ebNavigation.CheckedItem == null)
                    return;
                if (key != ebNavigation.CheckedItem.Key && group.Items[key].Visible)
                {
                    group.Items[key].Checked = true;
                    group.Items[key].Active = true;
                }
            }
        }

        private void Workplace_ViewClosed(object sender, ViewContentEventArgs e)
        {
            if (ebNavigation.CheckedItem == null)
                return;

            if (e.Content.Key == ebNavigation.CheckedItem.Key)
            {
                ebNavigation.CheckedItem.Active = false;
                ebNavigation.CheckedItem.Checked = false;
            }
        }


        private void InitializeComponent()
        {
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup ultraExplorerBarGroup1 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem1 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem2 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup ultraExplorerBarGroup2 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup ultraExplorerBarGroup3 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem4 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem ultraExplorerBarItem5 = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem();
            this.ebNavigation = new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBar();
            ((System.ComponentModel.ISupportInitialize)(this.ebNavigation)).BeginInit();
            this.SuspendLayout();
            // 
            // ebNavigation
            // 
            this.ebNavigation.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.ebNavigation.Dock = System.Windows.Forms.DockStyle.Fill;
            ultraExplorerBarItem1.Key = "ReportsUI";
            ultraExplorerBarItem1.Text = "Отчеты";
            ultraExplorerBarItem2.Key = "ReportsConstructor";
            ultraExplorerBarItem2.Text = "Конструктор отчетов";
            ultraExplorerBarGroup1.Items.AddRange(new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem[] {
            ultraExplorerBarItem1,
            ultraExplorerBarItem2});
            ultraExplorerBarGroup1.Key = "MainGroup";
            ultraExplorerBarGroup1.Settings.AllowDrag = Infragistics.Win.DefaultableBoolean.False;
            ultraExplorerBarGroup1.Settings.AllowEdit = Infragistics.Win.DefaultableBoolean.False;
            ultraExplorerBarGroup1.Settings.AllowItemDrop = Infragistics.Win.DefaultableBoolean.False;
            ultraExplorerBarGroup1.Settings.HeaderVisible = Infragistics.Win.DefaultableBoolean.False;
            ultraExplorerBarGroup1.Text = "Отчеты системы";
            ultraExplorerBarGroup2.Expanded = false;
            ultraExplorerBarGroup2.Settings.AllowDrag = Infragistics.Win.DefaultableBoolean.False;
            ultraExplorerBarGroup2.Settings.AllowEdit = Infragistics.Win.DefaultableBoolean.False;
            ultraExplorerBarGroup2.Settings.AllowItemDrop = Infragistics.Win.DefaultableBoolean.False;
            ultraExplorerBarGroup2.Settings.HeaderVisible = Infragistics.Win.DefaultableBoolean.False;
            ultraExplorerBarGroup2.Text = "Справочники";
            ultraExplorerBarItem4.Key = "b_Org_EGRUL";
            ultraExplorerBarItem4.Text = "ЕГРЮЛ";
            ultraExplorerBarItem5.Key = "b_IP_EGRIP";
            ultraExplorerBarItem5.Text = "ЕГРИП";
            ultraExplorerBarGroup3.Items.AddRange(new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarItem[] {
            ultraExplorerBarItem4,
            ultraExplorerBarItem5});
            ultraExplorerBarGroup3.Text = "Единые государственные реестры";
            this.ebNavigation.Groups.AddRange(new Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarGroup[] {
            ultraExplorerBarGroup1,
            ultraExplorerBarGroup2,
            ultraExplorerBarGroup3});
            this.ebNavigation.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ebNavigation.Location = new System.Drawing.Point(0, 0);
            this.ebNavigation.Name = "ebNavigation";
            this.ebNavigation.ShowDefaultContextMenu = false;
            this.ebNavigation.Size = new System.Drawing.Size(216, 472);
            this.ebNavigation.Style = Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarStyle.VisualStudio2005Toolbox;
            this.ebNavigation.TabIndex = 2;
            this.ebNavigation.ViewStyle = Infragistics.Win.UltraWinExplorerBar.UltraExplorerBarViewStyle.Office2000;
            this.ebNavigation.ItemClick += new Infragistics.Win.UltraWinExplorerBar.ItemClickEventHandler(this.ebNavigation_ItemClick);
            // 
            // ReportsNavigation
            // 
            this.Controls.Add(this.ebNavigation);
            this.Name = "ReportsNavigation";
            ((System.ComponentModel.ISupportInitialize)(this.ebNavigation)).EndInit();
            this.ResumeLayout(false);

        }

        internal Infragistics.Win.UltraWinExplorerBar.UltraExplorerBar ebNavigation;

        private void ebNavigation_ItemClick(object sender, ItemEventArgs e)
        {

        }
    }
}
