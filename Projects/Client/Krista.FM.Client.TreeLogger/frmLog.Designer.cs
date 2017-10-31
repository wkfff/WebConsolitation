using Infragistics.Win.Misc;

namespace Krista.FM.Client.TreeLogger
{
	partial class frmLog
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
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLog));
			this.pnlProgress = new System.Windows.Forms.Panel();
			this.splitterMain = new System.Windows.Forms.SplitContainer();
			this.gridOperations = new System.Windows.Forms.DataGridView();
			this.Caption = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Status = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.pnlIndicator = new System.Windows.Forms.Panel();
            this.lbCurrentOperation = new UltraLabel();
            this.lbCaption = new UltraLabel();
			this.progressBarMain = new System.Windows.Forms.ProgressBar();
			this.pnlLog = new System.Windows.Forms.Panel();
			this.treeViewLog = new System.Windows.Forms.TreeView();
			this.imageListSmall = new System.Windows.Forms.ImageList(this.components);
			this.pnlLogButtons = new System.Windows.Forms.Panel();
			this.btnOpenLog = new System.Windows.Forms.Button();
			this.btnCloseLogger = new System.Windows.Forms.Button();
			this.btnSaveLog = new System.Windows.Forms.Button();
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.pnlProgress.SuspendLayout();
			this.splitterMain.Panel1.SuspendLayout();
			this.splitterMain.Panel2.SuspendLayout();
			this.splitterMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gridOperations)).BeginInit();
			this.pnlIndicator.SuspendLayout();
			this.pnlLog.SuspendLayout();
			this.pnlLogButtons.SuspendLayout();
			this.SuspendLayout();
			// 
			// pnlProgress
			// 
			this.pnlProgress.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pnlProgress.Controls.Add(this.splitterMain);
			this.pnlProgress.Controls.Add(this.pnlLogButtons);
			this.pnlProgress.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlProgress.Location = new System.Drawing.Point(0, 0);
			this.pnlProgress.Margin = new System.Windows.Forms.Padding(2);
			this.pnlProgress.Name = "pnlProgress";
			this.pnlProgress.Size = new System.Drawing.Size(632, 371);
			this.pnlProgress.TabIndex = 1;
			// 
			// splitterMain
			// 
			this.splitterMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.splitterMain.Location = new System.Drawing.Point(1, 2);
			this.splitterMain.Margin = new System.Windows.Forms.Padding(2);
			this.splitterMain.MinimumSize = new System.Drawing.Size(0, 81);
			this.splitterMain.Name = "splitterMain";
			this.splitterMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitterMain.Panel1
			// 
			this.splitterMain.Panel1.Controls.Add(this.gridOperations);
			this.splitterMain.Panel1MinSize = 100;
			// 
			// splitterMain.Panel2
			// 
			this.splitterMain.Panel2.Controls.Add(this.pnlIndicator);
			this.splitterMain.Panel2.Controls.Add(this.pnlLog);
			this.splitterMain.Panel2MinSize = 100;
			this.splitterMain.Size = new System.Drawing.Size(627, 331);
			this.splitterMain.SplitterDistance = 130;
			this.splitterMain.SplitterWidth = 3;
			this.splitterMain.TabIndex = 10;
			// 
			// gridOperations
			// 
			this.gridOperations.AllowUserToAddRows = false;
			this.gridOperations.AllowUserToDeleteRows = false;
			this.gridOperations.AllowUserToOrderColumns = true;
			this.gridOperations.AllowUserToResizeRows = false;
			this.gridOperations.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.gridOperations.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.gridOperations.BackgroundColor = System.Drawing.Color.White;
			this.gridOperations.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.gridOperations.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.gridOperations.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.gridOperations.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.gridOperations.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Caption,
            this.Status});
			this.gridOperations.Location = new System.Drawing.Point(0, 0);
			this.gridOperations.Margin = new System.Windows.Forms.Padding(2);
			this.gridOperations.MultiSelect = false;
			this.gridOperations.Name = "gridOperations";
			this.gridOperations.ReadOnly = true;
			this.gridOperations.RowHeadersVisible = false;
			this.gridOperations.RowTemplate.Height = 24;
			this.gridOperations.RowTemplate.ReadOnly = true;
			this.gridOperations.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.gridOperations.ShowEditingIcon = false;
			this.gridOperations.Size = new System.Drawing.Size(626, 128);
			this.gridOperations.TabIndex = 1;
			this.gridOperations.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridOperations_CellDoubleClick);
			// 
			// Caption
			// 
			this.Caption.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.Caption.HeaderText = "Наименование операции";
			this.Caption.MinimumWidth = 200;
			this.Caption.Name = "Caption";
			this.Caption.ReadOnly = true;
			this.Caption.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// Status
			// 
			this.Status.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.Status.HeaderText = "Статус";
			this.Status.MinimumWidth = 100;
			this.Status.Name = "Status";
			this.Status.ReadOnly = true;
			this.Status.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.Status.Width = 300;
			// 
			// pnlIndicator
			// 
			this.pnlIndicator.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.pnlIndicator.Controls.Add(this.lbCurrentOperation);
			this.pnlIndicator.Controls.Add(this.lbCaption);
			this.pnlIndicator.Controls.Add(this.progressBarMain);
			this.pnlIndicator.Location = new System.Drawing.Point(0, 2);
			this.pnlIndicator.Margin = new System.Windows.Forms.Padding(2);
			this.pnlIndicator.Name = "pnlIndicator";
			this.pnlIndicator.Size = new System.Drawing.Size(627, 38);
			this.pnlIndicator.TabIndex = 7;
			// 
			// lbCurrentOperation
			// 
			this.lbCurrentOperation.AutoSize = true;
			this.lbCurrentOperation.Location = new System.Drawing.Point(2, 1);
			this.lbCurrentOperation.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.lbCurrentOperation.Name = "lbCurrentOperation";
			this.lbCurrentOperation.Size = new System.Drawing.Size(106, 13);
			this.lbCurrentOperation.TabIndex = 4;
			this.lbCurrentOperation.Text = "Текущая операция:";
			// 
			// lbCaption
			// 
			this.lbCaption.AutoSize = true;
			this.lbCaption.Location = new System.Drawing.Point(118, 1);
			this.lbCaption.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.lbCaption.Name = "lbCaption";
			this.lbCaption.Size = new System.Drawing.Size(109, 13);
			this.lbCaption.TabIndex = 3;
			this.lbCaption.Text = "операция не задана";
			// 
			// progressBarMain
			// 
			this.progressBarMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.progressBarMain.Enabled = false;
			this.progressBarMain.Location = new System.Drawing.Point(2, 17);
			this.progressBarMain.Margin = new System.Windows.Forms.Padding(2);
			this.progressBarMain.Name = "progressBarMain";
			this.progressBarMain.Size = new System.Drawing.Size(621, 16);
			this.progressBarMain.Step = 1;
			this.progressBarMain.TabIndex = 2;
			// 
			// pnlLog
			// 
			this.pnlLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.pnlLog.BackColor = System.Drawing.SystemColors.Control;
			this.pnlLog.Controls.Add(this.treeViewLog);
			this.pnlLog.Location = new System.Drawing.Point(2, 38);
			this.pnlLog.Margin = new System.Windows.Forms.Padding(2);
			this.pnlLog.Name = "pnlLog";
			this.pnlLog.Size = new System.Drawing.Size(625, 159);
			this.pnlLog.TabIndex = 8;
			// 
			// treeViewLog
			// 
			this.treeViewLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.treeViewLog.Font = new System.Drawing.Font("Arial", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.treeViewLog.HideSelection = false;
			this.treeViewLog.ImageIndex = 4;
			this.treeViewLog.ImageList = this.imageListSmall;
			this.treeViewLog.Location = new System.Drawing.Point(0, 2);
			this.treeViewLog.Name = "treeViewLog";
			this.treeViewLog.SelectedImageIndex = 4;
			this.treeViewLog.Size = new System.Drawing.Size(626, 158);
			this.treeViewLog.TabIndex = 2;
			// 
			// imageListSmall
			// 
			this.imageListSmall.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListSmall.ImageStream")));
			this.imageListSmall.TransparentColor = System.Drawing.Color.Transparent;
			this.imageListSmall.Images.SetKeyName(0, "Warning_16");
			this.imageListSmall.Images.SetKeyName(1, "Error_16");
			this.imageListSmall.Images.SetKeyName(2, "Info_16");
			this.imageListSmall.Images.SetKeyName(3, "OK_16");
			this.imageListSmall.Images.SetKeyName(4, "Undefined_16");
			this.imageListSmall.Images.SetKeyName(5, "Time_16");
			this.imageListSmall.Images.SetKeyName(6, "Running_16");
			// 
			// pnlLogButtons
			// 
			this.pnlLogButtons.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.pnlLogButtons.Controls.Add(this.btnOpenLog);
			this.pnlLogButtons.Controls.Add(this.btnCloseLogger);
			this.pnlLogButtons.Controls.Add(this.btnSaveLog);
			this.pnlLogButtons.Location = new System.Drawing.Point(0, 336);
			this.pnlLogButtons.Margin = new System.Windows.Forms.Padding(2);
			this.pnlLogButtons.Name = "pnlLogButtons";
			this.pnlLogButtons.Size = new System.Drawing.Size(630, 34);
			this.pnlLogButtons.TabIndex = 9;
			// 
			// btnOpenLog
			// 
			this.btnOpenLog.Location = new System.Drawing.Point(292, 2);
			this.btnOpenLog.Margin = new System.Windows.Forms.Padding(2);
			this.btnOpenLog.Name = "btnOpenLog";
			this.btnOpenLog.Size = new System.Drawing.Size(109, 27);
			this.btnOpenLog.TabIndex = 2;
			this.btnOpenLog.Text = "Открыть лог";
			this.btnOpenLog.UseVisualStyleBackColor = true;
			this.btnOpenLog.Click += new System.EventHandler(this.btnOpenLog_Click);
			// 
			// btnCloseLogger
			// 
			this.btnCloseLogger.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCloseLogger.Location = new System.Drawing.Point(518, 2);
			this.btnCloseLogger.Margin = new System.Windows.Forms.Padding(2);
			this.btnCloseLogger.Name = "btnCloseLogger";
			this.btnCloseLogger.Size = new System.Drawing.Size(109, 27);
			this.btnCloseLogger.TabIndex = 1;
			this.btnCloseLogger.Text = "Закрыть";
			this.btnCloseLogger.UseVisualStyleBackColor = true;
			this.btnCloseLogger.Click += new System.EventHandler(this.btnCloseLogger_Click);
			// 
			// btnSaveLog
			// 
			this.btnSaveLog.Location = new System.Drawing.Point(406, 2);
			this.btnSaveLog.Margin = new System.Windows.Forms.Padding(2);
			this.btnSaveLog.Name = "btnSaveLog";
			this.btnSaveLog.Size = new System.Drawing.Size(109, 27);
			this.btnSaveLog.TabIndex = 0;
			this.btnSaveLog.Text = "Сохранить лог";
			this.btnSaveLog.UseVisualStyleBackColor = true;
			this.btnSaveLog.Click += new System.EventHandler(this.btnSaveLog_Click);
			// 
			// saveFileDialog
			// 
			this.saveFileDialog.DefaultExt = "xmllog";
			this.saveFileDialog.Filter = "Лог-файлы (*.xmllog)|*.xmllog|Все файлы (*.*)|*.*";
			this.saveFileDialog.SupportMultiDottedExtensions = true;
			this.saveFileDialog.Title = "Сохранить лог в файл";
			// 
			// openFileDialog
			// 
			this.openFileDialog.FileName = "*.xmllog";
			this.openFileDialog.Filter = "Лог-файлы (*.xmllog)|*.xmllog|Все файлы (*.*)|*.*";
			this.openFileDialog.SupportMultiDottedExtensions = true;
			this.openFileDialog.Title = "Восстановить лог из файла";
			// 
			// frmLog
			// 
			this.AcceptButton = this.btnOpenLog;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(632, 371);
			this.Controls.Add(this.pnlProgress);
			this.Cursor = System.Windows.Forms.Cursors.Default;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(640, 400);
			this.Name = "frmLog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmLog_FormClosing);
			this.pnlProgress.ResumeLayout(false);
			this.splitterMain.Panel1.ResumeLayout(false);
			this.splitterMain.Panel2.ResumeLayout(false);
			this.splitterMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.gridOperations)).EndInit();
			this.pnlIndicator.ResumeLayout(false);
			this.pnlIndicator.PerformLayout();
			this.pnlLog.ResumeLayout(false);
			this.pnlLogButtons.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel pnlProgress;
		private System.Windows.Forms.Panel pnlLog;
		private System.Windows.Forms.Panel pnlIndicator;
        private UltraLabel lbCurrentOperation;
        public UltraLabel lbCaption;
		internal System.Windows.Forms.ProgressBar progressBarMain;
		private System.Windows.Forms.Panel pnlLogButtons;
		private System.Windows.Forms.Button btnOpenLog;
		private System.Windows.Forms.Button btnCloseLogger;
		private System.Windows.Forms.Button btnSaveLog;
		internal System.Windows.Forms.TreeView treeViewLog;
		internal System.Windows.Forms.ImageList imageListSmall;
		internal System.Windows.Forms.SaveFileDialog saveFileDialog;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.SplitContainer splitterMain;
		public System.Windows.Forms.DataGridView gridOperations;
		private System.Windows.Forms.DataGridViewTextBoxColumn Caption;
		private System.Windows.Forms.DataGridViewTextBoxColumn Status;

	}
}