using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using bus.gov.ru.external.Item1;
using bus.gov.ru.types.Item1;
using Krista.FM.Common;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.ServerLibrary;
using Microsoft.Practices.Unity.Utility;

namespace Krista.FM.Server.DataPumps.BusGovRuPump
{
    /// <summary>
    /// закачка nsiOgs
    /// </summary>
    public partial class BusGovRuPumpModule
    {
        private IDictionary<string, D_Org_GRBS> grbsCache;
        private Dictionary<string, NsiOgsCachedItem> nsiOgsPumpCached;
        private Dictionary<string, D_Org_PPO> ppoByOkatoCache;
        private bool toProcessFounder;
        private bool toProcessOrgs;
        private Dictionary<string, FX_Org_TipYch> ychCache;

        private void ProcessNsiOgsPumpFile(FileInfo fileInfo)
        {
            // 01 - федеральный орган государственной власти (орган государственной власти субъекта РФ, орган местного самоуправления);
            // 03 - бюджетное учреждение;
            // 08 -  казенное учреждение;
            // 10 - автономное учреждение;
            var validOrgTypes = new[] { "01", "03", "08", "10" };

            // 13 Собственность субъектов Российской Федерации
            // 14 Муниципальная собственность
            var validOkfsCode = new[] { "13", "14" };
            List<nsiOgsType> pumpData =
                nsiOgs.Load(fileInfo.OpenText()).body.position
                    .Where(type => type.main.inn.StartsWith(ClientLocationRegionNumber) &&
                                   type.businessStatus != null && type.businessStatus.Equals(D_Org_NsiOGS.Included))
                    .ToList();

            // возможно изменится ключевой атрибут другого справочника
            // надо сохранить связь
            var orgKeysCache = new Dictionary<string, Pair<string, string>>();
            
            // pump D_Org_NsiOGS
            ProcessNsiOgs(pumpData, orgKeysCache);

            // pump D_Org_Structure
            if (toProcessOrgs)
            {
                ProcessOrgStructure(pumpData, orgKeysCache, validOkfsCode, validOrgTypes);
            }

            pumpData.Where(type => nsiOgsPumpCached.Keys.Contains(type.regNum)).Each(
                type =>
                WriteEventIntoDataPumpProtocol(
                    DataPumpEventKind.dpeWarning,
                    String.Format("Учреждение {0}, ИНН={1},КПП={2}, уже импортированно из более позднего файла.", type.main.fullName, type.main.inn, type.main.kpp)));

            pumpData.Where(type => !nsiOgsPumpCached.Keys.Contains(type.regNum)).Each(
                pump => nsiOgsPumpCached.Add(
                    pump.regNum,
                    new NsiOgsCachedItem
                        {
                            Founder =
                                pump.other.With(x => x.founder)
                                    .With(
                                        founder =>
                                        founder.Select(x => new NsiOgsCachedItem.RefNsiOgsSoftType(x)).ToList()) ??
                                pump.additional.With(type => type.enactment).With(type => type.founderAuthority)
                                    .With(
                                        type =>
                                        new List<NsiOgsCachedItem.RefNsiOgsSoftType>
                                            {
                                                new NsiOgsCachedItem.RefNsiOgsSoftType(type)
                                            }),
                            Grbs = pump.main.grbs.With(x => new NsiOgsCachedItem.RefNsiOgsSoftType(x)),
                            Ogrn = pump.main.ogrn,
                        }));
        }

        private void ProcessOrgStructure(
                                        IEnumerable<nsiOgsType> pumpData,
                                        Dictionary<string, Pair<string, string>> orgKeysCache,
                                        IEnumerable<string> validOkfsCode,
                                        IEnumerable<string> validOrgTypes)
        {
            var structureRepository = Resolver.Get<ILinqRepository<D_Org_Structure>>();
            var ppoRepo = Resolver.Get<ILinqRepository<D_Org_PPO>>();
            structureRepository.DbContext.BeginTransaction();
            pumpData
                .Where(type => validOrgTypes.Contains(type.main.orgType))
                .Where(type => validOkfsCode.Contains(type.main.classifier.okfs.code))
                .Each(
                    pump => structureRepository.Save(
                                (orgKeysCache.If(pairs => pairs.ContainsKey(pump.regNum)).With(
                                            pairs =>
                                            structureRepository.FindAll()
                                             .SingleOrDefault(
                                                 structure =>
                                                 structure.INN.Equals(pairs[pump.regNum].First) &&
                                                 structure.KPP.Equals(pairs[pump.regNum].Second)))
                                 ?? structureRepository.FindAll()
                                        .SingleOrDefault(
                                            structure =>
                                            structure.INN.Equals(pump.main.inn) &&
                                            structure.KPP.Equals(pump.main.kpp))
                                 ?? new D_Org_Structure())
                                    .Do(
                                        x =>
                                            {
                                                x.INN = pump.main.inn;
                                                x.KPP = pump.main.kpp;
                                                x.Name = pump.main.fullName;
                                                x.ShortName = pump.main.shortName ?? string.Empty;
                                                x.RefTipYc = ychCache[pump.main.orgType];
                                                x.RefOrgPPO = pump.additional.If(type => type.ppoOKATO != null)
                                                                  .With(type => type.ppoOKATO.code.If(s => ppoByOkatoCache.ContainsKey(s)))
                                                                  .With(s => ppoByOkatoCache[s]) ??
                                                              pump.additional.If(p => p.okato != null)
                                                                  .With(p => p.okato.code.If(s => ppoByOkatoCache.ContainsKey(s)))
                                                                  .With(s => ppoByOkatoCache[s]) ??
                                                              pump.main.If(type => type.ppo != null)
                                                                  .With(type => ppoRepo.FindAll().FirstOrDefault(ppo => ppo.Name == type.ppo.name));

                                                // восстанавливаем ссылку на ППО
                                                if (x.RefOrgPPO == null)
                                                {
                                                    var msg = string.Format(
                                                        "Во время обработки учреждения '{0}'(ИНН=;{1};КПП={2}), " +
                                                        "в справочнике ППО не найден OKАTO={3}{4} - RefOrgPPO не восстановлена",
                                                        pump.main.fullName,
                                                        pump.main.inn,
                                                        pump.main.kpp,
                                                        pump.additional.If(p => p.okato != null).With(p => p.okato.code),
                                                        pump.additional.If(type => type.ppoOKATO != null)
                                                            .With(type => type.ppoOKATO.code)
                                                            .Return(
                                                                s => string.Format("(ППО ОКАТО={0})", s), string.Empty));

                                                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, msg);
                                                }
                                            })
                                    .Do(
                                        structure =>
                                        structure
                                            .Unless(orgStructure => toProcessFounder)
                                            .If(
                                                orgStructure =>
                                                pump.main.grbs.Return(
                                                    type => grbsCache.ContainsKey(type.fullName), false))
                                            .Do(
                                                orgStructure =>
                                                orgStructure.RefOrgGRBS = grbsCache[pump.main.grbs.fullName]))));
            structureRepository.DbContext.CommitChanges();
            structureRepository.DbContext.CommitTransaction();
        }

        private void ProcessNsiOgs(
                                    IEnumerable<nsiOgsType> pumpData,
                                    Dictionary<string, Pair<string, string>> orgKeysCache)
        {
            var regnumRepository = Resolver.Get<ILinqRepository<D_Org_NsiOGS>>();
            regnumRepository.DbContext.BeginTransaction();
            
            foreach (var pump in pumpData)
            {
                var actualOgses = regnumRepository
                    .FindAll()
                    .Where(ogs => ogs.inn.Equals(pump.main.inn) && ogs.Stats.Equals(D_Org_NsiOGS.Included))
                    .OrderByDescending(ogs => ogs.changeDate);

                // Если в закачке неактуальная информация
                if (pump.businessStatus == D_Org_NsiOGS.Excluded ||
                    actualOgses.Any(ogs => ogs.changeDate >= pump.changeDate))
                {
                    break;
                }

                // Поступила более новая информация об учреждении

                // Cохраняем связь, если ключ изменился
                if (!actualOgses.Any() ||
                    !actualOgses.First().inn.Equals(pump.main.inn) ||
                    !actualOgses.First().kpp.Equals(pump.main.kpp))
                {
                    orgKeysCache.Add(pump.regNum, new Pair<string, string>(pump.main.inn, pump.main.kpp));
                }

                // Делаем все предыдущие записи неактуальными
                foreach (var ogs in actualOgses)
                {
                    ogs.Stats = D_Org_NsiOGS.Excluded;
                    ogs.CloseDate = DateTime.Now;
                    //// todo: if not find references for this ogs - need remove this ogs
                    regnumRepository.Save(ogs);
                }

                AddOgsToRepository(regnumRepository, pump);
            }           

            regnumRepository.DbContext.CommitChanges();
            regnumRepository.DbContext.CommitTransaction();
        }

        private void AddOgsToRepository(ILinqRepository<D_Org_NsiOGS> regnumRepository, nsiOgsType pump)
        {
            var newOgs = new D_Org_NsiOGS
            {
                changeDate = pump.changeDate,
                Stats = pump.businessStatus,
                OpenDate = pump.startDateActive,
                CloseDate = pump.businessStatus == D_Org_NsiOGS.Excluded ? (pump.endDateActive ?? DateTime.Now) : (DateTime?)null,
                regNum = pump.regNum,
                inn = pump.main.inn,
                kpp = pump.main.kpp,
                FullName = pump.main.fullName,
                ShortName = pump.main.shortName ?? string.Empty
            };
            regnumRepository.Save(newOgs);
        }

        private void AfterProcessNsiOgsPumpFiles()
        {
            #region Учредители

            if (toProcessFounder)
            {
                // обрабатываем учредителей
                SetProgress(0, 0, "Обрабатываются данные учредителей", string.Empty, true);
                WriteToTrace("Обрабатываются данные учредителей", TraceMessageKind.Information);

                var founderRepository = Resolver.Get<ILinqRepository<D_Org_OrgYchr>>();
                Dictionary<string, D_Org_NsiOGS> regnumCache =
                    Resolver.Get<ILinqRepository<D_Org_NsiOGS>>().FindAll().ToDictionary(ogs => ogs.regNum, ogs => ogs);

                // разворачиваем линейно и убираем дубли
                List<NsiOgsCachedItem.RefNsiOgsSoftType> founderCache =
                    nsiOgsPumpCached
                        .Where(pair => pair.Value.Founder != null)
                        .Select(pair => pair.Value.Founder)
                        .SelectMany(list => list)
                        .Distinct()
                        .ToList();

                int founderCount = founderCache.Count;

                founderRepository.DbContext.BeginTransaction();
                founderCache
                    .Select((founder, index) => new { founder, index })
                    .Each(
                        obj =>
                        founderRepository
                            .Do(
                                repository =>
                                SetProgress(
                                    founderCount,
                                    obj.index,
                                    string.Empty,
                                    string.Format("{0} из {1}", obj.index, founderCount),
                                    true))
                            .Save(
                                (founderRepository.FindAll().SingleOrDefault(
                                    ychr => ychr.Name.Equals(obj.founder.FullName)) ??
                                 new D_Org_OrgYchr())
                                    .Do(
                                        ychr =>
                                            {
                                                ychr.Name = obj.founder.FullName;
                                                ychr.RefNsiOgs = obj.founder.RegNum.With(
                                                    s => regnumCache.ContainsKey(s) ? regnumCache[s] : null);
                                                ychr.Code = obj.founder.RegNum.With(
                                                    s => nsiOgsPumpCached.ContainsKey(s)
                                                             ? nsiOgsPumpCached[s].Ogrn
                                                             : string.Empty);
                                            })));
                founderRepository.DbContext.CommitChanges();
                founderRepository.DbContext.CommitTransaction();
            }

            #endregion

            #region ГРБС

            if (toProcessFounder && toProcessOrgs)
            {
                SetProgress(0, 0, "Восстанавливаем ссылки на ГРБС", string.Empty, true);
                WriteToTrace("Восстанавливаем ссылки на ГРБС", TraceMessageKind.Information);
                grbsCache = BuildGrbsCache();

                var structureRepository = Resolver.Get<ILinqRepository<D_Org_Structure>>();
                _orgStructuresByRegnumCache = BuildOrgStructureByRegnumCache();
                var grbsCacheList =
                    nsiOgsPumpCached
                        .Where(pair => pair.Value.Grbs != null)
                        .Select(pair => new { regNum = pair.Key, pair.Value.Grbs })
                        .ToList();
                int ogrGrbsCacheCount = grbsCacheList.Count();
                structureRepository.DbContext.BeginTransaction();
                grbsCacheList
                    .Select((arg, index) => new { arg, index })
                    .Each(
                        obj =>
                        obj.arg
                            .Do(
                                arg => SetProgress(
                                    ogrGrbsCacheCount,
                                    obj.index,
                                    string.Empty,
                                    string.Format("{0} из {1}", obj.index, ogrGrbsCacheCount),
                                    true))
                            .If(arg => grbsCache.ContainsKey(arg.Grbs.FullName))
                            .With(arg => _orgStructuresByRegnumCache[arg.regNum])
                            .Do(structure => structure.RefOrgGRBS = grbsCache[obj.arg.Grbs.FullName])
                            .Do(structureRepository.Save));
                structureRepository.DbContext.CommitChanges();
                structureRepository.DbContext.CommitTransaction();
            }

            #endregion
        }

        private void BeforeProcessNsiOgsPumpFiles()
        {
            ychCache = Resolver.Get<ILinqRepository<FX_Org_TipYch>>().FindAll()
                .ToDictionary(ych => ych.ID.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0'), ych => ych);

            _ppoCache = BuildPpoCache();

            WriteToTrace("Построение кэша ППО в разрезе ОКАТО", TraceMessageKind.Information);
            ppoByOkatoCache = Resolver.Get<ILinqRepository<D_Org_PPO>>().FindAll()
                .ToDictionary(ppo => ppo.RefOKATO.Code, ppo => ppo);

            nsiOgsPumpCached = new Dictionary<string, NsiOgsCachedItem>();

            toProcessOrgs =
                Convert.ToBoolean(
                    GetParamValueByName(
                        PumpRegistryElement.ProgramConfig,
                        "ucbNsiOgs.Process.D_Org_Structure",
                        "True"));
            toProcessFounder =
                Convert.ToBoolean(
                    GetParamValueByName(
                        PumpRegistryElement.ProgramConfig,
                        "ucbNsiOgs.Process.D_Org_OrgYchr",
                        "True"));

            if (!toProcessFounder)
            {
                grbsCache = BuildGrbsCache();
            }
        }

        private IDictionary<string, D_Org_GRBS> BuildGrbsCache()
        {
            WriteToTrace("Построение кэша ГРБС", TraceMessageKind.Information);
            return Resolver.Get<ILinqRepository<D_Org_GRBS>>().FindAll()
                .ToDictionary(grbs => grbs.Name, grbs => grbs);
        }

        #region Nested type: NsiOgsCachedItem

        private struct NsiOgsCachedItem
        {
            public List<RefNsiOgsSoftType> Founder;
            public RefNsiOgsSoftType Grbs;
            public string Ogrn;

            #region Nested type: RefNsiOgsSoftType

            public class RefNsiOgsSoftType
            {
                private readonly string fullName;
                private readonly string regNum;

                public RefNsiOgsSoftType(refNsiOgsSoftType nsiOgsSoftType)
                {
                    fullName = nsiOgsSoftType.fullName;
                    regNum = nsiOgsSoftType.regNum;
                }

                public string FullName
                {
                    get { return fullName; }
                }

                public string RegNum
                {
                    get { return regNum; }
                }
            }

            #endregion
        }

        #endregion
    }
}
