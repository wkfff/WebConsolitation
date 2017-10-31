using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Domain.Services.FinSourceDebtorBook.Reports
{
    /// <summary>
    /// Получение данных - Ярославль
    /// </summary>
    public partial class ReportsDataService
    {
        public void GetDebtorBookYarData(
            int refVariant, 
            int refRegion, 
            ref DataTable[] tables, 
            DateTime calculateDate, 
            ref Dictionary<string, string> regionTitles)
        {
            const int ColumnCount = 18;
            const int TableCount = 18;
            var reportDate = calculateDate;

            // список полей 1) кредиты от от кр.орг.\бюдж. 2) ЦБ 3) гарантии "_" одноименное поле, "0" - нулевое 
            var fieldList = new Collection<string>();
            fieldList.Add("ChargeDate;_;_");           // Дата
            fieldList.Add("RemnsBgnMnthDbt;_;_");      // ОД /Остаток на начало месяца
            fieldList.Add("Attract;_;UpDebt");         // ОД /Получено
            fieldList.Add("Discharge;_;DownDebt");     // ОД /Погашено
            fieldList.Add("RemnsEndMnthDbt;_;_");      // ОД /Остаток на конец месяца
            fieldList.Add("DiffrncRatesDbt;0;0");      // ОД / Курсовая  разница
            fieldList.Add("CreditPercent;Coupon;0");   // Проценты /Ставка %
            fieldList.Add("RemnsBgnMnthInterest;_;0"); // Проценты /Остаток на начало месяца
            fieldList.Add("PlanService;_;0");          // Проценты /Начислено
            fieldList.Add("FactService;_;0");          // Проценты /Уплачено
            fieldList.Add("RemnsEndMnthInterest;_;0"); // Проценты /Остаток на конец месяца
            fieldList.Add("DiffrncRatesInterest;0;0"); // Проценты /Курсовая разница
            fieldList.Add("Penalty;0;0");              // Пени /Ставка пени
            fieldList.Add("RemnsBgnMnthPenlt;_;0"); // Пени /Остаток на начало месяца
            fieldList.Add("ChargePenlt;_;0");          // Пени /Начислено
            fieldList.Add("FactPenlt;_;0");          // Пени /Уплачено
            fieldList.Add("RemnsEndMnthPenlt;_;0");    // Пени /Остаток на конец месяца
            fieldList.Add("DiffrncRatesPenlt;0;0");    // Пени /Курсовая разница 

            // шаблон запроса на получение данных по гарантиям
            const string ContactQuery1 =
                "select {0}, o.Name as OrgName, ct.Name as ContractName " +
                "from {1} c, {2} ct, {3} o " +
                "where c.RefVariant = {4} and c.RefRegion in ({5}) and " +
                " c.RefOrganizations = o.ID and c.RefTypeContract = ct.ID and c.RefTypeCredit = {6} order by c.ContractDate";

            const string ContactQuery2 =
                "select {0}, o.Name as OrgName " +
                "from {1} c, {2} o " +
                "where c.RefVariant = {4} and c.RefRegion in ({5}) and c.RefOrg = o.ID order by c.RegEmissionDate";

            const string ContactQuery3 =
                "select {0}, o.Name as OrgName " +
                "from {1} c, {2} o " +
                "where c.RefVariant = {4} and c.RefRegion in ({5}) and c.RefOrganizations = o.ID order by c.StartDate";

            const string ContactQuery4 =
                "select {0}, r.Name as RegionName from {1} c, {5} r " +
                "where c.RefVariant = {2} and c.RefRegion in ({3}) and c.RefTypeCredit = {4} and c.RefRegion = r.id";

            // Основные таблицы
            var grnEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.f_S_SchBGuarantissued);
            var crdEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.f_S_SchBCreditincome);
            var capEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.f_S_SchBCapital);
            
            // Таблицы поселений
            var crdSettleEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.f_S_SchBCreditincomePos);
            
            // Вспомогательные таблицы
            var cttEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.d_S_TypeContract);
            var orgEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.d_Organizations_Plan);
            var rgnEntity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.d_Regions_Analysis);

            const int ServiceTableCount = 1;
            var variantID = refVariant;
            
            // Немного подкоректируем дату вывода, тк 1 число равносильно выводу данных за все предыдущие
            if (calculateDate.Day == 1)
            {
                calculateDate = calculateDate.AddDays(-1);
            }

            using (var db = scheme.SchemeDWH.DB)
            {
                var regionName = String.Empty;
                var listMO = GetRegionSettles(db, rgnEntity, refRegion, ref regionName, 3);

                if (listMO.Length == 0)
                {
                    listMO = refRegion.ToString();
                }

                var regionsList = listMO.Split(',');
                var fullTableCount = regionsList.Length * TableCount;
                var tblFullData = new DataTable[fullTableCount + 6 + ServiceTableCount];

                tblFullData[tblFullData.Length - 1] = CreateReportCaptionTable(regionsList.Length);
                var rowFlagSettleExist = tblFullData[tblFullData.Length - 1].Rows.Add();

                for (var i = 1; i < 7; i++)
                {
                    CreateSummaryTable(ref tblFullData[tblFullData.Length - i - ServiceTableCount], ColumnCount + 1);
                }

                for (var rc = 0; rc < regionsList.Length; rc++)
                {
                    var dicPosData = new Dictionary<int, DataTable>();
                    var regionID = Convert.ToInt32(regionsList[rc]);
                    var regionChildsID = GetRegionSettles(db, rgnEntity, regionID, ref regionName, 4);
                    rowFlagSettleExist[rc] = 0;
                    var tblResult = new DataTable[TableCount];

                    if (!regionTitles.ContainsKey(regionName))
                    {
                        regionTitles.Add(regionName, GetRegionUserTitle(scheme, regionID));
                    }

                    for (int i = 0; i < TableCount; i++)
                    {
                        tblResult[i] = CreateReportCaptionTable(ColumnCount + 1);
                    }

                    var tblContracts = new DataTable[4];
                    
                    // создаем таблицы для результатов по месяцам
                    for (var i = 4; i < TableCount; i++)
                    {
                        CreateSummaryTable(ref tblResult[i], ColumnCount + 1);
                    }

                    tblResult[4].Rows[0][ColumnCount] = regionName;
                    
                    // заполняем таблицы для поселений
                    for (var i = 8; i < 10; i++)
                    {
                        var currentCrdIndex = i - 8;

                        if (regionChildsID.Length == 0)
                        {
                            continue;
                        }

                        var contractEntity = crdSettleEntity;
                        var tblContractsPos = (DataTable)db.ExecQuery(
                            String.Format(
                                ContactQuery4,
                                GetFieldNames(contractEntity, "c"), 
                                contractEntity.FullDBName,
                                variantID, 
                                regionChildsID,
                                currentCrdIndex,
                                rgnEntity.FullDBName), 
                            QueryResultTypes.DataTable);
                        var drsMaster = tblContractsPos.Select("ParentID is null");
                        rowFlagSettleExist[rc] = Math.Max(Convert.ToInt32(rowFlagSettleExist[rc]), drsMaster.Length);
                        var tblPosChanges = tblResult[currentCrdIndex].Clone();

                        dicPosData.Add(currentCrdIndex, tblPosChanges);

                        FillChangeRows(
                            tblContractsPos,
                            tblPosChanges,
                            tblResult[i],
                            drsMaster,
                            fieldList,
                            i,
                            ColumnCount,
                            calculateDate);
                    }

                    for (var i = 0; i < 4; i++)
                    {
                        var queryStr = ContactQuery1;
                        var contractEntity = crdEntity;
                        
                        if (i == 2)
                        {
                            contractEntity = capEntity;
                            queryStr = ContactQuery2;
                        }

                        if (i == 3)
                        {
                            contractEntity = grnEntity;
                            queryStr = ContactQuery3;
                        }

                        tblContracts[i] = (DataTable)db.ExecQuery(
                            String.Format(
                                queryStr,
                                GetFieldNames(contractEntity, "c"), 
                                contractEntity.FullDBName, 
                                cttEntity.FullDBName, 
                                orgEntity.FullDBName,
                                variantID, 
                                regionID, 
                                i), 
                            QueryResultTypes.DataTable);

                        var drsMaster = tblContracts[i].Select("ParentID is null");

                        FillChangeRows(
                            tblContracts[i], 
                            tblResult[i], 
                            tblResult[4 + i], 
                            drsMaster, 
                            fieldList, 
                            i,
                            ColumnCount, 
                            calculateDate);

                        if (dicPosData.ContainsKey(i))
                        {
                            foreach (DataRow dataRow in dicPosData[i].Rows)
                            {
                                tblResult[i].ImportRow(dataRow);
                            }
                        }
                    }

                    CalcSummaryTable(tblResult[10], tblResult[04], tblResult[08], ColumnCount);
                    CalcSummaryTable(tblResult[11], tblResult[05], tblResult[09], ColumnCount);
                    CalcSummaryTable(tblResult[12], tblResult[04], tblResult[05], ColumnCount);
                    CalcSummaryTable(tblResult[12], tblResult[12], tblResult[06], ColumnCount);
                    CalcSummaryTable(tblResult[13], tblResult[08], tblResult[09], ColumnCount);
                    CalcSummaryTable(tblResult[14], tblResult[12], tblResult[13], ColumnCount);
                    CalcSummaryTable(tblResult[15], tblResult[12], tblResult[07], ColumnCount);
                    CalcSummaryTable(tblResult[16], tblResult[16], tblResult[13], ColumnCount);
                    CalcSummaryTable(tblResult[17], tblResult[15], tblResult[16], ColumnCount);

                    var totalIndex = tblFullData.Length - 6 - ServiceTableCount;
                    CalcSummaryTable(tblFullData[totalIndex + 0], tblFullData[totalIndex + 0], tblResult[10], ColumnCount);
                    CalcSummaryTable(tblFullData[totalIndex + 1], tblFullData[totalIndex + 1], tblResult[11], ColumnCount);
                    CalcSummaryTable(tblFullData[totalIndex + 2], tblFullData[totalIndex + 2], tblResult[06], ColumnCount);
                    CalcSummaryTable(tblFullData[totalIndex + 3], tblFullData[totalIndex + 3], tblResult[14], ColumnCount);
                    CalcSummaryTable(tblFullData[totalIndex + 4], tblFullData[totalIndex + 4], tblResult[07], ColumnCount);
                    CalcSummaryTable(tblFullData[totalIndex + 5], tblFullData[totalIndex + 5], tblResult[17], ColumnCount);

                    var tableOffset = rc * TableCount;

                    for (var i = 0; i < TableCount; i++)
                    {
                        var index = tableOffset + i;
                        tblFullData[index] = tblResult[i];
                    }
                }
                
                // месячная отчетность
                var fullTerritoryList = String.Format("{0},{1}", listMO, GetSettlesHierarchyKeys(listMO)).Trim(',');
                Array.Resize(ref tblFullData, tblFullData.Length + 1);
                var tblMonthData = CreateReportCaptionTable(5);
                var rowMonthData = tblMonthData.Rows.Add();
                var kstCodes = new Collection<int>
                                   {
                                       984, 982, 986, 981
                                   };

                // итоговая по всему субъету
                for (var i = 0; i < kstCodes.Count; i++)
                {
                    rowMonthData[i] = GetMonthReportData(reportDate, fullTerritoryList, kstCodes[i], true);
                }
                
                // по районам
                foreach (var t in regionsList)
                {
                    rowMonthData = tblMonthData.Rows.Add();
                    fullTerritoryList =
                        String.Format("{0},{1}", t, GetSettlesHierarchyKeys(t)).Trim(',');

                    for (var i = 0; i < kstCodes.Count; i++)
                    {
                        rowMonthData[i] = GetMonthReportData(reportDate, fullTerritoryList, kstCodes[i], true);
                    }
                }

                tblFullData[tblFullData.Length - 1] = tblMonthData;
                tables = tblFullData;
            }
        }

        private static void FillChangeRows(
            DataTable tblContracts,
            DataTable tblChanges,
            DataTable tblSummary, 
            IEnumerable<DataRow> rowsChange,
            IList<string> fieldList,
            int contractType,
            int columnCount,
            DateTime calculateDate)
        {
            foreach (var rowMaster in rowsChange)
            {
                var drsDetail = tblContracts.Select(
                    String.Format(
                        "ParentID = {0} and ChargeDate <= '{1}'",
                        rowMaster["ID"],
                        calculateDate.ToShortDateString()),
                    "ChargeDate asc");
                var rowCaption = tblChanges.Rows.Add();
                FillDKCaptionRow(rowCaption, rowMaster, contractType, columnCount);
                var summary = new double[columnCount];
                FillSummaryTable(tblSummary, rowCaption, drsDetail, columnCount, fieldList, contractType);
                var staleSummary = new double[3];

                foreach (DataRow t in drsDetail)
                {
                    var rowData = tblChanges.Rows.Add();

                    for (var f = 0; f < fieldList.Count; f++)
                    {
                        var fieldName = GetFieldName(fieldList, f, contractType);

                        if (fieldName == "0")
                        {
                            rowData[f] = 0;
                        }
                        else
                        {
                            rowData[f] = t[fieldName];
                        }

                        if (f > 1)
                        {
                            if (f != 6 && f != 12)
                            {
                                if (f == 4 || f == 10 || f == 16)
                                {
                                    summary[f] = GetDoubleValue(rowData[f]);
                                }
                                else
                                {
                                    summary[f] += GetDoubleValue(rowData[f]);
                                }
                            }
                        }
                    }

                    rowData[0] = GetDateValue(rowData[0]);
                    rowData[columnCount] = 2;

                    if (t.Table.Columns.Contains("StaleDebt"))
                    {
                        staleSummary[0] = GetDoubleValue(t["StaleDebt"]);
                    }

                    if (t.Table.Columns.Contains("StaleInterest"))
                    {
                        staleSummary[1] = GetDoubleValue(t["StaleInterest"]);
                    }

                    if (t.Table.Columns.Contains("StalePenlt"))
                    {
                        staleSummary[2] = GetDoubleValue(t["StalePenlt"]);
                    }
                }

                // Просроченную задолженость выводим в таблицу итогов
                var resultSummaryTable = tblSummary;
                var rowStaleRow = resultSummaryTable.Rows[resultSummaryTable.Rows.Count - 1];

                for (var si = 0; si < 3; si++)
                {
                    var cellOffset = 6 * si;
                    var totalOffset = 04 + cellOffset;
                    rowStaleRow[totalOffset] = GetDoubleValue(rowStaleRow[totalOffset]) + staleSummary[si];
                }

                var rowSummary = tblChanges.Rows.Add();
                FillDKSummaryRow(rowSummary, summary, columnCount);
            }            
        }

        private static string GetFieldName(IList<string> fieldList, int colNumber, int contractType)
        {
            var values = fieldList[colNumber].Split(';');
            var index = 0;

            if (contractType == 2)
            {
                index = 1;
            }

            if (contractType == 3)
            {
                index = 2;
            }

            if (values[index] == "_")
            {
                index = 0;
            }

            return values[index];
        }

        private static void CreateSummaryTable(ref DataTable tblResult, int columnCount)
        {
            tblResult = CreateReportCaptionTable(columnCount);

            for (var i = 0; i < 15; i++)
            {
                var dr = tblResult.Rows.Add();

                for (var j = 1; j < columnCount; j++)
                {
                    dr[j] = 0;
                }
            }
        }

        private static void CalcSummaryTable(DataTable tblResult, DataTable tblResult1, DataTable tblResult2, int columnCount)
        {
            for (var j = 0; j < 15; j++)
            {
                for (var i = 1; i < columnCount; i++)
                {
                    tblResult.Rows[j][i] = GetDoubleValue(tblResult1.Rows[j][i]) +
                        GetDoubleValue(tblResult2.Rows[j][i]);
                }
            }
        }

        private static void FillSummaryTable(
            DataTable tblResult, 
            DataRow rowMaster, 
            IEnumerable<DataRow> drsDetails,
            int columnCount, 
            IList<string> fieldList, 
            int contractType)
        {
            for (var i = 1; i < columnCount; i++)
            {
                tblResult.Rows[0][i] = GetDoubleValue(tblResult.Rows[0][i]) + GetDoubleValue(rowMaster[i]);
            }

            var summary = new double[13, columnCount];
            var isValue = new bool[13, columnCount];
            var maxMonthNum = 0;
            var monthList = new Collection<int>();

            // Это чтобы и договора без детализаций учитываались полностью
            for (var i = 0; i < 3; i++)
            {
                var tableOffset = i * 6;
                summary[0, 1 + tableOffset] = GetDoubleValue(rowMaster[1 + tableOffset]);
                summary[0, 4 + tableOffset] = summary[0, 1 + tableOffset];
            }

            for (var i = 1; i < columnCount; i++)
            {
                foreach (DataRow t in drsDetails)
                {
                    if (i == 6 || i == 12 || t["ChargeDate"] == DBNull.Value)
                    {
                        continue;
                    }

                    var monthNum = Convert.ToDateTime(t["ChargeDate"]).Month;
                    maxMonthNum = Math.Max(maxMonthNum, monthNum);

                    if (!monthList.Contains(monthNum))
                    {
                        monthList.Add(monthNum);
                    }

                    double value = 0;
                    var fieldName = GetFieldName(fieldList, i, contractType);

                    if (fieldName != "0")
                    {
                        value = GetDoubleValue(t[fieldName]);
                    }

                    switch (i)
                    {
                        case 13:
                        case 7:
                        case 1:
                            if (!isValue[monthNum, i])
                            {
                                summary[monthNum, i] = value;
                                isValue[monthNum, i] = true;
                            }

                            break;
                        case 16:
                        case 10:
                        case 4:
                            summary[monthNum, i] = value;
                            break;
                        default:
                            summary[monthNum, i] += value;
                            break;
                    }
                }
            }

            if (maxMonthNum < 12)
            {
                for (var i = 0; i < 3; i++)
                {
                    for (var j = maxMonthNum + 1; j < 13; j++)
                    {
                        var delta = i * 6;
                        summary[j, 1 + delta] = summary[maxMonthNum, 4 + delta];
                        summary[j, 4 + delta] = summary[j, 1 + delta];
                    }
                }
            }

            // если не все месяца заполнены
            if (monthList.Count != maxMonthNum)
            {
                var sumAfter = new double[3];

                for (var j = 0; j < 3; j++)
                {
                    var cellOffset = j * 6;
                    sumAfter[j] = GetDoubleValue(rowMaster[1 + cellOffset]);
                }

                for (var j = 1; j <= maxMonthNum; j++)
                {
                    if (!monthList.Contains(j))
                    {
                        for (var k = 0; k < 3; k++)
                        {
                            var cellOffset = k * 6;
                            summary[j, 1 + cellOffset] = sumAfter[k];
                            summary[j, 4 + cellOffset] = sumAfter[k];
                        }
                    }

                    for (var k = 0; k < 3; k++)
                    {
                        var cellOffset = k * 6;
                        sumAfter[k] = summary[j, 4 + cellOffset];
                    }
                }
            }

            var totalSummary = new double[columnCount];

            for (var i = 1; i < columnCount; i++)
            {
                for (var j = 1; j < 13; j++)
                {
                    var value = summary[j, i];
                    tblResult.Rows[j][i] = GetDoubleValue(tblResult.Rows[j][i]) + summary[j, i];

                    if (i == 4 || i == 10 || i == 16)
                    {
                        totalSummary[i] = value;
                    }
                    else
                    {
                        totalSummary[i] += value;
                    }
                }
            }

            for (var i = 1; i < columnCount; i++)
            {
                tblResult.Rows[13][i] = GetDoubleValue(tblResult.Rows[13][i]) + totalSummary[i];
            }
        }

        private static void FillDKCaptionRow(DataRow rowDest, DataRow rowSource, int contractType, int flagIndex)
        {
            if (contractType == 0 || contractType == 1)
            {
                rowDest[0] = String.Format(
                    "{0} №{1} от {2} Кредитор: {3} Дата погашения: {4} Вид обеспечения: {5}",
                    rowSource["ContractName"],
                    rowSource["Num"],
                    GetDateValue(rowSource["ContractDate"]),
                    rowSource["Creditor"],
                    GetDateValue(rowSource["DebtEndDate"]),
                    rowSource["Collateral"]);
                rowDest[01] = rowSource["BgnYearDbt"];
                rowDest[06] = rowSource["CreditPercentDoc"];
            }

            if (contractType == 8 || contractType == 9)
            {
                rowDest[0] = String.Format(
                    "{0} №{1} от {2} Кредитор: {3} Дата погашения: {4}",
                    rowSource["RegionName"],
                    rowSource["Num"],
                    GetDateValue(rowSource["ContractDate"]),
                    rowSource["Creditor"],
                    GetDateValue(rowSource["DebtEndDate"]));
                rowDest[01] = rowSource["BgnYearDbt"];
                rowDest[06] = rowSource["CreditPercentDoc"];
            }

            if (contractType == 2)
            {
                rowDest[0] = String.Format(
                    "Выпуск № {0} от {1} {2} Дата погашения: {3} Вид обеспечения: {4}",
                    rowSource["OfficialNumber"],
                    GetDateValue(rowSource["RegEmissionDate"]),
                    rowSource["GenAgent"],
                    GetDateValue(rowSource["DateDischarge"]),
                    rowSource["Collateral"]);
                rowDest[01] = rowSource["BgnYearDbt"];
            }

            if (contractType == 3)
            {
                rowDest[0] = String.Format(
                    "Гарантия № {0} от {1} Принципал: {2} Кредитный договор № {3} от {4} Кредитор: {5} Дата погашения: {6} Вид обеспечения: {7}",
                    rowSource["Num"],
                    GetDateValue(rowSource["StartDate"]),
                    rowSource["Principal"],
                    rowSource["PrincipalNum"],
                    GetDateValue(rowSource["PrincipalStartDate"]),
                    rowSource["Creditor"],
                    GetDateValue(rowSource["PrincipalEndDate"]),
                    rowSource["Collateral"]);
                rowDest[01] = rowSource["BgnYearDebt"];
            }

            if (contractType > 3)
            {
                rowDest[01] = rowSource["BgnYearDbt"];
            }

            if (contractType != 3)
            {
                rowDest[07] = rowSource["BgnYearInterest"];
                rowDest[13] = rowSource["BgnYearPenlt"];
            }
            else
            {
                rowDest[07] = DBNull.Value;
                rowDest[13] = DBNull.Value;
            }

            rowDest[flagIndex] = 1;
        }

        private static void FillDKSummaryRow(DataRow rowDest, IList<double> summary, int flagIndex)
        {
            for (var i = 1; i < summary.Count; i++)
            {
                rowDest[i] = summary[i];
            }

            rowDest[flagIndex] = 3;
        }
    }
}
