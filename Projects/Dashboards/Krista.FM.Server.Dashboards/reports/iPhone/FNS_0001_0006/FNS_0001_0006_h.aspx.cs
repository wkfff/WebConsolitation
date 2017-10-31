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
    public partial class FNS_0001_0006_h : CustomReportPage
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
            UserParams.IncomesKDRootName.Value = RegionSettingsHelper.Instance.IncomesKDRootName;

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FNS_0001_0006_iphone_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            int year = Convert.ToInt32(dtDate.Rows[0][0]);
            string month = dtDate.Rows[0][3].ToString();
            currentDate = new DateTime(year, CRHelper.MonthNum(month), 1);

            UserParams.PeriodLastYear.Value = currentDate.AddYears(-1).Year.ToString();
            UserParams.PeriodYear.Value = currentDate.Year.ToString();
            UserParams.PeriodMonth.Value = month;
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(currentDate.Month));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(currentDate.Month));
            
            string regionName = RegionSettingsHelper.Instance.ShortName.Replace("ВологдО", "ВологО");
            TextBox1.Text = String.Format("Анализ доходов бюджета, получаемых от");
            TextBox2.Text = String.Format("отраслей хозяйства по {3} за {0} {1} {2} года",
                currentDate.Month, CRHelper.RusManyMonthGenitive(currentDate.Month), currentDate.Year, regionName);
            
            Label1.Text = string.Format("данные на 1 {0} {1} года", CRHelper.RusMonthGenitive(currentDate.AddMonths(1).Month), currentDate.AddMonths(1).Year);
            Label2.Text = string.Format("обновлено {0:dd.MM.yyyy HH:mm:ss}", DateTime.Now);

            UltraWebGrid.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("FNS_0001_0006_iphone_h");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "ОКВЭД", dt);

            foreach (DataRow row in dt.Rows)
            {
                if (row[0] != DBNull.Value)
                {
                    row[0] = DataDictionariesHelper.GetShortOKVDName(row[0].ToString());
                }
            }

            UltraWebGrid.DataSource = dt;
        }

        private static void AddColumnHeader(UltraGridLayout layout, string caption, int originX, int originY, int spanX)
        {
            ColumnHeader ch = new ColumnHeader(true);
            ch.Caption = caption;
            ch.RowLayoutColumnInfo.OriginY = originX;
            ch.RowLayoutColumnInfo.OriginX = originY;
            ch.RowLayoutColumnInfo.SpanX = spanX;
            ch.Style.Padding.Top = 1;
            ch.Style.Padding.Bottom = 1;
            ch.Style.VerticalAlign = VerticalAlign.Middle;
            layout.Bands[0].HeaderLayout.Add(ch);
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;

            if (e.Layout.Bands.Count == 0)
                return;

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (i == 0 || i == 3)
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                }
                else
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                }
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = FontUnit.Parse("16px");
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].HeaderStyle.Height = 5;

            AddColumnHeader(e.Layout, "Факт, млн.руб.", 0, 1, 2);

            e.Layout.Bands[0].Columns[1].Header.Caption = String.Format("на 01.01.{0}г.", currentDate.Year);
            e.Layout.Bands[0].Columns[1].Header.RowLayoutColumnInfo.OriginX = 1;
            e.Layout.Bands[0].Columns[2].Header.Caption = String.Format("на {0:dd.MM.yyyy}г.", currentDate.AddMonths(1));
            e.Layout.Bands[0].Columns[2].Header.RowLayoutColumnInfo.OriginX = 2;

            e.Layout.Bands[0].Columns[0].Width = 140;
            e.Layout.Bands[0].Columns[1].Width = 100;
            e.Layout.Bands[0].Columns[2].Width = 100;
            e.Layout.Bands[0].Columns[3].Width = 125;

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N2");
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                int deviationColumnIndex = 3;
                bool deviationIndication = (i == deviationColumnIndex);

                if (deviationIndication && e.Row.Cells[deviationColumnIndex].Value != null && e.Row.Cells[deviationColumnIndex].Value.ToString() != String.Empty)
                {
                    string img = String.Empty;
                    double deviationValue = Convert.ToDouble(e.Row.Cells[deviationColumnIndex].Value);

                    if (deviationValue > 0)
                    {
                        img = "~/images/arrowGreenUpBB.png";
                    }
                    else if (deviationValue < 0)
                    {
                        img = "~/images/arrowRedDownBB.png";
                    }

                    e.Row.Cells[i].Style.VerticalAlign = VerticalAlign.Middle;
                    e.Row.Cells[i].Style.Padding.Top = 1;
                    e.Row.Cells[i].Style.Padding.Bottom = 1;
                    e.Row.Cells[i].Style.BackgroundImage = img;
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; padding-top: 2px";
                }

                if (e.Row.Cells[0].Value != null && e.Row.Cells[0].Value.ToString() == "Все коды ОКВЭД")
                {
                    e.Row.Cells[i].Style.Font.Bold = true;
                    e.Row.Cells[i].Style.ForeColor = Color.White;
                }
            }
        }

        #endregion
    }
}
