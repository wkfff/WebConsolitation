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
    public partial class FK_0001_0001_h : CustomReportPage
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
            string monthStr;
            switch (monthNum)
            {
                case 1:
                    {
                        monthStr = "месяц";
                        break;
                    }
                case 2:
                case 3:
                case 4:
                    {
                        monthStr = "месяца";
                        break;
                    }
                default:
                    {
                        monthStr = "месяцев";
                        break;
                    }
            }
            TextBox1.Text = string.Format("Структура расходов {0} за {1} {2} {3} года", UserParams.ShortStateArea.Value, monthNum, monthStr, yearNum);
            TextBox2.Text = string.Format("Исп суб = исполнено в {0} за {1} {2} {3} года, млн. руб.", UserParams.ShortStateArea.Value, monthNum, monthStr, yearNum);
            
            if (monthNum == 12)
            {
                monthNum = 1;
                yearNum++;
            }
            else
            {
                monthNum++;
            }

            Label2.Text = string.Format("данные на 1 {0} {1} года", CRHelper.RusMonthGenitive(monthNum), yearNum);
            Label1.Text =
                string.Format("обновлено {0:dd.MM.yyyy HH:mm:ss}", DateTime.Now);
            TextBox3.Text = string.Format("% суб = доля расхода в общей сумме расходов в {0}", UserParams.ShortStateArea.Value);
            TextBox4.Text = string.Format("% {0} = доля расхода в федеральном округе", UserParams.ShortRegion.Value);
            TextBox5.Text = "% РФ = доля расхода в Российской Федерации";

            UltraWebGrid.DataBind();
        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("FK_0001_0001_h");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Район", dt);

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

            UltraWebGrid.DataSource = dt;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;

            if (e.Layout.Bands.Count == 0)
                return;

            if (e.Layout.Bands[0].Columns.Count > 3)
            {
                e.Layout.Bands[0].Columns[0].Header.Caption = "РзПр";
                e.Layout.Bands[0].Columns[1].Header.Caption = "Исп суб млн.руб.";
                e.Layout.Bands[0].Columns[2].Header.Caption = "% суб";
                e.Layout.Bands[0].Columns[3].Header.Caption = "% " + UserParams.ShortRegion.Value;
                e.Layout.Bands[0].Columns[4].Header.Caption = "% РФ";
                
                e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = FontUnit.Parse("16px");
                e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);

//                e.Layout.Bands[0].Columns[1].CellStyle.Padding.Right = 0;
//                e.Layout.Bands[0].Columns[3].CellStyle.Padding.Right = 0;
//                e.Layout.Bands[0].Columns[5].CellStyle.Padding.Right = 0;

                e.Layout.Bands[0].Columns[0].Width = 235;
                e.Layout.Bands[0].Columns[1].Width = 80;
                e.Layout.Bands[0].Columns[2].Width = 55;
                e.Layout.Bands[0].Columns[3].Width = 55;
                e.Layout.Bands[0].Columns[4].Width = 49;
                e.Layout.Bands[0].Columns[5].Hidden = true;

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N2");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N2");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N2");
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, Infragistics.WebUI.UltraWebGrid.RowEventArgs e)
        {
            if (e.Row.Cells[5].Value.ToString() == "Раздел")
            {
                foreach (UltraGridCell cell in e.Row.Cells)
                {
                    cell.Style.Font.Bold = true;
                }
            }
        }
    }
}
