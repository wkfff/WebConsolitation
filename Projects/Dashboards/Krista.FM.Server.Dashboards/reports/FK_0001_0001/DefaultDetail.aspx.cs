using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FK_0001_0001
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
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15);
            
            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;

            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);

            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.MultiHeader = true;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FK_0001_0001_date");
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
                regionsCombo.FillDictionaryValues(
                    CustomMultiComboDataHelper.FillRegions(RegionsNamingHelper.FoNames,
                                                           RegionsNamingHelper.RegionsFoDictionary));
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

            int monthNum = ComboMonth.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);

            Page.Title = string.Format("Расходы: {0}", UserParams.StateArea.Value);
            Label1.Text = Page.Title;
            Label2.Text = string.Format("Расходы консолидированного бюджета субъекта РФ ({3}) за {0} {1} {2} года в разрезе разделов, подразделов классификации расходов",
                    monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum, ComboSKIFLevel.SelectedValue);

            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            UserParams.PeriodYear.Value = ComboYear.SelectedValue;
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.SKIFLevel.Value = ComboSKIFLevel.SelectedValue;

            UltraWebGrid.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FK_0001_0001_detail_grid");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtGrid);

            foreach (DataRow row in dtGrid.Rows)
            {
                for (int i = 1; i < row.ItemArray.Length; i++)
                {
                    if ((i == 2 || i == 6 || i == 9 || i == 10) && row[i] != DBNull.Value)
                    {
                        row[i] = Convert.ToDouble(string.Format("{0:N3}", Convert.ToDouble(row[i]) / 1000000));
                    }
                }
            }

            UltraWebGrid.DataSource = dtGrid;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (Page.IsPostBack)
                return;
            
            e.Layout.GroupByBox.Hidden = true;

            if (e.Layout.Bands[0].Columns.Count > 13)
            {
                e.Layout.Bands[0].HeaderStyle.Wrap = true;
                e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                e.Layout.Bands[0].Columns[14].Hidden = true;

                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 0, "РзПр", "");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 1, "Код", "");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 2, "Исполнено, млн.руб.", "Фактическое исполнение нарастающим итогом с начала года");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 3, "Доля", "Доля расхода в общей сумме расходов субъекта");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 4, "Доля ФО", "Доля расхода в общей сумме расходов средняя для федерального округа");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 5, "Доля РФ", "Доля расхода в общей сумме расходов средняя по всем субъектам РФ");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 6, "Назначено, млн.руб.", "Плановые назначения на год");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 7, "Доля по назначениям", "Доля расхода в общей сумме расходов плановая");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 8, "Исполнено %", "Процент выполнения назначений. Оценка равномерности исполнения (1/12 годового плана в месяц)");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 9, "Исполнено прошлый год, млн.руб", "Исполнено за аналогичный период прошлого года");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 10, "Назначено прошлый год, млн.руб.", "Назначения в аналогичном периоде прошлого года");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 11, "Доля в прошлом году", "Доля расхода в общей сумме фактических расходов в прошлом году");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 12, "Исполнено % прошлый год", "Процент выполнения назначений за аналогичный период прошлого года");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 13, "Темп роста к прошлому году", "Темп роста исполнения к аналогичному периоду прошлого года");

                e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

                for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = (i == 0)
                                                                                 ?
                                                                             HorizontalAlign.Left
                                                                                 :
                                                                             HorizontalAlign.Right;
                    double width;
                    switch (i)
                    {
                        case 0:
                            {
                                width = 180;
                                break;
                            }
                        case 1:
                            {
                                width = 40;
                                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "00 00");
                                break;
                            }
                        case 2:
                        case 6:
                        case 9:
                        case 10:
                            {
                                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N3");
                                width = 75;
                                break;
                            }
                        case 11:
                        case 12:
                        case 13:
                            {
                                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "P2");
                                width = 80;
                                break;
                            }
                        default:
                            {
                                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "P2");
                                width = 60;
                                break;
                            }
                    }
                    e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(width);
                }

                e.Layout.Bands[0].Columns[7].Width = CRHelper.GetColumnWidth(80);
                e.Layout.Bands[0].Columns[8].Width = CRHelper.GetColumnWidth(70);
                e.Layout.Bands[0].Columns[3].Width = CRHelper.GetColumnWidth(75);
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

                if (i == 8 && e.Row.Cells[i].Value != null)
                {
                    double percent = (ComboMonth.SelectedIndex + 1)*100.0 / 12 ;
                    
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

                if (i == 11 && e.Row.Cells[i].Value != null)
                {
                    if (Convert.ToDouble(e.Row.Cells[3].Value) > Convert.ToDouble(e.Row.Cells[11].Value))
                    {
                        e.Row.Cells[3].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                        e.Row.Cells[3].Title = "Доля выросла с прошлого года";
                    }
                    else if (Convert.ToDouble(e.Row.Cells[3].Value) < Convert.ToDouble(e.Row.Cells[11].Value))
                    {
                        e.Row.Cells[3].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                        e.Row.Cells[3].Title = "Доля упала с прошлого года";
                    }
                    e.Row.Cells[3].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }

                if (i == 13 && e.Row.Cells[i].Value != null)
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
                e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                if (e.Row.Cells[14] != null &&  e.Row.Cells[14].Value != null && 
                        e.Row.Cells[14].Value.ToString() == "Раздел")
                {
                    e.Row.Cells[i].Style.Font.Bold = true;
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

        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(Label1.Text);

            //title = e.Section.AddText();
            //font = new Font("Verdana", 14);
            //title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            //title.AddContent(Label2.Text);
        }

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Label1.Text + " " + Label2.Text;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            e.CurrentWorksheet.Columns[0].Width = 400 * 37;
            e.CurrentWorksheet.Columns[1].Width = 100 * 37;
            e.CurrentWorksheet.Columns[2].Width = 200 * 37;
            e.CurrentWorksheet.Columns[3].Width = 200 * 37;
            e.CurrentWorksheet.Columns[4].Width = 200 * 37;
            e.CurrentWorksheet.Columns[5].Width = 200 * 37;
            e.CurrentWorksheet.Columns[6].Width = 200 * 37;
            e.CurrentWorksheet.Columns[7].Width = 200 * 37;
            e.CurrentWorksheet.Columns[8].Width = 200 * 37;
            e.CurrentWorksheet.Columns[9].Width = 200 * 37;
            e.CurrentWorksheet.Columns[10].Width = 200 * 37;
            e.CurrentWorksheet.Columns[11].Width = 200 * 37;
            e.CurrentWorksheet.Columns[12].Width = 200 * 37;
            e.CurrentWorksheet.Columns[13].Width = 200 * 37;

            e.CurrentWorksheet.Columns[1].CellFormat.FormatString = "00 00";
            e.CurrentWorksheet.Columns[2].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";
            e.CurrentWorksheet.Columns[3].CellFormat.FormatString = "0.00%";
            e.CurrentWorksheet.Columns[4].CellFormat.FormatString = "0.00%";
            e.CurrentWorksheet.Columns[5].CellFormat.FormatString = "0.00%";
            e.CurrentWorksheet.Columns[6].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";
            e.CurrentWorksheet.Columns[7].CellFormat.FormatString = "0.00%";
            e.CurrentWorksheet.Columns[8].CellFormat.FormatString = "0.00%";
            e.CurrentWorksheet.Columns[9].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";
            e.CurrentWorksheet.Columns[10].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";
            e.CurrentWorksheet.Columns[11].CellFormat.FormatString = "0.00%";
            e.CurrentWorksheet.Columns[12].CellFormat.FormatString = "0.00%";
            e.CurrentWorksheet.Columns[13].CellFormat.FormatString = "0.00%";
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