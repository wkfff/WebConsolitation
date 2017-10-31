using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FNS_0001_0001_H : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtRegions = new DataTable();
        private DataTable dtKD = new DataTable();
        private DataTable dtOKVD = new DataTable();

        private bool fns28nSplitting;
        private int month;
        private int year;
        private string rubMiltiplierCaption;

        #region Параметры запроса

        // произвольный фильтр
        private CustomParam customFilter;
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

            if (customFilter == null)
            {
                customFilter = UserParams.CustomParam("custom_filter");
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

            BudgetLevelCaption.Text = "Недоимка по налоговым доходным источникам";
            thsRubLabel.Text = "(тыс.руб.)";

            regionsGrid.Caption = "Регион";
            regionsGrid.InitializeRow += new InitializeRowEventHandler(regionsGrid_InitializeRow);
            KDGrid.Caption = "КД";
            KDGrid.InitializeRow += new InitializeRowEventHandler(regionsGrid_InitializeRow);
            OKVDGrid.Caption = "ОКВЭД";
            OKVDGrid.InitializeRow += new InitializeRowEventHandler(regionsGrid_InitializeRow);
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

            rubMiltiplierCaption = RegionSettingsHelper.Instance.GetPropertyValue("RubItemType");
            thsRubLabel.Text = String.Format("({0})", rubMiltiplierCaption);
            rubMultiplier.Value = IsThsRubMiltiplier ? "1000" : "1000000";
            kdGroup.Value = RegionSettingsHelper.Instance.GetPropertyValue("KDGroup").Replace("[КД].[Сопоставимый]", RegionSettingsHelper.Instance.IncomesKDDimension);

            string query = DataProvider.GetQueryText("FNS_0001_0001_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
            year = Convert.ToInt32(dtDate.Rows[0][0]);
            month = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());

            UserParams.PeriodLastYear.Value = string.Format("[{0}].[Полугодие 2].[Квартал 4].[Декабрь]", year - 1);
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(month));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(month));
            UserParams.PeriodYear.Value = string.Format("[{0}].[{1}].[{2}].[{3}]", year,
                UserParams.PeriodHalfYear.Value, UserParams.PeriodQuater.Value, CRHelper.RusMonth(month));
            topRegionsCount.Value = "5";

            month = month + 1;
            if (month > 12)
            {
                month = 1;
                year++;
            }

            TableRow row = new TableRow();
            TableCell cell = new TableCell();
            Label label1 = new Label();
            label1.CssClass = "ServeText";
            label1.Text = string.Format("данные на 1 {0} {1} года<br/>", CRHelper.RusMonthGenitive(month), year);
            cell.Controls.Add(label1);
            Label label2 = new Label();
            label2.Text =
                string.Format("обновлено {0:dd.MM.yyyy HH:mm:ss}", DateTime.Now);
            label2.CssClass = "ServeText";
            cell.Controls.Add(label2);
            row.Cells.Add(cell);
            dateTimeTable.Rows.Add(row);

            //regionsLabel.Text = "<span style=\"color:white;\"><b>5</b></span>&nbsp;территорий с наибольшей недоимкой";
            regionsLabel.Text = "территорий с наибольшей недоимкой";
            regionsBudgetLabel.Text = "(Консолидированный бюджет МО)";
            regionsGrid.DataBind();
            regionsGrid.Height = Unit.Empty;

            //OKVDLabel.Text = "<br /><span style=\"color:white;\"><b>5</b></span>&nbsp;ОКВЭД с наибольшей недоимкой";
            OKVDLabel.Text = "ОКВЭД с наибольшей недоимкой";
            OKVDBudgetLabel.Text = "(Консолидированный бюджет субъекта)";
            OKVDGrid.DataBind();
            OKVDGrid.Height = Unit.Empty;

            //KDLabel.Text = "<br /><span style=\"color:white;\"><b>5</b></span>&nbsp;доходных источников с наибольшей недоимкой";
            KDLabel.Text = "доходных источников с наибольшей недоимкой";
            KDBudgetLabel.Text = "(Консолидированный бюджет субъекта)";
            KDGrid.DataBind();
            KDGrid.Height = Unit.Empty;

            CRHelper.SetConditionImageGridCells(regionsGrid, 4, 0, CompareAction.Greater, "~/images/ArrowUpRed.gif", "~/images/ArrowDownGreen.gif");
            CRHelper.SetConditionImageGridCells(OKVDGrid, 4, 0, CompareAction.Greater, "~/images/ArrowUpRed.gif", "~/images/ArrowDownGreen.gif");
            CRHelper.SetConditionImageGridCells(KDGrid, 4, 0, CompareAction.Greater, "~/images/ArrowUpRed.gif", "~/images/ArrowDownGreen.gif");
        }

        #region Обработчики грида

        protected void OKVDGrid_DataBinding(object sender, EventArgs e)
        {
            customFilter.Value = "Descendants ([ОКВЭД].[Сопоставимый].[Все коды ОКВЭД],[ОКВЭД].[Сопоставимый].[Класс],SELF)";
            string budgetLevel = (fns28nSplitting)
                                     ? ", [Уровни бюджетов].[Все].[Все уровни].[Конс.бюджет субъекта]"
                                     : " ";
            UserParams.Filter.Value = String.Format("{0}, [Районы].[Сопоставимый].[Все районы] {1}",
                kdGroup.Value, budgetLevel);
            string queryName = (fns28nSplitting) ? "FNS_0001_0001_top5_H_split" : "FNS_0001_0001_top5_H";
            string query = DataProvider.GetQueryText(queryName);
            dtOKVD = new DataTable();
            dtOKVD.TableName = "ОКВЭД";
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, dtOKVD.TableName, dtOKVD);
            OKVDGrid.DataSource = ConvertDT(dtOKVD, dtOKVD.TableName);
        }

        protected void KDGrid_DataBinding(object sender, EventArgs e)
        {
            customFilter.Value = String.Format("Descendants ({0}.[Все коды доходов],{0}.[Статья],SELF)", UserParams.IncomesKDDimension.Value);
            string budgetLevel = (fns28nSplitting)
                         ? ", [Уровни бюджетов].[Все].[Все уровни].[Конс.бюджет субъекта]"
                         : " ";
            UserParams.Filter.Value = String.Format("[Районы].[Сопоставимый].[Все районы] {0}",
                budgetLevel);
            string queryName = (fns28nSplitting) ? "FNS_0001_0001_top5_H_split" : "FNS_0001_0001_top5_H";
            string query = DataProvider.GetQueryText(queryName);
            dtKD = new DataTable();
            dtKD.TableName = "Доходы";
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, dtKD.TableName, dtKD);
            KDGrid.DataSource = ConvertDT(dtKD, dtKD.TableName);
        }

        protected void regionsGrid_DataBinding(object sender, EventArgs e)
        {
            customFilter.Value = string.Format("Descendants ([Районы].[Сопоставимый].[Все районы],{0},SELF)", RegionSettingsHelper.Instance.RegionsLevel);
            string budgetLevel = (fns28nSplitting)
                         ? ", [Уровни бюджетов].[Все].[Все уровни].[Конс.бюджет субъекта].[Конс.бюджет МО]"
                         : " ";
            UserParams.Filter.Value = String.Format("{0} {1}",
                kdGroup.Value, budgetLevel);
            string queryName = (fns28nSplitting) ? "FNS_0001_0001_top5_H_split" : "FNS_0001_0001_top5_H";
            string query = DataProvider.GetQueryText(queryName);
            dtRegions = new DataTable();
            dtRegions.TableName = "Район (ГО)";
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, dtRegions.TableName, dtRegions);
            regionsGrid.DataSource = ConvertDT(dtRegions, dtRegions.TableName);
        }

        private DataTable ConvertDT(DataTable dtSource, string title)
        {
            DataTable dt = new DataTable();

            string prevMonth = string.Format("Недоимка на 01.{0:00}.{1}", month, year);

            DataColumn col = new DataColumn(title, typeof(string));
            dt.Columns.Add(col);
            col = new DataColumn("Недоимка на начало года", typeof(string));
            dt.Columns.Add(col);
            col = new DataColumn(prevMonth, typeof(string));
            dt.Columns.Add(col);
            col = new DataColumn("Прирост недоимки", typeof(string));
            dt.Columns.Add(col);

            string cellFormat = IsThsRubMiltiplier ? "N1" : "N3";

            foreach (DataRow row in dtSource.Rows)
            {
                DataRow r = dt.NewRow();
                r[0] = ParseDTValue(row, 0);
                if (r[0].ToString() == "Налоги на имущество")
                {
                    r[0] += " (задолженности и перерасчеты)";
                }
                                
                r[1] = ParseDoubleDTValue(row, 1, cellFormat);
                r[2] = ParseDoubleDTValue(row, 2, cellFormat);
                r[3] = string.Format("<span style=\"color:#d1d1d1;\">{0}<br />{1}</span>",
                    ParseDoubleDTValue(row, 3, cellFormat),
                    ParseDoubleDTValue(row, 4, "P2"));
                dt.Rows.Add(r);
            }

            return dt;
        }

        #region Методы получения значений DataTable

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

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;

            if (e.Layout.Bands.Count == 0)
                return;

            if (e.Layout.Bands[0].Columns.Count == 4)
            {
                e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
                e.Layout.Bands[0].Columns[0].AllowGroupBy = AllowGroupBy.No;
                e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

                for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    e.Layout.Bands[0].Columns[i].CellStyle.Font.Bold = true;
                }

                e.Layout.Bands[0].Columns[0].Width = 187;
                e.Layout.Bands[0].Columns[1].Width = 91;
                e.Layout.Bands[0].Columns[2].Width = 95;
                e.Layout.Bands[0].Columns[3].Width = 95;
            }
        }

        void regionsGrid_InitializeRow(object sender, RowEventArgs e)
        {
            DataTable dt = null;
            switch (e.Row.Band.Grid.Caption)
            {
                case "Регион":
                    {
                        dt = dtRegions;
                        break;
                    }
                case "КД":
                    {
                        dt = dtKD;
                        break;
                    }
                case "ОКВЭД":
                    {
                        dt = dtOKVD;
                        break;
                    }
            }

            if (dt != null && dt.Rows.Count > 0)
            {
                if (dt.Rows[e.Row.Index][4] != DBNull.Value &&
                    dt.Rows[e.Row.Index][4].ToString() != string.Empty)
                {
                    double value = Convert.ToDouble(dt.Rows[e.Row.Index][4]);
                    e.Row.Cells[3].Style.BackgroundImage = (value > 0)
                        ? "../../../images/arrowUpRed.png"
                        : "../../../images/arrowDownGreen.png";
                    e.Row.Cells[3].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; padding-top: 2px";
                }
            }
        }

        #endregion
    }
}
