using System.Drawing;

namespace Krista.FM.Server.Dashboards.Common.Export
{
	public interface IExporterBuilder
	{
		/// <summary>
		/// Публикует документ с отчетом
		/// </summary>
		void PublishReport();
		
		/// <summary>
		/// Добавляет хидер к экспортируемому отчету
		/// </summary>
		void AddHeader(ExportHeader header);


		/// <summary>
		/// Добавляет группу элементов к экспортируемому отчету
		/// </summary>
		void AddGroup(ExportGroup group);

	}
}
