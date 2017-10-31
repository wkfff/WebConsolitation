using System;
using System.Windows.Forms;

using Infragistics.Win.UltraWinEditors;


namespace Krista.FM.Client.ViewObjects.DataPumpUI
{
    public partial class ScheduleMonths : Form
    {
        delegate void SetUceCheckedDelegate(UltraCheckEditor uce, bool check);

        private SetUceCheckedDelegate setUceCheckedDelegate;
        private bool allMonthsClicked = true;
        private bool closeForm = true;


        public ScheduleMonths()
        {
            InitializeComponent();

            setUceCheckedDelegate = new SetUceCheckedDelegate(uce_SetChecked);
        }

        public void uce_SetChecked(UltraCheckEditor uce, bool check)
        {
            if (uce.Checked != check)
            {
                uce.Checked = check;
            }
        }

        private void SetCheckValue(UltraCheckEditor uce, bool check)
        {
            if (uce.Checked != check) uce.Invoke(setUceCheckedDelegate, new object[] { uce, check });
        }

        public void SetMonths(bool value)
        {
            SetCheckValue(uceApril, value);
            SetCheckValue(uceAugust, value);
            SetCheckValue(uceDecember, value);
            SetCheckValue(uceFebruary, value);
            SetCheckValue(uceJanuary, value);
            SetCheckValue(uceJuly, value);
            SetCheckValue(uceJune, value);
            SetCheckValue(uceMarch, value);
            SetCheckValue(uceMay, value);
            SetCheckValue(uceNovember, value);
            SetCheckValue(uceOctober, value);
            SetCheckValue(uceSeptember, value);
        }

        private void uceAllMonths_CheckedChanged(object sender, EventArgs e)
        {
            if (allMonthsClicked)
            {
                SetMonths(uceAllMonths.Checked);
            }
        }

        private void uceJanuary_CheckedChanged(object sender, EventArgs e)
        {
            UltraCheckEditor uce = (UltraCheckEditor)sender;
            if (!uce.Checked)
            {
                allMonthsClicked = false;
                SetCheckValue(uceAllMonths, false);
            }
        }

        private void uceAllMonths_Click(object sender, EventArgs e)
        {
            allMonthsClicked = true;
        }

        private void ubtnOk_Click(object sender, EventArgs e)
        {
            if (!uceApril.Checked && !uceAugust.Checked && !uceDecember.Checked && !uceFebruary.Checked &&
                !uceJanuary.Checked && !uceJuly.Checked && !uceJune.Checked && !uceMarch.Checked &&
                !uceMay.Checked && !uceNovember.Checked && !uceOctober.Checked && !uceSeptember.Checked)
            {
                closeForm = false;
                MessageBox.Show("Неправильное ежемесячное задание. Требуется выбрать один или несколько месяцев.",
                    "Расписание закачки", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                closeForm = true;
            }
        }

        private void ScheduleMonths_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !closeForm;
        }
    }
}