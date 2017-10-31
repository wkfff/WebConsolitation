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
    public partial class FK_0001_0004_wm2_V : CustomReportPage
    {
        private DataTable dt;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FK_0001_0004_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            UserParams.PeriodDayFK.Value = dtDate.Rows[0][4].ToString();

            int monthNum = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());
            int yearNum = Convert.ToInt32(dtDate.Rows[0][0]);
            string monthStr = CRHelper.RusManyMonthGenitive(monthNum);

            LabelTitle.Text = string.Format("Темп роста по налогу на прибыль и НДФЛ за {0}&nbsp;{3}&nbsp;{1}&nbsp;года к аналогичному периоду предыдущего года по {2} и субъектам {4}",
                            monthNum,
                            yearNum,
                            UserParams.ShortStateArea.Value,
                            monthStr,
                            UserParams.ShortRegion.Value);
            CRHelper.SetNextMonth(ref yearNum, ref monthNum);

            Label1.Text = string.Format("данные на 1 {0} {1} года", CRHelper.RusMonthGenitive(monthNum), yearNum);
            Label2.Text = string.Format("обновлено {0:dd.MM.yyyy HH:mm:ss}", DateTime.Now);
            LabelComments.Text = "доля % = доля налога в бюджете";

            FillData();
            SetDataTable(dt);
        }

        protected void FillData()
        {
            dt = new DataTable();
            string query = DataProvider.GetQueryText("FK_0001_0004_V");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dt);

            foreach (DataRow row in dt.Rows)
            {
                row[0] = row[0].ToString().TrimEnd('_');
                for (int i = 2; i < dt.Columns.Count; i++)
                {
                    if (row[i] != DBNull.Value)
                    {
                        row[i] = Convert.ToDouble(row[i]) * 100;
                    }
                }
            }
        }

        private string GetCellValue(double value)
        {
            if (value < 100)
            {
                return string.Format("{0:N1}", value);
            }
            return string.Format("{0:N0}", value);
        }

        private void SetDataTable(DataTable dtGrid)
        {
            TableRow rowHeader = new TableRow();

            int fontSize = CRHelper.fontFK0004V;
            Color fontColor = CRHelper.fontLightColor;
            Color captionColor = CRHelper.fontTableCaptionColor;
            Color mainColumnColor = CRHelper.fontTableDataColor;

            CRHelper.AddCaptionCell(rowHeader, "Субъект", fontSize, captionColor);
            CRHelper.AddCaptionCell(rowHeader, "Налог на прибыль", fontSize, captionColor);
            CRHelper.AddCaptionCell(rowHeader, "НДФЛ", fontSize, captionColor);

            rowHeader.Cells[0].RowSpan = 2;
            rowHeader.Cells[1].ColumnSpan = 2;
            rowHeader.Cells[2].ColumnSpan = 2;
            gridTable.Rows.Add(rowHeader);

            rowHeader = new TableRow();
            CRHelper.AddCaptionCell(rowHeader, "доля %", fontSize, captionColor);
            CRHelper.AddCaptionCell(rowHeader, "темп роста %", fontSize, captionColor);
            CRHelper.AddCaptionCell(rowHeader, "доля %", fontSize, captionColor);
            CRHelper.AddCaptionCell(rowHeader, "темп роста %", fontSize, captionColor);

            gridTable.Rows.Add(rowHeader);

            string imagePath = string.Empty;
            for (int i = 0; i < dtGrid.Rows.Count; i++)
            {
                TableRow row = new TableRow();
                CRHelper.AddDataCellL(row, string.Format("{0}", dtGrid.Rows[i][1]), fontSize, fontColor);
                CRHelper.AddDataCellR(row, string.Format("{0:N2}", dtGrid.Rows[i][2]), fontSize, fontColor);
                CRHelper.AddDataCellR(row, string.Format("{0:N2}", dtGrid.Rows[i][3]), fontSize, mainColumnColor);
                CRHelper.AddDataCellR(row, string.Format("{0:N2}", dtGrid.Rows[i][4]), fontSize, fontColor);
                CRHelper.AddDataCellR(row, string.Format("{0:N2}", dtGrid.Rows[i][5]), fontSize, mainColumnColor);

                CRHelper.SetCellVBorder(row, 0, 1);
                CRHelper.SetCellVBorder(row, 2, 3);

                if (i == 2) CRHelper.SetCellHBorder(row);
                gridTable.Rows.Add(row);
            }
        }
    }
}
