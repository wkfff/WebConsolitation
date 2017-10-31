using System;
using System.Collections.Generic;
using System.Linq;

using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core.Progress;
using Krista.FM.RIA.Extensions.Consolidation.Forms;

namespace Krista.FM.RIA.Extensions.Consolidation
{
    public class TaskBuilderService : ITaskBuilderService
    {
        private readonly IReportBuilder reportBuilder;
        private readonly IRepository<FX_Date_Year> yearRepository;
        private readonly IRepository<FX_FX_FormStatus> statusRepository;
        private readonly ISubjectTreeRepository subjectTreeRepository;
        private readonly ILinqRepository<D_CD_Reglaments> reglRepository;
        private readonly ILinqRepository<D_CD_Task> taskRepository;
        private readonly IProgressManager progressManager;

        public TaskBuilderService(
            IReportBuilder reportBuilder,
            IRepository<FX_Date_Year> yearRepository,
            IRepository<FX_FX_FormStatus> statusRepository,
            ISubjectTreeRepository subjectTreeRepository,
            ILinqRepository<D_CD_Reglaments> reglRepository,
            ILinqRepository<D_CD_Task> taskRepository,
            IProgressManager progressManager)
        {
            this.reportBuilder = reportBuilder;
            this.yearRepository = yearRepository;
            this.statusRepository = statusRepository;
            this.subjectTreeRepository = subjectTreeRepository;
            this.reglRepository = reglRepository;
            this.taskRepository = taskRepository;
            this.progressManager = progressManager;
        }

        public void BuildTasks(DateTime startDate, DateTime endDate, IList<D_CD_Reglaments> reglamentses)
        {
            throw new NotImplementedException();
        }

        public void BuildTasks(DateTime startDate, DateTime endDate, D_CD_Reglaments reglament)
        {
            throw new NotImplementedException();
        }

        public void BuildCollectingTask(D_CD_CollectTask task)
        {
            var subjects = subjectTreeRepository.GetChildsAndSelf(task.RefSubject);
            int subjectsCount = subjects.Count;
            var subjectsIndx = 0.0;
            foreach (var subject in subjects)
            {
                D_CD_Subjects subj = subject;
                var reglaments = reglRepository.FindAll()
                    .Where(x => x.RefRole == subj.RefRole && x.RefLevel == subj.RefLevel && x.RefRepKind == task.RefPeriod.RefRepKind)
                    .ToList();
                int reglCount = reglaments.Count;
                var reglIndx = 1.0;
                if (reglCount == 0)
                {
                    subjectsCount--;
                    subjectsIndx--;
                }

                foreach (var reglament in reglaments)
                {
                    BuildTask(task, reglament, subject);
                    progressManager.SetCompleted((subjectsIndx / subjectsCount) + (reglIndx++ / reglCount / subjectsCount));
                }

                subjectsIndx++;
            }

            progressManager.SetCompleted("Сохранение...", 0.9);
            taskRepository.DbContext.CommitChanges();
        }

        private void BuildTask(D_CD_CollectTask collectTask, D_CD_Reglaments reglament, D_CD_Subjects subject)
        {
            var year = collectTask.EndPeriod.AddDays(-1).Year;
            var period = collectTask.RefPeriod;
            var beginDate = new DateTime(year, (int)period.BeginMonth, (int)period.BeginDay);
            var endDate = new DateTime(year, (int)period.EndMonth, (int)period.EndDay);

            if (beginDate > endDate)
            {
                endDate = endDate.AddYears(1);
            }

            var task = taskRepository.FindAll()
                .FirstOrDefault(x => x.RefTemplate == reglament.RefTemplate && x.RefSubject == subject && x.RefCollectTask == collectTask && x.RefYear.ID == year);
            if (task == null)
            {
                task = new D_CD_Task
                {
                    RefSubject = subject,
                    RefCollectTask = collectTask,
                    RefTemplate = reglament.RefTemplate,
                    RefStatus = statusRepository.Get(1),
                    RefYear = yearRepository.Get(year),
                    BeginDate = beginDate,
                    EndDate = endDate,
                    OwnerUserId = subject.UserId,
                    Deadline = collectTask.ProvideDate,
                    Name = "-"
                };

                taskRepository.Save(task);
                
                reportBuilder.CreateReport(task);
            }
            else
            {
            }
        }
    }
}
