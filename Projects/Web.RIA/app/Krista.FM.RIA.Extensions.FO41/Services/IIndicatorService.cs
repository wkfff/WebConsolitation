using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.FO41.Services
{
    public interface IIndicatorService : ILinqRepository<D_Marks_Privilege>
    {
        IQueryable GetQueryAll();

        D_Marks_Privilege GetQueryOne(int id);

        int GetIdBySymbol(string symbol);
    }
}