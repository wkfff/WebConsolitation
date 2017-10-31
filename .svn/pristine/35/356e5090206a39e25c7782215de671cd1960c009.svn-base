using System;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.SkkSupport;

namespace Krista.FM.Server.Dashboards.reports.SKK.SKK_002
{
	public partial class Default : CustomReportPage
	{
		private GridFull gridUpDownHelper;
		
		// параметры запроса
		private static MemberAttributesDigest digestBorder;
		private static MemberAttributesDigest digestTransport;
		private static MemberAttributesDigest digestDirection;
		private CustomParam selectedBorder;
		private CustomParam selectedTransport;
		private CustomParam selectedPeriod;
		private CustomParam selectedPeriodCompare;
		private CustomParam selectedDirection;
		private CustomParam selectedDetail;


		protected override void Page_PreLoad(object sender, EventArgs e)
		{
			base.Page_PreLoad(sender, e);

			// Инициализация параметров запроса
			selectedBorder = UserParams.CustomParam("selected_border");
			selectedTransport = UserParams.CustomParam("selected_transport");
			selectedPeriod = UserParams.CustomParam("selected_period");
			selectedPeriodCompare = UserParams.CustomParam("selected_period_compare");
			selectedDirection = UserParams.CustomParam("selected_direction");
			selectedDetail = UserParams.CustomParam("selected_detail");

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

				// вид сообщения
				digestTransport = SKKHelper.DigestTransport_Init();
				comboTransport.ComboTransport_Init(digestTransport);
				
				// детализировать по пунктам пропуска
				checkDetailPP.CheckDetailPP_Init();
			}

			// пост-инициализация параметров

			checkDetailPP.CheckDetailPP_PostInit();

			// изменение результатов запроса

			SKKHelper.ChangeSelectedPeriod(comboPeriodMonth);
			SKKHelper.ChangeSelectedPeriodCompare(comboPeriodCompareMonth);

			// сбор параметров

			string selectedMonthUnique = SKKHelper.GetUniqueMonths(comboPeriodMonth.SelectedValues, comboPeriodYear.SelectedValue);
			string selectedMonthUniqueCompare = SKKHelper.GetUniqueMonths(comboPeriodCompareMonth.SelectedValues, comboPeriodCompareYear.SelectedValue);
			string selectedMonthAblativeCompare = SKKHelper.GetAblativeMonths(comboPeriodCompareMonth.SelectedValues);
			
			// текстовики

			PageTitle.Text = "Санитарно-карантинный контроль на выбранном участке Государственной границы РФ. Анализ по субъектам";
			Page.Title = PageTitle.Text;
			PageSubTitle.Text = String.Format("Санитарно-карантинный контроль на участке Государственной границы РФ \"<b>{0}</b>\" за <b>{1} {2} года</b> в сравнении с <b>{3} {4} года</b>, направление: <b>{5}</b>, вид международного сообщения: <b>{6}</b>",
				comboBorder.SelectedValue,
				comboPeriodMonth.SelectedValuesString.ToLower(), 
				comboPeriodYear.SelectedValue,
				selectedMonthAblativeCompare, 
				comboPeriodCompareYear.SelectedValue,
				CRHelper.ToLowerFirstSymbol(comboDirection.SelectedValue.Trim()),
				comboTransport.SelectedValue.ToLower()
			);
			txtPeriod.Text = "Отчетный период";
			txtPeriodCompare.Text = "Период для сравнения";
			
			// параметры для запроса

			selectedBorder.Value = digestBorder.GetMemberUniqueName(comboBorder.SelectedValue);
			selectedTransport.Value = digestTransport.GetMemberUniqueName(comboTransport.SelectedValue);
			selectedPeriod.Value = selectedMonthUnique;
			selectedPeriodCompare.Value = selectedMonthUniqueCompare;
			selectedDirection.Value = digestDirection.GetMemberUniqueName(comboDirection.SelectedValue);
			selectedDetail.Value = checkDetailPP.CheckDetailPP_GetDetailBorder();
			
			// инициализация и запуск
			SetDataHelpers();
		}

		private void SetDataHelpers()
		{
			gridUpDownHelper = new GridFull(GridBrick);
			gridUpDownHelper.DeleteColumns = 1;
			gridUpDownHelper.SafeMode = (!Page.IsPostBack);
			gridUpDownHelper.SetStyle();
			gridUpDownHelper.SetData("skk_002_grid");
		}

		private void PdfExportButton_Click(object sender, EventArgs e)
		{
			SKKHelper.ExportPdfSetStyle(ReportPDFExporter1);
			
			const int ItemsCount = 1;
			ISection[] sections = new ISection[ItemsCount];
			Report report = new Report();

			ReportPDFExporter1.PageTitle = PageTitle.Text;
			ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;
			ReportPDFExporter1.HeaderCellHeight = 30;

			int currentSection = 0;

			// таблица
			gridUpDownHelper.SetConstColumnWidth();
			
			sections[currentSection] = report.AddSection();
			ReportPDFExporter1.Export(
				gridUpDownHelper.Grid.GridHeaderLayout,
				sections[currentSection++]);

		}

		private void ExcelExportButton_Click(object sender, EventArgs e)
		{
			SKKHelper.ExportExcelSetStyle(ReportExcelExporter1);
			
			const int ItemsCount = 1;
			Worksheet[] sheets = new Worksheet[ItemsCount];
			Workbook workbook = new Workbook();

			ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
			ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;
			
			int currentSheet = 0;

			// таблица
			sheets[currentSheet] = workbook.Worksheets.Add("sheet" + (currentSheet + 1));
			ReportExcelExporter1.RowsAutoFitEnable = false;
			ReportExcelExporter1.ExcelExporter.EndExport += ExcelExporter_EndExport;
			ReportExcelExporter1.Export(
				gridUpDownHelper.Grid.GridHeaderLayout,
				sheets[currentSheet++], 
				3);
		}

		private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
		{
			const int px = 15; // 20 * 0.75

			e.CurrentWorksheet.Rows[3].Height = 40 * px;
			e.CurrentWorksheet.Rows[4].Height = 30 * px;
			e.CurrentWorksheet.Rows[5].Height = 30 * px;
			e.CurrentWorksheet.Rows[6].Height = 30 * px;
			e.CurrentWorksheet.Rows[7].Height = 60 * px;

			const int startRow = 3 + 5;
			int rowsCount = gridUpDownHelper.Grid.GridHeaderLayout.Grid.Rows.Count;
			for (int i = startRow; i < startRow+rowsCount; i++)
			{
				e.CurrentWorksheet.Rows[i].Height = 22 * px;
			}
		}

	}

}
