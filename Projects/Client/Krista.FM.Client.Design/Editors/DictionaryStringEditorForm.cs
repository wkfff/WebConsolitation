using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Infragistics.Win.UltraWinEditors;
using Infragistics.Win;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinGrid.Design;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinGrid.ExcelExport;

using Krista.FM.Client.Components;


namespace Krista.FM.Client.Design.Editors
{
    public partial class DictionaryStringEditorForm : Form
    {
        private IDictionary<string, string> value;

        public DictionaryStringEditorForm(DictionaryStringEditor editor, IDictionary<string, string> value)
        {
            this.value = value;

            this.Text = value.ToString();
            InitializeComponent();

            Krista.FM.Client.Common.DefaultFormState.Load(this);

            semanticsGrid.Value = value;
            semanticsGrid.RefreshAll();
        }
      
        private void okButton_Click(object sender, EventArgs e)
        {
            semanticsGrid.SaveChanges();
        }

        private void DictionaryStringEditorForm_FormClosed(object sender, EventArgs e)
        {
            Krista.FM.Client.Common.DefaultFormState.Save(this);
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult result;
            if (semanticsGrid.IsChanged())
            {
                result = MessageBox.Show("Сохранить изменения?","Сохранение", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (result == DialogResult.Yes)
                    semanticsGrid.SaveChanges();
                else
                    Close();
            }
            else
                Close();
        }
     }
}