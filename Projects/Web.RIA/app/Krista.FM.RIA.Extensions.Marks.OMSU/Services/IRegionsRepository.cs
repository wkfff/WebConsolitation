using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.MarksOMSU.Services
{
    /// <summary>
    /// Обеспечивает работу с таблицей Районы.Анализ в контексте выбранного года(источника)
    /// </summary>
    public interface IRegionsRepository : ILinqRepository<D_Regions_Analysis>
    {
        int GetDatasourceYear();
    }
}