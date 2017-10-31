using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using bus.gov.ru.external.Item1;
using bus.gov.ru.types.Item1;

using Krista.FM.Domain;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;

using GlobalConsts = Krista.FM.RIA.Extensions.E86N.Utils.GlobalConsts;

namespace Krista.FM.RIA.Extensions.E86N.Services.Export
{
    public static class ExportActionGrantService
    {
        public static byte[] Serialize(F_F_ParameterDoc header)
        {
            var authService = Resolver.Get<IAuthService>();

            D_Org_Structure target = header.RefUchr;
            D_Org_UserProfile placerProfile = authService.Profile;
            if (placerProfile == null)
            {
                throw new InvalidOperationException(GlobalConsts.NullProfile);
            }

            List<F_Fin_CapFunds> capFunds = header.CapitalConstructionFundses.ToList();
            List<F_Fin_realAssFunds> realAssFunds = header.RealAssFundses.ToList();
            List<F_Doc_Docum> documents = header.Documents.Where(docum => docum.RefTypeDoc.Code.Equals("A")).ToList();

            var othGrantFunds = header.OtherGrantFundses
                .GroupBy(x => new { x.RefOtherGrant.Code, x.RefOtherGrant.Name })
                .Select(
                    group => new
                                 {
                                     code = group.Key.Code,
                                     name = group.Key.Name,
                                     funds = group.Sum(x => x.funds)
                                 })
                .ToList();

            if (!(capFunds.Any() || realAssFunds.Any() || othGrantFunds.Any()))
            {
                throw new InvalidDataException("Отсутствуют данные для ActionGrant");
            }

            int year = header.RefYearForm.ID;

            var actionGrantContent =
                new actionGrantType
                    {
                        positionId = Guid.NewGuid().ToString(),
                        changeDate = DateTime.Now,
                        placer = ExportServiceHelper.RefNsiOgsExtendedType(placerProfile.RefUchr),
                        initiator = target.ID != placerProfile.RefUchr.ID
                                        ? ExportServiceHelper.RefNsiOgsExtendedType(target)
                                        : null,
                        financialYear = year,
                        planBudgetaryFunds = new actionGrantType.planBudgetaryFundsLocalType
                                                 {
                                                     capitalRealAssets =
                                                         capFunds.Sum(x => x.funds) +
                                                         realAssFunds.Sum(x => x.funds)
                                                         ?? 0,
                                                     total =
                                                         header.FinancialActivityPlan.Sum(x => x.budgetaryFunds) ?? 0
                                                 },
                        capitalConstructionFunds = capFunds
                            .Select(
                                p => new fundsType
                                         {
                                             name = p.Name,
                                             funds = p.funds ?? 0
                                         }).ToList(),
                        realAssetsFunds = realAssFunds
                            .Select(
                                p => new fundsType
                                         {
                                             name = p.Name,
                                             funds = p.funds ?? 0
                                         }).ToList(),
                        otherGrantFunds = othGrantFunds
                            .Select(
                                p => new actionGrantType.otherGrantFundsLocalType
                                         {
                                             name = p.name,
                                             funds = p.funds ?? 0,
                                             code = p.code
                                         })
                            .ToList(),
                        document = ExportServiceHelper.Documents(documents)
                    };

            return ExportServiceHelper.Serialize(
                new actionGrant
                    {
                        header = ExportServiceHelper.HeaderType(),
                        body = new actionGrant.bodyLocalType
                                   {
                                       position = actionGrantContent
                                   }
                    }.Save);
        }
    }
}
