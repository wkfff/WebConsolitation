using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.Server.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Scheme.Classes
{
    /// <summary>
    /// Ассоциации сопоставления с классификатором Расходы.Базовый
    /// </summary>
    internal class BridgeAssosiationReport : BridgeAssociation, IBridgeAssociationReport
    {
        public BridgeAssosiationReport(string key, ServerSideObject owner, string semantic, string name, ServerSideObjectStates state)
            : base(key, owner, semantic, name, state)
        {
            CheckOwnerTypes(owner);
        }

        private static void CheckOwnerTypes(ServerSideObject owner)
        {
            if (owner == null)
            {
                throw new ArgumentNullException("owner", "Не указан владелец.");
            }
        }

        protected override string GetGluedString(DataRow dataRow, AssociateRule rule, bool isBridge, bool withConversionTable)
        {
            string glued =  base.GetGluedString(dataRow, rule, isBridge, withConversionTable);
            if (this is BridgeAssosiationReportItSelf)
            {
                return glued;
            }

            if (!withConversionTable)
            {
                if (isBridge)
                    glued += dataRow["HierarchyLevel"];
                else
                    glued += CorrectionLevel(Convert.ToInt32(dataRow["HierarchyLevel"]) - 1);
            }

            return glued;
        }

        public override int FormBridgeClassifier(int dataSourceID, int bridgeSourceID)
        {
            Database db = (Database)SchemeClass.Instance.SchemeDWH.DB;
            int recodsAffected = base.FormBridgeClassifier(dataSourceID, bridgeSourceID);

            try
            {
                if (!IsMonthRep() && !IsThisRefRBridge)
                {
                    ClearB_R_BridgeRep(db);
                    ClearBridgeDup(db);
                }

                AddIdToDublicateName(db, bridgeSourceID);
            }
            finally
            {
                db.Dispose();
            }

            return recodsAffected;
        }

        /// <summary>
        /// Удаление записей с незаполненным Code, удаление дубликатов при формировании классификатора Расходы.Базовый
        /// </summary>
        /// <param name="db"></param>
        private void ClearB_R_BridgeRep(Database db)
        {
            // Удаление записей с незаполненным Code, исключ ая несопоставленные записи и запись Неуказанная ведомственная статья
            RoleB.DeleteData(String.Format("where Code = 0 and ID <> -1 and SourceID = {0} and ParentID is null", formSourceID), true);
            // В SQL Server при удалении родительской записи в сопоставимом для подчиненных
            // записей проставляется parentID = -1. Для Расходы.Базовый такой вариант не подходит
            db.ExecQuery("update B_R_BridgeRep set ParentID = null where ParentID = -1 and SourceID = ?",
                QueryResultTypes.NonQuery, db.CreateParameter("sourceID", formSourceID));
            /*
            //удаляем дубликаты
            db.ExecQuery(
                "delete from B_R_BridgeRep t1 where (t1.ID not in (select max(t2.ID) from B_R_BridgeRep t2 group by t2.Code )) AND (t1.ID not in (select max(t2.ID) from B_R_BridgeRep t2 group by UPPER(t2.Name)))",
                QueryResultTypes.NonQuery);*/
        }

        private void ClearBridgeDup(IDatabase db)
        {
            DataTable dt = new DataTable();
            using (IDataUpdater updater = RoleBridge.GetDataUpdater("SourceID = ?", null, db.CreateParameter("sourceID", formSourceID)))
            {
                updater.Fill(ref dt);
                foreach (DataRow row in dt.Rows)
                {
                    row["HierarchyLevel"] = GetHierarchyLevel(row);
                }
                updater.Update(ref dt);
            }
            
            //dt.AcceptChanges();

            List<object> delRowsID = new List<object>();
            Dictionary<object, object> updateRows = new Dictionary<object, object>();
            dt.BeginLoadData();

            for (int i = 0; i <= 6; i++)
            {
                DataRow[] rows = dt.Select(string.Format("HierarchyLevel = {0}", i));
                foreach (DataRow row in rows)
                {
                    if (row.RowState != DataRowState.Deleted)
                    {
                        db.ExecQuery(string.Format("update {0} set HierarchyLevel  = {1} where ID = {2}", RoleBridge.FullDBName, i, row["ID"]),
                            QueryResultTypes.NonQuery);
                        DataRow[] delRows = row.IsNull("ParentID") ?
                            dt.Select(string.Format("Code = {0} and HierarchyLevel = {1} and ID <> {2} and ParentID is Null",
                            row["Code"], row["HierarchyLevel"], row["ID"]))
                            : dt.Select(string.Format("Code = {0} and HierarchyLevel = {1} and ID <> {2} and ParentID = {3}",
                            row["Code"], row["HierarchyLevel"], row["ID"], row["ParentID"]));
                        foreach (DataRow delRow in delRows)
                        {
                            if (delRow.RowState == DataRowState.Deleted)
                                continue;
                            DataRow[] chldRows = dt.Select(string.Format("ParentID = {0}", delRow["ID"]));
                            foreach (DataRow chldRow in chldRows)
                            {
                                chldRow["ParentID"] = row["ID"];
                                updateRows.Add(chldRow["ID"], row["ID"]);
                            }

                            db.ExecQuery(string.Format("update {0} set {1} = {2} where ID = {3}", RoleData.FullDBName, FullDBName, row["ID"], delRow["ID"]),
                                QueryResultTypes.NonQuery);

                            delRowsID.Add(delRow["ID"]);
                            delRow.Delete();
                        }
                    }
                }
            }
            dt.EndLoadData();

            foreach (KeyValuePair<object, object> kvp in updateRows)
            {
                db.ExecQuery(string.Format("update B_R_BridgeRep set ParentID = {0} where ID = {1}", kvp.Value, kvp.Key), QueryResultTypes.NonQuery);
            }
            foreach (object delID in delRowsID)
            {
                RoleB.DeleteData(String.Format("where ID = {0}", delID), true);
            }
        }

        /// <summary>
        /// Метод получения уровня иерархии без поля HierarchyLevel
        /// После добавления можно будет убрать
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private static int GetHierarchyLevel(DataRow row)
        {
            int level = 1;
            while (!row.IsNull("ParentID"))
            {
                DataRow[] rows = row.Table.Select(string.Format("id = {0}", row["ParentID"]));
                if (rows.Length != 0)
                {
                    row = rows[0];
                    level++;
                }
                else
                    break;
            }
            return level;
        }

        public override int CopyAndAssociateRow(int rowID, int bridgeSourceID)
        {
            Database db = (Database) SchemeClass.Instance.SchemeDWH.DB;
            DataTable dt = GetSourceRows(rowID, true, db);
            DataRow row = dt.Rows[0];
            if (row.IsNull("ParentID") &&  !(this is BridgeAssosiationReportItSelf))
            {
                throw new Exception(String.Format("В классификатор Расходы.Базовый запрещено переносить записи верхнего уровня"));
            }

            try
            {
                return CopyRow(rowID, db, bridgeSourceID);
            }
            finally
            {
                db.Dispose();
            }
        }

        public int CopyAndAssociateRow(int rowID, Database db, int bridgeSourceID)
        {
            return CopyRow(rowID, db, bridgeSourceID);
        }

        private int CopyRow(int rowID, Database db, int bridgeSourceID)
        {
            DataTable dt = GetSourceRows(rowID, true, db);

            DataRow row = dt.Rows[0];
            DataTable dtParent = new DataTable();
            bool existRow = false;
            if (!row.IsNull("ParentID"))
            {
                dtParent =
                    (DataTable)
                    db.ExecQuery(GetRoleDataSQLString(row, true),
                                 QueryResultTypes.DataTable);
                if (dtParent.Rows.Count != 0)
                    existRow = true;
            }
            else if (this is BridgeAssosiationReportItSelf)
            {
                dtParent =
                    (DataTable)
                    db.ExecQuery(GetRoleDataSQLString(row, false),
                                 QueryResultTypes.DataTable);
                dtParent.Rows[0]["HierarchyLevel"] = Convert.ToInt32(row["HierarchyLevel"]) - 1;
                existRow = false;
            }

            // Переносим сначала несопоставленные родительские записи
            if (existRow)
            {
                if ((dtParent.Rows[0][FullDBName].ToString() == "-1"
                     && Convert.ToInt32(dtParent.Rows[0]["HierarchyLevel"]) != 1)
                    || (dtParent.Rows[0][FullDBName].ToString() == "-1"
                        && this is BridgeAssosiationReportItSelf)/*Проверка что запись родителя несопоставлена*/)
                {
                    CopyRow(Convert.ToInt32(dtParent.Rows[0]["ID"]), db, bridgeSourceID);
                }
            }

            // проверка на существование такойже записи
           // проверка на существование такойже записи
            DataTable existTable = (DataTable) db.ExecQuery(string.Format(
                "select * from {0} bridgeCLS where bridgeCLS.Code = {1} and bridgeCLS.Name = ? and bridgeCLS.HierarchyLevel = {2} and bridgeCLS.sourceid = {3}",
                RoleBridge.FullDBName,
                (!IsMonthRep() && !IsThisRefRBridge && !IsBribgeRepRep())
                    ? row["repcode"].ToString()
                    : row["code"].ToString(),
                    !existRow ? 1 : CorrectionLevel(Convert.ToInt32(dtParent.Rows[0]["HierarchyLevel"])),
                    bridgeSourceID),QueryResultTypes.DataTable,
                    db.CreateParameter("Name", row["Name"].ToString()));                   
            int addedRowID;
            if (existTable.Rows.Count != 0)
            {
                // Устанавливаем соответствие
                Convert.ToInt32(db.ExecQuery(String.Format("update {0} set {1} = ? where ID = ?", RoleData.FullDBName, FullDBName), QueryResultTypes.NonQuery,
                                             db.CreateParameter("RefBridge", existTable.Rows[0]["ID"]), db.CreateParameter("ID", rowID)));

                return Convert.ToInt32(existTable.Rows[0]["ID"]);
            }

            addedRowID = base.CopyAndAssociateRow(rowID, bridgeSourceID);

            if (dtParent.Rows.Count != 0)
            {
                db.ExecQuery(string.Format("update {0} set HierarchyLevel = {1} where ID = {2}",
                                           RoleBridge.FullDBName,
                                           CorrectionLevel(Convert.ToInt32(dtParent.Rows[0]["HierarchyLevel"])),
                                           addedRowID), QueryResultTypes.NonQuery);

                string noAdminCode = (!IsMonthRep() && !IsThisRefRBridge && !IsBribgeRepRep())
                                         ? dtParent.Rows[0]["repcode"].ToString()
                                         : dtParent.Rows[0]["code"].ToString();
                object queryResult;
                queryResult = db.ExecQuery(string.Format(
                    "select bridgeCLS.id from {0} bridgeCLS, {1} dataCLS where bridgeCLS.Code = {2} and bridgeCLS.Name = dataCLS.Name and bridgeCLS.HierarchyLevel = {3} and bridgeCLS.SourceID = {4}",
                    RoleBridge.FullDBName, RoleData.FullDBName, noAdminCode,
                    CorrectionLevel(Convert.ToInt32(dtParent.Rows[0]["HierarchyLevel"]) - 1),
                    bridgeSourceID /*CorrectionLevel(GetHierarchyLevel(row) - 1)*/),
                                           QueryResultTypes.Scalar);
                if (queryResult != null)
                {
                    db.ExecQuery(string.Format("update {0} set ParentID = {1} where ID = {2}",
                                               RoleBridge.FullDBName, queryResult, addedRowID),
                                 QueryResultTypes.NonQuery);
                }
            }

            return addedRowID;
        }

        private string GetRoleDataSQLString(DataRow row, bool value)
        {
            int id = value ? Convert.ToInt32(row["ParentID"]) : Convert.ToInt32(row["ID"]);

            if (IsThisRefRBridge)
            {
                return string.Format("Select id, codestr, name, {0}, hierarchylevel from {1} where id = {2}",
                                     FullDBName, RoleData.FullDBName, id);
            }

            if (IsMonthRep())
            {
                return string.Format("Select id, code, name, {0}, hierarchylevel from {1} where id = {2}",
                                         FullDBName, RoleData.FullDBName, id);
            }

            if (IsKDASBudget())
            {
                return string.Format("Select id, codestr, name, {0}, repcode, hierarchylevel from {1} where id = {2}",
                                 FullDBName, RoleData.FullDBName, id);
            }

            if (IsBribgeRepRep())
            {
                return string.Format("Select id, code, name, {0}, hierarchylevel from {1} where id = {2}",
                     FullDBName, RoleData.FullDBName, id);
            }

            return string.Format("Select id, code, name, {0}, repcode, hierarchylevel from {1} where id = {2}",
                                 FullDBName, RoleData.FullDBName, id);
        }

        /// <summary>
        /// Корректировка уровня иерархии для Расходы.МесОтч
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        protected int CorrectionLevel(int i)
        {
            if (this is BridgeAssosiationReportItSelf)
                return i + 1;

            if (IsMonthRep() || IsThisRefRBridge)
                return i + 1;

            return i;
        }

        /// <summary>
        /// Расходы.МесОтч
        /// </summary>
        /// <returns></returns>
        private bool IsMonthRep()
        {
            return RoleData.ObjectKey.Equals(SchemeClassKeys.rMonthRep);
        }

        /// <summary>
        /// Ассоциация Расходы.АС Бюджет -> Расходы.Сопоставимый
        /// Ассоциация Расходы.АС Бюджет -> Расходы.Сопоставимый Планирование
        /// </summary>
        private bool IsThisRefRBridge
        {
            get
            {
                return this.ObjectKey == SchemeClassKeys.refRBridge
                       || this.ObjectKey == SchemeClassKeys.refRBridgePlan;
            }
        }

        private bool IsKDASBudget()
        {
            return RoleData.ObjectKey.Equals(SchemeClassKeys.KDASBudget);
        }

        private bool IsBribgeRepRep()
        {
            return ObjectKey == "b85bfa11-a5ee-4721-9c60-3f72c00fe6c9";
        }

        protected override List<string> GetAttributeNames(IAssociateRule rule)
        {
            List<string> list = base.GetAttributeNames(rule);
            list.Add("ParentID");
            list.Add("HierarchyLevel");
            return list;
        }

        #region IBridgeAssociationReport Members

        /// <summary>
        /// Добавление в Расходы.Базовый всех несопоставленных записей
        /// </summary>
        public void CopyAndAssociateAllRow(int sourceID, int bridgeSourceID)
        {
            Database db = (Database)SchemeClass.Instance.SchemeDWH.DB;
            try
            {
                // находим все листовые записи
                DataTable table =
                    (DataTable)SchemeClass.Instance.SchemeDWH.DB.ExecQuery(
                        String.Format("select * from {0} where HierarchyLevel = (select max(HierarchyLevel) from {1}) and {2} = -1 and sourceID = {3}",
                                      RoleData.FullDBName, RoleData.FullDBName, FullDBName, sourceID), QueryResultTypes.DataTable);
                // если запись несопоставлена - вызываем процедур добавления записи в Расходы.Базовый
                foreach (DataRow dataRow in table.Rows)
                {
                    CopyAndAssociateRow(Convert.ToInt32(dataRow["ID"]), db, bridgeSourceID);
                }

                AddIdToDublicateName(db, bridgeSourceID);
            }
            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// Метод добавляет ID записи к именам-дубликатам. Необходимо для 
        /// операции обратной записи. Метод вызываем после формирования сопоставимого
        /// и добавления в него записей
        /// </summary>
        /// <param name="db"></param>
        private static void AddIdToDublicateName(Database db, int bridgeSourceID)
        {
            if (SchemeClass.Instance.SchemeDWH.FactoryName == ProviderFactoryConstants.MSOracleDataAccess)
            {
                db.ExecQuery(
                    String.Format("update b_r_bridgerep t1 set t1.name = concat(t1.id, t1.name) where t1.sourceid = {0} and exists (select null from b_r_bridgerep t2 where t1.name = t2.name and t1.sourceid = t2.sourceid and t1.id <> t2.id)", bridgeSourceID),
                    QueryResultTypes.NonQuery);
            }
            else
            {
                db.ExecQuery(
                    String.Format("update b_r_bridgerep set name = id + name where sourceid = {0} and exists (select null from b_r_bridgerep t2 where name = t2.name and sourceid = t2.sourceid and id <> t2.id)", bridgeSourceID),
                    QueryResultTypes.NonQuery);
            }
        }

        #endregion
    }

    internal class BridgeAssosiationReportItSelf : BridgeAssosiationReport
    {
        public BridgeAssosiationReportItSelf(string key, ServerSideObject owner, string semantic, string name, ServerSideObjectStates state) 
            : base(key, owner, semantic, name, state)
        {
            this.AssociationClassType = AssociationClassTypes.BridgeBridge;
        }
    }
}
