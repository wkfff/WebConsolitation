using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using Infragistics.Win.UltraWinTree;
using Krista.FM.Utils.MakeConfigurationLibrary;

namespace Krista.FM.Utils.MakeConfiguratuionGUI
{
    public partial class MakeConfigurationUI : Form
    {
        ConfigurationManager manager = new ConfigurationManager(AppDomain.CurrentDomain.BaseDirectory);
        private PackageConfig packageConfig;
        private string value;

        public MakeConfigurationUI()
        {
            InitializeComponent();

            InitializeTree();
        }

        private void InitializeTree()
        {
            ultraTree.EventManager.SetEnabled(EventGroups.AllEvents, true);

            ultraTree.AfterCheck += ultraTree_AfterCheck;

            packageConfig = manager.GetRootPackage();
            var node = new UltraTreeNode(packageConfig.Name)
                                     {
                                         Override = {NodeStyle = NodeStyle.CheckBox},
                                         CheckedState = CheckState.Checked,
                                         Tag = packageConfig
                                     };

            HandlePackage(packageConfig, node);
            node.Expanded = true;

            ultraTree.Nodes.Add(node);
        }

        void ultraTree_AfterCheck(object sender, NodeEventArgs e)
        {
            ultraTree.EventManager.SetEnabled(EventGroups.AllEvents, false);

            UltraTreeNode node = e.TreeNode;

            SetTagNode(node);

            SetChildNodes(node);

            if (node.Parent != null)
                CheckParentNode(node.Parent);

            ultraTree.EventManager.SetEnabled(EventGroups.AllEvents, true);
        }

        private static void SetChildNodes(UltraTreeNode node)
        {
            if (node.HasNodes)
            {
                foreach (var node1 in node.Nodes)
                {
                    node1.CheckedState = node.CheckedState;
                    SetTagNode(node1);
                    SetChildNodes(node1);
                }
            }
        }

        private static void SetTagNode(UltraTreeNode node)
        {
            ((BaseObject) node.Tag).IsContain = node.CheckedState == CheckState.Checked ||
                                                node.CheckedState == CheckState.Indeterminate
                                                    ? true
                                                    : false;
        }

        private static void CheckParentNode(UltraTreeNode ultraTreeNode)
        {
            if ((ultraTreeNode.Nodes.Cast<UltraTreeNode>().Where(node => node.CheckedState == CheckState.Checked).Count() > 0 &&
                ultraTreeNode.Nodes.Cast<UltraTreeNode>().Where(node => node.CheckedState == CheckState.Unchecked).Count() > 0) ||
                ultraTreeNode.Nodes.Cast<UltraTreeNode>().Where(node => node.CheckedState == CheckState.Indeterminate).Count() > 0)
                    ultraTreeNode.CheckedState = CheckState.Indeterminate;
            else if (ultraTreeNode.Nodes.Cast<UltraTreeNode>().Where(node => node.CheckedState == CheckState.Checked).Count() > 0)
                ultraTreeNode.CheckedState = CheckState.Checked;
            else
                ultraTreeNode.CheckedState = CheckState.Unchecked;
            
            SetTagNode(ultraTreeNode);

            if (ultraTreeNode.Parent != null)
                CheckParentNode(ultraTreeNode.Parent);
        }

        private static void HandlePackage(PackageConfig packageConfig, UltraTreeNode rootNode)
        {
            foreach (var package in packageConfig.Packages)
            {
                var node = new UltraTreeNode(package.Name)
                                         {
                                             Tag = package,
                                             Override = {NodeStyle = NodeStyle.CheckBox},
                                             CheckedState = CheckState.Checked
                                         };
                rootNode.Nodes.Add(node);

                HandlePackage(package, node);
            }

            foreach (var itemConfig in packageConfig.Items)
            {
                var node = new UltraTreeNode(itemConfig.Name)
                                         {
                                             Tag = itemConfig,
                                             Override = {NodeStyle = NodeStyle.CheckBox},
                                             CheckedState = CheckState.Checked
                                         };
                rootNode.Nodes.Add(node);
            }
        }

        private void closeForm_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void saveConfiguration_Click(object sender, EventArgs e)
        {
            manager.MakeConfiguration(AppDomain.CurrentDomain.BaseDirectory, packageConfig);

            MessageBox.Show("Конфигурация сохрнена", "Конфигурация сохрнена", MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
        }

        private void ultraToolbarsManager1_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "Open SchemeConfiguration File...":
                    var openFileDialog = new OpenFileDialog
                                                        {
                                                            InitialDirectory =
                                                                "x:\\DotNet\\Repository\\_Подопытная\\Packages",
                                                            Filter = "txt files (*.xml)|*.xml|All files (*.*)|*.*",
                                                            RestoreDirectory = true
                                                        };
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        XDocument document = XDocument.Load(openFileDialog.FileName);
                        AnalisysSchemeConfiguration(document, openFileDialog.InitialDirectory);
                    }
                    break;
            }
        }

        private void AnalisysSchemeConfiguration(XDocument document, string pathNewDoc)
        {
            if (!pathNewDoc.Contains(".xml"))
            {
                if (document.Root != null)
                    HandlePackagesNode(pathNewDoc, document);    
            }
            else
            {
                XDocument package = XDocument.Load(pathNewDoc);

            }
            
        }

        private List<string> HandlePackagesNode(string pathNewDoc, XDocument document)
        {
            if (document.Root.Descendants("Packages").First() != null)
            {
                foreach (var descendant in document.Root.Descendants("Package"))
                {
                    value = descendant.Attribute("privatePath").Value;

                    if (document.Root.Descendants("Packages").Select(node => node.Attribute("privatePath").Value = value).Count() == 0)
                    {
                        XElement element = new XElement("Package");
                        element.SetAttributeValue("privatePath", value);

                        document.Root.Descendants("Packages").First().Add(element);
                    }
                    AnalisysSchemeConfiguration(document, pathNewDoc + value);
                }
            }

            return new List<string>();
        }
    }
}
