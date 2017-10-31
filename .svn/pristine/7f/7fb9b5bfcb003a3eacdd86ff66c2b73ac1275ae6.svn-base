using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Krista.FM.Client.ViewObjects.TasksUI
{
    public partial class FormWritebackOptions : Form
    {

        public static bool ShowWritebackOptions(IWin32Window parentWindow, ref bool processSelectDocuments, ref bool rewriteData, ref bool processMD)
        {
            FormWritebackOptions frm = new FormWritebackOptions();
            if (!processSelectDocuments)
            {
                frm.uosProcessDocuments.Enabled = false;
                frm.uosProcessDocuments.CheckedIndex = 1;
            }
            if (frm.ShowDialog(parentWindow) == DialogResult.OK)
            {
                processSelectDocuments = Convert.ToBoolean(frm.uosProcessDocuments.CheckedItem.DataValue);
                rewriteData = Convert.ToBoolean(frm.uosDataWrite.CheckedItem.DataValue);
                processMD = frm.uceProcessMD.Checked;
                return true;
            }
            return false;
        }

        public FormWritebackOptions()
        {
            InitializeComponent();
        }

        private void FormWritebackOptions_Load(object sender, EventArgs e)
        {

        }
    }
}