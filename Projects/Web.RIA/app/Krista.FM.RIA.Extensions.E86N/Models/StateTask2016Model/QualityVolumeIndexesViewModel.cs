using System;
using System.ComponentModel;
using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Models.StateTask2016Model
{
    public class QualityVolumeIndexesViewModel : ViewModelBase
    {
        public int ID { get; set; }

        [DataBaseBindingField(typeof(F_F_PNRZnach2016), "Protklp")]
        public string Protklp { get; set; }

        [DataBaseBindingField(typeof(F_F_PNRZnach2016), "ReportingYear")]
        public string ReportingYear { get; set; }

        [DataBaseBindingField(typeof(F_F_PNRZnach2016), "CurrentYear")]
        public string CurrentYear { get; set; }

        [DataBaseBindingField(typeof(F_F_PNRZnach2016), "ComingYear")]
        public string ComingYear { get; set; }

        [DataBaseBindingField(typeof(F_F_PNRZnach2016), "FirstPlanYear")]
        public string FirstPlanYear { get; set; }

        [DataBaseBindingField(typeof(F_F_PNRZnach2016), "SecondPlanYear")]
        public string SecondPlanYear { get; set; }

        [DataBaseBindingField(typeof(F_F_PNRZnach2016), "ActualValue")]
        public string ActualValue { get; set; }

        [DataBaseBindingField(typeof(F_F_PNRZnach2016), "Deviation")]
        public string Deviation { get; set; }

        [DataBaseBindingField(typeof(F_F_PNRZnach2016), "Reject")]
        public string Reject { get; set; }
        
        [DataBaseBindingField(typeof(F_F_PNRZnach2016), "AveragePriceFact")]
        public decimal? AveragePriceFact { get; set; }

        public int? RefReport { get; set; }

        [Description("Отчет о выполнении")]
        [DataBaseBindingField(typeof(F_F_Reports), "NameReport")]
        public string RefReportName { get; set; }
        
        public int RefIndicators { get; set; }

        [Description("Код показателя")]
        [DataBaseBindingField(typeof(F_F_ServiceIndicators), "Code")]
        public string RefIndicatorsCode { get; set; }
        
        [Description("Наименование показателя")]
        [DataBaseBindingField(typeof(F_F_ServiceIndicators), "Name")]
        public string RefIndicatorsName { get; set; }

        public int RefIndicatorsType { get; set; }

        [Description("Код типа показателя")]
        [DataBaseBindingField(typeof(FX_FX_CharacteristicType), "Code")]
        public int RefIndicatorsTypeCode { get; set; }

        [Description("Тип показателя")]
        [DataBaseBindingField(typeof(FX_FX_CharacteristicType), "Name")]
        public string RefIndicatorsTypeName { get; set; }

        public int RefIndicatorsOKEI { get; set; }

        [Description("Код ОКЕИ")]
        [DataBaseBindingField(typeof(D_Org_OKEI), "Code")]
        public int RefIndicatorsOKEICode { get; set; }

        [Description("Наименование по ОКЕИ")]
        [DataBaseBindingField(typeof(D_Org_OKEI), "Name")]
        public string RefIndicatorsOKEIName { get; set; }
    }
}
