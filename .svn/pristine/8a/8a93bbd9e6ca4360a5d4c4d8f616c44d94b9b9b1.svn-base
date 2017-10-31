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
    /// <summary>
    /// закачка nsiPpo
    /// </summary>
    public partial class BusGovRuPumpModule : DataPumpModuleBase, IDataPumpModule
    {
        private readonly List<KeyValuePair<string, D_Org_PPO>> ppoHierarhyPumpCache = new List<KeyValuePair<string, D_Org_PPO>>();

        private void ProcessNsiPpoPumpFile(FileInfo fileInfo)
        {
            List<nsiPpoType> pumpData = nsiPpo.Load(fileInfo.OpenText()).body.position
                .Where(type => type.code.StartsWith(ClientLocationOkatoCodePrefix) || type.code.Equals("00000000000"))
                .Where(type => type.businessStatus.Equals("801"))
                .ToList();

            var ppoRepository = Resolver.Get<ILinqRepository<D_Org_PPO>>();
            ppoRepository.DbContext.BeginTransaction();

            pumpData.Each(
                type => type.Unless(okatoType => ppoRepository.FindAll().Any(ppo => ppo.Code.Equals(type.code)))
                    .With(
                        ppoType => new D_Org_PPO
                            {
                                Code = ppoType.code,
                                Name = ppoType.name,
                                RefOKATO = PumpOkato(type),
                            })
                    .Do(ppoRepository.Save)
                    .Do(
                        ppo =>
                            type.parentCode
                                .Do(s => this.ppoHierarhyPumpCache.Add(new KeyValuePair<string, D_Org_PPO>(s, ppo)))));
            
            ppoRepository.DbContext.CommitChanges();
            ppoRepository.DbContext.CommitTransaction();
        }

        private D_OKATO_OKATO PumpOkato(nsiPpoType type)
        {
            var okatoRepo = Resolver.Get<ILinqRepository<D_OKATO_OKATO>>();

            var cachedOkato = _okatoCache.FirstOrDefault(x => x.Code == type.code && x.Name == type.name);
            if (cachedOkato != null)
                return cachedOkato;

            // дублируем ППО в ОКАТО
            var okato = new D_OKATO_OKATO
                {
                    Code = type.code,
                    Name = type.name
                };
            okatoRepo.Save(okato);
            _okatoCache.Add(okato);
            return okato;
        }

        private void AfterProcessNsiPpoPumpFiles()
        {
            var ppoRepository = Resolver.Get<ILinqRepository<D_Org_PPO>>();
            Dictionary<string, int> ppoCache = ppoRepository.FindAll().ToDictionary(
                ppo => ppo.Code, ppo => ppo.ID);

            WriteToTrace("Востанавливаем иерархию классификатора ППО", TraceMessageKind.Information);
            ppoRepository.DbContext.BeginTransaction();
            this.ppoHierarhyPumpCache.Each(
                pair =>
                    {
                        if (ppoCache.ContainsKey(pair.Key))
                        {
                            pair.Value
                                .Do(okato => okato.ParentID = ppoCache[pair.Key])
                                .Do(ppoRepository.Save);
                        }
                        else
                        {
                            var msg = string.Format("Не удалось восстановить иерархию ППО={0}, parentCode={1} не найден", pair.Value.Name, pair.Key);
                            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, msg);
                        }
                    });

            ppoRepository.DbContext.CommitChanges();
            ppoRepository.DbContext.CommitTransaction();
        }

        private void BeforeProcessNsiPpoPumpFiles()
        {
            _okatoCache = BuildOkatoCache();
        }
    }
}
