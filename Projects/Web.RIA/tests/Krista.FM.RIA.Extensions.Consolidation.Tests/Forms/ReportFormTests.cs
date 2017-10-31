using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.RIA.Extensions.Consolidation.Forms.ConsForm;
using Krista.FM.RIA.Extensions.Consolidation.Services.Forms.DAL;
using Krista.FM.RIA.Extensions.Consolidation.Tests.Helpers;
using NUnit.Framework;

namespace Krista.FM.RIA.Extensions.Consolidation.Tests.Forms
{
    [TestFixture]
    public class ReportFormTests
    {
        [Ignore]
        [Test]
        public void DbTest()
        {
            NHibernateHelper.SetupNHibernate("Password=dv;Persist Security Info=True;User ID=DV;Data Source=DV");

            var reportForm = new ReportForm(new NHibernateLinqRepository<D_CD_Report>(), new ReportDataRepository(), null);

            var taskRepository = new NHibernateLinqRepository<D_CD_Task>();
            reportForm.CreateReport(taskRepository.Get(1));
        }
    }
}
