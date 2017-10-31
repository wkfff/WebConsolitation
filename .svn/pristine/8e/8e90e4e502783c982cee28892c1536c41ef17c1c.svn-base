using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using bus.gov.ru.external.Item1;

using Bus.Gov.Ru.Imports;

using bus.gov.ru.types.Item1;

using Krista.FM.Common;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.BusGovRuPump
{
    /// <summary>
    /// закачка institutionInfo
    /// </summary>
    public partial class BusGovRuPumpModule : DataPumpModuleBase, IDataPumpModule
    {
        private List<D_Org_PrOKVED> activityTypeCache;
        private List<D_Org_TipFil> branchTypesCache;
        private List<D_Org_Category> categoriesCache;
        private Dictionary<string, D_Org_OrgYchr> founderByNameCache;
        private List<D_Org_VidOrg> instititionTypeCache;
        private List<D_OKFS_OKFS> okfsCache;
        private List<D_OKOPF_OKOPF> okopfCache;
        private List<D_OKTMO_OKTMO> oktmoCache;

        private void ProcessInstitutionInfoPumpFile(FileInfo fileInfo)
        {
            institutionInfoType pumpData = institutionInfo.Load(fileInfo.OpenText()).body.position;
            refNsiConsRegExtendedStrongType targetOrg = pumpData.initiator ?? pumpData.placer;

            if (!CheckDocumentTarget(targetOrg))
            {
                return;
            }

            // магические числа из фиксированных классификаторов
            const int PassportDocTypeID = 1;
            const int ExportedStateID = 8;

            var headerRepository = Resolver.Get<ILinqRepository<F_F_ParameterDoc>>();

            headerRepository.DbContext.BeginTransaction();

            F_F_ParameterDoc header =
                _orgStructuresByRegnumCache[targetOrg.regNum]
                    .Unless(x => _toPumpNewDocument)
                    .With(x => x.Documents.FirstOrDefault(doc => doc.RefPartDoc.ID == PassportDocTypeID)) ??
                new F_F_ParameterDoc
                    {
                        PlanThreeYear = false,
                        RefPartDoc = _typeDocCache.First(typeDoc => typeDoc.ID == PassportDocTypeID),
                        RefSost = _stateDocCache.First(stateDoc => stateDoc.ID == ExportedStateID),
                        RefUchr = _orgStructuresByRegnumCache[targetOrg.regNum],
                        RefYearForm = _yearFormCache.First(form => form.ID == pumpData.changeDate.Year)
                    }.Do(SetDocumentSourceId);

            F_Org_Passport passport = (header.Passports.FirstOrDefault() ?? new F_Org_Passport())
                .Do(
                    x =>
                        {
                            x.OGRN = pumpData.main.With(type => type.ogrn) ?? string.Empty;
                            x.Fam = pumpData.other.With(type => type.chief).FirstOrDefault().With(type => type.lastName) ?? string.Empty;
                            x.NameRuc = pumpData.other.With(type => type.chief).FirstOrDefault().With(type => type.firstName) ??
                                        string.Empty;
                            x.Otch = pumpData.other.With(type => type.chief).FirstOrDefault().With(type => type.middleName) ??
                                     string.Empty;
                            x.Ordinary = pumpData.other.With(type => type.chief).FirstOrDefault().With(type => type.position) ??
                                         string.Empty;
                            x.Adr = pumpData.main
                                        .With(type => type.complexAddress.address)
                                        .With(
                                            type =>
                                            string.Format(
                                                "{0},{1},{2},{3},{4},{5},{6}",
                                                type.subject.name,
                                                type.region.With(strongType => strongType.name) ?? string.Empty,
                                                type.city.With(strongType => strongType.name) ?? string.Empty,
                                                type.locality.With(strongType => strongType.name) ?? string.Empty,
                                                type.street.With(softType => softType.name) ?? string.Empty,
                                                type.building ?? string.Empty,
                                                type.office ?? string.Empty)) ??
                                    string.Empty;
                            x.Indeks = pumpData.main.With(type => type.complexAddress.address.zip) ?? string.Empty;
                            x.Website = pumpData.additional.www ?? string.Empty;
                            x.OKPO = pumpData.main.With(type => type.classifier.okpo) ?? string.Empty;
                            x.Phone = pumpData.additional.phone;
                            x.Mail = pumpData.additional.eMail ?? string.Empty;
                            x.RefRaspor =
                                pumpData.main.With(type => type.rbs).With(
                                    type =>
                                    Resolver.Get<ILinqRepository<D_OrgGen_Raspor>>().FindAll().SingleOrDefault(raspor => raspor.Name.Equals(type.fullName))
                                    ?? new D_OrgGen_Raspor
                                        {
                                            Name = type.fullName,
                                            Code = string.Empty
                                        });
                            x.RefOKOPF = pumpData.main.With(
                                    type =>
                                    okopfCache.FirstOrDefault(
                                        okopf => okopf.Code == Convert.ToInt32(type.classifier.okopf.code)));
                            x.RefOKATO =
                                pumpData.With(
                                    type =>
                                    _okatoCache.FirstOrDefault(okato => okato.Code.Equals(type.additional.okato.code)));
                            x.RefOKFS =
                                pumpData.main.With(
                                    type =>
                                    okfsCache.First(okfs => okfs.Code == Convert.ToInt32(type.classifier.okfs.code)));
                            x.RefOKTMO =
                                oktmoCache.FirstOrDefault(oktmo => oktmo.Code.Equals(pumpData.main.classifier.oktmo.code));
                            x.RefVid =
                                instititionTypeCache.First(
                                    org => org.Code.Equals(pumpData.additional.institutionType.code));
                            x.RefCateg =
                                pumpData.additional.branch.With(
                                    list =>
                                    categoriesCache.First(category => category.Name.Equals("Головное учреждение"))) ??
                                pumpData.additional.branchInfo.With(
                                    type =>
                                        {
                                            switch (type.type)
                                            {
                                                case "01":
                                                    return
                                                        categoriesCache.First(
                                                            category => category.Name.Equals("Филиал"));
                                                case "02":
                                                    return
                                                        categoriesCache.First(
                                                            category =>
                                                            category.Name.Equals(
                                                                "Обособленное структурное подразделение"));
                                                case "03":
                                                    return
                                                        categoriesCache.First(
                                                            category => category.Name.Equals("Представительство"));
                                                default:
                                                    return null;
                                            }
                                        }) ??
                                categoriesCache.First(category => category.Name.Equals("Не имеет филиалов"));
                            x.RefParametr = header;
                        });

            // обрабатываем учредителей
            pumpData.other
                .With(type => type.founder)
                .If(list => list.Any())
                .Do(list => ProcessPassportFounder(passport, list));

            ProcessPassportFounderAuthority(
                passport,
                pumpData.additional.enactment.With(type => type.founderAuthority));

            // обрабатываем виды деятельности
            ProcessPassportActivity(passport, pumpData.additional.activity);
            
            // обрабатываем филиалы
            pumpData.additional.branch
                .If(list => list.Any())
                .Do(list => ProcessPassportBranches(passport, list));

            // родная проверка IsNew не работает рекомендуется использовать признак ID==0
            passport
                .If(orgPassport => orgPassport.ID == 0)
                .Do(header.Passports.Add);
            
            // обрабатываем документы-основания
            ProcessDocumentsHeader(
                header,
                pumpData.document.Where(type => type.type != null).Cast<documentType>(),
                type => type.With(x => (institutionInfoType.documentLocalType)x).With(x => _documentTypesCache.First(doc => doc.Code.Equals(x.type == "C" ? "X" : x.type))));

            headerRepository.Save(header);
            headerRepository.DbContext.CommitChanges();
            headerRepository.DbContext.CommitTransaction();
        }

        private void ProcessPassportFounderAuthority(F_Org_Passport passport, refNsiConsRegSoftType pumpDataFounderAuthority)
        {
            pumpDataFounderAuthority
                .If(type => founderByNameCache.ContainsKey(type.fullName))
                .With(type => founderByNameCache[type.fullName])
                .Unless(ychr => passport.Founders.Any(founder => founder.RefYchred.ID == ychr.ID))
                .Do(
                    ychr => passport.Founders.Add(
                        new F_F_Founder
                            {
                                RefYchred = ychr,
                                RefPassport = passport
                            }));
        }

        private void ProcessPassportFounder(F_Org_Passport passport, IEnumerable<institutionInfoType.otherLocalType.founderLocalType> pumpDataFounder)
        {
            List<string> founder1 =
                passport
                    .Unless(orgPassport => orgPassport.ID == 0)
                    .With(orgPassport => orgPassport.Founders.Select(founder => founder.RefYchred.Name).ToList());

            pumpDataFounder.Each(
                type =>
                passport
                    .Unless(orgPassport => founder1.Return(list => list.Contains(type.fullName), false))
                    .If(orgPassport => founderByNameCache.ContainsKey(type.fullName))
                    .With(orgPassport => new F_F_Founder
                        {
                            RefYchred = founderByNameCache[type.fullName]
                        })
                    .Do(
                        founder => type.authorities.Do(
                            authoritiesType =>
                                {
                                    founder.formative = authoritiesType.authoritiesGMU.formative;
                                    founder.stateTask = authoritiesType.authoritiesGMU.stateTask;
                                    founder.supervisoryBoard = authoritiesType.authoritiesGMU.supervisoryBoard;
                                    founder.financeSupply = authoritiesType.authoritiesGMU.financeSupply;
                                    founder.manageProperty = authoritiesType.authoritiesGMU.manageProperty;
                                }))
                    .Do(founder => founder.RefPassport = passport)
                    .Do(passport.Founders.Add));
        }

        private void ProcessPassportActivity(F_Org_Passport passport, IEnumerable<institutionActivityType> pumpDataActivity)
        {
            List<string> activity1 =
                passport
                    .Unless(orgPassport => orgPassport.ID == 0)
                    .With(orgPassport => orgPassport.Activity.Select(okvedy => okvedy.Name).ToList());

            pumpDataActivity.Each(
                type => passport
                            .Unless(orgPassport => activity1.Return(list => list.Contains(type.name), false))
                            .With(
                                arg => new F_F_OKVEDY
                                           {
                                               Name = type.name,
                                               RefOKVED = CommonPump.GetActualOKVED(passport.RefParametr, type.okved.code),
                                               RefPrOkved = type.okved.type.Return(s => s, "C").With(
                                                   s =>
                                                       {
                                                           switch (s)
                                                           {
                                                               case "C":
                                                                   return
                                                                       activityTypeCache.First(
                                                                           okved =>
                                                                           okved.Name.Equals("Основная деятельность"));
                                                               default:
                                                                   return
                                                                       activityTypeCache.First(
                                                                           okved =>
                                                                           okved.Name.Equals("Иная деятельность"));
                                                           }
                                                       }),
                                               RefPassport = passport
                                           })
                            .Do(passport.Activity.Add));
        }

        private void ProcessPassportBranches(F_Org_Passport passport, IEnumerable<institutionInfoType.additionalLocalType.branchLocalType> pumpDataBranch)
        {
            List<string> branches =
                passport
                    .Unless(orgPassport => orgPassport.ID == 0)
                    .With(
                        orgPassport => orgPassport.Branches
                                           .Join(
                                               Resolver.Get<ILinqRepository<D_Org_NsiOGS>>().FindAll(),
                                               filial => new
                                                   {
                                                       inn = filial.INN,
                                                       kpp = filial.KPP
                                                   },
                                               ogs => new
                                                   {
                                                       ogs.inn,
                                                       ogs.kpp
                                                   },
                                               (filial, ogs) => ogs.regNum)
                                           .ToList());

            pumpDataBranch.Each(
                type => passport
                            .Unless(orgPassport => branches.Return(list => list.Contains(type.regNum), false))
                            .If(orgPassport => _orgStructuresByRegnumCache.ContainsKey(type.regNum))
                            .With(orgPassport => _orgStructuresByRegnumCache[type.regNum])
                            .With(
                                structure =>
                                new F_F_Filial
                                    {
                                        INN = structure.INN,
                                        KPP = structure.KPP,
                                        Name = type.fullName,
                                        Nameshot = type.shortName,
                                        RefTipFi = type.type.With(
                                            s =>
                                                {
                                                    switch (s)
                                                    {
                                                        case "01":
                                                            return branchTypesCache.First(
                                                                fil => fil.Name.Equals("Филиал"));
                                                        case "02":
                                                            return branchTypesCache.First(
                                                                fil => fil.Name.Equals(
                                                                    "Обособленное структурное подразделение"));
                                                        case "03":
                                                            return branchTypesCache.First(
                                                                fil => fil.Name.Equals("Представительство"));
                                                        default:
                                                            return null;
                                                    }
                                                }),
                                        RefPassport = passport
                                    })
                            .If(filial => filial.ID == 0)
                            .Do(passport.Branches.Add));
        }

        private void BeforeProcessInstitutionInfoPumpFiles()
        {
            instititionTypeCache = Resolver.Get<ILinqRepository<D_Org_VidOrg>>().FindAll().ToList();
            oktmoCache = Resolver.Get<ILinqRepository<D_OKTMO_OKTMO>>().FindAll().ToList();
            _okatoCache = BuildOkatoCache();
            okopfCache = Resolver.Get<ILinqRepository<D_OKOPF_OKOPF>>().FindAll().ToList();
            okfsCache = Resolver.Get<ILinqRepository<D_OKFS_OKFS>>().FindAll().ToList();
            founderByNameCache = Resolver.Get<ILinqRepository<D_Org_OrgYchr>>().FindAll()
                .ToDictionary(ychr => ychr.Name, ychr => ychr);
            categoriesCache = Resolver.Get<ILinqRepository<D_Org_Category>>().FindAll().ToList();
            branchTypesCache = Resolver.Get<ILinqRepository<D_Org_TipFil>>().FindAll().ToList();
            activityTypeCache = Resolver.Get<ILinqRepository<D_Org_PrOKVED>>().FindAll().ToList();
        }
    }
}