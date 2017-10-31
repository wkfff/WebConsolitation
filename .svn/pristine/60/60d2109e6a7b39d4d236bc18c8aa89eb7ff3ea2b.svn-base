using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Ext.Net.MVC;

using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

using Krista.FM.Common;
using Krista.FM.Common.Consolidation.Forms;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Core.Progress;
using Krista.FM.RIA.Extensions.Consolidation.Data;
using Krista.FM.RIA.Extensions.Consolidation.Presentation.ViewModel;
using Krista.FM.RIA.Extensions.Consolidation.Services.Forms.ExportReports.Xml;
using Krista.FM.RIA.Extensions.Consolidation.Services.Forms.ImportReports;
using Krista.FM.RIA.Extensions.Consolidation.Services.Forms.ScriptingEngine;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.Consolidation.Presentation.Controllers
{
    public class ConsCollectingTasksController : SchemeBoundController
    {
        private readonly IUserSessionState userState;
        private readonly ICollectingTaskRepository taskRepository;
        private readonly ITaskBuilderService taskBuilderService;
        private readonly IXmlExportReportService exportService;
        private readonly IImportReportService importService;
        private readonly ILinqRepository<D_CD_CollectTask> collectTaskRepository;
        private readonly ILinqRepository<D_CD_Report> reportRepository;
        private readonly IProgressManager progressManager;

        public ConsCollectingTasksController(
            IUserSessionState userState, 
            ICollectingTaskRepository taskRepository,
            ITaskBuilderService taskBuilderService,
            IXmlExportReportService exportService,
            IImportReportService importService,
            ILinqRepository<D_CD_CollectTask> collectTaskRepository,
            ILinqRepository<D_CD_Report> reportRepository,
            IProgressManager progressManager)
        {
            this.userState = userState;
            this.taskRepository = taskRepository;
            this.taskBuilderService = taskBuilderService;
            this.exportService = exportService;
            this.importService = importService;
            this.collectTaskRepository = collectTaskRepository;
            this.reportRepository = reportRepository;
            this.progressManager = progressManager;
        }

        [HttpGet]
        public ActionResult Load()
        {
            var data = from t in taskRepository.GetSubjectTasks(userState.Subjects)
                        select new CollectingTaskViewModel
                        {
                            Id = t.ID,
                            Date = t.EndPeriod,
                            ProvideDate = t.ProvideDate,
                            PeriodId = t.RefPeriod.ID,
                            AuthorId = t.RefSubject.ID,
                            PeriodName = t.RefPeriod.RefRepKind.Name + ", " + t.RefPeriod.Name,
                            AuthorName = t.RefSubject.Name
                        };

            return new AjaxStoreResult(data, data.Count());
        }

        [HttpPost]
        [Transaction(RollbackOnModelStateError = true)]
        public ActionResult Create(DateTime date, DateTime provideDate, int periodId, int authorId)
        {
            try
            {
                D_CD_CollectTask task = taskRepository.Create(date, provideDate, periodId, authorId);

                taskBuilderService.BuildCollectingTask(task);

                return new AjaxFormResult { Success = true };
            }
            catch (Exception e)
            {
                ModelState.AddModelError("error", e);
                return new AjaxFormResult { Success = false, Errors = { new FieldError("asddf", e.ExpandException()) } };
            }
        }

        public ActionResult Export(int taskId)
        {
            try
            {
                MemoryStream outputMemStream = new MemoryStream();
                ZipOutputStream zipOutStream = new ZipOutputStream(outputMemStream);

                zipOutStream.SetLevel(6);

                var reports = reportRepository.FindAll().Where(x => x.RefTask.RefCollectTask.ID == taskId && (x.RefTask.RefStatus.ID == 2 || x.RefTask.RefStatus.ID == 3)).ToList();

                using (new ServerContext())
                {
                    var total = reports.Count;
                    var current = 1.0;
                    foreach (var report in reports)
                    {
                        var stream = exportService.Export(report);
                        AddZipFile("reportdump_{0}.xml".FormatWith(report.ID), stream, zipOutStream);
                        stream.Close();

                        progressManager.SetCompleted(current++ / total);
                    }
                }

                zipOutStream.IsStreamOwner = false;
                zipOutStream.Close();

                outputMemStream.Position = 0;

                progressManager.SetCompleted(1);

                return File(outputMemStream, "application/zip", "CollectingTaskDump_{0}.zip".FormatWith(taskId));
            }
            catch (Exception e)
            {
                AjaxFormResult result = new AjaxFormResult();
                result.Success = false;
                result.Script = null;
                result.ExtraParams["msg"] = "Ошибка экспорта формы.";
                result.ExtraParams["responseText"] = String.Format("Ошибка экспорта формы: {0}", e.Message);
                result.IsUpload = true;
                return result;
            }
        }

        [HttpPost]
        [Transaction(RollbackOnModelStateError = true)]
        public ActionResult Import(int taskId)
        {
            AjaxFormResult result = new AjaxFormResult();
            try
            {
                var collectTask = collectTaskRepository.FindOne(taskId);

                HttpPostedFileBase file = Request.Files[0];

                file.InputStream.Position = 0;
                var zf = new ZipFile(file.InputStream);
                var total = zf.Count;
                var current = 1.0;
                foreach (ZipEntry zipEntry in zf)
                {
                    if (!zipEntry.IsFile)
                    {
                        // Игнорируем каталоги
                        continue;
                    }

                    Stream zipStream = zf.GetInputStream(zipEntry);
                    using (new ServerContext())
                    {
                        importService.Import(collectTask, zipStream);
                    }

                    progressManager.SetCompleted(current++ / total);
                }

                progressManager.SetCompleted(1);

                result.Success = true;
                result.ExtraParams["msg"] = "Импорт формы успешно выполнен.";
                result.IsUpload = true;
                return result;
            }
            catch (Exception e)
            {
                ViewData.ModelState.AddModelError("dummy", e);
                result.Success = false;
                result.Script = null;
                result.ExtraParams["msg"] = "Ошибка импорта формы.";
                result.ExtraParams["responseText"] = e.Message;
                result.IsUpload = true;
                return result;
            }
        }

        [HttpPost]
        public AjaxResult Delete(int taskId)
        {
            var taskRepository = new NHibernateLinqRepository<D_CD_Task>();

            var collectTask = collectTaskRepository.FindOne(taskId);

            using (new ServerContext())
            {
                var tasks = taskRepository.GetAll().Where(x => x.RefCollectTask == collectTask);

                foreach (var task in tasks)
                {
                    Trace.TraceError("Подготовка к удалению задачи ID= " + task.ID);

                    Trace.TraceError("Удаление данных...");
                    var reports = reportRepository.FindAll().Where(x => x.RefTask == task);
                    using (var db = Scheme.SchemeDWH.DB)
                    {
                        foreach (var report in reports.ToList())
                        {
                            DeleteReportReq(db, report, RequisiteKinds.Header);

                            foreach (var section in report.Sections.ToList())
                            {
                                DeleteSqctionReq(db, section, report, RequisiteKinds.Header);

                                DeleteTableRow(db, section, report);
                                
                                DeleteSqctionReq(db, section, report, RequisiteKinds.Footer);

                                DeleteSection(db, section.ID);
                            }

                            DeleteReportReq(db, report, RequisiteKinds.Footer);

                            DeleteReport(db, report.ID);
                        }

                        DeleteTask(db, task.ID);
                    }
                }

                using (var db = Scheme.SchemeDWH.DB)
                {
                    DeleteCollectTask(db, collectTask.ID);
                }
            }

            return new AjaxResult();
        }

        private static void AddZipFile(string name, Stream templateFileStream, ZipOutputStream zipOutStream)
        {
            ZipEntry newEntry = new ZipEntry(name);
            newEntry.DateTime = DateTime.Now;

            zipOutStream.PutNextEntry(newEntry);

            StreamUtils.Copy(templateFileStream, zipOutStream, new byte[4096]);
            templateFileStream.Close();

            zipOutStream.CloseEntry();
        }

        private static void DeleteCollectTask(IDatabase db, int collectTaskId)
        {
            try
            {
                db.ExecQuery("delete from D_CD_CollectTask where id = ?", QueryResultTypes.NonQuery, new DbParameterDescriptor("ID", collectTaskId));
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
            }
        }

        private static void DeleteTask(IDatabase db, int taskId)
        {
            try
            {
                db.ExecQuery("delete from D_CD_Task where id = ?", QueryResultTypes.NonQuery, new DbParameterDescriptor("ID", taskId));
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
            }
        }

        private static void DeleteReport(IDatabase db, int reportId)
        {
            try
            {
                db.ExecQuery("delete from D_CD_Report where id = ?", QueryResultTypes.NonQuery, new DbParameterDescriptor("ID", reportId));
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
            }
        }

        private static void DeleteSection(IDatabase db, int sectionId)
        {
            try
            {
                db.ExecQuery("delete from D_Report_Section where id = ?", QueryResultTypes.NonQuery, new DbParameterDescriptor("ID", sectionId));
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
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
                if (ExistsObject(db, tableName))
                {
                    db.ExecQuery(
                        "delete from {0} where ID = ?".FormatWith(tableName),
                        QueryResultTypes.NonQuery,
                        new DbParameterDescriptor("ID", report.ID));
                }
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
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
                if (ExistsObject(db, tableName))
                {
                    db.ExecQuery(
                        "delete from {0} where ID = ?".FormatWith(tableName),
                        QueryResultTypes.NonQuery,
                        new DbParameterDescriptor("ID", report.ID));
                }
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
            }
        }

        private static void DeleteTableRow(IDatabase db, D_Report_Section section, D_CD_Report report)
        {
            var tableName = ScriptingUtils.GetSectionTableName(
                report.RefForm.InternalName,
                section.RefFormSection.InternalName,
                report.RefForm.FormVersion,
                "rw");

            var dt = (DataTable)db.ExecQuery("select ID from D_Report_Row where RefSection = ?", QueryResultTypes.DataTable, new DbParameterDescriptor("Section", section.ID));

            foreach (DataRow row in dt.Rows)
            {
                try
                {
                    db.ExecQuery(
                        "delete from {0} where ID = ?".FormatWith(tableName),
                        QueryResultTypes.NonQuery,
                        new DbParameterDescriptor("ID", Convert.ToInt32(row[0])));
                }
                catch (Exception e)
                {
                    Trace.TraceError(e.Message);
                }
            }

            try
            {
                db.ExecQuery(
                "delete from D_Report_Row where RefSection = ?",
                QueryResultTypes.NonQuery,
                new DbParameterDescriptor("ID", section.ID));
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
            }
        }

        private static bool ExistsObject(IDatabase db, string objectName)
        {
            return 1 == Convert.ToInt32(db.ExecQuery("select count(*) from DVDB_Objects where Name = ? and Type = 'TABLE'", QueryResultTypes.Scalar, new DbParameterDescriptor("Name", objectName.ToUpper())));
        }
    }
}
