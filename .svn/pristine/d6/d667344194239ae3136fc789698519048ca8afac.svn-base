using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Infragistics.Documents.Word;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Color = System.Drawing.Color;
using Image = System.Drawing.Image;
using TableLayout = Infragistics.Documents.Word.TableLayout;
using WFont = Infragistics.Documents.Word.Font;
using WTable = Infragistics.Documents.Word.Table;
using WTableRow = Infragistics.Documents.Word.TableRow;
using WTableCell = Infragistics.Documents.Word.TableCell;
using WTableColumn = Infragistics.Documents.Word.TableColumn;
using WDocument = Infragistics.Documents.Word.Document;

namespace Krista.FM.Server.Dashboards.SkkSupport
{
	public class SKKExportDoc : SKKExportBase
	{
		private readonly WDocument document;
		private readonly Control rootControl;
		private HashSet<ServiceFlags> servicesFlags;
		private Dictionary<string, string> currentStyle;
		private Collection<string> stylesAllowed;
		private Collection<string> stylesInherits;
		private Collection<string> recipes;
		private int chartGroupsIndex;
		private double chartGroupsScale;
		private WTable chartGroupsTable;

		public SKKExportDoc(Page page, Control rootControl)
			: base(page, ExportType.Doc)
		{
			this.rootControl = rootControl;

			document = new WDocument();
			
			servicesFlags = new HashSet<ServiceFlags>();
			currentStyle = new Dictionary<string, string>();
			stylesAllowed =
				new Collection<string>
					{
						"text-align",
						"font-weight",
						"font-size",
						"margin",
						"margin-left",
						"margin-right",
						"float",
						"clear",
						"display"
					};
			stylesInherits =
				new Collection<string>
					{
						"text-align",
						"font-weight",
						"font-size",
					};
			recipes = new Collection<string>();
			
			Chart.ScaleFactor = 1;
			Grid.ScaleFactor = 1.25;
			//Grid.ScaleFactor = 0.80;
			//Grid.ScaleFontFactor = 1;
			
			chartGroupsIndex = 0;
			chartGroupsScale = 0.85;
		}
		
		public override void Export()
		{
			HeaderText = HeaderText.Replace("&nbsp;", " ");

			Init();

			AddHeader();
			AddFooter();

			ExportContent();
			
			Publish();
		}

		private void Init()
		{
			document.DocumentProperties.Subject = Header1stTextLeft;
			document.DocumentProperties.Title = HeaderText;

			document.Unit = UnitOfMeasurement.Centimeter;
			document.FinalSection.Properties.PageSize = new SizeF(21f, 29.7f);
			document.FinalSection.Properties.PageOrientation = PageOrientation.Portrait;
			document.FinalSection.Properties.PageMargins = new Padding(2.5f, 1.5f, 1f, 1.5f);
			document.FinalSection.Properties.HeaderMargin = 1f;
			document.FinalSection.Properties.FooterMargin = 1f;

			document.FinalSection.Properties.StartingPageNumber = 1;

			document.NewLineType = NewLineType.CarriageReturn;

			document.Unit = UnitOfMeasurement.Point;
			document.DefaultFont.Name = "Arial";
			document.DefaultFont.NameComplexScript = "Arial";
			document.DefaultFont.NameEastAsia = "Arial";
			document.DefaultFont.NameHighAnsi = "Arial";
			document.DefaultFont.Size = 12f;
			document.DefaultFont.ForeColor = Color.Black;

			document.DefaultTableProperties.Alignment = ParagraphAlignment.Center;
			document.DefaultTableProperties.CellProperties.Margins = new Padding(4, 2, 4, 2);
		}
		
		protected void AddHeader()
		{
			if (HeaderHeight == 0)
				return;

			WTable table;
			WTableRow row;
			WTableCell cell;
			Paragraph para;

			// first page
			table = CreateHeaderTable(document.FinalSection.HeaderFirstPageOnly);
			
			row = table.Rows.Add();
			cell = row.Cells.Add();
			cell.Properties.PreferredWidthAsPercentage = 60;
			para = cell.ContentBlocks.AddParagraph();
			para.Properties.Alignment = ParagraphAlignment.Left;
			TextRun wText = para.ContentRuns.AddTextRun(Header1stTextLeft);
			wText.Font.Italic = true;
			wText.Font.Size = 11;

			cell = row.Cells.Add();
			cell.Properties.PreferredWidthAsPercentage = 40;
			para = cell.ContentBlocks.AddParagraph();
			para.Properties.Alignment = ParagraphAlignment.Right;
			wText = para.ContentRuns.AddTextRun(Header1stTextRight);
			wText.Font.Italic = true;
			wText.Font.Size = 11;

			// all pages
			table = CreateHeaderTable(document.FinalSection.HeaderAllPages);
			cell = table.Rows.Add().Cells.Add();
			cell.Properties.PreferredWidthAsPercentage = 100;
			para = cell.ContentBlocks.AddParagraph();
			para.Properties.Alignment = ParagraphAlignment.Center;
			wText = para.ContentRuns.AddTextRun(HeaderText);
			wText.Font.Italic = true;
			wText.Font.Size = 11;
		}

		private WTable CreateHeaderTable(SectionHeaderFooter header)
		{
			WTable table = header.ContentBlocks.AddTable();

			table.Properties.Alignment = ParagraphAlignment.Center;
			table.Properties.BorderProperties.Width = 1;
			table.Properties.BorderProperties.Style = TableBorderStyle.Single;
			table.Properties.BorderProperties.Sides = TableBorderSides.Bottom;
			table.Properties.Layout = TableLayout.Fixed;
			table.Properties.PreferredWidthAsPercentage = 100;
			table.Properties.CellMargins = new Padding(0, 0, 0, 3);

			table.Columns.Add();

			Paragraph para = header.ContentBlocks.AddParagraph();
			para.ContentRuns.AddTextRun(((char)160).ToString()).Font.Size = 4;

			return table;
		}

		protected void AddFooter()
		{
			if (FooterHeight == 0)
				return;
			
			CreateFooterTable(document.FinalSection.FooterFirstPageOnly);
			CreateFooterTable(document.FinalSection.FooterAllPages);
		}

		private void CreateFooterTable(SectionHeaderFooter footer)
		{
			WTable table = footer.ContentBlocks.AddTable();
			table.Properties.Alignment = ParagraphAlignment.Center;
			table.Properties.BorderProperties.Width = 1;
			table.Properties.BorderProperties.Style = TableBorderStyle.Single;
			table.Properties.BorderProperties.Sides = TableBorderSides.Top;
			table.Properties.Layout = TableLayout.Fixed;
			table.Properties.PreferredWidthAsPercentage = 100;
			table.Properties.CellMargins = new Padding(0, 3, 0, 0);

			table.Columns.Add();

			WTableRow row = table.Rows.Add();
			WTableCell cell;
			Paragraph para;

			cell = row.Cells.Add();
			cell.Properties.PreferredWidthAsPercentage = 75;
			para = cell.ContentBlocks.AddParagraph();
			para.Properties.Alignment = ParagraphAlignment.Left;
			TextRun wText = para.ContentRuns.AddTextRun(FooterText);
			wText.Font.Italic = true;
			wText.Font.Size = 11;

			cell = row.Cells.Add();
			cell.Properties.PreferredWidthAsPercentage = 25;
			para = cell.ContentBlocks.AddParagraph();
			para.Properties.Alignment = ParagraphAlignment.Right;
			
			wText = para.ContentRuns.AddTextRun(String.Format("Страница{0}", (char)160));
			wText.Font.Italic = true;
			wText.Font.Size = 11;

			PageNumberField wNumberField = para.ContentRuns.AddPageNumberField(PageNumberFieldFormat.Decimal);
			wNumberField.Font.Italic = true;
			wNumberField.Font.Size = 11;
		}

		protected override void Publish()
		{
			document.Save(stream);
			base.Publish();
		}

		protected override void ExportContent()
		{
			ExportControls(new Collection<Control> { rootControl });
		}

		/// <summary>
		/// Экспорт контролов
		/// </summary>
		public void ExportControls(IEnumerable controls)
		{
			foreach (Control control in controls)
			{
				if (control is HtmlControl)
				{
					ExportHtmlControl(control as HtmlControl);
				}
				else if (control is LiteralControl)
				{
					ExportTextControl(control as LiteralControl);
				}
				else if (control is UltraChart)
				{
					ExportChartControl(control as UltraChart);
				}
				else if (control is UltraGridBrick)
				{
					ExportGridControl(control as UltraGridBrick);
				}
			}
		}

		/// <summary>
		/// Экспорт элемента разметки
		/// </summary>
		protected void ExportHtmlControl(HtmlControl control)
		{
			// сохранить состояние стиля
			Dictionary<string, string> parentStyle = currentStyle;

			// новый стиль
			CreateCurrentStyle(parentStyle, control.Style);

			ApplyHtmlServiceStyles(control);

			if (!StyleContains("display", "none"))
			{
				if (ApplyHtmlOuterStyles())
				{
					ExportControls(control.Controls);
				}
			}

			currentStyle = parentStyle;
		}

		/// <summary>
		/// Экспорт текстового элемента
		/// </summary>
		protected void ExportTextControl(LiteralControl control)
		{
			// парсинг стилей
			string value;

			ParagraphAlignment alignment = ParagraphAlignment.Left;
			if (currentStyle.TryGetValue("text-align", out value))
			{
				alignment = ParseCssAlignment(value);
			}

			bool styleBold = false;
			if (currentStyle.TryGetValue("font-weight", out value))
			{
				switch (value)
				{
					case "bold":
						styleBold = true;
						break;
				}
			}

			float styleSize = 0;
			if (currentStyle.TryGetValue("font-size", out value))
			{
				float floatValue;
				if (ParseCssValue(value, out floatValue))
				{
					styleSize = floatValue;
				}
			}
			
			// постобработка текста
			string raw_text = control.Text
				.Replace("&nbsp;", ((char)160).ToString());
			
			// составляем текст из параграфов
			string[] textLines = raw_text.Split(new[] { SKKHelper.AddParagraph() }, StringSplitOptions.None);
			for (int index = 0; index < textLines.Length; index++)
			{
				string textLine = textLines[index];

				if (String.IsNullOrEmpty(textLine))
					continue;

				Paragraph para = document.ContentBlocks.AddParagraph();
				
				// настройка свойств
				
				para.Properties.Alignment = alignment;
				
				RectangleF margins = ParseCssMargins();
				para.Properties.SpacingBefore = margins.Y;
				para.Properties.SpacingAfter = margins.Height;
				currentStyle.RemoveAnyway("margin");
				
				if (recipes.Contains(ServiceRecipe.NewPage.ToString()))
				{
					para.Properties.PageBreakBefore = true;
					recipes.Remove(ServiceRecipe.NewPage.ToString());
				}

				// красную строку для первого (пустого) параграфа не делаем
				if (index > 0)
					para.ContentRuns.AddTextRun(SKKHelper.AddParagraphSpaces());
				
				string[] lines = textLine.Split(new[] { "<br />" }, StringSplitOptions.RemoveEmptyEntries);
				foreach (string line in lines)
				{
					TextRun wText = para.ContentRuns.AddTextRun(Regex.Replace(line, "<[^>]*?>", String.Empty));
					
					wText.Font.Bold = styleBold;
					if (styleSize > 0)
						wText.Font.Size = styleSize;

					para.ContentRuns.AddNewLine(NewLineType.LineBreak);
				}

				// последний разрыв строки лишний
				if (para.ContentRuns.Count > 0)
					para.ContentRuns.RemoveAt(para.ContentRuns.Count - 1);
			}
			
		}

		/// <summary>
		/// Экспорт диаграммы
		/// </summary>
		protected override void ExportChartControl(UltraChart control)
		{
			Paragraph para;

			if (chartGroupsIndex > 0)
			{
				control.Width = new Unit(control.Width.Value * chartGroupsScale);
				control.Axis.X.Margin.Far.Value = control.Axis.X.Margin.Far.Value / chartGroupsScale;
				
				if (chartGroupsIndex == 1)
				{
					chartGroupsTable = document.ContentBlocks.AddTable();
					chartGroupsTable.Properties.BorderProperties.Style = TableBorderStyle.None;
					chartGroupsTable.Rows.Add();
					chartGroupsTable.Columns.Add();
				}

				WTableCell cell = chartGroupsTable.Rows[0].Cells.Add();
				para = cell.ContentBlocks.AddParagraph();
				chartGroupsIndex++;
			}
			else
			{
				para = document.ContentBlocks.AddParagraph();
			}

			para.Properties.Alignment = ParagraphAlignment.Center;
			para.ContentRuns.AddInlinePicture(Chart.Export(control));
		}

		/// <summary>
		/// Экспорт таблицы
		/// </summary>
		protected override void ExportGridControl(UltraGridBrick control)
		{
			Grid.Export(document, control.GridHeaderLayout);
		}

		/// <summary>
		/// Применить управляющие стили разметки
		/// </summary>
		protected void ApplyHtmlServiceStyles(HtmlControl control)
		{
			if (!StyleContains("display", "none"))
				return;
			recipes = new Collection<string>(new List<string>(((LiteralControl)(control.Controls[0])).Text.Split(',')));

			// ChartGroups
			if (recipes.Contains(ServiceRecipe.ChartGroupsBegin.ToString()))
			{
				servicesFlags.Add(ServiceFlags.ChartGroups);
				chartGroupsIndex = 1;
			}
			if (recipes.Contains(ServiceRecipe.ChartGroupsEnd.ToString()))
			{
				servicesFlags.Remove(ServiceFlags.ChartGroups);
				chartGroupsIndex = 0;
			}
		}

		/// <summary>
		/// Применить приоритетные стили разметки (управляющие контейнером)
		/// </summary>
		protected bool ApplyHtmlOuterStyles()
		{
			bool skip = false;

			string value;
			if (currentStyle.TryGetValue("clear", out value))
			{
				skip = true;
			}

			return !skip;
		}
		
		/// <summary>
		/// Создает новый текущий стиль
		/// </summary>
		private void CreateCurrentStyle(Dictionary<string, string> parentStyle, CssStyleCollection newStyle)
		{
			currentStyle = new Dictionary<string, string>();

			// наследуемые элементы родительского стиля
			foreach (KeyValuePair<string, string> styleItem in parentStyle)
			{
				if (stylesInherits.Contains(styleItem.Key))
				{
					currentStyle.Add(styleItem.Key, styleItem.Value);
				}
			}

			// новое состояние стиля
			foreach (string keyStyle in newStyle.Keys)
			{
				string style = keyStyle.Trim().ToLower();
				if (stylesAllowed.Contains(style))
				{
					currentStyle.AddRewrite(style, newStyle[keyStyle].Trim().TrimEnd(';').Trim().ToLower());
				}
			}
		}

		/// <summary>
		/// Проверяет, установлен ли указанный стиль
		/// </summary>
		private bool StyleContains(string style, string value)
		{
			string realValue;
			if (currentStyle.TryGetValue(style, out realValue))
			{
				return value.ToLower().Equals(realValue.ToLower());
			}
			return false;
		}
		
		/// <summary>
		/// Получает значение CSS-атрибута margin (Y, Width, Height, X)
		/// </summary>
		protected RectangleF ParseCssMargins()
		{
			RectangleF result = new RectangleF();

			string value;
			if (currentStyle.TryGetValue("margin", out value))
			{
				List<string> values = new List<string>(value.Split(' '));
				values.Capacity = 4;
				if (values.Count == 1) values.Insert(1, values[0]);
				if (values.Count == 2) values.Insert(2, values[0]);
				if (values.Count == 3) values.Insert(3, values[1]);

				for (int i = 0; i < values.Count; i++)
				{
					float floatValue;
					if (!ParseCssValue(values[i], out floatValue))
						continue;

					switch (i)
					{
						case 0:
							result.Y = floatValue;
							break;
						case 1:
							result.Width = floatValue;
							break;
						case 2:
							result.Height = floatValue;
							break;
						case 3:
							result.X = floatValue;
							break;
					}
				}
			}
			return result;
		}

		/// <summary>
		/// Получает значение, соответствующее имени стиля
		/// </summary>
		private static string GetCssValue(string cssRule, string cssStyle)
		{
			int ruleIndex = cssStyle.IndexOf(cssRule);
			if (ruleIndex == -1)
				return String.Empty;
			
			int startIndex = cssStyle.IndexOf(":", ruleIndex);
			if (startIndex == -1)
				return String.Empty;
			startIndex++;

			int endIndex = cssStyle.IndexOf(";", startIndex);
			if (endIndex == -1)
				endIndex = cssStyle.Length - 1;

			return cssStyle.Substring(startIndex, endIndex - startIndex).Trim();
		}

		/// <summary>
		/// Получает нормализованное значение CSS-атрибута text-align
		/// </summary>
		private static ParagraphAlignment ParseCssAlignment(string value)
		{
			switch (value)
			{
				case "center":
					return ParagraphAlignment.Center;
				case "justify":
					return ParagraphAlignment.Both;
				case "right":
					return ParagraphAlignment.Right;
				case "left":
					return ParagraphAlignment.Left;
				default:
					return ParagraphAlignment.Left;
			}
		}

		/// <summary>
		/// Переводит единицы расстояния CSS в пункты
		/// </summary>
		private static bool ParseCssValue(string value, out float floatValue)
		{
			double scale = 1;
			if (value.Contains("px"))
			{
				scale = 0.75;
			}

			value = value
				.Replace("pt", String.Empty)
				.Replace("px", String.Empty);

			if (Single.TryParse(value, out floatValue))
			{
				floatValue = (float)(floatValue * scale);
				return true;
			}

			return false;
		}

		/// <summary>
		/// Содержит параметры экспорта диаграммы
		/// </summary>
		protected ChartExporter Chart { get { return chartExporter ?? (chartExporter = new ChartExporter()); } }
		private ChartExporter chartExporter;
		protected class ChartExporter
		{
			public double ScaleFactor { set; get; }

			public ChartExporter()
			{
				ScaleFactor = 1;
			}

			/// <summary>
			/// Экспорт диаграммы
			/// </summary>
			public Image Export(UltraChart control)
			{
				// не используем RenderPdfFriendlyGraphics, потому что он косячит перевод пикселей в пункты

				MemoryStream imageStream = new MemoryStream();
				control.SaveTo(imageStream, ImageFormat.Png);

				return (new Bitmap(imageStream)).ScaleImage(ScaleFactor, 96);
			}

		}

		/// <summary>
		/// Содержит параметры экспорта таблицы и внутренние вспомогательные методы
		/// </summary>
		protected GridExporter Grid { get { return gridExporter ?? (gridExporter = new GridExporter()); } }
		private GridExporter gridExporter;
		protected class GridExporter
		{
			private WDocument Document { set; get; }
			private WTable Table { set; get; }
			
			private GridHeaderLayout HeaderLayout { set; get; }
			private Collection<WTableRow> HeaderRows { set; get; }
			private Color HeaderBackgroundColor { set; get; }
			private Color AlternativeRowBackgroundColor { set; get; }

			private string DefaultFontName { set; get; }
			private FontUnit DefaultFontSize { set; get; }

			public double ScaleFontFactor { set; get; }
			public double ScaleColumnsWidthFactor { set; get; }
			public double ScalePicturesFactor { set; get; }

			
			public GridExporter()
			{
				DefaultFontName = "Arial";
				DefaultFontSize = FontUnit.Empty;

				HeaderBackgroundColor = Color.FromArgb(unchecked((int)0xFFcccccc));
				AlternativeRowBackgroundColor = Color.FromArgb(unchecked((int)0xFFf1f1f1));

				// По умолчанию ничего не масштабируем
				ScaleFontFactor = 1;
				ScaleColumnsWidthFactor = 1;
				ScalePicturesFactor = 1;
			}

			/// <summary>
			/// Устанавливает масштаб сразу для всех элементов
			/// </summary>
			public double ScaleFactor
			{
				set
				{
					ScaleFontFactor = value;
					ScaleColumnsWidthFactor = value;
					ScalePicturesFactor = value;
				}
			}

			public void Export(WDocument document, GridHeaderLayout headerLayout)
			{
				Document = document;
				HeaderLayout = headerLayout;
				Table = document.ContentBlocks.AddTable();
				
				if (HeaderLayout.AllChildCount == 0)
					return;
				
				// если шрифт по умолчанию не определен, возьмем его из UltraGridBrick
				if (DefaultFontSize.Type == FontSize.NotSet)
				{
					DefaultFontSize = HeaderLayout.Grid.DisplayLayout.RowStyleDefault.Font.Size;
				}

				if (HeaderLayout.Grid.Rows.Count == 0)
				{
					Paragraph para = Document.ContentBlocks.AddParagraph();
					para.Properties.Alignment = ParagraphAlignment.Center;
					TextRun text = para.ContentRuns.AddTextRun("Нет данных");
					text.Font.Size = (float)(DefaultFontSize.Unit.Value * ScaleFontFactor);
					return;
				}

				GenerateTableHeader();
				GenerateTableBody();
			}
			
			#region Формирование заголовка грида

			/// <summary>
			/// Формирует заголовок грида (хидер)
			/// </summary>
			private void GenerateTableHeader()
			{
				// остальные колонки
				for (int i = 0; i < HeaderLayout.AllChildCount; i++)
				{
					WTableColumn column = Table.Columns.Add();
					column.PreferredWidth = GetGridColumnWidth(i);
				}

				// строки, запоминаем в переменную объекта
				HeaderRows = new Collection<WTableRow>();
				for (int i = 0; i < HeaderLayout.HeaderHeight; i++)
				{
					HeaderRows.Add(Table.Rows.Add());
				}
				
				// формируем хидер
				AddHeaderTable(HeaderLayout, -1);

			}

			/// <summary>
			/// Формирует заголовок грида для текущего уровня хидера
			/// </summary>
			private void AddHeaderTable(GridHeaderCell headerCell, int rowIndex)
			{
				// пропуск заголовочной ячейки (сам HeaderLayout)
				if (rowIndex >= 0)
				{
					WTableRow tableRow = HeaderRows[rowIndex];
					AddHeaderCell(headerCell, tableRow, rowIndex);
				}

				foreach (GridHeaderCell childCell in headerCell.childCells)
				{
					AddHeaderTable(childCell, rowIndex + childCell.SpanY);
				}
			}

			/// <summary>
			/// Добавление ячейки в хидер
			/// </summary>
			private void AddHeaderCell(GridHeaderCell headerCell, WTableRow gridRow, int rowIndex)
			{
				WTableCell tableCell = gridRow.Cells.Add();
				tableCell.Properties.VerticalAlignment = TableCellVerticalAlignment.Center;
				tableCell.Properties.BackColor = HeaderBackgroundColor;
				tableCell.Properties.ColumnSpan = headerCell.AllChildCount;
				tableCell.Properties.VerticalMerge = TableCellVerticalMerge.Start;
				
				int spanY = headerCell.ChildCount == 0
					? HeaderLayout.HeaderHeight - rowIndex
					: headerCell.SpanY;
				
				for (int rowDelta = 1; rowDelta < spanY; rowDelta++)
				{
					for (int colDelta = 0; colDelta < tableCell.Properties.ColumnSpan; colDelta++)
					{
						WTableRow row = HeaderRows[rowIndex + rowDelta];
						WTableCell cell = row.Cells.Add();
						cell.Properties.VerticalMerge = TableCellVerticalMerge.Continue;
					}
				}
				
				Paragraph para = tableCell.ContentBlocks.AddParagraph();
				para.Properties.Alignment = ParagraphAlignment.Center;
				
				// обработка текста
				string caption = headerCell.Caption;
				caption = caption
					.Replace("&nbsp;", " ");
				caption = Regex.Replace(caption, "<br[^>]*?>", Environment.NewLine);
				caption = Regex.Replace(caption, "<[^>]*?>", String.Empty);

				TextRun text = para.ContentRuns.AddTextRun(caption);
				text.Font.Name = DefaultFontName;
				text.Font.NameHighAnsi = DefaultFontName;
				text.Font.Size = (float)(DefaultFontSize.Unit.Value * ScaleFontFactor);
				text.Font.Bold = true;
			}
			
			/// <summary>
			/// Получает ширину колонки грида (в пунктах)
			/// </summary>
			private float GetGridColumnWidth(int columnIndex)
			{
				UltraGridColumn column = HeaderLayout.GetNonHiddenColumn(columnIndex);
				if (column != null)
				{
					return CRHelper.PixelsToPoints(column.Width.Value);
				}
				return 0;
			}

			#endregion

			#region Формирование тела грида

			/// <summary>
			/// Формирует тело таблицы
			/// </summary>
			private void GenerateTableBody()
			{
				for (int rowIndex = 0; rowIndex < HeaderLayout.Grid.Rows.Count; rowIndex++)
				{
					UltraGridRow gridRow = HeaderLayout.Grid.Rows[rowIndex];
					WTableRow tableRow = Table.Rows.Add();
					
					for (int columnIndex = 0; columnIndex < HeaderLayout.AllChildCount; columnIndex++)
					{
						UltraGridCell gridCell = gridRow.Cells[columnIndex];
						WTableCell tableCell = tableRow.Cells.Add();
						
						if (rowIndex % 2 == 1)
						{
							tableCell.Properties.BackColor = AlternativeRowBackgroundColor;
						}

						AddTableCell(tableCell, gridCell);
					}
				}
			}

			private void AddTableCell(WTableCell tableCell, UltraGridCell gridCell)
			{
				tableCell.Properties.VerticalAlignment = TableCellVerticalAlignment.Center;

				Paragraph para = tableCell.ContentBlocks.AddParagraph();

				// отступы в ячейке
				if (!gridCell.Style.CustomRules.IsEmpty())
				{
					float floatValue;
					if (ParseCssValue(GetCssValue("padding-left", gridCell.Style.CustomRules), out floatValue))
					{
						if (Document.DefaultTableProperties.CellProperties.Margins.HasValue)
						{
							Padding padding = Document.DefaultTableProperties.CellProperties.Margins.Value;
							tableCell.Properties.Margins = new Padding(floatValue, padding.Top, padding.Right, padding.Bottom);
						}
					}
				}

				// выравнивание текста
				switch (gridCell.Column.CellStyle.HorizontalAlign)
				{
					case HorizontalAlign.Center:
						para.Properties.Alignment = ParagraphAlignment.Center;
						break;
					case HorizontalAlign.Justify:
						para.Properties.Alignment = ParagraphAlignment.Both;
						break;
					case HorizontalAlign.Right:
						para.Properties.Alignment = ParagraphAlignment.Right;
						break;
					default:
						para.Properties.Alignment = ParagraphAlignment.Left;
						break;
				}

				// фоновая картинка
				if (!String.IsNullOrEmpty(gridCell.Style.BackgroundImage))
				{
					string imagePath = HttpContext.Current.Server.MapPath(gridCell.Style.BackgroundImage);
					//AnchoredPicture pic = para.Anchors.AddAnchoredPicture(new Bitmap(imagePath));
					para.ContentRuns.AddInlinePicture(new Bitmap(imagePath));
				}
				
				string value = String.Empty;
				if (gridCell.Value != null)
				{
					// формат текста
					string format = String.Format("{{0{0}{1}}}", !gridCell.Column.Format.IsEmpty() ? ":" : String.Empty, gridCell.Column.Format);
					value = String.Format(format, gridCell.Value);
				}

				// обработка текста
				value = value
					.Replace("&nbsp;", " ");
				value = Regex.Replace(value, "<br[^>]*?>", Environment.NewLine);
				value = Regex.Replace(value, "<[^>]*?>", String.Empty);
				
				TextRun text = para.ContentRuns.AddTextRun(value);
				text.Font.Name = DefaultFontName;
				text.Font.NameHighAnsi = DefaultFontName;
				text.Font.Size = (float)(DefaultFontSize.Unit.Value * ScaleFontFactor);
				text.Font.Bold = gridCell.Style.Font.Bold;
				
			}

			#endregion

			#region Вспомогательные методы

			/// <summary>
			/// Масштабирует размер шрифта
			/// </summary>
			private void ScaleFont(FontInfo font)
			{
				if (font.Size.Type == FontSize.NotSet)
				{
					font.Size = DefaultFontSize;
				}

				if (font.Size.Type == FontSize.AsUnit && font.Size.Unit.Type == UnitType.Point)
				{
					UnitType unitType = font.Size.Unit.Type;
					font.Size = new FontUnit((int)Math.Round(font.Size.Unit.Value * ScaleFontFactor), unitType);
				}
			}

			#endregion
		}
	}
}
