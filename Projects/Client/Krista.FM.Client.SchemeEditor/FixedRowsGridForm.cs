using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Krista.FM.Client.SchemeEditor
{
    public partial class FixedRowsGridForm : Form
    {
        public FixedRowsGridForm()
        {
            InitializeComponent();
            Krista.FM.Client.Common.DefaultFormState.Load(this);
        }

        internal Krista.FM.ServerLibrary.IClassifier Classifier
        {
            set { fixedRowsGridControl.Classifier = value; }
        }

        private void FixedRowsGridForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Krista.FM.Client.Common.DefaultFormState.Save(this);
        }
    }
}