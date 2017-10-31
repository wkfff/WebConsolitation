using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Extensions.E86N.Models.Abstract;

namespace Krista.FM.RIA.Extensions.E86N.Models.ResultOfActivityModel
{
    public class CashPaymentsViewModel : AbstractModelBase<CashPaymentsViewModel, F_ResultWork_CashPay>
    {
        public string Name { get; set; }

        public int? RefKosgy { get; set; }

        [Description("КОСГУ")]
        public string RefKosgyName { get; set; }

        public int? RefRazdPodr { get; set; }

        [Description("Раздел/Подраздел")]
        public string RefRazdPodrName { get; set; }

        public int? RefVidRash { get; set; }

        [Description("Вид расходов")]
        public string RefVidRashName { get; set; }

        public string ExpenseWay { get; set; }

        [Description("Сумма ")]
        public decimal? Payment { get; set; }

        [Description("Целевая статья")]
        public string CelStatya { get; set; }

        public override string ValidateData()
        {
            const string Msg = "Необходимо заполнить поле \"{0}\"<br>";

            var doc = GetNewRestService().GetItem<F_F_ParameterDoc>(RefParent);
            var type = doc.RefUchr.RefTipYc.ID;
            var yearForm = doc.RefYearForm.ID;

            var message = new StringBuilder(string.Empty);

            if (!Payment.HasValue)
            {
                message.Append(Msg.FormatWith(DescriptionOf(() => Payment)));
            }

            if (type.Equals(FX_Org_TipYch.GovernmentID) || (type.Equals(FX_Org_TipYch.BudgetaryID) && yearForm > 2015))
            {
                if (!RefVidRash.HasValue)
                {
                    message.Append(Msg.FormatWith(DescriptionOf(() => RefVidRashName)));
                }

                if (!RefRazdPodr.HasValue)
                {
                    message.Append(Msg.FormatWith(DescriptionOf(() => RefRazdPodrName)));
                }

                if (CelStatya.IsNullOrEmpty())
                {
                    message.Append(Msg.FormatWith(DescriptionOf(() => CelStatya)));
                }
            }

            if (!RefKosgy.HasValue)
            {
                message.Append(Msg.FormatWith(DescriptionOf(() => RefKosgyName)));
            }
            else
            {
                // проверки по задаче https://panda.krista.ru/issues/6423 (не должно быть дублей)
                var kosgyValidator = GetNewRestService().GetItems<F_ResultWork_CashPay>()
                    .Where(
                        p => p.RefParametr.ID == RefParent
                        && p.ID != ID
                        && p.RefKosgy.ID == RefKosgy);
                if (kosgyValidator.Any())
                {
                    var profile = GetNewRestService().GetItem<F_F_ParameterDoc>(RefParent).RefUchr.RefTipYc.ID;
                    if (profile == FX_Org_TipYch.BudgetaryID || profile == FX_Org_TipYch.AutonomousID)
                    {
                        message.Append("Введенная Вами кассовая выплата уже присутствует в документе");
                    }
                    else
                    {
                        if (kosgyValidator.Any(
                            p => p.RefRazdPodr.ID == RefRazdPodr
                            && p.RefVidRash.ID == RefVidRash
                            && p.CelStatya == CelStatya))
                        {
                            message.Append("Введенная Вами кассовая выплата уже присутствует в документе");
                        }
                    }
                }
            }

            return message.ToString();
        }

        public override CashPaymentsViewModel GetModelByDomain(F_ResultWork_CashPay domain)
        {
            return new CashPaymentsViewModel
                    {
                        ID = domain.ID,
                        Payment = domain.Payment,
                        CelStatya = domain.CelStatya,
                        RefKosgy = domain.RefKosgy.ID,
                        RefKosgyName = string.Format("{0, -5 :#\\.#\\.#};{1}", Convert.ToInt32(domain.RefKosgy.Code), domain.RefKosgy.Name),
                        RefRazdPodr = domain.RefRazdPodr != null ? domain.RefRazdPodr.ID : (int?)null,
                        RefRazdPodrName = domain.RefRazdPodr != null ? domain.RefRazdPodr.Name : string.Empty,
                        RefVidRash = domain.RefVidRash != null ? domain.RefVidRash.ID : (int?)null,
                        RefVidRashName = domain.RefVidRash != null ? domain.RefVidRash.Name : string.Empty,
                        RefParent = domain.RefParametr.ID
                    };
        }

        public override F_ResultWork_CashPay GetDomainByModel()
        {
            return new F_ResultWork_CashPay
                    {
                        ID = ID,
                        RefParametr = GetNewRestService().GetItem<F_F_ParameterDoc>(RefParent),
                        CelStatya = CelStatya,
                        Payment = Payment.GetValueOrDefault(),
                        RefKosgy = RefKosgy.HasValue ? GetNewRestService().GetItem<D_KOSGY_KOSGY>(RefKosgy) : null,
                        RefRazdPodr = RefRazdPodr.HasValue ? GetNewRestService().GetItem<D_Fin_RazdPodr>(RefRazdPodr) : null,
                        RefVidRash = RefVidRash.HasValue ? GetNewRestService().GetItem<D_Fin_VidRash>(RefVidRash) : null
                    };
        }

        public override IQueryable<CashPaymentsViewModel> GetModelData(NameValueCollection paramsList)
        {
            int docId = Convert.ToInt32(paramsList["docId"]);
            return GetNewRestService().GetItems<F_ResultWork_CashPay>().Where(p => p.RefParametr.ID == docId).Select(p => GetModelByDomain(p));
        }
    }
}
