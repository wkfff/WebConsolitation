namespace Krista.FM.Client.MDXExpert
{
    partial class ShapePropertiesForm
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
            this.tvShape = new Infragistics.Win.UltraWinTree.UltraTree();
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.btOK = new Infragistics.Win.Misc.UltraButton();
            ((System.ComponentModel.ISupportInitialize)(this.tvShape)).BeginInit();
            this.SuspendLayout();
            // 
            // tvShape
            // 
            this.tvShape.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.tvShape.HideSelection = false;
            this.tvShape.Location = new System.Drawing.Point(12, 12);
            this.tvShape.Name = "tvShape";
            this.tvShape.ScrollBounds = Infragistics.Win.UltraWinTree.ScrollBounds.ScrollToFill;
            this.tvShape.ShowRootLines = false;
            this.tvShape.Size = new System.Drawing.Size(226, 257);
            this.tvShape.TabIndex = 0;
            this.tvShape.AfterSelect += new Infragistics.Win.UltraWinTree.AfterNodeSelectEventHandler(this.tvLayers_AfterSelect);
            // 
            // propertyGrid
            // 
            this.propertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGrid.Location = new System.Drawing.Point(264, 16);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(204, 253);
            this.propertyGrid.TabIndex = 1;
            this.propertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid_PropertyValueChanged);
            // 
            // btOK
            // 
            this.btOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btOK.Location = new System.Drawing.Point(395, 275);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(75, 23);
            this.btOK.TabIndex = 2;
            this.btOK.Text = "OK";
            this.btOK.Click += new System.EventHandler(this.btOK_Click);
            // 
            // ShapePropertiesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(482, 310);
            this.Controls.Add(this.btOK);
            this.Controls.Add(this.propertyGrid);
            this.Controls.Add(this.tvShape);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ShapePropertiesForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Свойства объекта";
            this.Deactivate += new System.EventHandler(this.ShapePropertiesForm_Deactivate);
            ((System.ComponentModel.ISupportInitialize)(this.tvShape)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinTree.UltraTree tvShape;
        private System.Windows.Forms.PropertyGrid propertyGrid;
        private Infragistics.Win.Misc.UltraButton btOK;
    }
}