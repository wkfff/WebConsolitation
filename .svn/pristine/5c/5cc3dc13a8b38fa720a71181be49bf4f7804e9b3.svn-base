using System;
using System.Data;
using System.IO;
using System.Text;
using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using DataTable=Microsoft.Office.Interop.Excel.DataTable;

namespace Krista.FM.Server.Forecast.ExcelAddin
{
	[Guid("7305B0B7-EA9C-4331-8AED-CE975C210BBE")]
	[ComVisible(true)]
	public interface IWorkbookOfModel : IDisposable
	{
		/// <summary>
		/// Получает имя файла
		/// </summary>
		String NameOfFile { get; }

		/// <summary>
		/// Получает объект книги Interface Microsoft.Office.Intropt.Excel
		/// </summary>
		Workbook WorkBook { get; }

		/// <summary>
		/// Открывает книгу Excel
		/// </summary>
		/// <param name="Name">Имя книги, без пути.</param>
		/// <returns></returns>
		Boolean OpenWorkbook(String Name);

		/// <summary>
		/// Закрывает книгу 
		/// </summary>
		void CloseWorkbook();

		/// <summary>
		/// Сохранить книгу 
		/// </summary>
		void SaveWorkbook();

		/// <summary>
		/// Получает данные из ячейки на пересечении поименованых строк и столбца
		/// </summary>
		/// <param name="Sheet">Название страницы</param>
		/// <param name="namerow">Имя строки</param>
		/// <param name="namecol">Имя колонки</param>
		/// <returns>Значение</returns>
		Object GetDataFromCell(String Sheet, String namerow, String namecol);

		/// <summary>
		/// Получает данные из поименованной ячейки 
		/// </summary>
		/// <param name="Sheet">Название страницы</param>
		/// <param name="nameCell">Имя ячейки</param>
		/// <returns>Значение</returns>
		Object GetDataFromCell(String Sheet, String nameCell);

		/// <summary>
		/// Получает данные из ячейки по номеру строки и букве столбца
		/// </summary>
		/// <param name="Sheet">Название страницы</param>
		/// <param name="row">Номер строки</param>
		/// <param name="namecol">Название столбца</param>
		/// <returns>Значение</returns>
		Object GetDataFromCell(String Sheet, Int32 row, String namecol);

		/// <summary>
		/// Получает данные из ячейки на пересечении поименованых строки и столбца
		/// </summary>
		/// <param name="Sheet">Объект страницы Excel.Worksheet</param>
		/// <param name="namerow">Название строки</param>
		/// <param name="namecol">Название столбца</param>
		/// <returns>Значение</returns>
		Object GetDataFromCell(Worksheet Sheet, String namerow, String namecol);

		/// <summary>
		/// Получает данные из строки
		/// </summary>
		/// <param name="Sheet">Название страницы</param>
		/// <param name="row">Номер строки</param>
		/// <param name="count">Количество ячеек читаемых из строки</param>
		/// <returns>Массив значений</returns>
		Object[,] GetDataFromRow(String Sheet, Int32 row, Int32 count);

		/// <summary>
		/// Получает данные из Worksheet на 6 лет (столбцы Est, y.1, y.2, y.3, y.4, y.5)
		/// </summary>
		/// <param name="Sheet">Объект страницы Excel.Worksheet</param>
		/// <returns>DataTable значений из 7 столбцов в последнем SIGNAT</returns>
		System.Data.DataTable GetDataFromSheetY6(Worksheet Sheet);

		/// <summary>
		/// Получает данные из Worksheet для формы 2п (столбцы Est, y.1, y.2, y.3, y.4, y.5)
		/// </summary>
		/// <param name="Sheet">Объект страницы Excel.Worksheet</param>
		/// <returns>DataTable значений из 7 столбцов в последнем SIGNAT</returns>
		System.Data.DataTable GetDataFromSheetF2p(Worksheet Sheet);
		
		/// <summary>
		/// Получает данные индикаторов из Worksheet на 6 лет 
		/// по стобцам (Est, y.1, y.2, y.3, y.4, y.5)
		/// </summary>
		/// <param name="Sheet">Объект страницы Excel.Worksheet</param>
		/// <returns>DataTable значений из 7 столбцов в последнем SIGNAT</returns>
		System.Data.DataTable GetChangedIndicatorsY6(Worksheet Sheet);

		/// <summary>
		/// Устанавливает значениев ячейке  на пересечении поименованых строки и столбца
		/// </summary>
		/// <param name="Sheet">Название страницы</param>
		/// <param name="namerow">Имя строки</param>
		/// <param name="namecol">Имя столбца</param>
		/// <param name="value">Устанавливаемое значение</param>
		void SetDataToCell(String Sheet, String namerow, String namecol, Decimal value);

		/// <summary>
		/// Устанавливает значение в ячейке  на пересечении поименованых строки и столбца
		/// </summary>
		/// <param name="Sheet">Объект страницы Excel.Worksheet</param>
		/// <param name="namerow">Имя строки</param>
		/// <param name="namecol">Имя столбца</param>
		/// <param name="value">Устанавливаемое значение</param>
		void SetDataToCell(Worksheet Sheet, String namerow, String namecol, Decimal value);

		/// <summary>
		/// Устанавливает значение в 6 ячейках поименованых строки в столбцах
		/// с наименованиями estimate, y.1, y.2, y.3, y.4, y.5 
		/// </summary>
		/// <param name="Sheet">Объект страницы Excel.Worksheet</param>
		/// <param name="namerow">Имя строки</param>
		void SetDataToCellY6(Worksheet Sheet, String namerow, Decimal estValue, Decimal Y1Value, Decimal Y2Value, Decimal Y3Value, Decimal Y4Value, Decimal Y5Value);

		/// <summary>
		/// Устанавливает значение в 5 ячейках поименованых строки в столбцах
		/// с наименованиями y.1, y.2, y.3, y.4, y.5 
		/// </summary>
		/// <param name="Sheet">Объект страницы Excel.Worksheet</param>
		/// <param name="namerow">Имя строки</param>
		void SetDataToCellY5(Worksheet Sheet, String namerow, Decimal Y1Value, Decimal Y2Value, Decimal Y3Value, Decimal Y4Value, Decimal Y5Value);

		/// <summary>
		/// Устанавливает значение в 6 ячейках поименованых строки в столбцах
		/// с наименованиями estimate, y.1, y.2, y.3, y.4, y.5 
		/// </summary>
		/// <param name="Sheet">Объект страницы Excel.Worksheet</param>
		/// <param name="dtValues">Таблица с данными 7 столбцов, в последнем сигнатура параметра</param>
		void SetDataToCellY6(Worksheet Sheet, System.Data.DataTable dtValues);
		
		/// <summary>
		/// Устанавливает значение в 6 ячейках поименованых строки в столбцах
		/// с наименованиями estimate, y.1, y.2, y.3, y.4, y.5 
		/// </summary>
		/// <param name="Sheet">Объект страницы Excel.Worksheet</param>
		/// <param name="dtValues">Таблица с данными 7 столбцов, в последнем сигнатура параметра</param>
		void SetDataToCellY6_2(Worksheet Sheet, System.Data.DataTable dtValues);

		/// <summary>
		/// Устанавливает значение в 2 ячейках поименованых строки в столбцах
		/// с наименованиями base, estimate
		/// </summary>
		/// <param name="Sheet">Объект страницы Excel.Worksheet</param>
		/// <param name="dtValues">Таблица с данными 3 столбца, в последнем сигнатура параметра</param>
		void SetDataToCellY2_2(Worksheet Sheet, System.Data.DataTable dtValues);

		/// <summary>
		/// Устанавливает значение в 2 ячейках поименованых строки в столбцах
		/// с наименованиями base, estimate согласно маске в столбце MASK
		/// </summary>
		/// <param name="Sheet">Объект страницы Excel.Worksheet</param>
		/// <param name="dtValues">Таблица с данными 4 столбца, в предпоследнем сигнатура параметра,
		/// последнем битовая маска </param>
		void SetDataToCellY2_2_Masked(Worksheet Sheet, System.Data.DataTable dtValues);

		/// <summary>
		/// Устанавливает значение в 2 ячейках поименованых строки в столбцах
		/// в форме2п согласно данных в столбце SIGNAT
		/// </summary>
		/// <param name="Sheet">Объект страницы Excel.Worksheet</param>
		/// <param name="dtValues">Таблица с данными 3 столбца, в последнем сигнатура параметра</param>
		void SetDataToCellForm2p_Masked(Worksheet Sheet, System.Data.DataTable dtValues);

		/// <summary>
		/// Устанавливает значение в 8 ячейках поименованых строки в столбцах
		/// в форме2п согласно данных в столбце SIGNAT для вывода формы 2п в формате МЭР
		/// Работает отдельно от модели.
		/// </summary>
		/// <param name="Sheet">Объект страницы Excel.Worksheet</param>
		/// <param name="dtValues">Таблица с данными 9 столбцов, в последнем сигнатура параметра</param>
		void SetDataToForm2p(Worksheet Sheet, System.Data.DataTable dtValues);
	}

	/// <summary>
	/// Класс описывает отдельновзятую книгу модели. Используется в классе
	/// ExModel для в качестве праметра структуры List<T>
	/// </summary>
	[Guid("3CAC1C84-AB80-4fe8-9361-D383FD6B1756")]
	[ComVisible(true)]
	public class WorkbookOfModel : IWorkbookOfModel
	{
		private readonly String basePath; //путь к xls файлам модели
		private Microsoft.Office.Interop.Excel.Application excelApp; //Хранит ссылку на объект приложение
		private String nameOfFile; //Название файла к которому привязан данный класс
		private Workbook workBook; //собственно объект книги
		
		/// <summary>
		///  Конструктор класса одной книги модели.
		/// </summary>
		/// <param name="app">Объект приложение Excel</param>
		/// <param name="modelPath">Путь к файлам модели</param>
		public WorkbookOfModel(Microsoft.Office.Interop.Excel.Application app, String modelPath)
		{
			basePath = modelPath;
			excelApp = app;
		}
		
		#region Свойства
		/// <summary>
		/// Получает имя файла
		/// </summary>
		public String NameOfFile
		{
			get { return nameOfFile; }
		}

		/// <summary>
		/// Получает объект книги
		/// </summary>
		public Workbook WorkBook
		{
			get { return workBook; }
		}
		#endregion
		
		/// <summary>
		/// Открывает книгу Excel
		/// </summary>
		/// <param name="Name">Имя книги, без пути.</param>
		/// <returns></returns>
		public Boolean OpenWorkbook(String Name)
		{
			nameOfFile = Name;
			String S = Path.Combine(basePath, Name);
			try
			{
				workBook = excelApp.Workbooks.Open(S, XlUpdateLinks.xlUpdateLinksAlways, Type.Missing, Type.Missing,
					Type.Missing, Type.Missing, Type.Missing, Type.Missing,
					Type.Missing, Type.Missing, Type.Missing, Type.Missing,
					Type.Missing, Type.Missing, Type.Missing);
			}
			catch (Exception e)
			{
				throw new Exception("Ошибка при открытия файла модели: "+S, e);
			}
			if (workBook == null) return false;
			else return true;
		}

		/// <summary>
		/// Закрывает книгу 
		/// </summary>
		public void CloseWorkbook()
		{
			if (workBook != null)
				workBook.Close(false, null, false);
		}

		/// <summary>
		/// Сохранить книгу 
		/// </summary>
		public void SaveWorkbook()
		{
			if (workBook != null)
				workBook.Save();
		}

		/// <summary>
		/// Получает данные из ячейки на пересечении поименованых строк и столбца
		/// </summary>
		/// <param name="Sheet">Название страницы</param>
		/// <param name="namerow">Имя строки</param>
		/// <param name="namecol">Имя колонки</param>
		/// <returns>Значение</returns>
		public Object GetDataFromCell(String Sheet, String namerow, String namecol)
		{
			Range excelcells;
			Worksheet tmp = (Worksheet)workBook.Worksheets.get_Item(Sheet);
			try
			{
				excelcells = tmp.get_Range(namerow + " " + namecol, Type.Missing);
				return excelcells.Value2;
			}
			catch (System.Runtime.InteropServices.COMException e)
			{
				String[] s = e.Message.Split(' ');
				if ((s.Length > 0) && (s[s.Length - 1] == "0x800A03EC"))
					return null;
				else
					throw new Exception(e.Message, e);
			}
		}

		/// <summary>
		/// Получает данные из поименованной ячейки 
		/// </summary>
		/// <param name="Sheet">Название страницы</param>
		/// <param name="nameCell">Имя ячейки</param>
		/// <returns>Значение</returns>
		public Object GetDataFromCell(String Sheet, String nameCell)
		{
			Range excelcells;
			Worksheet tmp = (Worksheet)workBook.Worksheets.get_Item(Sheet);
			try
			{
				excelcells = tmp.get_Range(nameCell, Type.Missing);
				return excelcells.Value2;
			}
			catch (System.Runtime.InteropServices.COMException e)
			{
				String[] s = e.Message.Split(' ');
				if ((s.Length > 0) && (s[s.Length - 1] == "0x800A03EC"))
					return null;
				else
					throw new Exception(e.Message, e);
			}
		}

		/// <summary>
		/// Получает данные из ячейки по номеру строки и букве столбца
		/// </summary>
		/// <param name="Sheet">Название страницы</param>
		/// <param name="row">Номер строки</param>
		/// <param name="namecol">Название столбца</param>
		/// <returns>Значение</returns>
		public Object GetDataFromCell(String Sheet, Int32 row, String namecol)
		{
			Range excelcells;
			Worksheet tmp = (Worksheet)workBook.Worksheets.get_Item(Sheet);
			try
			{
				excelcells = tmp.get_Range(namecol + row.ToString(), Type.Missing);
				return excelcells.Value2;
			}
			catch (System.Runtime.InteropServices.COMException e)
			{
				String[] s = e.Message.Split(' ');
				if ((s.Length > 0) && (s[s.Length - 1] == "0x800A03EC"))
					return null;
				else
					throw new Exception(e.Message, e);
			}
		}

		/// <summary>
		/// Получает данные из ячейки на пересечении поименованых строки и столбца
		/// </summary>
		/// <param name="Sheet">Объект страницы Excel.Worksheet</param>
		/// <param name="namerow">Название строки</param>
		/// <param name="namecol">Название столбца</param>
		/// <returns>Значение</returns>
		public Object GetDataFromCell(Worksheet Sheet, String namerow, String namecol)
		{
			try
			{
				Range excelcells = Sheet.get_Range(namerow + " " + namecol, Type.Missing);
				return excelcells.Value2;
			}
			catch (System.Runtime.InteropServices.COMException e)
			{
				String[] s = e.Message.Split(' ');
				if ((s.Length > 0) && (s[s.Length - 1] == "0x800A03EC"))
					return null;
				else
					throw new Exception(e.Message, e);
			}
		}


		/// <summary>
		/// Получает данные из строки
		/// </summary>
		/// <param name="Sheet">Название страницы</param>
		/// <param name="row">Номер строки</param>
		/// <param name="count">Количество ячеек читаемых из строки не более 26 ('A'..'Z')</param>
		/// <returns>Массив значений</returns>
		public Object[,] GetDataFromRow(String Sheet, Int32 row, Int32 count)
		{
			Range excelcells;
			Worksheet tmp = (Worksheet)workBook.Worksheets.get_Item(Sheet);
			Char a = 'A';
			for (Int32 i = 1; i < count; i++) a++;
			try
			{
				excelcells = tmp.get_Range("A" + row.ToString(), a + row.ToString());
				return (Object[,])excelcells.Value2;
			}
			catch (System.Runtime.InteropServices.COMException e)
			{
				String[] s = e.Message.Split(' ');
				if ((s.Length > 0) && (s[s.Length - 1] == "0x800A03EC"))
					return null;
				else
					throw new Exception(e.Message, e);
			}
		}

		public System.Data.DataTable GetDataFromSheetY6(Worksheet Sheet)
		{
			System.Data.DataTable dt = new System.Data.DataTable("Индикаторы");
			dt.Columns.Add("VALUEESTIMATE", System.Type.GetType("System.Decimal"));
			dt.Columns.Add("VALUEY1", System.Type.GetType("System.Decimal"));
			dt.Columns.Add("VALUEY2", System.Type.GetType("System.Decimal"));
			dt.Columns.Add("VALUEY3", System.Type.GetType("System.Decimal"));
			dt.Columns.Add("VALUEY4", System.Type.GetType("System.Decimal"));
			dt.Columns.Add("VALUEY5", System.Type.GetType("System.Decimal"));
			dt.Columns.Add("SIGNAT", System.Type.GetType("System.String"));

			Range excelcells;
			Int32 row = 1;
			Boolean allNull;
			do
			{
				allNull = false;
				try
				{
					excelcells = Sheet.get_Range("A" + row.ToString(), "K" + row.ToString());
					Object[,] o = (Object[,])excelcells.Value2;
					if (o[1, 1] != null)
					{
						DataRow dr = dt.NewRow();
						dr["VALUEESTIMATE"] = Math.Round(Convert.ToDecimal(o[1, 6]), 4);
						dr["VALUEY1"] = Math.Round(Convert.ToDecimal(o[1, 7]), 4);
						dr["VALUEY2"] = Math.Round(Convert.ToDecimal(o[1, 8]), 4);
						dr["VALUEY3"] = Math.Round(Convert.ToDecimal(o[1, 9]), 4);
						dr["VALUEY4"] = Math.Round(Convert.ToDecimal(o[1, 10]), 4);
						dr["VALUEY5"] = Math.Round(Convert.ToDecimal(o[1, 11]), 4);
						dr["SIGNAT"] = o[1, 1];
						dt.Rows.Add(dr);
					}
					else
					{
						if ((o[1, 1] == null) && (o[1, 2] == null) && (o[1, 3] == null) && (o[1, 4] == null) && (o[1, 5] == null) && (o[1, 6] == null) && (o[1, 7] == null)) 
							allNull = true;
					}
				}
				catch (System.Runtime.InteropServices.COMException e)
				{
					Trace.TraceMes("Ошибка при чтении индикаторов");
					String[] s = e.Message.Split(' ');
					if ((s.Length == 0) || (s[s.Length - 1] != "0x800A03EC"))
						throw new Exception(e.Message, e);
				}
				row++;
			} while (!allNull);
			return dt;
		}

		public System.Data.DataTable GetChangedIndicatorsY6(Worksheet Sheet)
		{
			System.Data.DataTable dt = new System.Data.DataTable("Индикаторы");
			dt.Columns.Add("VALUEESTIMATE", System.Type.GetType("System.Decimal"));
			dt.Columns.Add("VALUEY1", System.Type.GetType("System.Decimal"));
			dt.Columns.Add("VALUEY2", System.Type.GetType("System.Decimal"));
			dt.Columns.Add("VALUEY3", System.Type.GetType("System.Decimal"));
			dt.Columns.Add("VALUEY4", System.Type.GetType("System.Decimal"));
			dt.Columns.Add("VALUEY5", System.Type.GetType("System.Decimal"));
			dt.Columns.Add("SIGNAT", System.Type.GetType("System.String"));

			Range excelcells;
			Int32 row = 1;
			Boolean allNull;

			do
			{
				allNull = false;
				try
				{
					excelcells = Sheet.get_Range("A" + row.ToString(), "R" + row.ToString());

					Object[,] o = (Object[,])excelcells.Value2;
					if (o[1, 1] != null)
					{
						if (Convert.ToBoolean(o[1, 18]))
						{
							DataRow dr = dt.NewRow();
							dr["VALUEESTIMATE"] = o[1, 6];
							dr["VALUEY1"] = o[1, 7];
							dr["VALUEY2"] = o[1, 8];
							dr["VALUEY3"] = o[1, 9];
							dr["VALUEY4"] = o[1, 10];
							dr["VALUEY5"] = o[1, 11];
							dr["SIGNAT"] = o[1, 1];
							dt.Rows.Add(dr);
						}
					}
					else
					{
						if ((o[1, 1] == null) && (o[1, 2] == null) && (o[1, 3] == null) && (o[1, 4] == null) && (o[1, 5] == null) && (o[1, 6] == null) && (o[1, 7] == null))
							allNull = true;
					}
				}
				catch (System.Runtime.InteropServices.COMException e)
				{
					Trace.TraceMes("Ошибка при чтении индикаторов");
					String[] s = e.Message.Split(' ');
					if ((s.Length == 0) || (s[s.Length - 1] != "0x800A03EC"))
						throw new Exception(e.Message, e);
				}
				row++;
			} while (!allNull);
			return dt;
		}

		/// <summary>
		/// Получает данные из Worksheet для формы 2п (столбцы Est, y.1, y.2, y.3, y.4, y.5)
		/// </summary>
		/// <param name="Sheet">Объект страницы Excel.Worksheet</param>
		/// <returns>DataTable значений из 7 столбцов в последнем SIGNAT</returns>
		public System.Data.DataTable GetDataFromSheetF2p(Worksheet Sheet)
		{
			System.Data.DataTable dt = new System.Data.DataTable("Форма2п");
			dt.Columns.Add("VALUEESTIMATE", System.Type.GetType("System.Decimal"));
			dt.Columns.Add("VALUEY1", System.Type.GetType("System.Decimal"));
			dt.Columns.Add("VALUEY2", System.Type.GetType("System.Decimal"));
			dt.Columns.Add("VALUEY3", System.Type.GetType("System.Decimal"));
			dt.Columns.Add("SIGNAT", System.Type.GetType("System.String"));

			Range excelcells;
			Int32 row = 1;
			Boolean allNull;
			do
			{
				allNull = false;
				try
				{
					excelcells = Sheet.get_Range("M" + row.ToString(), "V" + row.ToString());
					Object[,] o = (Object[,])excelcells.Value2;
					if ((o[1, 10] != null) && (o[1, 10].ToString() != "-"))  //Ячейка "V" - сигнатура
					{
						DataRow dr = dt.NewRow();
						dr["VALUEESTIMATE"] = Math.Round(Convert.ToDecimal(o[1, 2]), 4);//Ячейка "N" - оценочный
						dr["VALUEY1"] = Math.Round(Convert.ToDecimal(o[1, 3]), 4);		//Ячейка "O" - 1-ый
						dr["VALUEY2"] = Math.Round(Convert.ToDecimal(o[1, 5]), 4);		//Ячейка "Q" - 2-ой 
						dr["VALUEY3"] = Math.Round(Convert.ToDecimal(o[1, 7]), 4);		//Ячейка "S" - 3-ий 
						dr["SIGNAT"] = o[1, 10];
						dt.Rows.Add(dr);
					}
					else
					{
						if ((o[1, 2] == null) && (o[1, 3] == null) && (o[1, 4] == null) && (o[1, 7] == null) && (o[1, 10] == null))
							allNull = true;
					}
				}
				catch (System.Runtime.InteropServices.COMException e)
				{
					Trace.TraceMes("Ошибка при чтении из файла формы 2п");
					String[] s = e.Message.Split(' ');
					if ((s.Length == 0) || (s[s.Length - 1] != "0x800A03EC"))
						throw new Exception(e.Message, e);
				}
				row++;
			} while (!allNull);
			return dt;
		}

		/// <summary>
		/// Устанавливает значениев ячейке  на пересечении поименованых строки и столбца
		/// </summary>
		/// <param name="Sheet">Название страницы</param>
		/// <param name="namerow">Имя строки</param>
		/// <param name="namecol">Имя столбца</param>
		/// <param name="value">Устанавливаемое значение</param>
		public void SetDataToCell(String Sheet, String namerow, String namecol, Decimal value)
		{
			Range excelcells;
			Worksheet tmp = (Worksheet)workBook.Worksheets.get_Item(Sheet);
			excelcells = tmp.get_Range(namerow + " " + namecol, Type.Missing);
			excelcells.Value2 = (Decimal)value;
		}

		/// <summary>
		/// Устанавливает значениев ячейке  на пересечении поименованых строки и столбца
		/// </summary>
		/// <param name="Sheet">Объект страницы Excel.Worksheet</param>
		/// <param name="namerow">Имя строки</param>
		/// <param name="namecol">Имя столбца</param>
		/// <param name="value">Устанавливаемое значение</param>
		public void SetDataToCell(Worksheet Sheet, String namerow, String namecol, Decimal value)
		{
			try
			{
				Range excelcells = Sheet.get_Range(namerow + " " + namecol, Type.Missing);
				excelcells.Value2 = value;
			}
			catch (System.Runtime.InteropServices.COMException e)
			{
				String[] s = e.Message.Split(' ');
				if ((s.Length == 0) || (s[s.Length - 1] != "0x800A03EC"))
					throw new Exception(e.Message, e);
				else
					Trace.TraceMes("Параметр {0} на {1} передан с ошибкой! ", namerow, namecol);
			}
		}

		public void SetDataToCellY6(Worksheet Sheet, String namerow, Decimal estValue, Decimal Y1Value, Decimal Y2Value,
		                            Decimal Y3Value, Decimal Y4Value, Decimal Y5Value)
		{
			Range excelcells = Sheet.get_Range(namerow + " " + "estimate", Type.Missing);
			excelcells.Value2 = (Decimal)estValue;
			excelcells = Sheet.get_Range(namerow + " " + "y.1", Type.Missing);
			excelcells.Value2 = (Decimal)Y1Value;
			excelcells = Sheet.get_Range(namerow + " " + "y.2", Type.Missing);
			excelcells.Value2 = (Decimal)Y2Value;
			excelcells = Sheet.get_Range(namerow + " " + "y.3", Type.Missing);
			excelcells.Value2 = (Decimal)Y3Value;
			excelcells = Sheet.get_Range(namerow + " " + "y.4", Type.Missing);
			excelcells.Value2 = (Decimal)Y4Value;
			excelcells = Sheet.get_Range(namerow + " " + "y.5", Type.Missing);
			excelcells.Value2 = (Decimal)Y5Value;
		}

		public void SetDataToCellY5(Worksheet Sheet, String namerow, Decimal Y1Value, Decimal Y2Value,
						Decimal Y3Value, Decimal Y4Value, Decimal Y5Value)
		{
			Range excelcells = Sheet.get_Range(namerow + " " + "y.1", Type.Missing);
			excelcells.Value2 = (Decimal)Y1Value;
			excelcells = Sheet.get_Range(namerow + " " + "y.2", Type.Missing);
			excelcells.Value2 = (Decimal)Y2Value;
			excelcells = Sheet.get_Range(namerow + " " + "y.3", Type.Missing);
			excelcells.Value2 = (Decimal)Y3Value;
			excelcells = Sheet.get_Range(namerow + " " + "y.4", Type.Missing);
			excelcells.Value2 = (Decimal)Y4Value;
			excelcells = Sheet.get_Range(namerow + " " + "y.5", Type.Missing);
			excelcells.Value2 = (Decimal)Y5Value;
		}

		[Obsolete("Very slow")]
		public void SetDataToCellY6(Worksheet Sheet, System.Data.DataTable dtValues)
		{
			foreach (DataRow row in dtValues.Rows)
			{
				String namerow = row["SIGNAT"].ToString();
				Range excelcells;

				try
				{
					if (row["VALUEESTIMATE"] != DBNull.Value)
					{
						excelcells = Sheet.get_Range(namerow + " " + "estimate", Type.Missing);
						excelcells.Value2 = Convert.ToDecimal(row["VALUEESTIMATE"]);
					}
					excelcells = Sheet.get_Range(namerow + " " + "y.1", Type.Missing);
					excelcells.Value2 = Convert.ToDecimal(row["VALUEY1"]);
					excelcells = Sheet.get_Range(namerow + " " + "y.2", Type.Missing);
					excelcells.Value2 = Convert.ToDecimal(row["VALUEY2"]);
					excelcells = Sheet.get_Range(namerow + " " + "y.3", Type.Missing);
					excelcells.Value2 = Convert.ToDecimal(row["VALUEY3"]);
					excelcells = Sheet.get_Range(namerow + " " + "y.4", Type.Missing);
					excelcells.Value2 = Convert.ToDecimal(row["VALUEY4"]);
					excelcells = Sheet.get_Range(namerow + " " + "y.5", Type.Missing);
					excelcells.Value2 = Convert.ToDecimal(row["VALUEY5"]);
				}
				catch (System.Runtime.InteropServices.COMException e)
				{
					Trace.TraceMes("Ошибка при передаче параметра: {0}", namerow);

					String[] s = e.Message.Split(' ');
					if ((s.Length == 0) || (s[s.Length - 1] != "0x800A03EC"))
						throw new Exception(e.Message, e);
				}
			}
		}

		public void SetDataToCellY6_2(Worksheet Sheet, System.Data.DataTable dtValues)
		{
			foreach (DataRow row in dtValues.Rows)
			{
				String namerow = row["SIGNAT"].ToString();

				try
				{
					Range excelcells = Sheet.get_Range(namerow, Type.Missing);
					Object[,] o = (Object[,])excelcells.Value2;

					if (row["VALUEESTIMATE"] != DBNull.Value)
					{
						o[1, 6] = Convert.ToDecimal(row["VALUEESTIMATE"]);
					}
					o[1, 7] = Convert.ToDecimal(row["VALUEY1"]);
					o[1, 8] = Convert.ToDecimal(row["VALUEY2"]);
					o[1, 9] = Convert.ToDecimal(row["VALUEY3"]);
					o[1, 10] = Convert.ToDecimal(row["VALUEY4"]);
					o[1, 11] = Convert.ToDecimal(row["VALUEY5"]);
					excelcells.Value2 = o;
				}
				catch (System.Runtime.InteropServices.COMException e)
				{
					Trace.TraceMes("Ошибка при передаче параметра: {0}", namerow);

					String[] s = e.Message.Split(' ');
					if ((s.Length == 0) || (s[s.Length - 1] != "0x800A03EC"))
						throw new Exception(e.Message, e);
				}
			}

		}

		public void SetDataToCellY2_2(Worksheet Sheet, System.Data.DataTable dtValues)
		{
			foreach (DataRow row in dtValues.Rows)
			{
				String namerow = row["SIGNAT"].ToString();
				
				Range excelcells;
				if (namerow.EndsWith(".base"))
				{
					try
					{
						excelcells = Sheet.get_Range(namerow, Type.Missing);
						excelcells.Value2 = Convert.ToDecimal(row["VALUEBASE"]);
					}
					catch (System.Runtime.InteropServices.COMException e)
					{
						Trace.TraceMes("Ошибка при передаче параметра на базовый год: {0}", namerow);
						String[] s = e.Message.Split(' ');
						if ((s.Length == 0) || (s[s.Length - 1] != "0x800A03EC"))
							throw new Exception(e.Message, e);
					}
				}
				else
				{
					if (row["VALUEBASE"] != DBNull.Value)
					{
						try
						{
							excelcells = Sheet.get_Range(namerow, Type.Missing);
							excelcells.Value2 = Convert.ToDecimal(row["VALUEBASE"]);
						}
						catch (System.Runtime.InteropServices.COMException e)
						{
							String[] s = e.Message.Split(' ');
							if ((s.Length == 0) || (s[s.Length - 1] != "0x800A03EC"))
								throw new Exception(e.Message, e);
							else
								Trace.TraceMes("Параметр {0} на базовый год передан с ошибкой! ", namerow);
						}
					}
					if (row["VALUEESTIMATE"] != DBNull.Value)
					{
						try
						{
							excelcells = Sheet.get_Range(namerow + ".est", Type.Missing);
							excelcells.Value2 = Convert.ToDecimal(row["VALUEESTIMATE"]);
						}
						catch (System.Runtime.InteropServices.COMException e)
						{
							String[] s = e.Message.Split(' ');
							if ((s.Length == 0) || (s[s.Length - 1] != "0x800A03EC"))
								throw new Exception(e.Message, e);
							else
								Trace.TraceMes("Параметр {0} на оценочный год передан с ошибкой! ", namerow);
						}
					}
				}
			}
		}


		public void SetDataToCellY2_2_Masked(Worksheet Sheet, System.Data.DataTable dtValues)
		{
			foreach (DataRow row in dtValues.Rows)
			{
				String namerow = row["SIGNAT"].ToString();

				Range excelcells;

				// Если сигнатура оканчивается на .base данный параметр передается только на базовый год
				if (namerow.EndsWith(".base"))
				{
					try
					{
						excelcells = Sheet.get_Range(namerow, Type.Missing);
						excelcells.Value2 = Convert.ToDecimal(row["VALUEBASE"]);
					}
					catch (System.Runtime.InteropServices.COMException e)
					{
						Trace.TraceMes("Ошибка при передаче параметра на базовый год: {0}", namerow);
						String[] s = e.Message.Split(' ');
						if ((s.Length == 0) || (s[s.Length - 1] != "0x800A03EC"))
							throw new Exception(e.Message, e);
					}
				}
				else
				{
					//Анализируем маску

					Byte mask = Convert.ToByte(row["MASK"]);
					Boolean baseyear = (mask & 0x02) != 0;
					Boolean estyear = (mask & 0x01) != 0;

					if (baseyear)
					{
						if (row["VALUEBASE"] == DBNull.Value)
						{
							Trace.TraceMes("Параметр {0} на базовый год не задан! ", namerow);
						}
						else
						try
						{
							excelcells = Sheet.get_Range(namerow, Type.Missing);
							excelcells.Value2 = Convert.ToDecimal(row["VALUEBASE"]);
						}
						catch (System.Runtime.InteropServices.COMException e)
						{
							String[] s = e.Message.Split(' ');
							if ((s.Length == 0) || (s[s.Length - 1] != "0x800A03EC"))
								throw new Exception(e.Message, e);
							else
								Trace.TraceMes("Параметр {0} на базовый год передан с ошибкой! ", namerow);
						}
					}
					if (estyear)
					{
						if (row["VALUEESTIMATE"] == DBNull.Value)
						{
							Trace.TraceMes("Параметр {0} на оценочный год не задан! ", namerow);
						}
						else
						try
						{
							excelcells = Sheet.get_Range(namerow + ".est", Type.Missing);
							excelcells.Value2 = Convert.ToDecimal(row["VALUEESTIMATE"]);
						}
						catch (System.Runtime.InteropServices.COMException e)
						{
							String[] s = e.Message.Split(' ');
							if ((s.Length == 0) || (s[s.Length - 1] != "0x800A03EC"))
								throw new Exception(e.Message, e);
							else
								Trace.TraceMes("Параметр {0} на оценочный год передан с ошибкой! ", namerow);
						}
					}
				}
			}
		}

		/// <summary>
		/// Устанавливает значение в 2 ячейках поименованых строки в столбцах
		/// в форме2п согласно данных в столбце SIGNAT
		/// </summary>
		/// <param name="Sheet">Объект страницы Excel.Worksheet</param>
		/// <param name="dtValues">Таблица с данными 3 столбца, в последнем сигнатура параметра</param>
		public void SetDataToCellForm2p_Masked(Worksheet Sheet, System.Data.DataTable dtValues)
		{
			foreach (DataRow row in dtValues.Rows)
			{
				String namerow = row["SIGNAT"].ToString();
				Range excelcells;
				Boolean baseyear1 = false;
				Boolean baseyear2 = false;

				//Анализируем маску
				if (namerow.Contains("_"))
				{
					String[] parts = namerow.Split('_');
					for (Int32 i = 1; i < parts.Length; i++)
						if (parts[i].StartsWith("I"))
						{
							Int32 k = Convert.ToInt32(parts[i].Substring(1));
							baseyear1 = (k & 0x02) != 0;
							baseyear2 = (k & 0x01) != 0;
						}
				}

				if (baseyear1)
				{
					if (row["r1"] == DBNull.Value)
					{
						Trace.TraceMes("Параметр {0} на первый отчетный год не задан! ", namerow);
					}
					else
						try
						{
							excelcells = Sheet.get_Range(String.Format("_{0}",namerow), Type.Missing);
							excelcells[1, 12] = Convert.ToDecimal(row["r1"]);
						}
						catch (System.Runtime.InteropServices.COMException e)
						{
							String[] s = e.Message.Split(' ');
							if ((s.Length == 0) || (s[s.Length - 1] != "0x800A03EC"))
								throw new Exception(e.Message, e);
							else
								Trace.TraceMes("Параметр {0} на первый отчетный год передан с ошибкой! ", namerow);
						}
				}
				if (baseyear2)
				{
					if (row["r2"] == DBNull.Value)
					{
						Trace.TraceMes("Параметр {0} на второй отчетный год не задан! ", namerow);
					}
					else
						try
						{
							excelcells = Sheet.get_Range(String.Format("_{0}", namerow), Type.Missing);
							excelcells[1, 13] = Convert.ToDecimal(row["r2"]);
						}
						catch (System.Runtime.InteropServices.COMException e)
						{
							String[] s = e.Message.Split(' ');
							if ((s.Length == 0) || (s[s.Length - 1] != "0x800A03EC"))
								throw new Exception(e.Message, e);
							else
								Trace.TraceMes("Параметр {0} на второй отчетный год передан с ошибкой! ", namerow);
						}
				}

			}

		}

		/// <summary>
		/// Устанавливает значение в 8 ячейках поименованых строки в столбцах
		/// в форме2п согласно данных в столбце SIGNAT для вывода формы 2п в формате МЭР
		/// Работает отдельно от модели.
		/// </summary>
		/// <param name="Sheet">Объект страницы Excel.Worksheet</param>
		/// <param name="dtValues">Таблица с данными 9 столбцов, в последнем сигнатура параметра</param>
		public void SetDataToForm2p(Worksheet Sheet, System.Data.DataTable dtValues)
		{
			foreach (DataRow row in dtValues.Rows)
			{
				String namerow = row["SIGNAT"].ToString();
				Range excelcells;
				
				try
				{
					excelcells = Sheet.get_Range(String.Format("_{0}", namerow), Type.Missing);
					if (row["r1"] != DBNull.Value)
						excelcells[1, 12] = Convert.ToDecimal(row["r1"]);
					if (row["r2"] != DBNull.Value)
						excelcells[1, 13] = Convert.ToDecimal(row["r2"]);
					if (row["est"] != DBNull.Value)
						excelcells[1, 14] = Convert.ToDecimal(row["est"]);
					if (row["v1_y1"] != DBNull.Value)
						excelcells[1, 15] = Convert.ToDecimal(row["v1_y1"]);
					if (row["v2_y1"] != DBNull.Value) 
						excelcells[1, 16] = Convert.ToDecimal(row["v2_y1"]);
					if (row["v1_y2"] != DBNull.Value) 
						excelcells[1, 17] = Convert.ToDecimal(row["v1_y2"]);
					if (row["v2_y2"] != DBNull.Value) 
						excelcells[1, 18] = Convert.ToDecimal(row["v2_y2"]);
					if (row["v1_y3"] != DBNull.Value) 
						excelcells[1, 19] = Convert.ToDecimal(row["v1_y3"]);
					if (row["v2_y3"] != DBNull.Value) 
						excelcells[1, 20] = Convert.ToDecimal(row["v2_y3"]);
				}
				catch (System.Runtime.InteropServices.COMException e)
				{
					String[] s = e.Message.Split(' ');
					if ((s.Length == 0) || (s[s.Length - 1] != "0x800A03EC"))
						throw new Exception(e.Message, e);
					else
						Trace.TraceMes("Параметр {0} передан с ошибкой! ", namerow);
				}
			}

		}

		public void Dispose()
		{
			try
			{
				if (Marshal.IsComObject(excelApp))
					Marshal.ReleaseComObject(excelApp);
			} catch (Exception e) {}
						
			try
			{
				if (Marshal.IsComObject(workBook))
					Marshal.ReleaseComObject(workBook);
			} catch (Exception e) {}

			excelApp = null;
			workBook = null;

			GC.Collect();
		}
	}
}