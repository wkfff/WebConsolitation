using System;
using System.Collections.Generic;
using System.Linq;
using bus.gov.ru.external.Item1;
using bus.gov.ru.types.Item1;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Xml.Schema.Linq;

namespace Krista.FM.RIA.Extensions.E86N.Services.Pump
{
    public static class InstitutionInfo2Passport
    {
        public static F_F_ParameterDoc Pump(XTypedElement pumpXml)
        {
            var categoriesRepository = Resolver.Get<ILinqRepository<D_Org_Category>>();
            
            var pumpHeader = institutionInfo.Parse(pumpXml.Untyped.ToString()).header;
            institutionInfoType pumpData = institutionInfo.Parse(pumpXml.Untyped.ToString()).body.position;
            refNsiConsRegExtendedStrongType targetOrg = pumpData.initiator ?? pumpData.placer;

            var header = Helpers.Header(targetOrg, FX_FX_PartDoc.PassportDocTypeID, pumpData.changeDate);
            
            var passport =
                new F_Org_Passport
                    {
                        OGRN = pumpData.main.With(type => type.ogrn) ?? string.Empty,
                        Fam = pumpData.other.With(type => type.chief).FirstOrDefault().With(type => type.lastName) ??
                              string.Empty,
                        NameRuc = pumpData.other.With(type => type.chief).FirstOrDefault().With(type => type.firstName) ??
                                  string.Empty,
                        Otch = pumpData.other.With(type => type.chief).FirstOrDefault().With(type => type.middleName) ??
                               string.Empty,
                        Ordinary = pumpData.other.With(type => type.chief).FirstOrDefault().With(type => type.position) ??
                                   string.Empty,
                        Adr = pumpData.main
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
                              string.Empty,
                        Indeks = pumpData.main.With(type => type.complexAddress.address.zip) ?? string.Empty,
                        Website = pumpData.additional.www ?? string.Empty,
                        OKPO = pumpData.main.With(type => type.classifier.okpo) ?? string.Empty,
                        Phone = pumpData.additional.phone,
                        Mail = pumpData.additional.eMail ?? string.Empty,
                        RefRaspor =
                            pumpData.main.With(type => type.rbs).With(
                                type =>
                                Resolver.Get<ILinqRepository<D_OrgGen_Raspor>>().FindAll()
                                    .SingleOrDefault(raspor => raspor.Name.Equals(type.fullName)) ?? 
                                        new D_OrgGen_Raspor
                                        {
                                            Name = type.fullName, Code = string.Empty
                                        }),
                        RefOKOPF =
                            pumpData.main.With(
                                type =>
                                Resolver.Get<ILinqRepository<D_OKOPF_OKOPF>>().FindAll().FirstOrDefault(
                                    okopf => okopf.Code5Zn.Equals(type.classifier.okopf.code))),
                        RefOKATO =
                            pumpData.With(
                                type => Resolver.Get<ILinqRepository<D_OKATO_OKATO>>().FindAll()
                                            .FirstOrDefault(
                                                okato => okato.Code.Equals(type.additional.okato.code))),
                        RefOKFS =
                            pumpData.main.With(
                                type => Resolver.Get<ILinqRepository<D_OKFS_OKFS>>().FindAll()
                                            .First(
                                                okfs =>
                                                okfs.Code == Convert.ToInt32(type.classifier.okfs.code))),
                        RefOKTMO =
                            Resolver.Get<ILinqRepository<D_OKTMO_OKTMO>>().FindAll().FirstOrDefault(
                                oktmo => oktmo.Code.Equals(pumpData.main.classifier.oktmo.code)),
                        RefVid =
                            Resolver.Get<ILinqRepository<D_Org_VidOrg>>().FindAll().First(
                                org => org.Code.Equals(pumpData.additional.institutionType.code)),
                        RefCateg =
                            pumpData.additional.branch.With(
                                _ => categoriesRepository.Load(D_Org_Category.FX_FX_HEAD_OFFICE)) ??
                            pumpData.additional.branchInfo.With(
                                type =>
                                    {
                                        switch (type.type)
                                        {
                                            case "01":
                                                return categoriesRepository.Load(D_Org_Category.FX_FX_BRANCH);
                                            case "02":
                                                return categoriesRepository.Load(D_Org_Category.FX_FX_SEPARATE_UNIT);
                                            case "03":
                                                return categoriesRepository.Load(D_Org_Category.FX_FX_AGENCY);
                                            default:
                                                return null;
                                        }
                                    }) ??
                            categoriesRepository.Load(D_Org_Category.FX_FX_NOTHING),
                        RefParametr = header
                    };

            // обрабатываем учредителей
            pumpData.other
                .With(type => type.founder)
                .Do(founder => ProcessPassportFounder(passport, founder));

            ProcessPassportFounderAuthority(
                passport,
                pumpData.additional.enactment.With(type => type.founderAuthority));

            // обрабатываем виды деятельности
            ProcessPassportActivity(passport, pumpData.additional.activity);

            // обрабатываем филиалы
            pumpData.additional.branch
                .If(list => list.Any())
                .Do(list => ProcessPassportBranches(passport, list));

            header.Passports.Add(passport);
            
            // обрабатываем документы-основания
            Helpers.ProcessDocumentsHeader(
                header,
                pumpHeader,
                pumpData.document.Where(type => type.type != null).Cast<documentType>(),
                type => type.With(x => (institutionInfoType.documentLocalType)x)
                            .With(
                                x =>
                                Resolver.Get<ILinqRepository<D_Doc_TypeDoc>>().FindAll()
                                    .Single(doc => doc.Code.Equals(x.type == "C" ? "X" : x.type))));

            return header;
        }

        private static void ProcessPassportFounderAuthority(F_Org_Passport passport, refNsiConsRegSoftType pumpDataFounderAuthority)
        {
            pumpDataFounderAuthority
                .With(type => Resolver.Get<ILinqRepository<D_Org_OrgYchr>>().FindAll().SingleOrDefault(ychr => ychr.Name.Equals(type.fullName)))
                .Do(
                    ychr => passport.Founders.Add(
                        new F_F_Founder
                            {
                                RefYchred = ychr,
                                RefPassport = passport
                            }));
        }

        private static void ProcessPassportFounder(
                        F_Org_Passport passport,
                        IEnumerable<institutionInfoType.otherLocalType.founderLocalType> pumpDataFounder)
        {
            pumpDataFounder
                .Select(
                    type =>
                    Resolver.Get<ILinqRepository<D_Org_OrgYchr>>().FindAll()
                        .SingleOrDefault(ychr => ychr.Name.Equals(type.fullName))
                        .With(
                            ychr => new F_F_Founder
                                        {
                                            RefYchred = ychr,
                                            RefPassport = passport
                                        })
                        .Do(
                            founder => type.authorities.Do(
                                authoritiesType =>
                                    {
                                        founder.formative = authoritiesType.authoritiesGMU.formative;
                                        founder.stateTask = authoritiesType.authoritiesGMU.stateTask;
                                        founder.supervisoryBoard = authoritiesType.authoritiesGMU.supervisoryBoard;
                                        founder.manageProperty = authoritiesType.authoritiesGMU.manageProperty;
                                        founder.financeSupply = authoritiesType.authoritiesGMU.financeSupply;
                                    })))
                .Each(passport.Founders.Add);
        }

        private static void ProcessPassportActivity(
                                                    F_Org_Passport passport,
                                                    IEnumerable<institutionActivityType> pumpDataActivity)
        {
            pumpDataActivity
                .Select(
                    type =>
                    new F_F_OKVEDY
                        {
                            Name = type.name,
                            RefPassport = passport,
                            RefOKVED =
                                Resolver.Get<ILinqRepository<D_OKVED_OKVED>>().FindAll()
                                .First(okved => okved.Code.Equals(type.okved.code)),
                            RefPrOkved =
                                Resolver.Get<ILinqRepository<D_Org_PrOKVED>>().Load(
                                    type.okved.type.Equals("O")
                                        ? D_Org_PrOKVED.OtherID
                                        : D_Org_PrOKVED.MainID)
                        })
                .Each(passport.Activity.Add);
        }

        private static void ProcessPassportBranches(
                                F_Org_Passport passport,
                                IEnumerable<institutionInfoType.additionalLocalType.branchLocalType> pumpDataBranch)
        {
            pumpDataBranch
                .Select(
                    type => Helpers.GetOrgStructureByRegnum(type.regNum).With(
                        structure =>
                        new F_F_Filial
                            {
                                INN = structure.INN,
                                KPP = structure.KPP,
                                Name = type.fullName,
                                Nameshot = type.shortName,
                                RefPassport = passport,
                                RefTipFi =
                                    Resolver.Get<ILinqRepository<D_Org_TipFil>>().Load(DOrgTipFilIDByType(type.type))
                            }))
                .Each(passport.Branches.Add);
        }

        private static int DOrgTipFilIDByType(string type)
        {
            if (type.Equals("01"))
            {
                return D_Org_TipFil.Branch;
            }

            if (type.Equals("02"))
            {
                return D_Org_TipFil.SeparateUnit;
            }

            if (type.Equals("03"))
            {
                return D_Org_TipFil.Agency;
            }

            throw new NotImplementedException();
        }
    }
}
