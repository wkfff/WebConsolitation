namespace Krista.FM.Utils.SQLEditor
{
    partial class frmSqlWindow
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
            this.dgvResults = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnExplain = new System.Windows.Forms.Button();
            this.btnRollback = new System.Windows.Forms.Button();
            this.btnCommit = new System.Windows.Forms.Button();
            this.btnRun = new System.Windows.Forms.Button();
            this.btnClearHist = new System.Windows.Forms.Button();
            this.btnSQLHist = new System.Windows.Forms.Button();
            this.btnNextSql = new System.Windows.Forms.Button();
            this.btnPrevSql = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.arrangeSQLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.txtSqlArea = new Krista.FM.Utils.SQLEditor.RichDDLBox();
            this.btOk = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResults)).BeginInit();
            this.panel1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvResults
            // 
            this.dgvResults.BackgroundColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.dgvResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvResults.Location = new System.Drawing.Point(0, 296);
            this.dgvResults.Name = "dgvResults";
            this.dgvResults.Size = new System.Drawing.Size(604, 123);
            this.dgvResults.TabIndex = 1;
            this.dgvResults.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgvResults_DataError);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btOk);
            this.panel1.Controls.Add(this.btnExplain);
            this.panel1.Controls.Add(this.btnRollback);
            this.panel1.Controls.Add(this.btnCommit);
            this.panel1.Controls.Add(this.btnRun);
            this.panel1.Controls.Add(this.btnClearHist);
            this.panel1.Controls.Add(this.btnSQLHist);
            this.panel1.Controls.Add(this.btnNextSql);
            this.panel1.Controls.Add(this.btnPrevSql);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(612, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(48, 418);
            this.panel1.TabIndex = 4;
            // 
            // btnExplain
            // 
            this.btnExplain.FlatAppearance.BorderSize = 0;
            this.btnExplain.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExplain.Image = global::Krista.FM.Client.Common.SQLForm.Properties.Resources.explain_small;
            this.btnExplain.Location = new System.Drawing.Point(9, 224);
            this.btnExplain.Name = "btnExplain";
            this.btnExplain.Size = new System.Drawing.Size(31, 32);
            this.btnExplain.TabIndex = 7;
            this.btnExplain.Tag = "Run";
            this.btnExplain.UseVisualStyleBackColor = true;
            this.btnExplain.Click += new System.EventHandler(this.btnExplain_Click);
            // 
            // btnRollback
            // 
            this.btnRollback.FlatAppearance.BorderSize = 0;
            this.btnRollback.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRollback.Image = global::Krista.FM.Client.Common.SQLForm.Properties.Resources.cancel;
            this.btnRollback.Location = new System.Drawing.Point(9, 300);
            this.btnRollback.Name = "btnRollback";
            this.btnRollback.Size = new System.Drawing.Size(31, 32);
            this.btnRollback.TabIndex = 6;
            this.btnRollback.Tag = "Rollback";
            this.btnRollback.UseVisualStyleBackColor = true;
            this.btnRollback.Click += new System.EventHandler(this.btnRollback_Click);
            // 
            // btnCommit
            // 
            this.btnCommit.FlatAppearance.BorderSize = 0;
            this.btnCommit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCommit.Image = global::Krista.FM.Client.Common.SQLForm.Properties.Resources.apply;
            this.btnCommit.Location = new System.Drawing.Point(9, 262);
            this.btnCommit.Name = "btnCommit";
            this.btnCommit.Size = new System.Drawing.Size(31, 32);
            this.btnCommit.TabIndex = 5;
            this.btnCommit.Tag = "Commit";
            this.btnCommit.UseVisualStyleBackColor = true;
            this.btnCommit.Click += new System.EventHandler(this.btnCommit_Click);
            // 
            // btnRun
            // 
            this.btnRun.FlatAppearance.BorderSize = 0;
            this.btnRun.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRun.Image = global::Krista.FM.Client.Common.SQLForm.Properties.Resources.forward;
            this.btnRun.Location = new System.Drawing.Point(9, 186);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(31, 32);
            this.btnRun.TabIndex = 4;
            this.btnRun.Tag = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // btnClearHist
            // 
            this.btnClearHist.FlatAppearance.BorderSize = 0;
            this.btnClearHist.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClearHist.Image = global::Krista.FM.Client.Common.SQLForm.Properties.Resources.Bin_empty_small;
            this.btnClearHist.Location = new System.Drawing.Point(9, 126);
            this.btnClearHist.Name = "btnClearHist";
            this.btnClearHist.Size = new System.Drawing.Size(31, 32);
            this.btnClearHist.TabIndex = 3;
            this.btnClearHist.UseVisualStyleBackColor = true;
            this.btnClearHist.Click += new System.EventHandler(this.btnClearHist_Click);
            // 
            // btnSQLHist
            // 
            this.btnSQLHist.FlatAppearance.BorderSize = 0;
            this.btnSQLHist.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSQLHist.Image = global::Krista.FM.Client.Common.SQLForm.Properties.Resources.file_temporary;
            this.btnSQLHist.Location = new System.Drawing.Point(9, 88);
            this.btnSQLHist.Name = "btnSQLHist";
            this.btnSQLHist.Size = new System.Drawing.Size(31, 32);
            this.btnSQLHist.TabIndex = 2;
            this.btnSQLHist.Tag = "History";
            this.btnSQLHist.UseVisualStyleBackColor = true;
            // 
            // btnNextSql
            // 
            this.btnNextSql.FlatAppearance.BorderSize = 0;
            this.btnNextSql.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNextSql.Image = global::Krista.FM.Client.Common.SQLForm.Properties.Resources.down_small;
            this.btnNextSql.Location = new System.Drawing.Point(9, 50);
            this.btnNextSql.Name = "btnNextSql";
            this.btnNextSql.Size = new System.Drawing.Size(31, 32);
            this.btnNextSql.TabIndex = 1;
            this.btnNextSql.Tag = "Next";
            this.btnNextSql.UseVisualStyleBackColor = true;
            this.btnNextSql.Click += new System.EventHandler(this.btnNextSql_Click);
            // 
            // btnPrevSql
            // 
            this.btnPrevSql.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnPrevSql.FlatAppearance.BorderSize = 0;
            this.btnPrevSql.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPrevSql.Image = global::Krista.FM.Client.Common.SQLForm.Properties.Resources.up_small;
            this.btnPrevSql.Location = new System.Drawing.Point(9, 12);
            this.btnPrevSql.Name = "btnPrevSql";
            this.btnPrevSql.Size = new System.Drawing.Size(31, 32);
            this.btnPrevSql.TabIndex = 0;
            this.btnPrevSql.Tag = "Previous";
            this.btnPrevSql.UseVisualStyleBackColor = true;
            this.btnPrevSql.Click += new System.EventHandler(this.btnPrevSql_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.arrangeSQLToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(147, 26);
            // 
            // arrangeSQLToolStripMenuItem
            // 
            this.arrangeSQLToolStripMenuItem.Name = "arrangeSQLToolStripMenuItem";
            this.arrangeSQLToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.arrangeSQLToolStripMenuItem.Text = "Arrange SQL";
            this.arrangeSQLToolStripMenuItem.Click += new System.EventHandler(this.arrangeSQLToolStripMenuItem_Click);
            // 
            // txtSqlArea
            // 
            this.txtSqlArea.AcceptsTab = true;
            this.txtSqlArea.ContextMenuStrip = this.contextMenuStrip1;
            this.txtSqlArea.Font = new System.Drawing.Font("Courier New", 10F);
            this.txtSqlArea.ForeColor = System.Drawing.Color.Black;
            this.txtSqlArea.Location = new System.Drawing.Point(0, 0);
            this.txtSqlArea.Name = "txtSqlArea";
            this.txtSqlArea.Size = new System.Drawing.Size(604, 300);
            this.txtSqlArea.TabIndex = 3;
            this.txtSqlArea.Text = "";
            // 
            // btOk
            // 
            this.btOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btOk.FlatAppearance.BorderSize = 0;
            this.btOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btOk.Image = global::Krista.FM.Client.Common.SQLForm.Properties.Resources.apply;
            this.btOk.Location = new System.Drawing.Point(9, 374);
            this.btOk.Name = "btOk";
            this.btOk.Size = new System.Drawing.Size(31, 32);
            this.btOk.TabIndex = 8;
            this.btOk.Tag = "Rollback";
            this.btOk.UseVisualStyleBackColor = true;
            this.btOk.Click += new System.EventHandler(this.btOk_Click);
            // 
            // frmSqlWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(660, 418);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.dgvResults);
            this.Controls.Add(this.txtSqlArea);
            this.Name = "frmSqlWindow";
            this.Text = "SQL Window";
            this.Load += new System.EventHandler(this.frmSqlWindow_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmSqlWindow_FormClosing);
            this.Resize += new System.EventHandler(this.frmSqlWindow_Resize);
            this.LocationChanged += new System.EventHandler(this.frmSqlWindow_LocationChanged);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmSqlWindow_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.dgvResults)).EndInit();
            this.panel1.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvResults;
        private RichDDLBox txtSqlArea;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnNextSql;
        private System.Windows.Forms.Button btnPrevSql;
        private System.Windows.Forms.Button btnSQLHist;
        private System.Windows.Forms.Button btnRollback;
        private System.Windows.Forms.Button btnCommit;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Button btnClearHist;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnExplain;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem arrangeSQLToolStripMenuItem;
        private System.Windows.Forms.Button btOk;

    }
}