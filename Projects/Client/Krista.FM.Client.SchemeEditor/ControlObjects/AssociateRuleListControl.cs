using Krista.FM.Client.SMO.Design;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SchemeEditor.ControlObjects
{
    public class AssociateRuleListControl : ModifiableListControl<SmoDictionaryBaseDesign<string, IAssociateRule>, IAssociateRule>
    {
        public AssociateRuleListControl(IAssociateRuleCollection controlObject, CustomTreeNodeControl parent)
            : base("IAssociateRuleCollection", "Правила сопоставления", new SmoAssociateRulesCollectionDesign(controlObject), parent, 49)
        {
        }

        public override CustomTreeNodeControl Create(IAssociateRule item)
        {
            return new AssociateRuleMappingListControl(new SmoAssociateRuleDesign(item), this);
        }

        [MenuAction("Создать правило сопоставления", Images.CreateAssociateRuleMapping, CheckMenuItemEnabling = "IsEditable")]
        public override void AddNew()
        {
            base.AddNew();
        } 
    }
}
