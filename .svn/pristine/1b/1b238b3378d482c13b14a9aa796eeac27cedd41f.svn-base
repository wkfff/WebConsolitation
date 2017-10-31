using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Krista.FM.Client.SchemeEditor
{
    public partial class HashCodeRowsGridForm : Form
    {
        public HashCodeRowsGridForm()
        {
            InitializeComponent();
            Krista.FM.Client.Common.DefaultFormState.Load(this);
        }

        internal DataTable Table
        {
            set { this.hashCodeRowsGridControl.Table = value; }
        }

        internal DataTable CollisionsTable
        {
            set { this.hashCodeRowsGridControl1.Table = value; }
        }

        private void HashCodeRowsGridForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Krista.FM.Client.Common.DefaultFormState.Save(this);
        }

        private void HashCodeRowsGridForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
        }
    }
}