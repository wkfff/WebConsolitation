using System;
using Microsoft.Practices.ServiceLocation;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Domain.Services.FinSourceDebtorBook
{
    public class DataSourceService
    {
        private readonly IScheme scheme;

        public DataSourceService()
            : this(ServiceLocator.Current.GetInstance<IScheme>())
        { }

        public DataSourceService(IScheme scheme)
        {
            this.scheme = scheme;
        }

        /// <summary>
        /// Возвращает источник данных по долговой книге исходя из указанного года
        /// Создание источника по выбору
        /// </summary>
        public int GetDataSource(int year, bool createDataSource)
        {
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                const string supplier = "ФО";
                const int dataCodeMain = 30;
                const int dataCodeSecond = 30;
                const string dataName = "Долговые обязательства";

                object sourceId = db.ExecQuery(
                    "select ID from DataSources where SupplierCode = ? and DataCode = ? and Year = ? and deleted = 0",
                    QueryResultTypes.Scalar,
                    db.CreateParameter("SupplierCode", supplier),
                    db.CreateParameter("DataCode", dataCodeMain),
                    db.CreateParameter("Year", year));

                if (sourceId == null)
                {
                    sourceId = db.ExecQuery(
                        "select ID from DataSources where SupplierCode = ? and DataCode = ? and Year = ? and deleted = 0",
                        QueryResultTypes.Scalar,
                        db.CreateParameter("SupplierCode", supplier),
                        db.CreateParameter("DataCode", dataCodeSecond),
                        db.CreateParameter("Year", year));
                }

                if (!createDataSource)
                {
                    if (sourceId == null)
                        return -1;
                    return Convert.ToInt32(sourceId);
                }

                if (sourceId == null)
                {
                    IDataSource ds = scheme.DataSourceManager.DataSources.
                            CreateElement();
                    ds.SupplierCode = supplier;
                    ds.DataCode = dataCodeMain.ToString();
                    ds.DataName = dataName;
                    ds.Year = year;
                    ds.ParametersType = ParamKindTypes.Year;
                    return ds.Save();
                }
                return Convert.ToInt32(sourceId);
            }
        }

        public int GetDataSource(int year, string supplier, int dataCode, string dataName, bool createDataSource)
        {
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                object sourceId = db.ExecQuery(
                    "select ID from DataSources where SupplierCode = ? and DataCode = ? and Year = ? and deleted = 0",
                    QueryResultTypes.Scalar,
                    db.CreateParameter("SupplierCode", supplier),
                    db.CreateParameter("DataCode", dataCode),
                    db.CreateParameter("Year", year));

                if (!createDataSource)
                {
                    if (sourceId == null)
                        return -1;
                    return Convert.ToInt32(sourceId);
                }

                if (sourceId == null)
                {
                    IDataSource ds = scheme.DataSourceManager.DataSources.CreateElement();
                    ds.SupplierCode = supplier;
                    ds.DataCode = dataCode.ToString();
                    ds.DataName = dataName;
                    ds.Year = year;
                    ds.ParametersType = ParamKindTypes.Year;
                    return ds.Save();
                }
                return Convert.ToInt32(sourceId);
            }
        }

        /// <summary>
        /// Возвращает источник данных по долговой книге исходя из указанного года
        /// Если источник не найден, создается новый
        /// </summary>
        public int GetDataSource(int year)
        {
            return GetDataSource(year, true);
        }
    }
}
