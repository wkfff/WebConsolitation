using System;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;


namespace Krista.FM.Client.Common.Forms
{
    public partial class ReportForm : Form
    {
        public ReportForm()
        {
            InitializeComponent();
            InfragisticComponentsCustomize.CustomizeUltraGridParams(DataGrid);
        }

        /// <summary>
        /// заголовок формы
        /// </summary>
        public string FormCaption
        {
            get { return Text; }
            set { Text = value; }
        }

        public UltraGrid DataGrid
        {
            get { return ugReportData; }
        }

        private string _reportFileName;
        /// <summary>
        /// имя файла отчета
        /// </summary>
        public String ReportFileName
        {
            get { return _reportFileName; }
            set { _reportFileName = value; }
        }

        private void ubtnReportSave_Click(object sender, EventArgs e)
        {
            DataSet ds = (DataSet)DataGrid.DataSource;
            if (ds != null)
            {
                string fullName = _reportFileName + "__" + DateTime.Now;
                if (!ExportImportHelper.GetFileName(fullName, ExportImportHelper.fileExtensions.txt, 
                    true, ref fullName))
                    return;

                StreamWriter sw = new StreamWriter(fullName, false, Encoding.Default);                
                try
                {
                    DataRowCollection taskRows = ds.Tables[0].Rows;
                    foreach (DataRow taskRow in taskRows)
                    {
                        sw.WriteLine(taskRow[1]);
                        sw.WriteLine();
                        DataRow[] rows = ds.Tables[1].Select(string.Format("RefTaskID = {0}", taskRow["ID"]));
                        foreach (DataRow row in rows)
                        {
                            sw.WriteLine(row[0]);
                        }
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                    }                    
                }
                finally
                {
                    sw.Close();
                }

            }
        }

        private void splitContainer_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}