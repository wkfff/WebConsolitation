using System;
using System.Collections.Generic;
using System.ComponentModel;
using Krista.FM.ServerLibrary;
using Krista.FM.Common;


namespace Krista.FM.Client.SMO
{
    public class SmoEntity : SmoCommonDBObject, IEntity
    {
        public SmoEntity(IEntity serverObject)
            : base(serverObject)
        {
        }

        public SmoEntity(SMOSerializationInfo cache)
            : base(cache)
        {
        }

        #region IEntity Members

        [Browsable(false)]
        public IDataAttributeCollection Attributes
        {
            get
            {
                LogicalCallContextData context = LogicalCallContextData.GetContext();
                if (context[String.Format("{0}.Presentation", serverControl.FullDBName)] != null)
                    if (((IEntity)serverControl).Presentations.ContainsKey(Convert.ToString(context[String.Format("{0}.Presentation", serverControl.FullDBName)])))
                        return ((IEntity)serverControl).Presentations[Convert.ToString(context[String.Format("{0}.Presentation", serverControl.FullDBName)])].Attributes;

                return
                    cached
                        ? (IDataAttributeCollection)
                          SmoObjectsCache.GetSmoObject(typeof (SMODataAttributeCollection),
                                                       GetCachedObject("Attributes"))
                        :
                            ((IEntity) serverControl).Attributes;
            }
        }

        
        public ClassTypes ClassType
        {
            get { return cached ? (ClassTypes) GetCachedValue("ClassType") : ((IEntity) serverControl).ClassType; }
        }

        internal const string SubClassTypePropertyName = "SubClassType";

        public SubClassTypes SubClassType
        {
            get { return cached ? (SubClassTypes)GetCachedValue("SubClassType") : ((IEntity)serverControl).SubClassType; }
            set { ((IEntity)serverControl).SubClassType = value; CallOnChange(); }
        }

        public string ShortCaption
        {
            get { return cached ? (string)GetCachedValue("ShortCaption") : ((IEntity)serverControl).ShortCaption; }
            set { ((IEntity)serverControl).ShortCaption = value; CallOnChange(); }
        }

        public string OlapName
        {
            get { return cached ? (string)GetCachedValue("OlapName") : ((IEntity)serverControl).OlapName; }
        }

        public string FullCaption
        {
            get { return cached ? (string)GetCachedValue("FullCaption") : ((IEntity)serverControl).FullCaption; }
        }

        [Browsable(false)]
        public string MacroSet
        {
            get { return cached ? (string)GetCachedValue("MacroSet") : ((IEntity)serverControl).MacroSet; }
            set { ((IEntity)serverControl).MacroSet = value; CallOnChange(); }
        }

        [Browsable(false)]
        public IEntityAssociationCollection Associations
        {
            get
            {
                return
                    cached
                        ? (IEntityAssociationCollection)
                          SmoObjectsCache.GetSmoObject(typeof (SmoAssociationCollection),
                                                       GetCachedObject("Associations"))
                        : ((IEntity) serverControl).Associations;
            }
        }

        public IEntityAssociationCollection Associated
        {
            get {
                return
                    cached
                        ? (IEntityAssociationCollection)SmoObjectsCache.GetSmoObject(typeof(SmoAssociatedCollection),
                                                       GetCachedObject("Associated"))
                        : ((IEntity) serverControl).Associated; }
        }

        public string GeneratorName
        {
            get { return cached ? (string)GetCachedValue("GeneratorName") : ((IEntity)serverControl).GeneratorName; }
        }

        public string DeveloperGeneratorName
        {
            get { return cached ? (string)GetCachedValue("DeveloperGeneratorName") : ((IEntity)serverControl).DeveloperGeneratorName; }
        }

        public int GetGeneratorNextValue
        {
            get { return ((IEntity)serverControl).GetGeneratorNextValue; }
        }

        public IDataUpdater GetDataUpdater()
        {
            return ((IEntity)serverControl).GetDataUpdater();
        }

        public IDataUpdater GetDataUpdater(string selectFilter, int? maxRecordCountInSelect, params System.Data.IDbDataParameter[] selectFilterParameters)
        {
            return ((IEntity)serverControl).GetDataUpdater(selectFilter, maxRecordCountInSelect, selectFilterParameters);
        }

        public int DeleteData(string whereClause, params object[] parameters)
        {
            return ((IEntity)serverControl).DeleteData(whereClause, parameters);
        }

        public int DeleteData(string whereClause, bool disableTriggerAudit, params object[] parameters)
        {
            return ((IEntity)serverControl).DeleteData(whereClause, disableTriggerAudit, parameters);
        }

        public int RecordsCount(int sourceID)
        {
            return ((IEntity)serverControl).RecordsCount(sourceID);
        }

        public System.Data.DataSet GetDependedData(int rowID, bool recursive)
        {
            return ((IEntity)serverControl).GetDependedData(rowID, recursive);
        }

        public bool ProcessObjectData()
        {
            return ((IEntity)serverControl).ProcessObjectData();
        }

        public void MergingDuplicates(int mainRecordID, List<int> duplicateRecordID, MergeDuplicatesListener listener)
        {
			((IEntity)serverControl).MergingDuplicates(mainRecordID, duplicateRecordID, listener);
		}

        public string GetObjectType()
        {
            return ((IEntity)serverControl).GetObjectType();
        }

        /// <summary>
        /// Коллекция представлений
        /// </summary>
        public IPresentationCollection Presentations
        {
            get
            {
                return cached ?
                    (IPresentationCollection)SmoObjectsCache.GetSmoObject(typeof(SmoPresentationCollection), 
                    GetCachedObject("Presentations")) :
                            ((IEntity)ServerControl).Presentations;
                    
            }
        }

        public IDataAttributeCollection GroupedAttributes
        {
            get { return ((IEntity)serverControl).GroupedAttributes; }
        }


        /// <summary>
        /// Коллекция уникальных ключей
        /// </summary>
        public IUniqueKeyCollection UniqueKeys
        {
            get { return ((IEntity) serverControl).UniqueKeys; }
        }

        public bool UniqueKeyAvailable
        {
            get { //return ((IEntity)serverControl).UniqueKeyAvailable;
                  return cached ? (bool)GetCachedValue("UniqueKeyAvailable") : ((IEntity)serverControl).UniqueKeyAvailable;
                }
        }

        #endregion
    }

    [ReadOnly(true)]
    public class SmoEntityReadOnly : SmoEntity, IEntity
    {
        public SmoEntityReadOnly(IEntity serverObject)
            : base(serverObject)
        {
        }
    }
}
