using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using Krista.FM.Server.DataPumps;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.ORG3Pump
{

    // Организации - 0003 - Цены и тарифы
    public class ORG3PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // Организации.Реестр организаций (d_Org_RegistrOrg)
        private IDbDataAdapter daOrg;
        private DataSet dsOrg;
        private IClassifier clsOrg;
        private Dictionary<string, int> cacheOrg = null;
        private int maxOrgCode = 0;
        // Организации.Товары и услуги (d_Org_Good)
        private IDbDataAdapter daGood;
        private DataSet dsGood;
        private IClassifier clsGood;
        private Dictionary<string, int> cacheGood = null;
        private int maxGoodCode = 0;
        // Организации.Тип цены (d_Org_TypePrice)
        private IDbDataAdapter daTypePrice;
        private DataSet dsTypePrice;
        private IClassifier clsTypePrice;
        private Dictionary<string, int> cacheTypePrice = null;
        private int maxTypePrice = 0;
        // Территории.РФ (d_Territory_RF)
        private IDbDataAdapter daTerritory;
        private DataSet dsTerritory;
        private IClassifier clsTerritory;
        private Dictionary<string, int> cacheTerritory = null;
        private int nullTerritory = -1;

        #endregion Классификаторы

        #region Факты

        // Организации.Организации_Цены и тарифы (f_Org_Price)
        private IDbDataAdapter daPrice;
        private DataSet dsPrice;
        private IFactTable fctPrice;

        #endregion Факты

        private List<int> deletedDatesList = null;
        private List<string> absentTerritories = null;

        #endregion Поля

        #region Закачка данных

        #region Работа с базой кэшами

        private int GetMaxCode(IEntity cls)
        {
            string query = string.Format("select max(Code) from {0}", cls.FullDBName);
            object result = this.DB.ExecQuery(query, QueryResultTypes.Scalar, new IDbDataParameter[] { });
            if ((result == null) || (result == DBNull.Value))
                return 0;
            return Convert.ToInt32(result);
        }

        // регулярное выражение для удаления лишних пробелов
        private Regex regexExtraSpaces = new Regex(@"(\s\s+)", RegexOptions.Compiled | RegexOptions.Multiline);
        private void FillCacheIgnoreCase(ref Dictionary<string, int> cache, DataTable dt, string keyField, bool territorySpecial)
        {
            cache = new Dictionary<string, int>();
            foreach (DataRow row in dt.Rows)
            {
                if (row.RowState != DataRowState.Deleted)
                {
                    string key = Convert.ToString(row[keyField]).ToUpper();
                    key = regexExtraSpaces.Replace(key, " ");
                    if (!cache.ContainsKey(key))
                        cache.Add(key, Convert.ToInt32(row["ID"]));
                    if (territorySpecial)
                    {
                        key = key.Trim();
                        if (!cache.ContainsKey(key))
                            cache.Add(key, Convert.ToInt32(row["ID"]));
                        key = key.Replace("Г.", string.Empty).Trim();
                        if (!cache.ContainsKey(key))
                            cache.Add(key, Convert.ToInt32(row["ID"]));
                    }
                }
            }
        }

        private void FillCaches()
        {
            FillCacheIgnoreCase(ref cacheOrg, dsOrg.Tables[0], "NameOrg", false);
            FillCacheIgnoreCase(ref cacheGood, dsGood.Tables[0], "Name", false);
            FillCacheIgnoreCase(ref cacheTerritory, dsTerritory.Tables[0], "Name", true);
            FillRowsCache(ref cacheTypePrice, dsTypePrice.Tables[0], "Name");
        }

        protected override void QueryData()
        {
            InitDataSet(ref daOrg, ref dsOrg, clsOrg, string.Empty);
            InitDataSet(ref daGood, ref dsGood, clsGood, string.Empty);
            InitDataSet(ref daTypePrice, ref dsTypePrice, clsTypePrice, string.Empty);
            InitDataSet(ref daTerritory, ref dsTerritory, clsTerritory, string.Empty);
            InitFactDataSet(ref daPrice, ref dsPrice, fctPrice);

            FillCaches();
            maxOrgCode = GetMaxCode(clsOrg);
            maxGoodCode = GetMaxCode(clsGood);
            maxTypePrice = GetMaxCode(clsTypePrice);
        }

        #region GUIDs

        private const string D_ORG_REGISTR_ORG_GUID = "c924d846-afdc-4f08-8e64-c572eec75405";
        private const string D_ORG_GOOD_GUID = "cdb1abde-8f13-47b5-9d10-9175d1b217f7";
        private const string D_ORG_TYPEPRICE_GUID = "ab4797b0-2729-4a0b-86fa-0158f179f147";
        private const string D_TERRITORY_RF_GUID = "66b9a66d-85ca-41de-910e-f9e6cb483960";
        private const string F_ORG_PRICE_GUID = "62b88e8a-5b2c-4e7c-b3b5-eaa71ac31b31";

        #endregion GUIDs
        protected override void InitDBObjects()
        {
            clsOrg = this.Scheme.Classifiers[D_ORG_REGISTR_ORG_GUID];
            clsGood = this.Scheme.Classifiers[D_ORG_GOOD_GUID];
            clsTypePrice = this.Scheme.Classifiers[D_ORG_TYPEPRICE_GUID];
            clsTerritory = this.Scheme.Classifiers[D_TERRITORY_RF_GUID];
            fctPrice = this.Scheme.FactTables[F_ORG_PRICE_GUID];

            this.UsedClassifiers = new IClassifier[] { };
            this.UsedFacts = new IFactTable[] { fctPrice };
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daOrg, dsOrg, clsOrg);
            UpdateDataSet(daGood, dsGood, clsGood);
            UpdateDataSet(daTypePrice, dsTypePrice, clsTypePrice);
            UpdateDataSet(daPrice, dsPrice, fctPrice);
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsPrice);
            ClearDataSet(ref dsOrg);
            ClearDataSet(ref dsGood);
            ClearDataSet(ref dsTypePrice);
            ClearDataSet(ref dsTerritory);
        }

        #endregion Работа с базой кэшами

        #region Работа с Xls

        private decimal CleanFactValue(string value)
        {
            decimal factValue = 0;
            Decimal.TryParse(CommonRoutines.TrimLetters(value).Replace('.', ','), out factValue);
            return factValue;
        }

        private int PumpXlsOrg(ExcelHelper excelDoc, int curRow)
        {
            string name = excelDoc.GetValue(curRow, 3).Trim();
            string key = regexExtraSpaces.Replace(name.ToUpper(), " ");
            if (cacheOrg.ContainsKey(key))
                return cacheOrg[key];
            maxOrgCode++;
            return PumpCachedRow(cacheOrg, dsOrg.Tables[0], clsOrg, key, new object[] {
                "Code", maxOrgCode, "NameOrg", name, "RefOK", -1, "RefOKOKFS", -1, "RefOrg", -1, "RefRegionAn", -1 });
        }

        private int PumpXlsGood(ExcelHelper excelDoc, int curRow)
        {
            string name = excelDoc.GetValue(curRow, 4).Trim();
            string key = regexExtraSpaces.Replace(name.ToUpper(), " ");
            if (cacheGood.ContainsKey(key))
                return cacheGood[key];
            maxGoodCode++;
            return PumpCachedRow(cacheGood, dsGood.Tables[0], clsGood, key, new object[] {
                "Code", maxGoodCode, "Name", name, "RefUnits", -1 });
        }

        private int PumpXlsTypePrice(string name)
        {
            if (cacheTypePrice.ContainsKey(name))
                return cacheTypePrice[name];
            maxTypePrice++;
            return PumpCachedRow(cacheTypePrice, dsTypePrice.Tables[0], clsTypePrice, name,
                new object[] { "Code", maxTypePrice, "Name", name });
        }

        private int GetTerritoryRef(string name)
        {
            if (cacheTerritory.ContainsKey(name.ToUpper()))
                return cacheTerritory[name.ToUpper()];
            if (!absentTerritories.Contains(name))
                absentTerritories.Add(name);
            return nullTerritory;
        }

        private void PumpXlsSheet(ExcelHelper excelDoc, int refDate)
        {
            int rowsCount = excelDoc.GetRowsCount();
            for (int curRow = 2; curRow <= rowsCount; curRow++)
                try
                {
                    decimal factValue = CleanFactValue(excelDoc.GetValue(curRow, 5));
                    if (factValue == 0)
                        continue;

                    object[] mapping = new object[] {
                        "Price", factValue,
                        "RefDay", refDate,
                        "RefOrgRegistrOrg", PumpXlsOrg(excelDoc, curRow), 
                        "RefGoodOrg", PumpXlsGood(excelDoc, curRow),
                        "RefTerritory", GetTerritoryRef(excelDoc.GetValue(curRow, 2).Trim()),
                        "RefOrg", PumpXlsTypePrice("Розничная цена")
                    };

                    PumpRow(dsPrice.Tables[0], mapping);
                    if (dsPrice.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
                    {
                        UpdateData();
                        ClearDataSet(daPrice, ref dsPrice);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format(
                        "При обработке строки {0} листа '{1}' возникла ошибка ({2})",
                        curRow, excelDoc.GetWorksheetName(), ex.Message), ex);
                }
        }

        // дату берем из названия файла, например ММДД.xlsx
        private int GetRefDate(string filename)
        {
            filename = CommonRoutines.TrimLetters(filename);
            return this.DataSource.Year * 10000 + Convert.ToInt32(filename.Substring(0, 2)) * 100 +
                Convert.ToInt32(filename.Substring(2, 2));
        }

        private void DeleteEarlierDataByDate(int refDate)
        {
            if (!deletedDatesList.Contains(refDate))
            {
                // удаляем только те записи, где заполнена мера "Цена"
                string constraint = string.Format("RefDay = {0} and not Price is null", refDate);
                DirectDeleteFactData(new IFactTable[] { fctPrice }, -1, this.SourceID, constraint);
                deletedDatesList.Add(refDate);
            }
        }

        private bool IsSkipFile(string filename)
        {
            if (filename.StartsWith("__") || filename.Contains("~"))
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                    "Файл '{0}' будет пропущен.", filename));
                return true;
            }
            return false;
        }

        private void PumpXlsFile(FileInfo file)
        {
            WriteToTrace("Открытие документа: " + file.Name, TraceMessageKind.Information);
            ExcelHelper excelDoc = new ExcelHelper();
            try
            {
                if (IsSkipFile(file.Name))
                    return;
                excelDoc.AskToUpdateLinks = false;
                excelDoc.DisplayAlerts = false;
                excelDoc.EnableEvents = false;
                excelDoc.OpenDocument(file.FullName);

                int refDate = GetRefDate(file.Name);
                DeleteEarlierDataByDate(refDate);
                excelDoc.SetWorksheet(1);
                PumpXlsSheet(excelDoc, refDate);
            }
            finally
            {
                if (excelDoc != null)
                    excelDoc.CloseDocument();
            }
        }

        #endregion Работа с Xls

        #region Перекрытые методы

        protected override void DeleteEarlierPumpedData()
        {
            // заглушка
        }

        private void ShowAbsentTerritorues()
        {
            if (absentTerritories.Count > 0)
            {
                absentTerritories.Sort();
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                    "Не удалось установить ссылку на территории: {0}.", string.Join(", ", absentTerritories.ToArray())));
            }
        }

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            absentTerritories = new List<string>();
            deletedDatesList = new List<int>();
            try
            {
                ProcessFilesTemplate(dir, "*.xls", new ProcessFileDelegate(PumpXlsFile), false);
                ShowAbsentTerritorues();
            }
            finally
            {
                absentTerritories.Clear();
                deletedDatesList.Clear();
            }
        }

        protected override void DirectPumpData()
        {
            PumpDataYTemplate();
        }

        #endregion Перекрытые методы

        #endregion Закачка данных

    }

}
