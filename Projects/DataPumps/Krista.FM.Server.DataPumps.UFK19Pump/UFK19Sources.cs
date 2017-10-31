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

namespace Krista.FM.Server.DataPumps.UFK19Pump
{
    // Закачка в блок "УФК_0024_Сводная ведомость по источникам финансирования"
    public partial class UFK19PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // Местные бюджеты.УФК (d_LocBdgt_UFK)
        private IDbDataAdapter daBudgetSources;
        private DataSet dsBudgetSources;
        private IClassifier clsBudgetSources;
        private Dictionary<string, int> cacheBudgetSources = null;
        // КИФ.УФК (d_KIF_UFK)
        private IDbDataAdapter daKif;
        private DataSet dsKif;
        private IClassifier clsKif;
        private Dictionary<string, int> cacheKif = null;
        // Период.Соответствие операционных дней (d_Date_ConversionFK)
        private IDbDataAdapter daPeriod;
        private DataSet dsPeriod;
        private IClassifier clsPeriod;
        private Dictionary<int, int> cachePeriod = null;

        #endregion Классификаторы

        #region Факты

        // ИсточникиФинансирования.УФК_Сводная ведомость (f_SrcFin_UFK24)
        private IDbDataAdapter daUFK24;
        private DataSet dsUFK24;
        private IFactTable fctUFK24;

        #endregion Факты

        private int ufk24SourceId = -1;
        private List<int> ufk24SourceIds = new List<int>();

        #endregion Поля

        #region Константы

        private int CHARACTER_DATA = 1;

        #endregion Константы

        #region Закачка данных

        #region Работа с базой и кэшами

        private void FillCachesSources()
        {
            FillRowsCache(ref cacheKif, dsKif.Tables[0], "CodeStr", "Id");
            FillRowsCache(ref cacheBudgetSources, dsBudgetSources.Tables[0], "Name", "Id");
            FillRowsCache(ref cachePeriod, dsPeriod.Tables[0], "RefFODate", "RefFKDate");
        }

        private void QueryDataSources()
        {
            string constr = string.Format("SOURCEID = {0}", ufk24SourceId);
            InitDataSet(ref daKif, ref dsKif, clsKif, false, constr, string.Empty);
            InitDataSet(ref daBudgetSources, ref dsBudgetSources, clsBudgetSources, false, constr, string.Empty);
            InitDataSet(ref daPeriod, ref dsPeriod, clsPeriod, string.Empty);
            InitDataSet(ref daUFK24, ref dsUFK24, fctUFK24, false, constr, string.Empty);
        }

        private void UpdateDataSources()
        {
            UpdateDataSet(daKif, dsKif, clsKif);
            UpdateDataSet(daBudgetSources, dsBudgetSources, clsBudgetSources);
            UpdateDataSet(daUFK24, dsUFK24, fctUFK24);
        }

        private const string D_KIF_GUID = "73b83ed3-fa26-4d05-8e8e-30dbe226a801";
        private const string D_DATE_CONVERSION_FK_GUID = "414c27e7-393c-4516-8b47-cf6df384569d";
        private const string F_SRCFIN_UFK_24_GUID = "f6aa4272-3d5f-402f-bdb8-96027310d375";
        private void InitDBObjectsSources()
        {
            clsKif = this.Scheme.Classifiers[D_KIF_GUID];
            clsBudgetSources = this.Scheme.Classifiers[D_BUD_UFK_GUID];
            clsPeriod = this.Scheme.Classifiers[D_DATE_CONVERSION_FK_GUID];
            fctUFK24 = this.Scheme.FactTables[F_SRCFIN_UFK_24_GUID];
        }

        private void PumpFinalizingSources()
        {
            ClearDataSet(ref dsUFK24);
            ClearDataSet(ref dsKif);
            ClearDataSet(ref dsBudgetSources);
            ClearDataSet(ref dsPeriod);
        }

        #endregion Работа с базой и кэшами

        #region Общие методы

        private int PumpBudgetSources(string name)
        {
            name = name.Trim();
            PumpCachedRow(cacheRegionForPump, dsRegionForPump.Tables[0], clsRegionForPump, name,
                new object[] { "Name", name, "OKATO", "0", "SourceId", clsSourceIdRegions });
            return PumpCachedRow(cacheBudgetSources, dsBudgetSources.Tables[0], clsBudgetSources, name,
                new object[] { "Account", "Неуказанный счет", "Name", name, "OKATO", 0, "SourceId", ufk24SourceId });
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
            string constr = string.Format("RefYearDayUNV = {0} AND CharacterData = {1}", refDate, CHARACTER_DATA);
            DirectDeleteFactData(new IFactTable[] { fctUFK24 }, -1, ufk24SourceId, constr);
        }

        #endregion Общие методы

        #region Работа с Txt

        private void PumpTxtRowSource(string[] rowValues, int refDate, string strLocaBudget)
        {
            int refFkDay = refDate;
            if (cachePeriod.ContainsKey(refDate))
                refFkDay = cachePeriod[refDate];

            if (strLocaBudget == string.Empty)
                strLocaBudget = rowValues[1];

            object[] mapping = new object[] {
                "ForPeriod", ConvertFactValue(rowValues[10]),
                "RefLocBdgt", PumpBudgetSources(strLocaBudget),
                "RefKIF", PumpKif(rowValues[8], rowValues[7]),
                "RefYearDayUNV", refDate,
                "RefFKDayUNV", refFkDay,
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
            if (this.DeleteEarlierData)
            {
                string constr = string.Format("CharacterData = {0}", CHARACTER_DATA);
                DirectDeleteFactData(new IFactTable[] { fctUFK24 }, -1, ufk24SourceId, constr);
            }
            base.DeleteEarlierPumpedData();
        }

        #endregion Перекрытые методы

        #endregion Закачка данных

        #region обработка

        protected void FillBudgetLevelSources()
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
                UpdateDataSources();
            }
        }

        #endregion

    }
}
