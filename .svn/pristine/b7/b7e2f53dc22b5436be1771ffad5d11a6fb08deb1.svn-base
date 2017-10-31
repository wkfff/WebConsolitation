using System;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.SkkSupport;

namespace Krista.FM.Server.Dashboards.reports.SKK.SKK_011
{
	public partial class Default : CustomReportPage
	{
		private GridUpDownBase gridUpDownHelper;
		private ChartBase chartTerraHelper;
		private ChartCmpBase chartCompareHelper;
		private ChartTransportBase chartTransportHelper;
		private GridCountBase gridTransportHelper;
		
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

			PageTitle.Text = "Досмотр лиц на выбранном участке Государственной границы РФ";
			Page.Title = PageTitle.Text;
			PageSubTitle.Text = String.Format("Досмотр лиц на участке Государственной границы РФ \"<b>{0}</b>\" за <b>{1} {2} года</b> в сравнении с <b>{3} {4} года</b>, направление: <b>{5}</b>",
				comboBorder.SelectedValue,
				comboPeriodMonth.SelectedValuesString.ToLower(), 
				comboPeriodYear.SelectedValue,
				selectedMonthAblativeCompare, 
				comboPeriodCompareYear.SelectedValue,
				CRHelper.ToLowerFirstSymbol(comboDirection.SelectedValue.Trim())
			);
			txtPeriod.Text = "Отчетный период";
			txtPeriodCompare.Text = "Период для сравнения";
			LabelGridUpDown.Text = "Досмотр лиц на выбранном участке границы";
			LabelChartTerra.Text = "";
			LabelChartCompare.Text = "Досмотр лиц на выбранном участке границы";
			LabelTransport.Text = "Досмотр лиц по видам международного сообщения";
			
			// параметры для запроса

			selectedBorder.Value = digestBorder.GetMemberUniqueName(comboBorder.SelectedValue);
			selectedPeriod.Value = selectedMonthUnique;
			selectedPeriodCompare.Value = selectedMonthUniqueCompare;
			selectedDirection.Value = digestDirection.GetMemberUniqueName(comboDirection.SelectedValue);

			// инициализация и запуск
			SetDataHelpers();
		}
		
		private void SetDataHelpers()
		{
			// рост/снижение, таблица
			gridUpDownHelper = new GridUpDownPeople(GridUpDown);
			gridUpDownHelper.DeleteColumns = 1;
			gridUpDownHelper.SetStyle();
			gridUpDownHelper.LabelCommon = "Территория";
			gridUpDownHelper.SetData("skk_011_grid_updown");
			scriptBlock.JavaScript_SetVerticalScrollBar(gridUpDownHelper.Grid);

			// ФО или ПП, диаграмма
			if (comboBorder.SelectedValue.Trim().ToLower().Equals("всего"))
			{
				// по ФО
				LabelChartTerra.Text = "Досмотр лиц по федеральным округам ";
				chartTerraHelper = new ChartDistrictsPeople(ChartTerra);
				chartTerraHelper.SetStyle(SKKHelper.default2ItemsWidth, SKKHelper.defaultChartHeight);
				chartTerraHelper.SetData("skk_011_chart_districts");
			}
			else
			{
				// по пунктам пропуска
				LabelChartTerra.Text = "Досмотр лиц по пунктам пропуска";
				chartTerraHelper = new ChartPointsPeople(ChartTerra);
				chartTerraHelper.SetStyle(SKKHelper.default2ItemsWidth, SKKHelper.defaultChartHeight);
				chartTerraHelper.SetData("skk_011_chart_points");
			}

			// сравнение, диаграмма
			chartCompareHelper = new ChartCmpPeople(ChartCompare);
			chartCompareHelper.SetStyle(SKKHelper.default2ItemsWidth, SKKHelper.defaultChartHeight);
			chartCompareHelper.Series1Label = String.Format("{0} {1} года", comboPeriodMonth.SelectedValuesString.ToLower(), comboPeriodYear.SelectedValue);
			chartCompareHelper.Series2Label = String.Format("{0} {1} года", comboPeriodCompareMonth.SelectedValuesString.ToLower(), comboPeriodCompareYear.SelectedValue);
			chartCompareHelper.SetData("skk_011_chart_compare");

			// таблица по виду сообщения
			gridTransportHelper = new GridCountPeople(GridTransport);
			gridTransportHelper.SetStyle();
			gridTransportHelper.SetData("skk_011_grid_transport");

			// диаграмма по виду сообщения
			chartTransportHelper = new ChartTransportPoints(ChartTransport);
			chartTransportHelper.SetStyle();
			chartTransportHelper.SetData("skk_011_grid_transport"); 

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

			// рост/снижение, таблица
			sections[currentSection] = report.AddSection();
			ReportPDFExporter1.Export(
				gridUpDownHelper.Grid.GridHeaderLayout,
				LabelGridUpDown.Text,
				sections[currentSection++]);

			// ФО или ПП, диаграмма
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

			// по виду сообщения, таблица
			sections[currentSection] = report.AddSection();
			ReportPDFExporter1.HeaderCellHeight = 48;
			ReportPDFExporter1.Export(
				gridTransportHelper.Grid.GridHeaderLayout,
				LabelTransport.Text,
				sections[currentSection++]);

			// по виду сообщения, диаграмма
			if (chartTransportHelper.Chart.Series.Count > 0)
			{
				sections[currentSection] = report.AddSection();
				ReportPDFExporter1.Export(
					chartTransportHelper.Chart,
					LabelTransport.Text,
					sections[currentSection++]); 
			}

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

			// рост/снижение, таблица
			sheets[currentSheet] = workbook.Worksheets.Add("sheet" + (currentSheet + 1));
			ReportExcelExporter1.Export(
				gridUpDownHelper.Grid.GridHeaderLayout,
				LabelGridUpDown.Text,
				sheets[currentSheet++], 
				3);

			// ФО или ПП, диаграмма
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

			// по виду сообщения, таблица
			sheets[currentSheet] = workbook.Worksheets.Add("sheet" + (currentSheet + 1));
			ReportExcelExporter1.Export(
				gridTransportHelper.Grid.GridHeaderLayout,
				LabelTransport.Text,
				sheets[currentSheet++], 
				3);

			// по виду сообщения, диаграмма
			if (chartTransportHelper.Chart.Series.Count > 0)
			{
				sheets[currentSheet] = workbook.Worksheets.Add("sheet" + (currentSheet + 1));
				ReportExcelExporter1.Export(
					chartTransportHelper.Chart,
					LabelTransport.Text,
					sheets[currentSheet++],
					3);
			}

		}

	}

}
