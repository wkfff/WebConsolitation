namespace Krista.FM.Client.MDXExpert.Controls
{
    partial class MdxQueryEditor
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
            this.mdxQueryControl = new Krista.FM.Client.MDXExpert.Controls.MdxQueryControl();
            this.ubApply = new Infragistics.Win.Misc.UltraButton();
            this.ubCancel = new Infragistics.Win.Misc.UltraButton();
            this.ubOk = new Infragistics.Win.Misc.UltraButton();
            this.ugbParent = new Infragistics.Win.Misc.UltraGroupBox();
            this.ugbControlPlace = new Infragistics.Win.Misc.UltraGroupBox();
            this.ugbInterfacePlace = new Infragistics.Win.Misc.UltraGroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.ugbParent)).BeginInit();
            this.ugbParent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ugbControlPlace)).BeginInit();
            this.ugbControlPlace.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ugbInterfacePlace)).BeginInit();
            this.ugbInterfacePlace.SuspendLayout();
            this.SuspendLayout();
            // 
            // mdxQueryControl
            // 
            this.mdxQueryControl.AutoSaveQuery = true;
            this.mdxQueryControl.CurrentPivotData = null;
            this.mdxQueryControl.DisplayMode = Krista.FM.Client.MDXExpert.Controls.ControlDisplayMode.Simple;
            this.mdxQueryControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mdxQueryControl.Location = new System.Drawing.Point(1, 0);
            this.mdxQueryControl.Name = "mdxQueryControl";
            this.mdxQueryControl.Size = new System.Drawing.Size(662, 397);
            this.mdxQueryControl.TabIndex = 0;
            // 
            // ubApply
            // 
            this.ubApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ubApply.Location = new System.Drawing.Point(494, 8);
            this.ubApply.Name = "ubApply";
            this.ubApply.Size = new System.Drawing.Size(75, 23);
            this.ubApply.TabIndex = 2;
            this.ubApply.Text = "Применить";
            this.ubApply.Click += new System.EventHandler(this.ubApply_Click);
            // 
            // ubCancel
            // 
            this.ubCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ubCancel.Location = new System.Drawing.Point(582, 8);
            this.ubCancel.Name = "ubCancel";
            this.ubCancel.Size = new System.Drawing.Size(75, 23);
            this.ubCancel.TabIndex = 1;
            this.ubCancel.Text = "Отмена";
            this.ubCancel.Click += new System.EventHandler(this.ubCancel_Click);
            // 
            // ubOk
            // 
            this.ubOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ubOk.Location = new System.Drawing.Point(413, 8);
            this.ubOk.Name = "ubOk";
            this.ubOk.Size = new System.Drawing.Size(75, 23);
            this.ubOk.TabIndex = 0;
            this.ubOk.Text = "ОК";
            this.ubOk.Click += new System.EventHandler(this.ubOk_Click);
            // 
            // ugbParent
            // 
            this.ugbParent.BorderStyle = Infragistics.Win.Misc.GroupBoxBorderStyle.None;
            this.ugbParent.Controls.Add(this.ugbControlPlace);
            this.ugbParent.Controls.Add(this.ugbInterfacePlace);
            this.ugbParent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ugbParent.Location = new System.Drawing.Point(0, 0);
            this.ugbParent.Name = "ugbParent";
            this.ugbParent.Size = new System.Drawing.Size(666, 438);
            this.ugbParent.TabIndex = 3;
            // 
            // ugbControlPlace
            // 
            this.ugbControlPlace.BorderStyle = Infragistics.Win.Misc.GroupBoxBorderStyle.None;
            this.ugbControlPlace.Controls.Add(this.mdxQueryControl);
            this.ugbControlPlace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ugbControlPlace.Location = new System.Drawing.Point(1, 0);
            this.ugbControlPlace.Name = "ugbControlPlace";
            this.ugbControlPlace.Size = new System.Drawing.Size(664, 398);
            this.ugbControlPlace.TabIndex = 4;
            // 
            // ugbInterfacePlace
            // 
            this.ugbInterfacePlace.Controls.Add(this.ubCancel);
            this.ugbInterfacePlace.Controls.Add(this.ubApply);
            this.ugbInterfacePlace.Controls.Add(this.ubOk);
            this.ugbInterfacePlace.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ugbInterfacePlace.Location = new System.Drawing.Point(1, 398);
            this.ugbInterfacePlace.Name = "ugbInterfacePlace";
            this.ugbInterfacePlace.Size = new System.Drawing.Size(664, 39);
            this.ugbInterfacePlace.TabIndex = 3;
            // 
            // MdxQueryEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(666, 438);
            this.Controls.Add(this.ugbParent);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "MdxQueryEditor";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Редактор MDX запросов";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.ugbParent)).EndInit();
            this.ugbParent.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ugbControlPlace)).EndInit();
            this.ugbControlPlace.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ugbInterfacePlace)).EndInit();
            this.ugbInterfacePlace.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraButton ubCancel;
        private Infragistics.Win.Misc.UltraButton ubOk;
        private MdxQueryControl mdxQueryControl;
        private Infragistics.Win.Misc.UltraButton ubApply;
        private Infragistics.Win.Misc.UltraGroupBox ugbParent;
        private Infragistics.Win.Misc.UltraGroupBox ugbControlPlace;
        private Infragistics.Win.Misc.UltraGroupBox ugbInterfacePlace;
    }
}