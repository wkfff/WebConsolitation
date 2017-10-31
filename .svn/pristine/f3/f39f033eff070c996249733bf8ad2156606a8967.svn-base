using System.Data;
using System.Drawing;
using Infragistics.UltraChart.Shared.Events;
using Krista.FM.Server.Dashboards.Components;

namespace Krista.FM.Server.Dashboards.Common
{
	/// <summary>
	/// Оборачивает UltraChartItem для расширения функционала в классах-потомках
	/// </summary>
	public abstract class UltraChartWrapper
	{
		#region Данные компонента

		/// <summary>
		/// Ссылка на оборачиваемый UltraChartItem
		/// </summary>
		public UltraChartItem ChartControl { private set; get; }

		public string ReportID
		{
			set { ChartControl.ReportID = value; }
			get { return ChartControl.ReportID; }
		}

		public string TemporaryUrlPrefix
		{
			set { ChartControl.TemporaryUrlPrefix = value; }
			get { return ChartControl.TemporaryUrlPrefix; }
		}

		#endregion

		#region Данные стиля

		protected Color DefaultColor
		{
			set { ChartControl.DefaultColor = value; }
			get { return ChartControl.DefaultColor; }
		}

		protected Color DefaultColorDark { set; get; }

		protected Font DefaultFont
		{
			set { ChartControl.DefaultFont = value; }
			get { return ChartControl.DefaultFont; }
		}

		protected Font DefaultFontSmall { set; get; }

		#endregion

		#region Данные для данных

		/// <summary>
		/// Идентификатор запроса для чтения данных
		/// </summary>
		public string QueryID { set; get; }

		/// <summary>
		/// Таблица для сохранения результатов запроса
		/// </summary>
		public DataTable Table { set; get; }
		
		#endregion


		/// <summary>
		/// Инстанцирует класс, расширяющий функционал UltraChartItem
		/// </summary>
		protected UltraChartWrapper(UltraChartItem chartItem)
		{
			ChartControl = chartItem;
			
			ChartControl.Chart.FillSceneGraph += FillSceneGraph;
		}

		/// <summary>
		/// Установка стиля и заполнение данными
		/// </summary>
		public virtual void SetStyleAndData()
		{
			SetStyle();
			SetData();
		}

		/// <summary>
		/// Устанавливает стиль диаграммы
		/// </summary>
		protected virtual void SetStyle()
		{
			ChartControl.SetDefaultStyle();
		}
		
		/// <summary>
		/// Выполняет запрос, заполняет диаграмму данными
		/// </summary>
		protected virtual void SetData()
		{
			// empty here
		}


		protected virtual void FillSceneGraph(object sender, FillSceneGraphEventArgs e)
		{
			// empty here
		}
		
	}

}
