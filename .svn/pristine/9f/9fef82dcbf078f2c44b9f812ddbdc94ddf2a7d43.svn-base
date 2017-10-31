using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using Krista.FM.Server.Scheme.ScriptingEngine.Classes;

using Krista.FM.ServerLibrary;
using Krista.FM.Server.Common;

namespace Krista.FM.Server.Scheme.Classes
{
    /// <summary>
    /// Перемещение записей между диапазонами пользователя и разработчика
    /// </summary>
    partial class Classifier
    {
        /// <summary>
        /// Изменяет диапазон записей в котором они находятся 
        /// с пользовательского диапазона на диапазон разработчика и наоборот.
        /// </summary>
        /// <param name="db">Объект доступа к базе данных.</param>
        /// <param name="rowsID">Массив исходных ID записей.</param>
        /// <returns>Массив новых ID записей.</returns>
        /// <remarks>
        /// Если массив исходных ID записей rowsID передается пустой, 
        /// то диапазон меняется для всех записей классификатора.
        /// </remarks>
        private int[] ReverseRowsRange(IDatabase db, int[] sourceRowsID)
        {
            // 1. Получаем исходные записи.
            string selectFilter = DataAttribute.RowTypeColumnName + " = 0";
            if (sourceRowsID.GetLength(0) > 0)
            {
                string[] stringRowsID = new string[sourceRowsID.GetLength(0)];
                for (int i = 0; i < sourceRowsID.GetLength(0); i++)
                {
                    stringRowsID[i] = Convert.ToString(sourceRowsID[i]);
                }
                selectFilter += String.Format(" and (ID = {0})", String.Join(" or ID = ", stringRowsID));
            }

            IDataUpdater du = GetDataUpdater(db, selectFilter, null);

            DataTable dt = new DataTable();
            du.Fill(ref dt);
            int columnIDIndex = dt.Columns.IndexOf(DataAttribute.IDColumnName);

            // 2. Создаем копии записей и помещаем их в целевой диапазон.
            Dictionary<int, int> mappingRowsID = new Dictionary<int,int>();
            List<DataRow> newRows = new List<DataRow>();
            List<DataRow> forDeleteRows = new List<DataRow>();
            foreach (DataRow row in dt.Rows)
            {
                DataRow newRow = dt.NewRow();
                newRow.ItemArray = row.ItemArray;
                // TODO Создать копии значений, не нарушаюших уникальность, для всех атрибутов, входящих в уникальный ключ

                string generatorName =
                    Convert.ToInt32(row[columnIDIndex]) >= ClassifierEntityScriptingEngine.DeveloperGeneratorSeed
                    ? ((EntityScriptingEngine)_scriptingEngine).GeneratorName(FullDBName)
                    : ((ClassifierEntityScriptingEngine)_scriptingEngine).DeveloperGeneratorName(FullDBName);

                int newRowID = db.GetGenerator(generatorName);
                newRow[columnIDIndex] = newRowID;
                newRows.Add(newRow);
                forDeleteRows.Add(row);
                mappingRowsID.Add(Convert.ToInt32(row[columnIDIndex]), newRowID);
            }

            foreach (DataRow row in newRows)
            {
                dt.Rows.Add(row);
            }
            du.Update(ref dt);

            // 4. Если классификатор иерархический, то меняем ссылки для родительских записей классификатора
            if (this.Levels.HierarchyType == Krista.FM.ServerLibrary.HierarchyType.ParentChild)
            {
                foreach (KeyValuePair<int, int> mapping in mappingRowsID)
                {
                    db.ExecQuery(String.Format(
                        "update {0} set {1} = ? where {1} = ?", this.FullDBName, DataAttribute.ParentIDColumnName),
                        QueryResultTypes.NonQuery,
                        db.CreateParameter("newRowID", mapping.Value),
                        db.CreateParameter("oldRowID", mapping.Key));

                    if (Attributes.ContainsKey(DataAttribute.CubeParentIDColumnName))
                    {
                        db.ExecQuery(String.Format(
                            "update {0} set {1} = ? where {1} = ?", this.FullDBName, DataAttribute.CubeParentIDColumnName),
                            QueryResultTypes.NonQuery,
                            db.CreateParameter("newRowID", mapping.Value),
                            db.CreateParameter("oldRowID", mapping.Key));
                    }
                }
            }

            // 5. Меняем все ссылки со старой на новую запись.
            foreach (IEntityAssociation item in Associated.Values)
            {
                try
                {
                    ((Entity)item.RoleData).DisableAllTriggers((Database)db);

                    foreach (KeyValuePair<int, int> mapping in mappingRowsID)
                    {
                        db.ExecQuery(String.Format(
                            "update {0} set {1} = ? where {1} = ?", item.RoleData.FullDBName, item.FullDBName),
                            QueryResultTypes.NonQuery,
                            db.CreateParameter("newRowID", mapping.Value),
                            db.CreateParameter("oldRowID", mapping.Key));
                    }
                }
                finally
                {
                    ((Entity)item.RoleData).EnableAllTriggers((Database)db);
                }
            }

            // 6. Удаляем старые записи.
            foreach (DataRow item in forDeleteRows)
            {
                item.Delete();
            }
            du.Update(ref dt);

            // 7. Восстанавливаем значения всех атрибутов скопированной записи
            // TODO Восстановить все значения для атрибутов входящих в уникальный ключ

            // 8. Формируем результирующий массив ID записей
            int[] rusultRorwID = new int[sourceRowsID.GetLength(0)];
            for (int i = 0; i < sourceRowsID.GetLength(0); i++)
            {
                if (mappingRowsID.ContainsKey(sourceRowsID[i]))
                    rusultRorwID[i] = mappingRowsID[sourceRowsID[i]];
                else
                    rusultRorwID[i] = sourceRowsID[i];
            }

            return rusultRorwID;
        }

        /// <summary>
        /// Изменяет диапазон записей в котором они находятся 
        /// с пользовательского диапазона на диапазон разработчика и наоборот.
        /// </summary>
        /// <param name="rowsID">Массив исходных ID записей.</param>
        /// <returns>Массив новых ID записей.</returns>
        /// <remarks>
        /// Если массив исходных ID записей rowsID передается пустой, 
        /// то диапазон меняется для всех записей классификатора.
        /// </remarks>
        public int[] ReverseRowsRange(int[] rowsID)
        {
            if (!(ClassType == ClassTypes.clsBridgeClassifier || ClassType == ClassTypes.clsDataClassifier))
                throw new InvalidOperationException(
                    "Перемещение записей между диапазонами пользователя и разработчика " +
                    "доступно только для сопоставимых и классификаторов данных.");

            IDatabase db = SchemeClass.Instance.SchemeDWH.DB;
            try
            {
                db.BeginTransaction();
                int[] newRowsID = ReverseRowsRange(db, rowsID);
                db.Commit();
                return newRowsID;
            }
            catch (Exception e)
            {
                db.Rollback();
                throw new Exception(e.Message, e);
            }
            finally
            {
                db.Dispose();
            }
        }
    }
}
