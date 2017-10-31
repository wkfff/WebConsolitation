using System;
using System.Collections.Generic;
using System.Data;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.MinSportSupport;



namespace Krista.FM.Server.Dashboards.reports.MinSport.MinSport_001
{
	public partial class Default : CustomReportPage
	{
        public MSQueryHelper sportData;
        public Dictionary<string, double> statData;
        public DataTable populationTable;

		private GridFull gridUpDownHelper;
		
		// параметры запроса
		private static MemberAttributesDigest digestTerritory;
		private static MemberAttributesDigest digestYear;
		private CustomParam selectedTerritory;
	    private CustomParam selectedTerritoryShort;
		private CustomParam selectedYear;


        /// <summary>
        /// Загру
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected override void Page_PreLoad(object sender, EventArgs e)
		{
			base.Page_PreLoad(sender, e);

			// Инициализация параметров запроса
			selectedTerritory = UserParams.CustomParam("selected_territory");
            selectedTerritoryShort = UserParams.CustomParam("selected_territory");
			selectedYear = UserParams.CustomParam("selected_year");

			// Настройка экспорта
			/*ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
			ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);*/
		}
		
		protected override void Page_Load(object sender, EventArgs e)
		{
			base.Page_Load(sender, e);


            if (!Page.IsPostBack)
            {
                // территория
                digestTerritory = SportHelper.DigestTerra_Init();
                comboTerritory.ComboTerra_Init(digestTerritory);

                //год 
                digestYear = SportHelper.DigestYear_Init();
                comboYear.ComboYear_Init(digestYear);
            }


			// текстовики

			PageTitle.Text = "Показатели развития физичской культуры и спорта";
			Page.Title = PageTitle.Text;
			PageSubTitle.Text = String.Format("Показатели развития физической культуры и спорта на территории \"<b>{0}</b>\" за <b>{1} год</b>",
				comboTerritory.SelectedValue,
				comboYear.SelectedValue
			);

			
			// параметры для запроса
			selectedTerritory.Value = digestTerritory.GetMemberUniqueName(comboTerritory.SelectedValue);
            selectedTerritoryShort.Value = digestTerritory.GetShortName(comboTerritory.SelectedValue);
			selectedYear.Value = digestYear.GetMemberUniqueName(comboYear.SelectedValue);

            // данные статистики
            populationTable = DataProvider.GetDataTableForChart("Sport_0001_0001_subjects_population", DataProvidersFactory.SecondaryMASDataProvider);
            DataTable dataTable = DataProvider.GetDataTableForChart("Sport_0001_0001_population", DataProvidersFactory.SecondaryMASDataProvider);
            //statData.Add("population_last", CRHelper.DBValueConvertToDoubleOrZero(dataTable.Rows[0][0]));

            // minsport data
            sportData = new MSQueryHelper();
		    sportData
                .AddMember("total", 46, 25, 2010) //всего сооружений
                .AddMember("stadiums", 47, 25, 2010) //стадионы
                .AddMember("structures", 48, 25, 2010) //плоскостные соружения
                .AddMember("chambers", 51, 25, 2010) //залы
                .AddMember("pools", 63, 25, 2010) //бассейны
                .AddMember("workers", 1, 1, 2010) //кадры
                .AddMember("countOccupy", 1, 13, 2010) //численность занимающихся всего
                .AddMember("countOccupyClub", 1, 18, 2010) //численность занимающихся в клубах
                .AddMember("countSpecialFollowers", 1, 22, 2010) //количесвто учащихся спецмедгруппа


                .AddSets(selectedTerritory.Value); //выборка нужных территорий
            
            gridUpDownHelper = new GridFull(GridBrick);
            gridUpDownHelper.SafeMode = (!Page.IsPostBack);
            gridUpDownHelper.SetStyle();
            gridUpDownHelper.SetDataQuery(sportData.ExecQuery(DataProvidersFactory.PrimaryMASDataProvider));
		}

		private void PdfExportButton_Click(object sender, EventArgs e)
		{
			/*SKKHelper.ExportPdfSetStyle(ReportPDFExporter1);
			
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
				sections[currentSection++]);*/

		}

		private void ExcelExportButton_Click(object sender, EventArgs e)
		{
			/*SKKHelper.ExportExcelSetStyle(ReportExcelExporter1);
			
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
				3);*/
			
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
			for (int i = startRow; i < startRow + rowsCount; i++)
			{
				e.CurrentWorksheet.Rows[i].Height = 22 * px;
			}
		}

	    public class MSQueryHelper
	    {
	        private const string cube = "[1-ФК]";
	        private List<string> members;
	        private List<string> select;
	        private List<string> sets;
	        private DataTable data;

	        public string Territory { get; set; }


	        public MSQueryHelper()
	        {
	            members = new List<string>();
	            select = new List<string>();
	            sets = new List<string>();
	        }

	        public MSQueryHelper AddMember(string id, int row, int column, int year)
	        {
	            select.Add(template_select.Replace("#ID#", id));
	            members.Add(
	                template_member
	                    .Replace("#ID#", id)
	                    .Replace("#ROW#", String.Format("{0}", row))
	                    .Replace("#COLUMN#", String.Format("{0}", column))
	                    .Replace("#YEAR#", String.Format("{0}", year))
	                );
	            return this;
	        }

            public MSQueryHelper AddSets(string territory)
            {
                sets.Add(template_sets.Replace("#TERRITORY#", territory));
                return this;
            }

	        public DataTable ExecQuery(DataProvider dataProvider)
	        {
	            string query = template_query
	                .Replace("#MEMBERS#", String.Join("\n", members.ToArray()))
                    .Replace("#SETS#", String.Join("\n", sets.ToArray()))
	                .Replace("#SELECT#", String.Join(",\n\t\t", select.ToArray()))
	                .Replace("#CUBE#", cube);

	            data = new DataTable();
	            dataProvider.GetDataTableForChart(query, "dummy", data);
                return data;
	        }

	        #region шаблоны для запроса

	        private const string template_query =
	            @"
                with
                    #MEMBERS#
                    #SETS#
                select
                    { 
                        #SELECT# 
                    } on columns, [Строки] on rows
                from #CUBE# ";

	        private const string template_select = @"[Measures].[#ID#]";

	        private const string template_member =
	            @"
                member [Measures].[#ID#]
                    as '
                        (
                            filter
                            (
                                [1ФК_Строки_сопоставимые].members,
                                [1ФК_Строки_сопоставимые].[Строки_сопоставимые].currentMember.properties(""Код"") = ""#ROW#""
                            ).item(0),
                            filter
                            (
                                [1ФК_Столбцы_сопоставимые].members,
                                [1ФК_Столбцы_сопоставимые].[Столбцы_сопоставимые].currentMember.properties(""Код"") = ""#COLUMN#""
                            ).item(0),
                            [Год].[Год].[Все].[#YEAR#],
                            [Measures].[Значение]
                        )'";

	        private const string template_sets =
                @"
                set [СтрокиРФ] 	
                    as 'descendants
                        (
                            {[Территории_сопоставимый].[Территории_сопоставимый].[#TERRITORY#].children },
                            [Территории_сопоставимый].[Территории_сопоставимый].[Субъект РФ]
                        )'
                    set [СтрокиФО] 	
                    as 'descendants
                        (
                            {[Территории_сопоставимый].[Территории_сопоставимый].[#TERRITORY#].children },
                            [Территории_сопоставимый].[Территории_сопоставимый].[Субъект РФ]
                        )'
                    set [СтрокиСубъект] 	
                    as 'descendants
                        (
                            {[Территории_сопоставимый].[Территории_сопоставимый].[#TERRITORY#].children },
                            [Территории_сопоставимый].[Территории_сопоставимый].[Субъект РФ]
                        )'
                set [Строки] 	
                    as '
		                IIF
		                (
			                [Территории_сопоставимый].[Территории_сопоставимый].[#TERRITORY#].level_number = 0,
			                [СтрокиРФ],
			                IIF
			                (
				                [Территории_сопоставимый].[Территории_сопоставимый].[#TERRITORY#].level_number = 1,
				                { [Территории_сопоставимый].[Территории_сопоставимый].[#TERRITORY#] } + [СтрокиФО],
				                IIF
				                (
					                [Территории_сопоставимый].[Территории_сопоставимый].[#TERRITORY#].level_number = 2,
					                { [Территории_сопоставимый].[Территории_сопоставимый].[#TERRITORY#].parent } + { [Территории_сопоставимый].[Территории_сопоставимый].[#TERRITORY#] } + [СтрокиСубъект],
					                null
				                )
			                )
		                )'";

	        #endregion
	    }

	}

}
