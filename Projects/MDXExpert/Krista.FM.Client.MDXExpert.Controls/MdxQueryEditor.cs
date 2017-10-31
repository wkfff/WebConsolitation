using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Krista.FM.Client.MDXExpert.Data;

namespace Krista.FM.Client.MDXExpert.Controls
{
    public partial class MdxQueryEditor : Form
    {
        private PivotData currentPivotData;

        public string Query
        {
            get { return this.mdxQueryControl.Query; }
        }

        public MdxQueryEditor()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ќбновим редактор
        /// </summary>
        /// <param name="pivotData"></param>
        public void RefreshEditor(PivotData pivotData)
        {
            this.currentPivotData = pivotData;
            //отмен€ем автоматическое сохранение запроса в пивот дату
            this.mdxQueryControl.AutoSaveQuery = false;
            this.mdxQueryControl.DisplayMode = ControlDisplayMode.Extended;
            this.mdxQueryControl.RefreshEditor(this.currentPivotData);
            this.mdxQueryControl.QueryTextChanged += new EventHandler(mdxQueryControl_QueryTextChanged);
            this.RefreshStateControl();
        }

        void mdxQueryControl_QueryTextChanged(object sender, EventArgs e)
        {
            this.RefreshStateControl();
        }

        private void RefreshStateControl()
        {
            this.ubApply.Enabled = false;
            if (this.currentPivotData != null)
            {
                //если запросы отличаютс€, кнопку разблокируем
                this.ubApply.Enabled = (this.currentPivotData.MDXQuery != this.mdxQueryControl.Query);
            }
        }

        /// <summary>
        /// ѕрименить запрос
        /// </summary>
        private void ApplyQuery()
        {
            if (this.currentPivotData != null)
            {
                if (this.currentPivotData.MDXQuery != this.mdxQueryControl.Query)
                {
                    this.currentPivotData.MDXQuery = this.mdxQueryControl.Query;
                    this.currentPivotData.DoDataChanged();
                }
            }
        }

        private void ubOk_Click(object sender, EventArgs e)
        {
            this.ApplyQuery();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void ubApply_Click(object sender, EventArgs e)
        {
            this.ApplyQuery();
            this.RefreshStateControl();
        }

        private void ubCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

    }
}