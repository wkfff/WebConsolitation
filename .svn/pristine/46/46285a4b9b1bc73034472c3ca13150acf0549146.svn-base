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

namespace Krista.FM.Server.DataPumps.UFK15Pump
{
    // УФК_0015 РЕЕСТР К ПЛАТЕЖНОМУ ПОРУЧЕНИЮ
    public class UFK15PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // КД.УФК (d.KD.UFK)
        private IDbDataAdapter daKD;
        private DataSet dsKD;
        private IClassifier clsKD;
        private Dictionary<string, int> cacheKD = null;
        // ОКАТО.УФК (d.OKATO.UFK)
        private IDbDataAdapter daOKATO;
        private DataSet dsOKATO;
        private IClassifier clsOKATO;
        private Dictionary<string, int> cacheOKATO = null;
		// Организации.УФК_Налоговые органы
		private IDbDataAdapter daOrg;
		private DataSet dsOrg;
		private IClassifier clsOrg;
		private Dictionary<string, int> cacheOrg = null;
        // период.соответствие операционных дней (d.Date.ConversionFK)
        private IDbDataAdapter daPeriod;
        private DataSet dsPeriod;
        private IClassifier clsPeriod;
        private Dictionary<string, int> cachePeriod = null;

        #endregion Классификаторы

        #region Факты

        // Доходы.УФК_Реестр к платежному поручению (f.D.UFKListDraft)
        private IDbDataAdapter daUFK15;
        private DataSet dsUFK15;
        private IFactTable fctUFK15;

        #endregion Факты

        private ExcelHelper excelHelper = null;
        private object excelObj = null;
        private int mark = -1;
        private bool finalOverturn = false;
        private List<int> deletedDateList = null;
        private int year = -1;
        private int month = -1;

        #endregion Поля

        #region Закачка данных

        #region Работа с базой и кэшами

        protected override void QueryData()
        {
            InitClsDataSet(ref daKD, ref dsKD, clsKD, false, string.Empty);
            InitClsDataSet(ref daOKATO, ref dsOKATO, clsOKATO, false, string.Empty);
            InitClsDataSet(ref daOrg, ref dsOrg, clsOrg, false, string.Empty);
            InitDataSet(ref daPeriod, ref dsPeriod, clsPeriod, string.Empty);
            InitFactDataSet(ref daUFK15, ref dsUFK15, fctUFK15);
            FillCaches();
        }

        private void FillCaches()
        {
            FillRowsCache(ref cacheKD, dsKD.Tables[0], "CodeStr");
            FillRowsCache(ref cacheOKATO, dsOKATO.Tables[0], "Code");
            FillRowsCache(ref cacheOrg, dsOrg.Tables[0], new string[] { "Code", "Name" }, "ID");
            FillRowsCache(ref cachePeriod, dsPeriod.Tables[0], "RefFKDate", "RefFODate");
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daKD, dsKD, clsKD);
            UpdateDataSet(daOKATO, dsOKATO, clsOKATO);
            UpdateDataSet(daOrg, dsOrg, clsOrg);
            UpdateDataSet(daUFK15, dsUFK15, fctUFK15);
        }

        private const string D_DATE_CONVERSION_FK_GUID = "414c27e7-393c-4516-8b47-cf6df384569d";
        private const string D_KD_UFK_GUID = "b713e1df-5584-4e3d-a399-8828a2906971";
        private const string D_OKATO_UFK_GUID = "4ae52664-ca7c-4994-bc5e-ba982421540e";
        private const string D_ORG_IMNS_GUID = "4b02172d-c79c-4cb8-86f1-30e1ca2ae604";
        private const string F_D_UFK_LIST_DRAFT_GUID = "c59898a9-2b63-40ab-860a-5fcc545d3691";
        protected override void InitDBObjects()
        {
            clsPeriod = this.Scheme.Classifiers[D_DATE_CONVERSION_FK_GUID];
            this.UsedClassifiers = new IClassifier[] {
                    clsKD = this.Scheme.Classifiers[D_KD_UFK_GUID],
                    clsOKATO = this.Scheme.Classifiers[D_OKATO_UFK_GUID],
                    clsOrg = this.Scheme.Classifiers[D_ORG_IMNS_GUID] };
            this.UsedFacts = new IFactTable[] { fctUFK15 = this.Scheme.FactTables[F_D_UFK_LIST_DRAFT_GUID] };
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsUFK15);
            ClearDataSet(ref dsOrg);
            ClearDataSet(ref dsOKATO);
            ClearDataSet(ref dsKD);
            ClearDataSet(ref dsPeriod);
        }

        #endregion Работа с базой и кэшами

        #region Работа с экселем

        #region работа с датой отчета

        private int GetUfkRefDate(object sheet)
        {
            string date = excelHelper.GetCell(sheet, 1, 1).Value.Trim().Split(new string[] { "от" }, StringSplitOptions.None)[1].Trim();
            return CommonRoutines.ShortDateToNewDate(date);
        }

        private int GetNotUfkRefDate(string fileName)
        {
            return this.DataSource.Year * 10000 + Convert.ToInt32(fileName.Substring(6, 2)) * 100 + 
                Convert.ToInt32(fileName.Substring(4, 2)); 
        }

        private int GetDate(string fileName, object sheet)
        {
            if (finalOverturn)
                return this.DataSource.Year * 10000 + 12 * 100 + 32;
            else
            {
                if (mark == UFK_MARK)
                    return GetUfkRefDate(sheet);
                else
                    return GetNotUfkRefDate(fileName);
            }
        }

        private int GetRefDate(string fileName, object sheet)
        {
            int refDate = GetDate(fileName, sheet);
            string dateFieldName = "RefYearDayUNV";
            if (!deletedDateList.Contains(refDate) && !finalOverturn)
            {
                DeleteData(string.Format("{0} = {1} and RefSign = {2}", dateFieldName, refDate, mark),
                    string.Format("Дата отчета: {0}.", refDate));
                deletedDateList.Add(refDate);
            }
            CheckDataSourceByDate(refDate, true);
            return refDate;
        }

        #endregion работа с датой отчета

        #region функции отчетов УФК

        private const string REPORT_END_MARK = "ВСЕГО";
        private bool IsReportEnd(string value)
        {
            return value.ToUpper().StartsWith(REPORT_END_MARK);
        }

        private string GetDocNumber(object sheet)
        {
            return excelHelper.GetCell(sheet, 1, 1).Value.Trim().Split('N')[1].Split(new string[] { "от" }, StringSplitOptions.None)[0].Trim();
        }

        private const string TOTAL_ROW = "ИТОГО";
        private bool IsTotalRow(string value)
        {
            return value.ToUpper().Contains(TOTAL_ROW);
        }

        private void PumpUfkXLSRow(object sheet, int row, object[] mapping)
        {
            string kd = excelHelper.GetCell(sheet, row, 5).Value.Trim();
            if (kd == string.Empty)
                return;
            int refKD = PumpCachedRow(cacheKD, dsKD.Tables[0], clsKD, kd, new object[] { "CodeStr", kd });
            decimal sum = Convert.ToDecimal(excelHelper.GetCell(sheet, row, 6).Value.Trim());
            mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "RefKD", refKD, "Sum", sum });
            PumpRow(dsUFK15.Tables[0], mapping);
            if (dsUFK15.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daUFK15, ref dsUFK15);
            }
        }

        private const int UFK_START_ROW = 4;
        private void PumpUfkXlsSheet(object sheet, FileInfo file, int refDate)
        {
            string docNumber = GetDocNumber(sheet);
            object[] mapping = null;
            for (int curRow = UFK_START_ROW; ; curRow++)
                try
                {
                    if (IsReportEnd(excelHelper.GetCell(sheet, curRow, 1).Value.Trim()))
                        return;
                    string okato = excelHelper.GetCell(sheet, curRow, 3).Value.Trim();
                    if (okato != string.Empty)
                    {
                        string okatoName = excelHelper.GetCell(sheet, curRow, 4).Value.Trim();
                        okato = okato.TrimStart('0').PadLeft(1, '0');
                        int refOkato = PumpCachedRow(cacheOKATO, dsOKATO.Tables[0], clsOKATO, okato,
                            new object[] { "Code", okato, "Name", okatoName });
                        mapping = new object[] { "RefSign", mark, "DocNumber", docNumber, "RefYearDayUNV", refDate, "RefOKATO", refOkato };
                    }
                    string orgCode = string.Empty;
                    if ((this.Region == RegionName.Kalmykya) || (this.Region == RegionName.Krasnodar))
                    {
                        orgCode = excelHelper.GetCell(sheet, curRow, 1).Value.Trim();
                        if ((orgCode != string.Empty) && (!IsTotalRow(excelHelper.GetCell(sheet, curRow, 1).Value.Trim())))
                        {
                            string orgName = excelHelper.GetCell(sheet, curRow, 2).Value.Trim();
                            orgCode = orgCode.TrimStart('0');
                            string cacheKey = GetComplexCacheKey(new string[] { orgCode, orgName });
                            int refOrg = PumpCachedRow(cacheOrg, dsOrg.Tables[0], clsOrg, cacheKey,
                                new object[] { "Code", orgCode, "Name", orgName });
                            mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "RefOrg", refOrg });
                        }
                    }
                    if ((okato != string.Empty) && (orgCode != string.Empty)) 
                        continue;
                    if (IsTotalRow(excelHelper.GetCell(sheet, curRow, 1).Value.Trim()))
                        continue;
                    PumpUfkXLSRow(sheet, curRow, mapping);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("При обработке строки {0} отчета {1} возникла ошибка ({2})",
                        curRow, file.FullName, ex.Message), ex);
                }
        }

        #endregion функции отчетов УФК

        #region функции отчетов не УФК

        private void PumpNotUfkXLSRow(DataRow row, int refDate)
        {
            string okato = row["OKATO"].ToString();
            if ((okato == string.Empty) || (CommonRoutines.TrimNumbers(okato) != string.Empty))
                return;
            int refOkato = PumpCachedRow(cacheOKATO, dsOKATO.Tables[0], clsOKATO, okato, new object[] { "Code", okato});
            string kd = row["KD"].ToString();
            int refKD = PumpCachedRow(cacheKD, dsKD.Tables[0], clsKD, kd, new object[] { "CodeStr", kd });
            PumpRow(dsUFK15.Tables[0], new object[] { "Sum", Convert.ToDouble(row["Sum"].ToString()), "RefSign", mark, 
                "DocNumber", row["DocNumber"].ToString(), "RefYearDayUNV", refDate, "REFKD", refKD, "RefOKATO", refOkato });
            if (dsUFK15.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daUFK15, ref dsUFK15);
            }
        }

        private int GetNotUfkLastRow(object sheet, int firstRow)
        {
            for (int i = firstRow; ; i++)
            {
                string value = excelHelper.GetCell(sheet, i, 1).Value.Trim().ToUpper();
                if (value == string.Empty)
                    return i;
            }
        }

        private const int NOT_UFK_START_ROW = 2;
        private object[] NOT_UFK_MAPPING = new object[] { "OKATO", 1, "KD", 2, "Sum", 3, "DocNumber", 4 };
        private void PumpNotUfkXlsSheet(object sheet, FileInfo file, int refDate)
        {
            int lastRow = GetNotUfkLastRow(sheet, NOT_UFK_START_ROW);
            DataTable dt = excelHelper.GetSheetDataTable(sheet, NOT_UFK_START_ROW, lastRow, NOT_UFK_MAPPING);
            for (int i = 0; i < dt.Rows.Count; i++)
                PumpNotUfkXLSRow(dt.Rows[i], refDate);
        }

        #endregion функции отчетов не УФК

        private void PumpXLSFile(FileInfo file)
        {
            object workbook = excelHelper.InitWorkBook(ref excelObj, file.FullName);
            try
            {
                object sheet = excelHelper.GetSheet(workbook, 1);
                int refDate = GetRefDate(file.Name, sheet);
                if (mark == UFK_MARK)
                    PumpUfkXlsSheet(sheet, file, refDate);
                else
                    PumpNotUfkXlsSheet(sheet, file, refDate);
            }
            finally
            {
                excelHelper.CloseExcel(ref excelObj);
            }
        }

        #endregion Работа с экселем

        #region Перекрытые методы закачки

        private ArchivatorName GetArchName()
        {
            if ((this.Region == RegionName.Kalmykya) || (this.Region == RegionName.Krasnodar))
                return ArchivatorName.Rar;
            else
                return ArchivatorName.Arj;
        }

        private string GetFileMask()
        {
            if (this.Region == RegionName.Krasnodar)
                return "*.xls";
            else if (this.Region == RegionName.Kalmykya)
                return "прил*.xls";
            else
                return "MF60*.xls";
        }

        private string GetArchMask()
        {
            if ((this.Region == RegionName.Kalmykya) || (this.Region == RegionName.Krasnodar))
                return "*.rar";
            else
                return "*.arj";
        }

        private void ProcessAuxDir(DirectoryInfo sourceDir, string auxDirName)
        {
            deletedDateList = new List<int>();
            try
            {
                DirectoryInfo[] auxDir = sourceDir.GetDirectories(auxDirName, SearchOption.AllDirectories);
                if (auxDir.GetLength(0) == 0)
                {
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                      string.Format("Отсутствует каталог '{0}'.", auxDirName));
                    this.DataSourceProcessingResult = DataSourceProcessingResult.ProcessedWithErrors;
                    sourceDir.CreateSubdirectory(auxDirName);
                    return;
                }
                // закачиваем файлы XLS
                string fileMask = GetFileMask();
                ProcessFilesTemplate(auxDir[0], fileMask, new ProcessFileDelegate(PumpXLSFile), false);
                // если есть архивы, распаковываем и качаем файлы XLS
                FileInfo[] archFiles = auxDir[0].GetFiles(GetArchMask(), SearchOption.AllDirectories);
                foreach (FileInfo archFile in archFiles)
                {
                    DirectoryInfo tempDir = CommonRoutines.ExtractArchiveFileToTempDir(archFile.FullName,
                        FilesExtractingOption.SingleDirectory, GetArchName());
                    try
                    {
                        ProcessFilesTemplate(tempDir, fileMask, new ProcessFileDelegate(PumpXLSFile), false);
                    }
                    finally
                    {
                        CommonRoutines.DeleteDirectory(tempDir);
                    }
                }
            }
            finally
            {
                deletedDateList.Clear();
            }
        }

        private const int UFK_MARK = 1;
        private const int NOT_UFK_MARK = 2;
        protected override void ProcessFiles(DirectoryInfo dir)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStart, "Старт инициализации Excel.");
            excelHelper = new ExcelHelper();
            finalOverturn = Convert.ToBoolean(GetParamValueByName(this.PumpRegistryElement.ProgramConfig, "cbFinalOverturn", "False"));
            try
            {
                mark = UFK_MARK; 
                ProcessAuxDir(dir, "УФК");
                if ((this.Region != RegionName.Kalmykya) && (this.Region != RegionName.Krasnodar))
                {
                    mark = NOT_UFK_MARK;
                    ProcessAuxDir(dir, "не УФК");
                }
                UpdateData();
            }
            finally
            {
                if (excelHelper != null)
                    excelHelper.Close();
            }
        }

        protected override void DirectPumpData()
        {
            PumpDataYMTemplate();
        }

        #endregion Перекрытые методы закачки

        #endregion Закачка данных

        #region Обработка данных

        protected override void DirectProcessData()
        {
            return;
        }

        #endregion Обработка данных

    }
}
