using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.EO5Pump
{

    // ЭО - 0005 - Муниципальные программы
    public class EO5PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // ОК.КОСГУ (d_OK_KOSGU)
        private IDbDataAdapter daKosgu;
        private DataSet dsKosgu;
        private IClassifier clsKosgu;
        private Dictionary<int, int> cacheKosgu = null;
        private int nullKosgu = -1;
        // Территории.РФ (d_Territory_RF)
        private IDbDataAdapter daTerritory;
        private DataSet dsTerritory;
        private IClassifier clsTerritory;
        private Dictionary<string, int> cacheTerritory = null;
        private int nullTerritory = -1;
        // Программы.Типы программ (d_Programs_TypeProg)
        private IDbDataAdapter daTypeProg;
        private DataSet dsTypeProg;
        private IClassifier clsTypeProg;
        private Dictionary<string, int> cacheTypeProg = null;
        private int nullTypeProg = -1;
        // Программы.Заказчики (d_Programs_CustomerProgramm)
        private IDbDataAdapter daCustomer;
        private DataSet dsCustomer;
        private IClassifier clsCustomer;
        private Dictionary<string, int> cacheCustomer = null;
        // Программы.Исполнители (d_Programs_Executive)
        private IDbDataAdapter daExecutive;
        private DataSet dsExecutive;
        private IClassifier clsExecutive;
        private Dictionary<string, int> cacheExecutive = null;
        // Программы.Реестр МЦП (d_Programs_ReestrMOP)
        private IDbDataAdapter daReestrMOP;
        private DataSet dsReestrMOP;
        private IClassifier clsReestrMOP;
        private Dictionary<string, int> cacheReestrMOP = null;
        // Программы.Реестр ОЦП (d_Programs_SEGP)
        private IDbDataAdapter daReestrSEGP;
        private DataSet dsReestrSEGP;
        private IClassifier clsReestrSEGP;
        private Dictionary<string, int> cacheReestrSEGP = null;
        // Программы.Реестр программ (d_Programs_ReestrLP)
        private IDbDataAdapter daReestrLP;
        private DataSet dsReestrLP;
        private IClassifier clsReestrLP;
        // Программы.Источники финансирования (d_Programs_Source)
        private IDbDataAdapter daSource;
        private DataSet dsSource;
        private IClassifier clsSource;
        private Dictionary<int, int> cacheSource = null;

        #endregion Классификаторы

        #region Факты

        // Программы.ЭО Программы МЦП (f_Programs_ProgrammMOP)
        private IDbDataAdapter daProgrammMOP;
        private DataSet dsProgrammMOP;
        private IFactTable fctProgrammMOP;
        // Программы.ЭО Программы МОЦП (f_Programs_ProgrammMOCP)
        private IDbDataAdapter daProgrammMOCP;
        private DataSet dsProgrammMOCP;
        private IFactTable fctProgrammMOCP;
        // Программы.ЭО Программы ОЦП (f_Programs_Prog4)
        private IDbDataAdapter daProg4;
        private DataSet dsProg4;
        private IFactTable fctProg4;
        // Программы.ЭО_Программы_Долгосрочные целевые программы (f_Programs_LProgram)
        private IDbDataAdapter daLProgram;
        private DataSet dsLProgram;
        private IFactTable fctLProgram;

        #endregion Факты

        private ReportType reportType;

        private int[] reestrHierarchy = new int[8];

        private int codeReestrIndex = 0;
        private string nextType = string.Empty;
        // ссылка на классификатор "Программы.Заказчики" - в отчётах МЦП и МОЦП
        private int refCustomer = -1;
        // ссылка на классификатор "Программы.Исполнители" - в отчётах ОЦП
        private int refExecutive = -1;
        private int codeExecutiveIndex = 0;
        // предыдущий вид расходов (используется при формировании реестра программ, закачка ДЦП)
        private string prevOutcome = string.Empty;
        private int codesRepeatCount = 0;
        private int nextHierLevel = -1;

        // параметры источника
        // на самом деле у источника один параметр: вариант,
        // но его наименование формируется из четырёх папок, вложенных друг в друга: год-вариант-месяц-территория
        // они нужны в процессе закачки, поэтому храним в глобальных переменных
        private int year = 0;
        private string variant = string.Empty;
        private int month = 0;
        private string territory = string.Empty;

        #endregion Поля

        #region Структуры, перечисления

        /// <summary>
        /// Тип отчёта
        /// </summary>
        private enum ReportType
        {
            /// <summary>
            /// МЦП (муниципальные целевые программы)
            /// </summary>
            MCP,
            /// <summary>
            /// МОЦП (региональные целевые программы)
            /// </summary>
            MOCP,
            /// <summary>
            /// ОЦП (областные целевые программы)
            /// </summary>
            OCP,
            /// <summary>
            /// ДЦП (долгосрочные целевые программы)
            /// </summary>
            DCP
        }

        #endregion Структуры, перечисления

        #region Закачка данных

        #region Работа с базой и кэшами

        private void FillCaches()
        {
            FillRowsCache(ref cacheTerritory, dsTerritory.Tables[0], "NAME", "ID");
            if (reportType == ReportType.MCP)
            {
                FillRowsCache(ref cacheKosgu, dsKosgu.Tables[0], "CODE", "ID");
                FillRowsCache(ref cacheCustomer, dsCustomer.Tables[0], "NAME", "ID");
                FillRowsCache(ref cacheReestrMOP, dsReestrMOP.Tables[0], new string[] { "PARENTID", "NAME" }, "|", "ID");
            }
            else if (reportType == ReportType.MOCP)
            {
                FillRowsCache(ref cacheKosgu, dsKosgu.Tables[0], "CODE", "ID");
                FillRowsCache(ref cacheCustomer, dsCustomer.Tables[0], "NAME", "ID");
                FillRowsCache(ref cacheTypeProg, dsTypeProg.Tables[0], "NAME", "ID");
                FillRowsCache(ref cacheReestrSEGP, dsReestrSEGP.Tables[0], new string[] { "PARENTID", "NAME" }, "|", "ID");
            }
            else if (reportType == ReportType.OCP)
            {
                FillRowsCache(ref cacheTypeProg, dsTypeProg.Tables[0], "NAME", "ID");
                FillRowsCache(ref cacheExecutive, dsExecutive.Tables[0], "NAME", "ID");
                FillRowsCache(ref cacheReestrSEGP, dsReestrSEGP.Tables[0], new string[] { "PARENTID", "NAME" }, "|", "ID");
            }
            else if (reportType == ReportType.DCP)
            {
                FillRowsCache(ref cacheSource, dsSource.Tables[0], "Code");
            }
        }

        protected override void QueryData()
        {
            InitDataSet(ref daTerritory, ref dsTerritory, clsTerritory, true, string.Empty, string.Empty);
            if (reportType == ReportType.MCP)
            {
                InitDataSet(ref daKosgu, ref dsKosgu, clsKosgu, false, string.Empty, string.Empty);
                InitDataSet(ref daCustomer, ref dsCustomer, clsCustomer, false, string.Empty, string.Empty);
                InitClsDataSet(ref daReestrMOP, ref dsReestrMOP, clsReestrMOP);
                InitFactDataSet(ref daProgrammMOP, ref dsProgrammMOP, fctProgrammMOP);
            }
            else if (reportType == ReportType.MOCP)
            {
                InitDataSet(ref daKosgu, ref dsKosgu, clsKosgu, false, string.Empty, string.Empty);
                InitDataSet(ref daCustomer, ref dsCustomer, clsCustomer, false, string.Empty, string.Empty);
                InitDataSet(ref daTypeProg, ref dsTypeProg, clsTypeProg, false, string.Empty, string.Empty);
                InitClsDataSet(ref daReestrSEGP, ref dsReestrSEGP, clsReestrSEGP);
                InitFactDataSet(ref daProgrammMOCP, ref dsProgrammMOCP, fctProgrammMOCP);
            }
            else if (reportType == ReportType.OCP)
            {
                InitDataSet(ref daTypeProg, ref dsTypeProg, clsTypeProg, false, string.Empty, string.Empty);
                InitDataSet(ref daExecutive, ref dsExecutive, clsExecutive, false, string.Empty, string.Empty);
                InitClsDataSet(ref daReestrSEGP, ref dsReestrSEGP, clsReestrSEGP);
                InitFactDataSet(ref daProg4, ref dsProg4, fctProg4);
            }
            else if (reportType == ReportType.DCP)
            {
                InitDataSet(ref daSource, ref dsSource, clsSource, false, string.Empty, string.Empty);
                InitClsDataSet(ref daReestrLP, ref dsReestrLP, clsReestrLP);
                InitFactDataSet(ref daLProgram, ref dsLProgram, fctLProgram);
            }
            FillCaches();
        }

        protected override void UpdateData()
        {
            if (reportType == ReportType.MCP)
            {
                UpdateDataSet(daCustomer, dsCustomer, clsCustomer);
                UpdateDataSet(daReestrMOP, dsReestrMOP, clsReestrMOP);
                UpdateDataSet(daProgrammMOP, dsProgrammMOP, fctProgrammMOP);
            }
            else if (reportType == ReportType.MOCP)
            {
                UpdateDataSet(daCustomer, dsCustomer, clsCustomer);
                UpdateDataSet(daReestrSEGP, dsReestrSEGP, clsReestrSEGP);
                UpdateDataSet(daProgrammMOCP, dsProgrammMOCP, fctProgrammMOCP);
            }
            else if (reportType == ReportType.OCP)
            {
                UpdateDataSet(daExecutive, dsExecutive, clsExecutive);
                UpdateDataSet(daReestrSEGP, dsReestrSEGP, clsReestrSEGP);
                UpdateDataSet(daProg4, dsProg4, fctProg4);
            }
            else if (reportType == ReportType.DCP)
            {
                UpdateDataSet(daReestrLP, dsReestrLP, clsReestrLP);
                UpdateDataSet(daLProgram, dsLProgram, fctLProgram);
            }
        }

        #region GUIDs

        private const string D_KOSGU_GUID = "0e4efeb9-9716-4508-bb67-29ef6483b487";
        private const string D_TERRITORY_GUID = "66b9a66d-85ca-41de-910e-f9e6cb483960";
        private const string D_TYPE_PROG_GUID = "3921a644-c2e5-449e-81e4-9c38ea2c7432";
        private const string D_CUSTOMER_GUID = "cd64efad-535d-4100-ae82-28602c640407";
        private const string D_EXECUTIVE_GUID = "684bab0a-ddfb-4eef-a5ab-6ed533713520";
        private const string D_REESTR_MOP_GUID = "b128adc8-3dbf-461e-a119-ef129ff16ccc";
        private const string D_REESTR_SEGP_GUID = "4e9e0d49-3f01-453f-97b2-18bdfe362fc4";
        private const string D_REESTR_LP_GUID = "25985de5-cedd-4da3-9080-63825f430fa8";
        private const string D_SOURCE_GUID = "0bed5456-5119-487b-8f4c-10178d048547";
        private const string F_PROGRAMM_MOP_GUID = "bc7f4cfc-51d6-46ad-aecb-1b538c226021";
        private const string F_PROGRAMM_MOCP_GUID = "8eaa0ab8-b3ae-4032-a894-2d0eeb6b8974";
        private const string F_PROG4_GUID = "9691acd9-5136-4725-8790-a85af8e98cc3";
        private const string F_LPROGRAM_GUID = "f666277e-fd5e-4a13-a1e0-33094ccebdd9";

        #endregion GUIDs
        // Объекты схемы инициализируются в зависимости от типа отчёта
        // Это необходимо, так как на схемах разного уровня (муниципального или )
        private void InitDBObjectsEx()
        {
            clsTerritory = this.Scheme.Classifiers[D_TERRITORY_GUID];
            if (reportType == ReportType.MCP)
            {
                clsKosgu = this.Scheme.Classifiers[D_KOSGU_GUID];
                clsCustomer = this.Scheme.Classifiers[D_CUSTOMER_GUID];
                this.UsedClassifiers = new IClassifier[] {
                    clsReestrMOP = this.Scheme.Classifiers[D_REESTR_MOP_GUID] };
                this.UsedFacts = new IFactTable[] {
                    fctProgrammMOP = this.Scheme.FactTables[F_PROGRAMM_MOP_GUID] };
            }
            else if (reportType == ReportType.MOCP)
            {
                clsKosgu = this.Scheme.Classifiers[D_KOSGU_GUID];
                clsCustomer = this.Scheme.Classifiers[D_CUSTOMER_GUID];
                clsTypeProg = this.Scheme.Classifiers[D_TYPE_PROG_GUID];
                this.UsedClassifiers = new IClassifier[] {
                    clsReestrSEGP = this.Scheme.Classifiers[D_REESTR_SEGP_GUID] };
                this.UsedFacts = new IFactTable[] {
                    fctProgrammMOCP = this.Scheme.FactTables[F_PROGRAMM_MOCP_GUID] };
            }
            else if (reportType == ReportType.OCP)
            {
                clsExecutive = this.Scheme.Classifiers[D_EXECUTIVE_GUID];
                clsTypeProg = this.Scheme.Classifiers[D_TYPE_PROG_GUID];
                this.UsedClassifiers = new IClassifier[] {
                    clsReestrSEGP = this.Scheme.Classifiers[D_REESTR_SEGP_GUID] };
                this.UsedFacts = new IFactTable[] {
                    fctProg4 = this.Scheme.FactTables[F_PROG4_GUID] };
            }
            else if (reportType == ReportType.DCP)
            {
                clsSource = this.Scheme.Classifiers[D_SOURCE_GUID];
                this.UsedClassifiers = new IClassifier[] {
                    clsReestrLP = this.Scheme.Classifiers[D_REESTR_LP_GUID] };
                this.UsedFacts = new IFactTable[] {
                    fctLProgram = this.Scheme.FactTables[F_LPROGRAM_GUID] };
            }
            this.AssociateClassifiers = this.UsedClassifiers;
            this.CubeClassifiers = this.UsedClassifiers;
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsTerritory);
                
            if (reportType == ReportType.MCP)
            {
                ClearDataSet(ref dsKosgu);
                ClearDataSet(ref dsCustomer);
                ClearDataSet(ref dsReestrMOP);
                ClearDataSet(ref dsProgrammMOP);
            }
            else if (reportType == ReportType.MOCP)
            {
                ClearDataSet(ref dsKosgu);
                ClearDataSet(ref dsTypeProg);
                ClearDataSet(ref dsCustomer);
                ClearDataSet(ref dsReestrSEGP);
                ClearDataSet(ref dsProgrammMOCP);
            }
            else if (reportType == ReportType.OCP)
            {
                ClearDataSet(ref dsTypeProg);
                ClearDataSet(ref dsExecutive);
                ClearDataSet(ref dsReestrSEGP);
                ClearDataSet(ref dsProg4);
            }
            else if (reportType == ReportType.DCP)
            {
                ClearDataSet(ref dsSource);
                ClearDataSet(ref dsReestrLP);
                ClearDataSet(ref dsLProgram);
            }
        }

        #endregion Работа с базой и кэшами

        #region Общие функции закачки

        private decimal CleanFactValue(string factValue)
        {
            factValue = CommonRoutines.TrimLetters(factValue.Trim().Replace(".", ",")).Trim();
            return Convert.ToDecimal(factValue.PadLeft(1, '0'));
        }

        // устанавливаем тип отчёта
        private const string REPORT_TYPE_MCP = "МЦП";
        private const string REPORT_TYPE_MOCP = "МОЦП";
        private const string REPORT_TYPE_OCP = "ОЦП";
        private const string REPORT_TYPE_DCP = "ДЦП";
        private void SetReportType()
        {
            if (variant == REPORT_TYPE_MCP)
                reportType = ReportType.MCP;
            else if (variant == REPORT_TYPE_MOCP)
                reportType = ReportType.MOCP;
            else if (variant == REPORT_TYPE_OCP)
                reportType = ReportType.OCP;
            else if (variant == REPORT_TYPE_DCP)
                reportType = ReportType.DCP;
            else
                throw new Exception("Не удалось определить тип отчёта.");
        }

        // дата определяется параметрами источника
        private int GetReportDate()
        {
            return (year * 10000 + month * 100);
        }

        // территория определяется параметрами источника
        private int GetTerritory()
        {
            int refTerritory = FindCachedRow(cacheTerritory, territory, nullTerritory);
            if (refTerritory == nullTerritory)
                throw new Exception(string.Format("Территория '{0}' не найдена. Данные закачаны не будут.", territory));
            return refTerritory;
        }

        // обнуляем иерарахию реестра
        private void SetNullReestrHierarchy(int startLevel)
        {
            for (int i = startLevel; i < reestrHierarchy.GetLength(0); i++)
                reestrHierarchy[i] = -1;
        }

        // выборка предыдущего уровня иерархии реестра
        private int GetParentLevelId(int curLevel)
        {
            while (curLevel > 0)
            {
                curLevel--;
                if (reestrHierarchy[curLevel] != -1)
                    return reestrHierarchy[curLevel];
            }
            return -1;
        }

        private const string TYPE_PROGRAM      = "ПРОГРАММА";
        private const string TYPE_SUBPROGRAM   = "ПОДПРОГРАММА";
        private const string TYPE_DIRECTION    = "НАПРАВЛЕНИЕ";
        private const string TYPE_SECTION      = "РАЗДЕЛ";
        private const string TYPE_CAMPAIGN     = "МЕРОПРИЯТИЕ";
        private const string TYPE_DETAILING    = "ДЕТАЛИЗАЦИЯ";
        private const string TYPE_SUBDETAILING = "ПОДДЕТАЛИЗАЦИЯ";
        private int GetReestrLevel(string typeName)
        {
            typeName = typeName.Trim().ToUpper();
            if (typeName == TYPE_PROGRAM)
                return 0;
            else if (typeName == TYPE_SUBPROGRAM)
                return 1;
            else if (typeName == TYPE_DIRECTION)
                return 2;
            else if (typeName == TYPE_SECTION)
                return 3;
            else if (typeName == TYPE_CAMPAIGN)
                return 4;
            else if (typeName == TYPE_DETAILING)
                return 5;
            else if (typeName == TYPE_SUBDETAILING)
                return 6;
            return 7;
        }

        #endregion Общие функции закачки

        #region Работа с Excel

        #region Закачка классификаторов

        // получить указатель на классификатор "ОК.КОСГУ"
        private int GetRefKosgu(string codeStr)
        {
            int code = Convert.ToInt32(codeStr.Trim().Substring(0, 3));
            return FindCachedRow(cacheKosgu, code, nullKosgu);
        }

        // получить ссылку на классификатор "Программы.Источники финансирования"
        private int GetRefSource(int code)
        {
            return FindCachedRow(cacheSource, code, -1);
        }

        // "Программы.Заказчик" - только в отчётах МЦП и МОЦП
        private int PumpXlsCustomer(ExcelHelper excelDoc)
        {
            string name = excelDoc.GetValue(1, 3).Trim();
            if (name == string.Empty)
                return -1;
            object[] mapping = new object[] { "Name", name };
            return PumpCachedRow(cacheCustomer, dsCustomer.Tables[0], clsCustomer, mapping, name, "ID");
        }

        // "Программы.Исполнители" - только в отчётах ОЦП
        private int PumpXlsExecutive(ExcelHelper excelDoc)
        {
            string name = excelDoc.GetValue(3, 4).Trim();
            if (name == string.Empty)
                return -1;
            codeExecutiveIndex++;
            object[] mapping = new object[] { "Name", name, "Code", codeExecutiveIndex };
            return PumpCachedRow(cacheExecutive, dsExecutive.Tables[0], clsExecutive, mapping, name, "ID");
        }

        // получить указатель на классификатор "Программы.Типы программ"
        private int GetRefTypeProg(string type)
        {
            return FindCachedRow(cacheTypeProg, type, nullTypeProg);
        }

        // "Программы.Реестр МЦП" - в отчётах МЦП;
        // "Программы.Реестр ОЦП" - в отчётах МОЦП и ОЦП
        private int PumpXlsReestr(Dictionary<string, string> dataRow, int curLevel)
        {
            codeReestrIndex++;
            int code = codeReestrIndex;
            string name = dataRow["Name"].Trim();
            int parentId = GetParentLevelId(curLevel);
            object[] mapping;
            string reestrKey = string.Empty;
            if (parentId > 0)
            {
                mapping = new object[] { "Code", code, "Name", name, "ParentId", parentId };
                reestrKey = string.Format("{0}|{1}", parentId, name);
            }
            else
            {
                mapping = new object[] { "Code", code, "Name", name };
                reestrKey = "|" + name;
            }

            if (reportType == ReportType.MCP)
            {
                string reason = dataRow["Reason"].Trim();
                mapping = (object[])CommonRoutines.ConcatArrays(mapping,
                    new object[] { "ReasonNonExecution", reason, "Make", dataRow["Type"].Trim() });
                return PumpCachedRow(cacheReestrMOP, dsReestrMOP.Tables[0], clsReestrMOP, mapping, reestrKey, "ID");
            }
            else
            {
                string reason = string.Empty;
                if (reportType == ReportType.MOCP)
                    reason = dataRow["Reason"].Trim();
                int refTypeProg = GetRefTypeProg(dataRow["Type"].Trim());
                mapping = (object[])CommonRoutines.ConcatArrays(mapping,
                    new object[] { "ReasonNonExecution", reason, "RefTypeProg", refTypeProg });
                return PumpCachedRow(cacheReestrSEGP, dsReestrSEGP.Tables[0], clsReestrSEGP, mapping, reestrKey, "ID");
            }
        }

        #endregion Закачка классификаторов

        #region Закачка фактов

        private void PumpFactMcpRow(Dictionary<string, string> dataRow, int refDate, int refTerritory, int curLevel)
        {
            object[] mapping = new object[] {
                "Vote", CleanFactValue(dataRow["Vote"]),
                "CashPlan", CleanFactValue(dataRow["CashPlan"]),
                "ExecuteYear", CleanFactValue(dataRow["ExecuteYear"]),
                "ExecuteMonth", CleanFactValue(dataRow["ExecuteMonth"]),
                "NoDigest", CleanFactValue(dataRow["NoDigest"]),
                "RefReestrMOP", GetParentLevelId(curLevel),
                "RefKOSGU", GetRefKosgu(dataRow["Kosgu"]),
                "RefTerritory", refTerritory,
                "RefDate", refDate,
                "RefCustomer", refCustomer };

            PumpRow(dsProgrammMOP.Tables[0], mapping);
            if (dsProgrammMOP.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daProgrammMOP, ref dsProgrammMOP);
            }
        }

        private void PumpFactMocpRow(Dictionary<string, string> dataRow, int refDate, int refTerritory, int curLevel)
        {
            object[] mapping = new object[] {
                "Vote", CleanFactValue(dataRow["Vote"]),
                "FirstQuarter", CleanFactValue(dataRow["FirstQuarter"]),
                "SecondQuarter", CleanFactValue(dataRow["SecondQuarter"]),
                "ThirdQuarter", CleanFactValue(dataRow["ThirdQuarter"]),
                "FourthQuarter", CleanFactValue(dataRow["FourthQuarter"]),
                "Limit", CleanFactValue(dataRow["Limit"]),
                "Income", CleanFactValue(dataRow["Income"]),
                "ExecuteYear", CleanFactValue(dataRow["ExecuteYear"]),
                "ExecuteMonth", CleanFactValue(dataRow["ExecuteMonth"]),
                "NoDigest", CleanFactValue(dataRow["NoDigest"]),
                "RefCustomerProgramm", refCustomer,
                "RefProgramsSEGP", GetParentLevelId(curLevel),
                "RefTerritoryRF", refTerritory,
                "RefDateYearDay", refDate,
                "RefKOSGU", GetRefKosgu(dataRow["Kosgu"]) };

            PumpRow(dsProgrammMOCP.Tables[0], mapping);
            if (dsProgrammMOCP.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daProgrammMOCP, ref dsProgrammMOCP);
            }
        }

        private void PumpFactOcpRow(object[] mapping, string fieldName, string value, int refSource, int refDirection)
        {
            if (value.Trim() == string.Empty)
                return;
            mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] {
                fieldName, CleanFactValue(value),
                "RefProgramsSource", refSource,
                "RefProgramsDirection", refDirection });

            PumpRow(dsProg4.Tables[0], mapping);
        }

        private void PumpFactOcp(Dictionary<string, string> dataRow, int refDate, int refTerritory, int curLevel)
        {
            int refReestr = PumpXlsReestr(dataRow, curLevel);
            object[] mapping = new object[] {
                "RefYearDayUNV", refDate,
                "RefProgramsSEGP", refReestr,
                "RefProgramsExecutive", refExecutive,
                "RefTerritory", refTerritory };

            PumpFactOcpRow(mapping, "Planing", dataRow["Plan1"], 1, -1);
            PumpFactOcpRow(mapping, "Planing", dataRow["Plan2"], 2, -1);
            PumpFactOcpRow(mapping, "Planing", dataRow["Plan3"], 3, -1);
            PumpFactOcpRow(mapping, "Planing", dataRow["Plan4"], 4, -1);
            PumpFactOcpRow(mapping, "PlanBudget", dataRow["PlanBudget1"], 1, 1);
            PumpFactOcpRow(mapping, "PlanBudget", dataRow["PlanBudget2"], 1, 2);
            PumpFactOcpRow(mapping, "Fact", dataRow["Fact1"], 1, 1);
            PumpFactOcpRow(mapping, "Fact", dataRow["Fact2"], 1, 2);
            PumpFactOcpRow(mapping, "Fact", dataRow["Fact3"], 2, -1);
            PumpFactOcpRow(mapping, "Fact", dataRow["Fact4"], 3, -1);
            PumpFactOcpRow(mapping, "Fact", dataRow["Fact5"], 4, -1);

            if (dsProg4.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daProg4, ref dsProg4);
            }
        }

        #endregion Закачка фактов

        #region Закачка ДЦП

        private int GetDcpHierLevel(Dictionary<string, string> dataRow, string prevOutcome)
        {
            int hierLevel = 0;
            for (int i = 2; i <= 5; i++)
            {
                if (dataRow[string.Format("Code{0}", i)].Trim() == string.Empty)
                    break;
                hierLevel++;
            }
            if ((dataRow["Code5"].Trim() != string.Empty) && (prevOutcome == dataRow["Code5"].Trim()))
            {
                hierLevel = 5;
            }
            return hierLevel;
        }

        private int PumpXlsReestrLP(Dictionary<string, string> dataRow, out int curHierLevel)
        {
            curHierLevel = GetDcpHierLevel(dataRow, prevOutcome);
            if (curHierLevel == 5)
                codesRepeatCount++;
            else
                codesRepeatCount = 0;
            string code = string.Format("{0}{1}{2}{3}{4}{5:D2}",
                dataRow["Code1"].Trim().Replace(" ", string.Empty).PadRight(7, '0'), // 1-ый уровень иерархии "Программа"
                dataRow["Code2"].Trim().PadLeft(3, '0'), // 2-ой уровень иерархии "Исполнитель"
                dataRow["Code3"].Trim().PadLeft(2, '0'), // 3-ий уровень иерархии "Раздел"
                dataRow["Code4"].Trim().PadLeft(2, '0'), // 4-ый уровень иерархии "Подраздел"
                dataRow["Code5"].Trim().PadLeft(3, '0'), // 5-ый уровень иерархии "Вид расхода"
                codesRepeatCount); // 6-ой уровень иерархии "Детализация"

            code = code.PadRight(19, '0');
            object[] mapping = null;
            if ((curHierLevel > 0) && (reestrHierarchy[curHierLevel - 1] != -1))
            {
                mapping = new object[] { "Code", code, "Name", dataRow["Name"], "ParentID", reestrHierarchy[curHierLevel - 1] };
            }
            else
            {
                mapping = new object[] { "Code", code, "Name", dataRow["Name"], "ParentID", DBNull.Value };
            }

            prevOutcome = dataRow["Code5"].Trim();
            reestrHierarchy[curHierLevel] = PumpRow(dsReestrLP.Tables[0], clsReestrLP, mapping);
            return reestrHierarchy[curHierLevel];
        }

        private void PumpFactDcpRow(string plan, string execute, string digest, int sourceCode, object[] mapping)
        {
            if ((plan.Trim() == string.Empty) && (execute.Trim() == string.Empty) && (digest.Trim() == string.Empty))
                return;

            mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] {
                "PlanF", CleanFactValue(plan),
                "ExecuteY", CleanFactValue(execute),
                "DigestY", CleanFactValue(digest),
                "RefProgpamSource", GetRefSource(sourceCode)});

            PumpRow(dsLProgram.Tables[0], mapping);
        }

        private void PumpFactDcp(Dictionary<string, string> dataRow, int refDate, int refTerritory)
        {
            int curHierLevel = -1;
            int refReestr = PumpXlsReestrLP(dataRow, out curHierLevel);
            // закачиваем факты, только если текущий уровень иерархии последний
            if (curHierLevel < nextHierLevel)
                return;

            object[] mapping = new object[] {
                "RefReestrLP", refReestr,
                "RefTerritory", refTerritory,
                "RefYearDayUNV", refDate };

            // Областной бюджет
            PumpFactDcpRow(dataRow["Plan1"], dataRow["Execute1"], dataRow["Digest1"], 1, mapping);
            // Муниципальный бюджет
            PumpFactDcpRow(dataRow["Plan2"], dataRow["Execute2"], dataRow["Digest2"], 3, mapping);

            if (dsLProgram.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daLProgram, ref dsLProgram);
            }
        }

        // в отчётах ДЦП необходимо знать следующий уровень иерархии - он должен быть последним,
        // т.к. факты качаются на самый нижний уровень
        private void SetNextHierLevel(Dictionary<string, string> currentRow, Dictionary<string, string> nextRow)
        {
            nextHierLevel = GetDcpHierLevel(nextRow, currentRow["Code5"]);
        }

        #endregion Закачка ДЦП

        private bool IsPumpedRow(int curLevel)
        {
            // в отчётах ОЦП качаются данные со всех уровней иерархии
            if (reportType == ReportType.OCP)
                return true;
            // в остальных отчётах закачиваем строки, невходящие в иерархию
            return (curLevel > 6);
        }

        private void PumpXlsRow(Dictionary<string, string> dataRow, int refDate, int refTerritory)
        {
            if (reportType == ReportType.DCP)
            {
                PumpFactDcp(dataRow, refDate, refTerritory);
                return;
            }
            int curLevel = GetReestrLevel(dataRow["Type"]);
            if (IsPumpedRow(curLevel))
            {
                if (reportType == ReportType.MCP)
                    PumpFactMcpRow(dataRow, refDate, refTerritory, curLevel);
                else if (reportType == ReportType.MOCP)
                    PumpFactMocpRow(dataRow, refDate, refTerritory, curLevel);
                else
                {
                    PumpFactOcp(dataRow, refDate, refTerritory, curLevel);
                    int nextLevel = GetReestrLevel(nextType);
                    if ((curLevel < 3) || (curLevel < nextLevel))
                    {
                        SetNullReestrHierarchy(curLevel);
                        reestrHierarchy[curLevel] = PumpXlsReestr(dataRow, curLevel);
                    }
                }
            }
            else
            {
                SetNullReestrHierarchy(curLevel);
                reestrHierarchy[curLevel] = PumpXlsReestr(dataRow, curLevel);
            }
        }

        #region Массивы пар "Поле-Столбец"

        private object[] XLS_MAPPING_MCP = new object[] {
            "Type", 1, "Name", 3, "Kosgu", 4, "Vote", 5, "CashPlan", 6,
            "ExecuteYear", 7, "ExecuteMonth", 8, "NoDigest", 9, "Reason", 10 };
        private object[] XLS_MAPPING_MOCP = new object[] {
            "Type", 1, "Name", 3, "Kosgu", 4, "Vote", 5, "FirstQuarter", 6,
            "SecondQuarter", 7, "ThirdQuarter", 8, "FourthQuarter", 9, "Limit", 10,
            "Income", 11, "ExecuteYear", 12, "ExecuteMonth", 13, "NoDigest", 14, "Reason", 15 };
        private object[] XLS_MAPPING_OCP = new object[] {
            "Type", 2, "Name", 3, "Plan1", 5, "Plan2", 6, "Plan3", 7, "Plan4", 8,
            "PlanBudget1", 10, "PlanBudget2", 11, "Fact1", 13, "Fact2", 14, "Fact3", 15,
            "Fact4", 16, "Fact5", 17 };
        private object[] XLS_MAPPING_DCP = new object[] {
            "Name", 2, "Code1", 3, "Code2", 4, "Code3", 5, "Code4", 6, "Code5", 7,
            "Plan1", 9, "Plan2", 10, "Execute1", 12, "Execute2", 13, "Digest1", 15, "Digest2", 16 };
        private object[] GetXlsMapping()
        {
            if (reportType == ReportType.MCP)
                return XLS_MAPPING_MCP;
            if (reportType == ReportType.MOCP)
                return XLS_MAPPING_MOCP;
            if (reportType == ReportType.DCP)
                return XLS_MAPPING_DCP;
            return XLS_MAPPING_OCP;
        }

        private Dictionary<string, string> GetXlsDataRow(ExcelHelper excelDoc, int curRow, object[] mapping)
        {
            Dictionary<string, string> dataRow = new Dictionary<string, string>();
            int count = mapping.GetLength(0);
            for (int i = 0; i < count; i += 2)
            {
                dataRow.Add(mapping[i].ToString(), excelDoc.GetValue(curRow, Convert.ToInt32(mapping[i + 1])));
            }
            return dataRow;
        }

        #endregion Массивы пар "Поле-Столбец"

        private bool IsSkipRow(ExcelHelper excelDoc, int curRow)
        {
            if (reportType == ReportType.DCP)
                return (excelDoc.GetValue(curRow, 2).Trim() == string.Empty);
            return (excelDoc.GetValue(curRow, 1).Trim() == string.Empty);
        }

        private bool IsStartSection(ExcelHelper excelDoc, int curRow)
        {
            if (reportType == ReportType.OCP)
                return (curRow == 13);
            else if (reportType == ReportType.DCP)
                return (excelDoc.GetValue(curRow, 1).Trim() == "1");
            return (excelDoc.GetValue(curRow, 1).Trim().ToUpper() == TYPE_PROGRAM);
        }

        // в отчётах ОЦП необходимо знать название следующего уровня иерархии реестра,
        // чтобы можно было определить строку с данными для закачки (см. IsPumpedRow)
        private void SetNextType(ExcelHelper excelDoc, int curRow)
        {
            nextType = excelDoc.GetValue(curRow + 1, 2).Trim();
        }

        private void PumpXlsSheetData(FileInfo file, ExcelHelper excelDoc, int refDate, int refTerritory)
        {
            codeReestrIndex = 0;
            SetNullReestrHierarchy(0);
            if (reportType == ReportType.OCP)
                refExecutive = PumpXlsExecutive(excelDoc);
            else
                refCustomer = PumpXlsCustomer(excelDoc);
            bool toPump = false;
            object[] xlsMapping = GetXlsMapping();
            int rowsCount = excelDoc.GetRowsCount();
            for (int curRow = 1; curRow <= rowsCount; curRow++)
                try
                {
                    SetProgress(rowsCount, curRow,
                        string.Format("Обработка файла {0}...", file.FullName),
                        string.Format("Строка {0} из {1}", curRow, rowsCount));

                    if (IsSkipRow(excelDoc, curRow))
                        continue;

                    if (IsStartSection(excelDoc, curRow))
                        toPump = true;

                    if (toPump)
                    {
                        if (reportType == ReportType.OCP)
                            SetNextType(excelDoc, curRow);
                        Dictionary<string, string> dataRow = GetXlsDataRow(excelDoc, curRow, xlsMapping);
                        if (reportType == ReportType.DCP)
                            SetNextHierLevel(dataRow, GetXlsDataRow(excelDoc, curRow + 1, xlsMapping));
                        PumpXlsRow(dataRow, refDate, refTerritory);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format(
                        "При обработке строки {0} листа '{1}' возникла ошибка ({2})",
                        curRow, excelDoc.GetWorksheetName(), ex.Message), ex);
                }
        }

        private bool AllowPumpSheet(ExcelHelper excelDoc)
        {
            if (reportType != ReportType.OCP)
                return true;
            return (excelDoc.GetValue(13, 2).Trim().ToUpper() == "ПРОГРАММА");
        }

        private void PumpXlsFile(FileInfo file)
        {
            WriteToTrace("Открытие документа: " + file.Name, TraceMessageKind.Information);
            ExcelHelper excelDoc = new ExcelHelper();
            try
            {
                excelDoc.AskToUpdateLinks = false;
                excelDoc.DisplayAlerts = false;
                excelDoc.OpenDocument(file.FullName);
                int refTerritory = GetTerritory();
                int refDate = GetReportDate();
                int wsCount = 1;
                if (reportType == ReportType.OCP)
                    wsCount = excelDoc.GetWorksheetsCount();
                for (int index = 1; index <= wsCount; index++)
                {
                    excelDoc.SetWorksheet(index);
                    if (AllowPumpSheet(excelDoc))
                        PumpXlsSheetData(file, excelDoc, refDate, refTerritory);
                }
            }
            finally
            {
                if (excelDoc != null)
                    excelDoc.CloseDocument();
            }
        }

        #endregion Работа с Excel

        #region Перекрытые методы закачки

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            ProcessFilesTemplate(dir, "*.xls", new ProcessFileDelegate(PumpXlsFile), false);
            UpdateData();
        }
        
        /// <summary>
        /// Рекурсивный обход каталогов источника
        /// </summary>
        /// <param name="rootDir">Текущий просматриваемы каталог</param>
        /// <param name="currentLevel">Текущий уровень каталогов</param>
        /// <param name="levelsCount">Количество уровней</param>
        private void PumpDataByParam(DirectoryInfo rootDir, int currentLevel, int levelsCount)
        {
            DirectoryInfo[] dir = rootDir.GetDirectories("*", SearchOption.TopDirectoryOnly);
            int dirsCount = dir.GetLength(0);
            if (dirsCount == 0)
            {
                throw new PumpDataFailedException(
                    string.Format("В каталоге {0} не найдено ни одного источника.", rootDir.FullName));
            }
            // обходим все каталоги
            for (int i = 0; i < dirsCount; i++)
            {
                string name = CheckDataSourceCommonDir(dir[i]).Trim();
                if (name == string.Empty)
                    continue;

                // на втором уровне каталогов определяем тип отчёта по названию каталога
                if (currentLevel == 1)
                    year = Convert.ToInt32(name);
                else if (currentLevel == 2)
                    variant = name;
                else if (currentLevel == 3)
                    month = Convert.ToInt32(name);
                else if (currentLevel == 4)
                    territory = name;

                // если добрались до последнего параметра источника, начинаем закачку данных
                if (currentLevel >= levelsCount)
                {
                    SetReportType();
                    string sourceName = string.Format("{0}, {1}, месяц: {2}, территория: {3}", year, variant, month, territory);
                    SetDataSource(ParamKindTypes.Variant, string.Empty, 0, 0, sourceName, 0, string.Empty);
                    InitDBObjectsEx();
                    InitAuxiliaryClss();
                    PumpDataSource(dir[i]);
                }
                // иначе рекурсивно переходим к следующему параметру
                else
                {
                    PumpDataByParam(dir[i], currentLevel + 1, levelsCount);
                }
            }
        }

        protected override void DirectPumpData()
        {
            PumpDataByParam(this.RootDir, 1, 4);
        }

        #endregion Перекрытые методы закачки

        #endregion Закачка данных

        #region Расчет кубов

        private const string MCP_CUBE_GUID = "bc7f4cfc-51d6-46ad-aecb-1b538c226021";
        private const string MCP_CUBE_NAME = "ЭО_Программы_МЦП";
        private const string MOCP_CUBE_GUID = "8eaa0ab8-b3ae-4032-a894-2d0eeb6b8974";
        private const string MOCP_CUBE_NAME = "ЭО_Программы_МОЦП";
        private const string OCP_CUBE_GUID = "9691acd9-5136-4725-8790-a85af8e98cc3";
        private const string OCP_CUBE_NAME = "ЭО_Программы_ОЦП";
        private const string DCP_CUBE_GUID = "f666277e-fd5e-4a13-a1e0-33094ccebdd9";
        private const string DCP_CUBE_NAME = "ЭО_Программы_Долгосрочные целевые программы";
        protected override void DirectProcessCube()
        {
            base.DirectProcessCube();
            this.CubeClassifiers = new IClassifier[] { };
            if (reportType == ReportType.MCP)
                cubesForProcess = new string[] { MCP_CUBE_GUID, MCP_CUBE_NAME };
            else if (reportType == ReportType.MOCP)
                cubesForProcess = new string[] { MOCP_CUBE_GUID, MOCP_CUBE_NAME };
            else if (reportType == ReportType.OCP)
                cubesForProcess = new string[] { OCP_CUBE_GUID, OCP_CUBE_NAME };
            else if (reportType == ReportType.DCP)
                cubesForProcess = new string[] { DCP_CUBE_GUID, DCP_CUBE_NAME };
            base.DirectProcessCube();
        }

        #endregion Расчет кубов

    }
}
