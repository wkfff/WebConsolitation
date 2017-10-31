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
    public partial class FO_0002_0011 : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();
        private string month;
        private bool flagMO = false; // город
        private bool flagBud = false; 
        // выбранный период
        private CustomParam selectedPeriod;
      

        private DateTime date;
        private GridHeaderLayout headerLayout;

        protected override void Page_Load(object sender, EventArgs e)
        {
            #region Инициализация параметров запроса

            selectedPeriod = UserParams.CustomParam("selected_period");
    
            #endregion

          //  CustomParams.MakeMoParams("22", "id");

            dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_0002_0011_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            int year = Convert.ToInt32(dtDate.Rows[0][0]);
            string half = string.Format("{0}", dtDate.Rows[0][1]);
            string quarter = string.Format("{0}", dtDate.Rows[0][2]);
            month = dtDate.Rows[0][3].ToString();

            date = new DateTime(year, CRHelper.MonthNum(month), 1).AddMonths(1);
            selectedPeriod.Value = string.Format("[Период__Период].[Период__Период].[Данные всех периодов].[{0}].[{1}].[{2}].[{3}]", year, half, quarter, month);

            OutcomesGrid.Width = Unit.Empty;
            OutcomesGrid.Height = Unit.Empty;
            OutcomesGrid.DisplayLayout.NoDataMessage = "Нет данных";

            OutcomesGrid.DataBind();
            
            CRHelper.SaveToErrorLog(UserParams.Mo.Value);
            CRHelper.SaveToErrorLog(UserParams.BudgetLevel.Value);
            lbPageTitle.Text = string.Format("{0}", UserParams.Mo.Value==String.Empty ? "Местные бюджеты": UserParams.Mo.Value);
            lbDescription.Text = String.Format("Анализ исполнения бюджета по состоянию на {0:dd.MM.yyyy} , тыс.руб.", date);

          }


        #region Обработчики грида

       protected void OutcomesGrid_DataBinding(object sender, EventArgs e)
        {
           

                if (UserParams.Mo.Value.Contains("район")) // выбран район
                {
                    CRHelper.SaveToErrorLog("OK13");
                    string query = DataProvider.GetQueryText("FO_0002_0011_Grid2");
                    dtGrid = new DataTable();
                    DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtGrid);
                }
                else if (UserParams.Mo.Value.Contains("г.") || UserParams.Mo.Value.Contains("р.п."))
                {
                    CRHelper.SaveToErrorLog("OK14");
                    string query = DataProvider.GetQueryText("FO_0002_0011_Grid1");
                    dtGrid = new DataTable();
                    DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtGrid);
                }
             /*   else if (UserParams.BudgetLevel.Value.Contains("Конс. бюджет МО"))
                {
                    string query = DataProvider.GetQueryText("FO_0002_0011_Grid3");
                    dtGrid = new DataTable();
                    DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtGrid);
                }
           */

               if (dtGrid.Rows.Count > 0)
            {
              OutcomesGrid.DataSource = dtGrid;
            }
        }

        protected void OutcomesGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowRowNumberingDefault = RowNumbering.None;

           
            e.Layout.Bands[0].Columns[0].Width = 350;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            e.Layout.Bands[0].Columns[1].Width = 140;
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            e.Layout.Bands[0].Columns[1].Header.Caption = "Исполнено";
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");

            e.Layout.Bands[0].Columns[2].Width = 140;
            e.Layout.Bands[0].Columns[2].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            e.Layout.Bands[0].Columns[2].Header.Caption = "Процент исполнения"; 
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "P2");

            e.Layout.Bands[0].Columns[3].Width = 140;
            e.Layout.Bands[0].Columns[3].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            e.Layout.Bands[0].Columns[3].Header.Caption = "Темп роста к прошлому году"; 
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "P2");
            
            headerLayout = new GridHeaderLayout(e.Layout.Grid);
            headerLayout = new GridHeaderLayout(e.Layout.Grid);
            headerLayout.AddCell("Наименование показателя");            

            headerLayout.ApplyHeaderInfo();
        }

        protected void OutcomesGrid_InitializeRow(object sender, RowEventArgs e)
        {
            int numMonth = CRHelper.MonthNum(month);
            double percent = numMonth/12.0;

            Label1.Text = string.Format("- не соблюдается условие равномерности({0:P2})", percent);
            Label2.Text = string.Format("- соблюдается условие равномерности ({0:P2})", percent);

            for (int i = 1; i < e.Row.Cells.Count; i++)
            {
                if (i == 2)
                {
                   
                        if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                        {
                            double rate = Convert.ToDouble(e.Row.Cells[i].Value);
                            string hint = string.Empty;

                            if (rate >= percent)
                            {
                                e.Row.Cells[i].Style.BackgroundImage = "~/images/ballGreenBB.png";
                                hint = string.Format("Соблюдается условие равномерности ({0:P2})", percent);
                            }
                            else
                            {
                                if (rate < percent)
                                {
                                    e.Row.Cells[i].Style.BackgroundImage = "~/images/ballRedBB.png";
                                    hint = string.Format("Не соблюдается условие равномерности ({0:P2})", percent);
                                }
                            }
                            e.Row.Cells[i].Title = hint;
                            e.Row.Cells[i].Style.CustomRules =
                                "background-repeat: no-repeat; background-position: left center; margin: 2px";
                        }
                  }

                if (i == 3)
                {
                    if (e.Row.Index <= 8)
                    {
                        if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                        {
                            double rate = 100*Convert.ToDouble(e.Row.Cells[i].Value);
                            string hint = string.Empty;

                            if (rate >= 100)
                            {
                                e.Row.Cells[i].Style.BackgroundImage = "~/images/ArrowGreenUpIPad.png";
                                hint = "Рост по отношению к предыдущему году";
                            }
                            else
                            {
                                if (rate < 100)
                                {
                                    e.Row.Cells[i].Style.BackgroundImage = "~/images/ArrowRedDownIPad.png";
                                    hint = "Сокращение по отношению к предыдущему году";
                                }
                            }
                            e.Row.Cells[i].Title = hint;
                            e.Row.Cells[i].Style.CustomRules =
                                "background-repeat: no-repeat; background-position: left center; margin: 2px";
                        }
                    }
                    else
                    {
                        if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                        {
                            double rate = 100 * Convert.ToDouble(e.Row.Cells[i].Value);
                            string hint = string.Empty;

                            if (rate >= 100)
                            {
                                e.Row.Cells[i].Style.BackgroundImage = "~/images/ArrowRedUpIPad.png";
                                hint = "Рост по отношению к предыдущему году";
                            }
                            else
                            {
                                if (rate < 100)
                                {
                                    e.Row.Cells[i].Style.BackgroundImage = "~/images/ArrowGreenDownIPad.png";
                                    hint = "Сокращение по отношению к предыдущему году";
                                }
                            }
                            e.Row.Cells[i].Title = hint;
                            e.Row.Cells[i].Style.CustomRules =
                                "background-repeat: no-repeat; background-position: left center; margin: 2px";
                        }
                    }
                }
            }
        }
    }

        #endregion

}
