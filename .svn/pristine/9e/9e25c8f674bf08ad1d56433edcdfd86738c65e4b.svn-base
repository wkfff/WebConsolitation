using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Krista.FM.Client.MDXExpert.Controls
{
    public partial class About : Form
    {
        public About(Form parentForm)
        {
            InitializeComponent();
            this.InitializeAbout();
        }

        private void InitializeAbout()
        {
            this.lbApplicationName.Text = Common.Consts.applicationNameWithNumber;
            this.lbDepartmentTelefons.Text = Common.Consts.departamentTelefons;
            this.lbApplicationVersion.Text = Common.Consts.applicationVersion;
        }

        private void btClose_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void laKristaHttpAddr_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.krista.ru");
        }

        private void laFmSupportHttpAddr_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string email = "mailto:" + laFmSupportHttpAddr.Text;
            System.Diagnostics.Process.Start(email);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://www.iminfin.ru");
        }

        private void pbKristaLogo_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.krista.ru");
        }

        private void pbiMinfinLogo_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.iminfin.ru");
        }
    }
}