using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Extensions.MarksOMSU.Services.Exceptions;

namespace Krista.FM.RIA.Extensions.MarksOMSU.Services
{
    public class ReportService : IReportService
    {
        private readonly IMarksOmsuRepository marksOmsuRepository;
        private readonly IRegionsRepository regionRepository;
        private readonly IRepository<FX_OMSU_StatusData> statusRepository;

        public ReportService(
                            IMarksOmsuRepository marksOmsuRepository,
                            IRegionsRepository regionRepository,
                            IRepository<FX_OMSU_StatusData> statusRepository)
        {
            this.marksOmsuRepository = marksOmsuRepository;
            this.regionRepository = regionRepository;
            this.statusRepository = statusRepository;
        }

        public void Accept(int regionId)
        {
            var facts = marksOmsuRepository.GetAllMarksForMO(regionRepository.FindOne(regionId));

            if (facts.Any(x => x.RefStatusData.ID != 3))
            {
                throw new CannotAcceptReportException();
            }

            var state = statusRepository.Get(4);

            foreach (var fact in facts)
            {
                fact.RefStatusData = state;
                marksOmsuRepository.Save(fact);
            }

            marksOmsuRepository.DbContext.CommitChanges();
        }

        public void Reject(int regionId)
        {
            var state = statusRepository.Get(3);

            foreach (var fact in marksOmsuRepository.GetAllMarksForMO(regionRepository.FindOne(regionId))
                .Where(x => x.RefStatusData.ID == 4))
            {
                fact.RefStatusData = state;
                marksOmsuRepository.Save(fact);
            }

            marksOmsuRepository.DbContext.CommitChanges();
        }
    }
}
