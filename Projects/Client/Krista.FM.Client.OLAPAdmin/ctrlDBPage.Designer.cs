namespace Krista.FM.Client.OLAPAdmin
{
	partial class ctrlDBPage
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
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab1 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab2 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab3 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab4 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            this.ultraTabPageControl1 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.cubesExplorer = new Krista.FM.Client.OLAPAdmin.ctrlObjectExplorer();
            this.ultraTabPageControl2 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.dimsExplorer = new Krista.FM.Client.OLAPAdmin.ctrlObjectExplorer();
            this.ultraTabPageControl3 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.tabCtrlDB = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this.ultraTabSharedControlsPage1 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.dsExplorer = new ctrlObjectExplorer();
            this.ultraTabPageControl4 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.dsvExplorer = new Krista.FM.Client.OLAPAdmin.ctrlObjectExplorer();
            this.ultraTabPageControl1.SuspendLayout();
            this.ultraTabPageControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tabCtrlDB)).BeginInit();
            this.tabCtrlDB.SuspendLayout();
            this.ultraTabPageControl4.SuspendLayout();
            this.SuspendLayout();
            // 
            // ultraTabPageControl1
            // 
            this.ultraTabPageControl1.Controls.Add(this.cubesExplorer);
            this.ultraTabPageControl1.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl1.Name = "ultraTabPageControl1";
            this.ultraTabPageControl1.Size = new System.Drawing.Size(480, 232);
            // 
            // cubesExplorer
            // 
            this.cubesExplorer.AutoSize = true;
            this.cubesExplorer.Caption = "Заголовок";
            this.cubesExplorer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cubesExplorer.Location = new System.Drawing.Point(0, 0);
            this.cubesExplorer.Margin = new System.Windows.Forms.Padding(2);
            this.cubesExplorer.Name = "cubesExplorer";
            this.cubesExplorer.Size = new System.Drawing.Size(480, 232);
            this.cubesExplorer.TabIndex = 0;
            // 
            // ultraTabPageControl2
            // 
            this.ultraTabPageControl2.Controls.Add(this.dimsExplorer);
            this.ultraTabPageControl2.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl2.Name = "ultraTabPageControl2";
            this.ultraTabPageControl2.Size = new System.Drawing.Size(480, 232);
            // 
            // dimsExplorer
            // 
            this.dimsExplorer.AutoSize = true;
            this.dimsExplorer.Caption = "Заголовок";
            this.dimsExplorer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dimsExplorer.Location = new System.Drawing.Point(0, 0);
            this.dimsExplorer.Margin = new System.Windows.Forms.Padding(2);
            this.dimsExplorer.Name = "dimsExplorer";
            this.dimsExplorer.Size = new System.Drawing.Size(480, 232);
            this.dimsExplorer.TabIndex = 0;
            // 
            // ultraTabPageControl3
            // 
            this.ultraTabPageControl3.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl3.Name = "ultraTabPageControl3";
            this.ultraTabPageControl3.Size = new System.Drawing.Size(480, 232);

            // 
            // ultraTabPageControl4
            // 
            this.ultraTabPageControl4.Controls.Add(this.dsExplorer);
            this.ultraTabPageControl4.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl4.Name = "ultraTabPageControl1";
            this.ultraTabPageControl4.Size = new System.Drawing.Size(480, 232);
            // 
            // dsExplorer
            // 
            this.dsExplorer.AutoSize = true;
            this.dsExplorer.Caption = "Заголовок";
            this.dsExplorer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dsExplorer.Location = new System.Drawing.Point(0, 0);
            this.dsExplorer.Margin = new System.Windows.Forms.Padding(2);
            this.dsExplorer.Name = "dsExplorer";
            this.dsExplorer.Size = new System.Drawing.Size(480, 232);
            this.dsExplorer.TabIndex = 0;
            // 
            // tabCtrlDB
            // 
            this.tabCtrlDB.Controls.Add(this.ultraTabSharedControlsPage1);
            this.tabCtrlDB.Controls.Add(this.ultraTabPageControl1);
            this.tabCtrlDB.Controls.Add(this.ultraTabPageControl2);
            this.tabCtrlDB.Controls.Add(this.ultraTabPageControl3);
            this.tabCtrlDB.Controls.Add(this.ultraTabPageControl4);
            this.tabCtrlDB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabCtrlDB.Location = new System.Drawing.Point(0, 0);
            this.tabCtrlDB.Name = "tabCtrlDB";
            this.tabCtrlDB.SharedControlsPage = this.ultraTabSharedControlsPage1;
            this.tabCtrlDB.Size = new System.Drawing.Size(484, 255);
            this.tabCtrlDB.TabIndex = 0;
            ultraTab1.TabPage = this.ultraTabPageControl1;
            ultraTab1.Text = "Кубы";
            ultraTab2.TabPage = this.ultraTabPageControl2;
            ultraTab2.Text = "Измерения";
            ultraTab3.TabPage = this.ultraTabPageControl4;
            ultraTab3.Text = "Источники данных";
            ultraTab4.TabPage = this.ultraTabPageControl3;
            ultraTab4.Text = "Управляющая информация";
            this.tabCtrlDB.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab1,
            ultraTab2,
            ultraTab3,
            ultraTab4});
            this.tabCtrlDB.ViewStyle = Infragistics.Win.UltraWinTabControl.ViewStyle.VisualStudio2005;
            // 
            // ultraTabSharedControlsPage1
            // 
            this.ultraTabSharedControlsPage1.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage1.Name = "ultraTabSharedControlsPage1";
            this.ultraTabSharedControlsPage1.Size = new System.Drawing.Size(480, 232);
            // 
            // ultraTabPageControl4
            // 
            this.ultraTabPageControl4.Controls.Add(this.dsvExplorer);
            this.ultraTabPageControl4.Location = new System.Drawing.Point(2, 21);
            this.ultraTabPageControl4.Name = "ultraTabPageControl4";
            this.ultraTabPageControl4.Size = new System.Drawing.Size(480, 232);
            // 
            // dsvExplorer
            // 
            this.dsvExplorer.AutoSize = true;
            this.dsvExplorer.Caption = "Заголовок";
            this.dsvExplorer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dsvExplorer.Location = new System.Drawing.Point(0, 0);
            this.dsvExplorer.Margin = new System.Windows.Forms.Padding(2);
            this.dsvExplorer.Name = "dsvExplorer";
            this.dsvExplorer.Size = new System.Drawing.Size(480, 232);
            this.dsvExplorer.TabIndex = 0;
            // 
            // ctrlDBPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabCtrlDB);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "ctrlDBPage";
            this.Size = new System.Drawing.Size(484, 255);
            this.ultraTabPageControl1.ResumeLayout(false);
            this.ultraTabPageControl1.PerformLayout();
            this.ultraTabPageControl2.ResumeLayout(false);
            this.ultraTabPageControl2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tabCtrlDB)).EndInit();
            this.tabCtrlDB.ResumeLayout(false);
            this.ultraTabPageControl4.ResumeLayout(false);
            this.ultraTabPageControl4.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

        private Infragistics.Win.UltraWinTabControl.UltraTabControl tabCtrlDB;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage1;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl1;
        private ctrlObjectExplorer cubesExplorer;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl2;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl3;
        private ctrlObjectExplorer dimsExplorer;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl4;
	    private ctrlObjectExplorer dsExplorer;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl5;
        private ctrlObjectExplorer dsvExplorer;
	}
}
