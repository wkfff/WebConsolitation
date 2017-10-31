using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.Region10MarksOIV.Services
{
    /// <summary>
    /// Обеспечивает работу(ReadOnly) с таблицей Территории.РФ в контексте выбранного года(источника)
    /// </summary>
    public interface ITerritoryRepository : ILinqRepository<D_Territory_RF>
    {
    }
}