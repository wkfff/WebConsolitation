using System;
using System.Collections.Generic;
using System.ComponentModel;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO
{
    public class SmoAssociation : SmoCommonDBObject, IAssociation
    {
        public SmoAssociation(IEntityAssociation serverObject)
            : base(serverObject)
        {
        }


        public SmoAssociation(SMOSerializationInfo cache) 
            : base(cache)
        {
        }

        public static SmoAssociation CreateInstance(IEntityAssociation serverObject)
        {
            if (serverObject is IBridgeClassifier)
            {
                if (serverObject is IBridgeAssociationReport)
                    return new SmoBridgeAssociationReport(serverObject);

                return new SmoBridgeAssociation(serverObject);
            }

            return new SmoAssociation(serverObject);
        }

        [Browsable(false)]
        public new IEntityAssociation ServerControl
        {
            get { return (IEntityAssociation)serverControl; }
        }

        public override string Name
        {
            get { return base.Name; }
            set { base.Name = value; }
        }

        public override string Semantic
        {
            get { return base.Semantic; }
            set { base.Semantic = value; }
        }

        #region IAssociation Members

        [Browsable(false)]
        public IEntity RoleData
        {
            get
            {
                Type objectType = null;
                if (ServerControl.RoleBridge is IVariantDataClassifier)
                    objectType = typeof (SmoVariantDataClassifier);
                else
                    objectType = (ServerControl.RoleBridge is IBridgeClassifier)
                                              ? typeof(SmoBridgeClassifier)
                                              : typeof(SmoClassifier);
                return
                    cached
                        ? (IEntity)SmoObjectsCache.GetSmoObject(objectType, ServerControl.RoleData)
                        : ServerControl.RoleData;
            }
        }

        [Browsable(false)]
        public IEntity RoleBridge
        {
            get
            {
                Type objectType = null;
                if (ServerControl.RoleBridge is IVariantDataClassifier)
                    objectType = typeof(SmoVariantDataClassifier);
                else
                    objectType = (ServerControl.RoleBridge is IBridgeClassifier)
                                              ? typeof(SmoBridgeClassifier)
                                              : typeof(SmoClassifier);
                return cached
                    ? (IEntity)SmoObjectsCache.GetSmoObject(objectType, ServerControl.RoleBridge)
                    : ServerControl.RoleBridge;
            }
        }

        public int Associate()
        {
            return ((IBridgeAssociation) ServerControl).Associate();
        }

        public int Associate(int dataClsSourceID, int bridgeClsSourceID)
        {
            return ((IBridgeAssociation)ServerControl).Associate(dataClsSourceID, bridgeClsSourceID);
        }

        public int Associate(int dataClsSourceId, int bridgeClsSourceID, int pumpID)
        {
            return ((IBridgeAssociation)ServerControl).Associate(dataClsSourceId, bridgeClsSourceID, pumpID);
        }

        public int Associate(int dataClsSourceId, int bridgeClsSourceID, int pumpID, bool allowDigits, bool reAssociate)
        {
            return ((IBridgeAssociation)ServerControl).Associate(dataClsSourceId, bridgeClsSourceID, pumpID, allowDigits, reAssociate);
        }

        public int Associate(int dataClsSourceId, int bridgeClsSourceID, IAssociateRule associateRule)
        {
            return ((IBridgeAssociation)ServerControl).Associate(dataClsSourceId, bridgeClsSourceID, associateRule);
        }

        public int Associate(int dataClsSourceId, int bridgeClsSourceID, string associateRuleObjectKey, StringElephanterSettings stringSettings, AssociationRuleParams ruleParams)
        {
            return ((IBridgeAssociation) ServerControl).Associate(dataClsSourceId, bridgeClsSourceID, associateRuleObjectKey, stringSettings,
                                                                  ruleParams);
        }

        public int FormBridgeClassifier(int dataSourceID, int bridgeSourceID)
        {
            return ((IBridgeAssociation)ServerControl).FormBridgeClassifier(dataSourceID, bridgeSourceID);
        }

        public int CopyAndAssociateRow(int rowID, int bridgeSourceID)
        {
            return ((IBridgeAssociation)ServerControl).CopyAndAssociateRow(rowID, bridgeSourceID);
        }

        public int CopyAndAssociateRow(int rowID, int parentId, int bridgeSourceID)
        {
            return ((IBridgeAssociation)ServerControl).CopyAndAssociateRow(rowID, parentId, bridgeSourceID);
        }

        public void ClearAssociationReference(int sourceID)
        {
            ((IBridgeAssociation)ServerControl).ClearAssociationReference(sourceID);
        }

        public int GetAllRecordsCount()
        {
            return ((IBridgeAssociation) ServerControl).GetAllRecordsCount();
        }

        public int GetAllUnassociateRecordsCount()
        {
            return ((IBridgeAssociation)ServerControl).GetAllUnassociateRecordsCount();
        }

        public int GetRecordsCountByCurrentDataSource(int sourceID)
        {
            return ((IBridgeAssociation) ServerControl).GetRecordsCountByCurrentDataSource(sourceID);
        }

        public int GetUnassociateRecordsCountByCurrentDataSource(int sourceID)
        {
            return ((IBridgeAssociation)ServerControl).GetUnassociateRecordsCountByCurrentDataSource(sourceID);
        }

        public void RefreshRecordsCount()
        {
            ((IBridgeAssociation) ServerControl).RefreshRecordsCount();
        }

        #endregion

        public virtual AssociationClassTypes AssociationClassType
        {
            get {
                return
                    cached
                        ? (AssociationClassTypes)GetCachedValue("AssociationClassType")
                        : ServerControl.AssociationClassType; }
        }

        public virtual string FullCaption
        {
            get { return cached ? (string)GetCachedValue("FullCaption") : ServerControl.FullCaption; }
        }

        [Browsable(false)]
        public IDataAttribute RoleDataAttribute
        {
            get {
                return
                    cached
                        ? (IDataAttribute)
                          SmoObjectsCache.GetSmoObject(typeof (SmoAttribute), ServerControl.RoleDataAttribute)
                        : ServerControl.RoleDataAttribute; }
        }
    }

    public class SmoAssociationReadOnly : SmoAssociation
    {
        public SmoAssociationReadOnly(IEntityAssociation serverObject)
            : base(serverObject)
        {
        }

        public new static SmoAssociation CreateInstance(IEntityAssociation serverObject)
        {
            if (serverObject is IBridgeClassifier)
                return new SmoBridgeAssociationReadOnly(serverObject);
            else
                return new SmoAssociationReadOnly(serverObject);
        }
    }
}
