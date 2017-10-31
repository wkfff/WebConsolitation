namespace PackageCleaner2000
{
	partial class frmMain
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
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.groupBoxPackageMethod = new System.Windows.Forms.GroupBox();
            this.radioButtonPackageReplace = new System.Windows.Forms.RadioButton();
            this.radioButtonPackageNew = new System.Windows.Forms.RadioButton();
            this.btnAcceptPackage = new System.Windows.Forms.Button();
            this.groupBoxDimensionsMethod = new System.Windows.Forms.GroupBox();
            this.radioButtonDimensionReplace = new System.Windows.Forms.RadioButton();
            this.radioButtonDimensionAdd = new System.Windows.Forms.RadioButton();
            this.groupBoxCubesMethod = new System.Windows.Forms.GroupBox();
            this.radioButtonCubeReplace = new System.Windows.Forms.RadioButton();
            this.radioButtonCubeAdd = new System.Windows.Forms.RadioButton();
            this.btnAcceptDimensions = new System.Windows.Forms.Button();
            this.btnAcceptCubes = new System.Windows.Forms.Button();
            this.btnOpenFMMD = new System.Windows.Forms.Button();
            this.openFileDlgMain = new System.Windows.Forms.OpenFileDialog();
            this.TwoLists = new Krista.FM.Client.OLAPAdmin.ctrlTwoLists();
            this.pnlButtons.SuspendLayout();
            this.groupBoxPackageMethod.SuspendLayout();
            this.groupBoxDimensionsMethod.SuspendLayout();
            this.groupBoxCubesMethod.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlButtons
            // 
            this.pnlButtons.Controls.Add(this.groupBoxPackageMethod);
            this.pnlButtons.Controls.Add(this.btnAcceptPackage);
            this.pnlButtons.Controls.Add(this.groupBoxDimensionsMethod);
            this.pnlButtons.Controls.Add(this.groupBoxCubesMethod);
            this.pnlButtons.Controls.Add(this.btnAcceptDimensions);
            this.pnlButtons.Controls.Add(this.btnAcceptCubes);
            this.pnlButtons.Controls.Add(this.btnOpenFMMD);
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlButtons.Location = new System.Drawing.Point(0, 462);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Size = new System.Drawing.Size(898, 84);
            this.pnlButtons.TabIndex = 1;
            // 
            // groupBoxPackageMethod
            // 
            this.groupBoxPackageMethod.Controls.Add(this.radioButtonPackageReplace);
            this.groupBoxPackageMethod.Controls.Add(this.radioButtonPackageNew);
            this.groupBoxPackageMethod.Location = new System.Drawing.Point(681, 34);
            this.groupBoxPackageMethod.Name = "groupBoxPackageMethod";
            this.groupBoxPackageMethod.Size = new System.Drawing.Size(212, 38);
            this.groupBoxPackageMethod.TabIndex = 5;
            this.groupBoxPackageMethod.TabStop = false;
            this.groupBoxPackageMethod.Text = "Метод обновления";
            // 
            // radioButtonPackageReplace
            // 
            this.radioButtonPackageReplace.AutoSize = true;
            this.radioButtonPackageReplace.Location = new System.Drawing.Point(118, 15);
            this.radioButtonPackageReplace.Name = "radioButtonPackageReplace";
            this.radioButtonPackageReplace.Size = new System.Drawing.Size(87, 17);
            this.radioButtonPackageReplace.TabIndex = 3;
            this.radioButtonPackageReplace.Text = "Обновление";
            this.radioButtonPackageReplace.UseVisualStyleBackColor = true;
            // 
            // radioButtonPackageNew
            // 
            this.radioButtonPackageNew.AutoSize = true;
            this.radioButtonPackageNew.Checked = true;
            this.radioButtonPackageNew.Location = new System.Drawing.Point(16, 16);
            this.radioButtonPackageNew.Name = "radioButtonPackageNew";
            this.radioButtonPackageNew.Size = new System.Drawing.Size(84, 17);
            this.radioButtonPackageNew.TabIndex = 2;
            this.radioButtonPackageNew.TabStop = true;
            this.radioButtonPackageNew.Text = "Новая база";
            this.radioButtonPackageNew.UseVisualStyleBackColor = true;
            // 
            // btnAcceptPackage
            // 
            this.btnAcceptPackage.Enabled = false;
            this.btnAcceptPackage.Location = new System.Drawing.Point(681, 5);
            this.btnAcceptPackage.Name = "btnAcceptPackage";
            this.btnAcceptPackage.Size = new System.Drawing.Size(212, 23);
            this.btnAcceptPackage.TabIndex = 5;
            this.btnAcceptPackage.Text = "4) Принять конфигурацию пакета";
            this.btnAcceptPackage.UseVisualStyleBackColor = true;
            this.btnAcceptPackage.Click += new System.EventHandler(this.btnAcceptPackage_Click);
            // 
            // groupBoxDimensionsMethod
            // 
            this.groupBoxDimensionsMethod.Controls.Add(this.radioButtonDimensionReplace);
            this.groupBoxDimensionsMethod.Controls.Add(this.radioButtonDimensionAdd);
            this.groupBoxDimensionsMethod.Location = new System.Drawing.Point(455, 34);
            this.groupBoxDimensionsMethod.Name = "groupBoxDimensionsMethod";
            this.groupBoxDimensionsMethod.Size = new System.Drawing.Size(212, 38);
            this.groupBoxDimensionsMethod.TabIndex = 4;
            this.groupBoxDimensionsMethod.TabStop = false;
            this.groupBoxDimensionsMethod.Text = "Метод обновления";
            // 
            // radioButtonDimensionReplace
            // 
            this.radioButtonDimensionReplace.AutoSize = true;
            this.radioButtonDimensionReplace.Location = new System.Drawing.Point(139, 15);
            this.radioButtonDimensionReplace.Name = "radioButtonDimensionReplace";
            this.radioButtonDimensionReplace.Size = new System.Drawing.Size(65, 17);
            this.radioButtonDimensionReplace.TabIndex = 3;
            this.radioButtonDimensionReplace.Text = "Replace";
            this.radioButtonDimensionReplace.UseVisualStyleBackColor = true;
            // 
            // radioButtonDimensionAdd
            // 
            this.radioButtonDimensionAdd.AutoSize = true;
            this.radioButtonDimensionAdd.Checked = true;
            this.radioButtonDimensionAdd.Location = new System.Drawing.Point(16, 16);
            this.radioButtonDimensionAdd.Name = "radioButtonDimensionAdd";
            this.radioButtonDimensionAdd.Size = new System.Drawing.Size(44, 17);
            this.radioButtonDimensionAdd.TabIndex = 2;
            this.radioButtonDimensionAdd.TabStop = true;
            this.radioButtonDimensionAdd.Text = "Add";
            this.radioButtonDimensionAdd.UseVisualStyleBackColor = true;
            // 
            // groupBoxCubesMethod
            // 
            this.groupBoxCubesMethod.Controls.Add(this.radioButtonCubeReplace);
            this.groupBoxCubesMethod.Controls.Add(this.radioButtonCubeAdd);
            this.groupBoxCubesMethod.Location = new System.Drawing.Point(229, 34);
            this.groupBoxCubesMethod.Name = "groupBoxCubesMethod";
            this.groupBoxCubesMethod.Size = new System.Drawing.Size(212, 38);
            this.groupBoxCubesMethod.TabIndex = 3;
            this.groupBoxCubesMethod.TabStop = false;
            this.groupBoxCubesMethod.Text = "Метод обновления";
            // 
            // radioButtonCubeReplace
            // 
            this.radioButtonCubeReplace.AutoSize = true;
            this.radioButtonCubeReplace.Location = new System.Drawing.Point(141, 15);
            this.radioButtonCubeReplace.Name = "radioButtonCubeReplace";
            this.radioButtonCubeReplace.Size = new System.Drawing.Size(65, 17);
            this.radioButtonCubeReplace.TabIndex = 1;
            this.radioButtonCubeReplace.Text = "Replace";
            this.radioButtonCubeReplace.UseVisualStyleBackColor = true;
            // 
            // radioButtonCubeAdd
            // 
            this.radioButtonCubeAdd.AutoSize = true;
            this.radioButtonCubeAdd.Checked = true;
            this.radioButtonCubeAdd.Location = new System.Drawing.Point(18, 16);
            this.radioButtonCubeAdd.Name = "radioButtonCubeAdd";
            this.radioButtonCubeAdd.Size = new System.Drawing.Size(44, 17);
            this.radioButtonCubeAdd.TabIndex = 0;
            this.radioButtonCubeAdd.TabStop = true;
            this.radioButtonCubeAdd.Text = "Add";
            this.radioButtonCubeAdd.UseVisualStyleBackColor = true;
            // 
            // btnAcceptDimensions
            // 
            this.btnAcceptDimensions.Enabled = false;
            this.btnAcceptDimensions.Location = new System.Drawing.Point(455, 5);
            this.btnAcceptDimensions.Name = "btnAcceptDimensions";
            this.btnAcceptDimensions.Size = new System.Drawing.Size(212, 23);
            this.btnAcceptDimensions.TabIndex = 2;
            this.btnAcceptDimensions.Text = "3) Принять изменения в измерениях";
            this.btnAcceptDimensions.UseVisualStyleBackColor = true;
            this.btnAcceptDimensions.Click += new System.EventHandler(this.btnAcceptDimensions_Click);
            // 
            // btnAcceptCubes
            // 
            this.btnAcceptCubes.Enabled = false;
            this.btnAcceptCubes.Location = new System.Drawing.Point(229, 5);
            this.btnAcceptCubes.Name = "btnAcceptCubes";
            this.btnAcceptCubes.Size = new System.Drawing.Size(212, 23);
            this.btnAcceptCubes.TabIndex = 1;
            this.btnAcceptCubes.Text = "2) Принять изменения в кубах";
            this.btnAcceptCubes.UseVisualStyleBackColor = true;
            this.btnAcceptCubes.Click += new System.EventHandler(this.btnAcceptCubes_Click);
            // 
            // btnOpenFMMD
            // 
            this.btnOpenFMMD.Location = new System.Drawing.Point(3, 5);
            this.btnOpenFMMD.Name = "btnOpenFMMD";
            this.btnOpenFMMD.Size = new System.Drawing.Size(212, 23);
            this.btnOpenFMMD.TabIndex = 0;
            this.btnOpenFMMD.Text = "1) Выбрать файл \"FMMD_All.xml\"";
            this.btnOpenFMMD.UseVisualStyleBackColor = true;
            this.btnOpenFMMD.Click += new System.EventHandler(this.btnOpenFMMD_Click);
            // 
            // openFileDlgMain
            // 
            this.openFileDlgMain.DefaultExt = "xml";
            this.openFileDlgMain.FileName = "FMMD_All.xml";
            this.openFileDlgMain.Filter = "Файлы xml (*.xml)|*.xml|Все файлы|*.*";
            this.openFileDlgMain.InitialDirectory = "X:\\dotNet\\Repository";
            this.openFileDlgMain.Title = "Выберите файл \"FMMD_All.xml\"";
            // 
            // TwoLists
            // 
            this.TwoLists.AutoSize = true;
            this.TwoLists.BackColor = System.Drawing.SystemColors.Control;
            this.TwoLists.Dock = System.Windows.Forms.DockStyle.Top;
            this.TwoLists.Location = new System.Drawing.Point(0, 0);
            this.TwoLists.Margin = new System.Windows.Forms.Padding(2);
            this.TwoLists.Name = "TwoLists";
            this.TwoLists.Size = new System.Drawing.Size(898, 462);
            this.TwoLists.TabIndex = 0;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(898, 546);
            this.Controls.Add(this.pnlButtons);
            this.Controls.Add(this.TwoLists);
            this.Name = "frmMain";
            this.Text = "Утилита для очистки файла \"FMMD_All.xml\" от ненужных кубов";
            this.Shown += new System.EventHandler(this.frmMain_Shown);
            this.pnlButtons.ResumeLayout(false);
            this.groupBoxPackageMethod.ResumeLayout(false);
            this.groupBoxPackageMethod.PerformLayout();
            this.groupBoxDimensionsMethod.ResumeLayout(false);
            this.groupBoxDimensionsMethod.PerformLayout();
            this.groupBoxCubesMethod.ResumeLayout(false);
            this.groupBoxCubesMethod.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private Krista.FM.Client.OLAPAdmin.ctrlTwoLists TwoLists;
		private System.Windows.Forms.Panel pnlButtons;
		private System.Windows.Forms.Button btnOpenFMMD;
		private System.Windows.Forms.Button btnAcceptCubes;
		private System.Windows.Forms.Button btnAcceptDimensions;
		private System.Windows.Forms.OpenFileDialog openFileDlgMain;
        private System.Windows.Forms.GroupBox groupBoxCubesMethod;
        private System.Windows.Forms.RadioButton radioButtonCubeAdd;
        private System.Windows.Forms.RadioButton radioButtonCubeReplace;
        private System.Windows.Forms.GroupBox groupBoxDimensionsMethod;
        private System.Windows.Forms.RadioButton radioButtonDimensionReplace;
        private System.Windows.Forms.RadioButton radioButtonDimensionAdd;
        private System.Windows.Forms.Button btnAcceptPackage;
        private System.Windows.Forms.GroupBox groupBoxPackageMethod;
        private System.Windows.Forms.RadioButton radioButtonPackageReplace;
        private System.Windows.Forms.RadioButton radioButtonPackageNew;
	}
}

