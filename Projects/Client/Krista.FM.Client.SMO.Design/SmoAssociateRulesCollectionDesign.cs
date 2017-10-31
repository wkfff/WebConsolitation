using System;
using System.ComponentModel;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO.Design
{
    public class SmoAssociateRulesCollectionDesign : SmoDictionaryBaseDesign<string, IAssociateRule>, IAssociateRuleCollection
    {
        public SmoAssociateRulesCollectionDesign(IDictionaryBase<string, IAssociateRule> serverControl)
            : base(serverControl)
        {
        }

        protected override Type GetItemValueSmoObjectType(object obj)
        {
            return typeof(SmoAssociateRuleDesign);
        }

        [DisplayName("Правило по умолчанию")]
        [Description("Правило сопоставления, использующееся по умолчанию.")]
        [TypeConverter(typeof(AssociateRulesConverter))]
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
