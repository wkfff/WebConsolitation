using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Xml;
using Krista.FM.Common.Xml;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.FK7Pump
{

    // ФК - 0007 - Закачка универсального формата (IBS)
    public class FK7PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // Показатели.КПЭ (d_Marks_KPI)
        private IDbDataAdapter daMarksKPI;
        private DataSet dsMarksKPI;
        private IClassifier clsMarksKPI;
        private Dictionary<string, int> cacheMarksKPI = null;
        private Dictionary<string, int> cacheMarksNames = null;
        private long maxMarksCode = 0;
        // Администратор.КПЭ (d_KVSR_KPI)
        private IDbDataAdapter daKvsr;
        private DataSet dsKvsr;
        private IClassifier clsKvsr;
        private Dictionary<string, int> cacheKvsr = null;
        private long maxKvsrCodeLine = 0;
        // Территории.КПЭ (d_Territory_KPI)
        private IDbDataAdapter daTerritory;
        private DataSet dsTerritory;
        private IClassifier clsTerritory;
        private Dictionary<string, int> cacheTerritory = null;
        private long maxTerritoryCodeLine = 0;

        #endregion Классификаторы

        #region Факты

        // Показатели.ФК_КПЭ (f_Marks_KPI)
        private IDbDataAdapter daMarksFact;
        private DataSet dsMarksFact;
        private IFactTable fctMarksFact;

        #endregion Факты

        private ReportType reportType;
        // даты отчета (в одном отчете может быть 1 или несколько дат)
        private List<int> refDates = null;
        // массив ссылок на классификатор "Показатели" (для отчетов uch_i_01_r1_rezf_fedb_*.xml)
        // массив заполняем из узлов DS_RASHOD_RХ_ID_KBK_R2.COMMENTS,
        // а ссылки проставляем из узлов DS_RASHOD_RХ_DETAIL_ID_D06.COMMENTS (см. метод PumpXmlMarksUch)
        private Dictionary<string, int> refMarksUch = null;
        // ссылка на классификатор Территории.КПЭ
        private int refTerritory = -1;
        // список дат, данные за которые уже были удалены
        private List<string> deletedDateList = null;

        #endregion Поля

        #region Константы

        private const char REPORT_DELIMITER = ';';

        private string[] SHORT_NAME_DELIMITER = new string[] { "--" };

        private const double MAX_FACT_VALUE = 1E15;

        private const double MULTIPLIER_BILLION = 1000000000;
        private const double MULTIPLIER_ONE = 1;

        #endregion Константы

        #region Структуры и перечисления

        /// <summary>
        /// Тип отчета
        /// </summary>
        private enum ReportType
        {
            /// <summary>
            /// Файлы rep_i_01_uch_*.xml
            /// </summary>
            RepUch,
            /// <summary>
            /// Файлы rep_i_01_uch_kb_rf_f_*.xml
            /// </summary>
            RepUchKb,
            /// <summary>
            /// Файлы rep_i_01_uch_ksubject_rf_bf_*.xml
            /// </summary>
            RepUchKSubject,
            /// <summary>
            /// Файлы rep_i_01_uch_plan_*.xml
            /// </summary>
            RepUchPlan,
            /// <summary>
            /// Файлы uch_ksubjects_al_*.xml
            /// </summary>
            UchKSubject,
            /// <summary>
            /// Файлы uch_i_01_r1_rezf_fedb_*.xml
            /// </summary>
            UchR1Rezf,
            /// <summary>
            /// Неизвестный формат
            /// </summary>
            Unknown
        }

        /// <summary>
        /// Поле в таблице фактов
        /// </summary>
        private struct FactValue
        {
            /// <summary>
            /// Название поля в отчете
            /// </summary>
            public string ReportField;
            /// <summary>
            /// Название поля в таблице
            /// </summary>
            public string FactField;
            /// <summary>
            /// Значение поля
            /// </summary>
            public double Value;
            /// <summary>
            /// Коэффициент домножения
            /// </summary>
            public double Multiplier;

            public FactValue(string reportField, string factField, double multiplier)
            {
                this.ReportField = reportField;
                this.FactField = factField;
                this.Multiplier = multiplier;
                this.Value = 0;
            }

            public FactValue(FactValue factValue, double value)
            {
                this.ReportField = factValue.ReportField;
                this.FactField = factValue.FactField;
                this.Multiplier = factValue.Multiplier;
                this.Value = value;
            }
        }

        #endregion Структуры и перечисления

        #region Делегаты

        private delegate void PumpXmlRow(XmlNode node);

        #endregion Делегаты

        #region Закачка данных

        #region Работа с базой и кэшами

        private void FillCaches()
        {
            FillRowsCache(ref cacheMarksKPI, dsMarksKPI.Tables[0], new string[] { "Code", "Name" }, "|", "ID");
            FillRowsCache(ref cacheMarksNames, dsMarksKPI.Tables[0], "Name");
            FillRowsCache(ref cacheKvsr, dsKvsr.Tables[0], "Name");
            FillRowsCache(ref cacheTerritory, dsTerritory.Tables[0], "Name");
        }

        private long GetClsMaxCode(IClassifier cls, string fieldName)
        {
            string query = string.Format(" select max({1}) from {0} ", cls.FullDBName, fieldName);
            object maxCode = this.DB.ExecQuery(query, QueryResultTypes.Scalar, new IDbDataParameter[] { });
            if ((maxCode == null) || (maxCode == DBNull.Value))
                return 0;
            return Convert.ToInt64(maxCode);
        }

        private void SetMaxCodes()
        {
            maxKvsrCodeLine = GetClsMaxCode(clsKvsr, "CodeLine");
            maxTerritoryCodeLine = GetClsMaxCode(clsTerritory, "CodeLine");
            maxMarksCode = GetClsMaxCode(clsMarksKPI, "Code");
        }

        protected override void QueryData()
        {
            InitDataSet(ref daMarksKPI, ref dsMarksKPI, clsMarksKPI, string.Empty);
            InitDataSet(ref daKvsr, ref dsKvsr, clsKvsr, string.Empty);
            InitDataSet(ref daTerritory, ref dsTerritory, clsTerritory, string.Empty);

            InitFactDataSet(ref daMarksFact, ref dsMarksFact, fctMarksFact);

            FillCaches();
            SetMaxCodes();
        }

        #region GUIDs

        private const string D_MARKS_KPI_GUID = "cb02f438-10c5-47d3-ba6c-461e1c5173ed";
        private const string D_KVSR_KPI_GUID = "6aa69879-e100-4103-9db8-09b5663bcfca";
        private const string D_TERRITORY_KPI_GUID = "5f987d1e-1de2-4e71-9db9-fc77bed010f5";
        private const string F_MARKS_KPI_GUID = "132944e7-4adb-45b0-91ec-d2f8bb1e1370";

        #endregion GUIDs
        protected override void InitDBObjects()
        {
            clsMarksKPI = this.Scheme.Classifiers[D_MARKS_KPI_GUID];
            clsKvsr = this.Scheme.Classifiers[D_KVSR_KPI_GUID];
            clsTerritory = this.Scheme.Classifiers[D_TERRITORY_KPI_GUID];

            fctMarksFact = this.Scheme.FactTables[F_MARKS_KPI_GUID];

            this.UsedClassifiers = new IClassifier[] { };
            this.UsedFacts = new IFactTable[] { fctMarksFact };
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daMarksKPI, dsMarksKPI, clsMarksKPI);
            UpdateDataSet(daKvsr, dsKvsr, clsKvsr);
            UpdateDataSet(daTerritory, dsTerritory, clsTerritory);

            UpdateDataSet(daMarksFact, dsMarksFact, fctMarksFact);
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsMarksFact);

            ClearDataSet(ref dsMarksKPI);
            ClearDataSet(ref dsKvsr);
            ClearDataSet(ref dsTerritory);
        }

        #endregion Работа с базой и кэшами

        #region Работа с Xml

        private string GetXmlTagValue(XmlNode node, string xpath)
        {
            XmlNode tag = node.SelectSingleNode(xpath);
            if (tag == null)
                return string.Empty;
            return tag.InnerText.Trim();
        }

        private double GetFactValue(string value)
        {
            return Convert.ToDouble(value.Trim().Replace('.', ',').PadLeft(1, '0'));
        }

        // в полях с фактами может быть несколько сумм, разделенных точкой с запятой ';'
        // параметр index указывает, какую именно из этих сумм брать
        private List<FactValue> GetFactValues(XmlNode node, int index, params FactValue[] factFields)
        {
            List<FactValue> factValues = new List<FactValue>();

            foreach (FactValue factField in factFields)
            {
                string[] values = GetXmlTagValue(node, factField.ReportField).Split(REPORT_DELIMITER);
                if (index >= values.Length)
                    break;

                string value = values[index].Trim();
                if (value.ToUpper() == "#MISSING")
                    continue;
                double factValue = GetFactValue(value) * factField.Multiplier;
                if (Math.Abs(factValue) > MAX_FACT_VALUE)
                {
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                        "Значение поля '{0}' = {1} больше, чем заданная точность. Поле не будет заполнено (ID записи = {2})",
                        factField.ReportField, value, GetXmlTagValue(node, "ID")));
                }
                else
                {
                    factValues.Add(new FactValue(factField, factValue));
                }
            }

            return factValues;
        }

        private List<FactValue> GetFactValues(XmlNode node, params FactValue[] factFields)
        {
            return GetFactValues(node, 0, factFields);
        }

        private bool CheckNullSums(List<FactValue> factValues)
        {
            if (factValues.Count == 0)
                return true;
            foreach (FactValue fact in factValues)
                if (fact.Value != 0)
                    return false;
            return true;
        }

        private void PumpFactRow(List<FactValue> factValues, int refDate, int refMarks, int refBudLevel, int refKvsr, int refTerritory)
        {
            // если все суммы нулевые, то запись не закачиваем
            if (CheckNullSums(factValues))
                return;

            object[] mapping = new object[] { "RefYearDayUNV", refDate, "RefKPI", refMarks,
                "RefBudLevels", refBudLevel, "RefKVSRKPI", refKvsr, "RefTerrKPI", refTerritory };
            foreach (FactValue fact in factValues)
            {
                mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { fact.FactField, fact.Value });
            }

            PumpRow(dsMarksFact.Tables[0], mapping);
        }

        private string GetMarksName(XmlNode node)
        {
            string marksName = string.Empty;

            foreach (XmlNode field in node.ChildNodes)
            {
                string fieldName = field.Name.Trim().ToUpper();
                if (fieldName.StartsWith("KBK") && (field.InnerText.Trim() != string.Empty))
                {
                    marksName = field.InnerText.Trim();
                    if (fieldName.EndsWith("COMMENTS"))
                    {
                        marksName = field.InnerText.Trim();
                        break;
                    }
                }
            }

            return marksName;
        }

        private int PumpMarks(XmlNode node)
        {
            double code = 0;
            string name = string.Empty;
            string parentNodeName = node.ParentNode.ParentNode.Name.Trim().ToUpper();

            if (parentNodeName.StartsWith("DS_DOHOD_GADB"))
            {
                code = 1000000000;
                name = "доходы";
            }
            else if (parentNodeName.StartsWith("DS_RASHOD_GRBS"))
            {
                code = 2000000000;
                name = "расходы";
            }
            else
            {
                code = Convert.ToDouble(GetXmlTagValue(node, "ID").Replace('.', ','));
                name = GetMarksName(node);
            }

            if (cacheMarksNames.ContainsKey(name))
            {
                // уникальность проверяем по Коду и Наименованию
                string key = string.Format("{0}|{1}", code, name);
                if (cacheMarksKPI.ContainsKey(key))
                    return cacheMarksKPI[key];
                // если не нашли такой записи, то, возможно, ещё есть запись с составным наименование (Code_Name)
                name = string.Format("{0}_{1}", code, name);
                if (cacheMarksNames.ContainsKey(name))
                    return cacheMarksNames[name];
            }

            string complexKey = string.Format("{0}|{1}", code, name);
            int refMarks = PumpCachedRow(cacheMarksKPI, dsMarksKPI.Tables[0], clsMarksKPI, complexKey,
                new object[] { "Code", code, "Name", name, "ShortName", name, "RefUnits", -1 });
            cacheMarksNames.Add(name, refMarks);
            return refMarks;
        }

        private int PumpMarksUch(XmlNode node)
        {
            double code = Convert.ToDouble(GetXmlTagValue(node, "ID").Replace('.', ','));
            string name = GetMarksName(node);
            string shortName = name.Split(SHORT_NAME_DELIMITER, StringSplitOptions.None)[0].Trim();
            string codeLineStr = GetXmlTagValue(node, "KBK_R2");
            codeLineStr = codeLineStr.Split(new string[] { "KBK_R2_" }, StringSplitOptions.None)[1].Trim();
            int codeLine = Convert.ToInt32(codeLineStr.PadLeft(1, '0'));

            if (cacheMarksNames.ContainsKey(name))
            {
                // уникальность проверяем по Коду и Наименованию
                string key = string.Format("{0}|{1}", code, name);
                if (cacheMarksKPI.ContainsKey(key))
                    return cacheMarksKPI[key];
                // если не нашли такой записи, то, возможно, ещё есть запись с составным наименование (Code_Name)
                name = string.Format("{0}_{1}", code, name);
                if (cacheMarksNames.ContainsKey(name))
                    return cacheMarksNames[name];
            }

            string complexKey = string.Format("{0}|{1}", code, name);
            int refMarks = PumpCachedRow(cacheMarksKPI, dsMarksKPI.Tables[0], clsMarksKPI, complexKey, new object[] {
                "Code", code, "Name", name, "ShortName", shortName, "CodeLine", codeLine, "RefUnits", -1 });
            cacheMarksNames.Add(name, refMarks);
            return refMarks;
        }

        private int PumpMarksSubjects(XmlNode node)
        {
            // т.к. кода в отчетах нет, то уникальность проверяем только по наименованию
            string name = GetMarksName(node);
            if (cacheMarksNames.ContainsKey(name))
                return cacheMarksNames[name];

            maxMarksCode++;
            return PumpCachedRow(cacheMarksNames, dsMarksKPI.Tables[0], clsMarksKPI, name, new object[] {
                "Code", maxMarksCode, "Name", name, "ShortName", name, "RefUnits", -1 });
        }

        private int PumpKvsr(XmlNode node)
        {
            int code = 0;
            string name = GetXmlTagValue(node, "D06.COMMENTS");
            if (name == string.Empty)
                name = GetXmlTagValue(node, "D06");
            if (name == string.Empty)
                return -1;

            string[] splittedName = name.Split(SHORT_NAME_DELIMITER, StringSplitOptions.None);
            if (splittedName.Length > 1)
                code = Convert.ToInt32(splittedName[1].Replace("D06_", string.Empty).Trim().PadLeft(1, '0'));

            if (cacheKvsr.ContainsKey(name))
                return cacheKvsr[name];

            maxKvsrCodeLine++;
            return PumpCachedRow(cacheKvsr, dsKvsr.Tables[0], clsKvsr, name, new object[] {
                "Code", code, "Name", name, "ShortName", splittedName[0].Trim(), "CodeLine", maxKvsrCodeLine });
        }

        private int PumpTerritory(XmlNode node)
        {
            string name = GetXmlTagValue(node, "COMMENTS");
            if (cacheTerritory.ContainsKey(name))
                return cacheTerritory[name];

            int code = Convert.ToInt32(GetXmlTagValue(node, "ID").PadLeft(1, '0'));
            string shortName = name.Split(SHORT_NAME_DELIMITER, StringSplitOptions.None)[0].Trim();
            maxTerritoryCodeLine++;

            return PumpCachedRow(cacheTerritory, dsTerritory.Tables[0], clsTerritory, name, new object[] {
                "Name", name, "Code", code, "ShortName", shortName, "CodeLine", maxTerritoryCodeLine });
        }

        private int GetPeriod(XmlNode node)
        {
            string period = GetXmlTagValue(node, "PERIODS");
            if (period == string.Empty)
                return -1;
            return Convert.ToInt32(period);
        }

        // закачка из файлов rep_i_01_uch_*.xml
        private void PumpXmlRowRepUch(XmlNode node)
        {
            int refMarks = PumpMarks(node);
            int refKvsr = PumpKvsr(node);

            int curDate = GetPeriod(node);
            if (curDate != -1)
            {
                DeleteEarlierDataByDate(curDate, 18);

                List<FactValue> factValues = GetFactValues(node,
                    new FactValue("RSUM_PDATE", "Execution", MULTIPLIER_BILLION),
                    new FactValue("RSUM_PERCENT", "PartExecution", MULTIPLIER_ONE),
                    new FactValue("RSUM_PERCENT_PREV_PER", "PartPrevPer", MULTIPLIER_ONE),
                    new FactValue("RSUM_PDATE_PREV_PER", "IncreasePrevPer", MULTIPLIER_ONE),
                    new FactValue("VVP_PERCENT", "PartVVP", MULTIPLIER_ONE),
                    new FactValue("VVP_PERCENT_PREV_PER", "PartVVPPrev", MULTIPLIER_ONE),
                    new FactValue("TURNOVER_PDATE", "InMonth", MULTIPLIER_BILLION),
                    new FactValue("TURNOVER_PDATE_PRIROST", "PartInMonth", MULTIPLIER_ONE),
                    new FactValue("TURNOVER_ON_DATE", "Turnover", MULTIPLIER_BILLION));

                PumpFactRow(factValues, curDate, refMarks, 18, refKvsr, -1);
            }
            else
            {
                // если нет такого поля, то закачиваем несколько сумм для разных дат
                for (int index = 0; index < refDates.Count; index++)
                {
                    DeleteEarlierDataByDate(refDates[index], 18);

                    List<FactValue> factValues = GetFactValues(node, index,
                        new FactValue("RSUM_PDATE", "Execution", MULTIPLIER_BILLION),
                        new FactValue("RSUM_PERCENT", "PartExecution", MULTIPLIER_ONE),
                        new FactValue("RSUM_PERCENT_PREV_PER", "PartPrevPer", MULTIPLIER_ONE),
                        new FactValue("RSUM_PDATE_PREV_PER", "IncreasePrevPer", MULTIPLIER_ONE),
                        new FactValue("VVP_PERCENT", "PartVVP", MULTIPLIER_ONE),
                        new FactValue("VVP_PERCENT_PREV_PER", "PartVVPPrev", MULTIPLIER_ONE),
                        new FactValue("TURNOVER_PDATE", "InMonth", MULTIPLIER_BILLION),
                        new FactValue("TURNOVER_PDATE_PRIROST", "PartInMonth", MULTIPLIER_ONE),
                        new FactValue("TURNOVER_ON_DATE", "Turnover", MULTIPLIER_BILLION));

                    PumpFactRow(factValues, refDates[index], refMarks, 18, refKvsr, -1);
                }
            }
        }

        // закачка из файлов rep_i_01_uch_kb_rf_f_*.xml
        private void PumpXmlRowRepUchKb(XmlNode node)
        {
            int refMarks = PumpMarks(node);

            int curDate = GetPeriod(node);
            if (curDate != -1)
            {
                // если поле Periods есть, то закачивается одна запись
                List<FactValue> factValues = GetFactValues(node,
                    new FactValue("RSUM_PDATE", "Execution", MULTIPLIER_BILLION),
                    new FactValue("RSUM_PERCENT", "PartExecution", MULTIPLIER_ONE),
                    new FactValue("RSUM_PERCENT_PREV_PER", "PartPrevPer", MULTIPLIER_ONE),
                    new FactValue("RSUM_PDATE_PREV_PER", "IncreasePrevPer", MULTIPLIER_ONE),
                    new FactValue("VVP_PERCENT", "PartVVP", MULTIPLIER_ONE),
                    new FactValue("VVP_PERCENT_PREV_PER", "PartVVPPrev", MULTIPLIER_ONE),
                    new FactValue("TURNOVER_PDATE", "InMonth", MULTIPLIER_BILLION),
                    new FactValue("TURNOVER_PDATE_PRIROST", "PartInMonth", MULTIPLIER_ONE));

                PumpFactRow(factValues, curDate, refMarks, 21, -1, -1);
            }
            else
            {
                // если нет такого поля, то закачиваем несколько сумм для разных дат
                for (int index = 0; index < refDates.Count; index++)
                {
                    List<FactValue> factValues = GetFactValues(node, index,
                        new FactValue("RSUM_PDATE", "Execution", MULTIPLIER_BILLION),
                        new FactValue("RSUM_PERCENT", "PartExecution", MULTIPLIER_ONE),
                        new FactValue("RSUM_PERCENT_PREV_PER", "PartPrevPer", MULTIPLIER_ONE),
                        new FactValue("RSUM_PDATE_PREV_PER", "IncreasePrevPer", MULTIPLIER_ONE),
                        new FactValue("VVP_PERCENT", "PartVVP", MULTIPLIER_ONE),
                        new FactValue("VVP_PERCENT_PREV_PER", "PartVVPPrev", MULTIPLIER_ONE),
                        new FactValue("TURNOVER_PDATE", "InMonth", MULTIPLIER_BILLION),
                        new FactValue("TURNOVER_PDATE_PRIROST", "PartInMonth", MULTIPLIER_ONE));

                    PumpFactRow(factValues, refDates[index], refMarks, 21, -1, -1);
                }
            }
        }

        // закачка из файлов rep_i_01_uch_ksubject_rf_bf_*.xml
        private void PumpXmlRowRepUchKSubject(XmlNode node)
        {
            int refMarks = PumpMarks(node);

            for (int index = 0; index < refDates.Count; index++)
            {
                DeleteEarlierDataByDate(refDates[index], 22);

                List<FactValue> factValues = GetFactValues(node, index,
                    new FactValue("RSUM_PDATE", "Execution", MULTIPLIER_BILLION),
                    new FactValue("RSUM_PERCENT", "PartExecution", MULTIPLIER_ONE),
                    new FactValue("RSUM_PERCENT_PREV_PER", "PartPrevPer", MULTIPLIER_ONE),
                    new FactValue("RSUM_PDATE_PREV_PER", "IncreasePrevPer", MULTIPLIER_ONE),
                    new FactValue("VVP_PERCENT", "PartVVP", MULTIPLIER_ONE),
                    new FactValue("VVP_PERCENT_PREV_PER", "PartVVPPrev", MULTIPLIER_ONE),
                    new FactValue("TURNOVER_PDATE", "InMonth", MULTIPLIER_BILLION),
                    new FactValue("TURNOVER_PDATE_PRIROST", "PartInMonth", MULTIPLIER_ONE));

                PumpFactRow(factValues, refDates[index], refMarks, 22, -1, -1);
            }
        }

        // закачка из файлов rep_i_01_uch_plan_*.xml
        private void PumpXmlRowRepUchPlan(XmlNode node)
        {
            int refMarks = PumpMarks(node);
            int refKvsr = PumpKvsr(node);

            for (int index = 0; index < refDates.Count; index++)
            {
                List<FactValue> factValues = GetFactValues(node, index,
                    new FactValue("RSUM_PDATE", "RefinPlan", MULTIPLIER_BILLION),
                    new FactValue("RSUM_ON_DATE", "OrigPlan", MULTIPLIER_BILLION));

                PumpFactRow(factValues, refDates[index], refMarks, 18, refKvsr, -1);
            }
        }

        // закачка из файлов uch_i_01_r1_rezf_fedb_*.xml
        private void PumpXmlRowUchR1Rezf(XmlNode node)
        {
            int refMarks = -1;
            #region Формирование показателей
            string parentNodeName = node.ParentNode.ParentNode.Name.Trim().ToUpper();
            // для узлов DS_RASHOD_RХ_DETAIL_ID_D06.COMMENTS классификатор не формируется,
            // т.к. эти данные подчиняются (детализируют) данные из узлов DS_RASHOD_RХ_ID_KBK_R2.COMMENTS
            // поэтому ссылка на показатели у них будет соответствовать
            if (parentNodeName.Contains("DETAIL"))
            {
                string key = parentNodeName.Split(new string[] { "_DETAIL" }, StringSplitOptions.None)[0].Trim();
                refMarks = refMarksUch[key];
            }
            else
            {
                string key = parentNodeName.Split(new string[] { "_ID" }, StringSplitOptions.None)[0].Trim();
                refMarks = PumpMarksUch(node);
                refMarksUch.Add(key, refMarks);
            }
            #endregion
            int refKvsr = PumpKvsr(node);

            int curDate = GetPeriod(node);
            if (curDate != -1)
            {
                DeleteEarlierDataByDate(curDate, 18);

                List<FactValue> factValues = GetFactValues(node,
                    new FactValue("UT_ROSP", "RefinPlan", MULTIPLIER_BILLION),
                    new FactValue("KASS_ISP", "Execution", MULTIPLIER_BILLION),
                    new FactValue("PROC_KASS_ISP", "PartExecution", MULTIPLIER_ONE));

                PumpFactRow(factValues, curDate, refMarks, 18, refKvsr, -1);
            }
            else
            {
                for (int index = 0; index < refDates.Count; index++)
                {
                    DeleteEarlierDataByDate(refDates[index], 18);

                    List<FactValue> factValues = GetFactValues(node, index,
                        new FactValue("UT_ROSP", "RefinPlan", MULTIPLIER_BILLION),
                        new FactValue("KASS_ISP", "Execution", MULTIPLIER_BILLION),
                        new FactValue("PROC_KASS_ISP", "PartExecution", MULTIPLIER_ONE));

                    PumpFactRow(factValues, refDates[index], refMarks, 18, refKvsr, -1);
                }
            }
        }

        // закачка из файлов uch_ksubjects_al_*.xml
        private void PumpXmlRowUchKSubjects(XmlNode node)
        {
            int refMarks = PumpMarksSubjects(node);

            int curDate = GetPeriod(node);
            if (curDate != -1)
            {
                DeleteEarlierDataByDate(curDate, 2);

                List<FactValue> factValues = GetFactValues(node,
                    new FactValue("RSUM_PDATE", "Execution", MULTIPLIER_BILLION),
                    new FactValue("RSUM_PERCENT", "PartExecution", MULTIPLIER_ONE),
                    new FactValue("RSUM_PERCENT_PREV_PER", "PartPrevPer", MULTIPLIER_ONE),
                    new FactValue("TURNOVER_PDATE", "InMonth", MULTIPLIER_BILLION));

                PumpFactRow(factValues, curDate, refMarks, 2, -1, refTerritory);
            }
            else
            {
                for (int index = 0; index < refDates.Count; index++)
                {
                    DeleteEarlierDataByDate(refDates[index], 2);

                    List<FactValue> factValues = GetFactValues(node, index,
                        new FactValue("RSUM_PDATE", "Execution", MULTIPLIER_BILLION),
                        new FactValue("RSUM_PERCENT", "PartExecution", MULTIPLIER_ONE),
                        new FactValue("RSUM_PERCENT_PREV_PER", "PartPrevPer", MULTIPLIER_ONE),
                        new FactValue("TURNOVER_PDATE", "InMonth", MULTIPLIER_BILLION));

                    PumpFactRow(factValues, refDates[index], refMarks, 2, -1, refTerritory);
                }
            }
        }

        private void PumpXmlRows(XmlNodeList xmlRows, string filename, PumpXmlRow pumpXmlRow)
        {
            string dataSourcePath = GetShortSourcePathBySourceID(this.SourceID);
            int rowsCount = xmlRows.Count;
            for (int curRow = 0; curRow < rowsCount; curRow++)
            {
                SetProgress(rowsCount, curRow,
                    string.Format("Обработка файла {0}\\{1}...", dataSourcePath, filename),
                    string.Format("Строка {0} из {1}", curRow, rowsCount));

                pumpXmlRow(xmlRows[curRow]);
            }
        }

        private void DeleteEarlierDataByDate(int refDate, int refBudLevel)
        {
            string key = string.Format("{0}|{1}", refDate, refBudLevel);
            if (!this.DeleteEarlierData && !deletedDateList.Contains(key))
            {
                DirectDeleteFactData(new IFactTable[] { fctMarksFact }, -1, this.SourceID, string.Format(
                    " RefYearDayUNV = {0} AND RefBudLevels = {1} AND PumpId <> {2} ",
                    refDate, refBudLevel, this.PumpID));
                deletedDateList.Add(key);
            }
        }

        private void DeleteEarlierDataForRepUchKb(List<int> refDates)
        {
            DirectDeleteFactData(new IFactTable[] { fctMarksFact }, -1, this.SourceID, string.Format(
                " RefYearDayUNV IN ({0}) AND RefBudLevels = 21 ",
                string.Join(",", refDates.ConvertAll<string>(Convert.ToString).ToArray())));
        }

        // в отчете может быть несколько дат, разбиваются они ';'
        // в этом случае суммы фактов разбиваются так же
        private List<int> GetRefDates(string dateStr)
        {
            List<int> refDates = new List<int>();
            string[] strDates = dateStr.Split(REPORT_DELIMITER);
            foreach (string strDate in strDates)
            {
                refDates.Add(Convert.ToInt32(strDate.Trim()));
            }
            return refDates;
        }

        private const string XPATH_DATE = "DATA/DS_DATE/DS_DATE_ROW/P_DATE";
        private const string XPATH_DATE_SUBJECTS = "DATA/DS_DATE/DS_DATE_ROW/TO_CHAR_MAX_D.D_INCOM__YYYYMMDD_";
        private const string XPATH_DATAROWS = "DATA/*//ROW";
        private const string XPATH_DATAROWS_SUBJECTS = "DATA/*//DS_STAT_DOHOD_ROW|" +
            "DATA/*//DS_RASHOD_KBK_R1_ROW|DATA/*//DS_RASHOD_KBK_R4_ROW|DATA/*//DS_DEF_PROF_ROW|" +
            "DATA/*//DS_IST_FIN_ID_KBK_D_ROW|DATA/*//DS_PRIVL_POGAH_ID_KBK_F_ROW";
        private const string XPATH_TERRITORY = "DATA/*//DS_SUBJECT_ROW";
        private void PumpXmlFile(FileInfo file)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(file.FullName);
            try
            {
                switch (reportType)
                {
                    case ReportType.RepUch:
                        refDates = GetRefDates(GetXmlTagValue(xmlDoc, XPATH_DATE));
                        PumpXmlRows(xmlDoc.SelectNodes(XPATH_DATAROWS), file.Name, new PumpXmlRow(PumpXmlRowRepUch));
                        break;
                    case ReportType.RepUchKb:
                        refDates = GetRefDates(GetXmlTagValue(xmlDoc, XPATH_DATE));
                        DeleteEarlierDataForRepUchKb(refDates);
                        PumpXmlRows(xmlDoc.SelectNodes(XPATH_DATAROWS), file.Name, new PumpXmlRow(PumpXmlRowRepUchKb));
                        break;
                    case ReportType.RepUchKSubject:
                        refDates = GetRefDates(GetXmlTagValue(xmlDoc, XPATH_DATE));
                        PumpXmlRows(xmlDoc.SelectNodes(XPATH_DATAROWS), file.Name, new PumpXmlRow(PumpXmlRowRepUchKSubject));
                        break;
                    case ReportType.RepUchPlan:
                        refDates = GetRefDates(GetXmlTagValue(xmlDoc, XPATH_DATE));
                        PumpXmlRows(xmlDoc.SelectNodes(XPATH_DATAROWS), file.Name, new PumpXmlRow(PumpXmlRowRepUchPlan));
                        break;
                    case ReportType.UchR1Rezf:
                        refDates = GetRefDates(GetXmlTagValue(xmlDoc, XPATH_DATE));
                        refMarksUch = new Dictionary<string, int>();
                        PumpXmlRows(xmlDoc.SelectNodes(XPATH_DATAROWS), file.Name, new PumpXmlRow(PumpXmlRowUchR1Rezf));
                        break;
                    case ReportType.UchKSubject:
                        refDates = GetRefDates(GetXmlTagValue(xmlDoc, XPATH_DATE_SUBJECTS));
                        refTerritory = PumpTerritory(xmlDoc.SelectSingleNode(XPATH_TERRITORY));
                        PumpXmlRows(xmlDoc.SelectNodes(XPATH_DATAROWS_SUBJECTS), file.Name, new PumpXmlRow(PumpXmlRowUchKSubjects));
                        break;
                }

                UpdateData();
            }
            finally
            {
                if (refMarksUch != null)
                    refMarksUch.Clear();
                XmlHelper.ClearDomDocument(ref xmlDoc);
            }
        }

        #endregion Работа с Xml

        #region Перекрытые методы закачки

        private void PumpZipFile(FileInfo file)
        {
            DirectoryInfo tempDir = CommonRoutines.ExtractArchiveFileToTempDir(
                file.FullName, FilesExtractingOption.SingleDirectory, ArchivatorName.Zip);
            try
            {
                ProcessFilesTemplate(tempDir, "*.xml", new ProcessFileDelegate(PumpXmlFile), false);
            }
            finally
            {
                CommonRoutines.DeleteDirectory(tempDir);
            }
        }

        private void ProcessAllFiles(DirectoryInfo dir)
        {
            ProcessFilesTemplate(dir, "*.zip", new ProcessFileDelegate(PumpZipFile), false);
            ProcessFilesTemplate(dir, "*.xml", new ProcessFileDelegate(PumpXmlFile), false);
        }

        private ReportType GetReportType(string dirname)
        {
            dirname = dirname.Trim().ToUpper();
            if (dirname == "UCH")
                return ReportType.RepUch;
            if (dirname == "UCH_KB_RF_F")
                return ReportType.RepUchKb;
            if (dirname == "UCH_KSUBJECT_RF_BF")
                return ReportType.RepUchKSubject;
            if (dirname == "UCH_KSUBJECTS_AL")
                return ReportType.UchKSubject;
            if (dirname == "UCH_PLAN")
                return ReportType.RepUchPlan;
            if (dirname == "UCH_REZF_FEDB")
                return ReportType.UchR1Rezf;
            return ReportType.Unknown;
        }

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            deletedDateList = new List<string>();
            try
            {
                DirectoryInfo[] subdirs = dir.GetDirectories("*", SearchOption.TopDirectoryOnly);
                foreach (DirectoryInfo subdir in subdirs)
                {
                    reportType = GetReportType(subdir.Name);
                    if (reportType == ReportType.Unknown)
                        continue;

                    WriteToTrace(string.Format("Старт закачки данных из каталога '{0}'.", subdir.Name), TraceMessageKind.Information);
                    ProcessAllFiles(subdir);
                }
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

        #endregion Перекрытые методы закачки

        #endregion Закачка данных

        #region Обработка данных

        private void GroupFactTable()
        {
            GroupTable(fctMarksFact,
                new string[] { "RefYearDayUNV", "RefKPI", "RefKVSRKPI", "RefTerrKPI" },
                new string[] {
                    "RefinPlan", "Execution", "PartExecution", "PartPrevPer", "IncreasePrevPer",
                    "PartVVP", "PartVVPPrev", "InMonth", "PartInMonth", "Turnover", "OrigPlan" },
                "RefBudLevels = 18");
        }

        protected override void ProcessDataSource()
        {
            GroupFactTable();
            UpdateData();
        }

        protected override void DirectProcessData()
        {
            ProcessDataSourcesTemplate("Группировка записей в таблице фактов.");
        }

        #endregion Обработка данных

    }

}
