using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Extensions.E86N.Models.Abstract;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Models.ResultOfActivityModel
{
    public class ServicesWorks2016ViewModel : AbstractModelBase<ServicesWorks2016ViewModel, F_F_ShowService2016>
    {
        [DataBaseBindingTable(typeof(F_F_ShowService2016))]
        public int? Customers { get; set; }

        [DataBaseBindingTable(typeof(F_F_ShowService2016))]
        public int? Complaints { get; set; }

        [DataBaseBindingTable(typeof(F_F_ShowService2016))]
        public string Reaction { get; set; }

        public int? RefService { get; set; }

        [Description("Наименование услуги(работы)")]
        public string RefServiceName { get; set; }

        public string RefServiceTypeCode { get; set; }

        [Description("Тип услуги")]
        public string RefServiceTypeName { get; set; }

        [Description("Раздел (пункт) в государственно (муниципальном) задании")]
        public int? NRazdel { get; set; }
        
        public override string ValidateData()
        {
            const string Msg = "Необходимо заполнить поле \"{0}\"<br>";

            var message = new StringBuilder(string.Empty);

            if (RefServiceTypeCode == FX_FX_ServiceType.CodeOfService && !Customers.HasValue)
            {
                message.Append(Msg.FormatWith(DescriptionOf(() => Customers)));
            }

            if (RefServiceTypeCode == FX_FX_ServiceType.CodeOfService && !Complaints.HasValue)
            {
                message.Append(Msg.FormatWith(DescriptionOf(() => Complaints)));
            }

            if (!RefService.HasValue)
            {
                message.Append(Msg.FormatWith(DescriptionOf(() => RefServiceName)));
            }

            return message.ToString();
        }

        public override ServicesWorks2016ViewModel GetModelByDomain(F_F_ShowService2016 domain)
        {
            return new ServicesWorks2016ViewModel
                       {
                           ID = domain.ID,
                           Customers = domain.Customers,
                           Complaints = domain.Complaints,
                           Reaction = domain.Reaction,
                           RefParent = domain.RefParametr.ID,
                           RefService = domain.RefService.ID,
                           RefServiceName = domain.RefService.NameName,
                           RefServiceTypeName = domain.RefService.RefType.Name,
                           RefServiceTypeCode = domain.RefService.RefType.Code,
                           NRazdel = domain.NRazdel
                       };
        }

        public override F_F_ShowService2016 GetDomainByModel()
        {
            return new F_F_ShowService2016
                        {
                           ID = ID,
                           RefParametr = GetNewRestService().GetItem<F_F_ParameterDoc>(RefParent),
                           RefService = GetNewRestService().GetItem<D_Services_Service>(RefService),
                           Complaints = Complaints,
                           Customers = Customers,
                           NRazdel = NRazdel,
                           Reaction = Reaction
                        };
        }

        public override IQueryable<ServicesWorks2016ViewModel> GetModelData(NameValueCollection paramsList)
        {
            int docId = Convert.ToInt32(paramsList["docId"]);

            return GetNewRestService().GetItems<F_F_ShowService2016>()
               .Where(p => p.RefParametr.ID == docId)
               .Select(p => GetModelByDomain(p));
        }
    }
}
