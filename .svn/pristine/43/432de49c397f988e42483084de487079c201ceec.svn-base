using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory.NHibernate;

namespace Krista.FM.RIA.Extensions.FO41.Services
{
    public class IndicatorService : NHibernateLinqRepository<D_Marks_Privilege>, IIndicatorService
    {
        public IQueryable GetQueryAll()
        {
            return from r in FindAll()
                   where r.RowType == 0
                   select new
                   {
                       r.ID,
                       r.Name,
                       r.NumberString,
                      r.RowType
                   };
        }

        public D_Marks_Privilege GetQueryOne(int id)
        {
            var r = FindOne(id);
            return r;
        }

        public int GetIdBySymbol(string symbol)
        {
            var r = FindAll()
                .Where(i => i.RowType == 0 && i.Symbol.ToUpper().Equals(symbol.ToUpper()));
            return r.Count() > 0 ? r.First().ID : -1;
        }
    }
}
