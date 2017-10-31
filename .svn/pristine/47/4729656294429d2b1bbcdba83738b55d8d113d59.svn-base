using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using System.Xml;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Core;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class STAT_0001_0001 : CustomReportPage
    {
        private DataTable dt;

        // Дата
        private CustomParam periodDay;

        private DateTime date;
        private DateTime lastDate;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            periodDay = UserParams.CustomParam("period_day");

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0001_0001_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            date = CRHelper.DateByPeriodMemberUName(dtDate.Rows[0][5].ToString(), 3);
            lastDate = date.AddDays(-7);

            Image4.ImageUrl = "../../../images/workers.png";

            periodDay.Value = dtDate.Rows[0][5].ToString();
            
            LabelsDataBind();
            UltraWebGrid.DataBind();
        }

        private void LabelsDataBind()
        {
            dt = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0001_0001_FO");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dt);

            lb1.Text = string.Format("Число безработных в УрФО<br/>на {0:dd.MM.yyyy}&nbsp;", date);
            lb2.Text = "Уровень безработицы&nbsp;";
            lb3.Text = "<br/>Зарегистрированных безработных в расчете ";
            
            lb1Value.Text = String.Format("{0:N0}&nbsp;", dt.Rows[0][1]);
            lb2Value.Text = String.Format("{0:N2}%&nbsp;", dt.Rows[1][1]);
            lb3Value.Text = String.Format("{0:N2}&nbsp;", dt.Rows[2][1]);

            if (Convert.ToDouble(dt.Rows[0][2]) > 0)
            {
                lb1Percent.Text = String.Format("+{0:P2}", dt.Rows[0][2]);
                Label2.Text = String.Format("&nbsp;+{0:N0}&nbsp;", dt.Rows[0][3]);
            }
            else
            {
                lb1Percent.Text = String.Format("{0:P2}", dt.Rows[0][2]);
                Label2.Text = String.Format("&nbsp;{0:N0}&nbsp;", dt.Rows[0][3]);
            }
            if (Convert.ToDouble(dt.Rows[0][2]) < 0)
            {
                Image1.ImageUrl = "../../../images/arrowDownGreen.png";
                lbGrown1.Text = "<br/>снижение c ";
            }
            else
            {
                Image1.ImageUrl = "../../../images/arrowUpRed.png";
                lbGrown1.Text = "<br/>прирост c ";
            }

            lbGrown1.Text += lastDate.ToString("dd.MM.yyyy") + "<br/>";

            Image1.Height = 20;
        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            DataTable dtSource = new DataTable();
            
            string query = DataProvider.GetQueryText("STAT_0001_0001_grid");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dtSource);

            dt = dtSource.Clone();
            DataRow[] rows = dtSource.Select("", "ТемпПрироста DESC");

            for (int i = 0; i < rows.Length; i++ )
            {
                dt.ImportRow(rows[i]);
                dt.Rows[i][0] = RegionsNamingHelper.ShortName(dt.Rows[i][0].ToString());
            }

            UltraWebGrid.DataSource = dt;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            
            if (e.Layout.Bands.Count == 0)
                return;

            
            e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = FontUnit.Parse("16px");
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);

            e.Layout.Bands[0].Columns[0].Width = 73;
            e.Layout.Bands[0].Columns[1].Width = 110;
            e.Layout.Bands[0].Columns[2].Width = 80;
            e.Layout.Bands[0].Columns[3].Width = 48;

            e.Layout.Bands[0].Columns[1].CellStyle.Padding.Right = 8;
            e.Layout.Bands[0].Columns[2].CellStyle.Padding.Right = 2;
            

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "P2");
          //  CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N0");

            e.Layout.Bands[0].Columns[2].Header.Caption = "Прирост";
            e.Layout.Bands[0].Columns[2].Header.Style.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[2].Header.Style.Padding.Left = 25;

            e.Layout.Bands[0].Columns[2].Header.RowLayoutColumnInfo.SpanX = 2;
            e.Layout.Bands[0].Columns[2].CellStyle.BorderDetails.WidthRight = 0;
            e.Layout.Bands[0].Columns[3].CellStyle.BorderDetails.WidthLeft = 0;
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            double value;
            if (e.Row.Cells[2].Value != null && Double.TryParse(e.Row.Cells[2].Value.ToString(), out value))
            {
                if (value > 0)
                {
                    e.Row.Cells[2].Value = String.Format("+{0:P2}", value);
                    e.Row.Cells[2].Style.BackgroundImage = "../../../images/arrowUpRed.png";
                }
                else
                {
                    e.Row.Cells[2].Value = String.Format("{0:P2}", value);
                    e.Row.Cells[2].Style.BackgroundImage = "../../../images/arrowDownGreen.png";
                }
                e.Row.Cells[2].Style.CustomRules =
                    "background-repeat: no-repeat; background-position: left center; padding-top: 2px; padding-right: 2px";
            }
        }
    }
}
