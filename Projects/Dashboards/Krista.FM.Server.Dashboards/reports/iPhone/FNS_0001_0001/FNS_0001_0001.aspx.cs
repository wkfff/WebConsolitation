using System;
using System.Data;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FNS_0001_0001 : CustomReportPage
    {
        private DataTable dtGrid;

        private bool fns28nSplitting;
        private int month;
        private int year;
        private string rubMiltiplierCaption;

        #region Параметры запроса

        // уровень МР и ГО
        private CustomParam regionsLevel;
        // куб
        private CustomParam cubeName;
        // число выбранных регионов
        private CustomParam topRegionsCount;
        // множитель рублей
        private CustomParam rubMultiplier;
        // группа КД
        private CustomParam kdGroup;

        #endregion

        private bool IsThsRubMiltiplier
        {
            get { return rubMiltiplierCaption == "тыс.руб."; }
        }


        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Инициализация параметров запроса

            if (regionsLevel == null)
            {
                regionsLevel = UserParams.CustomParam("regions_level");
            }
            if (cubeName == null)
            {
                cubeName = UserParams.CustomParam("cube_name");
            }
            if (topRegionsCount == null)
            {
                topRegionsCount = UserParams.CustomParam("top_regions_count");
            }
            rubMultiplier = UserParams.CustomParam("rub_multiplier");
            kdGroup = UserParams.CustomParam("kd_group");

            #endregion
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            fns28nSplitting = Convert.ToBoolean(RegionSettingsHelper.Instance.FNS28nSplitting);
            UserParams.IncomesKDRootName.Value = RegionSettingsHelper.Instance.IncomesKDRootName;
            UserParams.IncomesKDSocialNeedsTax.Value = RegionSettingsHelper.Instance.IncomesKDSocialNeedsTax;
            UserParams.IncomesKDReturnOfRemains.Value = RegionSettingsHelper.Instance.IncomesKDReturnOfRemains;
            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.IncomesKDAllLevel.Value = RegionSettingsHelper.Instance.IncomesKDAllLevel;
            UserParams.IncomesKD11800000000000000.Value = RegionSettingsHelper.Instance.IncomesKD11800000000000000;
            UserParams.IncomesKD11700000000000000.Value = RegionSettingsHelper.Instance.IncomesKD11700000000000000;
            UserParams.IncomesKDDimension.Value = RegionSettingsHelper.Instance.IncomesKDDimension;

            cubeName.Value = fns28nSplitting ? "ФНС_28н_с расщеплением" : "ФНС_28н_без расщепления";

            kdGroup.Value = RegionSettingsHelper.Instance.GetPropertyValue("KDGroup");
            rubMiltiplierCaption = RegionSettingsHelper.Instance.GetPropertyValue("RubItemType");
            thsRubLabel.Text = String.Format("({0})", rubMiltiplierCaption);
            rubMultiplier.Value = IsThsRubMiltiplier ? "1000" : "1000000";

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FNS_0001_0001_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            month = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());
            year = Convert.ToInt32(dtDate.Rows[0][0].ToString());

            UserParams.PeriodLastYear.Value = string.Format("[{0}].[Полугодие 2].[Квартал 4].[Декабрь]", year - 1);
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(month));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(month));
            UserParams.PeriodYear.Value = string.Format("[{0}].[{1}].[{2}].[{3}]", year,
                UserParams.PeriodHalfYear.Value, UserParams.PeriodQuater.Value, CRHelper.RusMonth(month));
            topRegionsCount.Value = "5";
            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;

            UltraWebGrid.DataBind();
            UltraWebGrid.Height = Unit.Empty;
        }

        #region Методы получения значений DataTable

//        private static string ParseDTValue(DataTable dt, int rowIndex, int columnIndex)
//        {
//            if (dt == null || dt.Rows[rowIndex][columnIndex] == DBNull.Value)
//            {
//                return string.Empty;
//            }
//
//            return dt.Rows[rowIndex][columnIndex].ToString();
//        }

        private static string ParseDTValue(DataRow row, int columnIndex)
        {
            if (row == null || row[columnIndex] == DBNull.Value)
            {
                return string.Empty;
            }

            return row[columnIndex].ToString();
        }

        private static string ParseDoubleDTValue(DataTable dt, int rowIndex, int columnIndex, string format)
        {
            if (dt == null || dt.Rows[rowIndex][columnIndex] == DBNull.Value)
            {
                return string.Empty;
            }

            return Convert.ToDouble(dt.Rows[rowIndex][columnIndex]).ToString(format);
        }

        private static string ParseDoubleDTValue(DataRow row, int columnIndex, string format)
        {
            if (row == null || row[columnIndex] == DBNull.Value)
            {
                return string.Empty;
            }

            return Convert.ToDouble(row[columnIndex]).ToString(format); 
        }

        #endregion

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            DataTable dtTop = new DataTable();
            string queryName = (fns28nSplitting) ? "FNS_0001_0001_top5_split" : "FNS_0001_0001_top5";
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Top", dtTop);

            string top5 = string.Empty;
            foreach (DataRow row in dtTop.Rows)
            {
                string rowStr = (row[0] == DBNull.Value) ? string.Empty : row[0].ToString();
                if (rowStr.Length < 30)
                {
                    rowStr = rowStr.Replace(" ", "&nbsp;");
                }
                top5 += string.Format("{0}, ", rowStr);
            }

            top5 = top5.TrimEnd(' ');
            top5 = top5.TrimEnd(',');
            top5 = top5.Replace("Городской округ", "ГО");
            top5 = top5.Replace("городской округ", "ГО"); 
            top5 = top5.Replace("муниципальное образование", "МО");
            top5 = top5.Replace("муниципальный район", "МР");
            top5 = top5.Replace("Муниципальный район", "МР");
            top5Label.Text = string.Format("Наибольшая недоимка по налоговым доходам:&nbsp;{0}", top5);

            if (month == 12)
            {
                month = 1;
                year++;
            }
            else
            {
                month++;
            }
            string prevMonth = string.Format("Недоимка на 01.{0:00}.{1}", month, year);

            DataTable dt = new DataTable();
            DataColumn col = new DataColumn("Недоимка на начало года", typeof (string));
            dt.Columns.Add(col);
            col = new DataColumn(prevMonth, typeof (string));
            dt.Columns.Add(col);
            col = new DataColumn("Прирост недоимки", typeof (string));
            dt.Columns.Add(col);
            
            dtGrid = new DataTable();
            queryName = (fns28nSplitting) ? "FNS_0001_0001_split" : "FNS_0001_0001";
            query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Район (ГО)", dtGrid);

            string cellFormat = IsThsRubMiltiplier ? "N1" : "N3";

            if (fns28nSplitting)
            {
                foreach (DataRow row in dtGrid.Rows)
                {
                    DataRow r = dt.NewRow();
                    r[0] = ParseDTValue(row, 0);
                    dt.Rows.Add(r);
                    r = dt.NewRow();
                    r[0] = string.Format("{0}", ParseDoubleDTValue(row, 1, cellFormat));
                    r[1] = string.Format("{0}", ParseDoubleDTValue(row, 2, cellFormat));
                    r[2] = string.Format("<span style=\"color:#d1d1d1;\">{0}<br />{1}</span>",
                        ParseDoubleDTValue(row, 3, cellFormat),
                        ParseDoubleDTValue(row, 4, "P2"));
                    dt.Rows.Add(r);
                }
            }
            else
            {
                DataRow r = dt.NewRow();
                r[0] = string.Format("{0}", ParseDoubleDTValue(dtGrid, 0, 0, cellFormat));
                r[1] = string.Format("{0}", ParseDoubleDTValue(dtGrid, 0, 1, cellFormat));
                r[2] = string.Format("<span style=\"color:#d1d1d1;\">{0}<br />{1}</span>",
                    ParseDoubleDTValue(dtGrid, 0, 2, cellFormat),
                    ParseDoubleDTValue(dtGrid, 0, 3, "P2"));
                dt.Rows.Add(r);
            }

            UltraWebGrid.DataSource = dt.DefaultView;
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;

            if (e.Layout.Bands[0].Columns.Count == 3)
            {
                e.Layout.Bands[0].Columns[0].Width = 104;
                e.Layout.Bands[0].Columns[1].Width = 104;
                e.Layout.Bands[0].Columns[2].Width = 103;

                e.Layout.Bands[0].Columns[0].CellStyle.Font.Bold = true;
                e.Layout.Bands[0].Columns[1].CellStyle.Font.Bold = true;
                e.Layout.Bands[0].Columns[2].CellStyle.Font.Bold = true;
            }
        }

        protected void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            e.Row.Cells[0].Style.BorderDetails.WidthLeft = 2;
            e.Row.Cells[1].Style.BorderDetails.WidthLeft = 2;
            e.Row.Cells[2].Style.BorderDetails.WidthLeft = 2;

            e.Row.Cells[0].Style.BorderDetails.WidthRight = 2;
            e.Row.Cells[1].Style.BorderDetails.WidthRight = 2;
            e.Row.Cells[2].Style.BorderDetails.WidthRight = 2;

            if (fns28nSplitting && (e.Row.Index == 0 || e.Row.Index == 2 || e.Row.Index == 4))
            {
                e.Row.Cells[0].ColSpan = 3;
                e.Row.Cells[0].Style.Font.Size = 11;

                e.Row.Cells[0].Style.HorizontalAlign = HorizontalAlign.Left;
                e.Row.Cells[0].Style.BorderDetails.WidthBottom = 0;
                e.Row.Cells[1].Style.BorderDetails.WidthBottom = 0;
                e.Row.Cells[2].Style.BorderDetails.WidthBottom = 0;

                e.Row.Cells[0].Style.Padding.Bottom = 0;
                e.Row.Cells[1].Style.Padding.Bottom = 0;
                e.Row.Cells[2].Style.Padding.Bottom = 0;

                e.Row.Cells[0].Style.Padding.Top = 0;
                e.Row.Cells[1].Style.Padding.Top = 0;
                e.Row.Cells[2].Style.Padding.Top = 0;
            }
            else
            {
                int rowIndex = 0;
                int percentColumnIndex = 3;
                int indincateColumnIndex = 2;

                if (fns28nSplitting)
                {
                    e.Row.Cells[0].Style.BorderDetails.WidthTop = 0;
                    e.Row.Cells[1].Style.BorderDetails.WidthTop = 0;
                    e.Row.Cells[2].Style.BorderDetails.WidthTop = 0;

                    e.Row.Cells[0].Style.BorderDetails.WidthBottom = 2;
                    e.Row.Cells[1].Style.BorderDetails.WidthBottom = 2;
                    e.Row.Cells[2].Style.BorderDetails.WidthBottom = 2;

                    rowIndex = (e.Row.Index - 1) / 2;
                    percentColumnIndex = 4;
                    indincateColumnIndex = 2;
                }
                else
                {
                    e.Row.Cells[0].Style.Font.Size = 12;
                    e.Row.Cells[1].Style.Font.Size = 12;
                }

                if (dtGrid.Rows[rowIndex][percentColumnIndex] != DBNull.Value &&
                     dtGrid.Rows[rowIndex][percentColumnIndex].ToString() != string.Empty)
                {
                    double value = Convert.ToDouble(dtGrid.Rows[rowIndex][percentColumnIndex]);
                    e.Row.Cells[indincateColumnIndex].Style.BackgroundImage = (value > 0)
                                                               ? "../../../images/arrowUpRed.png"
                                                               : "../../../images/arrowDownGreen.png";
                    e.Row.Cells[indincateColumnIndex].Style.CustomRules =
                        "background-repeat: no-repeat; background-position: left center; padding-top: 2px";
                }
            }
        }

        #endregion
    }
}
