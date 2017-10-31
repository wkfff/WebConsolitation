using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Common.GridIndicatorRules;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
namespace Krista.FM.Server.Dashboards.reports.FNS_0009_0001
{
    public partial class Default : CustomReportPage
    {
        #region Поля

          private DataTable dtTable = new DataTable();
          private DataTable dtDate = new DataTable();
          private string query;

          private int year;
          private string month;

        #endregion

        #region Параметры запроса

        private CustomParam selectedPeriod;
       
        #endregion


        private int GetScreenWidth
        {
            get
            {
                if (Request.Cookies != null)
                {
                    if (Request.Cookies[CustomReportConst.ScreenWidthKeyName] != null)
                    {
                        HttpCookie cookie = Request.Cookies[CustomReportConst.ScreenWidthKeyName];
                        int value = Int32.Parse(cookie.Value);
                        return value;
                    }
                }
                return (int)Session["width_size"];
            }
        }

        private bool IsSmallResolution1200
        {
            get { return GetScreenWidth < 1200; }
        }

        private bool IsSmallResolution900
        {
            get { return GetScreenWidth < 900; }
        }
        
       protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Настройка грида

            GridBrick.Height = CustomReportConst.minScreenHeight / 2;
            GridBrick.Width = IsSmallResolution900 ? 730 : IsSmallResolution1200 ? 970 : CustomReportConst.minScreenWidth - 15;
            GridBrick.AutoSizeStyle = GridAutoSizeStyle.AutoHeight;
            GridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(Grid_InitializeLayout);
            GridBrick.Grid.InitializeRow +=new InitializeRowEventHandler(Grid_InitializeRow);

            #endregion

            #region Инициализация параметров

            selectedPeriod = UserParams.CustomParam("selected_period");
           
            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                ComboPeriod.Title = "Период";
                ComboPeriod.Width = 300;
                ComboPeriod.MultiSelect = false;
                ComboPeriod.ParentSelect = false;
                FillComboDate(ComboPeriod, "FNS_0009_0001_Date");
            }

            Page.Title = "Мониторинг налоговых доходов, получаемых за пользование объектами животного мира и водных биологических ресурсов";
            Label1.Text = Page.Title;
            Label2.Text = "Данные ежемесячного мониторинга налоговых доходов, получаемых за пользование объектами животного мира и водных биологических ресурсов";

            string template = "[Период__Период].[Период__Период].[Данные всех периодов].[{0}].[Полугодие {1}].[Квартал {2}].[{3}]";
            string[] dateElements = ComboPeriod.SelectedValue.Split(' ');
            CRHelper.SaveToErrorLog(dateElements[0]);
            CRHelper.SaveToErrorLog(dateElements[1]);
            CRHelper.SaveToErrorLog(dateElements[2]);

            year = Convert.ToInt32(dateElements[1]);
            month = dateElements[0];
            int quarter = CRHelper.QuarterNumByMonthNum(CRHelper.MonthNum(month));
            int halfYear = CRHelper.HalfYearNumByQuarterNum(quarter);
            selectedPeriod.Value = string.Format(template, year, halfYear, quarter, month);
            
            GridDataBind();
        }

        protected void FillComboDate(CustomMultiCombo combo, string queryName)
        {
            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Таблица", dtDate);

            if (dtDate.Rows.Count > 0)
            {
                Dictionary<string, int> dictDate = new Dictionary<string, int>();

                string str = string.Empty;

                for (int numRow = 0; numRow < dtDate.Rows.Count; numRow++)
                {
                    if (dtDate.Rows[numRow][3].ToString() == "0")
                    {
                        if (dtDate.Rows[numRow + 1] != null && dtDate.Rows[numRow + 1][3].ToString() == "1")
                        {
                            dictDate.Add(dtDate.Rows[numRow][1].ToString(), 0);
                            str = dtDate.Rows[numRow][1].ToString();
                        }
                    }
                    else
                    {
                        dictDate.Add(string.Format("{0} {1}а", dtDate.Rows[numRow][1], str), 1);
                    }
                }
                combo.FillDictionaryValues(dictDate);
                combo.SelectLastNode();
            }
        }


        #region Обработчики грида
        
        private void GridDataBind()
        {
            
             string query = DataProvider.GetQueryText("FNS_0009_0001_grid");
             dtTable = new DataTable();
             DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей",
                                                                                 dtTable);
             if (dtTable.Rows.Count > 0)
             {
                /* FontRowLevelRule levelRule = new FontRowLevelRule(dtTable.Columns.Count - 1);
                 levelRule.AddFontLevel("1",new Font(GridBrick.Grid.DisplayLayout.RowStyleDefault.Font.Name, 10,FontStyle.Bold));
                 levelRule.AddFontLevel("2",new Font(GridBrick.Grid.DisplayLayout.RowStyleDefault.Font.Name, 10,FontStyle.Bold));
                 GridBrick.AddIndicatorRule(levelRule);
                 */
                 GridBrick.DataTable = dtTable;
             }
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            
            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }
            
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = IsSmallResolution1200 || IsSmallResolution900 ? CRHelper.GetColumnWidth(115) : CRHelper.GetColumnWidth(190);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            int columnCount = e.Layout.Bands[0].Columns.Count;
            
            for (int i = 1; i < columnCount ; i++)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                e.Layout.Bands[0].Columns[i].Width = IsSmallResolution1200 ? CRHelper.GetColumnWidth(91) : IsSmallResolution900 ? CRHelper.GetColumnWidth(75) : CRHelper.GetColumnWidth(105);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            GridHeaderLayout headerLayout = GridBrick.GridHeaderLayout;

            headerLayout.AddCell("МР ГО");
            GridHeaderCell cell = headerLayout.AddCell("Сборы за пользование объектами животного мира и за пользование объектами водных биологических ресурсов");
            GridHeaderCell cell1 = cell.AddCell("Всего");
            cell1.AddCell("Начислено");
            cell1.AddCell("Получено");
            cell1.AddCell("Недоимка");
            GridHeaderCell cell2 = cell.AddCell("Сбор за пользование объектами животного мира");
            cell2.AddCell("Начислено");
            cell2.AddCell("Получено");
            cell2.AddCell("Недоимка");
            GridHeaderCell cell3 = cell.AddCell("Сбор за пользование объектами водных биологических ресурсов (по внутренним водным объектам)");
            cell3.AddCell("Начислено");
            cell3.AddCell("Получено");
            cell3.AddCell("Недоимка");
            GridBrick.GridHeaderLayout.ApplyHeaderInfo();
        }
        
        protected  void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i=1; i< e.Row.Cells.Count; i++)
            {
                UltraGridCell cell = e.Row.Cells[i];
                int n = i%3;
                double value = Convert.ToDouble(cell.Value);
                string str = string.Empty;
                switch (n)
                {
                    case 0:
                        {
                            str ="Недоимка";
                            break;
                        }
                    case 1:
                        {
                            str = "Начислено";
                            break;
                        }
                    case 2:
                        {
                            str = "Получено"; 
                            break;
                        }
                }
                cell.Title = string.Format("{0} {1} рублей на {2} {3} г.", str, value, month,year);
            }
        }

        #endregion
        
        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;
            ReportExcelExporter1.SheetColumnCount = 15;
            ReportExcelExporter1.GridColumnWidthScale = 1.2;

            Workbook workbook = new Workbook();

            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            ReportExcelExporter1.Export(GridBrick.GridHeaderLayout, sheet1, 3);
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
            ReportPDFExporter1.Export(GridBrick.GridHeaderLayout, section1);
        }

        #endregion
    }
}