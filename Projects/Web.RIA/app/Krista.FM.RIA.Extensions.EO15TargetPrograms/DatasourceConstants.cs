using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.EO15TargetPrograms
{
    internal struct DatasourceStruct
        {
            public string SupplierCode;
            public int DataCode;
            public string DataName;
            public ParamKindTypes KindsOfParams;
            public string Variant;
        }

    internal static class DatasourceConstants
    {
        public static readonly DatasourceStruct DatasourceConsolidationInitial = new DatasourceStruct
                                                         {
                                                             SupplierCode = "ЭО",
                                                             DataCode = 15,
                                                             DataName = "Сбор ЦП",
                                                             KindsOfParams = ParamKindTypes.Variant,
                                                             Variant = "Первоначальные данные",
                                                         };

        public static readonly DatasourceStruct DatasourceConsolidationFact = new DatasourceStruct
        {
            SupplierCode = DatasourceConsolidationInitial.SupplierCode,
            DataCode = DatasourceConsolidationInitial.DataCode,
            DataName = DatasourceConsolidationInitial.DataName,
            KindsOfParams = DatasourceConsolidationInitial.KindsOfParams,
            Variant = "Фактические значения",
        };

        public static readonly DatasourceStruct DatasourceEstimateConcept = new DatasourceStruct
        {
            SupplierCode = DatasourceConsolidationInitial.SupplierCode,
            DataCode = 16,
            DataName = "Оценка ЦП",
            KindsOfParams = ParamKindTypes.Variant,
            Variant = "Разработка концептуальных предложений"
        };

        public static readonly DatasourceStruct DatasourceEstimateDevelopment = new DatasourceStruct
        {
            SupplierCode = DatasourceConsolidationInitial.SupplierCode,
            DataCode = DatasourceEstimateConcept.DataCode,
            DataName = DatasourceEstimateConcept.DataName,
            KindsOfParams = DatasourceEstimateConcept.KindsOfParams,
            Variant = "Разработка проекта"
        };

        public static readonly DatasourceStruct DatasourceEstimateRealization = new DatasourceStruct
        {
            SupplierCode = DatasourceConsolidationInitial.SupplierCode,
            DataCode = DatasourceEstimateConcept.DataCode,
            DataName = DatasourceEstimateConcept.DataName,
            KindsOfParams = DatasourceEstimateConcept.KindsOfParams,
            Variant = "Реализация"
        };
    }
}