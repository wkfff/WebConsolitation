namespace Krista.FM.Client.SchemeEditor.DiargamEditor
{
    partial class DiargamEditorForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			Krista.FM.Client.DiagramEditor.DefaultSettings defaultSettings1 = new Krista.FM.Client.DiagramEditor.DefaultSettings();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DiargamEditorForm));
			Krista.FM.Client.DiagramEditor.Commands.UndoRedo undoRedo1 = new Krista.FM.Client.DiagramEditor.Commands.UndoRedo();
			Infragistics.Win.UltraWinToolbars.UltraToolbar ultraToolbar1 = new Infragistics.Win.UltraWinToolbars.UltraToolbar("MenuBar");
			Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool1 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("File");
			Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool2 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("Edit");
			Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool3 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("Diagram");
			Infragistics.Win.UltraWinToolbars.UltraToolbar ultraToolbar2 = new Infragistics.Win.UltraWinToolbars.UltraToolbar("ToolBar");
			Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool5 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("Edit");
			Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
			Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool6 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("Diagram");
			Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool10 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("File");
			this.diargamEditor = new Krista.FM.Client.DiagramEditor.DiargamEditor();
			this.ultraToolbarsManager = new Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(this.components);
			this.DiargamEditorForm_Fill_Panel = new System.Windows.Forms.Panel();
			this._DiargamEditorForm_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
			this._DiargamEditorForm_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
			this._DiargamEditorForm_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
			this._DiargamEditorForm_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
			((System.ComponentModel.ISupportInitialize)(this.ultraToolbarsManager)).BeginInit();
			this.DiargamEditorForm_Fill_Panel.SuspendLayout();
			this.SuspendLayout();
			// 
			// diargamEditor
			// 
			this.diargamEditor.AllowDrop = true;
			this.diargamEditor.AutoScroll = true;
			this.diargamEditor.AutoScrollMinSize = new System.Drawing.Size(15, 15);
			this.diargamEditor.BackColor = System.Drawing.Color.White;
			this.diargamEditor.BeginDrawAssociate = false;
			this.diargamEditor.CreateAssociation = false;
			this.diargamEditor.CreateBridgeAssociation = false;
			this.diargamEditor.CreateMasterDetailAssociation = false;
			defaultSettings1.DafaultPageSettings = ((System.Drawing.Printing.PageSettings)(resources.GetObject("defaultSettings1.DafaultPageSettings")));
			this.diargamEditor.DefaultSettings = defaultSettings1;
			this.diargamEditor.Diagram = null;
			this.diargamEditor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.diargamEditor.LastClickPoint = new System.Drawing.Point(0, 0);
			this.diargamEditor.Location = new System.Drawing.Point(0, 0);
			this.diargamEditor.MinimumSize = new System.Drawing.Size(20, 20);
			this.diargamEditor.Name = "diargamEditor";
			this.diargamEditor.NewAssociationCommand = null;
			this.diargamEditor.NewEntityCommand = null;
			this.diargamEditor.NewPackageCommand = null;
			this.diargamEditor.Points = ((System.Collections.Generic.List<System.Drawing.Point>)(resources.GetObject("diargamEditor.Points")));
			this.diargamEditor.SchemeEditor = null;
			this.diargamEditor.SelectedShape = null;
			this.diargamEditor.Size = new System.Drawing.Size(584, 287);
			this.diargamEditor.TabIndex = 0;
			this.diargamEditor.Text = "diargamEditor1";
			this.diargamEditor.ToolbarsManager = null;
			undoRedo1.Site = this.diargamEditor;
			this.diargamEditor.UndoredoManager = undoRedo1;
			this.diargamEditor.ZoomFactor = 100;
			// 
			// ultraToolbarsManager
			// 
			this.ultraToolbarsManager.DesignerFlags = 1;
			this.ultraToolbarsManager.DockWithinContainer = this;
			this.ultraToolbarsManager.DockWithinContainerBaseType = typeof(System.Windows.Forms.Form);
			this.ultraToolbarsManager.ShowFullMenusDelay = 500;
			ultraToolbar1.DockedColumn = 0;
			ultraToolbar1.DockedRow = 0;
			ultraToolbar1.IsMainMenuBar = true;
			ultraToolbar1.NonInheritedTools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            popupMenuTool1,
            popupMenuTool2,
            popupMenuTool3});
			ultraToolbar1.Text = "MenuBar";
			ultraToolbar2.DockedColumn = 0;
			ultraToolbar2.DockedRow = 1;
			ultraToolbar2.Text = "ToolBar";
			this.ultraToolbarsManager.Toolbars.AddRange(new Infragistics.Win.UltraWinToolbars.UltraToolbar[] {
            ultraToolbar1,
            ultraToolbar2});
			appearance1.Image = 0;
			popupMenuTool5.SharedProps.AppearancesSmall.Appearance = appearance1;
			popupMenuTool5.SharedProps.Caption = "&Правка";
			popupMenuTool6.SharedProps.Caption = "&Диаграмма";
			popupMenuTool6.SharedProps.CustomizerCaption = "&Диаграмма";
			popupMenuTool10.SharedProps.Caption = "&Файл";
			popupMenuTool10.SharedProps.DisplayStyle = Infragistics.Win.UltraWinToolbars.ToolDisplayStyle.TextOnlyInMenus;
			this.ultraToolbarsManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            popupMenuTool5,
            popupMenuTool6,
            popupMenuTool10});
			// 
			// DiargamEditorForm_Fill_Panel
			// 
			this.DiargamEditorForm_Fill_Panel.Controls.Add(this.diargamEditor);
			this.DiargamEditorForm_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
			this.DiargamEditorForm_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.DiargamEditorForm_Fill_Panel.Location = new System.Drawing.Point(0, 71);
			this.DiargamEditorForm_Fill_Panel.Name = "DiargamEditorForm_Fill_Panel";
			this.DiargamEditorForm_Fill_Panel.Size = new System.Drawing.Size(584, 287);
			this.DiargamEditorForm_Fill_Panel.TabIndex = 0;
			// 
			// _DiargamEditorForm_Toolbars_Dock_Area_Left
			// 
			this._DiargamEditorForm_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
			this._DiargamEditorForm_Toolbars_Dock_Area_Left.BackColor = System.Drawing.SystemColors.Control;
			this._DiargamEditorForm_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
			this._DiargamEditorForm_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
			this._DiargamEditorForm_Toolbars_Dock_Area_Left.Location = new System.Drawing.Point(0, 71);
			this._DiargamEditorForm_Toolbars_Dock_Area_Left.Name = "_DiargamEditorForm_Toolbars_Dock_Area_Left";
			this._DiargamEditorForm_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(0, 287);
			this._DiargamEditorForm_Toolbars_Dock_Area_Left.ToolbarsManager = this.ultraToolbarsManager;
			// 
			// _DiargamEditorForm_Toolbars_Dock_Area_Right
			// 
			this._DiargamEditorForm_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
			this._DiargamEditorForm_Toolbars_Dock_Area_Right.BackColor = System.Drawing.SystemColors.Control;
			this._DiargamEditorForm_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
			this._DiargamEditorForm_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
			this._DiargamEditorForm_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(584, 71);
			this._DiargamEditorForm_Toolbars_Dock_Area_Right.Name = "_DiargamEditorForm_Toolbars_Dock_Area_Right";
			this._DiargamEditorForm_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(0, 287);
			this._DiargamEditorForm_Toolbars_Dock_Area_Right.ToolbarsManager = this.ultraToolbarsManager;
			// 
			// _DiargamEditorForm_Toolbars_Dock_Area_Top
			// 
			this._DiargamEditorForm_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
			this._DiargamEditorForm_Toolbars_Dock_Area_Top.BackColor = System.Drawing.SystemColors.Control;
			this._DiargamEditorForm_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
			this._DiargamEditorForm_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
			this._DiargamEditorForm_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
			this._DiargamEditorForm_Toolbars_Dock_Area_Top.Name = "_DiargamEditorForm_Toolbars_Dock_Area_Top";
			this._DiargamEditorForm_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(584, 71);
			this._DiargamEditorForm_Toolbars_Dock_Area_Top.ToolbarsManager = this.ultraToolbarsManager;
			// 
			// _DiargamEditorForm_Toolbars_Dock_Area_Bottom
			// 
			this._DiargamEditorForm_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
			this._DiargamEditorForm_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.SystemColors.Control;
			this._DiargamEditorForm_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
			this._DiargamEditorForm_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
			this._DiargamEditorForm_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 358);
			this._DiargamEditorForm_Toolbars_Dock_Area_Bottom.Name = "_DiargamEditorForm_Toolbars_Dock_Area_Bottom";
			this._DiargamEditorForm_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(584, 0);
			this._DiargamEditorForm_Toolbars_Dock_Area_Bottom.ToolbarsManager = this.ultraToolbarsManager;
			// 
			// DiargamEditorForm
			// 
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.ClientSize = new System.Drawing.Size(584, 358);
			this.Controls.Add(this.DiargamEditorForm_Fill_Panel);
			this.Controls.Add(this._DiargamEditorForm_Toolbars_Dock_Area_Left);
			this.Controls.Add(this._DiargamEditorForm_Toolbars_Dock_Area_Right);
			this.Controls.Add(this._DiargamEditorForm_Toolbars_Dock_Area_Top);
			this.Controls.Add(this._DiargamEditorForm_Toolbars_Dock_Area_Bottom);
			this.KeyPreview = true;
			this.Name = "DiargamEditorForm";
			this.Text = "DiargamEditorForm";
			this.DragOver += new System.Windows.Forms.DragEventHandler(this.DiargamEditorForm_DragOver);
			((System.ComponentModel.ISupportInitialize)(this.ultraToolbarsManager)).EndInit();
			this.DiargamEditorForm_Fill_Panel.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinToolbars.UltraToolbarsManager ultraToolbarsManager;
        private System.Windows.Forms.Panel DiargamEditorForm_Fill_Panel;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _DiargamEditorForm_Toolbars_Dock_Area_Left;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _DiargamEditorForm_Toolbars_Dock_Area_Right;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _DiargamEditorForm_Toolbars_Dock_Area_Top;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _DiargamEditorForm_Toolbars_Dock_Area_Bottom;
        private Krista.FM.Client.DiagramEditor.DiargamEditor diargamEditor;
    }
}