using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Infragistics.Documents.Reports.Report.Section;
using IgImage = Infragistics.Documents.Reports.Graphics.Image;

namespace Krista.FM.Server.Dashboards.Common.Export
{
	/// <summary>
	/// Экспортирует диаграмму в PDF
	/// </summary>
	public class ExportChartPdf : IExportablePdf
	{
		public ExportChartParams Params { set; get; }

		public ISection Section { set; get; }

		/// <summary>
		/// Создает новый экземпляр класса, экспортирующий диаграмму в PDF
		/// </summary>
		public ExportChartPdf(ExportChartParams paramsChart)
		{
			Params = paramsChart;
		}

		/// <summary>
		/// Экспортирует диаграмму в PDF
		/// </summary>
		public void Export()
		{
			// не используем RenderPdfFriendlyGraphics, потому что он косячит перевод пикселей в пункты
				
			MemoryStream imageStream = new MemoryStream();
			Params.Chart.SaveTo(imageStream, ImageFormat.Png);
			IgImage image = (new Bitmap(imageStream)).ScaleImageIg(Params.ScaleFactor);

			Params.ResultWidth = CRHelper.PixelsToPoints(image.Width + Params.Margins.Left + Params.Margins.Right);

			Section.AddImage(image);
		}

	}
}
