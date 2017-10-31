using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.BudgetVaultPump
{
    // модуль закачки файлов DBF
    public partial class BudgetVaultPumpModule : CorrectedPumpModuleBase
    {

        #region поля

        private Database dbfDB = null;
        private DBDataAccess dbDataAccess = new DBDataAccess();
        private DirectoryInfo currentDir;
        private DBFReportKind dbfReportKind;
        private int formNo;

        #endregion поля


        #region Структуры, перечисления

        /// <summary>
        /// Тип файла отчета дбф
        /// </summary>
        protected enum DBFReportKind
        {
            /// <summary>
            /// Шаблон
            /// </summary>
            Pattern,

            /// <summary>
            /// Отчет района
            /// </summary>
            Region,

            /// <summary>
            /// Консолидированный отчет
            /// </summary>
            Consolidated,
        }

        #endregion Структуры, перечисления


        #region Общие функции DBF

        /// <summary>
        /// Возвращает все формы файлов, разрешенные к закачке
        /// </summary>
        /// <returns>Массив номеров форм</returns>
        private int[] GetAllFormNo()//
        {
            return new int[] { 2, 3, 46, 47 };
        }

        /// <summary>
        /// Возвращает номер формы файла дбф
        /// </summary>
        /// <param name="fileName">Наименование файла</param>
        /// <returns>Номер формы</returns>
        private int GetFileFormNo(FileInfo file)//
        {
            return Convert.ToInt32(file.Name.Substring(file.Name.Length - file.Extension.Length - 2, 2));
        }

        /// <summary>
        /// Формирует строку ограничения по списку кодов листа
        /// </summary>
        /// <param name="kl">Массив кодов листа.
        /// Формат элементов массива: "code" - выбираются коды, равные указанному;
        /// "code1;code2" - выбираются указанные коды;
        /// "code1..code2" - выбираются коды из указанного диапазона</param>
        /// <param name="fieldName">Поле, по которому строится ограничение</param>
        /// <returns>Строка ограничения</returns>
        private string GetConstrByKlList(string kl, string fieldName)//
        {
            if (kl == string.Empty) 
                return string.Empty;
            string result = string.Empty;
            string[] values = kl.Split(';');
            int count = values.GetLength(0);
            for (int i = 0; i < count; i++)
            {
                string[] intervals = values[i].Split(new string[] { ".." }, StringSplitOptions.None);
                if (intervals.GetLength(0) == 1)
                    result += string.Format(" or (KL = {0})", intervals[0]);
                else
                    result += string.Format(" or (KL >= {0} and KL <= {1})", intervals[0], intervals[1]);
            }
            if (result != string.Empty) 
                result = result.Remove(0, 3).Trim();
            return result;
        }

        /// <summary>
        /// Определяет тип отчета дбф
        /// </summary>
        /// <param name="fileName">Наименование файла</param>
        /// <returns>Тип отчета</returns>
        private DBFReportKind GetDBFReportKind(string fileName)
        {
            if (fileName.ToUpper().StartsWith("SS")) 
                return DBFReportKind.Pattern;
            else if (fileName.ToUpper().StartsWith("SV")) 
                return DBFReportKind.Consolidated;
            else return DBFReportKind.Region;
        }//

        /// <summary>
        /// Проверяет название dbf-файла на соответствие источнику и т.п.
        /// </summary>
        /// <param name="fileName">Название файла</param>
        private bool CheckDBFFileName(string fileName)//
        {
            string str = fileName.ToUpper();
            if (str.StartsWith("MBUD") ||  str.StartsWith("MFORM") || str.StartsWith("MOKRUG"))
                return false;
            try
            {
                string reportYear = string.Empty;
                switch (dbfReportKind)
                {
                    case DBFReportKind.Consolidated:
                    case DBFReportKind.Pattern:
                        reportYear = fileName.Substring(2, 2);
                        break;
                    case DBFReportKind.Region:
                        reportYear = fileName.Substring(0, 2);
                        break;
                }
                if (Convert.ToInt32(reportYear) != this.DataSource.Year % 100)
                    throw new Exception("Год в названии файла не соответствует источнику.");
                return true;
            }
            catch (Exception ex)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError, string.Format("Ошибка при проверке названия файла {0}", fileName), ex);
                return false;
            }
        }

        /// <summary>
        /// Загружает dbf-файлы
        /// </summary>
        /// <param name="filesList">Список файлов</param>
        /// <param name="filesRepList">Список файлов отчетов</param>
        /// <param name="filesPtrnList">Список шаблонов</param>
        private void LoadDBFFiles(FileInfo[] filesList, out FileInfo[] filesRepList, out FileInfo[] filesPtrnList)//
        {
            filesRepList = new FileInfo[0];
            filesPtrnList = new FileInfo[0];
            for (int i = 0; i < filesList.GetLength(0); i++)
            {
                if (!File.Exists(filesList[i].FullName)) 
                    continue;
                dbfReportKind = GetDBFReportKind(filesList[i].Name);
                if (!CheckDBFFileName(filesList[i].Name))
                {
                    dbfFilesCount--;
                    continue;
                }
                switch (dbfReportKind)
                {
                    case DBFReportKind.Pattern:
                        filesPtrnList = (FileInfo[])CommonRoutines.RedimArray(filesPtrnList, filesPtrnList.GetLength(0) + 1);
                        filesPtrnList[filesPtrnList.GetLength(0) - 1] = filesList[i];
                        dbfFilesCount--;
                        break;
                    case DBFReportKind.Consolidated:
                    case DBFReportKind.Region:
                        filesRepList = (FileInfo[])CommonRoutines.RedimArray(filesRepList, filesRepList.GetLength(0) + 1);
                        filesRepList[filesRepList.GetLength(0) - 1] = filesList[i];
                        break;
                }
            }
        }

        /// <summary>
        /// Проверяет все файлы в каталоге на соответствие источнику, наличие шаблонов и т.п.
        /// </summary>
        /// <param name="dir">Каталог</param>
        private void CheckDBFFilesInDir(DirectoryInfo dir)
        {
            if (dir.GetFiles("MBUD.DBF", SearchOption.AllDirectories).GetLength(0) == 0)
                throw new PumpDataFailedException("Отсутствует справочник видов бюджетов.");
            if (dir.GetFiles("MOKRUG.DBF", SearchOption.AllDirectories).GetLength(0) == 0)
                throw new PumpDataFailedException("Отсутствует справочник территориальных образований.");
        }//

        /// <summary>
        /// Устанавливает значение множителя для сумм в зависимости от даты
        /// </summary>
        private void ConfigureDbfParams()
        {
            // Для закачки отчетов до 2005 переводить в рубли.
            sumFactor = 1000;
        }//

        /// <summary>
        /// Переустанавливает соединение с источником, используя указанный драйвер
        /// </summary>
        /// <param name="driver">Драйвер</param>
        private void ReconnectToDbfDataSource(ODBCDriverName driver)
        {
            dbDataAccess.ConnectToDataSource(ref dbfDB, currentDir.FullName, driver);
        }//

        #endregion Общие функции DBF


        #region Закачка данных

        #region Функции закачки шаблона

        private void PumpClsForm2(int kl, int kst, string kbk, string restrictedKbk, string n2)
        {
            // Доходы (КД)
            if (((this.DataSource.Year == 2001 || this.DataSource.Year == 2002) && kl >= 3 && kl <= 9) ||
                ((this.DataSource.Year == 2003 || this.DataSource.Year == 2004) && kl >= 3 && kl <= 16))
                if (ToPumpBlock(Block.bIncomes))
                    PumpCachedRow(kdCache, dsKD.Tables[0], clsKD, restrictedKbk,
                        new object[] { "CODESTR", restrictedKbk, "NAME", n2, "KL", kl, "KST", kst });
            // Источники финансирования (КИФ)
            if (((this.DataSource.Year == 2001 || this.DataSource.Year == 2002) && kl >= 10 && kl <= 12) ||
                ((this.DataSource.Year == 2003 || this.DataSource.Year == 2004) && kl >= 17 && kl <= 20))
                if (ToPumpBlock(Block.bFinSources))
                    PumpCachedRow(kifCache, dsKIF2004.Tables[0], clsKIF2004, kbk,
                        new object[] { "CODESTR", kbk, "NAME", n2, "KL", kl, "KST", kst });
            // Расходы
            if ((this.DataSource.Year == 2001 && kl >= 13 && kl <= 189) ||
                (this.DataSource.Year == 2002 && kl >= 13 && kl <= 190) ||
                (this.DataSource.Year == 2003 && kl >= 21 && kl <= 216) ||
                (this.DataSource.Year == 2004 && kl >= 21 && kl <= 259))
            {
                if (ToPumpBlock(Block.bOutcomes))
                {
                    // ФКР
                    string code = kbk.Substring(0, 4).TrimStart('0');
                    if (kbk.EndsWith("000000000000000") && kbk != "7980000000000000000" && code != string.Empty)
                        PumpCachedRow(fkrCache, dsFKR.Tables[0], clsFKR, code, new object[] { "CODE", code, "NAME", n2 });
                    // КЦСР
                    code = (kbk.Substring(7, 3) + "0000").TrimStart('0');
                    if ((kbk.EndsWith("000000000")) && (code != "0000000") && (code != string.Empty))
                        PumpCachedRow(kcsrCache, dsKCSR.Tables[0], clsKCSR, code, new object[] { "CODE", code, "NAME", n2 });
                    // КВР
                    code = kbk.Substring(10, 3).TrimStart('0');
                    if ((kbk.EndsWith("000000")) && (code != string.Empty))
                        PumpCachedRow(kvrCache, dsKVR.Tables[0], clsKVR, code, new object[] { "CODE", code, "NAME", n2 });
                    // ЭКР
                    code = kbk.Substring(13, 6).TrimStart('0');
                    if (code != string.Empty)
                        PumpCachedRow(ekrCache, dsEKR.Tables[0], clsEKR, code, new object[] { "CODESTR", code, "NAME", n2 });
                }
            }
        }

        private void PumpClsForm46(int kl, int kst, string kbk, string restrictedKbk, string n2)
        {
            // Доходы (КД)
            if ((this.DataSource.Year == 2001 && ((kl >= 194 && kl <= 198) || (kl == 199 && kst >= 1 && kst <= 2))) ||
                (this.DataSource.Year == 2002 && ((kl >= 195 && kl <= 199) || (kl == 200 && kst >= 1 && kst <= 2))) ||
                (this.DataSource.Year == 2003 && kl >= 222 && kl <= 226) ||
                (this.DataSource.Year == 2004 && kl >= 265 && kl <= 269))
                    if (ToPumpBlock(Block.bIncomes))
                        PumpCachedRow(kdCache, dsKD.Tables[0], clsKD, restrictedKbk,
                            new object[] { "CODESTR", restrictedKbk, "NAME", n2, "KL", kl, "KST", kst });
            // Источники финансирования (КИФ)
            if ((this.DataSource.Year == 2001 && ((kl == 199 && kst != 1 && kst != 2) || (kl >= 200 && kl <= 201))) ||
                (this.DataSource.Year == 2002 && ((kl == 200 && kst != 1 && kst != 2) || (kl >= 201 && kl <= 202))) ||
                (this.DataSource.Year == 2003 && kl >= 227 && kl <= 231) ||
                (this.DataSource.Year == 2004 && kl >= 270 && kl <= 274))
                    if (ToPumpBlock(Block.bFinSources))
                        PumpCachedRow(kifCache, dsKIF2004.Tables[0], clsKIF2004, kbk,
                            new object[] { "CODESTR", kbk, "NAME", n2, "KL", kl, "KST", kst });
        }

        private string GetEKRCode(int kl, int kst)
        {
            string code = string.Empty;
            switch (this.DataSource.Year)
            {
                case 2001:
                    if (kl != 203)
                        break;
                    if (kst == 7)
                        code = "110100";
                    else if (kst == 8)
                        code = "110200";
                    else if (kst == 9)
                        code = "240000";
                    else if (kst == 10)
                        code = "240100";
                    else if (kst == 11)
                        code = "240200";
                    break;
                case 2002:
                    if (kl != 204)
                        break;
                    if (kst == 9)
                        code = "110100";
                    else if (kst == 10)
                        code = "110200";
                    else if (kst == 11)
                        code = "240000";
                    else if (kst == 12)
                        code = "240100";
                    else if (kst == 13)
                        code = "240200";
                    break;
                case 2003:
                    if (kl != 233)
                        break;
                    if (kst == 10)
                        code = "110100";
                    else if (kst == 11)
                        code = "110200";
                    else if (kst == 12)
                        code = "240000";
                    else if (kst == 13)
                        code = "240100";
                    else if (kst == 14)
                        code = "240200";
                    break;
            }
            return code;
        }

        private void PumpClsForm47(int kl, int kst, string kbk, string restrictedKbk, string n2)
        {
            if (!ToPumpBlock(Block.bOutcomes))
                return;
            // ФКР
            string code = kbk.Substring(0, 4).TrimStart('0'); 
            if (kbk.EndsWith("000000000000000") && kbk != "7980000000000000000" && code != string.Empty)
                PumpCachedRow(fkrCache, dsFKR.Tables[0], clsFKR, code, new object[] { "CODE", code, "NAME", n2 });
            // ЭКР
            code = kbk.Substring(13, 6).TrimStart('0');
            if (formNo == 47)
                code = GetEKRCode(kl, kst);
            if (code != string.Empty)
                PumpCachedRow(ekrCache, dsEKR.Tables[0], clsEKR, code, new object[] { "CODESTR", code, "NAME", n2 });
        }

        private void PumpClsForm3(int kl, int kst, string kbk, string restrictedKbk, string n2)
        {
            if (!ToPumpBlock(Block.bNets))
                return;
            // сети (КСШК)
            string code = kbk.Substring(kbk.Length - 3, 3);
            PumpCachedRow(kshkCache, dsKSHK.Tables[0], clsKSHK, code, new object[] { "CODE", code, "NAME", n2 });
        }

        /// <summary>
        /// Закачивает классификатор из шаблона
        /// </summary>
        /// <param name="fileFormNo">Номер формы из названия файла</param>
        /// <param name="kl">Поле KL</param>
        /// <param name="kst">Поле KST</param>
        /// <param name="kbk">Поле KBK</param>
        /// <param name="n2">Поле N2</param>
        /// <returns>ИД классификатора</returns>
        private int PumpClsFromPattern(int kl, int kst, string kbk, string n2)//
        {
            int id = -1; string code = string.Empty;
            kbk = kbk.Replace(" ", "");
            string restrictedKbk = kbk.TrimStart('0').PadRight(1, '0');
            switch (formNo)
            {
                case 2:
                    PumpClsForm2(kl, kst, kbk, restrictedKbk, n2);
                    break;
                case 46:
                    PumpClsForm46(kl, kst, kbk, restrictedKbk, n2);
                    break;
                case 47:
                    PumpClsForm47(kl, kst, kbk, restrictedKbk, n2);
                    break;
                case 3:
                    PumpClsForm3(kl, kst, kbk, restrictedKbk, n2);
                    break;
            }
            return id;
        }

        #endregion Функции закачки шаблона

        #region Функции закачки блоков

        private void PumpDBFRow(DataTable factTable, DataRow row, object[] fieldValuesMapping, object[] clsValuesMapping, object[] refsMapping)
        {
            bool zeroSums = true;
            int count = fieldValuesMapping.GetLength(0);
            for (int i = 0; i < count; i += 2)
            {
                double sum = GetDoubleCellValue(row, fieldValuesMapping[i + 1].ToString(), 0) * sumFactor;
                fieldValuesMapping[i + 1] = sum.ToString();
                if (sum != 0)
                    zeroSums = false;
            }
            if (!zeroSums)
                PumpRow(factTable, (object[])CommonRoutines.ConcatArrays(fieldValuesMapping, clsValuesMapping, refsMapping));
        }

        private void ProcessDBFRow(DataTable factTable, DataRow row, object[] clsValuesMapping, int regionID, int budgetLevel)//
        {
            switch (formNo)
            {
                case 2:
                    PumpDBFRow(factTable, row, new object[] { "ASSIGNEDREPORT", "P1" }, clsValuesMapping,
                        new object[] { "REFYEARDAYUNV", periodID[0], "REFBDGTLEVELS", 2, "REFREGIONS", regionID });
                    PumpDBFRow(factTable, row, new object[] { "ASSIGNEDREPORT", "P2" }, clsValuesMapping,
                        new object[] { "REFYEARDAYUNV", periodID[0], "REFBDGTLEVELS", 3, "REFREGIONS", regionID });
                    PumpDBFRow(factTable, row, new object[] { "ASSIGNEDREPORT", "P3" }, clsValuesMapping,
                        new object[] { "REFYEARDAYUNV", periodID[0], "REFBDGTLEVELS", 7, "REFREGIONS", regionID });
                    break;
                case 46:
                case 47:
                    PumpDBFRow(factTable, row, new object[] { "ASSIGNEDREPORT", "P2" }, clsValuesMapping,
                        new object[] { "REFYEARDAYUNV", periodID[0], "REFBDGTLEVELS", budgetLevel, "REFREGIONS", regionID });
                    PumpDBFRow(factTable, row, new object[] { "ASSIGNEDREPORT", "P3" }, clsValuesMapping,
                        new object[] { "REFYEARDAYUNV", periodID[1], "REFBDGTLEVELS", budgetLevel, "REFREGIONS", regionID });
                    PumpDBFRow(factTable, row, new object[] { "ASSIGNEDREPORT", "P4" }, clsValuesMapping,
                        new object[] { "REFYEARDAYUNV", periodID[2], "REFBDGTLEVELS", budgetLevel, "REFREGIONS", regionID });
                    PumpDBFRow(factTable, row, new object[] { "ASSIGNEDREPORT", "P5" }, clsValuesMapping,
                        new object[] { "REFYEARDAYUNV", periodID[3], "REFBDGTLEVELS", budgetLevel, "REFREGIONS", regionID });
                    break;
                case 3:
                    PumpDBFRow(factTable, row, new object[] { "BEGYEAR", "P1", "ENDYEAR", "P2", "MIDYEAR", "P3" }, clsValuesMapping,
                        new object[] { "REFYEARDAYUNV", periodID[0], "REFREGIONS", regionID });
                    break;
            }
        }

        private int GetRegionRef(DataRow row, ref int budgetLevel)
        {
            int kvb = GetIntCellValue(row, "KVB", 0);
            int org = GetIntCellValue(row, "ORG", 0);
            string code = GetDBFRegionCode(kvb.ToString(), org.ToString());
            string name = string.Empty;
            foreach (KeyValuePair<string, int> item in regionCache)
                if (item.Key.ToString().Split('|')[0] == code)
                {
                    name = item.Key.ToString().Split('|')[1];
                    break;
                }
            string regKey = code + "|" + name;
            switch (dbfReportKind)
            {
                case DBFReportKind.Region:
                    if ((kvb == 1 || kvb == 2) && org == 900)
                        budgetLevel = 3;
                    else
                        budgetLevel = 7;
                    return FindCachedRow(regionCache, regKey, -1);
                default:
                    budgetLevel = 2;
                    return FindCachedRow(regionCache, "0000000001|Консолидированный отчет субъекта", -1);
            }
        }

        private string[] GetCacheKey(string kbk)
        {
            sumFactor = 1000;
            string[] cacheKeys = new string[] { };
            switch (block)
            {
                case Block.bOutcomes:
                    cacheKeys = (string[])CommonRoutines.ConcatArrays(cacheKeys,
                        new string[] { kbk.Substring(13, 6).TrimStart('0').PadLeft(1, '0'), string.Empty, 
                            (kbk.Substring(7, 3) + "0000").TrimStart('0').PadLeft(1, '0'), 
                            kbk.Substring(0, 4).TrimStart('0').PadLeft(1, '0'), kbk.Substring(10, 3).TrimStart('0').PadLeft(1, '0') });
                    break;
                case Block.bFinSources:
                    cacheKeys = (string[])CommonRoutines.ConcatArrays(cacheKeys, new string[] { kbk });
                    break;
                case Block.bNets:
                    string kshk = kbk.Substring(kbk.Length - 3, 3).PadLeft(1, '0');
                    cacheKeys = (string[])CommonRoutines.ConcatArrays(cacheKeys,
                        new string[] { kbk.Substring(0, 4).TrimStart('0').PadLeft(1, '0'), (kbk.Substring(7, 3) + "0000").TrimStart('0').PadLeft(1, '0'), 
                            kbk.Substring(10, 3).TrimStart('0').PadLeft(1, '0'), kbk.Substring(0, kbk.Length - 3).PadLeft(1, '0'), kshk });
                    // у сетей в случае ряда кодов ксшк домножать на 1000 не нужно
                    SetNetsSumFactor(Convert.ToInt32(kshk));
                    break;
                default:
                    kbk = kbk.TrimStart('0');
                    cacheKeys = (string[])CommonRoutines.ConcatArrays(cacheKeys, new string[] { kbk });
                    break;
            }
            return cacheKeys;
        }

        private void PumpDBFReportData(DataRow[] rows, IDbDataAdapter da, DataTable factTable, DataTable[] clsTables, 
            int[] nullRefsToCls, Dictionary<string, int>[] clsCaches, string[] codeExclusions, object[] clsValuesMapping)//
        {
            int rowsCount = rows.GetLength(0);
            for (int i = 0; i < rowsCount; i++)
            {
                SetProgress(rowsCount, i + 1, progressMsg, string.Format("{0}. Строка {1} из {2}", blockName, i + 1, rowsCount));
                string kbk = Convert.ToString(rows[i]["KBK"]).Trim();
                if (CheckCodeExclusion(kbk, codeExclusions)) 
                    continue;
                kbk = kbk.Replace(" ", "");
                // проверяем только для формы 2
                if ((formNo == 2) && (!CheckCode(kbk)))
                    continue;
                if (kbk != string.Empty)
                    FormCLSFromReport(kbk);
                GetClsValuesMapping(clsTables, nullRefsToCls, clsCaches, ref clsValuesMapping, GetCacheKey(kbk));
                string ekr = string.Empty;
                if (formNo == 47)
                {
                    ekr = GetEKRCode(Convert.ToInt32(rows[i]["KL"]), Convert.ToInt32(rows[i]["KST"]));
                    if (ekr != string.Empty)
                        clsValuesMapping[1] = FindCachedRow(ekrCache, ekr, nullEKR);
                }
                int budgetLevel = -1;
                int regionId = GetRegionRef(rows[i], ref budgetLevel);
                if (regionId == -1)
                    continue;
                ProcessDBFRow(factTable, rows[i], clsValuesMapping, regionId, budgetLevel);
                if (factTable.Rows.Count >= MAX_DS_RECORDS_AMOUNT)
                {
                    UpdateData();
                    ClearDataSet(da, factTable);
                }
            }
        }

        #region доходы

        private string GetIncomesKL()
        {
            string kl = string.Empty;
            switch (formNo)
            {
                case 2:
                    switch (this.DataSource.Year)
                    {
                        case 2001:
                        case 2002:
                            kl = "3..9";
                            break;
                        case 2003:
                        case 2004:
                            kl = "3..16";
                            break;
                    }
                    break;
                case 46:
                    switch (this.DataSource.Year)
                    {
                        case 2001:
                            kl = "194..199";
                            break;
                        case 2002:
                            kl = "195..200";
                            break;
                        case 2003:
                            kl = "222..226";
                            break;
                        case 2004:
                            kl = "265..269";
                            break;
                    }
                    break;
            }
            return kl;
        }

        /// <summary>
        /// Закачивает блок "Доходы"
        /// </summary>
        /// <param name="ds">Датасет с данными дбф-файла</param>
        private void PumpIncomesDBF(DataSet ds)//
        {
            if (!CommonRoutines.CheckValueEntry(formNo, new int[] { 2, 46 }))
                return;
            WriteToTrace("Старт закачки Блок \"Доходы\".", TraceMessageKind.Information);
            block = Block.bIncomes;
            blockName = "Блок \"Доходы\"";
            string kl = GetIncomesKL();
            DataRow[] rows = ds.Tables[0].Select(GetConstrByKlList(kl, "KL"));
            object[] ClsDefaultMapping = new object[] { "RefKDFOProj", nullKD };
            PumpDBFReportData(rows, daIncomes, dsIncomes.Tables[0], new DataTable[] { dsKD.Tables[0] }, 
                new int[] { nullKD }, new Dictionary<string, int>[] { kdCache }, null, ClsDefaultMapping);
            UpdateData();
            ClearDataSet(daIncomes, ref dsIncomes);
            WriteToTrace("Закачка Блок \"Доходы\" закончена.", TraceMessageKind.Information);
        }

        #endregion доходы

        #region расходы

        private string GetOutcomesKL()
        {
            string kl = string.Empty;
            if (formNo == 2)
                switch (this.DataSource.Year)
                {
                    case 2001:
                        kl = "13..189";
                        break;
                    case 2002:
                        kl = "13..190";
                        break;
                    case 2003:
                        kl = "21..216";
                        break;
                    case 2004:
                        kl = "21..259";
                        break;
                }
            return kl;
        }

        /// <summary>
        /// Закачивает блок "Расходы"
        /// </summary>
        /// <param name="ds">Датасет с данными дбф-файла</param>
        private void PumpOutcomesDBF(DataSet ds)//
        {
            if (!CommonRoutines.CheckValueEntry(formNo, new int[] { 2, 47 }))
                return;
            WriteToTrace("Старт закачки Блок \"Расходы\".", TraceMessageKind.Information);
            block = Block.bOutcomes;
            blockName = "Блок \"Расходы\"";
            string kl = GetOutcomesKL();
            DataRow[] rows = ds.Tables[0].Select(GetConstrByKlList(kl, "KL"));
            object[] ClsDefaultMapping = new object[] { "RefEKRFOProj", nullEKR, "REFKCSR", nullKCSR, "REFFKR", nullFKR, "REFKVR", nullKVR };
            PumpDBFReportData(rows, daOutcomes, dsOutcomes.Tables[0], 
                new DataTable[] { dsEKR.Tables[0], dsKCSR.Tables[0], dsFKR.Tables[0], dsKVR.Tables[0] },
                new int[] { nullEKR, nullKCSR, nullFKR, nullKVR }, 
                new Dictionary<string, int>[] { ekrCache, kcsrCache, fkrCache, kvrCache},
                new string[] { "7980000000000000000" }, ClsDefaultMapping);
            UpdateData();
            ClearDataSet(daOutcomes, ref dsOutcomes);
            WriteToTrace("Закачка Блок \"Расходы\" закончена.", TraceMessageKind.Information);
        }

        #endregion расходы

        #region дефицит профицит

        /// <summary>
        /// Закачивает блок "Дефицит Профицит"
        /// </summary>
        /// <param name="ds">Датасет с данными дбф-файла</param>
        /// <param name="file">Файл дбф (для контроля номера формы и т.д.)</param>
        private void PumpDefProfDBF(DataSet ds)//
        {
            if (!CommonRoutines.CheckValueEntry(formNo, new int[] { 2, 47 })) 
                return;
            WriteToTrace("Старт закачки Блок \"ДефицитПрофицит\".", TraceMessageKind.Information);
            block = Block.bDefProf;
            blockName = "Блок \"ДефицитПрофицит\"";
            string kl = string.Empty;
            DataRow[] rows = ds.Tables[0].Select(GetConstrByKlList(kl, "KL"));
            object[] ClsDefaultMapping = new object[] { };
            PumpDBFReportData(rows, daDefProf, dsDefProf.Tables[0], new DataTable[] { null }, new int[] { -1 }, new Dictionary<string, int>[] { null },
                              new string[] { "!7980000000000000000" }, ClsDefaultMapping);
            UpdateData();
            ClearDataSet(daDefProf, ref dsDefProf);
            WriteToTrace("Закачка Блок \"ДефицитПрофицит\" закончена.", TraceMessageKind.Information);
        }

        #endregion дефицит профицит

        #region источники финансирования

        private string GetFinSourcesKL()
        {
            string kl = string.Empty;
            switch (formNo)
            {
                case 2:
                    switch (this.DataSource.Year)
                    {
                        case 2001:
                        case 2002:
                            kl = "10..12";
                            break;
                        case 2003:
                        case 2004:
                            kl = "17..20";
                            break;
                    }
                    break;
                case 46:
                    switch (this.DataSource.Year)
                    {
                        case 2001:
                            kl = "199..201";
                            break;
                        case 2002:
                            kl = "200..202";
                            break;
                        case 2003:
                            kl = "227..231";
                            break;
                        case 2004:
                            kl = "270..274";
                            break;
                    }
                    break;
            }
            return kl;
        }

        /// <summary>
        /// Закачивает блок "Источники финансирования"
        /// </summary>
        /// <param name="ds">Датасет с данными дбф-файла</param>
        /// <param name="file">Файл дбф (для контроля номера формы и т.д.)</param>
        private void PumpFinSourcesDBF(DataSet ds)//
        {
            if (!CommonRoutines.CheckValueEntry(formNo, new int[] { 2, 46 }))
                return;
            WriteToTrace("Старт закачки Блок \"Источники финансирования\".", TraceMessageKind.Information);
            block = Block.bFinSources;
            blockName = "Блок \"Источники финансирования\"";
            string kl = GetFinSourcesKL();
            string constByKL = string.Empty;
            if ((formNo == 46) && (this.DataSource.Year == 2001))
                constByKL = "(KL >= 200 and KL <= 201) or (KL = 199 and KST <> 1 and KST <> 2)";
            else if ((formNo == 46) && (this.DataSource.Year == 2002))
                constByKL = "(KL >= 201 and KL <= 202) or (KL = 200 and KST <> 1 and KST <> 2)";
            else
                constByKL = GetConstrByKlList(kl, "KL");
            DataRow[] rows = ds.Tables[0].Select(constByKL);
            object[] ClsDefaultMapping = new object[] { "REFKIFFOPROJ2004", nullKIF2004, "REFKIF", nullKIF2005 };
            PumpDBFReportData(rows, daFinSources, dsFinSources.Tables[0], new DataTable[] { dsKIF2004.Tables[0], null }, 
                new int[] { nullKIF2004, nullKIF2005 }, new Dictionary<string, int>[] { kifCache, null }, null, ClsDefaultMapping);
            UpdateData();
            ClearDataSet(daFinSources, ref dsFinSources);
            WriteToTrace("Закачка Блок \"Источники финансирования\" закончена.", TraceMessageKind.Information);
        }

        #endregion источники финансирования

        #region сети

        /// <summary>
        /// Закачивает блок "Сеть Штаты Контингент"
        /// </summary>
        /// <param name="ds">Датасет с данными дбф-файла</param>
        /// <param name="file">Файл дбф (для контроля номера формы и т.д.)</param>
        private void PumpNetsDBF(DataSet ds)//
        {
            // должен быть заполнен субквр
            if (subKVRCache.Count <= 1)
                return;
            if (!CommonRoutines.CheckValueEntry(formNo, new int[] { 3 }))
                return;
            WriteToTrace("Старт закачки Блок \"Сеть Штаты Контингент\".", TraceMessageKind.Information);
            block = Block.bNets;
            blockName = "Блок \"Сеть Штаты Контингент\"";
            string kl = string.Empty;
            DataRow[] rows = ds.Tables[0].Select(GetConstrByKlList(kl, "KL"));
            object[] ClsDefaultMapping = new object[] { "REFFKR", nullFKR, "REFKCSR", nullKCSR, "REFKVR", nullKVR, "REFMARKS", nullSubKVR, "REFKSSHK", nullKSHK };
            PumpDBFReportData(rows, daNets, dsNets.Tables[0], new DataTable[] { dsFKR.Tables[0], dsKCSR.Tables[0], dsKVR.Tables[0], dsSubKVR.Tables[0], dsKSHK.Tables[0] },
                new int[] { nullFKR, nullKCSR, nullKVR, nullSubKVR, nullKSHK },
                new Dictionary<string, int>[] { fkrCache, kcsrCache, kvrCache, subKVRCache, kshkCache }, null, ClsDefaultMapping);
            UpdateData();
            ClearDataSet(daNets, ref dsNets);
            WriteToTrace("Закачка Блок \"Сеть Штаты Контингент\" закончена.", TraceMessageKind.Information);
        }

        #endregion сети

        #region закачка dbf отчета

        /// <summary>
        /// Формирует код района отчетов дбф по KVB и ORG
        /// </summary>
        /// <param name="kvb">KVB</param>
        /// <param name="org">ORG</param>
        /// <returns>Код района</returns>
        private string GetDBFRegionCode(string kvb, string org)//
        {
            return kvb.PadLeft(5, '0') + org.PadLeft(5, '0');
        }

        /// <summary>
        /// Закачивает район из MOKRUG.DBF
        /// </summary>
        private void PumpRegionsDBF()//
        {
            IDbDataAdapter da = null;
            DataSet ds = new DataSet();
            InitDataSet(dbfDB, ref da, ref ds, "select mo.KVB, mo.ORG, mo.N2 as MO_N2, mb.N2 as MB_N2 " +
                "from MOKRUG mo left join MBUD mb on (mo.KVB = mb.KVB)");
            bool noRegForPump = false;
            string code; string name; string regKey;
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                string kvb = Convert.ToString(ds.Tables[0].Rows[i]["KVB"]);
                string org = Convert.ToString(ds.Tables[0].Rows[i]["ORG"]);
                code = GetDBFRegionCode(kvb, org);
                name = GetStringCellValue(ds.Tables[0].Rows[i], "MO_N2", "Неуказанный район");
                regKey = code + "|" + name;
                PumpCachedRow(regionCache, dsRegions.Tables[0], clsRegions, regKey, new object[] { "CODE", code, "NAME", name, 
                    "BUDGETKIND", kvb, "BUDGETNAME", ds.Tables[0].Rows[i]["MB_N2"] });
                if (!PumpRegionForPump(code, regKey, name))
                    noRegForPump = true;
            }
            // консолидированный 
            code = "0000000001";
            name = "Консолидированный отчет субъекта";
            regKey = code + "|" + name;
            PumpCachedRow(regionCache, dsRegions.Tables[0], clsRegions, regKey,
                new object[] { "CODE", code, "NAME", name, "BUDGETKIND", "К", "BUDGETNAME", "Консолидированный бюджет" });
            if (!PumpRegionForPump(code, regKey, name))
                noRegForPump = true;
            if (noRegForPump)
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                    "Классификатор Районы.Служебный (SOURCEID {0}) имеет записи с неуказанным типом района. " +
                    "Необходимо установить значения поля \"ТипДокумента.СКИФ\" и запустить этап обработки.", regForPumpSourceID));
        }
        
        private void GetDBFData(string fileName, ref IDbDataAdapter da, ref DataSet ds)
        {
            switch (formNo)
            {
                case 2:
                case 3:
                    InitDataSet(dbfDB, ref da, ref ds, string.Format("select * from {0} where not (P1 = 0 and P2 = 0 and P3 = 0)", fileName));
                    break;
                case 46:
                case 47:
                    InitDataSet(dbfDB, ref da, ref ds, string.Format("select * from {0} where not (P2 = 0 and P3 = 0 and P4 = 0 and P5 = 0)", fileName));
                    break;
            }
        }

        private string[] GetPeriodRefs()
        {
            string[] periodID = new string[] { };
            switch (formNo)
            {
                case 2:
                case 3:
                    periodID = new string[] { this.DataSource.Year.ToString() + "0001" }; 
                    break;
                case 46:
                case 47:
                    for (int j = 1; j <= 4; j += 1)
                    {
                        // формат квартала: ГГГГ999К
                        string quarter = this.DataSource.Year.ToString() + "999" + j.ToString();
                        periodID = (string[])CommonRoutines.ConcatArrays(periodID, new string[] { quarter });
                    }
                    break;
            }
            return periodID;
        }

        /// <summary>
        /// Закачивает отчет формата DBF
        /// </summary>
        /// <param name="file">Файл отчета</param>
        private void PumpDBFReport(FileInfo file)//
        {
            IDbDataAdapter da = null;
            DataSet ds = new DataSet();
            GetDBFData(file.Name, ref da, ref ds);
            periodID = GetPeriodRefs();
            if (ToPumpBlock(Block.bIncomes))
                PumpIncomesDBF(ds);
            if (ToPumpBlock(Block.bOutcomes))
                PumpOutcomesDBF(ds);
            if (ToPumpBlock(Block.bDefProf))
                PumpDefProfDBF(ds);
            if (ToPumpBlock(Block.bFinSources))
                PumpFinSourcesDBF(ds);
            if (ToPumpBlock(Block.bNets))
                PumpNetsDBF(ds);
        }

        #endregion закачка dbf отчета

        #endregion Функции закачки блоков

        #region Общая организация закачки отчетов DBF

        /// <summary>
        /// Закачивает шаблоны дбф
        /// </summary>
        /// <param name="filesPtrnList">Список файлов шаблонов</param>
        private void PumpDBFPatterns(FileInfo[] filesPtrnList)//
        {
            if (filesPtrnList.GetLength(0) == 0)
                throw new PumpDataFailedException("Отсутствует шаблон.");
            // добавляем ссылку на "неуказанный код классификатора"  
            PumpCachedRow(kcsrCache, dsKCSR.Tables[0], clsKCSR, "0", new object[] { "CODE", "0", "NAME", "Неуказанный код классификатора" });
            nullKCSR = FindCachedRow(kcsrCache, "0", nullKCSR);
            PumpCachedRow(ekrCache, dsEKR.Tables[0], clsEKR, "0", 
                new object[] { "CODESTR", "0", "NAME", "Неуказанный код классификатора" });
            nullEKR = FindCachedRow(ekrCache, "0", nullEKR);
            PumpCachedRow(kvrCache, dsKVR.Tables[0], clsKVR, "0", new object[] { "CODE", "0", "NAME", "Неуказанный код классификатора" });
            nullKVR = FindCachedRow(kvrCache, "0", nullKVR);
            nullSubKVR = PumpOriginalRow(clsSubKVR, dsSubKVR.Tables[0], new object[] { "CodeRprt", "0", "NAME", "Неуказанный код классификатора" }, null);
            for (int i = 0; i < filesPtrnList.GetLength(0); i++)
            {
                formNo = GetFileFormNo(filesPtrnList[i]);
                if (!CommonRoutines.CheckValueEntry(formNo, GetAllFormNo()))
                    continue;
                WriteToTrace(string.Format("Старт обработки шаблона {0}.", filesPtrnList[i].Name), TraceMessageKind.Information);
                IDbDataAdapter da = null;
                DataSet ds = new DataSet();
                InitDataSet(dbfDB, ref da, ref ds, string.Format("SELECT * FROM {0}", filesPtrnList[i].Name));
                for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                {
                    int kl = GetIntCellValue(ds.Tables[0].Rows[j], "KL", 0);
                    int kst = GetIntCellValue(ds.Tables[0].Rows[j], "KST", 0);
                    string kbk = GetStringCellValue(ds.Tables[0].Rows[j], "KBK", "0");
                    string n2 = GetStringCellValue(ds.Tables[0].Rows[j], "N2", constDefaultClsName).Trim();
                    PumpClsFromPattern(kl, kst, kbk, n2);
                    SetProgress(ds.Tables[0].Rows.Count, j + 1, string.Format("Обработка шаблона {0}...", filesPtrnList[i].Name),
                        string.Format("Строка {0} из {1}", j + 1, ds.Tables[0].Rows.Count));
                }
                WriteToTrace(string.Format("Шаблон {0} обработан.", filesPtrnList[i].Name), TraceMessageKind.Information);
            }
            UpdateData();
        }

        private void PumpDBFReports(FileInfo[] filesRepList)
        {
            PumpRegionsDBF();
            // должен быть заполнен субквр
            if (subKVRCache.Count <= 1) 
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError, "Не заполнен классификатор Показатели.ФО_Свод_СубКВР, форма 3 закачана не будет");
            for (int i = 0; i < filesRepList.GetLength(0); i++)
            {
                filesCount++;
                progressMsg = string.Format("Обработка файла {0} ({1} из {2})...", filesRepList[i].Name, filesCount, dbfFilesCount);
                if (!filesRepList[i].Exists)
                    continue;
                formNo = GetFileFormNo(filesRepList[i]);
                dbfReportKind = GetDBFReportKind(filesRepList[i].Name);
                if (!CommonRoutines.CheckValueEntry(formNo, GetAllFormNo()))
                    continue;
                if ((subKVRCache.Count <= 1) && (formNo == 3))
                    continue;
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping, string.Format("Старт закачки файла {0}.", filesRepList[i].FullName));
                try
                {
                    PumpDBFReport(filesRepList[i]);
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeSuccessfullFinishFilePump, string.Format("Файл {0} успешно закачан.", filesRepList[i].FullName));
                }
                catch (ThreadAbortException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeFinishFilePumpWithError,
                        string.Format("Закачка из файла {0} закончена с ошибками", filesRepList[i].FullName), ex);
                    throw;
                }
                finally
                {
                    CollectGarbage();
                }
                UpdateData();
            }
        }

        /// <summary>
        /// Закачивает отчеты формата DBF
        /// </summary>
        /// <param name="dir">Каталог с отчетами</param>
        private void PumpDBFReports(DirectoryInfo dir)//
        {
            currentDir = dir;
            if (this.DataSource.Year < 2000 || this.DataSource.Year > 2004) 
                return;
            FileInfo[] filesList = dir.GetFiles("*.DBF", SearchOption.AllDirectories);
            if (filesList.GetLength(0) == 0) 
                return;
            ConfigureDbfParams();
            CheckDBFFilesInDir(dir);
            try
            {
                FileInfo[] filesRepList; FileInfo[] filesPtrnList;
                LoadDBFFiles(filesList, out filesRepList, out filesPtrnList);
                ReconnectToDbfDataSource(ODBCDriverName.Microsoft_dBase_Driver);
                PumpDBFPatterns(filesPtrnList);
                PumpDBFReports(filesRepList);
            }
            finally
            {
                if (dbfDB != null)
                {
                    dbfDB.Close();
                    dbfDB = null;
                }
            }
        }

        #endregion Общая организация закачки отчетов DBF

        #endregion Закачка данных

    }
}
