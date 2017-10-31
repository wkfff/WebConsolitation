using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.UltraGauge.Resources;
using Infragistics.WebUI.UltraWebGauge;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Core;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0038_0002 : CustomReportPage
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

            monthNum = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());
            yearNum = Convert.ToInt32(dtDate.Rows[0][0]);

            DateTime CurrentDate = new DateTime(yearNum, monthNum, 1);
            UserParams.PeriodCurrentDate.Value = CRHelper.PeriodMemberUName("", CurrentDate, 4);
            UserParams.PeriodLastDate.Value = CRHelper.PeriodMemberUName("", CurrentDate.AddYears(-1), 4);
            Label1.Text = string.Format("Консолидированный бюджет ВологО<br/>за {0} {1} {2} года, млн.руб.", monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum);

            UltraWebGrid1.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid1_InitializeLayout);
            UltraWebGrid1.InitializeRow += new InitializeRowEventHandler(UltraWebGrid1_InitializeRow);

            headerLayout = new GridHeaderLayout(UltraWebGrid1);
            UltraWebGrid1.Bands.Clear();

            UltraWebGrid1.DataSource = GetDataTable("FO_0002_0002_incomes");
            UltraWebGrid1.DataBind();

            UltraWebGrid1.Width = Unit.Empty;
            UltraWebGrid1.Height = Unit.Empty;
        }

        private static DataTable GetDataTable(string queryName)
        {
            DataTable dtGrid1 = new DataTable();
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Имя", dtGrid1);
            return dtGrid1;
        }
        
        void UltraWebGrid1_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Style.Padding.Right = 3;
                e.Row.Cells[i].Style.Padding.Top = 2;
                e.Row.Cells[i].Style.Padding.Bottom = 2;
            }
        }

        private string GetConditionBall(object cell)
        {
            if (cell != null && cell != DBNull.Value)
            {
                double value = Convert.ToDouble(cell.ToString());
                string positiveImg = "<img src='../../../images/ballGreenBB.png'>";
                string negativeImg =  "<img src='../../../images/ballRedBB.png'>";                
                if (value < CommonAssessionLimit())
                {
                    return negativeImg;                    
                }
                else
                {
                    return positiveImg;
                }                
            }
            return string.Empty;
        }

        public static void SetConditionArrow(RowEventArgs e, int index)
        {
            if (e.Row.Cells[index] != null &&
                e.Row.Cells[index].Value != null)
            {
                double value = Convert.ToDouble(e.Row.Cells[index].Value.ToString());
                string img = string.Empty;
                string title = string.Empty;
                if (value > 1)
                {
                    img = "~/images/arrowGreenUpBB.png";
                    title = "Рост к прошлому году";
                }
                else if (value < 1)
                {
                    img = "~/images/arrowRedDownBB.png";
                    title = "Снижение к прошлому году";
                }

                e.Row.Cells[index].Style.BackgroundImage = img;
                e.Row.Cells[index].Style.CustomRules = "background-repeat: no-repeat; background-position: 2px center; padding-left: 0px";
                e.Row.Cells[index].Title = title;
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

            e.Layout.Bands[0].Columns[0].Width = 97;
            e.Layout.Bands[0].Columns[1].Width = 80;
            e.Layout.Bands[0].Columns[2].Width = 80;
            e.Layout.Bands[0].Columns[3].Width = 59;

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N2");

            headerLayout.AddCell("Имя");
            headerLayout.AddCell("План");
            headerLayout.AddCell("Факт");
            headerLayout.AddCell("% исп.");

            headerLayout.ApplyHeaderInfo();

            //e.Layout.Bands[0].Columns[1].Header.Style.Font.Size = FontUnit.Parse("14px");
            //e.Layout.Bands[0].Columns[2].Header.Style.Font.Size = FontUnit.Parse("14px");
        }       
    }
}