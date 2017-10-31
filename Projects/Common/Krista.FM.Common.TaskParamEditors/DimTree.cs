using System;
using System.Windows.Forms;
using System.Xml;
using Krista.FM.Common.Xml;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Common.TaskParamEditors
{
    internal partial class DimTree : UserControl
    {

        internal DimTree()
        {
            InitializeComponent();
        }

        internal string LastError { get; set; }

        internal string GetSelectedDimensionName()
        {
            if (treeView1.SelectedNode == null)
                return string.Empty;
            TreeNode node = treeView1.SelectedNode;
            while (node.FirstNode != null)
                node = node.FirstNode;
            return (string) node.Tag;
        }

        internal new void Load(IScheme scheme, string selectedDimensionName)
        {
            LastError = string.Empty;
            XmlDocument document = LoadMetadata(scheme);
            if (document == null)
                return;
            LoadFromXml(document);
            LookupSelection(treeView1.Nodes, selectedDimensionName);
        }

        private void LoadFromXml(XmlDocument document)
        {
            if (document == null)
                return;
            treeView1.BeginUpdate();
            try
            {
                treeView1.Nodes.Clear();

                XmlNodeList dimensions = document.SelectNodes("function_result/SharedDimensions/Dimension");
                if (dimensions == null)
                    return;

                foreach (XmlNode dimNode in dimensions)
                    LoadDimension(dimNode);

                treeView1.Sort();
            }
            finally
            {
                treeView1.EndUpdate();
            }
        }

        private void LoadDimension(XmlNode dimNode)
        {
            if (dimNode == null)
                return;
            XmlNodeList hierarchies = dimNode.SelectNodes("Hierarchy");
            if (hierarchies == null)
                return;

            string fullDimName = XmlHelper.GetStringAttrValue(dimNode, Attr.Name, string.Empty);
            string firstName;
            string lastName;
            Utils.ParseDimensionName(fullDimName, out firstName, out lastName, "__");

            TreeNode node;
            if (treeView1.Nodes.ContainsKey(firstName))
            {
                node = treeView1.Nodes.Find(firstName, false)[0];
            }
            else
            {
                node = treeView1.Nodes.Add(firstName, firstName, "Dimension.bmp", "Dimension.bmp");
            }

            if (lastName != string.Empty)
                node = node.Nodes.Add(lastName, lastName, "Dimension.bmp", "Dimension.bmp");

            foreach (XmlNode hierNode in hierarchies)
            {
                string hierName = XmlHelper.GetStringAttrValue(hierNode, Attr.Name, "Иерархия по умолчанию");
                string fullDimHierName = hierName != "Иерархия по умолчанию"
                                             ? string.Format("{0}.{1}", fullDimName, hierName)
                                             : fullDimName;
                node.Nodes.Add(hierName, hierName, "Hierarchy.bmp", "Hierarchy.bmp").Tag = fullDimHierName;
            }

        }

        private bool LookupSelection(TreeNodeCollection nodes, string selectedDimensionName)
        {
            foreach (TreeNode treeNode in nodes)
            {
                if ((treeNode.Tag != null) && (treeNode.Tag.ToString() == selectedDimensionName))
                {
                    treeView1.SelectedNode = treeNode;
                    return true;
                }
                if (LookupSelection(treeNode.Nodes, selectedDimensionName))
                    return true;
            }
            return false;
        }

        private XmlDocument LoadMetadata(IScheme scheme)
        {
            if (scheme == null)
                return null;

            string metadata = scheme.PlaningProvider.GetMetaData();
            if (metadata == string.Empty)
                return null;
            XmlDocument document = new XmlDocument();
            try
            {
                document.LoadXml(metadata);
            }
            catch (Exception e)
            {
                LastError = Diagnostics.KristaDiagnostics.ExpandException(e);
                return null;
            }
            return document;
        }
    }

}
