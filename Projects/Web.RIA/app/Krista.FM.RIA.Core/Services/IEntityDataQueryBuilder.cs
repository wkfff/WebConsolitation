using System.Text;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Core
{
    public interface IEntityDataQueryBuilder
    {
        /// <summary>
        /// Строит sql-запрос для сущности. 
        /// </summary>
        StringBuilder BuildQuery(IEntity entity, string concatenateChar);
    }
}
