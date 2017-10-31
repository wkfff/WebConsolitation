using System.Collections.Generic;
using System.Globalization;
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
    /// закачка nsiSubjectService
    /// </summary>
    public partial class BusGovRuPumpModule : DataPumpModuleBase, IDataPumpModule
    {
        private Dictionary<string, D_Org_OrgYchr> _founderCache;
        private Dictionary<string, D_Services_TipY> _servicesTypeCache;
        private readonly List<KeyValuePair<string, D_Services_VedPer>> _subjectServiceHierarhyPumpCache =
            new List<KeyValuePair<string, D_Services_VedPer>>();

        private void ProcessNsiSubjectServicePumpFile(FileInfo fileInfo)
        {
            List<nsiSubjectServiceType> pumpData =
                nsiSubjectService.Load(fileInfo.OpenText()).body.position
                    .Where(x => x.ppo.code.StartsWith(ClientLocationOkatoCodePrefix))
                    //.Where(type => type.businessStatus.Equals("801"))
                    .ToList();

            var subjectServiceRepository = Resolver.Get<ILinqRepository<D_Services_VedPer>>();
            subjectServiceRepository.DbContext.BeginTransaction();

            pumpData.Each(
                type =>
                    {
                        D_Org_OrgYchr founder =
                            type.founder
                                .If(strongType => _founderCache.ContainsKey(strongType.regNum))
                                .With(strongType => _founderCache[strongType.regNum]);
                        var founderName = founder.Return(
                            ychr => ychr.Name,
                            type.founder.With(strongType => strongType.fullName));

                        D_Org_PPO ppo =
                            type.ppo
                                .If(ppoType => _ppoCache.ContainsKey(ppoType.code))
                                .With(ppoType => _ppoCache[ppoType.code]);

                        D_Org_GRBS grbs = founder
                            .With(ychr => ychr.RefNsiOgs)
                            .If(ogs => _orgStructuresByRegnumCache.ContainsKey(ogs.regNum))
                            .With(ogs => _orgStructuresByRegnumCache[ogs.regNum])
                            .With(structure => structure.RefOrgGRBS);

                        type.service.Each(
                            serviceType =>
                            (subjectServiceRepository.FindAll().FirstOrDefault(
                                per => per.Code.Equals(serviceType.code) && per.RefOrgPPO.Code.Equals(type.ppo.code))
                             ?? new D_Services_VedPer
                                    {
                                        Code = serviceType.code,
                                        Name = serviceType.name,
                                        NumberService = serviceType.number,
                                        RefSferaD =
                                            Resolver.Get<ILinqRepository<D_Services_SferaD>>().FindAll()
                                                .FirstOrDefault(d => d.Name.Equals(serviceType.field.name))
                                            ?? new D_Services_SferaD
                                                   {
                                                       Code = serviceType.field.code.Return(s => s, string.Empty),
                                                       Name = serviceType.field.name,
                                                   },
                                        RefTipY = _servicesTypeCache[serviceType.type],
                                    }
                                    .Do(
                                        per => serviceType.enactment.Do(
                                            localType =>
                                                {
                                                    per.AuthorLaw =
                                                        localType.author.Return(
                                                            softType => softType.fullName, string.Empty);
                                                    per.DateLaw = localType.date;
                                                    per.NameLow = localType.name.Return(s => s, string.Empty);
                                                    per.NumberLaw = localType.number.Return(s => s, string.Empty);
                                                    per.TypeLow = localType.type.Return(s => s, string.Empty);
                                                })))
                                .Do(
                                    per =>
                                        {
                                            // изменяемые параметры услуг
                                            per.BusinessStatus = type.businessStatus;
                                            per.DataIskluch = type.endDateActive;
                                            per.DataVkluch = type.startDateActive;
                                        })
                                .Do(
                                    per =>
                                        {
                                            // восстанавливаем ссылки, 
                                            // тк первоначально справочники могли быть в не актуальном состоянии
                                            grbs.Do(orgGRBS => per.RefGRBSs = orgGRBS);
                                            ppo.Do(orgPPO => per.RefOrgPPO = orgPPO);
                                            founder.Do(ychr => per.RefYchred = ychr);
                                            founderName.Do(s => per.Founder = s);
                                        })
                                .Do(
                                    per =>
                                    serviceType.category
                                        .With(list => list.Select(categoryType => categoryType.name))
                                        .Do(
                                            list =>
                                                {
                                                    // категории потребителей услуги
                                                    var customers = list.Except(
                                                        per.Customers.Select(ys => ys.RefCPotr.Name).ToList());
                                                    customers.Each(
                                                        s => per.Customers.Add(
                                                            new F_F_PotrYs
                                                                {
                                                                    RefCPotr =
                                                                        Resolver
                                                                            .Get<ILinqRepository<D_Services_CPotr>>()
                                                                            .FindAll()
                                                                            .FirstOrDefault(potr => potr.Name.Equals(s))
                                                                        ?? new D_Services_CPotr {Name = s,},
                                                                    RefVedPP = per,
                                                                }));
                                                }))
                                .Do(subjectServiceRepository.Save)
                                .Do(
                                    per => serviceType.parentCode.Do(
                                        s =>
                                        _subjectServiceHierarhyPumpCache.Add(
                                            new KeyValuePair<string, D_Services_VedPer>(s, per)))));
                    });

            subjectServiceRepository.DbContext.CommitChanges();
            subjectServiceRepository.DbContext.CommitTransaction();
        }

        private void AfterProcessNsiSubjectServicePumpFiles()
        {
            var subjectServiceRepository = Resolver.Get<ILinqRepository<D_Services_VedPer>>();
            Dictionary<string, int> subjectService = subjectServiceRepository.FindAll()
                .ToDictionary(service => service.Code, service => service.ID);

            WriteToTrace("Востанавливаем иерархию классификатора Ведомственный перечень услуг", TraceMessageKind.Information);
            subjectServiceRepository.DbContext.BeginTransaction();
            _subjectServiceHierarhyPumpCache.Each(
                pair =>
                {
                    if (subjectService.ContainsKey(pair.Key))
                    {
                        pair.Value
                            .Do(per => per.ParentID = subjectService[pair.Key])
                            .Do(subjectServiceRepository.Save);
                    }
                    else
                    {
                        WriteEventIntoDataPumpProtocol(
                            DataPumpEventKind.dpeWarning,
                            string.Format(
                                "Не удалось восстановить иерархию NsiSubjectService={0}, parentCode={1} не найден",
                                pair.Value.Code,
                                pair.Key));
                    }
                });

            subjectServiceRepository.DbContext.CommitChanges();
            subjectServiceRepository.DbContext.CommitTransaction();
        }

        private void BeforeProcessNsiSubjectServicePumpFiles()
        {
            _ppoCache = BuildPpoCache();

            WriteToTrace("Построение кэша учредителей", TraceMessageKind.Information);
            _founderCache = Resolver.Get<ILinqRepository<D_Org_OrgYchr>>().FindAll()
                .Where(ychr => ychr.RefNsiOgs != null)
                .ToDictionary(ychr => ychr.RefNsiOgs.regNum, ychr => ychr);

            WriteToTrace("Построение кэша справочника 'Виды услуг'", TraceMessageKind.Information);
            _servicesTypeCache = Resolver.Get<ILinqRepository<D_Services_TipY>>().FindAll()
                .ToDictionary(y => y.Name.Equals("Работа") ? "W" : "S", y => y);

            _orgStructuresByRegnumCache = BuildOrgStructureByRegnumCache();

        }
    }
}
