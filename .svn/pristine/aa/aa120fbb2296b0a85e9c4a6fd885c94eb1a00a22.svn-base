using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Common.GridIndicatorRules;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Core;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0035_0035_h : CustomReportPage
    {
        private DataTable gridDt;
        private DateTime currentDate;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            #region Настройка грида

            GridBrick.Width = 450;
            GridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(Grid_InitializeLayout);

            #endregion

            currentDate = CubeInfoHelper.FoBudgetOutcomesInfo.LastDate;

            UserParams.PeriodYear.Value = currentDate.Year.ToString();
            UserParams.PeriodMonth.Value = CRHelper.RusMonth(currentDate.Month);
            UserParams.PeriodHalfYear.Value = String.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(currentDate.Month));
            UserParams.PeriodQuater.Value = String.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(currentDate.Month));
            UserParams.PeriodDayFO.Value = currentDate.Day.ToString();

            string regionName = RegionSettingsHelper.Instance.ShortName.Replace("ВологдО", "ВологО");

            TextBox1.Text = String.Format("Исполнение бюджета {1} по расходам на {0:dd.MM.yyyy} г.", currentDate, regionName);

            DateTime nextMonthDate = currentDate.AddMonths(1);

            Label1.Text = String.Format("данные на 1 {0} {1} года", CRHelper.RusMonthGenitive(nextMonthDate.Month), nextMonthDate.Year);
            Label2.Text = String.Format("обновлено {0:dd.MM.yyyy HH:mm:ss}", DateTime.Now);

            planHintLabel.Text = "План = План текущего месяца, млн.руб.";
            factHintLabel.Text = "Факт = Исполнение с начала месяца до текущего дня, млн.руб.";
            percentHintLabel.Text = "% исп. = Процент исполнения плана текущего месяца, %";

            GridDataBind();
        }

        #region Обработчики грида

        private void GridDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0035_0035_h");
            gridDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатели", gridDt);

            if (gridDt.Rows.Count > 0)
            {
                GridBrick.DataTable = gridDt;

                FontRowLevelRule levelRule = new FontRowLevelRule(gridDt.Columns.Count - 1);
                levelRule.AddFontLevel("0", GridBrick.BoldFont11pt);
                GridBrick.AddIndicatorRule(levelRule);
            }
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.FixedCellStyleDefault.Padding.Right = 2;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = 250;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);

            int columnCount = e.Layout.Bands[0].Columns.Count;

            e.Layout.Bands[0].Columns[columnCount - 1].Hidden = true;

            for (int i = 1; i < columnCount - 1; i = i + 1)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], GetColumnFormat(e.Layout.Bands[0].Columns[i].Header.Caption));
                e.Layout.Bands[0].Columns[i].Width = 70;
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }
            
            GridHeaderLayout headerLayout = GridBrick.GridHeaderLayout;

            headerLayout.AddCell("РзПр");
            headerLayout.AddCell("План");
            headerLayout.AddCell("Факт");
            headerLayout.AddCell("% исп. за мес.");

            headerLayout.ApplyHeaderInfo();
        }

        private static string GetColumnFormat(string name)
        {
            return name.ToLower().Contains("%") ? "P1" : "N1";
        }


        #endregion
    }
}