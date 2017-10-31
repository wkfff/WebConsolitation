using System;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.SkkSupport;

namespace Krista.FM.Server.Dashboards.reports.SKK.SKK_007
{
	public partial class Default : CustomReportPage
	{
		private ChartBase chartSubjectsHelper;
		private ChartBase chartTransportHelper;
		private GridBase gridTransportHelper;
		private GridGroups gridGroupsHelper;
		private ChartGroupsCommon chartGroupsHelper;
		
		// параметры запроса
		private static MemberAttributesDigest digestBorder;
		private static MemberAttributesDigest digestDirection;
		private CustomParam selectedBorder;
		private CustomParam selectedPeriod;
		private CustomParam selectedPeriodCompare;
		private CustomParam selectedDirection;


		protected override void Page_PreLoad(object sender, EventArgs e)
		{
			base.Page_PreLoad(sender, e);

			// Инициализация параметров запроса
			selectedBorder = UserParams.CustomParam("selected_border");
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
				
				// участок границы
				digestBorder = SKKHelper.DigestBorder_Init();
				comboBorder.ComboBorder_Init(digestBorder);
				
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

			PageTitle.Text = "Досмотр грузов на выбранном участке Государственной границы РФ";
			Page.Title = PageTitle.Text;
			PageSubTitle.Text = String.Format("Досмотр грузов на участке Государственной границы РФ \"<b>{0}</b>\" за <b>{1} {2} года</b> в сравнении с <b>{3} {4} года</b>, направление: <b>{5}</b>",
				comboBorder.SelectedValue,
				comboPeriodMonth.SelectedValuesString.ToLower(), 
				comboPeriodYear.SelectedValue,
				selectedMonthAblativeCompare, 
				comboPeriodCompareYear.SelectedValue,
				CRHelper.ToLowerFirstSymbol(comboDirection.SelectedValue.Trim())
			);
			txtPeriod.Text = "Отчетный период";
			txtPeriodCompare.Text = "Период для сравнения";
			LabelChartSubjects.Text = "Досмотр грузов по субъектам";
			LabelGridGroups.Text = "Досмотр грузов по группам товаров";
			LabelChartCount.Text = "Число досмотренных партий грузов";
			LabelChartCapacity.Text = "Объем досмотренных партий грузов";
			LabelTransportGroup.Text = "Досмотр грузов по виду международного сообщения";

			// параметры для запроса

			selectedBorder.Value = digestBorder.GetMemberUniqueName(comboBorder.SelectedValue);
			selectedPeriod.Value = selectedMonthUnique;
			selectedPeriodCompare.Value = selectedMonthUniqueCompare;
			selectedDirection.Value = digestDirection.GetMemberUniqueName(comboDirection.SelectedValue);

			// инициализация и запуск

			InitHelpers();
			SetDataHelpers();
			
		}

		private void InitHelpers()
		{
			// диаграмма по субъектам
			chartSubjectsHelper = new ChartSubjectsGoods(ChartSubjects);
			chartSubjectsHelper.SetStyle(SKKHelper.default1ItemsWidth, SKKHelper.defaultChartHeight);

			// таблица по группам товаров
			gridGroupsHelper = new GridGroups(GridGroups);
			gridGroupsHelper.SetStyle();

			// диаграммы по группам товаров
			chartGroupsHelper = new ChartGroupsCommon(ChartGroupsCount, ChartGroupsVolume);
			chartGroupsHelper.SetStyle();

			// таблица по виду сообщения
			gridTransportHelper = new GridCountGoods(GridTransport);
			gridTransportHelper.SetStyle();

			// диаграмма по виду сообщения
			chartTransportHelper = new ChartTransportPoints(ChartTransport);
			chartTransportHelper.SetStyle();
		}

		private void SetDataHelpers()
		{
			chartSubjectsHelper.SetData("skk_007_chart_subjects");
			gridGroupsHelper.SetData("skk_007_grid_groups");
			chartGroupsHelper.SetData("skk_007_chart_groups");
			gridTransportHelper.SetData("skk_007_grid_transport");
			chartTransportHelper.SetData("skk_007_grid_transport");
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

			// по субъектам
			sections[currentSection] = report.AddSection();
			ReportPDFExporter1.Export(
				chartSubjectsHelper.Chart,
				LabelChartSubjects.Text,
				sections[currentSection++]);
			
			// по группам товаров, таблица
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
			
			const int ItemsCount = 6;
			Worksheet[] sheets = new Worksheet[ItemsCount];
			Workbook workbook = new Workbook();
			
			ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
			ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;
			
			int currentSheet = 0;

			// по субъектам
			sheets[currentSheet] = workbook.Worksheets.Add("sheet" + (currentSheet + 1));
			ReportExcelExporter1.Export(
				chartSubjectsHelper.Chart,
 				LabelChartSubjects.Text,
				sheets[currentSheet++], 
				3);
			
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
