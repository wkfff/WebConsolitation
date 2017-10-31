using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FNS_0001_0006
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private DataTable dtChart;
        private int firstYear = 2008;
        private int endYear = 2011;
        private string labelText;

        private DateTime currentDate;

        #endregion

        #region Параметры запроса

        // выбранная группа доходов КД
        private CustomParam selectedKDGroup;
        // Выбранный раздел ОКВЭД
        private CustomParam selectSection;

        // консолидированный элемент районов
        private CustomParam consolidateRegionElement;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.45);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";
            UltraWebGrid.ActiveRowChange += new ActiveRowChangeEventHandler(UltraWebGrid_ActiveRowChange);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);

            UltraChart1.Width = CRHelper.GetChartWidth(4 * CustomReportConst.minScreenWidth / 7 - 25);
            UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.50);
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            UltraChart2.Width = CRHelper.GetChartWidth(3 * CustomReportConst.minScreenWidth / 7 - 25);
            UltraChart2.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.50);
            UltraChart2.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            #region Настройка диаграммы

            UltraChart1.ChartType = ChartType.DoughnutChart;
            UltraChart1.DoughnutChart.RadiusFactor = 90;
            UltraChart1.Border.Thickness = 0;
            UltraChart1.DoughnutChart.OthersCategoryPercent = 0;
            UltraChart1.DoughnutChart.Labels.FormatString = "<PERCENT_VALUE:N2>%";
            UltraChart1.Tooltips.FormatString = "<ITEM_LABEL>\nС начала года: <DATA_VALUE:N2> тыс.руб.";
            CRHelper.FillCustomColorModel(UltraChart1, 6, true);

            UltraChart1.Legend.Visible = true;
            UltraChart1.Legend.Location = LegendLocation.Left;
            UltraChart1.Legend.SpanPercentage = 30;
            UltraChart1.Legend.Margins.Top = 0;
            UltraChart1.Legend.Margins.Left = 0;
            UltraChart1.Legend.Margins.Bottom = Convert.ToInt32((UltraChart2.Height.Value)) / 2;

            UltraChart1.TitleTop.Text = "";
            UltraChart1.TitleTop.Font = new Font("Verdana", 8);
            UltraChart1.TitleTop.HorizontalAlign = StringAlignment.Center;
            UltraChart1.TitleTop.Margins.Left = Convert.ToInt32((UltraChart1.Width.Value)) * UltraChart1.Legend.SpanPercentage / 100 + 5;
            UltraChart1.TitleTop.Visible = true;

            UltraChart2.ChartType = ChartType.DoughnutChart;
            UltraChart2.DoughnutChart.RadiusFactor = 90;
            UltraChart2.Border.Thickness = 0;
            UltraChart2.DoughnutChart.OthersCategoryPercent = 0;
            UltraChart2.DoughnutChart.Labels.FormatString = "<PERCENT_VALUE:N2>%";
            UltraChart2.Tooltips.FormatString = "<ITEM_LABEL>\nС начала года: <DATA_VALUE:N2> тыс.руб.";
            CRHelper.FillCustomColorModel(UltraChart2, 6, true);

            UltraChart2.Legend.Visible = false;

            UltraChart2.TitleTop.Text = "";
            UltraChart2.TitleTop.Font = new Font("Verdana", 8);
            UltraChart2.TitleTop.HorizontalAlign = StringAlignment.Center;
            UltraChart2.TitleTop.Visible = true;

            #endregion

            #region Инициализация параметров запроса

            selectedKDGroup = UserParams.CustomParam("selected_kd_group");
            selectSection = UserParams.CustomParam("selected_section");

            consolidateRegionElement = UserParams.CustomParam("consolidate_region_element");

            #endregion

            UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.IncomesKDRootName.Value = RegionSettingsHelper.Instance.IncomesKDRootName;
            consolidateRegionElement.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;

            if (!Page.IsPostBack)
            {
                chartWebAsyncPanel.AddRefreshTarget(chartHeaderLabel);
                chartWebAsyncPanel.AddRefreshTarget(UltraChart1);
                chartWebAsyncPanel.AddRefreshTarget(UltraChart2);
                chartWebAsyncPanel.AddLinkedRequestTrigger(UltraWebGrid);

                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FNS_0001_0006_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                string month = dtDate.Rows[0][3].ToString();

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

                FullComboOKVED();
                ComboOKVED.Title = "ОКВЭД";
                ComboOKVED.Width = 450;
                ComboOKVED.MultiSelect = false;
                ComboOKVED.ParentSelect = true;
                ComboOKVED.SetСheckedState("Все коды ОКВЭД", true);

                hiddenIndicatorLabel.Text = " ";
            }

            currentDate = new DateTime(Convert.ToInt32(ComboYear.SelectedValue), CRHelper.MonthNum(ComboMonth.SelectedValue), 1);
            currentDate = currentDate.AddMonths(1);

            Page.Title = String.Format("Структура доходов областного бюджета в разрезе видов деятельности: {0}", ComboOKVED.SelectedValue.ToLower());
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = String.Format("по состоянию на {0:dd.MM.yyyy} г.", currentDate);

            UltraChart1.TitleTop.Text = String.Format("На {0:dd.MM.yyyy} г.", currentDate.AddYears(-1));
            UltraChart2.TitleTop.Text = String.Format("На {0:dd.MM.yyyy} г.", currentDate);

            if (ComboMonth.SelectedValue == "Декабрь")
            {
                UserParams.PeriodLastYear.Value = (Convert.ToInt32(ComboYear.SelectedValue) - 2).ToString();
            }
            else
            {
                UserParams.PeriodLastYear.Value = currentDate.AddMonths(-1).AddYears(-1).Year.ToString();
            }

            // UserParams.PeriodLastYear.Value = currentDate.AddMonths(-1).AddYears(-1).Year.ToString();
            UserParams.PeriodYear.Value = currentDate.AddMonths(-1).Year.ToString();
            UserParams.PeriodMonth.Value = CRHelper.RusMonth(currentDate.AddMonths(-1).Month);
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(currentDate.AddMonths(-1).Month));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(currentDate.AddMonths(-1).Month));

            selectSection.Value = "[ОКВЭД].[Сопоставимый].[Все коды ОКВЭД].[Сельское хозяйство, охота и лесное хозяйство]";

            switch (ComboOKVED.SelectedValue)
            {
                case "С/х, охота и лесное хозяйств":
                    {
                        selectSection.Value = "[ОКВЭД].[Сопоставимый].[Все коды ОКВЭД].[Сельское хозяйство, охота и лесное хозяйство]";
                        break;
                    }
                case "Рыболовство, рыбоводство":
                    {
                        selectSection.Value = "[ОКВЭД].[Сопоставимый].[Все коды ОКВЭД].[Рыболовство, рыбоводство]";
                        break;
                    }
                case "Добыча полезных ископаемых":
                    {
                        selectSection.Value = "[ОКВЭД].[Сопоставимый].[Все коды ОКВЭД].[Добыча полезных ископаемых]";
                        break;
                    }
                case "Обрабатывающие производства":
                    {
                        selectSection.Value = "[ОКВЭД].[Сопоставимый].[Все коды ОКВЭД].[Обрабатывающие производства]";
                        break;
                    }
                case "Производство и распределение электро-и, газа и воды":
                    {
                        selectSection.Value = "[ОКВЭД].[Сопоставимый].[Все коды ОКВЭД].[Производство и распределение электроэнергии, газа и воды]";
                        break;
                    }
                case "Строительство":
                    {
                        selectSection.Value = "[ОКВЭД].[Сопоставимый].[Все коды ОКВЭД].[Строительство]";
                        break;
                    }
                case "Оптовая и розничная торговля":
                    {
                        selectSection.Value = "[ОКВЭД].[Сопоставимый].[Все коды ОКВЭД].[Оптовая и розничная торговля; ремонт автотранспортных средств, мотоциклов, бытовых изделий и предметов личного пользования]";
                        break;
                    }
                case "Гостиницы и рестораны":
                    {
                        selectSection.Value = "[ОКВЭД].[Сопоставимый].[Все коды ОКВЭД].[Гостиницы и рестораны]";
                        break;
                    }
                case "Транспорт и связь":
                    {
                        selectSection.Value = "[ОКВЭД].[Сопоставимый].[Все коды ОКВЭД].[Транспорт и связь]";
                        break;
                    }
                case "Финансовая деятельность":
                    {
                        selectSection.Value = "[ОКВЭД].[Сопоставимый].[Все коды ОКВЭД].[Финансовая деятельность]";
                        break;
                    }
                case "Операции с недвижимым имуществом":
                    {
                        selectSection.Value = "[ОКВЭД].[Сопоставимый].[Все коды ОКВЭД].[Операции с недвижимым имуществом, аренда и предоставление услуг]";
                        break;
                    }
                case "Гос. управление и обеспечение военной безопасности":
                    {
                        selectSection.Value = "[ОКВЭД].[Сопоставимый].[Все коды ОКВЭД].[Государственное управление и обеспечение военной безопасности; обязательное социальное обеспечение]";
                        break;
                    }
                case "Образование":
                    {
                        selectSection.Value = "[ОКВЭД].[Сопоставимый].[Все коды ОКВЭД].[Образование]";
                        break;
                    }
                case "Здравоохранение":
                    {
                        selectSection.Value = "[ОКВЭД].[Сопоставимый].[Все коды ОКВЭД].[Здравоохранение и предоставление социальных услуг]";
                        break;
                    }
                case "Прочие комм., соц. и перс. услуги":
                    {
                        selectSection.Value = "[ОКВЭД].[Сопоставимый].[Все коды ОКВЭД].[Предоставление прочих коммунальных, социальных и персональных услуг]";
                        break;
                    }
                case "Услуги по ведению домашнего хозяйства":
                    {
                        selectSection.Value = "[ОКВЭД].[Сопоставимый].[Все коды ОКВЭД].[Предоставление услуг по ведению домашнего хозяйства]";
                        break;
                    }
            }


            if (!chartWebAsyncPanel.IsAsyncPostBack)
            {
                UltraWebGrid.Bands.Clear();
                UltraWebGrid.DataBind();

                string patternValue = selectedKDGroup.Value;
                int defaultRowIndex = 0;
                if (patternValue == string.Empty)
                {
                    defaultRowIndex = 0;
                }

                if (UltraWebGrid.Columns.Count > 0 && UltraWebGrid.Rows.Count > 0)
                {
                    // ищем строку
                    UltraGridRow row = CRHelper.FindGridRow(UltraWebGrid, patternValue, UltraWebGrid.Columns.Count - 1, defaultRowIndex);
                    // выделяем строку
                    ActiveGridRow(row);
                }
            }

        }
        private void FullComboOKVED()
        {
            Dictionary<string, int> cods = new Dictionary<string, int>();
            cods.Add("Все коды ОКВЭД", 0);
            cods.Add("С/х, охота и лесное хозяйств", 1);
            cods.Add("Рыболовство, рыбоводство", 1);
            cods.Add("Добыча полезных ископаемых", 1);
            cods.Add("Обрабатывающие производства", 1);
            cods.Add("Производство и распределение электро-и, газа и воды", 1);
            cods.Add("Строительство", 1);
            cods.Add("Оптовая и розничная торговля", 1);
            cods.Add("Гостиницы и рестораны", 1);
            cods.Add("Транспорт и связь", 1);
            cods.Add("Финансовая деятельность", 1);
            cods.Add("Операции с недвижимым имуществом", 1);
            cods.Add("Гос. управление и обеспечение военной безопасности", 1);
            cods.Add("Образование", 1);
            cods.Add("Здравоохранение", 1);
            cods.Add("Прочие комм., соц. и перс. услуги", 1);
            cods.Add("Услуги по ведению домашнего хозяйства", 1);
            ComboOKVED.FillDictionaryValues(cods);
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

            string indicatorName = row.Cells[0].Text;

            hiddenIndicatorLabel.Text = row.Cells[row.Cells.Count - 1].Text;
            selectedKDGroup.Value = hiddenIndicatorLabel.Text;

            chartHeaderLabel.Text = indicatorName;

            UltraChart1.DataBind();
            UltraChart2.DataBind();
        }

        void UltraWebGrid_ActiveRowChange(object sender, RowEventArgs e)
        {
            ActiveGridRow(e.Row);
        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            if (ComboOKVED.SelectedValue == "Все коды ОКВЭД")
            {
                string query = DataProvider.GetQueryText("FNS_0001_0006_grid_allOKVED");
                dtGrid = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Доходы", dtGrid);
                if (dtGrid.Rows.Count > 0)
                {
                    foreach (DataRow row in dtGrid.Rows)
                    {
                        if (row[0] != DBNull.Value && row[0].ToString() != String.Empty)
                        {
                            row[0] = CRHelper.ToUpperFirstSymbol(row[0].ToString().ToLower());
                        }
                    }

                    UltraWebGrid.DataSource = dtGrid;
                }
                else
                {
                    UltraWebGrid.DataSource = null;
                }
            }
            else
            {
                string query = DataProvider.GetQueryText("FNS_0001_0006_grid_CurrentOKVED");
                dtGrid = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Доходы", dtGrid);

                if (dtGrid.Rows.Count > 0)
                {
                    foreach (DataRow row in dtGrid.Rows)
                    {
                        if (row[0] != DBNull.Value && row[0].ToString() != String.Empty)
                        {
                            row[0] = CRHelper.ToUpperFirstSymbol(row[0].ToString().ToLower());
                        }
                    }

                    for (int i = 1; i < dtGrid.Columns.Count - 1; i += 4)
                    {
                        for (int j = 0; j < dtGrid.Rows.Count; j++)
                        {
                            if (dtGrid.Rows[j][i] != DBNull.Value)
                            {
                                dtGrid.Rows[j][i] = Convert.ToDouble(dtGrid.Rows[j][i]) / 1000;
                            }
                            if (dtGrid.Rows[j][i + 1] != DBNull.Value)
                            {
                                dtGrid.Rows[j][i + 1] = Convert.ToDouble(dtGrid.Rows[j][i + 1]) / 1000;
                            }
                            if (dtGrid.Rows[j][i + 2] != DBNull.Value)
                            {
                                dtGrid.Rows[j][i + 2] = Convert.ToDouble(dtGrid.Rows[j][i + 2]) / 1000;
                            }
                        }
                    }


                    UltraWebGrid.DataSource = dtGrid;
                }
                else
                {
                    UltraWebGrid.DataSource = null;
                }
            }
        }


        void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            UltraWebGrid.Height = Unit.Empty;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;


            for (int i = 1; i < e.Layout.Bands[0].Columns.Count - 1; i = i + 1)
            {
                string formatString = "N0";
                int columnWidth = 60;

                int groupIndex = (i - 1) % 4;
                switch (groupIndex)
                {
                    case 0:
                    case 1:
                    case 2:
                        {
                            formatString = "N2";
                            columnWidth = 110;
                            break;
                        }
                    case 3:
                        {
                            formatString = "P2";
                            columnWidth = 80;
                            break;
                        }
                }

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(columnWidth);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(250);
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;


            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;
            if (ComboOKVED.SelectedValue == "Все коды ОКВЭД")
            {

                for (int i = 0; i < e.Layout.Bands[0].Columns.Count - 1; i++)
                {
                    if (i == 0)
                    {
                        e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                        e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                    }
                    else
                    {
                        e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                    }
                }


                int multiHeaderPos = 1;

                for (int i = 1; i < e.Layout.Bands[0].Columns.Count - 1; i = i + 4)
                {
                    ColumnHeader ch = new ColumnHeader(true);
                    string[] captions = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                    ch.Caption = captions[0];

                    if (ComboMonth.SelectedValue == "Декабрь")
                    {
                        CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, String.Format("на 01.01.{0} г., тыс.руб.", Convert.ToInt32(ComboYear.SelectedValue) - 1), String.Format("Данные по состоянию на 01.01.{0} г., тыс.руб.", currentDate.Year));
                    }
                    else
                    {
                        CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i,
                                                  String.Format("на 01.01.{0} г., тыс.руб.", currentDate.Year),
                                                  String.Format("Данные по состоянию на 01.01.{0} г., тыс.руб.",
                                                                currentDate.Year));
                    }

                    /* CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i,
                                               String.Format("на 01.01.{0} г., тыс.руб.", currentDate.Year),
                                               String.Format("Данные по состоянию на 01.01.{0} г., тыс.руб.",
                                                             currentDate.Year));
                     */
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1,
                                              String.Format("на {0:dd.MM.yyyy} г., тыс.руб.", currentDate.AddYears(-1)),
                                              String.Format("Данные по состоянию на {0:dd.MM.yyyy} г., тыс.руб.",
                                                            currentDate.AddYears(-1)));
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 2,
                                              String.Format("на {0:dd.MM.yyyy} г., тыс.руб.", currentDate),
                                              String.Format("Данные по состоянию на {0:dd.MM.yyyy} г., тыс.руб.",
                                                            currentDate));
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 3, "Темп роста",
                                              "Темп роста к аналогичному периоду предыдущего года");

                    ch.RowLayoutColumnInfo.OriginY = 0;
                    ch.RowLayoutColumnInfo.OriginX = multiHeaderPos;
                    multiHeaderPos += 4;
                    ch.RowLayoutColumnInfo.SpanX = 4;
                    ch.Style.Wrap = true;
                    e.Layout.Bands[0].HeaderLayout.Add(ch);
                }
            }

            else
            {
                for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    if (i == 0)
                    {
                        e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                        e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                    }
                    else
                    {
                        e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                    }
                }

                int multiHeaderPos = 1;

                for (int i = 1; i < e.Layout.Bands[0].Columns.Count - 1; i = i + 4)
                {
                    ColumnHeader ch = new ColumnHeader(true);
                    string[] captions = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');

                    if (captions[0].Contains("Подраздел"))
                    {
                        captions[0] = captions[0].ToUpper();
                        /*captions[0] = captions[0].Replace("Подраздел - ", "");
                        captions[0] = captions[0].Insert(captions[0].Length, ", всего");
                         * */
                    }
                    if (i == e.Layout.Bands[0].Columns.Count - 5)
                    {
                        captions[0] = captions[0].ToUpper();
                    }

                    ch.Caption = captions[0];
                    if (ComboMonth.SelectedValue == "Декабрь")
                    {
                        CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, String.Format("на 01.01.{0} г., тыс.руб.", Convert.ToInt32(ComboYear.SelectedValue) - 1), String.Format("Данные по состоянию на 01.01.{0} г., тыс.руб.", currentDate.Year));
                    }
                    else
                    {
                        CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i,
                                                  String.Format("на 01.01.{0} г., тыс.руб.", currentDate.Year),
                                                  String.Format("Данные по состоянию на 01.01.{0} г., тыс.руб.",
                                                                currentDate.Year));
                    }
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1,
                                              String.Format("на {0:dd.MM.yyyy} г., тыс.руб.",
                                                            currentDate.AddYears(-1)),
                                              String.Format("Данные по состоянию на {0:dd.MM.yyyy} г., тыс.руб.",
                                                            currentDate.AddYears(-1)));
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 2,
                                              String.Format("на {0:dd.MM.yyyy} г., тыс.руб.", currentDate),
                                              String.Format("Данные по состоянию на {0:dd.MM.yyyy} г., тыс.руб.",
                                                            currentDate));
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 3, "Темп роста",
                                              "Темп роста к аналогичному периоду предыдущего года");

                    ch.RowLayoutColumnInfo.OriginY = 0;
                    ch.RowLayoutColumnInfo.OriginX = multiHeaderPos;
                    multiHeaderPos += 4;
                    ch.RowLayoutColumnInfo.SpanX = 4;
                    ch.Style.Wrap = true;
                    e.Layout.Bands[0].HeaderLayout.Add(ch);

                }

                /* string str1 = string.Empty;
                 string str2 = string.Empty;
                 string str = string.Empty;

                int ind0 = 1; int ind = 1;

                for (int i=1; i< e.Layout.Bands[0].Columns.Count - 1; i = i + 4)
                {
                    str1 = dtGrid.Rows[0][i].ToString();
                    str2 = dtGrid.Rows[0][i + 4].ToString();

                    if (str1 == str2) // если подраздел один и тот же
                    {
                        ind += 4;
                    }
                    else
                    {
                        string[] captions = e.Layout.Bands[0].Columns[i+4].Header.Caption.Split(';');
                        str = captions[0]; // класс
                        if (str1==str)
                        {
                            ind += 4;
                            string cap = str1.Replace("Подраздел - ", "");
                            CRHelper.AddHierarchyHeader(UltraWebGrid, 0, cap, ind0, 0, ind - 1, 1);
                            ind0 = ind;
                        }
                        else
                        {
                          ind += 4;
                          string cap = str1.Replace("Подраздел - ", "");
                          CRHelper.AddHierarchyHeader(UltraWebGrid, 0, cap,ind0,0,ind-1,1);
                          ind0 = ind;
                        }
                    }
                   
                }*/

            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {


            for (int i = 0; i < e.Row.Cells.Count - 1; i++)
            {
                int groupIndex = (i - 1) % 4;
                bool growRateColumn = (groupIndex == 3);

                if (growRateColumn && e.Row.Cells[i].Value != null &&
                    e.Row.Cells[i].Value.ToString() != string.Empty)
                {
                    if (Convert.ToDouble(e.Row.Cells[i].Value) < 1)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                        e.Row.Cells[i].Title = "Снижение к прошлому отчетному году";
                    }
                    else if (Convert.ToDouble(e.Row.Cells[i].Value) > 1)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                        e.Row.Cells[i].Title = "Рост к прошлому отчетному году";
                    }
                    e.Row.Cells[i].Style.CustomRules =
                        "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }



                UltraGridCell cell = e.Row.Cells[i];
                if (e.Row.Cells[0].Value != null && e.Row.Cells[0].Value.ToString() == "Итого ")
                {
                    cell.Style.Font.Size = 9;
                    cell.Style.Font.Bold = true;
                }

                if (cell.Value != null && cell.Value.ToString() != String.Empty)
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
            /*   if (ComboOKVED.SelectedValue != "Все коды ОКВЭД")
               {
                 /* if (e.Row.Index == 0)
                  {
                      e.Row.Hidden = true;
                  }

                  if (e.Row.Index != 0)
                   {
                       for (int i = 1; i < e.Row.Cells.Count-1; i++)
                       {
                           string formatString = "N0";


                           int groupIndex = (i - 1) % 4;
                           switch (groupIndex)
                           {
                               case 0:
                               case 1:
                               case 2:
                                   {
                                       formatString = "{0:N2}";

                                       break;
                                   }
                               case 3:
                                   {
                                       formatString = "{0:P2}";

                                       break;
                                   }
                           }
                           if (e.Row.Cells[i].Value != null && e.Row.Cells[i].ToString() != string.Empty)
                           {
                               double value = Convert.ToDouble(e.Row.Cells[i].Value);
                               e.Row.Cells[i].Value = string.Format(formatString, value);
                               
                           }
                           e.Row.Cells[i].Style.HorizontalAlign = HorizontalAlign.Right;
                       }
                  // }
               }*/

        }

        #endregion

        #region Обработчики диаграммы

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            if (ComboOKVED.SelectedValue == "Все коды ОКВЭД") // если выбраны все коды ОКВЭД
            {
                string query = sender == UltraChart1
                                   ? DataProvider.GetQueryText("FNS_0001_0006_chart1")
                                   : DataProvider.GetQueryText("FNS_0001_0006_chart2");
                dtChart = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

                foreach (DataRow row in dtChart.Rows)
                {
                    if (row[0] != DBNull.Value && row[0].ToString() != String.Empty)
                    {
                        row[0] = DataDictionariesHelper.GetShortOKVDName(row[0].ToString());
                    }
                }

                ((UltraChart)sender).DataSource = dtChart;
            }
            else
            {
                string query = sender == UltraChart1
                                  ? DataProvider.GetQueryText("FNS_0001_0006_chart1_CurrentOkved")
                                  : DataProvider.GetQueryText("FNS_0001_0006_chart2_CurrentOkved");
                dtChart = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

                foreach (DataRow row in dtChart.Rows)
                {
                    if (row[0] != DBNull.Value && row[0].ToString() != String.Empty)
                    {
                        row[0] = DataDictionariesHelper.GetShortOKVDName(row[0].ToString());
                    }
                }

                ((UltraChart)sender).DataSource = dtChart;
            }



        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text + " " + PageSubTitle.Text;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            int columnCount = UltraWebGrid.Columns.Count - 1;
            int rowsCount = UltraWebGrid.Rows.Count;

            e.CurrentWorksheet.Columns[0].CellFormat.FormatString = "";
            e.CurrentWorksheet.Columns[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            e.CurrentWorksheet.Columns[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            e.CurrentWorksheet.Columns[0].Width = 200 * 37;
            /* if (ComboOKVED.SelectedValue == "Все коды ОКВЭД")
             {*/
            // расставляем стили у ячеек хидера
            for (int i = 1; i < columnCount; i++)
            {
                e.CurrentWorksheet.Rows[beginExportIndex - 1].Height = 20 * 37;
                e.CurrentWorksheet.Rows[beginExportIndex - 1].Cells[i].CellFormat.VerticalAlignment =
                    VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[beginExportIndex - 1].Cells[i].CellFormat.Alignment =
                    HorizontalCellAlignment.Center;
                e.CurrentWorksheet.Rows[beginExportIndex - 1].Cells[i].CellFormat.WrapText =
                    ExcelDefaultableBoolean.True;

                e.CurrentWorksheet.Rows[beginExportIndex].Height = 20 * 37;
                e.CurrentWorksheet.Rows[beginExportIndex].Cells[i].CellFormat.VerticalAlignment =
                    VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[beginExportIndex].Cells[i].CellFormat.Alignment =
                    HorizontalCellAlignment.Center;
                e.CurrentWorksheet.Rows[beginExportIndex].Cells[i].CellFormat.WrapText =
                    ExcelDefaultableBoolean.True;

                string formatString = "";
                int columnWidth = 100;

                int groupIndex = (i - 1) % 4;
                switch (groupIndex)
                {
                    case 0:
                    case 1:
                    case 2:
                        {
                            columnWidth = 100;
                            formatString = "#,##0.00;[Red]-#,##0.00";
                            break;
                        }
                    case 3:
                        {
                            columnWidth = 80;
                            formatString = "#,##0.00%;[Red]-#,##0.00%";
                            break;
                        }
                }

                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = formatString;
                e.CurrentWorksheet.Columns[i].CellFormat.Alignment = HorizontalCellAlignment.Right;
                e.CurrentWorksheet.Columns[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Columns[i].Width = columnWidth * 37;
            }
            /* }
             else
             {
                  columnCount = UltraWebGrid.Columns.Count - 1;
                  rowsCount = UltraWebGrid.Rows.Count;

                 for (int i = 1; i < columnCount; i++ )
                 {
                     e.CurrentWorksheet.Rows[4].Height = 20 * 37;
                     e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.VerticalAlignment =VerticalCellAlignment.Center;
                     e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.Alignment =HorizontalCellAlignment.Center;
                     e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.WrapText =ExcelDefaultableBoolean.True;

                     e.CurrentWorksheet.Rows[3].Height = 20 * 37;
                     e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.VerticalAlignment =VerticalCellAlignment.Center;
                     e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.Alignment =HorizontalCellAlignment.Center;
                     e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.WrapText =ExcelDefaultableBoolean.True;
                 }

                 for (int j = 1; j < rowsCount+4; j++)
                 {
                     for (int i = 1; i < columnCount; i++)
                     {
                         string formatString = "";
                         int columnWidth = 100;

                         int groupIndex = (i - 1)%4;
                         switch (groupIndex)
                         {
                             case 0:
                             case 1:
                             case 2:
                                 {
                                     columnWidth = 100;
                                     formatString = "#,##0.00;[Red]-#,##0.00";
                                     break;
                                 }
                             case 3:
                                 {
                                     columnWidth = 80;
                                     formatString = "#,##0.00%;[Red]-#,##0.00%";
                                     break;
                                 }
                         }
                         e.CurrentWorksheet.Columns[i].Width = columnWidth*37;
                         e.CurrentWorksheet.Columns[i].CellFormat.Alignment = HorizontalCellAlignment.Right;
                         e.CurrentWorksheet.Columns[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                         e.CurrentWorksheet.Rows[j].Cells[i].CellFormat.FormatString = formatString;
                     }
                 }


             }*/

            // расставляем стили у начальных колонок
            for (int i = beginExportIndex; i < rowsCount + beginExportIndex; i++)
            {
                e.CurrentWorksheet.Rows[i].Height = 30 * 37;
            }
        }

        private int beginExportIndex = 4;

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = beginExportIndex;
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Структура доходов ");
            Worksheet sheet2 = workbook.Worksheets.Add("Налог на прибыль организаций ");
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            sheet2.Rows[0].Cells[0].Value = PageTitle.Text + " " + PageSubTitle.Text;
            sheet2.Rows[1].Cells[0].Value = chartHeaderLabel.Text;
            UltraGridExporter.ChartExcelExport(sheet2.Rows[2].Cells[0], UltraChart1);
            UltraGridExporter.ChartExcelExport(sheet2.Rows[2].Cells[14], UltraChart2);

            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid, sheet1);
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            e.HeaderText = UltraWebGrid.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex].Header.Key.Split(';')[0];
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
            title.AddContent(PageTitle.Text + " " + PageSubTitle.Text);
        }

        void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {
            e.Section.AddPageBreak();

            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 58);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(PageTitle.Text + " " + PageSubTitle.Text);

            title = e.Section.AddText();
            font = new Font("Verdana", 56);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(chartHeaderLabel.Text);

            ITable table = e.Section.AddTable();
            ITableRow row = table.AddRow();
            ITableCell cell = row.AddCell();

            System.IO.MemoryStream imageStream = new System.IO.MemoryStream();
            UltraChart1.SaveTo(imageStream, System.Drawing.Imaging.ImageFormat.Png);
            Infragistics.Documents.Reports.Graphics.Image image = (new Bitmap(imageStream)).ScaleImageIg(9);
            cell.Width = new FixedWidth((float)UltraChart1.Width.Value * 5);
            cell.AddImage(image);

            cell = row.AddCell();
            imageStream = new System.IO.MemoryStream();
            UltraChart2.SaveTo(imageStream, System.Drawing.Imaging.ImageFormat.Png);
            image = (new Bitmap(imageStream)).ScaleImageIg(9);
            cell.AddImage(image);
        }

        #endregion
    }
}
