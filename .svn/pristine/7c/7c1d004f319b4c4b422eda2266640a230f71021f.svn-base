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
    public enum ConsTerritoryType
    {
        ConsMo = 20,
        ConsPos = 30,
        MoResult = 40,
        GoResult = 50,
        RegionsResult = 60,
        TotalResult = 70,
        MonthRep = 80
    }

    public class IncomesYearPlanService
    {
        private DataSet KdData
        {
            get;
            set;
        }

        private IScheme scheme;

        public IncomesYearPlanService(IScheme scheme)
        {
            this.scheme = scheme;
        }

        public DataTable GetIncomesData(IncomesYearPlanParams incomesYearPlanParams)
        {
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                DataTable dtData = new DataTable();
                GetData2(incomesYearPlanParams, db, ref dtData);
                SetResults(ref dtData);
                GetMesOtch(db, incomesYearPlanParams, ref dtData);
                DeleteRegionsSum(ref dtData, incomesYearPlanParams);
                dtData.AcceptChanges();
                return dtData;
            }
        }

        public DataTable GetRegions(int sourceId)
        {
            using (IDatabase db = scheme.SchemeDWH.DB)
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

        private void GetKdPlanIncomes(IDatabase db, IncomesYearPlanParams incomesYearPlanParams)
        {
            KdData = new DataSet();
            string query = "select id, parentId from d_KD_PlanIncomes where SourceId = ? and id >= 0";
            DataTable kdPlan =
                (DataTable)
                db.ExecQuery(query, QueryResultTypes.DataTable,
                             new DbParameterDescriptor("p0", incomesYearPlanParams.SourceId));
            KdData.Tables.Add(kdPlan);
            DataRelation relation = new DataRelation("rel", kdPlan.Columns["ID"], kdPlan.Columns["ParentID"], false);
            KdData.Relations.Add(relation);
        }

        private void GetMesOtch(IDatabase db, IncomesYearPlanParams incomesYearPlanParams, ref DataTable dtData)
        {
            DataRow newRow = dtData.NewRow();
            newRow["RegionName"] = "Годовые назначения по МесОтч";
            newRow["IsResult"] = true;
            newRow["ID"] = dtData.Rows.Count + 1;
            newRow["TerritoryType"] = ConsTerritoryType.MonthRep;
            int firstMonth = incomesYearPlanParams.Month == 0 ? 1 : incomesYearPlanParams.Month;
            int lastMonth = incomesYearPlanParams.Month == 0 ? 12 : incomesYearPlanParams.Month;
            for (int i = firstMonth; i <= lastMonth; i++)
            {
                string refYearDayUNV = string.Format("{0}{1}00", incomesYearPlanParams.Year,
                                                     i.ToString().PadLeft(2, '0'));
                string query = @"select YearPlanReport, RefBdgtLevels from f_F_MonthRepIncomes where 
                    SourceID = (select id from DataSources where DataCode = 2 and Year = ? and Month = ? and SUPPLIERCODE = ? and Deleted = 0) and
                    RefRegions in (select id from d_Regions_MonthRep where RefDocType = 3) and
                    RefKD in (select id from d_KD_MonthRep where CodeStr = ?) and
                    RefYearDayUNV = ?";
                DataTable dtMonthSum = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable,
                            new DbParameterDescriptor("p0", incomesYearPlanParams.Year),
                            new DbParameterDescriptor("p1", i),
                            new DbParameterDescriptor("p2", "ФО"),
                            new DbParameterDescriptor("p3", incomesYearPlanParams.IncomesCode),
                            new DbParameterDescriptor("p4", refYearDayUNV));
                if (dtMonthSum != null)
                {
                    DataRow[] dataRows = null;
                    switch (incomesYearPlanParams.BudgetLevel)
                    {
                        case BudgetLevel.RegionBudget:
                            decimal planValue = 0;
                            dataRows = dtMonthSum.Select(string.Format("RefBdgtLevels = 3"));
                            foreach (DataRow dataRow in dataRows)
                            {
                                if (!dataRow.IsNull("YearPlanReport"))
                                    planValue += Convert.ToDecimal(dataRow["YearPlanReport"]);
                            }
                            newRow[string.Format("{0}_OB", i)] = planValue / 1000;
                            break;
                        case BudgetLevel.LocalBudget:
                            planValue = 0;
                            dataRows = dtMonthSum.Select(string.Format("RefBdgtLevels = 3"));
                            foreach (DataRow dataRow in dataRows)
                            {
                                if (!dataRow.IsNull("YearPlanReport"))
                                    planValue += Convert.ToDecimal(dataRow["YearPlanReport"]);
                            }
                            newRow[string.Format("{0}_OB", i)] = planValue / 1000;
                            break;
                        case BudgetLevel.ConsSubjectBudget:
                            decimal kbsSum = 0;
                            planValue = 0;
                            dataRows = dtMonthSum.Select(string.Format("RefBdgtLevels = 3"));
                            foreach (DataRow dataRow in dataRows)
                            {
                                if (!dataRow.IsNull("YearPlanReport"))
                                    planValue += Convert.ToDecimal(dataRow["YearPlanReport"]);
                            }
                            newRow[string.Format("{0}_OB", i)] = planValue / 1000;

                            kbsSum = planValue;
                            planValue = 0;
                            dataRows =
                                dtMonthSum.Select(string.Format("RefBdgtLevels = 4 or RefBdgtLevels = 5 or RefBdgtLevels = 6"));
                            foreach (DataRow dataRow in dataRows)
                            {
                                if (!dataRow.IsNull("YearPlanReport"))
                                    planValue += Convert.ToDecimal(dataRow["YearPlanReport"]);
                            }
                            newRow[string.Format("{0}_KMB", i)] = planValue / 1000;

                            kbsSum += planValue;
                            // заполняем три поля данными
                            newRow[string.Format("{0}_KBS", i)] = kbsSum / 1000;
                            break;
                    }
                }
                else
                {
                    switch (incomesYearPlanParams.BudgetLevel)
                    {
                        case BudgetLevel.RegionBudget:
                            newRow[string.Format("{0}_OB", i)] = 0;
                            break;
                        case BudgetLevel.LocalBudget:
                            newRow[string.Format("{0}_OB", i)] = 0;
                            break;
                        case BudgetLevel.ConsSubjectBudget:
                            newRow[string.Format("{0}_OB", i)] = 0;
                            newRow[string.Format("{0}_KMB", i)] = 0;
                            // заполняем три поля данными
                            newRow[string.Format("{0}_KBS", i)] = 0;
                            break;
                    }
                }
            }
            dtData.Rows.Add(newRow);
        }

        private void GetData2(IncomesYearPlanParams incomesYearPlanParams, IDatabase db, ref DataTable emptyData)
        {
            string refYearDayUNV = string.Format("{0}__00", incomesYearPlanParams.Year);
            List<DbParameterDescriptor> queryParams = new List<DbParameterDescriptor>();
            string query = @"select YearPlan, RefYearDayUNV, RefRegions, RefBudLevel, RefKD, RefKVSR, IsBlocked from f_D_FOPlanIncDivide where 
                RefVariant = -2 and RefKD = ? and RefYearDayUNV like ?";
            queryParams.Add(new DbParameterDescriptor("p0", incomesYearPlanParams.IncomesSource));
            queryParams.Add(new DbParameterDescriptor("p1", refYearDayUNV));
            if (incomesYearPlanParams.Kvsr > 0)
            {
                query += " and RefKVSR = ?";
                queryParams.Add(new DbParameterDescriptor("p2", incomesYearPlanParams.Kvsr));
            }
            
            DataTable dtData = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable,
                queryParams.ToArray());
            // список подчиненных записей по коду дохода для нахождения суммы подчиненных элементов
            GetKdPlanIncomes(db, incomesYearPlanParams);
            DataRow[] kdRows =
                KdData.Tables[0].Select(string.Format("ParentID = {0}", incomesYearPlanParams.IncomesSource));
            DataTable dtRegions = GetRegions(incomesYearPlanParams.SourceId);
            emptyData = GetEmptyTable(incomesYearPlanParams);
            emptyData.BeginLoadData();
            DataRow parentRegionRow = dtRegions.Select("ParentId is null")[0];
            object topLevelId = parentRegionRow["ID"];
            int num = 1;
            int id = 0;
            GetData2(db, incomesYearPlanParams, parentRegionRow, dtData, kdRows, ref emptyData, ref num, ref id);
            foreach (DataRow row in emptyData.Select(string.Format("RegionParentId = {0}", topLevelId)))
            {
                row["RegionParentId"] = DBNull.Value;
            }
            emptyData.EndLoadData();
        }

        private void GetData2(IDatabase db, IncomesYearPlanParams incomesYearPlanParams,
            DataRow parentRegionRow, DataTable factData, DataRow[] kdRows, ref DataTable dtData, ref int num, ref int id)
        {
            foreach (DataRow regionRow in parentRegionRow.Table.Select(string.Format("ParentId = {0}", parentRegionRow["ID"]), "Code asc"))
            {
                AddNewDataRow(db, incomesYearPlanParams, regionRow, kdRows, factData, ref dtData, ref num, ref id);
                GetData2(db, incomesYearPlanParams, regionRow, factData, kdRows, ref dtData, ref num, ref id);
            }
        }

        private void AddNewDataRow(IDatabase db, IncomesYearPlanParams incomesYearPlanParams, DataRow regionRow, DataRow[] kdRows,
            DataTable factTable, ref DataTable dataTable, ref int num, ref int id)
        {
            bool isResult = false;
            int terType = Convert.ToInt32(regionRow["TerTypeId"]);
            object parentRegionId = regionRow["ParentId"];
            string regionName = regionRow["Name"].ToString();
            if (incomesYearPlanParams.Municipal == Municipal.MrGo &&
                (terType == 5 || terType == 6))
                return;
            if (regionName.Contains("Бюджеты поселений"))
            {
                if (incomesYearPlanParams.Municipal == Municipal.MrGo)
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
            newRow["RefKD"] = incomesYearPlanParams.IncomesSource;
            newRow["Year"] = incomesYearPlanParams.Year;
            newRow["IsResult"] = isResult;
            DataTable dtChildFacts = GetFactTable(db, regionRow["id"], incomesYearPlanParams.Year,
                                                 incomesYearPlanParams.SourceId, incomesYearPlanParams.Kvsr);
            dtChildFacts.BeginLoadData();
            int firstMonth = incomesYearPlanParams.Month == 0 ? 1 : incomesYearPlanParams.Month;
            int lastMonth = incomesYearPlanParams.Month == 0 ? 12 : incomesYearPlanParams.Month;

            for (int i = firstMonth; i <= lastMonth; i++)
            {
                string refYearDayUNV = string.Format("{0}{1}00", incomesYearPlanParams.Year, i.ToString().PadLeft(2, '0'));
                DataRow[] dataRows = null;
                decimal planValue = 0;
                decimal childSum = 0;
                bool isBlocked = false;
                bool isChildBlocked = false;
                long oldKvsr = 0;
                long newKvsr = 0;
                isResult = false;

                switch (incomesYearPlanParams.BudgetLevel)
                {
                    case BudgetLevel.RegionBudget:
                        dataRows = factTable.Select(string.Format("RefYearDayUNV = {0} and RefRegions = {1} and RefBudLevel = 3",
                            refYearDayUNV, regionRow["id"]));

                        oldKvsr = dataRows.Length > 0? Convert.ToInt64(dataRows[0]["RefKVSR"]) : 0;
                        foreach (DataRow dataRow in dataRows)
                        {
                            newKvsr = Convert.ToInt64(dataRow["RefKVSR"]);
                            isResult = isResult || (oldKvsr != newKvsr);
                            if (!dataRow.IsNull("YearPlan"))
                                planValue += Convert.ToDecimal(dataRow["YearPlan"]);
                            if (!dataRow.IsNull("IsBlocked"))
                                isBlocked = isBlocked || Convert.ToBoolean(dataRow["IsBlocked"]);
                            oldKvsr = newKvsr;
                        }
                        newRow[string.Format("{0}_OBResult", i)] =
                            GetChildSum(dtChildFacts, 3, refYearDayUNV, kdRows, ref childSum, ref isChildBlocked);
                        planValue += childSum;
                        newRow[string.Format("{0}_OB", i)] = planValue / 1000;
                        if (isResult)
                            newRow[string.Format("{0}_OBResult", i)] = true;
                        newRow[i + "_IsBlocked"] = isBlocked || isChildBlocked;
                        // заполняем одно поле данными)
                        break;
                    case BudgetLevel.ConsSubjectBudget:
                        decimal kbsSum = 0;
                        dataRows = factTable.Select(string.Format("RefYearDayUNV = {0} and RefRegions = {1} and RefBudLevel = 3",
                            refYearDayUNV, regionRow["id"]));
                        oldKvsr = dataRows.Length > 0 ? Convert.ToInt64(dataRows[0]["RefKVSR"]) : 0;
                        foreach (DataRow dataRow in dataRows)
                        {
                            newKvsr = Convert.ToInt64(dataRow["RefKVSR"]);
                            isResult = isResult || (oldKvsr != newKvsr);
                            if (!dataRow.IsNull("YearPlan"))
                                planValue += Convert.ToDecimal(dataRow["YearPlan"]);
                            if (!dataRow.IsNull("IsBlocked"))
                                isBlocked = isBlocked || Convert.ToBoolean(dataRow["IsBlocked"]);
                            oldKvsr = newKvsr;
                        }
                        newRow[string.Format("{0}_OBResult", i)] =
                            GetChildSum(dtChildFacts, 3, refYearDayUNV, kdRows, ref childSum, ref isChildBlocked);
                        if (isResult)
                            newRow[string.Format("{0}_OBResult", i)] = true;
                        isBlocked = isBlocked || isChildBlocked;
                        planValue += childSum;
                        childSum = 0;
                        newRow[string.Format("{0}_OB", i)] = planValue / 1000;
                        kbsSum = planValue;
                        planValue = 0;
                        int budlevel = GetBudLevel(terType, incomesYearPlanParams);
                        if (budlevel == 3)
                            budlevel = 14;
                        isResult = false;
                        dataRows = factTable.Select(string.Format("RefYearDayUNV = {0} and RefRegions = {1} and RefBudLevel = {2}",
                            refYearDayUNV, regionRow["id"], budlevel));
                        oldKvsr = dataRows.Length > 0 ? Convert.ToInt64(dataRows[0]["RefKVSR"]) : 0;
                        foreach (DataRow dataRow in dataRows)
                        {
                            newKvsr = Convert.ToInt64(dataRow["RefKVSR"]);
                            isResult = isResult || (oldKvsr != newKvsr);
                            if (!dataRow.IsNull("YearPlan"))
                                planValue += Convert.ToDecimal(dataRow["YearPlan"]);
                            if (!dataRow.IsNull("IsBlocked"))
                                isBlocked = isBlocked || Convert.ToBoolean(dataRow["IsBlocked"]);
                            oldKvsr = newKvsr;
                        }
                        newRow[string.Format("{0}_KMBResult", i)] =
                            GetChildSum(dtChildFacts, budlevel, refYearDayUNV, kdRows, ref childSum, ref isChildBlocked);
                        if (isResult)
                            newRow[string.Format("{0}_KMBResult", i)] = true;
                        isBlocked = isBlocked || isChildBlocked;
                        planValue += childSum;
                        newRow[string.Format("{0}_KMB", i)] = planValue / 1000;
                        kbsSum += planValue;
                        // заполняем три поля данными
                        newRow[string.Format("{0}_KBS", i)] = kbsSum / 1000;
                        newRow[i + "_IsBlocked"] = isBlocked;
                        break;
                    case BudgetLevel.LocalBudget:
                        planValue = 0;
                        budlevel = GetBudLevel(terType, incomesYearPlanParams);
                        if (budlevel == 3)
                            budlevel = 14;
                        dataRows = factTable.Select(string.Format("RefYearDayUNV = {0} and RefRegions = {1} and RefBudLevel = {2}",
                            refYearDayUNV, regionRow["id"], budlevel));
                        oldKvsr = dataRows.Length > 0 ? Convert.ToInt64(dataRows[0]["RefKVSR"]) : 0;
                        foreach (DataRow dataRow in dataRows)
                        {
                            newKvsr = Convert.ToInt64(dataRow["RefKVSR"]);
                            isResult = isResult || (oldKvsr != newKvsr);
                            if (!dataRow.IsNull("YearPlan"))
                                planValue += Convert.ToDecimal(dataRow["YearPlan"]);
                            if (!dataRow.IsNull("IsBlocked"))
                                isBlocked = isBlocked || Convert.ToBoolean(dataRow["IsBlocked"]);
                            oldKvsr = newKvsr;
                        }
                        newRow[string.Format("{0}_KMBResult", i)] =
                            GetChildSum(dtChildFacts, budlevel, refYearDayUNV, kdRows, ref childSum, ref isChildBlocked);
                        if (isResult)
                            newRow[string.Format("{0}_KMBResult", i)] = true;
                        isBlocked = isBlocked || isChildBlocked;
                        planValue += childSum;
                        newRow[string.Format("{0}_KMB", i)] = planValue / 1000;
                        newRow[i + "_IsBlocked"] = isBlocked;
                        break;
                }
            }
            newRow.EndEdit();
            dataTable.Rows.Add(newRow);
            if (terType == 4)
            {
                if (incomesYearPlanParams.Municipal == Municipal.MrGoSettlement)
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
                else if (incomesYearPlanParams.Municipal == Municipal.MrGo)
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

        private int GetBudLevel(int terType, IncomesYearPlanParams incomesYearPlanParams)
        {
            if (incomesYearPlanParams.Municipal == Municipal.MrGo
                && incomesYearPlanParams.BudgetLevel == BudgetLevel.ConsSubjectBudget)
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

        private DataTable GetEmptyTable(IncomesYearPlanParams incomesYearPlanParams)
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
            int firstMonth = incomesYearPlanParams.Month == 0 ? 1 : incomesYearPlanParams.Month;
            int lastMonth = incomesYearPlanParams.Month == 0 ? 12 : incomesYearPlanParams.Month;
            for (int i = firstMonth; i <= lastMonth; i++)
            {
                switch (incomesYearPlanParams.BudgetLevel)
                {
                    case BudgetLevel.RegionBudget:
                        column = dtEmpty.Columns.Add(string.Format("{0}_{1}", i, "OB"), typeof(decimal));
                        column.Caption = "ОБ";
                        column = dtEmpty.Columns.Add(string.Format("{0}_{1}", i, "OBResult"), typeof(bool));
                        column.Caption = string.Empty;
                        column = dtEmpty.Columns.Add(i + "_IsBlocked", typeof(bool));
                        column.Caption = string.Empty;
                        break;
                    case BudgetLevel.ConsSubjectBudget:
                        column = dtEmpty.Columns.Add(string.Format("{0}_{1}", i, "KBS"), typeof(decimal));
                        column.Caption = "КБС";
                        column = dtEmpty.Columns.Add(string.Format("{0}_{1}", i, "OB"), typeof(decimal));
                        column.Caption = "ОБ";
                        column = dtEmpty.Columns.Add(string.Format("{0}_{1}", i, "OBResult"), typeof(bool));
                        column.Caption = string.Empty;
                        column = dtEmpty.Columns.Add(string.Format("{0}_{1}", i, "KMB"), typeof(decimal));
                        column.Caption = "КМБ";
                        column = dtEmpty.Columns.Add(string.Format("{0}_{1}", i, "KMBResult"), typeof(bool));
                        column.Caption = string.Empty;
                        column = dtEmpty.Columns.Add(i + "_IsBlocked", typeof(bool));
                        column.Caption = string.Empty;
                        break;
                    case BudgetLevel.LocalBudget:
                        column = dtEmpty.Columns.Add(string.Format("{0}_{1}", i, "KMB"), typeof(decimal));
                        column.Caption = "КМБ";
                        column = dtEmpty.Columns.Add(string.Format("{0}_{1}", i, "KMBResult"), typeof(bool));
                        column.Caption = string.Empty;
                        column = dtEmpty.Columns.Add(i + "_IsBlocked", typeof(bool));
                        column.Caption = string.Empty;
                        break;
                }
            }
            return dtEmpty;
        }

        private DataTable GetFactTable(IDatabase db, object refRegion, int year, int sourceId, long refKVSR)
        {
            var queryParams = new List<DbParameterDescriptor>();
            string query = @"select YearPlan, RefKD, RefYearDayUNV, RefRegions, RefBudLevel, IsBlocked, RefKVSR 
                from f_D_FOPlanIncDivide where RefRegions = ? and RefYearDayUNV like ? and SourceId = ?";
            queryParams.Add(new DbParameterDescriptor("p0", refRegion));
            queryParams.Add(new DbParameterDescriptor("p1", string.Format("{0}__00", year)));
            queryParams.Add(new DbParameterDescriptor("p2", sourceId));
            if (refKVSR > 0)
            {
                query += " and RefKVSR = ?";
                queryParams.Add(new DbParameterDescriptor("p3", refKVSR));
            }
                
            return (DataTable)db.ExecQuery(query,
                QueryResultTypes.DataTable, queryParams.ToArray());
        }

        private bool GetChildSum(DataTable factData, int budLevel,
            string yearDayUNV, DataRow[] kdChildRows, ref decimal sum, ref bool isBlocked)
        {
            long? newKvsr = null;
            long? oldKvsr = null;
            bool difKvsr = false;

            foreach (DataRow childRow in kdChildRows)
            {
                foreach (DataRow row in factData.Rows.Cast<DataRow>().
                    Where(w => Convert.ToDecimal(w["RefBudLevel"]) == budLevel &&
                        Convert.ToDecimal(w["RefKD"]) == Convert.ToDecimal(childRow["ID"]) &&
                        w["RefYearDayUNV"].ToString() == yearDayUNV && !w.IsNull("YearPlan")))
                {
                    newKvsr = Convert.ToInt64(row["RefKVSR"]);
                    difKvsr = difKvsr || oldKvsr != null && oldKvsr.Value != newKvsr.Value;
                    sum += row.Field<decimal>("YearPlan");
                    if (!row.IsNull("IsBlocked"))
                        isBlocked = Convert.ToBoolean(row["IsBlocked"]) || isBlocked;
                    oldKvsr = newKvsr;
                }
                GetChildSum(factData, budLevel, yearDayUNV, childRow.GetChildRows("Rel"), ref sum, ref isBlocked);
            }
            return sum != 0 || difKvsr;
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

        private void DeleteRegionsSum(ref DataTable dtData, IncomesYearPlanParams incomesYearPlanParams)
        {
            DataTable dtResults = dtData.Clone();
            if (incomesYearPlanParams.ShowResults)
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

        public void SaveData(DataTable dtSaveData, IncomesYearPlanParams incomesYearPlanParams)
        {
            DataTable dtChanges = dtSaveData.GetChanges();
            if (dtChanges == null)
                return;
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                foreach (DataRow row in dtChanges.Rows)
                {
                    bool isResult = Convert.ToBoolean(row["IsResult"]);
                    if (!isResult)
                    {
                        SaveRowChanges(row, db, incomesYearPlanParams);
                    }
                }
            }
        }

        private void SaveRowChanges(DataRow row, IDatabase db, IncomesYearPlanParams incomesYearPlanParams)
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
                    int budLevel = GetBudgetLevel(row, column, incomesYearPlanParams);
                    string refYearDayUNV = GetYearDayUNV(column, row);

                    if (budLevel == 3 && column.ColumnName.Split('_')[1] == "KMB")
                        budLevel = 14;

                    if (row.IsNull(column))
                    {
                        // удаляем или обновляем запись до null
                        DeleteRow(db, row, DBNull.Value, refYearDayUNV, budLevel, incomesYearPlanParams.Kvsr);
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
                                DeleteRow(db, row, value, refYearDayUNV, budLevel, incomesYearPlanParams.Kvsr);
                            }
                            else
                            {
                                if (originalValue == 0)
                                {
                                    // пытаемся добавить запись
                                    InsertRow(db, row, value, refYearDayUNV, budLevel, incomesYearPlanParams.Kvsr);
                                }
                                else
                                {
                                    // обновляем запись
                                    UpdateRow(db, row, value, refYearDayUNV, budLevel, incomesYearPlanParams.Kvsr);
                                }
                            }
                        }
                    }
                }
            }
        }

        private int GetBudgetLevel(DataRow row, DataColumn column, IncomesYearPlanParams incomesYearPlanParams)
        {
            int budLevel = GetBudLevel(Convert.ToInt32(row["TerritoryType"]), incomesYearPlanParams);
            if (column.ColumnName.Split('_')[1] == "OB")
                return 3;
            return budLevel;
        }

        private string GetYearDayUNV(DataColumn column, DataRow row)
        {
            int year = Convert.ToInt32(row["Year"]);
            int month = Convert.ToInt32(column.ColumnName.Split('_')[0]);
            return string.Format("{0}{1}00", year, month.ToString().PadLeft(2, '0'));
        }

        private void InsertRow(IDatabase db, DataRow insertRow, object value, object refYearDayUNV, int budLevel, long refKvsr)
        {
            if (ExistRow(db, insertRow, refYearDayUNV, budLevel, refKvsr))
            {
                UpdateRow(db, insertRow, value, refYearDayUNV, budLevel, refKvsr);
                return;
            }

            string insertQuery =
                @"insert into f_D_FOPlanIncDivide (SourceID, TaskID, RefVariant, RefKD, RefRegions, RefBudLevel, RefYearDayUNV, 
                RefNormDeduct, RefKVSR, RefOrganizations, RefTaxObjects, RefFODepartments, YearPlan) 
                values (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
            DbParameterDescriptor[] queryParams = new DbParameterDescriptor[13];
            queryParams[0] = new DbParameterDescriptor("p0", insertRow["SourceId"]);
            queryParams[1] = new DbParameterDescriptor("p1", -1);
            queryParams[2] = new DbParameterDescriptor("p2", -2);
            queryParams[3] = new DbParameterDescriptor("p3", insertRow["RefKD"]);
            queryParams[4] = new DbParameterDescriptor("p4", insertRow["Region"]);
            queryParams[5] = new DbParameterDescriptor("p5", budLevel);
            queryParams[6] = new DbParameterDescriptor("p6", refYearDayUNV);
            queryParams[7] = new DbParameterDescriptor("p7", 6);
            queryParams[8] = new DbParameterDescriptor("p8", refKvsr);
            queryParams[9] = new DbParameterDescriptor("p9", -1);
            queryParams[10] = new DbParameterDescriptor("p10", -1);
            queryParams[11] = new DbParameterDescriptor("p11", -1);
            queryParams[12] = new DbParameterDescriptor("p12", value);

            db.ExecQuery(insertQuery, QueryResultTypes.NonQuery, queryParams);
        }

        private void UpdateRow(IDatabase db, DataRow updateRow, object value, object refYearDayUNV, int budLevel, long refKvsr)
        {
            var queryParams = new List<DbParameterDescriptor>();
            string updateQuery =
                @"update f_D_FOPlanIncDivide set YearPlan = ? where 
                SourceID = ? and RefKD = ? and RefRegions = ? and RefBudLevel = ? and RefYearDayUNV = ?";

            queryParams.Add(new DbParameterDescriptor("p0", value));
            queryParams.Add(new DbParameterDescriptor("p1", updateRow["SourceId"]));
            queryParams.Add(new DbParameterDescriptor("p2", updateRow["RefKD"]));
            queryParams.Add(new DbParameterDescriptor("p3", updateRow["Region"]));
            queryParams.Add(new DbParameterDescriptor("p4", budLevel));
            queryParams.Add(new DbParameterDescriptor("p5", refYearDayUNV));
            if (refKvsr != -1)
            {
                updateQuery += " and refKvsr = ?";
                queryParams.Add(new DbParameterDescriptor("p6", refKvsr));
            }

            db.ExecQuery(updateQuery, QueryResultTypes.NonQuery, queryParams.ToArray());
        }

        private void DeleteRow(IDatabase db, DataRow deleteRow, object value, object refYearDayUNV, int budLevel, long refKvsr)
        {
            if (ExistRow(db, deleteRow, refYearDayUNV, budLevel, refKvsr))
            {
                UpdateRow(db, deleteRow, value, refYearDayUNV, budLevel, refKvsr);
                return;
            }
            var queryParams = new List<DbParameterDescriptor>();
            string deleteQuery =
                @"delete from f_D_FOPlanIncDivide where 
                SourceID = ? and RefKD = ? and RefRegions = ? and RefBudLevel = ? and RefYearDayUNV = ?";
            queryParams.Add(new DbParameterDescriptor("p0", deleteRow["SourceId"]));
            queryParams.Add(new DbParameterDescriptor("p1", deleteRow["RefKD"]));
            queryParams.Add(new DbParameterDescriptor("p2", deleteRow["Region"]));
            queryParams.Add(new DbParameterDescriptor("p3", budLevel));
            queryParams.Add(new DbParameterDescriptor("p4", refYearDayUNV));
            if (refKvsr != -1)
            {
                deleteQuery += " and refKvsr = ?";
                queryParams.Add(new DbParameterDescriptor("p5", refKvsr));
            }

            db.ExecQuery(deleteQuery, QueryResultTypes.NonQuery, queryParams.ToArray());
        }

        private bool ExistRow(IDatabase db, DataRow row, object refYearDayUNV, int budLevel, long refKvsr)
        {
            var queryParams = new List<DbParameterDescriptor>();
            string query =
                @"select id from f_D_FOPlanIncDivide where 
                SourceID = ? and RefKD = ? and RefRegions = ? and RefBudLevel = ? and RefYearDayUNV = ?";

            queryParams.Add(new DbParameterDescriptor("p0", row["SourceId"]));
            queryParams.Add(new DbParameterDescriptor("p1", row["RefKD"]));
            queryParams.Add(new DbParameterDescriptor("p2", row["Region"]));
            queryParams.Add(new DbParameterDescriptor("p3", budLevel));
            queryParams.Add(new DbParameterDescriptor("p4", refYearDayUNV));
            if (refKvsr != -1)
            {
                query += " and refKvsr = ?";
                queryParams.Add(new DbParameterDescriptor("p5", refKvsr));
            }
            object queryResult = db.ExecQuery(query, QueryResultTypes.Scalar, queryParams.ToArray());
            return queryResult != DBNull.Value && queryResult != null;
        }

        #region копирование данных

        public void CopyMonthData(IncomesYearPlanParams incomesYearPlanParams, int sourceMonth, int destMonth)
        {
            DataRow kdRow = KdData.Tables[0].Select(string.Format("ID = {0}", incomesYearPlanParams.IncomesSource))[0];
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                CopyMonthData(incomesYearPlanParams, sourceMonth, destMonth, kdRow, db);
            }
        }

        private void CopyMonthData(IncomesYearPlanParams incomesYearPlanParams, int sourceMonth, int destMonth, DataRow parentKdRow, IDatabase db)
        {
            string yearDayUNVSource =
                    string.Format("{0}{1}00", incomesYearPlanParams.Year, sourceMonth.ToString().PadLeft(2, '0'));
            string yearDayUNVDest =
                string.Format("{0}{1}00", incomesYearPlanParams.Year, destMonth.ToString().PadLeft(2, '0'));
            db.ExecQuery("delete from f_D_FOPlanIncDivide where RefYearDayUNV = ? and SourceID = ? and RefKD = ?",
                         QueryResultTypes.NonQuery,
                         new DbParameterDescriptor("p0", yearDayUNVDest),
                         new DbParameterDescriptor("p1", incomesYearPlanParams.SourceId),
                         new DbParameterDescriptor("p2", parentKdRow["ID"]));
            string copyQuery =
                @"insert into f_D_FOPlanIncDivide (YearPlan, TaskID, SourceID, RefVariant, RefNormDeduct, RefKD, RefRegions, RefBudLevel, RefKVSR, RefYearDayUNV, RefOrganizations, RefTaxObjects, RefFODepartments)
                select YearPlan, TaskID, SourceID, RefVariant, RefNormDeduct, RefKD, RefRegions, RefBudLevel, RefKVSR, {0} RefYearDayUNV, RefOrganizations, RefTaxObjects, RefFODepartments
                    from f_D_FOPlanIncDivide where SourceID = ? and RefYearDayUNV = ? and RefKD = ?";
            db.ExecQuery(string.Format(copyQuery, yearDayUNVDest), QueryResultTypes.NonQuery,
                         new DbParameterDescriptor("p0", incomesYearPlanParams.SourceId),
                         new DbParameterDescriptor("p1", yearDayUNVSource),
                         new DbParameterDescriptor("p2", parentKdRow["ID"]));

            foreach (DataRow childRow in parentKdRow.GetChildRows("Rel"))
            {
                CopyMonthData(incomesYearPlanParams, sourceMonth, destMonth, childRow, db);
            }
        }

        #endregion

        #region блокирование записей

        public void BlockMonth(IncomesYearPlanParams incomesYearPlanParams, int month, bool isBlock)
        {
            DataRow kdRow = KdData.Tables[0].Select(string.Format("ID = {0}", incomesYearPlanParams.IncomesSource))[0];
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                BlockMonth(incomesYearPlanParams, month, isBlock, kdRow, db);
            }
        }

        private void BlockMonth(IncomesYearPlanParams incomesYearPlanParams, int month, bool isBlock, DataRow parentKdRow, IDatabase db)
        {
            string yearDayUNV =
                    string.Format("{0}{1}00", incomesYearPlanParams.Year, month.ToString().PadLeft(2, '0'));

            string query =
                "update f_D_FOPlanIncDivide set IsBlocked = ? where SourceID = ? and RefYearDayUNV = ? and RefKD = ?";
            db.ExecQuery(query, QueryResultTypes.NonQuery,
                         new DbParameterDescriptor("p0", isBlock),
                         new DbParameterDescriptor("p1", incomesYearPlanParams.SourceId),
                         new DbParameterDescriptor("p2", yearDayUNV),
                         new DbParameterDescriptor("p3", parentKdRow["ID"]));
            foreach (DataRow childRow in parentKdRow.GetChildRows("Rel"))
            {
                BlockMonth(incomesYearPlanParams, month, isBlock, childRow, db);
            }
        }

        #endregion

        #region перенос данных 

        public void TransfertData(long variantId, int variantYear)
        {
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                // удаляем старые данные
                string year = string.Format("{0}0100", variantYear);
                db.ExecQuery(
                    @"delete from f_D_FOPlanIncDivide where YearPlan <> 0 and RefYearDayUNV = ? and RefVariant = -2 and
                    SourceId in (select Id from DataSources where Year = ?)",
                    QueryResultTypes.NonQuery,
                    new DbParameterDescriptor("p0", year),
                    new DbParameterDescriptor("p1", variantYear));
                var transfertData = db.ExecQuery(
                    @"select SourceID, TaskID, Forecast, FromSF, RefNormDeduct, RefKD, RefRegions, RefBudLevel, 
                    RefKVSR, RefYearDayUNV, RefOrganizations, RefTaxObjects, RefFODepartments, IsBlocked
                    from f_D_FOPlanIncDivide where RefVariant = ? and RefYearDayUNV like ? and Forecast is not null
                    and SourceId in (select Id from DataSources where Year = ?)",
                    QueryResultTypes.DataTable,
                    new DbParameterDescriptor("p0", variantId),
                    new DbParameterDescriptor("p1", string.Format("{0}____", variantYear)), 
                    new DbParameterDescriptor("p2", variantYear)) as DataTable;
                if (transfertData == null)
                    return;

                var f_D_FOPlanIncDivideEntity = scheme.RootPackage.FindEntityByName(ObjectKeys.f_D_FOPlanIncDivide);
                using (IDataUpdater du = f_D_FOPlanIncDivideEntity.GetDataUpdater("1 = 2", null))
                {
                    var newData = new DataTable();
                    du.Fill(ref newData);
                    foreach (DataRow dataRow in transfertData.Rows)
                    {
                        var newRow = newData.NewRow();
                        newRow.BeginEdit();
                        newRow["SourceID"] = dataRow["SourceID"];
                        newRow["TaskID"] = dataRow["TaskID"];
                        newRow["FromSF"] = dataRow["FromSF"];
                        newRow["RefNormDeduct"] = dataRow["RefNormDeduct"];
                        newRow["RefKD"] = dataRow["RefKD"];
                        newRow["RefRegions"] = dataRow["RefRegions"];
                        newRow["RefBudLevel"] = dataRow["RefBudLevel"];
                        newRow["RefKVSR"] = dataRow["RefKVSR"];
                        newRow["RefOrganizations"] = dataRow["RefOrganizations"];
                        newRow["RefTaxObjects"] = dataRow["RefTaxObjects"];
                        newRow["RefFODepartments"] = dataRow["RefFODepartments"];
                        newRow["IsBlocked"] = dataRow["IsBlocked"];
                        newRow["RefYearDayUNV"] = year;
                        newRow["YearPlan"] = dataRow["Forecast"];
                        newRow["RefVariant"] = -2;
                        newRow.EndEdit();
                        newData.Rows.Add(newRow);
                    }
                    du.Update(ref newData);
                }
            }
        }

        #endregion
    }
}
