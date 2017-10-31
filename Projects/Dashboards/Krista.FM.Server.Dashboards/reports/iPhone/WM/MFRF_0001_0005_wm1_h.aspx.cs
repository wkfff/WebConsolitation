using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class MFRF_0001_0005_wm1_H : CustomReportPage
    {
        private DataTable dt;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("MFRF_0001_0005_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            UserParams.PeriodLastYear.Value = "2009";
            UserParams.PeriodYear.Value = "2010";

            Label4.Text = "данные на 2010 год";
            Label5.Text =
                string.Format("обновлено {0:dd.MM.yyyy HH:mm:ss}", DateTime.Now);

            FillData();
            SetDataTable(dt);
        }

        protected void FillData()
        {
            dt = new DataTable();
            string query = DataProvider.GetQueryText("MFRF_0001_0005_h");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект", dt);

            string lastYear = (DateTime.Now.Year - 1).ToString();
            LabelTitle.Text = string.Format("Межбюджетные трансферты {0} из Федеральных фондов на {1} год, данные в млн.руб., темп роста к {2} году",
                dt.Rows[0][2], UserParams.PeriodYear.Value, lastYear);
            
            foreach (DataRow row in dt.Rows)
            {
                for (int i = 3; i <= 9; i++)
                {
                    if (row[i] != DBNull.Value)
                    {
                        if (i == 4 || i == 6 || i == 8)
                        {
                            row[i] = Convert.ToDouble(row[i]) * 100;
                        }
                        else
                        {
                            row[i] = Convert.ToDouble(row[i]) / 1000;
                        }
                    }
                }
            }
        }

        private void SetDataTable(DataTable dtGrid)
        {
            TableRow rowHeader1 = new TableRow();

            int fontSize = CRHelper.fontMF0005H;
            Color fontColor = CRHelper.fontLightColor;
            Color captionColor = CRHelper.fontTableCaptionColor;
            Color mainColumnColor = CRHelper.fontTableDataColor;

            CRHelper.AddCaptionCell(rowHeader1, "Субъект", fontSize, captionColor);
            CRHelper.AddCaptionCell(rowHeader1, "ФФФПР", fontSize, captionColor);
            CRHelper.AddCaptionCell(rowHeader1, "ФФК", fontSize, captionColor);
            CRHelper.AddCaptionCell(rowHeader1, "ФФСР", fontSize, captionColor);
            CRHelper.AddCaptionCell(rowHeader1, "Иные", fontSize, captionColor);

            rowHeader1.Cells[0].RowSpan = 2;
            rowHeader1.Cells[1].ColumnSpan = 2;
            rowHeader1.Cells[2].ColumnSpan = 2;
            rowHeader1.Cells[3].ColumnSpan = 2;            
            
            gridTable1.Rows.Add(rowHeader1);

            rowHeader1 = new TableRow();
            CRHelper.AddCaptionCell(rowHeader1, "сумма", fontSize, captionColor);
            CRHelper.AddCaptionCell(rowHeader1, "темп роста %", fontSize, captionColor);
            CRHelper.AddCaptionCell(rowHeader1, "сумма", fontSize, captionColor);
            CRHelper.AddCaptionCell(rowHeader1, "темп роста %", fontSize, captionColor);
            CRHelper.AddCaptionCell(rowHeader1, "сумма", fontSize, captionColor);
            CRHelper.AddCaptionCell(rowHeader1, "темп роста %", fontSize, captionColor);
            CRHelper.AddCaptionCell(rowHeader1, "сумма", fontSize, captionColor);

            gridTable1.Rows.Add(rowHeader1);

            for (int i = 0; i < dtGrid.Rows.Count; i++)
            {
                TableRow row1 = new TableRow();
                CRHelper.AddDataCellL(row1, string.Format("{0}", dtGrid.Rows[i][1]), fontSize, fontColor);
                CRHelper.AddDataCellR(row1, string.Format("{0:N0}", dtGrid.Rows[i][3]), fontSize, mainColumnColor);
                CRHelper.AddDataCellR(row1, string.Format("{0:N1}", dtGrid.Rows[i][4]), fontSize, fontColor);
                CRHelper.AddDataCellR(row1, string.Format("{0:N0}", dtGrid.Rows[i][5]), fontSize, mainColumnColor);
                CRHelper.AddDataCellR(row1, string.Format("{0:N1}", dtGrid.Rows[i][6]), fontSize, fontColor);
                CRHelper.AddDataCellR(row1, string.Format("{0:N0}", dtGrid.Rows[i][7]), fontSize, mainColumnColor);
                CRHelper.AddDataCellR(row1, string.Format("{0:N1}", dtGrid.Rows[i][8]), fontSize, fontColor);
                CRHelper.AddDataCellR(row1, string.Format("{0:N0}", dtGrid.Rows[i][9]), fontSize, mainColumnColor);

                if (i == 2) 
                {
                    CRHelper.SetCellHBorder(row1);
                }

                CRHelper.SetCellVBorder(row1, 0, 1);
                CRHelper.SetCellVBorder(row1, 2, 3);
                CRHelper.SetCellVBorder(row1, 4, 5);
                CRHelper.SetCellVBorder(row1, 6, 7);

                gridTable1.Rows.Add(row1);            
            }
        }
    }
}

