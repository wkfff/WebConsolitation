using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.Server
{
    public class TaxpayersSumService
    {
        private IScheme Scheme
        {
            get; set;
        }

        public TaxpayersSumService(IScheme scheme)
        {
            Scheme = scheme;
        }

        public DataTable GetResultsSum(long refRegion, int yearDayUNV, long sourceId)
        {
            using (IDatabase db = Scheme.SchemeDWH.DB)
            {
                string query =
                    "select sum(SumPayment) as SumPayment, sum(SumReduction) as SumReduction from f_Marks_TaxBenPay where RefOrg = -1 and RefRegions = ? and RefBdgLevels = 0 and RefYearDayUNV = ? and SourceID = ? and PumpID = -1";
                var dtResultData = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable,
                    new DbParameterDescriptor("p0", refRegion),
                    new DbParameterDescriptor("p1", yearDayUNV),
                    new DbParameterDescriptor("p2", sourceId));
                query =
                    "select sum(SumPayment) as SumPayment, sum(SumReduction) as SumReduction from f_Marks_TaxBenPay where RefOrg = -1 and RefRegions = ? and RefBdgLevels = 1 and RefYearDayUNV = ? and SourceID = ? and PumpID = -1";
                var dtFederalData = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable,
                    new DbParameterDescriptor("p0", refRegion), 
                    new DbParameterDescriptor("p1", yearDayUNV),
                    new DbParameterDescriptor("p2", sourceId));
                dtResultData.Merge(dtFederalData);
                query =
                    "select sum(SumPayment) as SumPayment, sum(SumReduction) as SumReduction from f_Marks_TaxBenPay where RefOrg = -1 and RefRegions = ? and RefBdgLevels = 2 and RefYearDayUNV = ? and SourceID = ? and PumpID = -1";
                dtResultData.Merge((DataTable)db.ExecQuery(query, QueryResultTypes.DataTable,
                    new DbParameterDescriptor("p0", refRegion), 
                    new DbParameterDescriptor("p1", yearDayUNV),
                    new DbParameterDescriptor("p2", sourceId)));
                return dtResultData;
            }
        }

        public DataTable GetTaxpayerSum(long refRegion, long refOrg, int yearDayUNV, long sourceId)
        {
            using (IDatabase db = Scheme.SchemeDWH.DB)
            {
                string query =
                    "select sum(SumPayment) as SumPayment, sum(SumReduction) as SumReduction from f_Marks_TaxBenPay where RefRegions = ? and RefBdgLevels = 0 and RefOrg = ? and RefYearDayUNV = ? and SourceID = ? and PumpID = -1";
                var dtResultData = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable,
                    new DbParameterDescriptor("p0", refRegion),
                    new DbParameterDescriptor("p1", refOrg),
                    new DbParameterDescriptor("p2", yearDayUNV),
                    new DbParameterDescriptor("p3", sourceId));
                query =
                    "select sum(SumPayment) as SumPayment, sum(SumReduction) as SumReduction from f_Marks_TaxBenPay where RefRegions = ? and RefBdgLevels = 1 and RefOrg = ? and RefYearDayUNV = ? and SourceID = ? and PumpID = -1";
                dtResultData.Merge((DataTable)db.ExecQuery(query, QueryResultTypes.DataTable,
                    new DbParameterDescriptor("p0", refRegion), 
                    new DbParameterDescriptor("p1", refOrg),
                    new DbParameterDescriptor("p2", yearDayUNV),
                    new DbParameterDescriptor("p3", sourceId)));
                query =
                    "select sum(SumPayment) as SumPayment, sum(SumReduction) as SumReduction from f_Marks_TaxBenPay where RefRegions = ? and RefBdgLevels = 2 and RefOrg = ? and RefYearDayUNV = ? and SourceID = ? and PumpID = -1";
                dtResultData.Merge((DataTable)db.ExecQuery(query, QueryResultTypes.DataTable,
                    new DbParameterDescriptor("p0", refRegion), 
                    new DbParameterDescriptor("p1", refOrg),
                    new DbParameterDescriptor("p2", yearDayUNV),
                    new DbParameterDescriptor("p3", sourceId)));
                return dtResultData;
            }
        }

        public int GetSourceId(int year)
        {
            using (IDatabase db = Scheme.SchemeDWH.DB)
            {
                int dataSourceID = GetDataSourceID(db, "ФО", 29, 29, year);
                DataTable dt = (DataTable)db.ExecQuery("select ID from DataSources where SupplierCode = 'ФО' and DataCode = 29 and Year = ? and deleted = 0",
                    QueryResultTypes.DataTable,
                    new DbParameterDescriptor("ID", year));
                if (dt.Rows.Count == 0)
                {
                    IDataSource ds = Scheme.DataSourceManager.DataSources.CreateElement();
                    ds.SupplierCode = "ФО";
                    ds.DataCode = "0029";
                    ds.DataName = "Проект бюджета";
                    ds.Year = year;
                    ds.ParametersType = ParamKindTypes.Year;
                    dataSourceID = ds.Save();
                }
                return dataSourceID;
            }
        }

        internal int GetDataSourceID(IDatabase db, string supplier, int dataCodeMain, int dataCodeSecond, int year)
        {
            object sourceID = db.ExecQuery(
                "select ID from DataSources where SupplierCode = ? and DataCode = ? and Year = ? and deleted = 0",
                QueryResultTypes.Scalar,
                 new DbParameterDescriptor("SupplierCode", supplier),
                 new DbParameterDescriptor("DataCode", dataCodeMain),
                 new DbParameterDescriptor("Year", year));

            if (sourceID == null || sourceID == DBNull.Value)
            {
                sourceID = db.ExecQuery(
                    "select ID from DataSources where SupplierCode = ? and DataCode = ? and Year = ? and deleted = 0",
                    QueryResultTypes.Scalar,
                    new DbParameterDescriptor("SupplierCode", supplier),
                    new DbParameterDescriptor("DataCode", dataCodeSecond),
                    new DbParameterDescriptor("Year", year));
            }

            if (sourceID == null || sourceID == DBNull.Value)
            {
                IDataSource ds = Scheme.DataSourceManager.DataSources.CreateElement();
                ds.SupplierCode = supplier;
                ds.DataCode = dataCodeMain.ToString();
                ds.DataName = "Проект бюджета";
                ds.Year = year;
                ds.ParametersType = ParamKindTypes.Year;
                return ds.Save();
            }
            return Convert.ToInt32(sourceID);
        }

        public void SaveData(DataTable taxpayersResult, DataTable taxpayerData, object sourceId, object refRegion, object refYearDayUNV)
        {
            // сохраняем итоговые данные
            var resultChanges = taxpayersResult.GetChanges();
            if (resultChanges != null)
            {
                SaveResultData(resultChanges, sourceId, refRegion, refYearDayUNV);
            }

            // сохраняем данные по отдельным налогоплательщикам
            var changes = taxpayerData.GetChanges();
            if (changes != null)
            {
                foreach (DataRow row in changes.Rows)
                {
                    int refMarks = (row.IsNull("SumPayment") || Convert.ToDecimal(row["SumPayment"]) == 0) ?  3 :  2;
                    int delRefMarks = (row.IsNull("SumPayment") || Convert.ToDecimal(row["SumPayment"]) == 0) ? 2 : 3;
                    object value = (row.IsNull("SumPayment") || Convert.ToDecimal(row["SumPayment"]) == 0)
                                       ? row["SumReduction"] : row["SumPayment"];

                    DeleteData(row["RefOrg"], refRegion, delRefMarks, row["IndicatorCode"], sourceId, refYearDayUNV);
                    DeleteData(row["RefOrg"], refRegion, refMarks, row["IndicatorCode"], sourceId, refYearDayUNV);
                    // изменяем данные
                    InsertNewData(row["RefOrg"], refRegion, refMarks, row["IndicatorCode"], sourceId, refYearDayUNV, value);
                }
            }
        }

        public void SaveResultData(DataTable resultTable, object sourceId, object refRegion, object refYearDayUNV)
        {
            using (var db = Scheme.SchemeDWH.DB)
            {
                // записываем новые
                string query =
                    @"insert into f_Marks_TaxBenPay (PumpID, SourceID, TaskID, SumPayment, SumReduction, RefBdgLevels, RefMarks, RefRegions, RefOrg, RefYearDayUNV)
                    values (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
                foreach (DataRow row in resultTable.Rows)
                {
                    // удаляем предыдущие данные
                    db.ExecQuery(
                        "delete from f_Marks_TaxBenPay where RefOrg = -1 and RefRegions = ? and SourceID = ? and RefMarks = 1 and RefYearDayUNV = ? and RefBdgLevels = ? and PumpID = -1",
                        QueryResultTypes.NonQuery,
                        new DbParameterDescriptor("p0", refRegion),
                        new DbParameterDescriptor("p1", sourceId),
                        new DbParameterDescriptor("p2", refYearDayUNV),
                        new DbParameterDescriptor("p3", row["IndicatorCode"]));

                    decimal sumPayment = row.IsNull("SumPayment") ? 0 : Convert.ToDecimal(row["SumPayment"]) * 1000;
                    decimal sumReduction = row.IsNull("SumReduction") ? 0 : Convert.ToDecimal(row["SumReduction"]) * 1000;
                    if (sumPayment == 0 && sumReduction == 0)
                        continue;

                    db.ExecQuery(query, QueryResultTypes.NonQuery,
                                 new DbParameterDescriptor("p0", -1),
                                 new DbParameterDescriptor("p1", sourceId),
                                 new DbParameterDescriptor("p2", -1),
                                 new DbParameterDescriptor("p3", sumPayment),
                                 new DbParameterDescriptor("p4", sumReduction),
                                 new DbParameterDescriptor("p5", row["IndicatorCode"]),
                                 new DbParameterDescriptor("p6", 1),
                                 new DbParameterDescriptor("p7", refRegion),
                                 new DbParameterDescriptor("p8", -1),
                                 new DbParameterDescriptor("p9", refYearDayUNV));
                }
            }
        }

        public void DeleteOrgData(object refOrg, object refRegions, object sourceId, object refYearDayUNV)
        {
            using (var db = Scheme.SchemeDWH.DB)
            {
                db.ExecQuery("delete from f_Marks_TaxBenPay where RefOrg = ? and RefRegions = ? and SourceID = ? and RefYearDayUNV = ? and PumpID = -1",
                             QueryResultTypes.NonQuery,
                             new DbParameterDescriptor("p0", refOrg),
                             new DbParameterDescriptor("p1", refRegions),
                             new DbParameterDescriptor("p2", sourceId),
                             new DbParameterDescriptor("p3", refYearDayUNV));
            }
        }

        private void DeleteData(object refOrg, object refRegions, object refMarks, object refBudlevel, object sourceId, object refYearDayUNV)
        {
            using (IDatabase db = Scheme.SchemeDWH.DB)
            {
                string query = "delete from f_Marks_TaxBenPay where RefOrg = ? and RefRegions = ? and RefMarks = ? and RefBdgLevels = ? and SourceID = ? and RefYearDayUNV = ? and PumpID = -1";
                db.ExecQuery(query, QueryResultTypes.NonQuery,
                             new DbParameterDescriptor("p0", refOrg),
                             new DbParameterDescriptor("p1", refRegions),
                             new DbParameterDescriptor("p2", refMarks),
                             new DbParameterDescriptor("p3", refBudlevel),
                             new DbParameterDescriptor("p4", sourceId),
                             new DbParameterDescriptor("p5", refYearDayUNV));
            }
        }

        private void InsertNewData(object refOrg, object refRegions, object refMarks, object refBudlevel, object sourceId, object refYearDayUNV, object dataValue)
        {
            if (dataValue == null || dataValue == DBNull.Value)
                return;
            dataValue = Convert.ToDecimal(dataValue)*1000;

            string query =
                @"insert into f_Marks_TaxBenPay (PumpID, SourceID, TaskID, SumPayment, SumReduction, RefBdgLevels, RefMarks, RefRegions, RefOrg, RefYearDayUNV)
                values (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
            using (IDatabase db = Scheme.SchemeDWH.DB)
            {
                object sumPayment = Convert.ToInt64(refMarks) == 2 ? dataValue : DBNull.Value;
                object sumReduction = Convert.ToInt64(refMarks) == 3 ? dataValue : DBNull.Value;
                db.ExecQuery(query, QueryResultTypes.NonQuery,
                             new DbParameterDescriptor("p0", -1),
                             new DbParameterDescriptor("p1", sourceId),
                             new DbParameterDescriptor("p2", -1),
                             new DbParameterDescriptor("p3", sumPayment),
                             new DbParameterDescriptor("p4", sumReduction),
                             new DbParameterDescriptor("p5", refBudlevel),
                             new DbParameterDescriptor("p6", refMarks),
                             new DbParameterDescriptor("p7", refRegions),
                             new DbParameterDescriptor("p8", refOrg),
                             new DbParameterDescriptor("p9", refYearDayUNV));
            }
        }
    }
}
