using System;
using System.Collections.Generic;

using Krista.FM.Common.Consolidation.Data;

using NHibernate.Criterion;

namespace Krista.FM.RIA.Extensions.Consolidation.Services.Forms.DAL
{
    public interface IReportDataRepository
    {
        ReportDataRecord Create(Type recType);

        IList<IRecord> FindAll(Type recType);

        IList<IRecord> FindAll(Type recType, ICriterion criterion);

        void Save(IRecord record);

        void Delete(IRecord record);

        void CommitChanges();
    }
}