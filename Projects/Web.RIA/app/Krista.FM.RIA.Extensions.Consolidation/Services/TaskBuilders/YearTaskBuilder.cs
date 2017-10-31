using System;
using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;

namespace Krista.FM.RIA.Extensions.Consolidation
{
    public class YearTaskBuilder : ITaskBuilder
    {
        private readonly ILinqRepository<D_CD_Task> taskRepository;
        private readonly ILinqRepository<D_CD_Subjects> subjectRepository;
        private readonly IRepository<FX_Date_Year> yearRepository;
        private readonly IRepository<FX_FX_FormStatus> statusRepository;

        public YearTaskBuilder(
            ILinqRepository<D_CD_Task> taskRepository,
            ILinqRepository<D_CD_Subjects> subjectRepository,
            IRepository<FX_Date_Year> yearRepository,
            IRepository<FX_FX_FormStatus> statusRepository)
        {
            this.taskRepository = taskRepository;
            this.subjectRepository = subjectRepository;
            this.yearRepository = yearRepository;
            this.statusRepository = statusRepository;
        }

        public IList<D_CD_Task> Build(D_CD_Reglaments reglament, DateTime start, DateTime end)
        {
            var tasks = new List<D_CD_Task>();

            var status = statusRepository.Get(1);
            var subjects = subjectRepository.FindAll().Where(x => x.RefRole == reglament.RefRole && x.RefLevel == reglament.RefLevel);

            for (int period = start.Year; period <= end.Year; period++)
            {
                var beginDate = new DateTime(period, 1, 1);
                var endDate = new DateTime(period + 1, 1, 1);

                if (beginDate >= start && endDate <= end)
                {
                    var year = yearRepository.Get(period);
                    foreach (var subject in subjects)
                    {
                        D_CD_Subjects subj = subject;
                        if (!taskRepository.FindAll().Any(x => 
                                x.BeginDate == beginDate 
                                && x.EndDate == endDate 
                                && x.RefSubject == subj 
                                && x.RefTemplate == reglament.RefTemplate 
                                && x.RefYear == year))
                        {
                            D_CD_Task task = new D_CD_Task
                            {
                                Name = "{0} за {1} год".FormatWith(reglament.RefTemplate.Name, period),
                                OwnerUserId = subject.UserId,
                                BeginDate = beginDate,
                                EndDate = endDate,
                                Deadline = endDate.AddDays(reglament.Laboriousness - 1),
                                RefSubject = subject,
                                RefTemplate = reglament.RefTemplate,
                                RefYear = year,
                                RefStatus = status
                            };

                            taskRepository.Save(task);

                            tasks.Add(task);
                        }
                    }
                }
            }

            return tasks;
        }
    }
}
