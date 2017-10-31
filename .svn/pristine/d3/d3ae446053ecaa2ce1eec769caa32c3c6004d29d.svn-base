using System;
using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.Extensions;
using NHibernate;
using NHibernate.Criterion;

namespace Krista.FM.RIA.Extensions.DebtBook.Services.DAL
{
    public class ObjectRepository : IObjectRepository
    {
        protected virtual ISession Session
        {
            get { return NHibernateSession.Current; }
        }

        public IList<object> FindAll(Type recType, ICriterion criterion)
        {
            var criteria = Session.CreateCriteria(recType);
            if (criterion != null)
            {
                criteria.Add(criterion);
            }

            return (from object item in criteria.List() select item).ToList();
        }

        public IList<object> GetRows(string entityFullDbName, string serverFilter, int? variantId, int? sourceId)
        {
            string domainObjectName = GetDomainObjectName(entityFullDbName);
            Type recType = Type.GetType(String.Format("Krista.FM.Domain.{0}, Krista.FM.Domain", domainObjectName));

            string query = String.Format("select * from {0}", entityFullDbName);

            string whereClause = String.Empty;
            if (serverFilter.IsNotNullOrEmpty())
            {
                whereClause = serverFilter;
            }

            if (variantId != null)
            {
                whereClause = String.Format("{0}{1}{2}", whereClause, whereClause.IsNullOrEmpty() ? "refVariant = " : " and refVariant = ", variantId);
            }
            
            if (sourceId != null)
            {
                whereClause = String.Format("{0}{1}{2}", whereClause, whereClause.IsNullOrEmpty() ? "sourceId = " : " and sourceId = ", sourceId);
            }

            if (whereClause.IsNotNullOrEmpty())
            {
                query = String.Format("{0} where {1}", query, whereClause);
            }

            var sql = Session.CreateSQLQuery(query)
                             .AddEntity(recType);

            IList<object> result = sql.List<object>();
            return result;
        }

        public object GetRow(string entityFullDbName, int id)
        {
            string domainObjectName = GetDomainObjectName(entityFullDbName);
            Type recType = Type.GetType(String.Format("Krista.FM.Domain.{0}, Krista.FM.Domain", domainObjectName));
            var res = FindAll(recType, new EqPropertyExpression("ID", new ConstantProjection(id)));
            if (res.Count == 0)
            {
                return null;
            }

            return res[0]; 
        }

        public object GetPrevious(object record)
        {
            var sourceKey = record.GetType().GetProperty("SourceKey");
            object parentRowId = sourceKey.GetValue(record, null);

            if (parentRowId == null)
            {
                return null;
            }

            var res = FindAll(record.GetType(), new EqPropertyExpression("ID", new ConstantProjection(parentRowId)));
            if (res.Count == 0)
            {
                return null;
            }

            return res[0];
        }

        private string GetDomainObjectName(string entityFullDbName)
        {
            int underlinePosition = entityFullDbName.IndexOf("_", 0, StringComparison.Ordinal);
            string result = String.Format(
                                          "{0}{1}",
                                          entityFullDbName.Substring(0, underlinePosition).ToUpper(),
                                          entityFullDbName.Substring(underlinePosition));
            return result;
        }
    }
}
