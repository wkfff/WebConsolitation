namespace Krista.FM.Client.SchemeEditor
{
    partial class ModificationsTreeControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ModificationsTreeControl));
            Infragistics.Win.UltraWinToolbars.UltraToolbar ultraToolbar1 = new Infragistics.Win.UltraWinToolbars.UltraToolbar("Toolbar");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Refresh");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Applay");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool6 = new Infragistics.Win.UltraWinToolbars.ButtonTool("ApplayForNode");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool3 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Refresh");
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool4 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Applay");
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool5 = new Infragistics.Win.UltraWinToolbars.ButtonTool("ApplayForNode");
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.modificationsTreeView = new Krista.FM.Client.SchemeEditor.ModificationTreeView();
            this._ModificationsTreeControl_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.ultraToolbarsManager = new Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(this.components);
            this._ModificationsTreeControl_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._ModificationsTreeControl_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._ModificationsTreeControl_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            ((System.ComponentModel.ISupportInitialize)(this.modificationsTreeView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraToolbarsManager)).BeginInit();
            this.SuspendLayout();
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Magenta;
            this.imageList.Images.SetKeyName(0, "");
            this.imageList.Images.SetKeyName(1, "");
            this.imageList.Images.SetKeyName(2, "Применить изменения_16х16_Вариант5.bmp");
            // 
            // modificationsTreeView
            // 
            this.modificationsTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.modificationsTreeView.HideSelection = false;
            this.modificationsTreeView.ImageTransparentColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))));
            this.modificationsTreeView.Location = new System.Drawing.Point(0, 23);
            this.modificationsTreeView.Name = "modificationsTreeView";
            this.modificationsTreeView.PathSeparator = "::";
            this.modificationsTreeView.ReadOnly = false;
            this.modificationsTreeView.Size = new System.Drawing.Size(456, 319);
            this.modificationsTreeView.TabIndex = 0;
            // 
            // _ModificationsTreeControl_Toolbars_Dock_Area_Left
            // 
            this._ModificationsTreeControl_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._ModificationsTreeControl_Toolbars_Dock_Area_Left.BackColor = System.Drawing.SystemColors.Control;
            this._ModificationsTreeControl_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            this._ModificationsTreeControl_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._ModificationsTreeControl_Toolbars_Dock_Area_Left.Location = new System.Drawing.Point(0, 23);
            this._ModificationsTreeControl_Toolbars_Dock_Area_Left.Name = "_ModificationsTreeControl_Toolbars_Dock_Area_Left";
            this._ModificationsTreeControl_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(0, 319);
            this._ModificationsTreeControl_Toolbars_Dock_Area_Left.ToolbarsManager = this.ultraToolbarsManager;
            // 
            // ultraToolbarsManager
            // 
            this.ultraToolbarsManager.DesignerFlags = 1;
            this.ultraToolbarsManager.DockWithinContainer = this;
            this.ultraToolbarsManager.ImageListSmall = this.imageList;
            this.ultraToolbarsManager.LockToolbars = true;
            this.ultraToolbarsManager.ShowFullMenusDelay = 500;
            ultraToolbar1.DockedColumn = 0;
            ultraToolbar1.DockedRow = 0;
            ultraToolbar1.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool1,
            buttonTool2,
            buttonTool6});
            ultraToolbar1.Text = "Toolbar";
            this.ultraToolbarsManager.Toolbars.AddRange(new Infragistics.Win.UltraWinToolbars.UltraToolbar[] {
            ultraToolbar1});
            appearance1.Image = 0;
            buttonTool3.SharedProps.AppearancesSmall.Appearance = appearance1;
            buttonTool3.SharedProps.Caption = "Обновить";
            appearance2.Image = 1;
            buttonTool4.SharedProps.AppearancesSmall.Appearance = appearance2;
            buttonTool4.SharedProps.Caption = "Применить изменения";
            appearance13.Image = ((object)(resources.GetObject("appearance13.Image")));
            buttonTool5.SharedProps.AppearancesLarge.Appearance = appearance13;
            appearance14.Image = 2;
            buttonTool5.SharedProps.AppearancesSmall.Appearance = appearance14;
            buttonTool5.SharedProps.Caption = "Применить для текущего узла";
            buttonTool5.SharedProps.DisplayStyle = Infragistics.Win.UltraWinToolbars.ToolDisplayStyle.DefaultForToolType;
            this.ultraToolbarsManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool3,
            buttonTool4,
            buttonTool5});
            this.ultraToolbarsManager.ToolClick += new Infragistics.Win.UltraWinToolbars.ToolClickEventHandler(this.ultraToolbarsManager_ToolClick);
            // 
            // _ModificationsTreeControl_Toolbars_Dock_Area_Right
            // 
            this._ModificationsTreeControl_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._ModificationsTreeControl_Toolbars_Dock_Area_Right.BackColor = System.Drawing.SystemColors.Control;
            this._ModificationsTreeControl_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._ModificationsTreeControl_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._ModificationsTreeControl_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(456, 23);
            this._ModificationsTreeControl_Toolbars_Dock_Area_Right.Name = "_ModificationsTreeControl_Toolbars_Dock_Area_Right";
            this._ModificationsTreeControl_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(0, 319);
            this._ModificationsTreeControl_Toolbars_Dock_Area_Right.ToolbarsManager = this.ultraToolbarsManager;
            // 
            // _ModificationsTreeControl_Toolbars_Dock_Area_Top
            // 
            this._ModificationsTreeControl_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._ModificationsTreeControl_Toolbars_Dock_Area_Top.BackColor = System.Drawing.SystemColors.Control;
            this._ModificationsTreeControl_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._ModificationsTreeControl_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._ModificationsTreeControl_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._ModificationsTreeControl_Toolbars_Dock_Area_Top.Name = "_ModificationsTreeControl_Toolbars_Dock_Area_Top";
            this._ModificationsTreeControl_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(456, 23);
            this._ModificationsTreeControl_Toolbars_Dock_Area_Top.ToolbarsManager = this.ultraToolbarsManager;
            // 
            // _ModificationsTreeControl_Toolbars_Dock_Area_Bottom
            // 
            this._ModificationsTreeControl_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._ModificationsTreeControl_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.SystemColors.Control;
            this._ModificationsTreeControl_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._ModificationsTreeControl_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._ModificationsTreeControl_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 342);
            this._ModificationsTreeControl_Toolbars_Dock_Area_Bottom.Name = "_ModificationsTreeControl_Toolbars_Dock_Area_Bottom";
            this._ModificationsTreeControl_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(456, 0);
            this._ModificationsTreeControl_Toolbars_Dock_Area_Bottom.ToolbarsManager = this.ultraToolbarsManager;
            // 
            // ModificationsTreeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.modificationsTreeView);
            this.Controls.Add(this._ModificationsTreeControl_Toolbars_Dock_Area_Left);
            this.Controls.Add(this._ModificationsTreeControl_Toolbars_Dock_Area_Right);
            this.Controls.Add(this._ModificationsTreeControl_Toolbars_Dock_Area_Top);
            this.Controls.Add(this._ModificationsTreeControl_Toolbars_Dock_Area_Bottom);
            this.Name = "ModificationsTreeControl";
            this.Size = new System.Drawing.Size(456, 342);
            ((System.ComponentModel.ISupportInitialize)(this.modificationsTreeView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraToolbarsManager)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ModificationTreeView modificationsTreeView;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsManager ultraToolbarsManager;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _ModificationsTreeControl_Toolbars_Dock_Area_Left;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _ModificationsTreeControl_Toolbars_Dock_Area_Right;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _ModificationsTreeControl_Toolbars_Dock_Area_Top;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _ModificationsTreeControl_Toolbars_Dock_Area_Bottom;
        private System.Windows.Forms.ImageList imageList;
    }
}
