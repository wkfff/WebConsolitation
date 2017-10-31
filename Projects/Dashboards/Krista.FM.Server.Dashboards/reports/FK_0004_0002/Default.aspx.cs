using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI.WebControls;
using Infragistics.Web.UI.GridControls;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FK_0004_0002
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable grbsGridDt = new DataTable();
        private DataTable restChartDt = new DataTable();
        private DataTable dynamicChartDt = new DataTable();
        private DataTable dynamicChartDtVvp = new DataTable();
        private DataTable dtChartLimit = new DataTable();
        private DataTable RealDynamicChartDt = new DataTable();
        private DataTable RealDynamicChartDtVvp = new DataTable();

        private DateTime currentDate;
        private DateTime lastDate;

        #endregion

        protected global::Infragistics.Web.UI.GridControls.WebDataGrid WebDataGrid1;

        protected string xAxisCategories;
        protected string series;
        protected string formatter;
        protected string valueSuffix;
        protected string titleText;

        protected string xAxisCategories2;
        protected string dynamicSeries;

        #region Параметры запроса

        // выбранный период
        private CustomParam selectedPeriod;
        // выбранный показатель таблицы
        private CustomParam selectedGridIndicator;

        #endregion

        private bool IsMlnRubSelected
        {
            get { return RubMiltiplierButtonList.SelectedIndex == 0; }
        }

        private string multiplierCaption;

        // выбранный множитель рублей
        private CustomParam rubMultiplier;

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            CrossLink1.Text = "Доходы&nbsp;сектора&nbsp;гос.&nbsp;управления";
            CrossLink1.NavigateUrl = "http://www.budget.ifinmon.ru/index.php/razdely/dokhody/fk-0004-0003";
            CrossLink1.Target = "_blank";

            CrossLink2.Text = "Расходы&nbsp;сектора&nbsp;гос.&nbsp;управления";
            CrossLink2.NavigateUrl = "http://www.budget.ifinmon.ru/index.php/razdely/raskhody/fk-0004-0004";
            CrossLink2.Target = "_blank";

            #region Настройка грида

            WebDataGrid1.Height = Unit.Empty;
            WebDataGrid1.Width = (int)HttpContext.Current.Session["width_size"] < 1100 ?
                CRHelper.GetGridWidth(Convert.ToInt32(CustomReportConst.minScreenWidth - 25)) :
                Unit.Empty;
            WebDataGrid1.InitializeRow += new Infragistics.Web.UI.GridControls.InitializeRowEventHandler(WebDataGrid1_InitializeRow);
        //    WebDataGrid1.ColumnSorted += new ColumnSortedHandler(WebDataGrid1_ColumnSorted);

            #endregion
                        
            multiplierCaption = IsMlnRubSelected ? " млн.руб." : " млрд.руб.";

            #region Инициализация параметров запроса

            selectedPeriod = UserParams.CustomParam("selected_period");
            selectedGridIndicator = UserParams.CustomParam("selected_grid_indicator");

            rubMultiplier = UserParams.CustomParam("rub_multiplier");
            rubMultiplier.Value = IsMlnRubSelected ? "1000000" : "1000000000";

            #endregion

            //ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            //ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }
        
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                Dictionary<string, int> kinds = new Dictionary<string, int>();
                kinds.Add("Доходы", 0);
                kinds.Add("Расходы", 0);
                kinds.Add("Дефицит/Профицит", 0);
                kinds.Add("Источники внутреннего финансирования", 0);
                kinds.Add("Источники внешнего финансирования", 0);
                ComboKind.Title = "Показатель";
                ComboKind.Width = 400;
                ComboKind.MultiSelect = false;
                ComboKind.FillDictionaryValues(kinds);
                if (selectedGridIndicator.Value != null &&
                    selectedGridIndicator.Value != String.Empty)
                {
                    ComboKind.SetСheckedState(selectedGridIndicator.Value, true);
                }
                else
                {
                    ComboKind.SetСheckedState("Доходы", true);
                }
                ComboKind.AutoPostBack = true;

                CustomCalendar1.WebCalendar.SelectedDate = CubeInfo.GetLastDate(DataProvidersFactory.SecondaryMASDataProvider, "FK_0004_0002_lastDate");
            }
            
            selectedGridIndicator.Value = ComboKind.SelectedValue;
            currentDate = CustomCalendar1.WebCalendar.SelectedDate;
            lastDate = currentDate.AddYears(-1);
            selectedPeriod.Value = CRHelper.PeriodMemberUName("[Период].[Период]", currentDate, 5);
            UserParams.PeriodYear.Value = currentDate.Year.ToString();

            if (Request.Form["__EVENTTARGET"] != null &&
                Request.Form["__EVENTTARGET"].Contains("ComboKind"))
            {
                Response.Redirect("~/reports/fk_0004_0002/default.aspx#chart");
            }

            Page.Title = String.Format("Характеристика показателей сектора государственного управления (без учета средств от приносящей доход деятельности)");
            Label1.Text = Page.Title;
            Label2.Text = String.Format("по состоянию на <b>{0:dd.MM.yyyy} г., {1}</b>", currentDate.AddDays(1), multiplierCaption);
            Label3.Text = String.Format("{0} сектора государственного управления РФ на <b>{1:dd.MM} {1:yyyy}-{2:yyyy}гг., {3}</b>", ComboKind.SelectedValue.ToLower().ToUpperFirstSymbol(), currentDate.AddDays(1), lastDate.AddDays(1), multiplierCaption);

            DynamicChartCaption.Text = String.Format("Соотношение показателей исполнения бюджетов бюджетной системы РФ на {0:dd.MM.yyyy} г., {1}", currentDate.AddDays(1), multiplierCaption);

            GridDataBind();
            DynamicChartDataBind();
            RealDynamicChartDataBind();
        }

        #region Обработчики грида

        private void GridDataBind()
        {
            string query = vvp.Checked ? DataProvider.GetQueryText("FK_0004_0002_grid_vvp") : DataProvider.GetQueryText("FK_0004_0002_grid");
            grbsGridDt = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", grbsGridDt);

            if (grbsGridDt.Rows.Count > 0)
            {
                if (grbsGridDt.Columns.Count > 0)
                {
                    grbsGridDt.Columns.RemoveAt(0);
                }

                foreach (DataRow row in grbsGridDt.Rows)
                {
                    if (row[0] == DBNull.Value)
                    {
                        row[0] = "% к ВВП";
                    }
                    else if (row[0].ToString() == "ДОХОДЫ")
                    {
                        row[0] = "<a href='http://www.budget.ifinmon.ru/index.php/razdely/dokhody/fk-0004-0003' target='blank'>Доходы</a>";
                    }
                    else if (row[0].ToString() == "РАСХОДЫ")
                    {
                        row[0] = "<a href='http://www.budget.ifinmon.ru/index.php/razdely/raskhody/fk-0004-0004' target='blank'>Расходы</a>";
                    }
                }

                WebDataGrid1.DataSource = grbsGridDt;

                if (!Page.IsPostBack)
                {
                    WebDataGrid1.Columns.Clear();
                    WebDataGrid1.Behaviors.CreateBehavior<Sorting>();

                    BoundDataField col = new BoundDataField();
                    col.Key = "0";
                    col.DataFieldName = grbsGridDt.Columns[0].ColumnName;
                    col.HtmlEncode = false;
                    col.Header.Text = "Показатели";
                    col.Width = GetColumnWidth(130, 1250);
                    WebDataGrid1.Columns.Add(col);

                    col = new BoundDataField();
                    col.Key = "1";
                    col.DataFieldName = grbsGridDt.Columns[1].ColumnName;
                    col.HtmlEncode = false;
                    col.Header.Text = "Консолидированный бюджет РФ и государственных внебюджетных фондов";
                    col.Width = GetColumnWidth(120, 1250);
                    col.CssClass = "ValueCell";
                    col.DataFormatString = "{0:N2}";
                    WebDataGrid1.Columns.Add(col);

                    GroupField field = new GroupField();
                    field.Key = "Федеральный уровень бюджетной системы";
                    field.Header.Text = "Федеральный уровень бюджетной системы";

                    col = new BoundDataField();
                    col.Key = "2";
                    col.DataFieldName = grbsGridDt.Columns[2].ColumnName;
                    col.HtmlEncode = false;
                    col.Header.Text = "Федеральный бюджет";
                    col.Width = GetColumnWidth(80, 1250);
                    col.CssClass = "ValueCell";
                    col.DataFormatString = "{0:N2}";
                    field.Columns.Add(col);

                    col = new BoundDataField();
                    col.Key = "3";
                    col.DataFieldName = grbsGridDt.Columns[3].ColumnName;
                    col.HtmlEncode = false;
                    col.Header.Text = "Бюджеты государственных внебюджетных фондов";
                    col.Width = GetColumnWidth(100, 1250);
                    col.CssClass = "ValueCell";
                    col.DataFormatString = "{0:N2}";
                    field.Columns.Add(col);

                    WebDataGrid1.Columns.Add(field);

                    field = new GroupField();
                    field.Key = "Региональный уровень бюджетной системы";
                    field.Header.Text = "Региональный уровень бюджетной системы";

                    col = new BoundDataField();
                    col.Key = "4";
                    col.DataFieldName = grbsGridDt.Columns[4].ColumnName;
                    col.HtmlEncode = false;
                    col.Header.Text = "Бюджеты субъектов РФ";
                    col.Width = GetColumnWidth(80, 1250);
                    col.CssClass = "ValueCell";
                    col.DataFormatString = "{0:N2}";
                    field.Columns.Add(col);

                    col = new BoundDataField();
                    col.Key = "5";
                    col.DataFieldName = grbsGridDt.Columns[5].ColumnName;
                    col.HtmlEncode = false;
                    col.Header.Text = "Бюджеты территориальных государственных внебюджетных фондов (бюджеты территориальных фондов ОМС)";
                    col.Width = GetColumnWidth(100, 1250);
                    col.CssClass = "ValueCell";
                    col.DataFormatString = "{0:N2}";
                    field.Columns.Add(col);

                    WebDataGrid1.Columns.Add(field);

                    field = new GroupField();
                    field.Key = "Муниципальный уровень бюджетной системы";
                    field.Header.Text = "Муниципальный уровень бюджетной системы";

                    col = new BoundDataField();
                    col.Key = "6";
                    col.DataFieldName = grbsGridDt.Columns[6].ColumnName;
                    col.HtmlEncode = false;
                    col.Header.Text = "Районные бюджеты муниципальных районов";
                    col.Width = GetColumnWidth(90, 1250);
                    col.CssClass = "ValueCell";
                    col.DataFormatString = "{0:N2}";
                    field.Columns.Add(col);

                    col = new BoundDataField();
                    col.Key = "7";
                    col.DataFieldName = grbsGridDt.Columns[7].ColumnName;
                    col.HtmlEncode = false;
                    col.Header.Text = "Бюджеты городских округов";
                    col.Width = GetColumnWidth(80, 1250);
                    col.CssClass = "ValueCell";
                    col.DataFormatString = "{0:N2}";
                    field.Columns.Add(col);

                    col = new BoundDataField();
                    col.Key = "8";
                    col.DataFieldName = grbsGridDt.Columns[8].ColumnName;
                    col.HtmlEncode = false;
                    col.Header.Text = "Бюджеты городских муниципальных образований городов федерального значения Москвы и СПб";
                    col.Width = GetColumnWidth(90, 1250);
                    col.CssClass = "ValueCell";
                    col.DataFormatString = "{0:N2}";
                    field.Columns.Add(col);

                    col = new BoundDataField();
                    col.Key = "9";
                    col.DataFieldName = grbsGridDt.Columns[9].ColumnName;
                    col.HtmlEncode = false;
                    col.Header.Text = "Бюджеты городских и сельских поселений";
                    col.Width = GetColumnWidth(80, 1250);
                    col.CssClass = "ValueCell";
                    col.DataFormatString = "{0:N2}";
                    field.Columns.Add(col);

                    WebDataGrid1.Columns.Add(field);

                    col = new BoundDataField();
                    col.Key = "10";
                    col.DataFieldName = grbsGridDt.Columns[10].ColumnName;
                    col.HtmlEncode = false;
                    WebDataGrid1.Columns.Add(col);

                    WebDataGrid1.Columns[WebDataGrid1.Columns.Count - 1].Hidden = true;

                    //if ((int)HttpContext.Current.Session["width_size"] < 1350)
                    //{
                    //    WebDataGrid1.Behaviors.CreateBehavior<ColumnFixing>();
                    //    WebDataGrid1.Behaviors.ColumnFixing.AutoAdjustCells = true;
                    //    WebDataGrid1.Behaviors.ColumnFixing.FixedColumns.Add(WebDataGrid1.Columns[0]);
                    //}
                }

            }
        }

        void WebDataGrid1_InitializeRow(object sender, Infragistics.Web.UI.GridControls.RowEventArgs e)
        {
            if (e.Row.Items[0].Value.ToString() == "% к ВВП")
            {
                e.Row.Items[0].CssClass = "ValueCell";

                for (int i = 1; i < e.Row.Items.Count - 1; i++)
                {
                    if (e.Row.Items[i].Value != null)
                    {
                        double value;
                        if (Double.TryParse(e.Row.Items[i].Value.ToString(), out value))
                        {
                            e.Row.Items[i].Text = value.ToString("P2");
                        }
                    }
                }
            }

            for (int i = 1; i < e.Row.Items.Count - 1; i++)
            {
                if (e.Row.Items[i].Value != null)
                {
                    double value;
                    if (Double.TryParse(e.Row.Items[i].Value.ToString(), out value))
                    {
                        if (value < 0)
                        {
                            e.Row.Items[i].CssClass = "NegativeValue";
                        }
                    }
                }
            }
        }

        #endregion

        #region Обработчики диаграммы динамики

        private void DynamicChartDataBind()
        {
            dynamicChartDt = new DataTable();
            dynamicChartDtVvp = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("FK_0004_0002_dynamicChart"), "Наименование показателей", dynamicChartDt);
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("FK_0004_0002_dynamicChart_vvp"), "Наименование показателей", dynamicChartDtVvp);
                        
            if (vvp.Checked)
            {
                series = InitializeDynamicChartData(dynamicChartDtVvp);
                formatter = "this.value + '%'";
                valueSuffix = "%";
                titleText = String.Empty;
            }
            else
            {
                series = InitializeDynamicChartData(dynamicChartDt);
                formatter = "this.value";
                valueSuffix = multiplierCaption;
                titleText = multiplierCaption;
            }

            for (int rowCount = dynamicChartDt.Rows.Count - 1; rowCount >= 0; rowCount--)
            {
                xAxisCategories = String.Format("{0}, '{1}'", xAxisCategories, dynamicChartDt.Rows[rowCount][0]);
            }
            xAxisCategories = xAxisCategories.TrimStart(',');
        }

        private string InitializeDynamicChartData(DataTable dt)
        {
            string chartSeries = String.Empty;
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                string data = String.Empty;

                for (int rowCount = dt.Rows.Count - 1; rowCount >= 0; rowCount--)
                {
                    string value = dt.Rows[rowCount][i] == DBNull.Value || dt.Rows[rowCount][i].ToString() == String.Empty ?
                        "0" :
                        dt.Rows[rowCount][i].ToString().Replace(',', '.');
                    data = String.Format("{0}, {1}", data, value);
                }
                data = data.TrimStart(',');

                chartSeries = String.Format("{0}, {{ name: '{1}', data: [{2}] }}", chartSeries, dt.Columns[i].ColumnName, data);                
            }
            return chartSeries.TrimStart(',');
        }

       

        #endregion

        #region Обработчики диаграммы динамики

        private void RealDynamicChartDataBind()
        {
            RealDynamicChartDt = new DataTable();
            RealDynamicChartDtVvp = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("FK_0004_0002_realdynamicChart"), "Наименование показателей", RealDynamicChartDt);
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("FK_0004_0002_realdynamicChartVpp"), "Наименование показателей", RealDynamicChartDtVvp);

            RealDynamicChartDt.Columns[1].ColumnName = lastDate.AddDays(1).ToString("dd.MM.yyyy") + " года";
            RealDynamicChartDt.Columns[2].ColumnName = currentDate.AddDays(1).ToString("dd.MM.yyyy") + " года";

            RealDynamicChartDtVvp.Columns[1].ColumnName = lastDate.AddDays(1).ToString("dd.MM.yyyy") + " года";
            RealDynamicChartDtVvp.Columns[2].ColumnName = currentDate.AddDays(1).ToString("dd.MM.yyyy") + " года";
                        
            if (vvp.Checked)
            {
                dynamicSeries = InitializeDynamicChartData(RealDynamicChartDtVvp);
                formatter = "this.value + '%'";
                valueSuffix = "%";
                titleText = String.Empty;
            }
            else
            {
                dynamicSeries = InitializeDynamicChartData(RealDynamicChartDt);
                formatter = "this.value";
                valueSuffix = multiplierCaption;
                titleText = multiplierCaption;
            }
            
            for (int rowCount = 0; rowCount < RealDynamicChartDt.Rows.Count; rowCount++)
            {
                xAxisCategories2 = String.Format("{0}, '{1}'", xAxisCategories2, RealDynamicChartDt.Rows[rowCount][0]);
            }
            xAxisCategories2 = xAxisCategories2.TrimStart(',');
        }

        
        
        #endregion

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            //ReportExcelExporter1.WorksheetTitle = Label1.Text;
            //ReportExcelExporter1.WorksheetSubTitle = Label2.Text;
            //ReportExcelExporter1.SheetColumnCount = 15;
            //ReportExcelExporter1.GridColumnWidthScale = 1.2;

            //Workbook workbook = new Workbook();

            //Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            //ReportExcelExporter1.Export(GRBSGridBrick.GridHeaderLayout, sheet1, 3);

            //Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма");
            //ChartControl.Width = Convert.ToInt32(ChartControl.Width.Value * 0.8);
            //ReportExcelExporter1.Export(ChartControl, DynamicChartCaption.Text, sheet2, 3);
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            //ReportPDFExporter1.PageTitle = Label1.Text;
            //ReportPDFExporter1.PageSubTitle = Label2.Text;
            //ReportPDFExporter1.HeaderCellHeight = 50;

            //Report report = new Report();

            //ISection section1 = report.AddSection();
            //ReportPDFExporter1.Export(GRBSGridBrick.GridHeaderLayout, section1);

            //ISection section2 = report.AddSection();
            //ChartControl.Width = Convert.ToInt32(ChartControl.Width.Value * 0.8);
            //ReportPDFExporter1.Export(ChartControl, DynamicChartCaption.Text, section2);
        }

        #endregion

        public static int GetColumnWidth(double defaultWidth, int startExpandWidth)
        {
            // Если ширины достаточная
            if ((int)HttpContext.Current.Session["width_size"] > startExpandWidth)
            {
                // Увеличиваем колонки на размер превышения
                defaultWidth =
                    defaultWidth * (int)HttpContext.Current.Session["width_size"] / startExpandWidth;
            }
            string browser = HttpContext.Current.Request.Browser.Browser;
            switch (browser)
            {
                case ("Firefox"):
                    {
                        return (int)(defaultWidth / 0.980);
                    }
                case ("AppleMAC-Safari"):
                    {
                        return (int)(defaultWidth * 1.15);
                    }
            }
            return (int)defaultWidth;
        }
    }
}