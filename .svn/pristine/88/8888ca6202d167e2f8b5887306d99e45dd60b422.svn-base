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
    public partial class BottomCountSettingsForm : Form
    {
        private BottomCountSettings _bcSettings;
        private BottomCountSettings _newBottomCountSettings;
        private bool isInitialize = false;

        public BottomCountSettingsForm(BottomCountSettings bcSettings)
        {
            InitializeComponent();
            this._bcSettings = bcSettings;
            this.Init();
        }

        public BottomCountSettings BottomCountSettings
        {
            get { return _newBottomCountSettings; }
        }


        private void Init()
        {
            this.isInitialize = true;

            if (this._bcSettings == null)
                return;

            this.ceIsBottomCountCalculate.Checked = this._bcSettings.IsBottomCountCalculate;
            this.neBottomCount.Value = this._bcSettings.BottomCount;
            this.cpBottomCountColor.Color = this._bcSettings.BottomCountColor;

            this.isInitialize = false;

        }


        private void btOK_Click(object sender, EventArgs e)
        {
            this._newBottomCountSettings = new BottomCountSettings();
            this._newBottomCountSettings.IsBottomCountCalculate = this.ceIsBottomCountCalculate.Checked;
            this._newBottomCountSettings.BottomCount = (int)this.neBottomCount.Value;
            this._newBottomCountSettings.BottomCountColor = this.cpBottomCountColor.Color;
        }

    }
}
