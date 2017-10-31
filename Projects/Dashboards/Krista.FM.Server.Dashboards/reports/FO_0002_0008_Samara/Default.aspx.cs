using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0008_Samara
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private DataTable dtNameRegion;
        private int firstYear = 2009;
        private int endYear = 2012;
        private string endMonth = "";
        private int currYear;
        private int columnCount;
        int currMonthIndex;
        private GridHeaderLayout headerLayout;

        #endregion

        #region Параметры запроса

        // выбранный месяц
        private CustomParam currMonth;
        // множество периодов
        private CustomParam periodSet;
        // предыдущий месяц
        private CustomParam prevMonth;

        #endregion

        //private MeasureType MeasureType
        //{
        //    get
        //    {
        //        switch (MeasureButtonList.SelectedIndex)
        //        {
                    
        //            case 0:
        //                {
        //                    return MeasureType.Evaluation;
        //                }
        //            default:
        //                {
        //                    return MeasureType.Value;
        //                }
        //        }
        //    }
        //}

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth * 0.96);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight / 1.4);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            #region Инициализация параметров запроса

            currMonth = UserParams.CustomParam("curr_month");
            periodSet = UserParams.CustomParam("period_set");
            prevMonth = UserParams.CustomParam("prev_month");
            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click); 
        }
        
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0002_0008_Samara_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                endMonth = dtDate.Rows[0][3].ToString();
               
                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 200;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(endMonth, true);
            }
            currYear = Convert.ToInt32(ComboYear.SelectedValue);
            currMonth.Value = ComboMonth.SelectedValue;
            currMonthIndex = CRHelper.MonthNum(currMonth.Value);
            int prevMonthIndex = 0;
            if (currMonthIndex == 1)
            {
                prevMonthIndex = 12;
            }
            else
            {
                prevMonthIndex = currMonthIndex - 1;
            }
            prevMonth.Value = CRHelper.RusMonth(prevMonthIndex).ToUpperFirstSymbol();
            string period = "";
            if (currMonthIndex > 1)
            {
                for (int i = 1; i <= currMonthIndex; i++)
                {
                    period += string.Format("[Период__Период].[Период__Период].[Данные всех периодов].[{0}].[Полугодие {1}].[Квартал {2}].[{3}]",
                        currYear, CRHelper.HalfYearNumByMonthNum(i), CRHelper.QuarterNumByMonthNum(i), CRHelper.RusMonth(i).ToUpperFirstSymbol());
                    period += ", ";
                }
                period = period.Remove(period.Length - 2, 1);
          
            }
            else
            {
                period += string.Format("[Период__Период].[Период__Период].[Данные всех периодов].[{0}].[Полугодие 2].[Квартал 4].[Декабрь]", currYear - 1);
                period += ", ";
                period += string.Format("[Период__Период].[Период__Период].[Данные всех периодов].[{0}].[Полугодие 1].[Квартал 1].[Январь]", currYear);
            }
            periodSet.Value = period;
            Page.Title = String.Format("Динамика кредиторской задолженности муниципальных образований ");
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = string.Format("в {0} году", currYear);

            //selectedMeasure.Value = Convert.ToBoolean(MeasureButtonList.SelectedIndex) ? "Значение" : "Оценка индикатора";

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
        }

        #region Обработчики грида
        
        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0008_Samara_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Муниципальное образование", dtGrid);
            if (dtGrid.Rows.Count > 0)
            {
                
                //if (MeasureType == MeasureType.Value)
                //{
                //    ClearFirstLevelValues(dtGrid);
                //}
                dtGrid.Columns.Add(new DataColumn("column1", typeof(double)));
                dtGrid.Columns.Add(new DataColumn("column2", typeof(double)));
                dtGrid.Columns.Add(new DataColumn("column3", typeof(double)));
                dtGrid.Columns.Add(new DataColumn("column4", typeof(double)));
                for (int i = 0; i < dtGrid.Rows.Count; i++)
                {
                    if (dtGrid.Rows[i][dtGrid.Columns.Count - 8] == DBNull.Value) //|| (Convert.ToInt64(dtGrid.Rows[i][dtGrid.Columns.Count - 8]) == 0))
                    {
                        if (dtGrid.Rows[i][dtGrid.Columns.Count - 6] == DBNull.Value) //|| (Convert.ToInt64(dtGrid.Rows[i][dtGrid.Columns.Count - 6]) == 0))
                        {
                            dtGrid.Rows[i][dtGrid.Columns.Count - 4] = null;
                            dtGrid.Rows[i][dtGrid.Columns.Count - 3] = null;
                        }
                        else
                        {
                            if (Convert.ToInt64(dtGrid.Rows[i][dtGrid.Columns.Count - 6]) == 0)
                            {
                                dtGrid.Rows[i][dtGrid.Columns.Count - 4] = 0;
                                dtGrid.Rows[i][dtGrid.Columns.Count - 3] = 0;
                            }
                            else
                            {
                                dtGrid.Rows[i][dtGrid.Columns.Count - 4] = Convert.ToDouble(dtGrid.Rows[i][dtGrid.Columns.Count - 6]);
                                dtGrid.Rows[i][dtGrid.Columns.Count - 3] = 1;
                            }
                        }
                    }
                    else
                    {
                        if (Convert.ToInt64(dtGrid.Rows[i][dtGrid.Columns.Count - 8]) == 0)
                        {
                            if ((dtGrid.Rows[i][dtGrid.Columns.Count - 6] == DBNull.Value) || (Convert.ToInt64(dtGrid.Rows[i][dtGrid.Columns.Count - 6]) == 0))
                            {
                                dtGrid.Rows[i][dtGrid.Columns.Count - 4] = 0;
                                dtGrid.Rows[i][dtGrid.Columns.Count - 3] = 0;
                            }
                            else
                            {
                                dtGrid.Rows[i][dtGrid.Columns.Count - 4] = Convert.ToDouble(dtGrid.Rows[i][dtGrid.Columns.Count - 6]);
                                dtGrid.Rows[i][dtGrid.Columns.Count - 3] = 1;
                            }
                        }
                        else
                        {
                            if ((dtGrid.Rows[i][dtGrid.Columns.Count - 6] == DBNull.Value) || (Convert.ToInt64(dtGrid.Rows[i][dtGrid.Columns.Count - 6]) == 0))
                            {
                                dtGrid.Rows[i][dtGrid.Columns.Count - 4] = -Convert.ToDouble(dtGrid.Rows[i][dtGrid.Columns.Count - 8]);
                                dtGrid.Rows[i][dtGrid.Columns.Count - 3] = -1;
                            }
                            else
                            {
                                dtGrid.Rows[i][dtGrid.Columns.Count - 4] = (Convert.ToDouble(dtGrid.Rows[i][dtGrid.Columns.Count - 6]) - Convert.ToDouble(dtGrid.Rows[i][dtGrid.Columns.Count - 8]));
                                dtGrid.Rows[i][dtGrid.Columns.Count - 3] = (Convert.ToDouble(dtGrid.Rows[i][dtGrid.Columns.Count - 6]) / Convert.ToDouble(dtGrid.Rows[i][dtGrid.Columns.Count - 8]))  - 1;
                            }
                        }
                        
                    }

                    if (dtGrid.Rows[i][dtGrid.Columns.Count - 7] == DBNull.Value) //|| (Convert.ToInt64(dtGrid.Rows[i][dtGrid.Columns.Count - 8]) == 0))
                    {
                        if (dtGrid.Rows[i][dtGrid.Columns.Count - 5] == DBNull.Value) //|| (Convert.ToInt64(dtGrid.Rows[i][dtGrid.Columns.Count - 6]) == 0))
                        {
                            dtGrid.Rows[i][dtGrid.Columns.Count - 2] = null;
                            dtGrid.Rows[i][dtGrid.Columns.Count - 1] = null;
                        }
                        else
                        {
                            if (Convert.ToInt64(dtGrid.Rows[i][dtGrid.Columns.Count - 5]) == 0)
                            {
                                dtGrid.Rows[i][dtGrid.Columns.Count - 2] = 0;
                                dtGrid.Rows[i][dtGrid.Columns.Count - 1] = 0;
                            }
                            else
                            {
                                dtGrid.Rows[i][dtGrid.Columns.Count - 2] = Convert.ToDouble(dtGrid.Rows[i][dtGrid.Columns.Count - 5]);
                                dtGrid.Rows[i][dtGrid.Columns.Count - 1] = 1;
                            }
                        }
                    }
                    else
                    {
                        if (Convert.ToInt64(dtGrid.Rows[i][dtGrid.Columns.Count - 7]) == 0)
                        {
                            if ((dtGrid.Rows[i][dtGrid.Columns.Count - 5] == DBNull.Value) || (Convert.ToInt64(dtGrid.Rows[i][dtGrid.Columns.Count - 5]) == 0))
                            {
                                dtGrid.Rows[i][dtGrid.Columns.Count - 2] = 0;
                                dtGrid.Rows[i][dtGrid.Columns.Count - 1] = 0;
                            }
                            else
                            {
                                dtGrid.Rows[i][dtGrid.Columns.Count - 2] = Convert.ToDouble(dtGrid.Rows[i][dtGrid.Columns.Count - 5]);
                                dtGrid.Rows[i][dtGrid.Columns.Count - 1] = 1;
                            }
                        }
                        else
                        {
                            if ((dtGrid.Rows[i][dtGrid.Columns.Count - 5] == DBNull.Value) || (Convert.ToInt64(dtGrid.Rows[i][dtGrid.Columns.Count - 5]) == 0))
                            {
                                dtGrid.Rows[i][dtGrid.Columns.Count - 2] = -Convert.ToDouble(dtGrid.Rows[i][dtGrid.Columns.Count - 7]);
                                dtGrid.Rows[i][dtGrid.Columns.Count - 1] = -1;
                            }
                            else
                            {
                                dtGrid.Rows[i][dtGrid.Columns.Count - 2] = (Convert.ToDouble(dtGrid.Rows[i][dtGrid.Columns.Count - 5]) - Convert.ToDouble(dtGrid.Rows[i][dtGrid.Columns.Count - 7]));
                                dtGrid.Rows[i][dtGrid.Columns.Count - 1] = (Convert.ToDouble(dtGrid.Rows[i][dtGrid.Columns.Count - 5]) / Convert.ToDouble(dtGrid.Rows[i][dtGrid.Columns.Count - 7]))  - 1;
                            }
                        }

                    }

                    //if ((dtGrid.Rows[i][dtGrid.Columns.Count - 7] == DBNull.Value) || (Convert.ToInt32(dtGrid.Rows[i][dtGrid.Columns.Count - 7]) == 0))
                    //{
                    //    if (dtGrid.Rows[i][dtGrid.Columns.Count - 5] == DBNull.Value)
                    //    {
                    //        dtGrid.Rows[i][dtGrid.Columns.Count - 2] = null;
                    //        dtGrid.Rows[i][dtGrid.Columns.Count - 1] = null;
                    //    }
                    //    else
                    //    {
                    //        dtGrid.Rows[i][dtGrid.Columns.Count - 2] = Convert.ToInt64(dtGrid.Rows[i][dtGrid.Columns.Count - 5]);
                    //        dtGrid.Rows[i][dtGrid.Columns.Count - 1] = 100;
                    //    }

                    //}
                    //else
                    //{
                    //    if ((dtGrid.Rows[i][dtGrid.Columns.Count - 5] == DBNull.Value) || (Convert.ToInt64(dtGrid.Rows[i][dtGrid.Columns.Count - 5]) == 0))
                    //    {
                    //        dtGrid.Rows[i][dtGrid.Columns.Count - 2] = -Convert.ToInt64(dtGrid.Rows[i][dtGrid.Columns.Count - 7]);
                    //        dtGrid.Rows[i][dtGrid.Columns.Count - 1] = -100;
                    //    }
                    //    else
                    //    {
                    //        dtGrid.Rows[i][dtGrid.Columns.Count - 2] = Convert.ToInt64(Convert.ToDouble(dtGrid.Rows[i][dtGrid.Columns.Count - 5]) - Convert.ToDouble(dtGrid.Rows[i][dtGrid.Columns.Count - 7]));
                    //        dtGrid.Rows[i][dtGrid.Columns.Count - 1] = Convert.ToInt64((Convert.ToDouble(dtGrid.Rows[i][dtGrid.Columns.Count - 5]) / Convert.ToDouble(dtGrid.Rows[i][dtGrid.Columns.Count - 7])) * 100 - 100);
                    //    }
                    //}
                }
                UltraWebGrid.DataSource = dtGrid;
            }
            else
            {
                UltraWebGrid.DataSource = null;
            }
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowSortingDefault = AllowSorting.No;

            if (e.Layout.Bands[0].Columns.Count < 2)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(200);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            columnCount = e.Layout.Bands[0].Columns.Count;

            for (int i = 1; i < columnCount; i = i + 1)
            {
                if ((i == columnCount - 1) || (i == columnCount - 3))
                {
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "P0");
                }
                else
                {
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N0");
                }
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(110);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            headerLayout.AddCell("Муниципальное образование");

            if (currMonthIndex > 1)
            {
                string monthName;
                for (int i = 1; i <= currMonthIndex; i++)
                {
                    monthName = CRHelper.RusMonth(i).ToUpperFirstSymbol();
                    int monthNum = CRHelper.MonthNum(monthName);
                    GridHeaderCell groupCell;
                    if (monthNum < 9)
                    {
                        groupCell = headerLayout.AddCell(string.Format("на 01.0{0}.{1}", monthNum + 1, currYear));
                    }
                    else
                    {
                        if (monthNum == 12)
                        {
                            groupCell = headerLayout.AddCell(string.Format("на 01.01.{0}", currYear + 1));
                        }
                        else
                        {
                            groupCell = headerLayout.AddCell(string.Format("на 01.{0}.{1}", monthNum + 1, currYear));
                        }
                    }
                    //GridHeaderCell groupCell = headerLayout.AddCell(monthName);
                    groupCell.AddCell("ВСЕГО",2);
                    groupCell.AddCell("В том числе просроченная задолженность",2);
                }
            }
            else
            {
                GridHeaderCell groupCell = headerLayout.AddCell(string.Format("на 01.01.{0}", currYear));
                groupCell.AddCell("ВСЕГО",2);
                groupCell.AddCell("В том числе просроченная задолженность",2);
                GridHeaderCell groupCell2 = headerLayout.AddCell(string.Format("на 01.02.{0}", currYear));
                groupCell2.AddCell("ВСЕГО",2);
                groupCell2.AddCell("В том числе просроченная задолженность",2);
            }

            GridHeaderCell groupCell3 = headerLayout.AddCell("Изменение задолженности");
            GridHeaderCell groupCell3_1 = groupCell3.AddCell("ВСЕГО");
            GridHeaderCell groupCell3_2 = groupCell3.AddCell("Просроченная");
            groupCell3_1.AddCell("тыс.руб.");
            groupCell3_1.AddCell("%");
            groupCell3_2.AddCell("тыс.руб.");
            groupCell3_2.AddCell("%");
            headerLayout.ApplyHeaderInfo();

        }



        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = UltraWebGrid.Columns.Count - 4; i < UltraWebGrid.Columns.Count; i++)
            {
                if (e.Row.Cells[i].Value != null)
                {
                    if (Convert.ToDouble(e.Row.Cells[i].Value) < 0)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenDownBB.png";
                        e.Row.Cells[i].Title = "Снижение задолженности к прошлому периоду";
                    }
                    else
                    {
                        if (Convert.ToDouble(e.Row.Cells[i].Value) > 0)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedUpBB.png";
                            e.Row.Cells[i].Title = "Рост задолженности к прошлому периоду";
                        }
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }
            }
        }

        //protected void ClearFirstLevelValues(DataTable dt)
        //{
        //    for (int i = 0; i <= dtGrid.Rows.Count - 1; i++)
        //    {
        //        int level = Convert.ToInt32(dtGrid.Rows[i][dtGrid.Columns.Count - 1]);
        //        if (level == 1)
        //        {
        //            for (int j = 4; j < dtGrid.Columns.Count - 1; j++)
        //            {
        //                dtGrid.Rows[i][j] = DBNull.Value;
        //            }
        //        }
        //    }
        //}

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

            Report report = new Report();
            ISection section1 = report.AddSection();

            ReportPDFExporter1.HeaderCellHeight = 100;
            ReportPDFExporter1.Export(headerLayout, section1);
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");

            ReportExcelExporter1.HeaderCellHeight = 100;
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
        }
        
        #endregion
    }

    //public enum MeasureType
    //{
    //    Value,
    //    Evaluation
    //}
}