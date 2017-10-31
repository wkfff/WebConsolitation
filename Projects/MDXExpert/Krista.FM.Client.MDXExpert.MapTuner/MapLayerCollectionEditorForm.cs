using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Dundas.Maps.WinControl;

namespace Krista.FM.Client.MDXExpert.MapTuner
{
    public partial class MapLayerCollectionEditorForm : Form
    {
        private MapControl map;

        public MapLayerCollectionEditorForm(MapControl map)
        {
            InitializeComponent();

            this.map = map;
            InitLayerTree();
        }

        private void InitLayerTree()
        {
            if (map == null)
            {
                return;
            }
            AddLayers();
        }

        private void AddLayers()
        {
            foreach (Layer layer in map.Layers)
            {
                TreeNode layerNode = tvLayers.Nodes.Add(layer.Name);
                layerNode.Tag = layer;
                layerNode.Text = layer.Name;
                AddShapes(layerNode);
            }
        }

        private void AddShapes(TreeNode layerNode)
        {
            foreach(Shape sh in map.Shapes)
            {
                if (sh.Layer == layerNode.Text)
                {
                    if (sh.Name.Contains("Shape"))
                    {
                        continue;
                    }
                    TreeNode objectNode = layerNode.Nodes.Add(sh.Name);
                    objectNode.Tag = sh;
                    objectNode.Text = sh.Name;
                }
            }
            //layerNode.Nodes.Override.Sort = Infragistics.Win.UltraWinTree.SortType.Ascending;
        }


        private void tvLayers_AfterSelect(object sender, TreeViewEventArgs e)
        {
            object selectedObj = tvLayers.SelectedNode.Tag;
            if(selectedObj is Layer)
            {
                MapLayerBrowseClass layerBrowse = new MapLayerBrowseClass((Layer)selectedObj, this.map);
                propertyGrid.SelectedObject = layerBrowse;
            }
            else
            if (selectedObj is Shape)
            {
                propertyGrid.SelectedObject = new MapShapeBrowseClass((Shape)selectedObj);
            }
        }


    }
}