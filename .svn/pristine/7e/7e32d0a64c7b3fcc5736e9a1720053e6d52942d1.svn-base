using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.Consolidation.Forms.Mrot
{
    public class JobTitleService : IJobTitleService
    {
        private readonly ILinqRepository<D_Report_TAJobTitle> jobTitleRepository;

        public JobTitleService(ILinqRepository<D_Report_TAJobTitle> jobTitleRepository)
        {
            this.jobTitleRepository = jobTitleRepository;
        }

        public IList<D_Report_TAJobTitle> GetRegionExecuters(int regionId)
        {
            return jobTitleRepository.FindAll()
                .Where(r => r.RefReport.RefTask.RefSubject.ID == regionId)
                .ToList();
        }

        public D_Report_TAJobTitle GetTaskExecuters(int taskId)
        {
            return jobTitleRepository.FindAll()
                .Where(r => r.RefReport.RefTask.ID == taskId)
                .FirstOrDefault();
        }
    }
}
