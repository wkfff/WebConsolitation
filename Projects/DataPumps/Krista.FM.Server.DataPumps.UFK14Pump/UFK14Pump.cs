using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.UFK14Pump
{

    // УФК - 0014 - Выписка из сводного реестра поступлений и выбытий средств бюджета
    public partial class UFK14PumpModule : TextRepPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // КД.УФК (d_KD_UFK)
        private IDbDataAdapter daKd;
        private DataSet dsKd;
        private IClassifier clsKd;
        private Dictionary<string, int> cacheKd = null;
        private Dictionary<string, DataRow> cacheKdRow = null;
        // Организации.УФК_Плательщики (d_Org_UFKPayers)
        private IDbDataAdapter daOrg;
        private DataSet dsOrg;
        private IClassifier clsOrg;
        private Dictionary<string, int> cacheOrg = null;
        private Dictionary<string, int> cacheOrgName = null;
        // Администратор.УФК (d_KVSR_UFK)
        private IDbDataAdapter daAdmin;
        private DataSet dsAdmin;
        private IClassifier clsAdmin;
        private Dictionary<string, int> cacheAdmin = null;
        // ОКАТО.УФК (d_OKATO_UFK)
        private IDbDataAdapter daOkato;
        private DataSet dsOkato;
        private IClassifier clsOkato;
        private Dictionary<string, int> cacheOkato = null;
        // Период.Соответствие операционных дней (d_Date_ConversionFK)
        private IDbDataAdapter daPeriod;
        private DataSet dsPeriod;
        private IClassifier clsPeriod;
        private Dictionary<int, int> cachePeriod = null;
        private Dictionary<int, int> cachePeriodFo = null;
        // Организации.ЕГРЮЛ (b_Org_EGRUL)
        private IDbDataAdapter daEgrul;
        private DataSet dsEgrul;
        private IClassifier clsEgrul;
        private Dictionary<string, DataRow> cacheEgrul = null;
        private Dictionary<string, string> cacheEgrulName = null;
        // Организации.ОКВЭД (t_Org_OKVED)
        private IDbDataAdapter daOrgOkved;
        private DataSet dsOrgOkved;
        private IEntity clsOrgOkved;
        private Dictionary<int, string> cacheOrgOkved = null;
        // Индивидуальные предприниматели.ЕГРИП (b_IP_EGRIP)
        private IDbDataAdapter daEgrip;
        private DataSet dsEgrip;
        private IClassifier clsEgrip;
        private Dictionary<string, DataRow> cacheEgrip = null;
        private Dictionary<string, string> cacheEgripName = null;
        // Индивидуальные предприниматели.ОКВЭД (t_IP_OKVED)
        private IDbDataAdapter daIpOkved;
        private DataSet dsIpOkved;
        private IEntity clsIpOkved;
        private Dictionary<int, string> cacheIpOkved = null;
        // Организации.Плательщики_Сопоставимый (b_Org_PayersBridge)
        private IDbDataAdapter daOrgBridge;
        private DataSet dsOrgBridge;
        private IClassifier clsOrgBridge;
        private Dictionary<string, int> cacheOrgBridge = null;
        // Районы.Сопоставимый (b_Regions_Bridge)
        private IDbDataAdapter daRegionsBridge;
        private DataSet dsRegionsBridge;
        private IClassifier clsRegionsBridge;
        private Dictionary<int, DataRow> cacheRegionsBridge = null;
        // Фиксированный.Типы территорий (fx_FX_TerritorialPartitionType)
        private IDbDataAdapter daTerritoryType;
        private DataSet dsTerritoryType;
        private IClassifier clsTerritoryType;
        private Dictionary<int, string> cacheTerritoryType = null;
        // Организации.Перечисление по уровням бюджета (d_Org_BudgetTransfert)
        private IClassifier clsBudgetTransfert;
        // Организации.Неплательщики НП в ФБ (d_Org_NotPayer)
        private IClassifier clsNotPayer;

        #endregion Классификаторы

        #region Факты

        // Доходы.УФК_Выписка из сводного реестра_без расщепления (f_D_UFK14dirty)
        private IDbDataAdapter daFactUFK14Dirty;
        private DataSet dsFactUFK14Dirty;
        private IFactTable fctFactUFK14Dirty;
        // Доходы.УФК_Выписка из сводного реестра_c расщеплением (f_D_UFK14)
        private IDbDataAdapter daFactUFK14;
        private DataSet dsFactUFK14;
        private IFactTable fctFactUFK14;

        #endregion Факты

        private List<int> deletedDateList = null;
        private int year = -1;
        private int month = -1;
        private bool disintAll = false;
        private bool finalOverturn = false;
        private bool toBuildArchives = false;
        private bool toProcessOrgCls = false;
        private bool toDisintData = false;
        private bool toSetRefStrukt = false;
        private bool toDisintOutbankData = false;

        /// <summary>
        /// массив Id-шников записей первого уровня иерархии классификатора "Организации.Плательщики_Сопоставимый"
        /// </summary>
        private int[] orgBridgeParentIds;

        /// <summary>
        /// версия сопоставимого классификатора
        /// </summary>
        private int? bridgeClsSourceID = -1;

        #endregion Поля

        #region Закачка данных

        #region Работа с базой и кэшами

        // формируем кэш организаций: если по ключу несколько записей,
        // то оставляем только ту из них, у которой заполнено поле ParentId,
        // т.е. запись более низкого уровня
        private void FillOrgCache(string[] keyFields)
        {
            if (cacheOrg != null)
                cacheOrg.Clear();
            int count = dsOrg.Tables[0].Rows.Count;
            cacheOrg = new Dictionary<string, int>();
            for (int i = 0; i < count; i++)
            {
                DataRow row = dsOrg.Tables[0].Rows[i];
                if (row.RowState == DataRowState.Deleted)
                    continue;
                string key = GetComplexCacheKey(row, keyFields, "|");
                if (!cacheOrg.ContainsKey(key))
                {
                    cacheOrg.Add(key, Convert.ToInt32(row["ID"]));
                }
                else
                {
                    if (row["ParentId"] != DBNull.Value)
                        cacheOrg[key] = Convert.ToInt32(row["ID"]);
                }
            }
        }

        private void FillCaches()
        {
            FillRowsCache(ref cacheKd, dsKd.Tables[0], "CodeStr");
            FillOrgCache(new string[] { "INN", "KPP", "Name" });
            FillRowsCache(ref cacheAdmin, dsAdmin.Tables[0], new string[] { "CodeStr", "KPP", "Name" }, "|", "Id");
            FillRowsCache(ref cacheOkato, dsOkato.Tables[0], "Code");
            FillRowsCache(ref cachePeriod, dsPeriod.Tables[0], "RefFKDate", "RefFODate");
        }

        protected override void QueryData()
        {
            this.DB.UseBlob = true;
            if (this.Region == RegionName.MoskvaObl)
            {
                QueryDataMoskva();
                return;
            }
            InitClsDataSet(ref daKd, ref dsKd, clsKd, false, string.Empty);
            InitClsDataSet(ref daOrg, ref dsOrg, clsOrg, false, string.Empty);
            InitClsDataSet(ref daAdmin, ref dsAdmin, clsAdmin, false, string.Empty);
            InitClsDataSet(ref daOkato, ref dsOkato, clsOkato, false, string.Empty);
            InitDataSet(ref daPeriod, ref dsPeriod, clsPeriod, string.Empty);
            InitFactDataSet(ref daFactUFK14Dirty, ref dsFactUFK14Dirty, fctFactUFK14Dirty);
            InitFactDataSet(ref daFactUFK14, ref dsFactUFK14, fctFactUFK14);
            FillCaches();
        }

        protected override void UpdateData()
        {
            if (this.Region == RegionName.MoskvaObl)
            {
                UpdateDataMoskva();
                return;
            }
            UpdateDataSet(daKd, dsKd, clsKd);
            UpdateDataSet(daOrg, dsOrg, clsOrg);
            UpdateDataSet(daAdmin, dsAdmin, clsAdmin);
            UpdateDataSet(daOkato, dsOkato, clsOkato);
            UpdateDataSet(daFactUFK14Dirty, dsFactUFK14Dirty, fctFactUFK14Dirty);
            UpdateDataSet(daFactUFK14, dsFactUFK14, fctFactUFK14);
        }

        #region GUIDs

        private const string B_IP_EGRIP_GUID = "e7ac5579-1974-4a9f-8d83-80a8beace782";
        private const string B_ORG_EGRUL_GUID = "7473679b-3ebb-43ca-999d-7f8fdd3efb34";
        private const string D_DATE_CONVERSION_FK_GUID = "414c27e7-393c-4516-8b47-cf6df384569d";
        private const string D_KD_UFK_GUID = "b713e1df-5584-4e3d-a399-8828a2906971";
        private const string D_KVSR_UFK_GUID = "468224fd-24fa-447a-8001-66b0b3405a86";
        private const string D_OKATO_UFK_GUID = "4ae52664-ca7c-4994-bc5e-ba982421540e";
        private const string D_ORG_UFK_PAYERS_GUID = "5d7f6e1d-c202-49b3-b6ad-d584616aded0";
        private const string D_ORG_BUDGET_TRANSFERT_GUID = "b1eb3c76-713b-45c8-b32f-3890111a7f06";
        private const string D_ORG_NOT_PAYER_GUID = "7c92b17b-7626-40c2-bab7-b8269682d3cd";
        private const string F_D_UFK14_GUID = "81e7aaf2-8f99-448a-ac79-912b42b434d5";
        private const string F_D_UFK14_DIRTY_GUID = "4f3704ef-3970-4ef1-9eba-4c51dba4c395";
        private const string T_IP_OKVED_GUID = "01578c8a-fdb6-42fc-bd47-d805b6bf623a";
        private const string T_ORG_OKVED_GUID = "06e2c32b-395f-49e7-963a-c3edbdb9a194";
        private const string B_ORG_PAYERS_BRIDGE_GUID = "fe3675fe-8dec-4c1d-b821-d28eea3352c6";
        private const string B_REGIONS_BRIDGE_GUID = "0906ba3d-3d9a-4c6f-b3a1-f45dbe84a04a";
        private const string FX_TERRITORIAL_PARTITION_TYPE_GUID = "8afb5bc2-2df5-4ec4-9f3d-fa2d8e72f90b";

        #endregion GUIDs
        protected override void InitDBObjects()
        {
            if (this.Region == RegionName.MoskvaObl)
            {
                clsBudgetTransfert = this.Scheme.Classifiers[D_ORG_BUDGET_TRANSFERT_GUID];
                clsNotPayer = this.Scheme.Classifiers[D_ORG_NOT_PAYER_GUID];
            }
            clsPeriod = this.Scheme.Classifiers[D_DATE_CONVERSION_FK_GUID];
            clsEgrul = this.Scheme.Classifiers[B_ORG_EGRUL_GUID];
            foreach (IEntityAssociation association in clsEgrul.Associated.Values)
            {
                if (association.AssociationClassType != AssociationClassTypes.MasterDetail)
                    continue;
                if (association.RoleData.ObjectKey == T_ORG_OKVED_GUID)
                    clsOrgOkved = association.RoleData;
            }
            clsEgrip = this.Scheme.Classifiers[B_IP_EGRIP_GUID];
            foreach (IEntityAssociation association in clsEgrip.Associated.Values)
            {
                if (association.AssociationClassType != AssociationClassTypes.MasterDetail)
                    continue;
                if (association.RoleData.ObjectKey == T_IP_OKVED_GUID)
                    clsIpOkved = association.RoleData;
            }
            clsOrgBridge = this.Scheme.Classifiers[B_ORG_PAYERS_BRIDGE_GUID];
            clsRegionsBridge = this.Scheme.Classifiers[B_REGIONS_BRIDGE_GUID];
            clsTerritoryType = this.Scheme.Classifiers[FX_TERRITORIAL_PARTITION_TYPE_GUID];
            this.UsedClassifiers = new IClassifier[] {
                clsKd = this.Scheme.Classifiers[D_KD_UFK_GUID],
                clsOrg = this.Scheme.Classifiers[D_ORG_UFK_PAYERS_GUID],
                clsAdmin = this.Scheme.Classifiers[D_KVSR_UFK_GUID],
                clsOkato = this.Scheme.Classifiers[D_OKATO_UFK_GUID] };
            this.UsedFacts = new IFactTable[] {
                fctFactUFK14Dirty = this.Scheme.FactTables[F_D_UFK14_DIRTY_GUID], 
                fctFactUFK14 = this.Scheme.FactTables[F_D_UFK14_GUID] };
        }

        protected override void PumpFinalizing()
        {
            if (this.Region == RegionName.MoskvaObl)
            {
                PumpFinalizingMoskva();
                return;
            }
            ClearDataSet(ref dsKd);
            ClearDataSet(ref dsOrg);
            ClearDataSet(ref dsAdmin);
            ClearDataSet(ref dsOkato);
            ClearDataSet(ref dsPeriod);
            ClearDataSet(ref dsFactUFK14Dirty);
            ClearDataSet(ref dsFactUFK14);
        }

        #endregion Работа с базой и кэшами

        #region Общие методы

        private int PumpKd(string code)
        {
            code = code.Trim();
            object[] mapping = new object[] { "CodeStr", code };
            return PumpCachedRow(cacheKd, dsKd.Tables[0], clsKd, code, mapping);
        }

        private int PumpOrg(string innStr, string kppStr, string name)
        {
            long inn = GetCorrectLongValue(innStr, 0, "ИНН плательщика");
            long kpp = GetCorrectLongValue(kppStr, 0, "КПП плательщика");
            name = name.Trim();
            if (name == string.Empty)
                name = constDefaultClsName;
            string key = string.Format("{0}|{1}|{2}", inn, kpp, name);
            object[] mapping = new object[] { "INN", inn, "KPP", kpp, "Name", name };
            int refOrg = PumpCachedRow(cacheOrg, dsOrg.Tables[0], clsOrg, key, mapping);
            if (dsOrg.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateDataSet(daOrg, dsOrg, clsOrg);
                ClearDataSet(daOrg, dsOrg.Tables[0]);
            }
            return refOrg;
        }

        private int PumpAdmin(string innStr, string kppStr, string name)
        {
            long inn = GetCorrectLongValue(innStr, 0, "ИНН администратора");
            long kpp = GetCorrectLongValue(kppStr, 0, "КПП администратора");
            name = name.Trim();
            if (name == string.Empty)
                name = constDefaultClsName;
            string key = string.Format("{0}|{1}|{2}", inn, kpp, name);
            object[] mapping = new object[] { "CodeStr", inn, "KPP", kpp, "Name", name };
            return PumpCachedRow(cacheAdmin, dsAdmin.Tables[0], clsAdmin, key, mapping);
        }

        private int PumpOkato(string codeStr)
        {
            long code = CleanLongValue(codeStr);
            object[] mapping = new object[] { "Code", code };
            return PumpCachedRow(cacheOkato, dsOkato.Tables[0], clsOkato, code.ToString(), mapping);
        }

        private long GetCorrectLongValue(string longValue, long defaultValue, string field)
        {
            try
            {
                return CleanLongValue(longValue);
            }
            catch
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                    "Некорректное значение '{0}' = {1}. Полю '{0}' будет присвоено значение {2}.",
                    field, longValue, defaultValue));
                return defaultValue;
            }
        }

        private long CleanLongValue(string longValue)
        {
            longValue = CommonRoutines.TrimLetters(longValue.Trim()).Trim();
            return Convert.ToInt64(longValue.PadLeft(1, '0'));
        }

        private decimal CleanFactValue(string factValue)
        {
            factValue = factValue.Replace(" ", string.Empty).Replace(".", ",").Trim();
            return Convert.ToDecimal(factValue.PadLeft(1, '0'));
        }

        private int GetFoDate(int fkDate)
        {
            return FindCachedRow(cachePeriod, fkDate, fkDate);
        }

        private int GetCodePage()
        {
            if (this.Region == RegionName.Omsk)
                return CommonRoutines.GetTxtWinCodePage();
            else
                return CommonRoutines.GetTxtDosCodePage();
        }

        private void DeleteEarlierDataByDate(int refDate)
        {
            if (!deletedDateList.Contains(refDate))
            {
                if (!finalOverturn)
                    DeleteData(string.Format("RefFKDay = {0}", refDate), string.Format("Дата отчета: {0}.", refDate));
                deletedDateList.Add(refDate);
            }
        }

        #endregion Общие методы

        #region Работа с Txt

        #region закачка орска

        private const string TABLE_TITLE1 = "1";
        private const string TABLE_TITLE2 = "2";
        private bool IsTableTitle(string row)
        {
            return ((row.Split(DELIMETER)[1].Trim() == TABLE_TITLE1) && (row.Split(DELIMETER)[2].Trim() == TABLE_TITLE2));
        }

        private const string TOTAL_NAME = "ИТОГО";
        private bool IsTotalSum(string row)
        {
            return (row.ToUpper().StartsWith(TOTAL_NAME));
        }

        // код окончания строки таблицы "|-"
        private const int ROW_END_MARK_CODE = 9500;
        private char ROW_END_MARK = Convert.ToChar(ROW_END_MARK_CODE);
        private bool IsRowEnd(string row)
        {
            if (row.Trim() == string.Empty)
                return false;
            return (row[1] == ROW_END_MARK);
        }

        // код окончания таблицы "|_"
        private const int TABLE_END_MARK_CODE = 9492;
        private char TABLE_END_MARK = Convert.ToChar(TABLE_END_MARK_CODE);
        private bool IsTableEnd(string row)
        {
            if (row.Trim() == string.Empty)
                return false;
            return (row[1] == TABLE_END_MARK);
        }

        // индексы нужных колонок отчета при закачке
        // значение маппинга: 0 - Орг ИНН, 1 - Орг КПП, 2 - Орг Имя, 3 - Админ ИНН, 4 - Админ КПП, 5 - ОКАТО код,
        // 6 - КД код, 7 - назначение платежа, 8 - кредит
        private int[] PUMP_COLUMN_INDICES = new int[] { 3, 4, 5, 6, 7, 8, 9, 10, 12 };
        // индексы нужных колонок отчета при проверке
        // значение маппинга: 0 - КБК, 1 - Сумма
        private int[] CHECK_COLUMN_INDICES = new int[] { 1, 2 };
        private void GetColumnValues(string row, ref object[] mapping, int[] columnIndices)
        {
            if (mapping == null)
                mapping = new object[] { "", "", "", "", "", "", "", "", "" };
            string[] columnValues = row.Split(DELIMETER);
            for (int i = 0; i <= columnIndices.GetLength(0) - 1; i++)
            {
                string columnValue = columnValues[columnIndices[i]].Trim();
                if (columnValue != string.Empty)
                {
                    if (mapping[i].ToString() != string.Empty)
                        mapping[i] = string.Concat(mapping[i].ToString(), " ");
                    mapping[i] += columnValue;
                }
            }
        }

        private void PumpTxtRowOrsk(object[] mapping, int refDate)
        {
            if (refDate < 20080118)
            {
                string inn = CommonRoutines.TrimLetters(mapping[0].ToString().TrimStart('0').PadLeft(1, '0'));
                string kpp = CommonRoutines.TrimLetters(mapping[1].ToString().TrimStart('0').PadLeft(1, '0'));
                if (inn.Length == 12 || (inn == "0" && kpp == "0"))
                    return;
            }

            int refOrg = PumpOrg(mapping[0].ToString(), mapping[1].ToString(), mapping[2].ToString());
            int refAdmin = PumpAdmin(mapping[3].ToString(), mapping[4].ToString(), mapping[3].ToString());
            int refOKATO = PumpOkato(mapping[5].ToString());
            int refKD = PumpKd(mapping[6].ToString());

            object[] factMapping = new object[] {
                "RefOrg", refOrg, "RefKVSR", refAdmin, "RefOKATO", refOKATO, "RefKD", refKD, "ElectrNomer", 0,
                "NaznPlat", mapping[7], "RefFX", 0, "RefFKDay", refDate, "RefYearDayUNV", GetFoDate(refDate), 
                "Debit", 0, "Credit", mapping[8].ToString().Replace(" ", "") };

            PumpRow(dsFactUFK14Dirty.Tables[0], factMapping);
            if (dsFactUFK14Dirty.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daFactUFK14Dirty, ref dsFactUFK14Dirty);
            }
        }

        // код разделителя "|"
        private const int DELIMETER_CODE = 9474;
        private char DELIMETER = Convert.ToChar(DELIMETER_CODE);
        private bool RowContainsDelimeter(string row)
        {
            return row.Contains(DELIMETER.ToString());
        }

        private const string REPORT_END_MARK_ORSK = "ВСЕГО";
        private bool IsReportEndOrsk(string row)
        {
            return (row.ToUpper().Contains(REPORT_END_MARK_ORSK));
        }

        private void PumpTxtFileOrsk(FileInfo file)
        {
            string[] reportData = CommonRoutines.GetTxtReportData(file, GetCodePage());
            int refDate = Convert.ToInt32(CommonRoutines.LongDateToNewDate(reportData[6].Trim()));
            DeleteEarlierDataByDate(refDate);
            bool toGetColumnsValues = false;
            object[] mapping = null;
            int rowIndex = 0;
            foreach (string row in reportData)
                try
                {
                    rowIndex++;
                    if (IsReportEndOrsk(row.Split(DELIMETER)[0]))
                        return;
                    if ((IsRowEnd(row)) || (IsTableEnd(row)))
                    {
                        if (mapping != null)
                        {
                            PumpTxtRowOrsk(mapping, refDate);
                            mapping = null;
                        }
                        continue;
                    }
                    if (!RowContainsDelimeter(row))
                        continue;
                    // пропускаем строки, не содержащие данных (иногда запись "итого" - на нескольких строчках) - разделителей должно быть больше трех
                    if (row.Split(DELIMETER).GetLength(0) <= 4)
                        continue;
                    if (IsTotalSum(row.Split(DELIMETER)[0].Trim()))
                        continue;
                    if (IsTableTitle(row))
                    {
                        toGetColumnsValues = true;
                        continue;
                    }
                    if (toGetColumnsValues)
                        GetColumnValues(row, ref mapping, PUMP_COLUMN_INDICES);
                }
                catch (Exception exp)
                {
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                        string.Format("Ошибка при обработке строки {0} отчета {1}: {2}", rowIndex, file.Name, exp.Message));
                    this.DataSourceProcessingResult = DataSourceProcessingResult.ProcessedWithErrors;
                }
        }

        #endregion закачка орска

        #region закачка омска

        private const int REPORT_END_MARK_CODE_TXT_OMSK = 25;
        private char REPORT_END_MARK_TXT_OMSK = Convert.ToChar(REPORT_END_MARK_CODE_TXT_OMSK);
        private bool IsReportEndOmsk(string row)
        {
            return (row.Contains(REPORT_END_MARK_TXT_OMSK.ToString()));
        }

        private void PumpTxtRowOmsk(string[] columnsValues, int refDate)
        {
            int refOrg = PumpOrg(columnsValues[2], columnsValues[3], columnsValues[1]);
            if (refOrg == -1)
                return;
            int refAdmin = PumpAdmin(columnsValues[columnsValues.GetLength(0) - 3], "0", columnsValues[columnsValues.GetLength(0) - 2]);
            int refOKATO = PumpOkato(columnsValues[4]);
            int refKD = PumpKd(columnsValues[8]);
            // Доходы.УФК_Выписка из сводного реестра_без расщепления
            decimal credit = CleanFactValue(columnsValues[7]);
            if (credit == 0)
                return;
            string docNomer = columnsValues[5].Trim();
            string dateDoc = columnsValues[6].Trim().Insert(2, ".").Insert(5, ".");
            // встретилась запись с назначением платежа "...|....|..." - палочки в нем не являются разделителями
            string naznPlat = string.Empty;
            for (int i = 9; i < columnsValues.GetLength(0) - 3; i++)
                naznPlat += string.Concat(columnsValues[i].Trim(), "|");
            naznPlat = naznPlat.Remove(naznPlat.Length - 1);
            object[] factMapping = new object[] {
                "RefOrg", refOrg, "RefKVSR", refAdmin, "RefOKATO", refOKATO, "RefKD", refKD, 
                "NaznPlat", naznPlat, "RefFX", 0, "RefFKDay", refDate, "RefYearDayUNV", GetFoDate(refDate),
                "Debit", 0, "Credit", credit, "ElectrNomer", 0, "Nomer", docNomer, "DateDoc", dateDoc };
            PumpRow(dsFactUFK14Dirty.Tables[0], factMapping);
            if (dsFactUFK14Dirty.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daFactUFK14Dirty, ref dsFactUFK14Dirty);
            }
        }

        private const char WIN_DELIMETER = '|';
        private void PumpTxtFileOmsk(FileInfo file)
        {
            int refDate = CommonRoutines.ShortDateToNewDate(file.Name.Split('_')[1].Split('.')[0]);
            DeleteEarlierDataByDate(refDate);
            string[] reportData = CommonRoutines.GetTxtReportData(file, GetCodePage());
            int rowIndex = 0;
            foreach (string row in reportData)
                try
                {
                    rowIndex++;
                    if (IsReportEndOmsk(row))
                        return;
                    string[] columnsValue = row.Split(WIN_DELIMETER);
                    PumpTxtRowOmsk(columnsValue, refDate);
                }
                catch (Exception exp)
                {
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                        string.Format("Ошибка при обработке строки {0} отчета {1}: {2}", rowIndex, file.Name, exp.Message));
                    this.DataSourceProcessingResult = DataSourceProcessingResult.ProcessedWithErrors;
                }
        }

        #endregion закачка омска

        private const string OMSK_FILE_MASK = "ppmf_*.txt";
        private void ProcessTxtFilesOmsk(DirectoryInfo dir)
        {
            ProcessFilesTemplate(dir, OMSK_FILE_MASK, new ProcessFileDelegate(PumpTxtFileOmsk), false);
            
            FileInfo[] archFiles = dir.GetFiles("*.rar", SearchOption.AllDirectories);
            foreach (FileInfo archFile in archFiles)
            {
                DirectoryInfo tempDir = CommonRoutines.ExtractArchiveFileToTempDir(archFile.FullName,
                    FilesExtractingOption.SingleDirectory, ArchivatorName.Rar);
                try
                {
                    ProcessFilesTemplate(tempDir, OMSK_FILE_MASK, new ProcessFileDelegate(PumpTxtFileOmsk), false);
                }
                finally
                {
                    CommonRoutines.DeleteDirectory(tempDir);
                }
            }
        }

        private const string ORSK_FILE_MASK = "v*000.txt";
        private void PumpTxtFiles(DirectoryInfo dir)
        {
            if (this.Region == RegionName.Omsk)
                ProcessTxtFilesOmsk(dir);
            else
                ProcessFilesTemplate(dir, ORSK_FILE_MASK, new ProcessFileDelegate(PumpTxtFileOrsk), false);
        }

        #endregion Работа с Txt

        #region Работа с Xls

        #region Массивы пар "поле-столбец"

        #region для всех регионов (по умолчанию)

        private object[] XLS_MAPPING_DEFAULT = new object[] {
            "Cell", 1, "Debit", 12, "Credit", 13, "Number", 3, "Date", 2,
            "NaznPlat", 11, "CodeKd", 10, "InnOrg", 4, "KppOrg", 5, "NameOrg", 6,
            "InnAdmin", 7, "KppAdmin", 8, "CodeOkato", 9 };

        #endregion для всех регионов (по умолчанию)

        #region для Калмыкии

        // с 04.02.2008
        private object[] XLS_MAPPING_KALMYKYA_2008 = new object[] {
            "Cell", 1, "Debit", 13, "Credit", 14, "Number", 3, "Date", 2,
            "NaznPlat", 12, "CodeKd", 10, "InnOrg", 4, "KppOrg", 5, "NameOrg", 6,
            "InnAdmin", 7, "KppAdmin", 8, "CodeOkato", 9 };

        // с 01.01.2009
        private object[] XLS_MAPPING_KALMYKYA_2009 = new object[] {
            "Cell", 1, "Debit", 14, "Credit", 13, "Number", 3, "Date", 2,
            "NaznPlat", 12, "CodeKd", 11, "InnOrg", 4, "KppOrg", 5, "NameOrg", 6,
            "InnAdmin", 7, "KppAdmin", 8, "CodeOkato", 9 };
        
        // с 01.01.2012 
        private object[] XLS_MAPPING_KALMYKYA_2012 = new object[] {
            "Cell", 1, "Debit", 37, "Credit", 33, "Number", 5, "CodeGoal", 23,
            "CodeKd", 28, "InnOrg", 8, "KppOrg", 11,
            "InnAdmin", 14, "KppAdmin", 17, "CodeOkato", 20, "NameDoc", 1};

        #endregion для Калмыкии

        #region для Москвы

        // до 01.11.2009 (поле Date не заполняется, ElectrNomer заполняется нулём)
        private object[] XLS_MAPPING_MOSKVA_2009 = new object[] {
            "Cell", 1, "Debit", 14, "Credit", 13, "Number", 3, "NaznPlat", 12,
            "CodeGoal", 10, "CodeKd", 11, "InnOrg", 4, "KppOrg", 5, "NameOrg", 6,
            "InnAdmin", 7, "KppAdmin", 8, "CodeOkato", 9, "NameDoc", 1 };

        // с 01.12.2009 (ElectrNomer заполняется нулём)
        private object[] XLS_MAPPING_MOSKVA_2010 = new object[] {
            "Cell", 1, "Debit", 14, "Credit", 13, "Number", 3, "Date", 2,
            "NaznPlat", 12, "CodeGoal", 10, "CodeKd", 11, "InnOrg", 4, "KppOrg", 5,
            "NameOrg", 6, "InnAdmin", 7, "KppAdmin", 8, "CodeOkato", 9, "NameDoc", 1 };

        #endregion для Москвы

        #region для Новосибирска

        // до 05.08.2005 (поля Date и NaznPlan не заполняются)
        private object[] XLS_MAPPING_NOVOSIB_2005 = new object[] {
            "Cell", 1, "Debit", 12, "Credit", 13, "Number", 2, "Date", 2,
            "NaznPlat", 9, "CodeKd", 9, "InnOrg", 3, "KppOrg", 4, "NameOrg", 5,
            "InnAdmin", 6, "KppAdmin", 7, "CodeOkato", 8 };

        // с 15.01.2008
        private object[] XLS_MAPPING_NOVOSIB_2008 = new object[] {
            "Cell", 1, "Debit", 13, "Credit", 14, "Number", 3, "Date", 2,
            "NaznPlat", 12, "CodeKd", 10, "InnOrg", 4, "KppOrg", 5, "NameOrg", 6,
            "InnAdmin", 7, "KppAdmin", 8, "CodeOkato", 9 };

        #endregion для Новосибирска

        #region для Орска

        // до 01.01.2009 (поле Debit заполняется 0)
        private object[] XLS_MAPPING_ORSK_2008 = new object[] {
            "Cell", 1, "Debit", 14, "Credit", 14, "Number", 3, "Date", 2,
            "NaznPlat", 12, "CodeKd", 10, "InnOrg", 4, "KppOrg", 5, "NameOrg", 6,
            "InnAdmin", 7, "KppAdmin", 8, "CodeOkato", 9 };

        // c 01.01.2009 (поле Debit заполняется 0)
        private object[] XLS_MAPPING_ORSK_2009 = new object[] {
            "Cell", 1, "Debit", 13, "Credit", 13, "Number", 3, "Date", 2,
            "NaznPlat", 12, "CodeKd", 11, "InnOrg", 4, "KppOrg", 5, "NameOrg", 6,
            "InnAdmin", 7, "KppAdmin", 8, "CodeOkato", 9 };

        #endregion для Орска

        private object[] GetMapping(int dateMark)
        {
            if (this.Region == RegionName.Kalmykya)
            {
                if (dateMark >= 20120101)
                    return XLS_MAPPING_KALMYKYA_2012;
                if (dateMark >= 20090101)
                    return XLS_MAPPING_KALMYKYA_2009;
                if (dateMark >= 20080204)
                    return XLS_MAPPING_KALMYKYA_2008;
            }
            else if (this.Region == RegionName.MoskvaObl)
            {
                if (dateMark <= 20091200)
                    return XLS_MAPPING_MOSKVA_2009;
                return XLS_MAPPING_MOSKVA_2010;
            }
            else if (this.Region == RegionName.Novosibirsk)
            {
                if (dateMark < 20050805)
                    return XLS_MAPPING_NOVOSIB_2005;
                if (dateMark >= 20080115)
                    return XLS_MAPPING_NOVOSIB_2008;
            }
            else if (this.Region == RegionName.Orsk)
            {
                if (dateMark >= 20090101)
                    return XLS_MAPPING_ORSK_2009;
                return XLS_MAPPING_ORSK_2008;
            }
            return XLS_MAPPING_DEFAULT;
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

        private DataTable GetXlsDataTable(ExcelHelper excelDoc, object[] mapping, int firstRow, int lastRow)
        {
            DataTable dt = new DataTable();

            dt.BeginInit();
            int count = mapping.GetLength(0);
            for (int i = 0; i < count; i += 2)
            {
                dt.Columns.Add(Convert.ToString(mapping[i])); 
            }
            dt.EndInit();

            dt.BeginLoadData();
            for (int curRow = firstRow; curRow <= lastRow; curRow++)
            {
                DataRow row = dt.NewRow();
                for (int i = 0; i < count; i += 2)
                {
                    row[i / 2] = excelDoc.GetValue(curRow, Convert.ToInt32(mapping[i + 1]));
                }
                dt.Rows.Add(row);
            }
            dt.EndLoadData();

            return dt;
        }

        #endregion Массивы пар "поле-столбец"

        private string GetXlsDocDate(DataRow row, int dateMark)
        {
            if ((this.Region == RegionName.Novosibirsk) && (dateMark < 20071112))
                return null;
            string docDate = row["Date"].ToString().Trim();
            if (docDate == string.Empty)
                return null;
            return docDate;
        }

        private string GetXlsNaznPlan(DataRow row, int dateMark)
        {
            if ((this.Region == RegionName.Novosibirsk) && (dateMark < 20071112))
                return "Неуказанное назначение платежа";
            string naznPlat = row["NaznPlat"].ToString().Trim();
            if (naznPlat == string.Empty)
                return constDefaultClsName;
            return naznPlat;
        }

        private void PumpXlsRow(DataRow row, int refDate, int dateMark)
        {
            int refKd = PumpKd(row["CodeKd"].ToString());
            int refOrg = PumpOrg(row["InnOrg"].ToString(), row["KppOrg"].ToString(), (dateMark < 20120101) ? row["NameOrg"].ToString() : "");
            int refAdmin = PumpAdmin(row["InnAdmin"].ToString(), row["KppAdmin"].ToString(), row["InnAdmin"].ToString());
            int refOkato = PumpOkato(row["CodeOkato"].ToString());

            decimal credit = CleanFactValue(row["Credit"].ToString());
            decimal debit = 0;
            if (this.Region != RegionName.Orsk)
                debit = CleanFactValue(row["Debit"].ToString());
            if ((credit == 0) && (debit == 0))
                return;
            
            object[] mapping = new object[] {
                "Credit", credit,
                "Debit", debit,
                "ElectrNomer", 0,
                "Nomer", row["Number"].ToString().Trim().PadLeft(1, '0'),
                "RefKD", refKd,
                "RefOrg", refOrg,
                "RefKVSR", refAdmin,
                "RefOKATO", refOkato,
                "RefFX", 0,
                "RefFKDay", refDate,
                "RefYearDayUNV", GetFoDate(refDate)
            };

            if ((dateMark >= 20120101) && (this.Region == RegionName.Kalmykya))//изменения с 2012 года
            {
                mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "NumberRegistry", registryNumber, "NameDoc", row["NameDoc"].ToString()});
                string codeGoal = row["CodeGoal"].ToString().Trim();
                if (!(codeGoal == string.Empty))
                    mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "CodeGoal", Convert.ToInt32(codeGoal)});
            }
            else mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "NaznPlat", GetXlsNaznPlan(row, dateMark), "DateDoc", GetXlsDocDate(row, dateMark) });

            PumpRow(dsFactUFK14Dirty.Tables[0], mapping);
            if (dsFactUFK14Dirty.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daFactUFK14Dirty, ref dsFactUFK14Dirty);
            }
        }

        private const string XLS_REPORT_END_MARK = "ВСЕГО";
        private const string XLS_REPORT_END_MARK2 = "РУКОВОДИТ";
        private bool IsXlsReportEnd(string cellValue)
        {
            cellValue = cellValue.ToUpper();
            return (cellValue.Equals(XLS_REPORT_END_MARK) || cellValue.StartsWith(XLS_REPORT_END_MARK2));
        }

        // в орске не качаем данные по физическим лицам
        private bool IsXlsUnpumpRow(DataRow row, int refDate)
        {
            if (this.Region != RegionName.Orsk)
                return false;
            if (refDate < 20080118)
                return false;
            string inn = row["InnOrg"].ToString().TrimStart('0').PadLeft(1, '0');
            long kpp = CleanLongValue(row["KppOrg"].ToString());
            return (inn.Length == 12 || (inn == "0" && kpp == 0));
        }

        private bool SetRefDate(string dateStr, ref int dateMark, ref int refDate)
        {
            dateStr = dateStr.Trim();
            if (this.DataSource.Year < 2012)
                dateMark = Convert.ToInt32(CommonRoutines.LongDateToNewDate(dateStr.Trim()));
            else
            {
                // формат ДД.ММ.ГГГГ чч:мм:сс
                if (dateStr.Contains(" "))
                    dateStr = dateStr.Substring(0, dateStr.IndexOf(' '));
                dateMark = Convert.ToInt32(CommonRoutines.ShortDateToNewDate(dateStr.Trim()));
            }
            refDate = dateMark;
            if (finalOverturn)
                refDate = (this.DataSource.Year * 10000) + 1232;
            if (!CheckDataSourceByDate(refDate, false))
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                    string.Format("дата {0} не соответствует параметрам источника", refDate));
                return false;
            }
            DeleteEarlierDataByDate(refDate);
            return true;
        }

        private int GetXlsFirstRow(ExcelHelper excelDoc)
        {
            int rowsCount = excelDoc.GetRowsCount();
            int curRow = 1;
            //в сводных с 2012го в 1й строке записан порядковый номер столбца. поэтому со 2й строки
            if (this.DataSource.Year >= 2012)
                curRow = 2;

            for (; curRow <= rowsCount; curRow++)
            {
                string str = excelDoc.GetValue(curRow, 1).Trim();
                if (excelDoc.GetValue(curRow, 1).Trim() == "1")
                    return (curRow + 1);
            }
            return 1;
        }

        private string GetRefDateString(ExcelHelper excelDoc)
        {
            if (this.DataSource.Year >= 2012)
                return excelDoc.GetValue("AK4").Trim();
            else return excelDoc.GetValue("A3").Trim();
        }

        private string GetRegistryNumber(ExcelHelper excelDoc)
        {
            if ((this.DataSource.Year >= 2012) && (this.Region == RegionName.Kalmykya))
            {
                return excelDoc.GetValue("W2").Trim();
            }
            return string.Empty;
        }

        private void PumpXlsSheetData(ExcelHelper excelDoc, string fileName)
        {
            // на эту дату ориентироваться при определении формата
            int dateMark = -1;
            int refDate = -1;
            if (!SetRefDate(GetRefDateString(excelDoc), ref dateMark, ref refDate))
                return;

            int firstRow = GetXlsFirstRow(excelDoc);
            int lastRow = excelDoc.GetRowsCount();
            registryNumber = GetRegistryNumber(excelDoc);
            object[] mapping = GetMapping(dateMark);
            DataTable dt = GetXlsDataTable(excelDoc, mapping, firstRow, lastRow);
            int rowsCount = dt.Rows.Count;
            for (int curRow = 0; curRow < rowsCount; curRow++)
                try
                {
                    SetProgress(lastRow, curRow,
                        string.Format("Обработка файла {0}...", fileName),
                        string.Format("Строка {0} из {1}", curRow + firstRow, lastRow));

                    DataRow row = dt.Rows[curRow];
                    string cellValue = row["Cell"].ToString().Trim();

                    if (IsXlsReportEnd(cellValue) || IsXlsReportEnd(row["CodeKd"].ToString().Trim()))
                        return;

                    if (IsTotalSum(cellValue) || (cellValue == string.Empty))
                        continue;

                    if (IsXlsUnpumpRow(row, refDate))
                        continue;

                    PumpXlsRow(row, refDate, dateMark);
                }
                catch (Exception ex)
                {
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError, string.Format(
                        "При обработке строки {0} листа {1} отчета {2} возникла ошибка ({3})",
                        curRow + firstRow, excelDoc.GetWorksheetName(), fileName, ex.Message));
                    this.DataSourceProcessingResult = DataSourceProcessingResult.ProcessedWithErrors;
                }
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
                excelDoc.SetWorksheet(1);
                PumpXlsSheetData(excelDoc, file.Name);
            }
            finally
            {
                if (excelDoc != null)
                    excelDoc.CloseDocument();
            }
        }

        #region Калмыкия

        private void PumpKalmykiaXlsFiles(DirectoryInfo dir)
        {
            // закачиваем xls файлы из папки источника
            ProcessFilesTemplate(dir, "*.xls", new ProcessFileDelegate(PumpXlsFile), false);
            // если в папке источника есть архивы, распаковываем их во временную директорию и качаем
            DirectoryInfo tempDir = null;
            FileInfo[] archFiles = dir.GetFiles("*.rar", SearchOption.AllDirectories);
            foreach (FileInfo archFile in archFiles)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation,
                    string.Format("Начало закачки архива {0}", archFile.Name));
                tempDir = CommonRoutines.ExtractArchiveFileToTempDir(archFile.FullName,
                    FilesExtractingOption.SingleDirectory, ArchivatorName.Rar);
                try
                {
                    ProcessFilesTemplate(tempDir, "*.xls", new ProcessFileDelegate(PumpXlsFile), false);
                }
                finally
                {
                    CommonRoutines.DeleteDirectory(tempDir);
                }
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation,
                    string.Format("Завершение закачки архива {0}", archFile.Name));
            }
        }

        #endregion Калмыкия

        #region Новосибирск

        private void PumpNovosibArchFile(FileInfo acrhFile, string fileMask, bool containsSubArch)
        {
            DirectoryInfo tempDir = CommonRoutines.GetTempDir();
            try
            {
                string output = string.Empty;
                CommonRoutines.ExtractRar(acrhFile.FullName, tempDir.FullName, out output, fileMask);
                ProcessFilesTemplate(tempDir, "*.xls", new ProcessFileDelegate(PumpXlsFile), false);
                // качаем из архива okato.rar
                if (containsSubArch)
                    foreach (FileInfo okatoArch in tempDir.GetFiles("okato*.rar"))
                        PumpNovosibArchFile(okatoArch, string.Empty, false);
            }
            finally
            {
                CommonRoutines.DeleteDirectory(tempDir);
            }
        }

        private void PumpNovosibRarFiles(DirectoryInfo dir)
        {
            foreach (FileInfo archFile in dir.GetFiles("*.rar", SearchOption.AllDirectories))
            {
                string archName = archFile.Name.ToUpper();
                // не закачиваем архив RММДД_3.rar
                if (archName.EndsWith("_3.RAR"))
                    continue;
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation,
                    string.Format("Начало закачки архива {0}", archFile.Name));
                // качаем архив R_ММ_ДД.rar
                if (archName.Split('_').GetLength(0) >= 3)
                    PumpNovosibArchFile(archFile, "\"5406300195\\*\" \"okato*.rar\"", true);
                // качаем архив RММДД_1.rar
                else if (archName.EndsWith("_1.RAR"))
                    PumpNovosibArchFile(archFile, "\"R0903_1\\5406300195\\*\"", false);
                // качаем архив RММДД_2.rar
                else if (archName.EndsWith("_2.RAR"))
                    PumpNovosibArchFile(archFile, "\"R0903_2\\SR*\\*\"", false);
                // качаем архив RММДД.rar
                else
                    PumpNovosibArchFile(archFile, "\"5406300195\\*\" \"SR*\\*\"", false);
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation,
                    string.Format("Завершение закачки архива {0}", archFile.Name));
            }
        }

        private void PumpNovosibArjFiles(DirectoryInfo dir)
        {
            FileInfo[] archFiles = dir.GetFiles("*.arj", SearchOption.AllDirectories);
            foreach (FileInfo archFile in archFiles)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation,
                    string.Format("Начало закачки архива {0}", archFile.Name));
                string archName = archFile.Name.ToUpper();
                // качаем архив RММДД.arj
                DirectoryInfo tempDir = CommonRoutines.ExtractArchiveFileToTempDir(archFile.FullName,
                    FilesExtractingOption.SingleDirectory, ArchivatorName.Arj);
                try
                {
                    // закачиваем из папки 5406300195 и папок SR*
                    DirectoryInfo[] dirs = tempDir.GetDirectories("5406300195");
                    if (dirs.GetLength(0) != 0)
                        ProcessFilesTemplate(dirs[0], "*.xls", new ProcessFileDelegate(PumpXlsFile), false);
                    dirs = tempDir.GetDirectories("SR*");
                    foreach (DirectoryInfo subDir in dirs)
                        ProcessFilesTemplate(subDir, "*.xls", new ProcessFileDelegate(PumpXlsFile), false);
                }
                finally
                {
                    CommonRoutines.DeleteDirectory(tempDir);
                }
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation,
                    string.Format("Завершение закачки архива {0}", archFile.Name));
            }
        }

        private void PumpNovosibXlsFiles(DirectoryInfo dir)
        {
            PumpNovosibRarFiles(dir);
            PumpNovosibArjFiles(dir);
        }

        #endregion Новосибирск

        #region Орск

        private void PumpXlsFilesOrsk(DirectoryInfo dir)
        {
            ProcessFilesTemplate(dir, "*000.xls", new ProcessFileDelegate(PumpXlsFile), false);
        }

        #endregion Орск

        private void PumpXlsFiles(DirectoryInfo dir)
        {
            if (this.Region ==  RegionName.Kalmykya)
                PumpKalmykiaXlsFiles(dir);
            else if (this.Region == RegionName.Novosibirsk)
                PumpNovosibXlsFiles(dir);
            else if (this.Region == RegionName.Orsk)
                PumpXlsFilesOrsk(dir);
        }

        #endregion Работа с Xls

        #region Перекрытые методы закачки
        
        protected override void ProcessFiles(DirectoryInfo dir)
        {
            deletedDateList = new List<int>();
            switch (this.Region)
            {
                case RegionName.MoskvaObl:
                    PumpFilesMoskva(dir);
                    break;
                case RegionName.Kalmykya:
                case RegionName.Novosibirsk:
                    PumpXlsFiles(dir);
                    break;
                default:
                    PumpTxtFiles(dir);
                    PumpXlsFiles(dir);
                    break;
            }
            UpdateData();
        }

        private void SetFlags()
        {
            finalOverturn = Convert.ToBoolean(GetParamValueByName(this.PumpRegistryElement.ProgramConfig, "cbFinalOverturn", "False"));
            toBuildArchives = Convert.ToBoolean(GetParamValueByName(this.PumpRegistryElement.ProgramConfig, "cbBuildArchives", "False"));
        }

        protected override void DirectPumpData()
        {
            SetFlags();
            PumpDataYTemplate();
        }

        #endregion Перекрытые методы закачки

        #endregion Закачка данных

		#region Обработка данных

        #region Перекрытые методы обработки

        private void FillProcessCaches()
        {
            FillRowsCache(ref cacheEgrul, dsEgrul.Tables[0], new string[] { "INN", "INN20" });
            FillRowsCache(ref cacheOrgOkved, dsOrgOkved.Tables[0], "RefOrgEGRUL", "Code");
            FillRowsCache(ref cacheEgrip, dsEgrip.Tables[0], new string[] { "INN" });
            FillRowsCache(ref cacheIpOkved, dsIpOkved.Tables[0], "RefIPEGRIP", "Code");
            FillRowsCache(ref cacheOrgBridge, dsOrgBridge.Tables[0], "Inn", "Id");
        }

        private void QueryProcessData()
        {
            bridgeClsSourceID = this.Scheme.DataVersionsManager.DataVersions.FindCurrentVersion(clsOrgBridge.ObjectKey);
            if (this.Region == RegionName.MoskvaObl)
            {
                QueryProcessDataMoskva();
                return;
            }
            InitDataSet(ref daEgrul, ref dsEgrul, clsEgrul, string.Empty);
            InitDataSet(ref daOrgOkved, ref dsOrgOkved, clsOrgOkved, string.Empty);
            InitDataSet(ref daEgrip, ref dsEgrip, clsEgrip, string.Empty);
            InitDataSet(ref daIpOkved, ref dsIpOkved, clsIpOkved, string.Empty);
            InitDataSet(ref daOrgBridge, ref dsOrgBridge, clsOrgBridge, string.Format("SourceID = {0}", bridgeClsSourceID));
            FillProcessCaches();
        }

        protected override void QueryDataForProcess()
        {
            QueryData();
            QueryProcessData();
        }

        private void FinalizeProcessData()
        {
            ClearDataSet(ref dsEgrul);
            ClearDataSet(ref dsOrgOkved);
            ClearDataSet(ref dsEgrip);
            ClearDataSet(ref dsIpOkved);
            ClearDataSet(ref dsOrgBridge);
        }

        protected override void ProcessFinalizing()
        {
            PumpFinalizing();
            FinalizeProcessData();
        }

        protected override void UpdateProcessedData()
        {
            UpdateOkatoData();
            UpdateData();
            UpdateDataSet(daOrgBridge, dsOrgBridge, clsOrgBridge);
        }

        protected override void AfterProcessDataAction()
        {
            UpdateMessagesDS();
            WriteBadOkatoCodesCacheToBD();
        }

        #endregion Перекрытые методы обработки

        #region установка ОКВЭД классификатора

        private bool CheckBridgeCls()
        {
            if ((cacheEgrul.Count <= 1) && (cacheEgrip.Count <= 1))
            {
                WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeWarning,
                    string.Concat("Нет данных по классификаторам 'Организации.ЕГРЮЛ' и 'Индивидуальные предприниматели.ЕГРИП', ", 
                                  "поле 'ОКВЭД' классификатора 'Организации.УФК плательщики' заполнено не будет."));
                return false;
            }
            return true;
        }

        private string GetOKVED(string masterCacheKey, Dictionary<string, DataRow> masterCache, Dictionary<int, string> detailCache)
        {
            if (!masterCache.ContainsKey(masterCacheKey))
                return string.Empty;
            // получаем основной оквед
            DataRow masterRow = masterCache[masterCacheKey];
            string mainOKVED = masterRow["MainOKVED"].ToString();
            if (mainOKVED != string.Empty)
                return mainOKVED;
            // если основной оквед не заполнен, берем код из первой дочерней записи
            int masterID = Convert.ToInt32(masterRow["ID"]);
            if (!detailCache.ContainsKey(masterID))
                return string.Empty;
            return detailCache[masterID];
        }

        private string FormatOKVED(string okved)
        {
            okved = okved.Replace(".", "").PadRight(6, '0');
            return okved;
        }

        // установка ОКВЭД классификатора Организации.УФК плательщики
        private void SetOrgOKVED()
        {
            WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeInformation, 
                "начало установки поля 'Оквед' классификатора 'Организации.УФК плательщики'");
            if (!CheckBridgeCls())
                return;
            foreach (DataRow orgRow in dsOrg.Tables[0].Rows)
            {
                string key = string.Concat(orgRow["INN"].ToString().PadLeft(constTotalWidthForComplexKey, '0'),
                                           orgRow["KPP"].ToString().PadLeft(constTotalWidthForComplexKey, '0'));
                string okved = GetOKVED(key, cacheEgrul, cacheOrgOkved);
                if (okved == string.Empty)
                {
                    key = orgRow["INN"].ToString();
                    okved = GetOKVED(key, cacheEgrip, cacheIpOkved);
                }
                if (okved == string.Empty)
                    continue;
                orgRow["OKVED"] = FormatOKVED(okved);
            }
            WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeInformation, 
                "завершение установки поля 'Оквед' классификатора 'Организации.УФК плательщики'");
        }

        #endregion установка ОКВЭД классификатора

        #region ниибическая установка иерархии организаций

        #region формирование классификатора Организации.Плательщики_Сопоставимый

        private int GetOrgBridgeParentId(string inn)
        {
            inn = CommonRoutines.RemoveLetters(inn).Trim().TrimStart(new char[] { '0' });
            if (inn.Length == 10)
                return orgBridgeParentIds[0];
            if (inn.Length == 12)
                return orgBridgeParentIds[1];
            return orgBridgeParentIds[2];
        }

        private int PumpOrgBridgeParentRow(Dictionary<string, int> cache, string name)
        {
            string key = string.Format("{0}|0", name);
            object[] mapping = new object[] { "Name", name, "Inn", 0, "SourceId", bridgeClsSourceID };
            return PumpCachedRow(cache, dsOrgBridge.Tables[0], clsOrgBridge, key, mapping);
        }

        // добавляем записи в классификатор "Организации.Плательщики_Сопоставимый" на первый уровень иерархии
        private void PumpOrgBridgeParentRows()
        {
            Dictionary<string, int> cache = new Dictionary<string, int>();
            try
            {
                orgBridgeParentIds = new int[3];
                FillRowsCache(ref cache, dsOrgBridge.Tables[0], new string[] { "Name", "Inn" }, "|", "Id");
                orgBridgeParentIds[0] = PumpOrgBridgeParentRow(cache, "Организации");
                orgBridgeParentIds[1] = PumpOrgBridgeParentRow(cache, "Индивидуальные предприниматели");
                orgBridgeParentIds[2] = PumpOrgBridgeParentRow(cache, "Прочие");
                UpdateDataSet(daOrgBridge, dsOrgBridge, clsOrgBridge);
            }
            finally
            {
                cache.Clear();
            }
        }

        private string GetCorrectBridgeName(DataRow orgBridgeRow)
        {
            string key = GetComplexCacheKey(orgBridgeRow, new string[] { "Inn", "Kpp" }, "|");
            if (cacheEgrulName.ContainsKey(key))
                return cacheEgrulName[key];
            if (cacheEgripName.ContainsKey(orgBridgeRow["Inn"].ToString()))
                return cacheEgripName[orgBridgeRow["Inn"].ToString()];
            return orgBridgeRow["Name"].ToString();
        }

        private void AddBridgeOrgRowMoskva(DataRow sourceRow, bool checkCache)
        {
            string inn = sourceRow["Inn"].ToString();
            int parentId = GetOrgBridgeParentId(inn);
            string key = string.Format("{0}|{1}", inn, parentId);
            if (cacheOrgBridge.ContainsKey(key) && checkCache)
                return;

            DataRow orgBridgeRow = dsOrgBridge.Tables[0].NewRow();
            CopyRowToRow(sourceRow, orgBridgeRow);

            int bridgeId = GetGeneratorNextValue(clsOrgBridge.GeneratorName);
            orgBridgeRow["Id"] = bridgeId;
            orgBridgeRow["SourceId"] = bridgeClsSourceID;
            orgBridgeRow["ParentId"] = parentId;
            if (checkCache)
                orgBridgeRow["Name"] = GetCorrectBridgeName(orgBridgeRow);

            dsOrgBridge.Tables[0].Rows.Add(orgBridgeRow);
            if (checkCache)
                cacheOrgBridge.Add(key, bridgeId);
        }

        private void AddBridgeOrgRow(decimal orgInn, DataRow sourceRow)
        {
            if (cacheOrgBridge.ContainsKey(orgInn.ToString()))
                return;

            DataRow orgBridgeRow = dsOrgBridge.Tables[0].NewRow();
            CopyRowToRow(sourceRow, orgBridgeRow);

            int bridgeId = GetGeneratorNextValue(clsOrgBridge.GeneratorName);
            orgBridgeRow["Id"] = bridgeId;
            orgBridgeRow["SourceId"] = bridgeClsSourceID;

            dsOrgBridge.Tables[0].Rows.Add(orgBridgeRow);
            cacheOrgBridge.Add(orgInn.ToString(), bridgeId);
        }

        #endregion формирование классификатора Организации.Плательщики_Сопоставимый

        // получить ахуенный кэш организаций: связка инн - айди
        // инн - уникальный в кэше
        // по умолчанию записывается первый ай ди группы, 
        // если потом встречается запись с КПП = ХХХХ01ХХХ - переписываем айди, ХХХХ50ХХХ - еще более приоритетная
        private Dictionary<decimal, int> GetAuxOrgCache(ref Dictionary<decimal, DataRow[]> cacheInnOrg)
        {
            Dictionary<decimal, int> cacheAuxOrg = new Dictionary<decimal, int>();
            DataRow[] orgRows = dsOrg.Tables[0].Select(string.Empty, "Inn Asc");
            bool toChangeId = true;
            foreach (DataRow orgRow in orgRows)
            {
                decimal orgInn = Convert.ToDecimal(orgRow["Inn"]);
                // нулевые инн не группируются
                if (orgInn == 0)
                {
                    if (this.Region == RegionName.MoskvaObl)
                        AddBridgeOrgRowMoskva(orgRow, false);
                    continue;
                }

                string orgKpp = orgRow["Kpp"].ToString();
                string orgKppPart = string.Empty;
                if (orgKpp.Length > 3)
                {
                    orgKppPart = orgKpp.Remove(orgKpp.Length - 3);
                    if (orgKppPart.EndsWith("50"))
                        orgRow["PayerType"] = "Крупнейший";
                }
                int orgId = Convert.ToInt32(orgRow["Id"]);
                if (!cacheAuxOrg.ContainsKey(orgInn))
                {
                    cacheAuxOrg.Add(orgInn, orgId);
                    cacheInnOrg.Add(orgInn, new DataRow[] { orgRow });
                    toChangeId = true;
                }
                else
                {
                    cacheInnOrg[orgInn] = (DataRow[])CommonRoutines.ConcatArrays(cacheInnOrg[orgInn], new DataRow[] { orgRow });
                    if (!toChangeId)
                        continue;
                    if (orgKpp.Length <= 3)
                        continue;
                    if (orgKppPart.EndsWith("01"))
                        cacheAuxOrg[orgInn] = orgId;
                    if (orgKppPart.EndsWith("50"))
                    {
                        cacheAuxOrg[orgInn] = orgId;
                        toChangeId = false;
                    }
                }
            }
            return cacheAuxOrg;
        }

        // поиск записи в егрюлу по инн
        // Если их несколько, берем первую с КПП = ХХХХ50ХХХ, если такой нет, то с КПП= ХХХХ01ХХХ, 
        // если и таких нет, то берем первую попавшуюся с соответствующим ИНН
        private bool FindEgrulInnRow(decimal inn, ref string kpp, ref string name, ref string okato)
        {
            DataRow[] egrulRows = dsEgrul.Tables[0].Select(string.Format("Inn = {0}", inn), "INN20 Desc");
            if (egrulRows.GetLength(0) == 0)
                return false;
            string egrulKpp = string.Empty;
            string egrulName = string.Empty;
            string egrulOkato = string.Empty;
            foreach (DataRow egrulRow in egrulRows)
            {
                egrulKpp = egrulRow["INN20"].ToString();
                egrulName = egrulRow["NameP"].ToString();
                egrulOkato = egrulRow["Okato"].ToString();
                if (egrulKpp.Length <= 3)
                    continue;
                string egrulKppPart = egrulKpp.Remove(egrulKpp.Length - 3);
                if (egrulKppPart.EndsWith("50") || (egrulKppPart.EndsWith("01")))
                    break;
            }
            kpp = egrulKpp;
            name = egrulName;
            okato = egrulOkato;
            return true;
        }

        // поиск записи в егрипу по инн, берем первую попавшуюся с соответствующим ИНН
        private bool FindEgripInnRow(decimal inn, ref string kpp, ref string name, ref string okato, Dictionary<string, DataRow> cache)
        {
            if (!cache.ContainsKey(inn.ToString()))
                return false;
            DataRow egripRow = cache[inn.ToString()];
            kpp = "0";
            name = egripRow["FIO"].ToString();
            okato = egripRow["Okato"].ToString();
            return true;
        }

        // тру - иерархия установлена хотя бы у одной записи группы по инн
        private bool IsHierarchied(DataRow[] innRows, ref int parentId)
        {
            foreach (DataRow row in innRows)
                if (row["ParentId"] != DBNull.Value)
                {
                    parentId = Convert.ToInt32(row["ParentId"]);
                    return true;
                }
            return false;
        }

        // установка иерархии организации.уфк (алгоритм ниибаццо сложен, и логике не поддается)
        private void SetOrgHierarchy()
        {
            WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeInformation,
                "начало установки иерархии классификатора 'Организации.УФК плательщики'");
            if (this.Region == RegionName.MoskvaObl)
                PumpOrgBridgeParentRows();
            Dictionary<decimal, DataRow[]> cacheInnOrg = new Dictionary<decimal,DataRow[]>();
            Dictionary<decimal, int> cacheAuxOrg = GetAuxOrgCache(ref cacheInnOrg);
            Dictionary<int, DataRow> cacheRowOrg = null;
            FillRowsCache(ref cacheRowOrg, dsOrg.Tables[0], "ID");
            Dictionary<string, DataRow> cacheRowEgrip = null;
            FillRowsCache(ref cacheRowEgrip, dsEgrip.Tables[0], new string[] { "Inn" });
            Dictionary<int, int> hierDict = new Dictionary<int, int>();
            int processedRows = 0;
            try
            {
                foreach (KeyValuePair<decimal, int> cacheItem in cacheAuxOrg)
                {
                    processedRows++;
                    decimal orgInn = cacheItem.Key;
                    DataRow[] innRows = cacheInnOrg[orgInn];
                    // если группа по инн состоит из одной записи, добавляем в сопоставимый, больше ничего не делаем
                    if (innRows.GetLength(0) == 1)
                    {
                        if (this.Region == RegionName.MoskvaObl)
                            AddBridgeOrgRowMoskva(innRows[0], true);
                        else
                            AddBridgeOrgRow(orgInn, innRows[0]);
                        continue;
                    }
                    DataRow parentInnRow = null;
                    // если уже установлена иерархия - находим родительскую запись
                    int parentId = -1;
                    if (IsHierarchied(innRows, ref parentId))
                        parentInnRow = cacheRowOrg[parentId];
                    DataRow orgRow = cacheRowOrg[cacheItem.Value];
                    string parentKpp = string.Empty;
                    string parentName = string.Empty;
                    string parentOkato = string.Empty;
                    if (!FindEgrulInnRow(orgInn, ref parentKpp, ref parentName, ref parentOkato))
                        if (!FindEgripInnRow(orgInn, ref parentKpp, ref parentName, ref parentOkato, cacheRowEgrip))
                        {
                            parentKpp = orgRow["Kpp"].ToString();
                            parentName = orgRow["Name"].ToString();
                            parentOkato = orgRow["Okato"].ToString();
                        }
                    parentKpp = parentKpp.PadLeft(1, '0');
                    if (parentName.Trim() == string.Empty)
                        parentName = constDefaultClsName;
                    parentOkato = parentOkato.PadLeft(1, '0');
                    // добавляем родительскую запись, если ранее не найдена
                    if (parentInnRow == null)
                    {
                        parentInnRow = dsOrg.Tables[0].NewRow();
                        parentId = GetGeneratorNextValue(clsOrg.GeneratorName);
                        parentInnRow["ID"] = parentId;
                        dsOrg.Tables[0].Rows.Add(parentInnRow);
                    }
                    CopyRowToRow(orgRow, parentInnRow);
                    parentInnRow["ID"] = parentId;
                    parentInnRow["Kpp"] = parentKpp;
                    parentInnRow["Name"] = parentName;
                    parentInnRow["Okato"] = parentOkato;
                    parentInnRow["ParentId"] = DBNull.Value;
                    // подчиняем все записи группы родительской записи
                    foreach (DataRow innRow in innRows)
                    {
                        int rowId = Convert.ToInt32(innRow["Id"]);
                        if (rowId != parentId)
                            hierDict.Add(rowId, parentId);
                    }
                    // добавляем в организации.сопоставимый
                    if (this.Region == RegionName.MoskvaObl)
                        AddBridgeOrgRowMoskva(parentInnRow, true);
                    else
                        AddBridgeOrgRow(orgInn, parentInnRow);
                }
                UpdateDataSet(daOrg, dsOrg, clsOrg);
                // устанавливаем иерархию
                foreach (KeyValuePair<int, int> cacheItem in hierDict)
                    cacheRowOrg[cacheItem.Key]["ParentId"] = cacheItem.Value;
            }
            finally
            {
                cacheAuxOrg.Clear();
                cacheRowOrg.Clear();
                cacheInnOrg.Clear();
                cacheRowEgrip.Clear();
                hierDict.Clear();
            }
            WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeInformation,
                "завершение установки иерархии классификатора 'Организации.УФК плательщики'");
        }

        #endregion ниибическая установка иерархии организаций

        private string GetDateConstraint(string dateFieldName)
        {
            string dateConstraint = string.Empty;
            // если закачки не было, берем ограничение из параметров обработки
            if (!this.StagesQueue[PumpProcessStates.PumpData].IsExecuted)
            {
                int dateRefMin = -1;
                int dateRefMax = -1;
                if (year > 0)
                {
                    dateRefMin = year * 10000 + month * 100;
                    if (month > 0)
                        dateRefMax = year * 10000 + month * 100 + CommonRoutines.GetDaysInMonth(month, year);
                    else
                        dateRefMax = (year + 1) * 10000;
                }
                if (dateRefMin != -1)
                    dateConstraint = string.Format(" {0} >= {1} and {0} <= {2} ", dateFieldName, dateRefMin, dateRefMax);
            }
            else if (deletedDateList.Count > 0)
            {
                // закачка была, получаем даты из списка закачанных дат
                dateConstraint = string.Format(" {0} in ({1}) ", dateFieldName,
                    string.Join(", ", deletedDateList.ConvertAll<string>(Convert.ToString).ToArray()));
            }
            return dateConstraint;
        }

        private void Disintegrate()
        {
            WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeInformation, "Начало расщепления сумм");

            CheckDisintRulesCache();
            PrepareMessagesDS();
            PrepareBadOkatoCodesCache();
            PrepareRegionsForSumDisint();
            disintFlagFieldName = "RefFX";
            refBudgetLevelFieldName = "RefFX";
            this.disintDateConstraint = GetDateConstraint("RefFKDay");
            DisintegrateData(fctFactUFK14Dirty, fctFactUFK14, clsKd, clsOkato, new string[] { "Credit" },
                "RefFKDay", "RefKD", "RefOKATO", disintAll);
            UpdateProcessedData();

            WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeInformation, "Завершение расщепления сумм");
        }

        // для регионов, кроме омска, проставляем имя = инн
        private void SetAdminName()
        {
            if (this.Region == RegionName.Omsk)
                return;
            WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeInformation,
                "начало установки поля 'Наименование' классификатора 'Администратор.УФК по ИНН'");
            foreach (DataRow row in dsAdmin.Tables[0].Rows)
            {
                string name = row["Name"].ToString();
                if ((name.Trim() == string.Empty) || (name == constDefaultClsName))
                    row["Name"] = row["CodeStr"].ToString();
            }
            WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeInformation,
                "завершение установки поля 'Наименование' классификатора 'Администратор.УФК по ИНН'");
        }

        protected override void ProcessDataSource()
        {
            SetAdminName();
            if (toProcessOrgCls)
            {
                SetOrgHierarchy();
                SetOrgOKVED();
            }
            UpdateProcessedData();

            if (this.Region == RegionName.MoskvaObl)
            {
                ProcessDataSourceMoskva();
                return;
            }

            // расщепление
            if (toDisintData)
            {
                Disintegrate();
            }
        }

        protected override void DirectProcessData()
        {
            FillDisintRulesCache();
            year = -1;
            month = -1;
            if (!this.StagesQueue[PumpProcessStates.PumpData].IsExecuted)
                GetDisintParams(ref year, ref month, ref disintAll);
            toProcessOrgCls = Convert.ToBoolean(GetParamValueByName(this.PumpRegistryElement.ProgramConfig, "cbProcessOrgCls", "False"));
            toDisintData = Convert.ToBoolean(GetParamValueByName(this.PumpRegistryElement.ProgramConfig, "cbDisintData", "False"));
            toDisintOutbankData = Convert.ToBoolean(GetParamValueByName(this.PumpRegistryElement.ProgramConfig, "cbDisintOutbankData", "False"));
            string message = string.Empty;
            if (toDisintData)
                message += "Расщепление сумм по нормативам отчисления доходов";
            if (toProcessOrgCls)
            {
                if (message != string.Empty)
                    message += "; ";
                message += "Преобразование классификатора 'Организации.УФК плательщики' ";
            }
            if (this.Region == RegionName.MoskvaObl)
            {
                toSetRefStrukt = Convert.ToBoolean(GetParamValueByName(this.PumpRegistryElement.ProgramConfig, "cbSetRefStrukt", "False"));
                if (toSetRefStrukt)
                {
                    if (message != string.Empty)
                        message += "; ";
                    message += "Проставление ссылок на 'Фиксированный.Признак структуры'; ";
                }
            }
            ProcessDataSourcesTemplate(year, month, message);
        }

		#endregion Обработка данных

        #region Проверка данных

        #region проверка данных для текстового формата

        private void TXTQueryDataForCheck()
        {
            QueryData();
            InitDataSet(ref daFactUFK14, ref dsFactUFK14, fctFactUFK14, true,
                string.Format("SOURCEID = {0} and RefFX = 15", this.SourceID), string.Empty);
        }

        private void CheckRow(object[] mapping, int refDate)
        {
            string kd = mapping[0].ToString();
            int kdRef = FindCachedRow(cacheKd, kd, -1);
            if (kdRef == -1)
                return;
            double registrySum = Convert.ToDouble(mapping[1].ToString().Replace(" ", ""));
            DataRow[] rows = dsFactUFK14.Tables[0].Select(string.Format("RefYearDayUNV = {0} and RefKD = {1}", refDate, kdRef));
            if (rows.GetLength(0) == 0)
                return;
            double factSum = 0;
            foreach (DataRow row in rows)
                factSum += Convert.ToDouble(row["Credit"]);
            if (registrySum != factSum)
                CheckDataProtocol.WriteEventIntoReviseDataProtocol(ReviseDataEventKind.pdeWarning,
                    string.Concat(string.Format("Сумма по КБК {0} реестра перечисленных поступлений ({1}) ", kd, registrySum),
                        string.Format("не совпадает с суммой по выписке из сводного реестра поступлений и выбытий средств бюджета ({0})", factSum)), 
                        this.PumpID, this.SourceID);
        }

        private void CheckRegistryFile(FileInfo file)
        {
            string[] reportData = CommonRoutines.GetTxtReportData(file, GetCodePage());
            int refDate = Convert.ToInt32(CommonRoutines.LongDateToNewDate(reportData[6].Split(DELIMETER)[0].Trim()));
            bool toGetColumnsValues = false;
            object[] mapping = null;
            int rowIndex = 0;
            foreach (string row in reportData)
                try
                {
                    rowIndex++;
                    if (IsTotalSum(row.Split(DELIMETER)[0].Trim()))
                        return;
                    if (IsRowEnd(row))
                    {
                        if (mapping != null)
                        {
                            CheckRow(mapping, refDate);
                            mapping = null;
                        }
                        continue;
                    }
                    if (!RowContainsDelimeter(row))
                        continue;
                    if (IsTableTitle(row))
                    {
                        toGetColumnsValues = true;
                        continue;
                    }
                    if (toGetColumnsValues)
                        GetColumnValues(row, ref mapping, CHECK_COLUMN_INDICES);
                }
                catch (Exception exp)
                {
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                        string.Format("Ошибка при обработке строки {0} файла реестра {1}: {2}",
                        rowIndex, file.Name, exp.Message));
                }
        }

        private void TXTCheck(DirectoryInfo dir)
        {
            TXTQueryDataForCheck();
            FileInfo[] files = dir.GetFiles("r*.txt", SearchOption.AllDirectories);
            foreach (FileInfo file in files)
                CheckRegistryFile(file);
        }

        private void CheckOrskData()
        {
            DirectoryInfo[] dir_years = this.RootDir.GetDirectories("*", SearchOption.TopDirectoryOnly);
            if (dir_years.GetLength(0) == 0)
                throw new PumpDataFailedException(string.Format(
                    "В каталоге {0} не найдено ни одного источника.", this.RootDir.FullName));
            for (int i = 0; i < dir_years.GetLength(0); i++)
            {
                this.DataSource = null;
                if (dir_years[i].Name.StartsWith("__"))
                    continue;
                int sourceYear = Convert.ToInt32(dir_years[i].Name);
                SetDataSource(ParamKindTypes.Year, string.Empty, sourceYear, 0, string.Empty, 0, string.Empty);
                TXTCheck(dir_years[i]);
            }
        }

        #endregion проверка данных для текстового формата

        #region проверка данных базы

        private void CheckSums(decimal debit, decimal credit, string kd, string okato, string refDate)
        {
            if (debit == credit)
                return;
            string comparison = string.Empty;
            if (credit > debit)
                comparison = "больше";
            else
                comparison = "меньше";
            string message = string.Concat("На уровне «Бюджет субъекта» по данным выписки и сводного реестра поступлений ",
                                           string.Format("и выбытий средств бюджета по КБК {0} и ОКАТО {1} ", kd, okato),
                                           string.Format("сумма кредита {0} {1} суммы дебета {2} ", credit, comparison, debit),
                                           string.Format("за день {0}. Разница составила {1}.", refDate, Math.Abs(credit - debit)));
            CheckDataProtocol.WriteEventIntoReviseDataProtocol(ReviseDataEventKind.pdeWarning, message, this.PumpID, this.SourceID);
        }

        private string GetCacheKey(DataRow row, string dateFieldName)
        {
            return string.Format("{0}|{1}|{2}", row["RefKD"].ToString(), row["RefOKATO"].ToString(), row[dateFieldName].ToString());
        }

        private Dictionary<string, decimal> GetCheckCache(IFactTable fct, string constraint, string dateFieldName, string sumFieldName)
        {
            Dictionary<string, decimal> cache = new Dictionary<string, decimal>();
            string semantic = fct.FullCaption;
            int totalRecs = Convert.ToInt32(this.DB.ExecQuery(string.Format(
                "select count(id) from {0} where SOURCEID = {1} and {2}",
                fct.FullDBName, this.SourceID, constraint), QueryResultTypes.Scalar));
            int firstID = Convert.ToInt32(this.DB.ExecQuery(string.Format(
                "select min(id) from {0} where SOURCEID = {1}  and {2}",
                fct.FullDBName, this.SourceID, constraint), QueryResultTypes.Scalar));
            int lastID = firstID + MAX_DS_RECORDS_AMOUNT * 2 - 1;
            int processedRecCount = 0;
            IDbDataAdapter da = null;
            DataSet ds = null;
            do
            {
                string idConstr = string.Format("ID >= {0} and ID <= {1} and SOURCEID = {2} and {3}",
                    firstID, lastID, this.SourceID, constraint);
                firstID = lastID + 1;
                lastID += MAX_DS_RECORDS_AMOUNT * 2;
                InitDataSet(ref da, ref ds, fct, idConstr);
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    processedRecCount++;
                    string cacheKey = GetCacheKey(row, dateFieldName);
                    decimal sum = Convert.ToDecimal(row[sumFieldName]);
                    if (cache.ContainsKey(cacheKey))
                        cache[cacheKey] += sum;
                    else
                        cache.Add(cacheKey, sum);
                }
                ClearDataSet(ref ds);
            }
            while (processedRecCount < totalRecs);
            return cache;
        }

        private void CheckData()
        {
            Dictionary<string, decimal> creditCache = GetCheckCache(fctFactUFK14, GetDateConstraint("RefFKDay") + " and (RefFX = 3)", "RefYearDayUNV", "Credit");
            Dictionary<string, decimal> debitCache = GetCheckCache(fctFactUFK14Dirty, GetDateConstraint("RefFKDay"), "RefFKDay", "Debit");
            try
            {
                foreach (KeyValuePair<string, decimal> cacheItem in debitCache)
                {
                    string kd = FindRow(dsKd.Tables[0], new object[] { "ID", cacheItem.Key.Split('|')[0] })["CodeStr"].ToString();
                    string okato = FindRow(dsOkato.Tables[0], new object[] { "ID", cacheItem.Key.Split('|')[1] })["Code"].ToString();
                    string date = cacheItem.Key.Split('|')[2];
                    if (!creditCache.ContainsKey(cacheItem.Key))
                    {
                        string message = string.Concat(string.Format("В базе отсутствуют данные по кредиту на уровне «Бюджет субъекта» по КБК {0} и ОКАТО {1} ", kd, okato),
                                                       string.Format("за день ФО {0}", date));
                        CheckDataProtocol.WriteEventIntoReviseDataProtocol(ReviseDataEventKind.pdeWarning, message, this.PumpID, this.SourceID);
                        continue;
                    }
                    else
                        CheckSums(cacheItem.Value, Convert.ToDecimal(creditCache[cacheItem.Key]), kd, okato, date);
                }
            }
            finally
            {
                creditCache.Clear();
                debitCache.Clear();
            }
        }

        private void GetCheckParams()
        {
            year = -1;
            month = -1;
            if (!this.StagesQueue[PumpProcessStates.PumpData].IsExecuted)
                GetDisintParams(ref year, ref month, ref disintAll);
        }

        private Dictionary<int, string> GetDataSources()
        {
            Dictionary<int, string> dataSources = null;
            if (!this.StagesQueue[PumpProcessStates.PumpData].IsExecuted)
                dataSources = GetAllPumpedDataSources();
            else
                dataSources = this.PumpedSources;
            SortDataSources(ref dataSources);
            return dataSources;
        }

        private void CheckDatabaseData()
        {
            GetCheckParams();
            Dictionary<int, string> dataSources = GetDataSources();
            foreach (KeyValuePair<int, string> dataSource in dataSources)
            {
                if ((year > 0) && (GetDataSourceBySourceID(dataSource.Key).Year != year))
                    continue;
                SetDataSource(dataSource.Key);
                QueryData();
                CheckData();
            }
        }

        #endregion проверка данных базы

        protected override void DirectCheckData()
        {
            switch (this.Region)
            {
                case RegionName.Kalmykya:
                case RegionName.Novosibirsk:
                    CheckDatabaseData();
                    break;
                case RegionName.Omsk:
                    return;
                case RegionName.Orsk:
                    CheckOrskData();
                    break;
            }
            deletedDateList.Clear();
        }

        #endregion Проверка данных

    }
}
