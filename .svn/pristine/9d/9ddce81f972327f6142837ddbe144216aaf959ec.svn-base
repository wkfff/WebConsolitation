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

namespace Krista.FM.Server.DataPumps.UFK18Pump
{
    // Ведомость по кассовым выплатам из бюджета (ежедневная)
    public class UFK18PumpModule : TextRepPumpModuleBase
    {

        #region поля

        #region классификаторы

        // Районы.УФК (d.Regions.UFK)
        private IDbDataAdapter daRegion;
        private DataSet dsRegion;
        private IClassifier clsRegion;
        private Dictionary<string, int> cacheRegion = null;
        // ЭКР.УФК (d.EKR.UFK)
        private IDbDataAdapter daEkr;
        private DataSet dsEkr;
        private IClassifier clsEkr;
        private Dictionary<string, int> cacheEkr = null;
        private int nullEkr = -1;
        // КИФ.УФК (d.KIF.UFK)
        private IDbDataAdapter daKif;
        private DataSet dsKif;
        private IClassifier clsKif;
        private Dictionary<string, int> cacheKif = null;
        private int nullKif = -1;
        // КД.УФК (d.KD.UFK)
        private IDbDataAdapter daKd;
        private DataSet dsKd;
        private IClassifier clsKd;
        private Dictionary<string, int> cacheKd = null;
        private int nullKd = -1;
        // Расходы.УФК (d.R.UFK)
        private IDbDataAdapter daOutcomes;
        private DataSet dsOutcomes;
        private IClassifier clsOutcomes;
        private Dictionary<string, int> cacheOutcomes = null;
        private int nullOutcomes = -1;

        #endregion классификаторы

        #region факты

        // Факт.УФК_Ведомость по кассовым выплатам из бюджета ежедневная (f.R.UFK18)
        private IDbDataAdapter daFactUFK18;
        private DataSet dsFactUFK18;
        private IFactTable fctFactUFK18;

        #endregion факты

        private List<int> deletedDateList = null;
        private const char WIN_DELIMETER = '|';

        #endregion поля

        #region Закачка данных

        #region работа с базой и кэшами

        private void InitUpdatedFixedRows()
        {
            nullEkr = clsEkr.UpdateFixedRows(this.DB, this.SourceID);
            nullKif = clsKif.UpdateFixedRows(this.DB, this.SourceID);
            nullKd = clsKd.UpdateFixedRows(this.DB, this.SourceID);
        }

        private void FillCaches()
        {
            FillRowsCache(ref cacheRegion, dsRegion.Tables[0], "Code");
            FillRowsCache(ref cacheEkr, dsEkr.Tables[0], "Code");
            FillRowsCache(ref cacheKif, dsKif.Tables[0], "CodeStr");
            FillRowsCache(ref cacheKd, dsKd.Tables[0], "CodeStr");
            FillRowsCache(ref cacheOutcomes, dsOutcomes.Tables[0], "Code");
        }

        protected override void QueryData()
        {
            InitClsDataSet(ref daRegion, ref dsRegion, clsRegion, false, string.Empty);
            InitClsDataSet(ref daEkr, ref dsEkr, clsEkr, false, string.Empty);
            InitClsDataSet(ref daKif, ref dsKif, clsKif, false, string.Empty);
            InitClsDataSet(ref daKd, ref dsKd, clsKd, false, string.Empty);
            InitClsDataSet(ref daOutcomes, ref dsOutcomes, clsOutcomes, false, string.Empty);
            InitFactDataSet(ref daFactUFK18, ref dsFactUFK18, fctFactUFK18);
            InitUpdatedFixedRows();
            FillCaches();
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daRegion, dsRegion, clsRegion);
            UpdateDataSet(daEkr, dsEkr, clsEkr);
            UpdateDataSet(daKif, dsKif, clsKif);
            UpdateDataSet(daKd, dsKd, clsKd);
            UpdateDataSet(daOutcomes, dsOutcomes, clsOutcomes);
            UpdateDataSet(daFactUFK18, dsFactUFK18, fctFactUFK18);
        }

        #region guid

        private const string D_REGION_GUID = "90375d17-5145-43b9-81f1-2145aba86b7c";
        private const string D_EKR_GUID = "b234f8dc-d37d-4cc0-a32e-2e74b2bfb935";
        private const string D_KIF_GUID = "73b83ed3-fa26-4d05-8e8e-30dbe226a801";
        private const string D_KD_GUID = "b713e1df-5584-4e3d-a399-8828a2906971";
        private const string D_OUTCOMES_GUID = "ba2b17a6-191f-477c-894d-2f879053a69e";
        private const string F_F_UFK18_GUID = "2d1d12ef-f77e-404b-8d4b-ee022793704d";

        #endregion guid
        protected override void InitDBObjects()
        {
            this.UsedClassifiers = new IClassifier[] { 
                clsRegion = this.Scheme.Classifiers[D_REGION_GUID], 
                clsEkr = this.Scheme.Classifiers[D_EKR_GUID], 
                clsKif = this.Scheme.Classifiers[D_KIF_GUID], 
                clsKd = this.Scheme.Classifiers[D_KD_GUID], 
                clsOutcomes = this.Scheme.Classifiers[D_OUTCOMES_GUID] };
            this.UsedFacts = new IFactTable[] { fctFactUFK18 = this.Scheme.FactTables[F_F_UFK18_GUID] };
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsFactUFK18);
            ClearDataSet(ref dsRegion);
            ClearDataSet(ref dsEkr);
            ClearDataSet(ref dsKif);
            ClearDataSet(ref dsKd);
            ClearDataSet(ref dsOutcomes);
        }

        #endregion работа с базой и кэшами

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

        private void GetData(string[] data, object[] mapping)
        {
            for (int i = 0; i <= mapping.GetLength(0) - 1; i += 2)
                mapping[i + 1] = data[Convert.ToInt32(mapping[i + 1])].Trim();
        }

        // маппинг - имя нашего поля, индекс колонки отчета
        private int PumpCls(string[] data, object[] mapping, DataTable dt, IClassifier cls, 
            Dictionary<string, int> cache, object[] defaultMapping, int nullRef)
        {
            GetData(data, mapping);
            if (mapping.GetLength(0) != 0)
            {
                if (mapping[1].ToString() == string.Empty)
                    return nullRef;
                if (mapping[0].ToString() == "Code")
                    mapping[1] = mapping[1].ToString().TrimStart('0').PadLeft(1, '0');
            }
            if (defaultMapping != null)
                mapping = (object[])CommonRoutines.ConcatArrays(mapping, defaultMapping);
            return PumpCachedRow(cache, dt, cls, mapping[1].ToString(), mapping);
        }

        #endregion общие методы

        #region работа с отчетами txt

        private void PumpFactRow(string[] data, object[] clsMapping)
        {
            decimal amount = GetSum(data[11]);
            decimal amountRec = GetSum(data[12]);
            if ((amount == 0) && (amountRec == 0))
                return;
            object[] mapping = (object[])CommonRoutines.ConcatArrays(clsMapping, 
                new object[] { "amount", amount, "amountRec", amountRec });
            PumpRow(dsFactUFK18.Tables[0], mapping);
            if (dsFactUFK18.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daFactUFK18, ref dsFactUFK18);
            }
        }

        private int PumpKif(string[] data, DataTable dt, IClassifier cls,
            Dictionary<string, int> cache, object[] defaultMapping, int nullRef)
        {
            int refKif = nullKif;
            refKif = PumpCls(data, new object[] { "CodeStr", 6 }, dt, cls, cache, defaultMapping, nullRef);
            // если кифа нет в 6, качаем из 7
            if (refKif == nullKif)
                refKif = PumpCls(data, new object[] { "CodeStr", 7 }, dt, cls, cache, defaultMapping, nullRef);
            return refKif;
        }

        private string GetOutcomesCode(string[] data)
        {
            return string.Concat(data[1].Trim(), data[2].Trim(), data[3].Trim(), 
                data[4].Trim()).TrimStart('0').PadLeft(1, '0');
        }

        private void PumpUFK18Row(string[] data, int refDate, int refRegion)
        {
            int refEkr = PumpCls(data, new object[] { "Code", 5 }, dsEkr.Tables[0], clsEkr, cacheEkr, null, nullEkr);
            int refKif = PumpKif(data, dsKif.Tables[0], clsKif, cacheKif, null, nullKif);
            int refKd = PumpCls(data, new object[] { "CodeStr", 8 }, dsKd.Tables[0],
                clsKd, cacheKd, new object[] { "Name", constDefaultClsName }, nullKd);
            string outcomesCode = GetOutcomesCode(data);
            int refOutcomes = PumpCls(data, new object[] { }, dsOutcomes.Tables[0],
                clsOutcomes, cacheOutcomes, new object[] { "Code", outcomesCode }, nullOutcomes);
            object[] mapping = new object[] { "refEkr", refEkr, "refKif", refKif, "refKd", refKd, 
                "RefR", refOutcomes, "RefYearDayUNV", refDate, "RefRegions", refRegion };
            PumpFactRow(data, mapping);
        }

        private const string DATE_MARK = "VKV";
        private const string REGION_MARK = "FROM";
        private const string DATA_MARK = "VKVSTBK";
        private void PumpFiles(FileInfo file)
        {
            string[] reportData = CommonRoutines.GetTxtReportData(file, CommonRoutines.GetTxtWinCodePage());
            int rowIndex = 0;
            int refDate = -1;
            int refRegion = -1;
            foreach (string row in reportData)
                try
                {
                    rowIndex++;
                    string[] data = row.Split(WIN_DELIMETER);
                    if (data[0].Trim().ToUpper() == DATE_MARK)
                    {
                        refDate = CommonRoutines.ShortDateToNewDate(data[3].Trim());
                        DeleteDateData(refDate);
                    }
                    else if (data[0].Trim().ToUpper() == REGION_MARK)
                        refRegion = PumpCls(data, new object[] { "Code", 1, "Name", 2 }, 
                            dsRegion.Tables[0], clsRegion, cacheRegion, null, -1);
                    else if (data[0].Trim().ToUpper() == DATA_MARK)
                        PumpUFK18Row(data, refDate, refRegion);
                }
                catch (Exception exp)
                {
                    throw new Exception(string.Format("Ошибка при обработке строки {0} отчета {1}: {2}", rowIndex, file.Name, exp.Message), exp);
                }
        }

        #endregion работа с отчетами txt

        #region перекрытые методы закачки

        private const string FILE_MASK = "*.PE*";
        protected override void ProcessFiles(DirectoryInfo dir)
        {
            deletedDateList = new List<int>();
            try
            {
                ProcessFilesTemplate(dir, FILE_MASK, new ProcessFileDelegate(PumpFiles), false);
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

        #region Обработка данных

        protected void FillRepCode()
        {
            foreach (DataRow row in dsOutcomes.Tables[0].Rows)
            {
                row["RepCode"] = string.Concat(
                    row["Code2"].ToString().PadLeft(2, '0'),
                    row["Code3"].ToString().PadLeft(2, '0'),
                    row["Code4"].ToString().PadLeft(3, '0'),
                    row["Code5"].ToString().PadLeft(2, '0'),
                    row["Code6"].ToString().PadLeft(2, '0'),
                    row["Code7"].ToString().PadLeft(3, '0'));
            }
        }

        protected override void ProcessDataSource()
        {
            FillRepCode();
            UpdateData();
        }

        protected override void DirectProcessData()
        {
            int year = -1;
            int month = -1;
            GetPumpParams(ref year, ref month);
            ProcessDataSourcesTemplate(year, month, "Выполняется заполнение поля «Код_отчет» в классификаторе «Расходы.УФК»");
        }

        #endregion Обработка данных
    }
}
