using System;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.SkkSupport;

namespace Krista.FM.Server.Dashboards.reports.SKK.SKK_005
{
	public partial class Default : CustomReportPage
	{
		private ChartBase chartTerraHelper;
		private ChartBase chartBordersHelper;
		private ChartBase chartTransportHelper;
		private GridBase gridTransportHelper;
		private GridBorder gridBorderHelper;
		private GridGroups gridGroupsHelper;
		private ChartGroupsCommon chartGroupsHelper;

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

			PageTitle.Text = "Досмотр грузов на выбранной территории";
			Page.Title = PageTitle.Text;
			PageSubTitle.Text = String.Format("Досмотр грузов на территории \"<b>{0}</b>\" за <b>{1} {2} года</b> в сравнении с <b>{3} {4} года</b>, направление: <b>{5}</b>",
				comboTerra.SelectedValue,
				comboPeriodMonth.SelectedValuesString.ToLower(), 
				comboPeriodYear.SelectedValue,
				selectedMonthAblativeCompare, 
				comboPeriodCompareYear.SelectedValue,
				CRHelper.ToLowerFirstSymbol(comboDirection.SelectedValue.Trim())
			);
			txtPeriod.Text = "Отчетный период";
			txtPeriodCompare.Text = "Период для сравнения";

			LabelChartTerra.Text = "";
			LabelChartBorder.Text = "Досмотр грузов по участкам границы"; 
			LabelGridGroups.Text = "Досмотр грузов по группам товаров";
			LabelChartCount.Text = "Число досмотренных партий грузов";
			LabelChartCapacity.Text = "Объем досмотренных партий грузов";
			LabelTransportGroup.Text = "Досмотр грузов по виду международного сообщения";

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
			// диаграмма - общее число
			if ((comboTerra.SelectedNode.Level == 0) && (comboTerra.SelectedValue == "РФ"))
			{
				// по ФО
				chartTerraHelper = new ChartDistrictsGoods(ChartTerra);
				chartTerraHelper.SetStyle(SKKHelper.default2ItemsWidth, SKKHelper.defaultChartBordersHeight);
				LabelChartTerra.Text = "Досмотр грузов по федеральным округам";
				chartTerraHelper.SetData("skk_005_chart_districts");
			}
			else if (comboTerra.SelectedNode.Level == 0)
			{
				// по субъектам
				chartTerraHelper = new ChartSubjectsGoods(ChartTerra);
				chartTerraHelper.SetStyle(SKKHelper.default2ItemsWidth, SKKHelper.defaultChartBordersHeight);
				LabelChartTerra.Text = "Досмотр грузов по субъектам";
				chartTerraHelper.SetData("skk_005_chart_subjects");
			}
			else if (comboTerra.SelectedNode.Level == 1)
			{
				// по пунктам пропуска
				chartTerraHelper = new ChartPointsGoods(ChartTerra);
				chartTerraHelper.SetStyle(SKKHelper.default2ItemsWidth, SKKHelper.defaultChartBordersHeight);
				LabelChartTerra.Text = "Досмотр грузов по пунктам пропуска";
				chartTerraHelper.SetData("skk_005_chart_points");
			}

			// диаграмма по участкам границы
			chartBordersHelper = new ChartBordersGoods(ChartBorder);
			chartBordersHelper.TitleNoData = SKKHelper.noDataBordersByTerra;
			chartBordersHelper.SetStyle(SKKHelper.default2ItemsWidth, SKKHelper.defaultChartBordersHeight);
			chartBordersHelper.SetData("skk_005_chart_borders");
			chartTerraHelper.SetChartHeight(chartBordersHelper.ChartHeight);
			if ((chartBordersHelper.Chart.Series.Count > 0) && (chartBordersHelper.Chart.Series[0].GetPoints().Count == 1))
			{
				chartBordersHelper.Chart.Visible = false;

				GridBorderPlace.Visible = true;
				gridBorderHelper = new GridBorder(GridSimpleBorder);
				gridBorderHelper.SetLabels(GridBorderLabelParam.Terra, GridBorderLabelCount.Goods);
				gridBorderHelper.SetStyle(); 
				gridBorderHelper.SetFullAutoSizes();
				gridBorderHelper.SetData("skk_005_grid_borders");
				chartTerraHelper.SetOuterHeight(GridBorderPlace);
			}
			else
			{
				GridBorderPlace.Visible = false;
			}

			// таблица по группам товаров
			gridGroupsHelper = new GridGroups(GridGroups);
			gridGroupsHelper.SetStyle();
			gridGroupsHelper.SetFullAutoSizes();
			gridGroupsHelper.SetData("skk_005_grid_groups");

			// диаграммы по группам товаров
			chartGroupsHelper = new ChartGroupsCommon(ChartGroupsCount, ChartGroupsVolume);
			chartGroupsHelper.SetStyle();
			chartGroupsHelper.SetData("skk_005_chart_groups");

			// таблица по виду сообщения
			gridTransportHelper = new GridCountGoods(GridTransport);
			gridTransportHelper.SetStyle();
			gridTransportHelper.SetFullAutoSizes();
			gridTransportHelper.SetData("skk_005_grid_transport");

			// диаграмма по виду сообщения
			chartTransportHelper = new ChartTransportPoints(ChartTransport);
			chartTransportHelper.SetStyle();
			chartTransportHelper.SetData("skk_005_grid_transport");
		}

		private void PdfExportButton_Click(object sender, EventArgs e)
		{
			SKKHelper.ExportPdfSetStyle(ReportPDFExporter1);
			
			const int ItemsCount = 7;
			ISection[] sections = new ISection[ItemsCount];
			Report report = new Report();

			ReportPDFExporter1.PageTitle = PageTitle.Text;
			ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;
			
			int currentSection = 0;

			// диаграмма - общее число (фо, субъекты, пункты пропуска)
			sections[currentSection] = report.AddSection();
			ReportPDFExporter1.Export(
				chartTerraHelper.Chart,
				LabelChartTerra.Text,
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

			// таблица по группам товаров
			gridGroupsHelper.SetConstColumnWidth();
			sections[currentSection] = report.AddSection();
			ReportPDFExporter1.Export(
				gridGroupsHelper.Grid.GridHeaderLayout,
				LabelGridGroups.Text,
				sections[currentSection++]);

			// по группам товаров, диаграмма единиц
			sections[currentSection] = report.AddSection();
			ReportPDFExporter1.Export(
				chartGroupsHelper.ChartCount.Chart,
				LabelChartCount.Text,
				sections[currentSection++]);

			// по группам товаров, диаграмма тонн
			sections[currentSection] = report.AddSection();
			ReportPDFExporter1.Export(
				chartGroupsHelper.ChartVolume.Chart,
				LabelChartCapacity.Text,
				sections[currentSection++]);

			// по виду сообщения, таблица
			sections[currentSection] = report.AddSection();
			ReportPDFExporter1.HeaderCellHeight = 38;
			ReportPDFExporter1.Export(
				gridTransportHelper.Grid.GridHeaderLayout,
				LabelTransportGroup.Text,
				sections[currentSection++]);

			// по виду сообщения, диаграмма
			if (chartTransportHelper.Chart.Series.Count > 0)
			{
				sections[currentSection] = report.AddSection();
				ReportPDFExporter1.Export(
					chartTransportHelper.Chart,
					LabelTransportGroup.Text,
					sections[currentSection++]); 
			}

		}

		private void ExcelExportButton_Click(object sender, EventArgs e)
		{
			SKKHelper.ExportExcelSetStyle(ReportExcelExporter1);
			
			const int ItemsCount = 7;
			Worksheet[] sheets = new Worksheet[ItemsCount];
			Workbook workbook = new Workbook();
			
			ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
			ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;
			
			int currentSheet = 0;

			// диаграмма - общее число (фо, субъекты, пункты пропуска)
			sheets[currentSheet] = workbook.Worksheets.Add("sheet" + (currentSheet + 1));
			ReportExcelExporter1.Export(
				chartTerraHelper.Chart,
 				LabelChartTerra.Text,
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

			// по группам товаров, таблица
			sheets[currentSheet] = workbook.Worksheets.Add("sheet" + (currentSheet + 1));
			ReportExcelExporter1.Export(
				gridGroupsHelper.Grid.GridHeaderLayout, 
				LabelGridGroups.Text, 
				sheets[currentSheet++], 
				3);

			// по группам товаров, диаграмма единиц
			sheets[currentSheet] = workbook.Worksheets.Add("sheet" + (currentSheet + 1));
			ReportExcelExporter1.Export(
				chartGroupsHelper.ChartCount.Chart, 
				LabelChartCount.Text, 
				sheets[currentSheet++], 
				3);

			// по группам товаров, диаграмма тонн
			sheets[currentSheet] = workbook.Worksheets.Add("sheet" + (currentSheet + 1));
			ReportExcelExporter1.Export(
				chartGroupsHelper.ChartVolume.Chart, 
				LabelChartCapacity.Text,
				sheets[currentSheet++], 
				3);

			// по виду сообщения, таблица
			sheets[currentSheet] = workbook.Worksheets.Add("sheet" + (currentSheet + 1));
			ReportExcelExporter1.Export(
				gridTransportHelper.Grid.GridHeaderLayout,
 				LabelTransportGroup.Text,
				sheets[currentSheet++], 
				3);

			// по виду сообщения, диаграмма
			if (chartTransportHelper.Chart.Series.Count > 0)
			{
				sheets[currentSheet] = workbook.Worksheets.Add("sheet" + (currentSheet + 1));
				ReportExcelExporter1.Export(
					chartTransportHelper.Chart,
					LabelTransportGroup.Text,
					sheets[currentSheet++],
					3);
			}

		}

	}

}
