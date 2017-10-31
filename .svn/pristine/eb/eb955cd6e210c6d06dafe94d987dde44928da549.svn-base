using System.Collections.Generic;
using System.IO;
using System.Linq;

using bus.gov.ru.external.Item1;
using bus.gov.ru.types.Item1;

using Krista.FM.Common;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.BusGovRuPump
{
    public partial class BusGovRuPumpModule : DataPumpModuleBase, IDataPumpModule
    {
        private void ProcessNsiOktmoPumpFile(FileInfo fileInfo)
        {
            var pumpData = nsiOktmo.Load(fileInfo.OpenText()).body.position
                .Where(type => type.code.StartsWith(ClientLocationOkatoCodePrefix))
                .Where(type => type.businessStatus.Equals("801"))
                .ToList();

            var oktmoRepository = Resolver.Get<ILinqRepository<D_OKTMO_OKTMO>>();

            oktmoRepository.DbContext.BeginTransaction();

            PumpOktmo(pumpData, oktmoRepository);

            oktmoRepository.DbContext.CommitChanges();
            oktmoRepository.DbContext.CommitTransaction();
        }

        private void PumpOktmo(List<nsiOktmoType> pumpData, ILinqRepository<D_OKTMO_OKTMO> oktmoRepository)
        {
            foreach (var type in pumpData)
            {
                if (oktmoRepository.FindAll().Any(q => q.Code == type.code && q.Name == type.name))
                    return;

                var oktmo = new D_OKTMO_OKTMO
                    {
                        Code = type.code,
                        Name = type.name
                    };
                Resolver.Get<ILinqRepository<D_OKTMO_OKTMO>>().Save(oktmo);
            }
        }
    }
}
