using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.Form4NMPump
{

    // ФНС - 0006 - Форма 4-НМ
    public partial class Form4NMPumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // Задолженность.ФНС (d_Arrears_FNS)
        private IDbDataAdapter daArrears;
        private DataSet dsArrears;
        private IClassifier clsArrears;
        private Dictionary<string, int> cacheArrears = null;
        private int nullArrears;
        // Доходы.Группы ФНС (d_D_GroupFNS)
        private IDbDataAdapter daIncomes;
        private DataSet dsIncomes;
        private IClassifier clsIncomes;
        private Dictionary<string, int> cacheIncomes = null;
        private int nullIncomes;
        // Районы.ФНС (d_Regions_FNS)
        private IDbDataAdapter daRegions;
        private DataSet dsRegions; 
        private IClassifier clsRegions;
        private Dictionary<string, int> cacheRegions = null;
        private Dictionary<string, int> cacheRegionsName = null;
        private int nullRegions;

        #endregion Классификаторы

        #region Факты

        // Доходы.ФНС_4 НМ_Сводный (f_D_FNS4NMTotal)
        private IDbDataAdapter daIncomesTotal;
        private DataSet dsIncomesTotal;
        private IFactTable fctIncomesTotal;
        // Доходы.ФНС_4 НМ_Районы (f_D_FNS4NMRegions)
        private IDbDataAdapter daIncomesRegion;
        private DataSet dsIncomesRegion;
        private IFactTable fctIncomesRegion;

        #endregion Факты

        private ReportType reportType;
        // для проверки итоговых сумм
        private decimal[] totalSums = new decimal[26];
        // SourceID источника для классификатора Доходы.Группы ФНС
        private int incomesSourceId = -1;
        // признак отчетов нового формата с августа 2011 года
        private bool fromAugust2011 = false;

        // все коды задолженностей, разбитые по разделам
        private Dictionary<int, int[]> allArrearsCodes = null;
        // ID записей задолженности первого уровня иерархии
        private Dictionary<int, int> arrearsParentIds = null;
        // маппинги полей с кодами доходов
        private Dictionary<int, int[]> incomesMappings = null;
        // соотношение кодов "Задолженности" с показателями "Доходов"
        private Dictionary<int, int> incomesByArrears = null;
        // список отсутствующих в отчете кодов задолженностей (только для отчетов в разрезе строк)
        private List<int> absentArrearsCodesStr = null;

        // параметры обработки
        private int year = -1;
        private int month = -1;

        #endregion Поля

        #region Структуры, перечисления

        // тип отчета
        private enum ReportType
        {
            Svod,
            Str,
            Region
        }

        #endregion Структуры, перечисления

        #region Закачка данных

        #region Работа с базой и кэшами

        private void SetFlags()
        {
            fromAugust2011 = ((this.DataSource.Year >= 2011) && (this.DataSource.Month >= 8) || (this.DataSource.Year>=2012));
        }

        private void InitUpdateFixedRows()
        {
            nullArrears = clsArrears.UpdateFixedRows(this.DB, this.SourceID);
            nullIncomes = clsIncomes.UpdateFixedRows(this.DB, incomesSourceId);
            nullRegions = clsRegions.UpdateFixedRows(this.DB, this.SourceID);
        }

        private void FillCaches()
        {
            FillRowsCache(ref cacheArrears, dsArrears.Tables[0], "CODE", "ID");
            FillRowsCache(ref cacheIncomes, dsIncomes.Tables[0], "CODE", "ID");
            FillRowsCache(ref cacheRegions, dsRegions.Tables[0], "CODE", "ID");
            FillRowsCache(ref cacheRegionsName, dsRegions.Tables[0], new string[] { "CODE", "NAME" }, "|", "ID");
        }

        protected override void QueryData() 
        {
            SetFlags();

            InitClsDataSet(ref daArrears, ref dsArrears, clsArrears);
            InitClsDataSet(ref daRegions, ref dsRegions, clsRegions);
            incomesSourceId = AddDataSource("ФНС", "0006", ParamKindTypes.YearMonth, string.Empty, this.DataSource.Year, 0, string.Empty, 0, string.Empty).ID;
            InitDataSet(ref daIncomes, ref dsIncomes, clsIncomes, false, string.Format("SOURCEID = {0} AND ID > 0", incomesSourceId), string.Empty);
            
            InitFactDataSet(ref daIncomesTotal, ref dsIncomesTotal, fctIncomesTotal);
            InitFactDataSet(ref daIncomesRegion, ref dsIncomesRegion, fctIncomesRegion);

            FillCaches();

            InitUpdateFixedRows();
            InitAuxStructures();
        }

        private const string D_ARREARS_FNS_GUID = "516ec293-bf4c-4ff8-a2c5-bc04acb70a81";
        private const string D_D_GROUP_FNS_GUID = "b9169eb6-de81-420b-8a2b-05ffa2fd35c1";
        private const string D_REGIONS_FNS_GUID = "cf3202f9-e897-43ce-a158-5c617bedff55";
        private const string F_D_FNS4NM_REGIONS_GUID = "b51ee6f4-9a3f-4950-a76b-53661b610bd3";
        private const string F_D_FNS4NM_TOTAL_GUID = "8b5517d1-79ba-4fdd-8259-411e220540d5";
        protected override void InitDBObjects()
        {
            clsIncomes = this.Scheme.Classifiers[D_D_GROUP_FNS_GUID];
            clsArrears = this.Scheme.Classifiers[D_ARREARS_FNS_GUID];
            clsRegions = this.Scheme.Classifiers[D_REGIONS_FNS_GUID];

            fctIncomesTotal = this.Scheme.FactTables[F_D_FNS4NM_TOTAL_GUID];
            fctIncomesRegion = this.Scheme.FactTables[F_D_FNS4NM_REGIONS_GUID];

            this.UsedClassifiers = new IClassifier[] { clsArrears, clsRegions };
            this.UsedFacts = new IFactTable[] { fctIncomesTotal, fctIncomesRegion };
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daArrears, dsArrears, clsArrears);
            UpdateDataSet(daIncomes, dsIncomes, clsIncomes);
            UpdateDataSet(daRegions, dsRegions, clsRegions);
            UpdateDataSet(daIncomesTotal, dsIncomesTotal, fctIncomesTotal);
            UpdateDataSet(daIncomesRegion, dsIncomesRegion, fctIncomesRegion);
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsIncomesTotal);
            ClearDataSet(ref dsIncomesRegion);
            ClearDataSet(ref dsArrears);
            ClearDataSet(ref dsIncomes);
            ClearDataSet(ref dsRegions);

            allArrearsCodes.Clear();
            incomesMappings.Clear();
            incomesByArrears.Clear();
        }

        #endregion Работа с базой и кэшами

        #region Работа с Excel

        private int CleanIntValue(string value)
        {
            int intValue = 0;
            Int32.TryParse(CommonRoutines.TrimLetters(value.Trim()), out intValue);
            return intValue;
        }

        private decimal CleanFactValue(string value)
        {
            decimal factValue = 0;
            Decimal.TryParse(CommonRoutines.TrimLetters(value).Replace('.', ','), out factValue);
            return factValue;
        }

        #region Классификатор Задолженность.ФНС

        // закачивает строку в классификатор Задолженность.ФНС и возвращает ID закачанной записи
        private int PumpArrears(int code, string name, int parentId)
        {
            if (name.Length > 255)
                name = name.Substring(0, 255);

            object[] mapping = new object[] { "Code", code, "Name", name };
            if (parentId != -1)
                mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "ParentID", parentId });

            if (reportType == ReportType.Svod)
                // новые записи качает только для сводных отчетов
                return PumpCachedRow(cacheArrears, dsArrears.Tables[0], clsArrears, code.ToString(), mapping);
            // для остальных ищем ID в кэше по коду
            return FindCachedRow(cacheArrears, code.ToString(), nullArrears);
        }

        // в разделах II и II.I для некоторых задолженностей по некоторым столбцам формируется третий уровень иерархии
        // возвращает true, если для кода arrearsCode был сформирован третий уровень
        // в параметре refArrears передается ID записи второго уровня,
        // а возвращается ID записи третьего уровня, если он был сформирован
        private bool TryFormThirdArrearsLevel(int sectionIndex, int curCol, int arrearsCode, ref int refArrears)
        {
            string arrearsName = string.Empty;
            switch (sectionIndex)
            {
                case 1:
                    #region раздел II
                    // в столбцах D и E подчиненные записи с кодам XXXX1 и XXXX2 соответственно
                    switch (curCol)
                    {
                        case 4:
                            arrearsCode = arrearsCode * 10 + 1;
                            arrearsName = "задолженность по уплате пеней";
                            break;
                        case 5:
                            arrearsCode = arrearsCode * 10 + 2;
                            arrearsName = "задолженность по уплате налоговых санкций";
                            break;
                        default:
                            return false;
                    }
                    #endregion
                    break;
                case 2:
                    // проверяем, нужно ли для данного кода формировать третий уровень иерархии
                    if (!GetArrearsCodesWithThirdLevel().Contains(arrearsCode))
                        return false;
                    if (fromAugust2011)
                    {
                        #region с августа-2011:
                        // в столбцах E, F, G, H, I - записи с кодам XXXX1, XXXX2, XXXX3, XXXX4, XXXX5 соответственно
                        switch (curCol)
                        {
                            case 5:
                                arrearsCode = arrearsCode * 10 + 1;
                                arrearsName = "задолженность по недоимке";
                                break;
                            case 6:
                                arrearsCode = arrearsCode * 10 + 2;
                                arrearsName = "задолженность по пени";
                                break;
                            case 7:
                                arrearsCode = arrearsCode * 10 + 3;
                                arrearsName = "задолженность по штрафам";
                                break;
                            case 8:
                                arrearsCode = arrearsCode * 10 + 4;
                                arrearsName = "задолженность по процентам";
                                break;
                            case 9:
                                arrearsCode = arrearsCode * 10 + 5;
                                arrearsName = "Задолженность по страховым взносам в государственные социальные внебюджетные фонды";
                                break;
                            default:
                                return false;
                        }
                        #endregion
                    }
                    else
                    {
                        #region до августа-2011:
                        // в столбцах D, E и F - записи с кодам XXXX1, XXXX2 и XXXX3 соответственно
                        switch (curCol)
                        {
                            case 4:
                                arrearsCode = arrearsCode * 10 + 1;
                                arrearsName = "задолженность по недоимке";
                                break;
                            case 5:
                                arrearsCode = arrearsCode * 10 + 2;
                                arrearsName = "задолженность по пени";
                                break;
                            case 6:
                                arrearsCode = arrearsCode * 10 + 3;
                                arrearsName = "задолженность по штрафам (до 01.01.1999)";
                                break;
                            default:
                                return false;
                        }
                        #endregion
                    }
                    break;
                default:
                    return false;
            }
            refArrears = PumpArrears(arrearsCode, arrearsName, refArrears);
            return true;
        }

        #endregion

        #region Классификатор Районы.ФНС

        // закачивает строку в классификатор Районы.ФНС и возвращает ID закачанной записи
        private int PumpRegions(string code, string name)
        {
            code = CommonRoutines.TrimLetters(code.Trim()).PadLeft(1, '0');
            object[] mapping = new object[] { "Code", code, "Name", name };

            if (reportType == ReportType.Str)
                return PumpCachedRow(cacheRegions, dsRegions.Tables[0], clsRegions, code, mapping);

            string key = string.Format("{0}|{1}", code, name);
            return PumpCachedRow(cacheRegionsName, dsRegions.Tables[0], clsRegions, key, mapping);
        }
        
        // для отчетов в разрезе районов ищет данные о районе в начале листа,
        // закачивает запись в классификатор Районы и возвращает ID закачанной записи
        private int GetRefRegions(ExcelHelper excelDoc)
        {
            if (reportType != ReportType.Region)
                return nullRegions;

            int rowsCount = excelDoc.GetRowsCount();
            for (int curRow = 1; curRow < rowsCount; curRow++)
            {
                string cellValue = excelDoc.GetValue(curRow, 1).Trim().ToUpper();
                if (cellValue.Contains("ОБРАЗОВАНИЕ") && cellValue.Contains("ГОРОД"))
                {
                    string code = excelDoc.GetValue(curRow + 3, 1).Trim();
                    string name = excelDoc.GetValue(curRow + 1, 1).Trim();
                    return PumpRegions(code, name);
                }
            }
            return nullRegions;
        }

        #endregion

        // возращет ID записи классификатора Доходы.Группы ФНС по коду дохода incomesCode
        // для раздела II.I код дохода берется в соответствии с кодом задолженности
        private int GetRefIncomes(int incomesCode, int sectionIndex, int arrearsCode)
        {
            if (sectionIndex == 2)
            {
                if (incomesByArrears.ContainsKey(arrearsCode))
                    incomesCode = incomesByArrears[arrearsCode];
                if (incomesCode == -1)
                    return nullIncomes;
            }
            return FindCachedRow(cacheIncomes, incomesCode.ToString(), nullIncomes);
        }

        // закачиваем строку в таблицы фактов
        private void PumpFactRow(string value, string orgAmount, int refDate, int refArrears, int refIncomes, int refRegions, int sumIndex)
        {
            decimal factValue = CleanFactValue(value);
            if (factValue == 0)
                return;

            totalSums[sumIndex] += factValue;
            // суммы в тыс.руб. переводим в рубли
            factValue *= 1000;

            object[] mapping = new object[] {
                "ValueReport", factValue,
                "Value", DBNull.Value,
                "RefYearDayUNV", refDate,
                "RefArrears", refArrears,
                "RefD", refIncomes,
            };

            int orgAmountReport = CleanIntValue(orgAmount);
            if (orgAmountReport != 0)
                mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "OrgAmount", orgAmountReport });

            if (reportType == ReportType.Svod)
            {
                PumpRow(dsIncomesTotal.Tables[0], mapping);
                if (dsIncomesTotal.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
                {
                    UpdateData();
                    ClearDataSet(daIncomesTotal, ref dsIncomesTotal);
                }
            }
            else
            {
                mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "RefRegions", refRegions });
                PumpRow(dsIncomesRegion.Tables[0], mapping);
                if (dsIncomesRegion.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
                {
                    UpdateData();
                    ClearDataSet(daIncomesRegion, ref dsIncomesRegion);
                }
            }
        }

        // закачивает строку Xls-отчета
        private void PumpXlsRow(ExcelHelper excelDoc, int curRow, int sectionIndex, int arrearsCode, int refDate, int refRegions)
        {
            int refArrearsParent = nullArrears;
            if (reportType == ReportType.Str)
            {
                // для отчетов в разрезе строк в 1-м и 2-м столбцах находятся данные о районах
                string regionsCode = excelDoc.GetValue(curRow, 2).Trim();
                string regionsName = excelDoc.GetValue(curRow, 1).Trim();
                refRegions = PumpRegions(regionsCode, regionsName);
                refArrearsParent = FindCachedRow(cacheArrears, arrearsCode.ToString(), nullArrears);
                if (absentArrearsCodesStr.Contains(arrearsCode))
                    absentArrearsCodesStr.Remove(arrearsCode);
            }
            else
            {
                // для остальных отчетов - данные о задолженностях
                arrearsCode = CleanIntValue(excelDoc.GetValue(curRow, 2));
                string arrearsName = excelDoc.GetValue(curRow, 1).Trim();
                refArrearsParent = PumpArrears(arrearsCode, arrearsName, arrearsParentIds[sectionIndex]);
                if (absentArrearsCodesStr.Contains(arrearsCode))
                    absentArrearsCodesStr.Remove(arrearsCode);
            }

            int count = incomesMappings[sectionIndex].GetLength(0);
            for (int i = 0; i < count; i += 2)
            {
                // маппинг доходов представляет подряд идущих пар "Столбец"-"Код дохода"
                int curCol = incomesMappings[sectionIndex][i];
                int incomesCode = incomesMappings[sectionIndex][i + 1];

                string value = excelDoc.GetValue(curRow, curCol);
                string orgAmount = string.Empty;
                int refIncomes = GetRefIncomes(incomesCode, sectionIndex, arrearsCode);
                int refArrears = refArrearsParent;
                if (!TryFormThirdArrearsLevel(sectionIndex, curCol, arrearsCode, ref refArrears))
                {
                    // с августа-2011 в разделе II.I для записей классификатора Задолженность 2-го уровня иерархии
                    // в таблицу фактов из столбца C закачивается поле "Количество органзаций"
                    if (fromAugust2011 && sectionIndex.Equals(2))
                        orgAmount = excelDoc.GetValue(curRow, 3);
                }

                PumpFactRow(value, orgAmount, refDate, refArrears, refIncomes, refRegions, i / 2);
            }
        }

        #region Работа с разделами

        // переход к следующему разделу
        private bool GotoNextSection(ExcelHelper excelDoc, ref string sectionName, ref int firstRow, ref int lastRow)
        {
            bool findSection = false;
            int rowsCount = excelDoc.GetRowsCount();
            int curRow = lastRow + 1;
            for (; curRow < rowsCount; curRow++)
            {
                string cellValue = excelDoc.GetValue(curRow, 1).Trim().ToUpper();

                if (cellValue.StartsWith("РАЗДЕЛ"))
                {
                    sectionName = excelDoc.GetValue(curRow, 1).Trim();
                    continue;
                }

                if (cellValue.StartsWith("РАЗРЕЗ"))
                {
                    sectionName = excelDoc.GetValue(curRow + 1, 1).Trim();
                    continue;
                }

                if (cellValue.Equals("А") || cellValue.Equals("A"))
                {
                    firstRow = curRow + 1;
                    findSection = true;
                    continue;
                }

                if (findSection)
                    if (cellValue.Equals("КОНТРОЛЬНАЯ СУММА") || cellValue.Equals("ВСЕГО") || cellValue.Equals(string.Empty))
                    {
                        lastRow = curRow;
                        findSection = true;
                        break;
                    }
            }
            if (curRow == rowsCount)
                lastRow = curRow;

            return findSection;
        }

        // возвращает номер раздела по коду задолженности
        private int GetSectionIndexByArrearsCode(int arrearsCode)
        {
            foreach (int sectionIndex in allArrearsCodes.Keys)
            {
                if (allArrearsCodes[sectionIndex].Contains(arrearsCode))
                    return sectionIndex;
            }
            return -1;
        }

        // возвращает номер раздела по его названию
        private int GetSectionIndexByName(string sectionName)
        {
            sectionName = sectionName.ToUpper();
            if (sectionName.Contains("РАЗДЕЛ III.I"))
                return 4;
            else if (sectionName.Contains("РАЗДЕЛ III"))
                return 3;
            else if (sectionName.Contains("РАЗДЕЛ II.I"))
                return 2;
            else if (sectionName.Contains("РАЗДЕЛ II"))
                return 1;
            else if (sectionName.Contains("РАЗДЕЛ IV.I"))
                return 6;
            else if (sectionName.Contains("РАЗДЕЛ IV"))
                return 5;
            else if (sectionName.Contains("РАЗДЕЛ I"))
                return 0;
            else if (sectionName.Contains("РАЗДЕЛ VI"))
                return 9;
            else if (sectionName.Contains("РАЗДЕЛ V.I"))
                return 8;
            else if (sectionName.Contains("РАЗДЕЛ V"))
                return 7;
            return -1;
        }

        // возвращает код задолженности из названия раздела (только для отчетов в разрезе строк)
        private int GetArrearsCodeFromSectionName(string sectionName)
        {
            if (reportType != ReportType.Str)
                return 0;
            // название разрезов для отчетов по строкам имеет вид:
            // 1030 - НЕДОИМКА ОРГАНИЗАЦИЙ, НАХОДЯЩИХСЯ В ПРОЦЕДУРЕ БАНКРОТСТВА
            // цифры в начале и есть код задолженности
            return CleanIntValue(sectionName.Split('-')[0]);
        }

        // возвращает номер текущего раздела
        private int GetSectionIndex(string sectionName)
        {
            if (reportType.Equals(ReportType.Str))
            {
                // в разрезе строк название раздела состоит из кода и названия задолженности, например:
                // 1030 - НЕДОИМКА ОРГАНИЗАЦИЙ, НАХОДЯЩИХСЯ В ПРОЦЕДУРЕ БАНКРОТСТВА
                // берем из него код задолженности и по нему определяем номер раздела
                int arrearsCode = GetArrearsCodeFromSectionName(sectionName);
                return GetSectionIndexByArrearsCode(arrearsCode);
            }
            // для остальных отчетов номер раздела определяем по его названию
            return GetSectionIndexByName(sectionName);
        }

        #endregion

        #region Проверка контрольной суммы

        // обнуляет итоговые суммы
        private void SetNullTotalSum()
        {
            int sumsCount = totalSums.GetLength(0);
            for (int i = 0; i < sumsCount; i++)
            {
                totalSums[i] = 0;
            }
        }

        // выводит предупреждение, если контрольная сумма не сходится с итоговой
        private void CheckTotalSum(decimal totalSum, decimal controlSum, string comment)
        {
            if (totalSum != controlSum)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                    "Контрольная сумма {0:F} не сходится с итоговой {1:F} {2}",
                    controlSum, totalSum, comment));
            }
        }

        // выполняет проверку итоговых сумм в xls-отчете
        private void CheckXlsTotalSum(ExcelHelper excelDoc, int curRow, int sectionIndex, string sectionName)
        {
            int count = incomesMappings[sectionIndex].GetLength(0);
            for (int i = 0; i < count; i += 2)
            {
                int curCol = incomesMappings[sectionIndex][i];

                string comment = string.Format("в столбце {0} раздела '{1}'", curCol, sectionName);
                decimal controlSum = CleanFactValue(excelDoc.GetValue(curRow, curCol));
                CheckTotalSum(totalSums[i / 2], controlSum, comment);
            }
        }

        #endregion

        // закачивает лист Xls-отчета
        private void PumpXlsSheet(ExcelHelper excelDoc, int refDate)
        {
            int refRegions = GetRefRegions(excelDoc);

            string sectionName = string.Empty;
            int firstRow = 1;
            int lastRow = 1;
            while (GotoNextSection(excelDoc, ref sectionName, ref firstRow, ref lastRow))
            {
                int sectionIndex = GetSectionIndex(sectionName);
                if (sectionIndex == -1)
                    continue;

                SetNullTotalSum();
                int arrearsCode = GetArrearsCodeFromSectionName(sectionName);
                for (int curRow = firstRow; curRow < lastRow; curRow++)
                    try
                    {
                        // если во втором столбце код не указан, строку не качаем
                        if (excelDoc.GetValue(curRow, 2).Trim() == string.Empty)
                            continue;
                        PumpXlsRow(excelDoc, curRow, sectionIndex, arrearsCode, refDate, refRegions);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format(
                            "При обработке строки {0} листа '{1}' возникла ошибка ({2})",
                            curRow, excelDoc.GetWorksheetName(), ex.Message), ex);
                    }

                // в разделах III.I, IV.I, V.I контрольной суммы нет, поэтому ее не проверяем
                if (new int[] { 4, 6, 8 }.Contains(sectionIndex))
                    continue;
                // проверяем итоговые суммы
                CheckXlsTotalSum(excelDoc, lastRow, sectionIndex, sectionName);
            }
        }

        // возвращает дату из Xls-отчета или по параметрам источника
        private int GetXlsReportDate(ExcelHelper excelDoc)
        {
            int refDate = -1;

            // пытаемся найти дату в диапазоне ячеек A4..A14
            for (int curRow = 4; curRow <= 14; curRow++)
            {
                string cellValue = excelDoc.GetValue(curRow, 1).Trim().ToUpper();
                if (cellValue.Contains("ПО СОСТОЯНИЮ НА"))
                {
                    refDate = CommonRoutines.ShortDateToNewDate(CommonRoutines.TrimLetters(cellValue));
                    refDate = CommonRoutines.DecrementDate(refDate);
                    break;
                }
            }

            if (refDate == -1)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                    "Не удалось найти дату отчета или она не попадает в диапазон ячеек А4..А14. " +
                    "Дата будет определена параметрами источника");
                refDate = this.DataSource.Year * 10000 + this.DataSource.Month * 100;
            }

            CheckDataSourceByDate(refDate, true);
            return refDate;
        }

        // закачивает Xls-файл
        private void PumpXlsFile(FileInfo file)
        {
            CheckArrears();
            WriteToTrace("Открытие документа: " + file.Name, TraceMessageKind.Information);
            ExcelHelper excelDoc = new ExcelHelper();
            try
            {
                excelDoc.AskToUpdateLinks = false;
                excelDoc.DisplayAlerts = false;
                excelDoc.EnableEvents = false;
                excelDoc.OpenDocument(file.FullName);

                int refDate = GetXlsReportDate(excelDoc);
                int wsCount = excelDoc.GetWorksheetsCount();
                for (int index = 1; index <= wsCount; index++)
                {
                    excelDoc.SetWorksheet(index);
                    PumpXlsSheet(excelDoc, refDate);
                }
            }
            finally
            {
                if (excelDoc != null)
                    excelDoc.CloseDocument();
            }
        }

        #endregion Работа с Excel

        #region Работа с источником

        private void CheckArrears()
        {
            // для отчетов в разрезе районов и строк классификатор Задолженность.фнс должен быть заполнен
            if ((reportType != ReportType.Svod) && (cacheArrears.Count == 0))
                throw new Exception("Не заполнен Классификатор Задолженность.ФНС - закачайте сводные отчеты");
        }

        private void CheckIncomes()
        {
            // если не заполнен классификатор Доходы.группы ФНС - предупреждение
            if (cacheIncomes.Count <= 1)
                throw new Exception("Не заполнен классификатор 'Доходы.Группы ФНС'. Данные по этому источнику закачаны не будут.");
        }

        // наименования служебных каталогов
        private const string CONST_SVOD_DIR_NAME = "Сводный";
        private const string CONST_REG_DIR_NAME = "Районы";
        private const string CONST_STR_DIR_NAME = "Строки";
        private void CheckDirectories(DirectoryInfo dir)
        {
            DirectoryInfo[] svod = dir.GetDirectories(CONST_SVOD_DIR_NAME, SearchOption.TopDirectoryOnly);
            DirectoryInfo[] str = dir.GetDirectories(CONST_STR_DIR_NAME, SearchOption.TopDirectoryOnly);
            DirectoryInfo[] reg = dir.GetDirectories(CONST_REG_DIR_NAME, SearchOption.TopDirectoryOnly);
            // Каталог "Сводный" должен присутствовать
            if (svod.GetLength(0) == 0)
            {
                dir.CreateSubdirectory(CONST_SVOD_DIR_NAME);
                throw new Exception(string.Format("Отсутствует каталог \"{0}\"", CONST_SVOD_DIR_NAME));
            }

            if (reg.GetLength(0) == 0)
            {
                dir.CreateSubdirectory(CONST_REG_DIR_NAME);
            }

            if (str.GetLength(0) == 0)
            {
                dir.CreateSubdirectory(CONST_STR_DIR_NAME);
                absentArrearsCodesStr.Clear();
            }

            // Каталоги Строки и Районы для одного месяца не могут быть заполнены одновременно
            if ((str.GetLength(0) > 0 && str[0].GetFiles().GetLength(0) > 0) &&
                (reg.GetLength(0) > 0 && reg[0].GetFiles().GetLength(0) > 0))
            {
                throw new Exception("Каталоги \"Строки\" и \"Районы\" для одного месяца не могут быть заполнены одновременно");
            }
        }

        private void PumpXlsFiles(DirectoryInfo dir)
        {
            reportType = ReportType.Svod;
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping, "Старт закачки данных сводных отчетов.");
            ProcessFilesTemplate(dir.GetDirectories(CONST_SVOD_DIR_NAME)[0], "*.xls", new ProcessFileDelegate(PumpXlsFile), false);

            if (this.Region == RegionName.YNAO)
            {
                reportType = ReportType.Region;
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping, "Старт закачки данных отчетов в разрезе районов.");
                ProcessFilesTemplate(dir.GetDirectories(CONST_REG_DIR_NAME)[0], "*.xls", new ProcessFileDelegate(PumpXlsFile), false);
            }

            reportType = ReportType.Str;
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping, "Старт закачки данных отчетов в разрезе строк.");
            ProcessFilesTemplate(dir.GetDirectories(CONST_STR_DIR_NAME)[0], "*.xls", new ProcessFileDelegate(PumpXlsFile), false);
            
        }

        #endregion Работа с источником

        #region Перекрытые методы закачки

        private void ShowAbsentArrearsCodes()
        {
            if (absentArrearsCodesStr.Count > 0)
            {
                string[] absentCodes = absentArrearsCodesStr.ConvertAll<string>(Convert.ToString).ToArray(); 
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                    string.Format("Отсутствуют данные по кодам строк: {0}.",
                    string.Join(", ", absentCodes)));
            }
        }

        private void SetClsHierarchy()
        {
            string d_Arrears_FNS6_HierarchyFileName = string.Empty;
            if (this.DataSource.Year >= 2012)
            {
                d_Arrears_FNS6_HierarchyFileName = const_d_Arrears_FNS6_HierarchyFile2012;
            }
            else if (this.DataSource.Year >= 2011)
            {
                if (this.DataSource.Month >= 8)
                    d_Arrears_FNS6_HierarchyFileName = const_d_Arrears_FNS6_HierarchyFile201108;
                else
                    d_Arrears_FNS6_HierarchyFileName = const_d_Arrears_FNS6_HierarchyFile2011;
            }
            else if (this.DataSource.Year >= 2010)
            {
                d_Arrears_FNS6_HierarchyFileName = const_d_Arrears_FNS6_HierarchyFile2010;
            }
            else if (this.DataSource.Year >= 2009)
            {
                d_Arrears_FNS6_HierarchyFileName = const_d_Arrears_FNS6_HierarchyFile2009;
            }
            else if (this.DataSource.Year >= 2006)
            {
                d_Arrears_FNS6_HierarchyFileName = const_d_Arrears_FNS6_HierarchyFile2007;
            }
            else
            {
                d_Arrears_FNS6_HierarchyFileName = const_d_Arrears_FNS6_HierarchyFile2005;
            }
            SetClsHierarchy(clsArrears, ref dsArrears, "CODE", d_Arrears_FNS6_HierarchyFileName, ClsHierarchyMode.Special);
        }

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            CheckDirectories(dir);
            if (dir.GetFiles("*.xls", SearchOption.AllDirectories).GetLength(0) == 0)
                return;

            CheckIncomes();
            PumpXlsFiles(dir);
            ShowAbsentArrearsCodes();
            UpdateData();
            SetClsHierarchy();
        }

        protected override void DirectPumpData()
        {
            PumpDataYMTemplate();
        }

        #endregion Перекрытые методы закачки

        #endregion Закачка данных

        #region Обработка данных

        protected override void ProcessDataSource()
        {
            CommonLiteSumCorrectionConfig sumCorrectionConfig = new CommonLiteSumCorrectionConfig();
            sumCorrectionConfig.fields4CorrectedSums = new string[] { "Value" };
            sumCorrectionConfig.sumFieldForCorrect = new string[] { "ValueReport" };

            CorrectFactTableSums(fctIncomesTotal, dsArrears.Tables[0], clsArrears, "RefArrears",
                sumCorrectionConfig, BlockProcessModifier.MRStandard, new string[] { "RefD", "RefYearDayUNV" }, string.Empty, string.Empty, true);
            CorrectFactTableSums(fctIncomesTotal, dsIncomes.Tables[0], clsIncomes, "RefD",
                sumCorrectionConfig, BlockProcessModifier.MRStandard, new string[] { "RefArrears", "RefYearDayUNV" }, string.Empty, string.Empty, false);
            CorrectFactTableSums(fctIncomesRegion, dsArrears.Tables[0], clsArrears, "RefArrears",
                sumCorrectionConfig, BlockProcessModifier.MRStandard, new string[] { "RefD", "RefYearDayUNV" }, "RefRegions", string.Empty, true);
            CorrectFactTableSums(fctIncomesRegion, dsIncomes.Tables[0], clsIncomes, "RefD",
                sumCorrectionConfig, BlockProcessModifier.MRStandard, new string[] { "RefArrears", "RefYearDayUNV" }, "RefRegions", string.Empty, false);
            UpdateData();
        }

        protected override void DirectProcessData()
        {
            year = -1;
            month = -1;
            GetPumpParams(ref year, ref month);
            ProcessDataSourcesTemplate(year, month, "Коррекции сумм фактов по данным источника");
        }

        #endregion Обработка данных

    }

}