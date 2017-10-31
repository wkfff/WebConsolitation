using System;
using System.Windows.Forms;
using Krista.FM.Client.SMO.Design;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.SchemeEditor.ControlObjects
{
    public abstract class ModifiableListControl<TIntf, TItem> : CustomListControl<TIntf, TItem> where TIntf : IServerSideObject
    {
        public ModifiableListControl(string name, string text, TIntf controlObject, CustomTreeNodeControl parent, int imageIndex)
            : base(name, text, controlObject, parent, imageIndex)
        {
        }

        [MenuAction("Создать")]
        public virtual void AddNew()
        {
            SmoDictionaryBaseDesign<string, TItem> smoCollection = (SmoDictionaryBaseDesign<string, TItem>)ControlObject;
            IModifiableCollection<string, TItem> collection = (IModifiableCollection<string, TItem>)smoCollection.ServerControl;
            TItem newObj = collection.CreateItem();
            CustomTreeNodeControl node = Create(newObj);
            Nodes.Add(node);
            OnChange(this, new EventArgs());

            SelectNewObject(node);
        }

        public void SelectNewObject(CustomTreeNodeControl node)
        {
            this.SchemeEditor.SelectObject(node.Key, true);
        }
    }
}
