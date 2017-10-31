using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Krista.FM.Client.ViewObjects.BaseViewObject;

namespace Krista.FM.Client.ViewObjects.ReportsUI.Gui
{
    public class ReportsView : BaseView
    {
        private System.Windows.Forms.ContextMenuStrip cmsReportActions;
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.ToolStripMenuItem создатьОтчетToolStripMenuItem;
        internal Infragistics.Win.UltraWinTree.UltraTree tReports;
    
        public ReportsView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tReports = new Infragistics.Win.UltraWinTree.UltraTree();
            this.cmsReportActions = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.создатьОтчетToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.tReports)).BeginInit();
            this.cmsReportActions.SuspendLayout();
            this.SuspendLayout();
            // 
            // tReports
            // 
            this.tReports.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.tReports.ContextMenuStrip = this.cmsReportActions;
            this.tReports.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tReports.Location = new System.Drawing.Point(0, 0);
            this.tReports.Name = "tReports";
            this.tReports.Size = new System.Drawing.Size(588, 536);
            this.tReports.TabIndex = 0;
            // 
            // cmsReportActions
            // 
            this.cmsReportActions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.создатьОтчетToolStripMenuItem});
            this.cmsReportActions.Name = "cmsReportActions";
            this.cmsReportActions.Size = new System.Drawing.Size(151, 26);
            // 
            // создатьОтчетToolStripMenuItem
            // 
            this.создатьОтчетToolStripMenuItem.Name = "создатьОтчетToolStripMenuItem";
            this.создатьОтчетToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.создатьОтчетToolStripMenuItem.Text = "Создать отчет";
            // 
            // ReportsView
            // 
            this.Controls.Add(this.tReports);
            this.Name = "ReportsView";
            ((System.ComponentModel.ISupportInitialize)(this.tReports)).EndInit();
            this.cmsReportActions.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }
}
