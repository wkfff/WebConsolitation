using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DisintRules
{
    /// <summary>
    /// класс для работы с нормативами в процессе расщепления данных
    /// </summary>
    internal class NormativesService
    {

        private IScheme scheme;

        private DataTable dtNormativesBK;

        private DataTable dtNormativesRF;

        private DataTable dtNormativesMR;

        private DataTable dtNormativesVarRF;

        private DataTable dtNormativesVarMR;

        private int sourceId;

        internal NormativesService(IScheme scheme, int year)
        {
            this.scheme = scheme;

            // получение нормативов
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                dtNormativesBK = InnerGetNormativesForSplit(NormativesKind.NormativesBK, year, db);
                dtNormativesRF = InnerGetNormativesForSplit(NormativesKind.NormativesRegionRF, year, db);
                dtNormativesMR = InnerGetNormativesForSplit(NormativesKind.NormativesMR, year, db);
                dtNormativesVarRF = InnerGetNormativesForSplit(NormativesKind.VarNormativesRegionRF, year, db);
                dtNormativesVarMR = InnerGetNormativesForSplit(NormativesKind.VarNormativesMR, year, db);

                object queryResult = db.ExecQuery(
                    "select id from DataSources where SupplierCode = 'ФО' and DataCode = 6 and DataName = 'Анализ данных' and Year = ?",
                    QueryResultTypes.Scalar, new DbParameterDescriptor("p0", year));
                if (queryResult != null && queryResult != DBNull.Value)
                    sourceId = Convert.ToInt32(queryResult);
            }
        }

        #region получение нормативов для расщепления

        /// <summary>
        /// получение нормативов в виде, удобном для расщепления по ним данных
        /// </summary>
        /// <param name="normatives"></param>
        /// <param name="year"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        private DataTable InnerGetNormativesForSplit(NormativesKind normatives, int year, IDatabase db)
        {
            // создаем таблицу для хранения нормативов в том виде, в котором они будут отправляться на клиент
            DataTable dtNormatives = CreateNewNormativesTable(normatives);
            // получаем нормативы из таблицы фактов
            DataTable crudeNormatives = GetCrudeNormatives(normatives, year, db);
            // если нормативов нету, выходим
            if (crudeNormatives == null) return dtNormatives;
            if (crudeNormatives.Rows.Count == 0) return dtNormatives;
            crudeNormatives.BeginLoadData();
            // преобразуем нормативы в тот вид, который можно использовать для расщепления
            Dictionary<string, NormativeReferences> references = new Dictionary<string, NormativeReferences>();
            // получим разные наборы ссылок на КД и год
            foreach (DataRow row in crudeNormatives.Select(string.Empty, string.Empty))
            {
                int refKD = Convert.ToInt32(row["RefKD"]);
                int refYearDayUNV = Convert.ToInt32(row["RefYearDayUNV"].ToString().Substring(0, 4).PadRight(8, '0'));
                int refRegions = row.IsNull("RefRegions") ? -1 : Convert.ToInt32(row["RefRegions"]);
                string kdCode = row["Code"].ToString();
                string key = string.Format("{0}_{1}_{2}", refKD, refYearDayUNV, refRegions);
                if (!references.ContainsKey(key))
                    references.Add(key, new NormativeReferences(refKD, kdCode, refYearDayUNV, refRegions));
            }

            Dictionary<int, object> values = new Dictionary<int, object>();
            Dictionary<int, int> ids = new Dictionary<int, int>();

            foreach (NormativeReferences reference in references.Values)
            {
                DataRow[] newRow = null;
                if (reference.RefRegions != -1)
                    newRow = crudeNormatives.Select(string.Format("RefKD = {0} and RefYearDayUNV = {1} and RefRegions = {2}",
                        reference.RefKD, reference.RefYearDayUNV, reference.RefRegions));
                else
                    newRow = crudeNormatives.Select(string.Format("RefKD = {0} and RefYearDayUNV = {1} and RefRegions is null",
                        reference.RefKD, reference.RefYearDayUNV));
                foreach (DataRow row in newRow)
                {
                    if (!values.ContainsKey(Convert.ToInt32(row["RefBudLevel"])))
                    {
                        values.Add(Convert.ToInt32(row["RefBudLevel"]), row["value"]);
                        ids.Add(Convert.ToInt32(row["RefBudLevel"]), Convert.ToInt32(row["ID"]));
                    }
                }
                AddNewRow(dtNormatives, db, reference.RefKD,
                    reference.KDCode,
                    reference.RefYearDayUNV,
                    reference.RefRegions, values, ids);
                values.Clear();
                ids.Clear();
            }

            crudeNormatives.EndLoadData();

            FillAutofilledColumns(dtNormatives, normatives, db);

            dtNormatives.AcceptChanges();
            return dtNormatives;
        }

        /// <summary>
        /// добавление записи в общую таблицу нормативов
        /// </summary>
        /// <param name="normatives"></param>
        /// <param name="db"></param>
        /// <param name="refKD"></param>
        /// <param name="refYearDayUNV"></param>
        /// <param name="refRegions"></param>
        /// <param name="values"></param>
        /// <param name="ids"></param>
        private void AddNewRow(DataTable normatives, IDatabase db, int refKD, string kdCode, int refYearDayUNV, int refRegions, Dictionary<int, object> values, Dictionary<int, int> ids)
        {
            DataRow newRow = normatives.NewRow();
            newRow.BeginEdit();
            newRow["ID"] = normatives.Rows.Count;
            newRow["RefKD"] = refKD;
            if (newRow.Table.Columns.Contains("KDCode"))
                newRow["KDCode"] = kdCode;
            newRow["RefYearDayUNV"] = refYearDayUNV;
            if (newRow.Table.Columns.Contains("RefRegions"))
                newRow["RefRegions"] = refRegions;

            // добавляем значения по нормативам
            foreach (KeyValuePair<int, object> value in values)
            {
                newRow[Convert.ToString(value.Key) + NormativesObjectKeys.VALUE_POSTFIX] = value.Value;
                if (newRow.Table.Columns.Contains(value.Key.ToString()))
                    newRow[Convert.ToString(value.Key)] = ids[value.Key];
            }
            newRow.EndEdit();
            normatives.Rows.Add(newRow);
        }

        /// <summary>
        /// получение и заполнение зависимостей в нормативах от нормативов более высокого уровня
        /// </summary>
        /// <param name="normatives"></param>
        /// <param name="normativesKind"></param>
        /// <param name="db"></param>
        private void FillAutofilledColumns(DataTable normatives, NormativesKind normativesKind, IDatabase db)
        {
            normatives.BeginLoadData();
            foreach (DataRow row in normatives.Rows)
            {
                row.BeginEdit();
                int refKD = Convert.ToInt32(row["RefKD"]);
                int refYearDayUNV = Convert.ToInt32(row["RefYearDayUNV"]);
                int refRegions = Convert.ToInt32(row["RefRegions"]);
                switch (normativesKind)
                {
                    case NormativesKind.NormativesBK:
                        return;
                    case NormativesKind.NormativesRegionRF:
                        NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 1, db);
                        NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 2, db);

                        break;
                    case NormativesKind.NormativesMR:
                        NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 1, db);
                        NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 2, db);
                        NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 3, db);
                        NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 14, db);
                        NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 15, db);
                        NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 4, db);
                        break;
                    case NormativesKind.VarNormativesRegionRF:
                        NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 1, db);
                        NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 2, db);
                        NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 15, db);
                        NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 5, db);
                        NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 6, db);
                        NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 16, db);
                        NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 17, db);
                        break;
                    case NormativesKind.VarNormativesMR:
                        NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 1, db);
                        NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 2, db);
                        NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 3, db);
                        NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 14, db);
                        NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 15, db);
                        NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 4, db);
                        NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 6, db);
                        NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 16, db);
                        NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 17, db);
                        break;
                }
                // для всех одинаковые поля...
                NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 7, db);
                NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 8, db);
                NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 9, db);
                NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 10, db);
                NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 11, db);
                NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 12, db);
                NewFillCellValue(row, normativesKind, refKD, refYearDayUNV, 13, db);
                row.EndEdit();
            }
            normatives.EndLoadData();
        }

        /// <summary>
        /// заполнения поля - ссылки на другой норматив в нормативах для расщепления
        /// </summary>
        /// <param name="row"></param>
        /// <param name="normative"></param>
        /// <param name="refKD"></param>
        /// <param name="refYearDayUNV"></param>
        /// <param name="refBudLevel"></param>
        private void NewFillCellValue(DataRow row, NormativesKind normative, int refKD, int refYearDayUNV, int refBudLevel, IDatabase db)
        {
            decimal cellValue = GetConsRegionBudget(normative, refKD, refYearDayUNV, refBudLevel, db);
            string dataValueColumnName = string.Format("{0}{1}", refBudLevel, NormativesObjectKeys.VALUE_POSTFIX);
            string filledValueColumnName = string.Format("{0}{1}", refBudLevel, NormativesObjectKeys.REF_VALUE_POSTFIX);
            string selfValueColumn = string.Format("{0}{1}", refBudLevel, NormativesObjectKeys.SELF_VALUE_POSTFIX);

            if (row.Table.Columns.Contains(selfValueColumn))
            {
                if (cellValue >= 0)
                {
                    row[filledValueColumnName] = cellValue;
                    row[dataValueColumnName] = Convert.ToDecimal(row[dataValueColumnName]) + cellValue;
                }
                else
                {
                    row[filledValueColumnName] = 0;
                    row[dataValueColumnName] = Convert.ToDecimal(row[dataValueColumnName]);
                }
            }
            else
            {
                if (cellValue >= 0)
                    row[dataValueColumnName] = cellValue;
                else
                    row[dataValueColumnName] = 0;
            }
        }

        /// <summary>
        /// получает ID родительской записи
        /// </summary>
        /// <param name="refKD"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        private int GetParentRefKD(int refKD, IDatabase db)
        {
            string selectRefParentQuery = "select ParentID from d_KD_Analysis where ID = ?";
            IDbDataParameter param = db.CreateParameter("ID", refKD);
            object result = db.ExecQuery(selectRefParentQuery, QueryResultTypes.Scalar, param);
            if (result != DBNull.Value)
                return Convert.ToInt32(result);
            return -1;
        }

        private const string normativeRegionQuery =
            @"select norm.id, norm.RefKD, norm.RefRegions, norm.RefYearDayUNV, norm.RefBudLevel, norm.value, kd.CodeStr as Code
            from {0} norm, d_KD_Analysis kd where RefYearDayUNV like ? and norm.RefKD = kd.ID";
        private const string normativeQuery =
            @"select norm.id, norm.RefKD, norm.RefYearDayUNV, norm.RefBudLevel, norm.value, kd.CodeStr as Code 
            from {0} norm, d_KD_Analysis kd where RefYearDayUNV like ? and norm.RefKD = kd.ID";
        /// <summary>
        /// возвращает нормативы из таблицы фактов
        /// </summary>
        private DataTable GetCrudeNormatives(NormativesKind normatives, int year, IDatabase db)
        {
            string yearStr = string.Format("{0}____", year);
            DataTable dtCrudeNormatives = null;
            switch (normatives)
            {
                case NormativesKind.NormativesBK:
                    dtCrudeNormatives = (DataTable)db.ExecQuery(string.Format(normativeQuery, "f_Norm_BK"),
                        QueryResultTypes.DataTable, new DbParameterDescriptor("p0", yearStr));
                    break;
                case NormativesKind.NormativesRegionRF:
                    dtCrudeNormatives = (DataTable)db.ExecQuery(string.Format(normativeQuery, "f_Norm_Region"),
                        QueryResultTypes.DataTable, new DbParameterDescriptor("p0", yearStr));
                    break;
                case NormativesKind.NormativesMR:
                    dtCrudeNormatives = (DataTable)db.ExecQuery(string.Format(normativeQuery, "f_Norm_MR"),
                        QueryResultTypes.DataTable, new DbParameterDescriptor("p0", yearStr));
                    break;
                case NormativesKind.VarNormativesRegionRF:
                    dtCrudeNormatives = (DataTable)db.ExecQuery(string.Format(normativeRegionQuery, "F_NORM_VARIEDREGION"),
                        QueryResultTypes.DataTable, new DbParameterDescriptor("p0", yearStr));
                    break;
                case NormativesKind.VarNormativesMR:
                    dtCrudeNormatives = (DataTable)db.ExecQuery(string.Format(normativeRegionQuery, "F_NORM_VARIEDMR"),
                        QueryResultTypes.DataTable, new DbParameterDescriptor("p0", yearStr));
                    break;
            }
            if (dtCrudeNormatives != null && !dtCrudeNormatives.Columns.Contains("RefRegions"))
            {
                DataColumn column = dtCrudeNormatives.Columns.Add("RefRegions");
                column.DefaultValue = -1;
            }
            return dtCrudeNormatives;
        }

        public decimal GetConsRegionBudget(NormativesKind normative, int refKD, int refYearDayUNV, int refBudLevel, IDatabase db)
        {
            decimal value = -1;
            value = GetRegionBudgetValue(normative, refKD, refBudLevel);
            while (value == -1 && GetParentRefKD(refKD, db) != -1)
            {
                refKD = GetParentRefKD(refKD, db);
                value = GetRegionBudgetValue(normative, refKD, refBudLevel);
            }
            return value;
        }

        private decimal GetRegionBudgetValue(NormativesKind normative, int refKD, int refBudLevel)
        {
            DataRow[] values = null;
            switch (normative)
            {
                case NormativesKind.NormativesRegionRF:
                    values = dtNormativesBK.Select(string.Format("RefKD = {0}", refKD));
                    break;
                case NormativesKind.NormativesMR:
                    values = dtNormativesRF.Select(string.Format("RefKD = {0}", refKD));
                    if (values == null || values.Length == 0)
                    {
                        values = dtNormativesBK.Select(string.Format("RefKD = {0}", refKD));
                    }
                    break;
                case NormativesKind.VarNormativesRegionRF:
                case NormativesKind.VarNormativesMR:
                    values = dtNormativesMR.Select(string.Format("RefKD = {0}", refKD));
                    if (values == null || values.Length == 0)
                    {
                        values = dtNormativesRF.Select(string.Format("RefKD = {0}", refKD));
                        if (values == null || values.Length == 0)
                        {
                            values = dtNormativesBK.Select(string.Format("RefKD = {0}", refKD));
                        }
                    }
                    break;
            }
            if (values == null || values.Length == 0)
                return -1;

            return Convert.ToDecimal(values[0][string.Format("{0}{1}", refBudLevel, NormativesObjectKeys.VALUE_POSTFIX)]);
        }

        /// <summary>
        /// создает общую таблицу для получения нормативов
        /// </summary>
        /// <param name="normatives"></param>
        /// <returns></returns>
        private DataTable CreateNewNormativesTable(NormativesKind normatives)
        {
            DataTable table = new DataTable();
            // поля под хранение ссылок на классификаторы
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("RefKD", typeof(int));
            table.Columns.Add("KDCode", typeof(string));
            table.Columns.Add("RefYearDayUNV", typeof(int));
            table.Columns.Add("RefRegions", typeof(int));
            // поля под хранение значений нормативов
            DataColumn column = table.Columns.Add("1", typeof(int));
            column = table.Columns.Add("1" + NormativesObjectKeys.VALUE_POSTFIX, typeof(decimal));
            column.DefaultValue = 0;

            column = table.Columns.Add("2" + NormativesObjectKeys.VALUE_POSTFIX, typeof(decimal));
            column.DefaultValue = 0;

            column = table.Columns.Add("3" + NormativesObjectKeys.VALUE_POSTFIX, typeof(decimal));
            column.DefaultValue = 0;

            column = table.Columns.Add("14" + NormativesObjectKeys.VALUE_POSTFIX, typeof(decimal));
            column.DefaultValue = 0;

            if (normatives == NormativesKind.VarNormativesRegionRF)
            {
                column = table.Columns.Add("15" + NormativesObjectKeys.SELF_VALUE_POSTFIX, typeof(decimal));
                column.DefaultValue = 0;
                column = table.Columns.Add("15" + NormativesObjectKeys.REF_VALUE_POSTFIX, typeof(decimal));
                column.DefaultValue = 0;
            }
            column = table.Columns.Add("15" + NormativesObjectKeys.VALUE_POSTFIX, typeof(decimal));
            column.DefaultValue = 0;

            column = table.Columns.Add("4" + NormativesObjectKeys.VALUE_POSTFIX, typeof(decimal));
            column.DefaultValue = 0;

            if (normatives == NormativesKind.VarNormativesRegionRF)
            {
                column = table.Columns.Add("5" + NormativesObjectKeys.SELF_VALUE_POSTFIX, typeof(decimal));
                column.DefaultValue = 0;
                column = table.Columns.Add("5" + NormativesObjectKeys.REF_VALUE_POSTFIX, typeof(decimal));
                column.DefaultValue = 0;
            }
            column = table.Columns.Add("5" + NormativesObjectKeys.VALUE_POSTFIX, typeof(decimal));
            column.DefaultValue = 0;


            if (normatives == NormativesKind.VarNormativesRegionRF || normatives == NormativesKind.VarNormativesMR)
            {
                column = table.Columns.Add("6" + NormativesObjectKeys.SELF_VALUE_POSTFIX, typeof(decimal));
                column.DefaultValue = 0;
                column = table.Columns.Add("6" + NormativesObjectKeys.REF_VALUE_POSTFIX, typeof(decimal));
                column.DefaultValue = 0;
            }
            column = table.Columns.Add("6" + NormativesObjectKeys.VALUE_POSTFIX, typeof(decimal));
            column.DefaultValue = 0;

            if (normatives == NormativesKind.VarNormativesRegionRF || normatives == NormativesKind.VarNormativesMR)
            {
                column = table.Columns.Add("16" + NormativesObjectKeys.SELF_VALUE_POSTFIX, typeof(decimal));
                column.DefaultValue = 0;
                column = table.Columns.Add("16" + NormativesObjectKeys.REF_VALUE_POSTFIX, typeof(decimal));
                column.DefaultValue = 0;
            }
            column = table.Columns.Add("16" + NormativesObjectKeys.VALUE_POSTFIX, typeof(decimal));
            column.DefaultValue = 0;

            if (normatives == NormativesKind.VarNormativesRegionRF || normatives == NormativesKind.VarNormativesMR)
            {
                column = table.Columns.Add("17" + NormativesObjectKeys.SELF_VALUE_POSTFIX, typeof(decimal));
                column.DefaultValue = 0;
                column = table.Columns.Add("17" + NormativesObjectKeys.REF_VALUE_POSTFIX, typeof(decimal));
                column.DefaultValue = 0;
            }

            column = table.Columns.Add("17" + NormativesObjectKeys.VALUE_POSTFIX, typeof(decimal));
            column.DefaultValue = 0;

            column = table.Columns.Add("7" + NormativesObjectKeys.VALUE_POSTFIX, typeof(decimal));
            column.DefaultValue = 0;

            column = table.Columns.Add("8" + NormativesObjectKeys.VALUE_POSTFIX, typeof(decimal));
            column.DefaultValue = 0;

            column = table.Columns.Add("9" + NormativesObjectKeys.VALUE_POSTFIX, typeof(decimal));
            column.DefaultValue = 0;

            column = table.Columns.Add("10" + NormativesObjectKeys.VALUE_POSTFIX, typeof(decimal));
            column.DefaultValue = 0;

            column = table.Columns.Add("11" + NormativesObjectKeys.VALUE_POSTFIX, typeof(decimal));
            column.DefaultValue = 0;

            column = table.Columns.Add("12" + NormativesObjectKeys.VALUE_POSTFIX, typeof(decimal));
            column.DefaultValue = 0;

            column = table.Columns.Add("13" + NormativesObjectKeys.VALUE_POSTFIX, typeof(decimal));
            column.DefaultValue = 0;
            return table;
        }

        #endregion

        #region преобразование ссылок на классификаторы в коды

        /// <summary>
        /// ищем соответствие между классификаторами КД анализ и КД планирование
        /// </summary>
        /// <param name="refKD"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        private List<string> GetRefKDNormative(int refKD, IDatabase db)
        {
            // получим ID источника Анализ данных по текущему году
            List<string> KDCodes = new List<string>();
            string selectQuery = string.Format("select CodeStr from D_KD_PLANINCOMES where ID = {0}", refKD);
            DataTable classifierRow = (DataTable)db.ExecQuery(selectQuery, QueryResultTypes.DataTable);
            string code = classifierRow.Rows[0]["CodeStr"].ToString();
            selectQuery = "select ID, CodeStr, ParentID from d_KD_Analysis where CodeStr = ? and SourceId = ?";
            IDbDataParameter param = db.CreateParameter("CodeStr", code);
            DataTable dt = (DataTable)db.ExecQuery(selectQuery, QueryResultTypes.DataTable,
                new DbParameterDescriptor("p0", code),
                new DbParameterDescriptor("p1", sourceId));
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    KDCodes.Add(row["CodeStr"].ToString());
                    while (true)
                    {
                        if (dt.Rows[0]["ParentID"] == DBNull.Value)
                            break;
                        string getParentRowId = string.Format("select ID, CodeStr, ParentID from d_KD_Analysis where ID = {0}", dt.Rows[0]["ParentID"]);
                        dt = (DataTable)db.ExecQuery(getParentRowId, QueryResultTypes.DataTable);
                        if (dt.Rows.Count == 0)
                            break;
                        KDCodes.Add(dt.Rows[0]["CodeStr"].ToString());
                    }
                }
            }
            else
            {
                string noProgrammCode = GetNoProgrammKDCode(code);
                selectQuery = "select ID, CodeStr, ParentID from d_KD_Analysis where CodeStr = ? and SourceId = ?";
                dt = (DataTable)db.ExecQuery(selectQuery, QueryResultTypes.DataTable,
                    new DbParameterDescriptor("p0", noProgrammCode),
                new DbParameterDescriptor("p1", sourceId));
                if (dt.Rows.Count > 0)
                    KDCodes.Add(dt.Rows[0]["CodeStr"].ToString());
            }
            if (KDCodes.Count == 0)
            {
                string noAdministratorCode = GetNoAdministratorCode(code);
                selectQuery = "select ID, CodeStr, ParentID from d_KD_Analysis where CodeStr LIKE ? and SourceId = ?";
                dt = (DataTable)db.ExecQuery(selectQuery, QueryResultTypes.DataTable,
                    new DbParameterDescriptor("p0", "%" + noAdministratorCode),
                new DbParameterDescriptor("p1", sourceId));
                if (dt.Rows.Count > 0)
                    KDCodes.Add(dt.Rows[0]["CodeStr"].ToString());

            }
            return KDCodes;
        }

        private static string GetNoProgrammKDCode(string KDCode)
        {
            return KDCode.Substring(0, 13) + "0000" + KDCode.Substring(17, 3);
        }

        private static string GetNoAdministratorCode(string kdCode)
        {
            return kdCode.Substring(3, 17);
        }

        private int GetRegionAnalysisForPlan(int regionPlanId, int year, IDatabase db)
        {
            object queryResult = db.ExecQuery(
                @"select id from d_Regions_Analysis where RefBridgeRegions in (select RefBridge from d_Regions_Plan where id = ?) and
                SourceID in (select id from DataSources where year = ?)",
                QueryResultTypes.Scalar, new DbParameterDescriptor("p0", regionPlanId),
                new DbParameterDescriptor("p1", year));
            if (queryResult != DBNull.Value && queryResult != null)
                return Convert.ToInt32(queryResult);
            return -1;
        }

        #endregion

        #region

        /// <summary>
        /// поиск норматива среди загруженных
        /// </summary>
        public DataRow FindNormative(int refKd, int year, int refRegions, IDatabase db)
        {
            refRegions = GetRegionAnalysisForPlan(refRegions, year, db);
            string refYear = year.ToString().PadRight(8, '0');
            List<string> kdList = GetRefKDNormative(refKd, db);
            DataRow normativeRow = null;
            if (refRegions >= 0)
            {
                normativeRow = FindNormative(kdList, refYear, refRegions, NormativesKind.VarNormativesMR);
                if (normativeRow == null)
                    normativeRow = FindNormative(kdList, refYear, refRegions, NormativesKind.VarNormativesRegionRF);
            }
            if (normativeRow == null)
            {
                normativeRow = FindNormative(kdList, refYear, refRegions, NormativesKind.NormativesMR);
                if (normativeRow == null)
                    normativeRow = FindNormative(kdList, refYear, refRegions, NormativesKind.NormativesRegionRF);
                if (normativeRow == null)
                    normativeRow = FindNormative(kdList, refYear, refRegions, NormativesKind.NormativesBK);
            }

            return normativeRow;
        }

        /// <summary>
        /// поиск норматива среди загруженных
        /// </summary>
        public DataRow FindNormative(int refKd, int year, int refRegions, NormativesKind normatives, IDatabase db)
        {
            refRegions = GetRegionAnalysisForPlan(refRegions, year, db);
            string refYear = year.ToString().PadRight(8, '0');
            List<string> kdList = GetRefKDNormative(refKd, db);
            return FindNormative(kdList, refYear, refRegions, normatives);
        }

        private DataRow FindNormative(List<string> kdList, string refYear, int refRegions, NormativesKind normatives)
        {
            string query = "KDCode = {0} and RefYearDayUNV = {1}";
            string regionQuery = "RefRegions = {0} and KDCode = {1} and RefYearDayUNV = {2}";
            DataRow[] normative = null;
            foreach (string kd in kdList)
            {
                switch (normatives)
                {
                    case NormativesKind.NormativesBK:
                        normative = dtNormativesBK.Select(string.Format(query, kd, refYear));
                        break;
                    case NormativesKind.NormativesRegionRF:
                        normative = dtNormativesRF.Select(string.Format(query, kd, refYear));
                        break;
                    case NormativesKind.NormativesMR:
                        normative = dtNormativesMR.Select(string.Format(query, kd, refYear));
                        break;
                    case NormativesKind.VarNormativesRegionRF:
                        normative = dtNormativesVarRF.Select(string.Format(regionQuery, refRegions, kd, refYear));
                        break;
                    case NormativesKind.VarNormativesMR:
                        normative = dtNormativesVarMR.Select(string.Format(regionQuery, refRegions, kd, refYear));
                        break;
                }
                if (normative.Length != 0)
                    return normative[0];
            }
            return null;
        }

        #endregion

        #region перенос нормативов 

        internal void NormativeTransfert(string kdCode, string kdName, int variantId, int marksId, ref string messages)
        {
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                var warningMessages = new StringBuilder();
                DataTable dtYears =
                    (DataTable)db.ExecQuery(
                        "select distinct RefYearDayUNV from f_Fund_FO9Marks where RefMarks = ? and RefVariant = ?",
                        QueryResultTypes.DataTable,
                        new DbParameterDescriptor("p0", marksId),
                        new DbParameterDescriptor("p1", variantId));

                int normativeSourceId = GetDataSourceId(db, "ФО", 23, "Нормативы отчислений", 0);
                IEntity kdAnalizEntity = scheme.RootPackage.FindEntityByName(KdAnalizKey);

                foreach (DataRow yearRow in dtYears.Rows)
                {
                    int year = Convert.ToInt32(yearRow[0])/10000;
                    int sourceId = GetDataSourceIDByYear(year, db);
                    long kdId = GetKdAnalizId(kdCode, kdName, sourceId, db, kdAnalizEntity);
                    List<string> deletedRegions = new List<string>();

                    DataTable dtMarks = (DataTable)db.ExecQuery(
                        @"select marks.Norm, aRegion.ID, aRegion.RefTerr from f_Fund_FO9Marks marks, d_Regions_Analysis aRegion, d_Regions_Plan pRegion where
                        marks.RefMarks = ? and marks.RefVariant = ? and aRegion.SourceID = ? and
                        marks.RefRegions = pRegion.ID and pRegion.RefBridge = aRegion.RefBridgeRegions and pRegion.RefBridge <> -1 and
                        marks.RefYearDayUNV like ?",
                        QueryResultTypes.DataTable,
                        new DbParameterDescriptor("p0", marksId),
                        new DbParameterDescriptor("p1", variantId),
                        new DbParameterDescriptor("p2", sourceId),
                        new DbParameterDescriptor("p3", string.Format("{0}____", year)));
                    // переносим записи
                    if (dtMarks.Rows.Count == 0)
                    {
                        warningMessages.AppendLine(
                            string.Format(
                                "Перенос данных за {0} год не возможен. Проверьте заполнение и сопоставление классификаторов «КД.Анализ» и «Районы.Анализ» за {0} год",
                                year));
                    }
                    foreach (DataRow markRow in dtMarks.Rows)
                    {
                        int budLevel = GetBudLevel(markRow[2]);
                        if (budLevel == -1)
                            continue;

                        string paramsKey = string.Format("{0}_{1}_{2}", kdId, year, markRow[1]);
                        if (!deletedRegions.Contains(paramsKey))
                        {
                            DeleteRules(kdId, year, Convert.ToInt32(markRow[1]), db);
                            deletedRegions.Add(paramsKey);
                        }
                        AddNormatives(normativeSourceId, kdId, year, Convert.ToInt32(markRow[1]), budLevel, Convert.ToDecimal(markRow[0]) / 100,
                                        db);
                    }
                }
                messages = warningMessages.ToString();
            }
        }

        private const string KdAnalizKey = "2553274b-4cee-4d20-a9a6-eef173465d8b";

        private long GetKdAnalizId(string codeStr, string name, int sourceId, IDatabase db, IEntity kdAnalizEntity)
        {
            object kdId = db.ExecQuery("select id from d_KD_Analysis where codeStr = ? and sourceId = ?", QueryResultTypes.Scalar,
                         new DbParameterDescriptor("p0", codeStr),
                         new DbParameterDescriptor("p1", sourceId));
            if (kdId == null || kdId == DBNull.Value)
            {
                long newId = kdAnalizEntity.GetGeneratorNextValue;
                db.ExecQuery(
                    @"insert into d_KD_Analysis (ID, RowType, SourceId, CodeStr, Name, RefBridgeKD, RefDGroupKD, RefDGroup, RefKIFBridge, RefKVSRBridge, RefProgramsBridge, RefBridgeKDPlan, RefKIFBrdgPlan)
                    values (?, 0, ?, ?, ?, -1, -1, -1, -1, -1, -1, -1, -1)",
                    QueryResultTypes.NonQuery,
                    new DbParameterDescriptor("p0", newId),
                    new DbParameterDescriptor("p1", sourceId),
                    new DbParameterDescriptor("p2", codeStr),
                    new DbParameterDescriptor("p3", name));
                kdId = newId;
            }
            return Convert.ToInt64(kdId);
        }

        private int GetDataSourceIDByYear(int year, IDatabase db)
        {
            return GetDataSourceId(db, "ФО", 6, string.Empty, year);
        }

        /// <summary>
        /// Ищет источник данных с указанными параметрами.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="supplier">Поставщик.</param>
        /// <param name="dataCode">Поставщик.</param>
        /// <param name="year">Параметр "Год"</param>
        /// <returns>ID источника данных.</returns>
        internal int GetDataSourceId(IDatabase db, string supplier, int dataCode, string sourceName, int year)
        {
            object sourceID = db.ExecQuery(
                "select ID from DataSources where SupplierCode = ? and DataCode = ? and Year = ? and deleted = 0",
                QueryResultTypes.Scalar,
                 new DbParameterDescriptor("p0", supplier),
                 new DbParameterDescriptor("p1", dataCode),
                 new DbParameterDescriptor("p2", year));

            if (sourceID == null || sourceID == DBNull.Value)
            {
                IDataSource ds = scheme.DataSourceManager.DataSources.CreateElement();
                ds.SupplierCode = supplier;
                ds.DataCode = dataCode.ToString();
                ds.DataName = sourceName;
                ds.Year = year;
                ds.ParametersType = year == 0 ? ParamKindTypes.WithoutParams : ParamKindTypes.Year;
                return ds.Save();
            }

            return Convert.ToInt32(sourceID);
        }

        private void DeleteRules(long refKd, int year, int refRegions, IDatabase db)
        {
            db.ExecQuery("delete from f_Norm_VariedRegion where RefKD = ? and RefRegions = ? and RefYearDayUNV like ?",
                         QueryResultTypes.NonQuery,
                         new DbParameterDescriptor("p0", refKd),
                         new DbParameterDescriptor("p1", refRegions),
                         new DbParameterDescriptor("p2", string.Format("{0}____", year)));
        }

        private void AddNormativeRow(int sourceId, long refKd, int year, int refRegions, int budLevel, object value, IDatabase db)
        {
            db.ExecQuery(
                "insert into f_Norm_VariedRegion (SourceID, TaskID, Value, RefKD, RefRegions, RefBudLevel, RefYearDayUNV) values (?, -1, ?, ?, ?, ?, ?)",
                QueryResultTypes.NonQuery,
                new DbParameterDescriptor("p0", sourceId),
                new DbParameterDescriptor("p1", value),
                new DbParameterDescriptor("p2", refKd),
                new DbParameterDescriptor("p3", refRegions),
                new DbParameterDescriptor("p4", budLevel),
                new DbParameterDescriptor("p5", year * 10000));
        }

        private void AddNormatives(int sourceId, long refKd, int year, int refRegions, int budLevel, object value, IDatabase db)
        {
            switch (budLevel)
            {
                case 5:
                    AddNormativeRow(sourceId, refKd, year, refRegions, budLevel, value, db);
                    AddNormativeRow(sourceId, refKd, year, refRegions, 3, 0, db);
                    AddNormativeRow(sourceId, refKd, year, refRegions, 4, 0, db);
                    AddNormativeRow(sourceId, refKd, year, refRegions, 6, 0, db);
                    AddNormativeRow(sourceId, refKd, year, refRegions, 14, 0, db);
                    AddNormativeRow(sourceId, refKd, year, refRegions, 16, 0, db);
                    AddNormativeRow(sourceId, refKd, year, refRegions, 17, 0, db);
                    AddNormativeRow(sourceId, refKd, year, refRegions, 15, 0, db);
                    break;
                case 16:
                    AddNormativeRow(sourceId, refKd, year, refRegions, budLevel, value, db);
                    AddNormativeRow(sourceId, refKd, year, refRegions, 3, 0, db);
                    AddNormativeRow(sourceId, refKd, year, refRegions, 4, 0, db);
                    AddNormativeRow(sourceId, refKd, year, refRegions, 6, 0, db);
                    AddNormativeRow(sourceId, refKd, year, refRegions, 14, 0, db);
                    AddNormativeRow(sourceId, refKd, year, refRegions, 5, 0, db);
                    AddNormativeRow(sourceId, refKd, year, refRegions, 17, 0, db);
                    AddNormativeRow(sourceId, refKd, year, refRegions, 15, 0, db);
                    break;
                case 17:
                    AddNormativeRow(sourceId, refKd, year, refRegions, budLevel, value, db);
                    AddNormativeRow(sourceId, refKd, year, refRegions, 3, 0, db);
                    AddNormativeRow(sourceId, refKd, year, refRegions, 4, 0, db);
                    AddNormativeRow(sourceId, refKd, year, refRegions, 6, 0, db);
                    AddNormativeRow(sourceId, refKd, year, refRegions, 14, 0, db);
                    AddNormativeRow(sourceId, refKd, year, refRegions, 5, 0, db);
                    AddNormativeRow(sourceId, refKd, year, refRegions, 16, 0, db);
                    AddNormativeRow(sourceId, refKd, year, refRegions, 15, 0, db);
                    break;
                case 15:
                    AddNormativeRow(sourceId, refKd, year, refRegions, budLevel, value, db);
                    AddNormativeRow(sourceId, refKd, year, refRegions, 3, 0, db);
                    AddNormativeRow(sourceId, refKd, year, refRegions, 4, 0, db);
                    AddNormativeRow(sourceId, refKd, year, refRegions, 6, 0, db);
                    AddNormativeRow(sourceId, refKd, year, refRegions, 14, 0, db);
                    AddNormativeRow(sourceId, refKd, year, refRegions, 5, 0, db);
                    AddNormativeRow(sourceId, refKd, year, refRegions, 16, 0, db);
                    AddNormativeRow(sourceId, refKd, year, refRegions, 17, 0, db);
                    break;
            }
        }

        private int GetBudLevel(object terrType)
        {
            if (terrType == DBNull.Value || terrType == null)
                return -1;
            switch (Convert.ToInt32(terrType))
            {
                case 4:
                    return 5;
                case 5:
                    return 16;
                case 6:
                    return 17;
                case 7:
                    return 15;
                default:
                    return -1;
            }
        }

        #endregion
    }
}
