using System;
using System.Data;
using System.Reflection;
using System.Text;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Domain.Reporitory
{
    public class SystemDataService : IDomainDataService
    {
        private readonly IDatabase db;

        public SystemDataService(IDatabase db)
        {
            this.db = db;
        }

        public DataRow[] GetObjectData(Type objectType, string selectFilter)
        {
            StringBuilder query = new StringBuilder("select ");

            foreach (PropertyInfo propertyInfo in objectType.GetProperties())
            {
                query.Append(propertyInfo.Name).Append(",");
            }
            query.Remove(query.Length - 1, 1)
                .Append(" FROM ")
                .Append(objectType.Name);
            if (!String.IsNullOrEmpty(selectFilter))
            {
                query.Append(" WHERE ").Append(selectFilter);
            }

            DataTable dataTable = (DataTable)db.ExecQuery(query.ToString(),
                QueryResultTypes.DataTable);
            return dataTable.Select();
        }

        public void Create(DomainObject obj)
        {
            throw new NotImplementedException();
        }

        public void Update(DomainObject obj)
        {
            throw new NotImplementedException();
        }
    }
}
