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
    public partial class FNS_0001_0006_v : CustomReportPage
    {
        private DataTable dt;
        private DateTime currentDate;

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

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FNS_0001_0006_iphone_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            int year = Convert.ToInt32(dtDate.Rows[0][0]);
            string month = dtDate.Rows[0][3].ToString();
            currentDate = new DateTime(year, CRHelper.MonthNum(month), 1);

            UserParams.PeriodYear.Value = currentDate.Year.ToString();
            UserParams.PeriodMonth.Value = month;
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(currentDate.Month));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(currentDate.Month));

            string regionName = RegionSettingsHelper.Instance.ShortName.Replace("ВологдО", "ВологО");
            Label3.Text = string.Format("Структура доходов {0} по 5 лучшим отраслям за {1}&nbsp;{2}&nbsp;{3}&nbsp;года",
                regionName, currentDate.Month, CRHelper.RusManyMonthGenitive(currentDate.Month), currentDate.Year);

            Label1.Text = string.Format("данные на 1 {0} {1} года", CRHelper.RusMonthGenitive(currentDate.AddMonths(1).Month), currentDate.AddMonths(1).Year);
            Label2.Text = string.Format("обновлено {0:dd.MM.yyyy HH:mm:ss}", DateTime.Now);

            UltraWebGrid.DataBind();
        }

        #region Обработчики грида
        
        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            dt = new DataTable();
            string query = DataProvider.GetQueryText("FNS_0001_0006_iphone_v");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "ОКВЭД", dt);

            if (dt.Rows.Count > 0)
            {
                DataTable newDT = new DataTable();
                DataColumn column = new DataColumn("ОКВЭД", typeof(string));
                newDT.Columns.Add(column);
                string factRateColumnName = String.Format("Факт на {0:dd.MM.yyyy}г., млн.руб. и темп роста", currentDate.AddMonths(1));
                column = new DataColumn(factRateColumnName, typeof(string));
                newDT.Columns.Add(column);
                column = new DataColumn("Удел. вес, %", typeof(double));
                newDT.Columns.Add(column);
                column = new DataColumn("Ранг", typeof(double));
                newDT.Columns.Add(column);
                column = new DataColumn("Худший ранг", typeof(double));
                newDT.Columns.Add(column);
                column = new DataColumn("Прирост/снижение", typeof(double));
                newDT.Columns.Add(column);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string okvdName = GetStringDTValue(dt, i, "ОКВЭД");
                    double fact = GetDoubleDTValue(dt, i, "С начала года, млн.руб.");
                    double growRate = GetDoubleDTValue(dt, i, "Темп роста к аналогичному периоду предыдущего года_С начала года");
                    bool rateIncreasing = growRate > 1;
                    double percent = GetDoubleDTValue(dt, i, "Доля");
                    double rank = GetDoubleDTValue(dt, i, "Ранг по доле");
                    double badRank = GetDoubleDTValue(dt, i, "Худший ранг по доле");

                    string growRateString = growRate == Double.MinValue ? "" : String.Format("{0:P2}", growRate);

                    DataRow newRow = newDT.NewRow();
                    newRow["ОКВЭД"] = DataDictionariesHelper.GetShortOKVDName(okvdName);
                    newRow[factRateColumnName] = String.Format("{0:N2}{2}{1}", fact, growRateString, "<br/>");
                    
                    if (percent != Double.MinValue)
                    {
                        newRow["Удел. вес, %"] = percent;
                    }

                    if (rank != Double.MinValue)
                    {
                        newRow["Ранг"] = rank;
                    }

                    if (badRank != Double.MinValue)
                    {
                        newRow["Худший ранг"] = badRank;
                    }

                    if (growRate != Double.MinValue)
                    {
                        newRow["Прирост/снижение"] = rateIncreasing;
                    }
                    newDT.Rows.Add(newRow);
                }
                newDT.AcceptChanges();

                UltraWebGrid.DataSource = newDT;
            }
        }

        private static Double GetDoubleDTValue(DataTable dt, int rowIndex, string columnName)
        {
            return GetDoubleDTValue(dt, rowIndex, columnName, Double.MinValue);
        }

        private static Double GetDoubleDTValue(DataTable dt, int rowIndex, string columnName, double defaultValue)
        {
            if (dt.Rows[rowIndex][columnName] != DBNull.Value && dt.Rows[rowIndex][columnName].ToString() != string.Empty)
            {
                return Convert.ToDouble(dt.Rows[rowIndex][columnName].ToString());
            }
            return defaultValue;
        }

        private static string GetStringDTValue(DataTable dt, int rowIndex,string columnName)
        {
            if (dt.Rows[rowIndex][columnName] != DBNull.Value && dt.Rows[rowIndex][columnName].ToString() != string.Empty)
            {
                return dt.Rows[rowIndex][columnName].ToString();
            }
            return string.Empty;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.TableLayout = TableLayout.Fixed;

            if (e.Layout.Bands.Count == 0)
                return;

            e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = FontUnit.Parse("16px");
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].HeaderStyle.Height = 5;

            e.Layout.Bands[0].Columns[0].Width = 140;
            e.Layout.Bands[0].Columns[1].Width = 105;
            e.Layout.Bands[0].Columns[2].Width = 70;

            e.Layout.Bands[0].Columns[3].Hidden = true;
            e.Layout.Bands[0].Columns[4].Hidden = true;
            e.Layout.Bands[0].Columns[5].Hidden = true;

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N1");
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                int rankColumnIndex = 3;
                int worseRankColumnIndex = 4;
                int rateIncreaseColumnIndex = 5;
                bool rateIndication = (i == 1);
                bool rankIndication = (i == 2);

                if (rankIndication && e.Row.Cells[rankColumnIndex].Value != null && e.Row.Cells[worseRankColumnIndex].Value != null &&
                    e.Row.Cells[rankColumnIndex].Value.ToString() != String.Empty && e.Row.Cells[worseRankColumnIndex].Value.ToString() != String.Empty)
                {
                    string img = string.Empty;
                    double rank = Convert.ToDouble(e.Row.Cells[rankColumnIndex].Value);
                    double worseRank = Convert.ToDouble(e.Row.Cells[worseRankColumnIndex].Value);

                    if (rank != Double.MinValue && worseRank != double.MinValue)
                    {
                        if (rank == 1)
                        {
                            img = "~/images/starYellow.png";
                        }
                        else if (rank == worseRank)
                        {
                            img = "~/images/starGray.png";
                        }

                        e.Row.Cells[i].Style.VerticalAlign = VerticalAlign.Middle;
                        e.Row.Cells[i].Style.Padding.Top = 1;
                        e.Row.Cells[i].Style.Padding.Bottom = 1;
                        e.Row.Cells[i].Style.BackgroundImage = img;
                        e.Row.Cells[i].Style.CustomRules =
                            "background-repeat: no-repeat; background-position: left center; padding-top: 2px";
                    }
                }

                if (rateIndication && e.Row.Cells[rateIncreaseColumnIndex].Value != null && e.Row.Cells[rateIncreaseColumnIndex].Value.ToString() != String.Empty)
                {
                    string img;
                    bool growIncreasing = Convert.ToBoolean(e.Row.Cells[rateIncreaseColumnIndex].Value);

                    if (growIncreasing)
                    {
                        img = "~/images/arrowGreenUpBB.png";
                    }
                    else
                    {
                        img = "~/images/arrowRedDownBB.png";
                    }

                    e.Row.Cells[i].Style.VerticalAlign = VerticalAlign.Middle;
                    e.Row.Cells[i].Style.Padding.Top = 1;
                    e.Row.Cells[i].Style.Padding.Bottom = 1;
                    e.Row.Cells[i].Style.BackgroundImage = img;
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; padding-top: 2px";
                }
            }
        }

        #endregion
    }
}

