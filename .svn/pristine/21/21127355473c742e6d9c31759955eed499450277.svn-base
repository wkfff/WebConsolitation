using System;
using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.EO15TargetPrograms.Services
{
    public class DatasourceService : IDatasourceService
    {
        private readonly IScheme scheme;
        private readonly ILinqRepository<DataSources> datasourceRepository;

        public DatasourceService(IScheme scheme, ILinqRepository<DataSources> datasourceRepository)
        {
            this.scheme = scheme;
            this.datasourceRepository = datasourceRepository;
        }

        public int GetDefaultDatasourceId()
        {
            var data = from f in datasourceRepository.FindAll()
                       where f.SupplierCode == DatasourceConstants.DatasourceConsolidationInitial.SupplierCode
                             && f.DataCode == DatasourceConstants.DatasourceConsolidationInitial.DataCode
                             && f.DataName == DatasourceConstants.DatasourceConsolidationInitial.DataName
                             && f.Variant == DatasourceConstants.DatasourceConsolidationInitial.Variant
                       select f.ID;
            switch (data.Count())
            {
                case 0:
                    throw new KeyNotFoundException("Не найден источник по умолчанию");
                case 1:
                    return data.First();
                default: 
                    throw new Exception("Обнаружено несколько источников по умолчанию");
            }
        }
        
        public void CreateDefaultDatasource()
        {
            IDataSource ds = scheme.DataSourceManager.DataSources.CreateElement();

            ds.SupplierCode = DatasourceConstants.DatasourceConsolidationInitial.SupplierCode;
            ds.DataCode = DatasourceConstants.DatasourceConsolidationInitial.DataCode.ToString();
            ds.DataName = DatasourceConstants.DatasourceConsolidationInitial.DataName;
            ds.ParametersType = DatasourceConstants.DatasourceConsolidationInitial.KindsOfParams;
            ds.Variant = DatasourceConstants.DatasourceConsolidationInitial.Variant;
               
            int sourceId = ds.Save();

            if (sourceId < 0)
            {
                throw new Exception("Ошибка создания источника по умолчанию");
            }

            Trace.TraceInformation("\"Целевые программы\" - создан источник данных \"Первоначальные данные\", id={0}", sourceId);
        }

        public int GetFactDatasourceId()
        {
            var data = from f in datasourceRepository.FindAll()
                       where f.SupplierCode == DatasourceConstants.DatasourceConsolidationFact.SupplierCode
                             && f.DataCode == DatasourceConstants.DatasourceConsolidationFact.DataCode
                             && f.DataName == DatasourceConstants.DatasourceConsolidationFact.DataName
                             && f.Variant == DatasourceConstants.DatasourceConsolidationFact.Variant
                       select f.ID;
            switch (data.Count())
            {
                case 0:
                    throw new KeyNotFoundException("Не найден источник для фактических значений");
                case 1:
                    return data.First();
                default: 
                    throw new Exception("Обнаружено несколько источников для фактических значений");
            }
        }

        public void CreateFactDatasource()
        {
            IDataSource ds = scheme.DataSourceManager.DataSources.CreateElement();

            ds.SupplierCode = DatasourceConstants.DatasourceConsolidationFact.SupplierCode;
            ds.DataCode = DatasourceConstants.DatasourceConsolidationFact.DataCode.ToString();
            ds.DataName = DatasourceConstants.DatasourceConsolidationFact.DataName;
            ds.ParametersType = DatasourceConstants.DatasourceConsolidationFact.KindsOfParams;
            ds.Variant = DatasourceConstants.DatasourceConsolidationFact.Variant;

            int sourceId = ds.Save();

            if (sourceId < 0)
            {
                throw new Exception("Ошибка создания источника для фактических значений");
            }

            Trace.TraceInformation("\"Целевые программы\" - создан источник данных \"Фактические значения\", id={0}", sourceId);
        }

        public int GetCriteriasSourceId(ProgramStage stage)
        {
            string supplierCode;
            int dataCode;
            string dataName;
            string variant;

            switch (stage)
            {
                case ProgramStage.Design:
                    supplierCode = DatasourceConstants.DatasourceEstimateDevelopment.SupplierCode;
                    dataCode = DatasourceConstants.DatasourceEstimateDevelopment.DataCode;
                    dataName = DatasourceConstants.DatasourceEstimateDevelopment.DataName;
                    variant = DatasourceConstants.DatasourceEstimateDevelopment.Variant;
                    break;
                case ProgramStage.Concept:
                    supplierCode = DatasourceConstants.DatasourceEstimateConcept.SupplierCode;
                    dataCode = DatasourceConstants.DatasourceEstimateConcept.DataCode;
                    dataName = DatasourceConstants.DatasourceEstimateConcept.DataName;
                    variant = DatasourceConstants.DatasourceEstimateConcept.Variant;
                    break;
                case ProgramStage.Realization:
                    supplierCode = DatasourceConstants.DatasourceEstimateRealization.SupplierCode;
                    dataCode = DatasourceConstants.DatasourceEstimateRealization.DataCode;
                    dataName = DatasourceConstants.DatasourceEstimateRealization.DataName;
                    variant = DatasourceConstants.DatasourceEstimateRealization.Variant;
                    break;
                default:
                    throw new Exception("Вид источника для критериев не определен");
            }

            var data = from f in datasourceRepository.FindAll()
                       where f.SupplierCode == supplierCode
                             && f.DataCode == dataCode
                             && f.DataName == dataName
                             && f.Variant == variant
                       select f.ID;
            switch (data.Count())
            {
                case 0:
                    throw new KeyNotFoundException("Не найден источник для критериев");
                case 1:
                    return data.First();
                default:
                    throw new Exception("Обнаружено несколько источников для критериев");
            }
        }

        public void CreateCriteriasDatasource(ProgramStage stage)
        {
            IDataSource ds = scheme.DataSourceManager.DataSources.CreateElement();

            switch (stage)
            {
                case ProgramStage.Design:
                    ds.SupplierCode = DatasourceConstants.DatasourceEstimateDevelopment.SupplierCode;
                    ds.DataCode = DatasourceConstants.DatasourceEstimateDevelopment.DataCode.ToString();
                    ds.DataName = DatasourceConstants.DatasourceEstimateDevelopment.DataName;
                    ds.Variant = DatasourceConstants.DatasourceEstimateDevelopment.Variant;
                    break;
                case ProgramStage.Concept:
                    ds.SupplierCode = DatasourceConstants.DatasourceEstimateConcept.SupplierCode;
                    ds.DataCode = DatasourceConstants.DatasourceEstimateConcept.DataCode.ToString();
                    ds.DataName = DatasourceConstants.DatasourceEstimateConcept.DataName;
                    ds.Variant = DatasourceConstants.DatasourceEstimateConcept.Variant;
                    break;
                case ProgramStage.Realization:
                    ds.SupplierCode = DatasourceConstants.DatasourceEstimateRealization.SupplierCode;
                    ds.DataCode = DatasourceConstants.DatasourceEstimateRealization.DataCode.ToString();
                    ds.DataName = DatasourceConstants.DatasourceEstimateRealization.DataName;
                    ds.Variant = DatasourceConstants.DatasourceEstimateRealization.Variant;
                    break;
                default:
                    throw new Exception("Вид источника для критериев не определен");
            }

            int sourceId = ds.Save();

            if (sourceId < 0)
            {
                throw new Exception("Ошибка создания источника для критериев");
            }

            Trace.TraceInformation("\"Целевые программы\" - создан источник данных для критериев, id={0}", sourceId);
        }
    }
}
