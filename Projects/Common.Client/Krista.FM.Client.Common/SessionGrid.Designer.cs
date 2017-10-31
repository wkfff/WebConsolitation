namespace Krista.FM.Client.Common
{
    partial class SessionGrid
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SessionGrid));
            this.ultraGridEx1 = new Krista.FM.Client.Components.UltraGridEx();
            this.dsSessions = new System.Data.DataSet();
            this.dtSessions = new System.Data.DataTable();
            this.dataColumn1 = new System.Data.DataColumn();
            this.dataColumn2 = new System.Data.DataColumn();
            this.dataColumn3 = new System.Data.DataColumn();
            this.dataColumn4 = new System.Data.DataColumn();
            this.dataColumn5 = new System.Data.DataColumn();
            this.dataColumn6 = new System.Data.DataColumn();
            this.dataColumn7 = new System.Data.DataColumn();
            this.dataColumn8 = new System.Data.DataColumn();
            this.il = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dsSessions)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtSessions)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraGridEx1
            // 
            this.ultraGridEx1.AllowAddNewRecords = true;
            this.ultraGridEx1.AllowClearTable = true;
            this.ultraGridEx1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraGridEx1.ExportImportToolbarVisible = false;
            this.ultraGridEx1.InDebugMode = false;
            this.ultraGridEx1.Location = new System.Drawing.Point(0, 0);
            this.ultraGridEx1.MaxCalendarDate = new System.DateTime(((long)(0)));
            this.ultraGridEx1.MinCalendarDate = new System.DateTime(((long)(0)));
            this.ultraGridEx1.Name = "ultraGridEx1";
            this.ultraGridEx1.SaveLoadFileName = "";
            this.ultraGridEx1.ServerFilterEnabled = false;
            this.ultraGridEx1.SingleBandLevelName = "Добавить запись...";
            this.ultraGridEx1.Size = new System.Drawing.Size(556, 474);
            this.ultraGridEx1.sortColumnName = "";
            this.ultraGridEx1.StateRowEnable = false;
            this.ultraGridEx1.TabIndex = 0;
            // 
            // dsSessions
            // 
            this.dsSessions.DataSetName = "NewDataSet";
            this.dsSessions.Tables.AddRange(new System.Data.DataTable[] {
            this.dtSessions});
            // 
            // dtSessions
            // 
            this.dtSessions.Columns.AddRange(new System.Data.DataColumn[] {
            this.dataColumn1,
            this.dataColumn2,
            this.dataColumn3,
            this.dataColumn4,
            this.dataColumn5,
            this.dataColumn6,
            this.dataColumn7,
            this.dataColumn8});
            this.dtSessions.TableName = "Table1";
            // 
            // dataColumn1
            // 
            this.dataColumn1.ColumnName = "ID";
            // 
            // dataColumn2
            // 
            this.dataColumn2.Caption = "Логин";
            this.dataColumn2.ColumnName = "Login";
            // 
            // dataColumn3
            // 
            this.dataColumn3.Caption = "Время подключения";
            this.dataColumn3.ColumnName = "ConnectionTime";
            // 
            // dataColumn4
            // 
            this.dataColumn4.Caption = "Машина";
            this.dataColumn4.ColumnName = "Host";
            // 
            // dataColumn5
            // 
            this.dataColumn5.Caption = "Приложение";
            this.dataColumn5.ColumnName = "Application";
            // 
            // dataColumn6
            // 
            this.dataColumn6.Caption = "Количество выделеных ресурсов";
            this.dataColumn6.ColumnName = "SourcesCount";
            // 
            // dataColumn7
            // 
            this.dataColumn7.Caption = "Тип сессии";
            this.dataColumn7.ColumnName = "SessionType";
            // 
            // dataColumn8
            // 
            this.dataColumn8.ColumnName = "isBlocked";
            this.dataColumn8.DataType = typeof(bool);
            // 
            // il
            // 
            this.il.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("il.ImageStream")));
            this.il.TransparentColor = System.Drawing.Color.Magenta;
            this.il.Images.SetKeyName(0, "RedCross.bmp");
            this.il.Images.SetKeyName(1, "Check.bmp");
            // 
            // SessionGrid
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ultraGridEx1);
            this.Name = "SessionGrid";
            this.Size = new System.Drawing.Size(556, 474);
            ((System.ComponentModel.ISupportInitialize)(this.dsSessions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtSessions)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Krista.FM.Client.Components.UltraGridEx ultraGridEx1;
        private System.Data.DataSet dsSessions;
        private System.Data.DataTable dtSessions;
        private System.Data.DataColumn dataColumn1;
        private System.Data.DataColumn dataColumn2;
        private System.Data.DataColumn dataColumn3;
        private System.Data.DataColumn dataColumn4;
        private System.Data.DataColumn dataColumn5;
        private System.Data.DataColumn dataColumn6;
        private System.Data.DataColumn dataColumn7;
        private System.Windows.Forms.ImageList il;
        private System.Data.DataColumn dataColumn8;
    }
}
