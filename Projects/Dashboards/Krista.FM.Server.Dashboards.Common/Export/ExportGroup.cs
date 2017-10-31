using System.Collections.ObjectModel;
using System.Drawing;
using Infragistics.WebUI.UltraWebChart;

namespace Krista.FM.Server.Dashboards.Common.Export
{
	/// <summary>
	/// Представляет группу элементов отчета
	/// </summary>
	public class ExportGroup : ParamsMargins, IHeadered
	{
		public Collection<ParamsBase> Items { private set; get; }
		public ExportGroupOrientation Orientation { set; get; }
		public double Width { set; get; }
		public Font Font { set; get; }

		private ExportChartParams defChartParams;
		private ExportGridParams defGridParams;

		/// <summary>
		/// Инстанцирует класс группы элементов отчета
		/// </summary>
		public ExportGroup()
		{
			Items = new Collection<ParamsBase>();
			Font = new Font("Verdana", 12, FontStyle.Bold);
			Orientation = ExportGroupOrientation.Landscape;
			Margins = new ItemMargins(10, 10, 10, 20);
			Width = 0;
		}

		/// <summary>
		/// Устанавливает ориентацию страницы для группы
		/// </summary>
		public ExportGroup SetOrientation(ExportGroupOrientation orientation)
		{
			Orientation = orientation;
			return this;
		}
		
		/// <summary>
		/// Задает ширину страницы для группы (в пикселях)
		/// </summary>
		public ExportGroup SetWidth(double width)
		{
			Width = CRHelper.PixelsToPoints(width);
			return this;
		}

		#region SetMargins

		/// <summary>
		/// Задает поля страницы для группы (в миллиметрах)
		/// </summary>
		public new ExportGroup SetMargins(ItemMargins margins)
		{
			Margins = margins;
			return this;
		}

		/// <summary>
		/// Задает поля страницы для группы (в миллиметрах)
		/// </summary>
		public new ExportGroup SetMargins(double all)
		{
			Margins.All = all;
			return this;
		}

		/// <summary>
		/// Задает поля страницы для группы (в миллиметрах)
		/// </summary>
		public new ExportGroup SetMargins(double top, double right, double bottom, double left)
		{
			Margins.SetMargins(top, right, bottom, left);
			return this;
		}

		#endregion

		#region Добавление строковой группы в группу

		/// <summary>
		/// Добавляет однострочную серию элементов к экспортируемому отчету
		/// </summary>
		/// <param name="series">Серия элементов</param>
		public ExportGroup AddSeries(ExportSeries series)
		{
			Items.Add(series);
			return this;
		}

		#endregion

		#region Добавление таблицы в группу

		/// <summary>
		/// Добавляет таблицу в группу
		/// </summary>
		/// <param name="gridParams">Параметры экспорта грида</param>
		public ExportGroup AddGrid(ExportGridParams gridParams)
		{
			Items.Add(gridParams);
			return this;
		}

		/// <summary>
		/// Добавляет таблицу в группу, используя параметры по умолчанию
		/// </summary>
		/// <param name="headerLayout">Грид</param>
		public ExportGroup AddGrid(GridHeaderLayout headerLayout)
		{
			if (defGridParams == null)
			{
				defGridParams = new ExportGridParams(null);
			}
			return AddGrid(defGridParams, headerLayout);
		}

		/// <summary>
		/// Добавляет таблицу в группу
		/// </summary>
		/// <param name="gridParams">Параметры экспорта грида</param>
		/// <param name="headerLayout">Грид. Если не указан, берется из свойства HeaderLayout параметров</param>
		public ExportGroup AddGrid(ExportGridParams gridParams, GridHeaderLayout headerLayout)
		{
			gridParams.HeaderLayout = headerLayout;
			return AddGrid(gridParams);
		}
		
		#endregion

		#region Добавление диаграммы в группу

		/// <summary>
		/// Добавляет диаграмму в группу
		/// </summary>
		/// <param name="chartParams">Параметры экспорта диаграммы</param> 
		public ExportGroup AddChart(ExportChartParams chartParams)
		{
			Items.Add(chartParams);
			return this;
		}

		/// <summary>
		/// Добавляет диаграмму в группу, используя параметры по умолчанию
		/// </summary>
		/// /// <param name="chart">Диаграмма</param>
		public ExportGroup AddChart(UltraChart chart)
		{
			if (defChartParams == null)
			{
				defChartParams = new ExportChartParams(null);
			}
			return AddChart(defChartParams, chart);
		}

		/// <summary>
		/// Добавляет диаграмму в группу
		/// </summary>
		/// <param name="chartParams">Параметры экспорта диаграммы</param> 
		/// /// <param name="chart">Диаграмма. Если не указана, берется из свойства Chart параметров</param>
		public ExportGroup AddChart(ExportChartParams chartParams, UltraChart chart)
		{
			chartParams.Chart = chart;
			return AddChart(chartParams);
		}

		#endregion
	
	}


	public enum ExportGroupOrientation
	{
		Landscape,
		Portrait
	}
}
