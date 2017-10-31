using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.Common;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.Server
{
    public class PriorForecastService
    {
        private IScheme Scheme
        {
            get; set;
        }

        public PriorForecastService(IScheme scheme)
        {
            Scheme = scheme;
        }

        public DataTable GetPriorForecastData(PriorForecastParams dataParams)
        {
            var priorForecastData = GetDataStructure(dataParams);
            FillData(dataParams, ref priorForecastData);
            priorForecastData.AcceptChanges();
            return priorForecastData;
        }

        #region получение данных

        private void FillData(PriorForecastParams dataParams, ref DataTable priorForecastData)
        {
            using (var db = Scheme.SchemeDWH.DB)
            {
                var kdData = GetKDData(db, dataParams.SourceId);
                var noSplitData = GetNoSplitData(dataParams, db);
                var splitData = GetSplitData(dataParams, db);
                FillData(dataParams, kdData, noSplitData, splitData, ref priorForecastData);
            }
        }

        private void FillData(PriorForecastParams dataParams, DataTable kdData, 
            DataTable noSplitData, DataTable splitData, ref DataTable priorForecastData)
        {
            foreach (DataRow dataRow in noSplitData.Rows)
            {
                var year = Convert.ToInt32(dataRow["RefYearDayUNV"].ToString().Substring(0, 4));
                string columnName = dataParams.TerrType == TerrType.SB ? "KBS_" + year : "KBMR_" + year;
                if (priorForecastData.Columns.Contains(columnName))
                {
                    string dataColumnName = year < dataParams.Year ? "Estimate" : "Forecast";
                    var dataValue = dataRow.IsNull(dataColumnName) ? 0 : Convert.ToDecimal(dataRow[dataColumnName]);
                    AddParentRow(kdData, dataValue, columnName, dataRow["RefParentKd"], ref priorForecastData);

                    var newRow = GetDataRow(dataRow["RefKd"], ref priorForecastData);
                    newRow["RefKd"] = dataRow["RefKd"];
                    newRow["RefParentKd"] = dataRow["RefParentKd"];
                    newRow["Code"] = dataRow["CodeStr"];
                    newRow["Name"] = dataRow["Name"];
                    newRow["IsResult"] = false;

                    newRow[columnName] = newRow.IsNull(columnName)
                                             ? dataValue : Convert.ToDecimal(columnName) + dataValue;
                }
            }

            foreach (DataRow dataRow in splitData.Rows)
            {
                var year = Convert.ToInt32(dataRow["RefYearDayUNV"].ToString().Substring(0, 4));
                int budLevel = Convert.ToInt32(dataRow["RefBudLevel"]);
                string resultColumnName = dataParams.TerrType == TerrType.SB ? "KBS_" + year : dataParams.TerrType == TerrType.MR ? "KBMR_" + year : "GO_" + year;
                string columnName = dataParams.TerrType == TerrType.SB ? "OB_" + year : "MR_" + year;
                string columnName2 = dataParams.TerrType == TerrType.SB ? "KMB_" + year : "POS_" + year;
                columnName = budLevel == 5 || budLevel == 3 ? columnName : columnName2;
                string dataColumnName = year < dataParams.Year ? "Estimate" : "Forecast";
                if (!priorForecastData.Columns.Contains(columnName) && !priorForecastData.Columns.Contains(resultColumnName))
                    continue;

                var val = dataRow.IsNull(dataColumnName) ? 0 : Convert.ToDecimal(dataRow[dataColumnName]);
                AddParentRow(kdData, val, columnName, dataRow["RefParentKd"], ref priorForecastData);

                var newRow = GetDataRow(dataRow["RefKd"], ref priorForecastData);
                newRow["RefKd"] = dataRow["RefKd"];
                newRow["RefParentKd"] = dataRow["RefParentKd"];
                newRow["Code"] = dataRow["CodeStr"];
                newRow["Name"] = dataRow["Name"];
                newRow["IsResult"] = false;
                if (dataParams.PriorForecastFormParams == PriorForecastFormParams.ContingentSplit)
                {
                    newRow[columnName] = newRow.IsNull(columnName)
                                              ? val : Convert.ToDecimal(newRow[columnName]) + val;
                }
                else if (dataParams.PriorForecastFormParams == PriorForecastFormParams.BudLevelsVariant)
                {
                    var resultValue = newRow.IsNull(resultColumnName) ? 0 : Convert.ToDecimal(newRow[resultColumnName]);
                    AddParentRow(kdData, resultValue + val, resultColumnName, dataRow["RefParentKd"], ref priorForecastData);

                    newRow[resultColumnName] = resultValue + val;
                    newRow[columnName] = newRow.IsNull(columnName)
                                              ? val : Convert.ToDecimal(newRow[columnName]) + val;
                }
                else if (dataParams.PriorForecastFormParams == PriorForecastFormParams.Contingent || dataParams.TerrType == TerrType.GO)
                {
                    AddParentRow(kdData, val, resultColumnName, dataRow["RefParentKd"], ref priorForecastData);
                    newRow[resultColumnName] = newRow.IsNull(resultColumnName)
                                              ? val : Convert.ToDecimal(newRow[resultColumnName]) + val;
                }
            }
        }

        private void AddParentRow(DataTable kdData, decimal value, string valueColumn, object parentKdId, ref DataTable priorForecastData)
        {
            if (parentKdId == null || parentKdId == DBNull.Value)
                return;

            var parentRows = kdData.Select(string.Format("Id = {0}", parentKdId));
            if (parentRows.Length > 0)
            {
                AddParentRow(kdData, value, valueColumn, parentRows[0]["ParentId"], ref priorForecastData);

                var newRow = GetDataRow(parentRows[0]["ID"], ref priorForecastData);
                newRow["RefKd"] = parentRows[0]["ID"];
                newRow["RefParentKd"] = parentRows[0]["ParentId"];
                newRow["Code"] = parentRows[0]["CodeStr"];
                newRow["Name"] = parentRows[0]["Name"];
                if (priorForecastData.Columns.Contains(valueColumn))
                {
                    newRow[valueColumn] = newRow.IsNull(valueColumn)
                                              ? value
                                              : Convert.ToDecimal(newRow[valueColumn]) + value;
                }
                newRow["IsResult"] = true;
            }
        }

        private DataRow GetDataRow(object refKd, ref DataTable priorForecastData)
        {
            var rows = priorForecastData.Select(string.Format("RefKd = {0}", refKd));
            if (rows.Length != 0)
                return rows[0];
            var newRow = priorForecastData.NewRow();
            priorForecastData.Rows.Add(newRow);
            return newRow;
        }

        private DataTable GetKDData(IDatabase db, long sourceId)
        {
            var kdData = (DataTable)db.ExecQuery("select Id, ParentId, CodeStr, Name from d_KD_PlanIncomes where SourceId = ?",
                         QueryResultTypes.DataTable,
                         new DbParameterDescriptor("p0", sourceId));
            return kdData;
        }

        private DataTable GetNoSplitData(PriorForecastParams dataParams, IDatabase db)
        {
            var query =
                @"select fact.Estimate, fact.Forecast, fact.RefKD, fact.RefYearDayUNV, cls.CodeStr, cls.Name, cls.ParentID as RefParentKd 
                from f_D_FOPlanInc fact, d_KD_PlanIncomes cls
                where fact.SourceId = ? and fact.RefVariant = ? and fact.RefKD = cls.ID order by cls.CodeStr asc";
            if (dataParams.PriorForecastFormParams != PriorForecastFormParams.ContingentSplit)
                query = @"select fact.Estimate, fact.Forecast, fact.RefKD, fact.RefYearDayUNV, cls.CodeStr, cls.Name, cls.ParentID as RefParentKd 
                from f_D_FOPlanInc fact, d_KD_PlanIncomes cls
                where fact.SourceId = ? and fact.RefVariant = ? and fact.RefKD = cls.ID and 1 = 2 order by cls.CodeStr asc";

            var dt = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable,
                new DbParameterDescriptor("p0", dataParams.SourceId),
                new DbParameterDescriptor("p1", dataParams.ContingentVariant));

            foreach (DataRow row in dt.Rows)
            {
                if (!row.IsNull("Estimate"))
                    row["Estimate"] = Convert.ToDecimal(row["Estimate"])/1000;
                if (!row.IsNull("Forecast"))
                    row["Forecast"] = Convert.ToDecimal(row["Forecast"])/1000;
            }

            return dt;
        }

        private DataTable GetSplitData(PriorForecastParams dataParams, IDatabase db)
        {
            string query = string.Empty;
            var queryParams = new List<DbParameterDescriptor>();
            queryParams.Add(new DbParameterDescriptor("p0", dataParams.SourceId));
            switch (dataParams.PriorForecastFormParams)
            {
                case PriorForecastFormParams.Contingent:
                    query =
                        @"select fact.Estimate, fact.Forecast, fact.RefKD, fact.RefBudLevel, fact.RefYearDayUNV, cls.CodeStr, cls.Name, cls.ParentID as RefParentKd
                        from f_D_FOPlanIncDivide fact, d_KD_PlanIncomes cls
                        where fact.SourceId = ? and fact.RefBudLevel = ? and fact.RefVariant = ? and fact.RefKD = cls.ID order by cls.CodeStr asc";
                    if (dataParams.TerrType == TerrType.SB)
                    {
                        queryParams.Add(new DbParameterDescriptor("p1", 2));
                    }
                    else if (dataParams.TerrType == TerrType.MR)
                    {
                        queryParams.Add(new DbParameterDescriptor("p1", 14));
                    }
                    break;
                case PriorForecastFormParams.ContingentSplit:
                case PriorForecastFormParams.BudLevelsVariant:
                    query =
                        @"select fact.Estimate, fact.Forecast, fact.RefKD, fact.RefBudLevel, fact.RefYearDayUNV, cls.CodeStr, cls.Name, cls.ParentID as RefParentKd
                        from f_D_FOPlanIncDivide fact, d_KD_PlanIncomes cls
                        where fact.SourceId = ? and (fact.RefBudLevel = ? or fact.RefBudLevel = ?) and fact.RefVariant = ? and fact.RefKD = cls.ID order by cls.CodeStr asc";
                    if (dataParams.TerrType == TerrType.SB)
                    {
                        queryParams.Add(new DbParameterDescriptor("p1", 3));
                        queryParams.Add(new DbParameterDescriptor("p2", 14));
                    }
                    else if (dataParams.TerrType == TerrType.MR)
                    {
                        queryParams.Add(new DbParameterDescriptor("p1", 5));
                        queryParams.Add(new DbParameterDescriptor("p2", 6));
                    }
                    break;
            }
            if (dataParams.TerrType == TerrType.GO)
            {
                query =
                    @"select fact.Estimate, fact.Forecast, fact.RefKD, fact.RefBudLevel, fact.RefYearDayUNV, cls.CodeStr, cls.Name, cls.ParentID as RefParentKd
                    from f_D_FOPlanIncDivide fact, d_KD_PlanIncomes cls
                    where fact.SourceId = ? and fact.RefBudLevel = ? and fact.RefVariant = ? and fact.RefKD = cls.ID order by cls.CodeStr asc";
                queryParams.Add(new DbParameterDescriptor("p1", 15));
            }

            queryParams.Add(new DbParameterDescriptor("p" + queryParams.Count, dataParams.BudLevelVariant));

            var dt = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable, queryParams.ToArray());

            foreach (DataRow row in dt.Rows)
            {
                if (!row.IsNull("Estimate"))
                    row["Estimate"] = Convert.ToDecimal(row["Estimate"]) / 1000;
                if (!row.IsNull("Forecast"))
                    row["Forecast"] = Convert.ToDecimal(row["Forecast"]) / 1000;
            }
            return dt;
        }

        #endregion

        #region сохранение данных

        public void SaveData(PriorForecastParams dataParams, DataTable priorForecastData)
        {
            var dtChanges = priorForecastData.GetChanges();
            if (dtChanges == null)
                return;
            using (var db = Scheme.SchemeDWH.DB)
            {
                var regionId = GetRegionId(dataParams);
                foreach (DataRow row in dtChanges.Rows.Cast<DataRow>().Where(w => !Convert.ToBoolean(w["IsResult"])))
                {
                    foreach (DataColumn column in dtChanges.Columns.Cast<DataColumn>().Where(w => w.DataType == typeof(Decimal) && w.ColumnName.Contains("_")))
                    {
                        int year = Convert.ToInt32(column.ColumnName.Split('_')[1]);
                        object value = row[column];
                        int budLevel = GetBudLevel(dataParams, column.ColumnName.Split('_')[0]);
                        long refKd = Convert.ToInt64(row["RefKd"]);
                        DeleteData(dataParams, year, budLevel, refKd, db);
                        AddNewRow(dataParams, year, budLevel, refKd, regionId, value, db);
                    }
                }
            }
        }

        private int GetBudLevel(PriorForecastParams dataParams, string columnName)
        {
            switch (columnName)
            {
                case "KBS":
                    if (dataParams.TerrType == TerrType.SB && 
                        dataParams.PriorForecastFormParams == PriorForecastFormParams.Contingent)
                    {
                        return 2;
                    }
                    break;
                case "OB":
                    if (dataParams.TerrType == TerrType.SB)
                    {
                        return 3;
                    }
                    break;
                case "KMB":
                    if (dataParams.TerrType == TerrType.SB)
                    {
                        return 14;
                    }
                    break;
                case "KBMR":
                    if (dataParams.TerrType == TerrType.MR &&
                        dataParams.PriorForecastFormParams == PriorForecastFormParams.Contingent)
                    {
                        return 14;
                    }
                    break;
                case "MR":
                    if (dataParams.TerrType == TerrType.MR)
                    {
                        return 5;
                    }
                    break;
                case "POS":
                    if (dataParams.TerrType == TerrType.MR)
                    {
                        return 6;
                    }
                    break;
                case "GO":
                    return 15;
            }
            return -1;
        }

        private void DeleteData(PriorForecastParams dataParams, int year, int budLevel, long  refKd, IDatabase db)
        {
            string dataColumn = year < dataParams.Year ? "Estimate" : "Forecast";
            if (budLevel == -1)
            {
                string query =
                    string.Format("delete from f_D_FOPlanInc where SourceId = ? and RefVariant = ? and RefKD = ? and {0} is not Null and RefYearDayUNV = {1}0001",
                    dataColumn, year);
                db.ExecQuery(string.Format(query, dataColumn, year), QueryResultTypes.NonQuery,
                             new DbParameterDescriptor("p0", dataParams.SourceId),
                             new DbParameterDescriptor("p1", dataParams.ContingentVariant),
                             new DbParameterDescriptor("p2", refKd));
            }
            if (budLevel != -1)
            {
                long variant = dataParams.BudLevelVariant;
                if (dataParams.PriorForecastFormParams == PriorForecastFormParams.Contingent)
                    variant = dataParams.ContingentVariant;
                string query =
                    string.Format("delete from f_D_FOPlanIncDivide where SourceId = ? and RefVariant = ? and RefBudLevel = ? and RefKD = ? and {0} is not Null and RefYearDayUNV = {1}0001",
                    dataColumn, year);
                db.ExecQuery(string.Format(query, dataColumn, year), QueryResultTypes.NonQuery,
                             new DbParameterDescriptor("p0", dataParams.SourceId),
                             new DbParameterDescriptor("p1", variant),
                             new DbParameterDescriptor("p2", budLevel),
                             new DbParameterDescriptor("p3", refKd));
            }
        }

        private void AddNewRow(PriorForecastParams dataParams, int year, int budLevel, long refKd, long regionId, object value, IDatabase db)
        {
            if (value == null || value == DBNull.Value)
                return;
            value = Convert.ToDecimal(value)*1000;

            string dataColumn = year < dataParams.Year ? "Estimate" : "Forecast";
            var paramsList = new List<DbParameterDescriptor>();
            paramsList.Add(new DbParameterDescriptor("p" + paramsList.Count, dataParams.SourceId));
            paramsList.Add(new DbParameterDescriptor("p" + paramsList.Count, value));
            string query = string.Empty;
            // если уровень бюджета не указан, добавляем данные в таблицу без расщепления иначе с расщеплением
            if (budLevel == -1)
            {
                query =
                    string.Format(@"insert into f_D_FOPlanInc (SourceID, TaskID, {0}, RefVariant, RefKD, RefRegions, RefKVSR, RefFODepartments, RefYearDayUNV, RefTaxObjects, RefOrganizations) 
                    values (?, -1, ?, ?, ?, ?, -1, -1, ?, -1, -1)", dataColumn);
                paramsList.Add(new DbParameterDescriptor("p" + paramsList.Count, dataParams.ContingentVariant));
                paramsList.Add(new DbParameterDescriptor("p" + paramsList.Count, refKd));
                paramsList.Add(new DbParameterDescriptor("p" + paramsList.Count, regionId));
                paramsList.Add(new DbParameterDescriptor("p" + paramsList.Count, year + "0001"));
            }
            else
            {
                long variant = dataParams.BudLevelVariant;
                query =
                    string.Format(@"insert into f_D_FOPlanIncDivide (SourceID, TaskID, {0}, RefVariant, RefNormDeduct, RefKD, RefRegions, RefBudLevel, RefKVSR, RefYearDayUNV, RefOrganizations, RefTaxObjects, RefFODepartments) 
                    values (?, -1, ?, ?, 6, ?, ?, ?, -1, ?, -1, -1, -1)", dataColumn);
                paramsList.Add(new DbParameterDescriptor("p" + paramsList.Count, variant));
                paramsList.Add(new DbParameterDescriptor("p" + paramsList.Count, refKd));
                paramsList.Add(new DbParameterDescriptor("p" + paramsList.Count, regionId));
                paramsList.Add(new DbParameterDescriptor("p" + paramsList.Count, budLevel));
                paramsList.Add(new DbParameterDescriptor("p" + paramsList.Count, year + "0001"));
            }
            db.ExecQuery(query, QueryResultTypes.NonQuery, paramsList.ToArray());
        }

        #endregion

        #region структура данных

        private DataTable GetDataStructure(PriorForecastParams dataParams)
        {
            var dtNew = new DataTable();
            dtNew.Columns.Add("ID", typeof (int));
            dtNew.Columns.Add("RefKd", typeof(long));
            dtNew.Columns.Add("RefParentKd", typeof(long));
            dtNew.Columns.Add("Code", typeof(string));
            dtNew.Columns.Add("Name", typeof(string));
            dtNew.Columns.Add("IsResult", typeof(bool));

            if (dataParams.Estimate)
                dtNew = AddColumns(dataParams, dtNew, dataParams.Year - 1);
            if (dataParams.Forecast)
            {
                dtNew = AddColumns(dataParams, dtNew, dataParams.Year);
                dtNew = AddColumns(dataParams, dtNew, dataParams.Year + 1);
                dtNew = AddColumns(dataParams, dtNew, dataParams.Year + 2);
            }
            return dtNew;
        }

        private DataTable AddColumns(PriorForecastParams dataParams, DataTable dtData, int year)
        {
            switch (dataParams.TerrType)
            {
                case TerrType.SB:
                    dtData.Columns.Add("KBS_" + year, typeof(decimal));
                    if (dataParams.PriorForecastFormParams != PriorForecastFormParams.Contingent)
                    {
                        dtData.Columns.Add("OB_" + year, typeof (decimal));
                        dtData.Columns.Add("KMB_" + year, typeof (decimal));
                    }
                    break;
                case TerrType.MR:
                    dtData.Columns.Add("KBMR_" + year, typeof(decimal));
                    if (dataParams.PriorForecastFormParams != PriorForecastFormParams.Contingent)
                    {
                        dtData.Columns.Add("MR_" + year, typeof (decimal));
                        dtData.Columns.Add("POS_" + year, typeof (decimal));
                    }
                    break;
                case TerrType.GO:
                    dtData.Columns.Add("GO_" + year, typeof(decimal));
                    break;
            }
            return dtData;
        }

        #endregion

        #region получение источника данных

        public int GetSourceId(int year)
        {
            using (IDatabase db = Scheme.SchemeDWH.DB)
            {
                int dataSourceID = GetDataSourceID(db, "ФО", 29, 29, year);
                DataTable dt = (DataTable)db.ExecQuery("select ID from DataSources where SupplierCode = 'ФО' and DataCode = 29 and Year = ? and deleted = 0",
                    QueryResultTypes.DataTable,
                    new DbParameterDescriptor("ID", year));
                if (dt.Rows.Count == 0)
                {
                    IDataSource ds = Scheme.DataSourceManager.DataSources.CreateElement();
                    ds.SupplierCode = "ФО";
                    ds.DataCode = "0029";
                    ds.DataName = "Проект бюджета";
                    ds.Year = year;
                    ds.ParametersType = ParamKindTypes.Year;
                    dataSourceID = ds.Save();
                }
                return dataSourceID;
            }
        }

        internal int GetDataSourceID(IDatabase db, string supplier, int dataCodeMain, int dataCodeSecond, int year)
        {
            object sourceID = db.ExecQuery(
                "select ID from DataSources where SupplierCode = ? and DataCode = ? and Year = ? and deleted = 0",
                QueryResultTypes.Scalar,
                 new DbParameterDescriptor("SupplierCode", supplier),
                 new DbParameterDescriptor("DataCode", dataCodeMain),
                 new DbParameterDescriptor("Year", year));

            if (sourceID == null || sourceID == DBNull.Value)
            {
                sourceID = db.ExecQuery(
                    "select ID from DataSources where SupplierCode = ? and DataCode = ? and Year = ? and deleted = 0",
                    QueryResultTypes.Scalar,
                    new DbParameterDescriptor("SupplierCode", supplier),
                    new DbParameterDescriptor("DataCode", dataCodeSecond),
                    new DbParameterDescriptor("Year", year));
            }

            if (sourceID == null || sourceID == DBNull.Value)
            {
                IDataSource ds = Scheme.DataSourceManager.DataSources.CreateElement();
                ds.SupplierCode = supplier;
                ds.DataCode = dataCodeMain.ToString();
                ds.DataName = "Проект бюджета";
                ds.Year = year;
                ds.ParametersType = ParamKindTypes.Year;
                return ds.Save();
            }
            return Convert.ToInt32(sourceID);
        }

        #endregion

        #region расщепление данных

        /// <summary>
        /// расщепляем данные
        /// </summary>
        public long SplitData(int variantId, int variantType, int variantForSplit, int year)
        {
            if (variantForSplit == -1)
                Scheme.DisintRules.SplitData(variantId, variantType, false, ref variantForSplit, year);
            else
                Scheme.DisintRules.SplitData(variantId, variantType, false, variantForSplit, year);
            return variantForSplit;
        }

        #endregion

        #region получение различных параметров системы

        public long GetRegionId(PriorForecastParams dataParams)
        {
            using (var db = Scheme.SchemeDWH.DB)
            {
                switch (dataParams.TerrType)
                {
                    case TerrType.MR:
                    case TerrType.SB:
                        string query = "select id from d_Regions_Plan where SourceId = ? and RefTerr = ?";
                        var queryParams = new List<DbParameterDescriptor>();
                        queryParams.Add(new DbParameterDescriptor("p0", dataParams.SourceId));
                        queryParams.Add(new DbParameterDescriptor("p1", (int)dataParams.TerrType));
                        return Convert.ToInt64(db.ExecQuery(query, QueryResultTypes.Scalar, queryParams.ToArray()));
                    case TerrType.GO:
                        return -1;
                }
                return -1;
            }
        }

        #endregion
    }
}
