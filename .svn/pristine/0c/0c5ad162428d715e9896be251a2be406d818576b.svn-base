using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.Common;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Common;

namespace Krista.FM.Client.ViewObjects.AdministrationUI
{
    public partial class AdministrationNavigation : BaseNavigationCtrl, IAdministrationNavigation
    {
        private static AdministrationNavigation instance;

        internal static AdministrationNavigation Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AdministrationNavigation();
                }
                return instance;
            }
        }

        private Dictionary<string, AdministrationUI> openedViewObjects;

        public AdministrationNavigation()
        {
            instance = this;
            Caption = "Администрирование";
        }

        public override System.Drawing.Image TypeImage16
        {
            get { return Properties.Resources.Admin_16; }
        }

        public override System.Drawing.Image TypeImage24
        {
            get { return Properties.Resources.Admin_32; }
        }

        /// <summary>
        /// Инициализация объкта навигации.
        /// </summary>
        public override void Initialize()
        {
            InitializeComponent();

            openedViewObjects = new Dictionary<string, AdministrationUI>();

            adminExplorerBar.ItemCheckStateChanged += new Infragistics.Win.UltraWinExplorerBar.ItemCheckStateChangedEventHandler(adminExplorerBar_ItemCheckStateChanged);

            utbExportImport.ToolClick += new ToolClickEventHandler(utbExportImport_ToolClick);

            Workplace.ViewClosed += new Krista.FM.Client.Common.Gui.ViewContentEventHandler(Workplace_ViewClosed);
            Workplace.ActiveWorkplaceWindowChanged += new EventHandler(Workplace_ActiveWorkplaceWindowChanged);
			if (ClientAuthentication.UserID == 1)
            {
                // делаем доступным только для администратора сохранение и выгрузку данных по администрированию
                utbExportImport.Tools["btnImport"].SharedProps.Enabled = true;
            }

            base.Initialize();

            InfragisticsRusification.LocalizeAll();
        }

        void Workplace_ActiveWorkplaceWindowChanged(object sender, EventArgs e)
        {
			if (Workplace.WorkplaceLayout.ActiveContent != null)
            {
                string key = ((BaseViewObj)Workplace.WorkplaceLayout.ActiveContent).Key;
                if (!openedViewObjects.ContainsKey(key))
                    return;
                Workplace.SwitchTo("Администрирование");
                if (adminExplorerBar.CheckedItem == null)
                    return;
                if (key != adminExplorerBar.CheckedItem.Key)
                {
                    if (adminExplorerBar.Groups[0].Items.Exists(key))
                    {
                        adminExplorerBar.Groups[0].Items[key].Checked = true;
                        adminExplorerBar.Groups[0].Items[key].Active = true;
                    }
                    else
                    {
                        
                        adminExplorerBar.Groups[1].Items[key].Checked = true;
                        adminExplorerBar.Groups[1].Items[key].Active = true;
                    }
                }
                if (adminExplorerBar.Groups[1].Items.Exists(key))
                    adminExplorerBar.Groups[1].Expanded = true;
            }
        }

        void adminExplorerBar_ItemCheckStateChanged(object sender, Infragistics.Win.UltraWinExplorerBar.ItemEventArgs e)
        {
            if (!e.Item.Checked)
                return;

            string key = e.Item.Key;
            if (!openedViewObjects.ContainsKey(key))
            {
                AdministrationUI viewObject = new AdministrationUI(key);
                viewObject.Workplace = Workplace;
                viewObject.Initialize();
                viewObject.LoadData();
                OnActiveItemChanged(this, viewObject);
                openedViewObjects.Add(key, viewObject);
            }
            else
            {
                if (openedViewObjects[key].ViewCtrl.IsDisposed)
                {
                    AdministrationUI viewObject = new AdministrationUI(key);
                    viewObject.Workplace = Workplace;
                    viewObject.Initialize();
                    viewObject.LoadData();
                    openedViewObjects[key] = viewObject;
                }
                OnActiveItemChanged(this, openedViewObjects[key]);
            }
        }

        private void Workplace_ViewClosed(object sender, Krista.FM.Client.Common.Gui.ViewContentEventArgs e)
        {
            if (adminExplorerBar.CheckedItem == null)
                return;

            if (e.Content.Key == adminExplorerBar.CheckedItem.Key)
            {
                adminExplorerBar.CheckedItem.Active = false;
                adminExplorerBar.CheckedItem.Checked = false;
            }

            if (openedViewObjects.ContainsKey(e.Content.Key))
            {
                try
                {
                    if (openedViewObjects[e.Content.Key] != null)
                        openedViewObjects[e.Content.Key].Dispose();
                }
                finally
                {
                    openedViewObjects.Remove(e.Content.Key);
                }
            }
        }

        private void utbExportImport_ToolClick(object sender, ToolClickEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "btnExport":
                    ExportImportHelper.Export(this.Workplace);
                    break;
                case "btnImport":
                    ExportImportHelper.Import(this.Workplace);
                    break;
            }
        }

        public IInplaceTasksPermissionsView GetTasksPermissions()
        {
            AdministrationUI admUI = new AdministrationUI(AdministrationNavigationObjectKeys.Users);
            admUI.Workplace = this.Workplace;
            admUI.Initialize();
            return (IInplaceTasksPermissionsView)admUI;
        }

        internal static NavigationNodeKind GetNodeKind(string nodeName)
        {
            switch (nodeName)
            {
                case AdministrationNavigationObjectKeys.Users:
                    return NavigationNodeKind.ndAllUsers;
                case AdministrationNavigationObjectKeys.Groups:
                    return NavigationNodeKind.ndAllGroups;
                case AdministrationNavigationObjectKeys.Objects:
                    return NavigationNodeKind.ndAllObjects;
                case AdministrationNavigationObjectKeys.Organizations:
                    return NavigationNodeKind.ndOrganizations;
                case AdministrationNavigationObjectKeys.Divisions:
                    return NavigationNodeKind.ndDivisions;
                case AdministrationNavigationObjectKeys.TaskTypes:
                    return NavigationNodeKind.ndTasksTypes;
                case AdministrationNavigationObjectKeys.Sessions:
                    return NavigationNodeKind.ndSessions;
                default:
                    return NavigationNodeKind.ndUnknown;
            }
        }
    }

    public static class AdministrationNavigationObjectKeys
    {
        public const string Users = "9DB8F522-0800-44db-A686-C93D9054F98C";
        public const string Groups = "FFA006FE-2303-421a-AF50-125030E0B395";
        public const string Objects = "92224422-C6C7-4010-90BE-E34D99B0DA8F";
        public const string Sessions = "9522B26F-3543-49f8-BF0C-C20C44FF3698";
        public const string Organizations = "D88EED65-54B1-4b07-A233-9025EF8B9697";
        public const string Divisions = "7A7EFFBC-F96C-4a93-B2BD-3F95BC79A2AF";
        public const string TaskTypes = "4DB25563-2039-47e3-A132-92DF6BCD7D79";
    }
}
