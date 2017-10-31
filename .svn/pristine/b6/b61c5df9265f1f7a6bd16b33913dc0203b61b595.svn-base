using System;
using System.Linq;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;

namespace Krista.FM.RIA.Extensions.Consolidation.Forms.Mrot
{
    public class JobTitleController : SchemeBoundController
    {
        private readonly ILinqRepository<D_Report_TrihedrAgr> reportRepository;
        private readonly IRepository<D_CD_Task> taskRepository;
        private readonly ILinqRepository<D_Report_TAJobTitle> jobTitlesRepository;

        public JobTitleController(
            ILinqRepository<D_Report_TrihedrAgr> reportRepository,
            IRepository<D_CD_Task> taskRepository,
            ILinqRepository<D_Report_TAJobTitle> jobTitlesRepository)
        {
            this.reportRepository = reportRepository;
            this.taskRepository = taskRepository;
            this.jobTitlesRepository = jobTitlesRepository;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [Transaction]
        public RestResult Create(int taskId, string data)
        {
            try
            {
                var task = taskRepository.Get(taskId);
                if (task.RefStatus.ID != (int)TaskViewModel.TaskStatus.Edit)
                {
                    return new RestResult { Message = "Изменять данные отчета запрещено.", Success = false };
                }

                D_Report_TAJobTitle record = JavaScriptDomainConverter<D_Report_TAJobTitle>.DeserializeSingle(data);

                record.ID = 0;
                var report = reportRepository.FindAll().Where(x => x.RefTask == task).FirstOrDefault();
                if (report == null)
                {
                    report = new D_Report_TrihedrAgr { RefTask = task };
                    reportRepository.Save(report);
                }

                record.RefReport = report;
                jobTitlesRepository.Save(record);
                jobTitlesRepository.DbContext.CommitChanges();

                return new RestResult
                {
                    Success = true,
                    Message = "Новая запись добавлена",
                    Data = (from d in jobTitlesRepository.FindAll() where d.ID == record.ID select new { d.ID, d.Name, d.Office, d.Phone, RefReport = d.RefReport.ID }).ToList()
                };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public RestResult Read(int taskId)
        {
            try
            {
                var list = (from d in jobTitlesRepository.FindAll()
                            join r in reportRepository.FindAll() on d.RefReport equals r
                            where r.RefTask.ID == taskId
                            select new { d.ID, d.Name, d.Office, d.Phone, RefReport = d.RefReport.ID }).ToList();

                return new RestResult { Success = true, Data = list };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [AcceptVerbs(HttpVerbs.Put)]
        [Transaction]
        public RestResult Update(int id, string data)
        {
            try
            {
                JsonObject fields = JSON.Deserialize<JsonObject>(data);

                var record = jobTitlesRepository.FindOne(id);

                if (record.RefReport.RefTask.RefStatus.ID != (int)TaskViewModel.TaskStatus.Edit)
                {
                    return new RestResult { Message = "Изменять данные отчета запрещено.", Success = false };
                }

                record.Name = fields["Name"].ToString();
                record.Office = fields["Office"].ToString();
                record.Phone = fields["Phone"].ToString();
                record.RefReport = reportRepository.FindOne(Convert.ToInt32(fields["RefReport"]));

                jobTitlesRepository.Save(record);
                jobTitlesRepository.DbContext.CommitChanges();

                return new RestResult { Success = true, Message = "Запись обновлена" };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [AcceptVerbs(HttpVerbs.Delete)]
        [Transaction]
        public RestResult Destroy(int id)
        {
            try
            {
                var record = jobTitlesRepository.FindOne(id);
                if (record.RefReport.RefTask.RefStatus.ID != (int)TaskViewModel.TaskStatus.Edit)
                {
                    return new RestResult { Message = "Изменять данные отчета запрещено.", Success = false };
                }

                jobTitlesRepository.Delete(record);
                jobTitlesRepository.DbContext.CommitChanges();

                return new RestResult { Success = true, Message = "Запись удалена" };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }
    }
}
