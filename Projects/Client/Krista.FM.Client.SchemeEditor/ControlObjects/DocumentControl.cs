using System;
using System.Windows.Forms;

using Krista.FM.Client.SMO.Design;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.SchemeEditor.ControlObjects
{
    public class DocumentControl : MajorObjectControl<SmoDocumentDesign>
    {
        public DocumentControl(IDocument controlObject, CustomTreeNodeControl parent)
            : base(new SmoDocumentDesign(controlObject), parent, (int)Images.File)
        {
        }

        public override string Caption
        {
            get { return ((IDocument)ControlObject).Name; }
        }

        public override void OnDoubleClick(Infragistics.Win.UltraWinTree.NodeEventArgs e)
        {
            SchemeEditor.OpenObject((ICommonDBObject)ControlObject);
        }

        [MenuAction("Удалить", Images.Remove, CheckMenuItemEnabling = "IsEditable")]
        public void Delete()
        {
            DocumentListControl documentList = (DocumentListControl)Parent;

            SmoDictionaryBaseDesign<string, IDocument> smoCollection = (SmoDictionaryBaseDesign<string, IDocument>)documentList.ControlObject;
            IModifiableCollection<string, IDocument> collection = (IModifiableCollection<string, IDocument>)smoCollection.ServerControl;

            if (MessageBox.Show(String.Format("Вы действительно хотите удалить документ \r {0} ?", this.Caption), "Удаление", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
            {
                collection.Remove(((IDocument)this.ControlObject).ObjectKey);
                OnChange(this, new EventArgs());
                this.Remove();
            }
        }
    }
}
