using System.Data;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Components;

namespace Krista.FM.Server.Dashboards.Common
{
	/// <summary>
	/// Оборачивает UltraGridBrick для расширения функционала в классах-потомках
	/// </summary>
	public abstract class UltraGridWrapper
	{
		/// <summary>
		/// Ссылка на оборачиваемый UltraGridBrick
		/// </summary>
		public UltraGridBrick GridBrick { private set; get; }

		/// <summary>
		/// Банды данных в UltraGridBrick
		/// </summary>
		protected UltraGridBand Band { private set; get; }
		
		/// <summary>
		/// Количество удаляемых колонок в начале таблицы
		/// </summary>
		public int DeleteColumns { set; get; }

		/// <summary>
		/// Количество скрываемых колонок в конце таблицы
		/// </summary>
		public int HiddenColumns { set; get; }

		/// <summary>
		/// Идентификатор запроса для чтения данных
		/// </summary>
		public string QueryID { set; get; }
		
		/// <summary>
		/// Таблица для сохранения результатов запроса
		/// </summary>
		public DataTable Table { set; get; }


		/// <summary>
		/// Инстанцирует класс, расширяющий функционал UltraGridBrick
		/// </summary>
		protected UltraGridWrapper(UltraGridBrick gridBrick)
		{
			GridBrick = gridBrick;

			DeleteColumns = 0;
			HiddenColumns = 0;
			
			GridBrick.Grid.InitializeLayout += InitializeLayout;
			GridBrick.Grid.InitializeRow += InitializeRow;
			
		}

		/// <summary>
		/// Установка стиля и заполнение данными
		/// </summary>
		public void SetStyleAndData()
		{
			SetStyle();
			SetData();
		}

		/// <summary>
		/// Устанавливает стиль таблицы
		/// </summary>
		protected abstract void SetStyle();

		/// <summary>
		/// Выполняет запрос, заполняет грид данными
		/// </summary>
		protected abstract void SetData();

		/// <summary>
		/// Инициализация строки грида
		/// </summary>
		protected virtual void InitializeRow(object sender, RowEventArgs e)
		{
			// empty
		}

		/// <summary>
		/// Инициализация Layout'а, настройка стиля данных, правил и заголовка грида
		/// </summary>
		protected virtual void InitializeLayout(object sender, LayoutEventArgs e)
		{
			if (e.Layout.Bands[0].Columns.Count == 0)
			{
				return;
			}
			
			Band = e.Layout.Bands[0];
			Band.HideColumns(HiddenColumns);

			SetDataStyle();
			SetDataRules();
			SetDataHeader();
		}

		/// <summary>
		/// Устанавливает стиль данных грида
		/// </summary>
		protected abstract void SetDataStyle();
		
		/// <summary>
		/// Устанавливает применяемые правила для грида
		/// </summary>
		protected abstract void SetDataRules();

		/// <summary>
		/// Устанавливает хидер грида
		/// </summary>
		protected abstract void SetDataHeader();


	}

}
