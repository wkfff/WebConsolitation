using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Krista.FM.Client.Common;

namespace Krista.FM.Client.ViewObjects.TasksUI
{
    public partial class FormTaskParamsResult : Form
    {
        public static DialogResult ShowResults(string message, DataTable lockTasks)
        {
            FormTaskParamsResult frmResults = new FormTaskParamsResult();
            frmResults.lResults.Text = message;
            if (lockTasks != null)
            {
                foreach (DataRow row in lockTasks.Rows)
                {
                    frmResults.udsLockTasks.Rows.Add(row.ItemArray);
                }
                frmResults.ugResults.Visible = true;
                frmResults.Size = new Size(507, 250);
                frmResults.button2.Location = new Point(414, 196);
            }
            else
            {
                frmResults.Size = new Size(300, 120);
                frmResults.button2.Location = new Point(207, 64);
            }
            return frmResults.ShowDialog();
        }

        public FormTaskParamsResult()
        {
            InitializeComponent();
            InfragisticComponentsCustomize.CustomizeUltraGridParams(this.ugResults);
            this.ugResults.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.Horizontal;
        }

        private void ugResults_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

        }

    }
}