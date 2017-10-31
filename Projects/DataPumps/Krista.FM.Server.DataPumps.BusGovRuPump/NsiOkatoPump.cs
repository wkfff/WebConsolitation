using System.Collections.Generic;
using System.IO;
using System.Linq;
using Krista.FM.Common;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Domain.Reporitory.NHibernate.IoC;
using Krista.FM.Extensions;
using Krista.FM.ServerLibrary;
using bus.gov.ru.external.Item1;
using bus.gov.ru.types.Item1;

namespace Krista.FM.Server.DataPumps.BusGovRuPump
{
    /// <summary>
    /// закачка nsiOkato
    /// </summary>
    public partial class BusGovRuPumpModule : DataPumpModuleBase, IDataPumpModule
    {
        private readonly List<KeyValuePair<string, D_OKATO_OKATO>> _okatoHierarhyPumpCache =
            new List<KeyValuePair<string, D_OKATO_OKATO>>();

        private void ProcessNsiOkatoPumpFile(FileInfo fileInfo)
        {
            List<nsiOkatoType> pumpData = nsiOkato.Load(fileInfo.OpenText()).body.position
                .Where(type => type.code.StartsWith(ClientLocationOkatoCodePrefix) || type.code.Equals("00000000000"))
                .Where(type => type.businessStatus.Equals("801"))
                .ToList();

            var okatoRepository = Resolver.Get<ILinqRepository<D_OKATO_OKATO>>();
            okatoRepository.DbContext.BeginTransaction();

            pumpData.Each(
                type => type.Unless(
                    okatoType => okatoRepository.FindAll()
                                     .Any(okato => okato.Code.Equals(type.code)))
                            .With(
                                okatoType => new D_OKATO_OKATO
                                                 {
                                                     Code = okatoType.code,
                                                     Name = okatoType.name,
                                                 })
                            .Do(okatoRepository.Save)
                            .Do(
                                okato =>
                                type.parentCode.Do(
                                    s => _okatoHierarhyPumpCache.Add(new KeyValuePair<string, D_OKATO_OKATO>(s, okato)))));

            okatoRepository.DbContext.CommitChanges();
            okatoRepository.DbContext.CommitTransaction();
        }

        private void AfterProcessNsiOkatoPumpFiles()
        {
            var okatoRepository = Resolver.Get<ILinqRepository<D_OKATO_OKATO>>();
            Dictionary<string, int> okatoCache = okatoRepository.FindAll().ToDictionary(
                okato => okato.Code, okato => okato.ID);

            WriteToTrace("Востанавливаем иерархию классификатора ОКАТО", TraceMessageKind.Information);
            okatoRepository.DbContext.BeginTransaction();
            _okatoHierarhyPumpCache.Each(
                pair =>
                    {
                        if (okatoCache.ContainsKey(pair.Key))
                        {
                            pair.Value
                                .Do(okato => okato.ParentID = okatoCache[pair.Key])
                                .Do(okatoRepository.Save);
                        }
                        else
                        {
                            WriteEventIntoDataPumpProtocol(
                                DataPumpEventKind.dpeWarning,
                                string.Format(
                                    "Не удалось восстановить иерархию OKATO={0}, parentCode={1} не найден",
                                    pair.Value.Code,
                                    pair.Key));
                        }
                    });

            okatoRepository.DbContext.CommitChanges();
            okatoRepository.DbContext.CommitTransaction();
        }

        //private void BeforeProcessNsiOkatoPumpFiles()
        //{
        //    _okatoHierarhyPumpCache = new List<KeyValuePair<string, D_OKATO_OKATO>>();
        //}
    }
}
