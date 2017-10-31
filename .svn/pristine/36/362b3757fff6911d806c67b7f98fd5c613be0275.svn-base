using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Practices.ServiceLocation;

using Krista.FM.ServerLibrary;
using Krista.FM.Common;

namespace Krista.FM.Domain.Services.FinSourceDebtorBook
{
    public class  RegionsAccordanceService
    {
        readonly IScheme scheme;
        private readonly DataSourceService dataSourceService;

        public RegionsAccordanceService()
            : this(ServiceLocator.Current.GetInstance<IScheme>(), new DataSourceService())
        {
        }

        public RegionsAccordanceService(IScheme scheme, DataSourceService dataSourceService)
        {
            this.scheme = scheme;
            this.dataSourceService = dataSourceService;
        }

        /// <summary>
        /// заполнение данными таблицы соответствия районов по указанному году
        /// </summary>
        public void FillData(int currentUserYear)
        {
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                DataTable dt = GetRegionData(currentUserYear, db);
                int sourceID = dataSourceService.GetDataSource(currentUserYear);
                IEntity entity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.d_S_RegionMatchOperation);
                using (IDataUpdater du = entity.GetDataUpdater(string.Format("SourceID = {0}", sourceID), null))
                {

                    DataTable dtRegionAccordance = new DataTable();
                    du.Fill(ref dtRegionAccordance);
                    if (dtRegionAccordance.Rows.Count == 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            DataRow newRow = dtRegionAccordance.NewRow();
                            newRow["ID"] = entity.GetGeneratorNextValue;
                            newRow["RowType"] = 0;
                            newRow["SourceID"] = sourceID;
                            newRow["Code"] = row[0];
                            newRow["RefRegionOld"] = row[1];
                            newRow["RefRegionNew"] = row[2];
                            dtRegionAccordance.Rows.Add(newRow);
                        }
                        du.Update(ref dtRegionAccordance);
                    }
                }
            }
        }

        /// <summary>
        /// Получение района, соответсвующего указанному району текущего года
        /// </summary>
        public List<int> GetRegionsByYear(int year, int currentRegion, int currentUserYear)
        {
            List<int> regions = new List<int>();
            if (year == currentUserYear)
            {
                regions.Add(currentRegion);
                return regions;
            }

            regions.Add(currentRegion);
            List<int> sourceIDList = new List<int>();
            IEntity entity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.d_S_RegionMatchOperation);
            string regionQuery = year < currentUserYear
                                        ?
                                            string.Format("select RefRegionOld from {0} ", entity.FullDBName) +
                                            "where SourceID = {0} and RefRegionNew = {1}"
                                        :
                                            string.Format("select RefRegionNew from {0} ", entity.FullDBName) +
                                            "where SourceID = {0} and RefRegionOld = {1}";
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                for (int i = 1; i <= Math.Abs(currentUserYear - year); i++)
                {
                    if (currentUserYear > year)
                        sourceIDList.Add(dataSourceService.GetDataSource(currentUserYear - (i - 1)));
                    else
                        sourceIDList.Add(dataSourceService.GetDataSource(currentUserYear + i));
                }

                foreach (int sourceID in sourceIDList)
                {
                    regions = GetRegions(sourceID, regions, regionQuery, db);
                }
            }
            if (regions.Count == 0)
                regions.Add(currentRegion);
            return regions;
        }

        /// <summary>
        /// копирование должностей и фамилий с года на год для нового района
        /// </summary>
        /// <param name="currentUserRegion"></param>
        /// <param name="newYear"></param>
        /// <param name="currentYear"></param>
        public void SetRegionTitles(int currentUserRegion, int currentUserYear, int newYear)
        {
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                // получаем район пользователя, соответствующего новому году
                List<int> newRegions = GetRegionsByYear(newYear, currentUserRegion, currentUserYear);
                if (newRegions.Count > 0)
                {
                    int newRegion = 0;
                    int oldRegion = 0;

                    if (newYear > currentUserYear)
                    {
                        newRegion = newRegions[0];
                        oldRegion = currentUserRegion;
                    }
                    else
                    {
                        newRegion = currentUserRegion;
                        oldRegion = newRegions[0];
                    }

                    object newRegionTitlesCount =
                        db.ExecQuery("select count(id) from d_S_TitleReport where RefRegion = ?",
                                     QueryResultTypes.Scalar, new DbParameterDescriptor("p0", newRegion));
                    // если для текущего района есть записи в должностях, выходим
                    if (Convert.ToInt32(newRegionTitlesCount) > 0)
                        return;
                    // иначе заполняем исходя из существующих должностей района годом раньше
                    DataTable dtRegionTitles = (DataTable)db.ExecQuery("select TitleManager, LastName, TitleAccountant, LastAccountant from d_S_TitleReport where RefRegion = ?",
                        QueryResultTypes.DataTable, new DbParameterDescriptor("p0", oldRegion));
                    if (dtRegionTitles.Rows.Count > 0)
                    {
                        foreach (DataRow row in dtRegionTitles.Rows)
                        {
                            db.ExecQuery(
                                @"insert into d_S_TitleReport (RowType, TitleManager, LastName, TitleAccountant, LastAccountant, RefRegion) 
                                values (?, ?, ?, ?, ?, ?)",
                                QueryResultTypes.NonQuery,
                                new DbParameterDescriptor("p0", 0),
                                new DbParameterDescriptor("p1", row["TitleManager"]),
                                new DbParameterDescriptor("p2", row["LastName"]),
                                new DbParameterDescriptor("p3", row["TitleAccountant"]),
                                new DbParameterDescriptor("p4", row["LastAccountant"]),
                                new DbParameterDescriptor("p5", newRegion));

                        }
                    }
                }
            }
        }

        private static List<int> GetRegions(int sourceID, List<int> currentRegions, string query, IDatabase db)
        {
            List<int> regions = new List<int>();
            foreach (int region in currentRegions)
            {
                DataTable dtRegions = (DataTable)db.ExecQuery(string.Format(query, sourceID, region), QueryResultTypes.DataTable);
                foreach (DataRow row in dtRegions.Rows)
                {
                    // если нашли пустое значение, соответствующее району, ничего не делаем, 
                    if (!row.IsNull(0))
                        regions.Add(Convert.ToInt32(row[0]));
                }
            }
            return regions;
        }

        public int GetRegionYear(int region, IDatabase db)
        {
            const string sourceIDQuery = "select SourceID from d_Regions_Analysis where ID = {0}";
            object queryResult = db.ExecQuery(string.Format(sourceIDQuery, region), QueryResultTypes.Scalar);
            return scheme.DataSourceManager.DataSources[Convert.ToInt32(queryResult)].Year;
        }

        public int GetRegionYear(int region)
        {
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                return GetRegionYear(region, db);
            }
        }

        /// <summary>
        /// данные классификатора Районы.Анализ по двум источникам, текущего года и предыдущего
        /// </summary>
        private DataTable GetRegionData(int year, IDatabase db)
        {
            int currrentSourceID = dataSourceService.GetDataSource(year, "ФО", 6, "Анализ данных", false);
            int prevSourceID = dataSourceService.GetDataSource(year - 1, "ФО", 6, "Анализ данных", false);

            const string regionsQuery =
                "select region1.Code, region1.ID, region2.ID from d_Regions_Analysis region1 " +
                "left outer join d_Regions_Analysis region2 on region1.Name = region2.Name and region1.Code = region2.Code " +
                "and region2.SourceID = {1} where region1.SourceID = {0}";
            return (DataTable)db.ExecQuery(string.Format(regionsQuery, prevSourceID, currrentSourceID), QueryResultTypes.DataTable);
        }
    }
}
