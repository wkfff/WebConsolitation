using System.Drawing;
using System.Web.UI.WebControls;

namespace Krista.FM.Server.Dashboards.Common.Export
{
	/// <summary>
	/// Инкапсулирует параметры экспорта таблицы
	/// </summary>
	public class ExportGridParams : ParamsMargins
	{
		/// <summary>
		/// Таблица
		/// </summary>
		public GridHeaderLayout HeaderLayout { set; get; }
		
		public string DefaultFontName { set; get; }
		public FontUnit DefaultFontSize { set; get; }
		public Color HeaderBackgroundColor { set; get; }
		
		public double ScaleFontFactor { set; get; }
		public double ScaleColumnsWidthFactor { set; get; }
		public double ScalePicturesFactor { set; get; }


		/// <summary>
		/// Инстанцирует класс параметров экспорта таблицы
		/// </summary>
		public ExportGridParams(GridHeaderLayout headerLayout)
		{
			HeaderLayout = headerLayout;
			
			// как-то исторически сложилось экспортить в arial (раньше были глюки с буквой "ы")
			DefaultFontName = "Arial";

			// размера по умолчанию нет, будем брать из UltraGridBrick
			DefaultFontSize = FontUnit.Empty;

			// TODO Должно выставляться в UltraGridBrick и использоваться оттуда
			// сейчас в UltraGridBrick в качестве фона используется картинка этого цвета
			HeaderBackgroundColor = Color.FromArgb(unchecked((int)0xFFB2B2B2));
			
			// По умолчанию ничего не масштабируем
			ScaleFontFactor = 1;
			ScaleColumnsWidthFactor = 1;
			ScalePicturesFactor = 1;
		}
		
		/// <summary>
		/// Устанавливает масштаб сразу для всех элементов
		/// </summary>
		public double ScaleFactor
		{
			set
			{
				ScaleFontFactor = value;
				ScaleColumnsWidthFactor = value;
				ScalePicturesFactor = value;
			}
		}

	}
}
