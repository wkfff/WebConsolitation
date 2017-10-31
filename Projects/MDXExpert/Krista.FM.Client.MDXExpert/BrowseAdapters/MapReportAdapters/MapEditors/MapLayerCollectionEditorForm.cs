using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Dundas.Maps.WinControl;
using Infragistics.Win.UltraWinTree;

namespace Krista.FM.Client.MDXExpert
{
    public partial class MapLayerCollectionEditorForm : Form
    {
        private MapReportElement mapElement;

        public MapLayerCollectionEditorForm(MapReportElement mapElement)
        {
            InitializeComponent();

            this.mapElement = mapElement;
            AddLayers();
        }

        private void AddLayers()
        {
            foreach (Layer layer in this.mapElement.Map.Layers)
            {
                UltraTreeNode layerNode = tvLayers.Nodes.Add(layer.Name);
                layerNode.Tag = layer;
                layerNode.Text = layer.Name;
                AddShapes(layerNode);
            }
        }

        private void AddShapes(UltraTreeNode layerNode)
        {
            foreach(Shape sh in this.mapElement.Map.Shapes)
            {
                if (sh.Layer == layerNode.Text)
                {
                    if (sh.Name.Contains("Shape"))
                    {
                        continue;
                    }
                    UltraTreeNode objectNode = layerNode.Nodes.Add(sh.Name);
                    objectNode.Tag = sh;
                    objectNode.Text = (string)sh["NAME"];
                    AddSymbols(objectNode, sh.Name);
                }
            }
            layerNode.Nodes.Override.Sort = Infragistics.Win.UltraWinTree.SortType.Ascending;
        }

        private void AddSymbols(UltraTreeNode objectNode, string shapeName)
        {
            foreach (Symbol symbol in this.mapElement.Map.Symbols)
            {
                if ((symbol.ParentShape == shapeName)&&(symbol.Visible))
                {
                    UltraTreeNode symbolNode = objectNode.Nodes.Add(symbol.Name);
                    symbolNode.Tag = symbol;
                    symbolNode.Text = string.Format("{0} ({1})", symbol.Category, symbol.Tag.ToString());
                }

            }
        }

        private void tvLayers_AfterSelect(object sender, SelectEventArgs e)
        {
            object selectedObj = tvLayers.SelectedNodes[0].Tag;
            if(selectedObj is Layer)
            {
                MapLayerBrowseClass layerBrowse = new MapLayerBrowseClass((Layer)selectedObj, this.mapElement);
                propertyGrid.SelectedObject = layerBrowse;
            }
            else
            if (selectedObj is Shape)
            {
                propertyGrid.SelectedObject = new MapShapeBrowseClass((Shape)selectedObj);
            }
            else
            if (selectedObj is Symbol)
            {
                propertyGrid.SelectedObject = new MapSymbolBrowseClass((Symbol)selectedObj);
            }
        }


    }
}