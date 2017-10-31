using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Image = System.Drawing.Image;

namespace Krista.FM.Server.Dashboards.Common
{
	/// <summary>
	/// Класс расширений
	/// </summary>
	public static class Extensions
	{
		#region Работа с таблицами

		/// <summary>
		/// Инвертирует порядок строк в таблице
		/// </summary>
		public static void InvertRowsOrder(this DataTable table)
		{
			for (int i = table.Rows.Count - 1; i >= 0; i--)
			{
				table.ImportRow(table.Rows[i]);
				table.Rows.RemoveAt(i);
			}
		}

		/// <summary>
		/// Возвращает объект ячейки, расположенный на пересечении строки с указанным текстом в нулевой ячейке и заданного по номеру столбца
		/// </summary>
		public static object FindValue(this DataTable table, string pattern, int index)
		{
			DataRow[] rows = table.Select(String.Format("{0} = '{1}'", table.Columns[0].ColumnName, pattern));
			if (rows.Length == 0)
				return null;
			if (rows[0].ItemArray.Length <= index)
				return null;
			return rows[0].ItemArray[index];
		}

		/// <summary>
		/// Удаляет столбцы в начале таблицы
		/// </summary>
		public static void DeleteColumns(this DataTable table, int count)
		{
			for (int i = 0; i < count; i++)
			{
				table.Columns.RemoveAt(0);
			}
		}
		
		/// <summary>
		/// Скрывает столбцы в конце таблицы
		/// </summary>
		public static void HideColumns(this UltraGridBand band, int count)
		{
			int columnsCount = band.Columns.Count;
			for (int i = 0; i < count; i++)
			{
				band.Columns[columnsCount - 1].Hidden = true;
				columnsCount--;
			}
		}

		/// <summary>
		/// Масштабирует ширину столбцов
		/// </summary>
		public static void ScaleColumnsWidth(this UltraGridBand band, double scale)
		{
			foreach (UltraGridColumn column in band.Columns)
			{
				if (!column.Width.IsEmpty && column.Width.Value > 0)
				{
					column.Width = new Unit(column.Width.Value * scale);
				}
			}
		}
		
		#endregion

		#region Работа со строками

		/// <summary>
		/// Возвращает строку с заглавной буквы
		/// </summary>
		public static string ToUpperFirstSymbol(this string source)
		{
			return source == string.Empty
					   ?
				   string.Empty
					   :
				   source[0].ToString().ToUpper() + source.Remove(0, 1);
		}

		/// <summary>
		/// Возвращает строку со строчной буквы
		/// </summary>
		public static string ToLowerFirstSymbol(this string source)
		{
			return source == string.Empty
					   ?
				   string.Empty
					   :
				   source[0].ToString().ToLower() + source.Remove(0, 1);
		}

		/// <summary>
		/// Возвращает строковый массив, содержащий подстроки данной строки, разделенные заданной строкой
		/// </summary>
		public static string[] Split(this string source, string separator)
		{
			return source.Split(new string[] { separator }, StringSplitOptions.None);
		}

		/// <summary>
		/// Возвращает строковый массив, содержащий подстроки данной строки, разделенные заданной строкой.
		/// Параметр указывает, следует ли возвращать пустые элементы массива
		/// </summary>
		public static string[] Split(this string source, string separator, StringSplitOptions options)
		{
			return source.Split(new string[] {separator}, options);
		}

		/// <summary>
		/// Указывает, пуста ли строка (равна null, String.Empty или "")
		/// </summary>
		public static bool IsEmpty(this string source)
		{
			return source == null || source.Equals(String.Empty) || source.Equals("");
		}

		#endregion

		#region Работа с картинками

		/// <summary>
		/// Масштабирует картинку
		/// </summary>
		public static Image ScaleImage(this Image sourceImage, double scale, int dpi = 72)
		{
			Bitmap bmp = new Bitmap(
				Convert.ToInt32(Math.Round(CRHelper.PixelsToPoints(sourceImage.Width * scale))),
				Convert.ToInt32(Math.Round(CRHelper.PixelsToPoints(sourceImage.Height * scale))));

			bmp.SetResolution(dpi, dpi);

			using (Graphics graphics = Graphics.FromImage(bmp))
			{
				graphics.CompositingQuality = CompositingQuality.HighQuality;
				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics.SmoothingMode = SmoothingMode.HighQuality;

				graphics.DrawImage(sourceImage, 0, 0, bmp.Width, bmp.Height);
			}
			
			return bmp;
		}

		/// <summary>
		/// Масштабирует картинку и преобразует ее в инфрагистический формат
		/// </summary>
		public static Infragistics.Documents.Reports.Graphics.Image ScaleImageIg(this Image sourceImage, double scale, int dpi = 72)
		{
			Infragistics.Documents.Reports.Graphics.Image image = 
				new Infragistics.Documents.Reports.Graphics.Image(sourceImage.ScaleImage(scale, dpi));
			image.Preferences.Compressor = Infragistics.Documents.Reports.Graphics.ImageCompressors.Flate;

			return image;
		}

		#endregion

		#region Работа с числами

		/// <summary>
		/// Сравнивает два числа double на равенство с наивысшей точностью
		/// </summary>
		public static bool EqualsTo(this double a, double b)
		{
			return Math.Abs(a - b) < Double.Epsilon;
		}

		/// <summary>
		/// Сравнивает два числа double на равенство с заданной точностью
		/// </summary>
		public static bool EqualsTo(this double a, double b, double epsilon)
		{
			return Math.Abs(a - b) < epsilon;
		}
		
		/// <summary>
		/// Сравнивает два числа decimal с заданной точностью (количество разрядов)
		/// </summary>
		public static int CompareTo(this decimal a, decimal b, int decimals)
		{
			decimal one = Decimal.Round(a, decimals);
			decimal two = Decimal.Round(b, decimals);
			return one.CompareTo(two);
		}

		/// <summary>
		/// Сравнивает два числа decimal на равенство с заданной точностью (количество разрядов)
		/// </summary>
		public static bool EqualsTo(this decimal a, decimal b, int decimals)
		{
			return a.CompareTo(b, decimals) == 0;
		}

		#endregion

	}
}