namespace Krista.FM.Client.ViewObjects.TasksUI
{
    partial class FormChildTaskParams
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
            this.cbBeginDate = new System.Windows.Forms.CheckBox();
            this.cbEndDate = new System.Windows.Forms.CheckBox();
            this.cbOwner = new System.Windows.Forms.CheckBox();
            this.cbDoer = new System.Windows.Forms.CheckBox();
            this.cbCurator = new System.Windows.Forms.CheckBox();
            this.cbTaskKind = new System.Windows.Forms.CheckBox();
            this.cbGroups = new System.Windows.Forms.CheckBox();
            this.cbUsers = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cbBeginDate
            // 
            this.cbBeginDate.AutoSize = true;
            this.cbBeginDate.Location = new System.Drawing.Point(12, 14);
            this.cbBeginDate.Name = "cbBeginDate";
            this.cbBeginDate.Size = new System.Drawing.Size(90, 17);
            this.cbBeginDate.TabIndex = 0;
            this.cbBeginDate.Text = "Дата начала";
            this.cbBeginDate.UseVisualStyleBackColor = true;
            this.cbBeginDate.CheckedChanged += new System.EventHandler(this.cbBeginDate_CheckedChanged);
            // 
            // cbEndDate
            // 
            this.cbEndDate.AutoSize = true;
            this.cbEndDate.Location = new System.Drawing.Point(12, 37);
            this.cbEndDate.Name = "cbEndDate";
            this.cbEndDate.Size = new System.Drawing.Size(117, 17);
            this.cbEndDate.TabIndex = 0;
            this.cbEndDate.Text = "Дата завершения";
            this.cbEndDate.UseVisualStyleBackColor = true;
            this.cbEndDate.CheckedChanged += new System.EventHandler(this.cbBeginDate_CheckedChanged);
            // 
            // cbOwner
            // 
            this.cbOwner.AutoSize = true;
            this.cbOwner.Location = new System.Drawing.Point(12, 60);
            this.cbOwner.Name = "cbOwner";
            this.cbOwner.Size = new System.Drawing.Size(75, 17);
            this.cbOwner.TabIndex = 0;
            this.cbOwner.Text = "Владелец";
            this.cbOwner.UseVisualStyleBackColor = true;
            this.cbOwner.CheckedChanged += new System.EventHandler(this.cbBeginDate_CheckedChanged);
            // 
            // cbDoer
            // 
            this.cbDoer.AutoSize = true;
            this.cbDoer.Location = new System.Drawing.Point(12, 83);
            this.cbDoer.Name = "cbDoer";
            this.cbDoer.Size = new System.Drawing.Size(93, 17);
            this.cbDoer.TabIndex = 0;
            this.cbDoer.Text = "Исполнитель";
            this.cbDoer.UseVisualStyleBackColor = true;
            this.cbDoer.CheckedChanged += new System.EventHandler(this.cbBeginDate_CheckedChanged);
            // 
            // cbCurator
            // 
            this.cbCurator.AutoSize = true;
            this.cbCurator.Location = new System.Drawing.Point(12, 106);
            this.cbCurator.Name = "cbCurator";
            this.cbCurator.Size = new System.Drawing.Size(67, 17);
            this.cbCurator.TabIndex = 0;
            this.cbCurator.Text = "Куратор";
            this.cbCurator.UseVisualStyleBackColor = true;
            this.cbCurator.CheckedChanged += new System.EventHandler(this.cbBeginDate_CheckedChanged);
            // 
            // cbTaskKind
            // 
            this.cbTaskKind.AutoSize = true;
            this.cbTaskKind.Location = new System.Drawing.Point(12, 129);
            this.cbTaskKind.Name = "cbTaskKind";
            this.cbTaskKind.Size = new System.Drawing.Size(83, 17);
            this.cbTaskKind.TabIndex = 0;
            this.cbTaskKind.Text = "Вид задачи";
            this.cbTaskKind.UseVisualStyleBackColor = true;
            this.cbTaskKind.CheckedChanged += new System.EventHandler(this.cbBeginDate_CheckedChanged);
            // 
            // cbGroups
            // 
            this.cbGroups.AutoSize = true;
            this.cbGroups.Location = new System.Drawing.Point(12, 152);
            this.cbGroups.Name = "cbGroups";
            this.cbGroups.Size = new System.Drawing.Size(89, 17);
            this.cbGroups.TabIndex = 0;
            this.cbGroups.Text = "Права групп";
            this.cbGroups.UseVisualStyleBackColor = true;
            this.cbGroups.CheckedChanged += new System.EventHandler(this.cbBeginDate_CheckedChanged);
            // 
            // cbUsers
            // 
            this.cbUsers.AutoSize = true;
            this.cbUsers.Location = new System.Drawing.Point(12, 175);
            this.cbUsers.Name = "cbUsers";
            this.cbUsers.Size = new System.Drawing.Size(138, 17);
            this.cbUsers.TabIndex = 0;
            this.cbUsers.Text = "Права пользователей";
            this.cbUsers.UseVisualStyleBackColor = true;
            this.cbUsers.CheckedChanged += new System.EventHandler(this.cbBeginDate_CheckedChanged);
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(75, 209);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Применить";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(156, 209);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "Отмена";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // FormChildTaskParams
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(243, 244);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.cbUsers);
            this.Controls.Add(this.cbGroups);
            this.Controls.Add(this.cbTaskKind);
            this.Controls.Add(this.cbCurator);
            this.Controls.Add(this.cbDoer);
            this.Controls.Add(this.cbOwner);
            this.Controls.Add(this.cbEndDate);
            this.Controls.Add(this.cbBeginDate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormChildTaskParams";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Применить к подчиненным задачам";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cbBeginDate;
        private System.Windows.Forms.CheckBox cbEndDate;
        private System.Windows.Forms.CheckBox cbOwner;
        private System.Windows.Forms.CheckBox cbDoer;
        private System.Windows.Forms.CheckBox cbCurator;
        private System.Windows.Forms.CheckBox cbTaskKind;
        private System.Windows.Forms.CheckBox cbGroups;
        private System.Windows.Forms.CheckBox cbUsers;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}