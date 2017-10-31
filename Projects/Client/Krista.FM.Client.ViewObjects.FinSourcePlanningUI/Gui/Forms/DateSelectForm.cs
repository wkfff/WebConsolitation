using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Forms
{
    public partial class DateSelectForm : Form
    {
        public DateSelectForm()
        {
            InitializeComponent();
        }

        public static bool ShowDateForm(IWin32Window parent, ref DateTime calculateDate)
        {
            DateSelectForm form = new DateSelectForm();
            form.TimeEditor.Value = DateTime.Today;
            if (form.ShowDialog(parent) == DialogResult.OK)
            {
                calculateDate = Convert.ToDateTime(form.TimeEditor.Value);
                return true;
            }
            return false;
        }
    }
}