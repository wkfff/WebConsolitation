using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using bus.gov.ru.external.Item1;
using bus.gov.ru.types.Item1;

using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;
using Krista.FM.RIA.Extensions.E86N.Services.DocService;
using Krista.FM.ServerLibrary;

using GlobalConsts = Krista.FM.RIA.Extensions.E86N.Utils.GlobalConsts;

namespace Krista.FM.RIA.Extensions.E86N.Services.Export
{
    public static class ExportPassportService
    {
        public static byte[] Serialize(IAuthService authService, F_F_ParameterDoc header)
        {
            var regionCode = Resolver.Get<IScheme>().GlobalConstsManager.Consts["OKTMO"].Value.ToString().Replace(" ", string.Empty) + "000";
            F_Org_Passport passport = header.Passports.First();
            D_Org_Structure target = header.RefUchr;
            D_Org_UserProfile placerProfile = authService.Profile;
            if (placerProfile == null)
            {
                throw new InvalidOperationException(GlobalConsts.NullProfile);
            }

            List<F_F_OKVEDY> activity = passport.Activity.ToList();
            List<F_F_Filial> branch = passport.Branches.ToList();
            List<F_Doc_Docum> documents = header.Documents.ToList();
            List<F_F_Founder> founders = passport.Founders.ToList();

            var infoType = new institutionInfoType
                               {
                                   positionId = Guid.NewGuid().ToString(),
                                   changeDate = DateTime.Now,
                                   placer = ExportServiceHelper.RefNsiOgsExtendedType(placerProfile.RefUchr),
                                   initiator = target.ID != placerProfile.RefUchr.ID ? ExportServiceHelper.RefNsiOgsExtendedType(target) : null,
                                   main = new institutionInfoType.mainLocalType
                                            {
                                                  shortName = target.ShortName,
                                                  ogrn = passport.OGRN,
                                                  orgType = target.RefTipYc.ID.ToString("00"),
                                                  special = target.RefTipYc.ID.Equals(FX_Org_TipYch.GovernmentID) ? "04" : "05",
                                                  complexAddress = new institutionInfoType.mainLocalType.complexAddressLocalType
                                                                       {
                                                                           address = new addressStrongType
                                                                                         {
                                                                                             zip = passport.Indeks,
                                                                                             subject = new refNsiKladrStrongType
                                                                                                           {
                                                                                                               code = regionCode
                                                                                                           }
                                                                                         }
                                                                       },
                                                  classifier = new institutionClassifierType
                                                                   {
                                                                       okfs = new refNsiOkfsType
                                                                                  {
                                                                                      code = passport.RefOKFS.Code.ToString("00"),
                                                                                      name = passport.RefOKFS.Name
                                                                                  },
                                                                       okopf = new refNsiOkopfType
                                                                       {
                                                                           code = passport.RefOKOPF.Code.GetValueOrDefault().ToString("00000"),
                                                                           name = passport.RefOKOPF.Name
                                                                       },
                                                                       okpo = passport.OKPO,
                                                                       okved = activity.Select(p => new refNsiOkvedType
                                                                               {
                                                                                   code = p.RefOKVED.Code,
                                                                                   name = p.RefOKVED.Name,
                                                                                   type = p.RefPrOkved.Code.Equals(D_Org_PrOKVED.MainID) ? "C" : "O"
                                                                               }).ToList()
                                                  }
                                            },
                                   additional =
                                       new institutionInfoType.additionalLocalType
                                           {
                                               institutionType = target.RefTipYc.ID.Equals(FX_Org_TipYch.BudgetaryID)
                                                                       || target.RefTipYc.ID.Equals(FX_Org_TipYch.GovernmentID)
                                                                       || target.RefTipYc.ID.Equals(FX_Org_TipYch.AutonomousID)
                                                                   ? new refNsiInstitutionTypeType
                                                                         {
                                                                             code = passport.RefVid.Code,
                                                                             name = passport.RefVid.Name
                                                                         }
                                                                   : null,
                                               ppo = new institutionInfoType.additionalLocalType.ppoLocalType
                                                       {
                                                           name = target.RefOrgPPO.Name
                                                       },
                                               enactment = target.RefTipYc.ID.Equals(FX_Org_TipYch.AutonomousID)
                                                                ? Enactment(documents.First(p => p.RefTypeDoc.Code.Equals("I") && FileExists(p)))
                                                                : null,
                                               okato = new refNsiOkatoType
                                                       {
                                                           code = passport.RefOKATO.Code,
                                                           name = passport.RefOKATO.Name
                                                       },
                                               branch = branch.Select(p => new institutionInfoType.additionalLocalType.branchLocalType
                                                           {
                                                               fullName = p.Name,
                                                               regNum = ExportServiceHelper.RegNum(p.INN, p.KPP),
                                                               type = BranchTypeType(p)
                                                           }).ToList(),
                                               activity = activity.Select(p => new institutionActivityType
                                                           {
                                                               name = p.Name,
                                                               okved = new refNsiOkvedType
                                                                           {
                                                                               name = p.RefOKVED.Name,
                                                                               code = p.RefOKVED.Code,
                                                                               type = p.RefPrOkved.ID.Equals(D_Org_PrOKVED.MainID) ? "C" : "O"
                                                                           }
                                                           }).ToList(),
                                               phone = passport.Phone,
                                               section = target.RefOrgGRBS.Code
                                           }
                                       .Do(type => target.If(structure => structure.RefTipYc.ID.Equals(FX_Org_TipYch.GovernmentID))
                                       .Do(structure => type.section = structure.RefOrgGRBS.Code))
                                       .Do(type => passport.If(orgPassport => orgPassport.Mail.IsNotNullOrEmpty()).Do(orgPassport => type.eMail = orgPassport.Mail))
                                       .Do(type => passport.If(orgPassport => orgPassport.Website.IsNotNullOrEmpty()).Do(orgPassport => type.www = orgPassport.Website)),
                                   other =
                                       new institutionInfoType.otherLocalType
                                           {
                                               chief =
                                                   new List<employeeType>
                                                       {
                                                           new employeeType
                                                               {
                                                                   position = passport.Ordinary,
                                                                   firstName = passport.NameRuc,
                                                                   lastName = passport.Fam
                                                               }.Do(type => passport.If(orgPassport => orgPassport.Otch.IsNotNullOrEmpty())
                                                                .Do(orgPassport => type.middleName = orgPassport.Otch))
                                                       },
                                               founder =
                                                   founders.Select(
                                                       founder =>
                                                       new institutionInfoType.otherLocalType.founderLocalType
                                                           {
                                                               fullName = founder.RefYchred.Name,
                                                               authorities =
                                                                   new founderAuthoritiesVSRIType
                                                                       {
                                                                           authoritiesGMU = new founderAuthoritiesVSRIType.authoritiesGMULocalType
                                                                                   {
                                                                                       formative = founder.formative,
                                                                                       stateTask = founder.stateTask,
                                                                                       supervisoryBoard = founder.supervisoryBoard ?? false,
                                                                                       financeSupply = founder.financeSupply,
                                                                                       manageProperty = founder.manageProperty
                                                                                   }
                                                                       }
                                                           }.Do(type => founder.RefYchred.RefNsiOgs.Do(ogs => type.regNum = ogs.regNum)))
                                                   .ToList()
                                           },
                                   document = documents.Where(p => File.Exists(DocHelpers.GetFullFilePath(p)))
                                       .Select(p => new institutionInfoType.documentLocalType
                                                       {
                                                           name = p.Name + Path.GetExtension(DocHelpers.GetFullFilePath(p)) + ".zip",
                                                           date = ExportServiceHelper.DocDateTime(p),
                                                           content = ExportServiceHelper.GetFileContent(DocHelpers.GetFullFilePath(p)),
                                                           type = p.RefTypeDoc.Code.Equals("X") ? "C" : p.RefTypeDoc.Code
                                                       }).ToList()
                               };

            return ExportServiceHelper.Serialize(
                new institutionInfo
                    {
                        header = ExportServiceHelper.HeaderType(),
                        body = new institutionInfo.bodyLocalType
                                   {
                                       position = infoType
                                   }
                    }.Save);
        }

        private static string BranchTypeType(F_F_Filial branch)
        {
            if (branch.RefTipFi.ID.Equals(D_Org_TipFil.Branch))
            {
                return "01";
            }

            if (branch.RefTipFi.ID.Equals(D_Org_TipFil.SeparateUnit))
            {
                return "02";
            }

            if (branch.RefTipFi.ID.Equals(D_Org_TipFil.Agency))
            {
                return "03";
            }

            throw new NotImplementedException();
        }

        private static bool FileExists(F_Doc_Docum doc)
        {
            var path = DocHelpers.GetFullFilePath(doc);
            if (path.IsNullOrEmpty())
            {
                throw new FileNotFoundException("Прикрепленный фалйл {0} не найден".FormatWith(doc.Url));
            }

            return true;
        }

        private static institutionEnactmentVSRIType Enactment(F_Doc_Docum enactment)
        {
            return
                new institutionEnactmentVSRIType
                {
                    type = enactment.RefTypeDoc.Name,
                    name = enactment.Name,
                    number = enactment.NumberNPA,
                    date = ExportServiceHelper.DocDateTime(enactment)
                };
        }
    }
}
