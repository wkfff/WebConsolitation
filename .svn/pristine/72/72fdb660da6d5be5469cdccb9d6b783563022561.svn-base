using System;
using System.Collections.Generic;
using System.Linq;

using bus.gov.ru.external.Item1;
using bus.gov.ru.types.Item1;

using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;
using GlobalConsts = Krista.FM.RIA.Extensions.E86N.Utils.GlobalConsts;

namespace Krista.FM.RIA.Extensions.E86N.Services.Export
{
    public static class ExportDiverseInfoService
    {
        private static D_Org_UserProfile placerProfile;

        private static D_Org_Structure target;
        
        public static byte[] Serialize(IAuthService authService, F_F_ParameterDoc header)
        {
            target = header.RefUchr;
            placerProfile = authService.Profile;
            if (placerProfile == null)
            {
                throw new InvalidOperationException(GlobalConsts.NullProfile);
            }

            return ExportServiceHelper.Serialize(
                new diverseInfo
                {
                    header = ExportServiceHelper.HeaderType(),
                    body = new diverseInfo.bodyLocalType { position = Position(header) }
                }.Save);    
        }

        private static diverseInfoType Position(F_F_ParameterDoc header)
        {
            return new diverseInfoType
            {
                positionId = Guid.NewGuid().ToString(),
                changeDate = DateTime.Now,
                placer = ExportServiceHelper.RefNsiOgsExtendedType(placerProfile.RefUchr),
                initiator = target.ID != placerProfile.RefUchr.ID
                                ? ExportServiceHelper.RefNsiOgsExtendedType(target)
                                : null,
                
                main = new diverseInfoType.mainLocalType
                    {
                        orgList = header.TofkList.Select(x => new orgListType
                                                                {
                                                                    orgName = x.TofkName,
                                                                    orgAddress = x.TofkAddress
                                                                }).ToList()
                    },

                additional = new List<diverseInfoType.additionalLocalType>
                    {
                      new diverseInfoType.additionalLocalType
                        {
                            paymentDetails = header.PaymentDetails.Select(x =>
                                                                          {
                                                                              var t = new diverseInfoType.additionalLocalType.paymentDetailsLocalType
                                                                                  {
                                                                                      paymentDetailsType = x.RefPaymentDetailsType.Code,
                                                                                      bankName = x.BankName,
                                                                                      bik = x.Bik,
                                                                                      calcAccountCode = x.CalcAccountCode,
                                                                                      accountDetails = new diverseInfoType.additionalLocalType.paymentDetailsLocalType.accountDetailsLocalType(),
                                                                                      paymentDetailsTargets =
                                                                                          x.PaymentDetailsTargets.Select(
                                                                                              pt =>
                                                                                              new diverseInfoType.additionalLocalType.paymentDetailsLocalType.paymentDetailsTargetsLocalType
                                                                                                  {
                                                                                                      kbk = pt.Kbk,
                                                                                                      paymentType = pt.PaymentType,
                                                                                                      paymentTargetName = pt.PaymentTargetName
                                                                                                  }).ToList()
                                                                                  };

                                                                              if (!x.TofkName.Equals(string.Empty))
                                                                              {
                                                                                 /* t.tofkName = x.TofkName;*/
                                                                              }

                                                                              if (!x.BankCity.Equals(string.Empty))
                                                                              {
                                                                                  t.bankCity = x.BankCity;
                                                                              }

                                                                              if (!x.CorAccountCode.Equals(string.Empty))
                                                                              {
                                                                                    if (t.paymentDetailsType.Equals(FX_FX_PaymentDetailsType.BankAccount))
                                                                                    {
                                                                                        if (t.accountDetails.bankAccount == null)
                                                                                        {
                                                                                            t.accountDetails.bankAccount = new diverseInfoType.additionalLocalType.paymentDetailsLocalType.accountDetailsLocalType.bankAccountLocalType();
                                                                                        }

                                                                                        t.accountDetails.bankAccount.corAccountCode = x.CorAccountCode;
                                                                                    }
                                                                              }

                                                                              if (!x.PersonalAccountCode.Equals(string.Empty))
                                                                              {
                                                                                  switch (t.paymentDetailsType)
                                                                                  {
                                                                                      case FX_FX_PaymentDetailsType.BankAccount:
                                                                                          if (t.accountDetails.bankAccount == null)
                                                                                          {
                                                                                              t.accountDetails.bankAccount = new diverseInfoType.additionalLocalType.paymentDetailsLocalType.accountDetailsLocalType.bankAccountLocalType();
                                                                                          }

                                                                                          t.accountDetails.bankAccount.personalAccountCode = x.PersonalAccountCode;
                                                                                          break;
                                                                                      case FX_FX_PaymentDetailsType.OrfkAccount:
                                                                                          if (t.accountDetails.orfkAccount == null)
                                                                                          {
                                                                                              t.accountDetails.orfkAccount = new diverseInfoType.additionalLocalType.paymentDetailsLocalType.accountDetailsLocalType.orfkAccountLocalType();
                                                                                          }

                                                                                          t.accountDetails.orfkAccount.personalAccountCode = x.PersonalAccountCode;
                                                                                          break;
                                                                                      case FX_FX_PaymentDetailsType.FoAccount:
                                                                                          if (t.accountDetails.foAccount == null)
                                                                                          {
                                                                                              t.accountDetails.foAccount = new diverseInfoType.additionalLocalType.paymentDetailsLocalType.accountDetailsLocalType.foAccountLocalType();
                                                                                          }

                                                                                          t.accountDetails.foAccount.personalAccountCode = x.PersonalAccountCode;
                                                                                          break;
                                                                                  }
                                                                              }

                                                                              if (x.AccountName.IsNotNullOrEmpty())
                                                                              {
                                                                                  switch (t.paymentDetailsType)
                                                                                  {
                                                                                      case FX_FX_PaymentDetailsType.OrfkAccount:
                                                                                          if (t.accountDetails.orfkAccount == null)
                                                                                          {
                                                                                              t.accountDetails.orfkAccount = new diverseInfoType.additionalLocalType.paymentDetailsLocalType.accountDetailsLocalType.orfkAccountLocalType();
                                                                                          }

                                                                                          t.accountDetails.orfkAccount.orfkName = x.AccountName;
                                                                                          break;
                                                                                      case FX_FX_PaymentDetailsType.FoAccount:
                                                                                          if (t.accountDetails.foAccount == null)
                                                                                          {
                                                                                              t.accountDetails.foAccount = new diverseInfoType.additionalLocalType.paymentDetailsLocalType.accountDetailsLocalType.foAccountLocalType();
                                                                                          }

                                                                                          t.accountDetails.foAccount.foName = x.AccountName;
                                                                                          break;
                                                                                  }
                                                                              }

                                                                              return t;
                                                                          }).ToList(),

                            licenseDetails = header.LicenseDetails.Select(x => new diverseInfoType.additionalLocalType.licenseDetailsLocalType
                                {
                                   licenseAgencyName = x.LicenseAgencyName,
                                   licenseDate = x.LicenseDate,
                                   licenseName = x.LicenseName,
                                   licenseNum = x.LicenseNum,
                                   licenseExpDate = x.LicenseExpDate
                                }).ToList(),

                            accreditationDetails = header.AccreditationDetails.Select(x => new diverseInfoType.additionalLocalType.accreditationDetailsLocalType
                                {
                                    accreditationAgencyName = x.AccreditationAgencyName,
                                    accreditationName = x.AccreditationName,
                                    accreditationExpDate = x.AccreditationExpDate
                                }).ToList()
                        }
                    } 
            };
        }
    }
}
