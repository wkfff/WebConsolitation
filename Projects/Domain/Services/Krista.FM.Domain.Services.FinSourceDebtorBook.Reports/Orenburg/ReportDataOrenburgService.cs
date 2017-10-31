using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Domain.Services.FinSourceDebtorBook.Reports
{
    /// <summary>
    /// Получение данных - Оренбург
    /// </summary>
    public partial class ReportsDataService
    {
        public void GetDebtBookOrenburgData(
            DebtorBookOrenburgReport.ReportType reportType,
            int formVersion,
            int refVariant, 
            int refRegion, 
            ref DataTable[] tables, 
            DateTime calculateDate)
        {
            // Основные таблицы
            tables = new DataTable[26];
            const string ContactQuery = "select {0} from {1} where RefVariant = {2} and RefRegion in ({3}) {4}";
            var dataEntityList = new Collection<string>();
            var dataEntityFilters = new Collection<string>();

            // Кредиты организаций
            dataEntityList.Add(DomainObjectsKeys.f_S_SchBCreditincome);
            dataEntityFilters.Add("and RefTypeCredit = 0");

            // Кредиты бюджетов
            dataEntityList.Add(DomainObjectsKeys.f_S_SchBCreditincome);
            dataEntityFilters.Add("and RefTypeCredit = 1");

            // Гарантии
            dataEntityList.Add(DomainObjectsKeys.f_S_SchBGuarantissued);
            dataEntityFilters.Add(String.Empty);

            // ЦБ
            dataEntityList.Add(DomainObjectsKeys.f_S_SchBCapital);
            dataEntityFilters.Add(String.Empty);

            // Иные
            dataEntityList.Add(DomainObjectsKeys.f_S_SchBCreditincome);
            dataEntityFilters.Add("and RefTypeCredit = 5");
            var rgnEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.d_Regions_Analysis);
            var isSubject = true;

            using (var db = scheme.SchemeDWH.DB)
            {
                var regionName = String.Empty;
                var listMO = GetRegionSettles(db, rgnEntity, refRegion, ref regionName, 3);
                var tblSettle = GetRegionByType("5,6");
                var listSettle = tblSettle.Rows.Cast<DataRow>()
                    .Aggregate(String.Empty, (current, rowSettle) => String.Format("{0},{1}", current, Convert.ToString(rowSettle["id"])));
                listSettle = listSettle.Trim(',');
                var stubName = String.Empty;

                if (listMO.Length == 0)
                {
                    listMO = refRegion.ToString();
                    isSubject = false;
                    listSettle = GetRegionSettles(db, rgnEntity, refRegion, ref stubName, 4);
                }

                var listMOActual = listMO;

                if (reportType == DebtorBookOrenburgReport.ReportType.Region)
                {
                    listSettle = String.Empty;
                }
                
                if (reportType == DebtorBookOrenburgReport.ReportType.Settles)
                {
                    listMO = String.Empty;
                }

                var fullTerritoryList = String.Format("{0},{1}", listMOActual, GetSettlesKeys(listMOActual)).Trim(',');
                var listFull = String.Format("{0},{1}", listMO, listSettle).Trim(',');
                var dictRegion = new Dictionary<string, List<int>>();

                if (String.Compare(listMOActual, listFull, StringComparison.InvariantCultureIgnoreCase) != 0)
                {
                    var regionList = listMOActual.Split(',');

                    foreach (var region in regionList)
                    {
                        var regionId = Convert.ToInt32(region);
                        var strSettle = GetRegionSettles(db, rgnEntity, regionId, ref stubName, 4);

                        if (strSettle.Length <= 0)
                        {
                            continue;
                        }

                        var childs = strSettle.Split(',');
                        var list = childs.Select(child => Convert.ToInt32(child)).ToList();

                        dictRegion.Add(stubName, list);
                    }
                }

                // данные по одному мо
                for (var i = 0; i < dataEntityList.Count; i++)
                {
                    var dataColumns = new Dictionary<string, ReportColumnType>();

                    // настраиваем списки колонок
                    if (i == 0)
                    {
                        dataColumns = GetOrenburgOrgCreditColumnList(formVersion);
                    }

                    if (i == 1)
                    {
                        dataColumns = GetOrenburgBudCreditColumnList(formVersion);
                    }

                    if (i == 2)
                    {
                        dataColumns = GetOrenburgGarantColumnList(formVersion);
                    }

                    if (i == 3)
                    {
                        dataColumns = GetOrenburgCapitalColumnList(formVersion);
                    }

                    if (i == 4)
                    {
                        dataColumns = GetOrenburgCreditOtherColumnList(formVersion);
                    }

                    // выбираем данные из мастер таблицы
                    var dataEntity = scheme.RootPackage.FindEntityByName(dataEntityList[i]);
                    var fieldList = GetFieldNames(dataEntity, String.Empty);
                    var fltType = dataEntityFilters[i];
                    var tableName = dataEntity.FullDBName;

                    if (listMO.Length != 0)
                    {
                        var tblContracts = (DataTable)db.ExecQuery(
                            String.Format(ContactQuery, fieldList, tableName, refVariant, listMO, fltType),
                            QueryResultTypes.DataTable);

                        // преобразуем данные мастер таблицы согласно списку нужных колонок
                        tables[i] = FillTableData(tblContracts, dataColumns);
                    }

                    // табличка по поселениям
                    if (listSettle.Length != 0)
                    {
                        var tblContractsSettles = (DataTable)db.ExecQuery(
                            String.Format(ContactQuery, fieldList, tableName, refVariant, listSettle, fltType),
                            QueryResultTypes.DataTable);
                        tables[14 + i] = FillTableData(tblContractsSettles, dataColumns);
                    }
                }

                var tblVillage = GetRegionByType(listMOActual, 4);
                var tblTown = GetRegionByType(listMOActual, 7);
                var tblTownSettle = GetRegionByType(listFull, 5);
                var tblSeloSettle = GetRegionByType(listFull, 6);

                // данные по всем мо
                for (var i = 0; i < dataEntityList.Count; i++)
                {
                    var dataColumns = new Dictionary<string, ReportColumnType>();

                    // настраиваем списки колонок
                    if (i == 0 || i == 1 || i == 4)
                    {
                        dataColumns = GetOrenburgSubjectColumnListCredit(formVersion);
                    }

                    if (i == 2)
                    {
                        dataColumns = GetOrenburgSubjectColumnListGarant(formVersion);
                    }

                    if (i == 3)
                    {
                        dataColumns = GetOrenburgSubjectColumnListCapital(formVersion);
                    }

                    // выбираем данные из мастер таблицы
                    var dataEntity = scheme.RootPackage.FindEntityByName(dataEntityList[i]);
                    var fieldList = GetFieldNames(dataEntity, String.Empty);
                    var fltType = dataEntityFilters[i];
                    var tableName = dataEntity.FullDBName;

                    if (listSettle.Length != 0)
                    {
                        var tblSettleContracts = (DataTable)db.ExecQuery(
                            String.Format(ContactQuery, fieldList, tableName, refVariant, listSettle, fltType),
                            QueryResultTypes.DataTable);
                        var settleData = FillTableData(tblSettleContracts, dataColumns);
                        var tblResultSettle = CreateSettleList(
                            settleData, 
                            tblTown, 
                            tblVillage, 
                            tblTownSettle, 
                            tblSeloSettle);
                        tables[19 + i] = GroupSubjectDataOrenburg(settleData, dictRegion, tblResultSettle);
                    }

                    var tblContracts = (DataTable)db.ExecQuery(
                        String.Format(ContactQuery, fieldList, tableName, refVariant, listFull, fltType),
                       QueryResultTypes.DataTable);
                    var fullData = FillTableData(tblContracts, dataColumns);
                    var tblResultRegion = CreateSettleList(
                        fullData,
                        tblTown,
                        tblVillage,
                        tblTownSettle,
                        tblSeloSettle);
                    tables[7 + i] = GroupSubjectDataOrenburg(fullData, dictRegion, tblResultRegion);
                }

                if (listSettle.Length != 0)
                {
                    tables[24] = tables[7].Clone();

                    for (var i = 0; i < 5; i++)
                    {
                        var sumIndex = 0;

                        switch (i)
                        {
                            case 0:
                            case 1:
                            case 4:
                                sumIndex = 15;
                                break;
                            case 2:
                                sumIndex = formVersion == 1 ? 19 : 17;
                                break;
                            case 3:
                                sumIndex = 3;
                                break;
                        }

                        var currentTable = tables[7 + i];

                        for (var j = 0; j < currentTable.Rows.Count - 1; j++)
                        {
                            var sourceRow = currentTable.Rows[j];
                            var rowSettleSummary = i == 0 ? tables[24].Rows.Add() : tables[24].Rows[j];
                            var settleName = Convert.ToString(sourceRow[1]);
                            var settleCode = Convert.ToInt32(sourceRow[2]);
                            rowSettleSummary[0] = sourceRow[0];
                            rowSettleSummary[1] = settleName;

                            if (dictRegion.Any(r => r.Value.Contains(settleCode)))
                            {
                                var regionRecord = dictRegion.Where(r => r.Value.Contains(settleCode));
                                var columnName = currentTable.Columns[1].ColumnName;

                                var rowFind = FindDataRow(
                                    rowSettleSummary.Table, 
                                    Convert.ToString(regionRecord.First().Key), 
                                    columnName);

                                if (rowFind == null)
                                {
                                    continue;
                                }

                                rowFind[2] = GetNumber(sourceRow[sumIndex]) + GetNumber(rowFind[2]);
                            }
                        }
                    }
                }

                // просроченная задолженность
                var tblStale = CreateSettleList(
                    CreateReportCaptionTable(50),
                    tblTown,
                    tblVillage,
                    tblTownSettle,
                    tblSeloSettle);

                // данные по всем мо
                for (var i = 0; i < dataEntityList.Count; i++)
                {
                    var dataColumns = new Dictionary<string, ReportColumnType>();

                    // настраиваем списки колонок
                    if (i == 0 || i == 1 || i == 4)
                    {
                        dataColumns = GetOrenburgStaleListCredit();
                    }

                    if (i == 2)
                    {
                        dataColumns = GetOrenburgStaleListGarant();
                    }

                    if (i == 3)
                    {
                        dataColumns = GetOrenburgStaleListCapital();
                    }

                    // выбираем данные из мастер таблицы
                    var dataEntity = scheme.RootPackage.FindEntityByName(dataEntityList[i]);
                    var fieldList = GetFieldNames(dataEntity, String.Empty);
                    var fltType = dataEntityFilters[i];
                    var tableName = dataEntity.FullDBName;

                    var tblContracts = (DataTable)db.ExecQuery(
                        String.Format(ContactQuery, fieldList, tableName, refVariant, listFull, fltType),
                       QueryResultTypes.DataTable);
                    var fullData = FillTableData(tblContracts, dataColumns);
                    tblStale = GroupStaleDataOrenburg(fullData, dictRegion, tblStale, i);
                }

                tables[25] = tblStale;
                DeleteRegionColumn(tables[25]);

                for (var i = 0; i < 5; i++)
                {
                    DeleteRegionColumn(tables[07 + i]);
                    DeleteRegionColumn(tables[19 + i]);
                }

                tables[12] = GetLimitsData(refVariant, listMOActual);
                tables[13] = GetNotesData(refVariant, listMOActual);

                var realReportDate = calculateDate.AddMonths(-1);

                // структура долга одиночного МО
                tables[05] = FillMOStructureTable(tables, formVersion);

                // заголовочная таблица
                tables[6] = CreateReportCaptionTable(50);
                var rowCaption = tables[6].Rows.Add();
                rowCaption[0] = realReportDate.Year;
                rowCaption[1] = String.Format("01.01.{0}", calculateDate.Year);
                rowCaption[2] = realReportDate.Month;
                rowCaption[3] = calculateDate.ToShortDateString();
                rowCaption[4] = GetBookValue(scheme, DomainObjectsKeys.d_Regions_Analysis, refRegion.ToString());
                rowCaption[5] = GetMonthText(calculateDate.Month);
                rowCaption[6] = isSubject;

                var kstCodes = new Collection<int>
                                   {
                                       1074, 1072, 1076, 1071, 1075
                                   };

                for (var i = 0; i < kstCodes.Count; i++)
                {
                    rowCaption[07 + i] = GetMonthReportData(calculateDate, listMOActual, kstCodes[i], false);
                    rowCaption[12 + i] = GetMonthReportData(calculateDate, fullTerritoryList, kstCodes[i], true);
                }

                rowCaption[40] = reportType == DebtorBookOrenburgReport.ReportType.Full;
                rowCaption[41] = reportType == DebtorBookOrenburgReport.ReportType.Region;
                rowCaption[42] = reportType == DebtorBookOrenburgReport.ReportType.Settles;
                rowCaption[43] = formVersion;
            }
        }

        private void DeleteRegionColumn(DataTable tbl)
        {
            if (tbl != null)
            {
                tbl.Columns.RemoveAt(2);
            }
        }

        // Список колонок ДК Оренбург - Кредиты организаций
        private Dictionary<string, ReportColumnType> GetOrenburgOrgCreditColumnList(int formVersion)
        {
            var dataColumns = new Dictionary<string, ReportColumnType>();
            dataColumns.Add("Counter", ReportColumnType.ctCounter);
            dataColumns.Add("Occasion", ReportColumnType.ctDataStr);
            dataColumns.Add("CreditInfo", ReportColumnType.ctCalc);
            dataColumns.Add("DebtEndDate", ReportColumnType.ctDataStr);
            dataColumns.Add("Creditor", ReportColumnType.ctDataStr);
            dataColumns.Add("Purpose", ReportColumnType.ctDataStr);
            dataColumns.Add("Sum", ReportColumnType.ctDataNum);
            dataColumns.Add("CreditPercent", ReportColumnType.ctDataStr);
            dataColumns.Add("+9;+10;+11", ReportColumnType.ctForm);
            dataColumns.Add("BgnYearDbt", ReportColumnType.ctDataNum);
            dataColumns.Add("BgnYearInterest", ReportColumnType.ctDataNum);
            dataColumns.Add("BgnYearPenlt", ReportColumnType.ctDataNum);
            dataColumns.Add("+13;+14;+15", ReportColumnType.ctForm);
            dataColumns.Add("Attract", ReportColumnType.ctDataNum);
            dataColumns.Add("PlanService", ReportColumnType.ctDataNum);
            dataColumns.Add("ChargePenlt", ReportColumnType.ctDataNum);
            dataColumns.Add("+17;+18;+19", ReportColumnType.ctForm);
            dataColumns.Add("Discharge", ReportColumnType.ctDataNum);
            dataColumns.Add("FactService", ReportColumnType.ctDataNum);
            dataColumns.Add("FactPenlt", ReportColumnType.ctDataNum);
            dataColumns.Add("+9;+13;-17;+10;+14;-18;+11;+15;-19", ReportColumnType.ctForm);
            dataColumns.Add("+9;+13;-17", ReportColumnType.ctForm);
            dataColumns.Add("+10;+14;-18", ReportColumnType.ctForm);
            dataColumns.Add("+11;+15;-19", ReportColumnType.ctForm);
            dataColumns.Add("StaleDebt", ReportColumnType.ctDataNum);
            dataColumns.Add("StaleInterest", ReportColumnType.ctDataNum);
            dataColumns.Add("StalePenlt", ReportColumnType.ctDataNum);

            // для расчета листа структуры 27, 28, 29, 30
            dataColumns.Add("StaleDebtBgn", ReportColumnType.ctDataNum);
            dataColumns.Add("StaleInterestBgn", ReportColumnType.ctDataNum);
            dataColumns.Add("Zero", ReportColumnType.ctCalc);
            dataColumns.Add("StalePenltBgn", ReportColumnType.ctDataNum);
            dataColumns.Add("+24;+25;+26", ReportColumnType.ctForm);
            dataColumns.Add("+24", ReportColumnType.ctForm);
            dataColumns.Add("CalcDetailRegionName", ReportColumnType.ctCalc);
            return dataColumns;
        }

        // Список колонок ДК Оренбург - Кредиты бюджетов
        private Dictionary<string, ReportColumnType> GetOrenburgBudCreditColumnList(int formVersion)
        {
            var dataColumns = new Dictionary<string, ReportColumnType>();
            dataColumns.Add("Counter", ReportColumnType.ctCounter);
            dataColumns.Add("Occasion", ReportColumnType.ctDataStr);
            dataColumns.Add("Creditor", ReportColumnType.ctDataStr);
            dataColumns.Add("CreditInfo", ReportColumnType.ctCalc);
            dataColumns.Add("DebtEndDate", ReportColumnType.ctDataStr);
            dataColumns.Add("Purpose", ReportColumnType.ctDataStr);
            dataColumns.Add("Sum", ReportColumnType.ctDataNum);
            dataColumns.Add("CreditPercent", ReportColumnType.ctDataStr);
            dataColumns.Add("+9;+10;+11", ReportColumnType.ctForm);
            dataColumns.Add("BgnYearDbt", ReportColumnType.ctDataNum);
            dataColumns.Add("BgnYearInterest", ReportColumnType.ctDataNum);
            dataColumns.Add("BgnYearPenlt", ReportColumnType.ctDataNum);
            dataColumns.Add("+13;+14;+15", ReportColumnType.ctForm);
            dataColumns.Add("Attract", ReportColumnType.ctDataNum);
            dataColumns.Add("PlanService", ReportColumnType.ctDataNum);
            dataColumns.Add("ChargePenlt", ReportColumnType.ctDataNum);
            dataColumns.Add("+17;+18;+19", ReportColumnType.ctForm);
            dataColumns.Add("Discharge", ReportColumnType.ctDataNum);
            dataColumns.Add("FactService", ReportColumnType.ctDataNum);
            dataColumns.Add("FactPenlt", ReportColumnType.ctDataNum);
            dataColumns.Add("+9;+13;-17;+10;+14;-18;+11;+15;-19", ReportColumnType.ctForm);
            dataColumns.Add("+9;+13;-17", ReportColumnType.ctForm);
            dataColumns.Add("+10;+14;-18", ReportColumnType.ctForm);
            dataColumns.Add("+11;+15;-19", ReportColumnType.ctForm);
            dataColumns.Add("StaleDebt", ReportColumnType.ctDataNum);
            dataColumns.Add("StaleInterest", ReportColumnType.ctDataNum);
            dataColumns.Add("StalePenlt", ReportColumnType.ctDataNum);
            dataColumns.Add("DiffrncRatesDbt", ReportColumnType.ctDataNum);
            dataColumns.Add("DiffrncRatesInterest", ReportColumnType.ctDataNum);
            
            // для расчета листа структуры 29, 30, 31, 32
            dataColumns.Add("StaleDebtBgn", ReportColumnType.ctDataNum);
            dataColumns.Add("StaleInterestBgn", ReportColumnType.ctDataNum);
            dataColumns.Add("Zero", ReportColumnType.ctCalc);
            dataColumns.Add("StalePenltBgn", ReportColumnType.ctDataNum);
            dataColumns.Add("+24;+25;+26", ReportColumnType.ctForm);
            dataColumns.Add("+24", ReportColumnType.ctForm);
            dataColumns.Add("CalcDetailRegionName", ReportColumnType.ctCalc);
            return dataColumns;
        }

        // Список колонок ДК Оренбург - Гарантии
        private Dictionary<string, ReportColumnType> GetOrenburgGarantColumnList(int formVersion)
        {
            var dataColumns = new Dictionary<string, ReportColumnType>();

            if (formVersion == 2)
            {
                dataColumns.Add("Counter", ReportColumnType.ctCounter); // 0
                dataColumns.Add("Occasion", ReportColumnType.ctDataStr); // 1
                dataColumns.Add("GarantInfo", ReportColumnType.ctCalc); // 2
                dataColumns.Add("Purpose", ReportColumnType.ctDataStr); // 3
                dataColumns.Add("GarantNum", ReportColumnType.ctCalc); // 4
                dataColumns.Add("StartDate", ReportColumnType.ctDataStr); // 5
                dataColumns.Add("Sum", ReportColumnType.ctDataNum); // 6
                dataColumns.Add("Principal", ReportColumnType.ctDataStr); // 7
                dataColumns.Add("Creditor", ReportColumnType.ctDataStr); // 8
                dataColumns.Add("PurposeGarantee", ReportColumnType.ctDataStr); // 9
                dataColumns.Add("PrincipalEndDate", ReportColumnType.ctDataStr); // 10
                dataColumns.Add("Collateral", ReportColumnType.ctDataStr); // 11
                dataColumns.Add("Regress", ReportColumnType.ctDataStr); // 12
                dataColumns.Add("PrincipalCondition", ReportColumnType.ctDataStr); // 13
                dataColumns.Add("+15;+16;+17", ReportColumnType.ctForm); // 14
                dataColumns.Add("BgnYearDebt", ReportColumnType.ctDataNum); // 15
                dataColumns.Add("BgnInterest", ReportColumnType.ctDataNum); // 16
                dataColumns.Add("BgnPenalty", ReportColumnType.ctDataNum); // 17
                dataColumns.Add("+19;+20;+21", ReportColumnType.ctForm); // 18
                dataColumns.Add("UpDebt", ReportColumnType.ctDataNum); // 19
                dataColumns.Add("UpService", ReportColumnType.ctDataNum); // 20
                dataColumns.Add("UpPenalty", ReportColumnType.ctDataNum); // 21
                dataColumns.Add("+23;+24;+25", ReportColumnType.ctForm); // 22
                dataColumns.Add("DownDebt", ReportColumnType.ctDataNum); // 23
                dataColumns.Add("DownService", ReportColumnType.ctDataNum); // 24
                dataColumns.Add("DownPenalty", ReportColumnType.ctDataNum); // 25
                dataColumns.Add("DownPrincipal", ReportColumnType.ctDataNum); // 26
                dataColumns.Add("DownGarant", ReportColumnType.ctDataNum); // 27
                dataColumns.Add("+15;+16;+17;+19;+20;+21;-23;-24;-25", ReportColumnType.ctForm); // 28
                dataColumns.Add("+15;+19;-23", ReportColumnType.ctForm); // 29
                dataColumns.Add("+16;+20;-24", ReportColumnType.ctForm); // 30
                dataColumns.Add("+17;+21;-25", ReportColumnType.ctForm); // 31
                dataColumns.Add("+33;+39", ReportColumnType.ctForm); // 32
                dataColumns.Add("StalePrincipalDebt", ReportColumnType.ctDataNum); // 33
                dataColumns.Add("+39", ReportColumnType.ctForm); // 34

                // для расчета листа структуры 35, 36, 37, 38
                dataColumns.Add("BgnStaleDebt", ReportColumnType.ctDataNum);
                dataColumns.Add("BgnStaleInterest", ReportColumnType.ctDataNum);
                dataColumns.Add("BgnStaleComission", ReportColumnType.ctDataNum);
                dataColumns.Add("BgnStalePenalty", ReportColumnType.ctDataNum);
                dataColumns.Add("StaleInterest", ReportColumnType.ctDataNum);
                dataColumns.Add("StalePenalty", ReportColumnType.ctDataNum);
                dataColumns.Add("CalcDetailRegionName", ReportColumnType.ctCalc);  
            }
            else
            {
                dataColumns.Add("Counter", ReportColumnType.ctCounter); // 0
                dataColumns.Add("Occasion", ReportColumnType.ctDataStr);  // 1
                dataColumns.Add("GarantInfo", ReportColumnType.ctCalc);  // 2
                dataColumns.Add("GarantNum", ReportColumnType.ctCalc);  // 3
                dataColumns.Add("StartDate", ReportColumnType.ctDataStr); // 4
                dataColumns.Add("Sum", ReportColumnType.ctDataNum); // 5
                dataColumns.Add("Principal", ReportColumnType.ctDataStr); // 6
                dataColumns.Add("Creditor", ReportColumnType.ctDataStr); // 7
                dataColumns.Add("Purpose", ReportColumnType.ctDataStr); // 8
                dataColumns.Add("PrincipalEndDate", ReportColumnType.ctDataStr); // 9
                dataColumns.Add("Collateral", ReportColumnType.ctDataStr); // 10
                dataColumns.Add("Regress", ReportColumnType.ctDataStr); // 11
                dataColumns.Add("PrincipalCondition", ReportColumnType.ctDataStr); // 12
                dataColumns.Add("+14;+15;+16", ReportColumnType.ctForm); // 13
                dataColumns.Add("BgnYearDebt", ReportColumnType.ctDataNum); // 14
                dataColumns.Add("BgnInterest", ReportColumnType.ctDataNum); // 15
                dataColumns.Add("BgnPenalty", ReportColumnType.ctDataNum); // 16
                dataColumns.Add("+18;+19;+20;+21", ReportColumnType.ctForm); // 17
                dataColumns.Add("UpDebt", ReportColumnType.ctDataNum); // 18
                dataColumns.Add("UpService", ReportColumnType.ctDataNum); // 19
                dataColumns.Add("CommisionUp", ReportColumnType.ctDataNum); // 20
                dataColumns.Add("UpPenalty", ReportColumnType.ctDataNum); // 21
                dataColumns.Add("+23;+24;+25;+26", ReportColumnType.ctForm); // 22
                dataColumns.Add("DownDebt", ReportColumnType.ctDataNum); // 23
                dataColumns.Add("DownService", ReportColumnType.ctDataNum); // 24
                dataColumns.Add("CommisionDown", ReportColumnType.ctDataNum); // 5
                dataColumns.Add("DownPenalty", ReportColumnType.ctDataNum); // 26
                dataColumns.Add("DownGarant", ReportColumnType.ctDataNum); // 27
                dataColumns.Add("DownPrincipal", ReportColumnType.ctDataNum); // 28
                dataColumns.Add("+14;+18;-23;+15;+19;-24;+20;-25;+16;+21;-26", ReportColumnType.ctForm); // 29
                dataColumns.Add("+14;+18;-23", ReportColumnType.ctForm); // 30
                dataColumns.Add("+15;+19;-24", ReportColumnType.ctForm); // 31
                dataColumns.Add("+20;-25", ReportColumnType.ctForm); // 32
                dataColumns.Add("+16;+21;-26", ReportColumnType.ctForm); // 33
                dataColumns.Add("StalePrincipalDebt", ReportColumnType.ctDataNum); // 34
                dataColumns.Add("StaleInterest", ReportColumnType.ctDataNum); // 35
                dataColumns.Add("StaleCommision", ReportColumnType.ctDataNum); // 36
                dataColumns.Add("StalePenalty", ReportColumnType.ctDataNum); // 37

                // для расчета листа структуры 38, 39, 40, 41
                dataColumns.Add("BgnStaleDebt", ReportColumnType.ctDataNum);
                dataColumns.Add("BgnStaleInterest", ReportColumnType.ctDataNum);
                dataColumns.Add("BgnStaleComission", ReportColumnType.ctDataNum);
                dataColumns.Add("BgnStalePenalty", ReportColumnType.ctDataNum);
                dataColumns.Add("CalcDetailRegionName", ReportColumnType.ctCalc);                
            }

            return dataColumns;
        }

        // Список колонок ДК Оренбург - ЦБ
        private Dictionary<string, ReportColumnType> GetOrenburgCapitalColumnList(int formVersion)
        {
            var dataColumns = new Dictionary<string, ReportColumnType>();
            dataColumns.Add("Counter", ReportColumnType.ctCounter);
            dataColumns.Add("OfficialNumber", ReportColumnType.ctDataStr);
            dataColumns.Add("CapitalType", ReportColumnType.ctCalc);
            dataColumns.Add("Reason", ReportColumnType.ctDataStr);
            dataColumns.Add("RegEmissionDate", ReportColumnType.ctDataStr);
            dataColumns.Add("Nominal", ReportColumnType.ctDataStr);
            dataColumns.Add("FormCap", ReportColumnType.ctDataStr);
            dataColumns.Add("StartDate", ReportColumnType.ctDataStr);
            dataColumns.Add("AddDate", ReportColumnType.ctDataStr);
            dataColumns.Add("DueDate", ReportColumnType.ctDataStr);
            dataColumns.Add("GetBackDate", ReportColumnType.ctDataStr);
            dataColumns.Add("DateDischarge", ReportColumnType.ctDataStr);
            dataColumns.Add("Sum", ReportColumnType.ctDataNum);
            dataColumns.Add("CurrencySum", ReportColumnType.ctDataNum);
            dataColumns.Add("Coupon", ReportColumnType.ctDataNum);
            dataColumns.Add("GenAgent", ReportColumnType.ctDataStr);
            dataColumns.Add("CapitalCurrencySum", ReportColumnType.ctCalc);
            dataColumns.Add("Discharge", ReportColumnType.ctDataNum);
            dataColumns.Add("FactServiceSum", ReportColumnType.ctDataNum);
            dataColumns.Add("FactDiscountSum", ReportColumnType.ctDataNum);
            dataColumns.Add("FactService", ReportColumnType.ctDataNum);
            dataColumns.Add("+17;+18;+19;+20", ReportColumnType.ctForm);
            dataColumns.Add("+16;-17", ReportColumnType.ctForm);
            
            // для расчета листа структуры 23, 24, 25, 26
            dataColumns.Add("Zero1", ReportColumnType.ctCalc);
            dataColumns.Add("Zero2", ReportColumnType.ctCalc);
            dataColumns.Add("Zero3", ReportColumnType.ctCalc);
            dataColumns.Add("Zero4", ReportColumnType.ctCalc);
            dataColumns.Add("CalcDetailRegionName", ReportColumnType.ctCalc);
            return dataColumns;
        }

        // Список колонок ДК Оренбург - Иные кредиты
        private Dictionary<string, ReportColumnType> GetOrenburgCreditOtherColumnList(int formVersion)
        {
            var dataColumns = new Dictionary<string, ReportColumnType>();
            dataColumns.Add("Counter", ReportColumnType.ctCounter);
            dataColumns.Add("Occasion", ReportColumnType.ctDataStr);
            dataColumns.Add("CreditInfo", ReportColumnType.ctCalc);
            dataColumns.Add("DebtEndDate", ReportColumnType.ctDataStr);
            dataColumns.Add("Creditor", ReportColumnType.ctDataStr);
            dataColumns.Add("Purpose", ReportColumnType.ctDataStr);
            dataColumns.Add("Sum", ReportColumnType.ctDataNum);
            dataColumns.Add("+8;+9;+10", ReportColumnType.ctForm);
            dataColumns.Add("BgnYearDbt", ReportColumnType.ctDataNum);
            dataColumns.Add("BgnYearInterest", ReportColumnType.ctDataNum);
            dataColumns.Add("BgnYearPenlt", ReportColumnType.ctDataNum);
            dataColumns.Add("+12;+13;+14", ReportColumnType.ctForm);
            dataColumns.Add("Attract", ReportColumnType.ctDataNum);
            dataColumns.Add("PlanService", ReportColumnType.ctDataNum);
            dataColumns.Add("ChargePenlt", ReportColumnType.ctDataNum);
            dataColumns.Add("+16;+17;+18", ReportColumnType.ctForm);
            dataColumns.Add("Discharge", ReportColumnType.ctDataNum);
            dataColumns.Add("FactService", ReportColumnType.ctDataNum);
            dataColumns.Add("FactPenlt", ReportColumnType.ctDataNum);
            dataColumns.Add("+8;+12;-16;+9;+13;-17;+10;+14;-18", ReportColumnType.ctForm);
            dataColumns.Add("+8;+12;-16", ReportColumnType.ctForm);
            dataColumns.Add("+9;+13;-17", ReportColumnType.ctForm);
            dataColumns.Add("+10;+14;-18", ReportColumnType.ctForm);
            dataColumns.Add("StaleDebt", ReportColumnType.ctDataNum);
            dataColumns.Add("StaleInterest", ReportColumnType.ctDataNum);
            dataColumns.Add("StalePenlt", ReportColumnType.ctDataNum);
            
            // для расчета листа структуры 26, 27, 28, 29
            dataColumns.Add("StaleDebtBgn", ReportColumnType.ctDataNum);
            dataColumns.Add("StaleInterestBgn", ReportColumnType.ctDataNum);
            dataColumns.Add("Zero", ReportColumnType.ctCalc);
            dataColumns.Add("StalePenltBgn", ReportColumnType.ctDataNum);
            dataColumns.Add("+23;+24;+25", ReportColumnType.ctForm);
            dataColumns.Add("+23", ReportColumnType.ctForm);
            dataColumns.Add("CalcDetailRegionName", ReportColumnType.ctCalc);
            return dataColumns;
        }

        private DataTable FillMOStructureTable(DataTable[] tblTables, int formVersion)
        {
            if (formVersion == 1)
            {
                var tblResult = FillMOStructureTable1(tblTables, null, 0);
                return FillMOStructureTable1(tblTables, tblResult, 14);
            }
            else
            {
                var tblResult = FillMOStructureTable2(tblTables, null, 0);
                return FillMOStructureTable2(tblTables, tblResult, 14);                
            }
        }

        private DataTable FillMOStructureTable2(DataTable[] tblTables, DataTable tblDest, int offset)
        {
            var tblResult = tblDest;
            var addRows = offset == 0;

            if (addRows)
            {
                tblResult = CreateReportCaptionTable(16);
            }

            for (var i = 0; i < 5; i++)
            {
                var rowResult = addRows ? tblResult.Rows.Add() : tblResult.Rows[i];
                var currentTable = tblTables[offset + i];

                if (currentTable == null)
                {
                    continue;
                }

                var rowSourceRegion = currentTable.Rows[currentTable.Rows.Count - 1];

                var startPart1Index = 8;
                var startPart2Index = 20;
                var staleDebtIndex = 27;

                switch (i)
                {
                    case 1:
                        staleDebtIndex = 29;
                        break;
                    case 2:
                        startPart1Index = 14;
                        startPart2Index = 28;
                        staleDebtIndex = 35;
                        break;
                    case 3:
                        startPart1Index = 16;
                        startPart2Index = 22;
                        staleDebtIndex = 23;
                        break;
                    case 4:
                        startPart1Index = 7;
                        startPart2Index = 19;
                        staleDebtIndex = 26;
                        break;
                }

                if (addRows)
                {
                    for (var j = 0; j < tblResult.Columns.Count; j++)
                    {
                        rowResult[j] = 0;
                    }
                }

                rowResult[0] = GetNumber(rowResult[0]) + GetNumber(rowSourceRegion[startPart1Index + 0]);
                rowResult[7] = GetNumber(rowResult[7]) + GetNumber(rowSourceRegion[startPart2Index + 0]);

                if (i == 3)
                {
                    continue;
                }

                rowResult[01] = GetNumber(rowResult[01]) + GetNumber(rowSourceRegion[startPart1Index + 1]);
                rowResult[02] = GetNumber(rowResult[02]) + GetNumber(rowSourceRegion[startPart1Index + 2]);
                rowResult[03] = GetNumber(rowResult[03]) + GetNumber(rowSourceRegion[startPart1Index + 3]);

                rowResult[08] = GetNumber(rowResult[08]) + GetNumber(rowSourceRegion[startPart2Index + 1]);
                rowResult[09] = GetNumber(rowResult[09]) + GetNumber(rowSourceRegion[startPart2Index + 2]);
                rowResult[10] = GetNumber(rowResult[10]) + GetNumber(rowSourceRegion[startPart2Index + 3]);

                if (i != 2)
                {
                    rowResult[11] = GetNumber(rowResult[11]) + GetNumber(rowSourceRegion[startPart2Index + 4]);
                    rowResult[12] = GetNumber(rowResult[12]) + GetNumber(rowSourceRegion[startPart2Index + 5]);
                    rowResult[13] = GetNumber(rowResult[13]) + GetNumber(rowSourceRegion[startPart2Index + 6]);
                }
                else
                {
                    rowResult[11] = GetNumber(rowResult[11]) + GetNumber(rowSourceRegion[startPart2Index + 5]);
                    rowResult[12] = GetNumber(rowResult[12]) + GetNumber(rowSourceRegion[staleDebtIndex + 4]);
                    rowResult[13] = GetNumber(rowResult[13]) + GetNumber(rowSourceRegion[staleDebtIndex + 5]);
                }

                rowResult[4] = GetNumber(rowResult[4]) + GetNumber(rowSourceRegion[staleDebtIndex + 0]);
                rowResult[5] = GetNumber(rowResult[5]) + GetNumber(rowSourceRegion[staleDebtIndex + 1]);
                rowResult[6] = GetNumber(rowResult[6]) + GetNumber(rowSourceRegion[staleDebtIndex + 3]);

                // особенность гарантий - сумма по ОД = сумма процентов + сумме по основному долгу, проценты в ноль
                if (i == 2)
                {
                    rowResult[01] = GetNumber(rowResult[01]) + GetNumber(rowResult[02]);
                    rowResult[04] = GetNumber(rowResult[04]) + GetNumber(rowResult[05]);
                    rowResult[08] = GetNumber(rowResult[08]) + GetNumber(rowResult[09]);
                    rowResult[11] = GetNumber(rowResult[11]) + GetNumber(rowResult[12]);

                    rowResult[02] = 0;
                    rowResult[05] = 0;
                    rowResult[09] = 0;
                    rowResult[12] = 0;
                }
            }

            var rowSummary = addRows ? tblResult.Rows.Add() : tblResult.Rows[tblResult.Rows.Count - 1];

            for (var i = 0; i < tblResult.Columns.Count; i++)
            {
                double summ = 0;

                for (var j = 0; j < 5; j++)
                {
                    summ += GetNumber(tblResult.Rows[j][i]);
                }

                rowSummary[i] = summ;
            }

            return tblResult;
        }

        private DataTable FillMOStructureTable1(DataTable[] tblTables, DataTable tblDest, int offset)
        {
            var tblResult = tblDest;
            var addRows = offset == 0;

            if (addRows)
            {
                tblResult = CreateReportCaptionTable(16);
            }

            for (var i = 0; i < 5; i++)
            {
                var rowResult = addRows ? tblResult.Rows.Add() : tblResult.Rows[i];
                var currentTable = tblTables[offset + i];

                if (currentTable == null)
                {
                    continue;
                }

                var rowSourceRegion = currentTable.Rows[currentTable.Rows.Count - 1];

                var startPart1Index = 8;
                var startPart2Index = 20;
                var staleDebtIndex = 27;

                switch (i)
                {
                    case 1:
                        staleDebtIndex = 29;
                        break;
                    case 2:
                        startPart1Index = 13;
                        startPart2Index = 29;
                        staleDebtIndex = 38;
                        break;
                    case 3:
                        startPart1Index = 16;
                        startPart2Index = 22;
                        staleDebtIndex = 23;
                        break;
                    case 4:
                        startPart1Index = 7;
                        startPart2Index = 19;
                        staleDebtIndex = 26;
                        break;
                }

                if (addRows)
                {
                    for (var j = 0; j < tblResult.Columns.Count; j++)
                    {
                        rowResult[j] = 0;
                    }
                }

                rowResult[0] = GetNumber(rowResult[0]) + GetNumber(rowSourceRegion[startPart1Index + 0]);
                rowResult[8] = GetNumber(rowResult[8]) + GetNumber(rowSourceRegion[startPart2Index + 0]);

                if (i == 3)
                {
                    continue;
                }

                rowResult[01] = GetNumber(rowResult[01]) + GetNumber(rowSourceRegion[startPart1Index + 1]);
                rowResult[02] = GetNumber(rowResult[02]) + GetNumber(rowSourceRegion[startPart1Index + 2]);
                rowResult[03] = GetNumber(rowResult[03]) + GetNumber(rowSourceRegion[startPart1Index + 3]);

                rowResult[09] = GetNumber(rowResult[09]) + GetNumber(rowSourceRegion[startPart2Index + 1]);
                rowResult[10] = GetNumber(rowResult[10]) + GetNumber(rowSourceRegion[startPart2Index + 2]);

                if (i == 2)
                {
                    startPart2Index++;
                }

                rowResult[11] = GetNumber(rowResult[11]) + GetNumber(rowSourceRegion[startPart2Index + 3]);

                rowResult[12] = GetNumber(rowResult[12]) + GetNumber(rowSourceRegion[startPart2Index + 4]);
                rowResult[13] = GetNumber(rowResult[13]) + GetNumber(rowSourceRegion[startPart2Index + 5]);

                if (i != 2)
                {
                    rowResult[14] = 0;
                    rowResult[15] = GetNumber(rowResult[15]) + GetNumber(rowSourceRegion[startPart2Index + 6]);
                }
                else
                {
                    rowResult[14] = GetNumber(rowResult[14]) + GetNumber(rowSourceRegion[startPart2Index + 6]);
                    rowResult[15] = GetNumber(rowResult[15]) + GetNumber(rowSourceRegion[startPart2Index + 7]);
                }

                rowResult[4] = GetNumber(rowResult[4]) + GetNumber(rowSourceRegion[staleDebtIndex + 0]);
                rowResult[5] = GetNumber(rowResult[5]) + GetNumber(rowSourceRegion[staleDebtIndex + 1]);
                rowResult[6] = GetNumber(rowResult[6]) + GetNumber(rowSourceRegion[staleDebtIndex + 2]);
                rowResult[7] = GetNumber(rowResult[7]) + GetNumber(rowSourceRegion[staleDebtIndex + 3]);

                // особенность гарантий - сумма по ОД = сумма процентов + сумме по основному долгу, проценты в ноль
                if (i == 2)
                {
                    rowResult[01] = GetNumber(rowResult[01]) + GetNumber(rowResult[02]);
                    rowResult[04] = GetNumber(rowResult[04]) + GetNumber(rowResult[05]);
                    rowResult[09] = GetNumber(rowResult[09]) + GetNumber(rowResult[10]);
                    rowResult[12] = GetNumber(rowResult[12]) + GetNumber(rowResult[13]);

                    rowResult[02] = 0;
                    rowResult[05] = 0;
                    rowResult[10] = 0;
                    rowResult[13] = 0;
                }
            }

            var rowSummary = addRows ? tblResult.Rows.Add() : tblResult.Rows[tblResult.Rows.Count - 1];

            for (var i = 0; i < tblResult.Columns.Count; i++)
            {
                double summ = 0;

                for (var j = 0; j < 5; j++)
                {
                    summ += GetNumber(tblResult.Rows[j][i]);
                }

                rowSummary[i] = summ;
            }

            return tblResult;
        }

        // Список колонок ДК Оренбург - Кредиты субъект
        private Dictionary<string, ReportColumnType> GetOrenburgSubjectColumnListCredit(int formVersion)
        {
            var dataColumns = new Dictionary<string, ReportColumnType>();
            dataColumns.Add("CalcRegionName", ReportColumnType.ctCalc); // 0
            dataColumns.Add("+2;+3;+4", ReportColumnType.ctForm); // 1
            dataColumns.Add("BgnYearDbt", ReportColumnType.ctDataNum); // 2
            dataColumns.Add("BgnYearInterest", ReportColumnType.ctDataNum); // 3
            dataColumns.Add("BgnYearPenlt", ReportColumnType.ctDataNum); // 4
            dataColumns.Add("+6;+7;+8", ReportColumnType.ctForm); // 5
            dataColumns.Add("Attract", ReportColumnType.ctDataNum); // 6
            dataColumns.Add("PlanService", ReportColumnType.ctDataNum); // 7
            dataColumns.Add("ChargePenlt", ReportColumnType.ctDataNum); // 8
            dataColumns.Add("+10;+11;+12", ReportColumnType.ctForm); // 9
            dataColumns.Add("Discharge", ReportColumnType.ctDataNum); // 10
            dataColumns.Add("FactService", ReportColumnType.ctDataNum); // 11
            dataColumns.Add("FactPenlt", ReportColumnType.ctDataNum); // 12
            dataColumns.Add("+2;+6;-10;+3;+7;-11;+4;+8;-12", ReportColumnType.ctForm); // 13
            dataColumns.Add("+2;+6;-10", ReportColumnType.ctForm); // 14
            dataColumns.Add("+3;+7;-11", ReportColumnType.ctForm); // 15
            dataColumns.Add("+4;+8;-12", ReportColumnType.ctForm); // 16
            dataColumns.Add("StaleDebt", ReportColumnType.ctDataNum); // 17
            dataColumns.Add("StaleInterest", ReportColumnType.ctDataNum); // 18
            dataColumns.Add("StalePenlt", ReportColumnType.ctDataNum); // 19
            dataColumns.Add("DiffrncRatesDbt", ReportColumnType.ctDataNum); // 20
            dataColumns.Add("DiffrncRatesInterest", ReportColumnType.ctDataNum); // 21
            dataColumns.Add("RefRegion", ReportColumnType.ctDataStr); // 22
            return dataColumns;
        }

        // Список колонок ДК Оренбург - Гарантии субъект
        private Dictionary<string, ReportColumnType> GetOrenburgSubjectColumnListGarant(int formVersion)
        {
            var dataColumns = new Dictionary<string, ReportColumnType>();

            if (formVersion == 2)
            {
                dataColumns.Add("CalcRegionName", ReportColumnType.ctCalc); // 0
                dataColumns.Add("+2;+3;+4", ReportColumnType.ctForm); // 1
                dataColumns.Add("BgnYearDebt", ReportColumnType.ctDataNum); // 2
                dataColumns.Add("BgnInterest", ReportColumnType.ctDataNum); // 3
                dataColumns.Add("BgnPenalty", ReportColumnType.ctDataNum); // 4
                dataColumns.Add("+6;+7;+8", ReportColumnType.ctForm); // 5
                dataColumns.Add("UpDebt", ReportColumnType.ctDataNum); // 6
                dataColumns.Add("UpService", ReportColumnType.ctDataNum); // 7
                dataColumns.Add("UpPenalty", ReportColumnType.ctDataNum); // 8
                dataColumns.Add("+10;+11;+12", ReportColumnType.ctForm); // 9
                dataColumns.Add("DownDebt", ReportColumnType.ctDataNum); // 10
                dataColumns.Add("DownService", ReportColumnType.ctDataNum); // 11
                dataColumns.Add("DownPenalty", ReportColumnType.ctDataNum); // 12
                dataColumns.Add("DownPrincipal", ReportColumnType.ctDataNum); // 13
                dataColumns.Add("DownGarant", ReportColumnType.ctDataNum); // 14
                dataColumns.Add("+2;+3;+4;+6;+7;+8;-10;-11;-12", ReportColumnType.ctForm); // 15
                dataColumns.Add("+2;+6;-10", ReportColumnType.ctForm); // 16
                dataColumns.Add("+3;+7;-11", ReportColumnType.ctForm); // 17
                dataColumns.Add("+4;+8;-12", ReportColumnType.ctForm); // 18
                dataColumns.Add("+20;+21", ReportColumnType.ctForm); // 19
                dataColumns.Add("StalePrincipalDebt", ReportColumnType.ctDataNum); // 20
                dataColumns.Add("StaleInterest", ReportColumnType.ctDataNum); // 21
                dataColumns.Add("RefRegion", ReportColumnType.ctDataStr); // 22  
            }
            else
            {
                dataColumns.Add("CalcRegionName", ReportColumnType.ctCalc); // 0
                dataColumns.Add("+2;+3;+4", ReportColumnType.ctForm); // 1
                dataColumns.Add("BgnYearDebt", ReportColumnType.ctDataNum); // 2
                dataColumns.Add("BgnInterest", ReportColumnType.ctDataNum); // 3
                dataColumns.Add("BgnPenalty", ReportColumnType.ctDataNum); // 4
                dataColumns.Add("+6;+7;+8;+9", ReportColumnType.ctForm); // 5
                dataColumns.Add("UpDebt", ReportColumnType.ctDataNum); // 6
                dataColumns.Add("UpService", ReportColumnType.ctDataNum); // 7
                dataColumns.Add("CommisionUp", ReportColumnType.ctDataNum); // 8
                dataColumns.Add("UpPenalty", ReportColumnType.ctDataNum); // 9
                dataColumns.Add("+11;+12;+13;+14", ReportColumnType.ctForm); // 10
                dataColumns.Add("DownDebt", ReportColumnType.ctDataNum); // 11
                dataColumns.Add("DownService", ReportColumnType.ctDataNum); // 12
                dataColumns.Add("CommisionDown", ReportColumnType.ctDataNum); // 13
                dataColumns.Add("DownPenalty", ReportColumnType.ctDataNum); // 14
                dataColumns.Add("DownGarant", ReportColumnType.ctDataNum); // 15
                dataColumns.Add("DownPrincipal", ReportColumnType.ctDataNum); // 16
                dataColumns.Add("+2;+6;-11;+3;+7;-12;+8;-13;+4;+9;-14", ReportColumnType.ctForm); // 17
                dataColumns.Add("+2;+6;-11", ReportColumnType.ctForm); // 18
                dataColumns.Add("+3;+7;-12", ReportColumnType.ctForm); // 19
                dataColumns.Add("+8;-13", ReportColumnType.ctForm); // 20
                dataColumns.Add("+4;+9;-14", ReportColumnType.ctForm); // 21
                dataColumns.Add("StalePrincipalDebt", ReportColumnType.ctDataNum); // 22
                dataColumns.Add("StaleInterest", ReportColumnType.ctDataNum); // 23
                dataColumns.Add("StaleCommision", ReportColumnType.ctDataNum); // 24
                dataColumns.Add("StalePenalty", ReportColumnType.ctDataNum);  // 25    
                dataColumns.Add("RefRegion", ReportColumnType.ctDataStr); // 26                  
            }
            
            return dataColumns;
        }

        // Список колонок ДК Оренбург - ЦБ субъект
        private Dictionary<string, ReportColumnType> GetOrenburgSubjectColumnListCapital(int formVersion)
        {
            var dataColumns = new Dictionary<string, ReportColumnType>();
            dataColumns.Add("CalcRegionName", ReportColumnType.ctCalc); // 0
            dataColumns.Add("CapitalCurrencySum", ReportColumnType.ctCalc); // 1
            dataColumns.Add("Discharge", ReportColumnType.ctDataNum); // 2
            dataColumns.Add("FactServiceSum", ReportColumnType.ctDataNum); // 3
            dataColumns.Add("FactDiscountSum", ReportColumnType.ctDataNum); // 4
            dataColumns.Add("FactService", ReportColumnType.ctDataNum); // 5
            dataColumns.Add("+2;+3;+4;+5", ReportColumnType.ctForm); // 6
            dataColumns.Add("+1;-2", ReportColumnType.ctForm); // 7
            dataColumns.Add("RefRegion", ReportColumnType.ctDataStr); // 8
            return dataColumns;
        }

        private DataTable AddRegionRow(DataTable tbl, int code, DataTable tblSource)
        {
            for (var i = 0; i < tblSource.Rows.Count; i++)
            {
                var rowAdd = tbl.Rows.Add();
                rowAdd[0] = code;
                rowAdd[1] = tblSource.Rows[i]["Name"];
                rowAdd[2] = tblSource.Rows[i]["ID"];
            }

            return tbl;
        }

        // создаем структурку для вывода данных ГО-МО-Строка итогов
        private DataTable CreateSettleList(
            DataTable tblSource, 
            DataTable tblTown, 
            DataTable tblVillage, 
            DataTable tblTownSettle, 
            DataTable tblSeloSettle)
        {
            var tblResult = CreateReportCaptionTable(tblSource.Columns.Count + 2);
            tblResult = AddRegionRow(tblResult, 1, tblTown);
            tblResult = AddRegionRow(tblResult, 2, tblVillage);
            tblResult = AddRegionRow(tblResult, 3, tblTownSettle);
            tblResult = AddRegionRow(tblResult, 4, tblSeloSettle);
            return tblResult;
        }

        private DataTable GroupSubjectDataOrenburg(
            DataTable tblSource, 
            Dictionary<string, List<int>> dictRegion,
            DataTable tblResult)
        {
            var regionList = new Dictionary<string, int>();

            for (var i = 0; i < tblSource.Rows.Count - 1; i++)
            {
                var regionName = tblSource.Rows[i][0].ToString();
                var regionKey = tblSource.Rows[i]["RefRegion"].ToString();
                var rowFind = FindDataRow(tblResult, regionKey, tblResult.Columns[2].ColumnName);
                var regionId = Convert.ToInt32(regionKey);

                if (regionName.Length > 0)
                {
                    var isExist = regionList.ContainsKey(regionName);

                    if ((isExist && regionList[regionName] != regionId) || !isExist)
                    {
                        regionList.Add(regionName, regionId);
                    }
                }

                if (rowFind == null)
                {
                    continue;
                }

                for (var j = 1; j < tblSource.Columns.Count; j++)
                {
                    rowFind[j + 2] = GetNumber(rowFind[j + 2]) + ParseCurrencyStr(tblSource.Rows[i][j]);
                }
            }

            foreach (var region in regionList)
            {
                if (dictRegion.ContainsKey(region.Key))
                {
                    continue;
                }

                if (dictRegion.Any(r => r.Value.Contains(Convert.ToInt32(region.Value))))
                {
                    var regionRecord = dictRegion.Where(r => r.Value.Contains(region.Value));
                    var columnName = tblSource.Columns[1].ColumnName;
                    var columnCode = tblSource.Columns[2].ColumnName;

                    var rowFind = FindDataRow(tblResult, Convert.ToString(regionRecord.First().Key), columnName);

                    if (rowFind == null)
                    {
                        continue;
                    }

                    var rowsSettle = tblResult.Select(String.Format("{0} = '{1}'", columnCode, region.Value));

                    foreach (var rowSettle in rowsSettle)
                    {
                        for (var j = 1; j < tblSource.Columns.Count; j++)
                        {
                            rowFind[j + 2] = GetNumber(rowFind[j + 2]) + ParseCurrencyStr(rowSettle[j + 2]);
                        }
                    }
                }
            }
            
            // строку итогов без группировки добавляем в конец
            var rowLastSummary = tblSource.Rows[tblSource.Rows.Count - 1];
            tblResult.ImportRow(rowLastSummary);
            var rowSummary = tblResult.Rows[tblResult.Rows.Count - 1];
            rowSummary[2] = 0;

            for (var i = 2; i < rowLastSummary.Table.Columns.Count; i++)
            {
                rowSummary[i + 1] = rowLastSummary[i];
            }

            return tblResult;
        }

        private double GetMonthReportData(DateTime reportDate, string regionList, int code, bool needSettles)
        {
            double result = 0;

            // сущности
            var monthEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.f_F_MonthRepInDebtBooks);
            var clsMarksEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.d_Marks_MonthRepInDebt);
            var clsMonthRegionEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.d_Regions_MonthRep);
            var clsRegionAnalisys = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.d_Regions_Analysis);
            
            // запросы
            var monthDataQuery = "select {0}, r.RefRegionsBridge from {1} d, {2} c, {3} r " +
                "where d.RefBdgtLevels in ({4}) and d.RefMarksInDebt = c.id and c.kst >= {5}0 and c.kst <= {5}9" +
                " and d.RefYearDayUNV >={6} and d.RefYearDayUNV <={7} and r.id = d.RefRegions " +
                " and r.RefRegionsBridge in ({8}) order by d.RefYearDayUNV desc";
            const string RegionAnalisysDataQuery = "select RefBridgeRegions, id from {0} where id in ({1})";
            var monthMaxDataQuery = String.Format("select max(RefYearDayUNV) from {0} d", monthEntity.FullDBName);
            
            // уровни бюджета
            var levelCodes = "4, 5"; 
           
            if (needSettles)
            {
                levelCodes = levelCodes + ", 6";
            }

            using (var db = scheme.SchemeDWH.DB)
            {
                var maxDateUnv = Convert.ToString(db.ExecQuery(monthMaxDataQuery, QueryResultTypes.Scalar));
                var maxDay = Math.Max(1, Convert.ToInt32(maxDateUnv.Substring(6, 2)));
                var maxMonth = Math.Max(1, Convert.ToInt32(maxDateUnv.Substring(4, 2)));
                var maxYear = Convert.ToInt32(maxDateUnv.Substring(0, 4));
                var maxDate = new DateTime(maxYear, maxMonth, maxDay);

                if (DateTime.Compare(maxDate, reportDate) < 0)
                {
                    reportDate = maxDate;
                }

                var tblRegionAnalData = (DataTable)db.ExecQuery(
                    String.Format(RegionAnalisysDataQuery, clsRegionAnalisys.FullDBName, regionList),
                    QueryResultTypes.DataTable);

                var regionBridgeCodes = tblRegionAnalData.Rows.Cast<DataRow>()
                    .Aggregate(String.Empty, (current, rowRegion) => String.Format("{0},{1}", current, rowRegion["RefBridgeRegions"]));

                monthDataQuery = String.Format(
                    monthDataQuery, 
                    GetFieldNames(monthEntity, "d"), 
                    monthEntity.FullDBName,
                    clsMarksEntity.FullDBName, 
                    clsMonthRegionEntity.FullDBName, 
                    levelCodes,
                    code, 
                    GetUNVMonthBound(reportDate, true), 
                    GetUNVMonthBound(reportDate, false),
                    regionBridgeCodes.Trim(','));

                var tblMonthData = (DataTable)db.ExecQuery(monthDataQuery, QueryResultTypes.DataTable);
                var regionIDs = regionList.Split(',');

                foreach (var t in regionIDs)
                {
                    var rowsBridge = tblRegionAnalData.Select(String.Format("id = {0}", t));

                    if (rowsBridge.Length <= 0)
                    {
                        continue;
                    }

                    var regionBridge = Convert.ToInt32(rowsBridge[0]["RefBridgeRegions"]);

                    if (regionBridge == -1)
                    {
                        continue;
                    }

                    var rowsMonthData = tblMonthData.Select(String.Format("RefRegionsBridge = {0}", regionBridge));
                    
                    if (rowsMonthData.Length <= 0)
                    {
                        continue;
                    }

                    result = rowsMonthData.Sum(rowMonthData => GetNumber(rowMonthData["Fact"]));
                }
            }

            return result;
        }

        private DataTable GetLimitsData(int refVariant, string refRegion)
        {
            const string LimitDataQuery = "select {0} from {1} where RefRegion in ({2}) and RefVariant = {3}";
            var limitEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.f_S_SchBLimit);

            using (var db = scheme.SchemeDWH.DB)
            {
                var tblData = (DataTable)db.ExecQuery(
                    String.Format(
                        LimitDataQuery, 
                        GetFieldNames(limitEntity, String.Empty), 
                        limitEntity.FullDBName, 
                        refRegion, 
                        refVariant), 
                    QueryResultTypes.DataTable);

                var sum = new double[tblData.Columns.Count];

                foreach (DataRow dr in tblData.Rows)
                {
                    for (var i = 0; i < tblData.Columns.Count; i++)
                    {
                        sum[i] += GetNumber(dr[i]);
                    }
                }

                var tblResult = new DataTable();

                for (var i = 0; i < tblData.Columns.Count; i++)
                {
                    tblResult.Columns.Add(tblData.Columns[i].ColumnName, typeof(double));
                }

                var rowResult = tblResult.Rows.Add();

                for (var i = 0; i < tblData.Columns.Count; i++)
                {
                    rowResult[i] = sum[i];
                }

                return tblResult;
            }
        }

        private DataTable GetNotesData(int refVariant, string refRegion)
        {
            const string QueryNotes = "select note from {0} where RefRegion in ({1}) and RefVariant = {2}";
            var noteEntity = scheme.RootPackage.FindEntityByName("f6b4929b-8c0e-4bae-8d8a-b94e56c69aa9");

            using (var db = scheme.SchemeDWH.DB)
            {
                return (DataTable)db.ExecQuery(
                    String.Format(QueryNotes, noteEntity.FullDBName, refRegion, refVariant),
                    QueryResultTypes.DataTable);
            }
        }

        // Список колонок ДК Оренбург - Кредиты субъект
        private Dictionary<string, ReportColumnType> GetOrenburgStaleListCredit()
        {
            var dataColumns = new Dictionary<string, ReportColumnType>();
            dataColumns.Add("CalcRegionName", ReportColumnType.ctCalc); // 0
            dataColumns.Add("RefRegion", ReportColumnType.ctDataStr); // 1
            dataColumns.Add("+4;+5;+6", ReportColumnType.ctForm); // 2
            dataColumns.Add("+7;+8;+9", ReportColumnType.ctForm); // 3
            // служебные
            dataColumns.Add("StaleDebtBgn", ReportColumnType.ctDataNum); // 4
            dataColumns.Add("StaleInterestBgn", ReportColumnType.ctDataNum); // 5
            dataColumns.Add("StalePenltBgn", ReportColumnType.ctDataNum); // 6
            dataColumns.Add("StaleDebt", ReportColumnType.ctDataNum); // 7
            dataColumns.Add("StaleInterest", ReportColumnType.ctDataNum); // 8
            dataColumns.Add("StalePenlt", ReportColumnType.ctDataNum); // 9
            return dataColumns;
        }

        // Список колонок ДК Оренбург - Гарантии субъект
        private Dictionary<string, ReportColumnType> GetOrenburgStaleListGarant()
        {
            var dataColumns = new Dictionary<string, ReportColumnType>();
            dataColumns.Add("CalcRegionName", ReportColumnType.ctCalc); // 0
            dataColumns.Add("RefRegion", ReportColumnType.ctDataStr); // 1  
            dataColumns.Add("+4;+5", ReportColumnType.ctForm); // 2
            dataColumns.Add("+7;+8", ReportColumnType.ctForm); // 3
            // служебные
            dataColumns.Add("BgnStaleDebt", ReportColumnType.ctDataNum); // 4
            dataColumns.Add("BgnStaleInterest", ReportColumnType.ctDataNum); // 5
            dataColumns.Add("BgnStalePenalty", ReportColumnType.ctDataNum); // 6
            dataColumns.Add("StalePrincipalDebt", ReportColumnType.ctDataNum); // 7
            dataColumns.Add("StaleInterest", ReportColumnType.ctDataNum); // 8
            dataColumns.Add("Zero1", ReportColumnType.ctCalc); // 9
            return dataColumns;
        }

        // Список колонок ДК Оренбург - ЦБ субъект
        private Dictionary<string, ReportColumnType> GetOrenburgStaleListCapital()
        {
            var dataColumns = new Dictionary<string, ReportColumnType>();
            dataColumns.Add("CalcRegionName", ReportColumnType.ctCalc); // 0
            dataColumns.Add("RefRegion", ReportColumnType.ctDataStr); // 1
            dataColumns.Add("Zero1", ReportColumnType.ctCalc); // 2
            dataColumns.Add("Zero2", ReportColumnType.ctCalc); // 3
            // служебные
            dataColumns.Add("Zero3", ReportColumnType.ctCalc); // 4
            dataColumns.Add("Zero4", ReportColumnType.ctCalc); // 5
            dataColumns.Add("Zero5", ReportColumnType.ctCalc); // 6
            dataColumns.Add("Zero6", ReportColumnType.ctCalc); // 7
            dataColumns.Add("Zero7", ReportColumnType.ctCalc); // 8
            dataColumns.Add("Zero8", ReportColumnType.ctCalc); // 9
            return dataColumns;
        }

        private void AddStaleData(DataRow rowSrc, DataRow rowDst, int colIndex1, int colIndex2, bool isSettle = false)
        {
            const int TotalColIndex1 = 3;
            const int TotalColIndex2 = 9;
            var srcColIndex1 = isSettle ? colIndex1 : 2;
            var srcColIndex2 = isSettle ? colIndex2 : 3;
            var val1 = ParseCurrencyStr(rowSrc[srcColIndex1]);
            var val2 = ParseCurrencyStr(rowSrc[srcColIndex2]);
            rowDst[colIndex1] = GetNumber(rowDst[colIndex1]) + val1;
            rowDst[colIndex2] = GetNumber(rowDst[colIndex2]) + val2;
            rowDst[TotalColIndex1] = GetNumber(rowDst[TotalColIndex1]) + val1;
            rowDst[TotalColIndex2] = GetNumber(rowDst[TotalColIndex2]) + val2;
        }

        private DataTable GroupStaleDataOrenburg(
            DataTable tblSource,
            Dictionary<string, List<int>> dictRegion,
            DataTable tblResult,
            int columnIndex)
        {
            var regionList = new Dictionary<string, int>();
            var colIndex1 = 04 + columnIndex;
            var colIndex2 = 10 + columnIndex;

            for (var i = 0; i < tblSource.Rows.Count - 1; i++)
            {
                var rowSrc = tblSource.Rows[i];
                var regionName = Convert.ToString(rowSrc[0]);
                var regionKey = Convert.ToString(rowSrc[1]);
                var rowFind = FindDataRow(tblResult, regionKey, tblResult.Columns[2].ColumnName);
                var regionId = Convert.ToInt32(regionKey);

                if (regionName.Length > 0)
                {
                    var isExist = regionList.ContainsKey(regionName);

                    if ((isExist && regionList[regionName] != regionId) || !isExist)
                    {
                        regionList.Add(regionName, regionId);
                    }
                }

                if (rowFind == null)
                {
                    continue;
                }

                AddStaleData(rowSrc, rowFind, colIndex1, colIndex2);
            }

            foreach (var region in regionList)
            {
                if (dictRegion.ContainsKey(region.Key))
                {
                    continue;
                }

                if (dictRegion.Any(r => r.Value.Contains(Convert.ToInt32(region.Value))))
                {
                    var regionRecord = dictRegion.Where(r => r.Value.Contains(region.Value));
                    var columnName = tblResult.Columns[1].ColumnName;
                    var columnCode = tblResult.Columns[2].ColumnName;

                    var rowFind = FindDataRow(tblResult, Convert.ToString(regionRecord.First().Key), columnName);

                    if (rowFind == null)
                    {
                        continue;
                    }

                    var rowsSettle = tblResult.Select(String.Format("{0} = '{1}'", columnCode, region.Value));

                    foreach (var rowSettle in rowsSettle)
                    {
                        AddStaleData(rowSettle, rowFind, colIndex1, colIndex2, true);
                    }
                }
            }

            // строку итогов без группировки добавляем в конец
            var rowLastSummary = tblSource.Rows[tblSource.Rows.Count - 1];

            if (columnIndex == 0)
            {
                tblResult.Rows.Add();
            }

            var rowSummary = tblResult.Rows[tblResult.Rows.Count - 1];
            rowSummary[2] = 0;
            AddStaleData(rowLastSummary, rowSummary, colIndex1, colIndex2);
            return tblResult;
        }
    }
}
