using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.UltraGauge.Resources;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Core;

namespace Krista.FM.Server.Dashboards.reports.Dashboard_NAO
{
    public partial class FK_0001_0002_Gadget : GadgetControlBase
    {
        private string populationDate;

        private double gaugeSizeMultiplier;
        private int fontSizeMultiplier;
        private bool onWall;

        string labelCssClass;
        string valueCssClass;
        string greatLabelCssClass;
        string greatValueCssClass;

        private void SetScaleSizes()
        {
            Gauge.Width = Convert.ToInt32(260 * gaugeSizeMultiplier);
            Gauge.Height = Convert.ToInt32(260 * gaugeSizeMultiplier);

            int scaled8FontSize = 7 * fontSizeMultiplier;
            int scaled10FontSize = 10 * fontSizeMultiplier;

            labelCssClass = onWall ? "WallGadgetLabelText" : "GadgetLabelText";
            valueCssClass = onWall ? "WallGadgetValueText" : "GadgetValueText";

            greatLabelCssClass = onWall ? "WallGadgetGreatLabelText" : "GadgetGreatLabelText";
            greatValueCssClass = onWall ? "WallGadgetGreatValueText" : "GadgetGreatValueText";

            ExecutePercentLabel.CssClass = greatLabelCssClass;
            ExecuteLabel.CssClass = greatLabelCssClass;
            AssignLabel.CssClass = greatLabelCssClass;
            ExecuteAvgLabel.CssClass = labelCssClass;
            PopulationLabel.CssClass = labelCssClass;
            ExecuteAvgLabel.CssClass = labelCssClass;
            AvgLabel.CssClass = labelCssClass;
            AvgFOLabel.CssClass = labelCssClass;
            AvgRFLabel.CssClass = labelCssClass;

            ExecutePercentValue.CssClass = greatValueCssClass;
            AssignValue.CssClass = greatValueCssClass;
            ExecuteValue.CssClass = greatValueCssClass;
            PopulationValue.CssClass = valueCssClass;
            ExecuteAvgValue.CssClass = valueCssClass;

            AvgFO.CssClass = valueCssClass;
            AvgRF.CssClass = valueCssClass;

            PercentRank.CssClass = labelCssClass;
            AvgRank.CssClass = labelCssClass;
            
            GadgetTitle.Font.Size = scaled10FontSize;
            GadgetSubTitle.Font.Size = scaled8FontSize;

            ((RadialGauge)Gauge.Gauges[0]).Scales[0].Labels.Font = new Font("Verdana", Convert.ToInt32(7 * fontSizeMultiplier));
            ((RadialGauge)Gauge.Gauges[0]).Scales[0].Markers[1].StrokeElement.Thickness = 2 * gaugeSizeMultiplier;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            onWall = false;
            if (Session["onWall"] != null)
            {
                onWall = Convert.ToBoolean(((CustomParam)Session["onWall"]).Value);
            }
            
            gaugeSizeMultiplier = onWall ? 4 : 1;
            fontSizeMultiplier = onWall ? 8 : 2;

            IndicatorTable.Width = onWall ? "2550px" : "600px";
            IndicatorTable.Height = onWall ? "1500px" : "490px";

            CustomReportPage dashboard = GetCustomReportPage(this);

            SetScaleSizes();

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FK_0001_0002_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            dashboard.UserParams.PeriodDayFK.Value = dtDate.Rows[0][4].ToString();
            dashboard.UserParams.PeriodLastYear.Value = (Convert.ToInt32(dtDate.Rows[0][0]) - 1).ToString();
            dashboard.UserParams.PeriodYear.Value = dtDate.Rows[0][0].ToString();

            populationDate = DataDictionariesHelper.GetFederalPopulationDate();

            if (!Page.IsPostBack)
            {
                ExecutePercentLabel.Text = String.Format("Исполнено: ");
                ExecuteLabel.Text = String.Format("Исполнено, тыс.руб.: ");
                AssignLabel.Text = String.Format("Назначено, тыс.руб.: ");
                ExecuteAvgLabel.Text = String.Format("Среднедушевые доходы, тыс.руб./чел.: ");
                PopulationLabel.Text = String.Format("Численность пост.населения, тыс.чел. ({0}): ", populationDate);
                AvgLabel.Text = "среднее значение по";
                AvgRFLabel.Text = "РФ&nbsp;";

                ExecutePercentValue.Text = string.Empty;
                AssignValue.Text = string.Empty;
                ExecuteValue.Text = string.Empty;
                PopulationValue.Text = string.Empty;

                PercentRank.Text = string.Empty;
                AvgRank.Text = string.Empty;

                ((RadialGauge)Gauge.Gauges[0]).Scales[0].Axis.SetEndValue(0);
                ((RadialGauge)Gauge.Gauges[0]).Scales[0].Markers[0].Value = 0;
            }
            
            int monthNum = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());
            int yearNum = Convert.ToInt32(dtDate.Rows[0][0]);

            topLabel.Text = String.Empty;
            GadgetTitle.Text = "Доходы";
            GadgetSubTitle.Text = String.Format("за {0} {1} {2} года", monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum);

            DataTable dt = new DataTable();
            query = DataProvider.GetQueryText("FK_0001_0002");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Dummy", dt);

            foreach (DataRow row in dt.Rows)
            {
                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    if ((i == 1 || i == 2 || i == 7) && row[i] != DBNull.Value)
                    {
                        row[i] = (Convert.ToDouble(row[i])/1000);
                    }
                }
            }
            
            if (dt.Rows[0][0] != DBNull.Value)
            {
                ExecutePercentValue.Text = Convert.ToDouble(dt.Rows[0][0]).ToString("P2");
            }
            if (dt.Rows[0][1] != DBNull.Value)
            {
                ExecuteValue.Text = Convert.ToDouble(dt.Rows[0][1]).ToString("N3");
            }
            if (dt.Rows[0][2] != DBNull.Value)
            {
                AssignValue.Text = Convert.ToDouble(dt.Rows[0][2]).ToString("N3");
            }
            if (dt.Rows[0][7] != DBNull.Value)
            {
                ExecuteAvgValue.Text = Convert.ToDouble(dt.Rows[0][7]).ToString("N3");
            }
            if (dt.Rows[0][14] != DBNull.Value)
            {
                PopulationValue.Text = Convert.ToDouble(dt.Rows[0][14]).ToString("N3");
            }

            PercentRank.Text = string.Format("{0}&nbsp;&nbsp;{1}",
                GetRankString(dt, 0, 3, 4, RegionsNamingHelper.ShortName(dashboard.UserParams.Region.Value)),
                GetRankString(dt, 0, 5, 6, "РФ"));

            AvgRank.Text = string.Format("{0}&nbsp;&nbsp;{1}",
                GetRankString(dt, 0, 8, 9, RegionsNamingHelper.ShortName(dashboard.UserParams.Region.Value)),
                GetRankString(dt, 0, 10, 11, "РФ"));

            if (dt.Rows[0][12] != DBNull.Value)
            {
                AvgFO.Text = Convert.ToDouble(dt.Rows[0][12]).ToString("P2") + "&nbsp;";
            }

            if (dt.Rows[0][13] != DBNull.Value)
            {
                AvgRF.Text = Convert.ToDouble(dt.Rows[0][13]).ToString("P2") + "&nbsp;";
                ((RadialGauge)Gauge.Gauges[0]).Scales[0].Markers[1].Value = Convert.ToDouble(dt.Rows[0][13]) * 100;
            }

            double minPercent = 100 * monthNum / 12;
            ((RadialGauge)Gauge.Gauges[0]).Scales[0].Ranges[1].StartValue = 0;
            ((RadialGauge)Gauge.Gauges[0]).Scales[0].Ranges[1].EndValue = minPercent;
            ((RadialGauge)Gauge.Gauges[0]).Scales[0].Ranges[2].StartValue = minPercent;
            ((RadialGauge)Gauge.Gauges[0]).Scales[0].Ranges[2].EndValue = 100;
            
            // исполнение
            if (dt.Rows[0][0] != DBNull.Value)
            {
                double completeValue = Convert.ToDouble(dt.Rows[0][0]) * 100;
                int gaudeEndValue = Convert.ToInt32(Math.Ceiling(completeValue/50))*50;
                gaudeEndValue = gaudeEndValue < 100 ? 100 : gaudeEndValue;

                ((RadialGauge) Gauge.Gauges[0]).Scales[0].Axis.SetEndValue(gaudeEndValue);
                ((RadialGauge) Gauge.Gauges[0]).Scales[0].Markers[0].Value = completeValue;

                if (completeValue >= minPercent)
                {
                    ((SimpleGradientBrushElement) ((RadialGauge) Gauge.Gauges[0]).Scales[0].Markers[0].BrushElement).
                        StartColor = Color.LimeGreen;
                    ((SimpleGradientBrushElement)((RadialGauge)Gauge.Gauges[0]).Scales[0].Markers[0].BrushElement).
                        EndColor = Color.DarkGreen;
                }
                else
                {
                    ((SimpleGradientBrushElement)((RadialGauge)Gauge.Gauges[0]).Scales[0].Markers[0].BrushElement).
                        StartColor = Color.Red;
                    ((SimpleGradientBrushElement)((RadialGauge)Gauge.Gauges[0]).Scales[0].Markers[0].BrushElement).
                        EndColor = Color.Firebrick;
                }
            }
            else
            {
                ((RadialGauge)Gauge.Gauges[0]).Scales[0].Markers[0].Value = 0;
                ((SimpleGradientBrushElement)((RadialGauge)Gauge.Gauges[0]).Scales[0].Markers[0].BrushElement).
                        StartColor = Color.Transparent;
                ((SimpleGradientBrushElement)((RadialGauge)Gauge.Gauges[0]).Scales[0].Markers[0].BrushElement).
                    EndColor = Color.Transparent;
            }

            AvgFOLabel.Text = string.Format("{0}&nbsp;", RegionsNamingHelper.ShortName(dashboard.UserParams.Region.Value));
        }

        private string GetRankString(DataTable dt, int rowIndex, int curRankColumn, int maxRankColumn, string region)
        {
            if (dt != null && dt.Rows[rowIndex][curRankColumn] != DBNull.Value && dt.Rows[rowIndex][maxRankColumn] != DBNull.Value)
            {
                string rankImg = GetRankImg(Convert.ToInt32(dt.Rows[rowIndex][curRankColumn]), 
                                            Convert.ToInt32(dt.Rows[rowIndex][maxRankColumn]));
                return String.Format("ранг&nbsp;{0}&nbsp;<span class=\"{3}\">{1}</span>&nbsp;{2}",
                    region, 
                    Convert.ToInt32(dt.Rows[rowIndex][curRankColumn]), 
                    rankImg,
                    valueCssClass);
            }
            return string.Empty;
        }

        private string GetRankImg(int curRank, int maxRank)
        {
            string img = "&nbsp;";
            if (curRank == 1)
            {
                img = String.Format("<img style=\"vertical-align:top\" src=\"../../images/starYellowBBLarge.png\" width=\"{0}px\" height=\"{0}px\">",
                    onWall ? 96 : 27);
            }
            else if (curRank == maxRank)
            {
                img = String.Format("<img style=\"vertical-align:top\" src=\"../../images/starGrayBBLarge.png\" width=\"{0}px\" height=\"{0}px\">",
                    onWall ? 96 : 27);
            }
            return img;
        }

        #region IWebPart Members

        public override string Description
        {
            get { return "Раздел содержит данные Федерального казначейства об исполнении бюджетов субъектов РФ по доходам"; }
        }

        public override string Title
        {
            get { return "Доходы (РФ)"; }
        }

        public override string TitleUrl
        {
            get { return "~/reports/FK_0001_0004/DefaultDetail.aspx"; }
        }

        #endregion
    }
}