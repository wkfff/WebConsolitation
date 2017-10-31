using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FK_0001_0004
{
    public partial class DefaultDetail : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();
        private int firstYear = 2000;
        private int endYear = 2011;
        private string month = "Январь";

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight - 200);
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 7);

            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);

            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;
            
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FK_0001_0004_date");
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                month = dtDate.Rows[0][3].ToString();

                UserParams.PeriodYear.Value = endYear.ToString();
                UserParams.PeriodMonth.Value = month;

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

                regionsCombo.Width = 300;
                regionsCombo.Title = "Субъект РФ";
                regionsCombo.FillDictionaryValues(CustomMultiComboDataHelper.FillRegions(RegionsNamingHelper.FoNames, RegionsNamingHelper.RegionsFoDictionary));
                regionsCombo.ParentSelect = false;
                if (!string.IsNullOrEmpty(UserParams.StateArea.Value))
                {
                    regionsCombo.SetСheckedState(UserParams.StateArea.Value, true);
                }
                else if (RegionSettings.Instance != null && RegionSettings.Instance.Name != String.Empty)
                {
                    regionsCombo.SetСheckedState(RegionSettings.Instance.Name, true);
                }

                ComboSKIFLevel.Width = 250;
                ComboSKIFLevel.ParentSelect = true;
                ComboSKIFLevel.Title = "Уровень бюджета";
                ComboSKIFLevel.FillDictionaryValues(CustomMultiComboDataHelper.FillSKIFLevels());
                ComboSKIFLevel.SetСheckedState("Консолидированный бюджет субъекта", true);
            }

            UserParams.Region.Value = regionsCombo.SelectedNodeParent;
            UserParams.StateArea.Value = regionsCombo.SelectedValue;
            UserParams.SKIFLevel.Value = ComboSKIFLevel.SelectedValue;

            int monthNum = ComboMonth.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);

            Page.Title = string.Format("Доходы: {0}", UserParams.StateArea.Value);
            Label1.Text = Page.Title;
            Label2.Text = string.Format("Доходы субъекта РФ ({3}) за {0} {1} {2} года",
                monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum, ComboSKIFLevel.SelectedValue);

            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            UserParams.PeriodYear.Value = ComboYear.SelectedValue;
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));

            UltraWebGrid.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FK_0001_0004_grid");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtGrid);

            UltraWebGrid.DataSource = dtGrid;
        }

        protected void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            //            UltraWebGrid.Height = Unit.Empty;
            //            UltraWebGrid.Width = Unit.Empty;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (Page.IsPostBack)
                return;

            e.Layout.GroupByBox.Hidden = true;

            if (e.Layout.Bands[0].Columns.Count > 11)
            {
                e.Layout.Bands[0].HeaderStyle.Wrap = true;
                e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                e.Layout.Bands[0].Columns[11].Hidden = true;

                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 0, "КД", "");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 1, "Код", "");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 2, "Исполнено, млн.руб.", "Фактическое исполнение нарастающим итогом с начала года");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 3, "Исполнено прошлый год, млн.руб.", "Исполнено за аналогичный период прошлого года");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 4, "Темп роста к прошлому году", "Темп роста исполнения к аналогичному периоду прошлого года");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 5, "Доля", "Доля дохода в общей сумме доходов субъекта");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 6, "Доля в прошлом году", "Доля дохода в общей сумме фактических доходов в прошлом году");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 7, "Назначено, млн.руб.", "Плановые назначения на год");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 8, "Назначено прошлый год, млн.руб.", "Назначения в аналогичном периоде прошлого года");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 9, "Исполнено %", "Процент выполнения назначений/ Оценка равномерности исполнения (1/12 годового плана в месяц)");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 10, "Исполнено % прошлый год", "Процент выполнения назначений за аналогичный период прошлого года");

                e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

                for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = (i == 0) ?
                                                                             HorizontalAlign.Left :
                                                                             HorizontalAlign.Right;
                    double width;
                    switch (i)
                    {
                        case 0:
                            {
                                width = 200;
                                break;
                            }
                        case 2:
                        case 3:
                        case 7:
                        case 8:
                            {
                                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N3");
                                width = 92;
                                break;
                            }
                        case 4:
                        case 5:
                        case 6:
                        case 9:
                        case 10:
                            {
                                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "P2");
                                width = 81;
                                break;
                            }
                        default:
                            {
                                width = 114;
                                e.Layout.Bands[0].Columns[i].CellStyle.Padding.Right = 5;
                                break;
                            }
                    }
                    e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(width);
                }
                e.Layout.Bands[0].Columns[4].Width = CRHelper.GetColumnWidth(95);
                e.Layout.Bands[0].Columns[9].Width = CRHelper.GetColumnWidth(90);
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                if (e.Row.Cells[i] == null)
                {
                    continue;
                }

                if (i == 9 && e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                {
                    double percent = (ComboMonth.SelectedIndex + 1) * 100.0 / 12;

                    if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < percent)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/ballRedBB.png";
                        e.Row.Cells[i].Title = string.Format("Не соблюдается условие равномерности ({0:N2}%)", percent);
                    }
                    else
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/ballGreenBB.png";
                        e.Row.Cells[i].Title = string.Format("Соблюдается условие равномерности ({0:N2}%)", percent);
                    }
                    e.Row.Cells[i].Style.Padding.Right = 2;
                }

                if ((i == 4) && e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                {
                    if (100 * Convert.ToDouble(e.Row.Cells[i].Value) > 100)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                        e.Row.Cells[i].Title = "Рост к прошлому году";
                    }
                    else if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 100)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                        e.Row.Cells[i].Title = "Падение к прошлому году";
                    }
                }

                if (e.Row.Cells[5].Value != null && e.Row.Cells[5].Value.ToString() != string.Empty &&
                    e.Row.Cells[6].Value != null && e.Row.Cells[6].Value.ToString() != string.Empty)
                {
                    if (Convert.ToDouble(e.Row.Cells[5].Value) < Convert.ToDouble(e.Row.Cells[6].Value))
                    {
                        e.Row.Cells[5].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                        e.Row.Cells[5].Title = "Доля упала с прошлого года";
                    }
                    else if (Convert.ToDouble(e.Row.Cells[5].Value) > Convert.ToDouble(e.Row.Cells[6].Value))
                    {
                        e.Row.Cells[5].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                        e.Row.Cells[5].Title = "Доля выросла с прошлого года";
                    }
                }

                if (e.Row.Cells[11] != null && e.Row.Cells[11].Value.ToString() != string.Empty && i != 1)
                {
                    string level = e.Row.Cells[11].Value.ToString();
                    int fontSize = 8;
                    bool bold = false;
                    bool italic = false;
                    switch (level)
                    {
                        case "Группа":
                            {
                                fontSize = 10;
                                bold = false;
                                italic = false;
                                break;
                            }
                        case "Подгруппа":
                            {
                                fontSize = 10;
                                bold = false;
                                italic = true;
                                break;
                            }
                        case "Статья":
                            {
                                fontSize = 8;
                                bold = true;
                                italic = false;
                                break;
                            }
                    }
                    e.Row.Cells[i].Style.Font.Size = fontSize;
                    e.Row.Cells[i].Style.Font.Bold = bold;
                    e.Row.Cells[i].Style.Font.Italic = italic;
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center";
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

        #endregion

        private bool titleAdded = false;
        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
           /* IText title = e.Section.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(Label1.Text);

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(Label2.Text);*/
            if (!titleAdded)
            {
                IText title = e.Section.AddText();
                Font font = new Font("Verdana", 16);
                title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
                title.Style.Font.Bold = true;
                title.AddContent(Label1.Text);
            }

            titleAdded = true;
        }

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Label1.Text + " " + Label2.Text;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            e.CurrentWorksheet.Columns[0].Width = 400 * 37;
            e.CurrentWorksheet.Columns[2].Width = 200 * 37;
            e.CurrentWorksheet.Columns[3].Width = 200 * 37;
            e.CurrentWorksheet.Columns[4].Width = 200 * 37;
            e.CurrentWorksheet.Columns[5].Width = 200 * 37;
            e.CurrentWorksheet.Columns[6].Width = 200 * 37;
            e.CurrentWorksheet.Columns[7].Width = 200 * 37;
            e.CurrentWorksheet.Columns[8].Width = 200 * 37;
            e.CurrentWorksheet.Columns[9].Width = 200 * 37;
            e.CurrentWorksheet.Columns[10].Width = 200 * 37;

            e.CurrentWorksheet.Columns[1].CellFormat.FormatString = "0";
            e.CurrentWorksheet.Columns[2].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";
            e.CurrentWorksheet.Columns[3].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";

            e.CurrentWorksheet.Columns[4].CellFormat.FormatString = "0.00%";
            e.CurrentWorksheet.Columns[5].CellFormat.FormatString = "0.00%";
            e.CurrentWorksheet.Columns[6].CellFormat.FormatString = "0.00%";

            e.CurrentWorksheet.Columns[7].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";
            e.CurrentWorksheet.Columns[8].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";

            e.CurrentWorksheet.Columns[9].CellFormat.FormatString = "0.00%";
            e.CurrentWorksheet.Columns[10].CellFormat.FormatString = "0.00%";
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid);
        }

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            UltraGridExporter1.GridElementCaption = Label2.Text;
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
        }
    }
}
