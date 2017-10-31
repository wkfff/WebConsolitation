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
    public partial class FK_0004_0001 : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();

        // выбранный период
        private CustomParam selectedPeriod;

        private DateTime date;
        private GridHeaderLayout headerLayout;
        
        protected override void Page_Load(object sender, EventArgs e)
        {
            #region Инициализация параметров запроса

            selectedPeriod = UserParams.CustomParam("selected_period");

            #endregion

            dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FK_0004_0001_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "dummy", dtDate);

            date = CRHelper.DateByPeriodMemberUName(dtDate.Rows[0][1].ToString(), 3);
           
            selectedPeriod.Value = dtDate.Rows[0][1].ToString();

            UltraWebGrid.Width = Unit.Empty;
            UltraWebGrid.Height = Unit.Empty;
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            UltraWebGrid.DataBind();

            lbDescription.Text = String.Format("Анализ остатков средств бюджета на счетах Казначейства России, Резервного фонда<br/>и Фонда национального благосостояния по состоянию на&nbsp;<b><span class='DigitsValue'>{0:dd.MM.yyyy}</span></b>", date);

            query = DataProvider.GetQueryText("FK_0004_0001_Chart");
            DataTable dtChart = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Наименование", dtChart);

            double maxValue = Convert.ToDouble(dtChart.Rows[5][2]);

            FK_0004_0001_Chart1.Value = Convert.ToDouble(dtChart.Rows[0][2]);
            FK_0004_0001_Chart2.Value = Convert.ToDouble(dtChart.Rows[1][2]); 
            FK_0004_0001_Chart3.Value = Convert.ToDouble(dtChart.Rows[2][2]); 
            FK_0004_0001_Chart4.Value = Convert.ToDouble(dtChart.Rows[3][2]); 
            FK_0004_0001_Chart5.Value = Convert.ToDouble(dtChart.Rows[4][2]); 

            FK_0004_0001_Chart1.MaxValue = maxValue;
            FK_0004_0001_Chart2.MaxValue = maxValue;
            FK_0004_0001_Chart3.MaxValue = maxValue;
            FK_0004_0001_Chart4.MaxValue = maxValue;
            FK_0004_0001_Chart5.MaxValue = maxValue;

            FK_0004_0001_Chart1.Name = "Остатки средств федерального бюджета на ЕКС";
            FK_0004_0001_Chart2.Name = "Средства ФБ, размещенные на депозиты";
            FK_0004_0001_Chart3.Name = "Резервный Фонд";
            FK_0004_0001_Chart4.Name = "Фонд национального благосостояния";
            FK_0004_0001_Chart5.Name = "Остатки на других счетах Казначейства России";

            FK_0004_0001_Chart1.RestId = "1";
            FK_0004_0001_Chart2.RestId = "2";
            FK_0004_0001_Chart3.RestId = "3";
            FK_0004_0001_Chart4.RestId = "4";
            FK_0004_0001_Chart5.RestId = "5";
        }

        #region Обработчики грида

        protected void KosguGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FK_0004_0001_Grid");
            DataTable dtGridKosgu = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Наименование", dtGridKosgu);
            dtGridKosgu.Columns.RemoveAt(0);
            UltraWebGrid.DataSource = dtGridKosgu;
        }

        protected void OutcomesGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowRowNumberingDefault = RowNumbering.None;

            e.Layout.Bands[0].Columns[0].Width = 330;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            e.Layout.Bands[0].Columns[1].Width = 146;
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");

            e.Layout.Bands[0].Columns[2].Width = 146;
            e.Layout.Bands[0].Columns[2].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N2");

            e.Layout.Bands[0].Columns[3].Width = 126;
            e.Layout.Bands[0].Columns[3].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N2");
            

            e.Layout.Bands[0].Columns[4].Hidden = true;

            headerLayout = new GridHeaderLayout(e.Layout.Grid);

            headerLayout.AddCell(" ");
            headerLayout.AddCell(String.Format("на 01.01.{0:yyyy}, млрд.руб.", date));
            headerLayout.AddCell(String.Format("на {0:dd.MM.yyyy}, млрд.руб.", date));
            headerLayout.AddCell("Изменение с начала года, млрд.руб.");

            headerLayout.ApplyHeaderInfo();
        }

        protected void OutcomesGrid_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells[4].Value.ToString() == "1")
            {
                foreach (UltraGridCell cell in e.Row.Cells)
                {
                    cell.Style.Font.Bold = true;
                    cell.Style.Font.Size = 15;
                }
            }
            else if (e.Row.Cells[4].Value.ToString() == "2")
            {
                
            }
            else if (e.Row.Cells[4].Value.ToString() == "3")
            {
                //e.Row.Cells[0].Style.Padding.Left = 20;
                foreach (UltraGridCell cell in e.Row.Cells)
                {
                    cell.Style.ForeColor = Color.FromArgb(192, 192, 192);
                }
            }
            
            iPadBricks.iPadBricks.iPadBricksHelper.SetConditionArrow(e, 3);
            
        }

        #endregion       
    }
}
