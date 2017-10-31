using System;
using System.ComponentModel;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO
{
    public class SmoAssociateRulesCollection : SmoDictionaryBase<string, IAssociateRule>, IAssociateRuleCollection
    {
        public SmoAssociateRulesCollection(IDictionaryBase<string, IAssociateRule> serverObject)
            : base(serverObject)
        {
        }


        public SmoAssociateRulesCollection(SMOSerializationInfo cache) 
            : base(cache)
        {
        }

        protected override Type GetItemValueSmoObjectType(object obj)
        {
            return typeof(SmoAssociateRule);
        }

        public string AssociateRuleDefault
        {
            get { return ((IAssociateRuleCollection)serverControl).AssociateRuleDefault; }
            set
            {
                if (value == "Правило не назначено")
                {
                    value = "";
                }
                ((IAssociateRuleCollection)serverControl).AssociateRuleDefault = value; CallOnChange();
            }
        }

        [Browsable(false)]
        public ICommonDBObject Parent
        {
            get { return ((ICommonDBObject)serverControl.OwnerObject); }
        }
    }
}