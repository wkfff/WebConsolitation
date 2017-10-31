using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Domain.Services.FinSourceDebtorBook.Reports
{
    /// <summary>
    /// ќбщие функции
    /// </summary>
    public partial class ReportsDataService
    {
        private readonly IScheme scheme;

        public ReportsDataService(IScheme scheme)
        {
            this.scheme = scheme;
        }

        /// <summary>
        /// создание структуры дл€ таблицы
        /// </summary>
        public static string GetFieldNames(IEntity entity, string prefix)
        {
            var fieldStr = String.Empty;
            
            if (prefix != String.Empty)
            {
                prefix = prefix + ".";
            }

            // обычные пол€
            foreach (var attr in entity.Attributes.Values)
            {
                fieldStr = String.Format("{0},{1}{2}", fieldStr, prefix, attr.Name);
            }

            // ссылочные пол€
            foreach (var attr in entity.Associations.Values)
            {
                fieldStr = string.Format("{0},{1}{2}", fieldStr, prefix, attr.RoleDataAttribute.Name);
            }

            return fieldStr.TrimStart(',');
        }

        /// <summary>
        /// создание структуры дл€ таблицы
        /// </summary>
        public static string GetFieldNames2(IEntity entity, string prefix)
        {
            var fieldStr = String.Empty;
            
            if (prefix != String.Empty)
            {
                prefix = prefix + ".";
            }

            // обычные пол€
            foreach (var attr in entity.Attributes.Values)
            {
                fieldStr = String.Format("{0},{1}{2}", fieldStr, prefix, attr.Name);
            }

            return fieldStr.TrimStart(',');
        }

        public static string GetDateValue(object dateCell)
        {
            var strDate = String.Empty;

            if (dateCell != DBNull.Value)
            {
                DateTime outDate;

                if (DateTime.TryParse(Convert.ToString(dateCell), out outDate))
                {
                    strDate = outDate.ToString("d", GetRuCulture());                    
                }
            }

            return strDate;
        }

        /// <summary>
        /// создание структуры дл€ таблицы
        /// </summary>
        public static DataTable CreateReportCaptionTable(int columnCount)
        {
            var dt = new DataTable("ReportTable");

            for (var i = 0; i < columnCount; i++)
            {
                dt.Columns.Add(string.Format("ReportField{0}", i), typeof(string));
            }

            return dt;
        }

        public static DataTable FilterDataSet(DataTable dt, string filterStr)
        {
            var tblTemp = dt.Clone();
            var rows = dt.Select(filterStr);

            for (var i = 0; i < rows.Length; i++)
            {
                tblTemp.ImportRow(rows[i]);
            }

            tblTemp.AcceptChanges();
            return tblTemp;
        }

        public static DataTable SortDataSet(DataTable dt, string orderStr)
        {
            var tblTemp = dt.Clone();
            var rows = dt.Select(String.Empty, orderStr);

            for (var i = 0; i < rows.Length; i++)
            {
                tblTemp.ImportRow(rows[i]);
            }

            tblTemp.AcceptChanges();
            return tblTemp;
        }

        public static double GetDoubleValue(object dateCell)
        {
            double value = 0;

            if (dateCell != DBNull.Value)
            {
                double parseValue;
                if (!double.TryParse(dateCell.ToString(), out parseValue))
                {
                    parseValue = 0;
                }

                value = parseValue;
            }

            return value;
        }

        public double GetNumber(object obj)
        {
            double result;
            
            if (!double.TryParse(obj.ToString(), out result))
            {
                result = 0;
            }

            return result;
        }

        private static DataRow FindDataRow(DataTable dt, string pattern, string fieldName)
        {
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][fieldName].ToString() == pattern)
                {
                    return dt.Rows[i];
                }
            }

            return null;
        }

        private static CultureInfo GetRuCulture()
        {
            return CultureInfo.CreateSpecificCulture("ru-RU");
        }

        private static string GetRegionUserTitle(IScheme schema, object regionId)
        {
            var entity = schema.RootPackage.FindEntityByName(DomainObjectsKeys.d_S_TitleReport);
            var result = String.Empty;

            using (var du = entity.GetDataUpdater(String.Format("RefRegion = {0}", regionId), null))
            {
                var tblBook = new DataTable();
                du.Fill(ref tblBook);

                if (tblBook.Rows.Count > 0)
                {
                    result = Convert.ToString(tblBook.Rows[0]["LastName"]);
                }
            }

            return result;
        }

        private static string GetBookValue(IScheme schema, string book, string key, string fieldName = "Name")
        {
            var entity = schema.RootPackage.FindEntityByName(book);
            var result = String.Empty;

            if (key.Length > 0)
            {
                using (var du = entity.GetDataUpdater(String.Format("ID = {0}", key), null))
                {
                    var tblBook = new DataTable();
                    du.Fill(ref tblBook);

                    if (tblBook.Rows.Count > 0)
                    {
                        result = tblBook.Rows[0][fieldName].ToString();
                    }
                }
            }

            return result;
        }

        private static string GetMonthText(int monthCount)
        {
            if (monthCount == 1)
            {
                return "мес€ц";
            }

            if (monthCount < 5)
            {
                return "мес€ца";
            }

            return "мес€цев";
        }

        private static string GetFullSettlePath(DataTable tblBook, string parentIds)
        {
            var result = string.Empty;
            var drsSettles = tblBook.Select(String.Format("ParentID in ({0})", parentIds));

            for (var i = 0; i < drsSettles.Length; i++)
            {
                var refTerr = Convert.ToInt32(drsSettles[i]["RefTerr"]);
                var id = Convert.ToString(drsSettles[i]["id"]);

                if (refTerr == 5 || refTerr == 6)
                {
                    result = String.Format("{0},{1}", result, id);
                }
                else
                {
                    var settleList = GetFullSettlePath(tblBook, id);
                    
                    if (settleList != String.Empty)
                    {
                        result = String.Format("{0},{1}", result, settleList);
                    }
                }
            }

            return result.Trim(',');
        }

        private static string GetRegionSettles(IDatabase db, IEntity rgnEntity, int regionID, ref string regionName, int parentRegionType)
        {
            var regionChildsID = string.Empty;
            
            // данные по районам (refterr 4, 7)
            var regionsQuery = "select {0} from {1} region where region.ID = {2}";
            var regionChildsQuery = "select {0} from {1} region where region.ParentID = {2} order by code";

            // ¬ыбираем тип региона и его поселени€
            var tblRegion = (DataTable)db.ExecQuery(
                String.Format(regionsQuery, GetFieldNames(rgnEntity, "region"), rgnEntity.FullDBName, regionID), QueryResultTypes.DataTable);
            if (tblRegion.Rows.Count > 0)
            {
                var regionType = Convert.ToInt32(tblRegion.Rows[0]["RefTerr"]);
                regionName = tblRegion.Rows[0]["Name"].ToString();
                if (regionType == parentRegionType)
                {
                    var tblChildsLvl1 = (DataTable)db.ExecQuery(
                            String.Format(
                                regionChildsQuery, 
                                GetFieldNames(rgnEntity, "region"),
                                rgnEntity.FullDBName, 
                                regionID), 
                            QueryResultTypes.DataTable);

                    foreach (DataRow rowLvl1 in tblChildsLvl1.Rows)
                    {
                        var tblChildsLvl2 = (DataTable)db.ExecQuery(
                                String.Format(
                                    regionChildsQuery, 
                                    GetFieldNames(rgnEntity, "region"),
                                    rgnEntity.FullDBName,
                                    rowLvl1["ID"]), 
                                QueryResultTypes.DataTable);

                        foreach (DataRow rowLvl2 in tblChildsLvl2.Rows)
                        {
                            regionChildsID = String.Format("{0},{1}", regionChildsID, rowLvl2["ID"]);
                        }
                    }

                    regionChildsID = regionChildsID.TrimStart(',');
                }
            }

            return regionChildsID;
        }

        private static string GetUNVMonthBound(DateTime calcDate, bool monthStart)
        {
            var dayNum = 0;

            if (!monthStart)
            {
                dayNum = DateTime.DaysInMonth(calcDate.Year, calcDate.Month);
            }

            return String.Format(
                "{0}{1}{2}",
                calcDate.Year,
                calcDate.Month.ToString().PadLeft(2, '0'),
                dayNum.ToString().PadLeft(2, '0'));
        }

        private static string CombineStrings(params string[] strData)
        {
            var strBuilder = new StringBuilder();

            foreach (var t in strData)
            {
                strBuilder.Append(t);
                strBuilder.Append(',');
            }
            
            return strBuilder.ToString().Trim(',');
        }

        private DataTable GetEntityData(string key)
        {
            var entity = scheme.RootPackage.FindEntityByName(key);
            var tblBook = new DataTable();

            using (var du = entity.GetDataUpdater())
            {
                du.Fill(ref tblBook);
            }

            return tblBook;
        }

        private DataTable GetRegionByType(string codes)
        {
            var entity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.d_Regions_Analysis);
            var tblBook = new DataTable();

            using (var du = entity.GetDataUpdater(String.Format("RefTerr in ({0})", codes), null))
            {
                du.Fill(ref tblBook);
            }

            return SortDataSet(tblBook, "name asc");
        }

        private DataTable GetRegionByType(string regionCodes, int regionType)
        {
            var entity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.d_Regions_Analysis);
            var tblBook = new DataTable();

            using (var du = entity.GetDataUpdater(
                String.Format("ID in ({0}) and RefTerr in ({1})", regionCodes, regionType), null))
            {
                du.Fill(ref tblBook);
            }

            return SortDataSet(tblBook, "name asc");
        }

        private string GetSettlesHierarchyKeys(string regionCodes)
        {
            string settleKeys;
            var entity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.d_Regions_Analysis);

            using (var du = entity.GetDataUpdater())
            {
                var tblBook = new DataTable();
                du.Fill(ref tblBook);

                settleKeys = GetFullSettlePath(tblBook, regionCodes);
            }

            return settleKeys;
        }

        private string GetSettlesKeys(string regionCodes)
        {
            var settleKeys = String.Empty;
            var entity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.d_Regions_Analysis);

            using (var du = entity.GetDataUpdater(
                String.Format("ParentID in ({0}) and RefTerr in (5, 6)", regionCodes), null))
            {
                var tblBook = new DataTable();
                du.Fill(ref tblBook);

                foreach (DataRow dr in tblBook.Rows)
                {
                    settleKeys = string.Format("{0},{1}", settleKeys, dr["id"]);
                }
            }

            return settleKeys.Trim(',');
        }
    }
}
