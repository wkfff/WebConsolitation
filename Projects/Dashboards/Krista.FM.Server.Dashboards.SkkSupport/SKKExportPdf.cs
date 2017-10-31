using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Graphics;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Band;
using Infragistics.Documents.Reports.Report.Flow;
using Infragistics.Documents.Reports.Report.Grid;
using Infragistics.Documents.Reports.Report.Index;
using Infragistics.Documents.Reports.Report.QuickList;
using Infragistics.Documents.Reports.Report.QuickTable;
using Infragistics.Documents.Reports.Report.QuickText;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Segment;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Reports.Report.TOC;
using Infragistics.Documents.Reports.Report.Tree;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Color = System.Drawing.Color;
using FontUnit = System.Web.UI.WebControls.FontUnit;
using Graphics = System.Drawing.Graphics;
using IgBorders = Infragistics.Documents.Reports.Report.Borders;
using IgColor = Infragistics.Documents.Reports.Graphics.Color;
using IgContentAlignment = Infragistics.Documents.Reports.Report.ContentAlignment;
using IgFont = Infragistics.Documents.Reports.Graphics.Font;
using IgFontStyle = Infragistics.Documents.Reports.Graphics.FontStyle;
using IgImage = Infragistics.Documents.Reports.Graphics.Image;
using IgMargins = Infragistics.Documents.Reports.Report.Margins;
using IgMetafile = Infragistics.Documents.Reports.Graphics.Metafile;
using IgPen = Infragistics.Documents.Reports.Graphics.Pen;
using IgReport = Infragistics.Documents.Reports.Report.Report;
using IgSize = Infragistics.Documents.Reports.Graphics.Size;
using IgStyle = Infragistics.Documents.Reports.Report.Text.Style;
using IgTextAlignment = Infragistics.Documents.Reports.Report.TextAlignment;
using IgTextureBrush = Infragistics.Documents.Reports.Graphics.TextureBrush;
using IList = Infragistics.Documents.Reports.Report.List.IList;
using Image = System.Drawing.Image;
using PageOrientation = Infragistics.Documents.Reports.Report.PageOrientation;

namespace Krista.FM.Server.Dashboards.SkkSupport
{
	public class SKKExportPDF : SKKExportBase
	{
		private IgReport report { set; get; }
		private IgStyle CommonTextStyle { set; get; }
		private ReportHolder Holder { set; get; }

		private readonly Control rootControl;
		private HashSet<ServiceFlags> servicesFlags;
		private Dictionary<string, string> currentStyle;
		private Collection<string> stylesAllowed;
		private Collection<string> stylesInherits;
		private int columnsIndex;
		private int chartGroupsIndex;
		private double chartGroupsScale;

		public SKKExportPDF(UltraWebGridDocumentExporter gridExporter, Control rootControl)
			: base(gridExporter.Page, ExportType.Pdf)
		{
			Grid.UltraWebGridDocumentExporter = gridExporter;
			this.rootControl = rootControl;

			report = new IgReport();

			Holder = new ReportHolder(report);
			Holder.PageOrientation = PageOrientation.Portrait;
			Holder.PageSize = PageSizes.A4;

			CommonTextStyle = new IgStyle(
				new IgFont("Arial", CRHelper.PixelsToPoints(TextHeight)),
				new SolidColorBrush(new IgColor(Color.Black))
				);
			
			Init();
		}

		private void Init()
		{
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

			Chart.ScaleFactor = 1;
			Grid.ScaleFactor = 0.70;

			columnsIndex = 0;
			chartGroupsIndex = 0;
			chartGroupsScale = 0.85;
		}

		/// <summary>
		/// Добавляет хидер
		/// </summary>
		protected void AddHeader()
		{
			if (HeaderHeight == 0)
				return;

			IgStyle textStyle = new IgStyle(
				new IgFont("Verdana", CRHelper.PixelsToPoints(HeaderFooterTextHeight), IgFontStyle.Italic),
				new SolidColorBrush(new IgColor(Color.Black))
				);

			// колонтитул для первой страницы

			ISectionHeader header = Holder.AddHeader();
			header.Repeat = false;
			header.Height = CRHelper.PixelsToPoints(HeaderHeight);

			IText text;
			for (int i = 0; i < 2; i++)
			{
				text = header.AddText(0, 0);
				text.Paddings.Top = CRHelper.PixelsToPoints(30);
				text.Paddings.Bottom = CRHelper.PixelsToPoints(30);
				text.Alignment.Vertical = Alignment.Middle;
				text.Height = new RelativeHeight(100);
				text.Style = textStyle;

				text.Alignment.Horizontal = (i == 0)
					? Alignment.Left
					: Alignment.Right;
				text.AddRichContent((i == 0)
					? Header1stTextLeft
					: Header1stTextRight);
			}


			IRule rule = header.AddRule(0, CRHelper.PixelsToPoints(HeaderHeight - 10) - 1);
			rule.Pen = new IgPen(new IgColor(Color.Gray)) { Width = 1 };

			// колонтитул для остальных страниц

			textStyle = new IgStyle(
				new IgFont("Verdana", CRHelper.PixelsToPoints(HeaderFooterTextHeight), IgFontStyle.Italic),
				new SolidColorBrush(new IgColor(Color.Black))
				);

			header = Holder.AddHeader();
			header.Repeat = true;
			header.Height = CRHelper.PixelsToPoints(HeaderHeight);

			text = header.AddText(0, 0);
			text.Paddings.Top = CRHelper.PixelsToPoints(20);
			text.Paddings.Bottom = CRHelper.PixelsToPoints(30);
			text.Alignment.Horizontal = Alignment.Center;
			text.Alignment.Vertical = Alignment.Middle;
			text.Height = new RelativeHeight(100);
			text.Style = textStyle;
			text.AddRichContent(HeaderText);

			rule = header.AddRule(0, CRHelper.PixelsToPoints(HeaderHeight - 10) - 1);
			rule.Pen = new IgPen(new IgColor(Color.Gray)) { Width = 1 };

		}

		/// <summary>
		/// Добавляет футер
		/// </summary>
		protected void AddFooter()
		{
			if (FooterHeight == 0)
				return;

			float paddingBottom = CRHelper.PixelsToPoints(FooterHeight - HeaderFooterTextHeight - 13) / 2;

			IgStyle textStyle = new IgStyle(
				new IgFont("Verdana", CRHelper.PixelsToPoints(HeaderFooterTextHeight), IgFontStyle.Italic),
				new SolidColorBrush(new IgColor(Color.Black))
				);

			ISectionFooter footer = Holder.AddFooter();
			footer.Repeat = true;
			footer.Height = CRHelper.PixelsToPoints(FooterHeight);

			IText text = footer.AddText(0, 0);
			text.Height = new RelativeHeight(100);
			text.Paddings.Bottom = paddingBottom;
			text.Alignment.Horizontal = Alignment.Left;
			text.Alignment.Vertical = Alignment.Bottom;
			text.Style = textStyle;
			text.AddRichContent(FooterText);

			IRule rule = footer.AddRule(0, CRHelper.PixelsToPoints(10));
			rule.Pen = new IgPen(new IgColor(Color.Gray)) { Width = 1 };

			// PageNumbering
			(Holder as ISection).PageNumbering =
				new PageNumbering
				{
					Template = PageNumberingTemplate,
					Alignment =
					{
						Horizontal = Alignment.Right,
						Vertical = Alignment.Bottom
					},
					Style = textStyle,
					SkipFirst = false,
					OffsetX = -(float)(Holder.PageMargins.Right * 0.2),
					OffsetY = -Holder.PageMargins.Bottom - paddingBottom
				};

		}

		protected override void Publish()
		{
			report.Publish(stream, FileFormat.PDF);
			base.Publish();
		}

		/// <summary>
		/// Экспорт отчета
		/// </summary>
		public override void Export()
		{
			ExportContent();

			Holder.PageMargins.Left = CRHelper.PixelsToPoints(90);
			Holder.PageMargins.Right = CRHelper.PixelsToPoints(30);
			Holder.PageMargins.Vertical = CRHelper.PixelsToPoints(10);
			Holder.PagePaddings.All = 0;

			Holder.PageOrientation = PageOrientation.Portrait;
			Holder.PageSize = PageSizes.A4;

			AddHeader();
			AddFooter();

			Publish();
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
					// сохранить состояние
					Holder.PushBand();

					Holder.AddBand();
					ApplyHtmlInnerStyles();

					// debug
					//Holder.AddRule().Pen = new Pen(new IgColor(Color.Indigo));
					//Holder.AddQuickText(" ");
					// enddebug

					ExportControls(control.Controls);

					// debug
					//Holder.AddRule().Pen = new Pen(new IgColor(Color.Green));
					// enddebug

					// восстановить состояние
					Holder.PopBand();
				}
			}

			currentStyle = parentStyle;
		}

		/// <summary>
		/// Экспорт текстового элемента
		/// </summary>
		protected void ExportTextControl(LiteralControl control)
		{
			IText iText = Holder.AddText();

			string value;
			if (currentStyle.TryGetValue("text-align", out value))
			{
				iText.Alignment.Horizontal = ParseCssAlignment(value);
			}
			if (currentStyle.TryGetValue("font-weight", out value))
			{
				switch (value)
				{
					case "bold":
						iText.Style.Font.Bold = true;
						break;
				}
			}
			if (currentStyle.TryGetValue("font-size", out value))
			{
				float floatValue;
				if (ParseCssValue(value, out floatValue))
				{
					iText.Style.Font.Size = floatValue;
				}
			}

			// если является заголовком, то не разрывать абзац
			if (iText.Alignment.Horizontal == Alignment.Center)
			{
				iText.KeepSolid = true;
			}

			// отступ параграфа
			IgImage paragraphImg = new IgImage(1, 1);
			paragraphImg.SetPixel(0, 0, new IgColor(Color.Transparent));

			// постобработка текста
			string raw_text = control.Text
				.Replace("&nbsp;", ((char)160).ToString());
			
			string[] text = raw_text.Split(new[] { SKKHelper.AddParagraph() }, StringSplitOptions.RemoveEmptyEntries);

			// составляем текст из параграфов
			for (int i = 0; i < text.Length; i++)
			{
				if (i > 0 || (i == 0 && raw_text.StartsWith(SKKHelper.AddParagraph())))
				{
					iText.AddContent(paragraphImg, new IgSize(CRHelper.PixelsToPoints(SKKHelper.paragraphLength), 1));
				}
				iText.AddRichContent(text[i]);
			}

		}

		/// <summary>
		/// Экспорт диаграммы
		/// </summary>
		protected override void ExportChartControl(UltraChart control)
		{
			if (chartGroupsIndex == 1)
			{
				control.Width = new Unit(control.Width.Value * chartGroupsScale);
				control.Axis.X.Margin.Far.Value = control.Axis.X.Margin.Far.Value / chartGroupsScale;
			}
			Holder.AddImage(Chart.Export(control));
		}

		/// <summary>
		/// Экспорт таблицы
		/// </summary>
		protected override void ExportGridControl(UltraGridBrick control)
		{
			if (control.DataTable.Rows.Count == 0)
			{
				IText iText = Holder.AddText();
				ParseCssFontSize(iText.Style.Font);
				iText.Alignment.Horizontal = Alignment.Center;
				iText.Style.Font.Bold = true;
				iText.Paddings.Vertical = CRHelper.PixelsToPoints(30);
				iText.AddContent("Нет данных");
			}
			else
			{
				// %
				if (servicesFlags.Contains(ServiceFlags.GridGroups))
				{
					control.Grid.Bands[0].Columns[4].Width = CRHelper.GetColumnWidth(85);
					control.Grid.Bands[0].Columns[6].Width = CRHelper.GetColumnWidth(85);
				}
				ExportGridControlPDF(control);
			}
		}

		/// <summary>
		/// Экспорт таблицы
		/// </summary>
		private void ExportGridControlPDF(UltraGridBrick control)
		{
			Grid.HeaderLayout = control.GridHeaderLayout;

			// масштабировать ширину столбцов
			control.Grid.Bands[0].ScaleColumnsWidth(Grid.ScaleColumnsWidthFactor);

			Grid.UltraWebGridDocumentExporter.BeginExport += Grid.BeginExport;
			Grid.UltraWebGridDocumentExporter.HeaderRowExporting += Grid.HeaderRowExporting;
			Grid.UltraWebGridDocumentExporter.InitializeRow += Grid.InitializeRow;
			Grid.UltraWebGridDocumentExporter.CellExporting += Grid.CellExporting;

			Grid.UltraWebGridDocumentExporter.Export(control.GridHeaderLayout.Grid, Holder);

			Grid.UltraWebGridDocumentExporter.BeginExport -= Grid.BeginExport;
			Grid.UltraWebGridDocumentExporter.HeaderRowExporting -= Grid.HeaderRowExporting;
			Grid.UltraWebGridDocumentExporter.InitializeRow -= Grid.InitializeRow;
			Grid.UltraWebGridDocumentExporter.CellExporting -= Grid.CellExporting;
		}

		/// <summary>
		/// Применить управляющие стили разметки
		/// </summary>
		protected void ApplyHtmlServiceStyles(HtmlControl control)
		{
			if (!StyleContains("display", "none"))
				return;

			Collection<string> recipes = new Collection<string>(((LiteralControl)(control.Controls[0])).Text.Split(','));

			// New Page
			if (recipes.Contains(ServiceRecipe.NewPage.ToString()))
			{
				Holder.AddPageBreak();
			}

			// KeepingPlace
			if (recipes.Contains(ServiceRecipe.KeepingPlaceBegin.ToString()))
			{
				servicesFlags.Add(ServiceFlags.KeepingPlace);
				Holder.KeepBandsWithNext = true;
				// debug
				//Holder.PushBand();
				//Holder.AddBand();
				//Holder.AddRule().Pen = new Pen(new IgColor(Color.Red));
				//Holder.PopBand();
				// enddebug
			}
			if (recipes.Contains(ServiceRecipe.KeepingPlaceEnd.ToString()))
			{
				servicesFlags.Remove(ServiceFlags.KeepingPlace);
				// debug
				//Holder.AddRule().Pen = new Pen(new IgColor(Color.Green));
				// enddebug
				Holder.KeepBandsWithNext = false;

			}

			// GridLong
			//if (recipes.Contains(ServiceRecipe.GridLongBegin.ToString()))
			//{
			//    servicesFlags.Add(ServiceFlags.GridLong);
			//}
			//if (recipes.Contains(ServiceRecipe.GridLongEnd.ToString()))
			//{
			//    servicesFlags.Remove(ServiceFlags.GridLong);
			//}

			// GridGroups
			if (recipes.Contains(ServiceRecipe.GridGroupsBegin.ToString()))
			{
				servicesFlags.Add(ServiceFlags.GridGroups);
			}
			if (recipes.Contains(ServiceRecipe.GridGroupsEnd.ToString()))
			{
				servicesFlags.Remove(ServiceFlags.GridGroups);
			}

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
			if (currentStyle.TryGetValue("float", out value))
			{
				if (value.Equals("left"))
				{
					if (columnsIndex == 0)
					{
						Holder.TwoColumnsBegin();
					}
					else
					{
						Holder.TwoColumnsBreak();
					}
					columnsIndex++;
				}
			}
			if (currentStyle.TryGetValue("clear", out value))
			{
				Holder.TwoColumnsEnd();
				columnsIndex = 0;
				skip = true;
			}

			return !skip;
		}

		/// <summary>
		/// Применить остальные стили разметки
		/// </summary>
		protected void ApplyHtmlInnerStyles()
		{
			string value;

			// margin
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
							Holder.Margins.Top = floatValue;
							break;
						case 1:
							Holder.Margins.Right = floatValue;
							break;
						case 2:
							Holder.Margins.Bottom = floatValue;
							break;
						case 3:
							Holder.Margins.Left = floatValue;
							break;
					}
				}
			}

			// margin-left
			if (currentStyle.TryGetValue("margin-left", out value))
			{
				float floatValue;
				if (ParseCssValue(value, out floatValue))
				{
					Holder.Margins.Left = floatValue;
				}
			}

			// margin-right
			if (currentStyle.TryGetValue("margin-right", out value))
			{
				float floatValue;
				if (ParseCssValue(value, out floatValue))
				{
					Holder.Margins.Right = floatValue;
				}
			}

			// text-align
			if (currentStyle.TryGetValue("text-align", out value))
			{
				Holder.Alignment.Horizontal = ParseCssAlignment(value);
			}
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
		/// Получает нормализованное значение CSS-атрибута text-align
		/// </summary>
		private static Alignment ParseCssAlignment(string value)
		{
			switch (value)
			{
				case "center":
					return Alignment.Center;
				case "justify":
					return Alignment.Justify;
				case "right":
					return Alignment.Right;
				case "left":
					return Alignment.Left;
				default:
					return Alignment.Left;
			}
		}

		/// <summary>
		/// Получает значение CSS-атрибута font-size
		/// </summary>
		private void ParseCssFontSize(IgFont font)
		{
			string value;
			if (currentStyle.TryGetValue("font-size", out value))
			{
				float floatValue;
				if (ParseCssValue(value, out floatValue))
				{
					font.Size = floatValue;
				}
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
			public IgImage Export(UltraChart control)
			{
				// не используем RenderPdfFriendlyGraphics, потому что он косячит перевод пикселей в пункты

				MemoryStream imageStream = new MemoryStream();
				control.SaveTo(imageStream, ImageFormat.Png);
				IgImage image = (new Bitmap(imageStream)).ScaleImageIg(ScaleFactor, 72);

				return image;
			}

		}

		/// <summary>
		/// Содержит параметры экспорта таблицы и внутренние вспомогательные методы
		/// </summary>
		protected GridExporter Grid { get { return gridExporter ?? (gridExporter = new GridExporter()); } }
		private GridExporter gridExporter;
		protected class GridExporter
		{
			// TODO Размер границ ячейки надо брать из UltraGridBrick
			private const float borderSize = 2;

			private Collection<IGridRow> gridRows;

			public UltraWebGridDocumentExporter UltraWebGridDocumentExporter { set; get; }
			public GridHeaderLayout HeaderLayout { set; get; }
			public string DefaultFontName { set; get; }
			public FontUnit DefaultFontSize { set; get; }
			public Color HeaderBackgroundColor { set; get; }
			public double ScaleFontFactor { set; get; }
			public double ScaleColumnsWidthFactor { set; get; }
			public double ScalePicturesFactor { set; get; }

			protected IgBorders HeaderBorders { set; get; }

			public GridExporter()
			{
				// TODO Должно выставляться в UltraGridBrick и везде использоваться оттуда
				// сейчас в UltraGridBrick в качестве фона используется картинка этого цвета
				HeaderBackgroundColor = Color.FromArgb(unchecked((int)0xFFB2B2B2));
				HeaderBorders =
					new Borders
					{
						Left = new Border(new IgPen(new IgColor(Color.White), borderSize)),
						Top = new Border(new IgPen(new IgColor(Color.White), borderSize)),
						Right = new Border(new IgPen(new IgColor(Color.Silver), borderSize)),
						Bottom = new Border(new IgPen(new IgColor(Color.Silver), borderSize))
					};

				// как-то исторически сложилось экспортить в arial (раньше были глюки с буквой "ы")
				DefaultFontName = "Arial";

				// размера по умолчанию нет, будем брать из UltraGridBrick
				DefaultFontSize = FontUnit.Empty;

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

			#region Обработчики

			public void BeginExport(object sender, DocumentExportEventArgs e)
			{
				// TODO Должно выставляться в UltraGridBrick и везде использоваться оттуда
				e.Layout.HeaderStyleDefault.BackColor = HeaderBackgroundColor;

				// если шрифт по умолчанию не определен, возьмем его из UltraGridBrick
				if (DefaultFontSize.Type == FontSize.NotSet)
				{
					DefaultFontSize = e.Layout.RowStyleDefault.Font.Size;
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

			public void InitializeRow(object sender, DocumentExportInitializeRowEventArgs e)
			{
				for (int i = 0; i < e.Row.Cells.Count; i++)
				{
					e.Row.Cells[i].Style.Font.Name = DefaultFontName;

					// масштаб шрифта ячеек
					ScaleFont(e.Row.Cells[i].Style.Font);
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
					drawing.Scale = Convert.ToSingle(ScalePicturesFactor);
					drawing.BorderSize = borderSize;

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

				private IgImage image;
				public float BorderSize { set; get; }
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

					image = new IgImage(bitmap);
					image.Preferences.Compressor = ImageCompressors.Flate;
				}

				public void OnDraw(IGraphics graphics, float x, float y, float width, float height)
				{
					float scale = primaryScale * Scale;
					float newWidth = image.Width * scale;
					float newHeight = image.Height * scale;
					float offsetX = CRHelper.PixelsToPoints(5);
					float offsetY = (height - CRHelper.PixelsToPoints(newHeight) - BorderSize) / 2;


					IgTextureBrush textureBrush = new IgTextureBrush(image);
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
				int allColumnsCount = HeaderLayout.AllChildCount;
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
				gridRows = new Collection<IGridRow>();
				for (int i = 0; i < HeaderLayout.HeaderHeight; i++)
				{
					gridRows.Add(table.AddRow());
				}

				// добавляем ячейку селектора
				AddSelectorCell(gridRows[0]);

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
					IGridRow gridRow = gridRows[rowIndex];
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
					? HeaderLayout.HeaderHeight - rowIndex
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
				cell.Background = new Background(new IgColor(HeaderBackgroundColor));
			}

			/// <summary>
			/// Учтанавливает стиль текста ячейки хидера
			/// </summary>
			private void SetHeaderCellTextStyle(IText text)
			{
				text.Style.Font = new IgFont(DefaultFontName, (float)(DefaultFontSize.Unit.Value * ScaleFontFactor));
				text.Style.Font.Bold = true;
				text.Alignment = IgTextAlignment.Center;
			}

			/// <summary>
			/// Добавляет ячейку селектора
			/// </summary>
			private void AddSelectorCell(IGridRow row)
			{
				IGridCell cell = row.AddCell();
				cell.RowSpan = HeaderLayout.HeaderHeight;
				cell.Borders = HeaderBorders;
				cell.Background = new Background(new IgColor(HeaderBackgroundColor));
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

	/// <summary>
	/// Класс-декоратор, сливающий функционал ISection и IBand для правильного экспорта таблиц
	/// </summary>
	public class ReportHolder : ISection, IBand
	{
		public ISection CurrentSection { set; get; }
		public IBand CurrentBand { set; get; }
		private Stack<IBand> savedBands;
		private IFlow flowContent;
		public IBand clingBand;
		public IBand clingItemBand;
		private IGrid clingGrid;
		private IGridRow clingGridRow;

		public ReportHolder(Report report)
		{
			CurrentSection = report.AddSection();
			CurrentBand = CurrentSection.AddBand();
			savedBands = new Stack<IBand>();
		}

		#region магические методы и свойства

		/// <summary>
		/// Добавляет Band
		/// </summary>
		public IBand AddBand()
		{
			if (KeepBandsWithNext && clingBand == CurrentBand)
			{
				clingGridRow = clingGrid.AddRow();
				clingGridRow.KeepWithNext = true;
				CurrentBand = clingGridRow.AddCell().AddBand();
				clingItemBand = CurrentBand;
				clingItemBand.KeepSolid = true;
			}
			else
			{
				CurrentBand = CurrentBand.AddBand();
			}
			return CurrentBand;
		}

		/// <summary>
		/// Добавляет Table
		/// </summary>
		public ITable AddTable()
		{
			if (KeepBandsWithNext)
			{
				clingGridRow.KeepWithNext = false;
				clingGridRow.KeepSolid = false;
				clingItemBand.KeepSolid = false;
			}
			return CurrentBand.AddTable();
		}

		/// <summary>
		/// Включает размещение контента в 2х колонках 
		/// </summary>
		public void TwoColumnsBegin()
		{
			if (flowContent != null)
				return;
			PushBand();
			flowContent = CurrentBand.AddFlow();
			flowContent.AddColumn();
			flowContent.AddColumn();
			CurrentBand = flowContent.AddBand();
		}

		/// <summary>
		/// Выключает размещение контента в 2х колонках
		/// </summary>
		public void TwoColumnsBreak()
		{
			if (flowContent == null)
				return;

			flowContent.AddColumnBreak();
			CurrentBand = flowContent.AddBand();
		}

		/// <summary>
		/// Выключает размещение контента в 2х колонках
		/// </summary>
		public void TwoColumnsEnd()
		{
			flowContent = null;
			PopBand();
		}

		/// <summary>
		/// Родительский Band, если он существует
		/// </summary>
		public IBand ParentBand
		{
			get
			{
				if (CurrentBand.Parent is IBand)
				{
					return CurrentBand.Parent as IBand;
				}
				return null;
			}
		}

		/// <summary>
		/// Включет размещение контента, прилипающего друг к другу
		/// </summary>
		public bool KeepBandsWithNext
		{
			set
			{
				if (value)
				{
					clingBand = CurrentBand;
					clingGrid = CurrentBand.AddGrid();
					clingGrid.AddColumn();
				}
				else
				{
					clingBand = null;
					clingItemBand = null;
					clingGrid = null;
					clingGridRow = null;
				}
			}
			get { return clingGrid != null; }
		}

		/// <summary>
		/// Сохраняет текущий Band в стек
		/// </summary>
		public void PushBand()
		{
			savedBands.Push(CurrentBand);
		}

		/// <summary>
		/// Извлекает Band из стека и делает его текущим
		/// </summary>
		public void PopBand()
		{
			if (savedBands.Count > 0)
			{
				CurrentBand = savedBands.Pop();
			}
		}

		#endregion

		#region методы переадресованные на IBand

		public IFlow AddFlow()
		{
			return CurrentBand.AddFlow();
		}

		public IText AddText()
		{
			return CurrentBand.AddText();
		}

		public IImage AddImage(IgImage image)
		{
			return CurrentBand.AddImage(image);
		}

		public IMetafile AddMetafile(IgMetafile metafile)
		{
			return CurrentBand.AddMetafile(metafile);
		}

		public IRule AddRule()
		{
			return CurrentBand.AddRule();
		}

		public IGap AddGap()
		{
			return CurrentBand.AddGap();
		}

		public IGroup AddGroup()
		{
			return CurrentBand.AddGroup();
		}

		public IChain AddChain()
		{
			return CurrentBand.AddChain();
		}

		public IGrid AddGrid()
		{
			return CurrentBand.AddGrid();
		}

		public IList AddList()
		{
			return CurrentBand.AddList();
		}

		public ITree AddTree()
		{
			return CurrentBand.AddTree();
		}

		public ISite AddSite()
		{
			return CurrentBand.AddSite();
		}

		public ICanvas AddCanvas()
		{
			return CurrentBand.AddCanvas();
		}

		public IRotator AddRotator()
		{
			return CurrentBand.AddRotator();
		}

		public IContainer AddContainer(string name)
		{
			return CurrentBand.AddContainer(name);
		}

		public ICondition AddCondition(IContainer container, bool fit)
		{
			return CurrentBand.AddCondition(container, fit);
		}

		public IStretcher AddStretcher()
		{
			return CurrentBand.AddStretcher();
		}

		public void AddPageBreak()
		{
			CurrentBand.AddPageBreak();
		}

		public ITOC AddTOC()
		{
			return CurrentBand.AddTOC();
		}

		public IIndex AddIndex()
		{
			return CurrentBand.AddIndex();
		}

		public IQuickText AddQuickText(string text)
		{
			return CurrentBand.AddQuickText(text);
		}

		public IQuickImage AddQuickImage(IgImage image)
		{
			return CurrentBand.AddQuickImage(image);
		}

		public IQuickList AddQuickList()
		{
			return CurrentBand.AddQuickList();
		}

		public IQuickTable AddQuickTable()
		{
			return CurrentBand.AddQuickTable();
		}

		#endregion

		#region родные методы IBand

		public void AddDummy()
		{
			CurrentBand.AddDummy();
		}

		public IgSize Measure()
		{
			return CurrentBand.Measure();
		}

		#endregion

		#region свойства IBand

		public IgContentAlignment Alignment
		{
			get { return CurrentBand.Alignment; }
			set { CurrentBand.Alignment = value; }
		}

		public IBandHeader Header
		{
			get { return CurrentBand.Header; }
		}

		public IBandFooter Footer
		{
			get { return CurrentBand.Footer; }
		}

		public IBandDivider Divider
		{
			get { return CurrentBand.Divider; }
		}

		public Width Width
		{
			get { return CurrentBand.Width; }
			set { CurrentBand.Width = value; }
		}

		public Height Height
		{
			get { return CurrentBand.Height; }
			set { CurrentBand.Height = value; }
		}

		public Borders Borders
		{
			get { return CurrentBand.Borders; }
			set { CurrentBand.Borders = value; }
		}

		public IgMargins Margins
		{
			get { return CurrentBand.Margins; }
			set { CurrentBand.Margins = value; }
		}

		public Paddings Paddings
		{
			get { return CurrentBand.Paddings; }
			set { CurrentBand.Paddings = value; }
		}

		public Background Background
		{
			get { return CurrentBand.Background; }
			set { CurrentBand.Background = value; }
		}

		public bool KeepSolid
		{
			get { return CurrentBand.KeepSolid; }
			set { CurrentBand.KeepSolid = value; }
		}

		public bool Stretch
		{
			get { return CurrentBand.Stretch; }
			set { CurrentBand.Stretch = value; }
		}

		object IBand.Parent
		{
			get { return CurrentBand.Parent; }
		}

		#endregion

		#region методы ISection

		public ISectionHeader AddHeader()
		{
			return CurrentSection.AddHeader();
		}

		public ISectionFooter AddFooter()
		{
			return CurrentSection.AddFooter();
		}

		public IStationery AddStationery()
		{
			return CurrentSection.AddStationery();
		}

		public IDecoration AddDecoration()
		{
			return CurrentSection.AddDecoration();
		}

		public ISectionPage AddPage()
		{
			return CurrentSection.AddPage();
		}

		public ISectionPage AddPage(PageSize size)
		{
			return CurrentSection.AddPage(size);
		}

		public ISectionPage AddPage(float width, float height)
		{
			return CurrentSection.AddPage(width, height);
		}

		public ISegment AddSegment()
		{
			return CurrentSection.AddSegment();
		}

		#endregion

		#region свойства ISection

		public bool Flip
		{
			get { return CurrentSection.Flip; }
			set { CurrentSection.Flip = value; }
		}

		public PageSize PageSize
		{
			get { return CurrentSection.PageSize; }
			set { CurrentSection.PageSize = value; }
		}

		public PageOrientation PageOrientation
		{
			get { return CurrentSection.PageOrientation; }
			set { CurrentSection.PageOrientation = value; }
		}

		public IgContentAlignment PageAlignment
		{
			get { return CurrentSection.PageAlignment; }
			set { CurrentSection.PageAlignment = value; }
		}

		public Borders PageBorders
		{
			get { return CurrentSection.PageBorders; }
			set { CurrentSection.PageBorders = value; }
		}

		public IgMargins PageMargins
		{
			get { return CurrentSection.PageMargins; }
			set { CurrentSection.PageMargins = value; }
		}

		public Paddings PagePaddings
		{
			get { return CurrentSection.PagePaddings; }
			set { CurrentSection.PagePaddings = value; }
		}

		public Background PageBackground
		{
			get { return CurrentSection.PageBackground; }
			set { CurrentSection.PageBackground = value; }
		}

		public PageNumbering PageNumbering
		{
			get { return CurrentSection.PageNumbering; }
			set { CurrentSection.PageNumbering = value; }
		}

		public SectionLineNumbering LineNumbering
		{
			get { return CurrentSection.LineNumbering; }
			set { CurrentSection.LineNumbering = value; }
		}

		public Report Parent
		{
			get { return CurrentSection.Parent; }
		}

		public IEnumerable Content
		{
			get { return CurrentSection.Content; }
		}

		#endregion
	}

	/// <summary>
	/// Класс-декоратор
	/// </summary>
	public class TableHolder : ITable
	{
		private ITable currentTable;

		public TableHolder(IBand band)
		{
			currentTable = band.AddTable();
		}

		public TableHolder(ITableCell cell)
		{
			currentTable = cell.AddTable();
		}

		#region Методы ITable

		public ITableRow AddRow()
		{
			return currentTable.AddRow();
		}

		public void ApplyPattern(TablePattern pattern)
		{
			currentTable.ApplyPattern(pattern);
		}

		public IgSize Measure()
		{
			return currentTable.Measure();
		}

		#endregion

		#region Свойства ITable

		public ITableHeader Header
		{
			get { return currentTable.Header; }
		}

		public ITableFooter Footer
		{
			get { return currentTable.Footer; }
		}

		public ITableDivider Divider
		{
			get { return currentTable.Divider; }
		}

		public Width Width
		{
			get { return currentTable.Width; }
			set { currentTable.Width = value; }
		}

		public Height Height
		{
			get { return currentTable.Height; }
			set { currentTable.Height = value; }
		}

		public Borders Borders
		{
			get { return currentTable.Borders; }
			set { currentTable.Borders = value; }
		}

		public IgMargins Margins
		{
			get { return currentTable.Margins; }
			set { currentTable.Margins = value; }
		}

		public Paddings Paddings
		{
			get { return currentTable.Paddings; }
			set { currentTable.Paddings = value; }
		}

		public Background Background
		{
			get { return currentTable.Background; }
			set { currentTable.Background = value; }
		}

		public bool KeepSolid
		{
			get { return currentTable.KeepSolid; }
			set { currentTable.KeepSolid = value; }
		}

		public bool Stretch
		{
			get { return currentTable.Stretch; }
			set { currentTable.Stretch = value; }
		}

		public object Parent
		{
			get { return currentTable.Parent; }
		}

		#endregion
	}
}
