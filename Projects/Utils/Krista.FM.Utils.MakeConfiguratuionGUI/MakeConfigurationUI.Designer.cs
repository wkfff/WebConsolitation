namespace Krista.FM.Utils.MakeConfiguratuionGUI
{
    partial class MakeConfigurationUI
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.UltraWinToolbars.UltraToolbar ultraToolbar1 = new Infragistics.Win.UltraWinToolbars.UltraToolbar("UltraToolbar1");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool1 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("File");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool2 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("File");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Open SchemeConfiguration File...");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Open SchemeConfiguration File...");
            this.MakeConfigurationUI_Fill_Panel = new System.Windows.Forms.Panel();
            this.ultraPanel2 = new Infragistics.Win.Misc.UltraPanel();
            this.ultraTree = new Infragistics.Win.UltraWinTree.UltraTree();
            this.ultraPanel1 = new Infragistics.Win.Misc.UltraPanel();
            this.closeForm = new System.Windows.Forms.Button();
            this.saveConfiguration = new System.Windows.Forms.Button();
            this._MakeConfigurationUI_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._MakeConfigurationUI_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._MakeConfigurationUI_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._MakeConfigurationUI_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.ultraToolbarsManager1 = new Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(this.components);
            this.MakeConfigurationUI_Fill_Panel.SuspendLayout();
            this.ultraPanel2.ClientArea.SuspendLayout();
            this.ultraPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraTree)).BeginInit();
            this.ultraPanel1.ClientArea.SuspendLayout();
            this.ultraPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraToolbarsManager1)).BeginInit();
            this.SuspendLayout();
            // 
            // MakeConfigurationUI_Fill_Panel
            // 
            this.MakeConfigurationUI_Fill_Panel.Controls.Add(this.ultraPanel2);
            this.MakeConfigurationUI_Fill_Panel.Controls.Add(this.ultraPanel1);
            this.MakeConfigurationUI_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.MakeConfigurationUI_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MakeConfigurationUI_Fill_Panel.Location = new System.Drawing.Point(0, 50);
            this.MakeConfigurationUI_Fill_Panel.Name = "MakeConfigurationUI_Fill_Panel";
            this.MakeConfigurationUI_Fill_Panel.Size = new System.Drawing.Size(439, 485);
            this.MakeConfigurationUI_Fill_Panel.TabIndex = 0;
            // 
            // ultraPanel2
            // 
            // 
            // ultraPanel2.ClientArea
            // 
            this.ultraPanel2.ClientArea.Controls.Add(this.ultraTree);
            this.ultraPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraPanel2.Location = new System.Drawing.Point(0, 0);
            this.ultraPanel2.Name = "ultraPanel2";
            this.ultraPanel2.Size = new System.Drawing.Size(439, 428);
            this.ultraPanel2.TabIndex = 1;
            // 
            // ultraTree
            // 
            this.ultraTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraTree.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.ultraTree.Location = new System.Drawing.Point(0, 0);
            this.ultraTree.Name = "ultraTree";
            this.ultraTree.NodeConnectorColor = System.Drawing.SystemColors.ControlDark;
            this.ultraTree.Size = new System.Drawing.Size(439, 428);
            this.ultraTree.TabIndex = 0;
            // 
            // ultraPanel1
            // 
            // 
            // ultraPanel1.ClientArea
            // 
            this.ultraPanel1.ClientArea.Controls.Add(this.closeForm);
            this.ultraPanel1.ClientArea.Controls.Add(this.saveConfiguration);
            this.ultraPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ultraPanel1.Location = new System.Drawing.Point(0, 428);
            this.ultraPanel1.Name = "ultraPanel1";
            this.ultraPanel1.Size = new System.Drawing.Size(439, 57);
            this.ultraPanel1.TabIndex = 0;
            // 
            // closeForm
            // 
            this.closeForm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeForm.Location = new System.Drawing.Point(352, 19);
            this.closeForm.Name = "closeForm";
            this.closeForm.Size = new System.Drawing.Size(75, 23);
            this.closeForm.TabIndex = 1;
            this.closeForm.Text = "Close";
            this.closeForm.UseVisualStyleBackColor = true;
            this.closeForm.Click += new System.EventHandler(this.closeForm_Click);
            // 
            // saveConfiguration
            // 
            this.saveConfiguration.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.saveConfiguration.Location = new System.Drawing.Point(262, 19);
            this.saveConfiguration.Name = "saveConfiguration";
            this.saveConfiguration.Size = new System.Drawing.Size(75, 23);
            this.saveConfiguration.TabIndex = 0;
            this.saveConfiguration.Text = "Save";
            this.saveConfiguration.UseVisualStyleBackColor = true;
            this.saveConfiguration.Click += new System.EventHandler(this.saveConfiguration_Click);
            // 
            // _MakeConfigurationUI_Toolbars_Dock_Area_Left
            // 
            this._MakeConfigurationUI_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._MakeConfigurationUI_Toolbars_Dock_Area_Left.BackColor = System.Drawing.SystemColors.Control;
            this._MakeConfigurationUI_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            this._MakeConfigurationUI_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._MakeConfigurationUI_Toolbars_Dock_Area_Left.Location = new System.Drawing.Point(0, 50);
            this._MakeConfigurationUI_Toolbars_Dock_Area_Left.Name = "_MakeConfigurationUI_Toolbars_Dock_Area_Left";
            this._MakeConfigurationUI_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(0, 485);
            this._MakeConfigurationUI_Toolbars_Dock_Area_Left.ToolbarsManager = this.ultraToolbarsManager1;
            // 
            // _MakeConfigurationUI_Toolbars_Dock_Area_Right
            // 
            this._MakeConfigurationUI_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._MakeConfigurationUI_Toolbars_Dock_Area_Right.BackColor = System.Drawing.SystemColors.Control;
            this._MakeConfigurationUI_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._MakeConfigurationUI_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._MakeConfigurationUI_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(439, 50);
            this._MakeConfigurationUI_Toolbars_Dock_Area_Right.Name = "_MakeConfigurationUI_Toolbars_Dock_Area_Right";
            this._MakeConfigurationUI_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(0, 485);
            this._MakeConfigurationUI_Toolbars_Dock_Area_Right.ToolbarsManager = this.ultraToolbarsManager1;
            // 
            // _MakeConfigurationUI_Toolbars_Dock_Area_Top
            // 
            this._MakeConfigurationUI_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._MakeConfigurationUI_Toolbars_Dock_Area_Top.BackColor = System.Drawing.SystemColors.Control;
            this._MakeConfigurationUI_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._MakeConfigurationUI_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._MakeConfigurationUI_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._MakeConfigurationUI_Toolbars_Dock_Area_Top.Name = "_MakeConfigurationUI_Toolbars_Dock_Area_Top";
            this._MakeConfigurationUI_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(439, 50);
            this._MakeConfigurationUI_Toolbars_Dock_Area_Top.ToolbarsManager = this.ultraToolbarsManager1;
            // 
            // _MakeConfigurationUI_Toolbars_Dock_Area_Bottom
            // 
            this._MakeConfigurationUI_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._MakeConfigurationUI_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.SystemColors.Control;
            this._MakeConfigurationUI_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._MakeConfigurationUI_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._MakeConfigurationUI_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 535);
            this._MakeConfigurationUI_Toolbars_Dock_Area_Bottom.Name = "_MakeConfigurationUI_Toolbars_Dock_Area_Bottom";
            this._MakeConfigurationUI_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(439, 0);
            this._MakeConfigurationUI_Toolbars_Dock_Area_Bottom.ToolbarsManager = this.ultraToolbarsManager1;
            // 
            // ultraToolbarsManager1
            // 
            this.ultraToolbarsManager1.DesignerFlags = 1;
            this.ultraToolbarsManager1.DockWithinContainer = this;
            this.ultraToolbarsManager1.DockWithinContainerBaseType = typeof(System.Windows.Forms.Form);
            ultraToolbar1.DockedColumn = 0;
            ultraToolbar1.DockedRow = 0;
            ultraToolbar1.NonInheritedTools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            popupMenuTool1});
            ultraToolbar1.Text = "UltraToolbar1";
            this.ultraToolbarsManager1.Toolbars.AddRange(new Infragistics.Win.UltraWinToolbars.UltraToolbar[] {
            ultraToolbar1});
            popupMenuTool2.SharedPropsInternal.Caption = "&File";
            popupMenuTool2.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool1});
            buttonTool2.SharedPropsInternal.Caption = "&Open SchemeConfiguration File...";
            this.ultraToolbarsManager1.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            popupMenuTool2,
            buttonTool2});
            this.ultraToolbarsManager1.ToolClick += new Infragistics.Win.UltraWinToolbars.ToolClickEventHandler(this.ultraToolbarsManager1_ToolClick);
            // 
            // MakeConfigurationUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(439, 535);
            this.Controls.Add(this.MakeConfigurationUI_Fill_Panel);
            this.Controls.Add(this._MakeConfigurationUI_Toolbars_Dock_Area_Left);
            this.Controls.Add(this._MakeConfigurationUI_Toolbars_Dock_Area_Right);
            this.Controls.Add(this._MakeConfigurationUI_Toolbars_Dock_Area_Top);
            this.Controls.Add(this._MakeConfigurationUI_Toolbars_Dock_Area_Bottom);
            this.Name = "MakeConfigurationUI";
            this.Text = "Создание конфигурационного файла";
            this.MakeConfigurationUI_Fill_Panel.ResumeLayout(false);
            this.ultraPanel2.ClientArea.ResumeLayout(false);
            this.ultraPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ultraTree)).EndInit();
            this.ultraPanel1.ClientArea.ResumeLayout(false);
            this.ultraPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ultraToolbarsManager1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinToolbars.UltraToolbarsManager ultraToolbarsManager1;
        private System.Windows.Forms.Panel MakeConfigurationUI_Fill_Panel;
        private Infragistics.Win.Misc.UltraPanel ultraPanel2;
        private Infragistics.Win.UltraWinTree.UltraTree ultraTree;
        private Infragistics.Win.Misc.UltraPanel ultraPanel1;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _MakeConfigurationUI_Toolbars_Dock_Area_Left;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _MakeConfigurationUI_Toolbars_Dock_Area_Right;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _MakeConfigurationUI_Toolbars_Dock_Area_Top;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _MakeConfigurationUI_Toolbars_Dock_Area_Bottom;
        private System.Windows.Forms.Button closeForm;
        private System.Windows.Forms.Button saveConfiguration;
    }
}

