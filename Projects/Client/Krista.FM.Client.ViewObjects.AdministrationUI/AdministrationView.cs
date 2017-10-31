using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Krista.FM.Client.ViewObjects.AdministrationUI
{
    public partial class AdministrationView : Krista.FM.Client.ViewObjects.BaseViewObject.BaseView
    {
        public AdministrationView()
        {
            InitializeComponent();
        }

        public override string Caption
        {
            get { return Text; }
        }

    }
}
