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
    public class ServicesWorksViewModel : AbstractModelBase<ServicesWorksViewModel, F_ResultWork_ShowService>
    {
        [Description("Количество потребителей")]
        public int? Customers { get; set; }

        [Description("Количество жалоб")]
        public int? Complaints { get; set; }

        [Description("Принятые меры по результатам рассмотрения жалоб")]
        public string Reaction { get; set; }

        public int? RefVedPch { get; set; }

        [Description("Наименование услуги(работы)")]
        public string RefVedPchName { get; set; }

        public int? RefTipY { get; set; }

        [Description("Тип услуги")]
        public string RefTipYName { get; set; }

        [Description("Раздел (пункт) в государственно (муниципальном) задании")]
        public int? NRazdel { get; set; }
        
        public override string ValidateData()
        {
            const string Msg = "Необходимо заполнить поле \"{0}\"<br>";

            var message = new StringBuilder(string.Empty);

            if (RefTipY == D_Services_TipY.FX_FX_SERVICE && !Customers.HasValue)
            {
                message.Append(Msg.FormatWith(DescriptionOf(() => Customers)));
            }

            if (RefTipY == D_Services_TipY.FX_FX_SERVICE && !Complaints.HasValue)
            {
                message.Append(Msg.FormatWith(DescriptionOf(() => Complaints)));
            }

            if (!RefVedPch.HasValue)
            {
                message.Append(Msg.FormatWith(DescriptionOf(() => RefVedPchName)));
            }

            return message.ToString();
        }

        public override ServicesWorksViewModel GetModelByDomain(F_ResultWork_ShowService domain)
        {
            return new ServicesWorksViewModel
                       {
                            ID = domain.ID, 
                            Customers = domain.Customers, 
                            Complaints = domain.Complaints, 
                            Reaction = domain.Reaction, 
                            RefParent = domain.RefParametr.ID, 
                            RefVedPch = domain.RefVedPch.ID, 
                            RefVedPchName = domain.RefVedPch.Name, 
                            RefTipYName = domain.RefVedPch.RefTipY.Name, 
                            RefTipY = domain.RefVedPch.RefTipY.ID, 
                            NRazdel = domain.NRazdel
                    };
        }

        public override F_ResultWork_ShowService GetDomainByModel()
        {
            return new F_ResultWork_ShowService
                       {
                           ID = ID,
                           RefParametr = GetNewRestService().GetItem<F_F_ParameterDoc>(RefParent),
                           NRazdel = NRazdel,
                           Complaints = Complaints,
                           Customers = Customers,
                           Reaction = Reaction,
                           RefVedPch = GetNewRestService().GetItem<D_Services_VedPer>(RefVedPch)
                        };
        }

        public override IQueryable<ServicesWorksViewModel> GetModelData(NameValueCollection paramsList)
        {
            int docId = Convert.ToInt32(paramsList["docId"]);

            return GetNewRestService().GetItems<F_ResultWork_ShowService>().Where(p => p.RefParametr.ID == docId).Select(p => GetModelByDomain(p));
        }
    }
}
