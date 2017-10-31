using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Infragistics.Win.UltraWinTree;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Common.RegistryUtils;

namespace Krista.FM.Client.MDXExpert.Controls
{
    public partial class MapTemplateChooser : Form
    {
        private string _repositoryPath;
        private string _templateName;


        public string RepositoryPath
        {
            get { return this._repositoryPath; }
        }

        public string TemplateName
        {
            get { return this._templateName; }
        }

        public MapTemplateChooser(string repositoryPath, string templateName)
        {
            InitializeComponent();

            this._repositoryPath = repositoryPath;
            this._templateName = templateName;

            teRepository.Text = repositoryPath;

            if (Directory.Exists(repositoryPath))
            {
                LoadMapFolders(null, repositoryPath);
            }
            btOK.Enabled = false;
            SelectCurrentTemplate();
        }


        private void teRepository_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                if (this._repositoryPath != folderBrowserDialog.SelectedPath)
                {
                    tvMaps.Nodes.Clear();
                    this._repositoryPath = folderBrowserDialog.SelectedPath;
                    this.teRepository.Text = this._repositoryPath;
                    LoadMapFolders(null, this._repositoryPath);
                }
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

        /// <summary>
        /// Выделение текущего шаблона
        /// </summary>
        private void SelectCurrentTemplate()
        {
            tvMaps.SelectedNodes.Clear();
            if ((tvMaps.Nodes.Count == 0)||(String.IsNullOrEmpty(this.TemplateName)))
                return;


            string[] pathTails = this.TemplateName.TrimStart('\\').Split('\\');

            UltraTreeNode currNode = null;
            TreeNodesCollection nodes = tvMaps.Nodes;
            bool isFindNode = false;

            foreach(string pTail in pathTails)
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

        private void LoadMapFolders(UltraTreeNode rootNode, string root)
        {
            UltraTreeNode newNode;

            string path = (rootNode == null) ? root : rootNode.Key;

            string[] directories = Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);

            foreach (string directory in directories)
            {
                newNode = (rootNode == null)? tvMaps.Nodes.Add(directory) : rootNode.Nodes.Add(directory);
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
            this._repositoryPath = this.teRepository.Text;
            if (tvMaps.SelectedNodes.Count > 0)
            {
                this._templateName = "\\" + tvMaps.SelectedNodes[0].FullPath;
            }
        }

        private void tvMaps_AfterSelect(object sender, SelectEventArgs e)
        {
            btOK.Enabled = (e.NewSelections.Count > 0) ? IsTemplate((string)e.NewSelections[0].Key) : false;
        }



    }
}
