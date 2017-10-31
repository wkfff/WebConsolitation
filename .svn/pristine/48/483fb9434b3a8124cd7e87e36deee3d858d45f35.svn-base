using System;
using System.Windows.Forms;

namespace Krista.FM.Client.ViewObjects.DisintRulesUI
{
    public partial class YearDataTransfertParams : UserControl
    {
        public YearDataTransfertParams()
        {
            InitializeComponent();

            numericUpDown1.Value = numericUpDown2.Value = DateTime.Now.Year;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDown2.Enabled = !rbSelectedRows.Checked;
        }
        /// <summary>
        /// год, с которого копируем нормативы
        /// </summary>
        public int OldYear
        {
            get { return Convert.ToInt32(numericUpDown2.Value); }
        }
        /// <summary>
        /// год, на который копируем нормативы
        /// </summary>
        public int NewYear
        {
            get { return Convert.ToInt32(numericUpDown1.Value); }
        }
        /// <summary>
        /// показывает, копируем выделеные записи или за все за год
        /// </summary>
        public bool IsTransfertSelectedRows
        {
            get { return !rbSelectedRows.Checked; }
        }
    }
}
