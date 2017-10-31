using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Band;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Common.Export;
using ContentAlignment = Infragistics.Documents.Reports.Report.ContentAlignment;

namespace Krista.FM.Server.Dashboards.Components.Components.Exporters
{
	/// <summary>
	/// Строитель экспорта в PDF
	/// </summary>
	public partial class ExporterPdf : ExporterBase
	{
		private const double A4width = 210;
		private const double A4height = 297;
		private const double A4scale = A4width / A4height;
		

		private ReportHolderPdf Holder { set; get; }

		/// <summary>
		/// Кнопка для экспорта
		/// </summary>
		public LinkButton PdfExportButton
		{
			get { return pdfExportButton; }
		}

		/// <summary>
		/// UltraWebGridDocumentExporter, используемый для экспорта таблиц
		/// </summary>
		public UltraWebGridDocumentExporter UltraWebGridDocumentExporter
		{
			get { return ultraWebGridDocumentExporter; }
		}

		private Report Report { set; get; }
		
		/// <summary>
		/// Инстанцирует класс строителя экспорта
		/// </summary>
		public ExporterPdf()
		{
			Report = new Report();
		}
		
		#region Экспорт элементов верхнего уровня

		/// <summary>
		/// Добавляет группу элементов к экспортируемому отчету
		/// </summary>
		/// <param name="group">Группа элементов</param>
		public override void AddGroup(ExportGroup group)
		{
			Holder = new ReportHolderPdf(Report);
			Holder.PageSize = PageSizes.A4;

			// полностью переносим функционал з базового класса сюда,
			// т.к. добавление заголовков нужно делать с учетом итогового размера страницы (в Holder),
			// т.е. в самом конце

			// экспорт подэлементов, выполняется первым, т.к. таблицы меняют размеры страницы
			AddSubItems(group.Items);
			SetPageSettings(group);

			// заголовок отчета
			if (Header != null)
			{
				AddGroupHeader(Header);
			}

			// заголовок группы элементов
			if (!group.Title.Equals(String.Empty))
			{
				AddGroupHeader(group);
			}

		}

		#endregion

		#region внутренние методы
		
		/// <summary>
		/// Добавляет колонтитул
		/// </summary>
		protected override void AddGroupHeader(IHeadered header)
		{
			double scale =
				Holder.PageOrientation == PageOrientation.Portrait
					? (Holder.PageSize.Width - Holder.PageMargins.Left - Holder.PageMargins.Right) / PageSizes.A4.Width
					: (Holder.PageSize.Height - Holder.PageMargins.Left - Holder.PageMargins.Right) / PageSizes.A4.Height;

			IBandHeader bandHeader = Holder.Header;
			bandHeader.Repeat = true;

			IText iText = bandHeader.AddText();
			iText.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(header.Font);
			iText.Style.Font.Size = (float)(iText.Style.Font.Size * scale);	
			iText.AddContent(header.Title);
		}

		/// <summary>
		/// Экспортирует подэлементы
		/// </summary>
		protected override void AddSubItems(Collection<ParamsBase> items)
		{
			if (items.Count == 0)
			{
				Holder.AddDummy();
			}

			foreach (ParamsBase item in items)
			{
				Holder.PushBand();
				Holder.AddBand();

				if (item is ExportGridParams)
				{
					ExportGrid(item as ExportGridParams);
				}
				else if (item is ExportChartParams)
				{
					
					ExportChart(item as ExportChartParams);
				}
				else if (item is ExportSeries)
				{
					AddSeriesItems((item as ExportSeries));
				}

				Holder.PopBand();
			}
		}

		/// <summary>
		/// Экспортирует подэлементы расположенные в строку
		/// </summary>
		protected override void AddSeriesItems(ExportSeries series)
		{
			if (series.Items.Count == 0)
				{ return; }
			
			SetMargins(series.Margins);
			ITableRow row = Holder.AddTable().AddRow();
			
			foreach (ParamsBase item in series.Items)
			{
				ITableCell cell = row.AddCell();
				
				Holder.PushBand();
				Holder.SetBand(cell.AddBand());
				
				if (item is ExportGridParams)
				{
					ExportGrid(item as ExportGridParams);
				}
				else if (item is ExportChartParams)
				{
					ExportChart(item as ExportChartParams);
				}
				
				cell.Width = new FixedWidth((float)item.ResultWidth);

				Holder.PopBand();
			}
		}

		/// <summary>
		/// Устанавливает ориентацию раздела
		/// </summary>
		protected override void SetPageSettings(ExportGroup group)
		{
			SetPageOrientation(group.Orientation);
			ClearPageMargins();
			SetPageMargins(group.Margins);
			SetPageProportions(group.Width);
		}

		/// <summary>
		/// Экспортирует таблицу
		/// </summary>
		private void ExportGrid(ExportGridParams gridParams)
		{
			SetMargins(gridParams.Margins);
			ExportItem(new ExportGridPdf(gridParams, ultraWebGridDocumentExporter));
		}

		/// <summary>
		/// Экспортирует диаграмму
		/// </summary>
		private void ExportChart(ExportChartParams chartParams)
		{
			SetMargins(chartParams.Margins);
			ExportItem(new ExportChartPdf(chartParams));
		}

		/// <summary>
		/// Экспортирует элемент
		/// </summary>
		private void ExportItem(IExportablePdf exporter)
		{
			exporter.Section = Holder;
			exporter.Export();
		}

		/// <summary>
		/// Убирает поля со страницы
		/// </summary>
		private void ClearPageMargins()
		{
			SincPageMargins(false);
			Holder.PageMargins.All = 0;
		}
		
		/// <summary>
		/// Устанавливает поля страницы
		/// </summary>
		private void SetPageMargins(ItemMargins margins)
		{
			double dx =
				Holder.PageOrientation == PageOrientation.Portrait
					? margins.Right + margins.Left
					: margins.Top + margins.Bottom;

			double scale = Holder.PageSize.Width / (A4width - dx);
			Holder.PageMargins.Top = Convert.ToSingle(scale * margins.Top);
			Holder.PageMargins.Right = Convert.ToSingle(scale * margins.Right);
			Holder.PageMargins.Bottom = Convert.ToSingle(scale * margins.Bottom);
			Holder.PageMargins.Left = Convert.ToSingle(scale * margins.Left);

			SincPageMargins(true);
		}


		/// <summary>
		/// Синхронизирует поля и размр страницы
		/// </summary>
		/// <param name="increase">true - добавляет размер полей к размерам страницы, false - вычитает</param>
		private void SincPageMargins(bool increase)
		{
			int sign = increase ? 1 : -1;
			float dx = sign * (Holder.PageMargins.Left + Holder.PageMargins.Right);
			float dy = sign * (Holder.PageMargins.Top + Holder.PageMargins.Bottom);
			Holder.PageSize =
				Holder.PageOrientation == PageOrientation.Portrait
					? new PageSize(Holder.PageSize.Width + dx, Holder.PageSize.Height + dy)
					: new PageSize(Holder.PageSize.Width + dy, Holder.PageSize.Height + dx);
			
		}

		/// <summary>
		/// Устанавливает ориентацию раздела
		/// </summary>
		private void SetPageOrientation(ExportGroupOrientation orientation)
		{
			Holder.PageOrientation =
				orientation == ExportGroupOrientation.Portrait
					? PageOrientation.Portrait
					: PageOrientation.Landscape;
		}

		/// <summary>
		/// Устанавливает размер страницы
		/// Ширина остается неизменной (с учетом ориентации страницы), высота подгоняется под пропорции A4
		/// </summary>
		private void SetPageProportions(double newSize)
		{
			double width = Holder.PageSize.Width;
			double height = Holder.PageSize.Height;

			if (Math.Abs(newSize) > 0.00001)
			{
				width = newSize;
				height = newSize;
			}

			Holder.PageSize =
				Holder.PageOrientation == PageOrientation.Portrait
					? new PageSize((float) width, (float) (width/A4scale))
					: new PageSize((float) (height*A4scale), (float) height);
			
		}
		
		/// <summary>
		/// Устанавливает отступы для экспортируемого элемента
		/// </summary>
		private void SetMargins(ItemMargins margins)
		{
			Holder.Margins.Top = CRHelper.PixelsToPoints(margins.Top);
			Holder.Margins.Right = CRHelper.PixelsToPoints(margins.Right);
			Holder.Margins.Bottom = CRHelper.PixelsToPoints(margins.Bottom);
			Holder.Margins.Left = CRHelper.PixelsToPoints(margins.Left);
		}

		#endregion



		#region Низкоуровневые операции

		protected override string GetContentType()
		{
			return "application/pdf";
		}

		protected override string GetFileExt()
		{
			return "pdf";
		}

		protected override MemoryStream PublishReportToStream()
		{
			MemoryStream stream = new MemoryStream();
			Report.Publish(stream, FileFormat.PDF);
			return stream;
		}

		#endregion
	}

}