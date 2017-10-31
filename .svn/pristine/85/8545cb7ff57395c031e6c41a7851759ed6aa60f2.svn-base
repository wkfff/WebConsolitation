using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FK_0001_0004
{
    public partial class DefaultCompare_budget : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();
        private int firstYear = 2000;
        private int endYear = 2011;
        private string month = "Январь";

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 20);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight - 215);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);

            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;

            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);
            UltraGridExporter1.MultiHeader = true;

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler
                <Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);

            CrossLink1.Text = "Структурная&nbsp;динамика&nbsp;фактических&nbsp;доходов";
            CrossLink1.NavigateUrl = "~/reports/FK_0001_0004/DefaultCompareChart_budget.aspx";
            CrossLink2.Text = "Распределение&nbsp;на&nbsp;группы&nbsp;по&nbsp;темпам&nbsp;роста&nbsp;доходов";
            CrossLink2.NavigateUrl = "~/reports/FK_0001_0004/DefaultGroupAllocation_budget.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FK_0001_0004_date_budget");
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                UserParams.PeriodDayFK.Value = dtDate.Rows[0][4].ToString();
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                month = dtDate.Rows[0][3].ToString();

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(month, true);

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

                UserParams.Filter.Value = ComboFO.SelectedValue;
            }

            Page.Title = string.Format("Темп роста доходов ({0})", ComboFO.SelectedIndex == 0
                                                                       ? "РФ"
                                                                       :
                                                                   RegionsNamingHelper.ShortName(ComboFO.SelectedValue));
            Label1.Text = Page.Title;

            int monthNum = ComboMonth.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            Label2.Text = string.Format("Сравнение темпа роста фактических доходов по субъектам РФ ({3}) за {0} {1} {2} года", 
                monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum, ComboSKIFLevel.SelectedValue);

            int year = Convert.ToInt32(ComboYear.SelectedValue);

            UserParams.KDTotal.Value = (year < 2003) ? "Доходы бюджета - ВСЕГО" : "Доходы - всего в том числе:";
            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            UserParams.PeriodEndYear.Value = year.ToString();
            UserParams.PeriodYear.Value = (year - 1).ToString();
            UserParams.PeriodFirstYear.Value = (year - 2).ToString();
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.Subject.Value = string.Format("].[{0}].[{1}]", UserParams.Region.Value, UserParams.StateArea.Value);
            UserParams.SKIFLevel.Value = ComboSKIFLevel.SelectedValue;

            UltraWebGrid.DataBind();

            string patternValue = UserParams.StateArea.Value;
            int defaultRowIndex = 1;
            if (patternValue == string.Empty)
            {
                patternValue = UserParams.StateArea.Value;
                defaultRowIndex = 0;
            }

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
            string query = DataProvider.GetQueryText("FK_0001_0004_compare_Grid_budget");
            dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtGrid);

            if (dtGrid.Columns.Count > 2)
            {
                dtGrid.Columns[1].ColumnName = "ФО";
            }

            UserParams.Filter.Value = ComboFO.SelectedValue;
            if (ComboFO.SelectedIndex != 0)
            {
                UltraWebGrid.DataSource = CRHelper.SetDataTableFilter(dtGrid, "ФО", RegionsNamingHelper.ShortName(ComboFO.SelectedValue));
            }
            else
            {
                UltraWebGrid.DataSource = dtGrid;
            }
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

            for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "N0";
                int widthColumn = 60;

                int j = (i - 1) % 5;
                switch (j)
                {
                    case 1:
                    case 2:
                        {
                            formatString = "N3";
                            widthColumn = 90;
                            break;
                        }
                    case 3:
                        {
                            formatString = "P2";
                            widthColumn = 80;
                            break;
                        }
                    case 0:
                    case 4:
                        {
                            formatString = "P2";
                            widthColumn = 75;
                            break;
                        }
                }

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = widthColumn;
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            e.Layout.Bands[0].Columns[0].Width = 250;

            e.Layout.Bands[0].Columns[1].Header.Caption = "ФО";
            e.Layout.Bands[0].Columns[1].Header.Title = "Федеральный округ, которому принадлежит субъект";
            e.Layout.Bands[0].Columns[1].Width = 45;

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (i == 0 || i == 1)
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                }
                else
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                }
            }

            int multiHeaderPos = 2;

            for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i = i + 5)
            {
                ColumnHeader ch = new ColumnHeader(true);
                string[] captions = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                ch.Caption = captions[0].TrimEnd('_');

                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, "Исполнено, млн.руб.", "Фактическое исполнение нарастающим итогом с начала года");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1, "Исполнено прошлый год, млн.руб.", "Исполнено за аналогичный период прошлого года");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 2, "Темп роста к прошлому году", "Темп роста исполнения к аналогичному периоду прошлого года");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 3, "Доля", "Доля дохода в общей сумме доходов выбранного субъекта");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 4, "Доля в прошлом году", "Доля дохода в общей сумме фактических доходов в прошлом году");

                if (i == 22)
                {
                    e.Layout.Bands[0].Columns[i + 3].Hidden = true;
                    e.Layout.Bands[0].Columns[i + 4].Hidden = true;
                }

                ch.RowLayoutColumnInfo.OriginY = 0;
                ch.RowLayoutColumnInfo.OriginX = multiHeaderPos;
                multiHeaderPos += 5;
                ch.RowLayoutColumnInfo.SpanX = i == 22 ? 3 : 5;
                ch.Style.Wrap = true;
                e.Layout.Bands[0].HeaderLayout.Add(ch);
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 4; i < e.Row.Cells.Count; i = i + 5)
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
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px"; 
                }

                if (e.Row.Cells[i + 1].Value != null && e.Row.Cells[i + 1].Value.ToString() != string.Empty &&
                    e.Row.Cells[i + 2].Value != null && e.Row.Cells[i + 2].Value.ToString() != string.Empty)
                {
                    if (Convert.ToDouble(e.Row.Cells[i + 1].Value) < Convert.ToDouble(e.Row.Cells[i + 2].Value))
                    {
                        e.Row.Cells[i + 1].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                        e.Row.Cells[i + 1].Title = "Доля упала с прошлого года";
                    }
                    else if (Convert.ToDouble(e.Row.Cells[i + 1].Value) > Convert.ToDouble(e.Row.Cells[i + 2].Value))
                    {
                        e.Row.Cells[i + 1].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                        e.Row.Cells[i + 1].Title = "Доля выросла с прошлого года";
                    }
                    e.Row.Cells[i + 1].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }
            }

            if (e.Row.Cells[0].Value != null && e.Row.Cells[0].Value.ToString() != string.Empty)
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

        protected void UltraWebGrid_ActiveRowChange(object sender, RowEventArgs e)
        {
            ActiveGridRow(e.Row);
        }

        #endregion

        #region Экспорт в EXCEL

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Label1.Text + " " + Label2.Text;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            for (int i = 2; i < UltraWebGrid.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = UltraGridExporter.ExelNumericFormat;
                int widthColumn = 70;

                int j = (i - 1) % 5;
                switch (j)
                {
                    case 1:
                    case 2:
                        {
                            formatString = "#,##0.000;[Red]-#,##0.000";
                            widthColumn = 85;
                            break;
                        }
                    case 3:
                        {
                            formatString = UltraGridExporter.ExelPercentFormat;
                            widthColumn = 80;
                            break;
                        }
                    case 0:
                    case 4:
                        {
                            formatString = UltraGridExporter.ExelPercentFormat;
                            widthColumn = 75;
                            break;
                        }
                }

                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = formatString;
                e.CurrentWorksheet.Columns[i].Width = widthColumn * 37;
            }
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            e.HeaderText = UltraWebGrid.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex].Header.Key.Split(';')[0];
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid);
        }

#endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
        }

        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(Label1.Text);

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(Label2.Text);
        }

        private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {
        }

        #endregion
    }
}
