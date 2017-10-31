using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.UltraGauge.Resources;
using Infragistics.WebUI.UltraWebGauge;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Image = System.Web.UI.WebControls.Image;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0003_0001_Horizontal : CustomReportPage
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            CustomParams.MakeRegionParams("65", "id");

            HeraldImageContainer.InnerHtml = String.Format("<a href ='{1}'><img style='margin-right: 20px; height: 65px' src=\"../../../images/Heralds/{0}.png\"></a>", HttpContext.Current.Session["CurrentSubjectID"], HttpContext.Current.Session["CurrentSiteRef"]);
            PersonImageContainer.InnerHtml = String.Format("<a href ='webcommand?showPopoverReport=fo_0003_0003_{0}&width=690&height=530&fitByHorizontal=true'><img src=\"../../../images/person.png\"></a>", HttpContext.Current.Session["CurrentSubjectID"]);
            TwitterImageContainer.InnerHtml = String.Format("<a href ='webcommand?showPopoverReport=fo_0003_0007_{0}&width=690&height=530&fitByHorizontal=true'><img src=\"../../../images/twitter.png\"></a>", HttpContext.Current.Session["CurrentSubjectID"]);

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_0003_0001_Horizontal_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "дата", dtDate);

            UserParams.PeriodCurrentDate.Value = dtDate.Rows[0][1].ToString();
            DateTime date = CRHelper.DateByPeriodMemberUName(dtDate.Rows[0][1].ToString(), 3);

            UltraWebGridIncomes.InitializeRow += new InitializeRowEventHandler(UltraWebGridIncomes_InitializeRow);
            UltraWebGridOutcomes.InitializeRow += new InitializeRowEventHandler(UltraWebGridOutcomes_InitializeRow);
            UltraWebGridSources.InitializeRow += new InitializeRowEventHandler(UltraWebGridSources_InitializeRow);

            SetupGrid(UltraWebGridIncomes, "FO_0003_0001_Horizontal_incomes");
            SetupGrid(UltraWebGridOutcomes, "FO_0003_0001_Horizontal_outcomes");
            SetupGrid(UltraWebGridSources, "FO_0003_0001_Horizontal_sources");

            UltraWebGridOutcomes.Bands[0].HeaderLayout.Clear();
            UltraWebGridSources.Bands[0].HeaderLayout.Clear();

            UltraWebGridOthers.Width = Unit.Empty;
            UltraWebGridOthers.Height = Unit.Empty;
            UltraWebGridOthers.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGridOthers_InitializeLayout);
            UltraWebGridOthers.InitializeRow += new InitializeRowEventHandler(UltraWebGridOthers_InitializeRow);

            DataTable dt = new DataTable();
            query = DataProvider.GetQueryText("FO_0003_0001_Horizontal_others");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель ", dt);

            UltraWebGridOthers.DataSource = dt;
            dt.Columns.RemoveAt(0);
            UltraWebGridOthers.DataBind();

            lbDescription.Text = String.Format("Показатели исполнения бюджетов субъекта РФ&nbsp;<span class='DigitsValue'>на {0:dd.MM.yyyy}</span>, тыс.руб.", date.AddMonths(1));
        }

        void UltraWebGridOthers_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Index == 0)
            {
                e.Row.Cells[0].Value = "<div style='margin-bottom: -50px'><img style='padding-top: 5px' src='../../../images/Detail.png'/></a></div>";
                e.Row.Cells[0].Style.BorderDetails.ColorBottom = Color.Transparent;
            }
            else if (e.Row.Index != 3)
            {
                e.Row.Cells[0].Style.BorderDetails.ColorBottom = Color.Transparent;
                e.Row.Cells[0].Style.BorderDetails.ColorTop = Color.Transparent;
            }
        }

        void UltraWebGridSources_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Index == 0)
            {
                e.Row.Cells[0].Value = "<div style='margin-bottom: -50px;'><a href='webcommand?showPinchReport='><img style=' src='../../../images/Detail.png'/></a></div>";
                e.Row.Cells[0].Style.BorderDetails.ColorBottom = Color.Transparent;
            }
            else if (e.Row.Index != 2)
            {
                e.Row.Cells[0].Style.BorderDetails.ColorBottom = Color.Transparent;
                e.Row.Cells[0].Style.BorderDetails.ColorTop = Color.Transparent;
            }

            if (e.Row.Cells[5].Value.ToString() == "1")
            {
                e.Row.Cells[1].Style.Font.Bold = true;
                e.Row.Cells[1].Style.ForeColor = Color.White;
            }
            if (e.Row.Cells[5].Value.ToString() == "2")
            {
                e.Row.Cells[1].Style.ForeColor = Color.White;
            }
            if (e.Row.Cells[5].Value.ToString() == "3")
            {
                e.Row.Cells[1].Style.Padding.Left = 20;
            }
            if (e.Row.Cells[5].Value.ToString() == "4")
            {
                e.Row.Cells[1].Style.Font.Italic = true;
                e.Row.Cells[1].Style.Padding.Left = 20;
            }
            if (e.Row.Cells[1].Value.ToString().Contains("Недостаток") ||
                e.Row.Cells[1].Value.ToString().Contains("Дефицит"))
            {
                iPadBricks.iPadBricks.iPadBricksHelper.SetConditionCorner(e, 2);
            }
        }

        void UltraWebGridOutcomes_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Index == 0)
            {
                e.Row.Cells[0].Value = "<div style='margin-bottom: -50px'><img src='../../../images/Outcomes.png'/><br/><a href='webcommand?showPinchReport='><img style='padding-top: 5px' src='../../../images/Detail.png'/></a></div>";
                e.Row.Cells[0].Style.BorderDetails.ColorBottom = Color.Transparent;
            }
            else if (e.Row.Index != 6)
            {
                e.Row.Cells[0].Style.BorderDetails.ColorBottom = Color.Transparent;
                e.Row.Cells[0].Style.BorderDetails.ColorTop = Color.Transparent;
            }

            if (e.Row.Cells[5].Value.ToString() == "1")
            {
                e.Row.Cells[1].Style.Font.Bold = true;
                e.Row.Cells[1].Style.ForeColor = Color.White;
            }
            if (e.Row.Cells[5].Value.ToString() == "2")
            {
                e.Row.Cells[1].Style.ForeColor = Color.White;
            }
            if (e.Row.Cells[5].Value.ToString() == "3")
            {
                e.Row.Cells[1].Style.Padding.Left = 20;
            }
            if (e.Row.Cells[5].Value.ToString() == "4")
            {
                e.Row.Cells[1].Style.Font.Italic = true;
                e.Row.Cells[1].Style.Padding.Left = 20;
            }
            if (e.Row.Cells[1].Value.ToString().Contains("Недостаток") ||
                e.Row.Cells[1].Value.ToString().Contains("Дефицит"))
            {
                iPadBricks.iPadBricks.iPadBricksHelper.SetConditionCorner(e, 2);
            }
        }

        void UltraWebGridIncomes_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Index == 0)
            {
                e.Row.Cells[0].Value = "<div style='margin-bottom: -50px'><img src='../../../images/Incomes.png'/><br/><a href='webcommand?showPinchReport='><img style='padding-top: 5px' src='../../../images/Detail.png'/></a></div>";
                e.Row.Cells[0].Style.BorderDetails.ColorBottom = Color.Transparent;
            }
            else if (e.Row.Index != 9)
            {
                e.Row.Cells[0].Style.BorderDetails.ColorBottom = Color.Transparent;
                e.Row.Cells[0].Style.BorderDetails.ColorTop = Color.Transparent;
            }

            if (e.Row.Cells[5].Value.ToString() == "1")
            {
                e.Row.Cells[1].Style.Font.Bold = true;
                e.Row.Cells[1].Style.ForeColor = Color.White;
            }
            if (e.Row.Cells[5].Value.ToString() == "2")
            {
                e.Row.Cells[1].Style.ForeColor = Color.White;
            }
            if (e.Row.Cells[5].Value.ToString() == "3")
            {
                e.Row.Cells[1].Style.Padding.Left = 20;
            }
            if (e.Row.Cells[5].Value.ToString() == "4")
            {
                e.Row.Cells[1].Style.Font.Italic = true;
                e.Row.Cells[1].Style.Padding.Left = 20;
            }

        }

        void UltraWebGridOthers_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].Grid.Columns.Insert(0, new UltraGridColumn());

            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Center;

            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[1].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);

            e.Layout.Bands[0].Columns[0].Width = 53;
            e.Layout.Bands[0].Columns[1].Width = 425;
            e.Layout.Bands[0].Columns[2].Width = 285;

            e.Layout.Bands[0].Columns[2].CellStyle.Padding.Right = 3;

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N0");
        }

        private void SetupGrid(UltraWebGrid grid, string queryName)
        {
            grid.Width = Unit.Empty;
            grid.Height = Unit.Empty;
            grid.InitializeLayout += new InitializeLayoutEventHandler(grid_InitializeLayout);
            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель ", dt);

            grid.DataSource = dt;
            dt.Columns.RemoveAt(0);
            grid.DataBind();
        }

        void grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].Grid.Columns.Insert(0, new UltraGridColumn());

            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[1].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);

            e.Layout.Bands[0].Columns[0].Width = 53;
            e.Layout.Bands[0].Columns[1].Width = 370;
            e.Layout.Bands[0].Columns[2].Width = 110;
            e.Layout.Bands[0].Columns[3].Width = 180;
            e.Layout.Bands[0].Columns[4].Width = 180;

            e.Layout.Bands[0].Columns[2].CellStyle.Padding.Right = 3;
            e.Layout.Bands[0].Columns[3].CellStyle.Padding.Right = 3;
            e.Layout.Bands[0].Columns[4].CellStyle.Padding.Right = 3;

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "P0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "P0");

            e.Layout.Bands[0].Columns[5].Hidden = true;
        }
    }
}
