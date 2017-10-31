using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.UFK14Pump
{

    // УФК - 0014 - Выписка из сводного реестра поступлений и выбытий средств бюджета (закачка для Москвы)
    public partial class UFK14PumpModule : TextRepPumpModuleBase
    {

        #region Поля

        private int dateMark = -1;
        private int refDate = -1;
        private string dateStr = string.Empty;
        private string registryNumber = string.Empty;

        private string[] pdfValues;

        // сгруппированные факты по ИНН и КПП плательщика, ИНН и КПП администратора, ОКАТО, КБК и коду цели
        private Dictionary<string, FactRow> groupedFacts = null;

        #region Архивы по районам

        // путь для формирования архивов
        private string archivePath = string.Empty;
        // путь для хранения arj-архивов
        private string archiveArjPath = string.Empty;

        private string archiveDate = string.Empty;

        // коллекция архивов
        private List<Archive> archives = null;

        // сгруппированные по ОКАТО записи отчёта
        private Dictionary<string, List<ArchiveRow>> archiveRows = null;

        // кэш таблицы перекодировки "ОКАТО.УФК -> Районы.Сопоставимый"
        private Dictionary<string, string> conversionOKATO = null;

        #endregion Архивы по районам

        #endregion Поля

        #region Структуры

        // структура записи в таблице фактов
        private struct FactRow
        {
            public decimal credit;
            public decimal debit;
            public string registryNumber;
            public object codeGoal;
            public int refKd;
            public int refOrg;
            public int refAdmin;
            public int refOkato;
            public int refDate;
            public string payments;
        }

        // структура архива
        private struct Archive
        {
            public string codeAte;
            public string okato;
            public string regionName;
            public bool isDistrict;
        }

        // структура записи в архиве по районам
        private struct ArchiveRow
        {
            public string numVsr;
            public string dateVsr;
            public string nameRd;
            public string dateRd;
            public string numRd;
            public string innPl;
            public string kppPl;
            public string namePl;
            public string innAdb;
            public string kppAdb;
            public string addKlass;
            public string kbk;
            public string purpose;
            public string sumIn;
            public string sumOut;
            public string okatoTerr;
        }

        #endregion Структуры

        #region Закачка данных

        #region Работа с базой и кэшами

        private const string A_OKATO_UFK_REFREGIONS_BRIDGE_GUID = "d033b361-d53a-4923-a29f-964d5485f4d3.4af501e4-8e63-4e55-b3e6-95be2dc12aed";
        private void FillConversionOKATO()
        {
            IConversionTable conversion = this.Scheme.ConversionTables[A_OKATO_UFK_REFREGIONS_BRIDGE_GUID];
            DataSet ds = new DataSet();
            try
            {
                conversion.GetDataUpdater().Fill(ref ds);
                FillRowsCache(ref conversionOKATO, ds.Tables[0], ">Code", "<OKATO");
            }
            finally
            {
                ClearDataSet(ref ds);
            }
        }

        private void FillCachesMoskva()
        {
            if (toBuildArchives)
                FillConversionOKATO();
            FillRowsCache(ref cacheKdRow, dsKd.Tables[0], new string[] { "CodeStr" });
            FillOrgCache(new string[] { "INN", "KPP" });
            FillRowsCache(ref cacheOrgName, dsOrg.Tables[0], new string[] { "INN", "KPP", "Name" }, "|", "Id");
            FillRowsCache(ref cacheAdmin, dsAdmin.Tables[0], new string[] { "CodeStr", "KPP", "Name" }, "|", "Id");
            FillRowsCache(ref cacheOkato, dsOkato.Tables[0], "Code");
            FillRowsCache(ref cachePeriod, dsPeriod.Tables[0], "RefFKDate", "RefFODate");
            FillRowsCache(ref cachePeriodFo, dsPeriod.Tables[0], "RefFODate", "RefFKDate");
            FillRowsCache(ref cacheRegionsBridge, dsRegionsBridge.Tables[0], "ID");
            FillRowsCache(ref cacheTerritoryType, dsTerritoryType.Tables[0], "ID", "FullName");
            FillRowsCache(ref cacheEgrulName, dsEgrul.Tables[0], new string[] { "INN", "INN20" }, "|", "NameP");
        }

        private void QueryDataMoskva()
        {
            InitClsDataSet(ref daKd, ref dsKd, clsKd, false, string.Empty);
            InitClsDataSet(ref daOrg, ref dsOrg, clsOrg, false, string.Empty);
            InitClsDataSet(ref daAdmin, ref dsAdmin, clsAdmin, false, string.Empty);
            InitClsDataSet(ref daOkato, ref dsOkato, clsOkato, false, string.Empty);
            InitDataSet(ref daPeriod, ref dsPeriod, clsPeriod, string.Empty);
            // выбираем только регионы с заполненным кодом АТЕ
            InitDataSet(ref daRegionsBridge, ref dsRegionsBridge, clsRegionsBridge, "CodeStrATE >= 0");
            InitDataSet(ref daTerritoryType, ref dsTerritoryType, clsTerritoryType, string.Empty);
            InitDataSet(ref daEgrul, ref dsEgrul, clsEgrul, "Last = 1");
            InitFactDataSet(ref daFactUFK14Dirty, ref dsFactUFK14Dirty, fctFactUFK14Dirty);
            InitFactDataSet(ref daFactUFK14, ref dsFactUFK14, fctFactUFK14);
            FillCachesMoskva();
        }

        private void UpdateDataMoskva()
        {
            UpdateDataSet(daKd, dsKd, clsKd);
            UpdateDataSet(daOrg, dsOrg, clsOrg);
            UpdateDataSet(daAdmin, dsAdmin, clsAdmin);
            UpdateDataSet(daOkato, dsOkato, clsOkato);
            UpdateDataSet(daFactUFK14Dirty, dsFactUFK14Dirty, fctFactUFK14Dirty);
            UpdateDataSet(daFactUFK14, dsFactUFK14, fctFactUFK14);
        }

        private void PumpFinalizingMoskva()
        {
            ClearDataSet(ref dsEgrul);
            ClearDataSet(ref dsKd);
            ClearDataSet(ref dsOrg);
            ClearDataSet(ref dsAdmin);
            ClearDataSet(ref dsOkato);
            ClearDataSet(ref dsPeriod);
            ClearDataSet(ref dsRegionsBridge);
            ClearDataSet(ref dsTerritoryType);
            ClearDataSet(ref dsFactUFK14Dirty);
            ClearDataSet(ref dsFactUFK14);
        }

        #endregion Работа с базой и кэшами

        #region Общие методы

        #region Классификаторы

        private int PumpKdMoskva(string code)
        {
            code = code.Trim();
            object[] mapping = new object[] { "CodeStr", code, "Name", constDefaultClsName };
            DataRow pumpedRow = PumpCachedRow(cacheKdRow, dsKd.Tables[0], clsKd, code, mapping, false);
            return Convert.ToInt32(pumpedRow["ID"]);
        }

        // character=true, если ОКАТО=46000000000 или 10000000000, во всех остальных случаях character=false
        // используется при определении структура-неструктура на этапе обработки
        private int PumpOrgMoskva(string innStr, string kppStr, string name, bool character)
        {
            long inn = GetCorrectLongValue(innStr, 0, "ИНН плательщика");
            long kpp = GetCorrectLongValue(kppStr, 0, "КПП плательщика");
            name = name.Trim();
            if (name == string.Empty)
                name = constDefaultClsName;

            int refOrg = -1;
            object[] mapping = new object[] { "INN", inn, "KPP", kpp, "Name", name, "Character", character };
            if ((inn == 0) || (kpp == 0))
            {
                string key = string.Format("{0}|{1}|{2}", inn, kpp, name);
                refOrg = PumpCachedRow(cacheOrgName, dsOrg.Tables[0], clsOrg, key, mapping);
            }
            else
            {
                string key = string.Format("{0}|{1}", inn, kpp);
                refOrg = PumpCachedRow(cacheOrg, dsOrg.Tables[0], clsOrg, key, mapping);
            }

            if (dsOrg.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateDataSet(daOrg, dsOrg, clsOrg);
                ClearDataSet(daOrg, dsOrg.Tables[0]);
            }
            return refOrg;
        }

        #endregion Классификаторы

        #region Архивы по районам

        private void PrepareArchives()
        {
            if (!toBuildArchives)
                return;

            List<string> usedAte = new List<string>();
            foreach (DataRow region in cacheRegionsBridge.Values)
            {
                string codeAte = region["CodeStrATE"].ToString().Trim();
                if (usedAte.Contains(codeAte))
                    continue;
                usedAte.Add(codeAte);

                Archive archive = new Archive();
                archive.codeAte = codeAte;
                archive.okato = region["OKATO"].ToString().Trim();
                int terrType = Convert.ToInt32(region["RefTerrType"].ToString().PadLeft(1, '0'));
                if (cacheTerritoryType.ContainsKey(terrType))
                    archive.regionName = string.Format("{0} {1}", cacheTerritoryType[terrType], region["Name"]);
                else
                    archive.regionName = region["Name"].ToString();
                archive.isDistrict = archive.regionName.Trim().ToUpper().StartsWith("ГОРОДСКОЙ ОКРУГ");

                if (!archiveRows.ContainsKey(archive.okato))
                {
                    archiveRows.Add(archive.okato, new List<ArchiveRow>());
                }

                archives.Add(archive);
            }
            usedAte.Clear();
        }

        // перемещаем архивы в папку для архивов
        private void MoveToArchive()
        {
            string str = this.MoveFilesToArchive(
                new DirectoryInfo(string.Format("{0}arj\\", archivePath)),
                this.PumpRegistryElement.SupplierCode,
                this.PumpRegistryElement.DataCode.PadLeft(4, '0'),
                this.PumpID, this.SourceID);

            if (str != string.Empty)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                    string.Format("Ошибка при перемещении файлов в каталог архива: {0}.", str));
            }
            else
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation,
                    "Сформированы файлы по муниципальным образованиям.");
            }
        }

        // Получить расширение Excel-файла в зависимости от версии Excel
        private string GetXlsFileExtension(string version)
        {
            if (version == string.Empty)
                return "xls";
            version = version.Split(new char[] { '.' })[0].Trim().PadLeft(1, '0');
            if (Convert.ToInt32(version) > 11)
                return "xlsx";
            return "xls";
        }

        private void SaveArchiveToXls(Archive archive, List<ArchiveRow> rows)
        {
            ExcelHelper excelDoc = new ExcelHelper();
            try
            {
                excelDoc.AskToUpdateLinks = false;
                excelDoc.DisplayAlerts = false;
                string fileName = string.Format("{0}p22{1}{2}.{3}",
                    archivePath, archive.codeAte.PadRight(5, '0'),
                    archiveDate, GetXlsFileExtension(excelDoc.Version));
                string archiveName = string.Empty;
                if (archive.isDistrict)
                    archiveName = string.Format("{0}{1}{2}.arj", archiveArjPath, archive.codeAte, archiveDate);
                else
                    archiveName = string.Format("{0}p22{1}{2}.arj", archivePath, archive.codeAte.PadRight(5, '0'), archiveDate);

                excelDoc.CreateDocument();
                #region Шапка документа
                excelDoc.SetValue("A1", "budget");
                excelDoc.SetValue("B1", "num_vsr");
                excelDoc.SetValue("C1", "date_vsr");
                excelDoc.SetValue("D1", "name_rd");
                excelDoc.SetValue("E1", "date_rd");
                excelDoc.SetValue("F1", "num_rd");
                excelDoc.SetValue("G1", "inn_pl");
                excelDoc.SetValue("H1", "kpp_pl");
                excelDoc.SetValue("I1", "name_pl");
                excelDoc.SetValue("J1", "inn_adb");
                excelDoc.SetValue("K1", "kpp_adb");
                excelDoc.SetValue("L1", "okato");
                excelDoc.SetValue("M1", "add_klass");
                excelDoc.SetValue("N1", "kbk");
                excelDoc.SetValue("O1", "purpose");
                excelDoc.SetValue("P1", "sum_in");
                excelDoc.SetValue("Q1", "sum_out");
                excelDoc.SetValue("R1", "okato_terr");
                #endregion
                int currentRow = 3;
                foreach (ArchiveRow row in rows)
                {
                    #region Тело документа
                    excelDoc.SetValue(currentRow, 1, string.Concat("'", archive.regionName));
                    if (row.numVsr.Trim() != string.Empty)
                        excelDoc.SetValue(currentRow, 2, string.Concat("'", row.numVsr));
                    if (row.dateVsr.Trim() != string.Empty)
                        excelDoc.SetValue(currentRow, 3, string.Concat("'", row.dateVsr));
                    excelDoc.SetValue(currentRow, 4, string.Concat("'", row.nameRd));
                    if (row.dateRd.Trim() != string.Empty)
                        excelDoc.SetValue(currentRow, 5, string.Concat("'", row.dateRd));
                    excelDoc.SetValue(currentRow, 6, string.Concat("'", row.numRd));
                    if (row.innPl.Trim() != string.Empty)
                        excelDoc.SetValue(currentRow, 7, string.Concat("'", row.innPl));
                    if (row.kppPl.Trim() != string.Empty)
                        excelDoc.SetValue(currentRow, 8, string.Concat("'", row.kppPl));
                    if (row.namePl.Trim() != string.Empty)
                        excelDoc.SetValue(currentRow, 9, string.Concat("'", row.namePl));
                    if (row.innAdb.Trim() != string.Empty)
                        excelDoc.SetValue(currentRow, 10, string.Concat("'", row.innAdb));
                    if (row.kppAdb.Trim() != string.Empty)
                        excelDoc.SetValue(currentRow, 11, string.Concat("'", row.kppAdb));
                    excelDoc.SetValue(currentRow, 12, string.Concat("'", archive.okato));
                    if (row.addKlass.Trim() != string.Empty)
                        excelDoc.SetValue(currentRow, 13, string.Concat("'", row.addKlass));
                    excelDoc.SetValue(currentRow, 14, string.Concat("'", row.kbk));
                    if (row.purpose.Trim() != string.Empty)
                        excelDoc.SetValue(currentRow, 15, string.Concat("'", row.purpose));
                    excelDoc.SetValue(currentRow, 16, row.sumIn);
                    excelDoc.SetValue(currentRow, 17, row.sumOut);
                    excelDoc.SetValue(currentRow, 18, string.Concat("'", row.okatoTerr));
                    #endregion
                    currentRow++;
                }
                #region Ширина столбцов
                excelDoc.SetColumnWidth(1, (float)33.86);
                excelDoc.SetColumnWidth(2, (float)8.43);
                excelDoc.SetColumnWidth(3, (float)10.0);
                excelDoc.SetColumnWidth(4, (float)22.14);
                excelDoc.SetColumnWidth(5, (float)10.57);
                excelDoc.SetColumnWidth(6, (float)8.43);
                excelDoc.SetColumnWidth(7, (float)13.29);
                excelDoc.SetColumnWidth(8, (float)9.43);
                excelDoc.SetColumnWidth(9, (float)36.71);
                excelDoc.SetColumnWidth(10, (float)10.71);
                excelDoc.SetColumnWidth(11, (float)9.86);
                excelDoc.SetColumnWidth(12, (float)12.0);
                excelDoc.SetColumnWidth(13, (float)10.57);
                excelDoc.SetColumnWidth(14, (float)22.71);
                excelDoc.SetColumnWidth(15, (float)49.57);
                excelDoc.SetColumnWidth(16, (float)11.14);
                excelDoc.SetColumnWidth(17, (float)11.43);
                excelDoc.SetColumnWidth(18, (float)12.0);
                // формат столбцов с вещественными значениями
                excelDoc.SetColumnFormat(16, "0,00");
                excelDoc.SetColumnFormat(17, "0,00");
                #endregion

                excelDoc.SaveDocument(fileName);
                CommonRoutines.AddToArchiveFile(archiveName, new string[] { fileName }, ArchivatorName.Arj);
                // WriteToTrace(string.Format("Данные по району {0} успешно сохранены в архив {1}.",
                //    archive.codeAte, archiveName), TraceMessageKind.Information);
            }
            finally
            {
                if (excelDoc != null)
                    excelDoc.CloseDocument();
            }
        }

        private void SaveArchives(string workDir)
        {
            if (!toBuildArchives)
                return;

            archivePath = string.Format("{0}\\archives\\", workDir);
            if (!Directory.Exists(archivePath))
                Directory.CreateDirectory(archivePath);
            archiveArjPath = string.Format("{0}arj\\{1}\\", archivePath, dateMark);
            if (!Directory.Exists(archiveArjPath))
                Directory.CreateDirectory(archiveArjPath);

            try
            {
                foreach (Archive archive in archives)
                {
                    SaveArchiveToXls(archive, archiveRows[archive.okato]);
                }
                // группируем архивы по МО
                foreach (Archive archive in archives)
                {
                    string archiveName = string.Format("{0}{1}{2}.arj", archiveArjPath, archive.codeAte.Substring(0, 3), archiveDate);
                    string searchPattern = string.Format("p22{0}*.arj", archive.codeAte.Substring(0, 3));
                    string filesToArchive = string.Format("{0}{1}", archivePath, searchPattern);
                    string[] files = Directory.GetFiles(archivePath, searchPattern);
                    if (files.GetLength(0) > 0)
                    {
                        CommonRoutines.AddToArchiveFile(archiveName, new string[] { filesToArchive }, ArchivatorName.Arj);
                        for (int i = 0; i < files.GetLength(0); i++)
                        {
                            if (File.Exists(files[i]))
                                File.Delete(files[i]);
                        }
                    }
                }
                MoveToArchive();
            }
            finally
            {
                if (Directory.Exists(archivePath))
                    Directory.Delete(archivePath, true);
            }
        }

        #endregion Архивы по районам

        private object GetCodeGoalMoskva(string codeGoal)
        {
            if (codeGoal.Trim() == string.Empty)
                return DBNull.Value;
            return codeGoal.Trim();
        }

        private object GetDateDocMoskva(string dateDoc)
        {
            if (dateDoc.Trim() == string.Empty)
                return DBNull.Value;
            return dateDoc.Trim();
        }

        private void SaveFactRows()
        {
            UTF8Encoding encoding = new UTF8Encoding();
            foreach (FactRow row in groupedFacts.Values)
            {
                object[] mapping = new object[] {
                    "Credit", row.credit,
                    "Debit", row.debit,
                    "DateDoc", DBNull.Value,
                    "NaznPlat", DBNull.Value,
                    "NameDoc", DBNull.Value,
                    "Nomer", DBNull.Value,
                    "ElectrNomer", 0,
                    "NumberRegistry", row.registryNumber,
                    "CodeGoal", row.codeGoal,
                    "Payment", encoding.GetBytes(row.payments),
                    "RefKD", row.refKd,
                    "RefOrg", row.refOrg,
                    "RefKVSR", row.refAdmin,
                    "RefOKATO", row.refOkato,
                    "RefFX", 0,
                    "RefFKDay", row.refDate,
                    "RefYearDayUNV", GetFoDate(row.refDate)
                };

                PumpRow(dsFactUFK14Dirty.Tables[0], mapping);
                if (dsFactUFK14Dirty.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
                {
                    UpdateData();
                    ClearDataSet(daFactUFK14Dirty, ref dsFactUFK14Dirty);
                }
            }
        }

        private void PumpArchFilesMoskva(DirectoryInfo dir, ArchivatorName archivator)
        {
            FileInfo[] archFiles = null;
            if (archivator == ArchivatorName.Arj)
                archFiles = dir.GetFiles("*.arj", SearchOption.AllDirectories);
            else if (archivator == ArchivatorName.Rar)
                archFiles = dir.GetFiles("*.rar", SearchOption.AllDirectories);
            foreach (FileInfo archFile in archFiles)
            {
                if (archFile.Directory.Name.StartsWith("__"))
                    continue;
                DirectoryInfo tempDir = CommonRoutines.ExtractArchiveFileToTempDir(
                    archFile.FullName, FilesExtractingOption.SingleDirectory, archivator);
                try
                {
                    PumpFilesMoskva(tempDir);
                }
                finally
                {
                    CommonRoutines.DeleteDirectory(tempDir);
                }
            }
        }

        private void PumpFilesMoskva(DirectoryInfo dir)
        {
            // если есть распаковываем архивы
            PumpArchFilesMoskva(dir, ArchivatorName.Arj);
            PumpArchFilesMoskva(dir, ArchivatorName.Rar);
            ProcessFilesTemplate(dir, "*.xls", new ProcessFileDelegate(PumpXlsFileMoskva), false);
            ProcessFilesTemplate(dir, "*.pdf", new ProcessFileDelegate(PumpPdfFileMoskva), false);
        }

        private bool IsIncorrectOkato(string okato)
        {
            switch (okato.Trim())
            {
                case "10000000000":
                case "46000000000":
                    return true;
            }
            return false;
        }

        #endregion Общие методы

        #region Работа с Xls

        private void SetXlsKdName(string cellValue)
        {
            // выбираем код и наименование для КД.УФК из итоговой строки
            // формат строки: "Итого по коду БК XXXXXXXXXXXXXXXXXXXX (наименование доходного  источника)"
            Regex regex = new Regex(@"([\d]+)(\s*)\((.+)\)");
            if (regex.IsMatch(cellValue))
            {
                Match match = regex.Match(cellValue);
                string kdCode = match.Groups[1].Value.Trim();
                string kdName = match.Groups[3].Value.Trim();
                if (cacheKdRow.ContainsKey(kdCode))
                {
                    DataRow kdRow = cacheKdRow[kdCode];
                    if (kdName == string.Empty)
                        kdName = constDefaultClsName;
                    if (kdName.Length > 255)
                        kdName = kdName.Substring(0, 255);
                    kdRow["Name"] = kdName;
                }
            }
        }

        private void AddXlsArchiveRow(Dictionary<string, string> dataRow)
        {
            if (!toBuildArchives)
                return;

            string okato = dataRow["CodeOkato"].Trim();
            string mainOkato = string.Empty;
            if (conversionOKATO.ContainsKey(okato))
                mainOkato = conversionOKATO[okato];

            if (!archiveRows.ContainsKey(mainOkato))
            {
                if (!archiveRows.ContainsKey(okato))
                    return;
                mainOkato = okato;
            }

            ArchiveRow row = new ArchiveRow();

            row.numVsr = registryNumber;
            row.dateVsr = dateStr;
            row.nameRd = dataRow["NameDoc"];
            row.dateRd = dataRow["Date"];
            row.numRd = dataRow["Number"];
            row.innPl = dataRow["InnOrg"];
            row.kppPl = dataRow["KppOrg"];
            row.namePl = dataRow["NameOrg"];
            row.innAdb = dataRow["InnAdmin"];
            row.kppAdb = dataRow["KppAdmin"];
            row.addKlass = dataRow["CodeGoal"];
            row.kbk = dataRow["CodeKd"];
            row.purpose = dataRow["NaznPlat"];
            row.sumIn = dataRow["Credit"];
            row.sumOut = dataRow["Debit"];
            row.okatoTerr = okato;

            archiveRows[mainOkato].Add(row);
        }

        private void PumpXlsRowMoskva(Dictionary<string, string> dataRow)
        {
            AddXlsArchiveRow(dataRow);

            int refKd = PumpKdMoskva(dataRow["CodeKd"]);
            int refOrg = PumpOrgMoskva(dataRow["InnOrg"], dataRow["KppOrg"], dataRow["NameOrg"], IsIncorrectOkato(dataRow["CodeOkato"]));
            int refAdmin = PumpAdmin(dataRow["InnAdmin"], dataRow["KppAdmin"], dataRow["InnAdmin"]);
            int refOkato = PumpOkato(dataRow["CodeOkato"]);

            decimal credit = CleanFactValue(dataRow["Credit"]);
            decimal debit = CleanFactValue(dataRow["Debit"]);
            if ((credit == 0) && (debit == 0))
                return;

            string factKey = string.Format("{0}|{1}|{2}|{3}|{4}", refOrg,
                refAdmin, refOkato, refKd, CleanLongValue(dataRow["CodeGoal"]));

            if (!groupedFacts.ContainsKey(factKey))
            {
                FactRow newRow = new FactRow();
                newRow.credit = credit;
                newRow.debit = debit;
                newRow.registryNumber = registryNumber;
                newRow.codeGoal = GetCodeGoalMoskva(dataRow["CodeGoal"]);
                newRow.refKd = refKd;
                newRow.refOrg = refOrg;
                newRow.refAdmin = refAdmin;
                newRow.refOkato = refOkato;
                newRow.refDate = refDate;
                newRow.payments = string.Format("{0}; {1}; {2}; {3}; {4}; {5}.",
                    (dataRow["NameDoc"].Trim() != string.Empty ? dataRow["NameDoc"] : "-"),
                    (dataRow["Number"].Trim() != string.Empty ? dataRow["Number"] : "-"),
                    (GetDateDocMoskva(dataRow["Date"]) != DBNull.Value ? dataRow["Date"] : "-"),
                    (dataRow["NaznPlat"].Trim() != string.Empty ? dataRow["NaznPlat"] : "-"),
                    credit, debit);

                groupedFacts.Add(factKey, newRow);
                return;
            }

            FactRow updateRow = groupedFacts[factKey];
            updateRow.credit += credit;
            updateRow.debit += debit;
            string payments = string.Format("{0}; {1}; {2}; {3}; {4}; {5}.",
                (dataRow["NameDoc"].Trim() != string.Empty ? dataRow["NameDoc"] : "-"),
                (dataRow["Number"].Trim() != string.Empty ? dataRow["Number"] : "-"),
                (GetDateDocMoskva(dataRow["Date"]) != DBNull.Value ? dataRow["Date"] : "-"),
                (dataRow["NaznPlat"].Trim() != string.Empty ? dataRow["NaznPlat"] : "-"),
                credit, debit);
            updateRow.payments = string.Concat(updateRow.payments, "\n", payments);

            groupedFacts[factKey] = updateRow;
        }

        private string GetNumberRegistryMoskva(ExcelHelper excelDoc)
        {
            string cellValue = string.Empty;
            if (dateMark <= 20091101)
                cellValue = excelDoc.GetValue("J2").Trim();
            else
                cellValue = excelDoc.GetValue("I1").Trim();
            return CommonRoutines.TrimLetters(cellValue);
        }

        private void PumpXlsSheetDataMoskva(string fileName, ExcelHelper excelDoc, int worksheetIndex)
        {
            WriteToTrace(string.Format("Старт закачки данных с листа '{0}'", excelDoc.GetWorksheetName()),
                TraceMessageKind.Information);
            int firstRow = 1;
            if (worksheetIndex == 1)
            {
                firstRow = GetXlsFirstRow(excelDoc);
                // на эту дату ориентироваться при определении формата
                if (!SetRefDate(excelDoc.GetValue(3, 1).Trim(), ref dateMark, ref refDate))
                    return;
                dateStr = dateMark.ToString();
                archiveDate = string.Format("{0}{1}{2}", dateStr.Substring(6, 2),
                    dateStr.Substring(4, 2), dateStr.Substring(2, 2));
                dateStr = string.Format("{0}.{1}.{2}", dateStr.Substring(6, 2),
                    dateStr.Substring(4, 2), dateStr.Substring(0, 4));
                registryNumber = GetNumberRegistryMoskva(excelDoc);
            }

            object[] mapping = GetMapping(dateMark);
            int rowsCount = excelDoc.GetRowsCount();
            for (int curRow = firstRow; curRow <= rowsCount; curRow++)
                try
                {
                    SetProgress(rowsCount, curRow,
                        string.Format("Обработка файла {0}...", fileName),
                        string.Format("Строка {0} из {1}", curRow, rowsCount));

                    Dictionary<string, string> dataRow = GetXlsDataRow(excelDoc, curRow, mapping);
                    if (!dataRow.ContainsKey("Date"))
                        dataRow.Add("Date", string.Empty);

                    string cellValue = dataRow["Cell"].Trim();
                    if (cellValue == string.Empty)
                        continue;

                    if (IsTotalSum(cellValue))
                    {
                        SetXlsKdName(cellValue);
                        continue;
                    }

                    if (IsXlsReportEnd(cellValue))
                        return;

                    PumpXlsRowMoskva(dataRow);
                }
                catch (Exception ex)
                {
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError, string.Format(
                        "При обработке строки {0} листа {1} отчета {2} возникла ошибка ({3})",
                        curRow, excelDoc.GetWorksheetName(), fileName, ex.Message));
                    this.DataSourceProcessingResult = DataSourceProcessingResult.ProcessedWithErrors;
                }
        }

        private void PumpXlsFileMoskva(FileInfo file)
        {
            WriteToTrace("Открытие документа: " + file.Name, TraceMessageKind.Information);
            groupedFacts = new Dictionary<string, FactRow>();
            archives = new List<Archive>();
            archiveRows = new Dictionary<string, List<ArchiveRow>>();
            ExcelHelper excelDoc = new ExcelHelper();
            try
            {
                PrepareArchives();
                excelDoc.AskToUpdateLinks = true;
                excelDoc.DisplayAlerts = true;
                excelDoc.OpenDocument(file.FullName);
                int wsCount = excelDoc.GetWorksheetsCount();
                for (int index = 1; index <= wsCount; index++)
                {
                    excelDoc.SetWorksheet(index);
                    PumpXlsSheetDataMoskva(file.Name, excelDoc, index);
                }
                SaveFactRows();
                SaveArchives(file.DirectoryName);
            }
            finally
            {
                archives.Clear();
                archiveRows.Clear();
                groupedFacts.Clear();
                if (excelDoc != null)
                    excelDoc.CloseDocument();
            }
        }

        #endregion Работа с Xls

        #region Работа с Pdf

        // количество столбцов в Pdf-таблице
        private const int PDF_COLUMNS_COUNT = 11;
        // минимальное количество текстовых блоков в строке, чтобы она содержала достаточно данных
        private const int PDF_MIN_TOKENS_COUNT_AT_ROW = 5;

        private void CleanPdfValues()
        {
            for (int i = 0; i < PDF_COLUMNS_COUNT; i++)
                pdfValues[i] = string.Empty;
        }

        // получить номер столбца по координате X текстового блока
        private int GetPdfColumnIndex(float x)
        {
            if (x < 160)
                return 0;
            if (x < 250)
                return 1;
            if (x < 360)
                return 2;
            if (x < 455)
                return 3;
            if (x < 565)
                return 4;
            if (x < 660)
                return 5;
            if (x < 745)
                return 6;
            if (x < 820)
                return 7;
            if (x < 960)
                return 8;
            if (x < 1060)
                return 9;
            return 10;
        }

        private void AddPdfArchiveRow(string[] values)
        {
            if (!toBuildArchives)
                return;

            string okato = values[6].Trim();
            string mainOkato = string.Empty;
            if (conversionOKATO.ContainsKey(okato))
                mainOkato = conversionOKATO[okato];

            if (!archiveRows.ContainsKey(mainOkato))
            {
                if (!archiveRows.ContainsKey(okato))
                    return;
                mainOkato = okato;
            }

            ArchiveRow row = new ArchiveRow();

            row.numVsr = registryNumber;
            row.dateVsr = dateStr;
            row.nameRd = values[0];
            row.dateRd = string.Empty;
            row.numRd = values[1];
            row.innPl = values[2];
            row.kppPl = values[3];
            string orgKey = string.Format("{0}|{1}", row.innPl, row.kppPl);
            if (cacheEgrulName.ContainsKey(orgKey))
                row.namePl = cacheEgrulName[orgKey];
            else
                row.namePl = string.Empty;
            row.innAdb = values[4];
            row.kppAdb = values[5];
            row.addKlass = values[7];
            row.kbk = values[8];
            row.purpose = string.Empty;
            row.sumIn = values[9];
            row.sumOut = values[10];
            row.okatoTerr = okato;

            archiveRows[mainOkato].Add(row);
        }

        private void PumpPdfRowMoskva(string[] values)
        {
            AddPdfArchiveRow(values);

            int refKd = PumpKdMoskva(values[8]);
            int refOrg = PumpOrgMoskva(values[2], values[3], constDefaultClsName, IsIncorrectOkato(values[6]));
            int refAdmin = PumpAdmin(values[4], values[5], values[4]);
            int refOkato = PumpOkato(values[6]);

            decimal credit = CleanFactValue(values[9]);
            decimal debit = CleanFactValue(values[10]);
            if ((credit == 0) && (debit == 0))
                return;

            string factKey = string.Format("{0}|{1}|{2}|{3}|{4}", refOrg,
                refAdmin, refOkato, refKd, CleanLongValue(values[7]));

            if (!groupedFacts.ContainsKey(factKey))
            {
                FactRow newRow = new FactRow();
                newRow.credit = credit;
                newRow.debit = debit;
                newRow.registryNumber = registryNumber;
                newRow.codeGoal = GetCodeGoalMoskva(values[7]);
                newRow.refKd = refKd;
                newRow.refOrg = refOrg;
                newRow.refAdmin = refAdmin;
                newRow.refOkato = refOkato;
                newRow.refDate = refDate;
                newRow.payments = string.Format("{0}; {1}; -; -; {2}; {3}.",
                    (values[0] != string.Empty ? values[0] : "-"),
                    (values[1] != string.Empty ? values[1] : "-"),
                    credit, debit);

                groupedFacts.Add(factKey, newRow);
                return;
            }

            FactRow updateRow = groupedFacts[factKey];
            updateRow.credit += credit;
            updateRow.debit += debit;
            string payments = string.Format("{0}; {1}; -; -; {2}; {3}.",
                (values[0] != string.Empty ? values[0] : "-"),
                (values[1] != string.Empty ? values[1] : "-"),
                credit, debit);
            updateRow.payments = string.Concat(updateRow.payments, "\n", payments);

            groupedFacts[factKey] = updateRow;
        }

        private bool SetPdfRefDate(PdfTextToken[] tokens)
        {
            dateStr = string.Empty;
            for (int i = 0; i < (tokens.Length - 1); i++)
            {
                if (tokens[i].Text.Trim().ToUpper() == "ДАТА")
                    dateStr = tokens[i + 1].Text.Trim();
            }

            if (dateStr == string.Empty)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError, "Не удалось определить дату отчета");
                return false;
            }

            if (toBuildArchives)
            {
                string[] date = dateStr.Split(new char[] { '.' });
                archiveDate = string.Format("{0}{1}{2}", date[0].PadLeft(2, '0'),
                    date[1].PadLeft(2, '0'), date[2].Substring(2).PadLeft(2, '0'));
            }

            refDate = CommonRoutines.ShortDateToNewDate(dateStr);
            dateMark = refDate;
            if (finalOverturn)
                refDate = (this.DataSource.Year * 10000) + (12 * 100) + 32;
            if (!CheckDataSourceByDate(refDate, false))
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                    string.Format("Дата {0} не соответствует параметрам источника", refDate));
                return false;
            }
            DeleteEarlierDataByDate(refDate);
            return true;
        }

        private void PumpPdfReportMoskva(PdfReader reader, string filename)
        {
            int lastRowNumber = 0;
            bool lastRow = false;
            bool totalRow = false;
            pdfValues = new string[PDF_COLUMNS_COUNT];
            CleanPdfValues();
            while (reader.ReadNextRow())
                try
                {
                    SetProgress(reader.PagesCount, reader.CurrentPageNumber,
                        string.Format("Обработка файла {0}...", filename),
                        string.Format("Страница {0} из {1}", reader.CurrentPageNumber, reader.PagesCount));

                    // получаем номер сводного реестра
                    if (reader.IsSvodRow)
                        registryNumber = reader.CurrentRow[1].Text.Trim();

                    // получаем дату отчета
                    if (reader.IsDateRow)
                    {
                        if (!SetPdfRefDate(reader.CurrentRow))
                            return;
                    }

                    // пропускаем не нужные строки
                    if (!reader.ToPump || reader.IsSkipRow)
                        continue;

                    // количество текстовых блоков в текущей строке
                    int tokensCount = reader.CurrentRow.GetLength(0);

                    // если предыдущая строка была последней на странице,
                    // то проверяем, продолжается ли она на слеующей странице:
                    // если текущая строка полная (есть все данные в ней),
                    // то закачиваем предыдущую строку
                    if (lastRow && (tokensCount > PDF_MIN_TOKENS_COUNT_AT_ROW))
                    {
                        lastRowNumber++;
                        PumpPdfRowMoskva(pdfValues);
                        CleanPdfValues();
                        lastRowNumber = 0;
                        lastRow = false;
                    }

                    totalRow = false;
                    for (int i = 0; i < tokensCount; i++)
                    {
                        PdfTextToken token = reader.CurrentRow[i];

                        // если хотя бы один блок содержит слово "ИТОГО", то это строка с контрольной суммой
                        if (token.Text.Trim().ToUpper().StartsWith("ИТОГО"))
                        {
                            totalRow = true;
                            break;
                        }

                        // определяем номер столбца по координате блока
                        int columnIndex = GetPdfColumnIndex(token.X);
                        #region comment
                        // текст может располагаться в нескольких блоках, поэтому собираем значение из всех блоков
                        //
                        // например:
                        //
                        //  ---------------    этот текст располагается в двух блоках: "Платежное" и "поручение"
                        //  |  Платежное  |    по координате X определяем, что блоки друг под другом,
                        //  |  поручение  |    и записываем значения в одну строку
                        //  ---------------
                        //
                        #endregion
                        pdfValues[columnIndex] = string.Concat(pdfValues[columnIndex], " ", token.Text.Trim()).Trim();
                    }

                    if (!totalRow)
                    {
                        // если текущая строка последняя на странице, закачаем её попозже,
                        // т.к., возможно, она продолжается на следующей странице, и это надо проверить
                        if (reader.IsLastRowAtPage)
                        {
                            // lastRowNumber = reader.CurrentRowNumber - 2;
                            lastRow = true;
                            continue;
                        }

                        // если на текущей строке присутствуют все данные и она не последняя на странице,
                        // то просто закачиваем её
                        if (tokensCount > PDF_MIN_TOKENS_COUNT_AT_ROW)
                        {
                            lastRowNumber++;
                            PumpPdfRowMoskva(pdfValues);
                            CleanPdfValues();
                            lastRow = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    #region catch exception
                    int rowNumber = lastRowNumber;
                    int pageNumber = reader.CurrentPageNumber;
                    if (lastRow)
                    {
                        lastRowNumber = 0;
                        pageNumber = reader.CurrentPageNumber - 1;
                    }
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError, string.Format(
                        "При обработке строки {0} на странице {1} возникла ошибка ({2})",
                        rowNumber, pageNumber, ex.Message));
                    CleanPdfValues();
                    lastRow = false;
                    #endregion
                }
        }

        private void PumpPdfFileMoskva(FileInfo file)
        {
            WriteToTrace("Открытие документа: " + file.Name, TraceMessageKind.Information);
            groupedFacts = new Dictionary<string, FactRow>();
            archives = new List<Archive>();
            archiveRows = new Dictionary<string, List<ArchiveRow>>();
            try
            {
                PrepareArchives();
                PumpPdfReportMoskva(new PdfReader(file.FullName), file.Name);
                SaveFactRows();
                SaveArchives(file.DirectoryName);
            }
            finally
            {
                groupedFacts.Clear();
                archives.Clear();
                archiveRows.Clear();
            }
        }

        #endregion Работа с Pdf

        #endregion Закачка данных

        #region Обработка данных

        private void FillProcessCachesMoskva()
        {
            FillRowsCache(ref cacheEgrul, dsEgrul.Tables[0], new string[] { "INN", "INN20" });
            FillRowsCache(ref cacheEgrulName, dsEgrul.Tables[0], new string[] { "INN", "INN20" }, "|", "NameP");
            FillRowsCache(ref cacheOrgOkved, dsOrgOkved.Tables[0], "RefOrgEGRUL", "Code");
            FillRowsCache(ref cacheEgripName, dsEgrip.Tables[0], new string[] { "INN" }, "FIO");
            FillRowsCache(ref cacheEgrip, dsEgrip.Tables[0], new string[] { "INN" });
            FillRowsCache(ref cacheIpOkved, dsIpOkved.Tables[0], "RefIPEGRIP", "Code");
            FillRowsCache(ref cacheOrgBridge, dsOrgBridge.Tables[0], new string[] { "Inn", "ParentId" }, "|", "Id");
        }

        private void QueryProcessDataMoskva()
        {
            InitDataSet(ref daEgrul, ref dsEgrul, clsEgrul, "Last = 1");
            InitDataSet(ref daOrgOkved, ref dsOrgOkved, clsOrgOkved, string.Empty);
            InitDataSet(ref daEgrip, ref dsEgrip, clsEgrip, "Last = 1");
            InitDataSet(ref daIpOkved, ref dsIpOkved, clsIpOkved, string.Empty);
            InitDataSet(ref daOrgBridge, ref dsOrgBridge, clsOrgBridge, string.Format("SourceID = {0}", bridgeClsSourceID));
            FillProcessCachesMoskva();
        }

        #region Проставление ссылок на "Фиксированный.Признак структуры"

        // возвращает запрос для выборки организаций с признаком "Структура" в зависимости сервера БД
        private string GetStructureQuery(bool otherSubject)
        {
            string equalSymbol = "=";
            if (otherSubject)
                equalSymbol = "<>";

            switch (this.ServerDBMSName)
            {
                case DBMSName.Oracle:
                    return string.Format(@"
select ORG.ID
from {0} ORG left outer join {1} NP on (substr(ORG.INN, 1, 4) = NP.CHARACTERINN)
where
     ORG.SOURCEID = {2} and
     NP.ID is null and
     length(ORG.INN) = 10 and
     substr(ORG.INN, 1, 2) {3} 50 and
     ORG.CHARACTER <> 1 and
     ORG.ID not in
     (
         select F.REFORG
         from {4} F inner join {5} KD on (F.REFKD = KD.ID)
         where
             F.SOURCEID = {2} and
             KD.SOURCEID = {2} and
             substr(KD.CODESTR, 4, 10) = '1010101101'
     )
", clsOrg.FullDBName, clsNotPayer.FullDBName, this.SourceID, equalSymbol, fctFactUFK14Dirty.FullDBName, clsKd.FullDBName);

                case DBMSName.SQLServer:
                    return string.Format(@"
select ORG.ID
from {0} ORG left outer join {1} NP on (substring(cast(ORG.INN as varchar(12)), 1, 4) = NP.CHARACTERINN)
where
     ORG.SOURCEID = {2} and
     NP.ID is null and
     len(ORG.INN) = 10 and
     substring(cast(ORG.INN as varchar(12)), 1, 2) {3} 50 and
     ORG.CHARACTER <> 1 and
     ORG.ID not in
     (
         select F.REFORG
         from {4} F inner join {5} KD on (F.REFKD = KD.ID)
         where
             F.SOURCEID = {2} and
             KD.SOURCEID = {2} and
             substring(KD.CODESTR, 4, 10) = '1010101101'
     )
", clsOrg.FullDBName, clsNotPayer.FullDBName, this.SourceID, equalSymbol, fctFactUFK14Dirty.FullDBName, clsKd.FullDBName);
            }
            return string.Empty;
        }

        // проставляет признак структуры по классификатору Организации.УФК_Плательщики по стандартной процедуре
        private void SetStructureCharacterStandart()
        {
            // проставляем всем организациям признак "Не структура" (REFFX = 0)
            WriteToTrace("Проставление признака \"Не структура\"", TraceMessageKind.Information);
            string query = string.Format(@"
update {0}
set REFFX = 0
where SOURCEID = {1}
", clsOrg.FullDBName, this.SourceID);
            this.DB.ExecQuery(query, QueryResultTypes.NonQuery, new IDbDataParameter[] { });

            // признак "Структура" определяем у тех организаций, у которых:
            // 1) корректный ИНН (состоит из 10 знаков);
            // 2) первые 4 знака ИНН не встречаются в поле "Признак ИНН" классификатора "Организации.Неплательщики НП в ФБ"
            // 3) поле "Признак" не отмечено (Character = 0), см. метод PumpOrgMoskva
            //    (Character = 1 у организаций с ОКАТО 46000000000 и 10000000000)
            // 4) нет платежей "Налог на прибыль в ФБ" (по коду БК: ХХХ1010101101ХХХХХХХ)

            // если у организации с признаком "Структура" ИНН начинается с 50, то это "Структура субъекта" (REFFX = 1)
            WriteToTrace("Проставление признака \"Структура субъекта\"", TraceMessageKind.Information);
            query = string.Format(@"
update {0}
set REFFX = 1
where SOURCEID = {1} and ID in ({2})
", clsOrg.FullDBName, this.SourceID, GetStructureQuery(false));
            this.DB.ExecQuery(query, QueryResultTypes.NonQuery, new IDbDataParameter[] { });

            // если у организации с признаком "Структура" ИНН начинается НЕ с 50, то это "Структура другого субъекта" (REFFX = 2)
            WriteToTrace("Проставление признака \"Структура другого субъекта\"", TraceMessageKind.Information);
            query = string.Format(@"
update {0}
set REFFX = 2
where SOURCEID = {1} and ID in ({2})
", clsOrg.FullDBName, this.SourceID, GetStructureQuery(true));
            this.DB.ExecQuery(query, QueryResultTypes.NonQuery, new IDbDataParameter[] { });
        }

        // проставляет признак структуры по классификатору Организации.УФК_Плательщики за предыдущий год
        private void SetStructureCharacterPrevYear()
        {
            IDbDataAdapter da = null;
            DataSet ds = null;

            IDbDataAdapter daPrevYear = null;
            DataSet dsPrevYear = null;
            Dictionary<string, int> cachePrevYear = null;

            try
            {
                // получаем организации, для которых нужно проставить признак структуры
                InitDataSet(ref da, ref ds, clsOrg, string.Format("SOURCEID = {0}", this.SourceID));

                // получаем организации за предыдущий год, по которым будем проставлять признак структуры
                int prevYearSourceID = FindDataSourceID(ParamKindTypes.Year, "УФК", "0014", string.Empty, year - 1, 0,
                    string.Empty, 0, string.Empty, DataSourceNotFoundAction.CreateDataSource);
                InitDataSet(ref daPrevYear, ref dsPrevYear, clsOrg, string.Format("SOURCEID = {0}", prevYearSourceID));
                FillRowsCache(ref cachePrevYear, dsPrevYear.Tables[0], new string[] { "INN", "KPP" }, "|", "REFFX");

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    string key = string.Format("{0}|{1}", row["INN"], row["KPP"]);
                    if (cachePrevYear.ContainsKey(key))
                        row["REFFX"] = cachePrevYear[key];
                    else
                        row["REFFX"] = 0;
                }

                UpdateDataSet(da, ds, clsOrg);
            }
            finally
            {
                ClearDataSet(ref ds);
                ClearDataSet(ref dsPrevYear);
            }
        }

        // проставляет признак структуры в классификаторе Организации.УФК_Плательщики
        private void SetStructureCharacter()
        {
            WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeInformation, "Начало проставления признака структуры");
            if (year <= 0)
                return;

            // проверяем наличие данные в таблице фактов с марта текущего года
            string query = string.Format(@"
select count(*)
from {0}
where SOURCEID = {1} and REFYEARDAYUNV > {2}
", fctFactUFK14.FullDBName, this.SourceID, year * 10000 + 300);
            int count = Convert.ToInt32(this.DB.ExecQuery(query, QueryResultTypes.Scalar, new IDbDataParameter[] { }));

            // если данных с марта нет, а есть только за январь или февраль,
            // или в параметрах обработки указан месяц январь или февраль
            // то признак структуры проставляем по классификатору Организации.УФК_Плательщики за предыдущий год
            if (count.Equals(0) || (month == 1 || month == 2))
            {
                WriteToTrace(string.Format(
                    "В таблице фактов не найдено данных с марта {0} года. Признак структуры будет проставлен по предыдущему году.",
                    year), TraceMessageKind.Warning);
                SetStructureCharacterPrevYear();
            }
            else
            // если в текущем году есть данные с марта и далее,
            // то проставляем признак структуры по стандартной процедуре
            {
                SetStructureCharacterStandart();
            }

            WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeInformation, "Завершение проставления признака структуры");
        }

        #endregion

        #region Расщепление данных

        #region Расщепление поля Дебит (Списано)

        // возвращает предыдущую дату в формате ГГГГММДД
        private int GetPrevDate(int oldDate)
        {
            DateTime date = DateTime.ParseExact(Convert.ToString(oldDate),
                "yyyyMMdd", new CultureInfo("ru-RU", false)).AddDays(-1);
            return (date.Year * 10000 + date.Month * 100 + date.Day);
        }

        #region Возвраты

        // расщепляет возвраты
        // (платежи плательщиков, ИНН которых нет в клс "Организации.Перечисление по уровням бюджета")
        private void DisintegrateDebitReturns(string constraint)
        {
            WriteToTrace("Расщепление сумм \"Списано\" (возвраты)", TraceMessageKind.Information);
            this.disintDateConstraint = string.Format(@"
{0} DEBIT <> 0 and REFORG in
    (
     select ORG.ID
     from {1} ORG
     where ORG.SOURCEID = {2} and ORG.INN not in
         (
          select B.INN
          from {3} B
         )
    )
", constraint, clsOrg.FullDBName, this.SourceID, clsBudgetTransfert.FullDBName);

            // сначала расщепляет возвраты по стандартной процедуре
            DisintegrateData(fctFactUFK14Dirty, fctFactUFK14, clsKd, clsOkato, new string[] { "Debit" },
                "RefFKDay", "RefKD", "RefOKATO", disintAll);

            // потом расщепленные суммы переносим в поле Credit с противоположным знаком
            // и помечаем их как возвраты (RefOpertnTypes = 2)
            string query = string.Format(@"
update {0}
set CREDIT = -DEBIT, DEBIT = 0, REFOPERTNTYPES = 2
where {1} SOURCEID = {2} and DEBIT <> 0
", fctFactUFK14.FullDBName, constraint, this.SourceID);

            // WriteToTrace("Executing the query: " + query, TraceMessageKind.Information);
            this.DB.ExecQuery(query, QueryResultTypes.NonQuery, new IDbDataParameter[] { });

            // кроме того, день ФК нужно поменять на предыдущий
            // и, соответственно, поменять день ФО по классификатору "Период.Соответствие операционных дней"
            // для этого выберем сначала даты, которые есть в расщепленных данных
            query = string.Format(@"
select distinct REFFKDAY
from {0}
where {1} SOURCEID = {2} and REFOPERTNTYPES = 2
", fctFactUFK14.FullDBName, constraint, this.SourceID);

            // WriteToTrace("Executing the query: " + query, TraceMessageKind.Information);
            using (DataTable dt = (DataTable)this.DB.ExecQuery(query, QueryResultTypes.DataTable, new IDbDataParameter[] { }))
            {
                // затем для каждой даты вычисляем предыдущий день ФК и соответствующий ему день ФО
                foreach (DataRow row in dt.Rows)
                {
                    int oldDateFk = Convert.ToInt32(row["RefFKDay"]);
                    int newDateFk = GetPrevDate(oldDateFk);
                    int newDateFo = FindCachedRow(cachePeriod, newDateFk, newDateFk);

                    query = string.Format(@"
update {0}
set REFFKDAY = {1}, REFYEARDAYUNV = {2}
where {3} SOURCEID = {4} and REFOPERTNTYPES = 2 and REFFKDAY = {5}
", fctFactUFK14.FullDBName, newDateFk, newDateFo, constraint, this.SourceID, oldDateFk);

                    // WriteToTrace("Executing the query: " + query, TraceMessageKind.Information);
                    this.DB.ExecQuery(query, QueryResultTypes.NonQuery, new IDbDataParameter[] { });
                }
            }
        }

        #endregion

        #region Поступления

        // расщепляет поступления
        // (платежи плательщиков, ИНН которых есть в клс "Организации.Перечисление по уровням бюджета")
        private void DisintegrateDebitIncomes(string constraint)
        {
            WriteToTrace("Расщепление сумм \"Списано\" (поступления)", TraceMessageKind.Information);
            string query = string.Format(@"
select F.*, B.REFBUDGETLEVELS BUDGETLEVEL
from {0} F, {1} ORG inner join {2} B on (ORG.INN = B.INN)
where {3} F.DEBIT <> 0 and F.SOURCEID = {4} and F.REFORG = ORG.ID and ORG.SOURCEID = {4} and F.REFFX = 0
", fctFactUFK14Dirty.FullDBName, clsOrg.FullDBName, clsBudgetTransfert.FullDBName, constraint, this.SourceID);

            // WriteToTrace("Executing the query: " + query, TraceMessageKind.Information);
            // найденные суммы просто переносим в таблицу с расщеплением
            using (DataTable dt = (DataTable)this.DB.ExecQuery(query, QueryResultTypes.DataTable, new IDbDataParameter[] { }))
            {
                foreach (DataRow row in dt.Rows)
                {
                    DataRow disintRow = dsFactUFK14.Tables[0].NewRow();

                    CopyRowToRow(row, disintRow);
                    disintRow["SourceKey"] = row["ID"];
                    disintRow["RefFX"] = row["BUDGETLEVEL"];
                    disintRow["RefOpertnTypes"] = 1;

                    dsFactUFK14.Tables[0].Rows.Add(disintRow);
                    if (dsFactUFK14.Tables[0].Rows.Count >= constMaxQueryRecordsForDisint)
                    {
                        UpdateDataSet(daFactUFK14, dsFactUFK14, fctFactUFK14);
                        ClearDataSet(daFactUFK14, ref dsFactUFK14);
                    }
                }
                UpdateDataSet(daFactUFK14, dsFactUFK14, fctFactUFK14);
            }

            // помечаем данные как расщепленные
            query = string.Format(@"
update {0}
set REFFX = 1
where {1} DEBIT <> 0 and SOURCEID = {2} and REFORG in
    (
     select ORG.ID
     from {3} ORG inner join {4} B on (ORG.INN = B.INN)
     where ORG.SOURCEID = {2}
    )
", fctFactUFK14Dirty.FullDBName, constraint, this.SourceID, clsOrg.FullDBName, clsBudgetTransfert.FullDBName);

            // WriteToTrace("Executing the query: " + query, TraceMessageKind.Information);
            this.DB.ExecQuery(query, QueryResultTypes.NonQuery, new IDbDataParameter[] { });
        }

        #endregion

        // удаляет расщепленные данные по полю "Дебит"
        private void PrepareDisintDataDebit(string constraint)
        {
            string query = string.Format(
                "update {0} set REFFX = 0 where {1} DEBIT <> 0 and SOURCEID = {2}",
                fctFactUFK14Dirty.FullDBName, constraint, this.SourceID);

            if (this.StagesQueue[PumpProcessStates.PumpData].IsExecuted || disintAll)
            {
                // возвратам и внебанковским при расщеплении проставляется предыдущий день ФК, поэтому для них меняем условие
                string constraintEx = string.Empty;
                if (this.StagesQueue[PumpProcessStates.PumpData].IsExecuted)
                {
                    if (deletedDateList.Count > 0)
                    {
                        List<int> deletedDates = new List<int>();
                        foreach (int date in deletedDateList)
                            deletedDates.Add(GetPrevDate(date));
                        constraintEx = string.Format(" RefFKDay in ({0}) and ",
                            string.Join(", ", deletedDates.ConvertAll<string>(Convert.ToString).ToArray()));
                    }
                }
                else
                {
                    constraintEx = GetDateConstraint("RefYearDayUNV");
                    if (constraintEx != string.Empty)
                        constraintEx += " and ";
                }

                DeleteTableData(fctFactUFK14, -1, this.SourceID, constraint + " DEBIT <> 0 ");
                DeleteTableData(fctFactUFK14, -1, this.SourceID, constraintEx + " REFOPERTNTYPES = 2 ");
                this.DB.ExecQuery(query, QueryResultTypes.NonQuery);
            }
        }

        // расщепляет суммы "Списано"
        private void DisintegrateDebit()
        {
            WriteToTrace("Расщепление сумм \"Списано\"", TraceMessageKind.Warning);
            string constraint = GetDateConstraint("RefFKDay");
            if (constraint != string.Empty)
                constraint += " and ";

            PrepareDisintDataDebit(constraint);

            DisintegrateDebitReturns(constraint);
            DisintegrateDebitIncomes(constraint);
        }

        #endregion

        #region Расщепление поля Кредит (Зачислено)

        // расщепляет суммы "Зачислено"
        private void DisintegrateCredit()
        {
            WriteToTrace("Расщепление сумм \"Зачислено\"", TraceMessageKind.Warning);
            string constraint = GetDateConstraint("RefFKDay");
            if (constraint != string.Empty)
                constraint += " and ";
            constraint = constraint + " Credit <> 0 ";
            this.disintDateConstraint = constraint;
            DisintegrateData(fctFactUFK14Dirty, fctFactUFK14, clsKd, clsOkato, new string[] { "Credit" },
                "RefFKDay", "RefKD", "RefOKATO", disintAll);
        }

        #endregion

        private void DisintegrateMoskva()
        {
            WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeInformation, "Начало расщепления сумм");

            CheckDisintRulesCache();
            PrepareMessagesDS();
            PrepareBadOkatoCodesCache();
            PrepareRegionsForSumDisint();
            disintFlagFieldName = "RefFX";
            refBudgetLevelFieldName = "RefFX";

            DisintegrateCredit();
            DisintegrateDebit();

            UpdateProcessedData();

            WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeInformation, "Завершение расщепления сумм");
        }

        #endregion

        #region Внебанковские операции

        // возвращает ID плательщика для внебанковских операций (ИНН=7725057310 и КПП=772501001)
        // если в клс "Организации.УФК_Плательщики" нет такой записи, то добавляет ее
        private int GetRefOrgPayerForOutbank()
        {
            string query = string.Format(
                "select * from {0} where SOURCEID = {1} and INN = '7725057310' and KPP = '772501001'",
                clsOrg.FullDBName, this.SourceID);
            using (DataTable dt = (DataTable)this.DB.ExecQuery(query, QueryResultTypes.DataTable, new IDbDataParameter[] { }))
            {
                if (dt.Rows.Count > 0)
                    return Convert.ToInt32(dt.Rows[0]["ID"]);
            }

            object[] mapping = new object[] {
                "INN", "7725057310",
                "KPP", "772501001",
                "Name", constDefaultClsName,
                "OKATO", 0 };
            return PumpRow(dsOrg.Tables[0], clsOrg, mapping);
        }

        // возвращает ID администратора для внебанковских операций (ИНН=7725057310 и КПП=772501001)
        // если в клс "Администратор.УФК" нет такой записи, то добавляет ее
        private int GetRefKvsrForOutbank()
        {
            string query = string.Format(
                "select * from {0} where SOURCEID = {1} and CODESTR = '7725057310' and KPP = '772501001'",
                clsAdmin.FullDBName, this.SourceID);
            using (DataTable dt = (DataTable)this.DB.ExecQuery(query, QueryResultTypes.DataTable, new IDbDataParameter[] { }))
            {
                if (dt.Rows.Count > 0)
                    return Convert.ToInt32(dt.Rows[0]["ID"]);
            }

            object[] mapping = new object[] {
                "CodeStr", "7725057310",
                "KPP", "772501001",
                "Name", constDefaultClsName,
                "Code", 0 };
            return PumpRow(dsAdmin.Tables[0], clsAdmin, mapping);
        }

        // определяет суммы по внебанковским операциям
        private void DisintegrateDebitOutbank(string constraint)
        {
            WriteToTrace("Определение сумм по внебанковским операциям", TraceMessageKind.Information);
            int refOrgPayer = GetRefOrgPayerForOutbank();
            int refKvsr = GetRefKvsrForOutbank();

            // для определения внебанковских операций группируем суммы кредит (Credit) и дебет (Debit)
            // по КБК (RefKD), ОКАТО (RefOKATO) и уровню бюджета (RefFX)
            // при этом дата для поля дебет "Период.День ФК" (RefFKDay)
            // должна быть равна дате поля кредит "Период.День ФО" (RefYearDayUNV)
            // расщепленные записи, у которых "Фиксированный.Виды операций" (RefOpertnTypes) = 3, пропускаем
            string query = string.Format(@"
select
     F1.REFKD REFKD1, F1.REFOKATO REFOKATO1, F1.REFFX REFFX1, F1.REFYEARDAYUNV REFFODAY1, F1.SUM_CREDIT SUM_CREDIT,
     F2.REFKD REFKD2, F2.REFOKATO REFOKATO2, F2.REFFX REFFX2, F2.REFFKDAY REFFKDAY2, F2.SUM_DEBIT SUM_DEBIT
from
     (
       select REFKD, REFOKATO, REFFX, REFYEARDAYUNV, sum(CREDIT) SUM_CREDIT
       from {0}
       where {1} SOURCEID = {2} and CREDIT <> 0 and REFOPERTNTYPES <> 3
       group by REFKD, REFOKATO, REFFX, REFYEARDAYUNV
     ) F1
     full outer join
     (
       select REFKD, REFOKATO, REFFX, REFFKDAY, sum(DEBIT) SUM_DEBIT
       from {0}
       where {1} SOURCEID = {2} and DEBIT <> 0 and REFOPERTNTYPES <> 3
       group by REFKD, REFOKATO, REFFX, REFFKDAY
     ) F2
     on
     (
       F1.REFYEARDAYUNV = F2.REFFKDAY and
       F1.REFKD = F2.REFKD and
       F1.REFOKATO = F2.REFOKATO and
       F1.REFFX = F2.REFFX
     )
", fctFactUFK14.FullDBName, constraint, this.SourceID);

            // WriteToTrace("Executing the query: " + query, TraceMessageKind.Information);
            using (DataTable dt = (DataTable)this.DB.ExecQuery(query, QueryResultTypes.DataTable, new IDbDataParameter[] { }))
            {
                foreach (DataRow row in dt.Rows)
                {
                    DataRow outbankRow = dsFactUFK14.Tables[0].NewRow();

                    decimal sumCredit = 0;
                    if (!row["SUM_CREDIT"].Equals(DBNull.Value))
                        sumCredit = Convert.ToDecimal(row["SUM_CREDIT"]);
                    decimal sumDebit = 0;
                    if (!row["SUM_DEBIT"].Equals(DBNull.Value))
                        sumDebit = Convert.ToDecimal(row["SUM_DEBIT"]);
                    // нулевые разницы не записываем!
                    if ((sumDebit - sumCredit) == 0)
                        continue;

                    int dateFk = -1;
                    int dateFo = -1;
                    if (!row["REFFKDAY2"].Equals(DBNull.Value))
                    {
                        dateFk = GetPrevDate(Convert.ToInt32(row["REFFKDAY2"]));
                        dateFo = FindCachedRow(cachePeriod, dateFk, dateFk);
                    }
                    else
                    {
                        dateFo = Convert.ToInt32(row["REFFODAY1"]);
                        dateFk = FindCachedRow(cachePeriodFo, dateFo, dateFo);
                    }

                    outbankRow["PumpID"] = this.PumpID;
                    outbankRow["SourceID"] = this.SourceID;
                    outbankRow["Credit"] = sumDebit - sumCredit;
                    outbankRow["Debit"] = 0;
                    outbankRow["NaznPlat"] = "Неуказанное назначение платежа";
                    outbankRow["ElectrNomer"] = 0;
                    outbankRow["RefFKDay"] = dateFk;
                    outbankRow["RefYearDayUNV"] = dateFo;
                    outbankRow["RefOrg"] = refOrgPayer;
                    outbankRow["RefKVSR"] = refKvsr;
                    outbankRow["RefKD"] = row["REFKD1"].Equals(DBNull.Value) ? row["REFKD2"] : row["REFKD1"];
                    outbankRow["RefOKATO"] = row["REFOKATO1"].Equals(DBNull.Value) ? row["REFOKATO2"] : row["REFOKATO1"];
                    outbankRow["RefFX"] = row["REFFX1"].Equals(DBNull.Value) ? row["REFFX2"] : row["REFFX1"];
                    // Виды операций: ID = 3 «Внебанковские»
                    outbankRow["RefOpertnTypes"] = 3;

                    dsFactUFK14.Tables[0].Rows.Add(outbankRow);
                    if (dsFactUFK14.Tables[0].Rows.Count >= constMaxQueryRecordsForDisint)
                    {
                        UpdateDataSet(daFactUFK14, dsFactUFK14, fctFactUFK14);
                        ClearDataSet(daFactUFK14, ref dsFactUFK14);
                    }
                }
                UpdateDataSet(daFactUFK14, dsFactUFK14, fctFactUFK14);
            }
        }

        private void DisintegrateOutbankData()
        {
            string constraint = GetDateConstraint("RefYearDayUNV");
            if (constraint != string.Empty)
                constraint += " and ";
            DeleteTableData(fctFactUFK14, -1, this.SourceID, constraint + " REFOPERTNTYPES = 3 ");

            constraint = GetDateConstraint("RefFKDay");
            if (constraint != string.Empty)
                constraint += " and ";
            DisintegrateDebitOutbank(constraint);
        }

        #endregion

        private void ProcessDataSourceMoskva()
        {
            if (toDisintData)
            {
                DisintegrateMoskva();
            }

            if (toDisintOutbankData)
            {
                DisintegrateOutbankData();
            }

            if (toSetRefStrukt)
            {
                SetStructureCharacter();
            }
        }

        #endregion Обработка данных

    }

}
