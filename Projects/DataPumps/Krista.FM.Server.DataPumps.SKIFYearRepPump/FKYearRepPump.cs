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
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.SKIFYearRepPump
{
    // формат ФК
    public partial class SKIFYearRepPumpModule : SKIFRepPumpModuleBase
    {

        #region поля

        // КД.Эталонный (d_KD_Etalon)
        private IDbDataAdapter daKdTxt;
        private DataSet dsKdTxt;
        private IClassifier clsKdTxt;
        // ФКР.Эталонный (d_FKR_Etalon)
        private IDbDataAdapter daFkrTxt;
        private DataSet dsFkrTxt;
        private IClassifier clsFkrTxt;
        // КЦСР.Эталонный (d_KCSR_Etalon)
        private IDbDataAdapter daKcsrTxt;
        private DataSet dsKcsrTxt;
        private IClassifier clsKcsrTxt;
        // КВР.Эталонный (d_KVR_Etalon)
        private IDbDataAdapter daKvrTxt;
        private DataSet dsKvrTxt;
        private IClassifier clsKvrTxt;
        // ЭКР.Эталонный (d_EKR_Etalon)
        private IDbDataAdapter daEkrTxt;
        private DataSet dsEkrTxt;
        private IClassifier clsEkrTxt;
        // КИФ.Эталонный (d_KIF_Etalon)
        private IDbDataAdapter daKifTxt;
        private DataSet dsKifTxt;
        private IClassifier clsKifTxt;

        public string formTxt;
        public int refMeansTypeTxt;
        public int sourceBdgLvl;
        public int curBlock;
        public int refDateTxt;
        public int refRegionTxt;

        #endregion поля

        #region работа с классификаторами

        private void GetRegionBudgetParams(int refDocType, ref string budKind, ref string budName)
        {
            switch (refDocType)
            {
                case 3:
                    budKind = "КБС";
                    budName = "Консолидированный бюджет субъекта";
                    break;
                case 5:
                    budKind = "СБС";
                    budName = "Собственный бюджет субъекта";
                    break;
                case 7:
                case 10:
                    budKind = "МНЦП";
                    budName = "Муниципальные образования";
                    break;
                case 1:
                    budKind = "НБ";
                    budName = "Неуказанный бюджет";
                    break;
                default:
                    budKind = "МНЦП";
                    budName = "Муниципальные образования";
                    break;
            }
        }

        private bool PumpRegionsTxt(string regionCode)
        {
            string key = string.Format("{0}|{1}", regionCode, regionCode);
            DataRow[] regionRows = dsRegions4Pump.Tables[0].Select(string.Format("CodeStr = '{0}'", regionCode));
            if (regionRows.GetLength(0) == 0)
            {
                // если не найден в служебном - добавляем в служебный
                PumpCachedRow(region4PumpCache, dsRegions4Pump.Tables[0], clsRegions4Pump, key, 
                    new object[] { "CodeStr", regionCode, "NAME", regionCode, "REFDOCTYPE", 1, "SOURCEID", GetRegions4PumpSourceID() });
                // качаем в клс районы.год отч
                PumpRegionsTxt(regionCode);
                return false;
            }
            else
            {
                int refDocType = Convert.ToInt32(regionRows[0]["REFDOCTYPE"]);
                string budKind = string.Empty;
                string budName = string.Empty;
                GetRegionBudgetParams(refDocType, ref budKind, ref budName);
                object[] mapping = new object[] { "CodeStr", regionCode, "Name", regionCode, 
                    "BudgetKind", budKind, "BudgetName", budName, "RefDocType",  refDocType };
                refRegionTxt = PumpCachedRow(regionCache, dsRegions.Tables[0], clsRegions, key, mapping);
                return true;
            }
        }

        // инициализация эталонного классификатора
        // если за текущий месяц данных нет - берем предыдущий. за январь данные есть палюбасу
        private void InitStandartCls(ref IDbDataAdapter da, ref DataSet ds, IClassifier cls)
        {
            for (int curMonth = 12; curMonth >= 1; curMonth--)
            {
                string query = string.Format("select id from DataSources where DELETED = 0 and SUPPLIERCODE = 'ФО'" +
                                             " and DATACODE = 22 and year = {0} and month = {1}",
                    this.DataSource.Year, curMonth);
                DataTable sourceId = (DataTable)this.DB.ExecQuery(query, QueryResultTypes.DataTable, new IDbDataParameter[] { });
                if ((sourceId == null) || (sourceId.Rows.Count == 0))
                    continue;
                foreach (DataRow row in sourceId.Rows)
                {
                    string constr = string.Format("SOURCEID = {0}", row["Id"]);
                    InitDataSet(ref da, ref ds, cls, true, constr, string.Empty);
                    if (ds.Tables[0].Rows.Count > 3)
                        break;
                }
                if (ds.Tables[0].Rows.Count > 3)
                    break;
            }
            if (ds == null)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                    string.Format("Не заполнен эталонный классификатор '{0}'", cls.FullCaption));
                InitDataSet(ref da, ref ds, cls, true, "1=0", string.Empty);
            }
            else
            {
                if (ds.Tables[0].Rows.Count <= 3)
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                        string.Format("Не заполнен эталонный классификатор '{0}'", cls.FullCaption));
            }
        }

        private const string D_KD_TXT_GUID = "8c51f8ce-62cd-450f-8d6a-afeb998f3be9";
        private const string D_FKR_TXT_GUID = "f0298b14-1036-4851-838b-fbff183ed629";
        private const string D_KCSR_TXT_GUID = "5becf070-3ba1-49eb-8dec-2b559d1df9f3";
        private const string D_KVR_TXT_GUID = "5a407d4a-8056-4ef2-9a4b-f3a14fb4af9f";
        private const string D_EKR_TXT_GUID = "03c3a566-82fb-48e9-b3d9-3d71f42a947f";
        private const string D_KIF_TXT_GUID = "a57d3b07-7bd1-4314-a24e-90d418010c5a";
        private void InitClsTxt()
        {
            clsKdTxt = this.Scheme.Classifiers[D_KD_TXT_GUID];
            clsFkrTxt = this.Scheme.Classifiers[D_FKR_TXT_GUID];
            clsKcsrTxt = this.Scheme.Classifiers[D_KCSR_TXT_GUID];
            clsKvrTxt = this.Scheme.Classifiers[D_KVR_TXT_GUID];
            clsEkrTxt = this.Scheme.Classifiers[D_EKR_TXT_GUID];
            clsKifTxt = this.Scheme.Classifiers[D_KIF_TXT_GUID];

            InitStandartCls(ref daKdTxt, ref dsKdTxt, clsKdTxt);
            InitStandartCls(ref daFkrTxt, ref dsFkrTxt, clsFkrTxt);
            InitStandartCls(ref daKcsrTxt, ref dsKcsrTxt, clsKcsrTxt);
            InitStandartCls(ref daKvrTxt, ref dsKvrTxt, clsKvrTxt);
            InitStandartCls(ref daEkrTxt, ref dsEkrTxt, clsEkrTxt);
            InitStandartCls(ref daKifTxt, ref dsKifTxt, clsKifTxt);
        }

        private void ClearClsTxt()
        {
            ClearDataSet(ref dsKdTxt);
            ClearDataSet(ref dsFkrTxt);
            ClearDataSet(ref dsKcsrTxt);
            ClearDataSet(ref dsKvrTxt);
            ClearDataSet(ref dsEkrTxt);
            ClearDataSet(ref dsKifTxt);
        }

        private void PumpKdTxt()
        {
            string code = string.Empty;
            string name = string.Empty;
            foreach (DataRow row in dsKdTxt.Tables[0].Rows)
            {
                code = row["CodeStr"].ToString();
                name = row["Name"].ToString();
                // пропускаем служебные записи
                if (name.Contains("0022"))
                    continue;
                object[] mapping = new object[] { "CodeStr", code, "Name", row["Name"], "KL", 0, "KST", 10 };
                PumpCachedRow(kdCache, dsKD.Tables[0], clsKD, code, mapping);
            }
            code = "00085000000000000000";
            name = "Доходы бюджета - ИТОГО";
            PumpCachedRow(kdCache, dsKD.Tables[0], clsKD, code,
                new object[] { "CodeStr", code, "Name", name, "KL", 1, "KST", 10 });
            code = "00087000000000000000";
            name = "Суммы, подлежащие взаимоисключению";
            PumpCachedRow(kdCache, dsKD.Tables[0], clsKD, code,
                new object[] { "CodeStr", code, "Name", name, "KL", 18000, "KST", 20 });
            code = "00087000000000000120";
            name = "в том числе доходы от собственности (в части процентов и штрафных санкций по выданным бюджетным кредитам)";
            PumpCachedRow(kdCache, dsKD.Tables[0], clsKD, code,
                new object[] { "CodeStr", code, "Name", name, "KL", 18010, "KST", 21 });
            code = "00087000000000000151";
            name = "поступления от других бюджетов бюджетной системы Российской Федерации";
            PumpCachedRow(kdCache, dsKD.Tables[0], clsKD, code,
                new object[] { "CodeStr", code, "Name", name, "KL", 18020, "KST", 22 });
        }

        private void PumpFkrTxt()
        {
            string code = string.Empty;
            string name = string.Empty;
            foreach (DataRow row in dsFkrTxt.Tables[0].Rows)
            {
                code = row["Code"].ToString();
                name = row["Name"].ToString();
                // пропускаем служебные записи
                if (name.Contains("0022"))
                    continue;
                object[] mapping = new object[] { "Code", code, "Name", row["Name"] };
                PumpCachedRow(fkrCache, dsFKR.Tables[0], clsFKR, code, mapping);
            }
            code = "9600";
            name = "Расходы бюджета – ИТОГО";
            PumpCachedRow(fkrCache, dsFKR.Tables[0], clsFKR, code,
                new object[] { "Code", code, "Name", name });
            code = "9700";
            name = "Суммы, подлежащие взаимоисключению";
            PumpCachedRow(fkrCache, dsFKR.Tables[0], clsFKR, code,
                new object[] { "Code", code, "Name", name });

        }

        private void PumpKcsrTxt()
        {
            string code = string.Empty;
            string name = string.Empty;
            foreach (DataRow row in dsKcsrTxt.Tables[0].Rows)
            {
                code = row["Code"].ToString();
                name = row["Name"].ToString();
                // пропускаем служебные записи
                if (name.Contains("0022"))
                    continue;
                object[] mapping = new object[] { "Code", code, "Name", row["Name"] };
                PumpCachedRow(kcsrCache, dsKCSR.Tables[0], clsKCSR, code, mapping);
            }
        }

        private void PumpKvrTxt()
        {
            string code = string.Empty;
            string name = string.Empty;
            foreach (DataRow row in dsKvrTxt.Tables[0].Rows)
            {
                code = row["Code"].ToString();
                name = row["Name"].ToString();
                // пропускаем служебные записи
                if (name.Contains("0022"))
                    continue;
                object[] mapping = new object[] { "Code", code, "Name", row["Name"] };
                PumpCachedRow(kvrCache, dsKVR.Tables[0], clsKVR, code, mapping);
            }
        }

        private void PumpEkrTxt()
        {
            foreach (DataRow row in dsEkrTxt.Tables[0].Rows)
            {
                string code = row["Code"].ToString();
                string name = row["Name"].ToString();
                // пропускаем служебные записи
                if (name.Contains("0022"))
                    continue;
                object[] mapping = new object[] { "Code", code, "Name", row["Name"] };
                PumpCachedRow(ekrCache, dsEKR.Tables[0], clsEKR, code, mapping);
            }
        }

        private void PumpInFinTxt()
        {
            string code = string.Empty;
            string name = string.Empty;
            foreach (DataRow row in dsKifTxt.Tables[0].Rows)
            {
                code = row["CodeStr"].ToString();
                int kst = 0;
                if (code.StartsWith("00001"))
                    kst = 520;
                else if (code.StartsWith("00002"))
                    kst = 620;
                else
                    continue;
                name = row["Name"].ToString();
                // пропускаем служебные записи
                if (name.Contains("0022"))
                    continue;
                object[] mapping = new object[] { "CodeStr", code, "Name", row["Name"], "KL", 0, "KST", kst };
                PumpCachedRow(kifCache, dsKIF2005.Tables[0], clsKIF2005, code, mapping);
            }

            if (this.DataSource.Year >= 2009)
            {
                code = "00090000000000000000";
                name = "Источники финансирования дефицита бюджетов - всего";
                PumpCachedRow(kifCache, dsKIF2005.Tables[0], clsKIF2005, code,
                    new object[] { "CodeStr", code, "Name", name, "KL", 3, "KST", 500 });
                code = "00001000000000000000";
                name = "Источники внутреннего финансирования бюджета";
                PumpCachedRow(kifCache, dsKIF2005.Tables[0], clsKIF2005, code,
                    new object[] { "CodeStr", code, "Name", name, "KL", 3, "KST", 520 });
                code = "00002000000000000000";
                name = "Источники внешнего финансирования бюджета";
                PumpCachedRow(kifCache, dsKIF2005.Tables[0], clsKIF2005, code,
                    new object[] { "CodeStr", code, "Name", name, "KL", 3, "KST", 620 });
                code = "00001050000000000000";
                name = "Изменение остатков средств бюджетов";
                PumpCachedRow(kifCache, dsKIF2005.Tables[0], clsKIF2005, code,
                    new object[] { "CodeStr", code, "Name", name, "KL", 3, "KST", 700 });
                code = "00057000000000000000";
                name = "Суммы, подлежащие взаимоисключению";
                PumpCachedRow(kifCache, dsKIF2005.Tables[0], clsKIF2005, code,
                    new object[] { "CodeStr", code, "Name", name, "KL", 3, "KST", 750 });
                code = "00057000000000000710";
                name = "Увеличение внутренних заимствований";
                PumpCachedRow(kifCache, dsKIF2005.Tables[0], clsKIF2005, code,
                    new object[] { "CodeStr", code, "Name", name, "KL", 3, "KST", 751 });
                code = "00057000000000000810";
                name = "Уменьшение внутренних заимствований";
                PumpCachedRow(kifCache, dsKIF2005.Tables[0], clsKIF2005, code,
                    new object[] { "CodeStr", code, "Name", name, "KL", 3, "KST", 752 });
                code = "00057000000000000540";
                name = "Выдача бюджетных кредитов другим бюджетам бюджетной системы Российской Федерации";
                PumpCachedRow(kifCache, dsKIF2005.Tables[0], clsKIF2005, code,
                    new object[] { "CodeStr", code, "Name", name, "KL", 3, "KST", 753 });
                code = "00057000000000000640";
                name = "Погашение бюджетных кредитов, выданных другим бюджетам бюджетной системы Российской Федерации";
                PumpCachedRow(kifCache, dsKIF2005.Tables[0], clsKIF2005, code,
                    new object[] { "CodeStr", code, "Name", name, "KL", 3, "KST", 754 });
                code = "00057000000000000510";
                name = "Увеличение остатков средств";
                PumpCachedRow(kifCache, dsKIF2005.Tables[0], clsKIF2005, code,
                    new object[] { "CodeStr", code, "Name", name, "KL", 3, "KST", 755 });
                code = "00057000000000000610";
                name = "Уменьшение остатков средств";
                PumpCachedRow(kifCache, dsKIF2005.Tables[0], clsKIF2005, code,
                    new object[] { "CodeStr", code, "Name", name, "KL", 3, "KST", 756 });
            }
            else
            {
                code = "00090000000000000000";
                name = "Источники финансирования дефицита бюджетов - всего";
                PumpCachedRow(kifCache, dsKIF2005.Tables[0], clsKIF2005, code,
                    new object[] { "CodeStr", code, "Name", name, "KL", 3, "KST", 500 });
                code = "00001000000000000000";
                name = "Источники внутреннего финансирования бюджета";
                PumpCachedRow(kifCache, dsKIF2005.Tables[0], clsKIF2005, code,
                    new object[] { "CodeStr", code, "Name", name, "KL", 3, "KST", 520 });
                code = "00001050000000000000";
                name = "Изменение остатков средств бюджетов";
                PumpCachedRow(kifCache, dsKIF2005.Tables[0], clsKIF2005, code,
                    new object[] { "CodeStr", code, "Name", name, "KL", 3, "KST", 700 });
                code = "00057000000000000000";
                name = "Итого внутренних оборотов";
                PumpCachedRow(kifCache, dsKIF2005.Tables[0], clsKIF2005, code,
                    new object[] { "CodeStr", code, "Name", name, "KL", 3, "KST", 750 });
                code = "00057000000000000710";
                name = "увеличение внутренних заимствований (КОСГУ 710)";
                PumpCachedRow(kifCache, dsKIF2005.Tables[0], clsKIF2005, code,
                    new object[] { "CodeStr", code, "Name", name, "KL", 3, "KST", 751 });
                code = "00057000000000000810";
                name = "уменьшение внутренних заимствований (КОСГУ 810)";
                PumpCachedRow(kifCache, dsKIF2005.Tables[0], clsKIF2005, code,
                    new object[] { "CodeStr", code, "Name", name, "KL", 3, "KST", 752 });
            }
        }

        private void PumpClsTxt()
        {
            if (toPumpIncomes)
                PumpKdTxt();
            if (toPumpOutcomes)
            {
                PumpFkrTxt();
                PumpKcsrTxt();
                PumpKvrTxt();
                PumpEkrTxt();
            }
            if (toPumpFinSources)
                PumpInFinTxt();
            UpdateData();
        }

        #endregion работа с классификаторами

        #region работа с фактами

        // возвращает тру - если суммы ненулевые, фолс - все суммы нулевые, закачивать не надо
        private bool GetRowValuesTxt(ref object[] mapping, string[] rowValues)
        {
            bool zeroSums = true;
            for (int i = 0; i <= mapping.GetLength(0) - 1; i += 2)
            {
                try
                {
                    if (!mapping[i].ToString().ToUpper().Contains("REP"))
                        continue;
                    string sum = rowValues[Convert.ToInt32(mapping[i + 1])];
                    decimal sumDec = Convert.ToDecimal(sum.Replace('.', ','));
                    mapping[i + 1] = sumDec;
                    if (sumDec != 0)
                        zeroSums = false;
                }
                catch (Exception exp)
                {
                    throw new Exception(string.Format("При получении значения поля '{0}' произошла ошибка: {1}",
                        mapping[i].ToString(), exp.Message));
                }
            }
            return (!zeroSums);
        }

        private void PumpFactRow(DataTable dt, string[] rowValues, object[] mapping)
        {
            if (!GetRowValuesTxt(ref mapping, rowValues))
                return;
            PumpRow(dt, mapping);
        }

        #region форма 428

        private void PumpIncomes428Txt(string[] rowValues, int refKd)
        {
            if (this.DataSource.Year >= 2011)
            {
                PumpFactRow(dsFOYRIncomes.Tables[0], rowValues,
                    new object[] { "AssignedReport", 3, "PerformedReport", 13, "ExcSumPRep", 4, "ExcSumFRep", 14, 
                            "RefMeansType", refMeansTypeTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 1, "RefKD", refKd });
                PumpFactRow(dsFOYRIncomes.Tables[0], rowValues,
                    new object[] { "AssignedReport", 5, "PerformedReport", 15, "ExcSumPRep", 6, "ExcSumFRep", 16, 
                            "RefMeansType", refMeansTypeTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 2, "RefKD", refKd });
                PumpFactRow(dsFOYRIncomes.Tables[0], rowValues,
                    new object[] { "AssignedReport", 7, "PerformedReport", 17, "RefMeansType", refMeansTypeTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 3, "RefKD", refKd });
                PumpFactRow(dsFOYRIncomes.Tables[0], rowValues,
                    new object[] { "AssignedReport", 8, "PerformedReport", 18, "RefMeansType", refMeansTypeTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 11, "RefKD", refKd });
                PumpFactRow(dsFOYRIncomes.Tables[0], rowValues,
                    new object[] { "AssignedReport", 9, "PerformedReport", 19, "RefMeansType", refMeansTypeTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 4, "RefKD", refKd });
                PumpFactRow(dsFOYRIncomes.Tables[0], rowValues,
                    new object[] { "AssignedReport", 10, "PerformedReport", 20, "RefMeansType", refMeansTypeTxt,
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 5, "RefKD", refKd });
                PumpFactRow(dsFOYRIncomes.Tables[0], rowValues,
                    new object[] { "AssignedReport", 11, "PerformedReport", 21, "RefMeansType", refMeansTypeTxt,
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 6, "RefKD", refKd });
                PumpFactRow(dsFOYRIncomes.Tables[0], rowValues,
                    new object[] { "AssignedReport", 12, "PerformedReport", 22, "RefMeansType", refMeansTypeTxt,
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 8, "RefKD", refKd });
            }
            else
            {
                PumpFactRow(dsFOYRIncomes.Tables[0], rowValues,
                    new object[] { "AssignedReport", 3, "PerformedReport", 11, "RefMeansType", refMeansTypeTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 1, "RefKD", refKd });
                PumpFactRow(dsFOYRIncomes.Tables[0], rowValues,
                    new object[] { "AssignedReport", 4, "PerformedReport", 12, "RefMeansType", refMeansTypeTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 2, "RefKD", refKd });
                PumpFactRow(dsFOYRIncomes.Tables[0], rowValues,
                    new object[] { "AssignedReport", 5, "PerformedReport", 13, "RefMeansType", refMeansTypeTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 3, "RefKD", refKd });
                PumpFactRow(dsFOYRIncomes.Tables[0], rowValues,
                    new object[] { "AssignedReport", 6, "PerformedReport", 14, "RefMeansType", refMeansTypeTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 11, "RefKD", refKd });
                PumpFactRow(dsFOYRIncomes.Tables[0], rowValues,
                    new object[] { "AssignedReport", 7, "PerformedReport", 15, "RefMeansType", refMeansTypeTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 4, "RefKD", refKd });
                PumpFactRow(dsFOYRIncomes.Tables[0], rowValues,
                    new object[] { "AssignedReport", 8, "PerformedReport", 16, "RefMeansType", refMeansTypeTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 5, "RefKD", refKd });
                PumpFactRow(dsFOYRIncomes.Tables[0], rowValues,
                    new object[] { "AssignedReport", 9, "PerformedReport", 17, "RefMeansType", refMeansTypeTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 6, "RefKD", refKd });
                PumpFactRow(dsFOYRIncomes.Tables[0], rowValues,
                    new object[] { "AssignedReport", 10, "PerformedReport", 18, "RefMeansType", refMeansTypeTxt,
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 8, "RefKD", refKd });
            }
        }

        private void PumpOutcomes428Txt(string[] rowValues, int refFkr, int refEkr)
        {
            if (this.DataSource.Year >= 2011)
            {
                PumpFactRow(dsFOYROutcomes.Tables[0], rowValues,
                    new object[] { "AssignedReport", 6, "PerformedReport", 16,  "ExcSumPRep", 7, "ExcSumFRep", 17, 
                            "RefMeansType", refMeansTypeTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 1, "RefFKR", refFkr, "RefEKRFOYR", refEkr, 
                            "RefKVR", nullKVR, "RefKCSR", nullKCSR, "RefKVSRYR", nullKvsr, "RefR", nullOutcomesCls });
                PumpFactRow(dsFOYROutcomes.Tables[0], rowValues,
                    new object[] { "AssignedReport", 8, "PerformedReport", 18,  "ExcSumPRep", 9, "ExcSumFRep", 19, 
                            "RefMeansType", refMeansTypeTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 2, "RefFKR", refFkr, "RefEKRFOYR", refEkr, 
                            "RefKVR", nullKVR, "RefKCSR", nullKCSR, "RefKVSRYR", nullKvsr, "RefR", nullOutcomesCls });
                PumpFactRow(dsFOYROutcomes.Tables[0], rowValues,
                    new object[] { "AssignedReport", 10, "PerformedReport", 20, "RefMeansType", refMeansTypeTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 3, "RefFKR", refFkr, "RefEKRFOYR", refEkr, 
                            "RefKVR", nullKVR, "RefKCSR", nullKCSR, "RefKVSRYR", nullKvsr, "RefR", nullOutcomesCls });
                PumpFactRow(dsFOYROutcomes.Tables[0], rowValues,
                    new object[] { "AssignedReport", 11, "PerformedReport", 21, "RefMeansType", refMeansTypeTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 11, "RefFKR", refFkr, "RefEKRFOYR", refEkr, 
                            "RefKVR", nullKVR, "RefKCSR", nullKCSR, "RefKVSRYR", nullKvsr, "RefR", nullOutcomesCls });
                PumpFactRow(dsFOYROutcomes.Tables[0], rowValues,
                    new object[] { "AssignedReport", 12, "PerformedReport", 22, "RefMeansType", refMeansTypeTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 4, "RefFKR", refFkr, "RefEKRFOYR", refEkr, 
                            "RefKVR", nullKVR, "RefKCSR", nullKCSR, "RefKVSRYR", nullKvsr, "RefR", nullOutcomesCls });
                PumpFactRow(dsFOYROutcomes.Tables[0], rowValues,
                    new object[] { "AssignedReport", 13, "PerformedReport", 23, "RefMeansType", refMeansTypeTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 5, "RefFKR", refFkr, "RefEKRFOYR", refEkr, 
                            "RefKVR", nullKVR, "RefKCSR", nullKCSR, "RefKVSRYR", nullKvsr, "RefR", nullOutcomesCls });
                PumpFactRow(dsFOYROutcomes.Tables[0], rowValues,
                    new object[] { "AssignedReport", 14, "PerformedReport", 24, "RefMeansType", refMeansTypeTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 6, "RefFKR", refFkr, "RefEKRFOYR", refEkr, 
                            "RefKVR", nullKVR, "RefKCSR", nullKCSR, "RefKVSRYR", nullKvsr, "RefR", nullOutcomesCls });
                PumpFactRow(dsFOYROutcomes.Tables[0], rowValues,
                    new object[] { "AssignedReport", 15, "PerformedReport", 25, "RefMeansType", refMeansTypeTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 8, "RefFKR", refFkr, "RefEKRFOYR", refEkr, 
                            "RefKVR", nullKVR, "RefKCSR", nullKCSR, "RefKVSRYR", nullKvsr, "RefR", nullOutcomesCls });
            }
            else
            {
                PumpFactRow(dsFOYROutcomes.Tables[0], rowValues,
                    new object[] { "AssignedReport", 6, "PerformedReport", 14, "RefMeansType", refMeansTypeTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 1, "RefFKR", refFkr, "RefEKRFOYR", refEkr, 
                            "RefKVR", nullKVR, "RefKCSR", nullKCSR, "RefKVSRYR", nullKvsr, "RefR", nullOutcomesCls });
                PumpFactRow(dsFOYROutcomes.Tables[0], rowValues,
                    new object[] { "AssignedReport", 7, "PerformedReport", 15, "RefMeansType", refMeansTypeTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 2, "RefFKR", refFkr, "RefEKRFOYR", refEkr, 
                            "RefKVR", nullKVR, "RefKCSR", nullKCSR, "RefKVSRYR", nullKvsr, "RefR", nullOutcomesCls });
                PumpFactRow(dsFOYROutcomes.Tables[0], rowValues,
                    new object[] { "AssignedReport", 8, "PerformedReport", 16, "RefMeansType", refMeansTypeTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 3, "RefFKR", refFkr, "RefEKRFOYR", refEkr, 
                            "RefKVR", nullKVR, "RefKCSR", nullKCSR, "RefKVSRYR", nullKvsr, "RefR", nullOutcomesCls });
                PumpFactRow(dsFOYROutcomes.Tables[0], rowValues,
                    new object[] { "AssignedReport", 9, "PerformedReport", 17, "RefMeansType", refMeansTypeTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 11, "RefFKR", refFkr, "RefEKRFOYR", refEkr, 
                            "RefKVR", nullKVR, "RefKCSR", nullKCSR, "RefKVSRYR", nullKvsr, "RefR", nullOutcomesCls });
                PumpFactRow(dsFOYROutcomes.Tables[0], rowValues,
                    new object[] { "AssignedReport", 10, "PerformedReport", 18, "RefMeansType", refMeansTypeTxt,
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 4, "RefFKR", refFkr, "RefEKRFOYR", refEkr, 
                            "RefKVR", nullKVR, "RefKCSR", nullKCSR, "RefKVSRYR", nullKvsr, "RefR", nullOutcomesCls });
                PumpFactRow(dsFOYROutcomes.Tables[0], rowValues,
                    new object[] { "AssignedReport", 11, "PerformedReport", 19, "RefMeansType", refMeansTypeTxt,
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 5, "RefFKR", refFkr, "RefEKRFOYR", refEkr, 
                            "RefKVR", nullKVR, "RefKCSR", nullKCSR, "RefKVSRYR", nullKvsr, "RefR", nullOutcomesCls });
                PumpFactRow(dsFOYROutcomes.Tables[0], rowValues,
                    new object[] { "AssignedReport", 12, "PerformedReport", 20, "RefMeansType", refMeansTypeTxt,
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 6, "RefFKR", refFkr, "RefEKRFOYR", refEkr, 
                            "RefKVR", nullKVR, "RefKCSR", nullKCSR, "RefKVSRYR", nullKvsr, "RefR", nullOutcomesCls });
                PumpFactRow(dsFOYROutcomes.Tables[0], rowValues,
                    new object[] { "AssignedReport", 13, "PerformedReport", 21, "RefMeansType", refMeansTypeTxt,
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 8, "RefFKR", refFkr, "RefEKRFOYR", refEkr, 
                            "RefKVR", nullKVR, "RefKCSR", nullKCSR, "RefKVSRYR", nullKvsr, "RefR", nullOutcomesCls });
            }
        }

        private void PumpDefProf428Txt(string[] rowValues)
        {
            if (this.DataSource.Year >= 2011)
            {
                PumpFactRow(dsFOYRDefProf.Tables[0], rowValues,
                    new object[] { "AssignedReport", 6, "PerformedReport", 16, "RefMeansType", refMeansTypeTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 1 });
                PumpFactRow(dsFOYRDefProf.Tables[0], rowValues,
                    new object[] { "AssignedReport", 8, "PerformedReport", 18, "RefMeansType", refMeansTypeTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 2 });
                PumpFactRow(dsFOYRDefProf.Tables[0], rowValues,
                    new object[] { "AssignedReport", 10, "PerformedReport", 20, "RefMeansType", refMeansTypeTxt,
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 3 });
                PumpFactRow(dsFOYRDefProf.Tables[0], rowValues,
                    new object[] { "AssignedReport", 11, "PerformedReport", 21, "RefMeansType", refMeansTypeTxt,
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 11 });
                PumpFactRow(dsFOYRDefProf.Tables[0], rowValues,
                    new object[] { "AssignedReport", 12, "PerformedReport", 22, "RefMeansType", refMeansTypeTxt,
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 4 });
                PumpFactRow(dsFOYRDefProf.Tables[0], rowValues,
                    new object[] { "AssignedReport", 13, "PerformedReport", 23, "RefMeansType", refMeansTypeTxt,
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 5 });
                PumpFactRow(dsFOYRDefProf.Tables[0], rowValues,
                    new object[] { "AssignedReport", 14, "PerformedReport", 24, "RefMeansType", refMeansTypeTxt,
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 6 });
                PumpFactRow(dsFOYRDefProf.Tables[0], rowValues,
                    new object[] { "AssignedReport", 15, "PerformedReport", 25, "RefMeansType", refMeansTypeTxt,
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 8 });
            }
            else
            {
                PumpFactRow(dsFOYRDefProf.Tables[0], rowValues,
                    new object[] { "AssignedReport", 6, "PerformedReport", 14, "RefMeansType", refMeansTypeTxt,
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 1 });
                PumpFactRow(dsFOYRDefProf.Tables[0], rowValues,
                    new object[] { "AssignedReport", 7, "PerformedReport", 15, "RefMeansType", refMeansTypeTxt,
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 2 });
                PumpFactRow(dsFOYRDefProf.Tables[0], rowValues,
                    new object[] { "AssignedReport", 8, "PerformedReport", 16, "RefMeansType", refMeansTypeTxt,
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 3 });
                PumpFactRow(dsFOYRDefProf.Tables[0], rowValues,
                    new object[] { "AssignedReport", 9, "PerformedReport", 17, "RefMeansType", refMeansTypeTxt,
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 11 });
                PumpFactRow(dsFOYRDefProf.Tables[0], rowValues,
                    new object[] { "AssignedReport", 10, "PerformedReport", 18, "RefMeansType", refMeansTypeTxt,
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 4 });
                PumpFactRow(dsFOYRDefProf.Tables[0], rowValues,
                    new object[] { "AssignedReport", 11, "PerformedReport", 19, "RefMeansType", refMeansTypeTxt,
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 5 });
                PumpFactRow(dsFOYRDefProf.Tables[0], rowValues,
                    new object[] { "AssignedReport", 12, "PerformedReport", 20, "RefMeansType", refMeansTypeTxt,
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 6 });
                PumpFactRow(dsFOYRDefProf.Tables[0], rowValues,
                    new object[] { "AssignedReport", 13, "PerformedReport", 21, "RefMeansType", refMeansTypeTxt,
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 8 });
            }
        }

        private void PumpFin428Txt(string[] rowValues, int refSrcInFin)
        {
            if (this.DataSource.Year >= 2011)
            {
                PumpFactRow(dsFOYRSrcFin.Tables[0], rowValues,
                    new object[] { "AssignedReport", 3, "PerformedReport", 13, "ExcSumPRep", 4, "ExcSumFRep", 14, 
                            "RefMeansType", refMeansTypeTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 1, "RefKIF2005", refSrcInFin, "RefKIF2004", nullKIF2004 });
                PumpFactRow(dsFOYRSrcFin.Tables[0], rowValues,
                    new object[] { "AssignedReport", 5, "PerformedReport", 15, "ExcSumPRep", 6, "ExcSumFRep", 16, 
                            "RefMeansType", refMeansTypeTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 2, "RefKIF2005", refSrcInFin, "RefKIF2004", nullKIF2004  });
                PumpFactRow(dsFOYRSrcFin.Tables[0], rowValues,
                    new object[] { "AssignedReport", 7, "PerformedReport", 17, "RefMeansType", refMeansTypeTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 3, "RefKIF2005", refSrcInFin, "RefKIF2004", nullKIF2004  });
                PumpFactRow(dsFOYRSrcFin.Tables[0], rowValues,
                    new object[] { "AssignedReport", 8, "PerformedReport", 18, "RefMeansType", refMeansTypeTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 11, "RefKIF2005", refSrcInFin, "RefKIF2004", nullKIF2004  });
                PumpFactRow(dsFOYRSrcFin.Tables[0], rowValues,
                    new object[] { "AssignedReport", 9, "PerformedReport", 19, "RefMeansType", refMeansTypeTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 4, "RefKIF2005", refSrcInFin, "RefKIF2004", nullKIF2004 });
                PumpFactRow(dsFOYRSrcFin.Tables[0], rowValues,
                    new object[] { "AssignedReport", 10, "PerformedReport", 20, "RefMeansType", refMeansTypeTxt,
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 5, "RefKIF2005", refSrcInFin, "RefKIF2004", nullKIF2004  });
                PumpFactRow(dsFOYRSrcFin.Tables[0], rowValues,
                    new object[] { "AssignedReport", 11, "PerformedReport", 21, "RefMeansType", refMeansTypeTxt,
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 6, "RefKIF2005", refSrcInFin, "RefKIF2004", nullKIF2004  });
                PumpFactRow(dsFOYRSrcFin.Tables[0], rowValues,
                    new object[] { "AssignedReport", 12, "PerformedReport", 22, "RefMeansType", refMeansTypeTxt,
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 8, "RefKIF2005", refSrcInFin, "RefKIF2004", nullKIF2004  });
            }
            else
            {
                PumpFactRow(dsFOYRSrcFin.Tables[0], rowValues,
                    new object[] { "AssignedReport", 3, "PerformedReport", 11, "RefMeansType", refMeansTypeTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 1, "RefKIF2005", refSrcInFin, "RefKIF2004", nullKIF2004  });
                PumpFactRow(dsFOYRSrcFin.Tables[0], rowValues,
                    new object[] { "AssignedReport", 4, "PerformedReport", 12, "RefMeansType", refMeansTypeTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 2, "RefKIF2005", refSrcInFin, "RefKIF2004", nullKIF2004  });
                PumpFactRow(dsFOYRSrcFin.Tables[0], rowValues,
                    new object[] { "AssignedReport", 5, "PerformedReport", 13, "RefMeansType", refMeansTypeTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 3, "RefKIF2005", refSrcInFin, "RefKIF2004", nullKIF2004  });
                PumpFactRow(dsFOYRSrcFin.Tables[0], rowValues,
                    new object[] { "AssignedReport", 6, "PerformedReport", 14, "RefMeansType", refMeansTypeTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 11, "RefKIF2005", refSrcInFin, "RefKIF2004", nullKIF2004  });
                PumpFactRow(dsFOYRSrcFin.Tables[0], rowValues,
                    new object[] { "AssignedReport", 7, "PerformedReport", 15, "RefMeansType", refMeansTypeTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 4, "RefKIF2005", refSrcInFin, "RefKIF2004", nullKIF2004  });
                PumpFactRow(dsFOYRSrcFin.Tables[0], rowValues,
                    new object[] { "AssignedReport", 8, "PerformedReport", 16, "RefMeansType", refMeansTypeTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 5, "RefKIF2005", refSrcInFin, "RefKIF2004", nullKIF2004  });
                PumpFactRow(dsFOYRSrcFin.Tables[0], rowValues,
                    new object[] { "AssignedReport", 9, "PerformedReport", 17, "RefMeansType", refMeansTypeTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 6, "RefKIF2005", refSrcInFin, "RefKIF2004", nullKIF2004  });
                PumpFactRow(dsFOYRSrcFin.Tables[0], rowValues,
                    new object[] { "AssignedReport", 10, "PerformedReport", 18, "RefMeansType", refMeansTypeTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 8, "RefKIF2005", refSrcInFin, "RefKIF2004", nullKIF2004  });
            }
        }

        #endregion форма 428

/*        #region форма 428V

        private void PumpIncomes428VTxt(string[] rowValues, int refKd)
        {
            PumpFactRow(dsMonthRepIncomes.Tables[0], rowValues,
                new object[] { "YearPlanReport", 3, "FactReport", 6, "SpreadFactYearPlanReport", 9, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 2, "RefKD", refKd });
            PumpFactRow(dsMonthRepIncomes.Tables[0], rowValues,
                new object[] { "YearPlanReport", 4, "FactReport", 7, "SpreadFactYearPlanReport", 10, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 3, "RefKD", refKd });
            PumpFactRow(dsMonthRepIncomes.Tables[0], rowValues,
                new object[] { "YearPlanReport", 5, "FactReport", 8, "SpreadFactYearPlanReport", 11, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 7, "RefKD", refKd });
        }

        private void PumpOutcomes428VTxt(string[] rowValues, int refFkr, int refEkr)
        {
            PumpFactRow(dsMonthRepOutcomes.Tables[0], rowValues,
                new object[] { "YearPlanReport", 6, "FactReport", 9, "SpreadFactYearPlanReport", 12, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 2, "RefFKR", refFkr, "RefEKR", refEkr });
            PumpFactRow(dsMonthRepOutcomes.Tables[0], rowValues,
                new object[] { "YearPlanReport", 7, "FactReport", 10, "SpreadFactYearPlanReport", 13, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 3, "RefFKR", refFkr, "RefEKR", refEkr });
            PumpFactRow(dsMonthRepOutcomes.Tables[0], rowValues,
                new object[] { "YearPlanReport", 8, "FactReport", 11, "SpreadFactYearPlanReport", 14, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 7, "RefFKR", refFkr, "RefEKR", refEkr });
        }

        private void PumpDefProf428VTxt(string[] rowValues)
        {
            PumpFactRow(dsMonthRepDefProf.Tables[0], rowValues,
                new object[] { "YearPlanReport", 6, "FactReport", 9, "SpreadFactYearPlanReport", 12, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 2 });
            PumpFactRow(dsMonthRepDefProf.Tables[0], rowValues,
                new object[] { "YearPlanReport", 7, "FactReport", 10, "SpreadFactYearPlanReport", 13, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 3 });
            PumpFactRow(dsMonthRepDefProf.Tables[0], rowValues,
                new object[] { "YearPlanReport", 8, "FactReport", 11, "SpreadFactYearPlanReport", 14, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 7 });
        }

        private void PumpInFin428VTxt(string[] rowValues, int refSrcInFin)
        {
            PumpFactRow(dsMonthRepInFin.Tables[0], rowValues,
                new object[] { "YearPlanReport", 3, "FactReport", 6, "SpreadFactYearPlanReport", 9, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 2, "RefSIF", refSrcInFin });
            PumpFactRow(dsMonthRepInFin.Tables[0], rowValues,
                new object[] { "YearPlanReport", 4, "FactReport", 7, "SpreadFactYearPlanReport", 10, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 3, "RefSIF", refSrcInFin });
            PumpFactRow(dsMonthRepInFin.Tables[0], rowValues,
                new object[] { "YearPlanReport", 5, "FactReport", 8, "SpreadFactYearPlanReport", 11, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 7, "RefSIF", refSrcInFin });
        }

        #endregion форма 428V

        #region форма 117

        private void PumpIncomes117Txt(string[] rowValues, int refKd)
        {
            PumpFactRow(dsMonthRepIncomes.Tables[0], rowValues,
                new object[] { "YearPlanReport", 3, "FactReport", 4, "SpreadFactYearPlanReport", 5, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", sourceBdgLvl, "RefKD", refKd });
        }

        private void PumpOutcomes117Txt(string[] rowValues, int refFkr, int refEkr)
        {
            PumpFactRow(dsMonthRepOutcomes.Tables[0], rowValues,
                new object[] { "YearPlanReport", 6, "FactReport", 7, "SpreadFactYearPlanReport", 8, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", sourceBdgLvl, "RefFKR", refFkr, "RefEKR", refEkr });
        }

        private void PumpDefProf117Txt(string[] rowValues)
        {
            PumpFactRow(dsMonthRepDefProf.Tables[0], rowValues,
                new object[] { "YearPlanReport", 6, "FactReport", 7, "SpreadFactYearPlanReport", 8, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", sourceBdgLvl });
        }

        private void PumpOutFin117Txt(string[] rowValues, int refSrcOutFin)
        {
            PumpFactRow(dsMonthRepOutFin.Tables[0], rowValues,
                new object[] { "YearPlanReport", 3, "FactReport", 4, "SpreadFactYearPlanReport", 5, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", sourceBdgLvl, "RefSOF", refSrcOutFin });
        }

        private void PumpInFin117Txt(string[] rowValues, int refSrcInFin)
        {
            PumpFactRow(dsMonthRepInFin.Tables[0], rowValues,
                new object[] { "YearPlanReport", 3, "FactReport", 4, "SpreadFactYearPlanReport", 5, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", sourceBdgLvl, "RefSIF", refSrcInFin });
        }

        #endregion форма 117*/

        private void PumpMainFormTxt(string row)
        {
            object[] mapping = null;
            string code = string.Empty;
            string[] rowValues = row.Split('|');
            int kst = Convert.ToInt32(rowValues[0]);
            switch (curBlock)
            {
                case 1:
                    if (!toPumpIncomes)
                        break;
                    // доходы
                    code = rowValues[2].PadLeft(20, '0');
                    mapping = new object[] { "CodeStr", code, "Name", constDefaultClsName, "KL", 0, "KST", 0 };
                    int refKd = PumpCachedRow(kdCache, dsKD.Tables[0], clsKD, code, mapping);
                    switch (formTxt)
                    {
                        case "428":
                            PumpIncomes428Txt(rowValues, refKd);
                            break;
                        case "428V":
                     //       PumpIncomes428VTxt(rowValues, refKd);
                            break;
                        case "117":
                         //   PumpIncomes117Txt(rowValues, refKd);
                            break;
                    }
                    break;
                case 2:
                    if (kst.ToString().StartsWith("2"))
                    {
                        if (!toPumpOutcomes)
                            break;
                        // расходы
                        string fkrCode = string.Format("{0}", Convert.ToInt32(rowValues[2]));
                        mapping = new object[] { "Code", fkrCode, "Name", constDefaultClsName };
                        int refFkr = PumpCachedRow(fkrCache, dsFKR.Tables[0], clsFKR, fkrCode, mapping);

                        string ekrCode = Convert.ToInt32(rowValues[5]).ToString();
                        mapping = new object[] { "Code", ekrCode, "Name", constDefaultClsName };
                        int refEkr = PumpCachedRow(ekrCache, dsEKR.Tables[0], clsEKR, ekrCode, mapping);

                        switch (formTxt)
                        {
                            case "428":
                                PumpOutcomes428Txt(rowValues, refFkr, refEkr);
                                break;
                            case "428V":
                           //     PumpOutcomes428VTxt(rowValues, refFkr, refEkr);
                                break;
                            case "117":
                             //   PumpOutcomes117Txt(rowValues, refFkr, refEkr);
                                break;
                        }
                    }
                    else if (kst == 450)
                    {
                        // деф проф
                        if (!toPumpDefProf)
                            break;
                        switch (formTxt)
                        {
                            case "428":
                                PumpDefProf428Txt(rowValues);
                                break;
                            case "428V":
                              //  PumpDefProf428VTxt(rowValues);
                                break;
                            case "117":
                           //     PumpDefProf117Txt(rowValues);
                                break;
                        }
                    }
                    break;
                case 3:
                    // источники внешнего финансирования
                    if (!toPumpFinSources)
                        break;
                    code = rowValues[2].PadLeft(20, '0');
                    mapping = new object[] { "CodeStr", code, "Name", constDefaultClsName, "KL", 0, "KST", 0 };
                    int refOutFin = PumpCachedRow(kifCache, dsKIF2005.Tables[0], clsKIF2005, code, mapping);
                    switch (formTxt)
                    {
                        case "428":
                            PumpFin428Txt(rowValues, refOutFin);
                            break;
                        case "428V":
                           // PumpFin428VTxt(rowValues, refOutFin);
                            break;
                        case "117":
                          //  PumpFin117Txt(rowValues, refOutFin);
                            break;
                    }
                    break;
            }
        }

        private void PumpReportRowTxt(string row)
        {
            switch (formTxt)
            {
                case "428":
                case "428V":
                case "117":
                    PumpMainFormTxt(row);
                    break;
            }
        }

        private string GetFormTxt(string fileName)
        {
            if (fileName.ToUpper().Contains("428V"))
                return "428V";
            else if (fileName.ToUpper().Contains("428"))
                return "428";
            else if (fileName.ToUpper().Contains("117"))
                return "117";
            else
                return string.Empty;
        }

        private void GetSourceBdgLvl(string sourceKind)
        {
            switch (sourceKind)
            {
                case "4":
                    sourceBdgLvl = 2;
                    break;
                case "2":
                    sourceBdgLvl = 3;
                    break;
                case "8":
                    sourceBdgLvl = 4;
                    break;
                case "5":
                    sourceBdgLvl = 10;
                    break;
                default:
                    sourceBdgLvl = 10;
                    break;
            }
        }

        private void ProcessTxtFile(FileInfo file)
        {
            formTxt = GetFormTxt(file.Name);
            refMeansTypeTxt = 1;
            if (formTxt == "428V")
                refMeansTypeTxt = 2;
            string[] reportData = CommonRoutines.GetTxtReportData(file, CommonRoutines.GetTxtWinCodePage());
            bool toPumpRow = false;
            int rowIndex = 0;
            string sourceKindTxt = string.Empty;
            foreach (string row in reportData)
            {
                try
                {
                    rowIndex++;
                    string auxRow = row.Replace("\n", string.Empty).Trim();
                    if (auxRow.ToUpper().StartsWith("ВИД"))
                    {
                        sourceKindTxt = auxRow.Split('=')[1].Trim();
                        // для 117 формы уровень бюджета получаем из секции отчета
                        if (formTxt == "117")
                            GetSourceBdgLvl(sourceKindTxt);
                    }
                    if (auxRow.ToUpper().StartsWith("ИСТ"))
                    {
                        string regionSource = auxRow.Split('=')[1].Trim().TrimStart('0').PadLeft(1, '0');
                        string regionCode = string.Format("{0}{1}", regionSource, sourceKindTxt).PadLeft(10, '0');
                        if (!PumpRegionsTxt(regionCode))
                        {
                            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                                string.Format("В классификаторе 'Районы.Служебный для закачки СКИФ' отсутствовала запись с кодом '{0}'. Отчет: {1}.",
                                               regionCode, file.Name));
                        }
                    }
                    if (auxRow.ToUpper().StartsWith("ТБ"))
                        curBlock = Convert.ToInt32(auxRow.Split('=')[1].Trim());

                    if (auxRow == "#")
                        toPumpRow = false;
                    if (toPumpRow)
                        PumpReportRowTxt(auxRow);
                    if (auxRow == "#$")
                        toPumpRow = true;
                }
                catch (Exception exp)
                {
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                        string.Format("Ошибка при обработке строки {0} отчета {1}: {2}", rowIndex, file.Name, exp.Message));
                }
            }
            UpdateData();
        }

        private void ProcessTxtDir(DirectoryInfo dir)
        {
            ProcessFilesTemplate(dir, "428*.txt", new ProcessFileDelegate(ProcessTxtFile), false);
        }

        private void PumpFactTxt(DirectoryInfo dir)
        {
            ProcessTxtDir(dir);
        }

        #endregion работа с фактами

        #region общая организация закачки

        protected override void PumpTxtReports(DirectoryInfo dir)
        {
            refDateTxt = this.DataSource.Year * 10000 + 1;
            if (dir.GetFiles("*.txt", SearchOption.AllDirectories).GetLength(0) == 0)
                return;
            GetPumpedBlocks();
            InitClsTxt();
            PumpClsTxt();
            PumpFactTxt(dir);
            UpdateData();
            ClearClsTxt();
        }

        #endregion общая организация закачки 

    }
}
