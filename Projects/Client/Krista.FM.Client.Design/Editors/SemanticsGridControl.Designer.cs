namespace Krista.FM.Client.Design.Editors
{
    partial class SemanticsGridControl
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
            this.ultraGridEx1 = new Krista.FM.Client.Components.UltraGridEx();
            this.dataSet1 = new System.Data.DataSet();
            this.dataTable1 = new System.Data.DataTable();
            this.dataColumn1 = new System.Data.DataColumn();
            this.dataColumn2 = new System.Data.DataColumn();
            this.dataColumn3 = new System.Data.DataColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable1)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraGridEx1
            // 
            this.ultraGridEx1.AllowAddNewRecords = true;
            this.ultraGridEx1.ColumnsToolbarVisible = false;
            this.ultraGridEx1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraGridEx1.ExportImportToolbarVisible = false;
            this.ultraGridEx1.Location = new System.Drawing.Point(0, 0);
            this.ultraGridEx1.MaxCalendarDate = new System.DateTime(((long)(0)));
            this.ultraGridEx1.MinCalendarDate = new System.DateTime(((long)(0)));
            this.ultraGridEx1.Name = "ultraGridEx1";
            //this.ultraGridEx1.ParentColumnName = "";
            //this.ultraGridEx1.RefColumnName = "";
            this.ultraGridEx1.SaveLoadFileName = "";
            this.ultraGridEx1.SingleBandLevelName = "Добавить запись";
            this.ultraGridEx1.Size = new System.Drawing.Size(372, 355);
            this.ultraGridEx1.StateRowEnable = false;
            this.ultraGridEx1.TabIndex = 1;
            // 
            // dataSet1
            // 
            this.dataSet1.DataSetName = "NewDataSet";
            this.dataSet1.Tables.AddRange(new System.Data.DataTable[] {
            this.dataTable1});
            // 
            // dataTable1
            // 
            this.dataTable1.Columns.AddRange(new System.Data.DataColumn[] {
            this.dataColumn1,
            this.dataColumn2,
            this.dataColumn3});
            this.dataTable1.Constraints.AddRange(new System.Data.Constraint[] {
            new System.Data.UniqueConstraint("Constraint1", new string[] {
                        "ID"}, false)});
            this.dataTable1.TableName = "Table1";
            // 
            // dataColumn1
            // 
            this.dataColumn1.Caption = "Ключ";
            this.dataColumn1.ColumnName = "Key";
            // 
            // dataColumn2
            // 
            this.dataColumn2.Caption = "Значение";
            this.dataColumn2.ColumnName = "Value";
            // 
            // dataColumn3
            // 
            this.dataColumn3.AutoIncrement = true;
            this.dataColumn3.AutoIncrementSeed = ((long)(1));
            this.dataColumn3.Caption = "ID";
            this.dataColumn3.ColumnMapping = System.Data.MappingType.Hidden;
            this.dataColumn3.ColumnName = "ID";
            this.dataColumn3.DataType = typeof(int);
            // 
            // SemanticsGridControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ultraGridEx1);
            this.Name = "SemanticsGridControl";
            this.Size = new System.Drawing.Size(372, 355);
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Krista.FM.Client.Components.UltraGridEx ultraGridEx1;
        private System.Data.DataSet dataSet1;
        private System.Data.DataTable dataTable1;
        private System.Data.DataColumn dataColumn1;
        private System.Data.DataColumn dataColumn2;
        private System.Data.DataColumn dataColumn3;
    }
}
