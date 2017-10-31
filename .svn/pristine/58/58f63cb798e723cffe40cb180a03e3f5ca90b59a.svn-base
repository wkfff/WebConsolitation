using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FK_0001_0013
{
    public partial class Default : CustomReportPage
    {
        #region Поля
        private const int firstYear = 2007; // TODO: Как взять значение из констант сервера
        private GridHeaderLayout headerLayout;
        #endregion
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 7);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.6 + 80);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);

            #region Настройка диаграммы

            LineAppearance lineAppearance = new LineAppearance();
            lineAppearance.IconAppearance.Icon = SymbolIcon.Diamond;
            lineAppearance.IconAppearance.IconSize = SymbolIconSize.Medium;

            #endregion

            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {

                //gridWebAsyncPanel.AddRefreshTarget(UltraWebGrid);
                //gridWebAsyncPanel.AddLinkedRequestTrigger(ComboFO);

                string query = DataProvider.GetQueryText("FK_0001_0013_date");

                DataTable dtDate = new DataTable();
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                UserParams.PeriodDayFK.Value = dtDate.Rows[0][4].ToString();
                int endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                string month = dtDate.Rows[0][3].ToString();

                UserParams.PeriodYear.Value = endYear.ToString();
                UserParams.PeriodMonth.Value = month;
                //UserParams.Filter.Value = "Все федеральные округа";

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(UserParams.PeriodYear.Value, true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(UserParams.PeriodMonth.Value, true);

                ComboFO.Title = "Федеральный округ";
                ComboFO.Width = 410;
                ComboFO.MultiSelect = false;
                ComboFO.FillDictionaryValues(CustomMultiComboDataHelper.FillFONames(RegionsNamingHelper.FoNames));
                ComboFO.SetСheckedState("Все федеральные округа", true);

                ComboSKIFLevel.Width = 250;
                ComboSKIFLevel.ParentSelect = true;
                ComboSKIFLevel.Title = "Уровень бюджета";
                ComboSKIFLevel.FillDictionaryValues(CustomMultiComboDataHelper.FillSKIFLevels());
                ComboSKIFLevel.SetСheckedState("Консолидированный бюджет субъекта", true);

            }

            Page.Title = "Отдельные показатели исполнения бюджетов";
            Label1.Text = string.Format("Отдельные показатели исполнения бюджетов субъектов {0}", ComboFO.SelectedIndex == 0 ? "РФ" :
                RegionsNamingHelper.ShortName(ComboFO.SelectedValue)); ;

            int monthNum = ComboMonth.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            string SKIFLevel = (ComboSKIFLevel.SelectedValue).ToLower();
            Label2.Text = string.Format("Сравнение показателей исполнения бюджетов субъектов РФ за {0} {1} {2} года ({3})", monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum,
                SKIFLevel) + ", тыс. рублей";

            int year = Convert.ToInt32(ComboYear.SelectedValue);

            UserParams.KDTotal.Value = (year < 2003) ?
                "Доходы бюджета - ВСЕГО" : "Доходы - всего в том числе:";
            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            UserParams.PeriodEndYear.Value = year.ToString();
            UserParams.PeriodYear.Value = (year - 1).ToString();
            UserParams.PeriodFirstYear.Value = (year - 2).ToString();
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.Subject.Value = string.Format("].[{0}].[{1}]", UserParams.Region.Value, UserParams.StateArea.Value);
            UserParams.SKIFLevel.Value = ComboSKIFLevel.SelectedValue;

            if (ComboFO.SelectedIndex != 0)
            {
                UserParams.Filter.Value = string.Format(".[{0}]", ComboFO.SelectedValue);
            }
            else
            {
                UserParams.Filter.Value = ".Children";
            }
            UltraWebGrid.Bands.Clear();
            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.DataBind();


            int defaultRowIndex = 1;

            string patternValue = UserParams.StateArea.Value;
            UltraGridRow row = CRHelper.FindGridRow(UltraWebGrid, patternValue, 0, defaultRowIndex);
            ActiveGridRow(row);

        }

        #region Обработчики грида

        /// <summary>
        /// Активация строки грида
        /// </summary>
        /// <param name="row"></param>
        private void ActiveGridRow(UltraGridRow row)
        {
            if (row == null)
                return;

            string subject = row.Cells[0].Text;

            if (RegionsNamingHelper.IsRF(subject))
            {
                UserParams.Subject.Value = "]";
                UserParams.SubjectFO.Value = "]";
            }
            else if (RegionsNamingHelper.IsFO(subject))
            {
                UserParams.Region.Value = subject;
                UserParams.Subject.Value = string.Format("].[{0}]", UserParams.Region.Value);
                UserParams.SubjectFO.Value = string.Format("].[{0}]", UserParams.Region.Value);
            }
            else
            {
                UserParams.Region.Value = RegionsNamingHelper.FullName(row.Cells[1].Text);
                UserParams.StateArea.Value = subject;
                UserParams.Subject.Value = string.Format("].[{0}].[{1}]", UserParams.Region.Value, UserParams.StateArea.Value);
                UserParams.SubjectFO.Value = string.Format("].[{0}]", UserParams.Region.Value);
            }


        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FK_0001_0013_compare_Grid");
            DataTable dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtGrid);

            query = DataProvider.GetQueryText("FK_0001_0013_compare_Grid1");
            DataTable dtGrid1 = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtGrid1);

            query = DataProvider.GetQueryText("FK_0001_0013_compare_Grid2");
            DataTable dtGrid2 = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtGrid2);

            query = DataProvider.GetQueryText("FK_0001_0013_compare_Grid2a");
            DataTable dtGrid2a = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtGrid2a);



            DataTable dt = new DataTable();

            for (int i = 0; i < dtGrid.Columns.Count; i++)
                dt.Columns.Add(dtGrid.Columns[i].ColumnName, dtGrid.Columns[i].DataType);

            for (int i = 0; i < dtGrid2a.Columns.Count; i++)
                if (dtGrid2a.Columns[i].ColumnName != "Субъект")
                    dt.Columns.Add(dtGrid2a.Columns[i].ColumnName, dtGrid2a.Columns[i].DataType);

            for (int i = 0; i < dtGrid1.Columns.Count; i++)
                if (dtGrid1.Columns[i].ColumnName != "Субъект")
                    dt.Columns.Add(dtGrid1.Columns[i].ColumnName, dtGrid1.Columns[i].DataType);

            for (int i = 0; i < dtGrid2.Columns.Count; i++)
                if (dtGrid1.Columns[i].ColumnName != "Субъект")
                    dt.Columns.Add(dtGrid2.Columns[i].ColumnName, dtGrid2.Columns[i].DataType);

            for (int i = 0; i < dtGrid.Rows.Count; i++)
                dt.ImportRow(dtGrid.Rows[i]);


            int gridOffset = dtGrid.Columns.Count;
            for (int cellsCount = gridOffset; cellsCount < (gridOffset + dtGrid2a.Columns.Count - 1); cellsCount++)
            {
                for (int rowsCount = 0; rowsCount < dt.Rows.Count; rowsCount++)
                {
                    dt.Rows[rowsCount][cellsCount] = dtGrid2a.Rows[rowsCount][cellsCount - gridOffset + 1];
                }
            }

            gridOffset = dtGrid.Columns.Count + dtGrid2a.Columns.Count - 1;
            for (int cellsCount = gridOffset; cellsCount < (gridOffset + dtGrid1.Columns.Count - 1); cellsCount++)
            {
                for (int rowsCount = 0; rowsCount < dt.Rows.Count; rowsCount++)
                {
                    dt.Rows[rowsCount][cellsCount] = dtGrid1.Rows[rowsCount][cellsCount - gridOffset + 1];
                }
            }

            gridOffset = dtGrid.Columns.Count + dtGrid1.Columns.Count - 1 + dtGrid2a.Columns.Count - 1;
            for (int cellsCount = gridOffset; cellsCount < (gridOffset + dtGrid2.Columns.Count - 1); cellsCount++)
            {
                for (int rowsCount = 0; rowsCount < dt.Rows.Count; rowsCount++)
                {
                    dt.Rows[rowsCount][cellsCount] = dtGrid2.Rows[rowsCount][cellsCount - gridOffset + 1];
                }
            }


            for (int k = 0; k < dt.Rows.Count; k++)
            {
                DataRow row = dt.Rows[k];

                for (int i = 1; i < row.ItemArray.Length - 1; i++)
                {
                    if (row[i] != DBNull.Value)
                    {
                        if (i % 3 != 0) row[i] = Convert.ToDouble(row[i]) / 1000;
                    }
                }
            }

            if (dt.Columns.Count > 2)
            {
                dt.Columns[0].ColumnName = "Территории";
                dt.Columns[1].ColumnName = "ИТОГО ДОХОДОВ";
                dt.Columns[dt.Columns.Count - 1].ColumnName = "ФО";
            }


            //dt.Columns.RemoveAt(dt.Columns.Count);
            UltraWebGrid.DataSource = dt;
        }

        void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            if (ComboFO.SelectedIndex != 0)
            {
                UltraWebGrid.Height = Unit.Empty;
            }
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;
            //e.Layout.UseFixedHeaders = true;
            //e.Layout.Bands[0].Columns[0].Header.Fixed = true;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                string formatString = "N0";
                int widthColumn = 80;
                int j = (i - 1) % 3;
                switch (j)
                {
                    case 0:
                        {
                            formatString = "N1";
                            widthColumn = 100;
                            break;
                        }

                    case 1:
                        {
                            formatString = "N1";
                            widthColumn = 100;
                            break;
                        }

                    case 2:
                        {
                            formatString = "P2";
                            widthColumn = 77;
                            break;
                        }

                }

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = widthColumn;
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            e.Layout.Bands[0].Columns[0].Width = 250;
            headerLayout.AddCell("Территории");
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count - 1; i = i + 3)
            {
                string[] captions = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');

                GridHeaderCell cell = headerLayout.AddCell(captions[0].TrimEnd('_'));
                DateTime date = new DateTime(Convert.ToInt32(ComboYear.SelectedValue) - 1, ComboMonth.SelectedIndex + 1,
                                             1);
                date = date.AddMonths(1);
                cell.AddCell(String.Format("На {0:dd.MM.yyyy}", date),
                             "Фактическое исполнение за аналогичный период прошлого года");
                date = new DateTime(Convert.ToInt32(ComboYear.SelectedValue), ComboMonth.SelectedIndex + 1, 1);
                date = date.AddMonths(1);
                cell.AddCell(String.Format("На {0:dd.MM.yyyy}", date), "Исполнено");
                if (!captions[0].TrimEnd('_').Contains("Профицит(+)/Дефицит(-)"))
                    cell.AddCell("Темп роста ", "Темп роста к аналогичному периоду прошлого года");
            }
            headerLayout.ApplyHeaderInfo();
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 3; i < e.Row.Cells.Count - 1; i += 3)
            {
                if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                {
                    if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 100)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                        e.Row.Cells[i].Title = "Падение к прошлому году";
                    }
                    else if (100 * Convert.ToDouble(e.Row.Cells[i].Value) > 100)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                        e.Row.Cells[i].Title = "Рост к прошлому году";
                    }
                }

                string style = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                e.Row.Cells[i].Style.CustomRules = style;
            }

            if (e.Row.Cells[0].Value != null && e.Row.Cells[0].Value.ToString() != String.Empty)
            {
                if (!RegionsNamingHelper.IsSubject(e.Row.Cells[0].Value.ToString()))
                {
                    foreach (UltraGridCell cell in e.Row.Cells)
                    {
                        cell.Style.Font.Bold = true;
                    }
                }
            }

            foreach (UltraGridCell cell in e.Row.Cells)
            {
                if (cell.Value != null && cell.Value.ToString() != String.Empty)
                {
                    decimal value;
                    if (Decimal.TryParse(cell.Value.ToString(), out value))
                    {
                        if (value < 0)
                        {
                            cell.Style.ForeColor = Color.Red;
                        }
                    }
                }
            }
        }

        protected void UltraWebGrid_ActiveRowChange(object sender, RowEventArgs e)
        {
            ActiveGridRow(e.Row);
        }

        #endregion

        #region Экспорт в Excel
        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            sheet1.Rows[0].Cells[0].Value = Label1.Text + " " + Label2.Text;
            ReportExcelExporter1.HeaderCellHeight = 70;
            ReportExcelExporter1.GridColumnWidthScale = 1.3;
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
        }
        #endregion

        #region Экспорт в PDF
        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = Label1.Text;
            Report report = new Report();
            ISection section = report.AddSection();
            ReportPDFExporter1.Export(headerLayout, Label2.Text, section);
        }
        #endregion
    }
}
