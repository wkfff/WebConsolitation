using System;
using System.Collections.Generic;
using System.Text;
using Krista.FM.Client.SMO.Design;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.SchemeEditor.ControlObjects
{
    public class DocumentListControl : ModifiableListControl<SmoDictionaryBaseDesign<string, IDocument>, IDocument>
    {
        public DocumentListControl(IDocumentCollection controlObject, CustomTreeNodeControl parent)
            : base("IDocumentsCollection", "Диаграммы", new SmoDictionaryBaseDesign<string, IDocument>(controlObject), parent, (int)Images.Documents)
        {
        }

        public override CustomTreeNodeControl Create(IDocument item)
        {
            return new DocumentControl(item, this);
        }

        [MenuAction("Создать диаграмму классов", Images.CreateDocument, CheckMenuItemEnabling = "IsEditable")]
        public override void AddNew()
        {
            base.AddNew();
        }

		[MenuAction("Создать диаграмму документов", Images.CreateDocument, CheckMenuItemEnabling = "IsEditable")]
		public void AddNewDocumentEntityDiagram()
		{
			SmoDictionaryBaseDesign<string, IDocument> smoCollection = (SmoDictionaryBaseDesign<string, IDocument>)ControlObject;
            IModifiableCollection<string, IDocument> collection = (IModifiableCollection<string, IDocument>)smoCollection.ServerControl;
			IDocument newObj = ((IDocumentCollection)collection).CreateItem(DocumentTypes.DocumentEntityDiagram);
			CustomTreeNodeControl node = Create(newObj);
			Nodes.Add(node);
			OnChange(this, new EventArgs());

			SelectNewObject(node);
		}
	}
}
