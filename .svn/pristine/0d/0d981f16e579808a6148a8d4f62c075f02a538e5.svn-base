using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0002_0016 : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();
        private string month;
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
            string query = DataProvider.GetQueryText("FO_0002_0016_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
            int year = Convert.ToInt32(dtDate.Rows[0][0]);
            string half = string.Format("{0}", dtDate.Rows[0][1]); 
            string quarter = string.Format("{0}", dtDate.Rows[0][2]);
            month = dtDate.Rows[0][3].ToString();
            date = new DateTime(year, CRHelper.MonthNum(month), 1).AddMonths(1);
            // date = CRHelper.DateByPeriodMemberUName(dtDate.Rows[0][1].ToString(), 3);

            selectedPeriod.Value = string.Format("[Период].[Период].[Данные всех периодов].[{0}].[{1}].[{2}].[{3}]", year, half, quarter, month);

            OutcomesGrid.Width = Unit.Empty;
            OutcomesGrid.Height = Unit.Empty;
            OutcomesGrid.DisplayLayout.NoDataMessage = "Нет данных";

            OutcomesGrid.DataBind();

            lbPageTitle.Text = "Исполнение бюджетов Омской области";
            lbDescription.Text = String.Format("Анализ исполнения бюджетов Омской области по состоянию на {0:dd.MM.yyyy} , тыс.руб.", date);
            
        }


        #region Обработчики грида

       protected void OutcomesGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0016_Grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtGrid);

            foreach (DataRow dataRow in dtGrid.Rows)
            {
                string moId = CustomParams.GetMOIdByName((dataRow[0].ToString()));
                string budgetId = string.Empty;

                if (dataRow[0].ToString().Contains("Консолидированный бюджет субъекта"))
                {
                   budgetId = "1";
                }
                else if (dataRow[0].ToString().Contains("Собственный бюджет субъекта"))
                {
                    budgetId = "2";
                }
                else if (dataRow[0].ToString().Contains("Местные бюджеты"))
                {
                    budgetId = "3";
                }
                
                if (moId == String.Empty)
                {
                    if (budgetId == "1" ||
                        budgetId == "2")
                    {
                        dataRow[1] =
                              String.Format(
                                  "<div style='float: right'><a href='webcommand?showReport=FO_0002_0014_BUDGET={0}'><img src='../../../images/detail.png'></a></div>", budgetId);
                    }
                    else
                    {
                        dataRow[1] =
                              String.Format(
                                  "<div style='float: right'><a href='webcommand?showReport=FO_0002_0015'><img src='../../../images/detail.png'></a></div>", budgetId);
                    }
                }
                else
                {
                    dataRow[1] =
                        String.Format(
                            "<div style='float: right'><a href='webcommand?showPinchReport=FO_0002_0017_mo={0}'><img src='../../../images/detail.png'></a></div>", moId);
                }

             
            }

            OutcomesGrid.DataSource = dtGrid;
        }

        protected void OutcomesGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowRowNumberingDefault = RowNumbering.None;

            UltraGridColumn col = e.Layout.Bands[0].Columns[1]; // колонка с "глазом"

            e.Layout.Bands[0].Columns.RemoveAt(1); // вырезаем колонку с "глазом"
            e.Layout.Bands[0].Columns.Insert(0, col); // вставляем "глаз" в позицию 0

            e.Layout.Bands[0].Columns[0].Width = 50;

            e.Layout.Bands[0].Columns[1].Width = 190;
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[1].CellStyle.Wrap = true;
         
            e.Layout.Bands[0].Columns[2].Width = 140;
            e.Layout.Bands[0].Columns[2].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            e.Layout.Bands[0].Columns[2].Header.Caption = "Доходы бюджета"; 
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N0");

            e.Layout.Bands[0].Columns[3].Width = 140;
            e.Layout.Bands[0].Columns[3].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            e.Layout.Bands[0].Columns[3].Header.Caption = "Расходы бюджета"; 
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N0");

            e.Layout.Bands[0].Columns[4].Width = 130;
            e.Layout.Bands[0].Columns[4].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            e.Layout.Bands[0].Columns[4].Header.Caption = "Результат исполнения";
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N0");

            headerLayout = new GridHeaderLayout(e.Layout.Grid);

            e.Layout.Bands[0].Columns[5].Width = 120;
            e.Layout.Bands[0].Columns[5].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            e.Layout.Bands[0].Columns[5].Header.Caption = "Долг"; 
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "N0");

            headerLayout = new GridHeaderLayout(e.Layout.Grid);

            e.Layout.Bands[0].Columns[6].Hidden = true;
            e.Layout.Bands[0].Columns[7].Hidden = true;

            headerLayout.AddCell(" ");
            headerLayout.AddCell("Бюджет");            

            headerLayout.ApplyHeaderInfo();
        }

        protected void OutcomesGrid_InitializeRow(object sender, RowEventArgs e)
        {
            int numMonth = CRHelper.MonthNum(month);
            double percent = (100 * numMonth) / 12.0;
            Label1.Text = string.Format("- не соблюдается условие равномерности({0:N2}%)", percent);
            Label2.Text = string.Format("- соблюдается условие равномерности ({0:N2}%)", percent);
            for (int i = 1; i < e.Row.Cells.Count; i++)
            {
                if (i == 2 || i == 3)
                {
                  
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty && e.Row.Cells[i + 4].Value != null && e.Row.Cells[i + 4].Value.ToString() != string.Empty)
                    {
                        double rate = 100 * Convert.ToDouble(e.Row.Cells[i+4].Value);
                        string hint = string.Empty;

                        if (rate >=percent)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/ballGreenBB.png";
                            hint = string.Format("Соблюдается условие равномерности ({0:N2}%)", percent);
                        }
                        else
                        {
                            if (rate < percent)
                            {
                                e.Row.Cells[i].Style.BackgroundImage = "~/images/ballRedBB.png";
                                hint = string.Format("Не соблюдается условие равномерности ({0:N2}%)", percent);
                            }
                        }
                        e.Row.Cells[i].Title = hint;
                        e.Row.Cells[i].Style.CustomRules ="background-repeat: no-repeat; background-position: left center; margin: 2px";
                    }
                }
            }

            DataTable dtChart = new DataTable();
            dtChart.Columns.Add(new DataColumn("1", typeof(double)));
            
            if (e.Row.Cells[6].Value != null)
            {
                e.Row.Cells[2].Value = String.Format("{0:N0}<br/>{1:P1}", e.Row.Cells[2].Value, e.Row.Cells[6].Value); // 3
            }

            if (e.Row.Cells[7].Value != null)
            {
                e.Row.Cells[3].Value = String.Format("{0:N0}<br/>{1:P1}", e.Row.Cells[3].Value, e.Row.Cells[7].Value); // 4
            }

            if (e.Row.Cells[1].Value.ToString() == "Консолидированный бюджет субъекта")
            {
                foreach (UltraGridCell cell in e.Row.Cells)
                {
                    cell.Style.Font.Bold = true;
                }
                e.Row.Cells[6].Style.Font.Size = 14;
            }

           
        }

        #endregion

   }
}
