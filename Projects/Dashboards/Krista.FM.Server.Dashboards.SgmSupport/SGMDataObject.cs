using System;
using System.Data;
using System.Collections.ObjectModel;
using System.Text;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.SgmSupport;

namespace Krista.FM.Server.Dashboards.reports.SGM
{
    public enum PeopleGroupType
    {
        pgtAll = 0,
        pgtChild = 1,
        pgtTeen = 2,
        pgtMature = 3,
        pgtChildAndTeens = 4,
        pgtLess1Year = 5,
        pgtMore1Less2 = 6,
        pgtMore3Less6 = 7,
        pgtVillageChild = 8,
        pgtVillageTeens = 9
    }

    /// <summary>
    /// Псевдонимы колонок из базы
    /// </summary>
    public class SGMFieldNamer
    {
        /// Таблица населения
        public const string fnPopulationAll = "PAll";
        public const string fnPopulationLess14 = "PL14";
        public const string fnPopulationMore14Less17 =  "PM14L17";
        public const string fnPopulationLess17 = "PL17";
        public const string fnPopulationMore17 = "PM17";
        public const string fnYear = "Year";
        public const string fnArea = "Area";

        /// Таблица территорий
        public const string fnAreaName = "name";
        public const string fnAreaCode = "kod";

        /// Таблица забоелваний
        public const string fnDiesName = "DiesName";
        public const string fnDiesCode = "DiesCode";
        public const string fnDiesNameLvl = "DiesNameLvl";

        /// Служебное поле группировки
        public const string fnGroupField = "GroupField";
        public const string fnDataField = "DataField";
    }

    public class SGMDataObject
    {
        private const int maxColumnCount = 1000;
        private const int maxParamCount = 5;
        private const int mainColumnIndex = 0;

        public DataTable dtDeseasesShort;
        public DataTable dtDeseasesFull;
        public DataTable dtAreaFull;
        public DataTable dtAreaShort;
        public DataTable dtPopulation;
        public DataTable dtPopulationData;

        public DataTable dtResult;

        public MainColumnType mainColumn;
        public string mainColumnRange;

        public int columnCount;

        public DependentColumnType[] columnTypes = new DependentColumnType[maxColumnCount];
        public string[,] columnParams = new string[maxColumnCount, maxParamCount];

        public bool useLongNames = true;
        public bool useHierarchicalNames;
        public bool ignoreRegionConversion;

        public SGMDataRotator reportFormRotator = new SGMDataRotator();
        public SGMRegionNamer reportRegionNamer = new SGMRegionNamer();
        private readonly SGMSupport supportClass = new SGMSupport();
        private readonly SGMSQLTexts sqlTextClass = new SGMSQLTexts();

        #region SQLQueries

        /// <summary>
        /// Не добавляет пустые значения параметров в запрос
        /// </summary>
        private string CombineFilter(string filterValue, string paramName, string paramValue)
        {
            string paramString = string.Empty;
            if (paramValue != string.Empty)
            {
                if (filterValue.Length > 0)
                {
                    filterValue = String.Format("{0} and ", filterValue);
                }
                paramString = String.Format("(m.{1} in ({0}))", paramValue, paramName);
            }
            return String.Format("{0}{1}", filterValue, paramString);
        }

        /// <summary>
        /// Классификатор заболеваний
        /// </summary>
        public string GetDeseasesSQLText(bool isFull)
        {
            string dbClassifierPath = SGMDataRotator.ClsDeseases;
            string fieldName = "d.name";
            string hierarchicalName = String.Empty;

            if (isFull)
            {
                dbClassifierPath = SGMDataRotator.ClsDeseasesFull;
                fieldName = "d.name_head";
                hierarchicalName = String.Format(", d.name_mem as {0}", SGMFieldNamer.fnDiesNameLvl);
            }

            return String.Format(
                "select {2} as {3}, d.kod as {4} {5} from {0}\\{1} d",
                reportFormRotator.PathClsData,
                dbClassifierPath,
                fieldName,
                SGMFieldNamer.fnDiesName,
                SGMFieldNamer.fnDiesCode,
                hierarchicalName);
        }

        /// <summary>
        /// Классификатор территорий краткий
        /// </summary>
        public string GetAreaFullSQLText()
        {
            return String.Format(
                "select a.name_mem as {2}, a.kod as {3} from {0}\\{1} a",
                reportFormRotator.PathClsData,
                SGMDataRotator.ClsAreaFull,
                SGMFieldNamer.fnAreaName,
                SGMFieldNamer.fnAreaCode);
        }

        /// <summary>
        /// Классификатор территорий краткий
        /// </summary>
        public string GetAreaShortSQLText()
        {
            return String.Format(
                "select a.name as {2}, a.kod as {3}, a.cod, a.f_a1, a.f_a2, a.l_a1 from {0}\\{1} a where cod <> 0 order by a.num",
                reportFormRotator.PathClsData,
                SGMDataRotator.ClsArea,
                SGMFieldNamer.fnAreaName,
                SGMFieldNamer.fnAreaCode);
        }

        /// <summary>
        /// Данные о населении
        /// </summary>
        public string GetPopulationSQLQueryText()
        {
            if (reportFormRotator.formNumber == 1)
            {
                return String.Format(
                    "select p.s1 as {2}, p.s3 as {3}, p.s21 - p.s3 as {4}, p.s1 - p.s21 as {5}, p.s21 as {6}, p.yr as {7}, p.area as {8}  from {0}\\{1} p",
                    reportFormRotator.PathIllData, reportFormRotator.ClsPopulation,
                    SGMFieldNamer.fnPopulationAll, SGMFieldNamer.fnPopulationLess14,
                    SGMFieldNamer.fnPopulationMore14Less17, SGMFieldNamer.fnPopulationMore17,
                    SGMFieldNamer.fnPopulationLess17, SGMFieldNamer.fnYear,
                    SGMFieldNamer.fnArea);
            }

            return String.Format(
                "select p.s1 as {2}, p.s3 as {3}, p.s21 - p.s3 as {4}, p.s1 - p.s21 as {5}, p.s21 as {6}, p.s4, p.s5, p.s7, p.s11, p.s23, p.yr as {7}, p.area as {8}  from {0}\\{1} p",
                reportFormRotator.PathIllData, reportFormRotator.ClsPopulation,
                SGMFieldNamer.fnPopulationAll, SGMFieldNamer.fnPopulationLess14,
                SGMFieldNamer.fnPopulationMore14Less17, SGMFieldNamer.fnPopulationMore17,
                SGMFieldNamer.fnPopulationLess17, SGMFieldNamer.fnYear,
                SGMFieldNamer.fnArea);
        }

        /// <summary>
        /// Данные о населении(просто поля)
        /// </summary>
        public string GetPopulationSimpleSQLQueryText()
        {
            if (reportFormRotator.formNumber == 1)
            {
                return String.Format(
                    "select p.s1 as {2}, p.s3 as {3}, p.s20 as {4}, p.s1 - p.s3 - p.s20 as {5}, p.s3 + p.s20 as {6}, p.yr as {7}, p.area as {8}  from {0}\\{1} p",
                    reportFormRotator.PathIllData, reportFormRotator.ClsPopulation,
                    SGMFieldNamer.fnPopulationAll, SGMFieldNamer.fnPopulationLess14,
                    SGMFieldNamer.fnPopulationMore14Less17, SGMFieldNamer.fnPopulationMore17,
                    SGMFieldNamer.fnPopulationLess17, SGMFieldNamer.fnYear,
                    SGMFieldNamer.fnArea);
            }

            return String.Format(
                "select p.s1 as {2}, p.s3 as {3}, p.s21 as {4}, p.s1 - p.s21 as {5}, p.s21 as {6}, p.s4, p.s5, p.s7, p.s11, p.s23, p.yr as {7}, p.area as {8}  from {0}\\{1} p",
                reportFormRotator.PathIllData, reportFormRotator.ClsPopulation,
                SGMFieldNamer.fnPopulationAll, SGMFieldNamer.fnPopulationLess14,
                SGMFieldNamer.fnPopulationMore14Less17, SGMFieldNamer.fnPopulationMore17,
                SGMFieldNamer.fnPopulationLess17, SGMFieldNamer.fnYear,
                SGMFieldNamer.fnArea);
        }

        /// <summary>
        /// Основной запрос
        /// </summary>
        public string GetMainSQLText(string headField, string year, string month, string subjectIDs, string dies, string groupType)
        {
            var strBuilder = new StringBuilder();
            int lastYear = Convert.ToInt32(year.Split(',')[year.Split(',').Length - 1]);

            string dataField = "sum(m.s1)";
            if (groupType == "1") dataField = "sum(m.s3)";
            if (lastYear < 2006)
            {
                if (groupType == "2") dataField = "sum(m.s20)";
                if (groupType == "3") dataField = "sum(m.s1) - sum(m.s20) - sum(m.s3)";
                if (groupType == "4") dataField = "sum(m.s20) + sum(m.s3)";
            }
            else
            {
                if (groupType == "2") dataField = "sum(m.s21) - sum(m.s3)";
                if (groupType == "3") dataField = "sum(m.s1) - sum(m.s21)";
                if (groupType == "4") dataField = "sum(m.s21)";
            }
            if (groupType == "5") dataField = "sum(m.s4)";
            if (groupType == "6") dataField = "sum(m.s5)";
            if (groupType == "7") dataField = "sum(m.s7)";
            if (groupType == "8") dataField = "sum(m.s11)";
            if (groupType == "9") dataField = "sum(m.s23)";

            strBuilder.Append("select {3} as {4}, m.{5} as {6} ");
            strBuilder.Append("from ");
            strBuilder.Append(" {0}\\{1} m ");
            strBuilder.Append("where ");
            strBuilder.Append(" {2} ");
            if (headField.Length > 0)
            {
                strBuilder.Append(String.Format("group by m.{0}", headField));
            }

            string conditionValue = String.Empty;
            conditionValue = CombineFilter(conditionValue, "dies", dies);
            conditionValue = CombineFilter(conditionValue, "area", subjectIDs);
            conditionValue = CombineFilter(conditionValue, "yr", year);
            conditionValue = CombineFilter(conditionValue, "mon", month);

            return String.Format(strBuilder.ToString(),
                reportFormRotator.PathIllData, 
                reportFormRotator.ClsDataTable,
                conditionValue,
                dataField, SGMFieldNamer.fnDataField,
                headField, SGMFieldNamer.fnGroupField);
        }

        #endregion

        /// <summary>
        /// Перечисление типов главных колонок
        /// </summary>
        public enum MainColumnType
        {
            mctNone = 0,
            mctYear = 1,
            mctMapName = 2,
            mctDeseaseName = 3
        }

        /// <summary>
        /// Перечисление типов выводимых колонок
        /// </summary>
        public enum DependentColumnType
        {
            dctNone = -1,
            dctAbs = 0,
            dctRelation = 1,
            dctRank = 2, 
            dctFO = 3,
            dctPopulation = 4,  
            dctUpDownPercent = 5,
            dctForecast = 6, 
            dctPercentFromTotal = 7,
            dctCounter = 8, 
            dctSubjectListPercentMore50 = 9,
            dctSubjectListPercentMore95 = 10,
            dctUpDownPercentText = 11,
            dctShortDeseaseName = 12,
            dctCodeDesease = 13,
        }

        // Заполняем классификаторы
        public void InitObject()
        {
            mainColumn = MainColumnType.mctNone;

            dtAreaFull = new DataTable();
            dtAreaShort = new DataTable();
            dtDeseasesFull = new DataTable();
            dtDeseasesShort = new DataTable();
            dtPopulation = new DataTable();
            dtPopulationData = new DataTable();

            reportFormRotator.ExecQuery(dtAreaShort, GetAreaShortSQLText());
            reportFormRotator.ExecQuery(dtAreaFull, GetAreaFullSQLText());
            reportFormRotator.ExecQuery(dtDeseasesFull, GetDeseasesSQLText(true));
            reportFormRotator.ExecQuery(dtDeseasesShort, GetDeseasesSQLText(false));
            reportFormRotator.ExecQuery(dtPopulation, GetPopulationSQLQueryText());
            reportFormRotator.ExecQuery(dtPopulationData, GetPopulationSimpleSQLQueryText());

            dtResult = new DataTable();

            ClearParams();

            useLongNames = false;
            useHierarchicalNames = false;
            mainColumnRange = string.Empty;

            reportRegionNamer.FillFMtoSGMRegions();

            reportFormRotator.FillRegionsDictionary(dtAreaShort);
        }

        // Чистим список колонок запроса
        public void ClearParams()
        {
            columnCount = 0;
            dtResult = new DataTable();
            for (int i = 0; i < maxColumnCount; i++)
            {
                columnTypes[i] = DependentColumnType.dctNone;
                for (int j = 0; j < maxParamCount; j++)
                {
                    columnParams[i, j] = string.Empty;
                }
            }
        }

        // Для получаемых запросом колонок
        public void AddColumn(DependentColumnType colType,
            string param1, string param2, string param3, string param4, string param5)
        {
            columnCount++;
            columnTypes[columnCount] = colType;
            columnParams[columnCount, 0] = param1;
            columnParams[columnCount, 1] = param2;
            columnParams[columnCount, 2] = param3;
            columnParams[columnCount, 3] = param4;
            columnParams[columnCount, 4] = param5;
        }

        // Для вычисляемых колонок        
        public void AddColumn(DependentColumnType colType, string param1)
        {
            AddColumn(colType, param1, string.Empty, string.Empty, string.Empty, string.Empty);
        }

        public void AddColumn(DependentColumnType colType, string param1, string param2)
        {
            AddColumn(colType, param1, param2, string.Empty, string.Empty, string.Empty);
        }

        public void AddColumn(DependentColumnType colType, string param1, string param2, string param3)
        {
            AddColumn(colType, param1, param2, param3, string.Empty, string.Empty);
        }

        public void AddColumn(DependentColumnType colType, string param1, string param2, string param3, string param4)
        {
            AddColumn(colType, param1, param2, param3, param4, string.Empty);
        }

        private string GetMainColumnName()
        {
            string result = string.Empty;
            if (mainColumn == MainColumnType.mctDeseaseName) result = MILL1.Dies;
            if (mainColumn == MainColumnType.mctMapName) result = MILL1.Area;
            if (mainColumn == MainColumnType.mctYear) result = MILL1.Yr;
            return result;
        }

        // Создание структуры для вывода
        private void CreateFields()
        {            
            DataColumn dataColumn = dtResult.Columns.Add();
            dataColumn.DataType = Type.GetType("System.String");

            for (int i = 1; i < columnCount + 1; i++)
            {
                dataColumn = dtResult.Columns.Add();
                string dataType = "Double";

                if (columnTypes[i] == DependentColumnType.dctFO
                    || columnTypes[i] == DependentColumnType.dctSubjectListPercentMore50
                    || columnTypes[i] == DependentColumnType.dctSubjectListPercentMore95
                    || columnTypes[i] == DependentColumnType.dctUpDownPercentText
                    || columnTypes[i] == DependentColumnType.dctShortDeseaseName
                    || columnTypes[i] == DependentColumnType.dctCodeDesease)
                {
                    dataType = "String";
                }

                dataColumn.DataType = Type.GetType(String.Format("System.{0}", dataType));
            }
        }

        // Заполнение полей из БД(абсолютки)
        private void FillDataField(DataTable dt, int colIndex)
        {
            // 1 - year, 2 - month, 3 - mapName, 4 - groupName, 5 - dies
            string param1 = columnParams[colIndex, 0];
            string param2 = columnParams[colIndex, 1];
            string param3 = columnParams[colIndex, 2];
            string param4 = columnParams[colIndex, 3];
            string param5 = columnParams[colIndex, 4];

            if (param3 != string.Empty)
            {
                param3 = reportFormRotator.regionSubstrSubjectIDs[param3];
            }

            var dtAbsData = new DataTable();
            reportFormRotator.ExecQuery(dtAbsData, 
                GetMainSQLText(
                    GetMainColumnName(), 
                    param1,
                    param2,
                    param3,
                    param5, 
                    param4
                ));

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string mainColumnValue = dt.Rows[i][mainColumnIndex].ToString();

                if (mainColumn == MainColumnType.mctMapName)
                {
                    mainColumnValue = reportFormRotator.regionSubstrSubjectIDs[GetMapName(mainColumnValue)];
                }
                
                DataRow[] drsSelect = dtAbsData.Select(String.Format("{0} in ({1})", 
                    SGMFieldNamer.fnGroupField, 
                    mainColumnValue));

                double relColumnValue = 0;
                for (int j = 0; j < drsSelect.Length; j++)
                {
                    relColumnValue += Convert.ToDouble(drsSelect[j][SGMFieldNamer.fnDataField]);
                }
                dt.Rows[i][colIndex] = relColumnValue;
            }
        }

        // Заполнение вычисляемых полей(отн. показтель, ранк и прочее)
        private void FillCalcFieldBO(DataTable dt, int colIndex)
        {
            string param1;
            string param4;

            // Федеральный округ
            if (columnTypes[colIndex] == DependentColumnType.dctFO)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dt.Rows[i][colIndex] = GetFOName(dt.Rows[i][mainColumnIndex].ToString());
                }
            }

            // Считаем ранги
            if (columnTypes[colIndex] == DependentColumnType.dctRank)
            {
                int parentColumnIndex = Convert.ToInt32(columnParams[colIndex, 0]);
                //колонка, по которой считаем ранг
                string columnName = dt.Columns[parentColumnIndex].ColumnName;
                int rank = 1;
                DataRow[] rows = dt.Select(String.Empty, String.Format("{0} desc", columnName));
                for (int i = 0; i < rows.Length; i++)
                {
                    string mainColumnValue = rows[i][mainColumnIndex].ToString();
                    bool needRank = !(mainColumn == MainColumnType.mctMapName && Convert.ToInt32(mainColumnValue) > 300);
                    if (needRank)
                    {
                        DataRow findRow = supportClass.GetTableRowValue(dt, mainColumnValue, mainColumnIndex);
                        findRow[colIndex] = rank;
                        rank++;
                    }
                }
            }
            // Считаем относительную заболеваемость
            if (columnTypes[colIndex] == DependentColumnType.dctRelation)
            {
                int parentColumnIndex = Convert.ToInt32(columnParams[colIndex, 0]);
                param1 = columnParams[parentColumnIndex, 0];
                string param3 = columnParams[parentColumnIndex, 2];
                param4 = columnParams[parentColumnIndex, 3];

                // 1 - year 3 - map 4 - groupIndex
                string year = param1;
                string mapCode = GetMapCode(param3);
                var groupType = (PeopleGroupType)(Convert.ToInt32(param4));
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (mainColumn == MainColumnType.mctMapName)
                    {
                        mapCode = Convert.ToString(dt.Rows[i][mainColumnIndex]);
                    }
                    double absValue = Convert.ToDouble(dt.Rows[i][parentColumnIndex]);
                    double population = GetPopulationValue(mapCode, year, groupType);
                    if (population > 0)
                    {
                        dt.Rows[i][colIndex] = 100000 * absValue / population;
                    }
                }
            }

            // Считаем население
            if (columnTypes[colIndex] == DependentColumnType.dctPopulation)
            {
                int parentColumnIndex = Convert.ToInt32(columnParams[colIndex, 0]);
                param1 = columnParams[parentColumnIndex, 0];
                param4 = columnParams[parentColumnIndex, 3];
                // 1 - year 3 - map 4 - groupIndex
                string year = param1;
                var groupType = (PeopleGroupType)(Convert.ToInt32(param4));
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string mapCode = dt.Rows[i][0].ToString();
                    dt.Rows[i][colIndex] = GetPopulationValue(mapCode, year, groupType);
                }
            }

            // Считаем рост
            if (columnTypes[colIndex] == DependentColumnType.dctUpDownPercent)
            {
                int index1 = Convert.ToInt32(columnParams[colIndex, 0]);
                int index2 = Convert.ToInt32(columnParams[colIndex, 1]);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    double value1 = Convert.ToDouble(dt.Rows[i][index1]);
                    double value2 = Convert.ToDouble(dt.Rows[i][index2]);
                    if (value2 != 0)
                    {
                        dt.Rows[i][colIndex] = value1 / value2;
                    }
                    else
                    {
                        dt.Rows[i][colIndex] = 0;
                    }
                }
            }

            // Текст роста
            if (columnTypes[colIndex] == DependentColumnType.dctUpDownPercentText)
            {
                int index1 = Convert.ToInt32(columnParams[colIndex, 0]);
                int index2 = Convert.ToInt32(columnParams[colIndex, 1]);
                int index3 = Convert.ToInt32(columnParams[colIndex, 2]);
                int index4 = Convert.ToInt32(columnParams[colIndex, 3]);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    double value1 = 0;
                    if (dt.Rows[i][index1] != DBNull.Value) value1 = Convert.ToDouble(dt.Rows[i][index1]);
                    double value2 = 0;
                    if (dt.Rows[i][index2] != DBNull.Value) value2 = Convert.ToDouble(dt.Rows[i][index2]);
                    double value3 = 0;
                    if (dt.Rows[i][index3] != DBNull.Value) value3 = Convert.ToDouble(dt.Rows[i][index3]);
                    double value4 = 0;
                    if (dt.Rows[i][index4] != DBNull.Value) value4 = Convert.ToDouble(dt.Rows[i][index4]);
                    if (dt.Rows[i][index1] == DBNull.Value || dt.Rows[i][index2] == DBNull.Value
                        || dt.Rows[i][index3] == DBNull.Value || dt.Rows[i][index4] == DBNull.Value)
                    {
                        value1 = 0;
                        value2 = 0;
                        value3 = 0;
                        value4 = 0;
                    }

                    if (mainColumn == MainColumnType.mctDeseaseName)
                    {
                        string diesCode = Convert.ToString(dt.Rows[i][mainColumnIndex]);
                        int year1 = Convert.ToInt32(columnParams[index1, 0]);
                        int year2 = Convert.ToInt32(columnParams[index2, 0]);
                        dt.Rows[i][colIndex] = supportClass.GetDifferenceTextEx(
                            diesCode, year1, year2, value1, value2, value3, value4, false, false);                        
                    }
                    else
                    {
                        dt.Rows[i][colIndex] = supportClass.GetDifferenceTextEx(
                            value1, value2, value3, value4, false, false);                        
                    }
                }
            }

            if (columnTypes[colIndex] == DependentColumnType.dctShortDeseaseName)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dt.Rows[i][colIndex] = GetDeseaseName(Convert.ToString(dt.Rows[i][mainColumnIndex]), true);
                }
            }

            if (columnTypes[colIndex] == DependentColumnType.dctCodeDesease)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dt.Rows[i][colIndex] = dt.Rows[i][mainColumnIndex];
                }
            }
        }

        private double GetNumber(object obj)
        {
            double result;
            if (!double.TryParse(obj.ToString(), out result))
            {
                result = 0;
            }
            
            return result;
        }

        // Заполнение вычисляемых полей(отн. показтель, ранк и прочее)
        private void FillCalcFieldAO(DataTable dt, int colIndex)
        {
            string param1 = columnParams[colIndex, 0];
            string param2 = columnParams[colIndex, 1];
            // Процент от общего
            if (columnTypes[colIndex] == DependentColumnType.dctPercentFromTotal)
            {
                string[] indexes = param1.Split(',');
                int partIndex = Convert.ToInt32(param2);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    double sum = 0;
                    for (int j = 0; j < indexes.Length; j++)
                    {
                        sum += GetNumber(dt.Rows[i][Convert.ToInt32(indexes[j])]);
                    }
                    dt.Rows[i][colIndex] = 100 * GetNumber(dt.Rows[i][partIndex]) / sum;
                }
            }
        }

        // Данные по населению территории
        private double GetPopulationValue(string mapCode, string year, PeopleGroupType groupType)
        {
            double result = 0;
            const string selectStr = "({0} in ({1})) and ({2} in ({3}))";
            DataRow[] drsPopulation = dtPopulation.Select(String.Format(selectStr,
                SGMFieldNamer.fnYear, year, SGMFieldNamer.fnArea, mapCode));

            var colIndex = (int)groupType;
            if (drsPopulation.Length > 0)
            {
                if (groupType == PeopleGroupType.pgtChildAndTeens && Convert.ToInt32(year) < 2006)
                {
                    drsPopulation = dtPopulationData.Select(String.Format(selectStr,
                        SGMFieldNamer.fnYear, year, SGMFieldNamer.fnArea, mapCode));
                }
                for (int i = 0; i < drsPopulation.Length; i++)
                {
                    result += Convert.ToDouble(drsPopulation[i][colIndex]);
                }
            }
            else
            {
                // А если не нашли то возможно ФО
                DataRow drArea = supportClass.FindDataRow(dtAreaShort, mapCode, AREA.Kod);
                if (drArea != null && drArea[AREA.F_A1].ToString() != string.Empty)
                {
                    drsPopulation = dtPopulation.Select(String.Format(selectStr,
                                    SGMFieldNamer.fnYear, year, SGMFieldNamer.fnArea, drArea[AREA.F_A1]));

                    for (int i = 0; i < drsPopulation.Length; i++)
                    {
                        result += Convert.ToDouble(drsPopulation[i][colIndex]);
                    }
                }
            }
            return result;
        }

        public string GetFOName(string subjectCode, bool needShort = true)
        {
            DataRow[] regionsSet = dtAreaShort.Select(sqlTextClass.GetFOFilter());
            for (int i = 0; i < regionsSet.Length; i++)
            {
                if (regionsSet[i][AREA.Kod].ToString() != "303")
                {
                    string[] subjectKeys = Convert.ToString(regionsSet[i][AREA.F_A2]).Split(',');
                    
                    foreach (string subjectKey in subjectKeys)
                    {
                        var drArea = supportClass.FindDataRow(dtAreaShort, subjectKey.Trim(), AREA.Kod);
                        if (drArea != null)
                        {
                            if (drArea[AREA.Kod].ToString() == subjectCode)
                            {
                                string foName = Convert.ToString(regionsSet[i][AREA.Name]);
                                if (needShort)
                                {
                                    foName = supportClass.GetFOShortName(foName);
                                }
                                return foName;
                            }
                        }
                    }
                }
            }
            return String.Empty;
        }

        public string GetDeseaseName(string code, bool needShort)
        {
            string result = String.Empty;
            DataRow drFind = supportClass.FindDataRow(needShort ? dtDeseasesShort : dtDeseasesFull, code, SGMFieldNamer.fnDiesCode);

            if (supportClass.CheckValue(drFind))
            {
                result = supportClass.ConvertEncode2(drFind[SGMFieldNamer.fnDiesName].ToString().Trim());
                if (useHierarchicalNames)
                {
                    result = supportClass.ConvertEncode2(drFind[SGMFieldNamer.fnDiesNameLvl].ToString().Trim());
                }
            }
            
            return result;
        }

        public string GetDeseaseFullName(string shortName)
        {
            string result = String.Empty;
            DataRow drFind = supportClass.FindDataRowEx(dtDeseasesShort, shortName, SGMFieldNamer.fnDiesName);
            
            if (supportClass.CheckValue(drFind))
            {
                string code = drFind[SGMFieldNamer.fnDiesCode].ToString();
                drFind = supportClass.FindDataRowEx(dtDeseasesFull, code, SGMFieldNamer.fnDiesCode);
                if (supportClass.CheckValue(drFind))
                {
                    result = supportClass.ConvertEncode1(drFind[SGMFieldNamer.fnDiesName].ToString());
                }
            }

            return result;
        }
        
        // Имя территории
        public string GetMapName(string code)
        {
            return GetMapName(code, true);
        }

        public string GetMapName(string code, bool needShort)
        {
            string result = string.Empty;
            
            DataTable dtArea = dtAreaFull;
            if (needShort) dtArea = dtAreaShort;

            DataRow drFind = supportClass.FindDataRow(dtArea, code, SGMFieldNamer.fnAreaCode);
            if (supportClass.CheckValue(drFind))
            {
                result = drFind[SGMFieldNamer.fnAreaName].ToString();
            }

            return result;
        }

        public string GetMapCode(string mapName)
        {
            string result = string.Empty;
            DataRow drFind = supportClass.FindDataRow(dtAreaShort, mapName, SGMFieldNamer.fnAreaName);
            if (supportClass.CheckValue(drFind))
            {
                result = drFind[SGMFieldNamer.fnAreaCode].ToString();
            }
            return result;
        }

        private int GetMaxYear()
        {
            int result = 0;
            for (int i = 1; i < columnCount + 1; i++)
            {
                if (columnTypes[i] == DependentColumnType.dctAbs)
                {
                    string yearList = columnParams[i, 0];
                    result = Math.Max(result, Convert.ToInt32(yearList.Split(',')[yearList.Split(',').Length - 1]));
                }
            }
            return result;
        }

        // Заполняем данные в ведущей колонке(территория, заболевание,год)
        private void FillMainColumn()
        {
            if (mainColumn == MainColumnType.mctYear)
            {
                int startYear = reportFormRotator.GetFirstYear();
                int endYear = reportFormRotator.GetLastYear();
                for (int i = startYear; i < endYear + 1; i++)
                {
                    DataRow drAdd = dtResult.Rows.Add();
                    drAdd[mainColumnIndex] = i;
                }
            }
            if (mainColumn == MainColumnType.mctDeseaseName)
            {
                string[] values = mainColumnRange.Split(',');
                for (int i = 0; i < values.Length; i++)
                {
                    DataRow drAdd = dtResult.Rows.Add();
                    drAdd[mainColumnIndex] = values[i];
                }
            }
            if (mainColumn == MainColumnType.mctMapName)
            {
                int maxYear = GetMaxYear();
                var values = new Collection<string>(mainColumnRange.Split(','));
                for (int i = 0; i < dtAreaShort.Rows.Count; i++)
                {
                    int subjectCode = Convert.ToInt32(dtAreaShort.Rows[i][SGMFieldNamer.fnAreaCode]);
                    if (CheckSubjectExist(subjectCode, maxYear))
                    {
                        if (mainColumnRange == string.Empty || values.Contains(subjectCode.ToString()))
                        {
                            DataRow drAdd = dtResult.Rows.Add();
                            drAdd[mainColumnIndex] = subjectCode;
                        }
                    }
                }
            }
        }

        public DataTable SortDataSet(DataTable dt, int sortIndex, bool sortDesc)
        {
            string sortDirection = "asc";
            if (sortDesc) sortDirection = "desc";

            DataTable dtTemp = dt.Clone();
            var rows = dt.Select(String.Empty, String.Format("{1} {0}", sortDirection, dt.Columns[sortIndex].ColumnName));
            for (int i = 0; i < rows.Length; i++)
            {
                dtTemp.ImportRow(rows[i]);
            }
            dtTemp.AcceptChanges();            
            return dtTemp;
        }

        public DataTable FilterDataSet(DataTable dt, string filter)
        {
            DataTable dtTemp = dt.Clone();
            var rows = dt.Select(filter);
            for (int i = 0; i < rows.Length; i++)
            {
                dtTemp.ImportRow(rows[i]);
            }
            dtTemp.AcceptChanges();
            return dtTemp;
        }

        public DataTable CloneDataTable(DataTable dt)
        {
            DataTable dtTemp = dt.Clone();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dtTemp.ImportRow(dt.Rows[i]);
            }
            dtTemp.AcceptChanges();

            return dtTemp;
        }

        public DataTable CloneDataTable(DataTable dt, DataRow[] dr)
        {
            DataTable dtTemp = dt.Clone();
            for (int i = 0; i < dr.Length; i++)
            {
                dtTemp.ImportRow(dr[i]);
            }
            dtTemp.AcceptChanges();

            return dtTemp;
        }

        private void ReplaceCodeValues(DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                if (mainColumn == MainColumnType.mctMapName)
                {
                    string areaName = GetMapName(Convert.ToString(row[mainColumnIndex]));

                    if (!ignoreRegionConversion)
                    {
                        areaName = supportClass.GetFOShortName(areaName);                        
                    }

                    if (useLongNames)
                    {
                        areaName = reportRegionNamer.GetFMName(areaName);
                    }

                    row[mainColumnIndex] = areaName;
                }
                if (mainColumn == MainColumnType.mctDeseaseName)
                {
                    row[mainColumnIndex] = GetDeseaseName(Convert.ToString(row[mainColumnIndex]), !useLongNames);
                }
            }
        }

        public DataTable FillData()
        {
            return FillData(-1, true);
        }

        public DataTable FillData(int sortIndex)
        {
            return FillData(sortIndex, true);
        }

        // Заполняем данные
        public DataTable FillData(int sortIndex, bool sortDesc)
        {
            CreateFields();
            FillMainColumn();
            for (int i = 1; i < columnCount + 1; i++)
            {
                if (columnTypes[i] != DependentColumnType.dctAbs)
                {
                    FillCalcFieldBO(dtResult, i);
                }
                else
                {
                    FillDataField(dtResult, i);
                }
            }
            
            for (int i = 1; i < columnCount + 1; i++)
            {
                if (columnTypes[i] != DependentColumnType.dctAbs)
                {
                    FillCalcFieldAO(dtResult, i);
                }
            }  

            DataTable dtOutput = dtResult;
            ReplaceCodeValues(dtOutput);
            if (sortIndex != -1)
            {
                dtOutput = SortDataSet(dtResult, sortIndex, sortDesc);
            }

            return dtOutput;
        }

        public string GetRootMapName()
        {
            var drsSelect = dtAreaShort.Select(String.Format("{0} = 999", AREA.Cod));
            
            if (drsSelect.Length > 0)
            {
                return supportClass.GetFOShortName(drsSelect[0][AREA.Name].ToString());
            }

            return String.Empty;
        }

        private bool CheckSubjectExist(int subjectCode, int maxYear)
        {

            if (maxYear > 2007)
            {
                if (subjectCode == 57
                    || subjectCode == 102
                    || subjectCode == 105
                    || subjectCode == 106
                    || subjectCode == 30
                    || subjectCode == 110)
                {
                    return false;
                }
                if (maxYear > 2008)
                {
                    if (subjectCode == 76
                        || subjectCode == 108
                        || subjectCode == 107)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public Collection<string> GetFOSubjectList(string foName)
        {
            const string strFilter = "{0} = '{1}'";
            var subjectList = new Collection<string>();

            DataRow[] rowRegion = dtAreaShort.Select(String.Format(strFilter, AREA.Name, foName));

            if (rowRegion.Length > 0)
            {
                string[] subjectArray = rowRegion[0].ToString().Trim().Split(',');
                foreach (string regionKey in subjectArray)
                {
                    DataRow[] rowSubject = dtAreaShort.Select(String.Format(strFilter, AREA.Kod, regionKey));
                    subjectList.Add(Convert.ToString(rowSubject[0][AREA.Name]));
                }
            }

            return subjectList;
        }

    }
}
