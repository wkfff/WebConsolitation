using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Infragistics.Win.UltraWinTree;
using System.Windows.Forms.Design;

namespace Krista.FM.Client.MDXExpert
{
    public partial class MapTemplateBrowser : UserControl
    {
        private string mapTemplatePath;
        private bool valueChanged;

        public bool ValueChanged
        {
            get { return this.valueChanged; }
        }

        public string MapTemplatePath
        {
            get { return mapTemplatePath; }
            set { mapTemplatePath = value; }
        }

        public MapTemplateBrowser(string mapRepositoryPath, string mapTemplatePath)
        {
            InitializeComponent();
            this.mapTemplatePath = mapTemplatePath;
            if (Directory.Exists(mapRepositoryPath))
            {
                LoadMapFolders(null, mapRepositoryPath);
            }

            SelectCurrentTemplate();
            this.valueChanged = false;
        }

        /// <summary>
        /// Выделение текущего шаблона
        /// </summary>
        private void SelectCurrentTemplate()
        {
            tvMaps.SelectedNodes.Clear();
            if ((tvMaps.Nodes.Count == 0) || (String.IsNullOrEmpty(this.MapTemplatePath)))
                return;


            string[] pathTails = this.MapTemplatePath.TrimStart('\\').Split('\\');

            UltraTreeNode currNode = tvMaps.Nodes[0];
            TreeNodesCollection nodes = tvMaps.Nodes[0].Nodes;
            bool isFindNode = false;

            foreach (string pTail in pathTails)
            {
                if (currNode != null)
                {
                    currNode.Expanded = true;
                    nodes = currNode.Nodes;
                }

                isFindNode = false;
                foreach (UltraTreeNode child in nodes)
                {
                    if (child.Text == pTail)
                    {
                        currNode = child;
                        isFindNode = true;
                        break;
                    }
                }
            }

            if (isFindNode)
            {
                currNode.Selected = true;
                tvMaps.TopNode = currNode;
            }

        }


        private bool IsTemplate(string path)
        {
            bool result = false;
            try
            {
                if (path != string.Empty)
                    result = (Directory.GetFiles(path, "*.emt", SearchOption.TopDirectoryOnly).Length > 0);
            }
            catch
            {
            }
            return result;
        }

        private void SetFolderAppearance(UltraTreeNode node)
        {
            string directory = (string)node.Key;

            node.Text = Path.GetFileName(directory);
            node.Override.NodeAppearance.Image = IsTemplate(directory) ? 1 : 0;

            try
            {
                if ((directory != string.Empty) &&
                    (Directory.GetDirectories(directory, "*", SearchOption.TopDirectoryOnly).Length > 0))
                {
                    node.Override.ShowExpansionIndicator = ShowExpansionIndicator.Always;
                }
            }
            catch
            {
            }
        }

        private void LoadMapFolders(UltraTreeNode rootNode, string root)
        {
            UltraTreeNode newNode;

            if (rootNode == null)
            {
                newNode = tvMaps.Nodes.Add(root);
                SetFolderAppearance(newNode);
                return;
            }

            string path = (string)rootNode.Key; 
            string[] directories = Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);

            foreach (string directory in directories)
            {
                newNode = rootNode.Nodes.Add(directory);
                SetFolderAppearance(newNode);
            }
        }

        private void tvMaps_AfterExpand(object sender, NodeEventArgs e)
        {
            if (e.TreeNode.Nodes.Count == 0)
            {
                LoadMapFolders(e.TreeNode, "");
            }
        }

        private string NodePathWithoutRoot(string fullPath)
        {
            string result = "";
            int rootEndPos = fullPath.IndexOf('\\');
            if (rootEndPos > -1)
            {
                result = fullPath.Substring(rootEndPos);
            }
            return result;
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            if (sender == btOK)
            {
                MapTemplatePath = NodePathWithoutRoot(tvMaps.SelectedNodes[0].FullPath); // Key;
                this.valueChanged = true;
            }
            else
            if(sender == btClear)
            {
                MapTemplatePath = "";
                this.valueChanged = true;
            }

            if (Tag != null)
            {
                ((IWindowsFormsEditorService)Tag).CloseDropDown();
            }
            else
            {
                this.Hide();
            }

        }

        private void tvMaps_AfterSelect(object sender, SelectEventArgs e)
        {
            btOK.Enabled = IsTemplate((string)e.NewSelections[0].Key);
        }


        

    }
}
