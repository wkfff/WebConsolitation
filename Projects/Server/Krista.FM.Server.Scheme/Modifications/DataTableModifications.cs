using System;
using System.Collections.Generic;
using System.Data;

using Krista.FM.Server.Scheme.ScriptingEngine.Classes;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Scheme.Modifications
{
    internal class DataTableModifications
    {
        private static ModificationItem GetRowChanges(string tableName, string keyColumnName, DataRow fromRow, DataRow toRow)
        {
            string keyColumnValue = AttributeScriptingEngine.Value2QueryConstantParameter(fromRow[keyColumnName]);

            // Проверяем состав колонок
            List<string> newColumns = new List<string>();
            foreach (DataColumn column in toRow.Table.Columns)
                newColumns.Add(column.ColumnName);

            List<string> setters = new List<string>();
            List<IDbDataParameter> parameters = new List<IDbDataParameter>();
            foreach (DataColumn column in fromRow.Table.Columns)
            {
                newColumns.Remove(column.ColumnName);

                if (!toRow.Table.Columns.Contains(column.ColumnName))
                    continue;

                if (Convert.ToString(fromRow[column.ColumnName]) != Convert.ToString(toRow[column.ColumnName]))
                {
                    setters.Add(String.Format("{0} = ?", column.ColumnName));
                    parameters.Add(SchemeClass.Instance.DDLDatabase.CreateParameter(column.ColumnName, toRow[column.ColumnName]));
                }
            }

            if (setters.Count > 0)
            {
                DataRowModificationItem mi = new DataRowModificationItem(ModificationTypes.Modify,
                    String.Format("Строка изменена ({0} = {1})", keyColumnName, keyColumnValue), null, toRow);
                
                mi.SqlModificationQuery = String.Format("update {0} set {1} where {2} = {3}",
                    tableName,
                    String.Join(", ", setters.ToArray()),
                    keyColumnName,
                    keyColumnValue);
                mi.Parameters = parameters.ToArray();
                
                return mi;
            }

			return null;
        }

        /// <summary>
        /// Формирует операции обновления таблицы по отличиям fromTable от toTable.
        /// </summary>
        /// <param name="modificationItem"></param>
        /// <param name="tableName">Имя таблицы.</param>
        /// <param name="keyColumnName">Имя ключевого уникального слолбца.</param>
        /// <param name="toTable">Таблица с новыми значениями.</param>
        /// <param name="fromTable">Таблица со старыми значениями.</param>
        /// <param name="afterApplayHandler"></param>
        /// <returns></returns>
		internal static ModificationItem GetChangesDataTable(ModificationItem modificationItem, string tableName, string keyColumnName, DataTable toTable, DataTable fromTable, DataRowModificationItem.ModificationItemAfterApplay afterApplayHandler)
        {
            if (toTable == null || fromTable == null)
                return null;

            List<object> newRows = new List<object>(toTable.Rows.Count);
            foreach (DataRow row in toTable.Rows)
                newRows.Add(row[keyColumnName]);

            foreach (DataRow row in fromTable.Rows)
            {
                newRows.Remove(row[keyColumnName]);
                string keyColumnValue = AttributeScriptingEngine.Value2QueryConstantParameter(row[keyColumnName]);
                DataRow[] toRows = toTable.Select(String.Format("{0} = {1}", keyColumnName, keyColumnValue));
                if (0 == toRows.Length)
                {
                    // строка удалена
                    DataRowModificationItem miRowDeleted = new DataRowModificationItem(ModificationTypes.Remove, String.Format("Строка удалена ({0} = {1})", keyColumnName, keyColumnValue), null, row);
					
					if (afterApplayHandler != null)
						miRowDeleted.AfterApplay += afterApplayHandler;
                    
					miRowDeleted.SqlModificationQuery = String.Format("delete from {0} where {1} = {2}", tableName, keyColumnName, keyColumnValue);
                    miRowDeleted.Parent = modificationItem;
                    modificationItem.Items.Add(miRowDeleted.Key, miRowDeleted);
                }
                else
                {
                    // строка изменена
                    ModificationItem miRowChanged = GetRowChanges(tableName, keyColumnName, row, toRows[0]);
                    if (miRowChanged != null)
                    {
                        miRowChanged.Parent = modificationItem;
                        modificationItem.Items.Add(miRowChanged.Key, miRowChanged);
                    }
                }
            }

            foreach (object key in newRows)
            {
                string keyColumnValue = AttributeScriptingEngine.Value2QueryConstantParameter(key);
                DataRow newRow = toTable.Select(String.Format("{0} = {1}", keyColumnName, keyColumnValue))[0];
                DataRowModificationItem miRowNew = new DataRowModificationItem(ModificationTypes.Create, String.Format("Строка добавлена ({0} = {1})", keyColumnName, keyColumnValue), null, newRow);
				
				if (afterApplayHandler != null)
					miRowNew.AfterApplay += afterApplayHandler;

                List<string> attrNames = new List<string>();
                List<string> attrValues = new List<string>();
                List<IDbDataParameter> parameters = new List<IDbDataParameter>();

                foreach (DataColumn column in toTable.Columns)
                {
                    attrNames.Add(column.ColumnName);
                    attrValues.Add("?");
                    parameters.Add(SchemeClass.Instance.DDLDatabase.CreateParameter(column.ColumnName, newRow[column.ColumnName]));
                }

                miRowNew.SqlModificationQuery =
                    String.Format("insert into {0} ({1}) values ({2})",
                        tableName,
                        String.Join(", ", attrNames.ToArray()),
                        String.Join(", ", attrValues.ToArray()));
                miRowNew.Parameters = parameters.ToArray();

                miRowNew.Parent = modificationItem;
                modificationItem.Items.Add(miRowNew.Key, miRowNew);
            }

            if (modificationItem.Items.Count > 0)
                return modificationItem;

			return null;
        }
    }
}
