using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Graphics;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Grid;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Color = System.Drawing.Color;
using Font = System.Drawing.Font;
using Graphics = System.Drawing.Graphics;
using IgColor = Infragistics.Documents.Reports.Graphics.Color;
using IgFont = Infragistics.Documents.Reports.Graphics.Font;
using IgImage = Infragistics.Documents.Reports.Graphics.Image;
using Image = System.Drawing.Image;
using Pen = Infragistics.Documents.Reports.Graphics.Pen;
using TextureBrush = Infragistics.Documents.Reports.Graphics.TextureBrush;

namespace Krista.FM.Server.Dashboards.Common.Export
{
	/// <summary>
	/// Экспортирует таблицу в PDF
	/// </summary>
	public class ExportGridPdf : IExportablePdf
	{
		// поправка на селектор и прочее (в пунктах)
		private const double widthCorrection = 5;

		public ExportGridParams Params { set; get; }

		public ISection Section { set; get; }

		public UltraWebGridDocumentExporter UltraWebGridDocumentExporter { set; get; }

		// TODO Размер границ ячейки надо брать из UltraGridBrick
		private const float BorderSize = 2;
		
		// TODO Цвет границ ячейки надо брать из UltraGridBrick
		private Borders HeaderBorders = 
			new Borders
			{
				Left = new Border(new Pen(new IgColor(Color.White), BorderSize)),
				Top = new Border(new Pen(new IgColor(Color.White), BorderSize)),
				Right = new Border(new Pen(new IgColor(Color.Silver), BorderSize)),
				Bottom = new Border(new Pen(new IgColor(Color.Silver), BorderSize))
			};
		
		private Collection<IGridRow> HeaderRows { set; get; }

		/// <summary>
		/// Инстанцирует класс, экспортирующий таблицу в PDF
		/// </summary>
		public ExportGridPdf(ExportGridParams paramsGrid, UltraWebGridDocumentExporter ultraWebGridDocumentExporter)
		{
			Params = paramsGrid;
			UltraWebGridDocumentExporter = ultraWebGridDocumentExporter;
		}

		/// <summary>
		/// Экспортирует таблицу в PDF
		/// </summary>
		public void Export()
		{
			// масштабировать ширину столбцов
			Params.ResultWidth = ScaleColumnsWidth() + CRHelper.PixelsToPoints(Params.Margins.Left + Params.Margins.Right);

			UltraWebGridDocumentExporter.BeginExport += BeginExport;
			UltraWebGridDocumentExporter.HeaderRowExporting += HeaderRowExporting;
			UltraWebGridDocumentExporter.RowExporting += RowExporting;
			UltraWebGridDocumentExporter.CellExporting += CellExporting;

			UltraWebGridDocumentExporter.Export(Params.HeaderLayout.Grid, Section);

			UltraWebGridDocumentExporter.BeginExport -= BeginExport;
			UltraWebGridDocumentExporter.HeaderRowExporting -= HeaderRowExporting;
			UltraWebGridDocumentExporter.RowExporting -= RowExporting;
			UltraWebGridDocumentExporter.CellExporting -= CellExporting;
		}
		
		#region Обработчики

		public void BeginExport(object sender, DocumentExportEventArgs e)
		{
			// TODO Должно выставляться в UltraGridBrick и везде использоваться оттуда
			e.Layout.HeaderStyleDefault.BackColor = Params.HeaderBackgroundColor;

			// если шрифт по умолчанию не определен, возьмем его из UltraGridBrick
			if (Params.DefaultFontSize.Type == FontSize.NotSet)
			{
				Params.DefaultFontSize = e.Layout.RowStyleDefault.Font.Size;
			}
		}

		public void HeaderRowExporting(object sender, MarginRowExportingEventArgs e)
		{
			// создаем самодельный хидер
			GenerateTableHeader(e.ContainingTable.Header.AddCell());
			e.ContainingTable.Header.Repeat = true;

			// а создание дефолтного отменяем
			e.Cancel = true;
		}

		public void RowExporting(object sender, RowExportingEventArgs e)
		{
			for (int i = 0; i < e.GridRow.Cells.Count; i++)
			{
				e.GridRow.Cells[i].Style.Font.Name = Params.DefaultFontName;

				// масштаб шрифта ячеек
				ScaleFont(e.GridRow.Cells[i].Style.Font);
			}
		}
		
		public void CellExporting(object sender, CellExportingEventArgs e)
		{
			// обработка текста в ячейке
			// TODO Сделать нормальную обработку в IText используя AddRichContent
			if (e.ExportValue is string)
			{
				string value = e.ExportValue as string;
				value = Regex.Replace(value, "<br[^>]*?>", Environment.NewLine);
				value = Regex.Replace(value, "<[^>]*?>", String.Empty);
				e.ExportValue = value
					.Replace("&nbsp;", " ");
			}

			if (!String.IsNullOrEmpty(e.GridCell.Style.BackgroundImage))
			{
				string imagePath = HttpContext.Current.Server.MapPath(e.GridCell.Style.BackgroundImage);

				Color backColor = Color.Transparent;
				if (e.ReportCell.Background.Brush.Type == BrushType.SolidColor)
				{
					IgColor color = ((SolidColorBrush)e.ReportCell.Background.Brush).Color;
					backColor = Color.FromArgb(unchecked((int)(color.ToARGB())));
				}

				CellBackgroundDrawing drawing = new CellBackgroundDrawing(imagePath, backColor);
				drawing.Scale = Convert.ToSingle(Params.ScalePicturesFactor);
				drawing.ImageBorderSize = BorderSize;

				e.ReportCell.Background = new Background(e.ReportCell.Background.Brush, drawing);
			}
		}

		#endregion

		#region Перерисовка картинок

		/// <summary>
		/// Воспомогательный класс для отрисовки картинок в гриде
		/// </summary>
		private class CellBackgroundDrawing : IDrawing
		{
			// масштаб картинок, выполняется в любом случае
			private const float primaryScale = 0.75f;

			private IgImage Picture { set; get; }
			public float ImageBorderSize { set; get; }
			public float Scale { set; get; }

			public CellBackgroundDrawing(string imagePath, Color backColor)
			{
				// Загружаем картинку
				Image imageLoaded = Image.FromFile(imagePath);

				// Рисуем картинку с рамкой
				Bitmap bitmap = new Bitmap(imageLoaded.Width + 4, imageLoaded.Height + 4);
				bitmap.SetResolution(72, 72);

				// Создаем из нее картинку для ячейки.
				using (Graphics graphics = Graphics.FromImage(bitmap))
				{
					graphics.CompositingQuality = CompositingQuality.HighQuality;
					graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
					graphics.SmoothingMode = SmoothingMode.AntiAlias;

					SolidBrush brush = new SolidBrush(backColor);
					graphics.FillRectangle(brush, 0, 0, bitmap.Width, bitmap.Height);
					graphics.DrawImageUnscaled(imageLoaded, 2, 2);
				}

				Picture = new IgImage(bitmap);
				Picture.Preferences.Compressor = ImageCompressors.Flate;
			}

			public void OnDraw(IGraphics graphics, float x, float y, float width, float height)
			{
				float scale = primaryScale * Scale;
				float newWidth = Picture.Width * scale;
				float newHeight = Picture.Height * scale;
				float offsetX = CRHelper.PixelsToPoints(5);
				float offsetY = (height - CRHelper.PixelsToPoints(newHeight) - ImageBorderSize) / 2;


				TextureBrush textureBrush = new TextureBrush(Picture);
				textureBrush.ScaleX = scale;
				textureBrush.ScaleY = scale;
				graphics.Brush = textureBrush;
				graphics.FillRectangle(x + offsetX, y + offsetY, newWidth, newHeight);

			}
		}

		#endregion

		#region Формирование заголовка грида

		/// <summary>
		/// Формирует заголовок грида (хидер)
		/// </summary>
		private void GenerateTableHeader(ITableCell rootCell)
		{
			int allColumnsCount = Params.HeaderLayout.AllChildCount;
			if (allColumnsCount == 0)
				return;

			// таблица хидера
			IGrid table = rootCell.AddGrid();

			// колонка ячейки селектора
			table.AddColumn().Width = new FixedWidth(16.5f);

			// остальные колонки
			for (int i = 0; i < allColumnsCount; i++)
			{
				IGridColumn column = table.AddColumn();
				column.Width = new FixedWidth(GetGridColumnWidth(i));
			}

			// строки, запоминаем в переменную объекта
			HeaderRows = new Collection<IGridRow>();
			for (int i = 0; i < Params.HeaderLayout.HeaderHeight; i++)
			{
				HeaderRows.Add(table.AddRow());
			}

			// добавляем ячейку селектора
			AddSelectorCell(HeaderRows[0]);

			// формируем хидер
			AddHeaderTable(Params.HeaderLayout, -1);

		}

		/// <summary>
		/// Формирует заголовок грида для текущего уровня хидера
		/// </summary>
		private void AddHeaderTable(GridHeaderCell headerCell, int rowIndex)
		{
			// пропуск заголовочной ячейки (сам HeaderLayout)
			if (rowIndex >= 0)
			{
				IGridRow gridRow = HeaderRows[rowIndex];
				AddHeaderCell(headerCell, gridRow, rowIndex);
			}

			foreach (GridHeaderCell childCell in headerCell.childCells)
			{
				AddHeaderTable(childCell, rowIndex + childCell.SpanY);
			}
		}

		/// <summary>
		/// Добавление ячейки в хидер
		/// </summary>
		private void AddHeaderCell(GridHeaderCell headerCell, IGridRow gridRow, int rowIndex)
		{
			IGridCell gridCell = gridRow.AddCell();
			SetHeaderCellStyle(gridCell);
			gridCell.ColSpan = headerCell.AllChildCount;
			gridCell.RowSpan =
				headerCell.ChildCount == 0
				? Params.HeaderLayout.HeaderHeight - rowIndex
				: headerCell.SpanY;

			IText text = gridCell.AddText();
			SetHeaderCellTextStyle(text);

			// обработка текста
			string caption = headerCell.Caption;
			caption = caption
				.Replace("&nbsp;", " ");
			caption = Regex.Replace(caption, "<br[^>]*?>", Environment.NewLine);
			caption = Regex.Replace(caption, "<[^>]*?>", String.Empty);

			text.AddContent(caption);
		}

		/// <summary>
		/// Устанавливает стиль ячейки хидера
		/// </summary>
		private void SetHeaderCellStyle(IGridCell cell)
		{
			cell.Alignment.Horizontal = Alignment.Center;
			cell.Alignment.Vertical = Alignment.Middle;
			cell.Paddings.All = 2;
			cell.Borders = HeaderBorders;
			cell.Background = new Background(new IgColor(Params.HeaderBackgroundColor));
		}

		/// <summary>
		/// Учтанавливает стиль текста ячейки хидера
		/// </summary>
		private void SetHeaderCellTextStyle(IText text)
		{
			text.Style.Font = new IgFont(new Font(Params.DefaultFontName, (float)(Params.DefaultFontSize.Unit.Value * Params.ScaleFontFactor)));
			text.Style.Font.Bold = true;
			text.Alignment = TextAlignment.Center;
		}

		/// <summary>
		/// Добавляет ячейку селектора
		/// </summary>
		private void AddSelectorCell(IGridRow row)
		{
			IGridCell cell = row.AddCell();
			cell.RowSpan = Params.HeaderLayout.HeaderHeight;
			cell.Borders = HeaderBorders;
			cell.Background = new Background(new IgColor(Params.HeaderBackgroundColor));
		}

		/// <summary>
		/// Получает ширину колонки грида (в пунктах)
		/// </summary>
		private float GetGridColumnWidth(int columnIndex)
		{
			UltraGridColumn column = Params.HeaderLayout.GetNonHiddenColumn(columnIndex);
			if (column != null)
			{
				return CRHelper.PixelsToPoints(column.Width.Value);
			}
			return 0;
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
				font.Size = Params.DefaultFontSize;
			}
			
			if (font.Size.Type == FontSize.AsUnit && font.Size.Unit.Type == UnitType.Point)
			{
				UnitType unitType = font.Size.Unit.Type;
				int newSize = (int) Math.Round(font.Size.Unit.Value*Params.ScaleFontFactor);
				if (newSize == 0) 
					{ newSize = 1; }
				font.Size = new FontUnit(newSize, unitType);
			}
		}

		/// <summary>
		/// Масштабирует ширину столбцов
		/// </summary>
		private double ScaleColumnsWidth()
		{
			Params.HeaderLayout.Grid.Bands[0].ScaleColumnsWidth(Params.ScaleColumnsWidthFactor);
			
			double width = 0;
			// минимальная ширина столбца для PDF около 16 пикселей - хз почему
			foreach (UltraGridColumn column in Params.HeaderLayout.Grid.Bands[0].Columns)
			{
				if (Math.Abs(column.Width.Value) < 16)
				{
					column.Width = 16;
				}
				width += column.Width.Value;
			}
			
			return CRHelper.PixelsToPoints(width) + widthCorrection;
		}

		#endregion

	}
}
