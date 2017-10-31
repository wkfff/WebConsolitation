using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Krista.FM.Client.iMonitoringWM.Common;

namespace Krista.FM.Client.iMonitoringWM.Controls
{
    public partial class SettingsView : UserControl
    {
        #region События
        private event EventHandler _applySettings;

        public event EventHandler ApplySettings
        {
            add { _applySettings += value; }
            remove { _applySettings -= value; }
        }
        #endregion

        #region Поля
        private List<string> _enityList;

        private EntityListView _entityListView;
        #endregion

        #region Свойства
        public bool IsInitializedVisualComponent
        {
            get { return this.mainControlsPanel != null; }
        }

        /// <summary>
        /// Список отчетов
        /// </summary>
        public List<ScrollListItem> ReportList
        {
            get { return this.scrollReportList.Items; }
        }

        /// <summary>
        /// Список субъектов
        /// </summary>
        public List<string> EntityList
        {
            get { return _enityList; }
            set 
            { 
                _enityList = value;
                this.InitEntityCaption();
            }
        }

        /// <summary>
        /// Выбранный субъект
        /// </summary>
        public int ChangedEntity
        {
            get { return this.EntityListView.ChangedEntity; }
            set { this.EntityListView.ChangedEntity = value; }
        }

        /// <summary>
        /// Форма выбора субъекта
        /// </summary>
        public EntityListView EntityListView
        {
            get { return _entityListView; }
            set { _entityListView = value; }
        }
        #endregion

        public SettingsView()
        {
            this.EntityListView = new EntityListView();
            this.EntityList = new List<string>();
        }

        public void InitVisualComponent()
        {
            InitializeComponent();
            this.InitImageList();
            this.EntityListView.CloseEntityView += new EventHandler(EntityListView_CloseEntityView);
        }

        private void InitImageList()
        {
            switch (Utils.ScreenSize)
            {
                case ScreenSizeMode.s240x320:
                    {
                        this.scrollReportList.ImageList = this.imageListFor240x320;
                        break;
                    }
                case ScreenSizeMode.s480x640:
                case ScreenSizeMode.s480x800:
                    {
                        this.scrollReportList.ImageList = this.imageListFor480x640;
                        break;
                    }
            }
        }

        public void InitSettings(List<ScrollListItem> reportList, int changedEntity)
        {
            this.scrollReportList.Clear();
            foreach (ScrollListItem item in reportList)
            {
                this.scrollReportList.AddItem(item);
            }

            this.ChangedEntity = changedEntity;
        }

        private void InitEntityCaption()
        {
            if ((this.EntityList.Count > 0) && (this.ChangedEntity > 0))
            {
                string entityName = this.EntityList[this.ChangedEntity - 1];
                Graphics gr = this.CreateGraphics();
                SizeF stringSize = Utils.GetStringSize(entityName + "    ", gr, this.btChangeEntity.Font);
                this.btChangeEntity.TextDefault = Utils.CutString(entityName, stringSize.Width,
                    this.btChangeEntity.Width, "...");
                gr.Dispose();
            }
        }

        /// <summary>
        /// По отчету, получаем элемент для контрола управляющего порядком следования и 
        /// видимостью отчетов в настройках
        /// </summary>
        /// <param name="report">отчет</param>
        /// <returns>скролируемый элемент</returns>
        private ScrollListItem GetScrollListItem(string entityName)
        {
            ScrollListItem result = new ScrollListItem();
            result.IsEditMode = false;
            int heightItem = Utils.ScreenSize == ScreenSizeMode.s240x320 ? 30 : 60;
            result.Size = new Size(this.Width, heightItem);
            result.BackColor = Color.Black;
            result.ForeColor = Color.LightGray;
            result.Text = entityName;
            result.IsChanged = false;

            return result;
        }

        private void ShowEntityView()
        {
            //Если открываем в первый раз, сначала проинициализируем список субъектов
            if (!this.EntityListView.IsInitializedVisualComponent)
            {
                List<ScrollListItem> scrollItems = new List<ScrollListItem>();
                foreach (string entity in this.EntityList)
                {
                    scrollItems.Add(this.GetScrollListItem(entity));
                }
                this.EntityListView.InitVisualComponent(scrollItems);
            }

            this.grandPanel.Visible = false;
            this.EntityListView.Dock = DockStyle.Fill;
            this.EntityListView.Visible = true;
            this.EntityListView.Parent = this;
        }

        public void HideEntityView()
        {
            this.EntityListView.Visible = false;
            this.EntityListView.Parent = null;

            this.grandPanel.Visible = true;
        }

        protected virtual void OnApplySettings()
        {
            if (this._applySettings != null)
                this._applySettings(this, new EventArgs());
        }

        private void btChangeEntity_Click(object sender, EventArgs e)
        {
            this.ShowEntityView();
        }

        private void EntityListView_CloseEntityView(object sender, EventArgs e)
        {
            this.InitEntityCaption();
            this.HideEntityView();
        }

        private void btApply_Click(object sender, EventArgs e)
        {
            this.OnApplySettings();
        }
    }
}
