using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Xml;
using Krista.FM.Common.Xml;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.FO99Pump
{

    #region DataSource
    public sealed class DataSource
    {
        internal string supplierCode;
        internal string dataCode;
        internal string kindsOfParams;
        internal string year;
        internal string month;
        internal string quarter;
        internal string variant;
        internal int id;
        internal CorrectedPumpModuleBase pumpModule = null;

        public DataSource(CorrectedPumpModuleBase pumpModule)
        {
            this.pumpModule = pumpModule;
        }

        internal void LoadFromXml(XmlNode node)
        {
            supplierCode = XmlHelper.GetStringAttrValue(node, XmlConsts.supplierCodeAttr, String.Empty);
            dataCode = XmlHelper.GetStringAttrValue(node, XmlConsts.dataCodeAttr, String.Empty);
            kindsOfParams = XmlHelper.GetStringAttrValue(node, XmlConsts.kindsOfParamsAttr, String.Empty);
            year = XmlHelper.GetStringAttrValue(node, XmlConsts.yearAttr, "0");
            month = XmlHelper.GetStringAttrValue(node, XmlConsts.monthAttr, "0");
            quarter = XmlHelper.GetStringAttrValue(node, XmlConsts.quarterAttr, "0");
            variant = XmlHelper.GetStringAttrValue(node, XmlConsts.variantAttr, String.Empty);
            id = pumpModule.AddDataSource(supplierCode, dataCode, (ParamKindTypes)Convert.ToInt32(kindsOfParams),
                string.Empty, Convert.ToInt32(year), Convert.ToInt32(month), variant, Convert.ToInt32(quarter), string.Empty).ID;
        }

    }
    #endregion

    #region FMObject
    public sealed class FMObject
    {
        internal FmObjectsTypes objectType;
        internal string objectKey;
        internal IEntity entity;
        internal IDbDataAdapter adapter;
        internal DataSet dataset;
        internal DataSource dataSource = null;
        internal CorrectedPumpModuleBase pumpModule = null;
        internal Dictionary<int, DataRow> cache = null;
        internal XmlNodeList dataNodes = null;
        internal string deleteConstraint = string.Empty;

        public FMObject(CorrectedPumpModuleBase pumpModule)
        {
            this.pumpModule = pumpModule;
        }

        internal void Clear()
        {
            if (entity != null)
                entity = null;
            if (adapter != null)
                adapter = null;
            if (dataset != null)
            {
                dataset.Clear();
                dataset = null;
            }
            if (cache != null)
            {
                cache.Clear();
                cache = null;
            }
        }

        internal void LoadFromXml(XmlNode node)
        {
            objectKey = XmlHelper.GetStringAttrValue(node, XmlConsts.fmObjectKey, string.Empty);
            deleteConstraint = XmlHelper.GetStringAttrValue(node, XmlConsts.fmObjectDeleteConstraint, string.Empty);
            if (XmlHelper.GetStringAttrValue(node, XmlConsts.fmObjectTypeAttr, string.Empty) == "cls")
                objectType = FmObjectsTypes.cls;
            else
                objectType = FmObjectsTypes.fct;

            XmlNode xn = node.SelectSingleNode(XmlConsts.dataSourceNodeTag);
            if (xn == null)
                xn = node.OwnerDocument.DocumentElement.SelectSingleNode(XmlConsts.commonDataSourceNodeTag);
            dataSource = new DataSource(pumpModule);
            dataSource.LoadFromXml(xn);

            dataNodes = node.SelectNodes(XmlConsts.fmObjectRowTag);
        }

        private const string ERROR_ENTITY_NOT_FIND = "{0} '{1}' не найден в коллекции объектов схемы'";
        internal string Validate()
        {
            entity = null;
            try
            {
                IScheme scheme = pumpModule.Scheme;
                switch (objectType)
                {
                    case FmObjectsTypes.cls:
                        if (!scheme.Classifiers.ContainsKey(objectKey))
                            return String.Format(ERROR_ENTITY_NOT_FIND, "Классификатор", objectKey);
                        else
                            entity = scheme.Classifiers[objectKey];
                        break;
                    case FmObjectsTypes.fct:
                        if (!scheme.FactTables.ContainsKey(objectKey))
                            return String.Format(ERROR_ENTITY_NOT_FIND, "Таблица фактов", objectKey);
                        else
                            entity = scheme.FactTables[objectKey];
                        break;
                }
                return string.Empty;
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        internal void FillCache()
        {
            int count = dataset.Tables[0].Rows.Count;
            cache = new Dictionary<int, DataRow>(count);
            for (int i = 0; i < count; i++)
            {
                DataRow row = dataset.Tables[0].Rows[i];
                int key = Convert.ToInt32(row["Id"]);
                if (!cache.ContainsKey(key))
                    cache.Add(key, row);
            }
        }

        internal void QueryData()
        {
            switch (objectType)
            {
                case FmObjectsTypes.cls:
                    pumpModule.InitClsDataSet(ref adapter, ref dataset, (IClassifier)entity, false, string.Empty, dataSource.id);
                    break;
                case FmObjectsTypes.fct:
                    pumpModule.DirectDeleteFactData(new IFactTable[] { (IFactTable)entity }, -1, dataSource.id, deleteConstraint);
                    pumpModule.InitDataSet(ref adapter, ref dataset, (IFactTable)entity, string.Format(" SourceId = {0} ", dataSource.id));
                    break;
            }
            FillCache();
        }

        internal void CopyValuesToRow(DataRow row, object[] mapping)
        {
            for (int i = 0; i < mapping.GetLength(0) - 1; i += 2)
            {
                string value = mapping[i + 1].ToString().Trim();
                if (value == string.Empty)
                    row[mapping[i].ToString()] = DBNull.Value;
                else
                    row[mapping[i].ToString()] = value;
            }
        }

        internal DataRow PumpRow(DataTable dt, object[] mapping)
        {
            DataRow row = dt.NewRow();
            row["SOURCEID"] = dataSource.id;
            row["PUMPID"] = pumpModule.PumpID;
            CopyValuesToRow(row, mapping);
            dt.Rows.Add(row);
            return row;
        }

        internal void PumpData()
        {
            if (dataNodes == null)
                return;
            pumpModule.WriteToTrace(string.Format("Закачка данных объекта - {0}", entity.FullDBName),
                DataPumpModuleBase.TraceMessageKind.Information);
            foreach (XmlNode node in dataNodes)
            {
                int id = XmlHelper.GetIntAttrValue(node, "Id", -1);
                if (id == -1)
                    continue;
                if (cache.ContainsKey(id))
                    continue;
                
                // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!переделать этот аццкий вложенный цикл
                XmlAttributeCollection xac = node.Attributes;
                object[] mapping = new object[] {}; 
                foreach (XmlAttribute xa in xac)
                    mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { xa.Name, xa.Value });
                PumpRow(dataset.Tables[0], mapping);
            }
        }

        internal void UpdateData()
        {
            if (adapter == null)
                return;
            pumpModule.WriteToTrace(string.Format("Сохранение данных объекта - {0}", entity.FullDBName), 
                DataPumpModuleBase.TraceMessageKind.Information);
            pumpModule.UpdateDataSet(adapter, dataset, (IEntity)entity);
        }

    }
    #endregion

    #region FMObjectsCollection
    public sealed class FMObjectsCollection : ArrayList
    {
        internal CorrectedPumpModuleBase pumpModule = null;

        public FMObjectsCollection(CorrectedPumpModuleBase pumpModule)
        {
            this.pumpModule = pumpModule;
        }

        public void InitPumpModuleArrays()
        {
            List<string> clss = new List<string>();
            List<string> fcts = new List<string>();
            foreach (FMObject obj in this)
            {
                switch (obj.objectType)
                {
                    case FmObjectsTypes.cls:
                        clss.Add(obj.objectKey);
                        obj.entity = pumpModule.Scheme.Classifiers[obj.objectKey];
                        break;
                    case FmObjectsTypes.fct:
                        fcts.Add(obj.objectKey);
                        obj.entity = pumpModule.Scheme.FactTables[obj.objectKey];
                        break;
                }
            }
  //          pumpModule.UsedClassifiers = new IClassifier[clss.Count];
   //         for (int i = 0; i < clss.Count; i++)
    //            pumpModule.UsedClassifiers[i] = pumpModule.Scheme.Classifiers[clss[i]];
   //         pumpModule.UsedFacts = new IFactTable[fcts.Count];
    //        for (int i = 0; i < fcts.Count; i++)
     //           pumpModule.UsedFacts[i] = pumpModule.Scheme.FactTables[fcts[i]];
        }

        private static IEntity GetRefedClsByRefName(IEntity dataObj, string refName)
        {
            IAssociationCollection assCol = (IAssociationCollection)(dataObj).Associations;
            string fullName = String.Concat(dataObj.Name, ".", refName);
            IEntity refedCls = null;
            foreach (IAssociation item in assCol.Values)
                if (String.Compare(item.Name, fullName, true) == 0)
                {
                    refedCls = item.RoleBridge;
                    break;
                }
            return refedCls;
        }

        internal void QueryData()
        {
            foreach (FMObject obj in this)
                obj.QueryData();
        }

        internal void PumpData()
        {
            foreach (FMObject obj in this)
                obj.PumpData();
        }

        internal void UpdateData()
        {
            foreach (FMObject obj in this)
                obj.UpdateData();
        }

        internal void ClearData()
        {
            foreach (FMObject obj in this)
                obj.Clear();
            Clear();
        }

        public void LoadFromXml(XmlNode parentNode)
        {
            XmlNodeList childs = parentNode.SelectNodes(XmlConsts.fmObjectTag);
            foreach (XmlNode node in childs)
            {
                FMObject obj = new FMObject(pumpModule);
                obj.LoadFromXml(node);
                Add(obj);
            }
        }

    }
    #endregion

    #region XmlImporter
    public sealed class XmlImporter
    {
        internal FMObjectsCollection fmObjects;
        internal DataSource commonDataSource;
        internal CorrectedPumpModuleBase pumpModule = null;
        internal XmlDocument xmlDoc = null;

        public XmlImporter(CorrectedPumpModuleBase pumpModule)
        {
            this.pumpModule = pumpModule;
            fmObjects = new FMObjectsCollection(this.pumpModule);
            commonDataSource = new DataSource(this.pumpModule);
        }

        internal void LoadFromXml(FileInfo fi)
        {
            xmlDoc = new XmlDocument();
            xmlDoc.Load(fi.FullName);
            try
            {
                XmlNode xn = xmlDoc.DocumentElement.SelectSingleNode(XmlConsts.commonDataSourceNodeTag);
                commonDataSource.LoadFromXml(xn);

                xn = xmlDoc.DocumentElement.SelectSingleNode(XmlConsts.fmObjectsTag);
                fmObjects.LoadFromXml(xn);
            }
            finally
            {
            }
        }

        public void QueryData()
        {
            fmObjects.InitPumpModuleArrays();
            fmObjects.QueryData();
        }

        public void PumpData()
        {
            pumpModule.WriteToTrace("Закачка данных", DataPumpModuleBase.TraceMessageKind.Information);
            fmObjects.PumpData();
        }

        public void UpdateData()
        {
            pumpModule.WriteToTrace("Сохранение данных",  DataPumpModuleBase.TraceMessageKind.Information);
            fmObjects.UpdateData();
        }

        public void Clear()
        {
            XmlHelper.ClearDomDocument(ref xmlDoc);
            commonDataSource = null;
            fmObjects.ClearData();
        }

    }
    #endregion
}
