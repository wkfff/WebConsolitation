using System;
using System.Collections.ObjectModel;

namespace Krista.FM.Server.Dashboards.Common.Export
{
	/// <summary>
	/// Представляет группу элементов отчета, расположенных в строку
	/// </summary>
	public class ExportSeries : ParamsMargins
	{
		/// <summary>
		/// Элементы группы
		/// </summary>
		public Collection<ParamsBase> Items { private set; get; }

		/// <summary>
		/// Инстанцирует класс группы элементов отчета, расположенных в строку
		/// </summary>
		public ExportSeries()
		{
			Items = new Collection<ParamsBase>();
		}
		
		/// <summary>
		/// Добавляет таблицу в строковую группу
		/// </summary>
		public ExportSeries AddGrid(ExportGridParams item)
		{
			Items.Add(item);
			return this;
		}

		/// <summary>
		/// Добавляет диаграмму в строковую группу
		/// </summary>
		public ExportSeries AddChart(ExportChartParams item)
		{
			Items.Add(item);
			return this;
		}

		#region SetMargins

		/// <summary>
		/// Задает поля элемента
		/// </summary>
		public new ExportSeries SetMargins(ItemMargins margins)
		{
			base.SetMargins(margins);
			return this;
		}

		/// <summary>
		/// Задает поля элемента
		/// </summary>
		public new ExportSeries SetMargins(double all)
		{
			Margins.All = all;
			return this;
		}

		/// <summary>
		/// Задает поля элемента
		/// </summary>
		public new ExportSeries SetMargins(double top, double right, double bottom, double left)
		{
			Margins.SetMargins(top, right, bottom, left);
			return this;
		}

		#endregion
	}
	
}
