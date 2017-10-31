using System;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.XmlExportImporter.Helpers
{
    internal class DataSourceHelper
    {
        /// <summary>
        /// проверяет, существует ли источник данных из XML
        /// </summary>
        internal static int AddDataSource(IScheme scheme, XMLDataSource loadedDataSource, IEntity schemeObject, IClassifiersProtocol protocol)
        {
            int sourceId;
            var sourceManager = scheme.DataSourceManager;
            var ds = sourceManager.DataSources.CreateElement();
            ds.BudgetName = loadedDataSource.name;
            ds.DataCode = loadedDataSource.dataCode;
            ds.DataName = loadedDataSource.dataName;
            ds.Month = Convert.ToInt32(loadedDataSource.month);
            ds.ParametersType = GetParamsType(loadedDataSource.kindsOfParams);
            ds.Quarter = Convert.ToInt32(loadedDataSource.quarter);
            ds.SupplierCode = loadedDataSource.suppplierCode;
            ds.Territory = loadedDataSource.territory;
            ds.Variant = loadedDataSource.variant;
            ds.Year = Convert.ToInt32(loadedDataSource.year);
            if (!sourceManager.DataSources.Contains(ds))
            {
                // если источника нету, то добавляем его
                sourceId = sourceManager.DataSources.Add(ds);
                string dataSourceName = sourceManager.GetDataSourceName(sourceId);
                ProtocolHelper.SendMessageToProtocol(schemeObject, protocol, String.Format("Добавлен источник {0} (ID = {1})", dataSourceName, sourceId), sourceId, ClassifiersEventKind.ceInformation);
            }
            else
            {
                // если есть, то палучаем его ID
                sourceId = Convert.ToInt32(sourceManager.DataSources.FindDataSource(ds));
            }
            // Создаем новую версию
            IDataVersion newDataVersion = scheme.DataVersionsManager.DataVersions.Create();
            newDataVersion.IsCurrent = false;
            newDataVersion.ObjectKey = schemeObject.ObjectKey;
            newDataVersion.PresentationKey = Guid.Empty.ToString();
            newDataVersion.SourceID = sourceId;
            newDataVersion.Name = String.Format("{0}.{1}", schemeObject.FullCaption,
                                                scheme.DataSourceManager.GetDataSourceName(
                                                    sourceId));
            scheme.DataVersionsManager.DataVersions.Add(newDataVersion);
            return sourceId;
        }

        /// <summary>
        /// получение из строки типа параметров источника
        /// </summary>
        /// <param name="kindsOfParams"></param>
        /// <returns></returns>
        internal static ParamKindTypes GetParamsType(string kindsOfParams)
        {
            switch (kindsOfParams)
            {
                case "YearVariant":
                    return ParamKindTypes.YearVariant;
                case "YearTerritory":
                    return ParamKindTypes.YearTerritory;
                case "YearQuarterMonth":
                    return ParamKindTypes.YearQuarterMonth;
                case "YearQuarter":
                    return ParamKindTypes.YearQuarter;
                case "YearMonthVariant":
                    return ParamKindTypes.YearMonthVariant;
                case "YearMonth":
                    return ParamKindTypes.YearMonth;
                case "Year":
                    return ParamKindTypes.Year;
                case "WithoutParams":
                    return ParamKindTypes.WithoutParams;
                case "Budget":
                    return ParamKindTypes.Budget;
                case "Variant":
                    return ParamKindTypes.Variant;
                case "YearVariantMonthTerritory":
                    return ParamKindTypes.YearVariantMonthTerritory;
            }
            return ParamKindTypes.NoDivide;
        }
    }
}
