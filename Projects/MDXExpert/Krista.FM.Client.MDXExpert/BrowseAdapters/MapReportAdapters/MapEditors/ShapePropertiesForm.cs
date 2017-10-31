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
    public partial class ShapePropertiesForm : Form
    {
        private MapReportElement mapElement;
        private Shape _shape;

        public ShapePropertiesForm(MapReportElement mapElement, Shape shape)
        {
            InitializeComponent();

            this.mapElement = mapElement;
            this._shape = shape;
            this.Text = String.Format("Свойства объекта \"{0}\"", this._shape["NAME"]);
            AddShape();
            SetFormPosition();
        }

        /// <summary>
        /// Установка координат формы в позиции курсора
        /// </summary>
        private void SetFormPosition()
        {
            int borderWidth = 5;
            int x = Cursor.Position.X;
            int y = Cursor.Position.Y;

            if ((x + this.Width + borderWidth) > Screen.PrimaryScreen.WorkingArea.Width)
            {
                x = Screen.PrimaryScreen.WorkingArea.Width - this.Width - borderWidth;
            }

            if ((y + this.Height + borderWidth) > Screen.PrimaryScreen.WorkingArea.Height)
            {
                y = Screen.PrimaryScreen.WorkingArea.Height - this.Height - borderWidth;
            }

            if (x < borderWidth)
            {
                x = borderWidth;
            }

            this.Left = x;
            this.Top = y;
        }

        private void AddShape()
        {
            this.tvShape.Nodes.Clear();
            if (this._shape.Name.Contains("Shape"))
            {
                return;
            }
            UltraTreeNode objectNode = tvShape.Nodes.Add(this._shape.Name);
            objectNode.Tag = this._shape;
            objectNode.Text = (string) this._shape["NAME"];
            AddSymbols(objectNode, this._shape.Name);
            tvShape.Nodes[0].Selected = true;
            tvShape.ExpandAll();
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
            object selectedObj = tvShape.SelectedNodes[0].Tag;
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

        private void btOK_Click(object sender, EventArgs e)
        {
            //this.mapElement.MainForm.Saved = false;
            this.Close();
        }

        private void ShapePropertiesForm_Deactivate(object sender, EventArgs e)
        {
            //this.mapElement.MainForm.Saved = false;
            //this.Close();
        }

        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            this.mapElement.MainForm.Saved = false;
        }


    }
}