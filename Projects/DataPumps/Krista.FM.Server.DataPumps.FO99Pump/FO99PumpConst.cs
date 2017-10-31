using System;
using System.Collections.Generic;
using System.Text;

namespace Krista.FM.Server.DataPumps.FO99Pump
{

    public struct XmlConsts
    {

        #region Источники данных

        public const string supplierCodeAttr = "supplierCode";
        public const string dataCodeAttr = "dataCode";
        public const string kindsOfParamsAttr = "kindsOfParams";
        public const string yearAttr = "year";
        public const string monthAttr = "month";
        public const string quarterAttr = "quarter";
        public const string variantAttr = "variant";
        public const string dataSourceNodeTag = "dataSource";
        #endregion

        public const string commonDataSourceNodeTag = "commonDataSource";
        public const string rootTag = "fmDbData";
        public const string fmObjectsTag = "fmEntities";
        public const string fmObjectTag = "fmEntity";
        public const string fmObjectRowTag = "fmEntityRow";
        public const string fmObjectTypeAttr = "fmEntityType";
        public const string fmObjectKey = "objectKey";
        public const string fmObjectFullDbName = "fullDbName";
        public const string fmObjectDeleteConstraint = "deleteConstraint";

    }

    public enum FmObjectsTypes : int
    {
        cls = 0,
        fct = 1
    }


}
