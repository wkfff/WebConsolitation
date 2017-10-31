using System;
using System.Collections.Generic;
using Krista.FM.Client.SMO.Design;
using Krista.FM.ServerLibrary;
using Krista.FM.Client.SchemeEditor.Gui.Wizard;

namespace Krista.FM.Client.SchemeEditor.ControlObjects
{
    public class PresentationListControl : ModifiableListControl<SmoPresentationCollectionDesign, IPresentation>
    {
        public PresentationListControl(IPresentationCollection controlObject, CustomTreeNodeControl parent)
            : base("IPresentationCollection", "Представления",
            new SmoPresentationCollectionDesign(controlObject), parent, (int)Images.Views)
        {
        }

        /// <summary>
        /// Создание нового представления
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override CustomTreeNodeControl Create(IPresentation item)
        {
            return new PresentationControl(item, this);
        }

        [MenuAction("Создать представление", Images.CreateAttribute, CheckMenuItemEnabling = "IsEditable")]
        public override void AddNew()
        {
            string name;
            string levelNamingTemplate;
            List<IDataAttribute> attributes;

            SmoDictionaryBaseDesign<string, IPresentation> smoCollection = (SmoDictionaryBaseDesign<string, IPresentation>)ControlObject;
            IPresentationCollection collection = (IPresentationCollection)smoCollection.ServerControl;
            if (CreatePresentationFormWizard.ShowCreatePresentationDialog((IEntity)collection.OwnerObject, out name, out attributes, out levelNamingTemplate))
            {
                IPresentation newObj = collection.CreateItem(Guid.NewGuid().ToString(), name, attributes, levelNamingTemplate);
                CustomTreeNodeControl node = new PresentationControl(new SmoPresentationDesign(newObj), this);
                Nodes.Add(node);
                OnChange(this, new EventArgs());
            }
        }

        [Obsolete()]
        protected override void ExpandNode()
        {
            StartExpand();

            base.ExpandNode();

            EndExpand();
        }
    }
}
