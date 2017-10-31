using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Drawing;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FK_0001_0001_wm1_h : CustomReportPage
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FK_0001_0001_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            UserParams.PeriodDayFK.Value = dtDate.Rows[0][4].ToString();

            int monthNum = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());
            int yearNum = Convert.ToInt32(dtDate.Rows[0][0]);
            string monthStr = CRHelper.RusManyMonthGenitive(monthNum);

            LabelPageTitle.Text = string.Format("—труктура расходов {0} за {1} {2} {3}", UserParams.ShortStateArea.Value, monthNum, monthStr, yearNum);
            Comment1.Text = string.Format("»сп суб = исполнено в {0} за {1} {2} {3} года, млн. руб.", UserParams.ShortStateArea.Value, monthNum, monthStr, yearNum);
            CRHelper.SetNextMonth(ref yearNum, ref monthNum);

            Label2.Text = string.Format("данные на 1 {0} {1} года", CRHelper.RusMonthGenitive(monthNum), yearNum);
            Label1.Text =
                string.Format("обновлено {0:dd.MM.yyyy HH:mm:ss}", DateTime.Now);
            Comment2.Text = string.Format("% суб = дол€ расхода в общей сумме расходов в {0}", UserParams.ShortStateArea.Value);
            Comment3.Text = string.Format("% {0} = дол€ расхода в федеральном округе", UserParams.ShortRegion.Value);
            Comment4.Text = "% –‘ = дол€ расхода в –оссийской ‘едерации";


            DataTable dt = new DataTable();
            string query1 = DataProvider.GetQueryText("FK_0001_0001_h");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query1, "–айон", dt);

            foreach (DataRow row in dt.Rows)
            {
                if (row[1] != DBNull.Value)
                {
                    row[1] = Convert.ToDouble(row[1]) / 1000000;
                }

                if (row[2] != DBNull.Value)
                {
                    row[2] = Convert.ToDouble(row[2]) * 100;
                }

                if (row[3] != DBNull.Value)
                {
                    row[3] = Convert.ToDouble(row[3]) * 100;
                }

                if (row[4] != DBNull.Value)
                {
                    row[4] = Convert.ToDouble(row[4]) * 100;
                }
            }

            SetDataTable(dt);
        }

        private void SetDataTable(DataTable dtGrid)
        {
            int fontSize = CRHelper.fontFK0001H;
            Color fontColor = CRHelper.fontLightColor;
            Color captionColor = CRHelper.fontTableCaptionColor;
            Color mainColumnColor = CRHelper.fontTableDataColor;

            TableRow rowHeader = new TableRow();

            CRHelper.AddCaptionCell(rowHeader, "–зѕр", fontSize, captionColor);
            CRHelper.AddCaptionCell(rowHeader, "»сп суб млн. руб", fontSize, captionColor);
            CRHelper.AddCaptionCell(rowHeader, "% суб", fontSize, captionColor);
            CRHelper.AddCaptionCell(rowHeader, "% " + UserParams.ShortRegion.Value, fontSize, captionColor);
            CRHelper.AddCaptionCell(rowHeader, "% –‘", fontSize, captionColor);            

            gridTable.Rows.Add(rowHeader);

            for (int i = 0; i < dtGrid.Rows.Count; i++)
            {
                bool isBold = (dtGrid.Rows[i][5].ToString() == "–аздел");
                TableRow row = new TableRow();
                string value = string.Format("{0} ", dtGrid.Rows[i][0]);
                value = value.Replace("ќЅў≈√ќ—”ƒј–—“¬≈ЌЌџ≈", "ќЅў≈√ќ—”ƒј–—“¬.");
                value = value.Replace("ѕ–ј¬ќќ’–јЌ»“≈Ћ№Ќјя", "ѕ–ј¬ќќ’–.");

                CRHelper.AddDataCellL(row, value, fontSize, mainColumnColor, isBold);
                CRHelper.AddDataCellR(row, string.Format("{0:N1}", Convert.ToDouble(dtGrid.Rows[i][1])), fontSize, mainColumnColor, isBold);
                CRHelper.AddDataCellR(row, string.Format("{0:N1}", dtGrid.Rows[i][2]), fontSize, mainColumnColor, isBold);
                CRHelper.AddDataCellR(row, string.Format("{0:N1}", dtGrid.Rows[i][3]), fontSize, mainColumnColor, isBold);
                CRHelper.AddDataCellR(row, string.Format("{0:N1}", dtGrid.Rows[i][4]), fontSize, mainColumnColor, isBold);

                gridTable.Rows.Add(row);
            }
        }
    }
}
