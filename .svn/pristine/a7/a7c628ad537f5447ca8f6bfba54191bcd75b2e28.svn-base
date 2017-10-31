using System;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.Region10MarksOIV.Services
{
    public class OivService : IOivService
    {
        private readonly IFactRepository factRepository;
        private readonly IRepository<FX_OIV_StatusData> statusRepository;

        public OivService(
                            IFactRepository factRepository,
                            IRepository<FX_OIV_StatusData> statusRepository)
        {
            this.factRepository = factRepository;
            this.statusRepository = statusRepository;
        }

        public void Accept()
        {
            var facts = factRepository.GetMarksForIMA();

            if (facts.Any(x => x.RefStatusData.ID != (int)OivStatus.Approved))
            {
                throw new Exception("Доклад не может быть принят, так как не все показатели утверждены УИМ или находятся на редактировании у МО.");
            }

            var state = statusRepository.Get((int)OivStatus.Accepted);

            foreach (var fact in facts)
            {
                fact.RefStatusData = state;
                factRepository.Save(fact);
            }

            factRepository.DbContext.CommitChanges();
        }

        public void Reject()
        {
            var state = statusRepository.Get((int)OivStatus.Approved);

            foreach (var fact in factRepository.GetMarksForIMA()
                                                   .Where(x => x.RefStatusData.ID == (int)OivStatus.Accepted))
            {
                fact.RefStatusData = state;
                factRepository.Save(fact);
            }

            factRepository.DbContext.CommitChanges();
        }
    }
}
