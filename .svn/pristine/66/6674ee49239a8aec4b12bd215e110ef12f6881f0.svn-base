using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Krista.FM.Client.MDXExpert.Data;

namespace Krista.FM.Client.MDXExpert.Controls
{
    public partial class TopCountSettingsForm : Form
    {
        private TopCountSettings _tcSettings;
        private TopCountSettings _newTopCountSettings;
        private bool isInitialize = false;

        public TopCountSettingsForm(TopCountSettings mSettings)
        {
            InitializeComponent();
            this._tcSettings = mSettings;
            this.Init();
        }

        public TopCountSettings TopCountSettings
        {
            get { return _newTopCountSettings; }
        }


        private void Init()
        {
            this.isInitialize = true;

            if (this._tcSettings == null)
                return;

            this.ceIsTopCountCalculate.Checked = this._tcSettings.IsTopCountCalculate;
            this.neTopCount.Value = this._tcSettings.TopCount;
            this.cpTopCountColor.Color = this._tcSettings.TopCountColor;

            this.isInitialize = false;

        }


        private void btOK_Click(object sender, EventArgs e)
        {
            this._newTopCountSettings = new TopCountSettings();
            this._newTopCountSettings.IsTopCountCalculate = this.ceIsTopCountCalculate.Checked;
            this._newTopCountSettings.TopCount = (int)this.neTopCount.Value;
            this._newTopCountSettings.TopCountColor = this.cpTopCountColor.Color;
        }

    }
}
