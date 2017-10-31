using System;
using System.Data;
using Microsoft.Practices.ServiceLocation;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Domain.Services.FinSourceDebtorBook
{
    public class RegionsService
    {
        private readonly IScheme scheme;

        public RegionsService()
            : this(ServiceLocator.Current.GetInstance<IScheme>())
        {
        }

        public RegionsService(IScheme scheme)
        {
            this.scheme = scheme;
        }

        /// <summary>
        /// получаем тип региона, к которому привязан пользователь
        /// </summary>
        public UserRegionType GetUserRegionType(int regionId)
        {
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                string regionQuery = String.Format("select RefTerr from d_Regions_Analysis where id = {0}", regionId);
                DataTable dtRegion = (DataTable)db.ExecQuery(regionQuery, QueryResultTypes.DataTable);
                if (dtRegion.Rows.Count != 0)
                {
                    int regionType = Convert.ToInt32(dtRegion.Rows[0][0]);
                    switch (regionType)
                    {
                        case 3:
                            return UserRegionType.Subject;
                        case 4:
                            return UserRegionType.Region;
                        case 7:
                            return UserRegionType.Town;
                        case 5:
                        case 6:
                        case 11:
                            return UserRegionType.Settlement;
                    }
                }
                return UserRegionType.Unknown;
            }
        }

        /// <summary>
        /// Возвращает Id субъекта по указанному источнику данных из классификатора "Районы.Анализ".
        /// </summary>
        public int GetSubjectRegionId(int sourceId)
        {
            IEntity entity = scheme.RootPackage.FindEntityByName(DomainObjectsKeys.d_Regions_Analysis);
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                object queryResult = db.ExecQuery(string.Format("select ID from {0} where RefTerr = 3 and SourceID = {1} and ParentId is null",
                    entity.FullDBName, sourceId), QueryResultTypes.Scalar);
                if (queryResult is DBNull)
                    return -1;
                return Convert.ToInt32(queryResult);
            }
        }
    }
}
