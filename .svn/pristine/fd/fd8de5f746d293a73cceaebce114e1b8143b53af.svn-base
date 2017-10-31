using System;
using System.Linq;
using bus.gov.ru.external.Item1;
using bus.gov.ru.types.Item1;
using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;
using GlobalConsts = Krista.FM.RIA.Extensions.E86N.Utils.GlobalConsts;

namespace Krista.FM.RIA.Extensions.E86N.Services.Export
{
    public static class ExportBudgetaryCircumstancesService
    {
        public static byte[] Serialize(IAuthService authService, F_F_ParameterDoc header)
        {
            var target = header.RefUchr;
            var yearForm = header.RefYearForm.ID;
            var placerProfile = authService.Profile;
            if (placerProfile == null)
            {
                throw new InvalidOperationException(GlobalConsts.NullProfile);
            }

            var budgetaryCircumstancesType =
                new budgetaryCircumstancesType
                    {
                        positionId = Guid.NewGuid().ToString(),
                        changeDate = DateTime.Now,
                        placer = ExportServiceHelper.RefNsiOgsExtendedType(placerProfile.RefUchr),
                        initiator = target.ID != placerProfile.RefUchr.ID
                                        ? ExportServiceHelper.RefNsiOgsExtendedType(target)
                                        : null,
                        financialYear = header.RefYearForm.ID,
                        section = target.RefOrgGRBS.Code,
                        budgetaryCircumstance = header.Smetas.ToList()
                            .GroupBy(
                                x => new
                                         {
                                             code = GetKbkCode(target, x, yearForm),
                                             budget = x.RefBudget.Code,
                                             name = x.RefKosgy.Name
                                         },
                                x => x.Funds.GetValueOrDefault(0))
                            .Select(
                                x => new budgetaryCircumstancesType.budgetaryCircumstanceLocalType
                                         {
                                             kbkBudget = new refNsiKbkBudgetType
                                                             {
                                                                 code = x.Key.code,
                                                                 budget = new refNsiBudgetStrongType
                                                                              {
                                                                                  code = x.Key.budget
                                                                              },
                                                                 name = x.Key.name
                                                             },
                                             circumstance = x.Sum()
                                         })
                            .ToList(),
                        document = ExportServiceHelper.Documents(header.Documents.ToList())
                    };

            return ExportServiceHelper.Serialize(
                new budgetaryCircumstances
                    {
                        header = ExportServiceHelper.HeaderType(),
                        body = new budgetaryCircumstances.bodyLocalType
                                   {
                                       position = budgetaryCircumstancesType
                                   }
                    }.Save);
        }

        private static string GetKbkCode(D_Org_Structure structure, F_Fin_Smeta smeta, int yearForm)
        {
            if (yearForm < 2016)
            {
                return string.Format(
                    "{0,3}{1,4}{2,7}{3,3}{4,3}",
                    structure.RefOrgGRBS.Code.PadLeft(3, '0'),
                    smeta.RefRazdPodr.Code.PadLeft(4, '0'),
                    smeta.CelStatya.PadLeft(7, '0'),
                    smeta.RefVidRash.Code.PadLeft(3, '0'),
                    smeta.RefKosgy.Code.PadLeft(3, '0'));
            }

            return string.Format(
                "{0,3}{1,4}{2,10}{3,3}",
                structure.RefOrgGRBS.Code.PadLeft(3, '0'),
                smeta.RefRazdPodr.Code.PadLeft(4, '0'),
                smeta.CelStatya.PadLeft(10, '0'),
                smeta.RefVidRash.Code.PadLeft(3, '0'));
        }
    }
}
