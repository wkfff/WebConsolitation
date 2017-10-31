using System.Drawing;

namespace Krista.FM.Server.Dashboards.Common.Export
{
	/// <summary>
	/// Представляет параметры экспорта колонтитула
	/// </summary>
	public class ExportHeader : ParamsBase, IHeadered
	{
		public Font Font { set; get; }

		/// <summary>
		/// Инстанцирует класс параметров экспорта колонтитула
		/// </summary>
		public ExportHeader()
		{
			Font = new Font("Verdana", 14, FontStyle.Bold);
		}

		/// <summary>
		/// Инстанцирует класс параметров экспорта колонтитула
		/// </summary>
		public ExportHeader(string title) 
			: this()
		{
			Title = title;
		}

		/// <summary>
		/// Инстанцирует класс параметров экспорта колонтитула
		/// </summary>
		public ExportHeader(string title, Font font) 
			: this(title)
		{
			Font = font;
		}
	}
}
