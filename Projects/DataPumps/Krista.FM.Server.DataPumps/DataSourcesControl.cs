using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Lifetime;
using System.Threading;

using Krista.FM.Common;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.Common;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataPumps
{
    // Модуль с функциями управления источниками данных

    /// <summary>
    /// Базовый класс для всех закачек
    /// </summary>
    public abstract partial class DataPumpModuleBase : DisposableObject
    {
        #region Поиск и добавление источников

        /// <summary>
        /// Добавляет источник данных
        /// </summary>
        /// <param name="supplierCode">Код поставщика</param>
        /// <param name="dataCode">Порядковый номер информации</param>
        /// <param name="dsType">Тип источника</param>
        /// <param name="budgetName">Наименование бюджета</param>
        /// <param name="year">Год</param>
        /// <param name="month">Месяц</param>
        /// <param name="variant">Вариант</param>
        /// <param name="quarter">Квартал</param>
        /// <param name="isNewDataSource">Это новый источник, мать вашу!</param>
        /// <returns>ИД источника</returns>
        public IDataSource AddDataSource(IPumpHistoryElement phe, string supplierCode, string dataCode,
            ParamKindTypes parametersType, string budgetName, int year, int month, string variant,
            int quarter, string territory, out bool isNewDataSource)
        {
            string str;
            isNewDataSource = true;

            IDataSource dataSource = null;

            try
            {
                // Ищем такой источник
                dataSource = FindDataSource(parametersType, supplierCode, dataCode, budgetName, year, month, variant,
                    quarter, territory);

                // Если такой источник уже есть, то его и возвращаем. Если нет - создаем новый
                if (dataSource != null)
                {
                    isNewDataSource = false;
                    this.Scheme.DataSourceManager.DataSources.Add(dataSource, phe);
                }
                else
                {
                    dataSource = this.Scheme.DataSourceManager.DataSources.CreateElement();
                    dataSource.SupplierCode = supplierCode;
                    dataSource.DataCode = dataCode;
                    dataSource.DataName = constDefaultClsName;
                    str = dataCode.PadLeft(4, '0');

                    if (Scheme.DataSourceManager.DataSuppliers.ContainsKey(supplierCode))
                    {
                        if (Scheme.DataSourceManager.DataSuppliers[supplierCode].DataKinds.ContainsKey(str))
                        {
                            dataSource.DataName = Scheme.DataSourceManager.DataSuppliers[supplierCode].DataKinds[str].Name;
                        }
                    }

                    dataSource.ParametersType = parametersType;
                    dataSource.BudgetName = budgetName;
                    dataSource.Year = year;
                    dataSource.Month = month;
                    dataSource.Variant = variant;
                    dataSource.Quarter = quarter;
                    dataSource.Territory = territory;

                    this.Scheme.DataSourceManager.DataSources.Add(dataSource);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при добавлении источника данных", ex);
            }

            return dataSource;
        }

        /// <summary>
        /// Добавляет источник данных
        /// </summary>
        /// <param name="supplierCode">Код поставщика</param>
        /// <param name="dataCode">Порядковый номер информации</param>
        /// <param name="dsType">Тип источника</param>
        /// <param name="budgetName">Наименование бюджета</param>
        /// <param name="year">Год</param>
        /// <param name="month">Месяц</param>
        /// <param name="variant">Вариант</param>
        /// <param name="quarter">Квартал</param>
        /// <returns>ИД источника</returns>
        public IDataSource AddDataSource(string supplierCode, string dataCode, ParamKindTypes parametersType,
            string budgetName, int year, int month, string variant, int quarter, string territory)
        {
            bool isNewDataSource = false;

            return AddDataSource(this.PumpRegistryElement.PumpHistoryCollection[this.PumpID], supplierCode,
                dataCode, parametersType, budgetName, year, month, variant, quarter, territory, out isNewDataSource);
        }

        /// <summary>
        /// Добавляет источник данных и устанавливает свойство DataSource
        /// </summary>
        /// <param name="dsType">Тип источника</param>
        /// <param name="budgetName">Наименование бюджета</param>
        /// <param name="year">Год</param>
        /// <param name="month">Месяц</param>
        /// <param name="variant">Вариант</param>
        /// <param name="quarter">Квартал</param>
        protected void SetDataSource(ParamKindTypes parametersType, string budgetName, int year, int month,
            string variant, int quarter, string territory)
        {
            bool isNewDataSource;

            this.DataSource = AddDataSource(this.PumpRegistryElement.PumpHistoryCollection[this.PumpID],
                this.PumpRegistryElement.SupplierCode, this.PumpRegistryElement.DataCode, parametersType,
                budgetName, year, month, variant, quarter, territory, out isNewDataSource);

            WriteToTrace(string.Format(
                "Текущий SOURCEID: {0} ({1}), PUMPID: {2}.",
                this.SourceID, GetDataSourceDescription(this.SourceID), this.PumpID), TraceMessageKind.Warning);
        }

        /// <summary>
        /// Добавляет источник данных и устанавливает свойство DataSource
        /// </summary>
        /// <param name="sourceID">ИД источника</param>
        protected void SetDataSource(int sourceID)
        {
            IDataSource ds = GetDataSourceBySourceID(sourceID);

            SetDataSource(ds.ParametersType, ds.BudgetName, ds.Year, ds.Month,
                ds.Variant, ds.Quarter, ds.Territory);
        }

        /// <summary>
        /// Ищет источник данных по его параметрам
        /// </summary>
        /// <param name="parametersType">Тип источника</param>
        /// <param name="supplierCode">Код поставщика</param>
        /// <param name="dataCode">Код данных</param>
        /// <param name="budgetName">Наименование бюджета</param>
        /// <param name="year">Год</param>
        /// <param name="month">Месяц</param>
        /// <param name="variant">Вариант</param>
        /// <param name="quarter">Квартал</param>
        /// <returns>Источник</returns>
        protected IDataSource FindDataSource(ParamKindTypes parametersType, string supplierCode, string dataCode,
            string budgetName, int year, int month, string variant, int quarter, string territory)
        {
            IDataSource ds = this.Scheme.DataSourceManager.DataSources.CreateElement();
            ds.BudgetName = budgetName;
            ds.DataCode = dataCode;
            ds.Month = month;
            ds.ParametersType = parametersType;
            ds.Quarter = quarter;
            ds.SupplierCode = supplierCode;
            ds.Territory = territory;
            ds.Variant = variant;
            ds.Year = year;

            int? result = this.Scheme.DataSourceManager.DataSources.FindDataSource(ds);

            if (result == null) return null;

            return this.Scheme.DataSourceManager.DataSources[(int)result];
        }

        /// <summary>
        /// Действие в случае, если источник не найден
        /// </summary>
        protected enum DataSourceNotFoundAction
        {
            /// <summary>
            /// Создать источник
            /// </summary>
            CreateDataSource,

            /// <summary>
            /// Вернуть null или -1 в зависимости от типа
            /// </summary>
            ReturnNull,

            /// <summary>
            /// Генерировать исключение
            /// </summary>
            ThrowException
        }

        /// <summary>
        /// Ищет источник данных по его параметрам
        /// </summary>
        /// <param name="parametersType">Тип источника</param>
        /// <param name="supplierCode">Код поставщика</param>
        /// <param name="dataCode">Код данных</param>
        /// <param name="budgetName">Наименование бюджета</param>
        /// <param name="year">Год</param>
        /// <param name="month">Месяц</param>
        /// <param name="variant">Вариант</param>
        /// <param name="quarter">Квартал</param>
        /// <returns>ИД источника</returns>
        protected int FindDataSourceID(ParamKindTypes parametersType, string supplierCode, string dataCode,
            string budgetName, int year, int month, string variant, int quarter, string territory,
            DataSourceNotFoundAction dataSourceNotFoundAction)
        {
            IDataSource ds = FindDataSource(parametersType, supplierCode, dataCode, budgetName, year, month, variant,
                quarter, territory);

            if (ds == null)
            {
                switch (dataSourceNotFoundAction)
                {
                    case DataSourceNotFoundAction.ReturnNull: return -1;

                    case DataSourceNotFoundAction.CreateDataSource:
                    case DataSourceNotFoundAction.ThrowException:
                        throw new Exception("Не найден источник " +
                            GetDataSourceDescription(parametersType, supplierCode, dataCode, budgetName, year, month, variant,
                                quarter, territory));
                }
            }

            return ds.ID;
        }

        /// <summary>
        /// Формирует ключ для коллекций по параметрам источника
        /// </summary>
        /// <param name="sourceID">ИД источника</param>
        /// <returns>Ключ</returns>
        private string GetCacheKeyByDataSource(int sourceID)
        {
            IDataSource ds = this.scheme.DataSourceManager.DataSources[sourceID];
            if (ds == null)
                return string.Empty;

            int totalWidthForKey = 100;
            string result = ds.Year.ToString().PadLeft(totalWidthForKey, '0');

            switch (ds.ParametersType)
            {
                case ParamKindTypes.Budget:
                    result += ds.BudgetName.PadLeft(totalWidthForKey, ' ');
                    break;

                case ParamKindTypes.YearMonth:
                    result += ds.Month.ToString().PadLeft(totalWidthForKey, '0');
                    break;

                case ParamKindTypes.YearMonthVariant:
                    result += ds.Month.ToString().PadLeft(totalWidthForKey, '0') +
                        ds.Variant.PadLeft(totalWidthForKey, ' ');
                    break;

                case ParamKindTypes.YearQuarter:
                    result += ds.Quarter.ToString().PadLeft(totalWidthForKey, '0');
                    break;

                case ParamKindTypes.YearQuarterMonth:
                    result += ds.Quarter.ToString().PadLeft(totalWidthForKey, '0') +
                        ds.Month.ToString().PadLeft(totalWidthForKey, '0');
                    break;

                case ParamKindTypes.YearTerritory:
                    result += ds.Territory.PadLeft(totalWidthForKey, ' ');
                    break;

                case ParamKindTypes.YearVariant:
                    result += ds.Variant.PadLeft(totalWidthForKey, ' ');
                    break;

                case ParamKindTypes.Variant:
                    result = ds.Variant.PadLeft(totalWidthForKey, ' ');
                    break;

                case ParamKindTypes.YearVariantMonthTerritory:
                    result += ds.Variant.PadLeft(totalWidthForKey, ' ') +
                        ds.Month.ToString().PadLeft(totalWidthForKey, '0') +
                        ds.Territory.PadLeft(totalWidthForKey, ' ');
                    break;
            }

            return result;
        }

        /// <summary>
        /// Сортирует список закачанных источников по году
        /// </summary>
        protected void SortDataSources(ref Dictionary<int, string> dataSources)
        {
            if (dataSources.Count == 0)
                return;

            SortedList<string, int> st = new SortedList<string, int>(dataSources.Count);

            foreach (KeyValuePair<int, string> kvp in dataSources)
            {
                string key = GetCacheKeyByDataSource(kvp.Key);
                if (!st.ContainsKey(key))
                {
                    st.Add(key, kvp.Key);
                }
                else
                {
              //      throw new Exception(string.Format(
               //         "Источник с параметрами, аналогичными источнику {0}, уже присутствует в коллекции",
                //        kvp.Key));
                }
            }

            Dictionary<int, string> tmp = new Dictionary<int, string>(dataSources.Count);
            foreach (KeyValuePair<string, int> kvp in st)
            {
                int value = kvp.Value;
                if (!tmp.ContainsKey(value))
                {
                    tmp.Add(value, dataSources[kvp.Value]);
                }
                else
                {
                 //   throw new Exception(string.Format("Источник с ID {0} уже присутствует в коллекции", value));
                }
            }

            dataSources.Clear();
            dataSources = tmp;
        }

        #endregion Поиск и добавление источников


        #region Получение информации об источниках

        /// <summary>
        /// Массив для преобразования номера типа источника данных в название
        /// </summary>
        protected string[] KindsOfParamsByNumber = new string[] { 
            "Финансовый орган, год", "Год", "Год, месяц", "Год, месяц, вариант", "Год, вариант", 
            "Год, квартал", "Год, территория", "Год квартал месяц", "Без параметров", "Вариант",
            "Год, месяц, территория", "Год, квартал, территория", "Год, вариант, месяц, территория" };

        /// <summary>
        /// Возвращает описание источника по его параметрам
        /// </summary>
        /// <param name="parametersType">Тип источника</param>
        /// <param name="supplierCode">Код поставщика</param>
        /// <param name="dataCode">Код данных</param>
        /// <param name="budgetName">Наименование бюджета</param>
        /// <param name="year">Год</param>
        /// <param name="month">Месяц</param>
        /// <param name="variant">Вариант</param>
        /// <param name="quarter">Квартал</param>
        /// <returns>Описание</returns>
        protected string GetDataSourceDescription(ParamKindTypes parametersType, string supplierCode, string dataCode,
            string budgetName, int year, int month, string variant, int quarter, string territory)
        {
            string result = string.Format(
                "Код поставщика: {0}; Код данных: {1}; Вид источника: {2}",
                supplierCode, dataCode, KindsOfParamsByNumber[(int)parametersType]);

            switch (parametersType)
            {
                case ParamKindTypes.Budget:
                    result += string.Format("; Финансовый орган: {0}; Год: {1}", budgetName, year);
                    break;

                case ParamKindTypes.Year:
                    result += string.Format("; Год: {0}", year);
                    break;

                case ParamKindTypes.YearMonth:
                    result += string.Format("; Год: {0}; Месяц: {1}", year, month);
                    break;

                case ParamKindTypes.YearMonthVariant:
                    result += string.Format("; Год: {0}; Месяц: {1}; Вариант: {2}", year, month, variant);
                    break;

                case ParamKindTypes.YearQuarter:
                    result += string.Format("; Год: {0}; Квартал: {1}", year, quarter);
                    break;

                case ParamKindTypes.YearQuarterMonth:
                    result += string.Format("; Год: {0}; Квартал: {1}; Месяц: {2}", year, quarter, month);
                    break;

                case ParamKindTypes.YearTerritory:
                    result += string.Format("; Год: {0}; Территория: {1}", year, territory);
                    break;

                case ParamKindTypes.YearVariant:
                    result += string.Format("; Год: {0}; Вариант: {1}", year, variant);
                    break;

                case ParamKindTypes.Variant:
                    result += string.Format("; Вариант: {0}", variant);
                    break;

                case ParamKindTypes.YearMonthTerritory:
                    result += string.Format("; Год: {0};  Месяц: {1}; Территория: {2}", year, month, territory);
                    break;

                case ParamKindTypes.YearQuarterTerritory:
                    result += string.Format("; Год: {0};  Квартал: {1}; Территория: {2}", year, quarter, territory);
                    break;

                case ParamKindTypes.YearVariantMonthTerritory:
                    result += string.Format("; Год: {0}; Вариант: {1}; Месяц: {2}; Территория: {3}", year, variant, month, territory);
                    break;
            }

            return result;
        }

        /// <summary>
        /// Возвращает описание источника по его параметрам
        /// </summary>
        /// <param name="sourceID">ID источника</param>
        /// <returns>Описание</returns>
        protected string GetDataSourceDescription(int sourceID)
        {
            IDataSource ds = GetDataSourceBySourceID(sourceID);

            return GetDataSourceDescription(ds.ParametersType, ds.SupplierCode, ds.DataCode,
                ds.BudgetName, ds.Year, ds.Month, ds.Variant, ds.Quarter, ds.Territory);
        }

        /// <summary>
        /// Формирует путь к источнику по данным из коллекции источников данных
        /// </summary>
        /// <param name="sourceID">ИД источника</param>
        /// <returns>Путь</returns>
        protected string GetSourcePathBySourceID(int sourceID)
        {
            return string.Format(
                "{0}{1}",
                Scheme.DataSourceManager.BaseDirectory,
                GetShortSourcePathBySourceID(sourceID));
        }

        /// <summary>
        /// Формирует путь к источнику по данным из коллекции источников данных
        /// </summary>
        /// <param name="sourceID">ИД источника</param>
        /// <returns>Путь</returns>
        protected DirectoryInfo GetSourceDirBySourceID(int sourceID)
        {
            return new DirectoryInfo(GetSourcePathBySourceID(sourceID));
        }

        /// <summary>
        /// Формирует укороченный путь к источнику по данным из коллекции источников данных
        /// </summary>
        /// <param name="sourceID">ИД источника</param>
        /// <returns>Путь</returns>
        protected string GetShortSourcePathBySourceID(int sourceID)
        {
            IDataSource ds = Scheme.DataSourceManager.DataSources[sourceID];
            if (ds == null) return string.Empty;

            string result = string.Format(
                "\\{0}\\{1}_{2}",
                ds.SupplierCode, ds.DataCode.PadLeft(4, '0'), ds.DataName);

            if (ds.BudgetName != string.Empty)
                result += string.Format("\\{0}", ds.BudgetName);
            if (ds.Year != 0)
                result += string.Format("\\{0}", ds.Year);
            if (ds.Month != 0)
                result += string.Format("\\{0}", ds.Month);
            if (ds.Variant != string.Empty)
                result += string.Format("\\{0}", ds.Variant);
            if (ds.Quarter != 0)
                result += string.Format("\\{0}", ds.Quarter);
            if (ds.Territory != string.Empty)
                result += string.Format("\\{0}", ds.Territory);

            return result;
        }

        /// <summary>
        /// Формирует укороченный путь к источнику по данным из коллекции источников данных
        /// </summary>
        /// <param name="sourceID">ИД источника</param>
        /// <returns>Путь</returns>
        protected DirectoryInfo GetShortSourceDirBySourceID(int sourceID)
        {
            return new DirectoryInfo(GetShortSourcePathBySourceID(sourceID));
        }

        /// <summary>
        /// Возвращает дату последней закачки из данного источника
        /// </summary>
        /// <param name="sourceID">ИД источника</param>
        /// <returns>Дата</returns>
        protected string GetLastPumpDateBySourceID(int sourceID)
        {
            if (sourceID < 0) return string.Empty;

            DataTable dt = (DataTable)this.DB.ExecQuery(string.Format(
                "select EVENTDATETIME from DATAPUMPPROTOCOL where DATASOURCEID = {0} and KINDSOFEVENTS = 104 " +
                "order by PUMPHISTORYID desc, EVENTDATETIME asc", sourceID), QueryResultTypes.DataTable);
            if (dt.Rows.Count == 0)
            {
                return string.Empty;
            }

            return Convert.ToString(dt.Rows[0]["EVENTDATETIME"]);
        }

        /// <summary>
        /// Возвращает список всех когда-либо закачанных данной закачкой источников
        /// </summary>
        /// <param name="pumpID">ИД закачки. Если -1, то не учитывается. Если >0, то выбираются источники по этому
        /// ИД закачки</param>
        /// <returns>Список источников</returns>
        protected Dictionary<int, string> GetAllPumpedDataSources(int pumpID)
        {
            Dictionary<int, string> result = new Dictionary<int, string>(100);
            DataTable dt;

            if (pumpID < 0)
            {
                dt = this.PumpRegistryElement.DataSources;
            }
            else
            {
                dt = this.DB.ExecQuery(
                    "select REFDATASOURCES as ID from DATASOURCES2PUMPHISTORY where REFPUMPHISTORY = ?",
                    QueryResultTypes.DataTable,
                    this.DB.CreateParameter("REFPUMPHISTORY", pumpID)) as DataTable;
            }

            int count = dt.Rows.Count;
            for (int i = 0; i < count; i++)
            {
                DataRow row = dt.Rows[i];
                if (Convert.ToInt32(dt.Rows[i]["Deleted"]) != 0)
                    continue;
                result.Add(Convert.ToInt32(row["ID"]), string.Empty);
            }

            return result;
        }

        /// <summary>
        /// Возвращает список всех когда-либо закачанных данной закачкой источников
        /// </summary>
        /// <returns>Список источников</returns>
        protected Dictionary<int, string> GetAllPumpedDataSources()
        {
            return GetAllPumpedDataSources(-1);
        }

        /// <summary>
        /// Возвращает интерфейс источника по его ИД
        /// </summary>
        /// <param name="id">ИД</param>
        /// <returns>Источник</returns>
        protected IDataSource GetDataSourceBySourceID(int id)
        {
            return this.Scheme.DataSourceManager.DataSources[id];
        }

        /// <summary>
        /// Создает сообщение со сводной информацией по закачанным и пропущенным источникам.
        /// </summary>
        /// <returns>Сообщение</returns>
        private string MakeDataSourcesVault()
        {
            string result = string.Empty;

            for (int i = 0; i < this.ProcessedSources.Keys.Count; i++)
            {
                result += string.Format(
                    "Источник {0}: {1} \n",
                    this.ProcessedSources.Keys[i],
                    this.ProcessedSources[this.ProcessedSources.Keys[i]]);
            }
            if (result != string.Empty)
            {
                result = "Результаты обработки источников: \n" + result;
            }

            return result;
        }

        /// <summary>
        /// Проверяет параметры источника на соответствие указанной дате
        /// </summary>
        /// <param name="date">Дата YYYYMMDD</param>
        /// <param name="generateException">Генерировать исключение при несоответствии или нет</param>
        protected bool CheckDataSourceByDate(int date, bool generateException)
        {
            int year = -1;
            int month = -1;
            int day = -1;
            CommonRoutines.DecodeNewDate(date, out year, out month, out day);

            if (this.DataSource.Year != year && (this.DataSource.ParametersType != ParamKindTypes.NoDivide))
            {
                if (generateException)
                    throw new Exception(string.Format("дата {0} не соответствует параметрам источника", date));
                else
                    return false;
            }

            if (this.DataSource.Month != month && (this.DataSource.ParametersType == ParamKindTypes.YearMonth ||
                this.DataSource.ParametersType == ParamKindTypes.YearMonthVariant))
            {
                if (generateException)
                {
                    throw new Exception(string.Format("дата {0} не соответствует параметрам источника", date));
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        #endregion Получение информации об источниках
    }
}