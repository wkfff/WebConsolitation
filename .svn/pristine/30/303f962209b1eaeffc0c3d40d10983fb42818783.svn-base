using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Windows.Forms.Design;

namespace Krista.FM.Client.MDXExpert.MapTuner
{
    public partial class MapTemplateBrowser : UserControl
    {
        private string mapTemplatePath;


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

        private void SetFolderAppearance(TreeNode node)
        {
            string directory = (string)node.Tag;

            node.Text = Path.GetFileName(directory);
            node.ImageIndex = IsTemplate(directory) ? 1 : 0;
            try
            {
                if ((directory != string.Empty) &&
                    (Directory.GetDirectories(directory, "*", SearchOption.TopDirectoryOnly).Length > 0))
                {
                    ////node.Override.ShowExpansionIndicator = ShowExpansionIndicator.Always;
                }
            }
            catch
            {
            }
        }

        private void LoadMapFolders(TreeNode rootNode, string root)
        {
            TreeNode newNode;

            if (rootNode == null)
            {
                newNode = tvMaps.Nodes.Add(root);
                newNode.Tag = root;
                SetFolderAppearance(newNode);
                LoadMapFolders(newNode, "");
                newNode.Expand();
                return;
            }

            string path = (string)rootNode.Tag; 
            string[] directories = Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);

            foreach (string directory in directories)
            {
                newNode = rootNode.Nodes.Add(directory);
                newNode.Tag = directory;
                SetFolderAppearance(newNode);
                if ((newNode.Parent != null)&&(newNode.Parent.IsExpanded))
                {
                    LoadMapFolders(newNode, "");
                }
            }
        }

        private void tvMaps_AfterExpand(object sender, TreeViewEventArgs e)
        {
            e.Node.Nodes.Clear();
            LoadMapFolders(e.Node, "");
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
                MapTemplatePath = NodePathWithoutRoot(tvMaps.SelectedNode.FullPath); // Key;
            }
            else
            if(sender == btClear)
            {
                MapTemplatePath = "";
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

        private void tvMaps_AfterSelect(object sender, TreeViewEventArgs e)
        {
            btOK.Enabled = IsTemplate((string)e.Node.Tag);
            e.Node.SelectedImageIndex = e.Node.ImageIndex;
        }

    }
}
