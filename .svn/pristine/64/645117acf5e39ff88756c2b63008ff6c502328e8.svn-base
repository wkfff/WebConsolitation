using System;
using System.Collections.Generic;
using System.Text;
using Krista.FM.Client.SMO.Design;
using Krista.FM.ServerLibrary;
using System.Windows.Forms;

namespace Krista.FM.Client.SchemeEditor.ControlObjects
{
    public class PresentationControl : MinorObjectControl<SmoPresentationDesign>
    {
        public PresentationControl(IPresentation controlObject, CustomTreeNodeControl parent)
            : base(controlObject.ObjectKey, String.Format("{0}", controlObject.Name),
                    new SmoPresentationDesign(controlObject), parent, (int)Images.View)
        {
            AddLoadNode();
        }

        /// <summary>
        /// Добавляет дочерние элементы 
        /// </summary>
        protected override void ExpandNode()
        {
            base.ExpandNode();

            IPresentation presentation = (IPresentation)ControlObject;

            AttributePresentationListControl attributeListControl = new AttributePresentationListControl(presentation.Attributes, this);
            Nodes.Add(attributeListControl);

            attributeListControl.Expanded = true;

            attributeListControl.Refresh();
        }

        [MenuAction("Удалить", Images.Remove)]
        public void Delete()
        {
            PresentationListControl presentationList = (PresentationListControl)Parent;

            SmoDictionaryBaseDesign<string, IPresentation> smoCollection = (SmoDictionaryBaseDesign<string, IPresentation>)presentationList.ControlObject;
            IModifiableCollection<string, IPresentation> collection = (IModifiableCollection<string, IPresentation>)smoCollection.ServerControl;

            if (MessageBox.Show(String.Format("Вы действительно хотите удалить представление \r {0} ?", this.Caption), "Удаление", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
            {
                collection.Remove(((IPresentation)this.ControlObject).ObjectKey);
                OnChange(this, new EventArgs());
                this.Remove();
            }
        }
    }
}
