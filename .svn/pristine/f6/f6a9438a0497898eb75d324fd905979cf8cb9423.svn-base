using System;
using Quartz;

namespace Krista.FM.RIA.Core
{
    public class GlobalJobListener : IJobListener
    {
        public GlobalJobListener()
        {
        }

        public virtual string Name
        {
            get { return "MainJobListener"; }
        }

        public void JobToBeExecuted(IJobExecutionContext context)
        {
            if (context.JobDetail.JobType != typeof(Quartz.Job.FileScanJob))
            {
                Trace.TraceInformation(String.Format("Старт выполнения задачи {0}.", context.JobDetail.Key));
            }
        }

        public void JobExecutionVetoed(IJobExecutionContext context)
        {
            Trace.TraceInformation("JobExecutionVetoed");
        }

        public void JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException)
        {
            if (jobException != null)
            {
                Trace.TraceError(String.Format("При выполеннии задачи возникло исключение : {0}", jobException.Message));
            }

            if (context.JobDetail.JobType != typeof(Quartz.Job.FileScanJob))
            {
                Trace.TraceInformation(String.Format("Задача {0} выполнена успешно.", context.JobDetail.Key));
                Trace.TraceInformation(String.Format("Следующее выполнение запланировано на {0}.", context.NextFireTimeUtc));
            }
        }
    }
}