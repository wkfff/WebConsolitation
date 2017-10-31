using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using Krista.FM.Server.Dashboards.Core;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs;
using Infragistics.UltraChart.Core.Primitives;


namespace Krista.FM.Server.Dashboards.reports.FK_0002_0002
{
    public partial class Default: CustomReportPage
    {
        #region Поля

        private DataTable dtGrid;
        private DataTable dtDate;

        private DataTable dtChart1;
        private DataTable dtChart2;

        private string month;
        private int firstYear = 2005;
        private int endYear=2011;

        private DateTime currentDate;
        private GridHeaderLayout headerLayout;
       
        #endregion

        #region

        public CustomParam levelBudget;
       
        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15);
            UltraWebGrid1.Height = Unit.Empty;
            UltraWebGrid1.DisplayLayout.NoDataMessage = "Нет данных";

            UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth/2 - 30);
            UltraChart1.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight/2);

            UltraChart2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth/2 - 30);
            UltraChart2.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight/2);

            #region инициализация параметров
            if (levelBudget == null)
             {
                 levelBudget = UserParams.CustomParam("level_budget");
             }
          
            #endregion

            #region  Настройка диаграмм

            System.Drawing.Font font = new System.Drawing.Font("Verdana", 10);

            UltraChart2.ChartType = ChartType.StackColumnChart;
            UltraChart2.Border.Thickness = 0;
            UltraChart2.Axis.X.Extent = 70;
            UltraChart2.Axis.Y.Extent = 70;
            UltraChart2.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart2.Axis.X.Labels.Visible = false;
            UltraChart2.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
         
            UltraChart2.Data.SwapRowsAndColumns = false;
            UltraChart2.TitleLeft.Visible = true;
            UltraChart2.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart2.TitleLeft.Text = "Млн. руб.";

            UltraChart2.Legend.Visible = true;
            UltraChart2.Legend.Location = LegendLocation.Top;
            UltraChart2.Legend.SpanPercentage = 10;
            UltraChart2.Legend.Font = font;
            UltraChart2.Axis.X.Labels.SeriesLabels.Font = font;
            UltraChart2.Axis.Y.Labels.Font = font;

            UltraChart2.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart2.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart2_FillSceneGraph);
            

            
            UltraChart1.ChartType = ChartType.DoughnutChart;
            UltraChart1.ColorModel.ModelStyle = ColorModels.PureRandom;

            UltraChart1.Legend.Visible = true;
            UltraChart1.Legend.SpanPercentage = 28;
            UltraChart1.Legend.Margins.Bottom = 0;
            UltraChart1.Legend.Margins.Top = 0;
            UltraChart1.Legend.Margins.Left = 0;
            UltraChart1.Legend.Margins.Right = 0;
            UltraChart1.Legend.Location = LegendLocation.Bottom;
            UltraChart1.Legend.Font = font;
            UltraChart1.DoughnutChart.Labels.Font = font;

            UltraChart1.DoughnutChart.ShowConcentricLegend = false;
            UltraChart1.DoughnutChart.Concentric = true;
            UltraChart1.DoughnutChart.OthersCategoryPercent = 0;

            CalloutAnnotation planAnnotation = new CalloutAnnotation();
            planAnnotation.Text = "Назначено";
            planAnnotation.Width = 90;
            planAnnotation.Height = 20;
            planAnnotation.TextStyle.Font = font;
            planAnnotation.Location.Type = LocationType.Percentage;
            planAnnotation.Location.LocationX = 50;
            planAnnotation.Location.LocationY = 52;

            CalloutAnnotation factAnnotation = new CalloutAnnotation();
            factAnnotation.Text = "Исполнено";
            factAnnotation.Width = 90;
            factAnnotation.Height = 20;
            factAnnotation.TextStyle.Font = font;
            factAnnotation.Location.Type = LocationType.Percentage;
            factAnnotation.Location.LocationX = 50;
            factAnnotation.Location.LocationY = 10;

            UltraChart1.Annotations.Add(planAnnotation);
            UltraChart1.Annotations.Add(factAnnotation);
        
            UltraChart1.ChartDrawItem += new ChartDrawItemEventHandler(UltraChart2_ChartDrawItem);
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            
            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }
        void UltraChart2_ChartDrawItem(object sender, ChartDrawItemEventArgs e)
        {
            //устанавливаем ширину текста легенды 
            Text text = e.Primitive as Text;
            if ((text != null) && !(string.IsNullOrEmpty(text.Path)) && text.Path.EndsWith("Legend"))
            {
                int widthLegendLabel;

                if ((UltraChart2.Legend.Location == LegendLocation.Top) || (UltraChart2.Legend.Location == LegendLocation.Bottom))
                {
                    widthLegendLabel = (int)UltraChart2.Width.Value - 20;
                }
                else
                {
                    widthLegendLabel = ((int)UltraChart2.Legend.SpanPercentage * (int)UltraChart2.Width.Value / 100) - 20;
                }

                widthLegendLabel -= UltraChart2.Legend.Margins.Left + UltraChart2.Legend.Margins.Right;
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
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FK_0002_0002_date");
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

                if (dtDate != null && dtDate.Rows.Count > 0)
                {
                    endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                    month = dtDate.Rows[0][3].ToString();
                }

                UserParams.PeriodYear.Value = endYear.ToString();
                UserParams.PeriodMonth.Value = month;
            
                ComboYear.Title = "Год";
                ComboYear.Visible = true;
                ComboYear.Width = 100;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear,endYear));
                ComboYear.MultiSelect = false;
                ComboYear.SetСheckedState(UserParams.PeriodYear.Value, true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 150;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.MultiSelect = false;
                ComboMonth.SetСheckedState(UserParams.PeriodMonth.Value, true);

                FillComboBudget();
                ComboBudget.Title = "Уровень бюджета";
                ComboBudget.Visible = true;
                ComboBudget.Width = 410;
                ComboBudget.MultiSelect = false;
                ComboBudget.ParentSelect = true;
                ComboBudget.SetСheckedState("Консолидированный бюджет субъекта",true);

                ComboRzPR.Title = "РзПр";
                ComboRzPR.Width = 400;
                ComboRzPR.Visible = true;
                ComboRzPR.MultiSelect = false;
                ComboRzPR.ParentSelect = true;
                ComboRzPR.FillDictionaryValues(CustomMultiComboDataHelper.FillFKRNames(
                                                  DataDictionariesHelper.OutcomesFKRTypes,
                                                  DataDictionariesHelper.OutcomesFKRLevels));
                ComboRzPR.SetСheckedState("Расходы бюджета - ИТОГО", true);

                UserParams.KDGroup.Value = DataDictionariesHelper.OutcomesFKRTypes[ComboRzPR.SelectedValue];
            }

            currentDate = new DateTime(Convert.ToInt32(ComboYear.SelectedValue), CRHelper.MonthNum(ComboMonth.SelectedValue), 1);
            currentDate = currentDate.AddMonths(1);
            
            gridCaptionElement.Text = string.Empty;
 
            Page.Title = "Сравнение расходов субъектов Сибирского федерального округа по разделам/подразделам классификации расходов";
            PageTitle.Text = Page.Title;
             
            PageSubTitle.Text =string.Format("Консолидированный бюджет субъекта, РзПр \"{0}\", данные по состоянию на {1:dd.MM.yyyy} г.",ComboRzPR.SelectedValue,currentDate) ;
            if (ComboBudget.SelectedIndex == 1)
            {
                PageSubTitle.Text = string.Format("Бюджет субъект,  РзПр \"{0}\", данные по состоянию на {1:dd.MM.yyyy} г.", ComboRzPR.SelectedValue, currentDate);
            }
            else if (ComboBudget.SelectedIndex==2)
            {
                PageSubTitle.Text = string.Format("Консолидированный бюджет МО,  РзПр \"{0}\", данные по состоянию на {1:dd.MM.yyyy} г.", ComboRzPR.SelectedValue, currentDate);
            }

            UltraChart1.Tooltips.FormatString = string.Format("Доля  <PERCENT_VALUE:#0.00>% \n <ITEM_LABEL> \n  <DATA_VALUE:N2> млн. руб \n {0}", ComboRzPR.SelectedValue);
            UltraChart2.Tooltips.FormatString = string.Format("<ITEM_LABEL>");

           int year = Convert.ToInt32(ComboYear.SelectedValue);

            UserParams.PeriodLastYear.Value = (year - 1).ToString();
            UserParams.PeriodYear.Value = year.ToString();
            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            UserParams.PeriodHalfYear.Value =
                string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodQuater.Value =
                string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.KDGroup.Value = DataDictionariesHelper.OutcomesFKRTypes[ComboRzPR.SelectedValue];

            levelBudget.Value = "[Уровни бюджета].[СКИФ].[Все].[Конс.бюджет субъекта]";
            
            switch (ComboBudget.SelectedValue)
            {
                case "Консолидированный бюджет субъекта":
                    {
                        levelBudget.Value = "[Уровни бюджета].[СКИФ].[Все].[Конс.бюджет субъекта]";
                        break;
                    }
                case "Бюджет субъекта":
                    {
                        levelBudget.Value = "[Уровни бюджета].[СКИФ].[Все].[Бюджет субъекта]";
                        break;
                    }
                case "Консолидированный бюджет МО":
                    {
                        levelBudget.Value = "[Уровни бюджета].[СКИФ].[Все].[Конс.бюджет МО]";
                        break;
                    }
            }
           headerLayout = new GridHeaderLayout(UltraWebGrid1);
           UltraWebGrid1.Bands.Clear();
           UltraWebGrid1.DataBind();

            chart1ElementCaption.Text = "Сравнение плановых назначений и фактического исполнения по расходам консолидированных бюджетов субъектов Сибирского федерального округа ";
            chart2ElementCaption.Text = "Сравнение абсолютного исполнения расходов субъектов Сибирского федерального округа";
            if (ComboBudget.SelectedIndex == 1)
            {
              chart1ElementCaption.Text = "Сравнение плановых назначений и фактического исполнения по расходам бюджетов субъектов Сибирского федерального округа ";
            }
            else if (ComboBudget.SelectedIndex == 2)
            {
              chart1ElementCaption.Text = "Сравнение плановых назначений и фактического исполнения по расходам консолидированных бюджетов МО Сибирского федерального округа ";
            }

           UltraChart1.DataBind();
           UltraChart2.DataBind();
           
        }

        private void FillComboBudget()
         {
            Dictionary <string, int> levels = new Dictionary<string, int>();
            levels.Add("Консолидированный бюджет субъекта",0);
            levels.Add("Бюджет субъекта",1);
            levels.Add("Консолидированный бюджет МО",1);
            ComboBudget.FillDictionaryValues(levels);
         }


        #region Обработчик грида


        protected void UltraWebGrid_DataBinding(Object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FK_0002_0002_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query,"Субъект",dtGrid);
            for (int i = 0; i < dtGrid.Rows.Count; i++)
            {
               if (dtGrid.Rows[i][1] != DBNull.Value)
                    {
                        dtGrid.Rows[i][1] = Convert.ToDouble(dtGrid.Rows[i][1]) / 1000000;

                    }
               if (dtGrid.Rows[i][6] != DBNull.Value)
                    {
                        dtGrid.Rows[i][6] = Convert.ToDouble(dtGrid.Rows[i][6]) / 1000000;

                    }
            }
             
            UltraWebGrid1.DataSource = dtGrid;
        }


        protected void UltraWebGrid_InitializeLayout(Object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

           /* for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
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
            */
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(220);

            e.Layout.Bands[0].Columns[11].Hidden = true;
            e.Layout.Bands[0].Columns[12].Hidden = true;
            e.Layout.Bands[0].Columns[13].Hidden = true;
            e.Layout.Bands[0].Columns[14].Hidden = true;
            e.Layout.Bands[0].Columns[15].Hidden = true;
            e.Layout.Bands[0].Columns[16].Hidden = true;

            headerLayout.AddCell("Субъект","",2);

            GridHeaderCell cell0 = headerLayout.AddCell(string.Format("{0}",ComboRzPR.SelectedValue), "Раздел/ Подраздел ");
            cell0.AddCell(string.Format("Назначено,<br/> млн. руб."), string.Format("Годовые назначения на {0} год", ComboYear.SelectedValue));
            cell0.AddCell(string.Format("Доля в общих расходах ФО,<br/> %"), "Доля субъекта в общей сумме расходов всех субъектов ФО");
            cell0.AddCell(string.Format("Доля в общих расходах РФ,<br/> %"), "Доля субъекта в общей сумме расходов всех субъектов РФ");
            cell0.AddCell(string.Format("Ранг ФО <br/>(к плану)"), "Ранг (место) субъекта по доле в расходах Сибирского федерального округа, расчет по плановым назначениям");
            cell0.AddCell(string.Format("Ранг РФ <br/>(к плану)"), "Ранг (место) субъекта по доле в расходах Российской Федерации, расчет по плановым назначениям");
            cell0.AddCell(string.Format("Исполнено,<br/> млн.руб."), string.Format("Исполнение по расходам нарастающим итогом с начала года по состоянию на {0:dd.MM.yyyy}", currentDate));
            cell0.AddCell(string.Format("Доля в общих расходах ФО,<br/> %"), "Доля субъекта в общей сумме расходов всех субъектов ФО");
            cell0.AddCell(string.Format("Доля в общих расходах РФ,<br/> %"), "Доля субъекта в общей сумме расходов всех субъектов РФ");
            cell0.AddCell(string.Format("Ранг ФО <br/>(к исполнению)"), "Ранг (место) субъекта по доле в расходах Сибирского федерального округа, расчет по фактическому исполнению");
            cell0.AddCell(string.Format("Ранг РФ <br/>(к исполнению)"), "Ранг (место) субъекта по доле в расходах Российской федерации, расчет по фактическому исполнению");
        

          /*  CRHelper.SetHeaderCaption(UltraWebGrid1, 0, 1, string.Format("Назначено,<br/> млн. руб."), string.Format("Годовые назначения на {0} год",ComboYear.SelectedValue));
            CRHelper.SetHeaderCaption(UltraWebGrid1, 0, 2, string.Format("Доля в общих расходах ФО,<br/> %"), "Доля субъекта в общей сумме расходов всех субъектов ФО");
            CRHelper.SetHeaderCaption(UltraWebGrid1, 0, 3, string.Format("Доля в общих расходах РФ,<br/> %"), "Доля субъекта в общей сумме расходов всех субъектов РФ");
            CRHelper.SetHeaderCaption(UltraWebGrid1, 0, 4, string.Format("Ранг ФО <br/>(к плану)"), "Ранг (место) субъекта по доле в расходах Сибирского федерального округа, расчет по плановым назначениям");
            CRHelper.SetHeaderCaption(UltraWebGrid1, 0, 5, string.Format("Ранг РФ <br/>(к плану)"), "Ранг (место) субъекта по доле в расходах Российской Федерации, расчет по плановым назначениям");
            CRHelper.SetHeaderCaption(UltraWebGrid1, 0, 6, string.Format("Исполнено,<br/> млн.руб."), string.Format("Исполнение по расходам нарастающим итогом с начала года по состоянию на {0:dd.MM.yyyy}", currentDate));
            CRHelper.SetHeaderCaption(UltraWebGrid1, 0, 7, string.Format("Доля в общих расходах ФО,<br/> %"), "Доля субъекта в общей сумме расходов всех субъектов ФО");
            CRHelper.SetHeaderCaption(UltraWebGrid1, 0, 8, string.Format("Доля в общих расходах РФ,<br/> %"), "Доля субъекта в общей сумме расходов всех субъектов РФ");
            CRHelper.SetHeaderCaption(UltraWebGrid1, 0, 9, string.Format("Ранг ФО <br/>(к исполнению)"), "Ранг (место) субъекта по доле в расходах Сибирского федерального округа, расчет по фактическому исполнению");
            CRHelper.SetHeaderCaption(UltraWebGrid1, 0, 10, string.Format("Ранг РФ <br/>(к исполнению)"), "Ранг (место) субъекта по доле в расходах Российской федерации, расчет по фактическому исполнению");
            */
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");
            e.Layout.Bands[0].Columns[1].Width = 100;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "P2");
            e.Layout.Bands[0].Columns[2].Width = 100;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "P2");
            e.Layout.Bands[0].Columns[3].Width = 100;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N0");
            e.Layout.Bands[0].Columns[4].Width = 70;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "N0");
            e.Layout.Bands[0].Columns[5].Width = 70;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[6], "N2");
            e.Layout.Bands[0].Columns[6].Width = 100;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[7], "P2");
            e.Layout.Bands[0].Columns[7].Width = 100;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[8], "P2");
            e.Layout.Bands[0].Columns[8].Width = 100;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[9], "N0");
            e.Layout.Bands[0].Columns[9].Width = 100;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[10], "N0");
            e.Layout.Bands[0].Columns[10].Width = 100;

         //   CRHelper.AddHierarchyHeader(UltraWebGrid1, 0, string.Format("{0}", ComboRzPR.SelectedValue), 1, 0, 10, 1);
            headerLayout.ApplyHeaderInfo();
        }

        protected void UltraWebGrid_InitializeRow(Object sender, RowEventArgs e )
        {
            foreach (UltraGridCell cell in e.Row.Cells)
            {
                if (e.Row.Cells[0].Value != null && e.Row.Cells[0].Value.ToString() != string.Empty)
                {
                    if (!RegionsNamingHelper.IsSubject(e.Row.Cells[0].Value.ToString()))
                    {
                        cell.Style.Font.Bold = true;
                    }
                }
            }
            if (RegionsNamingHelper.IsSubject(e.Row.Cells[0].Value.ToString()))
            {
               for (int i = 1; i < UltraWebGrid1.Columns.Count; i++)
                {
                    if (i == 4)
                    {
                        if (e.Row.Cells[i] != null || e.Row.Cells[i].ToString() != string.Empty)
                        {
                            int value = Convert.ToInt32(e.Row.Cells[i].Value);
                            if (value == 1)
                            {
                                e.Row.Cells[i].Style.BackgroundImage = "~/images/starYellowBB.png";
                                e.Row.Cells[i].Title = "Самая высокая доля в расходах ФО";
                            }
                            else if (value == Convert.ToInt32(e.Row.Cells[i + 7].Value))
                            {
                                e.Row.Cells[i].Style.BackgroundImage = "~/images/starGrayBB.png";
                                e.Row.Cells[i].Title = "Самая низкая доля в расходах ФО";
                            }

                            e.Row.Cells[i].Style.CustomRules =
                                "background-repeat: no-repeat; background-position: left center; margin: 2px";
                        }
                    }
                    if (i == 9)
                    {
                        if (e.Row.Cells[i] != null || e.Row.Cells[i].ToString() != string.Empty)
                        {
                            int value = Convert.ToInt32(e.Row.Cells[i].Value);
                            if (value == 1)
                            {
                                e.Row.Cells[i].Style.BackgroundImage = "~/images/starYellowBB.png";
                                e.Row.Cells[i].Title = "Самая высокая доля в расходах ФО";
                            }
                            else if (value == Convert.ToInt32(e.Row.Cells[12].Value))
                            {
                                e.Row.Cells[i].Style.BackgroundImage = "~/images/starGrayBB.png";
                                e.Row.Cells[i].Title = "Самая низкая доля в расходах ФО";
                            }
                            e.Row.Cells[i].Style.CustomRules =
                                "background-repeat: no-repeat; background-position: left center; margin: 2px";
                        }
                    }
                    if (i == 5)
                    {
                        if (e.Row.Cells[i] != null || e.Row.Cells[i].ToString() != string.Empty)
                        {
                            int value = Convert.ToInt32(e.Row.Cells[i].Value);
                            if (value == Convert.ToInt32(e.Row.Cells[14].Value))
                            {
                                e.Row.Cells[i].Style.BackgroundImage = "~/images/starYellowBB.png";
                                e.Row.Cells[i].Title = "Самая высокая доля в расходах РФ";
                            }
                            else if (value == Convert.ToInt32(e.Row.Cells[13].Value))
                            {
                                e.Row.Cells[i].Style.BackgroundImage = "~/images/starGrayBB.png";
                                e.Row.Cells[i].Title = "Самая низкая доля в расходах РФ";
                            }
                            e.Row.Cells[i].Style.CustomRules =
                                "background-repeat: no-repeat; background-position: left center; margin: 2px";
                        }
                    }

                    if (i == 10)
                    {
                        if (e.Row.Cells[i] != null || e.Row.Cells[i].ToString() != string.Empty)
                        {
                            int value = Convert.ToInt32(e.Row.Cells[i].Value);
                            if (value == Convert.ToInt32(e.Row.Cells[16].Value))
                            {
                                e.Row.Cells[i].Style.BackgroundImage = "~/images/starYellowBB.png";
                                e.Row.Cells[i].Title = "Самая высокая доля в расходах РФ";
                            }
                            else if (value == Convert.ToInt32(e.Row.Cells[15].Value))
                            {
                                e.Row.Cells[i].Style.BackgroundImage = "~/images/starGrayBB.png";
                                e.Row.Cells[i].Title = "Самая низкая доля в расходах РФ";
                            }
                            e.Row.Cells[i].Style.CustomRules =
                                "background-repeat: no-repeat; background-position: left center; margin: 2px";
                        }
                    }
                }
            }
        }

        #endregion

        #region Обработчики диаграмм

        protected void UltraChart1_DataBinding(Object sender, EventArgs e)
          {
              string query = DataProvider.GetQueryText("FK_0002_0002_chart1");
              dtChart1 = new DataTable();
              DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query,"Диаграмма1",dtChart1);

              for (int i = 0; i < dtChart1.Rows.Count; i++)
              {
                  if (dtChart1.Rows[i][1] != DBNull.Value)
                  {
                      dtChart1.Rows[i][1] = Convert.ToDouble(dtChart1.Rows[i][1]) / 1000000;

                  }
                  if (dtChart1.Rows[i][2] != DBNull.Value)
                  {
                      dtChart1.Rows[i][2] = Convert.ToDouble(dtChart1.Rows[i][2]) / 1000000;

                  }
              }
              
              UltraChart1.DataSource = dtChart1;
          }

          protected void UltraChart2_DataBinding(Object sender, EventArgs e)
          {
              string query = DataProvider.GetQueryText("FK_0002_0002_chart2");
              dtChart2 = new DataTable();
              DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query,"Диаграмма2",dtChart2);


              for (int i = 0; i < dtChart2.Rows.Count; i++)
              {
                  if (dtChart2.Rows[i][1] != DBNull.Value)
                  {
                      dtChart2.Rows[i][1] = Convert.ToDouble(dtChart2.Rows[i][1]) / 1000000;

                  }
                  if (dtChart2.Rows[i][2] != DBNull.Value)
                  {
                      dtChart2.Rows[i][2] = Convert.ToDouble(dtChart2.Rows[i][2]) / 1000000;

                  }
              }

              RegionsNamingHelper.ReplaceRegionNames(dtChart2, 0);

              UltraChart2.Data.SwapRowsAndColumns = true;
              UltraChart2.Series.Clear();
              for (int i = 1; i < dtChart2.Columns.Count; i++)
              {
                  NumericSeries series = CRHelper.GetNumericSeries(i, dtChart2);
                  series.Label = dtChart2.Columns[i].ColumnName;
                  UltraChart2.Series.Add(series);
              }

           //   UltraChart2.DataSource = dtChart2;
              
          }

       double value;
        protected void UltraChart2_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
           
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    
                    if (box.DataPoint != null)
                    {
                        if (box.DataPoint.Label != null)
                        
                        {
                           switch (box.DataPoint.Label)
                            {
                                case "Исполнено":
                                    {
                                        box.DataPoint.Label = string.Format("{1:N2} млн. руб. \n {0} \n Исполнено \n {2}", box.Series.Label, box.Value, ComboRzPR.SelectedValue);
                                        value = Convert.ToDouble(box.Value);
                                        break;

                                    }
                                case "Назначено ":
                                    {
                                        box.DataPoint.Label = string.Format("{1:N2} млн. руб. \n {0} \n Назначено \n {2}", box.Series.Label, box.Value , ComboRzPR.SelectedValue);
                                        break;

                                    }
                            }
                        }
                    }
                }
            }
        }



        #endregion 

        #region экспорт

          #region экспорт в Excel

           private void ExcelExporter_BeginExport(Object sender, BeginExportEventArgs e)
            {
                e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text;
                e.CurrentWorksheet.Rows[1].Cells[0].Value = PageSubTitle.Text;
            }

           private void ExcelExporter_EndExport(Object sender, Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs e)
           {
               e.CurrentWorksheet.Columns[0].Width = 200*37;
               e.CurrentWorksheet.Rows[3].Height = 10*37;
               e.CurrentWorksheet.Rows[4].Height = 10*37;

               for (int i=1 ; i<UltraWebGrid1.Columns.Count;i++)
               {
                   e.CurrentWorksheet.Columns[i].Width = 90*37;
                   e.CurrentWorksheet.Columns[i+1].Width = 90 * 37;
                   e.CurrentWorksheet.Columns[i].CellFormat.FormatString = "#,##0";
                   e.CurrentWorksheet.Columns[i+1].CellFormat.FormatString = "#,##0";
               }

               for (int i=3; i<UltraWebGrid1.Columns.Count; i+=4)
               {
                   e.CurrentWorksheet.Columns[i].Width = 90*37;
                   e.CurrentWorksheet.Columns[i + 1].Width = 90*37;
                   e.CurrentWorksheet.Columns[i].CellFormat.FormatString = "0.00%";
                   e.CurrentWorksheet.Columns[i+1].CellFormat.FormatString = "0.00%";
               }

               for (int i=1; i<UltraWebGrid1.Columns.Count;i++)
               {
                   e.CurrentWorksheet.Columns[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                   e.CurrentWorksheet.Columns[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                  
               }
               e.CurrentWorksheet.Rows[4].Height = 23 * 37;
               e.CurrentWorksheet.Rows[5].Height = 23 * 37;
             }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");
            Worksheet sheet2 = workbook.Worksheets.Add("sheet2");
            Worksheet sheet3 = workbook.Worksheets.Add("sheet3");

            ReportExcelExporter1.HeaderCellHeight = 30;
            ReportExcelExporter1.GridColumnWidthScale = 1.1;
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
            ReportExcelExporter1.Export(UltraChart1,chart1ElementCaption.Text, sheet2, 3);
            ReportExcelExporter1.Export(UltraChart2,chart2ElementCaption.Text, sheet3, 3);
        }

        #endregion

        #region Экспорт в Pdf

        private void PdfExporter_BeginExport(Object sender, DocumentExportEventArgs e)
         {
             IText title = e.Section.AddText();
             System.Drawing.Font font = new System.Drawing.Font("Verdana", 16);
             title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
             title.Style.Font.Bold = true;
             title.AddContent(PageTitle.Text);

             title = e.Section.AddText();
             title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
             title.Style.Font.Bold = true;
             title.AddContent(PageSubTitle.Text);

             title = e.Section.AddText();
             title.AddContent("                ");

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Width = 170;
            }

            title = e.Section.AddText();
            title.AddContent("                ");
       }
       private void PdfExporter_EndExport(Object sender, EndExportEventArgs e)
         {
           ITable table = e.Section.AddTable();
           ITableRow row = table.AddRow();
           ITableCell cell = row.AddCell();

           UltraChart1.Width = 1000;
           UltraChart2.Width = 700;

           cell.Width = new FixedWidth((float)UltraChart1.Width.Value + 100);
           cell.AddImage(UltraGridExporter.GetImageFromChart(UltraChart1));
           
           cell = row.AddCell();
           cell.AddImage(UltraGridExporter.GetImageFromChart(UltraChart2));

          }

        private void PdfExportButton_Click(Object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

            Report report = new Report();
            ISection section1 = report.AddSection();
            ISection section2 = report.AddSection();
            ISection section3 = report.AddSection();

            UltraChart1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            UltraChart2.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));

            ReportPDFExporter1.HeaderCellHeight = 50;
            ReportPDFExporter1.Export(headerLayout, section1);
            ReportPDFExporter1.Export(UltraChart1,chart1ElementCaption.Text, section2);
            ReportPDFExporter1.Export(UltraChart2,chart2ElementCaption.Text, section3);

        }
        
        #endregion

      #endregion


       }
}