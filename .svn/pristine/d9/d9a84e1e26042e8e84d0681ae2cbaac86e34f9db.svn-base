using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Krista.FM.Client.ViewObjects.MDObjectsManagementUI
{
    public partial class FormDatabaseConnection : Form
    {
        public FormDatabaseConnection()
        {
            InitializeComponent();
        }

        public DialogResult ShowOptions(IWin32Window owner, ref string serverName, ref string databaseName, ref string dataSourceName)
        {
            textBoxServerName.Text = serverName;
            textBoxDatabaseName.Text = databaseName;
            textBoxDataSourceName.Text = dataSourceName;
            DialogResult dialogResult = ShowDialog(owner);
            serverName = textBoxServerName.Text;
            databaseName = textBoxDatabaseName.Text;
            dataSourceName = textBoxDataSourceName.Text;
            return dialogResult;
        }
    }
}