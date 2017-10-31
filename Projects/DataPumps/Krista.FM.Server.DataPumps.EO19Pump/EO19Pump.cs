using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using Krista.FM.Server.DataPumps;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.EO19Pump
{

    // ЕО - 0019 - Мониторинг строительства
    public class EO19PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // Строительство.Заказчики (d_Build_Customer)
        private IDbDataAdapter daCustomer;
        private DataSet dsCustomer;
        private IClassifier clsCustomer;
        private Dictionary<string, int> cacheCustomer = null;
        // Строительство.Ход работ (d_Build_StrWorks)
        private IDbDataAdapter daStrWorks;
        private DataSet dsStrWorks;
        private IClassifier clsStrWorks;
        private Dictionary<string, int> cacheStrWorks = null;
        private Dictionary<int, int> strWorksColumns = null;
        // Строительство.Характеристика объектов (d_Build_Contract)
        private IDbDataAdapter daContract;
        private DataSet dsContract;
        private IClassifier clsContract;
        private Dictionary<string, int> cacheContract = null;
        // Территории.РФ (d_Territory_RF)
        private IDbDataAdapter daTettitory;
        private DataSet dsTerritory;
        private IClassifier clsTerritory;

        #endregion Классификаторы

        #region Факты

        // Строительство.ЭО_Мониторинг строительства_Объекты строительства (f_Build_BuildObj)
        private IDbDataAdapter daBuildObj;
        private DataSet dsBuildObj;
        private IFactTable fctBuildObj;

        #endregion Факты

        private int refDate = -1;
        private int maxCustomerCode = 0;
        private int maxStrWorksCode = 0;
        private int maxContractCode = 0;
        private int lastRefCustomer = -1;

        #endregion Поля

        #region Структуры

        private struct FactValue
        {
            public double Value;
            public string EI;

            public FactValue(double value, string ei)
            {
                this.Value = value;
                this.EI = ei;
            }
        }

        #endregion Структуры

        #region Закачка данных

        #region Работа с базой и кэшами

        private void FillCaches()
        {
            FillRowsCache(ref cacheCustomer, dsCustomer.Tables[0], "Name");
            FillRowsCache(ref cacheStrWorks, dsStrWorks.Tables[0], new string[] { "ParentId", "Name" }, "|", "ID");
            FillRowsCache(ref cacheContract, dsContract.Tables[0], new string[] { "RefCustomer", "Object" }, "|", "ID");
        }

        private int GetClsMaxCode(IClassifier cls)
        {
            string query = string.Format(" select max(CODE) from {0} ", cls.FullDBName);
            object maxCode = this.DB.ExecQuery(query, QueryResultTypes.Scalar, new IDbDataParameter[] { });
            if ((maxCode == null) || (maxCode == DBNull.Value))
                return 0;
            return Convert.ToInt32(maxCode);
        }

        private void SetMaxCodes()
        {
            maxCustomerCode = GetClsMaxCode(clsCustomer);
            maxStrWorksCode = GetClsMaxCode(clsStrWorks);
            maxContractCode = GetClsMaxCode(clsContract);
        }

        protected override void QueryData()
        {
            InitDataSet(ref daCustomer, ref dsCustomer, clsCustomer, string.Empty);
            InitDataSet(ref daStrWorks, ref dsStrWorks, clsStrWorks, string.Empty);
            InitDataSet(ref daContract, ref dsContract, clsContract, string.Empty);
            InitDataSet(ref daTettitory, ref dsTerritory, clsTerritory, string.Empty);

            InitFactDataSet(ref daBuildObj, ref dsBuildObj, fctBuildObj);

            FillCaches();
            SetMaxCodes();
        }

        #region GUIDs

        private const string D_BUILD_CUSTOMER_GUID = "460fd0f8-6459-48b7-88c8-1ad00233ab72";
        private const string D_BUILD_STRWORKS_GUID = "ac7e7ccc-bae4-4159-b17d-d251a2605d4a";
        private const string D_BUILD_CONTRACT_GUID = "d8b533ff-4b1f-4471-abd4-8376739b47b3";
        private const string D_TERRITORY_RF_GUID = "66b9a66d-85ca-41de-910e-f9e6cb483960";
        private const string F_BUILD_BUILDOBJ_GUID = "e3a3dd7f-e4bd-4717-9ef6-f0a866b5d0a3";

        #endregion GUIDs
        protected override void InitDBObjects()
        {
            clsCustomer = this.Scheme.Classifiers[D_BUILD_CUSTOMER_GUID];
            clsStrWorks = this.Scheme.Classifiers[D_BUILD_STRWORKS_GUID];
            clsContract = this.Scheme.Classifiers[D_BUILD_CONTRACT_GUID];
            clsTerritory = this.Scheme.Classifiers[D_TERRITORY_RF_GUID];

            fctBuildObj = this.Scheme.FactTables[F_BUILD_BUILDOBJ_GUID];

            this.UsedClassifiers = new IClassifier[] { };
            this.UsedFacts = new IFactTable[] { fctBuildObj };
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daCustomer, dsCustomer, clsCustomer);
            UpdateDataSet(daStrWorks, dsStrWorks, clsStrWorks);
            UpdateDataSet(daContract, dsContract, clsContract);

            UpdateDataSet(daBuildObj, dsBuildObj, fctBuildObj);
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsBuildObj);
            ClearDataSet(ref dsCustomer);
            ClearDataSet(ref dsStrWorks);
            ClearDataSet(ref dsContract);
            ClearDataSet(ref dsTerritory);
        }

        #endregion Работа с базой и кэшами

        #region Общие методы

        // регулярное выражение для проверки корректности названия папки источника
        private Regex regexValidSourceDirName = new Regex(@"[\d]{1,2}\.[\d]{4}", RegexOptions.Compiled);
        private bool CheckSourceDirName(string dirName)
        {
            return regexValidSourceDirName.IsMatch(dirName.Trim());
        }

        private int GetRefDate(string dirName)
        {
            string[] date = dirName.Split('.');
            return Convert.ToInt32(date[1].Trim()) * 10000 + Convert.ToInt32(date[0].Trim()) * 100;
        }

        private int GetRefTerritory(DataTable dt)
        {
            switch (this.Region)
            {
                case RegionName.YNAO:
                    dt.CaseSensitive = false;
                    DataRow[] rows = dt.Select(" Name like 'ямало%' and RefTerritorialPartType = 3 ");
                    if (rows.Length > 0)
                        return Convert.ToInt32(rows[0]["ID"]);
                    break;
            }
            return -1;
        }

        private Regex regexExtraSpaces = new Regex(@"(\s\s+)", RegexOptions.Compiled);
        private string RemoveExtraSpaces(string value)
        {
            return regexExtraSpaces.Replace(value.Replace("\n", string.Empty).Trim(), " ");
        }

        #endregion Общие методы

        #region Работа с Xls

        #region Формирование классификатора Строительство.Ход работ

        // проверка, является ли столбец пустые (первые несколько строк незаполнены)
        private bool IsEmptyColumn(ExcelHelper excelDoc, int curColumn)
        {
            for (int curRow = 1; curRow <= 15; curRow++)
            {
                if (excelDoc.GetValue(curRow, curColumn).Trim() != string.Empty)
                    return false;
            }
            return true;
        }

        private int PumpStrWorks(string name, int parentId)
        {
            name = RemoveExtraSpaces(name);
            if (cacheStrWorks.ContainsKey(name))
                return cacheStrWorks[name];

            maxStrWorksCode++;

            string key = string.Empty;
            object[] mapping = new object[] { "Code", maxStrWorksCode, "Name", name, "RefUnits", -1 };
            if (parentId == -1)
            {
                key = string.Format("|{0}", name);
            }
            else
            {
                key = string.Format("{0}|{1}", parentId, name);
                mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "ParentId", parentId });
            }
            return PumpCachedRow(cacheStrWorks, dsStrWorks.Tables[0], clsStrWorks, key, mapping);
        }

        private void PumpXlsStrWorks(ExcelHelper excelDoc)
        {
            int[] parentIds = new int[5] { -1, -1, -1, -1, -1 };
            // классификатор 'Строительство.Ход работ' формируется из заголовка таблицы
            // начиная со столбца S до ПРЕДпоследнего столбца
            for (int curColumn = 19; ; curColumn++)
            {
                if (IsEmptyColumn(excelDoc, curColumn + 1))
                    break;

                int curLevel = 0;
                int lastLevel = 0;
                foreach (int curRow in new int[] { 6, 8, 9, 10 })
                {
                    curLevel++;
                    string cellValue = excelDoc.GetValue(curRow, curColumn).Trim();
                    if ((cellValue != string.Empty) && (cellValue.ToUpper() != "ВСЕГО"))
                    {
                        if ((curRow == 9) && (curColumn == 50))
                            cellValue = "Единицы измерения";
                        parentIds[curLevel] = PumpStrWorks(cellValue, parentIds[curLevel - 1]);
                        lastLevel = curLevel;
                    }
                }
                strWorksColumns.Add(curColumn, parentIds[lastLevel]);
            }
        }

        #endregion

        private int PumpXlsCustomer(ExcelHelper excelDoc, int curRow)
        {
            string name = excelDoc.GetValue(curRow, 2).Trim();
            if (cacheCustomer.ContainsKey(name))
                return cacheCustomer[name];

            maxCustomerCode++;
            return PumpCachedRow(cacheCustomer, dsCustomer.Tables[0], clsCustomer, name,
                new object[] { "Name", name, "Code", maxCustomerCode });
        }

        private bool IsCustomerRow(ExcelHelper excelDoc, int curRow)
        {
            string customerNum = excelDoc.GetValue(curRow, 1).Trim();
            string contractNum = excelDoc.GetValue(curRow + 1, 1).Trim();
            if ((customerNum == string.Empty) || (contractNum == string.Empty))
                return false;
            return (Convert.ToInt32(contractNum) == 1);
        }

        private int PumpXlsContract(ExcelHelper excelDoc, int curRow)
        {
            string objectName = excelDoc.GetValue(curRow, 2).Trim();
            string key = string.Format("{0}|{1}", lastRefCustomer, objectName);
            if (cacheContract.ContainsKey(key))
                return cacheContract[key];

            maxContractCode++;
            object[] mapping = new object[] {
                "Code", maxContractCode,
                "Object", objectName,
                "ConPSD", excelDoc.GetValue(curRow, 8).Trim(),
                "ContPSD", excelDoc.GetValue(curRow, 9).Trim(),
                "OrganPIR", excelDoc.GetValue(curRow, 10).Trim(),
                "PSD", excelDoc.GetValue(curRow, 11).Trim(),
                "StExpert", excelDoc.GetValue(curRow, 12).Trim(),
                "LandPlot", excelDoc.GetValue(curRow, 13).Trim(),
                "ResBuild", excelDoc.GetValue(curRow, 14).Trim(),
                "ConSMR", excelDoc.GetValue(curRow, 15).Trim(),
                "ContSMR", excelDoc.GetValue(curRow, 16).Trim(),
                "OrganSMR", excelDoc.GetValue(curRow, 17).Trim(),
                "TermPerf", excelDoc.GetValue(curRow, 18).Trim(),
                "Overview", excelDoc.GetValue(curRow, 54).Trim(),
                "RefCustomer", lastRefCustomer
            };
            return PumpCachedRow(cacheContract, dsContract.Tables[0], clsContract, key, mapping);
        }

        private void PumpFactRow(double factValue, string ei, int refDate, int refStrWorks, int refContract, int refTerritory)
        {
            if (factValue == 0)
                return;

            object[] mapping = new object[] {
                "Report", factValue,
                "EI", ei,
                "RefYearDayUNV", refDate,
                "RefContract", refContract,
                "RefStrWorks", refStrWorks,
                "RefTerritory", refTerritory
            };

            PumpRow(dsBuildObj.Tables[0], mapping);
            if (dsBuildObj.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daBuildObj, ref dsBuildObj);
            }
        }

        private string GetDelimiter(string value)
        {
            if (value.Split('\n').GetLength(0) > 1)
                return "\n";
            if (value.Split(';').GetLength(0) > 1)
                return ";";
            if (value.Split('/').GetLength(0) > 1)
                return "/";
            if (value.Split(' ').GetLength(0) > 1)
                return " ";
            return string.Empty;
        }

        private List<FactValue> GetFactValues(ExcelHelper excelDoc, int curRow, int curColumn)
        {
            // в столбце 50 (AX) единицы измерения - их не качаем
            // эти значения качаются вместе со столбцами 51 (AY) и 52 (AZ)
            if (curColumn == 50)
                return null;

            string value = excelDoc.GetValue(curRow, curColumn).Trim();
            // пустые значения пропускаем
            if (value == string.Empty)
                return null;

            double factValue = 0;
            List<FactValue> factValues = new List<FactValue>();
            // из всех столбцов кроме 51 (AY) и 52 (AZ) просто качаем одну сумму
            if ((curColumn != 51) && (curColumn != 52))
            {
                Double.TryParse(value, out factValue);
                factValues.Add(new FactValue(factValue, string.Empty));
                return factValues;
            }

            string delimeter = GetDelimiter(value);
            // если разделить не удалось определить, то в ячейке одна сумма
            if (delimeter == string.Empty)
            {
                Double.TryParse(value, out factValue);
                factValues.Add(new FactValue(factValue, excelDoc.GetValue(curRow, 50).Trim()));
            }
            else
            {
                // качаем несколько сумм из одной ячейки
                string[] values = value.Split(new string[] { delimeter }, StringSplitOptions.None);
                // из предыдущей ячейки берем единицы измерения, которые разделены таким же разделителем
                string[] eis = excelDoc.GetValue(curRow, 50).Split(new string[] { delimeter }, StringSplitOptions.None);
                for (int i = 0; i < values.GetLength(0); i++)
                {
                    Double.TryParse(values[i].Trim(), out factValue);
                    factValues.Add(new FactValue(factValue, (eis.GetLength(0) > i) ? eis[i].Trim() : eis[0].Trim()));
                }
            }

            return factValues;
        }

        private void PumpXlsRow(ExcelHelper excelDoc, int curRow, int refTerritory)
        {
            // если в диапазоне ячеек H-R ничего нет,
            // то из этой строки закачиваем классификатор Строительство.Заказчики,
            // иначе - Строительство.Характеристика объекта
            if (IsCustomerRow(excelDoc, curRow))
            {
                lastRefCustomer = PumpXlsCustomer(excelDoc, curRow);
                return;
            }

            int refContract = PumpXlsContract(excelDoc, curRow);
            foreach (KeyValuePair<int, int> strWorksColumn in strWorksColumns)
            {
                List<FactValue> factValues = GetFactValues(excelDoc, curRow, strWorksColumn.Key);
                if (factValues != null)
                    foreach (FactValue factValue in factValues)
                    {
                        PumpFactRow(factValue.Value, factValue.EI, refDate, strWorksColumn.Value, refContract, refTerritory);
                    }
            }
        }

        private void PumpXlsSheetData(ExcelHelper excelDoc, FileInfo file, int refTerritory)
        {
            string dataSourcePath = GetShortSourcePathBySourceID(this.SourceID);
            int rowsCount = excelDoc.GetRowsCount();
            for (int curRow = 13; curRow <= rowsCount; curRow++)
                try
                {
                    SetProgress(rowsCount, curRow,
                        string.Format("Обработка файла {0}\\{1}...", dataSourcePath, file.Name),
                        string.Format("Строка {0} из {1}", curRow, rowsCount));

                    string cellValue = excelDoc.GetValue(curRow, 2).Trim().ToUpper();
                    if ((cellValue == string.Empty) || cellValue.StartsWith("ВСЕГО") || cellValue.StartsWith("ИТОГО"))
                        continue;

                    PumpXlsRow(excelDoc, curRow, refTerritory);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format(
                        "При обработке строки {0} листа '{1}' возникла ошибка ({2})",
                        curRow, excelDoc.GetWorksheetName(), ex.Message), ex);
                }
        }

        private void PumpXlsFile(FileInfo file)
        {
            WriteToTrace("Открытие документа: " + file.Name, TraceMessageKind.Information);
            ExcelHelper excelDoc = new ExcelHelper();
            strWorksColumns = new Dictionary<int, int>();
            try
            {
                excelDoc.AskToUpdateLinks = false;
                excelDoc.DisplayAlerts = false;
                excelDoc.EnableEvents = false;
                excelDoc.OpenDocument(file.FullName);

                int wsCount = excelDoc.GetWorksheetsCount();
                for (int index = 1; index <= wsCount; index++)
                {
                    excelDoc.SetWorksheet(index);
                    int refTerritory = GetRefTerritory(dsTerritory.Tables[0]);
                    PumpXlsStrWorks(excelDoc);
                    PumpXlsSheetData(excelDoc, file, refTerritory);
                }
            }
            finally
            {
                strWorksColumns.Clear();
                if (excelDoc != null)
                    excelDoc.CloseDocument();
            }
        }

        #endregion Работа с Xls

        #region Перекрытые методы закачки

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            if (!CheckSourceDirName(dir.Name))
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError, string.Format(
                    "Название папки источника '{0}' не соответствует формат 'ММ.ГГГГ'. " +
                    "Данные из этого источника закачаны не будут.", dir.Name));
                return;
            }
            refDate = GetRefDate(dir.Name);
            ProcessFilesTemplate(dir, "*.xls", new ProcessFileDelegate(PumpXlsFile), false, SearchOption.AllDirectories);
            UpdateData();
        }

        protected override void  DirectPumpData()
        {
            PumpDataVTemplate();
 	    }

        #endregion Перекрытые методы закачки

        #endregion Закачка данных

    }

}
