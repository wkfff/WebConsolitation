using System;

namespace Krista.FM.Server.Dashboards.Common.Export
{
	/// <summary>
	/// Представляет базовый класс параметров экспорта элементов отчета
	/// </summary>
	public abstract class ParamsBase
	{
		/// <summary>
		/// Заголовок элемента
		/// </summary>
		public string Title { set; get; }

		/// <summary>
		/// Примечание элемента
		/// </summary>
		public string Note { set; get; }

		/// <summary>
		/// Результирующая ширина элемента, определяется после экспорта
		/// </summary>
		public double ResultWidth { set; get; }

		/// <summary>
		/// Инстанцирует базовый класс параметров экспорта элемента отчета
		/// </summary>
		protected ParamsBase()
		{
			Title = String.Empty;
			Note = String.Empty;
			ResultWidth = 0;
		}
	}
}
