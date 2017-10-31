using System;
using System.Data;
using System.Drawing;
using Infragistics.UltraGauge.Resources;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Core;

namespace Krista.FM.Server.Dashboards.reports.Dashboard
{
    public partial class FK_0001_0003_Gadget : GadgetControlBase
    {
        private string populationDate;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            CustomReportPage dashboard = GetCustomReportPage(this);

            DateTime currentDate = CubeInfo.GetLastDate(DataProvidersFactory.SecondaryMASDataProvider, "FK_0001_0003_gadget_date");

            dashboard.UserParams.PeriodDayFK.Value = CRHelper.PeriodMemberUName("[Период].[Период]", currentDate, 4);
            dashboard.UserParams.PeriodLastYear.Value = (currentDate.Year - 1).ToString();
            dashboard.UserParams.PeriodYear.Value = currentDate.Year.ToString();

            populationDate = DataDictionariesHelper.GetFederalPopulationDate();

            if (!Page.IsPostBack)
            {
                Label1_1.Text = String.Format("Исполнено:");
                Label2_1.Text = String.Format("Исполнено, тыс.руб.: ");
                Label3_1.Text = String.Format("Назначено, тыс.руб.: ");
                Label4_1.Text = String.Format("Бюджетные расходы на душу населения, тыс.руб./чел.:");
                Label5_1.Text = String.Format("Численность постоянного");
                Label5_2.Text = String.Format("населения, тыс.чел.<br/>({0}):", populationDate);
                avgRFLabel.Text = "РФ";

                Label1.Text = string.Empty;
                Label2.Text = string.Empty;
                Label3.Text = string.Empty;
                Label4.Text = string.Empty;

                Rank1.Text = string.Empty;
                Rank2.Text = string.Empty;

                ((RadialGauge)Gauge.Gauges[0]).Scales[0].Axis.SetEndValue(0);
                ((RadialGauge)Gauge.Gauges[0]).Scales[0].Markers[0].Value = 0;
            }

            topLabel.Text = string.Format("данные за {0} {1} {2} года", currentDate.Month, CRHelper.RusManyMonthGenitive(currentDate.Month), currentDate.Year);

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("FK_0001_0003_gadget_grid");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Dummy", dt);

            foreach (DataRow row in dt.Rows)
            {
                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    if ((i == 1 || i == 2 || i == 7) && row[i] != DBNull.Value)
                    {
                        row[i] = (Convert.ToDouble(row[i]) / 1000);
                    }
                }
            }

            if (dt.Rows[0][0] != DBNull.Value)
            {
                Label1.Text = Convert.ToDouble(dt.Rows[0][0]).ToString("P2");
            }
            if (dt.Rows[0][1] != DBNull.Value)
            {
                Label2.Text = Convert.ToDouble(dt.Rows[0][1]).ToString("N3");
            }
            if (dt.Rows[0][2] != DBNull.Value)
            {
                Label3.Text = Convert.ToDouble(dt.Rows[0][2]).ToString("N3");
            }
            if (dt.Rows[0][7] != DBNull.Value)
            {
                Label4.Text = Convert.ToDouble(dt.Rows[0][7]).ToString("N3");
            }
            if (dt.Rows[0][14] != DBNull.Value)
            {
                Label5.Text = Convert.ToDouble(dt.Rows[0][14]).ToString("N3");
            }

            Rank1.Text = string.Format("{0}&nbsp;&nbsp;{1}",
                GetRankString(dt, 0, 3, 4, RegionsNamingHelper.ShortName(dashboard.UserParams.Region.Value)),
                GetRankString(dt, 0, 5, 6, "РФ"));

            Rank2.Text = string.Format("{0}&nbsp;&nbsp;{1}",
                GetRankString(dt, 0, 8, 9, RegionsNamingHelper.ShortName(dashboard.UserParams.Region.Value)),
                GetRankString(dt, 0, 10, 11, "РФ"));

            if (dt.Rows[0][12] != DBNull.Value)
            {
                avgFO.Text = Convert.ToDouble(dt.Rows[0][12]).ToString("P2");
            }

            if (dt.Rows[0][13] != DBNull.Value)
            {
                avgRF.Text = Convert.ToDouble(dt.Rows[0][13]).ToString("P2");
                ((RadialGauge)Gauge.Gauges[0]).Scales[0].Markers[1].Value = Convert.ToDouble(dt.Rows[0][13]) * 100;
            }

            Gauge.Width = 190;
            Gauge.Height = 190;

            double minPercent = 100 * currentDate.Month / 12;
            ((RadialGauge)Gauge.Gauges[0]).Scales[0].Ranges[1].StartValue = 0;
            ((RadialGauge)Gauge.Gauges[0]).Scales[0].Ranges[1].EndValue = minPercent;
            ((RadialGauge)Gauge.Gauges[0]).Scales[0].Ranges[2].StartValue = minPercent;
            ((RadialGauge)Gauge.Gauges[0]).Scales[0].Ranges[2].EndValue = 100;

            // исполнение
            if (dt.Rows[0][0] != DBNull.Value)
            {
                double completeValue = Convert.ToDouble(dt.Rows[0][0]) * 100;
                int gaudeEndValue = Convert.ToInt32(Math.Ceiling(completeValue / 50)) * 50;
                gaudeEndValue = gaudeEndValue < 100 ? 100 : gaudeEndValue;

                ((RadialGauge)Gauge.Gauges[0]).Scales[0].Axis.SetEndValue(gaudeEndValue);
                ((RadialGauge)Gauge.Gauges[0]).Scales[0].Markers[0].Value = completeValue;

                if (completeValue >= minPercent)
                {
                    ((SimpleGradientBrushElement)((RadialGauge)Gauge.Gauges[0]).Scales[0].Markers[0].BrushElement).
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

            HyperLink1.Text = "Расходы по субъектам РФ";
            HyperLink1.NavigateUrl = "~/reports/FK_0001_0003/DefaultCompare.aspx";
            HyperLink2.Text = "Распределение по бюджетным расходам на душу населения";
            HyperLink2.NavigateUrl = "~/reports/FK_0001_0003/DefaultAllocation.aspx";
            HyperLink3.Text = string.Format("Подробнее расходы {0}", dashboard.UserParams.StateArea.Value);
            HyperLink3.NavigateUrl = "~/reports/FK_0001_0001/DefaultDetail.aspx";
            avgFOLabel.Text = string.Format("средн. {0}", RegionsNamingHelper.ShortName(dashboard.UserParams.Region.Value));
        }

        private static string GetRankString(DataTable dt, int rowIndex, int curRankColumn, int maxRankColumn, string region)
        {
            if (dt != null && dt.Rows[rowIndex][curRankColumn] != DBNull.Value && dt.Rows[rowIndex][maxRankColumn] != DBNull.Value)
            {
                string rankImg = GetRankImg(Convert.ToInt32(dt.Rows[rowIndex][curRankColumn]),
                                            Convert.ToInt32(dt.Rows[rowIndex][maxRankColumn]));
                return string.Format("ранг&nbsp;{0}&nbsp;<b>{1}</b>&nbsp;{2}",
                    region,
                    Convert.ToInt32(dt.Rows[rowIndex][curRankColumn]),
                    rankImg);
            }
            return string.Empty;
        }

        private static string GetRankImg(int curRank, int maxRank)
        {
            string img = "&nbsp;";
            if (curRank == 1)
            {
                img = "<img style=\"vertical-align:baseline\" src=\"../../images/starYellowBB.png\">";
            }
            else if (curRank == maxRank)
            {
                img = "<img style=\"vertical-align:baseline\" src=\"../../images/starGrayBB.png\">";
            }
            return img;
        }

        #region IWebPart Members

        public override string Description
        {
            get { return "Раздел содержит данные Федерального казначейства об исполнении бюджетов субъектов РФ по расходам"; }
        }

        public override string Title
        {
            get { return "Расходы"; }
        }

        public override string TitleUrl
        {
            get { return "~/reports/FK_0001_0001/DefaultDetail.aspx"; }
        }

        #endregion
    }
}