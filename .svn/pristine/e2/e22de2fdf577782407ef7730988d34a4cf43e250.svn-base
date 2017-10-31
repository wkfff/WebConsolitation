using System;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.SkkSupport;

namespace Krista.FM.Server.Dashboards.reports.SKK.SKK_006
{
	public partial class Default : CustomReportPage
	{
		private ChartBase chartDistrictsHelper;
		private ChartBase chartBordersHelper;
		private GridBorder gridBorderHelper;
		private GridGroups gridGroupsHelper;
		private ChartGroupsCommon chartGroupsHelper;
		
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

			PageTitle.Text = "Досмотр грузов в пунктах пропуска выбранного вида международного сообщения";
			Page.Title = PageTitle.Text;
			PageSubTitle.Text = String.Format("Досмотр грузов в пунктах пропуска вида сообщения \"<b>{0}</b>\" за <b>{1} {2} года</b> в сравнении с <b>{3} {4} года</b>, направление: <b>{5}</b>",
				comboTransport.SelectedValue.ToLower(),
				comboPeriodMonth.SelectedValuesString.ToLower(), 
				comboPeriodYear.SelectedValue,
				selectedMonthAblativeCompare, 
				comboPeriodCompareYear.SelectedValue,
				CRHelper.ToLowerFirstSymbol(comboDirection.SelectedValue.Trim())
			);
			txtPeriod.Text = "Отчетный период";
			txtPeriodCompare.Text = "Период для сравнения";
			LabelChartDistrict.Text = "Досмотр грузов по федеральным округам";
			LabelChartBorder.Text = "Досмотр грузов по участкам границы";
			LabelGridGroups.Text = "Досмотр грузов по группам товаров";
			LabelChartCount.Text = "Число досмотренных партий грузов";
			LabelChartCapacity.Text = "Объем досмотренных партий грузов";

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
			// диаграмма по федеральным округам
			chartDistrictsHelper = new ChartDistrictsGoods(ChartDistrict);
			chartDistrictsHelper.SetStyle(SKKHelper.default2ItemsWidth, SKKHelper.defaultChartBordersHeight);
			chartDistrictsHelper.SetData("skk_006_chart_districts");

			// диаграмма по участкам границы
			GridBorderPlace.Visible = false;
			chartBordersHelper = new ChartBordersGoods(ChartBorder);
			chartBordersHelper.TitleNoData = SKKHelper.noDataBordersByTransport;
			chartBordersHelper.SetStyle(SKKHelper.default2ItemsWidth, SKKHelper.defaultChartHeight);
			chartDistrictsHelper.SetChartHeight(chartBordersHelper.ChartHeight);
			if (!comboTransport.SelectedValue.Trim().ToLower().Equals("воздушный")
				&& !comboTransport.SelectedValue.Trim().ToLower().Equals("морской")
				&& !comboTransport.SelectedValue.Trim().ToLower().Equals("речной"))
			{
				chartBordersHelper.SetChartHeight(SKKHelper.defaultChartBordersHeight);
				chartBordersHelper.SetData("skk_006_chart_borders");
				chartDistrictsHelper.SetChartHeight(chartBordersHelper.ChartHeight);

				if ((chartBordersHelper.Chart.Series.Count > 0) && (chartBordersHelper.Chart.Series[0].GetPoints().Count == 1))
				{
					chartBordersHelper.Chart.Visible = false;

					GridBorderPlace.Visible = true;
					gridBorderHelper = new GridBorder(GridSimpleBorder);
					gridBorderHelper.SetLabels(GridBorderLabelParam.Transport, GridBorderLabelCount.Goods);
					gridBorderHelper.SetStyle();
					gridBorderHelper.SetFullAutoSizes();
					gridBorderHelper.SetData("skk_006_grid_borders");
					chartDistrictsHelper.SetOuterHeight(GridBorderPlace);
				}
			}

			// таблица по группам товаров
			gridGroupsHelper = new GridGroups(GridBrick);
			gridGroupsHelper.SetStyle();
			gridGroupsHelper.SetData("skk_006_grid_groups");

			// диаграммы по группам товаров
			chartGroupsHelper = new ChartGroupsCommon(ChartCount, ChartCapacity);
			chartGroupsHelper.SetStyle();
			chartGroupsHelper.SetData("skk_006_chart_groups");
		}

		private void PdfExportButton_Click(object sender, EventArgs e)
		{
			SKKHelper.ExportPdfSetStyle(ReportPDFExporter1);

			const int ItemsCount = 5;
			ISection[] sections = new ISection[ItemsCount];
			Report report = new Report();

			ReportPDFExporter1.PageTitle = PageTitle.Text;
			ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;
			
			int currentSection = 0;

			// по округам
			sections[currentSection] = report.AddSection();
			ReportPDFExporter1.Export(
				chartDistrictsHelper.Chart,
				LabelChartDistrict.Text,
				sections[currentSection++]);

			// по участкам границы
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

			// по группам товаров, таблица
			gridGroupsHelper.SetConstColumnWidth();
			sections[currentSection] = report.AddSection();
			ReportPDFExporter1.Export(
				gridGroupsHelper.Grid.GridHeaderLayout,
				LabelGridGroups.Text,
				sections[currentSection++]);

			// по группам товаров, диаграмма-единицы
			sections[currentSection] = report.AddSection();
			ReportPDFExporter1.Export(
				chartGroupsHelper.ChartCount.Chart, 
				LabelChartCount.Text,
				sections[currentSection++]);

			// по группам товаров, диаграмма-тонны
			sections[currentSection] = report.AddSection();
			ReportPDFExporter1.Export(
				chartGroupsHelper.ChartVolume.Chart, 
				LabelChartCapacity.Text,
				sections[currentSection++]);
		}

		private void ExcelExportButton_Click(object sender, EventArgs e)
		{
			SKKHelper.ExportExcelSetStyle(ReportExcelExporter1);

			const int ItemsCount = 5;
			Worksheet[] sheets = new Worksheet[ItemsCount];
			Workbook workbook = new Workbook();

			ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
			ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;
			
			int currentSheet = 0;

			// по округам
			sheets[currentSheet] = workbook.Worksheets.Add("sheet" + (currentSheet + 1));
			ReportExcelExporter1.Export(
				chartDistrictsHelper.Chart,
				LabelChartDistrict.Text,
				sheets[currentSheet++], 
				3);

			// по участкам границы
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
			
			// по группам товаров, диаграмма-единицы
			sheets[currentSheet] = workbook.Worksheets.Add("sheet" + (currentSheet + 1));
			ReportExcelExporter1.Export(
				chartGroupsHelper.ChartCount.Chart,
				LabelChartCount.Text,
				sheets[currentSheet++], 
				3);
			
			// по группам товаров, диаграмма-тонны
			sheets[currentSheet] = workbook.Worksheets.Add("sheet" + (currentSheet + 1));
			ReportExcelExporter1.Export(
				chartGroupsHelper.ChartVolume.Chart,
				LabelChartCapacity.Text,
				sheets[currentSheet++], 
				3);
		}

	}

}
