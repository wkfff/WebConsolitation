using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.MINZDRAV1Pump
{

    // МИНЗДРАВ - 0001 - Цены ЛС (Ежемесячные данные о ценах на лекарственные средства)
    public class MINZDRAV1PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // Цены ЛС.Лекарственные средства Амбулаторный (d_PriceMR_MRAmbulator)
        private IDbDataAdapter daMRAmbulator;
        private DataSet dsMRAmbulator;
        private IClassifier clsMRAmbulator;
        private Dictionary<string, int> cacheMRAmbulator = null;
        private int maxCodeMRAmbulator = -1;
        // Цены ЛС.Лекарственные средства Госпитальный (d_PriceMR_MRHospital)
        private IDbDataAdapter daMRHospital;
        private DataSet dsMRHospital;
        private IClassifier clsMRHospital;
        private Dictionary<string, int> cacheMRHospital = null;
        private int maxCodeMRHospital = -1;
        // Цены ЛС.Территории (d_PriceMR_Territory)
        private IDbDataAdapter daTerritory;
        private DataSet dsTerritory;
        private IClassifier clsTerritory;
        private Dictionary<string, int> cacheTerritory = null;
        private int maxCodeTerritory = -1;
        // Продукция.Лекарственные средства Сопоставимый (b_Production_MRBridge)
        private IDbDataAdapter daProduction;
        private DataSet dsProduction;
        private IClassifier clsProduction;
        private Dictionary<string, int> cacheProduction = null;
        private int maxCodeProduction = -1;
        private int? productionClsSourceID = -1;
        // Организации.Тип цены (d_Org_TypePrice)
        private IDbDataAdapter daTypePrice;
        private DataSet dsTypePrice;
        private IClassifier clsTypePrice;
        private Dictionary<string, int> cacheTypePrice = null;

        #endregion Классификаторы

        #region Факты

        // Цены ЛС.МИНЗДРАВСОЦРАЗВИТИЯ_Цены ЛС_Амбулаторный (f_PriceMR_MHPriceMRAmb)
        private IDbDataAdapter daPriceMRAmbulator;
        private DataSet dsPriceMRAmbulator;
        private IFactTable fctPriceMRAmbulator;
        private List<string> pumpedAmbulator = null;
        // Цены ЛС.МИНЗДРАВСОЦРАЗВИТИЯ_Цены ЛС_Госпитальный (f_PriceMR_MHPriceMRHos)
        private IDbDataAdapter daPriceMRHospital;
        private DataSet dsPriceMRHospital;
        private IFactTable fctPriceMRHospital;
        private List<string> pumpedHospital = null;

        #endregion Факты

        private ReportType reportType;
        private Dictionary<string, string> doseConversion;

        #endregion Поля

        #region Делегаты

        private delegate void PumpXlsDataRow(ExcelHelper excelDoc, int curRow, int refDate);

        #endregion Делегаты

        #region Константы

        private const string TYPE_PRICE_COST = "Отпускная цена";
        private const string TYPE_PRICE_TRADE = "Оптовая цена";
        private const string TYPE_PRICE_RETAIL = "Розничная цена";

        // Регулярное выражение для поиска дублирующихся пробелов
        private Regex regExDoubleSpace = new Regex(@"(\s[\s]+)", RegexOptions.IgnoreCase);

        #endregion Константы

        #region Структуры, перечисления

        private enum ReportType
        {
            Ambulant,
            Hospital
        }

        #endregion Структуры, перечисления

        #region Закачка данных

        #region Работа с базой и кэшами

        private int GetMaxCode(string tableName, int sourceId)
        {
            string query = string.Format(" select max(Code) from {0} ", tableName);
            if (sourceId != -1)
                query += string.Format(" where SourceID = {0} ", this.SourceID);
            object maxCode = this.DB.ExecQuery(query, QueryResultTypes.Scalar, new IDbDataParameter[] { });
            if ((maxCode == null) || (maxCode == DBNull.Value))
                return 0;
            return Convert.ToInt32(maxCode);
        }

        private void SetMaxCodes()
        {
            maxCodeMRAmbulator = GetMaxCode(clsMRAmbulator.FullDBName, this.SourceID);
            maxCodeMRHospital = GetMaxCode(clsMRHospital.FullDBName, this.SourceID);
            maxCodeTerritory = GetMaxCode(clsTerritory.FullDBName, -1);
            maxCodeProduction = GetMaxCode(clsProduction.FullDBName, -1);
        }

        private void FillCaches()
        {
            FillRowsCache(ref cacheMRAmbulator, dsMRAmbulator.Tables[0], new string[] { "Name", "IUN", "Producer" }, "|", "Id");
            FillRowsCache(ref cacheMRHospital, dsMRHospital.Tables[0], new string[] { "Name", "IUN", "Producer" }, "|", "Id");
            FillRowsCache(ref cacheTerritory, dsTerritory.Tables[0], "Name");
            FillRowsCache(ref cacheProduction, dsProduction.Tables[0], new string[] { "Name", "IUN", "Producer" }, "|", "Id");
            FillRowsCache(ref cacheTypePrice, dsTypePrice.Tables[0], "Name");
        }

        protected override void QueryData()
        {
            InitClsDataSet(ref daMRAmbulator, ref dsMRAmbulator, clsMRAmbulator);
            InitClsDataSet(ref daMRHospital, ref dsMRHospital, clsMRHospital);
            InitDataSet(ref daTerritory, ref dsTerritory, clsTerritory, string.Empty);
            productionClsSourceID = this.Scheme.DataVersionsManager.DataVersions.FindCurrentVersion(clsProduction.ObjectKey);
            InitDataSet(ref daProduction, ref dsProduction, clsProduction, string.Format("SourceID = {0}", productionClsSourceID));
            InitDataSet(ref daTypePrice, ref dsTypePrice, clsTypePrice, string.Empty);
            InitFactDataSet(ref daPriceMRAmbulator, ref dsPriceMRAmbulator, fctPriceMRAmbulator);
            InitFactDataSet(ref daPriceMRHospital, ref dsPriceMRHospital, fctPriceMRHospital);
            SetMaxCodes();
            FillCaches();
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daMRAmbulator, dsMRAmbulator, clsMRAmbulator);
            UpdateDataSet(daMRHospital, dsMRHospital, clsMRHospital);
            UpdateDataSet(daTerritory, dsTerritory, clsTerritory);
            UpdateDataSet(daProduction, dsProduction, clsProduction);
            UpdateDataSet(daPriceMRAmbulator, dsPriceMRAmbulator, fctPriceMRAmbulator);
            UpdateDataSet(daPriceMRHospital, dsPriceMRHospital, fctPriceMRHospital);
        }

        #region GUIDs

        private const string D_MR_AMBULATOR_GUID = "84d0063b-2bb9-4589-bbaa-789e25e434ed";
        private const string D_MR_HOSPITAL_GUID = "4872502b-1b73-414d-bec3-bec396598b89";
        private const string D_TERRITORY_GUID = "2aefb01d-79e0-4473-b78c-276e4441d5af";
        private const string B_PRODUCTION_GUID = "3e4b2b33-561f-49ac-bd20-c874f7d41941";
        private const string D_TYPE_PRICE_GUID = "ab4797b0-2729-4a0b-86fa-0158f179f147";
        private const string F_PRICE_MR_AMBULATOR_GUID = "9586ccc3-1c84-4b33-a459-c1dfe8c6e7eb";
        private const string F_PRICE_MR_HOSPITAL_GUID = "f7819a9e-0019-496f-81cc-52dae691bcd4";
        
        #endregion GUIDs
        protected override void InitDBObjects()
        {
            clsTerritory = this.Scheme.Classifiers[D_TERRITORY_GUID];
            clsProduction = this.Scheme.Classifiers[B_PRODUCTION_GUID];
            clsTypePrice = this.Scheme.Classifiers[D_TYPE_PRICE_GUID];

            this.UsedClassifiers = new IClassifier[] {
                clsMRAmbulator = this.Scheme.Classifiers[D_MR_AMBULATOR_GUID],
                clsMRHospital = this.Scheme.Classifiers[D_MR_HOSPITAL_GUID],
            };

            this.UsedFacts = new IFactTable[] {
                fctPriceMRAmbulator = this.Scheme.FactTables[F_PRICE_MR_AMBULATOR_GUID],
                fctPriceMRHospital = this.Scheme.FactTables[F_PRICE_MR_HOSPITAL_GUID]
            };

            this.AssociateClassifiersEx = new IClassifier[] { clsTerritory };
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsMRAmbulator);
            ClearDataSet(ref dsMRHospital);
            ClearDataSet(ref dsTerritory);
            ClearDataSet(ref dsProduction);
            ClearDataSet(ref dsTypePrice);
            ClearDataSet(ref dsPriceMRAmbulator);
            ClearDataSet(ref dsPriceMRHospital);
        }

        #endregion Работа с базой и кэшами

        #region Общие методы

        private void FillDoseConversion()
        {
            doseConversion.Add("КАПСУЛА КИШЕЧНОРАСТВОРИМАЯ", "капсулы кишечнорастворимые");
            doseConversion.Add("КАПСУЛА", "капсулы");
            doseConversion.Add("ТАБЛЕТКА ВАГИНАЛЬНАЯ", "таблетки вагинальные");
            doseConversion.Add("ТАБЛЕТКА ДИСПЕРГИРУЕМАЯ", "таблетки диспергируемые");
            doseConversion.Add("ТАБЛЕТКА ЖЕВАТЕЛЬНАЯ", "таблетки жевательные");
            doseConversion.Add("ТАБЛЕТКА ПОКРЫТАЯ", "таблетки покрытые");
            doseConversion.Add("ТАБЛЕТКА ПОДЪЯЗЫЧНАЯ", "таблетки подъязычные");
            doseConversion.Add("ТАБЛЕТКА", "таблетки");
        }

        private string ConvertDoseValue(string dose)
        {
            foreach (KeyValuePair<string, string> conversion in doseConversion)
            {
                int startPosition = dose.ToUpper().IndexOf(conversion.Key);
                if (startPosition >= 0)
                {
                    dose = dose.Remove(startPosition, conversion.Key.Length).
                        Insert(startPosition, conversion.Value);
                }
            }
            return dose;
        }

        private string CleanNameValue(string name)
        {
            name = name.Replace("~", string.Empty);
            return regExDoubleSpace.Replace(name, " ");
        }

        private int GetReportDate()
        {
            string month = this.DataSource.Variant.ToUpper();
            for (int monthIndex = 0; monthIndex < CommonRoutines.MonthByNumber.GetLength(0); monthIndex++)
            {
                if (CommonRoutines.MonthByNumber[monthIndex].ToUpper() == month)
                {
                    return (this.DataSource.Year * 10000 + (monthIndex + 1) * 100);
                }
            }
            return -1;
        }

        private int GetRefTypePrice(string name)
        {
            if (cacheTypePrice.ContainsKey(name))
                return cacheTypePrice[name];
            return -1;
        }

        private decimal CleanFactValue(string factValue)
        {
            factValue = CommonRoutines.TrimLetters(factValue.Trim().Replace('.', ','));
            return Convert.ToDecimal(factValue.PadLeft(1, '0'));
        }

        private void ProcessAllFiles(DirectoryInfo dir)
        {
            doseConversion = new Dictionary<string, string>();
            pumpedAmbulator = new List<string>();
            pumpedHospital = new List<string>();
            try
            {
                FillDoseConversion();
                ProcessFilesTemplate(dir, "*.xls", new ProcessFileDelegate(PumpXlsFile), false);
            }
            finally
            {
                doseConversion.Clear();
                pumpedAmbulator.Clear();
                pumpedHospital.Clear();
            }
        }

        #endregion Общие методы

        #region Работа с Excel

        private void PumpProductionBridge(string name, string iun, string dose, string producer)
        {
            string key = string.Format("{0}|{1}|{2}", name, iun, producer);
            if (cacheProduction.ContainsKey(key))
                return;

            maxCodeProduction++;
            object[] mapping = new object[] {
                "Code", maxCodeProduction, "Name", name, "Shortname", name, "IUN", iun,
                "Dose", dose, "Producer", producer, "SourceId", productionClsSourceID };
            PumpCachedRow(cacheProduction, dsProduction.Tables[0], clsProduction, key, mapping);
        }

        private int PumpXlsTerritory(ExcelHelper excelDoc, int curRow)
        {
            string name = excelDoc.GetValue(curRow, 2).Trim();
            if (cacheTerritory.ContainsKey(name))
                return cacheTerritory[name];

            maxCodeTerritory++;
            object[] mapping = new object[] { "Code", maxCodeTerritory, "Name", name, "RefRFBridge", -1 };
            return PumpCachedRow(cacheTerritory, dsTerritory.Tables[0], clsTerritory, name, mapping);
        }

        #region Амбулаторный сегмент

        private string GetMRAmbulantDose(ExcelHelper excelDoc, int curRow)
        {
            List<string> components = new List<string>();

            string cellValue = excelDoc.GetValue(curRow, 10).Trim();
            if (cellValue != string.Empty)
                components.Add(cellValue);
            cellValue = excelDoc.GetValue(curRow, 5).Trim();
            if (cellValue != string.Empty)
                components.Add(cellValue);

            // проверяем, не дублируется ли информация в 5 и в 12+13 столбцах
            string dose = string.Format("{0} {1}", excelDoc.GetValue(curRow, 12).Trim(),
                excelDoc.GetValue(curRow, 13).Trim());
            if (cellValue != dose)
            {
                cellValue = excelDoc.GetValue(curRow, 12).Trim();
                if (cellValue != string.Empty)
                    components.Add(cellValue);
                cellValue = excelDoc.GetValue(curRow, 13).Trim();
                if (cellValue != string.Empty)
                    components.Add(cellValue);
            }

            cellValue = excelDoc.GetValue(curRow, 11).Trim();
            if (cellValue != string.Empty)
                components.Add(string.Concat("№ ", cellValue));

            return ConvertDoseValue(string.Join(" ", components.ToArray()));
        }

        private int PumpXlsMRAmbulant(ExcelHelper excelDoc, int curRow)
        {
            string iun = excelDoc.GetValue(curRow, 3).Trim();
            string name = excelDoc.GetValue(curRow, 4).Trim();
            string dose = GetMRAmbulantDose(excelDoc, curRow);
            if (dose != string.Empty)
                name = string.Format("{0}, {1}", name, dose);
            name = CleanNameValue(name);
            dose = CleanNameValue(dose);
            string producer = excelDoc.GetValue(curRow, 6).Trim();
            string country = excelDoc.GetValue(curRow, 7).Trim();
            if (country != string.Empty)
                producer = string.Format("{0} ({1})", producer, country);

            PumpProductionBridge(name, iun, dose, producer);

            string key = string.Format("{0}|{1}|{2}", name, iun, producer);
            if (cacheMRAmbulator.ContainsKey(key))
                return cacheMRAmbulator[key];

            maxCodeMRAmbulator++;
            object[] mapping = new object[] { "Code", maxCodeMRAmbulator, "Name", name,
                "IUN", iun, "Dose", dose, "Producer", producer, "RefMRBridge", -1 };
            return PumpCachedRow(cacheMRAmbulator, dsMRAmbulator.Tables[0], clsMRAmbulator, key, mapping);
        }

        private void PumpAmbulantFact(string valueField, decimal factValue, int refTypePrice, object[] mapping)
        {
            if (factValue == 0)
                return;
            mapping = (object[])CommonRoutines.ConcatArrays(mapping,
                new object[] { valueField, factValue, "RefTypePrice", refTypePrice });
            PumpRow(dsPriceMRAmbulator.Tables[0], mapping);
        }

        private void PumpXlsAmbulantDataRow(ExcelHelper excelDoc, int curRow, int refDate)
        {
            int refMRAmb = PumpXlsMRAmbulant(excelDoc, curRow);
            int refTerritory = PumpXlsTerritory(excelDoc, curRow);

            // для 1-го ЛС на 1-ой территории могут попадаться несколько записей фактов
            // в таком случае оставляем только первую из них, а остальные пропускаем
            string pumpedKey = string.Format("{0}|{1}", refMRAmb, refTerritory);
            if (pumpedAmbulator.Contains(pumpedKey))
                return;
            pumpedAmbulator.Add(pumpedKey);

            object[] mapping = new object[] {
                "RefUNV", refDate, "RefManufacturer", -1,
                "RefMRAmb", refMRAmb, "RefTerritory", refTerritory };

            PumpAmbulantFact("Price", CleanFactValue(excelDoc.GetValue(curRow, 14)),
                GetRefTypePrice(TYPE_PRICE_RETAIL), mapping);
            PumpAmbulantFact("Price", CleanFactValue(excelDoc.GetValue(curRow, 15)),
                GetRefTypePrice(TYPE_PRICE_TRADE), mapping);
            PumpAmbulantFact("Price", CleanFactValue(excelDoc.GetValue(curRow, 16)),
                GetRefTypePrice(TYPE_PRICE_COST), mapping);
            PumpAmbulantFact("Markup", CleanFactValue(excelDoc.GetValue(curRow, 17)),
                GetRefTypePrice(TYPE_PRICE_RETAIL), mapping);
            PumpAmbulantFact("Markup", CleanFactValue(excelDoc.GetValue(curRow, 18)),
                GetRefTypePrice(TYPE_PRICE_TRADE), mapping);

            if (dsPriceMRAmbulator.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daPriceMRAmbulator, ref dsPriceMRAmbulator);
            }
        }

        #endregion Амбулаторный сегмент

        #region Госпитальный сегмент

        private int PumpXlsMRHospital(ExcelHelper excelDoc, int curRow)
        {
            string iun = excelDoc.GetValue(curRow, 3).Trim();
            string name = excelDoc.GetValue(curRow, 4).Trim();
            string dose = excelDoc.GetValue(curRow, 7).Trim();
            if (dose != string.Empty)
            {
                dose = ConvertDoseValue(dose.Trim(new char[] { '№' }).Trim());
                name = string.Format("{0}, {1}", name, dose);
            }
            name = CleanNameValue(name);
            dose = CleanNameValue(dose);
            string producer = excelDoc.GetValue(curRow, 5).Trim();
            string country = excelDoc.GetValue(curRow, 6).Trim();
            if (country != string.Empty)
                producer = string.Format("{0} ({1})", producer, country);
            
            PumpProductionBridge(name, iun, dose, producer);

            string key = string.Format("{0}|{1}|{2}", name, iun, producer);
            if (cacheMRHospital.ContainsKey(key))
                return cacheMRHospital[key];

            maxCodeMRHospital++;
            object[] mapping = new object[] { "Code", maxCodeMRHospital, "Name", name,
                "IUN", iun, "Dose", dose, "Producer", producer, "RefMRBridge", -1 };
            return PumpCachedRow(cacheMRHospital, dsMRHospital.Tables[0],
                clsMRHospital, key, mapping);
        }

        private void PumpXlsHospitalDataRow(ExcelHelper excelDoc, int curRow, int refDate)
        {
            int refMRHospital = PumpXlsMRHospital(excelDoc, curRow);
            int refTerritory = PumpXlsTerritory(excelDoc, curRow);

            // для 1-го ЛС на 1-ой территории могут попадаться несколько записей фактов
            // в таком случае оставляем только первую из них, а остальные пропускаем
            string pumpedKey = string.Format("{0}|{1}", refMRHospital, refTerritory);
            if (pumpedHospital.Contains(pumpedKey))
                return;
            pumpedHospital.Add(pumpedKey);

            decimal price = CleanFactValue(excelDoc.GetValue(curRow, 8));
            if (price == 0)
                return;

            int refTypePrice = GetRefTypePrice(TYPE_PRICE_TRADE);
            object[] mapping = new object[] { "Price", price, "RefUNV", refDate, "RefManufacturer", -1,
                "RefMRHospital", refMRHospital, "RefTerritory", refTerritory, "RefTypePrice", refTypePrice };

            PumpRow(dsPriceMRHospital.Tables[0], mapping);
            if (dsPriceMRHospital.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daPriceMRHospital, ref dsPriceMRHospital);
            }
        }

        #endregion Госпитальный сегмент

        private void PumpXlsReport(string filename, ExcelHelper excelDoc, PumpXlsDataRow pumpXlsDataRow, int refDate)
        {
            string dataSourcePath = GetShortSourcePathBySourceID(this.SourceID);
            int rowsCount = excelDoc.GetRowsCount();
            for (int curRow = 3; curRow <= rowsCount; curRow++)
            {
                try
                {
                    SetProgress(rowsCount, curRow,
                        string.Format("Обработка файла {0}\\{1}...", dataSourcePath, filename),
                        string.Format("Строка {0} из {1}", curRow, rowsCount));

                    if (excelDoc.GetValue(curRow, 1).Trim() == string.Empty)
                        continue;

                    pumpXlsDataRow(excelDoc, curRow, refDate);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("При обработке строки {0} возникла ошибка ({1})", curRow, ex.Message), ex);
                }
            }
        }

        private void PumpXlsFile(FileInfo file)
        {
            ExcelHelper excelDoc = new ExcelHelper();
            try
            {
                excelDoc.AskToUpdateLinks = false;
                excelDoc.DisplayAlerts = false;
                excelDoc.OpenDocument(file.FullName);
                excelDoc.SetWorksheet(1);
                int refDate = GetReportDate();
                switch (reportType)
                {
                    case ReportType.Ambulant:
                        PumpXlsReport(file.Name, excelDoc, PumpXlsAmbulantDataRow, refDate);
                        break;
                    case ReportType.Hospital:
                        PumpXlsReport(file.Name, excelDoc, PumpXlsHospitalDataRow, refDate);
                        break;
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

        private const string CONST_AMBULANT_DIR_NAME = "Амбулаторный";
        private const string CONST_HOSPITAL_DIR_NAME = "Госпитальный";
        protected override void ProcessFiles(DirectoryInfo dir)
        {
            if (dir.GetDirectories(CONST_AMBULANT_DIR_NAME).GetLength(0) > 0)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping,
                    "Старт закачки данных амбулаторного сегмента.");
                reportType = ReportType.Ambulant;
                ProcessAllFiles(dir.GetDirectories(CONST_AMBULANT_DIR_NAME)[0]);
            }
            if (dir.GetDirectories(CONST_HOSPITAL_DIR_NAME).GetLength(0) > 0)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping,
                    "Старт закачки данных госпитального сегмента.");
                reportType = ReportType.Hospital;
                ProcessAllFiles(dir.GetDirectories(CONST_HOSPITAL_DIR_NAME)[0]);
            }
        }

        protected override void DirectPumpData()
        {
            PumpDataYVTemplate();
        }

        #endregion Перекрытые методы закачки

        #endregion Закачка данных

    }

}
