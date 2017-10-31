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
    public partial class MedianSettingsForm : Form
    {
        private MedianSettings _mSettings;
        private MedianSettings _newMedianSettings;
        private bool isInitialize = false;

        public MedianSettingsForm(MedianSettings mSettings)
        {
            InitializeComponent();
            this._mSettings = mSettings;
            this.Init();
        }

        public MedianSettings MedianSettings
        {
            get { return _newMedianSettings; }
        }


        private void Init()
        {
            this.isInitialize = true;

            if (this._mSettings == null)
                return;

            this.ceIsMedianCalculate.Checked = this._mSettings.IsMedianCalculate;

            this.ceLowerHigherSeparate.Checked = this._mSettings.IsLowerHigherSeparate;

            this.cpLowerMedian.Color = this._mSettings.LowerMedianColor;
            this.cpHigherMedian.Color = this._mSettings.HigherMedianColor;

            this.isInitialize = false;

        }


        private void btOK_Click(object sender, EventArgs e)
        {
            this._newMedianSettings = new MedianSettings();
            this._newMedianSettings.IsMedianCalculate = this.ceIsMedianCalculate.Checked;
            this._newMedianSettings.IsLowerHigherSeparate = this.ceLowerHigherSeparate.Checked;
            this._newMedianSettings.HigherMedianColor = this.cpHigherMedian.Color;
            this._newMedianSettings.LowerMedianColor = this.cpLowerMedian.Color;
        }

    }
}
