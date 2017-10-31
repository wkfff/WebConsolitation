using System;
using System.Data;
using System.Drawing;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FNS_0001_0006 : CustomReportPage
    {
        private DataTable dt;

        // консолидированный элемент районов
        private CustomParam consolidateRegionElement;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            consolidateRegionElement = UserParams.CustomParam("consolidate_region_element");
            consolidateRegionElement.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.FKRDimension.Value = RegionSettingsHelper.Instance.FKRDimension;
            UserParams.IncomesKDRootName.Value = RegionSettingsHelper.Instance.IncomesKDRootName;

            #region Настройка диаграммы

            UltraChart.Width = 310;
            UltraChart.Height = 240;
            UltraChart.ColorModel.ModelStyle = ColorModels.PureRandom;
            ((GradientEffect)UltraChart.Effects.Effects[0]).Coloring = GradientColoringStyle.Lighten;

            UltraChart.ChartType = ChartType.PieChart;
            UltraChart.Data.SwapRowsAndColumns = true;
//            UltraChart.ChartType = ChartType.StackColumnChart;
//            UltraChart.StackChart.StackStyle = StackStyle.Complete;
            UltraChart.Axis.Y.Visible = false;
            UltraChart.Axis.Y.Labels.SeriesLabels.Visible = false;
            UltraChart.Axis.X.Visible = false;
            UltraChart.PieChart.OthersCategoryPercent = 0;
            UltraChart.Legend.Visible = false;

//            ChartTextAppearance appearance = new ChartTextAppearance();
//            appearance.Column = -2;
//            appearance.Row = -2;
//            appearance.VerticalAlign = StringAlignment.Center;
//            appearance.ItemFormatString = "<ITEM_LABEL>";
//            appearance.ChartTextFont = new Font("Verdana", 8);
//            appearance.Visible = true;
//            UltraChart.ColumnChart.ChartText.Add(appearance);
            UltraChart.Tooltips.FormatString = "<ITEM_LABEL>\n<DATA_VALUE:N2> млн.руб.\n<PERCENT_VALUE:N2>%";
            UltraChart.PieChart.Labels.LeaderDrawStyle = LineDrawStyle.Solid;
            UltraChart.PieChart.Labels.LeaderLineColor = Color.White;
            UltraChart.PieChart.Labels.FontColor = Color.White;
            UltraChart.PieChart.Labels.FormatString = "<ITEM_LABEL>\n<PERCENT_VALUE:N2>%";
            UltraChart.PieChart.StartAngle = 30;
            UltraChart.PieChart.RadiusFactor = 80;

            UltraChart.ColorModel.ModelStyle = ColorModels.CustomSkin;
            UltraChart.ColorModel.Skin.PEs.Clear();
            for (int i = 1; i < 7; i++)
            {
                UltraChart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(GetColorChart(i), 150));
            }

            #endregion

            DataTable dtDate = new DataTable();
            dt = new DataTable();
            string query = DataProvider.GetQueryText("FNS_0001_0006_iphone_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            int yearNum = Convert.ToInt32(dtDate.Rows[0][0]);
            string month = dtDate.Rows[0][3].ToString();
            int monthNum = CRHelper.MonthNum(month);

            string regionName = RegionSettingsHelper.Instance.ShortName.Replace("ВологдО", "ВологО");

            TitleLabel.Text = string.Format("Структура доходов {0} по 5 лучшим отраслям за {1}&nbsp;{2}&nbsp;{3}г",
                            regionName,
                            monthNum,
                            CRHelper.RusManyMonthGenitive(monthNum),
                            yearNum);

            UserParams.PeriodYear.Value = yearNum.ToString();
            UserParams.PeriodMonth.Value = month;
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(monthNum));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(monthNum));

            query = DataProvider.GetQueryText("FNS_0001_0006_iphone_chart");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "ОКВЭД", dt);

            foreach (DataColumn column in dt.Columns)
            {
                column.ColumnName = DataDictionariesHelper.GetShortOKVDName(column.ColumnName);
                column.ColumnName = column.ColumnName.Replace("Обрабатывающие производства", "Обрабатывающие\nпроизводства");
            }

            CommentTextDataBind();

            UltraChart.DataSource = dt;
            UltraChart.DataBind();
        }

        private static Color GetColorChart(int i)
        {
            switch (i)
            {
                case 1:
                    {
                        return Color.LawnGreen;
                    }
                case 2:
                    {
                        return Color.Magenta;
                    }
                case 3:
                    {
                        return Color.Gold;
                    }
                case 4:
                    {
                        return Color.Peru;
                    }
                case 5:
                    {
                        return Color.Cyan;
                    }
                case 6:
                    {
                        return Color.PeachPuff;
                    }
                case 7:
                    {
                        return Color.MediumSlateBlue;
                    }
                case 8:
                    {
                        return Color.ForestGreen;
                    }
                case 9:
                    {
                        return Color.HotPink;
                    }
            }
            return Color.White;
        }

        private void CommentTextDataBind()
        {
            string query = DataProvider.GetQueryText("FNS_0001_0006_iphone_top5");
            DataTable dtTop5 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "ОКВЭД", dtTop5);

            string bestOKVD = GetStringDTValue(dtTop5, "Лучший из лучших ОКВЭД");
            string worseOKVD = GetStringDTValue(dtTop5, "Худший из лучших ОКВЭД");
            double bestOKVDPercent = GetDoubleDTValue(dtTop5, "Доля лучшего из лучших ОКВЭД");
            double worseOKVDPercent = GetDoubleDTValue(dtTop5, "Доля худшего из лучших ОКВЭД");

            string yellowStar = "<img src=\"../../../images/starYellow.png\" width=\"15px\" height=\"15px\"/>";
            string grayStar = "<img src=\"../../../images/starGray.png\" width=\"15px\" height=\"15px\"/>";

            BestOKVDLabel.Text = String.Format("&nbsp;&nbsp;{2}&nbsp;{0:P2} - {1}", bestOKVDPercent,
                DataDictionariesHelper.GetShortOKVDName(bestOKVD), yellowStar);
            WorseOKVDLabel.Text = String.Format("&nbsp;&nbsp;{2}&nbsp;{0:P2} - {1}", worseOKVDPercent, 
                DataDictionariesHelper.GetShortOKVDName(worseOKVD), grayStar);
        }

        private static Double GetDoubleDTValue(DataTable dt, string columnName)
        {
            return GetDoubleDTValue(dt, columnName, 0);
        }

        private static Double GetDoubleDTValue(DataTable dt, string columnName, double defaultValue)
        {
            if (dt.Rows[0][columnName] != DBNull.Value && dt.Rows[0][columnName].ToString() != string.Empty)
            {
                return Convert.ToDouble(dt.Rows[0][columnName].ToString());
            }
            return defaultValue;
        }

        private static string GetStringDTValue(DataTable dt, string columnName)
        {
            if (dt.Rows[0][columnName] != DBNull.Value && dt.Rows[0][columnName].ToString() != string.Empty)
            {
                return dt.Rows[0][columnName].ToString();
            }
            return string.Empty;
        }
    }
}
