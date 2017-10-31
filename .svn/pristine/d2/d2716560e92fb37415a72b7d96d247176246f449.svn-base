using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Core;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FK_0001_0003_wm1_h : CustomReportPage
    {
        private DataTable dt;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FK_0001_0003_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            UserParams.PeriodDayFK.Value = dtDate.Rows[0][4].ToString();
            UserParams.PeriodLastYear.Value = dtDate.Rows[0][0].ToString();
            UserParams.PeriodYear.Value = (Convert.ToInt32(dtDate.Rows[0][0]) + 1).ToString();

            int monthNum = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());
            int yearNum = Convert.ToInt32(dtDate.Rows[0][0]);
            string monthStr = CRHelper.RusManyMonthGenitive(monthNum);

            LabelTitle.Text = string.Format("Расходы бюджетов, процент исполнения плана по расходам и коэффициент бюджетной обеспеченности (тыс.руб./чел.) за {0} {1} {2} года по субъектам {3}.<br /> Ранг субъекта в {3}.",
                monthNum,
                monthStr,
                yearNum,
                UserParams.ShortRegion.Value);


            LabelComments1.Text = string.Format("Расходы млдр.руб. = расходы консолидированных бюджетов субъектов РФ за {0} {1} {2} года",
                monthNum,
                monthStr,
                yearNum);
            CRHelper.SetNextMonth(ref yearNum, ref monthNum);

            Label1.Text = string.Format("данные на 1 {0} {1} года", CRHelper.RusMonthGenitive(monthNum), yearNum);
            Label2.Text = string.Format("обновлено {0:dd.MM.yyyy HH:mm:ss}", DateTime.Now);

            FillData();
            SetDataTable(dt);
        }

        protected void FillData()
        {
            dt = new DataTable();
            string query = DataProvider.GetQueryText("FK_0001_0003_h");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dt);

            foreach (DataRow row in dt.Rows)
            {
                if (row[2] != DBNull.Value)
                {
                    row[2] = Convert.ToDouble(string.Format("{0:N2}", Convert.ToDouble(row[2]) / 1000000000));
                }
                if (row[3] != DBNull.Value)
                {
                    row[3] = Convert.ToDouble(string.Format("{0:N2}", Convert.ToDouble(row[3]) * 100));
                }
                if (row[6] != DBNull.Value)
                {
                    row[6] = Convert.ToDouble(string.Format("{0:N2}", Convert.ToDouble(row[6])));
                }
                if (row[7] != DBNull.Value)
                {
                    row[7] = Convert.ToDouble(string.Format("{0:N2}", Convert.ToDouble(row[7]) / 1000));
                }
            }
        }

        private void SetDataTable(DataTable dtGrid)
        {
            TableRow rowHeader = new TableRow();

            int fontSize = CRHelper.fontFK0003H;
            Color fontColor = CRHelper.fontLightColor;
            Color captionColor = CRHelper.fontTableCaptionColor;
            Color mainColumnColor = CRHelper.fontTableDataColor;

            CRHelper.AddCaptionCell(rowHeader, "Субъект", fontSize, captionColor);
            CRHelper.AddCaptionCell(rowHeader, "Расход", fontSize, captionColor);
            CRHelper.AddCaptionCell(rowHeader, "Исполн.", fontSize, captionColor);
            CRHelper.AddCaptionCell(rowHeader, "Население", fontSize, captionColor);
            CRHelper.AddCaptionCell(rowHeader, "Коэф. бюдж. обесп.", fontSize, captionColor);

            rowHeader.Cells[2].ColumnSpan = 2;
            rowHeader.Cells[4].ColumnSpan = 2;
            gridTable.Rows.Add(rowHeader);

            rowHeader = new TableRow();
            CRHelper.AddCaptionCell(rowHeader, "", fontSize, captionColor);
            CRHelper.AddCaptionCell(rowHeader, "млрд. руб.", fontSize, captionColor);
            CRHelper.AddCaptionCell(rowHeader, "%", fontSize, captionColor);
            CRHelper.AddCaptionCell(rowHeader, "ранг", fontSize, captionColor);
            CRHelper.AddCaptionCell(rowHeader, "тыс. чел.", fontSize, captionColor);
            CRHelper.AddCaptionCell(rowHeader, "тыс. руб.", fontSize, captionColor);
            CRHelper.AddCaptionCell(rowHeader, "ранг", fontSize, captionColor);

            gridTable.Rows.Add(rowHeader);

            for (int i = 0; i < dtGrid.Rows.Count; i++)
            {
                TableRow row = new TableRow();
                CRHelper.AddDataCellL(row, string.Format("{0}", dtGrid.Rows[i][1]), fontSize, fontColor);
                CRHelper.AddDataCellR(row, string.Format("{0:N2}", Convert.ToDouble(dtGrid.Rows[i][2])), fontSize, mainColumnColor);
                CRHelper.AddDataCellR(row, string.Format("{0:N1}", Convert.ToDouble(dtGrid.Rows[i][3])), fontSize, mainColumnColor);
                CRHelper.AddDataCellR(row, string.Format("{1}{0}", dtGrid.Rows[i][4], CRHelper.GetImage(dtGrid.Rows[i], 4, 5)), fontSize, fontColor);
                CRHelper.AddDataCellR(row, string.Format("{0:N1}", dtGrid.Rows[i][6]), fontSize, mainColumnColor);
                CRHelper.AddDataCellR(row, string.Format("{0:N2}", dtGrid.Rows[i][7]), fontSize, mainColumnColor);
                CRHelper.AddDataCellR(row, string.Format("{1}{0}", dtGrid.Rows[i][8], CRHelper.GetImage(dtGrid.Rows[i], 8, 9)), fontSize, fontColor);

                if (i == 2) CRHelper.SetCellHBorder(row);

                CRHelper.SetCellVBorder(row, 0, 1);
                CRHelper.SetCellVBorder(row, 1, 2);
                CRHelper.SetCellVBorder(row, 3, 4);
                CRHelper.SetCellVBorder(row, 4, 5);

                gridTable.Rows.Add(row);
            }
        }
    }
}
