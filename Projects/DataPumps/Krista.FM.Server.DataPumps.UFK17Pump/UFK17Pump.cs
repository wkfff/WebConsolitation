using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

using Krista.FM.Server.Common;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.UFK17Pump
{
    public class UFK17PumpModule : TextRepPumpModuleBase
    {

        #region поля

        #region классификаторы

        // Межбюджетные трансферты.УФК (d.TransferBudget.UFK)
        private IDbDataAdapter daTransfer;
        private DataSet dsTransfer;
        private IClassifier clsTransfer;
        private Dictionary<string, int> cacheTransfer = null;

        #endregion классификаторы

        #region факты

        // Факт.УФК_Ведомость по движению свободного остатка (f.F.UFK17)
        private IDbDataAdapter daFactUFK17;
        private DataSet dsFactUFK17;
        private IFactTable fctFactUFK17;
        // Факт.УФК_Справка к ведомости по движению свободного остатка (f.F.UFK17Certificate)
        private IDbDataAdapter daFactUFK17Cert;
        private DataSet dsFactUFK17Cert;
        private IFactTable fctFactUFK17Cert;

        #endregion факты

        private List<int> deletedDateList = null;
        private int[] transferParentId = new int[] { -1, -1 };

        #endregion поля

        #region Закачка данных

        #region работа с базой и кэшами

        private void FillCaches()
        {
            FillRowsCache(ref cacheTransfer, dsTransfer.Tables[0], new string[] { "PurposeCode", "Name" }, "|", "Id");
        }

        private string[] sectionNames = new string[] { "Раздел I. Субсидии (субвенции), полученные из федерального бюджета, за исключением субсидий (субвенций), полученных в порядке компенсации произведенных кассовых расходов бюджета субъекта Российской Федерации (местного бюджета) (далее - целевые расходы)", 
                                                       "Раздел II. Субсидии (субвенции), полученные из федерального бюджета в порядке компенсации произведенных кассовых расходов бюджета субъекта Российской Федерации (местного бюджета)" };
        private void GetTransferParentId()
        {
            DataRow[] sectionRecords = dsTransfer.Tables[0].Select("PurposeCode = '0'");
            foreach (DataRow sectionRecord in sectionRecords)
            {
                int id = Convert.ToInt32(sectionRecord["Id"]);
                if (sectionRecord["Name"].ToString().ToUpper().StartsWith("РАЗДЕЛ I."))
                    transferParentId[0] = id;
                else if (sectionRecord["Name"].ToString().ToUpper().StartsWith("РАЗДЕЛ II."))
                    transferParentId[1] = id;
                else
                    continue;
            }
            for (int i = 0; i <= 1; i++)
                if (transferParentId[i] < 0)
                    transferParentId[i] = PumpRow(dsTransfer.Tables[0], clsTransfer, new object[] { "PurposeCode", 0, "Name", sectionNames[i] });
        }

        protected override void QueryData()
        {
            InitClsDataSet(ref daTransfer, ref dsTransfer, clsTransfer, false, string.Empty);
            InitFactDataSet(ref daFactUFK17, ref dsFactUFK17, fctFactUFK17);
            InitFactDataSet(ref daFactUFK17Cert, ref dsFactUFK17Cert, fctFactUFK17Cert);
            FillCaches();
            GetTransferParentId();
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daTransfer, dsTransfer, clsTransfer);
            UpdateDataSet(daFactUFK17, dsFactUFK17, fctFactUFK17);
            UpdateDataSet(daFactUFK17Cert, dsFactUFK17Cert, fctFactUFK17Cert);
        }

        private const string D_TRANSFER_BUDGET_UFK_GUID = "225cf221-2587-4d03-9740-e8cface2680e";
        private const string F_F_UFK17_GUID = "fd0a4164-bc09-49cd-82e5-6e37fcdec342";
        private const string F_F_UFK17_CERTIFICATE_GUID = "f1c9b095-c59f-4e29-b63a-de23827bbb39";
        protected override void InitDBObjects()
        {
            this.UsedClassifiers = new IClassifier[] { clsTransfer = this.Scheme.Classifiers[D_TRANSFER_BUDGET_UFK_GUID] };
            this.UsedFacts = new IFactTable[] {
                fctFactUFK17 = this.Scheme.FactTables[F_F_UFK17_GUID], 
                fctFactUFK17Cert = this.Scheme.FactTables[F_F_UFK17_CERTIFICATE_GUID] };
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsTransfer);
            ClearDataSet(ref dsFactUFK17);
            ClearDataSet(ref dsFactUFK17Cert);
        }

        #endregion работа с базой и кэшами

        #region работа с txt

        private const char WIN_DELIMETER = '|';

        #region общие методы 

        private void DeleteDateData(int dateRef)
        {
            if (!deletedDateList.Contains(dateRef))
            {
                DeleteData(string.Format("RefYearDayUNV = {0}", dateRef), string.Format("Дата отчета: {0}.", dateRef), false);
                deletedDateList.Add(dateRef);
            }
        }

        private decimal GetSum(string sum)
        {
            return Convert.ToDecimal(sum.Trim().PadLeft(1, '0').Replace('.', ','));
        }

        #endregion общие методы

        #region файлы Rm

        private void PumpRmSum(int refDate, decimal sum, int refMarks)
        {
            if (sum == 0)
                return;
            object[] mapping = new object[] { "RefYearDayUNV", refDate, "Amount", sum, "RefMarks", refMarks };
            PumpRow(dsFactUFK17.Tables[0], mapping);
            if (dsFactUFK17.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daFactUFK17, ref dsFactUFK17);
            }
        }

        private int GetDate(string[] columnsValue)
        {
            if (this.DataSource.Year >= 2009)
                return CommonRoutines.ShortDateToNewDate(columnsValue[2].Trim());
            return CommonRoutines.ShortDateToNewDate(columnsValue[1].Trim());
        }

        private int[] NUM_COLUMNS_2008 = new int[] { 5, 6, 7, 8, 9, 10, 12, 13, 14, 15, 16 };
        private int[] NUM_COLUMNS_2009 = new int[] { 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 };
        private void PumpRmRow(string[] columnsValue)
        {
            int refDate = GetDate(columnsValue);
            DeleteDateData(refDate);
            int[] numColumns = this.DataSource.Year >= 2009 ? NUM_COLUMNS_2009 : NUM_COLUMNS_2008;
            for (int i = 0; i < 11; i++)
            {
                PumpRmSum(refDate, GetSum(columnsValue[numColumns[i]]), i + 1);
            }
        }

        private const string VSO_MARK = "VSO";
        private const string RM_MARK = "RM";
        private void PumpRmFiles(FileInfo file)
        {
            string[] reportData = CommonRoutines.GetTxtReportData(file, CommonRoutines.GetTxtWinCodePage());
            int rowIndex = 0;
            string pumpedRowMark = this.DataSource.Year >= 2009 ? RM_MARK : VSO_MARK;
            foreach (string row in reportData)
                try
                {
                    rowIndex++;
                    string[] columnsValue = row.Split(WIN_DELIMETER);
                    if (columnsValue[0].Trim().ToUpper() != pumpedRowMark)
                        continue;
                    PumpRmRow(columnsValue);
                }
                catch (Exception exp)
                {
                    throw new Exception(string.Format("Ошибка при обработке строки {0} отчета {1}: {2}", rowIndex, file.Name, exp.Message), exp);
                }
        }

        #endregion файлы Rm

        #region файлы Pv

        private void PumpPvSum(int refDate, decimal sum, int refMarks, int refTransfer)
        {
            if (sum == 0)
                return;
            object[] mapping = new object[] { "RefYearDayUNV", refDate, "Amount", sum, "RefMarks", refMarks, "RefTransferBudget", refTransfer };
            PumpRow(dsFactUFK17Cert.Tables[0], mapping);
            if (dsFactUFK17Cert.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daFactUFK17Cert, ref dsFactUFK17Cert);
            }
        }

        private int PumpTransfer(string name, string code, int prvsorIndex)
        {
            string key = string.Format("{0}|{1}", code, name);
            return PumpCachedRow(cacheTransfer, dsTransfer.Tables[0], clsTransfer, key,
                new object[] { "PurposeCode", code, "Name", name, "ParentID", transferParentId[prvsorIndex - 1] });
        }

        private void PumpPvRowPrvsor1(string[] columnsValue, int refDate, int refTransfer)
        {
            PumpPvSum(refDate, GetSum(columnsValue[3]), 1, refTransfer);
            PumpPvSum(refDate, GetSum(columnsValue[4]), 2, refTransfer);
            PumpPvSum(refDate, GetSum(columnsValue[5]), 3, refTransfer);
            PumpPvSum(refDate, GetSum(columnsValue[6]), 4, refTransfer);
        }

        private void PumpPvRowPrvsor2(string[] columnsValue, int refDate, int refTransfer)
        {
            PumpPvSum(refDate, GetSum(columnsValue[3]), 2, refTransfer);
        }

        private void PumpPvRow(string[] columnsValue, int prvsorIndex, int refDate)
        {
            int refTransfer = PumpTransfer(columnsValue[1].Trim(), columnsValue[2].Trim().TrimStart('0').PadLeft(1, '0'), prvsorIndex);
            if (prvsorIndex == 1)
                PumpPvRowPrvsor1(columnsValue, refDate, refTransfer);
            else
                PumpPvRowPrvsor2(columnsValue, refDate, refTransfer);
        }

        private const string PRVSO_MARK = "PRVSO";
        private const string PRVSOR1_MARK = "PRVSOR1";
        private const string PRVSOR2_MARK = "PRVSOR2";
        private void PumpPvFiles(FileInfo file)
        {
            string[] reportData = CommonRoutines.GetTxtReportData(file, CommonRoutines.GetTxtWinCodePage());
            int rowIndex = 0;
            int refDate = -1;
            foreach (string row in reportData)
                try
                {
                    rowIndex++;
                    string[] columnsValue = row.Split(WIN_DELIMETER);
                    if (columnsValue[0].Trim().ToUpper() == PRVSO_MARK)
                    {
                        refDate = CommonRoutines.ShortDateToNewDate(columnsValue[1].Trim());
                        DeleteDateData(refDate);
                        continue;
                    }
                    else if (columnsValue[0].Trim().ToUpper() == PRVSOR1_MARK)
                        PumpPvRow(columnsValue, 1, refDate);
                    else if (columnsValue[0].Trim().ToUpper() == PRVSOR2_MARK)
                        PumpPvRow(columnsValue, 2, refDate);
                    else
                        continue;
                }
                catch (Exception exp)
                {
                    throw new Exception(string.Format("Ошибка при обработке строки {0} отчета {1}: {2}", rowIndex, file.Name, exp.Message), exp);
                }
        }

        #endregion файлы Pv

        #endregion работа с txt

        #region перекрытые методы закачки

        private const string FILE_MASK_1 = "*.RM*";
        private const string FILE_MASK_2 = "*.PV*";
        protected override void ProcessFiles(DirectoryInfo dir)
        {
            deletedDateList = new List<int>();
            try
            {
                ProcessFilesTemplate(dir, FILE_MASK_1, new ProcessFileDelegate(PumpRmFiles), false);
                ProcessFilesTemplate(dir, FILE_MASK_2, new ProcessFileDelegate(PumpPvFiles), false);
                UpdateData();
            }
            finally
            {
                deletedDateList.Clear();
            }
        }

        protected override void DirectPumpData()
        {
            PumpDataYTemplate();
        }

        #endregion перекрытые методы закачки

        #endregion Закачка данных

    }
}
