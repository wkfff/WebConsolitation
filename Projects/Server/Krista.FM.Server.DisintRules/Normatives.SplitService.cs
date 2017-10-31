using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DisintRules
{
    /// <summary>
    /// класс расщепления данных по нормативам
    /// </summary>
    public class NormativesSplitService
    {
        private IScheme scheme;

        //private NormativesService normativeService;

        private Dictionary<int, NormativesService> normatives;

        public NormativesSplitService(IScheme scheme)
        {
            this.scheme = scheme;
            normatives = new Dictionary<int, NormativesService>();
            //normativeService = new NormativesService(scheme);
        }

        #region расщепление данных
        /// <summary>
        /// Расщепление данных. Выполняется построчно
        /// </summary>
        /// <param name="rowForSplit"></param>
        /// <param name="dtSplittedData"></param>
        /// <param name="year"></param>
        /// <param name="variantType"></param>
        /// <param name="db"></param>
        /// <param name="protocol"></param>
        internal bool SplitData(DataRow rowForSplit, DataTable dtSplittedData,
            int year, VariantType variantType, IDatabase db, IClassifiersProtocol protocol, int variant)
        {
            if (!normatives.ContainsKey(year))
                normatives.Add(year, new NormativesService(scheme, year));
            // получаем тип территории
            int refRegions = Convert.ToInt32(rowForSplit["RefRegions"]);
            TerritoryType territoryType = NormativesUtils.GetTerritoryType(refRegions, db);
            if (territoryType == TerritoryType.Unknown)
            {
                NormativesUtils.SendMessageToProtocol(protocol,
                    string.Format("Таблица фактов 'Доходы. Результаты доходов без ращепления'. Источник данных = {0}. Запись с ID = {1}. В классификаторе «Районы.Сопоставимый план» не указан тип территории",
                    rowForSplit["SourceID"], rowForSplit["ID"]), ClassifiersEventKind.ceWarning, variant);
                return false;
            }
            // ищем норматив
            int refKd = Convert.ToInt32(rowForSplit["RefKD"]);
            DataRow normative = normatives[year].FindNormative(refKd, year, refRegions, db);
            string subCode = rowForSplit["Code"].ToString().Substring(3, 1);
            if (normative != null)
            {
                // если норматив нашли, производим ращепление
                SplitRow(rowForSplit, dtSplittedData, normative, variantType, territoryType);
            }
            else if(subCode == "2" || subCode == "3")
            {
                SplitRow(rowForSplit, dtSplittedData, null, VariantType.BudgetLevels, territoryType);
            }
            else
            {
                NormativesUtils.SendMessageToProtocol(protocol,
                        string.Format(normativeNotFound, rowForSplit["Name"], GetDataSourceName(Convert.ToInt32(rowForSplit["SourceID"])), year, "норматив расщепления не найденн"),
                        ClassifiersEventKind.ceWarning, variant);
                return false;
            }
            return dtSplittedData.Rows.Count > 0;
        }

        private const string normativeNotFound =
            "Таблица фактов «Доходы.ФО_Результат доходов без расщепления». По району «{0}» для кода «{1}» классификатора данных КД.Планирование по источнику «{2}» {3} года {4}";

        /// <summary>
        /// Расщепление данных с учетом видов нормативов. Выполняется построчно
        /// </summary>
        internal bool SplitData(DataRow rowForSplit, DataTable dtSplittedData,
            bool useNornativeType, int year, VariantType variantType, IDatabase db, IClassifiersProtocol protocol, int variant)
        {
            if (!useNornativeType)
            {
                return SplitData(rowForSplit, dtSplittedData, year, variantType, db, protocol, variant);
            }
            if (!normatives.ContainsKey(year))
                normatives.Add(year, new NormativesService(scheme, year));
            // тип региона по которому ищем нормативы)
            int territory = Convert.ToInt32(scheme.GlobalConstsManager.Consts["TerrPartType"].Value);

            int refRegions = Convert.ToInt32(rowForSplit["RefRegions"]);
            TerritoryType territoryType = NormativesUtils.GetTerritoryType(refRegions, db);
            if (territoryType == TerritoryType.Unknown)
            {
                NormativesUtils.SendMessageToProtocol(protocol,
                    string.Format("Таблица фактов 'Доходы. Результаты доходов без ращепления'. Источник данных = {0}. Запись с ID = {1}. В классификаторе «Районы.Сопоставимый план» не указан тип территории",
                    rowForSplit["SourceID"], rowForSplit["ID"]), ClassifiersEventKind.ceWarning, variant);
                return false;
            }
            int refKd = Convert.ToInt32(rowForSplit["RefKD"]);
            string subCode = rowForSplit["Code"].ToString().Substring(3, 1);
            // в зависимости от района пытаемся по разному расщепить нормативы
            if (territory == 4)
            {
                DataRow bkNorm = normatives[year].FindNormative(refKd, year, refRegions, NormativesKind.NormativesBK, db);
                DataRow rfNorm = normatives[year].FindNormative(refKd, year, refRegions, NormativesKind.NormativesRegionRF, db);
                DataRow mrNorm = normatives[year].FindNormative(refKd, year, refRegions, NormativesKind.NormativesMR, db);
                DataRow varMrNorm = normatives[year].FindNormative(refKd, year, refRegions, NormativesKind.VarNormativesMR, db);

                if (bkNorm != null)
                    SplitRow(rowForSplit, dtSplittedData, bkNorm, variantType, territoryType, NormativesKind.NormativesBK);
                else
                {
                    NormativesUtils.SendMessageToProtocol(protocol,
                        string.Format(normativeNotFound, rowForSplit["Name"], rowForSplit["Code"], GetDataSourceName(Convert.ToInt32(rowForSplit["SourceID"])), year, "норматив расщепления БК не найден"), 
                        ClassifiersEventKind.ceWarning, variant);
                }
                if (rfNorm != null)
                    SplitRow(rowForSplit, dtSplittedData, rfNorm, variantType, territoryType, NormativesKind.NormativesRegionRF);
                else
                {
                    NormativesUtils.SendMessageToProtocol(protocol,
                        string.Format(normativeNotFound, rowForSplit["Name"], rowForSplit["Code"], GetDataSourceName(Convert.ToInt32(rowForSplit["SourceID"])), year, "норматив расщепления по субъекту РФ не найден"),
                        ClassifiersEventKind.ceWarning, variant);
                }
                if (mrNorm != null)
                    SplitRow(rowForSplit, dtSplittedData, mrNorm, variantType, territoryType, NormativesKind.NormativesMR);
                else
                {
                    NormativesUtils.SendMessageToProtocol(protocol,
                        string.Format(normativeNotFound, rowForSplit["Name"], rowForSplit["Code"], GetDataSourceName(Convert.ToInt32(rowForSplit["SourceID"])), year, "норматив расщепления по МР не найден"),
                        ClassifiersEventKind.ceWarning, variant);
                }
                if (varMrNorm != null)
                    SplitRow(rowForSplit, dtSplittedData, varMrNorm, variantType, territoryType, NormativesKind.VarNormativesMR);
                else
                {
                    NormativesUtils.SendMessageToProtocol(protocol,
                        string.Format(normativeNotFound, rowForSplit["Name"], rowForSplit["Code"], GetDataSourceName(Convert.ToInt32(rowForSplit["SourceID"])), year, "норматив расщепления по диф. МР не найден"),
                        ClassifiersEventKind.ceWarning, variant);
                }
                if (bkNorm == null && rfNorm == null && mrNorm == null && varMrNorm == null && (subCode == "2" || subCode == "3"))
                    SplitRow(rowForSplit, dtSplittedData, null, VariantType.BudgetLevels, territoryType);

            }
            else if (territory == 3)
            {
                DataRow bkNorm = normatives[year].FindNormative(refKd, year, refRegions, NormativesKind.NormativesBK, db);
                DataRow rfNorm = normatives[year].FindNormative(refKd, year, refRegions, NormativesKind.NormativesRegionRF, db);
                DataRow varRfNorm = normatives[year].FindNormative(refKd, year, refRegions, NormativesKind.VarNormativesRegionRF, db);

                if (bkNorm != null)
                    SplitRow(rowForSplit, dtSplittedData, bkNorm, variantType, territoryType, NormativesKind.NormativesBK);
                else
                {
                    NormativesUtils.SendMessageToProtocol(protocol,
                        string.Format(normativeNotFound, rowForSplit["Name"], rowForSplit["Code"], GetDataSourceName(Convert.ToInt32(rowForSplit["SourceID"])), year, "норматив расщепления БК не найден"),
                        ClassifiersEventKind.ceWarning, variant);
                }
                if (rfNorm != null)
                    SplitRow(rowForSplit, dtSplittedData, rfNorm, variantType, territoryType, NormativesKind.NormativesRegionRF);
                else
                {
                    NormativesUtils.SendMessageToProtocol(protocol,
                        string.Format(normativeNotFound, rowForSplit["Name"], rowForSplit["Code"], GetDataSourceName(Convert.ToInt32(rowForSplit["SourceID"])), year, "норматив расщепления по субъекту РФ не найден"),
                        ClassifiersEventKind.ceWarning, variant);
                }
                if (varRfNorm != null)
                    SplitRow(rowForSplit, dtSplittedData, varRfNorm, variantType, territoryType, NormativesKind.VarNormativesRegionRF);
                else
                {
                    NormativesUtils.SendMessageToProtocol(protocol,
                        string.Format(normativeNotFound, rowForSplit["Name"], rowForSplit["Code"], GetDataSourceName(Convert.ToInt32(rowForSplit["SourceID"])), year, "норматив расщепления по диф. субъекту РФ не найден"),
                        ClassifiersEventKind.ceWarning, variant);
                }
                if (bkNorm == null && rfNorm == null && varRfNorm == null && (subCode == "2" || subCode == "3"))
                    SplitRow(rowForSplit, dtSplittedData, null, VariantType.BudgetLevels, territoryType);
            }
            SubstractData(dtSplittedData);
            return dtSplittedData.Rows.Count > 0;
        }

        /// <summary>
        /// вычитание значений
        /// </summary>
        /// <param name="dtSplittedData"></param>
        private void SubstractData(DataTable dtSplittedData)
        {
            Dictionary<string, decimal> prevValues = new Dictionary<string, decimal>();
            for (int i = 1; i <= 5; i++)
            {
                for (int j = 1; j <= 17; j ++)
                {
                    foreach (DataRow row in dtSplittedData.Select(string.Format("RefNormDeduct = {0} and RefBudLevel = {1}", i, j)))
                    {
                        string estimate = string.Format("Estimate_{0}", j);
                        if (!prevValues.ContainsKey(estimate))
                            prevValues.Add(estimate, 0);
                        prevValues[estimate] = CalculateValue(row, "Estimate", prevValues[estimate]);

                        string forecast = string.Format("Forecast_{0}", j);
                        if (!prevValues.ContainsKey(forecast))
                            prevValues.Add(forecast, 0);
                        prevValues[forecast] = CalculateValue(row, "Forecast", prevValues[forecast]);

                        string design = string.Format("Design_{0}", j);
                        if (!prevValues.ContainsKey(design))
                            prevValues.Add(design, 0);
                        prevValues[design] = CalculateValue(row, "Design", prevValues[design]);

                        string fact = string.Format("Fact_{0}", j);
                        if (!prevValues.ContainsKey(fact))
                            prevValues.Add(fact, 0);
                        prevValues[fact] = CalculateValue(row, "Fact", prevValues[fact]);

                        string planPer = string.Format("PlanPer_{0}", j);
                        if (!prevValues.ContainsKey(planPer))
                            prevValues.Add(planPer, 0);
                        prevValues[planPer] = CalculateValue(row, "PlanPer", prevValues[planPer]);

                        string total = string.Format("Total_{0}", j);
                        if (!prevValues.ContainsKey(total))
                            prevValues.Add(total, 0);
                        prevValues[total] = CalculateValue(row, "Total", prevValues[total]);

                        string taxResource = string.Format("TaxResource_{0}", j);
                        if (!prevValues.ContainsKey(taxResource))
                            prevValues.Add(taxResource, 0);
                        prevValues[taxResource] = CalculateValue(row, "TaxResource", prevValues[taxResource]);
                    }
                }
            }

            foreach (DataRow row in dtSplittedData.Rows)
            {
                if (row.IsNull("Estimate") && row.IsNull("Forecast") && row.IsNull("Design") &&
                    row.IsNull("Fact") && row.IsNull("PlanPer") && row.IsNull("Total") && row.IsNull("TaxResource"))
                {
                    row.AcceptChanges();
                    row.Delete();
                }
            }
            dtSplittedData.AcceptChanges();
        }

        private decimal CalculateValue(DataRow row, string valueName, decimal prevValue)
        {
            string valueFisson = valueName + "Fission";
            decimal estimateFission = row.IsNull(valueFisson)
                                                      ? 0
                                                      : Convert.ToDecimal(row[valueFisson]);
            row[valueName] = (estimateFission - prevValue) == 0 ? DBNull.Value : (object)(estimateFission - prevValue);
            return estimateFission;
        }

        private void SplitRow(DataRow row, DataTable dtSplittedData, DataRow normativeRow,
            VariantType variantType, TerritoryType territoryType, NormativesKind normativesKind)
        {
            switch (variantType)
            {
                case VariantType.BudgetLevels:
                    // не производим никакого расщепления
                    // просто переписываем запись
                    InsertNewRow(row, territoryType, dtSplittedData);
                    break;
                case VariantType.QuotaToAllLevels:
                case VariantType.QuotaToConsMR:
                case VariantType.QuotaToConsRegion:
                    // производим расщепление согласно матрице
                    if (!((territoryType == TerritoryType.GO || territoryType == TerritoryType.SB) &&
                          variantType == VariantType.QuotaToConsMR)) 
                    {
                        SplitRow(row, territoryType, normativeRow, variantType, dtSplittedData, normativesKind);
                    }
                    break;
            }
        }

        /// <summary>
        /// ращепление записи 
        /// </summary>
        private void SplitRow(DataRow row, DataTable dtSplittedData, DataRow normativeRow,
            VariantType variantType, TerritoryType territoryType)
        {
            switch (variantType)
            {
                case VariantType.BudgetLevels:
                    // не производим никакого расщепления
                    // просто переписываем запись
                    InsertNewRow(row, territoryType, dtSplittedData);
                    break;
                case VariantType.QuotaToAllLevels:
                case VariantType.QuotaToConsMR:
                case VariantType.QuotaToConsRegion:
                    // производим расщепление согласно матрице
                    if (!((territoryType == TerritoryType.GO || territoryType == TerritoryType.SB) && variantType == VariantType.QuotaToConsMR))
                        SplitRow(row, territoryType, normativeRow, variantType, dtSplittedData, NormativesKind.Unknown);
                    break;
            }
        }

        /// <summary>
        /// расщепление данных в контингенте во все уровни бюджета
        /// </summary>
        private void SplitRow(DataRow splittedRow, TerritoryType territory,
            DataRow normative, VariantType variantType, DataTable dtSplittedData, NormativesKind normativesKind)
        {
            // значение, которое будем расщеплять 
            decimal forecastValue = (splittedRow.IsNull("Forecast") ? 0 : Convert.ToDecimal(splittedRow["Forecast"])) +
                (splittedRow.IsNull("Restructuring") ? 0 : Convert.ToDecimal(splittedRow["Restructuring"])) +
                (splittedRow.IsNull("Arrears") ? 0 : Convert.ToDecimal(splittedRow["Arrears"])) +
                (splittedRow.IsNull("Priorcharge") ? 0 : Convert.ToDecimal(splittedRow["Priorcharge"]));
            decimal estimateValue = splittedRow.IsNull("Estimate") ? 0 : Convert.ToDecimal(splittedRow["Estimate"]);
            decimal planValue = splittedRow.IsNull("Design") ? 0 : Convert.ToDecimal(splittedRow["Design"]);
            decimal factValue = splittedRow.IsNull("Fact") ? 0 : Convert.ToDecimal(splittedRow["Fact"]);
            decimal planPerValue = splittedRow.IsNull("PlanPer") ? 0 : Convert.ToDecimal(splittedRow["PlanPer"]);
            decimal totalValue = splittedRow.IsNull("Total") ? 0 : Convert.ToDecimal(splittedRow["Total"]);
            decimal taxResourceValue = splittedRow.IsNull("TaxResource") ? 0 : Convert.ToDecimal(splittedRow["TaxResource"]);
            // ращепляем данные
            Dictionary<int, decimal> normativePercents = GetNormativePercents(normative, variantType);
            Dictionary<int, decimal> estimateValues = SplitValue(estimateValue, variantType, normative);
            Dictionary<int, decimal> forecastValues = SplitValue(forecastValue, variantType, normative);
            Dictionary<int, decimal> planValues = SplitValue(planValue, variantType, normative);
            Dictionary<int, decimal> factValues = SplitValue(factValue, variantType, normative);
            Dictionary<int, decimal> planPerValues = SplitValue(planPerValue, variantType, normative);
            Dictionary<int, decimal> totalValues = SplitValue(totalValue, variantType, normative);
            Dictionary<int, decimal> taxResourceValues = SplitValue(taxResourceValue, variantType, normative);

            // для разных территорий записываем различные расщепления
            if (variantType == VariantType.QuotaToAllLevels && territory != TerritoryType.MT)
            {
                InsertSplittedRow(splittedRow, dtSplittedData, 1, factValues, planValues,
                    planPerValues, estimateValues, forecastValues, totalValues, taxResourceValues, normativesKind, normativePercents[1]);
            }
            if ((variantType == VariantType.QuotaToAllLevels || variantType == VariantType.QuotaToConsRegion)
                && territory != TerritoryType.MT)
            {
                InsertSplittedRow(splittedRow, dtSplittedData, 3, factValues, planValues,
                    planPerValues, estimateValues, forecastValues, totalValues, taxResourceValues, normativesKind, normativePercents[3]);
            }
            switch (territory)
            {
                case TerritoryType.GO:
                    if (variantType != VariantType.QuotaToConsMR)
                    {
                        InsertSplittedRow(splittedRow, dtSplittedData, 15, factValues, planValues,
                            planPerValues, estimateValues, forecastValues, totalValues, taxResourceValues, normativesKind, normativePercents[15]);
                    }
                    break;
                case TerritoryType.GP:
                    InsertSplittedRow(splittedRow, dtSplittedData, 5, factValues, planValues,
                        planPerValues, estimateValues, forecastValues, totalValues, taxResourceValues, normativesKind, normativePercents[5]);

                    InsertSplittedRow(splittedRow, dtSplittedData, 16, factValues, planValues,
                        planPerValues, estimateValues, forecastValues, totalValues, taxResourceValues, normativesKind, normativePercents[16]);
                    break;
                case TerritoryType.MR:
                    InsertSplittedRow(splittedRow, dtSplittedData, 5, factValues, planValues,
                        planPerValues, estimateValues, forecastValues, totalValues, taxResourceValues, normativesKind, normativePercents[5]);

                    InsertSplittedRow(splittedRow, dtSplittedData, 6, factValues, planValues,
                        planPerValues, estimateValues, forecastValues, totalValues, taxResourceValues, normativesKind, normativePercents[6]);
                    break;
                case TerritoryType.MT:
                    InsertSplittedRow(splittedRow, dtSplittedData, 1, factValues, planValues,
                        planPerValues, estimateValues, forecastValues, totalValues, taxResourceValues, normativesKind, normativePercents[1]);

                    InsertSplittedRow(splittedRow, dtSplittedData, 3, factValues, planValues,
                        planPerValues, estimateValues, forecastValues, totalValues, taxResourceValues, normativesKind, normativePercents[3]);
                    // Данные по консолидированному бюджету МР записываем на уровень 
                    InsertSplittedRow(splittedRow, dtSplittedData, 7, factValues, planValues,
                        planPerValues, estimateValues, forecastValues, totalValues, taxResourceValues, normativesKind, normativePercents[7]);
                    break;
                case TerritoryType.SB:
                    if (variantType != VariantType.QuotaToConsMR)
                    {
                        // в таблицу с расщеплением записываем записи согласно матрице
                        InsertSplittedRow(splittedRow, dtSplittedData, 14, factValues, planValues,
                            planPerValues, estimateValues, forecastValues, totalValues, taxResourceValues, normativesKind, normativePercents[14]);
                    }
                    break;
                case TerritoryType.SP:
                    InsertSplittedRow(splittedRow, dtSplittedData, 5, factValues, planValues,
                        planPerValues, estimateValues, forecastValues, totalValues, taxResourceValues, normativesKind, normativePercents[5]);

                    InsertSplittedRow(splittedRow, dtSplittedData, 17, factValues, planValues,
                        planPerValues, estimateValues, forecastValues, totalValues, taxResourceValues, normativesKind, normativePercents[17]);
                    break;
                case TerritoryType.RC:
                    InsertSplittedRow(splittedRow, dtSplittedData, 1, factValues, planValues,
                        planPerValues, estimateValues, forecastValues, totalValues, taxResourceValues, normativesKind, normativePercents[1]);

                    InsertSplittedRow(splittedRow, dtSplittedData, 6, factValues, planValues,
                        planPerValues, estimateValues, forecastValues, totalValues, taxResourceValues, normativesKind, normativePercents[6]);
                    break;
            }
            // для всех территорий одинаковые вычисления
            if (variantType == VariantType.QuotaToAllLevels && territory != TerritoryType.MT)
            {
                InsertSplittedRow(splittedRow, dtSplittedData, 7, factValues, planValues,
                    planPerValues, estimateValues, forecastValues, totalValues, taxResourceValues, normativesKind, normativePercents[7]);

                InsertSplittedRow(splittedRow, dtSplittedData, 12, factValues, planValues,
                    planPerValues, estimateValues, forecastValues, totalValues, taxResourceValues, normativesKind, normativePercents[12]);

                InsertSplittedRow(splittedRow, dtSplittedData, 13, factValues, planValues,
                    planPerValues, estimateValues, forecastValues, totalValues, taxResourceValues, normativesKind, normativePercents[13]);
            }
        }

        /// <summary>
        /// расщепляем любое значение по нормативу исходя из типа варианта
        /// </summary>
        private Dictionary<int, decimal> SplitValue(decimal splitedValue, VariantType variantType,
            DataRow normative)
        {
            Dictionary<int, decimal> splitResults = new Dictionary<int, decimal>();
            switch (variantType)
            {
                case VariantType.QuotaToConsMR:
                    if (Convert.ToDecimal(normative["4_Value"]) == 0)
                    {
                        splitedValue = 0;
                    }
                    else
                    {
                        splitedValue = Math.Round(splitedValue/Convert.ToDecimal(normative["4_Value"]), 6,
                                                  MidpointRounding.AwayFromZero);
                    }
                    break;
                case VariantType.QuotaToConsRegion:
                    if (Convert.ToDecimal(normative["2_Value"]) == 0)
                    {
                        splitedValue = 0;
                    }
                    else
                    {
                        splitedValue = Math.Round(splitedValue/Convert.ToDecimal(normative["2_Value"]), 6,
                                                  MidpointRounding.AwayFromZero);
                    }
                    break;
            }
            
            // федеральный бюджет
            splitResults.Add(1, splitedValue * Convert.ToDecimal(normative[string.Format("{0}{1}", 1, NormativesObjectKeys.VALUE_POSTFIX)]));
            // бюджет субъекта
            splitResults.Add(3, splitedValue * Convert.ToDecimal(normative[string.Format("{0}{1}", 3, NormativesObjectKeys.VALUE_POSTFIX)]));
            // консолидированный бюджет муниципального образования
            splitResults.Add(14, splitedValue * Convert.ToDecimal(normative[string.Format("{0}{1}", 14, NormativesObjectKeys.VALUE_POSTFIX)]));
            // бюджет муниципального района
            splitResults.Add(5, splitedValue * Convert.ToDecimal(normative[string.Format("{0}{1}", 5, NormativesObjectKeys.VALUE_POSTFIX)]));
            // бюджет поселения
            splitResults.Add(6, splitedValue * Convert.ToDecimal(normative[string.Format("{0}{1}", 6, NormativesObjectKeys.VALUE_POSTFIX)]));
            // бюджет городского поселения
            splitResults.Add(16, splitedValue * Convert.ToDecimal(normative[string.Format("{0}{1}", 16, NormativesObjectKeys.VALUE_POSTFIX)]));
            // бюджет сельского поселения
            splitResults.Add(17, splitedValue * Convert.ToDecimal(normative[string.Format("{0}{1}", 17, NormativesObjectKeys.VALUE_POSTFIX)]));
            // бюджет городского округа
            splitResults.Add(15, splitedValue * Convert.ToDecimal(normative[string.Format("{0}{1}", 15, NormativesObjectKeys.VALUE_POSTFIX)]));
            // внебюджетные фонды
            splitResults.Add(7, splitedValue * Convert.ToDecimal(normative[string.Format("{0}{1}", 7, NormativesObjectKeys.VALUE_POSTFIX)]));
            // уфк смоленска
            splitResults.Add(12, splitedValue * Convert.ToDecimal(normative[string.Format("{0}{1}", 12, NormativesObjectKeys.VALUE_POSTFIX)]));
            // тюмень
            splitResults.Add(13, splitedValue * Convert.ToDecimal(normative[string.Format("{0}{1}", 13, NormativesObjectKeys.VALUE_POSTFIX)]));
            return splitResults;
        }

        private Dictionary<int, decimal> GetNormativePercents(DataRow normativeRow, VariantType variantType)
        {
            Dictionary<int, decimal> normativePercents = new Dictionary<int, decimal>();
            switch (variantType)
            {
                case VariantType.QuotaToConsMR:
                    if (Convert.ToDecimal(normativeRow["4_Value"]) == 0)
                    {
                        normativePercents.Add(4, 0);
                    }
                    else
                    {
                        normativePercents.Add(4, 1 / Convert.ToDecimal(normativeRow["4_Value"]));
                    }
                    break;
                case VariantType.QuotaToConsRegion:
                    if (Convert.ToDecimal(normativeRow["2_Value"]) == 0)
                    {
                        normativePercents.Add(2, 0);
                    }
                    else
                    {
                        normativePercents.Add(2, 1 / Convert.ToDecimal(normativeRow["2_Value"]));
                    }
                    break;
            }

            normativePercents.Add(1, Convert.ToDecimal(normativeRow[string.Format("{0}{1}", 1, NormativesObjectKeys.VALUE_POSTFIX)]));
            normativePercents.Add(3, Convert.ToDecimal(normativeRow[string.Format("{0}{1}", 3, NormativesObjectKeys.VALUE_POSTFIX)]));
            normativePercents.Add(14, Convert.ToDecimal(normativeRow[string.Format("{0}{1}", 14, NormativesObjectKeys.VALUE_POSTFIX)]));
            normativePercents.Add(5, Convert.ToDecimal(normativeRow[string.Format("{0}{1}", 5, NormativesObjectKeys.VALUE_POSTFIX)]));
            normativePercents.Add(6, Convert.ToDecimal(normativeRow[string.Format("{0}{1}", 6, NormativesObjectKeys.VALUE_POSTFIX)]));
            normativePercents.Add(16, Convert.ToDecimal(normativeRow[string.Format("{0}{1}", 16, NormativesObjectKeys.VALUE_POSTFIX)]));
            normativePercents.Add(17, Convert.ToDecimal(normativeRow[string.Format("{0}{1}", 17, NormativesObjectKeys.VALUE_POSTFIX)]));
            normativePercents.Add(15, Convert.ToDecimal(normativeRow[string.Format("{0}{1}", 15, NormativesObjectKeys.VALUE_POSTFIX)]));
            normativePercents.Add(7, Convert.ToDecimal(normativeRow[string.Format("{0}{1}", 7, NormativesObjectKeys.VALUE_POSTFIX)]));
            normativePercents.Add(12, Convert.ToDecimal(normativeRow[string.Format("{0}{1}", 12, NormativesObjectKeys.VALUE_POSTFIX)]));
            normativePercents.Add(13, Convert.ToDecimal(normativeRow[string.Format("{0}{1}", 13, NormativesObjectKeys.VALUE_POSTFIX)]));
            return normativePercents;
        }


        /// <summary>
        /// добавление записи в таблицу с расщеплениями
        /// </summary>
        private void InsertSplittedRow(DataRow splittedRow, DataTable dt,
            int budLevel, Dictionary<int, decimal> factValues, Dictionary<int, decimal> planValues,
            Dictionary<int, decimal> planPerValues, Dictionary<int, decimal> estimateValues,
            Dictionary<int, decimal> forecastValues, Dictionary<int, decimal> totalValues,
            Dictionary<int, decimal> taxResources, NormativesKind normativesKind, decimal normativeValue)
        {
            if ((factValues[budLevel] != 0 || planValues[budLevel] != 0 || planPerValues[budLevel] != 0 ||
                        estimateValues[budLevel] != 0 || forecastValues[budLevel] != 0 || totalValues[budLevel] != 0 || taxResources[budLevel] != 0) || (normativesKind != NormativesKind.Unknown))
            {
                string fission = normativesKind == NormativesKind.Unknown ? string.Empty : "Fission";
                // разобраться с вариантом и полчить ID для новой записи
                DataRow newRow = dt.NewRow();
                newRow.BeginEdit();
                newRow["SourceID"] = splittedRow["SourceID"];
                newRow["TaskID"] = -1;
                newRow["RefYearDayUNV"] = splittedRow["RefYearDayUNV"];
                newRow["RefOrganizations"] = splittedRow["RefOrganizations"];
                newRow["RefTaxObjects"] = splittedRow["RefTaxObjects"];
                newRow["RefKVSR"] = splittedRow["RefKVSR"];
                newRow["RefFODepartments"] = splittedRow["RefFODepartments"];
                newRow["RefRegions"] = splittedRow["RefRegions"];
                newRow["RefKD"] = splittedRow["RefKD"];
                newRow["RefBudLevel"] = budLevel;
                newRow["Fact" + fission] = splittedRow.IsNull("Fact") ? (object)DBNull.Value : factValues[budLevel];
                newRow["Design" + fission] = splittedRow.IsNull("Design") ? (object)DBNull.Value : planValues[budLevel];
                newRow["PlanPer" + fission] = splittedRow.IsNull("PlanPer") ? (object)DBNull.Value : planPerValues[budLevel];
                newRow["Estimate" + fission] = splittedRow.IsNull("Estimate") ? (object)DBNull.Value : estimateValues[budLevel];
                newRow["Forecast" + fission] = forecastValues[budLevel] == 0 ? (object)DBNull.Value : forecastValues[budLevel];
                newRow["Total" + fission] = splittedRow.IsNull("Total") ? (object)DBNull.Value : totalValues[budLevel];
                newRow["TaxResource" + fission] = splittedRow.IsNull("TaxResource")
                                                      ? (object) DBNull.Value
                                                      : taxResources[budLevel];
                newRow["FromSF"] = 0;
                newRow["RefNormDeduct"] = (int) normativesKind;
                newRow["NormDeductVal"] = normativeValue;
                newRow["IsBlocked"] = 0;
                newRow.EndEdit();
                dt.Rows.Add(newRow);
            }
        }

        private static void InsertNewRow(DataRow row, TerritoryType territoryType, DataTable dtSplittedData)
        {
            // разобраться с вариантом и полчить ID для новой записи
            DataRow newRow = dtSplittedData.NewRow();
            newRow.BeginEdit();
            newRow["SourceID"] = row["SourceID"];
            newRow["TaskID"] = -1;
            newRow["RefYearDayUNV"] = row["RefYearDayUNV"];
            newRow["RefOrganizations"] = row["RefOrganizations"];
            newRow["RefTaxObjects"] = row["RefTaxObjects"];
            newRow["RefKVSR"] = row["RefKVSR"];
            newRow["RefFODepartments"] = row["RefFODepartments"];
            newRow["RefRegions"] = row["RefRegions"];
            newRow["RefKD"] = row["RefKD"];
            newRow["RefBudLevel"] = NormativesUtils.GetBudgetLevel(territoryType);
            newRow["Fact"] = row.IsNull("Fact") ? (object)DBNull.Value : row["Fact"];
            newRow["Design"] = row.IsNull("Design") ? (object)DBNull.Value : row["Design"];
            newRow["PlanPer"] = row.IsNull("PlanPer") ? (object)DBNull.Value : row["PlanPer"];
            newRow["Estimate"] = row.IsNull("Estimate") ? (object)DBNull.Value : row["Estimate"];
            decimal forecastValue = (row.IsNull("Forecast") ? 0 : Convert.ToDecimal(row["Forecast"])) +
                (row.IsNull("Restructuring") ? 0 : Convert.ToDecimal(row["Restructuring"])) +
                (row.IsNull("Arrears") ? 0 : Convert.ToDecimal(row["Arrears"])) +
                (row.IsNull("Priorcharge") ? 0 : Convert.ToDecimal(row["Priorcharge"]));
            newRow["Forecast"] = forecastValue == 0 ? (object)DBNull.Value :
                (object)forecastValue;
            newRow["Total"] = row.IsNull("Total") ? (object)DBNull.Value : row["Total"];
            newRow["TaxResource"] = row.IsNull("TaxResource") ? (object)DBNull.Value : row["TaxResource"];
            newRow["FromSF"] = 0;
            newRow["RefNormDeduct"] = (int)NormativesKind.Unknown;
            newRow["NormDeductVal"] = 1;
            newRow["IsBlocked"] = 0;
            newRow.EndEdit();
            dtSplittedData.Rows.Add(newRow);
        }

        #endregion

        private Dictionary<int, string> dataSourcesNames;

        private string GetDataSourceName(int sourceId)
        {
            if (dataSourcesNames == null)
                dataSourcesNames = new Dictionary<int, string>();
            if (dataSourcesNames.ContainsKey(sourceId))
                return dataSourcesNames[sourceId];
            dataSourcesNames.Add(sourceId, scheme.DataSourceManager.GetDataSourceName(sourceId));
            return dataSourcesNames[sourceId];
        }

    }

    internal class NormativeReferences
    {
        private int refKD;
        private int refYearDayUNV;
        private int refRegions;
        private string kdCode;

        internal NormativeReferences(object refKD, string kdCode, object refYearDayUNV, object refRegions)
        {
            this.refKD = Convert.ToInt32(refKD);
            this.refYearDayUNV = Convert.ToInt32(refYearDayUNV);
            this.refRegions = Convert.ToInt32(refRegions);
            this.kdCode = kdCode;
        }

        public int RefKD
        {
            get { return refKD; }
        }

        public  string KDCode
        {
            get { return kdCode; }
        }

        public int RefYearDayUNV
        {
            get { return refYearDayUNV; }
        }

        public int RefRegions
        {
            get { return refRegions; }
        }
    }
}
