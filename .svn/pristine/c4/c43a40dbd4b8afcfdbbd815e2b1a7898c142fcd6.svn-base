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

namespace Krista.FM.Server.DataPumps.SKIFMonthRepPump
{
    // формат ФК - районы
    public partial class SKIFMonthRepPumpModule : SKIFRepPumpModuleBase
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
        // ЭКР.Эталонный (d_EKR_Etalon)
        private IDbDataAdapter daEkrTxt;
        private DataSet dsEkrTxt;
        private IClassifier clsEkrTxt;
        // КИФ.Эталонный (d_KIF_Etalon)
        private IDbDataAdapter daKifTxt;
        private DataSet dsKifTxt;
        private IClassifier clsKifTxt;

        public int refMeansTypeTxt;
        public int refRegionTxt;
        public int sourceBdgLvl;
        public string formTxt;
        public int refDateTxt;
        public int curBlock;

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
                    budKind = "МНЦП";
                    budName = "Муниципальные образования";
                    break;
                case 10:
                    budKind = "МНЦП";
                    budName = "Муниципальные образования";
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
            for (int curMonth = this.DataSource.Month; curMonth >= 1; curMonth--)
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
        private const string D_EKR_TXT_GUID = "03c3a566-82fb-48e9-b3d9-3d71f42a947f";
        private const string D_KIF_TXT_GUID = "a57d3b07-7bd1-4314-a24e-90d418010c5a";
        private void InitClsTxt()
        {
            clsKdTxt = this.Scheme.Classifiers[D_KD_TXT_GUID];
            clsFkrTxt = this.Scheme.Classifiers[D_FKR_TXT_GUID];
            clsEkrTxt = this.Scheme.Classifiers[D_EKR_TXT_GUID];
            clsKifTxt = this.Scheme.Classifiers[D_KIF_TXT_GUID];

            InitStandartCls(ref daKdTxt, ref dsKdTxt, clsKdTxt);
            InitStandartCls(ref daFkrTxt, ref dsFkrTxt, clsFkrTxt);
            InitStandartCls(ref daEkrTxt, ref dsEkrTxt, clsEkrTxt);
            InitStandartCls(ref daKifTxt, ref dsKifTxt, clsKifTxt);
        }

        private void ClearClsTxt()
        {
            ClearDataSet(ref dsKdTxt);
            ClearDataSet(ref dsFkrTxt);
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
                code = string.Format("{0}0000000000", row["Code"].ToString());
                name = row["Name"].ToString();
                // пропускаем служебные записи
                if (name.Contains("0022"))
                    continue;
                object[] mapping = new object[] { "Code", code, "Name", row["Name"] };
                PumpCachedRow(fkrCache, dsFKR.Tables[0], clsFKR, code, mapping);
            }
            code = "96000000000000";
            name = "Расходы бюджета – ИТОГО";
            PumpCachedRow(fkrCache, dsFKR.Tables[0], clsFKR, code,
                new object[] { "Code", code, "Name", name });
            code = "97000000000000";
            name = "Суммы, подлежащие взаимоисключению";
            PumpCachedRow(fkrCache, dsFKR.Tables[0], clsFKR, code,
                new object[] { "Code", code, "Name", name });

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
                if (!code.StartsWith("00001"))
                    continue;
                name = row["Name"].ToString();
                // пропускаем служебные записи
                if (name.Contains("0022"))
                    continue;
                object[] mapping = new object[] { "CodeStr", code, "Name", row["Name"], "KL", 0, "KST", 520 };
                PumpCachedRow(srcInFinCache, dsSrcInFin.Tables[0], clsSrcInFin, code, mapping);
            }
            code = "00090000000000000000";
            name = "Источники финансирования дефицита бюджетов - всего";
            PumpCachedRow(srcInFinCache, dsSrcInFin.Tables[0], clsSrcInFin, code,
                new object[] { "CodeStr", code, "Name", name, "KL", 10, "KST", 500 });
            code = "00057000000000000000";
            name = "Суммы, подлежащие взаимоисключению";
            PumpCachedRow(srcInFinCache, dsSrcInFin.Tables[0], clsSrcInFin, code,
                new object[] { "CodeStr", code, "Name", name, "KL", 3590, "KST", 750 });
            code = "00057000000000000710";
            name = "Увеличение внутренних заимствований";
            PumpCachedRow(srcInFinCache, dsSrcInFin.Tables[0], clsSrcInFin, code,
                new object[] { "CodeStr", code, "Name", name, "KL", 3600, "KST", 751 });
            code = "00057000000000000810";
            name = "Уменьшение внутренних заимствований";
            PumpCachedRow(srcInFinCache, dsSrcInFin.Tables[0], clsSrcInFin, code,
                new object[] { "CodeStr", code, "Name", name, "KL", 3610, "KST", 752 });
            code = "00057000000000000540";
            name = "Выдача бюджетных кредитов другим бюджетам бюджетной системы Российской Федерации";
            PumpCachedRow(srcInFinCache, dsSrcInFin.Tables[0], clsSrcInFin, code,
                new object[] { "CodeStr", code, "Name", name, "KL", 3612, "KST", 753 });
            code = "00057000000000000640";
            name = "Погашение бюджетных кредитов, выданных другим бюджетам бюджетной системы Российской Федерации";
            PumpCachedRow(srcInFinCache, dsSrcInFin.Tables[0], clsSrcInFin, code,
                new object[] { "CodeStr", code, "Name", name, "KL", 3614, "KST", 754 });
            code = "00057000000000000510";
            name = "Увеличение остатков средств";
            PumpCachedRow(srcInFinCache, dsSrcInFin.Tables[0], clsSrcInFin, code,
                new object[] { "CodeStr", code, "Name", name, "KL", 3616, "KST", 755 });
            code = "00057000000000000610";
            name = "Уменьшение остатков средств";
            PumpCachedRow(srcInFinCache, dsSrcInFin.Tables[0], clsSrcInFin, code,
                new object[] { "CodeStr", code, "Name", name, "KL", 3618, "KST", 756 });

        }

        private void PumpOutFinTxt()
        {
            foreach (DataRow row in dsKifTxt.Tables[0].Rows)
            {
                string code = row["CodeStr"].ToString();
                if (!code.StartsWith("00002"))
                    continue;
                string name = row["Name"].ToString();
                // пропускаем служебные записи
                if (name.Contains("0022"))
                    continue;
                object[] mapping = new object[] { "CodeStr", code, "Name", row["Name"], "KL", 0, "KST", 620 };
                PumpCachedRow(srcOutFinCache, dsSrcOutFin.Tables[0], clsSrcOutFin, code, mapping);
            }
        }

        private void PumpClsTxt()
        {
            if (toPumpIncomes)
                PumpKdTxt();
            if (toPumpOutcomes)
            {
                PumpFkrTxt();
                PumpEkrTxt();
            }
            if (toPumpInnerFinSources)
                PumpInFinTxt();
            if (toPumpOuterFinSources)
                PumpOutFinTxt();
            UpdateData();
        }

        #endregion работа с классификаторами

        #region импорт классификаторов

        private void SetPumpId(DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
                row["PumpId"] = this.PumpID;
        }

        private void SetTransaction(IDbDataAdapter da)
        {
            if (da.SelectCommand != null)
            {
                da.SelectCommand.Transaction = this.DB.Transaction;
            }
            if (da.InsertCommand != null)
            {
                da.InsertCommand.Transaction = this.DB.Transaction;
            }
            if (da.UpdateCommand != null)
            {
                da.UpdateCommand.Transaction = this.DB.Transaction;
            }
            if (da.DeleteCommand != null)
            {
                da.DeleteCommand.Transaction = this.DB.Transaction;
            }
        }

        // устанавливаем всем дата-адаптерам новую транзакцию,
        // т.к. после коммита старая транзакция слетает
        // (почему-то такое происходит только при работе с MsSQL, а на Oracle нет)
        private void SetNewTransaction()
        {
            // SetTransaction(daAnalFKR);
            SetTransaction(daRegions);
            SetTransaction(daRegions4Pump);
            SetTransaction(daKD);
            SetTransaction(daSrcOutFin);
            SetTransaction(daSrcInFin);
            SetTransaction(daFKR);
            SetTransaction(daFKRBook);
            SetTransaction(daEKRBook);
            SetTransaction(daKVSR);
            // SetTransaction(daMeansType);
            SetTransaction(daEKR);

            SetTransaction(daMonthRepDefProf);
            SetTransaction(daMonthRepIncomes);
            SetTransaction(daMonthRepOutFin);
            SetTransaction(daMonthRepInFin);
            SetTransaction(daMonthRepOutcomes);
            SetTransaction(daMonthRepOutDebtBooks);
            SetTransaction(daMonthRepInDebtBooks);
            SetTransaction(daMonthRepIncomesBooks);
            SetTransaction(daMonthRepArrearsBooks);
            SetTransaction(daMonthRepOutcomesBooks);
            SetTransaction(daMonthRepOutcomesBooksEx);
            SetTransaction(daMonthRepExcessBooks);
            SetTransaction(daMonthRepAccount);
        }

        private void ImportClsTxt(DirectoryInfo dir)
        {
            WriteToTrace("начало импорта классификаторов", TraceMessageKind.Information);
            CommitTransaction();

            IExportImporter exportImporter = Scheme.GetXmlExportImportManager().GetExportImporter(ObjectType.Classifier);
            ExportImportClsParams exportImportParams = new ExportImportClsParams();
            exportImportParams.DataSource = this.SourceID;
            // а вот и хуй, хитрый торасыч не устанавливает памп ай ди, поэтому нужен метод SetPumpId
            exportImportParams.PumpID = this.PumpID;

            exportImportParams.Filter = string.Empty;
            exportImportParams.FilterParams = null;
            exportImportParams.TaskID = -1;
            exportImportParams.RefVariant = -1;

            if (dir.GetFiles("*.xml", SearchOption.AllDirectories).GetLength(0) == 0)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                    "В данных для закачки не найдены xml-файлы, содержащие расшифровку показателей справочной таблицы.");
            }

            foreach (FileInfo file in dir.GetFiles("*.xml", SearchOption.AllDirectories))
            {
                string fileName = file.Name.ToUpper();
                // берем только хмл с классификаторами
                if (!fileName.StartsWith("ПОКАЗАТЕЛИ_"))
                    continue;
                if (fileName.Contains("СПРВНЕШДОЛГ"))
                {
                    if (!toPumpOuterFinSourcesRefs)
                        continue;
                    exportImportParams.ClsObjectKey = D_MARKS_MONTH_REP_OUT_DEBT_GUID;
                }
                else if (fileName.Contains("СПРВНУТРДОЛГ"))
                {
                    if (!toPumpInnerFinSourcesRefs)
                        continue;
                    exportImportParams.ClsObjectKey = D_MARKS_MONTH_REP_IN_DEBT_GUID;
                }
                else if (fileName.Contains("СПРЗАДОЛЖЕННОСТЬ"))
                {
                    if (!toPumpArrearsRefs)
                        continue;
                    exportImportParams.ClsObjectKey = D_MARKS_MONTH_REP_ARREARS_GUID;
                }
                else if (fileName.Contains("СПРОСТАТКИ"))
                {
                    if (!toPumpExcessRefs)
                        continue;
                    exportImportParams.ClsObjectKey = D_MARKS_EXCESS_GUID;
                }
                else if (fileName.Contains("СПРРАСХОДЫ"))
                {
                    if (!toPumpOutcomesRefsAdd)
                        continue;
                    exportImportParams.ClsObjectKey = D_MARKS_MONTH_REP_OUTCOMES_GUID;
                }
                else if (fileName.Contains("КОНСРАСЧЕТЫ"))
                {
                    if (!toPumpAccount)
                        continue;
                    exportImportParams.ClsObjectKey = D_MARKS_ACCOUNT_GUID;
                }
                else
                    continue;

                FileStream stream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read);
                exportImporter.ImportData(stream, exportImportParams);
            }

            BeginTransaction();
            SetNewTransaction();

            InitClsDataSet(ref daMarksOutDebt, ref dsMarksOutDebt, clsMarksOutDebt);
            FillRowsCache(ref scrOutFinSourcesRefCache, dsMarksOutDebt.Tables[0], "LONGCODE");
            SetPumpId(dsMarksOutDebt.Tables[0]);

            InitClsDataSet(ref daMarksInDebt, ref dsMarksInDebt, clsMarksInDebt);
            FillRowsCache(ref scrInFinSourcesRefCache, dsMarksInDebt.Tables[0], "LONGCODE");
            SetPumpId(dsMarksInDebt.Tables[0]);

            InitClsDataSet(ref daMarksArrears, ref dsMarksArrears, clsMarksArrears);
            FillRowsCache(ref arrearsCache, dsMarksArrears.Tables[0], "LONGCODE");
            SetPumpId(dsMarksArrears.Tables[0]);

            InitClsDataSet(ref daMarksExcess, ref dsMarksExcess, clsMarksExcess);
            FillRowsCache(ref excessCache, dsMarksExcess.Tables[0], "LongCode");
            SetPumpId(dsMarksExcess.Tables[0]);

            InitClsDataSet(ref daMarksOutcomes, ref dsMarksOutcomes, clsMarksOutcomes);
            FillRowsCache(ref marksOutcomesCache, dsMarksOutcomes.Tables[0], "LONGCODE");
            SetPumpId(dsMarksOutcomes.Tables[0]);

            InitClsDataSet(ref daMarksAccount, ref dsMarksAccount, clsMarksAccount);
            FillRowsCache(ref marksAccountCache, dsMarksAccount.Tables[0], "CODE");
            SetPumpId(dsMarksAccount.Tables[0]);

            WriteToTrace("завершение импорта классификаторов", TraceMessageKind.Information);
        }

        #endregion импорт классификаторов

        #region работа с фактами

        private string GetFormTxt(string fileName)
        {
            if (fileName.ToUpper().Contains("428V"))
                return "428V";
            else if (fileName.ToUpper().Contains("428"))
                return "428";
            else if (fileName.ToUpper().Contains("117"))
                return "117";
            else if (fileName.ToUpper().Contains("487"))
                return "487";
            else 
                return string.Empty;
        }

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
                PumpFactRow(dsMonthRepIncomes.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 3, "FactReport", 13, "ExcSumPRep", 4, "ExcSumFRep", 14, 
                            "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 1, "RefKD", refKd });
                PumpFactRow(dsMonthRepIncomes.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 5, "FactReport", 15, "ExcSumPRep", 6, "ExcSumFRep", 16, 
                            "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 2, "RefKD", refKd });
                PumpFactRow(dsMonthRepIncomes.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 7, "FactReport", 17, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 3, "RefKD", refKd });
                PumpFactRow(dsMonthRepIncomes.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 8, "FactReport", 18, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 11, "RefKD", refKd });
                PumpFactRow(dsMonthRepIncomes.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 9, "FactReport", 19, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 4, "RefKD", refKd });
                PumpFactRow(dsMonthRepIncomes.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 10, "FactReport", 20, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 5, "RefKD", refKd });
                PumpFactRow(dsMonthRepIncomes.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 11, "FactReport", 21, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 6, "RefKD", refKd });
                PumpFactRow(dsMonthRepIncomes.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 12, "FactReport", 22, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 8, "RefKD", refKd });
            }
            else
            {
                PumpFactRow(dsMonthRepIncomes.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 3, "FactReport", 11, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 1, "RefKD", refKd });
                PumpFactRow(dsMonthRepIncomes.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 4, "FactReport", 12, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 2, "RefKD", refKd });
                PumpFactRow(dsMonthRepIncomes.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 5, "FactReport", 13, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 3, "RefKD", refKd });
                PumpFactRow(dsMonthRepIncomes.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 6, "FactReport", 14, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 11, "RefKD", refKd });
                PumpFactRow(dsMonthRepIncomes.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 7, "FactReport", 15, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 4, "RefKD", refKd });
                PumpFactRow(dsMonthRepIncomes.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 8, "FactReport", 16, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 5, "RefKD", refKd });
                PumpFactRow(dsMonthRepIncomes.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 9, "FactReport", 17, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 6, "RefKD", refKd });
                PumpFactRow(dsMonthRepIncomes.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 10, "FactReport", 18, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 8, "RefKD", refKd });
            }
        }

        private void PumpOutcomes428Txt(string[] rowValues, int refFkr, int refEkr)
        {
            if (this.DataSource.Year >= 2011)
            {
                PumpFactRow(dsMonthRepOutcomes.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 6, "FactReport", 16,  "ExcSumPRep", 7, "ExcSumFRep", 17, 
                            "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 1, "RefFKR", refFkr, "RefEKR", refEkr });
                PumpFactRow(dsMonthRepOutcomes.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 8, "FactReport", 18,  "ExcSumPRep", 9, "ExcSumFRep", 19, 
                            "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 2, "RefFKR", refFkr, "RefEKR", refEkr });
                PumpFactRow(dsMonthRepOutcomes.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 10, "FactReport", 20, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 3, "RefFKR", refFkr, "RefEKR", refEkr });
                PumpFactRow(dsMonthRepOutcomes.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 11, "FactReport", 21, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 11, "RefFKR", refFkr, "RefEKR", refEkr });
                PumpFactRow(dsMonthRepOutcomes.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 12, "FactReport", 22, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 4, "RefFKR", refFkr, "RefEKR", refEkr });
                PumpFactRow(dsMonthRepOutcomes.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 13, "FactReport", 23, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 5, "RefFKR", refFkr, "RefEKR", refEkr });
                PumpFactRow(dsMonthRepOutcomes.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 14, "FactReport", 24, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 6, "RefFKR", refFkr, "RefEKR", refEkr });
                PumpFactRow(dsMonthRepOutcomes.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 15, "FactReport", 25, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 8, "RefFKR", refFkr, "RefEKR", refEkr });
            }
            else
            {
                PumpFactRow(dsMonthRepOutcomes.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 6, "FactReport", 14, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 1, "RefFKR", refFkr, "RefEKR", refEkr });
                PumpFactRow(dsMonthRepOutcomes.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 7, "FactReport", 15, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 2, "RefFKR", refFkr, "RefEKR", refEkr });
                PumpFactRow(dsMonthRepOutcomes.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 8, "FactReport", 16, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 3, "RefFKR", refFkr, "RefEKR", refEkr });
                PumpFactRow(dsMonthRepOutcomes.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 9, "FactReport", 17, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 11, "RefFKR", refFkr, "RefEKR", refEkr });
                PumpFactRow(dsMonthRepOutcomes.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 10, "FactReport", 18, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 4, "RefFKR", refFkr, "RefEKR", refEkr });
                PumpFactRow(dsMonthRepOutcomes.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 11, "FactReport", 19, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 5, "RefFKR", refFkr, "RefEKR", refEkr });
                PumpFactRow(dsMonthRepOutcomes.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 12, "FactReport", 20, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 6, "RefFKR", refFkr, "RefEKR", refEkr });
                PumpFactRow(dsMonthRepOutcomes.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 13, "FactReport", 21, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 8, "RefFKR", refFkr, "RefEKR", refEkr });
            }
        }

        private void PumpDefProf428Txt(string[] rowValues)
        {
            if (this.DataSource.Year >= 2011)
            {
                PumpFactRow(dsMonthRepDefProf.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 6, "FactReport", 16, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 1 });
                PumpFactRow(dsMonthRepDefProf.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 8, "FactReport", 18, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 2 });
                PumpFactRow(dsMonthRepDefProf.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 10, "FactReport", 20, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 3 });
                PumpFactRow(dsMonthRepDefProf.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 11, "FactReport", 21, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 11 });
                PumpFactRow(dsMonthRepDefProf.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 12, "FactReport", 22, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 4 });
                PumpFactRow(dsMonthRepDefProf.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 13, "FactReport", 23, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 5 });
                PumpFactRow(dsMonthRepDefProf.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 14, "FactReport", 24, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 6 });
                PumpFactRow(dsMonthRepDefProf.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 15, "FactReport", 25, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 8 });
            }
            else
            {
                PumpFactRow(dsMonthRepDefProf.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 6, "FactReport", 14, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 1 });
                PumpFactRow(dsMonthRepDefProf.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 7, "FactReport", 15, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 2 });
                PumpFactRow(dsMonthRepDefProf.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 8, "FactReport", 16, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 3 });
                PumpFactRow(dsMonthRepDefProf.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 9, "FactReport", 17, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 11 });
                PumpFactRow(dsMonthRepDefProf.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 10, "FactReport", 18, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 4 });
                PumpFactRow(dsMonthRepDefProf.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 11, "FactReport", 19, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 5 });
                PumpFactRow(dsMonthRepDefProf.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 12, "FactReport", 20, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 6 });
                PumpFactRow(dsMonthRepDefProf.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 13, "FactReport", 21, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 8 });
            }
        }

        private void PumpOutFin428Txt(string[] rowValues, int refSrcOutFin)
        {
            if (this.DataSource.Year >= 2011)
            {
                PumpFactRow(dsMonthRepOutFin.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 3, "FactReport", 13, "ExcSumPRep", 4, "ExcSumFRep", 14, 
                            "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 1, "RefSOF", refSrcOutFin });
                PumpFactRow(dsMonthRepOutFin.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 5, "FactReport", 15, "ExcSumPRep", 6, "ExcSumFRep", 16, 
                            "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 2, "RefSOF", refSrcOutFin });
                PumpFactRow(dsMonthRepOutFin.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 7, "FactReport", 17, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 3, "RefSOF", refSrcOutFin });
                PumpFactRow(dsMonthRepOutFin.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 8, "FactReport", 18, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 11, "RefSOF", refSrcOutFin });
                PumpFactRow(dsMonthRepOutFin.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 9, "FactReport", 19, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 4, "RefSOF", refSrcOutFin });
                PumpFactRow(dsMonthRepOutFin.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 10, "FactReport", 20, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 5, "RefSOF", refSrcOutFin });
                PumpFactRow(dsMonthRepOutFin.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 11, "FactReport", 21, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 6, "RefSOF", refSrcOutFin });
                PumpFactRow(dsMonthRepOutFin.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 12, "FactReport", 22, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 8, "RefSOF", refSrcOutFin });
            }
            else
            {
                PumpFactRow(dsMonthRepOutFin.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 3, "FactReport", 11, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 1, "RefSOF", refSrcOutFin });
                PumpFactRow(dsMonthRepOutFin.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 4, "FactReport", 12, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 2, "RefSOF", refSrcOutFin });
                PumpFactRow(dsMonthRepOutFin.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 5, "FactReport", 13, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 3, "RefSOF", refSrcOutFin });
                PumpFactRow(dsMonthRepOutFin.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 6, "FactReport", 14, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 11, "RefSOF", refSrcOutFin });
                PumpFactRow(dsMonthRepOutFin.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 7, "FactReport", 15, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 4, "RefSOF", refSrcOutFin });
                PumpFactRow(dsMonthRepOutFin.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 8, "FactReport", 16, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 5, "RefSOF", refSrcOutFin });
                PumpFactRow(dsMonthRepOutFin.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 9, "FactReport", 17, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 6, "RefSOF", refSrcOutFin });
                PumpFactRow(dsMonthRepOutFin.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 10, "FactReport", 18, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 8, "RefSOF", refSrcOutFin });
            }
        }

        private void PumpInFin428Txt(string[] rowValues, int refSrcInFin)
        {
            if (this.DataSource.Year >= 2011)
            {
                PumpFactRow(dsMonthRepInFin.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 3, "FactReport", 13, "ExcSumPRep", 4, "ExcSumFRep", 14, 
                            "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 1, "RefSIF", refSrcInFin });
                PumpFactRow(dsMonthRepInFin.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 5, "FactReport", 15, "ExcSumPRep", 6, "ExcSumFRep", 16, 
                            "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 2, "RefSIF", refSrcInFin });
                PumpFactRow(dsMonthRepInFin.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 7, "FactReport", 17, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 3, "RefSIF", refSrcInFin });
                PumpFactRow(dsMonthRepInFin.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 8, "FactReport", 18, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 11, "RefSIF", refSrcInFin });
                PumpFactRow(dsMonthRepInFin.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 9, "FactReport", 19, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 4, "RefSIF", refSrcInFin });
                PumpFactRow(dsMonthRepInFin.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 10, "FactReport", 20, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 5, "RefSIF", refSrcInFin });
                PumpFactRow(dsMonthRepInFin.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 11, "FactReport", 21, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 6, "RefSIF", refSrcInFin });
                PumpFactRow(dsMonthRepInFin.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 12, "FactReport", 22, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 8, "RefSIF", refSrcInFin });
            }
            else
            {
                PumpFactRow(dsMonthRepInFin.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 3, "FactReport", 11, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 1, "RefSIF", refSrcInFin });
                PumpFactRow(dsMonthRepInFin.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 4, "FactReport", 12, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 2, "RefSIF", refSrcInFin });
                PumpFactRow(dsMonthRepInFin.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 5, "FactReport", 13, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 3, "RefSIF", refSrcInFin });
                PumpFactRow(dsMonthRepInFin.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 6, "FactReport", 14, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 11, "RefSIF", refSrcInFin });
                PumpFactRow(dsMonthRepInFin.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 7, "FactReport", 15, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 4, "RefSIF", refSrcInFin });
                PumpFactRow(dsMonthRepInFin.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 8, "FactReport", 16, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 5, "RefSIF", refSrcInFin });
                PumpFactRow(dsMonthRepInFin.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 9, "FactReport", 17, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 6, "RefSIF", refSrcInFin });
                PumpFactRow(dsMonthRepInFin.Tables[0], rowValues,
                    new object[] { "YearPlanReport", 10, "FactReport", 18, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 8, "RefSIF", refSrcInFin });
            }
        }

        private void PumpAccount428Txt(string[] rowValues, int refMarksAccount)
        {
            PumpFactRow(dsMonthRepAccount.Tables[0], rowValues,
                new object[] { "ArrivalRep", 1, "RefMeansType", refMeansTypeTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 3, "RefAccount", refMarksAccount });
            PumpFactRow(dsMonthRepAccount.Tables[0], rowValues,
                new object[] { "ArrivalRep", 2, "RefMeansType", refMeansTypeTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 11, "RefAccount", refMarksAccount });
            PumpFactRow(dsMonthRepAccount.Tables[0], rowValues,
                new object[] { "ArrivalRep", 3, "RefMeansType", refMeansTypeTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 4, "RefAccount", refMarksAccount });
            PumpFactRow(dsMonthRepAccount.Tables[0], rowValues,
                new object[] { "ArrivalRep", 4, "RefMeansType", refMeansTypeTxt,
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 5, "RefAccount", refMarksAccount });
            PumpFactRow(dsMonthRepAccount.Tables[0], rowValues,
                new object[] { "ArrivalRep", 5, "RefMeansType", refMeansTypeTxt,
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 6, "RefAccount", refMarksAccount });
            PumpFactRow(dsMonthRepAccount.Tables[0], rowValues,
                new object[] { "ArrivalRep", 6, "RefMeansType", refMeansTypeTxt,
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 8, "RefAccount", refMarksAccount });
        }

        #endregion форма 428

        #region форма 428V

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

        private void PumpOutFin428VTxt(string[] rowValues, int refSrcOutFin)
        {
            PumpFactRow(dsMonthRepOutFin.Tables[0], rowValues,
                new object[] { "YearPlanReport", 3, "FactReport", 6, "SpreadFactYearPlanReport", 9, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 2, "RefSOF", refSrcOutFin });
            PumpFactRow(dsMonthRepOutFin.Tables[0], rowValues,
                new object[] { "YearPlanReport", 4, "FactReport", 7, "SpreadFactYearPlanReport", 10, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 3, "RefSOF", refSrcOutFin });
            PumpFactRow(dsMonthRepOutFin.Tables[0], rowValues,
                new object[] { "YearPlanReport", 5, "FactReport", 8, "SpreadFactYearPlanReport", 11, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt, 
                            "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 7, "RefSOF", refSrcOutFin });
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

        #endregion форма 117

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
                            PumpIncomes428VTxt(rowValues, refKd);
                            break;
                        case "117":
                            PumpIncomes117Txt(rowValues, refKd);
                            break;
                    }
                    break;
                case 2:
                    if (kst.ToString().StartsWith("2"))
                    {
                        if (!toPumpOutcomes)
                            break;
                        // расходы
                        string fkrCode = string.Format("{0}0000000000", Convert.ToInt32(rowValues[2]));
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
                                PumpOutcomes428VTxt(rowValues, refFkr, refEkr);
                                break;
                            case "117":
                                PumpOutcomes117Txt(rowValues, refFkr, refEkr);
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
                                PumpDefProf428VTxt(rowValues);
                                break;
                            case "117":
                                PumpDefProf117Txt(rowValues);
                                break;
                        }
                    }
                    break;
                case 3:
                    if (kst.ToString().StartsWith("6"))
                    {
                        // источники внешнего финансирования
                        if (!toPumpInnerFinSources)
                            break;
                        code = rowValues[2].PadLeft(20, '0');
                        mapping = new object[] { "CodeStr", code, "Name", constDefaultClsName, "KL", 0, "KST", 0 };
                        int refOutFin = PumpCachedRow(srcOutFinCache, dsSrcOutFin.Tables[0], clsSrcOutFin, code, mapping);
                        switch (formTxt)
                        {
                            case "428":
                                PumpOutFin428Txt(rowValues, refOutFin);
                                break;
                            case "428V":
                                PumpOutFin428VTxt(rowValues, refOutFin);
                                break;
                            case "117":
                                PumpOutFin117Txt(rowValues, refOutFin);
                                break;
                        }
                    }
                    else if ((kst.ToString().StartsWith("5")) || (kst.ToString().StartsWith("7")))
                    {
                        // источники внутреннего финансирования
                        if (!toPumpOuterFinSources)
                            break;
                        code = rowValues[2].PadLeft(20, '0');
                        mapping = new object[] { "CodeStr", code, "Name", constDefaultClsName, "KL", 0, "KST", 0 };
                        int refInFin = PumpCachedRow(srcInFinCache, dsSrcInFin.Tables[0], clsSrcInFin, code, mapping);
                        switch (formTxt)
                        {
                            case "428":
                                PumpInFin428Txt(rowValues, refInFin);
                                break;
                            case "428V":
                                PumpInFin428VTxt(rowValues, refInFin);
                                break;
                            case "117":
                                PumpInFin117Txt(rowValues, refInFin);
                                break;
                        }
                    }
                    break;
                case 4:
                    // конс расчеты
                    if (!toPumpAccount)
                        break;
                    if (this.DataSource.Year < 2011)
                        break;
                    code = rowValues[0].Trim();
                    mapping = new object[] { "Code", code, "Name", constDefaultClsName };
                    int refMarksAccount = PumpCachedRow(marksAccountCache, dsMarksAccount.Tables[0], clsMarksAccount, code, mapping);
                    if (formTxt == "428")
                        PumpAccount428Txt(rowValues, refMarksAccount);
                    break;
            }
        }

        #region форма 487

        private void Pump487ClsTxt(DataTable dt, string[] rowValues, string refClsName, int refClsValue)
        {
            if (this.DataSource.Year >= 2009)
            {
                PumpFactRow(dt, rowValues, new object[] {
                    "AssignedReport", 6, "FactReport", 18, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt,
                    "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 2, refClsName, refClsValue });
                PumpFactRow(dt, rowValues, new object[] {
                    "AssignedReport", 7, "FactReport", 19, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt,
                    "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 12, refClsName, refClsValue });
                PumpFactRow(dt, rowValues, new object[] {
                    "AssignedReport", 8, "FactReport", 20, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt,
                    "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 3, refClsName, refClsValue });
                PumpFactRow(dt, rowValues, new object[] {
                    "AssignedReport", 9, "FactReport", 21, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt,
                    "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 13, refClsName, refClsValue });
                PumpFactRow(dt, rowValues, new object[] {
                    "AssignedReport", 10, "FactReport", 22, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt,
                    "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 11, refClsName, refClsValue });
                PumpFactRow(dt, rowValues, new object[] {
                    "AssignedReport", 11, "FactReport", 23, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt,
                    "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 17, refClsName, refClsValue });
                PumpFactRow(dt, rowValues, new object[] {
                    "AssignedReport", 12, "FactReport", 24, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt,
                    "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 4, refClsName, refClsValue });
                PumpFactRow(dt, rowValues, new object[] {
                    "AssignedReport", 13, "FactReport", 25, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt,
                    "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 14, refClsName, refClsValue });
                PumpFactRow(dt, rowValues, new object[] {
                    "AssignedReport", 14, "FactReport", 26, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt,
                    "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 5, refClsName, refClsValue });
                PumpFactRow(dt, rowValues, new object[] {
                    "AssignedReport", 15, "FactReport", 27, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt,
                    "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 15, refClsName, refClsValue });
                PumpFactRow(dt, rowValues, new object[] {
                    "AssignedReport", 16, "FactReport", 28, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt,
                    "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 6, refClsName, refClsValue });
                PumpFactRow(dt, rowValues, new object[] {
                    "AssignedReport", 17, "FactReport", 29, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt,
                    "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 16, refClsName, refClsValue });
            }
            else
            {
                PumpFactRow(dt, rowValues, new object[] {
                    "AssignedReport", 6, "FactReport", 12, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt,
                    "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 2, refClsName, refClsValue });
                PumpFactRow(dt, rowValues, new object[] {
                    "AssignedReport", 7, "FactReport", 13, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt,
                    "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 3, refClsName, refClsValue });
                PumpFactRow(dt, rowValues, new object[] {
                    "AssignedReport", 8, "FactReport", 14, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt,
                    "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 11, refClsName, refClsValue });
                PumpFactRow(dt, rowValues, new object[] {
                    "AssignedReport", 9, "FactReport", 15, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt,
                    "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 4, refClsName, refClsValue });
                PumpFactRow(dt, rowValues, new object[] {
                    "AssignedReport", 10, "FactReport", 16, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt,
                    "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 5, refClsName, refClsValue });
                PumpFactRow(dt, rowValues, new object[] {
                    "AssignedReport", 11, "FactReport", 17, "RefMeansType", refMeansTypeTxt, "RefYearMonth", refDateTxt,
                    "RefYearDayUNV", refDateTxt, "RefRegions", refRegionTxt, "RefBdgtLevels", 6, refClsName, refClsValue });
            }
        }

        #endregion форма 487

        #region закачка справочной таблицы

        private void PumpAddFormTxt2011(string row)
        {
            string[] rowValues = row.Split('|');
            int kst = Convert.ToInt32(rowValues[0]);
            switch (curBlock)
            {
                case 1:
                    string code = string.Empty;
                    // Код = Код строки +ПРз+КЦСР+КВР+КОСГУ
                    code = string.Format("{0}{1}{2}{3}{4}", rowValues[0], rowValues[2], rowValues[3], rowValues[4], rowValues[5]);
                    if (((kst >= 10500) && (kst < 10600)) || ((kst >= 10700) && (kst < 10800)) || (kst == 12400))
                    {
                        // Факт.ФО_МесОтч_СпрВнутрДолг
                        if (!toPumpInnerFinSourcesRefs)
                            break;
                        int refInFin = FindCachedRow(scrInFinSourcesRefCache, code, nullMarksInDebt);
                        Pump487ClsTxt(dsMonthRepInDebtBooks.Tables[0], rowValues, "RefMarksInDebt", refInFin);
                    }
                    else if ((kst >= 10600) && (kst < 10700))
                    {
                        // Факт.ФО_МесОтч_СпрВнешДолг
                        if (!toPumpOuterFinSourcesRefs)
                            break;
                        int refOutFin = FindCachedRow(scrOutFinSourcesRefCache, code, nullMarksOutDebt);
                        Pump487ClsTxt(dsMonthRepOutDebtBooks.Tables[0], rowValues, "RefMarksOutDebt", refOutFin);
                    }
                    else if ((kst >= 10900) && (kst < 12100))
                    {
                        // Факт.ФО_МесОтч_СпрЗадолженность
                        if (!toPumpArrearsRefs)
                            break;
                        int refArrears = FindCachedRow(arrearsCache, code, nullMarksArrears);
                        Pump487ClsTxt(dsMonthRepArrearsBooks.Tables[0], rowValues, "RefMarksArrears", refArrears);
                    }
                    else if (((kst >= 100) && (kst < 10500)) || ((kst >= 12100) && (kst < 12400)) || (kst >= 13000 && kst <= 13602) ||
                        (kst >= 13000 && kst <= 14999 && this.DataSource.Year >= 2012))
                    {
                        // Факт.ФО_МесОтч_СпрРасходыДоп
                        if (!toPumpOutcomesRefsAdd)
                            break;
                        int refOutcomes = FindCachedRow(marksOutcomesCache, code, nullMarksOutcomes);
                        Pump487ClsTxt(dsMonthRepOutcomesBooksEx.Tables[0], rowValues, "RefMarksOutcomes", refOutcomes);
                    }
                    else if ((kst >= 10800) && (kst < 10900))
                    {
                        // Факт.ФО_МесОтч_СпрОстатки
                        if (!toPumpExcessRefs)
                            break;
                        int refMarks = FindCachedRow(excessCache, code, nullMarksExcess);
                        Pump487ClsTxt(dsMonthRepExcessBooks.Tables[0], rowValues, "RefMarks", refMarks);
                    }
                    break;
            }
        }

        private void PumpAddFormTxt2010(string row)
        {
            string[] rowValues = row.Split('|');
            int kst = Convert.ToInt32(rowValues[0]);
            switch (curBlock)
            {
                case 1:
                    string code = string.Empty;
                    // Код = Код строки +ПРз+КЦСР+КВР+КОСГУ
                    code = string.Format("{0}{1}{2}{3}{4}", rowValues[0], rowValues[2], rowValues[3], rowValues[4], rowValues[5]);
                    if (((kst >= 9600) && (kst < 9700)) || ((kst >= 9800) && (kst < 9900)) || (kst == 11600)) 
                    {
                        // Факт.ФО_МесОтч_СпрВнутрДолг
                        if (!toPumpInnerFinSourcesRefs)
                            break;
                        int refInFin = FindCachedRow(scrInFinSourcesRefCache, code, nullMarksInDebt);
                        Pump487ClsTxt(dsMonthRepInDebtBooks.Tables[0], rowValues, "RefMarksInDebt", refInFin);
                    }
                    else if ((kst >= 9700) && (kst < 9800))
                    {
                        // Факт.ФО_МесОтч_СпрВнешДолг
                        if (!toPumpOuterFinSourcesRefs)
                            break;
                        int refOutFin = FindCachedRow(scrOutFinSourcesRefCache, code, nullMarksOutDebt);
                        Pump487ClsTxt(dsMonthRepOutDebtBooks.Tables[0], rowValues, "RefMarksOutDebt", refOutFin);
                    }
                    else if ((kst >= 10000) && (kst < 11300))
                    {
                        // Факт.ФО_МесОтч_СпрЗадолженность
                        if (!toPumpArrearsRefs)
                            break;
                        int refArrears = FindCachedRow(arrearsCache, code, nullMarksArrears);
                        Pump487ClsTxt(dsMonthRepArrearsBooks.Tables[0], rowValues, "RefMarksArrears", refArrears);
                    }
                    else if (((kst >= 100) && (kst < 9600)) || ((kst >= 11300) && (kst < 11600))) 
                    {
                        // Факт.ФО_МесОтч_СпрРасходыДоп
                        if (!toPumpOutcomesRefsAdd)
                            break;
                        int refOutcomes = FindCachedRow(marksOutcomesCache, code, nullMarksOutcomes);
                        Pump487ClsTxt(dsMonthRepOutcomesBooksEx.Tables[0], rowValues, "RefMarksOutcomes", refOutcomes);
                    }
                    else if ((kst >= 9900) && (kst < 10000))
                    {
                        // Факт.ФО_МесОтч_СпрОстатки
                        if (!toPumpExcessRefs)
                            break;
                        int refMarks = FindCachedRow(excessCache, code, nullMarksExcess);
                        Pump487ClsTxt(dsMonthRepExcessBooks.Tables[0], rowValues, "RefMarks", refMarks);
                    }
                    break;
            }
        }

        private void PumpAddFormTxt2009(string row)
        {
            string[] rowValues = row.Split('|');
            int kst = Convert.ToInt32(rowValues[0]);
            switch (curBlock)
            {
                case 1:

                    string code = string.Empty;
                    // Код = ПРз+КЦСР+КВР+КОСГУ+Код строки
                    code = string.Format("{0}{1}{2}{3}{4}", rowValues[2], rowValues[3], rowValues[4], rowValues[5], rowValues[0]);
                    if (((kst >= 9700) && (kst < 9800)) || ((kst >= 9900) && (kst < 10000)))
                    {
                        // Факт.ФО_МесОтч_СпрВнутрДолг
                        if (!toPumpInnerFinSourcesRefs)
                            break;
                        int refInFin = FindCachedRow(scrInFinSourcesRefCache, code, nullMarksInDebt);
                        Pump487ClsTxt(dsMonthRepInDebtBooks.Tables[0], rowValues, "RefMarksInDebt", refInFin);
                    }
                    else if ((kst >= 9800) && (kst < 9900))
                    {
                        // Факт.ФО_МесОтч_СпрВнешДолг
                        if (!toPumpOuterFinSourcesRefs)
                            break;
                        int refOutFin = FindCachedRow(scrOutFinSourcesRefCache, code, nullMarksOutDebt);
                        Pump487ClsTxt(dsMonthRepOutDebtBooks.Tables[0], rowValues, "RefMarksOutDebt", refOutFin);
                    }
                    else if ((kst >= 10100) && (kst < 11400))
                    {
                        // Факт.ФО_МесОтч_СпрЗадолженность
                        if (!toPumpArrearsRefs)
                            break;
                        int refArrears = FindCachedRow(arrearsCache, code, nullMarksArrears);
                        Pump487ClsTxt(dsMonthRepArrearsBooks.Tables[0], rowValues, "RefMarksArrears", refArrears);
                    }
                    else if ((kst < 9700) || (kst >= 11400))
                    {
                        // Факт.ФО_МесОтч_СпрРасходыДоп
                        if (!toPumpOutcomesRefsAdd)
                            break;
                        int refOutcomes = FindCachedRow(marksOutcomesCache, code, nullMarksOutcomes);
                        Pump487ClsTxt(dsMonthRepOutcomesBooksEx.Tables[0], rowValues, "RefMarksOutcomes", refOutcomes);
                    }
                    else if ((kst >= 10000) && (kst < 10100))
                    {
                        // Факт.ФО_МесОтч_СпрОстатки
                        if (!toPumpExcessRefs)
                            break;
                        int refMarks = FindCachedRow(excessCache, code, nullMarksExcess);
                        Pump487ClsTxt(dsMonthRepExcessBooks.Tables[0], rowValues, "RefMarks", refMarks);
                    }
                    break;
            }
        }

        private void PumpAddFormTxt2008(string row)
        {
            string[] rowValues = row.Split('|');
            int kst = Convert.ToInt32(rowValues[0]);
            switch (curBlock)
            {
                case 1:

                    string code = string.Empty;
                    // Код = ПРз+КЦСР+КВР+КОСГУ+Код строки
                    code = string.Format("{0}{1}{2}{3}{4}", rowValues[2], rowValues[3], rowValues[4], rowValues[5], rowValues[0]);
                    if (((kst >= 8200) && (kst < 8300)) || ((kst >= 8400) && (kst < 8500)))
                    {
                        // Факт.ФО_МесОтч_СпрВнутрДолг
                        if (!toPumpInnerFinSourcesRefs)
                            break;
                        int refInFin = FindCachedRow(scrInFinSourcesRefCache, code, nullMarksInDebt);
                        Pump487ClsTxt(dsMonthRepInDebtBooks.Tables[0], rowValues, "RefMarksInDebt", refInFin);
                    }
                    else if ((kst >= 8300) && (kst < 8400))
                    {
                        // Факт.ФО_МесОтч_СпрВнешДолг
                        if (!toPumpOuterFinSourcesRefs)
                            break;
                        int refOutFin = FindCachedRow(scrOutFinSourcesRefCache, code, nullMarksOutDebt);
                        Pump487ClsTxt(dsMonthRepOutDebtBooks.Tables[0], rowValues, "RefMarksOutDebt", refOutFin);
                    }
                    else if (kst >= 8600)
                    {
                        // Факт.ФО_МесОтч_СпрЗадолженность
                        if (!toPumpArrearsRefs)
                            break;
                        int refArrears = FindCachedRow(arrearsCache, code, nullMarksArrears);
                        Pump487ClsTxt(dsMonthRepArrearsBooks.Tables[0], rowValues, "RefMarksArrears", refArrears);
                    }
                    else if (kst < 8200)
                    {
                        // Факт.ФО_МесОтч_СпрРасходыДоп
                        if (!toPumpOutcomesRefsAdd)
                            break;
                        int refOutcomes = FindCachedRow(marksOutcomesCache, code, nullMarksOutcomes);
                        Pump487ClsTxt(dsMonthRepOutcomesBooksEx.Tables[0], rowValues, "RefMarksOutcomes", refOutcomes);
                    }
                    else if ((kst >= 8500) && (kst < 8600))
                    {
                        // Факт.ФО_МесОтч_СпрОстатки
                        if (!toPumpExcessRefs)
                            break;
                        int refMarks = FindCachedRow(excessCache, code, nullMarksExcess);
                        Pump487ClsTxt(dsMonthRepExcessBooks.Tables[0], rowValues, "RefMarks", refMarks);
                    }
                    
                    break;
            }
        }

        private void PumpAddFormTxt(string row)
        {
            if (this.DataSource.Year >= 2011)
                PumpAddFormTxt2011(row);
            else if (this.DataSource.Year >= 2010)
                PumpAddFormTxt2010(row);
            else if (this.DataSource.Year >= 2009)
                PumpAddFormTxt2009(row);
            else
                PumpAddFormTxt2008(row);
        }

        #endregion закачка справочной таблицы

        private void PumpReportRowTxt(string row)
        {
            switch (formTxt)
            {
                case "428":
                case "428V":
                case "117":
                    PumpMainFormTxt(row);
                    break;
                case "487":
                    PumpAddFormTxt(row);
                    break;
            }
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
            bool form248IsPumped = false;
            FileInfo[] files = dir.GetFiles("*428*.txt", SearchOption.AllDirectories);
            foreach (FileInfo file in files)
            {
                form248IsPumped = true;
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping,
                    string.Format("Старт закачки файла {0}.", file.FullName));
                ProcessTxtFile(file);
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeSuccessfullFinishFilePump,
                    string.Format("Закачка файла {0} успешно завершена.", file.FullName));
            }
            if (!form248IsPumped)
                ProcessFilesTemplate(dir, "*117*.txt", new ProcessFileDelegate(ProcessTxtFile), false);

            ProcessFilesTemplate(dir, "*487*.txt", new ProcessFileDelegate(ProcessTxtFile), false);
        }

        private void PumpFactTxt(DirectoryInfo dir)
        {
            ProcessTxtDir(dir);

            FileInfo[] archFiles = dir.GetFiles("*.rar", SearchOption.AllDirectories);
            foreach (FileInfo archFile in archFiles)
            {

                WriteToTrace(string.Format("начало закачки архива {0}", archFile.Name), TraceMessageKind.Information);
                DirectoryInfo tempDir = CommonRoutines.ExtractArchiveFileToTempDir(archFile.FullName,
                    FilesExtractingOption.SingleDirectory, ArchivatorName.Rar);
                try
                {
                    ProcessTxtDir(tempDir);
                }
                finally
                {
                    CommonRoutines.DeleteDirectory(tempDir);
                }
                WriteToTrace(string.Format("завершение закачки архива {0}", archFile.Name), TraceMessageKind.Information);
            }
        }

        #endregion работа с фактами

        #region общая организация закачки

        protected override void PumpTxtReports(DirectoryInfo dir)
        {
            refDateTxt = this.DataSource.Year * 10000 + this.DataSource.Month * 100;
            if ((dir.GetFiles("*.txt", SearchOption.AllDirectories).GetLength(0) == 0) &&
                (dir.GetFiles("*.rar", SearchOption.AllDirectories).GetLength(0) == 0))
                return;

            toPumpIncomes = (ToPumpBlock(Block.bIncomes));
            toPumpOutcomes = (ToPumpBlock(Block.bOutcomes));
            toPumpDefProf = (ToPumpBlock(Block.bDefProf));
            toPumpInnerFinSources = (ToPumpBlock(Block.bInnerFinSources));
            toPumpOuterFinSources = (ToPumpBlock(Block.bOuterFinSources));
            toPumpAccount = (ToPumpBlock(Block.bAccount));
            toPumpIncomesRefs = (ToPumpBlock(Block.bIncomesRefs));
            toPumpOutcomesRefs = (ToPumpBlock(Block.bOutcomesRefs));
            toPumpOutcomesRefsAdd = (ToPumpBlock(Block.bOutcomesRefsAdd));
            toPumpInnerFinSourcesRefs = (ToPumpBlock(Block.bInnerFinSourcesRefs));
            toPumpOuterFinSourcesRefs = (ToPumpBlock(Block.bOuterFinSourcesRefs));
            toPumpArrearsRefs = (ToPumpBlock(Block.bArrearsRefs));
            toPumpExcessRefs = (ToPumpBlock(Block.bExcessRefs));

            ImportClsTxt(dir);

            InitClsTxt();
            PumpClsTxt();

            PumpFactTxt(dir);
            UpdateData();
            ClearClsTxt();
        }

        #endregion общая организация закачки

    }
}
