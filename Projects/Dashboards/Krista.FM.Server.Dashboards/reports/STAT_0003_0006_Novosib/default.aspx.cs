using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Infragistics.WebUI.UltraWebNavigator;

using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.UltraChart.Core.Primitives;

/**
 *  Мониторинг ситуации на рынке труда в субъекте РФ по состоянию на ЧЧ.ММ.ГГГГ
 */
namespace Krista.FM.Server.Dashboards.reports.STAT_0003_0006_Novosib
{
	public partial class Default : CustomReportPage
	{

		#region Поля

		private DataTable dtDate;
		private DataTable dtGrid;
		private DataTable dtChart1;
		private DataTable dtChart2;
		private DataTable dtRegion;
		private GridHeaderLayout headerLayout;

        static Dictionary<string, string> prevDates = new Dictionary<string, string>();

		#endregion

        private bool IsSmallResolution
        {
            get { return CRHelper.GetScreenWidth < 1000; }
        }

		private static bool IsMozilla
		{
			get { return HttpContext.Current.Request.Browser.Browser == "Firefox"; }
		}

		private static string Browser
		{
			get { return HttpContext.Current.Request.Browser.Browser; }
		}

        private static bool IsSubject = false;

		#region Параметры запроса

        private CustomParam selectedDate;
        private CustomParam selectedDateFD;
        private CustomParam compareMode;
		private CustomParam selectedRegion;
        private CustomParam selectedParameter;
		private CustomParam lastYear;

		#endregion

		// --------------------------------------------------------------------

        private const string PageTitleCaption = "Мониторинг ситуации на рынке труда ({0})";
        private const string PageSubTitleCaption = "Данные ежемесячного мониторинга ситуации на рынке труда Новосибирской области";
        private const string Chart1TitleCaption = "Сравнительная динамика показателя «{0}», {1}";
        private const string Chart2TitleCaption = "Сравнительная динамика числа незанятых граждан, официально зарегистрированных безработных и числа заявленных вакансий, {0}";

		// --------------------------------------------------------------------

		protected override void Page_PreLoad(object sender, EventArgs e)
		{
			base.Page_PreLoad(sender, e);

            if (!IsSmallResolution)
            {
                ComboDate.Width = 200;
                ComboCompareMode.Width = 300;
                ComboRegion.Width = 350;
                UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 50);
            }
            else
            {
                ComboDate.Width = 125;
                ComboCompareMode.Width = 225;
                ComboRegion.Width = 225;
                UltraWebGrid.Width = CRHelper.GetGridWidth(750);
            }

			ComboDate.Title = "Дата";
			ComboDate.ParentSelect = true;
            ComboDate.MultiSelect = true;

			ComboCompareMode.Title = "Период для сравнения";
			ComboCompareMode.ParentSelect = true;

			ComboRegion.Title = "Территория";
			ComboRegion.ParentSelect = true;

            #region Асинхронные панели

            PanelChart1.AddRefreshTarget(UltraChart1);
            PanelChart1.AddLinkedRequestTrigger(cbMO1);
            PanelChart1.AddLinkedRequestTrigger(cbMO2);
            PanelChart1.AddLinkedRequestTrigger(cbMO3);
            PanelChart1.AddLinkedRequestTrigger(cbMO4);
            PanelChart1.AddLinkedRequestTrigger(cbSubject1);
            PanelChart1.AddLinkedRequestTrigger(cbSubject2);

            PanelCaption1.AddRefreshTarget(LabelChart1);
            PanelCaption1.LinkedRefreshControlID = "PanelChart1";

            #endregion

            #region Грид

			UltraWebGrid.Height = Unit.Empty;
			UltraWebGrid.DataBinding += new EventHandler(UltraWebGrid_DataBinding);
			UltraWebGrid.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);
			UltraWebGrid.InitializeRow += new Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler(UltraWebGrid_InitializeRow);
			UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";
			LabelText.Width = UltraWebGrid.Width;

            #endregion

            #region Настройка диаграммы 1

            UltraChart1.Width = new Unit(UltraWebGrid.Width.Value - 10);
			UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.45);

            UltraChart1.ChartType = ChartType.ColumnChart;
			UltraChart1.Border.Thickness = 0;
            UltraChart1.ColumnChart.SeriesSpacing = 1;
            UltraChart1.Data.ZeroAligned = true;

            UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin;
            UltraChart1.ColorModel.Skin.ApplyRowWise = false;
            UltraChart1.ColorModel.Skin.PEs.Clear();

            UltraChart1.ColumnChart.ChartText.Clear();
            ChartTextAppearance cta = new ChartTextAppearance();
            cta.ChartTextFont = new Font("Verdana", 8);
            cta.Column = -2;
            cta.Row = -2;
            cta.VerticalAlign = StringAlignment.Far;
            cta.ItemFormatString = "<DATA_VALUE:N2>";
            cta.Visible = true;
            UltraChart1.ColumnChart.ChartText.Add(cta);

            PaintElement pe1 = new PaintElement();
            pe1.ElementType = PaintElementType.Gradient;
            pe1.FillGradientStyle = GradientStyle.ForwardDiagonal;
            pe1.Fill = Color.PowderBlue;
            pe1.FillStopColor = Color.SkyBlue;
            pe1.FillOpacity = 100;
            pe1.FillStopOpacity = 200;
            
            PaintElement pe2 = new PaintElement();
            pe2.ElementType = PaintElementType.Gradient;
            pe2.FillGradientStyle = GradientStyle.ForwardDiagonal;
            pe2.Fill = Color.MediumBlue;
            pe2.FillOpacity = 100;
            pe2.FillStopColor = Color.Navy;
            pe2.FillStopOpacity = 200;

            UltraChart1.ColorModel.Skin.PEs.Add(pe1);
            UltraChart1.ColorModel.Skin.PEs.Add(pe2);

            UltraChart1.Axis.X.Labels.Visible = false;
            UltraChart1.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart1.Axis.X.Labels.SeriesLabels.Layout.Behavior = AxisLabelLayoutBehaviors.None;
            UltraChart1.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 8);
            UltraChart1.Axis.X.Labels.SeriesLabels.FontColor = Color.Black;
            UltraChart1.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart1.Axis.Y.Labels.Font = new Font("Verdana", 8);
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N2>";
            UltraChart1.Axis.Y.Labels.FontColor = Color.Black;
			UltraChart1.Axis.Y.Extent = 50;
            UltraChart1.Axis.X.Extent = 60;

            UltraChart1.Axis.X.Margin.Near.Value = 2;
            UltraChart1.Axis.X.Margin.Far.Value = 2;

			UltraChart1.Legend.Visible = true;
			UltraChart1.Legend.Location = LegendLocation.Top;
			UltraChart1.Legend.SpanPercentage = 10;
			UltraChart1.Legend.Font = new Font("Verdana", 10);

			UltraChart1.InvalidDataReceived += new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
			UltraChart1.FillSceneGraph += new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);
			UltraChart1.DataBinding += new EventHandler(UltraChart1_DataBinding);

			#endregion

			#region Настройка диаграммы 2

            UltraChart2.Width = new Unit(UltraWebGrid.Width.Value - 10);
			UltraChart2.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.5);

            UltraChart2.ChartType = ChartType.LineChart;
			UltraChart2.Border.Thickness = 0;

            UltraChart2.Data.ZeroAligned = true;

			UltraChart2.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;
			UltraChart2.Axis.X.Extent = 60;
			UltraChart2.Axis.X.Labels.Font = new Font("Verdana", 8);
			UltraChart2.Axis.X.Labels.FontColor = Color.Black;
			UltraChart2.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
			UltraChart2.Axis.Y.Labels.Font = new Font("Verdana", 8);
			UltraChart2.Axis.Y.Labels.FontColor = Color.Black;
			UltraChart2.Axis.Y.Extent = 40;

            UltraChart2.Axis.X.Margin.Near.Value = 2;
            UltraChart2.Axis.X.Margin.Far.Value = 2;

			UltraChart2.AreaChart.NullHandling = NullHandling.DontPlot;

			UltraChart2.Legend.Visible = true;
			UltraChart2.Legend.Location = LegendLocation.Top;
			UltraChart2.Legend.SpanPercentage = 15;
			UltraChart2.Legend.Font = new Font("Verdana", 10);

			UltraChart2.InvalidDataReceived += new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
			UltraChart2.FillSceneGraph += new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(UltraChart2_FillSceneGraph);
			UltraChart2.DataBinding += new EventHandler(UltraChart2_DataBinding);

            UltraChart2.ColorModel.ModelStyle = ColorModels.CustomLinear;

            UltraChart2.LineChart.LineAppearances.Clear();
            LineAppearance la = new LineAppearance();
            la.IconAppearance.Icon = SymbolIcon.Circle;
            la.IconAppearance.IconSize = SymbolIconSize.Small;
            UltraChart2.LineChart.LineAppearances.Add(la);
                        
            #endregion

			#region Параметры

            selectedDate = UserParams.CustomParam("selected_date");
            selectedDateFD = UserParams.CustomParam("selected_date_fd");
            compareMode = UserParams.CustomParam("compare_mode");
			selectedRegion = UserParams.CustomParam("selected_region");
			lastYear = UserParams.CustomParam("last_year");
            selectedParameter = UserParams.CustomParam("selected_param");

			#endregion

			#region Экспорт
			
			ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
			ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
			ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click); ;
			
			#endregion
		}

		// --------------------------------------------------------------------

		protected override void Page_Load(object sender, EventArgs e)
		{
			base.Page_Load(sender, e);
			if (!Page.IsPostBack)
			{
                prevDates.Clear();
                FillComboDate();
				FillComboByCustomValues(ComboCompareMode);
				FillComboRegion();

                cbMO1.Attributes.Add("onclick", String.Format("uncheck('{0}', '{1}', '{2}')", cbMO2.ClientID, cbMO3.ClientID, cbMO4.ClientID));
                cbMO2.Attributes.Add("onclick", String.Format("uncheck('{0}', '{1}', '{2}')", cbMO1.ClientID, cbMO3.ClientID, cbMO4.ClientID));
                cbMO3.Attributes.Add("onclick", String.Format("uncheck('{0}', '{1}', '{2}')", cbMO1.ClientID, cbMO2.ClientID, cbMO4.ClientID));
                cbMO4.Attributes.Add("onclick", String.Format("uncheck('{0}', '{1}', '{2}')", cbMO1.ClientID, cbMO2.ClientID, cbMO3.ClientID));

                cbSubject1.Attributes.Add("onclick", String.Format("uncheck('{0}', '{1}', '{2}')", cbSubject2.ClientID, cbSubject2.ClientID, cbSubject2.ClientID));
                cbSubject2.Attributes.Add("onclick", String.Format("uncheck('{0}', '{1}', '{2}')", cbSubject1.ClientID, cbSubject1.ClientID, cbSubject1.ClientID));
            }

			#region Анализ параметров

            selectedDate.Value = GetDateParam();
            selectedDateFD.Value = GetDateParamFD();
			selectedRegion.Value = ComboRegion.SelectedValue;
			compareMode.Value = ComboCompareMode.SelectedValue;

			#endregion

			PageTitle.Text = String.Format(PageTitleCaption, selectedRegion.Value);
			Page.Title = PageTitle.Text;
			PageSubTitle.Text = PageSubTitleCaption;

            IsSubject = ComboRegion.SelectedNode.Level == 0;
            if (!IsSubject)
                selectedRegion.Value = "Новосибирская область].[" + selectedRegion.Value;

            if (IsSubject)
            {
                cbMORow.Visible = false;
                cbSubjectRow.Visible = true;
            }
            else
            {
                cbMORow.Visible = true;
                cbSubjectRow.Visible = false;
            }

            if (!PanelChart1.IsAsyncPostBack)
            {
                MakeDynamicText();
                UltraWebGrid.Bands.Clear();
                headerLayout = new GridHeaderLayout(UltraWebGrid);
                UltraWebGrid.DataBind();
                UltraWebGrid_SetGridHints(UltraWebGrid);
                UltraWebGrid_SetRankStars(UltraWebGrid, 1, true);
                UltraWebGrid_SetRankStars(UltraWebGrid, 4, true);
                UltraWebGrid_SetRankStars(UltraWebGrid, 5, true);
                UltraWebGrid_SetRankStars(UltraWebGrid, 7, false);
                UltraChart2.DataBind();
            }
            UltraChart1.DataBind();
        }

		#region Динамический текст

        private string GetQuarterDate(string mdxDate)
        {
            string[] separator = { "].[" };
            string[] splittedDate = mdxDate.Split(separator, StringSplitOptions.None);
            if (splittedDate.Length < 6)
                return "1 квартал 1998 года";
            string quarter = splittedDate[5].Replace("Квартал", String.Empty).Replace("]", String.Empty).Trim();
            string year = splittedDate[3];
            return String.Format("{0} квартал {1} года", quarter, year);
        }

		protected void MakeDynamicText()
		{
            string text;
            string query;
            int queryRes;
            string compareQuarter;
            DataTable dtText = new DataTable();

            TextArea.Visible = true;

            if (IsSubject)
            {
                query = DataProvider.GetQueryText("STAT_0003_0006_Novosib_text_subject");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Параметр", dtText);
                if (dtText.Rows.Count == 0)
                {
                    TextArea.Visible = false;
                    return;
                }

                DataRow row = dtText.Rows[0];

                string currentQuarter = GetQuarterDate(row["Текущая дата_Имя"].ToString());

                text = String.Format("По данным выборочных обследований населения по проблемам занятости в среднем за <b>{0}</b>:", currentQuarter);

                queryRes = Convert.ToInt32(row["Тип результата"]);
                if (queryRes != 0)
                {
                    compareQuarter = GetQuarterDate(row["Дата для сравнения_Имя"].ToString());
                    text += String.Format("<br/>численность экономически активного населения составила <b>{0:N2}</b> тыс. чел. ", row["Значение на текущую дату"]);
                    if (queryRes != 1)
                    {
                        if (queryRes == 2)
                        {
                            text += String.Format("Значение показателя в сравнении со значением за {0} не изменилось;", compareQuarter);
                        }
                        else if (queryRes == 3)
                        {
                            text += String.Format(
                                "Снижение <img src='../../images/ArrowRedDownBB.png'> численности экономически активного населения в сравнении со значением за <b>{0}</b>" +
                                " составило <b>{1:N2}</b> тыс. чел. (темп снижения <b>{2:P2}</b>);",
                                compareQuarter, -Convert.ToDouble(row["Изменение численности"]), -Convert.ToDouble(row["Темп"]));
                        }
                        else if (queryRes == 4)
                        {
                            text += String.Format(
                                "Прирост <img src='../../images/ArrowGreenUpBB.png'> численности экономически активного населения в сравнении со значением за <b>{0}</b>" +
                                " составил <b>{1:N2}</b> тыс. чел. (темп прироста <b>{2:P2}</b>);",
                                compareQuarter, row["Изменение численности"], row["Темп"]);
                        }
                    }
                }

                row = dtText.Rows[1];
                queryRes = Convert.ToInt32(row["Тип результата"]);
                if (queryRes != 0)
                {
                    compareQuarter = GetQuarterDate(row["Дата для сравнения_Имя"].ToString());
                    text += String.Format("<br/>численность занятого населения составила <b>{0:N2}</b> тыс. чел. ", row["Значение на текущую дату"]);
                    if (queryRes != 1)
                    {
                        if (queryRes == 2)
                        {
                            text += String.Format("Значение показателя в сравнении со значением за {0} не изменилось;", compareQuarter);
                        }
                        else if (queryRes == 3)
                        {
                            text += String.Format(
                                "Снижение <img src='../../images/ArrowRedDownBB.png'> численности занятого населения в сравнении со значением за <b>{0}</b>" +
                                " составило <b>{1:N2}</b> тыс. чел. (темп снижения <b>{2:P2}</b>);",
                                compareQuarter, -Convert.ToDouble(row["Изменение численности"]), -Convert.ToDouble(row["Темп"]));
                        }
                        else if (queryRes == 4)
                        {
                            text += String.Format(
                                "Прирост <img src='../../images/ArrowGreenUpBB.png'> численности занятого населения в сравнении со значением за <b>{0}</b>" +
                                " составил <b>{1:N2}</b> тыс. чел. (темп прироста <b>{2:P2}</b>);",
                                compareQuarter, row["Изменение численности"], row["Темп"]);
                        }
                    }
                }

                row = dtText.Rows[2];
                queryRes = Convert.ToInt32(row["Тип результата"]);
                if (queryRes != 0)
                {
                    compareQuarter = GetQuarterDate(row["Дата для сравнения_Имя"].ToString());
                    text += String.Format("<br/>численность безработных составила <b>{0:N2}</b> тыс. чел. ", row["Значение на текущую дату"]);
                    if (queryRes != 1)
                    {
                        if (queryRes == 2)
                        {
                            text += String.Format("Значение показателя в сравнении со значением за {0} не изменилось;", compareQuarter);
                        }
                        else if (queryRes == 3)
                        {
                            text += String.Format(
                                "Снижение <img src='../../images/ArrowGreenDownBB.png'> численности безработных в сравнении со значением за <b>{0}</b>" +
                                " составило <b>{1:N2}</b> тыс. чел. (темп снижения <b>{2:P2}</b>);",
                                compareQuarter, -Convert.ToDouble(row["Изменение численности"]), -Convert.ToDouble(row["Темп"]));
                        }
                        else if (queryRes == 4)
                        {
                            text += String.Format(
                                "Прирост <img src='../../images/ArrowRedUpBB.png'> численности безработных в сравнении со значением за <b>{0}</b>" +
                                " составил <b>{1:N2}</b> тыс. чел. (темп прироста <b>{2:P2}</b>);",
                                compareQuarter, row["Изменение численности"], row["Темп"]);
                        }
                    }
                }

                row = dtText.Rows[3];
                queryRes = Convert.ToInt32(row["Тип результата"]);
                if (queryRes != 0)
                {
                    compareQuarter = GetQuarterDate(row["Дата для сравнения_Имя"].ToString());
                    text += String.Format("<br/>уровень занятости, в % от численности экономически активного населения, составил <b>{0:N2}%</b> ", row["Значение на текущую дату"]);
                    if (queryRes != 1)
                    {
                        if (queryRes == 2)
                        {
                            text += String.Format("и в сравнении со значением за {0} не изменился;", compareQuarter);
                        }
                        else if (queryRes == 3)
                        {
                            text += String.Format(
                                "и в сравнении со значением за <b>{0}</b> уменьшился <img src='../../images/ArrowRedDownBB.png'> на <b>{1:N2}</b> п. п.;",
                                compareQuarter, -Convert.ToDouble(row["Изменение численности"]));
                        }
                        else if (queryRes == 4)
                        {
                            text += String.Format(
                                "и в сравнении со значением за <b>{0}</b> уменьшился <img src='../../images/ArrowGreenUpBB.png'> на <b>{1:N2}</b> п. п.;",
                                compareQuarter, -Convert.ToDouble(row["Изменение численности"]));
                        }
                    }
                }

                row = dtText.Rows[4];
                queryRes = Convert.ToInt32(row["Тип результата"]);
                if (queryRes != 0)
                {
                    compareQuarter = GetQuarterDate(row["Дата для сравнения_Имя"].ToString());
                    text += String.Format("<br/>общий уровень безработицы, в % от численности экономически активного населения, составил <b>{0:N2}%</b> ", row["Значение на текущую дату"]);
                    if (queryRes != 1)
                    {
                        if (queryRes == 2)
                        {
                            text += String.Format("и в сравнении со значением за {0} не изменился;", compareQuarter);
                        }
                        else if (queryRes == 3)
                        {
                            text += String.Format(
                                "и в сравнении со значением за <b>{0}</b> уменьшился <img src='../../images/ArrowGreenDownBB.png'> на <b>{1:N2}</b> п. п.;",
                                compareQuarter, -Convert.ToDouble(row["Изменение численности"]));
                        }
                        else if (queryRes == 4)
                        {
                            text += String.Format(
                                "и в сравнении со значением за <b>{0}</b> уменьшился <img src='../../images/ArrowRedUpBB.png'> на <b>{1:N2}</b> п. п.;",
                                compareQuarter, -Convert.ToDouble(row["Изменение численности"]));
                        }
                    }
                }

            }
            else
            {
                query = DataProvider.GetQueryText("STAT_0003_0006_Novosib_text");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Период", dtText);
                if (dtText.Rows.Count == 0)
                {
                    TextArea.Visible = false;
                    return;
                }

                DataRow row = dtText.Rows[0];
                queryRes = Convert.ToInt32(row["Тип результата"]);
                if (queryRes == 0)
                {
                    TextArea.Visible = false;
                    return;
                }

                text = String.Format("На <b>{0:dd.MM.yyyy}</b> численность трудоспособного населения в трудоспособном возрасте составила <b>{1:N0}</b> человек.",
                    CRHelper.DateByPeriodMemberUName(row["Текущая дата_Имя"].ToString(), 3), row["Значение на текущую дату"]);

                if (queryRes != 1)
                {
                    if (queryRes == 2)
                        text += String.Format(" Численность трудоспособного населения в трудоспособном возрасте за период с <b>{0:dd.MM.yyyy}</b> по <b>{1:dd.MM.yyyy}</b> осталась неизменной.",
                            CRHelper.DateByPeriodMemberUName(row["Дата для сравнения_Имя"].ToString(), 3), CRHelper.DateByPeriodMemberUName(row["Текущая дата_Имя"].ToString(), 3));
                    else
                    {
                        string direction, dir, s = String.Empty;
                        if (queryRes == 3)
                        {
                            direction = "Снижение <img src='../../images/ArrowRedDownBB.png'>";
                            dir = "снижения";
                            s = "о";
                        }
                        else
                        {
                            direction = "Прирост <img src='../../images/ArrowGreenUpBB.png'>";
                            dir = "прироста";
                        }
                        text += String.Format(" {0} численности трудоспособного населения в трудоспособном возрасте в сравнении со значением на <b>{1:dd.MM.yyyy}</b> составил{5} <b>{2:N0}</b> человек (темп {4} <b>{3:P2}</b>).",
                            direction, CRHelper.DateByPeriodMemberUName(row["Дата для сравнения_Имя"].ToString(), 3), Math.Abs(Convert.ToDouble(row["Изменение численности"])), Math.Abs(Convert.ToDouble(row["Темп"])), dir, s);
                    }
                }
            }

			LabelText.Text = text;
		}

		#endregion

		// --------------------------------------------------------------------

		#region Обработчики грида

        protected bool IsRankedRow(UltraGridRow row)
        {
            string paramName = row.Cells[0].GetText();
            return paramName.Contains("Уровень регистрируемой безработицы, %")
                || paramName.Contains("Уровень напряженности на рынке труда")
                || paramName.Contains("Средняя продолжительность безработицы")
                || paramName.Contains("Уровень трудоустройства");
        }

        protected bool IsArrowGreenDownRow(UltraGridRow row)
        {
            string paramName = row.Cells[0].GetText();
            return paramName.Contains("Численность официально зарегистрированных безработных")
                || paramName.Contains("Уровень регистрируемой безработицы, % от численности трудоспособного населения в трудоспособном возрасте")
                || paramName.Contains("Численность незанятых граждан, состоящих на учете")
                || paramName.Contains("Уровень напряженности на рынке труда")
                || paramName.Contains("Средняя продолжительность безработицы");
        }

        protected string GetValueFormatString(UltraGridRow row)
        {
            string paramName = row.Cells[0].GetText();
            if (paramName.Contains("Численность официально зарегистрированных безработных")
                || paramName.Contains("Численность незанятых граждан, состоящих на учете")
                || paramName.Contains("Число заявленных вакансий")
                || paramName.Contains("Численность трудоустроенных граждан"))
                return "{0:N0}";
            else
                return "{0:N2}";
        }

        protected string GetDataValueFormatString()
        {
            if (selectedParameter.Value.Contains("Численность официально зарегистрированных безработных")
                || selectedParameter.Value.Contains("Численность незанятых граждан, состоящих на учете")
                || selectedParameter.Value.Contains("Число заявленных вакансий")
                || selectedParameter.Value.Contains("Численность трудоустроенных граждан"))
                return "<DATA_VALUE:N0>";
            else
                return "<DATA_VALUE:N2>";
        }

        protected void UltraWebGrid_SetRankStars(UltraWebGrid grid, int rowIndex, bool revert)
        {
            if (grid.Rows[rowIndex * 3 + 2] == null)
                return;
            foreach (UltraGridCell cell in grid.Rows[rowIndex * 3 + 2].Cells)
            {
                if (cell.Value == null || !MathHelper.IsDouble(cell.GetText().Replace("Ранг: ", String.Empty)) || cell.Column.Index == 0)
                    continue;
                int rank = Convert.ToInt32(cell.GetText().Replace("Ранг: ", String.Empty).Split(',')[0]);
                int badRank = Convert.ToInt32(cell.GetText().Replace("Ранг: ", String.Empty).Split(',')[1]);
                cell.Value = String.Format("Ранг: {0}", rank);
                if (rank == 1)
                {
                    cell.Style.BackgroundImage = "~/images/starYellowbb.png";
                    cell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    if (revert)
                        cell.Title = "Самый низкий уровень показателя";
                    else
                        cell.Title = "Самый высокий уровень показателя";
                }
                else if (rank == badRank)
                {
                    cell.Style.BackgroundImage = "~/images/starGraybb.png";
                    cell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    if (revert)
                        cell.Title = "Самый высокий уровень показателя";
                    else
                        cell.Title = "Самый низкий уровень показателя";
                }
            }
        }

        protected void UltraWebGrid_SetGridHints(UltraWebGrid grid)
        {
            foreach (UltraGridRow row in grid.Rows)
                foreach (UltraGridCell cell in row.Cells)
                {
                    if (cell.Value == null)
                        continue;
                    object realValue = cell.GetText().Replace("%", String.Empty).Replace("Ранг: ", String.Empty);
                    if (cell.Column.Index == 0 || !MathHelper.IsDouble(realValue) || cell.Row.Index % 3 == 0 || cell.Column.Index == row.Cells.Count - 1)
                        continue;
                    string paramName = row.Cells[0].GetText();
                    double value = Convert.ToDouble(realValue);
                    string compareDate;
                    if (compareMode.ValueIs("к началу года"))
                    {
                        compareDate = "01.01." + cell.Column.Key.Substring(6);
                    }
                    else if (compareMode.ValueIs("к АППГ"))
                    {
                        compareDate = cell.Column.Key.Substring(0, 6) + Convert.ToString((Convert.ToInt32(cell.Column.Key.Substring(6)) - 1));
                    }
                    else
                    {
                        if (!prevDates.TryGetValue(cell.Column.Key, out compareDate))
                        {
                            compareDate = "01.01.1998";
                        }
                    }
                    switch (cell.Row.Index % 3)
                    {
                        case 1:
                            {
                                if (value > 0)
                                {
                                    cell.Title = "Темп прироста к " + compareDate;
                                }
                                else if (value < 0)
                                {
                                    cell.Title = "Темп снижения к " + compareDate;
                                }
                                break;
                            }
                        case 2:
                            {
                                if (IsRankedRow(row))
                                {
                                    cell.Title = "Ранг на " + cell.Column.Key;
                                }
                                else
                                {
                                    if (value > 0)
                                    {
                                        cell.Title = "Прирост к " + compareDate;
                                    }
                                    else if (value < 0)
                                    {
                                        cell.Title = "Снижение к " + compareDate;
                                    }
                                }
                                break;
                            }
                    }

                }
        }

		protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
		{
            string query;
            if (IsSubject)
                query = DataProvider.GetQueryText("STAT_0003_0006_Novosib_grid_subject");
            else
                query = DataProvider.GetQueryText("STAT_0003_0006_Novosib_grid");
			dtGrid = new DataTable();
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtGrid);

            if (dtGrid.Rows.Count > 0)
            {
                dtGrid.Columns.RemoveAt(0);
                for (int i = 0; i < dtGrid.Rows.Count; ++i)
                {
                    if (dtGrid.Rows[i]["Название параметра"] == DBNull.Value || dtGrid.Rows[i]["Название параметра"].ToString() == "1")
                        dtGrid.Rows[i]["Название параметра"] = dtGrid.Rows[i - 1]["Название параметра"];
                    if (dtGrid.Rows[i]["Показатель с единицей измерения"] == DBNull.Value || dtGrid.Rows[i]["Показатель с единицей измерения"].ToString() == "1")
                        dtGrid.Rows[i]["Показатель с единицей измерения"] = dtGrid.Rows[i - 1]["Показатель с единицей измерения"];
                    if (dtGrid.Rows[i]["Показатель с единицей измерения"].ToString().Contains("Уровень регистрируемой безработицы, %"))
                        dtGrid.Rows[i]["Показатель с единицей измерения"] = dtGrid.Rows[i]["Показатель с единицей измерения"].ToString().Replace(", процент", String.Empty);
                    if (dtGrid.Rows[i]["Показатель с единицей измерения"].ToString().Contains("Численность трудоустроенных граждан"))
                        dtGrid.Rows[i]["Показатель с единицей измерения"] = "Численность трудоустроенных граждан (с начала года), человек";
                    if (dtGrid.Rows[i]["Показатель с единицей измерения"].ToString().Contains("Уровень трудоустройства"))
                        dtGrid.Rows[i]["Показатель с единицей измерения"] = "Уровень трудоустройства ищущих работу граждан (с начала года), %";
                }

                for (int i = 1; i < dtGrid.Columns.Count - 1; ++i)
                {
                    dtGrid.Columns[i].ColumnName = dtGrid.Columns[i].Caption = String.Format("{0:dd.MM.yyyy}", CRHelper.DateByPeriodMemberUName(dtGrid.Rows[0][i].ToString(), 3));
                }
                dtGrid.Rows.RemoveAt(0);

                UltraWebGrid.DataSource = dtGrid;
            }
            else
            {
                UltraWebGrid.DataSource = null;
            }
		}

		protected void UltraWebGrid_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
		{
			e.Layout.AllowSortingDefault = AllowSorting.No;
			e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
			e.Layout.NullTextDefault = "-";
			e.Layout.RowAlternateStylingDefault = Infragistics.WebUI.Shared.DefaultableBoolean.False;
			e.Layout.Bands[0].Columns[0].MergeCells = true;
			e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(200);
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            int columnWidth = CRHelper.GetColumnWidth(75);
			// Заголовки
			e.Layout.HeaderStyleDefault.Wrap = true;
			e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
			e.Layout.HeaderStyleDefault.VerticalAlign = VerticalAlign.Middle;
			headerLayout.AddCell("Показатель");
			for (int i = 1; i < e.Layout.Bands[0].Columns.Count; ++i)
			{
				e.Layout.Bands[0].Columns[i].Width = columnWidth;
				e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
				e.Layout.Bands[0].Columns[i].CellStyle.Padding.Right = 5;
				e.Layout.Bands[0].Columns[i].CellStyle.Padding.Left = 5;
				headerLayout.AddCell(e.Layout.Bands[0].Columns[i].Key);
			}
			headerLayout.ApplyHeaderInfo();
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;
		}

		protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
		{
            UltraGridRow row = e.Row;
            string paramName = row.Cells[0].ToString();
            for (int i = 1; i < row.Cells.Count - 1; ++i)
			{
				UltraGridCell cell = row.Cells[i];
				if (row.Index % 3 != 0)
				{
					cell.Style.BorderDetails.StyleTop = BorderStyle.None;
				}
				if (row.Index % 3 != 2)
				{
					cell.Style.BorderDetails.StyleBottom = BorderStyle.None;
				}
                if (MathHelper.IsDouble(cell.Value) || (row.Index % 3 == 2 && IsRankedRow(row)))
                {
                    if (row.Index % 3 == 1)
                    {
                        double value = Convert.ToDouble(cell.Value);
                        if (value != 0)
                        {
                            if (value < 0)
                            {
                                if (IsArrowGreenDownRow(row))
                                {
                                    cell.Style.BackgroundImage = "~/images/ArrowGreenDownBB.png";
                                    cell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                                }
                                else
                                {
                                    cell.Style.BackgroundImage = "~/images/ArrowRedDownBB.png";
                                    cell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                                }
                            }
                            else
                            {
                                if (IsArrowGreenDownRow(row))
                                {
                                    cell.Style.BackgroundImage = "~/images/ArrowRedUpBB.png";
                                    cell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                                }
                                else
                                {
                                    cell.Style.BackgroundImage = "~/images/ArrowGreenUpBB.png";
                                    cell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                                }
                            }
                        }
                    }

                    switch (row.Index % 3)
                    {
                        case 0:
                            {
                                cell.Value = String.Format(GetValueFormatString(row), Convert.ToDouble(cell.Value));
                                break;
                            }
                        case 1:
                            {
                                cell.Value = String.Format("{0:P2}", Convert.ToDouble(cell.Value));
                                break;
                            }
                        case 2:
                            {
                                if (IsRankedRow(row))
                                {
                                    if (cell.Value != null)
                                    {
                                        cell.Value = String.Format("Ранг: {0}", cell.Value);
                                    }
                                }
                                else
                                {
                                    cell.Value = String.Format(GetValueFormatString(row), Convert.ToDouble(cell.Value));
                                }
                                break;
                            }
                    }
                }
			}
		}

		#endregion

		// --------------------------------------------------------------------

		#region Обработчики диаграммы 1

		protected void UltraChart1_DataBinding(object sender, EventArgs e)
		{
            if (IsSubject)
            {
                if (cbSubject1.Checked)
                {
                    LabelChart1.Text = "Сравнительная динамика показателя <b>«Уровень зарегистрированной безработицы (от экономически активного населения)», %</b>";
                    selectedParameter.Value = "[Труд__Трудовые ресурсы].[Труд__Трудовые ресурсы].[Все показатели].[Уровень зарегистрированной безработицы (от экономически активного населения)]";
                    UltraChart1.Tooltips.FormatString = String.Format("<ITEM_LABEL> \n{0} \nна <SERIES_LABEL>: <b><DATA_VALUE:N2></b>", "Уровень зарегистрированной безработицы (от экономически активного населения), %");
                }
                else if (cbSubject2.Checked)
                {
                    LabelChart1.Text = "Сравнительная динамика показателя <b>«Уровень напряженности на рынке труда», ед.</b>";
                    selectedParameter.Value = "[Труд__Трудовые ресурсы].[Труд__Трудовые ресурсы].[Все показатели].[Уровень напряжённости на рынке труда]";
                    UltraChart1.Tooltips.FormatString = String.Format("<ITEM_LABEL> \n{0} \nна <SERIES_LABEL>: <b><DATA_VALUE:N2></b>", "Уровень напряженности на рынке труда, ед.");
                }
            }
            else
            {
                if (cbMO1.Checked)
                {
                    LabelChart1.Text = "Сравнительная динамика показателя <b>«Уровень регистрируемой безработицы», % от численности трудоспособного населения в трудоспособном возрасте</b>";
                    selectedParameter.Value = "[Показатели__Рынок труда].[Показатели__Рынок труда].[Все].[Уровень регистрируемой безработицы, % от численности трудоспособного населения в трудоспособном возрасте]";
                    UltraChart1.Tooltips.FormatString = String.Format("<ITEM_LABEL> \n{0} \nна <SERIES_LABEL>: <b><DATA_VALUE:N2></b>", "Уровень регистрируемой безработицы, % от численности трудоспособного населения в трудоспособном возрасте");
                }
                else if (cbMO2.Checked)
                {
                    LabelChart1.Text = "Сравнительная динамика показателя <b>«Уровень напряженности на рынке труда», ед.</b>";
                    selectedParameter.Value = "[Показатели__Рынок труда].[Показатели__Рынок труда].[Все].[Уровень напряженности на рынке труда]";
                    UltraChart1.Tooltips.FormatString = String.Format("<ITEM_LABEL> \n{0} \nна <SERIES_LABEL>: <b><DATA_VALUE:N2></b>", "Уровень напряженности на рынке труда, ед.");
                }
                else if (cbMO3.Checked)
                {
                    LabelChart1.Text = "Сравнительная динамика показателя <b>«Средняя продолжительность безработицы», мес.</b>";
                    selectedParameter.Value = "[Показатели__Рынок труда].[Показатели__Рынок труда].[Все].[Средняя продолжительность безработицы]";
                    UltraChart1.Tooltips.FormatString = String.Format("<ITEM_LABEL> \n{0} \nна <SERIES_LABEL>: <b><DATA_VALUE:N2></b>", "Средняя продолжительность безработицы, мес.");
                }
                else if (cbMO4.Checked)
                {
                    LabelChart1.Text = "Сравнительная динамика показателя <b>«Уровень трудоустройства (с начала года)», %</b>";
                    selectedParameter.Value = "[Показатели__Рынок труда].[Показатели__Рынок труда].[Все].[Уровень трудоустройства]";
                    UltraChart1.Tooltips.FormatString = String.Format("<ITEM_LABEL> \n{0} \nна <SERIES_LABEL>: <b><DATA_VALUE:N2></b>", "Уровень трудоустройства (с начала года), %");
                }
            }

            string query;
            dtChart1 = new DataTable();
            if (IsSubject)
            {
                query = DataProvider.GetQueryText("STAT_0003_0006_Novosib_chart1_subject");
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart1);
            }
            else
            {
                query = DataProvider.GetQueryText("STAT_0003_0006_Novosib_chart1");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart1);
            }

            if (dtChart1.Rows.Count > 0)
            {
                dtChart1.Columns.RemoveAt(0);
                foreach (DataRow row in dtChart1.Rows)
                {
                    row["Дата"] = String.Format("{0:dd.MM.yyyy}", CRHelper.DateByPeriodMemberUName(row["Дата"].ToString(), 3).AddMonths(IsSubject ? 1 : 0));
                    if (selectedParameter.Value == "[Труд__Трудовые ресурсы].[Труд__Трудовые ресурсы].[Все показатели].[Уровень зарегистрированной безработицы (от экономически активного населения)]")
                    {
                        row["Новосибирская область"] = MathHelper.Mult(row["Новосибирская область"], 100);
                        row["Российская  Федерация"] = MathHelper.Mult(row["Российская  Федерация"], 100);
                    }
                }
                UltraChart1.DataSource = dtChart1.DefaultView;
            }
            else
                UltraChart1.DataSource = null;
        }

		protected void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
		{
			for (int i = 0; i < e.SceneGraph.Count; i++)
			{
				Primitive primitive = e.SceneGraph[i];

                if (primitive is Text)
                {
                    Text text = primitive as Text;

                    if (text.GetTextString() == "<DATA_VALUE:N2>")
                        text.SetTextString(String.Empty);

                }

			}
		}

		#endregion

		#region Обработчики диаграммы 2

		protected void UltraChart2_DataBinding(object sender, EventArgs e)
		{
			LabelChart2.Text = String.Format(Chart2TitleCaption, selectedRegion.Value.Replace("Новосибирская область].[", String.Empty));
            UltraChart2.Tooltips.FormatString = String.Format("{0} \n<SERIES_LABEL> \nна <ITEM_LABEL>: <b><DATA_VALUE></b>",
                selectedRegion.Value.Replace("Новосибирская область].[", String.Empty));
            string query = DataProvider.GetQueryText("STAT_0003_0006_Novosib_chart2");
			dtChart2 = new DataTable();
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart2);

            if (dtChart2.Rows.Count > 0)
            {
                dtChart2.Columns.RemoveAt(0);

                for (int i = 1; i < dtChart2.Columns.Count; ++i)
                {
                    dtChart2.Columns[i].Caption = dtChart2.Columns[i].ColumnName = dtChart2.Rows[0][i].ToString();
                }

                dtChart2.Rows.RemoveAt(0);

                foreach (DataRow row in dtChart2.Rows)
                {
                    row["Дата"] = String.Format("{0:dd.MM.yyyy}", CRHelper.DateByPeriodMemberUName(row["Дата"].ToString(), 3));
                }

                UltraChart2.Series.Clear();

                for (int i = 1; i < dtChart2.Columns.Count; ++i)
                {
                    UltraChart2.Series.Add(CRHelper.GetNumericSeries(i, dtChart2));
                }

                UltraChart2.Data.SwapRowsAndColumns = false;

                //UltraChart2.DataSource = dtChart2.DefaultView;
            }
            else
                UltraChart2.DataSource = null;
		}

		protected void UltraChart2_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
		{
			/*for (int i = 0; i < e.SceneGraph.Count; i++)
			{
				Primitive primitive = e.SceneGraph[i];

				if (primitive is Polyline)
				{
					Polyline polyline = (Polyline)primitive;
					foreach (DataPoint point in polyline.points)
					{
						if (point.Series != null)
						{
							string label = String.Empty;
							if (point.Series.Label == "Численность зарегистрированных безработных в расчете на 1 вакансию в целом по ХМАО")
							{
								label = "Численность зарегистрированных безработных\nв расчете на 1 вакансию в целом по ХМАО";
							}
							else
							{
								label = point.Series.Label.Replace(",", ",\n");
							}
							point.DataPoint.Label = string.Format("{1} на {2}\n {0:N2}", ((NumericDataPoint)point.DataPoint).Value, label, point.DataPoint.Label);

						}
					}
				}
			}*/
		}

		#endregion

		// --------------------------------------------------------------------

		#region Заполнение словарей и выпадающих списков параметров

		/// <summary>
		/// Добаляет одну строку к другой с разделителем между ними. Если первая строка пустая, то разделитель не добавляется
		/// </summary>
		/// <param name="firstString">Строка к которой добавляем</param>
		/// <param name="secondString">Строка, которую добавляем</param>
		/// <param name="separator">Разделитель</param>
		/// <returns>Возвращает получившуюся в результате добавления строку</returns>
		protected string AddStringWithSeparator(string firstString, string secondString, string separator)
		{
			if (String.IsNullOrEmpty(firstString))
			{
				return secondString;
			}
			else
			{
				return firstString + separator + secondString;
			}
		}

		protected void SetComboSelection(CustomMultiCombo combo)
		{
			combo.SetСheckedState(combo.GetRootNodesName(combo.GetRootNodesCount() - 1), true);
		}

		protected void FillComboByCustomValues(CustomMultiCombo combo)
		{
			Collection<string> collection = new Collection<string>();
            collection.Add("к предыдущей дате");
            collection.Add("к началу года");
            collection.Add("к АППГ");
            combo.FillValues(collection);
            combo.SetСheckedState("к предыдущей дате", true);
		}
		
		protected void FillComboDate()
		{
            prevDates.Clear();
			dtDate = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0003_0006_Novosib_list_of_dates");
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Поле", dtDate);
            if (dtDate.Rows.Count == 0)
                throw new Exception("Нет данных для формирования отчета");
			Dictionary<string, int> dictDate = new Dictionary<string, int>();
            DateTime prevDate = new DateTime();
			foreach (DataRow row  in dtDate.Rows)
			{
                DateTime date = CRHelper.DateByPeriodMemberUName(row["Дата"].ToString(), 3);
                AddPairToDictionary(dictDate, CRHelper.PeriodDescr(date, 1), 0);
                AddPairToDictionary(dictDate, CRHelper.PeriodDescr(date, 4), 1);
                AddPairToDictionary(dictDate, CRHelper.PeriodDescr(date, 5), 2);
                prevDates.Add(String.Format("{0:dd.MM.yyyy}", date), String.Format("{0:dd.MM.yyyy}", prevDate));
                prevDate = date;
            }
			ComboDate.FillDictionaryValues(dictDate);
            ComboDate.SetСheckedState(ComboDate.GetLastNode(0).Text, true);
		}

        protected void GetDate(Node node, ref string result)
        {
            if (node.Nodes == null || node.Nodes.Count == 0)
                result = String.IsNullOrEmpty(result) ? StringToMDXDate(node.Text) : result + ",\n" + StringToMDXDate(node.Text);
            else
                foreach (Node cNode in node.Nodes)
                    GetDate(cNode, ref result);
        }
        
        protected string GetDateParam()
        {
            string result = String.Empty;
            foreach (Node node in ComboDate.SelectedNodes)
            {
                GetDate(node, ref result);
            }
            return result;
        }

        protected void GetDateFD(Node node, ref string result)
        {
            if (node.Nodes == null || node.Nodes.Count == 0)
                result = String.IsNullOrEmpty(result) ? StringToMDXPrevMonth(node.Text) : result + ",\n" + StringToMDXPrevMonth(node.Text);
            else
                foreach (Node cNode in node.Nodes)
                    GetDateFD(cNode, ref result);
        }

        protected string GetDateParamFD()
        {
            string result = String.Empty;
            foreach (Node node in ComboDate.SelectedNodes)
            {
                GetDateFD(node, ref result);
            }
            return result;
        }

        protected void FillComboRegion()
		{
			dtRegion = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0003_0006_Novosib_list_of_regions");
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Поле", dtRegion);
            if (dtRegion.Rows.Count == 0)
                throw new Exception("Нет данных для формирования отчета");
            Dictionary<string, int> dict = new Dictionary<string, int>();
            AddPairToDictionary(dict, "Новосибирская область", 0);
			foreach (DataRow row in dtRegion.Rows)
			{
				AddPairToDictionary(dict, CRHelper.GetLastBlock(row["Территория"].ToString()), 1);
			}
			ComboRegion.FillDictionaryValues(dict);
            ComboRegion.SetСheckedState("Новосибирская область", true);
            ComboRegion.SelectedNode.Expand(true);
		}

		protected void AddPairToDictionary(Dictionary<string, int> dict, string key, int value)
		{
			if (!dict.ContainsKey(key))
			{
				dict.Add(key, value);
			}
		}

		#endregion

		#region Функции-полезняшки преобразования и все такое

		public string GetMaxMDXDate(string firstDate, string secondDate)
		{
			if (Convert.ToInt32(FormatMDXDate(firstDate, "{0}{1:00}{2:00}")) > Convert.ToInt32(FormatMDXDate(secondDate, "{0}{1:00}{2:00}")))
			{
				return firstDate;
			}
			else
			{
				return secondDate;
			}
		}

		public string FormatMDXDate(string mdxDate, string formatString, int yearIndex, int monthIndex, int dayIndex)
		{
			string[] separator = { "].[" };
			string[] dateElements = mdxDate.Split(separator, StringSplitOptions.None);
			if (dateElements.Length < 8)
			{
				return String.Format(formatString, 1998, 1, 1);
			}
			int year = Convert.ToInt32(dateElements[yearIndex]);
			int month = Convert.ToInt32(CRHelper.MonthNum(dateElements[monthIndex]));
			int day = Convert.ToInt32(dateElements[dayIndex]);
			return String.Format(formatString, year, month, day);
		}

		public string FormatMDXDate(string mdxDate, string formatString)
		{
			string[] separator = { "].[" };
			string[] dateElements = mdxDate.Split(separator, StringSplitOptions.None);
			if (dateElements.Length < 8)
			{
				return String.Format(formatString, 1998, 1, 1);
			}
			int year = Convert.ToInt32(dateElements[3]);
			int month = Convert.ToInt32(CRHelper.MonthNum(dateElements[6]));
			int day = Convert.ToInt32(dateElements[7].Replace("]", String.Empty));
			return String.Format(formatString, year, month, day);
		}

		public string StringToMDXDate(string str)
		{
			string template = "[Период__Период].[Период__Период].[Данные всех периодов].[{0}].[Полугодие {1}].[Квартал {2}].[{3}].[{4}]";
			string[] dateElements = str.Split(' ');
			int year = Convert.ToInt32(dateElements[2]);
			string month = CRHelper.RusMonth(CRHelper.MonthNum(dateElements[1]));
			int quarter = CRHelper.QuarterNumByMonthNum(CRHelper.MonthNum(month));
			int halfYear = CRHelper.HalfYearNumByQuarterNum(quarter);
			int day = Convert.ToInt32(dateElements[0]);
			return String.Format(template, year, halfYear, quarter, month, day);
		}

        public string StringToMDXPrevMonth(string str)
        {
            string template = "[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[Данные всех периодов].[{0}].[Полугодие {1}].[Квартал {2}].[{3}]";
            string[] dateElements = str.Split(' ');
            int year = Convert.ToInt32(dateElements[2]);
            int monthNum = CRHelper.MonthNum(dateElements[1]);
            DateTime date = new DateTime(year, monthNum, 1).AddMonths(-1);
            year = date.Year;
            string month = CRHelper.RusMonth(date.Month);
            int quarter = CRHelper.QuarterNumByMonthNum(CRHelper.MonthNum(month));
            int halfYear = CRHelper.HalfYearNumByQuarterNum(quarter);
            return String.Format(template, year, halfYear, quarter, month);
        }

		public string MDXDateToShortDateString(string mdxDateString)
		{
			if (String.IsNullOrEmpty(mdxDateString))
			{
				return null;
			}
			string[] separator = { "].[" };
			string[] dateElements = mdxDateString.Split(separator, StringSplitOptions.None);
			string template = "{0}.{1}.{2}";
			if (dateElements.Length < 8)
			{
				return null;
			}
			string day = dateElements[7].Replace("]", String.Empty);
			day = day.Length == 1 ? "0" + day : day;
			string month = CRHelper.MonthNum(dateElements[6]).ToString();
			month = month.Length == 1 ? "0" + month : month;
			string year = dateElements[3];
			return String.Format(template, day, month, year);
		}

		public string GetYearFromMDXDate(string mdxDate)
		{
			string[] separator = { "].[" };
			string[] mdxDateElements = mdxDate.Split(separator, StringSplitOptions.None);
			if (mdxDateElements.Length == 8)
			{
				return mdxDateElements[3];
			}
			else
			{
				return "2010";
			}
		}

		#endregion

		#region Экспорт в PDF
		
		private void PdfExportButton_Click(object sender, EventArgs e)
		{
			Report report = new Report();
			ISection section1 = report.AddSection();
			ISection section2 = report.AddSection();
			ISection section3 = report.AddSection();

			foreach (UltraGridRow row in UltraWebGrid.Rows)
				foreach (UltraGridCell cell in row.Cells)
				{
					if (cell.Value != null)
					{
						cell.Value = Regex.Replace(cell.GetText(), "<[\\s\\S]*?>", String.Empty);
					}
				}

			IText title = section1.AddText();
			Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
			title.Style.Font.Bold = true;
			title.AddContent(PageTitle.Text);

			title = section1.AddText();
			font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
			title.AddContent(PageSubTitle.Text);

			title = section1.AddText();
			font = new Font("Verdana", 12);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
			title.AddContent("\n" + Regex.Replace(LabelText.Text.Replace("<br/>", "\n"), "<[\\s\\S]*?>", String.Empty) + "\n");

			foreach (UltraGridRow row in headerLayout.Grid.Rows)
			{
				if (row.Index % 3 != 0)
				{
					row.Cells[0].Style.BorderDetails.StyleTop = BorderStyle.None;
				}
				else
				{
					row.Cells[0].Value = null;
				}
				if (row.Index % 3 != 2)
				{
					row.Cells[0].Style.BorderDetails.StyleBottom = BorderStyle.None;
				}
				else
				{
					row.Cells[0].Value = null;
				}
			}

			ReportPDFExporter1.HeaderCellHeight = 60;
			ReportPDFExporter1.Export(headerLayout, section1);

			UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.55);
			ReportPDFExporter1.Export(UltraChart1, LabelChart1.Text, section2);

			UltraChart2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.55);
			ReportPDFExporter1.Export(UltraChart2, LabelChart2.Text, section3);
		}
		
		#endregion

		#region Экспорт в Excel

		private void ExcelExportButton_Click(object sender, EventArgs e)
		{
			ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
			ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

			Workbook workbook = new Workbook();
			Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
			Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма 1");
            Worksheet sheet3 = workbook.Worksheets.Add("Диаграмма 2");
            Worksheet sheet4 = workbook.Worksheets.Add("Пустая страница");

			SetExportGridParams(headerLayout.Grid);

			ReportExcelExporter1.HeaderCellHeight = 80;
			ReportExcelExporter1.HeaderCellFont = new Font("Verdana", 11);
			ReportExcelExporter1.TitleFont = new Font("Verdana", 12, FontStyle.Bold);
			ReportExcelExporter1.SubTitleFont = new Font("Verdana", 11);
			ReportExcelExporter1.TitleAlignment = HorizontalCellAlignment.Center;
			ReportExcelExporter1.TitleStartRow = 0;

			foreach (UltraGridRow row in UltraWebGrid.Rows)
			{
				foreach (UltraGridCell cell in row.Cells)
				{
					if (cell.Value != null)
					{
						cell.Value = Regex.Replace(cell.GetText(), "<[\\s\\S]*?>", String.Empty);
					}
				}
                if (row.IsActiveRow())
                {
                    row.Activated = row.Selected = false;
                }
			}

            headerLayout.childCells.RemoveAt(headerLayout.childCells.Count - 1);

            int gridStartRow = 3;

            ReportExcelExporter1.Export(headerLayout, sheet1, gridStartRow);

			foreach (UltraGridRow row in UltraWebGrid.Rows)
			{
                sheet1.Rows[gridStartRow + 1 + row.Index].Height = 255;
			}

			sheet1.Rows[0].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.False;
			sheet1.Rows[0].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
			sheet1.Rows[1].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            sheet1.Rows[1].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
            sheet1.Rows[1].Height = 600;
            for (int i = 0; i < UltraWebGrid.Rows.Count; i += 3)
			{
				for (int j = 0; j < UltraWebGrid.Columns.Count; ++j)
				{
                    sheet1.Rows[gridStartRow + 1 + i].Cells[j].CellFormat.BottomBorderStyle = CellBorderLineStyle.None;
                    sheet1.Rows[gridStartRow + 2 + i].Cells[j].CellFormat.BottomBorderStyle = CellBorderLineStyle.None;
                    sheet1.Rows[gridStartRow + 2 + i].Cells[j].CellFormat.TopBorderStyle = CellBorderLineStyle.None;
                    sheet1.Rows[gridStartRow + 3 + i].Cells[j].CellFormat.TopBorderStyle = CellBorderLineStyle.None;
				}
			}

			ReportExcelExporter1.WorksheetTitle = String.Empty;
			ReportExcelExporter1.WorksheetSubTitle = String.Empty;
			UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.6);
			UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.4);
			ReportExcelExporter1.Export(UltraChart1, LabelChart1.Text, sheet2, 1);
            sheet2.Columns[0].Width = (int)(20 * UltraChart1.Width.Value);
            sheet2.Rows[0].Height = 600;
            sheet2.Rows[0].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;

			UltraChart2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.6);
			UltraChart2.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.4);
            UltraChart2.Legend.SpanPercentage = 20;
			ReportExcelExporter1.Export(UltraChart2, LabelChart2.Text, sheet3, 1);
            sheet3.Columns[0].Width = (int)(20 * UltraChart2.Width.Value);
            sheet3.Rows[0].Height = 600;
            sheet3.Rows[0].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;

            ReportExcelExporter1.Export(new GridHeaderLayout(UltraWebGrid), sheet4, 0);

            workbook.Worksheets.Remove(sheet4);
        }

		private void SetEmptyCellFormat(Worksheet sheet, int row, int column)
		{
			WorksheetCell cell = sheet.Rows[9 + row * 3].Cells[column];
			cell.CellFormat.Font.Name = "Verdana";
			cell.CellFormat.Font.Height = 200;
			cell.CellFormat.VerticalAlignment = VerticalCellAlignment.Center;

			cell.CellFormat.LeftBorderStyle = CellBorderLineStyle.Thin;
			cell.CellFormat.LeftBorderColor = Color.Black;
			cell.CellFormat.RightBorderStyle = CellBorderLineStyle.Thin;
			cell.CellFormat.RightBorderColor = Color.Black;
			cell.CellFormat.TopBorderStyle = CellBorderLineStyle.Thin;
			cell.CellFormat.TopBorderColor = Color.Black;
			cell.CellFormat.BottomBorderStyle = CellBorderLineStyle.None;
			cell.CellFormat.BottomBorderColor = Color.Black;

			cell = sheet.Rows[10 + row * 3].Cells[column];
			cell.CellFormat.LeftBorderStyle = CellBorderLineStyle.Thin;
			cell.CellFormat.LeftBorderColor = Color.Black;
			cell.CellFormat.RightBorderStyle = CellBorderLineStyle.Thin;
			cell.CellFormat.RightBorderColor = Color.Black;
			cell.CellFormat.BottomBorderStyle = CellBorderLineStyle.None;
			cell.CellFormat.BottomBorderColor = Color.Black;
			cell.CellFormat.TopBorderStyle = CellBorderLineStyle.None;
			cell.CellFormat.TopBorderColor = Color.Black;

			cell = sheet.Rows[11 + row * 3].Cells[column];
			cell.CellFormat.BottomBorderStyle = CellBorderLineStyle.Thin;
			cell.CellFormat.BottomBorderColor = Color.Black;
			cell.CellFormat.LeftBorderStyle = CellBorderLineStyle.Thin;
			cell.CellFormat.LeftBorderColor = Color.Black;
			cell.CellFormat.RightBorderStyle = CellBorderLineStyle.Thin;
			cell.CellFormat.RightBorderColor = Color.Black;
			cell.CellFormat.TopBorderStyle = CellBorderLineStyle.None;
			cell.CellFormat.TopBorderColor = Color.Black;
		}

		private void SetCellFormat(WorksheetCell cell)
		{
			cell.CellFormat.Font.Name = "Verdana";
			cell.CellFormat.Font.Height = 200;
			cell.CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
			cell.CellFormat.BottomBorderStyle = CellBorderLineStyle.Thin;
			cell.CellFormat.BottomBorderColor = Color.Black;
			cell.CellFormat.LeftBorderStyle = CellBorderLineStyle.Thin;
			cell.CellFormat.LeftBorderColor = Color.Black;
			cell.CellFormat.RightBorderStyle = CellBorderLineStyle.Thin;
			cell.CellFormat.RightBorderColor = Color.Black;
			cell.CellFormat.TopBorderStyle = CellBorderLineStyle.Thin;
			cell.CellFormat.TopBorderColor = Color.Black;
		}

		private static void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
		{
            e.CurrentWorksheet.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;
            e.CurrentWorksheet.PrintOptions.PaperSize = PaperSize.A4;
            e.CurrentWorksheet.PrintOptions.BottomMargin = 0.25;
            e.CurrentWorksheet.PrintOptions.TopMargin = 0.25;
            e.CurrentWorksheet.PrintOptions.LeftMargin = 0.25;
            e.CurrentWorksheet.PrintOptions.RightMargin = 0.25;
            e.CurrentWorksheet.PrintOptions.ScalingType = ScalingType.FitToPages;
        }

		private static void SetExportGridParams(UltraWebGrid grid)
		{
			string exportFontName = "Verdana";
			int fontSize = 10;
			double coeff = 1.0;
			foreach (UltraGridColumn column in grid.Columns)
			{
				column.Width = Convert.ToInt32(column.Width.Value * coeff);
				column.CellStyle.Font.Name = exportFontName;
				column.Header.Style.Font.Name = exportFontName;
				column.CellStyle.Font.Size = fontSize;
				column.Header.Style.Font.Size = fontSize;
			}
		}
		
		#endregion
	}
}
