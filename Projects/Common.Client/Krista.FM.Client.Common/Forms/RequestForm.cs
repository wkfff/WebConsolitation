using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Krista.FM.Client.Common.Forms
{
    public partial class RequestForm : Form
    {
        public RequestForm()
        {
            InitializeComponent();
        }

        [Category("Internal controls")]
        [Description("Рабочая панель")]
        public Panel WorkPanel
        {
            get { return panel2; }
            set { }
        }

        [Category("Internal controls")]
        [Description("Надпись на кнопочке ОК")]
        public string OKButtotCaption
        {
            get { return btnOK.Text; }
            set { btnOK.Text = value; }
        }

        [Category("Internal controls")]
        [Description("Надпись на кнопочке Cancel")]
        public string CancelButtonCaption
        {
            get { return btnCancel.Text; }
            set { btnCancel.Text = value; }
        }

        [Category("Internal controls")]
        [Description("Надпись на форме")]
        public string FormCaption
        {
            get { return Text; }
            set { Text = value; }
        }
    }
}