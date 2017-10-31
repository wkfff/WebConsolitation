using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Krista.FM.Client.iMonitoringWM.Controls
{
    public partial class ClosedDialog : UserControl
    {
        private CloseDialogResult dialogResult;

        public ClosedDialog()
        {
            InitializeComponent();
        }

        private void btYes_Click(object sender, EventArgs e)
        {
            dialogResult = CloseDialogResult.Yes;
            isChangeResult = true;
        }

        private void btClose_Click(object sender, EventArgs e)
        {
            dialogResult = CloseDialogResult.Cancel;
            isChangeResult = true;
        }

        private void btMinimize_Click(object sender, EventArgs e)
        {
            dialogResult = CloseDialogResult.Minimize;
            isChangeResult = true;
        }

        private bool isChangeResult;

        public CloseDialogResult ShowModal()
        {
            dialogResult = CloseDialogResult.None;
            this.caption.TextBounds = new Rectangle(10, 0, this.caption.Bounds.Width - 10, this.caption.Bounds.Height);
            this.Show();
            while (!isChangeResult)
            {
                Application.DoEvents();
            }
            this.Hide();
            return dialogResult;
        }
    }

    public enum CloseDialogResult
    {
        None,
        Yes,
        Cancel,
        Minimize
    }
}
