using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Forms
{
    public partial class frmNewCalculate : Form
    {
        public frmNewCalculate()
        {
            InitializeComponent();
            FormDate = DateTime.Today;
        }

        public static bool GetCaclculationParams(ref string comment, ref DateTime formDate)
        {
            var form = new frmNewCalculate();
            if (form.ShowDialog() == DialogResult.OK)
            {
                comment = form.Comment;
                formDate = form.FormDate;
                return true;
            }
            return false;
        }

        private string Comment
        {
            get; set;
        }

        private DateTime FormDate
        {
            get; set;
        }

        private void Comment_TextChanged(object sender, EventArgs e)
        {
            Comment = CalcComment.Text;
            ultraButton2.Enabled = (FormDate != DateTime.MinValue && !string.IsNullOrEmpty(Comment));
        }

        private void FormDate_ValueChanged(object sender, EventArgs e)
        {
            if (Date.Value != null)
            {
                FormDate = Convert.ToDateTime(Date.Value);
                ultraButton2.Enabled = (FormDate != DateTime.MinValue && !string.IsNullOrEmpty(Comment));
            }
        }
    }
}
