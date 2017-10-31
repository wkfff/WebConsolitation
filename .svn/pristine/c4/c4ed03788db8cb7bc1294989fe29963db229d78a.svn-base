using System.Collections.ObjectModel;
using System.Drawing;

namespace Krista.FM.Server.Dashboards.Common.Export
{
	/// <summary>
	/// Управляющий экспортом
	/// </summary>
	public class ExportDirector
	{
		private Collection<ParamsBase> Items { set; get; }

		/// <summary>
		/// Инстанцирует класс, управляющий экспортом
		/// </summary>
		public ExportDirector()
		{
			Items = new Collection<ParamsBase>();
		}

		/// <summary>
		/// Формирует и публикует отчет, используя конкретный компонент экспорта
		/// </summary>
		public void Publish(IExporterBuilder builder)
		{
			foreach (ParamsBase item in Items)
			{
				if (item is ExportHeader)
				{
					builder.AddHeader(item as ExportHeader);
				}
				else if (item is ExportGroup)
				{
					builder.AddGroup(item as ExportGroup);
				}
			}
			
			builder.PublishReport();
		}

		#region Добавление элементов

		/// <summary>
		/// Добавляет хидер к экспортируемому отчету
		/// </summary>
		/// <param name="headerText">текст хидера</param>
		public ExportDirector AddHeader(string headerText)
		{
			return AddHeader(new ExportHeader(headerText));
		}

		/// <summary>
		/// Добавляет хидер к экспортируемому отчету
		/// </summary>
		/// <param name="headerText">текст хидера</param>
		/// <param name="font">шрифт текста хидера</param>
		public ExportDirector AddHeader(string headerText, Font font)
		{
			return AddHeader(new ExportHeader(headerText, font));
		}

		/// <summary>
		/// Добавляет колонтитул к экспортируемому отчету
		/// </summary>
		public ExportDirector AddHeader(ExportHeader header)
		{
			Items.Add(header);
			return this;
		}

		/// <summary>
		/// Добавляет группу элементов к экспортируемому отчету
		/// </summary>
		public virtual ExportDirector AddGroup(ExportGroup group)
		{
			Items.Add(group);
			return this;
		}

		#endregion

		#region Создание элементов
		
		/// <summary>
		/// Создает пустую группу элементов
		/// </summary>
		public ExportGroup CreateGroup()
		{
			return new ExportGroup();
		}

		/// <summary>
		/// Создает пустую группу элементов
		/// </summary>
		/// <param name="title">Заголовок группы</param>
		public ExportGroup CreateGroup(string title)
		{
			ExportGroup group = CreateGroup();
			group.Title = title;
			return group;
		}

		/// <summary>
		/// Создает пустую группу элементов
		/// </summary>
		/// <param name="title">Заголовок группы</param>
		/// <param name="font">Шрифт заголовка группы</param>
		public ExportGroup CreateGroup(string title, Font font)
		{
			ExportGroup group = CreateGroup(title);
			group.Font = font;
			return group;
		}

		/// <summary>
		/// Создает пустую строковую группу элементов
		/// </summary>
		public ExportSeries CreateSeries()
		{
			return new ExportSeries();
		}

		#endregion
	}
}
