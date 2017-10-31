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
    public partial class UFK_0008_0001 : CustomReportPage
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UltraWebGrid.DataBind();
        }

        protected void Grid_DataBinding(object sender, EventArgs e)
        {
            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("UFK_0008_0001_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            int yearNum = Convert.ToInt32(dtDate.Rows[0][0]);

            UserParams.PeriodYear.Value = yearNum.ToString();
            UserParams.PeriodLastYear.Value = (yearNum - 1).ToString();
            UserParams.PeriodMonth.Value = string.Format("[{0}].[{1}].[{2}]",
                                                               dtDate.Rows[0][1],
                                                               dtDate.Rows[0][2],
                                                               dtDate.Rows[0][3]);
            UserParams.PeriodCurrentDate.Value = string.Format("[{0}].[{1}].[{2}].[{3}]",
                                                               dtDate.Rows[0][1],
                                                               dtDate.Rows[0][2],
                                                               dtDate.Rows[0][3],
                                                               dtDate.Rows[0][4]);

            DataTable dt = new DataTable();
            query = DataProvider.GetQueryText("UFK_0008_0001");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Доходы", dt);
            DataColumn col = new DataColumn("Image");
            dt.Columns.Add(col);

            foreach(DataRow row in dt.Rows)
            {
                row[1] = Convert.ToDouble(row[1]) * 100;
                row[3] = Convert.ToDouble(row[3]) * 100;
            }

            UltraWebGrid.DataSource = dt.DefaultView;

            Label.Text = string.Format("данные на {0} {1} {2} года", dtDate.Rows[0][4], CRHelper.RusMonthGenitive(CRHelper.MonthNum(dtDate.Rows[0][3].ToString())), dtDate.Rows[0][0]);
            Label1.Text =
                string.Format("обновлено {0:dd.MM.yyyy HH:mm:ss}", DateTime.Now);
            int year = Convert.ToInt32(dtDate.Rows[0][0]);
            Label2.Text = String.Format("План = запланированный темп роста доходов {0}г. к {1}г.", year, year - 1);
            DateTime dateCurrentYear = new DateTime(year, CRHelper.MonthNum(dtDate.Rows[0][3].ToString()), Convert.ToInt32(dtDate.Rows[0][4]));
            DateTime dateLastYear = new DateTime(year - 1, CRHelper.MonthNum(dtDate.Rows[0][3].ToString()), Convert.ToInt32(dtDate.Rows[0][4]));
            Label3.Text = String.Format("Факт = фактический темп роста доходов {0:dd.MM.yyyy}г. к {1:dd.MM.yyyy}г.", dateCurrentYear, dateLastYear);
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            UltraWebGrid.DisplayLayout.GroupByBox.Hidden = true;

            if (UltraWebGrid.DisplayLayout.Bands.Count == 0)
                return;

            if (UltraWebGrid.DisplayLayout.Bands[0].Columns.Count > 4)
            {
                UltraWebGrid.DisplayLayout.Bands[0].Columns[1].Header.Caption = "План %";
                UltraWebGrid.DisplayLayout.Bands[0].Columns[2].Hidden = true;
                UltraWebGrid.DisplayLayout.Bands[0].Columns[3].Header.Caption = "Факт %";
                
                UltraWebGrid.DisplayLayout.Bands[0].Columns[4].Header.Caption = string.Empty;
                
                UltraWebGrid.DisplayLayout.Bands[0].Columns[0].CellStyle.Font.Size = FontUnit.Parse("16px");
                UltraWebGrid.DisplayLayout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
                UltraWebGrid.DisplayLayout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

                UltraWebGrid.DisplayLayout.Bands[0].Columns[1].CellStyle.Font.Bold = true;
                UltraWebGrid.DisplayLayout.Bands[0].Columns[3].CellStyle.Font.Bold = true;
               
                UltraWebGrid.DisplayLayout.Bands[0].Columns[0].Width = 145;
                UltraWebGrid.DisplayLayout.Bands[0].Columns[1].Width = 70;
                UltraWebGrid.DisplayLayout.Bands[0].Columns[3].Width = 70;
                UltraWebGrid.DisplayLayout.Bands[0].Columns[4].Width = 22;

                UltraWebGrid.DisplayLayout.Bands[0].Columns[0].CellStyle.Padding.Left = 5;

                CRHelper.FormatNumberColumn(UltraWebGrid.DisplayLayout.Bands[0].Columns[1], "N2");
                CRHelper.FormatNumberColumn(UltraWebGrid.DisplayLayout.Bands[0].Columns[3], "N2");
            }
        }

        protected void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            if (Convert.ToDouble(e.Row.Cells[3].Value) < Convert.ToDouble(e.Row.Cells[1].Value))
            {
                e.Row.Cells[4].Style.BackgroundImage = "~/images/red.png";
                e.Row.Cells[4].Style.CustomRules = "background-repeat: no-repeat; background-position: center";
            }
            else if (Convert.ToDouble(e.Row.Cells[3].Value) > Convert.ToDouble(e.Row.Cells[1].Value))
            {
                e.Row.Cells[4].Style.BackgroundImage = "~/images/green.png";
                e.Row.Cells[4].Style.CustomRules = "background-repeat: no-repeat; background-position: center";
            }
            e.Row.Cells[4].Style.BorderDetails.StyleLeft = BorderStyle.None;
            e.Row.Cells[3].Style.BorderDetails.StyleRight = BorderStyle.None;
        }
        

    }
}
