using Krista.FM.Client.Components;
namespace Krista.FM.Client.ViewObjects.DataSourcesUI.DataSourceWizard
{
    partial class DataSourcesTreePage
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
            this.dataSourcesTree = new UltraTreeEx();
            ((System.ComponentModel.ISupportInitialize)(this.dataSourcesTree)).BeginInit();
            this.SuspendLayout();
            // 
            // dataSourcesTree
            // 
            this.dataSourcesTree.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.dataSourcesTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataSourcesTree.Location = new System.Drawing.Point(0, 0);
            this.dataSourcesTree.Name = "dataSourcesTree";
            this.dataSourcesTree.Size = new System.Drawing.Size(867, 535);
            this.dataSourcesTree.TabIndex = 0;
            this.dataSourcesTree.AfterSelect += new Infragistics.Win.UltraWinTree.AfterNodeSelectEventHandler(this.DataSourcesTree_AfterSelect);
            // 
            // DataSourcesTreePage
            // 
            this.Controls.Add(this.dataSourcesTree);
            this.Name = "DataSourcesTreePage";
            this.Size = new System.Drawing.Size(867, 535);
            ((System.ComponentModel.ISupportInitialize)(this.dataSourcesTree)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private UltraTreeEx dataSourcesTree;
    }
}