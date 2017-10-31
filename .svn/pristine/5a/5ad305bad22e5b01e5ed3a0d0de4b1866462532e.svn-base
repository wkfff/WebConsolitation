using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading;
using Krista.FM.Common;
using mshtml;

namespace Krista.FM.Providers.DataAccess
{
	/// <summary>
	/// Класс для работы с html-файлами
	/// </summary>
	public class HtmlHelper : DisposableObject
	{
		#region Поля

		IHTMLDocument2 htmlDoc = null;

		#endregion Поля


		#region Константы HTML tags

		// Создает таблицу
		private const string tagTable = "table";
		// Определяет строку в таблице
		private const string tagTableRow = "tr";
		// Определяет отдельную ячейку в таблице
		private const string tagTableCell = "td";
		// Определяет заголовок таблицы (нормальная ячейка с отцентрованным жирным текстом)
		private const string tagTableHeaderCell = "th";

		#endregion Константы HTML tags


		/// <summary>
		/// Деструктор
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (htmlDoc != null) htmlDoc.close();
			}

			base.Dispose(disposing);
		}

		/// <summary>
		/// Удаляет из кода хтмл ненужные теги (ссылки, банеры и проч.)
		/// </summary>
		/// <param name="html">Код хтмл</param>
		/// <returns>Почиканный хтмл</returns>
		private string CorrectHtml(string html)
		{
			string result = html;
			int pos;

			// Ищем и удаляем ссылки на другие страницы
			while ((pos = result.IndexOf("<a href")) >= 0)
			{
				result = result.Remove(pos, result.IndexOf("</a>", pos) - pos + 4);
			}

			// Ищем все баннеры и удаляем их
			while ((pos = result.IndexOf("<img")) >= 0)
			{
				// Ищем закрывающий тэг баннера и удаляем его нафиг
				result = result.Remove(pos, result.IndexOf(">", pos) - pos + 1);
			}

			return result;
		}

		/// <summary>
		/// Загружает html-файл как хмл
		/// </summary>
		/// <param name="stream">Файл</param>
		private IHTMLDocument2 LoadHtmlDocument(Stream stream)
		{
			htmlDoc = new HTMLDocumentClass();

			//htmlDoc.designMode = "on";
			//while (htmlDoc.readyState != "complete")
			//{
			//    Thread.Sleep(20);
			//}

			StreamReader sr = new StreamReader(stream, Encoding.GetEncoding(1251));
			string str = CorrectHtml(sr.ReadToEnd());
			sr.Close();

			htmlDoc.write(str);

			//htmlDoc.designMode = "off";
			//while (htmlDoc.readyState != "complete")
			//{
			//    Thread.Sleep(20);
			//}

			return htmlDoc;
		}

		/// <summary>
		/// Создает указанное количество столбов в таблице. Старые столбцы удаляются
		/// </summary>
		/// <param name="dt">Таблица</param>
		/// <param name="numFields">Количество столбцов</param>
		private void CreateDataTableFields(DataTable dt, int numFields)
		{
			if (dt == null) return;

			dt.BeginInit();
			dt.Columns.Clear();

			for (int i = 0; i < numFields; i++)
			{
				dt.Columns.Add(string.Format("FIELD{0}", i));
			}
			dt.EndInit();
		}

		/// <summary>
		/// Возвращает датасет с данными всех таблиц Html-файла.
		/// </summary>
		/// <param name="htmlDoc">html-документ.</param>
		/// <param name="removeEmptyTables">Пустые таблицы удаляются.</param>
		/// <returns>Датасет</returns>
		private DataSet GetTablesFromHtml(IHTMLDocument2 htmlDoc, bool removeEmptyTables)
		{
			if (htmlDoc == null) return null;

			DataSet result = new DataSet();
			result.EnforceConstraints = false;

			int innerTablesRowsCount;
			int innerTablesDepth = 0;

			// Выбираем все таблицы
			IHTMLElementCollection tables = htmlDoc.all.tags(tagTable) as IHTMLElementCollection;
			int tablesLength = tables.length;
			for (int i = 0; i < tablesLength; i++)
			{
				IHTMLElementCollection table = (tables.item(i, null) as IHTMLElement).all as IHTMLElementCollection;

				DataTable dt = new DataTable();
				result.Tables.Add(dt);

				innerTablesRowsCount = 0;

				dt.BeginLoadData();
				FillTableFromHtml(result, dt, table, ref innerTablesRowsCount, ref innerTablesDepth);
				dt.EndLoadData();

				if (removeEmptyTables && dt.Rows.Count == 0)
				{
					result.Tables.Remove(dt);
				}

				i += innerTablesDepth;
			}

			return result;
		}

		/// <summary>
		/// Возвращает датасет с данными всех таблиц Html-файла.
		/// </summary>
		/// <param name="stream">Файл html</param>
		/// <param name="removeEmptyTables">Пустые таблицы удаляются</param>
		/// <returns>Датасет</returns>
		public DataSet GetTablesFromHtml(Stream stream, bool removeEmptyTables)
		{
			IHTMLDocument2 htmlDoc = LoadHtmlDocument(stream);
			return GetTablesFromHtml(htmlDoc, removeEmptyTables);
		}

		/// <summary>
		/// Возвращает датасет с данными всех таблиц Html-файла.
		/// </summary>
		/// <param name="file">Файл html</param>
		/// <param name="removeEmptyTables">Пустые таблицы удаляются</param>
		/// <returns>Датасет</returns>
		public DataSet GetTablesFromHtml(FileInfo file, bool removeEmptyTables)
		{
			if (!file.Exists)
				return null;

			IHTMLDocument2 htmlDoc = LoadHtmlDocument(file.OpenRead());
			return GetTablesFromHtml(htmlDoc, removeEmptyTables);
		}

		/// <summary>
		/// Создает таблицу в датасете и заполняет ее данными их хтмл
		/// </summary>
		/// <param name="ds">Датасет</param>
		/// <param name="dt">Текущая таблица (куда качаются данные)</param>
		/// <param name="table">Таблица хтмл</param>
		/// <param name="innerTablesRowsCount">Общее количество строк во вложенных таблицах</param>
		/// <param name="innerTablesDepth">Количество вложенных таблиц</param>
		private void FillTableFromHtml(DataSet ds, DataTable dt, IHTMLElementCollection table,
			ref int innerTablesRowsCount, ref int innerTablesDepth)
		{
			// Получаем строки таблицы
			IHTMLElementCollection tableRows = table.tags(tagTableRow) as IHTMLElementCollection;

			if (innerTablesDepth > 0)
			{
				innerTablesRowsCount += tableRows.length;
			}

			// Бежим по всем строкам и перекачиваем данные
			for (int i = 0; i < tableRows.length; i++)
			{
				IHTMLElement tableRow = tableRows.item(i, null) as IHTMLElement;
				//IHTMLElement2 tableRow2 = tableRows.item(i, null) as IHTMLElement2;

				IHTMLElementCollection childrens = tableRow.children as IHTMLElementCollection;

				// Если строка содержит описание заголовка - пропускаем ее
				IHTMLElementCollection headerCells = childrens.tags(tagTableHeaderCell) as IHTMLElementCollection;
				if (headerCells.length > 0)
				{
					CreateDataTableFields(dt, headerCells.length);
					continue;
				}

				// Выбираем ячейки строки
				IHTMLElementCollection rowCells = tableRow.children as IHTMLElementCollection;
				int rowCellsLength = rowCells.length;

				// Ячейки заголовка могут быть не заданы. Потому создаем столбцы по ячейкам первой строки
				if (i == 0)
				{
					CreateDataTableFields(dt, rowCellsLength);
				}

				if (rowCellsLength == 0)
					continue;

				DataRow row = dt.NewRow();
				row.BeginEdit();

				bool rowIsEmpty = true;

				// Ячейка может содержать вложенные таблицы. Рекурсивно обходим все.
				for (int j = 0; j < rowCellsLength; j++)
				{
					object cellBoxed = rowCells.item(j, null);
					IHTMLElement cell = cellBoxed as IHTMLElement;
					IHTMLElement2 cell2 = cellBoxed as IHTMLElement2;

					#region UNUSED
					/*int childsLength = 0;
                    IHTMLElementCollection childs = cell.children as IHTMLElementCollection;
                    if (childs != null)
                        childsLength = childs.length;

                    StringBuilder sb = null;
                    if (childsLength > 0)
                    {
                        sb = new StringBuilder();
                        IHTMLElementCollection innerTables = childs.tags(tagTable) as IHTMLElementCollection;
                        int innerTablesLength = innerTables.length;
                        for (int k = 0; k < innerTablesLength; k++)
                        {
                            innerTablesDepth++;

                            IHTMLElement innerTabl = innerTables.item(k, null) as IHTMLElement;
                            IHTMLElementCollection innerTable = innerTabl.all as IHTMLElementCollection;

                            DataTable newTable = new DataTable();
                            newTable.BeginLoadData();
                            ds.Tables.Add(newTable);

                            FillTableFromHtml(ds, newTable, innerTable, ref innerTablesRowsCount, ref innerTablesDepth);
                            newTable.EndLoadData();

                            i += innerTablesRowsCount;
                            sb.Append(innerTabl.innerText);
                        }

                    }*/
					#endregion

					// Ячейка может содержать вложенные таблицы. Рекурсивно обходим все.
					IHTMLElementCollection innerTables = cell2.getElementsByTagName(tagTable) as IHTMLElementCollection;
					int innerTablesLength = innerTables.length;
					StringBuilder sb = null;
					if (innerTablesLength > 0)
					{
						sb = new StringBuilder();
						for (int k = 0; k < innerTablesLength; k++)
						{
							innerTablesDepth++;

							IHTMLElement innerTabl = innerTables.item(k, null) as IHTMLElement;
							IHTMLElementCollection innerTable = innerTabl.all as IHTMLElementCollection;

							DataTable newTable = new DataTable();
							newTable.BeginLoadData();
							ds.Tables.Add(newTable);

							FillTableFromHtml(ds, newTable, innerTable, ref innerTablesRowsCount, ref innerTablesDepth);
							newTable.EndLoadData();

							i += innerTablesRowsCount;
							sb.Append(innerTabl.innerText);
						}
					}
					string innerTablesText = string.Empty;
					if (sb != null)
						innerTablesText = sb.ToString();
					string cellInnerText = cell.innerText;

					if ((!String.IsNullOrEmpty(cellInnerText) &&
						(!String.IsNullOrEmpty(innerTablesText))))
					{
						row[j] = cellInnerText.Remove(cellInnerText.Length - innerTablesText.Length).Trim();
					}
					else if (cellInnerText != null)
					{
						row[j] = cellInnerText.Trim();
					}

					rowIsEmpty = String.IsNullOrEmpty(cellInnerText);
				}

				if (!rowIsEmpty)
				{
					row.EndEdit();
					dt.Rows.Add(row);
				}
			}
		}
	}
}
