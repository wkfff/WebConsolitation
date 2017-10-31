using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.Region10MarksOIV.Services
{
    /// <summary>
    /// Обеспечивает работу с таблицей Показатели_ОИВ в контексте выбранного года(источника)
    /// </summary>
    public interface IMarksRepository : ILinqRepository<D_OIV_Mark>
    {
        int GetDatasourceYear();
    }
}