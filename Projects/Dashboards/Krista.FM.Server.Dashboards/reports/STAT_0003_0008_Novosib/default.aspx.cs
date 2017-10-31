using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.WebUI.Shared;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;

using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.STAT_0003_0008_Novosib
{
	public partial class Default : CustomReportPage
	{

		#region Поля

		private DataTable dtDate;
		private DataTable dtGrid;
		private GridHeaderLayout headerLayout;

        private static string[] years;
        private static string[] compareYears;
            
		#endregion

		// --------------------------------------------------------------------

        private const string PageTitleCaption = "Начисленная среднемесячная заработная плата в расчете на одного работника предприятий и организаций по видам экономической деятельности (ежегодный)";
        private const string PageSubTitleCaption = "Анализ данных ежемесячного мониторинга начисленной среднемесячной заработной платы в расчете на одного работника предприятий и организаций Новосибирской области по видам экономической деятельности за {0} год(ы)";

		// --------------------------------------------------------------------

		protected override void Page_PreLoad(object sender, EventArgs e)
		{
			base.Page_PreLoad(sender, e);

            ComboDate.Title = "Годы анализа";
			ComboDate.Width = 300;
			ComboDate.ParentSelect = true;
			ComboDate.MultiSelect = true;

            ComboCompare.Title = "Год для сравнения";
			ComboCompare.Width = 300;
			ComboCompare.ParentSelect = true;

			UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 30);
			UltraWebGrid.Height = Unit.Empty;
			UltraWebGrid.DataBinding += new EventHandler(UltraWebGrid_DataBinding);
			UltraWebGrid.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);
			UltraWebGrid.InitializeRow += new Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler(UltraWebGrid_InitializeRow);
			UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

			#region Экспорт
			
			ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
			ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
			ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click); ;
			
			#endregion
		}

		// --------------------------------------------------------------------

        protected string GetYears()
        {
            string result = String.Empty;
            foreach (string year in years)
            {
                result = AddStringWithSeparator(result, year, ", ");
            }
            return result;
        }

		protected override void Page_Load(object sender, EventArgs e)
		{
			base.Page_Load(sender, e);
			if (!Page.IsPostBack)
			{
				FillComboDate("STAT_0003_0008_Novosib_list_of_dates", ComboDate);
				SetComboSelection(ComboDate);
                FillComboDate("STAT_0003_0008_Novosib_list_of_dates", ComboCompare, "Предыдущий год");
			}
            
			#region Анализ параметров

            years = new string[ComboDate.SelectedValues.Count];
            ComboDate.SelectedValues.CopyTo(years, 0);
            compareYears = new string[years.Length];
            for (int i = 0; i < years.Length; ++i)
                if (ComboCompare.SelectedValue.Contains("Предыдущий"))
                    compareYears[i] = Convert.ToString(Convert.ToInt32(years[i]) - 1);
                else
                    compareYears[i] = ComboCompare.SelectedValue;

			#endregion
            
			PageTitle.Text = String.Format(PageTitleCaption);
			Page.Title = PageTitle.Text;
            PageSubTitle.Text = String.Format(PageSubTitleCaption, GetYears());
			
			headerLayout = new GridHeaderLayout(UltraWebGrid);
			UltraWebGrid.DataBind();
		}

		// --------------------------------------------------------------------

        #region Обработчики грида

		protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
		{
            dtGrid = new DataTable();
            DataTable dtGridDescr = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0003_0008_Novosib_grid");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Виды экономической деятельности", dtGridDescr);
            dtGrid.Columns.Add("Виды экономической деятельности", typeof(string));
            foreach (DataRow row in dtGridDescr.Rows)
            {
                for (int i = 0; i < 3; ++i)
                {
                    DataRow newRow = dtGrid.NewRow();
                    newRow["Виды экономической деятельности"] = row["Виды экономической деятельности"];
                    dtGrid.Rows.Add(newRow);
                }
            }
            for (int i = 0; i < years.Length; ++i)
            {
                string year = years[i];
                UserParams.PeriodYear.Value = year;
                DataTable dtYear = new DataTable();
                query = DataProvider.GetQueryText("STAT_0003_0008_Novosib_grid_data");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Виды экономической деятельности", dtYear);
                string compareYear = compareYears[i];
                UserParams.PeriodYear.Value = compareYear;
                DataTable dtCompareYear = new DataTable();
                query = DataProvider.GetQueryText("STAT_0003_0008_Novosib_grid_data");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Виды экономической деятельности", dtCompareYear);
                dtGrid.Columns.Add(year, typeof(double));
                dtGrid.Columns[year].DefaultValue = DBNull.Value;
                for (int j = 0; j < dtYear.Rows.Count; ++j)
                {
                    dtGrid.Rows[j * 3][year] = dtYear.Rows[j]["Среднегодовое значение"];
                    dtGrid.Rows[j * 3 + 2][year] = MathHelper.Div(dtYear.Rows[j]["Среднегодовое значение"], dtCompareYear.Rows[j]["Среднегодовое значение"]);
                    if (j > 0)
                        dtGrid.Rows[j * 3 + 1][year] = MathHelper.Div(dtYear.Rows[j]["Среднегодовое значение"], dtYear.Rows[0]["Среднегодовое значение"]);
                }
                dtGrid.AcceptChanges();
            }
			if (dtGrid.Rows.Count > 0)
			{
                DataRow row = dtGrid.NewRow();
                row["Виды экономической деятельности"] = "В том числе по видам деятельности:";
                dtGrid.Rows.InsertAt(row, 3);
				UltraWebGrid.DataSource = dtGrid;
			}
			else
			{
				UltraWebGrid.DataSource = null;
			}
		}

		protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
		{
            UltraGridBand band = e.Layout.Bands[0];
			e.Layout.AllowSortingDefault = AllowSorting.No;
			e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
			//e.Layout.NullTextDefault = "-";
			e.Layout.RowAlternateStylingDefault = DefaultableBoolean.False;
            band.Columns[0].MergeCells = true;
            band.Columns[0].CellStyle.Wrap = true;
            band.Columns[0].Width = Unit.Parse("500px");
            band.Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
			// Заголовки
			e.Layout.HeaderStyleDefault.Wrap = true;
			e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
			e.Layout.HeaderStyleDefault.VerticalAlign = VerticalAlign.Middle;
            headerLayout.AddCell("Виды экономической деятельности");
            GridHeaderCell headerCell = headerLayout.AddCell("Среднегодовая заработная плата, рублей");
			for (int i = 1; i < e.Layout.Bands[0].Columns.Count; ++i)
			{
                band.Columns[i].Width = Unit.Parse("150px");
                band.Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                band.Columns[i].CellStyle.Padding.Right = 5;
                band.Columns[i].CellStyle.Padding.Left = 5;
                headerCell.AddCell(e.Layout.Bands[0].Columns[i].Key);
			}
			headerLayout.ApplyHeaderInfo();
		}

		protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
		{
            UltraGridRow row = e.Row;
            int rowIndex = row.Index;
            // Дурная строчка шириной во весь грид
            if (rowIndex == 3)
            {
                foreach (UltraGridCell cell in row.Cells)
                {
                    cell.Style.BorderDetails.StyleLeft = cell.Style.BorderDetails.StyleRight = BorderStyle.None;
                }
                row.Cells[row.Cells.Count - 1].Style.BorderDetails.StyleRight = BorderStyle.Solid;
                row.Cells[0].Style.BorderDetails.StyleLeft = BorderStyle.Solid;
            }
            // Отступ слева для всех, кроме первого параметра
            if (rowIndex > 2)
                row.Cells[0].Style.Padding.Left = Unit.Parse("25px");
            // Шарики
            if (rowIndex > 3 && rowIndex % 3 == 2)
            {
                foreach (UltraGridCell cell in row.Cells)
                {
                    if (MathHelper.IsDouble(cell.Value))
                    {
                        double value = Convert.ToDouble(cell.Value);
                        if (value > 1)
                        {
                            cell.Style.BackgroundImage = "~/images/ballGreenBB.png";
                            cell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                            cell.Title = String.Format("Выше среднеобластного уровня на {0:P2}", value - 1);
                        }
                        else if (value < 1)
                        {
                            cell.Style.BackgroundImage = "~/images/ballRedBB.png";
                            cell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                            cell.Title = String.Format("Ниже среднеобластного уровня на {0:P2}", 1 - value);
                        }
                        else
                        {
                            cell.Style.BackgroundImage = "~/images/ballYellowBB.png";
                            cell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                            cell.Title = String.Format("Равен среднеобластному уровню");
                        }
                    }
                }
            }
            // Стрелочки
            if ((rowIndex == 2 || rowIndex % 3 == 0) && rowIndex != 0)
            {
                foreach (UltraGridCell cell in row.Cells)
                {
                    if (MathHelper.IsDouble(cell.Value))
                    {
                        double value = Convert.ToDouble(cell.Value);
                        if (value > 1)
                        {
                            cell.Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                            cell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                            cell.Title = String.Format("Темп роста к {0} году", GetCompareYear(cell.Column.Key));
                        }
                        else if (value < 1)
                        {
                            cell.Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                            cell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                            cell.Title = String.Format("Темп роста к {0} году", GetCompareYear(cell.Column.Key));
                        }
                        else
                        {
                            cell.Style.BackgroundImage = "~/images/ballYellowBB.png";
                            cell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                        }
                    }
                }
            }
            // Форматирование
            if (rowIndex == 0 || rowIndex % 3 == 1)
            {
                foreach (UltraGridCell cell in row.Cells)
                {
                    cell.Value = String.Format("{0:N2}", cell.Value);
                }
            }
            else if (rowIndex != 3)
            {
                foreach (UltraGridCell cell in row.Cells)
                {
                    cell.Value = String.Format("{0:P2}", cell.Value);
                }
            }

		}

        protected string GetCompareYear(string year)
        {
            return compareYears[Array.IndexOf(years, year)];
        }

		#endregion

		// --------------------------------------------------------------------

		#region Заполнение словарей и выпадающих списков параметров

		/// <summary>
		/// Добаляет одну строку к другой с разделителем между ними. Если первая строка пустая, то разделитель не добавляется
		/// </summary>
		/// <param name="firstString">Строка к которой добавляем</param>
		/// <param name="secondString">Строка, которую добавляем</param>
		/// <param name="separator">Разделитель</param>
		/// <returns>Возвращает получившуюся в результате добавления строку</returns>
		protected string AddStringWithSeparator(string firstString, string secondString, string separator)
		{
			if (String.IsNullOrEmpty(firstString))
			{
				return secondString;
			}
			else
			{
				return firstString + separator + secondString;
			}
		}

		protected void SetComboSelection(CustomMultiCombo combo)
		{
            for (int i = combo.GetRootNodesCount() - 1; i > combo.GetRootNodesCount() - 4 && i >= 0; --i)
            {
                combo.SetСheckedState(combo.GetRootNodesName(i), true);
            }
		}
		
		protected void FillComboDate(string queryName, CustomMultiCombo combo)
		{
			dtDate = new DataTable();
			string query = DataProvider.GetQueryText(queryName);
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Год", dtDate);
			Dictionary<string, int> dictDate = new Dictionary<string, int>();
			for (int row = 0; row < dtDate.Rows.Count; ++row)
			{
				string year = dtDate.Rows[row]["Год"].ToString();
				AddPairToDictionary(dictDate, year, 0);
			}
			combo.FillDictionaryValues(dictDate);
		}

        protected void FillComboDate(string queryName, CustomMultiCombo combo, string addedValue)
        {
            dtDate = new DataTable();
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Год", dtDate);
            Dictionary<string, int> dictDate = new Dictionary<string, int>();
            AddPairToDictionary(dictDate, addedValue, 0);
            for (int row = 0; row < dtDate.Rows.Count; ++row)
            {
                string year = dtDate.Rows[row]["Год"].ToString();
                AddPairToDictionary(dictDate, year, 0);
            }
            combo.FillDictionaryValues(dictDate);
        }

		protected void AddPairToDictionary(Dictionary<string, int> dict, string key, int value)
		{
			if (!dict.ContainsKey(key))
			{
				dict.Add(key, value);
			}
		}

		#endregion

		#region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            Report report = new Report();
            ISection section1 = report.AddSection();

            foreach (UltraGridRow row in UltraWebGrid.Rows)
                foreach (UltraGridCell cell in row.Cells)
                {
                    if (cell.Value != null)
                    {
                        cell.Value = Regex.Replace(cell.GetText(), "<[\\s\\S]*?>", String.Empty);
                    }
                }

            IText title = section1.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(PageTitle.Text);

            title = section1.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(PageSubTitle.Text);

            foreach (UltraGridRow row in headerLayout.Grid.Rows)
            {
                if (row.Index % 3 != 0)
                {
                    row.Cells[0].Style.BorderDetails.StyleTop = BorderStyle.None;
                }
                else
                {
                    row.Cells[0].Value = null;
                }
                if (row.Index % 3 != 2)
                {
                    row.Cells[0].Style.BorderDetails.StyleBottom = BorderStyle.None;
                }
                else
                {
                    row.Cells[0].Value = null;
                }
            }

            ReportPDFExporter1.HeaderCellHeight = 60;
            ReportPDFExporter1.Export(headerLayout, section1);
        }
		
		#endregion

		#region Экспорт в Excel

		private void ExcelExportButton_Click(object sender, EventArgs e)
		{
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            Worksheet sheet4 = workbook.Worksheets.Add("Пустая страница");

            SetExportGridParams(headerLayout.Grid);

            ReportExcelExporter1.HeaderCellHeight = 80;
            ReportExcelExporter1.HeaderCellFont = new Font("Verdana", 11);
            ReportExcelExporter1.TitleFont = new Font("Verdana", 12, FontStyle.Bold);
            ReportExcelExporter1.SubTitleFont = new Font("Verdana", 11);
            ReportExcelExporter1.TitleAlignment = HorizontalCellAlignment.Center;
            ReportExcelExporter1.TitleStartRow = 0;

            foreach (UltraGridRow row in UltraWebGrid.Rows)
            {
                foreach (UltraGridCell cell in row.Cells)
                {
                    if (cell.Value != null)
                    {
                        cell.Value = Regex.Replace(cell.GetText(), "<[\\s\\S]*?>", String.Empty);
                    }
                }
                if (row.IsActiveRow())
                {
                    row.Activated = row.Selected = false;
                }
            }

            int gridStartRow = 3;

            ReportExcelExporter1.Export(headerLayout, sheet1, gridStartRow);

            foreach (UltraGridRow row in UltraWebGrid.Rows)
            {
                sheet1.Rows[gridStartRow + 1 + row.Index].Height = 255;
            }

            sheet1.Rows[0].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.False;
            sheet1.Rows[0].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
            sheet1.Rows[1].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            sheet1.Rows[1].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
            sheet1.Rows[1].Height = 600;
            for (int i = 0; i < UltraWebGrid.Rows.Count; i += 3)
            {
                for (int j = 0; j < UltraWebGrid.Columns.Count; ++j)
                {
                    sheet1.Rows[gridStartRow + 1 + i].Cells[j].CellFormat.BottomBorderStyle = CellBorderLineStyle.None;
                    sheet1.Rows[gridStartRow + 2 + i].Cells[j].CellFormat.BottomBorderStyle = CellBorderLineStyle.None;
                    sheet1.Rows[gridStartRow + 2 + i].Cells[j].CellFormat.TopBorderStyle = CellBorderLineStyle.None;
                    sheet1.Rows[gridStartRow + 3 + i].Cells[j].CellFormat.TopBorderStyle = CellBorderLineStyle.None;
                }
            }

            ReportExcelExporter1.Export(new GridHeaderLayout(EmptyUltraWebGrid), sheet4, 0);

            workbook.Worksheets.Remove(sheet4);
		}

		private static void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
		{
            e.CurrentWorksheet.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;
            e.CurrentWorksheet.PrintOptions.PaperSize = PaperSize.A4;
            e.CurrentWorksheet.PrintOptions.BottomMargin = 0.25;
            e.CurrentWorksheet.PrintOptions.TopMargin = 0.25;
            e.CurrentWorksheet.PrintOptions.LeftMargin = 0.25;
            e.CurrentWorksheet.PrintOptions.RightMargin = 0.25;
            e.CurrentWorksheet.PrintOptions.ScalingType = ScalingType.FitToPages;
        }

		private static void SetExportGridParams(UltraWebGrid grid)
		{
			string exportFontName = "Verdana";
			int fontSize = 10;
			double coeff = 1.0;
			foreach (UltraGridColumn column in grid.Columns)
			{
				column.Width = Convert.ToInt32(column.Width.Value * coeff);
				column.CellStyle.Font.Name = exportFontName;
				column.Header.Style.Font.Name = exportFontName;
				column.CellStyle.Font.Size = fontSize;
				column.Header.Style.Font.Size = fontSize;
			}
		}
		
		#endregion
	}
}
