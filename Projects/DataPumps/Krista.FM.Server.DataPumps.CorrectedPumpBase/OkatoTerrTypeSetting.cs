using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataPumps
{
    // Класс для расщепления сумм фактов по нормативам отчислений доходов

    /// <summary>
    /// Базовый класс для всех закачек.
    /// </summary>
    public abstract partial class CorrectedPumpModuleBase : DataPumpModuleBase
    {
        #region Поля

        // Коллекция пропущенных кодов ОКАТО
        protected List<string> badOkatoCodesCache = new List<string>(100);
        // список окато с неуказанным типом территории
        protected List<string> nullTerrTypeOkatoCodesCache = new List<string>(100);
        protected Dictionary<int, DataRow> okatoCache = null;
        protected Dictionary<int, string> okatoCodesCache = null;
        protected Dictionary<string, string> okatoBridgeCache = null;

        // Районы.Служебный
        protected IClassifier clsRegionsForPump;
        protected IDbDataAdapter daRegionsForPump = null;
        protected DataSet dsRegionsForPump = null;
        protected int regionsForPumpSourceID;
        protected Dictionary<string, DataRow> regionsForPumpCache = null; 

        #endregion Поля

        /// <summary>
        /// Проверка на наличие неуказанного типа территории у классификатора Районы.Служебный
        /// </summary>
        private void CheckRefTerrType(DataSet dsRegionsForPump)
        {
            List<string> nullTerrType = new List<string>();

            foreach (DataRow row in dsRegionsForPump.Tables[0].Rows)
            {
                if (row["REFTERRTYPE"].ToString() == "0")
                    nullTerrType.Add(string.Format("{0} ({1})", row["NAME"].ToString(), row["OKATO"].ToString()));
            }

            if (nullTerrType.Count > 0)
            {
                string regions = string.Join(", ", nullTerrType.ToArray());
                WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeWarning, 
                    string.Format("Классификатор 'Районы.Служебный для закачки' имеет районы с неуказанным типом территории. " +
                                  "Заполните в этом классификаторе поле \"Тип территории\" и запустите этап обработки еще раз. " +
                                  "Всего кодов: {0}. Список наименований районов и кодов ОКАТО: \n{1}.",
                                  nullTerrType.Count, regions));
            }
        }
        
        /// <summary>
        /// Подготовка данных классификатора Районы.Служебный
        /// </summary>
        private const string D_REGIONS_FOR_PUMP_GUID = "e9d2898d-fc2d-4626-834a-ed1ac98a1673";
        protected void PrepareRegionsForSumDisint()
        {
            clsRegionsForPump = this.Scheme.Classifiers[D_REGIONS_FOR_PUMP_GUID];

            regionsForPumpSourceID = AddDataSource("ФО", "0006", ParamKindTypes.Year, string.Empty,
                this.DataSource.Year, 0, string.Empty, 0, string.Empty).ID;

            InitDataSet(ref daRegionsForPump, ref dsRegionsForPump, clsRegionsForPump, false,
                string.Format("SOURCEID = {0}", regionsForPumpSourceID), string.Empty);

            FillRowsCache(ref regionsForPumpCache, dsRegionsForPump.Tables[0], new string[] { "OKATO" });

            CheckRefTerrType(dsRegionsForPump);
        }

        /// <summary>
        /// Подготавливает данные ОКАТО
        /// </summary>
        private const string B_OKATO_BRIDGE_GUID = "ba98ebef-0b02-4548-9766-c1e8bc2e55e4";
        protected void PrepareOkatoForSumDisint(IClassifier clsOKATO)
        {
            InitClsDataSet(ref daOKATO, ref dsOKATO, clsOKATO, false, string.Empty);
            FillRowsCache(ref okatoCache, dsOKATO.Tables[0], "ID");
            FillRowsCache(ref okatoCodesCache, dsOKATO.Tables[0], "ID", "CODE");

            brdOKATO = this.Scheme.Classifiers[B_OKATO_BRIDGE_GUID];
            InitDataSet(ref daOKATOBridge, ref dsOKATOBridge, brdOKATO, true, "CODE > 0", string.Empty);
            FillRowsCache(ref okatoBridgeCache, dsOKATOBridge.Tables[0], "CODE", "NAME");
            ClearDataSet(ref dsOKATOBridge);
        }

        /// <summary>
        /// Инициализация кэша кодов ОКАТО, не найденных в классификаторе Районы.Служебный
        /// </summary>
        protected void PrepareBadOkatoCodesCache()
        {
            if (badOkatoCodesCache != null)
                badOkatoCodesCache.Clear();
            if (nullTerrTypeOkatoCodesCache != null)
                nullTerrTypeOkatoCodesCache.Clear();
        }

        public void WriteToBadOkatoCodesCache(List<string> cache, string code)
        {
            if (!cache.Contains(code))
                cache.Add(code);
        }

        /// <summary>
        /// Записывает в лог данные о пропущенных кодах ОКАТО
        /// </summary>
        protected void WriteBadOkatoCodesCacheToBD()
        {
            string accountStr = string.Empty;
            string msg = string.Empty;
            if (this.PumpProgramID == PumpProgramID.TaxesRegulationDataPump)
                accountStr = "и \"Расчетный счет\" (начиная с 2006 года) ";
            if (nullTerrTypeOkatoCodesCache.Count > 0)
            {
                string regionsCode = string.Join(", ", nullTerrTypeOkatoCodesCache.ToArray());
                if (this.PumpProgramID == PumpProgramID.TaxesRegulationDataPump)
                    accountStr = "и \"Расчетный счет\" (начиная с 2006 года) ";
                msg = string.Format("Классификатор 'Районы.Служебный для закачки' имеет районы с неуказанным типом территории. " +
                                    "Заполните в этом классификаторе поле \"Тип территории\" " + accountStr +
                                    "и запустите этап обработки еще раз. " +
                                    "Всего кодов: {0}. Список кодов ОКАТО: \n{1}.",
                                    nullTerrTypeOkatoCodesCache.Count, regionsCode);
                WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeWarning, msg);
            }
        }

        private void SetTerrType(DataRow okatoRow)
        {
            DataRow regionsForPumpRow = null;
            string okatoCode = okatoRow["Code"].ToString();
            if (regionsForPumpCache.ContainsKey(okatoCode))
            {
                regionsForPumpRow = regionsForPumpCache[okatoCode];
            }
            else
            {
                regionsForPumpRow = PumpCachedRow(regionsForPumpCache, dsRegionsForPump.Tables[0], clsRegionsForPump,
                    okatoCode, new object[] { "SOURCEID", regionsForPumpSourceID, "OKATO", okatoCode, 
                    "NAME", FindCachedRow(okatoBridgeCache, okatoCode, constDefaultClsName), "REFTERRTYPE", 0 }, false);
                WriteToBadOkatoCodesCache(badOkatoCodesCache, okatoCode);
            }
            int terrType = Convert.ToInt32(regionsForPumpRow["REFTERRTYPE"]);
            okatoRow["REFTERRTYPE"] = terrType;
            if (terrType == 0)
                WriteToBadOkatoCodesCache(nullTerrTypeOkatoCodesCache, okatoCode);
            if (this.PumpProgramID == PumpProgramID.TaxesRegulationDataPump)
                okatoRow["DUTYACCOUNT"] = regionsForPumpRow["ACCOUNT"];
        }

        // Устанавливает признак территории у ОКАТО и возвращает его
        protected DataRow GetOkatoRow(int okatoID)
        {
            if (!okatoCache.ContainsKey(okatoID))
                return null;
            DataRow row = okatoCache[okatoID];
            SetTerrType(row);
            return row;
        }

        private void AddOkatoToRegionsForPump()
        {
            foreach (DataRow row in dsOKATO.Tables[0].Rows)
                SetTerrType(row);
        }

        /// <summary>
        /// Сохраняет данные ОКАТО
        /// </summary>
        protected void UpdateOkatoData()
        {
            UpdateDataSet(daRegionsForPump, dsRegionsForPump, clsRegionsForPump);
            UpdateDataSet(daOKATO, dsOKATO, clsOKATO);
        }
    }
}