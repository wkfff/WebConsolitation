namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Capital
{
    partial class CapitalDetailControl
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

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Обязательный метод для поддержки конструктора - не изменяйте 
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.ugePlan = new Krista.FM.Client.Components.UltraGridEx();
            this.ugeFact = new Krista.FM.Client.Components.UltraGridEx();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.ugePlan);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.ugeFact);
            this.splitContainer1.Size = new System.Drawing.Size(864, 460);
            this.splitContainer1.SplitterDistance = 420;
            this.splitContainer1.TabIndex = 0;
            // 
            // ugePlan
            // 
            this.ugePlan.AllowAddNewRecords = true;
            this.ugePlan.AllowClearTable = true;
            this.ugePlan.Caption = "";
            this.ugePlan.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ugePlan.EnableGroups = false;
            this.ugePlan.InDebugMode = false;
            this.ugePlan.LoadMenuVisible = false;
            this.ugePlan.Location = new System.Drawing.Point(0, 0);
            this.ugePlan.MaxCalendarDate = new System.DateTime(((long)(0)));
            this.ugePlan.MinCalendarDate = new System.DateTime(((long)(0)));
            this.ugePlan.Name = "ugePlan";
            this.ugePlan.SaveLoadFileName = "";
            this.ugePlan.SaveMenuVisible = false;
            this.ugePlan.ServerFilterEnabled = false;
            this.ugePlan.SingleBandLevelName = "Добавить запись...";
            this.ugePlan.Size = new System.Drawing.Size(420, 460);
            this.ugePlan.sortColumnName = "";
            this.ugePlan.StateRowEnable = false;
            this.ugePlan.TabIndex = 0;
            // 
            // ugeFact
            // 
            this.ugeFact.AllowAddNewRecords = true;
            this.ugeFact.AllowClearTable = true;
            this.ugeFact.Caption = "";
            this.ugeFact.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ugeFact.EnableGroups = false;
            this.ugeFact.InDebugMode = false;
            this.ugeFact.LoadMenuVisible = false;
            this.ugeFact.Location = new System.Drawing.Point(0, 0);
            this.ugeFact.MaxCalendarDate = new System.DateTime(((long)(0)));
            this.ugeFact.MinCalendarDate = new System.DateTime(((long)(0)));
            this.ugeFact.Name = "ugeFact";
            this.ugeFact.SaveLoadFileName = "";
            this.ugeFact.SaveMenuVisible = false;
            this.ugeFact.ServerFilterEnabled = false;
            this.ugeFact.SingleBandLevelName = "Добавить запись...";
            this.ugeFact.Size = new System.Drawing.Size(440, 460);
            this.ugeFact.sortColumnName = "";
            this.ugeFact.StateRowEnable = false;
            this.ugeFact.TabIndex = 0;
            // 
            // CapitalDetailControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "CapitalDetailControl";
            this.Size = new System.Drawing.Size(864, 460);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        internal Components.UltraGridEx ugePlan;
        internal Components.UltraGridEx ugeFact;
    }
}
