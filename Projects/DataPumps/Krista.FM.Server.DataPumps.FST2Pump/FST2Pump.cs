using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using Krista.FM.Common.Xml;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.FST2Pump
{
    // ФСТ - 0002 - Тарифы ЖКХ по МО
    public class FST2PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // ФСТ.Виды благоустройств (d_FTS_GKHType)
        private IDbDataAdapter daGKHType;
        private DataSet dsGKHType;
        private IClassifier clsGKHType;
        private Dictionary<string, int> cacheGKHType = null;
        // ФСТ.Детали учета (d_FTS_Details)
        private IDbDataAdapter daDetails;
        private DataSet dsDetails;
        private IClassifier clsDetails;
        private Dictionary<string, int> cacheDetails = null;
        // ФСТ.Организации ЖКХ (d_FTS_GKHOrg)
        private IDbDataAdapter daGKHOrg;
        private DataSet dsGKHOrg;
        private IClassifier clsGKHOrg;
        private Dictionary<string, int> cacheGKHOrg = null;
        private Dictionary<string, int> cacheGKHOrgForm = null;
        // ФСТ.Услуги ЖКХ (d_FTS_ServicesGKH)
        private IDbDataAdapter daServicesGKH;
        private DataSet dsServicesGKH;
        private IClassifier clsServicesGKH;
        private Dictionary<string, int> cacheServicesGKH = null;
        // ФСТ.Услуги ЖКХ по форме 22 (d_FTS_Services22GKH)
        private IDbDataAdapter daServices22GKH;
        private DataSet dsServices22GKH;
        private IClassifier clsServices22GKH;
        private Dictionary<string, int> cacheServices22GKH = null;
        // Территории.РФ (d_Territory_RF)
        private IDbDataAdapter daTerritory;
        private DataSet dsTerritory;
        private IClassifier clsTerritory;
        private Dictionary<int, int> cacheTerritory = null;
        private int nullTerritory = -1;
        // ФСТ.Показатели Формы П3 (d_FTS_FormP3Index)
        private IDbDataAdapter daFormP3Index;
        private DataSet dsFormP3Index;
        private IClassifier clsFormP3Index;
        private Dictionary<string, int> cacheFormP3Index = null;
        // ФСТ.Показатели Формы 22ЖКХ (d_FTS_Form22Index)
        private IDbDataAdapter daForm22Index;
        private DataSet dsForm22Index;
        private IClassifier clsForm22Index;
        private Dictionary<string, int> cacheForm22Index = null;
        // ФСТ.Показатели Формы №1 БухБаланс (d_FTS_BIndex)
        private IDbDataAdapter daBIndex;
        private DataSet dsBIndex;
        private IClassifier clsBIndex;
        private Dictionary<string, int> cacheBIndex = null;

        #endregion Классификаторы

        #region Факты

        // ФСТ.ФСТ_ЖКХ по МО_Тарифы и нормативы (f_FTS_MOTariff)
        private IDbDataAdapter daMOTariff;
        private DataSet dsMOTariff;
        private IFactTable fctMOTariff;
        // ФСТ.ФСТ_ЖКХ по МО_Форма П3 (f_FTS_FormP3)
        private IDbDataAdapter daFormP3;
        private DataSet dsFormP3;
        private IFactTable fctFormP3;
        // ФСТ.ФСТ_ЖКХ по МО_Форма 22ЖКХ (f_FTS_Form22GKH)
        private IDbDataAdapter daForm22GKH;
        private DataSet dsForm22GKH;
        private IFactTable fctForm22GKH;
        // ФСТ.ФСТ_ЖКХ по МО_Форма №1 БухБаланс (f_FTS_BBalance)
        private IDbDataAdapter daBBalance;
        private DataSet dsBBalance;
        private IFactTable fctBBalance;

        #endregion Факты

        // тип отчета
        private ReportType reportType;
        // осуществляется закачка заголовочного файла
        private bool pumpingTitle = false;
        // заголовочная информация из файла imon_buhg_1_tit.xml
        private Dictionary<int, RecordInfo> titleRecords = null;
        // осуществляется закачка списка территорий
        private bool pumpingMO = false;
        // список территорий из файла imon_nt_ku*_list_mo.xml
        private Dictionary<int, int> moList = null;
        // список дат, за которые уже были удалены данные
        private Dictionary<ReportType, List<int>> deletedDateList = null;
        private List<int> absentTerritoryCodes = null;
        // список записей из файла ntku11_m_list_org(*).xml,
        // которые будут использоваться при закачке файла ntku11_m_ku_avg(*).xml
        // ключом является связка значений тегов LGL_ID и NOMER_ID
        private Dictionary<string, Dictionary<string, string>> orgRecords = null;
        private Dictionary<string, string[]> auxOrgCache = null;
        // дата закачки
        private int refPumpDate;

        #endregion Поля

        #region Структуры, перечисления

        // структура записи
        private struct RecordInfo
        {
            public int refDate;
            public int refTerritory;
            public int refOrg;
        }

        /// <summary>
        /// Тип отчета
        /// </summary>
        private enum ReportType
        {
            /// <summary>
            /// Тарифы и нормативы (файлы imon_nt_ku*_ku.xml)
            /// </summary>
            MOTariff,
            /// <summary>
            /// Список территорий (файлы imon_nt_ku*_list_mo.xml)
            /// </summary>
            MOList,
            /// <summary>
            /// Оперативные тарифы (файлы ntku11_m_ku_avg(*).xml, ntku11_m_list_org(*).xml)
            /// </summary>
            MOOperative,
            /// <summary>
            /// Форма П-3 (файл imon_p3_r1.xml)
            /// </summary>
            FormP3,
            /// <summary>
            /// Форма 22 ЖКХ (файл imon_jkh22_opn.xml)
            /// </summary>
            Form22GKH,
            /// <summary>
            /// Форма №1 БухБаланс (файлы imon_buhg_1_aktiv.xml, imon_buhg_1_passiv.xml)
            /// </summary>
            FormN1,
            /// <summary>
            /// Форма №1 БухБаланс (файл imon_buhg_1_tit.xml)
            /// </summary>
            FormN1Tit,
            /// <summary>
            /// Неизвестный формат
            /// </summary>
            Unknown
        }

        #endregion Структуры, перечисления

        #region Делегаты

        // Функция обработки строки Xml-отчёта
        private delegate void PumpXmlRow(Dictionary<string, string> record);

        #endregion Делегаты

        #region Закачка данных

        #region Работа с базой и кэшами

        private void FillCaches()
        {
            FillRowsCache(ref cacheGKHType, dsGKHType.Tables[0], "Name", "ID");
            FillRowsCache(ref cacheDetails, dsDetails.Tables[0], "Name", "ID");
            FillRowsCache(ref cacheGKHOrgForm, dsGKHOrg.Tables[0], new string[] { "INN", "KPP" }, "|", "ID");
            FillRowsCache(ref cacheGKHOrg, dsGKHOrg.Tables[0], new string[] { "INN", "KPP", "Fil", "FilName" }, "|", "ID");
            FillRowsCache(ref cacheServicesGKH, dsServicesGKH.Tables[0], "Name", "ID");
            FillRowsCache(ref cacheServices22GKH, dsServices22GKH.Tables[0], "Code", "ID");
            FillRowsCache(ref cacheTerritory, dsTerritory.Tables[0], "Code", "ID");
            FillRowsCache(ref cacheFormP3Index, dsFormP3Index.Tables[0], "Code", "ID");
            FillRowsCache(ref cacheForm22Index, dsForm22Index.Tables[0], "Code", "ID");
            FillRowsCache(ref cacheBIndex, dsBIndex.Tables[0], "PNumber", "ID");
        }

        protected override void QueryData()
        {
            InitDataSet(ref daGKHType, ref dsGKHType, clsGKHType, string.Empty);
            InitDataSet(ref daDetails, ref dsDetails, clsDetails, string.Empty);
            InitDataSet(ref daGKHOrg, ref dsGKHOrg, clsGKHOrg, string.Empty);
            InitDataSet(ref daServicesGKH, ref dsServicesGKH, clsServicesGKH, string.Empty);
            InitDataSet(ref daServices22GKH, ref dsServices22GKH, clsServices22GKH, string.Empty);
            InitDataSet(ref daTerritory, ref dsTerritory, clsTerritory, string.Empty);
            InitDataSet(ref daFormP3Index, ref dsFormP3Index, clsFormP3Index, string.Empty);
            InitDataSet(ref daForm22Index, ref dsForm22Index, clsForm22Index, string.Empty);
            InitDataSet(ref daBIndex, ref dsBIndex, clsBIndex, string.Empty);

            InitFactDataSet(ref daMOTariff, ref dsMOTariff, fctMOTariff);
            InitFactDataSet(ref daFormP3, ref dsFormP3, fctFormP3);
            InitFactDataSet(ref daForm22GKH, ref dsForm22GKH, fctForm22GKH);
            InitFactDataSet(ref daBBalance, ref dsBBalance, fctBBalance);

            FillCaches();
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daGKHType, dsGKHType, clsGKHType);
            UpdateDataSet(daDetails, dsDetails, clsDetails);
            UpdateDataSet(daGKHOrg, dsGKHOrg, clsGKHOrg);
            UpdateDataSet(daServicesGKH, dsServicesGKH, clsServicesGKH);
            UpdateDataSet(daServices22GKH, dsServices22GKH, clsServices22GKH);

            UpdateDataSet(daMOTariff, dsMOTariff, fctMOTariff);
            UpdateDataSet(daFormP3, dsFormP3, fctFormP3);
            UpdateDataSet(daForm22GKH, dsForm22GKH, fctForm22GKH);
            UpdateDataSet(daBBalance, dsBBalance, fctBBalance);
        }

        #region GUIDs

        private const string D_FTS_GKH_TYPE_GUID = "6c84e3f7-2936-42fc-82b8-a69755cc7a16";
        private const string D_FTS_DETAILS_GUID = "ee3b53f3-ad02-499b-8e87-3386bf7fa4c9";
        private const string D_FTS_GKH_ORG_GUID = "eec98cb1-a89f-4d6a-8320-36210588cad2";
        private const string D_FTS_SERVICES_GKH_GUID = "c328659d-b21a-4dc9-9327-27419f8d4951";
        private const string D_FTS_SERVICES_22_GKH_GUID = "0bc3bc5f-ca56-4080-a67c-932eaaacf26b";
        private const string D_TERRITORY_RF_GUID = "66b9a66d-85ca-41de-910e-f9e6cb483960";
        private const string D_FTS_FORM_P3_INDEX_GUID = "14328d66-2306-4699-8c56-6616dec6d551";
        private const string D_FTS_FORM_22_INDEX_GUID = "ceb54e83-5c8f-4839-944d-df84be5464da";
        private const string D_FTS_B_INDEX_GUID = "81e78b1a-904b-4556-bf69-bc8331ff678b";

        private const string F_FTS_MOTARIFF_GUID = "95219be5-b6fb-4a91-ae78-e3bc49267c1e";
        private const string F_FTS_FORM_P3_GUID = "bd4bb3cb-0bf1-444c-a088-8ac1b182c37b";
        private const string F_FTS_FORM_22_GKH_GUID = "a9b555ff-265b-4708-b0ec-1a0976d0673e";
        private const string F_FTS_B_BALANCE_GUID = "6335cfa2-37df-4d36-90a0-da167d0d749b";

        #endregion GUIDs
        protected override void InitDBObjects()
        {
            clsTerritory = this.Scheme.Classifiers[D_TERRITORY_RF_GUID];
            clsGKHType = this.Scheme.Classifiers[D_FTS_GKH_TYPE_GUID];
            clsDetails = this.Scheme.Classifiers[D_FTS_DETAILS_GUID];
            clsGKHOrg = this.Scheme.Classifiers[D_FTS_GKH_ORG_GUID];
            clsServicesGKH = this.Scheme.Classifiers[D_FTS_SERVICES_GKH_GUID];
            clsServices22GKH = this.Scheme.Classifiers[D_FTS_SERVICES_22_GKH_GUID];
            clsFormP3Index = this.Scheme.Classifiers[D_FTS_FORM_P3_INDEX_GUID];
            clsForm22Index = this.Scheme.Classifiers[D_FTS_FORM_22_INDEX_GUID];
            clsBIndex = this.Scheme.Classifiers[D_FTS_B_INDEX_GUID];

            fctMOTariff = this.Scheme.FactTables[F_FTS_MOTARIFF_GUID];
            fctFormP3 = this.Scheme.FactTables[F_FTS_FORM_P3_GUID];
            fctForm22GKH = this.Scheme.FactTables[F_FTS_FORM_22_GKH_GUID];
            fctBBalance = this.Scheme.FactTables[F_FTS_B_BALANCE_GUID];

            this.UsedClassifiers = new IClassifier[] { };
            this.UsedFacts = new IFactTable[] { fctMOTariff };
            this.CubeFacts = new IFactTable[] { fctMOTariff, fctFormP3, fctForm22GKH, fctBBalance };
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsMOTariff);
            ClearDataSet(ref dsFormP3);
            ClearDataSet(ref dsForm22GKH);
            ClearDataSet(ref dsBBalance);

            ClearDataSet(ref dsGKHType);
            ClearDataSet(ref dsDetails);
            ClearDataSet(ref dsGKHOrg);
            ClearDataSet(ref dsServicesGKH);
            ClearDataSet(ref dsServices22GKH);
            ClearDataSet(ref dsTerritory);
            ClearDataSet(ref dsFormP3Index);
            ClearDataSet(ref dsForm22Index);
            ClearDataSet(ref dsBIndex);
        }

        #endregion Работа с базой и кэшами

        #region Работа с Xml

        #region Общие методы

        private decimal CleanFactValue(string value)
        {
            decimal factValue = 0;
            if (!Decimal.TryParse(value.Replace('.', ',').Trim().PadLeft(1, '0'), out factValue))
                WriteToTrace(string.Format("Обнаружено некорректное значение показателя {0}. Заменено на 0.", value),
                    TraceMessageKind.Information);
            return factValue;
        }

        private Regex regExParseTag = new Regex(@"^L[0-9_]*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private bool IsTagToPump(string tagName)
        {
            return regExParseTag.IsMatch(tagName);
        }

        private int PumpDetails(Dictionary<string, string> record)
        {
            int code = Convert.ToInt32(record["DET2_ID"].Trim().PadLeft(1, '0'));
            string name = record["DET2_NAME"].Trim();
            object[] mapping = new object[] { "Code", code, "Name", name };
            return PumpCachedRow(cacheDetails, dsDetails.Tables[0], clsDetails, name, mapping);
        }

        private int PumpGKHType(Dictionary<string, string> record, bool newFormat)
        {
            int code = 0;
            string name = string.Empty;
            if (newFormat)
            {
                code = Convert.ToInt32(record["VBLAG_ID"].Trim().PadLeft(1, '0'));
                name = record["VBLAG_NAME"].Trim();
            }
            else
            {
                code = Convert.ToInt32(record["DET_ID"].Trim().PadLeft(1, '0'));
                name = record["DET_NAME"].Trim();
            }
            object[] mapping = new object[] { "Code", code, "Name", name };
            return PumpCachedRow(cacheGKHType, dsGKHType.Tables[0], clsGKHType, name, mapping);
        }

        private int PumpGKHOrg(Dictionary<string, string> record)
        {
            int code = Convert.ToInt32(record["ORG_ID"].Trim().PadLeft(1, '0'));
            string name = record["ORG_NAME"].Trim();
            string fil = record["L1"].Trim();
            string filName = record["FIL_NAME"].Trim();
            if (fil.ToUpper() == "ДА")
                name = string.Format("{0} {1}", name, filName);

            long inn = 0;
            Int64.TryParse(record["INN_NAME"].Trim(), out inn);
            long kpp = 0;
            Int64.TryParse(record["KPP_NAME"].Trim(), out kpp);

            name += string.Format(" ИНН: {0} КПП: {1}", inn, kpp);
            string key = string.Format("{0}|{1}|{2}|{3}", inn, kpp, fil, filName);
            object[] mapping = new object[] { "Code", code, "Name", name, "Inn", inn, "Kpp", kpp, "Fil", fil, "FilName", filName };
            return PumpCachedRow(cacheGKHOrg, dsGKHOrg.Tables[0], clsGKHOrg, key, mapping);
        }

        private int PumpGKHServices(Dictionary<string, string> record, bool newFormat)
        {
            int code = 0;
            string name = string.Empty;
            string unit = record["L2"].Trim();
            if (newFormat)
            {
                code = Convert.ToInt32(record["SPHERE_ID"].Trim().PadLeft(1, '0'));
                name = record["SPHERE_NAME"].Trim();
            }
            else
            {
                code = Convert.ToInt32(record["VDET_ID"].Trim().PadLeft(1, '0'));
                name = record["VDET_NAME"].Trim();
            }
            object[] mapping = new object[] { "Code", code, "Name", name, "Unit", unit };
            return PumpCachedRow(cacheServicesGKH, dsServicesGKH.Tables[0], clsServicesGKH, name, mapping);
        }

        private int GetTerritoryRef(string codeStr)
        {
            int code = Convert.ToInt32(CommonRoutines.TrimLetters(codeStr));
            if (cacheTerritory.ContainsKey(code))
                return cacheTerritory[code];
            if (!absentTerritoryCodes.Contains(code))
                absentTerritoryCodes.Add(code);
            return nullTerritory;
        }

        private int GetTerritoryRef2011(Dictionary<string, string> record, int year)
        {
            if (year >= 2011)
            {
                int lglId = Convert.ToInt32(record["LGL_ID"].Trim().PadLeft(1, '0'));
                if (!moList.ContainsKey(lglId))
                    return nullTerritory;
                return moList[lglId];
            }
            return GetTerritoryRef(record["OKTMO_NAME"]);
        }

        private void DeleteEarlierDataByDate(int refDate)
        {
            if (!deletedDateList[reportType].Contains(refDate) && this.DeleteEarlierData)
            {
                if (reportType == ReportType.FormP3)
                    DirectDeleteFactData(new IFactTable[] { fctFormP3 }, -1, this.SourceID, string.Format("RefDate = {0}", refDate));
                if (reportType == ReportType.Form22GKH)
                    DirectDeleteFactData(new IFactTable[] { fctForm22GKH }, -1, this.SourceID, string.Format("RefDate = {0}", refDate));
                if (reportType == ReportType.FormN1Tit)
                    DirectDeleteFactData(new IFactTable[] { fctBBalance }, -1, this.SourceID, string.Format("RefPeriod = {0}", refDate));
                deletedDateList[reportType].Add(refDate);
            }
        }

        private int GetRefDate(Dictionary<string, string> record)
        {
            int refDate = -1;
            switch (reportType)
            {
                case ReportType.FormP3:
                case ReportType.Form22GKH:
                case ReportType.FormN1Tit:
                    refDate = Convert.ToInt32(record["PRD_NAME"].Trim()) * 10000;
                    string prd2Name = record["PRD2_NAME"].Trim().ToUpper();
                    if (prd2Name.Contains("КВАРТАЛ"))
                        refDate += 9991;
                    else if (prd2Name.Contains("ПОЛУГОДИЕ"))
                        refDate += 9992;
                    else if (prd2Name.Contains("МЕСЯЦЕВ"))
                        refDate += 9993;
                    else
                        refDate += 1;
                    break;
            }
            DeleteEarlierDataByDate(refDate);
            return refDate;
        }

        #endregion Общие методы

        #region Форма №1 БухБаланс

        private int PumpOrgFormN1(Dictionary<string, string> record)
        {
            int code = Convert.ToInt32(record["ORG_ID"].Trim().PadLeft(1, '0'));
            string name = record["ORG_NAME"].Trim();
            if (record["FIL_NAME"].Trim().ToUpper() != "НЕ ОПРЕДЕЛЕНО")
                name = string.Format("{0} ({1})", name, record["FIL_NAME"].Trim());
            string[] entityCode = regExParseEntityCode.Split(record["ENTITY_CODE"]);
            long inn = Convert.ToInt64(record["INN_NAME"].Trim().PadLeft(1, '0'));
            long kpp = Convert.ToInt64(record["KPP_NAME"].Trim().PadLeft(1, '0'));
            string work = record["L3"].Trim();
            string ownership = record["L6"].Trim();

            string key = string.Concat(inn, "|", kpp);
            object[] mapping = new object[] {
                "Code", code, "Name", name, "Inn", inn,
                "Kpp", kpp, "Work", work, "Ownership", ownership };
            return PumpCachedRow(cacheGKHOrgForm, dsGKHOrg.Tables[0], clsGKHOrg, key, mapping);
        }

        private void PumpFormN1Tit(Dictionary<string, string> record)
        {
            int lglId = Convert.ToInt32(record["LGL_ID"].Trim().PadLeft(1, '0'));
            RecordInfo recordInfo = new RecordInfo();
            recordInfo.refDate = GetRefDate(record);
            recordInfo.refOrg = PumpOrgFormN1(record);
            recordInfo.refTerritory = GetTerritoryRef(record["OKTMO_NAME"]);

            if (!titleRecords.ContainsKey(lglId))
                titleRecords.Add(lglId, recordInfo);
            else
                titleRecords[lglId] = recordInfo;
        }

        private void PumpFormN1(Dictionary<string, string> record)
        {
            int lglId = Convert.ToInt32(record["LGL_ID"].Trim().PadLeft(1, '0'));
            if (!titleRecords.ContainsKey(lglId))
                return;
            RecordInfo recordInfo = titleRecords[lglId];
            string factField = record["DET_NAME"].Trim().ToUpper() == "НАЧАЛО ГОДА" ? "StartPeriod" : "EndPeriod";

            foreach (KeyValuePair<string, string> tag in record)
            {
                if (!IsTagToPump(tag.Key) || tag.Key.StartsWith("L5_"))
                    continue;

                if (!cacheBIndex.ContainsKey(tag.Key))
                {
                    throw new Exception(string.Format(
                        "В классификаторе \"ФСТ.Показатели Формы №1 БухБаланс\" не найден код {0}", tag.Key));
                }

                // пустые суммы не качаем
                if (tag.Value.Trim() == string.Empty)
                    continue;

                object[] mapping = new object[] {
                    factField, CleanFactValue(tag.Value),
                    "RefTerritory", recordInfo.refTerritory,
                    "RefPeriod", recordInfo.refDate,
                    "RefGKHOrg", recordInfo.refOrg,
                    "RefBIndex", cacheBIndex[tag.Key],
                    "RefPumpDate", refPumpDate };

                PumpRow(dsBBalance.Tables[0], mapping);
                if (dsBBalance.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
                {
                    UpdateData();
                    ClearDataSet(daBBalance, ref dsBBalance);
                }
            }
        }

        #endregion

        #region Форма 22-ЖКХ

        private Regex regExParseEntityCode = new Regex(@"_INN:(.*?)_KPP:(.*)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private int PumpOrgForm22(Dictionary<string, string> record)
        {
            int code = Convert.ToInt32(record["ORG_ID"].Trim().PadLeft(1, '0'));
            string name = record["ORG_NAME"].Trim();
            if (record["FIL_NAME"].Trim().ToUpper() != "НЕ ОПРЕДЕЛЕНО")
                name = string.Format("{0} ({1})", name, record["FIL_NAME"].Trim());
            string[] entityCode = regExParseEntityCode.Split(record["ENTITY_CODE"]);
            long inn = Convert.ToInt64(entityCode[1].Trim().PadLeft(1, '0'));
            long kpp = Convert.ToInt64(entityCode[2].Trim().PadLeft(1, '0')); 

            string key = string.Concat(inn, "|", kpp);
            object[] mapping = new object[] { "Code", code, "Name", name, "Inn", inn, "Kpp", kpp };
            return PumpCachedRow(cacheGKHOrgForm, dsGKHOrg.Tables[0], clsGKHOrg, key, mapping);
        }

        private int PumpServices22GKH(Dictionary<string, string> record)
        {
            string code = record["JKU_ID"].Trim();
            string name = record["JKU_NAME"].Trim();
            return PumpCachedRow(cacheServices22GKH, dsServices22GKH.Tables[0], clsForm22Index, code,
                new object[] { "Code", code, "Name", name });
        }

        private void PumpForm22GKH(Dictionary<string, string> record)
        {
            int refOrg = PumpOrgForm22(record);
            int refServices = PumpServices22GKH(record);
            int refTerritory = GetTerritoryRef(record["OKTMO_NAME"]);
            int refDate = GetRefDate(record);
            foreach (KeyValuePair<string, string> tag in record)
            {
                if (!IsTagToPump(tag.Key))
                    continue;

                if (!cacheForm22Index.ContainsKey(tag.Key))
                {
                    throw new Exception(string.Format(
                        "В классификаторе \"ФСТ.Показатели Формы 22ЖКХ\" не найден код {0}", tag.Key));
                }

                // пустые суммы не качаем
                if (tag.Value.Trim() == string.Empty)
                    continue;

                object[] mapping = new object[] {
                    "Value", CleanFactValue(tag.Value),
                    "RefTerritory", refTerritory,
                    "RefDate", refDate,
                    "RefGKHOrg", refOrg,
                    "RefService22GKH", refServices,
                    "RefForm22Index", cacheForm22Index[tag.Key],
                    "RefPumpDate", refPumpDate };

                PumpRow(dsForm22GKH.Tables[0], mapping);
                if (dsForm22GKH.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
                {
                    UpdateData();
                    ClearDataSet(daForm22GKH, ref dsForm22GKH);
                }
            }
        }

        #endregion

        #region Форма П-3

        private int PumpOrgFormP3(Dictionary<string, string> record)
        {
            int code = Convert.ToInt32(record["ORG_ID"].Trim().PadLeft(1, '0'));
            string name = record["ORG_NAME"].Trim();
            if (record["FIL_NAME"].Trim().ToUpper() != "НЕ ОПРЕДЕЛЕНО")
                name = string.Format("{0} ({1})", name, record["FIL_NAME"].Trim());
            long inn = Convert.ToInt64(record["INN_NAME"].Trim().PadLeft(1, '0'));
            long kpp = Convert.ToInt64(record["KPP_NAME"].Trim().PadLeft(1, '0'));

            string key = string.Concat(inn, "|", kpp);
            object[] mapping = new object[] { "Code", code, "Name", name, "Inn", inn, "Kpp", kpp };
            return PumpCachedRow(cacheGKHOrgForm, dsGKHOrg.Tables[0], clsGKHOrg, key, mapping);
        }

        private void PumpFormP3(Dictionary<string, string> record)
        {
            string factField = record["DET_NAME"].Trim().ToUpper() == "ВСЕГО" ? "Total" : "Arrears";
            int refOrg = PumpOrgFormP3(record);
            int refTerritory = GetTerritoryRef(record["OKTMO_NAME"]);
            int refDate = GetRefDate(record);
            foreach (KeyValuePair<string, string> tag in record)
            {
                if (!IsTagToPump(tag.Key))
                    continue;

                if (!cacheFormP3Index.ContainsKey(tag.Key))
                {
                    throw new Exception(string.Format(
                        "В классификаторе \"ФСТ.Показатели Формы П3\" не найден код {0}", tag.Key));
                }

                // пустые суммы не качаем
                if (tag.Value.Trim() == string.Empty)
                    continue;

                object[] mapping = new object[] {
                    factField, CleanFactValue(tag.Value),
                    "RefTerritory", refTerritory,
                    "RefDate", refDate,
                    "RefGKHOrg", refOrg,
                    "RefIndex", cacheFormP3Index[tag.Key],
                    "RefPumpDate", refPumpDate };

                PumpRow(dsFormP3.Tables[0], mapping);
                if (dsFormP3.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
                {
                    UpdateData();
                    ClearDataSet(daFormP3, ref dsFormP3);
                }
            }
        }

        #endregion

        #region Тарифы и нормативы

        private void PumpFactMOTariff(Dictionary<string, string> record, int refGKHType, int refDetails,
            int refGKHOrg, int refServices, int refTerritory, int year)
        {
            object[] mapping = null;

            if (year >= 2011)
            {
                mapping = new object[] {
                    "Tariff", CleanFactValue(record["L4_2"]),
                    "Normative", CleanFactValue(record["L6_2"]),
                    "TariffLegalAct", record["L3_2"],
                    "NormativeLegalAct", record["L5_2"],
                    "RefYear", year * 10000 + 1,
                    "RefGKHType", refGKHType,
                    "RefDetail", refDetails,
                    "RefGKHOrg", refGKHOrg,
                    "RefServicesGKH", refServices,
                    "RefTerritory", refTerritory };
            }
            else
            {
                mapping = new object[] {
                    "Tariff", CleanFactValue(record["L6"]),
                    "Normative", CleanFactValue(record["L8"]),
                    "TariffLegalAct", record["L3"],
                    "IncreaseLegalAct", record["L4"],
                    "OtherPayLegalAct", record["L5"],
                    "NormativeLegalAct", record["L7"],
                    "RefYear", year * 10000 + 1,
                    "RefGKHType", refGKHType,
                    "RefDetail", refDetails,
                    "RefGKHOrg", refGKHOrg,
                    "RefServicesGKH", refServices,
                    "RefTerritory", refTerritory,
                    "RefPumpDate", refPumpDate };
            }

            PumpRow(dsMOTariff.Tables[0], mapping);
            if (dsMOTariff.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daMOTariff, ref dsMOTariff);
            }
        }

        private void PumpMOTariff(Dictionary<string, string> record)
        {
            if (record["FORM"].ToUpper() != this.DataSource.Variant.ToUpper())
                return;

            int year = Convert.ToInt32(CommonRoutines.TrimLetters(record["FORM"]));

            int refGKHType = PumpGKHType(record, year >= 2011);
            int refDetails = PumpDetails(record);
            int refGKHOrg = PumpGKHOrg(record);
            int refServices = PumpGKHServices(record, false);
            int refTerritory = GetTerritoryRef2011(record, year);

            PumpFactMOTariff(record, refGKHType, refDetails, refGKHOrg, refServices, refTerritory, year);
        }

        private void PumpMOList(Dictionary<string, string> record)
        {
            int lglId = Convert.ToInt32(record["LGL_ID"].Trim().PadLeft(1, '0'));

            if (!moList.ContainsKey(lglId))
                moList.Add(lglId, GetTerritoryRef(record["OKTMO_NAME"]));
            else
                moList[lglId] = GetTerritoryRef(record["OKTMO_NAME"]);
        }

        #endregion

        #region Оперативные тарифы

        // true - если новый номер меньше старого
        private bool CompareNomerName(string oldNomerName, string newNomerName)
        {
            string[] oldNomer = oldNomerName.Split('.');
            string[] newNomer = newNomerName.Split('.');

            int count = Math.Min(oldNomer.GetLength(0), newNomer.GetLength(0));
            for (int i = 0; i < count; i++)
            {
                if (Convert.ToInt32(oldNomer[i]) > Convert.ToInt32(newNomer[i]))
                    return true;
            }
            return false;
        }

        // из файлов ntku11_m_list_org(*).xml просто извлекаем записи и помещаем их в коллекцию
        private void PumpXmlOperativeOrg(Dictionary<string, string> orgRecord)
        {
            string scenarioName = orgRecord["SCENARIO_NAME"].ToUpper();
            if (scenarioName.Contains("PLAN"))
                return;

            // в одном и том же файле встречается несколько абсолютно одинаковых организаций (INN_ID, KPP_ID, L1, FIL_NAME)
            // с одной и той же услугой (VBLAG_ID) в одной и той же сфере (SPHERE_ID)
            // оставляем только одну из них - ту, у которой поле NOMER_NAME - наименьший
            string auxKey = string.Format("{0}|{1}|{2}|{3}|{4}|{5}", orgRecord["INN_ID"], orgRecord["KPP_ID"],
                orgRecord["VBLAG_ID"], orgRecord["SPHERE_ID"], orgRecord["L1"], orgRecord["FIL_NAME"]);

            string key = string.Format("{0}|{1}", orgRecord["LGL_ID"], orgRecord["NOMER_ID"]);
            // для этого в специальном кэше сохраняем ключ записи key и NOMER_NAME
            // key нужен, чтобы удалить из orgRecords лишние записи
            // NOMER_NAME нужен, чтобы выбрать наименьший из них
            if (!auxOrgCache.ContainsKey(auxKey))
            {
                auxOrgCache.Add(auxKey, new string[] { key, orgRecord["NOMER_NAME"].Trim() });
            }
            else
            {
                // если встречается запись с таким же auxKey, сравниваем их NOMER_NAME
                string oldNomerName = auxOrgCache[auxKey][1];
                string newNomerName = orgRecord["NOMER_NAME"].Trim();

                // если новый номер больше, ничего не делаем
                if (!CompareNomerName(oldNomerName, newNomerName))
                    return;

                // если номер меньше, то удаляем строку с бОльшим номером из orgRecords
                // вместо нее будет добавлена строка с меньшим номером
                orgRecords.Remove(auxOrgCache[auxKey][0]);
                // кроме того запоминаем меньший NOMER_NAME в auxOrgCache
                auxOrgCache[auxKey][0] = key;
                auxOrgCache[auxKey][1] = newNomerName;
            }

            if (!orgRecords.ContainsKey(key))
            {
                orgRecords.Add(key, orgRecord);
            }
        }

        private void PumpFactOperative(Dictionary<string, string> record, int refGKHType, int refDetails,
            int refGKHOrg, int refServices, int refTerritory, int refDate)
        {
            object[] mapping = new object[] {
                "Tariff", CleanFactValue(record["L4_3_2"]),
                "TariffLegalAct", record["L4_1_2"],
                "Normative", CleanFactValue(record["L5_2_2"]),
                "NormativeLegalAct", record["L5_1_2"],
                "RefYear", refDate,
                "RefGKHType", refGKHType,
                "RefDetail", refDetails,
                "RefGKHOrg", refGKHOrg,
                "RefServicesGKH", refServices,
                "RefTerritory", refTerritory,
                "RefPumpDate", refPumpDate };

            PumpRow(dsMOTariff.Tables[0], mapping);
            if (dsMOTariff.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daMOTariff, ref dsMOTariff);
            }
        }

        private int GetOperativeDate()
        {
            // для оперативных тарифов дата берется из названия источника
            // название источника должно быть в формате ГГГГ.ММ
            // из него берем ГГГГ - год, ММ - месяц
            int refDate = Convert.ToInt32(this.DataSource.Variant.Replace(".", string.Empty));
            return refDate * 100;
        }

        private void PumpXmlOperativeAvg(Dictionary<string, string> avgRecord)
        {
            string scenarioName = avgRecord["SCENARIO_NAME"].ToUpper();
            if (scenarioName.Contains("PLAN"))
                return;

            string key = string.Format("{0}|{1}", avgRecord["LGL_ID"], avgRecord["NOMER_ID"]);
            if (!orgRecords.ContainsKey(key))
                return;

            Dictionary<string, string> orgRecord = orgRecords[key];
            if (!orgRecord.ContainsKey("L2"))
                orgRecord.Add("L2", avgRecord["L2"]);

            int refGKHType = PumpGKHType(orgRecord, true);
            int refGKHOrg = PumpGKHOrg(orgRecord);
            int refDetails = PumpDetails(avgRecord);
            int refServices = PumpGKHServices(orgRecord, true);
            int refTerritory = GetTerritoryRef(avgRecord["ENTITY_CODE"]);
            int refDate = GetOperativeDate();

            PumpFactOperative(avgRecord, refGKHType, refDetails, refGKHOrg, refServices, refTerritory, refDate);
        }

        #endregion

        private Dictionary<string, string> GetXmlRecord(XmlNodeList nodes)
        {
            Dictionary<string, string> record = new Dictionary<string,string>();
            foreach (XmlNode node in nodes)
            {
                record.Add(node.Name.Trim().ToUpper(), node.FirstChild.Value);
            }
            return record;
        }

        private void PumpXmlDoc(XmlDocument xmlDoc, string filename, PumpXmlRow pumpXmlRow)
        {
            WriteToTrace("Открытие документа: " + filename, TraceMessageKind.Information);
            XmlNodeList records = xmlDoc.GetElementsByTagName("record");
            int recordsCount = records.Count;
            for (int currentRecord = 0; currentRecord < recordsCount; currentRecord++)
                try
                {
                    SetProgress(recordsCount, currentRecord,
                        string.Format("Обработка файла {0}...", filename),
                        string.Format("Запись {0} из {1}", currentRecord + 1, recordsCount));

                    Dictionary<string, string> record = GetXmlRecord(records.Item(currentRecord).ChildNodes);

                    pumpXmlRow(record);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("При обработке записи {0} возникла ошибка ({1})",
                        currentRecord + 1, ex.Message), ex);
                }
        }

        private const string MO_TARIFF_REPORT = "NT_KU";
        private const string MO_LIST_REPORT = "LIST_MO";
        private const string MO_OPERATIVE_REPORT = "LIST_ORG";
        private const string FORM_P3_REPORT = "P3_R1";
        private const string FORM_22_GKH_REPORT = "JKH22_OPN";
        private const string FORM_N1_TIT_REPORT = "BUHG_1_TIT";
        private const string FORM_N1_PASSIV_REPORT = "BUHG_1_PASSIV";
        private const string FORM_N1_ACTIV_REPORT = "BUHG_1_ACTIV";
        private ReportType SetReportType(string fileName)
        {
            fileName = fileName.Trim().ToUpper();
            if (fileName.Contains(MO_LIST_REPORT))
                return ReportType.MOList;
            if (fileName.Contains(MO_TARIFF_REPORT))
                return ReportType.MOTariff;
            if (fileName.Contains(FORM_P3_REPORT))
                return ReportType.FormP3;
            if (fileName.Contains(FORM_22_GKH_REPORT))
                return ReportType.Form22GKH;
            if (fileName.Contains(FORM_N1_TIT_REPORT))
                return ReportType.FormN1Tit;
            if (fileName.Contains(FORM_N1_PASSIV_REPORT) || fileName.Contains(FORM_N1_ACTIV_REPORT))
                return ReportType.FormN1;
            if (fileName.Contains(MO_OPERATIVE_REPORT))
                return ReportType.MOOperative;
            return ReportType.Unknown;
        }

        // файлы ntku11_m_ku_avg(*).xml, ntku11_m_list_org(*).xml должны идти парами,
        // где (*) - номер в формате (НННН,ММ) НННН - регион и ММ - месяц,
        // этот номер у обоих файлов должен совпадать, если одно из этих файлов нет, выдаем исключение
        // если удалось найти такой файл, функция возвращает его
        private FileInfo CheckOperativeAvgFile(FileInfo orgFile)
        {
            string number = orgFile.Name.ToUpper().Split(new string[] { "LIST_ORG" }, StringSplitOptions.None)[1];
            FileInfo[] avgFiles = orgFile.Directory.GetFiles("*avg" + number, SearchOption.TopDirectoryOnly);
            if (avgFiles.Length == 0)
            {
                throw new PumpDataFailedException(string.Format(
                    "не найден файл ntku11_m_ku_avg{0} соответствующий файлу {1}",
                    number, orgFile.Name));
            }
            return avgFiles[0];
        }

        private void PumpXmlFile(FileInfo file)
        {
            auxOrgCache = new Dictionary<string, string[]>();
            orgRecords = new Dictionary<string, Dictionary<string, string>>();
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                reportType = SetReportType(file.Name);
                if (!deletedDateList.ContainsKey(reportType))
                    deletedDateList.Add(reportType, new List<int>());
                xmlDoc.Load(file.FullName);
                switch (reportType)
                {
                    case ReportType.FormN1:
                        PumpXmlDoc(xmlDoc, file.Name, new PumpXmlRow(PumpFormN1));
                        break;
                    case ReportType.FormN1Tit:
                        if (pumpingTitle)
                            PumpXmlDoc(xmlDoc, file.Name, new PumpXmlRow(PumpFormN1Tit));
                        break;
                    case ReportType.Form22GKH:
                        PumpXmlDoc(xmlDoc, file.Name, new PumpXmlRow(PumpForm22GKH));
                        break;
                    case ReportType.FormP3:
                        PumpXmlDoc(xmlDoc, file.Name, new PumpXmlRow(PumpFormP3));
                        break;
                    case ReportType.MOTariff:
                        PumpXmlDoc(xmlDoc, file.Name, new PumpXmlRow(PumpMOTariff));
                        break;
                    case ReportType.MOList:
                        if (pumpingMO)
                            PumpXmlDoc(xmlDoc, file.Name, new PumpXmlRow(PumpMOList));
                        break;
                    case ReportType.MOOperative:
                        // файлы ntku11_m_ku_avg(*).xml, ntku11_m_list_org(*).xml качаем одновременно,
                        // для этого сначала ищем пару соответствующих файлов,
                        // затем, если удалось такую пару найти, берем инфу из файла ntku11_m_list_org(*).xml
                        // и используем её при закачке второго файла ntku11_m_ku_avg(*).xml
                        FileInfo avgFile = CheckOperativeAvgFile(file);
                        PumpXmlDoc(xmlDoc, file.Name, new PumpXmlRow(PumpXmlOperativeOrg));
                        xmlDoc.Load(avgFile.FullName);
                        PumpXmlDoc(xmlDoc, avgFile.Name, new PumpXmlRow(PumpXmlOperativeAvg));
                        break;
                }
            }
            catch (PumpDataFailedException ex)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeFinishFilePumpWithError, string.Format(
                    "Ошибка при обработке файла {0}: {1}. Файл будет пропущен.", file.Name, ex.Message));
            }
            finally
            {
                auxOrgCache.Clear();
                orgRecords.Clear();
                if (xmlDoc != null)
                    XmlHelper.ClearDomDocument(ref xmlDoc);
            }
        }

        #endregion Работа с Xml

        #region Перекрытые методы закачки

        private void ShowAbsentCodes()
        {
            if (absentTerritoryCodes.Count > 0)
            {
                absentTerritoryCodes.Sort();
                string[] absentCodes = absentTerritoryCodes.ConvertAll<string>(Convert.ToString).ToArray();
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                    "В классификаторе '{0}' отсутствуют территории с кодами: {1}.",
                    clsTerritory.FullCaption, string.Join(", ", absentCodes)));
            }
        }

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            deletedDateList = new Dictionary<ReportType, List<int>>();
            titleRecords = new Dictionary<int, RecordInfo>();
            moList = new Dictionary<int, int>();
            absentTerritoryCodes = new List<int>();
            try
            {
                pumpingTitle = true;
                ProcessFilesTemplate(dir, "*tit*.xml", new ProcessFileDelegate(PumpXmlFile), false);
                pumpingMO = true;
                ProcessFilesTemplate(dir, "*list_mo.xml", new ProcessFileDelegate(PumpXmlFile), false);
                pumpingTitle = false;
                pumpingMO = false;
                ProcessFilesTemplate(dir, "*.xml", new ProcessFileDelegate(PumpXmlFile), false);
                ShowAbsentCodes();
                UpdateData();
            }
            finally
            {
                absentTerritoryCodes.Clear();
                moList.Clear();
                titleRecords.Clear();
                deletedDateList.Clear();
            }
        }

        protected override void DirectPumpData()
        {
            refPumpDate = Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd"));
            PumpDataVTemplate();
        }

        #endregion Перекрытые методы закачки

        #endregion Закачка данных

    }
}
