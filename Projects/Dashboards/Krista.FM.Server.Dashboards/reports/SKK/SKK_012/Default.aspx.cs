using System;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.SkkSupport;

namespace Krista.FM.Server.Dashboards.reports.SKK.SKK_012
{
	public partial class Default : CustomReportPage
	{
		private GridCmpBase gridPeopleHelper;
		private ChartCmpBase chartPeopleHelper;

		// Параметры запроса
		private static MemberAttributesDigest digestPoint;
		private static MemberAttributesDigest digestDirection;
		private CustomParam selectedPoint;
		private CustomParam selectedPeriod;
		private CustomParam selectedPeriodCompare;
		private CustomParam selectedDirection;


		protected override void Page_PreLoad(object sender, EventArgs e)
		{
			base.Page_PreLoad(sender, e);

			// Инициализация параметров запроса
			selectedPoint = UserParams.CustomParam("selected_point");
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

				// пункт пропуска
				digestPoint = SKKHelper.DigestPoint_Init();
				comboPoint.ComboPoint_Init(digestPoint);

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
			if (comboPoint.SelectedNode.Level == 1)
			{
				comboPoint.SetСheckedState(comboPoint.SelectedNode.FirstNode.Text, true);
			}

			// сбор параметров

			string selectedMonthUnique = SKKHelper.GetUniqueMonths(comboPeriodMonth.SelectedValues, comboPeriodYear.SelectedValue);
			string selectedMonthUniqueCompare = SKKHelper.GetUniqueMonths(comboPeriodCompareMonth.SelectedValues, comboPeriodCompareYear.SelectedValue);
			string selectedMonthAblativeCompare = SKKHelper.GetAblativeMonths(comboPeriodCompareMonth.SelectedValues);
			
			// текстовики

			PageTitle.Text = "Досмотр лиц в выбранном пункте пропуска";
			Page.Title = PageTitle.Text;

			PageSubTitle.Text = String.Format("Досмотр лиц в пункте пропуска \"<b>{0}</b>\" за <b>{1} {2} года</b> в сравнении с <b>{3} {4} года</b>, направление: <b>{5}</b>",
				comboPoint.SelectedValue,
				comboPeriodMonth.SelectedValuesString.ToLower(), 
				comboPeriodYear.SelectedValue,
				selectedMonthAblativeCompare, 
				comboPeriodCompareYear.SelectedValue,
				CRHelper.ToLowerFirstSymbol(comboDirection.SelectedValue.Trim())
			);

			txtPeriod.Text = "Отчетный период";
			txtPeriodCompare.Text = "Период для сравнения";

			// параметры для запроса

			selectedPoint.Value = digestPoint.GetMemberUniqueName(comboPoint.SelectedValue);
			selectedPeriod.Value = selectedMonthUnique;
			selectedPeriodCompare.Value = selectedMonthUniqueCompare;
			selectedDirection.Value = digestDirection.GetMemberUniqueName(comboDirection.SelectedValue);

			// инициализация и запуск
			SetDataHelpers();
		}

		private void SetDataHelpers()
		{
			// таблица
			gridPeopleHelper = new GridCmpPeople(GridBrick);
			gridPeopleHelper.SetStyle();
			gridPeopleHelper.SetData("skk_012_grid");

			// диаграмма
			chartPeopleHelper = new ChartCmpPeople(Chart);
			chartPeopleHelper.SetStyle();
			chartPeopleHelper.Series1Label = String.Format("{0} {1} года", comboPeriodMonth.SelectedValuesString.ToLower(), comboPeriodYear.SelectedValue);
			chartPeopleHelper.Series2Label = String.Format("{0} {1} года", comboPeriodCompareMonth.SelectedValuesString.ToLower(), comboPeriodCompareYear.SelectedValue);
			chartPeopleHelper.SetData("skk_012_chart");
		}

		private void PdfExportButton_Click(object sender, EventArgs e)
		{
			SKKHelper.ExportPdfSetStyle(ReportPDFExporter1);

			Report report = new Report();
			ISection section1 = report.AddSection();
			
			ReportPDFExporter1.PageTitle = PageTitle.Text;
			ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;
			
			ReportPDFExporter1.Export(gridPeopleHelper.Grid.GridHeaderLayout, section1);
			ReportPDFExporter1.Export(chartPeopleHelper.Chart, section1);
		}

		private void ExcelExportButton_Click(object sender, EventArgs e)
		{
			SKKHelper.ExportExcelSetStyle(ReportExcelExporter1);

			Workbook workbook = new Workbook();
			Worksheet sheet1 = workbook.Worksheets.Add("sheet1");
			Worksheet sheet2 = workbook.Worksheets.Add("sheet2");
			
			ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
			ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

			ReportExcelExporter1.Export(gridPeopleHelper.Grid.GridHeaderLayout, sheet1, 3);
			ReportExcelExporter1.Export(chartPeopleHelper.Chart, sheet2, 3);
		}

	}

}
