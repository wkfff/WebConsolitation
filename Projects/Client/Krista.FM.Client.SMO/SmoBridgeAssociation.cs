using System;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO
{
    public class SmoBridgeAssociation : SmoAssociation, IBridgeAssociation
    {
        public SmoBridgeAssociation(IEntityAssociation serverObject)
            : base(serverObject)
        {
        }


        public SmoBridgeAssociation(SMOSerializationInfo cache) 
            : base(cache)
        {
        }

        public new IBridgeAssociation ServerControl
        {
            get { return (IBridgeAssociation)serverControl; }
        }

        #region IBridgeAssociation Members

        public int GetCountBridgeRow(int sourceID)
        {
            return ((IBridgeAssociation) serverControl).GetCountBridgeRow(sourceID);
        }

        public int ReplaceLinkToNewVersionCls(int sourceID, int pumpID, int oldSourceID)
        {
            return ((IBridgeAssociation)serverControl).ReplaceLinkToNewVersionCls(sourceID, pumpID, oldSourceID);
        }

        public bool MappingRuleExist
        {
            get { return cached ? (bool)GetCachedValue("MappingRuleExist") : ServerControl.MappingRuleExist; }
        }

        public IAssociateMappingCollection Mappings
        {
            get {
                return
                    cached
                        ? (IAssociateMappingCollection)
                          SmoObjectsCache.GetSmoObject(typeof (SMOAssociateMappingCollection),
                                                       GetCachedObject("Mappings"))
                        : ServerControl.Mappings; }
        }

        public IAssociateRuleCollection AssociateRules
        {
            get {
                return
                    cached
                        ? (IAssociateRuleCollection)
                          SmoObjectsCache.GetSmoObject(typeof (SmoAssociateRulesCollection),
                                                       GetCachedObject("AssociateRules"))
                        : ServerControl.AssociateRules; }
        }

		public string GetDefaultAssociateRule()
		{
			return ServerControl.GetDefaultAssociateRule();
		}

		public void SetDefaultAssociateRule(string key)
		{
			ServerControl.SetDefaultAssociateRule(key);
		}

        public int GetAssociateBridgeSourceID(IDatabase db, int dataClsSourceID)
        {
            return ServerControl.GetAssociateBridgeSourceID(db, dataClsSourceID);
        }

        #endregion
	}

    public class SmoBridgeAssociationReadOnly : SmoBridgeAssociation
    {
        public SmoBridgeAssociationReadOnly(IEntityAssociation serverObject)
            : base(serverObject)
        {
        }
    }

    public class SmoBridgeAssociationReport : SmoBridgeAssociation, IBridgeAssociationReport
    {
         public SmoBridgeAssociationReport(IEntityAssociation serverObject)
            : base(serverObject)
        {
        }


        public SmoBridgeAssociationReport(SMOSerializationInfo cache) 
            : base(cache)
        {
        }

        #region IBridgeAssociationReport Members

        public void CopyAndAssociateAllRow(int sourceID, int bridgeSourceID)
        {
            ((IBridgeAssociationReport)serverControl).CopyAndAssociateAllRow(sourceID, bridgeSourceID);
        }

        #endregion
    }
}
