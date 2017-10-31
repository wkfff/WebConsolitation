using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Infragistics.WebUI.UltraWebGrid;
using System.Drawing;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FNS_0001_0005
{
    public partial class Default_avgIncTable : CustomReportPage
    {
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            int currentWidth = (int)Session["width_size"] - 50;
            UltraWebGrid.Width = (int)(currentWidth);

            int currentHeight = (int)Session["height_size"] - 340;
            UltraWebGrid.Height = (int)(currentHeight);           
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!Page.IsPostBack)
            {
                comboYear.SelectedIndex = 10;
                comboMonth.SelectedIndex = 4;
            }
            string pValue = string.Format("[Период].[Месяц].[Данные всех периодов].[{0}].[Полугодие {1}].[Квартал {2}].[{3}]",
                comboYear.SelectedRow.Cells[0].Text,
                CRHelper.HalfYearNumByMonthNum(comboMonth.SelectedIndex + 1),
                CRHelper.QuarterNumByMonthNum(comboMonth.SelectedIndex + 1),
                comboMonth.SelectedRow.Cells[0].Text);
            if (!Page.IsPostBack || UserParams.PeriodMonth.Value != pValue)
            {
                UserParams.PeriodMonth.Value = pValue;
                UserParams.PeriodYear.Value = string.Format("[Период].[Год].[Данные всех периодов].[{0}]",
                    comboYear.SelectedRow.Cells[0].Text);
                UltraWebGrid.DataBind();
            }
        }

        DataTable dtTable = new DataTable();

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("tableAvgInc");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Район", dtTable);            
            UltraWebGrid.DataSource = dtTable;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            if (e.Layout.Bands[0].Columns.Count == 0)
                return;

            UltraGridColumn col;

            if (!Page.IsPostBack)
            {
                col = e.Layout.Bands[0].Columns[1];
                e.Layout.Bands[0].Columns.RemoveAt(1);
                e.Layout.Bands[0].Columns.Insert(0, col);


                col = e.Layout.Bands[0].Columns[0];
                col.Width = 50;

                col = e.Layout.Bands[0].Columns[2];
                col.Hidden = true;

                for (int i = 3; i <= 6; i++)
                {
                    col = e.Layout.Bands[0].Columns[i];
                    col.Width = 110;
                    CRHelper.FormatNumberColumn(col, "N2");
                    col.CellStyle.HorizontalAlign = HorizontalAlign.Right;
                    col.Header.Caption = string.Format("{0}, руб.", col.Header.Caption);
                }

                col = e.Layout.Bands[0].Columns[6];
                col.Width = 130;

                col = e.Layout.Bands[0].Columns[7];
                col.Width = 100;
                CRHelper.FormatNumberColumn(col, "P4");
                col.CellStyle.HorizontalAlign = HorizontalAlign.Right;

                col = e.Layout.Bands[0].Columns[8];
                col.Width = 100;
                CRHelper.FormatNumberColumn(col, "N0");
                col.CellStyle.HorizontalAlign = HorizontalAlign.Right;

                col = e.Layout.Bands[0].Columns[9];
                col.Width = 100;
                CRHelper.FormatNumberColumn(col, "P4");
                col.CellStyle.HorizontalAlign = HorizontalAlign.Right;

                col = e.Layout.Bands[0].Columns[10];
                col.Width = 100;
                CRHelper.FormatNumberColumn(col, "N2");
                col.CellStyle.HorizontalAlign = HorizontalAlign.Right;
                col.Header.Caption = string.Format("{0}, руб./чел.", col.Header.Caption);

                col = e.Layout.Bands[0].Columns[11];
                col.Width = 75;
                col.CellStyle.HorizontalAlign = HorizontalAlign.Center;
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            switch (e.Row.Cells[2].Text)
            {
                case "(All)":
                    {
                        e.Row.Style.Font.Size = FontUnit.Small;
                        break;
                    }
                case "Районы":
                    {
                        e.Row.Style.Font.Bold = true;
                        break;
                    }
                case "Поселения":
                    {
                        break;
                    }
            }

            double cellValue;            
            if (double.TryParse(e.Row.Cells[11].Text, out cellValue))
            {
                e.Row.Cells[11].Style.BackColor = Color.FromArgb(200, 200, (int)(byte)(255 / 20 * cellValue));
            }
        }
    }
}
