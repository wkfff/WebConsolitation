using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Common.GridIndicatorRules;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.FO_0002_0013_01_Settlement.Default.reports.FO_0002_0013_01_Settlement;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0013_01_Settlement
{
	/// <summary>
	/// Исполнение бюджетов поселений в разрезе отдельных показателей
	/// Астрахань
	/// ФО_МесОтч_Доходы, 
	/// ФО_МесОтч_Расходы, 
	/// ФО_МесОтч_Дефицит Профицит
	/// </summary>
	public partial class Default : CustomReportPage
	{
		private const int firstYear = 2010;
		private int lastYear = firstYear;
		private string lastMonth = "Январь";
		
		// Параметры запроса
		private CustomParam paramPeriod;
		private CustomParam paramArea;
		private CustomParam paramScale;
		private CustomParam paramMeasure;
		private static MemberAttributesDigest digestAreas;
		
		
		protected override void Page_PreLoad(object sender, EventArgs e)
		{
			base.Page_PreLoad(sender, e);

			// Инициализация параметров запроса
			paramPeriod   = UserParams.CustomParam("selected_period");
			paramArea     = UserParams.CustomParam("selected_area");
			paramScale    = UserParams.CustomParam("selected_scale");
			paramMeasure  = UserParams.CustomParam("selected_measure");

			// Настройка экспорта
			ReportExcelExporter1.ExcelExportButton.Click += ExcelExportButton_Click;
			ReportPDFExporter1.PdfExportButton.Click += PdfExportButton_Click;
			
		}

		protected override void Page_Load(object sender, EventArgs e)
		{
			base.Page_Load(sender, e);
			
			if (!Page.IsPostBack)
			{
				// javascript-события
				ArgScale.Attributes.Add("onclick", "document.getElementById('RefreshButton').className='Button';");

				// последний год, на который есть данные
				DataTable table = DataProvider.GetDataTableForChart("FO_0002_0013_01_Settlement_last_period", DataProvidersFactory.PrimaryMASDataProvider);
				if (table.Rows.Count > 0 && table.Rows[0][0] != DBNull.Value)
				{
				    lastYear = firstYear;
				    // [Период__Период].[Период__Период].[Данные всех периодов].[2011].[Полугодие 2].[Квартал 3].[Сентябрь]
				    MatchCollection matches = Regex.Matches(table.Rows[0][0].ToString(), @"\[([^\]]*)\]");
				    if (matches.Count > 3)
				    {
				        lastYear = Convert.ToInt32(matches[3].Groups[1].Value);
				    }
				    if (matches.Count > 6)
				    {
				        lastMonth = matches[6].Groups[1].Value;
				    }
				}


				// параметр - год
				ArgYear.Title = "Год";
				ArgYear.Width = 100;
				ArgYear.MultiSelect = false;
				ArgYear.ParentSelect = false;
				ArgYear.ShowSelectedValue = true;
				ArgYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, lastYear));
				ArgYear.SelectLastNode();

				// параметр - месяц
				ArgMonth.Title = "Месяц";
				ArgMonth.Width = 150;
				ArgMonth.MultiSelect = false;
				ArgMonth.ParentSelect = false;
				ArgMonth.ShowSelectedValue = true;
				ArgMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
				ArgMonth.SetСheckedState(lastMonth, true);

				// параметр - раздел отчета
				ArgArea.Title = "Муниципальный район";
				ArgArea.Width = 300;
				ArgArea.MultiSelect = false;
				ArgArea.ParentSelect = false;
				ArgArea.ShowSelectedValue = true;
				digestAreas = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0002_0013_01_Settlement_filter_area");
				ArgArea.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(digestAreas.UniqueNames, digestAreas.MemberLevels));
			}

			// параметры для запроса
			paramPeriod.Value = String.Format(
				"[Период__Период].[Период__Период].[Данные всех периодов].[{0}].[Полугодие {1}].[Квартал {2}].[{3}]", 
				ArgYear.SelectedValue,
				CRHelper.HalfYearNumByMonthNum(ArgMonth.SelectedIndex + 1),
				CRHelper.QuarterNumByMonthNum(ArgMonth.SelectedIndex + 1),
				ArgMonth.SelectedValue);
			paramArea.Value = digestAreas.GetMemberUniqueName(ArgArea.SelectedValue);
			paramScale.Value = ArgScale.SelectedIndex == 0 ? "1000" : "1000000";
			
			// текстовики
			Page.Title = "Исполнение бюджетов поселений в разрезе отдельных показателей";
			PageTitle.Text = Page.Title;
			PageSubTitle.Text = String.Format(
				"{0} район, данные за {1} {2} {3} года, {4}",
				ArgArea.SelectedValue,
				ArgMonth.SelectedIndex + 1,
				CRHelper.RusManyMonthGenitive(ArgMonth.SelectedIndex + 1),
				ArgYear.SelectedValue,
				ArgScale.SelectedValue
				);

			// погнали!
			GridHelper grid = new GridHelper(this);
			grid.Init(GridBrick, "FO_0002_0013_01_Settlement_grid", DataProvidersFactory.PrimaryMASDataProvider);
			
		}
		
		#region Экспорт
		
		private void PdfExportButton_Click(object sender, EventArgs e)
		{
			Report report = new Report();
			ISection section1 = report.AddSection();

			ReportPDFExporter1.HeaderCellHeight = 20;

			ReportPDFExporter1.PageTitle = PageTitle.Text;
			ReportPDFExporter1.Export(GridBrick.GridHeaderLayout, section1);
		}

		private void ExcelExportButton_Click(object sender, EventArgs e)
		{
			Workbook workbook = new Workbook();
			Worksheet sheet1 = workbook.Worksheets.Add("sheet1");

			ReportExcelExporter1.SheetColumnCount = 15;
			ReportExcelExporter1.HeaderCellHeight = 20;
			ReportExcelExporter1.GridColumnWidthScale = 1.3;
			ReportExcelExporter1.RowsAutoFitEnable = true;

			ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
			ReportExcelExporter1.Export(GridBrick.GridHeaderLayout, sheet1, 3);
		}

		#endregion

		public class GridHelper : GridHelperBase
		{
			private Default Page;
			private Collection<double> avgList; 

			public GridHelper(Default page)
			{
				Page = page;
				HiddenColumns = 1;
				avgList = new Collection<double>();
			}

			protected override void SetStyle()
			{
				base.SetStyle();
				Grid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 10);
				Grid.AutoSizeStyle = GridAutoSizeStyle.AutoHeight;
				Grid.AllowColumnSorting = true;
			}

			protected override void SetData(string queryName, DataProvider provider)
			{
				base.SetData(queryName, provider);
				
				if (Data.Rows.Count == 0) 
					return;
				
				Data.Columns.RemoveAt(Data.Columns.Count - HiddenColumns-1);

				for (int i = 3; i < Data.Rows[0].ItemArray.Length - HiddenColumns; i += 3)
				{
					if (Data.Rows[0][i] != DBNull.Value && !Data.Rows[0][i].ToString().Equals(String.Empty))
					{
						avgList.Add(Math.Round(Double.Parse(Data.Rows[0][i].ToString()), 4));
					}
				}

				// пустые строки
				Collection<DataRow> toDelete = new Collection<DataRow>();
				foreach (DataRow row in Data.Rows)
				{
					bool empty = true;
					for (int i = 1; i < row.ItemArray.Length - HiddenColumns; i++)
					{
						if (row[i] != DBNull.Value && !row[i].ToString().Equals(String.Empty))
						{
							empty = false;
							break;
						}
					}
					if (empty)
					{
						toDelete.Add(row);
					}
				}
				// и 1 строку
				if (!toDelete.Contains(Data.Rows[0]))
				{
					toDelete.Add(Data.Rows[0]);
				}
				// удаляем
				foreach (DataRow row in toDelete)
				{
					Data.Rows.Remove(row);
				}
				
				if (Data.Rows.Count == 0)
				{
					Grid.DataTable.Dispose();
				}
					
			}

			protected override void SetDataStyle()
			{
				Band.Columns[0].CellStyle.Wrap = true;
				Band.Columns[0].Width = CRHelper.GetColumnWidth(150);
				Band.Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

				Band.HideColumns(HiddenColumns);

				for (int i = 1; i < Band.Columns.Count - HiddenColumns; i++)
				{
					Band.Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;

					string caption = Band.Columns[i].Header.Caption;
					
					int width;
					if (caption.Contains("%"))
						width = 80;
					else
						width = 90;
					Band.Columns[i].Width = CRHelper.GetColumnWidth(width);

					string format;
					if (caption.Contains("%"))
						format = "P2";
					else 
						format = "N3";
					CRHelper.FormatNumberColumn(Band.Columns[i], format);

				}

			}
			
			protected override void SetDataRules()
			{
				// правило для "Итого"
				FontRowLevelRule levelRule = new FontRowLevelRule(Band.Columns.Count - 1);
				levelRule.AddFontLevel("0", Grid.BoldFont8pt);
				Grid.AddIndicatorRule(levelRule);
			}

			protected override void InitializeRow(object sender, RowEventArgs e)
			{
				UltraGridRow row = e.Row;
				
				for (int i = 0; i < avgList.Count; i++)
				{
					UltraGridCell cell = row.Cells[i*3+3];
					if (cell.Value != null && !cell.Value.ToString().Equals(String.Empty))
					{
						double percent = Math.Round(Double.Parse(cell.Value.ToString()), 4);
						if (percent < avgList[i])
						{
							cell.Style.BackgroundImage = "~/images/ballRedBB.png";
							cell.Title = "Процент исполнения ниже среднего по поселениям";
						}
						else if (percent > avgList[i])
						{
							cell.Style.BackgroundImage = "~/images/ballGreenBB.png";
							cell.Title = "Процент исполнения выше среднего по поселениям";
						}
						cell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
					}
				
				}
			}

			protected override void SetDataHeader()
			{
				GridHeaderLayout headerLayout = Grid.GridHeaderLayout;
				
				headerLayout.AddCell("Наименование");

				Collection<string> headerTitles = 
					new Collection<string>
						{
							"ДОХОДЫ ВСЕГО",
							"Налоговые и неналоговые доходы",
							"Безвозмездные поступления",
							"РАСХОДЫ ВСЕГО",
							"Оплата труда с начислениями",
							"Капитальные вложения",
							"Иные расходы",
							"ДЕФИЦИТ/ПРОФИЦИТ",
						};

				foreach (string title in headerTitles)
				{
					GridHeaderCell cell = headerLayout.AddCell(title);
					cell.AddCell("Назначено, " + Page.ArgScale.SelectedValue);
					cell.AddCell("Исполнено, " + Page.ArgScale.SelectedValue);
					cell.AddCell("Исполнено, %");
				}
				
				Grid.GridHeaderLayout.ApplyHeaderInfo();
			}


		}
		
	}

	
}
