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
    public partial class FO_0038_0003_H : CustomReportPage
    {
        private DataTable dtGrid = new DataTable();

        private int monthNum;
        private int yearNum;

        private GridHeaderLayout headerLayout;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_0002_0002_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            //period_day_fk
            //[Период].[Период].[Данные всех периодов].[2010].[Полугодие 2].[Квартал 4].[Октябрь]
            monthNum = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());
            yearNum = Convert.ToInt32(dtDate.Rows[0][0]);

            DateTime CurrentDate = new DateTime(yearNum, monthNum, 1);
            UserParams.PeriodCurrentDate.Value = CRHelper.PeriodMemberUName("", CurrentDate, 4);
            UserParams.PeriodLastDate.Value = CRHelper.PeriodMemberUName("", CurrentDate.AddMonths(-1), 4);
            Label1.Text = String.Format("за {0} {1} {2} года", monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum);
            //Label3.Text = String.Format("Темп роста фактических показателей исполнения бюджета за {0} {1} {2} г. к аналогичному периоду предыдущего года по ВологО", monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum);
            TextBox1.Text = "Информация о расходах консолидированного бюджета";
            TextBox2.Text = String.Format("ВологО за {0} {1} {2} года, % исполнения и темп роста", monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum);
            TextBox3.Text = "к аналогичному периоду прошлого года";


            UltraWebGrid1.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid1_InitializeLayout);
            UltraWebGrid1.InitializeRow += new InitializeRowEventHandler(UltraWebGrid1_InitializeRow);

            headerLayout = new GridHeaderLayout(UltraWebGrid1);
            UltraWebGrid1.Bands.Clear();

            UltraWebGrid1.DataSource = GetDataSource();
            UltraWebGrid1.DataBind();

            UltraWebGrid1.Width = Unit.Empty;
            UltraWebGrid1.Height = Unit.Empty;

            Label2.Text = string.Format("обновлено {0:dd.MM.yyyy HH:mm:ss}", DateTime.Now);
        }

        private DataTable GetDataSource()
        {
            string query = DataProvider.GetQueryText("FO_0038_0003_grid_h");
            DataTable dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Имя", dtGrid);

            return dtGrid;
        }

        void UltraWebGrid1_InitializeRow(object sender, RowEventArgs e)
        {
            //SetConditionCorner(e, 2);
            //SetConditionCorner(e, 1);

            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Style.Padding.Right = 3;
            }
        }

        public static void SetConditionCorner(RowEventArgs e, int index)
        {
            if (e.Row.Cells[index] != null &&
                e.Row.Cells[index].Value != null)
            {
                double value = Convert.ToDouble(e.Row.Cells[index].Value.ToString());
                string img = string.Empty;

                if (value > 1)
                {
                    img = "~/images/cornerGreen.gif";
                }
                else if (value < 1)
                {
                    img = "~/images/cornerRed.gif";
                }

                e.Row.Cells[index].Style.BackgroundImage = img;
                e.Row.Cells[index].Style.CustomRules = "background-repeat: no-repeat; background-position: right top; padding-left: 0px";
            }
        }

        /// <summary>
        /// Пороговое значение оценки остальных.
        /// </summary>
        /// <returns></returns>
        private double CommonAssessionLimit()
        {
            return 1.0 / 12.0 * (Double)monthNum;
        }

        void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].HeaderStyle.Wrap = true;
            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;

            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N1");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N1");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N1");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N1");

            e.Layout.Bands[0].Columns[0].Width = 165;
            e.Layout.Bands[0].Columns[1].Width = 92;
            e.Layout.Bands[0].Columns[2].Width = 92;
            e.Layout.Bands[0].Columns[3].Width = 62;
            e.Layout.Bands[0].Columns[4].Width = 62;

            headerLayout.AddCell("РзПр");
            headerLayout.AddCell("План");
            headerLayout.AddCell("Факт");
            headerLayout.AddCell("% исп.");
            headerLayout.AddCell("Темп роста %");

            headerLayout.ApplyHeaderInfo();

            e.Layout.Bands[0].Columns[1].Header.Style.Font.Size = FontUnit.Parse("14px");
            e.Layout.Bands[0].Columns[2].Header.Style.Font.Size = FontUnit.Parse("14px");
            e.Layout.Bands[0].Columns[3].Header.Style.Font.Size = FontUnit.Parse("14px");
            e.Layout.Bands[0].Columns[4].Header.Style.Font.Size = FontUnit.Parse("14px");
        }
    }
}