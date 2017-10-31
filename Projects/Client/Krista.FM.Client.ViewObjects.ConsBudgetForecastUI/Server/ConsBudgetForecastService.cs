using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using Krista.FM.Common;
using Krista.FM.ServerLibrary;
using Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.Common;

namespace Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.Server
{
    public class ConsBudgetForecastService
    {
        private IScheme scheme;

        private int variant;

        private int sourceId;

        private Dictionary<int, KDData> kdData;

        private const string code1 = "___1%";
        private const string code2 = "___2%";

        public ConsBudgetForecastService(IScheme scheme)
        {
            this.scheme = scheme;
        }

        public Dictionary<string, string> GetRegionNames(int sourceId)
        {
            DataTable dtRegions = GetRegions(sourceId);
            Dictionary<string , string> result = new Dictionary<string, string>();
            foreach (DataRow row in dtRegions.Select("RefTerr = 7 or RefTerr = 4"))
            {
                result.Add(row[0].ToString(), row[1].ToString());
            }

            result.Add("ResultMR", "Итого по МР");
            result.Add("ResultGO", "Итого по ГО");

            DataRow[] region = dtRegions.Select("RefTerr = 3");
            if (region.Length > 0)
            {
                result.Add(region[0][0].ToString(), region[0][1].ToString());
            }
            result.Add("Result", "ВСЕГО");
            return result;
        }

        public DataTable GetRegions(int sourceId)
        {
            IEntity d_Regions_Plan = scheme.RootPackage.FindEntityByName(ObjectKeys.d_Regions_Plan);
            IEntity b_Regions_BridgePlan = scheme.RootPackage.FindEntityByName(ObjectKeys.b_Regions_BridgePlan);

            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                DataTable dtRegions = (DataTable)db.ExecQuery(
                string.Format(
                    "select reg.id, reg.name, regbrdg.RefTerrType as RefTerr from {0} reg, {1} regbrdg where reg.SourceID = ? and reg.ParentID is null and reg.RefBridgeRegionsPlan = regbrdg.id and (regbrdg.RefTerrType = 3 or regbrdg.RefTerrType = 4 or regbrdg.RefTerrType = 7) order by reg.Code",
                    d_Regions_Plan.FullDBName, b_Regions_BridgePlan.FullDBName), QueryResultTypes.DataTable,
                new DbParameterDescriptor("p0", sourceId));
                return dtRegions;
            }
        }


        /// <summary>
        /// возвращает коды классификатора КД планирование с их индексами по порядку в иерархии
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, int> GetKdCodeIndexes(int sourceId)
        {
            IEntity d_KD_PlanIncomes = scheme.RootPackage.FindEntityByName(ObjectKeys.d_KD_PlanIncomes);
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                GetKDData(db, d_KD_PlanIncomes, sourceId);
                return GetCodeIndex(kdData);
            }
        }

        /// <summary>
        /// формируем датасет с данными из таблицы без расщепления
        /// </summary>
        public DataSet GetNosplitData(int variant, int variantType, int year, int sourceId)
        {
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                DataSet ds = new DataSet();
                int refVariant = -1;
                ds.Tables.Add(GetDataStructure(variant, variantType, year, sourceId, false, false, ref refVariant, db));
                return ds;
            }
        }

        /// <summary>
        /// получаем данные из таблиц без расщепления и расщеплением
        /// </summary>
        public DataSet GetSplittedData(int variant, int variantType, int year, int sourceId, bool splitData, ref int variantForSplit)
        {
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                DataSet ds = new DataSet();
                ds.Tables.Add(GetDataStructure(variant, variantType, year, sourceId, splitData, true, ref variantForSplit, db));
                return ds;
            }
        }

        private DataTable GetDataStructure(int variant, int variantType, int year, int sourceId, bool splitData,
            bool getSplitData, ref int variantForSplit, IDatabase db)
        {
            this.sourceId = sourceId;
            // получаем данные из классификаторов и таблиц фактов
            DataTable dtRegions = new DataTable();
            IEntity d_Regions_Plan = scheme.RootPackage.FindEntityByName(ObjectKeys.d_Regions_Plan);

            dtRegions = GetRegions(sourceId);
            IEntity d_KD_PlanIncomes = scheme.RootPackage.FindEntityByName(ObjectKeys.d_KD_PlanIncomes);
            if (kdData == null || kdData.Count == 0)
                GetKDData(db, d_KD_PlanIncomes, sourceId);

            List<string> dataColumns = new List<string>();
            DataTable resultTable = GetDataStructure(dtRegions, ref dataColumns);
            // данные без расщепления
            IEntity f_D_FOPlanInc = scheme.RootPackage.FindEntityByName(ObjectKeys.f_D_FOPlanInc);
            // НАЛОГИ НА ПРИБЫЛЬ, ДОХОДЫ
            DataTable dtNoSplitData;
            // заполняем данными
            this.variant = variant;
            dtNoSplitData = GetNoSplitFactData(db, f_D_FOPlanInc.FullDBName, d_KD_PlanIncomes.FullDBName,
                d_Regions_Plan.FullDBName, code1, code2, variant, year);
            FillNoSplitDataRow(resultTable, dtNoSplitData, kdData);
            // получаем расщепленные данные
            if (getSplitData)
            {
                IEntity f_D_FOPlanIncDivide = scheme.RootPackage.FindEntityByName(ObjectKeys.f_D_FOPlanIncDivide);
                // если указана опция расщепления, расщепляем данные
                if (splitData)
                    variantForSplit = RunSplitData(variant, variantType, variantForSplit, year);
                this.variant = variantForSplit;
                // получаем данные
                DataTable dtSplitData = GetSplitFactData(db, f_D_FOPlanIncDivide.FullDBName, d_KD_PlanIncomes.FullDBName,
                                               d_Regions_Plan.FullDBName, code1, code2, this.variant, year);
                FillSplitData(resultTable, dtSplitData, kdData);
            }
            // добавляем строки результатов
            FillResults(resultTable, kdData);
            // добавляем все родительские записи
            SetParentRows(resultTable, kdData, dataColumns);
            // проставляем суммы по каждой записи
            SetResultSum(resultTable);
            // ставим id в том порядке как должны отображаться все записи
            FillSortColumn(resultTable, kdData);

            if (getSplitData || splitData)
            {
                FillGlobalResult(resultTable);
            }

            resultTable.AcceptChanges();
            return resultTable;
        }

        /// <summary>
        /// заполнение нерасщепленных данных
        /// </summary>
        private void FillNoSplitDataRow(DataTable dtResult, DataTable dtNoSplitData, Dictionary<int, KDData> kdData)
        {
            foreach (DataColumn column in dtResult.Columns)
            {
                if (column.ColumnName.Contains("_"))
                {
                    string[] regionData = column.ColumnName.Split('_');
                    string refRegion = regionData[0];
                    string budType = regionData[1];
                    if (budType == "4" || budType == "15" || budType == "3")
                    {
                        DataRow[] noSplitRows = dtNoSplitData.Select(string.Format("RefRegions = {0}", refRegion), "RefKD ASC");
                        foreach (DataRow noSplitRow in noSplitRows)
                        {
                            DataRow resultRow = null;
                            if (!noSplitRow.IsNull("Forecast"))
                            {
                                resultRow = GetRow(dtResult, noSplitRow, ValuesIndex.Forecast, kdData);
                                resultRow[column] = Convert.ToDecimal(noSplitRow["Forecast"]);
                            }
                            if (!noSplitRow.IsNull("Restructuring"))
                            {
                                resultRow = GetRow(dtResult, noSplitRow, ValuesIndex.Restructuring, kdData);
                                resultRow[column] = Convert.ToDecimal(noSplitRow["Restructuring"]);
                            }
                            if (!noSplitRow.IsNull("Arrears"))
                            {
                                resultRow = GetRow(dtResult, noSplitRow, ValuesIndex.Arrears, kdData);
                                resultRow[column] = Convert.ToDecimal(noSplitRow["Arrears"]);
                            }
                            if (!noSplitRow.IsNull("Priorcharge"))
                            {
                                resultRow = GetRow(dtResult, noSplitRow, ValuesIndex.Priorcharge, kdData);
                                resultRow[column] = Convert.ToDecimal(noSplitRow["Priorcharge"]);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// заполнение расщепленных данных
        /// </summary>
        private void FillSplitData(DataTable dtResult, DataTable dtsplitData, Dictionary<int, KDData> kdData)
        {
            foreach (DataColumn column in dtResult.Columns)
            {
                if (column.ColumnName.Contains("_"))
                {
                    string[] regionData = column.ColumnName.Split('_');
                    int refRegion = -1;
                    if (!Int32.TryParse(regionData[0], out refRegion))
                        continue;
                    string budType = regionData[1];
                    DataRow[] splitRows = dtsplitData.Select(string.Format("RefRegions = {0}", refRegion));
                    foreach (DataRow splitRow in splitRows)
                    {
                        if (splitRow.IsNull("Forecast"))
                            continue;
                        string rowBudLevel = splitRow["RefBudLevel"].ToString();
                        if (budType == rowBudLevel)
                        {
                            string refNormDeduct = splitRow["RefNormDeduct"].ToString();
                            decimal nomativeVal = splitRow.IsNull("NormVal")
                                                      ? 0
                                                      : Convert.ToDecimal(splitRow["NormVal"]);
                            DataRow resultRow = null;
                            DataRow normativeRow = null;
                            switch (refNormDeduct)
                            {
                                case "1":
                                    if (nomativeVal != 0)
                                    {
                                        normativeRow = GetRow(dtResult, splitRow, ValuesIndex.NormBK, kdData);
                                        normativeRow[column] = nomativeVal * 100;
                                    }
                                    resultRow = GetRow(dtResult, splitRow, ValuesIndex.BK, kdData);
                                    resultRow[column] = Convert.ToDecimal(splitRow["Forecast"]);
                                    break;
                                case "2":
                                    if (nomativeVal != 0)
                                    {
                                        normativeRow = GetRow(dtResult, splitRow, ValuesIndex.NormRF, kdData);
                                        normativeRow[column] = nomativeVal * 100;
                                    }
                                    resultRow = GetRow(dtResult, splitRow, ValuesIndex.RF, kdData);
                                    resultRow[column] = Convert.ToDecimal(splitRow["Forecast"]);
                                    break;
                                case "3":
                                    if (nomativeVal != 0)
                                    {
                                        normativeRow = GetRow(dtResult, splitRow, ValuesIndex.NormMR, kdData);
                                        normativeRow[column] = nomativeVal * 100;
                                    }
                                    resultRow = GetRow(dtResult, splitRow, ValuesIndex.MR, kdData);
                                    resultRow[column] = Convert.ToDecimal(splitRow["Forecast"]);
                                    break;
                                case "4":
                                    if (nomativeVal != 0)
                                    {
                                        normativeRow = GetRow(dtResult, splitRow, ValuesIndex.NormDifRF, kdData);
                                        normativeRow[column] = nomativeVal * 100;
                                    }
                                    resultRow = GetRow(dtResult, splitRow, ValuesIndex.DifRF, kdData);
                                    resultRow[column] = Convert.ToDecimal(splitRow["Forecast"]);
                                    break;
                                case "5":
                                    if (nomativeVal != 0)
                                    {
                                        normativeRow = GetRow(dtResult, splitRow, ValuesIndex.NormDifMR, kdData);
                                        normativeRow[column] = nomativeVal * 100;
                                    }
                                    resultRow = GetRow(dtResult, splitRow, ValuesIndex.DifMR, kdData);
                                    resultRow[column] = Convert.ToDecimal(splitRow["Forecast"]);
                                    break;
                                case "6":
                                    resultRow = GetRow(dtResult, splitRow, ValuesIndex.AllNormatives, kdData);
                                    resultRow[column] = Convert.ToDecimal(splitRow["Forecast"]);
                                    break;
                            }
                            // итог по консолидированному бюджету
                            if (resultRow != null && (budType == "6" || budType == "5"))
                            {
                                decimal sum = 0;
                                string column1 = string.Format("{0}_5", regionData[0]);
                                string column2 = string.Format("{0}_6", regionData[0]);
                                sum += resultRow.IsNull(column1) ? 0 : Convert.ToDecimal(resultRow[column1]);
                                sum += resultRow.IsNull(column2) ? 0 : Convert.ToDecimal(resultRow[column2]);
                                resultRow[string.Format("{0}_4", regionData[0])] = sum;
                            }
                            // итог по нормативам для консолидированного бюджета
                            if (normativeRow != null && (budType == "6" || budType == "5"))
                            {
                                decimal sum = 0;
                                string column1 = string.Format("{0}_5", regionData[0]);
                                string column2 = string.Format("{0}_6", regionData[0]);
                                if (!normativeRow.IsNull(column1) && !normativeRow.IsNull(column2))
                                {
                                    sum += Convert.ToDecimal(normativeRow[column1]);
                                    sum += Convert.ToDecimal(normativeRow[column2]);
                                    normativeRow[string.Format("{0}_4", regionData[0])] = sum;
                                }
                            }
                        }
                        // добавим строку - результат в том случае, если у нас есть расщепленные данные 
                        GetRow(dtResult, splitRow, ValuesIndex.Result, kdData);
                    }
                    // просуммируем отдельно данные по субъекту
                    if (budType == "3")
                    {
                        DataRow[] subjectRows = dtsplitData.Select(string.Format("RefBudLevel = {0} and RefRegions <> {1}", budType, refRegion));
                        foreach (DataRow subjectRow in subjectRows)
                        {
                            string refNormDeduct = subjectRow["RefNormDeduct"].ToString();
                            ValuesIndex valuesIndex = GetValueIndex(refNormDeduct);
                            DataRow resultRow = GetRow(dtResult, subjectRow, valuesIndex, kdData);
                            if (resultRow.IsNull(column))
                                resultRow[column] = subjectRow["Forecast"];
                            else
                            {
                                decimal resultValue = Convert.ToDecimal(resultRow[column]);
                                decimal rowValue = 0;
                                if (Decimal.TryParse(subjectRow["Forecast"].ToString(), out rowValue))
                                {
                                    resultRow[column] = resultValue + rowValue;
                                }
                            }
                        }
                    }
                }
            }
        }

        private ValuesIndex GetValueIndex(string index)
        {
            switch (index)
            {
                case "1":
                    return ValuesIndex.BK;
                case "2":
                    return ValuesIndex.RF;
                case "3":
                    return ValuesIndex.MR;
                case "4":
                    return ValuesIndex.DifRF;
                case "5":
                    return ValuesIndex.DifMR;
            }
            return ValuesIndex.Result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dtResult"></param>
        /// <param name="dataRow"></param>
        /// <param name="index"></param>
        /// <param name="kdData"></param>
        /// <returns></returns>
        private DataRow GetRow(DataTable dtResult, DataRow dataRow, ValuesIndex index, Dictionary<int, KDData> kdData)
        {
            DataRow[] rows = dtResult.Select(string.Format("RefKD = {0} and Index = {1}", dataRow["RefKD"], (int)index));
            if (rows.Length == 0)
            {
                DataRow newRow = dtResult.NewRow();
                newRow["ID"] = dtResult.Rows.Count;
                newRow["RefVariant"] = this.variant;
                newRow["SourceID"] = sourceId;
                newRow["RefKD"] = dataRow["RefKD"];
                newRow["Index"] = (int)index;
                newRow["IndexStr"] = GetIndexStr(index);
                newRow["Code"] = dataRow["Code"];
                newRow["Name"] = dataRow["Name"];
                newRow["kdParentId"] = kdData[Convert.ToInt32(newRow["RefKD"])].RefID == null ? (object)DBNull.Value : kdData[Convert.ToInt32(newRow["RefKD"])].RefID;
                newRow["IsResultRow"] = index == ValuesIndex.Result;
                newRow["ParentRow"] = false;
                dtResult.Rows.Add(newRow);
                return newRow;
            }
            return rows[0];
        }

        public int GetSourceId(int year)
        {
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                object result =
                    db.ExecQuery(
                        "select id from DataSources where SupplierCode = 'ФО' and DataCode = 29 and DataName = 'Проект бюджета' and Year = ?",
                        QueryResultTypes.Scalar, new DbParameterDescriptor("p0", year));
                if (result == DBNull.Value || result == null)
                    return -1;
                return Convert.ToInt32(result);
            }
        }

        /// <summary>
        /// расщепляем данные
        /// </summary>
        /// <param name="variantId"></param>
        /// <param name="variantType"></param>
        /// <returns></returns>
        private int RunSplitData(int variantId, int variantType, int variantForSplit, int year)
        {
            if (variantForSplit == -1)
                scheme.DisintRules.SplitData(variantId, variantType, true, ref variantForSplit, year);
            else
                scheme.DisintRules.SplitData(variantId, variantType, true, variantForSplit, year);
            return variantForSplit;
        }

        private void SetResultSum(DataTable dtResultData)
        {
            decimal mrSum = 0;
            decimal goSum = 0;
            decimal result = 0;
            decimal rSum = 0;
            decimal sSum = 0;

            foreach (DataRow row in dtResultData.Rows)
            {
                ValuesIndex index = (ValuesIndex)row["Index"];
                // для нормативов не выводим родительские записи
                if (index == ValuesIndex.NormBK || index == ValuesIndex.NormDifMR || index == ValuesIndex.NormDifRF || index == ValuesIndex.NormMR || index == ValuesIndex.NormRF)
                    continue;

                foreach (DataColumn column in dtResultData.Columns)
                {
                    // консолидированный бюджет муниципального района
                    if (column.ColumnName.Contains("_4"))
                    {
                        if (!row.IsNull(column))
                            mrSum += Convert.ToDecimal(row[column]);
                    }
                    // бюджет городского округа
                    if (column.ColumnName.Contains("_15"))
                    {
                        if (!row.IsNull(column))
                            goSum += Convert.ToDecimal(row[column]);
                    }
                    // бюджет субъекта
                    if (column.ColumnName.Contains("_3"))
                    {
                        if (!row.IsNull(column))
                            result += Convert.ToDecimal(row[column]);
                    }
                    // суммы района
                    if (column.ColumnName.Contains("_5"))
                    {
                        if (!row.IsNull(column))
                            rSum += Convert.ToDecimal(row[column]);
                    }
                    // суммы по поселениям
                    if (column.ColumnName.Contains("_6"))
                    {
                        if (!row.IsNull(column))
                            sSum += Convert.ToDecimal(row[column]);
                    }
                }

                row["ResultMR_ResultMR"] = rSum == 0 ? DBNull.Value : (object)rSum;
                row["ResultMR_ResultMRSettlement"] = sSum == 0 ? DBNull.Value : (object)sSum;
                row["ResultMR_ResultMRKB"] = mrSum == 0 ? DBNull.Value : (object)mrSum;
                row["ResultGO_ResultGO"] = goSum == 0 ? DBNull.Value : (object)goSum;
                if (result != 0 || goSum != 0 || mrSum != 0)
                {
                    result = result + goSum + mrSum;
                    row["Result_Result"] = result;
                }
                else
                    row["Result_Result"] = DBNull.Value;
                mrSum = 0;
                goSum = 0;
                result = 0;
                rSum = 0;
                sSum = 0;
            }
        }

        private DataRow GetResultRow(DataTable dtresultTable, string name)
        {
            DataRow resultRow = dtresultTable.NewRow();
            resultRow["name"] = name;
            foreach (DataRow row in dtresultTable.Rows)
            {
                foreach (DataColumn column in dtresultTable.Columns)
                {
                    if (column.DataType == typeof(Decimal))
                    {
                        if (!row.IsNull(column))
                            resultRow[column] = resultRow.IsNull(column)
                                                    ? row[column]
                                                    : Convert.ToDecimal(resultRow[column]) + Convert.ToDecimal(row[column]);
                    }
                }
            }
            return resultRow;
        }

        /// <summary>
        /// создает структуру для хранения и отображения данных
        /// </summary>
        /// <param name="dtRegions"></param>
        /// <returns></returns>
        private  DataTable GetDataStructure(DataTable dtRegions, ref List<string> dataColumns)
        {
            // строим структуру, которую будем отображать в таблице на клиенте
            DataTable resultTable = new DataTable();
            DataColumn column = resultTable.Columns.Add("ID", typeof(int));
            column.Caption = string.Empty;
            column = resultTable.Columns.Add("RefVariant", typeof(int));
            column.Caption = string.Empty;
            column = resultTable.Columns.Add("SourceID", typeof(int));
            column.Caption = string.Empty;
            resultTable.Columns.Add("RefKD", typeof(int));
            column = resultTable.Columns.Add("Code", typeof(string));
            column.Caption = string.Empty;
            column = resultTable.Columns.Add("Name", typeof(string));
            column.Caption = string.Empty;
            column = resultTable.Columns.Add("kdParentId", typeof(int));
            column.Caption = string.Empty;
            column = resultTable.Columns.Add("Index", typeof(int));
            column.Caption = string.Empty;
            column = resultTable.Columns.Add("IndexStr", typeof(string));
            column.Caption = string.Empty;
            column = resultTable.Columns.Add("IsResultRow", typeof(bool));
            column.Caption = string.Empty;
            column = resultTable.Columns.Add("SortColumn", typeof(string));
            column.Caption = string.Empty;
            column = resultTable.Columns.Add("ParentRow", typeof(bool));
            column.Caption = string.Empty;

            foreach (DataRow row in dtRegions.Rows)
            {
                int territory = Convert.ToInt32(row["RefTerr"]);
                switch (territory)
                {
                    case 4:
                        column = resultTable.Columns.Add(string.Format("{0}_{1}", row["ID"], 4), typeof(decimal));
                        column.Caption = "КБМР";
                        dataColumns.Add(column.ColumnName);
                        column = resultTable.Columns.Add(string.Format("{0}_{1}", row["ID"], 5), typeof(decimal));
                        column.Caption = "МР";
                        dataColumns.Add(column.ColumnName);
                        column = resultTable.Columns.Add(string.Format("{0}_{1}", row["ID"], 6), typeof(decimal));
                        column.Caption = "Всего по поселениям";
                        dataColumns.Add(column.ColumnName);
                        break;
                    case 7:
                        column = resultTable.Columns.Add(string.Format("{0}_{1}", row["ID"], 15), typeof(decimal));
                        column.Caption = "Бюджет ГО";
                        dataColumns.Add(column.ColumnName);
                        break;
                }
            }
            column = resultTable.Columns.Add("ResultMR_ResultMRKB", typeof(decimal));
            column.Caption = "КБМР";
            dataColumns.Add(column.ColumnName);
            column = resultTable.Columns.Add("ResultMR_ResultMR", typeof(decimal));
            column.Caption = "МР";
            dataColumns.Add(column.ColumnName);
            column = resultTable.Columns.Add("ResultMR_ResultMRSettlement", typeof(decimal));
            column.Caption = "Всего по поселениям"; 
            dataColumns.Add(column.ColumnName);
            column = resultTable.Columns.Add("ResultGO_ResultGO", typeof(decimal));
            column.Caption = "Итого по ГО";
            dataColumns.Add(column.ColumnName);

            DataRow[] region = dtRegions.Select("RefTerr = 3");
            if (region.Length > 0)
            {
                column = resultTable.Columns.Add(string.Format("{0}_{1}", region[0]["ID"], 3), typeof(decimal));
                column.Caption = "Бюджет субъекта";
                dataColumns.Add(column.ColumnName);
            }

            column = resultTable.Columns.Add("Result_Result", typeof(decimal));
            column.Caption = string.Empty;
            dataColumns.Add(column.ColumnName);
            return resultTable;
        }

        /// <summary>
        /// получение данных из таблицы фактов
        /// </summary>
        /// <returns></returns>
        private DataTable GetNoSplitFactData(IDatabase db, string tableName, string kdTableName, string regionTableName, string kdCode1, string kdCode2, int variant, int year)
        {
            string query = string.Format(
                @"select data.Forecast, data.Restructuring, data.Arrears, data.Priorcharge, 0 Result,
                data.RefRegions, data.RefKD, kd.CodeStr as Code, kd.Name from {0} data, {1} kd
                where data.RefVariant = ? and data.SourceID = ? and data.RefYearDayUNV like ? and data.RefKD = kd.ID and
                data.RefRegions in (select id from {2} where SourceID = ? and ParentID is null) and 
                data.RefKD in (select id from {1} where SourceID = ? and (CodeStr like ? or CodeStr like ?))",
                 tableName, kdTableName, regionTableName);

            DataTable dt = new DataTable();

            dt = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable,
                         new DbParameterDescriptor("p0", variant),
                         new DbParameterDescriptor("p1", sourceId),
                         new DbParameterDescriptor("p2", string.Format("{0}____", year)),
                         new DbParameterDescriptor("p3", sourceId), 
                         new DbParameterDescriptor("p4", sourceId),
                         new DbParameterDescriptor("p5", kdCode1),
                         new DbParameterDescriptor("p6", kdCode2));

            foreach (DataRow row in dt.Rows)
            {
                if (!row.IsNull("Forecast"))
                    row["Forecast"] = Convert.ToDecimal(row["Forecast"]) / 1000;
                if (!row.IsNull("Restructuring"))
                    row["Restructuring"] = Convert.ToDecimal(row["Restructuring"]) / 1000;
                if (!row.IsNull("Arrears"))
                    row["Arrears"] = Convert.ToDecimal(row["Arrears"]) / 1000;
                if (!row.IsNull("Priorcharge"))
                    row["Priorcharge"] = Convert.ToDecimal(row["Priorcharge"]) / 1000;
                row.AcceptChanges();
            }
            return dt;
        }

        /// <summary>
        /// получение данных из таблицы фактов
        /// </summary>
        /// <returns></returns>
        private DataTable GetSplitFactData(IDatabase db, string tableName, string kdTableName, string regionTableName, string kdCode1, string kdCode2, int variant, int year)
        {
            string query = string.Format(
                @"select data.Forecast, data.RefKD, data.RefBudLevel, data.RefNormDeduct, 0 Result,
                data.RefRegions, kd.CodeStr as Code, kd.Name, data.NormDeductVal as NormVal from {0} data, {1} kd
                where data.RefVariant = ? and data.SourceID = ? and data.RefYearDayUNV like ? and data.RefKD = kd.ID and
                data.RefRegions in (select id from {2} where SourceID = ? and ParentID is null) and 
                data.RefKD in (select id from {1} where SourceID = ? and (CodeStr like ? or CodeStr like ?))",
                 tableName, kdTableName, regionTableName);

            DataTable dt = new DataTable();

            dt = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable,
                         new DbParameterDescriptor("p0", variant),
                         new DbParameterDescriptor("p1", sourceId),
                         new DbParameterDescriptor("p2", string.Format("{0}____", year)),
                         new DbParameterDescriptor("p3", sourceId),
                         new DbParameterDescriptor("p4", sourceId),
                         new DbParameterDescriptor("p5", kdCode1),
                         new DbParameterDescriptor("p6", kdCode2));
            foreach (DataRow row in dt.Rows)
            {
                if (!row.IsNull("Forecast"))
                {
                    row["Forecast"] = Convert.ToDecimal(row["Forecast"])/1000;
                    row.AcceptChanges();
                }
            }
            return dt;
        }

        #region заполнение таблицы данными

        private string GetIndexStr(ValuesIndex index)
        {
            switch (index)
            {
                case ValuesIndex.Forecast:
                    return "Прогноз";
                case ValuesIndex.Restructuring:
                    return "Реструктуризация";
                case ValuesIndex.Arrears:
                    return "Недоимка";
                case ValuesIndex.Priorcharge:
                    return "Доначисленные";
                case ValuesIndex.NormBK:
                    return "Норматив по БК";
                case ValuesIndex.BK:
                    return "Отчисления по БК";
                case ValuesIndex.NormRF:
                    return "Норматив по СБ";
                case ValuesIndex.RF:
                    return "Отчисления по СБ";
                case  ValuesIndex.NormMR:
                    return "Норматив по МР";
                case ValuesIndex.MR:
                    return "Отчисления по МР";
                case ValuesIndex.NormDifRF:
                    return "Норматив по Диф.СБ";
                case ValuesIndex.DifRF:
                    return "Отчисления по Диф.СБ";
                case ValuesIndex.NormDifMR:
                    return "Норматив по Диф.МР";
                case ValuesIndex.DifMR:
                    return "Отчисления по Диф.МР";
                case ValuesIndex.AllNormatives:
                    return "Сумма";
                case ValuesIndex.Result:
                    return "Всего отчисления";
            }
            return string.Empty;
        }

        private void SetParentRows(DataTable dtResults, Dictionary<int, KDData> kdData, List<string> dataColumns)
        {
            Dictionary<string, DataRow> newRows = new Dictionary<string, DataRow>();
            foreach (DataRow row in dtResults.Rows)
            {
                if (row.IsNull("kdParentId"))
                    continue;

                ValuesIndex index = (ValuesIndex) row["Index"];
                // для нормативов не выводим родительские записи
                if (index == ValuesIndex.NormBK || index == ValuesIndex.NormDifMR || index == ValuesIndex.NormDifRF || index == ValuesIndex.NormMR || index == ValuesIndex.NormRF)
                    continue;

                AddParentRows(dtResults, kdData, dataColumns, row, row, newRows);
            }
            foreach (KeyValuePair<string, DataRow> kvp in newRows)
            {
                dtResults.Rows.Add(kvp.Value);
            }
        }

        /// <summary>
        /// добавляем родительские записи и суммируем для них значения
        /// </summary>
        private void AddParentRows(DataTable dtResults, Dictionary<int, KDData> kdData, List<string> dataColumns,
            DataRow dataRow, DataRow resultRow, Dictionary<string, DataRow> newRows)
        {
            if (dataRow.IsNull("kdParentId"))
                return;
            // ссылка на родительскую запись кд
            int parentId = Convert.ToInt32(dataRow["kdParentId"]);
            string key = string.Format("{0}_{1}", kdData[parentId].ID, dataRow["Index"]);
            DataRow newRow = null;
            DataRow[] parentRows =
                dtResults.Select(string.Format("Code = '{0}' and Index = {1}", kdData[parentId].Code, dataRow["Index"]));
            if (parentRows.Length != 0)
            {
                newRow = parentRows[0];
                newRow["IsResultRow"] = true;
            }
            if (newRow == null && newRows.ContainsKey(key))
            {
                newRow = newRows[key];
            }
            else if (newRow == null)
            {
                newRow = dtResults.NewRow();
                newRow["RefKD"] = kdData[parentId].ID;
                newRow["kdParentId"] = kdData[parentId].RefID == null ? DBNull.Value : (object)kdData[parentId].RefID;
                newRow["Name"] = kdData[parentId].Name;
                newRow["Code"] = kdData[parentId].Code;
                newRow["Index"] = dataRow["Index"];
                newRow["IndexStr"] = dataRow["IndexStr"];
                newRow["IsResultRow"] = true;
                newRow["ParentRow"] = true;
                newRows.Add(key, newRow);
            }

            foreach (string columName in dataColumns)
            {
                if (newRow.IsNull(columName))
                {
                    newRow[columName] = resultRow[columName];
                }
                else
                {
                    newRow[columName] = resultRow.IsNull(columName)
                                            ? newRow[columName]
                                            : Convert.ToDecimal(resultRow[columName]) + Convert.ToDecimal(newRow[columName]);
                }
            }

            if (kdData[parentId].RefID != null)
                AddParentRows(dtResults, kdData, dataColumns, newRow, resultRow, newRows);
        }

        /// <summary>
        /// добавление и заполнение результатов по кодам дохода
        /// </summary>
        /// <param name="dtResults"></param>
        /// <param name="kdData"></param>
        private void FillResults(DataTable dtResults, Dictionary<int, KDData> kdData)
        {
            foreach (int id in kdData.Keys)
            {
                DataRow[] dataRows = dtResults.Select(string.Format("RefKD = {0}", id));
                FillResult(dataRows);
            }
        }

        private void FillGlobalResult(DataTable dtResults)
        {
            DataRow[] childRows = dtResults.Select(string.Format("ParentRow = false and Index = {0}", (int)ValuesIndex.Result));
            if (childRows.Length == 0)
                return;
            DataRow resultRow = dtResults.NewRow();
            resultRow.BeginEdit();
            resultRow["RefKD"] = -1;
            resultRow["Name"] = "ИТОГО ДОХОДОВ";
            resultRow["Index"] = ValuesIndex.Result;
            resultRow["ParentRow"] = true;
            resultRow["IsResultRow"] = true;
            resultRow["ID"] = 100000;
            foreach (DataColumn column in dtResults.Columns)
            {
                if (!column.ColumnName.Contains("_"))
                    continue;

                foreach (DataRow row in childRows)
                {
                    if (!row.IsNull(column))
                    {
                        if (resultRow.IsNull(column))
                            resultRow[column] = row[column];
                        else
                            resultRow[column] = Convert.ToDecimal(resultRow[column]) +
                                                Convert.ToDecimal(row[column]);
                    }
                }
            }
            resultRow.EndEdit();
            dtResults.Rows.Add(resultRow);
        }

        /// <summary>
        /// заполняем результаты по коду дохода
        /// </summary>
        /// <param name="rows"></param>
        private void FillResult(DataRow[] rows)
        {
            DataRow resultRow = null;
            foreach (DataRow row in rows)
            {
                if ((ValuesIndex)row["Index"] == ValuesIndex.Result)
                {
                    resultRow = row;
                    break;
                }
            }
            if (resultRow == null)
                return;
            foreach (DataRow row in rows)
            {
                // считаем итоги только для расщепленных данных
                ValuesIndex index = (ValuesIndex) row["Index"];
                if (index == ValuesIndex.Result || index == ValuesIndex.Forecast ||
                    index == ValuesIndex.Priorcharge || index == ValuesIndex.Restructuring || index == ValuesIndex.Arrears)
                    continue;
                // для нормативов не выводим родительские записи
                if (index == ValuesIndex.NormBK || index == ValuesIndex.NormDifMR || index == ValuesIndex.NormDifRF || index == ValuesIndex.NormMR || index == ValuesIndex.NormRF)
                    continue;
                foreach (DataColumn column in row.Table.Columns)
                {
                    if (column.DataType == typeof(Decimal))
                    {
                        if (!row.IsNull(column))
                        {
                            if (resultRow.IsNull(column))
                                resultRow[column] = row[column];
                            else
                                resultRow[column] = Convert.ToDecimal(resultRow[column]) +
                                                    Convert.ToDecimal(row[column]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// получение кодов дохода в таком порядке, в каком они должны отображаться 
        /// </summary>
        /// <param name="kd"></param>
        /// <returns></returns>
        private Dictionary<int, KDData> GetKdData(DataTable kd)
        {
            kd.BeginLoadData();
            Dictionary<int, KDData> kdData = new Dictionary<int, KDData>();
            foreach(DataRow row in kd.Select("ParentID is null", "CodeStr ASC"))
            {
                kdData.Add(Convert.ToInt32(row["ID"]), new KDData(row));
                GetKdData(kd, row, ref kdData);
            }
            kd.EndLoadData();
            return kdData;
        }

        private void GetKdData(DataTable kd, DataRow parentRow, ref Dictionary<int, KDData> kdData)
        {
            foreach (DataRow row in kd.Select(string.Format("ParentID = {0}", parentRow["ID"]), "CodeStr ASC"))
            {
                kdData.Add(Convert.ToInt32(row["ID"]), new KDData(row));
                GetKdData(kd, row, ref kdData);
            }
        }

        #endregion

        private void AddIntermediateResults(DataTable dtKd)
        {
            if (dtKd.Rows.Count == 0)
                return;

            dtKd.BeginLoadData();
            DataRow parentRow = dtKd.Select(string.Empty, "CodeStr ASC")[0];

            foreach (DataRow row in dtKd.Select(string.Format("ParentID = {0}", parentRow["ID"])))
            {
                string codeSub = row["CodeStr"].ToString().Substring(3, 3);
                if (codeSub == "101" || codeSub == "102" || codeSub == "103" || codeSub == "104" || codeSub == "105" ||
                    codeSub == "106" || codeSub == "107" || codeSub == "108" || codeSub == "109")
                    row["ParentID"] = -1;
                else
                    row["ParentID"] = -2;
            }

            // добавляем итоги по налоговым и неналоговым доходам
            DataRow newRow = dtKd.NewRow();
            newRow["CodeStr"] = string.Empty;
            newRow["Name"] = "Всего налоговых доходов";
            newRow["ParentID"] = parentRow["ID"];
            newRow["ID"] = -1;
            dtKd.Rows.Add(newRow);
            newRow = dtKd.NewRow();
            newRow["CodeStr"] = string.Empty;
            newRow["Name"] = "Всего неналоговых доходов";
            newRow["ParentID"] = parentRow["ID"];
            newRow["ID"] = -2;
            dtKd.Rows.Add(newRow);
            dtKd.EndLoadData();
        }

        private void FillSortColumn(DataTable dtResult, Dictionary<int, KDData> kdData)
        {
            int id = 0;
            foreach (KeyValuePair<int, KDData> kvp in kdData)
            {
                DataRow[] rows = dtResult.Select(string.Format("RefKD = {0}", kvp.Key), "Index ASC");
                foreach (DataRow row in rows)
                {
                    row["ID"] = id + Convert.ToInt32(row["Index"]);
                }
                id += (int)ValuesIndex.Result + 1;
            }
        }

        private Dictionary<string, int> GetCodeIndex(Dictionary<int, KDData> kdData)
        {
            int i = 0;
            Dictionary<string, int> codeIndexes = new Dictionary<string, int>();
            foreach (KeyValuePair<int, KDData> kvp in kdData)
            {
                string code = string.IsNullOrEmpty(kvp.Value.Code) ? kvp.Value.ID.ToString() : kvp.Value.Code;
                if (!codeIndexes.ContainsKey(code))
                    codeIndexes.Add(code, i);
                i += (int)ValuesIndex.Result + 1;
            }
            return codeIndexes;
        }

        private void GetKDData(IDatabase db, IEntity kdEntity, int sourceId)
        {
            DataTable dtKd = (DataTable)
                db.ExecQuery(
                    string.Format(
                        "select Distinct(CodeStr), id, Name, ParentID from {0} where SourceID = ? and (CodeStr like ? or CodeStr like ?) order by CodeStr",
                        kdEntity.FullDBName),
                    QueryResultTypes.DataTable,
                    new DbParameterDescriptor("p0", sourceId),
                    new DbParameterDescriptor("p1", code1),
                    new DbParameterDescriptor("p2", code2));

            AddIntermediateResults(dtKd);
            if (this.sourceId != sourceId)
                kdData = GetKdData(dtKd);
        }
    }

    /// <summary>
    /// класс, описывает данные классификатора KD Анализ
    /// </summary>
    internal class KDData
    {
        internal KDData(DataRow kdRow)
        {
            ID = Convert.ToInt32(kdRow["ID"]);
            Code = kdRow["CodeStr"].ToString();
            Name = kdRow["Name"].ToString();
            if (kdRow.IsNull("ParentID"))
                RefID = null;
            else
                RefID = Convert.ToInt32(kdRow["ParentID"]);
        }

        private int id;
        internal int ID
        {
            get { return id; }
            private set { id = value;}
        }

        private string code;
        internal string Code
        {
            get { return code;}
            private set { code = value;}
        }

        private string name;
        internal  string Name
        {
            get { return name;}
            private set { name = value;}
        }

        private int? refID;
        internal int? RefID
        {
            get { return refID;}
            private set { refID = value;}
        }
    }
}
