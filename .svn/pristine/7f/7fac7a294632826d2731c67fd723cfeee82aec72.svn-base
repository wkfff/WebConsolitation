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
using System.Collections.ObjectModel;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0009_Samara
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
        private static MemberAttributesDigest moDigest;
        private static MemberAttributesDigest grbsDigest;

        #endregion

        #region Параметры запроса

        // выбранный месяц
        private CustomParam currMonth;
        // множество периодов
        private CustomParam periodSet;
        // предыдущий месяц
        private CustomParam prevMonth;
        // выбранный индикатор
        private CustomParam selectedIndicator;
        // множество районов(администраторов)
        private CustomParam moSet;

        #endregion

        private IndicatorType IndicatorType
        {
            get
            {
                switch (IndicatorButtonList.SelectedIndex)
                {

                    case 0:
                        {
                            return IndicatorType.MO;
                        }
                    default:
                        {
                            return IndicatorType.GRBS;
                        }
                }
            }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth * 0.99);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight / 1.4);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            #region Инициализация параметров запроса

            currMonth = UserParams.CustomParam("curr_month");
            periodSet = UserParams.CustomParam("period_set");
            prevMonth = UserParams.CustomParam("prev_month");
            selectedIndicator = UserParams.CustomParam("selected_indicator");
            moSet = UserParams.CustomParam("mo_set");
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
                string query = DataProvider.GetQueryText("FO_0002_0009_Samara_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                endMonth = dtDate.Rows[0][3].ToString();

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 150;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(endMonth, true);

                ComboMO.Width = 230;
                ComboMO.Title = "МО";
                ComboMO.MultiSelect = true;
                ComboMO.ParentSelect = true;
                moDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0002_0009_Samara_moIndicator");
                ComboMO.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(moDigest.UniqueNames, moDigest.MemberLevels));
                ComboMO.SetСheckedState("Все районы", true);

                ComboGRBS.Width = 480;
                ComboGRBS.Title = "ГРБС";
                ComboGRBS.MultiSelect = true;
                ComboGRBS.ParentSelect = true;
                grbsDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0002_0009_Samara_grbsIndicator");
                ComboGRBS.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(grbsDigest.UniqueNames, grbsDigest.MemberLevels));
                ComboGRBS.SetСheckedState("Все администраторы", true);
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

            selectedIndicator.Value = Convert.ToBoolean(IndicatorButtonList.SelectedIndex) ? "ГРБС" : "МО";

            if (selectedIndicator.Value == "МО")
            {
                tdGRBS.Visible = false;
                tdMO.Visible = true;
                Collection<string> selectedMo = ComboMO.SelectedValues;
                if (ComboMO.SelectedNodes.Count > 0)
                {
                    string UNameStr = String.Empty;
                    if (selectedMo[0] == "Все районы")
                    {
                        moSet.Value = "[Районы]";
                    }
                    else
                    {
                        for (int i = 0; i < ComboMO.SelectedNodes.Count; i++)
                        {
                            UNameStr += moDigest.GetMemberUniqueName(ComboMO.SelectedValues[i]);
                            UNameStr += ", ";
                        }

                        UNameStr = UNameStr.Remove(UNameStr.Length - 2, 1);

                        moSet.Value = string.Format("{0}", UNameStr);
                    }
                }
                else
                {
                    moSet.Value = "{}";
                }
            }
            else
            {
                tdMO.Visible = false;
                tdGRBS.Visible = true;
                Collection<string> selectedMo = ComboGRBS.SelectedValues;
                if (ComboGRBS.SelectedNodes.Count > 0)
                {                
                    string UNameStr = String.Empty;
                    if (selectedMo[0] == "Все администраторы")
                    {
                        moSet.Value = "[Администраторы]";
                    }
                    else
                    {
                        for (int i = 0; i < ComboGRBS.SelectedNodes.Count; i++)
                        {
                            UNameStr += grbsDigest.GetMemberUniqueName(ComboGRBS.SelectedValues[i]);
                            UNameStr += ", ";
                        }

                        UNameStr = UNameStr.Remove(UNameStr.Length - 2, 1);

                        moSet.Value = string.Format("{0}", UNameStr);
                    }
                }
                else
                {
                    moSet.Value = "{}";
                }   
            }
            Page.Title = String.Format("Информация о кредиторской задолженности ");
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = string.Format("в {0} году", currYear);

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0009_Samara_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование источника", dtGrid);
            if (dtGrid.Columns.Count > 5)
            {
                int i = 0;
                while (i < dtGrid.Rows.Count)
                { 
                    bool isEmpty = true;
                    for (int j = 4; j < dtGrid.Columns.Count; j++)
                    {
                        if (dtGrid.Rows[i][j] != DBNull.Value)
                        {
                            isEmpty = false;
                        }
                    }
                    if (isEmpty)
                    {
                        dtGrid.Rows.RemoveAt(i);
                    }
                    else
                    {
                        string value = dtGrid.Rows[i][0].ToString();
                        int index = value.IndexOf(";");
                        value = value.Remove(index);
                        dtGrid.Rows[i][0] = value;
                        i++;
                    }
                }

                if (dtGrid.Rows.Count == 0)
                {
                    UltraWebGrid.DataSource = null;
                }
                else
                {
                    dtGrid.Columns[2].SetOrdinal(0);
                    dtGrid.Columns[3].SetOrdinal(2);

                    dtGrid.Columns.Add(new DataColumn("column1", typeof(double)));
                    dtGrid.Columns.Add(new DataColumn("column2", typeof(double)));
                    dtGrid.Columns.Add(new DataColumn("column3", typeof(double)));
                    dtGrid.Columns.Add(new DataColumn("column4", typeof(double)));



                    for (i = 0; i < dtGrid.Rows.Count; i++)
                    {
                        if (dtGrid.Rows[i][dtGrid.Columns.Count - 8] == DBNull.Value)
                        {
                            if (dtGrid.Rows[i][dtGrid.Columns.Count - 6] == DBNull.Value)
                            {
                                dtGrid.Rows[i][dtGrid.Columns.Count - 4] = DBNull.Value;
                            }
                            else
                            {
                                dtGrid.Rows[i][dtGrid.Columns.Count - 4] = Convert.ToDouble(dtGrid.Rows[i][dtGrid.Columns.Count - 6]);
                            }
                        }
                        else
                        {
                            if ((dtGrid.Rows[i][dtGrid.Columns.Count - 6] == DBNull.Value) || (Convert.ToDouble(dtGrid.Rows[i][dtGrid.Columns.Count - 6]) == 0))
                            {
                                dtGrid.Rows[i][dtGrid.Columns.Count - 4] = -Convert.ToDouble(dtGrid.Rows[i][dtGrid.Columns.Count - 8]);
                            }
                            else
                            {
                                dtGrid.Rows[i][dtGrid.Columns.Count - 4] = (Convert.ToDouble(dtGrid.Rows[i][dtGrid.Columns.Count - 6]) - Convert.ToDouble(dtGrid.Rows[i][dtGrid.Columns.Count - 8]));
                            }
                        }

                        if (dtGrid.Rows[i][dtGrid.Columns.Count - 7] == DBNull.Value)
                        {
                            if (dtGrid.Rows[i][dtGrid.Columns.Count - 5] == DBNull.Value)
                            {
                                dtGrid.Rows[i][dtGrid.Columns.Count - 3] = DBNull.Value;
                            }
                            else
                            {
                                dtGrid.Rows[i][dtGrid.Columns.Count - 3] = Convert.ToDouble(dtGrid.Rows[i][dtGrid.Columns.Count - 5]);
                            }
                        }
                        else
                        {
                            if ((dtGrid.Rows[i][dtGrid.Columns.Count - 5] == DBNull.Value) || (Convert.ToInt64(dtGrid.Rows[i][dtGrid.Columns.Count - 5]) == 0))
                            {
                                dtGrid.Rows[i][dtGrid.Columns.Count - 3] = -Convert.ToDouble(dtGrid.Rows[i][dtGrid.Columns.Count - 7]);
                            }
                            else
                            {
                                dtGrid.Rows[i][dtGrid.Columns.Count - 3] = (Convert.ToDouble(dtGrid.Rows[i][dtGrid.Columns.Count - 5]) - Convert.ToDouble(dtGrid.Rows[i][dtGrid.Columns.Count - 7]));
                            }
                        }

                        if (dtGrid.Rows[i][4] == DBNull.Value)
                        {
                            if (dtGrid.Rows[i][dtGrid.Columns.Count - 6] == DBNull.Value)
                            {
                                dtGrid.Rows[i][dtGrid.Columns.Count - 2] = DBNull.Value;
                            }
                            else
                            {
                                dtGrid.Rows[i][dtGrid.Columns.Count - 2] = Convert.ToDouble(dtGrid.Rows[i][dtGrid.Columns.Count - 6]);
                            }
                        }
                        else
                        {
                            if ((dtGrid.Rows[i][dtGrid.Columns.Count - 6] == DBNull.Value) || (Convert.ToDouble(dtGrid.Rows[i][dtGrid.Columns.Count - 6]) == 0))
                            {
                                dtGrid.Rows[i][dtGrid.Columns.Count - 2] = -Convert.ToDouble(dtGrid.Rows[i][4]);
                            }
                            else
                            {
                                dtGrid.Rows[i][dtGrid.Columns.Count - 2] = (Convert.ToDouble(dtGrid.Rows[i][dtGrid.Columns.Count - 6]) - Convert.ToDouble(dtGrid.Rows[i][4]));
                            }
                        }
                        if (dtGrid.Rows[i][5] == DBNull.Value)
                        {
                            if (dtGrid.Rows[i][dtGrid.Columns.Count - 5] == DBNull.Value)
                            {
                                dtGrid.Rows[i][dtGrid.Columns.Count - 1] = DBNull.Value;
                            }
                            else
                            {
                                dtGrid.Rows[i][dtGrid.Columns.Count - 1] = Convert.ToDouble(dtGrid.Rows[i][dtGrid.Columns.Count - 5]);
                            }
                        }
                        else
                        {
                            if ((dtGrid.Rows[i][dtGrid.Columns.Count - 5] == DBNull.Value) || (Convert.ToDouble(dtGrid.Rows[i][dtGrid.Columns.Count - 5]) == 0))
                            {
                                dtGrid.Rows[i][dtGrid.Columns.Count - 1] = -Convert.ToDouble(dtGrid.Rows[i][5]);
                            }
                            else
                            {
                                dtGrid.Rows[i][dtGrid.Columns.Count - 1] = (Convert.ToDouble(dtGrid.Rows[i][dtGrid.Columns.Count - 5]) - Convert.ToDouble(dtGrid.Rows[i][5]));
                            }
                        }


                    }

                    UltraWebGrid.DataSource = dtGrid;
                }
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

            if (e.Layout.Bands[0].Columns.Count < 5)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(70);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].MergeCells = true;

            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(200);
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[1].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[1].MergeCells = true;

            e.Layout.Bands[0].Columns[2].Width = CRHelper.GetColumnWidth(80);
            e.Layout.Bands[0].Columns[2].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[2].CellStyle.Wrap = true;

            e.Layout.Bands[0].Columns[3].Width = CRHelper.GetColumnWidth(200);
            e.Layout.Bands[0].Columns[3].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[3].CellStyle.Wrap = true;

            columnCount = e.Layout.Bands[0].Columns.Count;

            for (int i = 4; i < columnCount; i = i + 1)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N0");
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(110);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }
            headerLayout.AddCell("Код источника");
            headerLayout.AddCell("Наименование источника");
            headerLayout.AddCell("Код счета бюджетного учета");
            headerLayout.AddCell("План счетов");

            if (currMonthIndex > 1)
            {
                string monthName;
                for (int i = 1; i <= currMonthIndex; i++)
                {
                    monthName = CRHelper.RusMonth(i).ToUpperFirstSymbol();
                    int monthNum = CRHelper.MonthNum(monthName);
                    GridHeaderCell groupCell = headerLayout.AddCell(string.Format("на {0}", getDate(monthNum, currYear)));
                    groupCell.AddCell("Сумма, руб.");
                    groupCell.AddCell("В том числе просроченная (нереальная) задолженность");
                }

                headerLayout.AddCell(string.Format("Прирост кредиторской задолженности относительно {0}", getDate(currMonthIndex-1, currYear)));
                headerLayout.AddCell(string.Format("Прирост просроченной кредиторской задолженности относительно {0}", getDate(currMonthIndex - 1, currYear)));
                headerLayout.AddCell(string.Format("Прирост кредиторской задолженности относительно 01.02.{0}", currYear));
                headerLayout.AddCell(string.Format("Прирост просроченной кредиторской задолженности относительно 01.02.{0}", currYear));
            }
            else
            {
                GridHeaderCell groupCell = headerLayout.AddCell(string.Format("на 01.01.{0}", currYear));
                groupCell.AddCell("Сумма, руб.");
                groupCell.AddCell("В том числе просроченная (нереальная) задолженность");
                GridHeaderCell groupCell2 = headerLayout.AddCell(string.Format("на 01.02.{0}", currYear));
                groupCell2.AddCell("Сумма, руб.");
                groupCell2.AddCell("В том числе просроченная (нереальная) задолженность");

                headerLayout.AddCell(string.Format("Прирост кредиторской задолженности относительно 01.01.{0}", currYear));
                headerLayout.AddCell(string.Format("Прирост просроченной кредиторской задолженности относительно 01.01.{0}", currYear));
                headerLayout.AddCell(string.Format("Прирост кредиторской задолженности относительно 01.01.{0}", currYear));
                headerLayout.AddCell(string.Format("Прирост просроченной кредиторской задолженности относительно 01.01.{0}", currYear));
            }

            headerLayout.ApplyHeaderInfo();

        }

        protected string getDate(int month, int year)
        {
            if (month < 9)
            {
                return string.Format("01.0{0}.{1}", month + 1, year);
            }
            else
            {
                if (month == 12)
                {
                    return string.Format("на 01.01.{0}", currYear + 1);
                }
                else
                {
                    return string.Format("на 01.{0}.{1}", month + 1, currYear);
                }
            }
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

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

            Report report = new Report();
            ISection section1 = report.AddSection();

            int i = 0;
            Color backGround = Color.FromArgb(255, 255, 255);
            while (i < UltraWebGrid.Rows.Count)
            {
                int first = i;
                int code = Convert.ToInt32(UltraWebGrid.Rows[i].Cells[0].Text);
                string value = UltraWebGrid.Rows[i].Cells[1].ToString();
                if (backGround == Color.FromArgb(241, 241, 241))
                {
                    backGround = Color.FromArgb(255, 255, 255);
                }
                else
                {
                    backGround = Color.FromArgb(241, 241, 241);
                }
                while (((i + 1) < UltraWebGrid.Rows.Count) && (UltraWebGrid.Rows[i].Cells[0].Text == UltraWebGrid.Rows[i + 1].Cells[0].Text))
                {
                    UltraWebGrid.Rows[i].Cells[0].Style.BorderDetails.WidthBottom = 0;
                    UltraWebGrid.Rows[i].Cells[1].Style.BorderDetails.WidthBottom = 0;
                    UltraWebGrid.Rows[i].Cells[0].Style.BorderDetails.WidthTop = 0;
                    UltraWebGrid.Rows[i].Cells[1].Style.BorderDetails.WidthTop = 0;
                    UltraWebGrid.Rows[i].Cells[0].Style.BackColor = backGround;
                    UltraWebGrid.Rows[i].Cells[1].Style.BackColor = backGround;
                    UltraWebGrid.Rows[i].Cells[0].Text = "";
                    UltraWebGrid.Rows[i].Cells[1].Text = "";
                    i++;
                }
                UltraWebGrid.Rows[i].Cells[0].Style.BorderDetails.WidthTop = 0;
                UltraWebGrid.Rows[i].Cells[1].Style.BorderDetails.WidthTop = 0;
                UltraWebGrid.Rows[i].Cells[0].Style.BackColor = backGround;
                UltraWebGrid.Rows[i].Cells[1].Style.BackColor = backGround;
                UltraWebGrid.Rows[i].Cells[0].Text = "";
                UltraWebGrid.Rows[i].Cells[1].Text = "";
                int middle = (i - first) / 2 + first;
                UltraWebGrid.Rows[middle].Cells[0].Value = code;
                UltraWebGrid.Rows[middle].Cells[1].Value = value;
                i++;
            }
    
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

    public enum IndicatorType
    {
        MO,
        GRBS
    }
}