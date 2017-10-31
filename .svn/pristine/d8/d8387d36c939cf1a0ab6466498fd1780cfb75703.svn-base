using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;
using Krista.FM.Common;
using Krista.FM.Domain.Reporitory.NHibernate.IoC;
using Krista.FM.Server.Common;

using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.TemplatesService;

namespace Krista.FM.Server.TemplatesService
{
	public partial class TemplatesRepository
	{
		#region Export

		/// <summary>
		/// создает и записываем XML атрибут
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="attributeName"></param>
		/// <param name="attributeValue"></param>
		private static void CreateXMLAttribute(XmlWriter writer, string attributeName, string attributeValue)
		{
			writer.WriteStartAttribute(attributeName);
			writer.WriteString(attributeValue);
			writer.WriteEndAttribute();
		}

		/// <summary>
		/// построение XML
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="reader"></param>
		private void SaveDataToStream(Stream stream, IDataReader reader)
		{
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Encoding = Encoding.GetEncoding(1251);
			settings.CheckCharacters = false;
			settings.Indent = true;
			XmlWriter writer = XmlWriter.Create(stream, settings);

			// создаем XML файл со структурой... Сохраняем параметры записи
			// Сперва для каждой таблицы получаем параметры, записываем их
			// формируем XML документ
			writer.WriteStartDocument(true);
			writer.WriteStartElement(XmlConsts.rootNode);
			CreateXMLAttribute(writer, XmlConsts.versionAttribute, scheme.UsersManager.ServerLibraryVersion());

			// запись секции с данными
			WriteTemplatesData(writer, reader);

			// закрываем корневой элемент и документ в целом
			writer.WriteEndElement();
			writer.WriteEndDocument();

			// закрывает поток и записывает все в файл
			writer.Flush();
			writer.Close();
		}

		private void WriteTemplatesData(XmlWriter writer, IDataReader reader)
		{
			object[] values = new object[12];
			while (reader.Read())
			{
				writer.WriteStartElement(XmlConsts.templateNode);

				reader.GetValues(values);
				WriteTemplateValue(writer, values[0], TemplateFields.ID);
				WriteTemplateValue(writer, values[1], TemplateFields.Code);
				WriteTemplateValue(writer, values[2], TemplateFields.Name);
				WriteTemplateValue(writer, values[3], TemplateFields.Type);
				WriteTemplateValue(writer, values[4], TemplateFields.Description);
				WriteTemplateValue(writer, values[5], TemplateFields.DocumentFileName);
				values[6] = GetDocument(Convert.ToInt32(values[0]));
				WriteTemplateValue(writer, values[6], TemplateFields.Document);
				WriteTemplateValue(writer, values[7], TemplateFields.ParentID);
				WriteTemplateValue(writer, values[9], TemplateFields.RefTemplatesTypes);
				WriteTemplateValue(writer, values[10], TemplateFields.SortIndex);
				WriteTemplateValue(writer, values[11], TemplateFields.Flags);

				writer.WriteEndElement();
			}
		}

		private static void WriteTemplateValue(XmlWriter writer, object value, string elementName)
		{
			writer.WriteStartElement(elementName);
			if (!(value is byte[]))
				if (value != null)
					writer.WriteValue(value.ToString());
				else
					writer.WriteValue(string.Empty);
			else
				writer.WriteCData(Convert.ToBase64String((byte[])value, Base64FormattingOptions.InsertLineBreaks));
			writer.WriteEndElement();
		}

		/// <summary>
		/// Возвращает объект для просмотра записей по одной штуке (DataReader).
		/// </summary>
		private static IDataReader GetDataReader(IDatabase db, IDbConnection connection, string query, IDbDataParameter[] filterParams)
		{
			IDataReader reader;

			if (connection.State == ConnectionState.Closed)
				connection.Open();
			IDbCommand command = connection.CreateCommand();

			if (filterParams != null)
				foreach (IDbDataParameter param in filterParams)
				{
					command.Parameters.Add(param);
				}

			command.CommandText = ((Database)db).GetQuery(query, command.Parameters);

			reader = command.ExecuteReader();

			return reader;
		}

		public void RepositoryExport(Stream exportStream, TemplateTypes templateType)
		{
			RepositoryExport(exportStream, null, templateType);
		}

		public void RepositoryExport(Stream exportStream, List<int> exportRowsIds, TemplateTypes templateType)
		{
			List<string> filterParts = new List<string>();
			if (exportRowsIds != null)
			{
				foreach (int id in exportRowsIds)
				{
					filterParts.Add(string.Format("ID = {0}", id));
				}
			}
			string resultQuery = String.Format("{0} {1} {2} {3}",
				selectAllQuery + " where RefTemplatesTypes = ?",
				filterParts.Count == 0 ? String.Empty : "and (",
				String.Join(" or ", filterParts.ToArray()),
				filterParts.Count == 0 ? String.Empty : ")");

			// получаем данные и последовательно передаем их в поток, который передаем на клиент
			using (IDbConnection connection = ((IConnectionProvider)scheme.SchemeDWH).Connection)
			using (IDatabase db = scheme.SchemeDWH.DB)
			{
				IDbDataParameter[] prms = ((Database) db).GetParameters(1);
				prms[0] = db.CreateParameter("TemplateType", (int) templateType);
				IDataReader reader = GetDataReader(db, connection, resultQuery, prms);
				SaveDataToStream(exportStream, reader);
			}
		}

		#endregion Export

		#region Import

		public void RepositoryImport(Stream stream, TemplateTypes templateType)
		{
			RepositoryImport(stream, -1, templateType);
		}

        //[UnitOfWork]
		public virtual void RepositoryImport(Stream stream, int parentId, TemplateTypes templateType)
		{
			Dictionary<int, int> newIDs;
			DataTable table = GetImportTableFromStream(stream, out newIDs);
			SaveImportTableToDatabase(table, newIDs, parentId);
		}

		private DataTable GetImportTableFromStream(Stream stream, out Dictionary<int, int> newIDs)
		{
			Dictionary<string, object> xmlObjects = new Dictionary<string, object>();
			newIDs = new Dictionary<int, int>();

			// Получаем пустую таблицу
			DataTable table;
			using (IDatabase database = scheme.SchemeDWH.DB)
			{
				table = (DataTable)database.ExecQuery(selectAllQuery + " where 1 <> 1", QueryResultTypes.DataTable, (IDbDataParameter[])null);
			}

			// Наполняем таблицу данными
			XmlTextReader reader = new XmlTextReader(stream);
			while (reader.Read())
			{
				switch (reader.NodeType)
				{
					case XmlNodeType.EndElement:
						switch (reader.LocalName)
						{
							case XmlConsts.templateNode:
								// типа записываем все, что получили 
								DataRow row = table.NewRow();
								int newID = NewTemplateID();
								newIDs.Add(Convert.ToInt32(xmlObjects[TemplateFields.ID]), newID);

								foreach (KeyValuePair<string, object> pair in xmlObjects)
								{
									if (pair.Key == TemplateFields.Document)
									{
										row[pair.Key] = Convert.FromBase64String((string)pair.Value);
									}
									else
										row[pair.Key] = String.IsNullOrEmpty(Convert.ToString(pair.Value)) ? DBNull.Value : pair.Value;
								}
								row[TemplateFields.ID] = newID;
								row[TemplateFields.Editor] = Authentication.UserID;

								table.Rows.Add(row);

								break;
						}
						break;
					case XmlNodeType.Element:
						switch (reader.LocalName)
						{
							case XmlConsts.templateNode:
								xmlObjects.Clear();
								break;
							case TemplateFields.ID:
							case TemplateFields.Code:
							case TemplateFields.Name:
							case TemplateFields.Type:
							case TemplateFields.Description:
							case TemplateFields.DocumentFileName:
							case TemplateFields.ParentID:
							case TemplateFields.RefTemplatesTypes:
							case TemplateFields.Document:
							case TemplateFields.SortIndex:
							case TemplateFields.Flags:
								xmlObjects.Add(reader.LocalName, reader.ReadElementString());
								break;
						}
						break;
				}
			}
			return table;
		}

		/// <summary>
		/// Сохраняет данные импорта в базу данных.
		/// </summary>
		private void SaveImportTableToDatabase(DataTable table, Dictionary<int, int> newIDs, int parentId)
		{
			Database db = (Database)scheme.SchemeDWH.DB;
			DataUpdater du = null;
			try
			{
				db.BeginTransaction();
				du = (DataUpdater)GetInternalDataUpdater(db);
				
				// ---------------------------------------------------------------
				// Сохраняем данные без иерархии				
				// - очищаем ссылки на родителя
				DataTable changes = table.GetChanges();
				foreach (DataRow row in changes.Rows)
				{
					row[TemplateFields.ParentID] = DBNull.Value;
				}

				du.Update(ref changes);
				changes.Clear();
				table.AcceptChanges();

				// ---------------------------------------------------------------
				// Устанавливаем иерархию
				// - меняем ссылки на родительские записи на новые
				foreach (int oldID in newIDs.Keys)
				{
					DataRow[] rows = table.Select(string.Format("ParentID = {0}", oldID));
					foreach (DataRow row in rows)
					{
						row[TemplateFields.ParentID] = newIDs[oldID];
					}
				}

				// - ссылки на несуществующие записи делаем пустыми
				foreach (DataRow row in table.Rows)
				{
					if (!(row[TemplateFields.ParentID] is DBNull))
						if (table.Select(string.Format("ID = {0}", row[TemplateFields.ParentID])).Length == 0)
							row[TemplateFields.ParentID] = DBNull.Value;
				}

				// - устанавливаем "родителя" для записей, которые были верхнего уровня
				if (parentId >= 0)
				{
					DataRow[] rows = table.Select("ParentID is null");
					foreach (DataRow row in rows)
					{
						row[TemplateFields.ParentID] = parentId;
					}
				}

				// ---------------------------------------------------------------
				// Сохраняем иерархию в базу данных
				changes = table.GetChanges();
				if (changes != null)
				{
					du.Update(ref changes);
					changes.Clear();
				}

				db.Commit();
                // ---------------------------------------------------------------
                // Сохраняем документы в базу данных
                foreach (DataRow row in table.Rows)
                {
                    int templateID = Convert.ToInt32(row[TemplateFields.ID]);
                    SetDocument((byte[])row[TemplateFields.Document], templateID);
                }
                // разблокируем отчеты, которые загрузили
                // разблокируем уже после коммита изменений
                foreach (DataRow row in table.Rows)
                {
                    int templateID = Convert.ToInt32(row[TemplateFields.ID]);
                    UnlockTemplate(templateID);
                }
			}
			catch (Exception e)
			{
				db.Rollback();
				throw new ServerException("Ошибка импорта данных в базу.", e);
			}
			finally
			{
				if (du != null)
					du.Dispose();

				if (db != null)
					db.Dispose();
			}
		}

		#endregion Import
	}
}
