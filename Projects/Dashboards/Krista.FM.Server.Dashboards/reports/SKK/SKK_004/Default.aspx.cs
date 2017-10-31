using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.SkkSupport;

namespace Krista.FM.Server.Dashboards.reports.SKK.SKK_004
{
	public partial class Default : CustomReportPage
	{
		private GridMark gridBordersHelper;
		private GridMark gridSubjectsHelper;
		private MapRF mapRFHelper;
		private MapSubject mapSubjectHelper;

		public int comboMarkIndex;
		public string markFormat = "N0";
		public Dictionary<string, string> border2country;
		public Dictionary<int, string> mark2unit;
		public Collection<string> subjects;

		// параметры запроса
		private static MemberAttributesDigest digestMark;
		private static MemberAttributesDigest digestTransport;
		private static MemberAttributesDigest digestDirection;
		private CustomParam selectedMark;
		private CustomParam selectedTransport;
		private CustomParam selectedPeriod;
		private CustomParam selectedDirection;
		public CustomParam selectedSubject;

		public Default()
		{
			subjects = new Collection<string>();

			border2country = new Dictionary<string, string>
			{
			    {"Российско-норвежский", "Норвегия"},
			    {"Российско-финский", "Финляндия"},
			    {"Российско-эстонский", "Эстония"},
			    {"Российско-латвийский", "Латвия"},
			    {"Российско-литовский", "Литва"},
			    {"Российско-польский", "Польша"},
			    {"Российско-украинский", "Украина"},
			    {"Российско-абхазский", "Абхазия"},
			    {"Российско-южноосетинский", "Южная Осетия"},
			    {"Российско-грузинский", "Грузия"},
			    {"Российско-азербайджанский", "Азербайджан"},
			    {"Российско-казахстанский", "Казахстан"},
			    {"Российско-монгольский", "Монголия"},
			    {"Российско-китайский", "Китай"},
			    {"Российско-корейский", "Северная Корея"}
			};

			mark2unit = new Dictionary<int, string>
			{
			    {0, "Досмотрено ТС"},
			    {1, "Приостановлено ТС"},
			    {2, "Досмотрено лиц"},
			    {3, "Выявлено лиц"},
			    {4, "Досмотрено партий грузов, всего"},
			    {5, "Досмотрено партий грузов"},
			    {6, "Приостановлено партий грузов"},
			    {7, "Досмотрено тонн"},
			    {8, "Приостановлено тонн"}
			};
		}

		protected override void Page_PreLoad(object sender, EventArgs e)
		{
			base.Page_PreLoad(sender, e);
			
			// Инициализация параметров запроса
			selectedMark = UserParams.CustomParam("selected_mark");
			selectedTransport = UserParams.CustomParam("selected_transport");
			selectedPeriod = UserParams.CustomParam("selected_period");
			selectedDirection = UserParams.CustomParam("selected_direction");
			selectedSubject = UserParams.CustomParam("selected_subject");

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
				digestMark = SKKHelper.DigestMark_Init();
				comboMark.ComboMark_Init(digestMark);
				
				// отчетный период
				comboPeriodYear.ComboCurrentYear_Init(paramDate.GetFirstYear(), paramDate.GetLastYear(), paramDate.GetLastYear());
				comboPeriodMonth.ComboCurrentMonth_Init(paramDate.GetLastMonth());
				
				// направление
				digestDirection = SKKHelper.DigestDirection_Init();
				comboDirection.ComboDirection_Init(digestDirection);

				// вид сообщения
				digestTransport = SKKHelper.DigestTransport_Init();
				comboTransport.ComboTransport_Init(digestTransport);

				// субъект на карте
				selectedSubject.Value = String.Empty;
			}

			// изменение результатов запроса

			SKKHelper.ChangeSelectedPeriod(comboPeriodMonth);

			// сбор параметров

			comboMarkIndex = comboMark.SelectedIndex;
			string selectedMonthUnique = SKKHelper.GetUniqueMonths(comboPeriodMonth.SelectedValues, comboPeriodYear.SelectedValue);
			
			// текстовики

			PageTitle.Text = "Санитарно-карантинный контроль по выбранному показателю";
			Page.Title = PageTitle.Text;
			PageSubTitle.Text = String.Format("Санитарно-карантинный контроль по показателю \"<b>{0}</b>\" за <b>{1} {2} года</b>, направление: <b>{3}</b>, вид международного сообщения: <b>{4}</b>",
				comboMark.SelectedValue,
				comboPeriodMonth.SelectedValuesString.ToLower(), 
				comboPeriodYear.SelectedValue,
				CRHelper.ToLowerFirstSymbol(comboDirection.SelectedValue.Trim()),
				comboTransport.SelectedValue.ToLower()
			);
			txtPeriod.Text = "Отчетный период";
			labelSubjects.Text = String.Empty;

			// параметры для запроса

			selectedMark.Value = digestMark.GetMemberUniqueName(comboMark.SelectedValue);
			selectedTransport.Value = digestTransport.GetMemberUniqueName(comboTransport.SelectedValue);
			selectedPeriod.Value = selectedMonthUnique;
			selectedDirection.Value = digestDirection.GetMemberUniqueName(comboDirection.SelectedValue);
			
			// инициализация и запуск

			SetDataHelpers();
		}

		private void SetDataHelpers()
		{
			string jsScrolls = String.Empty;

			if (comboMark.SelectedIndex == 7 || comboMark.SelectedIndex == 8)
			{
				markFormat = "N3";
			}
			
			// карта РФ
			mapRFHelper = new MapRF(DundasMapRF, this);
			mapRFHelper.SetStyle();
			mapRFHelper.FillMapData();
			
			// таблица по субъектам
			gridSubjectsHelper = new GridMark(GridSubjects, comboMark.SelectedValue);
			gridSubjectsHelper.MarkFormat = markFormat;
			gridSubjectsHelper.LabelCommon = "Территория,<br />пункты пропуска";
			gridSubjectsHelper.SetStyle();
			gridSubjectsHelper.SetData("skk_004_grid_subjects");
			jsScrolls += SKKHelper.JavaScript_GetVScroll(gridSubjectsHelper.Grid);
			
			if (selectedSubject.Value != String.Empty)
			{
				GridBorders.Visible = false;
				
				// карта субъекта
				labelSubjects.Text = String.Format("Санитарно-карантинный контроль по выбранной территории - {0}", selectedSubject.Value);
				mapRFHelper.SelectShape(selectedSubject.Value);
				mapSubjectHelper = new MapSubject(DundasMapSubject, this, mapRFHelper.GetShapeColor(selectedSubject.Value));
				mapSubjectHelper.SetStyle();
				mapSubjectHelper.FillMapData();
			}
			else
			{
				DundasMapSubject.Visible = false;
				
				// таблица по участкам
				labelSubjects.Text = "Санитарно-карантинный контроль по выбранной территории - РФ";
				gridBordersHelper = new GridMark(GridBorders, comboMark.SelectedValue);
				gridBordersHelper.DeleteColumns = 1;
				gridBordersHelper.MarkFormat = markFormat;
				gridBordersHelper.LabelCommon = "Участок Гос. границы РФ,<br />пункты пропуска";
				gridBordersHelper.SetStyle();
				gridBordersHelper.SetData("skk_004_grid_borders");
				jsScrolls += SKKHelper.JavaScript_GetVScroll(gridBordersHelper.Grid);
			}

			// скроллинг таблиц
			scriptBlock.JavaScript_SetVScrollOnLoad(jsScrolls);
		
		}

		private void PdfExportButton_Click(object sender, EventArgs e)
		{
			SKKHelper.ExportPdfSetStyle(ReportPDFExporter1);
			
			const int ItemsCount = 3;
			ISection[] sections = new ISection[ItemsCount];
			Report report = new Report();

			ReportPDFExporter1.PageTitle = PageTitle.Text;
			ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;
			ReportPDFExporter1.HeaderCellHeight = gridSubjectsHelper.HeaderHeight-20;

			int currentSection = 0;

			// карта РФ
			sections[currentSection] = report.AddSection();
			mapRFHelper.Map.Width = SKKHelper.ExportPageWidth;
			ReportPDFExporter1.Export(
				mapRFHelper.Map,
				sections[currentSection++]);

			if (selectedSubject.Value != String.Empty)
			{
				// карта субъекта
				sections[currentSection] = report.AddSection();
				ReportPDFExporter1.Export(
					mapSubjectHelper.Map,
					labelSubjects.Text,
					sections[currentSection++]);
			}
			else
			{
				// таблица по участкам
				sections[currentSection] = report.AddSection();
				ReportPDFExporter1.Export(
					gridBordersHelper.Grid.GridHeaderLayout,
					sections[currentSection++]);
			}
			
			// таблица по субъектам
			sections[currentSection] = report.AddSection();
			ReportPDFExporter1.Export(
				gridSubjectsHelper.Grid.GridHeaderLayout,
				sections[currentSection++]);
		}

		private void ExcelExportButton_Click(object sender, EventArgs e)
		{
			SKKHelper.ExportExcelSetStyle(ReportExcelExporter1);
			
			const int ItemsCount = 3;
			Worksheet[] sheets = new Worksheet[ItemsCount];
			Workbook workbook = new Workbook();

			ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
			ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;
			
			int currentSheet = 0;

			// карта РФ
			sheets[currentSheet] = workbook.Worksheets.Add("sheet" + (currentSheet + 1));
			mapRFHelper.Map.Width = SKKHelper.ExportPageWidth;
			ReportExcelExporter1.Export(
				mapRFHelper.Map,
				sheets[currentSheet++], 
				3);
			
			if (selectedSubject.Value != String.Empty)
			{
				// карта субъекта
				sheets[currentSheet] = workbook.Worksheets.Add("sheet" + (currentSheet + 1));
				ReportExcelExporter1.Export(
					mapSubjectHelper.Map,
					labelSubjects.Text,
					sheets[currentSheet++], 
					3);
			}
			else
			{
				// таблица по участкам
				sheets[currentSheet] = workbook.Worksheets.Add("sheet" + (currentSheet + 1));
				ReportExcelExporter1.Export(
					gridBordersHelper.Grid.GridHeaderLayout,
					sheets[currentSheet++], 
					3);
			}

			// таблица по субъектам
			sheets[currentSheet] = workbook.Worksheets.Add("sheet" + (currentSheet + 1));
			ReportExcelExporter1.Export(
				gridSubjectsHelper.Grid.GridHeaderLayout,
				sheets[currentSheet++], 
				3);

		}

	}

}


