using System.Collections.Generic;
using System.Data;
using Ext.Net.MVC;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Core
{
    public interface IEntityDataService
    {
        AjaxStoreResult GetData(IEntity entity, int start, int limit, string dir, string sort, string filter, IDbDataParameter[] prms);

        AjaxStoreResult GetData(IEntity entity, int start, int limit, string dir, string sort, string filter, IDbDataParameter[] prms, IEntityDataQueryBuilder queryBuilder);

        List<long> Create(IEntity entity, List<Dictionary<string, object>> table, Dictionary<long, long> idHierarchy);

        void Update(IEntity entity, List<Dictionary<string, object>> table);

        void Delete(IEntity entity, List<Dictionary<string, object>> table);

        void DeleteDependentData(IEntity entity, int rowId);
    }
}
