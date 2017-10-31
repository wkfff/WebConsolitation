using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Krista.FM.Client.iMonitoringWM.Common;
using Microsoft.WindowsCE.Forms;

namespace Krista.FM.Client.iMonitoringWM.Controls
{
    public partial class EntityListView : UserControl
    {
        #region События
        private event EventHandler _closeEntityView;

        public event EventHandler CloseEntityView
        {
            add { _closeEntityView += value; }
            remove { _closeEntityView -= value; }
        }
        #endregion

        private int _changedEntity;

        /// <summary>
        /// Выбранный субъект
        /// </summary>
        public int ChangedEntity
        {
            get { return _changedEntity; }
            set { _changedEntity = value; }
        }

        public bool IsInitializedVisualComponent
        {
            get { return this.mainControlsPanel != null; }
        }

        public EntityListView()
        {
        }

        public void InitVisualComponent(List<ScrollListItem> entityList)
        {
            InitializeComponent();
            this.InitImageList();

            this.scrollEntityList.SelectedItem += new ScrollList.SelectedItemHandler(scrollEntityList_SelectedItem);

            this.InitEntityList(entityList);
            this.scrollEntityList.Items[this.ChangedEntity - 1].IsChanged = true;

            this.searchBox.TextChanged += new EventHandler(searchBox_TextChanged);
        }

        private void InitEntityList(List<ScrollListItem> entityList)
        {
            this.scrollEntityList.Clear();
            foreach (ScrollListItem item in entityList)
            {
                this.scrollEntityList.AddItem(item);
            }
        }

        private void InitImageList()
        {
            switch (Utils.ScreenSize)
            {
                case ScreenSizeMode.s240x320:
                    {
                        this.scrollEntityList.ImageList = this.imageListFor240x320;
                        break;
                    }
                case ScreenSizeMode.s480x640:
                case ScreenSizeMode.s480x800:
                    {
                        this.scrollEntityList.ImageList = this.imageListFor480x640;
                        break;
                    }
            }
        }

        void scrollEntityList_SelectedItem(object sender, ScrollListItem item)
        {
            this.SelectEntity(item);
        }

        private void SelectEntity(ScrollListItem item)
        {
            if (item != null)
            {
                //снимим галочку с предыдущего выделеного субъекта
                this.scrollEntityList.Items[this.ChangedEntity - 1].IsChanged = false;
                //выставим новому
                item.IsChanged = true;
                this.ChangedEntity = this.scrollEntityList.Items.IndexOf(item) + 1;
            }
        }

        private void FilterEntityList(string filterText)
        {
            filterText = filterText.ToLower();
            filterText = filterText.Trim();

            foreach (ScrollListItem item in this.scrollEntityList.Items)
            {
                bool isItemVisible = (filterText == string.Empty);
                if (!isItemVisible)
                {
                    string entityName = item.Text.ToLower();
                    if (filterText.Contains(" "))
                    {
                        isItemVisible = entityName.StartsWith(filterText);
                    }

                    //Разделим название субъекта на состовляющие
                    string[] nameParts = entityName.Split(new char[] { ' ', '-', '.', '(', ')' });
                    foreach (string part in nameParts)
                    {
                        if (part.StartsWith(filterText))
                        {
                            isItemVisible = true;
                            break;
                        }
                    }
                }

                item.Visible = isItemVisible;
            }
            this.scrollEntityList.RecalculatedItemsLocation();
            this.scrollEntityList.Invalidate();
        }

        private void EnabledInputPanel(bool value)
        {
            if (this.inputPanel.Enabled != value)
            {
                this.inputPanel.Enabled = value;
            }
        }

        private void RecalculateHeightScrollEntityList()
        {
            int inputPanelHeight = this.inputPanel.Enabled ?  this.Height - this.inputPanel.Bounds.Top : 0;
            this.scrollEntityList.Height = this.mainControlsPanel.Height 
                - this.searchBox.Height - inputPanelHeight;
        }

        private void btApply_Click(object sender, EventArgs e)
        {
            this.EnabledInputPanel(false);
            this.OnCloseEntityView();
        }

        protected virtual void OnCloseEntityView()
        {
            if (this._closeEntityView != null)
                this._closeEntityView(this, new EventArgs());
        }

        private void searchBox_TextChanged(object sender, EventArgs e)
        {
            this.FilterEntityList(this.searchBox.Text);
        }

        private void searchBox_MouseDown(object sender, MouseEventArgs e)
        {
            this.EnabledInputPanel(!this.inputPanel.Enabled);
        }

        private void inputPanel_EnabledChanged(object sender, EventArgs e)
        {
            this.RecalculateHeightScrollEntityList();
        }
    }
}
