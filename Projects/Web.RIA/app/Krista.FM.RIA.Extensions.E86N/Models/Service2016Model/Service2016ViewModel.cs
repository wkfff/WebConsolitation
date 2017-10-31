using System;
using System.ComponentModel;
using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Models.Service2016Model
{
    public class Service2016ViewModel : ViewModelBase
    {
        public int ID { get; set; }

        [DataBaseBindingTable(typeof(D_Services_Service))]
        public string Regrnumber { get; set; }

        [DataBaseBindingTable(typeof(D_Services_Service))]
        public DateTime EffectiveFrom { get; set; }

        [DataBaseBindingTable(typeof(D_Services_Service))]
        public DateTime? EffectiveBefore { get; set; }

        [DataBaseBindingTable(typeof(D_Services_Service))]
        public string InstCode { get; set; }

        [DataBaseBindingTable(typeof(D_Services_Service))]
        public string NameCode { get; set; }

        [DataBaseBindingTable(typeof(D_Services_Service))]
        public string NameName { get; set; }

        [DataBaseBindingTable(typeof(D_Services_Service))]
        public string SvcCnts1CodeVal { get; set; }

        [DataBaseBindingTable(typeof(D_Services_Service))]
        public string SvcCnts2CodeVal { get; set; }

        [DataBaseBindingTable(typeof(D_Services_Service))]
        public string SvcCnts3CodeVal { get; set; }

        [DataBaseBindingTable(typeof(D_Services_Service))]
        public string SvcCntsName1Val { get; set; }

        [DataBaseBindingTable(typeof(D_Services_Service))]
        public string SvcCntsName2Val { get; set; }

        [DataBaseBindingTable(typeof(D_Services_Service))]
        public string SvcCntsName3Val { get; set; }

        [DataBaseBindingTable(typeof(D_Services_Service))]
        public string SvcTerms1CodeVal { get; set; }

        [DataBaseBindingTable(typeof(D_Services_Service))]
        public string SvcTerms2CodeVal { get; set; }

        [DataBaseBindingTable(typeof(D_Services_Service))]
        public string SvcTermsName1Val { get; set; }

        [DataBaseBindingTable(typeof(D_Services_Service))]
        public string SvcTermsName2Val { get; set; }

        [DataBaseBindingTable(typeof(D_Services_Service))]
        public string GUID { get; set; }

        [DataBaseBindingTable(typeof(D_Services_Service))]
        public bool? IsActual { get; set; }
        
        [DataBaseBindingTable(typeof(D_Services_Service))]
        public bool IsEditable { get; set; }
        
        [Description("Статус")]
        public bool BusinessStatus { get; set; }

        public int RefType { get; set; }

        [Description("Код вида услуги")]
        [DataBaseBindingField(typeof(FX_FX_ServiceType), "Code")]
        public string RefTypeCode { get; set; }

        [Description("Услуга/работа")]
        [DataBaseBindingField(typeof(FX_FX_ServiceType), "Name")]
        public string RefTypeName { get; set; }

        public int RefPay { get; set; }

        [Description("Код вида платности")]
        [DataBaseBindingField(typeof(FX_FX_ServicePayType2), "Code")]
        public string RefPayCode { get; set; }

        [Description("Платность")]
        [DataBaseBindingField(typeof(FX_FX_ServicePayType2), "Name")]
        public string RefPayName { get; set; }

        public int RefOKTMO { get; set; }

        [Description("Код ППО")]
        [DataBaseBindingField(typeof(D_OKTMO_OKTMO), "Code")]
        public string RefOKTMOCode { get; set; }

        [Description("Наименование ППО")]
        [DataBaseBindingField(typeof(D_OKTMO_OKTMO), "Name")]
        public string RefOKTMOName { get; set; }

        public int RefYchr { get; set; }

        [Description("Сокращенное наименование учредителя")]
        [DataBaseBindingField(typeof(D_Org_Structure), "ShortName")]
        public string RefYchrShortName { get; set; }

        [Description("Полное наименование учредителя")]
        [DataBaseBindingField(typeof(D_Org_Structure), "Name")]
        public string RefYchrName { get; set; }

        [Description("ИНН учредителя")]
        [DataBaseBindingField(typeof(D_Org_Structure), "INN")]
        public string RefYchrInn { get; set; }

        public int RefActivityType { get; set; }

        [Description("Код вида деятельности")]
        [DataBaseBindingField(typeof(D_Services_ActivityType), "Code")]
        public string RefActivityTypeCode { get; set; }

        [Description("Вид деятельности")]
        [DataBaseBindingField(typeof(D_Services_ActivityType), "Name")]
        public string RefActivityTypeName { get; set; }

        [DataBaseBindingTable(typeof(D_Services_Service))]
        public virtual bool FromPlaning { get; set; }
    }
}
