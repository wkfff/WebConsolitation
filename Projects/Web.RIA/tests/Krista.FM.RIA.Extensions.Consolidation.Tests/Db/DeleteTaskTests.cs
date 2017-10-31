using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Krista.FM.Common;
using Krista.FM.Common.Consolidation.Forms;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.Extensions;
using Krista.FM.RIA.Extensions.Consolidation.Forms.ConsForm;
using Krista.FM.RIA.Extensions.Consolidation.Services.Forms.ScriptingEngine;
using Krista.FM.RIA.Extensions.Consolidation.Services.Forms.ScriptingEngine.Oracle;
using Krista.FM.RIA.Extensions.Consolidation.Tests.Helpers;
using Krista.FM.ServerLibrary;
using NUnit.Framework;

namespace Krista.FM.RIA.Extensions.Consolidation.Tests.Db
{
    [TestFixture]
    public class DeleteTaskTests
    {
        private IScheme scheme;

        [SetUp]
        public void SetUp()
        {
            Console.WriteLine("Подключение к базе...");
            NHibernateHelper.SetupNHibernate("Password=dv;Persist Security Info=True;User ID=DV;Data Source=DV");
            Console.WriteLine("Подключение к схеме...");
            scheme = ServerSchemeConnectionHelper.Connect("fmserv:8008", "krista2\\gbelov");
        }

        [Ignore]
        [Test]
        public void DeleteTaskTest()
        {
            List<int> tasks = new List<int> { };

            var taskRepository = new NHibernateLinqRepository<D_CD_Task>();
            var reportRepository = new NHibernateLinqRepository<D_CD_Report>();
            var sectionRepository = new NHibernateLinqRepository<D_Report_Section>();
            var rowRepository = new NHibernateLinqRepository<D_Report_Row>();
            var scriptingEngine = new FormScriptingEngine(new ScriptingEngineFactory(scheme), new DatabaseObjectHashNameResolver(scheme));

            tasks.AddRange(taskRepository.GetAll().Select(t => t.ID));

            foreach (var taskId in tasks)
            {
                Console.WriteLine("Подготовка к удалению задачи ID= " + taskId);

                var task = taskRepository.Get(taskId);
                
                Console.WriteLine("Удаление структур...");
                var scripts = scriptingEngine.Drop(task.RefTemplate);
                using (var db = scheme.SchemeDWH.DB)
                {
                    foreach (var sctipt in scripts)
                    {
                        try
                        {
                            db.ExecQuery(sctipt, QueryResultTypes.NonQuery);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("При удалении структур произошла ошибка: " + e.Message);
                        }
                    }
                }

                Console.WriteLine("Удаление данных...");
                var reports = reportRepository.FindAll().Where(x => x.RefTask == task);
                foreach (var report in reports.ToList())
                {
                    foreach (var section in report.Sections.ToList())
                    {
                        foreach (var row in section.Rows.ToList())
                        {
                            rowRepository.Delete(row);
                        }

                        section.Rows.Clear();
                        sectionRepository.Delete(section);
                    }

                    report.Sections.Clear();
                    reportRepository.Delete(report);
                }

                taskRepository.DbContext.CommitChanges();

                taskRepository.Delete(task);
                taskRepository.DbContext.CommitChanges();
            }
        }

        [Ignore]
        [Test]
        public void DeleteTaskWithoutTablesTest()
        {
            List<int> tasks = new List<int> { };

            var taskRepository = new NHibernateLinqRepository<D_CD_Task>();
            var reportRepository = new NHibernateLinqRepository<D_CD_Report>();
            var sectionRepository = new NHibernateLinqRepository<D_Report_Section>();
            var rowRepository = new NHibernateLinqRepository<D_Report_Row>();

            tasks.AddRange(taskRepository.GetAll().Where(x => x.RefCollectTask != null).Select(t => t.ID));

            foreach (var taskId in tasks)
            {
                Console.WriteLine("Подготовка к удалению задачи ID= " + taskId);

                var task = taskRepository.Get(taskId);

                Console.WriteLine("Удаление данных...");
                var reports = reportRepository.FindAll().Where(x => x.RefTask == task);
                using (var db = scheme.SchemeDWH.DB)
                {
                    foreach (var report in reports.ToList())
                    {
                        DeleteReportReq(db, report, RequisiteKinds.Header);
                        foreach (var section in report.Sections.ToList())
                        {
                            DeleteSqctionReq(db, section, report, RequisiteKinds.Header);
                            DeleteTableRow(db, section, report);
                            DeleteSqctionReq(db, section, report, RequisiteKinds.Footer);
                        }

                        DeleteReportReq(db, report, RequisiteKinds.Footer);
                    }
                }

                foreach (var report in reports.ToList())
                {
                    foreach (var section in report.Sections.ToList())
                    {
                        foreach (var row in section.Rows.ToList())
                        {
                            rowRepository.Delete(row);
                        }

                        section.Rows.Clear();
                        sectionRepository.Delete(section);
                    }

                    report.Sections.Clear();
                    reportRepository.Delete(report);
                }

                taskRepository.DbContext.CommitChanges();

                taskRepository.Delete(task);
                taskRepository.DbContext.CommitChanges();
            }
        }

        private static void DeleteSqctionReq(IDatabase db, D_Report_Section section, D_CD_Report report, RequisiteKinds kind)
        {
            var tableName = ScriptingUtils.GetSectionTableName(
                report.RefForm.InternalName,
                section.RefFormSection.InternalName,
                report.RefForm.FormVersion,
                kind == RequisiteKinds.Header ? "rh" : "rf");

            try
            {
                db.ExecQuery(
                    "delete from {0} where ID = ?".FormatWith(tableName),
                    QueryResultTypes.NonQuery,
                    new DbParameterDescriptor("ID", section.ID));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static void DeleteReportReq(IDatabase db, D_CD_Report report, RequisiteKinds kind)
        {
            var tableName = ScriptingUtils.GetReportTableName(
                report.RefForm.InternalName,
                report.RefForm.FormVersion,
                kind == RequisiteKinds.Header ? "rh" : "rf");

            try
            {
                db.ExecQuery(
                    "delete from {0} where ID = ?".FormatWith(tableName),
                    QueryResultTypes.NonQuery,
                    new DbParameterDescriptor("ID", report.ID));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static void DeleteTableRow(IDatabase db, D_Report_Section section, D_CD_Report report)
        {
            var tableName = ScriptingUtils.GetSectionTableName(
                report.RefForm.InternalName, 
                section.RefFormSection.InternalName,
                report.RefForm.FormVersion, 
                "rw");

            foreach (var row in section.Rows.ToList())
            {
                try
                {
                    db.ExecQuery(
                        "delete from {0} where ID = ?".FormatWith(tableName),
                        QueryResultTypes.NonQuery,
                        new DbParameterDescriptor("ID", row.ID));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
