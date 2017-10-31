using System;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.SkkSupport;

namespace Krista.FM.Server.Dashboards.reports.SKK.SKK_010
{
	public partial class Default : CustomReportPage
	{
		private GridUpDownBase gridPeopleHelper;
		private ChartCmpBase chartCompareHelper;
		private ChartBase chartDistrictsHelper;
		private ChartBase chartBordersHelper;
		private GridBorder gridBorderHelper;
		
		// параметры запроса
		private static MemberAttributesDigest digestTransport;
		private static MemberAttributesDigest digestDirection;
		private CustomParam selectedTransport;
		private CustomParam selectedPeriod;
		private CustomParam selectedPeriodCompare;
		private CustomParam selectedDirection;


		protected override void Page_PreLoad(object sender, EventArgs e)
		{
			base.Page_PreLoad(sender, e);

			// Инициализация параметров запроса
			selectedTransport = UserParams.CustomParam("selected_transport");
			selectedPeriod = UserParams.CustomParam("selected_period");
			selectedPeriodCompare = UserParams.CustomParam("selected_period_compare");
			selectedDirection = UserParams.CustomParam("selected_direction");

			// Настройка экспорта
			ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
			ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
		}

		protected override void Page_Load(object sender, EventArgs e)
		{
			base.Page_Load(sender, e);
			
			if (!Page.IsPostBack)
			{
				ParamDate paramDate = SKKHelper.ParamDate_Init();
				
				// Вид сообщения
				digestTransport = SKKHelper.DigestTransport_Init();
				comboTransport.ComboTransport_Init(digestTransport);
				
				// отчетный период
				comboPeriodYear.ComboCurrentYear_Init(paramDate.GetFirstYear(), paramDate.GetLastYear(), paramDate.GetLastYear());
				comboPeriodMonth.ComboCurrentMonth_Init(paramDate.GetLastMonth());

				// период для сравнения
				comboPeriodCompareYear.ComboCurrentYear_Init(paramDate.GetFirstYear(), paramDate.GetLastYear(), paramDate.GetPrevYear());
				comboPeriodCompareMonth.ComboCurrentMonth_Init(paramDate.GetPrevMonth());
				
				// направление
				digestDirection = SKKHelper.DigestDirection_Init();
				comboDirection.ComboDirection_Init(digestDirection);
			}

			// изменение результатов запроса

			SKKHelper.ChangeSelectedPeriod(comboPeriodMonth);
			SKKHelper.ChangeSelectedPeriodCompare(comboPeriodCompareMonth);

			// сбор параметров

			string selectedMonthUnique = SKKHelper.GetUniqueMonths(comboPeriodMonth.SelectedValues, comboPeriodYear.SelectedValue);
			string selectedMonthUniqueCompare = SKKHelper.GetUniqueMonths(comboPeriodCompareMonth.SelectedValues, comboPeriodCompareYear.SelectedValue);
			string selectedMonthAblativeCompare = SKKHelper.GetAblativeMonths(comboPeriodCompareMonth.SelectedValues);
			
			// текстовики

			PageTitle.Text = "Досмотр лиц в пунктах пропуска выбранного вида международного сообщения";
			Page.Title = PageTitle.Text;
			PageSubTitle.Text = String.Format("Досмотр лиц в пунктах пропуска вида сообщения \"<b>{0}</b>\" за <b>{1} {2} года</b> в сравнении с <b>{3} {4} года</b>, направление: <b>{5}</b>",
				comboTransport.SelectedValue.ToLower(),
				comboPeriodMonth.SelectedValuesString.ToLower(), 
				comboPeriodYear.SelectedValue,
				selectedMonthAblativeCompare, 
				comboPeriodCompareYear.SelectedValue,
				CRHelper.ToLowerFirstSymbol(comboDirection.SelectedValue.Trim())
			);
			txtPeriod.Text = "Отчетный период";
			txtPeriodCompare.Text = "Период для сравнения";
			labelGrid.Text = "";
			labelChartDistricts.Text = "Досмотр лиц по федеральным округам";
			labelChartCompare.Text = "Досмотр лиц по выбранному виду международного сообщения";
			labelChartBorders.Text = "Досмотр лиц по участкам границы";

			// параметры для запроса

			selectedTransport.Value = digestTransport.GetMemberUniqueName(comboTransport.SelectedValue);
			selectedPeriod.Value = selectedMonthUnique;
			selectedPeriodCompare.Value = selectedMonthUniqueCompare;
			selectedDirection.Value = digestDirection.GetMemberUniqueName(comboDirection.SelectedValue);

			// инициализация и запуск
			SetDataHelpers();
		}

		private void SetDataHelpers()
		{
			// таблица рост/снижение
			gridPeopleHelper = new GridUpDownPeople(GridBrick);
			gridPeopleHelper.SetStyle();
			if (comboTransport.SelectedValue.Trim().ToLower().Equals("всего"))
			{
				// в разрезе видов сообщения
				labelGrid.Text = "Досмотр лиц по видам международного сообщения";
				gridPeopleHelper.LabelCommon = "Вид сообщения";
				gridPeopleHelper.SetData("skk_010_grid_transport");
			}
			else
			{
				// в разрезе пунктов пропуска
				labelGrid.Text = "Досмотр лиц по пунктам пропуска";
				gridPeopleHelper.LabelCommon = "Пункт пропуска";
				gridPeopleHelper.SetData("skk_010_grid_points");
			}
			scriptBlock.JavaScript_SetVerticalScrollBar(gridPeopleHelper.Grid);

			// диаграмма сравнения
			chartCompareHelper = new ChartCmpPeople(ChartCompare);
			chartCompareHelper.SetStyle(SKKHelper.default2ItemsWidth, SKKHelper.defaultChartBordersHeight);
			chartCompareHelper.Series1Label = String.Format("{0} {1} года", comboPeriodMonth.SelectedValuesString.ToLower(), comboPeriodYear.SelectedValue);
			chartCompareHelper.Series2Label = String.Format("{0} {1} года", comboPeriodCompareMonth.SelectedValuesString.ToLower(), comboPeriodCompareYear.SelectedValue);
			chartCompareHelper.SetData("skk_010_chart_compare");

			// диаграмма по федеральным округам
			chartDistrictsHelper = new ChartDistrictsPeople(ChartDistricts);
			chartDistrictsHelper.SetStyle(SKKHelper.default1ItemsWidth, SKKHelper.defaultChartHeight);
			chartDistrictsHelper.SetData("skk_010_chart_districts");
			
			// диаграмма по участкам границы
			GridBorderPlace.Visible = false;
			chartBordersHelper = new ChartBordersPeople(ChartBorders);
			chartBordersHelper.TitleNoData = SKKHelper.noDataBordersByTransport;
			chartBordersHelper.SetStyle(SKKHelper.default2ItemsWidth, SKKHelper.defaultChartHeight);
			chartCompareHelper.SetChartHeight(chartBordersHelper.ChartHeight);
			if (!comboTransport.SelectedValue.Trim().ToLower().Equals("воздушный")
				&& !comboTransport.SelectedValue.Trim().ToLower().Equals("морской")
				&& !comboTransport.SelectedValue.Trim().ToLower().Equals("речной"))
			{
				chartBordersHelper.SetChartHeight(SKKHelper.defaultChartBordersHeight);
				chartBordersHelper.SetData("skk_010_chart_borders");
				chartCompareHelper.SetChartHeight(chartBordersHelper.ChartHeight);

				if ((chartBordersHelper.Chart.Series.Count > 0) && (chartBordersHelper.Chart.Series[0].GetPoints().Count == 1))
				{
					chartBordersHelper.Chart.Visible = false;

					GridBorderPlace.Visible = true;
					gridBorderHelper = new GridBorder(GridSimpleBorder);
					gridBorderHelper.SetLabels(GridBorderLabelParam.Transport, GridBorderLabelCount.People);
					gridBorderHelper.SetStyle();
					gridBorderHelper.SetFullAutoSizes();
					gridBorderHelper.SetData("skk_010_grid_borders");
					chartCompareHelper.SetOuterHeight(GridBorderPlace);
				}
			}
			
		}

		private void PdfExportButton_Click(object sender, EventArgs e)
		{
			SKKHelper.ExportPdfSetStyle(ReportPDFExporter1);
			
			const int ItemsCount = 4;
			ISection[] sections = new ISection[ItemsCount];
			Report report = new Report();

			ReportPDFExporter1.PageTitle = PageTitle.Text;
			ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;
			
			int currentSection = 0;
			
			// таблица
			sections[currentSection] = report.AddSection();
			ReportPDFExporter1.Export(
				gridPeopleHelper.Grid.GridHeaderLayout,
				labelGrid.Text,
				sections[currentSection++]);

			// диаграмма сравнение
			sections[currentSection] = report.AddSection();
			ReportPDFExporter1.Export(
				chartCompareHelper.Chart,
				String.Empty,
				sections[currentSection++]);

			// диаграмма по федеральным округам
			sections[currentSection] = report.AddSection();
			ReportPDFExporter1.Export(
				chartDistrictsHelper.Chart, 
				labelChartDistricts.Text,
				sections[currentSection++]);

			// диаграмма по участкам границы
			sections[currentSection] = report.AddSection();
			if ((chartBordersHelper.Chart.Series.Count > 0) && (chartBordersHelper.Chart.Series[0].GetPoints().Count == 1))
			{
				// таблица
				ReportPDFExporter1.Export(
					gridBorderHelper.Grid.GridHeaderLayout,
					labelChartBorders.Text,
					sections[currentSection++]);
			}
			else
			{
				// диаграмма
				ReportPDFExporter1.Export(
					chartBordersHelper.Chart,
					labelChartBorders.Text,
					sections[currentSection++]);
			}
			
		}

		private void ExcelExportButton_Click(object sender, EventArgs e)
		{
			SKKHelper.ExportExcelSetStyle(ReportExcelExporter1);

			const int ItemsCount = 4;
			Worksheet[] sheets = new Worksheet[ItemsCount];
			Workbook workbook = new Workbook();
			
			ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
			ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;
			
			int currentSheet = 0;
			
			// таблица
			sheets[currentSheet] = workbook.Worksheets.Add("sheet" + (currentSheet + 1));
			ReportExcelExporter1.Export(
				gridPeopleHelper.Grid.GridHeaderLayout,
				labelGrid.Text,
				sheets[currentSheet++], 
				3);

			// диаграмма сравнение
			sheets[currentSheet] = workbook.Worksheets.Add("sheet" + (currentSheet + 1));
			ReportExcelExporter1.Export(
				chartCompareHelper.Chart,
				String.Empty,
				sheets[currentSheet++], 
				3);
			
			// диаграмма по федеральным округам
			sheets[currentSheet] = workbook.Worksheets.Add("sheet" + (currentSheet + 1));
			ReportExcelExporter1.Export(
				chartDistrictsHelper.Chart,
				labelChartDistricts.Text,
				sheets[currentSheet++], 
				3);

			// диаграмма по участкам границы
			sheets[currentSheet] = workbook.Worksheets.Add("sheet" + (currentSheet + 1));
			if ((chartBordersHelper.Chart.Series.Count > 0) && (chartBordersHelper.Chart.Series[0].GetPoints().Count == 1))
			{
				// таблица
				ReportExcelExporter1.Export(
					gridBorderHelper.Grid.GridHeaderLayout,
					labelChartBorders.Text,
					sheets[currentSheet++],
					3);
			}
			else
			{
				// диаграмма
				ReportExcelExporter1.Export(
					chartBordersHelper.Chart,
					labelChartBorders.Text,
					sheets[currentSheet++],
					3);
			}
			
		}

	}

}
