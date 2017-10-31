using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.Common;
using Krista.FM.Server.FinSourcePlanning.Services;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.FinSourcePlanning
{
    internal class Utils
    {

        public static int GetClassifierRowID(IScheme scheme, IDatabase db, string entityKey, int sourceID, string fieldName, string fieldValue)
        {
            IEntity entity = scheme.RootPackage.FindEntityByName(entityKey);
            DataTable dt;
            if (sourceID == -1)
            {
                dt = (DataTable)db.ExecQuery(
                    String.Format("select ID from {0} where {1} = ?", entity.FullDBName, fieldName),
                    QueryResultTypes.DataTable,
                    db.CreateParameter(fieldName, fieldValue));
            }
            else
            {
                dt = (DataTable)db.ExecQuery(
                    String.Format("select ID from {0} where SourceID = ? and {1} = ?",
                        entity.FullDBName, fieldName),
                    QueryResultTypes.DataTable,
                    db.CreateParameter("SourceID", sourceID),
                    db.CreateParameter(fieldName, fieldValue));
            }

            if (dt.Rows.Count == 1)
            {
                return Convert.ToInt32(dt.Rows[0][0]);
            }
            return 0;
        }

        #region получение данных из деталей

        #endregion

        #region работа с периодами

        /// <summary>
        /// вычисление конца периода по началу периода, количеству месяцев в периоде и дню конца периода
        /// </summary>
        protected static DateTime GetEndPeriod(DateTime startPeriod, int addMonthsCount, int day)
        {
            if (startPeriod.Day < day)
            {
                addMonthsCount--;
            }
            DateTime endPeriod = startPeriod.AddMonths(addMonthsCount);
            endPeriod = new DateTime(endPeriod.Year, endPeriod.Month, 1);
            int dayOfMonth = DateTime.DaysInMonth(endPeriod.Year, endPeriod.Month);
            if (dayOfMonth < day)
                return endPeriod.AddDays(dayOfMonth - 1);
            return endPeriod.AddDays(day - 1);
        }

        internal static DateTime GetEndPeriod(DateTime startDate, PayPeriodicity payPeriodicity, int payDay, int monthInc)
        {
            DateTime endPeriod = startDate;

            switch (payPeriodicity)
            {
                case PayPeriodicity.Year:
                    endPeriod = startDate.Month == 12 ?
                        GetDateTime(startDate.Year + 1, 12, payDay) : GetDateTime(startDate.Year, 12, payDay);
                    break;
                case PayPeriodicity.Quarter:
                    switch (startDate.Month)
                    {
                        case 1:
                        case 2:
                            endPeriod = GetDateTime(startDate.Year, 3, payDay);
                            break;
                        case 4:
                        case 5:
                            endPeriod = GetDateTime(startDate.Year, 6, payDay);
                            break;
                        case 7:
                        case 8:
                            endPeriod = GetDateTime(startDate.Year, 9, payDay);
                            break;
                        case 10:
                        case 11:
                            endPeriod = GetDateTime(startDate.Year, 12, payDay);
                            break;
                        case 3:
                        case 6:
                        case 9:
                            endPeriod = GetDateTime(startDate.Year, startDate.Month + monthInc, payDay);
                            break;
                        case 12:
                            endPeriod = payDay > startDate.Day ? GetDateTime(startDate.Year, 12, payDay) : GetDateTime(startDate.Year + 1, monthInc, payDay);
                            break;
                    }
                    break;
                case PayPeriodicity.HalfYear:
                    if (startDate.Month < 6)
                        endPeriod = GetDateTime(startDate.Year, payDay > startDate.Day ? 6 : 12, payDay);
                    if (startDate.Month >= 6 && startDate.Month < 12)
                        endPeriod = GetDateTime(startDate.Year, 12, payDay);
                    if (startDate.Month == 12)
                        endPeriod = payDay > startDate.Day ? GetDateTime(startDate.Year, 12, payDay) : GetDateTime(startDate.Year + 1, 6, payDay);
                    break;
                case PayPeriodicity.Month:
                    endPeriod = startDate.Day < payDay ?
                        GetDateTime(startDate.Year, startDate.Month, payDay) :
                        GetDateTime(startDate.Year, startDate.Month + 1, payDay);
                    break;
            }
            return endPeriod;
        }

        private static DateTime GetDateTime(int year, int month, int day)
        {
            if (month > 12)
            {
                month = month - 12;
                year++;
            }
            if (DateTime.DaysInMonth(year, month) < day)
                day = DateTime.DaysInMonth(year, month);
            return new DateTime(year, month, day);
        }

        /// <summary>
        /// Вычисляет количество периодов в финансовой операции.
        /// </summary>
        /// <param name="startDate">Дата начала.</param>
        /// <param name="endDate">Дата закрытия.</param>
        /// <param name="payPeriodicity">Переодичность выплат.</param>
        /// <param name="periods"></param>
        /// <returns>Количество периодов в финансовой операции.</returns>
        internal static int GetPeriodCount(DateTime startDate, DateTime endDate, PayPeriodicity payPeriodicity, DataTable periods)
        {
            return GetPeriodCount(startDate, endDate, payPeriodicity, periods, 0);
        }

        /// <summary>
        /// Вычисляет количество периодов в финансовой операции.
        /// </summary>
        internal static int GetPeriodCount(DateTime startDate, DateTime endDate,
            PayPeriodicity payPeriodicity, DataTable periods, int dayCount)
        {
            int value = 0;
            switch (payPeriodicity)
            {
                case PayPeriodicity.Day:
                    DateTime endPeriod = startDate;
                    while (endPeriod < endDate)
                    {
                        endPeriod = endPeriod.AddDays(dayCount);
                        value++;
                    }
                    return value;
                case PayPeriodicity.Other:
                    return periods.Rows.Count;
                case PayPeriodicity.Single:
                    return 1;
                case PayPeriodicity.Month:
                case PayPeriodicity.Quarter:
                case PayPeriodicity.HalfYear:
                case PayPeriodicity.Year:
                    endPeriod = startDate;
                    int subtract = -GetSubtractValue(payPeriodicity);
                    while (endPeriod < endDate)
                    {
                        endPeriod = endPeriod.AddDays(subtract);
                        value++;
                    }
                    return value;
            }
            return value;
        }

        /// <summary>
        /// заполнение плана погашения основного долга
        /// </summary>
        internal static int GetPeriodCount(DateTime startDate, DateTime endDate,
            PayPeriodicity payPeriodicity, int payDay, DataTable periods, bool firstDayInclude)
        {
            int increment = -GetSubtractValue(payPeriodicity);

            if (payPeriodicity == PayPeriodicity.Other)
            {
                startDate = Convert.ToDateTime(periods.Rows[0]["StartDate"]);
                endDate = Convert.ToDateTime(periods.Rows[periods.Rows.Count - 1]["EndDate"]);
            }

            DateTime startPeriod = startDate;
            DateTime endPeriod = DateTime.MinValue;

            if (payDay == 0)
                payDay = DateTime.DaysInMonth(startDate.Year, startDate.Month);

            int i = 0;
            // если конец периода не совпадает с 
            if (payPeriodicity != PayPeriodicity.Single && payDay >= 0)
            {
                endPeriod = GetEndPeriod(startPeriod, payPeriodicity, payDay, increment);
                i++;
                startPeriod = endPeriod.AddDays(1);
            }

            while (endPeriod < endDate)
            {
                if (i == 0 && !firstDayInclude)
                    startPeriod = startPeriod.AddDays(1);

                if (payPeriodicity == PayPeriodicity.Other)
                    endPeriod = Convert.ToDateTime(periods.Rows[i]["EndDate"]);
                else endPeriod = payPeriodicity == PayPeriodicity.Single ? endDate : GetEndPeriod(startPeriod, increment, payDay);

                if (endPeriod > endDate)
                    endPeriod = endDate;
                startPeriod = endPeriod.AddDays(1);
                i++;
            }
            return i;
        }

        internal static int GetSubtractValue(PayPeriodicity payPeriodicity)
        {
            switch (payPeriodicity)
            {
                case PayPeriodicity.Month:
                    return -1;
                case PayPeriodicity.Quarter:
                    return -3;
                case PayPeriodicity.HalfYear:
                    return -6;
                case PayPeriodicity.Year:
                    return -12;
                default:
                    return 0;
            }
        }

        public static int GetYearBase(int year)
        {
            return (year % 4) == 0 ? 366 : 365;
        }

        public static int GetVariantBaseYear(IScheme scheme, int variantID)
        {
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                object result = db.ExecQuery(string.Format("select CurrentYear from d_Variant_Borrow where id = {0}", variantID),
                             QueryResultTypes.Scalar);
                return Convert.ToInt32(result);
            }
        }

        #endregion

        #region источники данных

        internal static int GetDataSourceId(IScheme scheme, IDatabase db, string supplier, int dataCodeMain, int dataCodeSecond, int year, bool createDataSource)
        {
            object sourceID = db.ExecQuery(
                "select ID from DataSources where SupplierCode = ? and DataCode = ? and Year = ? and deleted = 0",
                QueryResultTypes.Scalar,
                 new System.Data.OleDb.OleDbParameter("SupplierCode", supplier),
                 new System.Data.OleDb.OleDbParameter("DataCode", dataCodeMain),
                 new System.Data.OleDb.OleDbParameter("Year", year));

            if (sourceID == null)
            {
                sourceID = db.ExecQuery(
                    "select ID from DataSources where SupplierCode = ? and DataCode = ? and Year = ? and deleted = 0",
                    QueryResultTypes.Scalar,
                    new System.Data.OleDb.OleDbParameter("SupplierCode", supplier),
                    new System.Data.OleDb.OleDbParameter("DataCode", dataCodeSecond),
                    new System.Data.OleDb.OleDbParameter("Year", year));
            }

            if (!createDataSource)
            {
                if (sourceID == null)
                    return -1;
                return Convert.ToInt32(sourceID);
            }

            if (sourceID == null)
            {
                IDataSource ds = scheme.DataSourceManager.DataSources.CreateElement();
                ds.SupplierCode = supplier;
                ds.DataCode = dataCodeMain.ToString();
                ds.DataName = "Проект бюджета";
                ds.Year = year;
                ds.ParametersType = ParamKindTypes.Year;
                return ds.Save();
            }
            return Convert.ToInt32(sourceID);
        }

        /// <summary>
        /// Ищет источник данных с указанными параметрами.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="scheme"></param>
        /// <param name="supplier">Поставщик.</param>
        /// <param name="dataCodeMain">Поставщик.</param>
        /// <param name="dataCodeSecond">Поставщик альтернативный.</param>
        /// <param name="year">Параметр "Год"</param>
        /// <returns>ID источника данных.</returns>
        internal static int GetDataSourceId(IScheme scheme, IDatabase db, string supplier, int dataCodeMain, int dataCodeSecond, int year)
        {
            return GetDataSourceId(scheme, db, supplier, dataCodeMain, dataCodeSecond, year, true);
        }

        internal static int GetDataSourceID(IScheme scheme, IDatabase db, string supplier, int dataCodeMain, int dataCodeSecond, int year, bool createDataSource)
        {
            object sourceID = db.ExecQuery(
                "select ID from DataSources where SupplierCode = ? and DataCode = ? and Year = ? and deleted = 0",
                QueryResultTypes.Scalar,
                 db.CreateParameter("SupplierCode", supplier),
                 db.CreateParameter("DataCode", dataCodeMain),
                 db.CreateParameter("Year", year));

            if (sourceID == null)
            {
                sourceID = db.ExecQuery(
                    "select ID from DataSources where SupplierCode = ? and DataCode = ? and Year = ? and deleted = 0",
                    QueryResultTypes.Scalar,
                    db.CreateParameter("SupplierCode", supplier),
                    db.CreateParameter("DataCode", dataCodeSecond),
                    db.CreateParameter("Year", year));
            }

            if (!createDataSource)
            {
                if (sourceID == null)
                    return -1;
                return Convert.ToInt32(sourceID);
            }

            if (sourceID == null)
            {
                IDataSource ds = scheme.DataSourceManager.DataSources.CreateElement();
                ds.SupplierCode = supplier;
                ds.DataCode = dataCodeMain.ToString();
                ds.DataName = "Проект бюджета";
                ds.Year = year;
                ds.ParametersType = ParamKindTypes.Year;
                return ds.Save();
            }
            return Convert.ToInt32(sourceID);
        }

        /// <summary>
        /// Ищет источник данных с указанными параметрами.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="supplier">Поставщик.</param>
        /// <param name="dataCodeMain">Поставщик.</param>
        /// <param name="dataCodeSecond">Поставщик альтернативный.</param>
        /// <param name="year">Параметр "Год"</param>
        /// <returns>ID источника данных.</returns>
        internal static int GetDataSourceID(IScheme scheme, IDatabase db, string supplier, int dataCodeMain, int dataCodeSecond, int year)
        {
            return GetDataSourceID(scheme, db, supplier, dataCodeMain, dataCodeSecond, year, true);
        }

        #endregion

        #region классификаторы данных и таблицы фактов

        /// <summary>
        /// поиск и добавление записи в классификатор
        /// </summary>
        public static int GetClassifierRowID(IScheme scheme, IDatabase db, string entityKey, int sourceID, string codeFieldName, string codeFieldValue, string nameFieldValue)
        {
            return GetClassifierRowID(scheme, db, entityKey, sourceID, codeFieldName, codeFieldValue, nameFieldValue, true);
        }

        /// <summary>
        /// поиск и добавление записи в классификатор (добавление записи на выбор)
        /// </summary>
        public static int GetClassifierRowID(IScheme scheme, IDatabase db, string entityKey,
            int sourceID, string codeFieldName, string codeFieldValue, string nameFieldValue, bool addToClassifier)
        {
            IEntity entity = scheme.RootPackage.FindEntityByName(entityKey);
            object classifierId = null;
            if (sourceID == -1)
            {
                classifierId = db.ExecQuery(
                    String.Format("select ID from {0} where {1} = ?", entity.FullDBName, codeFieldName),
                    QueryResultTypes.Scalar,
                    new DbParameterDescriptor(codeFieldName, codeFieldValue));
            }
            else
            {
                classifierId = db.ExecQuery(
                    String.Format("select ID from {0} where SourceID = ? and {1} = ?",
                        entity.FullDBName, codeFieldName),
                    QueryResultTypes.Scalar,
                    new DbParameterDescriptor("SourceID", sourceID),
                    new DbParameterDescriptor(codeFieldName, codeFieldValue));
            }

            if (classifierId != null && classifierId != DBNull.Value)
            {
                return Convert.ToInt32(classifierId);
            }
            if (!addToClassifier)
                return -1;
            return AddClassifierRow(sourceID, codeFieldValue, codeFieldName, nameFieldValue, entity);
        }

        /// <summary>
        /// Возвращает ставку ЦБ на указанный период.
        /// </summary>
        /// <remarks>
        /// За указанный период может быть несколько изменений ставки ЦБ. 
        /// Функция возвращает наиболее раннее изменение ставки.
        /// ВАЖНО! Необходимо сделать чтобы возвращались субпериоды [Дата, Ставка].
        /// </remarks>
        /// <param name="dt"></param>
        /// <param name="startDate">Дата начала периода.</param>
        /// <param name="endDate">Дата конца периода.</param>
        /// <returns>Ставку ЦБ.</returns>
        internal static decimal GetRateCBOnDateRange(DataTable dt, DateTime startDate, DateTime endDate)
        {
            return GetRateCBOnDateRange(dt, "PercentRate", startDate, endDate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="percentName"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        internal static decimal GetRateCBOnDateRange(DataTable dt, string percentName, DateTime startDate, DateTime endDate)
        {
            if (!dt.Columns.Contains(percentName))
                throw new Exception(String.Format("Столбец {0} не был найден в детали 'Журнал ставок процентов'", percentName));
            DataRow[] rows = dt.Select(String.Format("ChargeDate < '{0}'", endDate), "ChargeDate DESC");
            if (rows.Length > 0)
            {
                if (rows[0].IsNull(percentName))
                    throw new Exception(String.Format("Журнал ставок процентов. Поле {0} записи с ID = {1} не заполнено", percentName, rows[0]["ID"]));

                return Convert.ToDecimal(rows[0][percentName]);
            }
            throw new Exception(String.Format("Не удалось найти ставку процентов ЦБ на дату {0}. Заполните классификатор \"Журнал ставок ЦБ\".", endDate.Subtract(new TimeSpan(1, 0, 0, 0))));
        }

        public static DataTable GetEntityTable(IScheme scheme, string entityKey)
        {
            DataTable table = new DataTable();
            IEntity entity = scheme.RootPackage.FindEntityByName(entityKey);
            using (IDataUpdater du = entity.GetDataUpdater())
            {
                du.Fill(ref table);
            }
            return table;
        }

        /// <summary>
        /// возвращает последний актуальный курс данной валюты
        /// </summary>
        public static decimal GetLastCurrencyExchange(IScheme scheme, DateTime lastDate, int refOkv)
        {
            DataTable dt;
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                string query = "select ExchangeRate from d_S_ExchangeRate where RefOKV = ? and DateFixing <= ? order by DateFixing";
                IDbDataParameter[] prms = new IDbDataParameter[2];
                prms[0] = new System.Data.OleDb.OleDbParameter("RefOKV", refOkv);
                prms[1] = new System.Data.OleDb.OleDbParameter("DateFixing", lastDate);
                dt = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable, prms);
            }
            if (dt.Rows.Count > 0)
                return Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1][0]);
            return -1;
        }

        /// <summary>
        /// получение наименования валюты
        /// </summary>
        /// <param name="refOKV"></param>
        /// <returns></returns>
        internal static string GetCurrencyName(IScheme scheme,  int refOKV)
        {
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                string query = string.Format("select Name from d_OKV_Currency where ID = {0}", refOKV);
                return db.ExecQuery(query, QueryResultTypes.Scalar).ToString();
            }
        }

        /// <summary>
        /// добавляем запись в классификатор со всеми значениями по умолчанию
        /// </summary>
        /// <param name="sourceID"></param>
        /// <param name="code"></param>
        /// <param name="codeColumn"></param>
        /// <param name="name"></param>
        /// <param name="classifier"></param>
        /// <returns></returns>
        internal static int AddClassifierRow(int sourceID, string code, string codeColumn, string name, IEntity classifier)
        {
            using (IDataUpdater du = classifier.GetDataUpdater("1 = 2", null, null))
            {
                DataTable dt = new DataTable();
                du.Fill(ref dt);
                DataRow row = dt.NewRow();
                int id = classifier.GetGeneratorNextValue;
                row.BeginEdit();
                foreach (IDataAttribute attr in classifier.Attributes.Values)
                {
                    if (attr.Name == "ID")
                    {
                        row["ID"] = id;
                    }
                    else if (string.Compare(attr.Name, "SourceID", true) == 0)
                    {
                        row[attr.Name] = sourceID;
                    }
                    else if (string.Compare(attr.Name, codeColumn, true) == 0)
                    {
                        row[codeColumn] = code;
                    }
                    else if (string.Compare(attr.Name, "Name", true) == 0)
                    {
                        row[attr.Name] = name;
                    }
                    else
                    {
                        row[attr.Name] = attr.DefaultValue ?? DBNull.Value;
                    }
                }
                row.EndEdit();
                dt.Rows.Add(row);
                du.Update(ref dt);
                return id;
            }
        }

        /// <summary>
        /// получение уровня бюджета в зависимости от константы
        /// </summary>
        /// <param name="constValue"></param>
        /// <returns></returns>
        public static int GetBudgetLevel(int constValue)
        {
            switch (constValue)
            {
                case 1:
                    return 1;
                case 3:
                    return 3;
                case 4:
                    return 5;
                case 5:
                    return 16;
                case 6:
                    return 17;
                case 7:
                    return 15;
                case 8:
                    return 3;
                case 11:
                    return 6;
            }
            return 0;
        }

        #endregion

        #region данные деталей

        /// <summary>
        /// получение данных детали с определенным фильтром и значениями параметров фильтра
        /// </summary>
        /// <param name="masterID"></param>
        /// <param name="detailAssociation"></param>
        /// <param name="scheme"></param>
        /// <param name="detailFilter"></param>
        /// <param name="paramsValues"></param>
        /// <returns></returns>
        internal static DataTable GetDetailTable(IScheme scheme, int masterID, string detailAssociation,
            string detailFilter, params object[] paramsValues)
        {
            DataTable dt = new DataTable();

            IEntityAssociation associationEntity =
                scheme.RootPackage.FindAssociationByName(detailAssociation);
            string refMasterFieldName = associationEntity.RoleDataAttribute.Name;
            IEntity detailEntity = associationEntity.RoleData;

            List<IDbDataParameter> paramsList = new List<IDbDataParameter>();
            foreach (object paramValue in paramsValues)
            {
                paramsList.Add(new System.Data.OleDb.OleDbParameter(string.Format("p{0}", paramsList.Count), paramValue));
            }
            if (string.IsNullOrEmpty(detailFilter))
                detailFilter = string.Format("{0} = {1}", refMasterFieldName, masterID);
            else
                detailFilter = detailFilter + string.Format(" and {0} = {1}", refMasterFieldName, masterID);
            using (IDataUpdater du = detailEntity.GetDataUpdater(detailFilter, null, paramsList.ToArray()))
            {
                du.Fill(ref dt);
            }
            return dt;
        }

        public static DataTable GetDetailTable(IScheme scheme, IDatabase db, int masterID, string associationKey)
        {
            DataTable dt = new DataTable();

            IEntityAssociation entityAssociation =
                scheme.RootPackage.FindAssociationByName(associationKey);
            IEntity entity = entityAssociation.RoleData;
            string refMasterFieldName = entityAssociation.RoleDataAttribute.Name;

            IDbDataParameter[] prms = new IDbDataParameter[1];
            prms[0] = new System.Data.OleDb.OleDbParameter(refMasterFieldName, masterID);
            using (IDataUpdater du = entity.GetDataUpdater(
                String.Format("{0} = ?", refMasterFieldName), null, prms))
            {
                du.Fill(ref dt);
            }
            return dt;
        }

        #endregion
    }
}
