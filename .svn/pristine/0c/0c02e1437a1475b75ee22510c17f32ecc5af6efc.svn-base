using System;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;

using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.Dashboard
{
    public partial class MFRF_0001_0004_Gadget : GadgetControlBase, IHotReport
    {
        private DataTable dataTable = new DataTable();

        public int Width
        {
            get { return 410; }
        }

        private bool EmbeddedReport
        {
            get
            {
                // Да, если это указано в урле или сессии
                return (Request.Params["embedded"] != null &&
                        Request.Params["embedded"].ToLower() == "yes") ||
                       (Session["Embedded"] != null &&
                        (bool)Session["Embedded"]);
            }
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            #region Настройка диаграммы

            //ultraChart.Width = 320;
            ultraChart.Width = 311;

            string browser = HttpContext.Current.Request.Browser.Browser;
            switch (browser)
            {
                case ("Firefox"):
                    {
                        ultraChart.Height = 232;
                        break;
                    }
                case ("AppleMAC-Safari"):
                    {
                        if (Request.UserAgent.Contains("Chrome"))
                        {
                            ultraChart.Height = 233;
                        }
                        else
                        {
                            ultraChart.Height = 229;    
                        }
                        break;
                    }
                default :
                    {
                        ultraChart.Height = 232;
                        break;
                    }
            }
            
            ultraChart.ChartType = ChartType.StackColumnChart;
            ultraChart.Data.SwapRowsAndColumns = true;
            ultraChart.Axis.Y.Extent = 35;
            ultraChart.Axis.X.Extent = 60;
            ultraChart.Legend.SpanPercentage = 12;
            ultraChart.Legend.Margins.Right = 0;
            ultraChart.TitleLeft.Text = "Млн. руб.";
            ultraChart.TitleLeft.Margins.Bottom = Convert.ToInt32(ultraChart.Height.Value / 3);
            ultraChart.TitleLeft.VerticalAlign = System.Drawing.StringAlignment.Near;
            ultraChart.Tooltips.FormatString = "<SERIES_LABEL>\n<ITEM_LABEL>\n<DATA_VALUE:N3> млн.руб.";
            ultraChart.ChartDrawItem += new ChartDrawItemEventHandler(ultraChart_ChartDrawItem);

            if (EmbeddedReport)
            {
                //ultraChart.Width = 410;
                ultraChart.Height = 280;
                ultraChart.Tooltips.Font.Name = "Arial";
                ultraChart.Tooltips.Font.Size = 9;
                ultraChart.Legend.Font = new Font("Arial", 9);
                ultraChart.Legend.FontColor = Color.FromArgb(85, 85, 85);
                ultraChart.Legend.BackgroundColor = Color.White;
                ultraChart.Legend.Margins.Left = -5;
                ultraChart.Legend.BorderThickness = 0;
                ultraChart.Legend.SpanPercentage = 24;
               // ultraChart.Tooltips.FormatString = "<ITEM_LABEL><br/><DATA_VALUE_ITEM:P2>";
               // HyperLink1.Target = "_blank";
               // HyperLink2.Target = "_blank";

                ultraChart.Axis.Y.Labels.Font = new Font("Arial", 9);
                ultraChart.Axis.Y.Labels.FontColor = Color.FromArgb(85, 85, 85);
                ultraChart.Axis.Y.Labels.SeriesLabels.Font = new Font("Arial", 9);
                ultraChart.Axis.Y.Labels.SeriesLabels.FontColor = Color.FromArgb(85, 85, 85);
                
                ultraChart.Axis.X.Labels.SeriesLabels.Font = new Font("Arial", 9);
                ultraChart.Axis.X.Labels.SeriesLabels.FontColor = Color.FromArgb(85, 85, 85);
               // ultraChart.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
                ultraChart.Axis.X.Labels.SeriesLabels.FontSizeBestFit = false;
                ultraChart.Axis.X.Labels.SeriesLabels.VerticalAlign = StringAlignment.Center;
                ultraChart.Axis.X.Extent = 67;

                ultraChart.TitleLeft.Font = new Font("Arial", 9);
                ultraChart.TitleLeft.FontColor = Color.FromArgb(85, 85, 85);
            }

            ultraChart.DataBind();

            #endregion
        }

        void ultraChart_ChartDrawItem(object sender, ChartDrawItemEventArgs e)
        {
            //устанавливаем ширину текста легенды 
            Text text = e.Primitive as Text;
            if ((text != null) && !(string.IsNullOrEmpty(text.Path)) && text.Path.EndsWith("Legend"))
            {
                int widthLegendLabel;

                if ((ultraChart.Legend.Location == LegendLocation.Top) || (ultraChart.Legend.Location == LegendLocation.Bottom))
                {
                    widthLegendLabel = (int)ultraChart.Width.Value + 20;
                }
                else
                {
                    widthLegendLabel = (ultraChart.Legend.SpanPercentage * (int)ultraChart.Width.Value / 100) - 20;
                }

                widthLegendLabel -= ultraChart.Legend.Margins.Left + ultraChart.Legend.Margins.Right;
                if (text.labelStyle.Trimming != StringTrimming.None)
                {
                    text.bounds.Width = widthLegendLabel;
                }
            }
            Box box = e.Primitive as Box;
            if ((box != null) && !(string.IsNullOrEmpty(box.Path)) && box.Path.EndsWith("Legend") && (box.rect.Width != box.rect.Height))
            {
                box.rect.X = 10;
                box.rect.Width = box.rect.Width + 15;
            }
        }

        protected void ultraChart_DataBinding(object sender, EventArgs e)
        {
            CustomReportPage dashboard = GetCustomReportPage(this);

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("MFRF_0001_0004_date", Server.MapPath("~/reports/DashboardFederal/"));
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            int year = 2011;
            dashboard.UserParams.PeriodYear.Value = year.ToString();
            dashboard.UserParams.PeriodLastYear.Value = (year - 1).ToString();

            Label1.Text = string.Format("данные на {0} год и плановый период", year);

            query = DataProvider.GetQueryText("MFRF_0001_0004", Server.MapPath("~/reports/DashboardFederal/"));
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Series", dataTable);

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                for (int j = 1; j < dataTable.Columns.Count; j++)
                {
                    dataTable.Columns[j].ColumnName = dataTable.Columns[j].ColumnName.TrimEnd('_');
                    dataTable.Columns[j].ColumnName = dataTable.Columns[j].ColumnName.Replace("br", "\n");
                    if (dataTable.Rows[i][j] != DBNull.Value)
                    {
                        dataTable.Rows[i][j] = Convert.ToDouble(dataTable.Rows[i][j]) / 1000;
                    }
                }
            }

            ultraChart.DataSource = dataTable;
        }

        #region IWebPart Members

        public override string Description
        {
            get { return "Раздел содержит данные с сайта Министерства финансов РФ по межбюджетным трансфертам, перечисляемым из Федерального бюджета в бюджеты субъектов РФ"; }
        }

        public override string Title
        {
            get { return "Федеральные фонды"; }
        }

        public override string TitleUrl
        {
            get { return "~/reports/MFRF_0001_0004/Default_FF.aspx"; }
        }

        #endregion
    }
}