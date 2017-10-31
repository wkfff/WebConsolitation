using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Krista.FM.Client.ViewObjects.TemplatesUI
{
    public partial class ImportParams : Form
    {
        public static bool ShowForm(ref bool addToTopLevel)
        {
            ImportParams form = new ImportParams();
            // устанавливаем возможность импорта записей как подчиненных
            form.radioButton2.Enabled = !addToTopLevel;
            if (form.ShowDialog() == DialogResult.OK)
            {
                addToTopLevel = form.radioButton1.Checked;
                return true;
            }
            return false;
        }

        public ImportParams()
        {
            InitializeComponent();
        }
    }
}