using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Krista.FM.Client.Common.Forms;

namespace Krista.FM.Client.Help
{
    public partial class ParametersForm : Form
    {
        public ParametersForm()
        {
            InitializeComponent();

            if (btnHelpCreate.CanSelect)
                btnHelpCreate.Select();
        }

        private void btnHelpCreate_Click(object sender, EventArgs e)
        {

            HelpMode mode = DifineMode();
            HelpVariant variant = rbDiagramAdd.Checked ? HelpVariant.diagramAdd : HelpVariant.diagramFull;

            HelpManager manager = new HelpManager(variant, mode);
            manager.HelpGenerator();

        }

        /// <summary>
        /// Определяет режим генерации справки
        /// </summary>
        /// <returns></returns>
        private HelpMode DifineMode()
        {
            if (rbDeveloperMode.Checked)
                return HelpMode.developerMode;
            if (rbLiteMode.Checked)
                return HelpMode.liteMode;

            return HelpMode.userMode;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }

    public struct Parameters
    {
        public static void Show(Form parent)
        {
            ParametersForm form = new ParametersForm();

            if(form.ShowDialog(parent)== DialogResult.Cancel)
            {
                form.Close();
            }
        }
    }
}