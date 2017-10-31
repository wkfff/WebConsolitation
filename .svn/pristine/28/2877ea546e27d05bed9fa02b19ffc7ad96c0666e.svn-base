using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Layers;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Core.MemberDigests;
using System.Collections.Generic;
using System.Collections;
using System.Globalization;
using Krista.FM.Server.Dashboards.Components;
//using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs;
using Krista.FM.ServerLibrary;
using System.Web.SessionState;
using System.Web;
using Krista.FM.Common;
using System.Runtime.Remoting.Messaging;


namespace Krista.FM.Server.Dashboards.reports.FO_0002_0015
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();
        private DataTable dtChart = new DataTable();
        private int firstYear = 2007;
        private int endYear;
        private string endMonth;
        private int badRank;
        private int rubMulti = 1000;
        private GridHeaderLayout headerLayout;
        private static MemberAttributesDigest levelDigest;
        private string multiplierCaption;
        private DateTime date;
        private bool sumRowFlag = false;

        private CustomParam periodYear;
        private CustomParam periodMonth;
        private CustomParam periodHalfYear;
        private CustomParam periodQuater;
        private CustomParam periodPrevYear;
        private CustomParam levelBudget;
        private CustomParam regionChart;
        //private CustomParam chartKateg;
        private CustomParam currentRegion;
        private CustomParam countItemLegend;

        // выбранный множитель рублей
        private CustomParam rubMultiplier;

        private bool IsNotEmptyYears()
        {
            return ComboYear.SelectedNodes.Count > 0;
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight - 230);
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            UltraWebGrid.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            periodYear = UserParams.CustomParam("period_year");
            periodMonth = UserParams.CustomParam("period_month");
            periodHalfYear = UserParams.CustomParam("period_halfyear");
            periodQuater = UserParams.CustomParam("period_quater");
            periodPrevYear = UserParams.CustomParam("period_prev_year");
            levelBudget = UserParams.CustomParam("level_budget");
            rubMultiplier = UserParams.CustomParam("rubMultiplier");
            regionChart = UserParams.CustomParam("region_chart");
            //chartKateg = UserParams.CustomParam("chart_kateg");
            currentRegion = UserParams.CustomParam("current_Region");
            countItemLegend = UserParams.CustomParam("count_item");
        }


        private void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            int countBox = 0;
            int countText = 0;
            int offsetBox = 0;
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];

                if (primitive is Text)
                {
                    Text text = (Text)primitive;
                    string strText = text.GetTextString();
                    if (!strText.Contains("%"))
                    {
                        text.bounds.Height = 50;
                        text.labelStyle.WrapText = true;
                        text.bounds.Offset(0, countText * 20);
                        countText++;
                    }
                }

                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    countBox++;
                    if (countBox >= (Convert.ToInt32(countItemLegend.Value) + 2))
                    {
                        box.rect.Offset(0, (offsetBox * 20) + 3);
                        offsetBox++;
                    }
                }
            }
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                //WebAsyncRefreshPanel1.AddRefreshTarget(UltraChart1);
                //WebAsyncRefreshPanel1.AddRefreshTarget(chart1Caption);
                //WebAsyncRefreshPanel1.AddLinkedRequestTrigger(UltraWebGrid);

                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0002_0015_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                endMonth = dtDate.Rows[0][3].ToString();

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);


                ComboMonth.Width = 140;
                ComboMonth.Title = "Месяц";
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(endMonth, true);


                ComboLevel.Width = 350;
                ComboLevel.Title = "Уровень бюджета";
                ComboLevel.MultiSelect = false;
                levelDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0002_0015_level_budget");
                ComboLevel.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(levelDigest.UniqueNames, levelDigest.MemberLevels));
                ComboLevel.SetСheckedState("Конс.бюджет субъекта", true);
            }

            Page.Title = "Информация о государственном и муниципальном долге";
            Label1.Text = Page.Title;

            int monthNum = ComboMonth.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            date = new DateTime(yearNum, monthNum, 1).AddMonths(1);

            Label2.Text = String.Format("{1}, данные на {0:dd.MM.yyyy} года",
            date, ComboLevel.SelectedValue);
            periodYear.Value = ComboYear.SelectedValue;
            periodMonth.Value = ComboMonth.SelectedValue;
            periodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            periodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));
            periodPrevYear.Value = string.Format("{0}", Convert.ToInt32(ComboYear.SelectedValue) - 1);
            levelBudget.Value = ComboLevel.SelectedValue;

            int selIndex = RubMiltiplierButtonList.SelectedIndex;
            if (selIndex == 0)
            {
                multiplierCaption = "руб.";
                rubMulti = 1;
            }
            else
            {
                if (selIndex == 1)
                {
                    multiplierCaption = "тыс.руб.";
                    rubMulti = 1000;
                }
                else
                {
                    multiplierCaption = "млн.руб.";
                    rubMulti = 1000000;
                }
            }
            rubMultiplier.Value = string.Format("{0}", rubMulti);



            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();

        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0015_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Бюджет", dtGrid);
            if (dtGrid.Rows.Count > 0)
            {

                const string columnName = "Объем долга на душу населения со множителем";
                string rankColumnName = "Ранг";
                RankCalculator rank = new RankCalculator(RankDirection.Desc);
                dtGrid.PrimaryKey = new DataColumn[] { dtGrid.Columns[0] };
                foreach (DataRow row in dtGrid.Rows)
                {
                    if (row[0] != DBNull.Value)
                    {
                        string rowName = row[0].ToString();
                        DataRow dtRow = dtGrid.Rows.Find(rowName);
                        if ((rowName != "Итого") && (rowName != "Новосибирская Область (Бюджет Субъекта)"))
                        {
                            if (multiplierCaption == "млн.руб.")
                            {
                                rank.AddItem(Convert.ToDouble(dtRow[columnName])*1000);
                            }else
                            {
                                rank.AddItem(Convert.ToDouble(dtRow[columnName]));
                            }
                        }
                    }
                }
                foreach (DataRow row in dtGrid.Rows)
                {
                    string rowName = row[0].ToString();  
                    if (((rowName != "Итого") && (rowName != "Новосибирская Область (Бюджет Субъекта)")) && row[columnName] != DBNull.Value && row[columnName].ToString() != String.Empty)
                    {
                        DataRow dtRow = dtGrid.Rows.Find(rowName);
                        double value = 0;
                        if (multiplierCaption == "млн.руб.")
                        {
                            value = Convert.ToDouble(dtRow[columnName]) * 1000;
                        }
                        else
                        {
                            value = Convert.ToDouble(dtRow[columnName]);
                        }
                        int colrank = rank.GetRank(value);
                        if (colrank != 0)
                        {
                            row[rankColumnName] = colrank;
                        }
                    }
                    else
                    {
                        row[rankColumnName] = DBNull.Value;
                    }
                }
                badRank = rank.GetWorseRank();
                if (((ComboLevel.SelectedValue == "Бюджет субъекта") || (ComboLevel.SelectedValue == "Бюджет поселения")) && dtGrid.Rows.Count == 2)
                {
                    dtGrid.Rows.RemoveAt(0);
                }
                if (!((ComboLevel.SelectedValue == "Бюджет субъекта") || (ComboLevel.SelectedValue == "Конс.бюджет субъекта")) && dtGrid.Rows.Count > 1)
                {
                    dtGrid.Rows.RemoveAt(dtGrid.Rows.Count - 1);
                }
                if (Convert.ToDouble(dtGrid.Rows[dtGrid.Rows.Count - 1][2].ToString()) != 0)
                {
                    UltraWebGrid.DataSource = dtGrid;
                }
                else
                {
                    UltraWebGrid.DataSource = null;
                }
            }
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowSortingDefault = AllowSorting.OnClient;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(180);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.VerticalAlign = VerticalAlign.Top;

            int columnCount = e.Layout.Bands[0].Columns.Count;
            for (int i = 1; i < columnCount; i = i + 1)
            {
                switch (i)
                {
                    case 1:
                    case 2:
                    case 3:
                    case 5:
                        {
                            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                            e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(114);
                            break;
                        }

                    case 4:
                    case 6:
                        {
                            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N0");
                            e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(114);
                            break;
                        }
                }
                e.Layout.Bands[0].Columns[3].Width = CRHelper.GetColumnWidth(127);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }
            headerLayout = new GridHeaderLayout(UltraWebGrid);
            headerLayout.AddCell("Бюджет");
            headerLayout.AddCell(string.Format("Долг на 01.01.{0} года, ",
            ComboYear.SelectedValue) + multiplierCaption,
            string.Format("Сумма долга на 01.01.{0} года", ComboYear.SelectedValue));
            headerLayout.AddCell(string.Format("Долг на {0:dd.MM.yyyy} года, ",
            date) + multiplierCaption,
            string.Format("Сумма долга на {0:dd.MM.yyyy} года", date));
            headerLayout.AddCell("Изменение долга к началу года, " + multiplierCaption,
            "Абсолютное изменение долга к началу года");
            headerLayout.AddCell(string.Format("Численность населения на 01.01.{0} года, чел.",
            ComboYear.SelectedValue),
            string.Format("Численность постоянного населения на начало года", ComboYear.SelectedValue));
            headerLayout.AddCell("Объем долга на душу населения, " + multiplierCaption,
            "Объем долга на душу населения");
            headerLayout.AddCell("Ранг", "Ранг по объему долга на душу населения");


            headerLayout.ApplyHeaderInfo();
        }

        private static DataTable GetNonEmptyDt(DataTable sourceDt)
        {
            DataTable dt = sourceDt.Clone();

            foreach (DataRow row in sourceDt.Rows)
            {
                if (!IsEmptyRow(row, 2))
                {
                    dt.ImportRow(row);
                }
            }
            dt.AcceptChanges();
            return dt;
        }

        private static bool IsEmptyRow(DataRow row, int startColumnIndex)
        {
            for (int i = startColumnIndex; i < row.Table.Columns.Count; i++)
            {
                if (row[i] != DBNull.Value)
                {
                    return false;
                }
            }
            return true;
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells[0].ToString() == "Новосибирская Область (Бюджет Субъекта)")
            {
                e.Row.Cells[0].Value = "Областной бюджет";
                e.Row.Cells[0].Style.VerticalAlign = VerticalAlign.Middle;
                for (int j = 0; j < e.Row.Cells.Count; j = j + 1)
                {
                    e.Row.Cells[j].Style.Font.Bold = true;
                    e.Row.Height = 40;
                }
            }
            else
            {
                if (e.Row.Cells[0].ToString() == "Итого")
                {
                    for (int j = 0; j < e.Row.Cells.Count; j = j + 1)
                    {
                        e.Row.Cells[j].Style.Font.Bold = true;
                    }
                }
            }

            for (int i = 1; i < e.Row.Cells.Count; i = i + 1)
            {

                if (i == 6 && e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                {
                    int rank = Convert.ToInt32(e.Row.Cells[i].Value);
                    if (rank == 1)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/starYellowBB.png";
                        e.Row.Cells[i].Title = "Минимальный объем долга на душу населения";
                    }
                    else if (rank == badRank)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/starGrayBB.png";
                        e.Row.Cells[i].Title = "Максимальный объем долга на душу населения";
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }
                if (i == 3 && e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                {
                    if (Convert.ToDouble(e.Row.Cells[i].Value) > 0)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedUpBB.png";
                        e.Row.Cells[i].Title = "Рост долга по сравнению с началом года";
                    }
                    else if (Convert.ToDouble(e.Row.Cells[i].Value) < 0)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenDownBB.png";
                        e.Row.Cells[i].Title = "Снижение долга по сравнению с началом года";
                    }
                    else if (Convert.ToDouble(e.Row.Cells[i].Value) == 0)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowYellowRightBB.png";
                        e.Row.Cells[i].Title = "Стабильное значение долга";
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }
                UltraGridCell cell = e.Row.Cells[i];
                if (cell.Value != null && cell.Value.ToString() != string.Empty)
                {
                    decimal value;
                    if (decimal.TryParse(cell.Value.ToString(), out value))
                    {
                        if (value < 0)
                        {
                            cell.Style.ForeColor = Color.Red;
                        }
                    }
                }


            }
        }

        private void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            UltraGridRow row = CRHelper.FindGridRow(UltraWebGrid, currentRegion.Value, 0, 0);
            ActivateGridRow(row);
        }

        protected void UltraWebGrid_ActiveRowChange(object sender, Infragistics.WebUI.UltraWebGrid.RowEventArgs e)
        {
            ActivateGridRow(e.Row);

        }

        private void ActivateGridRow(UltraGridRow row)
        {
            if (row == null)
            {
                return;
            }

            string region = row.Cells[0].Text;
            string caption = string.Empty;

            if (region == "Областной бюджет")
            {
                caption = string.Format("Структура государственного долга на {0:dd.MM.yyyy} года, Новосибирская область", date);
                regionChart.Value = "[Новосибирская Область (Бюджет Субъекта)]";
                //currentRegion.Value = region;
            }
            else
            {
                if (region == "Местные бюджеты")
                {
                    caption = string.Format("Структура муниципального долга на {0:dd.MM.yyyy} года", date);
                    sumRowFlag = true;
                    //currentRegion.Value = region;
                }
                else
                {
                    if (region.Contains("г."))
                    {
                        caption = string.Format("Структура муниципального долга на {0:dd.MM.yyyy} года, {1}", date, region);
                        regionChart.Value = string.Format("[Городские округа].[{0}]", region);
                        //currentRegion.Value = region;
                    }
                    else
                    {
                        caption = string.Format("Структура муниципального долга на {0:dd.MM.yyyy} года, {1}", date, region);
                        regionChart.Value = string.Format("[Муниципальные районы].[{0}]", region);
                        //currentRegion.Value = region;
                    }
                }
            }
            currentRegion.Value = region;
            UltraWebGrid.DisplayLayout.SelectedRows.Clear();
            row.Selected = true;
            UltraWebGrid.DisplayLayout.ActiveRow = row;
        }


        #endregion

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;
            ReportExcelExporter1.GridColumnWidthScale = 0.9;

            Workbook workbook = new Workbook();

            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");

            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = Label1.Text;
            ReportPDFExporter1.PageSubTitle = Label2.Text;
            ReportPDFExporter1.HeaderCellHeight = 50;

            Report report = new Report();

            ISection section1 = report.AddSection();
            ISection section2 = report.AddSection();
            ReportPDFExporter1.Export(headerLayout, section1);
        }

        #endregion
    }
}