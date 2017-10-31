using System;
using System.Collections.Generic;
using System.Linq;

using Krista.FM.Common.Consolidation.Data;
using Krista.FM.Domain.Reporitory.NHibernate;

using NHibernate;
using NHibernate.Criterion;

namespace Krista.FM.RIA.Extensions.Consolidation.Services.Forms.DAL
{
    public class ReportDataRepository : IReportDataRepository
    {
        protected virtual ISession Session
        {
            get { return NHibernateSession.Current; }
        }

        public ReportDataRecord Create(Type recType)
        {
            return new ReportDataRecord(recType.Assembly.CreateInstance(recType.FullName));
        }

        public IList<IRecord> FindAll(Type recType)
        {
            return FindAll(recType, null);
        }

        public IList<IRecord> FindAll(Type recType, ICriterion criterion)
        {
            var criteria = Session.CreateCriteria(recType);
            if (criterion != null)
            {
                criteria.Add(criterion);
            }

            return (from object item in criteria.List() select new ReportDataRecord(item)).Cast<IRecord>().ToList();
        }

        public void Save(IRecord record)
        {
            Session.SaveOrUpdate(record.Value);
        }

        public void Delete(IRecord record)
        {
            Session.Delete(record.Value);
        }

        public void CommitChanges()
        {
            Session.Flush();
        }
    }
}
