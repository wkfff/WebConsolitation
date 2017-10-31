using System;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.SkkSupport;

namespace Krista.FM.Server.Dashboards.reports.SKK.SKK_009
{
	public partial class Default : CustomReportPage
	{
		private GridBase gridUpDownHelper;
		private ChartBase chartTerraHelper;
		private ChartCmpBase chartCompareHelper;
		private ChartBordersBase chartBordersHelper;
		private GridBorder gridBorderHelper;
		private GridCountBase gridTransportHelper;
		private ChartTransportBase chartTransportHelper;

		// параметры запроса
		private static MemberAttributesDigest digestTerra;
		private static MemberAttributesDigest digestDirection;
		private CustomParam selectedTerra;
		private CustomParam selectedPeriod;
		private CustomParam selectedPeriodCompare;
		private CustomParam selectedDirection;


		protected override void Page_PreLoad(object sender, EventArgs e)
		{
			base.Page_PreLoad(sender, e);

			// Инициализация параметров запроса
			selectedTerra = UserParams.CustomParam("selected_terra");
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
				
				// территория
				digestTerra = SKKHelper.DigestTerra_Init();
				comboTerra.ComboTerra_Init(digestTerra);
				
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

			PageTitle.Text = "Досмотр лиц на выбранной территории";
			Page.Title = PageTitle.Text;
			PageSubTitle.Text = String.Format("Досмотр лиц на территории \"<b>{0}</b>\" за <b>{1} {2} года</b> в сравнении с <b>{3} {4} года</b>, направление: <b>{5}</b>",
				comboTerra.SelectedValue,
				comboPeriodMonth.SelectedValuesString.ToLower(), 
				comboPeriodYear.SelectedValue,
				selectedMonthAblativeCompare, 
				comboPeriodCompareYear.SelectedValue,
				CRHelper.ToLowerFirstSymbol(comboDirection.SelectedValue.Trim())
			);
			txtPeriod.Text = "Отчетный период";
			txtPeriodCompare.Text = "Период для сравнения";

			LabelGrid.Text = "Досмотр лиц на выбранной территории";
			LabelChartTerra.Text = "";
			LabelChartCompare.Text = "Досмотр лиц на выбранной территории";
			LabelChartBorder.Text = "Досмотр лиц по участкам границы"; 
			LabelTransport.Text = "Досмотр лиц по виду международного сообщения";

			// параметры для запроса

			selectedTerra.Value = digestTerra.GetMemberUniqueName(comboTerra.SelectedValue);
			selectedPeriod.Value = selectedMonthUnique;
			selectedPeriodCompare.Value = selectedMonthUniqueCompare;
			selectedDirection.Value = digestDirection.GetMemberUniqueName(comboDirection.SelectedValue);

			// инициализация и запуск
			SetDataHelpers();
		}
		
		private void SetDataHelpers()
		{
			// таблица рост/снижение
			gridUpDownHelper = new GridUpDownPeople(GridUpDown);
			gridUpDownHelper.SetStyle();
			gridUpDownHelper.LabelCommon = "Территория";
			gridUpDownHelper.SetData("skk_009_grid_updown");
			scriptBlock.JavaScript_SetVerticalScrollBar(gridUpDownHelper.Grid);

			// диаграмма - общее число
			if (comboTerra.SelectedValue.ToUpper().Equals("РФ"))
			{
				// по ФО
				LabelChartTerra.Text = "Досмотр лиц по федеральным округам";
				chartTerraHelper = new ChartDistrictsPeople(ChartTerra);
				chartTerraHelper.SetData("skk_009_chart_districts");
			}
			else if (comboTerra.SelectedNode.Level == 0)
			{
				// по субъектам
				LabelChartTerra.Text = "Досмотр лиц по субъектам";
				chartTerraHelper = new ChartSubjectsPeople(ChartTerra);
				chartTerraHelper.SetData("skk_009_chart_subjects");
			}
			else if (comboTerra.SelectedNode.Level == 1)
			{
				// по пунктам пропуска
				LabelChartTerra.Text = "Досмотр лиц по пунктам пропуска";
				chartTerraHelper = new ChartPointsPeople(ChartTerra);
				chartTerraHelper.SetData("skk_009_chart_points");
			}
			chartTerraHelper.SetStyle(SKKHelper.default1ItemsWidth, SKKHelper.defaultChartHeight);

			// сравнение, диаграмма
			chartCompareHelper = new ChartCmpPeople(ChartCompare);
			chartCompareHelper.SetStyle(SKKHelper.default2ItemsWidth, SKKHelper.defaultChartBordersHeight);
			chartCompareHelper.Series1Label = String.Format("{0} {1} года", comboPeriodMonth.SelectedValuesString.ToLower(), comboPeriodYear.SelectedValue);
			chartCompareHelper.Series2Label = String.Format("{0} {1} года", comboPeriodCompareMonth.SelectedValuesString.ToLower(), comboPeriodCompareYear.SelectedValue);
			chartCompareHelper.SetData("skk_009_chart_compare");

			// по участкам границы, диаграмма
			chartBordersHelper = new ChartBordersPeople(ChartBorder);
			chartBordersHelper.TitleNoData = SKKHelper.noDataBordersByTerra;
			chartBordersHelper.SetStyle(SKKHelper.default2ItemsWidth, SKKHelper.defaultChartBordersHeight);
			chartBordersHelper.SetData("skk_009_chart_borders");
			chartCompareHelper.SetChartHeight(chartBordersHelper.ChartHeight);
			if ((chartBordersHelper.Chart.Series.Count > 0) && (chartBordersHelper.Chart.Series[0].GetPoints().Count == 1))
			{
				chartBordersHelper.Chart.Visible = false;

				GridBorderPlace.Visible = true;
				gridBorderHelper = new GridBorder(GridSimpleBorder);
				gridBorderHelper.SetLabels(GridBorderLabelParam.Terra, GridBorderLabelCount.People);
				gridBorderHelper.SetStyle();
				gridBorderHelper.SetFullAutoSizes();
				gridBorderHelper.SetData("skk_009_grid_borders");
				chartCompareHelper.SetOuterHeight(GridBorderPlace);
			}
			else
			{
				GridBorderPlace.Visible = false;
			}

			// по виду сообщения, таблица
			gridTransportHelper = new GridCountPeople(GridTransport);
			gridTransportHelper.SetStyle();
			gridTransportHelper.SetFullAutoSizes();
			gridTransportHelper.SetData("skk_009_grid_transport");

			// по виду сообщения, диаграмма
			chartTransportHelper = new ChartTransportPoints(ChartTransport);
			chartTransportHelper.SetStyle();
			chartTransportHelper.SetData("skk_009_grid_transport");
			
		}

		private void PdfExportButton_Click(object sender, EventArgs e)
		{
			SKKHelper.ExportPdfSetStyle(ReportPDFExporter1);
			
			const int ItemsCount = 6;
			ISection[] sections = new ISection[ItemsCount];
			Report report = new Report();

			ReportPDFExporter1.PageTitle = PageTitle.Text;
			ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;
			
			int currentSection = 0;

			// таблица рост/снижение
			sections[currentSection] = report.AddSection();
			ReportPDFExporter1.Export(
				gridUpDownHelper.Grid.GridHeaderLayout,
				LabelGrid.Text,
				sections[currentSection++]);

			// диаграмма - общее число (фо, субъекты, пп)
			sections[currentSection] = report.AddSection();
			ReportPDFExporter1.Export(
				chartTerraHelper.Chart,
				LabelChartTerra.Text,
				sections[currentSection++]);
			
			// сравнение, диаграмма
			sections[currentSection] = report.AddSection();
			ReportPDFExporter1.Export(
				chartCompareHelper.Chart,
				LabelChartCompare.Text,
				sections[currentSection++]);

			// по участкам границы, диаграмма
			sections[currentSection] = report.AddSection();
			if ((chartBordersHelper.Chart.Series.Count > 0) && (chartBordersHelper.Chart.Series[0].GetPoints().Count == 1))
			{
				// таблица
				ReportPDFExporter1.Export(
					gridBorderHelper.Grid.GridHeaderLayout,
					LabelChartBorder.Text,
					sections[currentSection++]);
			}
			else
			{
				// диаграмма
				ReportPDFExporter1.Export(
					chartBordersHelper.Chart,
					LabelChartBorder.Text,
					sections[currentSection++]);
			}

			// по виду сообщения, таблица
			sections[currentSection] = report.AddSection();
			ReportPDFExporter1.HeaderCellHeight = 48;
			ReportPDFExporter1.Export(
				gridTransportHelper.Grid.GridHeaderLayout,
				LabelTransport.Text,
				sections[currentSection++]);

			// по виду сообщения, диаграмма
			sections[currentSection] = report.AddSection();
			ReportPDFExporter1.Export(
				chartTransportHelper.Chart,
				LabelTransport.Text,
				sections[currentSection++]);
			
		}

		private void ExcelExportButton_Click(object sender, EventArgs e)
		{
			SKKHelper.ExportExcelSetStyle(ReportExcelExporter1);
			
			const int ItemsCount = 6;
			Worksheet[] sheets = new Worksheet[ItemsCount];
			Workbook workbook = new Workbook();
			
			ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
			ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;
			
			int currentSheet = 0;

			// таблица рост/снижение
			sheets[currentSheet] = workbook.Worksheets.Add("sheet" + (currentSheet + 1));
			ReportExcelExporter1.Export(
				gridUpDownHelper.Grid.GridHeaderLayout,
				LabelGrid.Text,
				sheets[currentSheet++], 
				3);

			// диаграмма - общее число (фо, субъекты, пп)
			sheets[currentSheet] = workbook.Worksheets.Add("sheet" + (currentSheet + 1));
			ReportExcelExporter1.Export(
				chartTerraHelper.Chart,
				LabelChartTerra.Text,
				sheets[currentSheet++],
				3);
			
			// сравнение, диаграмма
			sheets[currentSheet] = workbook.Worksheets.Add("sheet" + (currentSheet + 1));
			ReportExcelExporter1.Export(
				chartCompareHelper.Chart, 
				LabelChartCompare.Text,
				sheets[currentSheet++], 
				3);

			// по участкам границы, диаграмма
			sheets[currentSheet] = workbook.Worksheets.Add("sheet" + (currentSheet + 1));
			if ((chartBordersHelper.Chart.Series.Count > 0) && (chartBordersHelper.Chart.Series[0].GetPoints().Count == 1))
			{
				// таблица
				ReportExcelExporter1.Export(
					gridBorderHelper.Grid.GridHeaderLayout,
					LabelChartBorder.Text,
					sheets[currentSheet++],
					3);
			}
			else
			{
				// диаграмма
				ReportExcelExporter1.Export(
					chartBordersHelper.Chart,
					LabelChartBorder.Text,
					sheets[currentSheet++],
					3);
			}

			// по виду сообщения, таблица
			sheets[currentSheet] = workbook.Worksheets.Add("sheet" + (currentSheet + 1));
			ReportExcelExporter1.Export(
				gridTransportHelper.Grid.GridHeaderLayout, 
				LabelTransport.Text,
				sheets[currentSheet++], 
				3);

			// по виду сообщения, диаграмма
			sheets[currentSheet] = workbook.Worksheets.Add("sheet" + (currentSheet + 1));
			ReportExcelExporter1.Export(
				chartTransportHelper.Chart,
 				LabelTransport.Text,
				sheets[currentSheet++], 
				3);
			
		}

	}

}
