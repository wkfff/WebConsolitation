using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Krista.FM.Client.SchemeEditor.Gui.ViewControls
{
    public partial class ValidationErrorListForm : Form
    {
        public ValidationErrorListForm(DataTable errorListTable)
        {
            InitializeComponent();

            Krista.FM.Client.Common.DefaultFormState.Load(this);

            this.validationErrorListControl.DataSource = errorListTable;
        }

        private void ValidationErrorListForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Krista.FM.Client.Common.DefaultFormState.Save(this);
        }
    }
}