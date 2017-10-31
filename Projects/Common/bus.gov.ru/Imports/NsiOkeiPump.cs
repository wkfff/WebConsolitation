using System;
using System.Linq;

using bus.gov.ru.external.Item1;

using Krista.FM.Common;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;

namespace bus.gov.ru.Imports
{
    public class NsiOkeiPump
    {
        private readonly ILinqRepository<D_Org_OKEI> okeiRepository;
        
        public NsiOkeiPump()
        {
            okeiRepository = Resolver.Get<ILinqRepository<D_Org_OKEI>>();
        }

        public int PumpFile(nsiOkei.bodyLocalType pumpData)
        {
            okeiRepository.DbContext.BeginTransaction();
            pumpData.position.Where(x => x.businessStatus.Equals("801")).Each(
                p =>
                    {
                        var okei = okeiRepository.FindAll().SingleOrDefault(x => x.Code == Convert.ToInt32(p.code)) ??
                                      new D_Org_OKEI { Code = Convert.ToInt32(p.code) };

                        okei.Name = p.name ?? "Не указано";
                        okei.Symbol = p.symbol ?? "Не указано";

                        okeiRepository.Save(okei);
                    });

            okeiRepository.DbContext.CommitChanges();
            okeiRepository.DbContext.CommitTransaction();

            return -1;
        }
    }
}
