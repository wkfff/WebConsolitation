using Infragistics.Win.Misc;

namespace Krista.FM.Client.ViewObjects.TasksUI
{
    partial class FormLockedTasksAction
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLockedTasksAction));
            this.label1 = new UltraLabel();
            this.pbQuestion = new System.Windows.Forms.PictureBox();
            this.ugLockedTasks = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnApplay = new System.Windows.Forms.Button();
            this.btnContinueWork = new System.Windows.Forms.Button();
            this.il = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pbQuestion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ugLockedTasks)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(66, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(341, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Обнаружены задачи, заблокированные текущим пользователем:";
            // 
            // pbQuestion
            // 
            this.pbQuestion.Image = ((System.Drawing.Image)(resources.GetObject("pbQuestion.Image")));
            this.pbQuestion.Location = new System.Drawing.Point(12, 12);
            this.pbQuestion.Name = "pbQuestion";
            this.pbQuestion.Size = new System.Drawing.Size(48, 48);
            this.pbQuestion.TabIndex = 8;
            this.pbQuestion.TabStop = false;
            // 
            // ugLockedTasks
            // 
            this.ugLockedTasks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ugLockedTasks.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.ugLockedTasks.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.ugLockedTasks.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.ugLockedTasks.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.ugLockedTasks.Location = new System.Drawing.Point(12, 69);
            this.ugLockedTasks.Name = "ugLockedTasks";
            this.ugLockedTasks.Size = new System.Drawing.Size(608, 333);
            this.ugLockedTasks.TabIndex = 1;
            this.ugLockedTasks.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(ugLockedTasks_InitializeLayout);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.ImageIndex = 0;
            this.btnCancel.ImageList = this.il;
            this.btnCancel.Location = new System.Drawing.Point(480, 418);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(140, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Закрыть приложение";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnApplay
            // 
            this.btnApplay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApplay.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnApplay.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnApplay.ImageIndex = 2;
            this.btnApplay.ImageList = this.il;
            this.btnApplay.Location = new System.Drawing.Point(328, 418);
            this.btnApplay.Name = "btnApplay";
            this.btnApplay.Size = new System.Drawing.Size(146, 23);
            this.btnApplay.TabIndex = 4;
            this.btnApplay.Text = "Применить и закрыть";
            this.btnApplay.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnApplay.UseVisualStyleBackColor = true;
            // 
            // btnContinueWork
            // 
            this.btnContinueWork.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnContinueWork.DialogResult = System.Windows.Forms.DialogResult.Ignore;
            this.btnContinueWork.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnContinueWork.ImageIndex = 1;
            this.btnContinueWork.ImageList = this.il;
            this.btnContinueWork.Location = new System.Drawing.Point(182, 418);
            this.btnContinueWork.Name = "btnContinueWork";
            this.btnContinueWork.Size = new System.Drawing.Size(140, 23);
            this.btnContinueWork.TabIndex = 9;
            this.btnContinueWork.Text = "   Продолжить работу";
            this.btnContinueWork.UseVisualStyleBackColor = true;
            // 
            // il
            // 
            this.il.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("il.ImageStream")));
            this.il.TransparentColor = System.Drawing.Color.Magenta;
            this.il.Images.SetKeyName(0, "ExitDoor.bmp");
            this.il.Images.SetKeyName(1, "GotoAssociation.bmp");
            this.il.Images.SetKeyName(2, "Save.bmp");
            // 
            // FormLockedTasksAction
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(632, 453);
            this.Controls.Add(this.btnContinueWork);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnApplay);
            this.Controls.Add(this.ugLockedTasks);
            this.Controls.Add(this.pbQuestion);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormLockedTasksAction";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Выбор действия над заблокированными задачами";
            ((System.ComponentModel.ISupportInitialize)(this.pbQuestion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ugLockedTasks)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private UltraLabel label1;
        private System.Windows.Forms.PictureBox pbQuestion;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnApplay;
        private System.Windows.Forms.Button btnContinueWork;
        public Infragistics.Win.UltraWinGrid.UltraGrid ugLockedTasks;
        private System.Windows.Forms.ImageList il;
    }
}