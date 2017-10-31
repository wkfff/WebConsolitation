using System;
using System.Data;
using System.Reflection;
using System.Runtime.Hosting;
using System.Runtime.Remoting.Messaging;
using System.Security.Principal;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Logger
{
    public class DataOperations : DisposableObject, IDataOperations
    {
        // часть основного запроса
        private const string auditQuery = "select ID, CHANGETIME, KINDOFOPERATION, OBJECTNAME, OBJECTTYPE, USERNAME, SESSIONID, RECORDID, TASKID, PUMPID from DVAudit.DataOperations";
        // часть запроса количества записей
        private const string countQuery = "select Count(ID) from DVAudit.DataOperations";

        private const string orderByFilter = " order by ID DESC";

        private IScheme _scheme = null;

        private static int maxRowCount = 5000;

        public DataOperations(IScheme scheme)
        {
            if (scheme == null)
                throw new Exception("Не задан интерфейс схемы");
            _scheme = scheme;
        }

        public void GetAuditData(ref DataTable auditData, string filter, params IDbDataParameter[] parameters)
        {
            IDatabase db = this._scheme.SchemeDWH.DB;
            try
            {
                auditData = (DataTable)db.ExecQuery(auditQuery + filter + orderByFilter, QueryResultTypes.DataTable, maxRowCount, parameters);
            }
            finally
            {
                db.Dispose();
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
