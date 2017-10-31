using System;
using System.ComponentModel;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO.Design
{
    public class SmoBridgeAssociationDesign : SmoAssociationDesign, IBridgeAssociation
    {
        public SmoBridgeAssociationDesign(IEntityAssociation serverControl)
            : base(serverControl)
        {
        }

        [Browsable(false)]
        public new IBridgeAssociation ServerControl
        {
            get { return (IBridgeAssociation)serverControl; }
        }

        #region IBridgeAssociation Members

        public int GetCountBridgeRow(int sourceID)
        {
            return ((IBridgeAssociation)serverControl).GetCountBridgeRow(sourceID);
        }

        public int ReplaceLinkToNewVersionCls(int sourceID, int pumpID, int oldSourceID)
        {
            return ((IBridgeAssociation)serverControl).ReplaceLinkToNewVersionCls(sourceID, pumpID, oldSourceID);
        }

        [Browsable(false)]
        public bool MappingRuleExist
        {
            get { return ServerControl.MappingRuleExist; }
        }

        [Browsable(false)]
        public IAssociateMappingCollection Mappings
        {
            get
            {
                return
                    ServerControl.Mappings;
            }
        }

        [Browsable(false)]
        public IAssociateRuleCollection AssociateRules
        {
            get
            {
                return
                    ServerControl.AssociateRules;
            }
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

    [ReadOnly(true)]
    public class SmoBridgeAssociationReadOnlyDesign : SmoBridgeAssociationDesign
    {
        public SmoBridgeAssociationReadOnlyDesign(IEntityAssociation serverControl)
            : base(serverControl)
        {
        }
    }
}
