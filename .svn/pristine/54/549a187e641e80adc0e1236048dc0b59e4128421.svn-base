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
    public partial class AverageSettingsForm : Form
    {
        private AverageSettings _aSettings;
        private AverageSettings _newAverageSettings;
        private bool isInitialize = false;

        public AverageSettingsForm(AverageSettings aSettings)
        {
            InitializeComponent();
            this._aSettings = aSettings;
            this.Init();
        }

        public AverageSettings AverageSettings
        {
            get { return _newAverageSettings; }
        }


        private void Init()
        {
            this.isInitialize = true;
            ceAverageType.Items.Clear();

            ceAverageType.Items.Add(AverageType.None, GetAverageTypeDescription(AverageType.None));
            ceAverageType.Items.Add(AverageType.Arithmetical, GetAverageTypeDescription(AverageType.Arithmetical));
            ceAverageType.Items.Add(AverageType.Geometrical, GetAverageTypeDescription(AverageType.Geometrical));
            ceAverageType.Items.Add(AverageType.Harmonic, GetAverageTypeDescription(AverageType.Harmonic));

            if (this._aSettings == null)
                return;

            this.ceAverageType.Value = this._aSettings.AverageType;

            this.ceLowerHigherSeparate.Checked = this._aSettings.IsLowerHigherAverageSeparate;
            this.cpLowerAverage.Color = this._aSettings.LowerAverageColor;
            this.cpHigherAverage.Color = this._aSettings.HigherAverageColor;

            this.ceIsAverageDeviationCalculate.Checked = this._aSettings.IsAverageDeviationCalculate;


            this.ceStandartDeviationCalculate.Checked = this._aSettings.IsStandartDeviationCalculate;
            this.cpHigherDevLimit.Color = this._aSettings.HigherDeviationColor;
            this.cpLowerDevLimit.Color = this._aSettings.LowerDeviationColor;
            this.isInitialize = false;

        }

        /// <summary>
        /// Получение описания типа среднего значения
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string GetAverageTypeDescription(AverageType value)
        {
            FieldInfo fi = value.GetType().GetField(Enum.GetName(typeof(AverageType), value));
            DescriptionAttribute dna =
              (DescriptionAttribute)Attribute.GetCustomAttribute(fi, typeof(DescriptionAttribute));

            if (dna != null)
                return dna.Description;
            else
                return value.ToString();
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            this._newAverageSettings = new AverageSettings();
            this._newAverageSettings.AverageType = (AverageType)this.ceAverageType.Value;
            this._newAverageSettings.IsLowerHigherAverageSeparate = this.ceLowerHigherSeparate.Checked;
            this._newAverageSettings.HigherAverageColor = this.cpHigherAverage.Color;
            this._newAverageSettings.LowerAverageColor = this.cpLowerAverage.Color;
            this._newAverageSettings.IsAverageDeviationCalculate = this.ceIsAverageDeviationCalculate.Checked;
            this._newAverageSettings.IsStandartDeviationCalculate = this.ceStandartDeviationCalculate.Checked;
            this._newAverageSettings.HigherDeviationColor = this.cpHigherDevLimit.Color;
            this._newAverageSettings.LowerDeviationColor = this.cpLowerDevLimit.Color;
        }

        private void ceAverageType_ValueChanged(object sender, EventArgs e)
        {
            if (this.isInitialize)
                return;
            if(((AverageType)this.ceAverageType.Value == AverageType.Geometrical)||((AverageType)this.ceAverageType.Value == AverageType.Harmonic))
            {
                MessageBox.Show(
                    "Расчет данного типа среднего не возможен, если хотя бы одно значение в группе равно или меньше «0».",
                    "MDX Эксперт", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }



    }
}
