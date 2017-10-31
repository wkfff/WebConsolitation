using System;
using System.Collections.Generic;
using System.Text;

using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.Common;
using Krista.FM.Client.Workplace.Commands;

namespace Krista.FM.Client.Workplace.Services
{
    public static class ToolbarService
    {
        private static UltraToolbarsManager toolbarsManager;

        public static void Attach(System.Windows.Forms.Form parentForm)
        {
            UltraToolbarsDockArea toolbars_Dock_Area_Left = new UltraToolbarsDockArea();
            UltraToolbarsDockArea toolbars_Dock_Area_Right = new UltraToolbarsDockArea();
            UltraToolbarsDockArea toolbars_Dock_Area_Top = new UltraToolbarsDockArea();
            UltraToolbarsDockArea toolbars_Dock_Area_Bottom = new UltraToolbarsDockArea();

            UltraToolbar ultraMenuToolbar = new UltraToolbar("tbStandart");

            PopupMenuTool popupMenuToolFile = new PopupMenuTool("menuFile");
            PopupMenuTool popupMenuToolView = new PopupMenuTool("pmView");
            ListTool listToolColorSchemeList = new ListTool("ColorSchemeList");
            PopupMenuTool popupMenuToolWindow = new PopupMenuTool("Window");
            StateButtonTool stateButtonToolTabbedMDI = new StateButtonTool("TabbedMDI", "");
            MdiWindowListTool mdiWindowListTool = new MdiWindowListTool("MDIWindowListTool");

            PopupMenuTool popupMenuToolHelp = new PopupMenuTool("pmHelp");

            toolbarsManager = new UltraToolbarsManager(parentForm.Container);

            parentForm.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(toolbarsManager)).BeginInit();

            // 
            // ToolbarsManager
            // 
            toolbarsManager.DesignerFlags = 1;
            toolbarsManager.DockWithinContainer = parentForm;
            toolbarsManager.ImageTransparentColor = System.Drawing.Color.Black;
            toolbarsManager.ShowFullMenusDelay = 500;
            toolbarsManager.Style = Infragistics.Win.UltraWinToolbars.ToolbarStyle.Office2003;
            toolbarsManager.ToolbarSettings.AllowCustomize = Infragistics.Win.DefaultableBoolean.False;

            toolbarsManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
                popupMenuToolFile,
                popupMenuToolView,
                listToolColorSchemeList,
                popupMenuToolWindow,
                stateButtonToolTabbedMDI,
                mdiWindowListTool,
                popupMenuToolHelp
                });

            ultraMenuToolbar.DockedColumn = 0;
            ultraMenuToolbar.DockedRow = 0;
            ultraMenuToolbar.Settings.AllowCustomize = Infragistics.Win.DefaultableBoolean.False;
            ultraMenuToolbar.Text = "Стандартная";
            ultraMenuToolbar.IsMainMenuBar = true;
            ultraMenuToolbar.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
                popupMenuToolFile,
                popupMenuToolView,
                popupMenuToolWindow,
                popupMenuToolHelp});


            toolbarsManager.Toolbars.AddRange(new UltraToolbar[] {
                ultraMenuToolbar});

            //
            // File
            //
            popupMenuToolFile.SharedProps.Caption = "&Файл";

            //
            // View
            //
            popupMenuToolView.SharedProps.Caption = "&Вид";

			CommandService.AttachToolbarTool(new ChangePasswordCommand(), ultraMenuToolbar, popupMenuToolFile.Key);
			CommandService.AttachToolbarTool(new ExitWorkplaceCommand(), ultraMenuToolbar, popupMenuToolFile.Key);
			CommandService.AttachToolbarTool(new ShowNavigationCommand(), ultraMenuToolbar, popupMenuToolView.Key);
			CommandService.AttachToolbarTool(new AboutCommand(), ultraMenuToolbar, popupMenuToolHelp.Key);

			listToolColorSchemeList.SharedProps.Caption = "&Стилевые таблицы";
			popupMenuToolView.Tools.AddRange(new ToolBase[] { listToolColorSchemeList });

			//
            // Window
            //
            stateButtonToolTabbedMDI.Checked = true;
            stateButtonToolTabbedMDI.MenuDisplayStyle = StateButtonMenuDisplayStyle.DisplayCheckmark;
            stateButtonToolTabbedMDI.SharedProps.Caption = "В виде &закладок";

            mdiWindowListTool.SharedProps.Caption = "MDIWindowListTool";

            popupMenuToolWindow.SharedProps.Caption = "&Окно";
            popupMenuToolWindow.SharedProps.MergeOrder = 998;
            popupMenuToolWindow.Tools.AddRange(new ToolBase[] {
                stateButtonToolTabbedMDI,
                mdiWindowListTool});

            //
            // About
            //
            popupMenuToolHelp.SharedProps.Caption = "&Справка";

            // 
            // toolbars_Dock_Area_Left
            // 
            toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            toolbars_Dock_Area_Left.BackColor = System.Drawing.SystemColors.Control;
            toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            toolbars_Dock_Area_Left.Location = new System.Drawing.Point(0, 25);
            toolbars_Dock_Area_Left.Name = "toolbars_Dock_Area_Left";
            toolbars_Dock_Area_Left.Size = new System.Drawing.Size(0, 525);
            toolbars_Dock_Area_Left.ToolbarsManager = toolbarsManager;
            // 
            // toolbars_Dock_Area_Right
            // 
            toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            toolbars_Dock_Area_Right.BackColor = System.Drawing.SystemColors.Control;
            toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            toolbars_Dock_Area_Right.Location = new System.Drawing.Point(931, 25);
            toolbars_Dock_Area_Right.Name = "toolbars_Dock_Area_Right";
            toolbars_Dock_Area_Right.Size = new System.Drawing.Size(0, 525);
            toolbars_Dock_Area_Right.ToolbarsManager = toolbarsManager;
            // 
            // toolbars_Dock_Area_Top
            // 
            toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            toolbars_Dock_Area_Top.BackColor = System.Drawing.SystemColors.Control;
            toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            toolbars_Dock_Area_Top.Name = "toolbars_Dock_Area_Top";
            toolbars_Dock_Area_Top.Size = new System.Drawing.Size(931, 25);
            toolbars_Dock_Area_Top.ToolbarsManager = toolbarsManager;
            // 
            // toolbars_Dock_Area_Bottom
            // 
            toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            toolbars_Dock_Area_Bottom.BackColor = System.Drawing.SystemColors.Control;
            toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 550);
            toolbars_Dock_Area_Bottom.Name = "toolbars_Dock_Area_Bottom";
            toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(931, 0);
            toolbars_Dock_Area_Bottom.ToolbarsManager = toolbarsManager;

            parentForm.Controls.Add(toolbars_Dock_Area_Left);
            parentForm.Controls.Add(toolbars_Dock_Area_Right);
            parentForm.Controls.Add(toolbars_Dock_Area_Top);
            parentForm.Controls.Add(toolbars_Dock_Area_Bottom);

            toolbarsManager.ToolClick += new Infragistics.Win.UltraWinToolbars.ToolClickEventHandler(OnToolClickEvent);

            ((System.ComponentModel.ISupportInitialize)(toolbarsManager)).EndInit();
            parentForm.ResumeLayout(false);
            InfragisticComponentsCustomize.CustomizeUltraToolbarsManager(toolbarsManager);
        }

        public static UltraToolbarsManager Control
        {
            get
            {
                System.Diagnostics.Debug.Assert(toolbarsManager != null);
                return toolbarsManager;
            }
        }

        public static event ToolClickEventHandler ToolClick;

        private static void OnToolClickEvent(object sender, ToolClickEventArgs e)
        {
            if (ToolClick != null)
            {
                ToolClick(toolbarsManager, e);
            }
        }
    }
}
