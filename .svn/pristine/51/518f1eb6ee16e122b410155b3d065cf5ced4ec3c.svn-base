using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Domain.Services.FinSourceDebtorBook.Reports
{
    /// <summary>
    /// Получение данных - Саратов
    /// </summary>
    public partial class ReportsDataService
    {
        public void GetDebtBookSaratovData(int refVariant, int refRegion, ref DataTable[] tables, DateTime calculateDate)
        {
            // Основные таблицы
            tables = new DataTable[11];
            const string ContactQuery = "select {0} from {1} where RefVariant = {2} and RefRegion in ({3}) {4}";
            var dataEntityList = DKSaratovEntityList();
            var dataEntityFilters = DKSaratovEntityFiltersList(calculateDate);
            var dataDateStartList = DKSaratovDateStartList();

            var rgnEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.d_Regions_Analysis);
            var isSubject = true;
            var regionName = String.Empty;

            using (var db = scheme.SchemeDWH.DB)
            {
                var listMO = GetRegionSettles(db, rgnEntity, refRegion, ref regionName, 3);
                
                if (listMO == String.Empty)
                {
                    listMO = refRegion.ToString();
                    isSubject = false;
                }

                // данные по одному мо
                for (var i = 0; i < dataEntityList.Count; i++)
                {
                    var dataColumns = new Dictionary<string, ReportColumnType>();

                    var startDate = dataDateStartList[i];
                    var debtRestColumns = new int[2];

                    switch (i)
                    {
                        case 0:
                            dataColumns = GetSaratovCapitalColumnList();
                            debtRestColumns[0] = 04;
                            debtRestColumns[1] = 13;
                            break;
                        case 1:
                            dataColumns = GetSaratovBudCreditColumnList();
                            debtRestColumns[0] = 11;
                            debtRestColumns[1] = 17;
                            break;
                        case 2:
                            dataColumns = GetSaratovOrgCreditColumnList();
                            debtRestColumns[0] = 08;
                            debtRestColumns[1] = 18;
                            break;
                        case 3:
                            dataColumns = GetSaratovGarantColumnList();
                            debtRestColumns[0] = 08;
                            debtRestColumns[1] = 21;
                            break;
                        case 4:
                            dataColumns = GetSaratovCreditOtherColumnList();
                            debtRestColumns[0] = 10;
                            debtRestColumns[1] = 20;
                            break;
                    }

                    // выбираем данные из мастер таблицы
                    var dataEntity = scheme.RootPackage.FindEntityByName(dataEntityList[i]);
                    var tblContracts = (DataTable)db.ExecQuery(
                        String.Format(ContactQuery, GetFieldNames(dataEntity, String.Empty), dataEntity.FullDBName, refVariant, listMO, dataEntityFilters[i]),
                        QueryResultTypes.DataTable);

                    // преобразуем данные мастер таблицы согласно списку нужных колонок
                    tables[i] = FillTableData(SortDataSet(tblContracts, String.Format("{0} asc", startDate)), dataColumns);
                    tables[i] = CheckCloseCondition(tables[i], calculateDate, debtRestColumns);

                    if (i == 1)
                    {
                        foreach (DataRow row in tables[i].Rows)
                        {
                            var endDateStr = GetDateValue(row[4]);

                            if (String.IsNullOrEmpty(endDateStr))
                            {
                                continue;
                            }

                            if (DateTime.Compare(calculateDate, Convert.ToDateTime(endDateStr)) > 0)
                            {
                                row[10] = 0;
                            }
                        }
                    }

                    var columnIndex = 0;
                    foreach (var fieldRecord in dataColumns)
                    {
                        if (fieldRecord.Value == ReportColumnType.ctDataNum 
                            || fieldRecord.Value == ReportColumnType.ctForm
                            || fieldRecord.Value == ReportColumnType.ctDetailSum)
                        {
                            tables[i] = RecalcSummaryColumn(tables[i], columnIndex);
                        }

                        columnIndex++;
                    }
                }

                var allRegionKeys = listMO;
                var settleKeys = GetSettlesHierarchyKeys(listMO);
                var tblVillage = GetRegionByType(listMO, 4);
                var tblTown = GetRegionByType(listMO, 7);
                var tblStructure = CreateStructureSaratovList(tblTown, tblVillage);

                DataTable tblStructurePos = null;
                if (settleKeys.Length != 0)
                {
                    allRegionKeys = String.Format("{0},{1}", allRegionKeys, settleKeys);
                    tblVillage = GetRegionByType(settleKeys, 5);
                    tblTown = GetRegionByType(settleKeys, 6);
                    tblStructurePos = CreateStructureSaratovList(tblTown, tblVillage);
                }

                var convertColumnNumber = new Dictionary<int, int> { { 1, 3 }, { 2, 0 }, { 3, 2 }, { 4, 1 } };

                // данные по всем мо
                for (var i = 1; i < dataEntityList.Count; i++)
                {
                    var dataColumns = new Dictionary<string, ReportColumnType>();

                    // настраиваем списки колонок
                    if (i == 1 || i == 2 || i == 4)
                    {
                        dataColumns = GetSaratovSubjectColumnListCredit();
                    }

                    if (i == 3)
                    {
                        dataColumns = GetSaratovSubjectColumnListGarant();
                    }

                    // выбираем данные из мастер таблицы
                    var dataEntity = scheme.RootPackage.FindEntityByName(dataEntityList[i]);
                    var tblContracts = (DataTable)db.ExecQuery(
                        String.Format(
                            ContactQuery,
                            GetFieldNames(dataEntity, String.Empty),
                            dataEntity.FullDBName,
                            refVariant,
                            allRegionKeys,
                            dataEntityFilters[i]),
                        QueryResultTypes.DataTable);

                    // преобразуем данные мастер таблицы согласно списку нужных колонок
                    var fullData = FillTableData(tblContracts, dataColumns);
                    tblStructure = GroupSubjectDataSaratov(fullData, tblStructure, convertColumnNumber[i]);
                    
                    if (settleKeys.Length != 0)
                    {
                        tblStructurePos = GroupSubjectDataSaratov(fullData, tblStructurePos, convertColumnNumber[i]);
                    }
                }

                for (var i = 0; i < 5; i++)
                {
                    tables[i] = GroupByRegion(tables[i]);
                }

                tables[9] = tblStructurePos;
                tables[8] = tblStructure;
                tables[5] = GetStructureTableSaratov(scheme, refVariant, listMO);
                tables[6] = GetControlSumSaratov(scheme, refVariant, listMO);
                tables[7] = GetServiceSumSaratov(scheme, refVariant, listMO);

                // заголовочная таблица
                var tblCaptionData = CreateReportCaptionTable(17);
                var rowCaption = tblCaptionData.Rows.Add();
                rowCaption[0] = calculateDate.Year;
                rowCaption[1] = String.Format("01.01.{0}", calculateDate.Year);
                rowCaption[2] = calculateDate.Month;
                rowCaption[3] = calculateDate.ToShortDateString();
                rowCaption[4] = GetBookValue(scheme, DomainObjectsKeys.d_Regions_Analysis, refRegion.ToString());
                rowCaption[5] = GetMonthText(calculateDate.Month);
                rowCaption[6] = isSubject;
                var fullTerritoryList = String.Format("{0},{1}", listMO, GetSettlesKeys(listMO)).Trim(',');
                var kstCodes = new Collection<int> { 1074, 1072, 1076, 1071, 1075 };
                
                for (var i = 0; i < kstCodes.Count; i++)
                {
                    rowCaption[7 + i] = GetMonthReportData(calculateDate, fullTerritoryList, kstCodes[i], true);
                }

                if (isSubject)
                {
                    rowCaption[4] = "МУНИЦИПАЛЬНЫХ ОБРАЗОВАНИЙ САРАТОВСКОЙ ОБЛАСТИ";
                }

                tables[tables.Length - 1] = tblCaptionData;
            }
        }

        private DataTable RecalcSummaryColumn(DataTable tblData, int columnindex)
        {
            if (tblData.Rows.Count > 0)
            {
                double summary = 0;

                for (var i = 0; i < tblData.Rows.Count - 1; i++)
                {
                    summary += GetNumber(tblData.Rows[i][columnindex]);
                }

                tblData.Rows[tblData.Rows.Count - 1][columnindex] = summary;
            }

            return tblData;
        }

        private DataTable CheckCloseCondition(DataTable tblData, DateTime calcDate, IList<int> columnList)
        {
            var rowsEmpty = new Collection<DataRow>();

            for (var i = 0; i < tblData.Rows.Count - 1; i++)
            {
                var rowData = tblData.Rows[i];
                var dateColumnIndex = columnList[0];
                
                // дата погашения непуста, иначе считается действующим
                if (rowData.IsNull(dateColumnIndex))
                {
                    continue;
                }

                var dataColumnIndex = columnList[1];
                var staleColumnIndex = dataColumnIndex + 1;
                var sumTotal = GetNumber(rowData[dataColumnIndex]);
                DateTime dateClose;

                if (DateTime.TryParse(Convert.ToString(rowData[dateColumnIndex]), out dateClose))
                {
                    var isEmptyContract = Math.Abs(sumTotal) < 0.001;
                    rowData[staleColumnIndex] = DBNull.Value;

                    // скрываем лишние просрочки по погашенным
                    if (DateTime.Compare(calcDate, dateClose) > 0)
                    {
                        rowData[staleColumnIndex] = isEmptyContract ? (object)null : sumTotal;
                    }

                    // если закрыт, то выводим в любом случае до конца года
                    if (isEmptyContract && dateClose.Year < calcDate.Year)
                    {
                        rowsEmpty.Add(rowData);
                    }
                }
            }

            foreach (var dataRow in rowsEmpty)
            {
                tblData.Rows.Remove(dataRow);
            }

            return tblData;
        }

        private DataTable GroupByRegion(DataTable table)
        {
            var tableResult = table.Clone();
            var regionIDs = new Collection<int>();
            var lastColumnIndex = table.Columns.Count - 1;

            foreach (DataRow row in table.Rows)
            {
                if (row[lastColumnIndex - 1] == DBNull.Value)
                {
                    continue;
                }

                var refRegion = Convert.ToInt32(row[lastColumnIndex - 1]);
                
                if (!regionIDs.Contains(refRegion))
                {
                    regionIDs.Add(refRegion);
                }
            }

            // вставляем заголовочные строки и сроки договоров по каждому району
            foreach (var refRegion in regionIDs)
            {
                var tableRegion = FilterDataSet(
                    table, 
                    String.Format("{0} = {1}", table.Columns[lastColumnIndex - 1].ColumnName, refRegion));

                if (tableRegion.Rows.Count > 0)
                {
                    var captionRow = tableResult.Rows.Add();
                    captionRow[1] = tableRegion.Rows[0][lastColumnIndex];
                    var contractCounter = 1;

                    foreach (DataRow rowRegion in tableRegion.Rows)
                    {
                        rowRegion[0] = contractCounter++;
                        tableResult.ImportRow(rowRegion);
                    }
                }
            }

            // вставим итоговую строчку
            tableResult.ImportRow(table.Rows[table.Rows.Count - 1]);

            return tableResult;
        }

        // Список колонок ДК Саратов - ЦБ
        private Dictionary<string, ReportColumnType> GetSaratovCapitalColumnList()
        {
            var dataColumns = new Dictionary<string, ReportColumnType>();
            dataColumns.Add("Counter", ReportColumnType.ctCounter); // 0 
            dataColumns.Add("CapitalType", ReportColumnType.ctCalc); // 1
            dataColumns.Add("RegNumber", ReportColumnType.ctDataStr); // 2
            dataColumns.Add("Sum", ReportColumnType.ctDataNum); // 3
            dataColumns.Add("DateDischarge", ReportColumnType.ctDataStr); // 4
            dataColumns.Add("BgnYearDbt", ReportColumnType.ctDataNum); // 5
            dataColumns.Add("BgnYearInterest", ReportColumnType.ctDataNum); // 6
            dataColumns.Add("DebtStartDate", ReportColumnType.ctDataStr); // 7
            dataColumns.Add("IssueSum", ReportColumnType.ctDataNum); // 8
            dataColumns.Add("AddDate", ReportColumnType.ctDataStr); // 9
            dataColumns.Add("ExstraIssueSum", ReportColumnType.ctDataNum); // 10
            dataColumns.Add("PaymentDate", ReportColumnType.ctDataStr); // 11
            dataColumns.Add("RemnsEndMnthDbt", ReportColumnType.ctDataNum); // 12
            dataColumns.Add("+5;+8;+10;-12", ReportColumnType.ctForm); // 13
            dataColumns.Add("+5;+8;+10;-12;+6", ReportColumnType.ctForm); // 14
            dataColumns.Add("Collateral", ReportColumnType.ctDataStr); // 15
            dataColumns.Add("FactServiceSum", ReportColumnType.ctDataNum); // 16
            dataColumns.Add("StaleInterest", ReportColumnType.ctDataNum); // 17
            dataColumns.Add("ChargeDate", ReportColumnType.ctDataStr); // 18
            dataColumns.Add("StartDate", ReportColumnType.ctDataStr); // 19

            dataColumns.Add("RefRegion", ReportColumnType.ctDataStr); // 20
            dataColumns.Add("CalcRegionName", ReportColumnType.ctCalc); // 21

            return dataColumns;
        }

        // Список колонок ДК Саратов - Кредиты бюджетов
        private Dictionary<string, ReportColumnType> GetSaratovBudCreditColumnList()
        {
            var dataColumns = new Dictionary<string, ReportColumnType>();
            dataColumns.Add("Counter", ReportColumnType.ctCounter); // 0 
            dataColumns.Add("Creditor", ReportColumnType.ctDataStr); // 1            
            dataColumns.Add("CreditNumContractDate", ReportColumnType.ctCalc); // 2
            dataColumns.Add("Purpose", ReportColumnType.ctDataStr); // 3
            dataColumns.Add("DebtEndDate", ReportColumnType.ctDataStr); // 4
            dataColumns.Add("CreditRenewalInfo", ReportColumnType.ctCalc); // 5
            dataColumns.Add("CreditPercentN3", ReportColumnType.ctCalc); // 6
            dataColumns.Add("CreditActPercentN3", ReportColumnType.ctCalc); // 7
            dataColumns.Add("Sum", ReportColumnType.ctDataNum); // 8
            dataColumns.Add("BgnYearDbt", ReportColumnType.ctDataNum); // 9
            dataColumns.Add("BgnYearInterest", ReportColumnType.ctDataNum); // 10
            dataColumns.Add("DebtStartDate", ReportColumnType.ctDetailStr); // 11
            dataColumns.Add("Attract", ReportColumnType.ctDetailSum); // 12
            dataColumns.Add("PaymentDate", ReportColumnType.ctDetailStr); // 13
            dataColumns.Add("Discharge", ReportColumnType.ctDetailSum); // 14
            dataColumns.Add("OffsetDate", ReportColumnType.ctDetailStr); // 15
            dataColumns.Add("Offset", ReportColumnType.ctDetailSum); // 16
            dataColumns.Add("+9;+12;-14;-16", ReportColumnType.ctForm); // 17
            dataColumns.Add("+9;+12;-14;-16;+10", ReportColumnType.ctForm); // 18
            dataColumns.Add("+20;+21", ReportColumnType.ctForm); // 19
            dataColumns.Add("ServiceDebt", ReportColumnType.ctDataNum); // 20
            dataColumns.Add("PenaltyDebt", ReportColumnType.ctDataNum); // 21
            dataColumns.Add("ChargeDate", ReportColumnType.ctDataStr); // 22
            dataColumns.Add("StartDate", ReportColumnType.ctDataStr); // 23

            dataColumns.Add("RefRegion", ReportColumnType.ctDataStr); // 24
            dataColumns.Add("CalcRegionName", ReportColumnType.ctCalc); // 25

            return dataColumns;
        }

        // Список колонок ДК Саратов - Кредиты организаций
        private Dictionary<string, ReportColumnType> GetSaratovOrgCreditColumnList()
        {
            var dataColumns = new Dictionary<string, ReportColumnType>();
            dataColumns.Add("Counter", ReportColumnType.ctCounter); // 0 
            dataColumns.Add("Creditor", ReportColumnType.ctDataStr); // 1            
            dataColumns.Add("CreditNumContractDate", ReportColumnType.ctCalc); // 2
            dataColumns.Add("Purpose", ReportColumnType.ctDataStr); // 3
            dataColumns.Add("Collateral", ReportColumnType.ctDataStr); // 4
            dataColumns.Add("CreditRegRenewalInfo", ReportColumnType.ctCalc); // 5
            dataColumns.Add("CreditPercentN3", ReportColumnType.ctCalc); // 6
            dataColumns.Add("CreditActPercentN3", ReportColumnType.ctCalc); // 7
            dataColumns.Add("DebtEndDate", ReportColumnType.ctDataStr); // 8
            dataColumns.Add("Sum", ReportColumnType.ctDataNum); // 9
            dataColumns.Add("BgnYearDbt", ReportColumnType.ctDataNum); // 10
            dataColumns.Add("BgnYearInterest", ReportColumnType.ctDataNum); // 11
            dataColumns.Add("DebtStartDate", ReportColumnType.ctDetailStr); // 12
            dataColumns.Add("Attract", ReportColumnType.ctDetailSum); // 13
            dataColumns.Add("PaymentDate", ReportColumnType.ctDetailStr); // 14
            dataColumns.Add("Discharge", ReportColumnType.ctDetailSum); // 15
            dataColumns.Add("OffsetDate", ReportColumnType.ctDetailStr); // 16
            dataColumns.Add("Offset", ReportColumnType.ctDetailSum); // 17
            dataColumns.Add("+10;+13;-15;-17", ReportColumnType.ctForm); // 18
            dataColumns.Add("+10;+13;-15;-17;+11", ReportColumnType.ctForm); // 19
            dataColumns.Add("+21;+22", ReportColumnType.ctForm); // 20
            dataColumns.Add("ServiceDebt", ReportColumnType.ctDataNum); // 21
            dataColumns.Add("PenaltyDebt", ReportColumnType.ctDataNum); // 22
            dataColumns.Add("FurtherConvention", ReportColumnType.ctDataStr); // 23
            dataColumns.Add("ChargeDate", ReportColumnType.ctDataStr); // 24
            dataColumns.Add("StartDate", ReportColumnType.ctDataStr); // 25

            dataColumns.Add("RefRegion", ReportColumnType.ctDataStr); // 26
            dataColumns.Add("CalcRegionName", ReportColumnType.ctCalc); // 27

            return dataColumns;
        }

        // Список колонок ДК Саратов - Гарантии
        private Dictionary<string, ReportColumnType> GetSaratovGarantColumnList()
        {
            var dataColumns = new Dictionary<string, ReportColumnType>();
            dataColumns.Add("Counter", ReportColumnType.ctCounter); // 0
            dataColumns.Add("Creditor", ReportColumnType.ctDataStr); // 1
            dataColumns.Add("Principal", ReportColumnType.ctDataStr); // 2
            dataColumns.Add("Occasion", ReportColumnType.ctDataStr); // 3
            dataColumns.Add("GarantInfo", ReportColumnType.ctCalc); // 4 
            dataColumns.Add("GarantNumStartDate", ReportColumnType.ctCalc); // 5
            dataColumns.Add("Collateral", ReportColumnType.ctDataStr); // 6
            dataColumns.Add("CreditRenewalInfo", ReportColumnType.ctCalc); // 7
            dataColumns.Add("OldAgreementDate", ReportColumnType.ctDataStr); // 8
            dataColumns.Add("BgnYearDebt", ReportColumnType.ctDataNum); // 9
            dataColumns.Add("StalePenalty", ReportColumnType.ctDataNum); // 10
            dataColumns.Add("StartCreditDate", ReportColumnType.ctDetailStr); // 11
            dataColumns.Add("UpDebt", ReportColumnType.ctDetailSum); // 12
            dataColumns.Add("PrincipalEndDate", ReportColumnType.ctDataStr); // 13
            dataColumns.Add("DownDebt", ReportColumnType.ctDetailSum); // 14
            dataColumns.Add("EndCreditDate", ReportColumnType.ctDataStr); // 15
            dataColumns.Add("+17;+18;+19;+20", ReportColumnType.ctForm); // 16
            dataColumns.Add("DownPrincipal", ReportColumnType.ctDetailSum); // 17
            dataColumns.Add("DownGarant", ReportColumnType.ctDetailSum); // 18
            dataColumns.Add("DownService", ReportColumnType.ctDetailSum); // 19
            dataColumns.Add("DownPenalty", ReportColumnType.ctDetailSum); // 20
            dataColumns.Add("+9;+12;-16", ReportColumnType.ctForm); // 21
            dataColumns.Add("+21;+10", ReportColumnType.ctForm); // 22
            dataColumns.Add("ChargeDate", ReportColumnType.ctDataStr); // 23
            dataColumns.Add("StartDate", ReportColumnType.ctDataStr); // 24

            dataColumns.Add("RefRegion", ReportColumnType.ctDataStr); // 25
            dataColumns.Add("CalcRegionName", ReportColumnType.ctCalc); // 26

            return dataColumns;
        }

        // Список колонок ДК Саратов - Иные
        private Dictionary<string, ReportColumnType> GetSaratovCreditOtherColumnList()
        {
            var dataColumns = new Dictionary<string, ReportColumnType>();
            dataColumns.Add("Counter", ReportColumnType.ctCounter); // 0
            dataColumns.Add("Borrower", ReportColumnType.ctDataStr); // 1
            dataColumns.Add("Creditor", ReportColumnType.ctDataStr); // 2
            dataColumns.Add("Occasion", ReportColumnType.ctDataStr); // 3
            dataColumns.Add("CreditNumContractDate", ReportColumnType.ctCalc); // 4
            dataColumns.Add("Purpose", ReportColumnType.ctDataStr); // 5
            dataColumns.Add("Collateral", ReportColumnType.ctDataStr); // 6
            dataColumns.Add("CreditRegRenewalInfo", ReportColumnType.ctCalc); // 7
            dataColumns.Add("CreditPercentN3", ReportColumnType.ctCalc); // 8
            dataColumns.Add("CreditActPercentN3", ReportColumnType.ctCalc); // 9
            dataColumns.Add("DebtEndDate", ReportColumnType.ctDataStr); // 10
            dataColumns.Add("Sum", ReportColumnType.ctDataNum); // 11
            dataColumns.Add("BgnYearDbt", ReportColumnType.ctDataNum); // 12
            dataColumns.Add("BgnYearInterest", ReportColumnType.ctDataNum); // 13
            dataColumns.Add("DebtStartDate", ReportColumnType.ctDetailStr); // 14
            dataColumns.Add("Attract", ReportColumnType.ctDetailSum); // 15
            dataColumns.Add("PaymentDate", ReportColumnType.ctDetailStr); // 16
            dataColumns.Add("Discharge", ReportColumnType.ctDetailSum); // 17            
            dataColumns.Add("OffsetDate", ReportColumnType.ctDetailStr); // 18
            dataColumns.Add("Offset", ReportColumnType.ctDetailSum); // 19
            dataColumns.Add("+12;+15;-17;-19", ReportColumnType.ctForm); // 20
            dataColumns.Add("+20;+13", ReportColumnType.ctForm); // 21
            dataColumns.Add("+23;+24", ReportColumnType.ctForm); // 22
            dataColumns.Add("ServiceDebt", ReportColumnType.ctDataNum); // 23
            dataColumns.Add("PenaltyDebt", ReportColumnType.ctDataNum); // 24
            dataColumns.Add("ChargeDate", ReportColumnType.ctDataStr); // 25
            dataColumns.Add("StartDate", ReportColumnType.ctDataStr); // 26

            dataColumns.Add("RefRegion", ReportColumnType.ctDataStr); // 27
            dataColumns.Add("CalcRegionName", ReportColumnType.ctCalc); // 28

            return dataColumns;
        }

        // Список колонок ДК Саратов - Кредиты Субъект
        private Dictionary<string, ReportColumnType> GetSaratovSubjectColumnListCredit()
        {
            var dataColumns = new Dictionary<string, ReportColumnType>();
            dataColumns.Add("CalcRegionName", ReportColumnType.ctCalc); // 0
            dataColumns.Add("BgnYearDbt", ReportColumnType.ctDataNum); // 1
            dataColumns.Add("Attract", ReportColumnType.ctDetailSum); // 2
            dataColumns.Add("Discharge", ReportColumnType.ctDetailSum); // 3
            dataColumns.Add("Offset", ReportColumnType.ctDetailSum); // 4
            dataColumns.Add("+1;+2;-3", ReportColumnType.ctForm); // 5
            return dataColumns;
        }

        // Список колонок ДК Саратов - Гарантии
        private Dictionary<string, ReportColumnType> GetSaratovSubjectColumnListGarant()
        {
            var dataColumns = new Dictionary<string, ReportColumnType>();

            dataColumns.Add("CalcRegionName", ReportColumnType.ctCalc); // 0            
            dataColumns.Add("BgnYearDebt", ReportColumnType.ctDataNum); // 1
            dataColumns.Add("UpDebt", ReportColumnType.ctDetailSum); // 2
            dataColumns.Add("+6;+7;+8;+9", ReportColumnType.ctForm); // 3
            dataColumns.Add("DownDebt", ReportColumnType.ctDetailSum); // 4
            dataColumns.Add("+1;+2;-3", ReportColumnType.ctForm); // 5
            // служебные
            dataColumns.Add("DownPrincipal", ReportColumnType.ctDetailSum); // 6
            dataColumns.Add("DownGarant", ReportColumnType.ctDetailSum); // 7
            dataColumns.Add("DownService", ReportColumnType.ctDetailSum); // 8
            dataColumns.Add("DownPenalty", ReportColumnType.ctDetailSum); // 9
            return dataColumns;
        }

        private DataTable GroupSubjectDataSaratov(DataTable tblSource, DataTable tblResult, int index)
        {
            for (int i = 0; i < tblSource.Rows.Count - 1; i++)
            {
                var rowFind = FindDataRow(tblResult, tblSource.Rows[i][0].ToString(), tblResult.Columns[1].ColumnName);
                var offset = 2 + index + 1 - 5;

                if (rowFind == null)
                {
                    continue;
                }

                for (var j = 1; j < 6; j++)
                {
                    var cellOffset = j * 5;
                    var totalOffset = cellOffset + offset;
                    rowFind[totalOffset] = GetNumber(rowFind[totalOffset]) + ParseCurrencyStr(tblSource.Rows[i][j]);
                }
            }

            return tblResult;
        }

        // создаем структурку для вывода данных ГО-МО-Строка итогов
        private DataTable CreateStructureSaratovList(DataTable tblTown, DataTable tblVillage)
        {
            var tblResult = CreateReportCaptionTable(30);
            
            for (var i = 0; i < tblTown.Rows.Count; i++)
            {
                var rowAdd = tblResult.Rows.Add();
                rowAdd[0] = i + 1;
                rowAdd[1] = tblTown.Rows[i]["Name"];
            }

            for (var i = 0; i < tblVillage.Rows.Count; i++)
            {
                var rowAdd = tblResult.Rows.Add();
                rowAdd[0] = tblTown.Rows.Count + i + 1;
                rowAdd[1] = tblVillage.Rows[i]["Name"];
            }

            return tblResult;
        }

        private Collection<string> DKSaratovEntityList()
        {
            var dataEntityList = new Collection<string>();
            
            // ЦБ
            dataEntityList.Add(DomainObjectsKeys.f_S_SchBCapital);
            
            // Кредиты бюджетов
            dataEntityList.Add(DomainObjectsKeys.f_S_SchBCreditincome);
            
            // Кредиты организаций
            dataEntityList.Add(DomainObjectsKeys.f_S_SchBCreditincome);
            
            // Гарантии
            dataEntityList.Add(DomainObjectsKeys.f_S_SchBGuarantissued);
            
            // Иные
            dataEntityList.Add(DomainObjectsKeys.f_S_SchBCreditincome);
            return dataEntityList;
        }

        private Collection<string> DKSaratovDateStartList()
        {
            var dataEntityList = new Collection<string>();
            
            // ЦБ
            dataEntityList.Add("StartDate");
            
            // Кредиты бюджетов
            dataEntityList.Add("ContractDate");
            
            // Кредиты организаций
            dataEntityList.Add("ContractDate");
            
            // Гарантии
            dataEntityList.Add("StartDate");
            
            // Иные
            dataEntityList.Add("ContractDate");
            return dataEntityList;
        }

        private Collection<string> DKSaratovDateEndList()
        {
            var dataEntityList = new Collection<string>();
            
            // ЦБ
            dataEntityList.Add("DateDischarge");

            // Кредиты бюджетов
            dataEntityList.Add("DebtEndDate");

            // Кредиты организаций
            dataEntityList.Add("DebtEndDate");

            // Гарантии
            dataEntityList.Add("OldAgreementDate");

            // Иные
            dataEntityList.Add("DebtEndDate");
            return dataEntityList;
        }

        private Collection<string> DKSaratovEntityFiltersList(DateTime calcDate)
        {
            var dataEntityFilters = new Collection<string>();
            var dateText = calcDate.ToShortDateString();

            // ЦБ
            dataEntityFilters.Add(String.Empty);
            
            // Кредиты бюджетов
            dataEntityFilters.Add(String.Format("and RefTypeCredit = 1 and ContractDate < '{0}'", dateText));
            
            // Кредиты организаций
            dataEntityFilters.Add(String.Format("and RefTypeCredit = 0 and ContractDate < '{0}'", dateText));
            
            // Гарантии
            dataEntityFilters.Add(String.Empty);
            
            // Иные
            dataEntityFilters.Add("and RefTypeCredit = 5");
            return dataEntityFilters;
        }

        // Список колонок ДК Саратов - Контроль соблюдения требований по долгу
        private Dictionary<string, ReportColumnType> GetControlColumnsListSaratov()
        {
            var dataColumns = new Dictionary<string, ReportColumnType>();        
            dataColumns.Add("StartYearlyDebt", ReportColumnType.ctDataNum); // 0
            dataColumns.Add("YearlyDebt", ReportColumnType.ctDataNum); // 1
            dataColumns.Add("FactDebt", ReportColumnType.ctDataNum); // 2
            dataColumns.Add("StartYearlyService", ReportColumnType.ctDataNum); // 3
            dataColumns.Add("YearlyService", ReportColumnType.ctDataNum); // 4
            dataColumns.Add("FactService", ReportColumnType.ctDataNum); // 5
            return dataColumns;
        }

        // Контроль соблюдения требований по долгу
        private DataTable GetControlSumSaratov(IScheme schema, int refVariant, string listMO)
        {
            const string DataQuery = "select {0} from {1} where RefVariant = {2} and RefRegion in ({3})";
            using (var db = schema.SchemeDWH.DB)
            {
                var dataColumns = GetControlColumnsListSaratov();
                
                // выбираем данные из мастер таблицы
                var dataEntity = schema.RootPackage.FindEntityByName("e53099e0-dcf6-4feb-8e3b-2f8b060a4fc9");
                var tblData = (DataTable)db.ExecQuery(
                    String.Format(
                        DataQuery, 
                        GetFieldNames2(dataEntity, String.Empty),
                        dataEntity.FullDBName, 
                        refVariant,
                        listMO), 
                    QueryResultTypes.DataTable);
                
                // преобразуем данные мастер таблицы согласно списку нужных колонок
                return FillTableData(tblData, dataColumns);
            }
        }

        // Список колонок ДК Саратов - Обслуживание долга
        private Dictionary<string, ReportColumnType> GetServiceColumnsListSaratov()
        {
            var dataColumns = new Dictionary<string, ReportColumnType>();
            dataColumns.Add("Empty1", ReportColumnType.ctCalc); // 0
            dataColumns.Add("Empty2", ReportColumnType.ctCalc); // 1
            dataColumns.Add("TotalBgnYearBud", ReportColumnType.ctDataNum); // 2
            dataColumns.Add("TotalYearBud", ReportColumnType.ctDataNum); // 3
            dataColumns.Add("+5;+6", ReportColumnType.ctForm); // 4
            dataColumns.Add("PercentYearBud", ReportColumnType.ctDataNum); // 5
            dataColumns.Add("PenaltyYearBud", ReportColumnType.ctDataNum); // 6
            return dataColumns;
        }

        // обслуживание долга
        private DataTable GetServiceSumSaratov(IScheme schema, int refVariant, string listMO)
        {
            const string DataQuery = "select {0} from {1} where RefVariant = {2} and RefRegion in ({3}) and RefKind = {4}";
            using (var db = schema.SchemeDWH.DB)
            {
                var dataColumns = GetServiceColumnsListSaratov();
                
                // выбираем данные из мастер таблицы
                var dataEntity = schema.RootPackage.FindEntityByName("493806c9-9043-4714-83e4-4131a97dcfec");

                var tblSummary = CreateReportCaptionTable(dataColumns.Count);
                for (var i = 1; i < 6; i++)
                {
                    var tblData = (DataTable)db.ExecQuery(
                        String.Format(
                            DataQuery,
                            GetFieldNames2(dataEntity, String.Empty),
                            dataEntity.FullDBName,
                            refVariant,
                            listMO,
                            i),
                        QueryResultTypes.DataTable);

                    // преобразуем данные мастер таблицы согласно списку нужных колонок
                    var tblResult = FillTableData(tblData, dataColumns);
                    tblSummary.ImportRow(tblResult.Rows[tblResult.Rows.Count - 1]);
                }

                return tblSummary;
            }
        }

        // Список колонок ДК Саратов - Структура долга
        private Dictionary<string, ReportColumnType> GetStructureColumnsListSaratov()
        {
            var dataColumns = new Dictionary<string, ReportColumnType>();
            dataColumns.Add("Empty1", ReportColumnType.ctCalc); // 0
            dataColumns.Add("BgnYearDbt", ReportColumnType.ctDataNum); // 1
            dataColumns.Add("StaleBgnYearDbt", ReportColumnType.ctDataNum); // 2
            dataColumns.Add("ParentBudAttract", ReportColumnType.ctDataNum); // 3
            dataColumns.Add("DeficitBudAttract", ReportColumnType.ctDataNum); // 4
            dataColumns.Add("Zero1", ReportColumnType.ctCalc); // 5
            dataColumns.Add("StartBudgDischarge", ReportColumnType.ctDataNum); // 6
            dataColumns.Add("BudgetaryDischarge", ReportColumnType.ctDataNum); // 7
            dataColumns.Add("Zero2", ReportColumnType.ctCalc); // 8
            dataColumns.Add("+1;+5;-8", ReportColumnType.ctForm); // 9
            dataColumns.Add("+9;+2", ReportColumnType.ctForm); // 10
            dataColumns.Add("+5;-8", ReportColumnType.ctForm); // 11
            dataColumns.Add("StartLimitDebt", ReportColumnType.ctDataNum); // 12
            dataColumns.Add("LimitDebt", ReportColumnType.ctDataNum); // 13
            return dataColumns;
        }

        // собственные расходы
        private DataTable GetStructureTableSaratov(IScheme schema, int refVariant, string listMO)
        {
            const string DataQuery = "select {0} from {1} where RefVariant = {2} and RefRegion in ({3}) and RefKind = {4}";
            using (var db = schema.SchemeDWH.DB)
            {
                var dataColumns = GetStructureColumnsListSaratov();

                var tblSummary = CreateReportCaptionTable(dataColumns.Count);
                for (var i = 1; i < 6; i++)
                {
                    // выбираем данные из мастер таблицы
                    var dataEntity = schema.RootPackage.FindEntityByName("8739941e-a0d6-49fb-bece-ce276351e804");
                    var tblData = (DataTable)db.ExecQuery(
                        String.Format(
                            DataQuery,
                            GetFieldNames2(dataEntity, String.Empty),
                            dataEntity.FullDBName,
                            refVariant,
                            listMO,
                            i),
                        QueryResultTypes.DataTable);

                    // преобразуем данные мастер таблицы согласно списку нужных колонок
                    var tblResult = FillTableData(tblData, dataColumns);
                    tblSummary.ImportRow(tblResult.Rows[tblResult.Rows.Count - 1]);
                }

                return tblSummary;
            }
        }
    }
}
