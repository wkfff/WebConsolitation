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
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class UFK_0008_0001_H : CustomReportPage
    {
        private DateTime dateCurrentYear;
        private DateTime dateLastYear;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("UFK_0008_0001_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            UserParams.PeriodYear.Value = dtDate.Rows[0][0].ToString();
            UserParams.PeriodLastYear.Value = (Convert.ToInt32(dtDate.Rows[0][0]) - 1).ToString();
            UserParams.PeriodCurrentDate.Value = dtDate.Rows[0][5].ToString();
            UserParams.PeriodLastDate.Value =
                string.Format("{0}].[{1}].[{2}].[{3}].[{4}", UserParams.PeriodLastYear.Value,
                                                             dtDate.Rows[0][1],
                                                             dtDate.Rows[0][2],
                                                             dtDate.Rows[0][3],
                                                             dtDate.Rows[0][4]);
            
            Label7.Text = string.Format("данные на {0} {1} {2} года", dtDate.Rows[0][4], CRHelper.RusMonthGenitive(CRHelper.MonthNum(dtDate.Rows[0][3].ToString())), dtDate.Rows[0][0]);
            Label6.Text = string.Format("обновлено {0:dd.MM.yyyy HH:mm:ss}", DateTime.Now);
            int year = Convert.ToInt32(dtDate.Rows[0][0]);
            
            TextBox1.Text = String.Format("План % = запланированный темп роста доходов {0}г. к {1}г.", year, year - 1);
            dateCurrentYear = new DateTime(year, CRHelper.MonthNum(dtDate.Rows[0][3].ToString()), Convert.ToInt32(dtDate.Rows[0][4]));
            dateLastYear = new DateTime(year - 1, CRHelper.MonthNum(dtDate.Rows[0][3].ToString()), Convert.ToInt32(dtDate.Rows[0][4]));
            TextBox2.Text = String.Format("Факт % = фактический темп роста доходов {0:dd.MM.yyyy}г. к {1:dd.MM.yyyy}г.", dateCurrentYear, dateLastYear);
            TextBox3.Text = String.Format("Фактическое исполнение в таблице в тысячах рублей");
            TextBox4.Text = String.Format("Данные на диаграммах в миллионах рублей");
            
            UltraWebGrid.DataBind();
            ChartsDataBind();
        }

        private void ChartsDataBind()
        {
            DataTable dt1 = new DataTable();
            DataTable dt2 = new DataTable();
            DataTable dt3 = new DataTable();

            string query = DataProvider.GetQueryText("UFK_0008_0001_h_chart1");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Доходы", dt1);
            ConvertToMLN(ref dt1);

            UltraChart1.DataSource = dt1;
            UltraChart1.DataBind();

            query = DataProvider.GetQueryText("UFK_0008_0001_h_chart2");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Доходы", dt2);
            ConvertToMLN(ref dt2);
            
            UltraChart2.DataSource = dt2;
            UltraChart2.DataBind();
            UltraChart2.Legend.Visible = false;

            query = DataProvider.GetQueryText("UFK_0008_0001_h_chart3");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Доходы", dt3);
            ConvertToMLN(ref dt3);

            UltraChart3.DataSource = dt3;
            UltraChart3.DataBind();
            UltraChart3.Legend.Visible = false;

            UltraChart1.TitleTop.Text = "Всего доходов";
            UltraChart2.TitleTop.Text = "Налог на прибыль";
            UltraChart3.TitleTop.Text = "Налог на доходы физических лиц";

            UltraChart1.Height = 300;
            UltraChart2.Height = 270;
            UltraChart3.Height = 270;
            UltraChart1.Width = 470;
            UltraChart2.Width = 470;
            UltraChart3.Width = 470;

            UltraChart1.Legend.Margins.Bottom = 10;
            UltraChart1.Legend.Margins.Top = 0;

            UltraChart1.TitleTop.Margins.Bottom = -46;
            UltraChart2.TitleTop.Margins.Bottom = -5;
            UltraChart3.TitleTop.Margins.Bottom = 0;

            UltraChart1.Tooltips.FormatString = "<ITEM_LABEL> <DATA_VALUE:N3> млн.руб.";
            UltraChart2.Tooltips.FormatString = "<ITEM_LABEL> <DATA_VALUE:N3> млн.руб.";
            UltraChart3.Tooltips.FormatString = "<ITEM_LABEL> <DATA_VALUE:N3> млн.руб.";
       }

        private static void ConvertToMLN(ref DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                if (row[1] != DBNull.Value)
                {
                    row[1] = Convert.ToDouble(row[1]) / 1000000;
                }
                if (row[2] != DBNull.Value)
                {
                    row[2] = Convert.ToDouble(row[2]) / 1000000;
                }
                if (row[3] != DBNull.Value)
                {
                    row[3] = Convert.ToDouble(row[3]) / 1000000;
                }
            }
        }

        protected void Grid_DataBinding(object sender, EventArgs e)
        {
            

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("UFK_0008_0001_h_table");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Доходы", dt);

            foreach(DataRow row in dt.Rows)
            {
                if (row[1] != DBNull.Value)
                {
                    row[1] = Convert.ToDouble(row[1]) * 100;
                }

                if (row[2] != DBNull.Value)
                {
                    row[2] = Convert.ToDouble(row[2]) * 100;
                }

                if (row[3] != DBNull.Value)
                {
                    row[3] = Convert.ToDouble(row[3]) / 1000;
                }

                if (row[4] != DBNull.Value)
                {
                    row[4] = Convert.ToDouble(row[4]) / 1000;
                }
            }

            UltraWebGrid.DataSource = dt.DefaultView;
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;

            if (e.Layout.Bands.Count == 0)
                return;

            if (e.Layout.Bands[0].Columns.Count > 3)
            {
                UltraGridColumn column = new UltraGridColumn();
                e.Layout.Bands[0].Columns.Insert(3, column);
                e.Layout.Bands[0].Columns[1].Header.Caption = "План %";
                e.Layout.Bands[0].Columns[2].Header.Caption = "Факт %";
                e.Layout.Bands[0].Columns[4].Header.Caption = string.Format("Факт {0:dd.MM.yy}", dateCurrentYear);
                e.Layout.Bands[0].Columns[5].Header.Caption = string.Format("Факт {0:dd.MM.yy}", dateLastYear);
                e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = FontUnit.Parse("16px");
                e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
                e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

                e.Layout.Bands[0].Columns[1].CellStyle.Font.Bold = true;
                e.Layout.Bands[0].Columns[2].CellStyle.Font.Bold = true;
                e.Layout.Bands[0].Columns[3].CellStyle.Font.Bold = true;
                e.Layout.Bands[0].Columns[4].CellStyle.Font.Bold = true;
                e.Layout.Bands[0].Columns[5].CellStyle.Font.Bold = true;

                e.Layout.Bands[0].Columns[0].Width = 158;
                e.Layout.Bands[0].Columns[1].Width = 62;
                e.Layout.Bands[0].Columns[2].Width = 60;
                e.Layout.Bands[0].Columns[3].Width = 25;
                e.Layout.Bands[0].Columns[4].Width = 85;
                e.Layout.Bands[0].Columns[5].Width = 85;

                e.Layout.Bands[0].Columns[0].CellStyle.Padding.Left = 5;

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N2");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N0");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "N0");
            }
        }

        protected void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            if (Convert.ToDouble(e.Row.Cells[2].Value) < Convert.ToDouble(e.Row.Cells[1].Value))
            {
                e.Row.Cells[3].Style.BackgroundImage = "~/images/red.png";
                e.Row.Cells[3].Style.CustomRules = "background-repeat: no-repeat; background-position: center";
            }
            else if (Convert.ToDouble(e.Row.Cells[2].Value) > Convert.ToDouble(e.Row.Cells[1].Value))
            {
                e.Row.Cells[3].Style.BackgroundImage = "~/images/green.png";
                e.Row.Cells[3].Style.CustomRules = "background-repeat: no-repeat; background-position: center";
            }
            foreach(UltraGridCell cell in e.Row.Cells)
            {
                cell.Style.BorderColor = Color.FromArgb(50, 50, 50);
            }
            e.Row.Cells[2].Style.BorderDetails.StyleRight = BorderStyle.None;
        }
    }
}
