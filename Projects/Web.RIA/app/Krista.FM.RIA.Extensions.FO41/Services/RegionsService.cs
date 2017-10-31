using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory.NHibernate;

namespace Krista.FM.RIA.Extensions.FO41.Services
{
    public class RegionsService : NHibernateLinqRepository<B_Regions_Bridge>, IRegionsService
    {
        public IQueryable GetQueryAll()
        {
            return from r in FindAll()
                   select new
                              {
                                  r.ID,
                                  r.Name,
                                  RefZone = r.RefClimZone.ID,
                                  RefTerr  = r.RefTerrType.ID
                              };
        }

        public object GetQueryOne(int id)
        {
            var r = FindOne(id);
            return new
                       {
                           r.ID,
                           r.Name,
                           RefZone = r.RefClimZone.ID,
                           RefTerr = r.RefTerrType.ID
                       };
        }
    }
}
