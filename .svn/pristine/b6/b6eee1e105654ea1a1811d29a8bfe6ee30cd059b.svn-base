using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;

using Krista.FM.Common;

namespace Krista.FM.Client.Design.Editors
{
    public partial class SystemInfoForm : Form
    {
        SystemInfo info;

        public SystemInfoForm()
        {
            InitializeComponent();

            info = new SystemInfo();

            DataTable table = info.GetInfo();
            this.ultraGrid1.DataSource = table;

            this.ultraGrid1.DisplayLayout.Bands[0].SortedColumns.Add("category", false, true);
            this.ultraGrid1.DisplayLayout.Rows.ExpandAll(true);
            this.ultraGrid1.Selected.Rows.Clear();
        }
    }

    public struct SystemInfoStruct
    {
        public static void Show()
        {
            SystemInfoForm systemInfoForm = new SystemInfoForm();

            systemInfoForm.Show();
        }
    }
}
