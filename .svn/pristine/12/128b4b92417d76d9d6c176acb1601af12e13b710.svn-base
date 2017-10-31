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
    public enum CalculateValueType
    {
        Estimate,
        Forecast,
        TaxResource
    }

    public class IncomesEvalPlanService
    {
        private IScheme Scheme
        {
            get; set;
        }

        public IncomesEvalPlanService(IScheme scheme)
        {
            Scheme = scheme;
        }

        public DataTable GetData(IncomesEvalPlanParams incomesEvalPlanParams)
        {
            DataTable dtData = new DataTable();
            GetData(incomesEvalPlanParams, ref dtData);
            SetResults(ref dtData);
            DeleteRegionsSum(ref dtData, incomesEvalPlanParams);
            return dtData;
        }

        public DataTable GetRegions(int sourceId)
        {
            using (IDatabase db = Scheme.SchemeDWH.DB)
            {
                const string query = @"select region.id, region.code, region.name, region.parentId, region.CodeLine, region.SourceId,
                    terType.id as TerTypeId, terType.Name as TerTypeName
                    from d_Regions_Plan region, fx_FX_TerritorialPartitionType terType where
                    region.RefTerr = terType.Id and region.SourceID = ? and region.ID >= 0 order by region.CodeLine";
                var dtRegions =
                    (DataTable)
                    db.ExecQuery(query, QueryResultTypes.DataTable, new DbParameterDescriptor("p0", sourceId));
                return dtRegions;
            }
        }

        private DataSet GetKdPlanIncomes(IDatabase db, IncomesEvalPlanParams incomesEvalPlanParams)
        {
            DataSet kdData = new DataSet();
            string query = "select id, parentId from d_KD_PlanIncomes where SourceId = ? and id >= 0";
            DataTable kdPlan =
                (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable,
                new DbParameterDescriptor("p0", incomesEvalPlanParams.SourceId));
            kdData.Tables.Add(kdPlan);
            DataRelation relation = new DataRelation("rel", kdPlan.Columns["ID"], kdPlan.Columns["ParentID"], false);
            kdData.Relations.Add(relation);
            return kdData;
        }

        private void GetData(IncomesEvalPlanParams incomesEvalPlanParams, ref DataTable emptyData)
        {
            List<DbParameterDescriptor> queryParams = new List<DbParameterDescriptor>(); 
            string query =
                @"select Forecast, Estimate, TaxResource, RefYearDayUNV, RefRegions, RefBudLevel, RefKD, RefKVSR, IsBlocked from f_D_FOPlanIncDivide where 
                RefVariant = ? and RefKD = ?";
            queryParams.Add(new DbParameterDescriptor("p0", incomesEvalPlanParams.IncomesVariant));
            queryParams.Add( new DbParameterDescriptor("p1", incomesEvalPlanParams.PlanIncomes));
            if (Convert.ToInt32(incomesEvalPlanParams.KvsrPlan) != -1)
            {
                query += " and RefKVSR = ?";
                queryParams.Add(new DbParameterDescriptor("p2", incomesEvalPlanParams.KvsrPlan));
            }
            
            if (incomesEvalPlanParams.IsEstimate)
            {
                query += " and (RefYearDayUNV like ?";
                queryParams.Add(new DbParameterDescriptor("p" + queryParams.Count + 1, incomesEvalPlanParams.Year - 1 + "____"));
            }
            if (incomesEvalPlanParams.IsForecast || incomesEvalPlanParams.IsTaxResource)
            {
                if (incomesEvalPlanParams.IsEstimate)
                    query += " or RefYearDayUNV like ? or RefYearDayUNV like ? or RefYearDayUNV like ?)";
                else
                    query += " and (RefYearDayUNV like ? or RefYearDayUNV like ? or RefYearDayUNV like ?)";
                queryParams.Add(new DbParameterDescriptor("p" + queryParams.Count + 1, incomesEvalPlanParams.Year + "____"));
                queryParams.Add(new DbParameterDescriptor("p" + queryParams.Count + 1, incomesEvalPlanParams.Year + 1 + "____"));
                queryParams.Add(new DbParameterDescriptor("p" + queryParams.Count + 1, incomesEvalPlanParams.Year + 2 + "____"));
            }
            else
                query += ")";

            using (IDatabase db = Scheme.SchemeDWH.DB)
            {
                DataSet kdData = GetKdPlanIncomes(db, incomesEvalPlanParams);
                DataTable dtData = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable, queryParams.ToArray());
                // список подчиненных записей по коду дохода для нахождения суммы подчиненных элементов
                GetKdPlanIncomes(db, incomesEvalPlanParams);
                DataRow[] kdRows =
                    kdData.Tables[0].Select(string.Format("ParentID = {0}", incomesEvalPlanParams.PlanIncomes));
                DataTable dtRegions = GetRegions(incomesEvalPlanParams.SourceId);
                emptyData = GetEmptyTable(incomesEvalPlanParams);
                if (dtRegions.Rows.Count == 0)
                    return;
                emptyData.BeginLoadData();
                DataRow parentRegionRow = dtRegions.Select("ParentId is null")[0];
                object topLevelId = parentRegionRow["ID"];
                int num = 1;
                int id = 0;
                GetData(db, incomesEvalPlanParams, parentRegionRow, dtData, kdRows, ref emptyData, ref num, ref id);
                foreach (DataRow row in emptyData.Select(string.Format("RegionParentId = {0}", topLevelId)))
                {
                    row["RegionParentId"] = DBNull.Value;
                }
                emptyData.EndLoadData();
                emptyData.AcceptChanges();
            }
        }

        private void GetData(IDatabase db, IncomesEvalPlanParams incomesEvalPlanParams,
            DataRow parentRegionRow, DataTable factData, DataRow[] kdRows, ref DataTable dtData, ref int num, ref int id)
        {
            foreach (DataRow regionRow in parentRegionRow.Table.Select(string.Format("ParentId = {0}", parentRegionRow["ID"]), "Code asc"))
            {
                AddNewDataRow(db, incomesEvalPlanParams, regionRow, kdRows, factData, ref dtData, ref num, ref id);
                GetData(db, incomesEvalPlanParams, regionRow, factData, kdRows, ref dtData, ref num, ref id);
            }
        }

        private void AddNewDataRow(IDatabase db, IncomesEvalPlanParams incomesEvalPlanParams, DataRow regionRow, DataRow[] kdRows,
            DataTable factTable, ref DataTable dataTable, ref int num, ref int id)
        {
            bool isResult = false;
            int terType = Convert.ToInt32(regionRow["TerTypeId"]);
            object parentRegionId = regionRow["ParentId"];
            string regionName = regionRow["Name"].ToString();
            if (incomesEvalPlanParams.Municipal == Municipal.MrGo &&
                (terType == 5 || terType == 6))
                return;
            if (regionName.Contains("Бюджеты поселений"))
            {
                if (incomesEvalPlanParams.Municipal == Municipal.MrGo)
                    return;
                regionName = "Бюджеты поселений";
                terType = (int)ConsTerritoryType.ConsPos;
                isResult = true;
            }
            if (regionName.Contains("Муниципальные районы"))
            {
                terType = (int)ConsTerritoryType.MoResult;
                parentRegionId = -5;
                isResult = true;
            }
            if (regionName.Contains("Городские округа"))
            {
                terType = (int)ConsTerritoryType.GoResult;
                parentRegionId = -5;
                isResult = true;
            }
            if (terType == 3)
            {
                DataRow totalRegions = dataTable.NewRow();
                totalRegions["ID"] = id;
                id++;
                totalRegions["TerritoryType"] = ConsTerritoryType.RegionsResult;
                totalRegions["RegionName"] = "Итого";
                totalRegions["RegionID"] = -5;
                totalRegions["RegionParentID"] = -10;
                totalRegions["IsResult"] = true;
                dataTable.Rows.Add(totalRegions);
                parentRegionId = -10;
            }
            DataRow newRow = dataTable.NewRow();
            newRow.BeginEdit();
            newRow["id"] = id;
            id++;
            if (terType == 4 || terType == 7)
            {
                newRow["Num"] = num.ToString().PadLeft(3, '0');
                num++;
            }
            newRow["TerritoryType"] = terType;
            if (terType > 0 && terType < 20)
                newRow["TerritoryTypeName"] = regionRow["TerTypeName"];
            newRow["Region"] = regionRow["id"];
            newRow["RegionId"] = regionRow["id"];
            newRow["RegionParentId"] = parentRegionId;
            newRow["RegionName"] = regionName;
            newRow["SourceId"] = regionRow["SourceId"];
            newRow["RefKD"] = incomesEvalPlanParams.PlanIncomes;
            newRow["Year"] = incomesEvalPlanParams.Year;
            newRow["IsResult"] = isResult;

            int firstYear = incomesEvalPlanParams.IsEstimate
                                ? incomesEvalPlanParams.Year - 1
                                : incomesEvalPlanParams.Year;
            int lastYear = incomesEvalPlanParams.IsForecast || incomesEvalPlanParams.IsTaxResource
                               ? incomesEvalPlanParams.Year + 2
                               : incomesEvalPlanParams.Year - 1;

            for (int year = firstYear; year <= lastYear; year++)
            {
                DataTable dtChildFacts = GetFactTable(db, regionRow["id"], incomesEvalPlanParams, year); 
                dtChildFacts.BeginLoadData();
                bool isEstimate = year == incomesEvalPlanParams.Year - 1;
                bool isBlocked = false;
                switch (incomesEvalPlanParams.BudgetLevel)
                {
                    case BudgetLevel.RegionBudget:
                        // оценка
                        if (isEstimate)
                        {
                            string columnName = string.Format("{0}_OB", year);
                            string resultColumnName = string.Format("{0}_OBResult", year);
                            SetData(factTable, regionRow["id"], 3, year, dtChildFacts, CalculateValueType.Estimate,
                                    kdRows, columnName, resultColumnName, ref newRow, ref isBlocked);
                        }
                        else
                        {
                            // прогноз
                            if (incomesEvalPlanParams.IsForecast)
                            {
                                string columnName = string.Format("{0}_OB", year);
                                string resultColumnName = string.Format("{0}_OBResult", year);
                                SetData(factTable, regionRow["id"], 3, year, dtChildFacts, CalculateValueType.Forecast,
                                    kdRows, columnName, resultColumnName, ref newRow, ref isBlocked);
                            }
                            // налоговый потенциал
                            if (incomesEvalPlanParams.IsTaxResource)
                            {
                                string columnName = string.Empty;
                                string resultColumnName = string.Empty;
                                SetData(factTable, regionRow["id"], 3, year, dtChildFacts, CalculateValueType.TaxResource,
                                    kdRows, columnName, resultColumnName, ref newRow, ref isBlocked);
                            }
                        }
                        newRow[year + "_IsBlocked"] = isBlocked;
                        break;
                    case BudgetLevel.ConsSubjectBudget:
                        decimal kbsSum = 0;
                        // оценка
                        if (isEstimate)
                        {
                            string columnName = string.Format("{0}_OB", year);
                            string resultColumnName = string.Format("{0}_OBResult", year);
                            kbsSum += SetData(factTable, regionRow["id"], 3, year, dtChildFacts, CalculateValueType.Estimate,
                                    kdRows, columnName, resultColumnName, ref newRow, ref isBlocked);
                            int budlevel = GetBudLevel(terType, incomesEvalPlanParams);
                            if (budlevel == 3)
                                budlevel = 14;
                            columnName = string.Format("{0}_KMB", year);
                            resultColumnName = string.Format("{0}_KMBResult", year);
                            kbsSum += SetData(factTable, regionRow["id"], budlevel, year, dtChildFacts, CalculateValueType.Estimate,
                                    kdRows, columnName, resultColumnName, ref newRow, ref isBlocked);
                        }
                        else
                        {
                            // прогноз
                            if (incomesEvalPlanParams.IsForecast)
                            {
                                string columnName = string.Format("{0}_OB", year);
                                string resultColumnName = string.Format("{0}_OBResult", year);
                                kbsSum += SetData(factTable, regionRow["id"], 3, year, dtChildFacts, CalculateValueType.Forecast,
                                    kdRows, columnName, resultColumnName, ref newRow, ref isBlocked);
                                int budlevel = GetBudLevel(terType, incomesEvalPlanParams);
                                if (budlevel == 3)
                                    budlevel = 14;
                                columnName = string.Format("{0}_KMB", year);
                                resultColumnName = string.Format("{0}_KMBResult", year);
                                kbsSum += SetData(factTable, regionRow["id"], budlevel, year, dtChildFacts, CalculateValueType.Forecast,
                                    kdRows, columnName, resultColumnName, ref newRow, ref isBlocked);
                            }
                            // налоговый потенциал
                            if (incomesEvalPlanParams.IsTaxResource)
                            {
                                int budlevel = GetBudLevel(terType, incomesEvalPlanParams);
                                string columnName = string.Empty;
                                string resultColumnName = string.Empty;
                                SetData(factTable, regionRow["id"], budlevel, year, dtChildFacts, CalculateValueType.TaxResource,
                                    kdRows, columnName, resultColumnName, ref newRow, ref isBlocked);
                            }
                        }
                        // заполняем три поля данными
                        if (incomesEvalPlanParams.IsForecast || isEstimate)
                            newRow[string.Format("{0}_KBS", year)] = kbsSum / 1000;
                        newRow[year + "_IsBlocked"] = isBlocked;
                        break;
                    case BudgetLevel.LocalBudget:
                        // оценка
                        if (isEstimate)
                        {
                            string columnName = string.Format("{0}_KMB", year);
                            string resultColumnName = string.Format("{0}_KMBResult", year);
                            int budlevel = GetBudLevel(terType, incomesEvalPlanParams);
                            if (budlevel == 3)
                                budlevel = 14;
                            SetData(factTable, regionRow["id"], budlevel, year, dtChildFacts, CalculateValueType.Estimate,
                                    kdRows, columnName, resultColumnName, ref newRow, ref isBlocked);
                        }
                        else
                        {
                            int budlevel = GetBudLevel(terType, incomesEvalPlanParams);
                            if (budlevel == 3)
                                budlevel = 14;
                            // прогноз
                            if (incomesEvalPlanParams.IsForecast)
                            {
                                string columnName = string.Format("{0}_KMB", year);
                                string resultColumnName = string.Format("{0}_KMBResult", year);
                                SetData(factTable, regionRow["id"], budlevel, year, dtChildFacts, CalculateValueType.Forecast,
                                    kdRows, columnName, resultColumnName, ref newRow, ref isBlocked);
                            }
                            // налоговый потенциал
                            if (incomesEvalPlanParams.IsTaxResource)
                            {
                                string columnName = string.Empty;
                                string resultColumnName = string.Empty;
                                SetData(factTable, regionRow["id"], budlevel, year, dtChildFacts, CalculateValueType.TaxResource,
                                    kdRows, columnName, resultColumnName, ref newRow, ref isBlocked);
                            }
                        }
                        newRow[year + "_IsBlocked"] = isBlocked;
                        break;
                }
            }
            newRow.EndEdit();
            dataTable.Rows.Add(newRow);
            if (terType == 4)
            {
                if (incomesEvalPlanParams.Municipal == Municipal.MrGoSettlement)
                {
                    DataRow regionBudgetRow = dataTable.Rows.Add(newRow.ItemArray);
                    newRow["TerritoryType"] = ConsTerritoryType.ConsMo;
                    isResult = true;
                    newRow["IsResult"] = isResult;
                    regionBudgetRow["Num"] = DBNull.Value;
                    regionBudgetRow["RegionName"] = "Районный бюджет";
                    regionBudgetRow["Region"] = regionBudgetRow["RegionID"];
                    regionBudgetRow["RegionParentId"] = regionBudgetRow["RegionID"];
                    regionBudgetRow["RegionID"] = 0;
                    regionBudgetRow["id"] = id;
                    regionBudgetRow["IsResult"] = false;
                    id++;
                    newRow["TerritoryTypeName"] = string.Empty;
                    // обнуляем родительскую запись
                    foreach (DataColumn column in dataTable.Columns)
                    {
                        if (column.DataType == typeof(decimal))
                            newRow[column] = 0;
                    }
                }
                else if (incomesEvalPlanParams.Municipal == Municipal.MrGo)
                {
                    newRow["TerritoryType"] = 14;
                }
            }
            if (terType == 3)
            {
                DataRow total = dataTable.NewRow();
                total["ID"] = id;
                id++;
                total["TerritoryType"] = ConsTerritoryType.TotalResult;
                total["RegionID"] = -10;
                total["RegionName"] = "Всего";
                total["IsResult"] = true;
                dataTable.Rows.Add(total);
            }
        }

        private decimal SetData(DataTable factTable, object region, object budLevel, object year, 
            DataTable dtChildFacts, CalculateValueType calculateValueType, DataRow[] kdRows, 
            string columnName, string resultColumnName, ref DataRow newRow, ref bool isBlocked)
        {
            decimal resultValue = 0;
            string refYearDayUNV = string.Format("{0}0001", year);
            DataRow[] dataRows = factTable.Select(string.Format("RefYearDayUNV = {0} and RefRegions = {1} and RefBudLevel = {2}",
                            refYearDayUNV, region, budLevel));
            decimal rowValue = 0;
            decimal childSum = 0;
            bool isChildBlocked = false;
            bool isResult = false;

            switch (calculateValueType)
            {
                case CalculateValueType.Estimate:
                    // оценка
                    rowValue = GetValue(dataRows, "Estimate", ref isBlocked, ref isResult);
                    newRow[resultColumnName] =
                        GetChildSum(dtChildFacts, Convert.ToInt32(budLevel), refYearDayUNV, kdRows, "Estimate", ref childSum, ref isChildBlocked);
                    if (isResult)
                        newRow[resultColumnName] = isResult;
                    rowValue += childSum;
                    resultValue += rowValue;
                    newRow[columnName] = rowValue / 1000;
                    break;
                case CalculateValueType.Forecast:
                    // прогноз
                    rowValue = GetValue(dataRows, "Forecast", ref isBlocked, ref isResult);
                    newRow[resultColumnName] =
                        GetChildSum(dtChildFacts, Convert.ToInt32(budLevel), refYearDayUNV, kdRows, "Forecast", ref childSum, ref isChildBlocked);
                    if (isResult)
                        newRow[resultColumnName] = isResult;
                    rowValue += childSum;
                    resultValue += rowValue;
                    newRow[columnName] = rowValue / 1000;
                    break;
                case CalculateValueType.TaxResource:
                    // налоговый ресурс
                    dataRows = factTable.Select(string.Format("RefYearDayUNV = {0} and RefRegions = {1} and (RefBudLevel = 5)",
                        refYearDayUNV, region));
                    rowValue = GetValue(dataRows, "TaxResource", ref isBlocked, ref isResult);
                    newRow[string.Format("{0}_BMRResult", year)] =
                        GetChildSum(dtChildFacts, 5, refYearDayUNV, kdRows, "TaxResource", ref childSum, ref isChildBlocked);

                    if (isResult)
                        newRow[string.Format("{0}_BMRResult", year)] = isResult;

                    rowValue += childSum;
                    resultValue += rowValue;
                    newRow[string.Format("{0}_BMR", year)] = rowValue / 1000;

                    dataRows = factTable.Select(string.Format("RefYearDayUNV = {0} and RefRegions = {1} and (RefBudLevel = 6)",
                        refYearDayUNV, region));
                    rowValue = GetValue(dataRows, "TaxResource", ref isBlocked, ref isResult);
                    newRow[string.Format("{0}_BPosResult", year)] =
                        GetChildSum(dtChildFacts, 6, refYearDayUNV, kdRows, "TaxResource", ref childSum, ref isChildBlocked);
                    if (isResult)
                        newRow[string.Format("{0}_BPosResult", year)] = isResult;
                    rowValue += childSum;
                    resultValue += rowValue;
                    newRow[string.Format("{0}_BPos", year)] = rowValue / 1000;
                    break;
            }
            isBlocked = isBlocked || isChildBlocked;
            return resultValue;
        }

        private decimal GetValue(DataRow[] dataRows, string dataColum, ref bool isBlocked, ref bool isResult)
        {
            decimal value = 0;
            int newRefKVSR = dataRows.Length > 0 ? Convert.ToInt32(dataRows[0]["RefKVSR"]) : 0;
            int oldRefKVSR = dataRows.Length > 0 ? Convert.ToInt32(dataRows[0]["RefKVSR"]) : 0;
            foreach (DataRow dataRow in dataRows)
            {
                newRefKVSR = Convert.ToInt32(dataRow["RefKVSR"]);
                isResult = isResult || (newRefKVSR != oldRefKVSR);
                if (!dataRow.IsNull(dataColum))
                    value += Convert.ToDecimal(dataRow[dataColum]);
                if (!dataRow.IsNull("IsBlocked"))
                    isBlocked = isBlocked || Convert.ToBoolean(dataRow["IsBlocked"]);
                oldRefKVSR = newRefKVSR;
            }
            return value;
        }

        private int GetBudLevel(int terType, IncomesEvalPlanParams incomesEvalPlanParams)
        {
            if (incomesEvalPlanParams.Municipal == Municipal.MrGo
                && incomesEvalPlanParams.BudgetLevel == BudgetLevel.ConsSubjectBudget)
                return 14;

            switch (terType)
            {
                case 4:
                    return 5;
                case 5:
                    return 16;
                case 6:
                    return 17;
                case 7:
                    return 15;
                case 3:
                    return 3;
                case 14:
                    return 14;
            }
            return 0;
        }

        private DataTable GetFactTable(IDatabase db, object refRegion, IncomesEvalPlanParams incomesEvalPlanParams, int year)
        {
            if (Convert.ToInt32(incomesEvalPlanParams.KvsrPlan) == -1)
            {
                return (DataTable)db.ExecQuery(
                @"select Forecast, Estimate, TaxResource, RefKD, RefYearDayUNV, RefRegions, RefBudLevel,
                    IsBlocked from f_D_FOPlanIncDivide where RefRegions = ? and RefYearDayUNV like ? and SourceId = ? and RefVariant = ?",
                QueryResultTypes.DataTable,
                new DbParameterDescriptor("p0", refRegion),
                new DbParameterDescriptor("p1", string.Format("{0}0001", year)),
                new DbParameterDescriptor("p2", incomesEvalPlanParams.SourceId),
                new DbParameterDescriptor("p3", incomesEvalPlanParams.IncomesVariant));
            }

            return (DataTable)db.ExecQuery(
                @"select Forecast, Estimate, TaxResource, RefKD, RefYearDayUNV, RefRegions, RefBudLevel,
                    IsBlocked from f_D_FOPlanIncDivide where RefRegions = ? and RefYearDayUNV like ? and SourceId = ? and RefVariant = ?",
                QueryResultTypes.DataTable,
                new DbParameterDescriptor("p0", refRegion),
                new DbParameterDescriptor("p1", string.Format("{0}0001", year)),
                new DbParameterDescriptor("p2", incomesEvalPlanParams.SourceId),
                new DbParameterDescriptor("p3", incomesEvalPlanParams.IncomesVariant));
        }

        private bool GetChildSum(DataTable factData, int budLevel,
            string yearDayUNV, DataRow[] kdChildRows, string dataColumn, ref decimal sum, ref bool isBlocked)
        {
            foreach (DataRow childRow in kdChildRows)
            {
                foreach (DataRow row in factData.Rows.Cast<DataRow>().
                    Where(w => Convert.ToInt32(w["RefBudLevel"]) == budLevel &&
                        Convert.ToInt64(w["RefKD"]) == Convert.ToInt64(childRow["ID"]) &&
                        w["RefYearDayUNV"].ToString() == yearDayUNV && !w.IsNull(dataColumn)))
                {
                    sum += row.Field<decimal>(dataColumn);
                    if (!row.IsNull("IsBlocked"))
                        isBlocked = Convert.ToBoolean(row["IsBlocked"]) || isBlocked;
                }
                GetChildSum(factData, budLevel, yearDayUNV, childRow.GetChildRows("Rel"), dataColumn, ref sum, ref isBlocked);
            }
            return sum != 0;
        }


        #region получение промежуточных сумм

        public void SetParentSum(ref DataTable dtData)
        {
            SetConsSum(ref dtData);
            SetResults(ref dtData);
        }

        private void SetConsSum(ref DataTable dtData)
        {
            foreach (DataRow dataRow in dtData.Rows.Cast<DataRow>().Where(w => !Convert.ToBoolean(w["IsResult"])))
            {
                foreach (DataColumn column in dtData.Columns)
                {
                    if (column.DataType != typeof(decimal))
                        continue;
                    if (!column.ColumnName.Contains("_"))
                        continue;
                    if (column.ColumnName.Contains("KBS"))
                    {
                        dataRow[column] = 0;
                        continue;
                    }
                    string month = column.ColumnName.Split('_')[0];
                    string kbsColumn = month + "_KBS";
                    if (dtData.Columns.Contains(kbsColumn))
                    {
                        dataRow[month + "_KBS"] = Convert.ToDecimal(dataRow[month + "_KBS"]) +
                                                  Convert.ToDecimal(dataRow[column]);
                    }
                }
            }
        }

        private void SetResults(ref DataTable dtData)
        {
            foreach (DataRow parentRow in dtData.Select("RegionParentId is null"))
            {
                SetResults(ref dtData, parentRow);
            }
        }

        private void SetResults(ref DataTable dtData, DataRow parentRow)
        {
            if (parentRow.IsNull("RegionID"))
                return;
            foreach (DataRow childRow in dtData.Select(string.Format("RegionParentId = {0}", parentRow["RegionID"])))
            {
                SetResults(ref dtData, childRow);
                foreach (DataColumn column in dtData.Columns)
                {
                    if (column.DataType == typeof(decimal))
                    {
                        decimal value = parentRow.IsNull(column) ? 0 : Convert.ToDecimal(parentRow[column]);
                        parentRow[column] = value + Convert.ToDecimal(childRow[column]);
                    }
                    if (column.DataType == typeof(bool) && column.ColumnName.Contains("IsBlocked"))
                    {
                        bool isBlocked = parentRow.IsNull(column) ? false : Convert.ToBoolean(parentRow[column]);
                        parentRow[column] = isBlocked || Convert.ToBoolean(childRow[column]);
                    }
                }
            }
        }

        private void DeleteRegionsSum(ref DataTable dtData, IncomesEvalPlanParams incomesEvalPlanParams)
        {
            DataTable dtResults = dtData.Clone();
            if (incomesEvalPlanParams.ShowResults)
            {
                foreach (DataRow row in dtData.Rows)
                {
                    int terrType = Convert.ToInt32(row["TerritoryType"]);
                    if (terrType == 3 || (terrType != 5 && terrType != 6 && terrType != 30 && terrType != 4))
                    {
                        dtResults.Rows.Add(row.ItemArray);
                    }
                }
                dtData = dtResults;
            }
        }

        #endregion


        private DataTable GetEmptyTable(IncomesEvalPlanParams incomesEvalPlanParams)
        {
            DataTable dtEmpty = new DataTable();
            DataColumn column = dtEmpty.Columns.Add("Num", typeof(string));
            column.Caption = string.Empty;
            column = dtEmpty.Columns.Add("SourceId", typeof(int));
            column.Caption = string.Empty;
            column = dtEmpty.Columns.Add("Id", typeof(int));
            column.Caption = string.Empty;
            column = dtEmpty.Columns.Add("TerritoryType", typeof(int));
            column.Caption = string.Empty;
            column = dtEmpty.Columns.Add("TerritoryTypeName", typeof(string));
            column.Caption = string.Empty;
            column = dtEmpty.Columns.Add("RegionId", typeof(long));
            column.Caption = string.Empty;
            column = dtEmpty.Columns.Add("Region", typeof(long));
            column.Caption = string.Empty;
            column = dtEmpty.Columns.Add("RegionCode", typeof(long));
            column.Caption = string.Empty;
            column = dtEmpty.Columns.Add("RegionParentId", typeof(long));
            column.Caption = string.Empty;
            column = dtEmpty.Columns.Add("RegionName", typeof(string));
            column.Caption = string.Empty;
            column = dtEmpty.Columns.Add("RefKD", typeof(string));
            column.Caption = string.Empty;
            column = dtEmpty.Columns.Add("Year", typeof(string));
            column.Caption = string.Empty;
            column = dtEmpty.Columns.Add("IsResult", typeof(bool));
            column.Caption = string.Empty;

            if (incomesEvalPlanParams.IsEstimate)
            {
                AddColumns(incomesEvalPlanParams.Year - 1, incomesEvalPlanParams, true, ref dtEmpty);
            }
            if (incomesEvalPlanParams.IsForecast || incomesEvalPlanParams.IsTaxResource)
            {
                for (int year = incomesEvalPlanParams.Year; year <= incomesEvalPlanParams.Year + 2; year++)
                {
                    AddColumns(year, incomesEvalPlanParams, false, ref dtEmpty);
                }
            }

            return dtEmpty;
        }

        private void AddColumns(int year, IncomesEvalPlanParams incomesEvalPlanParams, bool isEstimate, ref DataTable dtEmpty)
        {
            DataColumn column = null;
            switch (incomesEvalPlanParams.BudgetLevel)
            {
                case BudgetLevel.RegionBudget:
                    if (isEstimate || incomesEvalPlanParams.IsForecast)
                    {
                        column = dtEmpty.Columns.Add(string.Format("{0}_{1}", year, "OB"), typeof (decimal));
                        column.Caption = "ОБ";
                        column = dtEmpty.Columns.Add(string.Format("{0}_{1}", year, "OBResult"), typeof (bool));
                        column.Caption = string.Empty;
                    }
                    if (incomesEvalPlanParams.IsTaxResource && !isEstimate)
                    {
                        column = dtEmpty.Columns.Add(string.Format("{0}_{1}", year, "BMR"), typeof(decimal));
                        column.Caption = "Налоговый потенциал в БМР";
                        column = dtEmpty.Columns.Add(string.Format("{0}_{1}", year, "BMRResult"), typeof(bool));
                        column.Caption = string.Empty;
                        column = dtEmpty.Columns.Add(string.Format("{0}_{1}", year, "BPos"), typeof(decimal));
                        column.Caption = "Налоговый потенциал в Бпос";
                        column = dtEmpty.Columns.Add(string.Format("{0}_{1}", year, "BPosResult"), typeof(bool));
                        column.Caption = string.Empty;
                    }
                    column = dtEmpty.Columns.Add(year + "_IsBlocked", typeof(bool));
                    column.Caption = string.Empty;
                    break;
                case BudgetLevel.ConsSubjectBudget:
                    if (isEstimate || incomesEvalPlanParams.IsForecast)
                    {
                        column = dtEmpty.Columns.Add(string.Format("{0}_{1}", year, "KBS"), typeof(decimal));
                        column.Caption = "КБС";
                        column = dtEmpty.Columns.Add(string.Format("{0}_{1}", year, "OB"), typeof (decimal));
                        column.Caption = "ОБ";
                        column = dtEmpty.Columns.Add(string.Format("{0}_{1}", year, "OBResult"), typeof (bool));
                        column.Caption = string.Empty;
                        column = dtEmpty.Columns.Add(string.Format("{0}_{1}", year, "KMB"), typeof (decimal));
                        column.Caption = "КМБ";
                        column = dtEmpty.Columns.Add(string.Format("{0}_{1}", year, "KMBResult"), typeof (bool));
                        column.Caption = string.Empty;
                    }
                    if (incomesEvalPlanParams.IsTaxResource && !isEstimate)
                    {
                        column = dtEmpty.Columns.Add(string.Format("{0}_{1}", year, "BMR"), typeof(decimal));
                        column.Caption = "Налоговый потенциал в БМР";
                        column = dtEmpty.Columns.Add(string.Format("{0}_{1}", year, "BMRResult"), typeof(bool));
                        column.Caption = string.Empty;
                        column = dtEmpty.Columns.Add(string.Format("{0}_{1}", year, "BPos"), typeof(decimal));
                        column.Caption = "Налоговый потенциал в Бпос";
                        column = dtEmpty.Columns.Add(string.Format("{0}_{1}", year, "BPosResult"), typeof(bool));
                        column.Caption = string.Empty;
                    }
                    column = dtEmpty.Columns.Add(year + "_IsBlocked", typeof(bool));
                    column.Caption = string.Empty;
                    break;
                case BudgetLevel.LocalBudget:
                    if (isEstimate || incomesEvalPlanParams.IsForecast)
                    {
                        column = dtEmpty.Columns.Add(string.Format("{0}_{1}", year, "KMB"), typeof (decimal));
                        column.Caption = "КМБ";
                        column = dtEmpty.Columns.Add(string.Format("{0}_{1}", year, "KMBResult"), typeof (bool));
                        column.Caption = string.Empty;
                    }
                    if (incomesEvalPlanParams.IsTaxResource && !isEstimate)
                    {
                        column = dtEmpty.Columns.Add(string.Format("{0}_{1}", year, "BMR"), typeof(decimal));
                        column.Caption = "Налоговый потенциал в БМР";
                        column = dtEmpty.Columns.Add(string.Format("{0}_{1}", year, "BMRResult"), typeof(bool));
                        column.Caption = string.Empty;
                        column = dtEmpty.Columns.Add(string.Format("{0}_{1}", year, "BPos"), typeof(decimal));
                        column.Caption = "Налоговый потенциал в Бпос";
                        column = dtEmpty.Columns.Add(string.Format("{0}_{1}", year, "BPosResult"), typeof(bool));
                        column.Caption = string.Empty;
                    }
                    column = dtEmpty.Columns.Add(year + "_IsBlocked", typeof(bool));
                    column.Caption = string.Empty;
                    break;
            }
        }

        #region сохранение изменений в базу

        public void SaveData(DataTable dtSaveData, IncomesEvalPlanParams incomesEvalPlanParams)
        {
            DataTable dtChanges = dtSaveData.GetChanges();
            if (dtChanges == null)
                return;
            using (IDatabase db = Scheme.SchemeDWH.DB)
            {
                foreach (DataRow row in dtChanges.Rows)
                {
                    bool isResult = Convert.ToBoolean(row["IsResult"]);
                    if (!isResult)
                    {
                        SaveRowChanges(row, db, incomesEvalPlanParams);
                    }
                }
            }
        }

        private void SaveRowChanges(DataRow row, IDatabase db, IncomesEvalPlanParams incomesEvalPlanParams)
        {
            foreach (DataColumn column in row.Table.Columns)
            {
                if (column.DataType == typeof(decimal))
                {
                    if (column.ColumnName.Split('_')[1] == "KBS")
                        continue;
                    // пропускаем данные, у которых есть подчиненные данные
                    if (Convert.ToBoolean(row[column.ColumnName + "Result"]))
                        continue;

                    int year = Convert.ToInt32(column.ColumnName.Split('_')[0]);

                    string columnName = incomesEvalPlanParams.Year > year ? "Estimate" : "Forecast";
                    int budLevel = GetBudgetLevel(row, column, incomesEvalPlanParams);
                    string refYearDayUNV = GetYearDayUNV(year);

                    if (budLevel == 3 && column.ColumnName.Split('_')[1] == "KMB")
                        budLevel = 14;

                    if (column.ColumnName.Split('_')[1] == "BMR")
                    {
                        budLevel = 5;
                        columnName = "TaxResource";
                    }

                    if (column.ColumnName.Split('_')[1] == "BPos")
                    {
                        budLevel = 6;
                        columnName = "TaxResource";
                    }

                    if (row.IsNull(column))
                    {
                        // удаляем или обновляем запись до null
                        DeleteRow(db, row, DBNull.Value, refYearDayUNV, budLevel, incomesEvalPlanParams.KvsrPlan, incomesEvalPlanParams.IncomesVariant, columnName);
                    }
                    else
                    {
                        decimal value = Convert.ToDecimal(row[column]);
                        decimal originalValue = Convert.ToDecimal(row[column, DataRowVersion.Original]);
                        if (originalValue == value)
                            continue;
                        if (originalValue != value)
                        {
                            value = value * 1000;
                            if (value == 0)
                            {
                                // удаляем запись или обновляем
                                DeleteRow(db, row, value, refYearDayUNV, budLevel, incomesEvalPlanParams.KvsrPlan,
                                    incomesEvalPlanParams.IncomesVariant, columnName);
                            }
                            else
                            {
                                if (originalValue == 0)
                                {
                                    // пытаемся добавить запись
                                    InsertRow(db, row, value, refYearDayUNV, budLevel, incomesEvalPlanParams.KvsrPlan,
                                        columnName, incomesEvalPlanParams.IncomesVariant);
                                }
                                else
                                {
                                    // обновляем запись
                                    UpdateRow(db, row, value, refYearDayUNV, budLevel, incomesEvalPlanParams.KvsrPlan,
                                        incomesEvalPlanParams.IncomesVariant, columnName);
                                }
                            }
                        }
                    }
                }
            }
        }

        private int GetBudgetLevel(DataRow row, DataColumn column, IncomesEvalPlanParams incomesEvalPlanParams)
        {
            int budLevel = GetBudLevel(Convert.ToInt32(row["TerritoryType"]), incomesEvalPlanParams);
            if (column.ColumnName.Split('_')[1] == "OB")
                return 3;
            return budLevel;
        }

        private string GetYearDayUNV(int year)
        {
            return string.Format("{0}0001", year);
        }

        private void InsertRow(IDatabase db, DataRow insertRow, object value, object refYearDayUNV,
            int budLevel, object kvsr, string columnName, object variant)
        {
            if (ExistRow(db, insertRow, refYearDayUNV, budLevel, kvsr, variant))
            {
                UpdateRow(db, insertRow, value, refYearDayUNV, budLevel, kvsr, variant, columnName);
                return;
            }

            string insertQuery =
                string.Format(@"insert into f_D_FOPlanIncDivide (SourceID, TaskID, RefVariant, RefKD, RefRegions, RefBudLevel, RefYearDayUNV, 
                RefNormDeduct, RefKVSR, RefOrganizations, RefTaxObjects, RefFODepartments, {0}) 
                values (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)", columnName);
            DbParameterDescriptor[] queryParams = new DbParameterDescriptor[13];
            queryParams[0] = new DbParameterDescriptor("p0", insertRow["SourceId"]);
            queryParams[1] = new DbParameterDescriptor("p1", -1);
            queryParams[2] = new DbParameterDescriptor("p2", variant);
            queryParams[3] = new DbParameterDescriptor("p3", insertRow["RefKD"]);
            queryParams[4] = new DbParameterDescriptor("p4", insertRow["Region"]);
            queryParams[5] = new DbParameterDescriptor("p5", budLevel);
            queryParams[6] = new DbParameterDescriptor("p6", refYearDayUNV);
            queryParams[7] = new DbParameterDescriptor("p7", 6);
            queryParams[8] = new DbParameterDescriptor("p8", kvsr);
            queryParams[9] = new DbParameterDescriptor("p9", -1);
            queryParams[10] = new DbParameterDescriptor("p10", -1);
            queryParams[11] = new DbParameterDescriptor("p11", -1);
            queryParams[12] = new DbParameterDescriptor("p12", value);

            db.ExecQuery(insertQuery, QueryResultTypes.NonQuery, queryParams);
        }

        private void UpdateRow(IDatabase db, DataRow updateRow, object value, object refYearDayUNV,
           int budLevel, object kvsr, object variant, string columnName)
        {
            string updateQuery = string.Empty;
            DbParameterDescriptor[] queryParams = null;
            if (Convert.ToInt32(kvsr) == -1)
            {
                updateQuery =
                string.Format(@"update f_D_FOPlanIncDivide set {0} = ? where 
                SourceID = ? and RefKD = ? and RefRegions = ? and RefBudLevel = ? and RefYearDayUNV = ? and RefVariant = ?", columnName);
                queryParams = new DbParameterDescriptor[7];
                queryParams[0] = new DbParameterDescriptor("p0", value);
                queryParams[1] = new DbParameterDescriptor("p1", updateRow["SourceId"]);
                queryParams[2] = new DbParameterDescriptor("p2", updateRow["RefKD"]);
                queryParams[3] = new DbParameterDescriptor("p3", updateRow["Region"]);
                queryParams[4] = new DbParameterDescriptor("p4", budLevel);
                queryParams[5] = new DbParameterDescriptor("p5", refYearDayUNV);
                queryParams[6] = new DbParameterDescriptor("p6", variant);
            }
            else
            {
                updateQuery =
                string.Format(@"update f_D_FOPlanIncDivide set {0} = ? where 
                SourceID = ? and RefKD = ? and RefRegions = ? and RefBudLevel = ? and RefYearDayUNV = ? and RefKVSR = ? and RefVariant = ?", columnName);
                queryParams = new DbParameterDescriptor[8];
                queryParams[0] = new DbParameterDescriptor("p0", value);
                queryParams[1] = new DbParameterDescriptor("p1", updateRow["SourceId"]);
                queryParams[2] = new DbParameterDescriptor("p2", updateRow["RefKD"]);
                queryParams[3] = new DbParameterDescriptor("p3", updateRow["Region"]);
                queryParams[4] = new DbParameterDescriptor("p4", budLevel);
                queryParams[5] = new DbParameterDescriptor("p5", refYearDayUNV);
                queryParams[6] = new DbParameterDescriptor("p6", kvsr);
                queryParams[7] = new DbParameterDescriptor("p7", variant);
            }
            db.ExecQuery(updateQuery, QueryResultTypes.NonQuery, queryParams);
        }

        private void DeleteRow(IDatabase db, DataRow deleteRow, object value, object refYearDayUNV,
            int budLevel, object kvsr, object variant, string columnName)
        {
            if (ExistRow(db, deleteRow, refYearDayUNV, budLevel, kvsr, variant))
            {
                UpdateRow(db, deleteRow, value, refYearDayUNV, budLevel, kvsr, variant, columnName);
                return;
            }
            string deleteQuery = string.Empty;
            DbParameterDescriptor[] queryParams = null;

            if (Convert.ToInt32(kvsr) == -1)
            {
                deleteQuery =
                @"delete from f_D_FOPlanIncDivide where 
                SourceID = ? and RefKD = ? and RefRegions = ? and RefBudLevel = ? and RefYearDayUNV = ? and RefVariant = ?";
                queryParams = new DbParameterDescriptor[6];
                queryParams[0] = new DbParameterDescriptor("p0", deleteRow["SourceId"]);
                queryParams[1] = new DbParameterDescriptor("p1", deleteRow["RefKD"]);
                queryParams[2] = new DbParameterDescriptor("p2", deleteRow["Region"]);
                queryParams[3] = new DbParameterDescriptor("p3", budLevel);
                queryParams[4] = new DbParameterDescriptor("p4", refYearDayUNV);
                queryParams[5] = new DbParameterDescriptor("p5", variant);
            }
            else
            {
                deleteQuery =
                @"delete from f_D_FOPlanIncDivide where 
                SourceID = ? and RefKD = ? and RefRegions = ? and RefBudLevel = ? and RefYearDayUNV = ? and RefKVSR = ? and RefVariant = ?";
                queryParams = new DbParameterDescriptor[7];
                queryParams[0] = new DbParameterDescriptor("p0", deleteRow["SourceId"]);
                queryParams[1] = new DbParameterDescriptor("p1", deleteRow["RefKD"]);
                queryParams[2] = new DbParameterDescriptor("p2", deleteRow["Region"]);
                queryParams[3] = new DbParameterDescriptor("p3", budLevel);
                queryParams[4] = new DbParameterDescriptor("p4", refYearDayUNV);
                queryParams[5] = new DbParameterDescriptor("p5", kvsr);
                queryParams[6] = new DbParameterDescriptor("p6", variant);
            }

            db.ExecQuery(deleteQuery, QueryResultTypes.NonQuery, queryParams);
        }

        private bool ExistRow(IDatabase db, DataRow row, object refYearDayUNV, int budLevel, object kvsr, object variant)
        {
            string query = string.Empty;
            DbParameterDescriptor[] queryParams = null;
            if (Convert.ToInt32(kvsr) == -1)
            {
                query =
                @"select id from f_D_FOPlanIncDivide where 
                SourceID = ? and RefKD = ? and RefRegions = ? and RefBudLevel = ? and RefYearDayUNV = ? and RefVariant = ?";
                queryParams = new DbParameterDescriptor[6];
                queryParams[0] = new DbParameterDescriptor("p0", row["SourceId"]);
                queryParams[1] = new DbParameterDescriptor("p1", row["RefKD"]);
                queryParams[2] = new DbParameterDescriptor("p2", row["Region"]);
                queryParams[3] = new DbParameterDescriptor("p3", budLevel);
                queryParams[4] = new DbParameterDescriptor("p4", refYearDayUNV);
                queryParams[5] = new DbParameterDescriptor("p6", variant);
            }
            else
            {
                query =
                @"select id from f_D_FOPlanIncDivide where 
                SourceID = ? and RefKD = ? and RefRegions = ? and RefBudLevel = ? and RefYearDayUNV = ? and RefKVSR = ? and RefVariant = ?";
                queryParams = new DbParameterDescriptor[7];
                queryParams[0] = new DbParameterDescriptor("p0", row["SourceId"]);
                queryParams[1] = new DbParameterDescriptor("p1", row["RefKD"]);
                queryParams[2] = new DbParameterDescriptor("p2", row["Region"]);
                queryParams[3] = new DbParameterDescriptor("p3", budLevel);
                queryParams[4] = new DbParameterDescriptor("p4", refYearDayUNV);
                queryParams[5] = new DbParameterDescriptor("p5", kvsr);
                queryParams[6] = new DbParameterDescriptor("p6", variant);
            }

            object queryResult = db.ExecQuery(query, QueryResultTypes.Scalar, queryParams);
            return queryResult != DBNull.Value && queryResult != null;
        }

        #endregion

        #region копирование данных с варианта на вариант

        public void CopyVariantData(IncomesEvalPlanParams incomesEvalPlanParams, object sourceVariant, object destVariant, CalculateValueType[] calculateValueTypes)
        {
            using (IDatabase db = Scheme.SchemeDWH.DB)
            {
                DataSet kdData = GetKdPlanIncomes(db, incomesEvalPlanParams);
                DataRow kdRow = kdData.Tables[0].Select(string.Format("ID = {0}", incomesEvalPlanParams.PlanIncomes))[0];
                CopyVariantData(incomesEvalPlanParams, sourceVariant, destVariant, calculateValueTypes, kdRow, db);
            }
        }

        private void CopyVariantData(IncomesEvalPlanParams incomesEvalPlanParams, object sourceVariant,
            object destVariant, CalculateValueType[] calculateValueTypes, DataRow parentKdRow, IDatabase db)
        {
            foreach (CalculateValueType calcType in calculateValueTypes)
            {
                UpdateVariantData(incomesEvalPlanParams, destVariant, calcType, parentKdRow["ID"], db);
                DeleteVariantData(incomesEvalPlanParams, destVariant, calcType, parentKdRow["ID"], db);
                InsertVariantData(incomesEvalPlanParams, sourceVariant, destVariant, calcType, parentKdRow, db);
            }

            foreach (DataRow childRow in parentKdRow.GetChildRows("Rel"))
            {
                CopyVariantData(incomesEvalPlanParams, sourceVariant, destVariant, calculateValueTypes, childRow, db);
            }
        }

        private void UpdateVariantData(IncomesEvalPlanParams incomesEvalPlanParams, object destVariant,
            CalculateValueType calculateValueType, object refKD, IDatabase db)
        {
            List<DbParameterDescriptor> paramsList = new List<DbParameterDescriptor>();
            paramsList.Add(new DbParameterDescriptor("p0", incomesEvalPlanParams.SourceId));
            paramsList.Add(new DbParameterDescriptor("p1", destVariant));
            paramsList.Add(new DbParameterDescriptor("p2", refKD));
            string query = string.Empty;
            if (Convert.ToInt32(incomesEvalPlanParams.KvsrPlan) == -1)
            {
                query = "update f_D_FOPlanIncDivide set {0} = null where SourceID = ? and RefVariant = ? and RefKD = ?";
}
            else
            {
                query =
                    "update f_D_FOPlanIncDivide set {0} = null where SourceID = ? and RefVariant = ? and RefKD = ? and RefKVSR = ?";
                paramsList.Add(new DbParameterDescriptor("p3", incomesEvalPlanParams.KvsrPlan));
            }

            switch (calculateValueType)
            {
                case CalculateValueType.Estimate:
                    query = string.Format(query, "Estimate");
                    break;
                case CalculateValueType.Forecast:
                    query = string.Format(query, "Forecast");
                    break;
                case CalculateValueType.TaxResource:
                    query = string.Format(query, "TaxResource");
                    break;
            }

            db.ExecQuery(query, QueryResultTypes.NonQuery, paramsList.ToArray());
        }

        private void DeleteVariantData(IncomesEvalPlanParams incomesEvalPlanParams, object destVariant,
            CalculateValueType calculateValueType, object refKD, IDatabase db)
        {
            List<DbParameterDescriptor> paramsList = new List<DbParameterDescriptor>();
            paramsList.Add(new DbParameterDescriptor("p0", incomesEvalPlanParams.SourceId));
            paramsList.Add(new DbParameterDescriptor("p1", destVariant));
            paramsList.Add(new DbParameterDescriptor("p2", refKD));
            string query = string.Empty;
            if (Convert.ToInt32(incomesEvalPlanParams.KvsrPlan) == -1)
            {
                query = "delete from f_D_FOPlanIncDivide where SourceID = ? and RefVariant = ? and RefKD = ? and {0}";
            }
            else
            {
                query =
                    "delete from f_D_FOPlanIncDivide where SourceID = ? and RefVariant = ? and RefKD = ? and RefKVSR = ? and {0}";
                paramsList.Add(new DbParameterDescriptor("p3", incomesEvalPlanParams.KvsrPlan));
            }
            switch (calculateValueType)
            {
                case CalculateValueType.Estimate:
                    query = string.Format(query, "Forecast is null and TaxResource is null");
                    break;
                case CalculateValueType.Forecast:
                    query = string.Format(query, "Estimate is null and TaxResource is null");
                    break;
                case CalculateValueType.TaxResource:
                    query = string.Format(query, "Forecast is null and Estimate is null");
                    break;
            }
            db.ExecQuery(query, QueryResultTypes.NonQuery, paramsList.ToArray());
        }

        private void InsertVariantData(IncomesEvalPlanParams incomesEvalPlanParams, object sourceVariant,
            object destVariant, CalculateValueType calculateValueType, DataRow parentKdRow, IDatabase db)
        {
            string query = string.Empty;
            List<DbParameterDescriptor> paramsList = new List<DbParameterDescriptor>();
            paramsList.Add(new DbParameterDescriptor("p0", incomesEvalPlanParams.SourceId));
            paramsList.Add(new DbParameterDescriptor("p1", sourceVariant));
            paramsList.Add(new DbParameterDescriptor("p2", parentKdRow["ID"]));
            if (Convert.ToInt32(incomesEvalPlanParams.KvsrPlan) == -1)
            {
                query = @"insert into f_D_FOPlanIncDivide ({0}, TaskID, SourceID, RefVariant, RefNormDeduct, RefKD, RefRegions, RefBudLevel, RefKVSR, RefYearDayUNV, RefOrganizations, RefTaxObjects, RefFODepartments)
                    select {0}, TaskID, SourceID, {1} RefVariant, RefNormDeduct, RefKD, RefRegions, RefBudLevel, RefKVSR, RefYearDayUNV, RefOrganizations, RefTaxObjects, RefFODepartments
                    from f_D_FOPlanIncDivide where SourceID = ? and RefVariant = ? and RefKD = ?";
            }
            else
            {
                query =
                    @"insert into f_D_FOPlanIncDivide ({0}, TaskID, SourceID, RefVariant, RefNormDeduct, RefKD, RefRegions, RefBudLevel, RefKVSR, RefYearDayUNV, RefOrganizations, RefTaxObjects, RefFODepartments)
                    select {0}, TaskID, SourceID, {1} RefVariant, RefNormDeduct, RefKD, RefRegions, RefBudLevel, RefKVSR, RefYearDayUNV, RefOrganizations, RefTaxObjects, RefFODepartments
                    from f_D_FOPlanIncDivide where SourceID = ? and RefVariant = ? and RefKD = ? and RefKVSR = ?";
                paramsList.Add(new DbParameterDescriptor("p3", incomesEvalPlanParams.KvsrPlan));
            }

            switch (calculateValueType)
            {
                case CalculateValueType.Estimate:
                    query = string.Format(query, "Estimate", destVariant) + " and Estimate is not null";
                    break;
                case CalculateValueType.Forecast:
                    query = string.Format(query, "Forecast", destVariant) + " and Forecast is not null";
                    break;
                case CalculateValueType.TaxResource:
                    query = string.Format(query, "TaxResource", destVariant) + " and TaxResource is not null";
                    break;
            }

            db.ExecQuery(query, QueryResultTypes.NonQuery, paramsList.ToArray());
        }

        #endregion
    }
}
