using System;
using System.Collections.Generic;
using System.Text;
using Krista.FM.Client.SMO.Design;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SchemeEditor.ControlObjects
{
    public class AssociateMappingListControl : CustomListControl<IAssociateMappingCollection, IAssociateMapping>
    {
        public AssociateMappingListControl(IAssociateMappingCollection controlObject, CustomTreeNodeControl parent)
            : base("AssociateMappingList", "Правила формирования", controlObject, parent, 47)
        {
        }

        public override CustomTreeNodeControl Create(IAssociateMapping item)
        {
            return new AssociateMappingControl(new SmoAssociateMappingDesign(item), this);
        }

        [MenuAction("Создать новое правило", Images.CreateAssociateMapping, CheckMenuItemEnabling = "IsEditable")]
        public void AddNew()
        {
            IAssociateMappingCollection collection = (IAssociateMappingCollection)ControlObject;
            IAssociateMapping newObj = collection.CreateItem();
            CustomTreeNodeControl node = Create(newObj);
            Nodes.Add(node);
            OnChange(this, new EventArgs());

            this.SchemeEditor.SelectObject(node.Key, true);
        }
    }
}
