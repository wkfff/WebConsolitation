using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using EndExportEventArgs=Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs;
using InitializeRowEventHandler=Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler;

namespace Krista.FM.Server.Dashboards.reports.FO_0005_0003
{
    public partial class Default : CustomReportPage
    {
        private static bool IsSmallResolution
        {
            get { return CRHelper.GetScreenWidth < 900; }
        }
        
        #region Поля

        private DataTable dtGrid = new DataTable();
        private DataTable dtChart = new DataTable();
        private int firstYear = 2011;
        private int endYear;

        private GridHeaderLayout headerLayout;

        #endregion

        #region Параметры запроса

        // множество выбранных лет
        private CustomParam AdminType;
        private CustomParam Year;
      
        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

           
            #region Настройка диаграммы

            UltraChart1.ChartType = ChartType.PieChart; // круговая диаграмма
            System.Drawing.Font font = new System.Drawing.Font("Verdana", 9);
            UltraChart1.Border.Thickness = 0;
            UltraChart1.Axis.X.Extent = 130;
            UltraChart1.Axis.Y.Extent = 130;
            UltraChart1.PieChart.Labels.Font = font;
            UltraChart1.Tooltips.FormatString = "<ITEM_LABEL>\n<DATA_VALUE:N0> тыс. руб.";
            UltraChart1.Legend.Visible = true;
            UltraChart1.Legend.Location = LegendLocation.Bottom;
            UltraChart1.Legend.SpanPercentage = 13;
           // UltraChart1.Legend.Margins.Right = Convert.ToInt32(UltraChart1.Width.Value / 2);
            UltraChart1.Legend.Font = font;
            UltraChart1.PieChart.RadiusFactor = 60;
            UltraChart1.PieChart.StartAngle = 245;
            UltraChart1.Data.SwapRowsAndColumns = true;
            UltraChart1.PieChart.OthersCategoryPercent = 0;
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart1.ChartDrawItem += new ChartDrawItemEventHandler(UltraChart1_ChartDrawItem);

            #endregion

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth / 1.8);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight/1.4);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth /2.3);
            UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight /1.45);

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
           
           #region Инициализация параметров
            if (AdminType == null)
            {
                AdminType = CustomParam.CustomParamFactory("admin_type");
            }

            if (Year == null)
            {
                Year = CustomParam.CustomParamFactory("year");
            }

            #endregion 
        }

        void UltraChart1_ChartDrawItem(object sender, ChartDrawItemEventArgs e)
        {
            //устанавливаем ширину текста легенды 
            Text text = e.Primitive as Text;
            if ((text != null) && !(string.IsNullOrEmpty(text.Path)) && text.Path.EndsWith("Legend"))
            {
                int widthLegendLabel;

                if ((UltraChart1.Legend.Location == LegendLocation.Top) || (UltraChart1.Legend.Location == LegendLocation.Bottom))
                {
                    widthLegendLabel = (int)UltraChart1.Width.Value - 20;
                }
                else
                {
                    widthLegendLabel = ((int)UltraChart1.Legend.SpanPercentage * (int)UltraChart1.Width.Value / 100) - 20;
                }

                widthLegendLabel -= UltraChart1.Legend.Margins.Left + UltraChart1.Legend.Margins.Right;
                if (text.labelStyle.Trimming != StringTrimming.None)
                {
                    text.bounds.Width = widthLegendLabel;
                }
            }
        }        
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!Page.IsPostBack)
            {
                string query = DataProvider.GetQueryText("FO_0005_0003_date");
                DataTable dtDate = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query,dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                Year.Value = 2011.ToString();
                FillAdmin();
                ComboBill.Width = 550;
                ComboBill.Title = "ГРБС";
                ComboBill.FillDictionaryValues(admin);
                ComboBill.MultiSelect = false;
                ComboBill.ParentSelect = true;
                ComboBill.SetСheckedState("Все ГРБС", true);
             }

            PageTitle.Text = "Удельный вес утвержденного объема расходов областного бюджета, формируемого в рамках планово-прогнозных документов в общем утвержденном объеме расходов";
            Page.Title = "Удельный вес утвержденного объема расходов областного бюджета";
            PageSubTitle.Text = String.Format("{0}, данные за {1} год", ComboBill.SelectedValue, ComboYear.SelectedValue);

            int year = Convert.ToInt32(ComboYear.SelectedValue);
            UserParams.PeriodYear.Value = year.ToString();
            Year.Value = ComboYear.SelectedValue;

            if (ComboBill.SelectedValue == "Все ГРБС")
             {
                AdminType.Value = string.Format(" [Администратор__АС Бюджет].[Администратор__АС Бюджет].[Данные всех источников].[ФО АС Бюджет - УФиНП {0}]", year);
                chart1ElementCaption.Text =
                     string.Format("Удельный вес ведомственных целевых программ в общем утвержденном объеме расходов областного бюджета на {0} год", year);
             }
            else
            {
                string[] name = ComboBill.SelectedValue.Split(')');
                string fullName = ComboBill.SelectedValue.Replace(name[0], "");
                fullName = fullName.Replace(") ", "");
                AdminType.Value = string.Format("[Администратор__АС Бюджет].[Администратор__АС Бюджет].[Данные всех источников].[ФО АС Бюджет - УФиНП {0}].[{1}]", year, fullName);
                chart1ElementCaption.Text = string.Format("{1}. Удельный вес ведомственных целевых программ в утвержденном объеме расходов областного бюджета на {0} год", year, fullName);
             } 

            headerLayout = new GridHeaderLayout(UltraWebGrid);

            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
            UltraChart1.DataBind();
        }

        #region Обработчики грида

       protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0005_0003_Grid");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtGrid);

            if (dtGrid.Rows.Count > 0)
            {
                double sum=0;
                for (int i = 0; i < dtGrid.Rows.Count; i++)
                {
                    string[] caption = dtGrid.Rows[i][0].ToString().Split(';');
                    dtGrid.Rows[i][0] = caption[0];

                    if ( caption[0].Contains("Строительство и реконструкция   автомобильных  дорог  и  дорожных  сооружений  общего пользования") ||
                         caption[0].Contains("Капитальный ремонт автомобильных дорог  и  дорожных сооружений  общего пользования") ||
                         caption[0].Contains("Ремонт автомобильных дорог и дорожных сооружений общего пользования") ||
                         caption[0].Contains("Строительство, модернизация, ремонт и содержание автомобильных дорог общего пользования, в том числе дорог в поселениях (за исключением автомобильных дорог федерального значения") ||
                         caption[0].Contains("Содержание автомобильных дорог общего пользования и сооружений на них") ||
                         caption[0].Contains("Субсидии на объекты дорожной инфраструктуры, отнесенные к муниципальной собственности") ||
                         caption[0].Contains("НИОКР для нужд дорожно-строительного комплекса") ||
                         caption[0].Contains("Проектно-сметная документация для нужд дорожно-строительного комплекса") ||
                         caption[0].Contains("Управление дорожным хозяйством") ||
                         caption[0].Contains("Выполнение работ по инвентаризации и паспортизации автомобильных дорог и дорожных сооружений на них") ||
                         caption[0].Contains("Резерв средств") ||
                         caption[0].Contains("Планово-предупредительный ремонт автомобильных дорог общего пользования и сооружений на них")
                       )
                    {
                        dtGrid.Rows[i][0] = "Ведомственная целевая программа \"Развитие автомобильных дорог в Новосибирской области в 2011 - 2013 годах\"";
                    }

                    if (dtGrid.Rows[i][5] != DBNull.Value && dtGrid.Rows[i][5].ToString() != string.Empty)
                    {
                       sum = sum + Convert.ToDouble(dtGrid.Rows[i][5]);
                    }

                }
                DataRow row = dtGrid.NewRow();
                row[0] = "Итого";
                row[1] = DBNull.Value;
                row[2] = DBNull.Value;
                row[3] = DBNull.Value;
                row[4] = DBNull.Value;
                row[5] = sum;
                dtGrid.Rows.Add(row);

                dtGrid.AcceptChanges();
                UltraWebGrid.DataSource = dtGrid;
            }
        }
        
       protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.Bands[0].HeaderStyle.Wrap = true;
            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].MergeCells = true;

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(250);
            
            headerLayout.AddCell("Наименование программы");
            headerLayout.AddCell("ГРБС","Код ГРБС");
            headerLayout.AddCell("РзПр","Код РзПр");
            headerLayout.AddCell("ЦСР", "Код ЦСР");
            headerLayout.AddCell("ВР", "Код ВР");
            headerLayout.AddCell("Утвержденный план, тыс. руб.", "Утвержденный годовой план");

            headerLayout.ApplyHeaderInfo();

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(60);
              
            }
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "000");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "00 00");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "000 00 00");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "000");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "N2");
            e.Layout.Bands[0].Columns[5].Width = CRHelper.GetColumnWidth(107);
             
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            e.Row.Cells[0].Style.HorizontalAlign = HorizontalAlign.Center;
            e.Row.Cells[0].Style.VerticalAlign = VerticalAlign.Middle;
            e.Row.Cells[0].Style.BackColor = Color.WhiteSmoke;

            if (e.Row.Cells[0].Value.ToString() == "Итого")
            {
                e.Row.Style.Font.Bold = true;
                e.Row.Cells[0].Style.HorizontalAlign = HorizontalAlign.Left;
                e.Row.Cells[0].Style.VerticalAlign = VerticalAlign.NotSet;
            }
        }

       #endregion

        #region Обработчики диаграммы

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0005_0003_Chart");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Диаграмма", dtChart);
            UltraChart1.DataSource = dtChart;
        }

       #endregion

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");
            Worksheet sheet2 = workbook.Worksheets.Add("sheet2");

            ReportExcelExporter1.HeaderCellHeight = 30;
            ReportExcelExporter1.GridColumnWidthScale = 1.1;
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
            ReportExcelExporter1.Export(UltraChart1,chart1ElementCaption.Text, sheet2, 3);
        }
      
        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(Object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

            Report report = new Report();
            ISection section1 = report.AddSection();
            ISection section2 = report.AddSection();

            ReportPDFExporter1.HeaderCellHeight = 50;
            ReportPDFExporter1.Export(headerLayout, section1);
            ReportPDFExporter1.Export(UltraChart1, chart1ElementCaption.Text ,section2);
            }

        #endregion

       // private Collection<string> admin = new Collection<string>();
        private Dictionary<string, int> admin = new Dictionary<string, int>(); 
        private Dictionary<string, string> adminDict = new Dictionary<string, string>();

        private void FillAdmin()
        {
            DataTable dtAdmin = new DataTable();
            string query = DataProvider.GetQueryText("FO_0005_0003_adminType");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dtAdmin);

            admin.Add("Все ГРБС", 0);
            foreach (DataRow row in dtAdmin.Rows)
            {
                admin.Add(String.Format("{0}", row[1]), 0);
                adminDict.Add(String.Format("({0}) {1}", row[1], row[0]), row[0].ToString());
            }
        }
    }
}