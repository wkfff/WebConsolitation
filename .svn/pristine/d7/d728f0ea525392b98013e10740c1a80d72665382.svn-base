using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Web.UI;
using Krista.FM.Server.Dashboards.Common.Export;

namespace Krista.FM.Server.Dashboards.Components.Components.Exporters
{
	/// <summary>
	/// Абстрактный строитель экспорта в файл
	/// </summary>
	public abstract partial class ExporterBase : UserControl, IExporterBuilder
	{

		protected virtual void Page_Load(object sender, EventArgs e)
		{
			// общее поведение всех строителей
			if (Session["PrintVersion"] != null && (bool)Session["PrintVersion"])
			{
				Visible = false;
			}
		}

		public string FileName { set; get; }
		public string FileDescr { set; get; }

		protected ExporterBase()
		{
			FileName = "report"; 
			FileDescr = String.Empty;
		}

		#region Методы нижнего уровня, реализуемые в потомках

		protected abstract string GetContentType();
		protected abstract string GetFileExt();
		protected abstract MemoryStream PublishReportToStream();

		protected abstract void AddGroupHeader(IHeadered header);
		protected abstract void AddSubItems(Collection<ParamsBase> items);
		protected abstract void AddSeriesItems(ExportSeries series);
		protected abstract void SetPageSettings(ExportGroup group);

		#endregion

		#region Методы верхнего уровня
		
		protected ExportHeader Header { set; get; }
		public void AddHeader(ExportHeader header)
		{
			Header = header;
		}

		public virtual void AddGroup(ExportGroup group)
		{
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

			// экспорт подэлементов
			AddSubItems(group.Items);

			// настройки страницы
			SetPageSettings(group);
		}
		
		#endregion

		/// <summary>
		/// Публикует и отправляет отчет пользователю
		/// </summary>
		public void PublishReport()
		{
			MemoryStream reportStream = PublishReportToStream();
			reportStream.Position = 0;

			// установка свойств ответа
			Page.Response.Clear();
			Page.Response.ClearHeaders();
			Page.Response.Buffer = true;
			Page.Response.ContentType = GetContentType();
			Page.Response.CacheControl = "public";
			Page.Response.AddHeader("Pragma", "public");
			Page.Response.AddHeader("Expires", "0");
			Page.Response.AddHeader("Cache-Control", "must-revalidate, post-check=0, pre-check=0");
			Page.Response.AddHeader("Content-Description", FileDescr);
			Page.Response.AddHeader("Content-Disposition", String.Format("attachment; filename={0}.{1}", FileName, GetFileExt()));
			Page.Response.AddHeader("Content-Length", reportStream.Length.ToString());

			// отправка ответа целиком
			// page.Response.OutputStream.Write(stream.ToArray(), 0, (int)stream.Length);

			// отправка ответа по частям
			const int bytesToRead = 50 * 1024;
			byte[] buffer = new Byte[bytesToRead];
			int length;
			do
			{
				if (Page.Response.IsClientConnected)
				{
					length = reportStream.Read(buffer, 0, bytesToRead);
					Page.Response.OutputStream.Write(buffer, 0, length);
					Page.Response.Flush();
					buffer = new Byte[bytesToRead];
				}
				else
				{
					length = -1;
				}
			} while (length > 0);

			Page.Response.End();
		}

	}
}