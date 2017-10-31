using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Infragistics.Win.UltraWinListView;
using Krista.FM.Common.Xml;
using Microsoft.AnalysisServices.AdomdClient;

namespace Krista.FM.Client.MDXExpert.Data
{
    public partial class LookupCubeForm : Form
    {
        private PivotData _pivotData;
        private CubeDef _currentCube;
        private XmlNode _filters;

        private UltraListViewItem dragItem = null;
        private bool canItemRemove;


        public XmlNode Filters
        {
            get{return this._filters;}
        }

        public LookupCubeForm(PivotData pData, CubeDef lookupCube, XmlNode filters)
        {
            InitializeComponent();
            this._pivotData = pData;
            this._currentCube = lookupCube;
            this._filters = filters.Clone();

            InitForm();
        }


        private void InitForm()
        {
            LoadHierarchies();
            LoadFilters(this._filters);
        }


        private void LoadHierarchies()
        {
            lvHierarchies.Items.Clear();

            foreach (Dimension dim in this._currentCube.Dimensions)
            {
                if (dim.UniqueName == "[Measures]")
                    continue;

                foreach(Hierarchy h in dim.Hierarchies)
                {
                    lvHierarchies.Items.Add(h.UniqueName, h.Caption).Tag = h;
                }
            }
        }

        private Hierarchy GetHierarchy(string uniqueName)
        {
            foreach(Dimension dim in this._currentCube.Dimensions)
            {
                foreach(Hierarchy h in dim.Hierarchies)
                {
                    if (h.UniqueName == uniqueName)
                        return h;
                }
            }
            return null;
        }


        private void LoadFilters(XmlNode filters)
        {
            lvFilters.Items.Clear();
            if (filters == null)
                return;

            XmlNodeList fsNodes = filters.SelectNodes("fieldSet");
            foreach (XmlNode fsNode in fsNodes)
            {
                string hierarchyUName = XmlHelper.GetStringAttrValue(fsNode, "uname", "");
                Hierarchy h = GetHierarchy(hierarchyUName);
                if (h != null)
                {
                    UltraListViewItem item = lvFilters.Items.Add(h.UniqueName, h.Caption);
                    item.Tag = h;
                    item.Appearance.Image = 0;
                }
            }
        }


        private void InitMemberNames(Hierarchy h, XmlNode parentNode, ref XmlNode fieldSetNode)
        {
            if (fieldSetNode == null)
                fieldSetNode = XmlHelper.AddChildNode(parentNode, "fieldSet", new string[2] { "uname", h.UniqueName });

            if (fieldSetNode.SelectSingleNode("dummy") == null)
            {
                XmlNode memberNames = XmlHelper.AddChildNode(fieldSetNode, "dummy", new string[2] { "childrentype", "included" });
                foreach (Member member in h.Levels[0].GetMembers())
                    XmlHelper.AddChildNode(memberNames, "member", new string[] {"uname", member.UniqueName});
            }
        }

        /// <summary>
        /// удаление из фильтра иерархий, которые не вытащены
        /// </summary>
        private void RemoveUnusedFilters()
        {
            XmlNodeList fsNodes = this._filters.SelectNodes("fieldSet");
            if (fsNodes == null)
                return;

            for(int i = 0; i < fsNodes.Count; i++)
            {
                string fsName = XmlHelper.GetStringAttrValue(fsNodes[i], "uname", "");
                bool isFindItem = false;
                foreach(UltraListViewItem item in lvFilters.Items)
                {
                    if (fsName == ((Hierarchy)item.Tag).UniqueName)
                    {
                        isFindItem = true;
                        break;
                    }
                }
                if (!isFindItem)
                {
                    fsNodes[i].ParentNode.RemoveChild(fsNodes[i]);
                }
            }

        }

        private void lvHierarchies_MouseDown(object sender, MouseEventArgs e)
        {
            this.canItemRemove = false;
            if (e.Button == MouseButtons.Left)
            {
                UltraListView listView = sender as UltraListView;
                UltraListViewItem itemAtPoint = listView.ItemFromPoint(e.Location);
                if (itemAtPoint != null)
                {

                    if (e.X < 16)
                    {
                        Hierarchy h = (Hierarchy) itemAtPoint.Tag;
                        XmlNode fsNode = this._filters.SelectSingleNode("fieldSet[@uname=\""+ h.UniqueName +"\"]");

                        InitMemberNames(h, this._filters, ref fsNode);

                        XmlNode memberNames = fsNode.SelectSingleNode("dummy");
                        this._pivotData.ShowMemberListEx(h, ref memberNames);
                        fsNode.InnerXml = memberNames.OuterXml;

                        //MessageBox.Show(this._total.Filters.OuterXml);
                    }
                    else
                    {
                        this.dragItem = itemAtPoint;
                        listView.DoDragDrop(this.dragItem, DragDropEffects.Move);
                    }
                }

            }


        }


        private void lvHierarchies_DragDrop(object sender, DragEventArgs e)
        {
            UltraListView listView = sender as UltraListView;
            this.OnDragEnd(listView, false);
        }

        private void lvHierarchies_DragOver(object sender, DragEventArgs e)
        {
            if (this.dragItem != null)
            {
                e.Effect = DragDropEffects.All;
            }
        }

        private void lvHierarchies_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            UltraListView listView = sender as UltraListView;
            
            if (e.Action == DragAction.Drop)
            {
                if (this.canItemRemove)
                {
                    listView.Items.Remove(this.dragItem);
                    RemoveUnusedFilters();
                }
            }
        }

        private void OnDragEnd(UltraListView listView, bool canceled)
        {
            if (canceled == false && this.dragItem != null)
            {
                if (!listView.Items.Exists(this.dragItem.Key))
                {
                    listView.BeginUpdate();
                    UltraListViewItem newItem = this.dragItem.Clone();
                    listView.Items.Add(newItem);
                    newItem.Appearance.Image = 0;
                    listView.EndUpdate();

                    Hierarchy h = (Hierarchy)newItem.Tag;
                    XmlNode fsNode = this._filters.SelectSingleNode("/fieldSet[@uname=\"" + h.UniqueName + "\"]");
                    InitMemberNames(h, this._filters, ref fsNode);
                }
            }
        }

        private void lvFilters_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            this.canItemRemove = (e.Effect == DragDropEffects.None);
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            RemoveUnusedFilters();
        }

        private void btAddFilter_Click(object sender, EventArgs e)
        {
            if (lvHierarchies.SelectedItems.Count > 0)
            {
                this.dragItem = lvHierarchies.SelectedItems[0];
                this.OnDragEnd(lvFilters, false);
            }
        }

        private void btDeleteFilter_Click(object sender, EventArgs e)
        {
            if (lvFilters.SelectedItems.Count > 0)
            {
                lvFilters.Items.Remove(lvFilters.SelectedItems[0]);
                RemoveUnusedFilters();
            }

        }

        private void lvHierarchies_ItemDoubleClick(object sender, ItemDoubleClickEventArgs e)
        {
            this.dragItem = e.Item;
            this.OnDragEnd(lvFilters, false);
        }
    }
}
