using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.reports.Dashboard;
using Krista.FM.Server.Dashboards.Core;

namespace Krista.FM.Server.Dashboards.reports.Dashboard
{
    public partial class FK_0001_0004_Gadget : GadgetControlBase
    {
        DataTable dt = new DataTable();
        DataTable dtDate = new DataTable();
        CustomReportPage dashboard;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            dashboard = GetCustomReportPage(this);

            UltraWebGrid.Width = 418;
            UltraWebGrid.Height = 320;
            UltraWebGrid.BorderWidth = 0;
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);

            string query = DataProvider.GetQueryText("FK_0001_0004_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            dashboard.UserParams.PeriodDayFK.Value = dtDate.Rows[0][4].ToString();

            int monthNum = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());
            int yearNum = Convert.ToInt32(dtDate.Rows[0][0]);

            Label1.Text = string.Format("данные за {0} {1} {2} года", monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum);

            HyperLink1.Text = "Сравнение темпа роста доходов по субъектам РФ";
            HyperLink2.Text = string.Format("Подробнее {0}", dashboard.UserParams.StateArea.Value);
            HyperLink1.NavigateUrl = "~/reports/FK_0001_0004/DefaultCompare.aspx";
            HyperLink2.NavigateUrl = "~/reports/FK_0001_0004/DefaultDetail.aspx";
            
            UltraWebGrid.DataBind();
        }

        void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            UltraWebGrid.Height = Unit.Empty;
            UltraWebGrid.Width = Unit.Empty;
        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FK_0001_0004");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Доходы", dt);

            foreach (DataRow row in dt.Rows)
            {
                row[0] = row[0].ToString().TrimEnd('_');
            }

            UltraWebGrid.DataSource = dt;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;

            if (e.Layout.Bands[0].Columns.Count > 3)
            {
                e.Layout.Bands[0].HeaderStyle.Wrap = true;
                e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;

                e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
                
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 0, "Доходы", "");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 1, "Темп роста к прошлому году", "Темп роста исполнения к аналогичному периоду прошлого года");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 2, "Доля", "Доля дохода в общей сумме доходов субъекта");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 3, "Доля в прошлом году", "Доля дохода в общей сумме фактических доходов в прошлом году");

                for (int i = 1; i <= 3; i++)
                {
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "P2");
                }

                e.Layout.Bands[0].Columns[0].Width = 180;
                e.Layout.Bands[0].Columns[1].Width = 80;
                e.Layout.Bands[0].Columns[2].Width = 80;
                e.Layout.Bands[0].Columns[3].Width = 80;
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells.Count > 3)
            {
                if (e.Row.Cells[1].Value != null && e.Row.Cells[1].Value.ToString() != string.Empty)
                {
                    if (100*Convert.ToDouble(e.Row.Cells[1].Value) < 100)
                    {
                        e.Row.Cells[1].Style.CssClass = "ArrowDownRed";
                    }
                    else if (100*Convert.ToDouble(e.Row.Cells[1].Value) > 100)
                    {
                        e.Row.Cells[1].Style.CssClass = "ArrowUpGreen";
                    }
                }

                if (e.Row.Cells[2].Value != null && e.Row.Cells[3].Value != null && 
                        e.Row.Cells[3].Value.ToString() != string.Empty)
                {
                    if (Convert.ToDouble(e.Row.Cells[2].Value) < Convert.ToDouble(e.Row.Cells[3].Value))
                    {
                        e.Row.Cells[2].Style.CssClass = "ArrowDownRed";
                    }
                    else if (Convert.ToDouble(e.Row.Cells[2].Value) > Convert.ToDouble(e.Row.Cells[3].Value))
                    {
                        e.Row.Cells[2].Style.CssClass = "ArrowUpGreen";
                    }
                }

                if (e.Row.Index == 8 || e.Row.Index == dt.Rows.Count - 1)
                {
                    foreach(UltraGridCell c in e.Row.Cells)
                    {
                        c.Style.Font.Bold = true;
                    }
                }

                if (e.Row.Index > 0 && e.Row.Index < 6)
                {
                    e.Row.Cells[0].Value = string.Format("      {0}", e.Row.Cells[0].Value);
                    foreach (UltraGridCell cell in e.Row.Cells)
                    {
                        cell.Style.Font.Size = FontUnit.XSmall;
                    }
                }
            }

            foreach (UltraGridCell cell in e.Row.Cells)
            {
                if (cell.Value != null && cell.Value.ToString() != string.Empty)
                {
                    decimal value;
                    if (decimal.TryParse(cell.Value.ToString(), out value))
                    {
                        if (value < 0)
                        {
                            cell.Style.ForeColor = Color.Red;
                        }
                    }
                }
            }
        }

        #region IWebPart Members

        public override string Description
        {
            get { return "Раздел содержит данные Федерального казначейства об исполнении бюджетов субъектов РФ по доходам"; }
        }

        public override string Title
        {
            get { return "Темп роста доходов"; }
        }

        public override string TitleUrl
        {
            get { return "~/reports/FK_0001_0004/DefaultCompare.aspx"; }
        }

        #endregion
    }
}