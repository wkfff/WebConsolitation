using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml;

using Bus.Gov.Ru.Imports;

using bus.gov.ru.types.Item1;

using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.E86N.Services.DocService;

namespace Krista.FM.RIA.Extensions.E86N.Services.Export
{
    public static class ExportServiceHelper
    {
        public static List<documentType> Documents(IEnumerable<F_Doc_Docum> documents)
        {
            return documents
                .Where(p => File.Exists(DocHelpers.GetFullFilePath(p)))
                .Select(
                    p => new documentType
                             {
                                 name = p.Name + Path.GetExtension(DocHelpers.GetFullFilePath(p)) + ".zip",
                                 date = DocDateTime(p),
                                 content = GetFileContent(DocHelpers.GetFullFilePath(p))
                             })
                .ToList();
        }

        public static DateTime DocDateTime(F_Doc_Docum p)
        {
            if (p.DocDate != null)
            {
                return (DateTime)p.DocDate;
            }

            // todo: ситуация с нулевой датой исключительная
            throw new NotImplementedException();
        }

        public static byte[] GetFileContent(string filename)
        {
            using (var content = new MemoryStream())
            {
                using (var sourceStream = new FileStream(filename, FileMode.Open, FileAccess.Read))
                using (var zipStream = new GZipStream(content, CompressionMode.Compress))
                {
                    sourceStream.CopyTo(zipStream);
                }

                return content.ToArray();
            }
        }

        public static headerType HeaderType()
        {
            return
                new headerType
                    {
                        createDateTime = DateTime.Now,
                        id = Guid.NewGuid().ToString()
                    };
        }

        public static refNsiConsRegExtendedStrongType RefNsiOgsExtendedType(D_Org_Structure target)
        {
            return
                new refNsiConsRegExtendedStrongType
                    {
                        fullName = target.Name,
                        inn = Convert.ToInt64(target.INN),
                        kpp = Convert.ToInt64(target.KPP),
                        regNum = RegNum(target.INN, target.KPP)
                    };
        }

        public static string RegNum(string inn, string kpp)
        {
            var nsiOgs = CommonPump.GetOGSDictionary();
            try
            {
                return nsiOgs.Single(p => inn.Equals(p.Value.INN) && kpp.Equals(p.Value.KPP)).Key;
            }
            catch
            {   
                throw new Exception("Не найден регистрационный номер в ОГС для ИНН: {0} и КПП: {1}".FormatWith(inn, kpp));
            }
        }

        public static byte[] Serialize(Action<XmlWriter> documentSaver)
        {
            using (var resultStream = new MemoryStream())
            {
                using (var xmlWriter = new XmlTextWriter(resultStream, Encoding.UTF8))
                {
                    xmlWriter.Formatting = Formatting.Indented;
                    xmlWriter.Indentation = 2;

                    documentSaver(xmlWriter);
                }

                return resultStream.ToArray();
            }
        }

        public static F_Org_Passport GetLastPassport(int uchrID)
        {
            var passports = Resolver.Get<ILinqRepository<F_Org_Passport>>().FindAll()
                                                        .Where(
                                                                x => (x.RefParametr.RefUchr.ID == uchrID)
                                                                 && (x.RefParametr.RefPartDoc.ID == FX_FX_PartDoc.PassportDocTypeID));
            var passportsID = passports.Max(x => x.ID);
            try
            {
                return passports.Single(x => x.ID == passportsID);
            }
            catch
            {
                throw new InvalidOperationException("Должна быть опубликована общая информация об учреждении");
            }
        }

        public static annualBalanceBudgetGeneralDataType AnnualBalanceBudgetGeneralDataType(F_F_ParameterDoc header)
        {
            D_Org_Structure uchr = header.RefUchr;
            F_Report_HeadAttribute reportHeadAttribute = header.ReportHeadAttribute.First();

            F_Org_Passport passport = GetLastPassport(uchr.ID);
            
            var result = new annualBalanceBudgetGeneralDataType
                    {
                        date = reportHeadAttribute.Datedata,
                        periodicity = reportHeadAttribute.RefPeriodic.NameEng,
                        okei = new refNsiOkeiType
                                   {
                                       code = "383",
                                       symbol = "руб"
                                   },
                        okpo = passport.OKPO,
                        okato = new refNsiOkatoType
                                    {
                                        code = passport.RefOKATO.Code
                                    },
                        section = string.Format("{0,3}", uchr.RefOrgGRBS.Code.PadLeft(3, '0'))
                    };

            // todo бывает что бюджетов для ппо нет!!!
            result.budget = new refNsiBudgetStrongType { code = Resolver.Get<ILinqRepository<D_Fin_nsiBudget>>().FindAll().Single(x => x.RefOrgPPO.ID == uchr.RefOrgPPO.ID).Code };

            result.grbs = new refNsiConsRegSoftType { regNum = RegNum(uchr.INN, uchr.KPP), fullName = uchr.Name };
            
            return result;
        }

        public static annualBalanceBudgetGeneralDataTypeCommon2014 AnnualBalanceBudgetGeneralDataTypeCommon2014(F_F_ParameterDoc header)
        {
            D_Org_Structure uchr = header.RefUchr;
            F_Report_HeadAttribute reportHeadAttribute = header.ReportHeadAttribute.First();

            F_Org_Passport passport = GetLastPassport(uchr.ID);

            var result = new annualBalanceBudgetGeneralDataTypeCommon2014
                             {
                                 date = reportHeadAttribute.Datedata,
                                 periodicity = reportHeadAttribute.RefPeriodic.NameEng,
                                 okei = new refNsiOkeiType
                                            {
                                                code = "383",
                                                symbol = "руб"
                                            },
                                 okpo = passport.OKPO,
                                 oktmo = new refNsiOktmoType
                                             {
                                                 code = passport.RefOKTMO.Code
                                             },
                                 section = string.Format("{0,3}", uchr.RefOrgGRBS.Code.PadLeft(3, '0'))
                             };

            // todo бывает что бюджетов для ппо нет!!!
            result.budget = new refNsiBudgetStrongType { code = Resolver.Get<ILinqRepository<D_Fin_nsiBudget>>().FindAll().Single(x => x.RefOrgPPO.ID == uchr.RefOrgPPO.ID).Code };

            result.grbs = new refNsiConsRegSoftType { regNum = RegNum(uchr.INN, uchr.KPP), fullName = uchr.Name };

            return result;
        }

        public static annualBalanceBudgetGeneralDataType2014 AnnualBalanceBudgetGeneralDataType2014(F_F_ParameterDoc header)
        {
            D_Org_Structure uchr = header.RefUchr;
            F_Report_HeadAttribute reportHeadAttribute = header.ReportHeadAttribute.First();

            F_Org_Passport passport = GetLastPassport(uchr.ID);

            var result = new annualBalanceBudgetGeneralDataType2014
                             {
                                 date = reportHeadAttribute.Datedata,
                                 periodicity = reportHeadAttribute.RefPeriodic.NameEng,
                                 okei = new refNsiOkeiType
                                            {
                                                code = "383",
                                                symbol = "руб"
                                            },
                                 okpo = passport.OKPO,
                                 oktmo = new refNsiOktmoType
                                             {
                                                 code = passport.RefOKTMO.Code
                                             },
                                 inn = uchr.INN,
                                 section = string.Format("{0,3}", uchr.RefOrgGRBS.Code.PadLeft(3, '0'))
                             };

            // todo бывает что бюджетов для ппо нет!!!
            result.budget = new refNsiBudgetStrongType { code = Resolver.Get<ILinqRepository<D_Fin_nsiBudget>>().FindAll().Single(x => x.RefOrgPPO.ID == uchr.RefOrgPPO.ID).Code };

            result.grbs = new refNsiConsRegSoftType { regNum = RegNum(uchr.INN, uchr.KPP), fullName = uchr.Name };

            return result;
        }

        public static annualBalanceFounderDataType AnnualBalanceFounderDataType(F_F_ParameterDoc header)
        {
            F_F_Founder founder;
            string regNum, fullName;
            
            D_Org_Structure uchr = header.RefUchr;
            F_Report_HeadAttribute reportHeadAttribute = header.ReportHeadAttribute.First();

            F_Org_Passport passport = GetLastPassport(uchr.ID);

            try
            {
                founder = Resolver.Get<ILinqRepository<F_F_Founder>>().FindAll().First(x => x.RefPassport.ID == passport.ID);
            }
            catch
            {
                throw new InvalidOperationException("Отсутствует учредитель");
            }

            try
            {
                regNum = founder.RefYchred.RefNsiOgs.regNum;
                fullName = founder.RefYchred.RefNsiOgs.FullName;
            }
            catch
            {
                throw new NullReferenceException("Отсутствует ОГС для учредителя с кодом {0}".FormatWith(founder.RefYchred.Code));
            }

            return new annualBalanceFounderDataType
                    {
                        date = reportHeadAttribute.Datedata,
                        periodicity = reportHeadAttribute.RefPeriodic.NameEng,
                        okei = new refNsiOkeiType
                                   {
                                       code = "383",
                                       symbol = "руб"
                                   },
                        okpo = passport.OKPO,
                        okato = new refNsiOkatoType
                                    {
                                        code = passport.RefOKATO.Code
                                    },
                        section = string.Format("{0,3}", uchr.RefOrgGRBS.Code.PadLeft(3, '0')),
                        founderName = founder.RefYchred.Name,
                        founderAuthority = new refNsiConsRegSoftType
                                               {
                                                   regNum = regNum,
                                                   fullName = fullName
                                               }
                    };
        }

        public static annualBalanceFounderDataType_2014 AnnualBalanceFounderDataType2014(F_F_ParameterDoc header)
        {
            F_F_Founder founder;
            string regNum, fullName;

            D_Org_Structure uchr = header.RefUchr;
            F_Report_HeadAttribute reportHeadAttribute = header.ReportHeadAttribute.First();

            F_Org_Passport passport = GetLastPassport(uchr.ID);

            try
            {
                founder = Resolver.Get<ILinqRepository<F_F_Founder>>().FindAll().First(x => x.RefPassport.ID == passport.ID);
            }
            catch
            {
                throw new InvalidOperationException("Отсутствует учредитель");
            }

            try
            {
                regNum = founder.RefYchred.RefNsiOgs.regNum;
                fullName = founder.RefYchred.RefNsiOgs.FullName;
            }
            catch
            {
                throw new NullReferenceException("Отсутствует ОГС для учредителя с кодом {0}".FormatWith(founder.RefYchred.Code));
            }

            return new annualBalanceFounderDataType_2014
                       {
                           date = reportHeadAttribute.Datedata,
                           periodicity = reportHeadAttribute.RefPeriodic.NameEng,
                           okei = new refNsiOkeiType
                                      {
                                          code = "383",
                                          symbol = "руб"
                                      },
                           okpo = passport.OKPO,
                           inn = uchr.INN,
                           oktmo = new refNsiOktmoType
                                       {
                                           code = passport.RefOKTMO.Code
                                       },
                           section = string.Format("{0,3}", uchr.RefOrgGRBS.Code.PadLeft(3, '0')),
                           founderName = founder.RefYchred.Name,
                           founderAuthority = new refNsiConsRegSoftType
                                                  {
                                                      regNum = regNum,
                                                      fullName = fullName
                                                  }
                       };
        }
    }
}
