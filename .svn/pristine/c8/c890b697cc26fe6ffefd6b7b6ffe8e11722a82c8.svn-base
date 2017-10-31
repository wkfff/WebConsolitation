using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.Win32;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.SKIFMonthRepPump
{
    // ФО 2 мес отч скиф - формат пульс
    // только у новосибирска

    // !!!!!!!!!!!обязательно переписать при переписывании мес отч!!!!!!!!!! - НАПИСАН ПОЛНЫЙ АЦТОЙ

    public partial class SKIFMonthRepPumpModule : SKIFRepPumpModuleBase
    {

        #region поля

        private PulseReportPeriod pulseReportPeriod;
        private string refDate = string.Empty;
        private Database dbfDatabase = null;
        private DirectoryInfo sourceDir = null;
        private DataSet orgDS = null;
        private string regionKey = "";
        private List<string> pulseWarnedRegions = null;

        #endregion поля

        #region Структуры, перечисления

        // период отчетности 
        protected enum PulseReportPeriod
        {
            // до 2005 года 10 месяца
            period2004,
            // с 2005 года 10 месяца до 2006 года
            period2005,
            // с 2006 года до 2007 года 2 месяца
            period2006,
            // с 2007 года 2 месяца
            period2007,
        }

        #endregion Структуры, перечисления

        #region функции закачки классификаторов

        private void GetRegionBudget2005(ref object[] mapping, DataRow row)
        {
            int budgetKind = -1;
            string budgetName = string.Empty;
            string regionCode = row["CodeStr"].ToString();
            if (this.DataSource.Year <= 2005)
            {
                if (regionCode == "1037")
                {
                    budgetKind = 0;
                    budgetName = "Консолидированный бюджет субъекта";
                }
                else if (regionCode == "1038")
                {
                    budgetKind = 4;
                    budgetName = "Собственный бюджет субъекта";
                }
                else if ((regionCode.Length == 4) || (regionCode.Length == 8))
                {
                    budgetKind = 8;
                    budgetName = "Бюджет муниципального образования";
                }
            }
            mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "BudgetKind", budgetKind, "BudgetName", budgetName });
        }

        private void GetRegionBudget2007(ref object[] mapping, DataRow row)
        {
            string budgetName = string.Empty;
            string budgetKind = row["BudgetKind"].ToString();
            if (budgetKind == "0")
                budgetName = "Консолидированный бюджет МР";
            else
            {
                string query = string.Format("ID = {0}", budgetKind);
                DataRow[] sv_orgRows = orgDS.Tables[0].Select(query);
                if (sv_orgRows.GetLength(0) == 0)
                    budgetName = "";
                else
                    budgetName = sv_orgRows[0]["Org_Name"].ToString();
            }
            mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "BudgetName", budgetName });
        }

        private void GetRegionBudget(ref object[] mapping, DataRow row)
        {
            if (this.DataSource.Year <= 2005)
                GetRegionBudget2005(ref mapping, row);
            else
                GetRegionBudget2007(ref mapping, row);
        }

        private bool ProcessOutcomesRow(ref object[] mapping, DataRow row, IClassifier cls)
        {
            if (pulseReportPeriod == PulseReportPeriod.period2005)
            {
                string outcomesCode = row["Code"].ToString().Replace(" ", "");
                if (cls == clsFKR)
                {
                    if (!outcomesCode.EndsWith("000"))
                        return false;
                    mapping[1] = outcomesCode.Substring(3, 4);
                }
                else
                {
                    if (outcomesCode.EndsWith("000"))
                        return false;
                    mapping[1] = outcomesCode.Substring(outcomesCode.Length - 3, 3);
                }
            }
            else if (pulseReportPeriod != PulseReportPeriod.period2004)
            {
                if (cls == clsFKR)
                {
                    string outcomesName = row["Name"].ToString();
                    if (outcomesName == "Расходы бюджета - всего")
                    {
                        mapping[1] = 9800;
                        mapping[3] = outcomesName;
                    }
                    else if (outcomesName == "Расходы бюджета - итого")
                    {
                        mapping[1] = 9600;
                        mapping[3] = outcomesName;
                    }
                    else
                        mapping[3] = "Неуказанное наименование";
                    if (row["Code"].ToString().Trim() == string.Empty)
                        mapping[1] = 0;
                }
                else
                {
                    string outcomesCode = row["Code"].ToString();
                    if (outcomesCode == string.Empty)
                    {
                        mapping[1] = "0";
                        mapping[3] = "Неуказанное значение";
                    }
                }
            }
            return true;
        }

        private void ProcessInnerFinSourcesRow(ref object[] mapping)
        {
            if ((pulseReportPeriod != PulseReportPeriod.period2006) && (pulseReportPeriod != PulseReportPeriod.period2007))
                return;
            string name = mapping[3].ToString();
            if (name == "Источники финансирования дефицитов бюджетов - всего")
                mapping[1] = "00090000000000000000";
            // kl - последний 4 символа 
            mapping[5] = mapping[5].ToString().Substring(3, 4);
            // kst - первые 3 символа 
            mapping[7] = mapping[7].ToString().Substring(0, 3);
        }

        private string ProcessOutcomesRefsAddRow(ref object[] mapping)
        {
            if (pulseReportPeriod == PulseReportPeriod.period2007)
            {
                // фкр 4 - 7 символы
                mapping[1] = mapping[1].ToString().Substring(3, 4);
                // экр 18 - 20 символы
                mapping[3] = mapping[3].ToString().Substring(17, 3);
                // кцср 8 - 14 символы
                mapping[5] = mapping[5].ToString().Substring(7, 7);
                // квр 15 - 17 символы
                mapping[7] = mapping[7].ToString().Substring(14, 3);
                string code = string.Concat(mapping[1].ToString(), mapping[3].ToString(), mapping[13].ToString());
                mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "LongCode", code });
                return code;
            }
            else
            {
                string code = string.Concat(mapping[1].ToString(), mapping[3].ToString(), mapping[9].ToString());
                mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "KCSR", 0, "KVR", 0, "LongCode", code });
                return code;
            }
        }

        private string ProcessInnerFinSourcesRefsRow(ref object[] mapping)
        {
            string code = string.Empty;
            if (pulseReportPeriod != PulseReportPeriod.period2007)
            {
                code = string.Concat(mapping[1].ToString(), mapping[3].ToString(), mapping[9].ToString());
                mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "LongCode", code });
            }
            else
            {
                string GvrmInDebt = "000";
                code = string.Concat(mapping[1].ToString(), GvrmInDebt, mapping[7].ToString());
                mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "LongCode", code, "GvrmInDebt", GvrmInDebt });
            }
            return code;
        }

        private string ProcessOuterFinSourcesRefsRow(ref object[] mapping)
        {
            string code = string.Empty;
            if (pulseReportPeriod != PulseReportPeriod.period2007)
            {
                code = string.Concat(mapping[1].ToString(), mapping[3].ToString(), mapping[9].ToString());
                mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "LongCode", code });
            }
            else
            {
                string GvrmOutDebt = "000000";
                code = string.Concat(mapping[1].ToString(), GvrmOutDebt, mapping[7].ToString());
                mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "LongCode", code, "GvrmOutDebt", GvrmOutDebt });
            }
            return code;
        }

        private string ProcessArrearsRow(ref object[] mapping)
        {
            string code = string.Empty;
            if (pulseReportPeriod == PulseReportPeriod.period2007)
            {
                // фкр 4 - 7 символы
                mapping[1] = mapping[1].ToString().Substring(3, 4);
                // экр 18 - 20 символы
                mapping[3] = mapping[3].ToString().Substring(17, 3);
                // кцср 8 - 14 символы
                mapping[5] = mapping[5].ToString().Substring(7, 7);
                // квр 15 - 17 символы
                mapping[7] = mapping[7].ToString().Substring(14, 3);
                code = string.Concat(mapping[1].ToString(), mapping[5].ToString(), mapping[7].ToString(), 
                    mapping[3].ToString(), mapping[13].ToString());
                mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "LongCode", code });
            }
            else
            {
                code = string.Concat(mapping[1].ToString(), mapping[3].ToString(), mapping[9].ToString());
                mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "KCSR", 0, "KVR", 0, "LongCode", code });
            }
            return code;
        }

        private void PumpPulsePatternRow(string patternName, DataRow row, Dictionary<string, int> cache, 
            DataTable dt, IClassifier cls, string keyField)
        {
            object[] mapping = null;
            string keyValue = row[keyField].ToString().Replace(" ", "");

            if (!(((block == Block.bOutcomes) || (block == Block.bInnerFinSources)) && (this.DataSource.Year >= 2006)))
                if (CommonRoutines.TrimLetters(keyValue) == string.Empty)
                    return;

            foreach (DataColumn column in row.Table.Columns)
            {
                string value = row[column.ColumnName].ToString();
                if ((column.ColumnName == "Code") || (column.ColumnName == "CodeStr") ||
                    (column.ColumnName == "SrcOutFin") || (column.ColumnName == "SrcInFin"))
                    value = value.Replace(" ", "");
                if ((value == string.Empty) && (column.ColumnName == "Name"))
                    value = constDefaultClsName;
                mapping = (object[])CommonRoutines.ConcatArrays(mapping,
                    new object[] { column.ColumnName, value });
            }
            switch (block)
            {
                case Block.bRegions:
                    GetRegionBudget(ref mapping, row);
                    keyValue = string.Format("{0}|{1}", mapping[1].ToString().PadLeft(10, '0'), mapping[3].ToString());
                    regionKey = keyValue;
                    if (region4PumpCache.ContainsKey(keyValue))
                        if (region4PumpCache[keyValue] == 1)
                            return;
                    break;
                case Block.bOutcomes:
                    if (!ProcessOutcomesRow(ref mapping, row, cls))
                        return;
                    keyValue = mapping[1].ToString();
                    break;
                case Block.bInnerFinSources:
                    ProcessInnerFinSourcesRow(ref mapping);
                    break;
                case Block.bOutcomesRefs:
                    if (cls == clsEKRBook)
                    {
                        keyValue = keyValue.PadRight(6, '0');
                        mapping[1] = keyValue;
                    }
                    break;
                case Block.bOutcomesRefsAdd:
                    keyValue = ProcessOutcomesRefsAddRow(ref mapping);
                    break;
                case Block.bInnerFinSourcesRefs:
                    keyValue = ProcessInnerFinSourcesRefsRow(ref mapping);
                    break;
                case Block.bOuterFinSourcesRefs:
                    if (mapping[3].ToString().Trim() == string.Empty)
                        mapping[3] = "000000";
                    keyValue = ProcessOuterFinSourcesRefsRow(ref mapping);
                    break;
                case Block.bArrearsRefs:
                    keyValue = ProcessArrearsRow(ref mapping);
                    break;
            }
            PumpCachedRow(cache, dt, cls, keyValue, mapping);
        }

        private void PumpPulseUnknownCLS()
        {
            if (this.DataSource.Year < 2006)
                PumpCachedRow(ekrCache, dsEKR.Tables[0], clsEKR, "0",
                    new object[] { "Code", "0000", "Name", "Неуказанное наименование" });
            PumpCachedRow(fkrBookCache, dsFKRBook.Tables[0], clsFKRBook, "0",
                new object[] { "Code", "0000", "Name", "Неуказанное наименование" });
        }

        private string GetPulseDBFQuery(object[] mapping, string patternName, string constraint)
        {
            string dbfQuery = "select ";
            int length = mapping.GetLength(0);
            string comma = ",";
            for (int i = 0; i <= length - 1; i++)
            {
                if (i == length - 1)
                    comma = string.Empty;
                string dbfAlias = mapping[i].ToString().Split('=')[0];
                string dsAlias = mapping[i].ToString().Split('=')[1];
                dbfQuery += string.Format("{0} as {1}{2} ", dbfAlias, dsAlias, comma);
            }
            dbfQuery += string.Format("from {0} ", patternName);
            if (constraint != string.Empty)
                dbfQuery += string.Format("where {0} ", constraint);
            return dbfQuery;
        }

        private void PumpPulsePattern(FileInfo pattern, object[] mapping, string constraint, Dictionary<string, int> cache, 
            DataTable dt, IClassifier cls, string keyField)
        {
            IDbDataAdapter da = null;
            DataSet ds = new DataSet();
            string query = GetPulseDBFQuery(mapping, pattern.Name, constraint);
            InitDataSet(dbfDatabase, ref da, ref ds, query);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                PumpPulsePatternRow(pattern.Name, ds.Tables[0].Rows[i], cache, dt, cls, keyField);
        }

        private bool GetPulsePattern(DirectoryInfo dir, ref FileInfo pattern, string patternName)
        {
            FileInfo[] patterns = dir.GetFiles(patternName, SearchOption.AllDirectories);
            if (patterns.GetLength(0) == 0)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format("Не найден шаблон {0}", patternName));
                return false;
            }
            pattern = patterns[0];
            return true;
        }

        #region районы

        private const string REGION_PATTERN_NAME_2005 = "TS2.dbf";
        private const string REGION_PATTERN_NAME_2007 = "f128_d.dbf";
        private string GetRegionPatternName()
        {
            switch (pulseReportPeriod)
            {
                case PulseReportPeriod.period2004:
                case PulseReportPeriod.period2005:
                    return REGION_PATTERN_NAME_2005;
                case PulseReportPeriod.period2006:
                case PulseReportPeriod.period2007:
                    return REGION_PATTERN_NAME_2007;
            }
            return string.Empty;
        }

        private object[] REGION_DBF_MAPPING_2005 = new object[] { "Krn=CodeStr", "Org=Name" };
        private object[] REGION_DBF_MAPPING_2007 = new object[] { "INN=CodeStr", "Org_Name=Name", "SV_ID=BudgetKind" };
        private object[] GetRegionDbfMapping()
        {
            switch (pulseReportPeriod)
            {
                case PulseReportPeriod.period2004:
                case PulseReportPeriod.period2005:
                    return REGION_DBF_MAPPING_2005;
                case PulseReportPeriod.period2006:
                case PulseReportPeriod.period2007:
                    return REGION_DBF_MAPPING_2007;
            }
            return null;
        }

        private const string REGION_CONDITION_2007 = "INN <> '0051'";
        private string GetRegionCondition()
        {
            switch (pulseReportPeriod)
            {
                case PulseReportPeriod.period2004:
                case PulseReportPeriod.period2005:
                    return string.Empty;
                case PulseReportPeriod.period2006:
                case PulseReportPeriod.period2007:
                    return REGION_CONDITION_2007;
            }
            return string.Empty;
        }

        private void GetOrgDS()
        {
            IDbDataAdapter da = null;
            string query = "select * from Org_SV";
            InitDataSet(dbfDatabase, ref da, ref orgDS, query);
        }

        private bool PumpPulseRegionForPump(string code, string key, string name)
        {
            if (!region4PumpCache.ContainsKey(key))
            {
                PumpCachedRow(region4PumpCache, dsRegions4Pump.Tables[0], clsRegions4Pump, code, key, "CODESTR", "REFDOCTYPE",
                    new object[] { "NAME", name, "REFDOCTYPE", 1, "SOURCEID", regForPumpSourceID });
                return false;
            }
            return true;
        }

        private void PumpPulseRegionsForPump()
        {
            bool noRegForPump = false;
            foreach (DataRow row in dsRegions.Tables[0].Rows)
            {
                string code = row["CodeStr"].ToString().PadLeft(10, '0');
                string name = row["Name"].ToString();
                string regKey = string.Format("{0}|{1}", code, name);
                if (!PumpPulseRegionForPump(code, regKey, name))
                    noRegForPump = true;
            }
            if (noRegForPump)
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                    "Классификатор Районы.Служебный (SOURCEID {0}) имеет записи с неуказанным типом района. " +
                    "Необходимо установить значения поля \"ТипДокумента.СКИФ\" и запустить закачку снова.", regForPumpSourceID));
        }

        private bool PumpPulseRegions(DirectoryInfo dir)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'Районы.МесОтч'");
            FileInfo pattern = null;
            if (!GetPulsePattern(dir, ref pattern, GetRegionPatternName()))
                return false;
            if (this.DataSource.Year >= 2006)
                GetOrgDS();
            PumpPulsePattern(pattern, GetRegionDbfMapping(), GetRegionCondition(), regionCache, dsRegions.Tables[0], clsRegions, "CodeStr");
            PumpPulseRegionsForPump();
            if (this.DataSource.Year >= 2006)
                orgDS.Clear();
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'Районы.МесОтч'");
            return true;
        }

        #endregion районы

        #region доходы

        private const string KD_PATTERN_NAME_2004 = "Tsp3n.dbf";
        private const string KD_PATTERN_NAME_2005 = "TSP428.DBF";
        private const string KD_PATTERN_NAME_2007 = "f128_d.dbf";
        private string GetKDPatternName()
        {
            switch (pulseReportPeriod)
            {
                case PulseReportPeriod.period2004:
                    return KD_PATTERN_NAME_2004;
                case PulseReportPeriod.period2005:
                    return KD_PATTERN_NAME_2005;
                case PulseReportPeriod.period2006:
                case PulseReportPeriod.period2007:
                    return KD_PATTERN_NAME_2007;
            }
            return string.Empty;
        }

        private object[] KD_DBF_MAPPING_2004 = new object[] { "Kod=Code", "Im=Name", "VO=Kl", "U=Kst" };
        private object[] KD_DBF_MAPPING_2005 = new object[] { "Kod=CodeStr", "Im=Name", "VO=Kl", "U=Kst" };
        private object[] KD_DBF_MAPPING_2007 = new object[] { "Kod=CodeStr", "K_Name=Name", "10=Kl", "Str_Num=Kst" };
        private object[] GetKdDbfMapping()
        {
            if (this.DataSource.Year < 2005)
                return KD_DBF_MAPPING_2004;
            else if (this.DataSource.Year == 2005)
                return KD_DBF_MAPPING_2005;
            else
                return KD_DBF_MAPPING_2007;
        }

        private const string KD_CONDITION = "VO = '1'";
        private string GetKDCondition()
        {
            switch (pulseReportPeriod)
            {
                case PulseReportPeriod.period2004:
                case PulseReportPeriod.period2005:
                    return KD_CONDITION;
                case PulseReportPeriod.period2006:
                case PulseReportPeriod.period2007:
                    return string.Empty;
            }
            return string.Empty;
        }

        private void PumpIncomes(DirectoryInfo dir)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'КД.МесОтч'");
            FileInfo pattern = null;
            if (!GetPulsePattern(dir, ref pattern, GetKDPatternName()))
                return;
            PumpPulsePattern(pattern, GetKdDbfMapping(), GetKDCondition(), kdCache, dsKD.Tables[0], clsKD, "CodeStr");
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'КД.МесОтч'");
        }

        #endregion доходы

        #region расходы

        private const string FKR_PATTERN_NAME_2004 = "Tsp3n.dbf";
        private const string FKR_PATTERN_NAME_2005 = "TSP428.DBF";
        private const string FKR_PATTERN_NAME_2007 = "f128_r.dbf";
        private string GetFKRPatternName()
        {
            switch (pulseReportPeriod)
            {
                case PulseReportPeriod.period2004:
                    return FKR_PATTERN_NAME_2004;
                case PulseReportPeriod.period2005:
                    return FKR_PATTERN_NAME_2005;
                case PulseReportPeriod.period2006:
                case PulseReportPeriod.period2007:
                    return FKR_PATTERN_NAME_2007;
            }
            return string.Empty;
        }

        private object[] FKR_DBF_MAPPING_2005 = new object[] { "Kod=Code", "Im=Name", "VO=Kl", "U=Kst" };
        private object[] FKR_DBF_MAPPING_2007 = new object[] { "Rasd=Code", "Eco_Name=Name" };
        private object[] GetFkrDbfMapping()
        {
            switch (pulseReportPeriod)
            {
                case PulseReportPeriod.period2004:
                case PulseReportPeriod.period2005:
                    return FKR_DBF_MAPPING_2005;
                case PulseReportPeriod.period2006:
                case PulseReportPeriod.period2007:
                    return FKR_DBF_MAPPING_2007;
            }
            return null;
        }

        private object[] EKR_DBF_MAPPING_2005 = new object[] { "Kod=Code", "Im=Name", "VO=Kl", "U=Kst" };
        private object[] EKR_DBF_MAPPING_2007 = new object[] { "Eco_St=Code", "Eco_Name=Name" };
        private object[] GetEkrDbfMapping()
        {
            switch (pulseReportPeriod)
            {
                case PulseReportPeriod.period2004:
                case PulseReportPeriod.period2005:
                    return EKR_DBF_MAPPING_2005;
                case PulseReportPeriod.period2006:
                case PulseReportPeriod.period2007:
                    return EKR_DBF_MAPPING_2007;
            }
            return null;
        }

        private const string FKR_CONDITION_2005 = "VO = '2' and Kod <> '000 7900 0000000 000 000'";
        private const string FKR_CONDITION_2007 = "Str_Num <> 4500000";
        private string GetFkrCondition()
        {
            switch (pulseReportPeriod)
            {
                case PulseReportPeriod.period2004:
                case PulseReportPeriod.period2005:
                    return FKR_CONDITION_2005;
                case PulseReportPeriod.period2006:
                case PulseReportPeriod.period2007:
                    return FKR_CONDITION_2007;
            }
            return string.Empty;
        }

        private void PumpOutcomes(DirectoryInfo dir)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'ФКР.МесОтч'");
            FileInfo pattern = null;
            if (!GetPulsePattern(dir, ref pattern, GetFKRPatternName()))
                return;
            PumpPulsePattern(pattern, GetFkrDbfMapping(), GetFkrCondition(), fkrCache, dsFKR.Tables[0], clsFKR, "Code");
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'ФКР.МесОтч'");
            if (pulseReportPeriod == PulseReportPeriod.period2004)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Классификатор 'ЭКР.МесОтч' за данный период не качается.");
                return;
            }
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'ЭКР.МесОтч'");
            PumpPulsePattern(pattern, GetEkrDbfMapping(), GetFkrCondition(), ekrCache, dsEKR.Tables[0], clsEKR, "Code");
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'ЭКР.МесОтч'");
        }

        #endregion расходы

        #region источники внутреннего финансирования

        private const string KIF_INNER_PATTERN_NAME_2004 = "Tsp3n.dbf";
        private const string KIF_INNER_PATTERN_NAME_2005 = "TSP428.DBF";
        private const string KIF_INNER_PATTERN_NAME_2007 = "f128_i.dbf";
        private string GetKifInnerPatternName()
        {
            switch (pulseReportPeriod)
            {
                case PulseReportPeriod.period2004:
                    return KIF_INNER_PATTERN_NAME_2004;
                case PulseReportPeriod.period2005:
                    return KIF_INNER_PATTERN_NAME_2005;
                case PulseReportPeriod.period2006:
                case PulseReportPeriod.period2007:
                    return KIF_INNER_PATTERN_NAME_2007;
            }
            return string.Empty;
        }

        private object[] KIF_INNER_DBF_MAPPING_2004 = new object[] { "Kod=Code", "Im=Name", "VO=Kl", "U=Kst" };
        private object[] KIF_INNER_DBF_MAPPING_2005 = new object[] { "Kod=CodeStr", "Im=Name", "VO=Kl", "U=Kst" };
        private object[] KIF_INNER_DBF_MAPPING_2007 = new object[] { "Kod=CodeStr", "K_Name=Name", "Str_Num=Kl", "Str_Num=KST" };
        private object[] GetKifInnerDbfMapping()
        {
            if (this.DataSource.Year < 2005)
                return KIF_INNER_DBF_MAPPING_2004;
            else if (this.DataSource.Year == 2005)
                return KIF_INNER_DBF_MAPPING_2005;
            else
                return KIF_INNER_DBF_MAPPING_2007;
        }

        private const string KIF_INNER_CONDITION = "VO = '4'";
        private string GetKifInnerCondition()
        {
            switch (pulseReportPeriod)
            {
                case PulseReportPeriod.period2004:
                case PulseReportPeriod.period2005:
                    return KIF_INNER_CONDITION;
                case PulseReportPeriod.period2006:
                case PulseReportPeriod.period2007:
                    return string.Empty;
            }
            return string.Empty;
        }

        private void PumpInnerFinSources(DirectoryInfo dir)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'КИВнутрФ.МесОтч'");
            FileInfo pattern = null;
            if (!GetPulsePattern(dir, ref pattern, GetKifInnerPatternName()))
                return;
            PumpPulsePattern(pattern, GetKifInnerDbfMapping(), GetKifInnerCondition(),
                srcInFinCache, dsSrcInFin.Tables[0], clsSrcInFin, "CodeStr");
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'КИВнутрФ.МесОтч'");
        }

        #endregion источники внутреннего финансирования

        #region источники внешнего финансирования

        private const string KIF_OUTER_PATTERN_NAME_2004 = "Tsp3n.dbf";
        private const string KIF_OUTER_PATTERN_NAME_2005 = "TSP428.DBF";
        private string GetKifOuterPatternName()
        {
            switch (pulseReportPeriod)
            {
                case PulseReportPeriod.period2004:
                    return KIF_OUTER_PATTERN_NAME_2004;
                case PulseReportPeriod.period2005:
                    return KIF_OUTER_PATTERN_NAME_2005;
                case PulseReportPeriod.period2006:
                case PulseReportPeriod.period2007:
                    return string.Empty;
            }
            return string.Empty;
        }

        private object[] KIF_OUTER_DBF_MAPPING_2004 = new object[] { "Kod=Code", "Im=Name", "VO=Kl", "U=Kst" };
        private object[] KIF_OUTER_DBF_MAPPING_2005 = new object[] { "Kod=CodeStr", "Im=Name", "VO=Kl", "U=Kst" };
        private object[] KIF_OUTER_DBF_MAPPING_2007 = new object[] { "Kod=CodeStr", "Im=Name", "Str_Num=Kl", "Str_Num=Kst" };
        private object[] GetKifOuterDbfMapping()
        {
            if (this.DataSource.Year < 2005)
                return KIF_OUTER_DBF_MAPPING_2004;
            else if (this.DataSource.Year == 2005)
                return KIF_OUTER_DBF_MAPPING_2005;
            else
                return KIF_OUTER_DBF_MAPPING_2007;
        }

        private const string KIF_OUTER_CONDITION = "VO = '5'";
        private string GetKifOuterCondition()
        {
            switch (pulseReportPeriod)
            {
                case PulseReportPeriod.period2004:
                case PulseReportPeriod.period2005:
                    return KIF_OUTER_CONDITION;
                case PulseReportPeriod.period2006:
                case PulseReportPeriod.period2007:
                    return string.Empty;
            }
            return string.Empty;
        }

        private void PumpOuterFinSources(DirectoryInfo dir)
        {
            if (this.DataSource.Year > 2005)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Классификатор 'КИВнешФ.МесОтч' за данный период не качается.");
                return;
            }
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'КИВнешФ.МесОтч'");
            FileInfo pattern = null;
            if (!GetPulsePattern(dir, ref pattern, GetKifInnerPatternName()))
                return;
            PumpPulsePattern(pattern, GetKifOuterDbfMapping(), GetKifOuterCondition(),
                srcOutFinCache, dsSrcOutFin.Tables[0], clsSrcOutFin, "CodeStr");
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'КИВнешФ.МесОтч'");
        }

        #endregion источники внешнего финансирования

        #region справочные доходы

        private const string KVSR_PATTERN_NAME_2005 = "tsps.dbf";
        private const string KVSR_PATTERN_NAME_2006 = "f414.dbf";
        private string GetKVSRPatternName()
        {
            switch (pulseReportPeriod)
            {
                case PulseReportPeriod.period2004: 
                case PulseReportPeriod.period2005: 
                    return KVSR_PATTERN_NAME_2005;
                case PulseReportPeriod.period2006:
                    return KVSR_PATTERN_NAME_2006;
                case PulseReportPeriod.period2007:
                    return string.Empty;
            }
            return string.Empty;
        }

        private object[] KVSR_DBF_MAPPING_2004 = new object[] { "St=Code", "Im=Name", "U=Kl", "U=Kst" };
        private object[] KVSR_DBF_MAPPING_2005 = new object[] { "KodSO1=Code", "Im=Name", "Ctr=Kl", "Ctr=Kst" };
        private object[] KVSR_DBF_MAPPING_2006 = new object[] { "Kod1=Code", "K_Name=Name", "Ctr=Kl", "Str_Num=Kst" };
        private object[] GetKvsrDbfMapping()
        {
            switch (pulseReportPeriod)
            {
                case PulseReportPeriod.period2004:
                    return KVSR_DBF_MAPPING_2004;
                case PulseReportPeriod.period2005:
                    return KVSR_DBF_MAPPING_2005;
                case PulseReportPeriod.period2006:
                case PulseReportPeriod.period2007:
                    return KVSR_DBF_MAPPING_2006;
            }
            return null;
        }

        private const string KVSR_CONDITION2005 = "VO = '1'";
        private const string KVSR_CONDITION2006 = "Rasdel = 1";
        private string GetKvsrCondition()
        {
            switch (pulseReportPeriod)
            {
                case PulseReportPeriod.period2004:
                case PulseReportPeriod.period2005:
                    return KVSR_CONDITION2005;
                case PulseReportPeriod.period2006:
                    return KVSR_CONDITION2006;
                case PulseReportPeriod.period2007:
                    return string.Empty;
            }
            return string.Empty;
        }

        private void PumpIncomesRefs(DirectoryInfo dir)
        {
            if (pulseReportPeriod == PulseReportPeriod.period2007)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Классификатор 'Администратор.МесОтч' за данный период не качается.");
                return;
            }
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'Администратор.МесОтч'");
            FileInfo pattern = null;
            if (!GetPulsePattern(dir, ref pattern, GetKVSRPatternName()))
                return;
            PumpPulsePattern(pattern, GetKvsrDbfMapping(), GetKvsrCondition(), kvsrCache, dsKVSR.Tables[0], clsKVSR, "Code");
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'Администратор.МесОтч'");
        }

        #endregion справочные доходы

        #region справочные расходы

        #region ФКР.МесОтч_СправРасходы

        private const string FKR_REFS_PATTERN_NAME_2005 = "tsps.dbf";
        private const string FKR_REFS_PATTERN_NAME_2007 = "f414.dbf";
        private string GetFkrRefsPatternName()
        {
            switch (pulseReportPeriod)
            {
                case PulseReportPeriod.period2004:
                case PulseReportPeriod.period2005:
                    return FKR_REFS_PATTERN_NAME_2005;
                case PulseReportPeriod.period2006:
                    return FKR_REFS_PATTERN_NAME_2007;
            }
            return string.Empty;
        }

        private object[] FKR_REFS_DBF_MAPPING_2004 = new object[] { "Rasd=Code", "Im=Name", "VO=Kl", "U=Kst" };
        private object[] FKR_REFS_DBF_MAPPING_2005 = new object[] { "KodSP=Code", "Im=Name", "VO=Kl", "Ctr=Kst" };
        private object[] FKR_REFS_DBF_MAPPING_2006 = new object[] { "Kod=Code", "K_Name=Name" };
        private object[] GetFkrRefsDbfMapping()
        {
            switch (this.DataSource.Year)
            {
                case 2004:
                    return FKR_REFS_DBF_MAPPING_2004;
                case 2005:
                    return FKR_REFS_DBF_MAPPING_2005;
                default:
                    return FKR_REFS_DBF_MAPPING_2006;
            }
        }

        private const string FKR_REFS_CONDITION2004 = "(VO = '2') and (Rasd <> '0000') and ((U = '044') or (U = '051') or ((U >= '005') and (U <= '020')))";
        private const string FKR_REFS_CONDITION2005 = "(VO = '2') and (KodSP <> '0000') and (Ctr >= '002') and (Ctr <= '016')";
        private const string FKR_REFS_CONDITION2006 = "(Rasdel = 2) and (Kod <> '0000') and (Ctr >= '002') and (Ctr <= '016')";
        private string GetFkrRefsCondition()
        {
            if (this.DataSource.Year == 2004)
                return FKR_REFS_CONDITION2004;
            else if (this.DataSource.Year == 2005)
                return FKR_REFS_CONDITION2005;
            else if (pulseReportPeriod == PulseReportPeriod.period2006)
                return FKR_REFS_CONDITION2006;
            else
                return "";
        }

        private void PumpFKRRefs(DirectoryInfo dir)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'ФКР.МесОтч_СправРасходы'");
            FileInfo pattern = null;
            if (!GetPulsePattern(dir, ref pattern, GetFkrRefsPatternName()))
                return;
            PumpPulsePattern(pattern, GetFkrRefsDbfMapping(), GetFkrRefsCondition(), 
                fkrBookCache, dsFKRBook.Tables[0], clsFKRBook, "Code");
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'ФКР.МесОтч_СправРасходы'");
        }

        #endregion ФКР.МесОтч_СправРасходы

        #region ЭКР.МесОтч_СправРасходы

        private const string EKR_REFS_PATTERN_NAME_2005 = "tsps.dbf";
        private const string EKR_REFS_PATTERN_NAME_2007 = "f414.dbf";
        private string GetEkrRefsPatternName()
        {
            switch (pulseReportPeriod)
            {
                case PulseReportPeriod.period2004:
                case PulseReportPeriod.period2005:
                    return EKR_REFS_PATTERN_NAME_2005;
                case PulseReportPeriod.period2006:
                    return EKR_REFS_PATTERN_NAME_2007;
            }
            return string.Empty;
        }

        private object[] EKR_REFS_DBF_MAPPING_2004 = new object[] { "St=Code", "Im=Name", "VO=Kl", "U=Kst" };
        private object[] EKR_REFS_DBF_MAPPING_2005 = new object[] { "KodSO1=Code", "Im=Name", "VO=Kl", "Ctr=Kst" };
        private object[] EKR_REFS_DBF_MAPPING_2006 = new object[] { "Kod1=Code", "K_Name=Name" };
        private object[] GetEkrRefsDbfMapping()
        {
            switch (this.DataSource.Year)
            {
                case 2004:
                    return EKR_REFS_DBF_MAPPING_2004;
                case 2005:
                    return EKR_REFS_DBF_MAPPING_2005;
                case 2006:
                case 2007:
                    return EKR_REFS_DBF_MAPPING_2006;
            }
            return null;
        }

        private const string EKR_REFS_CONDITION2004 = "(VO = '2') and (Rasd = '0000') and ((U = '044') or (U = '051') or ((U >= '005') and (U <= '020')))";
        private const string EKR_REFS_CONDITION2005 = "(VO = '2') and (KodSP = '0000') and (Ctr >= '002') and (Ctr <= '016')";
        private const string EKR_REFS_CONDITION2006 = "(Rasdel = 2) and (Kod = '0000') and (Ctr >= '002') and (Ctr <= '016')";
        private string GetEkrRefsCondition()
        {
            if (this.DataSource.Year == 2004)
                return EKR_REFS_CONDITION2004;
            else if (this.DataSource.Year == 2005)
                return EKR_REFS_CONDITION2005;
            else if (pulseReportPeriod == PulseReportPeriod.period2006)
                return EKR_REFS_CONDITION2006;
            else
                return "";
        }

        private void PumpEKRRefs(DirectoryInfo dir)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'ЕКР.МесОтч_СправРасходы'");
            FileInfo pattern = null;
            if (!GetPulsePattern(dir, ref pattern, GetEkrRefsPatternName()))
                return;
            PumpPulsePattern(pattern, GetEkrRefsDbfMapping(), GetEkrRefsCondition(),
                ekrBookCache, dsEKRBook.Tables[0], clsEKRBook, "Code");
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'ЕКР.МесОтч_СправРасходы'");
        }

        #endregion ЭКР.МесОтч_СправРасходы

        private void PumpOutcomesRefs(DirectoryInfo dir)
        {
            if (pulseReportPeriod == PulseReportPeriod.period2007)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Классификаторы 'ФКР.МесОтч' и 'ЭКР.МесОтч' за данный период не качаются.");
                return;
            }
            PumpFKRRefs(dir);
            PumpEKRRefs(dir);
        }

        #endregion справочные расходы

        #region справочные расходы доп

        private const string MARKS_OUTCOMES_PATTERN_NAME_2005 = "tsps.dbf";
        private const string MARKS_OUTCOMES_PATTERN_NAME_2006 = "f414.dbf";
        private const string MARKS_OUTCOMES_PATTERN_NAME_2007 = "f387m.dbf";
        private string GetMarksOutcomesPatternName()
        {
            switch (pulseReportPeriod)
            {
                case PulseReportPeriod.period2004:
                case PulseReportPeriod.period2005:
                    return MARKS_OUTCOMES_PATTERN_NAME_2005;
                case PulseReportPeriod.period2006:
                    return MARKS_OUTCOMES_PATTERN_NAME_2006;
                case PulseReportPeriod.period2007:
                    return MARKS_OUTCOMES_PATTERN_NAME_2007;
            }
            return string.Empty;
        }

        private object[] MARKS_OUTCOMES_DBF_MAPPING_2004 = new object[] { "Rasd=FKR", "St=EKR", "Im=Name", "U=Kl", "U=Kst" };
        private object[] MARKS_OUTCOMES_DBF_MAPPING_2005 = new object[] { "KodSP=FKR", "KodSO1=EKR", "Im=Name", "Ctr=Kl", "Ctr=Kst" };
        private object[] MARKS_OUTCOMES_DBF_MAPPING_2006 = new object[] { "Kod=FKR", "Kod1=EKR", "K_Name=Name", "Ctr=Kl", "Str_Num=Kst" };
        private object[] MARKS_OUTCOMES_DBF_MAPPING_2007 = new object[] { "Kod=FKR", "Kod=EKR", "Kod=KCSR", "Kod=KVR", "K_Name=Name", "Str_Num=Kl", "Ctr=Kst" };
        private object[] GetMarksOutcomesDbfMapping()
        {
            if (this.DataSource.Year == 2004)
                return MARKS_OUTCOMES_DBF_MAPPING_2004;
            else if (this.DataSource.Year == 2005)
                return MARKS_OUTCOMES_DBF_MAPPING_2005;
            else if (pulseReportPeriod == PulseReportPeriod.period2006)
                return MARKS_OUTCOMES_DBF_MAPPING_2006;
            else
                return MARKS_OUTCOMES_DBF_MAPPING_2007;
        }

        private const string MARKS_OUTCOMES_CONDITION2004 = "(VO = '2') and (((U >= '021') and (U <= '043')) or ((U >= '045') and (U <= '050')) or ((U >= '052') and (U <= '084')))";
        private const string MARKS_OUTCOMES_CONDITION2005 = "(VO = '2') and (Ctr >= '017') and (Ctr <= '075')";
        private const string MARKS_OUTCOMES_CONDITION2006 = "(Rasdel = 2) and (Ctr >= '017') and (Ctr <= '075')";
        private const string MARKS_OUTCOMES_CONDITION2007 = "Rasdel = 2";
        private string GetMarksOutcomesCondition()
        {
            if (this.DataSource.Year == 2004)
                return MARKS_OUTCOMES_CONDITION2004;
            else if (this.DataSource.Year == 2005)
                return MARKS_OUTCOMES_CONDITION2005;
            else if (pulseReportPeriod == PulseReportPeriod.period2006)
                return MARKS_OUTCOMES_CONDITION2006;
            else
                return MARKS_OUTCOMES_CONDITION2007;
        }

        private void PumpOutcomesRefsAdd(DirectoryInfo dir)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'Показатели.МесОтч_СпрРасходы'");
            FileInfo pattern = null;
            if (!GetPulsePattern(dir, ref pattern, GetMarksOutcomesPatternName()))
                return;
             PumpPulsePattern(pattern, GetMarksOutcomesDbfMapping(), GetMarksOutcomesCondition(),
                marksOutcomesCache, dsMarksOutcomes.Tables[0], clsMarksOutcomes, "FKR");
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'Показатели.МесОтч_СпрРасходы'");
        }

        #endregion справочные расходы доп

        #region справочные внутренние источники финансирования

        private const string INNER_FIN_SOURCES_REFS_PATTERN_NAME_2005 = "tsps.dbf";
        private const string INNER_FIN_SOURCES_REFS_PATTERN_NAME_2006 = "f414.dbf";
        private const string INNER_FIN_SOURCES_REFS_PATTERN_NAME_2007 = "f387m.dbf";
        private string GetInnerFinSourcesRefsPatternName()
        {
            switch (pulseReportPeriod)
            {
                case PulseReportPeriod.period2004:
                case PulseReportPeriod.period2005:
                    return INNER_FIN_SOURCES_REFS_PATTERN_NAME_2005;
                case PulseReportPeriod.period2006:
                    return INNER_FIN_SOURCES_REFS_PATTERN_NAME_2006;
                case PulseReportPeriod.period2007:
                    return INNER_FIN_SOURCES_REFS_PATTERN_NAME_2007;
            }
            return string.Empty;
        }

        private object[] INNER_FIN_SOURCES_REFS_DBF_MAPPING_2004 = new object[] { "Rasd=SrcInFin", "St=GvrmInDebt", "Im=Name", "U=Kl", "U=Kst" };
        private object[] INNER_FIN_SOURCES_REFS_DBF_MAPPING_2005 = new object[] { "KodSp=SrcInFin", "KodSO1=GvrmInDebt", "Im=Name", "Ctr=Kl", "Ctr=Kst" };
        private object[] INNER_FIN_SOURCES_REFS_DBF_MAPPING_2006 = new object[] { "Kod=SrcInFin", "Kod1=GvrmInDebt", "K_Name=Name", "Ctr=Kl", "Str_Num=Kst" };
        private object[] INNER_FIN_SOURCES_REFS_DBF_MAPPING_2007 = new object[] { "Kod=SrcInFin", "K_Name=Name", "Str_Num=Kl", "Ctr=Kst" };
        private object[] GetInnerFinSourcesRefsDbfMapping()
        {
            if (this.DataSource.Year == 2004)
                return INNER_FIN_SOURCES_REFS_DBF_MAPPING_2004;
            else if (this.DataSource.Year == 2005)
                return INNER_FIN_SOURCES_REFS_DBF_MAPPING_2005;
            else if (pulseReportPeriod == PulseReportPeriod.period2006)
                return INNER_FIN_SOURCES_REFS_DBF_MAPPING_2006;
            else
                return INNER_FIN_SOURCES_REFS_DBF_MAPPING_2007;
        }

        private const string INNER_FIN_SOURCES_REFS_CONDITION2005 = "VO = '4'";
        private const string INNER_FIN_SOURCES_REFS_CONDITION2006 = "Rasdel = 4";
        private const string INNER_FIN_SOURCES_REFS_CONDITION2007 = "(Rasdel = 3) and (Ctr >= '4000') and (Ctr <= '4060')";
        private string GetInnerFinSourcesRefsCondition()
        {
            switch (pulseReportPeriod)
            {
                case PulseReportPeriod.period2004:
                case PulseReportPeriod.period2005:
                    return INNER_FIN_SOURCES_REFS_CONDITION2005;
                case PulseReportPeriod.period2006:
                    return INNER_FIN_SOURCES_REFS_CONDITION2006;
                case PulseReportPeriod.period2007:
                    return INNER_FIN_SOURCES_REFS_CONDITION2007;
            }
            return string.Empty;
        }

        private void PumpInnerFinSourcesRefs(DirectoryInfo dir)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'Показатели.МесОтч_СпрВнутрДолг'");
            FileInfo pattern = null;
            if (!GetPulsePattern(dir, ref pattern, GetInnerFinSourcesRefsPatternName()))
                return;
            PumpPulsePattern(pattern, GetInnerFinSourcesRefsDbfMapping(), GetInnerFinSourcesRefsCondition(),
                scrInFinSourcesRefCache, dsMarksInDebt.Tables[0], clsMarksInDebt, "SrcInFin");
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'Показатели.МесОтч_СпрВнутрДолг'");
        }

        #endregion справочные внутренние источники финансирования

        #region справочные внешние источники финансирования

        private const string OUTER_FIN_SOURCES_REFS_PATTERN_NAME_2005 = "tsps.dbf";
        private const string OUTER_FIN_SOURCES_REFS_PATTERN_NAME_2007 = "f387m.dbf";
        private string GetOuterFinSourcesRefsPatternName()
        {
            switch (pulseReportPeriod)
            {
                case PulseReportPeriod.period2004:
                case PulseReportPeriod.period2005:
                    return OUTER_FIN_SOURCES_REFS_PATTERN_NAME_2005;
                case PulseReportPeriod.period2006:
                    return string.Empty;
                case PulseReportPeriod.period2007:
                    return OUTER_FIN_SOURCES_REFS_PATTERN_NAME_2007;
            }
            return string.Empty;
        }

        private object[] OUTER_FIN_SOURCES_REFS_DBF_MAPPING_2004 = new object[] { "Rasd=SrcOutFin", "St=GvrmOutDebt", "Im=Name", "U=Kl", "U=Kst" };
        private object[] OUTER_FIN_SOURCES_REFS_DBF_MAPPING_2005 = new object[] { "KodSp=SrcOutFin", "KodSO1=GvrmOutDebt", "Im=Name", "Ctr=Kl", "Ctr=Kst" };
        private object[] OUTER_FIN_SOURCES_REFS_DBF_MAPPING_2007 = new object[] { "Kod=SrcOutFin", "K_Name=Name", "Str_Num=Kl", "Ctr=Kst" };
        private object[] GetOuterFinSourcesRefsDbfMapping()
        {
            if (this.DataSource.Year == 2004)
                return OUTER_FIN_SOURCES_REFS_DBF_MAPPING_2004;
            else if (this.DataSource.Year == 2005)
                return OUTER_FIN_SOURCES_REFS_DBF_MAPPING_2005;
            else if (pulseReportPeriod == PulseReportPeriod.period2006)
                return null;
            else
                return OUTER_FIN_SOURCES_REFS_DBF_MAPPING_2007;
        }

        private const string OUTER_FIN_SOURCES_REFS_CONDITION2005 = "VO = '5'";
        private const string OUTER_FIN_SOURCES_REFS_CONDITION2007 = "((Rasdel = 3) or (Rasdel = 4)) and (Ctr >= '4100') and (Ctr <= '4210')";
        private string GetOuterFinSourcesRefsCondition()
        {
            switch (pulseReportPeriod)
            {
                case PulseReportPeriod.period2004:
                case PulseReportPeriod.period2005:
                    return OUTER_FIN_SOURCES_REFS_CONDITION2005;
                case PulseReportPeriod.period2006:
                    return string.Empty;
                case PulseReportPeriod.period2007:
                    return OUTER_FIN_SOURCES_REFS_CONDITION2007;
            }
            return string.Empty;
        }

        private void PumpOuterFinSourcesRefs(DirectoryInfo dir)
        {
            if (pulseReportPeriod == PulseReportPeriod.period2006)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Классификатор 'Показатели.МесОтч_СпрВнешнДолг' за данный период не качается.");
                return;
            }
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'Показатели.МесОтч_СпрВнешнДолг'");
            FileInfo pattern = null;
            if (!GetPulsePattern(dir, ref pattern, GetOuterFinSourcesRefsPatternName()))
                return;
            PumpPulsePattern(pattern, GetOuterFinSourcesRefsDbfMapping(), GetOuterFinSourcesRefsCondition(),
                scrOutFinSourcesRefCache, dsMarksOutDebt.Tables[0], clsMarksOutDebt, "SrcOutFin");
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'Показатели.МесОтч_СпрВнешнДолг'");
        }

        #endregion справочные внешние источники финансирования

        #region задолженность

        private const string ARREARS_REFS_PATTERN_NAME_2005 = "tsps.dbf";
        private const string ARREARS_REFS_PATTERN_NAME_2006 = "f414.dbf";
        private const string ARREARS_REFS_PATTERN_NAME_2007 = "f387m.dbf";
        private string GetArrearsRefsPatternName()
        {
            switch (pulseReportPeriod)
            {
                case PulseReportPeriod.period2004:
                case PulseReportPeriod.period2005:
                    return ARREARS_REFS_PATTERN_NAME_2005;
                case PulseReportPeriod.period2006:
                    return ARREARS_REFS_PATTERN_NAME_2006;
                case PulseReportPeriod.period2007:
                    return ARREARS_REFS_PATTERN_NAME_2007;
            }
            return string.Empty;
        }

        private object[] ARREARS_REFS_DBF_MAPPING_2004 = new object[] { "Rasd=FKR", "St=EKR", "Im=Name", "U=Kl", "U=Kst" };
        private object[] ARREARS_REFS_DBF_MAPPING_2005 = new object[] { "KodSp=FKR", "KodSO1=EKR", "Im=Name", "Ctr=Kl", "Ctr=Kst" };
        private object[] ARREARS_REFS_DBF_MAPPING_2006 = new object[] { "Kod=FKR", "Kod1=EKR", "K_Name=Name", "Ctr=Kl", "Str_Num=Kst" };
        private object[] ARREARS_REFS_DBF_MAPPING_2007 = new object[] { "Kod=FKR", "Kod=EKR", "Kod=KCSR", "Kod=KVR", "K_Name=Name", "Str_Num=Kl", "Ctr=Kst" };
        private object[] GetArrearsRefsDbfMapping()
        {
            if (this.DataSource.Year == 2004)
                return ARREARS_REFS_DBF_MAPPING_2004;
            else if (this.DataSource.Year == 2005)
                return ARREARS_REFS_DBF_MAPPING_2005;
            else if (pulseReportPeriod == PulseReportPeriod.period2006)
                return ARREARS_REFS_DBF_MAPPING_2006;
            else
                return ARREARS_REFS_DBF_MAPPING_2007;
        }

        private const string ARREARS_REFS_CONDITION2005 = "VO = '6'";
        private const string ARREARS_REFS_CONDITION2007 = "Rasdel = 6";
        private string GetArrearsRefsCondition()
        {
            switch (pulseReportPeriod)
            {
                case PulseReportPeriod.period2004:
                case PulseReportPeriod.period2005:
                    return ARREARS_REFS_CONDITION2005;
                case PulseReportPeriod.period2006:
                case PulseReportPeriod.period2007:
                    return ARREARS_REFS_CONDITION2007;
            }
            return string.Empty;
        }

        private void PumpArrearsRefs(DirectoryInfo dir)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'Показатели.МесОтч_СпрЗадолженность'");
            FileInfo pattern = null;
            if (!GetPulsePattern(dir, ref pattern, GetArrearsRefsPatternName()))
                return;
            PumpPulsePattern(pattern, GetArrearsRefsDbfMapping(), GetArrearsRefsCondition(),
                arrearsCache, dsMarksArrears.Tables[0], clsMarksArrears, "FKR");
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'Показатели.МесОтч_СпрЗадолженность'");
        }

        #endregion задолженность

        private void PumpPatterns(DirectoryInfo dir)
        {
            PumpPulseUnknownCLS();
            block = Block.bRegions;
            if (this.DataSource.Year > 2005)
            {
                // копируем org_sv
                FileInfo[] orgSvFiles = sourceDir.GetFiles("org_sv.dbf");
                if (orgSvFiles.GetLength(0) == 0)
                    throw new Exception("Не найден шаблон org_sv.dbf");
                foreach (FileInfo file in orgSvFiles)
                    file.CopyTo(string.Format("{0}\\{1}", dir.FullName, file.Name), true);
                PumpPulseRegions(dir);
            }
            if (ToPumpBlock(Block.bIncomes))
            {
                block = Block.bIncomes;
                PumpIncomes(dir);
            }
            if (ToPumpBlock(Block.bOutcomes))
            {
                block = Block.bOutcomes;
                PumpOutcomes(dir);
            }
            if (ToPumpBlock(Block.bInnerFinSources))
            {
                block = Block.bInnerFinSources;
                PumpInnerFinSources(dir);
            }
            if (ToPumpBlock(Block.bOuterFinSources))
            {
                block = Block.bOuterFinSources;
                PumpOuterFinSources(dir);
            }
            if (ToPumpBlock(Block.bIncomesRefs))
            {
                block = Block.bIncomesRefs;
                PumpIncomesRefs(dir);
            }
            if (ToPumpBlock(Block.bOutcomesRefs))
            {
                block = Block.bOutcomesRefs;
                PumpOutcomesRefs(dir);
            }
            if (ToPumpBlock(Block.bOutcomesRefsAdd))
            {
                block = Block.bOutcomesRefsAdd;
                PumpOutcomesRefsAdd(dir);
            }
            if (ToPumpBlock(Block.bInnerFinSourcesRefs))
            {
                block = Block.bInnerFinSourcesRefs;
                PumpInnerFinSourcesRefs(dir);
            }
            if (ToPumpBlock(Block.bOuterFinSourcesRefs))
            {
                block = Block.bOuterFinSourcesRefs;
                PumpOuterFinSourcesRefs(dir);
            }
            if (ToPumpBlock(Block.bArrearsRefs))
            {
                block = Block.bArrearsRefs;
                PumpArrearsRefs(dir);
            }
        }

        #endregion функции закачки классификаторов

        #region функции закачки фактов

        private void ShowRegWarning(string regKey)
        {
            if (pulseWarnedRegions.Contains(regKey))
                return;
            string regCode = regKey.Split('|')[0];
            string regName = regKey.Split('|')[1];
            string message = string.Concat(string.Format("Значение признака 'Тип документа.Скиф' для района '{0}' (код {1}) ", regName, regCode),
                                          "равно 1 (Неуказанный тип отчетности). Данные по этому району закачаны не будут. ",
                                          "Заполните поле 'ТипДокумента.СКИФ' района и запустите закачку снова.");
            pulseWarnedRegions.Add(regKey);
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, message);
        }

        private bool CheckDocType(string regKey)
        {
            bool result = true;
            if (region4PumpCache.ContainsKey(regKey))
                result = (region4PumpCache[regKey] != 1);
            if (!result)
                ShowRegWarning(regKey);
            return result;
        }

        private bool CheckDocType2005(string regCode)
        {
            bool result = true;
            foreach (KeyValuePair<string, int> item in region4PumpCache)
                if (item.Key.StartsWith(regCode))
                {
                    result = (item.Value != 1);
                    if (!result)
                        ShowRegWarning(item.Key);
                    break;
                }
            return result;
        }

        private string GetPulseRegKey(string code, string name)
        {
            return string.Format("{0}|{1}", code.PadLeft(10, '0'), name);
        }

        #region факт 2004 - 2005

        private DataTable GetDBFFActData50(string reportName, string constraint)
        {
            IDbDataAdapter da = null;
            DataSet ds = new DataSet();
            string query = string.Format("select * from {0} where not " +
                "(P1 = 0 and P2 = 0 and P3 = 0 and P4 = 0 and P5 = 0 and P6 = 0 and P7 = 0 and P8 = 0 and P9 = 0)", reportName);
            if (constraint != string.Empty)
                query += "and " + constraint;
            InitDataSet(dbfDatabase, ref da, ref ds, query);
            return ds.Tables[0];
        }

        private DataTable GetDBFFActData51(string reportName, string constraint)
        {
            IDbDataAdapter da = null;
            DataSet ds = new DataSet();
            string query = string.Format("select * from {0} where not (P1 = 0 and P2 = 0 and P3 = 0)", reportName);
            if (constraint != string.Empty)
                query += " and " + constraint;
            InitDataSet(dbfDatabase, ref da, ref ds, query);
            return ds.Tables[0];
        }

        private int GetSumFactor2005()
        {
            int sumFactor = 1;
            int date = this.DataSource.Year * 100 + this.DataSource.Month;
            // Данные отчетов с января по сентябрь (включительно) 2005 года переводим из тысячей рублей в рубли. 
            if ((date >= 200501) && (date <= 200509))
                sumFactor = 1000;
            return sumFactor;
        }

        private void PumpPulseDbfFactRow50(DataTable dt, DataRow row, object[] clsMapping)
        {
            int sumFactor = GetSumFactor2005();

            object[] mapping = (object[])CommonRoutines.ConcatArrays(clsMapping, new object[] { "RefBdgtLevels", 2, 
                        "YearPlanReport", Convert.ToDouble(row["P1"].ToString().PadLeft(1, '0')) * sumFactor, 
                        "QuarterPlanReport", Convert.ToDouble(row["P4"].ToString().PadLeft(1, '0')) * sumFactor, 
                        "FactReport", Convert.ToDouble(row["P7"].ToString().PadLeft(1, '0')) * sumFactor });
            PumpRow(dt, mapping);

            mapping = (object[])CommonRoutines.ConcatArrays(clsMapping, new object[] { "RefBdgtLevels", 3, 
                        "YearPlanReport", Convert.ToDouble(row["P2"].ToString().PadLeft(1, '0')) * sumFactor, 
                        "QuarterPlanReport", Convert.ToDouble(row["P5"].ToString().PadLeft(1, '0')) * sumFactor, 
                        "FactReport", Convert.ToDouble(row["P8"].ToString().PadLeft(1, '0')) * sumFactor });
            PumpRow(dt, mapping);

            mapping = (object[])CommonRoutines.ConcatArrays(clsMapping, new object[] { "RefBdgtLevels", 7, 
                        "YearPlanReport", Convert.ToDouble(row["P3"].ToString().PadLeft(1, '0')) * sumFactor,
                        "QuarterPlanReport", Convert.ToDouble(row["P6"].ToString().PadLeft(1, '0')) * sumFactor, 
                        "FactReport", Convert.ToDouble(row["P9"].ToString().PadLeft(1, '0')) * sumFactor });
            PumpRow(dt, mapping);
        }

        private void PumpPulseDbfFactRow51(DataTable dt, DataRow row, object[] clsMapping)
        {
            int sumFactor = GetSumFactor2005();

            object[] mapping = (object[])CommonRoutines.ConcatArrays(clsMapping,
                new object[] { "RefBdgtLevels", 2, "FactReport", Convert.ToDouble(row["P1"].ToString().PadLeft(1, '0')) * sumFactor });
            PumpRow(dt, mapping);

            mapping = (object[])CommonRoutines.ConcatArrays(clsMapping,
                new object[] { "RefBdgtLevels", 3, "FactReport", Convert.ToDouble(row["P2"].ToString().PadLeft(1, '0')) * sumFactor });
            PumpRow(dt, mapping);

            mapping = (object[])CommonRoutines.ConcatArrays(clsMapping,
                new object[] { "RefBdgtLevels", 7, "FactReport", Convert.ToDouble(row["P3"].ToString().PadLeft(1, '0')) * sumFactor });
            PumpRow(dt, mapping);
        }

        private int FindRegionByCode(Dictionary<string, int> regCache, string regCode, int regDefault)
        {
            foreach (KeyValuePair<string, int> item in regCache)
                if (item.Key.StartsWith(regCode))
                    return item.Value;
            return regDefault;
        }

        private void PumpO65001Report(FileInfo report)
        {
            WriteToTrace(string.Format("Обработка отчета {0}", report.Name), TraceMessageKind.Information);
            int refRegion = nullRegions;
            object[] clsMapping = null;
            if (ToPumpBlock(Block.bIncomes))
            {
                DataTable dt = GetDBFFActData50(report.Name, "(KL = 1)");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow row = dt.Rows[i];
                    regionKey = row["ORG"].ToString().PadLeft(10, '0');
                    if (!CheckDocType2005(regionKey))
                        continue;
                    refRegion = FindRegionByCode(regionCache, regionKey, nullRegions);
                    string code = row["KBK"].ToString().Replace(" ", "");
                    int refKD = FindCachedRow(kdCache, code, nullKD);
                    clsMapping = new object[] { "RefYearDayUNV", refDate, "RefRegions", refRegion, "RefMeansType", 1, "RefKd", refKD };
                    PumpPulseDbfFactRow50(dsMonthRepIncomes.Tables[0], row, clsMapping);
                }
            }
            if (ToPumpBlock(Block.bOutcomes))
            {
                string constraint = "(KL = 2)";
                if (pulseReportPeriod == PulseReportPeriod.period2005)
                    constraint += " and (KBK <> '000 7900 0000000 000 000')";
                DataTable dt = GetDBFFActData50(report.Name, constraint);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow row = dt.Rows[i];
                    regionKey = row["ORG"].ToString().PadLeft(10, '0');
                    if (!CheckDocType2005(regionKey))
                        continue;
                    refRegion = FindRegionByCode(regionCache, regionKey, nullRegions);
                    string kbk = row["KBK"].ToString().Replace(" ", "");
                    string fkrCode = string.Empty;
                    string ekrCode = string.Empty;
                    if (kbk.Length > 4) 
                    {
                        fkrCode = kbk.Substring(3, 4);
                        ekrCode = Convert.ToInt32(kbk.Substring(kbk.Length - 3, 3)).ToString();
                    }
                    else
                        fkrCode = kbk;
                    int refFkr = FindCachedRow(fkrCache, fkrCode, nullFKR);
                    int refEKR = nullEKR;
                    if (pulseReportPeriod == PulseReportPeriod.period2004)
                        ekrCode = "0";
                    refEKR = FindCachedRow(ekrCache, ekrCode, nullEKR);
                    clsMapping = new object[] { "RefYearDayUNV", refDate, "RefRegions", refRegion, 
                        "RefFKR", refFkr, "RefEKR", refEKR, "RefMeansType", 1 };
                    PumpPulseDbfFactRow50(dsMonthRepOutcomes.Tables[0], row, clsMapping);
                }
            }
            if (ToPumpBlock(Block.bDefProf))
            {
                string constraint = "(KL = 3)";
                if (pulseReportPeriod == PulseReportPeriod.period2005)
                    constraint = "(KBK = '000 7900 0000000 000 000')";
                DataTable dt = GetDBFFActData50(report.Name, constraint);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow row = dt.Rows[i];
                    regionKey = row["ORG"].ToString().PadLeft(10, '0');
                    if (!CheckDocType2005(regionKey))
                        continue;
                    refRegion = FindRegionByCode(regionCache, regionKey, nullRegions);
                    clsMapping = new object[] { "RefYearDayUNV", refDate, "RefRegions", refRegion, "RefMeansType", 1 };
                    PumpPulseDbfFactRow50(dsMonthRepDefProf.Tables[0], row, clsMapping);
                }
            }
            if (ToPumpBlock(Block.bInnerFinSources))
            {
                DataTable dt = GetDBFFActData50(report.Name, "(KL = 4)");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow row = dt.Rows[i];
                    regionKey = row["ORG"].ToString().PadLeft(10, '0');
                    if (!CheckDocType2005(regionKey))
                        continue;
                    refRegion = FindRegionByCode(regionCache, regionKey, nullRegions);
                    string code = row["KBK"].ToString().Replace(" ", "");
                    int refInnerFinSource = FindCachedRow(srcInFinCache, code, nullSrcInFin);
                    clsMapping = new object[] { "RefYearDayUNV", refDate, "RefRegions", refRegion, "RefMeansType", 1, 
                            "RefSIF", refInnerFinSource };
                    PumpPulseDbfFactRow50(dsMonthRepInFin.Tables[0], row, clsMapping);
                }
            }
            if (ToPumpBlock(Block.bOuterFinSources))
            {
                DataTable dt = GetDBFFActData50(report.Name, "(KL = 5)");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow row = dt.Rows[i];
                    regionKey = row["ORG"].ToString().PadLeft(10, '0');
                    if (!CheckDocType2005(regionKey))
                        continue;
                    refRegion = FindRegionByCode(regionCache, regionKey, nullRegions);
                    string code = row["KBK"].ToString().Replace(" ", "");
                    int refOuterFinSource = nullSrcOutFin;
                    if (pulseReportPeriod != PulseReportPeriod.period2005)
                        refOuterFinSource = FindCachedRow(srcOutFinCache, code, nullSrcOutFin);
                    else
                    {
                        string name = constDefaultClsName;
                        if (code == "00090000000000000000")
                            name = "Итого источников финансирования дефицита бюджета - всего";
                        object[] mapping = new object[] { "CodeStr", code, "Name", name, 
                            "KL", row["KL"].ToString(), "KST", row["KST"].ToString() };
                        refOuterFinSource = PumpCachedRow(srcOutFinCache, dsSrcOutFin.Tables[0], clsSrcOutFin, code, mapping);
                    }
                    clsMapping = new object[] { "RefYearDayUNV", refDate, "RefRegions", refRegion, 
                        "RefMeansType", 1, "RefSOF", refOuterFinSource };
                    PumpPulseDbfFactRow50(dsMonthRepOutFin.Tables[0], row, clsMapping);
                }
            }
            WriteToTrace(string.Format("Обработка отчета {0} завершена", report.Name), TraceMessageKind.Information);
        }

        private void PumpO65101Report(FileInfo report)
        {
            WriteToTrace(string.Format("Обработка отчета {0}", report.Name), TraceMessageKind.Information);
            string kbk = "";
            int refRegion = nullRegions;
            object[] clsMapping = null;
            string klConstraint = "";
            string key = "";
            if (ToPumpBlock(Block.bIncomesRefs))
            {
                DataTable dt = GetDBFFActData51(report.Name, "KST = 1");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow row = dt.Rows[i];
                    regionKey = row["ORG"].ToString().PadLeft(10, '0');
                    if (!CheckDocType2005(regionKey))
                        continue;
                    refRegion = FindRegionByCode(regionCache, regionKey, nullRegions);
                    kbk = row["KBK"].ToString();
                    int refKVSR = FindCachedRow(kvsrCache, kbk.Substring(kbk.Length - 3, 3), nullKVSR);
                    clsMapping = new object[] { "RefYearDayUNV", refDate, "RefRegions", refRegion, 
                        "RefKVSR", refKVSR, "RefMeansType", 1 };
                    PumpPulseDbfFactRow51(dsMonthRepIncomesBooks.Tables[0], row, clsMapping);
                }
            }
            if (ToPumpBlock(Block.bOutcomesRefs))
            {
                if (this.DataSource.Year == 2004)
                    klConstraint = "((kl >= 5 and kl <= 20) or kl = 44 or kl = 51)";
                else
                    klConstraint = "(kl >= 2 and kl <= 16)";
                DataTable dt = GetDBFFActData51(report.Name, "(KST = 2) and " + klConstraint);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow row = dt.Rows[i];
                    regionKey = row["ORG"].ToString().PadLeft(10, '0');
                    if (!CheckDocType2005(regionKey))
                        continue;
                    refRegion = FindRegionByCode(regionCache, regionKey, nullRegions);
                    kbk = row["KBK"].ToString();
                    kbk = kbk.Replace(" ", "");
                    string fkr = kbk.Substring(0, 4);
                    if (fkr == "0000")
                        fkr = "0";
                    // если не было в шаблоне, качаем из отчета
                    int refFKR = PumpCachedRow(fkrBookCache, dsFKRBook.Tables[0], clsFKRBook, fkr,
                        new object[] { "Code", fkr, "Name", constDefaultClsName });
                    string ekr = "";
                    if (this.DataSource.Year == 2004)
                        ekr = kbk.Substring(4, 6);
                    else
                        ekr = kbk.Substring(4, 3).PadRight(6, '0');
                    // если не было в шаблоне, качаем из отчета
                    int refEKR = PumpCachedRow(ekrBookCache, dsEKRBook.Tables[0], clsEKRBook, ekr,
                        new object[] { "Code", ekr, "Name", constDefaultClsName });
                    clsMapping = new object[] { "RefYearDayUNV", refDate, "RefRegions", refRegion, 
                        "RefFKR", refFKR, "RefEKR", refEKR, "RefMeansType", 1 };
                    PumpPulseDbfFactRow51(dsMonthRepOutcomesBooks.Tables[0], row, clsMapping);
                }
            }
            if (ToPumpBlock(Block.bOutcomesRefsAdd))
            {
                if (this.DataSource.Year == 2004)
                    klConstraint = "(((kl >= 21) and (kl <= 43)) or ((kl >= 45) and (kl <= 50)) or ((kl >= 52) and (kl <= 84)))";
                else
                    klConstraint = "((kl >= 17) and (kl <= 75))";
                DataTable dt = GetDBFFActData51(report.Name, "(KST = 2) and " + klConstraint);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow row = dt.Rows[i];
                    regionKey = row["ORG"].ToString().PadLeft(10, '0');
                    if (!CheckDocType2005(regionKey))
                        continue;
                    refRegion = FindRegionByCode(regionCache, regionKey, nullRegions);
                    kbk = row["KBK"].ToString();
                    kbk = kbk.Replace(" ", "");
                    if (this.DataSource.Year == 2004)
                        key = kbk.Substring(0, 10) + row["kl"].ToString().PadLeft(3, '0');
                    else
                        key = kbk.Substring(0, 7) + row["kl"].ToString().PadLeft(3, '0');
                    int refMarksOutcomes = FindCachedRow(marksOutcomesCache, key, nullMarksOutcomes);
                    clsMapping = new object[] { "RefYearDayUNV", refDate, "RefRegions", refRegion, 
                        "RefMarksOutcomes", refMarksOutcomes, "RefMeansType", 1 };
                    PumpPulseDbfFactRow51(dsMonthRepOutcomesBooksEx.Tables[0], row, clsMapping);
                }
            }
            if (ToPumpBlock(Block.bInnerFinSourcesRefs))
            {
                DataTable dt = GetDBFFActData51(report.Name, "(KST = 4)");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow row = dt.Rows[i];
                    regionKey = row["ORG"].ToString().PadLeft(10, '0');
                    if (!CheckDocType2005(regionKey))
                        continue;
                    refRegion = FindRegionByCode(regionCache, regionKey, nullRegions);
                    kbk = row["KBK"].ToString().Replace(" ", "");
                    if (this.DataSource.Year == 2004)
                        key = kbk.Substring(0, 8) + row["kl"].ToString();
                    else
                        key = kbk.Substring(0, 23) + row["kl"].ToString().PadLeft(3, '0'); 
                    int refMarkInDebt = FindCachedRow(scrInFinSourcesRefCache, key, nullMarksInDebt);
                    clsMapping = new object[] { "RefYearDayUNV", refDate, "RefRegions", refRegion, 
                        "RefMarksInDebt", refMarkInDebt, "RefMeansType", 1 };
                    PumpPulseDbfFactRow51(dsMonthRepInDebtBooks.Tables[0], row, clsMapping);
                }
            }
            if (ToPumpBlock(Block.bOuterFinSourcesRefs))
            {
                DataTable dt = GetDBFFActData51(report.Name, "(KST = 5)");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow row = dt.Rows[i];
                    regionKey = row["ORG"].ToString().PadLeft(10, '0');
                    if (!CheckDocType2005(regionKey))
                        continue;
                    refRegion = FindRegionByCode(regionCache, regionKey, nullRegions);
                    kbk = row["KBK"].ToString().Replace(" ", "");
                    if (this.DataSource.Year == 2004)
                        key = kbk.Substring(0, 8) + row["kl"].ToString();
                    else
                        key = kbk.Substring(0, 23) + row["kl"].ToString().PadLeft(3, '0');
                    int refMarkOutDebt = FindCachedRow(scrOutFinSourcesRefCache, key, nullMarksOutDebt);
                    clsMapping = new object[] { "RefYearDayUNV", refDate, "RefRegions", refRegion, 
                        "RefMarksOutDebt", refMarkOutDebt, "RefMeansType", 1 };
                    PumpPulseDbfFactRow51(dsMonthRepOutDebtBooks.Tables[0], row, clsMapping);
                }
            }
            if (ToPumpBlock(Block.bArrearsRefs))
            {
                DataTable dt = GetDBFFActData51(report.Name, "(KST = 6)");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow row = dt.Rows[i];
                    regionKey = row["ORG"].ToString().PadLeft(10, '0');
                    if (!CheckDocType2005(regionKey))
                        continue;
                    refRegion = FindRegionByCode(regionCache, regionKey, nullRegions);
                    kbk = row["KBK"].ToString().Replace(" ", "");
                    if (this.DataSource.Year == 2004)
                        key = kbk.Substring(0, 10) + row["kl"].ToString();
                    else
                        key = kbk.Substring(0, 7) + row["kl"].ToString().PadLeft(3, '0');
                    int RefMarksArrears = FindCachedRow(arrearsCache, key, nullMarksArrears);
                    clsMapping = new object[] { "RefYearDayUNV", refDate, "RefRegions", refRegion, 
                        "RefMarksArrears", RefMarksArrears, "RefMeansType", 1 };
                    PumpPulseDbfFactRow51(dsMonthRepArrearsBooks.Tables[0], row, clsMapping);
                }
            }
            WriteToTrace(string.Format("Обработка отчета {0} завершена", report.Name), TraceMessageKind.Information);
        }

        #endregion факт 2004 - 2005

        #region факт 2006 - 2007

        private DataTable GetDBFFActData2007(string reportName, string constraint)
        {
            IDbDataAdapter da = null;
            DataSet ds = new DataSet();
            string query = string.Format("select * from {0} where ", reportName);
            if (constraint != string.Empty)
                query += constraint;
            InitDataSet(dbfDatabase, ref da, ref ds, query);
            return ds.Tables[0];
        }

        private int GetBudgetLevel2006(DataRow row)
        {
            int svID = Convert.ToInt32(row["SV_ID"].ToString());
            switch (svID)
            {
                case 0:
                    string inn = row["INN"].ToString();
                    if (inn.Length > 4)
                        return 10;
                    if ((Convert.ToInt32(inn) > 51))
                        return 10;
                    break;
                case 4:
                    return 3;
                case 5:
                    return 4;
                case 6:
                    return 5;
                case 7:
                    return 6;
                case 8:
                    return 6;
                case 9:
                    return 8;
                case 10:
                    return 9;
            }
            return -1;
        }

        private int GetPulseMeansType(string platMark)
        {
            if (platMark.ToUpper().Trim() == "ПЛАТНЫЕ")
                return 2;
            else
                return 1;
        }

        private void Pumpf128_dReport(FileInfo report)
        {
            if (!ToPumpBlock(Block.bIncomes))
                return;
            WriteToTrace(string.Format("Обработка отчета {0}", report.Name), TraceMessageKind.Information);
            int refRegion = nullRegions;
            object[] clsMapping = null;
            DataTable dt = GetDBFFActData2007(report.Name, "(INN <> '0051') and not (Plan = 0 and Fact = 0)");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow row = dt.Rows[i];
                string regKey = GetPulseRegKey(row["INN"].ToString(), row["ORG_NAME"].ToString());
                if (!CheckDocType(regKey))
                    continue;
                refRegion = FindCachedRow(regionCache, regKey, nullRegions);
                int budgetLevel = GetBudgetLevel2006(row);
                int refKD = FindCachedRow(kdCache, row["Kod"].ToString(), nullKD);
                int refMeansType = GetPulseMeansType(row["Plat_Mark"].ToString());
                clsMapping = new object[] { "RefYearDayUNV", refDate, "RefRegions", refRegion, 
                    "RefMeansType", refMeansType, "RefKD", refKD, "RefBdgtLevels", budgetLevel };
                object[] mapping = (object[])CommonRoutines.ConcatArrays(clsMapping,
                    new object[] { "YearPlanReport", Convert.ToDouble(row["Plan"]), "FactReport", Convert.ToDouble(row["Fact"]) });
                PumpRow(dsMonthRepIncomes.Tables[0], mapping);
            }
            WriteToTrace(string.Format("Обработка отчета {0} завершена", report.Name), TraceMessageKind.Information);
        }

        private void Pumpf128_rReport(FileInfo report)
        {
            WriteToTrace(string.Format("Обработка отчета {0}", report.Name), TraceMessageKind.Information);
            int refRegion = nullRegions;
            object[] clsMapping = null;
            DataTable dt = null;
            DataRow row = null;
            if (ToPumpBlock(Block.bOutcomes))
            {
                dt = GetDBFFActData2007(report.Name,
                    "(INN <> '0051') and (Str_Num <> 4500000) and not (Plan = 0 and Fact = 0 and Limit = 0)");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    row = dt.Rows[i];
                    string regKey = GetPulseRegKey(row["INN"].ToString(), row["ORG_NAME"].ToString());
                    if (!CheckDocType(regKey))
                        continue;
                    refRegion = FindCachedRow(regionCache, regKey, nullRegions);
                    int budgetLevel = GetBudgetLevel2006(row);
                    int refMeansType = GetPulseMeansType(row["Plat_Mark"].ToString());
                    string fkr = row["Rasd"].ToString();
                    string outcomesName = row["Eco_Name"].ToString().ToUpper();
                    if (outcomesName == "РАСХОДЫ БЮДЖЕТА - ВСЕГО")
                        fkr = "9800";
                    else if (outcomesName == "РАСХОДЫ БЮДЖЕТА - ИТОГО")
                        fkr = "9600";
                    string ekr = row["Eco_St"].ToString().PadLeft(1, '0');
                    int refFKR = FindCachedRow(fkrCache, fkr, nullFKR);
                    int refEKR = FindCachedRow(ekrCache, ekr, nullEKR);
                    clsMapping = new object[] { "RefYearDayUNV", refDate, "RefRegions", refRegion, 
                      "RefMeansType", refMeansType, "RefFKR", refFKR, "RefEKR", refEKR, "RefBdgtLevels", budgetLevel };
                    object[] mapping = (object[])CommonRoutines.ConcatArrays(clsMapping,
                        new object[] { "YearPlanReport", Convert.ToDouble(row["Plan"]), 
                                   "FactReport", Convert.ToDouble(row["Fact"]), "MonthPlanReport", Convert.ToDouble(row["Limit"]) });
                    PumpRow(dsMonthRepOutcomes.Tables[0], mapping);
                }
            }
            if (ToPumpBlock(Block.bDefProf))
            {
                dt = GetDBFFActData2007(report.Name,
                    "(INN <> '0051') and (Str_Num = 4500000) and not (Plan = 0 and Fact = 0 and Limit = 0)");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    row = dt.Rows[i];
                    string regKey = GetPulseRegKey(row["INN"].ToString(), row["ORG_NAME"].ToString());
                    if (!CheckDocType(regKey))
                        continue;
                    refRegion = FindCachedRow(regionCache, regKey, nullRegions);
                    int budgetLevel = GetBudgetLevel2006(row);
                    int refMeansType = GetPulseMeansType(row["Plat_Mark"].ToString());
                    clsMapping = new object[] { "RefYearDayUNV", refDate, "RefRegions", refRegion, 
                        "RefMeansType", refMeansType, "RefBdgtLevels", budgetLevel };
                    object[] mapping = (object[])CommonRoutines.ConcatArrays(clsMapping,
                        new object[] { "YearPlanReport", Convert.ToDouble(row["Plan"]), 
                                   "FactReport", Convert.ToDouble(row["Fact"]), "MonthPlanReport", Convert.ToDouble(row["Limit"]) });
                    PumpRow(dsMonthRepDefProf.Tables[0], mapping);
                }
            }
            WriteToTrace(string.Format("Обработка отчета {0} завершена", report.Name), TraceMessageKind.Information);
        }

        private void Pumpf128_iReport(FileInfo report)
        {
            if (!ToPumpBlock(Block.bInnerFinSources))
                return;
            WriteToTrace(string.Format("Обработка отчета {0}", report.Name), TraceMessageKind.Information);
            int refRegion = nullRegions;
            object[] clsMapping = null;
            DataTable dt = GetDBFFActData2007(report.Name, "(INN <> '0051') and not (Plan = 0 and Fact = 0)");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow row = dt.Rows[i];
                string regKey = GetPulseRegKey(row["INN"].ToString(), row["ORG_NAME"].ToString());
                if (!CheckDocType(regKey))
                    continue;
                refRegion = FindCachedRow(regionCache, regKey, nullRegions);
                int budgetLevel = GetBudgetLevel2006(row);
                int refMeansType = GetPulseMeansType(row["Plat_Mark"].ToString());
                int refSrcInFin = FindCachedRow(srcInFinCache, row["Kod"].ToString(), nullSrcInFin);
                clsMapping = new object[] { "RefYearDayUNV", refDate, "RefRegions", refRegion, 
                    "RefMeansType", refMeansType, "RefSIF", refSrcInFin, "RefBdgtLevels", budgetLevel };
                object[] mapping = (object[])CommonRoutines.ConcatArrays(clsMapping,
                    new object[] { "YearPlanReport", Convert.ToDouble(row["Plan"]), "FactReport", Convert.ToDouble(row["Fact"]) });
                PumpRow(dsMonthRepInFin.Tables[0], mapping);
            }
            WriteToTrace(string.Format("Обработка отчета {0} завершена", report.Name), TraceMessageKind.Information);
        }

        private void Pumpf414Report(FileInfo report)
        {
            WriteToTrace(string.Format("Обработка отчета {0}", report.Name), TraceMessageKind.Information);
            int refRegion = nullRegions;
            object[] clsMapping = null;
            DataTable dt = null;
            DataRow row = null;
            // справочные доходы
            if (ToPumpBlock(Block.bIncomesRefs))
            {
                dt = GetDBFFActData2007(report.Name, "(INN <> '0051') and (Rasdel = 1) and not (C5 = 0 and C6 = 0 and C7 = 0)");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    row = dt.Rows[i];
                    string regKey = GetPulseRegKey(row["INN"].ToString(), row["ORG_NAME"].ToString());
                    if (!CheckDocType(regKey))
                        continue;
                    refRegion = FindCachedRow(regionCache, regKey, nullRegions);
                    int refMeansType = GetPulseMeansType(row["Plat_Mark"].ToString());
                    int refKVSR = FindCachedRow(kvsrCache, row["Kod"].ToString(), nullKVSR);
                    clsMapping = new object[] { "RefYearDayUNV", refDate, "RefRegions", refRegion, 
                       "RefMeansType", refMeansType, "RefKVSR", refKVSR };
                    object[] mapping = (object[])CommonRoutines.ConcatArrays(clsMapping,
                        new object[] { "FactReport", Convert.ToDouble(row["C5"]), "RefBdgtLevels", 2 });
                    PumpRow(dsMonthRepIncomesBooks.Tables[0], mapping);
                    mapping = (object[])CommonRoutines.ConcatArrays(clsMapping,
                        new object[] { "FactReport", Convert.ToDouble(row["C6"]), "RefBdgtLevels", 3 });
                    PumpRow(dsMonthRepIncomesBooks.Tables[0], mapping);
                    mapping = (object[])CommonRoutines.ConcatArrays(clsMapping,
                        new object[] { "FactReport", Convert.ToDouble(row["C7"]), "RefBdgtLevels", 7 });
                    PumpRow(dsMonthRepIncomesBooks.Tables[0], mapping);
                }
            }
            // справочные расходы
            if (ToPumpBlock(Block.bOutcomesRefs))
            {
                dt = GetDBFFActData2007(report.Name,
                    "(INN <> '0051') and (Rasdel = 2) and not (C5 = 0 and C6 = 0 and C7 = 0)");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    row = dt.Rows[i];
                    int ctr = Convert.ToInt32(row["ctr"].ToString());
                    if ((ctr < 2) || (ctr > 16))
                        continue;
                    string regKey = GetPulseRegKey(row["INN"].ToString(), row["ORG_NAME"].ToString());
                    if (!CheckDocType(regKey))
                        continue;
                    refRegion = FindCachedRow(regionCache, regKey, nullRegions);
                    int refMeansType = GetPulseMeansType(row["Plat_Mark"].ToString());
                    int refFKR = FindCachedRow(fkrBookCache, row["Kod"].ToString(), nullFKRBook);
                    int refEKR = FindCachedRow(ekrBookCache, row["Kod"].ToString(), nullEKRBook);
                    clsMapping = new object[] { "RefYearDayUNV", refDate, "RefRegions", refRegion, 
                       "RefMeansType", refMeansType, "RefFKR", refFKR, "RefEKR", refEKR };
                    object[] mapping = (object[])CommonRoutines.ConcatArrays(clsMapping,
                        new object[] { "FactReport", Convert.ToDouble(row["C5"]), "RefBdgtLevels", 2 });
                    PumpRow(dsMonthRepOutcomesBooks.Tables[0], mapping);
                    mapping = (object[])CommonRoutines.ConcatArrays(clsMapping,
                        new object[] { "FactReport", Convert.ToDouble(row["C6"]), "RefBdgtLevels", 3 });
                    PumpRow(dsMonthRepOutcomesBooks.Tables[0], mapping);
                    mapping = (object[])CommonRoutines.ConcatArrays(clsMapping,
                        new object[] { "FactReport", Convert.ToDouble(row["C7"]), "RefBdgtLevels", 7 });
                    PumpRow(dsMonthRepOutcomesBooks.Tables[0], mapping);
                }
            }
            // справочные расходы доп
            if (ToPumpBlock(Block.bOutcomesRefsAdd))
            {
                dt = GetDBFFActData2007(report.Name,
                    "(INN <> '0051') and (Rasdel = 2) and not (C5 = 0 and C6 = 0 and C7 = 0)");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    row = dt.Rows[i];
                    int ctr = Convert.ToInt32(row["ctr"].ToString());
                    if ((ctr < 17) || (ctr > 75))
                        continue;
                    string regKey = GetPulseRegKey(row["INN"].ToString(), row["ORG_NAME"].ToString());
                    if (!CheckDocType(regKey))
                        continue;
                    refRegion = FindCachedRow(regionCache, regKey, nullRegions);
                    int refMeansType = GetPulseMeansType(row["Plat_Mark"].ToString());
                    // фкр 4 - 7 символы
                    string fkr = row["Kod"].ToString();
                    // экр 18 - 20 символы
                    string ekr = row["Kod1"].ToString();
                    string kst = row["Str_Num"].ToString();
                    string code = string.Concat(fkr, ekr, kst);
                    int refMarksOutcomes = FindCachedRow(marksOutcomesCache, code, nullMarksOutcomes);
                    clsMapping = new object[] { "RefYearDayUNV", refDate, "RefRegions", refRegion, 
                       "RefMeansType", refMeansType, "RefMarksOutcomes", refMarksOutcomes };
                    object[] mapping = (object[])CommonRoutines.ConcatArrays(clsMapping,
                        new object[] { "FactReport", Convert.ToDouble(row["C5"]), "RefBdgtLevels", 2 });
                    PumpRow(dsMonthRepOutcomesBooksEx.Tables[0], mapping);
                    mapping = (object[])CommonRoutines.ConcatArrays(clsMapping,
                        new object[] { "FactReport", Convert.ToDouble(row["C6"]), "RefBdgtLevels", 3 });
                    PumpRow(dsMonthRepOutcomesBooksEx.Tables[0], mapping);
                    mapping = (object[])CommonRoutines.ConcatArrays(clsMapping,
                        new object[] { "FactReport", Convert.ToDouble(row["C7"]), "RefBdgtLevels", 7 });
                    PumpRow(dsMonthRepOutcomesBooksEx.Tables[0], mapping);
                }
            }
            // справочный внутренний долг
            if (ToPumpBlock(Block.bInnerFinSourcesRefs))
            {
                dt = GetDBFFActData2007(report.Name,
                    "(INN <> '0051') and (Rasdel = 4) and not (C5 = 0 and C6 = 0 and C7 = 0)");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    row = dt.Rows[i];
                    string regKey = GetPulseRegKey(row["INN"].ToString(), row["ORG_NAME"].ToString());
                    if (!CheckDocType(regKey))
                        continue;
                    refRegion = FindCachedRow(regionCache, regKey, nullRegions);
                    int refMeansType = GetPulseMeansType(row["Plat_Mark"].ToString());
                    string marksInDebtCode = row["Kod"].ToString();
                    if (this.DataSource.Year >= 2006)
                        marksInDebtCode = string.Concat(row["Kod"].ToString(), row["Kod1"].ToString(), row["Str_Num"].ToString());
                    int refMarksInDebt = FindCachedRow(scrInFinSourcesRefCache, marksInDebtCode, nullMarksInDebt);
                    clsMapping = new object[] { "RefYearDayUNV", refDate, "RefRegions", refRegion, 
                       "RefMeansType", refMeansType, "RefMarksInDebt", refMarksInDebt };
                    object[] mapping = (object[])CommonRoutines.ConcatArrays(clsMapping,
                        new object[] { "FactReport", Convert.ToDouble(row["C5"]), "RefBdgtLevels", 2 });
                    PumpRow(dsMonthRepInDebtBooks.Tables[0], mapping);
                    mapping = (object[])CommonRoutines.ConcatArrays(clsMapping,
                        new object[] { "FactReport", Convert.ToDouble(row["C6"]), "RefBdgtLevels", 3 });
                    PumpRow(dsMonthRepInDebtBooks.Tables[0], mapping);
                    mapping = (object[])CommonRoutines.ConcatArrays(clsMapping,
                        new object[] { "FactReport", Convert.ToDouble(row["C7"]), "RefBdgtLevels", 7 });
                    PumpRow(dsMonthRepInDebtBooks.Tables[0], mapping);
                }
            }
            // справочные задолженности
            if (ToPumpBlock(Block.bArrearsRefs))
            {
                dt = GetDBFFActData2007(report.Name,
                    "(INN <> '0051') and (Rasdel = 6) and not (C5 = 0 and C6 = 0 and C7 = 0)");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    row = dt.Rows[i];
                    string regKey = GetPulseRegKey(row["INN"].ToString(), row["ORG_NAME"].ToString());
                    if (!CheckDocType(regKey))
                        continue;
                    refRegion = FindCachedRow(regionCache, regKey, nullRegions);
                    int refMeansType = GetPulseMeansType(row["Plat_Mark"].ToString());
                    string marksArrearsCode = row["Kod"].ToString();
                    if (this.DataSource.Year >= 2006)
                        marksArrearsCode = string.Concat(row["Kod"].ToString(), row["Kod1"].ToString(), row["Str_Num"].ToString());
                    int refMarksArrears = FindCachedRow(arrearsCache, marksArrearsCode, nullMarksArrears);
                    clsMapping = new object[] { "RefYearDayUNV", refDate, "RefRegions", refRegion, 
                       "RefMeansType", refMeansType, "RefMarksArrears", refMarksArrears };
                    object[] mapping = (object[])CommonRoutines.ConcatArrays(clsMapping,
                        new object[] { "FactReport", Convert.ToDouble(row["C5"]), "RefBdgtLevels", 2 });
                    PumpRow(dsMonthRepArrearsBooks.Tables[0], mapping);
                    mapping = (object[])CommonRoutines.ConcatArrays(clsMapping,
                        new object[] { "FactReport", Convert.ToDouble(row["C6"]), "RefBdgtLevels", 3 });
                    PumpRow(dsMonthRepArrearsBooks.Tables[0], mapping);
                    mapping = (object[])CommonRoutines.ConcatArrays(clsMapping,
                        new object[] { "FactReport", Convert.ToDouble(row["C7"]), "RefBdgtLevels", 7 });
                    PumpRow(dsMonthRepArrearsBooks.Tables[0], mapping);
                }
            }
            WriteToTrace(string.Format("Обработка отчета {0} завершена", report.Name), TraceMessageKind.Information);
        }

        private void Pumpf387mReport(FileInfo report)
        {
            WriteToTrace(string.Format("Обработка отчета {0}", report.Name), TraceMessageKind.Information);
            int refRegion = nullRegions;
            object[] clsMapping = null;
            DataTable dt = null;
            DataRow row = null;
            // справочные расходы доп
            if (ToPumpBlock(Block.bOutcomesRefsAdd))
            {
                dt = GetDBFFActData2007(report.Name, "(INN <> '0051') and (Rasdel = 2) and not (C4 = 0 and C5 = 0)");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    row = dt.Rows[i];
                    string regKey = GetPulseRegKey(row["INN"].ToString(), row["ORG_NAME"].ToString());
                    if (!CheckDocType(regKey))
                        continue;
                    refRegion = FindCachedRow(regionCache, regKey, nullRegions);
                    int budgetLevel = GetBudgetLevel2006(row);
                    int refMeansType = GetPulseMeansType(row["Plat_Mark"].ToString());
                    string code = row["Kod"].ToString();
                    // фкр 4 - 7 символы
                    string fkr = code.Substring(3, 4);
                    // экр 18 - 20 символы
                    string ekr = code.Substring(17, 3);
                    code = string.Concat(fkr, ekr, row["Ctr"].ToString());
                    int refMarksOutcomes = FindCachedRow(marksOutcomesCache, code, nullMarksOutcomes);
                    clsMapping = new object[] { "RefYearDayUNV", refDate, "RefRegions", refRegion, 
                       "RefMeansType", refMeansType, "RefMarksOutcomes", refMarksOutcomes, "RefBdgtLevels", budgetLevel };
                    object[] mapping = (object[])CommonRoutines.ConcatArrays(clsMapping,
                        new object[] { "YearPlanReport", Convert.ToDouble(row["C4"]), "FactReport", Convert.ToDouble(row["C5"]) });
                    PumpRow(dsMonthRepOutcomesBooksEx.Tables[0], mapping);
                }
            }
            // справочный внутренний долг
            if (ToPumpBlock(Block.bInnerFinSourcesRefs))
            {
                dt = GetDBFFActData2007(report.Name,
                    "(INN <> '0051') and (Rasdel = 3) and not (C5 = 0)");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    row = dt.Rows[i];
                    int ctr = Convert.ToInt32(row["ctr"].ToString());
                    if ((ctr < 4000) || (ctr > 4060))
                        continue;
                    string regKey = GetPulseRegKey(row["INN"].ToString(), row["ORG_NAME"].ToString());
                    if (!CheckDocType(regKey))
                        continue;
                    refRegion = FindCachedRow(regionCache, regKey, nullRegions);
                    int budgetLevel = GetBudgetLevel2006(row);
                    int refMeansType = GetPulseMeansType(row["Plat_Mark"].ToString());
                    string marksInDebtCode = string.Concat(row["Kod"].ToString(), "000", row["Ctr"].ToString());
                    int refMarksInDebt = FindCachedRow(scrInFinSourcesRefCache, marksInDebtCode, nullMarksInDebt);
                    clsMapping = new object[] { "RefYearDayUNV", refDate, "RefRegions", refRegion, 
                       "RefMeansType", refMeansType, "RefMarksInDebt", refMarksInDebt, "RefBdgtLevels", budgetLevel };
                    object[] mapping = (object[])CommonRoutines.ConcatArrays(clsMapping,
                        new object[] { "FactReport", Convert.ToDouble(row["C5"]) });
                    PumpRow(dsMonthRepInDebtBooks.Tables[0], mapping);
                }
            }
            // справочный внешний долг
            if (ToPumpBlock(Block.bOuterFinSourcesRefs))
            {
                dt = GetDBFFActData2007(report.Name,
                    "(INN <> '0051') and (Rasdel = 4) and not (C5 = 0)");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    row = dt.Rows[i];
                    int ctr = Convert.ToInt32(row["ctr"].ToString());
                    if ((ctr < 4100) || (ctr > 4210))
                        continue;
                    string regKey = GetPulseRegKey(row["INN"].ToString(), row["ORG_NAME"].ToString());
                    if (!CheckDocType(regKey))
                        continue;
                    refRegion = FindCachedRow(regionCache, regKey, nullRegions);
                    int budgetLevel = GetBudgetLevel2006(row);
                    int refMeansType = GetPulseMeansType(row["Plat_Mark"].ToString());
                    string marksOutDebtCode = string.Concat(row["Kod"].ToString(), "000000", row["Ctr"].ToString());
                    int refMarksOutDebt = FindCachedRow(scrOutFinSourcesRefCache, marksOutDebtCode, nullMarksOutDebt);
                    clsMapping = new object[] { "RefYearDayUNV", refDate, "RefRegions", refRegion, 
                       "RefMeansType", refMeansType, "RefMarksOutDebt", refMarksOutDebt, "RefBdgtLevels", budgetLevel };
                    object[] mapping = (object[])CommonRoutines.ConcatArrays(clsMapping,
                        new object[] { "FactReport", Convert.ToDouble(row["C5"]) });
                    PumpRow(dsMonthRepOutDebtBooks.Tables[0], mapping);
                }
            }
            // справочные задолженности
            if (ToPumpBlock(Block.bArrearsRefs))
            {
                dt = GetDBFFActData2007(report.Name, "(INN <> '0051') and (Rasdel = 6) and not (C4 = 0 and C5 = 0)");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    row = dt.Rows[i];
                    string regKey = GetPulseRegKey(row["INN"].ToString(), row["ORG_NAME"].ToString());
                    if (!CheckDocType(regKey))
                        continue;
                    refRegion = FindCachedRow(regionCache, regKey, nullRegions);
                    int budgetLevel = GetBudgetLevel2006(row);
                    int refMeansType = GetPulseMeansType(row["Plat_Mark"].ToString());
                    string arrearsCode = row["Kod"].ToString();
                    // фкр 4 - 7 символы
                    string fkr = arrearsCode.Substring(3, 4);
                    // экр 18 - 20 символы
                    string ekr = arrearsCode.Substring(17, 3);
                    // кцср 8 - 14 символы
                    string kcsr = arrearsCode.Substring(7, 7);
                    // квр 15 - 17 символы
                    string kvr = arrearsCode.Substring(14, 3);
                    string kst = row["Ctr"].ToString();
                    arrearsCode = string.Concat(fkr, kcsr, kvr, ekr, kst);
                    int refMarksArrears = FindCachedRow(arrearsCache, arrearsCode, nullMarksOutDebt);
                    clsMapping = new object[] { "RefYearDayUNV", refDate, "RefRegions", refRegion, 
                       "RefMeansType", refMeansType, "RefMarksArrears", refMarksArrears, "RefBdgtLevels", budgetLevel };
                    object[] mapping = (object[])CommonRoutines.ConcatArrays(clsMapping,
                        new object[] { "FactReport", Convert.ToDouble(row["C5"]) });
                    PumpRow(dsMonthRepArrearsBooks.Tables[0], mapping);
                }
            }
            WriteToTrace(string.Format("Обработка отчета {0} завершена", report.Name), TraceMessageKind.Information);
        }

        #endregion факт 2006 - 2007

        private void PumpReports(DirectoryInfo dir)
        {
            refDate = this.DataSource.Year.ToString() + this.DataSource.Month.ToString().PadLeft(2, '0') + "00";
            FileInfo[] filesList = null;
            if (this.DataSource.Year <= 2005)
            {
                block = Block.bRegions;
                if (!PumpPulseRegions(dir))
                    return;
                filesList = dir.GetFiles("O65001.dbf", SearchOption.AllDirectories);
                if (filesList.GetLength(0) != 0)
                    PumpO65001Report(filesList[0]);

                filesList = dir.GetFiles("O65101.dbf", SearchOption.AllDirectories);
                if (filesList.GetLength(0) != 0)
                    PumpO65101Report(filesList[0]);
            }
            else
            {
                filesList = dir.GetFiles("f128_d.dbf", SearchOption.AllDirectories);
                if (filesList.GetLength(0) != 0)
                    Pumpf128_dReport(filesList[0]);

                filesList = dir.GetFiles("f128_r.dbf", SearchOption.AllDirectories);
                if (filesList.GetLength(0) != 0)
                    Pumpf128_rReport(filesList[0]);

                filesList = dir.GetFiles("f128_i.dbf", SearchOption.AllDirectories);
                if (filesList.GetLength(0) != 0)
                    Pumpf128_iReport(filesList[0]);

                if (pulseReportPeriod == PulseReportPeriod.period2006)
                {
                    filesList = dir.GetFiles("f414.dbf", SearchOption.AllDirectories);
                    if (filesList.GetLength(0) != 0)
                        Pumpf414Report(filesList[0]);
                }

                filesList = dir.GetFiles("f387m.dbf", SearchOption.AllDirectories);
                if (filesList.GetLength(0) != 0)
                    Pumpf387mReport(filesList[0]);
            }
        }
        
        #endregion функции закачки фактов

        #region общая организация закачки

        private bool CheckPulseRegion()
        {
            return (this.Region == RegionName.Novosibirsk);
        }

        private PulseReportPeriod GetPulseReportPeriod()
        {
            if ((this.DataSource.Year <= 2004) || ((this.DataSource.Year == 2005) && (this.DataSource.Month <= 9)))
                return PulseReportPeriod.period2004;
            else if (this.DataSource.Year == 2005)
                return PulseReportPeriod.period2005;
            else if ((this.DataSource.Year == 2006) || ((this.DataSource.Year == 2007) && (this.DataSource.Month == 1)))
                return PulseReportPeriod.period2006;
            else
                return PulseReportPeriod.period2007;
        }

        private void ReconnectToDbfDataSource(ODBCDriverName driver, DirectoryInfo dir)
        {
            dbDataAccess.ConnectToDataSource(ref dbfDatabase, dir.FullName, driver);
        }

        private void SetDBFAnsiEncoding(string encodingName)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software\\microsoft\\jet\\4.0\\Engines\\Xbase", true);
            key.DeleteValue("DataCodePage", false);
            key.SetValue("DataCodePage", encodingName);
        }

        private void PumpPulseData(DirectoryInfo patternDir, DirectoryInfo reportDir)
        {
            // так как дбфки ебанутые - непонятный формат (вроде visual fox pro, но открыть не получилось)
            // приходится конвертить их в формат dbase III причем обязательно менять кодировку провайдера
            SetDBFAnsiEncoding("ANSI");
            try
            {
                if (patternDir != null)
                {
                    ReconnectToDbfDataSource(ODBCDriverName.Microsoft_dBase_Driver, patternDir);
                    PumpPatterns(patternDir);
                    UpdateData();
                }
                if (reportDir != null)
                {
                    ReconnectToDbfDataSource(ODBCDriverName.Microsoft_dBase_Driver, reportDir);
                    PumpReports(reportDir);
                    UpdateData();
                }
            }
            finally
            {
                SetDBFAnsiEncoding("OEM");
                if (dbfDatabase != null)
                {
                    dbfDatabase.Close();
                    dbfDatabase = null;
                }
            }
        }

        private DirectoryInfo GetTempDir(string fileName, ArchivatorName archName)
        {
            string tempDirPath = CommonRoutines.ExtractArchiveFileToTempDir(fileName, archName, FilesExtractingOption.SingleDirectory);
            return new DirectoryInfo(tempDirPath);
        }

        private void DeleteFiles(DirectoryInfo dir)
        {
            foreach (FileInfo file in dir.GetFiles("*.*", SearchOption.AllDirectories))
                file.Delete();
        }

        protected override void PumpPulseReports(DirectoryInfo dir)
        {
            if (!CheckPulseRegion())
                throw new PumpDataFailedException("Закачка месячной отчетности формата 'Пульс' предназначена только для Новосибирска.");
            pulseReportPeriod = GetPulseReportPeriod();
            sourceDir = dir;
            pulseWarnedRegions = new List<string>();
            try
            {
                if (this.DataSource.Year >= 2006)
                {
                    FileInfo[] archFiles = dir.GetFiles("*.zip", SearchOption.AllDirectories);
                    foreach (FileInfo archFile in archFiles)
                    {
                        WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation,
                            string.Format("Начало закачки архива {0}", archFile.Name));
                        DirectoryInfo tempDir = GetTempDir(archFile.FullName, ArchivatorName.Zip);
                        try
                        {
                            PumpPulseData(tempDir, tempDir);
                        }
                        finally
                        {
                            DeleteFiles(tempDir);
                        }
                        WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation,
                            string.Format("Завершение закачки архива {0}", archFile.Name));
                    }
                }
                else
                {
                    // блять из за смены кодировки драйвера он не может приконнектиться к директории с русским названием
                    // копируем файлы во временную папку и качаем оттуда
                    DirectoryInfo tempDir = CommonRoutines.GetTempDir();
                    foreach (FileInfo file in dir.GetFiles("*.dbf"))
                        file.CopyTo(string.Format("{0}\\{1}", tempDir.FullName, file.Name), true);
                    // качаем шаблоны
                    try
                    {
                        PumpPulseData(tempDir, null);
                    }
                    finally
                    {
                        DeleteFiles(tempDir);
                    }
                    // качаем отчеты
                    FileInfo[] archFiles = dir.GetFiles("*.arj", SearchOption.AllDirectories);
                    foreach (FileInfo archFile in archFiles)
                    {
                        WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation,
                            string.Format("Начало закачки архива {0}", archFile.Name));
                        tempDir = GetTempDir(archFile.FullName, ArchivatorName.Arj);
                        try
                        {
                            PumpPulseData(null, tempDir);
                        }
                        finally
                        {
                            DeleteFiles(tempDir);
                        }
                        WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation,
                            string.Format("Завершение закачки архива {0}", archFile.Name));
                    }
                }
            }
            finally
            {
                pulseWarnedRegions.Clear();
            }
        }
        
        #endregion общая организация закачки

    }

}
