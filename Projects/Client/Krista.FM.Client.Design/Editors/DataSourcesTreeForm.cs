using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Krista.FM.ServerLibrary;
using Krista.FM.Client.Common.Controls;

namespace Krista.FM.Client.Design.Editors
{
    public partial class DataSourcesTreeForm : Form
    {
        private static DataSourcesTreeForm instance = null;

        public DataSourcesTreeForm()
        {
            InitializeComponent();
        }

        public DataSourcesTreeControl DataSourceControl
        {
            get
            {
                return dataSourcesTreeControl;
            }
        }

        public static DataSourcesTreeForm Instance
        {
            get
            {
                if (instance == null)
                    instance = new DataSourcesTreeForm();

                return instance;
            }
        }

        public static DataSourcesTreeForm InstanceForm(string dataKindCollection)
        {
            DataSourcesTreeForm form = Instance;
            form.DataSourceControl.InitializeCheckedNodes(dataKindCollection);

            form.DataSourceControl.Refresh();

            return form;
        }
    }
}