using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.Common.Forms
{
    public partial class FormPermissionException : Form
    {
        public FormPermissionException()
        {
            InitializeComponent();
        }

        public static void ShowErrorForm(PermissionException e)
        {
            FormPermissionException tmpFrm = new FormPermissionException();
            tmpFrm.laUser.Text = e.UserName;
            tmpFrm.laObject.Text = e.ObjectName;
            tmpFrm.laOperation.Text = e.Operation;
            tmpFrm.ShowDialog();
            tmpFrm.Dispose();
        }
    }
}