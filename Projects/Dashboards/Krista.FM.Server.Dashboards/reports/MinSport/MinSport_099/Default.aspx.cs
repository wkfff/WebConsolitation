using System;
using System.Collections.Generic;
using System.Data;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.MinSportSupport;

namespace Krista.FM.Server.Dashboards.MinSport_099.Default.reports.MinSport.MinSport_099
{
	public partial class Default : CustomReportPage
	{
	    private Factor factor;
	    private QueryCreator queryCreator;
        private GridConverter gridConverter;
	    private DataTable tempTable;

        private static MemberAttributesDigest digestTerritory;

	    public CustomParam paramYear; 

		protected override void Page_PreLoad(object sender, EventArgs e)
		{
			base.Page_PreLoad(sender, e);
            factor = new Factor();

            ReportExcelExporter1.ExcelExportButton.Click += ExcelExportButton_Click;
            ReportExcelExporter1.ExcelExportButton.Text = "Сформировать файл данных ЕМИСС";
            ReportExcelExporter1.ExcelExporter.NameValidating += ultraWebGridExcelExporter_NameValidating;
		
            paramYear = UserParams.CustomParam("param_year");
		}

        private static void ultraWebGridExcelExporter_NameValidating(object sender, NameValidatingEventArgs e)
        {
            e.ApprovedName = e.ProposedName;
        }
		
		protected override void Page_Load(object sender, EventArgs e)
		{
            base.Page_Load(sender, e);
		    if (!Page.IsPostBack)
		    {
                PageTitle.Text = "Конвертер данных статистических форм в ЕМИСС";
                LabelEnterCode.Text = @"Введите уникальный код посылки";
                FactorList.Items.Add("Выберите показатель...");
                FactorList.AppendDataBoundItems = true;
                FactorList.DataSource = XmlWorker.GetFactorsDictionary();
		        FactorList.DataTextField = "key";
                FactorList.DataValueField = "value";
                FactorList.DataBind();

                //территория
                digestTerritory = SportHelper.DigestTerra_Init();
                MultiTerritory.ComboTerritoryForConverter_Init(digestTerritory);

                if (!XmlWorker.IsDebugMode())
                {
                    VisibleTotal.Visible = false;
                    VisibleDefinition.Visible = false;
                }
		    }
            
            Page.Title = "Конвертер в ЕМИСС";

            if (MultiTerritory.SelectedValues.Count > 0 )
            {
                foreach (var value in MultiTerritory.SelectedValues)
                {
                    factor.AddValueInTerritorySet(digestTerritory.GetMemberUniqueName(value));      
                }
            }
		}

        /// <summary>
        /// Чтение даных по показателю 
        /// </summary>
        private bool ReadData()
        {
            if (FactorList.SelectedIndex == 0)
            {
                report.Controls.Add(SportHelper.StyleError(SportHelper.ToHTML(SportHelper.Text("Выберите код показателя!"))));
                return false;  
            }
            if (String.IsNullOrEmpty(EnterCodeBox.Text))
            {
                report.Controls.Add(SportHelper.StyleError(SportHelper.ToHTML(SportHelper.Text("Введите уникальный код посылки!"))));
                return false; 
            }
            XmlWorker.Code = FactorList.SelectedValue;
            XmlWorker.GetFactor(factor);
            UseHandBooksManager.SetupHandBooksManager();

            ReportExcelExporter1.DownloadFileName = SportHelper.FormattionFileName(EnterCodeBox.Text, factor.Code);

            factor.WorkYear = YearList.SelectedItem.ToString();
            queryCreator = new QueryCreator(factor);

            tempTable = ProcessingQueryResult.CreateTempDataTable();

            foreach (var handBook in XmlWorker.GetFactorUseHandBooks())
            {
                var useHandBookTable = SportHelper.ExecQuery(DataProvidersFactory.PrimaryMASDataProvider, queryCreator.GenerateQueryForUseHandBook(handBook));
                //ProcessingQueryResult.DeleteEmptyValues(useHandBookTable);
                ProcessingQueryResult.UpdateTerritoryValue(useHandBookTable);
                ProcessingQueryResult.ProcessingUseHandBookData(useHandBookTable, tempTable, handBook, factor);
            }

            foreach (var crossHandBook in XmlWorker.GetFactorCrossHandBooks())
            {
                var crossHandBookTable = SportHelper.ExecQuery(DataProvidersFactory.PrimaryMASDataProvider,
                                                   queryCreator.GenerateQueryForCrossHandBook(crossHandBook));
                //ProcessingQueryResult.DeleteEmptyValues(crossHandBookTable);
                ProcessingQueryResult.UpdateTerritoryValue(crossHandBookTable);
                ProcessingQueryResult.ProcessingCrossHandBookData(crossHandBookTable, tempTable, crossHandBook, factor);
            }

            var dataTable = SportHelper.ExecQuery(DataProvidersFactory.PrimaryMASDataProvider, queryCreator.GenerateQueryForTotalElements());
           // ProcessingQueryResult.DeleteEmptyValues(dataTable);
            ProcessingQueryResult.UpdateTerritoryValue(dataTable);
            ProcessingQueryResult.ProcessingTotalElements(dataTable, tempTable, factor);
            
            if (VisibleTotal.Checked)
            {
                ProcessingQueryResult.FillTotalElements(tempTable);
            }

            switch (factor.Code)
            {
                case "2.1.61.11": ProcessingQueryResult.ProvisionSportConstruction(tempTable, ReadTerritoryPopulation());
                    break;
                case "47.1.10":
                    ProcessingQueryResult.PercentSportPopulation(tempTable, ReadTerritoryPopulation());
                    break;
                case "47.2.08":
                    tempTable = ProcessingQueryResult.QuantitySportsBuilding(tempTable);
                    break;
            }

            gridConverter = new GridConverter(GridBrick, XmlWorker.GetFactorUseHandBooks().Count, VisibleDefinition.Checked);
            gridConverter.SetStyle();
            gridConverter.SetDataFromDataTable(tempTable);

            CreateReportHead(); 

            return true;
        }

        /// <summary>
        /// Создание шапки отчета 
        /// </summary>
        private void CreateReportHead()
        {
            report.Controls.Add(SportHelper.StyleMainTitle(SportHelper.ToHTML(SportHelper.Text(String.Format("{0} <br/> {1} год"
                , factor.Name, YearList.SelectedValue)))));
            var descriptorFactor = new List<string>
                                       {
                                           String.Format("Код посылки уникальный: {0}", EnterCodeBox.Text),
                                           String.Format("Дата/время формирования: {0}", DateTime.Now.Date),
                                           String.Format("Контрольная сумма: {0}", gridConverter.Grid.Grid.Rows.Count),
                                           String.Format("Ответственный: {0}", XmlWorker.GetResponsibleName()),
                                           String.Format("Телефон: {0}", XmlWorker.GetResponsibleMobile()),
                                           String.Format("Код показателя: {0}", factor.Code),
                                           String.Format("Наименование показателя: {0}", factor.Name),
                                           String.Format("Ведомство: {0}", XmlWorker.GetDepartmentCode())
                                       };
            foreach (var element in descriptorFactor)
            {
                report.Controls.Add(SportHelper.StyleSimple(SportHelper.ToHTML(SportHelper.Text(element))));
            }
            report.Controls.Add(SportHelper.StyleSimple(SportHelper.ToHTML(SportHelper.Text("<br/><br/>"))));
        }

        protected void BtnGetData_Click(object sender, EventArgs e)
        {
            ReadData();
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            if (!ReadData()) { return; }
            const int itemsCount = 1;
            var sheets = new Worksheet[itemsCount];
            var workbook = new Workbook();

            const int currentSheet = 0;

            // таблица
            sheets[currentSheet] = workbook.Worksheets.Add("sheet" + (currentSheet + 1));
            ReportExcelExporter1.RowsAutoFitEnable = false;
            ReportExcelExporter1.ExcelExporter.EndExport += ExcelExporter_EndExport;
            ReportExcelExporter1.Export(
                gridConverter.Grid.GridHeaderLayout,
                sheets[currentSheet],
                15);
            sheets[currentSheet].Rows[9].Cells[1].CellFormat.WrapText = ExcelDefaultableBoolean.False;
            sheets[currentSheet].Rows[9].Cells[1].Value = "TTT";
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            e.CurrentWorksheet.Name = "Данные";
            e.CurrentWorksheet.Rows[0].SetCellValue(0, "Код посылки(уникальный):");
            e.CurrentWorksheet.Rows[0].SetCellValue(1, EnterCodeBox.Text);
            e.CurrentWorksheet.Rows[1].SetCellValue(0, "Дата/время формирования");
            e.CurrentWorksheet.Rows[1].SetCellValue(1, DateTime.Now.Date);
            e.CurrentWorksheet.Rows[2].SetCellValue(0, "Контрольная сумма");
            e.CurrentWorksheet.Rows[2].SetCellValue(1, gridConverter.Grid.Grid.Rows.Count);
            e.CurrentWorksheet.Rows[3].SetCellValue(0, "Количество листов данных");
            e.CurrentWorksheet.Rows[4].SetCellValue(0, "Количество элементов справочников");
            e.CurrentWorksheet.Rows[5].SetCellValue(0, "Количество листов справочников");
            e.CurrentWorksheet.Rows[6].SetCellValue(0, "Ответственный");
            e.CurrentWorksheet.Rows[6].SetCellValue(1, XmlWorker.GetResponsibleName());
            e.CurrentWorksheet.Rows[6].SetCellValue(2, "Телефон");
            e.CurrentWorksheet.Rows[6].SetCellValue(3, XmlWorker.GetResponsibleMobile());
            e.CurrentWorksheet.Rows[8].SetCellValue(0, "Код показателя");
            e.CurrentWorksheet.Rows[8].SetCellValue(1, factor.Code);
            e.CurrentWorksheet.Rows[9].SetCellValue(0, "Наименование показателя");
            e.CurrentWorksheet.Rows[9].SetCellValue(1, factor.Name);
            e.CurrentWorksheet.Rows[10].SetCellValue(0, "Источник");
            e.CurrentWorksheet.Rows[11].SetCellValue(0, "Ведомство");
            e.CurrentWorksheet.Rows[11].SetCellValue(1, XmlWorker.GetDepartmentCode());
            e.CurrentWorksheet.Rows[12].SetCellValue(0, "Временной ряд");         
        }

        private DataTable ReadTerritoryPopulation()
        {
            paramYear.Value = YearList.SelectedValue;
            var msTable = DataProvider.GetDataTableForChart("minsport_territory_okato", DataProvidersFactory.PrimaryMASDataProvider);
            var fmTable = DataProvider.GetDataTableForChart("minsport_territory_population",
                                                                  DataProvidersFactory.SecondaryMASDataProvider);
            msTable.Columns.Add("Population");
            for (var i = 0; i<msTable.Rows.Count; i++)
            {
                msTable.Rows[i]["Population"] = fmTable.Rows[i]["Факт"];
            }
            ProcessingQueryResult.UpdateTerritoryValue(msTable);

            return msTable;
        }

        protected void FactorList_SelectedIndexChanged(object sender, EventArgs e)
        {
            YearList.Items.Clear();
            if (FactorList.SelectedIndex == 0)
            {
                return;
            }
            XmlWorker.Code = FactorList.SelectedValue;
            XmlWorker.GetFactor(factor);
            for (var year=Convert.ToInt32(factor.DepthTimeSet); year < DateTime.Now.Date.Year; year++)
            {
                YearList.Items.Add(year.ToString());
            }
        }
        
	}

}
