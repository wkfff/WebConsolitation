using System.Linq;
using System.Text.RegularExpressions;
using bus.gov.ru.external.Item1;
using bus.gov.ru.types.Item1;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Xml.Schema.Linq;

namespace Krista.FM.RIA.Extensions.E86N.Services.Pump
{
    public static class BudgetaryCircumstances2Smeta
    {
        public static F_F_ParameterDoc Pump(XTypedElement pumpXml)
        {
            var pumpHeader = budgetaryCircumstances.Parse(pumpXml.Untyped.ToString()).header;
            budgetaryCircumstancesType pumpData = budgetaryCircumstances.Parse(pumpXml.Untyped.ToString()).body.position;
            refNsiConsRegExtendedStrongType targetOrg = pumpData.initiator ?? pumpData.placer;
            
            const string KBKPattern = @"(\d{3}|null|)(\d{4}|null)(\d{7}|null)(\d{3}|null)(\d{3}|null)";

            var header = Helpers.Header(targetOrg, FX_FX_PartDoc.SmetaDocTypeID, pumpData.changeDate);

            pumpData.budgetaryCircumstance
                .Select(
                    type => Regex.Match(type.kbkBudget.code, KBKPattern).With(
                        kbkBudget =>
                        new F_Fin_Smeta
                            {
                                Funds = type.circumstance,
                                RefRazdPodr = Resolver.Get<ILinqRepository<D_Fin_RazdPodr>>().FindAll()
                                    .FirstOrDefault(podr => podr.Code.Equals(kbkBudget.Groups[2].Value)),
                                CelStatya = kbkBudget.Groups[3].Value,
                                RefVidRash = Resolver.Get<ILinqRepository<D_Fin_VidRash>>().FindAll()
                                    .FirstOrDefault(rash => rash.Code.Equals(kbkBudget.Groups[4].Value)),
                                RefKosgy = Resolver.Get<ILinqRepository<D_KOSGY_KOSGY>>().FindAll()
                                    .FirstOrDefault(kosgy => kosgy.Code.Equals(kbkBudget.Groups[5].Value)),
                                RefBudget = Resolver.Get<ILinqRepository<D_Fin_nsiBudget>>().FindAll()
                                    .First(budget => budget.Code.Equals(type.kbkBudget.budget.code)),
                                RefParametr = header,
                            }))
                .Each(header.Smetas.Add);

            Helpers.ProcessDocumentsHeader(
                header,
                pumpHeader,
                pumpData.document,
                type => type.With(
                    x => Resolver.Get<ILinqRepository<D_Doc_TypeDoc>>().FindAll().Single(doc => doc.Code.Equals("B"))));

            return header;
        }
    }
}
