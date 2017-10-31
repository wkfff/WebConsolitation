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
using Microsoft.AnalysisServices.AdomdClient;

using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Common;

namespace Krista.FM.Server.Dashboards.reports.PMO_0001_0010
{
    public partial class Default : CustomReportPage
    {
        #region Разбор SQL
        public  DataTable getDataSet(string[] name,string SQL)
        {
             DataTable DS = new DataTable();
             try
             {
                 CellSet val;
                 string s = DataProvider.GetQueryText(SQL);
                 val = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(s);
                 for (int i = 0; i < name.Length; i++)
                 {
                     DS.Columns.Add(name[i]);
                 };

                 for (int i = 0; i <= (val.Cells.Count) / (name.Length - 1)-1; i++)
                 {
                     object[] ob = new object[name.Length];
                     ob[0] = val.Axes[1].Positions[i].Members[0].Caption;

                     for (int j = 1; j < name.Length; j++)
                     {
                         ob[j] = float.Parse(val.Cells[j - 1, i].FormattedValue);
                     }
                     DS.Rows.Add(ob);
                 }
             }
             catch
             {
                 DS = null; 
             };
            return DS;
        }
        #endregion
        #region ForChart
        public DataTable GetDSForChart(string sql)
        {
            DataTable dt = new DataTable();
            try
            {
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText(sql), "a", dt);
            }
            catch
            {
                dt = null;
            }
            return dt;
        }

        #endregion
        protected void Layot(LayoutEventArgs e)
        {
            e.Layout.NoDataMessage = "Нет данных";
            e.Layout.NullTextDefault = "Нет данных";
        }
        string pageSubTitle = "Анализ динамики и структуры основных показателей, характеризующих инвестиционную сферу в муниципальном образовании";
        string gridLeftTitle = "Инвестиции производственного и непроизводственного назначения, {0}";
        string gridRightTitle = "Источники инвестиций, ";
        string chartNewTitle = "Структура инвестиций непроизводственного назначения по отраслям в&nbsp;{0}&nbsp;году, %";
        string chartCenterTitle = "Структура инвестиций производственного назначения по отраслям в&nbsp;{0}&nbsp;году, %";
        string chartLeftTitle = "Удельный вес производственных и непроизводственных инвестиций в общем объеме в&nbsp;{0}&nbsp;году, %";
        string chartRightTitle = "Удельный вес источников финансирования в общем объеме инвестиций в&nbsp;{0}&nbsp;году, %";
        string[] nameFieldTableLeft = {"Год","Производственного назначения","Непроизводственого назначения"};
        string[] nameFieldTableCenter = {"_","__"};
        string[] nameFieldTableRight = {"Год","Средства бюджета","Собственые средства предприятий","Привлеченные средства"};
        string[] nameForLeftChart = {"",""};
        private int Width { get { return (int)Session["width_size"]; } }
        private CustomParam current_region { get { return (UserParams.CustomParam("current_region")); } }
        private CustomParam current_year { get { return (UserParams.CustomParam("current_year")); } }
        private CustomParam current_years { get { return (UserParams.CustomParam("current_years")); } }
        private CustomParam lastYear { get { return (UserParams.CustomParam("lastYear")); } }
        // параметр запроса для региона
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }
        string BN = "IE";
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
            BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();
            WebAsyncRefreshPanel4.AddLinkedRequestTrigger(GridLeft);
            WebAsyncRefreshPanel5.AddLinkedRequestTrigger(GridRight);

            GridRight.EnableAppStyling = Infragistics.WebUI.Shared.DefaultableBoolean.True;
            GridRight.StyleSetName = "Office2007Blue";

            GridLeft.EnableAppStyling = Infragistics.WebUI.Shared.DefaultableBoolean.True;
            GridLeft.StyleSetName = "Office2007Blue";
            GridLeft.Width = (int)((Width - 55) * 0.33);
            GridRight.Width = (int)((Width - 55) * 0.33);
            ChartLeft.Width = (int)((Width - 55) * 0.33);

            ChartCenter.Width = (int)((Width - 55) * 0.33);
            ChartRight.Width = (int)((Width - 55) * 0.33);
            ChartNew.Width = (int)((Width - 55) * 0.33);

            ChartLeft.Height = 300;
            if (BN == "IE")
            {
                GridLeft.Height = CRHelper.GetChartHeight(284);
                GridRight.Height =CRHelper.GetChartHeight(271);
            }
            else
            {
                if (BN == "FIREFOX")
                {
                    GridLeft.Height = CRHelper.GetChartHeight(320);
                    GridRight.Height = CRHelper.GetChartHeight(319);
                }
                else
                {
                    GridLeft.Height = CRHelper.GetChartHeight(285);
                    GridRight.Height = CRHelper.GetChartHeight(274);
                }
            }

        }
        protected override void Page_Load(object sender, EventArgs e)
        {
                base.Page_Load(sender, e);
                if (!Page.IsPostBack)
                {
                    current_region.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
                    lastYear.Value = GetLastYear("lastYear");
                    GridLeft.DataBind();

                    current_year.Value = GridLeft.Rows[GridLeft.Rows.Count-1].Cells[0].Text;
                    Label7.Text = String.Format(chartCenterTitle, current_year.Value);// "Структура инвестиций производственного назначения, по отраслям в&nbsp;" + current_year.Value + "&nbsp;году, %";
                    Label3.Text = String.Format(chartNewTitle, current_year.Value);// "Структура инвестиций непроизводственного назначения по отраслям в&nbsp;" + current_year.Value + "&nbsp;году, %";
                    GridRight.DataBind();
                    if (GridRight.DataSource != null)
                    {
                        current_years.Value = GridRight.Rows[GridRight.Rows.Count-1].Cells[0].Text;
                    }
                    Label8.Text = String.Format(chartRightTitle, current_years.Value);//"Удельный вес источников финансирования в общем объеме инвестиций в&nbsp;" + current_years.Value + "&nbsp;году";


                    ChartLeft.DataBind();
                    ChartCenter.DataBind();
                    ChartRight.DataBind();
                    ChartNew.DataBind();



                    GridRight.DisplayLayout.GroupByBox.Hidden = 1 == 1;
                    GridLeft.DisplayLayout.GroupByBox.Hidden = 1 == 1;
                    GridLeft.DisplayLayout.RowSelectorsDefault = RowSelectors.Yes;
                    GridRight.DisplayLayout.RowSelectorsDefault = RowSelectors.Yes;

                    GridLeft.BorderColor = Color.White;
                    GridRight.BorderColor = Color.White;

                    GridLeft.DisplayLayout.HeaderStyleDefault.Wrap = 1 == 1;
                    GridRight.DisplayLayout.HeaderStyleDefault.Wrap = 1 == 1;

                    GridLeft.DisplayLayout.CellClickActionDefault = CellClickAction.RowSelect;
                    GridRight.DisplayLayout.CellClickActionDefault = CellClickAction.RowSelect;

                    ChartLeft.Border.Color = Color.Transparent;
                    ChartCenter.Border.Color = Color.Transparent;
                    ChartRight.Border.Color = Color.Transparent;
                    ChartNew.Border.Color = Color.Transparent;

                    ChartLeft.Legend.Location = Infragistics.UltraChart.Shared.Styles.LegendLocation.Bottom;
                    ChartLeft.Legend.Visible = 1 == 1;
                    ChartLeft.Legend.SpanPercentage = 25;
                    ChartLeft.Legend.BorderColor = Color.Transparent;

                    ChartCenter.Legend.Location = Infragistics.UltraChart.Shared.Styles.LegendLocation.Bottom;
                    ChartCenter.Legend.Visible = 1 == 1;
                    ChartCenter.Legend.BorderColor = Color.Transparent;
                    ChartCenter.Legend.SpanPercentage = 40;

                    ChartNew.Legend.Location = Infragistics.UltraChart.Shared.Styles.LegendLocation.Bottom;
                    ChartNew.Legend.Visible = 1 == 1;
                    ChartNew.Legend.BorderColor = Color.Transparent;
                    ChartNew.Legend.SpanPercentage = 40;

                    ChartRight.Legend.Location = Infragistics.UltraChart.Shared.Styles.LegendLocation.Bottom;
                    ChartRight.Legend.Visible = 1 == 2;
                    ChartRight.Legend.BorderColor = Color.Transparent;
                    ChartRight.Legend.SpanPercentage = 20;

                    Label6.Text = String.Format(chartLeftTitle, current_year.Value); //"Удельный вес производственных и непроизводственных инвестиций в общем объеме в&nbsp;" + current_year.Value + "&nbsp;году";
                    Label1.Text = "Информация об инвестициях за последние 10 лет (" + RegionSettingsHelper.Instance.Name + ")";
                    Page.Title = Label1.Text;
                    Label4.Text = pageSubTitle;
                    // получаем выбранную строку
                    UltraGridRow row = GridLeft.Rows[GridLeft.Rows.Count-1];
                    if (GridLeft.Rows.Count != 0)
                    {
                        // устанавливаем ее активной, если необходимо
                        row.Activate();
                        row.Selected = 1 == 1;
                    }
                    if (GridRight.Rows.Count!=0)
                    {
                        // получаем выбранную строку
                        row = GridRight.Rows[GridRight.Rows.Count-1];
                        // устанавливаем ее активной, если необходимо
                        row.Activate();
                        row.Selected = 1 == 1;
                    }
                }
        }
        private string GetLastYear(string sql)
        {
            DataTable dt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText(sql), "lastYear", dt);
            return "[Период].[Год Квартал Месяц].[Данные всех периодов].["+dt.Rows[dt.Rows.Count-1].ItemArray[0].ToString()+"]";
        }
        public static void setChartErrorFont(Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            e.Text = "Нет данных";
            e.LabelStyle.Font = new Font("Verdana", 15);
            e.LabelStyle.FontColor = Color.LightGray;
            e.LabelStyle.VerticalAlign = StringAlignment.Center;
            e.LabelStyle.HorizontalAlign = StringAlignment.Center;
        }

        protected void GridLeft_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("TableLeft"), "TableLeft", dt);
            if (dt.Rows.Count != 0)
            {
                Label2.Text = String.Format(gridLeftTitle, dt.Rows[0].ItemArray[3].ToString().ToLower());
                dt.Columns.Remove(dt.Columns[3]);
                GridLeft.DataSource = dt;
            }
            else
            {
                GridLeft.DataSource = null;
            }
        }

        protected void GridCenterTop_DataBinding(object sender, EventArgs e)
        {

        }

        protected void GridCenterBottom_DataBinding(object sender, EventArgs e)
        {

        }

        protected void GridRight_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("TableRight"), "TableLeft", dt);
            Label5.Text = gridRightTitle;
            if (dt.Rows.Count != 0)
            {
                Label5.Text += dt.Rows[0].ItemArray[4].ToString().ToLower();
                dt.Columns.Remove(dt.Columns[4]);
                GridRight.DataSource = dt;
            }
            else { GridRight.DataSource = null; }
        }
       
        protected void ChartCenter_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("TableCenterTop"), "TableCenterTop", dt);
            if (dt.Rows.Count != 0)
            {
                ChartCenter.Tooltips.FormatString = "<ITEM_LABEL>, <b><DATA_VALUE:### ### ##0.00></b>, " + dt.Rows[0].ItemArray[2].ToString().ToLower();
                dt.Columns.Remove(dt.Columns[2]);
                ChartCenter.DataSource = dt;
            }
            else { ChartCenter.DataSource = dt; }
        }

        protected void GridLeft_ActiveRowChange(object sender, RowEventArgs e)
        {
            try
            {
                current_year.Value = e.Row.Cells[0].Text;
                Label6.Text = String.Format(chartLeftTitle, current_year.Value);// "Удельный вес производственных и непроизводственных инвестиций в общем объеме в&nbsp;" + current_year.Value + "&nbsp;году";
                Label7.Text = String.Format(chartCenterTitle, current_year.Value); //"Структура инвестиций производственного назначения, по отраслям, тыс. р. (%) в&nbsp;" + current_year.Value + "&nbsp;году";
                Label3.Text = String.Format(chartNewTitle, current_year.Value);// "Структура инвестиций непроизводственного назначения, по отраслям, тыс. р. (%)  в&nbsp;" + current_year.Value + "&nbsp;году";
                ChartLeft.DataBind();
                ChartNew.DataBind();
                ChartCenter.DataBind();
            }
            catch { }
        }

        protected void GridLeft_InitializeLayout(object sender, LayoutEventArgs e)
        {
            Layot(e);
            double tempWidth = GridLeft.Width.Value - 14;
            GridLeft.DisplayLayout.RowSelectorStyleDefault.Width = 20 - 2;
            e.Layout.Bands[0].Columns[0].Width = (int)((tempWidth - 20) * 0.13) - 5;
            e.Layout.Bands[0].Columns[1].Width = (int)((tempWidth - 20) * 0.43) - 5;
            e.Layout.Bands[0].Columns[2].Width = (int)((tempWidth - 20) * 0.43) - 5;
            if (BN == "FIREFOX")
            {
                e.Layout.Bands[0].Columns[1].Width = (int)((tempWidth - 20) * 0.41) - 5;
            }
            if (BN=="APPLEMAC-SAFARI")
            {
            e.Layout.Bands[0].Columns[0].Width = (int)((tempWidth - 20) * 0.15) - 5;
            }
            CRHelper.FormatNumberColumn(GridLeft.Bands[0].Columns[1], "### ### ##0.00");
            CRHelper.FormatNumberColumn(GridLeft.Bands[0].Columns[2], "### ### ##0.00");
            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Header.Caption = nameFieldTableLeft[i];
            }
        }


        protected void ChartLeft_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("ChartLeft"), "ChartLeft", dt);
            if (dt.Rows.Count != 0)
            {
                ChartLeft.Tooltips.FormatString = "<ITEM_LABEL>, <b><DATA_VALUE:### ### ##0.00></b>, " + dt.Rows[0].ItemArray[2].ToString().ToLower();
                dt.Columns.Remove(dt.Columns[2]);
                ChartLeft.DataSource = dt;
            }
            else { ChartLeft.DataSource = null; }
        }

        protected void GridCenterBottom_ActiveRowChange(object sender, RowEventArgs e)
        {
           // ChartCenter.DataSource = GetDSForChart("TableCenterBottom");

            //Label7.Text = "Структура инвестиций производственного назначения, по отраслям в "+current_year.Value+" году";
        }

        protected void GridCenterTop_ActiveRowChange(object sender, RowEventArgs e)
        {
           // ChartCenter.DataSource = GetDSForChart("TableCenterTop");

           // Label7.Text = "Структура инвестиций непроизводственного назначения, по отраслям в "+current_year.Value+" году";
        }

        protected void ChartRight_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("ChartRight"), "ChartRight", dt);
            if (dt.Rows.Count != 0)
            {
                ChartRight.Tooltips.FormatString = "<ITEM_LABEL>, <b><DATA_VALUE:### ### ##0.00></b>, " + dt.Rows[0].ItemArray[2].ToString().ToLower();
                dt.Columns.Remove(dt.Columns[2]);
                ChartRight.DataSource = dt;
            }
            else { ChartRight.DataSource = null; }
        }

        protected void GridRight_ActiveRowChange(object sender, RowEventArgs e)
        {
            current_years.Value = e.Row.Cells[0].Text;
            ChartRight.DataSource = GetDSForChart("ChartRight");
            Label8.Text = String.Format(chartRightTitle, current_years.Value);//"Удельный вес источников финансирования в общем объеме инвестиций в&nbsp; " + current_years.Value + "&nbsp;году";
            ChartRight.DataBind();
        }

        protected void GridCenterTop_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[0].Width = 170;
        }

        protected void GridCenterBottom_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[0].Width = 170;
        }

        protected void ChartLeft_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            setChartErrorFont(e);
        }

        protected void ChartNew_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("TableCenterBottom"), "TableCenterBottom", dt);
            if (dt.Rows.Count != 0)
            {
                ChartNew.Tooltips.FormatString = "<ITEM_LABEL>, <b><DATA_VALUE:### ### ##0.00></b>, " + dt.Rows[0].ItemArray[2].ToString().ToLower();
                dt.Columns.Remove(dt.Columns[2]);
                ChartNew.DataSource = dt;
            }
            else { ChartNew.DataSource = null; }
        }

        protected void ChartNew_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            setChartErrorFont(e);
        }

        protected void GridRight_InitializeLayout(object sender, LayoutEventArgs e)
        {
            double tempWidth = GridRight.Width.Value - 14;
            GridRight.DisplayLayout.RowSelectorStyleDefault.Width = 20 - 2;
            e.Layout.Bands[0].Columns[0].Width = (int)((tempWidth - 20) * 0.13) - 6;
            e.Layout.Bands[0].Columns[1].Width = (int)((tempWidth - 20) * 0.28) - 6;
            e.Layout.Bands[0].Columns[2].Width = (int)((tempWidth - 20) * 0.29) - 6;
            e.Layout.Bands[0].Columns[3].Width = (int)((tempWidth - 20) * 0.29) - 6;
            if (BN=="APPLEMAC-SAFARI")
            {
                e.Layout.Bands[0].Columns[1].Width = (int)((tempWidth - 20) * 0.30) - 6;
            }
            CRHelper.FormatNumberColumn(GridRight.Bands[0].Columns[1], "#### #### ##0.00");
            CRHelper.FormatNumberColumn(GridRight.Bands[0].Columns[2], "#### #### ##0.00");
            CRHelper.FormatNumberColumn(GridRight.Bands[0].Columns[3], "#### #### ##0.00");
            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Header.Caption = nameFieldTableRight[i];
            }
        }

        protected void ChartNew_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            { 
                Infragistics.UltraChart.Core.Primitives.Primitive primitive = e.SceneGraph[i];
                if (primitive is Infragistics.UltraChart.Core.Primitives.Text)
                {
                    
                    Infragistics.UltraChart.Core.Primitives.Text te = (Infragistics.UltraChart.Core.Primitives.Text)primitive;
                    if (te.Path == "Border.Title.Legend")
                    {
                        te.bounds.Width += 30;
                    }
                    
                }
            }
        }

        protected void ChartCenter_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Infragistics.UltraChart.Core.Primitives.Primitive primitive = e.SceneGraph[i];
                if (primitive is Infragistics.UltraChart.Core.Primitives.Text)
                {

                    Infragistics.UltraChart.Core.Primitives.Text te = (Infragistics.UltraChart.Core.Primitives.Text)primitive;
                    if (te.Path == "Border.Title.Legend")
                    {
                        te.bounds.Width += 30;
                    }

                }
            }
        }




    }
}
