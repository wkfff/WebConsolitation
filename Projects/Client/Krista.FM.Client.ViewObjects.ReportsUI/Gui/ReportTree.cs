using System.Linq;
using System.Windows.Forms;
using Infragistics.Win.UltraWinTree;
using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.TemplatesService;

namespace Krista.FM.Client.ViewObjects.ReportsUI.Gui
{
    public partial class ReportTree : UserControl
    {
        public ReportTree()
        {
            InitializeComponent();
        }

        public void BuildTree(ITree<ITemplateReport> tree)
        {
            foreach (var parentNode in tree.Nodes.Values.Where(w => w.Parent == null))
            {
                UltraTreeNode node = tReports.Nodes.Add(parentNode.DataValue.Key, parentNode.DataValue.Caption);
                node.DataKey = parentNode.DataValue;
                PrepareTreeNode(node, parentNode.DataValue.DocumentType);
                CreateTreeStructure(node, parentNode);
            }
        }

        private static void CreateTreeStructure(UltraTreeNode parentNode, ITreeNode<ITemplateReport> parentDataNode)
        {
            foreach (var childDataNode in parentDataNode.Nodes.Values)
            {
                UltraTreeNode childNode = parentNode.Nodes.Add(childDataNode.DataValue.Key, childDataNode.DataValue.Caption);
                childNode.DataKey = childDataNode.DataValue;
                PrepareTreeNode(childNode, childDataNode.DataValue.DocumentType);
                CreateTreeStructure(childNode, childDataNode);
            }
        }

        /// <summary>
        /// настраиваем узел в дереве в зависимости от элемента репозитория, который он представляет
        /// </summary>
        /// <param name="node"></param>
        /// <param name="docType"></param>
        private static void PrepareTreeNode(UltraTreeNode node, TemplateDocumentTypes docType)
        {
            switch (docType)
            {
                case TemplateDocumentTypes.Group:
                    node.LeftImages.Add(Properties.Resources.Folder_icon);
                    break;
                case TemplateDocumentTypes.Arbitrary:
                    node.LeftImages.Add(Properties.Resources.Document_Blank_icon);
                    break;
                case TemplateDocumentTypes.MDXExpert:
                case TemplateDocumentTypes.MDXExpert3:
                    node.LeftImages.Add(Properties.Resources.Mdx_Expert_icon);
                    break;
                case TemplateDocumentTypes.MSWord:
                case TemplateDocumentTypes.MSWordPlaning:
                case TemplateDocumentTypes.MSWordTemplate:
                    node.LeftImages.Add(Properties.Resources.Document_Microsoft_Word_icon);
                    break;
                case TemplateDocumentTypes.MSExcel:
                case TemplateDocumentTypes.MSExcelPlaning:
                case TemplateDocumentTypes.MSExcelTemplate:
                    node.LeftImages.Add(Properties.Resources.Document_Microsoft_Excel_icon);
                    break;
            }
            node.Tag = docType;
        }
    }
}
