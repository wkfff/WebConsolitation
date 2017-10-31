namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Forms
{
    partial class SelectCommentForm
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
            this.tbComment = new System.Windows.Forms.TextBox();
            this.cbCommentsList = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ultraButton1 = new Infragistics.Win.Misc.UltraButton();
            this.ultraButton2 = new Infragistics.Win.Misc.UltraButton();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // tbComment
            // 
            this.tbComment.Location = new System.Drawing.Point(12, 78);
            this.tbComment.MaxLength = 255;
            this.tbComment.Multiline = true;
            this.tbComment.Name = "tbComment";
            this.tbComment.Size = new System.Drawing.Size(321, 61);
            this.tbComment.TabIndex = 0;
            this.tbComment.TextChanged += new System.EventHandler(this.tbComment_TextChanged);
            // 
            // cbCommentsList
            // 
            this.cbCommentsList.FormattingEnabled = true;
            this.cbCommentsList.Location = new System.Drawing.Point(12, 25);
            this.cbCommentsList.Name = "cbCommentsList";
            this.cbCommentsList.Size = new System.Drawing.Size(321, 21);
            this.cbCommentsList.TabIndex = 1;
            this.cbCommentsList.SelectedIndexChanged += new System.EventHandler(this.cbCommentsList_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(122, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Список комментариев";
            // 
            // ultraButton1
            // 
            this.ultraButton1.Enabled = false;
            this.ultraButton1.Location = new System.Drawing.Point(177, 145);
            this.ultraButton1.Name = "ultraButton1";
            this.ultraButton1.Size = new System.Drawing.Size(75, 23);
            this.ultraButton1.TabIndex = 3;
            this.ultraButton1.Text = "ОК";
            this.ultraButton1.Click += new System.EventHandler(this.ultraButton1_Click);
            // 
            // ultraButton2
            // 
            this.ultraButton2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ultraButton2.Location = new System.Drawing.Point(258, 145);
            this.ultraButton2.Name = "ultraButton2";
            this.ultraButton2.Size = new System.Drawing.Size(75, 23);
            this.ultraButton2.TabIndex = 3;
            this.ultraButton2.Text = "Отмена";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Комментарий";
            // 
            // SelectCommentForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(345, 179);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ultraButton2);
            this.Controls.Add(this.ultraButton1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbCommentsList);
            this.Controls.Add(this.tbComment);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectCommentForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Комментарий к расчету";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbComment;
        private System.Windows.Forms.ComboBox cbCommentsList;
        private System.Windows.Forms.Label label1;
        private Infragistics.Win.Misc.UltraButton ultraButton1;
        private Infragistics.Win.Misc.UltraButton ultraButton2;
        private System.Windows.Forms.Label label2;
    }
}