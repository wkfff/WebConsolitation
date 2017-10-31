namespace Krista.FM.Client.iMonitoringWM.Controls
{
    partial class ClosedDialog
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
            this.tbMessage = new System.Windows.Forms.TextBox();
            this.btYes = new System.Windows.Forms.Button();
            this.btClose = new System.Windows.Forms.Button();
            this.btMinimize = new System.Windows.Forms.Button();
            this.caption = new Krista.FM.Client.iMonitoringWM.Controls.GradientPanel();
            this.SuspendLayout();
            // 
            // tbMessage
            // 
            this.tbMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbMessage.BackColor = System.Drawing.Color.White;
            this.tbMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbMessage.Location = new System.Drawing.Point(8, 37);
            this.tbMessage.Multiline = true;
            this.tbMessage.Name = "tbMessage";
            this.tbMessage.Size = new System.Drawing.Size(208, 22);
            this.tbMessage.TabIndex = 1;
            this.tbMessage.Text = "Выйти из iМониторинг?";
            // 
            // btYes
            // 
            this.btYes.BackColor = System.Drawing.Color.Gainsboro;
            this.btYes.Location = new System.Drawing.Point(8, 65);
            this.btYes.Name = "btYes";
            this.btYes.Size = new System.Drawing.Size(65, 20);
            this.btYes.TabIndex = 2;
            this.btYes.Text = "Да";
            this.btYes.Click += new System.EventHandler(this.btYes_Click);
            // 
            // btClose
            // 
            this.btClose.BackColor = System.Drawing.Color.Gainsboro;
            this.btClose.Location = new System.Drawing.Point(79, 65);
            this.btClose.Name = "btClose";
            this.btClose.Size = new System.Drawing.Size(65, 20);
            this.btClose.TabIndex = 3;
            this.btClose.Text = "Отмена";
            this.btClose.Click += new System.EventHandler(this.btClose_Click);
            // 
            // btMinimize
            // 
            this.btMinimize.BackColor = System.Drawing.Color.Gainsboro;
            this.btMinimize.Location = new System.Drawing.Point(153, 65);
            this.btMinimize.Name = "btMinimize";
            this.btMinimize.Size = new System.Drawing.Size(65, 20);
            this.btMinimize.TabIndex = 4;
            this.btMinimize.Text = "Свернуть";
            this.btMinimize.Click += new System.EventHandler(this.btMinimize_Click);
            // 
            // caption
            // 
            this.caption.BackColor = System.Drawing.Color.LightGray;
            this.caption.BorderColor = System.Drawing.Color.Black;
            this.caption.Dock = System.Windows.Forms.DockStyle.Top;
            this.caption.EndColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.caption.ForeColor = System.Drawing.Color.White;
            this.caption.Location = new System.Drawing.Point(0, 0);
            this.caption.Name = "caption";
            this.caption.Size = new System.Drawing.Size(224, 20);
            this.caption.StartColor = System.Drawing.Color.Silver;
            this.caption.TabIndex = 0;
            this.caption.Text = "Выйти из iМониторинг?";
            // 
            // ClosedDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.btMinimize);
            this.Controls.Add(this.btClose);
            this.Controls.Add(this.btYes);
            this.Controls.Add(this.tbMessage);
            this.Controls.Add(this.caption);
            this.Name = "ClosedDialog";
            this.Size = new System.Drawing.Size(224, 90);
            this.ResumeLayout(false);

        }

        #endregion

        private GradientPanel caption;
        private System.Windows.Forms.TextBox tbMessage;
        private System.Windows.Forms.Button btYes;
        private System.Windows.Forms.Button btClose;
        private System.Windows.Forms.Button btMinimize;
    }
}
