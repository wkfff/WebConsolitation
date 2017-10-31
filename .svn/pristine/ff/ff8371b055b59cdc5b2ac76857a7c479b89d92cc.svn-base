using System;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Extensions.E86N.Models.Abstract;
using Krista.FM.RIA.Extensions.E86N.Services.AnnualBalance;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Models.AnnualBalance
{
    public class AnnualBalanceF0503721ViewModel : AbstractModelBase<AnnualBalanceF0503721ViewModel, F_Report_BalF0503721>
    {
        public int Section { get; set; }

        [DataBaseBindingTable(typeof(F_Report_BalF0503721))]
        public string Name { get; set; }

        [DataBaseBindingField(typeof(F_Report_BalF0503721), "lineCode")]
        public string LineCode { get; set; }

        [DataBaseBindingField(typeof(F_Report_BalF0503721), "analyticCode")]
        public string AnalyticCode { get; set; }

        [DataBaseBindingField(typeof(F_Report_BalF0503721), "targetFunds")]
        public decimal? TargetFunds { get; set; }

        [DataBaseBindingField(typeof(F_Report_BalF0503721), "services")]
        public decimal? Services { get; set; }

        [DataBaseBindingField(typeof(F_Report_BalF0503721), "temporaryFunds")]
        public decimal? TemporaryFunds { get; set; }

        [DataBaseBindingTable(typeof(F_Report_BalF0503721))]
        public decimal? StateTaskFunds { get; set; }

        [DataBaseBindingTable(typeof(F_Report_BalF0503721))]
        public decimal? RevenueFunds { get; set; }

        [DataBaseBindingField(typeof(F_Report_BalF0503721), "total")]
        public decimal? Total { get; set; }
        
        public override string ValidateData()
        {
            const string Msg = "Необходимо заполнить поле \"{0}\" <br>";
            const string Msg1 = "Показатель с кодом строки \"{0}\" уже присутствует в документе <br>";
            const string Msg2 = "Показатель с кодом строки \"{0}\" должен размещаться только в детализации \"{1}\"<br>";

            var message = new StringBuilder(string.Empty);

            if (Name.IsNullOrEmpty())
            {
                message.Append(Msg.FormatWith(DescriptionOf(() => Name)));
            }

            if (LineCode.IsNullOrEmpty())
            {
                message.Append(Msg.FormatWith(DescriptionOf(() => LineCode)));
            }
            else
            {
                int intLineCode = Convert.ToInt32(LineCode.Substring(0, 2));

                if (intLineCode < 12 
                    && !Section.Equals((int)F0503721Details.Incomes))
                {
                    message.Append(Msg2.FormatWith(LineCode, AnnualBalanceHelpers.F0503721DetailsNameMapping(F0503721Details.Incomes)));
                }

                if (intLineCode > 14 
                    && intLineCode < 31 
                    && !Section.Equals((int)F0503721Details.Expenses))
                {
                    message.Append(Msg2.FormatWith(LineCode, AnnualBalanceHelpers.F0503721DetailsNameMapping(F0503721Details.Expenses)));
                }

                if (intLineCode > 30
                    && intLineCode < 38
                    && !Section.Equals((int)F0503721Details.NonFinancialAssets))
                {
                    message.Append(Msg2.FormatWith(LineCode, AnnualBalanceHelpers.F0503721DetailsNameMapping(F0503721Details.NonFinancialAssets)));
                }

                if (intLineCode > 37
                    && intLineCode < 55
                    && !Section.Equals((int)F0503721Details.FinancialAssetsLiabilities))
                {
                    message.Append(Msg2.FormatWith(LineCode, AnnualBalanceHelpers.F0503721DetailsNameMapping(F0503721Details.FinancialAssetsLiabilities)));
                }
            }
            
            if (GetNewRestService().GetItems<F_Report_BalF0503721>().Any(x => x.RefParametr.ID.Equals(RefParent.Value)
                                                        && x.Section.Equals(Section)
                                                        && x.lineCode.Equals(LineCode)
                                                        && !x.ID.Equals(ID)))
            {
                message.Append(Msg1.FormatWith(LineCode));
            }
            
            return message.ToString();
        }
        
        public override AnnualBalanceF0503721ViewModel GetModelByDomain(F_Report_BalF0503721 domain)
        {
            return new AnnualBalanceF0503721ViewModel
                        {
                            ID = domain.ID,
                            Name = domain.Name,
                            RefParent = domain.RefParametr.ID,
                            LineCode = domain.lineCode,
                            AnalyticCode = domain.analyticCode,
                            TargetFunds = domain.targetFunds,
                            Services = domain.services,
                            TemporaryFunds = domain.temporaryFunds,
                            StateTaskFunds = domain.StateTaskFunds,
                            RevenueFunds = domain.RevenueFunds,
                            Total = domain.total,
                            Section = domain.Section
                       };
        }
        
        public override F_Report_BalF0503721 GetDomainByModel()
        {
            return new F_Report_BalF0503721
            {
                           ID = ID,
                           RefParametr = GetNewRestService().GetItem<F_F_ParameterDoc>(RefParent),
                           Name = Name,
                           targetFunds = TargetFunds.GetValueOrDefault(),
                           RevenueFunds = RevenueFunds,
                           StateTaskFunds = StateTaskFunds,
                           services = Services,
                           lineCode = LineCode,
                           Section = Section,
                           analyticCode = AnalyticCode,
                           temporaryFunds = TemporaryFunds,
                           total = Total.GetValueOrDefault()
            };
        }

        public override IQueryable<AnnualBalanceF0503721ViewModel> GetModelData(NameValueCollection paramsList)
        {
            int docId = Convert.ToInt32(paramsList["docId"]);
            int section = Convert.ToInt32(paramsList["section"]);

            return GetNewRestService().GetItems<F_Report_BalF0503721>()
               .Where(p => p.RefParametr.ID.Equals(docId) && p.Section.Equals(section))
               .Select(p => this.GetModelByDomain(p));
        }
    }
}
