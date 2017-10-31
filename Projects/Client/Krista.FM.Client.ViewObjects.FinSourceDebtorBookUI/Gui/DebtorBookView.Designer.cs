namespace Krista.FM.Client.ViewObjects.FinSourceDebtorBookUI
{
    partial class DebtorBookView : BaseViewObject.BaseView
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
            this.ultraTabPageControl1 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.ultraTabPageControl2 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.ultraTabPageControl3 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.utcDebtorBookData = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this.ultraTabSharedControlsPage1 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.pSubject = new System.Windows.Forms.Panel();
            this.pRegion = new System.Windows.Forms.Panel();
            this.pSettlement = new System.Windows.Forms.Panel();
            this.ultraTabPageControl1.SuspendLayout();
            this.ultraTabPageControl2.SuspendLayout();
            this.ultraTabPageControl3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.utcDebtorBookData)).BeginInit();
            this.utcDebtorBookData.SuspendLayout();
            this.SuspendLayout();
            // 
            // ultraTabPageControl1
            // 
            this.ultraTabPageControl1.Controls.Add(this.pSubject);
            this.ultraTabPageControl1.Location = new System.Drawing.Point(1, 20);
            this.ultraTabPageControl1.Name = "ultraTabPageControl1";
            this.ultraTabPageControl1.Size = new System.Drawing.Size(586, 515);
            // 
            // ultraTabPageControl2
            // 
            this.ultraTabPageControl2.Controls.Add(this.pRegion);
            this.ultraTabPageControl2.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl2.Name = "ultraTabPageControl2";
            this.ultraTabPageControl2.Size = new System.Drawing.Size(586, 515);
            // 
            // ultraTabPageControl3
            // 
            this.ultraTabPageControl3.Controls.Add(this.pSettlement);
            this.ultraTabPageControl3.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl3.Name = "ultraTabPageControl3";
            this.ultraTabPageControl3.Size = new System.Drawing.Size(586, 515);
            // 
            // utcDebtorBookData
            // 
            this.utcDebtorBookData.Controls.Add(this.ultraTabSharedControlsPage1);
            this.utcDebtorBookData.Controls.Add(this.ultraTabPageControl1);
            this.utcDebtorBookData.Controls.Add(this.ultraTabPageControl2);
            this.utcDebtorBookData.Controls.Add(this.ultraTabPageControl3);
            this.utcDebtorBookData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.utcDebtorBookData.Location = new System.Drawing.Point(0, 0);
            this.utcDebtorBookData.Name = "utcDebtorBookData";
            this.utcDebtorBookData.SharedControlsPage = this.ultraTabSharedControlsPage1;
            this.utcDebtorBookData.Size = new System.Drawing.Size(588, 536);
            this.utcDebtorBookData.Style = Infragistics.Win.UltraWinTabControl.UltraTabControlStyle.Excel;
            this.utcDebtorBookData.TabIndex = 0;
            ultraTab1.Key = "subjectRF";
            ultraTab1.TabPage = this.ultraTabPageControl1;
            ultraTab1.Text = "Данные по субъекту РФ";
            ultraTab2.Key = "region";
            ultraTab2.TabPage = this.ultraTabPageControl2;
            ultraTab2.Text = "Данные по районам, городским округам";
            ultraTab3.Key = "settlement";
            ultraTab3.TabPage = this.ultraTabPageControl3;
            ultraTab3.Text = "Данные по поселениям";
            this.utcDebtorBookData.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab1,
            ultraTab2,
            ultraTab3});
            // 
            // ultraTabSharedControlsPage1
            // 
            this.ultraTabSharedControlsPage1.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage1.Name = "ultraTabSharedControlsPage1";
            this.ultraTabSharedControlsPage1.Size = new System.Drawing.Size(586, 515);
            // 
            // pSubject
            // 
            this.pSubject.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pSubject.Location = new System.Drawing.Point(0, 0);
            this.pSubject.Name = "pSubject";
            this.pSubject.Size = new System.Drawing.Size(586, 515);
            this.pSubject.TabIndex = 0;
            // 
            // pRegion
            // 
            this.pRegion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pRegion.Location = new System.Drawing.Point(0, 0);
            this.pRegion.Name = "pRegion";
            this.pRegion.Size = new System.Drawing.Size(586, 515);
            this.pRegion.TabIndex = 0;
            // 
            // pSettlement
            // 
            this.pSettlement.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pSettlement.Location = new System.Drawing.Point(0, 0);
            this.pSettlement.Name = "pSettlement";
            this.pSettlement.Size = new System.Drawing.Size(586, 515);
            this.pSettlement.TabIndex = 0;
            // 
            // DebtorBookView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.utcDebtorBookData);
            this.Name = "DebtorBookView";
            this.ultraTabPageControl1.ResumeLayout(false);
            this.ultraTabPageControl2.ResumeLayout(false);
            this.ultraTabPageControl3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.utcDebtorBookData)).EndInit();
            this.utcDebtorBookData.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage1;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl1;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl2;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl3;
        public Infragistics.Win.UltraWinTabControl.UltraTabControl utcDebtorBookData;
        private System.Windows.Forms.Panel pSubject;
        private System.Windows.Forms.Panel pRegion;
        private System.Windows.Forms.Panel pSettlement;
    }
}
