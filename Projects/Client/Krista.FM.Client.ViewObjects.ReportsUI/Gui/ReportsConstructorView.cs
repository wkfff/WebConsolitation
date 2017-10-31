using System;
using Krista.FM.Client.ViewObjects.BaseViewObject;

namespace Krista.FM.Client.ViewObjects.ReportsUI.Gui
{
    public class ReportsConstructorView : BaseView
    {
        private System.Windows.Forms.MenuStrip menuMain;
        private System.Windows.Forms.ToolStripMenuItem miLoad;
        private System.Windows.Forms.ToolStripMenuItem miSave;
        private System.Windows.Forms.ToolStripMenuItem miCreate;

        public ReportsConstructorView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.menuMain = new System.Windows.Forms.MenuStrip();
            this.miLoad = new System.Windows.Forms.ToolStripMenuItem();
            this.miSave = new System.Windows.Forms.ToolStripMenuItem();
            this.miCreate = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuMain
            // 
            this.menuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miLoad,
            this.miSave,
            this.miCreate});
            this.menuMain.Location = new System.Drawing.Point(0, 0);
            this.menuMain.Name = "menuMain";
            this.menuMain.Size = new System.Drawing.Size(588, 24);
            this.menuMain.TabIndex = 1;
            this.menuMain.Text = "menuStrip1";
            // 
            // miLoad
            // 
            this.miLoad.Name = "miLoad";
            this.miLoad.Size = new System.Drawing.Size(71, 20);
            this.miLoad.Text = "Загрузить";
            this.miLoad.Click += new System.EventHandler(this.miLoad_Click);
            // 
            // miSave
            // 
            this.miSave.Name = "miSave";
            this.miSave.Size = new System.Drawing.Size(74, 20);
            this.miSave.Text = "Сохранить";
            this.miSave.Click += new System.EventHandler(this.miSave_Click);
            // 
            // miCreate
            // 
            this.miCreate.Name = "miCreate";
            this.miCreate.Size = new System.Drawing.Size(94, 20);
            this.miCreate.Text = "Сформировать";
            this.miCreate.Click += new System.EventHandler(this.miCreate_Click);
            // 
            // ReportsConstructorView
            // 
            this.Controls.Add(this.menuMain);
            this.Name = "ReportsConstructorView";
            this.Resize += new System.EventHandler(this.ReportsConstructorView_Resize);
            this.menuMain.ResumeLayout(false);
            this.menuMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void ReportsConstructorView_Resize(object sender, EventArgs e)
        {
        }

        private void miLoad_Click(object sender, EventArgs e)
        {
        }

        private void miSave_Click(object sender, EventArgs e)
        {
        }

        private void miCreate_Click(object sender, EventArgs e)
        {
        }
    }
}
