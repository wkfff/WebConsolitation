using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Xml;

using Krista.FM.Server.Common;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Scheme.Classes
{
    /// <summary>
    /// Классификатор вариантов
    /// </summary>
    internal class VariantDataClassifier : DataClassifier, IVariantDataClassifier
    {
        internal static string VariantSemantic = "Variant";

        /// <summary>
        /// Классификатор вариантов
        /// </summary>
        /// <param name="owner">Родитель</param>
        /// <param name="name">Имя</param>
        /// <param name="state"></param>
        public VariantDataClassifier(string key, ServerSideObject owner, string name, ServerSideObjectStates state)
            : this(key, owner, name,  SubClassTypes.Input, state)
		{
        }

        public VariantDataClassifier(string key, ServerSideObject owner, string name, SubClassTypes subClassType, ServerSideObjectStates state)
            : base(key, owner, VariantSemantic, name, subClassType, state, SchemeClass.ScriptingEngineFactory.VariantDataClassifierScriptingEngine)
        {
        }

        /// <summary>
        /// Метод инициализации объекта
        /// </summary>
        internal override XmlDocument Initialize()
        {
            XmlDocument doc = base.Initialize();

            Attributes.Add(DataAttribute.SystemVariantCompleted);
            Attributes.Add(DataAttribute.SystemVariantComment);

            return doc;
        }

        #region DDL

        internal override void Create(Krista.FM.Server.Scheme.Modifications.ModificationContext context)
        {
            if (!Attributes.ContainsKey(DataAttribute.SystemVariantCompleted.Name))
                Attributes.Add(DataAttribute.SystemVariantCompleted);
            if (!Attributes.ContainsKey(DataAttribute.SystemVariantComment.Name))
                Attributes.Add(DataAttribute.SystemVariantComment);

            base.Create(context);
        }

        /// <summary>
        /// Возвращает скрипт триггера для дочернего отношения
        /// </summary>
        /// <param name="entity">Дочернее отношение</param>
        /// <returns>Скрипт</returns>
        internal override List<string> GetCustomTriggerScriptForChildEntity(Entity entity, DataAttribute withoutAttribute)
        {
            return ((ScriptingEngine.Classes.VariantDataClassifierScriptingEngine)_scriptingEngine)
                .CreateVariantLockTriggerScript(this, entity, withoutAttribute);
        }

        #endregion DDL

        /// <summary>
        /// Запись события в протокол действий пользователя
        /// </summary>
        /// <param name="kind">Тип события</param>
        /// <param name="eventMsg">Текст сообщения</param>
        private void WriteIntoProtocol(ClassifiersEventKind kind, string eventMsg)
        {
            IClassifiersProtocol log = null;
            try
            {
                Krista.FM.Common.LogicalCallContextData callerContext = Krista.FM.Common.LogicalCallContextData.GetContext();
                string userHost = callerContext["Host"].ToString();
                log = (IClassifiersProtocol)SchemeClass.Instance.GetProtocol("Krista.FM.Server.Scheme.dll");
                log.WriteEventIntoClassifierProtocol(kind, this.OlapName, -1, -1, (int)this.ClassType, eventMsg);
            }
            finally
            {
                if (log != null)
                    log.Dispose();
            }
        }

        #region Реализация интерфейса IVariantDataClassifier

        /// <summary>
        /// Возвращает имя варианта по ID записи.
        /// </summary>
        /// <param name="variantID">ID записи варианта.</param>
        /// <returns>Имя варианта.</returns>
        private string GetVariantName(int variantID)
        {
            IDatabase db = SchemeClass.Instance.SchemeDWH.DB;
            try
            {
                if (DataAttributeCollection.GetAttributeByKeyName(Attributes, "Name", "Name") != null)
                {
                    string variantName;
                    variantName = Convert.ToString(db.ExecQuery(String.Format("select Name from {0} where {1} = ?", FullDBName, DataAttribute.IDColumnName),
                        QueryResultTypes.Scalar,
                        db.CreateParameter(DataAttribute.IDColumnName, variantID)));
                    return String.IsNullOrEmpty(variantName) ? String.Empty : " \"" + variantName + "\"";
                }
                else
                    return String.Empty;
            }
            catch
            {
                return String.Empty;
            }
            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// Дополнительная обработка скопированных данных
        /// </summary>
        /// <param name="row">Строка содержащая копию данных</param>
        private void ProcessCopiedData(DataRow row)
        {
            // Устанавливаем признак, что вариант не завершон
            row[DataAttribute.VariantCompletedColumnName] = 0;

            // Для строковых полей в начало вставляем "Копия: "
            foreach (DataAttribute item in Attributes.Values)
                if (item.Type == DataAttributeTypes.dtString)
                    row[item.Name] = String.Format("Копия: {0}", row[item.Name, DataRowVersion.Original]);

            // Формируем комментарий для новой копии, 
            // содержащий прежние значения полей перечисленные через точку с запятой
            string comment = String.Empty;
            foreach (DataAttribute item in Attributes.Values)
            {
                if (item.Name == DataAttribute.RowTypeColumnName)
                    continue;
                object data = row[item.Name, DataRowVersion.Original];
                if (data == null || data is DBNull)
                    continue;
                comment += String.Format(" {0}: \"{1}\";", item.Caption, row[item.Name, DataRowVersion.Original]);
            }

            row[DataAttribute.VariantCommentColumnName] = String.Format("Копия варианта:{0}", comment);
        }

        /// <summary>
        /// Копирует строку классификатора
        /// </summary>
        /// <param name="db">Объект доступа к базе данных</param>
        /// <param name="rowID">ID строки</param>
        /// <returns>ID новой строки</returns>
        private int CopyRow(IDatabase db, int rowID)
        {
            int newRowID = db.GetGenerator(GeneratorName);

            IDbDataParameter[] parameters = ((Database)db).GetParameters(1);
            parameters[0] = db.CreateParameter(DataAttribute.IDColumnName, rowID);
            IDataUpdater du = GetDataUpdater(db, String.Format("{0} = ?", DataAttribute.IDColumnName), null, parameters);

            DataTable dt = new DataTable();

            du.Fill(ref dt);

            if (dt.Rows.Count == 0)
                throw new Exception(String.Format("Запись(ID={0}) в классификаторе \"{1}\" не найдена.", rowID, this.FullCaption));

            DataRow row = dt.Rows[0];

            row[DataAttribute.IDColumnName] = newRowID;
            
            //--------------------------------------------------
            ProcessCopiedData(row);
            //--------------------------------------------------

            row.AcceptChanges();
            row.SetAdded();

            du.Update(ref dt);

            return newRowID;
        }

        // Все ссылки на вариант должны бать из таблицы фактов
        private void CheckAssocietedEntities()
        {
            foreach (EntityAssociation item in Associated.Values)
                if (item.RoleA.ClassType != ClassTypes.clsFactData)
                    throw new Exception(String.Format(
                        "Для копирования данных варианта необходимо, " +
                        "чтобы на классификатор варианта ссылались только таблицы фактов. " +
                        "Объект \"{0}\"({1}) не является таблицей фактов.",
                        item.RoleA.OlapName, item.RoleA.FullName));
        }

        /*
        /// <summary>
        /// Копирование данных варианта
        /// </summary>
        /// <param name="db">Объект доступа к базе данных</param>
        /// <param name="variantID">ID строки варианта источника</param>
        /// <param name="newVariantID">ID строки варианта приемника</param>
        private void CopyVariantData(IDatabase db, int variantID, int newVariantID, VariantListenerDelegate listener, out string copyResults)
        {
            CheckAssocietedEntities();
            StringBuilder sbResults = new StringBuilder();
            foreach (EntityAssociation item in Associated.Values)
            {
                IDbDataParameter[] parameters = ((Database)db).GetParameters(1);
                parameters[0] = db.CreateParameter(item.FullDBName, variantID);
                IDataUpdater du = item.RoleA.GetDataUpdater(db, String.Format("{0} = ?", item.FullDBName), null, parameters);
                
                DataTable dt = new DataTable();
                
                du.Fill(ref dt);

                Dictionary<int, int> copiedFactTableId = new Dictionary<int, int>();
                FactTable factTable = (FactTable)item.RoleA;
                foreach (DataRow row in dt.Rows)
                {
                    // Запоминаем ID копируемых строк.
                    int oldFactTableID = Convert.ToInt32(row[DataAttribute.IDColumnName]);
                    int newFactTableID = db.GetGenerator(factTable.GeneratorName);
                    copiedFactTableId.Add(oldFactTableID, newFactTableID);

                    row.SetAdded();
                    row[DataAttribute.IDColumnName] = newFactTableID;
                    row[item.FullDBName] = newVariantID;
                }

                int affectedRows = du.Update(ref dt);
                // Копируем детали.
                CopyFactTableDetails(factTable, db, copiedFactTableId, listener, out copyResults);

                sbResults.AppendLine(String.Format("Таблица \"{0}\", скопировано строк: {1} ", item.RoleA.OlapName, affectedRows));
                if (listener != null)
                    listener(String.Format("Таблица \"{0}\", скопировано строк: {1}", item.RoleA.OlapName, affectedRows));
            }
            copyResults = sbResults.ToString();
        }*/

        /// <summary>
        /// Копирование данных варианта
        /// </summary>
        /// <param name="db">Объект доступа к базе данных</param>
        /// <param name="variantID">ID строки варианта источника</param>
        /// <param name="newVariantID">ID строки варианта приемника</param>
        private void CopyVariantData(IDatabase db, int variantID, int newVariantID, VariantListenerDelegate listener, out string copyResults)
        {
            //CheckAssocietedEntities();
            StringBuilder sbResults = new StringBuilder();
            foreach (EntityAssociation item in Associated.Values)
            {
                IDbDataParameter[] parameters = ((Database)db).GetParameters(1);
                parameters[0] = db.CreateParameter(item.FullDBName, variantID);
                IDataUpdater du = item.RoleA.GetDataUpdater(db, String.Format("{0} = ?", item.FullDBName), null, parameters);

                DataTable dt = new DataTable();

                du.Fill(ref dt);

                Dictionary<int, int> copiedFactTableId = new Dictionary<int, int>();
                Entity entity = item.RoleA;
                int affectedRows = 0;
                foreach (DataRow row in dt.Rows)
                {
                    // Запоминаем ID копируемых строк.
                    int oldFactTableID = Convert.ToInt32(row[DataAttribute.IDColumnName]);
                    int newFactTableID = entity is IFactTable ? -1 : db.GetGenerator(entity.GeneratorName);

                    row.SetAdded();
                    
                    row[item.FullDBName] = newVariantID;
                    row[DataAttribute.IDColumnName] = newFactTableID == -1 ? (object)DBNull.Value : newFactTableID;
                    if (newFactTableID == -1)
                    {
                        // для таблиц фактов мастер деталей сохраняем каждую запись по отдельности для получения ID записи
                        if (IsMasterDetailFactTable(entity))
                        {
                            DataTable dtChange = dt.GetChanges();
                            if (dtChange != null)
                            {
                                affectedRows += du.Update(ref dtChange);
                                newFactTableID = GetNewFactTableID(entity, db);
                            }
                        }
                    }
                    copiedFactTableId.Add(oldFactTableID, newFactTableID);
                }
                if (!IsMasterDetailFactTable(entity))
                    affectedRows = du.Update(ref dt);
                // Копируем детали.
                CopyTableDetails(entity, db, copiedFactTableId, listener, out copyResults);

                sbResults.AppendLine(String.Format("Таблица \"{0}\", скопировано строк: {1} ", item.RoleA.OlapName, affectedRows));
                if (listener != null)
                    listener(String.Format("Таблица \"{0}\", скопировано строк: {1}", item.RoleA.OlapName, affectedRows));
            }
            copyResults = sbResults.ToString();
        }

        private static Int32 GetNewFactTableID(IEntity factEntity, IDatabase db)
        {
            object resultQuery = db.ExecQuery(string.Format("select Max(ID) from {0}", factEntity.FullDBName), QueryResultTypes.Scalar);
            if (!(resultQuery is DBNull))
                return Convert.ToInt32(resultQuery);
            return -1;
        }

        private static bool IsMasterDetailFactTable(IEntity factEntity)
        {
            foreach (EntityAssociation item in factEntity.Associated.Values)
            {
                if (item is MasterDetailAssociation)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Копирует детали таблицы фактов.
        /// </summary>
        /// <param name="copiedFactTableID">Словарь старый-новый ID копируемых записей таблицы фактов.</param>
        private void CopyTableDetails(Entity entity, IDatabase db, Dictionary<int, int> copiedFactTableID, VariantListenerDelegate listener, out string copyResults)
        {
            StringBuilder sbResults = new StringBuilder();
            foreach (EntityAssociation item in entity.Associated.Values)
            {
                if (item.RoleA.ClassType == ClassTypes.Table)
                {
                    int affectedRows = 0;
                    foreach (KeyValuePair<int, int> pair in copiedFactTableID)
                    {
                        IDbDataParameter[] parameters = ((Database)db).GetParameters(1);
                        parameters[0] = db.CreateParameter(item.FullDBName, pair.Key);
                        IDataUpdater du = item.RoleA.GetDataUpdater(db, String.Format("{0} = ?", item.FullDBName), null, parameters);
                        DataTable dt = new DataTable();
                        du.Fill(ref dt);
                        foreach (DataRow row in dt.Rows)
                        {
                            row.SetAdded();
                            row[DataAttribute.IDColumnName] = DBNull.Value;
                            if (pair.Value != -1)
                                row[item.FullDBName] = pair.Value;
                        }
                        affectedRows += du.Update(ref dt);
                    }
                    sbResults.AppendLine(String.Format("    Таблица \"{0}\", скопировано строк: {1} ", item.RoleA.OlapName, affectedRows));
                    if (listener != null)
                        listener(String.Format("    Таблица \"{0}\", скопировано строк: {1}", item.RoleA.OlapName, affectedRows));

                }
            }
            copyResults = sbResults.ToString();
        }

        /// <summary>
        /// Копирует детали таблицы фактов.
        /// </summary>
        /// <param name="factTable">Таблица фактов.</param>
        /// <param name="db">Объект доступа к базе данных.</param>
        /// <param name="copiedFactTableID">Словарь старый-новый ID копируемых записей таблицы фактов.</param>
        private void CopyFactTableDetails(FactTable factTable, IDatabase db, Dictionary<int, int> copiedFactTableID, VariantListenerDelegate listener, out string copyResults)
        {
            StringBuilder sbResults = new StringBuilder();
            foreach (EntityAssociation item in factTable.Associated.Values)
            {
                if (item.RoleA.ClassType == ClassTypes.Table)
                {
                    int affectedRows = 0;
                    foreach (KeyValuePair<int, int> pair in copiedFactTableID)
                    {
                        IDbDataParameter[] parameters = ((Database)db).GetParameters(1);
                        parameters[0] = db.CreateParameter(item.FullDBName, pair.Key);
                        IDataUpdater du = item.RoleA.GetDataUpdater(db, String.Format("{0} = ?", item.FullDBName), null, parameters);
                        DataTable dt = new DataTable();
                        du.Fill(ref dt);
                        foreach (DataRow row in dt.Rows)
                        {
                            row.SetAdded();
                            row[DataAttribute.IDColumnName] = DBNull.Value;
                            row[item.FullDBName] = pair.Value;
                        }
                        affectedRows += du.Update(ref dt);   
                    }
                    sbResults.AppendLine(String.Format("    Таблица \"{0}\", скопировано строк: {1} ", item.RoleA.OlapName, affectedRows));
                    if (listener != null)
                        listener(String.Format("    Таблица \"{0}\", скопировано строк: {1}", item.RoleA.OlapName, affectedRows));
                    
                }
            }
            copyResults = sbResults.ToString();
        }

        /// <summary>
        /// Копирование варианта
        /// </summary>
        /// <param name="variantID">ID варианта</param>
        /// <returns>ID копии варианта</returns>
        public int CopyVariant(int variantID)
        {
            return CopyVariant(variantID, null);
        }

        /// <summary>
        /// Копирование варианта
        /// </summary>
        /// <param name="variantID">ID варианта</param>
        /// <param name="listener"></param>
        /// <returns>ID копии варианта</returns>
        public int CopyVariant(int variantID, VariantListenerDelegate listener)
        {
            IDatabase db = SchemeClass.Instance.SchemeDWH.DB; 
            try
            {
                db.BeginTransaction();
                int newVariantID = CopyRow(db, variantID);

                string copyResults;

                CopyVariantData(db, variantID, newVariantID, listener, out copyResults);

                db.Commit();

                WriteIntoProtocol(ClassifiersEventKind.ceVariantCopy, String.Format("Выполнено копирование варианта ID={0}{2}. Cоздана копия (ID={1}). {3}Результаты копирования: {3}{4}",
                    variantID, newVariantID, GetVariantName(variantID), Environment.NewLine, copyResults));
                
                return newVariantID;
            }
            catch(Exception e)
            {
                db.Rollback();
                throw new Exception(e.Message, e);
            }
            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// Удаляет строку классификатора
        /// </summary>
        /// <param name="db">Объект доступа к базе данных</param>
        /// <param name="variantID">ID строки</param>
        private void DeleteVariantRow(IDatabase db, int variantID)
        {
            db.ExecQuery(String.Format("delete from {0} where {1} = ?", FullDBName, DataAttribute.IDColumnName),
                QueryResultTypes.NonQuery,
                db.CreateParameter(DataAttribute.IDColumnName, variantID));
        }

        /// <summary>
        /// Удаление данных варианта
        /// </summary>
        /// <param name="db">Объект доступа к базе данных</param>
        /// <param name="variantID">ID строки варианта источника</param>
        private void DeleteVariantData(IDatabase db, int variantID, VariantListenerDelegate listener, out string deleteResults)
        {
            //CheckAssocietedEntities();
            StringBuilder sbResults = new StringBuilder();
            foreach (EntityAssociation item in Associated.Values)
            {
                int affectedRows = Convert.ToInt32(db.ExecQuery(
                    String.Format("delete from {0} where {1} = ?", item.RoleA.FullDBName, item.FullDBName),
                    QueryResultTypes.NonQuery,
                    db.CreateParameter(item.FullDBName, variantID))); 
                
                if (listener != null)
                    listener(String.Format("Таблица \"{0}\", удалено строк: {1}", item.RoleA.OlapName, affectedRows));
                sbResults.AppendLine(String.Format("Таблица \"{0}\", удалено строк: {1}", item.RoleA.OlapName, affectedRows));
            }
            deleteResults = sbResults.ToString();
        }

        /// <summary>
        /// Удаление варианта и всех данных
        /// </summary>
        /// <param name="variantID">ID варианта</param>
        public void DeleteVariant(int variantID)
        {
            DeleteVariant(variantID, null);
        }

        /// <summary>
        /// Удаление варианта и всех данных
        /// </summary>
        /// <param name="variantID">ID варианта</param>
        /// <param name="listener"></param>
        public void DeleteVariant(int variantID, VariantListenerDelegate listener)
        {
            IDatabase db = SchemeClass.Instance.SchemeDWH.DB;
            try
            {
                db.BeginTransaction();

                UpdateVariantState(variantID, 0);

                string deleteResults;

                DeleteVariantData(db, variantID, listener, out deleteResults);

                string variantName = GetVariantName(variantID);
                DeleteVariantRow(db, variantID);

                db.Commit();

                WriteIntoProtocol(ClassifiersEventKind.ceVariantDelete, String.Format("Вариант ID={0}{1} удален. {2}Результаты удаления:{2}{3}",
                    variantID, variantName, Environment.NewLine, deleteResults));
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


        private void UpdateVariantState(int variantID, int completed)
        {
            IDatabase db = SchemeClass.Instance.SchemeDWH.DB;
            try
            {
                UpdateVariantState(db, variantID, completed);
            }
            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// Изменяет поле состояния варианта записи в базе данных.
        /// </summary>
        /// <param name="db">Объект доступа к базе данных.</param>
        /// <param name="variantID">ID варианта.</param>
        /// <param name="completed">Состояние варианта.</param>
        private void UpdateVariantState(IDatabase db, int variantID, int completed)
        {
            db.ExecQuery(String.Format("update {0} set {1} = ? where {2} = ?", FullDBName, DataAttribute.VariantCompletedColumnName, DataAttribute.IDColumnName),
                QueryResultTypes.NonQuery,
                db.CreateParameter(DataAttribute.VariantCompletedColumnName, completed),
                db.CreateParameter(DataAttribute.IDColumnName, variantID));
        }

        /// <summary>
        /// Блокировка(закрытие) варианта
        /// </summary>
        /// <param name="variantID">ID варианта</param>
        public void LockVariant(int variantID)
        {
            UpdateVariantState(variantID, 1);

            WriteIntoProtocol(ClassifiersEventKind.ceVariantLock, String.Format("Вариант ID={0}{1} закрыт от изменений", variantID, GetVariantName(variantID)));
        }

        /// <summary>
        /// Открытие варианта
        /// </summary>
        /// <param name="variantID">ID варианта</param>
        public void UnlockVariant(int variantID)
        {
            UpdateVariantState(variantID, 0);

            WriteIntoProtocol(ClassifiersEventKind.ceVariantUnlok, String.Format("Вариант ID={0}{1} открыт для изменений", variantID, GetVariantName(variantID)));
        }

        #endregion Реализация интерфейса
    }
}
