using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.UFK20Pump
{

    // УФК - 0024 - Сводная ведомость по источникам финансирования
    public partial class UFK20PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // КИФ.УФК (d_KIF_UFK)
        private IDbDataAdapter daKif;
        private DataSet dsKif;
        private IClassifier clsKif;
        private Dictionary<string, int> cacheKif = null;
        // Местные бюджеты.УФК (d_LocBdgt_UFK)
        private IDbDataAdapter daBudgetSources;
        private DataSet dsBudgetSources;
        private IClassifier clsBudgetSources;
        private Dictionary<string, int> cacheBudgetSources = null;

        #endregion Классификаторы

        #region Факты

        // ИсточникиФинансирования.УФК_Сводная ведомость (f_SrcFin_UFK24)
        private IDbDataAdapter daUFK24;
        private DataSet dsUFK24;
        private IFactTable fctUFK24;

        #endregion Факты

        private int ufk24SourceId = -1;
        private List<int> ufk24SourceIds = new List<int>();
        private List<int> deletedSourcesList = null;

        #endregion Поля

        #region Константы

        private int CHARACTER_DATA = 2;

        #endregion Константы

        #region Закачка данных

        #region Работа с базой и кэшами

        private void FillCachesSources()
        {
            FillRowsCache(ref cacheKif, dsKif.Tables[0], "CodeStr", "Id");
            FillRowsCache(ref cacheBudgetSources, dsBudgetSources.Tables[0], "Name", "Id");
        }

        private void QueryDataSources()
        {
            string constr = string.Format("SOURCEID = {0}", ufk24SourceId);
            InitDataSet(ref daKif, ref dsKif, clsKif, false, constr, string.Empty);
            InitDataSet(ref daBudgetSources, ref dsBudgetSources, clsBudgetSources, false, constr, string.Empty);
            InitDataSet(ref daUFK24, ref dsUFK24, fctUFK24, false, constr, string.Empty);
        }

        private void UpdateDataSources()
        {
            UpdateDataSet(daKif, dsKif, clsKif);
            UpdateDataSet(daBudgetSources, dsBudgetSources, clsBudgetSources);
            UpdateDataSet(daUFK24, dsUFK24, fctUFK24);
        }

        private const string D_KIF_GUID = "73b83ed3-fa26-4d05-8e8e-30dbe226a801";
        private const string F_SRCFIN_UFK_24_GUID = "f6aa4272-3d5f-402f-bdb8-96027310d375";
        private void InitDBObjectsSources()
        {
            clsKif = this.Scheme.Classifiers[D_KIF_GUID];
            clsBudgetSources = this.Scheme.Classifiers[D_BUDGET_GUID];
            fctUFK24 = this.Scheme.FactTables[F_SRCFIN_UFK_24_GUID];
        }

        private void PumpFinalizingSources()
        {
            ClearDataSet(ref dsUFK24);
            ClearDataSet(ref dsKif);
            ClearDataSet(ref dsBudgetSources);
        }

        #endregion Работа с базой и кэшами

        #region Общие методы

        private int PumpBudgetSources(string name)
        {
            name = name.Trim();
            PumpCachedRow(cacheRegionForPump, dsRegionForPump.Tables[0], clsRegionForPump, name,
                new object[] { "Name", name, "OKATO", "0", "SourceId", clsSourceIdRegions });
            return PumpCachedRow(cacheBudgetSources, dsBudgetSources.Tables[0], clsBudgetSources, name,
                new object[] { "Account", "Неуказанный счет", "Name", name, "OKATO", "0", "SourceId", ufk24SourceId });
        }

        private int PumpKif(string codeStr, string codeTypeStr)
        {
            int codeType = Convert.ToInt32(CommonRoutines.TrimLetters(codeTypeStr.Trim()).PadLeft(1, '0'));
            return PumpCachedRow(cacheKif, dsKif.Tables[0], clsKif, codeStr,
                new object[] { "CodeStr", codeStr, "CodeType", codeType, "SourceId", ufk24SourceId });
        }

        private void PumpUfk24FactRow(object[] mapping)
        {
            PumpRow(dsUFK24.Tables[0], mapping);
            if (dsUFK24.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daUFK24, ref dsUFK24);
            }
        }

        private void DeleteDataSources(int refDate)
        {
            string constr = string.Format("RefFKDayUNV = {0} AND CharacterData = {1}", refDate, CHARACTER_DATA);
            DirectDeleteFactData(new IFactTable[] { fctUFK24 }, -1, ufk24SourceId, constr);
        }

        #endregion Общие методы

        #region Работа с Txt

        private void PumpTxtRowSource(string[] rowValues, int refDate, string strLocalBudget)
        {
            int refFODay = refDate;
            if (cachePeriod.ContainsKey(refDate))
                refFODay = cachePeriod[refDate];

            if (strLocalBudget == string.Empty)
                strLocalBudget = rowValues[1];

            int refBudget = PumpBudgetSources(strLocalBudget);

            object[] mapping = new object[] {
                "ForPeriod", ConvertFactValue(rowValues[5]),
                "RefLocBdgt", refBudget,
                "RefKIF", PumpKif(rowValues[3], rowValues[2]),
                "RefYearDayUNV", refFODay,
                "RefFKDayUNV", refDate,
                "CharacterData", CHARACTER_DATA,
                "SourceId", ufk24SourceId
            };

            PumpUfk24FactRow(mapping);
        }

        #endregion Работа с Txt

        #region Перекрытые методы

        protected override void DeleteEarlierPumpedData()
        {
            ufk24SourceId = AddDataSource("УФК", "0024", ParamKindTypes.Year,
                string.Empty, this.DataSource.Year, 0, string.Empty, 0, string.Empty).ID;
            ufk24SourceIds.Add(ufk24SourceId);
            if (this.DeleteEarlierData && !deletedSourcesList.Contains(ufk24SourceId))
            {
                // источники формируются на год, но данные удаляем за год-месяц
                int refDate = this.DataSource.Year * 10000 + this.DataSource.Month * 100;
                string constr = string.Format(" CharacterData = {0} AND RefFKDayUNV >= {1} AND RefFKDayUNV < {2} ",
                    CHARACTER_DATA, refDate, refDate + 100);
                DirectDeleteFactData(new IFactTable[] { fctUFK24 }, -1, ufk24SourceId, constr);
                deletedSourcesList.Add(ufk24SourceId);
            }
            base.DeleteEarlierPumpedData();
        }

        #endregion Перекрытые методы

        #endregion Закачка данных

        #region Обработка
        private void ProcessDataSourcesUFK24()
        {
            Dictionary<string, int> cacheRegionsForPumpRefTerr = null;
            FillRowsCache(ref cacheRegionsForPumpRefTerr, dsRegionForPump.Tables[0], "Name", "RefTerrType");
            if (cacheRegionsForPumpRefTerr.Count <= 1)
            {
                WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeWarning,
                    "Классификатор «Районы.Служебный для закачки» не заполнен.");
            }
            else
            {
                foreach (DataRow row in dsBudgetSources.Tables[0].Rows)
                {
                    int refTerrType = FindCachedRow(cacheRegionsForPumpRefTerr, row["Name"].ToString(), -1);
                    row["RefBudgetLevels"] = GetRefBudgetLevels(refTerrType);
                }
            }
        }
        #endregion

    }

}
