using System;
using System.Collections.Generic;
using System.Data;

using Krista.FM.Common;
using Krista.FM.Client.Workplace.Gui;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server
{
	internal class Utils
	{
        /// <summary>
        /// вычисление конца периода по началу периода, количеству месяцев в периоде и дню конца периода
        /// </summary>
        internal static DateTime GetEndPeriod(DateTime startPeriod, int addMonthsCount, int day)
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
                    endPeriod = startDate.Month == 12 && startDate.Day >= payDay ?
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
                    endPeriod = startDate.Day <= payDay ?
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
            bool singlePeriod = false;

            if (payPeriodicity == PayPeriodicity.Other)
            {
                startDate = Convert.ToDateTime(periods.Rows[0]["StartDate"]);
                endDate = Convert.ToDateTime(periods.Rows[periods.Rows.Count - 1]["EndDate"]);
                singlePeriod = periods.Rows.Count == 1;
            }

            DateTime startPeriod = startDate;
            DateTime endPeriod = DateTime.MinValue;

            if (payDay == 0)
                payDay = DateTime.DaysInMonth(startDate.Year, startDate.Month);

            int i = 0;
            // если конец периода не совпадает с 
            if (payPeriodicity != PayPeriodicity.Single && payDay >= 0 && !singlePeriod)
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

        /// <summary>
        /// Ищет источник данных с указанными параметрами.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="supplier">Поставщик.</param>
        /// <param name="dataCodeMain">Поставщик.</param>
        /// <param name="dataCodeSecond">Поставщик альтернативный.</param>
        /// <param name="year">Параметр "Год"</param>
        /// <returns>ID источника данных.</returns>
        internal static int GetDataSourceID(IDatabase db, string supplier, int dataCodeMain, int dataCodeSecond, int year)
        {
            object sourceID = db.ExecQuery(
                "select ID from DataSources where SupplierCode = ? and DataCode = ? and Year = ? and deleted = 0",
                QueryResultTypes.Scalar,
                 new System.Data.OleDb.OleDbParameter("SupplierCode", supplier),
                 new System.Data.OleDb.OleDbParameter("DataCode", dataCodeMain),
                 new System.Data.OleDb.OleDbParameter("Year", year));

            if (sourceID == null || sourceID == DBNull.Value)
            {
                sourceID = db.ExecQuery(
                    "select ID from DataSources where SupplierCode = ? and DataCode = ? and Year = ? and deleted = 0",
                    QueryResultTypes.Scalar,
                    new System.Data.OleDb.OleDbParameter("SupplierCode", supplier),
                    new System.Data.OleDb.OleDbParameter("DataCode", dataCodeSecond),
                    new System.Data.OleDb.OleDbParameter("Year", year));
            }

            if (sourceID == null || sourceID == DBNull.Value)
            {
                IDataSource ds = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme.DataSourceManager.DataSources.CreateElement();
                ds.SupplierCode = supplier;
                ds.DataCode = dataCodeMain.ToString();
                ds.DataName = "Проект бюджета";
                ds.Year = year;
                ds.ParametersType = ParamKindTypes.Year;
                return ds.Save();
            }
            return Convert.ToInt32(sourceID);
        }

		public static int GetRowsCount(string objectKey, IScheme scheme, string filter, params IDbDataParameter[] dbParams)
		{
		    IEntity entity = scheme.RootPackage.FindEntityByName(objectKey);
            using (IDatabase db = scheme.SchemeDWH.DB)
		    {
		        string query = string.Format("select Count(ID) from {0}", entity.FullDBName);
                if (!string.IsNullOrEmpty(filter))
                    query += " where " + filter;
		        return Convert.ToInt32(db.ExecQuery(query, QueryResultTypes.Scalar, dbParams));
		    }
		}

        public static int GetClassifierRowID(IDatabase db, string entityKey,
            int sourceID, string codeFieldName, string codeFieldValue, string nameFieldValue, bool addToClassifier)
        {
            IEntity entity = WorkplaceSingleton.Workplace.ActiveScheme.RootPackage.FindEntityByName(entityKey);
            DataTable dt;
            if (sourceID == -1)
            {
                dt = (DataTable)db.ExecQuery(
                    String.Format("select ID from {0} where {1} = ?", entity.FullDBName, codeFieldName),
                    QueryResultTypes.DataTable,
                    new System.Data.OleDb.OleDbParameter(codeFieldName, codeFieldValue));
            }
            else
            {
                dt = (DataTable)db.ExecQuery(
                    String.Format("select ID from {0} where SourceID = ? and {1} = ?",
                        entity.FullDBName, codeFieldName),
                    QueryResultTypes.DataTable,
                    new System.Data.OleDb.OleDbParameter("SourceID", sourceID),
                    new System.Data.OleDb.OleDbParameter(codeFieldName, codeFieldValue));
            }

            if (dt.Rows.Count == 1)
            {
                return Convert.ToInt32(dt.Rows[0][0]);
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
		public static decimal GetRateCBOnDateRange(DataTable dt, DateTime startDate, DateTime endDate)
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
        public static decimal GetRateCBOnDateRange(DataTable dt, string percentName, DateTime startDate, DateTime endDate)
        {
            if (!dt.Columns.Contains(percentName))
                throw new FinSourcePlanningException(String.Format("Столбец {0} не был найден в детали 'Журнал ставок процентов'", percentName));
            DataRow[] rows = dt.Select(String.Format("ChargeDate < '{0}'", endDate), "ChargeDate DESC");
            if (rows.Length > 0)
            {
                if (rows[0].IsNull(percentName))
                    throw new FinSourcePlanningException(String.Format("Журнал ставок процентов. Поле {0} записи с ID = {1} не заполнено", percentName, rows[0]["ID"]));

                return Convert.ToDecimal(rows[0][percentName]);
            }
            throw new FinSourcePlanningException(String.Format("Не удалось найти ставку процентов ЦБ на дату {0}. Заполните классификатор \"Журнал ставок ЦБ\".", endDate.Subtract(new TimeSpan(1, 0, 0, 0))));
        }
        
		public static DataTable GetEntityTable(string entityKey)
		{
			DataTable table = new DataTable();
			IEntity entity = WorkplaceSingleton.Workplace.ActiveScheme.RootPackage.FindEntityByName(entityKey);
			using (IDataUpdater du = entity.GetDataUpdater())
			{
				du.Fill(ref table);
			}
			return table;
		}

        /// <summary>
        /// получение данных детали с определенным фильтром и значениями параметров фильтра
        /// </summary>
        /// <param name="masterID"></param>
        /// <param name="detailAssociation"></param>
        /// <param name="scheme"></param>
        /// <param name="detailFilter"></param>
        /// <param name="paramsValues"></param>
        /// <returns></returns>
        public static DataTable GetDetailTable(int masterID, string detailAssociation,
            IScheme scheme, string detailFilter, params object[] paramsValues)
        {
            DataTable dt = new DataTable();

            IEntityAssociation associationEntity =
                WorkplaceSingleton.Workplace.ActiveScheme.RootPackage.FindAssociationByName(detailAssociation);
            string refMasterFieldName = associationEntity.RoleDataAttribute.Name;
            IEntity detailEntity = associationEntity.RoleData;

            List <IDbDataParameter> paramsList = new List<IDbDataParameter>();
            foreach (object paramValue in paramsValues)
            {
                paramsList.Add(new DbParameterDescriptor(string.Format("p{0}", paramsList.Count), paramValue));
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

		public static DataTable GetDetailTable(IDatabase db, int masterID, string associationKey)
		{
			DataTable dt = new DataTable();

			IEntityAssociation entityAssociation =
				WorkplaceSingleton.Workplace.ActiveScheme.RootPackage.FindAssociationByName(associationKey);
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

        public static DataTable GetDetailTable(IDatabase db, int masterID, string associationKey, VersionParams calcVersion)
        {
            DataTable dt = new DataTable();

            IEntityAssociation entityAssociation =
                WorkplaceSingleton.Workplace.ActiveScheme.RootPackage.FindAssociationByName(associationKey);
            IEntity entity = entityAssociation.RoleData;
            string refMasterFieldName = entityAssociation.RoleDataAttribute.Name;

            string filter = String.Format("{0} = ?", refMasterFieldName);
            var prms = new List<IDbDataParameter>();
            prms.Add(new DbParameterDescriptor("p0", masterID));
            if (calcVersion.CalculationDate != null)
            {
                prms.Add(new DbParameterDescriptor("p1", calcVersion.CalculationDate));
                prms.Add(new DbParameterDescriptor("p2", calcVersion.CalculationComment));
                filter += " and EstimtDate = ? and CalcComment = ?";
            }
            else
            {
                filter += " and EstimtDate is null and CalcComment is null";
            }

            using (IDataUpdater du = entity.GetDataUpdater(filter, null, prms.ToArray()))
            {
                du.Fill(ref dt);
            }
            return dt;
        }

        public static int GetYearBase(int year)
        {
            return (year % 4) == 0 ? 366 : 365;
        }

        /// <summary>
        /// возвращает последний актуальный курс данной валюты
        /// </summary>
        public static decimal GetLastCurrencyExchange(DateTime lastDate, int refOKV)
        {
            bool isPrognoz;
            return GetLastCurrencyExchange(lastDate, refOKV, out isPrognoz);
        }

        /// <summary>
        /// возвращает последний актуальный курс данной валюты
        /// </summary>
        public static decimal GetLastCurrencyExchange(DateTime lastDate, int refOKV, out bool isPrognozRate)
        {
            IScheme scheme = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme;
            DataTable dt;
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                string query = "select ExchangeRate, IsPrognozExchRate from d_S_ExchangeRate where RefOKV = ? and DateFixing <= ? order by DateFixing";
                IDbDataParameter[] prms = new IDbDataParameter[2];
                prms[0] = new DbParameterDescriptor("RefOKV", refOKV);
                prms[1] = new DbParameterDescriptor("DateFixing", lastDate);
                dt = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable, prms);
            }
            if (dt.Rows.Count > 0)
            {
                isPrognozRate = Convert.ToBoolean(dt.Rows[dt.Rows.Count - 1][1]);
                return Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1][0]);
            }
            isPrognozRate = false;
            return -1;
        }

        /// <summary>
        /// получение наименования валюты
        /// </summary>
        /// <param name="refOKV"></param>
        /// <returns></returns>
        public static string GetCurrencyName(int refOKV)
        {
            IScheme scheme = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme;
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                string query = string.Format("select Name from d_OKV_Currency where ID = {0}", refOKV);
                return db.ExecQuery(query, QueryResultTypes.Scalar).ToString();
            }
            
        }

        /// <summary>
        /// получение кода валюты
        /// </summary>
        /// <param name="refOKV"></param>
        /// <returns></returns>
        public static object GetCurrencyCode(object refOKV)
        {
            IScheme scheme = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme;
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                string query = string.Format("select Code from d_OKV_Currency where ID = {0}", refOKV);
                return db.ExecQuery(query, QueryResultTypes.Scalar);
            }
        }

        public static int AddClassifierRow(int sourceID, string code, string codeColumn, string name, IEntity classifier)
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

        public static int GetVariantBaseYear(int variantID)
        {
            using (IDatabase db = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme.SchemeDWH.DB)
            {
                object result = db.ExecQuery(string.Format("select CurrentYear from d_Variant_Borrow where id = {0}", variantID),
                             QueryResultTypes.Scalar);
                return Convert.ToInt32(result);
            }
        }

        /// <summary>
        /// новый вариант при переводе договора с одного варианта в другой
        /// </summary>
        public static bool SetNewVariant(DataRow row, IEntity entity, IScheme scheme)
        {
            int refVariant = Convert.ToInt32(row["RefVariant"]);
            int newVariant = refVariant;
            int refStatusPlan = -1;
            if (entity.ObjectKey == SchemeObjectsKeys.f_S_Capital_Key)
            {
                refStatusPlan = Convert.ToInt32(row["RefStatusPlan"]);
                switch (refVariant)
                {
                    case 0:
                        if (refStatusPlan == 3)
                            newVariant = -2;
                        break;
                    case -2:
                        newVariant = 0;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                refStatusPlan = Convert.ToInt32(row["RefSStatusPlan"]);
                switch (refVariant)
                {
                    case 0:
                        if (refStatusPlan == 2 || refStatusPlan == 3 || refStatusPlan == 4)
                            newVariant = -2;
                        break;
                    case -2:
                        newVariant = 0;
                        break;
                    default:
                        if (refStatusPlan == 0 || refStatusPlan == 2 || refStatusPlan == 3
                            || refStatusPlan == 4 || refStatusPlan == 5)
                            newVariant = 0;
                        break;
                }
            }

            if (newVariant != refVariant)
            {
                // если новый и старый варианты разные, переведем запись на этот самый новый
                using (IDatabase db = scheme.SchemeDWH.DB)
                {
                    db.ExecQuery(
                        string.Format("update {0} set RefVariant = {1} where ID = {2}", entity.FullDBName, newVariant,
                        row["ID"]), QueryResultTypes.NonQuery);
                    // если могут быть подчиненные записи, то их тоже переносим
                    if (row.Table.Columns.Contains("ParentID"))
                    {
                        db.ExecQuery(
                            string.Format("update {0} set RefVariant = {1} where ParentID = {2}", entity.FullDBName, newVariant,
                            row["ID"]), QueryResultTypes.NonQuery);
                    }
                    // из таблицы удалим запись, так как она уже в новом варианте
                    row.Delete();
                    row.AcceptChanges();
                }
                return true;
            }
            return false;
        }

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

        public static int GetBudgetLevel(IScheme scheme)
        {
            int constBudLevel = Convert.ToInt32(scheme.GlobalConstsManager.Consts["TerrPartType"].Value);
            switch (constBudLevel)
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
	}
}
