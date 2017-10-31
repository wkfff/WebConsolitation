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

namespace Krista.FM.Server.DataPumps.UFK16Pump
{
    // УФК_0016_116Н_4_СПРАВКА ОРГАНА ФК
    public class UFK16PumpModule : CorrectedPumpModuleBase
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
        // Организации.УФК_Плательщики (d.Org.UFKPayers)
        private IDbDataAdapter daOrg;
        private DataSet dsOrg;
        private IClassifier clsOrg;
        private Dictionary<string, int> cacheOrg = null;
        // Администратор.УФК (d.KVSR.UFK)
        private IDbDataAdapter daAdm;
        private DataSet dsAdm;
        private IClassifier clsAdm;
        private Dictionary<string, int> cacheAdm = null;

        #endregion Классификаторы

        #region Факты

        // Доходы.УФК_Справка органа ФК (f.D.UFKCertificate)
        private IDbDataAdapter daUFK16;
        private DataSet dsUFK16;
        private IFactTable fctUFK16;

        #endregion Факты

        private ExcelHelper excelHelper;
        private int rowsCount = 0;
        private object excelObj = null;
        private List<int> deletedDateList = null;

        #endregion Поля

        #region Закачка данных

        #region Работа с базой и кэшами

        protected override void QueryData()
        {
            InitClsDataSet(ref daKD, ref dsKD, clsKD, false, string.Empty);
            InitClsDataSet(ref daOKATO, ref dsOKATO, clsOKATO, false, string.Empty);
            InitClsDataSet(ref daOrg, ref dsOrg, clsOrg, false, string.Empty);
            InitClsDataSet(ref daAdm, ref dsAdm, clsAdm, false, string.Empty);
            InitFactDataSet(ref daUFK16, ref dsUFK16, fctUFK16);
            FillCaches();
        }

        private void FillCaches()
        {
            FillRowsCache(ref cacheKD, dsKD.Tables[0], "CodeStr");
            FillRowsCache(ref cacheOKATO, dsOKATO.Tables[0], "Code");
            FillRowsCache(ref cacheOrg, dsOrg.Tables[0], new string[] { "INN", "KPP" }, "|", "ID");
            FillRowsCache(ref cacheAdm, dsAdm.Tables[0], new string[] { "CodeStr", "KPP" }, "|", "ID");
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daKD, dsKD, clsKD);
            UpdateDataSet(daOKATO, dsOKATO, clsOKATO);
            UpdateDataSet(daOrg, dsOrg, clsOrg);
            UpdateDataSet(daAdm, dsAdm, clsAdm);
            UpdateDataSet(daUFK16, dsUFK16, fctUFK16);
        }

        private const string D_KD_UFK_GUID = "b713e1df-5584-4e3d-a399-8828a2906971";
        private const string D_OKATO_UFK_GUID = "4ae52664-ca7c-4994-bc5e-ba982421540e";
        private const string D_ORG_GUID = "5d7f6e1d-c202-49b3-b6ad-d584616aded0";
        private const string D_ADM_GUID = "468224fd-24fa-447a-8001-66b0b3405a86";
        private const string F_D_UFK_16_GUID = "d4be0346-9e60-45a0-88e6-6d6a9a140ce2";
        protected override void InitDBObjects()
        {
            this.UsedClassifiers = new IClassifier[] {
                    clsKD = this.Scheme.Classifiers[D_KD_UFK_GUID],
                    clsOKATO = this.Scheme.Classifiers[D_OKATO_UFK_GUID],
                    clsOrg = this.Scheme.Classifiers[D_ORG_GUID], 
                    clsAdm = this.Scheme.Classifiers[D_ADM_GUID] };
            this.UsedFacts = new IFactTable[] { fctUFK16 = this.Scheme.FactTables[F_D_UFK_16_GUID] };
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsUFK16);
            ClearDataSet(ref dsOrg);
            ClearDataSet(ref dsOKATO);
            ClearDataSet(ref dsKD);
            ClearDataSet(ref dsAdm);
        }

        #endregion Работа с базой и кэшами

        #region Работа с экселем

        // возвращает количетсво строк в выбранном Excel-листе отчёта
        private int GetRowsCount(object sheet)
        {
            int emptyStrCount = 0;
            int curRow = 1;
            while (emptyStrCount < 10)
            {
                if (excelHelper.GetCell(sheet, curRow, 1).Value.Trim() == string.Empty)
                    emptyStrCount++;
                else
                    emptyStrCount = 0;
                curRow++;
            }
            return (curRow - 10);
        }

        private int GetRefDate(object sheet)
        {
            return CommonRoutines.ShortDateToNewDate(excelHelper.GetCell(sheet, 3, 14).Value);
        }

        private int PumpKd(string code)
        {
            return PumpCachedRow(cacheKD, dsKD.Tables[0], clsKD, code,
                new object[] { "CodeStr", code, "Name", constDefaultClsName });
        }

        private int PumpOkato(long code)
        {
            return PumpCachedRow(cacheOKATO, dsOKATO.Tables[0], clsKD, code.ToString(),
                new object[] { "Code", code, "Name", constDefaultClsName });
        }

        private int PumpOrg(long inn, long kpp)
        {
            string key = string.Format("{0}|{1}", inn, kpp);
            return PumpCachedRow(cacheOrg, dsOrg.Tables[0], clsOrg, key,
                new object[] { "INN", inn, "KPP", kpp, "Name", inn, "Okato", 0 });
        }

        private int PumpAdm(string inn, string kpp)
        {
            string key = string.Format("{0}|{1}", inn, kpp);
            return PumpCachedRow(cacheAdm, dsAdm.Tables[0], clsAdm, key,
                new object[] { "CodeStr", inn, "KPP", kpp, "Name", inn });
        }

        private void PumpXlsRow(DataRow row, int refDate)
        {
            int refKdDebit = PumpKd(row["Kd1"].ToString().PadLeft(1, '0'));
            int refKdCredit = PumpKd(row["Kd2"].ToString().PadLeft(1, '0'));
            int refOkato = PumpOkato(Convert.ToInt64(row["Okato"].ToString().PadLeft(1, '0')));
            int refOrg = PumpOrg(Convert.ToInt64(row["OrgInn"].ToString().PadLeft(1, '0')),
                Convert.ToInt64(row["OrgKpp"].ToString().PadLeft(1, '0')));
            int refAdm = PumpAdm(row["AdmInn"].ToString().PadLeft(1, '0'), row["AdmKpp"].ToString().PadLeft(1, '0'));
            decimal sum = Convert.ToDecimal(row["Sum"].ToString().Trim().PadLeft(1, '0'));
            if (sum == 0)
                return;
            object[] mapping = new object[] { "RefYearDayUNV", refDate, "RefKDDebit", refKdDebit, "RefKDCredit", refKdCredit, "RefOKATO", refOkato, 
                "RefOrg", refOrg, "RefKVSR", refAdm, "ForPeriod", sum, "NumberDoc", row["DocNumber"].ToString(), 
                "NumberDocExec", row["ExecDocNumber"].ToString(), "Contents", row["OperCont"].ToString() };
            PumpRow(dsUFK16.Tables[0], mapping);
            if (dsUFK16.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daUFK16, ref dsUFK16);
            }
        }

        private const int FIRST_ROW = 11;
        private object[] XLS_MAPPING = new object[] { "OrgInn", 1, "OrgKpp", 2, "OperCont", 3, "DocNumber", 4, 
            "AdmInn", 7, "AdmKpp", 8, "Okato", 9, "Kd1", 10, "Kd2", 11, "Sum", 12, "ExecDocNumber", 13 };
        private void PumpXlsSheet(object sheet, string fileName)
        {
            int refDate = GetRefDate(sheet);
            if (!deletedDateList.Contains(refDate))
            {
                DeleteData(string.Format("RefYearDayUNV = {0}", refDate), string.Format("Дата отчета: {0}.", refDate));
                deletedDateList.Add(refDate);
            }
            DataTable dt = excelHelper.GetSheetDataTable(sheet, FIRST_ROW, rowsCount - 1, XLS_MAPPING);
            int rowIndex = FIRST_ROW;
            for (int i = 0; i < dt.Rows.Count; i++)
                try
                {
                    if ((dt.Rows[i][0].ToString().Trim() == string.Empty) && (dt.Rows[i][1].ToString().Trim() == string.Empty) &&
                        (dt.Rows[i][2].ToString().Trim() == string.Empty))
                        break;
                    PumpXlsRow(dt.Rows[i], refDate);
                    rowIndex++;
                }
                catch (Exception exp)
                {
                    throw new Exception(string.Format("Ошибка при обработке строки {0} отчета {1}: {2}", 
                        rowIndex, fileName, exp.Message), exp);
                }
        }

        private void PumpXLSFile(FileInfo file)
        {
            object workbook = excelHelper.InitWorkBook(ref excelObj, file.FullName);
            try
            {
                object sheet = excelHelper.GetSheet(workbook, 1);
                rowsCount = GetRowsCount(sheet);
                PumpXlsSheet(sheet, file.Name);
            }
            finally
            {
                excelHelper.CloseExcel(ref excelObj);
            }
        }

        #endregion Работа с экселем

        #region Перекрытые методы закачки

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStart, "Старт инициализации Excel.");
            excelHelper = new ExcelHelper();
            deletedDateList = new List<int>();
            try
            {
                ProcessFilesTemplate(dir, "*.xls", new ProcessFileDelegate(PumpXLSFile), false);
                UpdateData();
            }
            finally
            {
                if (excelHelper != null)
                    excelHelper.Close();
                deletedDateList.Clear();
            }
        }

        protected override void DirectPumpData()
        {
            PumpDataYTemplate();
        }

        #endregion Перекрытые методы закачки

        #endregion Закачка данных

    }
}
