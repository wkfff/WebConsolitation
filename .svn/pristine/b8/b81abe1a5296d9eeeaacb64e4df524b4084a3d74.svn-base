using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Krista.FM.Client.Components
{
    public delegate void SelectDate (object sender, DateRangeEventArgs e);

    public partial class MonthCalendarDropDown : UserControl
    {
        public MonthCalendarDropDown()
        {
            InitializeComponent();
        }

        private SelectDate _OnSelectDate = null;
        [Category("Internal events")]
        [Description("¬ызываетс€ при выборе даты")]
        public event SelectDate OnSelectDate
        {
            add { _OnSelectDate += value; }
            remove { _OnSelectDate -= value; }
        }

        /// <summary>
        /// —ама€ нижн€€ панель, на случай внедрени€ куда-нибудь в рантайме
        /// </summary>
        [Category("Internal controls")]
        [Description("ќбъект - нижн€€ панель")]
        public Panel pnTemplate
        {
            get { return _pnTemplate; }
        }

        /// <summary>
        /// —ама€ нижн€€ панель, на случай внедрени€ куда-нибудь в рантайме
        /// </summary>
        [Category("Internal controls")]
        [Description("ќбъект - календарь")]
        public MonthCalendar Calendar
        {
            get { return monthCalendar1; }
        }

        private void monthCalendar1_DateSelected(object sender, DateRangeEventArgs e)
        {
            _SelectedDay = e.Start.Day;
            _SelectedMonth = e.Start.Month;
            _SelectedYear = e.Start.Year;
            if (_OnSelectDate != null)
                _OnSelectDate(sender, e);
        }

        public void SetMaxMinDate(DateTime MinData, DateTime MaxData)
        {
            this.monthCalendar1.MaxDate = MaxData;
            this.monthCalendar1.MinDate = MinData;
        }

        int _SelectedYear;
        /// <summary>
        /// √од в выбранной дате как целое
        /// </summary>
        [Category("Internal controls")]
        [Description("√од в выбранной дате")]
        public int SelectedYear
        {
            get { return _SelectedYear; }
        }

        int _SelectedMonth;
        /// <summary>
        /// мес€ц в выбранной дате как целое
        /// </summary>
        [Category("Internal controls")]
        [Description("мес€ц в выбранной дате")]
        public int SelectedMonth
        {
            get { return _SelectedMonth; }
        }

        int _SelectedDay;
        /// <summary>
        /// день в выбранной дате как целое
        /// </summary>
        [Category("Internal controls")]
        [Description("ƒень в выбранной дате")]
        public int SelectedDay
        {
            get { return _SelectedDay; }
        }

    }
}
