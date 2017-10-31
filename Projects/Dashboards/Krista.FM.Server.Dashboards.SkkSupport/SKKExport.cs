using System;
using System.IO;
using System.Web.UI;
using Infragistics.WebUI.UltraWebChart;
using Krista.FM.Server.Dashboards.Components;

namespace Krista.FM.Server.Dashboards.SkkSupport
{
	/// <summary>
	/// Служебные метки в отчете
	/// </summary>
	public enum ServiceRecipe
	{
		NewPage,
		KeepingPlaceBegin,
		KeepingPlaceEnd,
		GridLongBegin,
		GridLongEnd,
		GridGroupsBegin,
		GridGroupsEnd,
		ChartGroupsBegin,
		ChartGroupsEnd,
	}

	/// <summary>
	/// Служебные флаги
	/// </summary>
	public enum ServiceFlags
	{
		KeepingPlace,
		GridLong,
		GridGroups,
		ChartGroups,
	}

	/// <summary>
	/// Возможные варианты экспорта
	/// </summary>
	public enum ExportType
	{
		Pdf, Doc
	}

	/// <summary>
	/// Базовый помощник экспорта
	/// </summary>
	public abstract class SKKExportBase
	{
		// внутренние переменные
		private readonly ExportType exportType;
		private Page page;
		protected MemoryStream stream;


		// используемое в любом месте
		public string FileName { set; get; }
		public string FileDescr { set; get; }

		public int TextHeight { set; get; }
		public int HeaderHeight { set; get; }
		public string HeaderText { set; get; }
		public string Header1stTextLeft { set; get; }
		public string Header1stTextRight { set; get; }
		public int FooterHeight { set; get; }
		public string FooterText { set; get; }
		public string PageNumberingTemplate { set; get; }
		public int HeaderFooterTextHeight { set; get; }



		protected SKKExportBase(Page page, ExportType exportType)
		{
			this.page = page;
			this.exportType = exportType;

			stream = new MemoryStream();

			FileName = "report";
			FileDescr = String.Empty;

			HeaderFooterTextHeight = 12;
			HeaderHeight = 0;
			HeaderText = String.Empty;
			Header1stTextLeft = String.Empty;
			Header1stTextRight = String.Empty;
			FooterHeight = 0;
			FooterText = String.Empty;
			PageNumberingTemplate = "Страница [Page #] из [TotalPages]";

			TextHeight = 14;

		}

		/// <summary>
		/// Публикует отчет (реализуется в потомках)
		/// </summary>
		protected abstract void ExportContent();


		public abstract void Export();

		/// <summary>
		/// Публикует и отправляет отчет в нужном формате
		/// </summary>
		protected virtual void Publish()
		{
			string contentType;
			string fileExt;

			switch (exportType)
			{
				case ExportType.Pdf:
					contentType = "application/pdf";
					fileExt = "pdf";
					break;
				case ExportType.Doc:
					contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
					fileExt = "docx";
					break;
				default:
					return;
			}

			stream.Position = 0;

			page.Response.Clear();
			page.Response.ClearHeaders();
			page.Response.Buffer = true;
			page.Response.ContentType = contentType;
			page.Response.CacheControl = "public";
			page.Response.AddHeader("Pragma", "public");
			page.Response.AddHeader("Expires", "0");
			page.Response.AddHeader("Cache-Control", "must-revalidate, post-check=0, pre-check=0");

			page.Response.AddHeader("Content-Description", FileDescr);
			page.Response.AddHeader("Content-Disposition", String.Format("attachment; filename={0}.{1}", FileName, fileExt));
			page.Response.AddHeader("Content-Length", stream.Length.ToString());

			// отправка целиком
			// page.Response.OutputStream.Write(stream.ToArray(), 0, (int)stream.Length);

			// отправка по частям
			const int bytesToRead = 10240;
			byte[] buffer = new Byte[bytesToRead];
			int length;
			do
			{
				if (page.Response.IsClientConnected)
				{
					length = stream.Read(buffer, 0, bytesToRead);
					page.Response.OutputStream.Write(buffer, 0, length);
					page.Response.Flush();
					buffer = new Byte[bytesToRead];
				}
				else
				{
					length = -1;
				}
			} while (length > 0);

			page.Response.End();
		}

		protected abstract void ExportChartControl(UltraChart control);
		protected abstract void ExportGridControl(UltraGridBrick control);

	}
	
}
